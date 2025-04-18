using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RubyShellGenerator
{
    public partial class ClientForm : Form
    {
        private readonly Process _proc;
        private readonly int _port;

        public ClientForm(Process proc, int port)
        {
            _proc = proc ?? throw new ArgumentNullException(nameof(proc));
            _port = port;
            InitializeComponent();
            SetupProcess();
        }

        private void SetupProcess()
        {
            // Subscribe to process output and error events
            _proc.OutputDataReceived += (s, e) => {
                if (!string.IsNullOrEmpty(e.Data))
                    AppendOutput(e.Data);
            };
            _proc.ErrorDataReceived += (s, e) => {
                if (!string.IsNullOrEmpty(e.Data))
                    AppendOutput(e.Data);
            };
            // Note: BeginOutputReadLine is called in the main form
        }

        private void AppendOutput(string text)
        {
            if (txtOutput.InvokeRequired)
                txtOutput.Invoke(new Action(() => {
                    txtOutput.AppendText(text + Environment.NewLine);
                    txtOutput.ScrollToCaret();
                }));
            else
            {
                txtOutput.AppendText(text + Environment.NewLine);
                txtOutput.ScrollToCaret();
            }
        }

        private void SendCommand()
        {
            var cmd = txtInput.Text;
            if (!string.IsNullOrEmpty(cmd) && !_proc.HasExited)
            {
                _proc.StandardInput.WriteLine(cmd);
                AppendOutput($">> {cmd}");
                txtInput.Clear();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendCommand();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendCommand();
            }
        }
    }
}

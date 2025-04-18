namespace RubyShellGenerator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabPayload;
        private System.Windows.Forms.TabPage tabListener;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.TableLayoutPanel pnlSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.CheckBox chkSkipNetworkTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRegistryPath;
        private System.Windows.Forms.Button btnGeneratePayload;
        private System.Windows.Forms.DataGridView dgvPorts;
        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.TextBox txtListenerInput;
        private System.Windows.Forms.Button btnSendCommand;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPorts;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripClients;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // StatusStrip
            statusStrip = new System.Windows.Forms.StatusStrip();
            statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);
            statusStrip.Location = new System.Drawing.Point(0, 578);
            statusStrip.Size = new System.Drawing.Size(900, 22);

            // ProgressBar
            progressBar = new System.Windows.Forms.ProgressBar
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 20
            };

            // TabControl
            tabs = new System.Windows.Forms.TabControl { Dock = System.Windows.Forms.DockStyle.Fill };
            tabPayload = new System.Windows.Forms.TabPage("Payload");
            tabListener = new System.Windows.Forms.TabPage("Listener");
            tabLog = new System.Windows.Forms.TabPage("Log");
            tabs.TabPages.AddRange(new[] { tabPayload, tabListener, tabLog });

            // Payload Tab
            pnlSettings = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                AutoSize = true,
                Padding = new System.Windows.Forms.Padding(10),
                ColumnCount = 2,
                RowCount = 5
            };
            label1 = new System.Windows.Forms.Label { Text = "LHOST:", Anchor = System.Windows.Forms.AnchorStyles.Left };
            txtIP = new System.Windows.Forms.TextBox { Text = "127.0.0.1", Dock = System.Windows.Forms.DockStyle.Fill };
            label2 = new System.Windows.Forms.Label { Text = "LPORT:", Anchor = System.Windows.Forms.AnchorStyles.Left };
            txtPort = new System.Windows.Forms.TextBox { Text = "9001", Dock = System.Windows.Forms.DockStyle.Fill };
            chkSkipNetworkTest = new System.Windows.Forms.CheckBox { Text = "Skip bind test", Dock = System.Windows.Forms.DockStyle.Fill };
            label3 = new System.Windows.Forms.Label { Text = "Registry Path:", Anchor = System.Windows.Forms.AnchorStyles.Left };
            txtRegistryPath = new System.Windows.Forms.TextBox { Text = "HKCU:\\Software\\RubyShell", Dock = System.Windows.Forms.DockStyle.Fill };
            btnGeneratePayload = new System.Windows.Forms.Button { Text = "Generate Payload", Dock = System.Windows.Forms.DockStyle.Right };
            btnGeneratePayload.Click += btnGeneratePayload_Click;

            pnlSettings.Controls.Add(label1, 0, 0);
            pnlSettings.Controls.Add(txtIP, 1, 0);
            pnlSettings.Controls.Add(label2, 0, 1);
            pnlSettings.Controls.Add(txtPort, 1, 1);
            pnlSettings.Controls.Add(chkSkipNetworkTest, 1, 2);
            pnlSettings.Controls.Add(label3, 0, 3);
            pnlSettings.Controls.Add(txtRegistryPath, 1, 3);
            pnlSettings.Controls.Add(btnGeneratePayload, 1, 4);
            tabPayload.Controls.Add(pnlSettings);

            // Listener Tab
            dgvPorts = new System.Windows.Forms.DataGridView { Dock = System.Windows.Forms.DockStyle.Top, Height = 150, AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill };
            dgvPorts.Columns.Add("Port", "Port");
            dgvPorts.Columns.Add("Status", "Active");
            dgvPorts.Columns.Add("Connections", "Connections");

            dgvClients = new System.Windows.Forms.DataGridView { Dock = System.Windows.Forms.DockStyle.Top, Height = 150, AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill };
            dgvClients.Columns.Add("ClientIP", "Client IP");
            dgvClients.Columns.Add("ClientPort", "Port");
            dgvClients.Columns.Add("ClientStatus", "Status");
            dgvClients.Columns.Add("LastActive", "Last Active");

            txtListenerInput = new System.Windows.Forms.TextBox { Multiline = true, Height = 80, Dock = System.Windows.Forms.DockStyle.Top };
            btnSendCommand = new System.Windows.Forms.Button { Text = "Send to Selected", Dock = System.Windows.Forms.DockStyle.Top };
            btnSendCommand.Click += btnSendCommand_Click;

            tabListener.Controls.Add(btnSendCommand);
            tabListener.Controls.Add(txtListenerInput);
            tabListener.Controls.Add(dgvClients);
            tabListener.Controls.Add(dgvPorts);

            // Log Tab
            txtLog = new System.Windows.Forms.RichTextBox { Dock = System.Windows.Forms.DockStyle.Fill, ReadOnly = true };
            tabLog.Controls.Add(txtLog);

            // Context menus
            contextMenuStripPorts = new System.Windows.Forms.ContextMenuStrip(this.components);
            contextMenuStripPorts.Items.Add("Enable", null, this.portEnable_Click);
            contextMenuStripPorts.Items.Add("Disable", null, this.portDisable_Click);
            dgvPorts.ContextMenuStrip = contextMenuStripPorts;

            contextMenuStripClients = new System.Windows.Forms.ContextMenuStrip(this.components);
            contextMenuStripClients.Items.Add("Access", null, this.clientAccess_Click);
            contextMenuStripClients.Items.Add("Disconnect", null, this.clientDisconnect_Click);
            dgvClients.ContextMenuStrip = contextMenuStripClients;

            // Add controls to form
            this.Controls.Add(tabs);
            this.Controls.Add(progressBar);
            this.Controls.Add(statusStrip);
        }
        #endregion
    }
}

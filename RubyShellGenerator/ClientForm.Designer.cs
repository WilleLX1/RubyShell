namespace RubyShellGenerator
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnSend;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            // Panel Bottom
            panelBottom = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 60
            };

            // txtInput
            txtInput = new System.Windows.Forms.TextBox
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 10F),
            };
            txtInput.KeyDown += txtInput_KeyDown;

            // btnSend
            btnSend = new System.Windows.Forms.Button
            {
                Text = "Send",
                Dock = System.Windows.Forms.DockStyle.Right,
                Width = 80
            };
            btnSend.Click += btnSend_Click;

            // Add controls to bottom panel
            panelBottom.Controls.Add(txtInput);
            panelBottom.Controls.Add(btnSend);

            // txtOutput
            txtOutput = new System.Windows.Forms.RichTextBox
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10F),
                BackColor = System.Drawing.Color.Black,
                ForeColor = System.Drawing.Color.Lime
            };

            // ClientForm
            this.Controls.Add(txtOutput);
            this.Controls.Add(panelBottom);
            this.Text = $"Client Console - Port {_port}";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        }

        #endregion
    }
}

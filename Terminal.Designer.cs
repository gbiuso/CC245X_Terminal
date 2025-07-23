namespace JDY23Terminal {
    partial class CC245XTerminal {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            txt_dataTX = new TextBox();
            btn_send = new Button();
            menu_BLE = new MenuStrip();
            bLEToolStripMenuItem = new ToolStripMenuItem();
            scanToolStripMenuItem = new ToolStripMenuItem();
            devicesToolStripMenuItem = new ToolStripMenuItem();
            disconnectToolStripMenuItem = new ToolStripMenuItem();
            btn_clear = new Button();
            rtx_dataRX = new RichTextBox();
            btn_copy = new Button();
            chk_autoScroll = new CheckBox();
            menu_BLE.SuspendLayout();
            SuspendLayout();
            // 
            // txt_dataTX
            // 
            txt_dataTX.Location = new Point(12, 27);
            txt_dataTX.Name = "txt_dataTX";
            txt_dataTX.Size = new Size(532, 23);
            txt_dataTX.TabIndex = 0;
            txt_dataTX.TextChanged += txt_dataTX_TextChanged;
            txt_dataTX.KeyPress += txt_dataTX_KeyPress;
            // 
            // btn_send
            // 
            btn_send.Enabled = false;
            btn_send.Location = new Point(550, 27);
            btn_send.Name = "btn_send";
            btn_send.Size = new Size(60, 23);
            btn_send.TabIndex = 1;
            btn_send.Text = "Send";
            btn_send.UseVisualStyleBackColor = true;
            btn_send.Click += btn_send_Click;
            // 
            // menu_BLE
            // 
            menu_BLE.Items.AddRange(new ToolStripItem[] { bLEToolStripMenuItem });
            menu_BLE.Location = new Point(0, 0);
            menu_BLE.Name = "menu_BLE";
            menu_BLE.Size = new Size(807, 24);
            menu_BLE.TabIndex = 3;
            menu_BLE.Text = "BLE menu";
            // 
            // bLEToolStripMenuItem
            // 
            bLEToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { scanToolStripMenuItem, devicesToolStripMenuItem, disconnectToolStripMenuItem });
            bLEToolStripMenuItem.Name = "bLEToolStripMenuItem";
            bLEToolStripMenuItem.Size = new Size(38, 20);
            bLEToolStripMenuItem.Text = "BLE";
            // 
            // scanToolStripMenuItem
            // 
            scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            scanToolStripMenuItem.Size = new Size(133, 22);
            scanToolStripMenuItem.Text = "Scan";
            scanToolStripMenuItem.Click += scanToolStripMenuItem_Click;
            // 
            // devicesToolStripMenuItem
            // 
            devicesToolStripMenuItem.Enabled = false;
            devicesToolStripMenuItem.Name = "devicesToolStripMenuItem";
            devicesToolStripMenuItem.Size = new Size(133, 22);
            devicesToolStripMenuItem.Text = "Connect";
            // 
            // disconnectToolStripMenuItem
            // 
            disconnectToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            disconnectToolStripMenuItem.Size = new Size(133, 22);
            disconnectToolStripMenuItem.Text = "Disconnect";
            disconnectToolStripMenuItem.Click += disconnectToolStripMenuItem_Click;
            // 
            // btn_clear
            // 
            btn_clear.Location = new Point(743, 25);
            btn_clear.Name = "btn_clear";
            btn_clear.Size = new Size(60, 23);
            btn_clear.TabIndex = 4;
            btn_clear.Text = "Clear";
            btn_clear.UseVisualStyleBackColor = true;
            btn_clear.Click += btnClear_Click;
            // 
            // rtx_dataRX
            // 
            rtx_dataRX.ForeColor = SystemColors.WindowText;
            rtx_dataRX.HideSelection = false;
            rtx_dataRX.Location = new Point(11, 56);
            rtx_dataRX.Name = "rtx_dataRX";
            rtx_dataRX.ReadOnly = true;
            rtx_dataRX.Size = new Size(792, 388);
            rtx_dataRX.TabIndex = 5;
            rtx_dataRX.Text = "";
            rtx_dataRX.MouseDown += rtx_dataRX_MouseDown;
            // 
            // btn_copy
            // 
            btn_copy.Location = new Point(677, 25);
            btn_copy.Name = "btn_copy";
            btn_copy.Size = new Size(60, 23);
            btn_copy.TabIndex = 6;
            btn_copy.Text = "Copy";
            btn_copy.UseVisualStyleBackColor = true;
            btn_copy.Click += btnCopy_Click;
            // 
            // chk_autoScroll
            // 
            chk_autoScroll.AutoSize = true;
            chk_autoScroll.Location = new Point(616, 29);
            chk_autoScroll.Name = "chk_autoScroll";
            chk_autoScroll.Size = new Size(55, 19);
            chk_autoScroll.TabIndex = 7;
            chk_autoScroll.Text = "Scroll";
            chk_autoScroll.UseVisualStyleBackColor = true;
            chk_autoScroll.CheckedChanged += chk_autoScroll_CheckedChanged;
            // 
            // CC245XTerminal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 446);
            Controls.Add(chk_autoScroll);
            Controls.Add(btn_copy);
            Controls.Add(rtx_dataRX);
            Controls.Add(btn_clear);
            Controls.Add(btn_send);
            Controls.Add(txt_dataTX);
            Controls.Add(menu_BLE);
            MainMenuStrip = menu_BLE;
            MinimumSize = new Size(300, 150);
            Name = "CC245XTerminal";
            Text = "CC245X Terminal";
            Load += CC245XTerminal_Load;
            Layout += CC245XTerminal_Layout;
            menu_BLE.ResumeLayout(false);
            menu_BLE.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txt_dataTX;
        private Button btn_send;
        private MenuStrip menu_BLE;
        private ToolStripMenuItem bLEToolStripMenuItem;
        private ToolStripMenuItem scanToolStripMenuItem;
        private Button btn_clear;
        private ToolStripMenuItem devicesToolStripMenuItem;
        private ToolStripMenuItem disconnectToolStripMenuItem;
        private RichTextBox rtx_dataRX;
        private Button btn_copy;
        private CheckBox chk_autoScroll;
    }
}

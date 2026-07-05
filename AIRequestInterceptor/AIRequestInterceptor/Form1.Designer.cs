namespace AIRequestInterceptor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            btnStartStop = new Button();
            lblStatus = new Label();
            chkAutoPass = new CheckBox();
            lstHistory = new ListBox();
            lblPrompt = new Label();
            txtPrompt = new TextBox();
            lblTemperature = new Label();
            txtTemperature = new TextBox();
            lblRawJson = new Label();
            txtRawJson = new TextBox();
            btnPass = new Button();
            btnPassOriginal = new Button();
            lblHistory = new Label();
            panelTop = new Panel();
            panelTop.SuspendLayout();
            SuspendLayout();
            // 
            // btnStartStop
            // 
            btnStartStop.BackColor = Color.LightGreen;
            btnStartStop.Font = new Font("Microsoft YaHei UI", 10F);
            btnStartStop.Location = new Point(31, 21);
            btnStartStop.Margin = new Padding(5, 4, 5, 4);
            btnStartStop.Name = "btnStartStop";
            btnStartStop.Size = new Size(236, 56);
            btnStartStop.TabIndex = 0;
            btnStartStop.Text = "▶ 启动代理";
            btnStartStop.UseVisualStyleBackColor = false;
            btnStartStop.Click += btnStartStop_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft YaHei UI", 10F);
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(291, 35);
            lblStatus.Margin = new Padding(5, 0, 5, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(132, 27);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "状态：未启动";
            // 
            // chkAutoPass
            // 
            chkAutoPass.AutoSize = true;
            chkAutoPass.Checked = true;
            chkAutoPass.CheckState = CheckState.Checked;
            chkAutoPass.Font = new Font("Microsoft YaHei UI", 9F);
            chkAutoPass.Location = new Point(629, 35);
            chkAutoPass.Margin = new Padding(5, 4, 5, 4);
            chkAutoPass.Name = "chkAutoPass";
            chkAutoPass.Size = new Size(306, 28);
            chkAutoPass.TabIndex = 2;
            chkAutoPass.Text = "自动放行（不拦截，只记录历史）";
            chkAutoPass.UseVisualStyleBackColor = true;
            // 
            // lstHistory
            // 
            lstHistory.Font = new Font("Microsoft YaHei UI", 9F);
            lstHistory.FormattingEnabled = true;
            lstHistory.ItemHeight = 24;
            lstHistory.Location = new Point(31, 155);
            lstHistory.Margin = new Padding(5, 4, 5, 4);
            lstHistory.Name = "lstHistory";
            lstHistory.Size = new Size(548, 916);
            lstHistory.TabIndex = 4;
            lstHistory.SelectedIndexChanged += lstHistory_SelectedIndexChanged;
            // 
            // lblPrompt
            // 
            lblPrompt.AutoSize = true;
            lblPrompt.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblPrompt.Location = new Point(613, 124);
            lblPrompt.Margin = new Padding(5, 0, 5, 0);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(153, 25);
            lblPrompt.TabIndex = 5;
            lblPrompt.Text = "📝 Prompt 内容";
            // 
            // txtPrompt
            // 
            txtPrompt.Font = new Font("Microsoft YaHei UI", 10F);
            txtPrompt.Location = new Point(613, 155);
            txtPrompt.Margin = new Padding(5, 4, 5, 4);
            txtPrompt.Multiline = true;
            txtPrompt.Name = "txtPrompt";
            txtPrompt.ReadOnly = true;
            txtPrompt.ScrollBars = ScrollBars.Both;
            txtPrompt.Size = new Size(1208, 422);
            txtPrompt.TabIndex = 6;
            // 
            // lblTemperature
            // 
            lblTemperature.AutoSize = true;
            lblTemperature.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblTemperature.Location = new Point(613, 600);
            lblTemperature.Margin = new Padding(5, 0, 5, 0);
            lblTemperature.Name = "lblTemperature";
            lblTemperature.Size = new Size(159, 25);
            lblTemperature.TabIndex = 7;
            lblTemperature.Text = "🌡️ Temperature";
            // 
            // txtTemperature
            // 
            txtTemperature.Font = new Font("Microsoft YaHei UI", 10F);
            txtTemperature.Location = new Point(833, 593);
            txtTemperature.Margin = new Padding(5, 4, 5, 4);
            txtTemperature.Name = "txtTemperature";
            txtTemperature.ReadOnly = true;
            txtTemperature.Size = new Size(155, 33);
            txtTemperature.TabIndex = 8;
            txtTemperature.Text = "0.7";
            // 
            // lblRawJson
            // 
            lblRawJson.AutoSize = true;
            lblRawJson.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblRawJson.Location = new Point(613, 649);
            lblRawJson.Margin = new Padding(5, 0, 5, 0);
            lblRawJson.Name = "lblRawJson";
            lblRawJson.Size = new Size(205, 25);
            lblRawJson.TabIndex = 9;
            lblRawJson.Text = "📄 原始 JSON（只读）";
            // 
            // txtRawJson
            // 
            txtRawJson.BackColor = SystemColors.ControlLight;
            txtRawJson.Font = new Font("Consolas", 9F);
            txtRawJson.Location = new Point(613, 685);
            txtRawJson.Margin = new Padding(5, 4, 5, 4);
            txtRawJson.Multiline = true;
            txtRawJson.Name = "txtRawJson";
            txtRawJson.ReadOnly = true;
            txtRawJson.ScrollBars = ScrollBars.Both;
            txtRawJson.Size = new Size(1208, 295);
            txtRawJson.TabIndex = 10;
            // 
            // btnPass
            // 
            btnPass.BackColor = Color.LightGreen;
            btnPass.Enabled = false;
            btnPass.Font = new Font("Microsoft YaHei UI", 10F);
            btnPass.Location = new Point(613, 1002);
            btnPass.Margin = new Padding(5, 4, 5, 4);
            btnPass.Name = "btnPass";
            btnPass.Size = new Size(251, 56);
            btnPass.TabIndex = 11;
            btnPass.Text = "✅ 修改后放行";
            btnPass.UseVisualStyleBackColor = false;
            btnPass.Click += btnPass_Click;
            // 
            // btnPassOriginal
            // 
            btnPassOriginal.BackColor = Color.LightYellow;
            btnPassOriginal.Enabled = false;
            btnPassOriginal.Font = new Font("Microsoft YaHei UI", 10F);
            btnPassOriginal.Location = new Point(896, 1002);
            btnPassOriginal.Margin = new Padding(5, 4, 5, 4);
            btnPassOriginal.Name = "btnPassOriginal";
            btnPassOriginal.Size = new Size(251, 56);
            btnPassOriginal.TabIndex = 12;
            btnPassOriginal.Text = "⏭️ 原始放行";
            btnPassOriginal.UseVisualStyleBackColor = false;
            btnPassOriginal.Click += btnPassOriginal_Click;
            // 
            // lblHistory
            // 
            lblHistory.AutoSize = true;
            lblHistory.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblHistory.Location = new Point(31, 124);
            lblHistory.Margin = new Padding(5, 0, 5, 0);
            lblHistory.Name = "lblHistory";
            lblHistory.Size = new Size(115, 25);
            lblHistory.TabIndex = 3;
            lblHistory.Text = "📋 历史记录";
            // 
            // panelTop
            // 
            panelTop.Controls.Add(btnStartStop);
            panelTop.Controls.Add(lblStatus);
            panelTop.Controls.Add(chkAutoPass);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Margin = new Padding(5, 4, 5, 4);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1861, 113);
            panelTop.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1861, 1074);
            Controls.Add(btnPassOriginal);
            Controls.Add(btnPass);
            Controls.Add(txtRawJson);
            Controls.Add(lblRawJson);
            Controls.Add(txtTemperature);
            Controls.Add(lblTemperature);
            Controls.Add(txtPrompt);
            Controls.Add(lblPrompt);
            Controls.Add(lstHistory);
            Controls.Add(lblHistory);
            Controls.Add(panelTop);
            Font = new Font("Microsoft YaHei UI", 9F);
            Margin = new Padding(5, 4, 5, 4);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AI 请求拦截器 - 崇祯专用";
            FormClosing += Form1_FormClosing;
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkAutoPass;
        private System.Windows.Forms.ListBox lstHistory;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.TextBox txtPrompt;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.TextBox txtTemperature;
        private System.Windows.Forms.Label lblRawJson;
        private System.Windows.Forms.TextBox txtRawJson;
        private System.Windows.Forms.Button btnPass;
        private System.Windows.Forms.Button btnPassOriginal;
        private System.Windows.Forms.Label lblHistory;
        private System.Windows.Forms.Panel panelTop;
    }
}
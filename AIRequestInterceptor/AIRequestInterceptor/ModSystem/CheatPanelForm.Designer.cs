namespace AIRequestInterceptor.ModSystem
{
    partial class CheatPanelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            buttonApply = new Button();
            buttonOpenModsFolder = new Button();
            checkedListBoxMods = new CheckedListBox();
            panel2 = new Panel();
            flowLayoutPanelConflicts = new FlowLayoutPanel();
            labelConflicts = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(6);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new Size(1467, 1000);
            splitContainer1.SplitterDistance = 916;
            splitContainer1.SplitterWidth = 7;
            splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(buttonApply);
            panel1.Controls.Add(buttonOpenModsFolder);
            panel1.Controls.Add(checkedListBoxMods);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(6);
            panel1.Name = "panel1";
            panel1.Size = new Size(916, 1000);
            panel1.TabIndex = 0;
            // 
            // buttonApply
            // 
            buttonApply.Dock = DockStyle.Bottom;
            buttonApply.Location = new Point(0, 800);
            buttonApply.Margin = new Padding(6);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(916, 100);
            buttonApply.TabIndex = 2;
            buttonApply.Text = "✅ 应用并关闭";
            buttonApply.UseVisualStyleBackColor = true;
            buttonApply.Click += buttonApply_Click;
            // 
            // buttonOpenModsFolder
            // 
            buttonOpenModsFolder.Dock = DockStyle.Bottom;
            buttonOpenModsFolder.Location = new Point(0, 900);
            buttonOpenModsFolder.Margin = new Padding(6);
            buttonOpenModsFolder.Name = "buttonOpenModsFolder";
            buttonOpenModsFolder.Size = new Size(916, 100);
            buttonOpenModsFolder.TabIndex = 1;
            buttonOpenModsFolder.Text = "📁 打开 Mod 文件夹";
            buttonOpenModsFolder.UseVisualStyleBackColor = true;
            buttonOpenModsFolder.Click += buttonOpenModsFolder_Click;
            // 
            // checkedListBoxMods
            // 
            checkedListBoxMods.Dock = DockStyle.Fill;
            checkedListBoxMods.FormattingEnabled = true;
            checkedListBoxMods.Location = new Point(0, 0);
            checkedListBoxMods.Margin = new Padding(6);
            checkedListBoxMods.Name = "checkedListBoxMods";
            checkedListBoxMods.Size = new Size(916, 1000);
            checkedListBoxMods.TabIndex = 0;
            checkedListBoxMods.ItemCheck += checkedListBoxMods_ItemCheck;
            // 
            // panel2
            // 
            panel2.Controls.Add(flowLayoutPanelConflicts);
            panel2.Controls.Add(labelConflicts);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(6);
            panel2.Name = "panel2";
            panel2.Size = new Size(544, 1000);
            panel2.TabIndex = 0;
            // 
            // flowLayoutPanelConflicts
            // 
            flowLayoutPanelConflicts.AutoScroll = true;
            flowLayoutPanelConflicts.Dock = DockStyle.Fill;
            flowLayoutPanelConflicts.Location = new Point(0, 49);
            flowLayoutPanelConflicts.Margin = new Padding(6);
            flowLayoutPanelConflicts.Name = "flowLayoutPanelConflicts";
            flowLayoutPanelConflicts.Size = new Size(544, 951);
            flowLayoutPanelConflicts.TabIndex = 2;
            // 
            // labelConflicts
            // 
            labelConflicts.AutoSize = true;
            labelConflicts.Dock = DockStyle.Top;
            labelConflicts.Font = new Font("微软雅黑", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            labelConflicts.Location = new Point(0, 24);
            labelConflicts.Margin = new Padding(6, 0, 6, 0);
            labelConflicts.Name = "labelConflicts";
            labelConflicts.Size = new Size(97, 25);
            labelConflicts.TabIndex = 1;
            labelConflicts.Text = "✅ 无冲突";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(0, 0);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(158, 24);
            label1.TabIndex = 0;
            label1.Text = "⚠️ Mod 冲突详情";
            // 
            // CheatPanelForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1467, 1000);
            Controls.Add(splitContainer1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CheatPanelForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "⚡ 金手指面板";
            Load += CheatPanelForm_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckedListBox checkedListBoxMods;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelConflicts;
        private System.Windows.Forms.Label labelConflicts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonOpenModsFolder;
    }
}
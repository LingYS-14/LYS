using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AIRequestInterceptor.ModSystem
{
    public partial class ModConflictDialog : Form
    {
        public ModConflict Conflict { get; private set; }
        public string SelectedResolution { get; private set; }

        public ModConflictDialog(ModConflict conflict)
        {
            InitializeComponent();
            Conflict = conflict;
            SetupUI();
        }

        private void SetupUI()
        {
            Text = $"⚠️ 检测到 Mod 冲突: {Conflict.ConflictType}";

            // 标题
            labelTitle.Text = $"【{Conflict.ConflictType}】冲突: {Conflict.Target}";

            // 冲突描述
            string description = $"以下 Mod 尝试修改同一个 {Conflict.ConflictType}:\n\n";
            for (int i = 0; i < Conflict.ModNames.Count; i++)
            {
                description += $"  • {Conflict.ModNames[i]}\n";
            }
            description += "\n请选择保留哪个 Mod 的设置:";
            labelDescription.Text = description;

            // 添加选项按钮
            flowLayoutPanel.Controls.Clear();
            foreach (var option in Conflict.ResolutionOptions)
            {
                Button btn = new Button
                {
                    Text = option,
                    Dock = DockStyle.Top,
                    Margin = new Padding(5),
                    Height = 35
                };
                btn.Click += (s, e) =>
                {
                    SelectedResolution = option;
                    DialogResult = DialogResult.OK;
                    Close();
                };
                flowLayoutPanel.Controls.Add(btn);
            }
        }

        private void ModConflictDialog_Load(object sender, EventArgs e)
        {
            // 确保窗口始终在最前面
            TopMost = true;
        }
    }
}
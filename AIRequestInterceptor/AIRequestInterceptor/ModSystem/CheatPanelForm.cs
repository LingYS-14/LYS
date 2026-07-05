using AIRequestInterceptor.ModSystem; // 替换为你的命名空间
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AIRequestInterceptor.ModSystem
{
    public partial class CheatPanelForm : Form
    {
        public CheatPanelForm()
        {
            InitializeComponent();
            LoadMods();
        }

        private void LoadMods()
        {
            // 强制刷新日志
            
            ModManager.LoadAllMods();
            RefreshModList();
            ModManager.DetectConflicts();
            ShowConflicts();
        }

        private void RefreshModList()
        {
            checkedListBoxMods.Items.Clear();

            // 【关键】添加这行！确保 UI 重绘
            checkedListBoxMods.BeginUpdate();

            foreach (var mod in ModManager.AllMods)
            {
                checkedListBoxMods.Items.Add(mod, mod.IsEnabled);
            }

            checkedListBoxMods.EndUpdate(); // 结束更新
        }

        private void ShowConflicts()
        {
            if (ModManager.Conflicts.Count == 0)
            {
                labelConflicts.Text = "✅ 无冲突";
                labelConflicts.ForeColor = Color.Green;
                return;
            }

            labelConflicts.Text = $"⚠️ 检测到 {ModManager.Conflicts.Count} 个冲突";
            labelConflicts.ForeColor = Color.Red;

            // 清理之前的冲突详情
            flowLayoutPanelConflicts.Controls.Clear();

            // 显示冲突详情
            foreach (var conflict in ModManager.Conflicts)
            {
                Label conflictLabel = new Label
                {
                    Text = $"• {conflict.ConflictType}: {conflict.Target} (涉及 {conflict.ModNames.Count} 个 Mod)",
                    AutoSize = true,
                    Margin = new Padding(0, 5, 0, 0),
                    ForeColor = Color.Red
                };
                flowLayoutPanelConflicts.Controls.Add(conflictLabel);
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            // 保存 Mod 启用状态
            for (int i = 0; i < checkedListBoxMods.Items.Count; i++)
            {
                var mod = ModManager.AllMods[i];
                mod.IsEnabled = checkedListBoxMods.GetItemChecked(i);
            }

            // 检测冲突
            ModManager.DetectConflicts();

            // 如果有冲突，提示解决
            if (ModManager.Conflicts.Count > 0)
            {
                MessageBox.Show(
                    "检测到 Mod 冲突！请解决冲突后再应用。" +
                    "\n\n冲突详情已显示在下方，" +
                    "请在勾选 Mod 时注意红色警告。",
                    "⚠️ Mod 冲突",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                ShowConflicts();
                return;
            }

            // 生成最终配置
            ModManager.GenerateActiveProfile();
            ModManager.SaveModStates();

            MessageBox.Show("✅ Mod 配置已应用！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void buttonOpenModsFolder_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 获取正确路径（使用 GetExecutingAssembly 代替 BaseDirectory）
                string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string modsFolder = Path.Combine(exePath, "Mods");

                // 2. 创建文件夹（如果不存在）
                if (!Directory.Exists(modsFolder))
                {
                    Directory.CreateDirectory(modsFolder);
                }

                // 3. 安全打开文件夹（关键修复！）
                Process.Start(new ProcessStartInfo
                {
                    FileName = modsFolder,
                    UseShellExecute = true,
                    Verb = "open" // 明确指定打开操作
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开文件夹失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkedListBoxMods_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 临时修改状态以检测冲突
            var mod = ModManager.AllMods[e.Index];
            mod.IsEnabled = e.NewValue == CheckState.Checked;

            // 检测冲突
            ModManager.DetectConflicts();
            ShowConflicts();

            // 恢复原状态（因为实际应用需要点击"应用"按钮）
            mod.IsEnabled = e.CurrentValue == CheckState.Checked;
        }

        private void CheatPanelForm_Load(object sender, EventArgs e)
        {
            // 确保窗口始终在最前面
            TopMost = true;
        }
    }
}
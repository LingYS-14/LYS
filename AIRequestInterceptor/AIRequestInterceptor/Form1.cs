using AIRequestInterceptor.ModSystem; // 👈 必须加上这一行！
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace AIRequestInterceptor
{
    public partial class Form1 : Form
    {
        // ============ 代理服务器核心组件 ============
        private ProxyServer proxyServer;
        private ExplicitProxyEndPoint explicitEndPoint;
        private bool isRunning = false;

        // ============ 拦截相关 ============
        // 存储等待用户处理的请求
        private ConcurrentDictionary<string, PendingRequest> pendingRequests
            = new ConcurrentDictionary<string, PendingRequest>();

        // 当前正在查看的请求ID
        private string currentRequestId = null;

        // ============ 历史记录 ============
        private List<HistoryRecord> historyList = new List<HistoryRecord>();

        // ============ 配置 ============
        private const int PROXY_PORT = 8888;
        // 需要拦截的域名列表（可以在这里添加更多）
        private readonly string[] TARGET_DOMAINS = new string[]
        {
            "api.deepseek.com",
            "api.openai.com",
            "dashscope.aliyuncs.com",  // 通义千问
            "localhost",                // 本地 Ollama
            "127.0.0.1"                 // 本地 Ollama
        };

        public Form1()
        {
            InitializeComponent();
            InitializeProxy();

            // 👇 添加这一行，用于初始化金手指按钮
            InitializeCheatPanelButton();
        }

        /// <summary>
        /// 动态在顶部面板添加“金手指面板”按钮
        /// </summary>
        private void InitializeCheatPanelButton()
        {
            Button btnCheatPanel = new Button
            {
                Text = "⚡ 金手指面板",
                Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold),
                BackColor = Color.Gold,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 56),
                // 靠右对齐，距离右边距 20
                Location = new Point(panelTop.Width - 220, 21),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnCheatPanel.Click += (s, e) =>
            {
                // 打开金手指面板
                using (var form = new AIRequestInterceptor.ModSystem.CheatPanelForm())
                {
                    form.ShowDialog(this);
                }
            };

            panelTop.Controls.Add(btnCheatPanel);
        }

        /// <summary>
        /// 初始化代理服务器
        /// </summary>
        private void InitializeProxy()
        {
            proxyServer = new ProxyServer();

            // 配置证书（用于 HTTPS 解密）
            proxyServer.CertificateManager.EnsureRootCertificate();

            // 创建代理端点
            explicitEndPoint = new ExplicitProxyEndPoint(
                System.Net.IPAddress.Loopback, PROXY_PORT, true);

            // 绑定事件
            proxyServer.BeforeRequest += OnBeforeRequest;
            proxyServer.BeforeResponse += OnBeforeResponse;

            proxyServer.AddEndPoint(explicitEndPoint);
        }

        /// <summary>
        /// 请求到达代理时触发（在发送给服务器之前）
        /// </summary>
        /// <summary>
        /// 请求到达代理时触发（在发送给服务器之前）
        /// </summary>
        private async Task OnBeforeRequest(object sender, SessionEventArgs e)
        {
            try
            {
                string url = e.HttpClient.Request.Url;
                string host = e.HttpClient.Request.RequestUri.Host;

                // ========== 1. 检查是否是目标域名 ==========
                // 【修复点】：在这里正确声明 isTarget 变量
                bool isTarget = TARGET_DOMAINS.Any(d =>
                    host.Contains(d, StringComparison.OrdinalIgnoreCase));

                if (!isTarget || e.HttpClient.Request.Method != "POST")
                {
                    return; // 非目标请求，直接放行
                }

                // ========== 2. 检查 Content-Type ==========
                var contentType = e.HttpClient.Request.Headers.GetFirstHeader("Content-Type");
                if (contentType == null ||
                    !(contentType.Value.Contains("application/json") || contentType.Value.Contains("text")))
                {
                    return; // 非 JSON/文本请求，直接放行
                }

                // ========== 3. 读取请求体 ==========
                string rawString = await e.GetRequestBodyAsString();
                string body = "";
                if (e.HttpClient.Request.Body != null && e.HttpClient.Request.Body.Length > 0)
                {
                    body = Encoding.UTF8.GetString(e.HttpClient.Request.Body);
                }
                else
                {
                    body = rawString;
                }

                if (string.IsNullOrEmpty(body))
                {
                    return; // 没有请求体，直接放行
                }

                // ========== 🌟 4. Mod 注入核心逻辑 🌟 ==========
                // 【修复点】：确保 ModManager 已被正确引用（顶部加了 using）
                if (ModManager.ActiveProfile != null &&
                    (ModManager.ActiveProfile.SystemPromptOverrides.Count > 0 ||
                     ModManager.ActiveProfile.Parameters.Count > 0 ||
                     ModManager.ActiveProfile.DatabaseInjections.Count > 0 ||
                     ModManager.ActiveProfile.NpcPersonas.Count > 0 ||
                     ModManager.ActiveProfile.DeductionContexts.Count > 0))
                {
                    try
                    {
                        // 尝试应用 Mod 注入
                        string injectedBody = ModSystem.InjectionEngine.ApplyModProfile(body, ModManager.ActiveProfile);

                        if (injectedBody != body)
                        {
                            // 注入成功，替换请求体
                            e.SetRequestBody(Encoding.UTF8.GetBytes(injectedBody));
                            body = injectedBody; // 更新 body 变量供后续显示
                            System.Diagnostics.Debug.WriteLine($"✅ [Mod] 已成功注入请求: {host}");
                        }
                    }
                    catch (Exception modEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ [Mod] 注入失败，使用原始请求: {modEx.Message}");
                    }
                }
                // ==========================================

                // ========== 5. 记录历史与 UI 交互 ==========
                string requestId = Guid.NewGuid().ToString("N")[..8];
                string timestamp = DateTime.Now.ToString("HH:mm:ss");

                var record = new HistoryRecord
                {
                    Id = requestId,
                    Timestamp = timestamp,
                    Url = url,
                    OriginalBody = body,
                    ModifiedBody = body
                };

                ParseRequestBody(body, out string prompt, out double temperature);
                record.Prompt = prompt;
                record.Temperature = temperature;

                historyList.Add(record);

                bool autoPass = false;
                this.Invoke(new Action(() =>
                {
                    autoPass = chkAutoPass.Checked;
                    lstHistory.Items.Insert(0, $"[{timestamp}] [{requestId}] {host}");
                }));

                if (!autoPass)
                {
                    var pending = new PendingRequest
                    {
                        Session = e,
                        Record = record,
                        WaitHandle = new ManualResetEventSlim(false)
                    };
                    pendingRequests[requestId] = pending;

                    this.Invoke(new Action(() =>
                    {
                        ShowRequestInUI(record);
                        currentRequestId = requestId;
                        btnPass.Enabled = true;
                        btnPassOriginal.Enabled = true;
                        txtPrompt.ReadOnly = false;
                        txtTemperature.ReadOnly = false;

                        lblStatus.Text = $"状态：⏸ 等待处理 [{requestId}]";
                        lblStatus.ForeColor = Color.OrangeRed;
                    }));

                    pending.WaitHandle.Wait();

                    string modifiedBody = pending.ModifiedBody;

                    if (modifiedBody != body)
                    {
                        try
                        {
                            e.SetRequestBody(Encoding.UTF8.GetBytes(modifiedBody));
                        }
                        catch
                        {
                            e.SetRequestBodyString(modifiedBody);
                        }
                    }

                    record.ModifiedBody = modifiedBody;
                    pendingRequests.TryRemove(requestId, out _);
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        lblStatus.Text = $"状态：✅ 自动放行 [{requestId}]";
                        lblStatus.ForeColor = Color.Green;
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"代理拦截出错，已自动放行: {ex.Message}");
            }
        }

        /// <summary>
        /// 响应返回时触发（我们不需要修改响应，只记录）
        /// </summary>
        private static async Task OnBeforeResponse(object sender, SessionEventArgs e)
        {
            // 不拦截响应，直接放行
            await Task.CompletedTask;
        }

        /// <summary>
        /// 解析 AI 请求的 JSON Body，提取 Prompt 和 Temperature
        /// </summary>
        private static void ParseRequestBody(string body, out string prompt, out double temperature)
        {
            prompt = "";
            temperature = 0.7; // 默认值

            try
            {
                JObject json = JObject.Parse(body);

                // 提取 temperature
                if (json["temperature"] != null)
                {
                    temperature = json["temperature"].Value<double>();
                }

                // 提取 messages 数组中的最后一条 user message 作为 prompt
                if (json["messages"] != null)
                {
                    var messages = json["messages"] as JArray;
                    if (messages != null && messages.Count > 0)
                    {
                        // 获取所有 user 角色的消息
                        var userMessages = messages
                            .Where(m => m["role"]?.ToString() == "user")
                            .ToList();

                        if (userMessages.Count > 0)
                        {
                            var lastUserMsg = userMessages.Last();
                            prompt = lastUserMsg["content"]?.ToString() ?? "";
                        }
                        else
                        {
                            // 如果没有 user 消息，显示所有消息
                            var sb = new StringBuilder();
                            foreach (var msg in messages)
                            {
                                string role = msg["role"]?.ToString() ?? "unknown";
                                string content = msg["content"]?.ToString() ?? "";
                                sb.AppendLine($"[{role}]: {content}");
                                sb.AppendLine("---");
                            }
                            prompt = sb.ToString();
                        }
                    }
                }
                else if (json["prompt"] != null)
                {
                    // 某些 API 使用 prompt 字段
                    prompt = json["prompt"].ToString();
                }
            }
            catch (Exception ex)
            {
                prompt = $"[解析失败] {ex.Message}\n\n原始内容:\n{body}";
            }
        }

        /// <summary>
        /// 在界面上显示请求内容
        /// </summary>
        private void ShowRequestInUI(HistoryRecord record)
        {
            txtPrompt.Text = record.Prompt;
            txtTemperature.Text = record.Temperature.ToString();
            txtRawJson.Text = TryFormatJson(record.OriginalBody);
        }

        /// <summary>
        /// 尝试格式化 JSON 字符串（美化显示）
        /// </summary>
        private static string TryFormatJson(string json)
        {
            try
            {
                var obj = JObject.Parse(json);
                return obj.ToString(Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        /// <summary>
        /// 根据修改后的 Prompt 和 Temperature 重建 JSON Body
        /// </summary>
        private string RebuildRequestBody(string originalBody, string newPrompt, double newTemperature)
        {
            try
            {
                JObject json = JObject.Parse(originalBody);

                // 更新 temperature
                json["temperature"] = newTemperature;

                // 更新 messages 中最后一条 user message
                if (json["messages"] != null)
                {
                    var messages = json["messages"] as JArray;
                    if (messages != null)
                    {
                        // 找到最后一个 user 消息并替换
                        for (int i = messages.Count - 1; i >= 0; i--)
                        {
                            if (messages[i]["role"]?.ToString() == "user")
                            {
                                messages[i]["content"] = newPrompt;
                                break;
                            }
                        }
                    }
                }
                else if (json["prompt"] != null)
                {
                    json["prompt"] = newPrompt;
                }

                return json.ToString(Formatting.None);
            }
            catch
            {
                return originalBody;
            }
        }

        // ============ 按钮事件 ============

        /// <summary>
        /// 启动/停止代理按钮
        /// </summary>
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                StartProxy();
            }
            else
            {
                StopProxy();
            }
        }

        private void StartProxy()
        {
            try
            {
                proxyServer.Start();

                // 设置系统代理
                proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
                proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

                isRunning = true;
                btnStartStop.Text = "⏹ 停止代理";
                btnStartStop.BackColor = Color.LightCoral;
                lblStatus.Text = $"状态：✅ 运行中 (端口 {PROXY_PORT})";
                lblStatus.ForeColor = Color.Green;

                // 禁用自动放行的复选框提示
                MessageBox.Show(
                    "代理已启动！\n\n" +
                    $"系统代理已设置为 127.0.0.1:{PROXY_PORT}\n\n" +
                    "现在可以打开《历史模拟器：崇祯》游戏了。\n" +
                    "游戏的 AI 请求会被拦截并显示在此窗口中。\n\n" +
                    "⚠️ 关闭程序前请先点击【停止代理】以恢复系统代理设置！",
                    "代理启动成功",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"启动代理失败：{ex.Message}\n\n" +
                    "请尝试：\n" +
                    "1. 以管理员身份运行此程序\n" +
                    "2. 确保端口 8888 没有被占用\n" +
                    "3. 关闭其他代理软件（如 Fiddler、Clash 等）",
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void StopProxy()
        {
            try
            {
                // 恢复系统代理
                proxyServer.RestoreOriginalProxySettings();

                // 停止代理服务器
                proxyServer.Stop();

                isRunning = false;
                btnStartStop.Text = "▶ 启动代理";
                btnStartStop.BackColor = Color.LightGreen;
                lblStatus.Text = "状态：已停止";
                lblStatus.ForeColor = Color.Red;

                // 释放所有等待中的请求
                foreach (var pending in pendingRequests.Values)
                {
                    pending.WaitHandle.Set();
                }
                pendingRequests.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"停止代理时出错：{ex.Message}", "警告",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 修改后放行按钮
        /// </summary>
        private void btnPass_Click(object sender, EventArgs e)
        {
            if (currentRequestId != null &&
                pendingRequests.TryGetValue(currentRequestId, out var pending))
            {
                // 获取用户修改后的值
                string newPrompt = txtPrompt.Text;
                double newTemp = 0.7;
                if (!double.TryParse(txtTemperature.Text, out newTemp))
                {
                    MessageBox.Show(
                        "Temperature 格式不正确，已使用默认值 0.7\n\n请输入 0 到 2 之间的数字，例如：0.7、1.0、1.5",
                        "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    newTemp = 0.7;
                    txtTemperature.Text = "0.7";
                }

                // 重建 JSON
                string modifiedBody = RebuildRequestBody(
                    pending.Record.OriginalBody, newPrompt, newTemp);

                pending.ModifiedBody = modifiedBody;
                pending.Record.ModifiedBody = modifiedBody;
                pending.Record.Prompt = newPrompt;
                pending.Record.Temperature = newTemp;

                // 放行
                pending.WaitHandle.Set();

                // 重置界面
                ResetUIAfterPass("已修改并放行");
            }
        }

        /// <summary>
        /// 原始放行按钮（不修改）
        /// </summary>
        private void btnPassOriginal_Click(object sender, EventArgs e)
        {
            if (currentRequestId != null &&
                pendingRequests.TryGetValue(currentRequestId, out var pending))
            {
                pending.ModifiedBody = pending.Record.OriginalBody;
                pending.WaitHandle.Set();
                ResetUIAfterPass("原始放行（未修改）");
            }
        }

        /// <summary>
        /// 放行后重置界面
        /// </summary>
        private void ResetUIAfterPass(string message)
        {
            currentRequestId = null;
            btnPass.Enabled = false;
            btnPassOriginal.Enabled = false;
            txtPrompt.ReadOnly = true;
            txtTemperature.ReadOnly = true;
            lblStatus.Text = $"状态：✅ {message}";
            lblStatus.ForeColor = Color.Green;
        }

        /// <summary>
        /// 点击历史记录查看
        /// </summary>
        private void lstHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstHistory.SelectedIndex >= 0 &&
                lstHistory.SelectedIndex < historyList.Count)
            {
                // 因为新记录插入在顶部，索引对应 historyList 的倒序
                int actualIndex = historyList.Count - 1 - lstHistory.SelectedIndex;
                if (actualIndex >= 0 && actualIndex < historyList.Count)
                {
                    var record = historyList[actualIndex];
                    txtPrompt.Text = record.Prompt;
                    txtPrompt.ReadOnly = true;
                    txtTemperature.Text = record.Temperature.ToString();
                    txtTemperature.ReadOnly = true;
                    txtRawJson.Text = TryFormatJson(record.OriginalBody);

                    btnPass.Enabled = false;
                    btnPassOriginal.Enabled = false;
                }
            }
        }

        // ============ 窗体事件 ============

        /// <summary>
        /// 窗体关闭时清理
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning)
            {
                var result = MessageBox.Show(
                    "代理正在运行！\n\n" +
                    "关闭前需要先停止代理，否则系统代理设置可能无法恢复。\n" +
                    "是否先停止代理再关闭？",
                    "确认关闭",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    StopProxy();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // 取消关闭
                    return;
                }
            }

            // 释放所有等待中的请求
            foreach (var pending in pendingRequests.Values)
            {
                pending.WaitHandle.Set();
            }
        }
    }

    // ============ 数据类 ============

    /// <summary>
    /// 等待处理的请求
    /// </summary>
    public class PendingRequest
    {
        public SessionEventArgs Session { get; set; }
        public HistoryRecord Record { get; set; }
        public ManualResetEventSlim WaitHandle { get; set; }
        public string ModifiedBody { get; set; }
    }

    /// <summary>
    /// 历史记录
    /// </summary>
    public class HistoryRecord
    {
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public string Url { get; set; }
        public string Prompt { get; set; }
        public double Temperature { get; set; }
        public string OriginalBody { get; set; }
        public string ModifiedBody { get; set; }
    }
}
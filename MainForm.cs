using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.IO;

using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Sockets;

namespace SiriAssistant
{
    public partial class MainForm : Form
    {
        private HttpService _httpService;
        private uint Port = uint.Parse(ConfigurationManager.AppSettings["Port"]);
        private TextLogger logger;

        public MainForm()
        {
            InitializeComponent();
            FormClosing += MainForm_FormClosing;

            IPValueLabel.Text = GetLocalIpAddresses();
            PortText.Text = Port.ToString();
            PortText.TextChanged += PortText_TextChanged;
            PortText.KeyPress += PortText_KeyPress;
            SaveBtn.Click += SaveBtn_Click;

            // 添加右键菜单
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem showItem = new ToolStripMenuItem("显示");
            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出");

            showItem.Click += new EventHandler(ShowItem_Click);
            exitItem.Click += new EventHandler(ExitItem_Click);

            contextMenu.Items.AddRange(new ToolStripItem[] { showItem, exitItem });
            NotifyIcon.ContextMenuStrip = contextMenu;

            OnStart(Port);
            logger = new TextLogger(this);
        }

        protected bool OnStart(uint port)
        {
            try
            {
                _httpService = new HttpService();
                _httpService.Setup(new TouchSocketConfig()
                    .SetListenIPHosts((int)Port)
                    .ConfigurePlugins(a =>
                    {
                        a.Add<PostHttpPlug>().Init(this);
                        a.Add<GetHttpPlug>().Init(this);

                        a.UseDefaultHttpServicePlugin();
                    }));

                _httpService.Start();
            } catch (Exception e)
            {
                _httpService.Dispose();
                MessageBox.Show($"启动失败：{e.Message}");
                return false;
            }
            return true;
        }

        protected void OnStop()
        {
            _httpService.Stop();
            _httpService.Dispose();
        }

        private void Restart(uint port)
        {
            logger.Info($"Restart at port {port}...");
            if (!_httpService.DisposedValue)
            {
                OnStop();
            }
            Port = port;
            bool success = OnStart(port);
            if (success)
            {
                logger.Info("Success!");
            } else
            {
                logger.Error("Failed!");
            }
        }

        private void PortText_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 检查键入的字符是否为数字或允许的特殊键（如Backspace）
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // 如果不是数字或控制字符，阻止输入
                e.Handled = true;
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            string port = PortText.Text;
            if (port == this.Port.ToString())
            {
                logger.Info($"端口{port}未变，无需重启服务！");
                return;
            }
            if (string.IsNullOrEmpty(port))
            {
                MessageBox.Show("端口不能为空！");
                return;
            }
            port = port.Trim();
            bool success = uint.TryParse(port, out uint IntPort);
            if (success)
            {
                if (IntPort < 0 || IntPort > 65535)
                {
                    MessageBox.Show("端口不在0~65535之间");
                } else
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Port"].Value = IntPort.ToString();
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    Restart(IntPort);
                }
            } else
            {
                MessageBox.Show("端口号不是数字");
            }
            SaveBtn.Enabled = false;
        }

        private void PortText_TextChanged(object sender, EventArgs e)
        {
            SaveBtn.Enabled = true;
        }

        private string GetLocalIpAddresses()
        {
            using(Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                string localIP = endPoint.Address.ToString();
                return localIP;
            }
        }

        
        public void ExecLog(string msg)
        {
            if (ExecLogText.InvokeRequired)
            {
                ExecLogText.BeginInvoke(new Action(() =>
                {
                    ExecLogText.AppendText(msg + "\n");
                }));
            } else
            {
                ExecLogText.AppendText(msg + "\n");
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
            this.Hide();
        }

        private void ShowItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            NotifyIcon.Dispose();
            Application.Exit();
        }
    }
}

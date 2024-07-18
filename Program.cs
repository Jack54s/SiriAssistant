using System;
using System.Windows.Forms;
using NetFwTypeLib;

namespace SiriAssistant
{
    internal static class Program
    {
        private const string AppName = "SiriAssistant";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                INetFwMgr fwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                INetFwAuthorizedApplication app = (INetFwAuthorizedApplication)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication"));
                app.Name = AppName;
                app.ProcessImageFileName = Application.ExecutablePath;
                app.Enabled = true;
                fwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(app);
            } catch (Exception e)
            {
                MessageBox.Show($"添加防火墙规则错误：{e.Message}");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

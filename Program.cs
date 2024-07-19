using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using NetFwTypeLib;

namespace SiriAssistant
{
    internal static class Program
    {
        private const string AppName = "SiriAssistant";

        public static bool IsUserAnAdmin()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                // 在某些情况下（例如，当在非交互式会话中运行时），获取当前WindowsIdentity可能会失败
                return false;
            }
        }

        public static bool IsProcessElevated()
        {
            bool isElevated = false;
            try
            {
                WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                return isElevated && !IsTokenElevated();
            }
            catch
            {
                return false;
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, int desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr tokenHandle, int tokenInformationClass, ref int tokenInformationLength, int tokenSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        private const int TokenElevation = 20; // TOKEN_ELEVATION
        private const int TokenQuery = 8;      // STANDARD_RIGHTS_REQUIRED | TOKEN_QUERY

        private static bool IsTokenElevated()
        {
            var hCurrentProcess = Process.GetCurrentProcess().Handle;
            var tokenHandle = IntPtr.Zero;

            if (!OpenProcessToken(hCurrentProcess, TokenQuery, out tokenHandle))
            {
                return false;
            }

            try
            {
                var elevation = new Elevation();
                var size = Marshal.SizeOf(elevation);
                if (!GetTokenInformation(tokenHandle, TokenElevation, ref size, size))
                {
                    return false;
                }

                return elevation.TokenIsElevated != 0;
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                {
                    CloseHandle(tokenHandle);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Elevation
        {
            public int TokenIsElevated;
        }

        private static void AddAuthApp()
        {
            INetFwMgr fwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
            INetFwAuthorizedApplication app = (INetFwAuthorizedApplication)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwAuthorizedApplication"));
            app.Name = AppName;
            app.ProcessImageFileName = Application.ExecutablePath;
            app.Enabled = true;
            fwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(app);
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                try
                {
                    INetFwMgr fwMgr = (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                    var app = fwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications.Item(Application.ExecutablePath);
                } catch(System.IO.FileNotFoundException e)
                {
                    if (IsUserAnAdmin())
                    {
                        AddAuthApp();
                    } else
                    {
                        if (IsProcessElevated())
                        {
                            AddAuthApp();
                        } else
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.UseShellExecute = true;
                            startInfo.WorkingDirectory = Environment.CurrentDirectory;
                            startInfo.FileName = Assembly.GetExecutingAssembly().Location;
                            startInfo.Verb = "runas";

                            try
                            {
                                Process.Start(startInfo);
                            }
                            catch (System.ComponentModel.Win32Exception noAdmin)
                            {
                                if (noAdmin.ErrorCode == -2147467259)
                                    MessageBox.Show("Please run as administrator.");
                            }
                            Application.Exit();
                            return;
                        }
                    }
                }
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

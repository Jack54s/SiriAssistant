using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Newtonsoft.Json;
using TouchSocket.Core;
using TouchSocket.Http;
using WindowsInput;
using WindowsInput.Events;

namespace SiriAssistant
{
    internal class PostHttpPlug : PluginBase, IHttpPlugin<IHttpSocketClient>
    {
        private TextLogger logger;
        public void Init(MainForm form)
        {
            this.logger = new TextLogger(form);
        }
        private KeyCode GetKeyCode(char c)
        {
            KeyCode key;
            if ((c >= 0x41 && c <= 0x5A) || (c >= 0x30 && c <= 0x39))
            {
                bool r = Enum.TryParse<KeyCode>(c.ToString(), out key);
                if (r)
                {
                    return key;
                }
                else
                {
                    throw new Exception("HotKey只允许字母数字和#^+!，分别代表Win,Ctrl,Shift,Alt");
                }
            }
            else if (c == '#')
            {
                return KeyCode.LWin;
            }
            else if (c == '^')
            {
                return KeyCode.Control;
            }
            else if (c == '+')
            {
                return KeyCode.Shift;
            }
            else if (c == '!')
            {
                return KeyCode.Alt;
            }
            else
            {
                throw new Exception("HotKey只允许字母数字和#^+!，分别代表Win,Ctrl,Shift,Alt");
            }
        }

        public async Task OnHttpRequest(IHttpSocketClient client, HttpContextEventArgs e)
        {
            if (e.Context.Request.IsPost())
            {
                try
                {
                    if (e.Context.Request.TryGetContent(out var body))
                    {
                        var jsonStr = Encoding.UTF8.GetString(body);
                        var action = JsonConvert.DeserializeObject<ActionBody>(jsonStr);
                        var responseString = "OK!";
                        switch (action.type)
                        {
                            case ActionType.PROGRAM:
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = action.path;
                                startInfo.Arguments = action.opts;
                                try
                                {
                                    using (Process process = Process.Start(startInfo))
                                    {
                                        logger.Info($"执行程序文件：{action.path}...");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    responseString = $"Error: {ex.Message}";
                                    logger.Warn($"执行程序文件出错:{ex.Message}");
                                }
                                finally
                                {
                                    e.Context.Response.FromText(responseString).Answer();
                                }
                                break;
                            case ActionType.HOTKEY:
                                try
                                {
                                    var l = action.path.Length;
                                    action.path = action.path.ToUpper();
                                    KeyCode[] keyCodes = new KeyCode[l];
                                    for (int i = 0; i < l; i++)
                                    {
                                        keyCodes[i] = GetKeyCode(action.path[i]);
                                    }
                                    await Simulate.Events()
                                        .ClickChord(keyCodes)
                                        .Invoke();
                                    string hotKeyName = "";
                                    foreach (char c in action.path)
                                    {
                                        if (c == '^')
                                        {
                                            hotKeyName += "Ctrl";
                                        } else if (c == '+')
                                        {
                                            hotKeyName += "Shift";
                                        } else if (c == '!')
                                        {
                                            hotKeyName += "Alt";
                                        } else if (c == '#')
                                        {
                                            hotKeyName += "Win";
                                        } else
                                        {
                                            hotKeyName += c;
                                        }
                                        hotKeyName += "+";
                                    }
                                    logger.Info($"执行热键：{hotKeyName.Substring(0, hotKeyName.Length - 1)}");
                                }
                                catch (Exception ex)
                                {
                                    responseString = $"Error: {ex.Message}";
                                    logger.Warn($"执行热键出错：{ex.Message}");
                                }
                                finally
                                {
                                    e.Context.Response.FromText(responseString).Answer();
                                }
                                break;
                            default:
                                responseString = "Type error!";
                                logger.Warn("指令参数错误！");
                                e.Context.Response.FromText(responseString).Answer();
                                break;
                        }
                        return;
                    }
                } catch (Exception ex)
                {
                    logger.Error($"程序出错：{ex.Message}");
                }
            }
            await e.InvokeNext();
        }
    }
}

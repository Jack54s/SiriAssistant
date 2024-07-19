using System;
using System.Collections.Generic;
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
            string exMsg = "HotKey只允许字母数字空格和下列符号/#^+!=<>[]，分别代表回车,Win,Ctrl,Shift,Alt,媒体键PlayPause,PrevTrack,NextTrack,VolumeUp,VolumeDown";
            KeyCode key;
            Dictionary<char, KeyCode> keyMap = new Dictionary<char, KeyCode>();
            keyMap.Add(' ', KeyCode.Space);
            keyMap.Add('/', KeyCode.Enter);
            keyMap.Add('#', KeyCode.LWin);
            keyMap.Add('^', KeyCode.Control);
            keyMap.Add('+', KeyCode.Shift);
            keyMap.Add('!', KeyCode.Alt);
            keyMap.Add('=', KeyCode.MediaPlayPause);
            keyMap.Add('<', KeyCode.MediaPreviousTrack);
            keyMap.Add('>', KeyCode.MediaNextTrack);
            keyMap.Add('[', KeyCode.VolumeUp);
            keyMap.Add(']', KeyCode.VolumeDown);
            if ((c >= 0x41 && c <= 0x5A) || (c >= 0x30 && c <= 0x39))
            {
                bool r = Enum.TryParse<KeyCode>(c.ToString(), out key);
                if (r)
                {
                    return key;
                }
                else
                {
                    throw new Exception(exMsg);
                }
            }
            else if (keyMap.ContainsKey(c))
            {
                return keyMap.GetValue(c);
            }
            else
            {
                throw new Exception(exMsg);
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
                                ProcessStartInfo startInfo = new ProcessStartInfo(action.path, action.opts);
                                try
                                {
                                    using (Process process = Process.Start(startInfo))
                                    {
                                        logger.Info($"执行程序文件：{action.path} {action.opts}...");
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
                                    int cc = 1;
                                    for (int i = 0; i < l; i++)
                                    {
                                        keyCodes[i] = GetKeyCode(action.path[i]);
                                    }
                                    if (int.TryParse(action.opts, out var count))
                                    {
                                        cc = count;
                                    }
                                    var eventBuilder = Simulate.Events();
                                    for (int i = 0; i < cc; i++)
                                    {
                                        eventBuilder.ClickChord(keyCodes).Wait(100);
                                    }
                                    await eventBuilder.Invoke();
                                    string hotKeyName = "";
                                    Dictionary<char, string> keyMap = new Dictionary<char, string>();
                                    keyMap.Add(' ', "Space");
                                    keyMap.Add('/', "Enter");
                                    keyMap.Add('#', "Win");
                                    keyMap.Add('^', "Ctrl");
                                    keyMap.Add('+', "Shift");
                                    keyMap.Add('!', "Alt");
                                    keyMap.Add('=', "MediaPlayPause");
                                    keyMap.Add('<', "MediaPreviousTrack");
                                    keyMap.Add('>', "MediaNextTrack");
                                    keyMap.Add('[', "VolumeUp");
                                    keyMap.Add(']', "VolumeDown");
                                    foreach (char c in action.path)
                                    {
                                        if (keyMap.ContainsKey(c))
                                        {
                                            hotKeyName += keyMap.GetValue(c);
                                        } else
                                        {
                                            hotKeyName += c;
                                        }
                                        hotKeyName += "+";
                                    }
                                    string ccStr = cc == 1 ? "" : cc + "x ";
                                    logger.Info($"执行热键：{ccStr}{hotKeyName.Substring(0, hotKeyName.Length - 1)}");
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

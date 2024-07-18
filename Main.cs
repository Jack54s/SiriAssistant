using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WindowsInput;
using WindowsInput.Events;

namespace VoiceAssistant
{
    public partial class Main : ServiceBase
    {
        private HttpListener _listener;
        private Task _listenerTask;
        private const string _url = "http://localhost:9919/";

        public Main()
        {
        }

        protected override void OnStart(string[] args)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(_url);
            _listener.Start();
            _listenerTask = Task.Run(() => StartListener());
        }

        private async Task StartListener()
        {
            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();
                var req = context.Request;
                if (req.HttpMethod == "POST")
                {
                    using(StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding))
                    {
                        string jsonStr = await reader.ReadToEndAsync();
                        var action = JsonConvert.DeserializeObject<Action>(jsonStr);
                        var responseString = "OK!";
                        var response = context.Response;
                        var output = response.OutputStream;
                        byte[] buffer;
                        switch(action.type)
                        {
                            case ActionType.PROGRAM:
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = action.path;
                                startInfo.Arguments = action.opts;
                                try
                                {
                                    using(Process process = Process.Start(startInfo))
                                    { }
                                } catch (Exception ex)
                                {
                                    responseString = $"Error: {ex.Message}";
                                } finally
                                {
                                    buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                                    response.ContentLength64 = buffer.Length;
                                    await output.WriteAsync(buffer, 0, buffer.Length);
                                    output.Close();
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
                                } catch (Exception ex)
                                {
                                    responseString = $"Error: {ex.Message}";
                                } finally
                                {
                                    buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                                    response.ContentLength64 = buffer.Length;
                                    await output.WriteAsync(buffer, 0, buffer.Length);
                                    output.Close();
                                }
                                break;
                            default:
                                responseString = "Type error!";
                                buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                                response.ContentLength64 = buffer.Length;
                                await output.WriteAsync(buffer, 0, buffer.Length);
                                output.Close();
                                break;
                        }
                    }
                } else
                {
                    var response = context.Response;
                    string responseString = "Hello World";
                    var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    var output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }

        protected override void OnStop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public void TestStartupAndStop(String[] args)
        {
            this.OnStart(args);
            Console.WriteLine("Press any key to stop the service...");
            Console.ReadKey();
            this.OnStop();
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
    }
}

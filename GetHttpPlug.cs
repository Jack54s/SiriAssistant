using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Http;

namespace SiriAssistant
{
    internal class GetHttpPlug : PluginBase, IHttpPlugin<IHttpSocketClient>
    {
        private TextLogger logger;
        public void Init(MainForm form)
        {
            this.logger = new TextLogger(form);
        }

        public async Task OnHttpRequest(IHttpSocketClient client, HttpContextEventArgs e)
        {
            if (e.Context.Request.IsGet())
            {
                if (e.Context.Request.UrlEquals("/help"))
                {
                    string helpStr = @"
<!DOCTYPE html>
<html lang=""zh"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body, html {
            height: 100%;
            margin: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: #f0f0f0;
            font-family: Arial, sans-serif;
        }
        .info-box {
            background-color: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(43, 160, 210, 0.1);
            text-align: left;
            max-width: 60%;
            word-wrap: break-word;
        }
    </style>
</head>
<body>
    <div class=""info-box"">
        <p>
          使用ShortCuts中的GetContentOfUrl向主机发送Post请求，请求体格式为JSON，键名有type，path，opts。
        </p>
        <p>
          参数说明：
        </p>
        <ol>
          <li>
            <p>
              type:操作类型，1为运行程序或打开文件、网址，2为模拟按键。
            </p>
          </li>
          <li>
            <p>
              path:在type为1时为文件路径、网址，在type为2时为按键序列。
            </p>
            <p>
              支持模拟按键有字母、数字、空格、回车、Win、Ctrl、Alt、Shift以及媒体键。<br>
              映射关系：<br>
              字母 &nbsp;&lt;==&gt;&nbsp; 字母<br>
              数字 &nbsp;&lt;==&gt;&nbsp; 数字<br>
              &nbsp; &nbsp;&lt;==&gt;&nbsp; 空格<br>
              / &nbsp;&lt;==&gt;&nbsp; 回车<br>
              # &nbsp;&lt;==&gt;&nbsp; Win<br>
              ^ &nbsp;&lt;==&gt;&nbsp; Ctrl<br>
              + &nbsp;&lt;==&gt;&nbsp; Shift<br>
              ! &nbsp;&lt;==&gt;&nbsp; Alt
              = &nbsp;&lt;==&gt;&nbsp; 播放暂停<br>
              &lt; &nbsp;&lt;==&gt;&nbsp; 上一首<br>
              &gt; &nbsp;&lt;==&gt;&nbsp; 下一首<br>
              [ &nbsp;&lt;==&gt;&nbsp; 音量加<br>
              ] &nbsp;&lt;==&gt;&nbsp; 音量减<br>
            </p>
          </li>
          <li>
            <p>
              opts:在type为1时为程序参数，在type为2时，为模拟按键重复次数。
            </p>
          </li>
        </ol>
    </div>
</body>
</html>
";
                    e.Context.Response.SetStatus().SetContentTypeByExtension(".html").SetContent(helpStr).Answer();
                    logger.Info("Return help.html");
                    return;
                }
            }

            await e.InvokeNext();
        }
    }
}

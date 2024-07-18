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
                e.Context.Response.FromText("Hello world").Answer();
                logger.Info("Hello world");
                return;
            }

            await e.InvokeNext();
        }
    }
}

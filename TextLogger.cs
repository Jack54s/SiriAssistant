using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiriAssistant
{
    internal class TextLogger
    {
        private MainForm form;
        public TextLogger(MainForm form)
        {
            this.form = form;
        }

        public void Info(string msg)
        {
            form.ExecLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [INFO] {msg}");
        }

        public void Warn(string msg)
        {
            form.ExecLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [WARN] {msg}");
        }
        public void Error(string msg)
        {
            form.ExecLog($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] {msg}");
        }
    }
}

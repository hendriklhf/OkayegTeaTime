using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OkayegTeaTimeCSharp.JavaScript
{
    public static class JavaScriptHelper
    {
        public static int Now()
        {
            WebBrowser webBrowser = new()
            {
                Url = new(string.Format("file:///{0}/Script.js", Directory.GetCurrentDirectory())),
                ScriptErrorsSuppressed = true
            };
        }
    }
}

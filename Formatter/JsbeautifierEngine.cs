using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    public class JsbeautifierEngine : WebViewJavascriptContext
    {
        public JsbeautifierEngine(Control disp) : base(disp)
        {
            var uri = "file:///C:/Users/frank/Desktop/js-beautify-master/js-beautify-master/index.html";
            Navigate(uri);
        }

        public string Format(string source)
        {
            return Invoke("__format", source) as string;
        }
    }
}

using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    /// <summary>
    /// 基于http://jsbeautifier.org/的JS格式化器
    /// </summary>
    public class JsbeautifierFormatter : IFormatter, IComment
    {
        JsbeautifierEngine _engine;

        public JsbeautifierFormatter(JsbeautifierEngine engine)
        {
            _engine = engine;
        }

        public string Append(string text, string comment)
        {
            return string.Concat("/* ", comment, " */\r\n\r\n", text);
        }

        public string Format(string text)
        {
            return _engine.Format(text);
        }
    }
}

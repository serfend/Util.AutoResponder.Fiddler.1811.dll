using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    public class HtmlFormatter : IFormatter, IComment
    {
        readonly static Regex _rexJs = new Regex(@"(?<=<\s*script[^>]*>).*?(?=<\s*/\s*script\s*>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        readonly static Regex _rexJsSplit = new Regex(@"function\s+_____split\d+_____\s*\(\s*\)\s*\{\s*\s*\}", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        readonly static Regex _rexCss = new Regex(@"(?<=<\s*style[^>]*>).*?(?=<\s*/\s*style\s*>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        IFormatter _jsFotmatter;
        IFormatter _cssFotmatter = new CssFormatter();

        public HtmlFormatter(IFormatter jsFormatter)
        {
            _jsFotmatter = jsFormatter;
        }

        public string Append(string text, string comment)
        {
            return string.Concat(text, "<!-- ", comment, " -->\r\n\r\n");
        }

        public string Format(string text)
        {
            var index = 0;
            var joinedJs = new StringBuilder();
            var html = _rexJs.Replace(text, m =>
            {
                if (string.IsNullOrWhiteSpace(m.Value))
                    return m.Value;

                if (joinedJs.Length > 0)
                    joinedJs.AppendLine($"function _____split{index++}_____(){{}}");
                joinedJs.AppendLine(m.Value);

                return "_____split_____";
            });
            var fmtedJs = _jsFotmatter.Format(joinedJs.ToString());
            var jsArr = _rexJsSplit.Split(fmtedJs);
            index = 0;
            html = _rexJs.Replace(html, m =>
            {
                if (string.IsNullOrWhiteSpace(m.Value))
                    return m.Value;

                return "\r\n" + _jsFotmatter.AppendCopyright(jsArr[index++]);
            });

            html = _rexCss.Replace(html, m =>
            {
                if (string.IsNullOrWhiteSpace(m.Value))
                    return m.Value;
                else
                    return "\r\n" + _cssFotmatter.AppendCopyright(_cssFotmatter.Format(m.Value));
            });

            return html;
        }
    }
}

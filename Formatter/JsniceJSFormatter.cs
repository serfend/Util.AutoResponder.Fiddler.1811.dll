using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Mondol.FiddlerExtension.AutoResponder.AutoResponder
{
    /// <summary>
    /// 基于http://jsnice.org/的JS格式化器
    /// </summary>
    public class JsniceJSFormatter : IFormatter, IComment
    {
        readonly static Regex _rexJsonJs = new Regex(@"""js""\s*\:\s*""(.+)""");

        IFormatter _jsFormatter = new JSFormatter();

        public string Append(string text, string comment)
        {
            return string.Concat("/* ", comment, " */\r\n\r\n", text);
        }

        public string Format(string text)
        {
            try
            {
                return FormatInternal(text);
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogString(ex.Message);

                var fmtedJs = _jsFormatter.Format(text);
                return _jsFormatter.Append(fmtedJs, "re formated");
            }
        }

        private string FormatInternal(string text)
        {
            var url = "http://jsnice.org/beautify?pretty=0&rename=0&types=1&suggest=0";
            var req = WebRequest.Create(url) as HttpWebRequest;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
            req.Referer = "http://jsnice.org/";
            req.Accept = "application/json, text/javascript, */*; q=0.01";
            req.Headers["X-Requested-With"] = "XMLHttpRequest";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Timeout = 1000 * 60;
#if DEBUG
            //req.Proxy = new WebProxy("127.0.0.1", 8888);
#endif

            using (var stream = new StreamWriter(req.GetRequestStream(), Encoding.UTF8))
            {
                stream.Write(text);
            }

            string rspStr = null;
            HttpWebResponse rsp = null;
            try
            {
                rsp = req.GetResponse() as HttpWebResponse;
                using (var stream = new StreamReader(rsp.GetResponseStream()))
                {
                    rspStr = stream.ReadToEnd();
                }
            }
            finally
            {
                rsp?.Close();
            }

            var m = _rexJsonJs.Match(rspStr);
            if (!m.Success)
                throw new InvalidProgramException($"{nameof(JsniceJSFormatter)} 接口错误：{rspStr}");

            return m.Groups[1].Value.Replace("\\n", "\n");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mondol.FiddlerExtension.AutoResponder
{
    class HookService
    {
        private readonly Regex _rexReqNo = new Regex(@"^\d+");
        private readonly Regex _rexReqUrl = new Regex(@"^([A-Z]+)\s+([^\s]+)");

        private readonly Dictionary<string, ReqInfo> _hookUrls = new Dictionary<string, ReqInfo>();

        public void AddBySazFile(string path)
        {
			path = "sfAutoResponseTemplates/" + path;
			path = Path.Combine(path, "raw");
            var files = Directory.GetFiles(path, "*_c.txt");
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var m = _rexReqNo.Match(fileName);
                if (!m.Success)
                    throw new InvalidDataException("无效文件名：" + fileName);
                var reqNo = m.Value;

                var methodLine = GetFirstLine(file);
                m = _rexReqUrl.Match(methodLine);
                if (!m.Success)
                    throw new InvalidDataException($"无效文件行：fileName={fileName}, line={methodLine}");

                var reqMethod = m.Groups[1].Value;
                var reqUrl = m.Groups[2].Value;
                var rspFilePath = Path.Combine(Path.GetDirectoryName(file), $"{reqNo}_s.txt");
                var reqInfo = new ReqInfo()
                {
                    Method =  reqMethod,
                    Url = reqUrl,
                    ResponseFilePath = rspFilePath
                };

                var reqKey = reqMethod + reqUrl;
                _hookUrls[reqKey] = reqInfo;
            }
			MessageBox.Show($"新增{files.Length}条规则");
        }

        public void AddBySazPaths(string paths)
        {
            var pathLst = (paths ?? string.Empty).Split('\r', '\n');
            foreach (var path in pathLst)
            {
                if (!string.IsNullOrEmpty(path))
                    AddBySazFile(path);
            }
        }

        public void Clear()
        {
            _hookUrls.Clear();
        }

        public string GetResponseFile(Fiddler.Session session)
        {
            var reqKey = session.RequestMethod + session.fullUrl;
            ReqInfo reqInfo;
            if (_hookUrls.TryGetValue(reqKey, out reqInfo))
                return reqInfo.ResponseFilePath;

            return null;
        }

        private string GetFirstLine(string filePath)
        {
            using (var sr = new StreamReader(filePath, Encoding.UTF8, true))
            {
				while (!sr.EndOfStream)
				{
					var tmp = sr.ReadLine();
					if (!tmp.StartsWith("//"))
						return tmp;
				}
            }
			return null;
        }

        private class ReqInfo
        {
            public string Method { get; set; }
            public string Url { get; set; }
            public string ResponseFilePath { get; set; }
        }
    }
}


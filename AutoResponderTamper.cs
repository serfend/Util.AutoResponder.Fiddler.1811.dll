using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Mondol.FiddlerExtension.AutoResponder.Views;

namespace Mondol.FiddlerExtension.AutoResponder
{
    public class AutoResponderTamper : Fiddler.IAutoTamper
    {
        private readonly  Config _cfg = Config.Instance;
        private readonly HookService _hookSvce = new HookService();

        public void OnBeforeUnload()
        {
        }

        public void OnLoad()
        {
            try
            {
                _hookSvce.AddBySazPaths(Config.Instance.Paths);
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogString(ex.Message);
            }

            var tabPage = new TabPage("AutoResponder For SAZ")
            {
                Controls =
                {
                    new ConfigView(Config.Instance, _hookSvce)
                    {
                        Dock = DockStyle.Fill
                    }
                }
            };
            var tabPages = FiddlerApplication.UI.tabsViews.TabPages;
            for (var i = 0; i < tabPages.Count; ++i)
            {
                if (tabPages[i].Text == "AutoResponder")
                    tabPages.Insert(i + 1, tabPage);
            }
        }

        public void AutoTamperRequestAfter(Fiddler.Session oSession)
        {
        }

        public void AutoTamperRequestBefore(Fiddler.Session oSession)
        {
           
        }

        public void AutoTamperResponseAfter(Fiddler.Session oSession)
        {
        }

        public void AutoTamperResponseBefore(Fiddler.Session oSession)
        {
			if (_cfg.Enabled)
			{
				var rspFilePath = _hookSvce.GetResponseFile(oSession);
				if (!string.IsNullOrEmpty(rspFilePath))
				{
					var info = File.ReadAllText(rspFilePath);
					oSession.utilSetResponseBody(info);
				}
			}
			//         if (oSession.responseBodyBytes?.Length < 1)
			//             return;
			//if (Config.Instance.Enabled)
			//{
			//	oSession.fullUrl



			//	string fmtedText = null;
			//	try
			//	{

			//	}
			//	catch (Exception ex)
			//	{
			//		FiddlerApplication.Log.LogString($"格式化 {oSession.fullUrl} 时异常 \r\n{ex.ToString()}");
			//	}
			//	finally
			//	{
			//		oSession.utilSetResponseBody(fmtedText);
			//	}
			//}
		}

        public void OnBeforeReturningError(Fiddler.Session oSession)
        {
        }        
    }
}

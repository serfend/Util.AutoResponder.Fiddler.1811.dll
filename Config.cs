using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mondol.FiddlerExtension.AutoResponder
{
    class Config
    {
        public static Config Instance { get; private set; } = new Config();

        public bool Enabled
        {
            get
            {
                return FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.mondol.autoResponder.enabled", true);
            }
            set
            {
                FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.mondol.autoResponder.enabled", value);
            }
        }

        public string Paths
        {
            get
            {
                return FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.mondol.autoResponder.paths", string.Empty);
            }
            set
            {
                FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.mondol.autoResponder.paths", value);
            }
        }
    }
}

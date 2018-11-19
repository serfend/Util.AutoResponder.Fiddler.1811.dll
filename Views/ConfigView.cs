using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mondol.FiddlerExtension.AutoResponder.Views
{
    internal partial class ConfigView : UserControl
    {
        private readonly Config _cfg;
        private readonly HookService _hookSvce;

        public ConfigView(Config cfg, HookService hookSvce)
        {
            _cfg = cfg;
            _hookSvce = hookSvce;
            InitializeComponent();
        }

        private void ConfigView_Load(object sender, EventArgs e)
        {
            ctlEnable.Checked = _cfg.Enabled;
            ctlPaths.Text = _cfg.Paths;
        }

        private void ctlSave_Click(object sender, EventArgs e)
        {
            _cfg.Enabled = ctlEnable.Checked;
            _cfg.Paths = ctlPaths.Text;

            try
            {
                _hookSvce.Clear();
                _hookSvce.AddBySazPaths(_cfg.Paths);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ctlEnable_CheckedChanged(object sender, EventArgs e)
        {
            _cfg.Enabled = ctlEnable.Checked;
        }

        private void ctlPowerBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://github.com/serfend");
        }
    }
}

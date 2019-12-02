using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BearBuildTool.UI.VisualStudio
{
    public partial class CompilersSetting : Form
    {
        public CompilersSetting()
        {
            InitializeComponent();
        }

        private void MVSC_Load(object sender, EventArgs e)
        {

            textBoxPath.Text = Config.Global.MinGWPath;
            IEnumerable<string> vs = null;
            try
            {
                checkBoxWindowsSDK8.Checked = !Config.Global.Windows10SDKUsing;
                vs = Windows.VCBuildTools.GetWindows10SDKs(Windows.VCBuildTools.FindWindows10SDKInstallationFolder());

            }
            finally

            {
                foreach (string ver in vs)
                {
                    comboBoxSDKs.Items.Add(ver);
                    if (Config.Global.Windows10SDK == ver)
                    {
                        comboBoxSDKs.SelectedIndex = comboBoxSDKs.Items.Count - 1;
                    }
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (comboBoxSDKs.SelectedIndex < 0)
            { Close(); return; }
            string ver = comboBoxSDKs.SelectedItem as string;
            if (String.IsNullOrEmpty(ver)) { Close(); return; }
            Config.Global.Windows10SDK = ver;
            Config.Global.MinGWPath = textBoxPath.Text;
            Config.Global.SaveConfig();
            Close();
        }

        private void checkBoxWindowsSDK8_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Windows.VCBuildTools.FindWindows10SDKInstallationFolder();
            }
            catch
            {
                Config.Global.Windows10SDKUsing = false;
                Config.Global.SaveConfig();
            }
            finally
            {
                Config.Global.Windows10SDKUsing = !checkBoxWindowsSDK8.Checked;
                Config.Global.SaveConfig();
            }


        }

        private void buttonEditPath_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialogForMinGW.ShowDialog()==DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserDialogForMinGW.SelectedPath;
            }
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BearBuildTool.UI.VisualStudio
{
    public partial class VSProjectManager : Form
    {
        public VSProjectManager()
        {
            InitializeComponent();
        }

     



        private void comboBoxSubProject_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }

        private void comboBoxProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProject.SelectedIndex < 0) return;
            string name = comboBoxProject.SelectedItem as string;
            if (String.IsNullOrEmpty(name)) return;
            List<string> projects = new List<string>();
            {
                Projects.GenerateProjectFile generateProjectFile = new Projects.GenerateProjectFile();
                generateProjectFile.GetProjects(name, ref projects);
            }
            comboBoxSubProject.Items.Clear();
            foreach (string project in projects)
            {
                comboBoxSubProject.Items.Add(project);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (comboBoxProject.SelectedIndex < 0) return;
            string name = comboBoxProject.SelectedItem as string;
            if (String.IsNullOrEmpty(name)) return;
            if (openFileDialog.ShowDialog() == DialogResult.Cancel) return;

            Windows.VisualProject.VisualProject.CreateFilters(openFileDialog.FileName, name);

        }

        private void buttonSaveToProject_Click(object sender, EventArgs e)
        {
            if (comboBoxProject.SelectedIndex < 0) return;
            string name = comboBoxProject.SelectedItem as string;
            if (String.IsNullOrEmpty(name)) return;
            if (comboBoxSubProject.SelectedIndex < 0) return;
            string sub_name = comboBoxSubProject.SelectedItem as string;
            if (String.IsNullOrEmpty(sub_name)) return;
            Windows.VisualProject.VisualProject.SaveFilters(name,sub_name);
        }

        private void VSProjectManager_Load(object sender, EventArgs e)
        {
            foreach (string name in Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure][Config.Global.DevVersion].Keys)
            {
                comboBoxProject.Items.Add(name);
            }
        }
    }
}

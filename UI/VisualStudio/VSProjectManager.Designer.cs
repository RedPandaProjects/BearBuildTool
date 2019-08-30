namespace BearBuildTool.UI.VisualStudio
{
    partial class VSProjectManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.GroupBox groupBox1;
            this.buttonSaveToProject = new System.Windows.Forms.Button();
            this.comboBoxProject = new System.Windows.Forms.ComboBox();
            this.comboBoxSubProject = new System.Windows.Forms.ComboBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(47, 13);
            label1.TabIndex = 3;
            label1.Text = "Проект:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 41);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(68, 13);
            label2.TabIndex = 4;
            label2.Text = "Суб Проект:";
            // 
            // buttonSaveToProject
            // 
            this.buttonSaveToProject.Location = new System.Drawing.Point(92, 65);
            this.buttonSaveToProject.Name = "buttonSaveToProject";
            this.buttonSaveToProject.Size = new System.Drawing.Size(131, 23);
            this.buttonSaveToProject.TabIndex = 0;
            this.buttonSaveToProject.Text = "Сохранить в проект";
            this.buttonSaveToProject.UseVisualStyleBackColor = true;
            this.buttonSaveToProject.Click += new System.EventHandler(this.buttonSaveToProject_Click);
            // 
            // comboBoxProject
            // 
            this.comboBoxProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProject.FormattingEnabled = true;
            this.comboBoxProject.Location = new System.Drawing.Point(53, 13);
            this.comboBoxProject.Name = "comboBoxProject";
            this.comboBoxProject.Size = new System.Drawing.Size(170, 21);
            this.comboBoxProject.TabIndex = 1;
            this.comboBoxProject.SelectedIndexChanged += new System.EventHandler(this.comboBoxProject_SelectedIndexChanged);
            // 
            // comboBoxSubProject
            // 
            this.comboBoxSubProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSubProject.FormattingEnabled = true;
            this.comboBoxSubProject.Location = new System.Drawing.Point(71, 38);
            this.comboBoxSubProject.Name = "comboBoxSubProject";
            this.comboBoxSubProject.Size = new System.Drawing.Size(152, 21);
            this.comboBoxSubProject.TabIndex = 2;
            this.comboBoxSubProject.SelectedIndexChanged += new System.EventHandler(this.comboBoxSubProject_SelectedIndexChanged);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(7, 65);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 5;
            this.buttonLoad.Text = "Загрузить";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Visual filters|*.vcxproj.filters|Все файлы|*";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(this.comboBoxSubProject);
            groupBox1.Controls.Add(this.buttonSaveToProject);
            groupBox1.Controls.Add(this.comboBoxProject);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(this.buttonLoad);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(232, 100);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Менеджер филтров";
            // 
            // VSProjectManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 112);
            this.Controls.Add(groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VSProjectManager";
            this.Text = "Менеджер VS проектов ";
            this.Load += new System.EventHandler(this.VSProjectManager_Load);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSaveToProject;
        private System.Windows.Forms.ComboBox comboBoxSubProject;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.ComboBox comboBoxProject;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}
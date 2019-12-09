namespace BearBuildTool.UI.VisualStudio
{
    partial class CompilersSetting
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxSDKs = new System.Windows.Forms.ComboBox();
            this.checkBoxWindowsSDK8 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonEditPath = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialogForMinGW = new System.Windows.Forms.FolderBrowserDialog();
            label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 23);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(94, 13);
            label1.TabIndex = 0;
            label1.Text = "Windows 10 SDK:";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(161, 154);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxSDKs
            // 
            this.comboBoxSDKs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSDKs.FormattingEnabled = true;
            this.comboBoxSDKs.Location = new System.Drawing.Point(96, 19);
            this.comboBoxSDKs.Name = "comboBoxSDKs";
            this.comboBoxSDKs.Size = new System.Drawing.Size(132, 21);
            this.comboBoxSDKs.TabIndex = 2;
            // 
            // checkBoxWindowsSDK8
            // 
            this.checkBoxWindowsSDK8.AutoSize = true;
            this.checkBoxWindowsSDK8.Location = new System.Drawing.Point(6, 49);
            this.checkBoxWindowsSDK8.Name = "checkBoxWindowsSDK8";
            this.checkBoxWindowsSDK8.Size = new System.Drawing.Size(113, 17);
            this.checkBoxWindowsSDK8.TabIndex = 3;
            this.checkBoxWindowsSDK8.Text = "Windows SDK 8.1";
            this.checkBoxWindowsSDK8.UseVisualStyleBackColor = true;
            this.checkBoxWindowsSDK8.CheckedChanged += new System.EventHandler(this.checkBoxWindowsSDK8_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSDKs);
            this.groupBox1.Controls.Add(this.checkBoxWindowsSDK8);
            this.groupBox1.Controls.Add(label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 73);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Microsoft C/C++ Compiler";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonEditPath);
            this.groupBox2.Controls.Add(this.textBoxPath);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(4, 75);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(232, 75);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MinGW-w64";
            // 
            // buttonEditPath
            // 
            this.buttonEditPath.Location = new System.Drawing.Point(198, 44);
            this.buttonEditPath.Name = "buttonEditPath";
            this.buttonEditPath.Size = new System.Drawing.Size(30, 23);
            this.buttonEditPath.TabIndex = 2;
            this.buttonEditPath.Text = "...";
            this.buttonEditPath.UseVisualStyleBackColor = true;
            this.buttonEditPath.Click += new System.EventHandler(this.buttonEditPath_Click);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(42, 18);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(186, 20);
            this.textBoxPath.TabIndex = 1;
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Путь:";
            // 
            // CompilersSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 182);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CompilersSetting";
            this.Text = "Настройка кампиляторов";
            this.Load += new System.EventHandler(this.MVSC_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox comboBoxSDKs;
        private System.Windows.Forms.CheckBox checkBoxWindowsSDK8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonEditPath;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogForMinGW;
    }
}
namespace BearBuildTool.UI.VisualStudio
{
    partial class MVSC
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
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(2, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(94, 13);
            label1.TabIndex = 0;
            label1.Text = "Windows 10 SDK:";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(152, 35);
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
            this.comboBoxSDKs.Location = new System.Drawing.Point(95, 9);
            this.comboBoxSDKs.Name = "comboBoxSDKs";
            this.comboBoxSDKs.Size = new System.Drawing.Size(132, 21);
            this.comboBoxSDKs.TabIndex = 2;
            // 
            // checkBoxWindowsSDK8
            // 
            this.checkBoxWindowsSDK8.AutoSize = true;
            this.checkBoxWindowsSDK8.Location = new System.Drawing.Point(5, 39);
            this.checkBoxWindowsSDK8.Name = "checkBoxWindowsSDK8";
            this.checkBoxWindowsSDK8.Size = new System.Drawing.Size(113, 17);
            this.checkBoxWindowsSDK8.TabIndex = 3;
            this.checkBoxWindowsSDK8.Text = "Windows SDK 8.1";
            this.checkBoxWindowsSDK8.UseVisualStyleBackColor = true;
            this.checkBoxWindowsSDK8.CheckedChanged += new System.EventHandler(this.checkBoxWindowsSDK8_CheckedChanged);
            // 
            // MVSC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 64);
            this.Controls.Add(this.checkBoxWindowsSDK8);
            this.Controls.Add(this.comboBoxSDKs);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MVSC";
            this.Text = "Майкрософт С++ кампилятор";
            this.Load += new System.EventHandler(this.MVSC_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ComboBox comboBoxSDKs;
        private System.Windows.Forms.CheckBox checkBoxWindowsSDK8;
    }
}
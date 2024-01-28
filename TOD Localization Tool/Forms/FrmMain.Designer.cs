namespace TOD_Localization_Tool
{
    partial class FrmMain
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
            this.TxtMainPath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.ExportFonts = new System.Windows.Forms.Button();
            this.ImportFonts = new System.Windows.Forms.Button();
            this.ImportTexts = new System.Windows.Forms.Button();
            this.ExportTexts = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TxtMainPath
            // 
            this.TxtMainPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtMainPath.Location = new System.Drawing.Point(12, 34);
            this.TxtMainPath.Name = "TxtMainPath";
            this.TxtMainPath.Size = new System.Drawing.Size(215, 20);
            this.TxtMainPath.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(233, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Main file path:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(11, 60);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(95, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Take a backup";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // ExportFonts
            // 
            this.ExportFonts.Location = new System.Drawing.Point(11, 86);
            this.ExportFonts.Name = "ExportFonts";
            this.ExportFonts.Size = new System.Drawing.Size(142, 75);
            this.ExportFonts.TabIndex = 4;
            this.ExportFonts.Text = "Export Fonts";
            this.ExportFonts.UseVisualStyleBackColor = true;
            this.ExportFonts.Click += new System.EventHandler(this.ExportFonts_Click);
            // 
            // ImportFonts
            // 
            this.ImportFonts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ImportFonts.Location = new System.Drawing.Point(161, 86);
            this.ImportFonts.Name = "ImportFonts";
            this.ImportFonts.Size = new System.Drawing.Size(142, 75);
            this.ImportFonts.TabIndex = 5;
            this.ImportFonts.Text = "Impot Fonts";
            this.ImportFonts.UseVisualStyleBackColor = true;
            this.ImportFonts.Click += new System.EventHandler(this.ImportFonts_Click);
            // 
            // ImportTexts
            // 
            this.ImportTexts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ImportTexts.Location = new System.Drawing.Point(161, 167);
            this.ImportTexts.Name = "ImportTexts";
            this.ImportTexts.Size = new System.Drawing.Size(142, 75);
            this.ImportTexts.TabIndex = 7;
            this.ImportTexts.Text = "Impot Texts";
            this.ImportTexts.UseVisualStyleBackColor = true;
            this.ImportTexts.Click += new System.EventHandler(this.ImportTexts_Click);
            // 
            // ExportTexts
            // 
            this.ExportTexts.Location = new System.Drawing.Point(11, 167);
            this.ExportTexts.Name = "ExportTexts";
            this.ExportTexts.Size = new System.Drawing.Size(142, 75);
            this.ExportTexts.TabIndex = 6;
            this.ExportTexts.Text = "Export Texts";
            this.ExportTexts.UseVisualStyleBackColor = true;
            this.ExportTexts.Click += new System.EventHandler(this.ExportTexts_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(11, 248);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(292, 269);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "By: Amr shaheen (@amrshaheen61)\r\n\r\nSpecial thanks for: Michael0ne";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 529);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ImportTexts);
            this.Controls.Add(this.ExportTexts);
            this.Controls.Add(this.ImportFonts);
            this.Controls.Add(this.ExportFonts);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TxtMainPath);
            this.Name = "FrmMain";
            this.Text = "FrmMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtMainPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button ExportFonts;
        private System.Windows.Forms.Button ImportFonts;
        private System.Windows.Forms.Button ImportTexts;
        private System.Windows.Forms.Button ExportTexts;
        private System.Windows.Forms.TextBox textBox1;
    }
}
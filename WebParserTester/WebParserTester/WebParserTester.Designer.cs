namespace WebParserTester
{
    partial class frmWebParserTester
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.richTextBoxResult = new System.Windows.Forms.RichTextBox();
            this.btnStartTest = new System.Windows.Forms.Button();
            this.btnStopTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxResult
            // 
            this.richTextBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxResult.Location = new System.Drawing.Point(12, 41);
            this.richTextBoxResult.Name = "richTextBoxResult";
            this.richTextBoxResult.Size = new System.Drawing.Size(515, 261);
            this.richTextBoxResult.TabIndex = 3;
            this.richTextBoxResult.Text = "";
            // 
            // btnStartTest
            // 
            this.btnStartTest.Location = new System.Drawing.Point(12, 8);
            this.btnStartTest.Name = "btnStartTest";
            this.btnStartTest.Size = new System.Drawing.Size(109, 23);
            this.btnStartTest.TabIndex = 4;
            this.btnStartTest.Text = "Start test process";
            this.btnStartTest.UseVisualStyleBackColor = true;
            this.btnStartTest.Click += new System.EventHandler(this.btnStartTest_Click);
            // 
            // btnStopTest
            // 
            this.btnStopTest.Enabled = false;
            this.btnStopTest.Location = new System.Drawing.Point(127, 8);
            this.btnStopTest.Name = "btnStopTest";
            this.btnStopTest.Size = new System.Drawing.Size(109, 23);
            this.btnStopTest.TabIndex = 5;
            this.btnStopTest.Text = "Stop test process";
            this.btnStopTest.UseVisualStyleBackColor = true;
            this.btnStopTest.Click += new System.EventHandler(this.btnStopTest_Click);
            // 
            // frmWebParserTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 314);
            this.Controls.Add(this.btnStopTest);
            this.Controls.Add(this.btnStartTest);
            this.Controls.Add(this.richTextBoxResult);
            this.Name = "frmWebParserTester";
            this.Text = "WebParserTester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWebParserTester_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxResult;
        private System.Windows.Forms.Button btnStartTest;
        private System.Windows.Forms.Button btnStopTest;
    }
}


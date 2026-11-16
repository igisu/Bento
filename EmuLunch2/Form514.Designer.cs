namespace Emu.Bento
{
    partial class Form514
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
            this.btnExec = new System.Windows.Forms.Button();
            this.btnDisp = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.CausesValidation = false;
            this.btnExec.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExec.Location = new System.Drawing.Point(265, 134);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(150, 25);
            this.btnExec.TabIndex = 2;
            this.btnExec.Text = "請求締処理を開始";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnDisp
            // 
            this.btnDisp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDisp.AutoSize = true;
            this.btnDisp.Location = new System.Drawing.Point(12, 134);
            this.btnDisp.Name = "btnDisp";
            this.btnDisp.Size = new System.Drawing.Size(150, 25);
            this.btnDisp.TabIndex = 1;
            this.btnDisp.Text = "クレカ決済一覧を表示";
            this.btnDisp.UseVisualStyleBackColor = true;
            this.btnDisp.Click += new System.EventHandler(this.btnDisp_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(381, 75);
            this.label2.TabIndex = 0;
            this.label2.Text = "クレカ決済が完了している請求データを締め直す場合は、\r\n「クレカ決済一覧を表示」からクレカ決済の取消を行ってください。\r\n\r\nクレカ決済の取消を行わない場合は、\r" +
    "\n「請求締処理を開始」をクリックしてください。\r\n";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(410, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "クレカ決済が完了している請求データは、締処理の対象外です。\r\n";
            // 
            // Form514
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 171);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.btnDisp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form514";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "請求締処理確認ダイアログ";
            this.Load += new System.EventHandler(this.Form514_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form514_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnDisp;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

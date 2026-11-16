namespace Emu.Bento
{
    partial class Form700N
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
            this.components = new System.ComponentModel.Container();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtReceiptTime = new Emu.Bento.Common.Control.TextBoxAHMS(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.txtTelNo = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnYes
            // 
            this.btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(12, 108);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(171, 47);
            this.btnYes.TabIndex = 2;
            this.btnYes.Text = "はい。既存または新規の顧客として登録します。(&Y)";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(189, 108);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(159, 47);
            this.btnNo.TabIndex = 3;
            this.btnNo.Text = "いいえ。顧客ではないので今後無視します。(&N)";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(354, 108);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 47);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "キャンセル(Esc)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtReceiptTime
            // 
            this.txtReceiptTime.BackColor = System.Drawing.Color.PapayaWhip;
            this.txtReceiptTime.Location = new System.Drawing.Point(345, 12);
            this.txtReceiptTime.Name = "txtReceiptTime";
            this.txtReceiptTime.ReadOnly = true;
            this.txtReceiptTime.Size = new System.Drawing.Size(78, 22);
            this.txtReceiptTime.TabIndex = 8;
            this.txtReceiptTime.TabStop = false;
            this.txtReceiptTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(272, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "応答時刻";
            // 
            // txtTelNo
            // 
            this.txtTelNo.BackColor = System.Drawing.Color.PapayaWhip;
            this.txtTelNo.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTelNo.Location = new System.Drawing.Point(129, 12);
            this.txtTelNo.Name = "txtTelNo";
            this.txtTelNo.ReadOnly = true;
            this.txtTelNo.Size = new System.Drawing.Size(113, 22);
            this.txtTelNo.TabIndex = 6;
            this.txtTelNo.TabStop = false;
            this.txtTelNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "電話番号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(294, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "本電話番号はどの顧客にも登録されていません。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(354, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "この電話番号は受注で使用される顧客からの番号ですか？";
            // 
            // Form700N
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(478, 167);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtReceiptTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTelNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form700N";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "電話新規登録確認";
            this.Load += new System.EventHandler(this.Form700N_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Button btnCancel;
        private Common.Control.TextBoxAHMS txtReceiptTime;
        private System.Windows.Forms.Label label3;
        private Common.Control.TextBoxA txtTelNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
    }
}
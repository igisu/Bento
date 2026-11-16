namespace Emu.Bento
{
    partial class Form307P
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
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblKind = new System.Windows.Forms.Label();
            this.bigBagLabelPrintCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.lblSyokujiKbn = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.lblBusyo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblChumonbi = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPrinted = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bigBagLabelPrintCriteriaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(135, 207);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 4;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(26, 207);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 25);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "印刷(F10)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblKind);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblSyokujiKbn);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblBusyo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblChumonbi);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(241, 161);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "印刷対象";
            // 
            // lblKind
            // 
            this.lblKind.AutoEllipsis = true;
            this.lblKind.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblKind.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "印刷種別名", true));
            this.lblKind.Location = new System.Drawing.Point(64, 76);
            this.lblKind.Name = "lblKind";
            this.lblKind.Size = new System.Drawing.Size(86, 22);
            this.lblKind.TabIndex = 5;
            this.lblKind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bigBagLabelPrintCriteriaBindingSource
            // 
            this.bigBagLabelPrintCriteriaBindingSource.AllowNew = false;
            this.bigBagLabelPrintCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.BigBagLabelPrintCriteria);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "種別";
            // 
            // lblSyokujiKbn
            // 
            this.lblSyokujiKbn.AutoEllipsis = true;
            this.lblSyokujiKbn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSyokujiKbn.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "食事区分名", true));
            this.lblSyokujiKbn.Location = new System.Drawing.Point(64, 104);
            this.lblSyokujiKbn.Name = "lblSyokujiKbn";
            this.lblSyokujiKbn.Size = new System.Drawing.Size(86, 22);
            this.lblSyokujiKbn.TabIndex = 7;
            this.lblSyokujiKbn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "区分";
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Checked = true;
            this.rdoDesc.Location = new System.Drawing.Point(180, 134);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 11;
            this.rdoDesc.TabStop = true;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Location = new System.Drawing.Point(119, 134);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 10;
            this.rdoAsc.Text = "昇順";
            this.rdoAsc.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "食数の";
            // 
            // lblBusyo
            // 
            this.lblBusyo.AutoEllipsis = true;
            this.lblBusyo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBusyo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "部署名", true));
            this.lblBusyo.Location = new System.Drawing.Point(64, 49);
            this.lblBusyo.Name = "lblBusyo";
            this.lblBusyo.Size = new System.Drawing.Size(165, 22);
            this.lblBusyo.TabIndex = 3;
            this.lblBusyo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "部署";
            // 
            // lblChumonbi
            // 
            this.lblChumonbi.AutoEllipsis = true;
            this.lblChumonbi.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblChumonbi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "注文日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.lblChumonbi.Location = new System.Drawing.Point(64, 21);
            this.lblChumonbi.Name = "lblChumonbi";
            this.lblChumonbi.Size = new System.Drawing.Size(121, 22);
            this.lblChumonbi.TabIndex = 1;
            this.lblChumonbi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "印刷順";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "注文日";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "下記の印刷を開始します";
            // 
            // lblPrinted
            // 
            this.lblPrinted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrinted.AutoSize = true;
            this.lblPrinted.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblPrinted.ForeColor = System.Drawing.Color.Red;
            this.lblPrinted.Location = new System.Drawing.Point(171, 15);
            this.lblPrinted.Name = "lblPrinted";
            this.lblPrinted.Size = new System.Drawing.Size(82, 15);
            this.lblPrinted.TabIndex = 1;
            this.lblPrinted.Text = "印刷済です";
            // 
            // Form307P
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 244);
            this.Controls.Add(this.lblPrinted);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form307P";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "大袋ラベル印刷確認ダイアログ";
            this.Load += new System.EventHandler(this.Form307P_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form307P_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bigBagLabelPrintCriteriaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bigBagLabelPrintCriteriaBindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblChumonbi;
        private System.Windows.Forms.Label lblPrinted;
        private System.Windows.Forms.Label lblBusyo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private System.Windows.Forms.Label lblSyokujiKbn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblKind;
        private System.Windows.Forms.Label label7;
    }
}

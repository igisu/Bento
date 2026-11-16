namespace Emu.Bento
{
    partial class Form710
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtTantousyaName = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.kanriBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtTantousyaNameKana = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.chkInvalid = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.lblUpdateUserCD = new System.Windows.Forms.Label();
            this.lblUpdateUserName = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.txtZipCode = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.lblCreateDate = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblCreateUserName = new System.Windows.Forms.Label();
            this.lblCreateUserCD = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAddress = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtTel = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFax = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.txtBoss = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtShimebi = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.kanriBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "会社名";
            //
            // txtTantousyaName
            //
            this.txtTantousyaName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "会社名", true));
            this.txtTantousyaName.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTantousyaName.Location = new System.Drawing.Point(100, 12);
            this.txtTantousyaName.MaxLength = 30;
            this.txtTantousyaName.Name = "txtTantousyaName";
            this.txtTantousyaName.Size = new System.Drawing.Size(190, 22);
            this.txtTantousyaName.TabIndex = 1;
            //
            // kanriBindingSource
            //
            this.kanriBindingSource.DataSource = typeof(Emu.Bento.Data.Kanri);
            //
            // txtTantousyaNameKana
            //
            this.txtTantousyaNameKana.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "会社名カナ", true));
            this.txtTantousyaNameKana.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTantousyaNameKana.Location = new System.Drawing.Point(100, 40);
            this.txtTantousyaNameKana.MaxLength = 15;
            this.txtTantousyaNameKana.Name = "txtTantousyaNameKana";
            this.txtTantousyaNameKana.Size = new System.Drawing.Size(190, 22);
            this.txtTantousyaNameKana.TabIndex = 3;
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "会社名カナ";
            //
            // label8
            //
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 183);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "代表取締役";
            //
            // chkInvalid
            //
            this.chkInvalid.AutoSize = true;
            this.chkInvalid.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.kanriBindingSource, "チケット回収時売上フラグ", true));
            this.chkInvalid.Location = new System.Drawing.Point(12, 236);
            this.chkInvalid.Name = "chkInvalid";
            this.chkInvalid.Size = new System.Drawing.Size(250, 19);
            this.chkInvalid.TabIndex = 16;
            this.chkInvalid.Text = "回収したチケットを売上として計上する";
            this.chkInvalid.UseVisualStyleBackColor = true;
            //
            // label10
            //
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 328);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 22;
            this.label10.Text = "修正担当者";
            //
            // lblUpdateUserCD
            //
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(102, 324);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 23;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUpdateUserName
            //
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(183, 324);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 24;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // label13
            //
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 356);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 25;
            this.label13.Text = "修正日時";
            //
            // lblUpdateDate
            //
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(102, 352);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 26;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblMode
            //
            this.lblMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblMode.ForeColor = System.Drawing.Color.Black;
            this.lblMode.Location = new System.Drawing.Point(12, 388);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(82, 21);
            this.lblMode.TabIndex = 27;
            this.lblMode.Text = "新規モード";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(198, 386);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 28;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnQuit
            //
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(294, 386);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 29;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            //
            // errorProvider1
            //
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.kanriBindingSource;
            //
            // label11
            //
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 71);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 15);
            this.label11.TabIndex = 4;
            this.label11.Text = "郵便番号";
            //
            // txtZipCode
            //
            this.txtZipCode.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "郵便番号", true));
            this.txtZipCode.Location = new System.Drawing.Point(100, 68);
            this.txtZipCode.MaxLength = 8;
            this.txtZipCode.Name = "txtZipCode";
            this.txtZipCode.Size = new System.Drawing.Size(74, 22);
            this.txtZipCode.TabIndex = 5;
            //
            // lblCreateDate
            //
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 296);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 21;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // label16
            //
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 300);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 20;
            this.label16.Text = "作成日時";
            //
            // lblCreateUserName
            //
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 268);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 19;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCreateUserCD
            //
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 268);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 18;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // label19
            //
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 272);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 17;
            this.label19.Text = "作成担当者";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "住所";
            //
            // txtAddress
            //
            this.txtAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "住所", true));
            this.txtAddress.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtAddress.Location = new System.Drawing.Point(100, 96);
            this.txtAddress.MaxLength = 30;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(249, 22);
            this.txtAddress.TabIndex = 7;
            //
            // txtTel
            //
            this.txtTel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "電話番号", true));
            this.txtTel.Location = new System.Drawing.Point(100, 124);
            this.txtTel.MaxLength = 15;
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(121, 22);
            this.txtTel.TabIndex = 9;
            //
            // label12
            //
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 127);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 15);
            this.label12.TabIndex = 8;
            this.label12.Text = "電話番号";
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "FAX";
            //
            // txtFax
            //
            this.txtFax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "ＦＡＸ", true));
            this.txtFax.Location = new System.Drawing.Point(100, 152);
            this.txtFax.MaxLength = 15;
            this.txtFax.Name = "txtFax";
            this.txtFax.Size = new System.Drawing.Size(121, 22);
            this.txtFax.TabIndex = 11;
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "自社締日";
            //
            // txtBoss
            //
            this.txtBoss.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "代表取締役", true));
            this.txtBoss.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtBoss.Location = new System.Drawing.Point(100, 180);
            this.txtBoss.MaxLength = 10;
            this.txtBoss.Name = "txtBoss";
            this.txtBoss.Size = new System.Drawing.Size(121, 22);
            this.txtBoss.TabIndex = 13;
            //
            // txtShimebi
            //
            this.txtShimebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kanriBindingSource, "自社締日", true));
            this.txtShimebi.Location = new System.Drawing.Point(100, 208);
            this.txtShimebi.MaxLength = 2;
            this.txtShimebi.Name = "txtShimebi";
            this.txtShimebi.Size = new System.Drawing.Size(25, 22);
            this.txtShimebi.TabIndex = 15;
            this.txtShimebi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // Form710
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 423);
            this.Controls.Add(this.txtShimebi);
            this.Controls.Add(this.txtBoss);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtFax);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtTel);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblCreateDate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCreateUserName);
            this.Controls.Add(this.lblCreateUserCD);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtZipCode);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.lblUpdateDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblUpdateUserName);
            this.Controls.Add(this.lblUpdateUserCD);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.chkInvalid);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTantousyaNameKana);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTantousyaName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form710";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "管理マスタ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form710_FormClosing);
            this.Load += new System.EventHandler(this.Form710_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form710_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.kanriBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtTantousyaName;
        private Common.Control.TextBoxA txtTantousyaNameKana;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private Common.Control.CheckBoxA chkInvalid;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUpdateUserCD;
        private System.Windows.Forms.Label lblUpdateUserName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource kanriBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblCreateDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCreateUserName;
        private System.Windows.Forms.Label lblCreateUserCD;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label11;
        private Common.Control.TextBoxA txtZipCode;
        private System.Windows.Forms.Label label5;
        private Common.Control.TextBoxA txtFax;
        private System.Windows.Forms.Label label12;
        private Common.Control.TextBoxA txtTel;
        private Common.Control.TextBoxA txtAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private Common.Control.TextBoxA txtBoss;
        private Common.Control.TextBoxAN txtShimebi;
    }
}
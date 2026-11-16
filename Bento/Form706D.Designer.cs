namespace Emu.Bento
{
    partial class Form706D
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
            this.tantousyaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtTantousyaCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtTantousyaNameKana = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.chkInvalid = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.lblUpdateUserCD = new System.Windows.Forms.Label();
            this.lblUpdateUserName = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboEigyoKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label15 = new System.Windows.Forms.Label();
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
            this.txtUserID = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.txtPassword = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.cboKengen = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tantousyaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "担当者名";
            // 
            // txtTantousyaName
            // 
            this.txtTantousyaName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "担当者名", true));
            this.txtTantousyaName.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTantousyaName.Location = new System.Drawing.Point(115, 40);
            this.txtTantousyaName.MaxLength = 30;
            this.txtTantousyaName.Name = "txtTantousyaName";
            this.txtTantousyaName.Size = new System.Drawing.Size(190, 22);
            this.txtTantousyaName.TabIndex = 3;
            // 
            // tantousyaBindingSource
            // 
            this.tantousyaBindingSource.DataSource = typeof(Emu.Bento.Data.Tantousya);
            // 
            // txtTantousyaCD
            // 
            this.txtTantousyaCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "担当者ＣＤ", true));
            this.txtTantousyaCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTantousyaCD.Location = new System.Drawing.Point(115, 12);
            this.txtTantousyaCD.MaxLength = 9;
            this.txtTantousyaCD.Name = "txtTantousyaCD";
            this.txtTantousyaCD.Size = new System.Drawing.Size(83, 22);
            this.txtTantousyaCD.TabIndex = 1;
            this.txtTantousyaCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "担当者CD";
            // 
            // txtTantousyaNameKana
            // 
            this.txtTantousyaNameKana.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "担当者名カナ", true));
            this.txtTantousyaNameKana.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTantousyaNameKana.Location = new System.Drawing.Point(115, 68);
            this.txtTantousyaNameKana.MaxLength = 30;
            this.txtTantousyaNameKana.Name = "txtTantousyaNameKana";
            this.txtTantousyaNameKana.Size = new System.Drawing.Size(190, 22);
            this.txtTantousyaNameKana.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "担当者名カナ";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.tantousyaBindingSource, "所属部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(115, 208);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 211);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 15);
            this.label8.TabIndex = 14;
            this.label8.Text = "所属部署";
            // 
            // chkInvalid
            // 
            this.chkInvalid.AutoSize = true;
            this.chkInvalid.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.tantousyaBindingSource, "無効フラグ", true));
            this.chkInvalid.Location = new System.Drawing.Point(12, 351);
            this.chkInvalid.Name = "chkInvalid";
            this.chkInvalid.Size = new System.Drawing.Size(56, 19);
            this.chkInvalid.TabIndex = 24;
            this.chkInvalid.Text = "無効";
            this.chkInvalid.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 437);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 30;
            this.label10.Text = "修正担当者";
            // 
            // lblUpdateUserCD
            // 
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(102, 433);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 31;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUpdateUserName
            // 
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(183, 433);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 32;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 465);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 33;
            this.label13.Text = "修正日時";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(102, 461);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 34;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMode
            // 
            this.lblMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblMode.ForeColor = System.Drawing.Color.Black;
            this.lblMode.Location = new System.Drawing.Point(12, 497);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(82, 21);
            this.lblMode.TabIndex = 35;
            this.lblMode.Text = "新規モード";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(102, 495);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 36;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(198, 495);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 25);
            this.btnDelete.TabIndex = 37;
            this.btnDelete.Text = "削除(F11)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(294, 495);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 38;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.tantousyaBindingSource;
            // 
            // cboEigyoKbn
            // 
            this.cboEigyoKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboEigyoKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboEigyoKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboEigyoKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.tantousyaBindingSource, "営業業務区分", true));
            this.cboEigyoKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboEigyoKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEigyoKbn.FormattingEnabled = true;
            this.cboEigyoKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboEigyoKbn.Location = new System.Drawing.Point(115, 322);
            this.cboEigyoKbn.Name = "cboEigyoKbn";
            this.cboEigyoKbn.Size = new System.Drawing.Size(121, 23);
            this.cboEigyoKbn.TabIndex = 23;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 325);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(97, 15);
            this.label15.TabIndex = 22;
            this.label15.Text = "営業業務区分";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 15);
            this.label11.TabIndex = 6;
            this.label11.Text = "郵便番号";
            // 
            // txtZipCode
            // 
            this.txtZipCode.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "郵便番号", true));
            this.txtZipCode.Location = new System.Drawing.Point(115, 96);
            this.txtZipCode.MaxLength = 8;
            this.txtZipCode.Name = "txtZipCode";
            this.txtZipCode.Size = new System.Drawing.Size(74, 22);
            this.txtZipCode.TabIndex = 7;
            // 
            // lblCreateDate
            // 
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 405);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 29;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 409);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 28;
            this.label16.Text = "作成日時";
            // 
            // lblCreateUserName
            // 
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 377);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 27;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCreateUserCD
            // 
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 377);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 26;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 381);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 25;
            this.label19.Text = "作成担当者";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "住所";
            // 
            // txtAddress
            // 
            this.txtAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "住所", true));
            this.txtAddress.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtAddress.Location = new System.Drawing.Point(115, 124);
            this.txtAddress.MaxLength = 120;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(249, 22);
            this.txtAddress.TabIndex = 9;
            // 
            // txtTel
            // 
            this.txtTel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "電話番号", true));
            this.txtTel.Location = new System.Drawing.Point(115, 152);
            this.txtTel.MaxLength = 15;
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(121, 22);
            this.txtTel.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 155);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "電話番号";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "FAX";
            // 
            // txtFax
            // 
            this.txtFax.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "ＦＡＸ", true));
            this.txtFax.Location = new System.Drawing.Point(115, 180);
            this.txtFax.MaxLength = 15;
            this.txtFax.Name = "txtFax";
            this.txtFax.Size = new System.Drawing.Size(121, 22);
            this.txtFax.TabIndex = 13;
            // 
            // txtUserID
            // 
            this.txtUserID.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "ユーザーＩＤ", true));
            this.txtUserID.Location = new System.Drawing.Point(115, 237);
            this.txtUserID.MaxLength = 20;
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(172, 22);
            this.txtUserID.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 240);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 15);
            this.label6.TabIndex = 16;
            this.label6.Text = "ユーザーＩＤ";
            // 
            // txtPassword
            // 
            this.txtPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "ユーザーＰＷ", true));
            this.txtPassword.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtPassword.Location = new System.Drawing.Point(115, 265);
            this.txtPassword.MaxLength = 20;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(172, 22);
            this.txtPassword.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 268);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 15);
            this.label7.TabIndex = 18;
            this.label7.Text = "パスワード";
            // 
            // cboKengen
            // 
            this.cboKengen.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboKengen.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboKengen.BackColor = System.Drawing.SystemColors.Control;
            this.cboKengen.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.tantousyaBindingSource, "権限区分", true));
            this.cboKengen.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboKengen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKengen.FormattingEnabled = true;
            this.cboKengen.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboKengen.Location = new System.Drawing.Point(115, 293);
            this.cboKengen.Name = "cboKengen";
            this.cboKengen.Size = new System.Drawing.Size(121, 23);
            this.cboKengen.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 296);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 20;
            this.label9.Text = "権限";
            // 
            // Form706D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 532);
            this.Controls.Add(this.cboKengen);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtUserID);
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
            this.Controls.Add(this.cboEigyoKbn);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.lblUpdateDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblUpdateUserName);
            this.Controls.Add(this.lblUpdateUserCD);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.chkInvalid);
            this.Controls.Add(this.cboBusyo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTantousyaNameKana);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTantousyaName);
            this.Controls.Add(this.txtTantousyaCD);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form706D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "担当者マスタ詳細";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form706D_FormClosing);
            this.Load += new System.EventHandler(this.Form706D_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form706D_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.tantousyaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtTantousyaName;
        private Common.Control.TextBoxAN txtTantousyaCD;
        private System.Windows.Forms.Label label1;
        private Common.Control.TextBoxA txtTantousyaNameKana;
        private System.Windows.Forms.Label label4;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Label label8;
        private Common.Control.CheckBoxA chkInvalid;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUpdateUserCD;
        private System.Windows.Forms.Label lblUpdateUserName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource tantousyaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Common.Control.ComboBoxA cboEigyoKbn;
        private System.Windows.Forms.Label label15;
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
        private Common.Control.ComboBoxA cboKengen;
        private System.Windows.Forms.Label label9;
        private Common.Control.TextBoxA txtPassword;
        private System.Windows.Forms.Label label7;
        private Common.Control.TextBoxA txtUserID;
        private System.Windows.Forms.Label label6;
    }
}
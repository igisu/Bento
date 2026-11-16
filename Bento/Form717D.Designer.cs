namespace Emu.Bento
{
    partial class Form717D
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
            this.txtZeiritsu = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.shiwakeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtTekiyo = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.cboKashiHojo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.txtKashiBumonCD = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtKashiCD = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.cboKariHojo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.txtKariBumonCD = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtKariCD = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.cboNyukinHoho = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.chkSyukeiCD = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.cboUriageKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.txtSyukeiCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.cboSyukeiKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCreateDate = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblCreateUserName = new System.Windows.Forms.Label();
            this.lblCreateUserCD = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblMode = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblUpdateUserName = new System.Windows.Forms.Label();
            this.lblUpdateUserCD = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.chkTicket = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSyoriNaiyo = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtShiwakeCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.shiwakeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(294, 592);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 45;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // txtZeiritsu
            // 
            this.txtZeiritsu.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "税率", true));
            this.txtZeiritsu.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtZeiritsu.Location = new System.Drawing.Point(115, 435);
            this.txtZeiritsu.MaxLength = 25;
            this.txtZeiritsu.Name = "txtZeiritsu";
            this.txtZeiritsu.Size = new System.Drawing.Size(150, 22);
            this.txtZeiritsu.TabIndex = 31;
            // 
            // shiwakeBindingSource
            // 
            this.shiwakeBindingSource.DataSource = typeof(Emu.Bento.Data.Shiwake);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(12, 438);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(67, 15);
            this.label18.TabIndex = 30;
            this.label18.Text = "税率区分";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 410);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(37, 15);
            this.label17.TabIndex = 28;
            this.label17.Text = "摘要";
            // 
            // txtTekiyo
            // 
            this.txtTekiyo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "摘要", true));
            this.txtTekiyo.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTekiyo.Location = new System.Drawing.Point(115, 407);
            this.txtTekiyo.MaxLength = 25;
            this.txtTekiyo.Name = "txtTekiyo";
            this.txtTekiyo.Size = new System.Drawing.Size(190, 22);
            this.txtTekiyo.TabIndex = 29;
            // 
            // cboKashiHojo
            // 
            this.cboKashiHojo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboKashiHojo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboKashiHojo.BackColor = System.Drawing.SystemColors.Control;
            this.cboKashiHojo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "貸方補助ＣＤ", true));
            this.cboKashiHojo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboKashiHojo.FormattingEnabled = true;
            this.cboKashiHojo.ImeMode = System.Windows.Forms.ImeMode.On;
            this.cboKashiHojo.Location = new System.Drawing.Point(115, 378);
            this.cboKashiHojo.MaxLength = 25;
            this.cboKashiHojo.Name = "cboKashiHojo";
            this.cboKashiHojo.Size = new System.Drawing.Size(191, 23);
            this.cboKashiHojo.TabIndex = 27;
            // 
            // txtKashiBumonCD
            // 
            this.txtKashiBumonCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "貸方部門ＣＤ", true));
            this.txtKashiBumonCD.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtKashiBumonCD.Location = new System.Drawing.Point(115, 350);
            this.txtKashiBumonCD.MaxLength = 25;
            this.txtKashiBumonCD.Name = "txtKashiBumonCD";
            this.txtKashiBumonCD.Size = new System.Drawing.Size(150, 22);
            this.txtKashiBumonCD.TabIndex = 25;
            // 
            // txtKashiCD
            // 
            this.txtKashiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "貸方ＣＤ", true));
            this.txtKashiCD.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtKashiCD.Location = new System.Drawing.Point(115, 322);
            this.txtKashiCD.MaxLength = 25;
            this.txtKashiCD.Name = "txtKashiCD";
            this.txtKashiCD.Size = new System.Drawing.Size(150, 22);
            this.txtKashiCD.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 381);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 15);
            this.label9.TabIndex = 26;
            this.label9.Text = "貸方補助";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 353);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(67, 15);
            this.label14.TabIndex = 24;
            this.label14.Text = "貸方部門";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 325);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 15);
            this.label15.TabIndex = 22;
            this.label15.Text = "貸方";
            // 
            // cboKariHojo
            // 
            this.cboKariHojo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboKariHojo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboKariHojo.BackColor = System.Drawing.SystemColors.Control;
            this.cboKariHojo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "借方補助ＣＤ", true));
            this.cboKariHojo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboKariHojo.FormattingEnabled = true;
            this.cboKariHojo.ImeMode = System.Windows.Forms.ImeMode.On;
            this.cboKariHojo.Location = new System.Drawing.Point(115, 293);
            this.cboKariHojo.MaxLength = 25;
            this.cboKariHojo.Name = "cboKariHojo";
            this.cboKariHojo.Size = new System.Drawing.Size(191, 23);
            this.cboKariHojo.TabIndex = 21;
            // 
            // txtKariBumonCD
            // 
            this.txtKariBumonCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "借方部門ＣＤ", true));
            this.txtKariBumonCD.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtKariBumonCD.Location = new System.Drawing.Point(115, 265);
            this.txtKariBumonCD.MaxLength = 25;
            this.txtKariBumonCD.Name = "txtKariBumonCD";
            this.txtKariBumonCD.Size = new System.Drawing.Size(150, 22);
            this.txtKariBumonCD.TabIndex = 19;
            // 
            // txtKariCD
            // 
            this.txtKariCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "借方ＣＤ", true));
            this.txtKariCD.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtKariCD.Location = new System.Drawing.Point(115, 237);
            this.txtKariCD.MaxLength = 25;
            this.txtKariCD.Name = "txtKariCD";
            this.txtKariCD.Size = new System.Drawing.Size(150, 22);
            this.txtKariCD.TabIndex = 17;
            // 
            // cboNyukinHoho
            // 
            this.cboNyukinHoho.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboNyukinHoho.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboNyukinHoho.BackColor = System.Drawing.SystemColors.Control;
            this.cboNyukinHoho.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.shiwakeBindingSource, "入金方法", true));
            this.cboNyukinHoho.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboNyukinHoho.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNyukinHoho.FormattingEnabled = true;
            this.cboNyukinHoho.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboNyukinHoho.Location = new System.Drawing.Point(115, 180);
            this.cboNyukinHoho.Name = "cboNyukinHoho";
            this.cboNyukinHoho.Size = new System.Drawing.Size(121, 23);
            this.cboNyukinHoho.TabIndex = 13;
            // 
            // chkSyukeiCD
            // 
            this.chkSyukeiCD.AutoSize = true;
            this.chkSyukeiCD.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.shiwakeBindingSource, "集計ＣＤ使用フラグ", true));
            this.chkSyukeiCD.Location = new System.Drawing.Point(115, 97);
            this.chkSyukeiCD.Name = "chkSyukeiCD";
            this.chkSyukeiCD.Size = new System.Drawing.Size(250, 19);
            this.chkSyukeiCD.TabIndex = 6;
            this.chkSyukeiCD.Text = "得意先の集計CDと紐付けて集計する";
            this.chkSyukeiCD.UseVisualStyleBackColor = true;
            // 
            // cboUriageKbn
            // 
            this.cboUriageKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboUriageKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboUriageKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboUriageKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.shiwakeBindingSource, "売上区分", true));
            this.cboUriageKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboUriageKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUriageKbn.FormattingEnabled = true;
            this.cboUriageKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboUriageKbn.Location = new System.Drawing.Point(115, 151);
            this.cboUriageKbn.Name = "cboUriageKbn";
            this.cboUriageKbn.Size = new System.Drawing.Size(121, 23);
            this.cboUriageKbn.TabIndex = 10;
            // 
            // txtSyukeiCD
            // 
            this.txtSyukeiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "集計ＣＤ", true));
            this.txtSyukeiCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSyukeiCD.Location = new System.Drawing.Point(115, 69);
            this.txtSyukeiCD.MaxLength = 9;
            this.txtSyukeiCD.Name = "txtSyukeiCD";
            this.txtSyukeiCD.Size = new System.Drawing.Size(83, 22);
            this.txtSyukeiCD.TabIndex = 5;
            this.txtSyukeiCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cboSyukeiKbn
            // 
            this.cboSyukeiKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSyukeiKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSyukeiKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboSyukeiKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.shiwakeBindingSource, "集計対象区分", true));
            this.cboSyukeiKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSyukeiKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSyukeiKbn.FormattingEnabled = true;
            this.cboSyukeiKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboSyukeiKbn.Location = new System.Drawing.Point(115, 122);
            this.cboSyukeiKbn.Name = "cboSyukeiKbn";
            this.cboSyukeiKbn.Size = new System.Drawing.Size(121, 23);
            this.cboSyukeiKbn.TabIndex = 8;
            this.cboSyukeiKbn.SelectedValueChanged += new System.EventHandler(this.cboSyukeiKbn_SelectedValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 296);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 15);
            this.label7.TabIndex = 20;
            this.label7.Text = "借方補助";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 268);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "借方部門";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "処理内容";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 183);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 15);
            this.label12.TabIndex = 12;
            this.label12.Text = "入金方法";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "売上区分";
            // 
            // lblCreateDate
            // 
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 502);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 36;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 506);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 35;
            this.label16.Text = "作成日時";
            // 
            // lblCreateUserName
            // 
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 474);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 34;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCreateUserCD
            // 
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 474);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 33;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 478);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 32;
            this.label19.Text = "作成担当者";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 125);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 15);
            this.label11.TabIndex = 7;
            this.label11.Text = "集計対象区分";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(198, 592);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 25);
            this.btnDelete.TabIndex = 44;
            this.btnDelete.Text = "削除(F11)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(102, 592);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 43;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblMode
            // 
            this.lblMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblMode.ForeColor = System.Drawing.Color.Black;
            this.lblMode.Location = new System.Drawing.Point(12, 594);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(82, 21);
            this.lblMode.TabIndex = 42;
            this.lblMode.Text = "新規モード";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(102, 558);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 41;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 562);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 40;
            this.label13.Text = "修正日時";
            // 
            // lblUpdateUserName
            // 
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(183, 530);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 39;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUpdateUserCD
            // 
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(102, 530);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 38;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 534);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 37;
            this.label10.Text = "修正担当者";
            // 
            // chkTicket
            // 
            this.chkTicket.AutoSize = true;
            this.chkTicket.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.shiwakeBindingSource, "チケット束フラグ", true));
            this.chkTicket.Location = new System.Drawing.Point(242, 153);
            this.chkTicket.Name = "chkTicket";
            this.chkTicket.Size = new System.Drawing.Size(109, 19);
            this.chkTicket.TabIndex = 11;
            this.chkTicket.Text = "チケット束売り";
            this.chkTicket.UseVisualStyleBackColor = true;
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.shiwakeBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(115, 12);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 240);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "借方";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "集計CD";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "仕訳CD";
            // 
            // txtSyoriNaiyo
            // 
            this.txtSyoriNaiyo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "処理内容", true));
            this.txtSyoriNaiyo.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtSyoriNaiyo.Location = new System.Drawing.Point(115, 209);
            this.txtSyoriNaiyo.MaxLength = 25;
            this.txtSyoriNaiyo.Name = "txtSyoriNaiyo";
            this.txtSyoriNaiyo.Size = new System.Drawing.Size(190, 22);
            this.txtSyoriNaiyo.TabIndex = 15;
            // 
            // txtShiwakeCD
            // 
            this.txtShiwakeCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.shiwakeBindingSource, "仕訳ＣＤ", true));
            this.txtShiwakeCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtShiwakeCD.Location = new System.Drawing.Point(115, 41);
            this.txtShiwakeCD.MaxLength = 9;
            this.txtShiwakeCD.Name = "txtShiwakeCD";
            this.txtShiwakeCD.Size = new System.Drawing.Size(83, 22);
            this.txtShiwakeCD.TabIndex = 3;
            this.txtShiwakeCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "部署CD";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.shiwakeBindingSource;
            // 
            // Form717D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 629);
            this.Controls.Add(this.txtZeiritsu);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtTekiyo);
            this.Controls.Add(this.cboKashiHojo);
            this.Controls.Add(this.txtKashiBumonCD);
            this.Controls.Add(this.txtKashiCD);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.cboKariHojo);
            this.Controls.Add(this.txtKariBumonCD);
            this.Controls.Add(this.txtKariCD);
            this.Controls.Add(this.cboNyukinHoho);
            this.Controls.Add(this.chkSyukeiCD);
            this.Controls.Add(this.cboUriageKbn);
            this.Controls.Add(this.txtSyukeiCD);
            this.Controls.Add(this.cboSyukeiKbn);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblCreateDate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCreateUserName);
            this.Controls.Add(this.lblCreateUserCD);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.lblUpdateDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblUpdateUserName);
            this.Controls.Add(this.lblUpdateUserCD);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.chkTicket);
            this.Controls.Add(this.cboBusyo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSyoriNaiyo);
            this.Controls.Add(this.txtShiwakeCD);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form717D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "仕訳マスタ詳細";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form717D_FormClosing);
            this.Load += new System.EventHandler(this.Form717D_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form717D_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.shiwakeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtSyoriNaiyo;
        private Common.Control.TextBoxAN txtShiwakeCD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Label label8;
        private Common.Control.CheckBoxA chkTicket;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUpdateUserCD;
        private System.Windows.Forms.Label lblUpdateUserName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource shiwakeBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblCreateDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCreateUserName;
        private System.Windows.Forms.Label lblCreateUserCD;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private Common.Control.ComboBoxA cboSyukeiKbn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private Common.Control.TextBoxAN txtSyukeiCD;
        private Common.Control.ComboBoxA cboUriageKbn;
        private Common.Control.TextBoxA txtKariCD;
        private Common.Control.ComboBoxA cboNyukinHoho;
        private Common.Control.CheckBoxA chkSyukeiCD;
        private Common.Control.TextBoxA txtKariBumonCD;
        private Common.Control.ComboBoxA cboKashiHojo;
        private Common.Control.TextBoxA txtKashiBumonCD;
        private Common.Control.TextBoxA txtKashiCD;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private Common.Control.ComboBoxA cboKariHojo;
        private System.Windows.Forms.Label label17;
        private Common.Control.TextBoxA txtTekiyo;
        private Common.Control.TextBoxA txtZeiritsu;
        private System.Windows.Forms.Label label18;
    }
}
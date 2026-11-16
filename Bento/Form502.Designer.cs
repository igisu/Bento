namespace Emu.Bento
{
    partial class Form502
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
            this.seikyuKagamiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblUpdateUserCD = new System.Windows.Forms.Label();
            this.lblUpdateUserName = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblCreateDate = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblCreateUserName = new System.Windows.Forms.Label();
            this.lblCreateUserCD = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblSeikyuNo = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblSeikyuName = new System.Windows.Forms.Label();
            this.grpZenZen = new System.Windows.Forms.GroupBox();
            this.lblSyohizei2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblSeikyuZandaka2 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblAdjust2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblMonthUriage2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCarryover2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNyukinAmount2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblZenkaiSeikyuAmount2 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grpZen = new System.Windows.Forms.GroupBox();
            this.lblSyohizei1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblSeikyuZandaka1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAdjust1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblMonthUriage1 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblCarryover1 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.lblNyukinAmount1 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lblZenkaiSeikyuAmount1 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.grpKon = new System.Windows.Forms.GroupBox();
            this.txtSyohizei = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.txtSeikyuZandaka = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.txtAdjust = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.txtMonthUriage = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.txtCarryover = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.txtNyukinAmount = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.txtZenkaiSeikyuAmount = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.label26 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuKagamiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.grpZenZen.SuspendLayout();
            this.grpZen.SuspendLayout();
            this.grpKon.SuspendLayout();
            this.SuspendLayout();
            // 
            // seikyuKagamiBindingSource
            // 
            this.seikyuKagamiBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuKagami);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "請求番号";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 365);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 12;
            this.label10.Text = "修正担当者";
            // 
            // lblUpdateUserCD
            // 
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(102, 361);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 13;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUpdateUserName
            // 
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(183, 361);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 14;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 393);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 15;
            this.label13.Text = "修正日時";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(102, 389);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 16;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(584, 388);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "更新(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(680, 388);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 18;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.seikyuKagamiBindingSource;
            // 
            // lblCreateDate
            // 
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 333);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 11;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 337);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 10;
            this.label16.Text = "作成日時";
            // 
            // lblCreateUserName
            // 
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 305);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 9;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCreateUserCD
            // 
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 305);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 8;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 309);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 7;
            this.label19.Text = "作成担当者";
            // 
            // lblSeikyuNo
            // 
            this.lblSeikyuNo.AutoEllipsis = true;
            this.lblSeikyuNo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuNo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "請求番号", true));
            this.lblSeikyuNo.Location = new System.Drawing.Point(85, 11);
            this.lblSeikyuNo.Name = "lblSeikyuNo";
            this.lblSeikyuNo.Size = new System.Drawing.Size(91, 22);
            this.lblSeikyuNo.TabIndex = 1;
            this.lblSeikyuNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(226, 15);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 2;
            this.label14.Text = "請求先";
            // 
            // lblSeikyuName
            // 
            this.lblSeikyuName.AutoEllipsis = true;
            this.lblSeikyuName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "請求先名", true));
            this.lblSeikyuName.Location = new System.Drawing.Point(284, 11);
            this.lblSeikyuName.Name = "lblSeikyuName";
            this.lblSeikyuName.Size = new System.Drawing.Size(153, 22);
            this.lblSeikyuName.TabIndex = 3;
            this.lblSeikyuName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpZenZen
            // 
            this.grpZenZen.Controls.Add(this.lblSyohizei2);
            this.grpZenZen.Controls.Add(this.label8);
            this.grpZenZen.Controls.Add(this.lblSeikyuZandaka2);
            this.grpZenZen.Controls.Add(this.label12);
            this.grpZenZen.Controls.Add(this.lblAdjust2);
            this.grpZenZen.Controls.Add(this.label9);
            this.grpZenZen.Controls.Add(this.lblMonthUriage2);
            this.grpZenZen.Controls.Add(this.label7);
            this.grpZenZen.Controls.Add(this.lblCarryover2);
            this.grpZenZen.Controls.Add(this.label4);
            this.grpZenZen.Controls.Add(this.lblNyukinAmount2);
            this.grpZenZen.Controls.Add(this.label5);
            this.grpZenZen.Controls.Add(this.lblZenkaiSeikyuAmount2);
            this.grpZenZen.Controls.Add(this.label2);
            this.grpZenZen.Location = new System.Drawing.Point(12, 45);
            this.grpZenZen.Name = "grpZenZen";
            this.grpZenZen.Size = new System.Drawing.Size(755, 72);
            this.grpZenZen.TabIndex = 4;
            this.grpZenZen.TabStop = false;
            this.grpZenZen.Text = "前々回請求内容";
            // 
            // lblSyohizei2
            // 
            this.lblSyohizei2.AutoEllipsis = true;
            this.lblSyohizei2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSyohizei2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回消費税", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblSyohizei2.Location = new System.Drawing.Point(541, 43);
            this.lblSyohizei2.Name = "lblSyohizei2";
            this.lblSyohizei2.Size = new System.Drawing.Size(101, 22);
            this.lblSyohizei2.TabIndex = 11;
            this.lblSyohizei2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(538, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 15);
            this.label8.TabIndex = 10;
            this.label8.Text = "消費税額";
            // 
            // lblSeikyuZandaka2
            // 
            this.lblSeikyuZandaka2.AutoEllipsis = true;
            this.lblSeikyuZandaka2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuZandaka2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回今回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblSeikyuZandaka2.Location = new System.Drawing.Point(648, 43);
            this.lblSeikyuZandaka2.Name = "lblSeikyuZandaka2";
            this.lblSeikyuZandaka2.Size = new System.Drawing.Size(101, 22);
            this.lblSeikyuZandaka2.TabIndex = 13;
            this.lblSeikyuZandaka2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(648, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 15);
            this.label12.TabIndex = 12;
            this.label12.Text = "当月請求額";
            // 
            // lblAdjust2
            // 
            this.lblAdjust2.AutoEllipsis = true;
            this.lblAdjust2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAdjust2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回調整額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblAdjust2.Location = new System.Drawing.Point(434, 43);
            this.lblAdjust2.Name = "lblAdjust2";
            this.lblAdjust2.Size = new System.Drawing.Size(101, 22);
            this.lblAdjust2.TabIndex = 9;
            this.lblAdjust2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(434, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 8;
            this.label9.Text = "調整額";
            // 
            // lblMonthUriage2
            // 
            this.lblMonthUriage2.AutoEllipsis = true;
            this.lblMonthUriage2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMonthUriage2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回買上金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblMonthUriage2.Location = new System.Drawing.Point(327, 43);
            this.lblMonthUriage2.Name = "lblMonthUriage2";
            this.lblMonthUriage2.Size = new System.Drawing.Size(101, 22);
            this.lblMonthUriage2.TabIndex = 7;
            this.lblMonthUriage2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(327, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "当月売上額";
            // 
            // lblCarryover2
            // 
            this.lblCarryover2.AutoEllipsis = true;
            this.lblCarryover2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCarryover2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回繰越額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblCarryover2.Location = new System.Drawing.Point(220, 43);
            this.lblCarryover2.Name = "lblCarryover2";
            this.lblCarryover2.Size = new System.Drawing.Size(101, 22);
            this.lblCarryover2.TabIndex = 5;
            this.lblCarryover2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(220, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "繰越額";
            // 
            // lblNyukinAmount2
            // 
            this.lblNyukinAmount2.AutoEllipsis = true;
            this.lblNyukinAmount2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNyukinAmount2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回入金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblNyukinAmount2.Location = new System.Drawing.Point(113, 43);
            this.lblNyukinAmount2.Name = "lblNyukinAmount2";
            this.lblNyukinAmount2.Size = new System.Drawing.Size(101, 22);
            this.lblNyukinAmount2.TabIndex = 3;
            this.lblNyukinAmount2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(113, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "入金額";
            // 
            // lblZenkaiSeikyuAmount2
            // 
            this.lblZenkaiSeikyuAmount2.AutoEllipsis = true;
            this.lblZenkaiSeikyuAmount2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblZenkaiSeikyuAmount2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前々回前回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblZenkaiSeikyuAmount2.Location = new System.Drawing.Point(6, 43);
            this.lblZenkaiSeikyuAmount2.Name = "lblZenkaiSeikyuAmount2";
            this.lblZenkaiSeikyuAmount2.Size = new System.Drawing.Size(101, 22);
            this.lblZenkaiSeikyuAmount2.TabIndex = 1;
            this.lblZenkaiSeikyuAmount2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "前回請求額";
            // 
            // grpZen
            // 
            this.grpZen.Controls.Add(this.lblSyohizei1);
            this.grpZen.Controls.Add(this.label15);
            this.grpZen.Controls.Add(this.lblSeikyuZandaka1);
            this.grpZen.Controls.Add(this.label6);
            this.grpZen.Controls.Add(this.lblAdjust1);
            this.grpZen.Controls.Add(this.label11);
            this.grpZen.Controls.Add(this.lblMonthUriage1);
            this.grpZen.Controls.Add(this.label17);
            this.grpZen.Controls.Add(this.lblCarryover1);
            this.grpZen.Controls.Add(this.label20);
            this.grpZen.Controls.Add(this.lblNyukinAmount1);
            this.grpZen.Controls.Add(this.label22);
            this.grpZen.Controls.Add(this.lblZenkaiSeikyuAmount1);
            this.grpZen.Controls.Add(this.label24);
            this.grpZen.Location = new System.Drawing.Point(12, 132);
            this.grpZen.Name = "grpZen";
            this.grpZen.Size = new System.Drawing.Size(755, 72);
            this.grpZen.TabIndex = 5;
            this.grpZen.TabStop = false;
            this.grpZen.Text = "前回請求内容";
            // 
            // lblSyohizei1
            // 
            this.lblSyohizei1.AutoEllipsis = true;
            this.lblSyohizei1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSyohizei1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回消費税", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblSyohizei1.Location = new System.Drawing.Point(541, 43);
            this.lblSyohizei1.Name = "lblSyohizei1";
            this.lblSyohizei1.Size = new System.Drawing.Size(101, 22);
            this.lblSyohizei1.TabIndex = 11;
            this.lblSyohizei1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(541, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(67, 15);
            this.label15.TabIndex = 10;
            this.label15.Text = "消費税額";
            // 
            // lblSeikyuZandaka1
            // 
            this.lblSeikyuZandaka1.AutoEllipsis = true;
            this.lblSeikyuZandaka1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuZandaka1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回今回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblSeikyuZandaka1.Location = new System.Drawing.Point(648, 43);
            this.lblSeikyuZandaka1.Name = "lblSeikyuZandaka1";
            this.lblSeikyuZandaka1.Size = new System.Drawing.Size(101, 22);
            this.lblSeikyuZandaka1.TabIndex = 13;
            this.lblSeikyuZandaka1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(648, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "当月請求額";
            // 
            // lblAdjust1
            // 
            this.lblAdjust1.AutoEllipsis = true;
            this.lblAdjust1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAdjust1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回調整額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblAdjust1.Location = new System.Drawing.Point(434, 43);
            this.lblAdjust1.Name = "lblAdjust1";
            this.lblAdjust1.Size = new System.Drawing.Size(101, 22);
            this.lblAdjust1.TabIndex = 9;
            this.lblAdjust1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(434, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 15);
            this.label11.TabIndex = 8;
            this.label11.Text = "調整額";
            // 
            // lblMonthUriage1
            // 
            this.lblMonthUriage1.AutoEllipsis = true;
            this.lblMonthUriage1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMonthUriage1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回買上金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblMonthUriage1.Location = new System.Drawing.Point(327, 43);
            this.lblMonthUriage1.Name = "lblMonthUriage1";
            this.lblMonthUriage1.Size = new System.Drawing.Size(101, 22);
            this.lblMonthUriage1.TabIndex = 7;
            this.lblMonthUriage1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(327, 25);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 15);
            this.label17.TabIndex = 6;
            this.label17.Text = "当月売上額";
            // 
            // lblCarryover1
            // 
            this.lblCarryover1.AutoEllipsis = true;
            this.lblCarryover1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCarryover1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回繰越額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblCarryover1.Location = new System.Drawing.Point(220, 43);
            this.lblCarryover1.Name = "lblCarryover1";
            this.lblCarryover1.Size = new System.Drawing.Size(101, 22);
            this.lblCarryover1.TabIndex = 5;
            this.lblCarryover1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(220, 25);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(52, 15);
            this.label20.TabIndex = 4;
            this.label20.Text = "繰越額";
            // 
            // lblNyukinAmount1
            // 
            this.lblNyukinAmount1.AutoEllipsis = true;
            this.lblNyukinAmount1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNyukinAmount1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回入金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblNyukinAmount1.Location = new System.Drawing.Point(113, 43);
            this.lblNyukinAmount1.Name = "lblNyukinAmount1";
            this.lblNyukinAmount1.Size = new System.Drawing.Size(101, 22);
            this.lblNyukinAmount1.TabIndex = 3;
            this.lblNyukinAmount1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(113, 25);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(52, 15);
            this.label22.TabIndex = 2;
            this.label22.Text = "入金額";
            // 
            // lblZenkaiSeikyuAmount1
            // 
            this.lblZenkaiSeikyuAmount1.AutoEllipsis = true;
            this.lblZenkaiSeikyuAmount1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblZenkaiSeikyuAmount1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回前回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.lblZenkaiSeikyuAmount1.Location = new System.Drawing.Point(6, 43);
            this.lblZenkaiSeikyuAmount1.Name = "lblZenkaiSeikyuAmount1";
            this.lblZenkaiSeikyuAmount1.Size = new System.Drawing.Size(101, 22);
            this.lblZenkaiSeikyuAmount1.TabIndex = 1;
            this.lblZenkaiSeikyuAmount1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 25);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(82, 15);
            this.label24.TabIndex = 0;
            this.label24.Text = "前回請求額";
            // 
            // grpKon
            // 
            this.grpKon.Controls.Add(this.txtSyohizei);
            this.grpKon.Controls.Add(this.label3);
            this.grpKon.Controls.Add(this.txtSeikyuZandaka);
            this.grpKon.Controls.Add(this.txtAdjust);
            this.grpKon.Controls.Add(this.txtMonthUriage);
            this.grpKon.Controls.Add(this.txtCarryover);
            this.grpKon.Controls.Add(this.txtNyukinAmount);
            this.grpKon.Controls.Add(this.txtZenkaiSeikyuAmount);
            this.grpKon.Controls.Add(this.label26);
            this.grpKon.Controls.Add(this.label28);
            this.grpKon.Controls.Add(this.label30);
            this.grpKon.Controls.Add(this.label32);
            this.grpKon.Controls.Add(this.label34);
            this.grpKon.Controls.Add(this.label36);
            this.grpKon.Location = new System.Drawing.Point(12, 221);
            this.grpKon.Name = "grpKon";
            this.grpKon.Size = new System.Drawing.Size(755, 72);
            this.grpKon.TabIndex = 6;
            this.grpKon.TabStop = false;
            this.grpKon.Text = "今回請求内容";
            // 
            // txtSyohizei
            // 
            this.txtSyohizei.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "消費税", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtSyohizei.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSyohizei.Location = new System.Drawing.Point(541, 43);
            this.txtSyohizei.Name = "txtSyohizei";
            this.txtSyohizei.Size = new System.Drawing.Size(101, 22);
            this.txtSyohizei.TabIndex = 11;
            this.txtSyohizei.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSyohizei.Validated += new System.EventHandler(this.txtCarryoverMonthUriageAdjustTax_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(541, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "消費税額";
            // 
            // txtSeikyuZandaka
            // 
            this.txtSeikyuZandaka.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "今回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtSeikyuZandaka.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSeikyuZandaka.Location = new System.Drawing.Point(648, 43);
            this.txtSeikyuZandaka.Name = "txtSeikyuZandaka";
            this.txtSeikyuZandaka.ReadOnly = true;
            this.txtSeikyuZandaka.Size = new System.Drawing.Size(101, 22);
            this.txtSeikyuZandaka.TabIndex = 13;
            this.txtSeikyuZandaka.TabStop = false;
            this.txtSeikyuZandaka.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAdjust
            // 
            this.txtAdjust.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "調整額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtAdjust.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtAdjust.Location = new System.Drawing.Point(434, 43);
            this.txtAdjust.Name = "txtAdjust";
            this.txtAdjust.Size = new System.Drawing.Size(101, 22);
            this.txtAdjust.TabIndex = 9;
            this.txtAdjust.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAdjust.Validated += new System.EventHandler(this.txtCarryoverMonthUriageAdjustTax_Validated);
            // 
            // txtMonthUriage
            // 
            this.txtMonthUriage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "買上金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtMonthUriage.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtMonthUriage.Location = new System.Drawing.Point(327, 43);
            this.txtMonthUriage.Name = "txtMonthUriage";
            this.txtMonthUriage.Size = new System.Drawing.Size(101, 22);
            this.txtMonthUriage.TabIndex = 7;
            this.txtMonthUriage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMonthUriage.Validated += new System.EventHandler(this.txtCarryoverMonthUriageAdjustTax_Validated);
            // 
            // txtCarryover
            // 
            this.txtCarryover.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "繰越額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtCarryover.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCarryover.Location = new System.Drawing.Point(220, 43);
            this.txtCarryover.Name = "txtCarryover";
            this.txtCarryover.Size = new System.Drawing.Size(101, 22);
            this.txtCarryover.TabIndex = 5;
            this.txtCarryover.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCarryover.Validated += new System.EventHandler(this.txtCarryoverMonthUriageAdjustTax_Validated);
            // 
            // txtNyukinAmount
            // 
            this.txtNyukinAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "入金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtNyukinAmount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtNyukinAmount.Location = new System.Drawing.Point(113, 43);
            this.txtNyukinAmount.Name = "txtNyukinAmount";
            this.txtNyukinAmount.Size = new System.Drawing.Size(101, 22);
            this.txtNyukinAmount.TabIndex = 3;
            this.txtNyukinAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNyukinAmount.Validated += new System.EventHandler(this.txtZenkaiSeikyuNyukinAmount_Validated);
            // 
            // txtZenkaiSeikyuAmount
            // 
            this.txtZenkaiSeikyuAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuKagamiBindingSource, "前回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtZenkaiSeikyuAmount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtZenkaiSeikyuAmount.Location = new System.Drawing.Point(6, 43);
            this.txtZenkaiSeikyuAmount.Name = "txtZenkaiSeikyuAmount";
            this.txtZenkaiSeikyuAmount.Size = new System.Drawing.Size(101, 22);
            this.txtZenkaiSeikyuAmount.TabIndex = 1;
            this.txtZenkaiSeikyuAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtZenkaiSeikyuAmount.Validated += new System.EventHandler(this.txtZenkaiSeikyuNyukinAmount_Validated);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(648, 25);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(82, 15);
            this.label26.TabIndex = 12;
            this.label26.Text = "当月請求額";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(434, 25);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(52, 15);
            this.label28.TabIndex = 8;
            this.label28.Text = "調整額";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(327, 25);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(82, 15);
            this.label30.TabIndex = 6;
            this.label30.Text = "当月売上額";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(220, 25);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(52, 15);
            this.label32.TabIndex = 4;
            this.label32.Text = "繰越額";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(113, 25);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(52, 15);
            this.label34.TabIndex = 2;
            this.label34.Text = "入金額";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(6, 25);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(82, 15);
            this.label36.TabIndex = 0;
            this.label36.Text = "前回請求額";
            // 
            // Form502
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 425);
            this.Controls.Add(this.grpKon);
            this.Controls.Add(this.grpZen);
            this.Controls.Add(this.grpZenZen);
            this.Controls.Add(this.lblSeikyuName);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lblSeikyuNo);
            this.Controls.Add(this.lblCreateDate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCreateUserName);
            this.Controls.Add(this.lblCreateUserCD);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblUpdateDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblUpdateUserName);
            this.Controls.Add(this.lblUpdateUserCD);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form502";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "請求鑑編集";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form502_FormClosing);
            this.Load += new System.EventHandler(this.Form502_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form502_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.seikyuKagamiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.grpZenZen.ResumeLayout(false);
            this.grpZenZen.PerformLayout();
            this.grpZen.ResumeLayout(false);
            this.grpZen.PerformLayout();
            this.grpKon.ResumeLayout(false);
            this.grpKon.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUpdateUserCD;
        private System.Windows.Forms.Label lblUpdateUserName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource seikyuKagamiBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblCreateDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCreateUserName;
        private System.Windows.Forms.Label lblCreateUserCD;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblSeikyuNo;
        private System.Windows.Forms.Label lblSeikyuName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox grpKon;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.GroupBox grpZen;
        private System.Windows.Forms.Label lblSeikyuZandaka1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAdjust1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblMonthUriage1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblCarryover1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label lblNyukinAmount1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblZenkaiSeikyuAmount1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.GroupBox grpZenZen;
        private System.Windows.Forms.Label lblSeikyuZandaka2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblAdjust2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblMonthUriage2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCarryover2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNyukinAmount2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblZenkaiSeikyuAmount2;
        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxAMC txtSeikyuZandaka;
        private Common.Control.TextBoxAMC txtAdjust;
        private Common.Control.TextBoxAMC txtMonthUriage;
        private Common.Control.TextBoxAMC txtCarryover;
        private Common.Control.TextBoxAMC txtNyukinAmount;
        private Common.Control.TextBoxAMC txtZenkaiSeikyuAmount;
        private Common.Control.TextBoxAMC txtSyohizei;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSyohizei1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblSyohizei2;
        private System.Windows.Forms.Label label8;
    }
}
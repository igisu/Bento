namespace Emu.Bento
{
    partial class Form508
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
            this.seikyuAddBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblUpdateUserCD = new System.Windows.Forms.Label();
            this.seikyuGokeiBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
            this.btnTokuisakiCD = new System.Windows.Forms.Button();
            this.lblTokuisakiName = new System.Windows.Forms.Label();
            this.txtTokuisakiCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.dtpSeikyubi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.cboShime = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboTaxRate = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuAddBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuGokeiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.grpKon.SuspendLayout();
            this.SuspendLayout();
            // 
            // seikyuAddBindingSource
            // 
            this.seikyuAddBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuAdd);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(229, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "請求先";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 219);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 18;
            this.label10.Text = "修正担当者";
            // 
            // lblUpdateUserCD
            // 
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuGokeiBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(102, 215);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 19;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // seikyuGokeiBindingSource
            // 
            this.seikyuGokeiBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuGokei);
            // 
            // lblUpdateUserName
            // 
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuGokeiBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(183, 215);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 20;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 247);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 21;
            this.label13.Text = "修正日時";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuGokeiBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(102, 243);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 22;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(584, 242);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 26;
            this.btnSave.Text = "追加(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(680, 242);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 27;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.seikyuAddBindingSource;
            // 
            // lblCreateDate
            // 
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuGokeiBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 187);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 17;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 191);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 16;
            this.label16.Text = "作成日時";
            // 
            // lblCreateUserName
            // 
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuGokeiBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 159);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 15;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCreateUserCD
            // 
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuGokeiBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 159);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 14;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 163);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 13;
            this.label19.Text = "作成担当者";
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
            this.grpKon.Location = new System.Drawing.Point(12, 75);
            this.grpKon.Name = "grpKon";
            this.grpKon.Size = new System.Drawing.Size(755, 72);
            this.grpKon.TabIndex = 12;
            this.grpKon.TabStop = false;
            this.grpKon.Text = "請求内容";
            // 
            // txtSyohizei
            // 
            this.txtSyohizei.CopyFromWhenEmpty = null;
            this.txtSyohizei.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "消費税額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            this.txtSeikyuZandaka.CopyFromWhenEmpty = null;
            this.txtSeikyuZandaka.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "当月請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            this.txtAdjust.CopyFromWhenEmpty = null;
            this.txtAdjust.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "調整額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            this.txtMonthUriage.CopyFromWhenEmpty = null;
            this.txtMonthUriage.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "当月売上額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            this.txtCarryover.CopyFromWhenEmpty = null;
            this.txtCarryover.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "繰越額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            this.txtNyukinAmount.CopyFromWhenEmpty = null;
            this.txtNyukinAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "入金額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            this.txtZenkaiSeikyuAmount.CopyFromWhenEmpty = null;
            this.txtZenkaiSeikyuAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "前回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
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
            // btnTokuisakiCD
            // 
            this.btnTokuisakiCD.AutoSize = true;
            this.btnTokuisakiCD.Location = new System.Drawing.Point(494, 10);
            this.btnTokuisakiCD.Name = "btnTokuisakiCD";
            this.btnTokuisakiCD.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCD.TabIndex = 5;
            this.btnTokuisakiCD.TabStop = false;
            this.btnTokuisakiCD.Text = "参照";
            this.btnTokuisakiCD.UseVisualStyleBackColor = true;
            this.btnTokuisakiCD.Click += new System.EventHandler(this.btnTokuisakiCD_Click);
            // 
            // lblTokuisakiName
            // 
            this.lblTokuisakiName.AutoEllipsis = true;
            this.lblTokuisakiName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "請求先名", true));
            this.lblTokuisakiName.Location = new System.Drawing.Point(376, 12);
            this.lblTokuisakiName.Name = "lblTokuisakiName";
            this.lblTokuisakiName.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiName.TabIndex = 4;
            this.lblTokuisakiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCD
            // 
            this.txtTokuisakiCD.CopyFromWhenEmpty = null;
            this.txtTokuisakiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "請求先ＣＤ", true));
            this.txtTokuisakiCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCD.Location = new System.Drawing.Point(287, 12);
            this.txtTokuisakiCD.MaxLength = 9;
            this.txtTokuisakiCD.Name = "txtTokuisakiCD";
            this.txtTokuisakiCD.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCD.TabIndex = 3;
            this.txtTokuisakiCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCD_KeyDown);
            // 
            // dtpSeikyubi
            // 
            this.dtpSeikyubi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuAddBindingSource, "請求日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubi.Location = new System.Drawing.Point(287, 40);
            this.dtpSeikyubi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubi.Name = "dtpSeikyubi";
            this.dtpSeikyubi.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubi.TabIndex = 9;
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuAddBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(70, 11);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // cboShime
            // 
            this.cboShime.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboShime.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboShime.BackColor = System.Drawing.SystemColors.Control;
            this.cboShime.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuAddBindingSource, "締区分", true));
            this.cboShime.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboShime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShime.FormattingEnabled = true;
            this.cboShime.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboShime.Location = new System.Drawing.Point(70, 41);
            this.cboShime.Name = "cboShime";
            this.cboShime.Size = new System.Drawing.Size(121, 23);
            this.cboShime.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "締区分";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "請求日";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "部署";
            // 
            // cboTaxRate
            // 
            this.cboTaxRate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboTaxRate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboTaxRate.BackColor = System.Drawing.SystemColors.Control;
            this.cboTaxRate.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuAddBindingSource, "計算税率", true));
            this.cboTaxRate.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTaxRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTaxRate.FormattingEnabled = true;
            this.cboTaxRate.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboTaxRate.Location = new System.Drawing.Point(564, 41);
            this.cboTaxRate.Name = "cboTaxRate";
            this.cboTaxRate.Size = new System.Drawing.Size(121, 23);
            this.cboTaxRate.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(491, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "計算税率";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(339, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(399, 15);
            this.label7.TabIndex = 23;
            this.label7.Text = "既存の得意先が既存の請求先の子から外れた場合に使用します。";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(339, 175);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(362, 30);
            this.label8.TabIndex = 24;
            this.label8.Text = "※新しい得意先が既存の請求先の子になる場合は、\r\n新しい得意先の請求先に既存の得意先を指定してください。";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(339, 210);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(324, 30);
            this.label9.TabIndex = 25;
            this.label9.Text = "※既存の得意先が既存の請求先の子になる場合は、\r\n親の請求を鑑編集してください。";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Location = new System.Drawing.Point(339, 242);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 25);
            this.btnClear.TabIndex = 28;
            this.btnClear.Text = "クリア(F3)";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Form508
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 279);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboTaxRate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dtpSeikyubi);
            this.Controls.Add(this.cboBusyo);
            this.Controls.Add(this.cboShime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnTokuisakiCD);
            this.Controls.Add(this.lblTokuisakiName);
            this.Controls.Add(this.txtTokuisakiCD);
            this.Controls.Add(this.grpKon);
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
            this.Name = "Form508";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "請求レコード追加";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form508_FormClosing);
            this.Load += new System.EventHandler(this.Form508_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form508_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.seikyuAddBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuGokeiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
        private System.Windows.Forms.BindingSource seikyuAddBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label lblCreateDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCreateUserName;
        private System.Windows.Forms.Label lblCreateUserCD;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox grpKon;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label36;
        private Common.Control.TextBoxAMC txtSeikyuZandaka;
        private Common.Control.TextBoxAMC txtAdjust;
        private Common.Control.TextBoxAMC txtMonthUriage;
        private Common.Control.TextBoxAMC txtCarryover;
        private Common.Control.TextBoxAMC txtNyukinAmount;
        private Common.Control.TextBoxAMC txtZenkaiSeikyuAmount;
        private Common.Control.TextBoxAMC txtSyohizei;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTokuisakiCD;
        private System.Windows.Forms.Label lblTokuisakiName;
        private Common.Control.TextBoxAN txtTokuisakiCD;
        private Common.Control.DateTimePickerA dtpSeikyubi;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.ComboBoxA cboShime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private Common.Control.ComboBoxA cboTaxRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.BindingSource seikyuGokeiBindingSource;
        private System.Windows.Forms.Button btnClear;
    }
}
namespace Emu.Bento
{
    partial class Form702D
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
            FarPoint.Win.Spread.EnhancedFocusIndicatorRenderer enhancedFocusIndicatorRenderer1 = new FarPoint.Win.Spread.EnhancedFocusIndicatorRenderer();
            FarPoint.Win.Spread.EnhancedScrollBarRenderer enhancedScrollBarRenderer1 = new FarPoint.Win.Spread.EnhancedScrollBarRenderer();
            FarPoint.Win.Spread.EnhancedScrollBarRenderer enhancedScrollBarRenderer2 = new FarPoint.Win.Spread.EnhancedScrollBarRenderer();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType2 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType3 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType4 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            this.cboBusyoGroup = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.syohinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSyohinName = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtSyohinCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtSyohinNameRyaku = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.cboMainDishCD = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.cboSubDishCD = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboBentoKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.txtTankaKomi = new Emu.Bento.Common.Control.TextBoxANC(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.chkInvalid = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.txtUnit = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label9 = new System.Windows.Forms.Label();
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
            this.cboSyokujiKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label15 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtSyohinKigo = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.lblCreateDate = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblCreateUserName = new System.Windows.Forms.Label();
            this.lblCreateUserCD = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.chkKeigen = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.txtTankaNuki = new Emu.Bento.Common.Control.TextBoxANC(this.components);
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.syohinBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // cboBusyoGroup
            // 
            this.cboBusyoGroup.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyoGroup.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyoGroup.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyoGroup.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.syohinBindingSource, "部署グループ", true));
            this.cboBusyoGroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyoGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyoGroup.FormattingEnabled = true;
            this.cboBusyoGroup.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyoGroup.Location = new System.Drawing.Point(130, 40);
            this.cboBusyoGroup.Name = "cboBusyoGroup";
            this.cboBusyoGroup.Size = new System.Drawing.Size(121, 23);
            this.cboBusyoGroup.TabIndex = 3;
            // 
            // syohinBindingSource
            // 
            this.syohinBindingSource.DataSource = typeof(Emu.Bento.Data.Syohin);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 43);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 15);
            this.label12.TabIndex = 2;
            this.label12.Text = "部署グループ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "商品名";
            // 
            // txtSyohinName
            // 
            this.txtSyohinName.CopyFromWhenEmpty = null;
            this.txtSyohinName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "商品名", true));
            this.txtSyohinName.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtSyohinName.Location = new System.Drawing.Point(130, 69);
            this.txtSyohinName.MaxLength = 30;
            this.txtSyohinName.Name = "txtSyohinName";
            this.txtSyohinName.Size = new System.Drawing.Size(190, 22);
            this.txtSyohinName.TabIndex = 5;
            // 
            // txtSyohinCD
            // 
            this.txtSyohinCD.CopyFromWhenEmpty = null;
            this.txtSyohinCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "商品ＣＤ", true));
            this.txtSyohinCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSyohinCD.Location = new System.Drawing.Point(130, 12);
            this.txtSyohinCD.MaxLength = 9;
            this.txtSyohinCD.Name = "txtSyohinCD";
            this.txtSyohinCD.Size = new System.Drawing.Size(83, 22);
            this.txtSyohinCD.TabIndex = 1;
            this.txtSyohinCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "商品CD";
            // 
            // txtSyohinNameRyaku
            // 
            this.txtSyohinNameRyaku.CopyFromWhenEmpty = null;
            this.txtSyohinNameRyaku.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "商品名略称", true));
            this.txtSyohinNameRyaku.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtSyohinNameRyaku.Location = new System.Drawing.Point(130, 97);
            this.txtSyohinNameRyaku.MaxLength = 10;
            this.txtSyohinNameRyaku.Name = "txtSyohinNameRyaku";
            this.txtSyohinNameRyaku.Size = new System.Drawing.Size(190, 22);
            this.txtSyohinNameRyaku.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "商品名略称";
            // 
            // cboMainDishCD
            // 
            this.cboMainDishCD.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboMainDishCD.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboMainDishCD.BackColor = System.Drawing.SystemColors.Control;
            this.cboMainDishCD.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.syohinBindingSource, "主食ＣＤ", true));
            this.cboMainDishCD.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboMainDishCD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMainDishCD.FormattingEnabled = true;
            this.cboMainDishCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboMainDishCD.Location = new System.Drawing.Point(130, 267);
            this.cboMainDishCD.Name = "cboMainDishCD";
            this.cboMainDishCD.Size = new System.Drawing.Size(172, 23);
            this.cboMainDishCD.TabIndex = 19;
            // 
            // cboSubDishCD
            // 
            this.cboSubDishCD.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSubDishCD.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSubDishCD.BackColor = System.Drawing.SystemColors.Control;
            this.cboSubDishCD.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.syohinBindingSource, "副食ＣＤ", true));
            this.cboSubDishCD.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSubDishCD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSubDishCD.FormattingEnabled = true;
            this.cboSubDishCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboSubDishCD.Location = new System.Drawing.Point(130, 238);
            this.cboSubDishCD.Name = "cboSubDishCD";
            this.cboSubDishCD.Size = new System.Drawing.Size(172, 23);
            this.cboSubDishCD.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 270);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 18;
            this.label8.Text = "主食";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 241);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 15);
            this.label7.TabIndex = 16;
            this.label7.Text = "副食";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 212);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "弁当区分";
            // 
            // cboBentoKbn
            // 
            this.cboBentoKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBentoKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBentoKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboBentoKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.syohinBindingSource, "弁当区分", true));
            this.cboBentoKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBentoKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBentoKbn.FormattingEnabled = true;
            this.cboBentoKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBentoKbn.Location = new System.Drawing.Point(130, 209);
            this.cboBentoKbn.Name = "cboBentoKbn";
            this.cboBentoKbn.Size = new System.Drawing.Size(121, 23);
            this.cboBentoKbn.TabIndex = 15;
            // 
            // txtTankaKomi
            // 
            this.txtTankaKomi.CopyFromWhenEmpty = null;
            this.txtTankaKomi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "商品単価税込", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtTankaKomi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTankaKomi.Location = new System.Drawing.Point(130, 153);
            this.txtTankaKomi.MaxLength = 10;
            this.txtTankaKomi.Name = "txtTankaKomi";
            this.txtTankaKomi.Size = new System.Drawing.Size(100, 22);
            this.txtTankaKomi.TabIndex = 11;
            this.txtTankaKomi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTankaKomi.Validated += new System.EventHandler(this.txtTankaKomi_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "商品単価";
            // 
            // chkInvalid
            // 
            this.chkInvalid.AutoSize = true;
            this.chkInvalid.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.syohinBindingSource, "無効フラグ", true));
            this.chkInvalid.Location = new System.Drawing.Point(15, 378);
            this.chkInvalid.Name = "chkInvalid";
            this.chkInvalid.Size = new System.Drawing.Size(56, 19);
            this.chkInvalid.TabIndex = 29;
            this.chkInvalid.Text = "無効";
            this.chkInvalid.UseVisualStyleBackColor = true;
            // 
            // txtUnit
            // 
            this.txtUnit.CopyFromWhenEmpty = null;
            this.txtUnit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "部数単位", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtUnit.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtUnit.Location = new System.Drawing.Point(130, 296);
            this.txtUnit.MaxLength = 9;
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(100, 22);
            this.txtUnit.TabIndex = 21;
            this.txtUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 299);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 15);
            this.label9.TabIndex = 20;
            this.label9.Text = "部数単位";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 471);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 35;
            this.label10.Text = "修正担当者";
            // 
            // lblUpdateUserCD
            // 
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(102, 467);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 36;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUpdateUserName
            // 
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(183, 467);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 37;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 499);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 38;
            this.label13.Text = "修正日時";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(102, 495);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 39;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMode
            // 
            this.lblMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblMode.ForeColor = System.Drawing.Color.Black;
            this.lblMode.Location = new System.Drawing.Point(12, 531);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(82, 21);
            this.lblMode.TabIndex = 40;
            this.lblMode.Text = "新規モード";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(520, 529);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 41;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(616, 529);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 25);
            this.btnDelete.TabIndex = 42;
            this.btnDelete.Text = "削除(F11)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(712, 529);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 43;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.syohinBindingSource;
            // 
            // cboSyokujiKbn
            // 
            this.cboSyokujiKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSyokujiKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSyokujiKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboSyokujiKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.syohinBindingSource, "食事区分", true));
            this.cboSyokujiKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSyokujiKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSyokujiKbn.FormattingEnabled = true;
            this.cboSyokujiKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboSyokujiKbn.Location = new System.Drawing.Point(130, 324);
            this.cboSyokujiKbn.Name = "cboSyokujiKbn";
            this.cboSyokujiKbn.Size = new System.Drawing.Size(121, 23);
            this.cboSyokujiKbn.TabIndex = 23;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 327);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(67, 15);
            this.label15.TabIndex = 22;
            this.label15.Text = "食事区分";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 128);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 15);
            this.label11.TabIndex = 8;
            this.label11.Text = "商品名記号";
            // 
            // txtSyohinKigo
            // 
            this.txtSyohinKigo.CopyFromWhenEmpty = null;
            this.txtSyohinKigo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "商品名記号", true));
            this.txtSyohinKigo.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtSyohinKigo.Location = new System.Drawing.Point(130, 125);
            this.txtSyohinKigo.MaxLength = 10;
            this.txtSyohinKigo.Name = "txtSyohinKigo";
            this.txtSyohinKigo.Size = new System.Drawing.Size(190, 22);
            this.txtSyohinKigo.TabIndex = 9;
            // 
            // lblCreateDate
            // 
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 439);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 34;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 443);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 33;
            this.label16.Text = "作成日時";
            // 
            // lblCreateUserName
            // 
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 411);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 32;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCreateUserCD
            // 
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 411);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 31;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 415);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 30;
            this.label19.Text = "作成担当者";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.fpSpread1);
            this.groupBox1.Location = new System.Drawing.Point(329, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(473, 505);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "この商品が属しているグループ";
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1";
            this.fpSpread1.AllowUserZoom = false;
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fpSpread1.CellSpanSelectionPolicy = FarPoint.Win.Spread.CellSpanSelectionPolicy.IgnoreSpan;
            this.fpSpread1.ColumnSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.FocusRenderer = enhancedFocusIndicatorRenderer1;
            this.fpSpread1.HorizontalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.HorizontalScrollBar.Name = "";
            this.fpSpread1.HorizontalScrollBar.Renderer = enhancedScrollBarRenderer1;
            this.fpSpread1.HorizontalScrollBarMode = FarPoint.Win.HorizontalScrollMode.PixelAndColumn;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1.Location = new System.Drawing.Point(6, 21);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(461, 478);
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.VerticalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.VerticalScrollBar.Name = "";
            this.fpSpread1.VerticalScrollBar.Renderer = enhancedScrollBarRenderer2;
            this.fpSpread1.VerticalScrollBarMode = FarPoint.Win.VerticalScrollMode.PixelAndRow;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 4;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.Parent = "ColumnFooterEnhanced";
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.Parent = "CornerEnhanced";
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "部署CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "部署名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "商品グループCD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "商品グループ名";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "部署CD";
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "部署名";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "商品グループCD";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 110F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "商品グループ名";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 150F;
            this.fpSpread1_Sheet1.DataAutoCellTypes = false;
            this.fpSpread1_Sheet1.DataAutoHeadings = false;
            this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread1_Sheet1.DataSource = this.bindingSource1;
            this.fpSpread1_Sheet1.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet1.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpSpread1_Sheet1.FilterBar.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.FilterBar.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.FilterBar.DefaultStyle.Parent = "FilterBarEnhanced";
            this.fpSpread1_Sheet1.FilterBar.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.FilterBarHeaderStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.FilterBarHeaderStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.FilterBarHeaderStyle.Parent = "RowHeaderEnhanced";
            this.fpSpread1_Sheet1.FilterBarHeaderStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpSpread1_Sheet1.Protect = false;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "RowHeaderEnhanced";
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.RowHeader.Visible = false;
            this.fpSpread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpSpread1_Sheet1.SheetCornerStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.SheetCornerStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.SheetCornerStyle.Parent = "CornerEnhanced";
            this.fpSpread1_Sheet1.SheetCornerStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // chkKeigen
            // 
            this.chkKeigen.AutoSize = true;
            this.chkKeigen.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.syohinBindingSource, "軽減税率フラグ", true));
            this.chkKeigen.Location = new System.Drawing.Point(15, 353);
            this.chkKeigen.Name = "chkKeigen";
            this.chkKeigen.Size = new System.Drawing.Size(116, 19);
            this.chkKeigen.TabIndex = 28;
            this.chkKeigen.Text = "軽減税率対象";
            this.chkKeigen.UseVisualStyleBackColor = true;
            this.chkKeigen.Click += new System.EventHandler(this.chkKeigen_Click);
            // 
            // txtTankaNuki
            // 
            this.txtTankaNuki.CopyFromWhenEmpty = null;
            this.txtTankaNuki.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.syohinBindingSource, "商品単価税抜", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtTankaNuki.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTankaNuki.Location = new System.Drawing.Point(130, 181);
            this.txtTankaNuki.MaxLength = 10;
            this.txtTankaNuki.Name = "txtTankaNuki";
            this.txtTankaNuki.Size = new System.Drawing.Size(100, 22);
            this.txtTankaNuki.TabIndex = 13;
            this.txtTankaNuki.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 184);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "（税抜単価）";
            // 
            // Form702D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 566);
            this.Controls.Add(this.txtTankaNuki);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkKeigen);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblCreateDate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCreateUserName);
            this.Controls.Add(this.lblCreateUserCD);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtSyohinKigo);
            this.Controls.Add(this.cboSyokujiKbn);
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
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.chkInvalid);
            this.Controls.Add(this.cboMainDishCD);
            this.Controls.Add(this.cboSubDishCD);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cboBentoKbn);
            this.Controls.Add(this.txtTankaKomi);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSyohinNameRyaku);
            this.Controls.Add(this.cboBusyoGroup);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSyohinName);
            this.Controls.Add(this.txtSyohinCD);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form702D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "商品マスタ詳細";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form702D_FormClosing);
            this.Load += new System.EventHandler(this.Form702D_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form702D_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.syohinBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Common.Control.ComboBoxA cboBusyoGroup;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtSyohinName;
        private Common.Control.TextBoxAN txtSyohinCD;
        private System.Windows.Forms.Label label1;
        private Common.Control.TextBoxA txtSyohinNameRyaku;
        private System.Windows.Forms.Label label4;
        private Common.Control.ComboBoxA cboMainDishCD;
        private Common.Control.ComboBoxA cboSubDishCD;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private Common.Control.ComboBoxA cboBentoKbn;
        private Common.Control.TextBoxANC txtTankaKomi;
        private System.Windows.Forms.Label label5;
        private Common.Control.CheckBoxA chkInvalid;
        private Common.Control.TextBoxAN txtUnit;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUpdateUserCD;
        private System.Windows.Forms.Label lblUpdateUserName;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource syohinBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Common.Control.ComboBoxA cboSyokujiKbn;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lblCreateDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCreateUserName;
        private System.Windows.Forms.Label lblCreateUserCD;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label11;
        private Common.Control.TextBoxA txtSyohinKigo;
        private System.Windows.Forms.GroupBox groupBox1;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private Common.Control.CheckBoxA chkKeigen;
        private Common.Control.TextBoxANC txtTankaNuki;
        private System.Windows.Forms.Label label3;
    }
}

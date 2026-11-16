namespace Emu.Bento
{
    partial class Form513
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
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType1 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType3 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType4 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType5 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType6 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType1 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSeikyubiTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.frSettlementListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dtpSeikyubiFrom = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSeikyusakiCDTo = new System.Windows.Forms.Button();
            this.lblSeikyusakiNameTo = new System.Windows.Forms.Label();
            this.txtSeikyusakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.btnCourseCD = new System.Windows.Forms.Button();
            this.lblCourseName = new System.Windows.Forms.Label();
            this.txtCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboShiten = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.cboSettlementStatus = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.btnSeikyusakiCDFrom = new System.Windows.Forms.Button();
            this.lblSeikyusakiNameFrom = new System.Windows.Forms.Label();
            this.txtSeikyusakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.btnQuery = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cboListMax = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnExec = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDetail = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frSettlementListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dtpSeikyubiTo);
            this.groupBox1.Controls.Add(this.dtpSeikyubiFrom);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnSeikyusakiCDTo);
            this.groupBox1.Controls.Add(this.lblSeikyusakiNameTo);
            this.groupBox1.Controls.Add(this.txtSeikyusakiCDTo);
            this.groupBox1.Controls.Add(this.btnCourseCD);
            this.groupBox1.Controls.Add(this.lblCourseName);
            this.groupBox1.Controls.Add(this.txtCourseCD);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboShiten);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cboSettlementStatus);
            this.groupBox1.Controls.Add(this.btnSeikyusakiCDFrom);
            this.groupBox1.Controls.Add(this.lblSeikyusakiNameFrom);
            this.groupBox1.Controls.Add(this.txtSeikyusakiCDFrom);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cboListMax);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(797, 171);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // dtpSeikyubiTo
            // 
            this.dtpSeikyubiTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "請求日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubiTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubiTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubiTo.Location = new System.Drawing.Point(253, 80);
            this.dtpSeikyubiTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubiTo.Name = "dtpSeikyubiTo";
            this.dtpSeikyubiTo.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubiTo.TabIndex = 7;
            // 
            // frSettlementListBindingSource
            // 
            this.frSettlementListBindingSource.AllowNew = false;
            this.frSettlementListBindingSource.DataSource = typeof(Emu.Bento.Data.FRSettlementListCriteria);
            // 
            // dtpSeikyubiFrom
            // 
            this.dtpSeikyubiFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "請求日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubiFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubiFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubiFrom.Location = new System.Drawing.Point(65, 80);
            this.dtpSeikyubiFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubiFrom.Name = "dtpSeikyubiFrom";
            this.dtpSeikyubiFrom.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubiFrom.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(224, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "～";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "請求日";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(376, 141);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 16;
            this.label12.Text = "～";
            // 
            // btnSeikyusakiCDTo
            // 
            this.btnSeikyusakiCDTo.AutoSize = true;
            this.btnSeikyusakiCDTo.Location = new System.Drawing.Point(662, 136);
            this.btnSeikyusakiCDTo.Name = "btnSeikyusakiCDTo";
            this.btnSeikyusakiCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnSeikyusakiCDTo.TabIndex = 19;
            this.btnSeikyusakiCDTo.TabStop = false;
            this.btnSeikyusakiCDTo.Text = "参照";
            this.btnSeikyusakiCDTo.UseVisualStyleBackColor = true;
            this.btnSeikyusakiCDTo.Click += new System.EventHandler(this.btnSeikyusakiCDTo_Click);
            // 
            // lblSeikyusakiNameTo
            // 
            this.lblSeikyusakiNameTo.AutoEllipsis = true;
            this.lblSeikyusakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "請求先名To", true));
            this.lblSeikyusakiNameTo.Location = new System.Drawing.Point(493, 137);
            this.lblSeikyusakiNameTo.Name = "lblSeikyusakiNameTo";
            this.lblSeikyusakiNameTo.Size = new System.Drawing.Size(163, 22);
            this.lblSeikyusakiNameTo.TabIndex = 18;
            this.lblSeikyusakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSeikyusakiCDTo
            // 
            this.txtSeikyusakiCDTo.CopyFromWhenEmpty = null;
            this.txtSeikyusakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "請求先ＣＤTo", true));
            this.txtSeikyusakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSeikyusakiCDTo.Location = new System.Drawing.Point(404, 137);
            this.txtSeikyusakiCDTo.MaxLength = 9;
            this.txtSeikyusakiCDTo.Name = "txtSeikyusakiCDTo";
            this.txtSeikyusakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtSeikyusakiCDTo.TabIndex = 17;
            this.txtSeikyusakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSeikyusakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSeikyusakiCDTo_KeyDown);
            // 
            // btnCourseCD
            // 
            this.btnCourseCD.AutoSize = true;
            this.btnCourseCD.Location = new System.Drawing.Point(323, 108);
            this.btnCourseCD.Name = "btnCourseCD";
            this.btnCourseCD.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCD.TabIndex = 11;
            this.btnCourseCD.TabStop = false;
            this.btnCourseCD.Text = "参照";
            this.btnCourseCD.UseVisualStyleBackColor = true;
            this.btnCourseCD.Click += new System.EventHandler(this.btnCourseCD_Click);
            // 
            // lblCourseName
            // 
            this.lblCourseName.AutoEllipsis = true;
            this.lblCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "コース名", true));
            this.lblCourseName.Location = new System.Drawing.Point(154, 109);
            this.lblCourseName.Name = "lblCourseName";
            this.lblCourseName.Size = new System.Drawing.Size(163, 22);
            this.lblCourseName.TabIndex = 10;
            this.lblCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCD
            // 
            this.txtCourseCD.CopyFromWhenEmpty = null;
            this.txtCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "コースＣＤ", true));
            this.txtCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCD.Location = new System.Drawing.Point(65, 109);
            this.txtCourseCD.MaxLength = 9;
            this.txtCourseCD.Name = "txtCourseCD";
            this.txtCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCD.TabIndex = 9;
            this.txtCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCD_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "コース";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "支店";
            // 
            // cboShiten
            // 
            this.cboShiten.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboShiten.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboShiten.BackColor = System.Drawing.SystemColors.Control;
            this.cboShiten.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.frSettlementListBindingSource, "支店ＣＤ", true));
            this.cboShiten.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboShiten.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShiten.FormattingEnabled = true;
            this.cboShiten.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboShiten.Location = new System.Drawing.Point(65, 20);
            this.cboShiten.Name = "cboShiten";
            this.cboShiten.Size = new System.Drawing.Size(161, 23);
            this.cboShiten.TabIndex = 1;
            this.cboShiten.SelectedValueChanged += new System.EventHandler(this.cboShiten_SelectedValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(420, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 15);
            this.label7.TabIndex = 20;
            this.label7.Text = "決済状態";
            // 
            // cboSettlementStatus
            // 
            this.cboSettlementStatus.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSettlementStatus.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSettlementStatus.BackColor = System.Drawing.SystemColors.Control;
            this.cboSettlementStatus.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.frSettlementListBindingSource, "決済状態", true));
            this.cboSettlementStatus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSettlementStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSettlementStatus.FormattingEnabled = true;
            this.cboSettlementStatus.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboSettlementStatus.Location = new System.Drawing.Point(508, 20);
            this.cboSettlementStatus.Name = "cboSettlementStatus";
            this.cboSettlementStatus.Size = new System.Drawing.Size(90, 23);
            this.cboSettlementStatus.TabIndex = 21;
            // 
            // btnSeikyusakiCDFrom
            // 
            this.btnSeikyusakiCDFrom.AutoSize = true;
            this.btnSeikyusakiCDFrom.Location = new System.Drawing.Point(323, 136);
            this.btnSeikyusakiCDFrom.Name = "btnSeikyusakiCDFrom";
            this.btnSeikyusakiCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnSeikyusakiCDFrom.TabIndex = 15;
            this.btnSeikyusakiCDFrom.TabStop = false;
            this.btnSeikyusakiCDFrom.Text = "参照";
            this.btnSeikyusakiCDFrom.UseVisualStyleBackColor = true;
            this.btnSeikyusakiCDFrom.Click += new System.EventHandler(this.btnSeikyusakiCDFrom_Click);
            // 
            // lblSeikyusakiNameFrom
            // 
            this.lblSeikyusakiNameFrom.AutoEllipsis = true;
            this.lblSeikyusakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "請求先名From", true));
            this.lblSeikyusakiNameFrom.Location = new System.Drawing.Point(154, 137);
            this.lblSeikyusakiNameFrom.Name = "lblSeikyusakiNameFrom";
            this.lblSeikyusakiNameFrom.Size = new System.Drawing.Size(163, 22);
            this.lblSeikyusakiNameFrom.TabIndex = 14;
            this.lblSeikyusakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSeikyusakiCDFrom
            // 
            this.txtSeikyusakiCDFrom.CopyFromWhenEmpty = null;
            this.txtSeikyusakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.frSettlementListBindingSource, "請求先ＣＤFrom", true));
            this.txtSeikyusakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSeikyusakiCDFrom.Location = new System.Drawing.Point(65, 137);
            this.txtSeikyusakiCDFrom.MaxLength = 9;
            this.txtSeikyusakiCDFrom.Name = "txtSeikyusakiCDFrom";
            this.txtSeikyusakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtSeikyusakiCDFrom.TabIndex = 13;
            this.txtSeikyusakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSeikyusakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSeikyusakiCDFrom_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "請求先";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.frSettlementListBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(65, 50);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 3;
            this.cboBusyo.SelectedValueChanged += new System.EventHandler(this.cboBusyo_SelectedValueChanged);
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(718, 136);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 29;
            this.btnQuery.Text = "検索(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(602, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 15);
            this.label11.TabIndex = 28;
            this.label11.Text = "件";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(422, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 26;
            this.label9.Text = "表示数";
            // 
            // cboListMax
            // 
            this.cboListMax.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboListMax.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboListMax.BackColor = System.Drawing.SystemColors.Control;
            this.cboListMax.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboListMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboListMax.FormattingEnabled = true;
            this.cboListMax.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboListMax.Location = new System.Drawing.Point(508, 80);
            this.cboListMax.Name = "cboListMax";
            this.cboListMax.Size = new System.Drawing.Size(88, 23);
            this.cboListMax.TabIndex = 27;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Location = new System.Drawing.Point(696, 52);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 25;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Checked = true;
            this.rdoAsc.Location = new System.Drawing.Point(635, 52);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 24;
            this.rdoAsc.TabStop = true;
            this.rdoAsc.Text = "昇順";
            this.rdoAsc.UseVisualStyleBackColor = true;
            // 
            // cboSortKey
            // 
            this.cboSortKey.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSortKey.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSortKey.BackColor = System.Drawing.SystemColors.Control;
            this.cboSortKey.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSortKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSortKey.FormattingEnabled = true;
            this.cboSortKey.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboSortKey.Location = new System.Drawing.Point(508, 50);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(121, 23);
            this.cboSortKey.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(422, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "ソート";
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 189);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(797, 270);
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            this.fpSpread1.TabIndex = 1;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.VerticalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.VerticalScrollBar.Name = "";
            this.fpSpread1.VerticalScrollBar.Renderer = enhancedScrollBarRenderer2;
            this.fpSpread1.VerticalScrollBarMode = FarPoint.Win.VerticalScrollMode.PixelAndRow;
            this.fpSpread1.LeaveCell += new FarPoint.Win.Spread.LeaveCellEventHandler(this.fpSpread1_LeaveCell);
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            this.fpSpread1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fpSpread1_KeyPress);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 11;
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
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "決済状態";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "請求番号";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "請求日";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "コースCD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "コース名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "請求先CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "請求先名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "今回請求額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "クレカ決済完了フラグ";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "クレカ決済エラー区分";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 10).Value = "クレカ無効フラグ";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "決済状態";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "請求番号";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 70F;
            generalCellType1.EnableSubEditor = false;
            generalCellType1.FormatString = "yyyy/MM/dd（ddd）";
            generalCellType1.IsDateFormat = true;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "請求日";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "コースCD";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = emptyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(4).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "コース名";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = emptyCellType5;
            this.fpSpread1_Sheet1.Columns.Get(5).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "請求先CD";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 75F;
            this.fpSpread1_Sheet1.Columns.Get(6).CellType = emptyCellType6;
            this.fpSpread1_Sheet1.Columns.Get(6).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "請求先名";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 150F;
            currencyCellType1.DecimalPlaces = 0;
            currencyCellType1.EnableSubEditor = false;
            currencyCellType1.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType1.MinimumValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            currencyCellType1.Separator = ",";
            currencyCellType1.ShowCurrencySymbol = false;
            currencyCellType1.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(7).CellType = currencyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "今回請求額";
            this.fpSpread1_Sheet1.Columns.Get(7).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(8).Label = "クレカ決済完了フラグ";
            this.fpSpread1_Sheet1.Columns.Get(8).Width = 154F;
            this.fpSpread1_Sheet1.Columns.Get(9).Label = "クレカ決済エラー区分";
            this.fpSpread1_Sheet1.Columns.Get(9).Width = 151F;
            this.fpSpread1_Sheet1.Columns.Get(10).Label = "クレカ無効フラグ";
            this.fpSpread1_Sheet1.Columns.Get(10).Width = 105F;
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
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Width = 38F;
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
            // bindingSource1
            // 
            this.bindingSource1.AllowNew = false;
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.Location = new System.Drawing.Point(107, 470);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(80, 15);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "lblCount";
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoSize = true;
            this.btnExport.CausesValidation = false;
            this.btnExport.Location = new System.Drawing.Point(597, 465);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(103, 25);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Excel出力";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(706, 465);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 7;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.frSettlementListBindingSource;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.Filter = "Excel ブック(*.xlsx)|*.xlsx|Excel 97-2003 ブック(*.xls)|*.xls";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.CausesValidation = false;
            this.btnExec.Enabled = false;
            this.btnExec.Location = new System.Drawing.Point(370, 465);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(108, 25);
            this.btnExec.TabIndex = 4;
            this.btnExec.Text = "決済処理";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(12, 465);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(89, 25);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "クリア";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDetail
            // 
            this.btnDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetail.AutoSize = true;
            this.btnDetail.CausesValidation = false;
            this.btnDetail.Location = new System.Drawing.Point(484, 465);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(107, 25);
            this.btnDetail.TabIndex = 5;
            this.btnDetail.Text = "詳細画面(F8)";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // Form513
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 502);
            this.Controls.Add(this.btnDetail);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form513";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "クレカ決済一覧";
            this.Load += new System.EventHandler(this.Form513_Load);
            this.Shown += new System.EventHandler(this.Form513_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form513_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.frSettlementListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private Common.Control.ComboBoxA cboListMax;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private Common.Control.ComboBoxA cboSortKey;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnQuery;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource frSettlementListBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label2;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Button btnSeikyusakiCDFrom;
        private System.Windows.Forms.Label lblSeikyusakiNameFrom;
        private Common.Control.TextBoxAN txtSeikyusakiCDFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private Common.Control.ComboBoxA cboSettlementStatus;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboShiten;
        private System.Windows.Forms.Button btnCourseCD;
        private System.Windows.Forms.Label lblCourseName;
        private Common.Control.TextBoxAN txtCourseCD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnSeikyusakiCDTo;
        private System.Windows.Forms.Label lblSeikyusakiNameTo;
        private Common.Control.TextBoxAN txtSeikyusakiCDTo;
        private Common.Control.DateTimePickerA dtpSeikyubiTo;
        private Common.Control.DateTimePickerA dtpSeikyubiFrom;
    }
}

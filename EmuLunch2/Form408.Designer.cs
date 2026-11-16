namespace Emu.Bento
{
    partial class Form408
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
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType2 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType3 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType2 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType3 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType4 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType5 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType6 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType7 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType8 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType9 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType10 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType11 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType1 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType2 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType3 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType4 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType5 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType6 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMonth = new System.Windows.Forms.Label();
            this.txtMonth = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.hanbaiJissekiListCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblYear = new System.Windows.Forms.Label();
            this.txtYear = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.rdoMonthly = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoDaily = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.dtpSeisanbi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.btnExport = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hanbaiJissekiListCriteriaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            //
            // btnQuit
            //
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(230, 154);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            //
            // btnPrint
            //
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(12, 154);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 25);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "印刷(F10)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lblMonth);
            this.groupBox1.Controls.Add(this.txtMonth);
            this.groupBox1.Controls.Add(this.lblYear);
            this.groupBox1.Controls.Add(this.txtYear);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.rdoMonthly);
            this.groupBox1.Controls.Add(this.rdoDaily);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpSeisanbi);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(53, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 136);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象指定";
            //
            // lblMonth
            //
            this.lblMonth.AutoSize = true;
            this.lblMonth.Location = new System.Drawing.Point(174, 108);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(22, 15);
            this.lblMonth.TabIndex = 11;
            this.lblMonth.Text = "月";
            //
            // txtMonth
            //
            this.txtMonth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.hanbaiJissekiListCriteriaBindingSource, "対象月", true));
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtMonth.Location = new System.Drawing.Point(141, 105);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(27, 22);
            this.txtMonth.TabIndex = 10;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // hanbaiJissekiListCriteriaBindingSource
            //
            this.hanbaiJissekiListCriteriaBindingSource.AllowNew = false;
            this.hanbaiJissekiListCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.HanbaiJissekiListCriteria);
            //
            // lblYear
            //
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(113, 108);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(22, 15);
            this.lblYear.TabIndex = 9;
            this.lblYear.Text = "年";
            //
            // txtYear
            //
            this.txtYear.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.hanbaiJissekiListCriteriaBindingSource, "対象年", true));
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtYear.Location = new System.Drawing.Point(64, 105);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(41, 22);
            this.txtYear.TabIndex = 8;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // label9
            //
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 7;
            this.label9.Text = "年月";
            //
            // rdoMonthly
            //
            this.rdoMonthly.AutoSize = true;
            this.rdoMonthly.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.hanbaiJissekiListCriteriaBindingSource, "月計", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rdoMonthly.Location = new System.Drawing.Point(125, 50);
            this.rdoMonthly.Name = "rdoMonthly";
            this.rdoMonthly.Size = new System.Drawing.Size(55, 19);
            this.rdoMonthly.TabIndex = 4;
            this.rdoMonthly.Text = "月計";
            this.rdoMonthly.UseVisualStyleBackColor = true;
            this.rdoMonthly.CheckedChanged += new System.EventHandler(this.rdoRange_CheckedChanged);
            //
            // rdoDaily
            //
            this.rdoDaily.AutoSize = true;
            this.rdoDaily.Checked = true;
            this.rdoDaily.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.hanbaiJissekiListCriteriaBindingSource, "日計", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rdoDaily.Location = new System.Drawing.Point(64, 50);
            this.rdoDaily.Name = "rdoDaily";
            this.rdoDaily.Size = new System.Drawing.Size(55, 19);
            this.rdoDaily.TabIndex = 3;
            this.rdoDaily.TabStop = true;
            this.rdoDaily.Text = "日計";
            this.rdoDaily.UseVisualStyleBackColor = true;
            this.rdoDaily.CheckedChanged += new System.EventHandler(this.rdoRange_CheckedChanged);
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "範囲";
            //
            // dtpSeisanbi
            //
            this.dtpSeisanbi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.hanbaiJissekiListCriteriaBindingSource, "精算日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeisanbi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeisanbi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeisanbi.Location = new System.Drawing.Point(64, 76);
            this.dtpSeisanbi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeisanbi.Name = "dtpSeisanbi";
            this.dtpSeisanbi.Size = new System.Drawing.Size(152, 22);
            this.dtpSeisanbi.TabIndex = 6;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "部署";
            //
            // cboBusyo
            //
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.hanbaiJissekiListCriteriaBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(64, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "精算日";
            //
            // errorProvider1
            //
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.hanbaiJissekiListCriteriaBindingSource;
            //
            // fpSpread1
            //
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1";
            this.fpSpread1.AllowUserZoom = false;
            this.fpSpread1.CellSpanSelectionPolicy = FarPoint.Win.Spread.CellSpanSelectionPolicy.IgnoreSpan;
            this.fpSpread1.ColumnSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.HorizontalScrollBarMode = FarPoint.Win.HorizontalScrollMode.PixelAndColumn;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.Location = new System.Drawing.Point(362, 12);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(268, 167);
            this.fpSpread1.TabIndex = 5;
            this.fpSpread1.Visible = false;
            //
            // fpSpread1_Sheet1
            //
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 20;
            this.fpSpread1_Sheet1.ColumnHeader.RowCount = 2;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).ColumnSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "持出数";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).ColumnSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "貰い";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).ColumnSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "渡し";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).ColumnSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "販売数";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 11).ColumnSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 11).Value = "残数";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 13).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 13).Value = "食数";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 14).ColumnSpan = 4;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 14).Value = "売上実績";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 15).Value = "⑤";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 17).Value = "①+②+⑤ A";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 18).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 18).Value = "実質売上";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 19).Value = "平均単価";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 0).Value = "コースCD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 1).Value = "コース名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 2).Value = "配送者";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 3).Value = "副食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 4).Value = "主食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 5).Value = "副食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 6).Value = "主食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 7).Value = "副食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 8).Value = "主食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 9).Value = "副食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 10).Value = "主食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 11).Value = "副食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 12).Value = "主食";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 14).Value = "現金";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 15).Value = "掛売";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 16).Value = "電子決済";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 17).Value = "その他";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 19).Value = "(実質売上/食数)";
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "コースCD";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "コース名";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "配送者";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 130F;
            numberCellType1.DecimalPlaces = 0;
            numberCellType1.EnableSubEditor = false;
            numberCellType1.MaximumValue = 999999D;
            numberCellType1.MinimumValue = 0D;
            numberCellType1.Separator = ",";
            numberCellType1.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = numberCellType1;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "副食";
            numberCellType2.DecimalPlaces = 0;
            numberCellType2.EnableSubEditor = false;
            numberCellType2.MaximumValue = 999999D;
            numberCellType2.MinimumValue = 0D;
            numberCellType2.Separator = ",";
            numberCellType2.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = numberCellType2;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "主食";
            numberCellType3.DecimalPlaces = 0;
            numberCellType3.EnableSubEditor = false;
            numberCellType3.MaximumValue = 999999D;
            numberCellType3.MinimumValue = 0D;
            numberCellType3.Separator = ",";
            numberCellType3.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = numberCellType3;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "副食";
            numberCellType4.DecimalPlaces = 0;
            numberCellType4.EnableSubEditor = false;
            numberCellType4.MaximumValue = 999999D;
            numberCellType4.MinimumValue = 0D;
            numberCellType4.Separator = ",";
            numberCellType4.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(6).CellType = numberCellType4;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "主食";
            numberCellType5.DecimalPlaces = 0;
            numberCellType5.EnableSubEditor = false;
            numberCellType5.MaximumValue = 999999D;
            numberCellType5.MinimumValue = 0D;
            numberCellType5.Separator = ",";
            numberCellType5.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(7).CellType = numberCellType5;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "副食";
            numberCellType6.DecimalPlaces = 0;
            numberCellType6.EnableSubEditor = false;
            numberCellType6.MaximumValue = 999999D;
            numberCellType6.MinimumValue = 0D;
            numberCellType6.Separator = ",";
            numberCellType6.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(8).CellType = numberCellType6;
            this.fpSpread1_Sheet1.Columns.Get(8).Label = "主食";
            numberCellType7.DecimalPlaces = 0;
            numberCellType7.EnableSubEditor = false;
            numberCellType7.MaximumValue = 999999D;
            numberCellType7.MinimumValue = 0D;
            numberCellType7.Separator = ",";
            numberCellType7.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(9).CellType = numberCellType7;
            this.fpSpread1_Sheet1.Columns.Get(9).Label = "副食";
            numberCellType8.DecimalPlaces = 0;
            numberCellType8.EnableSubEditor = false;
            numberCellType8.MaximumValue = 999999D;
            numberCellType8.MinimumValue = 0D;
            numberCellType8.Separator = ",";
            numberCellType8.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(10).CellType = numberCellType8;
            this.fpSpread1_Sheet1.Columns.Get(10).Label = "主食";
            numberCellType9.DecimalPlaces = 0;
            numberCellType9.EnableSubEditor = false;
            numberCellType9.MaximumValue = 999999D;
            numberCellType9.MinimumValue = 0D;
            numberCellType9.Separator = ",";
            numberCellType9.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(11).CellType = numberCellType9;
            this.fpSpread1_Sheet1.Columns.Get(11).Label = "副食";
            numberCellType10.DecimalPlaces = 0;
            numberCellType10.EnableSubEditor = false;
            numberCellType10.MaximumValue = 999999D;
            numberCellType10.MinimumValue = 0D;
            numberCellType10.Separator = ",";
            numberCellType10.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(12).CellType = numberCellType10;
            this.fpSpread1_Sheet1.Columns.Get(12).Label = "主食";
            numberCellType11.DecimalPlaces = 0;
            numberCellType11.EnableSubEditor = false;
            numberCellType11.MaximumValue = 999999D;
            numberCellType11.MinimumValue = 0D;
            numberCellType11.Separator = ",";
            numberCellType11.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(13).CellType = numberCellType11;
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
            this.fpSpread1_Sheet1.Columns.Get(14).CellType = currencyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(14).Label = "現金";
            this.fpSpread1_Sheet1.Columns.Get(14).Width = 90F;
            currencyCellType2.DecimalPlaces = 0;
            currencyCellType2.EnableSubEditor = false;
            currencyCellType2.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType2.MinimumValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            currencyCellType2.Separator = ",";
            currencyCellType2.ShowCurrencySymbol = false;
            currencyCellType2.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(15).CellType = currencyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(15).Label = "掛売";
            this.fpSpread1_Sheet1.Columns.Get(15).Width = 90F;
            currencyCellType3.DecimalPlaces = 0;
            currencyCellType3.EnableSubEditor = false;
            currencyCellType3.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType3.MinimumValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            currencyCellType3.Separator = ",";
            currencyCellType3.ShowCurrencySymbol = false;
            currencyCellType3.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(16).CellType = currencyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(16).Label = "電子決済";
            this.fpSpread1_Sheet1.Columns.Get(16).Width = 90F;
            currencyCellType4.DecimalPlaces = 0;
            currencyCellType4.EnableSubEditor = false;
            currencyCellType4.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType4.MinimumValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            currencyCellType4.Separator = ",";
            currencyCellType4.ShowCurrencySymbol = false;
            currencyCellType4.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(17).CellType = currencyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(17).Label = "その他";
            this.fpSpread1_Sheet1.Columns.Get(17).Width = 90F;
            currencyCellType5.DecimalPlaces = 0;
            currencyCellType5.EnableSubEditor = false;
            currencyCellType5.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType5.MinimumValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            currencyCellType5.Separator = ",";
            currencyCellType5.ShowCurrencySymbol = false;
            currencyCellType5.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(18).CellType = currencyCellType5;
            this.fpSpread1_Sheet1.Columns.Get(18).Width = 120F;
            currencyCellType6.DecimalPlaces = 0;
            currencyCellType6.EnableSubEditor = false;
            currencyCellType6.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType6.MinimumValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            currencyCellType6.Separator = ",";
            currencyCellType6.ShowCurrencySymbol = false;
            currencyCellType6.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(19).CellType = currencyCellType6;
            this.fpSpread1_Sheet1.Columns.Get(19).Label = "(実質売上/食数)";
            this.fpSpread1_Sheet1.Columns.Get(19).Width = 120F;
            this.fpSpread1_Sheet1.DataAutoCellTypes = false;
            this.fpSpread1_Sheet1.DataAutoHeadings = false;
            this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread1_Sheet1.DataSource = this.bindingSource1;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet1.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpSpread1_Sheet1.Protect = false;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.RowHeader.Visible = false;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            //
            // btnExport
            //
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.AutoSize = true;
            this.btnExport.Location = new System.Drawing.Point(121, 154);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(103, 25);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Excel出力";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // label4
            //
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(297, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 135);
            this.label4.TabIndex = 4;
            this.label4.Text = "→ここにExcel出力用のSpreadが隠れている";
            this.label4.Visible = false;
            //
            // saveFileDialog1
            //
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.Filter = "Excel ブック(*.xlsx)|*.xlsx|Excel 97-2003 ブック(*.xls)|*.xls";
            this.saveFileDialog1.Title = "名前を付けて保存";
            //
            // Form408
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 191);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form408";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "販売実績一覧表印刷";
            this.Load += new System.EventHandler(this.Form408_Load);
            this.Shown += new System.EventHandler(this.Form408_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form408_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            ((System.ComponentModel.ISupportInitialize)(this.hanbaiJissekiListCriteriaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource hanbaiJissekiListCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpSeisanbi;
        private Common.Control.RadioButtonA rdoMonthly;
        private Common.Control.RadioButtonA rdoDaily;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblMonth;
        private Common.Control.TextBoxAN txtMonth;
        private System.Windows.Forms.Label lblYear;
        private Common.Control.TextBoxAN txtYear;
        private System.Windows.Forms.Label label9;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

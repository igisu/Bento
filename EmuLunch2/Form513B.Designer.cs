namespace Emu.Bento
{
    partial class Form513B
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
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType1 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSeikyusakiCDTo = new System.Windows.Forms.Label();
            this.seikyuListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblSeikyusakiNameTo = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblSeikyusakiCDFrom = new System.Windows.Forms.Label();
            this.lblCourceCD = new System.Windows.Forms.Label();
            this.lblDeptName = new System.Windows.Forms.Label();
            this.lblSeikyuDateTo = new System.Windows.Forms.Label();
            this.lblSeikyuDateFrom = new System.Windows.Forms.Label();
            this.lblBranchName = new System.Windows.Forms.Label();
            this.lblCourseName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSeikyusakiNameFrom = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnExec = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnQuit = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblSeikyusakiCDTo);
            this.groupBox1.Controls.Add(this.lblSeikyusakiNameTo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblSeikyusakiCDFrom);
            this.groupBox1.Controls.Add(this.lblCourceCD);
            this.groupBox1.Controls.Add(this.lblDeptName);
            this.groupBox1.Controls.Add(this.lblSeikyuDateTo);
            this.groupBox1.Controls.Add(this.lblSeikyuDateFrom);
            this.groupBox1.Controls.Add(this.lblBranchName);
            this.groupBox1.Controls.Add(this.lblCourseName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblSeikyusakiNameFrom);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(611, 110);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "指定条件";
            // 
            // lblSeikyusakiCDTo
            // 
            this.lblSeikyusakiCDTo.AutoEllipsis = true;
            this.lblSeikyusakiCDTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "請求先ＣＤTo", true));
            this.lblSeikyusakiCDTo.Location = new System.Drawing.Point(317, 80);
            this.lblSeikyusakiCDTo.Name = "lblSeikyusakiCDTo";
            this.lblSeikyusakiCDTo.Size = new System.Drawing.Size(62, 22);
            this.lblSeikyusakiCDTo.TabIndex = 15;
            this.lblSeikyusakiCDTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // seikyuListBindingSource
            // 
            this.seikyuListBindingSource.AllowNew = false;
            this.seikyuListBindingSource.DataSource = typeof(Emu.Bento.Data.FRSettlementListCriteria);
            // 
            // lblSeikyusakiNameTo
            // 
            this.lblSeikyusakiNameTo.AutoEllipsis = true;
            this.lblSeikyusakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "請求先名To", true));
            this.lblSeikyusakiNameTo.Location = new System.Drawing.Point(385, 80);
            this.lblSeikyusakiNameTo.Name = "lblSeikyusakiNameTo";
            this.lblSeikyusakiNameTo.Size = new System.Drawing.Size(151, 22);
            this.lblSeikyusakiNameTo.TabIndex = 16;
            this.lblSeikyusakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(289, 84);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "～";
            // 
            // lblSeikyusakiCDFrom
            // 
            this.lblSeikyusakiCDFrom.AutoEllipsis = true;
            this.lblSeikyusakiCDFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "請求先ＣＤFrom", true));
            this.lblSeikyusakiCDFrom.Location = new System.Drawing.Point(64, 80);
            this.lblSeikyusakiCDFrom.Name = "lblSeikyusakiCDFrom";
            this.lblSeikyusakiCDFrom.Size = new System.Drawing.Size(62, 22);
            this.lblSeikyusakiCDFrom.TabIndex = 12;
            this.lblSeikyusakiCDFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCourceCD
            // 
            this.lblCourceCD.AutoEllipsis = true;
            this.lblCourceCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourceCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "コースＣＤ", true));
            this.lblCourceCD.Location = new System.Drawing.Point(260, 20);
            this.lblCourceCD.Name = "lblCourceCD";
            this.lblCourceCD.Size = new System.Drawing.Size(62, 22);
            this.lblCourceCD.TabIndex = 5;
            this.lblCourceCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDeptName
            // 
            this.lblDeptName.AutoEllipsis = true;
            this.lblDeptName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDeptName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "部署名L", true));
            this.lblDeptName.Location = new System.Drawing.Point(64, 50);
            this.lblDeptName.Name = "lblDeptName";
            this.lblDeptName.Size = new System.Drawing.Size(132, 22);
            this.lblDeptName.TabIndex = 3;
            this.lblDeptName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSeikyuDateTo
            // 
            this.lblSeikyuDateTo.AutoEllipsis = true;
            this.lblSeikyuDateTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuDateTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "請求日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.lblSeikyuDateTo.Location = new System.Drawing.Point(415, 50);
            this.lblSeikyuDateTo.Name = "lblSeikyuDateTo";
            this.lblSeikyuDateTo.Size = new System.Drawing.Size(121, 22);
            this.lblSeikyuDateTo.TabIndex = 10;
            this.lblSeikyuDateTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSeikyuDateFrom
            // 
            this.lblSeikyuDateFrom.AutoEllipsis = true;
            this.lblSeikyuDateFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuDateFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "請求日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.lblSeikyuDateFrom.Location = new System.Drawing.Point(260, 50);
            this.lblSeikyuDateFrom.Name = "lblSeikyuDateFrom";
            this.lblSeikyuDateFrom.Size = new System.Drawing.Size(121, 22);
            this.lblSeikyuDateFrom.TabIndex = 8;
            this.lblSeikyuDateFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBranchName
            // 
            this.lblBranchName.AutoEllipsis = true;
            this.lblBranchName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBranchName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "支店名L", true));
            this.lblBranchName.Location = new System.Drawing.Point(64, 20);
            this.lblBranchName.Name = "lblBranchName";
            this.lblBranchName.Size = new System.Drawing.Size(132, 22);
            this.lblBranchName.TabIndex = 1;
            this.lblBranchName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCourseName
            // 
            this.lblCourseName.AutoEllipsis = true;
            this.lblCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "コース名", true));
            this.lblCourseName.Location = new System.Drawing.Point(328, 20);
            this.lblCourseName.Name = "lblCourseName";
            this.lblCourseName.Size = new System.Drawing.Size(208, 22);
            this.lblCourseName.TabIndex = 6;
            this.lblCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(204, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "コース";
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
            // lblSeikyusakiNameFrom
            // 
            this.lblSeikyusakiNameFrom.AutoEllipsis = true;
            this.lblSeikyusakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuListBindingSource, "請求先名From", true));
            this.lblSeikyusakiNameFrom.Location = new System.Drawing.Point(132, 80);
            this.lblSeikyusakiNameFrom.Name = "lblSeikyusakiNameFrom";
            this.lblSeikyusakiNameFrom.Size = new System.Drawing.Size(151, 22);
            this.lblSeikyusakiNameFrom.TabIndex = 13;
            this.lblSeikyusakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "請求先";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(387, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "～";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(202, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "請求日";
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 128);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(611, 425);
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            this.fpSpread1.TabIndex = 1;
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
            this.fpSpread1_Sheet1.ColumnCount = 6;
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
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "請求先CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "請求先名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "今回請求額";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "決済状態";
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorSize = new System.Drawing.Size(7, 7);
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 75F;
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
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "請求先CD";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 75F;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = emptyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(4).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "請求先名";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 150F;
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
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = currencyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "今回請求額";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 100F;
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
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.Location = new System.Drawing.Point(262, 562);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(143, 18);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "5000/5000 決済中...";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.CausesValidation = false;
            this.btnExec.Location = new System.Drawing.Point(411, 559);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(103, 25);
            this.btnExec.TabIndex = 4;
            this.btnExec.Text = "処理開始(F9)";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(12, 559);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(244, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(520, 559);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 5;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // Form513B
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 596);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form513B";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "クレカ決済処理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form513B_FormClosing);
            this.Load += new System.EventHandler(this.Form513B_Load);
            this.Shown += new System.EventHandler(this.Form513B_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form513B_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource seikyuListBindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblSeikyusakiNameFrom;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCourseName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSeikyusakiCDFrom;
        private System.Windows.Forms.Label lblCourceCD;
        private System.Windows.Forms.Label lblDeptName;
        private System.Windows.Forms.Label lblSeikyuDateTo;
        private System.Windows.Forms.Label lblSeikyuDateFrom;
        private System.Windows.Forms.Label lblBranchName;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblSeikyusakiCDTo;
        private System.Windows.Forms.Label lblSeikyusakiNameTo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnQuit;
    }
}

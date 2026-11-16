namespace Emu.Bento
{
    partial class Form806
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
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType1 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType2 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType3 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpOperationDateTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.courseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dtpOperationDateFrom = new Emu.Bento.Common.Control.DateTimePickerA();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdoHistoryTypeError = new System.Windows.Forms.RadioButton();
            this.rdoHistoryTypeClean = new System.Windows.Forms.RadioButton();
            this.rdoHistoryTypeAll = new System.Windows.Forms.RadioButton();
            this.txtOperationTimeTo = new Emu.Bento.Common.Control.TextBoxAHMS(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.txtOperationTimeFrom = new Emu.Bento.Common.Control.TextBoxAHMS(this.components);
            this.btnQuery = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboListMax = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSearchWord = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.dtpOperationDateTo);
            this.groupBox1.Controls.Add(this.dtpOperationDateFrom);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.txtOperationTimeTo);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtOperationTimeFrom);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cboListMax);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtSearchWord);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 114);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // dtpOperationDateTo
            // 
            this.dtpOperationDateTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "操作日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpOperationDateTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpOperationDateTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpOperationDateTo.Location = new System.Drawing.Point(341, 21);
            this.dtpOperationDateTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpOperationDateTo.Name = "dtpOperationDateTo";
            this.dtpOperationDateTo.Size = new System.Drawing.Size(152, 22);
            this.dtpOperationDateTo.TabIndex = 4;
            // 
            // courseBindingSource
            // 
            this.courseBindingSource.AllowNew = false;
            this.courseBindingSource.DataSource = typeof(Emu.Bento.Data.OperationHistoryCriteria);
            // 
            // dtpOperationDateFrom
            // 
            this.dtpOperationDateFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "操作日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpOperationDateFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpOperationDateFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpOperationDateFrom.Location = new System.Drawing.Point(83, 21);
            this.dtpOperationDateFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dtpOperationDateFrom.Name = "dtpOperationDateFrom";
            this.dtpOperationDateFrom.Size = new System.Drawing.Size(152, 22);
            this.dtpOperationDateFrom.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdoHistoryTypeError);
            this.panel1.Controls.Add(this.rdoHistoryTypeClean);
            this.panel1.Controls.Add(this.rdoHistoryTypeAll);
            this.panel1.Location = new System.Drawing.Point(83, 78);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(202, 28);
            this.panel1.TabIndex = 9;
            // 
            // rdoHistoryTypeError
            // 
            this.rdoHistoryTypeError.AutoSize = true;
            this.rdoHistoryTypeError.Location = new System.Drawing.Point(132, 3);
            this.rdoHistoryTypeError.Name = "rdoHistoryTypeError";
            this.rdoHistoryTypeError.Size = new System.Drawing.Size(58, 19);
            this.rdoHistoryTypeError.TabIndex = 2;
            this.rdoHistoryTypeError.Text = "エラー";
            this.rdoHistoryTypeError.UseVisualStyleBackColor = true;
            // 
            // rdoHistoryTypeClean
            // 
            this.rdoHistoryTypeClean.AutoSize = true;
            this.rdoHistoryTypeClean.Location = new System.Drawing.Point(71, 3);
            this.rdoHistoryTypeClean.Name = "rdoHistoryTypeClean";
            this.rdoHistoryTypeClean.Size = new System.Drawing.Size(55, 19);
            this.rdoHistoryTypeClean.TabIndex = 1;
            this.rdoHistoryTypeClean.Text = "正常";
            this.rdoHistoryTypeClean.UseVisualStyleBackColor = true;
            // 
            // rdoHistoryTypeAll
            // 
            this.rdoHistoryTypeAll.AutoSize = true;
            this.rdoHistoryTypeAll.Checked = true;
            this.rdoHistoryTypeAll.Location = new System.Drawing.Point(3, 3);
            this.rdoHistoryTypeAll.Name = "rdoHistoryTypeAll";
            this.rdoHistoryTypeAll.Size = new System.Drawing.Size(62, 19);
            this.rdoHistoryTypeAll.TabIndex = 0;
            this.rdoHistoryTypeAll.TabStop = true;
            this.rdoHistoryTypeAll.Text = "すべて";
            this.rdoHistoryTypeAll.UseVisualStyleBackColor = true;
            // 
            // txtOperationTimeTo
            // 
            this.txtOperationTimeTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "操作時刻To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "HH:mm:ss"));
            this.txtOperationTimeTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtOperationTimeTo.Location = new System.Drawing.Point(500, 21);
            this.txtOperationTimeTo.MaxLength = 8;
            this.txtOperationTimeTo.Name = "txtOperationTimeTo";
            this.txtOperationTimeTo.Size = new System.Drawing.Size(66, 22);
            this.txtOperationTimeTo.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(314, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "～";
            // 
            // txtOperationTimeFrom
            // 
            this.txtOperationTimeFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "操作時刻From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "HH:mm:ss"));
            this.txtOperationTimeFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtOperationTimeFrom.Location = new System.Drawing.Point(242, 21);
            this.txtOperationTimeFrom.MaxLength = 8;
            this.txtOperationTimeFrom.Name = "txtOperationTimeFrom";
            this.txtOperationTimeFrom.Size = new System.Drawing.Size(66, 22);
            this.txtOperationTimeFrom.TabIndex = 2;
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(820, 83);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 17;
            this.btnQuery.Text = "検索(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(741, 53);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 15);
            this.label11.TabIndex = 16;
            this.label11.Text = "件";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(589, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 14;
            this.label10.Text = "表示数";
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
            this.cboListMax.Location = new System.Drawing.Point(647, 50);
            this.cboListMax.Name = "cboListMax";
            this.cboListMax.Size = new System.Drawing.Size(88, 23);
            this.cboListMax.TabIndex = 15;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Checked = true;
            this.rdoDesc.Location = new System.Drawing.Point(835, 22);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 13;
            this.rdoDesc.TabStop = true;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Location = new System.Drawing.Point(774, 22);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 12;
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
            this.cboSortKey.Location = new System.Drawing.Point(647, 21);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(121, 23);
            this.cboSortKey.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(589, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 15);
            this.label9.TabIndex = 10;
            this.label9.Text = "ソート";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "履歴タイプ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "検索ワード";
            // 
            // txtSearchWord
            // 
            this.txtSearchWord.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "操作内容", true));
            this.txtSearchWord.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtSearchWord.Location = new System.Drawing.Point(83, 50);
            this.txtSearchWord.Name = "txtSearchWord";
            this.txtSearchWord.Size = new System.Drawing.Size(400, 22);
            this.txtSearchWord.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "操作日時";
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1";
            this.fpSpread1.AllowUserZoom = false;
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fpSpread1.ColumnSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1.Location = new System.Drawing.Point(12, 132);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(899, 286);
            this.fpSpread1.TabIndex = 1;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.SetActiveViewport(0, -1, -1);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 5;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "操作日時";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "IPアドレス";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "操作内容";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "履歴タイプ";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "エラーフラグ";
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "操作日時";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "IPアドレス";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "操作内容";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 528F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "履歴タイプ";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "エラーフラグ";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 80F;
            this.fpSpread1_Sheet1.DataAutoCellTypes = false;
            this.fpSpread1_Sheet1.DataAutoHeadings = false;
            this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread1_Sheet1.DataSource = this.bindingSource1;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet1.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpSpread1_Sheet1.OperationMode = FarPoint.Win.Spread.OperationMode.SingleSelect;
            this.fpSpread1_Sheet1.Protect = false;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.RowHeader.Visible = false;
            this.fpSpread1_Sheet1.SelectionPolicy = FarPoint.Win.Spread.Model.SelectionPolicy.Single;
            this.fpSpread1_Sheet1.SelectionUnit = FarPoint.Win.Spread.Model.SelectionUnit.Row;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.Location = new System.Drawing.Point(107, 429);
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
            this.btnExport.Location = new System.Drawing.Point(699, 424);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(103, 25);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Excel出力";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(808, 424);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 5;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.courseBindingSource;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xls";
            this.saveFileDialog1.Filter = "Excel 97-2003 ブック(*.xls)|*.xls|Excel ブック(*.xlsx)|*.xlsx";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(12, 424);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(89, 25);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "クリア";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Form806
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 461);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form806";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "操作履歴一覧";
            this.Load += new System.EventHandler(this.Form806_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form806_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtSearchWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private Common.Control.ComboBoxA cboListMax;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private Common.Control.ComboBoxA cboSortKey;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnQuery;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource courseBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Common.Control.TextBoxAHMS txtOperationTimeTo;
        private System.Windows.Forms.Label label5;
        private Common.Control.TextBoxAHMS txtOperationTimeFrom;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdoHistoryTypeError;
        private System.Windows.Forms.RadioButton rdoHistoryTypeClean;
        private System.Windows.Forms.RadioButton rdoHistoryTypeAll;
        private System.Windows.Forms.Label label3;
        private Common.Control.DateTimePickerA dtpOperationDateTo;
        private Common.Control.DateTimePickerA dtpOperationDateFrom;
        private System.Windows.Forms.Button btnClear;
    }
}
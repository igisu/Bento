namespace Emu.Bento
{
    partial class Form820
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
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType4 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType5 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType6 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType7 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboOrderKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.orderHistoryCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnTokuisakiCD = new System.Windows.Forms.Button();
            this.lblTokuisakiName = new System.Windows.Forms.Label();
            this.txtTokuisakiCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUA = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.dtpOperationDateTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpOperationDateFrom = new Emu.Bento.Common.Control.DateTimePickerA();
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtIPAddress = new Emu.Bento.Common.Control.TextBoxA(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.orderHistoryCriteriaBindingSource)).BeginInit();
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
            this.groupBox1.Controls.Add(this.cboOrderKbn);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnTokuisakiCD);
            this.groupBox1.Controls.Add(this.lblTokuisakiName);
            this.groupBox1.Controls.Add(this.txtTokuisakiCD);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUA);
            this.groupBox1.Controls.Add(this.dtpOperationDateTo);
            this.groupBox1.Controls.Add(this.dtpOperationDateFrom);
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
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtIPAddress);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(899, 137);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // cboOrderKbn
            // 
            this.cboOrderKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboOrderKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOrderKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboOrderKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.orderHistoryCriteriaBindingSource, "注文区分", true));
            this.cboOrderKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboOrderKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrderKbn.FormattingEnabled = true;
            this.cboOrderKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboOrderKbn.Location = new System.Drawing.Point(83, 78);
            this.cboOrderKbn.Name = "cboOrderKbn";
            this.cboOrderKbn.Size = new System.Drawing.Size(176, 23);
            this.cboOrderKbn.TabIndex = 11;
            // 
            // orderHistoryCriteriaBindingSource
            // 
            this.orderHistoryCriteriaBindingSource.AllowNew = false;
            this.orderHistoryCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.OrderHistoryCriteria);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 81);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "注文区分";
            // 
            // btnTokuisakiCD
            // 
            this.btnTokuisakiCD.AutoSize = true;
            this.btnTokuisakiCD.Location = new System.Drawing.Point(341, 49);
            this.btnTokuisakiCD.Name = "btnTokuisakiCD";
            this.btnTokuisakiCD.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCD.TabIndex = 9;
            this.btnTokuisakiCD.TabStop = false;
            this.btnTokuisakiCD.Text = "参照";
            this.btnTokuisakiCD.UseVisualStyleBackColor = true;
            this.btnTokuisakiCD.Click += new System.EventHandler(this.btnTokuisakiCD_Click);
            // 
            // lblTokuisakiName
            // 
            this.lblTokuisakiName.AutoEllipsis = true;
            this.lblTokuisakiName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "得意先名", true));
            this.lblTokuisakiName.Location = new System.Drawing.Point(172, 50);
            this.lblTokuisakiName.Name = "lblTokuisakiName";
            this.lblTokuisakiName.Size = new System.Drawing.Size(163, 22);
            this.lblTokuisakiName.TabIndex = 8;
            this.lblTokuisakiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCD
            // 
            this.txtTokuisakiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "得意先ＣＤ", true));
            this.txtTokuisakiCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCD.Location = new System.Drawing.Point(83, 50);
            this.txtTokuisakiCD.MaxLength = 9;
            this.txtTokuisakiCD.Name = "txtTokuisakiCD";
            this.txtTokuisakiCD.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCD.TabIndex = 7;
            this.txtTokuisakiCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCD_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 6;
            this.label14.Text = "得意先";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "UA";
            // 
            // txtUA
            // 
            this.txtUA.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "UA", true));
            this.txtUA.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtUA.Location = new System.Drawing.Point(258, 107);
            this.txtUA.MaxLength = 300;
            this.txtUA.Name = "txtUA";
            this.txtUA.Size = new System.Drawing.Size(308, 22);
            this.txtUA.TabIndex = 15;
            // 
            // dtpOperationDateTo
            // 
            this.dtpOperationDateTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "操作日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpOperationDateTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpOperationDateTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpOperationDateTo.Location = new System.Drawing.Point(341, 21);
            this.dtpOperationDateTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpOperationDateTo.Name = "dtpOperationDateTo";
            this.dtpOperationDateTo.Size = new System.Drawing.Size(152, 22);
            this.dtpOperationDateTo.TabIndex = 4;
            // 
            // dtpOperationDateFrom
            // 
            this.dtpOperationDateFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "操作日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpOperationDateFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpOperationDateFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpOperationDateFrom.Location = new System.Drawing.Point(83, 21);
            this.dtpOperationDateFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dtpOperationDateFrom.Name = "dtpOperationDateFrom";
            this.dtpOperationDateFrom.Size = new System.Drawing.Size(152, 22);
            this.dtpOperationDateFrom.TabIndex = 1;
            // 
            // txtOperationTimeTo
            // 
            this.txtOperationTimeTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "操作時刻To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "HH:mm:ss"));
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
            this.txtOperationTimeFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "操作時刻From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "HH:mm:ss"));
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
            this.btnQuery.Location = new System.Drawing.Point(820, 106);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 23;
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
            this.label11.TabIndex = 22;
            this.label11.Text = "件";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(589, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 20;
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
            this.cboListMax.TabIndex = 21;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Checked = true;
            this.rdoDesc.Location = new System.Drawing.Point(835, 22);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 19;
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
            this.rdoAsc.TabIndex = 18;
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
            this.cboSortKey.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(589, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 15);
            this.label9.TabIndex = 16;
            this.label9.Text = "ソート";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "IPアドレス";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.orderHistoryCriteriaBindingSource, "IPアドレス", true));
            this.txtIPAddress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtIPAddress.Location = new System.Drawing.Point(83, 107);
            this.txtIPAddress.MaxLength = 15;
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(115, 22);
            this.txtIPAddress.TabIndex = 13;
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 155);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(899, 263);
            this.fpSpread1.TabIndex = 1;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 8;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "操作日時";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "得意先CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "得意先名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "注文区分";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "操作内容";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "トークン";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "IPアドレス";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "UA";
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "操作日時";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 150F;
            emptyCellType1.EnableSubEditor = false;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "得意先CD";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 75F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "得意先名";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 135F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "注文区分";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 130F;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = emptyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(4).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "操作内容";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 200F;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = emptyCellType5;
            this.fpSpread1_Sheet1.Columns.Get(5).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "トークン";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 200F;
            this.fpSpread1_Sheet1.Columns.Get(6).CellType = emptyCellType6;
            this.fpSpread1_Sheet1.Columns.Get(6).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "IPアドレス";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(7).CellType = emptyCellType7;
            this.fpSpread1_Sheet1.Columns.Get(7).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "UA";
            this.fpSpread1_Sheet1.Columns.Get(7).Width = 200F;
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
            this.errorProvider1.DataSource = this.orderHistoryCriteriaBindingSource;
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
            // Form820
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
            this.Name = "Form820";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Web注文履歴一覧";
            this.Load += new System.EventHandler(this.Form820_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form820_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.orderHistoryCriteriaBindingSource)).EndInit();
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
        private Common.Control.TextBoxA txtIPAddress;
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
        private System.Windows.Forms.BindingSource orderHistoryCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Common.Control.TextBoxAHMS txtOperationTimeTo;
        private System.Windows.Forms.Label label5;
        private Common.Control.TextBoxAHMS txtOperationTimeFrom;
        private Common.Control.DateTimePickerA dtpOperationDateTo;
        private Common.Control.DateTimePickerA dtpOperationDateFrom;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label3;
        private Common.Control.TextBoxA txtUA;
        private System.Windows.Forms.Button btnTokuisakiCD;
        private System.Windows.Forms.Label lblTokuisakiName;
        private Common.Control.TextBoxAN txtTokuisakiCD;
        private System.Windows.Forms.Label label14;
        private Common.Control.ComboBoxA cboOrderKbn;
        private System.Windows.Forms.Label label12;
    }
}
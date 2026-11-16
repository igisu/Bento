namespace Emu.Bento
{
    partial class Form709
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGroupSyubetsuName = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.courseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtGroupSyubetsu = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.cboCourseKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboListMax = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.chkInvalid = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.txtCourseName = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDetail = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtGroupSyubetsuName);
            this.groupBox1.Controls.Add(this.txtGroupSyubetsu);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboCourseKbn);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cboListMax);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.chkInvalid);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCourseName);
            this.groupBox1.Controls.Add(this.txtCourseCD);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(641, 166);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "グループ種別名";
            // 
            // txtGroupSyubetsuName
            // 
            this.txtGroupSyubetsuName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "グループ種別名", true));
            this.txtGroupSyubetsuName.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtGroupSyubetsuName.Location = new System.Drawing.Point(116, 80);
            this.txtGroupSyubetsuName.MaxLength = 40;
            this.txtGroupSyubetsuName.Name = "txtGroupSyubetsuName";
            this.txtGroupSyubetsuName.Size = new System.Drawing.Size(165, 22);
            this.txtGroupSyubetsuName.TabIndex = 5;
            // 
            // courseBindingSource
            // 
            this.courseBindingSource.AllowNew = false;
            this.courseBindingSource.DataSource = typeof(Emu.Bento.Data.CourseGroupCriteria);
            // 
            // txtGroupSyubetsu
            // 
            this.txtGroupSyubetsu.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "グループ種別ＣＤ", true));
            this.txtGroupSyubetsu.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtGroupSyubetsu.Location = new System.Drawing.Point(116, 52);
            this.txtGroupSyubetsu.MaxLength = 9;
            this.txtGroupSyubetsu.Name = "txtGroupSyubetsu";
            this.txtGroupSyubetsu.Size = new System.Drawing.Size(83, 22);
            this.txtGroupSyubetsu.TabIndex = 3;
            this.txtGroupSyubetsu.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "グループ種別CD";
            // 
            // cboCourseKbn
            // 
            this.cboCourseKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCourseKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCourseKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboCourseKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.courseBindingSource, "コース区分", true));
            this.cboCourseKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboCourseKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCourseKbn.FormattingEnabled = true;
            this.cboCourseKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboCourseKbn.Location = new System.Drawing.Point(116, 137);
            this.cboCourseKbn.Name = "cboCourseKbn";
            this.cboCourseKbn.Size = new System.Drawing.Size(121, 23);
            this.cboCourseKbn.TabIndex = 9;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 140);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 15);
            this.label12.TabIndex = 8;
            this.label12.Text = "コース区分";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(562, 131);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 20;
            this.btnQuery.Text = "検索(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(464, 112);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 15);
            this.label11.TabIndex = 19;
            this.label11.Text = "件";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(309, 112);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 17;
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
            this.cboListMax.Location = new System.Drawing.Point(370, 109);
            this.cboListMax.Name = "cboListMax";
            this.cboListMax.Size = new System.Drawing.Size(88, 23);
            this.cboListMax.TabIndex = 18;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Location = new System.Drawing.Point(575, 81);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 16;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Checked = true;
            this.rdoAsc.Location = new System.Drawing.Point(514, 81);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 15;
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
            this.cboSortKey.Location = new System.Drawing.Point(370, 80);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(138, 23);
            this.cboSortKey.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(309, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 15);
            this.label9.TabIndex = 13;
            this.label9.Text = "ソート";
            // 
            // chkInvalid
            // 
            this.chkInvalid.AutoSize = true;
            this.chkInvalid.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.courseBindingSource, "無効フラグ", true));
            this.chkInvalid.Location = new System.Drawing.Point(312, 52);
            this.chkInvalid.Name = "chkInvalid";
            this.chkInvalid.Size = new System.Drawing.Size(105, 19);
            this.chkInvalid.TabIndex = 12;
            this.chkInvalid.Text = "無効も含める";
            this.chkInvalid.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.courseBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(116, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "コース名";
            // 
            // txtCourseName
            // 
            this.txtCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "コース名", true));
            this.txtCourseName.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtCourseName.Location = new System.Drawing.Point(370, 21);
            this.txtCourseName.MaxLength = 30;
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.Size = new System.Drawing.Size(182, 22);
            this.txtCourseName.TabIndex = 11;
            // 
            // txtCourseCD
            // 
            this.txtCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.courseBindingSource, "コースＣＤ", true));
            this.txtCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCD.Location = new System.Drawing.Point(116, 109);
            this.txtCourseCD.MaxLength = 9;
            this.txtCourseCD.Name = "txtCourseCD";
            this.txtCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCD.TabIndex = 7;
            this.txtCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "コースCD";
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "";
            this.fpSpread1.AllowUserZoom = false;
            this.fpSpread1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fpSpread1.ColumnSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1.Location = new System.Drawing.Point(12, 184);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(641, 230);
            this.fpSpread1.TabIndex = 1;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.CellDoubleClick += new FarPoint.Win.Spread.CellClickEventHandler(this.fpSpread1_CellDoubleClick);
            this.fpSpread1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.fpSpread1_KeyPress);
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
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "部署";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "グループ種別CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "グループ種別名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "無効フラグ";
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "部署";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "グループ種別CD";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "グループ種別名";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 300F;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "無効フラグ";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 80F;
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
            this.lblCount.Location = new System.Drawing.Point(107, 425);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(80, 15);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "lblCount";
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.AutoSize = true;
            this.btnNew.CausesValidation = false;
            this.btnNew.Location = new System.Drawing.Point(223, 420);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(103, 25);
            this.btnNew.TabIndex = 4;
            this.btnNew.Text = "新規作成(F6)";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDetail
            // 
            this.btnDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetail.AutoSize = true;
            this.btnDetail.CausesValidation = false;
            this.btnDetail.Location = new System.Drawing.Point(332, 420);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(103, 25);
            this.btnDetail.TabIndex = 5;
            this.btnDetail.Text = "詳細画面(F8)";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoSize = true;
            this.btnExport.CausesValidation = false;
            this.btnExport.Location = new System.Drawing.Point(441, 420);
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
            this.btnQuit.Location = new System.Drawing.Point(550, 420);
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
            this.btnClear.Location = new System.Drawing.Point(12, 420);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(89, 25);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "クリア";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Form709
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 457);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnDetail);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form709";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "コース別グループマスタ";
            this.Load += new System.EventHandler(this.Form709_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form709_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.courseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtCourseName;
        private Common.Control.TextBoxAN txtCourseCD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private Common.Control.ComboBoxA cboListMax;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private Common.Control.ComboBoxA cboSortKey;
        private System.Windows.Forms.Label label9;
        private Common.Control.CheckBoxA chkInvalid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnQuery;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnQuit;
        private Common.Control.ComboBoxA cboCourseKbn;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource courseBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label5;
        private Common.Control.TextBoxA txtGroupSyubetsuName;
        private Common.Control.TextBoxAN txtGroupSyubetsu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnClear;
    }
}
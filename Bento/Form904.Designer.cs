namespace Emu.Bento
{
    partial class Form904
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
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType4 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType5 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType6 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboEigyoKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.tantousyaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label13 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboListMax = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.cboKengen = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.txtTantousyaName = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.txtTantousyaCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnClear = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tantousyaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboEigyoKbn);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cboListMax);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cboKengen);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTantousyaName);
            this.groupBox1.Controls.Add(this.txtTantousyaCD);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(685, 141);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // cboEigyoKbn
            // 
            this.cboEigyoKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboEigyoKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboEigyoKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboEigyoKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.tantousyaBindingSource, "営業業務区分", true));
            this.cboEigyoKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboEigyoKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEigyoKbn.FormattingEnabled = true;
            this.cboEigyoKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboEigyoKbn.Location = new System.Drawing.Point(436, 21);
            this.cboEigyoKbn.Name = "cboEigyoKbn";
            this.cboEigyoKbn.Size = new System.Drawing.Size(121, 23);
            this.cboEigyoKbn.TabIndex = 9;
            // 
            // tantousyaBindingSource
            // 
            this.tantousyaBindingSource.AllowNew = false;
            this.tantousyaBindingSource.DataSource = typeof(Emu.Bento.Data.TantousyaCriteria);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(333, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(97, 15);
            this.label13.TabIndex = 8;
            this.label13.Text = "営業業務区分";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(606, 106);
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
            this.label11.Location = new System.Drawing.Point(530, 111);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 15);
            this.label11.TabIndex = 16;
            this.label11.Text = "件";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(333, 111);
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
            this.cboListMax.Location = new System.Drawing.Point(436, 108);
            this.cboListMax.Name = "cboListMax";
            this.cboListMax.Size = new System.Drawing.Size(88, 23);
            this.cboListMax.TabIndex = 15;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Location = new System.Drawing.Point(624, 80);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 13;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Checked = true;
            this.rdoAsc.Location = new System.Drawing.Point(563, 80);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 12;
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
            this.cboSortKey.Location = new System.Drawing.Point(436, 79);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(121, 23);
            this.cboSortKey.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(333, 82);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 15);
            this.label9.TabIndex = 10;
            this.label9.Text = "ソート";
            // 
            // cboKengen
            // 
            this.cboKengen.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboKengen.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboKengen.BackColor = System.Drawing.SystemColors.Control;
            this.cboKengen.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.tantousyaBindingSource, "権限区分", true));
            this.cboKengen.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboKengen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKengen.FormattingEnabled = true;
            this.cboKengen.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboKengen.Location = new System.Drawing.Point(117, 108);
            this.cboKengen.Name = "cboKengen";
            this.cboKengen.Size = new System.Drawing.Size(121, 23);
            this.cboKengen.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "権限";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "所属部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.tantousyaBindingSource, "所属部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(117, 79);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "担当者名／カナ";
            // 
            // txtTantousyaName
            // 
            this.txtTantousyaName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "担当者名", true));
            this.txtTantousyaName.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtTantousyaName.Location = new System.Drawing.Point(117, 50);
            this.txtTantousyaName.MaxLength = 30;
            this.txtTantousyaName.Name = "txtTantousyaName";
            this.txtTantousyaName.Size = new System.Drawing.Size(190, 22);
            this.txtTantousyaName.TabIndex = 3;
            // 
            // txtTantousyaCD
            // 
            this.txtTantousyaCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tantousyaBindingSource, "担当者ＣＤ", true));
            this.txtTantousyaCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTantousyaCD.Location = new System.Drawing.Point(117, 21);
            this.txtTantousyaCD.MaxLength = 9;
            this.txtTantousyaCD.Name = "txtTantousyaCD";
            this.txtTantousyaCD.Size = new System.Drawing.Size(83, 22);
            this.txtTantousyaCD.TabIndex = 1;
            this.txtTantousyaCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "担当者CD";
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 159);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(685, 227);
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
            this.fpSpread1_Sheet1.ColumnCount = 6;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "担当者CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "担当者名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "担当者名カナ";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "所属部署";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "権限";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "営業業務区分";
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "担当者CD";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "担当者名";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "担当者名カナ";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "所属部署";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = emptyCellType5;
            this.fpSpread1_Sheet1.Columns.Get(4).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "権限";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = emptyCellType6;
            this.fpSpread1_Sheet1.Columns.Get(5).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "営業業務区分";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 100F;
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
            this.lblCount.Location = new System.Drawing.Point(107, 397);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(80, 15);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "lblCount";
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.AutoSize = true;
            this.btnSelect.Location = new System.Drawing.Point(485, 392);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(103, 25);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.Text = "選択";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(594, 392);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 5;
            this.btnQuit.Text = "閉じる(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.tantousyaBindingSource;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(12, 392);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(89, 25);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "クリア";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Form904
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnQuit;
            this.ClientSize = new System.Drawing.Size(709, 429);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form904";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "担当者選択ダイアログ";
            this.Load += new System.EventHandler(this.Form904_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form904_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tantousyaBindingSource)).EndInit();
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
        private Common.Control.TextBoxA txtTantousyaName;
        private Common.Control.TextBoxAN txtTantousyaCD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private Common.Control.ComboBoxA cboListMax;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private Common.Control.ComboBoxA cboSortKey;
        private System.Windows.Forms.Label label9;
        private Common.Control.ComboBoxA cboKengen;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Button btnQuery;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource tantousyaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Common.Control.ComboBoxA cboEigyoKbn;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnClear;
    }
}
namespace Emu.Bento
{
    partial class Form510
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
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType1 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType2 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType2 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType1 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType2 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType3 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboUriageKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.paySeikyuListCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dtpSyoribi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpYoteibiTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpYoteibiFrom = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpSeikyubiTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpSeikyubiFrom = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label13 = new System.Windows.Forms.Label();
            this.txtSeikyuAmount = new Emu.Bento.Common.Control.TextBoxAMC(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboListMax = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDetail = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paySeikyuListCriteriaBindingSource)).BeginInit();
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
            this.groupBox1.Controls.Add(this.cboUriageKbn);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.dtpSyoribi);
            this.groupBox1.Controls.Add(this.dtpYoteibiTo);
            this.groupBox1.Controls.Add(this.dtpYoteibiFrom);
            this.groupBox1.Controls.Add(this.dtpSeikyubiTo);
            this.groupBox1.Controls.Add(this.dtpSeikyubiFrom);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtSeikyuAmount);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cboListMax);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(801, 142);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // cboUriageKbn
            // 
            this.cboUriageKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboUriageKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboUriageKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboUriageKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.paySeikyuListCriteriaBindingSource, "売上区分", true));
            this.cboUriageKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboUriageKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUriageKbn.FormattingEnabled = true;
            this.cboUriageKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboUriageKbn.Location = new System.Drawing.Point(106, 51);
            this.cboUriageKbn.Name = "cboUriageKbn";
            this.cboUriageKbn.Size = new System.Drawing.Size(121, 23);
            this.cboUriageKbn.TabIndex = 5;
            // 
            // paySeikyuListCriteriaBindingSource
            // 
            this.paySeikyuListCriteriaBindingSource.AllowNew = false;
            this.paySeikyuListCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.PaySeikyuListCriteria);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 86);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(82, 15);
            this.label16.TabIndex = 6;
            this.label16.Text = "回収予定日";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 24);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 15);
            this.label15.TabIndex = 0;
            this.label15.Text = "請求日";
            // 
            // dtpSyoribi
            // 
            this.dtpSyoribi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuListCriteriaBindingSource, "処理日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSyoribi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSyoribi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSyoribi.Location = new System.Drawing.Point(554, 22);
            this.dtpSyoribi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSyoribi.Name = "dtpSyoribi";
            this.dtpSyoribi.Size = new System.Drawing.Size(152, 22);
            this.dtpSyoribi.TabIndex = 14;
            // 
            // dtpYoteibiTo
            // 
            this.dtpYoteibiTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuListCriteriaBindingSource, "回収予定日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpYoteibiTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpYoteibiTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpYoteibiTo.Location = new System.Drawing.Point(294, 81);
            this.dtpYoteibiTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpYoteibiTo.Name = "dtpYoteibiTo";
            this.dtpYoteibiTo.Size = new System.Drawing.Size(152, 22);
            this.dtpYoteibiTo.TabIndex = 9;
            // 
            // dtpYoteibiFrom
            // 
            this.dtpYoteibiFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuListCriteriaBindingSource, "回収予定日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpYoteibiFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpYoteibiFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpYoteibiFrom.Location = new System.Drawing.Point(106, 81);
            this.dtpYoteibiFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dtpYoteibiFrom.Name = "dtpYoteibiFrom";
            this.dtpYoteibiFrom.Size = new System.Drawing.Size(152, 22);
            this.dtpYoteibiFrom.TabIndex = 7;
            // 
            // dtpSeikyubiTo
            // 
            this.dtpSeikyubiTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuListCriteriaBindingSource, "請求日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubiTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubiTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubiTo.Location = new System.Drawing.Point(294, 22);
            this.dtpSeikyubiTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubiTo.Name = "dtpSeikyubiTo";
            this.dtpSeikyubiTo.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubiTo.TabIndex = 3;
            // 
            // dtpSeikyubiFrom
            // 
            this.dtpSeikyubiFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuListCriteriaBindingSource, "請求日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubiFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubiFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubiFrom.Location = new System.Drawing.Point(106, 22);
            this.dtpSeikyubiFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubiFrom.Name = "dtpSeikyubiFrom";
            this.dtpSeikyubiFrom.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubiFrom.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(213, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 15);
            this.label13.TabIndex = 12;
            this.label13.Text = "円以上";
            // 
            // txtSeikyuAmount
            // 
            this.txtSeikyuAmount.CopyFromWhenEmpty = null;
            this.txtSeikyuAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuListCriteriaBindingSource, "今回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "N0"));
            this.txtSeikyuAmount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSeikyuAmount.Location = new System.Drawing.Point(106, 110);
            this.txtSeikyuAmount.Name = "txtSeikyuAmount";
            this.txtSeikyuAmount.Size = new System.Drawing.Size(101, 22);
            this.txtSeikyuAmount.TabIndex = 11;
            this.txtSeikyuAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 113);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(94, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "今回請求額が";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(495, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "処理日";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(265, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "～";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(265, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "～";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "売上区分";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(722, 111);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 22;
            this.btnQuery.Text = "検索(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(648, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 15);
            this.label11.TabIndex = 21;
            this.label11.Text = "件";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(495, 86);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 19;
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
            this.cboListMax.Location = new System.Drawing.Point(554, 83);
            this.cboListMax.Name = "cboListMax";
            this.cboListMax.Size = new System.Drawing.Size(88, 23);
            this.cboListMax.TabIndex = 20;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Location = new System.Drawing.Point(742, 54);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 18;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Checked = true;
            this.rdoAsc.Location = new System.Drawing.Point(681, 54);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 17;
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
            this.cboSortKey.Location = new System.Drawing.Point(554, 51);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(121, 23);
            this.cboSortKey.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(495, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 15);
            this.label9.TabIndex = 15;
            this.label9.Text = "ソート";
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 160);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(801, 242);
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
            this.fpSpread1_Sheet1.ColumnCount = 9;
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
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "請求番号";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "請求日";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "売上区分";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "回収予定";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "買上金額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "入金額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "今回請求額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "請求編集日時";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "売上編集日時";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "請求番号";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 80F;
            generalCellType1.EnableSubEditor = false;
            generalCellType1.FormatString = "yyyy/MM/dd（ddd）";
            generalCellType1.IsDateFormat = true;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "請求日";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 120F;
            emptyCellType2.EnableSubEditor = false;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "売上区分";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 100F;
            generalCellType2.EnableSubEditor = false;
            generalCellType2.FormatString = "yyyy/MM/dd（ddd）";
            generalCellType2.IsDateFormat = true;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = generalCellType2;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "回収予定";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 120F;
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
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = currencyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "買上金額";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 120F;
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
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = currencyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "入金額";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 120F;
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
            this.fpSpread1_Sheet1.Columns.Get(6).CellType = currencyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "今回請求額";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "請求編集日時";
            this.fpSpread1_Sheet1.Columns.Get(7).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(8).Label = "売上編集日時";
            this.fpSpread1_Sheet1.Columns.Get(8).Width = 100F;
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
            this.lblCount.Location = new System.Drawing.Point(107, 413);
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
            this.btnExport.Location = new System.Drawing.Point(601, 408);
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
            this.btnQuit.Location = new System.Drawing.Point(710, 408);
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
            this.errorProvider1.DataSource = this.paySeikyuListCriteriaBindingSource;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.Filter = "Excel ブック(*.xlsx)|*.xlsx|Excel 97-2003 ブック(*.xls)|*.xls";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.AutoSize = true;
            this.btnDelete.CausesValidation = false;
            this.btnDelete.Location = new System.Drawing.Point(487, 408);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(108, 25);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "締め解除(F11)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(12, 408);
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
            this.btnDetail.Location = new System.Drawing.Point(378, 408);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(103, 25);
            this.btnDetail.TabIndex = 4;
            this.btnDetail.Text = "明細画面(F8)";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // Form510
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 445);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnDetail);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form510";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "電子決済請求一覧";
            this.Load += new System.EventHandler(this.Form510_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form510_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paySeikyuListCriteriaBindingSource)).EndInit();
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
        private System.Windows.Forms.BindingSource paySeikyuListCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label12;
        private Common.Control.TextBoxAMC txtSeikyuAmount;
        private System.Windows.Forms.Label label13;
        private Common.Control.DateTimePickerA dtpSyoribi;
        private Common.Control.DateTimePickerA dtpYoteibiTo;
        private Common.Control.DateTimePickerA dtpYoteibiFrom;
        private Common.Control.DateTimePickerA dtpSeikyubiTo;
        private Common.Control.DateTimePickerA dtpSeikyubiFrom;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private Common.Control.ComboBoxA cboUriageKbn;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDetail;
    }
}

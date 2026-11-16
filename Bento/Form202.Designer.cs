namespace Emu.Bento
{
    partial class Form202
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
            FarPoint.Win.Spread.InputMap fpSpread1_InputMapWhenFocusedNormal;
            FarPoint.Win.Spread.InputMap fpSpread1_InputMapWhenAncestorOfFocusedNormal;
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType1 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.TextCellType textCellType1 = new FarPoint.Win.Spread.CellType.TextCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType2 = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblNotice = new System.Windows.Forms.Label();
            this.ikkatsuChumonBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblPast = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMonth = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.txtYear = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.btnTokuisakiCD = new System.Windows.Forms.Button();
            this.lblTokuisakiName = new System.Windows.Forms.Label();
            this.txtTokuisakiCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnCopy = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHolidayCourseCD = new System.Windows.Forms.Button();
            this.lblHolidayCourseName = new System.Windows.Forms.Label();
            this.ikkatsuChumonCourseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtHolidayCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label13 = new System.Windows.Forms.Label();
            this.btnSundayCourseCD = new System.Windows.Forms.Button();
            this.lblSundayCourseName = new System.Windows.Forms.Label();
            this.txtSundayCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.btnSaturdayCourseCD = new System.Windows.Forms.Button();
            this.lblSaturdayCourseName = new System.Windows.Forms.Label();
            this.txtSaturdayCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.btnWeekdayCourseCD = new System.Windows.Forms.Button();
            this.lblWeekdayCourseName = new System.Windows.Forms.Label();
            this.txtWeekdayCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            fpSpread1_InputMapWhenFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenFocusedNormal.Parent = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent = new FarPoint.Win.Spread.InputMap();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikkatsuChumonBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikkatsuChumonCourseBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblNotice);
            this.groupBox1.Controls.Add(this.lblPast);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtMonth);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtYear);
            this.groupBox1.Controls.Add(this.btnTokuisakiCD);
            this.groupBox1.Controls.Add(this.lblTokuisakiName);
            this.groupBox1.Controls.Add(this.txtTokuisakiCD);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(801, 79);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // lblNotice
            // 
            this.lblNotice.AutoEllipsis = true;
            this.lblNotice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNotice.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonBindingSource, "受注時特記事項", true));
            this.lblNotice.Location = new System.Drawing.Point(350, 48);
            this.lblNotice.Name = "lblNotice";
            this.lblNotice.Size = new System.Drawing.Size(294, 22);
            this.lblNotice.TabIndex = 12;
            this.lblNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ikkatsuChumonBindingSource
            // 
            this.ikkatsuChumonBindingSource.DataSource = typeof(Emu.Bento.Data.IkkatsuChumonCriteria);
            // 
            // lblPast
            // 
            this.lblPast.AutoSize = true;
            this.lblPast.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblPast.ForeColor = System.Drawing.Color.Red;
            this.lblPast.Location = new System.Drawing.Point(200, 52);
            this.lblPast.Name = "lblPast";
            this.lblPast.Size = new System.Drawing.Size(111, 15);
            this.lblPast.TabIndex = 11;
            this.lblPast.Text = "過去の注文です";
            this.lblPast.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(172, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "月";
            // 
            // txtMonth
            // 
            this.txtMonth.CopyFromWhenEmpty = null;
            this.txtMonth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonBindingSource, "注文月", true));
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtMonth.Location = new System.Drawing.Point(139, 49);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(27, 22);
            this.txtMonth.TabIndex = 9;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(111, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "年";
            // 
            // txtYear
            // 
            this.txtYear.CopyFromWhenEmpty = null;
            this.txtYear.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonBindingSource, "注文年", true));
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtYear.Location = new System.Drawing.Point(64, 49);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(41, 22);
            this.txtYear.TabIndex = 7;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnTokuisakiCD
            // 
            this.btnTokuisakiCD.AutoSize = true;
            this.btnTokuisakiCD.Location = new System.Drawing.Point(350, 20);
            this.btnTokuisakiCD.Name = "btnTokuisakiCD";
            this.btnTokuisakiCD.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCD.TabIndex = 3;
            this.btnTokuisakiCD.TabStop = false;
            this.btnTokuisakiCD.Text = "参照";
            this.btnTokuisakiCD.UseVisualStyleBackColor = true;
            this.btnTokuisakiCD.Click += new System.EventHandler(this.btnTokuisakiCD_Click);
            // 
            // lblTokuisakiName
            // 
            this.lblTokuisakiName.AutoEllipsis = true;
            this.lblTokuisakiName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonBindingSource, "得意先名", true));
            this.lblTokuisakiName.Location = new System.Drawing.Point(153, 21);
            this.lblTokuisakiName.Name = "lblTokuisakiName";
            this.lblTokuisakiName.Size = new System.Drawing.Size(191, 22);
            this.lblTokuisakiName.TabIndex = 2;
            this.lblTokuisakiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCD
            // 
            this.txtTokuisakiCD.CopyFromWhenEmpty = null;
            this.txtTokuisakiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonBindingSource, "得意先ＣＤ", true));
            this.txtTokuisakiCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCD.Location = new System.Drawing.Point(64, 21);
            this.txtTokuisakiCD.MaxLength = 9;
            this.txtTokuisakiCD.Name = "txtTokuisakiCD";
            this.txtTokuisakiCD.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCD.TabIndex = 1;
            this.txtTokuisakiCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCD_KeyDown);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "得意先";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(722, 48);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 13;
            this.btnQuery.Text = "表示(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(419, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ikkatsuChumonBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(462, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 6;
            this.label9.Text = "注文月";
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 181);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(977, 345);
            this.fpSpread1.TabIndex = 2;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.EditModeOn += new System.EventHandler(this.fpSpread1_EditModeOn);
            this.fpSpread1.EditModeOff += new System.EventHandler(this.fpSpread1_EditModeOff);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyDown);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Back, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.StartEditing);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke('='), FarPoint.Win.Spread.SpreadActions.StartEditingFormula);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.C, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCopy);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.V, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardPasteAll);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.X, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCut);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Insert, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCopy);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Delete, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCut);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Insert, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardPasteAll);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Z, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.Undo);
            fpSpread1_InputMapWhenFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Y, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.Redo);
            this.fpSpread1.SetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused, FarPoint.Win.Spread.OperationMode.Normal, fpSpread1_InputMapWhenFocusedNormal);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.F2, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.StartEditing);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.F4, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.Redo);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Return, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Return, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousPageOfColumns);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextPageOfColumns);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Delete, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.ClearSelectedCellsData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Oem1, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.DateTimeNow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Tab, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Tab, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.A, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Back, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.Undo);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.F3, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.None);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToPreviousRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToPreviousColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToPreviousPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToFirstColumn);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToFirstCell);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToLastCellWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToFirstColumn);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToFirstCell);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToLastCellWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Space, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Space, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectColumn);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Space, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.CancelEditing);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ComboShowList);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ComboShowList);
            this.fpSpread1.SetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused, FarPoint.Win.Spread.OperationMode.Normal, fpSpread1_InputMapWhenAncestorOfFocusedNormal);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 5;
            this.fpSpread1_Sheet1.ColumnHeader.RowCount = 2;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "注文日";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "祝日";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "備考\r\n(顧客メモ)";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).ColumnSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "商品名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 2).Value = "備考\r\n(顧客メモ)";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 3).Value = "予測";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 4).Value = "注文";
            this.fpSpread1_Sheet1.Columns.Get(0).CanFocus = true;
            generalCellType1.EnableSubEditor = false;
            generalCellType1.FormatString = "yyyy/MM/dd（ddd）";
            generalCellType1.IsDateFormat = true;
            generalCellType1.ReadOnly = true;
            generalCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(0).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 120F;
            this.fpSpread1_Sheet1.Columns.Get(1).CanFocus = true;
            emptyCellType1.ReadOnly = true;
            emptyCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 120F;
            textCellType1.CharacterSet = FarPoint.Win.Spread.CellType.CharacterSet.AllIME;
            textCellType1.EnableSubEditor = false;
            textCellType1.MaxLength = 20;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = textCellType1;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "備考\r\n(顧客メモ)";
            this.fpSpread1_Sheet1.Columns.Get(2).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(2).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 180F;
            this.fpSpread1_Sheet1.Columns.Get(3).BackColor = System.Drawing.Color.WhiteSmoke;
            this.fpSpread1_Sheet1.Columns.Get(3).CanFocus = false;
            numberCellType1.DecimalPlaces = 0;
            numberCellType1.EnableSubEditor = false;
            numberCellType1.MaximumValue = 99999D;
            numberCellType1.MinimumValue = 0D;
            numberCellType1.ReadOnly = true;
            numberCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = numberCellType1;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "予測";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 50F;
            numberCellType2.DecimalPlaces = 0;
            numberCellType2.EnableSubEditor = false;
            numberCellType2.MaximumValue = 99999D;
            numberCellType2.MinimumValue = 0D;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = numberCellType2;
            this.fpSpread1_Sheet1.Columns.Get(4).ForeColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "注文";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 50F;
            this.fpSpread1_Sheet1.DataAutoCellTypes = false;
            this.fpSpread1_Sheet1.DataAutoHeadings = false;
            this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread1_Sheet1.DataSource = this.bindingSource1;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.DefaultStyle.Parent = "DataAreaDefault";
            this.fpSpread1_Sheet1.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.Center;
            this.fpSpread1_Sheet1.Protect = false;
            this.fpSpread1_Sheet1.RowHeader.AutoText = FarPoint.Win.Spread.HeaderAutoText.Blank;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.Location = new System.Drawing.Point(668, 532);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 25);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "更新(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoSize = true;
            this.btnExport.CausesValidation = false;
            this.btnExport.Location = new System.Drawing.Point(777, 532);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(103, 25);
            this.btnExport.TabIndex = 5;
            this.btnExport.Text = "Excel出力";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(886, 532);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 6;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xls";
            this.saveFileDialog1.Filter = "Excel 97-2003 ブック(*.xls)|*.xls|Excel ブック(*.xlsx)|*.xlsx";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.ikkatsuChumonBindingSource;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.AutoSize = true;
            this.btnCopy.Location = new System.Drawing.Point(280, 532);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(103, 25);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "コピー(F7)";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHolidayCourseCD);
            this.groupBox2.Controls.Add(this.lblHolidayCourseName);
            this.groupBox2.Controls.Add(this.txtHolidayCourseCD);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.btnSundayCourseCD);
            this.groupBox2.Controls.Add(this.lblSundayCourseName);
            this.groupBox2.Controls.Add(this.txtSundayCourseCD);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.btnSaturdayCourseCD);
            this.groupBox2.Controls.Add(this.lblSaturdayCourseName);
            this.groupBox2.Controls.Add(this.txtSaturdayCourseCD);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btnWeekdayCourseCD);
            this.groupBox2.Controls.Add(this.lblWeekdayCourseName);
            this.groupBox2.Controls.Add(this.txtWeekdayCourseCD);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(801, 77);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "コース";
            // 
            // btnHolidayCourseCD
            // 
            this.btnHolidayCourseCD.AutoSize = true;
            this.btnHolidayCourseCD.Location = new System.Drawing.Point(736, 47);
            this.btnHolidayCourseCD.Name = "btnHolidayCourseCD";
            this.btnHolidayCourseCD.Size = new System.Drawing.Size(47, 25);
            this.btnHolidayCourseCD.TabIndex = 15;
            this.btnHolidayCourseCD.TabStop = false;
            this.btnHolidayCourseCD.Text = "参照";
            this.btnHolidayCourseCD.UseVisualStyleBackColor = true;
            this.btnHolidayCourseCD.Click += new System.EventHandler(this.btnHolidayCourseCD_Click);
            // 
            // lblHolidayCourseName
            // 
            this.lblHolidayCourseName.AutoEllipsis = true;
            this.lblHolidayCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblHolidayCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "祝日コース名", true));
            this.lblHolidayCourseName.Location = new System.Drawing.Point(539, 48);
            this.lblHolidayCourseName.Name = "lblHolidayCourseName";
            this.lblHolidayCourseName.Size = new System.Drawing.Size(191, 22);
            this.lblHolidayCourseName.TabIndex = 14;
            this.lblHolidayCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ikkatsuChumonCourseBindingSource
            // 
            this.ikkatsuChumonCourseBindingSource.DataSource = typeof(Emu.Bento.Data.IkkatsuChumonCourse);
            // 
            // txtHolidayCourseCD
            // 
            this.txtHolidayCourseCD.CopyFromWhenEmpty = null;
            this.txtHolidayCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "祝日コースＣＤ", true));
            this.txtHolidayCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtHolidayCourseCD.Location = new System.Drawing.Point(450, 49);
            this.txtHolidayCourseCD.MaxLength = 9;
            this.txtHolidayCourseCD.Name = "txtHolidayCourseCD";
            this.txtHolidayCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtHolidayCourseCD.TabIndex = 13;
            this.txtHolidayCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtHolidayCourseCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHolidayCourseCD_KeyDown);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(407, 52);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(37, 15);
            this.label13.TabIndex = 12;
            this.label13.Text = "祝日";
            // 
            // btnSundayCourseCD
            // 
            this.btnSundayCourseCD.AutoSize = true;
            this.btnSundayCourseCD.Location = new System.Drawing.Point(736, 19);
            this.btnSundayCourseCD.Name = "btnSundayCourseCD";
            this.btnSundayCourseCD.Size = new System.Drawing.Size(47, 25);
            this.btnSundayCourseCD.TabIndex = 11;
            this.btnSundayCourseCD.TabStop = false;
            this.btnSundayCourseCD.Text = "参照";
            this.btnSundayCourseCD.UseVisualStyleBackColor = true;
            this.btnSundayCourseCD.Click += new System.EventHandler(this.btnSundayCourseCD_Click);
            // 
            // lblSundayCourseName
            // 
            this.lblSundayCourseName.AutoEllipsis = true;
            this.lblSundayCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSundayCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "日曜コース名", true));
            this.lblSundayCourseName.Location = new System.Drawing.Point(539, 20);
            this.lblSundayCourseName.Name = "lblSundayCourseName";
            this.lblSundayCourseName.Size = new System.Drawing.Size(191, 22);
            this.lblSundayCourseName.TabIndex = 10;
            this.lblSundayCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSundayCourseCD
            // 
            this.txtSundayCourseCD.CopyFromWhenEmpty = null;
            this.txtSundayCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "日曜コースＣＤ", true));
            this.txtSundayCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSundayCourseCD.Location = new System.Drawing.Point(450, 21);
            this.txtSundayCourseCD.MaxLength = 9;
            this.txtSundayCourseCD.Name = "txtSundayCourseCD";
            this.txtSundayCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtSundayCourseCD.TabIndex = 9;
            this.txtSundayCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSundayCourseCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSundayCourseCD_KeyDown);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(407, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 15);
            this.label11.TabIndex = 8;
            this.label11.Text = "日曜";
            // 
            // btnSaturdayCourseCD
            // 
            this.btnSaturdayCourseCD.AutoSize = true;
            this.btnSaturdayCourseCD.Location = new System.Drawing.Point(335, 47);
            this.btnSaturdayCourseCD.Name = "btnSaturdayCourseCD";
            this.btnSaturdayCourseCD.Size = new System.Drawing.Size(47, 25);
            this.btnSaturdayCourseCD.TabIndex = 7;
            this.btnSaturdayCourseCD.TabStop = false;
            this.btnSaturdayCourseCD.Text = "参照";
            this.btnSaturdayCourseCD.UseVisualStyleBackColor = true;
            this.btnSaturdayCourseCD.Click += new System.EventHandler(this.btnSaturdayCourseCD_Click);
            // 
            // lblSaturdayCourseName
            // 
            this.lblSaturdayCourseName.AutoEllipsis = true;
            this.lblSaturdayCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSaturdayCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "土曜コース名", true));
            this.lblSaturdayCourseName.Location = new System.Drawing.Point(138, 48);
            this.lblSaturdayCourseName.Name = "lblSaturdayCourseName";
            this.lblSaturdayCourseName.Size = new System.Drawing.Size(191, 22);
            this.lblSaturdayCourseName.TabIndex = 6;
            this.lblSaturdayCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSaturdayCourseCD
            // 
            this.txtSaturdayCourseCD.CopyFromWhenEmpty = null;
            this.txtSaturdayCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "土曜コースＣＤ", true));
            this.txtSaturdayCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSaturdayCourseCD.Location = new System.Drawing.Point(49, 49);
            this.txtSaturdayCourseCD.MaxLength = 9;
            this.txtSaturdayCourseCD.Name = "txtSaturdayCourseCD";
            this.txtSaturdayCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtSaturdayCourseCD.TabIndex = 5;
            this.txtSaturdayCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSaturdayCourseCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSaturdayCourseCD_KeyDown);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "土曜";
            // 
            // btnWeekdayCourseCD
            // 
            this.btnWeekdayCourseCD.AutoSize = true;
            this.btnWeekdayCourseCD.Location = new System.Drawing.Point(335, 19);
            this.btnWeekdayCourseCD.Name = "btnWeekdayCourseCD";
            this.btnWeekdayCourseCD.Size = new System.Drawing.Size(47, 25);
            this.btnWeekdayCourseCD.TabIndex = 3;
            this.btnWeekdayCourseCD.TabStop = false;
            this.btnWeekdayCourseCD.Text = "参照";
            this.btnWeekdayCourseCD.UseVisualStyleBackColor = true;
            this.btnWeekdayCourseCD.Click += new System.EventHandler(this.btnWeekdayCourseCD_Click);
            // 
            // lblWeekdayCourseName
            // 
            this.lblWeekdayCourseName.AutoEllipsis = true;
            this.lblWeekdayCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWeekdayCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "平日コース名", true));
            this.lblWeekdayCourseName.Location = new System.Drawing.Point(138, 20);
            this.lblWeekdayCourseName.Name = "lblWeekdayCourseName";
            this.lblWeekdayCourseName.Size = new System.Drawing.Size(191, 22);
            this.lblWeekdayCourseName.TabIndex = 2;
            this.lblWeekdayCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtWeekdayCourseCD
            // 
            this.txtWeekdayCourseCD.CopyFromWhenEmpty = null;
            this.txtWeekdayCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuChumonCourseBindingSource, "平日コースＣＤ", true));
            this.txtWeekdayCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtWeekdayCourseCD.Location = new System.Drawing.Point(49, 21);
            this.txtWeekdayCourseCD.MaxLength = 9;
            this.txtWeekdayCourseCD.Name = "txtWeekdayCourseCD";
            this.txtWeekdayCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtWeekdayCourseCD.TabIndex = 1;
            this.txtWeekdayCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtWeekdayCourseCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtWeekdayCourseCD_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "平日";
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            this.errorProvider2.DataSource = this.ikkatsuChumonCourseBindingSource;
            // 
            // Form202
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 569);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form202";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "一括注文入力";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form202_FormClosing);
            this.Load += new System.EventHandler(this.Form202_Load);
            this.Shown += new System.EventHandler(this.Form202_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form202_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikkatsuChumonBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikkatsuChumonCourseBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.BindingSource ikkatsuChumonBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnTokuisakiCD;
        private System.Windows.Forms.Label lblTokuisakiName;
        private Common.Control.TextBoxAN txtTokuisakiCD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private Common.Control.TextBoxAN txtMonth;
        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxAN txtYear;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnWeekdayCourseCD;
        private System.Windows.Forms.Label lblWeekdayCourseName;
        private Common.Control.TextBoxAN txtWeekdayCourseCD;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnHolidayCourseCD;
        private System.Windows.Forms.Label lblHolidayCourseName;
        private Common.Control.TextBoxAN txtHolidayCourseCD;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnSundayCourseCD;
        private System.Windows.Forms.Label lblSundayCourseName;
        private Common.Control.TextBoxAN txtSundayCourseCD;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSaturdayCourseCD;
        private System.Windows.Forms.Label lblSaturdayCourseName;
        private Common.Control.TextBoxAN txtSaturdayCourseCD;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.BindingSource ikkatsuChumonCourseBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        private System.Windows.Forms.Label lblPast;
        private System.Windows.Forms.Label lblNotice;
    }
}
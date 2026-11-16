namespace Emu.Bento
{
    partial class Form102
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
            FarPoint.Win.Spread.InputMap fpSpread1_InputMapWhenFocusedNormal;
            FarPoint.Win.Spread.InputMap fpSpread1_InputMapWhenAncestorOfFocusedNormal;
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType1 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType2 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpChumonbi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.seisanYoteiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cboGroupSyubetsu = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.lblKikan = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtWeeks = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.btnIncrease = new System.Windows.Forms.Button();
            this.btnDecrease = new System.Windows.Forms.Button();
            this.btnMochidashiHanei = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtpTarget = new Emu.Bento.Common.Control.DateTimePickerA();
            this.seisanYoteiFromBetsubiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnLoad = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblUriage = new System.Windows.Forms.Label();
            this.lblInput = new System.Windows.Forms.Label();
            fpSpread1_InputMapWhenFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal = new FarPoint.Win.Spread.InputMap();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seisanYoteiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seisanYoteiFromBetsubiBindingSource)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpChumonbi);
            this.groupBox1.Controls.Add(this.cboGroupSyubetsu);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // dtpChumonbi
            // 
            this.dtpChumonbi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seisanYoteiBindingSource, "注文日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpChumonbi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpChumonbi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpChumonbi.Location = new System.Drawing.Point(64, 21);
            this.dtpChumonbi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpChumonbi.Name = "dtpChumonbi";
            this.dtpChumonbi.Size = new System.Drawing.Size(152, 22);
            this.dtpChumonbi.TabIndex = 1;
            // 
            // seisanYoteiBindingSource
            // 
            this.seisanYoteiBindingSource.DataSource = typeof(Emu.Bento.Data.SeisanYoteiCriteria);
            // 
            // cboGroupSyubetsu
            // 
            this.cboGroupSyubetsu.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboGroupSyubetsu.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboGroupSyubetsu.BackColor = System.Drawing.SystemColors.Control;
            this.cboGroupSyubetsu.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seisanYoteiBindingSource, "グループ種別ＣＤ", true));
            this.cboGroupSyubetsu.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboGroupSyubetsu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGroupSyubetsu.FormattingEnabled = true;
            this.cboGroupSyubetsu.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboGroupSyubetsu.Location = new System.Drawing.Point(352, 49);
            this.cboGroupSyubetsu.Name = "cboGroupSyubetsu";
            this.cboGroupSyubetsu.Size = new System.Drawing.Size(138, 23);
            this.cboGroupSyubetsu.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(229, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "コースグループ種別";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seisanYoteiBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(64, 49);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 3;
            this.cboBusyo.SelectedValueChanged += new System.EventHandler(this.cboBusyo_SelectedValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "注文日";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(503, 47);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 6;
            this.btnQuery.Text = "表示(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 96);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(1073, 374);
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            this.fpSpread1.TabIndex = 3;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.VerticalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.VerticalScrollBar.Name = "";
            this.fpSpread1.VerticalScrollBar.Renderer = enhancedScrollBarRenderer2;
            this.fpSpread1.VerticalScrollBarMode = FarPoint.Win.VerticalScrollMode.PixelAndRow;
            this.fpSpread1.EditModeOn += new System.EventHandler(this.fpSpread1_EditModeOn);
            this.fpSpread1.EditModeOff += new System.EventHandler(this.fpSpread1_EditModeOff);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyDown);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Back, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.StartEditing);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke('='), FarPoint.Win.Spread.SpreadActions.StartEditingFormula);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.C, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCopy);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.V, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardPasteAll);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.X, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCut);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Insert, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCopy);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Delete, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardCut);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Insert, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ClipboardPasteAll);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Z, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.Undo);
            fpSpread1_InputMapWhenFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Y, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.Redo);
            this.fpSpread1.SetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused, FarPoint.Win.Spread.OperationMode.Normal, fpSpread1_InputMapWhenFocusedNormal);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.F2, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.StartEditing);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.F4, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.Redo);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Return, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Return, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousPageOfColumns);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextPageOfColumns);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Delete, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.ClearSelectedCellsData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Oem1, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.DateTimeNow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Tab, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Tab, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.A, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Back, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.Undo);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.F3, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.None);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.None);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextRowWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextColumnWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToPreviousRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToPreviousColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Left, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Right, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextColumnVisual);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToPreviousPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToNextPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToPreviousPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToNextPageOfRows);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.PageUp, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToPreviousSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Next, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToNextSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.MoveToFirstColumn);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToFirstCell);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.MoveToLastCellWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToFirstColumn);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Home, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToFirstCell);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.End, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ExtendToLastCellWithData);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Space, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectRow);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Space, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectColumn);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Space, ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                    | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.SelectSheet);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.None), FarPoint.Win.Spread.SpreadActions.CancelEditing);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Down, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ComboShowList);
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Put(new FarPoint.Win.Spread.Keystroke(System.Windows.Forms.Keys.Up, ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.None)))), FarPoint.Win.Spread.SpreadActions.ComboShowList);
            this.fpSpread1.SetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused, FarPoint.Win.Spread.OperationMode.Normal, fpSpread1_InputMapWhenAncestorOfFocusedNormal);
            // 
            // fpSpread1_Sheet1
            // 
            this.fpSpread1_Sheet1.Reset();
            this.fpSpread1_Sheet1.SheetName = "Sheet1";
            // Formulas and custom names must be loaded with R1C1 reference style
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.R1C1;
            this.fpSpread1_Sheet1.ColumnCount = 4;
            this.fpSpread1_Sheet1.ColumnHeader.RowCount = 2;
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
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "日付";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "商品名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).RowSpan = 2;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "生産予定数";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, 0).Value = "日付";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.Rows.Get(1).Height = 50F;
            this.fpSpread1_Sheet1.Columns.Get(0).CanFocus = false;
            generalCellType1.EnableSubEditor = false;
            generalCellType1.FormatString = "yyyy/MM/dd";
            generalCellType1.IsDateFormat = true;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "日付";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(1).CanFocus = true;
            emptyCellType1.ReadOnly = true;
            emptyCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(2).CanFocus = true;
            emptyCellType2.ReadOnly = true;
            emptyCellType2.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(3).CanFocus = true;
            numberCellType1.DecimalPlaces = 0;
            numberCellType1.EnableSubEditor = false;
            numberCellType1.MaximumValue = 999999999D;
            numberCellType1.MinimumValue = 0D;
            numberCellType1.ReadOnly = true;
            numberCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = numberCellType1;
            this.fpSpread1_Sheet1.Columns.Get(3).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 90F;
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
            this.fpSpread1_Sheet1.Protect = false;
            this.fpSpread1_Sheet1.RowHeader.Columns.Default.Resizable = false;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.Parent = "RowHeaderEnhanced";
            this.fpSpread1_Sheet1.RowHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.RowHeader.Visible = false;
            this.fpSpread1_Sheet1.SheetCornerStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.SheetCornerStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.SheetCornerStyle.Parent = "CornerEnhanced";
            this.fpSpread1_Sheet1.SheetCornerStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ReferenceStyle = FarPoint.Win.Spread.Model.ReferenceStyle.A1;
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(982, 508);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 13;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.seisanYoteiBindingSource;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.Location = new System.Drawing.Point(639, 508);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 25);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblKikan
            // 
            this.lblKikan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblKikan.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblKikan.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seisanYoteiBindingSource, "期間", true));
            this.lblKikan.Location = new System.Drawing.Point(45, 480);
            this.lblKikan.Name = "lblKikan";
            this.lblKikan.Size = new System.Drawing.Size(200, 22);
            this.lblKikan.TabIndex = 4;
            this.lblKikan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(251, 484);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(155, 15);
            this.label12.TabIndex = 5;
            this.label12.Text = "の期間の祝日を除く曜日";
            // 
            // txtWeeks
            // 
            this.txtWeeks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtWeeks.CopyFromWhenEmpty = null;
            this.txtWeeks.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seisanYoteiBindingSource, "計算対象週数", true));
            this.txtWeeks.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtWeeks.Location = new System.Drawing.Point(12, 510);
            this.txtWeeks.MaxLength = 2;
            this.txtWeeks.Name = "txtWeeks";
            this.txtWeeks.Size = new System.Drawing.Size(27, 22);
            this.txtWeeks.TabIndex = 6;
            this.txtWeeks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtWeeks.Validated += new System.EventHandler(this.txtWeeks_Validated);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 513);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(349, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "回の過去の曜日で生産予定数（受注見込数）を作成する";
            // 
            // btnIncrease
            // 
            this.btnIncrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIncrease.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnIncrease.Location = new System.Drawing.Point(45, 505);
            this.btnIncrease.Name = "btnIncrease";
            this.btnIncrease.Size = new System.Drawing.Size(30, 16);
            this.btnIncrease.TabIndex = 7;
            this.btnIncrease.Text = "▲";
            this.btnIncrease.UseVisualStyleBackColor = true;
            this.btnIncrease.Click += new System.EventHandler(this.btnIncrease_Click);
            // 
            // btnDecrease
            // 
            this.btnDecrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDecrease.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDecrease.Location = new System.Drawing.Point(45, 521);
            this.btnDecrease.Name = "btnDecrease";
            this.btnDecrease.Size = new System.Drawing.Size(30, 16);
            this.btnDecrease.TabIndex = 8;
            this.btnDecrease.Text = "▼";
            this.btnDecrease.UseVisualStyleBackColor = true;
            this.btnDecrease.Click += new System.EventHandler(this.btnDecrease_Click);
            // 
            // btnMochidashiHanei
            // 
            this.btnMochidashiHanei.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMochidashiHanei.AutoSize = true;
            this.btnMochidashiHanei.Location = new System.Drawing.Point(748, 508);
            this.btnMochidashiHanei.Name = "btnMochidashiHanei";
            this.btnMochidashiHanei.Size = new System.Drawing.Size(119, 25);
            this.btnMochidashiHanei.TabIndex = 11;
            this.btnMochidashiHanei.Text = "日配持出に反映";
            this.btnMochidashiHanei.UseVisualStyleBackColor = true;
            this.btnMochidashiHanei.Click += new System.EventHandler(this.btnMochidashiHanei_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoSize = true;
            this.btnExport.CausesValidation = false;
            this.btnExport.Location = new System.Drawing.Point(873, 508);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(103, 25);
            this.btnExport.TabIndex = 12;
            this.btnExport.Text = "Excel出力";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xlsx";
            this.saveFileDialog1.Filter = "Excel ブック(*.xlsx)|*.xlsx|Excel 97-2003 ブック(*.xls)|*.xls";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtpTarget);
            this.groupBox2.Controls.Add(this.btnLoad);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(717, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 78);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "別日データ読込";
            // 
            // dtpTarget
            // 
            this.dtpTarget.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seisanYoteiFromBetsubiBindingSource, "注文日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpTarget.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpTarget.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpTarget.Location = new System.Drawing.Point(64, 21);
            this.dtpTarget.Margin = new System.Windows.Forms.Padding(4);
            this.dtpTarget.Name = "dtpTarget";
            this.dtpTarget.Size = new System.Drawing.Size(152, 22);
            this.dtpTarget.TabIndex = 1;
            // 
            // seisanYoteiFromBetsubiBindingSource
            // 
            this.seisanYoteiFromBetsubiBindingSource.DataSource = typeof(Emu.Bento.Data.SeisanYoteiFromBetsubi);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.AutoSize = true;
            this.btnLoad.Location = new System.Drawing.Point(143, 47);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(73, 25);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "読込";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "注文日";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnImport);
            this.groupBox3.Location = new System.Drawing.Point(600, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(111, 78);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "注文予測取込";
            // 
            // btnImport
            // 
            this.btnImport.AutoSize = true;
            this.btnImport.Location = new System.Drawing.Point(19, 47);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(73, 25);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "取込";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.lblUriage);
            this.groupBox4.Controls.Add(this.lblInput);
            this.groupBox4.Location = new System.Drawing.Point(953, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(132, 79);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "凡例";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(67, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 15);
            this.label7.TabIndex = 9;
            this.label7.Text = "売上から";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(67, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 8;
            this.label6.Text = "手入力";
            // 
            // lblUriage
            // 
            this.lblUriage.AutoEllipsis = true;
            this.lblUriage.BackColor = System.Drawing.Color.White;
            this.lblUriage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUriage.ForeColor = System.Drawing.Color.Red;
            this.lblUriage.Location = new System.Drawing.Point(6, 50);
            this.lblUriage.Name = "lblUriage";
            this.lblUriage.Size = new System.Drawing.Size(55, 22);
            this.lblUriage.TabIndex = 7;
            this.lblUriage.Text = "赤色";
            this.lblUriage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInput
            // 
            this.lblInput.AutoEllipsis = true;
            this.lblInput.BackColor = System.Drawing.Color.White;
            this.lblInput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(255)))));
            this.lblInput.Location = new System.Drawing.Point(6, 21);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(55, 22);
            this.lblInput.TabIndex = 6;
            this.lblInput.Text = "青色";
            this.lblInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form102
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1097, 545);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnMochidashiHanei);
            this.Controls.Add(this.btnDecrease);
            this.Controls.Add(this.btnIncrease);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtWeeks);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lblKikan);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form102";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "生産予定数入力";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form102_FormClosing);
            this.Load += new System.EventHandler(this.Form102_Load);
            this.Shown += new System.EventHandler(this.Form102_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form102_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seisanYoteiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seisanYoteiFromBetsubiBindingSource)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnQuery;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource seisanYoteiBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label4;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblKikan;
        private Common.Control.TextBoxAN txtWeeks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMochidashiHanei;
        private System.Windows.Forms.Button btnDecrease;
        private System.Windows.Forms.Button btnIncrease;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Common.Control.ComboBoxA cboGroupSyubetsu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.BindingSource seisanYoteiFromBetsubiBindingSource;
        private Common.Control.DateTimePickerA dtpChumonbi;
        private Common.Control.DateTimePickerA dtpTarget;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblUriage;
        private System.Windows.Forms.Label lblInput;
    }
}

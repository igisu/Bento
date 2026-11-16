namespace Emu.Bento
{
    partial class Form604
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
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.ButtonCellType buttonCellType1 = new FarPoint.Win.Spread.CellType.ButtonCellType();
            FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType1 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType2 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType1 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType2 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType1 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType3 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType4 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType5 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType6 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType1 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType3 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType2 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtFile = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.anserApiImportCriteria = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.btnImportFile = new System.Windows.Forms.Button();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblCount = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.lblNyukinTotal = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.anserApiContainerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtNyukinBiko = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cboTekiyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            fpSpread1_InputMapWhenFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal = new FarPoint.Win.Spread.InputMap();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anserApiImportCriteria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.anserApiContainerBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSelectFile);
            this.groupBox1.Controls.Add(this.txtFile);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnImportFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 50);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "取込設定";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.AutoSize = true;
            this.btnSelectFile.Location = new System.Drawing.Point(380, 19);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(47, 25);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.TabStop = false;
            this.btnSelectFile.Text = "参照";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtFile
            // 
            this.txtFile.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.anserApiImportCriteria, "取込ファイル", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtFile.Location = new System.Drawing.Point(83, 21);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(291, 22);
            this.txtFile.TabIndex = 1;
            this.txtFile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFile_KeyDown);
            // 
            // anserApiImportCriteria
            // 
            this.anserApiImportCriteria.AllowNew = false;
            this.anserApiImportCriteria.DataSource = typeof(Emu.Bento.Data.AnserApiImportCriteria);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "APIファイル";
            // 
            // btnImportFile
            // 
            this.btnImportFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportFile.AutoSize = true;
            this.btnImportFile.Location = new System.Drawing.Point(461, 19);
            this.btnImportFile.Name = "btnImportFile";
            this.btnImportFile.Size = new System.Drawing.Size(73, 25);
            this.btnImportFile.TabIndex = 3;
            this.btnImportFile.Text = "読込";
            this.btnImportFile.UseVisualStyleBackColor = true;
            this.btnImportFile.Click += new System.EventHandler(this.btnImportFile_Click);
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 68);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(1177, 266);
            this.fpSpread1.TabIndex = 2;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.EditModeOn += new System.EventHandler(this.fpSpread1_EditModeOn);
            this.fpSpread1.EditModeOff += new System.EventHandler(this.fpSpread1_EditModeOff);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
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
            this.fpSpread1_Sheet1.ColumnCount = 14;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "振込依頼人名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "選択";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "得意先名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "部署名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "取込入金額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "手数料";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "負担";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "前回請求額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "今回請求額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "請求後入金";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 10).Value = "売掛残";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 11).Value = "入金日";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 12).Value = "取引名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 13).Value = "無効";
            this.fpSpread1_Sheet1.Columns.Get(0).CanFocus = false;
            emptyCellType1.ReadOnly = true;
            emptyCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "振込依頼人名";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 155F;
            buttonCellType1.ButtonColor2 = System.Drawing.SystemColors.ButtonFace;
            buttonCellType1.Text = "選択";
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = buttonCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "選択";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 50F;
            comboBoxCellType1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            comboBoxCellType1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            comboBoxCellType1.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = comboBoxCellType1;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "得意先名";
            this.fpSpread1_Sheet1.Columns.Get(2).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(2).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 160F;
            this.fpSpread1_Sheet1.Columns.Get(3).CanFocus = false;
            emptyCellType2.EnableSubEditor = false;
            emptyCellType2.ReadOnly = true;
            emptyCellType2.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(3).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "部署名";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(4).CanFocus = false;
            currencyCellType1.CurrencySymbol = "\\";
            currencyCellType1.DecimalPlaces = 0;
            currencyCellType1.EnableSubEditor = false;
            currencyCellType1.MaximumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            currencyCellType1.MinimumValue = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            currencyCellType1.ReadOnly = true;
            currencyCellType1.Separator = ",";
            currencyCellType1.ShowSeparator = true;
            currencyCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = currencyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(4).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "取込入金額";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 90F;
            this.fpSpread1_Sheet1.Columns.Get(5).CanFocus = false;
            currencyCellType2.CurrencySymbol = "\\";
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
            currencyCellType2.ReadOnly = true;
            currencyCellType2.Separator = ",";
            currencyCellType2.ShowSeparator = true;
            currencyCellType2.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = currencyCellType2;
            this.fpSpread1_Sheet1.Columns.Get(5).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "手数料";
            this.fpSpread1_Sheet1.Columns.Get(6).CanFocus = true;
            this.fpSpread1_Sheet1.Columns.Get(6).CellType = checkBoxCellType1;
            this.fpSpread1_Sheet1.Columns.Get(6).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(6).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "負担";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 40F;
            this.fpSpread1_Sheet1.Columns.Get(7).CanFocus = false;
            currencyCellType3.CurrencySymbol = "\\";
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
            currencyCellType3.ReadOnly = true;
            currencyCellType3.Separator = ",";
            currencyCellType3.ShowSeparator = true;
            currencyCellType3.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(7).CellType = currencyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(7).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "前回請求額";
            this.fpSpread1_Sheet1.Columns.Get(7).Width = 90F;
            this.fpSpread1_Sheet1.Columns.Get(8).CanFocus = false;
            currencyCellType4.CurrencySymbol = "\\";
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
            currencyCellType4.ReadOnly = true;
            currencyCellType4.Separator = ",";
            currencyCellType4.ShowSeparator = true;
            currencyCellType4.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(8).CellType = currencyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(8).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(8).Label = "今回請求額";
            this.fpSpread1_Sheet1.Columns.Get(8).Width = 91F;
            this.fpSpread1_Sheet1.Columns.Get(9).CanFocus = false;
            currencyCellType5.CurrencySymbol = "\\";
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
            currencyCellType5.ReadOnly = true;
            currencyCellType5.Separator = ",";
            currencyCellType5.ShowSeparator = true;
            currencyCellType5.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(9).CellType = currencyCellType5;
            this.fpSpread1_Sheet1.Columns.Get(9).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(9).Label = "請求後入金";
            this.fpSpread1_Sheet1.Columns.Get(9).Width = 90F;
            this.fpSpread1_Sheet1.Columns.Get(10).CanFocus = false;
            currencyCellType6.CurrencySymbol = "\\";
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
            currencyCellType6.ReadOnly = true;
            currencyCellType6.Separator = ",";
            currencyCellType6.ShowSeparator = true;
            currencyCellType6.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(10).CellType = currencyCellType6;
            this.fpSpread1_Sheet1.Columns.Get(10).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(10).Label = "売掛残";
            this.fpSpread1_Sheet1.Columns.Get(10).Width = 90F;
            this.fpSpread1_Sheet1.Columns.Get(11).CanFocus = false;
            generalCellType1.EnableSubEditor = false;
            generalCellType1.FormatString = "MM/dd";
            generalCellType1.IsDateFormat = true;
            generalCellType1.ReadOnly = true;
            generalCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(11).CellType = generalCellType1;
            this.fpSpread1_Sheet1.Columns.Get(11).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(11).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(11).Label = "入金日";
            this.fpSpread1_Sheet1.Columns.Get(12).CanFocus = false;
            emptyCellType3.ReadOnly = true;
            emptyCellType3.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(12).CellType = emptyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(12).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(12).Label = "取引名";
            this.fpSpread1_Sheet1.Columns.Get(12).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(13).CanFocus = true;
            this.fpSpread1_Sheet1.Columns.Get(13).CellType = checkBoxCellType2;
            this.fpSpread1_Sheet1.Columns.Get(13).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(13).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(13).Label = "無効";
            this.fpSpread1_Sheet1.Columns.Get(13).Width = 40F;
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
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCount.Location = new System.Drawing.Point(12, 345);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(65, 15);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "lblCount";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.CausesValidation = false;
            this.btnSave.Location = new System.Drawing.Point(977, 340);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 25);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(1086, 340);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 8;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.anserApiImportCriteria;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(263, 345);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "取込入金額合計";
            // 
            // lblNyukinTotal
            // 
            this.lblNyukinTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNyukinTotal.AutoEllipsis = true;
            this.lblNyukinTotal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNyukinTotal.Location = new System.Drawing.Point(381, 341);
            this.lblNyukinTotal.Name = "lblNyukinTotal";
            this.lblNyukinTotal.Size = new System.Drawing.Size(91, 22);
            this.lblNyukinTotal.TabIndex = 5;
            this.lblNyukinTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(478, 345);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "円";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "api";
            this.openFileDialog1.Filter = "銀行API ファイル(*.api)|*.api|すべてのファイル(*.*)|*.*";
            this.openFileDialog1.Title = "ファイルを開く";
            // 
            // anserApiContainerBindingSource
            // 
            this.anserApiContainerBindingSource.DataSource = typeof(Emu.Bento.Data.AnserApiContainer);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtNyukinBiko);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cboTekiyo);
            this.groupBox2.Location = new System.Drawing.Point(585, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(411, 50);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "入金明細登録内容";
            // 
            // txtNyukinBiko
            // 
            this.txtNyukinBiko.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.anserApiContainerBindingSource, "金融機関名", true));
            this.txtNyukinBiko.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtNyukinBiko.Location = new System.Drawing.Point(231, 21);
            this.txtNyukinBiko.MaxLength = 20;
            this.txtNyukinBiko.Name = "txtNyukinBiko";
            this.txtNyukinBiko.Size = new System.Drawing.Size(169, 22);
            this.txtNyukinBiko.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "備考";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "摘要";
            // 
            // cboTekiyo
            // 
            this.cboTekiyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboTekiyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboTekiyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboTekiyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.anserApiContainerBindingSource, "入金摘要区分", true));
            this.cboTekiyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTekiyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTekiyo.FormattingEnabled = true;
            this.cboTekiyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboTekiyo.Location = new System.Drawing.Point(49, 21);
            this.cboTekiyo.Name = "cboTekiyo";
            this.cboTekiyo.Size = new System.Drawing.Size(121, 23);
            this.cboTekiyo.TabIndex = 1;
            // 
            // Form604
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1201, 377);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblNyukinTotal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form604";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "銀行APIファイル取込";
            this.Load += new System.EventHandler(this.Form604_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form604_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anserApiImportCriteria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.anserApiContainerBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnImportFile;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource anserApiImportCriteria;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblNyukinTotal;
        private Common.Control.TextBoxA txtFile;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.BindingSource anserApiContainerBindingSource;
        private System.Windows.Forms.GroupBox groupBox2;
        private Common.Control.TextBoxA txtNyukinBiko;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private Common.Control.ComboBoxA cboTekiyo;
    }
}
namespace Emu.Bento
{
    partial class Form401
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
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType1 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.ButtonCellType buttonCellType1 = new FarPoint.Win.Spread.CellType.ButtonCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType1 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType1 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType2 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType2 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.CurrencyCellType currencyCellType3 = new FarPoint.Win.Spread.CellType.CurrencyCellType();
            FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType1 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType2 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            FarPoint.Win.Spread.CellType.ComboBoxCellType comboBoxCellType3 = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
            FarPoint.Win.Spread.CellType.ButtonCellType buttonCellType2 = new FarPoint.Win.Spread.CellType.ButtonCellType();
            this.uriageHeaderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.uriageMeisaiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.uriageMeisaiCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblCreateDate = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblCreateUserName = new System.Windows.Forms.Label();
            this.lblCreateUserCD = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblUpdateUserName = new System.Windows.Forms.Label();
            this.lblUpdateUserCD = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblKakeuri = new System.Windows.Forms.Label();
            this.lblGenkin = new System.Windows.Forms.Label();
            this.lblHikazei = new System.Windows.Forms.Label();
            this.lblSotozei = new System.Windows.Forms.Label();
            this.cboCourse = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.lblRound = new System.Windows.Forms.Label();
            this.lblFloor = new System.Windows.Forms.Label();
            this.lblCeiling = new System.Windows.Forms.Label();
            this.lblCourseTantosyaName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCourseTantosyaCD = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblShitenName = new System.Windows.Forms.Label();
            this.uriageTokuisakiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.lblShitenCD = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dtpUriagebi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label4 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.btnTokuisakiCD = new System.Windows.Forms.Button();
            this.lblTokuisakiName = new System.Windows.Forms.Label();
            this.txtTokuisakiCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTantosyaCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.lblTantosyaName = new System.Windows.Forms.Label();
            this.btnTantosyaCD = new System.Windows.Forms.Button();
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboShiharaiHohoDummy = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.cboUriageKubunDummy = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.btnClear = new System.Windows.Forms.Button();
            fpSpread1_InputMapWhenFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal = new FarPoint.Win.Spread.InputMap();
            ((System.ComponentModel.ISupportInitialize)(this.uriageHeaderBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uriageMeisaiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uriageMeisaiCriteriaBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uriageTokuisakiBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // uriageHeaderBindingSource
            // 
            this.uriageHeaderBindingSource.DataSource = typeof(Emu.Bento.Data.UriageHeader);
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
            this.fpSpread1.Location = new System.Drawing.Point(12, 213);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(895, 221);
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            this.fpSpread1.TabIndex = 5;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.VerticalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.VerticalScrollBar.Name = "";
            this.fpSpread1.VerticalScrollBar.Renderer = enhancedScrollBarRenderer2;
            this.fpSpread1.VerticalScrollBarMode = FarPoint.Win.VerticalScrollMode.PixelAndRow;
            this.fpSpread1.EditModeOn += new System.EventHandler(this.fpSpread1_EditModeOn);
            this.fpSpread1.EditModeOff += new System.EventHandler(this.fpSpread1_EditModeOff);
            this.fpSpread1.Change += new FarPoint.Win.Spread.ChangeEventHandler(this.fpSpread1_Change);
            this.fpSpread1.ButtonClicked += new FarPoint.Win.Spread.EditorNotifyEventHandler(this.fpSpread1_ButtonClicked);
            this.fpSpread1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fpSpread1_KeyDown);
            fpSpread1_InputMapWhenFocusedNormal.Parent = new FarPoint.Win.Spread.InputMap();
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
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent = new FarPoint.Win.Spread.InputMap();
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
            this.fpSpread1_Sheet1.ColumnCount = 11;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.Parent = "ColumnFooterEnhanced";
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.Parent = "CornerEnhanced";
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "商品CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "参照";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "商品名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "単価";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "個数";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "金額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "値引";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "値引事由";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 8).Value = "支払区分";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 9).Value = "食事区分";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 10).Value = "削除";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.Columns.Default.NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Default.NoteIndicatorSize = new System.Drawing.Size(8, 8);
            numberCellType1.DecimalPlaces = 0;
            numberCellType1.EnableSubEditor = false;
            numberCellType1.MaximumValue = 999999999D;
            numberCellType1.MinimumValue = 0D;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = numberCellType1;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(0).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "商品CD";
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(1).CanFocus = true;
            buttonCellType1.ButtonColor2 = System.Drawing.SystemColors.ButtonFace;
            buttonCellType1.Text = "参照";
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = buttonCellType1;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(1).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "参照";
            this.fpSpread1_Sheet1.Columns.Get(2).CanFocus = false;
            emptyCellType1.ReadOnly = true;
            emptyCellType1.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = emptyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "商品名";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(3).CanFocus = true;
            currencyCellType1.CurrencySymbol = "\\";
            currencyCellType1.DecimalPlaces = 0;
            currencyCellType1.EnableSubEditor = false;
            currencyCellType1.MaximumValue = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            currencyCellType1.MinimumValue = new decimal(new int[] {
            9999999,
            0,
            0,
            -2147483648});
            currencyCellType1.Separator = ",";
            currencyCellType1.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = currencyCellType1;
            this.fpSpread1_Sheet1.Columns.Get(3).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "単価";
            numberCellType2.DecimalPlaces = 0;
            numberCellType2.EnableSubEditor = false;
            numberCellType2.MaximumValue = 99999D;
            numberCellType2.MinimumValue = -99999D;
            numberCellType2.Separator = ",";
            numberCellType2.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = numberCellType2;
            this.fpSpread1_Sheet1.Columns.Get(4).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "個数";
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
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "金額";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 80F;
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
            currencyCellType3.Separator = ",";
            currencyCellType3.ShowSeparator = true;
            this.fpSpread1_Sheet1.Columns.Get(6).CellType = currencyCellType3;
            this.fpSpread1_Sheet1.Columns.Get(6).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "値引";
            this.fpSpread1_Sheet1.Columns.Get(6).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(6).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 80F;
            comboBoxCellType1.AllowEditorVerticalAlign = true;
            comboBoxCellType1.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
            this.fpSpread1_Sheet1.Columns.Get(7).CellType = comboBoxCellType1;
            this.fpSpread1_Sheet1.Columns.Get(7).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "値引事由";
            this.fpSpread1_Sheet1.Columns.Get(7).Width = 130F;
            comboBoxCellType2.AllowEditorVerticalAlign = true;
            comboBoxCellType2.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
            this.fpSpread1_Sheet1.Columns.Get(8).CellType = comboBoxCellType2;
            this.fpSpread1_Sheet1.Columns.Get(8).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(8).Label = "支払区分";
            this.fpSpread1_Sheet1.Columns.Get(8).Width = 90F;
            comboBoxCellType3.AllowEditorVerticalAlign = true;
            comboBoxCellType3.ButtonAlign = FarPoint.Win.ButtonAlign.Right;
            this.fpSpread1_Sheet1.Columns.Get(9).CellType = comboBoxCellType3;
            this.fpSpread1_Sheet1.Columns.Get(9).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(9).Label = "食事区分";
            this.fpSpread1_Sheet1.Columns.Get(9).Width = 80F;
            buttonCellType2.ButtonColor2 = System.Drawing.SystemColors.ButtonFace;
            buttonCellType2.Text = "削除";
            this.fpSpread1_Sheet1.Columns.Get(10).CellType = buttonCellType2;
            this.fpSpread1_Sheet1.Columns.Get(10).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(10).Label = "削除";
            this.fpSpread1_Sheet1.DataAllowAddNew = true;
            this.fpSpread1_Sheet1.DataAutoCellTypes = false;
            this.fpSpread1_Sheet1.DataAutoHeadings = false;
            this.fpSpread1_Sheet1.DataAutoSizeColumns = false;
            this.fpSpread1_Sheet1.DataSource = this.uriageMeisaiBindingSource;
            this.fpSpread1_Sheet1.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.DefaultStyle.NoteIndicatorSize = new System.Drawing.Size(8, 8);
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
            this.fpSpread1_Sheet1.Rows.Default.NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Rows.Default.NoteIndicatorSize = new System.Drawing.Size(8, 8);
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
            this.btnQuit.Location = new System.Drawing.Point(804, 509);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 20;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.uriageMeisaiCriteriaBindingSource;
            // 
            // uriageMeisaiCriteriaBindingSource
            // 
            this.uriageMeisaiCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.UriageMeisaiCriteria);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.AutoSize = true;
            this.btnDelete.CausesValidation = false;
            this.btnDelete.Location = new System.Drawing.Point(695, 509);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(103, 25);
            this.btnDelete.TabIndex = 19;
            this.btnDelete.Text = "削除(F11)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.Location = new System.Drawing.Point(586, 509);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 25);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "登録(F9)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblCreateDate
            // 
            this.lblCreateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateDate.AutoEllipsis = true;
            this.lblCreateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "作成日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblCreateDate.Location = new System.Drawing.Point(102, 474);
            this.lblCreateDate.Name = "lblCreateDate";
            this.lblCreateDate.Size = new System.Drawing.Size(180, 22);
            this.lblCreateDate.TabIndex = 10;
            this.lblCreateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(12, 478);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 15);
            this.label16.TabIndex = 9;
            this.label16.Text = "作成日時";
            // 
            // lblCreateUserName
            // 
            this.lblCreateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserName.AutoEllipsis = true;
            this.lblCreateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "作成担当者名", true));
            this.lblCreateUserName.Location = new System.Drawing.Point(183, 446);
            this.lblCreateUserName.Name = "lblCreateUserName";
            this.lblCreateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblCreateUserName.TabIndex = 8;
            this.lblCreateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCreateUserCD
            // 
            this.lblCreateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreateUserCD.AutoEllipsis = true;
            this.lblCreateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCreateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "作成担当者ＣＤ", true));
            this.lblCreateUserCD.Location = new System.Drawing.Point(102, 446);
            this.lblCreateUserCD.Name = "lblCreateUserCD";
            this.lblCreateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblCreateUserCD.TabIndex = 7;
            this.lblCreateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(12, 450);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 15);
            this.label19.TabIndex = 6;
            this.label19.Text = "作成担当者";
            // 
            // lblUpdateDate
            // 
            this.lblUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(433, 474);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 15;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(343, 478);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 14;
            this.label13.Text = "修正日時";
            // 
            // lblUpdateUserName
            // 
            this.lblUpdateUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserName.AutoEllipsis = true;
            this.lblUpdateUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "修正担当者名", true));
            this.lblUpdateUserName.Location = new System.Drawing.Point(514, 446);
            this.lblUpdateUserName.Name = "lblUpdateUserName";
            this.lblUpdateUserName.Size = new System.Drawing.Size(140, 22);
            this.lblUpdateUserName.TabIndex = 13;
            this.lblUpdateUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUpdateUserCD
            // 
            this.lblUpdateUserCD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblUpdateUserCD.AutoEllipsis = true;
            this.lblUpdateUserCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateUserCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "修正担当者ＣＤ", true));
            this.lblUpdateUserCD.Location = new System.Drawing.Point(433, 446);
            this.lblUpdateUserCD.Name = "lblUpdateUserCD";
            this.lblUpdateUserCD.Size = new System.Drawing.Size(75, 22);
            this.lblUpdateUserCD.TabIndex = 12;
            this.lblUpdateUserCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(343, 450);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 15);
            this.label10.TabIndex = 11;
            this.label10.Text = "修正担当者";
            // 
            // lblMode
            // 
            this.lblMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblMode.ForeColor = System.Drawing.Color.Black;
            this.lblMode.Location = new System.Drawing.Point(12, 511);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(82, 21);
            this.lblMode.TabIndex = 16;
            this.lblMode.Text = "新規モード";
            this.lblMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblKakeuri);
            this.groupBox1.Controls.Add(this.lblGenkin);
            this.groupBox1.Controls.Add(this.lblHikazei);
            this.groupBox1.Controls.Add(this.lblSotozei);
            this.groupBox1.Controls.Add(this.cboCourse);
            this.groupBox1.Controls.Add(this.lblRound);
            this.groupBox1.Controls.Add(this.lblFloor);
            this.groupBox1.Controls.Add(this.lblCeiling);
            this.groupBox1.Controls.Add(this.lblCourseTantosyaName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtCourseTantosyaCD);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblShitenName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblShitenCD);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(890, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "売上先";
            // 
            // lblKakeuri
            // 
            this.lblKakeuri.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(242)))), ((int)(((byte)(102)))));
            this.lblKakeuri.ForeColor = System.Drawing.Color.Black;
            this.lblKakeuri.Location = new System.Drawing.Point(462, 136);
            this.lblKakeuri.Name = "lblKakeuri";
            this.lblKakeuri.Size = new System.Drawing.Size(70, 21);
            this.lblKakeuri.TabIndex = 6;
            this.lblKakeuri.Text = "掛売";
            this.lblKakeuri.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGenkin
            // 
            this.lblGenkin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.lblGenkin.ForeColor = System.Drawing.Color.Black;
            this.lblGenkin.Location = new System.Drawing.Point(386, 136);
            this.lblGenkin.Name = "lblGenkin";
            this.lblGenkin.Size = new System.Drawing.Size(70, 21);
            this.lblGenkin.TabIndex = 5;
            this.lblGenkin.Text = "現金";
            this.lblGenkin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHikazei
            // 
            this.lblHikazei.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(135)))), ((int)(((byte)(231)))), ((int)(((byte)(176)))));
            this.lblHikazei.ForeColor = System.Drawing.Color.Black;
            this.lblHikazei.Location = new System.Drawing.Point(82, 136);
            this.lblHikazei.Name = "lblHikazei";
            this.lblHikazei.Size = new System.Drawing.Size(70, 21);
            this.lblHikazei.TabIndex = 1;
            this.lblHikazei.Text = "非課税";
            this.lblHikazei.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSotozei
            // 
            this.lblSotozei.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(153)))));
            this.lblSotozei.ForeColor = System.Drawing.Color.Black;
            this.lblSotozei.Location = new System.Drawing.Point(6, 136);
            this.lblSotozei.Name = "lblSotozei";
            this.lblSotozei.Size = new System.Drawing.Size(70, 21);
            this.lblSotozei.TabIndex = 0;
            this.lblSotozei.Text = "外税";
            this.lblSotozei.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboCourse
            // 
            this.cboCourse.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCourse.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCourse.BackColor = System.Drawing.SystemColors.Control;
            this.cboCourse.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.uriageHeaderBindingSource, "コースＣＤ", true));
            this.cboCourse.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCourse.FormattingEnabled = true;
            this.cboCourse.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboCourse.Location = new System.Drawing.Point(522, 47);
            this.cboCourse.Name = "cboCourse";
            this.cboCourse.Size = new System.Drawing.Size(273, 23);
            this.cboCourse.TabIndex = 11;
            this.cboCourse.SelectedIndexChanged += new System.EventHandler(this.cboCourse_SelectedIndexChanged);
            this.cboCourse.SelectedValueChanged += new System.EventHandler(this.cboCourse_SelectedValueChanged);
            // 
            // lblRound
            // 
            this.lblRound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(178)))), ((int)(((byte)(222)))));
            this.lblRound.ForeColor = System.Drawing.Color.Black;
            this.lblRound.Location = new System.Drawing.Point(310, 136);
            this.lblRound.Name = "lblRound";
            this.lblRound.Size = new System.Drawing.Size(70, 21);
            this.lblRound.TabIndex = 4;
            this.lblRound.Text = "四捨五入";
            this.lblRound.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFloor
            // 
            this.lblFloor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(197)))), ((int)(((byte)(143)))));
            this.lblFloor.ForeColor = System.Drawing.Color.Black;
            this.lblFloor.Location = new System.Drawing.Point(234, 136);
            this.lblFloor.Name = "lblFloor";
            this.lblFloor.Size = new System.Drawing.Size(70, 21);
            this.lblFloor.TabIndex = 3;
            this.lblFloor.Text = "切り捨て";
            this.lblFloor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCeiling
            // 
            this.lblCeiling.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(235)))), ((int)(((byte)(250)))));
            this.lblCeiling.ForeColor = System.Drawing.Color.Black;
            this.lblCeiling.Location = new System.Drawing.Point(158, 136);
            this.lblCeiling.Name = "lblCeiling";
            this.lblCeiling.Size = new System.Drawing.Size(70, 21);
            this.lblCeiling.TabIndex = 2;
            this.lblCeiling.Text = "切り上げ";
            this.lblCeiling.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCourseTantosyaName
            // 
            this.lblCourseTantosyaName.AutoEllipsis = true;
            this.lblCourseTantosyaName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseTantosyaName.Location = new System.Drawing.Point(611, 76);
            this.lblCourseTantosyaName.Name = "lblCourseTantosyaName";
            this.lblCourseTantosyaName.Size = new System.Drawing.Size(184, 22);
            this.lblCourseTantosyaName.TabIndex = 14;
            this.lblCourseTantosyaName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(449, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "担当者";
            // 
            // txtCourseTantosyaCD
            // 
            this.txtCourseTantosyaCD.AutoEllipsis = true;
            this.txtCourseTantosyaCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.txtCourseTantosyaCD.Location = new System.Drawing.Point(522, 76);
            this.txtCourseTantosyaCD.Name = "txtCourseTantosyaCD";
            this.txtCourseTantosyaCD.Size = new System.Drawing.Size(83, 22);
            this.txtCourseTantosyaCD.TabIndex = 13;
            this.txtCourseTantosyaCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(449, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "コース";
            // 
            // lblShitenName
            // 
            this.lblShitenName.AutoEllipsis = true;
            this.lblShitenName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblShitenName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageTokuisakiBindingSource, "支店名", true));
            this.lblShitenName.Location = new System.Drawing.Point(611, 18);
            this.lblShitenName.Name = "lblShitenName";
            this.lblShitenName.Size = new System.Drawing.Size(184, 22);
            this.lblShitenName.TabIndex = 9;
            this.lblShitenName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uriageTokuisakiBindingSource
            // 
            this.uriageTokuisakiBindingSource.DataSource = typeof(Emu.Bento.Data.UriageTokuisaki);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(449, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "支店";
            // 
            // lblShitenCD
            // 
            this.lblShitenCD.AutoEllipsis = true;
            this.lblShitenCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblShitenCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageTokuisakiBindingSource, "支店ＣＤ", true));
            this.lblShitenCD.Location = new System.Drawing.Point(522, 19);
            this.lblShitenCD.Name = "lblShitenCD";
            this.lblShitenCD.Size = new System.Drawing.Size(83, 22);
            this.lblShitenCD.TabIndex = 8;
            this.lblShitenCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dtpUriagebi);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cboBusyo);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.btnTokuisakiCD);
            this.groupBox2.Controls.Add(this.lblTokuisakiName);
            this.groupBox2.Controls.Add(this.txtTokuisakiCD);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnQuery);
            this.groupBox2.Location = new System.Drawing.Point(9, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(423, 100);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // dtpUriagebi
            // 
            this.dtpUriagebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageMeisaiCriteriaBindingSource, "売上日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpUriagebi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpUriagebi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpUriagebi.Location = new System.Drawing.Point(84, 72);
            this.dtpUriagebi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpUriagebi.Name = "dtpUriagebi";
            this.dtpUriagebi.Size = new System.Drawing.Size(152, 22);
            this.dtpUriagebi.TabIndex = 7;
            this.dtpUriagebi.Validated += new System.EventHandler(this.dtpUriagebi_Validated);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.uriageMeisaiCriteriaBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(84, 43);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 5;
            this.cboBusyo.Validated += new System.EventHandler(this.cboBusyo_Validated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 75);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 6;
            this.label9.Text = "売上日";
            // 
            // btnTokuisakiCD
            // 
            this.btnTokuisakiCD.AutoSize = true;
            this.btnTokuisakiCD.Location = new System.Drawing.Point(370, 14);
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
            this.lblTokuisakiName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageMeisaiCriteriaBindingSource, "得意先名", true));
            this.lblTokuisakiName.Location = new System.Drawing.Point(173, 15);
            this.lblTokuisakiName.Name = "lblTokuisakiName";
            this.lblTokuisakiName.Size = new System.Drawing.Size(191, 22);
            this.lblTokuisakiName.TabIndex = 2;
            this.lblTokuisakiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCD
            // 
            this.txtTokuisakiCD.CopyFromWhenEmpty = null;
            this.txtTokuisakiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageMeisaiCriteriaBindingSource, "得意先ＣＤ", true));
            this.txtTokuisakiCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCD.Location = new System.Drawing.Point(84, 15);
            this.txtTokuisakiCD.MaxLength = 9;
            this.txtTokuisakiCD.Name = "txtTokuisakiCD";
            this.txtTokuisakiCD.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCD.TabIndex = 1;
            this.txtTokuisakiCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCD_KeyDown);
            this.txtTokuisakiCD.Validated += new System.EventHandler(this.txtTokuisakiCD_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "得意先CD";
            // 
            // btnQuery
            // 
            this.btnQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(344, 69);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 8;
            this.btnQuery.Text = "表示(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "担当者CD";
            // 
            // txtTantosyaCD
            // 
            this.txtTantosyaCD.CopyFromWhenEmpty = null;
            this.txtTantosyaCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "配送担当者ＣＤ", true));
            this.txtTantosyaCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTantosyaCD.Location = new System.Drawing.Point(90, 184);
            this.txtTantosyaCD.MaxLength = 9;
            this.txtTantosyaCD.Name = "txtTantosyaCD";
            this.txtTantosyaCD.Size = new System.Drawing.Size(83, 22);
            this.txtTantosyaCD.TabIndex = 2;
            this.txtTantosyaCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTantosyaCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTantosyaCD_KeyDown);
            // 
            // lblTantosyaName
            // 
            this.lblTantosyaName.AutoEllipsis = true;
            this.lblTantosyaName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTantosyaName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uriageHeaderBindingSource, "配送担当者名", true));
            this.lblTantosyaName.Location = new System.Drawing.Point(179, 184);
            this.lblTantosyaName.Name = "lblTantosyaName";
            this.lblTantosyaName.Size = new System.Drawing.Size(131, 22);
            this.lblTantosyaName.TabIndex = 3;
            this.lblTantosyaName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnTantosyaCD
            // 
            this.btnTantosyaCD.AutoSize = true;
            this.btnTantosyaCD.Location = new System.Drawing.Point(316, 182);
            this.btnTantosyaCD.Name = "btnTantosyaCD";
            this.btnTantosyaCD.Size = new System.Drawing.Size(47, 25);
            this.btnTantosyaCD.TabIndex = 4;
            this.btnTantosyaCD.TabStop = false;
            this.btnTantosyaCD.Text = "参照";
            this.btnTantosyaCD.UseVisualStyleBackColor = true;
            this.btnTantosyaCD.Click += new System.EventHandler(this.btnTantosyaCD_Click);
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            this.errorProvider2.DataSource = this.uriageHeaderBindingSource;
            // 
            // cboShiharaiHohoDummy
            // 
            this.cboShiharaiHohoDummy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboShiharaiHohoDummy.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboShiharaiHohoDummy.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboShiharaiHohoDummy.BackColor = System.Drawing.SystemColors.Control;
            this.cboShiharaiHohoDummy.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.uriageHeaderBindingSource, "コースＣＤ", true));
            this.cboShiharaiHohoDummy.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboShiharaiHohoDummy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShiharaiHohoDummy.FormattingEnabled = true;
            this.cboShiharaiHohoDummy.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboShiharaiHohoDummy.Location = new System.Drawing.Point(218, 509);
            this.cboShiharaiHohoDummy.Name = "cboShiharaiHohoDummy";
            this.cboShiharaiHohoDummy.Size = new System.Drawing.Size(55, 23);
            this.cboShiharaiHohoDummy.TabIndex = 25;
            this.cboShiharaiHohoDummy.Visible = false;
            // 
            // cboUriageKubunDummy
            // 
            this.cboUriageKubunDummy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboUriageKubunDummy.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboUriageKubunDummy.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboUriageKubunDummy.BackColor = System.Drawing.SystemColors.Control;
            this.cboUriageKubunDummy.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.uriageHeaderBindingSource, "コースＣＤ", true));
            this.cboUriageKubunDummy.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboUriageKubunDummy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUriageKubunDummy.FormattingEnabled = true;
            this.cboUriageKubunDummy.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboUriageKubunDummy.Location = new System.Drawing.Point(279, 509);
            this.cboUriageKubunDummy.Name = "cboUriageKubunDummy";
            this.cboUriageKubunDummy.Size = new System.Drawing.Size(55, 23);
            this.cboUriageKubunDummy.TabIndex = 26;
            this.cboUriageKubunDummy.Visible = false;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.Location = new System.Drawing.Point(100, 509);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(89, 25);
            this.btnClear.TabIndex = 17;
            this.btnClear.Text = "クリア(F3)";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Form401
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 546);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.cboUriageKubunDummy);
            this.Controls.Add(this.cboShiharaiHohoDummy);
            this.Controls.Add(this.btnTantosyaCD);
            this.Controls.Add(this.lblTantosyaName);
            this.Controls.Add(this.txtTantosyaCD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.lblCreateDate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lblCreateUserName);
            this.Controls.Add(this.lblCreateUserCD);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.lblUpdateDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblUpdateUserName);
            this.Controls.Add(this.lblUpdateUserCD);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.fpSpread1);
            this.KeyPreview = true;
            this.Name = "Form401";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "売上入力";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form401_FormClosing);
            this.Load += new System.EventHandler(this.Form401_Load);
            this.Shown += new System.EventHandler(this.Form401_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form401_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.uriageHeaderBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uriageMeisaiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uriageMeisaiCriteriaBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uriageTokuisakiBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource uriageMeisaiBindingSource;
        private System.Windows.Forms.BindingSource uriageHeaderBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblCreateDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblCreateUserName;
        private System.Windows.Forms.Label lblCreateUserCD;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateUserName;
        private System.Windows.Forms.Label lblUpdateUserCD;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTantosyaCD;
        private System.Windows.Forms.Label lblTantosyaName;
        private Common.Control.TextBoxAN txtTantosyaCD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCourseTantosyaName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label txtCourseTantosyaCD;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblShitenName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblShitenCD;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnTokuisakiCD;
        private System.Windows.Forms.Label lblTokuisakiName;
        private Common.Control.TextBoxAN txtTokuisakiCD;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Label lblRound;
        private System.Windows.Forms.Label lblFloor;
        private System.Windows.Forms.Label lblCeiling;
        private System.Windows.Forms.BindingSource uriageMeisaiCriteriaBindingSource;
        private System.Windows.Forms.BindingSource uriageTokuisakiBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        private Common.Control.ComboBoxA cboCourse;
        private System.Windows.Forms.Label lblHikazei;
        private System.Windows.Forms.Label lblSotozei;
        private System.Windows.Forms.Label lblKakeuri;
        private System.Windows.Forms.Label lblGenkin;
        private Common.Control.ComboBoxA cboShiharaiHohoDummy;
        private Common.Control.ComboBoxA cboUriageKubunDummy;
        private System.Windows.Forms.Label label4;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpUriagebi;
        private System.Windows.Forms.Button btnClear;
    }
}

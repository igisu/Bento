namespace Emu.Bento
{
    partial class Form210
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
            FarPoint.Win.Spread.CellType.NumberCellType numberCellType4 = new FarPoint.Win.Spread.CellType.NumberCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType4 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.CheckBoxCellType checkBoxCellType4 = new FarPoint.Win.Spread.CellType.CheckBoxCellType();
            this.youbiChumonHanneiCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnExec = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSeisanbiTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpSeisanbiFrom = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.btnCourseCDTo = new System.Windows.Forms.Button();
            this.lblCourseNameTo = new System.Windows.Forms.Label();
            this.txtCourseCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtCourseCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.btnCourseCDFrom = new System.Windows.Forms.Button();
            this.lblCourseNameFrom = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDTo = new System.Windows.Forms.Button();
            this.lblTokuisakiNameTo = new System.Windows.Forms.Label();
            this.txtTokuisakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtTokuisakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDFrom = new System.Windows.Forms.Button();
            this.lblTokuisakiNameFrom = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkContainsWebTokuisaki = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            fpSpread1_InputMapWhenFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenFocusedNormal.Parent = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal = new FarPoint.Win.Spread.InputMap();
            fpSpread1_InputMapWhenAncestorOfFocusedNormal.Parent = new FarPoint.Win.Spread.InputMap();
            ((System.ComponentModel.ISupportInitialize)(this.youbiChumonHanneiCriteriaBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // youbiChumonHanneiCriteriaBindingSource
            // 
            this.youbiChumonHanneiCriteriaBindingSource.AllowNew = false;
            this.youbiChumonHanneiCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.YoubiChumonHanneiCriteria);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(588, 449);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 4;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.Location = new System.Drawing.Point(479, 449);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(103, 25);
            this.btnExec.TabIndex = 3;
            this.btnExec.Text = "処理開始(F9)";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dtpSeisanbiTo);
            this.groupBox1.Controls.Add(this.dtpSeisanbiFrom);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 79);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "注文対象指定";
            // 
            // dtpSeisanbiTo
            // 
            this.dtpSeisanbiTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "適用日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeisanbiTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeisanbiTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeisanbiTo.Location = new System.Drawing.Point(252, 50);
            this.dtpSeisanbiTo.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeisanbiTo.Name = "dtpSeisanbiTo";
            this.dtpSeisanbiTo.Size = new System.Drawing.Size(152, 22);
            this.dtpSeisanbiTo.TabIndex = 5;
            // 
            // dtpSeisanbiFrom
            // 
            this.dtpSeisanbiFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "適用日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeisanbiFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeisanbiFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeisanbiFrom.Location = new System.Drawing.Point(65, 50);
            this.dtpSeisanbiFrom.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeisanbiFrom.Name = "dtpSeisanbiFrom";
            this.dtpSeisanbiFrom.Size = new System.Drawing.Size(152, 22);
            this.dtpSeisanbiFrom.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(224, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(22, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "～";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "適用日";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.youbiChumonHanneiCriteriaBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(65, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "部署";
            // 
            // label8
            // 
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(323, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(350, 36);
            this.label8.TabIndex = 3;
            this.label8.Text = "得意先マスタの休日登録で休日になっている曜日の注文は作成されません。";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Red;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(323, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 36);
            this.label4.TabIndex = 2;
            this.label4.Text = "適用範囲内で対象得意先の注文が全て削除されたあと、曜日別の注文が作成されます。";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnClearAll);
            this.groupBox2.Controls.Add(this.btnSelectAll);
            this.groupBox2.Location = new System.Drawing.Point(323, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(164, 52);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "作成対象";
            // 
            // btnClearAll
            // 
            this.btnClearAll.AutoSize = true;
            this.btnClearAll.Location = new System.Drawing.Point(85, 21);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(73, 25);
            this.btnClearAll.TabIndex = 1;
            this.btnClearAll.Text = "全解除";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.AutoSize = true;
            this.btnSelectAll.Location = new System.Drawing.Point(6, 21);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(73, 25);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "全選択";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // fpSpread1
            // 
            this.fpSpread1.AccessibleDescription = "fpSpread1, Sheet1";
            this.fpSpread1.AllowUserZoom = false;
            this.fpSpread1.ColumnSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.Location = new System.Drawing.Point(6, 21);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(311, 196);
            this.fpSpread1.TabIndex = 0;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
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
            this.fpSpread1_Sheet1.ColumnCount = 3;
            this.fpSpread1_Sheet1.ActiveColumnIndex = -1;
            this.fpSpread1_Sheet1.ActiveRowIndex = -1;
            this.fpSpread1_Sheet1.AutoGenerateColumns = false;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "商品CD";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "商品名";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "対象";
            numberCellType4.DecimalPlaces = 0;
            numberCellType4.EnableSubEditor = false;
            numberCellType4.MaximumValue = 999999999D;
            numberCellType4.MinimumValue = 0D;
            numberCellType4.ReadOnly = true;
            numberCellType4.Static = true;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = numberCellType4;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(0).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "商品CD";
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(0).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 80F;
            this.fpSpread1_Sheet1.Columns.Get(1).CanFocus = false;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType4;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "商品名";
            this.fpSpread1_Sheet1.Columns.Get(1).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
            this.fpSpread1_Sheet1.Columns.Get(1).NoteIndicatorSize = new System.Drawing.Size(8, 8);
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 150F;
            this.fpSpread1_Sheet1.Columns.Get(2).CellType = checkBoxCellType4;
            this.fpSpread1_Sheet1.Columns.Get(2).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(2).ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "対象";
            this.fpSpread1_Sheet1.DataAllowAddNew = true;
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
            // bindingSource1
            // 
            this.bindingSource1.AllowNew = false;
            // 
            // btnCourseCDTo
            // 
            this.btnCourseCDTo.AutoSize = true;
            this.btnCourseCDTo.Location = new System.Drawing.Point(626, 19);
            this.btnCourseCDTo.Name = "btnCourseCDTo";
            this.btnCourseCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDTo.TabIndex = 7;
            this.btnCourseCDTo.TabStop = false;
            this.btnCourseCDTo.Text = "参照";
            this.btnCourseCDTo.UseVisualStyleBackColor = true;
            this.btnCourseCDTo.Click += new System.EventHandler(this.btnCourseCDTo_Click);
            // 
            // lblCourseNameTo
            // 
            this.lblCourseNameTo.AutoEllipsis = true;
            this.lblCourseNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "コース名To", true));
            this.lblCourseNameTo.Location = new System.Drawing.Point(475, 21);
            this.lblCourseNameTo.Name = "lblCourseNameTo";
            this.lblCourseNameTo.Size = new System.Drawing.Size(145, 22);
            this.lblCourseNameTo.TabIndex = 6;
            this.lblCourseNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDTo
            // 
            this.txtCourseCDTo.CopyFromWhenEmpty = this.txtCourseCDFrom;
            this.txtCourseCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "コースＣＤTo", true));
            this.txtCourseCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDTo.Location = new System.Drawing.Point(386, 21);
            this.txtCourseCDTo.MaxLength = 9;
            this.txtCourseCDTo.Name = "txtCourseCDTo";
            this.txtCourseCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDTo.TabIndex = 5;
            this.txtCourseCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDTo_KeyDown);
            // 
            // txtCourseCDFrom
            // 
            this.txtCourseCDFrom.CopyFromWhenEmpty = null;
            this.txtCourseCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "コースＣＤFrom", true));
            this.txtCourseCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDFrom.Location = new System.Drawing.Point(65, 21);
            this.txtCourseCDFrom.MaxLength = 9;
            this.txtCourseCDFrom.Name = "txtCourseCDFrom";
            this.txtCourseCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDFrom.TabIndex = 1;
            this.txtCourseCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDFrom_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(358, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "～";
            // 
            // btnCourseCDFrom
            // 
            this.btnCourseCDFrom.AutoSize = true;
            this.btnCourseCDFrom.Location = new System.Drawing.Point(305, 19);
            this.btnCourseCDFrom.Name = "btnCourseCDFrom";
            this.btnCourseCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDFrom.TabIndex = 3;
            this.btnCourseCDFrom.TabStop = false;
            this.btnCourseCDFrom.Text = "参照";
            this.btnCourseCDFrom.UseVisualStyleBackColor = true;
            this.btnCourseCDFrom.Click += new System.EventHandler(this.btnCourseCDFrom_Click);
            // 
            // lblCourseNameFrom
            // 
            this.lblCourseNameFrom.AutoEllipsis = true;
            this.lblCourseNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "コース名From", true));
            this.lblCourseNameFrom.Location = new System.Drawing.Point(154, 21);
            this.lblCourseNameFrom.Name = "lblCourseNameFrom";
            this.lblCourseNameFrom.Size = new System.Drawing.Size(145, 22);
            this.lblCourseNameFrom.TabIndex = 2;
            this.lblCourseNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "コース";
            // 
            // btnTokuisakiCDTo
            // 
            this.btnTokuisakiCDTo.AutoSize = true;
            this.btnTokuisakiCDTo.Location = new System.Drawing.Point(626, 49);
            this.btnTokuisakiCDTo.Name = "btnTokuisakiCDTo";
            this.btnTokuisakiCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDTo.TabIndex = 15;
            this.btnTokuisakiCDTo.TabStop = false;
            this.btnTokuisakiCDTo.Text = "参照";
            this.btnTokuisakiCDTo.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDTo.Click += new System.EventHandler(this.btnTokuisakiCDTo_Click);
            // 
            // lblTokuisakiNameTo
            // 
            this.lblTokuisakiNameTo.AutoEllipsis = true;
            this.lblTokuisakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "得意先名To", true));
            this.lblTokuisakiNameTo.Location = new System.Drawing.Point(475, 51);
            this.lblTokuisakiNameTo.Name = "lblTokuisakiNameTo";
            this.lblTokuisakiNameTo.Size = new System.Drawing.Size(145, 22);
            this.lblTokuisakiNameTo.TabIndex = 14;
            this.lblTokuisakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDTo
            // 
            this.txtTokuisakiCDTo.CopyFromWhenEmpty = this.txtTokuisakiCDFrom;
            this.txtTokuisakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "得意先ＣＤTo", true));
            this.txtTokuisakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDTo.Location = new System.Drawing.Point(386, 51);
            this.txtTokuisakiCDTo.MaxLength = 9;
            this.txtTokuisakiCDTo.Name = "txtTokuisakiCDTo";
            this.txtTokuisakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDTo.TabIndex = 13;
            this.txtTokuisakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDTo_KeyDown);
            // 
            // txtTokuisakiCDFrom
            // 
            this.txtTokuisakiCDFrom.CopyFromWhenEmpty = null;
            this.txtTokuisakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "得意先ＣＤFrom", true));
            this.txtTokuisakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDFrom.Location = new System.Drawing.Point(65, 51);
            this.txtTokuisakiCDFrom.MaxLength = 9;
            this.txtTokuisakiCDFrom.Name = "txtTokuisakiCDFrom";
            this.txtTokuisakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDFrom.TabIndex = 9;
            this.txtTokuisakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDFrom_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(358, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 4;
            this.label12.Text = "～";
            // 
            // btnTokuisakiCDFrom
            // 
            this.btnTokuisakiCDFrom.AutoSize = true;
            this.btnTokuisakiCDFrom.Location = new System.Drawing.Point(305, 49);
            this.btnTokuisakiCDFrom.Name = "btnTokuisakiCDFrom";
            this.btnTokuisakiCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDFrom.TabIndex = 11;
            this.btnTokuisakiCDFrom.TabStop = false;
            this.btnTokuisakiCDFrom.Text = "参照";
            this.btnTokuisakiCDFrom.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDFrom.Click += new System.EventHandler(this.btnTokuisakiCDFrom_Click);
            // 
            // lblTokuisakiNameFrom
            // 
            this.lblTokuisakiNameFrom.AutoEllipsis = true;
            this.lblTokuisakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.youbiChumonHanneiCriteriaBindingSource, "得意先名From", true));
            this.lblTokuisakiNameFrom.Location = new System.Drawing.Point(154, 51);
            this.lblTokuisakiNameFrom.Name = "lblTokuisakiNameFrom";
            this.lblTokuisakiNameFrom.Size = new System.Drawing.Size(145, 22);
            this.lblTokuisakiNameFrom.TabIndex = 10;
            this.lblTokuisakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 54);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 8;
            this.label14.Text = "得意先";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.youbiChumonHanneiCriteriaBindingSource;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.fpSpread1);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Location = new System.Drawing.Point(12, 220);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(679, 223);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "作成対象商品";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkContainsWebTokuisaki);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.txtTokuisakiCDFrom);
            this.groupBox3.Controls.Add(this.lblTokuisakiNameFrom);
            this.groupBox3.Controls.Add(this.btnTokuisakiCDFrom);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.btnCourseCDTo);
            this.groupBox3.Controls.Add(this.txtTokuisakiCDTo);
            this.groupBox3.Controls.Add(this.lblCourseNameTo);
            this.groupBox3.Controls.Add(this.lblTokuisakiNameTo);
            this.groupBox3.Controls.Add(this.txtCourseCDTo);
            this.groupBox3.Controls.Add(this.btnTokuisakiCDTo);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.btnCourseCDFrom);
            this.groupBox3.Controls.Add(this.txtCourseCDFrom);
            this.groupBox3.Controls.Add(this.lblCourseNameFrom);
            this.groupBox3.Location = new System.Drawing.Point(12, 97);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(679, 117);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "対象得意先";
            // 
            // chkContainsWebTokuisaki
            // 
            this.chkContainsWebTokuisaki.AutoSize = true;
            this.chkContainsWebTokuisaki.Location = new System.Drawing.Point(15, 85);
            this.chkContainsWebTokuisaki.Name = "chkContainsWebTokuisaki";
            this.chkContainsWebTokuisaki.Size = new System.Drawing.Size(188, 19);
            this.chkContainsWebTokuisaki.TabIndex = 19;
            this.chkContainsWebTokuisaki.Text = "Web得意先の注文を含める";
            this.chkContainsWebTokuisaki.UseVisualStyleBackColor = true;
            // 
            // Form210
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 486);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form210";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "曜日別固定数注文反映";
            this.Load += new System.EventHandler(this.Form210_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form210_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.youbiChumonHanneiCriteriaBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource youbiChumonHanneiCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Button btnTokuisakiCDTo;
        private System.Windows.Forms.Label lblTokuisakiNameTo;
        private Common.Control.TextBoxAN txtTokuisakiCDTo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnTokuisakiCDFrom;
        private System.Windows.Forms.Label lblTokuisakiNameFrom;
        private Common.Control.TextBoxAN txtTokuisakiCDFrom;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private Common.Control.DateTimePickerA dtpSeisanbiTo;
        private Common.Control.DateTimePickerA dtpSeisanbiFrom;
        private System.Windows.Forms.Button btnCourseCDTo;
        private System.Windows.Forms.Label lblCourseNameTo;
        private Common.Control.TextBoxAN txtCourseCDTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCourseCDFrom;
        private System.Windows.Forms.Label lblCourseNameFrom;
        private Common.Control.TextBoxAN txtCourseCDFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private Common.Control.CheckBoxA chkContainsWebTokuisaki;
    }
}

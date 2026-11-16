namespace Emu.Bento
{
    partial class Form513D
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
            FarPoint.Win.Spread.EnhancedFocusIndicatorRenderer enhancedFocusIndicatorRenderer2 = new FarPoint.Win.Spread.EnhancedFocusIndicatorRenderer();
            FarPoint.Win.Spread.EnhancedScrollBarRenderer enhancedScrollBarRenderer3 = new FarPoint.Win.Spread.EnhancedScrollBarRenderer();
            FarPoint.Win.Spread.EnhancedScrollBarRenderer enhancedScrollBarRenderer4 = new FarPoint.Win.Spread.EnhancedScrollBarRenderer();
            FarPoint.Win.Spread.CellType.GeneralCellType generalCellType2 = new FarPoint.Win.Spread.CellType.GeneralCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType6 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType7 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType8 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType9 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            FarPoint.Win.Spread.CellType.EmptyCellType emptyCellType10 = new FarPoint.Win.Spread.CellType.EmptyCellType();
            this.btnQuit = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSeikyuDate = new System.Windows.Forms.Label();
            this.seikyuBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblSeikyuAmount = new System.Windows.Forms.Label();
            this.lblSeikyusakiName = new System.Windows.Forms.Label();
            this.lblSeikyusakiCD = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSettlementStatus = new System.Windows.Forms.Label();
            this.btnExec = new System.Windows.Forms.Button();
            this.lblSeikyuNo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.fpSpread1 = new FarPoint.Win.Spread.FpSpread();
            this.fpSpread1_Sheet1 = new FarPoint.Win.Spread.SheetView();
            this.label11 = new System.Windows.Forms.Label();
            this.btnInquiry = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(830, 257);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 17;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "請求先";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(341, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "請求日";
            // 
            // lblSeikyuDate
            // 
            this.lblSeikyuDate.AutoEllipsis = true;
            this.lblSeikyuDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuBindingSource, "請求日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.lblSeikyuDate.Location = new System.Drawing.Point(399, 8);
            this.lblSeikyuDate.Name = "lblSeikyuDate";
            this.lblSeikyuDate.Size = new System.Drawing.Size(123, 22);
            this.lblSeikyuDate.TabIndex = 5;
            this.lblSeikyuDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // seikyuBindingSource
            // 
            this.seikyuBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuGokei);
            // 
            // lblSeikyuAmount
            // 
            this.lblSeikyuAmount.AutoEllipsis = true;
            this.lblSeikyuAmount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuBindingSource, "今回請求額", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, "0", "N0"));
            this.lblSeikyuAmount.Location = new System.Drawing.Point(616, 8);
            this.lblSeikyuAmount.Name = "lblSeikyuAmount";
            this.lblSeikyuAmount.Size = new System.Drawing.Size(123, 22);
            this.lblSeikyuAmount.TabIndex = 7;
            this.lblSeikyuAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSeikyusakiName
            // 
            this.lblSeikyusakiName.AutoEllipsis = true;
            this.lblSeikyusakiName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuBindingSource, "請求先名", true));
            this.lblSeikyusakiName.Location = new System.Drawing.Point(170, 36);
            this.lblSeikyusakiName.Name = "lblSeikyusakiName";
            this.lblSeikyusakiName.Size = new System.Drawing.Size(352, 22);
            this.lblSeikyusakiName.TabIndex = 13;
            this.lblSeikyusakiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSeikyusakiCD
            // 
            this.lblSeikyusakiCD.AutoEllipsis = true;
            this.lblSeikyusakiCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyusakiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuBindingSource, "請求先ＣＤ", true));
            this.lblSeikyusakiCD.Location = new System.Drawing.Point(89, 36);
            this.lblSeikyusakiCD.Name = "lblSeikyusakiCD";
            this.lblSeikyusakiCD.Size = new System.Drawing.Size(75, 22);
            this.lblSeikyusakiCD.TabIndex = 12;
            this.lblSeikyusakiCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(528, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "今回請求額";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "決済状態";
            // 
            // lblSettlementStatus
            // 
            this.lblSettlementStatus.AutoEllipsis = true;
            this.lblSettlementStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSettlementStatus.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuBindingSource, "決済状態", true));
            this.lblSettlementStatus.Location = new System.Drawing.Point(89, 8);
            this.lblSettlementStatus.Name = "lblSettlementStatus";
            this.lblSettlementStatus.Size = new System.Drawing.Size(75, 22);
            this.lblSettlementStatus.TabIndex = 1;
            this.lblSettlementStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.CausesValidation = false;
            this.btnExec.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExec.Location = new System.Drawing.Point(721, 257);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(103, 25);
            this.btnExec.TabIndex = 16;
            this.btnExec.Text = "決済取消";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // lblSeikyuNo
            // 
            this.lblSeikyuNo.AutoEllipsis = true;
            this.lblSeikyuNo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeikyuNo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuBindingSource, "請求番号", true));
            this.lblSeikyuNo.Location = new System.Drawing.Point(243, 8);
            this.lblSeikyuNo.Name = "lblSeikyuNo";
            this.lblSeikyuNo.Size = new System.Drawing.Size(92, 22);
            this.lblSeikyuNo.TabIndex = 3;
            this.lblSeikyuNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "請求番号";
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
            this.fpSpread1.FocusRenderer = enhancedFocusIndicatorRenderer2;
            this.fpSpread1.HorizontalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.HorizontalScrollBar.Name = "";
            this.fpSpread1.HorizontalScrollBar.Renderer = enhancedScrollBarRenderer3;
            this.fpSpread1.HorizontalScrollBarMode = FarPoint.Win.HorizontalScrollMode.PixelAndColumn;
            this.fpSpread1.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
            this.fpSpread1.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.fpSpread1.Location = new System.Drawing.Point(12, 87);
            this.fpSpread1.Name = "fpSpread1";
            this.fpSpread1.RowSplitBoxPolicy = FarPoint.Win.Spread.SplitBoxPolicy.Never;
            this.fpSpread1.ScrollBarTrackPolicy = FarPoint.Win.Spread.ScrollBarTrackPolicy.Both;
            this.fpSpread1.Sheets.AddRange(new FarPoint.Win.Spread.SheetView[] {
            this.fpSpread1_Sheet1});
            this.fpSpread1.Size = new System.Drawing.Size(921, 164);
            this.fpSpread1.Skin = FarPoint.Win.Spread.DefaultSpreadSkins.Office2007;
            this.fpSpread1.TabIndex = 14;
            this.fpSpread1.TextTipPolicy = FarPoint.Win.Spread.TextTipPolicy.Floating;
            this.fpSpread1.VerticalScrollBar.Buttons = new FarPoint.Win.Spread.FpScrollBarButtonCollection("BackwardLineButton,ThumbTrack,ForwardLineButton");
            this.fpSpread1.VerticalScrollBar.Name = "";
            this.fpSpread1.VerticalScrollBar.Renderer = enhancedScrollBarRenderer4;
            this.fpSpread1.VerticalScrollBarMode = FarPoint.Win.VerticalScrollMode.PixelAndRow;
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
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.Parent = "ColumnFooterEnhanced";
            this.fpSpread1_Sheet1.ColumnFooter.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.Parent = "CornerEnhanced";
            this.fpSpread1_Sheet1.ColumnFooterSheetCornerStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 0).Value = "処理日時";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 1).Value = "請求番号";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 2).Value = "取引金額";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 3).Value = "処理タイプ";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 4).Value = "処理結果";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 5).Value = "処理結果内容";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 6).Value = "承認番号";
            this.fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, 7).Value = "取引番号";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.General;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.NoteIndicatorColor = System.Drawing.Color.Red;
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.Parent = "ColumnHeaderEnhanced";
            this.fpSpread1_Sheet1.ColumnHeader.DefaultStyle.VerticalAlignment = FarPoint.Win.Spread.CellVerticalAlignment.General;
            generalCellType2.EnableSubEditor = false;
            generalCellType2.FormatString = "yyyy/MM/dd（ddd）HH:mm:ss";
            generalCellType2.IsDateFormat = true;
            this.fpSpread1_Sheet1.Columns.Get(0).CellType = generalCellType2;
            this.fpSpread1_Sheet1.Columns.Get(0).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(0).Label = "処理日時";
            this.fpSpread1_Sheet1.Columns.Get(0).Width = 180F;
            this.fpSpread1_Sheet1.Columns.Get(1).CellType = emptyCellType6;
            this.fpSpread1_Sheet1.Columns.Get(1).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
            this.fpSpread1_Sheet1.Columns.Get(1).Label = "請求番号";
            this.fpSpread1_Sheet1.Columns.Get(1).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(2).Label = "取引金額";
            this.fpSpread1_Sheet1.Columns.Get(2).Width = 100F;
            this.fpSpread1_Sheet1.Columns.Get(3).CellType = emptyCellType7;
            this.fpSpread1_Sheet1.Columns.Get(3).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(3).Label = "処理タイプ";
            this.fpSpread1_Sheet1.Columns.Get(3).Width = 75F;
            this.fpSpread1_Sheet1.Columns.Get(4).CellType = emptyCellType8;
            this.fpSpread1_Sheet1.Columns.Get(4).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(4).Label = "処理結果";
            this.fpSpread1_Sheet1.Columns.Get(4).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(5).CellType = emptyCellType9;
            this.fpSpread1_Sheet1.Columns.Get(5).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
            this.fpSpread1_Sheet1.Columns.Get(5).Label = "処理結果内容";
            this.fpSpread1_Sheet1.Columns.Get(5).Width = 170F;
            this.fpSpread1_Sheet1.Columns.Get(6).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(6).Label = "承認番号";
            this.fpSpread1_Sheet1.Columns.Get(6).Width = 70F;
            this.fpSpread1_Sheet1.Columns.Get(7).CellType = emptyCellType10;
            this.fpSpread1_Sheet1.Columns.Get(7).HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Center;
            this.fpSpread1_Sheet1.Columns.Get(7).Label = "取引番号";
            this.fpSpread1_Sheet1.Columns.Get(7).Width = 165F;
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(16, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 15);
            this.label11.TabIndex = 17;
            this.label11.Text = "決済履歴";
            // 
            // btnInquiry
            // 
            this.btnInquiry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInquiry.AutoSize = true;
            this.btnInquiry.CausesValidation = false;
            this.btnInquiry.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnInquiry.Location = new System.Drawing.Point(576, 257);
            this.btnInquiry.Name = "btnInquiry";
            this.btnInquiry.Size = new System.Drawing.Size(139, 25);
            this.btnInquiry.TabIndex = 15;
            this.btnInquiry.Text = "中間サーバー問合せ";
            this.btnInquiry.UseVisualStyleBackColor = true;
            this.btnInquiry.Click += new System.EventHandler(this.btnInquiry_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(528, 40);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(407, 30);
            this.lblMessage.TabIndex = 18;
            this.lblMessage.Text = "クレカ決済または決済取消にて不整合エラーが発生しています。\r\nシステム管理者までお問い合わせください。";
            this.lblMessage.Visible = false;
            // 
            // Form513D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 294);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnInquiry);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.fpSpread1);
            this.Controls.Add(this.lblSeikyuNo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblSeikyuDate);
            this.Controls.Add(this.lblSeikyuAmount);
            this.Controls.Add(this.lblSeikyusakiName);
            this.Controls.Add(this.lblSeikyusakiCD);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.lblSettlementStatus);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form513D";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "クレカ決済詳細";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form513D_FormClosing);
            this.Load += new System.EventHandler(this.Form513D_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form513D_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.seikyuBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fpSpread1_Sheet1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSeikyuDate;
        private System.Windows.Forms.Label lblSeikyuAmount;
        private System.Windows.Forms.Label lblSeikyusakiName;
        private System.Windows.Forms.Label lblSeikyusakiCD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSettlementStatus;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Label lblSeikyuNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingSource seikyuBindingSource;
        private System.Windows.Forms.BindingSource bindingSource1;
        private FarPoint.Win.Spread.FpSpread fpSpread1;
        private FarPoint.Win.Spread.SheetView fpSpread1_Sheet1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnInquiry;
        private System.Windows.Forms.Label lblMessage;
    }
}

namespace Emu.Bento
{
    partial class Form206
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
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkUseSetting = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.nohinPublishBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.dtpUriagebi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.rdoUriagebi = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.dtpHaisobi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.rdoHaisobi = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.chkTitle = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkPrintKingaku = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.btnTokuisakiCDTo = new System.Windows.Forms.Button();
            this.lblTokuisakiNameTo = new System.Windows.Forms.Label();
            this.txtTokuisakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtTokuisakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDFrom = new System.Windows.Forms.Button();
            this.lblTokuisakiNameFrom = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.chkRePrint = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.btnCourseCDTo = new System.Windows.Forms.Button();
            this.lblCourseNameTo = new System.Windows.Forms.Label();
            this.txtCourseCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtCourseCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnCourseCDFrom = new System.Windows.Forms.Button();
            this.lblCourseNameFrom = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nohinPublishBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(521, 214);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(412, 214);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 25);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "印刷(F10)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkUseSetting);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.chkTitle);
            this.groupBox1.Controls.Add(this.chkPrintKingaku);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameTo);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameFrom);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.chkRePrint);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnCourseCDTo);
            this.groupBox1.Controls.Add(this.lblCourseNameTo);
            this.groupBox1.Controls.Add(this.txtCourseCDTo);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnCourseCDFrom);
            this.groupBox1.Controls.Add(this.lblCourseNameFrom);
            this.groupBox1.Controls.Add(this.txtCourseCDFrom);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(612, 196);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象指定";
            // 
            // chkUseSetting
            // 
            this.chkUseSetting.AutoSize = true;
            this.chkUseSetting.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.nohinPublishBindingSource, "納品書発行区分使用フラグ", true));
            this.chkUseSetting.Location = new System.Drawing.Point(327, 144);
            this.chkUseSetting.Name = "chkUseSetting";
            this.chkUseSetting.Size = new System.Drawing.Size(210, 19);
            this.chkUseSetting.TabIndex = 23;
            this.chkUseSetting.Text = "得意先マスタの設定を使用する";
            this.chkUseSetting.UseVisualStyleBackColor = true;
            // 
            // nohinPublishBindingSource
            // 
            this.nohinPublishBindingSource.AllowNew = false;
            this.nohinPublishBindingSource.DataSource = typeof(Emu.Bento.Data.NohinPublish);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dtpUriagebi);
            this.panel1.Controls.Add(this.rdoUriagebi);
            this.panel1.Controls.Add(this.dtpHaisobi);
            this.panel1.Controls.Add(this.rdoHaisobi);
            this.panel1.Location = new System.Drawing.Point(225, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(370, 64);
            this.panel1.TabIndex = 2;
            // 
            // dtpUriagebi
            // 
            this.dtpUriagebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "売上日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpUriagebi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpUriagebi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpUriagebi.Location = new System.Drawing.Point(100, 39);
            this.dtpUriagebi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpUriagebi.Name = "dtpUriagebi";
            this.dtpUriagebi.Size = new System.Drawing.Size(152, 22);
            this.dtpUriagebi.TabIndex = 3;
            // 
            // rdoUriagebi
            // 
            this.rdoUriagebi.AutoSize = true;
            this.rdoUriagebi.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.nohinPublishBindingSource, "売上日指定", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rdoUriagebi.Location = new System.Drawing.Point(23, 39);
            this.rdoUriagebi.Name = "rdoUriagebi";
            this.rdoUriagebi.Size = new System.Drawing.Size(70, 19);
            this.rdoUriagebi.TabIndex = 2;
            this.rdoUriagebi.Text = "売上日";
            this.rdoUriagebi.UseVisualStyleBackColor = true;
            this.rdoUriagebi.CheckedChanged += new System.EventHandler(this.rdoUriagebi_CheckedChanged);
            // 
            // dtpHaisobi
            // 
            this.dtpHaisobi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "配送日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpHaisobi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpHaisobi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpHaisobi.Location = new System.Drawing.Point(100, 9);
            this.dtpHaisobi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpHaisobi.Name = "dtpHaisobi";
            this.dtpHaisobi.Size = new System.Drawing.Size(152, 22);
            this.dtpHaisobi.TabIndex = 1;
            // 
            // rdoHaisobi
            // 
            this.rdoHaisobi.AutoSize = true;
            this.rdoHaisobi.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.nohinPublishBindingSource, "配送日指定", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rdoHaisobi.Location = new System.Drawing.Point(23, 9);
            this.rdoHaisobi.Name = "rdoHaisobi";
            this.rdoHaisobi.Size = new System.Drawing.Size(70, 19);
            this.rdoHaisobi.TabIndex = 0;
            this.rdoHaisobi.Text = "配送日";
            this.rdoHaisobi.UseVisualStyleBackColor = true;
            this.rdoHaisobi.CheckedChanged += new System.EventHandler(this.rdoHaisobi_CheckedChanged);
            // 
            // chkTitle
            // 
            this.chkTitle.AutoSize = true;
            this.chkTitle.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.nohinPublishBindingSource, "見出しフラグ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkTitle.Location = new System.Drawing.Point(327, 171);
            this.chkTitle.Name = "chkTitle";
            this.chkTitle.Size = new System.Drawing.Size(235, 19);
            this.chkTitle.TabIndex = 26;
            this.chkTitle.Text = "見出しを「納品書兼請求書」にする";
            this.chkTitle.UseVisualStyleBackColor = true;
            this.chkTitle.CheckedChanged += new System.EventHandler(this.chkTitle_CheckedChanged);
            // 
            // chkPrintKingaku
            // 
            this.chkPrintKingaku.AutoSize = true;
            this.chkPrintKingaku.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.nohinPublishBindingSource, "金額印字フラグ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkPrintKingaku.Location = new System.Drawing.Point(191, 171);
            this.chkPrintKingaku.Name = "chkPrintKingaku";
            this.chkPrintKingaku.Size = new System.Drawing.Size(121, 19);
            this.chkPrintKingaku.TabIndex = 25;
            this.chkPrintKingaku.Text = "金額を印字する";
            this.chkPrintKingaku.UseVisualStyleBackColor = true;
            // 
            // btnTokuisakiCDTo
            // 
            this.btnTokuisakiCDTo.AutoSize = true;
            this.btnTokuisakiCDTo.Location = new System.Drawing.Point(559, 110);
            this.btnTokuisakiCDTo.Name = "btnTokuisakiCDTo";
            this.btnTokuisakiCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDTo.TabIndex = 18;
            this.btnTokuisakiCDTo.TabStop = false;
            this.btnTokuisakiCDTo.Text = "参照";
            this.btnTokuisakiCDTo.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDTo.Click += new System.EventHandler(this.btnTokuisakiCDTo_Click);
            // 
            // lblTokuisakiNameTo
            // 
            this.lblTokuisakiNameTo.AutoEllipsis = true;
            this.lblTokuisakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "得意先名To", true));
            this.lblTokuisakiNameTo.Location = new System.Drawing.Point(441, 111);
            this.lblTokuisakiNameTo.Name = "lblTokuisakiNameTo";
            this.lblTokuisakiNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameTo.TabIndex = 17;
            this.lblTokuisakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDTo
            // 
            this.txtTokuisakiCDTo.CopyFromWhenEmpty = this.txtTokuisakiCDFrom;
            this.txtTokuisakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "得意先ＣＤTo", true));
            this.txtTokuisakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDTo.Location = new System.Drawing.Point(352, 112);
            this.txtTokuisakiCDTo.MaxLength = 9;
            this.txtTokuisakiCDTo.Name = "txtTokuisakiCDTo";
            this.txtTokuisakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDTo.TabIndex = 16;
            this.txtTokuisakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDTo_KeyDown);
            // 
            // txtTokuisakiCDFrom
            // 
            this.txtTokuisakiCDFrom.CopyFromWhenEmpty = null;
            this.txtTokuisakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "得意先ＣＤFrom", true));
            this.txtTokuisakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDFrom.Location = new System.Drawing.Point(64, 112);
            this.txtTokuisakiCDFrom.MaxLength = 9;
            this.txtTokuisakiCDFrom.Name = "txtTokuisakiCDFrom";
            this.txtTokuisakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDFrom.TabIndex = 12;
            this.txtTokuisakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDFrom_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "～";
            // 
            // btnTokuisakiCDFrom
            // 
            this.btnTokuisakiCDFrom.AutoSize = true;
            this.btnTokuisakiCDFrom.Location = new System.Drawing.Point(271, 110);
            this.btnTokuisakiCDFrom.Name = "btnTokuisakiCDFrom";
            this.btnTokuisakiCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDFrom.TabIndex = 14;
            this.btnTokuisakiCDFrom.TabStop = false;
            this.btnTokuisakiCDFrom.Text = "参照";
            this.btnTokuisakiCDFrom.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDFrom.Click += new System.EventHandler(this.btnTokuisakiCDFrom_Click);
            // 
            // lblTokuisakiNameFrom
            // 
            this.lblTokuisakiNameFrom.AutoEllipsis = true;
            this.lblTokuisakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "得意先名From", true));
            this.lblTokuisakiNameFrom.Location = new System.Drawing.Point(153, 111);
            this.lblTokuisakiNameFrom.Name = "lblTokuisakiNameFrom";
            this.lblTokuisakiNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameFrom.TabIndex = 13;
            this.lblTokuisakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "得意先";
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
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.nohinPublishBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(64, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // chkRePrint
            // 
            this.chkRePrint.AutoSize = true;
            this.chkRePrint.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.nohinPublishBindingSource, "印刷済みフラグ", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkRePrint.Location = new System.Drawing.Point(9, 171);
            this.chkRePrint.Name = "chkRePrint";
            this.chkRePrint.Size = new System.Drawing.Size(167, 19);
            this.chkRePrint.TabIndex = 24;
            this.chkRePrint.Text = "印刷済みデータを含める";
            this.chkRePrint.UseVisualStyleBackColor = true;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Location = new System.Drawing.Point(252, 145);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 22;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Checked = true;
            this.rdoAsc.Location = new System.Drawing.Point(191, 145);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 21;
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
            this.cboSortKey.Location = new System.Drawing.Point(64, 144);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(121, 23);
            this.cboSortKey.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "印刷順";
            // 
            // btnCourseCDTo
            // 
            this.btnCourseCDTo.AutoSize = true;
            this.btnCourseCDTo.Location = new System.Drawing.Point(559, 79);
            this.btnCourseCDTo.Name = "btnCourseCDTo";
            this.btnCourseCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDTo.TabIndex = 10;
            this.btnCourseCDTo.TabStop = false;
            this.btnCourseCDTo.Text = "参照";
            this.btnCourseCDTo.UseVisualStyleBackColor = true;
            this.btnCourseCDTo.Click += new System.EventHandler(this.btnCourseCDTo_Click);
            // 
            // lblCourseNameTo
            // 
            this.lblCourseNameTo.AutoEllipsis = true;
            this.lblCourseNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "コース名To", true));
            this.lblCourseNameTo.Location = new System.Drawing.Point(441, 80);
            this.lblCourseNameTo.Name = "lblCourseNameTo";
            this.lblCourseNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameTo.TabIndex = 9;
            this.lblCourseNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDTo
            // 
            this.txtCourseCDTo.CopyFromWhenEmpty = this.txtCourseCDFrom;
            this.txtCourseCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "コースＣＤTo", true));
            this.txtCourseCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDTo.Location = new System.Drawing.Point(352, 81);
            this.txtCourseCDTo.MaxLength = 9;
            this.txtCourseCDTo.Name = "txtCourseCDTo";
            this.txtCourseCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDTo.TabIndex = 8;
            this.txtCourseCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDTo_KeyDown);
            // 
            // txtCourseCDFrom
            // 
            this.txtCourseCDFrom.CopyFromWhenEmpty = null;
            this.txtCourseCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "コースＣＤFrom", true));
            this.txtCourseCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDFrom.Location = new System.Drawing.Point(64, 81);
            this.txtCourseCDFrom.MaxLength = 9;
            this.txtCourseCDFrom.Name = "txtCourseCDFrom";
            this.txtCourseCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDFrom.TabIndex = 4;
            this.txtCourseCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDFrom_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(324, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 7;
            this.label12.Text = "～";
            // 
            // btnCourseCDFrom
            // 
            this.btnCourseCDFrom.AutoSize = true;
            this.btnCourseCDFrom.Location = new System.Drawing.Point(271, 79);
            this.btnCourseCDFrom.Name = "btnCourseCDFrom";
            this.btnCourseCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDFrom.TabIndex = 6;
            this.btnCourseCDFrom.TabStop = false;
            this.btnCourseCDFrom.Text = "参照";
            this.btnCourseCDFrom.UseVisualStyleBackColor = true;
            this.btnCourseCDFrom.Click += new System.EventHandler(this.btnCourseCDFrom_Click);
            // 
            // lblCourseNameFrom
            // 
            this.lblCourseNameFrom.AutoEllipsis = true;
            this.lblCourseNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.nohinPublishBindingSource, "コース名From", true));
            this.lblCourseNameFrom.Location = new System.Drawing.Point(153, 80);
            this.lblCourseNameFrom.Name = "lblCourseNameFrom";
            this.lblCourseNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameFrom.TabIndex = 5;
            this.lblCourseNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 84);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 15);
            this.label14.TabIndex = 3;
            this.label14.Text = "コース";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.nohinPublishBindingSource;
            // 
            // Form206
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 251);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form206";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "納品書発行";
            this.Load += new System.EventHandler(this.Form206_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form206_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nohinPublishBindingSource)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource nohinPublishBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button btnCourseCDTo;
        private System.Windows.Forms.Label lblCourseNameTo;
        private Common.Control.TextBoxAN txtCourseCDTo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnCourseCDFrom;
        private System.Windows.Forms.Label lblCourseNameFrom;
        private Common.Control.TextBoxAN txtCourseCDFrom;
        private System.Windows.Forms.Label label14;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private Common.Control.ComboBoxA cboSortKey;
        private System.Windows.Forms.Label label9;
        private Common.Control.CheckBoxA chkRePrint;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Button btnTokuisakiCDTo;
        private System.Windows.Forms.Label lblTokuisakiNameTo;
        private Common.Control.TextBoxAN txtTokuisakiCDTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnTokuisakiCDFrom;
        private System.Windows.Forms.Label lblTokuisakiNameFrom;
        private Common.Control.TextBoxAN txtTokuisakiCDFrom;
        private System.Windows.Forms.Label label6;
        private Common.Control.CheckBoxA chkPrintKingaku;
        private Common.Control.CheckBoxA chkTitle;
        private System.Windows.Forms.Panel panel1;
        private Common.Control.RadioButtonA rdoHaisobi;
        private Common.Control.DateTimePickerA dtpHaisobi;
        private Common.Control.DateTimePickerA dtpUriagebi;
        private Common.Control.RadioButtonA rdoUriagebi;
        private Common.Control.CheckBoxA chkUseSetting;
    }
}
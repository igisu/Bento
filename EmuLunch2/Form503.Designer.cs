namespace Emu.Bento
{
    partial class Form503
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
            this.chkCard = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.seikyuPublishBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.chkPrintFurikaeKoza = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.seikyuCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.chkDueDateBlank = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkUriage = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkTankaZero = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkOutOfCourse = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkPrintZero = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.dtpSeikyubi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.btnCourseCDTo = new System.Windows.Forms.Button();
            this.lblCourseNameTo = new System.Windows.Forms.Label();
            this.txtCourseCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtCourseCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.btnCourseCDFrom = new System.Windows.Forms.Button();
            this.lblCourseNameFrom = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.chkRePrint = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.rdoDesc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoAsc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.cboSortKey = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdoShimeMonthly = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoShimeWeekly = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoShimeDaily = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoShimeAll = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.btnTokuisakiCDTo = new System.Windows.Forms.Button();
            this.lblTokuisakiNameTo = new System.Windows.Forms.Label();
            this.txtTokuisakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtTokuisakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDFrom = new System.Windows.Forms.Button();
            this.lblTokuisakiNameFrom = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnPDF = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuPublishBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuCriteriaBindingSource)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(521, 295);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(412, 295);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 25);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "印刷(F10)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.chkCard);
            this.groupBox1.Controls.Add(this.chkPrintFurikaeKoza);
            this.groupBox1.Controls.Add(this.chkDueDateBlank);
            this.groupBox1.Controls.Add(this.chkUriage);
            this.groupBox1.Controls.Add(this.chkTankaZero);
            this.groupBox1.Controls.Add(this.chkOutOfCourse);
            this.groupBox1.Controls.Add(this.chkPrintZero);
            this.groupBox1.Controls.Add(this.dtpSeikyubi);
            this.groupBox1.Controls.Add(this.btnCourseCDTo);
            this.groupBox1.Controls.Add(this.lblCourseNameTo);
            this.groupBox1.Controls.Add(this.txtCourseCDTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnCourseCDFrom);
            this.groupBox1.Controls.Add(this.lblCourseNameFrom);
            this.groupBox1.Controls.Add(this.txtCourseCDFrom);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.chkRePrint);
            this.groupBox1.Controls.Add(this.rdoDesc);
            this.groupBox1.Controls.Add(this.rdoAsc);
            this.groupBox1.Controls.Add(this.cboSortKey);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameTo);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameFrom);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(612, 277);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象指定";
            // 
            // chkCard
            // 
            this.chkCard.AutoSize = true;
            this.chkCard.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuPublishBindingSource, "クレカフラグ", true));
            this.chkCard.Location = new System.Drawing.Point(312, 250);
            this.chkCard.Name = "chkCard";
            this.chkCard.Size = new System.Drawing.Size(182, 19);
            this.chkCard.TabIndex = 33;
            this.chkCard.Text = "クレカ決済請求書を含める";
            this.chkCard.UseVisualStyleBackColor = true;
            // 
            // seikyuPublishBindingSource
            // 
            this.seikyuPublishBindingSource.AllowNew = false;
            this.seikyuPublishBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuPublish);
            // 
            // chkPrintFurikaeKoza
            // 
            this.chkPrintFurikaeKoza.AutoSize = true;
            this.chkPrintFurikaeKoza.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuCriteriaBindingSource, "振替口座印字フラグ", true));
            this.chkPrintFurikaeKoza.Location = new System.Drawing.Point(9, 250);
            this.chkPrintFurikaeKoza.Name = "chkPrintFurikaeKoza";
            this.chkPrintFurikaeKoza.Size = new System.Drawing.Size(151, 19);
            this.chkPrintFurikaeKoza.TabIndex = 29;
            this.chkPrintFurikaeKoza.Text = "振替口座を印字する";
            this.chkPrintFurikaeKoza.UseVisualStyleBackColor = true;
            // 
            // seikyuCriteriaBindingSource
            // 
            this.seikyuCriteriaBindingSource.AllowNew = false;
            this.seikyuCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuCriteria);
            // 
            // chkDueDateBlank
            // 
            this.chkDueDateBlank.AutoSize = true;
            this.chkDueDateBlank.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuCriteriaBindingSource, "回収予定日を出力しない", true));
            this.chkDueDateBlank.Location = new System.Drawing.Point(312, 225);
            this.chkDueDateBlank.Name = "chkDueDateBlank";
            this.chkDueDateBlank.Size = new System.Drawing.Size(163, 19);
            this.chkDueDateBlank.TabIndex = 32;
            this.chkDueDateBlank.Text = "支払期限を印字しない";
            this.chkDueDateBlank.UseVisualStyleBackColor = true;
            // 
            // chkUriage
            // 
            this.chkUriage.AutoSize = true;
            this.chkUriage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuCriteriaBindingSource, "今回売上のみを出力", true));
            this.chkUriage.Location = new System.Drawing.Point(312, 200);
            this.chkUriage.Name = "chkUriage";
            this.chkUriage.Size = new System.Drawing.Size(167, 19);
            this.chkUriage.TabIndex = 31;
            this.chkUriage.Text = "当月売上額のみを請求";
            this.chkUriage.UseVisualStyleBackColor = true;
            // 
            // chkTankaZero
            // 
            this.chkTankaZero.AutoSize = true;
            this.chkTankaZero.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuCriteriaBindingSource, "単価ゼロフラグ", true));
            this.chkTankaZero.Location = new System.Drawing.Point(312, 175);
            this.chkTankaZero.Name = "chkTankaZero";
            this.chkTankaZero.Size = new System.Drawing.Size(294, 19);
            this.chkTankaZero.TabIndex = 30;
            this.chkTankaZero.Text = "単価と値引の両方が0の売上明細を出力する";
            this.chkTankaZero.UseVisualStyleBackColor = true;
            // 
            // chkOutOfCourse
            // 
            this.chkOutOfCourse.AutoSize = true;
            this.chkOutOfCourse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuPublishBindingSource, "コース外", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkOutOfCourse.Location = new System.Drawing.Point(9, 225);
            this.chkOutOfCourse.Name = "chkOutOfCourse";
            this.chkOutOfCourse.Size = new System.Drawing.Size(99, 19);
            this.chkOutOfCourse.TabIndex = 28;
            this.chkOutOfCourse.Text = "コース外のみ";
            this.chkOutOfCourse.UseVisualStyleBackColor = true;
            this.chkOutOfCourse.CheckedChanged += new System.EventHandler(this.chkOutOfCourse_CheckedChanged);
            // 
            // chkPrintZero
            // 
            this.chkPrintZero.AutoSize = true;
            this.chkPrintZero.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuPublishBindingSource, "ゼロ印刷フラグ", true));
            this.chkPrintZero.Location = new System.Drawing.Point(9, 200);
            this.chkPrintZero.Name = "chkPrintZero";
            this.chkPrintZero.Size = new System.Drawing.Size(228, 19);
            this.chkPrintZero.TabIndex = 27;
            this.chkPrintZero.Text = "請求金額が0の請求書も印刷する";
            this.chkPrintZero.UseVisualStyleBackColor = true;
            // 
            // dtpSeikyubi
            // 
            this.dtpSeikyubi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "請求日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubi.Location = new System.Drawing.Point(304, 21);
            this.dtpSeikyubi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubi.Name = "dtpSeikyubi";
            this.dtpSeikyubi.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubi.TabIndex = 3;
            // 
            // btnCourseCDTo
            // 
            this.btnCourseCDTo.AutoSize = true;
            this.btnCourseCDTo.Location = new System.Drawing.Point(559, 49);
            this.btnCourseCDTo.Name = "btnCourseCDTo";
            this.btnCourseCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDTo.TabIndex = 11;
            this.btnCourseCDTo.TabStop = false;
            this.btnCourseCDTo.Text = "参照";
            this.btnCourseCDTo.UseVisualStyleBackColor = true;
            this.btnCourseCDTo.Click += new System.EventHandler(this.btnCourseCDTo_Click);
            // 
            // lblCourseNameTo
            // 
            this.lblCourseNameTo.AutoEllipsis = true;
            this.lblCourseNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "コース名To", true));
            this.lblCourseNameTo.Location = new System.Drawing.Point(441, 50);
            this.lblCourseNameTo.Name = "lblCourseNameTo";
            this.lblCourseNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameTo.TabIndex = 10;
            this.lblCourseNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDTo
            // 
            this.txtCourseCDTo.CopyFromWhenEmpty = this.txtCourseCDFrom;
            this.txtCourseCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "コースＣＤTo", true));
            this.txtCourseCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDTo.Location = new System.Drawing.Point(352, 51);
            this.txtCourseCDTo.MaxLength = 9;
            this.txtCourseCDTo.Name = "txtCourseCDTo";
            this.txtCourseCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDTo.TabIndex = 9;
            this.txtCourseCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDTo_KeyDown);
            // 
            // txtCourseCDFrom
            // 
            this.txtCourseCDFrom.CopyFromWhenEmpty = null;
            this.txtCourseCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "コースＣＤFrom", true));
            this.txtCourseCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDFrom.Location = new System.Drawing.Point(64, 51);
            this.txtCourseCDFrom.MaxLength = 9;
            this.txtCourseCDFrom.Name = "txtCourseCDFrom";
            this.txtCourseCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDFrom.TabIndex = 5;
            this.txtCourseCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDFrom_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(324, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "～";
            // 
            // btnCourseCDFrom
            // 
            this.btnCourseCDFrom.AutoSize = true;
            this.btnCourseCDFrom.Location = new System.Drawing.Point(271, 49);
            this.btnCourseCDFrom.Name = "btnCourseCDFrom";
            this.btnCourseCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDFrom.TabIndex = 7;
            this.btnCourseCDFrom.TabStop = false;
            this.btnCourseCDFrom.Text = "参照";
            this.btnCourseCDFrom.UseVisualStyleBackColor = true;
            this.btnCourseCDFrom.Click += new System.EventHandler(this.btnCourseCDFrom_Click);
            // 
            // lblCourseNameFrom
            // 
            this.lblCourseNameFrom.AutoEllipsis = true;
            this.lblCourseNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "コース名From", true));
            this.lblCourseNameFrom.Location = new System.Drawing.Point(153, 50);
            this.lblCourseNameFrom.Name = "lblCourseNameFrom";
            this.lblCourseNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameFrom.TabIndex = 6;
            this.lblCourseNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "コース";
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
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuPublishBindingSource, "部署ＣＤ", true));
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
            this.chkRePrint.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuPublishBindingSource, "印刷済みフラグ", true));
            this.chkRePrint.Location = new System.Drawing.Point(9, 175);
            this.chkRePrint.Name = "chkRePrint";
            this.chkRePrint.Size = new System.Drawing.Size(167, 19);
            this.chkRePrint.TabIndex = 26;
            this.chkRePrint.Text = "印刷済みデータを含める";
            this.chkRePrint.UseVisualStyleBackColor = true;
            // 
            // rdoDesc
            // 
            this.rdoDesc.AutoSize = true;
            this.rdoDesc.Location = new System.Drawing.Point(251, 149);
            this.rdoDesc.Name = "rdoDesc";
            this.rdoDesc.Size = new System.Drawing.Size(55, 19);
            this.rdoDesc.TabIndex = 25;
            this.rdoDesc.Text = "降順";
            this.rdoDesc.UseVisualStyleBackColor = true;
            // 
            // rdoAsc
            // 
            this.rdoAsc.AutoSize = true;
            this.rdoAsc.Checked = true;
            this.rdoAsc.Location = new System.Drawing.Point(190, 149);
            this.rdoAsc.Name = "rdoAsc";
            this.rdoAsc.Size = new System.Drawing.Size(55, 19);
            this.rdoAsc.TabIndex = 24;
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
            this.cboSortKey.Location = new System.Drawing.Point(63, 146);
            this.cboSortKey.Name = "cboSortKey";
            this.cboSortKey.Size = new System.Drawing.Size(121, 23);
            this.cboSortKey.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 149);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 22;
            this.label9.Text = "印刷順";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdoShimeMonthly);
            this.panel1.Controls.Add(this.rdoShimeWeekly);
            this.panel1.Controls.Add(this.rdoShimeDaily);
            this.panel1.Controls.Add(this.rdoShimeAll);
            this.panel1.Location = new System.Drawing.Point(64, 111);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(313, 29);
            this.panel1.TabIndex = 21;
            // 
            // rdoShimeMonthly
            // 
            this.rdoShimeMonthly.AutoSize = true;
            this.rdoShimeMonthly.Location = new System.Drawing.Point(193, 3);
            this.rdoShimeMonthly.Name = "rdoShimeMonthly";
            this.rdoShimeMonthly.Size = new System.Drawing.Size(55, 19);
            this.rdoShimeMonthly.TabIndex = 3;
            this.rdoShimeMonthly.Text = "月締";
            this.rdoShimeMonthly.UseVisualStyleBackColor = true;
            // 
            // rdoShimeWeekly
            // 
            this.rdoShimeWeekly.AutoSize = true;
            this.rdoShimeWeekly.Location = new System.Drawing.Point(132, 3);
            this.rdoShimeWeekly.Name = "rdoShimeWeekly";
            this.rdoShimeWeekly.Size = new System.Drawing.Size(55, 19);
            this.rdoShimeWeekly.TabIndex = 2;
            this.rdoShimeWeekly.Text = "週締";
            this.rdoShimeWeekly.UseVisualStyleBackColor = true;
            // 
            // rdoShimeDaily
            // 
            this.rdoShimeDaily.AutoSize = true;
            this.rdoShimeDaily.Location = new System.Drawing.Point(71, 3);
            this.rdoShimeDaily.Name = "rdoShimeDaily";
            this.rdoShimeDaily.Size = new System.Drawing.Size(55, 19);
            this.rdoShimeDaily.TabIndex = 1;
            this.rdoShimeDaily.Text = "日締";
            this.rdoShimeDaily.UseVisualStyleBackColor = true;
            // 
            // rdoShimeAll
            // 
            this.rdoShimeAll.AutoSize = true;
            this.rdoShimeAll.Checked = true;
            this.rdoShimeAll.Location = new System.Drawing.Point(3, 3);
            this.rdoShimeAll.Name = "rdoShimeAll";
            this.rdoShimeAll.Size = new System.Drawing.Size(62, 19);
            this.rdoShimeAll.TabIndex = 0;
            this.rdoShimeAll.TabStop = true;
            this.rdoShimeAll.Text = "すべて";
            this.rdoShimeAll.UseVisualStyleBackColor = true;
            // 
            // btnTokuisakiCDTo
            // 
            this.btnTokuisakiCDTo.AutoSize = true;
            this.btnTokuisakiCDTo.Location = new System.Drawing.Point(559, 80);
            this.btnTokuisakiCDTo.Name = "btnTokuisakiCDTo";
            this.btnTokuisakiCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDTo.TabIndex = 19;
            this.btnTokuisakiCDTo.TabStop = false;
            this.btnTokuisakiCDTo.Text = "参照";
            this.btnTokuisakiCDTo.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDTo.Click += new System.EventHandler(this.btnTokuisakiCDTo_Click);
            // 
            // lblTokuisakiNameTo
            // 
            this.lblTokuisakiNameTo.AutoEllipsis = true;
            this.lblTokuisakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "請求先名To", true));
            this.lblTokuisakiNameTo.Location = new System.Drawing.Point(441, 81);
            this.lblTokuisakiNameTo.Name = "lblTokuisakiNameTo";
            this.lblTokuisakiNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameTo.TabIndex = 18;
            this.lblTokuisakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDTo
            // 
            this.txtTokuisakiCDTo.CopyFromWhenEmpty = this.txtTokuisakiCDFrom;
            this.txtTokuisakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "請求先ＣＤTo", true));
            this.txtTokuisakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDTo.Location = new System.Drawing.Point(352, 82);
            this.txtTokuisakiCDTo.MaxLength = 9;
            this.txtTokuisakiCDTo.Name = "txtTokuisakiCDTo";
            this.txtTokuisakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDTo.TabIndex = 17;
            this.txtTokuisakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDTo_KeyDown);
            // 
            // txtTokuisakiCDFrom
            // 
            this.txtTokuisakiCDFrom.CopyFromWhenEmpty = null;
            this.txtTokuisakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "請求先ＣＤFrom", true));
            this.txtTokuisakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDFrom.Location = new System.Drawing.Point(64, 82);
            this.txtTokuisakiCDFrom.MaxLength = 9;
            this.txtTokuisakiCDFrom.Name = "txtTokuisakiCDFrom";
            this.txtTokuisakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDFrom.TabIndex = 13;
            this.txtTokuisakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDFrom_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(324, 85);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 16;
            this.label12.Text = "～";
            // 
            // btnTokuisakiCDFrom
            // 
            this.btnTokuisakiCDFrom.AutoSize = true;
            this.btnTokuisakiCDFrom.Location = new System.Drawing.Point(271, 80);
            this.btnTokuisakiCDFrom.Name = "btnTokuisakiCDFrom";
            this.btnTokuisakiCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDFrom.TabIndex = 15;
            this.btnTokuisakiCDFrom.TabStop = false;
            this.btnTokuisakiCDFrom.Text = "参照";
            this.btnTokuisakiCDFrom.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDFrom.Click += new System.EventHandler(this.btnTokuisakiCDFrom_Click);
            // 
            // lblTokuisakiNameFrom
            // 
            this.lblTokuisakiNameFrom.AutoEllipsis = true;
            this.lblTokuisakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuPublishBindingSource, "請求先名From", true));
            this.lblTokuisakiNameFrom.Location = new System.Drawing.Point(153, 81);
            this.lblTokuisakiNameFrom.Name = "lblTokuisakiNameFrom";
            this.lblTokuisakiNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameFrom.TabIndex = 14;
            this.lblTokuisakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 85);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 12;
            this.label14.Text = "請求先";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 20;
            this.label4.Text = "締区分";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(246, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "請求日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.seikyuPublishBindingSource;
            // 
            // btnPDF
            // 
            this.btnPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPDF.AutoSize = true;
            this.btnPDF.Location = new System.Drawing.Point(303, 295);
            this.btnPDF.Name = "btnPDF";
            this.btnPDF.Size = new System.Drawing.Size(103, 25);
            this.btnPDF.TabIndex = 1;
            this.btnPDF.Text = "PDF保存";
            this.btnPDF.UseVisualStyleBackColor = true;
            this.btnPDF.Click += new System.EventHandler(this.btnPDF_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 297);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(199, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProgress.Location = new System.Drawing.Point(217, 300);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(80, 15);
            this.lblProgress.TabIndex = 5;
            this.lblProgress.Text = "9999/9999";
            // 
            // Form503
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 332);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnPDF);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form503";
            this.Text = "請求書発行";
            this.Load += new System.EventHandler(this.Form503_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form503_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuPublishBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuCriteriaBindingSource)).EndInit();
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
        private System.Windows.Forms.BindingSource seikyuPublishBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private Common.Control.RadioButtonA rdoShimeDaily;
        private Common.Control.RadioButtonA rdoShimeAll;
        private System.Windows.Forms.Button btnTokuisakiCDTo;
        private System.Windows.Forms.Label lblTokuisakiNameTo;
        private Common.Control.TextBoxAN txtTokuisakiCDTo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnTokuisakiCDFrom;
        private System.Windows.Forms.Label lblTokuisakiNameFrom;
        private Common.Control.TextBoxAN txtTokuisakiCDFrom;
        private System.Windows.Forms.Label label14;
        private Common.Control.RadioButtonA rdoShimeMonthly;
        private Common.Control.RadioButtonA rdoShimeWeekly;
        private Common.Control.RadioButtonA rdoDesc;
        private Common.Control.RadioButtonA rdoAsc;
        private Common.Control.ComboBoxA cboSortKey;
        private System.Windows.Forms.Label label9;
        private Common.Control.CheckBoxA chkRePrint;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private System.Windows.Forms.Button btnCourseCDTo;
        private System.Windows.Forms.Label lblCourseNameTo;
        private Common.Control.TextBoxAN txtCourseCDTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCourseCDFrom;
        private System.Windows.Forms.Label lblCourseNameFrom;
        private Common.Control.TextBoxAN txtCourseCDFrom;
        private System.Windows.Forms.Label label5;
        private Common.Control.DateTimePickerA dtpSeikyubi;
        private Common.Control.CheckBoxA chkPrintZero;
        private Common.Control.CheckBoxA chkOutOfCourse;
        private Common.Control.CheckBoxA chkTankaZero;
        private System.Windows.Forms.BindingSource seikyuCriteriaBindingSource;
        private Common.Control.CheckBoxA chkUriage;
        private Common.Control.CheckBoxA chkDueDateBlank;
        private Common.Control.CheckBoxA chkPrintFurikaeKoza;
        private System.Windows.Forms.Button btnPDF;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblProgress;
        private Common.Control.CheckBoxA chkCard;
    }
}

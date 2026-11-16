namespace Emu.Bento
{
    partial class Form500
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
            this.grpMonth = new System.Windows.Forms.GroupBox();
            this.chkTokushimeMonth = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.seikyuShimeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.txtTsukishimebi = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label34 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMonth = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txtYear = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.grpWeek = new System.Windows.Forms.GroupBox();
            this.chkTokushimeWeek = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.cboYoubi = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkOutOfCourse = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.btnCourseCD = new System.Windows.Forms.Button();
            this.lblCourseName = new System.Windows.Forms.Label();
            this.txtCourseCD = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDTo = new System.Windows.Forms.Button();
            this.lblTokuisakiNameTo = new System.Windows.Forms.Label();
            this.txtTokuisakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDFrom = new System.Windows.Forms.Button();
            this.lblTokuisakiNameFrom = new System.Windows.Forms.Label();
            this.txtTokuisakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnExec = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtpSeikyubi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.cboShime = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.grpMonth.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuShimeBindingSource)).BeginInit();
            this.grpWeek.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // grpMonth
            // 
            this.grpMonth.Controls.Add(this.chkTokushimeMonth);
            this.grpMonth.Controls.Add(this.label8);
            this.grpMonth.Controls.Add(this.label35);
            this.grpMonth.Controls.Add(this.txtTsukishimebi);
            this.grpMonth.Controls.Add(this.label34);
            this.grpMonth.Controls.Add(this.label6);
            this.grpMonth.Controls.Add(this.txtMonth);
            this.grpMonth.Controls.Add(this.label7);
            this.grpMonth.Controls.Add(this.txtYear);
            this.grpMonth.Controls.Add(this.label9);
            this.grpMonth.Location = new System.Drawing.Point(292, 68);
            this.grpMonth.Name = "grpMonth";
            this.grpMonth.Size = new System.Drawing.Size(332, 78);
            this.grpMonth.TabIndex = 2;
            this.grpMonth.TabStop = false;
            this.grpMonth.Text = "月締";
            // 
            // chkTokushimeMonth
            // 
            this.chkTokushimeMonth.AutoSize = true;
            this.chkTokushimeMonth.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuShimeBindingSource, "月特締", true));
            this.chkTokushimeMonth.ForeColor = System.Drawing.Color.Red;
            this.chkTokushimeMonth.Location = new System.Drawing.Point(270, 24);
            this.chkTokushimeMonth.Name = "chkTokushimeMonth";
            this.chkTokushimeMonth.Size = new System.Drawing.Size(56, 19);
            this.chkTokushimeMonth.TabIndex = 9;
            this.chkTokushimeMonth.Text = "特締";
            this.chkTokushimeMonth.UseVisualStyleBackColor = true;
            this.chkTokushimeMonth.CheckedChanged += new System.EventHandler(this.chkTokushime_CheckedChanged);
            // 
            // seikyuShimeBindingSource
            // 
            this.seikyuShimeBindingSource.AllowNew = false;
            this.seikyuShimeBindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuShimeCriteria);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(121, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "※末日：99";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(78, 53);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(22, 15);
            this.label35.TabIndex = 7;
            this.label35.Text = "日";
            // 
            // txtTsukishimebi
            // 
            this.txtTsukishimebi.CopyFromWhenEmpty = null;
            this.txtTsukishimebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "月締日", true));
            this.txtTsukishimebi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTsukishimebi.Location = new System.Drawing.Point(47, 50);
            this.txtTsukishimebi.MaxLength = 2;
            this.txtTsukishimebi.Name = "txtTsukishimebi";
            this.txtTsukishimebi.Size = new System.Drawing.Size(25, 22);
            this.txtTsukishimebi.TabIndex = 6;
            this.txtTsukishimebi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTsukishimebi.Validated += new System.EventHandler(this.txtTsukishime_Validated);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 53);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(37, 15);
            this.label34.TabIndex = 5;
            this.label34.Text = "締日";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(157, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "月";
            // 
            // txtMonth
            // 
            this.txtMonth.CopyFromWhenEmpty = null;
            this.txtMonth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "月締月", true));
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtMonth.Location = new System.Drawing.Point(124, 22);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(27, 22);
            this.txtMonth.TabIndex = 3;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMonth.Validated += new System.EventHandler(this.txtTsukishime_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(96, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "年";
            // 
            // txtYear
            // 
            this.txtYear.CopyFromWhenEmpty = null;
            this.txtYear.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "月締年", true));
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtYear.Location = new System.Drawing.Point(47, 22);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(41, 22);
            this.txtYear.TabIndex = 1;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtYear.Validated += new System.EventHandler(this.txtTsukishime_Validated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "年月";
            // 
            // grpWeek
            // 
            this.grpWeek.Controls.Add(this.chkTokushimeWeek);
            this.grpWeek.Controls.Add(this.label5);
            this.grpWeek.Controls.Add(this.cboYoubi);
            this.grpWeek.Location = new System.Drawing.Point(291, 12);
            this.grpWeek.Name = "grpWeek";
            this.grpWeek.Size = new System.Drawing.Size(333, 50);
            this.grpWeek.TabIndex = 1;
            this.grpWeek.TabStop = false;
            this.grpWeek.Text = "週締";
            // 
            // chkTokushimeWeek
            // 
            this.chkTokushimeWeek.AutoSize = true;
            this.chkTokushimeWeek.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuShimeBindingSource, "週特締", true));
            this.chkTokushimeWeek.ForeColor = System.Drawing.Color.Red;
            this.chkTokushimeWeek.Location = new System.Drawing.Point(271, 23);
            this.chkTokushimeWeek.Name = "chkTokushimeWeek";
            this.chkTokushimeWeek.Size = new System.Drawing.Size(56, 19);
            this.chkTokushimeWeek.TabIndex = 2;
            this.chkTokushimeWeek.Text = "特締";
            this.chkTokushimeWeek.UseVisualStyleBackColor = true;
            this.chkTokushimeWeek.CheckedChanged += new System.EventHandler(this.chkTokushime_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "曜日";
            // 
            // cboYoubi
            // 
            this.cboYoubi.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboYoubi.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboYoubi.BackColor = System.Drawing.SystemColors.Control;
            this.cboYoubi.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuShimeBindingSource, "週締曜日", true));
            this.cboYoubi.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboYoubi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboYoubi.FormattingEnabled = true;
            this.cboYoubi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboYoubi.Location = new System.Drawing.Point(49, 21);
            this.cboYoubi.Name = "cboYoubi";
            this.cboYoubi.Size = new System.Drawing.Size(93, 23);
            this.cboYoubi.TabIndex = 1;
            this.cboYoubi.SelectedValueChanged += new System.EventHandler(this.cboYoubi_SelectedValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkOutOfCourse);
            this.groupBox2.Controls.Add(this.btnCourseCD);
            this.groupBox2.Controls.Add(this.lblCourseName);
            this.groupBox2.Controls.Add(this.txtCourseCD);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnTokuisakiCDTo);
            this.groupBox2.Controls.Add(this.lblTokuisakiNameTo);
            this.groupBox2.Controls.Add(this.txtTokuisakiCDTo);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.btnTokuisakiCDFrom);
            this.groupBox2.Controls.Add(this.lblTokuisakiNameFrom);
            this.groupBox2.Controls.Add(this.txtTokuisakiCDFrom);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Location = new System.Drawing.Point(12, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(612, 84);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "対象絞込";
            // 
            // chkOutOfCourse
            // 
            this.chkOutOfCourse.AutoSize = true;
            this.chkOutOfCourse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuShimeBindingSource, "コース外", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkOutOfCourse.Location = new System.Drawing.Point(327, 23);
            this.chkOutOfCourse.Name = "chkOutOfCourse";
            this.chkOutOfCourse.Size = new System.Drawing.Size(99, 19);
            this.chkOutOfCourse.TabIndex = 4;
            this.chkOutOfCourse.Text = "コース外のみ";
            this.chkOutOfCourse.UseVisualStyleBackColor = true;
            this.chkOutOfCourse.CheckedChanged += new System.EventHandler(this.chkOutOfCourse_CheckedChanged);
            // 
            // btnCourseCD
            // 
            this.btnCourseCD.AutoSize = true;
            this.btnCourseCD.Location = new System.Drawing.Point(271, 19);
            this.btnCourseCD.Name = "btnCourseCD";
            this.btnCourseCD.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCD.TabIndex = 3;
            this.btnCourseCD.TabStop = false;
            this.btnCourseCD.Text = "参照";
            this.btnCourseCD.UseVisualStyleBackColor = true;
            this.btnCourseCD.Click += new System.EventHandler(this.btnCourseCD_Click);
            // 
            // lblCourseName
            // 
            this.lblCourseName.AutoEllipsis = true;
            this.lblCourseName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "コース名", true));
            this.lblCourseName.Location = new System.Drawing.Point(153, 20);
            this.lblCourseName.Name = "lblCourseName";
            this.lblCourseName.Size = new System.Drawing.Size(112, 22);
            this.lblCourseName.TabIndex = 2;
            this.lblCourseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCD
            // 
            this.txtCourseCD.CopyFromWhenEmpty = null;
            this.txtCourseCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "コースＣＤ", true));
            this.txtCourseCD.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCD.Location = new System.Drawing.Point(64, 21);
            this.txtCourseCD.MaxLength = 9;
            this.txtCourseCD.Name = "txtCourseCD";
            this.txtCourseCD.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCD.TabIndex = 1;
            this.txtCourseCD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCD_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "コース";
            // 
            // btnTokuisakiCDTo
            // 
            this.btnTokuisakiCDTo.AutoSize = true;
            this.btnTokuisakiCDTo.Location = new System.Drawing.Point(559, 50);
            this.btnTokuisakiCDTo.Name = "btnTokuisakiCDTo";
            this.btnTokuisakiCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDTo.TabIndex = 12;
            this.btnTokuisakiCDTo.TabStop = false;
            this.btnTokuisakiCDTo.Text = "参照";
            this.btnTokuisakiCDTo.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDTo.Click += new System.EventHandler(this.btnTokuisakiCDTo_Click);
            // 
            // lblTokuisakiNameTo
            // 
            this.lblTokuisakiNameTo.AutoEllipsis = true;
            this.lblTokuisakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "請求先名To", true));
            this.lblTokuisakiNameTo.Location = new System.Drawing.Point(441, 52);
            this.lblTokuisakiNameTo.Name = "lblTokuisakiNameTo";
            this.lblTokuisakiNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameTo.TabIndex = 11;
            this.lblTokuisakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDTo
            // 
            this.txtTokuisakiCDTo.CopyFromWhenEmpty = this.txtTokuisakiCDFrom;
            this.txtTokuisakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "請求先ＣＤTo", true));
            this.txtTokuisakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDTo.Location = new System.Drawing.Point(352, 52);
            this.txtTokuisakiCDTo.MaxLength = 9;
            this.txtTokuisakiCDTo.Name = "txtTokuisakiCDTo";
            this.txtTokuisakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDTo.TabIndex = 10;
            this.txtTokuisakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDTo_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(324, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 9;
            this.label12.Text = "～";
            // 
            // btnTokuisakiCDFrom
            // 
            this.btnTokuisakiCDFrom.AutoSize = true;
            this.btnTokuisakiCDFrom.Location = new System.Drawing.Point(271, 50);
            this.btnTokuisakiCDFrom.Name = "btnTokuisakiCDFrom";
            this.btnTokuisakiCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDFrom.TabIndex = 8;
            this.btnTokuisakiCDFrom.TabStop = false;
            this.btnTokuisakiCDFrom.Text = "参照";
            this.btnTokuisakiCDFrom.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDFrom.Click += new System.EventHandler(this.btnTokuisakiCDFrom_Click);
            // 
            // lblTokuisakiNameFrom
            // 
            this.lblTokuisakiNameFrom.AutoEllipsis = true;
            this.lblTokuisakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "請求先名From", true));
            this.lblTokuisakiNameFrom.Location = new System.Drawing.Point(153, 52);
            this.lblTokuisakiNameFrom.Name = "lblTokuisakiNameFrom";
            this.lblTokuisakiNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameFrom.TabIndex = 7;
            this.lblTokuisakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDFrom
            // 
            this.txtTokuisakiCDFrom.CopyFromWhenEmpty = null;
            this.txtTokuisakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "請求先ＣＤFrom", true));
            this.txtTokuisakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDFrom.Location = new System.Drawing.Point(64, 52);
            this.txtTokuisakiCDFrom.MaxLength = 9;
            this.txtTokuisakiCDFrom.Name = "txtTokuisakiCDFrom";
            this.txtTokuisakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDFrom.TabIndex = 6;
            this.txtTokuisakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDFrom_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 55);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 5;
            this.label14.Text = "請求先";
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(521, 245);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 5;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.Location = new System.Drawing.Point(412, 245);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(103, 25);
            this.btnExec.TabIndex = 4;
            this.btnExec.Text = "処理開始(F9)";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtpSeikyubi);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.cboShime);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "締め対象指定";
            // 
            // dtpSeikyubi
            // 
            this.dtpSeikyubi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuShimeBindingSource, "請求日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubi.Location = new System.Drawing.Point(64, 76);
            this.dtpSeikyubi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubi.Name = "dtpSeikyubi";
            this.dtpSeikyubi.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubi.TabIndex = 5;
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuShimeBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(64, 20);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // cboShime
            // 
            this.cboShime.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboShime.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboShime.BackColor = System.Drawing.SystemColors.Control;
            this.cboShime.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuShimeBindingSource, "締区分", true));
            this.cboShime.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboShime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShime.FormattingEnabled = true;
            this.cboShime.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboShime.Location = new System.Drawing.Point(64, 49);
            this.cboShime.Name = "cboShime";
            this.cboShime.Size = new System.Drawing.Size(121, 23);
            this.cboShime.TabIndex = 3;
            this.cboShime.SelectedValueChanged += new System.EventHandler(this.cboShime_SelectedValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "締区分";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "請求日";
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.seikyuShimeBindingSource;
            // 
            // Form500
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 282);
            this.Controls.Add(this.grpMonth);
            this.Controls.Add(this.grpWeek);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form500";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "請求締処理";
            this.Load += new System.EventHandler(this.Form500_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form500_KeyDown);
            this.grpMonth.ResumeLayout(false);
            this.grpMonth.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuShimeBindingSource)).EndInit();
            this.grpWeek.ResumeLayout(false);
            this.grpWeek.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource seikyuShimeBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpMonth;
        private System.Windows.Forms.GroupBox grpWeek;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnTokuisakiCDFrom;
        private System.Windows.Forms.Label lblTokuisakiNameFrom;
        private Common.Control.TextBoxAN txtTokuisakiCDFrom;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private Common.Control.ComboBoxA cboYoubi;
        private System.Windows.Forms.Label label6;
        private Common.Control.TextBoxAN txtMonth;
        private System.Windows.Forms.Label label7;
        private Common.Control.TextBoxAN txtYear;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label35;
        private Common.Control.TextBoxAN txtTsukishimebi;
        private System.Windows.Forms.Label label34;
        private Common.Control.CheckBoxA chkTokushimeMonth;
        private System.Windows.Forms.Button btnTokuisakiCDTo;
        private System.Windows.Forms.Label lblTokuisakiNameTo;
        private Common.Control.TextBoxAN txtTokuisakiCDTo;
        private Common.Control.ComboBoxA cboShime;
        private Common.Control.CheckBoxA chkTokushimeWeek;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpSeikyubi;
        private System.Windows.Forms.Button btnCourseCD;
        private System.Windows.Forms.Label lblCourseName;
        private Common.Control.TextBoxAN txtCourseCD;
        private System.Windows.Forms.Label label2;
        private Common.Control.CheckBoxA chkOutOfCourse;
    }
}
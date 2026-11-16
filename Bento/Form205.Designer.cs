namespace Emu.Bento
{
    partial class Form205
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
            this.koteiChumonHanneiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnExec = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCourseCDTo = new System.Windows.Forms.Button();
            this.lblCourseNameTo = new System.Windows.Forms.Label();
            this.txtCourseCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtCourseCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.btnCourseCDFrom = new System.Windows.Forms.Button();
            this.lblCourseNameFrom = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpSeisanbiTo = new Emu.Bento.Common.Control.DateTimePickerA();
            this.dtpSeisanbiFrom = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDTo = new System.Windows.Forms.Button();
            this.lblTokuisakiNameTo = new System.Windows.Forms.Label();
            this.txtTokuisakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.txtTokuisakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDFrom = new System.Windows.Forms.Button();
            this.lblTokuisakiNameFrom = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.koteiChumonHanneiBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // koteiChumonHanneiBindingSource
            // 
            this.koteiChumonHanneiBindingSource.AllowNew = false;
            this.koteiChumonHanneiBindingSource.DataSource = typeof(Emu.Bento.Data.KoteiChumonHanneiCriteria);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(522, 157);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnExec
            // 
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExec.AutoSize = true;
            this.btnExec.Location = new System.Drawing.Point(413, 157);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(103, 25);
            this.btnExec.TabIndex = 1;
            this.btnExec.Text = "処理開始(F9)";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnCourseCDTo);
            this.groupBox1.Controls.Add(this.lblCourseNameTo);
            this.groupBox1.Controls.Add(this.txtCourseCDTo);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnCourseCDFrom);
            this.groupBox1.Controls.Add(this.lblCourseNameFrom);
            this.groupBox1.Controls.Add(this.txtCourseCDFrom);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpSeisanbiTo);
            this.groupBox1.Controls.Add(this.dtpSeisanbiFrom);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameTo);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameFrom);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(613, 139);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "注文対象指定";
            // 
            // btnCourseCDTo
            // 
            this.btnCourseCDTo.AutoSize = true;
            this.btnCourseCDTo.Location = new System.Drawing.Point(560, 78);
            this.btnCourseCDTo.Name = "btnCourseCDTo";
            this.btnCourseCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDTo.TabIndex = 13;
            this.btnCourseCDTo.TabStop = false;
            this.btnCourseCDTo.Text = "参照";
            this.btnCourseCDTo.UseVisualStyleBackColor = true;
            this.btnCourseCDTo.Click += new System.EventHandler(this.btnCourseCDTo_Click);
            // 
            // lblCourseNameTo
            // 
            this.lblCourseNameTo.AutoEllipsis = true;
            this.lblCourseNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "コース名To", true));
            this.lblCourseNameTo.Location = new System.Drawing.Point(442, 80);
            this.lblCourseNameTo.Name = "lblCourseNameTo";
            this.lblCourseNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameTo.TabIndex = 12;
            this.lblCourseNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDTo
            // 
            this.txtCourseCDTo.CopyFromWhenEmpty = this.txtCourseCDFrom;
            this.txtCourseCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "コースＣＤTo", true));
            this.txtCourseCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDTo.Location = new System.Drawing.Point(353, 80);
            this.txtCourseCDTo.MaxLength = 9;
            this.txtCourseCDTo.Name = "txtCourseCDTo";
            this.txtCourseCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDTo.TabIndex = 11;
            this.txtCourseCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDTo_KeyDown);
            // 
            // txtCourseCDFrom
            // 
            this.txtCourseCDFrom.CopyFromWhenEmpty = null;
            this.txtCourseCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "コースＣＤFrom", true));
            this.txtCourseCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDFrom.Location = new System.Drawing.Point(65, 80);
            this.txtCourseCDFrom.MaxLength = 9;
            this.txtCourseCDFrom.Name = "txtCourseCDFrom";
            this.txtCourseCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDFrom.TabIndex = 7;
            this.txtCourseCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDFrom_KeyDown);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(325, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 18;
            this.label6.Text = "～";
            // 
            // btnCourseCDFrom
            // 
            this.btnCourseCDFrom.AutoSize = true;
            this.btnCourseCDFrom.Location = new System.Drawing.Point(272, 78);
            this.btnCourseCDFrom.Name = "btnCourseCDFrom";
            this.btnCourseCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnCourseCDFrom.TabIndex = 9;
            this.btnCourseCDFrom.TabStop = false;
            this.btnCourseCDFrom.Text = "参照";
            this.btnCourseCDFrom.UseVisualStyleBackColor = true;
            this.btnCourseCDFrom.Click += new System.EventHandler(this.btnCourseCDFrom_Click);
            // 
            // lblCourseNameFrom
            // 
            this.lblCourseNameFrom.AutoEllipsis = true;
            this.lblCourseNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCourseNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "コース名From", true));
            this.lblCourseNameFrom.Location = new System.Drawing.Point(154, 80);
            this.lblCourseNameFrom.Name = "lblCourseNameFrom";
            this.lblCourseNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameFrom.TabIndex = 8;
            this.lblCourseNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "コース";
            // 
            // dtpSeisanbiTo
            // 
            this.dtpSeisanbiTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "適用日To", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
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
            this.dtpSeisanbiFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "適用日From", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
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
            // btnTokuisakiCDTo
            // 
            this.btnTokuisakiCDTo.AutoSize = true;
            this.btnTokuisakiCDTo.Location = new System.Drawing.Point(560, 108);
            this.btnTokuisakiCDTo.Name = "btnTokuisakiCDTo";
            this.btnTokuisakiCDTo.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDTo.TabIndex = 21;
            this.btnTokuisakiCDTo.TabStop = false;
            this.btnTokuisakiCDTo.Text = "参照";
            this.btnTokuisakiCDTo.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDTo.Click += new System.EventHandler(this.btnTokuisakiCDTo_Click);
            // 
            // lblTokuisakiNameTo
            // 
            this.lblTokuisakiNameTo.AutoEllipsis = true;
            this.lblTokuisakiNameTo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "得意先名To", true));
            this.lblTokuisakiNameTo.Location = new System.Drawing.Point(442, 110);
            this.lblTokuisakiNameTo.Name = "lblTokuisakiNameTo";
            this.lblTokuisakiNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameTo.TabIndex = 20;
            this.lblTokuisakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDTo
            // 
            this.txtTokuisakiCDTo.CopyFromWhenEmpty = this.txtTokuisakiCDFrom;
            this.txtTokuisakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "得意先ＣＤTo", true));
            this.txtTokuisakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDTo.Location = new System.Drawing.Point(353, 110);
            this.txtTokuisakiCDTo.MaxLength = 9;
            this.txtTokuisakiCDTo.Name = "txtTokuisakiCDTo";
            this.txtTokuisakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDTo.TabIndex = 19;
            this.txtTokuisakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDTo_KeyDown);
            // 
            // txtTokuisakiCDFrom
            // 
            this.txtTokuisakiCDFrom.CopyFromWhenEmpty = null;
            this.txtTokuisakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "得意先ＣＤFrom", true));
            this.txtTokuisakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDFrom.Location = new System.Drawing.Point(65, 110);
            this.txtTokuisakiCDFrom.MaxLength = 9;
            this.txtTokuisakiCDFrom.Name = "txtTokuisakiCDFrom";
            this.txtTokuisakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDFrom.TabIndex = 15;
            this.txtTokuisakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDFrom_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(325, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "～";
            // 
            // btnTokuisakiCDFrom
            // 
            this.btnTokuisakiCDFrom.AutoSize = true;
            this.btnTokuisakiCDFrom.Location = new System.Drawing.Point(272, 108);
            this.btnTokuisakiCDFrom.Name = "btnTokuisakiCDFrom";
            this.btnTokuisakiCDFrom.Size = new System.Drawing.Size(47, 25);
            this.btnTokuisakiCDFrom.TabIndex = 17;
            this.btnTokuisakiCDFrom.TabStop = false;
            this.btnTokuisakiCDFrom.Text = "参照";
            this.btnTokuisakiCDFrom.UseVisualStyleBackColor = true;
            this.btnTokuisakiCDFrom.Click += new System.EventHandler(this.btnTokuisakiCDFrom_Click);
            // 
            // lblTokuisakiNameFrom
            // 
            this.lblTokuisakiNameFrom.AutoEllipsis = true;
            this.lblTokuisakiNameFrom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.koteiChumonHanneiBindingSource, "得意先名From", true));
            this.lblTokuisakiNameFrom.Location = new System.Drawing.Point(154, 110);
            this.lblTokuisakiNameFrom.Name = "lblTokuisakiNameFrom";
            this.lblTokuisakiNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameFrom.TabIndex = 16;
            this.lblTokuisakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 113);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 14;
            this.label14.Text = "得意先";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.koteiChumonHanneiBindingSource, "部署ＣＤ", true));
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
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.koteiChumonHanneiBindingSource;
            // 
            // Form205
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 194);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form205";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "固定注文反映";
            this.Load += new System.EventHandler(this.Form205_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form205_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.koteiChumonHanneiBindingSource)).EndInit();
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
        private System.Windows.Forms.BindingSource koteiChumonHanneiBindingSource;
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
    }
}
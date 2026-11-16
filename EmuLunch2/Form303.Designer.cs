namespace Emu.Bento
{
    partial class Form303
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
            this.cboCourseKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.yokiKaisyuHyoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.dtpUriagebi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.btnCourseCDTo = new System.Windows.Forms.Button();
            this.lblCourseNameTo = new System.Windows.Forms.Label();
            this.txtCourseCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnCourseCDFrom = new System.Windows.Forms.Button();
            this.lblCourseNameFrom = new System.Windows.Forms.Label();
            this.txtCourseCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yokiKaisyuHyoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(550, 128);
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
            this.btnPrint.Location = new System.Drawing.Point(441, 128);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 25);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "印刷(F10)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboCourseKbn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpUriagebi);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.btnCourseCDTo);
            this.groupBox1.Controls.Add(this.lblCourseNameTo);
            this.groupBox1.Controls.Add(this.txtCourseCDTo);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnCourseCDFrom);
            this.groupBox1.Controls.Add(this.lblCourseNameFrom);
            this.groupBox1.Controls.Add(this.txtCourseCDFrom);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(641, 110);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象指定";
            // 
            // cboCourseKbn
            // 
            this.cboCourseKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboCourseKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboCourseKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboCourseKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.yokiKaisyuHyoBindingSource, "コース区分", true));
            this.cboCourseKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboCourseKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCourseKbn.FormattingEnabled = true;
            this.cboCourseKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboCourseKbn.Location = new System.Drawing.Point(86, 50);
            this.cboCourseKbn.Name = "cboCourseKbn";
            this.cboCourseKbn.Size = new System.Drawing.Size(121, 23);
            this.cboCourseKbn.TabIndex = 5;
            // 
            // yokiKaisyuHyoBindingSource
            // 
            this.yokiKaisyuHyoBindingSource.AllowNew = false;
            this.yokiKaisyuHyoBindingSource.DataSource = typeof(Emu.Bento.Data.YokiKaisyuHyo);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "コース区分";
            // 
            // dtpUriagebi
            // 
            this.dtpUriagebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.yokiKaisyuHyoBindingSource, "売上日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpUriagebi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpUriagebi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpUriagebi.Location = new System.Drawing.Point(326, 21);
            this.dtpUriagebi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpUriagebi.Name = "dtpUriagebi";
            this.dtpUriagebi.Size = new System.Drawing.Size(152, 22);
            this.dtpUriagebi.TabIndex = 3;
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
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.yokiKaisyuHyoBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(86, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            // 
            // btnCourseCDTo
            // 
            this.btnCourseCDTo.AutoSize = true;
            this.btnCourseCDTo.Location = new System.Drawing.Point(581, 77);
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
            this.lblCourseNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.yokiKaisyuHyoBindingSource, "コース名To", true));
            this.lblCourseNameTo.Location = new System.Drawing.Point(463, 78);
            this.lblCourseNameTo.Name = "lblCourseNameTo";
            this.lblCourseNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameTo.TabIndex = 12;
            this.lblCourseNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDTo
            // 
            this.txtCourseCDTo.CopyFromWhenEmpty = this.txtCourseCDFrom;
            this.txtCourseCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.yokiKaisyuHyoBindingSource, "コースＣＤTo", true));
            this.txtCourseCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDTo.Location = new System.Drawing.Point(374, 79);
            this.txtCourseCDTo.MaxLength = 9;
            this.txtCourseCDTo.Name = "txtCourseCDTo";
            this.txtCourseCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDTo.TabIndex = 11;
            this.txtCourseCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDTo_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(346, 82);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 10;
            this.label12.Text = "～";
            // 
            // btnCourseCDFrom
            // 
            this.btnCourseCDFrom.AutoSize = true;
            this.btnCourseCDFrom.Location = new System.Drawing.Point(293, 77);
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
            this.lblCourseNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.yokiKaisyuHyoBindingSource, "コース名From", true));
            this.lblCourseNameFrom.Location = new System.Drawing.Point(175, 78);
            this.lblCourseNameFrom.Name = "lblCourseNameFrom";
            this.lblCourseNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblCourseNameFrom.TabIndex = 8;
            this.lblCourseNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCourseCDFrom
            // 
            this.txtCourseCDFrom.CopyFromWhenEmpty = null;
            this.txtCourseCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.yokiKaisyuHyoBindingSource, "コースＣＤFrom", true));
            this.txtCourseCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCourseCDFrom.Location = new System.Drawing.Point(86, 79);
            this.txtCourseCDFrom.MaxLength = 9;
            this.txtCourseCDFrom.Name = "txtCourseCDFrom";
            this.txtCourseCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtCourseCDFrom.TabIndex = 7;
            this.txtCourseCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCourseCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCourseCDFrom_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 82);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(40, 15);
            this.label14.TabIndex = 6;
            this.label14.Text = "コース";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(268, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "売上日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.yokiKaisyuHyoBindingSource;
            // 
            // Form303
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 165);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form303";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "容器回収表印刷";
            this.Load += new System.EventHandler(this.Form303_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form303_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.yokiKaisyuHyoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource yokiKaisyuHyoBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCourseCDTo;
        private System.Windows.Forms.Label lblCourseNameTo;
        private Common.Control.TextBoxAN txtCourseCDTo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnCourseCDFrom;
        private System.Windows.Forms.Label lblCourseNameFrom;
        private Common.Control.TextBoxAN txtCourseCDFrom;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpUriagebi;
        private Common.Control.ComboBoxA cboCourseKbn;
        private System.Windows.Forms.Label label2;
    }
}

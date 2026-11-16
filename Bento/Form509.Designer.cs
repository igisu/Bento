namespace Emu.Bento
{
    partial class Form509
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
            this.paySeikyuShimeCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnExec = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkTokushimeMonth = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMonth = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txtYear = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.lblTokuisakiCD = new System.Windows.Forms.Label();
            this.lblTokuisakiName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboUriageKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.dtpSeikyubi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.lblShimebi = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.paySeikyuShimeCriteriaBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // paySeikyuShimeCriteriaBindingSource
            // 
            this.paySeikyuShimeCriteriaBindingSource.AllowNew = false;
            this.paySeikyuShimeCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.PaySeikyuShimeCriteria);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(474, 124);
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
            this.btnExec.Location = new System.Drawing.Point(365, 124);
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
            this.groupBox1.Controls.Add(this.lblShimebi);
            this.groupBox1.Controls.Add(this.chkTokushimeMonth);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtMonth);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtYear);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblTokuisakiCD);
            this.groupBox1.Controls.Add(this.lblTokuisakiName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboUriageKbn);
            this.groupBox1.Controls.Add(this.dtpSeikyubi);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(565, 106);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "締め対象指定";
            // 
            // chkTokushimeMonth
            // 
            this.chkTokushimeMonth.AutoSize = true;
            this.chkTokushimeMonth.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.paySeikyuShimeCriteriaBindingSource, "月特締", true));
            this.chkTokushimeMonth.ForeColor = System.Drawing.Color.Red;
            this.chkTokushimeMonth.Location = new System.Drawing.Point(232, 80);
            this.chkTokushimeMonth.Name = "chkTokushimeMonth";
            this.chkTokushimeMonth.Size = new System.Drawing.Size(56, 19);
            this.chkTokushimeMonth.TabIndex = 14;
            this.chkTokushimeMonth.Text = "特締";
            this.chkTokushimeMonth.UseVisualStyleBackColor = true;
            this.chkTokushimeMonth.CheckedChanged += new System.EventHandler(this.chkTokushime_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(470, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 15);
            this.label8.TabIndex = 8;
            this.label8.Text = "※末日：99";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(442, 53);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(22, 15);
            this.label35.TabIndex = 7;
            this.label35.Text = "日";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(365, 53);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(37, 15);
            this.label34.TabIndex = 5;
            this.label34.Text = "締日";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(189, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "月";
            // 
            // txtMonth
            // 
            this.txtMonth.CopyFromWhenEmpty = null;
            this.txtMonth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuShimeCriteriaBindingSource, "月締月", true));
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtMonth.Location = new System.Drawing.Point(156, 78);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(27, 22);
            this.txtMonth.TabIndex = 12;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMonth.Validated += new System.EventHandler(this.txtTsukishime_Validated);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(128, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "年";
            // 
            // txtYear
            // 
            this.txtYear.CopyFromWhenEmpty = null;
            this.txtYear.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuShimeCriteriaBindingSource, "月締年", true));
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtYear.Location = new System.Drawing.Point(79, 78);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(41, 22);
            this.txtYear.TabIndex = 10;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtYear.Validated += new System.EventHandler(this.txtTsukishime_Validated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 9;
            this.label9.Text = "年月";
            // 
            // lblTokuisakiCD
            // 
            this.lblTokuisakiCD.AutoEllipsis = true;
            this.lblTokuisakiCD.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiCD.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuShimeCriteriaBindingSource, "請求先ＣＤ", true));
            this.lblTokuisakiCD.Location = new System.Drawing.Point(79, 50);
            this.lblTokuisakiCD.Name = "lblTokuisakiCD";
            this.lblTokuisakiCD.Size = new System.Drawing.Size(83, 22);
            this.lblTokuisakiCD.TabIndex = 3;
            this.lblTokuisakiCD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTokuisakiName
            // 
            this.lblTokuisakiName.AutoEllipsis = true;
            this.lblTokuisakiName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTokuisakiName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuShimeCriteriaBindingSource, "請求先名", true));
            this.lblTokuisakiName.Location = new System.Drawing.Point(168, 50);
            this.lblTokuisakiName.Name = "lblTokuisakiName";
            this.lblTokuisakiName.Size = new System.Drawing.Size(191, 22);
            this.lblTokuisakiName.TabIndex = 4;
            this.lblTokuisakiName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "請求先";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "売上区分";
            // 
            // cboUriageKbn
            // 
            this.cboUriageKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboUriageKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboUriageKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboUriageKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.paySeikyuShimeCriteriaBindingSource, "売上区分", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cboUriageKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboUriageKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUriageKbn.FormattingEnabled = true;
            this.cboUriageKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboUriageKbn.Location = new System.Drawing.Point(79, 21);
            this.cboUriageKbn.Name = "cboUriageKbn";
            this.cboUriageKbn.Size = new System.Drawing.Size(121, 23);
            this.cboUriageKbn.TabIndex = 1;
            this.cboUriageKbn.SelectedIndexChanged += new System.EventHandler(this.cboUriageKbn_SelectedIndexChanged);
            // 
            // dtpSeikyubi
            // 
            this.dtpSeikyubi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuShimeCriteriaBindingSource, "請求日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpSeikyubi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpSeikyubi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpSeikyubi.Location = new System.Drawing.Point(366, 78);
            this.dtpSeikyubi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpSeikyubi.Name = "dtpSeikyubi";
            this.dtpSeikyubi.Size = new System.Drawing.Size(152, 22);
            this.dtpSeikyubi.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "請求日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.paySeikyuShimeCriteriaBindingSource;
            // 
            // lblShimebi
            // 
            this.lblShimebi.AutoEllipsis = true;
            this.lblShimebi.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblShimebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.paySeikyuShimeCriteriaBindingSource, "月締日", true));
            this.lblShimebi.Location = new System.Drawing.Point(408, 50);
            this.lblShimebi.Name = "lblShimebi";
            this.lblShimebi.Size = new System.Drawing.Size(28, 22);
            this.lblShimebi.TabIndex = 6;
            this.lblShimebi.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form509
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 161);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form509";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "電子決済請求締処理";
            this.Load += new System.EventHandler(this.Form509_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form509_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.paySeikyuShimeCriteriaBindingSource)).EndInit();
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
        private System.Windows.Forms.BindingSource paySeikyuShimeCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private Common.Control.DateTimePickerA dtpSeikyubi;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboUriageKbn;
        private Common.Control.CheckBoxA chkTokushimeMonth;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label6;
        private Common.Control.TextBoxAN txtMonth;
        private System.Windows.Forms.Label label7;
        private Common.Control.TextBoxAN txtYear;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTokuisakiCD;
        private System.Windows.Forms.Label lblTokuisakiName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblShimebi;
    }
}
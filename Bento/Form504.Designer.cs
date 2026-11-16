namespace Emu.Bento
{
    partial class Form504
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
            this.btnOutput = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.txtTsukishimebi = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.seikyuDataEBbindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label34 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstBusyoEB = new System.Windows.Forms.ListBox();
            this.busyoEBbindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.txtMonth = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txtYear = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.dtpHikiotoshibi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.chkOutput = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.btnTokuisakiCDTo = new System.Windows.Forms.Button();
            this.lblTokuisakiNameTo = new System.Windows.Forms.Label();
            this.txtTokuisakiCDTo = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.btnTokuisakiCDFrom = new System.Windows.Forms.Button();
            this.lblTokuisakiNameFrom = new System.Windows.Forms.Label();
            this.txtTokuisakiCDFrom = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label14 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chkAllBusyo = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuDataEBbindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.busyoEBbindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(521, 182);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutput.AutoSize = true;
            this.btnOutput.Location = new System.Drawing.Point(412, 182);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(103, 25);
            this.btnOutput.TabIndex = 2;
            this.btnOutput.Text = "出力(F10)";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.txtTsukishimebi);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lstBusyoEB);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtMonth);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtYear);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.dtpHikiotoshibi);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.chkOutput);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameTo);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDTo);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.lblTokuisakiNameFrom);
            this.groupBox1.Controls.Add(this.txtTokuisakiCDFrom);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(612, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象指定";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(138, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "※末日：99";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(95, 139);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(22, 15);
            this.label35.TabIndex = 21;
            this.label35.Text = "日";
            // 
            // txtTsukishimebi
            // 
            this.txtTsukishimebi.CopyFromWhenEmpty = null;
            this.txtTsukishimebi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "月締日", true));
            this.txtTsukishimebi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTsukishimebi.Location = new System.Drawing.Point(64, 136);
            this.txtTsukishimebi.MaxLength = 2;
            this.txtTsukishimebi.Name = "txtTsukishimebi";
            this.txtTsukishimebi.Size = new System.Drawing.Size(25, 22);
            this.txtTsukishimebi.TabIndex = 20;
            this.txtTsukishimebi.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // seikyuDataEBbindingSource
            // 
            this.seikyuDataEBbindingSource.AllowNew = false;
            this.seikyuDataEBbindingSource.DataSource = typeof(Emu.Bento.Data.SeikyuDataEB);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(6, 139);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(52, 15);
            this.label34.TabIndex = 19;
            this.label34.Text = "月締日";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(246, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "出力先EB口座";
            // 
            // lstBusyoEB
            // 
            this.lstBusyoEB.DataSource = this.busyoEBbindingSource;
            this.lstBusyoEB.FormattingEnabled = true;
            this.lstBusyoEB.HorizontalScrollbar = true;
            this.lstBusyoEB.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.lstBusyoEB.ItemHeight = 15;
            this.lstBusyoEB.Location = new System.Drawing.Point(352, 21);
            this.lstBusyoEB.Name = "lstBusyoEB";
            this.lstBusyoEB.Size = new System.Drawing.Size(254, 79);
            this.lstBusyoEB.TabIndex = 10;
            this.lstBusyoEB.SelectedIndexChanged += new System.EventHandler(this.lstBusyoEB_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "月";
            // 
            // txtMonth
            // 
            this.txtMonth.CopyFromWhenEmpty = null;
            this.txtMonth.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "対象月", true));
            this.txtMonth.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtMonth.Location = new System.Drawing.Point(141, 50);
            this.txtMonth.MaxLength = 2;
            this.txtMonth.Name = "txtMonth";
            this.txtMonth.Size = new System.Drawing.Size(27, 22);
            this.txtMonth.TabIndex = 5;
            this.txtMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(113, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "年";
            // 
            // txtYear
            // 
            this.txtYear.CopyFromWhenEmpty = null;
            this.txtYear.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "対象年", true));
            this.txtYear.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtYear.Location = new System.Drawing.Point(64, 50);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(41, 22);
            this.txtYear.TabIndex = 3;
            this.txtYear.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 53);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "年月";
            // 
            // dtpHikiotoshibi
            // 
            this.dtpHikiotoshibi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "引落日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpHikiotoshibi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpHikiotoshibi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpHikiotoshibi.Location = new System.Drawing.Point(64, 79);
            this.dtpHikiotoshibi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpHikiotoshibi.Name = "dtpHikiotoshibi";
            this.dtpHikiotoshibi.Size = new System.Drawing.Size(152, 22);
            this.dtpHikiotoshibi.TabIndex = 8;
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
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.seikyuDataEBbindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(64, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            this.cboBusyo.SelectedValueChanged += new System.EventHandler(this.cboBusyo_SelectedValueChanged);
            // 
            // chkOutput
            // 
            this.chkOutput.AutoSize = true;
            this.chkOutput.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuDataEBbindingSource, "印刷済みフラグ", true));
            this.chkOutput.Location = new System.Drawing.Point(413, 138);
            this.chkOutput.Name = "chkOutput";
            this.chkOutput.Size = new System.Drawing.Size(193, 19);
            this.chkOutput.TabIndex = 23;
            this.chkOutput.Text = "作成済みのものも対象とする";
            this.chkOutput.UseVisualStyleBackColor = true;
            // 
            // btnTokuisakiCDTo
            // 
            this.btnTokuisakiCDTo.AutoSize = true;
            this.btnTokuisakiCDTo.Location = new System.Drawing.Point(559, 106);
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
            this.lblTokuisakiNameTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "請求先名To", true));
            this.lblTokuisakiNameTo.Location = new System.Drawing.Point(441, 107);
            this.lblTokuisakiNameTo.Name = "lblTokuisakiNameTo";
            this.lblTokuisakiNameTo.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameTo.TabIndex = 17;
            this.lblTokuisakiNameTo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDTo
            // 
            this.txtTokuisakiCDTo.CopyFromWhenEmpty = this.txtTokuisakiCDFrom;
            this.txtTokuisakiCDTo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "請求先ＣＤTo", true));
            this.txtTokuisakiCDTo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDTo.Location = new System.Drawing.Point(352, 108);
            this.txtTokuisakiCDTo.MaxLength = 9;
            this.txtTokuisakiCDTo.Name = "txtTokuisakiCDTo";
            this.txtTokuisakiCDTo.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDTo.TabIndex = 16;
            this.txtTokuisakiCDTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDTo_KeyDown);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(324, 111);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 15);
            this.label12.TabIndex = 15;
            this.label12.Text = "～";
            // 
            // btnTokuisakiCDFrom
            // 
            this.btnTokuisakiCDFrom.AutoSize = true;
            this.btnTokuisakiCDFrom.Location = new System.Drawing.Point(271, 106);
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
            this.lblTokuisakiNameFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "請求先名From", true));
            this.lblTokuisakiNameFrom.Location = new System.Drawing.Point(153, 107);
            this.lblTokuisakiNameFrom.Name = "lblTokuisakiNameFrom";
            this.lblTokuisakiNameFrom.Size = new System.Drawing.Size(112, 22);
            this.lblTokuisakiNameFrom.TabIndex = 13;
            this.lblTokuisakiNameFrom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtTokuisakiCDFrom
            // 
            this.txtTokuisakiCDFrom.CopyFromWhenEmpty = null;
            this.txtTokuisakiCDFrom.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.seikyuDataEBbindingSource, "請求先ＣＤFrom", true));
            this.txtTokuisakiCDFrom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtTokuisakiCDFrom.Location = new System.Drawing.Point(64, 108);
            this.txtTokuisakiCDFrom.MaxLength = 9;
            this.txtTokuisakiCDFrom.Name = "txtTokuisakiCDFrom";
            this.txtTokuisakiCDFrom.Size = new System.Drawing.Size(83, 22);
            this.txtTokuisakiCDFrom.TabIndex = 12;
            this.txtTokuisakiCDFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTokuisakiCDFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTokuisakiCDFrom_KeyDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 111);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 15);
            this.label14.TabIndex = 11;
            this.label14.Text = "請求先";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "引落日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.seikyuDataEBbindingSource;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "テキストファイル(*.txt)|*.txt";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // chkAllBusyo
            // 
            this.chkAllBusyo.AutoSize = true;
            this.chkAllBusyo.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.seikyuDataEBbindingSource, "全部署出力フラグ", true));
            this.chkAllBusyo.Location = new System.Drawing.Point(12, 186);
            this.chkAllBusyo.Name = "chkAllBusyo";
            this.chkAllBusyo.Size = new System.Drawing.Size(283, 19);
            this.chkAllBusyo.TabIndex = 1;
            this.chkAllBusyo.Text = "上記の設定で全ての部署を出力対象とする";
            this.chkAllBusyo.UseVisualStyleBackColor = true;
            // 
            // Form504
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 219);
            this.Controls.Add(this.chkAllBusyo);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form504";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "請求EBデータ出力";
            this.Load += new System.EventHandler(this.Form504_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form504_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seikyuDataEBbindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.busyoEBbindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource seikyuDataEBbindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTokuisakiCDTo;
        private System.Windows.Forms.Label lblTokuisakiNameTo;
        private Common.Control.TextBoxAN txtTokuisakiCDTo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnTokuisakiCDFrom;
        private System.Windows.Forms.Label lblTokuisakiNameFrom;
        private Common.Control.TextBoxAN txtTokuisakiCDFrom;
        private System.Windows.Forms.Label label14;
        private Common.Control.CheckBoxA chkOutput;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpHikiotoshibi;
        private System.Windows.Forms.Label label6;
        private Common.Control.TextBoxAN txtMonth;
        private System.Windows.Forms.Label label7;
        private Common.Control.TextBoxAN txtYear;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListBox lstBusyoEB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.BindingSource busyoEBbindingSource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label35;
        private Common.Control.TextBoxAN txtTsukishimebi;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Common.Control.CheckBoxA chkAllBusyo;
    }
}
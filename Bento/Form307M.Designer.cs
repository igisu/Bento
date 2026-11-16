namespace Emu.Bento
{
    partial class Form307M
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
            this.label6 = new System.Windows.Forms.Label();
            this.bigBagLabelPrintCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.cboSyokujiKbn = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.rdoUnit = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoSet = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.dtpChumonbi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.txtSyokusu = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txtCount = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bigBagLabelPrintCriteriaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(147, 256);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 5;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(38, 256);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 25);
            this.btnPrint.TabIndex = 4;
            this.btnPrint.Text = "印刷(F10)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtSyokusu);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboSyokujiKbn);
            this.groupBox1.Controls.Add(this.rdoUnit);
            this.groupBox1.Controls.Add(this.rdoSet);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpChumonbi);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 162);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "印刷対象";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "食数";
            // 
            // bigBagLabelPrintCriteriaBindingSource
            // 
            this.bigBagLabelPrintCriteriaBindingSource.AllowNew = false;
            this.bigBagLabelPrintCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.BigBagLabelPrintCriteria);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "区分";
            // 
            // cboSyokujiKbn
            // 
            this.cboSyokujiKbn.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboSyokujiKbn.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSyokujiKbn.BackColor = System.Drawing.SystemColors.Control;
            this.cboSyokujiKbn.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.bigBagLabelPrintCriteriaBindingSource, "食事区分ＣＤ", true));
            this.cboSyokujiKbn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSyokujiKbn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSyokujiKbn.FormattingEnabled = true;
            this.cboSyokujiKbn.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboSyokujiKbn.Location = new System.Drawing.Point(79, 105);
            this.cboSyokujiKbn.Name = "cboSyokujiKbn";
            this.cboSyokujiKbn.Size = new System.Drawing.Size(143, 23);
            this.cboSyokujiKbn.TabIndex = 8;
            // 
            // rdoUnit
            // 
            this.rdoUnit.AutoSize = true;
            this.rdoUnit.Location = new System.Drawing.Point(140, 80);
            this.rdoUnit.Name = "rdoUnit";
            this.rdoUnit.Size = new System.Drawing.Size(55, 19);
            this.rdoUnit.TabIndex = 6;
            this.rdoUnit.Text = "単品";
            this.rdoUnit.UseVisualStyleBackColor = true;
            // 
            // rdoSet
            // 
            this.rdoSet.AutoSize = true;
            this.rdoSet.Checked = true;
            this.rdoSet.Location = new System.Drawing.Point(79, 80);
            this.rdoSet.Name = "rdoSet";
            this.rdoSet.Size = new System.Drawing.Size(57, 19);
            this.rdoSet.TabIndex = 5;
            this.rdoSet.TabStop = true;
            this.rdoSet.Text = "セット";
            this.rdoSet.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "種別";
            // 
            // dtpChumonbi
            // 
            this.dtpChumonbi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "注文日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpChumonbi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpChumonbi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpChumonbi.Location = new System.Drawing.Point(79, 22);
            this.dtpChumonbi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpChumonbi.Name = "dtpChumonbi";
            this.dtpChumonbi.Size = new System.Drawing.Size(152, 22);
            this.dtpChumonbi.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "部署";
            // 
            // cboBusyo
            // 
            this.cboBusyo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboBusyo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboBusyo.BackColor = System.Drawing.SystemColors.Control;
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.bigBagLabelPrintCriteriaBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboBusyo.Location = new System.Drawing.Point(79, 51);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "注文日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.bigBagLabelPrintCriteriaBindingSource;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(265, 30);
            this.label4.TabIndex = 0;
            this.label4.Text = "なんらかの理由によりラベルが足りない場合に\r\n利用してください。";
            // 
            // txtSyokusu
            // 
            this.txtSyokusu.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "食数", true));
            this.txtSyokusu.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtSyokusu.Location = new System.Drawing.Point(79, 134);
            this.txtSyokusu.MaxLength = 4;
            this.txtSyokusu.Name = "txtSyokusu";
            this.txtSyokusu.Size = new System.Drawing.Size(41, 22);
            this.txtSyokusu.TabIndex = 10;
            this.txtSyokusu.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(126, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "食";
            // 
            // txtCount
            // 
            this.txtCount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bigBagLabelPrintCriteriaBindingSource, "印刷枚数", true));
            this.txtCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtCount.Location = new System.Drawing.Point(91, 227);
            this.txtCount.MaxLength = 3;
            this.txtCount.Name = "txtCount";
            this.txtCount.Size = new System.Drawing.Size(41, 22);
            this.txtCount.TabIndex = 3;
            this.txtCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(18, 230);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "印刷枚数";
            // 
            // Form307M
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 293);
            this.Controls.Add(this.txtCount);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form307M";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "大袋ラベル手動印刷";
            this.Load += new System.EventHandler(this.Form307M_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form307M_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bigBagLabelPrintCriteriaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource bigBagLabelPrintCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpChumonbi;
        private Common.Control.RadioButtonA rdoUnit;
        private Common.Control.RadioButtonA rdoSet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private Common.Control.ComboBoxA cboSyokujiKbn;
        private System.Windows.Forms.Label label7;
        private Common.Control.TextBoxAN txtSyokusu;
        private Common.Control.TextBoxAN txtCount;
        private System.Windows.Forms.Label label9;
    }
}
namespace Emu.Bento
{
    partial class Form603N
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
            this.btnExec = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboNyukinHoho = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.ikkatsuNyukinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dtpNyukinbi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.label1 = new System.Windows.Forms.Label();
            this.cboBusyo = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikkatsuNyukinBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            //
            // btnQuit
            //
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(150, 182);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            //
            // btnExec
            //
            this.btnExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExec.AutoSize = true;
            this.btnExec.Location = new System.Drawing.Point(41, 182);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(103, 25);
            this.btnExec.TabIndex = 2;
            this.btnExec.Text = "入金確定";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboNyukinHoho);
            this.groupBox1.Controls.Add(this.dtpNyukinbi);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboBusyo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 113);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "入金条件";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "入金方法";
            //
            // cboNyukinHoho
            //
            this.cboNyukinHoho.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboNyukinHoho.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboNyukinHoho.BackColor = System.Drawing.SystemColors.Control;
            this.cboNyukinHoho.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ikkatsuNyukinBindingSource, "入金方法", true));
            this.cboNyukinHoho.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboNyukinHoho.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNyukinHoho.FormattingEnabled = true;
            this.cboNyukinHoho.Location = new System.Drawing.Point(79, 80);
            this.cboNyukinHoho.Name = "cboNyukinHoho";
            this.cboNyukinHoho.Size = new System.Drawing.Size(121, 23);
            this.cboNyukinHoho.TabIndex = 5;
            //
            // ikkatsuNyukinBindingSource
            //
            this.ikkatsuNyukinBindingSource.AllowNew = false;
            this.ikkatsuNyukinBindingSource.DataSource = typeof(Emu.Bento.Data.IkkatsuNyukin);
            //
            // dtpNyukinbi
            //
            this.dtpNyukinbi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ikkatsuNyukinBindingSource, "入金日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpNyukinbi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpNyukinbi.Location = new System.Drawing.Point(79, 51);
            this.dtpNyukinbi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpNyukinbi.Name = "dtpNyukinbi";
            this.dtpNyukinbi.Size = new System.Drawing.Size(152, 22);
            this.dtpNyukinbi.TabIndex = 3;
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
            this.cboBusyo.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ikkatsuNyukinBindingSource, "部署ＣＤ", true));
            this.cboBusyo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboBusyo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBusyo.FormattingEnabled = true;
            this.cboBusyo.Location = new System.Drawing.Point(79, 21);
            this.cboBusyo.Name = "cboBusyo";
            this.cboBusyo.Size = new System.Drawing.Size(143, 23);
            this.cboBusyo.TabIndex = 1;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "入金日";
            //
            // errorProvider1
            //
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.ikkatsuNyukinBindingSource;
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(279, 45);
            this.label4.TabIndex = 0;
            this.label4.Text = "一括入金チェックをONにした得意先に対して、\r\n売掛残高と同額の入金を行います。\r\n※0円以下の売掛残高は入金しません。";
            //
            // Form603N
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 219);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExec);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form603N";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "一括入金入力";
            this.Load += new System.EventHandler(this.Form603N_Load);
            this.Shown += new System.EventHandler(this.Form603N_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form603N_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikkatsuNyukinBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource ikkatsuNyukinBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private Common.Control.ComboBoxA cboBusyo;
        private Common.Control.DateTimePickerA dtpNyukinbi;
        private System.Windows.Forms.Label label2;
        private Common.Control.ComboBoxA cboNyukinHoho;
        private System.Windows.Forms.Label label4;
    }
}

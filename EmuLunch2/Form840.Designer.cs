namespace Emu.Bento
{
    partial class Form840
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdoRecalc = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoFull = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoDiff = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.dtpHakkobi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.kaikeiRenkeiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kaikeiRenkeiBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(144, 146);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOutput.AutoSize = true;
            this.btnOutput.Location = new System.Drawing.Point(35, 146);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(103, 25);
            this.btnOutput.TabIndex = 1;
            this.btnOutput.Text = "出力(F10)";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.dtpHakkobi);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 128);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "出力条件";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rdoFull);
            this.groupBox2.Controls.Add(this.rdoDiff);
            this.groupBox2.Controls.Add(this.rdoRecalc);
            this.groupBox2.Location = new System.Drawing.Point(9, 51);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 71);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "出力対象";
            // 
            // rdoRecalc
            // 
            this.rdoRecalc.AutoSize = true;
            this.rdoRecalc.Location = new System.Drawing.Point(6, 21);
            this.rdoRecalc.Name = "rdoRecalc";
            this.rdoRecalc.Size = new System.Drawing.Size(166, 19);
            this.rdoRecalc.TabIndex = 2;
            this.rdoRecalc.Text = "この日だけを再集計する";
            this.rdoRecalc.UseVisualStyleBackColor = true;
            // 
            // rdoFull
            // 
            this.rdoFull.AutoSize = true;
            this.rdoFull.Location = new System.Drawing.Point(6, 46);
            this.rdoFull.Name = "rdoFull";
            this.rdoFull.Size = new System.Drawing.Size(146, 19);
            this.rdoFull.TabIndex = 1;
            this.rdoFull.Text = "履歴から再出力する";
            this.rdoFull.UseVisualStyleBackColor = true;
            // 
            // rdoDiff
            // 
            this.rdoDiff.AutoSize = true;
            this.rdoDiff.Checked = true;
            this.rdoDiff.Location = new System.Drawing.Point(6, 21);
            this.rdoDiff.Name = "rdoDiff";
            this.rdoDiff.Size = new System.Drawing.Size(120, 19);
            this.rdoDiff.TabIndex = 0;
            this.rdoDiff.TabStop = true;
            this.rdoDiff.Text = "差分を出力する";
            this.rdoDiff.UseVisualStyleBackColor = true;
            // 
            // dtpHakkobi
            // 
            this.dtpHakkobi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.kaikeiRenkeiBindingSource, "出力対象日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpHakkobi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpHakkobi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpHakkobi.Location = new System.Drawing.Point(95, 22);
            this.dtpHakkobi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpHakkobi.Name = "dtpHakkobi";
            this.dtpHakkobi.Size = new System.Drawing.Size(152, 22);
            this.dtpHakkobi.TabIndex = 1;
            // 
            // kaikeiRenkeiBindingSource
            // 
            this.kaikeiRenkeiBindingSource.AllowNew = false;
            this.kaikeiRenkeiBindingSource.DataSource = typeof(Emu.Bento.Data.KaikeiRenkei);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "出力対象日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.kaikeiRenkeiBindingSource;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // Form840
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 183);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form840";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "会計連携CSV出力";
            this.Load += new System.EventHandler(this.Form840_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form840_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kaikeiRenkeiBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource kaikeiRenkeiBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private Common.Control.DateTimePickerA dtpHakkobi;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Common.Control.RadioButtonA rdoFull;
        private Common.Control.RadioButtonA rdoDiff;
        private Common.Control.RadioButtonA rdoRecalc;
    }
}

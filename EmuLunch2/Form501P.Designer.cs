namespace Emu.Bento
{
    partial class Form501P
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
            this.rdoSeikyu = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.rdoUriage = new Emu.Bento.Common.Control.RadioButtonA(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.dtpHakkobi = new Emu.Bento.Common.Control.DateTimePickerA();
            this.receiptPrintBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.receiptPrintBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(129, 109);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "終了(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.Location = new System.Drawing.Point(20, 109);
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
            this.groupBox1.Controls.Add(this.rdoSeikyu);
            this.groupBox1.Controls.Add(this.rdoUriage);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpHakkobi);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 91);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定";
            // 
            // rdoSeikyu
            // 
            this.rdoSeikyu.AutoSize = true;
            this.rdoSeikyu.Checked = true;
            this.rdoSeikyu.Location = new System.Drawing.Point(140, 58);
            this.rdoSeikyu.Name = "rdoSeikyu";
            this.rdoSeikyu.Size = new System.Drawing.Size(70, 19);
            this.rdoSeikyu.TabIndex = 4;
            this.rdoSeikyu.TabStop = true;
            this.rdoSeikyu.Text = "請求額";
            this.rdoSeikyu.UseVisualStyleBackColor = true;
            // 
            // rdoUriage
            // 
            this.rdoUriage.AutoSize = true;
            this.rdoUriage.Location = new System.Drawing.Point(64, 58);
            this.rdoUriage.Name = "rdoUriage";
            this.rdoUriage.Size = new System.Drawing.Size(70, 19);
            this.rdoUriage.TabIndex = 3;
            this.rdoUriage.Text = "売上額";
            this.rdoUriage.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "金額";
            // 
            // dtpHakkobi
            // 
            this.dtpHakkobi.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.receiptPrintBindingSource, "発行日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd）"));
            this.dtpHakkobi.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpHakkobi.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dtpHakkobi.Location = new System.Drawing.Point(64, 21);
            this.dtpHakkobi.Margin = new System.Windows.Forms.Padding(4);
            this.dtpHakkobi.Name = "dtpHakkobi";
            this.dtpHakkobi.Size = new System.Drawing.Size(152, 22);
            this.dtpHakkobi.TabIndex = 1;
            // 
            // receiptPrintBindingSource
            // 
            this.receiptPrintBindingSource.AllowNew = false;
            this.receiptPrintBindingSource.DataSource = typeof(Emu.Bento.Data.ReceiptPrint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "発行日";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.receiptPrintBindingSource;
            // 
            // Form501P
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 146);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form501P";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "領収書印刷ダイアログ";
            this.Load += new System.EventHandler(this.Form501P_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form501P_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.receiptPrintBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource receiptPrintBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private Common.Control.DateTimePickerA dtpHakkobi;
        private System.Windows.Forms.Label label1;
        private Common.Control.RadioButtonA rdoSeikyu;
        private Common.Control.RadioButtonA rdoUriage;
    }
}

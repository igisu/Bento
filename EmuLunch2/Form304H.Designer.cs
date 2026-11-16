namespace Emu.Bento
{
    partial class Form304H
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtSofusaki = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.tanzakuSealOrderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.txtMailAddress = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.txtUnit = new Emu.Bento.Common.Control.TextBoxAN(this.components);
            this.lblMai = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tanzakuSealOrderBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "シール注文数";
            // 
            // txtSofusaki
            // 
            this.txtSofusaki.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tanzakuSealOrderBindingSource, "シール送付先", true));
            this.errorProvider1.SetIconAlignment(this.txtSofusaki, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.txtSofusaki.ImeMode = System.Windows.Forms.ImeMode.On;
            this.txtSofusaki.Location = new System.Drawing.Point(109, 40);
            this.txtSofusaki.MaxLength = 500;
            this.txtSofusaki.Multiline = true;
            this.txtSofusaki.Name = "txtSofusaki";
            this.txtSofusaki.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSofusaki.Size = new System.Drawing.Size(427, 84);
            this.txtSofusaki.TabIndex = 5;
            // 
            // tanzakuSealOrderBindingSource
            // 
            this.tanzakuSealOrderBindingSource.DataSource = typeof(Emu.Bento.Data.TanzakuSealOrder);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "送付先";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(350, 158);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "発注する";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(446, 158);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 9;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.tanzakuSealOrderBindingSource;
            // 
            // txtMailAddress
            // 
            this.txtMailAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tanzakuSealOrderBindingSource, "メールアドレス", true));
            this.errorProvider1.SetIconAlignment(this.txtMailAddress, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
            this.txtMailAddress.Location = new System.Drawing.Point(109, 130);
            this.txtMailAddress.MaxLength = 100;
            this.txtMailAddress.Name = "txtMailAddress";
            this.txtMailAddress.Size = new System.Drawing.Size(426, 22);
            this.txtMailAddress.TabIndex = 7;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 133);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 15);
            this.label11.TabIndex = 6;
            this.label11.Text = "メールアドレス";
            // 
            // txtUnit
            // 
            this.txtUnit.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tanzakuSealOrderBindingSource, "注文数", true));
            this.txtUnit.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtUnit.Location = new System.Drawing.Point(109, 12);
            this.txtUnit.MaxLength = 2;
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(25, 22);
            this.txtUnit.TabIndex = 1;
            this.txtUnit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblMai
            // 
            this.lblMai.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.tanzakuSealOrderBindingSource, "注文単位枚数編集", true));
            this.lblMai.Location = new System.Drawing.Point(168, 15);
            this.lblMai.Name = "lblMai";
            this.lblMai.Size = new System.Drawing.Size(100, 15);
            this.lblMai.TabIndex = 3;
            this.lblMai.Text = "枚";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(140, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(22, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "×";
            // 
            // Form304H
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 195);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblMai);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtMailAddress);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtSofusaki);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form304H";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "シール発注";
            this.Load += new System.EventHandler(this.Form304H_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form304H_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.tanzakuSealOrderBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Common.Control.TextBoxA txtSofusaki;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label11;
        private Common.Control.TextBoxA txtMailAddress;
        private Common.Control.TextBoxAN txtUnit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblMai;
        private System.Windows.Forms.BindingSource tanzakuSealOrderBindingSource;
    }
}

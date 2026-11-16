namespace Emu.Bento
{
    partial class FormPrinterSelect
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
            this.printSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.chkDefault = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.cboPrinter = new Emu.Bento.Common.Control.ComboBoxA(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblReportTitle = new System.Windows.Forms.Label();
            this.chkPreview = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.printSettingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // printSettingsBindingSource
            // 
            this.printSettingsBindingSource.DataSource = typeof(Emu.Bento.Data.PrintSettings);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(233, 307);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(107, 25);
            this.btnQuit.TabIndex = 7;
            this.btnQuit.Text = "キャンセル(Esc)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(137, 307);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 25);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.printSettingsBindingSource;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "帳票に対する操作を選んでください";
            // 
            // chkDefault
            // 
            this.chkDefault.AutoSize = true;
            this.chkDefault.Location = new System.Drawing.Point(12, 204);
            this.chkDefault.Name = "chkDefault";
            this.chkDefault.Size = new System.Drawing.Size(283, 19);
            this.chkDefault.TabIndex = 5;
            this.chkDefault.Text = "この帳票についてはこの設定を常に適用する";
            this.chkDefault.UseVisualStyleBackColor = true;
            // 
            // cboPrinter
            // 
            this.cboPrinter.BackColor = System.Drawing.SystemColors.Control;
            this.cboPrinter.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.printSettingsBindingSource, "プリンタ名", true));
            this.cboPrinter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrinter.FormattingEnabled = true;
            this.cboPrinter.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.cboPrinter.Location = new System.Drawing.Point(29, 137);
            this.cboPrinter.Name = "cboPrinter";
            this.cboPrinter.Size = new System.Drawing.Size(290, 23);
            this.cboPrinter.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblReportTitle);
            this.groupBox1.Location = new System.Drawing.Point(12, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 52);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "帳票名";
            // 
            // lblReportTitle
            // 
            this.lblReportTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReportTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.printSettingsBindingSource, "帳票名", true));
            this.lblReportTitle.Location = new System.Drawing.Point(6, 24);
            this.lblReportTitle.Name = "lblReportTitle";
            this.lblReportTitle.Size = new System.Drawing.Size(316, 15);
            this.lblReportTitle.TabIndex = 0;
            this.lblReportTitle.Text = "帳票タイトル";
            // 
            // chkPreview
            // 
            this.chkPreview.AutoSize = true;
            this.chkPreview.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.printSettingsBindingSource, "プレビューフラグ", true));
            this.chkPreview.Location = new System.Drawing.Point(12, 175);
            this.chkPreview.Name = "chkPreview";
            this.chkPreview.Size = new System.Drawing.Size(146, 19);
            this.chkPreview.TabIndex = 8;
            this.chkPreview.Text = "プレビューを表示する";
            this.chkPreview.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(219, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "印刷を行うプリンタを選択してください";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(12, 235);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(6);
            this.label3.Size = new System.Drawing.Size(328, 58);
            this.label3.TabIndex = 10;
            this.label3.Text = "この設定は「帳票／プリンタ設定」からいつでも変更できます";
            // 
            // FormPrinterSelect
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnQuit;
            this.ClientSize = new System.Drawing.Size(352, 344);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cboPrinter);
            this.Controls.Add(this.chkDefault);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkPreview);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPrinterSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "プリンタ選択ダイアログ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrinterSelect_FormClosing);
            this.Load += new System.EventHandler(this.FormPrinterSelect_Load);
            this.Shown += new System.EventHandler(this.FormPrinterSelect_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.printSettingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource printSettingsBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblReportTitle;
        private Common.Control.ComboBoxA cboPrinter;
        private Common.Control.CheckBoxA chkDefault;
        private System.Windows.Forms.Label label1;
        private Common.Control.CheckBoxA chkPreview;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

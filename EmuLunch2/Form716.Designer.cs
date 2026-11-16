namespace Emu.Bento
{
    partial class Form716
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
            this.hiKokyakuTelNoCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label10 = new System.Windows.Forms.Label();
            this.lblTelNo = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblUpdateDate = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTel = new Emu.Bento.Common.Control.TextBoxA(this.components);
            this.btnQuery = new System.Windows.Forms.Button();
            this.lblNotFound = new System.Windows.Forms.Label();
            this.hiKokyakuTelNoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.hiKokyakuTelNoCriteriaBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hiKokyakuTelNoBindingSource)).BeginInit();
            this.SuspendLayout();
            //
            // hiKokyakuTelNoCriteriaBindingSource
            //
            this.hiKokyakuTelNoCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.HiKokyakuTelNo);
            //
            // label10
            //
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(18, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "電話番号";
            //
            // lblTelNo
            //
            this.lblTelNo.AutoEllipsis = true;
            this.lblTelNo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTelNo.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.hiKokyakuTelNoBindingSource, "電話番号", true));
            this.lblTelNo.Location = new System.Drawing.Point(91, 75);
            this.lblTelNo.Name = "lblTelNo";
            this.lblTelNo.Size = new System.Drawing.Size(129, 22);
            this.lblTelNo.TabIndex = 2;
            this.lblTelNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // label13
            //
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 107);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 15);
            this.label13.TabIndex = 3;
            this.label13.Text = "登録日時";
            //
            // lblUpdateDate
            //
            this.lblUpdateDate.AutoEllipsis = true;
            this.lblUpdateDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblUpdateDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.hiKokyakuTelNoBindingSource, "修正日", true, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, "yyyy/MM/dd（ddd） HH:mm:ss"));
            this.lblUpdateDate.Location = new System.Drawing.Point(91, 103);
            this.lblUpdateDate.Name = "lblUpdateDate";
            this.lblUpdateDate.Size = new System.Drawing.Size(180, 22);
            this.lblUpdateDate.TabIndex = 4;
            this.lblUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnDelete
            //
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(119, 162);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 25);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "削除(F11)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnQuit
            //
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.CausesValidation = false;
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(215, 162);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(90, 25);
            this.btnQuit.TabIndex = 7;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            //
            // groupBox1
            //
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnQuery);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtTel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(293, 55);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            //
            // label12
            //
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 15);
            this.label12.TabIndex = 0;
            this.label12.Text = "電話番号";
            //
            // txtTel
            //
            this.txtTel.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.hiKokyakuTelNoCriteriaBindingSource, "電話番号", true));
            this.txtTel.Location = new System.Drawing.Point(79, 21);
            this.txtTel.MaxLength = 15;
            this.txtTel.Name = "txtTel";
            this.txtTel.Size = new System.Drawing.Size(129, 22);
            this.txtTel.TabIndex = 1;
            //
            // btnQuery
            //
            this.btnQuery.AutoSize = true;
            this.btnQuery.Location = new System.Drawing.Point(214, 19);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(73, 25);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "検索(F5)";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            //
            // lblNotFound
            //
            this.lblNotFound.AutoSize = true;
            this.lblNotFound.Location = new System.Drawing.Point(18, 137);
            this.lblNotFound.Name = "lblNotFound";
            this.lblNotFound.Size = new System.Drawing.Size(126, 15);
            this.lblNotFound.TabIndex = 5;
            this.lblNotFound.Text = "見つかりませんでした";
            //
            // hiKokyakuTelNoBindingSource
            //
            this.hiKokyakuTelNoBindingSource.DataSource = typeof(Emu.Bento.Data.HiKokyakuTelNo);
            //
            // Form716
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 199);
            this.Controls.Add(this.lblNotFound);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblUpdateDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblTelNo);
            this.Controls.Add(this.label10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form716";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "非顧客電話番号削除";
            this.Load += new System.EventHandler(this.Form716_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form716_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.hiKokyakuTelNoCriteriaBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hiKokyakuTelNoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblTelNo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblUpdateDate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource hiKokyakuTelNoCriteriaBindingSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label12;
        private Common.Control.TextBoxA txtTel;
        private System.Windows.Forms.Label lblNotFound;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.BindingSource hiKokyakuTelNoBindingSource;
    }
}

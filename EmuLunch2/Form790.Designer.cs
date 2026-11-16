namespace Emu.Bento
{
    partial class Form790
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
            this.btnExport = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkCourseTable = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.masterExportCriteriaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.chkTantoKadenCourse = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkTanka = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkCourseGroup = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkCourse = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkTanto = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkBusyo = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkGroup = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkSyuyoBuhin = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkSyuyoSyohin = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkSyohin = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkTankaTekiyo = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.chkTokuisaki = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chkOutputHeader = new Emu.Bento.Common.Control.CheckBoxA(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.masterExportCriteriaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.AutoSize = true;
            this.btnQuit.CausesValidation = false;
            this.btnQuit.Location = new System.Drawing.Point(582, 143);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(103, 25);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "終了(F12)";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.AutoSize = true;
            this.btnExport.Location = new System.Drawing.Point(473, 143);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(103, 25);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "エクスポート";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkCourseTable);
            this.groupBox1.Controls.Add(this.chkTantoKadenCourse);
            this.groupBox1.Controls.Add(this.chkTanka);
            this.groupBox1.Controls.Add(this.chkCourseGroup);
            this.groupBox1.Controls.Add(this.chkCourse);
            this.groupBox1.Controls.Add(this.chkTanto);
            this.groupBox1.Controls.Add(this.chkBusyo);
            this.groupBox1.Controls.Add(this.chkGroup);
            this.groupBox1.Controls.Add(this.chkSyuyoBuhin);
            this.groupBox1.Controls.Add(this.chkSyuyoSyohin);
            this.groupBox1.Controls.Add(this.chkSyohin);
            this.groupBox1.Controls.Add(this.chkTankaTekiyo);
            this.groupBox1.Controls.Add(this.chkTokuisaki);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(673, 125);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "対象指定";
            // 
            // chkCourseTable
            // 
            this.chkCourseTable.AutoSize = true;
            this.chkCourseTable.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "コーステーブル", true));
            this.chkCourseTable.Location = new System.Drawing.Point(560, 21);
            this.chkCourseTable.Name = "chkCourseTable";
            this.chkCourseTable.Size = new System.Drawing.Size(105, 19);
            this.chkCourseTable.TabIndex = 12;
            this.chkCourseTable.Text = "コーステーブル";
            this.chkCourseTable.UseVisualStyleBackColor = true;
            // 
            // masterExportCriteriaBindingSource
            // 
            this.masterExportCriteriaBindingSource.AllowNew = false;
            this.masterExportCriteriaBindingSource.DataSource = typeof(Emu.Bento.Data.MasterExportCriteria);
            // 
            // chkTantoKadenCourse
            // 
            this.chkTantoKadenCourse.AutoSize = true;
            this.chkTantoKadenCourse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "担当者架電コースマスタ", true));
            this.chkTantoKadenCourse.Location = new System.Drawing.Point(374, 96);
            this.chkTantoKadenCourse.Name = "chkTantoKadenCourse";
            this.chkTantoKadenCourse.Size = new System.Drawing.Size(166, 19);
            this.chkTantoKadenCourse.TabIndex = 11;
            this.chkTantoKadenCourse.Text = "担当者架電コースマスタ";
            this.chkTantoKadenCourse.UseVisualStyleBackColor = true;
            // 
            // chkTanka
            // 
            this.chkTanka.AutoSize = true;
            this.chkTanka.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "得意先単価マスタ", true));
            this.chkTanka.Location = new System.Drawing.Point(6, 71);
            this.chkTanka.Name = "chkTanka";
            this.chkTanka.Size = new System.Drawing.Size(133, 19);
            this.chkTanka.TabIndex = 2;
            this.chkTanka.Text = "得意先単価マスタ";
            this.chkTanka.UseVisualStyleBackColor = true;
            // 
            // chkCourseGroup
            // 
            this.chkCourseGroup.AutoSize = true;
            this.chkCourseGroup.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "コース別グループマスタ", true));
            this.chkCourseGroup.Location = new System.Drawing.Point(374, 46);
            this.chkCourseGroup.Name = "chkCourseGroup";
            this.chkCourseGroup.Size = new System.Drawing.Size(153, 19);
            this.chkCourseGroup.TabIndex = 9;
            this.chkCourseGroup.Text = "コース別グループマスタ";
            this.chkCourseGroup.UseVisualStyleBackColor = true;
            // 
            // chkCourse
            // 
            this.chkCourse.AutoSize = true;
            this.chkCourse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "コースマスタ", true));
            this.chkCourse.Location = new System.Drawing.Point(374, 21);
            this.chkCourse.Name = "chkCourse";
            this.chkCourse.Size = new System.Drawing.Size(91, 19);
            this.chkCourse.TabIndex = 8;
            this.chkCourse.Text = "コースマスタ";
            this.chkCourse.UseVisualStyleBackColor = true;
            // 
            // chkTanto
            // 
            this.chkTanto.AutoSize = true;
            this.chkTanto.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "担当者マスタ", true));
            this.chkTanto.Location = new System.Drawing.Point(374, 71);
            this.chkTanto.Name = "chkTanto";
            this.chkTanto.Size = new System.Drawing.Size(103, 19);
            this.chkTanto.TabIndex = 10;
            this.chkTanto.Text = "担当者マスタ";
            this.chkTanto.UseVisualStyleBackColor = true;
            // 
            // chkBusyo
            // 
            this.chkBusyo.AutoSize = true;
            this.chkBusyo.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "部署マスタ", true));
            this.chkBusyo.Location = new System.Drawing.Point(6, 21);
            this.chkBusyo.Name = "chkBusyo";
            this.chkBusyo.Size = new System.Drawing.Size(88, 19);
            this.chkBusyo.TabIndex = 0;
            this.chkBusyo.Text = "部署マスタ";
            this.chkBusyo.UseVisualStyleBackColor = true;
            // 
            // chkGroup
            // 
            this.chkGroup.AutoSize = true;
            this.chkGroup.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "商品グループマスタ", true));
            this.chkGroup.Location = new System.Drawing.Point(192, 46);
            this.chkGroup.Name = "chkGroup";
            this.chkGroup.Size = new System.Drawing.Size(135, 19);
            this.chkGroup.TabIndex = 5;
            this.chkGroup.Text = "商品グループマスタ";
            this.chkGroup.UseVisualStyleBackColor = true;
            // 
            // chkSyuyoBuhin
            // 
            this.chkSyuyoBuhin.AutoSize = true;
            this.chkSyuyoBuhin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "主要部品リストマスタ", true));
            this.chkSyuyoBuhin.Location = new System.Drawing.Point(192, 96);
            this.chkSyuyoBuhin.Name = "chkSyuyoBuhin";
            this.chkSyuyoBuhin.Size = new System.Drawing.Size(149, 19);
            this.chkSyuyoBuhin.TabIndex = 7;
            this.chkSyuyoBuhin.Text = "主要部品リストマスタ";
            this.chkSyuyoBuhin.UseVisualStyleBackColor = true;
            // 
            // chkSyuyoSyohin
            // 
            this.chkSyuyoSyohin.AutoSize = true;
            this.chkSyuyoSyohin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "主要商品リストマスタ", true));
            this.chkSyuyoSyohin.Location = new System.Drawing.Point(192, 71);
            this.chkSyuyoSyohin.Name = "chkSyuyoSyohin";
            this.chkSyuyoSyohin.Size = new System.Drawing.Size(149, 19);
            this.chkSyuyoSyohin.TabIndex = 6;
            this.chkSyuyoSyohin.Text = "主要商品リストマスタ";
            this.chkSyuyoSyohin.UseVisualStyleBackColor = true;
            // 
            // chkSyohin
            // 
            this.chkSyohin.AutoSize = true;
            this.chkSyohin.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "商品マスタ", true));
            this.chkSyohin.Location = new System.Drawing.Point(192, 21);
            this.chkSyohin.Name = "chkSyohin";
            this.chkSyohin.Size = new System.Drawing.Size(88, 19);
            this.chkSyohin.TabIndex = 4;
            this.chkSyohin.Text = "商品マスタ";
            this.chkSyohin.UseVisualStyleBackColor = true;
            // 
            // chkTankaTekiyo
            // 
            this.chkTankaTekiyo.AutoSize = true;
            this.chkTankaTekiyo.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "得意先単価適用マスタ", true));
            this.chkTankaTekiyo.Location = new System.Drawing.Point(6, 96);
            this.chkTankaTekiyo.Name = "chkTankaTekiyo";
            this.chkTankaTekiyo.Size = new System.Drawing.Size(163, 19);
            this.chkTankaTekiyo.TabIndex = 3;
            this.chkTankaTekiyo.Text = "得意先単価適用マスタ";
            this.chkTankaTekiyo.UseVisualStyleBackColor = true;
            // 
            // chkTokuisaki
            // 
            this.chkTokuisaki.AutoSize = true;
            this.chkTokuisaki.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.masterExportCriteriaBindingSource, "得意先マスタ", true));
            this.chkTokuisaki.Location = new System.Drawing.Point(6, 46);
            this.chkTokuisaki.Name = "chkTokuisaki";
            this.chkTokuisaki.Size = new System.Drawing.Size(103, 19);
            this.chkTokuisaki.TabIndex = 1;
            this.chkTokuisaki.Text = "得意先マスタ";
            this.chkTokuisaki.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataSource = this.masterExportCriteriaBindingSource;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "csv";
            this.saveFileDialog1.Filter = "CSVファイル(*.csv)|*.csv|すべてのファイル(*.*)|*.*";
            this.saveFileDialog1.Title = "名前を付けて保存";
            // 
            // chkOutputHeader
            // 
            this.chkOutputHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkOutputHeader.AutoSize = true;
            this.chkOutputHeader.Location = new System.Drawing.Point(18, 147);
            this.chkOutputHeader.Name = "chkOutputHeader";
            this.chkOutputHeader.Size = new System.Drawing.Size(123, 19);
            this.chkOutputHeader.TabIndex = 1;
            this.chkOutputHeader.Text = "ヘッダを出力する";
            this.chkOutputHeader.UseVisualStyleBackColor = true;
            // 
            // Form790
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 180);
            this.Controls.Add(this.chkOutputHeader);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form790";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "マスタエクスポート";
            this.Load += new System.EventHandler(this.Form790_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form790_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.masterExportCriteriaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.BindingSource masterExportCriteriaBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Common.Control.CheckBoxA chkTokuisaki;
        private Common.Control.CheckBoxA chkCourseGroup;
        private Common.Control.CheckBoxA chkCourse;
        private Common.Control.CheckBoxA chkTanto;
        private Common.Control.CheckBoxA chkBusyo;
        private Common.Control.CheckBoxA chkGroup;
        private Common.Control.CheckBoxA chkSyuyoBuhin;
        private Common.Control.CheckBoxA chkSyuyoSyohin;
        private Common.Control.CheckBoxA chkSyohin;
        private Common.Control.CheckBoxA chkTankaTekiyo;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Common.Control.CheckBoxA chkOutputHeader;
        private Common.Control.CheckBoxA chkTanka;
        private Common.Control.CheckBoxA chkTantoKadenCourse;
        private Common.Control.CheckBoxA chkCourseTable;
    }
}

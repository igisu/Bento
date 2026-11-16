namespace com.emukikaku.CtiService
{
    partial class frmTelList
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTelList));
            this.dgvList = new System.Windows.Forms.DataGridView();
            this.colCustomerNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTelNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWhereCustomerTelNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtWhereCustomerName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtWhereCustomerNo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvList
            // 
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.AllowUserToDeleteRows = false;
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCustomerNo,
            this.colCustomerName,
            this.colTelNo});
            this.dgvList.Location = new System.Drawing.Point(16, 146);
            this.dgvList.Margin = new System.Windows.Forms.Padding(4);
            this.dgvList.MultiSelect = false;
            this.dgvList.Name = "dgvList";
            this.dgvList.RowTemplate.Height = 21;
            this.dgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.Size = new System.Drawing.Size(611, 333);
            this.dgvList.TabIndex = 1;
            this.dgvList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvList_CellDoubleClick);
            this.dgvList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvList_KeyDown);
            // 
            // colCustomerNo
            // 
            this.colCustomerNo.DataPropertyName = "CUSTOMER_NO";
            this.colCustomerNo.HeaderText = "顧客コード";
            this.colCustomerNo.Name = "colCustomerNo";
            this.colCustomerNo.ReadOnly = true;
            this.colCustomerNo.Width = 150;
            // 
            // colCustomerName
            // 
            this.colCustomerName.DataPropertyName = "CUSTOMER_NAME";
            this.colCustomerName.HeaderText = "顧客名";
            this.colCustomerName.Name = "colCustomerName";
            this.colCustomerName.ReadOnly = true;
            this.colCustomerName.Width = 250;
            // 
            // colTelNo
            // 
            this.colTelNo.DataPropertyName = "TELNO";
            this.colTelNo.HeaderText = "電話番号";
            this.colTelNo.Name = "colTelNo";
            this.colTelNo.ReadOnly = true;
            this.colTelNo.Width = 150;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtWhereCustomerTelNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtWhereCustomerName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtWhereCustomerNo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(16, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(611, 125);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // txtWhereCustomerTelNo
            // 
            this.txtWhereCustomerTelNo.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtWhereCustomerTelNo.Location = new System.Drawing.Point(120, 84);
            this.txtWhereCustomerTelNo.Name = "txtWhereCustomerTelNo";
            this.txtWhereCustomerTelNo.Size = new System.Drawing.Size(161, 23);
            this.txtWhereCustomerTelNo.TabIndex = 5;
            this.txtWhereCustomerTelNo.Text = "090-4188-5889";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "電話番号";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(506, 84);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(98, 23);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "検　索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtWhereCustomerName
            // 
            this.txtWhereCustomerName.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.txtWhereCustomerName.Location = new System.Drawing.Point(120, 55);
            this.txtWhereCustomerName.Name = "txtWhereCustomerName";
            this.txtWhereCustomerName.Size = new System.Drawing.Size(380, 23);
            this.txtWhereCustomerName.TabIndex = 3;
            this.txtWhereCustomerName.Text = "090-4188-5889";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 58);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "顧客名";
            // 
            // txtWhereCustomerNo
            // 
            this.txtWhereCustomerNo.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtWhereCustomerNo.Location = new System.Drawing.Point(120, 26);
            this.txtWhereCustomerNo.Name = "txtWhereCustomerNo";
            this.txtWhereCustomerNo.Size = new System.Drawing.Size(123, 23);
            this.txtWhereCustomerNo.TabIndex = 1;
            this.txtWhereCustomerNo.Text = "090-4188-5889";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 29);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "顧客コード";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(418, 486);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(533, 486);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmTelList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(643, 521);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvList);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmTelList";
            this.Text = "顧客電話番号検索";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.all_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtWhereCustomerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWhereCustomerNo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtWhereCustomerTelNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCustomerNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTelNo;
    }
}
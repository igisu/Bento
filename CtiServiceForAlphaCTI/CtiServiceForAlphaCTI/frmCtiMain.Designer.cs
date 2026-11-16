namespace com.emukikaku.CtiServiceAlphaCTI
{
    partial class frmCtiMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCtiMain));
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCallTel = new System.Windows.Forms.TextBox();
            this.btnCall = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCallName = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTelStatus = new System.Windows.Forms.TextBox();
            this.picTelStatus = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtIncommingTel = new System.Windows.Forms.TextBox();
            this.txtIncommingName = new System.Windows.Forms.TextBox();
            this.linkCall = new System.Windows.Forms.LinkLabel();
            this.bgwUdpRecieve = new System.ComponentModel.BackgroundWorker();
            this.lblDebugInfo = new System.Windows.Forms.Label();
            this.bgwUdpRecieveSoftphone = new System.ComponentModel.BackgroundWorker();
            this.bgwWcfConnect = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTelStatus)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDebug
            // 
            this.txtDebug.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDebug.BackColor = System.Drawing.Color.DarkBlue;
            this.txtDebug.ForeColor = System.Drawing.Color.White;
            this.txtDebug.Location = new System.Drawing.Point(7, 272);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDebug.Size = new System.Drawing.Size(409, 303);
            this.txtDebug.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtCallTel);
            this.panel1.Controls.Add(this.btnCall);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtCallName);
            this.panel1.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.panel1.Location = new System.Drawing.Point(7, 186);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(414, 71);
            this.panel1.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 6;
            this.label6.Text = "発信番号";
            // 
            // txtCallTel
            // 
            this.txtCallTel.Location = new System.Drawing.Point(77, 8);
            this.txtCallTel.Name = "txtCallTel";
            this.txtCallTel.Size = new System.Drawing.Size(120, 23);
            this.txtCallTel.TabIndex = 5;
            // 
            // btnCall
            // 
            this.btnCall.Image = global::com.emukikaku.CtiServiceAlphaCTI.Properties.Resources.telephone_black;
            this.btnCall.Location = new System.Drawing.Point(210, 2);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(40, 33);
            this.btnCall.TabIndex = 6;
            this.btnCall.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "発信先";
            // 
            // txtCallName
            // 
            this.txtCallName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCallName.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtCallName.Location = new System.Drawing.Point(77, 38);
            this.txtCallName.Name = "txtCallName";
            this.txtCallName.ReadOnly = true;
            this.txtCallName.Size = new System.Drawing.Size(313, 23);
            this.txtCallName.TabIndex = 7;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Location = new System.Drawing.Point(56, 165);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(365, 10);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel2.Controls.Add(this.lblTelStatus);
            this.panel2.Controls.Add(this.picTelStatus);
            this.panel2.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.panel2.Location = new System.Drawing.Point(7, 101);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(414, 54);
            this.panel2.TabIndex = 2;
            // 
            // lblTelStatus
            // 
            this.lblTelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTelStatus.BackColor = System.Drawing.Color.LightYellow;
            this.lblTelStatus.Location = new System.Drawing.Point(77, 15);
            this.lblTelStatus.Name = "lblTelStatus";
            this.lblTelStatus.ReadOnly = true;
            this.lblTelStatus.Size = new System.Drawing.Size(313, 23);
            this.lblTelStatus.TabIndex = 3;
            this.lblTelStatus.Text = "待機中";
            this.lblTelStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // picTelStatus
            // 
            this.picTelStatus.Image = global::com.emukikaku.CtiServiceAlphaCTI.Properties.Resources.telephone_gray;
            this.picTelStatus.Location = new System.Drawing.Point(5, 5);
            this.picTelStatus.Name = "picTelStatus";
            this.picTelStatus.Size = new System.Drawing.Size(56, 43);
            this.picTelStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picTelStatus.TabIndex = 16;
            this.picTelStatus.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Location = new System.Drawing.Point(56, 9);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(365, 10);
            this.groupBox5.TabIndex = 18;
            this.groupBox5.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("MS UI Gothic", 13F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(4, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 18);
            this.label8.TabIndex = 17;
            this.label8.Text = "着信";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.txtIncommingTel);
            this.panel3.Controls.Add(this.txtIncommingName);
            this.panel3.Font = new System.Drawing.Font("MS UI Gothic", 12F);
            this.panel3.Location = new System.Drawing.Point(7, 29);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(414, 67);
            this.panel3.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 16);
            this.label9.TabIndex = 9;
            this.label9.Text = "相手名";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "着信番号";
            // 
            // txtIncommingTel
            // 
            this.txtIncommingTel.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtIncommingTel.Location = new System.Drawing.Point(80, 9);
            this.txtIncommingTel.Name = "txtIncommingTel";
            this.txtIncommingTel.ReadOnly = true;
            this.txtIncommingTel.Size = new System.Drawing.Size(120, 23);
            this.txtIncommingTel.TabIndex = 1;
            // 
            // txtIncommingName
            // 
            this.txtIncommingName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIncommingName.Location = new System.Drawing.Point(79, 36);
            this.txtIncommingName.Name = "txtIncommingName";
            this.txtIncommingName.ReadOnly = true;
            this.txtIncommingName.Size = new System.Drawing.Size(311, 23);
            this.txtIncommingName.TabIndex = 2;
            // 
            // linkCall
            // 
            this.linkCall.AutoSize = true;
            this.linkCall.Font = new System.Drawing.Font("MS UI Gothic", 13F, System.Drawing.FontStyle.Bold);
            this.linkCall.Location = new System.Drawing.Point(1, 162);
            this.linkCall.Name = "linkCall";
            this.linkCall.Size = new System.Drawing.Size(58, 18);
            this.linkCall.TabIndex = 3;
            this.linkCall.TabStop = true;
            this.linkCall.Text = " 発信 ";
            this.linkCall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCall_LinkClicked);
            // 
            // bgwUdpRecieve
            // 
            this.bgwUdpRecieve.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUdpRecieve_DoWork);
            // 
            // lblDebugInfo
            // 
            this.lblDebugInfo.AutoSize = true;
            this.lblDebugInfo.BackColor = System.Drawing.Color.Yellow;
            this.lblDebugInfo.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblDebugInfo.ForeColor = System.Drawing.Color.Red;
            this.lblDebugInfo.Location = new System.Drawing.Point(65, 5);
            this.lblDebugInfo.Name = "lblDebugInfo";
            this.lblDebugInfo.Size = new System.Drawing.Size(127, 16);
            this.lblDebugInfo.TabIndex = 20;
            this.lblDebugInfo.Text = "CTIデバッグモード";
            // 
            // bgwUdpRecieveSoftphone
            // 
            this.bgwUdpRecieveSoftphone.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUdpRecieveSoftphone_DoWork);
            // 
            // bgwWcfConnect
            // 
            this.bgwWcfConnect.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwWcfConnect_DoWork);
            // 
            // frmCtiMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(428, 265);
            this.Controls.Add(this.lblDebugInfo);
            this.Controls.Add(this.linkCall);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtDebug);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmCtiMain";
            this.Text = "CtiService(αCTI)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCtiMain_FormClosed);
            this.Load += new System.EventHandler(this.frmCtiMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTelStatus)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDebug;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCallTel;
        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCallName;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox lblTelStatus;
        private System.Windows.Forms.PictureBox picTelStatus;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtIncommingTel;
        private System.Windows.Forms.TextBox txtIncommingName;
        private System.Windows.Forms.LinkLabel linkCall;
        private System.ComponentModel.BackgroundWorker bgwUdpRecieve;
        private System.Windows.Forms.Label lblDebugInfo;
        private System.ComponentModel.BackgroundWorker bgwUdpRecieveSoftphone;
        private System.ComponentModel.BackgroundWorker bgwWcfConnect;
    }
}


namespace com.emukikaku.CtiService
{
    partial class frmCtiMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCtiMain));
            this.label1 = new System.Windows.Forms.Label();
            this.lblNaisenBangou = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCallerName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDisconnection = new System.Windows.Forms.Button();
            this.btnHoldAck = new System.Windows.Forms.Button();
            this.btnAckInnerLine = new System.Windows.Forms.Button();
            this.btnAck05 = new System.Windows.Forms.Button();
            this.btnAck04 = new System.Windows.Forms.Button();
            this.btnAck03 = new System.Windows.Forms.Button();
            this.btnAck02 = new System.Windows.Forms.Button();
            this.btnAck01 = new System.Windows.Forms.Button();
            this.lblCallerID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCallToNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCallToName = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCall = new System.Windows.Forms.Button();
            this.bgwUdpRecieve = new System.ComponentModel.BackgroundWorker();
            this.axAspL1 = new AxIP3DMCTLLib.AxAspL();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axAspL1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "内線番号";
            // 
            // lblNaisenBangou
            // 
            this.lblNaisenBangou.BackColor = System.Drawing.Color.LightYellow;
            this.lblNaisenBangou.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNaisenBangou.Location = new System.Drawing.Point(95, 17);
            this.lblNaisenBangou.Name = "lblNaisenBangou";
            this.lblNaisenBangou.Size = new System.Drawing.Size(60, 22);
            this.lblNaisenBangou.TabIndex = 2;
            this.lblNaisenBangou.Text = "114";
            this.lblNaisenBangou.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCallerName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnDisconnection);
            this.groupBox1.Controls.Add(this.btnHoldAck);
            this.groupBox1.Controls.Add(this.btnAckInnerLine);
            this.groupBox1.Controls.Add(this.btnAck05);
            this.groupBox1.Controls.Add(this.btnAck04);
            this.groupBox1.Controls.Add(this.btnAck03);
            this.groupBox1.Controls.Add(this.btnAck02);
            this.groupBox1.Controls.Add(this.btnAck01);
            this.groupBox1.Controls.Add(this.lblCallerID);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(19, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(646, 286);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "受信";
            // 
            // lblCallerName
            // 
            this.lblCallerName.BackColor = System.Drawing.Color.LightYellow;
            this.lblCallerName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCallerName.Location = new System.Drawing.Point(324, 25);
            this.lblCallerName.Name = "lblCallerName";
            this.lblCallerName.Size = new System.Drawing.Size(309, 22);
            this.lblCallerName.TabIndex = 6;
            this.lblCallerName.Text = "株式会社　ローソン本社　ジャックスカード専用";
            this.lblCallerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(261, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "相手名";
            // 
            // btnDisconnection
            // 
            this.btnDisconnection.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnDisconnection.Font = new System.Drawing.Font("ＭＳ ゴシック", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDisconnection.ForeColor = System.Drawing.Color.MidnightBlue;
            this.btnDisconnection.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDisconnection.Location = new System.Drawing.Point(449, 181);
            this.btnDisconnection.Name = "btnDisconnection";
            this.btnDisconnection.Size = new System.Drawing.Size(184, 85);
            this.btnDisconnection.TabIndex = 9;
            this.btnDisconnection.Text = "切　断";
            this.btnDisconnection.UseVisualStyleBackColor = false;
            this.btnDisconnection.Click += new System.EventHandler(this.btnDisconnection_Click);
            // 
            // btnHoldAck
            // 
            this.btnHoldAck.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnHoldAck.Font = new System.Drawing.Font("ＭＳ ゴシック", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnHoldAck.ForeColor = System.Drawing.Color.Red;
            this.btnHoldAck.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHoldAck.Location = new System.Drawing.Point(239, 181);
            this.btnHoldAck.Name = "btnHoldAck";
            this.btnHoldAck.Size = new System.Drawing.Size(184, 85);
            this.btnHoldAck.TabIndex = 8;
            this.btnHoldAck.Text = "保留解除";
            this.btnHoldAck.UseVisualStyleBackColor = false;
            this.btnHoldAck.Click += new System.EventHandler(this.btnHoldAck_Click);
            // 
            // btnAckInnerLine
            // 
            this.btnAckInnerLine.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnAckInnerLine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAckInnerLine.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAckInnerLine.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAckInnerLine.Location = new System.Drawing.Point(23, 181);
            this.btnAckInnerLine.Name = "btnAckInnerLine";
            this.btnAckInnerLine.Size = new System.Drawing.Size(97, 85);
            this.btnAckInnerLine.TabIndex = 7;
            this.btnAckInnerLine.Text = "内線";
            this.btnAckInnerLine.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAckInnerLine.UseVisualStyleBackColor = false;
            this.btnAckInnerLine.Click += new System.EventHandler(this.btnAckInnerLine_Click);
            // 
            // btnAck05
            // 
            this.btnAck05.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnAck05.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAck05.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAck05.Image = global::com.emukikaku.CtiService.Properties.Resources.telephone_green;
            this.btnAck05.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAck05.Location = new System.Drawing.Point(536, 65);
            this.btnAck05.Name = "btnAck05";
            this.btnAck05.Size = new System.Drawing.Size(97, 85);
            this.btnAck05.TabIndex = 6;
            this.btnAck05.Text = "着信5";
            this.btnAck05.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAck05.UseVisualStyleBackColor = false;
            this.btnAck05.Click += new System.EventHandler(this.btnAck_Click);
            // 
            // btnAck04
            // 
            this.btnAck04.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnAck04.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAck04.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAck04.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAck04.Location = new System.Drawing.Point(409, 65);
            this.btnAck04.Name = "btnAck04";
            this.btnAck04.Size = new System.Drawing.Size(97, 85);
            this.btnAck04.TabIndex = 5;
            this.btnAck04.Text = "着信4";
            this.btnAck04.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAck04.UseVisualStyleBackColor = false;
            this.btnAck04.Click += new System.EventHandler(this.btnAck_Click);
            // 
            // btnAck03
            // 
            this.btnAck03.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnAck03.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAck03.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAck03.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAck03.Location = new System.Drawing.Point(280, 65);
            this.btnAck03.Name = "btnAck03";
            this.btnAck03.Size = new System.Drawing.Size(97, 85);
            this.btnAck03.TabIndex = 4;
            this.btnAck03.Text = "着信3";
            this.btnAck03.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAck03.UseVisualStyleBackColor = false;
            this.btnAck03.Click += new System.EventHandler(this.btnAck_Click);
            // 
            // btnAck02
            // 
            this.btnAck02.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnAck02.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAck02.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAck02.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAck02.Location = new System.Drawing.Point(151, 65);
            this.btnAck02.Name = "btnAck02";
            this.btnAck02.Size = new System.Drawing.Size(97, 85);
            this.btnAck02.TabIndex = 3;
            this.btnAck02.Text = "着信2";
            this.btnAck02.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAck02.UseVisualStyleBackColor = false;
            this.btnAck02.Click += new System.EventHandler(this.btnAck_Click);
            // 
            // btnAck01
            // 
            this.btnAck01.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnAck01.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAck01.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnAck01.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAck01.Location = new System.Drawing.Point(23, 65);
            this.btnAck01.Name = "btnAck01";
            this.btnAck01.Size = new System.Drawing.Size(97, 85);
            this.btnAck01.TabIndex = 2;
            this.btnAck01.Text = "着信1";
            this.btnAck01.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAck01.UseVisualStyleBackColor = false;
            this.btnAck01.Click += new System.EventHandler(this.btnAck_Click);
            // 
            // lblCallerID
            // 
            this.lblCallerID.BackColor = System.Drawing.Color.LightYellow;
            this.lblCallerID.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCallerID.Location = new System.Drawing.Point(99, 25);
            this.lblCallerID.Name = "lblCallerID";
            this.lblCallerID.Size = new System.Drawing.Size(130, 22);
            this.lblCallerID.TabIndex = 1;
            this.lblCallerID.Text = "08027465581";
            this.lblCallerID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "相手番号";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtCallToNumber);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblCallToName);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btnCall);
            this.groupBox2.Location = new System.Drawing.Point(19, 361);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(646, 115);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "架電";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Silver;
            this.label8.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(418, 75);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 16);
            this.label8.TabIndex = 4;
            this.label8.Text = "F8:検索";
            // 
            // txtCallToNumber
            // 
            this.txtCallToNumber.Location = new System.Drawing.Point(99, 72);
            this.txtCallToNumber.Name = "txtCallToNumber";
            this.txtCallToNumber.Size = new System.Drawing.Size(312, 23);
            this.txtCallToNumber.TabIndex = 3;
            this.txtCallToNumber.Text = "090-4188-5889";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 75);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 16);
            this.label7.TabIndex = 2;
            this.label7.Text = "電話番号";
            // 
            // lblCallToName
            // 
            this.lblCallToName.BackColor = System.Drawing.Color.LightYellow;
            this.lblCallToName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCallToName.Location = new System.Drawing.Point(99, 28);
            this.lblCallToName.Name = "lblCallToName";
            this.lblCallToName.Size = new System.Drawing.Size(378, 22);
            this.lblCallToName.TabIndex = 1;
            this.lblCallToName.Text = "株式会社　ローソン本社　ジャックスカード専用";
            this.lblCallToName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 31);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "架電先名";
            // 
            // btnCall
            // 
            this.btnCall.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnCall.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCall.Image = global::com.emukikaku.CtiService.Properties.Resources.telephone_blue;
            this.btnCall.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCall.Location = new System.Drawing.Point(536, 15);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(97, 85);
            this.btnCall.TabIndex = 5;
            this.btnCall.Text = "架電";
            this.btnCall.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCall.UseVisualStyleBackColor = false;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // bgwUdpRecieve
            // 
            this.bgwUdpRecieve.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwUdpRecieve_DoWork);
            // 
            // axAspL1
            // 
            this.axAspL1.Enabled = true;
            this.axAspL1.Location = new System.Drawing.Point(637, 8);
            this.axAspL1.Name = "axAspL1";
            this.axAspL1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAspL1.OcxState")));
            this.axAspL1.Size = new System.Drawing.Size(28, 28);
            this.axAspL1.TabIndex = 5;
            this.axAspL1.EvLcsDialing += new AxIP3DMCTLLib._DAspLEvents_EvLcsDialingEventHandler(this.axAspLine_EvLcsDialing);
            this.axAspL1.EvLcsConnected += new AxIP3DMCTLLib._DAspLEvents_EvLcsConnectedEventHandler(this.axAspLine_EvLcsConnected);
            this.axAspL1.EvLcsOnHold += new AxIP3DMCTLLib._DAspLEvents_EvLcsOnHoldEventHandler(this.axAspLine_EvLcsOnHold);
            this.axAspL1.EvLcsUnknown += new AxIP3DMCTLLib._DAspLEvents_EvLcsUnknownEventHandler(this.axAspLine_EvLcsUnknown);
            this.axAspL1.EvLcsOffering += new AxIP3DMCTLLib._DAspLEvents_EvLcsOfferingEventHandler(this.axAspLine_EvLcsOffering);
            this.axAspL1.EvLcsDisconnected += new AxIP3DMCTLLib._DAspLEvents_EvLcsDisconnectedEventHandler(this.axAspLine_EvLcsDisconnected);
            this.axAspL1.EvLcsSpecialInfo += new AxIP3DMCTLLib._DAspLEvents_EvLcsSpecialInfoEventHandler(this.axAspLine_EvLcsSpecialInfo);
            // 
            // frmCtiMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(681, 489);
            this.Controls.Add(this.axAspL1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblNaisenBangou);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmCtiMain";
            this.Text = "CTIメイン(WX)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCtiMain_FormClosed);
            this.Load += new System.EventHandler(this.frmCtiMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCtiMain_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axAspL1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNaisenBangou;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblCallerID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAck01;
        private System.Windows.Forms.Button btnAck05;
        private System.Windows.Forms.Button btnAck04;
        private System.Windows.Forms.Button btnAck03;
        private System.Windows.Forms.Button btnAck02;
        private System.Windows.Forms.Button btnDisconnection;
        private System.Windows.Forms.Button btnHoldAck;
        private System.Windows.Forms.Button btnAckInnerLine;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCallToNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCallToName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblCallerName;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.BackgroundWorker bgwUdpRecieve;
        private AxIP3DMCTLLib.AxAspL axAspL1;
    }
}
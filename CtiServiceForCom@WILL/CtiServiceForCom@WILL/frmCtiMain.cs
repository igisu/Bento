using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Microsoft.VisualBasic.ApplicationServices;

using System.Runtime.InteropServices;

namespace com.emukikaku.CtiServiceComWILL
{

    public partial class frmCtiMain : Form
    {

        #region Member

        private int _intDefaltHeight = 0;

        public IniFile _iniFile;
        public DBAccessor _dbAccessor;
        public bool _isCtiTrace = false;       // デバッグモード：ソフトフォンからのイベントを画面に出力する
        public bool _isCtiDbConnect = true;    // デバッグモード：DB接続しない（NDなどのやりとりだけ）

        private bool _runningEvent;
        
        public System.Net.Sockets.UdpClient udpClient;
        public System.Net.Sockets.UdpClient udpClientSoftphone;

        private bool _exitReceiveLoop;

        #endregion

        #region Enum
        private enum ENUM_TELEPHONE_STATE
        {
            NONE,
            NORMAL,
            INCOMING,
            ACK,
            DIALING
        }
        #endregion

        public frmCtiMain()
        {
            InitializeComponent();
        }

        private void frmCtiMain_Load(object sender, EventArgs e)
        {

            //this.TopMost = true;

            // ----- 初期化 ----- 
            _iniFile = new IniFile();
            _dbAccessor = new DBAccessor(_iniFile.DBSERVER, _iniFile.DBUSERID, _iniFile.DBPASSWORD, _iniFile.DBDATABASENAME);            
            _isCtiTrace = _iniFile.SOFTPHONETRACE;
            _isCtiDbConnect = _iniFile.DBCONNECT;

            //ポート指定してWait
            udpClient = new System.Net.Sockets.UdpClient(_iniFile.CTIRECEIVEPORT);
            udpClient.DontFragment = true;
            udpClient.EnableBroadcast = true;

            udpClientSoftphone = new System.Net.Sockets.UdpClient(_iniFile.CTISOFTPHONEANALYZEPORT);
            udpClientSoftphone.DontFragment = true;
            udpClientSoftphone.EnableBroadcast = true;

            // SoftPhone（Com@WILL）からいきなり、起動された場合
            string strArgs = "";
            string[] cmds = System.Environment.GetCommandLineArgs();
            foreach (string cmd in cmds)
            {
                strArgs += "＜" + cmd;
            }
            if (strArgs != "")
            {
                getAnalyzeSoftPhoneMessage(strArgs);
            }

            // ----- 画面回りの設定 ----- 
            _intDefaltHeight = this.Height;

            lblDebugInfo.Visible = false;
            lblDebugInfo.Text = "";
            if (_isCtiTrace == true || _isCtiDbConnect == false)
            {
                string strDebugInfo = "CTIデバッグモード ";
                if (_isCtiTrace == true) strDebugInfo += "(イベントトレース)  ";
                if (_isCtiDbConnect == false) strDebugInfo += "(DB接続なし)";
                lblDebugInfo.Text = strDebugInfo;
                lblDebugInfo.Visible = true;
            }

            if (_isCtiTrace == false)
            {
                this.MaximumSize = new Size(this.Width, _intDefaltHeight);
                this.MinimumSize = new Size(this.Width - 150, _intDefaltHeight - 80);
            } else {
                this.MaximumSize = new Size(0, 0);
                this.MinimumSize = new Size(0, 0);
            }

            this.Size = new Size(this.Width - 150, this.Height);
            linkCall_LinkClicked(null, null);

            if (isRunningEvent()) return;
            try
            {
                this.bgwUdpRecieve.RunWorkerAsync();
                this.bgwUdpRecieveSoftphone.RunWorkerAsync();
                _exitReceiveLoop = false;
            }
            catch (Exception ex)
            {
                showMessage(ex.ToString());
            }
            finally
            {
                quitEvent();
            }
        }

        #region メッセージ表示
        private DialogResult showMessage(string message)
        {
            return showMessage(message, MessageBoxButtons.OK);
        }

        private DialogResult showMessage(string message, MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, this.Text, buttons);
        }
        #endregion

        #region イベント処理
        private bool isRunningEvent()
        {
            if (_runningEvent) return true;
            _runningEvent = true;
            return false;
        }

        private void quitEvent()
        {
            _runningEvent = false;
        }
        #endregion

        #region FormClosed
        private void frmCtiMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isRunningEvent()) return;
            try
            {
                _exitReceiveLoop = true;
                if (udpClient != null)
                {
                    udpClient.Close();
                }
                if (udpClientSoftphone != null)
                {
                    udpClientSoftphone.Close();
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.ToString());
            }
            finally
            {
                quitEvent();
            }
        }
        #endregion

        #region 受信番号設定
        private void setCallerID(string callerID)
        {           
            txtIncommingTel.Text = callerID;
            txtIncommingName.Text = string.Empty;
            if (!txtIncommingTel.Text.Equals(string.Empty))
            {
                CtiServiceDataSet.TelListDataTable dt = readTelList(string.Empty, txtIncommingTel.Text);
                if (dt != null && dt.Count >= 1)
                {
                    txtIncommingName.Text = dt[0].CUSTOMER_NAME;
                }
            }
        }            
        #endregion

        private void setPicTelStatus(ENUM_TELEPHONE_STATE enmStatus) 
        {
            switch (enmStatus)
            {
         
                case ENUM_TELEPHONE_STATE.NONE:
                    picTelStatus.Image = Properties.Resources.telephone_gray;
                    lblTelStatus.ForeColor = Color.Black;
                    break;
                            
                case ENUM_TELEPHONE_STATE.NORMAL:
                    picTelStatus.Image = Properties.Resources.telephone_black;
                    lblTelStatus.ForeColor = Color.Black;
                    break;
                    
                case ENUM_TELEPHONE_STATE.INCOMING:
                    picTelStatus.Image = Properties.Resources.telephone_red;
                    lblTelStatus.ForeColor = Color.Red;
                    break;

                case ENUM_TELEPHONE_STATE.ACK:
                    picTelStatus.Image =  Properties.Resources.telephone_green;
                    lblTelStatus.ForeColor = Color.Green;
                    break;

                case ENUM_TELEPHONE_STATE.DIALING:
                    picTelStatus.Image = Properties.Resources.telephone_blue;
                    lblTelStatus.ForeColor = Color.Blue;
                    break;
            }
        }


        private void linkCall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_intDefaltHeight == this.Height)
            {
                this.Height = this.Height - 80;
            }
            else
            {
                this.Height = _intDefaltHeight;
            }
        }

        private void btnCall_Click(object sender, EventArgs e)
        {

            writeLog("btnCall_Click", string.Empty);

            if (txtCallTel.Text.Equals(string.Empty))
            {
                showMessage("電話番号が指定されていません。");
                return;
            }

            call();

        }

        private void call()
        {
            // TODO数値チェック
            System.Diagnostics.Process.Start("telto:" + this.txtCallTel.Text);
        }

        #region 接続時のメッセージ送信
        private void SendTelConnectedMessage(string callerID)
        {
            SendMessage(string.Concat("TELCONNECT:", callerID));
        }

        /// <summary>==============================================================================
        /// UDPメッセージ送信
        /// </summary>
        /// <param name="sendMsg">メッセージ</param>
        private void SendMessage(string sendMsg)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            try
            {
                byte[] sendBytes = enc.GetBytes(sendMsg);
                udpClient.Send(sendBytes, sendBytes.Length, _iniFile.CTISENDHOST, _iniFile.CTISENDPORT);
            }
            catch (Exception ex)
            {
                writeLog("SendMessage", ex.ToString());
                return;
            }
        }
        #endregion

        #region 発信メッセージを受信
        /// <summary>==============================================================================
        /// UDPメッセージ受信
        /// </summary>
        /// <returns>受信メッセージ</returns>
        private string ReceiveMessage()
        {
            string rcvMsg = "";
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            try
            {
                System.Net.IPEndPoint remoteEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
                byte[] rcvBytes = udpClient.Receive(ref remoteEP);
                rcvMsg = enc.GetString(rcvBytes);
            }
            catch (Exception ex)
            {
                writeLog("RecieveMessage", ex.ToString());
            }
            return rcvMsg;
        }
        #endregion

        #region 発信メッセージを受信
        /// <summary>==============================================================================
        /// UDPメッセージ受信
        /// </summary>
        /// <returns>受信メッセージ</returns>
        private string ReceiveMessageSoftphone()
        {
            string rcvMsg = "";
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            try
            {
                System.Net.IPEndPoint remoteEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
                byte[] rcvBytes = udpClientSoftphone.Receive(ref remoteEP);
                rcvMsg = enc.GetString(rcvBytes);
            }
            catch (Exception ex)
            {
                writeLog("ReceiveMessageSoftphone", ex.ToString());
            }
            return rcvMsg;
        }
        #endregion

        #region 受信メッセージWait
        /// <summary>==============================================================================
        /// UDPメッセージ受信ポンプ
        /// </summary>
        private void ReceiveLoop()
        {
            do
            {
                if (_exitReceiveLoop) return;
                if (udpClient.Available <= 0)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }
                string MSG = this.ReceiveMessage();
                string[] ans = MSG.Split(':');
                if (ans == null || ans.Length <= 0) continue;
                switch (ans[0])
                {
                    case "CALL":
                        if (ans.Length <= 1) continue;

                        //txtCallTel.Text = ans[1];
                        Invoke(new ReciveDelegateFromBackOffice(SetCallId), ans[1]);

                        CtiServiceDataSet.TelListDataTable dt = readTelList(string.Empty, ans[1]);

                        call();

                        if (dt == null || dt.Count <= 0) continue;
                        //txtCallName.Text = dt[0].CUSTOMER_NAME;
                        Invoke(new ReciveDelegateFromBackOffice(SetCallName), dt[0].CUSTOMER_NAME);
                        
                        break;
                }
            } while (true);
        }
        #endregion

        #region 受信メッセージWait（Softphone）
        /// <summary>==============================================================================
        /// UDPメッセージ受信ポンプ
        /// </summary>
        private void ReceiveLoopSoftphone()
        {
            do
            {
                if (_exitReceiveLoop) return;
                if (udpClientSoftphone.Available <= 0)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }
                string MSG = this.ReceiveMessageSoftphone();
                Invoke(new AnalyzeSoftPhoneMessage(getAnalyzeSoftPhoneMessage), MSG);
            } while (true);
        }
        #endregion

        delegate void AnalyzeSoftPhoneMessage(string ans);

        private string _strCallingNo = ""; // 発信中番号

        internal void getAnalyzeSoftPhoneMessage(string strSoftphoneArgs)
        {

            string[] arrSoftphoneArgs = strSoftphoneArgs.Split('＜');
            if (arrSoftphoneArgs == null || arrSoftphoneArgs.Length <= 0) return;

            string strTextLine = "";

            if (_isCtiTrace == true) strTextLine += "-- Start ----------------------------------------------------" + Environment.NewLine;

            Boolean bolCalling = false;

            // 通話中イベント以外は発信中番号を初期化
            if (arrSoftphoneArgs.Contains("/EventId:ACTIVE_CALL") == false)
            {
                _strCallingNo = "";
            }
            else
            {
                // 通話中イベントで発信番号が入っているとき
                if (_strCallingNo != "")
                {
                    bolCalling = true;
                }
            }

            foreach (string strSoftphoneMessage in arrSoftphoneArgs)
            {

                // 着信中、発信中、通話中のND制御
                if (strSoftphoneMessage.IndexOf("/RemoteNo:") >= 0)
                {
                    // 発信中
                    if (0 <= Array.IndexOf(arrSoftphoneArgs, "/EventId:PLACE_CALL") ||
                        0 <= Array.IndexOf(arrSoftphoneArgs, "/EventId:RINGING_CALL"))
                    {
                        _strCallingNo = strSoftphoneMessage.Replace("/RemoteNo:", "");
                    }

                    // 着信中
                    if (0 <= Array.IndexOf(arrSoftphoneArgs, "/EventId:GOAL_CALL"))
                    {
                        setCallerID(strSoftphoneMessage.Replace("/RemoteNo:", ""));
                    }

                    // 通話中
                    if (0 <= Array.IndexOf(arrSoftphoneArgs, "/EventId:ACTIVE_CALL"))
                    {
                        string strNumber = strSoftphoneMessage.Replace("/RemoteNo:", "");
                        if ((strNumber == "****") && (bolCalling == true))
                        {
                            // 発信後の通話確立で、非通知「****」が来た場合は、発信番号を格納
                            strNumber = _strCallingNo;
                        }

                        setCallerID(strNumber);

                        // 着信通知の場合のみBO連携
                        if (bolCalling == false) {
                            if (strNumber == "****") {
                                strNumber = "";
                                SendTelConnectedMessage(strNumber);
                            } else if (strNumber.Length > _iniFile.EXTLENGTH) {
                                // 内線以外をBOに通知
                                SendTelConnectedMessage(strNumber);
                            }
                        }
                    }
                }

                if (strSoftphoneMessage.IndexOf("/EventId:LOGIN_COMPLETE") >= 0)
                {
                    lblTelStatus.Text = "ログインしました。";
                }
                if (strSoftphoneMessage.IndexOf("/EventId:PLACE_CALL") >= 0)
                {
                    lblTelStatus.Text = "発信中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.DIALING);
                }
                if (strSoftphoneMessage.IndexOf("/EventId:RINGING_CALL") >= 0)
                {
                    lblTelStatus.Text = "発信中（相手コール中）";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.DIALING);
                }
                if (strSoftphoneMessage.IndexOf("/EventId:ACTIVE_CALL") >= 0)
                {
                    lblTelStatus.Text = "通話中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                }
                if (strSoftphoneMessage.IndexOf("/EventId:IDLE_CALL") >= 0)
                {
                    lblTelStatus.Text = "待機中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.NONE);
                }
                if (strSoftphoneMessage.IndexOf("/EventId:GOAL_CALL") >= 0)
                {
                    lblTelStatus.Text = "着信中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.INCOMING);
                }

                //if (strSoftphoneMessage.IndexOf("/EventId:HOLD_CALL") >= 0)
                //{
                //    lblTelStatus.Text = "保留";
                //    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                //}
                //if (strSoftphoneMessage.IndexOf("/EventId:UNHOLD_CALL") >= 0)
                //{
                //    lblTelStatus.Text = "保留解除";
                //    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                //}
                //if (strSoftphoneMessage.IndexOf("/EventId:FORWARDING_CALL") >= 0)
                //{
                //    lblTelStatus.Text = "転送";
                //    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                //}
                //if (strSoftphoneMessage.IndexOf("/EventId:FORWARDING_OK") >= 0)
                //{
                //    lblTelStatus.Text = "転送成功";
                //    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                //}
                //if (strSoftphoneMessage.IndexOf("/EventId:FORWARDING_NG") >= 0)
                //{
                //    lblTelStatus.Text = "転送失敗";
                //    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                //}

                if (_isCtiTrace == true) strTextLine += strSoftphoneMessage + Environment.NewLine;
            }

            if (_isCtiTrace == true) strTextLine += "-- End ------------------------------------------------------" + Environment.NewLine;

            if (_isCtiTrace == true)
            {
                txtDebug.Text += strTextLine;
                txtDebug.SelectionStart = txtDebug.Text.Length;
            }

        }

        //Delegate を宣言しておく
        delegate void ReciveDelegateFromBackOffice(string text);

        //リストボックスにテキストを追加していくメソッド
        internal void SetCallId(string text) {
            txtCallTel.Text = text;
        }
        internal void SetCallName(string text)
        {
            txtCallName.Text = text;
        }
  
        #region メッセージWait
        private void bgwUdpRecieve_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveLoop();
        }
        private void bgwUdpRecieveSoftphone_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveLoopSoftphone();
        }
        #endregion

        #region WriteLog
        private void writeLog(string tag, string message)
        {
            if (!_iniFile.OUTPUTLOG) return;
            Utility.writeLog(tag, message);
        }
        #endregion

        #region 電話番号リスト読み込み
        private CtiServiceDataSet.TelListDataTable readTelList(string customerName, string telNo)
        {

            if (_isCtiDbConnect == false) return null;

            try
            {
                _dbAccessor.open();
            }
            catch (Exception ex)
            {
                writeLog("readTelList", ex.ToString());
                return null;
            }
            finally
            {
                try { _dbAccessor.close(); } catch (Exception ex) { } 
            }
           
            string sql = string.Empty;
            try
            {
                sql = @"
                    WITH BASEDATA AS (
                      {0}
                    )
                    SELECT
                      BASE.CUSTOMER_NO
                      ,BASE.CUSTOMER_NAME
                      ,BASE.TELNO
                    FROM
                      BASEDATA BASE
                    {1}
                    ORDER BY
                      BASE.CUSTOMER_NO
                      ,BASE.CUSTOMER_NAME
                      ,BASE.TELNO
                    ";

                List<string> lstWhere = new List<string>();
                if (!customerName.Equals(string.Empty))
                {
                    lstWhere.Add(string.Concat(" AND BASE.CUSTOMER_NAME LIKE '%", customerName, "%'"));
                }
                if (!telNo.Equals(string.Empty))
                {
                    lstWhere.Add(string.Concat(" AND BASE.TELNO LIKE '", telNo, "'"));
                }
                string whereStatement = string.Empty;
                foreach (string where in lstWhere)
                {
                    whereStatement = string.Concat(whereStatement, where);
                }
                if (!whereStatement.Equals(string.Empty))
                {
                    whereStatement = string.Concat("WHERE ", whereStatement.Substring(4));
                }

                sql = string.Format(sql, _iniFile.SQL_GETTELLIST, whereStatement);
                CtiServiceDataSet.TelListDataTable dt = new CtiServiceDataSet.TelListDataTable();
                dt.Merge(_dbAccessor.fill(sql));

                return dt;
            }
            catch (Exception ex)
            {
                writeLog("readTelList", ex.ToString());
                writeLog("readTelList", sql);
                return null;
            }
            finally
            {
                _dbAccessor.close();
            }
        }
        #endregion


    }
}

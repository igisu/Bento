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

namespace com.emukikaku.CtiServiceAlphaCTI
{

    public partial class frmCtiMain : Form
    {

        #region Member

        private int _intDefaltHeight = 0;
        private int _intDefulatWidth = 0;

        public IniFile _iniFile;
        public DBAccessor _dbAccessor;
        public bool _isCtiTrace = false;       // デバッグモード：ソフトフォンからのイベントを画面に出力する
        public bool _isCtiDbConnect = true;    // デバッグモード：DB接続しない（NDなどのやりとりだけ）

        private bool _runningEvent;

        public System.Net.Sockets.UdpClient udpClient;
        public System.Net.Sockets.UdpClient udpClientSoftphone;

        private bool _exitReceiveLoop;

        // WCF接続オブジェクト
        private alpha2GW.WCFConnection _wcfConnection = null;

        private bool _dialing = false;          // CtiService または BO から発信した場合はtrue

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

        #region Const
        private const string WCF_RECV_DELIVERED = "DELIVERED";          // 着信時識別子
        private const string WCF_RECV_CONNECTED = "CONNECTED";          // 通話時識別子
        private const string WCF_RECV_MADECALL  = "MADECALL";           // 発信時識別子
        private const string WCF_RECV_MENU      = "MENU";               // メニュー選択時識別子
        private const string WCF_RECV_QUIT      = "FINISH";             // 終了時識別子
        private const string WCF_RECV_DLOGINFO  = "DLOGINFO";           // 着信ログダブルクリック時識別子
        private const string WCF_RECV_MLOGINFO  = "MLOGINFO";           // 発信ログダブルクリック時識別子
        private const string WCF_RECV_TALKEND   = "TALKEND";            // 通話終了時識別子
        private const string WCF_RECV_CLOSE     = "CONNECTION_CLOSE";   // コネクション終了

        private const string WCF_SEND_MAKECALL  = "MAKECALL";           // 発信識別子

        private const char   WCF_DELIMITER      = ',';                  // 区切り文字
        #endregion

        #region 内部クラス
        /// <summary>
        /// アプリケーション接続ツールから飛んでくるWCFメッセージ
        /// </summary>
        private class WcfMessge
        {
            /// <summary>
            /// メッセージ識別子
            /// </summary>
            public enum WcfIdentifier
            {
                /// <summary>
                /// 通話
                /// </summary>
                DELIVERED,

                /// <summary>
                /// 通話
                /// </summary>
                CONNECTED,

                /// <summary>
                /// 発信
                /// </summary>
                MADECALL,

                /// <summary>
                /// メニュー選択
                /// </summary>
                MENU,

                /// <summary>
                /// 終了
                /// </summary>
                FINISH,

                /// <summary>
                /// 着信ログダブルクリック
                /// </summary>
                DLOGINFO,

                /// <summary>
                /// 発信ログダブルクリック
                /// </summary>
                MLOGINFO,

                /// <summary>
                /// 通話終了
                /// </summary>
                TALKEND,

                /// <summary>
                /// コネクション終了
                /// </summary>
                CONNECTION_CLOSE,

                /// <summary>
                /// 不明
                /// </summary>
                UNKNOWN,
            }
            /// <summary>
            /// 識別子
            /// </summary>
            public WcfIdentifier Identifier { get; set; }

            /// <summary>
            /// 発信者番号判明フラグ
            /// </summary>
            public bool telNoFlg { get; set; }

            /// <summary>
            /// 内線フラグ
            /// </summary>
            public bool IsExtension { get; set; }

            /// <summary>
            /// 発信者電話番号
            /// </summary>
            public string TelNo { get; set; }

            /// <summary>
            /// ダイヤルイン番号
            /// </summary>
            public string DialInNo { get; set; }

            public WcfMessge(string msg)
            {
                // ┌──────┬────────┬─────────────┬───────┬───────┬────────┐
                // │イベント    │メッセージ識別子│発信者番号判明フラグ(0or1)│"内線"or"外線"│発信者電話番号│ダイヤルイン番号│
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │着信        │DELIVERED       │            □            │      ○      │      ○      │       △       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │通話        │CONNECTED       │            □            │      ○      │      ○      │       △       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │発信        │MADECALL        │1固定                     │      ○      │      ○      │       ×       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │通話終了    │TALKEND         │1固定                     │      ○      │      ○      │       ○       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │発信ログ    │MLOGINFO        │1固定                     │      ○      │      ○      │       ○       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │着信ログ    │DLOGINFO        │1固定                     │      ○      │      ○      │       ○       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │メニュー選択│MENU            │            ×            │      ×      │      ×      │       ×       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │終了        │FINISH          │            ×            │      ×      │      ×      │       ×       │
                // ├──────┼────────┼─────────────┼───────┼───────┼────────┤
                // │切断        │CONNECTION_CLOSE│            ×            │      ×      │      ×      │       ×       │
                // └──────┴────────┴─────────────┴───────┴───────┴────────┘
                // ○:必ずある
                // △:設定値次第
                // □:WCFの場合は設定されているが、exe起動の場合は設定されない。
                // ×:無い

                Identifier = WcfIdentifier.UNKNOWN;

                string[] msgary = msg.Split(WCF_DELIMITER);
                if (msgary.Length > 0)  // メッセージ識別子がある
                {
                    switch (msgary[0].ToUpper())
                    {
                        case WCF_RECV_DELIVERED:
                            Identifier = WcfIdentifier.DELIVERED;
                            break;

                        case WCF_RECV_CONNECTED:
                            Identifier = WcfIdentifier.CONNECTED;
                            break;

                        case WCF_RECV_MADECALL:
                            Identifier = WcfIdentifier.MADECALL;
                            break;

                        case WCF_RECV_MENU:
                            Identifier = WcfIdentifier.MENU;
                            break;

                        case WCF_RECV_QUIT:
                            Identifier = WcfIdentifier.FINISH;
                            break;

                        case WCF_RECV_DLOGINFO:
                            Identifier = WcfIdentifier.DLOGINFO;
                            break;

                        case WCF_RECV_MLOGINFO:
                            Identifier = WcfIdentifier.MLOGINFO;
                            break;

                        case WCF_RECV_TALKEND:
                            Identifier = WcfIdentifier.TALKEND;
                            break;

                        case WCF_RECV_CLOSE:
                            Identifier = WcfIdentifier.CONNECTION_CLOSE;
                            break;

                        default:
                            Identifier = WcfIdentifier.UNKNOWN;
                            break;
                    }
                }

                if (msgary.Length > 1)  // 発信者番号判明フラグがある
                {
                    if (msgary[1] == "1")
                    {
                        telNoFlg = true;
                    }
                }

                if (msgary.Length > 2)  // "内線"or"外線"がある
                {
                    if (msgary[2] == "0" || msgary[2] == "内線")
                    {
                        IsExtension = true;
                    }
                }

                if (msgary.Length > 3)  // 発信者電話番号がある
                {
                    TelNo = msgary[3];
                    telNoFlg = true;    // 発信者電話番号があるなら発信者番号判明フラグもON
                }

                if (msgary.Length > 4)  // ダイヤルイン番号がある
                {
                    DialInNo = msgary[4];
                }
            }

            /// <summary>
            /// 発信者電話番号が無効かどうか
            /// </summary>
            /// <returns>無効な発信者電話番号の場合はtrue</returns>
            public bool IsInvalidTelNo()
            {
                if (string.IsNullOrEmpty(TelNo))
                {
                    return true;
                }

                foreach (char c in TelNo)
                {
                    // 数字以外が含まれているか？
                    if (c < '0' || '9' < c)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        public frmCtiMain()
        {
            InitializeComponent();
            _intDefaltHeight = this.Height;
            _intDefulatWidth = this.Width;
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

            // WCF通信開始
            WcfConnect();

            // SoftPhone（アプリケーション接続ツール）からいきなり、起動された場合
            if (_iniFile.USEWCFRECEIVE == false)
            {
                string[] cmds = System.Environment.GetCommandLineArgs();
                if (cmds.Length > 1)
                {
                    getAnalyzeSoftPhoneMessage(cmds[1]);
                }
            }

            // ----- 画面回りの設定 -----
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
                this.MaximumSize = new Size(_intDefulatWidth, _intDefaltHeight);
                this.MinimumSize = new Size(_intDefulatWidth - 150, _intDefaltHeight - 80);
            } else {
                this.MaximumSize = new Size(0, 0);
                this.MinimumSize = new Size(0, 0);
            }

            if (this.WindowState == FormWindowState.Normal)
            {
                // this.Size = new Size(this.Width - 150, this.Height);
                this.Size = new Size(_intDefulatWidth - 150, _intDefaltHeight);
                linkCall_LinkClicked(null, null);
            }
            else
            {
                // アプリケーション接続ツールでWCFでの通知を有効にしている場合、アプリケーション接続ツールからは最小化で起動される。
                this.Size = new Size(_intDefulatWidth - 150, _intDefaltHeight - 80);  // 発信エリアを畳んだ状態にする
            }

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

            try
            {
                WcfDisConnect();
            }
            catch (Exception ex)
            {
                showMessage(ex.ToString());
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
            if (_iniFile.USEWCFDIALING)
            {
                if (_wcfConnection != null)
                {
                    _wcfConnection.SendMessage(string.Format("{0}{1}{2}{1}{3}", WCF_SEND_MAKECALL, WCF_DELIMITER, this.txtCallTel.Text, "1"));
                }
            }
            else
            {
                System.Diagnostics.Process.Start(string.Format("alpha2to:{0}{1}{2}{1}{3}", WCF_SEND_MAKECALL, WCF_DELIMITER, this.txtCallTel.Text, "1"));
            }
            _dialing = true;
        }

        #region アプリケーション接続ツールからのWCF受信
        private void WCFConnection_OnReceive(string msg)
        {
            if (_iniFile.USEWCFRECEIVE)
            {
                Invoke(new AnalyzeSoftPhoneMessage(getAnalyzeSoftPhoneMessage), msg);
            }
        }
        #endregion


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

        /// <summary>
        /// 受信メッセージ解析
        /// </summary>
        /// <param name="msg"></param>
        internal void getAnalyzeSoftPhoneMessage(string msg)
        {
            string strTextLine = "";
            if (_isCtiTrace == true)
            {
                strTextLine += "-- Start ----------------------------------------------------" + Environment.NewLine;
                strTextLine += msg + Environment.NewLine;
            }

            WcfMessge wcfMessge = new WcfMessge(msg);
            switch (wcfMessge.Identifier)
            {
                case WcfMessge.WcfIdentifier.MENU:      // メニュー選択時
                case WcfMessge.WcfIdentifier.FINISH:    // 終了
                case WcfMessge.WcfIdentifier.DLOGINFO:  // 着信ログダブルクリック
                case WcfMessge.WcfIdentifier.MLOGINFO:  // 発信ログダブルクリック
                case WcfMessge.WcfIdentifier.UNKNOWN:   // 不明
                    return;

                case WcfMessge.WcfIdentifier.DELIVERED: // 着信中
                    // 発信者番号判明フラグがOFF かつ 発信者電話番号も空っぽの場合は無視する
                    if (wcfMessge.telNoFlg == false && string.IsNullOrEmpty(wcfMessge.TelNo))
                    {
                        return;
                    }
                    break;

                case WcfMessge.WcfIdentifier.CONNECTION_CLOSE: // コネクション終了
                    // 切断して再接続
                    // try
                    // {
                    //     WcfDisConnect();
                    // }
                    // catch (Exception)
                    // {
                    // }
                    // WcfConnect();
                    // break;
                    this.Close();     // CtiServiceを終了して、アプリケーション接続ツールによる次回の通知で起動されるのを期待する
                    return;
            }

            Boolean bolCalling = false;

            // 通話中イベント以外は発信中番号を初期化
            if (wcfMessge.Identifier != WcfMessge.WcfIdentifier.CONNECTED)
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

            // 着信中、発信中、通話中のND制御
            switch (wcfMessge.Identifier)
            {
                case WcfMessge.WcfIdentifier.MADECALL:  // 発信中
                    _strCallingNo = wcfMessge.TelNo;
                    _dialing = true;
                    break;

                case WcfMessge.WcfIdentifier.DELIVERED: // 着信中
                    setCallerID(wcfMessge.TelNo);
                    break;

                case WcfMessge.WcfIdentifier.CONNECTED: // 通話中
                    string strNumber = wcfMessge.TelNo;
                    if (bolCalling)
                    {
                        if (wcfMessge.telNoFlg == false || wcfMessge.IsInvalidTelNo())
                        {
                            // 発信後の通話確立で、発信者電話番号が場合は、発信番号を格納
                            strNumber = _strCallingNo;
                        }
                    }

                    // 着信通知の場合のみBO連携
                    if (_dialing == false)      // 発信通話は無視する
                    {
                        setCallerID(strNumber);

                        if (bolCalling == false)
                        {
                            if (wcfMessge.IsInvalidTelNo())
                            {
                                strNumber = "";
                                SendTelConnectedMessage(strNumber);
                            }
                            else
                            {
                                // 内線以外をBOに通知
                                if (wcfMessge.IsExtension == false && strNumber.Length > _iniFile.EXTLENGTH)
                                {
                                    SendTelConnectedMessage(strNumber);
                                }
                            }
                        }
                    }
                    _dialing = false;
                    break;
            }

            switch (wcfMessge.Identifier)
            {
                case WcfMessge.WcfIdentifier.DELIVERED: // 着信中
                    lblTelStatus.Text = "着信中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.INCOMING);
                    break;

                case WcfMessge.WcfIdentifier.CONNECTED: // 通話中
                    lblTelStatus.Text = "通話中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.ACK);
                    break;

                case WcfMessge.WcfIdentifier.MADECALL:  // 発信中
                    lblTelStatus.Text = "発信中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.DIALING);
                    break;

                case WcfMessge.WcfIdentifier.TALKEND:  // 通話終了
                    lblTelStatus.Text = "待機中";
                    setPicTelStatus(ENUM_TELEPHONE_STATE.NONE);
                    break;
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

        /// <summary>
        /// WCF通信開始
        /// </summary>
        private void WcfConnect()
        {
            // 未接続なら接続
            if (_wcfConnection == null)
            {
                bgwWcfConnect.RunWorkerAsync();
            }
        }

        private void bgwWcfConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_wcfConnection == null)
            {
                try
                {
                    _wcfConnection = new alpha2GW.WCFConnection();
                    _wcfConnection.Open("net.pipe://localhost/CtPB/");
                    _wcfConnection.BeginReceive();
                    _wcfConnection.OnReceive += WCFConnection_OnReceive;    // 受信イベントを登録
                    break;
                }
                catch (Exception)
                {
                    // 接続に失敗した場合はリトライ
                    _wcfConnection = null;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// WCF切断
        /// </summary>
        private void WcfDisConnect()
        {
            if (_wcfConnection != null)
            {
                _wcfConnection.OnReceive -= WCFConnection_OnReceive;    // 受信イベントを破棄
                _wcfConnection.Close();
                _wcfConnection = null;
            }
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
                try { _dbAccessor.close(); } catch (Exception) { }
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

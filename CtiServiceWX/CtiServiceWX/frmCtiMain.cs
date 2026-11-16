using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using log4net;

namespace com.emukikaku.CtiService
{
    public partial class frmCtiMain : Form
    {
        #region Member
        private IniFile _iniFile;
        private DBAccessor _dbAccessor;
        private bool _runningEvent;
        private Image[] _imageLIstTelephoneState;
        private Button[] _btnPhone;
        private int[] _incomingCallIndex;
        private int _incomingConnectedTrunk;
        private bool _internalConnected;
        private int _incomingInternalCallIndex;
        private bool _hold;
        private int _callToCallIndex;

        private System.Net.Sockets.UdpClient udpClient;
        private bool _exitReceiveLoop;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);   // 2020/09/30 Add
        #endregion

        #region Enum
        private enum ENUM_TELEPHONE_STATE
        {
             NONE
            ,NORMAL
            ,INCOMING
            ,ACK
            ,DIALING
        }

        private enum ENUM_KEYEVENT_TYPE
        {
           INTERNAL
           ,EXTENAL
           ,CALL
           ,ANSWER
           , UNKNOWN
        }
        #endregion

        #region Constructor
        public frmCtiMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Load
        private void frmCtiMain_Load(object sender, EventArgs e)
        {
            if (isRunningEvent()) return;
            try
            {
                _btnPhone = new Button[] {
                    btnAck01
                    ,btnAck02
                    ,btnAck03
                    ,btnAck04
                    ,btnAck05
                };
                _incomingCallIndex = new int[_btnPhone.Length];
                _hold = false;
                createTelephoneImageList();
                clear();

                //全機能停止
                setDownSystemState();

                _iniFile = new IniFile();
                _dbAccessor = new DBAccessor(_iniFile.DBSERVER, _iniFile.DBUSERID, _iniFile.DBPASSWORD, _iniFile.DBDATABASENAME);

                axAspL1.MtOpen();
                //showMessage(axAspL1.PpErrCode.ToString());
                if (axAspL1.PpErrCode != 0)
                {
                    showMessage("この端末ではCTI機能が利用できません。");
                    return;
                }

                //全機能開放
                setIncomingNo();
                setUpSystemState();

                //ポート指定してWait
                udpClient = new System.Net.Sockets.UdpClient(_iniFile.CTIRECEIVEPORT);
                udpClient.DontFragment = true;
                udpClient.EnableBroadcast = true;

                this.bgwUdpRecieve.RunWorkerAsync();
                _exitReceiveLoop = false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                showMessage(ex.ToString());
            }
            finally
            {
                quitEvent();
            }

        }
        #endregion

        #region メッセージ表示
        private DialogResult showMessage(string message)
        {
            return showMessage(message, MessageBoxButtons.OK);
        }

        private DialogResult showMessage(string message,MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, "CTI Service", buttons);
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
                axAspL1.MtClose();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                showMessage(ex.ToString());
            }
            finally
            {
                quitEvent();
            }
        }
        #endregion

        #region Create Telephone ImageList
        private void createTelephoneImageList()
        {
            _imageLIstTelephoneState = new Image[5];
            _imageLIstTelephoneState[0] = Properties.Resources.telephone_gray;
            _imageLIstTelephoneState[1] = Properties.Resources.telephone_black;
            _imageLIstTelephoneState[2] = Properties.Resources.telephone_red;
            _imageLIstTelephoneState[3] = Properties.Resources.telephone_green;
            _imageLIstTelephoneState[4] = Properties.Resources.telephone_blue;
        }
        #endregion

        #region クリア
        private void clear()
        {
            lblNaisenBangou.Text = string.Empty;
            lblCallerID.Text = string.Empty;
            lblCallerName.Text = string.Empty;
            btnAck01.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
            btnAck02.Image  = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
            btnAck03.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
            btnAck04.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
            btnAck05.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
            btnAckInnerLine.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
            lblCallToName.Text = string.Empty;
            txtCallToNumber.Text = string.Empty;
            btnCall.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NONE];
        }
        #endregion

        #region 全機能停止
        private void setDownSystemState()
        {
            btnAck01.Enabled = false;
            btnAck02.Enabled = false;
            btnAck03.Enabled = false;
            btnAck04.Enabled = false;
            btnAck05.Enabled = false;
            btnAckInnerLine.Enabled = false;
            btnHoldAck.Enabled = false;
            btnDisconnection.Enabled = false;
            txtCallToNumber.Enabled = false;
            btnCall.Enabled = false;
        }
        #endregion

        #region 接続
        private void axAspLine_EvLcsConnected(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsConnectedEvent e)
        {
            try
            {
                writeLog("EvLcsConnected", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));
                if (keyInfo2EventType(e.keyInfo).Equals(ENUM_KEYEVENT_TYPE.EXTENAL))
                {
                    setIncomingTelephoneState(axAspL1.get_PpTrunk(e.callIndex), ENUM_TELEPHONE_STATE.ACK);
                    _incomingConnectedTrunk = axAspL1.get_PpTrunk(e.callIndex);
                    _internalConnected = false;

                    if (_callToCallIndex == e.callIndex)
                    {
                        _btnPhone[axAspL1.get_PpTrunk(e.callIndex) - 1].Enabled = true;
                        setIncomingTelephoneState(axAspL1.get_PpTrunk(e.callIndex), ENUM_TELEPHONE_STATE.ACK);
                        btnCall.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.ACK];
                    }
                    else
                    {
                        setCallerID(axAspL1.get_PpCallerID(e.callIndex));
                        SendTelConnectedMessage(lblCallerID.Text);
                    }
                }
                else if (keyInfo2EventType(e.keyInfo).Equals(ENUM_KEYEVENT_TYPE.INTERNAL))
                {
                    btnAckInnerLine.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.ACK];
                    _internalConnected = false;
                }

                btnHoldAck.Enabled = true;
                btnHoldAck.Text = "保　留";
                _hold = false;
                btnDisconnection.Enabled = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 受信番号設定
        private void setCallerID(string callerID)
        {
            lblCallerID.Text = callerID;
            lblCallerName.Text = string.Empty;
            if( !callerID.Equals(string.Empty)){
                CtiServiceDataSet.TelListDataTable dt = readTelList(string.Empty, callerID);
                //20160125高崎 1件の時しか表示しないのを修正
                //if (dt != null && dt.Count.Equals(1))
                if (dt != null && dt.Count >= 1)
                {
                    lblCallerName.Text = dt[0].CUSTOMER_NAME;
                }
            }
        }
        #endregion

        #region 内線番号設定
        private void setIncomingNo()
        {
            string lineName = axAspL1.MtLineName(0);
            if( lineName.StartsWith("EXTENSION")){
                lineName = lineName.Replace("EXTENSION ",string.Empty);
                lineName = lineName.Replace(" Keyset", string.Empty);
            }
            lblNaisenBangou.Text = lineName;
        }
        #endregion

        #region 機能開放
        private void setUpSystemState()
        {
            btnAck01.Enabled = false;
            btnAck02.Enabled = false;
            btnAck03.Enabled = false;
            btnAck04.Enabled = false;
            btnAck05.Enabled = false;
            btnAckInnerLine.Enabled = false;
            btnHoldAck.Enabled = false;
            btnDisconnection.Enabled = false;
            txtCallToNumber.Enabled = true;
            btnCall.Enabled = true;


            btnAck01.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
            btnAck02.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
            btnAck03.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
            btnAck04.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
            btnAck05.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
            btnAckInnerLine.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
            btnCall.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
        }
        #endregion

        #region 着信呼発生
        private void axAspLine_EvLcsOffering(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsOfferingEvent e)
        {
            try
            {
                writeLog("EvLcsOffering", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));
                setCallerID(axAspL1.get_PpCallerID(e.callIndex));

                int trunk = axAspL1.get_PpTrunk(e.callIndex);
                if (trunk > 0 && trunk <= _btnPhone.Length && keyInfo2EventType(e.keyInfo).Equals(ENUM_KEYEVENT_TYPE.EXTENAL))
                {
                    setIncomingTelephoneState(trunk, ENUM_TELEPHONE_STATE.INCOMING);
                    _btnPhone[trunk - 1].Enabled = true;
                    _incomingCallIndex[trunk - 1] = e.callIndex;
                }
                if (keyInfo2EventType(e.keyInfo).Equals(ENUM_KEYEVENT_TYPE.INTERNAL))
                {
                    btnAckInnerLine.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.INCOMING];
                    btnAckInnerLine.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 切断検出
        private void axAspLine_EvLcsDisconnected(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsDisconnectedEvent e)
        {
            try
            {
                writeLog("EvLcsDisconnected", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));

                int trunk = axAspL1.get_PpTrunk(e.callIndex);
                if (trunk > 0 && trunk <= _btnPhone.Length && _incomingCallIndex[trunk - 1] == e.callIndex)
                {
                    setIncomingTelephoneState(trunk, ENUM_TELEPHONE_STATE.NORMAL);
                    _btnPhone[trunk - 1].Enabled = false;
                    if (_incomingConnectedTrunk == trunk)
                    {
                        setControlLineDisconnected();
                    }
                }
                if (_callToCallIndex == e.callIndex)
                {
                    setIncomingTelephoneState(trunk, ENUM_TELEPHONE_STATE.NORMAL);
                    btnCall.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
                    _btnPhone[trunk - 1].Enabled = false;
                    _callToCallIndex = -1;
                    setControlLineDisconnected();
                }
                if (keyInfo2EventType(e.keyInfo).Equals(ENUM_KEYEVENT_TYPE.INTERNAL))
                {
                    btnAckInnerLine.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.NORMAL];
                    btnAckInnerLine.Enabled = false;
                    setControlLineDisconnected();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region ダイアル検出
        private void axAspLine_EvLcsDialing(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsDialingEvent e)
        {
            try
            {
                writeLog("EvLcsDialing", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));
                setIncomingTelephoneState(axAspL1.get_PpTrunk(e.callIndex), ENUM_TELEPHONE_STATE.DIALING);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 受信電話状態設定
        private void setIncomingTelephoneState(int trunk , ENUM_TELEPHONE_STATE state)
        {
            if (trunk > 0 && trunk <= _btnPhone.Length)
            {
                _btnPhone[trunk - 1].Image = _imageLIstTelephoneState[(int)state];
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

        #region Unknown(他者応答など)
        private void axAspLine_EvLcsUnknown(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsUnknownEvent e)
        {
            try
            {
                writeLog("EvLcsUnknown", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));

                int trunk = axAspL1.get_PpTrunk(e.callIndex);
                if (trunk > 0 && trunk <= _btnPhone.Length && _incomingCallIndex[trunk - 1] == e.callIndex)
                {
                    setIncomingTelephoneState(trunk, ENUM_TELEPHONE_STATE.DIALING);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region KyeInfoからイベントタイプ取得
        private ENUM_KEYEVENT_TYPE keyInfo2EventType(int keyInfo)
        {
            if (keyInfo == 0) return ENUM_KEYEVENT_TYPE.INTERNAL;
            if (keyInfo > 0 && keyInfo <= 32) return ENUM_KEYEVENT_TYPE.EXTENAL;
            if (keyInfo == 33) return ENUM_KEYEVENT_TYPE.CALL;
            if (keyInfo == 34) return ENUM_KEYEVENT_TYPE.ANSWER;
            return ENUM_KEYEVENT_TYPE.UNKNOWN;
        }
        #endregion

        #region 保留
        private void axAspLine_EvLcsOnHold(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsOnHoldEvent e)
        {
            try
            {
                writeLog("EvLcsOnHold", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));
                btnHoldAck.Text = "保留解除";
                _hold = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 保留/保留解除
        private void btnHoldAck_Click(object sender, EventArgs e)
        {
            try
            {
                writeLog("btnHoldAck_Click", string.Empty);

                if (_hold)
                {
                    axAspL1.MtUnhold();
                }
                else
                {
                    axAspL1.MtHold();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 特殊状態
        private void axAspLine_EvLcsSpecialInfo(object sender, AxIP3DMCTLLib._DAspLEvents_EvLcsSpecialInfoEvent e)
        {
            writeLog("EvLcsSpecialInfo", string.Format("EvLcsConnectedEvent(CallIndex : {0} , CallNo : {1}  , KeyInfo : {2}) , Other(CallerID : {3})", e.callIndex, e.callNo, e.keyInfo, axAspL1.get_PpCallerID(e.callIndex)));
        }
        #endregion

        #region 切断
        private void btnDisconnection_Click(object sender, EventArgs e)
        {
            try
            {
                writeLog("btnDisconnection_Click", string.Empty);

                axAspL1.MtDrop();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 架電ボタン
        private void btnCall_Click(object sender, EventArgs e)
        {
            writeLog("btnCall_Click", string.Empty);

            if (txtCallToNumber.Text.Equals(string.Empty))
            {
                showMessage("電話番号が指定されていません。");
                return;
            }
            call();
        }
        #endregion

        #region 架電
        private void call()
        {
            try
            {
                axAspL1.MtDial(string.Concat("0", txtCallToNumber.Text), ref _callToCallIndex);
                btnCall.Image = _imageLIstTelephoneState[(int)ENUM_TELEPHONE_STATE.DIALING];
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 内線応答
        private void btnAckInnerLine_Click(object sender, EventArgs e)
        {
            try
            {
                writeLog("btnAckInnerLine_Click:lohpohp", string.Empty);

                axAspL1.MtAnswer(_incomingInternalCallIndex, null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 内外線接続時制御
        private void setControlLineDisconnected()
        {
            btnHoldAck.Enabled = false;
            btnHoldAck.Text = "保　留";
            btnDisconnection.Enabled = false;
        }
        #endregion

        #region 外線応答
        private void btnAck_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int idx = -1;
                for(int i = 0 ; i < _btnPhone.Length ; i++)
                {
                    if (_btnPhone[i].Name.Equals(btn.Name))
                    {
                        idx = i;
                        break;
                    }
                }
                if (idx == -1) return;
                axAspL1.MtAnswer(_incomingCallIndex[idx], null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        #endregion

        #region 電話番号リスト読み込み
        private CtiServiceDataSet.TelListDataTable readTelList(string customerName , string telNo)
        {
            try
            {
                _dbAccessor.open();
            }
            catch (Exception ex)
            {
                writeLog("readTelList", ex.ToString());
                return null;
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

        #region KeyDown
        private void frmCtiMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F8:
                    showSearchCustomer();
                    break;
            }

        }
        #endregion

        #region 顧客検索
        private void showSearchCustomer()
        {
            frmTelList frm = new frmTelList(_dbAccessor);
            frm.setDBAccessor(_dbAccessor);
            frm.setIniFile(_iniFile);
            frm.ShowDialog();
            if (!frm.ChoiceResult.Equals(DialogResult.OK))
            {
                return;
            }

            lblCallToName.Text = frm.CustomerName;
            txtCallToNumber.Text = frm.TelNo;
        }
        #endregion

        #region 接続時のメッセージ送信
        private void SendTelConnectedMessage(string callerID)
        {
            SendMessage(string.Concat("TELCONNECT:",callerID));
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
                log.Error(ex);
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
                System.Net.IPEndPoint remoteEP = new System.Net.IPEndPoint(System.Net.IPAddress.Any,0);
                byte[] rcvBytes = udpClient.Receive(ref remoteEP);
                rcvMsg = enc.GetString(rcvBytes);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                writeLog("RecieveMessage", ex.ToString());
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
                try
                {
                    if (_exitReceiveLoop) return;
                    if (udpClient.Available <= 0)
                    {
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    string MSG = this.ReceiveMessage();
                    string[] ans = MSG.Split(':');
                    if( ans == null || ans.Length <= 0 ) continue;
                    switch(ans[0])
                    {
                        case "CALL":
                            if (ans.Length <= 1) continue;
                            txtCallToNumber.Text = ans[1];
                            CtiServiceDataSet.TelListDataTable dt = readTelList(string.Empty, ans[1]);
                            if (dt == null || dt.Count <= 0) continue;
                            lblCallToName.Text = dt[0].CUSTOMER_NAME;
                            call();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            } while (true);
        }
        #endregion

        #region メッセージWait
        private void bgwUdpRecieve_DoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveLoop();
        }
        #endregion
    }
}
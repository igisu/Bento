using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento
{
    public partial class MDIParent1 : Form
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private Form100 _formHaisoYotei = null;                 // 配送予定入力
        private Form101 _formNippaiMochidashi = null;           // 日配持出入力
        private Form102 _formSeisanYotei = null;                // 生産予定数入力
        private Form103 _formSeisanYoteiTotal = null;           // 生産予定総数一覧
        private Form102A _formSeisanYoteiFromOrder = null;      // 実注文から生産予定数出力
        //private Form200 _formChumon = null;                   // 注文入力
        //private Form201 _formCourseChumon = null;             // コース注文入力
        private Form202 _formIkkatsuChumon = null;              // 一括注文入力
        private Form203 _formCourseChumonlist = null;           // コース注文一覧
        private Form204 _formJuchuHassin = null;                // 受注発信一覧
        private Form205 _formKoteiChumon = null;                // 固定注文反映
        private Form206 _formNohinPublish = null;               // 納品書発行
        private Form207 _formChumonlist = null;                 // 注文一覧
        //private Form208 _formIkkatsuCourseChumon = null;      // 一括コース注文入力
        private Form209 _formYoubiChumon = null;                // 曜日別固定数注文設定
        private Form300 _formKafuzoku = null;                   // 過不足一覧
        private Form301 _formIdoNyuryoku = null;                // 移動入力
        private Form302 _formRinjiCourse = null;                // コース臨時組み替え
        private Form303 _formYokiKaisyuHyo = null;              // 容器回収表印刷
        private Form304 _formTanzakuPrint = null;               // 短冊印刷
        private Form305 _formShippingStatus = null;             // コース出荷状態
        private Form307 _formBigBagLabelPrint = null;           // 大袋ラベル印刷
        private Form400 _formUriage = null;                     // 売上入力
        private Form401 _formUriageMeisai = null;               // 売上明細入力
        private Form402 _formUriageList = null;                 // 売上一覧
        //private Form403 _formUriageSeisan = null;               // 売上精算入力
        private Form404 _formUriageSeisanList = null;           // 売上精算一覧
        private Form405 _formUriageNikkeiHyo = null;            // 売上集計表印刷
        private Form407 _formMiBunpaiList = null;               // 未分配一覧
        private Form408 _formHanbaiJissekiList = null;          // 販売実績一覧表印刷
        private Form500 _formSeikyuShime = null;                // 請求締め処理
        private Form501 _formSeikyuList = null;                 // 請求一覧
        private Form503 _formSeikyuPublish = null;              // 請求書発行
        private Form504 _formSeikyuDataEB = null;               // 請求EBデータ作成
        private Form505 _formUrikakeZanList = null;             // 売掛残高一覧
        private Form507 _formSeikyuDataEBReCreate = null;       // 請求EBデータ再作成
        private Form508 _formSeikyuAdd = null;                  // 請求レコード追加
        private Form509 _formPaySeikyuShime = null;             // 電子決済請求締め処理
        private Form510 _formPaySeikyuList = null;              // 電子決済請求一覧
        private Form512 _formTapionList = null;                 // TP決済一覧
        private Form513 _formFRSettlementList = null;           // FRクレカ決済一覧
        private Form600 _formNyukinList = null;                 // 入金一覧
        private Form601 _formNyukinMeisai = null;               // 入金入力
        private Form602 _formNyukinEBImport = null;             // 入金EBデータ取込
        private Form603 _formMikaisyuList = null;               // 未回収金一覧
        private Form604 _formAnserApiImport = null;             // 銀行APIファイル取込
        private Form700 _formTokuisaki = null;                  // 得意先マスタ一覧
        private Form701 _formTankaTekiyou = null;               // 得意先単価適用マスタメンテ
        private Form702 _formSyohin = null;                     // 商品マスタ一覧
        private Form703 _formSyuyoBuhin = null;                 // 主要部品リストマスタ一覧
        private Form704 _formSyohinGourp = null;                // 商品グループマスタ
        private Form705 _formSyuyoSyohin = null;                // 主要商品リストマスタ一覧
        private Form706 _formTantousya = null;                  // 担当者マスタ一覧
        private Form707 _formBusyo = null;                      // 部署マスタ一覧
        private Form708 _formCource = null;                     // コースマスタ一覧
        private Form709 _formCourceGroup = null;                // コース別グループマスタ一覧
        private Form710 _formKanri = null;                      // 管理マスタ一覧
        private Form711 _formBiko = null;                       // 備考マスタ一覧
        private Form713 _formShukujitsu = null;                 // 祝日マスタメンテ
        private Form714 _formReportPrinter = null;              // 帳票／プリンタ設定
        private Form715 _formShiten = null;                     // 支店マスタ一覧
        private Form716 _formDeleteHiKokyaku = null;            // 非顧客電話番号削除
        private Form717 _formShiwake = null;                    // 仕訳マスタ
        private Form790 _formExport = null;                     // マスタエクスポート
        private Form800 _formVersion = null;                    // バージョン情報
        private Form806 _formSousaHistory = null;               // 操作履歴一覧
        private Form810 _formGetsujiSyukei = null;              // 月次集計
        private Form811 _formGetsujiUrikakeZanList = null;      // 月次売掛残高一覧
        private Form813 _formGetsujiTokuisakiList = null;       // 月次得意先別実績一覧
        private Form814 _formGetsujiSyohinList = null;          // 月次商品別売上一覧
        private Form815 _formGetsujiTokuisakiSuiiList = null;   // 得意先別売上推移
        private Form816 _formZuijiUriNyuList = null;            // 随時売上入金一覧
        private Form817 _formUriageBunsekiHyo = null;           // 集計タイプ別売上分析表印刷
        private Form818 _formFRUseSummaryList = null;           // クレカ利用状況一覧
        private Form819 _formFRUseDetailList = null;            // クレカ利用明細一覧
        private Form820 _formWebHistory = null;                 // Web注文履歴一覧
        private Form821 _formWebTokuisaki = null;               // Web得意先マスタ一覧
        private Form822 _formWebSetting = null;                 // Web用部署設定マスタ
        private Form830 _formWebChumonFuka = null;              // Web用注文不可日設定一覧
        private Form831 _formWebChumonFukaSyohin = null;        // Web注文用商品設定
        private Form832 _formWebChumonTeishi = null;            // Web用注文停止日設定
        private Form840 _formKaikeiRenkei = null;               // 会計連携CSV出力
        private Form841 _formKaikeiRenkeiList = null;           // 会計連携集計一覧

        /// <summary>
        /// メッセージ送信時の自分のポート番号
        /// </summary>
        private int MenuSendPort;

        /// <summary>
        /// UDPメッセージ受信時のポート番号
        /// </summary>
        private int MenuReceivePort;

        /// <summary>
        /// CTIを使用するかどうか
        /// </summary>
        private bool UseCti = false;

        /// <summary>
        /// 受信用UDP
        /// </summary>
        private System.Net.Sockets.UdpClient udpRecv = null;

        private bool _exitReceiveLoop;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public MDIParent1()
        {
            InitializeComponent();
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDIParent1_Load(object sender, EventArgs e)
        {
            // アイコンを設定
            this.Icon = Properties.Resources.icon;

            // バージョン番号を表示
            toolStripStatusLabel.Text = Application.ProductVersion;

            try
            {
                // 権限によりメニューを制御
                SetMenuEnable();

                // 各ポート番号を取得
                MenuSendPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MenuSendPort"]);
                MenuReceivePort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MenuReceivePort"]);
                // CTI使用フラグを取得
                UseCti = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["UseCti"]);

                // CtiServiceとの送受信開始
                udpRecv = new System.Net.Sockets.UdpClient(this.MenuReceivePort);
                backgroundWorker1.RunWorkerAsync();
                _exitReceiveLoop = false;

                // CtiService起動
                StartupCtiService();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // 非活性メニューの制御
                SetMenuDisable();
            }
            catch (Exception)
            {
            }

            try
            {
                // フォームの位置、サイズを復元
                this.Size = new Size(Properties.Settings.Default.Width, Properties.Settings.Default.Height);
                if (Properties.Settings.Default.Left >= 0 && Properties.Settings.Default.Top >= 0)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new Point(Properties.Settings.Default.Left, Properties.Settings.Default.Top);
                }
                this.WindowState = (FormWindowState)Properties.Settings.Default.WindowState;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDIParent1_Shown(object sender, EventArgs e)
        {
            // 受注発信一覧の表示条件（＝担当者架電コースマスタ）のコースで、コース注文入力を開く
            OpenForm201();
        }

        /// <summary>
        /// フォームリサイズ終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDIParent1_ResizeEnd(object sender, EventArgs e)
        {
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Height = this.Height;
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDIParent1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show(this, "終了しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDIParent1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 送受信終了
            _exitReceiveLoop = true;
            if (udpRecv != null) udpRecv.Close();

            // フォームの位置、サイズを保存
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Left = this.Left;
                Properties.Settings.Default.Top = this.Top;
                Properties.Settings.Default.Width = this.Width;
                Properties.Settings.Default.Height = this.Height;
            }
            Properties.Settings.Default.WindowState = (int)this.WindowState;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// RunWorkerAsync が呼び出されたときに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            // UDPメッセージ受信ポンプ
            ReceiveLoop();
        }

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in this.MdiChildren)
            {
                childForm.Close();
            }
        }

        private void arrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.ArrangeIcons);
        }

        /// <summary>
        /// 配送予定入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem100_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formHaisoYotei == null || _formHaisoYotei.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formHaisoYotei = null;
                    _formHaisoYotei = new Form100();
                    _formHaisoYotei.MdiParent = this;
                    _formHaisoYotei.Location = new Point(0, 0);
                }
                _formHaisoYotei.Show();
                _formHaisoYotei.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 日配持出入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem101_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formNippaiMochidashi == null || _formNippaiMochidashi.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formNippaiMochidashi = null;
                    _formNippaiMochidashi = new Form101();
                    _formNippaiMochidashi.MdiParent = this;
                    _formNippaiMochidashi.Location = new Point(0, 0);
                }
                _formNippaiMochidashi.Show();
                _formNippaiMochidashi.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 生産予定数入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem102_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeisanYotei == null || _formSeisanYotei.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeisanYotei = null;
                    _formSeisanYotei = new Form102();
                    _formSeisanYotei.MdiParent = this;
                    _formSeisanYotei.Location = new Point(0, 0);
                }
                _formSeisanYotei.Show();
                _formSeisanYotei.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 生産予定総数一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem103_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeisanYoteiTotal == null || _formSeisanYoteiTotal.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeisanYoteiTotal = null;
                    _formSeisanYoteiTotal = new Form103();
                    _formSeisanYoteiTotal.MdiParent = this;
                    _formSeisanYoteiTotal.Location = new Point(0, 0);
                }
                _formSeisanYoteiTotal.Show();
                _formSeisanYoteiTotal.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// /// 注文予定数出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem102A_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeisanYoteiFromOrder == null || _formSeisanYoteiFromOrder.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeisanYoteiFromOrder = null;
                    _formSeisanYoteiFromOrder = new Form102A();
                    _formSeisanYoteiFromOrder.MdiParent = this;
                    _formSeisanYoteiFromOrder.Location = new Point(0, 0);
                }
                _formSeisanYoteiFromOrder.Show();
                _formSeisanYoteiFromOrder.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem200_Click(object sender, EventArgs e)
        {
            //if (_formChumon == null || _formChumon.IsDisposed)
            //{
            //    this.Cursor = Cursors.WaitCursor;
            //    _formChumon = null;
            //    _formChumon = new Form200(new ChumonMeisaiCriteria());
            //}
            //_formChumon.MdiParent = this;
            //_formChumon.Location = new Point(0, 0);
            //_formChumon.Show();
            //this.Cursor = Cursors.Default;

            // 同時に複数のフォームを開けるようにする
            this.Cursor = Cursors.WaitCursor;
            Form200 formChumon = new Form200(new ChumonMeisaiCriteria());
            formChumon.MdiParent = this;
            formChumon.Location = new Point(0, 0);
            formChumon.FormClosed += new FormClosedEventHandler(formChumon_FormClosed);
            formChumon.Show();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 注文入力クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formChumon_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form200 form = (Form200)sender;
            form.Dispose();
            form = null;
        }

        /// <summary>
        /// コース注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem201_Click(object sender, EventArgs e)
        {
            //if (_formCourseChumon == null || _formCourseChumon.IsDisposed)
            //{
            //    this.Cursor = Cursors.WaitCursor;
            //    _formCourseChumon = null;
            //    _formCourseChumon = new Form201(new CourseChumonCriteria());
            //}
            //_formCourseChumon.MdiParent = this;
            //_formCourseChumon.Location = new Point(0, 0);
            //_formCourseChumon.Show();
            //this.Cursor = Cursors.Default;

            // 同時に複数のフォームを開けるようにする
            this.Cursor = Cursors.WaitCursor;
            Form201 formCourseChumon = new Form201(new CourseChumonCriteria());
            formCourseChumon.MdiParent = this;
            formCourseChumon.Location = new Point(0, 0);
            formCourseChumon.FormClosed += new FormClosedEventHandler(formCourseChumon_FormClosed);
            formCourseChumon.Show();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// コース注文入力クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formCourseChumon_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form201 form = (Form201)sender;
            form.Dispose();
            form = null;
        }

        /// <summary>
        /// 一括注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem202_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formIkkatsuChumon == null || _formIkkatsuChumon.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formIkkatsuChumon = null;
                    _formIkkatsuChumon = new Form202(new IkkatsuChumonCriteria());
                    _formIkkatsuChumon.MdiParent = this;
                    _formIkkatsuChumon.Location = new Point(0, 0);
                }
                _formIkkatsuChumon.Show();
                _formIkkatsuChumon.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// コース注文一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem203_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formCourseChumonlist == null || _formCourseChumonlist.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formCourseChumonlist = null;
                    _formCourseChumonlist = new Form203();
                    _formCourseChumonlist.MdiParent = this;
                    _formCourseChumonlist.Location = new Point(0, 0);
                }
                _formCourseChumonlist.Show();
                _formCourseChumonlist.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 受注発信一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem204_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formJuchuHassin == null || _formJuchuHassin.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formJuchuHassin = null;
                    _formJuchuHassin = new Form204();
                    _formJuchuHassin.MdiParent = this;
                    _formJuchuHassin.Location = new Point(0, 0);
                }
                _formJuchuHassin.Show();
                _formJuchuHassin.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 固定注文反映
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem205_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formKoteiChumon == null || _formKoteiChumon.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formKoteiChumon = null;
                    _formKoteiChumon = new Form205();
                    _formKoteiChumon.MdiParent = this;
                    _formKoteiChumon.Location = new Point(0, 0);
                }
                _formKoteiChumon.Show();
                _formKoteiChumon.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 納品書発行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem206_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formNohinPublish == null || _formNohinPublish.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formNohinPublish = null;
                    _formNohinPublish = new Form206();
                    _formNohinPublish.MdiParent = this;
                    _formNohinPublish.Location = new Point(0, 0);
                }
                _formNohinPublish.Show();
                _formNohinPublish.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        /// <summary>
        /// 注文一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem207_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formChumonlist == null || _formChumonlist.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formChumonlist = null;
                    _formChumonlist = new Form207();
                    _formChumonlist.MdiParent = this;
                    _formChumonlist.Location = new Point(0, 0);
                }
                _formChumonlist.Show();
                _formChumonlist.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 一括コース注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem208_Click(object sender, EventArgs e)
        {
            //if (_formIkkatsuCourseChumon == null || _formIkkatsuCourseChumon.IsDisposed)
            //{
            //    this.Cursor = Cursors.WaitCursor;
            //    _formIkkatsuCourseChumon = null;
            //    _formIkkatsuCourseChumon = new Form208();
            //}
            //_formIkkatsuCourseChumon.MdiParent = this;
            //_formIkkatsuCourseChumon.Location = new Point(0, 0);
            //_formIkkatsuCourseChumon.Show();
            //this.Cursor = Cursors.Default;

            // 同時に複数のフォームを開けるようにする
            this.Cursor = Cursors.WaitCursor;
            Form208 formIkkatsuCourseChumon = new Form208();
            formIkkatsuCourseChumon.MdiParent = this;
            formIkkatsuCourseChumon.Location = new Point(0, 0);
            formIkkatsuCourseChumon.FormClosed += new FormClosedEventHandler(formIkkatsuCourseChumon_FormClosed);
            formIkkatsuCourseChumon.Show();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 一括コース注文入力クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formIkkatsuCourseChumon_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form208 form = (Form208)sender;
            form.Dispose();
            form = null;
        }

        /// <summary>
        /// 曜日別固定数注文設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem209_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formYoubiChumon == null || _formYoubiChumon.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formYoubiChumon = null;
                    _formYoubiChumon = new Form209();
                    _formYoubiChumon.MdiParent = this;
                    _formYoubiChumon.Location = new Point(0, 0);
                }
                _formYoubiChumon.Show();
                _formYoubiChumon.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 過不足一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem300_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formKafuzoku == null || _formKafuzoku.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formKafuzoku = null;
                    _formKafuzoku = new Form300();
                    _formKafuzoku.MdiParent = this;
                    _formKafuzoku.Location = new Point(0, 0);
                }
                _formKafuzoku.Show();
                _formKafuzoku.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 移動入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem301_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formIdoNyuryoku == null || _formIdoNyuryoku.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formIdoNyuryoku = null;
                    _formIdoNyuryoku = new Form301();
                    _formIdoNyuryoku.MdiParent = this;
                    _formIdoNyuryoku.Location = new Point(0, 0);
                }
                _formIdoNyuryoku.Show();
                _formIdoNyuryoku.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 臨時コース組み換え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem302_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formRinjiCourse == null || _formRinjiCourse.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formRinjiCourse = null;
                    _formRinjiCourse = new Form302();
                    _formRinjiCourse.MdiParent = this;
                    _formRinjiCourse.Location = new Point(0, 0);
                }
                _formRinjiCourse.Show();
                _formRinjiCourse.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 容器回収表印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem303_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formYokiKaisyuHyo == null || _formYokiKaisyuHyo.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formYokiKaisyuHyo = null;
                    _formYokiKaisyuHyo = new Form303();
                    _formYokiKaisyuHyo.MdiParent = this;
                    _formYokiKaisyuHyo.Location = new Point(0, 0);
                }
                _formYokiKaisyuHyo.Show();
                _formYokiKaisyuHyo.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 短冊印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem304_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formTanzakuPrint == null || _formTanzakuPrint.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formTanzakuPrint = null;
                    _formTanzakuPrint = new Form304();
                    _formTanzakuPrint.MdiParent = this;
                    _formTanzakuPrint.Location = new Point(0, 0);
                }
                _formTanzakuPrint.Show();
                _formTanzakuPrint.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// コース出荷状態
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem305_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formShippingStatus == null || _formShippingStatus.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formShippingStatus = null;
                    _formShippingStatus = new Form305();
                    _formShippingStatus.MdiParent = this;
                    _formShippingStatus.Location = new Point(0, 0);
                }
                _formShippingStatus.Show();
                _formShippingStatus.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 大袋ラベル印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem307_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formBigBagLabelPrint == null || _formBigBagLabelPrint.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formBigBagLabelPrint = null;
                    _formBigBagLabelPrint = new Form307();
                    _formBigBagLabelPrint.MdiParent = this;
                    _formBigBagLabelPrint.Location = new Point(0, 0);
                }
                _formBigBagLabelPrint.Show();
                _formBigBagLabelPrint.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売上入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem400_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUriage == null || _formUriage.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUriage = null;
                    _formUriage = new Form400();
                    _formUriage.MdiParent = this;
                    _formUriage.Location = new Point(0, 0);
                }
                _formUriage.Show();
                _formUriage.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売上明細入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem401_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUriageMeisai == null || _formUriageMeisai.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUriageMeisai = null;
                    _formUriageMeisai = new Form401(new UriageMeisaiCriteria());
                    _formUriageMeisai.MdiParent = this;
                    _formUriageMeisai.Location = new Point(0, 0);
                }
                _formUriageMeisai.Show();
                _formUriageMeisai.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売上一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem402_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUriageList == null || _formUriageList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUriageList = null;
                    _formUriageList = new Form402();
                    _formUriageList.MdiParent = this;
                    _formUriageList.Location = new Point(0, 0);
                }
                _formUriageList.Show();
                _formUriageList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売上精算入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem403_Click(object sender, EventArgs e)
        {
            //if (_formUriageSeisan == null || _formUriageSeisan.IsDisposed)
            //{
            //    this.Cursor = Cursors.WaitCursor;
            //    _formUriageSeisan = null;
            //    _formUriageSeisan = new Form403(new UriageSeisanCriteria());
            //    _formUriageSeisan.MdiParent = this;
            //    _formUriageSeisan.Location = new Point(0, 0);
            //}
            //_formUriageSeisan.Show();
            //_formUriageSeisan.Activate();
            //this.Cursor = Cursors.Default;

            // 同時に複数のフォームを開けるようにする
            this.Cursor = Cursors.WaitCursor;
            Form403 formUriageSeisan = new Form403(new UriageSeisanCriteria());
            formUriageSeisan.MdiParent = this;
            formUriageSeisan.Location = new Point(0, 0);
            formUriageSeisan.FormClosed += new FormClosedEventHandler(formUriageSeisan_FormClosed);
            formUriageSeisan.Show();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 売上精算入力クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formUriageSeisan_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form403 form = (Form403)sender;
            form.Dispose();
            form = null;
        }

        /// <summary>
        /// 売上精算一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem404_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUriageSeisanList == null || _formUriageSeisanList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUriageSeisanList = null;
                    _formUriageSeisanList = new Form404();
                    _formUriageSeisanList.MdiParent = this;
                    _formUriageSeisanList.Location = new Point(0, 0);
                }
                _formUriageSeisanList.Show();
                _formUriageSeisanList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売上集計表印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem405_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUriageNikkeiHyo == null || _formUriageNikkeiHyo.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUriageNikkeiHyo = null;
                    _formUriageNikkeiHyo = new Form405();
                    _formUriageNikkeiHyo.MdiParent = this;
                    _formUriageNikkeiHyo.Location = new Point(0, 0);
                }
                _formUriageNikkeiHyo.Show();
                _formUriageNikkeiHyo.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 未分配一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem407_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formMiBunpaiList == null || _formMiBunpaiList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formMiBunpaiList = null;
                    _formMiBunpaiList = new Form407();
                    _formMiBunpaiList.MdiParent = this;
                    _formMiBunpaiList.Location = new Point(0, 0);
                }
                _formMiBunpaiList.Show();
                _formMiBunpaiList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 販売実績一覧表印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem408_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formHanbaiJissekiList == null || _formHanbaiJissekiList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formHanbaiJissekiList = null;
                    _formHanbaiJissekiList = new Form408();
                    _formHanbaiJissekiList.MdiParent = this;
                    _formHanbaiJissekiList.Location = new Point(0, 0);
                }
                _formHanbaiJissekiList.Show();
                _formHanbaiJissekiList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求締処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem500_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeikyuShime == null || _formSeikyuShime.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeikyuShime = null;
                    _formSeikyuShime = new Form500();
                    _formSeikyuShime.MdiParent = this;
                    _formSeikyuShime.Location = new Point(0, 0);
                }
                _formSeikyuShime.Show();
                _formSeikyuShime.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem501_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeikyuList == null || _formSeikyuList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeikyuList = null;
                    _formSeikyuList = new Form501();
                    _formSeikyuList.MdiParent = this;
                    _formSeikyuList.Location = new Point(0, 0);
                }
                _formSeikyuList.Show();
                _formSeikyuList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求書発行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem503_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeikyuPublish == null || _formSeikyuPublish.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeikyuPublish = null;
                    _formSeikyuPublish = new Form503();
                    _formSeikyuPublish.MdiParent = this;
                    _formSeikyuPublish.Location = new Point(0, 0);
                }
                _formSeikyuPublish.Show();
                _formSeikyuPublish.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求EBデータ出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem504_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeikyuDataEB == null || _formSeikyuDataEB.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeikyuDataEB = null;
                    _formSeikyuDataEB = new Form504();
                    _formSeikyuDataEB.MdiParent = this;
                    _formSeikyuDataEB.Location = new Point(0, 0);
                }
                _formSeikyuDataEB.Show();
                _formSeikyuDataEB.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売掛残高一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem505_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUrikakeZanList == null || _formUrikakeZanList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUrikakeZanList = null;
                    _formUrikakeZanList = new Form505();
                    _formUrikakeZanList.MdiParent = this;
                    _formUrikakeZanList.Location = new Point(0, 0);
                }
                _formUrikakeZanList.Show();
                _formUrikakeZanList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求EBデータ再出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem507_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSeikyuDataEBReCreate == null || _formSeikyuDataEBReCreate.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSeikyuDataEBReCreate = null;
                    _formSeikyuDataEBReCreate = new Form507();
                    // 別のインスタンスが開いていればそちらをアクティブにする（未回収金一覧からも起動される）
                    if (_formSeikyuDataEBReCreate.ActivateAnotherMe())
                    {
                        _formSeikyuDataEBReCreate = null;
                        return;
                    }
                    _formSeikyuDataEBReCreate.MdiParent = this;
                    _formSeikyuDataEBReCreate.Location = new Point(0, 0);
                }
                _formSeikyuDataEBReCreate.Show();
                _formSeikyuDataEBReCreate.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求レコード追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem508_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(this, "「請求レコード追加」はメンテナンス用の機能です。\n普段の業務での使用を目的としたものではありません。\n\nこの画面を開きますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.OK)
                {
                    if (_formSeikyuAdd == null || _formSeikyuAdd.IsDisposed)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        _formSeikyuAdd = null;
                        _formSeikyuAdd = new Form508();
                        _formSeikyuAdd.MdiParent = this;
                        _formSeikyuAdd.Location = new Point(0, 0);
                    }
                    _formSeikyuAdd.Show();
                    _formSeikyuAdd.Activate();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 電子決済請求締処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem509_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formPaySeikyuShime == null || _formPaySeikyuShime.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formPaySeikyuShime = null;
                    _formPaySeikyuShime = new Form509();
                    _formPaySeikyuShime.MdiParent = this;
                    _formPaySeikyuShime.Location = new Point(0, 0);
                }
                _formPaySeikyuShime.Show();
                _formPaySeikyuShime.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 電子決済請求一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem510_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formPaySeikyuList == null || _formPaySeikyuList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formPaySeikyuList = null;
                    _formPaySeikyuList = new Form510();
                    _formPaySeikyuList.MdiParent = this;
                    _formPaySeikyuList.Location = new Point(0, 0);
                }
                _formPaySeikyuList.Show();
                _formPaySeikyuList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// TP決済一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem512_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formTapionList == null || _formTapionList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formTapionList = null;
                    _formTapionList = new Form512();
                    _formTapionList.MdiParent = this;
                    _formTapionList.Location = new Point(0, 0);
                }
                _formTapionList.Show();
                _formTapionList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// クレカ決済一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem513_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formFRSettlementList == null || _formFRSettlementList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formFRSettlementList = null;
                    _formFRSettlementList = new Form513(null);
                    _formFRSettlementList.MdiParent = this;
                    _formFRSettlementList.Location = new Point(0, 0);
                }
                _formFRSettlementList.Show();
                _formFRSettlementList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 入金一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem600_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formNyukinList == null || _formNyukinList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formNyukinList = null;
                    _formNyukinList = new Form600();
                    _formNyukinList.MdiParent = this;
                    _formNyukinList.Location = new Point(0, 0);
                }
                _formNyukinList.Show();
                _formNyukinList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 入金入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem601_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formNyukinMeisai == null || _formNyukinMeisai.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formNyukinMeisai = null;
                    _formNyukinMeisai = new Form601(new NyukinTarget());
                    _formNyukinMeisai.MdiParent = this;
                    _formNyukinMeisai.Location = new Point(0, 0);
                }
                _formNyukinMeisai.Show();
                _formNyukinMeisai.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 入金EBデータ取込
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem602_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formNyukinEBImport == null || _formNyukinEBImport.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formNyukinEBImport = null;
                    _formNyukinEBImport = new Form602();
                    _formNyukinEBImport.MdiParent = this;
                    _formNyukinEBImport.Location = new Point(0, 0);
                }
                _formNyukinEBImport.Show();
                _formNyukinEBImport.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 未回収金一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem603_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formMikaisyuList == null || _formMikaisyuList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formMikaisyuList = null;
                    _formMikaisyuList = new Form603();
                    _formMikaisyuList.MdiParent = this;
                    _formMikaisyuList.Location = new Point(0, 0);
                }
                _formMikaisyuList.Show();
                _formMikaisyuList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 銀行APIファイル取込
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem604_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formAnserApiImport == null || _formAnserApiImport.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formAnserApiImport = null;
                    _formAnserApiImport = new Form604();
                    _formAnserApiImport.MdiParent = this;
                    _formAnserApiImport.Location = new Point(0, 0);
                }
                _formAnserApiImport.Show();
                _formAnserApiImport.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 得意先マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem700_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formTokuisaki == null || _formTokuisaki.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formTokuisaki = null;
                    _formTokuisaki = new Form700();
                    _formTokuisaki.MdiParent = this;
                    _formTokuisaki.Location = new Point(0, 0);
                }
                _formTokuisaki.Show();
                _formTokuisaki.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 得意先単価適用マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem701_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formTankaTekiyou == null || _formTankaTekiyou.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formTankaTekiyou = null;
                    _formTankaTekiyou = new Form701();
                    _formTankaTekiyou.MdiParent = this;
                    _formTankaTekiyou.Location = new Point(0, 0);
                }
                _formTankaTekiyou.Show();
                _formTankaTekiyou.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 商品マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem702_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSyohin == null || _formSyohin.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSyohin = null;
                    _formSyohin = new Form702();
                    _formSyohin.MdiParent = this;
                    _formSyohin.Location = new Point(0, 0);
                }
                _formSyohin.Show();
                _formSyohin.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 主要部品リストマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem703_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSyuyoBuhin == null || _formSyuyoBuhin.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSyuyoBuhin = null;
                    _formSyuyoBuhin = new Form703();
                    _formSyuyoBuhin.MdiParent = this;
                    _formSyuyoBuhin.Location = new Point(0, 0);
                }
                _formSyuyoBuhin.Show();
                _formSyuyoBuhin.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 商品グループマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem704_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSyohinGourp == null || _formSyohinGourp.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSyohinGourp = null;
                    _formSyohinGourp = new Form704();
                    _formSyohinGourp.MdiParent = this;
                    _formSyohinGourp.Location = new Point(0, 0);
                }
                _formSyohinGourp.Show();
                _formSyohinGourp.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 主要商品リストマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem705_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSyuyoSyohin == null || _formSyuyoSyohin.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSyuyoSyohin = null;
                    _formSyuyoSyohin = new Form705();
                    _formSyuyoSyohin.MdiParent = this;
                    _formSyuyoSyohin.Location = new Point(0, 0);
                }
                _formSyuyoSyohin.Show();
                _formSyuyoSyohin.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        /// <summary>
        /// 担当者マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem706_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formTantousya == null || _formTantousya.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formTantousya = null;
                    _formTantousya = new Form706();
                    _formTantousya.MdiParent = this;
                    _formTantousya.Location = new Point(0, 0);
                }
                _formTantousya.Show();
                _formTantousya.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 部署マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem707_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formBusyo == null || _formBusyo.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formBusyo = null;
                    _formBusyo = new Form707();
                    _formBusyo.MdiParent = this;
                    _formBusyo.Location = new Point(0, 0);
                }
                _formBusyo.Show();
                _formBusyo.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// コースマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem708_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formCource == null || _formCource.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formCource = null;
                    _formCource = new Form708();
                    _formCource.MdiParent = this;
                    _formCource.Location = new Point(0, 0);
                }
                _formCource.Show();
                _formCource.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// コース別グループマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem709_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formCourceGroup == null || _formCourceGroup.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formCourceGroup = null;
                    _formCourceGroup = new Form709();
                    _formCourceGroup.MdiParent = this;
                    _formCourceGroup.Location = new Point(0, 0);
                }
                _formCourceGroup.Show();
                _formCourceGroup.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 管理マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem710_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formKanri == null || _formKanri.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formKanri = null;
                    _formKanri = new Form710();
                    _formKanri.MdiParent = this;
                    _formKanri.Location = new Point(0, 0);
                }
                _formKanri.Show();
                _formKanri.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 備考マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem711_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formBiko == null || _formBiko.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formBiko = null;
                    _formBiko = new Form711();
                    _formBiko.MdiParent = this;
                    _formBiko.Location = new Point(0, 0);
                }
                _formBiko.Show();
                _formBiko.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 祝日マスタメンテ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem713_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formShukujitsu == null || _formShukujitsu.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formShukujitsu = null;
                    _formShukujitsu = new Form713();
                    _formShukujitsu.MdiParent = this;
                    _formShukujitsu.Location = new Point(0, 0);
                }
                _formShukujitsu.Show();
                _formShukujitsu.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 帳票／プリンタ設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem714_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formReportPrinter == null || _formReportPrinter.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formReportPrinter = null;
                    _formReportPrinter = new Form714();
                    _formReportPrinter.MdiParent = this;
                    _formReportPrinter.Location = new Point(0, 0);
                }
                _formReportPrinter.Show();
                _formReportPrinter.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 支店マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem715_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formShiten == null || _formShiten.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formShiten = null;
                    _formShiten = new Form715();
                    _formShiten.MdiParent = this;
                    _formShiten.Location = new Point(0, 0);
                }
                _formShiten.Show();
                _formShiten.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 非顧客電話番号削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem716_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formDeleteHiKokyaku == null || _formDeleteHiKokyaku.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formDeleteHiKokyaku = null;
                    _formDeleteHiKokyaku = new Form716();
                    _formDeleteHiKokyaku.MdiParent = this;
                    _formDeleteHiKokyaku.Location = new Point(0, 0);
                }
                _formDeleteHiKokyaku.Show();
                _formDeleteHiKokyaku.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 支店マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem717_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formShiwake == null || _formShiwake.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formShiwake = null;
                    _formShiwake = new Form717();
                    _formShiwake.MdiParent = this;
                    _formShiwake.Location = new Point(0, 0);
                }
                _formShiwake.Show();
                _formShiwake.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// マスタエクスポート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem790_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formExport == null || _formExport.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formExport = null;
                    _formExport = new Form790();
                    _formExport.MdiParent = this;
                    _formExport.Location = new Point(0, 0);
                }
                _formExport.Show();
                _formExport.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// バージョン情報
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem800_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formVersion == null || _formVersion.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formVersion = null;
                    _formVersion = new Form800();
                    _formVersion.MdiParent = this;
                    _formVersion.Location = new Point(0, 0);
                }
                _formVersion.Show();
                _formVersion.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 操作履歴一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem806_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formSousaHistory == null || _formSousaHistory.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formSousaHistory = null;
                    _formSousaHistory = new Form806();
                    _formSousaHistory.MdiParent = this;
                    _formSousaHistory.Location = new Point(0, 0);
                }
                _formSousaHistory.Show();
                _formSousaHistory.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 月次集計
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem810_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formGetsujiSyukei == null || _formGetsujiSyukei.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formGetsujiSyukei = null;
                    _formGetsujiSyukei = new Form810();
                    _formGetsujiSyukei.MdiParent = this;
                    _formGetsujiSyukei.Location = new Point(0, 0);
                }
                _formGetsujiSyukei.Show();
                _formGetsujiSyukei.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 月次売掛残高一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem811_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formGetsujiUrikakeZanList == null || _formGetsujiUrikakeZanList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formGetsujiUrikakeZanList = null;
                    _formGetsujiUrikakeZanList = new Form811();
                    _formGetsujiUrikakeZanList.MdiParent = this;
                    _formGetsujiUrikakeZanList.Location = new Point(0, 0);
                }
                _formGetsujiUrikakeZanList.Show();
                _formGetsujiUrikakeZanList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 月次得意先別実績一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem813_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formGetsujiTokuisakiList == null || _formGetsujiTokuisakiList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formGetsujiTokuisakiList = null;
                    _formGetsujiTokuisakiList = new Form813();
                    _formGetsujiTokuisakiList.MdiParent = this;
                    _formGetsujiTokuisakiList.Location = new Point(0, 0);
                }
                _formGetsujiTokuisakiList.Show();
                _formGetsujiTokuisakiList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 月次商品別売上一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem814_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formGetsujiSyohinList == null || _formGetsujiSyohinList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formGetsujiSyohinList = null;
                    _formGetsujiSyohinList = new Form814();
                    _formGetsujiSyohinList.MdiParent = this;
                    _formGetsujiSyohinList.Location = new Point(0, 0);
                }
                _formGetsujiSyohinList.Show();
                _formGetsujiSyohinList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 得意先別売上推移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem815_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formGetsujiTokuisakiSuiiList == null || _formGetsujiTokuisakiSuiiList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formGetsujiTokuisakiSuiiList = null;
                    _formGetsujiTokuisakiSuiiList = new Form815();
                    _formGetsujiTokuisakiSuiiList.MdiParent = this;
                    _formGetsujiTokuisakiSuiiList.Location = new Point(0, 0);
                }
                _formGetsujiTokuisakiSuiiList.Show();
                _formGetsujiTokuisakiSuiiList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 随時売上入金一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem816_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formZuijiUriNyuList == null || _formZuijiUriNyuList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formZuijiUriNyuList = null;
                    _formZuijiUriNyuList = new Form816();
                    _formZuijiUriNyuList.MdiParent = this;
                    _formZuijiUriNyuList.Location = new Point(0, 0);
                }
                _formZuijiUriNyuList.Show();
                _formZuijiUriNyuList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 集計タイプ別売上分析表印刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem817_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formUriageBunsekiHyo == null || _formUriageBunsekiHyo.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formUriageBunsekiHyo = null;
                    _formUriageBunsekiHyo = new Form817();
                    _formUriageBunsekiHyo.MdiParent = this;
                    _formUriageBunsekiHyo.Location = new Point(0, 0);
                }
                _formUriageBunsekiHyo.Show();
                _formUriageBunsekiHyo.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// クレカ利用状況一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem818_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formFRUseSummaryList == null || _formFRUseSummaryList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formFRUseSummaryList = null;
                    _formFRUseSummaryList = new Form818();
                    _formFRUseSummaryList.MdiParent = this;
                    _formFRUseSummaryList.Location = new Point(0, 0);
                }
                _formFRUseSummaryList.Show();
                _formFRUseSummaryList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// クレカ利用明細一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem819_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formFRUseDetailList == null || _formFRUseDetailList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formFRUseDetailList = null;
                    _formFRUseDetailList = new Form819();
                    _formFRUseDetailList.MdiParent = this;
                    _formFRUseDetailList.Location = new Point(0, 0);
                }
                _formFRUseDetailList.Show();
                _formFRUseDetailList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Web注文履歴一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem820_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formWebHistory == null || _formWebHistory.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formWebHistory = null;
                    _formWebHistory = new Form820();
                    _formWebHistory.MdiParent = this;
                    _formWebHistory.Location = new Point(0, 0);
                }
                _formWebHistory.Show();
                _formWebHistory.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Web得意先マスタ一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem821_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formWebTokuisaki == null || _formWebTokuisaki.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formWebTokuisaki = null;
                    _formWebTokuisaki = new Form821();
                    _formWebTokuisaki.MdiParent = this;
                    _formWebTokuisaki.Location = new Point(0, 0);
                }
                _formWebTokuisaki.Show();
                _formWebTokuisaki.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Web用部署設定マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem822_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formWebSetting == null || _formWebSetting.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formWebSetting = null;
                    _formWebSetting = new Form822();
                    _formWebSetting.MdiParent = this;
                    _formWebSetting.Location = new Point(0, 0);
                }
                _formWebSetting.Show();
                _formWebSetting.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Web用注文不可日設定一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem830_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formWebChumonFuka == null || _formWebChumonFuka.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formWebChumonFuka = null;
                    _formWebChumonFuka = new Form830();
                    _formWebChumonFuka.MdiParent = this;
                    _formWebChumonFuka.Location = new Point(0, 0);
                }
                _formWebChumonFuka.Show();
                _formWebChumonFuka.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Web注文用商品設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem831_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formWebChumonFukaSyohin == null || _formWebChumonFukaSyohin.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formWebChumonFukaSyohin = null;
                    _formWebChumonFukaSyohin = new Form831();
                    _formWebChumonFukaSyohin.MdiParent = this;
                    _formWebChumonFukaSyohin.Location = new Point(0, 0);
                }
                _formWebChumonFukaSyohin.Show();
                _formWebChumonFukaSyohin.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Web用注文停止日設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem832_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formWebChumonTeishi == null || _formWebChumonTeishi.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formWebChumonTeishi = null;
                    _formWebChumonTeishi = new Form832();
                    _formWebChumonTeishi.MdiParent = this;
                    _formWebChumonTeishi.Location = new Point(0, 0);
                }
                _formWebChumonTeishi.Show();
                _formWebChumonTeishi.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 会計連携CSV出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem840_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formKaikeiRenkei == null || _formKaikeiRenkei.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formKaikeiRenkei = null;
                    _formKaikeiRenkei = new Form840();
                    _formKaikeiRenkei.MdiParent = this;
                    _formKaikeiRenkei.Location = new Point(0, 0);
                }
                _formKaikeiRenkei.Show();
                _formKaikeiRenkei.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 会計連携集計一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem841_Click(object sender, EventArgs e)
        {
            try
            {
                if (_formKaikeiRenkeiList == null || _formKaikeiRenkeiList.IsDisposed)
                {
                    this.Cursor = Cursors.WaitCursor;
                    _formKaikeiRenkeiList = null;
                    _formKaikeiRenkeiList = new Form841();
                    _formKaikeiRenkeiList.MdiParent = this;
                    _formKaikeiRenkeiList.Location = new Point(0, 0);
                }
                _formKaikeiRenkeiList.Show();
                _formKaikeiRenkeiList.Activate();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 権限によりメニューを制御
        /// </summary>
        private void SetMenuEnable()
        {
            DataTable dt = GetAuth();

            SetMenuEnableMain("配送準備", haisoJunbiMenu, dt);
            SetMenuEnableMain("受注", juchuMenu, dt);
            SetMenuEnableMain("配送", haisoMenu, dt);
            SetMenuEnableMain("売上", uriageMenu, dt);
            SetMenuEnableMain("請求", seikyuMenu, dt);
            SetMenuEnableMain("電子決済", paySeikyuMenu, dt);
            SetMenuEnableMain("入金", nyuukinMenu, dt);
            SetMenuEnableMain("分析", bunsekiMenu, dt);
            SetMenuEnableMain("マスタメンテ", masterMenu, dt);
            SetMenuEnableMain("システム", systemMenu, dt);
            SetMenuEnableMain("Web", webMenu, dt);
        }

        /// <summary>
        /// 画面権限マスタを読み込む
        /// </summary>
        /// <returns>画面権限マスタのデータテーブル</returns>
        private DataTable GetAuth()
        {
            DataTable dt = null;
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT " +
                                      "\r\n     画面権限No, " +
                                      "\r\n     メニュー名, " +
                                      "\r\n     画面名, " +
                                      "\r\n     閲覧可能フラグ, " +
                                      "\r\n     編集可能フラグ " +
                                      "\r\n FROM " +
                                      "\r\n     画面権限マスタ " +
                                      "\r\n WHERE " +
                                      "\r\n     権限CD = @権限CD ";
                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@権限CD", SqlDbType.Int).Value = userData.権限区分 ?? -1;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adp.Fill(dt);
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
            return dt;
        }

        /// <summary>
        /// 権限により個々のメニューを制御
        /// </summary>
        /// <param name="menuTitle">メニュータイトル</param>
        /// <param name="mainMenuItem">メニューアイテム</param>
        /// <param name="dt">画面権限マスタデータテーブル</param>
        private void SetMenuEnableMain(string menuTitle, ToolStripMenuItem mainMenuItem, DataTable dt)
        {
            bool mainMenuEnabled = false;       // メニューアイテムのEnabled

            // 権限に基づき、子メニューアイテムのEnabledを設定する
            foreach (ToolStripItem item in mainMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuChild = (ToolStripMenuItem)item;
                    string menuText = Regex.Replace(menuChild.Text, "\\(&.\\)", "");
                    DataRow[] drs = dt.Select(string.Format("メニュー名 = '{0}' and 画面名 = '{1}'", dt.Escape(menuTitle), dt.Escape(menuText)));
                    if (drs != null && drs.Length > 0)
                    {
                        menuChild.Enabled = (bool)drs[0]["閲覧可能フラグ"];
                        if (menuChild.Enabled)
                        {
                            mainMenuEnabled = true;     // 子に１つでもTrueがあれば、親もTrue
                        }
                    }
                    else
                    {
                        mainMenuEnabled = true;         // 権限テーブルに無いメニューは使用可能と見なす
                    }
                }
            }

            mainMenuItem.Enabled = mainMenuEnabled;
        }

        /// <summary>
        /// 非活性メニューの制御
        /// </summary>
        private void SetMenuDisable()
        {
            // メニュー非活性フォームIDのリストを取得
            List<int> disableList = SystemMaster.GetDisableMenuIDs();

            SetMenuDisableMain(haisoJunbiMenu, disableList);
            SetMenuDisableMain(juchuMenu, disableList);
            SetMenuDisableMain(haisoMenu, disableList);
            SetMenuDisableMain(uriageMenu, disableList);
            SetMenuDisableMain(seikyuMenu, disableList);
            SetMenuDisableMain(paySeikyuMenu, disableList);
            SetMenuDisableMain(nyuukinMenu, disableList);
            SetMenuDisableMain(bunsekiMenu, disableList);
            SetMenuDisableMain(masterMenu, disableList);
            SetMenuDisableMain(systemMenu, disableList);
            SetMenuDisableMain(webMenu, disableList);
        }

        /// <summary>
        /// 個々の非活性メニューの制御
        /// </summary>
        /// <param name="mainMenuItem">メニューアイテム</param>
        /// <param name="disableList">メニュー非活性フォームIDのリスト</param>
        private void SetMenuDisableMain(ToolStripMenuItem mainMenuItem, List<int> disableList)
        {
            // 権限に基づき、子メニューアイテムのEnabledを設定する
            foreach (ToolStripItem item in mainMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuChild = (ToolStripMenuItem)item;
                    // 子メニューアイテムの名前の末尾3文字をID(数値)として取得
                    string suffix = menuChild.Name.Substring(menuChild.Name.Length - 3);
                    int id;
                    if (int.TryParse(suffix, out id))
                    {
                        // メニュー非活性フォームIDのリストに無ければ使用可能
                        if (disableList.Contains(id))
                        {
                            menuChild.Enabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 受注発信一覧の表示条件（＝担当者架電コースマスタ）のコースで、コース注文入力を開く
        /// </summary>
        private void OpenForm201()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     K.コースＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     担当者架電コースマスタ AS K" +
                                      "\r\n     INNER JOIN" +
                                      // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n         (SELECT" +
                                      "\r\n             CM.コースＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             コースマスタ AS CM" +
                                      "\r\n             LEFT JOIN" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     M.コースＣＤ" +
                                      "\r\n                 FROM" +
                                      "\r\n                     臨時コースマスタ AS M" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(M.無効フラグ, 0) = 0" +
                                      "\r\n                     AND M.実施日 = @注文日" +
                                      "\r\n                     AND M.コース区分 = @コース区分" +
                                      "\r\n                 ) AS RM" +
                                      "\r\n                 ON RM.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             AND CM.コース区分 = @コース区分" +
                                      "\r\n             AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             RM.コースＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コースマスタ AS RM" +
                                      "\r\n         WHERE" +
                                      "\r\n             RM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n             AND RM.実施日 = @注文日" +
                                      "\r\n             AND RM.コース区分 = @コース区分" +
                                      "\r\n         ) AS CT" +
                                      "\r\n         ON CT.コースＣＤ = K.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     K.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND K.担当者ＣＤ = @担当者ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     K.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = DateTime.Today;

                    // 注文日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun(DateTime.Today);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("担当者架電コースマスタ");
                    adp.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        // コース注文入力検索条件
                        CourseChumonCriteria courseChumonCriteria = new CourseChumonCriteria();
                        courseChumonCriteria.注文日 = DateTime.Today;
                        courseChumonCriteria.コースＣＤ = dr["コースＣＤ"] as int?;

                        Form201 formCourseChumon = new Form201(courseChumonCriteria);
                        if (formCourseChumon.HasAuthBrowse == false)
                        {
                            MessageBox.Show(this, "画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        formCourseChumon.MdiParent = this;
                        formCourseChumon.StartPosition = FormStartPosition.WindowsDefaultLocation;
                        formCourseChumon.FormClosed += new FormClosedEventHandler(formCourseChumon_FormClosed);
                        formCourseChumon.Show();
                    }
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// CtiService起動
        /// </summary>
        private void StartupCtiService()
        {
            try
            {
                if (!this.UseCti) return;
                if (Process.GetProcessesByName("CtiService").Length <= 0)
                {
                    Process p = Process.Start("CtiService.exe");
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// UDPメッセージ受信ポンプ
        /// </summary>
        private void ReceiveLoop()
        {
            do
            {
                if (_exitReceiveLoop) return;
                string MSG = this.ReceiveMessage(this.MenuReceivePort);
                string[] ans = MSG.Split(':');
                if (ans[0].Equals("TELCONNECT"))
                {
                    if (ans.Length > 1)
                    {
                        Invoke(new delegate_TelConnect(TelConnect), new object[] { ans[1] });
                    }
                }
            } while (true);
        }

        /// <summary>
        /// UDPメッセージ受信
        /// </summary>
        /// <param name="localPort">自身のポート番号</param>
        /// <returns>受信メッセージ</returns>
        private string ReceiveMessage(int localPort)
        {
            string rcvMsg = "";

            //文字コードを指定する
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            System.Net.IPEndPoint remoteEP = null;
            try
            {
                //データを受信する
                byte[] rcvBytes = udpRecv.Receive(ref remoteEP);
                rcvMsg = enc.GetString(rcvBytes);
            }
            catch
            {
            }
            return rcvMsg;
        }

        /// <summary>
        /// 注文入力表示
        /// </summary>
        /// <param name="tokuisaki">得意先マスタ</param>
        private void ShowChumonNyuryoku(Tokuisaki tokuisaki)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ChumonMeisaiCriteria chumonCriteria = new ChumonMeisaiCriteria();
                chumonCriteria.stopOverwrite = true;
                chumonCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;
                chumonCriteria.得意先名 = tokuisaki.得意先名;
                chumonCriteria.部署ＣＤ = tokuisaki.部署ＣＤ;
                chumonCriteria.注文日 = DateTime.Today;

                Form200 formChumon = new Form200(chumonCriteria);
                if (formChumon.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                formChumon.MdiParent = this;
                formChumon.StartPosition = FormStartPosition.WindowsDefaultLocation;
                formChumon.CTIOpened = true;
                formChumon.FormClosed += new FormClosedEventHandler(formChumon_FormClosed);
                formChumon.Show();
                this.Cursor = Cursors.Default;

                History.Save(string.Format("【{0}】の注文入力画面を表示しました。", tokuisaki.得意先名));
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// 得意先登録表示
        /// </summary>
        /// <param name="tokuisaki">得意先マスタ</param>
        private void ShowTokuisakiToroku(Tokuisaki tokuisaki)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                Form700D formTokuisaki = new Form700D(tokuisaki);
                if (formTokuisaki.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                formTokuisaki.MdiParent = this;
                formTokuisaki.Location = new Point(0, 0);
                formTokuisaki.Show();
                this.Cursor = Cursors.Default;

                History.Save("得意先登録画面を表示しました。");
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }
        #endregion

        #region 電話接続時
        /// <summary>
        /// 電話接続時
        /// </summary>
        /// <param name="telno">電話番号</param>
        private delegate void delegate_TelConnect(string telno);
        private void TelConnect(string telno)
        {
            //非通知
            if (telno.Equals(string.Empty))
            {
                TelConnectHituti();
            }
            else if (IsNotKokyakuTelNo(telno))
            {
                //非顧客登録済み
            }
            else if (HasTokuisaiTelNo(telno))
            {
                //得意先有り
            }
            else
            {
                //得意先登録なし
                this.Activate();
                TelConnectNewComer(telno);
            }
        }
        #endregion

        #region 非通知受信
        /// <summary>
        /// 非通知受信
        /// </summary>
        private void TelConnectHituti()
        {
            try
            {
                History.Save("非通知着信しました。");

                this.Cursor = Cursors.WaitCursor;

                // 得意先選択ダイアログを表示
                Form900M formTokuisakiDlg = new Form900M("非通知", DateTime.Now);
                formTokuisakiDlg.MdiParent = this;
                formTokuisakiDlg.Location = new Point(0, 0);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900MHituti_FormClosed);
                formTokuisakiDlg.Show();
                this.Cursor = Cursors.Default;
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 得意先選択ダイアログクローズ（非通知受信時）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900MHituti_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Enabled = true;
                this.Activate();
                Form900M form = (Form900M)sender;
                // 該当する得意先があった？
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Tokuisaki tokuisaki = form.SelectedTokuisaki;

                    // ***************************************
                    // 非通知の番号はC_TelToCustには登録しない
                    // ***************************************

                    // 注文入力画面を表示
                    ShowChumonNyuryoku(tokuisaki);
                }
                else
                {
                    // 該当する得意先が無かった
                    if (MessageBox.Show(this, "新規客情報を登録します。\nよろしいですか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        // 得意先マスタメンテを開く
                        ShowTokuisakiToroku(new Tokuisaki());
                    }
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 非顧客電話番号存在確認
        /// <summary>
        /// 非顧客電話番号存在確認
        /// </summary>
        private bool IsNotKokyakuTelNo(string telno)
        {
            try
            {
                bool hasNotKokyakuTelNo = false;

                // TODO:もし直すようにことがあれば、HiKokyakuTelNoを使うようにする
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     T.電話番号" +
                                      "\r\n FROM" +
                                      "\r\n     非顧客電話番号マスタ AS T" +
                                      "\r\n WHERE" +
                                      "\r\n     T.企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n     T.電話番号 = @電話番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = telno;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("非顧客電話番号マスタ");
                    adp.Fill(dt);
                    if (dt.Rows.Count != 0)
                    {
                        History.Save(con, string.Format("非顧客から着信しました。({0})", telno));
                        hasNotKokyakuTelNo = true;
                    }
                }
                return hasNotKokyakuTelNo;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }
        #endregion

        #region 得意先電話番号存在
        /// <summary>
        /// 得意先電話番号存在確認＆注文画面表示
        /// </summary>
        private bool HasTokuisaiTelNo(string telno)
        {
            try
            {
                bool hasTokuisai = false;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     T.Tel_CustNo" +
                                      "\r\n FROM" +
                                      "\r\n     C_TelToCust AS T" +
                                      "\r\n WHERE" +
                                      "\r\n     T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND T.Tel_TelNo = @Tel_TelNo";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@Tel_TelNo", SqlDbType.VarChar).Value = telno;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("C_TelToCust");
                    adp.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        return false;
                    }

                    History.Save(con, string.Format("得意先から着信しました。({0})", telno));

                    foreach (DataRow row in dt.Rows)
                    {
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)row["Tel_CustNo"]);
                        if (tokuisaki != null)
                        {
                            hasTokuisai = true;
                            // 注文入力画面を表示
                            ShowChumonNyuryoku(tokuisaki);
                        }
                    }
                }
                return hasTokuisai;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        #endregion

        #region 新規得意先
        /// <summary>
        /// 新規得意先
        /// </summary>
        /// <param name="telno"></param>
        private void TelConnectNewComer(string telno)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // 電話新規登録確認ダイアログを表示
                Form700N formTokuisakiDlg = new Form700N(telno, DateTime.Now);
                formTokuisakiDlg.MdiParent = this;
                formTokuisakiDlg.StartPosition = FormStartPosition.WindowsDefaultLocation;
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form700N_FormClosed);
                formTokuisakiDlg.Show();
                this.Cursor = Cursors.Default;

                History.Save(string.Format("新規顧客から着信しました。({0})", telno));
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 電話新規登録確認ダイアログクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form700N_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Enabled = true;
                this.Activate();
                Form700N form = (Form700N)sender;
                switch (form.DialogResult)
                {
                    case System.Windows.Forms.DialogResult.Yes: // 受注で使用される顧客番号である
                        this.Cursor = Cursors.WaitCursor;
                        // 得意先選択ダイアログを表示
                        Form900M formTokuisakiDlg = new Form900M(form.TelNo, form.ReceiptTime);
                        formTokuisakiDlg.MdiParent = this;
                        formTokuisakiDlg.Location = new Point(0, 0);
                        formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900MNewComer_FormClosed);
                        formTokuisakiDlg.Show();
                        this.Cursor = Cursors.Default;
                        break;

                    case System.Windows.Forms.DialogResult.No:  // 受注で使用される顧客番号ではない
                        // 非顧客電話番号マスタ登録
                        InsertNotCustomer(form.TelNo);
                        break;

                    default:
                        break;
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 得意先選択ダイアログクローズ（得意先新規登録時）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900MNewComer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.Enabled = true;
                this.Activate();
                Form900M form = (Form900M)sender;
                // 該当する得意先があった？
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Tokuisaki tokuisaki = form.SelectedTokuisaki;

                    if (MessageBox.Show(this, "この電話番号をお客様の連絡先に登録しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        // C_TelToCustに登録
                        AddCustomerTel((int)tokuisaki.得意先ＣＤ, form.TelNo);
                    }

                    // 注文入力画面を表示
                    ShowChumonNyuryoku(tokuisaki);
                }
                else
                {
                    // 該当する得意先が無かった
                    if (MessageBox.Show(this, "新規客情報を登録します。\nよろしいですか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        // 得意先マスタメンテを開く
                        Tokuisaki tokuisaki = new Tokuisaki();
                        tokuisaki.電話番号1 = form.TelNo;
                        ShowTokuisakiToroku(tokuisaki);
                    }
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 非顧客電話番号マスタ登録
        /// </summary>
        /// <param name="telno">電話番号</param>
        private void InsertNotCustomer(string telno)
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // TODO:もし直すようにことがあれば、HiKokyakuTelNoを使うようにする
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n INSERT INTO 非顧客電話番号マスタ (" +
                                      "\r\n     企業ＣＤ," +
                                      "\r\n     電話番号" +
                                      "\r\n     ,非顧客名称" +
                                      "\r\n     ,非顧客カナ" +
                                      "\r\n     ,修正担当者ＣＤ" +
                                      "\r\n     ,修正日" +
                                      "\r\n ) VALUES (" +
                                      "\r\n     @企業ＣＤ," +
                                      "\r\n     @電話番号" +
                                      "\r\n     ,null" +
                                      "\r\n     ,null" +
                                      "\r\n     ,@修正担当者ＣＤ" +
                                      "\r\n     ,getdate()" +
                                      "\r\n )";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = telno;

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    cmd.ExecuteNonQuery();

                    History.Save(con, this.Text, "非顧客電話番号マスタ", telno, historyProcess);
                }
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("非顧客電話番号マスタが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("非顧客電話番号マスタの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(string.Format("非顧客電話番号マスタの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// C_TelToCustに登録
        /// </summary>
        /// <param name="tokisakiCD">得意先ＣＤ</param>
        /// <param name="telno">電話番号</param>
        private void AddCustomerTel(int tokisakiCD, string telno)
        {
            History.ProcessType historyProcess = History.ProcessType.Create;
            bool ret = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    TelToCust telToCust = new TelToCust();
                    telToCust.Tel_TelNo = telno;
                    telToCust.Tel_CustNo = tokisakiCD;
                    telToCust.Tel_RepFlg = true;
                    telToCust.Tel_DelFlg = false;
                    ret = telToCust.Insert(con, tran);

                    if (ret == false)
                    {
                        return;
                    }

                    History.Save(con, tran, this.Text, "電話テーブル", telToCust.Code, historyProcess);

                    // コミット
                    tran.Commit();
                }
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }
        #endregion
    }
}

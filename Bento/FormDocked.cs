using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// ドッキングメニュー画面
    /// </summary>
    public partial class FormDocked : WeifenLuo.WinFormsUI.Docking.DockContent
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
        private Form100 _formHaisoYotei = null;         // 配送予定入力
        private Form101 _formNippaiMochidashi = null;   // 日配持出入力
        private Form200 _formChumon = null;             // 注文入力
        private Form202 _formIkkatsuChumon = null;      // 一括注文入力
        private Form203 _formChumonlist = null;         // 注文一覧
        private Form300 _formKafuzoku = null;           // 過不足一覧
        private Form301 _formIdoNyuryoku = null;        // 移動入力
        private Form302 _formRinjiCourse = null;        // コース臨時組み替え
        private Form400 _formUriage = null;             // 売上入力
        private Form401 _formUriageMeisai = null;       // 売上明細入力
        private Form402 _formUriageList = null;         // 売上一覧
        private Form403 _formUriageSeisan = null;       // 売上精算入力
        private Form404 _formUriageSeisanList = null;   // 売上精算一覧
        private Form500 _formSeikyuShime = null;        // 請求締め処理
        private Form501 _formSeikyuList = null;         // 請求一覧
        private Form503 _formSeikyuPublish = null;      // 請求書発行
        private Form700 _formTokuisaki = null;          // 得意先マスタ一覧
        private Form702 _formSyohin = null;             // 商品マスタ一覧
        private Form703 _formSyuyoBuhin = null;         // 主要部品リストマスタ一覧
        private Form704 _formSyohinGourp = null;        // 商品グループマスタ
        private Form705 _formSyuyoSyohin = null;        // 主要商品リストマスタ一覧
        private Form706 _formTantousya = null;          // 担当者マスタ一覧
        private Form707 _formBusyo = null;              // 部署マスタ一覧
        private Form708 _formCource = null;             // コースマスタ一覧
        private Form709 _formCourceGroup = null;        // コース別グループマスタ一覧
        private Form711 _formBiko = null;               // 備考マスタ一覧
        private Form713 _formShukujitsu = null;         // 祝日マスタメンテ
        private Form714 _formReportPrinter = null;      // 帳票／プリンタ設定
        private Form806 _formSousaHistory = null;       // 操作履歴一覧
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// MDI親フォーム
        /// </summary>
        public MDIParent1 MdiForm { get; set; }

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormDocked()
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
        private void FormDocked_Load(object sender, EventArgs e)
        {
            try
            {
                // 権限によりメニューを制御
                //SetMenuEnable();
            }
            catch (BentoException ex)
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

        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MdiForm.LayoutMdi(MdiLayout.Cascade);
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MdiForm.LayoutMdi(MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MdiForm.LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiForm.MdiChildren)
            {
                childForm.Close();
            }
        }

        private void arrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MdiForm.LayoutMdi(MdiLayout.ArrangeIcons);
        }

        /// <summary>
        /// 配送予定入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem100_Click(object sender, EventArgs e)
        {
            if (_formHaisoYotei == null || _formHaisoYotei.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formHaisoYotei = null;
                _formHaisoYotei = new Form100();
            }
            _formHaisoYotei.MdiParent = MdiForm;
            _formHaisoYotei.Location = new Point(0, 0);
            _formHaisoYotei.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 日配持出入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem101_Click(object sender, EventArgs e)
        {
            if (_formNippaiMochidashi == null || _formNippaiMochidashi.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formNippaiMochidashi = null;
                _formNippaiMochidashi = new Form101();
            }
            _formNippaiMochidashi.MdiParent = MdiForm;
            _formNippaiMochidashi.Location = new Point(0, 0);
            _formNippaiMochidashi.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem200_Click(object sender, EventArgs e)
        {
            if (_formChumon == null || _formChumon.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formChumon = null;
                _formChumon = new Form200(new ChumonCriteria());
            }
            _formChumon.MdiParent = MdiForm;
            _formChumon.Location = new Point(0, 0);
            _formChumon.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// コース別注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem201_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 一括注文入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem202_Click(object sender, EventArgs e)
        {
            if (_formIkkatsuChumon == null || _formIkkatsuChumon.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formIkkatsuChumon = null;
                _formIkkatsuChumon = new Form202();
            }
            _formIkkatsuChumon.MdiParent = MdiForm;
            _formIkkatsuChumon.Location = new Point(0, 0);
            _formIkkatsuChumon.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 注文一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem203_Click(object sender, EventArgs e)
        {
            if (_formChumonlist == null || _formChumonlist.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formChumonlist = null;
                _formChumonlist = new Form203();
            }
            _formChumonlist.MdiParent = MdiForm;
            _formChumonlist.Location = new Point(0, 0);
            _formChumonlist.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 受注発信一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem204_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 過不足一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem300_Click(object sender, EventArgs e)
        {
            if (_formKafuzoku == null || _formKafuzoku.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formKafuzoku = null;
                _formKafuzoku = new Form300();
            }
            _formKafuzoku.MdiParent = MdiForm;
            _formKafuzoku.Location = new Point(0, 0);
            _formKafuzoku.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 移動入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem301_Click(object sender, EventArgs e)
        {
            if (_formIdoNyuryoku == null || _formIdoNyuryoku.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formIdoNyuryoku = null;
                _formIdoNyuryoku = new Form301();
            }
            _formIdoNyuryoku.MdiParent = MdiForm;
            _formIdoNyuryoku.Location = new Point(0, 0);
            _formIdoNyuryoku.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 臨時コース組み換え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem302_Click(object sender, EventArgs e)
        {
            if (_formRinjiCourse == null || _formRinjiCourse.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formRinjiCourse = null;
                _formRinjiCourse = new Form302();
            }
            _formRinjiCourse.MdiParent = MdiForm;
            _formRinjiCourse.Location = new Point(0, 0);
            _formRinjiCourse.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 売上入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem400_Click(object sender, EventArgs e)
        {
            if (_formUriage == null || _formUriage.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formUriage = null;
                _formUriage = new Form400();
            }
            _formUriage.MdiParent = MdiForm;
            _formUriage.Location = new Point(0, 0);
            _formUriage.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 売上明細入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem401_Click(object sender, EventArgs e)
        {
            if (_formUriageMeisai == null || _formUriageMeisai.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formUriageMeisai = null;
                _formUriageMeisai = new Form401(new UriageMeisaiCriteria());
            }
            _formUriageMeisai.MdiParent = MdiForm;
            _formUriageMeisai.Location = new Point(0, 0);
            _formUriageMeisai.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 売上一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem402_Click(object sender, EventArgs e)
        {
            if (_formUriageList == null || _formUriageList.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formUriageList = null;
                _formUriageList = new Form402();
            }
            _formUriageList.MdiParent = MdiForm;
            _formUriageList.Location = new Point(0, 0);
            _formUriageList.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 売上精算入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem403_Click(object sender, EventArgs e)
        {
            if (_formUriageSeisan == null || _formUriageSeisan.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formUriageSeisan = null;
                _formUriageSeisan = new Form403(new UriageSeisanCriteria());
            }
            _formUriageSeisan.MdiParent = MdiForm;
            _formUriageSeisan.Location = new Point(0, 0);
            _formUriageSeisan.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 売上精算一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem404_Click(object sender, EventArgs e)
        {
            if (_formUriageSeisanList == null || _formUriageSeisanList.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formUriageSeisanList = null;
                _formUriageSeisanList = new Form404();
            }
            _formUriageSeisanList.MdiParent = MdiForm;
            _formUriageSeisanList.Location = new Point(0, 0);
            _formUriageSeisanList.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 請求締処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem500_Click(object sender, EventArgs e)
        {
            if (_formSeikyuShime == null || _formSeikyuShime.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSeikyuShime = null;
                _formSeikyuShime = new Form500();
            }
            _formSeikyuShime.MdiParent = MdiForm;
            _formSeikyuShime.Location = new Point(0, 0);
            _formSeikyuShime.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 請求一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem501_Click(object sender, EventArgs e)
        {
            if (_formSeikyuList == null || _formSeikyuList.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSeikyuList = null;
                _formSeikyuList = new Form501();
            }
            _formSeikyuList.MdiParent = MdiForm;
            _formSeikyuList.Location = new Point(0, 0);
            _formSeikyuList.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 請求書発行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem503_Click(object sender, EventArgs e)
        {
            if (_formSeikyuPublish == null || _formSeikyuPublish.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSeikyuPublish = null;
                _formSeikyuPublish = new Form503();
            }
            _formSeikyuPublish.MdiParent = MdiForm;
            _formSeikyuPublish.Location = new Point(0, 0);
            _formSeikyuPublish.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 請求EBデータ出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem504_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 入金一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem600_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 入金入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem601_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 入金EBデータ取込
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem602_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 入金CSVデータ取込
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem603_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// バークレー一覧問合せ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem604_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 未回収金一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem605_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 得意先マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem700_Click(object sender, EventArgs e)
        {
            if (_formTokuisaki == null || _formTokuisaki.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formTokuisaki = null;
                _formTokuisaki = new Form700();
            }
            _formTokuisaki.MdiParent = MdiForm;
            _formTokuisaki.Location = new Point(0, 0);
            _formTokuisaki.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 得意先単価マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem701_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 商品マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem702_Click(object sender, EventArgs e)
        {
            if (_formSyohin == null || _formSyohin.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSyohin = null;
                _formSyohin = new Form702();
            }
            _formSyohin.MdiParent = MdiForm;
            _formSyohin.Location = new Point(0, 0);
            _formSyohin.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 主要部品リストマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem703_Click(object sender, EventArgs e)
        {
            if (_formSyuyoBuhin == null || _formSyuyoBuhin.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSyuyoBuhin = null;
                _formSyuyoBuhin = new Form703();
            }
            _formSyuyoBuhin.MdiParent = MdiForm;
            _formSyuyoBuhin.Location = new Point(0, 0);
            _formSyuyoBuhin.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 商品グループマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem704_Click(object sender, EventArgs e)
        {
            if (_formSyohinGourp == null || _formSyohinGourp.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSyohinGourp = null;
                _formSyohinGourp = new Form704();
            }
            _formSyohinGourp.MdiParent = MdiForm;
            _formSyohinGourp.Location = new Point(0, 0);
            _formSyohinGourp.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 主要商品リストマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem705_Click(object sender, EventArgs e)
        {
            if (_formSyuyoSyohin == null || _formSyuyoSyohin.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSyuyoSyohin = null;
                _formSyuyoSyohin = new Form705();
            }
            _formSyuyoSyohin.MdiParent = MdiForm;
            _formSyuyoSyohin.Location = new Point(0, 0);
            _formSyuyoSyohin.Show();
            MdiForm.Cursor = Cursors.Default;

        }

        /// <summary>
        /// 担当者マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem706_Click(object sender, EventArgs e)
        {
            if (_formTantousya == null || _formTantousya.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formTantousya = null;
                _formTantousya = new Form706();
            }
            _formTantousya.MdiParent = MdiForm;
            _formTantousya.Location = new Point(0, 0);
            _formTantousya.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 部署マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem707_Click(object sender, EventArgs e)
        {
            if (_formBusyo == null || _formBusyo.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formBusyo = null;
                _formBusyo = new Form707();
            }
            _formBusyo.MdiParent = MdiForm;
            _formBusyo.Location = new Point(0, 0);
            _formBusyo.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// コースマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem708_Click(object sender, EventArgs e)
        {
            if (_formCource == null || _formCource.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formCource = null;
                _formCource = new Form708();
            }
            _formCource.MdiParent = MdiForm;
            _formCource.Location = new Point(0, 0);
            _formCource.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// コース別グループマスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem709_Click(object sender, EventArgs e)
        {
            if (_formCourceGroup == null || _formCourceGroup.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formCourceGroup = null;
                _formCourceGroup = new Form709();
            }
            _formCourceGroup.MdiParent = MdiForm;
            _formCourceGroup.Location = new Point(0, 0);
            _formCourceGroup.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 管理マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem710_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 備考マスタ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem711_Click(object sender, EventArgs e)
        {
            if (_formBiko == null || _formBiko.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formBiko = null;
                _formBiko = new Form711();
            }
            _formBiko.MdiParent = MdiForm;
            _formBiko.Location = new Point(0, 0);
            _formBiko.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 稼働日テーブル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem712_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 祝日マスタメンテ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem713_Click(object sender, EventArgs e)
        {
            if (_formShukujitsu == null || _formShukujitsu.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formShukujitsu = null;
                _formShukujitsu = new Form713();
            }
            _formShukujitsu.MdiParent = MdiForm;
            _formShukujitsu.Location = new Point(0, 0);
            _formShukujitsu.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 帳票／プリンタ設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem714_Click(object sender, EventArgs e)
        {
            if (_formReportPrinter == null || _formReportPrinter.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formReportPrinter = null;
                _formReportPrinter = new Form714();
            }
            _formReportPrinter.MdiParent = MdiForm;
            _formReportPrinter.Location = new Point(0, 0);
            _formReportPrinter.Show();
            MdiForm.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 操作履歴一覧
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem806_Click(object sender, EventArgs e)
        {
            if (_formSousaHistory == null || _formSousaHistory.IsDisposed)
            {
                MdiForm.Cursor = Cursors.WaitCursor;
                _formSousaHistory = null;
                _formSousaHistory = new Form806();
            }
            _formSousaHistory.MdiParent = MdiForm;
            _formSousaHistory.Location = new Point(0, 0);
            _formSousaHistory.Show();
            MdiForm.Cursor = Cursors.Default;
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

            SetMenuEnableMain("受注", juchuMenu, dt);
            SetMenuEnableMain("売上", uriageMenu, dt);
            SetMenuEnableMain("入金", nyuukinMenu, dt);
            SetMenuEnableMain("請求", seikyuMenu, dt);
            SetMenuEnableMain("管理", hojoMenu, dt);
            SetMenuEnableMain("マスタメンテ", masterMenu, dt);
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

                    cmd.CommandText = " SELECT " +
                                      "     画面権限No, " +
                                      "     メニュー名, " +
                                      "     画面名, " +
                                      "     閲覧可能フラグ, " +
                                      "     編集可能フラグ " +
                                      " FROM " +
                                      "     M_画面権限 " +
                                      " WHERE " +
                                      "     権限CD = @権限CD ";
                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@権限CD", SqlDbType.Int).Value = userData.権限区分;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adp.Fill(dt);
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
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
                if(item is ToolStripMenuItem)
                {
                    ToolStripMenuItem menuChild = (ToolStripMenuItem)item;
                    string menuText = Regex.Replace(menuChild.Text, "\\(&.\\)", "");
                    DataRow[] drs = dt.Select(string.Format("メニュー名 = '{0}' and 画面名 = '{1}'", menuTitle, menuText));
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
        #endregion
    }
}


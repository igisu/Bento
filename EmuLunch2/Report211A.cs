using System;
using System.Data;
using System.Linq;
using Emu.Bento.Common;

namespace Emu.Bento
{
    /// <summary>
    /// 配送明細表
    /// </summary>
    public partial class Report211A : BaseReport
    {
        private const string TITLE = "配送明細表";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4縦)"; } }

        /// <summary>
        /// 【合計】行を示す得意先CD
        /// </summary>
        public const int TOTAL_TOKUISAKI_CD = int.MaxValue - 2;

        /// <summary>
        /// 【内訳】行を示す得意先CD
        /// </summary>
        public const int SUMMARY_TOKUISAKI_CD = int.MaxValue - 1;

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// サブレポート副食
        /// </summary>
        private Report211ASubSyohin _report211SubSub;

        /// <summary>
        /// サブレポート主食
        /// </summary>
        private Report211ASubSyohin _report211SubMain;

        /// <summary>
        /// サブレポート備考
        /// </summary>
        private Report211ASubBiko _report211SubBiko;

        /// <summary>
        /// 得意先CD
        /// </summary>
        private int _tokuisakiCD;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        ///
        /// </summary>
        public Report211A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// サブレポート明細(副食/主食)用データテーブル
        /// </summary>
        public DataTable DetailDataSource { get; set; }

        /// <summary>
        /// サブレポート備考用データテーブル
        /// </summary>
        public DataTable BikoDataSource { get; set; }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// レポートが処理を開始する前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Report211A_ReportStart(object sender, EventArgs e)
        {
            _report211SubSub = new Report211ASubSyohin();
            _report211SubMain = new Report211ASubSyohin();
            _report211SubBiko = new Report211ASubBiko();
        }

        /// <summary>
        /// 新しいレコードが処理されるごとに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eArgs"></param>
        private void Report211A_FetchData(object sender, FetchEventArgs eArgs)
        {
            _tokuisakiCD = (int)Fields["得意先ＣＤ"].Value;
        }

        /// <summary>
        /// フォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_Format(object sender, EventArgs e)
        {
            // 得意先に該当する副食レコード群を明細データソースから取得
            var subList = DetailDataSource.Rows.Cast<DataRow>()
                                            .Where(row => row["得意先ＣＤ"] as int? == _tokuisakiCD && row["弁当区分"] as int? == (int)Consts.BentoKubun.SubDish)
                                            .ToArray();     // ToList()だとデータバインドがうまくいかない

            // 得意先に該当する主食レコード群を明細データソースから取得
            var mainList = DetailDataSource.Rows.Cast<DataRow>()
                                            .Where(row => row["得意先ＣＤ"] as int? == _tokuisakiCD && row["弁当区分"] as int? == (int)Consts.BentoKubun.MainDish)
                                            .ToArray();

            // 得意先に該当する備考レコード群を備考データソースから取得
            var bikoList = BikoDataSource.Rows.Cast<DataRow>()
                                            .Where(row => row["得意先ＣＤ"] as int? == _tokuisakiCD)
                                            .ToArray();

            // 取得したレコード群をサブレポートのデータソースとする
            _report211SubSub.DataSource = subList;
            _report211SubMain.DataSource = mainList;
            _report211SubBiko.DataSource = bikoList;

            // サブレポートのインスタンスを設定
            subReportSub.Report = _report211SubSub;
            subReportMain.Report = _report211SubMain;
            subReportBiko.Report = _report211SubBiko;

            // 【合計】行の場合はその罫線を印字
            lineDetailHeader.Visible = (_tokuisakiCD == TOTAL_TOKUISAKI_CD);
        }

        /// <summary>
        /// 詳細セクション描画前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_BeforePrint(object sender, EventArgs e)
        {
            float height = lineDetailBottom.Y1; // デフォルトの高さ

            // lineDetailBottomを各サブレポートのいずれか高い方に合わせる
            float h = Math.Max(subReportSub.Height, Math.Max(subReportMain.Height, subReportBiko.Height));
            if (h > 0)  // 印字するレコードがサブレポートに無い場合は高さが0になる
            {
                lineDetailBottom.Y1 = h;
                lineDetailBottom.Y2 = h;
            }

            // 2行以上の高さになったら、得意先名を改行して印字する
            if (height < h)
            {
                txtTokuisakiName.Height = h - txtTokuisakiName.Top;
                txtTokuisakiName.WrapMode = GrapeCity.ActiveReports.Document.Section.WrapMode.CharWrap;
            }
            else
            {
                txtTokuisakiName.WrapMode = GrapeCity.ActiveReports.Document.Section.WrapMode.NoWrap;
            }
        }
        #endregion
    }
}

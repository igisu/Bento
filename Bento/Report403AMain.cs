using System;
using System.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 売上明細表
    /// </summary>
    public partial class Report403AMain : GrapeCity.ActiveReports.SectionReport
    {

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 持出サブレポート
        /// </summary>
        private Report403ASubMochidashi _report403ASubMochidashi;

        /// <summary>
        /// 現金サブレポート
        /// </summary>
        private Report403ASubMeisaiGenkin _report403ASubMeisaiGenkin;

        /// <summary>
        /// 掛売サブレポート
        /// </summary>
        private Report403ASubMeisaiKakeuri _report403ASubMeisaiKakeuri;

        /// <summary>
        /// 電子決済サブレポート
        /// </summary>
        private Report403ASubMeisaiPay _report403ASubMeisaiPay;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Report403AMain()
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
        /// 持出サブレポート用データテーブル
        /// </summary>
        public DataTable DtMochidashi { get; set; }

        /// <summary>
        /// 現金サブレポート用データテーブル
        /// </summary>
        public DataTable DtGenkin { get; set; }

        /// <summary>
        /// 掛売サブレポート用データテーブル
        /// </summary>
        public DataTable DtKakeuri { get; set; }

        /// <summary>
        /// 電子決済サブレポート用データテーブル
        /// </summary>
        public DataTable DtPay { get; set; }
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
        private void Report403AMain_ReportStart(object sender, EventArgs e)
        {
            _report403ASubMochidashi = new Report403ASubMochidashi();
            _report403ASubMeisaiGenkin = new Report403ASubMeisaiGenkin();
            _report403ASubMeisaiKakeuri = new Report403ASubMeisaiKakeuri();
            _report403ASubMeisaiPay = new Report403ASubMeisaiPay();
        }

        /// <summary>
        /// フォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_Format(object sender, EventArgs e)
        {
            // 掛売_未集金を編集
            txtKakeuriMisyukin.Text = string.Format("({0,5:N0})", txtKakeuriMisyukin.Value);

            // サブレポートのデータソースをセット
            _report403ASubMochidashi.DataSource = DtMochidashi;
            _report403ASubMeisaiGenkin.DataSource = DtGenkin;
            _report403ASubMeisaiKakeuri.DataSource = DtKakeuri;
            _report403ASubMeisaiPay.DataSource = DtPay;

            // サブレポートのインスタンスを設定
            subReportMochidashi.Report = _report403ASubMochidashi;
            subReportMeisaiGenkin.Report = _report403ASubMeisaiGenkin;
            subReportMeisaiKakeuri.Report = _report403ASubMeisaiKakeuri;
            subReportMeisaiPay.Report = _report403ASubMeisaiPay;
        }
        #endregion
    }
}

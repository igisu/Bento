using System;
using System.Drawing;
using GrapeCity.ActiveReports.SectionReportModel;

namespace Emu.Bento
{
    /// <summary>
    /// 領収書
    /// </summary>
    public partial class Report300A : BaseReport
    {
        private const string TITLE = "領収書";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(B5縦)"; } }

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// グループヘッダーを表示するかどうか
        /// </summary>
        private bool _isHeaderVisible;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public float TopOffsetCM { get; set; }
        public float LeftOffsetCM { get; set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report300A()
        {
            //
            // デザイナー サポートに必要なメソッドです。
            //
            InitializeComponent();
            _isHeaderVisible = false;   // 最初のレコード(=1レコード目)はグループヘッダーを表示しない。以降は交互に切り替える
        }

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
        private void Report300A_ReportStart(object sender, EventArgs e)
        {
            // 印鑑画像フィールドの位置を補正する
            AdjustPosition(picInkan1);
            AdjustPosition(picInkan2);
        }

        /// <summary>
        /// グループヘッダー フォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupHeader1_Format(object sender, EventArgs e)
        {
            groupHeader1.Visible = _isHeaderVisible;
            _isHeaderVisible = !(_isHeaderVisible);
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// コントロールの位置を補正する
        /// </summary>
        /// <param name="ctrl">対象のActiveReportsコントロール</param>
        private void AdjustPosition(ARControl ctrl)
        {
            PointF p = ctrl.Location;
            p.X += CmToInch(LeftOffsetCM);
            p.Y += CmToInch(TopOffsetCM);
            ctrl.Location = p;
        }
        #endregion
    }
}

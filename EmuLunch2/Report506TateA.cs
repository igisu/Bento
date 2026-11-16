using System;
using System.Drawing;
using GrapeCity.ActiveReports.SectionReportModel;

namespace Emu.Bento
{
    /// <summary>
    /// 合算請求書(縦)
    /// </summary>
    public partial class Report506TateA : GrapeCity.ActiveReports.SectionReport
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool? _請求書郵便マークフラグ;
        private bool? _回収予定日印字フラグ;
        private bool? _振込手数料フラグ;
        private bool? _請求書ロゴ出力;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report506TateA()
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
        public float TopOffsetCM { get; set; }
        public float LeftOffsetCM { get; set; }
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
        private void Report506TateA_ReportStart(object sender, EventArgs e)
        {
            // 宛先フィールドの位置を補正する
            AdjustPosition(lblZipBox);
            AdjustPosition(lblZip);
            AdjustPosition(txtTokuisakiZip);

            AdjustPosition(txtTokuisakiAddress1);
            AdjustPosition(txtTokuisakiAddress2);

            AdjustPosition(lineName1);
            AdjustPosition(txtTokuisakiName);
            AdjustPosition(lineName2);

            AdjustPosition(lblTokuisakiTel);
            AdjustPosition(txtTokuisakiTel);
            AdjustPosition(lblTokuisakiFax);
            AdjustPosition(txtTokuisakiFax);
        }

        /// <summary>
        /// 新しいレコードが処理されるごとに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eArgs"></param>
        private void Report506TateA_FetchData(object sender, FetchEventArgs eArgs)
        {
            _請求書郵便マークフラグ = Fields["請求書郵便マークフラグ"].Value as bool?;
            _回収予定日印字フラグ = Fields["回収予定日印字フラグ"].Value as bool?;
            _振込手数料フラグ = Fields["振込手数料フラグ"].Value as bool?;
            _請求書ロゴ出力 = ((Fields["ロゴ画像"].Value as byte[]) != null);
        }

        /// <summary>
        /// レポートヘッダーフォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportHeader1_Format(object sender, EventArgs e)
        {
            lblZipBox.Visible = _請求書郵便マークフラグ ?? false;

            shpKaisyu.Visible = _回収予定日印字フラグ ?? false;
            lineKaisyu.Visible = _回収予定日印字フラグ ?? false;
            lblLimit.Visible = _回収予定日印字フラグ ?? false;
            txtKaisyuYotei.Visible = _回収予定日印字フラグ ?? false;

            lblTesuryo.Visible = !(_振込手数料フラグ ?? true);

            txtJishaName.Visible = !(_請求書ロゴ出力 ?? true);  // 請求書ロゴを出力する場合は、自社名称・郵便番号は非表示
            //lblJishaZip.Visible = !(_請求書ロゴ出力 ?? true);
            //txtJishaZip.Visible = !(_請求書ロゴ出力 ?? true);
            //picInkan.Visible = !(_請求書ロゴ出力 ?? true);      // 請求書ロゴを出力する場合は、印鑑画像は非表示
            //picLogo.Visible = (_請求書ロゴ出力 ?? false);
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

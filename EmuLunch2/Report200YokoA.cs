using System;
using System.Drawing;
using Emu.Bento.Common;
using Emu.Bento.Data;
using GrapeCity.ActiveReports.SectionReportModel;

namespace Emu.Bento
{
    /// <summary>
    /// 請求書(カレンダー)
    /// </summary>
    public partial class Report200YokoA : GrapeCity.ActiveReports.SectionReport
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool? _請求書郵便マークフラグ;
        private bool? _回収予定日印字フラグ;
        private bool? _合算請求フラグ;
        private bool? _振込手数料フラグ;
        private bool? _請求書ロゴ出力;
        private int _税区分;
        private bool? _内消費税見出し;
        private int? _支払方法;
        private bool? _振替口座印字フラグ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Report200YokoA()
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
        private void Report200YokoA_ReportStart(object sender, EventArgs e)
        {
            // 宛先フィールドの位置を補正する
            AdjustPosition(lblCodeNo);
            AdjustPosition(txtTokuisakiCD);
            AdjustPosition(lblHyphen1);
            AdjustPosition(txtCourseCD);
            AdjustPosition(lblKakko1);
            AdjustPosition(txtShimebi);
            AdjustPosition(lblHyphen2);
            AdjustPosition(txtSite);
            AdjustPosition(lblHyphen3);
            AdjustPosition(txtShiharaibi);
            AdjustPosition(lblHyphen4);
            AdjustPosition(txtShiharaiHoho);
            AdjustPosition(lblKakko2);

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
        private void Report200YokoA_FetchData(object sender, FetchEventArgs eArgs)
        {
            _請求書郵便マークフラグ = Fields["請求書郵便マークフラグ"].Value as bool?;
            _回収予定日印字フラグ = Fields["回収予定日印字フラグ"].Value as bool?;
            _合算請求フラグ = Fields["合算請求フラグ"].Value as bool?;
            _振込手数料フラグ = Fields["振込手数料フラグ"].Value as bool?;
            _請求書ロゴ出力 = ((Fields["ロゴ画像"].Value as byte[]) != null);
            _税区分 = (int)Fields["税区分"].Value;
            _内消費税見出し = Fields["内消費税見出し"].Value as bool?;
            _支払方法 = Fields["支払方法"].Value as int?;
            _振替口座印字フラグ = Fields["振替口座印字フラグ"].Value as bool?;
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

            lblUchiwake.Visible = _合算請求フラグ ?? false;

            lblTesuryo.Visible = !(_振込手数料フラグ ?? true);

            txtJishaName.Visible = !(_請求書ロゴ出力 ?? true);  // 請求書ロゴを出力する場合は、自社名称・郵便番号は非表示
            //lblJishaZip.Visible = !(_請求書ロゴ出力 ?? true);
            //txtJishaZip.Visible = !(_請求書ロゴ出力 ?? true);
            //picInkan.Visible = !(_請求書ロゴ出力 ?? true);      // 請求書ロゴを出力する場合は、印鑑画像は非表示
            //picLogo.Visible = (_請求書ロゴ出力 ?? false);

            // 内税の場合は、消費税の見出しと書式を変える
            if (_税区分 == (int)Syohizei.ZeiKubun.Uchi)
            {
                if (_内消費税見出し == true)
                {
                    lblZei.Text = "内消費税";
                    txtZei.OutputFormat = "#,##0";
                }
                else
                {
                    lblZei.Text = "消費税";
                    txtZei.OutputFormat = "(#,##0)";
                }
            }
            else
            {
                lblZei.Text = "消費税";
                txtZei.OutputFormat = "#,##0";
            }
            txtZei.Value = txtZei.Value;        // OutputFormatの再適用を促す

            // 支払方法が「4:振替」 かつ 請求書発行で「振替口座を印字する」チェックボックスがONの場合は、振替口座を印字する
            if (_支払方法 == (int)Consts.ShiharaiHohoKubun.Furikae && _振替口座印字フラグ == true)
            {
                shpFurikaeKoza.Visible = true;
                lineFurikaeKoza.Visible = true;
                lblFurikaeKoza.Visible = true;
                txtFurikaeKoza.Visible = true;
            }
            else
            {
                shpFurikaeKoza.Visible = false;
                lineFurikaeKoza.Visible = false;
                lblFurikaeKoza.Visible = false;
                txtFurikaeKoza.Visible = false;
            }
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

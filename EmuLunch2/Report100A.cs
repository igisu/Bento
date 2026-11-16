using System;
using System.Data;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 納品書
    /// </summary>
    public partial class Report100A : BaseReport
    {
        private const string TITLE = "納品書";

        public override string Title { get{ return TITLE; } }
        public override string TitleWithSize { get{ return TITLE + "(A4縦)"; } }

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// サブレポート(控え)
        /// </summary>
        private Report100ASub _report100ASub1;

        /// <summary>
        /// サブレポート(正)
        /// </summary>
        private Report100ASub _report100ASub2;

        /// <summary>
        /// ページ番号（サブレポートのデータソースとのリンク用）
        /// </summary>
        int _pageNo;

        /// <summary>
        /// 納品書伝票番号（サブレポートのデータソースとのリンク用）
        /// </summary>
        int _denpyoNo;

        private bool? _納品書ロゴ出力;
        private int _税区分;
        private int _売掛現金区分;
        private bool? _金額印字フラグ;
        private bool? _内消費税見出し;
        private bool? _納品書兼請求書フラグ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Report100A()
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
        /// サブレポート用データテーブル
        /// </summary>
        public DataTable DetailPageDt { get; set; }
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
        private void Report100A_ReportStart(object sender, EventArgs e)
        {
            _report100ASub1 = new Report100ASub();
            _report100ASub2 = new Report100ASub();
        }

        /// <summary>
        /// 新しいレコードが処理されるごとに発生します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eArgs"></param>
        private void Report100A_FetchData(object sender, FetchEventArgs eArgs)
        {
            //***********************************************************************************************************
            // 親データソースと子データソースをここでリンクする
            // 具体的には、親が持つ項目(キー)に該当するレコード群を子から取得して、それをサブレポートのデータソースとする
            //***********************************************************************************************************
            // 親が持つリンク項目(キー)を取得
            _pageNo = (int)Fields["ページ番号"].Value;
            _denpyoNo = (int)Fields["納品書伝票番号"].Value;

            _納品書ロゴ出力 = ((Fields["ロゴ画像"].Value as byte[]) != null);
            _税区分 = (int)Fields["税区分"].Value;
            _売掛現金区分 = (int)Fields["売掛現金区分"].Value;
            _金額印字フラグ = Fields["金額印字フラグ"].Value as bool?;
            _内消費税見出し = Fields["内消費税見出し"].Value as bool?;
            _納品書兼請求書フラグ = Fields["納品書兼請求書フラグ"].Value as bool?;
        }

        /// <summary>
        /// フォーマット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detail_Format(object sender, EventArgs e)
        {
            // 納品書ロゴを出力する場合は、自社名称は非表示
            txtJishaName1.Visible = !(_納品書ロゴ出力 ?? true);
            txtJishaName2.Visible = !(_納品書ロゴ出力 ?? true);

            picLogo1.Visible = (_納品書ロゴ出力 ?? false);
            picLogo2.Visible = (_納品書ロゴ出力 ?? false);

            // 内税の場合は、消費税の見出しと書式を変える
            if (_税区分 == (int)Syohizei.ZeiKubun.Uchi)
            {
                if (_内消費税見出し == true)
                {
                    lblZei1.Text = "内消費税";
                    lblZei2.Text = "内消費税";
                    txtZei1.OutputFormat = "#,##0";
                    txtZei2.OutputFormat = "#,##0";
                }
                else
                {
                    lblZei1.Text = "消費税";
                    lblZei2.Text = "消費税";
                    txtZei1.OutputFormat = "(#,##0)";
                    txtZei2.OutputFormat = "(#,##0)";
                }
            }
            else
            {
                lblZei1.Text = "消費税";
                lblZei2.Text = "消費税";
                txtZei1.OutputFormat = "#,##0";
                txtZei2.OutputFormat = "#,##0";
            }
            txtZei1.Value = txtZei1.Value;          // OutputFormatの再適用を促す
            txtZei2.Value = txtZei2.Value;

            // 売掛の場合は、消費税と合計金額は出力しない
            // 代わりに税区分備考を出力する
            if (_売掛現金区分 == (int)Consts.UriGenKubun.Urikake)
            {
                // ただし「納品書兼請求書」として出力する場合は、消費税と合計金額は出力する
                // 納品書兼請求書フラグがONの場合は必ず金額印字フラグもONになっている（画面で制御している）
                txtZei1.Visible = (_納品書兼請求書フラグ ?? false);
                txtZei2.Visible = (_納品書兼請求書フラグ ?? false);
                txtTotal1.Visible = (_納品書兼請求書フラグ ?? false);
                txtTotal2.Visible = (_納品書兼請求書フラグ ?? false);
                txtBikoZeikbn1.Visible = (_金額印字フラグ ?? false);
                txtBikoZeikbn2.Visible = (_金額印字フラグ ?? false);
            }
            else
            {
                // 現金の場合は常に消費税と合計金額を出力する
                // ただし税区分備考は出力しない
                txtZei1.Visible = true;
                txtZei2.Visible = true;
                txtTotal1.Visible = true;
                txtTotal2.Visible = true;
                txtBikoZeikbn1.Visible = false;
                txtBikoZeikbn2.Visible = false;
            }

            txtTaxDetail1.Visible = (_金額印字フラグ ?? false);
            txtTaxDetail2.Visible = (_金額印字フラグ ?? false);

            // キーに該当するレコード群を子データソースから取得
            DataRow[] drs = DetailPageDt.Select(string.Format("ページ番号={0} and 納品書伝票番号={1}", _pageNo, _denpyoNo), "明細ソート用番号");

            // 取得したレコード群をサブレポートのデータソースとする
            _report100ASub1.DataSource = drs;
            _report100ASub2.DataSource = drs;

            // サブレポートのインスタンスを設定
            subReport1.Report = _report100ASub1;
            subReport2.Report = _report100ASub2;
        }
        #endregion
    }
}

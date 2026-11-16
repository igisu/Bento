using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 領収書印刷ダイアログ
    /// </summary>
    public partial class Form501P : BaseForm
    {
        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// 軽減税率算出用
        /// </summary>
        private class KeigenZeiritsu
        {
            //**************************************
            //* 列挙値
            //**************************************
            #region 列挙値
            /// <summary>
            /// 税率種類
            /// </summary>
            private enum TaxKind
            {
                /// <summary>
                /// 適用前
                /// </summary>
                Unapplied,

                /// <summary>
                /// 通常税率
                /// </summary>
                Normal,

                /// <summary>
                /// 軽減税率
                /// </summary>
                Reduced,
            }

            #endregion

            //**************************************
            //* インスタンス変数
            //**************************************
            #region インスタンス変数
            /// <summary>
            /// 領収書に税率ごとの内訳を出力するかどうか
            /// </summary>
            private bool _isPrintRateDetailForReceipt;

            /// <summary>
            /// 税率ごとの合計金額を保持するマップ
            /// </summary>
            private SortedList<decimal, decimal> _kingakuMap;

            /// <summary>
            /// 適用前／通常／軽減ごとの明細件数を保持するマップ
            /// </summary>
            private Dictionary<TaxKind, int> _countMap;

            /// <summary>
            /// 軽減税率が適用済みかどうか
            /// </summary>
            private bool _hasKeigen;

            /// <summary>
            /// 通常税率
            /// </summary>
            private decimal? _normalTaxRate;

            /// <summary>
            /// 軽減税率
            /// </summary>
            private decimal? _reducedTaxRate;

            /// <summary>
            /// 内訳
            /// </summary>
            private string _taxDetail;

            /// <summary>
            /// 但書
            /// </summary>
            private string _proviso;
            #endregion

            //**************************************
            //* コンストラクタ
            //**************************************
            #region コンストラクタ
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="isPrintRateDetailForReceipt">領収書に税率ごとの内訳を出力する場合はtrue。出力しない場合はfalse。</param>
            public KeigenZeiritsu(bool isPrintRateDetailForReceipt)
            {
                _isPrintRateDetailForReceipt = isPrintRateDetailForReceipt;

                _kingakuMap = new SortedList<decimal, decimal>();
                _countMap = new Dictionary<TaxKind, int>();
                _hasKeigen = false;
                _normalTaxRate = null;
                _reducedTaxRate = null;
                _taxDetail = null;
                _proviso = null;
            }
            #endregion

            //**************************************
            //* プロパティ
            //**************************************
            #region プロパティ
            /// <summary>
            /// 税率ごとの合計金額を保持するマップ
            /// </summary>
            public SortedList<decimal, decimal> KingakuMap
            {
                get
                {
                    return _kingakuMap;
                }
            }

            /// <summary>
            /// 内訳
            /// </summary>
            public string TaxDetail
            {
                get
                {
                    if (_taxDetail == null)
                    {
                        GenerateProvisoAndDetail();
                    }
                    return _taxDetail;
                }
            }

            /// <summary>
            /// 但書
            /// </summary>
            public string Proviso
            {
                get
                {
                    if (_proviso == null)
                    {
                        GenerateProvisoAndDetail();
                    }
                    return _proviso;
                }
            }

            /// <summary>
            /// 軽減税率
            /// </summary>
            public decimal? ReducedTaxRate
            {
                get
                {
                    return _reducedTaxRate;
                }
            }

            /// <summary>
            /// 通常税率
            /// </summary>
            public decimal? NormalTaxRate
            {
                get
                {
                    return _normalTaxRate;
                }
            }
            #endregion

            //**************************************
            //* メソッド
            //**************************************
            #region メソッド
            /// <summary>
            /// 税率ごとの合計金額を保持するマップを初期化します
            /// </summary>
            /// <param name="syohizei">消費税率マスタ</param>
            public void InitKingaku(Syohizei syohizei)
            {
                if (_kingakuMap.ContainsKey((decimal)syohizei.消費税率) == false)
                {
                    _kingakuMap[(decimal)syohizei.消費税率] = 0m;
                }

                if (syohizei.HasKeigen())
                {
                    _hasKeigen = true;
                    _reducedTaxRate = syohizei.軽減税率;            // 軽減税率を取得
                    if (_kingakuMap.ContainsKey((decimal)syohizei.軽減税率) == false)
                    {
                        _kingakuMap[(decimal)syohizei.軽減税率] = 0m;
                    }
                }
            }

            /// <summary>
            /// 税率ごとの合計金額を加算する
            /// </summary>
            /// <param name="taxRate">消費税率</param>
            /// <param name="kingaku">金額</param>
            public void AddKingaku(decimal taxRate, decimal kingaku)
            {
                if (_kingakuMap.ContainsKey(taxRate) == false)
                {
                    _kingakuMap[taxRate] = 0m;
                }
                _kingakuMap[taxRate] += kingaku;
            }

            /// <summary>
            /// 適用前／通常／軽減ごとの明細件数をカウントする
            /// </summary>
            /// <param name="syohizei">消費税率マスタ</param>
            /// <param name="taxRate">消費税率</param>
            public void CountKeigen(Syohizei syohizei, decimal taxRate)
            {
                TaxKind taxKind = TaxKind.Normal;   // 通常税率

                // 適用前？
                if (syohizei.軽減税率 == null)
                {
                    taxKind = TaxKind.Unapplied;
                }
                else
                {
                    // 軽減税率？
                    if (taxRate == syohizei.軽減税率)
                    {
                        taxKind = TaxKind.Reduced;
                    }
                    _normalTaxRate = syohizei.消費税率;     // (軽減税率に対する)通常税率を取得
                }

                if (_countMap.ContainsKey(taxKind) == false)
                {
                    _countMap[taxKind] = 0;
                }
                _countMap[taxKind]++;
            }

            /// <summary>
            /// 内訳と但書を生成します
            /// </summary>
            private void GenerateProvisoAndDetail()
            {
                _taxDetail = "";
                _proviso = "";
                // ┌─────────────────┬─────────────────────┬──────────────┐
                // │                                  │但書                                      │内訳                        │
                // ├─────────────────┼─────────────────────┼──────────────┤
                // │通常税率と軽減税率が混在する場合  │お弁当(軽減対象)、お弁当                  │常に出力                    │
                // ├─────────────────┼─────────────────────┼──────────────┤
                // │通常税率と軽減税率が混在しない場合│全商品が軽減対象 または 全商品が軽減対象外│システムマスタのフラグで制御│
                // └─────────────────┴─────────────────────┴──────────────┘
                // ┌─┬───┬──┬──┬────────────┬──────────────┐
                // │No│適用前│通常│軽減│但書                    │内訳                        │
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 1│      │    │ ○ │全商品が軽減対象        │システムマスタのフラグで制御│
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 2│      │ ○ │    │全商品が軽減対象外      │システムマスタのフラグで制御│
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 3│      │ ○ │ ○ │お弁当(軽減対象)、お弁当│常に出力                    │
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 4│  ○  │    │    │                        │                            │
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 5│  ○  │    │ ○ │全商品が軽減対象        │システムマスタのフラグで制御│
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 6│  ○  │ ○ │    │お弁当(軽減対象)、お弁当│常に出力                    │
                // ├─┼───┼──┼──┼────────────┼──────────────┤
                // │ 7│  ○  │ ○ │ ○ │お弁当(軽減対象)、お弁当│常に出力                    │
                // └─┴───┴──┴──┴────────────┴──────────────┘

                // 軽減税率が始まっている(No4以外)？
                if (_hasKeigen)
                {
                    StringBuilder detail = new StringBuilder();
                    for (int i = _kingakuMap.Count - 1; i >= 0; i--)   // 税率の降順
                    {
                        detail.AppendFormat("{0,2:P0}対象 {1,10:N0}円", _kingakuMap.Keys[i], _kingakuMap.Values[i]);     // 10%対象 1,000円
                        detail.AppendLine();
                    }
                    _taxDetail = detail.ToString();
                    _proviso = "お弁当(軽減対象)、お弁当";

                    // 軽減のみ(No1)？
                    if (_countMap.ContainsKey(TaxKind.Unapplied) == false &&
                        _countMap.ContainsKey(TaxKind.Normal) == false &&
                        _countMap.ContainsKey(TaxKind.Reduced) == true)
                    {
                        _proviso = "全商品が軽減対象";
                        if (_isPrintRateDetailForReceipt == false)
                        {
                            _taxDetail = "";
                        }
                    }

                    // 通常のみ(No2)？
                    if (_countMap.ContainsKey(TaxKind.Unapplied) == false &&
                        _countMap.ContainsKey(TaxKind.Normal) == true &&
                        _countMap.ContainsKey(TaxKind.Reduced) == false)
                    {
                        _proviso = "全商品が軽減対象外";
                        if (_isPrintRateDetailForReceipt == false)
                        {
                            _taxDetail = "";
                        }
                    }

                    // 8%のみ(No5)？
                    if (_countMap.ContainsKey(TaxKind.Unapplied) == true &&
                        _countMap.ContainsKey(TaxKind.Normal) == false &&
                        _countMap.ContainsKey(TaxKind.Reduced) == true)
                    {
                        _proviso = "全商品が軽減対象";
                        if (_isPrintRateDetailForReceipt == false)
                        {
                            _taxDetail = "";
                        }
                    }
                }
            }

            /// <summary>
            /// 税率ごとの合計金額に含まれる内税額を返します
            /// </summary>
            /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
            /// <returns>税率ごとの合計金額に含まれる内税額の合計</returns>
            public decimal GetTaxIncluded(int? hasuKubun)
            {
                decimal totalTax = 0m;
                foreach (decimal rate in _kingakuMap.Keys)
                {
                    // 金額に含まれる内税額を算出
                    totalTax += Syohizei.GetTaxIncluded(Syohizei.ZeiKubun.Uchi, _kingakuMap[rate], rate, hasuKubun);
                }
                return totalTax;
            }

            /// <summary>
            /// 税率ごとの合計金額の外税額を返します
            /// </summary>
            /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
            /// <returns>税率ごとの合計金額の外税額の合計</returns>
            public decimal GetTaxExcluded(int? hasuKubun)
            {
                decimal totalTax = 0m;
                foreach (decimal rate in _kingakuMap.Keys)
                {
                    // 金額の外税額を算出
                    totalTax += Syohizei.GetTax(Syohizei.ZeiKubun.Soto, _kingakuMap[rate], rate, hasuKubun);
                }
                return totalTax;
            }
            #endregion
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求一覧検索条件（領収書印刷条件）
        /// </summary>
        private SeikyuListCriteria _seikyuListCriteria;

        /// <summary>
        /// 領収書印刷条件
        /// </summary>
        private ReceiptPrint _receiptPrint;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="seikyuListCriteria">領収書印刷条件</param>
        public Form501P(SeikyuListCriteria seikyuListCriteria)
        {
            InitializeComponent();

            _seikyuListCriteria = seikyuListCriteria;
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
        private void Form501P_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _receiptPrint = new ReceiptPrint();
                _receiptPrint.発行日 = DateTime.Today;
                receiptPrintBindingSource.DataSource = _receiptPrint;
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
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form501P_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:                      // 処理開始ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                    btnPrint.PerformClick();
                    break;

                // 終了はフォームのCancelButtonなのでEscキーとしていたが、
                // (ボタンのクリックではなく)キー押下でフォームを閉じたときに親フォームがアクティブにならないという問題がある。
                // これを回避するには当該キー押下時に当該ボタンにフォーカスすればよいのだが、
                // CancelButtonだとイベントが発生せずEscキー押下を検知できず、ボタンにフォーカスする契機が無い。
                case Keys.Escape:
                    btnQuit.Focus();
                    btnQuit.PerformClick();
                    break;
            }
        }

        /// <summary>
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件のエラーは呼び出し元でチェック済み

                PrintRyoshusho();           // 領収書印刷
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
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 領収書印刷処理
        /// </summary>
        private void PrintRyoshusho()
        {
            try
            {
                Report300A report = new Report300A();       // 領収書
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // システム設定マスタから印鑑画像のオフセット量を取得
                float topOffset = SystemMaster.GetReceiptStampTopOffset();      // 印鑑画像の上下方向オフセット量(cm)
                float leftOffset = SystemMaster.GetReceiptStampLeftOffset();    // 印鑑画像の左右方向オフセット量(cm)

                bool isPrintRateDetailForReceipt = SystemMaster.IsPrintRateDetailForReceipt();    // 領収書に税率ごとの内訳を出力するかどうか

                // 内消費税額と、税率ごとの内訳取得用の売上明細を取得
                DataTable dtMeisai = GetUriageMeisai();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    if (_seikyuListCriteria.出力件数 < 0)
                    {
                        sql.AppendLine(" SELECT");
                    }
                    else
                    {
                        sql.AppendFormat(" SELECT TOP({0})", _seikyuListCriteria.出力件数).AppendLine();
                    }
                    sql.AppendLine("     A.請求番号");
                    sql.AppendLine("     ,A.請求先ＣＤ AS 得意先ＣＤ");
                    sql.AppendLine("     ,C.コースＣＤ");
                    sql.AppendLine("     ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                    sql.AppendLine("     ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様')) AS 得意先長さ");
                    sql.AppendLine("     ,S.電話番号1 AS 得意先電話番号");
                    sql.AppendLine("     ,ISNULL(S.税区分, @税区分内税) AS 税区分");
                    sql.AppendLine("     ,ISNULL(S.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");      // 初期値は2:切り捨て
                    sql.AppendLine("     ,CASE WHEN ISNULL(A.消費税, 0) = 0 THEN ");
                    sql.AppendLine("           CASE WHEN ISNULL(A.買上金額, 0) = 0 THEN 消費税 ELSE NULL END"); // 内税と思われる場合はCalcTax()で消費税額を算出する
                    sql.AppendLine("          ELSE A.消費税");
                    sql.AppendLine("          END AS 消費税");
                    if (rdoUriage.Checked)
                    {
                        // 売上額を出力する場合、外税の場合は買上金額に消費税額を加算したものを出力する（内税の場合は消費税は加算せず、買上金額をそのまま出力する）
                        sql.AppendLine("     ,CASE WHEN ISNULL(A.消費税, 0) = 0 THEN");
                        sql.AppendLine("           CASE WHEN ISNULL(A.買上金額, 0) = 0 THEN ISNULL(A.買上金額, 0) + ISNULL(A.消費税, 0) ELSE ISNULL(A.買上金額, 0) END");
                        sql.AppendLine("          ELSE ISNULL(A.買上金額, 0) + ISNULL(A.消費税, 0)");
                        sql.AppendLine("          END AS 今回請求額");
                    }
                    else
                    {
                        sql.AppendLine("     ,ISNULL(A.今回請求額, 0) AS 今回請求額");    // 請求額を出力する場合
                    }
                    sql.AppendLine("     ,ISNULL(A.繰越額, 0) AS 繰越額");
                    sql.AppendLine("     ,A.請求日");
                    sql.AppendLine("     ,A.回収予定日 AS 支払日");
                    sql.AppendLine("     ,CASE A.締区分 WHEN @締区分月 THEN CAST(CAST(SUBSTRING(CONVERT(VARCHAR,A.請求日,112),5,2) AS INT) AS VARCHAR) + '月分お弁当代として'");
                    sql.AppendLine("                    ELSE 'お弁当代として' END AS 但書");
                    sql.AppendLine("     ,B.会社名称 AS 自社名称");
                    sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                    sql.AppendLine("     ,B.住所 AS 自社住所");
                    sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                    sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                    sql.AppendLine("     ,B.登録番号");
                    sql.AppendLine("     ,G.印影 AS 印鑑画像");
                    if (_receiptPrint.発行日 == null)
                    {
                        sql.AppendLine("     ,'' AS 発行日");
                    }
                    else
                    {
                        sql.AppendFormat("     ,'{0:yyyy/MM/dd}' AS 発行日", _receiptPrint.発行日).AppendLine();
                    }
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     請求合計 AS A");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                    if (_seikyuListCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT.コースＣＤ");
                        sql.AppendLine("             ,CT.得意先ＣＤ");
                        sql.AppendLine("             ,CT.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コースマスタ AS CM");
                        sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                        sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.コースＣＤ;
                    }
                    else
                    {
                        // -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
                        sql.AppendLine("     LEFT JOIN");
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT1.コースＣＤ");
                        sql.AppendLine("             ,CT1.得意先ＣＤ");
                        sql.AppendLine("             ,CT1.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コーステーブル AS CT1");
                        sql.AppendLine("             INNER JOIN");
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                     ,MIN(T.コースＣＤ) AS コースＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     コースマスタ AS M");
                        sql.AppendLine("                     LEFT JOIN コーステーブル AS T ON");
                        sql.AppendLine("                         T.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND T.コースＣＤ = M.コースＣＤ");
                        sql.AppendLine("                 GROUP BY");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                 ) AS CT2");
                        sql.AppendLine("                     ON CT2.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                     AND CT2.得意先ＣＤ = CT1.得意先ＣＤ");
                        sql.AppendLine("                     AND CT2.コースＣＤ = CT1.コースＣＤ");
                        sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                        sql.AppendLine("                 ON CM.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT1.コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("             ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("             AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("             AND J.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                    sql.AppendLine("         VS.請求書敬称CD = S.請求書敬称");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                    sql.AppendLine("         B.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署画像マスタ AS G ON");
                    sql.AppendLine("         G.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("         AND G.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@締区分月", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;

                    if (_seikyuListCriteria.部署ＣＤ != null && _seikyuListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("    AND A.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.部署ＣＤ;
                    }

                    if (_seikyuListCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("    AND A.請求先ＣＤ = @請求先ＣＤ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.得意先ＣＤ;
                    }

                    if (_seikyuListCriteria.請求日From != null)
                    {
                        sql.AppendLine("    AND A.請求日 >= @請求日From");
                        cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _seikyuListCriteria.請求日From;
                    }

                    if (_seikyuListCriteria.請求日To != null)
                    {
                        sql.AppendLine("    AND A.請求日 <= @請求日To");
                        cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _seikyuListCriteria.請求日To;
                    }

                    if (_seikyuListCriteria.回収予定日From != null)
                    {
                        sql.AppendLine("    AND A.回収予定日 >= @回収予定日From");
                        cmd.Parameters.Add("@回収予定日From", SqlDbType.Date).Value = _seikyuListCriteria.回収予定日From;
                    }

                    if (_seikyuListCriteria.回収予定日To != null)
                    {
                        sql.AppendLine("    AND A.回収予定日 <= @回収予定日To");
                        cmd.Parameters.Add("@回収予定日To", SqlDbType.Date).Value = _seikyuListCriteria.回収予定日To;
                    }

                    if (_seikyuListCriteria.処理日 != null)
                    {
                        sql.AppendLine("    AND CONVERT(date, A.修正日) = @処理日");
                        cmd.Parameters.Add("@処理日", SqlDbType.Date).Value = _seikyuListCriteria.処理日;
                    }

                    if (_seikyuListCriteria.今回請求額 != null)
                    {
                        sql.AppendLine("    AND A.今回請求額 >= @今回請求額");
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = _seikyuListCriteria.今回請求額;
                    }

                    if (_seikyuListCriteria.支払方法 != null && _seikyuListCriteria.支払方法 != -1)
                    {
                        sql.AppendLine("    AND S.支払方法 = @支払方法");
                        cmd.Parameters.Add("@支払方法", SqlDbType.Int).Value = _seikyuListCriteria.支払方法;

                        // 支払方法が振替であれば、EB銀行支店ＣＤを条件に追加する
                        if (_seikyuListCriteria.支払方法 == (int)Consts.ShiharaiHohoKubun.Furikae)
                        {
                            if (string.IsNullOrEmpty(_seikyuListCriteria.EB銀行支店ＣＤ) == false)
                            {
                                sql.AppendLine("    AND S.EB銀行支店ＣＤ = @EB銀行支店ＣＤ");
                                cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = _seikyuListCriteria.EB銀行支店ＣＤ;
                            }
                        }
                    }

                    if (_seikyuListCriteria.締区分 != null && _seikyuListCriteria.締区分 != -1)
                    {
                        sql.AppendLine("    AND A.締区分 = @締区分");
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = _seikyuListCriteria.締区分;
                    }

                    if (_seikyuListCriteria.週締曜日 != null && _seikyuListCriteria.週締曜日 != -1)
                    {
                        sql.AppendLine("    AND S.週締曜日 = @週締曜日");
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = _seikyuListCriteria.週締曜日;
                    }

                    if (_seikyuListCriteria.月締日 != null)
                    {
                        sql.AppendLine("    AND S.月締日 = @月締日");
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _seikyuListCriteria.月締日;
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat(_seikyuListCriteria.ソート順, _seikyuListCriteria.昇順 ? "ASC" : "DESC");

                    cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                    cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("領収書");
                    adp.Fill(dt);

                    // 内訳の列を追加（軽減税率対応）
                    dt.Columns.Add("消費税内訳", typeof(string));

                    // 必要な列を追加（インボイス対応）
                    dt.Columns.Add("消費税内訳ラベル1", typeof(string));
                    dt.Columns.Add("消費税内訳ラベル2", typeof(string));
                    dt.Columns.Add("軽減税率", typeof(decimal));
                    dt.Columns.Add("通常税率", typeof(decimal));
                    dt.Columns.Add("軽減対象額", typeof(string));    // 印刷用フォーマットでデータを詰めるので、stringで追加
                    dt.Columns.Add("通常対象額", typeof(string));
                    dt.Columns.Add("軽減消費税", typeof(string));
                    dt.Columns.Add("通常消費税", typeof(string));

                    // 軽減と通常の区別は使わない（2027.04.01からの軽減税率減税により8％と1％が混在する場合があるため）
                    // 現在レイアウトは3つまでしか対応していない
                    dt.Columns.Add("税率1", typeof(decimal));
                    dt.Columns.Add("税率2", typeof(decimal));
                    dt.Columns.Add("税率3", typeof(decimal));
                    dt.Columns.Add("対象額1", typeof(string));
                    dt.Columns.Add("対象額2", typeof(string));
                    dt.Columns.Add("対象額3", typeof(string));
                    dt.Columns.Add("消費税1", typeof(string));
                    dt.Columns.Add("消費税2", typeof(string));
                    dt.Columns.Add("消費税3", typeof(string));

                    // 全ての行に対して以下を設定
                    // ・内消費税額
                    // ・但書
                    // ・税率ごとの内訳
                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal seikyuNo = (decimal)dr["請求番号"];
                        KeigenZeiritsu keigenZeiritsu = GetKeigenZeiritsu(isPrintRateDetailForReceipt, seikyuNo, dtMeisai);

                        // 軽減税率に対応した場合、請求額を出力する場合は繰越額の消費税も考慮する必要がある。
                        // 繰越がある場合は入金もありうる。どの売上明細に対してどれくらい入金したのかは求められない。
                        // したがって繰越額に対する正確な8%／10%対象は求められない。それでも内訳を算出するとしたら、税率ごとの売上合計の割合に応じて繰越額を按分した値を売上合計に加算したものを内訳とする。
                        // ┌─────────────┬────────────────────────────────────────┐
                        // │請求データ                │出力内容                                                                        │
                        // ├───────┬─────┼─────────────────┬─────────────┬────────┤
                        // │請求額/売上額 │内税/外税 │金額                              │内訳                      │消費税          │
                        // ├───────┼─────┼─────────────────┼─────────────┼────────┤
                        // │売上額        │内税      │請求合計.買上金額                 │請求対象の売上から算出(☆)│内訳から算出    │
                        // ├───────┼─────┼─────────────────┼─────────────┼────────┤
                        // │売上額        │外税      │請求合計.買上金額＋請求合計.消費税│請求対象の売上から算出(☆)│請求合計.消費税 │
                        // ├───────┼─────┼─────────────────┼─────────────┼────────┤
                        // │請求額        │内税      │請求合計.今回請求額               │(注1)                     │内訳から算出    │
                        // ├───────┼─────┼─────────────────┼─────────────┼────────┤
                        // │請求額        │外税      │請求合計.今回請求額               │(注2)                     │内訳から算出    │
                        // └───────┴─────┴─────────────────┴─────────────┴────────┘
                        // (注1)
                        // (☆)＋請求合計.繰越額を(☆)の割合で按分したもの
                        //
                        // (注2)
                        // (☆)＋請求合計.繰越額を(☆)の割合で按分したもの(▲)。
                        // ただし、請求合計.繰越額には外税額が含まれているので、(▲)はそれぞれの税率に応じた外税額を抜いた値であること。

                        int? hasuKubun = dr["丸め処理区分"] as int?;
                        int zeiKubun = (int)dr["税区分"];

                        // 請求額を出力する場合
                        if (rdoUriage.Checked == false)
                        {
                            CalcKurikoshi(keigenZeiritsu, zeiKubun, (decimal)dr["繰越額"], hasuKubun);
                            if (zeiKubun == (int)Syohizei.ZeiKubun.Soto)
                            {
                                dr["消費税"] = keigenZeiritsu.GetTaxExcluded(hasuKubun);
                            }
                            else
                            {
                                dr["消費税"] = keigenZeiritsu.GetTaxIncluded(hasuKubun);
                            }
                        }
                        else
                        {
                            // 売上額を出力する場合
                            // 求めた消費税額を設定するのは内税の場合のみ（外税の場合は設定済み）
                            if (dr.IsNull("消費税"))
                            {
                                dr["消費税"] = keigenZeiritsu.GetTaxIncluded(hasuKubun);
                            }
                        }

                        if (string.IsNullOrEmpty(keigenZeiritsu.Proviso) == false)
                        {
                            dr["但書"] = keigenZeiritsu.Proviso;
                        }

                        dr["消費税内訳"] = keigenZeiritsu.TaxDetail;     // レイアウトでは未使用

                        // インボイス対応
                        dr["消費税内訳ラベル1"] = string.Format("{0}対象額", (zeiKubun == (int)Syohizei.ZeiKubun.Uchi) ? "内税" : "外税");
                        dr["消費税内訳ラベル2"] = string.Format("{0}消費税", (zeiKubun == (int)Syohizei.ZeiKubun.Uchi) ? "内" : "");

                        int i = 1;
                        foreach (decimal taxRate in keigenZeiritsu.KingakuMap.Keys)
                        {
                            // 金額がない場合は内訳を出力しない
                            // 3つの税率までしか追加しない
                            if (keigenZeiritsu.KingakuMap[taxRate] == 0m || i > 3)
                            {
                                continue;
                            }

                            string str = (taxRate == keigenZeiritsu.ReducedTaxRate) ? "軽減" : "通常";

                            decimal tax = Syohizei.GetTax(zeiKubun, keigenZeiritsu.KingakuMap[taxRate], taxRate, hasuKubun);

                            if (zeiKubun == (int)Syohizei.ZeiKubun.Uchi)
                            {
                                tax = Syohizei.GetTaxIncluded(zeiKubun, keigenZeiritsu.KingakuMap[taxRate], taxRate, hasuKubun);
                            }

                            //dr[str + "税率"] = taxRate;
                            //dr[str + "対象額"] = string.Format("{0:N0}円", keigenZeiritsu.KingakuMap[taxRate]);
                            //dr[str + "消費税"] = string.Format("{0:N0}円", tax);

                            dr["税率" + i] = taxRate;
                            dr["対象額" + i] = string.Format("{0:N0}円", keigenZeiritsu.KingakuMap[taxRate]);
                            dr["消費税" + i] = string.Format("{0:N0}円", tax);
                            i++;
                        }
                    }

                    report.TopOffsetCM = topOffset;
                    report.LeftOffsetCM = leftOffset;
                    report.DataSource = dt;

                    // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                    PrintReport(report, printSettings);

                    log.InfoFormat("領収書を印刷しました。件数={0}", dt.Rows.Count);
                    History.Save(con, this.Text, "領収書", "", History.ProcessType.Output);
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 内消費税額と、税率ごとの内訳取得用の売上明細を取得
        /// </summary>
        /// <returns>売上明細データテーブル</returns>
        private DataTable GetUriageMeisai()
        {
            DataTable dt = new DataTable("売上明細");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     G.請求番号");
                    sql.AppendLine("     ,G.請求先名");
                    sql.AppendLine("     ,URI.売上日");
                    sql.AppendLine("     ,URI.値引後金額");
                    sql.AppendLine("     ,URI.計算税率");
                    sql.AppendLine(" FROM");
                    // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         G1.企業ＣＤ");
                    sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                    sql.AppendLine("         ,G1.請求先ＣＤ");
                    sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         請求合計 AS G1");
                    sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                    sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                    sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                    sql.AppendLine("             AND P1.請求日 < G1.請求日");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_seikyuListCriteria.部署ＣＤ != null && _seikyuListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("         AND G1.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.部署ＣＤ;
                    }

                    if (_seikyuListCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("         AND G1.請求先ＣＤ = @請求先ＣＤ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.得意先ＣＤ;
                    }

                    if (_seikyuListCriteria.請求日From != null)
                    {
                        sql.AppendLine("         AND G1.請求日 >= @請求日From");
                        cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _seikyuListCriteria.請求日From;
                    }

                    if (_seikyuListCriteria.請求日To != null)
                    {
                        sql.AppendLine("         AND G1.請求日 <= @請求日To");
                        cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _seikyuListCriteria.請求日To;
                    }

                    if (_seikyuListCriteria.回収予定日From != null)
                    {
                        sql.AppendLine("         AND G1.回収予定日 >= @回収予定日From");
                        cmd.Parameters.Add("@回収予定日From", SqlDbType.Date).Value = _seikyuListCriteria.回収予定日From;
                    }

                    if (_seikyuListCriteria.回収予定日To != null)
                    {
                        sql.AppendLine("         AND G1.回収予定日 <= @回収予定日To");
                        cmd.Parameters.Add("@回収予定日To", SqlDbType.Date).Value = _seikyuListCriteria.回収予定日To;
                    }

                    if (_seikyuListCriteria.処理日 != null)
                    {
                        sql.AppendLine("         AND CONVERT(date, G1.修正日) = @処理日");
                        cmd.Parameters.Add("@処理日", SqlDbType.Date).Value = _seikyuListCriteria.処理日;
                    }

                    if (_seikyuListCriteria.今回請求額 != null)
                    {
                        sql.AppendLine("         AND G1.今回請求額 >= @今回請求額");
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = _seikyuListCriteria.今回請求額;
                    }

                    if (_seikyuListCriteria.締区分 != null && _seikyuListCriteria.締区分 != -1)
                    {
                        sql.AppendLine("         AND G1.締区分 = @締区分");
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = _seikyuListCriteria.締区分;
                    }
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         G1.企業ＣＤ");
                    sql.AppendLine("         ,G1.請求番号");
                    sql.AppendLine("         ,G1.請求先ＣＤ");
                    sql.AppendLine("     ) AS P");
                    // -- ②今回請求の詳細
                    sql.AppendLine("     INNER JOIN 請求合計 G ON");
                    sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                    sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS T");
                    sql.AppendLine("         ON  T.企業ＣＤ = G.企業ＣＤ");
                    sql.AppendLine("         AND T.得意先ＣＤ = G.請求先ＣＤ");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS J ");
                    sql.AppendLine("         ON J.企業ＣＤ = T.企業ＣＤ ");
                    sql.AppendLine("         AND J.得意先ＣＤ = ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ)");
                    // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                    // --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("              企業ＣＤ");
                    sql.AppendLine("              ,請求番号");
                    sql.AppendLine("              ,部署ＣＤ");
                    sql.AppendLine("              ,得意先ＣＤ");
                    sql.AppendLine("              ,請求先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             請求ヘッダ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             売上金額税込 <> 0");
                    sql.AppendLine("         ) AS H ON");
                    sql.AppendLine("         H.企業ＣＤ = G.企業ＣＤ");
                    sql.AppendLine("         AND H.請求番号 = G.請求番号");
                    // -- ④今回の請求に対する最新の売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             UH.企業ＣＤ");
                    sql.AppendLine("             ,UH.得意先ＣＤ");
                    sql.AppendLine("             ,UH.部署ＣＤ");
                    sql.AppendLine("             ,UH.売上日");
                    sql.AppendLine("             ,UH.納品書伝票番号");
                    sql.AppendLine("             ,UM.明細番号");
                    sql.AppendLine("             ,UM.商品ＣＤ");
                    sql.AppendLine("             ,UM.商品単価");
                    sql.AppendLine("             ,UM.値引後金額");
                    sql.AppendLine("             ,UM.商品数量");
                    sql.AppendLine("             ,UM.計算税率");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS UH");
                    sql.AppendLine("             INNER JOIN 売上明細 AS UM ON");
                    sql.AppendLine("                 UM.企業ＣＤ = UH.企業ＣＤ");
                    sql.AppendLine("                 AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                    sql.AppendLine("                 AND UM.部署ＣＤ = UH.部署ＣＤ");
                    sql.AppendLine("                 AND UM.売上日 = UH.売上日");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             UM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                    sql.AppendLine("             AND ISNULL(UM.商品数量, 0) <> 0");
                    sql.AppendLine("         ) AS URI ON");
                    sql.AppendLine("         URI.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("         AND URI.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("         AND URI.部署ＣＤ = H.部署ＣＤ");
                    sql.AppendLine("         AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                    sql.AppendLine("         AND URI.売上日 <= G.請求日");

                    if (_seikyuListCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT.コースＣＤ");
                        sql.AppendLine("             ,CT.得意先ＣＤ");
                        sql.AppendLine("             ,CT.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コースマスタ AS CM");
                        sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                        sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND G.部署ＣＤ = C.部署ＣＤ");     // -- 請求の部署と一致するものに限る
                        sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.コースＣＤ;
                    }

                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");

                    if (_seikyuListCriteria.支払方法 != null && _seikyuListCriteria.支払方法 != -1)
                    {
                        sql.AppendLine("     AND T.支払方法 = @支払方法");
                        cmd.Parameters.Add("@支払方法", SqlDbType.Int).Value = _seikyuListCriteria.支払方法;

                        // 支払方法が振替であれば、EB銀行支店ＣＤを条件に追加する
                        if (_seikyuListCriteria.支払方法 == (int)Consts.ShiharaiHohoKubun.Furikae)
                        {
                            if (string.IsNullOrEmpty(_seikyuListCriteria.EB銀行支店ＣＤ) == false)
                            {
                                sql.AppendLine("     AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ      -- 支払方法が振替の場合のみ有効");
                                cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = _seikyuListCriteria.EB銀行支店ＣＤ;
                            }
                        }
                    }

                    if (_seikyuListCriteria.週締曜日 != null && _seikyuListCriteria.週締曜日 != -1)
                    {
                        sql.AppendLine("     AND T.週締曜日 = @週締曜日");
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = _seikyuListCriteria.週締曜日;
                    }

                    if (_seikyuListCriteria.月締日 != null)
                    {
                        sql.AppendLine("     AND T.月締日 = @月締日");
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _seikyuListCriteria.月締日;
                    }

                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("売上明細を取得しました。");
                }

                return dt;
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
        /// 指定した請求先の売上明細から求めた軽減税率算出用クラスを返します
        /// </summary>
        /// <param name="isPrintRateDetailForReceipt">領収書に税率ごとの内訳を出力する場合はtrue。しない場合はfalse。</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="dtMeisai">内消費税額と、税率ごとの内訳取得用の売上明細が格納されたデータテーブル</param>
        /// <returns>軽減税率算出用クラス</returns>
        private KeigenZeiritsu GetKeigenZeiritsu(bool isPrintRateDetailForReceipt, decimal seikyuNo, DataTable dtMeisai)
        {
            try
            {
                KeigenZeiritsu keigenZeiritsu = new KeigenZeiritsu(isPrintRateDetailForReceipt);
                DataRow[] drs = dtMeisai.Select(string.Format("請求番号={0}", seikyuNo));
                foreach (DataRow dr in drs)
                {
                    if (dr.IsNull("売上日") == false)
                    {
                        Syohizei syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);

                        // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                        // 税率ごとの合計金額を初期化
                        keigenZeiritsu.InitKingaku(syohizei);

                        // 税率ごとの金額を加算
                        decimal taxRate = (decimal)dr["計算税率"];
                        keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値引後金額"]);

                        // 但書と内訳の出力制御のために、適用前／通常／軽減の件数を集計する
                        keigenZeiritsu.CountKeigen(syohizei, taxRate);
                    }
                }

                return keigenZeiritsu;
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
        /// 繰越額を按分して税率ごとの売上合計額に加算する
        /// </summary>
        /// <param name="keigenZeiritsu">軽減税率算出用クラス</param>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="kurikoshi">繰越額</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        private void CalcKurikoshi(KeigenZeiritsu keigenZeiritsu, int zeiKubun, decimal kurikoshi, int? hasuKubun)
        {
            try
            {
                //****************************************************************************
                // 繰越額を、税率ごとの売上合計金額／売上合計金額の総計 で求めた割合で按分する
                //****************************************************************************
                decimal gross = keigenZeiritsu.KingakuMap.Values.Sum(); // 売上合計金額の総計を算出

                // 割合を算出して、繰越額を按分する
                Dictionary<decimal, decimal> kurikoshiMap = new Dictionary<decimal, decimal>();     // 按分した繰越額を格納するマップ
                if (gross == 0m)
                {
                    // 売上が無い場合は、繰越額を全額8%対象とする
                    kurikoshiMap[0.08m] = kurikoshi;
                }
                else
                {
                    // 売上金額の割合を税率ごとに求め、繰越額を按分する
                    foreach (decimal taxRate in keigenZeiritsu.KingakuMap.Keys)
                    {
                        decimal kingaku = keigenZeiritsu.KingakuMap[taxRate];
                        decimal ratio = (kingaku / gross);
                        kurikoshiMap[taxRate] = Rounding.Round(kurikoshi * ratio, Rounding.HasuKubun.Round);    // 四捨五入
                    }
                }

                // 外税であれば繰越額には消費税額が含まれているので、按分した繰越額から外税額を抜く
                if (zeiKubun == (int)Syohizei.ZeiKubun.Soto)
                {
                    // kurikoshiMapの内容を更新する（=コレクションを修正する）ことになるので、InvalidOperationExceptionを回避すべく、いったん別のマップに格納する
                    Dictionary<decimal, decimal> wk_kurikoshiMap = new Dictionary<decimal, decimal>();
                    foreach (decimal taxRate in kurikoshiMap.Keys)
                    {
                        decimal kingaku = kurikoshiMap[taxRate];
                        decimal tax = Syohizei.GetTaxIncluded(Syohizei.ZeiKubun.Uchi, kingaku, taxRate, hasuKubun);     // 金額に含まれる外税額≒内税額
                        kingaku -= tax;
                        wk_kurikoshiMap[taxRate] = kingaku;
                    }
                    kurikoshiMap = wk_kurikoshiMap;
                }

                // 按分した繰越額を、税率ごとの売上合計額に加算する
                foreach (decimal taxRate in kurikoshiMap.Keys)
                {
                    decimal kingaku = kurikoshiMap[taxRate];        // 按分した繰越額

                    if (keigenZeiritsu.KingakuMap.ContainsKey(taxRate) == false)
                    {
                        keigenZeiritsu.KingakuMap[taxRate] = 0m;
                    }
                    keigenZeiritsu.KingakuMap[taxRate] += kingaku;  // 税率ごとの売上合計額に加算
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

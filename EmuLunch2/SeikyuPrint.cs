using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using GrapeCity.ActiveReports.Export.Pdf.Section;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// 請求書発行
    /// </summary>
    public class SeikyuPrint
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
            //* スタティックメソッド
            //**************************************
            #region スタティックメソッド
            /// <summary>
            /// 税率ごとの合計金額に含まれる内税額を返します
            /// </summary>
            /// <param name="kingakuMap">税率ごとの合計金額を保持するマップ</param>
            /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
            /// <returns>税率ごとの合計金額に含まれる内税額の合計</returns>
            public static decimal GetTaxIncluded(SortedList<decimal, decimal> kingakuMap, int? hasuKubun)
            {
                decimal totalTax = 0m;
                foreach (decimal rate in kingakuMap.Keys)
                {
                    // 金額に含まれる内税額を算出
                    totalTax += Syohizei.GetTaxIncluded(Syohizei.ZeiKubun.Uchi, kingakuMap[rate], rate, hasuKubun);
                }
                return totalTax;
            }

            /// <summary>
            /// フッターに出力する内訳を返します
            /// </summary>
            /// <param name="kingakuMap">税率ごとの合計金額を保持するマップ</param>
            /// <returns>フッターに出力する内訳。ただし３行目の「注)～」はここでは返さない</returns>
            public static string GenerateDetail(SortedList<decimal, decimal> kingakuMap)
            {
                StringBuilder detail = new StringBuilder();
                for (int i = kingakuMap.Count - 1; i >= 0; i--)   // 税率の降順
                {
                    detail.AppendFormat("({0:P0}対象 {1:N0}円)", kingakuMap.Keys[i], kingakuMap.Values[i]);     // 10%対象 1,000円
                    detail.AppendLine();
                }
                return detail.ToString();
            }

            /// <summary>
            /// フッターに出力する内訳を返します
            /// </summary>
            /// <param name="kingakuMap">税率ごとの合計金額を保持するマップ</param>
            /// <returns>フッターに出力する内訳。ただし３行目の「注)～」はここでは返さない</returns>
            public static string GenerateDetail(SortedList<decimal, decimal> kingakuMap, SortedList<decimal, decimal> taxAmountMap, bool isExcludedTax)
            {
                string taxKbn = isExcludedTax ? "外税" : "内税";
                string taxLabel = isExcludedTax ? "消費税額" : "内消費税額";

                StringBuilder detail = new StringBuilder();
                for (int i = kingakuMap.Count - 1; i >= 0; i--)   // 税率の降順
                {
                    // 金額がある場合のみ表示
                    if (kingakuMap.Values[i] != 0m)
                    {
                        detail.AppendFormat("({0} {1:P0}対象 {2:N0}円  {3} {4:N0}円)", taxKbn, kingakuMap.Keys[i], kingakuMap.Values[i], taxLabel, taxAmountMap.Values[i]);     // 内税 10%対象 1,000円  内消費税額 90円
                        detail.AppendLine();
                    }
                }
                return detail.ToString();
            }
            #endregion

            //**************************************
            //* インスタンス変数
            //**************************************
            #region インスタンス変数
            /// <summary>
            /// 軽減税率の商品を示す「※」を、全ての商品が軽減税率対象であっても出力するかどうか
            /// </summary>
            private bool _isPrintMarkWhenAllReducedTaxRate;

            /// <summary>
            /// 税率ごとの合計金額を保持するマップ
            /// </summary>
            private SortedList<decimal, decimal> _kingakuMap;

            /// <summary>
            /// 税率ごとの合計消費税額を保持するマップ
            /// </summary>
            private SortedList<decimal, decimal> _taxAmountMap;

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
            private decimal _normalTaxRate;

            /// <summary>
            /// 軽減税率
            /// </summary>
            private decimal _reducedTaxRate;

            /// <summary>
            /// 軽減税率対象を示すマーク「※」
            /// </summary>
            private string _mark;

            /// <summary>
            /// 内訳
            /// </summary>
            private string _taxDetail;

            /// <summary>
            /// 外税かどうか
            /// </summary>
            private bool _isExcludedTax = false;
            #endregion

            //**************************************
            //* コンストラクタ
            //**************************************
            #region コンストラクタ
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="isPrintMarkWhenAllReducedTaxRate">全ての商品が軽減税率対象の場合に「※」を出力したくない場合はfalse。出力する場合はtrue。</param>
            public KeigenZeiritsu(bool isPrintMarkWhenAllReducedTaxRate)
            {
                _isPrintMarkWhenAllReducedTaxRate = isPrintMarkWhenAllReducedTaxRate;

                _kingakuMap = new SortedList<decimal, decimal>();
                _taxAmountMap = new SortedList<decimal, decimal>();
                _countMap = new Dictionary<TaxKind, int>();
                _hasKeigen = false;
                _mark = null;
                _taxDetail = null;
                _isExcludedTax = false;
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
            /// 税率ごとの合計消費税額を保持するマップ
            /// </summary>
            public SortedList<decimal, decimal> TaxAmountMap
            {
                get
                {
                    return _taxAmountMap;
                }
            }

            /// <summary>
            /// 軽減税率対象を示すマーク(※)
            /// </summary>
            public string Mark
            {
                get
                {
                    if (_mark == null)
                    {
                        GenerateMarkAndDetail();
                    }
                    return _mark;
                }
            }

            /// <summary>
            /// フッターに出力する内訳
            /// </summary>
            public string TaxDetail
            {
                get
                {
                    if (_taxDetail == null)
                    {
                        GenerateMarkAndDetail();
                    }
                    return _taxDetail;
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
                if (_taxAmountMap.ContainsKey((decimal)syohizei.消費税率) == false)
                {
                    _taxAmountMap[(decimal)syohizei.消費税率] = 0m;
                }

                if (syohizei.HasKeigen())
                {
                    _hasKeigen = true;
                    _reducedTaxRate = (decimal)syohizei.軽減税率;       // 軽減税率を取得
                    if (_kingakuMap.ContainsKey((decimal)syohizei.軽減税率) == false)
                    {
                        _kingakuMap[(decimal)syohizei.軽減税率] = 0m;
                    }
                    if (_taxAmountMap.ContainsKey((decimal)syohizei.軽減税率) == false)
                    {
                        _taxAmountMap[(decimal)syohizei.軽減税率] = 0m;
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
                if (_kingakuMap.ContainsKey(taxRate) == false)  // 消費税率マスタに存在しない税率でもここで追加される
                {
                    _kingakuMap[taxRate] = 0m;
                }
                _kingakuMap[taxRate] += kingaku;
            }

            /// <summary>
            /// 税率ごとの合計消費税額を計算する
            /// </summary>
            /// <param name="zeiKbn">税区分</param>
            public void CalcTaxAmount(int zeiKbn, int? hasuKbn)
            {
                _isExcludedTax = (zeiKbn != (int)Syohizei.ZeiKubun.Hikazei && zeiKbn == (int)Syohizei.ZeiKubun.Soto);

                foreach (decimal rate in _kingakuMap.Keys)
                {
                    if (_isExcludedTax)
                    {
                        // 外税
                        _taxAmountMap[rate] = Syohizei.GetTax(Syohizei.ZeiKubun.Soto, _kingakuMap[rate], rate, hasuKbn);
                    }
                    else
                    {
                        // 内税
                        _taxAmountMap[rate] = Syohizei.GetTaxIncluded(Syohizei.ZeiKubun.Uchi, _kingakuMap[rate], rate, hasuKbn);
                    }
                }
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
                    //if (taxRate == syohizei.軽減税率)
                    if (taxRate != syohizei.消費税率)   // 通常税率と異なる場合に軽減税率と判定する（2027.04.01からの軽減税率減税により8％と1％が混在する場合があるため）
                    {
                        taxKind = TaxKind.Reduced;
                    }
                    _normalTaxRate = (decimal)syohizei.消費税率;    // (軽減税率に対する)通常税率を取得
                }

                if (_countMap.ContainsKey(taxKind) == false)
                {
                    _countMap[taxKind] = 0;
                }
                _countMap[taxKind]++;
            }

            /// <summary>
            /// 軽減税率対象を示すマークと、フッターに出力する内訳を生成します
            /// </summary>
            private void GenerateMarkAndDetail()
            {
                _mark = "";

                StringBuilder detail = new StringBuilder();

                //detail.Append(GenerateDetail(_kingakuMap));     // 税率ごとの金額を生成
                detail.Append(GenerateDetail(_kingakuMap, _taxAmountMap, _isExcludedTax));     // 税率ごとの金額を生成

                // 軽減税率の商品であれば「※」を出力するのが原則。しかし、全ての商品が軽減税率対象の場合に「※」を出力したくない場合もある。
                // ┌────────────────┬─────────┬─────────┬─────────────────┐
                // │isPrintMarkWhenAllReducedTaxRate│全て軽減税率対象  │全て通常税率対象  │両者が混在                        │
                // ├────────────────┼─────────┼─────────┼─────────────────┤
                // │true                            │「※」を出力する  │「※」は出力しない│軽減税率対象にのみ「※」を出力する│
                // ├────────────────┼─────────┼─────────┼─────────────────┤
                // │false                           │「※」は出力しない│「※」は出力しない│軽減税率対象にのみ「※」を出力する│
                // └────────────────┴─────────┴─────────┴─────────────────┘

                // 注釈から税率の表示を除去（2027.04.01からの軽減税率減税により8％と1％が混在する場合があるため）

                // 全て軽減対象でも※を出力？
                if (_isPrintMarkWhenAllReducedTaxRate)
                {
                    // 軽減税率が始まっている？
                    if (_hasKeigen)
                    {
                        //detail.AppendFormat("注)※印は軽減税率({0:P0})適用商品", _reducedTaxRate);
                        detail.Append("注)※印は軽減税率適用商品");
                        _mark = "　※";
                    }
                }
                else
                {
                    // 全て軽減税率対象の場合は※を出力しない

                    // ┌─┬───┬──┬──┬───────────────┐
                    // │No│適用前│通常│軽減│注)の内容                     │
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 1│      │    │ ○ │全て軽減税率(8%)適用商品です  │
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 2│      │ ○ │    │全て通常税率(10%)適用商品です │
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 3│      │ ○ │ ○ │※印は軽減税率(8%)適用商品    │
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 4│  ○  │    │    │                              │
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 5│  ○  │    │ ○ │全て軽減税率(8%)適用商品です  │
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 6│  ○  │ ○ │    │※印は軽減税率(8%)適用商品    │明細に※は付加されない
                    // ├─┼───┼──┼──┼───────────────┤
                    // │ 7│  ○  │ ○ │ ○ │※印は軽減税率(8%)適用商品    │
                    // └─┴───┴──┴──┴───────────────┘

                    // 軽減税率が始まっている(No4以外)？
                    if (_hasKeigen)
                    {
                        //string s = string.Format("注)※印は軽減税率({0:P0})適用商品", _reducedTaxRate);
                        string s = "注)※印は軽減税率適用商品";
                        _mark = "　※";

                        // 軽減のみ(No1)？
                        if (_countMap.ContainsKey(TaxKind.Unapplied) == false &&
                            _countMap.ContainsKey(TaxKind.Normal) == false &&
                            _countMap.ContainsKey(TaxKind.Reduced) == true)
                        {
                            //s = string.Format("注)全て軽減税率({0:P0})適用商品です", _reducedTaxRate);
                            s = "注)全て軽減税率適用商品です";
                            _mark = "";
                        }

                        // 通常のみ(No2)？
                        if (_countMap.ContainsKey(TaxKind.Unapplied) == false &&
                            _countMap.ContainsKey(TaxKind.Normal) == true &&
                            _countMap.ContainsKey(TaxKind.Reduced) == false)
                        {
                            //s = string.Format("注)全て通常税率({0:P0})適用商品です", _normalTaxRate);
                            s = "注)全て通常税率適用商品です";
                            _mark = "";
                        }

                        // 8%のみ(No5)？
                        if (_countMap.ContainsKey(TaxKind.Unapplied) == true &&
                            _countMap.ContainsKey(TaxKind.Normal) == false &&
                            _countMap.ContainsKey(TaxKind.Reduced) == true)
                        {
                            //s = string.Format("注)全て軽減税率({0:P0})適用商品です", _reducedTaxRate);
                            s = "注)全て軽減税率適用商品です";
                            _mark = "";
                        }

                        detail.Append(s);
                    }
                }

                _taxDetail = detail.ToString();
            }

            /// <summary>
            /// 税率ごとの合計金額に含まれる内税額を返します
            /// </summary>
            /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
            /// <returns>税率ごとの合計金額に含まれる内税額の合計</returns>
            public decimal GetTaxIncluded(int? hasuKubun)
            {
                return GetTaxIncluded(_kingakuMap, hasuKubun);
            }
            #endregion
        }
        #endregion

        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CALENDAR_TATE_COLUMNS = 6;        // カレンダー縦の１ページあたりの列数
        #endregion

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
        /// <summary>
        /// 呼び出し元画面のタイトル
        /// </summary>
        private string _ownerTitle;

        /// <summary>
        /// 宛名の上下方向オフセット量(cm)
        /// </summary>
        private float _topOffset;

        /// <summary>
        /// 宛名の左右方向オフセット量(cm)
        /// </summary>
        private float _leftOffset;

        /// <summary>
        /// 指定した部署のロゴ画像のバイト配列
        /// </summary>
        private byte[] _logoImageBytes = null;

        /// <summary>
        /// 指定した部署CDの印鑑画像のバイト配列
        /// </summary>
        private byte[] _inkanImageBytes = null;

        /// <summary>
        /// 請求書の「内消費税」見出しを出力するかどうか
        /// </summary>
        private bool _isPrintTaxIncludeTitle;

        /// <summary>
        /// 軽減税率の商品を示す「※」を、全ての商品が軽減税率対象であっても出力するかどうか
        /// </summary>
        private bool _isPrintMarkWhenAllReducedTaxRate;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ownerTitle">呼び出し元画面のタイトル</param>
        public SeikyuPrint(string ownerTitle)
        {
            _ownerTitle = ownerTitle;

            // システム設定マスタから宛名のオフセット量を取得
            _topOffset = SystemMaster.GetSeikyusyoAddrTopOffset();
            _leftOffset = SystemMaster.GetSeikyusyoAddrLeftOffset();

            // 請求書の「内消費税」見出しを出力するかどうか
            _isPrintTaxIncludeTitle = SystemMaster.IsPrintTaxIncludeTitle();

            // 軽減税率の商品を示す「※」を、全ての商品が軽減税率対象であっても出力するかどうか
            _isPrintMarkWhenAllReducedTaxRate = SystemMaster.IsPrintMarkWhenAllReducedTaxRate();
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 請求書・合算請求書マージ用レポート
        /// </summary>
        public Report000A Report000 { get; set; }

        /// <summary>
        /// 宛名の上下方向オフセット量(cm)
        /// </summary>
        public float TopOffset
        {
            get
            {
                return _topOffset;
            }
        }

        /// <summary>
        /// 宛名の左右方向オフセット量(cm)
        /// </summary>
        public float LeftOffset
        {
            get
            {
                return _leftOffset;
            }
        }

        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 帳票マージ開始
        /// </summary>
        public void PrintMergeInit()
        {
            Report000 = new Report000A();
            PrintMergeInit(Report000);
        }

        /// <summary>
        /// 帳票マージ開始（主に並列処理向け）
        /// </summary>
        /// <param name="report000A">請求書・合算請求書マージ用レポート</param>
        public void PrintMergeInit(Report000A report000A)
        {
            try
            {
                report000A.DataSource = new DataTable();
                report000A.Document.Printer.DocumentName = "請求書";

                log.Debug("マージ用帳票 Run 開始");
                report000A.Run();
                log.Debug("マージ用帳票 Run 終了");
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
        /// 帳票マージ追加
        /// </summary>
        /// <param name="rpt">追加対象の帳票</param>
        public void PrintMergeAdd(GrapeCity.ActiveReports.SectionReport rpt)
        {
            PrintMergeAdd(Report000, rpt);
        }

        /// <summary>
        /// 帳票マージ追加（主に並列処理向け）
        /// </summary>
        /// <param name="report000A">請求書・合算請求書マージ用レポート</param>
        /// <param name="rpt">追加対象の帳票</param>
        public void PrintMergeAdd(Report000A report000A, GrapeCity.ActiveReports.SectionReport rpt)
        {
            try
            {
                report000A.Document.Pages.AddRange((GrapeCity.ActiveReports.Document.Section.PagesCollection)rpt.Document.Pages.Clone());
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
        /// 帳票マージ終了
        /// </summary>
        public void PrintMergeFinish()
        {
            PrintMergeFinish(Report000);
        }

        /// <summary>
        /// 帳票マージ終了（主に並列処理向け）
        /// </summary>
        /// <param name="report000A">請求書・合算請求書マージ用レポート</param>
        public void PrintMergeFinish(Report000A report000A)
        {
            try
            {
                // 先頭ページを削除
                report000A.Document.Pages.RemoveAt(0);
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
        /// 帳票マージ終了・PDF出力
        /// </summary>
        /// <param name="pdfPath">PDF保存先フォルダ</param>
        /// <param name="pdfFileName">PDFファイル名</param>
        private void PrintMergeFinishPdf(string pdfPath, string pdfFileName)
        {
            PrintMergeFinishPdf(Report000, pdfPath, pdfFileName);
        }

        /// <summary>
        /// 帳票マージ終了・PDF出力（主に並列処理向け）
        /// </summary>
        /// <param name="pdfPath">PDF保存先フォルダ</param>
        /// <param name="pdfFileName">PDFファイル名</param>
        /// <param name="report000A">請求書・合算請求書マージ用レポート</param>
        private void PrintMergeFinishPdf(Report000A report000A, string pdfPath, string pdfFileName)
        {
            try
            {
                // 先頭ページを削除
                report000A.Document.Pages.RemoveAt(0);

                // PDFに出力
                // report000A.Run();
                PdfExport pdfExport = new PdfExport();
                pdfExport.Options.Title = "請求書";
                string filePath = Path.Combine(pdfPath, pdfFileName);
                pdfExport.Export(report000A.Document, filePath);
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
        /// ページ番号出力
        /// </summary>
        /// <param name="rpt">ページ番号出力の帳票</param>
        private void PrintPageTate(GrapeCity.ActiveReports.SectionReport rpt)
        {
            try
            {
                //*************************
                // ページ番号を直接出力する
                //*************************
                int pageNum = 1;
                int pages = rpt.Document.Pages.Count;       // 総ページ数

                Font f = new Font("ＭＳ Ｐゴシック", 9);    // フォントの設定

                // 出力位置の設定
                float width = 1.78125F;
                float height = 0.1354167F;
                float left = (rpt.Document.Pages[0].Width - width) / 2F;
                float top = rpt.Document.Pages[0].Height - rpt.Document.Pages[0].Margins.Bottom - 0.2185039F + 0.0388889F;  // ページ高さ－下余白－ページフッター高さ＋reportInfo1.Top
                RectangleF rect = new RectangleF(left, top, width, height);

                // 各ページにページ番号を出力
                foreach (GrapeCity.ActiveReports.Document.Section.Page page in rpt.Document.Pages)
                {
                    page.Font = f;
                    page.TextAlignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Center;
                    page.VerticalTextAlignment = GrapeCity.ActiveReports.Document.Section.VerticalTextAlignment.Top;
                    page.DrawText(string.Format("{0} / {1}", pageNum, pages), rect);
                    pageNum++;
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
        /// ページ番号出力
        /// </summary>
        /// <param name="rpt1">ページ番号出力の帳票(１枚目)</param>
        /// <param name="rpt2">ページ番号出力の帳票(２枚目)</param>
        private void PrintPageTate(GrapeCity.ActiveReports.SectionReport rpt1, GrapeCity.ActiveReports.SectionReport rpt2)
        {
            try
            {
                //*************************
                // ページ番号を直接出力する
                //*************************
                int pageNum = 1;
                int pages = rpt1.Document.Pages.Count + rpt2.Document.Pages.Count;       // 総ページ数

                Font f = new Font("ＭＳ Ｐゴシック", 9);    // フォントの設定

                // 出力位置の設定
                float width = 1.78125F;
                float height = 0.1354167F;
                float left = (rpt1.Document.Pages[0].Width - width) / 2F;
                float top = rpt1.Document.Pages[0].Height - rpt1.Document.Pages[0].Margins.Bottom - 0.2185039F + 0.0388889F;  // ページ高さ－下余白－ページフッター高さ＋reportInfo1.Top
                RectangleF rect = new RectangleF(left, top, width, height);

                // １枚目の各ページにページ番号を出力
                foreach (GrapeCity.ActiveReports.Document.Section.Page page in rpt1.Document.Pages)
                {
                    page.Font = f;
                    page.TextAlignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Center;
                    page.VerticalTextAlignment = GrapeCity.ActiveReports.Document.Section.VerticalTextAlignment.Top;
                    page.DrawText(string.Format("{0} / {1}", pageNum, pages), rect);
                    pageNum++;
                }

                // ２枚目の各ページにページ番号を出力
                foreach (GrapeCity.ActiveReports.Document.Section.Page page in rpt2.Document.Pages)
                {
                    page.Font = f;
                    page.TextAlignment = GrapeCity.ActiveReports.Document.Section.TextAlignment.Center;
                    page.VerticalTextAlignment = GrapeCity.ActiveReports.Document.Section.VerticalTextAlignment.Top;
                    page.DrawText(string.Format("{0} / {1}", pageNum, pages), rect);
                    pageNum++;
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
        /// 請求書印刷処理（１つの帳票として出力）
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="seikyuPrintTargetList">請求書印刷対象リスト</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="p">進行状況の更新のプロバイダー</param>
        /// <returns>成功した場合はnull。失敗した場合はエラーメッセージ</returns>
        public string PrintSeikyuWhole(int busyoCD, List<SeikyuPrintTarget> seikyuPrintTargetList, bool isGassan,
                                        bool isPrintKaisyuYotei, SeikyuCriteria seikyuCriteria, string pdfPath, IProgress<BaseForm.ProgressState> p)
        {
            try
            {
                // ロゴ・印鑑画像を取得
                BusyoImage busyoImage = BusyoImage.GetBusyoImage(busyoCD);
                _logoImageBytes = busyoImage.ロゴ;
                _inkanImageBytes = busyoImage.印影;

                int timeoutSec = SystemMaster.GetTimeoutSecForSeikyuPrint();                                // 請求書発行タイムアウト秒数を取得

                string pdfFileName = null;
                BaseForm.ProgressState progressState = new BaseForm.ProgressState();
                progressState.Style = ProgressBarStyle.Blocks;
                progressState.Maximum = seikyuPrintTargetList.Count;
                progressState.Value = 0;
                progressState.Message = string.Format("{0}/{1}", progressState.Value, progressState.Maximum);
                p.Report(progressState);

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    foreach (SeikyuPrintTarget seikyuPrintTarget in seikyuPrintTargetList)
                    {
                        decimal seikyuNo = seikyuPrintTarget.請求番号;
                        int seikyuCD = seikyuPrintTarget.請求先ＣＤ;
                        string seikyusakiName = seikyuPrintTarget.請求先名;
                        int type = seikyuPrintTarget.請求書タイプ区分;
                        int? courseCD = seikyuPrintTarget.コースＣＤ;

                        // PDFファイル名を生成（合算請求書の場合は１度だけ）
                        if (isGassan)
                        {
                            if (string.IsNullOrEmpty(pdfFileName))
                            {
                                // 合算請求書の場合は請求番号の前に"G"を付加する
                                pdfFileName = string.Format("{0:D9}_{1:yyyyMMdd}_G{2}.pdf", seikyuCD, seikyuPrintTarget.請求日, seikyuNo);
                            }
                        }
                        else
                        {
                            // 通常の請求書は請求番号ごとにPDFファイル名を生成
                            pdfFileName = string.Format("{0:yyyyMMdd}_{1}_{2:F0}_請求書.pdf", seikyuPrintTarget.請求日, seikyuCD, seikyuPrintTarget.今回請求額);
                        }

                        // 請求先にぶら下がる得意先の数を取得
                        int cnt = GetTokuisakiCount(con, seikyuNo);

                        // 請求書タイプで分岐
                        switch (type)
                        {
                            case (int)Consts.SeikyusyoType.Detail:          // 明細
                                if (cnt <= 1)
                                {
                                    // 請求先にぶら下がる得意先が自分しかいない場合（または売上を持つ得意先が全く無い場合）
                                    GrapeCity.ActiveReports.SectionReport rpt = PrintDetailSingle(con, seikyuNo, seikyusakiName, courseCD, isGassan, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                    // ページ番号出力(縦)
                                    PrintPageTate(rpt);

                                    // 追加先のActiveReportsドキュメントにページを追加
                                    PrintMergeAdd(rpt);
                                }
                                else
                                {
                                    Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap = new Dictionary<int, KeigenZeiritsu>();    // 得意先ごとに、軽減税率算出用クラスを保持するマップ

                                    // 請求先にぶら下がる得意先が複数ある場合
                                    // Ｑ．なぜ親→子の順ではなく子→親の順に生成しているのか？
                                    // Ａ．子で集計した「税率ごとの対象金額の内訳」（軽減税率対応）を親で使用するため
                                    GrapeCity.ActiveReports.SectionReport rptChild = PrintDetailChild(con, seikyuNo, seikyusakiName, courseCD, isGassan, keigenZeiritsuMap, seikyuCriteria, timeoutSec);       // 請求ヘッダと売上明細
                                    GrapeCity.ActiveReports.SectionReport rptParent = PrintDetailParent(con, seikyuNo, seikyusakiName, courseCD, isGassan, isPrintKaisyuYotei, keigenZeiritsuMap, seikyuCriteria, timeoutSec);     // 請求合計と請求ヘッダ

                                    // ページ番号を出力
                                    PrintPageTate(rptParent, rptChild);

                                    // 追加先のActiveReportsドキュメントにページを追加
                                    PrintMergeAdd(rptParent);
                                    PrintMergeAdd(rptChild);
                                }
                                break;

                            case (int)Consts.SeikyusyoType.CalendarYoko:    // カレンダー横
                                if (cnt <= 1)
                                {
                                    // 請求先にぶら下がる得意先が自分しかいない場合（または売上を持つ得意先が全く無い場合）
                                    GrapeCity.ActiveReports.SectionReport rpt = PrintCalendarYokoSingle(con, seikyuNo, seikyusakiName, courseCD, isGassan, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                    // 追加先のActiveReportsドキュメントにページを追加
                                    PrintMergeAdd(rpt);
                                }
                                else
                                {
                                    // 請求先にぶら下がる得意先が複数ある場合
                                    GrapeCity.ActiveReports.SectionReport rpt = PrintCalendarYokoMulti(con, seikyuNo, seikyusakiName, courseCD, isGassan, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                    // 追加先のActiveReportsドキュメントにページを追加
                                    PrintMergeAdd(rpt);
                                }
                                break;

                            case (int)Consts.SeikyusyoType.CalendarTate:    // カレンダー縦
                                if (cnt <= 1)
                                {
                                    // 請求先にぶら下がる得意先が自分しかいない場合（または売上を持つ得意先が全く無い場合）
                                    GrapeCity.ActiveReports.SectionReport rpt = PrintCalendarTateSingle(con, seikyuNo, seikyusakiName, courseCD, isGassan, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                    // ページ番号出力(縦)
                                    PrintPageTate(rpt);

                                    // 追加先のActiveReportsドキュメントにページを追加
                                    PrintMergeAdd(rpt);
                                }
                                else
                                {
                                    Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap = new Dictionary<int, KeigenZeiritsu>();    // 得意先ごとに、軽減税率算出用クラスを保持するマップ

                                    // 請求先にぶら下がる得意先が複数ある場合
                                    // 請求先にぶら下がる得意先が複数ある場合
                                    // Ｑ．なぜ親→子の順ではなく子→親の順に生成しているのか？
                                    // Ａ．子で集計した「税率ごとの対象金額の内訳」（軽減税率対応）を親で使用するため
                                    GrapeCity.ActiveReports.SectionReport rptChild = PrintCalendarTateChild(con, seikyuNo, seikyusakiName, courseCD, isGassan, keigenZeiritsuMap, seikyuCriteria, timeoutSec);     // 請求ヘッダと売上明細
                                    GrapeCity.ActiveReports.SectionReport rptParent = PrintDetailParent(con, seikyuNo, seikyusakiName, courseCD, isGassan, isPrintKaisyuYotei, keigenZeiritsuMap, seikyuCriteria, timeoutSec); // 請求合計と請求ヘッダ

                                    // ページ番号を出力
                                    PrintPageTate(rptParent, rptChild);

                                    // 追加先のActiveReportsドキュメントにページを追加
                                    PrintMergeAdd(rptParent);
                                    PrintMergeAdd(rptChild);
                                }
                                break;
                        }

                        string code = string.Format("請求番号=[{0}] 請求先CD=[{1}]", seikyuNo, seikyuCD);
                        History.Save(con, _ownerTitle, "請求書", code, History.ProcessType.Output);

                        // 請求合計の印刷者CDと印刷日時を更新する
                        string errMsg = UpdateSeikyuTotal(con, seikyuNo);
                        if (string.IsNullOrEmpty(errMsg) == false)
                        {
                            return errMsg;
                        }

                        progressState.Value++;
                        progressState.Message = string.Format("{0}/{1}", progressState.Value, progressState.Maximum);
                        p.Report(progressState);
                    }
                }

                if (string.IsNullOrEmpty(pdfPath))
                {
                    PrintMergeFinish();            // 帳票マージ終了・プレビュー表示
                }
                else
                {
                    PrintMergeFinishPdf(pdfPath, pdfFileName);  // 帳票マージ終了・PDF出力
                }

                return null;
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
        /// 請求書印刷処理（請求番号ごとにPDFを出力）
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="seikyuPrintTargetList">請求書印刷対象リスト</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="pdfPath">PDF保存先フォルダ</param>
        /// <param name="p">進行状況の更新のプロバイダー</param>
        /// <returns>成功した場合はnull。失敗した場合はエラーメッセージ</returns>
        public string PrintSeikyuDivide(int busyoCD, List<SeikyuPrintTarget> seikyuPrintTargetList,
                                         bool isPrintKaisyuYotei, SeikyuCriteria seikyuCriteria, string pdfPath, IProgress<BaseForm.ProgressState> p)
        {
            try
            {
                log.Debug("Start");
                // ロゴ・印鑑画像を取得
                BusyoImage busyoImage = BusyoImage.GetBusyoImage(busyoCD);
                _logoImageBytes = busyoImage.ロゴ;
                _inkanImageBytes = busyoImage.印影;

                int timeoutSec = SystemMaster.GetTimeoutSecForSeikyuPrint();                                // 請求書発行タイムアウト秒数を取得

                BaseForm.ProgressState progressState = new BaseForm.ProgressState();
                progressState.Style = ProgressBarStyle.Blocks;
                progressState.Maximum = seikyuPrintTargetList.Count;
                progressState.Value = 0;
                progressState.Message = string.Format("{0}/{1}", progressState.Value, progressState.Maximum);
                p.Report(progressState);

                try
                {
                    // ThreadLocalの.Valuesプロパティの参照を有効化する
                    // https://neue.cc/2013/03/09_400.html
                    using (var con = new ThreadLocal<SqlConnection>(() => { var conn = new SqlConnection(DBUtil.ConnectionString); conn.Open(); return conn; }, trackAllValues: true))
                    {
                        // 並列ループ
                        Parallel.ForEach(seikyuPrintTargetList, seikyuPrintTarget =>
                        {

                            Report000A report000A = new Report000A();
                            decimal seikyuNo = seikyuPrintTarget.請求番号;
                            int seikyuCD = seikyuPrintTarget.請求先ＣＤ;
                            string seikyusakiName = seikyuPrintTarget.請求先名;
                            int type = seikyuPrintTarget.請求書タイプ区分;
                            int? courseCD = seikyuPrintTarget.コースＣＤ;

                            // 帳票マージ開始
                            PrintMergeInit(report000A);

                            // 通常の請求書は請求番号ごとにPDFファイル名を生成
                            string pdfFileName = string.Format("{0:yyyyMMdd}_{1}_{2:F0}_請求書.pdf", seikyuPrintTarget.請求日, seikyuCD, seikyuPrintTarget.今回請求額);

                            // 請求先にぶら下がる得意先の数を取得
                            int cnt = GetTokuisakiCount(con.Value, seikyuNo);

                            // 請求書タイプで分岐
                            switch (type)
                            {
                                case (int)Consts.SeikyusyoType.Detail:          // 明細
                                    if (cnt <= 1)
                                    {
                                        // 請求先にぶら下がる得意先が自分しかいない場合（または売上を持つ得意先が全く無い場合）
                                        GrapeCity.ActiveReports.SectionReport rpt = PrintDetailSingle(con.Value, seikyuNo, seikyusakiName, courseCD, false, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                        // ページ番号出力(縦)
                                        PrintPageTate(rpt);

                                        // 追加先のActiveReportsドキュメントにページを追加
                                        PrintMergeAdd(report000A, rpt);
                                    }
                                    else
                                    {
                                        Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap = new Dictionary<int, KeigenZeiritsu>();    // 得意先ごとに、軽減税率算出用クラスを保持するマップ

                                        // 請求先にぶら下がる得意先が複数ある場合
                                        // Ｑ．なぜ親→子の順ではなく子→親の順に生成しているのか？
                                        // Ａ．子で集計した「税率ごとの対象金額の内訳」（軽減税率対応）を親で使用するため
                                        GrapeCity.ActiveReports.SectionReport rptChild = PrintDetailChild(con.Value, seikyuNo, seikyusakiName, courseCD, false, keigenZeiritsuMap, seikyuCriteria, timeoutSec);       // 請求ヘッダと売上明細
                                        GrapeCity.ActiveReports.SectionReport rptParent = PrintDetailParent(con.Value, seikyuNo, seikyusakiName, courseCD, false, isPrintKaisyuYotei, keigenZeiritsuMap, seikyuCriteria, timeoutSec); // 請求合計と請求ヘッダ

                                        // ページ番号を出力
                                        PrintPageTate(rptParent, rptChild);

                                        // 追加先のActiveReportsドキュメントにページを追加
                                        PrintMergeAdd(report000A, rptParent);
                                        PrintMergeAdd(report000A, rptChild);
                                    }
                                    break;

                                case (int)Consts.SeikyusyoType.CalendarYoko:    // カレンダー横
                                    if (cnt <= 1)
                                    {
                                        // 請求先にぶら下がる得意先が自分しかいない場合（または売上を持つ得意先が全く無い場合）
                                        GrapeCity.ActiveReports.SectionReport rpt = PrintCalendarYokoSingle(con.Value, seikyuNo, seikyusakiName, courseCD, false, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                        // 追加先のActiveReportsドキュメントにページを追加
                                        PrintMergeAdd(report000A, rpt);
                                    }
                                    else
                                    {
                                        // 請求先にぶら下がる得意先が複数ある場合
                                        GrapeCity.ActiveReports.SectionReport rpt = PrintCalendarYokoMulti(con.Value, seikyuNo, seikyusakiName, courseCD, false, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                        // 追加先のActiveReportsドキュメントにページを追加
                                        PrintMergeAdd(report000A, rpt);
                                    }
                                    break;

                                case (int)Consts.SeikyusyoType.CalendarTate:    // カレンダー縦
                                    if (cnt <= 1)
                                    {
                                        // 請求先にぶら下がる得意先が自分しかいない場合（または売上を持つ得意先が全く無い場合）
                                        GrapeCity.ActiveReports.SectionReport rpt = PrintCalendarTateSingle(con.Value, seikyuNo, seikyusakiName, courseCD, false, isPrintKaisyuYotei, seikyuCriteria, timeoutSec);

                                        // ページ番号出力(縦)
                                        PrintPageTate(rpt);

                                        // 追加先のActiveReportsドキュメントにページを追加
                                        PrintMergeAdd(report000A, rpt);
                                    }
                                    else
                                    {
                                        Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap = new Dictionary<int, KeigenZeiritsu>();    // 得意先ごとに、軽減税率算出用クラスを保持するマップ

                                        // 請求先にぶら下がる得意先が複数ある場合
                                        // Ｑ．なぜ親→子の順ではなく子→親の順に生成しているのか？
                                        // Ａ．子で集計した「税率ごとの対象金額の内訳」（軽減税率対応）を親で使用するため
                                        GrapeCity.ActiveReports.SectionReport rptChild = PrintCalendarTateChild(con.Value, seikyuNo, seikyusakiName, courseCD, false, keigenZeiritsuMap, seikyuCriteria, timeoutSec);     // 請求ヘッダと売上明細
                                        GrapeCity.ActiveReports.SectionReport rptParent = PrintDetailParent(con.Value, seikyuNo, seikyusakiName, courseCD, false, isPrintKaisyuYotei, keigenZeiritsuMap, seikyuCriteria, timeoutSec); // 請求合計と請求ヘッダ

                                        // ページ番号を出力
                                        PrintPageTate(rptParent, rptChild);

                                        // 追加先のActiveReportsドキュメントにページを追加
                                        PrintMergeAdd(report000A, rptParent);
                                        PrintMergeAdd(report000A, rptChild);
                                    }
                                    break;
                            }

                            string code = string.Format("請求番号=[{0}] 請求先CD=[{1}]", seikyuNo, seikyuCD);
                            History.Save(con.Value, _ownerTitle, "請求書", code, History.ProcessType.Output);

                            // 請求合計の印刷者CDと印刷日時を更新する
                            string errMsg = UpdateSeikyuTotal(con.Value, seikyuNo);
                            if (string.IsNullOrEmpty(errMsg) == false)
                            {
                                throw new Exception(errMsg);
                            }

                            PrintMergeFinishPdf(report000A, pdfPath, pdfFileName);  // 帳票マージ終了・PDF出力

                            lock (Thread.CurrentContext)
                            {
                                progressState.Value++;
                            }
                            progressState.Message = string.Format("{0}/{1}", progressState.Value, progressState.Maximum);
                            p.Report(progressState);
                        });

                        // 生成された全てのConnectionを一括Dispose
                        foreach (var item in con.Values.OfType<IDisposable>()) item.Dispose();
                    }
                }
                catch (AggregateException ex)
                {
                    log.Error(ex);
                    History.Error(ex);
                    throw new EmuException(ex);
                }

                log.Debug("Finish");
                return null;
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
        /// 請求先にぶら下がる得意先の数を返す
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <returns>請求先にぶら下がる得意先の数</returns>
        private int GetTokuisakiCount(SqlConnection con, decimal seikyuNo)
        {
            try
            {
                // 請求先にぶら下がる得意先の数を取得
                // 計算税率を無視したいので２段階で取得している
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     COUNT(A.得意先ＣＤ) AS CNT" +
                                  "\r\n FROM" +
                                  "\r\n     (SELECT DISTINCT" +
                                  "\r\n          得意先ＣＤ" +
                                  "\r\n      FROM" +
                                  "\r\n          請求ヘッダ AS H" +
                                  "\r\n      WHERE" +
                                  "\r\n          H.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n      AND H.請求番号 = @請求番号" +
                                  "\r\n      AND H.売上金額税込 <> 0" +
                                  "\r\n      ) AS A";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;

                log.Debug(cmd.CommandTextWithParameters());

                int cnt = 1;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    cnt = (int)reader["CNT"];
                }
                return cnt;
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
        /// 明細タイプの請求書出力（請求先にぶら下がる得意先が自分しかいない場合）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintDetailSingle(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                        int? courseCD, bool isGassan, bool isPrintKaisyuYotei,
                                                                        SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT");               // -- 明細
                sql.AppendLine("     @合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,G.請求番号");
                sql.AppendLine("     ,G.請求日");
                sql.AppendLine("     ,G.締区分");
                sql.AppendLine("     ,G.部署ＣＤ");
                sql.AppendLine("     ,G.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,G.前回請求額");
                sql.AppendLine("     ,G.入金額");
                sql.AppendLine("     ,G.繰越額");
                sql.AppendLine("     ,G.買上金額");
                sql.AppendLine("     ,G.調整額");
                sql.AppendLine("     ,G.消費税");
                sql.AppendLine("     ,G.今回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ前回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ入金額");
                sql.AppendLine("     ,'*****' AS 売上のみ繰越額");
                sql.AppendLine("     ,ISNULL(G.買上金額, 0) + ISNULL(G.調整額, 0) + ISNULL(G.消費税, 0) AS 売上のみ今回請求額");
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    sql.AppendLine(" ,NULL AS 回収予定日");
                }
                else
                {
                    sql.AppendLine(" ,G.回収予定日 AS 回収予定日");
                }
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(P.前回請求日, DATEADD(d, -1, P.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, P.前回請求日), P.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,G.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(G.請求日)");
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");              // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");    // -- 初期値は1:内税
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,H.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,URI.売上日");
                sql.AppendLine("     ,URI.納品書伝票番号 AS 伝票番号");
                sql.AppendLine("     ,URI.明細番号");
                sql.AppendLine("     ,URI.商品ＣＤ");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,URI.商品単価");
                sql.AppendLine("     ,URI.商品数量");
                sql.AppendLine("     ,URI.値引後金額");
                sql.AppendLine("     ,URI.計算税率");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("     FROM");
                sql.AppendLine("         請求合計 AS G1");
                sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("             AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("                 ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ AS OH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                     OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                     AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                     AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND OH.請求番号 = @請求番号");
                sql.AppendLine("                 AND OM.売上区分 = @売上区分売掛");
                sql.AppendLine("                 AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("             ) AS UM_OLD ON");
                sql.AppendLine("             UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("             AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND G1.請求番号 = @請求番号");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("     ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("     INNER JOIN 請求合計 G ON");
                sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = G.部署ＣＤ");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                sql.AppendLine("      LEFT JOIN ");
                sql.AppendLine("         (SELECT DISTINCT");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,部署ＣＤ");
                sql.AppendLine("             ,請求先ＣＤ");
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
                sql.AppendLine("             UM.売上区分 = @売上区分売掛");
                sql.AppendLine("             AND ISNULL(UM.商品数量, 0) <> 0");
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("             AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");        // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("         ) AS URI ON");
                sql.AppendLine("         URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("         AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("         AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("         AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("         AND URI.売上日 <= G.請求日");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = URI.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = URI.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     明細得意先ＣＤ");
                sql.AppendLine("     ,売上日");
                sql.AppendLine("     ,明細番号");
                sql.AppendLine("     ,商品ＣＤ");

                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = seikyuCriteria.今回売上のみを出力 ?? false;
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = isPrintKaisyuYotei;
                }
                else
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = true;
                }
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                if (courseCD != null)
                {
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = seikyuCriteria.振替口座印字フラグ ?? false;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                // 銀行名と支店名から作る金融機関名列を追加
                dt.Columns.Add("金融機関名称1", typeof(string));
                dt.Columns.Add("金融機関名称2", typeof(string));
                dt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                dt.Columns.Add("ロゴ画像", typeof(byte[]));
                dt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                dt.Columns.Add("消費税内訳", typeof(string));

                KeigenZeiritsu keigenZeiritsu = new KeigenZeiritsu(_isPrintMarkWhenAllReducedTaxRate);
                Syohizei syohizei = null;
                int? hasuKubun = null;

                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                    MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                    // 振替金融機関名称の末尾に振替口座番号を追加する
                    if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                    {
                        dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                    }

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = _logoImageBytes;
                    dr["印鑑画像"] = _inkanImageBytes;

                    //*************
                    // 軽減税率対応
                    //*************
                    // （得意先レコードでもなく小計／消費税レコードでもなく）明細レコードについて、明細得意先と税率ごとに値引後金額(=買上金額)の合計を集計
                    if (dr.IsNull("売上日") == false)
                    {
                        syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                        hasuKubun = dr["丸め処理区分"] as int?;

                        // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                        // 税率ごとの合計金額を初期化
                        keigenZeiritsu.InitKingaku(syohizei);

                        // 税率ごとの金額を加算
                        decimal taxRate = (decimal)dr["計算税率"];
                        keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値引後金額"]);

                        // 軽減税率対象の商品名に※を付ける。
                        // ただし、_isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                        // ここでは適用前／通常／軽減の件数を集計するのみ
                        keigenZeiritsu.CountKeigen(syohizei, taxRate);
                    }
                }

                decimal totalTax = keigenZeiritsu.GetTaxIncluded(hasuKubun);    // 内消費税額を算出

                // 再度全てのレコードに対して
                // ・内消費税額を出力
                // ・軽減税率対象の商品名に「※」を付加する
                // ・グループフッターに出力する内訳を出力
                foreach (DataRow dr in dt.Rows)
                {
                    // 税率ごとの合計消費税額を計算
                    keigenZeiritsu.CalcTaxAmount((int)dr["税区分"], hasuKubun);

                    // （得意先レコードでもなく小計／消費税レコードでもなく）明細レコードを対象にする
                    if (dr.IsNull("売上日") == false)
                    {
                        syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                        decimal taxRate = (decimal)dr["計算税率"];
                        if (syohizei.IsKeigen(taxRate))
                        {
                            dr["商品名"] += keigenZeiritsu.Mark;
                        }
                    }

                    // 内税であれば内消費税額を出力する
                    if ((int)dr["税区分"] == (int)Syohizei.ZeiKubun.Uchi)
                    {
                        dr["消費税"] = totalTax;
                    }

                    // 消費税内訳は全てのレコードに格納する
                    dr["消費税内訳"] = keigenZeiritsu.TaxDetail;
                }

                Report200TateA report200TateA = new Report200TateA();
                report200TateA.TopOffsetCM = _topOffset;
                report200TateA.LeftOffsetCM = _leftOffset;
                report200TateA.DataSource = dt;
                report200TateA.Document.Printer.DocumentName = string.Format("請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report200TateA.Run(false);

                log.InfoFormat("請求書（明細）（単独得意先）を印刷しました。請求番号={0}", seikyuNo);

                return report200TateA;
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
        /// 明細タイプの請求書出力（請求先にぶら下がる得意先が複数）（請求合計と請求ヘッダ）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="keigenZeiritsuMap">得意先ごとに、軽減税率算出用クラスを保持するマップ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintDetailParent(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                        int? courseCD, bool isGassan, bool isPrintKaisyuYotei,
                                                                        Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap,
                                                                        SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");               // -- 明細
                sql.AppendLine("     0 AS 合計区分");
                sql.AppendLine("     ,@合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,G.請求番号");
                sql.AppendLine("     ,G.請求日");
                sql.AppendLine("     ,G.締区分");
                sql.AppendLine("     ,G.部署ＣＤ");
                sql.AppendLine("     ,G.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,G.前回請求額");
                sql.AppendLine("     ,G.入金額");
                sql.AppendLine("     ,G.繰越額");
                sql.AppendLine("     ,G.買上金額");
                sql.AppendLine("     ,G.調整額");
                sql.AppendLine("     ,G.消費税");
                sql.AppendLine("     ,G.今回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ前回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ入金額");
                sql.AppendLine("     ,'*****' AS 売上のみ繰越額");
                sql.AppendLine("     ,ISNULL(G.買上金額, 0) + ISNULL(G.調整額, 0) + ISNULL(G.消費税, 0) AS 売上のみ今回請求額");
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    sql.AppendLine(" ,NULL AS 回収予定日");
                }
                else
                {
                    sql.AppendLine(" ,G.回収予定日 AS 回収予定日");
                }
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(P.前回請求日, DATEADD(d, -1, P.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, P.前回請求日), P.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,G.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(G.請求日)");
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");              // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,H.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,H.得意先名 AS 明細得意先名");
                sql.AppendLine("     ,H.売上金額 AS 明細売上金額");
                sql.AppendLine("     ,H.消費税 AS 明細消費税");
                sql.AppendLine("     ,H.売上金額税込 AS 明細売上金額税込");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("     FROM");
                sql.AppendLine("         請求合計 AS G1");
                sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("             AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("                 ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ AS OH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                     OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                     AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                     AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND OH.請求番号 = @請求番号");
                sql.AppendLine("                 AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                 AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("             ) AS UM_OLD ON");
                sql.AppendLine("             UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("             AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND G1.請求番号 = @請求番号");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("     ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("     INNER JOIN 請求合計 G ON");
                sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = G.部署ＣＤ");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）
                sql.AppendLine("      LEFT JOIN ");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,得意先名");
                sql.AppendLine("             ,請求先ＣＤ");
                sql.AppendLine("             ,SUM(売上金額)     AS 売上金額");
                sql.AppendLine("             ,SUM(消費税)       AS 消費税");
                sql.AppendLine("             ,SUM(売上金額税込) AS 売上金額税込");
                sql.AppendLine("         FROM");
                sql.AppendLine("             請求ヘッダ");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,得意先名");
                sql.AppendLine("             ,請求先ＣＤ");
                sql.AppendLine("         ) AS H ON");
                sql.AppendLine("         H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND H.請求番号 = G.請求番号");
                sql.AppendLine("         AND H.売上金額税込 <> 0");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" UNION");
                // -- 合計
                sql.AppendLine(" SELECT");
                sql.AppendLine("     1 AS 合計区分");
                sql.AppendLine("     ,@合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,G.請求番号");
                sql.AppendLine("     ,G.請求日");
                sql.AppendLine("     ,G.締区分");
                sql.AppendLine("     ,G.部署ＣＤ");
                sql.AppendLine("     ,G.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,G.前回請求額");
                sql.AppendLine("     ,G.入金額");
                sql.AppendLine("     ,G.繰越額");
                sql.AppendLine("     ,G.買上金額");
                sql.AppendLine("     ,G.調整額");
                sql.AppendLine("     ,G.消費税");
                sql.AppendLine("     ,G.今回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ前回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ入金額");
                sql.AppendLine("     ,'*****' AS 売上のみ繰越額");
                sql.AppendLine("     ,ISNULL(G.買上金額, 0) + ISNULL(G.調整額, 0) + ISNULL(G.消費税, 0) AS 売上のみ今回請求額");
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    sql.AppendLine(" ,NULL AS 回収予定日");
                }
                else
                {
                    sql.AppendLine(" ,G.回収予定日 AS 回収予定日");
                }
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(P.前回請求日, DATEADD(d, -1, P.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, P.前回請求日), P.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,G.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(G.請求日)");
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");              // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,NULL AS 明細得意先ＣＤ");
                sql.AppendLine("     ,'【  合　計  】' AS 明細得意先名");
                sql.AppendLine("     ,G.買上金額 AS 明細売上金額");
                sql.AppendLine("     ,G.消費税 AS 明細消費税");
                sql.AppendLine("     ,G.買上金額 + G.消費税 AS 明細売上金額税込");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("     FROM");
                sql.AppendLine("         請求合計 AS G1");
                sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("             AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("                 ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ AS OH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                     OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                     AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                     AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND OH.請求番号 = @請求番号");
                sql.AppendLine("                 AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                 AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("             ) AS UM_OLD ON");
                sql.AppendLine("             UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("             AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND G1.請求番号 = @請求番号");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("     ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("     INNER JOIN 請求合計 G ON");
                sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = G.部署ＣＤ");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）
                sql.AppendLine("      LEFT JOIN ");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,得意先名");
                sql.AppendLine("             ,請求先ＣＤ");
                sql.AppendLine("             ,SUM(売上金額)     AS 売上金額");
                sql.AppendLine("             ,SUM(消費税)       AS 消費税");
                sql.AppendLine("             ,SUM(売上金額税込) AS 売上金額税込");
                sql.AppendLine("         FROM");
                sql.AppendLine("             請求ヘッダ");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,得意先名");
                sql.AppendLine("             ,請求先ＣＤ");
                sql.AppendLine("         ) AS H ON");
                sql.AppendLine("         H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND H.請求番号 = G.請求番号");
                sql.AppendLine("         AND H.売上金額税込 <> 0");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     合計区分");
                sql.AppendLine("     ,明細得意先ＣＤ");

                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = seikyuCriteria.今回売上のみを出力 ?? false;
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = isPrintKaisyuYotei;
                }
                else
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = true;
                }
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                if (courseCD != null)
                {
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = seikyuCriteria.振替口座印字フラグ ?? false;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                // 銀行名と支店名から作る金融機関名列を追加
                dt.Columns.Add("金融機関名称1", typeof(string));
                dt.Columns.Add("金融機関名称2", typeof(string));
                dt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                dt.Columns.Add("ロゴ画像", typeof(byte[]));
                dt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                dt.Columns.Add("消費税内訳", typeof(string));

                // グループフッターに出力する内訳集計用マップ
                SortedList<decimal, decimal> kingakuMap = new SortedList<decimal, decimal>();
                int? hasuKubun = null;
                decimal totalTax;

                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                    MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                    // 振替金融機関名称の末尾に振替口座番号を追加する
                    if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                    {
                        dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                    }

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = _logoImageBytes;
                    dr["印鑑画像"] = _inkanImageBytes;

                    //*************
                    // 軽減税率対応
                    //*************
                    // 得意先に対する軽減税率算出用クラスを取得
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        KeigenZeiritsu keigenZeiritsu = keigenZeiritsuMap[(int)tokuisakiCD];
                        // 内税であれば明細消費税を算出する
                        if ((int)dr["税区分"] == (int)Syohizei.ZeiKubun.Uchi)
                        {
                            hasuKubun = dr["丸め処理区分"] as int?;
                            totalTax = keigenZeiritsu.GetTaxIncluded(hasuKubun);    // 内消費税額を算出
                            dr["明細消費税"] = totalTax;
                        }

                        // 内訳生成用に、税率ごとの金額を集計する
                        // 得意先ごとに算出するのはKeigenZeiritsuに実装されているが、ここで必要なのは全ての得意先の合計値。
                        foreach (decimal rate in keigenZeiritsu.KingakuMap.Keys)
                        {
                            if (kingakuMap.ContainsKey(rate) == false)
                            {
                                kingakuMap[rate] = 0m;
                            }
                            kingakuMap[rate] += keigenZeiritsu.KingakuMap[rate];
                        }
                    }
                }

                // グループフッターに出力する内訳を生成
                string detail = KeigenZeiritsu.GenerateDetail(kingakuMap);

                // 集計した内消費税額を算出
                totalTax = KeigenZeiritsu.GetTaxIncluded(kingakuMap, hasuKubun);

                // 再度全てのレコードに対して
                // ・内消費税額を出力
                // ・グループフッターに出力する内訳を出力
                foreach (DataRow dr in dt.Rows)
                {
                    if ((int)dr["税区分"] == (int)Syohizei.ZeiKubun.Uchi)
                    {
                        dr["消費税"] = totalTax;
                    }

                    dr["消費税内訳"] = detail;
                }

                Report201TateA report201TateA = new Report201TateA();
                report201TateA.TopOffsetCM = _topOffset;
                report201TateA.LeftOffsetCM = _leftOffset;
                report201TateA.DataSource = dt;
                report201TateA.Document.Printer.DocumentName = string.Format("一括請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report201TateA.Run(false);

                log.InfoFormat("一括請求書を印刷しました。請求番号={0}", seikyuNo);

                return report201TateA;
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
        /// 明細タイプの請求書出力（請求先にぶら下がる得意先が複数）（請求ヘッダと売上明細）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="keigenZeiritsuMap">得意先ごとに、軽減税率算出用クラスを保持するマップ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintDetailChild(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                       int? courseCD, bool isGassan,
                                                                       Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap, SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT");               // -- 得意先名
                sql.AppendLine("     0 AS 合計区分");
                sql.AppendLine("     ,0 AS 明細小計区分");
                sql.AppendLine("     ,@合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,G.請求番号");
                sql.AppendLine("     ,G.請求日");
                sql.AppendLine("     ,G.締区分");
                sql.AppendLine("     ,G.部署ＣＤ");
                sql.AppendLine("     ,G.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,NULL AS 前回請求額");
                sql.AppendLine("     ,NULL AS 入金額");
                sql.AppendLine("     ,NULL AS 繰越額");
                sql.AppendLine("     ,NULL AS 買上金額");
                sql.AppendLine("     ,NULL AS 調整額");
                sql.AppendLine("     ,NULL AS 消費税");
                sql.AppendLine("     ,NULL AS 今回請求額");
                sql.AppendLine("     ,NULL AS 売上のみ前回請求額");
                sql.AppendLine("     ,NULL AS 売上のみ入金額");
                sql.AppendLine("     ,NULL AS 売上のみ繰越額");
                sql.AppendLine("     ,NULL AS 売上のみ今回請求額");
                sql.AppendLine("     ,NULL AS 回収予定日");
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(P.前回請求日, DATEADD(d, -1, P.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, P.前回請求日), P.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,G.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(G.請求日)");
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");              // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,H.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,NULL AS 売上日");
                sql.AppendLine("     ,NULL AS 伝票番号");
                sql.AppendLine("     ,NULL AS 明細番号");
                sql.AppendLine("     ,NULL AS 商品ＣＤ");
                sql.AppendLine("     ,H.得意先名 AS 商品名");
                sql.AppendLine("     ,NULL AS 商品単価");
                sql.AppendLine("     ,NULL AS 商品数量");
                sql.AppendLine("     ,NULL AS 値引後金額");
                sql.AppendLine("     ,NULL AS 計算税率");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("     FROM");
                sql.AppendLine("         請求合計 AS G1");
                sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("             AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("                 ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ AS OH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                     OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                     AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                     AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND OH.請求番号 = @請求番号");
                sql.AppendLine("                 AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                 AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("             ) AS UM_OLD ON");
                sql.AppendLine("             UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("             AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND G1.請求番号 = @請求番号");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("     ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("     INNER JOIN 請求合計 G ON");
                sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = G.部署ＣＤ");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）
                sql.AppendLine("      LEFT JOIN ");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,得意先名");
                sql.AppendLine("             ,請求先ＣＤ");
                sql.AppendLine("             ,SUM(売上金額)     AS 売上金額");
                sql.AppendLine("             ,SUM(消費税)       AS 消費税");
                sql.AppendLine("             ,SUM(売上金額税込) AS 売上金額税込");
                sql.AppendLine("         FROM");
                sql.AppendLine("             請求ヘッダ");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             企業ＣＤ");
                sql.AppendLine("             ,請求番号");
                sql.AppendLine("             ,得意先ＣＤ");
                sql.AppendLine("             ,得意先名");
                sql.AppendLine("             ,請求先ＣＤ");
                sql.AppendLine("         ) AS H ON");
                sql.AppendLine("         H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND H.請求番号 = G.請求番号");
                sql.AppendLine("         AND H.売上金額税込 <> 0");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" UNION");
                // -- 明細
                sql.AppendLine(" SELECT");
                sql.AppendLine("     0 AS 合計区分");
                sql.AppendLine("     ,1 AS 明細小計区分");
                sql.AppendLine("     ,@合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,G.請求番号");
                sql.AppendLine("     ,G.請求日");
                sql.AppendLine("     ,G.締区分");
                sql.AppendLine("     ,G.部署ＣＤ");
                sql.AppendLine("     ,G.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,NULL AS 前回請求額");
                sql.AppendLine("     ,NULL AS 入金額");
                sql.AppendLine("     ,NULL AS 繰越額");
                sql.AppendLine("     ,NULL AS 買上金額");
                sql.AppendLine("     ,NULL AS 調整額");
                sql.AppendLine("     ,NULL AS 消費税");
                sql.AppendLine("     ,NULL AS 今回請求額");
                sql.AppendLine("     ,NULL AS 売上のみ前回請求額");
                sql.AppendLine("     ,NULL AS 売上のみ入金額");
                sql.AppendLine("     ,NULL AS 売上のみ繰越額");
                sql.AppendLine("     ,NULL AS 売上のみ今回請求額");
                sql.AppendLine("     ,NULL AS 回収予定日");
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(P.前回請求日, DATEADD(d, -1, P.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, P.前回請求日), P.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,G.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(G.請求日)");
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");              // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,H.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,URI.売上日");
                sql.AppendLine("     ,URI.納品書伝票番号 AS 伝票番号");
                sql.AppendLine("     ,URI.明細番号");
                sql.AppendLine("     ,URI.商品ＣＤ");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,URI.商品単価");
                sql.AppendLine("     ,URI.商品数量");
                sql.AppendLine("     ,URI.値引後金額");
                sql.AppendLine("     ,URI.計算税率");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("     FROM");
                sql.AppendLine("         請求合計 AS G1");
                sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("             AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("                 ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ AS OH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                     OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                     AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                     AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND OH.請求番号 = @請求番号");
                sql.AppendLine("                 AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                 AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 OH.企業ＣＤ");
                sql.AppendLine("                 ,OH.請求先ＣＤ");
                sql.AppendLine("             ) AS UM_OLD ON");
                sql.AppendLine("             UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("             AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND G1.請求番号 = @請求番号");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G1.企業ＣＤ");
                sql.AppendLine("         ,G1.請求番号");
                sql.AppendLine("         ,G1.請求先ＣＤ");
                sql.AppendLine("         ,UM_OLD.最古売上日");
                sql.AppendLine("     ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("     INNER JOIN 請求合計 G ON");
                sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = G.部署ＣＤ");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                sql.AppendLine("     LEFT JOIN 請求ヘッダ H ON");
                sql.AppendLine("         H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("         AND H.請求番号 = G.請求番号");
                sql.AppendLine("         AND H.売上金額税込 <> 0");
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
                sql.AppendLine("             UM.売上区分 = @売上区分売掛");
                sql.AppendLine("             AND ISNULL(UM.商品数量, 0) <> 0");
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("             AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");        // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("         ) AS URI ON");
                sql.AppendLine("         URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("         AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("         AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("         AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("         AND URI.売上日 <= G.請求日");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = URI.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = URI.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     合計区分");
                sql.AppendLine("     ,明細得意先ＣＤ");
                sql.AppendLine("     ,明細小計区分");
                sql.AppendLine("     ,売上日");
                sql.AppendLine("     ,明細番号");
                sql.AppendLine("     ,商品ＣＤ");

                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = false;   // false固定
                cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = false;   // false固定
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                if (courseCD != null)
                {
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = false;     // 一括請求書に出力しているので、ここでは出力しない

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());

                //cmd.CommandTimeout = 120;           // あじやでタイムアウトが発生しているので延ばす
                //cmd.CommandTimeout = 300;           // あじやでタイムアウトが発生しているのでさらに延ばす
                // cmd.CommandTimeout = 600;           // あじやでタイムアウトが発生しているのでさらに延ばす
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                // 銀行名と支店名から作る金融機関名列を追加
                dt.Columns.Add("金融機関名称1", typeof(string));
                dt.Columns.Add("金融機関名称2", typeof(string));
                dt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                dt.Columns.Add("ロゴ画像", typeof(byte[]));
                dt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                dt.Columns.Add("消費税内訳", typeof(string));

                Syohizei syohizei = null;

                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                    MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                    // 振替金融機関名称の末尾に振替口座番号を追加する
                    if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                    {
                        dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                    }

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = _logoImageBytes;
                    dr["印鑑画像"] = _inkanImageBytes;

                    //*************
                    // 軽減税率対応
                    //*************
                    // 得意先に対するマップを生成・取得
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        if (keigenZeiritsuMap.ContainsKey((int)tokuisakiCD) == false)
                        {
                            keigenZeiritsuMap[(int)tokuisakiCD] = new KeigenZeiritsu(_isPrintMarkWhenAllReducedTaxRate);
                        }
                        KeigenZeiritsu keigenZeiritsu = keigenZeiritsuMap[(int)tokuisakiCD];

                        // （得意先レコードでもなく小計／消費税レコードでもなく）明細レコードについて、明細得意先と税率ごとに値引後金額(=買上金額)の合計を集計
                        if (dr.IsNull("売上日") == false)
                        {
                            syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);

                            // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                            // 税率ごとの合計金額を初期化
                            keigenZeiritsu.InitKingaku(syohizei);

                            // 税率ごとの金額を加算
                            decimal taxRate = (decimal)dr["計算税率"];
                            keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値引後金額"]);

                            // 軽減税率対象の商品名に※を付ける。
                            // ただし、_isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                            // ここでは適用前／通常／軽減の件数を集計するのみ
                            keigenZeiritsu.CountKeigen(syohizei, taxRate);
                        }
                    }
                }

                // 再度全てのレコードに対して
                // ・軽減税率対象の商品名に「※」を付加する
                // ・グループフッターに出力する内訳を出力
                foreach (DataRow dr in dt.Rows)
                {
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        KeigenZeiritsu keigenZeiritsu = keigenZeiritsuMap[(int)tokuisakiCD];

                        // 税率ごとの合計消費税額を計算
                        keigenZeiritsu.CalcTaxAmount((int)dr["税区分"], dr["丸め処理区分"] as int?);

                        // （得意先レコードでもなく小計／消費税レコードでもなく）明細レコードを対象にする
                        if (dr.IsNull("売上日") == false)
                        {
                            syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                            decimal taxRate = (decimal)dr["計算税率"];
                            if (syohizei.IsKeigen(taxRate))
                            {
                                dr["商品名"] += keigenZeiritsu.Mark;
                            }
                        }

                        // 消費税内訳は全てのレコードに格納する
                        dr["消費税内訳"] = keigenZeiritsu.TaxDetail;
                    }
                }

                Report200TateA report200TateA = new Report200TateA();
                report200TateA.TopOffsetCM = _topOffset;
                report200TateA.LeftOffsetCM = _leftOffset;
                report200TateA.DataSource = dt;
                report200TateA.Document.Printer.DocumentName = string.Format("請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report200TateA.Run(false);

                log.InfoFormat("請求書（明細）（複数得意先）を印刷しました。請求番号={0}", seikyuNo);

                return report200TateA;
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
        /// カレンダー横タイプの請求書出力
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintCalendarYokoSingle(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                              int? courseCD, bool isGassan, bool isPrintKaisyuYotei,
                                                                              SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     @合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,A.請求番号");
                sql.AppendLine("     ,A.請求日");
                sql.AppendLine("     ,A.締区分");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,A.前回請求額");
                sql.AppendLine("     ,A.入金額");
                sql.AppendLine("     ,A.繰越額");
                sql.AppendLine("     ,A.買上金額");
                sql.AppendLine("     ,A.調整額");
                sql.AppendLine("     ,A.消費税");
                sql.AppendLine("     ,A.今回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ前回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ入金額");
                sql.AppendLine("     ,'*****' AS 売上のみ繰越額");
                sql.AppendLine("     ,ISNULL(A.買上金額, 0) + ISNULL(A.調整額, 0) + ISNULL(A.消費税, 0) AS 売上のみ今回請求額");
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    sql.AppendLine(" ,NULL AS 回収予定日");
                }
                else
                {
                    sql.AppendLine(" ,A.回収予定日 AS 回収予定日");
                }
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(A.前回請求日, DATEADD(d, -1, A.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, A.前回請求日), A.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,A.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(A.請求日)");      // -- 日締
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");         // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,A.商品ＣＤ");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,A.商品単価");
                sql.AppendLine("     ,A.売上日");
                sql.AppendLine("     ,A.値引後金額");
                sql.AppendLine("     ,A.計算税率");
                sql.AppendLine("     ,A.商品数量");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- 商品ごとに数量の合計を算出
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G.企業ＣＤ");
                sql.AppendLine("         ,G.請求番号");
                sql.AppendLine("         ,G.請求日");
                sql.AppendLine("         ,G.締区分");
                sql.AppendLine("         ,G.部署ＣＤ");
                sql.AppendLine("         ,G.請求先ＣＤ");
                sql.AppendLine("         ,G.請求先名");
                sql.AppendLine("         ,G.前回請求額");
                sql.AppendLine("         ,G.入金額");
                sql.AppendLine("         ,G.繰越額");
                sql.AppendLine("         ,G.買上金額");
                sql.AppendLine("         ,G.調整額");
                sql.AppendLine("         ,G.消費税");
                sql.AppendLine("         ,G.今回請求額");
                sql.AppendLine("         ,G.回収予定日");
                sql.AppendLine("         ,P.前回請求日");
                sql.AppendLine("         ,P.最古売上日");
                sql.AppendLine("         ,URI.商品ＣＤ");
                sql.AppendLine("         ,URI.商品単価");
                sql.AppendLine("         ,URI.売上日");
                sql.AppendLine("         ,SUM(URI.値引後金額) AS 値引後金額");
                sql.AppendLine("         ,URI.計算税率");
                sql.AppendLine("         ,SUM(URI.商品数量) AS 商品数量");
                sql.AppendLine("     FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             G1.企業ＣＤ");
                sql.AppendLine("             ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("             ,G1.請求先ＣＤ");
                sql.AppendLine("             ,UM_OLD.最古売上日");
                sql.AppendLine("             ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("         FROM");
                sql.AppendLine("             請求合計 AS G1");
                sql.AppendLine("             LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                 ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                 AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                 AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("             LEFT JOIN");
                sql.AppendLine("                 (SELECT");
                sql.AppendLine("                     OH.企業ＣＤ");
                sql.AppendLine("                     ,OH.請求先ＣＤ");
                sql.AppendLine("                     ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求ヘッダ AS OH");
                sql.AppendLine("                     INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                         OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                         AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                         AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                     AND OH.請求番号 = @請求番号");
                sql.AppendLine("                     AND OM.売上区分 = @売上区分売掛");
                sql.AppendLine("                     AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("                 GROUP BY");
                sql.AppendLine("                     OH.企業ＣＤ");
                sql.AppendLine("                     ,OH.請求先ＣＤ");
                sql.AppendLine("                 ) AS UM_OLD ON");
                sql.AppendLine("                 UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("                 AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND G1.請求番号 = @請求番号");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             G1.企業ＣＤ");
                sql.AppendLine("             ,G1.請求番号");
                sql.AppendLine("             ,G1.請求先ＣＤ");
                sql.AppendLine("             ,UM_OLD.最古売上日");
                sql.AppendLine("         ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("         INNER JOIN 請求合計 G ON");
                sql.AppendLine("             G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("             AND G.請求番号 = P.今回請求番号");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                // --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                  企業ＣＤ");
                sql.AppendLine("                  ,請求番号");
                sql.AppendLine("                  ,部署ＣＤ");
                sql.AppendLine("                  ,得意先ＣＤ");
                sql.AppendLine("                  ,請求先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 売上金額税込 <> 0");
                sql.AppendLine("             ) AS H ON");
                sql.AppendLine("             H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("             AND H.請求番号 = G.請求番号");
                // -- ④今回の請求に対する最新の売上
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.売上日");
                sql.AppendLine("                 ,UM.商品ＣＤ");
                sql.AppendLine("                 ,UM.商品単価");
                sql.AppendLine("                 ,ISNULL(SUM(UM.値引後金額), 0) AS 値引後金額");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("                 ,ISNULL(SUM(UM.商品数量), 0) AS 商品数量");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM ON");
                sql.AppendLine("                     UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UM.売上区分 = @売上区分売掛");
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("                 AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");        // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.売上日");
                sql.AppendLine("                 ,UM.商品ＣＤ");
                sql.AppendLine("                 ,UM.商品単価");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("             ) AS URI ON");
                sql.AppendLine("             URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("             AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("             AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("             AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("             AND URI.売上日 <= G.請求日");
                sql.AppendLine("             AND URI.商品数量 <> 0");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G.企業ＣＤ");
                sql.AppendLine("         ,G.請求番号");
                sql.AppendLine("         ,G.請求日");
                sql.AppendLine("         ,G.締区分");
                sql.AppendLine("         ,G.部署ＣＤ");
                sql.AppendLine("         ,G.請求先ＣＤ");
                sql.AppendLine("         ,G.請求先名");
                sql.AppendLine("         ,G.前回請求額");
                sql.AppendLine("         ,G.入金額");
                sql.AppendLine("         ,G.繰越額");
                sql.AppendLine("         ,G.買上金額");
                sql.AppendLine("         ,G.調整額");
                sql.AppendLine("         ,G.消費税");
                sql.AppendLine("         ,G.今回請求額");
                sql.AppendLine("         ,G.回収予定日");
                sql.AppendLine("         ,P.前回請求日");
                sql.AppendLine("         ,P.最古売上日");
                sql.AppendLine("         ,URI.商品ＣＤ");
                sql.AppendLine("         ,URI.商品単価");
                sql.AppendLine("         ,URI.売上日");
                sql.AppendLine("         ,URI.計算税率");
                sql.AppendLine("     ) AS A");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = A.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("     INNER JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     A.商品ＣＤ");

                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = seikyuCriteria.今回売上のみを出力 ?? false;
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = isPrintKaisyuYotei;
                }
                else
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = true;
                }
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = seikyuCriteria.振替口座印字フラグ ?? false;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                //*************
                // 軽減税率対応
                //*************
                KeigenZeiritsu keigenZeiritsu = new KeigenZeiritsu(_isPrintMarkWhenAllReducedTaxRate);
                Syohizei syohizei = null;
                int? hasuKubun = null;
                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // （得意先レコードでもなく小計／消費税レコードでもなく）明細レコードについて、明細得意先と税率ごとに値引後金額(=買上金額)の合計を集計
                    if (dr.IsNull("売上日") == false)
                    {
                        syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                        hasuKubun = dr["丸め処理区分"] as int?;

                        // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                        // 税率ごとの合計金額を初期化
                        keigenZeiritsu.InitKingaku(syohizei);

                        // 税率ごとの金額を加算
                        decimal taxRate = (decimal)dr["計算税率"];
                        keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値引後金額"]);

                        // 軽減税率対象の商品名に※を付ける。
                        // ただし、_isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                        // ここでは適用前／通常／軽減の件数を集計するのみ
                        keigenZeiritsu.CountKeigen(syohizei, taxRate);
                    }
                }

                decimal totalTax = keigenZeiritsu.GetTaxIncluded(hasuKubun);    // 内消費税額を算出

                // 再度全てのレコードに対して
                // ・内消費税額を出力
                // ・軽減税率対象の商品名に「※」を付加する
                foreach (DataRow dr in dt.Rows)
                {
                    // 税率ごとの合計消費税額を計算
                    keigenZeiritsu.CalcTaxAmount((int)dr["税区分"], hasuKubun);

                    // （得意先レコードでもなく小計／消費税レコードでもなく）明細レコードを対象にする
                    if (dr.IsNull("売上日") == false)
                    {
                        syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                        decimal taxRate = (decimal)dr["計算税率"];
                        if (syohizei.IsKeigen(taxRate))
                        {
                            dr["商品名"] += keigenZeiritsu.Mark;
                        }
                    }

                    // 内税であれば内消費税額を出力する
                    if ((int)dr["税区分"] == (int)Syohizei.ZeiKubun.Uchi)
                    {
                        dr["消費税"] = totalTax;
                    }
                }

                // 日付01～日付31と数量01～数量31の列を作成する。
                // 請求期間From～請求期間Toに該当する日付と商品数量をそれぞれ日付01～日付31と数量01～数量31に格納する。
                // ※請求期間From～請求期間Toが月を跨ぐ場合は、請求期間Toからの1ヶ月前(直近1ヶ月)の売上を表示する。
                // ※締め区分が月締以外の場合は、締め区分に応じた日数分を請求期間Toから戻った日付からの売上を表示する。
                // ※前回請求も売上も無い場合は前回請求日がnullなので、その場合は請求日の月初日とする
                DateTime dateTo = (DateTime)dt.Rows[0]["請求日"];
                DateTime dateFrom;
                DateTime? dateFrom_wk = dt.Rows[0]["前回請求日"] as DateTime?;
                if (dateFrom_wk == null)
                {
                    dateFrom = new DateTime(dateTo.Year, dateTo.Month, 1);  // 請求日の月初日
                }
                else
                {
                    dateFrom = ((DateTime)dateFrom_wk).AddDays(1);          // 前回請求日の翌日
                }

                // 請求期間From～請求期間Toが31日以下であれば、そのまま採用する。
                // 31を超える場合は、締区分を問わず、請求期間Toの翌日の1ヶ月前を請求期間Fromとする。
                TimeSpan ts = dateTo - dateFrom;
                if (ts.Days >= 31)  // 1日から31日だとその差は30
                {
                    dateFrom = dateTo.AddDays(1).AddMonths(-1);
                }

                // 帳票出力用のデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("カレンダー請求書");
                foreach (DataColumn column in dt.Columns)
                {
                    // 売上日、商品数量、値引後金額、計算税率は除く
                    if (column.ColumnName == "売上日" ||
                        column.ColumnName == "商品数量" ||
                        column.ColumnName == "値引後金額" ||
                        column.ColumnName == "計算税率")
                    {
                        continue;
                    }
                    cldt.AddFixedColumn(column);
                }
                // 銀行名と支店名から作る金融機関名列を追加
                cldt.Columns.Add("金融機関名称1", typeof(string));
                cldt.Columns.Add("金融機関名称2", typeof(string));
                cldt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                cldt.Columns.Add("ロゴ画像", typeof(byte[]));
                cldt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                cldt.Columns.Add("消費税内訳", typeof(string));

                // 31日間の日付列を作る
                for (int i = 1; i <= 31; i++)
                {
                    cldt.Columns.Add(string.Format("日付{0:00}", i), typeof(string));
                }

                // 31日間の数量の不定列を作るべく、dateFromから31日間のレコードを持ったデータテーブルを作る
                // 同時に合計列用の式も作成
                DataTable dtSuryo = new DataTable();
                dtSuryo.Columns.Add("数量列名", typeof(string));
                dtSuryo.Columns.Add("数量日付", typeof(DateTime));
                StringBuilder expression = new StringBuilder();
                string plus = "";
                for (int i = 1; i <= 31; i++)
                {
                    DataRow wkDr = dtSuryo.NewRow();
                    string columnName = string.Format("数量{0:00}", i);
                    wkDr["数量列名"] = columnName;
                    wkDr["数量日付"] = dateFrom.AddDays(i - 1);
                    dtSuryo.Rows.Add(wkDr);

                    // 合計列の式を作成
                    expression.AppendFormat("{0}ISNULL({1}, 0)", plus, columnName);
                    plus = " + ";
                }
                cldt.AddColumns(dtSuryo, "数量列名", "数量日付", typeof(decimal));

                DataColumn totalCuolumn = new DataColumn("数量合計", typeof(decimal), expression.ToString());
                cldt.Columns.Add(totalCuolumn);  // 合計列を追加

                // インポート
                cldt.Import(dt, "売上日", "商品数量");

                // 全ての行に対して…
                foreach (DataRow dr in cldt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                    MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                    // 振替金融機関名称の末尾に振替口座番号を追加する
                    if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                    {
                        dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                    }

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = _logoImageBytes;
                    dr["印鑑画像"] = _inkanImageBytes;

                    // 消費税内訳は全てのレコードに格納する
                    dr["消費税内訳"] = keigenZeiritsu.TaxDetail;

                    // 日付を埋める
                    for (int i = 1; i <= 31; i++)
                    {
                        string columnName = string.Format("日付{0:00}", i);
                        DateTime d = dateFrom.AddDays(i - 1);
                        if (d <= dateTo)
                        {
                            dr[columnName] = d.Day.ToString();
                        }
                    }
                }

                Report200YokoA report200YokoA = new Report200YokoA();
                report200YokoA.TopOffsetCM = _topOffset;
                report200YokoA.LeftOffsetCM = _leftOffset;
                report200YokoA.DataSource = cldt;
                report200YokoA.Document.Printer.DocumentName = string.Format("請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report200YokoA.Run(false);

                log.InfoFormat("請求書（カレンダー）を印刷しました。請求番号={0}", seikyuNo);

                return report200YokoA;
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
        /// カレンダー横タイプ（得意先ごと明細付き）の請求書出力
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintCalendarYokoMulti(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                             int? courseCD, bool isGassan, bool isPrintKaisyuYotei,
                                                                             SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     @合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,A.請求番号");
                sql.AppendLine("     ,A.請求日");
                sql.AppendLine("     ,A.締区分");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,A.前回請求額");
                sql.AppendLine("     ,A.入金額");
                sql.AppendLine("     ,A.繰越額");
                sql.AppendLine("     ,A.買上金額");
                sql.AppendLine("     ,A.調整額");
                sql.AppendLine("     ,A.消費税");
                sql.AppendLine("     ,A.今回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ前回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ入金額");
                sql.AppendLine("     ,'*****' AS 売上のみ繰越額");
                sql.AppendLine("     ,ISNULL(A.買上金額, 0) + ISNULL(A.調整額, 0) + ISNULL(A.消費税, 0) AS 売上のみ今回請求額");
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    sql.AppendLine(" ,NULL AS 回収予定日");
                }
                else
                {
                    sql.AppendLine(" ,A.回収予定日 AS 回収予定日");
                }
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(A.前回請求日, DATEADD(d, -1, A.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, A.前回請求日), A.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,A.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(A.請求日)");      // -- 日締
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");         // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,A.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,T2.得意先名 AS 明細得意先名");
                sql.AppendLine("     ,DATALENGTH(T2.得意先名) AS 明細得意先名長さ");
                sql.AppendLine("     ,A.商品ＣＤ");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,A.商品単価");
                sql.AppendLine("     ,A.計算税率");
                sql.AppendLine("     ,A.売上日");
                sql.AppendLine("     ,A.値");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                // -- 数量と商品金額合計税込の各合計をあとで算出するために、それぞれを別のレコードとして取得している。
                // -- 売上日のアンダーバー以降で区別している。
                // -- 数量:S 金額:K
                // -- ①商品ごとに数量の合計を算出
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         G.企業ＣＤ");
                sql.AppendLine("         ,G.請求番号");
                sql.AppendLine("         ,G.請求日");
                sql.AppendLine("         ,G.締区分");
                sql.AppendLine("         ,G.部署ＣＤ");
                sql.AppendLine("         ,G.請求先ＣＤ");
                sql.AppendLine("         ,G.請求先名");
                sql.AppendLine("         ,G.前回請求額");
                sql.AppendLine("         ,G.入金額");
                sql.AppendLine("         ,G.繰越額");
                sql.AppendLine("         ,G.買上金額");
                sql.AppendLine("         ,G.調整額");
                sql.AppendLine("         ,G.消費税");
                sql.AppendLine("         ,G.今回請求額");
                sql.AppendLine("         ,G.回収予定日");
                sql.AppendLine("         ,P.前回請求日");
                sql.AppendLine("         ,P.最古売上日");
                sql.AppendLine("         ,URI.得意先ＣＤ");
                sql.AppendLine("         ,URI.商品ＣＤ");
                sql.AppendLine("         ,URI.商品単価");
                sql.AppendLine("         ,URI.計算税率");
                sql.AppendLine("         ,convert(varchar, URI.売上日, 111) + '_S' AS 売上日");
                sql.AppendLine("         ,SUM(URI.商品数量) AS 値");
                sql.AppendLine("     FROM");
                // -- ①－１今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             G1.企業ＣＤ");
                sql.AppendLine("             ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("             ,G1.請求先ＣＤ");
                sql.AppendLine("             ,UM_OLD.最古売上日");
                sql.AppendLine("             ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("         FROM");
                sql.AppendLine("             請求合計 AS G1");
                sql.AppendLine("             LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                 ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                 AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                 AND P1.請求日 < G1.請求日");
                // -- ①－５前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("             LEFT JOIN");
                sql.AppendLine("                 (SELECT");
                sql.AppendLine("                      OH.企業ＣＤ");
                sql.AppendLine("                     ,OH.請求先ＣＤ");
                sql.AppendLine("                     ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求ヘッダ AS OH");
                sql.AppendLine("                     INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                         OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                         AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                         AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                     AND OH.請求番号 = @請求番号");
                sql.AppendLine("                     AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                     AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("                 GROUP BY");
                sql.AppendLine("                      OH.企業ＣＤ");
                sql.AppendLine("                     ,OH.請求先ＣＤ");
                sql.AppendLine("                 ) AS UM_OLD ON");
                sql.AppendLine("                 UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("                 AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND G1.請求番号 = @請求番号");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             G1.企業ＣＤ");
                sql.AppendLine("             ,G1.請求番号");
                sql.AppendLine("             ,G1.請求先ＣＤ");
                sql.AppendLine("             ,UM_OLD.最古売上日");
                sql.AppendLine("         ) AS P");
                // -- ①－２今回請求の詳細
                sql.AppendLine("         INNER JOIN 請求合計 G ON");
                sql.AppendLine("             G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("             AND G.請求番号 = P.今回請求番号");
                // -- ①－３今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                //          計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                  企業ＣＤ");
                sql.AppendLine("                  ,請求番号");
                sql.AppendLine("                  ,部署ＣＤ");
                sql.AppendLine("                  ,得意先ＣＤ");
                sql.AppendLine("                  ,請求先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 売上金額税込 <> 0");
                sql.AppendLine("             ) AS H ON");
                sql.AppendLine("             H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("             AND H.請求番号 = G.請求番号");
                // -- ①－４今回の請求に対する最新の売上
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.売上日");
                sql.AppendLine("                 ,UM.商品ＣＤ");
                sql.AppendLine("                 ,UM.商品単価");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("                 ,ISNULL(SUM(UM.商品数量), 0) AS 商品数量");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM ON");
                sql.AppendLine("                     UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UM.売上区分 = @売上区分売掛");
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("               AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");        // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.売上日");
                sql.AppendLine("                 ,UM.商品ＣＤ");
                sql.AppendLine("                 ,UM.商品単価");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("             ) AS URI ON");
                sql.AppendLine("             URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("             AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("             AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("             AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("             AND URI.売上日 <= G.請求日");
                sql.AppendLine("             AND URI.商品数量 <> 0");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G.企業ＣＤ");
                sql.AppendLine("         ,G.請求番号");
                sql.AppendLine("         ,G.請求日");
                sql.AppendLine("         ,G.締区分");
                sql.AppendLine("         ,G.部署ＣＤ");
                sql.AppendLine("         ,G.請求先ＣＤ");
                sql.AppendLine("         ,G.請求先名");
                sql.AppendLine("         ,G.前回請求額");
                sql.AppendLine("         ,G.入金額");
                sql.AppendLine("         ,G.繰越額");
                sql.AppendLine("         ,G.買上金額");
                sql.AppendLine("         ,G.調整額");
                sql.AppendLine("         ,G.消費税");
                sql.AppendLine("         ,G.今回請求額");
                sql.AppendLine("         ,G.回収予定日");
                sql.AppendLine("         ,P.前回請求日");
                sql.AppendLine("         ,P.最古売上日");
                sql.AppendLine("         ,URI.得意先ＣＤ");
                sql.AppendLine("         ,URI.商品ＣＤ");
                sql.AppendLine("         ,URI.商品単価");
                sql.AppendLine("         ,URI.計算税率");
                sql.AppendLine("         ,URI.売上日");
                sql.AppendLine("     UNION");
                // -- ②商品ごとに値引後金額（＝税抜き金額）の合計を算出
                sql.AppendLine("     SELECT");
                sql.AppendLine("         G.企業ＣＤ");
                sql.AppendLine("         ,G.請求番号");
                sql.AppendLine("         ,G.請求日");
                sql.AppendLine("         ,G.締区分");
                sql.AppendLine("         ,G.部署ＣＤ");
                sql.AppendLine("         ,G.請求先ＣＤ");
                sql.AppendLine("         ,G.請求先名");
                sql.AppendLine("         ,G.前回請求額");
                sql.AppendLine("         ,G.入金額");
                sql.AppendLine("         ,G.繰越額");
                sql.AppendLine("         ,G.買上金額");
                sql.AppendLine("         ,G.調整額");
                sql.AppendLine("         ,G.消費税");
                sql.AppendLine("         ,G.今回請求額");
                sql.AppendLine("         ,G.回収予定日");
                sql.AppendLine("         ,P.前回請求日");
                sql.AppendLine("         ,P.最古売上日");
                sql.AppendLine("         ,URI.得意先ＣＤ");
                sql.AppendLine("         ,URI.商品ＣＤ");
                sql.AppendLine("         ,URI.商品単価");
                sql.AppendLine("         ,URI.計算税率");
                sql.AppendLine("         ,convert(varchar, URI.売上日, 111) + '_K' AS 売上日");
                sql.AppendLine("         ,SUM(URI.値引後金額) AS 値");
                sql.AppendLine("     FROM");
                // -- ②－１今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             G1.企業ＣＤ");
                sql.AppendLine("             ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("             ,G1.請求先ＣＤ");
                sql.AppendLine("             ,UM_OLD.最古売上日");
                sql.AppendLine("             ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("         FROM");
                sql.AppendLine("             請求合計 AS G1");
                sql.AppendLine("             LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                 ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                 AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                 AND P1.請求日 < G1.請求日");
                // -- ②－５前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("             LEFT JOIN");
                sql.AppendLine("                 (SELECT");
                sql.AppendLine("                      OH.企業ＣＤ");
                sql.AppendLine("                     ,OH.請求先ＣＤ");
                sql.AppendLine("                     ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求ヘッダ AS OH");
                sql.AppendLine("                     INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                         OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                         AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                         AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                     AND OH.請求番号 = @請求番号");
                sql.AppendLine("                     AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                     AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("                 GROUP BY");
                sql.AppendLine("                      OH.企業ＣＤ");
                sql.AppendLine("                     ,OH.請求先ＣＤ");
                sql.AppendLine("                 ) AS UM_OLD ON");
                sql.AppendLine("                 UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("                 AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND G1.請求番号 = @請求番号");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             G1.企業ＣＤ");
                sql.AppendLine("             ,G1.請求番号");
                sql.AppendLine("             ,G1.請求先ＣＤ");
                sql.AppendLine("             ,UM_OLD.最古売上日");
                sql.AppendLine("         ) AS P");
                // -- ②－２今回請求の詳細
                sql.AppendLine("         INNER JOIN 請求合計 G ON");
                sql.AppendLine("             G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("             AND G.請求番号 = P.今回請求番号");
                // -- ②－３今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                //          計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                  企業ＣＤ");
                sql.AppendLine("                  ,請求番号");
                sql.AppendLine("                  ,部署ＣＤ");
                sql.AppendLine("                  ,得意先ＣＤ");
                sql.AppendLine("                  ,請求先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 売上金額税込 <> 0");
                sql.AppendLine("             ) AS H ON");
                sql.AppendLine("             H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("             AND H.請求番号 = G.請求番号");
                // -- ②－４今回の請求に対する最新の売上
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.売上日");
                sql.AppendLine("                 ,UM.商品ＣＤ");
                sql.AppendLine("                 ,UM.商品単価");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("                 ,ISNULL(SUM(UM.商品数量), 0) AS 商品数量");
                sql.AppendLine("                 ,ISNULL(SUM(UM.値引後金額), 0) AS 値引後金額");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM ON");
                sql.AppendLine("                     UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UM.売上区分 = @売上区分売掛");
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("               AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");        // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.売上日");
                sql.AppendLine("                 ,UM.商品ＣＤ");
                sql.AppendLine("                 ,UM.商品単価");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("             ) AS URI ON");
                sql.AppendLine("             URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("             AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("             AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("             AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("             AND URI.売上日 <= G.請求日");
                sql.AppendLine("             AND URI.商品数量 <> 0");
                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         G.企業ＣＤ");
                sql.AppendLine("         ,G.請求番号");
                sql.AppendLine("         ,G.請求日");
                sql.AppendLine("         ,G.締区分");
                sql.AppendLine("         ,G.部署ＣＤ");
                sql.AppendLine("         ,G.請求先ＣＤ");
                sql.AppendLine("         ,G.請求先名");
                sql.AppendLine("         ,G.前回請求額");
                sql.AppendLine("         ,G.入金額");
                sql.AppendLine("         ,G.繰越額");
                sql.AppendLine("         ,G.買上金額");
                sql.AppendLine("         ,G.調整額");
                sql.AppendLine("         ,G.消費税");
                sql.AppendLine("         ,G.今回請求額");
                sql.AppendLine("         ,G.回収予定日");
                sql.AppendLine("         ,P.前回請求日");
                sql.AppendLine("         ,P.最古売上日");
                sql.AppendLine("         ,URI.得意先ＣＤ");
                sql.AppendLine("         ,URI.商品ＣＤ");
                sql.AppendLine("         ,URI.商品単価");
                sql.AppendLine("         ,URI.計算税率");
                sql.AppendLine("         ,URI.売上日");
                sql.AppendLine("     ) AS A");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = A.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("     INNER JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T2");
                sql.AppendLine("         ON  T2.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T2.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     A.得意先ＣＤ");
                sql.AppendLine("     ,A.商品ＣＤ");

                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = seikyuCriteria.今回売上のみを出力 ?? false;
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = isPrintKaisyuYotei;
                }
                else
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = true;
                }
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = seikyuCriteria.振替口座印字フラグ ?? false;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());

                // cmd.CommandTimeout = 120;           // ひのこでタイムアウトが発生しているので延ばす
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                //*************
                // 軽減税率対応
                //*************
                KeigenZeiritsu keigenZeiritsu = new KeigenZeiritsu(_isPrintMarkWhenAllReducedTaxRate);
                Syohizei syohizei = null;
                int? hasuKubun = null;
                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    hasuKubun = dr["丸め処理区分"] as int?;
                    // 得意先に対するマップを生成・取得
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        // 数量と金額のレコードが交互に現れるので、金額のレコードのみを対象にする
                        string uriagebi = dr["売上日"].ToString();
                        if (uriagebi.EndsWith("_K"))
                        {
                            uriagebi = uriagebi.Replace("_K", "");
                            DateTime d = DateTime.Parse(uriagebi);
                            syohizei = Syohizei.GetSyohizei(d);

                            // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                            // 税率ごとの合計金額を初期化
                            keigenZeiritsu.InitKingaku(syohizei);

                            // 税率ごとの金額を加算
                            decimal taxRate = (decimal)dr["計算税率"];
                            keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値"]);

                            // 軽減税率対象の商品名に※を付ける。
                            // ただし、_isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                            // ここでは適用前／通常／軽減の件数を集計するのみ
                            keigenZeiritsu.CountKeigen(syohizei, taxRate);
                        }
                    }
                }

                // 集計した内消費税額を算出
                decimal totalTax = keigenZeiritsu.GetTaxIncluded(hasuKubun);

                // 再度全てのレコードに対して
                // ・内消費税額を出力
                // ・軽減税率対象の商品名に「※」を付加する
                foreach (DataRow dr in dt.Rows)
                {
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        // 税率ごとの合計消費税額を計算
                        keigenZeiritsu.CalcTaxAmount((int)dr["税区分"], hasuKubun);

                        // 数量と金額のレコードが交互に現れるが、全てのレコードを対象にする
                        string uriagebi = dr["売上日"].ToString();
                        if (uriagebi.EndsWith("_K") || uriagebi.EndsWith("_S"))
                        {
                            uriagebi = uriagebi.Replace("_K", "");
                            uriagebi = uriagebi.Replace("_S", "");
                            DateTime d = DateTime.Parse(uriagebi);
                            syohizei = Syohizei.GetSyohizei(d);
                            decimal taxRate = (decimal)dr["計算税率"];
                            if (syohizei.IsKeigen(taxRate))
                            {
                                dr["商品名"] += keigenZeiritsu.Mark;
                            }
                        }

                        // 内税であれば内消費税額を出力する
                        if ((int)dr["税区分"] == (int)Syohizei.ZeiKubun.Uchi)
                        {
                            dr["消費税"] = totalTax;
                        }
                    }
                }

                // 日付01～日付31と数量01～数量31と金額01～金額31の列を作成する。
                // 請求期間From～請求期間Toに該当する日付を日付01～日付31に、商品数量を数量01～数量31に、金額を金額01～金額31にそれぞれ格納する。
                // ※請求期間From～請求期間Toが月を跨ぐ場合は、請求期間Toからの1ヶ月前(直近1ヶ月)の売上を表示する。
                // ※締め区分が月締以外の場合は、締め区分に応じた日数分を請求期間Toから戻った日付からの売上を表示する。
                // ※前回請求も売上も無い場合は前回請求日がnullなので、その場合は請求日の月初日とする
                DateTime dateTo = (DateTime)dt.Rows[0]["請求日"];
                DateTime dateFrom;
                DateTime? dateFrom_wk = dt.Rows[0]["前回請求日"] as DateTime?;
                if (dateFrom_wk == null)
                {
                    dateFrom = new DateTime(dateTo.Year, dateTo.Month, 1);  // 請求日の月初日
                }
                else
                {
                    dateFrom = ((DateTime)dateFrom_wk).AddDays(1);          // 前回請求日の翌日
                }

                // 請求期間From～請求期間Toが31日以下であれば、そのまま採用する。
                // 31を超える場合は、締区分を問わず、請求期間Toの翌日の1ヶ月前を請求期間Fromとする。
                TimeSpan ts = dateTo - dateFrom;
                if (ts.Days >= 31)  // 1日から31日だとその差は30
                {
                    dateFrom = dateTo.AddDays(1).AddMonths(-1);
                }

                // 帳票出力用のデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("カレンダー請求書");
                foreach (DataColumn column in dt.Columns)
                {
                    // 売上日と値（商品数量と商品金額合計税込が格納されている）、計算税率は除く
                    if (column.ColumnName == "売上日" ||
                        column.ColumnName == "値" ||
                        column.ColumnName == "計算税率")
                    {
                        continue;
                    }
                    cldt.AddFixedColumn(column);
                }
                // 銀行名と支店名から作る金融機関名列を追加
                cldt.Columns.Add("金融機関名称1", typeof(string));
                cldt.Columns.Add("金融機関名称2", typeof(string));
                cldt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                cldt.Columns.Add("ロゴ画像", typeof(byte[]));
                cldt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                cldt.Columns.Add("消費税内訳", typeof(string));

                // 31日間の日付列を作る
                for (int i = 1; i <= 31; i++)
                {
                    cldt.Columns.Add(string.Format("日付{0:00}", i), typeof(string));
                }

                // 31日間の数量の不定列を作るべく、dateFromから31日間のレコードを持ったデータテーブルを作る
                // 同時に合計列と金額列の式も作成
                DataTable dtSuryo = new DataTable();
                dtSuryo.Columns.Add("数量金額列名", typeof(string));
                dtSuryo.Columns.Add("売上日", typeof(string));
                StringBuilder suryoTotalExpression = new StringBuilder();
                StringBuilder kingakuTotalExpression = new StringBuilder();
                string plus = "";
                for (int i = 1; i <= 31; i++)
                {
                    // 数量列用のレコードを作成
                    DataRow wkDr = dtSuryo.NewRow();
                    string columnName = string.Format("数量{0:00}", i);
                    wkDr["数量金額列名"] = columnName;
                    wkDr["売上日"] = string.Format("{0:yyyy/MM/dd}_S", dateFrom.AddDays(i - 1));
                    dtSuryo.Rows.Add(wkDr);

                    // 数量合計列の式を作成
                    suryoTotalExpression.AppendFormat("{0}ISNULL({1}, 0)", plus, columnName);

                    // 金額列用のレコードを作成
                    wkDr = dtSuryo.NewRow();
                    columnName = string.Format("金額{0:00}", i);
                    wkDr["数量金額列名"] = columnName;
                    wkDr["売上日"] = string.Format("{0:yyyy/MM/dd}_K", dateFrom.AddDays(i - 1));
                    dtSuryo.Rows.Add(wkDr);

                    // 金額合計列の式を作成
                    kingakuTotalExpression.AppendFormat("{0}ISNULL({1}, 0)", plus, columnName);

                    plus = " + ";
                }
                cldt.AddColumns(dtSuryo, "数量金額列名", "売上日", typeof(decimal));

                // 数量と金額の各合計列を追加
                cldt.Columns.Add(new DataColumn("数量合計", typeof(decimal), suryoTotalExpression.ToString()));
                cldt.Columns.Add(new DataColumn("金額合計", typeof(decimal), kingakuTotalExpression.ToString()));

                // インポート
                cldt.Import(dt, "売上日", "値");

                // 全ての行に対して…
                foreach (DataRow dr in cldt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                    MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                    // 振替金融機関名称の末尾に振替口座番号を追加する
                    if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                    {
                        dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                    }

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = _logoImageBytes;
                    dr["印鑑画像"] = _inkanImageBytes;

                    // 消費税内訳は全てのレコードに格納する
                    dr["消費税内訳"] = keigenZeiritsu.TaxDetail;

                    // 日付を埋める
                    for (int i = 1; i <= 31; i++)
                    {
                        string columnName = string.Format("日付{0:00}", i);
                        DateTime d = dateFrom.AddDays(i - 1);
                        if (d <= dateTo)
                        {
                            dr[columnName] = d.Day.ToString();
                        }
                    }
                }

                Report201YokoA report201YokoA = new Report201YokoA();
                report201YokoA.TopOffsetCM = _topOffset;
                report201YokoA.LeftOffsetCM = _leftOffset;
                report201YokoA.DataSource = cldt;
                report201YokoA.Document.Printer.DocumentName = string.Format("請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report201YokoA.Run(false);

                log.InfoFormat("請求書（カレンダー）（得意先ごと明細）を印刷しました。請求番号={0}", seikyuNo);

                return report201YokoA;
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
        /// カレンダー縦タイプの請求書出力（請求先にぶら下がる得意先が自分しかいない場合）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="isPrintKaisyuYotei">回収予定日印字フラグ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintCalendarTateSingle(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                              int? courseCD, bool isGassan, bool isPrintKaisyuYotei,
                                                                              SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                //*****************************
                // 日付範囲取得の基準日付を取得
                //*****************************
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     G.請求日" +
                                  "\r\n     ,ISNULL(P.前回請求日, '2000/01/01') AS 前回請求日" +
                                  "\r\n FROM" +
                                  // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                                  "\r\n     (SELECT" +
                                  "\r\n         G1.企業ＣＤ" +
                                  "\r\n         ,G1.請求番号 AS 今回請求番号" +
                                  "\r\n         ,G1.請求先ＣＤ" +
                                  "\r\n         ,MAX(P1.請求日) AS 前回請求日" +
                                  "\r\n     FROM" +
                                  "\r\n         請求合計 AS G1" +
                                  "\r\n         LEFT JOIN 請求合計 AS P1" +
                                  "\r\n             ON G1.企業ＣＤ = P1.企業ＣＤ" +
                                  "\r\n             AND G1.請求先ＣＤ = P1.請求先ＣＤ" +
                                  "\r\n             AND P1.請求日 < G1.請求日" +
                                  "\r\n     WHERE" +
                                  "\r\n         G1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n         AND G1.請求番号 = @請求番号" +
                                  "\r\n     GROUP BY" +
                                  "\r\n         G1.企業ＣＤ" +
                                  "\r\n         ,G1.請求番号" +
                                  "\r\n         ,G1.請求先ＣＤ" +
                                  "\r\n     ) AS P" +
                                  // -- ②今回請求の詳細
                                  "\r\n     INNER JOIN 請求合計 G ON" +
                                  "\r\n         G.企業ＣＤ = P.企業ＣＤ" +
                                  "\r\n         AND G.請求番号 = P.今回請求番号";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;

                log.Debug(cmd.CommandTextWithParameters());
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                // 売上明細の取得対象の日付範囲を求める。
                // ※請求期間From～請求期間Toが月を跨ぐ場合は、請求期間Toからの1ヶ月前(直近1ヶ月)の売上を表示する。
                // ※締め区分が月締以外の場合は、締め区分に応じた日数分を請求期間Toから戻った日付からの売上を表示する。
                // ※前回請求も売上も無い場合は前回請求日がnullなので、その場合は請求日の月初日とする
                DateTime dateTo;
                DateTime dateFrom;
                DateTime? dateFrom_wk;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    dateTo = (DateTime)reader["請求日"];
                    dateFrom_wk = reader["前回請求日"] as DateTime?;
                }
                if (dateFrom_wk == null)
                {
                    dateFrom = new DateTime(dateTo.Year, dateTo.Month, 1);  // 請求日の月初日
                }
                else
                {
                    dateFrom = ((DateTime)dateFrom_wk).AddDays(1);          // 前回請求日の翌日
                }

                // 請求期間From～請求期間Toが31日以下であれば、そのまま採用する。
                // 31を超える場合は、締区分を問わず、請求期間Toの翌日の1ヶ月前を請求期間Fromとする。
                TimeSpan ts = dateTo - dateFrom;
                if (ts.Days >= 31)  // 1日から31日だとその差は30
                {
                    dateFrom = dateTo.AddDays(1).AddMonths(-1);
                }

                //*****************************************
                // 日付範囲を指定して請求・売上データを取得
                //*****************************************
                cmd = con.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" WITH R AS");
                sql.AppendLine(" (SELECT");
                sql.AppendLine("     cast(@表示期間From AS Date ) AS 日付");
                sql.AppendLine(" UNION ALL");
                sql.AppendLine(" SELECT");
                sql.AppendLine("     DATEADD(day, 1, 日付 )");
                sql.AppendLine(" FROM");
                sql.AppendLine("     R");
                sql.AppendLine(" WHERE R.日付 < @表示期間To");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT");
                sql.AppendLine("     0 AS ページ番号");
                sql.AppendLine("     ,0 AS 明細小計区分");
                sql.AppendLine("     ,@合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,A.日付");
                sql.AppendLine("     ,RIGHT('00' + cast(month(A.日付) as varchar) ,2) + '/' +");
                sql.AppendLine("      RIGHT('00' + cast(day(A.日付) as varchar), 2) + '(' +");
                sql.AppendLine("      CASE DATEPART(WEEKDAY, A.日付)");
                sql.AppendLine("         WHEN 1 THEN '日'");
                sql.AppendLine("         WHEN 2 THEN '月'");
                sql.AppendLine("         WHEN 3 THEN '火'");
                sql.AppendLine("         WHEN 4 THEN '水'");
                sql.AppendLine("         WHEN 5 THEN '木'");
                sql.AppendLine("         WHEN 6 THEN '金'");
                sql.AppendLine("         WHEN 7 THEN '土'");
                sql.AppendLine("      END  + ')' AS 表示用日付");
                sql.AppendLine("     ,A.請求番号");
                sql.AppendLine("     ,A.請求日");
                sql.AppendLine("     ,A.締区分");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,A.前回請求額");
                sql.AppendLine("     ,A.入金額");
                sql.AppendLine("     ,A.繰越額");
                sql.AppendLine("     ,A.買上金額");
                sql.AppendLine("     ,A.調整額");
                sql.AppendLine("     ,A.消費税");
                sql.AppendLine("     ,A.今回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ前回請求額");
                sql.AppendLine("     ,'*****' AS 売上のみ入金額");
                sql.AppendLine("     ,'*****' AS 売上のみ繰越額");
                sql.AppendLine("     ,ISNULL(A.買上金額, 0) + ISNULL(A.調整額, 0) + ISNULL(A.消費税, 0) AS 売上のみ今回請求額");
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    sql.AppendLine(" ,NULL AS 回収予定日");
                }
                else
                {
                    sql.AppendLine(" ,A.回収予定日 AS 回収予定日");
                }
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(A.前回請求日, DATEADD(d, -1, A.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, A.前回請求日), A.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,A.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(A.請求日)");      // -- 日締
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");     // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,A.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,NULL AS 明細得意先名");
                sql.AppendLine("     ,A.商品ＣＤ");
                sql.AppendLine("     ,cast(A.商品ＣＤ as varchar) + '_' + cast(A.商品単価 as varchar) AS 商品キー");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,A.商品単価");
                sql.AppendLine("     ,A.売上日");
                sql.AppendLine("     ,A.値引後金額");
                sql.AppendLine("     ,A.計算税率");
                sql.AppendLine("     ,A.商品数量");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         X.日付");
                sql.AppendLine("         ,X.企業ＣＤ");
                sql.AppendLine("         ,X.請求番号");
                sql.AppendLine("         ,X.請求日");
                sql.AppendLine("         ,X.締区分");
                sql.AppendLine("         ,X.部署ＣＤ");
                sql.AppendLine("         ,X.請求先ＣＤ");
                sql.AppendLine("         ,X.請求先名");
                sql.AppendLine("         ,X.前回請求額");
                sql.AppendLine("         ,X.入金額");
                sql.AppendLine("         ,X.繰越額");
                sql.AppendLine("         ,X.買上金額");
                sql.AppendLine("         ,X.調整額");
                sql.AppendLine("         ,X.消費税");
                sql.AppendLine("         ,X.今回請求額");
                sql.AppendLine("         ,X.回収予定日");
                sql.AppendLine("         ,X.前回請求日");
                sql.AppendLine("         ,X.最古売上日");
                sql.AppendLine("         ,X.得意先ＣＤ");
                sql.AppendLine("         ,Y.商品ＣＤ");
                sql.AppendLine("         ,Y.商品単価");
                sql.AppendLine("         ,Y.売上日");
                sql.AppendLine("         ,Y.値引後金額");
                sql.AppendLine("         ,Y.計算税率");
                sql.AppendLine("         ,Y.商品数量");
                sql.AppendLine("     FROM");
                // -- 日付と請求合計をFULL JOINして取得
                sql.AppendLine("         (SELECT DISTINCT");
                sql.AppendLine("             R.日付");
                sql.AppendLine("             ,G.企業ＣＤ");
                sql.AppendLine("             ,G.請求番号");
                sql.AppendLine("             ,G.請求日");
                sql.AppendLine("             ,G.締区分");
                sql.AppendLine("             ,G.部署ＣＤ");
                sql.AppendLine("             ,G.請求先ＣＤ");
                sql.AppendLine("             ,G.請求先名");
                sql.AppendLine("             ,G.前回請求額");
                sql.AppendLine("             ,G.入金額");
                sql.AppendLine("             ,G.繰越額");
                sql.AppendLine("             ,G.買上金額");
                sql.AppendLine("             ,G.調整額");
                sql.AppendLine("             ,G.消費税");
                sql.AppendLine("             ,G.今回請求額");
                sql.AppendLine("             ,G.回収予定日");
                sql.AppendLine("             ,P.前回請求日");
                sql.AppendLine("             ,H.得意先ＣＤ");
                sql.AppendLine("             ,P.最古売上日");
                sql.AppendLine("         FROM");
                sql.AppendLine("             R,");      // -- 日付範囲を取得する再帰CTEを用いて、指定した範囲の日付を取得
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 G1.企業ＣＤ");
                sql.AppendLine("                 ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("                 ,G1.請求先ＣＤ");
                sql.AppendLine("                 ,UM_OLD.最古売上日");
                sql.AppendLine("                 ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求合計 AS G1");
                sql.AppendLine("                 LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                     ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                     AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                     AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("                 LEFT JOIN");
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         OH.企業ＣＤ");
                sql.AppendLine("                         ,OH.請求先ＣＤ");
                sql.AppendLine("                         ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS OH");
                sql.AppendLine("                         INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                             OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                             AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                             AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND OH.請求番号 = @請求番号");
                sql.AppendLine("                         AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                         AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         OH.企業ＣＤ");
                sql.AppendLine("                         ,OH.請求先ＣＤ");
                sql.AppendLine("                     ) AS UM_OLD ON");
                sql.AppendLine("                     UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("                     AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND G1.請求番号 = @請求番号");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 G1.企業ＣＤ");
                sql.AppendLine("                 ,G1.請求番号");
                sql.AppendLine("                 ,G1.請求先ＣＤ");
                sql.AppendLine("                 ,UM_OLD.最古売上日");
                sql.AppendLine("             ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("             INNER JOIN 請求合計 G ON");
                sql.AppendLine("                 G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("                 AND G.請求番号 = P.今回請求番号");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                // --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("             LEFT JOIN");
                sql.AppendLine("                 (SELECT DISTINCT");
                sql.AppendLine("                      企業ＣＤ");
                sql.AppendLine("                      ,請求番号");
                sql.AppendLine("                      ,部署ＣＤ");
                sql.AppendLine("                      ,得意先ＣＤ");
                sql.AppendLine("                      ,請求先ＣＤ");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求ヘッダ");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     売上金額税込 <> 0");
                sql.AppendLine("                 ) AS H ON");
                sql.AppendLine("                 H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("                 AND H.請求番号 = G.請求番号");
                sql.AppendLine("         ) AS X");
                // -- 売上
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 G.企業ＣＤ");
                sql.AppendLine("                 ,G.請求番号");
                sql.AppendLine("                 ,URI.得意先ＣＤ");
                sql.AppendLine("                 ,URI.商品ＣＤ");
                sql.AppendLine("                 ,URI.商品単価");
                sql.AppendLine("                 ,URI.売上日");
                sql.AppendLine("                 ,SUM(URI.値引後金額) AS 値引後金額");
                sql.AppendLine("                 ,URI.計算税率");
                sql.AppendLine("                 ,SUM(URI.商品数量) AS 商品数量");
                sql.AppendLine("             FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("                 (SELECT");
                sql.AppendLine("                     G1.企業ＣＤ");
                sql.AppendLine("                     ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("                     ,G1.請求先ＣＤ");
                sql.AppendLine("                     ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求合計 AS G1");
                sql.AppendLine("                     LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                         ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                         AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                         AND P1.請求日 < G1.請求日");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                     AND G1.請求番号 = @請求番号");
                sql.AppendLine("                 GROUP BY");
                sql.AppendLine("                     G1.企業ＣＤ");
                sql.AppendLine("                     ,G1.請求番号");
                sql.AppendLine("                     ,G1.請求先ＣＤ");
                sql.AppendLine("                 ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("                 INNER JOIN 請求合計 G ON");
                sql.AppendLine("                     G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("                     AND G.請求番号 = P.今回請求番号");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                // --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("                 LEFT JOIN");
                sql.AppendLine("                     (SELECT DISTINCT");
                sql.AppendLine("                          企業ＣＤ");
                sql.AppendLine("                          ,請求番号");
                sql.AppendLine("                          ,部署ＣＤ");
                sql.AppendLine("                          ,得意先ＣＤ");
                sql.AppendLine("                          ,請求先ＣＤ");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         売上金額税込 <> 0");
                sql.AppendLine("                     ) AS H ON");
                sql.AppendLine("                     H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("                     AND H.請求番号 = G.請求番号");
                // -- ④今回の請求に対する最新の売上
                sql.AppendLine("                 LEFT JOIN");
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         UH.企業ＣＤ");
                sql.AppendLine("                         ,UH.得意先ＣＤ");
                sql.AppendLine("                         ,UH.部署ＣＤ");
                sql.AppendLine("                         ,UH.売上日");
                sql.AppendLine("                         ,UM.商品ＣＤ");
                sql.AppendLine("                         ,UM.商品単価");
                sql.AppendLine("                         ,ISNULL(SUM(UM.値引後金額), 0) AS 値引後金額");
                sql.AppendLine("                         ,UM.計算税率");
                sql.AppendLine("                         ,ISNULL(SUM(UM.商品数量), 0) AS 商品数量");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         売上ヘッダ AS UH");
                sql.AppendLine("                         INNER JOIN 売上明細 AS UM ON");
                sql.AppendLine("                             UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                             AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                             AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                             AND UM.売上日 = UH.売上日");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         UM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("                         AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");      // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         UH.企業ＣＤ");
                sql.AppendLine("                         ,UH.得意先ＣＤ");
                sql.AppendLine("                         ,UH.部署ＣＤ");
                sql.AppendLine("                         ,UH.売上日");
                sql.AppendLine("                         ,UM.商品ＣＤ");
                sql.AppendLine("                         ,UM.商品単価");
                sql.AppendLine("                         ,UM.計算税率");
                sql.AppendLine("                     ) AS URI ON");
                sql.AppendLine("                         URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("                         AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("                         AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("                         AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("                         AND URI.売上日 <= G.請求日");
                sql.AppendLine("                         AND URI.商品数量 <> 0");
                sql.AppendLine("              GROUP BY");
                sql.AppendLine("                  G.企業ＣＤ");
                sql.AppendLine("                  ,G.請求番号");
                sql.AppendLine("                  ,URI.得意先ＣＤ");
                sql.AppendLine("                  ,URI.商品ＣＤ");
                sql.AppendLine("                  ,URI.商品単価");
                sql.AppendLine("                  ,URI.売上日");
                sql.AppendLine("                  ,URI.計算税率");
                sql.AppendLine("              ) AS Y ON");
                sql.AppendLine("                  X.企業ＣＤ = Y.企業ＣＤ");
                sql.AppendLine("                  AND X.請求番号 = Y.請求番号");
                sql.AppendLine("                  AND X.得意先ＣＤ = Y.得意先ＣＤ");
                sql.AppendLine("                  AND X.日付 = Y.売上日");
                sql.AppendLine("     ) AS A");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = A.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     A.日付");
                sql.AppendLine("     ,A.得意先ＣＤ");
                sql.AppendLine("     ,A.商品ＣＤ");

                cmd.Parameters.Add("@表示期間From", SqlDbType.Date).Value = dateFrom;
                cmd.Parameters.Add("@表示期間To", SqlDbType.Date).Value = dateTo;
                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = seikyuCriteria.今回売上のみを出力 ?? false;
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = isPrintKaisyuYotei;
                }
                else
                {
                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = true;
                }
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = seikyuCriteria.振替口座印字フラグ ?? false;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                //*************
                // 軽減税率対応
                //*************
                KeigenZeiritsu keigenZeiritsu = new KeigenZeiritsu(_isPrintMarkWhenAllReducedTaxRate);
                Syohizei syohizei = null;
                int? hasuKubun = null;
                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // 売上が存在する明細レコードについて、明細得意先と税率ごとに値引後金額(=買上金額)の合計を集計
                    if (dr.IsNull("売上日") == false)
                    {
                        syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                        hasuKubun = dr["丸め処理区分"] as int?;

                        // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                        // 税率ごとの合計金額を初期化
                        keigenZeiritsu.InitKingaku(syohizei);

                        // 税率ごとの金額を加算
                        decimal taxRate = (decimal)dr["計算税率"];
                        keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値引後金額"]);

                        // 軽減税率対象の商品名に※を付ける。
                        // ただし、_isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                        // ここでは適用前／通常／軽減の件数を集計するのみ
                        keigenZeiritsu.CountKeigen(syohizei, taxRate);
                    }
                }

                decimal totalTax = keigenZeiritsu.GetTaxIncluded(hasuKubun);    // 内消費税額を算出

                // 再度全てのレコードに対して
                // ・内消費税額を出力
                // ・軽減税率対象の商品名に「※」を付加する
                foreach (DataRow dr in dt.Rows)
                {
                    // 税率ごとの合計消費税額を計算
                    keigenZeiritsu.CalcTaxAmount((int)dr["税区分"], hasuKubun);

                    // 売上が存在する明細レコードを対象にする
                    if (dr.IsNull("売上日") == false)
                    {
                        syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                        decimal taxRate = (decimal)dr["計算税率"];
                        if (syohizei.IsKeigen(taxRate))
                        {
                            dr["商品名"] += keigenZeiritsu.Mark;
                            dr["商品キー"] += keigenZeiritsu.Mark;
                        }
                    }

                    // 内税であれば内消費税額を出力する
                    if ((int)dr["税区分"] == (int)Syohizei.ZeiKubun.Uchi)
                    {
                        dr["消費税"] = totalTax;
                    }
                }

                //*****************************************************************
                // 帳票の列として表現するための商品ＣＤ、商品名、商品単価を取得する
                //*****************************************************************
                DataView dv = new DataView(dt);         // 重複を除去するためのDataView
                DataTable syohinDt = dv.ToTable(true, "商品ＣＤ", "商品キー", "商品名", "商品単価");
                DataRow[] drs = syohinDt.Select("商品ＣＤ IS NOT NULL", "商品ＣＤ, 商品キー");      // ソート
                syohinDt = syohinDt.Clone();
                foreach (DataRow dr in drs)
                {
                    syohinDt.ImportRow(dr);
                }

                //*****************************************
                // 日付(行)×商品(列)のデータテーブルを作成
                //*****************************************
                ColumnListDataTable cldt = new ColumnListDataTable("カレンダー請求書");
                // 商品名、商品単価列を作成（不定列ではなく、普通の列として追加する）
                int i;
                for (i = 1; i <= syohinDt.Rows.Count; i++)
                {
                    cldt.Columns.Add(string.Format("商品名{0:00}", i), typeof(string));     // (☆)
                    cldt.Columns.Add(string.Format("商品単価{0:00}", i), typeof(decimal));  // (☆)
                }

                // 固定列を作成
                foreach (DataColumn column in dt.Columns)
                {
                    // 不定列として追加する列は除く
                    if (column.ColumnName == "商品ＣＤ" ||
                        column.ColumnName == "商品キー" ||
                        column.ColumnName == "商品名" ||
                        column.ColumnName == "商品単価" ||
                        column.ColumnName == "値引後金額" ||
                        column.ColumnName == "計算税率" ||
                        column.ColumnName == "商品数量")
                    {
                        continue;
                    }
                    cldt.AddFixedColumn(column);
                }

                // 銀行名と支店名から作る金融機関名列を追加
                cldt.Columns.Add("金融機関名称1", typeof(string));
                cldt.Columns.Add("金融機関名称2", typeof(string));
                cldt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                cldt.Columns.Add("ロゴ画像", typeof(byte[]));
                cldt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                cldt.Columns.Add("消費税内訳", typeof(string));

                // 不定列（を持つデータテーブル）を作成
                DataTable dtSuryo = new DataTable();
                dtSuryo.Columns.Add("数量列名", typeof(string));
                dtSuryo.Columns.Add("商品キー", typeof(string));
                // 取得した商品名の一覧からレコードを作成
                for (i = 1; i <= syohinDt.Rows.Count; i++)
                {
                    DataRow wkDr = dtSuryo.NewRow();
                    wkDr["数量列名"] = string.Format("数量{0:00}", i);  // (☆)
                    wkDr["商品キー"] = syohinDt.Rows[i - 1]["商品キー"];
                    dtSuryo.Rows.Add(wkDr);
                }
                // maxColumnsに満たない分を追加する
                //for (; i <= columns; i++)
                //{
                //    DataRow wkDr = dtSuryo.NewRow();
                //    wkDr["数量列名"] = string.Format("数量{0:00}", i);
                //    wkDr["商品キー"] = string.Format("blank{0:00}", i);
                //    dtSuryo.Rows.Add(wkDr);
                //}
                cldt.AddColumns(dtSuryo, "数量列名", "商品キー", typeof(decimal));

                // インポート
                cldt.Import(dt, "商品キー", "商品数量");

                // 全ての行に対して…
                foreach (DataRow dr in cldt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                    MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                    // 振替金融機関名称の末尾に振替口座番号を追加する
                    if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                    {
                        dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                    }

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = _logoImageBytes;
                    dr["印鑑画像"] = _inkanImageBytes;

                    // 消費税内訳は全てのレコードに格納する
                    dr["消費税内訳"] = keigenZeiritsu.TaxDetail;

                    // 商品名と商品単価を埋める
                    for (i = 1; i <= syohinDt.Rows.Count; i++)
                    {
                        string columnName = string.Format("商品名{0:00}", i);
                        dr[columnName] = syohinDt.Rows[i - 1]["商品名"];

                        columnName = string.Format("商品単価{0:00}", i);
                        dr[columnName] = syohinDt.Rows[i - 1]["商品単価"];
                    }
                }

                // 小計行を作成
                DataRow newRow = cldt.NewRow();
                newRow.ItemArray = cldt.Rows[0].ItemArray;  // 先頭行の内容をコピーする
                newRow["明細小計区分"] = 1;
                newRow["日付"] = DBNull.Value;
                newRow["表示用日付"] = "小計";
                for (i = 1; i <= syohinDt.Rows.Count; i++)
                {
                    string columnName = string.Format("数量{0:00}", i);
                    newRow[columnName] = cldt.Compute(string.Format("Sum({0})", columnName), null);
                }
                cldt.Rows.Add(newRow);

                // 商品数(=列数)がCALENDAR_TATE_COLUMNSを超えている場合は改ページする
                int totalPage = 0;
                DataTable printDt = CalendarTatePageFeed(cldt, syohinDt.Rows.Count, ref totalPage);

                Report202A report202A = new Report202A();
                report202A.TopOffsetCM = _topOffset;
                report202A.LeftOffsetCM = _leftOffset;
                report202A.DataSource = printDt;
                report202A.Document.Printer.DocumentName = string.Format("請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report202A.Run(false);

                log.InfoFormat("請求書（カレンダー縦）を印刷しました。請求番号={0}", seikyuNo);

                return report202A;
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
        /// カレンダー縦タイプの請求書出力（請求先にぶら下がる得意先が複数）（請求ヘッダと売上明細）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <param name="seikyusakiName">請求先名</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="isGassan">合算請求書フラグ</param>
        /// <param name="keigenZeiritsuMap">得意先ごとに、軽減税率算出用クラスを保持するマップ</param>
        /// <param name="seikyuCriteria">請求書出力条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>ActiveReportのPageオブジェクト</returns>
        private GrapeCity.ActiveReports.SectionReport PrintCalendarTateChild(SqlConnection con, decimal seikyuNo, string seikyusakiName,
                                                                            int? courseCD, bool isGassan,
                                                                            Dictionary<int, KeigenZeiritsu> keigenZeiritsuMap, SeikyuCriteria seikyuCriteria, int timeoutSec)
        {
            try
            {
                //*****************************
                // 日付範囲取得の基準日付を取得
                //*****************************
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     G.請求日" +
                                  "\r\n     ,ISNULL(P.前回請求日, '2000/01/01') AS 前回請求日" +
                                  "\r\n FROM" +
                                  // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                                  "\r\n     (SELECT" +
                                  "\r\n         G1.企業ＣＤ" +
                                  "\r\n         ,G1.請求番号 AS 今回請求番号" +
                                  "\r\n         ,G1.請求先ＣＤ" +
                                  "\r\n         ,MAX(P1.請求日) AS 前回請求日" +
                                  "\r\n     FROM" +
                                  "\r\n         請求合計 AS G1" +
                                  "\r\n         LEFT JOIN 請求合計 AS P1" +
                                  "\r\n             ON G1.企業ＣＤ = P1.企業ＣＤ" +
                                  "\r\n             AND G1.請求先ＣＤ = P1.請求先ＣＤ" +
                                  "\r\n             AND P1.請求日 < G1.請求日" +
                                  "\r\n     WHERE" +
                                  "\r\n         G1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n         AND G1.請求番号 = @請求番号" +
                                  "\r\n     GROUP BY" +
                                  "\r\n         G1.企業ＣＤ" +
                                  "\r\n         ,G1.請求番号" +
                                  "\r\n         ,G1.請求先ＣＤ" +
                                  "\r\n     ) AS P" +
                                  // -- ②今回請求の詳細
                                  "\r\n     INNER JOIN 請求合計 G ON" +
                                  "\r\n         G.企業ＣＤ = P.企業ＣＤ" +
                                  "\r\n         AND G.請求番号 = P.今回請求番号";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;

                log.Debug(cmd.CommandTextWithParameters());
                cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする

                // 売上明細の取得対象の日付範囲を求める。
                // ※請求期間From～請求期間Toが月を跨ぐ場合は、請求期間Toからの1ヶ月前(直近1ヶ月)の売上を表示する。
                // ※締め区分が月締以外の場合は、締め区分に応じた日数分を請求期間Toから戻った日付からの売上を表示する。
                // ※前回請求も売上も無い場合は前回請求日がnullなので、その場合は請求日の月初日とする
                DateTime dateTo;
                DateTime dateFrom;
                DateTime? dateFrom_wk;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    dateTo = (DateTime)reader["請求日"];
                    dateFrom_wk = reader["前回請求日"] as DateTime?;
                }
                if (dateFrom_wk == null)
                {
                    dateFrom = new DateTime(dateTo.Year, dateTo.Month, 1);  // 請求日の月初日
                }
                else
                {
                    dateFrom = ((DateTime)dateFrom_wk).AddDays(1);          // 前回請求日の翌日
                }

                // 請求期間From～請求期間Toが31日以下であれば、そのまま採用する。
                // 31を超える場合は、締区分を問わず、請求期間Toの翌日の1ヶ月前を請求期間Fromとする。
                TimeSpan ts = dateTo - dateFrom;
                if (ts.Days >= 31)  // 1日から31日だとその差は30
                {
                    dateFrom = dateTo.AddDays(1).AddMonths(-1);
                }

                //*****************************************
                // 日付範囲を指定して請求・売上データを取得
                //*****************************************
                cmd = con.CreateCommand();
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" WITH R AS");
                sql.AppendLine(" (SELECT");
                sql.AppendLine("     cast(@表示期間From AS Date ) AS 日付");
                sql.AppendLine(" UNION ALL");
                sql.AppendLine(" SELECT");
                sql.AppendLine("     DATEADD(day, 1, 日付 )");
                sql.AppendLine(" FROM");
                sql.AppendLine("     R");
                sql.AppendLine(" WHERE R.日付 < @表示期間To");
                sql.AppendLine(" )");
                sql.AppendLine(" SELECT");
                sql.AppendLine("     0 AS ページ番号");
                sql.AppendLine("     ,0 AS 明細小計区分");
                sql.AppendLine("     ,@合算請求フラグ AS 合算請求フラグ");
                sql.AppendLine("     ,@売上のみフラグ AS 売上のみフラグ");
                sql.AppendLine("     ,A.日付");
                sql.AppendLine("     ,RIGHT('00' + cast(month(A.日付) as varchar) ,2) + '/' +");
                sql.AppendLine("      RIGHT('00' + cast(day(A.日付) as varchar), 2) + '(' +");
                sql.AppendLine("      CASE DATEPART(WEEKDAY, A.日付)");
                sql.AppendLine("         WHEN 1 THEN '日'");
                sql.AppendLine("         WHEN 2 THEN '月'");
                sql.AppendLine("         WHEN 3 THEN '火'");
                sql.AppendLine("         WHEN 4 THEN '水'");
                sql.AppendLine("         WHEN 5 THEN '木'");
                sql.AppendLine("         WHEN 6 THEN '金'");
                sql.AppendLine("         WHEN 7 THEN '土'");
                sql.AppendLine("      END  + ')' AS 表示用日付");
                sql.AppendLine("     ,A.請求番号");
                sql.AppendLine("     ,A.請求日");
                sql.AppendLine("     ,A.締区分");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.請求先ＣＤ AS 得意先ＣＤ");
                sql.AppendLine("     ,A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(A.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ");
                sql.AppendLine("     ,NULL AS 前回請求額");
                sql.AppendLine("     ,NULL AS 入金額");
                sql.AppendLine("     ,NULL AS 繰越額");
                sql.AppendLine("     ,NULL AS 買上金額");
                sql.AppendLine("     ,NULL AS 調整額");
                sql.AppendLine("     ,NULL AS 消費税");
                sql.AppendLine("     ,NULL AS 今回請求額");
                sql.AppendLine("     ,NULL AS 回収予定日");
                sql.AppendLine("     ,NULL AS 売上のみ前回請求額");
                sql.AppendLine("     ,NULL AS 売上のみ入金額");
                sql.AppendLine("     ,NULL AS 売上のみ繰越額");
                sql.AppendLine("     ,NULL AS 売上のみ今回請求額");
                sql.AppendLine("     ,@回収予定日印字フラグ AS 回収予定日印字フラグ");
                sql.AppendLine("     ,ISNULL(A.前回請求日, DATEADD(d, -1, A.最古売上日)) AS 前回請求日");
                sql.AppendLine("     ,ISNULL(DATEADD(d, 1, A.前回請求日), A.最古売上日) AS 請求期間From");
                sql.AppendLine("     ,A.請求日 AS 請求期間To");
                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.電話番号1 AS 得意先電話番号");
                sql.AppendLine("     ,T.ＦＡＸ1 AS 得意先ＦＡＸ");
                sql.AppendLine("     ,T.締区分");
                sql.AppendLine("     ,CASE T.締区分");
                sql.AppendLine("         WHEN 1 THEN T.週締曜日");
                sql.AppendLine("         WHEN 2 THEN T.月締日");
                sql.AppendLine("         ELSE DAY(A.請求日)");      // -- 日締
                sql.AppendLine("      END AS 締日");
                sql.AppendLine("     ,T.支払サイト AS サイト");
                sql.AppendLine("     ,T.支払日");
                sql.AppendLine("     ,ISNULL(T.支払方法, -1) AS 支払方法");
                sql.AppendLine("     ,T.振込手数料フラグ");     // -- 負担をお願いする場合はfalse
                sql.AppendLine("     ,T.請求書郵便マークフラグ");
                sql.AppendLine("     ,ISNULL(T.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,ISNULL(T.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社ＦＡＸ");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,B.金融機関ＣＤ1");
                sql.AppendLine("     ,KKM1.銀行名 AS 銀行名1");
                sql.AppendLine("     ,B.金融機関支店ＣＤ1");
                sql.AppendLine("     ,KKSM1.支店名 AS 支店名1");
                sql.AppendLine("     ,B.口座種別1");
                sql.AppendLine("     ,VK1.口座種別名 AS 口座種別名1");
                sql.AppendLine("     ,B.口座番号1");
                sql.AppendLine("     ,B.口座名義人1");
                sql.AppendLine("     ,B.金融機関ＣＤ2");
                sql.AppendLine("     ,KKM2.銀行名 AS 銀行名2");
                sql.AppendLine("     ,B.金融機関支店ＣＤ2");
                sql.AppendLine("     ,KKSM2.支店名 AS 支店名2");
                sql.AppendLine("     ,B.口座種別2");
                sql.AppendLine("     ,VK2.口座種別名 AS 口座種別名2");
                sql.AppendLine("     ,B.口座番号2");
                sql.AppendLine("     ,B.口座名義人2");
                sql.AppendLine("     ,B.請求書備考1");
                sql.AppendLine("     ,B.請求書備考2");
                if (courseCD != null)
                {
                    sql.AppendLine("     ,@コースＣＤ AS コースＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                }
                else
                {
                    sql.AppendLine("     ,NULL AS コースＣＤ");
                }
                sql.AppendLine("     ,A.得意先ＣＤ AS 明細得意先ＣＤ");
                sql.AppendLine("     ,T2.得意先名 AS 明細得意先名");
                sql.AppendLine("     ,A.商品ＣＤ");
                sql.AppendLine("     ,cast(A.商品ＣＤ as varchar) + '_' + cast(A.商品単価 as varchar) AS 商品キー");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,A.商品単価");
                sql.AppendLine("     ,A.売上日");
                sql.AppendLine("     ,A.値引後金額");
                sql.AppendLine("     ,A.計算税率");
                sql.AppendLine("     ,A.商品数量");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,RIGHT('**********' + RIGHT(T.口座番号, 3), LEN(T.口座番号)) AS 振替口座番号");
                sql.AppendLine("     ,KKM3.銀行名 AS 振替銀行名");
                sql.AppendLine("     ,KKSM3.支店名 AS 振替支店名");
                sql.AppendLine("     ,@振替口座印字フラグ AS 振替口座印字フラグ");
                sql.AppendLine(" FROM");
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         X.日付");
                sql.AppendLine("         ,X.企業ＣＤ");
                sql.AppendLine("         ,X.請求番号");
                sql.AppendLine("         ,X.請求日");
                sql.AppendLine("         ,X.締区分");
                sql.AppendLine("         ,X.部署ＣＤ");
                sql.AppendLine("         ,X.請求先ＣＤ");
                sql.AppendLine("         ,X.請求先名");
                sql.AppendLine("         ,X.前回請求額");
                sql.AppendLine("         ,X.入金額");
                sql.AppendLine("         ,X.繰越額");
                sql.AppendLine("         ,X.買上金額");
                sql.AppendLine("         ,X.調整額");
                sql.AppendLine("         ,X.消費税");
                sql.AppendLine("         ,X.今回請求額");
                sql.AppendLine("         ,X.回収予定日");
                sql.AppendLine("         ,X.前回請求日");
                sql.AppendLine("         ,X.最古売上日");
                sql.AppendLine("         ,X.得意先ＣＤ");
                sql.AppendLine("         ,Y.商品ＣＤ");
                sql.AppendLine("         ,Y.商品単価");
                sql.AppendLine("         ,Y.売上日");
                sql.AppendLine("         ,Y.値引後金額");
                sql.AppendLine("         ,Y.計算税率");
                sql.AppendLine("         ,Y.商品数量");
                sql.AppendLine("     FROM");
                // -- 日付と請求合計をFULL JOINして取得
                sql.AppendLine("         (SELECT DISTINCT");
                sql.AppendLine("             R.日付");
                sql.AppendLine("             ,G.企業ＣＤ");
                sql.AppendLine("             ,G.請求番号");
                sql.AppendLine("             ,G.請求日");
                sql.AppendLine("             ,G.締区分");
                sql.AppendLine("             ,G.部署ＣＤ");
                sql.AppendLine("             ,G.請求先ＣＤ");
                sql.AppendLine("             ,G.請求先名");
                sql.AppendLine("             ,G.前回請求額");
                sql.AppendLine("             ,G.入金額");
                sql.AppendLine("             ,G.繰越額");
                sql.AppendLine("             ,G.買上金額");
                sql.AppendLine("             ,G.調整額");
                sql.AppendLine("             ,G.消費税");
                sql.AppendLine("             ,G.今回請求額");
                sql.AppendLine("             ,G.回収予定日");
                sql.AppendLine("             ,P.前回請求日");
                sql.AppendLine("             ,H.得意先ＣＤ");
                sql.AppendLine("             ,P.最古売上日");
                sql.AppendLine("         FROM");
                sql.AppendLine("             R,");      // -- 日付範囲を取得する再帰CTEを用いて、指定した範囲の日付を取得
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 G1.企業ＣＤ");
                sql.AppendLine("                 ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("                 ,G1.請求先ＣＤ");
                sql.AppendLine("                 ,UM_OLD.最古売上日");
                sql.AppendLine("                 ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求合計 AS G1");
                sql.AppendLine("                 LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                     ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                     AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                     AND P1.請求日 < G1.請求日");
                // -- ⑤前回請求日が無い場合の、最古の売上日（全てのレコードにおいて同じ値になるので、駆動表に含めることで高速化を図る）
                sql.AppendLine("                 LEFT JOIN");
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         OH.企業ＣＤ");
                sql.AppendLine("                         ,OH.請求先ＣＤ");
                sql.AppendLine("                         ,MIN(OM.売上日) AS 最古売上日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS OH");
                sql.AppendLine("                         INNER JOIN 売上明細 AS OM ON");
                sql.AppendLine("                             OH.企業ＣＤ = OM.企業ＣＤ");
                sql.AppendLine("                             AND OH.得意先ＣＤ = OM.得意先ＣＤ");
                sql.AppendLine("                             AND OH.部署ＣＤ = OM.部署ＣＤ");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         OH.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND OH.請求番号 = @請求番号");
                sql.AppendLine("                         AND OM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                sql.AppendLine("                         AND ISNULL(OM.商品数量, 0) <> 0");
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         OH.企業ＣＤ");
                sql.AppendLine("                         ,OH.請求先ＣＤ");
                sql.AppendLine("                     ) AS UM_OLD ON");
                sql.AppendLine("                     UM_OLD.企業ＣＤ = G1.企業ＣＤ");
                sql.AppendLine("                     AND UM_OLD.請求先ＣＤ = G1.請求先ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND G1.請求番号 = @請求番号");
                sql.AppendLine("             GROUP BY");
                sql.AppendLine("                 G1.企業ＣＤ");
                sql.AppendLine("                 ,G1.請求番号");
                sql.AppendLine("                 ,G1.請求先ＣＤ");
                sql.AppendLine("                 ,UM_OLD.最古売上日");
                sql.AppendLine("             ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("             INNER JOIN 請求合計 G ON");
                sql.AppendLine("                 G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("                 AND G.請求番号 = P.今回請求番号");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                // --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("             LEFT JOIN");
                sql.AppendLine("                 (SELECT DISTINCT");
                sql.AppendLine("                      企業ＣＤ");
                sql.AppendLine("                      ,請求番号");
                sql.AppendLine("                      ,部署ＣＤ");
                sql.AppendLine("                      ,得意先ＣＤ");
                sql.AppendLine("                      ,請求先ＣＤ");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求ヘッダ");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     売上金額税込 <> 0");
                sql.AppendLine("                 ) AS H ON");
                sql.AppendLine("                 H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("                 AND H.請求番号 = G.請求番号");
                sql.AppendLine("         ) AS X");
                // -- 売上
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 G.企業ＣＤ");
                sql.AppendLine("                 ,G.請求番号");
                sql.AppendLine("                 ,URI.得意先ＣＤ");
                sql.AppendLine("                 ,URI.商品ＣＤ");
                sql.AppendLine("                 ,URI.商品単価");
                sql.AppendLine("                 ,URI.売上日");
                sql.AppendLine("                 ,SUM(URI.値引後金額) AS 値引後金額");
                sql.AppendLine("                 ,URI.計算税率");
                sql.AppendLine("                 ,SUM(URI.商品数量) AS 商品数量");
                sql.AppendLine("             FROM");
                // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                sql.AppendLine("                 (SELECT");
                sql.AppendLine("                     G1.企業ＣＤ");
                sql.AppendLine("                     ,G1.請求番号 AS 今回請求番号");
                sql.AppendLine("                     ,G1.請求先ＣＤ");
                sql.AppendLine("                     ,MAX(P1.請求日) AS 前回請求日");
                sql.AppendLine("                 FROM");
                sql.AppendLine("                     請求合計 AS G1");
                sql.AppendLine("                     LEFT JOIN 請求合計 AS P1");
                sql.AppendLine("                         ON G1.企業ＣＤ = P1.企業ＣＤ");
                sql.AppendLine("                         AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                sql.AppendLine("                         AND P1.請求日 < G1.請求日");
                sql.AppendLine("                 WHERE");
                sql.AppendLine("                     G1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                     AND G1.請求番号 = @請求番号");
                sql.AppendLine("                 GROUP BY");
                sql.AppendLine("                     G1.企業ＣＤ");
                sql.AppendLine("                     ,G1.請求番号");
                sql.AppendLine("                     ,G1.請求先ＣＤ");
                sql.AppendLine("                 ) AS P");
                // -- ②今回請求の詳細
                sql.AppendLine("                 INNER JOIN 請求合計 G ON");
                sql.AppendLine("                     G.企業ＣＤ = P.企業ＣＤ");
                sql.AppendLine("                     AND G.請求番号 = P.今回請求番号");
                // -- ③今回の請求ヘッダ（売上がある請求ヘッダのみ取得する）（部署ごとの最新の売上を取得する必要があるので合計にはしない）
                // --   計算税率ごとにレコードがあるのでそれを同一視する（計算税率ごとのレコードを取得しない）
                sql.AppendLine("                 LEFT JOIN");
                sql.AppendLine("                     (SELECT DISTINCT");
                sql.AppendLine("                          企業ＣＤ");
                sql.AppendLine("                          ,請求番号");
                sql.AppendLine("                          ,部署ＣＤ");
                sql.AppendLine("                          ,得意先ＣＤ");
                sql.AppendLine("                          ,請求先ＣＤ");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         売上金額税込 <> 0");
                sql.AppendLine("                     ) AS H ON");
                sql.AppendLine("                     H.企業ＣＤ = G.企業ＣＤ");
                sql.AppendLine("                     AND H.請求番号 = G.請求番号");
                // -- ④今回の請求に対する最新の売上
                sql.AppendLine("                 LEFT JOIN");
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         UH.企業ＣＤ");
                sql.AppendLine("                         ,UH.得意先ＣＤ");
                sql.AppendLine("                         ,UH.部署ＣＤ");
                sql.AppendLine("                         ,UH.売上日");
                sql.AppendLine("                         ,UM.商品ＣＤ");
                sql.AppendLine("                         ,UM.商品単価");
                sql.AppendLine("                         ,ISNULL(SUM(UM.値引後金額), 0) AS 値引後金額");
                sql.AppendLine("                         ,UM.計算税率");
                sql.AppendLine("                         ,ISNULL(SUM(UM.商品数量), 0) AS 商品数量");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         売上ヘッダ AS UH");
                sql.AppendLine("                         INNER JOIN 売上明細 AS UM ON");
                sql.AppendLine("                             UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                             AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                             AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                             AND UM.売上日 = UH.売上日");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         UM.売上区分 = @売上区分売掛");     // -- 売掛のみ
                if (seikyuCriteria.単価ゼロフラグ != true)
                {
                    sql.AppendLine("                         AND (ISNULL(UM.商品単価, 0) <> 0 OR ISNULL(UM.値引額, 0) <> 0)");      // -- 単価が0でも、値引が0でなければ出力する
                }
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         UH.企業ＣＤ");
                sql.AppendLine("                         ,UH.得意先ＣＤ");
                sql.AppendLine("                         ,UH.部署ＣＤ");
                sql.AppendLine("                         ,UH.売上日");
                sql.AppendLine("                         ,UM.商品ＣＤ");
                sql.AppendLine("                         ,UM.商品単価");
                sql.AppendLine("                         ,UM.計算税率");
                sql.AppendLine("                     ) AS URI ON");
                sql.AppendLine("                         URI.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("                         AND URI.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine("                         AND URI.部署ＣＤ = H.部署ＣＤ");
                sql.AppendLine("                         AND URI.売上日 > ISNULL(P.前回請求日, '2000/01/01')");
                sql.AppendLine("                         AND URI.売上日 <= G.請求日");
                sql.AppendLine("                         AND URI.商品数量 <> 0");
                sql.AppendLine("              GROUP BY");
                sql.AppendLine("                  G.企業ＣＤ");
                sql.AppendLine("                  ,G.請求番号");
                sql.AppendLine("                  ,URI.得意先ＣＤ");
                sql.AppendLine("                  ,URI.商品ＣＤ");
                sql.AppendLine("                  ,URI.商品単価");
                sql.AppendLine("                  ,URI.売上日");
                sql.AppendLine("                  ,URI.計算税率");
                sql.AppendLine("              ) AS Y ON");
                sql.AppendLine("                  X.企業ＣＤ = Y.企業ＣＤ");
                sql.AppendLine("                  AND X.請求番号 = Y.請求番号");
                sql.AppendLine("                  AND X.得意先ＣＤ = Y.得意先ＣＤ");
                sql.AppendLine("                  AND X.日付 = Y.売上日");
                sql.AppendLine("     ) AS A");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = A.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON  T.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON");
                sql.AppendLine("         B.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS ON");
                sql.AppendLine("         VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM1 ON");
                sql.AppendLine("         KKM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM1 ON");
                sql.AppendLine("         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1");
                sql.AppendLine("         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK1 ON");
                sql.AppendLine("         VK1.口座種別CD = B.口座種別1");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM2 ON");
                sql.AppendLine("         KKM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM2 ON");
                sql.AppendLine("         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2");
                sql.AppendLine("         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2");
                sql.AppendLine("     LEFT JOIN V_口座種別 AS VK2 ON");
                sql.AppendLine("         VK2.口座種別CD = B.口座種別2");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T2");
                sql.AppendLine("         ON  T2.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T2.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関名称 AS KKM3 ON");
                sql.AppendLine("         KKM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS KKSM3 ON");
                sql.AppendLine("         KKSM3.銀行ＣＤ = T.金融機関ＣＤ");
                sql.AppendLine("         AND KKSM3.支店ＣＤ = T.金融機関支店ＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     A.得意先ＣＤ");
                sql.AppendLine("     ,A.日付");
                sql.AppendLine("     ,A.商品ＣＤ");

                cmd.Parameters.Add("@表示期間From", SqlDbType.Date).Value = dateFrom;
                cmd.Parameters.Add("@表示期間To", SqlDbType.Date).Value = dateTo;
                cmd.Parameters.Add("@合算請求フラグ", SqlDbType.Bit).Value = isGassan;
                cmd.Parameters.Add("@売上のみフラグ", SqlDbType.Bit).Value = false;         // false固定
                cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = false;   // false固定
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = seikyuNo;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = _isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@振替口座印字フラグ", SqlDbType.Bit).Value = false;     // 一括請求書に出力しているので、ここでは出力しない

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());
                // cmd.CommandTimeout = 600;       // 請求書発行は一律にタイムアウト時間を600秒にする
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("請求書");
                adp.Fill(dt);

                Syohizei syohizei = null;

                //*************
                // 軽減税率対応
                //*************
                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // 得意先に対するマップを生成・取得
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        if (keigenZeiritsuMap.ContainsKey((int)tokuisakiCD) == false)
                        {
                            keigenZeiritsuMap[(int)tokuisakiCD] = new KeigenZeiritsu(_isPrintMarkWhenAllReducedTaxRate);
                        }
                        KeigenZeiritsu keigenZeiritsu = keigenZeiritsuMap[(int)tokuisakiCD];

                        // 売上が存在する明細レコードについて、明細得意先と税率ごとに値引後金額(=買上金額)の合計を集計
                        if (dr.IsNull("売上日") == false)
                        {
                            syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);

                            // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                            // 税率ごとの合計金額を初期化
                            keigenZeiritsu.InitKingaku(syohizei);

                            // 税率ごとの金額を加算
                            decimal taxRate = (decimal)dr["計算税率"];
                            keigenZeiritsu.AddKingaku(taxRate, (decimal)dr["値引後金額"]);

                            // 軽減税率対象の商品名に※を付ける。
                            // ただし、_isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                            // ここでは適用前／通常／軽減の件数を集計するのみ
                            keigenZeiritsu.CountKeigen(syohizei, taxRate);
                        }
                    }
                }

                // 再度全てのレコードに対して
                // ・軽減税率対象の商品名に「※」を付加する
                // ・グループフッターに出力する内訳を出力
                foreach (DataRow dr in dt.Rows)
                {
                    int? tokuisakiCD = dr["明細得意先ＣＤ"] as int?;
                    if (tokuisakiCD != null)
                    {
                        KeigenZeiritsu keigenZeiritsu = keigenZeiritsuMap[(int)tokuisakiCD];

                        // 税率ごとの合計消費税額を計算
                        keigenZeiritsu.CalcTaxAmount((int)dr["税区分"], dr["丸め処理区分"] as int?);

                        // 売上が存在する明細レコードを対象にする
                        if (dr.IsNull("売上日") == false)
                        {
                            syohizei = Syohizei.GetSyohizei((DateTime)dr["売上日"]);
                            decimal taxRate = (decimal)dr["計算税率"];
                            if (syohizei.IsKeigen(taxRate))
                            {
                                dr["商品名"] += keigenZeiritsu.Mark;
                                dr["商品キー"] += keigenZeiritsu.Mark;
                            }
                        }
                    }
                }

                //***************************************************************************
                // 複数の得意先があるので、得意先ごとに売り上げた商品が異なることが有り得る。
                // 得意先ごとに出力する商品(=列)は、その得意先に売り上げた商品であるべき。
                // （別の得意先に売り上げた商品が出力されるのは望ましくない）
                //***************************************************************************
                // 最も多くの商品がある得意先の商品数を取得(cldtの列数になる)
                var tokuisakiCDList = dt.AsEnumerable().Where(row => row.IsNull("明細得意先ＣＤ") == false).Select(row => (int)row["明細得意先ＣＤ"]).Distinct().OrderBy(x => x);     // 得意先の一覧を取得
                int maxColumns = 0;
                foreach (int tokuisakiCD in tokuisakiCDList)
                {
                    // 商品の種類の個数
                    int columns = dt.AsEnumerable().Where(row => (int)row["明細得意先ＣＤ"] == tokuisakiCD && row.IsNull("商品キー") == false).Select(row => row["商品キー"].ToString()).Distinct().Count();
                    maxColumns = Math.Max(maxColumns, columns);
                }

                //*****************************************
                // 日付(行)×商品(列)のデータテーブルを作成
                //*****************************************
                ColumnListDataTable cldt = new ColumnListDataTable("カレンダー請求書");
                int i;
                for (i = 1; i <= maxColumns; i++)
                {
                    cldt.Columns.Add(string.Format("商品名{0:00}", i), typeof(string));         // (☆)
                    cldt.Columns.Add(string.Format("商品単価{0:00}", i), typeof(decimal));      // (☆)
                }

                // 固定列を作成
                foreach (DataColumn column in dt.Columns)
                {
                    // 不定列として追加する列は除く
                    if (column.ColumnName == "商品ＣＤ" ||
                        column.ColumnName == "商品キー" ||
                        column.ColumnName == "商品名" ||
                        column.ColumnName == "商品単価" ||
                        column.ColumnName == "値引後金額" ||
                        column.ColumnName == "計算税率" ||
                        column.ColumnName == "商品数量")
                    {
                        continue;
                    }
                    cldt.AddFixedColumn(column);
                }

                // 銀行名と支店名から作る金融機関名列を追加
                cldt.Columns.Add("金融機関名称1", typeof(string));
                cldt.Columns.Add("金融機関名称2", typeof(string));
                cldt.Columns.Add("振替金融機関名称", typeof(string));

                // ロゴ・印鑑画像の列を追加
                cldt.Columns.Add("ロゴ画像", typeof(byte[]));
                cldt.Columns.Add("印鑑画像", typeof(byte[]));

                // グループフッターに出力する内訳の列を追加（軽減税率対応）
                cldt.Columns.Add("消費税内訳", typeof(string));

                //***************************************************
                // 商品列を生成するので、ここから得意先ごとに処理する
                //***************************************************
                int totalPage = 0;
                DataTable printDt = null;           // 帳票用データテーブル
                foreach (int tokuisakiCD in tokuisakiCDList)
                {
                    // 日付(行)×商品(列)のデータテーブルを得意先別に生成
                    ColumnListDataTable cldt_wk = new ColumnListDataTable();
                    foreach (DataColumn column in cldt.Columns)
                    {
                        DataColumn newColumn = new DataColumn(column.ColumnName, column.DataType);

                        // dt.Columnsにあり、不定列として追加する列以外は固定列として追加。該当しない列は普通の列として追加
                        if (dt.Columns.Contains(column.ColumnName))
                        {
                            // 不定列として追加する列は除く
                            if (column.ColumnName == "商品ＣＤ" ||
                                column.ColumnName == "商品キー" ||
                                column.ColumnName == "商品名" ||
                                column.ColumnName == "商品単価" ||
                                column.ColumnName == "値引後金額" ||
                                column.ColumnName == "計算税率" ||
                                column.ColumnName == "商品数量")
                            {
                                continue;
                            }
                            cldt_wk.AddFixedColumn(newColumn);
                        }
                        else
                        {
                            cldt_wk.Columns.Add(newColumn);
                        }
                    }

                    //*****************************************************************
                    // 帳票の列として表現するための商品ＣＤ、商品名、商品単価を取得する
                    //*****************************************************************
                    DataView dv = new DataView(dt);                                     // 重複を除去するためのDataView
                    dv.RowFilter = string.Format("明細得意先ＣＤ = {0}", tokuisakiCD);    // 得意先で絞る
                    DataTable syohinDt = dv.ToTable(true, "商品ＣＤ", "商品キー", "商品名", "商品単価");
                    DataRow[] drs = syohinDt.Select("商品ＣＤ IS NOT NULL", "商品ＣＤ, 商品キー");      // ソート
                    syohinDt = syohinDt.Clone();
                    foreach (DataRow dr in drs)
                    {
                        syohinDt.ImportRow(dr);
                    }

                    // 不定列（を持つデータテーブル）を作成
                    DataTable dtSuryo = new DataTable();
                    dtSuryo.Columns.Add("数量列名", typeof(string));
                    dtSuryo.Columns.Add("商品キー", typeof(string));
                    // 取得した商品名の一覧からレコードを作成
                    for (i = 1; i <= syohinDt.Rows.Count; i++)
                    {
                        DataRow wkDr = dtSuryo.NewRow();
                        wkDr["数量列名"] = string.Format("数量{0:00}", i);  // (☆)
                        wkDr["商品キー"] = syohinDt.Rows[i - 1]["商品キー"];
                        dtSuryo.Rows.Add(wkDr);
                    }
                    cldt_wk.AddColumns(dtSuryo, "数量列名", "商品キー", typeof(decimal));

                    // インポート
                    DataTable tokuisakiDt = dv.ToTable();
                    cldt_wk.Import(tokuisakiDt, "商品キー", "商品数量");

                    KeigenZeiritsu keigenZeiritsu = keigenZeiritsuMap[(int)tokuisakiCD];    // 得意先に該当する軽減税率算出用クラスを取得

                    // 全ての行に対して…
                    foreach (DataRow dr in cldt_wk.Rows)
                    {
                        // 銀行名と支店名から金融機関名を作る
                        MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                        MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");
                        MakeBankName(dr, "振替銀行名", "振替支店名", "振替金融機関名称");

                        // 振替金融機関名称の末尾に振替口座番号を追加する
                        if (string.IsNullOrEmpty(dr["振替口座番号"].ToString()) == false)
                        {
                            dr["振替金融機関名称"] = string.Format("{0} {1}", dr["振替金融機関名称"], dr["振替口座番号"]);
                        }

                        // ロゴ・印鑑画像の列に画像を設定
                        dr["ロゴ画像"] = _logoImageBytes;
                        dr["印鑑画像"] = _inkanImageBytes;

                        // 消費税内訳は全てのレコードに格納する
                        dr["消費税内訳"] = keigenZeiritsu.TaxDetail;

                        // 商品名と商品単価を埋める
                        for (i = 1; i <= syohinDt.Rows.Count; i++)
                        {
                            string columnName = string.Format("商品名{0:00}", i);
                            dr[columnName] = syohinDt.Rows[i - 1]["商品名"];

                            columnName = string.Format("商品単価{0:00}", i);
                            dr[columnName] = syohinDt.Rows[i - 1]["商品単価"];
                        }
                    }

                    // 小計行を作成
                    DataRow newRow = cldt_wk.NewRow();
                    newRow.ItemArray = cldt_wk.Rows[0].ItemArray;  // 先頭行の内容をコピーする
                    newRow["明細小計区分"] = 1;
                    newRow["日付"] = DBNull.Value;
                    newRow["表示用日付"] = "小計";
                    for (i = 1; i <= syohinDt.Rows.Count; i++)
                    {
                        string columnName = string.Format("数量{0:00}", i);
                        newRow[columnName] = cldt_wk.Compute(string.Format("Sum({0})", columnName), null);
                    }
                    cldt_wk.Rows.Add(newRow);

                    // 商品数(=列数)がCALENDAR_TATE_COLUMNSを超えている場合は改ページする
                    DataTable pageDt = CalendarTatePageFeed(cldt_wk, syohinDt.Rows.Count, ref totalPage);
                    if (printDt == null)
                    {
                        printDt = pageDt.Clone();
                    }
                    foreach (DataRow dr in pageDt.Rows)
                    {
                        printDt.ImportRow(dr);
                    }
                }

                Report202A report202A = new Report202A();
                report202A.TopOffsetCM = _topOffset;
                report202A.LeftOffsetCM = _leftOffset;
                report202A.DataSource = printDt;
                report202A.Document.Printer.DocumentName = string.Format("請求書 - {0}({1})", seikyusakiName, seikyuNo);
                report202A.Run(false);

                log.InfoFormat("請求書（カレンダー縦）（複数得意先）を印刷しました。請求番号={0}", seikyuNo);

                return report202A;
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
        /// カレンダー縦タイプの請求書のデータテーブルを商品数(=列数)に応じて改ページする
        /// </summary>
        /// <param name="cldt">カレンダー縦タイプの請求書出力用のデータテーブル</param>
        /// <param name="columnsCount">現在の商品数(=列数)</param>
        /// <param name="totalPage">出力済みのページ数</param>
        /// <returns>改ページを施したデータテーブル</returns>
        private DataTable CalendarTatePageFeed(ColumnListDataTable cldt, int columnsCount, ref int totalPage)
        {
            // 例えば以下のようなデータテーブルがある場合
            // ┌───┬─────┬───┬─────┬───┬─────┬───┬─────┬───┬─────┬───┬─────┬───┬─────┬───┬───┬───┬──────┬───┐
            // │日付  │ページ番号│固定列│商品01    │数量01│商品02    │数量02│商品03    │数量03│商品04    │数量04│商品05    │数量05│商品06    │数量06│商品07│数量07│商品08      │数量08│
            // ├───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼───┼───┼──────┼───┤
            // │10/01 │         0│foo   │ライス(小)│    10│ライス(中)│     4│ライス(大)│     2│おかず(松)│      │おかず(竹)│    15│おかず(梅)│     1│みそ汁│     5│わかめスープ│     1│
            // ├───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼───┼───┼──────┼───┤
            // │  ：  │          │      │          │      │          │      │          │      │          │      │          │      │          │      │      │      │            │      │
            // ├───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼───┼───┼──────┼───┤
            // │10/31 │         0│foo   │ライス(小)│    20│ライス(中)│     5│ライス(大)│      │おかず(松)│    11│おかず(竹)│     5│おかず(梅)│    18│みそ汁│    32│わかめスープ│      │
            // ├───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┼───┼───┼──────┼───┤
            // │小計  │         0│      │          │    30│          │     9│          │     2│          │    11│          │    20│          │    19│      │    37│            │     1│
            // └───┴─────┴───┴─────┴───┴─────┴───┴─────┴───┴─────┴───┴─────┴───┴─────┴───┴───┴───┴──────┴───┘
            // １ページに印刷できる列数(=商品数)はCALENDAR_TATE_COLUMNS(=6)なので、そこで改ページさせるべく、以下のようなデータテーブルを生成する
            // ┌───┬─────┬───┬─────┬───┬──────┬───┬─────┬───┬─────┬───┬─────┬───┬─────┬───┐
            // │日付  │ページ番号│固定列│商品01    │数量01│商品02      │数量02│商品03    │数量03│商品04    │数量04│商品05    │数量05│商品06    │数量06│
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │10/01 │         0│foo   │ライス(小)│    10│ライス(中)  │     4│ライス(大)│     2│おかず(松)│      │おかず(竹)│    15│おかず(梅)│     1│
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │  ：  │          │      │          │      │            │      │          │      │          │      │          │      │          │      │
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │10/31 │         0│foo   │ライス(小)│    20│ライス(中)  │     5│ライス(大)│      │おかず(松)│    11│おかず(竹)│     5│おかず(梅)│    18│
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │小計  │         0│      │          │    30│            │     9│          │     2│          │    11│          │    20│          │    19│
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │10/01 │         1│foo   │みそ汁    │     5│わかめスープ│     1│          │      │          │      │          │      │          │      │
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │  ：  │          │foo   │          │      │            │      │          │      │          │      │          │      │          │      │
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │10/31 │         1│foo   │みそ汁    │    32│わかめスープ│      │          │      │          │      │          │      │          │      │
            // ├───┼─────┼───┼─────┼───┼──────┼───┼─────┼───┼─────┼───┼─────┼───┼─────┼───┤
            // │小計  │         1│      │          │    37│            │     1│          │      │          │      │          │      │          │      │
            // └───┴─────┴───┴─────┴───┴──────┴───┴─────┴───┴─────┴───┴─────┴───┴─────┴───┘

            try
            {
                // 元のテーブルと共通する固定列とCALENDAR_TATE_COLUMNS(=6)個の商品列を持つ新しいテーブルを作成し、
                // １レコードずつ、固定列と商品列を（1ページ目は商品01→商品01,商品02→商品02、2ページ目は商品07→商品01,商品08→商品02というように）コピーする

                // 必要なページ数にちょうど収まるように、不足している列を追加する
                int maxPages = ((columnsCount - 1) / CALENDAR_TATE_COLUMNS) + 1;        // 全ての商品の出力に必要なページ数

                string[] syohinColumnNames = { "商品名", "商品単価", "数量" };          // 商品列(☆)

                // コピー先のデータテーブルを作成
                DataTable newDt = new DataTable();
                // 商品列以外を作成
                foreach (DataColumn dc in cldt.Columns)
                {
                    bool found = false;
                    foreach (string syohinColumnName in syohinColumnNames)
                    {
                        if (dc.ColumnName.StartsWith(syohinColumnName))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                    {
                        newDt.Columns.Add(new DataColumn(dc.ColumnName, dc.DataType));
                    }
                }

                // 商品列を作成
                for (int i = 1; i <= CALENDAR_TATE_COLUMNS; i++)
                {
                    newDt.Columns.Add(string.Format("商品名{0:00}", i), typeof(string));
                    newDt.Columns.Add(string.Format("商品単価{0:00}", i), typeof(decimal));
                    newDt.Columns.Add(string.Format("数量{0:00}", i), typeof(decimal));
                }

                // コピー元のページ数分ループ
                for (int page = 0; page < maxPages; page++)
                {
                    // 作業用のデータテーブルを作成
                    DataTable dt_wk = newDt.Clone();

                    // コピー元テーブルの全レコード分ループ
                    foreach (DataRow dr in cldt.Rows)
                    {
                        // コピー先レコードを作成
                        DataRow newDr = dt_wk.NewRow();

                        dr["ページ番号"] = page + totalPage;

                        // まず商品列以外の内容をコピー
                        foreach (DataColumn dc in dt_wk.Columns)
                        {
                            bool found = false;
                            foreach (string syohinColumnName in syohinColumnNames)
                            {
                                if (dc.ColumnName.StartsWith(syohinColumnName))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found == false)
                            {
                                newDr[dc.ColumnName] = dr[dc.ColumnName];
                            }
                        }

                        // 次に商品列の内容をコピー
                        for (int columnIndex = 1; columnIndex <= CALENDAR_TATE_COLUMNS; columnIndex++)
                        {
                            int srcColumnIndex = (page * CALENDAR_TATE_COLUMNS) + columnIndex;    // コピー元の列インデックス
                            if (srcColumnIndex <= columnsCount)
                            {
                                foreach (string syohinColumnName in syohinColumnNames)
                                {
                                    string srcColumnName = string.Format("{0}{1:00}", syohinColumnName, srcColumnIndex);
                                    string destColumnName = string.Format("{0}{1:00}", syohinColumnName, columnIndex);
                                    newDr[destColumnName] = dr[srcColumnName];
                                }
                            }
                        }

                        dt_wk.Rows.Add(newDr);
                    }

                    // 作業用データテーブルをコピー先のデータテーブルにコピーする
                    foreach (DataRow newDr in dt_wk.Rows)
                    {
                        newDt.ImportRow(newDr);
                    }
                }

                totalPage += maxPages;      // 出力済みのページ数に加算

                return newDt;
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
        /// 銀行名と支店名から金融機関名を作る
        /// </summary>
        /// <param name="dr">データ行</param>
        /// <param name="bankColumnName">銀行名列名</param>
        /// <param name="branchColumnName">支店名列名</param>
        /// <param name="targetColumnName">金融機関名</param>
        public void MakeBankName(DataRow dr, string bankColumnName, string branchColumnName, string targetColumnName)
        {
            try
            {
                string bankName = dr[bankColumnName].ToString();
                string bank = "";
                if (string.IsNullOrEmpty(bankName) == false)
                {
                    bank = "銀行";
                    // 銀行名の末尾が以下に該当しない場合は、末尾に"銀行"を付加する
                    string[] bankSuffix = { "新銀行東京", "金庫", "信金", "組連", "信組", "機構", "金連", "労金", "中金", "信連", "農協", "漁連", "漁協" };
                    foreach (string s in bankSuffix)
                    {
                        if (bankName.EndsWith(s))
                        {
                            bank = "";
                            break;
                        }
                    }
                }

                string branchName = dr[branchColumnName].ToString();
                string branch = "";
                if (string.IsNullOrEmpty(branchName) == false)
                {
                    branch = "支店";
                    // 支店名の末尾が以下に該当しない場合は、末尾に"支店"を付加する
                    string[] branchSuffix = { "公務部", "法人部", "営業部", "営業本部", "統括本部", "出張所", "年金", "本店", "営業第一部", "営業第二部", "営業第三部", "営業第四部", "営業第五部" };
                    foreach (string s in branchSuffix)
                    {
                        if (branchName.EndsWith(s))
                        {
                            branch = "";
                            break;
                        }
                    }
                }

                string targetName = string.Format("{0}{1} {2}{3}", bankName, bank, branchName, branch);
                dr[targetColumnName] = targetName;
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
        /// 請求合計の印刷日時を更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="seikyuNo">請求番号</param>
        /// <returns>成功した場合はnull。失敗した場合はエラーメッセージ</returns>
        private string UpdateSeikyuTotal(SqlConnection con, decimal seikyuNo)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            try
            {
                UserData userData = UserData.getInstance();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE 請求合計 SET 印刷者ＣＤ = @印刷者ＣＤ, 印刷日時 = getdate() WHERE 企業ＣＤ = @企業ＣＤ AND 請求番号 = @請求番号";
                cmd.Parameters.Add("@印刷者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;
                log.Debug(cmd.CommandTextWithParameters());
                if (cmd.ExecuteNonQuery() != 1)
                {
                    return "別の端末で更新済みです。再度検索してください。";
                }

                // 操作履歴に保存
                History.Save(con, _ownerTitle, "請求合計", seikyuNo.ToString(), History.ProcessType.Update);

                return null;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    return "請求合計テーブルが重複しています。";
                }
                else
                {
                    History.Error(ex);
                    return string.Format("請求合計テーブルの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                return string.Format("請求合計テーブルの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
            }
        }
        #endregion
    }
}

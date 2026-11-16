using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 納品書発行
    /// </summary>
    public partial class Form206 : BaseForm
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
                if (_kingakuMap.ContainsKey(taxRate) == false)
                {
                    _kingakuMap[taxRate] = 0m;
                }
                _kingakuMap[taxRate] += kingaku;
            }

            /// <summary>
            /// 税率ごとの合計消費税額を計算する
            /// </summary>
            /// <param name="zeiKbn">税区分</param>
            /// <param name="hasuKbn">丸め区分</param>
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
                    if (taxRate == syohizei.軽減税率)
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

                string zeiKbn = _isExcludedTax ? "外税" : "内税";
                string label = _isExcludedTax ? "消費税" : "内消費税";

                for (int i = _kingakuMap.Count - 1 ; i >= 0 ; i--)  // 税率の降順
                {
                    detail.AppendFormat("({0} {1:P0}対象 {2:N0}円  {3} {4:N0}円)", zeiKbn, _kingakuMap.Keys[i], _kingakuMap.Values[i], label, _taxAmountMap.Values[i]);   // 内税 10%対象 1,000円  内消費税 90円
                    //detail.AppendFormat("({0:P0}対象 {1:N0}円)", _kingakuMap.Keys[i], _kingakuMap.Values[i]);   // 10%対象 1,000円
                    detail.AppendLine();
                }

                // 軽減税率の商品であれば「※」を出力するのが原則。しかし、全ての商品が軽減税率対象の場合に「※」を出力したくない場合もある。
                // ┌────────────────┬─────────┬─────────┬─────────────────┐
                // │isPrintMarkWhenAllReducedTaxRate│全て軽減税率対象  │全て通常税率対象  │両者が混在                        │
                // ├────────────────┼─────────┼─────────┼─────────────────┤
                // │true                            │「※」を出力する  │「※」は出力しない│軽減税率対象にのみ「※」を出力する│
                // ├────────────────┼─────────┼─────────┼─────────────────┤
                // │false                           │「※」は出力しない│「※」は出力しない│軽減税率対象にのみ「※」を出力する│
                // └────────────────┴─────────┴─────────┴─────────────────┘
                // 全て軽減対象でも※を出力？
                if (_isPrintMarkWhenAllReducedTaxRate)
                {
                    // 軽減税率が始まっている？
                    if (_hasKeigen)
                    {
                        detail.AppendFormat("注)※印は軽減税率({0:P0})適用商品", _reducedTaxRate);
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
                        string s = string.Format("注)※印は軽減税率({0:P0})適用商品", _reducedTaxRate);
                        _mark = "　※";

                        // 軽減のみ(No1)？
                        if (_countMap.ContainsKey(TaxKind.Unapplied) == false &&
                            _countMap.ContainsKey(TaxKind.Normal) == false &&
                            _countMap.ContainsKey(TaxKind.Reduced) == true)
                        {
                            s = string.Format("注)全て軽減税率({0:P0})適用商品です", _reducedTaxRate);
                            _mark = "";
                        }

                        // 通常のみ(No2)？
                        if (_countMap.ContainsKey(TaxKind.Unapplied) == false &&
                            _countMap.ContainsKey(TaxKind.Normal) == true &&
                            _countMap.ContainsKey(TaxKind.Reduced) == false)
                        {
                            s = string.Format("注)全て通常税率({0:P0})適用商品です", _normalTaxRate);
                            _mark = "";
                        }

                        // 8%のみ(No5)？
                        if (_countMap.ContainsKey(TaxKind.Unapplied) == true &&
                            _countMap.ContainsKey(TaxKind.Normal) == false &&
                            _countMap.ContainsKey(TaxKind.Reduced) == true)
                        {
                            s = string.Format("注)全て軽減税率({0:P0})適用商品です", _reducedTaxRate);
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
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// 納品書1ページあたりの行数
        /// </summary>
        private static int LINES_PER_PAGE = 7;
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 納品書発行
        /// </summary>
        private NohinPublish _nohinPublish;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form206()
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
        private void Form206_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                //ソートキーのリストを設定する
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "コースCD" },
                    new string[] { "A.得意先ＣＤ {0}, CASE WHEN A.納品書伝票番号 IS NULL THEN 1 ELSE 0 END, A.納品書伝票番号",
                                   "A.コースＣＤ {0}, C.コース順, CASE WHEN A.納品書伝票番号 IS NULL THEN 1 ELSE 0 END, A.納品書伝票番号" });

                //初期値設定
                this.cboSortKey.Text = "得意先CD";

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _nohinPublish = new NohinPublish();
                nohinPublishBindingSource.DataSource = _nohinPublish;
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

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form206_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F10:                      // 印刷ボタン押下
                        btnPrint.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 配送日ラジオボタン変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoHaisobi_CheckedChanged(object sender, EventArgs e)
        {
            // 配送日、売上日の活性状態を日付指定のラジオボタンに合わせる
            dtpHaisobi.Enabled = rdoHaisobi.Checked;
            dtpUriagebi.Enabled = rdoUriagebi.Checked;

            if (rdoHaisobi.Checked)
            {
                _nohinPublish.配送日 = _nohinPublish.売上日;
                _nohinPublish.売上日 = null;

                ReadBinding(dtpHaisobi);
                ReadBinding(dtpUriagebi);
            }
        }

        /// <summary>
        /// 売上日ラジオボタン変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoUriagebi_CheckedChanged(object sender, EventArgs e)
        {
            // 配送日、売上日の活性状態を日付指定のラジオボタンに合わせる
            dtpHaisobi.Enabled = rdoHaisobi.Checked;
            dtpUriagebi.Enabled = rdoUriagebi.Checked;

            if (rdoUriagebi.Checked)
            {
                _nohinPublish.売上日 = _nohinPublish.配送日;
                _nohinPublish.配送日 = null;
                chkPrintKingaku.Checked = true;            // 売上日が選択された場合は「金額を印字する」もONにする

                ReadBinding(dtpHaisobi);
                ReadBinding(dtpUriagebi);
            }
        }

        /// <summary>
        /// コースＣＤFromキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤFrom参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDFrom_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCDFrom_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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

        /// <summary>
        /// コース選択ダイアログクローズ（コースＣＤFrom）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCDFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCDFrom, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDFrom.Text = course.コースＣＤ.ToString();
                    _nohinPublish.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameFrom.Text = course.コース名;
                }
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

        /// <summary>
        /// コースＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDTo.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDTo_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCDTo_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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

        /// <summary>
        /// コース選択ダイアログクローズ（コースＣＤTo）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCDTo_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCDTo, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDTo.Text = course.コースＣＤ.ToString();
                    _nohinPublish.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameTo.Text = course.コース名;
                }
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

        /// <summary>
        /// 得意先ＣＤFromキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤFrom参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCDFrom_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDFrom_FormClosed);
                formTokuisakiDlg.ShowDialog();
                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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

        /// <summary>
        /// 得意先選択ダイアログクローズ（得意先ＣＤFrom）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form900 form = (Form900)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCDFrom, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCDFrom.Text = tokuisaki.得意先ＣＤ.ToString();
                    _nohinPublish.得意先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameFrom.Text = tokuisaki.得意先名;
                }
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

        /// <summary>
        /// 得意先ＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCDTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCDTo.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCDTo_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDTo_FormClosed);
                formTokuisakiDlg.ShowDialog();
                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
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

        /// <summary>
        /// 得意先選択ダイアログクローズ（得意先ＣＤTo）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDTo_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form900 form = (Form900)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCDTo, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCDTo.Text = tokuisaki.得意先ＣＤ.ToString();
                    _nohinPublish.得意先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameTo.Text = tokuisaki.得意先名;
                }
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

        /// <summary>
        /// 見出しを「納品書兼請求書」にするチェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTitle_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTitle.Checked == true)
            {
                // 納品書の見出しを「納品書兼請求書」にする場合は、必ず金額も印字する
                _nohinPublish.金額印字フラグ = true;
                ReadBinding(chkPrintKingaku);
                chkPrintKingaku.Enabled = false;
            }
            else
            {
                chkPrintKingaku.Enabled = true;
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
                // 検索条件の入力にエラーがある？
                if (_nohinPublish.HasErrors)
                {
                    if (string.IsNullOrEmpty(_nohinPublish.Error) == false)
                    {
                        MessageBox.Show(this, _nohinPublish.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.Nouhinsho, PrintMain);
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

        /// <summary>
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
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

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintMain(string printerNameA4)
        {
            byte[] logoImageBytes = null;   // 指定した部署のロゴ画像のバイト配列

            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataTable headerPageDt = null;
                DataTable detailPageDt = null;

                // ロゴ画像を取得
                BusyoImage busyoImage = BusyoImage.GetBusyoImage((int)_nohinPublish.部署ＣＤ);
                logoImageBytes = busyoImage.ロゴ;

                bool isPrintTaxIncludeTitle = SystemMaster.IsPrintTaxIncludeTitle();                        // 「内消費税」見出しを出力するかどうか
                bool isPrintMarkWhenAllReducedTaxRate = SystemMaster.IsPrintMarkWhenAllReducedTaxRate();    // 軽減税率の商品を示す「※」を、全ての商品が軽減税率対象であっても出力するかどうか

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    // 親レコード（ヘッダレコード）を取得
                    DataTable headerDt = GetHeader(con, isPrintTaxIncludeTitle);
                    if (headerDt == null || headerDt.Rows.Count == 0)
                    {
                        MessageBox.Show(this, "出力条件に合う納品書データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    // **********************************************************************************
                    // ここで納品書伝票番号を付与して注文ヘッダを更新しておくことで、
                    // 明細レコードを取得する際に納品書伝票番号がnullのレコードを取得することがなくなり、
                    // 親子のレコードが確実に結合できる。
                    // **********************************************************************************
                    // 取得した親レコードに納品書伝票番号を付与して更新する
                    if (UpdateDenpyoNo(con, headerDt) == false)
                    {
                        return;
                    }

                    // ロゴ出力フラグとロゴ画像の列を追加
                    headerDt.Columns.Add("納品書ロゴ出力", typeof(bool));
                    headerDt.Columns.Add("ロゴ画像", typeof(byte[]));

                    // 内訳の列を追加（軽減税率対応）
                    headerDt.Columns.Add("消費税内訳", typeof(string));

                    // 納品書伝票番号を付与した後の明細レコードを取得
                    DataTable detailDt = GetDetail(con);

                    // ページ番号を持ったヘッダレコードと、ページあたりの行数を持った明細レコードを生成
                    headerPageDt = headerDt.Clone();
                    detailPageDt = detailDt.Clone();

                    foreach (DataRow headerDr in headerDt.Rows)
                    {
                        // 得意先ごとに軽減税率処理を実施
                        KeigenZeiritsu keigenZeiritsu = new KeigenZeiritsu(isPrintMarkWhenAllReducedTaxRate);

                        // 【明細レコード】
                        DataRow[] drs = detailDt.Select(string.Format("得意先ＣＤ={0} and 納品書伝票番号={1}",
                                                        headerDr["得意先ＣＤ"], headerDr["納品書伝票番号"]),
                                                        "納品書伝票番号, 喫食日, 明細番号");

                        // 【明細レコード】
                        //*************
                        // 軽減税率対応
                        //*************
                        foreach (DataRow detailDr in drs)
                        {
                            if (_nohinPublish.金額印字フラグ == true && detailDr.IsNull("喫食日") == false)
                            {
                                Syohizei syohizei = Syohizei.GetSyohizei((DateTime)detailDr["喫食日"]);

                                // 軽減税率が開始されていれば、全ての税率の合計を保持する必要がある。
                                // 税率ごとの合計金額を初期化
                                keigenZeiritsu.InitKingaku(syohizei);

                                // 税率ごとの金額を加算
                                decimal taxRate = (decimal)detailDr["計算税率"];
                                keigenZeiritsu.AddKingaku(taxRate, (decimal)detailDr["金額"]);

                                // 軽減税率対象の商品名に※を付ける。
                                // ただし、isPrintMarkWhenAllReducedTaxRateに準ずるため、もう一度ループする必要がある。
                                // ここでは適用前／通常／軽減の件数を集計するのみ
                                keigenZeiritsu.CountKeigen(syohizei, taxRate);
                            }
                        }

                        // 【ヘッダレコード】
                        // 金額を印字する場合は
                        // ・消費税額を算出
                        // ・グループフッターに出力する内訳を出力
                        if (_nohinPublish.金額印字フラグ == true)
                        {
                            int? hasuKubun = headerDr["丸め処理区分"] as int?;
                            decimal totalTax = 0;

                            if ((int)headerDr["税区分"] == (int)Syohizei.ZeiKubun.Soto)
                            {
                                totalTax = keigenZeiritsu.GetTaxExcluded(hasuKubun);            // 消費税額を算出
                                headerDr["合計金額"] = ((decimal)headerDr["小計"]) + totalTax;  // 算出した消費税額を合計金額に含める
                            }
                            else
                            {
                                totalTax = keigenZeiritsu.GetTaxIncluded(hasuKubun);    // 内消費税額を算出
                            }

                            // 税率ごとの合計消費税額を計算
                            keigenZeiritsu.CalcTaxAmount((int)headerDr["税区分"], hasuKubun);

                            headerDr["消費税"] = totalTax;
                            headerDr["消費税内訳"] = keigenZeiritsu.TaxDetail;
                        }

                        // 【ヘッダレコード】ページ数と同じ数のレコードを追加
                        int pageCnt = (int)headerDr["ページ数"];
                        int page;
                        for (page = 1; page <= pageCnt; page++)
                        {
                            DataRow newDr = headerPageDt.NewRow();
                            newDr.ItemArray = headerDr.ItemArray;
                            newDr["ページ番号"] = page;
                            newDr["ロゴ画像"] = logoImageBytes;
                            headerPageDt.Rows.Add(newDr);
                        }


                        // 【明細レコード】
                        // 再度全てのレコードに対して
                        // ・軽減税率対象の商品名に「※」を付加する
                        // これらを実施した上で出力用データテーブルに格納する
                        int line = 0;
                        foreach (DataRow detailDr in drs)
                        {
                            if (_nohinPublish.金額印字フラグ == true && detailDr.IsNull("喫食日") == false)
                            {
                                Syohizei syohizei = Syohizei.GetSyohizei((DateTime)detailDr["喫食日"]);
                                decimal taxRate = (decimal)detailDr["計算税率"];
                                if (syohizei.IsKeigen(taxRate))
                                {
                                    detailDr["商品名"] += keigenZeiritsu.Mark;
                                }
                            }

                            DataRow newDr = detailPageDt.NewRow();
                            newDr.ItemArray = detailDr.ItemArray;
                            line++;
                            newDr["ページ番号"] = ((line - 1) / LINES_PER_PAGE) + 1;
                            newDr["明細ソート用番号"] = line;
                            detailPageDt.Rows.Add(newDr);
                        }

                        // 【明細レコード】
                        // ページ数の行数を満たすまで空行を追加
                        while (line < pageCnt * LINES_PER_PAGE)
                        {
                            DataRow newDr = detailPageDt.NewRow();
                            line++;
                            newDr["ページ番号"] = ((line - 1) / LINES_PER_PAGE) + 1;
                            newDr["納品書伝票番号"] = headerDr["納品書伝票番号"];
                            newDr["明細ソート用番号"] = line;
                            detailPageDt.Rows.Add(newDr);
                        }

                        string code = string.Format("部署CD=[{0}] 得意先CD=[{1}] 配送日=[{2}]", headerDr["部署ＣＤ"], headerDr["得意先ＣＤ"], headerDr["配送日"]);
                        History.Save(con, this.Text, "納品書", code, History.ProcessType.Output);
                    }
                }

                Report100A report100A = new Report100A();
                report100A.DataSource = headerPageDt;
                report100A.DetailPageDt = detailPageDt;
                report100A.Document.Printer.DocumentName = "納品書";
                report100A.Document.Printer.PrinterName = printerNameA4;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "納品書";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report100A);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// プレビュー画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormPreview form = (FormPreview)sender;
                form.Dispose();
                form = null;
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

        /// <summary>
        /// 親レコード（ヘッダレコード）を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="isPrintTaxIncludeTitle">納品書の消費税の見出しを「内消費税」にしたい場合はtrue。それ以外はfalse。</param>
        /// <returns>印刷対象が格納されたデータテーブル</returns>
        private DataTable GetHeader(SqlConnection con, bool isPrintTaxIncludeTitle)
        {
            if (_nohinPublish.配送日 != null)
            {
                // 配送日が指定されていれば注文から取得
                return GetHeaderChumon(con, isPrintTaxIncludeTitle);
            }
            else
            {
                // 売上日が指定されていれば売上から取得
                return GetHeaderUriage(con, isPrintTaxIncludeTitle);
            }
        }

        /// <summary>
        /// 注文データから親レコード（ヘッダレコード）を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="isPrintTaxIncludeTitle">納品書の消費税の見出しを「内消費税」にしたい場合はtrue。それ以外はfalse。</param>
        /// <returns>印刷対象が格納されたデータテーブル</returns>
        private DataTable GetHeaderChumon(SqlConnection con, bool isPrintTaxIncludeTitle)
        {
            try
            {
                DataTable dt = new DataTable();

                SqlCommand cmd = con.CreateCommand();

                // 親レコード（ヘッダレコード）を取得
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     A.納品書伝票番号");
                sql.AppendLine("     ,0 AS ページ番号");

                if (_nohinPublish.見出しフラグ == true)
                {
                    sql.AppendLine("     ,'納品書兼請求書(控)' AS 見出し控え");
                    sql.AppendLine("     ,'納品書兼請求書' AS 見出し");
                }
                else
                {
                    sql.AppendLine("     ,'納    品    書(控)' AS 見出し控え");
                    sql.AppendLine("     ,'納    品    書' AS 見出し");
                }

                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様')) AS 得意先名長さ");
                sql.AppendLine("     ,CASE WHEN T.お届け先住所1 IS NULL");
                sql.AppendLine("         THEN T.住所1 + ISNULL(T.住所2, '')");
                sql.AppendLine("         ELSE T.お届け先住所1 + ISNULL(T.お届け先住所2 ,'')");
                sql.AppendLine("     END AS 納品先住所");
                sql.AppendLine("     ,ISNULL(T.お届け先電話番号1, ISNULL(お届け先電話番号2, ISNULL(電話番号1, 電話番号2))) AS 納品先電話番号");
                sql.AppendLine("     ,ISNULL(A.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,CASE ISNULL(A.税区分, @税区分内税)");
                sql.AppendLine("         WHEN @税区分内税 THEN '全て税込です'");
                sql.AppendLine("         WHEN @税区分外税 THEN '全て税別です'");
                sql.AppendLine("         ELSE ''");
                sql.AppendLine("      END AS 税区分備考");
                sql.AppendLine("     ,ISNULL(A.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,ISNULL(T.売掛現金区分, @売掛現金区分現金) AS 売掛現金区分");
                sql.AppendLine("     ,A.企業ＣＤ");
                sql.AppendLine("     ,A.注文区分");
                sql.AppendLine("     ,A.得意先ＣＤ");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.配送日");
                sql.AppendLine("     ,RIGHT('0000' + convert(varchar, year(A.配送日)), 4) + '年' +");
                sql.AppendLine("      RIGHT('00' + convert(varchar, month(A.配送日)) ,2) + '月' +");
                sql.AppendLine("      RIGHT('00' + convert(varchar, day(A.配送日)), 2) + '日(' +");
                sql.AppendLine("      CASE DATEPART(WEEKDAY, A.配送日)");
                sql.AppendLine("         WHEN 1 THEN '日'");
                sql.AppendLine("         WHEN 2 THEN '月'");
                sql.AppendLine("         WHEN 3 THEN '火'");
                sql.AppendLine("         WHEN 4 THEN '水'");
                sql.AppendLine("         WHEN 5 THEN '木'");
                sql.AppendLine("         WHEN 6 THEN '金'");
                sql.AppendLine("         WHEN 7 THEN '土'");
                sql.AppendLine("      END  + ')' AS 納品日");
                sql.AppendLine("     ,A.明細件数");
                sql.AppendLine("     ,((A.明細件数 - 1) / @行数) + 1 AS ページ数");

                // 金額印字チェックボックスがONの場合は金額を取得
                if (_nohinPublish.金額印字フラグ == true)
                {
                    sql.AppendLine("     ,A.小計");
                    sql.AppendLine("     ,NULL AS 消費税");
                    sql.AppendLine("     ,A.合計金額");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS 小計");
                    sql.AppendLine("     ,NULL AS 消費税");
                    sql.AppendLine("     ,NULL AS 合計金額");
                }
                sql.AppendLine("     ,A.作成日 AS 注文日付");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社FAX");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,@金額印字フラグ AS 金額印字フラグ");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,@見出しフラグ AS 納品書兼請求書フラグ");
                sql.AppendLine(" FROM");
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         H.企業ＣＤ");
                sql.AppendLine("         ,H.注文区分");
                sql.AppendLine("         ,H.得意先ＣＤ");
                sql.AppendLine("         ,H.部署ＣＤ");
                sql.AppendLine("         ,H.配送日");
                sql.AppendLine("         ,H.コースＣＤ");
                sql.AppendLine("         ,H.税区分");
                sql.AppendLine("         ,H.丸め処理区分");
                sql.AppendLine("         ,H.納品書伝票番号");
                sql.AppendLine("         ,MIN(H.作成日) AS 作成日");
                sql.AppendLine("         ,COUNT(M.明細番号) AS 明細件数");
                sql.AppendLine("         ,SUM(M.商品金額合計) AS 小計");
                sql.AppendLine("         ,SUM(M.消費税額) AS 消費税");
                sql.AppendLine("         ,SUM(M.商品金額合計税込) AS 合計金額");
                sql.AppendLine("     FROM");
                sql.AppendLine("         注文ヘッダ AS H");
                sql.AppendLine("         INNER JOIN 注文明細 AS M");
                sql.AppendLine("             ON  H.企業ＣＤ   = M.企業ＣＤ");
                sql.AppendLine("             AND H.注文区分   = M.注文区分");
                sql.AppendLine("             AND H.喫食日     = M.喫食日");
                sql.AppendLine("             AND H.得意先ＣＤ = M.得意先ＣＤ");
                sql.AppendLine("             AND H.部署ＣＤ   = M.部署ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND H.注文区分 = @注文区分注文");
                sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");
                sql.AppendLine("         AND H.配送日 = @配送日");

                if (_nohinPublish.コースＣＤFrom != null)
                {
                    sql.AppendLine("         AND H.コースＣＤ >= @コースＣＤFrom");
                    cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _nohinPublish.コースＣＤFrom;
                }

                if (_nohinPublish.コースＣＤTo != null)
                {
                    sql.AppendLine("         AND H.コースＣＤ <= @コースＣＤTo");
                    cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _nohinPublish.コースＣＤTo;
                }

                if (_nohinPublish.得意先ＣＤFrom != null)
                {
                    sql.AppendLine("         AND H.得意先ＣＤ >= @得意先ＣＤFrom");
                    cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤFrom;
                }

                if (_nohinPublish.得意先ＣＤTo != null)
                {
                    sql.AppendLine("         AND H.得意先ＣＤ <= @得意先ＣＤTo");
                    cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤTo;
                }

                if (_nohinPublish.印刷済みフラグ == null || (bool)_nohinPublish.印刷済みフラグ == false)
                {
                    sql.AppendLine("         AND H.納品書伝票番号 IS NULL");
                }

                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         H.企業ＣＤ");
                sql.AppendLine("         ,H.注文区分");
                sql.AppendLine("         ,H.得意先ＣＤ");
                sql.AppendLine("         ,H.部署ＣＤ");
                sql.AppendLine("         ,H.配送日");
                sql.AppendLine("         ,H.コースＣＤ");
                sql.AppendLine("         ,H.税区分");
                sql.AppendLine("         ,H.丸め処理区分");
                sql.AppendLine("         ,H.納品書伝票番号");
                sql.AppendLine("     ) AS A");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON T.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     LEFT JOIN コーステーブル AS C");
                sql.AppendLine("         ON C.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND C.コースＣＤ = A.コースＣＤ");
                sql.AppendLine("         AND C.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS");
                sql.AppendLine("         ON VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B");
                sql.AppendLine("         ON B.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");

                if (_nohinPublish.納品書発行区分使用フラグ == true)
                {
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(T.納品書発行区分, 0) <> 0");    // 納品書発行区分がONの得意先のみ印刷対象にする
                }

                sql.AppendLine(" ORDER BY");
                sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@行数", SqlDbType.Int).Value = LINES_PER_PAGE;
                cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _nohinPublish.部署ＣＤ;
                cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _nohinPublish.配送日;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@税区分外税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Soto;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                cmd.Parameters.Add("@金額印字フラグ", SqlDbType.Bit).Value = (_nohinPublish.金額印字フラグ ?? false);
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@見出しフラグ", SqlDbType.Bit).Value = (_nohinPublish.見出しフラグ ?? false);

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

                return dt;
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
        }

        /// <summary>
        /// 売上データから親レコード（ヘッダレコード）を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="isPrintTaxIncludeTitle">納品書の消費税の見出しを「内消費税」にしたい場合はtrue。それ以外はfalse。</param>
        /// <returns>印刷対象が格納されたデータテーブル</returns>
        private DataTable GetHeaderUriage(SqlConnection con, bool isPrintTaxIncludeTitle)
        {
            try
            {
                DataTable dt = new DataTable();

                SqlCommand cmd = con.CreateCommand();

                // 親レコード（ヘッダレコード）を取得
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     A.納品書伝票番号");
                sql.AppendLine("     ,0 AS ページ番号");

                if (_nohinPublish.見出しフラグ == true)
                {
                    sql.AppendLine("     ,'納品書兼請求書(控)' AS 見出し控え");
                    sql.AppendLine("     ,'納品書兼請求書' AS 見出し");
                }
                else
                {
                    sql.AppendLine("     ,'納    品    書(控)' AS 見出し控え");
                    sql.AppendLine("     ,'納    品    書' AS 見出し");
                }

                sql.AppendLine("     ,T.郵便番号 AS 得意先郵便番号");
                sql.AppendLine("     ,T.住所1 AS 得意先住所1");
                sql.AppendLine("     ,T.住所2 AS 得意先住所2");
                sql.AppendLine("     ,T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名");
                sql.AppendLine("     ,DATALENGTH(T.得意先名 + ' ' + ISNULL(VS.請求書敬称名, '様')) AS 得意先名長さ");
                sql.AppendLine("     ,CASE WHEN T.お届け先住所1 IS NULL");
                sql.AppendLine("         THEN T.住所1 + ISNULL(T.住所2, '')");
                sql.AppendLine("         ELSE T.お届け先住所1 + ISNULL(T.お届け先住所2 ,'')");
                sql.AppendLine("     END AS 納品先住所");
                sql.AppendLine("     ,ISNULL(T.お届け先電話番号1, ISNULL(お届け先電話番号2, ISNULL(電話番号1, 電話番号2))) AS 納品先電話番号");
                sql.AppendLine("     ,ISNULL(A.税区分, @税区分内税) AS 税区分");
                sql.AppendLine("     ,CASE ISNULL(A.税区分, @税区分内税)");
                sql.AppendLine("         WHEN @税区分内税 THEN '全て税込です'");
                sql.AppendLine("         WHEN @税区分外税 THEN '全て税別です'");
                sql.AppendLine("         ELSE ''");
                sql.AppendLine("      END AS 税区分備考");
                sql.AppendLine("     ,ISNULL(A.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");
                sql.AppendLine("     ,ISNULL(T.売掛現金区分, @売掛現金区分現金) AS 売掛現金区分");
                sql.AppendLine("     ,A.企業ＣＤ");
                sql.AppendLine("     ,A.得意先ＣＤ");
                sql.AppendLine("     ,A.部署ＣＤ");
                sql.AppendLine("     ,A.配送日");
                sql.AppendLine("     ,RIGHT('0000' + convert(varchar, year(A.配送日)), 4) + '年' +");
                sql.AppendLine("      RIGHT('00' + convert(varchar, month(A.配送日)) ,2) + '月' +");
                sql.AppendLine("      RIGHT('00' + convert(varchar, day(A.配送日)), 2) + '日(' +");
                sql.AppendLine("      CASE DATEPART(WEEKDAY, A.配送日)");
                sql.AppendLine("         WHEN 1 THEN '日'");
                sql.AppendLine("         WHEN 2 THEN '月'");
                sql.AppendLine("         WHEN 3 THEN '火'");
                sql.AppendLine("         WHEN 4 THEN '水'");
                sql.AppendLine("         WHEN 5 THEN '木'");
                sql.AppendLine("         WHEN 6 THEN '金'");
                sql.AppendLine("         WHEN 7 THEN '土'");
                sql.AppendLine("      END  + ')' AS 納品日");
                sql.AppendLine("     ,A.明細件数");
                sql.AppendLine("     ,((A.明細件数 - 1) / @行数) + 1 AS ページ数");

                // 金額印字チェックボックスがONの場合は金額を取得
                if (_nohinPublish.金額印字フラグ == true)
                {
                    sql.AppendLine("     ,A.小計");
                    sql.AppendLine("     ,NULL AS 消費税");
                    sql.AppendLine("     ,A.合計金額");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS 小計");
                    sql.AppendLine("     ,NULL AS 消費税");
                    sql.AppendLine("     ,NULL AS 合計金額");
                }
                sql.AppendLine("     ,A.作成日 AS 注文日付");
                sql.AppendLine("     ,B.会社名称 AS 自社名称");
                sql.AppendLine("     ,B.郵便番号 AS 自社郵便番号");
                sql.AppendLine("     ,B.住所 AS 自社住所");
                sql.AppendLine("     ,B.電話番号 AS 自社電話番号");
                sql.AppendLine("     ,B.ＦＡＸ AS 自社FAX");
                sql.AppendLine("     ,B.登録番号");
                sql.AppendLine("     ,@金額印字フラグ AS 金額印字フラグ");
                sql.AppendLine("     ,@内消費税見出し AS 内消費税見出し");
                sql.AppendLine("     ,@見出しフラグ AS 納品書兼請求書フラグ");
                sql.AppendLine(" FROM");
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         H.企業ＣＤ");
                sql.AppendLine("         ,H.得意先ＣＤ");
                sql.AppendLine("         ,H.部署ＣＤ");
                sql.AppendLine("         ,H.売上日 AS 配送日");
                sql.AppendLine("         ,H.コースＣＤ");
                sql.AppendLine("         ,H.税区分");
                sql.AppendLine("         ,H.丸め処理区分");
                sql.AppendLine("         ,H.納品書伝票番号");
                sql.AppendLine("         ,MIN(H.作成日) AS 作成日");
                sql.AppendLine("         ,COUNT(M.明細番号) AS 明細件数");
                sql.AppendLine("         ,SUM(M.値引後金額) AS 小計");
                sql.AppendLine("         ,SUM(M.消費税額) AS 消費税");
                sql.AppendLine("         ,SUM(M.商品金額合計税込) AS 合計金額");
                sql.AppendLine("     FROM");
                sql.AppendLine("         売上ヘッダ AS H");
                sql.AppendLine("         INNER JOIN 売上明細 AS M");
                sql.AppendLine("             ON  H.企業ＣＤ   = M.企業ＣＤ");
                sql.AppendLine("             AND H.売上日     = M.売上日");
                sql.AppendLine("             AND H.得意先ＣＤ = M.得意先ＣＤ");
                sql.AppendLine("             AND H.部署ＣＤ   = M.部署ＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");
                sql.AppendLine("         AND H.売上日 = @売上日");

                if (_nohinPublish.コースＣＤFrom != null)
                {
                    sql.AppendLine("         AND H.コースＣＤ >= @コースＣＤFrom");
                    cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _nohinPublish.コースＣＤFrom;
                }

                if (_nohinPublish.コースＣＤTo != null)
                {
                    sql.AppendLine("         AND H.コースＣＤ <= @コースＣＤTo");
                    cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _nohinPublish.コースＣＤTo;
                }

                if (_nohinPublish.得意先ＣＤFrom != null)
                {
                    sql.AppendLine("         AND H.得意先ＣＤ >= @得意先ＣＤFrom");
                    cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤFrom;
                }

                if (_nohinPublish.得意先ＣＤTo != null)
                {
                    sql.AppendLine("         AND H.得意先ＣＤ <= @得意先ＣＤTo");
                    cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤTo;
                }

                if (_nohinPublish.印刷済みフラグ == null || (bool)_nohinPublish.印刷済みフラグ == false)
                {
                    sql.AppendLine("         AND H.納品書伝票番号 IS NULL");
                }

                sql.AppendLine("     GROUP BY");
                sql.AppendLine("         H.企業ＣＤ");
                sql.AppendLine("         ,H.得意先ＣＤ");
                sql.AppendLine("         ,H.部署ＣＤ");
                sql.AppendLine("         ,H.売上日");
                sql.AppendLine("         ,H.コースＣＤ");
                sql.AppendLine("         ,H.税区分");
                sql.AppendLine("         ,H.丸め処理区分");
                sql.AppendLine("         ,H.納品書伝票番号");
                sql.AppendLine("     ) AS A");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON T.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     LEFT JOIN コーステーブル AS C");
                sql.AppendLine("         ON C.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND C.コースＣＤ = A.コースＣＤ");
                sql.AppendLine("         AND C.得意先ＣＤ = A.得意先ＣＤ");
                sql.AppendLine("     LEFT JOIN V_請求書敬称 AS VS");
                sql.AppendLine("         ON VS.請求書敬称CD = T.請求書敬称");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS B");
                sql.AppendLine("         ON B.企業ＣＤ = A.企業ＣＤ");
                sql.AppendLine("         AND B.部署ＣＤ = A.部署ＣＤ");

                if (_nohinPublish.納品書発行区分使用フラグ == true)
                {
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(T.納品書発行区分, 0) <> 0");    // 納品書発行区分がONの得意先のみ印刷対象にする
                }

                sql.AppendLine(" ORDER BY");
                sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@行数", SqlDbType.Int).Value = LINES_PER_PAGE;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _nohinPublish.部署ＣＤ;
                cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _nohinPublish.売上日;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@税区分外税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Soto;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;
                cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                cmd.Parameters.Add("@金額印字フラグ", SqlDbType.Bit).Value = (_nohinPublish.金額印字フラグ ?? false);
                cmd.Parameters.Add("@内消費税見出し", SqlDbType.Bit).Value = isPrintTaxIncludeTitle;
                cmd.Parameters.Add("@見出しフラグ", SqlDbType.Bit).Value = (_nohinPublish.見出しフラグ ?? false);

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

                return dt;
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
        }

        /// <summary>
        /// 子レコード（明細レコード）を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <returns>印刷対象が格納されたデータテーブル</returns>
        private DataTable GetDetail(SqlConnection con)
        {
            if (_nohinPublish.配送日 != null)
            {
                // 配送日が指定されていれば注文から取得
                return GetDetailChumon(con);
            }
            else
            {
                // 売上日が指定されていれば売上から取得
                return GetDetailUriage(con);
            }
        }

        /// <summary>
        /// 注文データから子レコード（明細レコード）を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <returns>印刷対象が格納されたデータテーブル</returns>
        private DataTable GetDetailChumon(SqlConnection con)
        {
            try
            {
                DataTable dt = new DataTable();

                SqlCommand cmd = con.CreateCommand();

                // 子レコード（明細レコード）を取得
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     0 AS ページ番号");
                sql.AppendLine("     ,H.企業ＣＤ");
                sql.AppendLine("     ,H.注文区分");
                sql.AppendLine("     ,H.得意先ＣＤ");
                sql.AppendLine("     ,H.部署ＣＤ");
                sql.AppendLine("     ,H.配送日");
                sql.AppendLine("     ,H.コースＣＤ");
                sql.AppendLine("     ,H.納品書伝票番号");
                sql.AppendLine("     ,M.商品ＣＤ");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,M.喫食日");
                sql.AppendLine("     ,M.明細番号");
                sql.AppendLine("     ,M.商品数量 AS 数量");
                sql.AppendLine("     ,M.商品単価 AS 単価");

                // 金額印字チェックボックスがONの場合は金額を取得
                if (_nohinPublish.金額印字フラグ == true)
                {
                    sql.AppendLine("     ,M.商品金額合計 AS 金額");
                    sql.AppendLine("     ,M.計算税率");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS 金額");
                    sql.AppendLine("     ,NULL AS 計算税率");
                }
                sql.AppendLine("     ,0 AS 明細ソート用番号");
                sql.AppendLine(" FROM");
                sql.AppendLine("     注文ヘッダ AS H");
                sql.AppendLine("     INNER JOIN 注文明細 AS M");
                sql.AppendLine("         ON  H.企業ＣＤ   = M.企業ＣＤ");
                sql.AppendLine("         AND H.注文区分   = M.注文区分");
                sql.AppendLine("         AND H.喫食日     = M.喫食日");
                sql.AppendLine("         AND H.得意先ＣＤ = M.得意先ＣＤ");
                sql.AppendLine("         AND H.部署ＣＤ   = M.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = M.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON T.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     H.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("     AND H.注文区分 = @注文区分注文");
                sql.AppendLine("     AND H.部署ＣＤ = @部署ＣＤ");
                sql.AppendLine("     AND H.配送日 = @配送日");

                if (_nohinPublish.コースＣＤFrom != null)
                {
                    sql.AppendLine("     AND H.コースＣＤ >= @コースＣＤFrom");
                    cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _nohinPublish.コースＣＤFrom;
                }

                if (_nohinPublish.コースＣＤTo != null)
                {
                    sql.AppendLine("     AND H.コースＣＤ <= @コースＣＤTo");
                    cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _nohinPublish.コースＣＤTo;
                }

                if (_nohinPublish.得意先ＣＤFrom != null)
                {
                    sql.AppendLine("     AND H.得意先ＣＤ >= @得意先ＣＤFrom");
                    cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤFrom;
                }

                if (_nohinPublish.得意先ＣＤTo != null)
                {
                    sql.AppendLine("     AND H.得意先ＣＤ <= @得意先ＣＤTo");
                    cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤTo;
                }

                if (_nohinPublish.納品書発行区分使用フラグ == true)
                {
                    sql.AppendLine("     AND ISNULL(T.納品書発行区分, 0) <> 0");    // 納品書発行区分がONの得意先のみ印刷対象にする
                }

                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     H.得意先ＣＤ");
                sql.AppendLine("     ,CASE WHEN H.納品書伝票番号 IS NULL THEN 1 ELSE 0 END");
                sql.AppendLine("     ,H.納品書伝票番号");
                sql.AppendLine("     ,M.喫食日");
                sql.AppendLine("     ,M.明細番号");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _nohinPublish.部署ＣＤ;
                cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = _nohinPublish.配送日;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

                return dt;
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
        }

        /// <summary>
        /// 売上データから子レコード（明細レコード）を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <returns>印刷対象が格納されたデータテーブル</returns>
        private DataTable GetDetailUriage(SqlConnection con)
        {
            try
            {
                DataTable dt = new DataTable();

                SqlCommand cmd = con.CreateCommand();

                // 子レコード（明細レコード）を取得
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     0 AS ページ番号");
                sql.AppendLine("     ,H.企業ＣＤ");
                sql.AppendLine("     ,H.得意先ＣＤ");
                sql.AppendLine("     ,H.部署ＣＤ");
                sql.AppendLine("     ,H.売上日 AS 配送日");
                sql.AppendLine("     ,H.コースＣＤ");
                sql.AppendLine("     ,H.納品書伝票番号");
                sql.AppendLine("     ,M.商品ＣＤ");
                sql.AppendLine("     ,S.商品名");
                sql.AppendLine("     ,M.売上日 AS 喫食日");
                sql.AppendLine("     ,M.明細番号");
                sql.AppendLine("     ,M.商品数量 AS 数量");
                sql.AppendLine("     ,M.商品単価 AS 単価");

                // 金額印字チェックボックスがONの場合は金額を取得
                if (_nohinPublish.金額印字フラグ == true)
                {
                    sql.AppendLine("     ,M.値引後金額 AS 金額");
                    sql.AppendLine("     ,M.計算税率");
                }
                else
                {
                    sql.AppendLine("     ,NULL AS 金額");
                    sql.AppendLine("     ,NULL AS 計算税率");
                }
                sql.AppendLine("     ,0 AS 明細ソート用番号");
                sql.AppendLine(" FROM");
                sql.AppendLine("     売上ヘッダ AS H");
                sql.AppendLine("     INNER JOIN 売上明細 AS M");
                sql.AppendLine("         ON  H.企業ＣＤ   = M.企業ＣＤ");
                sql.AppendLine("         AND H.売上日     = M.売上日");
                sql.AppendLine("         AND H.得意先ＣＤ = M.得意先ＣＤ");
                sql.AppendLine("         AND H.部署ＣＤ   = M.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON");
                sql.AppendLine("         S.企業ＣＤ = M.企業ＣＤ");
                sql.AppendLine("         AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("     LEFT JOIN 得意先マスタ AS T");
                sql.AppendLine("         ON T.企業ＣＤ = H.企業ＣＤ");
                sql.AppendLine("         AND T.得意先ＣＤ = H.得意先ＣＤ");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     H.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("     AND H.部署ＣＤ = @部署ＣＤ");
                sql.AppendLine("     AND H.売上日 = @売上日");

                if (_nohinPublish.コースＣＤFrom != null)
                {
                    sql.AppendLine("     AND H.コースＣＤ >= @コースＣＤFrom");
                    cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _nohinPublish.コースＣＤFrom;
                }

                if (_nohinPublish.コースＣＤTo != null)
                {
                    sql.AppendLine("     AND H.コースＣＤ <= @コースＣＤTo");
                    cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _nohinPublish.コースＣＤTo;
                }

                if (_nohinPublish.得意先ＣＤFrom != null)
                {
                    sql.AppendLine("     AND H.得意先ＣＤ >= @得意先ＣＤFrom");
                    cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤFrom;
                }

                if (_nohinPublish.得意先ＣＤTo != null)
                {
                    sql.AppendLine("     AND H.得意先ＣＤ <= @得意先ＣＤTo");
                    cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _nohinPublish.得意先ＣＤTo;
                }

                if (_nohinPublish.納品書発行区分使用フラグ == true)
                {
                    sql.AppendLine("     AND ISNULL(T.納品書発行区分, 0) <> 0");    // 納品書発行区分がONの得意先のみ印刷対象にする
                }

                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     H.得意先ＣＤ");
                sql.AppendLine("     ,CASE WHEN H.納品書伝票番号 IS NULL THEN 1 ELSE 0 END");
                sql.AppendLine("     ,H.納品書伝票番号");
                sql.AppendLine("     ,M.売上日");
                sql.AppendLine("     ,M.明細番号");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _nohinPublish.部署ＣＤ;
                cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _nohinPublish.売上日;

                cmd.CommandText = sql.ToString();

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                adp.Fill(dt);

                return dt;
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
        }

        /// <summary>
        /// 納品書伝票番号を付与
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="headerDt">注文ヘッダが格納されたデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateDenpyoNo(SqlConnection con, DataTable headerDt)
        {
            if (_nohinPublish.配送日 != null)
            {
                // 配送日が指定されていれば注文(と売上)を更新
                return UpdateDenpyoNoChumon(con, headerDt);
            }
            else
            {
                // 売上日が指定されていれば売上を更新
                return UpdateDenpyoNoUriage(con, headerDt);
            }
        }

        /// <summary>
        /// 注文ヘッダの納品書伝票番号を付与
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="headerDt">注文ヘッダが格納されたデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateDenpyoNoChumon(SqlConnection con, DataTable headerDt)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            SqlTransaction tran = con.BeginTransaction();

            try
            {
                foreach (DataRow dr in headerDt.Rows)
                {
                    int? denpyoNo = dr["納品書伝票番号"] as int?;
                    if (denpyoNo != null)
                    {
                        continue;
                    }

                    // 付与する納品書伝票番号を取得
                    denpyoNo = GetNextDenpyoNo(con, tran);
                    dr["納品書伝票番号"] = denpyoNo;

                    // 注文ヘッダを更新
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = "\r\n UPDATE 注文ヘッダ" +
                                      "\r\n SET  納品書伝票番号 = @納品書伝票番号" +
                                      "\r\n  WHERE 企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n    AND 注文区分     = @注文区分" +
                                      "\r\n    AND 得意先ＣＤ   = @得意先ＣＤ" +
                                      "\r\n    AND 部署ＣＤ     = @部署ＣＤ" +
                                      "\r\n    AND 配送日       = @配送日" +        // 喫食日は条件に加えない
                                      "\r\n    AND 納品書伝票番号 IS NULL";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)dr["注文区分"];
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)dr["得意先ＣＤ"];
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)dr["部署ＣＤ"];
                    cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = (DateTime)dr["配送日"];
                    cmd.Parameters.Add("@納品書伝票番号", SqlDbType.Int).Value = denpyoNo;

                    log.Debug(cmd.CommandTextWithParameters());
                    cmd.ExecuteNonQuery();

                    // 操作履歴に保存
                    History.Save(con, tran, this.Text, "注文ヘッダ", string.Format("納品書伝票番号=[{0}]", denpyoNo), History.ProcessType.Update);

                    // 注文に該当する売上についても更新
                    cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = "\r\n UPDATE 売上ヘッダ" +
                                      "\r\n SET 納品書伝票番号 = CH.納品書伝票番号" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS UH" +
                                      "\r\n     INNER JOIN 注文ヘッダ AS CH" +
                                      "\r\n         ON CH.企業ＣＤ = UH.企業ＣＤ" +
                                      "\r\n         AND CH.喫食日 = UH.売上日" +
                                      "\r\n         and CH.得意先ＣＤ = UH.得意先ＣＤ" +
                                      "\r\n         AND CH.部署ＣＤ = UH.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     CH.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND CH.注文区分 = @注文区分" +
                                      "\r\n     AND CH.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND CH.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND CH.配送日 = @配送日" +
                                      "\r\n     AND UH.納品書伝票番号 IS NULL";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)dr["注文区分"];
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)dr["得意先ＣＤ"];
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)dr["部署ＣＤ"];
                    cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = (DateTime)dr["配送日"];

                    log.Debug(cmd.CommandTextWithParameters());
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("注文ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 売上ヘッダの納品書伝票番号を付与
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="headerDt">注文ヘッダが格納されたデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateDenpyoNoUriage(SqlConnection con, DataTable headerDt)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            SqlTransaction tran = con.BeginTransaction();

            try
            {
                foreach (DataRow dr in headerDt.Rows)
                {
                    int? denpyoNo = dr["納品書伝票番号"] as int?;
                    if (denpyoNo != null)
                    {
                        continue;
                    }

                    // 付与する納品書伝票番号を取得
                    denpyoNo = GetNextDenpyoNo(con, tran);
                    dr["納品書伝票番号"] = denpyoNo;

                    // 売上ヘッダを更新
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = "\r\n UPDATE 売上ヘッダ" +
                                      "\r\n SET  納品書伝票番号 = @納品書伝票番号" +
                                      "\r\n  WHERE 企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n    AND 得意先ＣＤ   = @得意先ＣＤ" +
                                      "\r\n    AND 部署ＣＤ     = @部署ＣＤ" +
                                      "\r\n    AND 売上日       = @売上日" +
                                      "\r\n    AND 納品書伝票番号 IS NULL";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)dr["得意先ＣＤ"];
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)dr["部署ＣＤ"];
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = (DateTime)dr["配送日"];   // 売上日を配送日という名前で取得している
                    cmd.Parameters.Add("@納品書伝票番号", SqlDbType.Int).Value = denpyoNo;

                    log.Debug(cmd.CommandTextWithParameters());
                    cmd.ExecuteNonQuery();

                    // 操作履歴に保存
                    History.Save(con, tran, this.Text, "売上ヘッダ", string.Format("納品書伝票番号=[{0}]", denpyoNo), History.ProcessType.Update);
                }

                tran.Commit();

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 付与する納品書伝票番号を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns></returns>
        private int GetNextDenpyoNo(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                int denpyoNo;

                // 付与する納品書伝票番号を取得
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                // 注文と売上の両方から最大の納品書伝票番号を取得
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     ISNULL(MAX(A.納品書伝票番号), 0) + 1 as 納品書伝票番号" +
                                  "\r\n FROM" +
                                  "\r\n     (SELECT" +
                                  "\r\n         ISNULL(MAX(納品書伝票番号), 0) AS 納品書伝票番号" +
                                  "\r\n     FROM" +
                                  "\r\n         注文ヘッダ" +
                                  "\r\n     WHERE" +
                                  "\r\n         企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     UNION" +
                                  "\r\n     SELECT" +
                                  "\r\n         ISNULL(MAX(納品書伝票番号), 0) AS 納品書伝票番号" +
                                  "\r\n     FROM" +
                                  "\r\n         売上ヘッダ" +
                                  "\r\n     WHERE" +
                                  "\r\n         企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     ) AS A";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    denpyoNo = (int)reader["納品書伝票番号"];
                }

                return denpyoNo;
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
        }

        #endregion
    }
}

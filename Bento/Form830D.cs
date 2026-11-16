using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// Web用注文不可日設定詳細
    /// </summary>
    public partial class Form830D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMONBI_COLUMN   = 0;    // 注文日列インデックス
        private const int HOLIDAY_COLUMN    = 1;    // 祝日名列インデックス
        private const int DISABLE_COLUMN    = 2;    // 注文不可列インデックス
        private const int CHUMON_UMU_COLUMN = 3;    // 注文有無列インデックス

        /// <summary>
        /// 得意先の休日の背景色
        /// </summary>
        private readonly Color HOLIDAY_BACK_COLOR = Color.LightPink;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// Web用注文不可日設定検索条件
        /// </summary>
        private WebDisableOrderCriteria _webDisableOrderCriteria;

        /// <summary>
        /// Web用注文不可日設定検索条件検索実行時スナップショット
        /// </summary>
        private WebDisableOrderCriteria _webDisableOrderCriteriaSnapShot;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        // Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// （休日取得用の）得意先マスタ
        /// </summary>
        private Tokuisaki _tokuisaki;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="webDisableOrderCriteria">Web用注文不可日設定検索条件</param>
        public Form830D(WebDisableOrderCriteria webDisableOrderCriteria)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _webDisableOrderCriteria = new WebDisableOrderCriteria(webDisableOrderCriteria);

                if (_webDisableOrderCriteria.注文年 == null)
                {
                    _webDisableOrderCriteria.注文年 = DateTime.Today.Year;
                }
                if (_webDisableOrderCriteria.注文月 == null)
                {
                    _webDisableOrderCriteria.注文月 = DateTime.Today.Month;
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
        private void Form830D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.world_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread1.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                webDisableOrderCriteriaBindingSource.DataSource = _webDisableOrderCriteria;

                // 過去注文ラベルを非表示
                lblPast.Visible = false;

                // 表示ボタン押下
                btnQuery.PerformClick();
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
        private void Form830D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }
                    btnSave.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form830D_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // このフォーム以外の要因で閉じられようとしている場合は閉じる
                if (e.CloseReason != CloseReason.UserClosing)
                {
                    return;
                }

                // 登録or削除済み？
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                // フォームが閉じられようとするとき、スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                if (fpSpread1.EditMode == true)
                {
                    fpSpread1.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                else
                {
                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                }

                // 初期状態と異なる？
                if (HasChange())
                {
                    DialogResult r = MessageBox.Show(this, "変更されています。更新しますか？", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            if (CanSave() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (Save())
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            else
                            {
                                e.Cancel = true;
                            }
                            return;

                        case DialogResult.Cancel:
                            e.Cancel = true;
                            return;
                    }
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
        /// 得意先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCD.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuisakiCD_FormClosed);
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuisakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    _webDisableOrderCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCD);
                    ReadBinding(lblTokuisakiName);
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_webDisableOrderCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_webDisableOrderCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _webDisableOrderCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                DateTime chumonMonth = new DateTime((int)_webDisableOrderCriteria.注文年, (int)_webDisableOrderCriteria.注文月, 1);
                if (chumonMonth > warnDate)
                {
                    if (MessageBox.Show(this, "注文年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        bindingSource1.DataSource = null;
                        _webDisableOrderCriteriaSnapShot = null;
                        return;
                    }
                }

                // 過去の場合はラベルを表示する
                lblPast.Visible = (chumonMonth < currentMonth);

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 得意先マスタを取得（休日取得用）
                _tokuisaki = Tokuisaki.GetTokuisaki((int)_webDisableOrderCriteria.得意先ＣＤ);

                // タイトルバーに得意先名略称を表示
                if (string.IsNullOrEmpty(_tokuisaki.得意先名略称))
                {
                    this.Text = "Web用注文不可日設定";
                }
                else
                {
                    this.Text = string.Format("{0} - Web用注文不可日設定", _tokuisaki.得意先名略称);
                }

                // Spreadの内部状態が壊れてColumnCountやRowCountを設定するとIndexOutOfRangeExceptionが発生することがあるので、それを回避すべく、DataSourceをリセットする
                bindingSource1.DataSource = null;

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                // データ本体を取得
                DataTable dt = new DataTable("Web用注文不可日設定");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n With T(I) AS" +         // -- 0～30までを取得する再帰CTE（共通テーブル式）
                                      "\r\n (SELECT" +
                                      "\r\n     0 AS I" +
                                      "\r\n UNION ALL" +
                                      "\r\n SELECT" +
                                      "\r\n     I + 1" +
                                      "\r\n FROM" +
                                      "\r\n     T" +
                                      "\r\n WHERE" +
                                      "\r\n     I < 30" +
                                      "\r\n )" +
                                      "\r\n SELECT" +               // -- ② ①で取得した日付に該当する注文を取得。同時にwhere句で年月を指定する
                                      "\r\n     A.日付" +
                                      "\r\n     ,@得意先ＣＤ AS 得意先ＣＤ" +
                                      "\r\n     ,S.名称 AS 祝日" +
                                      "\r\n     ,CASE WHEN B.注文不可日 IS NULL THEN  0 ELSE    1 END AS 注文不可" +
                                      "\r\n     ,CASE WHEN H.得意先ＣＤ IS NULL THEN '' ELSE '有' END AS 注文有無" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +          // -- ① 0～30までを取得する再帰CTEを用いて、指定した日付以降の日付を取得
                                      "\r\n         CONVERT(date, DateAdd(day, I, @注文日)) AS 日付" +
                                      "\r\n     FROM" +
                                      "\r\n         T" +
                                      "\r\n     ) AS A" +
                                      "\r\n     LEFT JOIN WEB_M_注文不可設定 AS B" +
                                      "\r\n         ON B.注文不可日 = A.日付" +
                                      "\r\n         AND B.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND B.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T" +
                                      "\r\n         ON T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND T.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n         AND ISNULL(T.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN 注文ヘッダ AS H" +
                                      "\r\n         ON H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND H.注文区分 = @注文区分注文" +
                                      "\r\n         AND H.喫食日 = A.日付" +
                                      "\r\n         AND H.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n         AND H.部署ＣＤ = T.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 祝日マスタ AS S" +
                                      "\r\n         ON S.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND S.対象日 = A.日付" +
                                      "\r\n WHERE" +
                                      "\r\n     YEAR(A.日付) = @注文年" +
                                      "\r\n AND MONTH(A.日付) = @注文月" +
                                      "\r\n ORDER BY" +
                                      "\r\n     日付";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _webDisableOrderCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = new DateTime((int)_webDisableOrderCriteria.注文年, (int)_webDisableOrderCriteria.注文月, 1);
                    cmd.Parameters.Add("@注文年", SqlDbType.Int).Value = _webDisableOrderCriteria.注文年;
                    cmd.Parameters.Add("@注文月", SqlDbType.Int).Value = _webDisableOrderCriteria.注文月;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "Web用注文不可日設定";

                    log.Info("Web用注文不可日設定リストを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(CHUMONBI_COLUMN,   "日付");
                fpSpread1_Sheet1.BindDataColumn(HOLIDAY_COLUMN,    "祝日");
                fpSpread1_Sheet1.BindDataColumn(DISABLE_COLUMN,    "注文不可");
                fpSpread1_Sheet1.BindDataColumn(CHUMON_UMU_COLUMN, "注文有無");

                // 休日・祝日に着色する
                // 日曜・祝日は日付の文字色を赤にする
                // 得意先の休日は行全体をピンクにする
                // 祝日 かつ 祝日配送区分がOFFの場合は行全体をピンクにする
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();

                    DataRow dr = dt.Rows[rowIndex];
                    DateTime? date = dr["日付"] as DateTime?;
                    string holiday = dr["祝日"] as string;

                    if (date != null)
                    {
                        // 文字色の設定
                        // 日曜 または 祝日？
                        if (((DateTime)date).DayOfWeek == DayOfWeek.Sunday || string.IsNullOrEmpty(holiday) == false)
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, CHUMONBI_COLUMN].ForeColor = Color.Red;
                        }
                        else
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, CHUMONBI_COLUMN].ForeColor = Color.Empty;  // ResetForeColor()が効かないので
                        }

                        // 背景色の設定
                        // 得意先の休日？
                        bool isHoliday = _tokuisaki.GetHoliday(((DateTime)date).DayOfWeek);
                        if (isHoliday)
                        {
                            fpSpread1_Sheet1.Rows[rowIndex].BackColor = HOLIDAY_BACK_COLOR;
                        }
                        else
                        {
                            // 祝日 かつ 祝日配送区分がOFF？
                            if (string.IsNullOrEmpty(holiday) == false && (_tokuisaki.祝日配送区分 ?? false) == false)
                            {
                                fpSpread1_Sheet1.Rows[rowIndex].BackColor = HOLIDAY_BACK_COLOR;
                            }
                        }
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, DISABLE_COLUMN);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _webDisableOrderCriteriaSnapShot = new WebDisableOrderCriteria(_webDisableOrderCriteria);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
            finally
            {
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread1.EditMode)
                {
                    fpSpread1.CancelCellEditing();
                }
                else
                {
                    fpSpread1.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= DISABLE_COLUMN)
                    {
                        newColumnIndex = DISABLE_COLUMN;
                        // ただし最終行は合計行なので移動しない
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 1)
                        {
                            newRowIndex++;
                        }
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread1_Sheet1.ActiveColumnIndex)
                {
                    case DISABLE_COLUMN:      // 注文不可
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// SPREAD編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(fpSpread1Edit_KeyDown);
            }
        }

        /// <summary>
        /// SPREAD編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread1, fpSpread1_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread1.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }
        }

        /// <summary>
        /// Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1Edit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread1.EditMode)
                    {
                        fpSpread1.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread1.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= DISABLE_COLUMN)
                    {
                        newColumnIndex = DISABLE_COLUMN;
                        // ただし最終行は合計行なので移動しない
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 1)
                        {
                            newRowIndex++;
                        }
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)bindingSource1.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CanSave() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;
                if (Save())
                {
                    // 再読込
                    Reload();
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                saveFileDialog1.FileName = this.Text;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = saveFileDialog1.FileName;
                    FarPoint.Excel.ExcelSaveFlags excelSaveFlags = FarPoint.Excel.ExcelSaveFlags.SaveCustomColumnHeaders;
                    if (fileName.ToLower().EndsWith("xlsx"))
                    {
                        excelSaveFlags |= FarPoint.Excel.ExcelSaveFlags.UseOOXMLFormat;
                    }
                    fpSpread1.SaveExcel(fileName, excelSaveFlags);
                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// Web用注文不可日設定リストが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // Web用注文不可日設定リストを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["Web用注文不可日設定"];

            // 変更をチェック
            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                DataRow dr = dt.Rows[rowIndex];
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSave()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_webDisableOrderCriteria.HasErrors)
                {
                    MessageBox.Show(this, _webDisableOrderCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_webDisableOrderCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_webDisableOrderCriteria.Equals(_webDisableOrderCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                return true;
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
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    UserData userData = UserData.getInstance();

                    // 全削除→全追加
                    SqlCommand cmdDel = con.CreateCommand();
                    cmdDel.CommandText = "DELETE FROM WEB_M_注文不可設定 " +
                                         "WHERE 企業ＣＤ = @企業ＣＤ " +
                                         "  AND 得意先ＣＤ = @得意先ＣＤ" +
                                         "  AND YEAR(注文不可日) = @注文年" +
                                         "  AND MONTH(注文不可日) = @注文月";
                    cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmdDel.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _webDisableOrderCriteria.得意先ＣＤ;
                    cmdDel.Parameters.Add("@注文年", SqlDbType.Int).Value = _webDisableOrderCriteria.注文年;
                    cmdDel.Parameters.Add("@注文月", SqlDbType.Int).Value = _webDisableOrderCriteria.注文月;
                    cmdDel.Transaction = tran;
                    log.Debug(cmdDel.CommandTextWithParameters());
                    cmdDel.ExecuteNonQuery();

                    History.ProcessType historyProcess = History.ProcessType.Create;

                    // 行数分ループ
                    DataSet ds = (DataSet)bindingSource1.DataSource;
                    DataTable dt = ds.Tables["Web用注文不可日設定"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (((int)dr["注文不可"]) != 0)
                        {
                            // 作成担当者などがデータテーブルに無い（取得できない）ので、データレコードを渡すコンストラクタで生成できない。
                            WebDisableOrder webDisableOrder = new WebDisableOrder();
                            webDisableOrder.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                            webDisableOrder.注文不可日 = dr["日付"] as DateTime?;
                            bool ret = webDisableOrder.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;

                            if (ret == false)
                            {
                                MessageBox.Show(this, webDisableOrder.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                            History.Save(con, tran, this.Text, "Web用注文不可日設定マスタ", webDisableOrder.Code, historyProcess);
                        }
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("Web用注文不可日設定マスタを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
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
        }

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                btnQuery.PerformClick();        // 検索ボタン押下
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

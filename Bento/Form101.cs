using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 日配持出入力
    /// </summary>
    public partial class Form101 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_CD_COLUMN   = 0;       // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN = 1;       // コース名列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN   = 2;    // 商品(部品)名列開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 日配持出入力検索条件
        /// </summary>
        private NippaiMochidashiCriteria _nippaiMochidashiCriteria;

        /// <summary>
        /// 日配持出入力検索条件検索実行時スナップショット
        /// </summary>
        private NippaiMochidashiCriteria _nippaiMochidashiCriteriaSnapShot;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        // Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        private bool _isF2forSpreadEdit = false;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = SYOHIN_START_COLUMN;

        /// <summary>
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        /// <summary>
        /// 更新・削除用モバイル_持ち出し入力データテーブル
        /// </summary>
        private DataTable _mobileMochidashiDt = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form101()
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
        private void Form101_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_flatbed_ico;

                lblCount.Text = "";

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

                // 常に更新
                _IsNewRecord = false;

                // コンボボックスの選択肢生成
                ComboUtil.SetCourseKubun(cboCourseKbn);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _nippaiMochidashiCriteria = new NippaiMochidashiCriteria();

                nippaiMochidashiBindingSource.DataSource = _nippaiMochidashiCriteria;
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form101_Shown(object sender, EventArgs e)
        {
            // 持出日にフォーカス
            dtpMochidashibi.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form101_KeyDown(object sender, KeyEventArgs e)
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
        private void Form101_FormClosing(object sender, FormClosingEventArgs e)
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

                // 初期状態と異なる？
                if (HasChange())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_nippaiMochidashiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_nippaiMochidashiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _nippaiMochidashiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 持出日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_nippaiMochidashiCriteria.持出日 > warnDate)
                {
                    if (MessageBox.Show(this, "持出日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _nippaiMochidashiCriteriaSnapShot = null;
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //************************************************************
                // 主要部品リストマスタにある商品名を列として追加し、
                // コースマスタにある得意先を行とする
                // データそのものはモバイル_持ち出し入力から取得する
                //************************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                DataTable dt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     MAIN.コースＣＤ" +
                                      "\r\n     ,MAIN.コース名" +
                                      "\r\n     ,MAIN.商品ＣＤ" +
                                      "\r\n     ,MCDS.個数" +
                                      "\r\n     ,0 AS 合計行" +
                                      "\r\n FROM" +
                                      // -- ①駆動表（コースと商品の全組み合わせ）
                                      "\r\n     (SELECT" +
                                      "\r\n         C.コースＣＤ," +
                                      "\r\n         C.コース名," +
                                      "\r\n         P.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                                      "\r\n         (SELECT" +
                                      "\r\n             CM.企業ＣＤ" +
                                      "\r\n             ,CM.部署ＣＤ" +
                                      "\r\n             ,CM.コースＣＤ" +
                                      "\r\n             ,CM.コース名" +
                                      "\r\n         FROM" +
                                      "\r\n             コースマスタ AS CM" +
                                      "\r\n             LEFT JOIN" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     コースＣＤ" +
                                      "\r\n                 FROM" +
                                      "\r\n                     臨時コースマスタ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND コース区分 = @コース区分" +
                                      "\r\n                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                     AND ISNULL(工場フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                     AND 実施日 = @持出日" +
                                      "\r\n                 ) AS RM" +
                                      "\r\n                 ON RM.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND CM.コース区分 = @コース区分" +
                                      "\r\n             AND CM.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND ISNULL(CM.工場フラグ, 0) = 0" +
                                      "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             RM.企業ＣＤ" +
                                      "\r\n             ,RM.部署ＣＤ" +
                                      "\r\n             ,RM.コースＣＤ" +
                                      "\r\n             ,RM.コース名" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コーステーブル AS RT" +
                                      "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n         WHERE" +
                                      "\r\n             RM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND RM.コース区分 = @コース区分" +
                                      "\r\n             AND RM.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND ISNULL(RM.工場フラグ, 0) = 0" +
                                      "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n             AND RM.実施日 = @持出日" +
                                      "\r\n         ) AS C" +
                                      "\r\n         INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = C.企業ＣＤ AND P.部署ＣＤ = C.部署ＣＤ" +
                                      "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n     ) AS MAIN" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ③保存済みのデータ
                                      "\r\n     (SELECT" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n         ,個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_持ち出し入力" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n         持出日 = @持出日" +
                                      "\r\n     ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ" +
                                      // -- ④合計行
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     NULL AS コースＣＤ" +
                                      "\r\n    ,'合計' AS コース名" +
                                      "\r\n    ,-1 AS 商品ＣＤ" +
                                      "\r\n    ,-1 AS 個数" +
                                      "\r\n    , 1 AS 合計行" +
                                      "\r\n ORDER BY" +
                                      "\r\n     合計行," +
                                      "\r\n     コースＣＤ," +
                                      "\r\n     商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _nippaiMochidashiCriteria.コース区分;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _nippaiMochidashiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@持出日", SqlDbType.Date).Value = _nippaiMochidashiCriteria.持出日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("日配持出入力リストを検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("日配持出入力");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["コースＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["コース名"]);
                // インポート
                cldt.Import(dt, "商品ＣＤ", "個数");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "日配持出入力";

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", bindingSource1.Count);

                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _nippaiMochidashiCriteriaSnapShot = new NippaiMochidashiCriteria(_nippaiMochidashiCriteria);

                // 更新対象のモバイル_持ち出し入力を取得
                GetMobileMochidashi();
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
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = SYOHIN_START_COLUMN;
                        // ただし最終行は合計行なので移動しない
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 2)
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
                    case COURSE_CD_COLUMN:   // コースＣＤ
                    case COURSE_NAME_COLUMN: // コース名
                        colIndex = -1;
                        break;

                    default:
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
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
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = SYOHIN_START_COLUMN;
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
            // セルの文字色を黒(デフォルト)に戻す
            fpSpread1_Sheet1.Cells[e.Row, e.Column].ResetForeColor();

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
                    // 再読み込み
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
        /// 列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetSyohinTable()
        {
            DataTable dt = new DataTable("商品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.商品ＣＤ," +
                                      "\r\n     S.商品名略称" +
                                      "\r\n FROM" +
                                      "\r\n     主要部品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _nippaiMochidashiCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("商品一覧を取得しました。");
                }

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
        /// 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="dt"></param>
        private void SetSpreadColumn(DataTable syohinDt, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = 0D;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
                    columnIndex++;
                }
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
        /// モバイル_持ち出し入力を取得します。
        /// </summary>
        private void GetMobileMochidashi()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,A.持出日" +
                                      "\r\n     ,A.個数" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                                      "\r\n     (SELECT" +
                                      "\r\n         C.企業ＣＤ" +
                                      "\r\n         ,C.コースＣＤ" +
                                      "\r\n         ,C.コース区分" +
                                      "\r\n         ,C.コース名" +
                                      "\r\n         ,C.部署ＣＤ" +
                                      "\r\n         ,C.担当者ＣＤ" +
                                      "\r\n         ,C.工場フラグ" +
                                      "\r\n         ,C.無効フラグ" +
                                      "\r\n         ,C.作成担当者ＣＤ" +
                                      "\r\n         ,C.作成日" +
                                      "\r\n         ,C.修正担当者ＣＤ" +
                                      "\r\n         ,C.修正日" +
                                      "\r\n     FROM" +
                                      "\r\n         コースマスタ AS C" +
                                      "\r\n         LEFT JOIN " +
                                      "\r\n             (SELECT" +
                                      "\r\n                 コースＣＤ" +
                                      "\r\n             FROM" +
                                      "\r\n                 臨時コースマスタ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                 AND 実施日 = @持出日" +
                                      "\r\n             ) AS RM" +
                                      "\r\n             ON RM.コースＣＤ = C.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         C.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND ISNULL(C.無効フラグ, 0) = 0" +
                                      "\r\n         AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         RM.企業ＣＤ" +
                                      "\r\n         ,RM.コースＣＤ" +
                                      "\r\n         ,RM.コース区分" +
                                      "\r\n         ,RM.コース名" +
                                      "\r\n         ,RM.部署ＣＤ" +
                                      "\r\n         ,RM.担当者ＣＤ" +
                                      "\r\n         ,RM.工場フラグ" +
                                      "\r\n         ,RM.無効フラグ" +
                                      "\r\n         ,RM.作成担当者ＣＤ" +
                                      "\r\n         ,RM.作成日" +
                                      "\r\n         ,RM.修正担当者ＣＤ" +
                                      "\r\n         ,RM.修正日" +
                                      "\r\n     FROM" +
                                      "\r\n         臨時コーステーブル AS RT" +
                                      "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n     WHERE" +
                                      "\r\n         RM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n         AND RM.実施日 = @持出日" +
                                      "\r\n     ) AS M" +
                                      "\r\n     LEFT JOIN モバイル_持ち出し入力 AS A ON A.企業ＣＤ = M.企業ＣＤ AND A.コースＣＤ = M.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M.コース区分 = @コース区分" +
                                      "\r\n     AND A.持出日 = @持出日" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _nippaiMochidashiCriteria.コース区分;
                    cmd.Parameters.Add("@持出日", SqlDbType.Date).Value = _nippaiMochidashiCriteria.持出日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    _mobileMochidashiDt = new DataTable("モバイル_持ち出し入力");
                    adp.Fill(_mobileMochidashiDt);

                    log.Info("モバイル_持ち出し入力を検索しました。");
                }
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
        /// 日配持出入力リストが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 日配持出入力リストを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["日配持出入力"];

            // 最終行(=合計行)を除いて、変更をチェック
            for (int rowIndex = 0; rowIndex < dt.Rows.Count - 1; rowIndex++)
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
                if (_nippaiMochidashiCriteria.HasErrors)
                {
                    MessageBox.Show(this, _nippaiMochidashiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_nippaiMochidashiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_nippaiMochidashiCriteria.Equals(_nippaiMochidashiCriteriaSnapShot) == false)
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
        /// <param name="shain">社員</param>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool? ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        //********************************************************************************************************
                        // ・cldtの行をループしながら以下を実施
                        //   ・商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //   ・cldtの行（コース）の、商品ＣＤに該当する列の値を取得
                        //   ・列の値がNull かつ、コースＣＤと商品ＣＤに該当するモバイル_持ち出し入力が有れば削除
                        //   ・列の値がNullではなく、コースＣＤと商品ＣＤに該当するモバイル_持ち出し入力が有れば更新。無ければ追加
                        //********************************************************************************************************

                        //***************************************************************************
                        // cldtの行をループしつつ、商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //***************************************************************************
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["日配持出入力"];

                        for (int rowIndex = 0; rowIndex < cldt.Rows.Count; rowIndex++)
                        {
                            int? courseCD = cldt.Rows[rowIndex]["コースＣＤ"] as int?;  // 行に該当するコースＣＤを取得
                            if (courseCD == null)       // 合計行(=最終行)はコースＣＤが無いので無視する
                            {
                                continue;
                            }

                            foreach (DataRow syohinDr in _syohinDt.Rows)
                            {
                                int syohinCD = (int)syohinDr["商品ＣＤ"];               // 列に該当する商品ＣＤを取得

                                MobileMochidashi mobileMochidashi = null;
                                // コースＣＤと商品ＣＤに該当するモバイル_持ち出し入力を取得
                                DataRow[] drs = _mobileMochidashiDt.Select(string.Format("コースＣＤ={0} and 商品ＣＤ={1}", courseCD, syohinCD));
                                if (drs != null && drs.Length != 0)
                                {
                                    mobileMochidashi = new MobileMochidashi(drs[0]);
                                }

                                ret = null;

                                //*****************************************************
                                // cldtの行（コース）の、商品ＣＤに該当する列の値を取得
                                //*****************************************************
                                int? suryo = (int?)cldt[rowIndex, syohinCD];
                                if (suryo == null)
                                {
                                    // ***********************************************************************************
                                    // 数量が入力されてなく、かつ、コースと商品に該当するモバイル_持ち出し入力が有れば削除
                                    // ***********************************************************************************
                                    if (mobileMochidashi != null)
                                    {
                                        // 有るので削除
                                        historyProcess = History.ProcessType.Delete;
                                        ret = mobileMochidashi.Delete(con, tran);
                                    }
                                }
                                else
                                {
                                    // *****************************************************************************
                                    // 数量が入力されており、コースと商品に該当するモバイル_持ち出し入力が有れば更新
                                    // *****************************************************************************
                                    if (mobileMochidashi != null)
                                    {
                                        historyProcess = History.ProcessType.Update;
                                        mobileMochidashi.個数 = suryo;
                                        ret = mobileMochidashi.Update(con, tran);
                                    }
                                    else
                                    {
                                        // *******************************************************************************
                                        // 数量が入力されており、コースと商品に該当するモバイル_持ち出し入力が無ければ追加
                                        // *******************************************************************************
                                        historyProcess = History.ProcessType.Create;
                                        mobileMochidashi = new MobileMochidashi();
                                        mobileMochidashi.コースＣＤ = courseCD;
                                        mobileMochidashi.商品ＣＤ = syohinCD;
                                        mobileMochidashi.持出日 = _nippaiMochidashiCriteria.持出日;
                                        mobileMochidashi.個数 = suryo;
                                        ret = mobileMochidashi.Insert(con, tran);
                                    }
                                }

                                // 追加も更新もしなかった場合は無視する
                                if (ret != null)
                                {
                                    if (ret == false)
                                    {
                                        MessageBox.Show(this, mobileMochidashi.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }
                                    History.Save(con, tran, this.Text, "モバイル_持ち出し入力", mobileMochidashi.Code, historyProcess);
                                }
                            }
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("日配持出入力を{0}しました。", historyProcess.DisplayName());
                        MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (BentoException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        log.Error(ex);
                        History.Error(ex);
                        throw new BentoException(ex);
                    }
                }
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 検索ボタン押下;

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread1.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = activeRowIndex;
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
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

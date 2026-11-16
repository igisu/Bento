using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 主要商品リストマスタ詳細
    /// </summary>
    public partial class Form705D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYOHIN_GROUP_COLUMN = 0;  // グループ列インデックス
        private const int SYOHIN_CD_COLUMN    = 1;  // 商品ＣＤ列インデックス
        private const int REFERENCE_COLUMN    = 2;  // 参照ボタン列インデックス
        private const int SYOHIN_NAME_COLUMN  = 3;  // 商品名列インデックス
        private const int SYUYO_COLUMN        = 4;  // 主要表示列インデックス
        private const int INVALID_COLUMN      = 5;  // 無効フラグ列インデックス
        private const int DELETE_COLUMN       = 6;  // 削除ボタン列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用モバイル主要商品リストデータ
        /// </summary>
        private SyuyoSyohinCriteria _syuyoSyohinCriteria;

        /// <summary>
        /// 検索条件入力用モバイル主要商品リストデータ検索実行時スナップショット
        /// </summary>
        private SyuyoSyohinCriteria _syuyoSyohinCriteriaSnapShot;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// スプレッドのコンボボックス（F2,Enter制御用）
        /// </summary>
        private FarPoint.Win.FpCombo _fpCombo;

        /// <summary>
        /// スプレッドのコンボボックスでEnterキーが押下されたか
        /// </summary>
        private bool _isEnterforFpCombo = false;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="syuyoSyohinCriteria"></param>
        public Form705D(SyuyoSyohinCriteria syuyoSyohinCriteria)
        {
            try
            {
                InitializeComponent();

                // 検索条件として取得
                _syuyoSyohinCriteria = syuyoSyohinCriteria;
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
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form705D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

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

                // コンボボックスの選択肢生成
                cboShiten.SelectedValueChanged -= cboShiten_SelectedValueChanged;   // 支店ＣＤコンボイベントをいったん削除

                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetBusyo(cboBusyo, _syuyoSyohinCriteria.支店ＣＤ);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                syuyoSyohinBindingSource.DataSource = _syuyoSyohinCriteria;

                // スプレッドのグループ名のコンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbGroup = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetSyohinGroup(cbGroup);
                cbGroup.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbGroup.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[SYOHIN_GROUP_COLUMN].CellType = cbGroup;

                // 常に更新
                _IsNewRecord = false;

                btnQuery.PerformClick();        // 検索ボタン押下
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
            finally
            {
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboShiten.SelectedValueChanged += new EventHandler(cboShiten_SelectedValueChanged);  // 支店ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form705D_KeyDown(object sender, KeyEventArgs e)
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
                    btnQuit.Focus();
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form705D_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 支店コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShiten_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 部署コンボの選択肢を、支店にぶら下がるものに再設定する
                ComboUtil.SetBusyo(cboBusyo, (int?)cboShiten.SelectedValue);

                // 部署コンボを初期化
                _syuyoSyohinCriteria.部署ＣＤ = -1;
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_syuyoSyohinCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_syuyoSyohinCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _syuyoSyohinCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.商品グループＣＤ");
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,A.商品ＣＤ AS 商品ＣＤorg");
                    sql.AppendLine("     ,S1.商品名略称");
                    sql.AppendLine("     ,S1.商品名記号");
                    sql.AppendLine("     ,A.主要商品フラグ");
                    sql.AppendLine("     ,A.WEB注文不可フラグ");
                    sql.AppendLine("     ,A.WEB注文可能期限日数");
                    sql.AppendLine("     ,A.WEB注文可能曜日");
                    sql.AppendLine("     ,A.WEB注文開始日");
                    sql.AppendLine("     ,A.WEB注文終了日");

                    sql.AppendLine("     ,A.WEB注文締切時間");
                    sql.AppendLine("     ,A.WEB注文締切分");

                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     主要商品リストマスタ AS A");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS S1 ON S1.企業ＣＤ = @企業ＣＤ AND ISNULL(S1.無効フラグ, 0) = 0 AND A.商品ＣＤ = S1.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 支店マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND B.支店ＣＤ = S.支店ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");

                    if (_syuyoSyohinCriteria.支店ＣＤ != null && _syuyoSyohinCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine("     AND B.支店ＣＤ = @支店ＣＤ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _syuyoSyohinCriteria.支店ＣＤ;
                    }

                    if (_syuyoSyohinCriteria.部署ＣＤ != null && _syuyoSyohinCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("     AND A.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _syuyoSyohinCriteria.部署ＣＤ;
                    }
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.商品グループＣＤ");
                    sql.AppendLine("     ,A.商品ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("主要商品リストマスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "主要商品リストマスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("主要商品リストマスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SYOHIN_GROUP_COLUMN, "商品グループＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN, "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread1_Sheet1.BindDataColumn(SYUYO_COLUMN, "主要商品フラグ");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN, "無効フラグ");

                fpSpread1.Focus();

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _syuyoSyohinCriteriaSnapShot = new SyuyoSyohinCriteria(_syuyoSyohinCriteria);

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

                SyuyoSyohin syuyoSyohin = new SyuyoSyohin(e.Row);

                // 商品CDの場合は商品名を取得
                if (e.Column.ColumnName == "商品ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["商品名略称"] = null;
                    if (syuyoSyohin.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)syuyoSyohin.商品ＣＤ);
                        if (syohin != null)
                        {
                            e.Row["商品名略称"] = syohin.商品名略称;
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

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
            // F1？
            if (e.KeyCode == Keys.F1)
            {
                OpenForm901BySpread();          // 商品選択ダイアログを表示
            }

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
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
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
                    case SYOHIN_CD_COLUMN:      // 商品ＣＤ
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
        /// Spread編集モードON
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
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread1.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
            }
        }

        /// <summary>
        /// Spread編集モードOFF
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

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // ドロップダウンについても各イベントのハンドルを解除します
                _fpCombo.KeyDown -= new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown -= new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
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
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
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
        /// Spreadコンボボックスキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合に呼ばれた場合は、リストを表示する
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                    _fpCombo.DroppedDown = true;
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

            //Enterキーが押された場合にフラグを立てる
            _isEnterforFpCombo = (e.KeyCode == Keys.Enter);
        }

        /// <summary>
        /// Spreadコンボボックスドロップダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_DropDown(object sender, FarPoint.Win.DropDownEventArgs e)
        {
            //Enterキーを契機とする場合は、DropDown処理をキャンセルする
            if (_isEnterforFpCombo)
            {
                _isEnterforFpCombo = false;
                e.Cancel = true;
                // 次のセルへ移動
                SendKeys.Send("{Tab}");
            }
        }

        /// <summary>
        /// Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 参照ボタン？
                if (e.Column == REFERENCE_COLUMN)
                {
                    OpenForm901BySpread();
                }

                // 無効チェックボックス？
                if (e.Column == INVALID_COLUMN)
                {
                    if (fpSpread1_Sheet1.Cells[e.Row, INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[e.Row].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[e.Row].ForeColor = SystemColors.GrayText;
                    }
                    else
                    {
                        fpSpread1_Sheet1.Rows[e.Row].ResetBackColor();
                        fpSpread1_Sheet1.Rows[e.Row].ResetForeColor();
                    }
                }

                // 削除ボタン？
                if (e.Column == DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                    dr.Delete();
                }
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
        /// Spreadから商品選択ダイアログを表示する
        /// </summary>
        private void OpenForm901BySpread()
        {
            try
            {
                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCD_FormClosed);
                formSyohinDlg.ShowDialog();
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
        /// 商品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form901SyohinCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form901 form = (Form901)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, newColumnIndex);

                    Syohin syohin = form.SelectedSyohin;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, SYOHIN_CD_COLUMN].Value = syohin.商品ＣＤ;   // 商品CDセルに値を設定→dt_ColumnChanged()が呼ばれる
                }
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
                    // 再読み込み
                    Reload();
                }
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
            catch (EmuException ex)
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
        /// 主要商品リストマスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 主要商品リストマスタを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["主要商品リストマスタ"];

            foreach (DataRow dr in dt.Rows)
            {
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
            bool hasError = false;

            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_syuyoSyohinCriteria.Equals(_syuyoSyohinCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "支店または部署が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["主要商品リストマスタ"];

                int syuyoCount = 0;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    SyuyoSyohin syuyoSyohin = new SyuyoSyohin(dr);
                    syuyoSyohin.部署ＣＤ = _syuyoSyohinCriteriaSnapShot.部署ＣＤ;

                    // エラーがある？
                    if (syuyoSyohin.HasErrors)
                    {
                        // 各セルにエラーをセット
                        dr.SetColumnError("商品グループＣＤ", syuyoSyohin["商品グループＣＤ"]);
                        dr.SetColumnError("商品ＣＤ", syuyoSyohin["商品ＣＤ"]);
                        hasError = true;
                    }

                    //************************************************************************************
                    // 主要商品リストマスタについて、以下をチェック
                    // ・主要表示は部署につき６個まで
                    // ・コミット時、親が無効でないかチェック
                    //************************************************************************************

                    // 主要表示を計数
                    if (syuyoSyohin.主要商品フラグ == true)
                    {
                        syuyoCount++;
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 主要商品として表示できる最大数を超えていないか？
                int syuyoSyohinMax = SystemMaster.GetSyuyoSyohinMax();
                if (syuyoCount > syuyoSyohinMax)
                {
                    MessageBox.Show(this, string.Format("主要表示できる商品は１つの部署につき{0}個までです。", syuyoSyohinMax), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                return true;
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
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Delete;
                List<SyuyoSyohin> syuyoSyohinList = new List<SyuyoSyohin>();        // 主要商品リストマスタリスト（主要部品リストマスタへの反映用）
                List<int> syuyoSyohinCDList = new List<int>();                      // 上記リストの商品CDリスト（重複チェック用）

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        DataTable dt = ds.Tables["主要商品リストマスタ"];

                        foreach (DataRow dr in dt.Rows)
                        {
                            SyuyoSyohin syuyoSyohin = new SyuyoSyohin(dr);

                            switch (dr.RowState)
                            {
                                case DataRowState.Added:        // 追加
                                    // 親が無効でないかチェック
                                    ret = IsInvalidSyohinGourp(con, tran, (int)syuyoSyohin.商品グループＣＤ);
                                    if (ret == true)
                                    {
                                        tran.Rollback();
                                        MessageBox.Show(this, string.Format("商品グループＣＤ({0})は既に無効になっています。このグループに商品を含めることはできません。", syuyoSyohin.商品グループＣＤ),
                                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return false;
                                    }

                                    syuyoSyohin.部署ＣＤ = _syuyoSyohinCriteriaSnapShot.部署ＣＤ;   // 部署ＣＤを設定する
                                    syuyoSyohin.WEB注文不可フラグ = false;                          // WEB注文不可フラグの初期値(OFF)を設定
                                    syuyoSyohin.WEB注文可能曜日 = SyuyoSyohin.ALL_ON;               // WEB注文可能曜日の初期値(ON)を設定
                                    historyProcess = History.ProcessType.Create;
                                    ret = syuyoSyohin.Insert(con, tran);
                                    break;

                                case DataRowState.Modified:     // 変更
                                    // 親が無効でないかチェック
                                    ret = IsInvalidSyohinGourp(con, tran, (int)syuyoSyohin.商品グループＣＤ);
                                    if (ret == true)
                                    {
                                        tran.Rollback();
                                        MessageBox.Show(this, string.Format("商品グループＣＤ({0})は既に無効になっています。このグループに商品を含めることはできません。", syuyoSyohin.商品グループＣＤ),
                                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return false;
                                    }

                                    historyProcess = History.ProcessType.Update;
                                    ret = syuyoSyohin.Update(con, tran);
                                    break;

                                case DataRowState.Unchanged:    // 変更なし
                                    // 親が無効でないかチェック
                                    ret = IsInvalidSyohinGourp(con, tran, (int)syuyoSyohin.商品グループＣＤ);
                                    if (ret == true)
                                    {
                                        tran.Rollback();
                                        MessageBox.Show(this, string.Format("商品グループＣＤ({0})は既に無効になっています。このグループに商品を含めることはできません。", syuyoSyohin.商品グループＣＤ),
                                                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return false;
                                    }
                                    ret = true;
                                    break;

                                case DataRowState.Deleted:      // 削除
                                    historyProcess = History.ProcessType.Delete;
                                    ret = syuyoSyohin.Delete(con, tran);
                                    break;

                                default:
                                    ret = true;
                                    break;
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, syuyoSyohin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            History.Save(con, tran, this.Text, "主要商品リストマスタ", syuyoSyohin.Code, historyProcess);

                            //***********************************************
                            // 主要部品リストマスタへの反映用リスト追加処理
                            //***********************************************
                            // （削除｜無効）以外のレコードについて、主要商品リストマスタの商品を部品（＝主食と副食）に分解する
                            // 分解して得られた商品は、もとの主要商品リストマスタの主要商品フラグを保持したいので、
                            // 主要商品リストマスタの形式で保持してリストに追加する
                            if (dr.RowState != DataRowState.Deleted && (syuyoSyohin.無効フラグ ?? false) == false)
                            {
                                Syohin syohin = Syohin.GetSyohin((int)syuyoSyohin.商品ＣＤ);
                                if (syohin != null)
                                {
                                    // 副食を追加
                                    if ((syohin.副食ＣＤ ?? 0) > 0)
                                    {
                                        SyuyoSyohin syuyoSyohinSub = new SyuyoSyohin();
                                        syuyoSyohinSub.企業ＣＤ = syuyoSyohin.企業ＣＤ;
                                        syuyoSyohinSub.部署ＣＤ = syuyoSyohin.部署ＣＤ;
                                        syuyoSyohinSub.主要商品フラグ = syuyoSyohin.主要商品フラグ;
                                        syuyoSyohinSub.商品ＣＤ = syohin.副食ＣＤ;
                                        // リストに追加（追加済みであれば主要商品フラグを更新する）
                                        if (syuyoSyohinCDList.Contains((int)syuyoSyohinSub.商品ＣＤ) == false)
                                        {
                                            syuyoSyohinList.Add(syuyoSyohinSub);
                                            syuyoSyohinCDList.Add((int)syuyoSyohinSub.商品ＣＤ);
                                        }
                                        else
                                        {
                                            if ((syuyoSyohinSub.主要商品フラグ ?? false) == true)
                                            {
                                                syuyoSyohinList[syuyoSyohinCDList.IndexOf((int)syuyoSyohinSub.商品ＣＤ)].主要商品フラグ = true;
                                            }
                                        }
                                    }

                                    // 主食を追加
                                    if ((syohin.主食ＣＤ ?? 0) > 0)
                                    {
                                        SyuyoSyohin syuyoSyohinMain = new SyuyoSyohin();
                                        syuyoSyohinMain.企業ＣＤ = syuyoSyohin.企業ＣＤ;
                                        syuyoSyohinMain.部署ＣＤ = syuyoSyohin.部署ＣＤ;
                                        syuyoSyohinMain.主要商品フラグ = syuyoSyohin.主要商品フラグ;
                                        syuyoSyohinMain.商品ＣＤ = syohin.主食ＣＤ;
                                        // リストに追加（追加済みであれば主要商品フラグを更新する）
                                        if (syuyoSyohinCDList.Contains((int)syuyoSyohinMain.商品ＣＤ) == false)
                                        {
                                            syuyoSyohinList.Add(syuyoSyohinMain);
                                            syuyoSyohinCDList.Add((int)syuyoSyohinMain.商品ＣＤ);
                                        }
                                        else
                                        {
                                            if ((syuyoSyohinMain.主要商品フラグ ?? false) == true)
                                            {
                                                syuyoSyohinList[syuyoSyohinCDList.IndexOf((int)syuyoSyohinMain.商品ＣＤ)].主要商品フラグ = true;
                                            }
                                        }
                                    }

                                    // 主食も副食も持たない場合は、自分自身を追加
                                    if ((syohin.副食ＣＤ ?? 0) <= 0 && (syohin.主食ＣＤ ?? 0) <= 0)
                                    {
                                        // リストに追加済みであれば無視する
                                        if (syuyoSyohinCDList.Contains((int)syuyoSyohin.商品ＣＤ) == false)
                                        {
                                            syuyoSyohinList.Add(syuyoSyohin);
                                            syuyoSyohinCDList.Add((int)syuyoSyohin.商品ＣＤ);
                                        }
                                        else
                                        {
                                            if ((syuyoSyohin.主要商品フラグ ?? false) == true)
                                            {
                                                syuyoSyohinList[syuyoSyohinCDList.IndexOf((int)syuyoSyohin.商品ＣＤ)].主要商品フラグ = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //***********************************
                        // 主要部品リストマスタへの反映処理
                        //***********************************
                        ret = UpdateSyuyoSyohin(con, tran, syuyoSyohinList);
                        if (ret == false)
                        {
                            return false;
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("主要商品リストマスタを{0}しました。", historyProcess.DisplayName());
                        MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (EmuException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        log.Error(ex);
                        History.Error(ex);
                        throw new EmuException(ex);
                    }
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            btnQuery.PerformClick();        // 検索ボタン押下;
        }

        /// <summary>
        /// 商品グループＣＤに該当する商品グループマスタが有効かどうかチェック
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="syohinGroupCD">商品グループＣＤ</param>
        /// <returns>商品グループＣＤに該当する商品グループマスタの無効フラグの値</returns>
        private bool IsInvalidSyohinGourp(SqlConnection con, SqlTransaction tran, int syohinGroupCD)
        {
            bool ret = true;        // 商品グループＣＤに該当するレコードが存在しない場合は無効扱い

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     ISNULL(無効フラグ,0) AS 無効フラグ" +
                                  "\r\n FROM" +
                                  "\r\n     商品グループマスタ" +
                                  "\r\n WHERE" +
                                  "\r\n     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND 商品グループＣＤ = @商品グループＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = syohinGroupCD;

                log.Debug(cmd.CommandTextWithParameters());

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ret = (bool)reader["無効フラグ"];
                    }
                }

                return ret;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// 主要商品リストマスタを主要部品リストマスタに反映する
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tran"></param>
        /// <param name="syuyoSyohinList"></param>
        /// <returns></returns>
        private bool UpdateSyuyoSyohin(SqlConnection con, SqlTransaction tran, List<SyuyoSyohin> syuyoSyohinList)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;

            try
            {
                syuyoSyohinList.Sort(CompareSyuyoSyohin);   // 主要商品リストマスタリストをソート

                // いったん削除
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE FROM 主要部品リストマスタ" +
                                  "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                                  "\r\n   AND 部署ＣＤ = @部署ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _syuyoSyohinCriteriaSnapShot.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // 主要部品として表示できる最大数を取得（主要商品ではなく主要部品であることに注意）
                int syuyoBuhinMax = SystemMaster.GetSyuyoBuhinMax();

                int syuyoCount = 0;
                bool ret;
                // リストの内容を追加
                foreach (SyuyoSyohin syuyoSyohin in syuyoSyohinList)
                {
                    SyuyoBuhin syuyoBuhin = new SyuyoBuhin();
                    syuyoBuhin.企業ＣＤ = syuyoSyohin.企業ＣＤ;
                    syuyoBuhin.部署ＣＤ = syuyoSyohin.部署ＣＤ;
                    syuyoBuhin.商品ＣＤ = syuyoSyohin.商品ＣＤ;

                    // 主要部品として表示できる最大数を超えていないか？
                    if (syuyoCount < syuyoBuhinMax && syuyoSyohin.主要商品フラグ == true)
                    {
                        syuyoBuhin.主要商品フラグ = true;
                        syuyoCount++;
                    }
                    else
                    {
                        syuyoBuhin.主要商品フラグ = false;
                    }
                    ret = syuyoBuhin.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, syuyoBuhin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "主要部品リストマスタ", syuyoBuhin.Code, historyProcess);
                }

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("主要部品リストマスタが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("主要商品リストマスタの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("主要部品リストマスタの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 主要商品リストマスタソート用比較メソッド
        /// </summary>
        /// <param name="x">比較する最初のオブジェクトです。</param>
        /// <param name="y">比較する 2 番目のオブジェクト。</param>
        /// <returns>x が y より小さい場合は 0 未満。x と y が等しい場合は 0。x が y より大きい場合は 0超過</returns>
        private static int CompareSyuyoSyohin(SyuyoSyohin x, SyuyoSyohin y)
        {
            // 商品ＣＤの昇順で比較
            if ((x.商品ＣＤ ?? 0) != (y.商品ＣＤ ?? 0))
            {
                return (x.商品ＣＤ ?? 0) - (y.商品ＣＤ ?? 0);
            }

            return 0;
        }

        #endregion

    }
}

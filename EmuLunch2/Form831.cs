using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using System.Drawing;

namespace Emu.Bento
{
    /// <summary>
    /// Web注文用商品設定
    /// </summary>
    public partial class Form831 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_COLUMN       = 0;   // 部署列インデックス
        private const int SYOHIN_CD_COLUMN   = 1;   // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 2;   // 商品名列インデックス
        private const int DISABLE_COLUMN     = 3;   // 注文不可列インデックス
        private const int DAYS_COLUMN        = 4;   // 注文可能期限日数列インデックス

        private const int HOUR_COLUMN = 5;   // 統一注文締切時刻時列インデックス
        private const int MINUTE_COLUMN = 6;   // 統一注文締切時刻分列インデックス

        private const int SUNDAY_COLUMN = 7;   // 注文可能日祝列インデックス
        private const int MONDAY_COLUMN = 8;   // 注文可能月曜列インデックス
        private const int TUESDAY_COLUMN = 9;   // 注文可能火曜列インデックス
        private const int WEDNESDAY_COLUMN = 10;   // 注文可能水曜列インデックス
        private const int THURSDAY_COLUMN = 11;   // 注文可能木曜列インデックス
        private const int FRIDAY_COLUMN = 12;  // 注文可能金曜列インデックス
        private const int SATURDAY_COLUMN = 13;  // 注文可能土曜列インデックス
        private const int START_DATE_COLUMN = 14;  // 注文開始日列インデックス
        private const int END_DATE_COLUMN = 15;  // 注文終了日列インデックス
        private const int INVALID_COLUMN = 16;  // 無効フラグ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// Web注文用商品設定検索条件
        /// </summary>
        private WebDisableSyohinCriteria _webDisableSyohinCriteria;

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
        public Form831()
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
        private void Form831_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.world_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

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

                // スプレッドの注文可能期限日数のコンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbDays = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetWebOrderLimitDays(cbDays);
                cbDays.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbDays.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[DAYS_COLUMN].CellType = cbDays;

                // スプレッドの締切時刻のコンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbHour = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.Set(cbHour,
                    new string[] { "", "0時", "1時", "2時", "3時", "4時", "5時", "6時", "7時", "8時", "9時", "10時", "11時", "12時", "13時", "14時", "15時", "16時", "17時", "18時", "19時", "20時", "21時", "22時", "23時" },
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 });
                cbHour.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbHour.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[HOUR_COLUMN].CellType = cbHour;

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbMinute = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.Set(cbMinute,
                    new string[] { "", "0分", "10分", "15分", "20分", "30分", "40分", "45分", "50分", "59分" },
                    new int[] { -1, 0, 10, 15, 20, 30, 40, 45, 50, 59 });
                cbMinute.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbMinute.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[MINUTE_COLUMN].CellType = cbMinute;

                // スプレッドの最終列（無効フラグ）を非表示にする
                fpSpread1_Sheet1.Columns[INVALID_COLUMN].Visible = false;

                //***********************************************************************************
                // Spreadの行のロック(編集不可)を有効にするためにシートのプロテクト(保護)を有効にする
                // ただし、Excel出力するとシートのあらゆる書式や設定が変更できなくなるので、
                // Excel出力の前後でプロテクト状態を切り替える(□)
                //***********************************************************************************
                fpSpread1_Sheet1.Protect = true;

                // 常に更新
                _IsNewRecord = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _webDisableSyohinCriteria = new WebDisableSyohinCriteria();

                // 部署CDコンボの初期値を設定
                _webDisableSyohinCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                webDisableSyohinCriteriaBindingSource.DataSource = _webDisableSyohinCriteria;
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
        private void Form831_KeyDown(object sender, KeyEventArgs e)
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
        private void Form831_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_webDisableSyohinCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_webDisableSyohinCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _webDisableSyohinCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,A.商品グループＣＤ");
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,A.商品ＣＤ AS 商品ＣＤorg");
                    sql.AppendLine("     ,S1.商品名");
                    sql.AppendLine("     ,A.主要商品フラグ");
                    sql.AppendLine("     ,A.WEB注文不可フラグ");
                    sql.AppendLine("     ,A.WEB注文可能期限日数");
                    sql.AppendLine("     ,A.WEB注文可能曜日");

                    sql.AppendLine("     ,A.WEB注文締切時間");
                    sql.AppendLine("     ,A.WEB注文締切分");

                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 1, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能日祝");
                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 2, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能月曜");
                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 3, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能火曜");
                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 4, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能水曜");
                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 5, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能木曜");
                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 6, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能金曜");
                    sql.AppendLine("     ,cast(CASE WHEN A.WEB注文可能曜日 IS NULL THEN 0 WHEN SUBSTRING(A.WEB注文可能曜日, 7, 1) = '1' THEN 1 ELSE 0 END as bit) AS WEB注文可能土曜");
                    sql.AppendLine("     ,A.WEB注文開始日");
                    sql.AppendLine("     ,A.WEB注文終了日");
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
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_webDisableSyohinCriteria.部署ＣＤ != null && _webDisableSyohinCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _webDisableSyohinCriteria.部署ＣＤ;
                    }

                    sql.AppendLine("order by A.部署ＣＤ, A.商品ＣＤ");

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

                fpSpread1_Sheet1.BindDataColumn(BUSYO_COLUMN,       "部署名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");
                fpSpread1_Sheet1.BindDataColumn(DISABLE_COLUMN,     "WEB注文不可フラグ");
                fpSpread1_Sheet1.BindDataColumn(DAYS_COLUMN,        "WEB注文可能期限日数");

                fpSpread1_Sheet1.BindDataColumn(HOUR_COLUMN, "WEB注文締切時間");
                fpSpread1_Sheet1.BindDataColumn(MINUTE_COLUMN, "WEB注文締切分");

                fpSpread1_Sheet1.BindDataColumn(SUNDAY_COLUMN,      "WEB注文可能日祝");
                fpSpread1_Sheet1.BindDataColumn(MONDAY_COLUMN,      "WEB注文可能月曜");
                fpSpread1_Sheet1.BindDataColumn(TUESDAY_COLUMN,     "WEB注文可能火曜");
                fpSpread1_Sheet1.BindDataColumn(WEDNESDAY_COLUMN,   "WEB注文可能水曜");
                fpSpread1_Sheet1.BindDataColumn(THURSDAY_COLUMN,    "WEB注文可能木曜");
                fpSpread1_Sheet1.BindDataColumn(FRIDAY_COLUMN,      "WEB注文可能金曜");
                fpSpread1_Sheet1.BindDataColumn(SATURDAY_COLUMN,    "WEB注文可能土曜");
                fpSpread1_Sheet1.BindDataColumn(START_DATE_COLUMN,  "WEB注文開始日");
                fpSpread1_Sheet1.BindDataColumn(END_DATE_COLUMN,    "WEB注文終了日");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN,     "無効フラグ");

                // 無効の場合は、すべての列を非活性(使用不可)にする(□)
                // Web注文不可の場合は、注文可能期限日数～注文終了日を非活性(使用不可)にする(□)
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                {
                    if (fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value as bool? == true)
                    {
                        for (int columnIndex = BUSYO_COLUMN; columnIndex <= END_DATE_COLUMN; columnIndex++)
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Locked = true;       // 入力不可にする(□)
                            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].BackColor = SystemColors.ControlDark;
                        }
                    }
                    else
                    {
                        if (fpSpread1_Sheet1.Cells[rowIndex, DISABLE_COLUMN].Value as bool? == true)
                        {
                            for (int columnIndex = DAYS_COLUMN; columnIndex <= END_DATE_COLUMN; columnIndex++)
                            {
                                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Locked = true;       // 入力不可にする(□)
                                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = SystemColors.GrayText;
                                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].BackColor = SystemColors.ControlLight;
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

                // 部署の注文可能期限日数を表示
                ShowWebOrderLimit();
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

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している

                SyuyoSyohin syuyoSyohin = new SyuyoSyohin(e.Row);

                // 締切時刻時の場合
                if (e.Column.ColumnName == "WEB注文締切時間")
                {
                    e.Row.SetColumnError(e.Column, null);
                    if (syuyoSyohin.WEB注文締切時間 != null && syuyoSyohin.WEB注文締切時間 != -1)
                    {
                        // 締切時刻分が未入力なら0で初期化
                        if (syuyoSyohin.WEB注文締切分 == null || syuyoSyohin.WEB注文締切分 < 0)
                        {
                            e.Row["WEB注文締切分"] = 0;
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
        /// Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            // 注文不可チェックボックス？
            if (e.Column == DISABLE_COLUMN)
            {
                // 注文不可？
                if (fpSpread1_Sheet1.Cells[e.Row, DISABLE_COLUMN].Value as bool? == true)
                {
                    for (int columnIndex = DAYS_COLUMN; columnIndex <= END_DATE_COLUMN; columnIndex++)
                    {
                        fpSpread1_Sheet1.Cells[e.Row, columnIndex].Locked = true;       // 入力不可にする(□)
                        fpSpread1_Sheet1.Cells[e.Row, columnIndex].ForeColor = SystemColors.GrayText;
                        fpSpread1_Sheet1.Cells[e.Row, columnIndex].BackColor = SystemColors.ControlLight;
                    }
                }
                else
                {
                    for (int columnIndex = DAYS_COLUMN; columnIndex <= END_DATE_COLUMN; columnIndex++)
                    {
                        fpSpread1_Sheet1.Cells[e.Row, columnIndex].Locked = false;
                        fpSpread1_Sheet1.Cells[e.Row, columnIndex].ResetForeColor();
                        fpSpread1_Sheet1.Cells[e.Row, columnIndex].ResetBackColor();
                    }
                }
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
                    if (newColumnIndex >= END_DATE_COLUMN)
                    {
                        newColumnIndex = DISABLE_COLUMN;
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
                    case DAYS_COLUMN:           // 注文可能期限日数
                    case START_DATE_COLUMN:     // 注文開始日
                    case END_DATE_COLUMN:       // 注文終了日
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    // Locked=true なセルについては、その値を消さないようにする(□)
                    if (fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Locked == false)
                    {
                        fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                    }
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

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread1.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
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

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= END_DATE_COLUMN)
                    {
                        newColumnIndex = DISABLE_COLUMN;
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
                fpSpread1_Sheet1.Protect = false;       // シートのプロテクトを解除する(□)

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
            finally
            {
                fpSpread1_Sheet1.Protect = true;        // シートのプロテクトを有効にする(□)
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
        /// Web部署注文設定を取得（部署の注文可能期限日数を表示）
        /// </summary>
        private void ShowWebOrderLimit()
        {
            try
            {
                lblBushoLimit.Text = "";

                if (_webDisableSyohinCriteria.部署ＣＤ == null || _webDisableSyohinCriteria.部署ＣＤ < 0)
                {
                    return;
                }

                // 得意先の部署に該当するWeb部署注文設定を取得
                WebOrderSetting webOrderSetting = WebOrderSetting.GetWebOrderSetting((int)_webDisableSyohinCriteria.部署ＣＤ);
                if (webOrderSetting == null)
                {
                    return;
                }

                if (webOrderSetting.注文可能期限日数 == null)
                {
                    lblBushoLimit.Text = "部署設定  (未設定)";
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = con.CreateCommand();
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     WEB注文可能期限日数名" +
                                          "\r\n FROM" +
                                          "\r\n     V_WEB注文可能期限日数" +
                                          "\r\n WHERE" +
                                          "\r\n     WEB注文可能期限日数CD = @WEB注文可能期限日数CD";

                        cmd.Parameters.Add("@WEB注文可能期限日数CD", SqlDbType.Int).Value = (int)webOrderSetting.注文可能期限日数;

                        log.Debug(cmd.CommandTextWithParameters());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblBushoLimit.Text = string.Format("部署設定  {0}", reader["WEB注文可能期限日数名"].ToString());
                            }
                        }
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

                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    return false;
                }

                DataTable dt = ds.Tables["主要商品リストマスタ"];

                // 締切時刻が1件でも設定されている場合はtrue
                bool hasLimit = false;

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

                    // エラーがある？
                    if (syuyoSyohin.HasErrors)
                    {
                        dr.SetColumnError("WEB注文可能期限日数", syuyoSyohin["WEB注文可能期限日数"]);

                        dr.SetColumnError("WEB注文締切時間", syuyoSyohin["WEB注文締切時間"]);
                        dr.SetColumnError("WEB注文締切分", syuyoSyohin["WEB注文締切分"]);

                        dr.SetColumnError("WEB注文開始日", syuyoSyohin["WEB注文開始日"]);
                        dr.SetColumnError("WEB注文終了日", syuyoSyohin["WEB注文終了日"]);
                        hasError = true;
                    }

                    if (!hasLimit && syuyoSyohin.HasLimit)
                    {
                        hasLimit = true;
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }


                // 締切日時が1個でも設定されている場合は警告ダイアログを表示
                if (hasLimit)
                {
                    DialogResult result = MessageBox.Show(
                        "統一注文締切時刻が設定されている商品があります。\nこのまま更新しますか?\n\n※この設定はWeb得意先マスタの「個別締切時刻」よりも優先されます。",
                        "確認",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );
                    if (result != DialogResult.Yes)
                    {
                        return false;
                    }
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
                History.ProcessType historyProcess = History.ProcessType.Update;

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
                                    historyProcess = History.ProcessType.Create;
                                    //ret = syuyoSyohin.Insert(con, tran);
                                    if (log.IsWarnEnabled) log.WarnFormat("主要商品リストマスタ追加({0})", syuyoSyohin.Code);
                                    ret = true;
                                    break;

                                case DataRowState.Modified:     // 変更
                                    historyProcess = History.ProcessType.Update;
                                    ret = syuyoSyohin.Update(con, tran);
                                    break;

                                case DataRowState.Deleted:      // 削除
                                    historyProcess = History.ProcessType.Delete;
                                    //ret = syuyoSyohin.Delete(con, tran);
                                    if (log.IsWarnEnabled) log.WarnFormat("主要商品リストマスタ削除({0})", syuyoSyohin.Code);
                                    ret = true;
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

        #endregion
    }
}

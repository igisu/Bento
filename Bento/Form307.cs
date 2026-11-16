using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 大袋ラベル印刷
    /// </summary>
    public partial class Form307 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // 印刷履歴
        private const int HISTORY_SYOKUJI_KBN_COLUMN     = 0;   // 食事区分列インデックス
        private const int HISTORY_PRINT_SHEETS_COLUMN    = 1;   // 印刷枚数列インデックス
        private const int HISTORY_PRINT_COUNT_COLUMN     = 2;   // 印刷回数列インデックス
        private const int HISTORY_PRINT_DATE_TIME_COLUMN = 3;   // 直近印刷日時列インデックス
        private const int HISTORY_PRINT_TANTOU_COLUMN    = 4;   // 印刷担当者列インデックス
        private const int HISTORY_PRINT_BUTTON_COLUMN    = 5;   // 印刷ボタン列インデックス

        // 印刷設定
        private const int SETTING_SYOKUJI_KBN_COLUMN = 0;       // 食事区分列インデックス
        private const int SETTING_SYOHIN_CD_COLUMN   = 1;       // 商品CD列インデックス
        private const int SETTING_SYOHIN_NAME_COLUMN = 2;       // 商品名列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 大袋ラベル表示条件
        /// </summary>
        private BigBagLabelCriteria _bigBagLabelCriteria;

        /// <summary>
        /// 大袋ラベル表示条件検索実行時スナップショット
        /// </summary>
        private BigBagLabelCriteria _bigBagLabelCriteriaSnapShot;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form307()
        {
            try
            {
                InitializeComponent();
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
        private void Form307_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                lblSetHistory.Text = "";
                lblUnitHistory.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _bigBagLabelCriteria = new BigBagLabelCriteria();
                _bigBagLabelCriteria.注文日 = DateTime.Today;
                _bigBagLabelCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
                bigBagLabelCriteriaBindingSource.DataSource = _bigBagLabelCriteria;

                _bigBagLabelCriteriaSnapShot = new BigBagLabelCriteria();
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
        private void Form307_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:                       // 表示ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                    btnQuery.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
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
                if (_bigBagLabelCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_bigBagLabelCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _bigBagLabelCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_bigBagLabelCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                lblSetHistory.Text = string.Format("{0:yyyy/MM/dd} {1}", _bigBagLabelCriteria.注文日, _bigBagLabelCriteria.部署名);
                lblUnitHistory.Text = string.Format("{0:yyyy/MM/dd} {1}", _bigBagLabelCriteria.注文日, _bigBagLabelCriteria.部署名);

                // セット印刷履歴を取得
                GetSetHistory();

                // 単品印刷履歴を取得
                GetUnitHistory();

                // セット印刷設定を取得
                GetSetSettings();

                // 単品印刷設定を取得
                GetUnitSettings();

                // 検索実行時の検索条件を保存（設定編集ボタン押下時に使用する）
                _bigBagLabelCriteriaSnapShot = new BigBagLabelCriteria(_bigBagLabelCriteria);
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
        /// セット印刷履歴Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_SetHistory_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 印刷ボタン
                if (e.Column == HISTORY_PRINT_BUTTON_COLUMN)
                {
                    // 印刷確認ダイアログを表示
                    ShowPrintDialog(Consts.BigBagPrintKind.Set);
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
        /// セット印刷履歴Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_SetHistory_KeyDown(object sender, KeyEventArgs e)
        {
            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_SetHistory_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_SetHistory_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= HISTORY_PRINT_BUTTON_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_SetHistory_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_SetHistory_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_SetHistory_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_SetHistory.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// セット設定編集ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索実行時の条件が変わっている？
                if (_bigBagLabelCriteria.Equals(_bigBagLabelCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "表示条件と一覧の内容が異なっています。先に表示ボタンをクリックしてください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail(Consts.BigBagPrintKind.Set);       // 大袋ラベル印刷設定画面表示
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
        /// 単品印刷履歴Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UnitHistory_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 印刷ボタン
                if (e.Column == HISTORY_PRINT_BUTTON_COLUMN)
                {
                    // 印刷確認ダイアログを表示
                    ShowPrintDialog(Consts.BigBagPrintKind.Unit);
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
        /// 単品印刷履歴Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UnitHistory_KeyDown(object sender, KeyEventArgs e)
        {
            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_UnitHistory_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_UnitHistory_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= HISTORY_PRINT_BUTTON_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_UnitHistory_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_UnitHistory_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_UnitHistory_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_UnitHistory.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 単品設定編集ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnitEdit_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索実行時の条件が変わっている？
                if (_bigBagLabelCriteria.Equals(_bigBagLabelCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "表示条件と一覧の内容が異なっています。先に表示ボタンをクリックしてください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail(Consts.BigBagPrintKind.Unit);       // 大袋ラベル印刷設定画面表示
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
        /// 手動印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManualPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索実行時の条件が変わっている？
                if (_bigBagLabelCriteria.Equals(_bigBagLabelCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "表示条件と一覧の内容が異なっています。先に表示ボタンをクリックしてください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowManualPrintDialog();            // 大袋ラベル手動印刷画面を表示
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
        /// セット印刷履歴を取得
        /// </summary>
        private void GetSetHistory()
        {
            try
            {
                // 大袋ラベル印刷履歴（セット）を取得
                DataTable dt = GetBigBagHistory(Consts.BigBagPrintKind.Set);

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                setHistoryBindingSource.DataSource = ds;
                setHistoryBindingSource.DataMember = dt.TableName;

                fpSpread_SetHistory_Sheet1.BindDataColumn(HISTORY_SYOKUJI_KBN_COLUMN,     "大袋ラベル食事区分名");
                fpSpread_SetHistory_Sheet1.BindDataColumn(HISTORY_PRINT_SHEETS_COLUMN,    "印刷枚数");
                fpSpread_SetHistory_Sheet1.BindDataColumn(HISTORY_PRINT_COUNT_COLUMN,     "印刷実行番号");
                fpSpread_SetHistory_Sheet1.BindDataColumn(HISTORY_PRINT_DATE_TIME_COLUMN, "直近印刷日時");
                fpSpread_SetHistory_Sheet1.BindDataColumn(HISTORY_PRINT_TANTOU_COLUMN,    "印刷担当者名");
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
        /// 単品印刷履歴を取得
        /// </summary>
        private void GetUnitHistory()
        {
            try
            {
                // 大袋ラベル印刷履歴（単品）を取得
                DataTable dt = GetBigBagHistory(Consts.BigBagPrintKind.Unit);

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                unitHistoryBindingSource.DataSource = ds;
                unitHistoryBindingSource.DataMember = dt.TableName;

                fpSpread_UnitHistory_Sheet1.BindDataColumn(HISTORY_SYOKUJI_KBN_COLUMN,     "大袋ラベル食事区分名");
                fpSpread_UnitHistory_Sheet1.BindDataColumn(HISTORY_PRINT_SHEETS_COLUMN,    "印刷枚数");
                fpSpread_UnitHistory_Sheet1.BindDataColumn(HISTORY_PRINT_COUNT_COLUMN,     "印刷実行番号");
                fpSpread_UnitHistory_Sheet1.BindDataColumn(HISTORY_PRINT_DATE_TIME_COLUMN, "直近印刷日時");
                fpSpread_UnitHistory_Sheet1.BindDataColumn(HISTORY_PRINT_TANTOU_COLUMN,    "印刷担当者名");
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
        /// 大袋ラベル印刷履歴を取得
        /// </summary>
        /// <param name="bigBagPrintKind">大袋ラベル印刷種別</param>
        /// <returns>大袋ラベル印刷種別に該当する大袋ラベル印刷設定を格納したデータテーブル</returns>
        private DataTable GetBigBagHistory(Consts.BigBagPrintKind bigBagPrintKind)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     OS.大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     ,VP.大袋ラベル印刷種別名" +
                                      "\r\n     ,OS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n     ,VS.大袋ラベル食事区分名" +
                                      "\r\n     ,OH.印刷実行番号" +
                                      "\r\n     ,OH.印刷枚数" +
                                      "\r\n     ,OH2.作成日 AS 直近印刷日時" +
                                      "\r\n     ,OH2.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 印刷担当者名" +
                                      "\r\n FROM" +
                                      // -- 商品を無視した大袋ラベル印刷設定（駆動表）
                                      "\r\n     (SELECT DISTINCT" +
                                      "\r\n         企業ＣＤ" +
                                      "\r\n         ,部署ＣＤ" +
                                      "\r\n         ,大袋ラベル印刷種別ＣＤ" +
                                      "\r\n         ,大袋ラベル食事区分ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         大袋ラベル印刷設定" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      // -- 手動印刷した場合は設定が無くても履歴は残る。設定が無くて履歴のみ存在する場合も履歴に表示する。
                                      "\r\n     UNION" +
                                      "\r\n     SELECT DISTINCT" +
                                      "\r\n         企業ＣＤ" +
                                      "\r\n         ,部署ＣＤ" +
                                      "\r\n         ,大袋ラベル印刷種別ＣＤ" +
                                      "\r\n         ,大袋ラベル食事区分ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         大袋ラベル印刷履歴" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         AND 喫食日 = @注文日" +
                                      "\r\n         AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     ) AS OS" +
                                      // -- 印刷実行番号と印刷枚数を集計した大袋ラベル印刷履歴
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             企業ＣＤ" +
                                      "\r\n             ,部署ＣＤ" +
                                      "\r\n             ,大袋ラベル印刷種別ＣＤ" +
                                      "\r\n             ,大袋ラベル食事区分ＣＤ" +
                                      "\r\n             ,喫食日" +
                                      "\r\n             ,MAX(印刷実行番号) AS 印刷実行番号" +
                                      "\r\n             ,SUM(印刷枚数) AS 印刷枚数" +
                                      "\r\n         FROM" +
                                      "\r\n             大袋ラベル印刷履歴" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND 喫食日 = @注文日" +
                                      "\r\n             AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             企業ＣＤ" +
                                      "\r\n             ,部署ＣＤ" +
                                      "\r\n             ,大袋ラベル印刷種別ＣＤ" +
                                      "\r\n             ,大袋ラベル食事区分ＣＤ" +
                                      "\r\n             ,喫食日" +
                                      "\r\n         ) AS OH" +
                                      "\r\n             ON OS.企業ＣＤ = OH.企業ＣＤ" +
                                      "\r\n             AND OS.部署ＣＤ = OS.部署ＣＤ" +
                                      "\r\n             AND OS.大袋ラベル印刷種別ＣＤ = OH.大袋ラベル印刷種別ＣＤ" +
                                      "\r\n             AND OS.大袋ラベル食事区分ＣＤ = OH.大袋ラベル食事区分ＣＤ" +
                                      // -- MAX(印刷実行番号)に該当する大袋ラベル印刷履歴（直近印刷日時とその担当者を取得する）
                                      "\r\n     LEFT JOIN 大袋ラベル印刷履歴 AS OH2" +
                                      "\r\n         ON OH.企業ＣＤ                = OH2.企業ＣＤ" +
                                      "\r\n         AND OH.部署ＣＤ               = OH2.部署ＣＤ" +
                                      "\r\n         AND OH.大袋ラベル印刷種別ＣＤ = OH2.大袋ラベル印刷種別ＣＤ" +
                                      "\r\n         AND OH.大袋ラベル食事区分ＣＤ = OH2.大袋ラベル食事区分ＣＤ" +
                                      "\r\n         AND OH.喫食日                 = OH2.喫食日" +
                                      "\r\n         AND OH.印刷実行番号           = OH2.印刷実行番号" +
                                      // -- 名称取得
                                      "\r\n     LEFT JOIN V_大袋ラベル印刷種別 AS VP ON OS.大袋ラベル印刷種別ＣＤ = VP.大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     LEFT JOIN V_大袋ラベル食事区分 AS VS ON OS.大袋ラベル食事区分ＣＤ = VS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON OH2.企業ＣＤ = A1.企業ＣＤ AND OH2.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     OS.大袋ラベル食事区分ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _bigBagLabelCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = (int)bigBagPrintKind;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _bigBagLabelCriteria.注文日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("大袋ラベル印刷履歴");
                    adp.Fill(dt);

                    log.InfoFormat("大袋ラベル印刷履歴({0})を検索しました。", bigBagPrintKind.DisplayName());

                    return dt;
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
        /// セット印刷設定を取得
        /// </summary>
        private void GetSetSettings()
        {
            try
            {
                // 大袋ラベル印刷設定（セット）を取得
                DataTable dt = GetBigBagSettings(Consts.BigBagPrintKind.Set);

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                setSettingsBindingSource.DataSource = ds;
                setSettingsBindingSource.DataMember = dt.TableName;

                fpSpread_SetSettings_Sheet1.BindDataColumn(SETTING_SYOKUJI_KBN_COLUMN, "大袋ラベル食事区分名");
                fpSpread_SetSettings_Sheet1.BindDataColumn(SETTING_SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread_SetSettings_Sheet1.BindDataColumn(SETTING_SYOHIN_NAME_COLUMN, "商品名");
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
        /// 単品印刷設定を取得
        /// </summary>
        private void GetUnitSettings()
        {
            try
            {
                // 大袋ラベル印刷設定（単品）を取得
                DataTable dt = GetBigBagSettings(Consts.BigBagPrintKind.Unit);

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                unitSettingsBindingSource.DataSource = ds;
                unitSettingsBindingSource.DataMember = dt.TableName;

                fpSpread_UnitSettings_Sheet1.BindDataColumn(SETTING_SYOKUJI_KBN_COLUMN, "大袋ラベル食事区分名");
                fpSpread_UnitSettings_Sheet1.BindDataColumn(SETTING_SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread_UnitSettings_Sheet1.BindDataColumn(SETTING_SYOHIN_NAME_COLUMN, "商品名");
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
        /// 大袋ラベル印刷設定を取得
        /// </summary>
        /// <param name="bigBagPrintKind">大袋ラベル印刷種別</param>
        /// <returns>大袋ラベル印刷種別に該当する大袋ラベル印刷設定を格納したデータテーブル</returns>
        private DataTable GetBigBagSettings(Consts.BigBagPrintKind bigBagPrintKind)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     OS.企業ＣＤ" +
                                      "\r\n     ,OS.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,OS.大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     ,VP.大袋ラベル印刷種別名" +
                                      "\r\n     ,OS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n     ,OS.大袋ラベル食事区分ＣＤ AS 大袋ラベル食事区分ＣＤorg" +
                                      "\r\n     ,VS.大袋ラベル食事区分名" +
                                      "\r\n     ,OS.商品ＣＤ" +
                                      "\r\n     ,OS.商品ＣＤ AS 商品ＣＤorg" +
                                      "\r\n     ,S.商品名" +
                                      "\r\n     ,OS.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,OS.作成日" +
                                      "\r\n     ,OS.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,OS.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     大袋ラベル印刷設定 AS OS" +
                                      "\r\n     LEFT JOIN V_大袋ラベル印刷種別 AS VP ON OS.大袋ラベル印刷種別ＣＤ = VP.大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     LEFT JOIN V_大袋ラベル食事区分 AS VS ON OS.大袋ラベル食事区分ＣＤ = VS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON OS.企業ＣＤ = B.企業ＣＤ AND OS.部署ＣＤ = B.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON OS.企業ＣＤ = S.企業ＣＤ AND OS.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON OS.企業ＣＤ = A1.企業ＣＤ AND OS.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON OS.企業ＣＤ = A2.企業ＣＤ AND OS.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     OS.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND OS.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND OS.大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     OS.大袋ラベル食事区分ＣＤ" +
                                      "\r\n     ,OS.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _bigBagLabelCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = (int)bigBagPrintKind;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("大袋ラベル印刷設定");
                    adp.Fill(dt);

                    log.InfoFormat("大袋ラベル印刷設定({0})を検索しました。", bigBagPrintKind.DisplayName());

                    return dt;
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
        /// 大袋ラベル印刷確認ダイアログ表示
        /// </summary>
        /// <param name="bigBagPrintKind"></param>
        private void ShowPrintDialog(Consts.BigBagPrintKind bigBagPrintKind)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // クリックされた行の集計区分（＝食事区分）を取得
                DataRow dr;
                if (bigBagPrintKind == Consts.BigBagPrintKind.Set)
                {
                    dr = ((DataRowView)setHistoryBindingSource.Current).Row;
                }
                else
                {
                    dr = ((DataRowView)unitHistoryBindingSource.Current).Row;
                }
                BigBagLabelPrintCriteria bigBagLabelPrintCriteria = new BigBagLabelPrintCriteria(_bigBagLabelCriteria);
                bigBagLabelPrintCriteria.食事区分ＣＤ = (int)dr["大袋ラベル食事区分ＣＤ"];
                bigBagLabelPrintCriteria.食事区分名 = dr["大袋ラベル食事区分名"].ToString();
                bigBagLabelPrintCriteria.印刷種別ＣＤ = bigBagPrintKind;
                Form307P form = new Form307P(bigBagLabelPrintCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "編集画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form307P_FormClosed);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 大袋ラベル印刷確認ダイアログクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form307P_FormClosed(object sender, FormClosedEventArgs e)
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

                Form307P form = (Form307P)sender;
                if (form.IsPrinted)     // 印刷した？
                {
                    if (form.BigBagPrintKind == Consts.BigBagPrintKind.Set) // セットで開いていた？
                    {
                        GetSetHistory();    // セット印刷履歴を取得
                    }
                    else
                    {
                        GetUnitHistory();   // 単品印刷履歴を取得
                    }
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
        /// 大袋ラベル印刷設定画面表示
        /// </summary>
        /// <param name="bigBagPrintKind">大袋ラベル印刷種別</param>
        private void ShowDetail(Consts.BigBagPrintKind bigBagPrintKind)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Form307E form = new Form307E(_bigBagLabelCriteria, bigBagPrintKind);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "編集画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form307E_FormClosed);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 大袋ラベル印刷設定画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form307E_FormClosed(object sender, FormClosedEventArgs e)
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

                Form307E form = (Form307E)sender;
                if (form.BigBagPrintKind == Consts.BigBagPrintKind.Set) // セットで開いていた？
                {
                    GetSetHistory();    // セット印刷履歴を取得
                    GetSetSettings();   // セット印刷設定を取得
                }
                else
                {
                    GetUnitHistory();   // 単品印刷履歴を取得
                    GetUnitSettings();  // 単品印刷設定を取得
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
        /// 大袋ラベル手動印刷画面表示
        /// </summary>
        private void ShowManualPrintDialog()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                BigBagLabelPrintCriteria bigBagLabelPrintCriteria = new BigBagLabelPrintCriteria(_bigBagLabelCriteria);
                if (tabControl1.SelectedIndex == 0)     // セット？
                {
                    bigBagLabelPrintCriteria.印刷種別ＣＤ = Consts.BigBagPrintKind.Set;
                }
                else
                {
                    bigBagLabelPrintCriteria.印刷種別ＣＤ = Consts.BigBagPrintKind.Unit;
                }
                Form307M form = new Form307M(bigBagLabelPrintCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "編集画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form307M_FormClosed);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 大袋ラベル手動印刷画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form307M_FormClosed(object sender, FormClosedEventArgs e)
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

                Form307M form = (Form307M)sender;
                if (form.IsPrinted)     // 印刷した？
                {
                    if (form.BigBagPrintKind == Consts.BigBagPrintKind.Set) // セットで開いていた？
                    {
                        GetSetHistory();    // セット印刷履歴を取得
                    }
                    else
                    {
                        GetUnitHistory();   // 単品印刷履歴を取得
                    }
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
    }
}

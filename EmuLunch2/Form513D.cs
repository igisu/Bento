using Emu.Bento.Common;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emu.Bento
{
    /// <summary>
    /// FRクレカ決済詳細
    /// </summary>
    public partial class Form513D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int OPERATION_DATETIME_COLUMN = 0;    // 処理日時列インデックス
        private const int SEIKYU_NO_COLUMN = 1;    // 請求番号列インデックス
        private const int SEIKYU_AMOUNT_COLUMN = 2;    // 取引金額列インデックス
        private const int OPERATION_TYPE_COLUMN = 3;    // 処理タイプ列インデックス        
        private const int OPERATION_RESULT_COLUMN = 4;    // 処理結果列インデックス
        private const int OPERATION_RESULT_MEMO_COLUMN = 5;    // 処理結果内容列インデックス
        private const int APPROVAL_NO_COLUMN = 6;    // 承認番号列インデックス
        private const int TRANSACTION_NO_COLUMN = 7;    // 取引番号列インデックス

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// 請求合計
        /// </summary>
        private SeikyuGokei _seikyuGokei;

        /// <summary>
        /// DB保存済のFR_T_クレカ決済履歴
        /// </summary>
        private FRSettlementHistory _saveHistory;

        /// <summary>
        /// API結果（レスポンスヘッダーの成功判定）のマップ
        /// ※レスポンスボディの処理結果ではない
        /// </summary>
        private bool _cancelResponseResult = false;

        /// <summary>
        /// 処理中かどうか
        /// </summary>
        private bool _isProceccing = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form513D(SeikyuGokei seikyuGokei)
        {
            InitializeComponent();
            _seikyuGokei = new SeikyuGokei(seikyuGokei);
            _saveHistory = new FRSettlementHistory();
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
        private void Form513D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // 権限によりボタンを制御
                btnExec.Enabled = this.HasAuthEdit;

                Init();
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
        private void Form513D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form513D_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_isProceccing)
                {
                    MessageBox.Show(this, "処理中のため、画面を閉じることはできません。\n処理が完了するまでお待ちください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                    return;
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
        /// 中間サーバー問合せボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInquiry_Click(object sender, EventArgs e)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Form515 form = new Form515(_seikyuGokei);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form515_FormClosed);
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form515_FormClosed(object sender, FormClosedEventArgs e)
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
        /// 決済取消ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(this, "クレカ決済の取消処理を行います。よろしいですか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                _isProceccing = true;
                btnExec.Enabled = false;
                btnQuit.Enabled = false;

                // 取消処理
                var response = await CancelExec();

                bool result = response.Item1;
                string message = response.Item2;

                if (_cancelResponseResult)
                {
                    // 不整合確認
                    response = await ConsistencyExec();

                    result = response.Item1;
                    message = response.Item2;

                    if (result)
                    {
                        MessageBox.Show(this, "取消処理が完了しました。\n" + message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(this, "取消処理は完了しましたが、整合性確認に失敗しました。\n" + message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show(this, "取消処理に失敗しました。\n" + message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // 再読込
                Reload();
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
                _isProceccing = false;
                btnQuit.Enabled = true;

                this.MdiParent.Cursor = Cursors.Default;
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
                if (_isProceccing)
                {
                    MessageBox.Show(this, "処理中のため、画面を閉じることはできません。\n処理が完了するまでお待ちください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

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
        /// 初期化処理
        /// </summary>
        private void Init()
        {
            try
            {
                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                seikyuBindingSource.DataSource = _seikyuGokei;

                // 決済状態で文字色とメッセージを変更
                if (_seikyuGokei.クレカ決済エラー区分 == (int)Consts.FRErrorType.MatchError)
                {
                    lblSettlementStatus.ForeColor = Color.Black;
                    lblMessage.Text = "クレカ決済または決済取消で不整合が発生しています。\nシステム管理者までお問い合わせください。";
                    lblMessage.Visible = true;
                }
                else if (_seikyuGokei.クレカ決済エラー区分 == (int)Consts.FRErrorType.SystemError)
                {
                    lblSettlementStatus.ForeColor = Color.Black;
                    lblMessage.Text = "クレカ決済または決済取消でシステムエラーが発生しています。\nシステム管理者までお問い合わせください。";
                    lblMessage.Visible = true;
                }
                else if (_seikyuGokei.クレカ決済完了フラグ == false)
                {
                    lblSettlementStatus.ForeColor = Color.Red;
                    lblMessage.Visible = false;
                }
                else if (_seikyuGokei.クレカ決済完了フラグ == true)
                {
                    lblSettlementStatus.ForeColor = Color.Blue;
                    lblMessage.Visible = false;
                }

                // 決済完了かつエラーなしのみ取消可能
                btnExec.Enabled = _seikyuGokei.クレカ決済完了フラグ == true && _seikyuGokei.クレカ決済エラー区分 == (int)Consts.FRErrorType.None;

                // 決済履歴を取得
                GetSettlementHistory();

                // 得意先マスタを取得
                Tokuisaki seikyusaki = Tokuisaki.GetTokuisaki((int)_seikyuGokei.請求先ＣＤ);

                // クレカ無効の未決済の場合はメッセージを表示
                if (!lblMessage.Visible && seikyusaki.クレカ無効フラグ == true && _seikyuGokei.クレカ決済完了フラグ != true)
                {
                    lblMessage.Text = "登録されているクレジットカードが有効ではありません。";
                    lblMessage.Visible = true;
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
        /// クレカ決済履歴を取得
        /// </summary>
        /// <exception cref="EmuException"></exception>
        private void GetSettlementHistory()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.履歴番号");
                    sql.AppendLine("     ,A.請求番号");
                    sql.AppendLine("     ,A.請求日");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.請求先ＣＤ");
                    sql.AppendLine("     ,A.請求先名");
                    sql.AppendLine("     ,A.今回請求額");
                    sql.AppendLine("     ,A.処理タイプ");
                    sql.AppendLine("     ,CASE A.処理タイプ ");
                    sql.AppendFormat("       WHEN {0} THEN '決済' ", (int)Consts.FRProcessType.Settle).AppendLine();
                    sql.AppendFormat("       WHEN {0} THEN '取消' ", (int)Consts.FRProcessType.Cancel).AppendLine();
                    sql.AppendLine("       ELSE '' ");
                    sql.AppendLine("      END AS 処理タイプ名 ");
                    sql.AppendLine("     ,A.処理結果");
                    sql.AppendLine("     ,CASE A.処理結果 ");
                    sql.AppendFormat("       WHEN {0} THEN '成功' ", (int)Consts.FRProcessResult.Success).AppendLine();
                    sql.AppendFormat("       WHEN {0} THEN '失敗' ", (int)Consts.FRProcessResult.Fail).AppendLine();
                    sql.AppendLine("       ELSE '' ");
                    sql.AppendLine("      END AS 処理結果名 ");
                    sql.AppendLine("     ,A.処理結果内容");
                    sql.AppendLine("     ,A.店舗ID");
                    sql.AppendLine("     ,A.伝票番号");
                    sql.AppendLine("     ,A.承認番号");
                    sql.AppendLine("     ,A.取引番号");
                    sql.AppendLine("     ,A.クレカ顧客番号");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     FR_T_クレカ決済履歴 AS A");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND A.請求日 = @請求日");
                    sql.AppendLine("     AND A.請求先ＣＤ = @請求先ＣＤ");
                    sql.AppendLine(" ORDER BY ");
                    sql.AppendLine("     A.履歴番号 DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = _seikyuGokei.請求日;
                    cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuGokei.請求先ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("FRクレカ決済履歴");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "FRクレカ決済履歴";

                    log.Info("FRクレカ決済履歴を取得しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(OPERATION_DATETIME_COLUMN, "作成日");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN, "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_AMOUNT_COLUMN, "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(OPERATION_TYPE_COLUMN, "処理タイプ名");
                fpSpread1_Sheet1.BindDataColumn(OPERATION_RESULT_COLUMN, "処理結果名");
                fpSpread1_Sheet1.BindDataColumn(OPERATION_RESULT_MEMO_COLUMN, "処理結果内容");
                fpSpread1_Sheet1.BindDataColumn(APPROVAL_NO_COLUMN, "承認番号");
                fpSpread1_Sheet1.BindDataColumn(TRANSACTION_NO_COLUMN, "取引番号");

                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, OPERATION_RESULT_COLUMN].Value.ToString() == "失敗")
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = System.Drawing.Color.DarkRed;
                    }
                    else if (fpSpread1_Sheet1.Cells[rowIndex, OPERATION_TYPE_COLUMN].Value.ToString() == "取消")
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = System.Drawing.Color.Silver;
                        //fpSpread1_Sheet1.Rows[rowIndex].ForeColor = System.Drawing.Color.Red;
                    }
                }

                fpSpread1.Focus();
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
        /// 取消処理
        /// </summary>
        /// <returns></returns>
        private async Task<ValueTuple<bool, string>> CancelExec()
        {
            try
            {
                bool result = false;
                string message = null;

                // 決済履歴をコピーして取消時の新しい情報のみ上書きする
                DataRow dr = ((DataRowView)bindingSource1[0]).Row;
                FRSettlementHistory frHistory = new FRSettlementHistory(dr);

                await Task.Run(() =>
                {
                    // 存在確認
                    SeikyuGokei seikyuGokei = SeikyuGokei.GetFRRecord(_seikyuGokei);

                    if (seikyuGokei == null)
                    {
                        message = "対象の請求データが変更された可能性があります。再度検索した後に実行してください。";
                    }
                    else
                    {
                        if (seikyuGokei.クレカ決済完了フラグ == false)
                        {
                            message = "対象の請求データが変更された可能性があります。再度検索した後に実行してください。";
                        }
                        else
                        {
                            // 取消API
                            FRSettlement frSettlement = new FRSettlement();
                            result = frSettlement.SaleCancel(seikyuGokei, ref frHistory, out message);

                            // 結果を反映
                            _cancelResponseResult = result;

                            result = Save(seikyuGokei, frHistory);
                            message = result ? message : "決済履歴の登録に失敗しました。";

                            //// DB登録は成功時のみ
                            //if (result)
                            //{
                            //    result = Save(_seikyuGokei, frHistory);
                            //    message = result ? message : "決済履歴の登録に失敗しました。";
                            //}
                        }
                    }

                    // 履歴保存
                    if (result && _cancelResponseResult)
                    {
                        History.Save(string.Format("{0}のクレカ決済の取消を実行しました。", seikyuGokei.Code));
                    }
                    else
                    {
                        History.Save(string.Format("{0}のクレカ決済の取消に失敗しました。", seikyuGokei.Code));
                    }

                    _seikyuGokei = new SeikyuGokei(seikyuGokei);
                });

                return ValueTuple.Create(result, message);
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
        /// DB更新
        /// </summary>
        /// <param name="seikyuGokei"></param>
        /// <param name="frHistory"></param>
        /// <returns></returns>
        /// <exception cref="EmuException"></exception>
        private bool Save(SeikyuGokei seikyuGokei, FRSettlementHistory frHistory)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    try
                    {
                        if (!_cancelResponseResult)
                        {
                            // APIが失敗した場合はシステムエラーとする

                            //**************************
                            //* 請求合計
                            //**************************

                            seikyuGokei.クレカ決済エラー区分 = (int)Consts.FRErrorType.SystemError;

                            if (!seikyuGokei.Update(con, tran))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // APIが成功した場合は履歴等の登録を行う

                            //**************************
                            //* FR_T_クレカ決済履歴
                            //**************************

                            // 履歴番号を採番
                            frHistory.履歴番号 = FRSettlementHistory.GetNextNo();

                            if (!frHistory.Insert(con, tran))
                            {
                                return false;
                            }

                            // 保存済にセット
                            _saveHistory = new FRSettlementHistory(frHistory);

                            // 決済処理が成功したときのみ請求合計の更新とその履歴を登録
                            if (frHistory.処理結果 == (int)Consts.FRProcessResult.Success)
                            {
                                //**************************
                                //* 請求合計
                                //**************************

                                // クレカ決済完了フラグを更新
                                seikyuGokei.クレカ決済完了フラグ = false;

                                if (!seikyuGokei.Update(con, tran))
                                {
                                    return false;
                                }

                                // 請求合計履歴と請求ヘッダ履歴は決済完了時に残しているので取消時は何もしない

                                //**************************
                                //* 入金明細
                                //**************************

                                // 請求番号に紐づく入金明細を削除
                                SqlCommand cmd = con.CreateCommand();
                                cmd.Transaction = tran;
                                cmd.CommandText = "DELETE FROM 入金明細 WHERE 企業ＣＤ = @企業ＣＤ AND 対応請求番号 = @対応請求番号";
                                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = seikyuGokei.企業ＣＤ;
                                cmd.Parameters.Add("@対応請求番号", SqlDbType.Decimal).Value = seikyuGokei.請求番号;
                                log.Debug(cmd.CommandTextWithParameters());
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // コミット
                        tran.Commit();

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
        /// 整合性確認処理
        /// </summary>
        /// <returns></returns>
        private async Task<ValueTuple<bool, string>> ConsistencyExec()
        {
            try
            {
                bool result = false;
                string message = null;
                FRSettlementHistory frHistory = new FRSettlementHistory();

                try
                {
                    //// APIが失敗している場合は何もしない
                    //if (!_cancelResponseResult)
                    //{
                    //    return (true, "");
                    //}

                    await Task.Run(() =>
                    {
                        // 決済履歴取得API
                        FRSettlement fRSettlement = new FRSettlement();
                        result = fRSettlement.InfoGetSettlement(_seikyuGokei, out frHistory, out message);

                        if (result)
                        {
                            // 保存なしor履歴内容が一致しない → 不整合の可能性あり
                            if (_saveHistory == null || _saveHistory.IsMatch(frHistory) == false)
                            {
                                // 存在確認
                                SeikyuGokei targetSG = SeikyuGokei.GetFRRecord(_seikyuGokei);

                                if (targetSG == null)
                                {
                                    message = "対象の請求データが変更された可能性があります。再度検索した後に実行してください。（整合性チェック）";
                                }
                                else
                                {
                                    message = "中間サーバーとの決済履歴に不整合がある可能性があります。システム管理者までお問い合わせください。";

                                    // エラー区分を更新
                                    targetSG.クレカ決済エラー区分 = (int)Consts.FRErrorType.MatchError;

                                    if (!targetSG.Update())
                                    {
                                        message = "不整合情報の更新に失敗しました。";
                                    }
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    result = false;
                    message = ex.ToString();
                    log.Error(ex);
                    History.Error(ex);
                }

                return ValueTuple.Create(result, message);
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
            try
            {
                // 決済状態が変わるので再取得
                _seikyuGokei = SeikyuGokei.GetSeikyuGokei((decimal)_seikyuGokei.請求番号);

                Init();
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

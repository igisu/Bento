using Emu.Bento.Common;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Emu.Bento
{
    /// <summary>
    /// FRクレカ決済処理
    /// </summary>
    public partial class Form513B : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int SETTLEMENT_STATUS_COLUMN = 0;   // 決済状態列インデックス
        private const int SEIKYU_NO_COLUMN = 1;    // 請求番号列インデックス
        private const int SEIKYUBI_COLUMN = 2;    // 請求日列インデックス
        private const int SEIKYUSAKI_CD_COLUMN = 3;    // 請求先ＣＤ列インデックス
        private const int SEIKYUSAKI_NAME_COLUMN = 4;    // 請求先名列インデックス
        private const int KONKAI_SEIKYU_COLUMN = 5;   // 今回請求額列インデックス

        private const string LABEL_FORMAT = "{0}/{1}";
        private const string LABEL_FORMAT_SETTLE = "{0}/{1} 決済中...";
        private const string LABEL_FORMAT_CHECK = "{0}/{1} 確認中...";

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// FRクレカ決済一覧検索条件
        /// </summary>
        private FRSettlementListCriteria _seikyuListCriteria;

        /// <summary>
        /// 請求合計リスト
        /// </summary>
        private List<SeikyuGokei> _seikyuGokeiList;

        /// <summary>
        /// DB保存済のFR_T_クレカ決済履歴リスト
        /// </summary>
        private List<FRSettlementHistory> _saveHistoryList;

        /// <summary>
        /// 請求番号とAPI結果（レスポンスヘッダーの成功判定）のマップ
        /// ※レスポンスボディの処理結果ではない
        /// </summary>
        private Dictionary<decimal, bool> _settleResponseResultList;

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
        /// <param name="seikyuListCriteria"></param>
        /// <param name="seikyuGokeis"></param>
        public Form513B(FRSettlementListCriteria seikyuListCriteria, List<SeikyuGokei> seikyuGokeis)
        {
            InitializeComponent();

            _seikyuListCriteria = seikyuListCriteria;
            _seikyuGokeiList = seikyuGokeis;
            _saveHistoryList = new List<FRSettlementHistory>();
            _settleResponseResultList = new Dictionary<decimal, bool>();
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
        private void Form513B_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                progressBar1.Visible = false;
                lblCount.Text = String.Format(LABEL_FORMAT, 0, _seikyuGokeiList.Count);

                // 権限によりボタンを制御
                btnExec.Enabled = this.HasAuthEdit;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                seikyuListBindingSource.DataSource = _seikyuListCriteria;

                bindingSource1.DataSource = _seikyuGokeiList;

                fpSpread1_Sheet1.BindDataColumn(SETTLEMENT_STATUS_COLUMN, "決済状態");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN, "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN, "請求日");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_CD_COLUMN, "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN, "今回請求額");

                // 決済状態で文字色を変更する
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Cells[rowIndex, SETTLEMENT_STATUS_COLUMN].ResetForeColor();
                    string status = fpSpread1_Sheet1.Cells[rowIndex, SETTLEMENT_STATUS_COLUMN].Value as string;
                    if (status == "未決済")
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, SETTLEMENT_STATUS_COLUMN].ForeColor = Color.Red;
                    }
                    else if (status == "決済完了")
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, SETTLEMENT_STATUS_COLUMN].ForeColor = Color.Blue;
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
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form513B_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:
                    btnExec.PerformClick();         // 処理実行ボタン押下
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form513B_Shown(object sender, EventArgs e)
        {
            fpSpread1.Focus();
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form513B_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 処理実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(this, string.Format("{0}件のクレカ決済を行います。よろしいですか？", fpSpread1_Sheet1.Rows.Count), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                _isProceccing = true;
                btnExec.Enabled = false;
                btnQuit.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                // 先頭行までスクロール
                fpSpread1.ShowRow(0, 0, FarPoint.Win.Spread.VerticalPosition.Top);

                // プログレスバー初期処理
                IProgress<ProgressState> p = new Progress<ProgressState>(ShowProgress);
                ProgressState progressState = new ProgressState();

                if (p != null)
                {
                    progressState.Style = ProgressBarStyle.Blocks;
                    progressState.Maximum = _seikyuGokeiList.Count * 2;    // 進行状況は半分を決済API呼び出し、半分を不整合確認に割り当てるので倍にする
                    progressState.Value = 0;
                    progressState.Message = string.Format(LABEL_FORMAT_SETTLE, progressState.Value, _seikyuGokeiList.Count);
                    p.Report(progressState);
                }

                // 決済処理
                await SettleExec(p, progressState);

                // 先頭行までスクロール
                fpSpread1.ShowRow(0, 0, FarPoint.Win.Spread.VerticalPosition.Top);

                // 表示メッセージを不整合確認用に変更
                if (p != null)
                {
                    progressState.Message = string.Format(LABEL_FORMAT_CHECK, 0, _seikyuGokeiList.Count);
                    p.Report(progressState);
                }

                // 不整合確認
                await ConsistencyExec(p, progressState);

                // 表示メッセージ更新
                progressState.Message = string.Format(LABEL_FORMAT, _seikyuGokeiList.Count, _seikyuGokeiList.Count);

                MessageBox.Show(this, "決済処理が完了しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                progressBar1.Visible = false;

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
        /// 決済処理
        /// </summary>
        /// <param name="p"></param>
        /// <param name="progressState"></param>
        /// <returns></returns>
        private async Task SettleExec(IProgress<ProgressState> p, ProgressState progressState)
        {
            try
            {
                for (int i = 0; i < _seikyuGokeiList.Count; i++)
                {
                    bool result = false;
                    string message = null;
                    FRSettlementHistory frHistory = new FRSettlementHistory();

                    try
                    {
                        // 決済対象を取得
                        SeikyuGokei seikyuGokei = _seikyuGokeiList[i];
                        decimal seikyuNo = (decimal)seikyuGokei.請求番号;

                        // 現在処理行を選択
                        fpSpread1_Sheet1.ActiveRowIndex = progressState.Value;
                        bindingSource1.Position = progressState.Value;
                        fpSpread1_Sheet1.AddSelection(progressState.Value, 0, 1, 1);

                        // 請求番号とAPI結果を初期セット
                        _settleResponseResultList.Add(seikyuNo, false);

                        await Task.Run(() =>
                        {
                            // 存在確認
                            seikyuGokei = SeikyuGokei.GetFRRecord(seikyuGokei);

                            if (seikyuGokei == null)
                            {
                                message = "対象の請求データが変更された可能性があります。再度検索した後に実行してください。";
                            }
                            else
                            {
                                if (seikyuGokei.クレカ決済完了フラグ == true)
                                {
                                    message = "対象の請求データが変更された可能性があります。再度検索した後に実行してください。";
                                }
                                else
                                {
                                    // 決済API
                                    FRSettlement frSettlement = new FRSettlement();
                                    result = frSettlement.SaleSettlement(seikyuGokei, out frHistory, out message);

                                    // 結果を反映
                                    _settleResponseResultList[seikyuNo] = result;

                                    //// DB登録は成功時のみ
                                    //if (result)
                                    //{
                                    //    result = Save(seikyuGokei, frHistory);
                                    //    message = result ? message : "決済履歴の登録に失敗しました。";
                                    //}

                                    result = Save(seikyuGokei, frHistory);
                                    message = result ? message : "決済履歴の登録に失敗しました。";
                                }
                            }

                            // 履歴保存
                            if (result && _settleResponseResultList[seikyuNo])
                            {
                                History.Save(string.Format("{0}のクレカ決済を実行しました。", seikyuGokei.Code));
                            }
                            else
                            {
                                History.Save(string.Format("{0}のクレカ決済に失敗しました。", seikyuGokei.Code));
                            }

                            _seikyuGokeiList[i] = seikyuGokei;
                        });
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        message = ex.ToString();
                        log.Error(ex);
                        History.Error(ex);
                    }
                    finally
                    {
                        // 一覧更新

                        // 全ての処理が完了した場合は状態を「決済完了」に変更
                        if (result && frHistory.処理結果 == (int)Consts.FRProcessResult.Success)
                        {
                            List<SeikyuGokei> bindingList = (List<SeikyuGokei>)bindingSource1.DataSource;
                            bindingList[i].クレカ決済完了フラグ = true;
                            bindingSource1.ResetItem(i);

                            fpSpread1_Sheet1.Cells[i, SETTLEMENT_STATUS_COLUMN].ForeColor = Color.Blue;
                        }

                        // エラーメッセージがあれば状態セルにNOTEを設定
                        if (!string.IsNullOrEmpty(message))
                        {
                            fpSpread1_Sheet1.Cells[i, SETTLEMENT_STATUS_COLUMN].Note = message;
                        }

                        if (p != null)
                        {
                            progressState.Value++;
                            progressState.Message = string.Format(LABEL_FORMAT_SETTLE, progressState.Value, _seikyuGokeiList.Count);
                            p.Report(progressState);
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
        /// 整合性確認処理
        /// </summary>
        /// <param name="p"></param>
        /// <param name="progressState"></param>
        /// <returns></returns>
        private async Task ConsistencyExec(IProgress<ProgressState> p, ProgressState progressState)
        {
            try
            {
                int progressValue = 0;

                for (int i = 0; i < _seikyuGokeiList.Count; i++)
                {
                    bool result = false;
                    string message = null;
                    FRSettlementHistory frHistory = new FRSettlementHistory();

                    try
                    {
                        // 決済対象を取得
                        SeikyuGokei seikyuGokei = _seikyuGokeiList[i];
                        decimal seikyuNo = (decimal)seikyuGokei.請求番号;

                        // 現在処理行を選択
                        fpSpread1_Sheet1.ActiveRowIndex = progressValue;
                        bindingSource1.Position = progressValue;
                        fpSpread1_Sheet1.AddSelection(progressValue, 0, 1, 1);

                        // APIが失敗している場合は何もしない
                        if (!_settleResponseResultList.ContainsKey((decimal)seikyuGokei.請求番号) || !_settleResponseResultList[(decimal)seikyuGokei.請求番号])
                        {
                            continue;
                        }

                        await Task.Run(() =>
                        {
                            // 決済履歴取得API
                            FRSettlement fRSettlement = new FRSettlement();
                            result = fRSettlement.InfoGetSettlement(seikyuGokei, out frHistory, out message);

                            if (result)
                            {
                                // APIの決済履歴と不整合がないか確認する
                                FRSettlementHistory target = _saveHistoryList.Find(x => x.請求番号 == frHistory.請求番号);

                                // 保存なしor履歴内容が一致しない → 不整合の可能性あり
                                if (target == null || target.IsMatch(frHistory) == false)
                                {
                                    // 存在確認
                                    SeikyuGokei targetSG = SeikyuGokei.GetFRRecord(seikyuGokei);

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
                    finally
                    {
                        // エラーメッセージがあれば状態セルにNOTEを設定
                        if (!string.IsNullOrEmpty(message))
                        {
                            if (string.IsNullOrEmpty(fpSpread1_Sheet1.Cells[i, SETTLEMENT_STATUS_COLUMN].Note))
                            {
                                fpSpread1_Sheet1.Cells[i, SETTLEMENT_STATUS_COLUMN].Note = message;
                            }
                            else
                            {
                                //fpSpread1_Sheet1.Cells[i, SETTLEMENT_STATUS_COLUMN].Note += "\n" + message;
                            }
                        }

                        if (p != null)
                        {
                            progressState.Value++;
                            progressState.Message = string.Format(LABEL_FORMAT_CHECK, ++progressValue, _seikyuGokeiList.Count);
                            p.Report(progressState);
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
                        if (!_settleResponseResultList.ContainsKey((decimal)seikyuGokei.請求番号))
                        {
                            return false;
                        }

                        if (!_settleResponseResultList[(decimal)seikyuGokei.請求番号])
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

                            // 足りない項目を追加
                            frHistory.請求番号 = seikyuGokei.請求番号;
                            frHistory.請求日 = seikyuGokei.請求日;
                            frHistory.部署ＣＤ = seikyuGokei.部署ＣＤ;
                            frHistory.請求先ＣＤ = seikyuGokei.請求先ＣＤ;
                            frHistory.請求先名 = seikyuGokei.請求先名;
                            frHistory.今回請求額 = seikyuGokei.今回請求額;

                            if (!frHistory.Insert(con, tran))
                            {
                                return false;
                            }

                            // 保存済みリストに追加
                            _saveHistoryList.Add(frHistory);

                            //**************************
                            //* 請求合計
                            //**************************

                            // クレカ決済完了フラグを更新
                            seikyuGokei.クレカ決済完了フラグ = (frHistory.処理結果 == (int)Consts.FRProcessResult.Success);

                            // クレカ決済エラー区分を更新
                            seikyuGokei.クレカ決済エラー区分 = (int)Consts.FRErrorType.None;

                            if (!seikyuGokei.Update(con, tran))
                            {
                                return false;
                            }

                            //**************************
                            //* FR_T_クレカ請求合計履歴
                            //**************************

                            // 請求合計をコピー登録する
                            FRSeikyuGokeiHistory sgHistory = new FRSeikyuGokeiHistory(seikyuGokei);

                            // 履歴番号を採番
                            sgHistory.履歴番号 = FRSeikyuGokeiHistory.GetNextNo();

                            if (!sgHistory.Insert(con, tran))
                            {
                                return false;
                            }

                            //**************************
                            //* FR_T_クレカ請求ヘッダ履歴
                            //**************************

                            // 請求番号から請求ヘッダのリストを取得してコピー登録する
                            List<SeikyuHeader> seikyuHeaderList = SeikyuHeader.GetSeikyuHeader((decimal)seikyuGokei.請求番号);

                            int detailNo = 0;
                            foreach (SeikyuHeader sh in seikyuHeaderList)
                            {
                                FRSeikyuHeaderHistory shHistory = new FRSeikyuHeaderHistory(sh);
                                shHistory.履歴番号 = sgHistory.履歴番号;
                                shHistory.履歴明細番号 = ++detailNo;

                                if (!shHistory.Insert(con, tran))
                                {
                                    return false;
                                }
                            }

                            // 決済処理が成功したときのみ入金明細を作成する
                            if (frHistory.処理結果 == (int)Consts.FRProcessResult.Success)
                            {
                                //**************************
                                //* 入金明細
                                //**************************

                                decimal newNyukinNo = 0;
                                Nyukin nyukin = new Nyukin();

                                SqlCommand cmdMax = con.CreateCommand();
                                cmdMax.Transaction = tran;
                                cmdMax.CommandText = "SELECT ISNULL(MAX(入金番号), 0) + 1 as 入金番号 FROM 入金明細 WHERE 企業ＣＤ = @企業ＣＤ";
                                cmdMax.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                                using (SqlDataReader reader = cmdMax.ExecuteReader())
                                {
                                    reader.Read();
                                    newNyukinNo = (decimal)reader["入金番号"];
                                }

                                nyukin.入金番号 = newNyukinNo;
                                nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.PC;
                                nyukin.入金日 = DateTime.Today.AddDays(1);
                                nyukin.部署ＣＤ = seikyuGokei.部署ＣＤ;
                                nyukin.得意先ＣＤ = seikyuGokei.請求先ＣＤ;
                                nyukin.得意先名 = seikyuGokei.請求先名;
                                nyukin.依頼人ＣＤ = null;
                                nyukin.振込依頼人名 = null;
                                nyukin.入金方法 = (int)Consts.NyukinHoho.CreditCard;
                                nyukin.入金額 = seikyuGokei.今回請求額;
                                nyukin.入金摘要区分 = seikyuGokei.請求日.Value.Month;
                                nyukin.対応請求番号 = seikyuGokei.請求番号;
                                nyukin.備考 = null;

                                if (!nyukin.Insert(con, tran))
                                {
                                    return false;
                                }
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
        /// 進捗状況更新
        /// </summary>
        /// <param name="progressState">プログレスバーを操作するパラメータ</param>
        private void ShowProgress(ProgressState progressState)
        {
            ShowProgress(progressBar1, lblCount, progressState);
        }

        #endregion
    }
}

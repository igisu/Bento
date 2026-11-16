using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 会計連携CSV出力
    /// </summary>
    public partial class Form840 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// 会計連携CSV出力条件
        /// </summary>
        private KaikeiRenkei _kaikeiRenkei;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form840()
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
        private void Form840_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.group_ico;

                // 会計連携CSV出力対象パターンに応じて各ラジオボタンのVisibleを決定する
                int pattern = SystemMaster.GetForm840Pattern();
                if (pattern == 0)
                {
                    // 広島駅弁当パターン
                    rdoDiff.Visible = true;
                    rdoFull.Visible = true;
                    rdoRecalc.Visible = false;

                    rdoDiff.Checked = true;
                }
                else
                {
                    // ミールジャパンパターン
                    rdoDiff.Visible = false;
                    rdoFull.Visible = false;
                    rdoRecalc.Visible = true;

                    rdoRecalc.Checked = true;
                }

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _kaikeiRenkei = new KaikeiRenkei();
                _kaikeiRenkei.出力対象日 = DateTime.Today;
                kaikeiRenkeiBindingSource.DataSource = _kaikeiRenkei;
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
        private void Form840_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:                      // 処理開始ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                    btnOutput.PerformClick();
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
        /// 出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutput_Click(object sender, EventArgs e)
        {
            string filePath = "";
            try
            {
                // 出力対象日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_kaikeiRenkei.出力対象日 > warnDate)
                {
                    if (MessageBox.Show(this, "出力対象日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // ファイル名を決定
                filePath = string.Format("会計連携_{0:yyyyMMdd}", _kaikeiRenkei.出力対象日);
                if (string.IsNullOrEmpty(saveFileDialog1.InitialDirectory))
                {
                    saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                }
                saveFileDialog1.FileName = filePath;
                if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                filePath = saveFileDialog1.FileName;    // ファイルパスとして取得
                saveFileDialog1.InitialDirectory = Path.GetDirectoryName(filePath);     // デフォルトのディレクトリにする

                // ファイルが書き込みできるか確認
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    // NGの場合はIOExceptionへthrow
                }

                // 以下書き込み可能
                // using内だと別プロセスで使用中となってしまうので外に出す

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (rdoDiff.Checked)
                {
                    _kaikeiRenkei.出力対象 = KaikeiRenkei.KaikeiRenkeiTarget.Diff;
                }
                if (rdoFull.Checked)
                {
                    _kaikeiRenkei.出力対象 = KaikeiRenkei.KaikeiRenkeiTarget.Full;
                }
                if (rdoRecalc.Checked)
                {
                    _kaikeiRenkei.出力対象 = KaikeiRenkei.KaikeiRenkeiTarget.Recalc;
                }

                // 出力処理
                Output(filePath);

                // 会計連携の集計対象外を抽出
                // 集計対象外があれば汎用レポート画面を使って表示
                ShowSyukeiTaisyogai();
            }
            catch (IOException)
            {
                MessageBox.Show(this, filePath + "\nファイルは保存できません。ファイルが開かれていないか確認してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 出力処理
        /// </summary>
        /// <param name="filePath">出力ファイルパス</param>
        private void Output(string filePath)
        {
            try
            {
                //**********************************************************************************************
                // 過去(CSV出力済み)であっても、出力対象月の一日～出力対象日に差分があればそれを出力する（差分）
                // 出力済みのCSVをそのまま出力することも選択可能とする（履歴から再出力）
                // 出力対象日のみの再集計・出力も可能とする（この日だけを再集計する）
                //**********************************************************************************************

                //***************************************************************************************
                // ※以下は差分を出力 または 履歴から再出力 の場合のみ
                // 出力対象日にデータが存在しない場合
                //   以下を集計して出力
                //   ・[売上日]＝出力対象日 の分
                //   ・出力対象月の一日≦[売上日]＜出力対象日 の差分
                //   ・[入金日]＝出力対象日 の分
                //   ・出力対象月の一日≦[入金日]＜出力対象日 の差分
                // 出力対象日にデータが存在する場合
                //   差分の場合：以下を集計して、追加分のみをCSV出力
                //               ・出力対象月の一日≦[売上日]＜出力対象日の翌日 の差分
                //               ・出力対象月の一日≦[入金日]＜出力対象日の翌日 の差分
                //   再出力の場合：[伝票日付]=出力対象日に該当する会計連携集計テーブルのレコードをCSV出力
                //***************************************************************************************

                //*********************************************************
                // ※この日だけ再集計 の場合のみ
                // 出力対象日に該当するレコードを削除し、以下を集計して出力
                //   ・[売上日]＝出力対象日 の分
                //   ・[入金日]＝出力対象日 の分
                //*********************************************************

                // 集計用出力対象日を初期化
                _kaikeiRenkei.集計用出力対象日 = _kaikeiRenkei.出力対象日;

                if (_kaikeiRenkei.出力対象 == KaikeiRenkei.KaikeiRenkeiTarget.Recalc)
                {
                    // 出力対象日だけを再集計する
                    SaiSyukei();

                    // [集計日]=出力対象日に該当する会計連携集計テーブルのレコードをCSV出力
                    OutputCSV(true, 0, filePath);
                }
                else if (_kaikeiRenkei.出力対象 == KaikeiRenkei.KaikeiRenkeiTarget.Diff)
                {
                    // 差分を出力する

                    // 出力対象日の最大伝票番号(=データの有無)を取得
                    int maxDenpyoNo = GetMaxDenpyoNo(true);

                    if (HasRecord() == false)
                    {
                        // データが無い？

                        // 集計する
                        if (Syukei(true, maxDenpyoNo) == false)
                        {
                            return;
                        }

                        // [集計日]=出力対象日に該当する会計連携集計テーブルのレコードをCSV出力
                        OutputCSV(true, maxDenpyoNo, filePath);
                    }
                    else
                    {
                        // データがある

                        // 集計用出力対象日を出力対象日の翌日にする。
                        // なぜか？
                        // SQLの
                        // 売上日 < @出力対象日
                        // を
                        // 売上日 <= @出力対象日
                        // にしたいが、SQLは変えたくないので
                        // 売上日 < @出力対象日の翌日
                        // にして実現する。
                        _kaikeiRenkei.SetNextDay();
                        if (Syukei(false, maxDenpyoNo) == false)
                        {
                            return;
                        }

                        // [集計日]=出力対象日に該当する会計連携集計テーブルのレコードをCSV出力
                        OutputCSV(true, maxDenpyoNo, filePath);
                    }
                }
                else
                {
                    // 履歴から再出力

                    // [伝票日付]=出力対象日に該当する会計連携集計テーブルのレコードをCSV出力して終了
                    if (OutputCSV(false, 0, filePath) == false)
                    {
                        return;
                    }
                }

                // 操作履歴に保存
                History.Save(this.Text, "会計連携CSV", null, History.ProcessType.Output);

                MessageBox.Show(this, "会計連携CSV出力が完了しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
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
        /// 最大伝票番号を取得
        /// </summary>
        /// <returns>出力対象日に該当する最大伝票番号</returns>
        private bool HasRecord()
        {
            return GetMaxDenpyoNo(false) != 0;
        }

        /// <summary>
        /// 最大伝票番号を取得
        /// </summary>
        /// <param name="isSyukeiDate">集計日で取得する場合はtrue、伝票日付で取得する場合はfalse</param>
        /// <returns>出力対象日に該当する最大伝票番号</returns>
        private int GetMaxDenpyoNo(bool isSyukeiDate)
        {
            try
            {
                int maxDenpyoNo = 0;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ISNULL(MAX(伝票番号), 0) AS 最大伝票番号" +
                                      "\r\n FROM" +
                                      "\r\n     会計連携集計テーブル" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ";

                    if (isSyukeiDate)
                    {
                        cmd.CommandText += "\r\n     AND 集計日 = @出力対象日";
                    }
                    else
                    {
                        cmd.CommandText += "\r\n     AND 伝票日付 = @出力対象日";
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        maxDenpyoNo = (int)reader["最大伝票番号"];
                    }
                }
                return maxDenpyoNo;
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
        /// 集計処理
        /// </summary>
        /// <param name="doToday">当日分も集計する場合はtrue。falseの場合は当日分の集計はせず、差分の集計のみを行なう</param>
        /// <param name="maxDenpyoNo">出力対象日の最大伝票番号</param>
        /// <returns>正常終了の場合はtrue</returns>
        private bool Syukei(bool doToday, int maxDenpyoNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    DataTable syukeiDt = new DataTable();   // それぞれの取得結果を格納するデータテーブル

                    //*********************************************************************************************************************
                    // 売上・入金のそれぞれについて、当日分の集計と前日までの差分を取得して会計連動集計テーブルに出力するが、
                    // ・伝票番号を部署・差分フラグ・伝票日付・仕訳ＣＤの順に採番したい
                    // ・金額がマイナスの場合は借方と貸方（のコード）を逆にする必要がある（その処理は１箇所にまとめたい）
                    // 以上の理由により、取得結果を１つのデータテーブルに格納し、そのデータテーブルのレコードを列挙して処理を行うようにする
                    // したがって、各取得処理は同じ項目(=列)を取得すること
                    //*********************************************************************************************************************

                    // 売上の集計（[売上日]＝出力対象日 の分）
                    if (doToday)
                    {
                        if (SyukeiUriageToday(con, tran, ref syukeiDt) == false)
                        {
                            return false;
                        }
                    }

                    // 売上の集計（出力対象月の一日≦[売上日]＜出力対象日 の分）
                    if (SyukeiUriagePast(con, tran, ref syukeiDt) == false)
                    {
                        return false;
                    }

                    // 入金の集計（[入金日]＝出力対象日 の分）
                    if (doToday)
                    {
                        if (SyukeiNyukinToday(con, tran, ref syukeiDt) == false)
                        {
                            return false;
                        }
                    }

                    // 入金の集計（出力対象月の一日≦[入金日]＜出力対象日 の分）
                    if (SyukeiNyukinPast(con, tran, ref syukeiDt) == false)
                    {
                        return false;
                    }

                    // 対象レコードがなければ出力しない
                    if (syukeiDt.Rows.Count == 0)
                    {
                        MessageBox.Show(this, "出力対象データはありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    // 差分フラグを設定する
                    SetDiffFlg(con, tran, ref syukeiDt);

                    // 結果を格納した集計テーブルをループし、伝票番号の採番と借方／貸方コードの調整をしながら、会計連動集計テーブルに出力する
                    if (OutputKaikeiRenkeiSyukei(con, tran, maxDenpyoNo, ref syukeiDt) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();

                    log.InfoFormat("会計連携集計テーブルを{0}しました。", History.ProcessType.Create.DisplayName());
                    return true;
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
        /// 売上の集計（[売上日]＝出力対象日 の分）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="syukeiDt">格納先のデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool SyukeiUriageToday(SqlConnection con, SqlTransaction tran, ref DataTable syukeiDt)
        {
            try
            {
                //*****************************
                // 当日分の売上の集計結果を取得
                //*****************************
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +               // -- 集計ＣＤ使用フラグOFF
                                  "\r\n     @出力対象日 AS 集計日" +
                                  "\r\n     ,SW.部署ＣＤ" +
                                  "\r\n     ,@出力対象日 AS 伝票日付" +
                                  "\r\n     ,SW.仕訳ＣＤ" +
                                  "\r\n     ,SW.集計ＣＤ" +
                                  "\r\n     ,SW.処理内容" +
                                  "\r\n     ,SW.借方ＣＤ" +
                                  "\r\n     ,SW.借方部門ＣＤ" +
                                  "\r\n     ,SW.借方補助ＣＤ" +     // -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
                                  "\r\n     ,SW.貸方ＣＤ" +
                                  "\r\n     ,SW.貸方部門ＣＤ" +
                                  "\r\n     ,SW.貸方補助ＣＤ" +     // -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
                                  "\r\n     ,SW.摘要" +
                                  "\r\n     ,SW.税率" +
                                  "\r\n     ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n     ,0 AS 売上入金のみフラグ" +
                                  "\r\n     ,0 AS 差分フラグ" +
                                  "\r\n FROM" +
                                  "\r\n     仕訳マスタ AS SW" +
                                  "\r\n     LEFT JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             企業ＣＤ" +
                                  "\r\n             ,部署ＣＤ" +
                                  "\r\n             ,売上区分" +
                                  "\r\n             ,チケット束フラグ" +
                                  "\r\n             ,SUM(商品金額合計税込) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             売上明細" +
                                  "\r\n         WHERE" +
                                  "\r\n             企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND 売上日 = @出力対象日" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             企業ＣＤ" +
                                  "\r\n             ,部署ＣＤ" +
                                  "\r\n             ,売上区分" +
                                  "\r\n             ,チケット束フラグ" +
                                  "\r\n         ) AS UM" +
                                  "\r\n         ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n         AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n         AND SW.売上区分 = UM.売上区分" +
                                  "\r\n         AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n WHERE" +
                                  "\r\n     SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n     AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n UNION" +
                                  // -- 集計ＣＤ使用フラグON
                                  "\r\n SELECT" +
                                  "\r\n     @出力対象日 AS 集計日" +
                                  "\r\n     ,SW.部署ＣＤ" +
                                  "\r\n     ,@出力対象日 AS 伝票日付" +
                                  "\r\n     ,SW.仕訳ＣＤ" +
                                  "\r\n     ,SW.集計ＣＤ" +
                                  "\r\n     ,SW.処理内容" +
                                  "\r\n     ,SW.借方ＣＤ" +
                                  "\r\n     ,SW.借方部門ＣＤ" +
                                  "\r\n     ,SW.借方補助ＣＤ" +
                                  "\r\n     ,SW.貸方ＣＤ" +
                                  "\r\n     ,SW.貸方部門ＣＤ" +
                                  "\r\n     ,SW.貸方補助ＣＤ" +
                                  "\r\n     ,SW.摘要" +
                                  "\r\n     ,SW.税率" +
                                  "\r\n     ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n     ,0 AS 売上入金のみフラグ" +
                                  "\r\n     ,0 AS 差分フラグ" +
                                  "\r\n FROM" +
                                  "\r\n     仕訳マスタ AS SW" +
                                  "\r\n     LEFT JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             M.企業ＣＤ" +
                                  "\r\n             ,M.部署ＣＤ" +
                                  "\r\n             ,T.集計ＣＤ" +
                                  "\r\n             ,M.売上区分" +
                                  "\r\n             ,M.チケット束フラグ" +
                                  "\r\n             ,SUM(M.商品金額合計税込) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             売上明細 AS M" +
                                  "\r\n             INNER JOIN 得意先マスタ AS T" +
                                  "\r\n             ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n             AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND M.売上日 = @出力対象日" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             M.企業ＣＤ" +
                                  "\r\n             ,M.部署ＣＤ" +
                                  "\r\n             ,T.集計ＣＤ" +
                                  "\r\n             ,M.売上区分" +
                                  "\r\n             ,M.チケット束フラグ" +
                                  "\r\n         ) AS UM" +
                                  "\r\n         ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n         AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n         AND SW.集計ＣＤ = UM.集計ＣＤ" +
                                  "\r\n         AND SW.売上区分 = UM.売上区分" +
                                  "\r\n         AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n WHERE" +
                                  "\r\n     SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n     AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1";      // -- 得意先マスタの集計CDと結合する

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                cmd.Parameters.Add("@集計対象区分売上", SqlDbType.Int).Value = (int)Consts.SyukeiTaisyoKubun.Uriage;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                // 格納先のデータテーブルが空っぽであれば取得（行数だと0件の場合があるので列数で判断）
                if (syukeiDt.Columns.Count == 0)
                {
                    adp.Fill(syukeiDt);
                }
                else
                {
                    // データを取得して、格納先のデータテーブルに追加
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow newDr = syukeiDt.NewRow();
                        newDr.ItemArray = dr.ItemArray;
                        syukeiDt.Rows.Add(newDr);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("当日分の売上の集計に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 売上の集計（出力対象月の一日≦[売上日]＜出力対象日 の分）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="syukeiDt">格納先のデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool SyukeiUriagePast(SqlConnection con, SqlTransaction tran, ref DataTable syukeiDt)
        {
            try
            {
                //*****************************************
                // 前日までの売上と集計テーブルの差分を取得
                //*****************************************
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     @出力対象日 AS 集計日" +
                                  "\r\n     ,SW.部署ＣＤ" +
                                  "\r\n     ,A.伝票日付" +
                                  "\r\n     ,SW.仕訳ＣＤ" +
                                  "\r\n     ,SW.集計ＣＤ" +
                                  "\r\n     ,SW.処理内容" +
                                  "\r\n     ,SW.借方ＣＤ" +
                                  "\r\n     ,SW.借方部門ＣＤ" +
                                  "\r\n     ,SW.借方補助ＣＤ" +
                                  "\r\n     ,SW.貸方ＣＤ" +
                                  "\r\n     ,SW.貸方部門ＣＤ" +
                                  "\r\n     ,SW.貸方補助ＣＤ" +
                                  "\r\n     ,SW.摘要" +
                                  "\r\n     ,SW.税率" +
                                  "\r\n     ,A.金額" +
                                  "\r\n     ,A.売上入金のみフラグ" +
                                  "\r\n     ,1 AS 差分フラグ" +
                                  "\r\n FROM" +
                                  // -- ①両方に存在（売上－集計テーブル）
                                  "\r\n     (SELECT" +
                                  "\r\n         A1.部署ＣＤ" +
                                  "\r\n         ,A1.伝票日付" +
                                  "\r\n         ,A1.仕訳ＣＤ" +
                                  "\r\n         ,A1.金額 - A2.金額 AS 金額" +
                                  "\r\n         ,0 AS 売上入金のみフラグ" +
                                  "\r\n     FROM" +
                                  // -- 売上（集計ＣＤ使用フラグOFF）
                                  "\r\n         (SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,UM.売上日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,売上日" +
                                  "\r\n                     ,売上区分" +
                                  "\r\n                     ,チケット束フラグ" +
                                  "\r\n                     ,SUM(商品金額合計税込) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     売上明細" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND 売上日 >= @出力対象月一日" +
                                  "\r\n                     AND 売上日 < @集計用出力対象日" +       // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,売上日" +
                                  "\r\n                     ,売上区分" +
                                  "\r\n                     ,チケット束フラグ" +
                                  "\r\n                 ) AS UM" +
                                  "\r\n                 ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n                 AND SW.売上区分 = UM.売上区分" +
                                  "\r\n                 AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n             AND UM.売上日 IS NOT NULL" +
                                  "\r\n         UNION" +
                                  // -- 売上（集計ＣＤ使用フラグON）
                                  "\r\n         SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,UM.売上日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.売上日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.売上区分" +
                                  "\r\n                     ,M.チケット束フラグ" +
                                  "\r\n                     ,SUM(M.商品金額合計税込) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     売上明細 AS M" +
                                  "\r\n                     INNER JOIN 得意先マスタ AS T" +
                                  "\r\n                     ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n                     AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND M.売上日 >= @出力対象月一日" +
                                  "\r\n                     AND M.売上日 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.売上日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.売上区分" +
                                  "\r\n                     ,M.チケット束フラグ" +
                                  "\r\n                 ) AS UM" +
                                  "\r\n                 ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n                 AND SW.集計ＣＤ = UM.集計ＣＤ" +
                                  "\r\n                 AND SW.売上区分 = UM.売上区分" +
                                  "\r\n                 AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1" +     // -- 得意先マスタの集計CDと結合する
                                  "\r\n             AND UM.売上日 IS NOT NULL" +
                                  "\r\n         ) AS A1" +
                                  "\r\n         INNER JOIN" +
                                  // -- 会計連携集計テーブルを前日までの差分取得用に集計
                                  "\r\n             (SELECT" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n                 ,SUM(K.金額) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 会計連携集計テーブル AS K" +
                                  "\r\n                 INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n                     ON K.企業ＣＤ = SW.企業ＣＤ" +
                                  "\r\n                     AND K.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n                     AND K.仕訳ＣＤ = SW.仕訳ＣＤ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 K.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND K.伝票日付 >= @出力対象月一日" +
                                  "\r\n                 AND K.伝票日付 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n             GROUP BY" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n             ) AS A2" +
                                  "\r\n             ON A1.部署ＣＤ = A2.部署ＣＤ" +
                                  "\r\n             AND A1.伝票日付 = A2.伝票日付" +
                                  "\r\n             AND A1.仕訳ＣＤ = A2.仕訳ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         (A1.金額 - A2.金額) <> 0" +
                                  "\r\n     UNION" +
                                  // -- ②売上に存在（売上のみ）
                                  "\r\n     SELECT" +
                                  "\r\n         A1.部署ＣＤ" +
                                  "\r\n         ,A1.伝票日付" +
                                  "\r\n         ,A1.仕訳ＣＤ" +
                                  "\r\n         ,A1.金額" +
                                  "\r\n         ,1 AS 売上入金のみフラグ" +
                                  "\r\n     FROM" +
                                  // -- 売上（集計ＣＤ使用フラグOFF）
                                  "\r\n         (SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,UM.売上日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,売上日" +
                                  "\r\n                     ,売上区分" +
                                  "\r\n                     ,チケット束フラグ" +
                                  "\r\n                     ,SUM(商品金額合計税込) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     売上明細" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND 売上日 >= @出力対象月一日" +
                                  "\r\n                     AND 売上日 < @集計用出力対象日" +       // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,売上日" +
                                  "\r\n                     ,売上区分" +
                                  "\r\n                     ,チケット束フラグ" +
                                  "\r\n                 ) AS UM" +
                                  "\r\n                 ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n                 AND SW.売上区分 = UM.売上区分" +
                                  "\r\n                 AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n             AND UM.売上日 IS NOT NULL" +
                                  "\r\n         UNION" +
                                  // -- 売上（集計ＣＤ使用フラグON）
                                  "\r\n         SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,UM.売上日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.売上日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.売上区分" +
                                  "\r\n                     ,M.チケット束フラグ" +
                                  "\r\n                     ,SUM(M.商品金額合計税込) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     売上明細 AS M" +
                                  "\r\n                     INNER JOIN 得意先マスタ AS T" +
                                  "\r\n                     ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n                     AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND M.売上日 >= @出力対象月一日" +
                                  "\r\n                     AND M.売上日 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.売上日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.売上区分" +
                                  "\r\n                     ,M.チケット束フラグ" +
                                  "\r\n                 ) AS UM" +
                                  "\r\n                 ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n                 AND SW.集計ＣＤ = UM.集計ＣＤ" +
                                  "\r\n                 AND SW.売上区分 = UM.売上区分" +
                                  "\r\n                 AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1" +     // -- 得意先マスタの集計CDと結合する
                                  "\r\n             AND UM.売上日 IS NOT NULL" +
                                  "\r\n         ) AS A1" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 会計連携集計テーブルを前日までの差分取得用に集計
                                  "\r\n             (SELECT" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n                 ,SUM(K.金額) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 会計連携集計テーブル AS K" +
                                  "\r\n                 INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n                     ON K.企業ＣＤ = SW.企業ＣＤ" +
                                  "\r\n                     AND K.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n                     AND K.仕訳ＣＤ = SW.仕訳ＣＤ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 K.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND K.伝票日付 >= @出力対象月一日" +
                                  "\r\n                 AND K.伝票日付 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n             GROUP BY" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n             ) AS A2" +
                                  "\r\n             ON A1.部署ＣＤ = A2.部署ＣＤ" +
                                  "\r\n             AND A1.伝票日付 = A2.伝票日付" +
                                  "\r\n             AND A1.仕訳ＣＤ = A2.仕訳ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         A2.仕訳ＣＤ IS NULL" +
                                  "\r\n     UNION" +
                                  // -- ③集計テーブルに存在（集計値をマイナスとして）
                                  "\r\n     SELECT" +
                                  "\r\n         A2.部署ＣＤ" +
                                  "\r\n         ,A2.伝票日付" +
                                  "\r\n         ,A2.仕訳ＣＤ" +
                                  "\r\n         ,A2.金額 * -1 AS 金額" +
                                  "\r\n         ,0 AS 売上入金のみフラグ" +
                                  "\r\n     FROM" +
                                  // -- 会計連携集計テーブルを前日までの差分取得用に集計
                                  "\r\n         (SELECT" +
                                  "\r\n             K.部署ＣＤ" +
                                  "\r\n             ,K.伝票日付" +
                                  "\r\n             ,K.仕訳ＣＤ" +
                                  "\r\n             ,SUM(K.金額) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             会計連携集計テーブル AS K" +
                                  "\r\n             INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n                 ON K.企業ＣＤ = SW.企業ＣＤ" +
                                  "\r\n                 AND K.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n                 AND K.仕訳ＣＤ = SW.仕訳ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             K.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND K.伝票日付 >= @出力対象月一日" +
                                  "\r\n             AND K.伝票日付 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             K.部署ＣＤ" +
                                  "\r\n             ,K.伝票日付" +
                                  "\r\n             ,K.仕訳ＣＤ" +
                                  "\r\n         ) AS A2" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 売上（集計ＣＤ使用フラグOFF）
                                  "\r\n             (SELECT" +
                                  "\r\n                 SW.部署ＣＤ" +
                                  "\r\n                 ,UM.売上日 AS 伝票日付" +
                                  "\r\n                 ,SW.仕訳ＣＤ" +
                                  "\r\n                 ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 仕訳マスタ AS SW" +
                                  "\r\n                 LEFT JOIN" +
                                  "\r\n                     (SELECT" +
                                  "\r\n                         企業ＣＤ" +
                                  "\r\n                         ,部署ＣＤ" +
                                  "\r\n                         ,売上日" +
                                  "\r\n                         ,売上区分" +
                                  "\r\n                         ,チケット束フラグ" +
                                  "\r\n                         ,SUM(商品金額合計税込) AS 金額" +
                                  "\r\n                     FROM" +
                                  "\r\n                         売上明細" +
                                  "\r\n                     WHERE" +
                                  "\r\n                         企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                         AND 売上日 >= @出力対象月一日" +
                                  "\r\n                         AND 売上日 < @集計用出力対象日" +       // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                     GROUP BY" +
                                  "\r\n                         企業ＣＤ" +
                                  "\r\n                         ,部署ＣＤ" +
                                  "\r\n                         ,売上日" +
                                  "\r\n                         ,売上区分" +
                                  "\r\n                         ,チケット束フラグ" +
                                  "\r\n                     ) AS UM" +
                                  "\r\n                     ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n                     AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n                     AND SW.売上区分 = UM.売上区分" +
                                  "\r\n                     AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n                 AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n                 AND UM.売上日 IS NOT NULL" +
                                  "\r\n             UNION" +
                                  // -- 売上（集計ＣＤ使用フラグON）
                                  "\r\n             SELECT" +
                                  "\r\n                 SW.部署ＣＤ" +
                                  "\r\n                 ,UM.売上日 AS 伝票日付" +
                                  "\r\n                 ,SW.仕訳ＣＤ" +
                                  "\r\n                 ,ISNULL(UM.金額, 0) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 仕訳マスタ AS SW" +
                                  "\r\n                 LEFT JOIN" +
                                  "\r\n                     (SELECT" +
                                  "\r\n                         M.企業ＣＤ" +
                                  "\r\n                         ,M.部署ＣＤ" +
                                  "\r\n                         ,M.売上日" +
                                  "\r\n                         ,T.集計ＣＤ" +
                                  "\r\n                         ,M.売上区分" +
                                  "\r\n                         ,M.チケット束フラグ" +
                                  "\r\n                         ,SUM(M.商品金額合計税込) AS 金額" +
                                  "\r\n                     FROM" +
                                  "\r\n                         売上明細 AS M" +
                                  "\r\n                         INNER JOIN 得意先マスタ AS T" +
                                  "\r\n                         ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n                         AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n                     WHERE" +
                                  "\r\n                         M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                         AND M.売上日 >= @出力対象月一日" +
                                  "\r\n                         AND M.売上日 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 売上日 <= @出力対象日 つまり 売上日 < @出力対象日の翌日 となる
                                  "\r\n                     GROUP BY" +
                                  "\r\n                         M.企業ＣＤ" +
                                  "\r\n                         ,M.部署ＣＤ" +
                                  "\r\n                         ,M.売上日" +
                                  "\r\n                         ,T.集計ＣＤ" +
                                  "\r\n                         ,M.売上区分" +
                                  "\r\n                         ,M.チケット束フラグ" +
                                  "\r\n                     ) AS UM" +
                                  "\r\n                     ON SW.企業ＣＤ = UM.企業ＣＤ" +
                                  "\r\n                     AND SW.部署ＣＤ = UM.部署ＣＤ" +
                                  "\r\n                     AND SW.集計ＣＤ = UM.集計ＣＤ" +
                                  "\r\n                     AND SW.売上区分 = UM.売上区分" +
                                  "\r\n                     AND SW.チケット束フラグ = UM.チケット束フラグ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分売上" +
                                  "\r\n                 AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1" +     // -- 得意先マスタの集計CDと結合する
                                  "\r\n                 AND UM.売上日 IS NOT NULL" +
                                  "\r\n             ) AS A1" +
                                  "\r\n             ON A2.部署ＣＤ = A1.部署ＣＤ" +
                                  "\r\n             AND A2.伝票日付 = A1.伝票日付" +
                                  "\r\n             AND A2.仕訳ＣＤ = A1.仕訳ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         A1.仕訳ＣＤ IS NULL" +
                                  "\r\n         AND A2.金額 <> 0" +
                                  "\r\n     ) AS A" +
                                  "\r\n     INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n         ON SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n         AND A.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n         AND A.仕訳ＣＤ = SW.仕訳ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                cmd.Parameters.Add("@集計用出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.集計用出力対象日;
                cmd.Parameters.Add("@集計対象区分売上", SqlDbType.Int).Value = (int)Consts.SyukeiTaisyoKubun.Uriage;
                cmd.Parameters.Add("@出力対象月一日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象月一日;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                // 格納先のデータテーブルが空っぽであれば取得（行数だと0件の場合があるので列数で判断）
                if (syukeiDt.Columns.Count == 0)
                {
                    adp.Fill(syukeiDt);
                }
                else
                {
                    // データを取得して、格納先のデータテーブルに追加
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow newDr = syukeiDt.NewRow();
                        newDr.ItemArray = dr.ItemArray;
                        syukeiDt.Rows.Add(newDr);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("前日までの売上の集計に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 入金の集計（[入金日]＝出力対象日 の分）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="syukeiDt">格納先のデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool SyukeiNyukinToday(SqlConnection con, SqlTransaction tran, ref DataTable syukeiDt)
        {
            try
            {
                //*****************************
                // 当日分の入金の集計結果を取得
                //*****************************
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +               // -- 集計ＣＤ使用フラグOFF
                                  "\r\n     @出力対象日 AS 集計日" +
                                  "\r\n     ,SW.部署ＣＤ" +
                                  "\r\n     ,@出力対象日 AS 伝票日付" +
                                  "\r\n     ,SW.仕訳ＣＤ" +
                                  "\r\n     ,SW.集計ＣＤ" +
                                  "\r\n     ,SW.処理内容" +
                                  "\r\n     ,SW.借方ＣＤ" +
                                  "\r\n     ,SW.借方部門ＣＤ" +
                                  "\r\n     ,SW.借方補助ＣＤ" +     // -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
                                  "\r\n     ,SW.貸方ＣＤ" +
                                  "\r\n     ,SW.貸方部門ＣＤ" +
                                  "\r\n     ,SW.貸方補助ＣＤ" +     // -- リテラル または [部署ＣＤ], [集計ＣＤ], [借方部門ＣＤ], [貸方部門ＣＤ]
                                  "\r\n     ,SW.摘要" +
                                  "\r\n     ,SW.税率" +
                                  "\r\n     --,SW.入金方法" +
                                  "\r\n     ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n     ,0 AS 売上入金のみフラグ" +
                                  "\r\n     ,0 AS 差分フラグ" +
                                  "\r\n FROM" +
                                  "\r\n     仕訳マスタ AS SW" +
                                  "\r\n     LEFT JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             企業ＣＤ" +
                                  "\r\n             ,部署ＣＤ" +
                                  "\r\n             ,入金方法" +
                                  "\r\n             ,SUM(入金額) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             入金明細" +
                                  "\r\n         WHERE" +
                                  "\r\n             企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND 入金日 = @出力対象日" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             企業ＣＤ" +
                                  "\r\n             ,部署ＣＤ" +
                                  "\r\n             ,入金方法" +
                                  "\r\n         ) AS NM" +
                                  "\r\n         ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n         AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n         AND SW.入金方法 = NM.入金方法" +
                                  "\r\n WHERE" +
                                  "\r\n     SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n     AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n UNION" +
                                  // -- 集計ＣＤ使用フラグON
                                  "\r\n SELECT" +
                                  "\r\n     @出力対象日 AS 集計日" +
                                  "\r\n     ,SW.部署ＣＤ" +
                                  "\r\n     ,@出力対象日 AS 伝票日付" +
                                  "\r\n     ,SW.仕訳ＣＤ" +
                                  "\r\n     ,SW.集計ＣＤ" +
                                  "\r\n     ,SW.処理内容" +
                                  "\r\n     ,SW.借方ＣＤ" +
                                  "\r\n     ,SW.借方部門ＣＤ" +
                                  "\r\n     ,SW.借方補助ＣＤ" +
                                  "\r\n     ,SW.貸方ＣＤ" +
                                  "\r\n     ,SW.貸方部門ＣＤ" +
                                  "\r\n     ,SW.貸方補助ＣＤ" +
                                  "\r\n     ,SW.摘要" +
                                  "\r\n     ,SW.税率" +
                                  "\r\n     --,SW.入金方法" +
                                  "\r\n     ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n     ,0 AS 売上入金のみフラグ" +
                                  "\r\n     ,0 AS 差分フラグ" +
                                  "\r\n FROM" +
                                  "\r\n     仕訳マスタ AS SW" +
                                  "\r\n     LEFT JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             M.企業ＣＤ" +
                                  "\r\n             ,M.部署ＣＤ" +
                                  "\r\n             ,T.集計ＣＤ" +
                                  "\r\n             ,M.入金方法" +
                                  "\r\n             ,SUM(M.入金額) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             入金明細 AS M" +
                                  "\r\n             INNER JOIN 得意先マスタ AS T" +
                                  "\r\n             ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n             AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND M.入金日 = @出力対象日" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             M.企業ＣＤ" +
                                  "\r\n             ,M.部署ＣＤ" +
                                  "\r\n             ,T.集計ＣＤ" +
                                  "\r\n             ,M.入金方法" +
                                  "\r\n         ) AS NM" +
                                  "\r\n         ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n         AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n         AND SW.集計ＣＤ = NM.集計ＣＤ" +
                                  "\r\n         AND SW.入金方法 = NM.入金方法" +
                                  "\r\n WHERE" +
                                  "\r\n     SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n     AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1";      // -- 得意先マスタの集計CDと結合する

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                cmd.Parameters.Add("@集計対象区分入金", SqlDbType.Int).Value = (int)Consts.SyukeiTaisyoKubun.Nyukin;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                // 格納先のデータテーブルが空っぽであれば取得（行数だと0件の場合があるので列数で判断）
                if (syukeiDt.Columns.Count == 0)
                {
                    adp.Fill(syukeiDt);
                }
                else
                {
                    // データを取得して、格納先のデータテーブルに追加
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow newDr = syukeiDt.NewRow();
                        newDr.ItemArray = dr.ItemArray;
                        syukeiDt.Rows.Add(newDr);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("当日分の入金の集計に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 入金の集計（出力対象月の一日≦[入金日]＜出力対象日 の分）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="syukeiDt">格納先のデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool SyukeiNyukinPast(SqlConnection con, SqlTransaction tran, ref DataTable syukeiDt)
        {
            try
            {
                //*****************************************
                // 前日までの入金と集計テーブルの差分を取得
                //*****************************************
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     @出力対象日 AS 集計日" +
                                  "\r\n     ,SW.部署ＣＤ" +
                                  "\r\n     ,A.伝票日付" +
                                  "\r\n     ,SW.仕訳ＣＤ" +
                                  "\r\n     ,SW.集計ＣＤ" +
                                  "\r\n     ,SW.処理内容" +
                                  "\r\n     ,SW.借方ＣＤ" +
                                  "\r\n     ,SW.借方部門ＣＤ" +
                                  "\r\n     ,SW.借方補助ＣＤ" +
                                  "\r\n     ,SW.貸方ＣＤ" +
                                  "\r\n     ,SW.貸方部門ＣＤ" +
                                  "\r\n     ,SW.貸方補助ＣＤ" +
                                  "\r\n     ,SW.摘要" +
                                  "\r\n     ,SW.税率" +
                                  "\r\n     ,A.金額" +
                                  "\r\n     ,A.売上入金のみフラグ" +
                                  "\r\n     ,1 AS 差分フラグ" +
                                  "\r\n FROM" +
                                  // -- ①両方に存在（入金－集計テーブル）
                                  "\r\n     (SELECT" +
                                  "\r\n         A1.部署ＣＤ" +
                                  "\r\n         ,A1.伝票日付" +
                                  "\r\n         ,A1.仕訳ＣＤ" +
                                  "\r\n         ,A1.金額 - A2.金額 AS 金額" +
                                  "\r\n         ,0 AS 売上入金のみフラグ" +
                                  "\r\n     FROM" +
                                  // -- 入金（集計ＣＤ使用フラグOFF）
                                  "\r\n         (SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,NM.入金日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,入金日" +
                                  "\r\n                     ,入金方法" +
                                  "\r\n                     ,SUM(入金額) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     入金明細" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND 入金日 >= @出力対象月一日" +
                                  "\r\n                     AND 入金日 < @集計用出力対象日" +       // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,入金日" +
                                  "\r\n                     ,入金方法" +
                                  "\r\n                 ) AS NM" +
                                  "\r\n                 ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n                 AND SW.入金方法 = NM.入金方法" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n             AND NM.入金日 IS NOT NULL" +
                                  "\r\n         UNION" +
                                  // -- 入金（集計ＣＤ使用フラグON）
                                  "\r\n         SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,NM.入金日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.入金日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.入金方法" +
                                  "\r\n                     ,SUM(M.入金額) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     入金明細 AS M" +
                                  "\r\n                     INNER JOIN 得意先マスタ AS T" +
                                  "\r\n                     ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n                     AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND M.入金日 >= @出力対象月一日" +
                                  "\r\n                     AND M.入金日 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.入金日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.入金方法" +
                                  "\r\n                 ) AS NM" +
                                  "\r\n                 ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n                 AND SW.集計ＣＤ = NM.集計ＣＤ" +
                                  "\r\n                 AND SW.入金方法 = NM.入金方法" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1" +     // -- 得意先マスタの集計CDと結合する
                                  "\r\n             AND NM.入金日 IS NOT NULL" +
                                  "\r\n         ) AS A1" +
                                  "\r\n         INNER JOIN" +
                                  // -- 会計連携集計テーブルを前日までの差分取得用に集計
                                  "\r\n             (SELECT" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n                 ,SUM(K.金額) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 会計連携集計テーブル AS K" +
                                  "\r\n                 INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n                     ON K.企業ＣＤ = SW.企業ＣＤ" +
                                  "\r\n                     AND K.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n                     AND K.仕訳ＣＤ = SW.仕訳ＣＤ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 K.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND K.伝票日付 >= @出力対象月一日" +
                                  "\r\n                 AND K.伝票日付 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n             GROUP BY" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n             ) AS A2" +
                                  "\r\n             ON A1.部署ＣＤ = A2.部署ＣＤ" +
                                  "\r\n             AND A1.伝票日付 = A2.伝票日付" +
                                  "\r\n             AND A1.仕訳ＣＤ = A2.仕訳ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         (A1.金額 - A2.金額) <> 0" +
                                  "\r\n     UNION" +
                                  // -- ②入金に存在（入金のみ）
                                  "\r\n     SELECT" +
                                  "\r\n         A1.部署ＣＤ" +
                                  "\r\n         ,A1.伝票日付" +
                                  "\r\n         ,A1.仕訳ＣＤ" +
                                  "\r\n         ,A1.金額" +
                                  "\r\n         ,1 AS 売上入金のみフラグ" +
                                  "\r\n     FROM" +
                                  // -- 入金（集計ＣＤ使用フラグOFF）
                                  "\r\n         (SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,NM.入金日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,入金日" +
                                  "\r\n                     ,入金方法" +
                                  "\r\n                     ,SUM(入金額) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     入金明細" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND 入金日 >= @出力対象月一日" +
                                  "\r\n                     AND 入金日 < @集計用出力対象日" +       // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     企業ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,入金日" +
                                  "\r\n                     ,入金方法" +
                                  "\r\n                 ) AS NM" +
                                  "\r\n                 ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n                 AND SW.入金方法 = NM.入金方法" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n             AND NM.入金日 IS NOT NULL" +
                                  "\r\n         UNION" +
                                  // -- 入金（集計ＣＤ使用フラグON）
                                  "\r\n         SELECT" +
                                  "\r\n             SW.部署ＣＤ" +
                                  "\r\n             ,NM.入金日 AS 伝票日付" +
                                  "\r\n             ,SW.仕訳ＣＤ" +
                                  "\r\n             ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             仕訳マスタ AS SW" +
                                  "\r\n             LEFT JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.入金日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.入金方法" +
                                  "\r\n                     ,SUM(M.入金額) AS 金額" +
                                  "\r\n                 FROM" +
                                  "\r\n                     入金明細 AS M" +
                                  "\r\n                     INNER JOIN 得意先マスタ AS T" +
                                  "\r\n                     ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n                     AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND M.入金日 >= @出力対象月一日" +
                                  "\r\n                     AND M.入金日 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     M.企業ＣＤ" +
                                  "\r\n                     ,M.部署ＣＤ" +
                                  "\r\n                     ,M.入金日" +
                                  "\r\n                     ,T.集計ＣＤ" +
                                  "\r\n                     ,M.入金方法" +
                                  "\r\n                 ) AS NM" +
                                  "\r\n                 ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n                 AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n                 AND SW.集計ＣＤ = NM.集計ＣＤ" +
                                  "\r\n                 AND SW.入金方法 = NM.入金方法" +
                                  "\r\n         WHERE" +
                                  "\r\n             SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n             AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1" +     // -- 得意先マスタの集計CDと結合する
                                  "\r\n             AND NM.入金日 IS NOT NULL" +
                                  "\r\n         ) AS A1" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 会計連携集計テーブルを前日までの差分取得用に集計
                                  "\r\n             (SELECT" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n                 ,SUM(K.金額) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 会計連携集計テーブル AS K" +
                                  "\r\n                 INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n                     ON K.企業ＣＤ = SW.企業ＣＤ" +
                                  "\r\n                     AND K.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n                     AND K.仕訳ＣＤ = SW.仕訳ＣＤ" +
                                  "\r\n             WHERE" +
                                  "\r\n                 K.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND K.伝票日付 >= @出力対象月一日" +
                                  "\r\n                 AND K.伝票日付 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n             GROUP BY" +
                                  "\r\n                 K.部署ＣＤ" +
                                  "\r\n                 ,K.伝票日付" +
                                  "\r\n                 ,K.仕訳ＣＤ" +
                                  "\r\n             ) AS A2" +
                                  "\r\n             ON A1.部署ＣＤ = A2.部署ＣＤ" +
                                  "\r\n             AND A1.伝票日付 = A2.伝票日付" +
                                  "\r\n             AND A1.仕訳ＣＤ = A2.仕訳ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         A2.仕訳ＣＤ IS NULL" +
                                  "\r\n     UNION" +
                                  // -- ③集計テーブルに存在（集計値をマイナスとして）
                                  "\r\n     SELECT" +
                                  "\r\n         A2.部署ＣＤ" +
                                  "\r\n         ,A2.伝票日付" +
                                  "\r\n         ,A2.仕訳ＣＤ" +
                                  "\r\n         ,A2.金額 * -1 AS 金額" +
                                  "\r\n         ,0 AS 売上入金のみフラグ" +
                                  "\r\n     FROM" +
                                  // -- 会計連携集計テーブルを前日までの差分取得用に集計
                                  "\r\n         (SELECT" +
                                  "\r\n             K.部署ＣＤ" +
                                  "\r\n             ,K.伝票日付" +
                                  "\r\n             ,K.仕訳ＣＤ" +
                                  "\r\n             ,SUM(K.金額) AS 金額" +
                                  "\r\n         FROM" +
                                  "\r\n             会計連携集計テーブル AS K" +
                                  "\r\n             INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n                 ON K.企業ＣＤ = SW.企業ＣＤ" +
                                  "\r\n                 AND K.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n                 AND K.仕訳ＣＤ = SW.仕訳ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             K.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND K.伝票日付 >= @出力対象月一日" +
                                  "\r\n             AND K.伝票日付 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n             AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             K.部署ＣＤ" +
                                  "\r\n             ,K.伝票日付" +
                                  "\r\n             ,K.仕訳ＣＤ" +
                                  "\r\n         ) AS A2" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 入金（集計ＣＤ使用フラグOFF）
                                  "\r\n             (SELECT" +
                                  "\r\n                 SW.部署ＣＤ" +
                                  "\r\n                 ,NM.入金日 AS 伝票日付" +
                                  "\r\n                 ,SW.仕訳ＣＤ" +
                                  "\r\n                 ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 仕訳マスタ AS SW" +
                                  "\r\n                 LEFT JOIN" +
                                  "\r\n                     (SELECT" +
                                  "\r\n                         企業ＣＤ" +
                                  "\r\n                         ,部署ＣＤ" +
                                  "\r\n                         ,入金日" +
                                  "\r\n                         ,入金方法" +
                                  "\r\n                         ,SUM(入金額) AS 金額" +
                                  "\r\n                     FROM" +
                                  "\r\n                         入金明細" +
                                  "\r\n                     WHERE" +
                                  "\r\n                         企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                         AND 入金日 >= @出力対象月一日" +
                                  "\r\n                         AND 入金日 < @集計用出力対象日" +       // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                     GROUP BY" +
                                  "\r\n                         企業ＣＤ" +
                                  "\r\n                         ,部署ＣＤ" +
                                  "\r\n                         ,入金日" +
                                  "\r\n                         ,入金方法" +
                                  "\r\n                     ) AS NM" +
                                  "\r\n                     ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n                     AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n                     AND SW.入金方法 = NM.入金方法" +
                                  "\r\n             WHERE" +
                                  "\r\n                 SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n                 AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 0" +     // -- 得意先マスタの集計CDと結合しない
                                  "\r\n                 AND NM.入金日 IS NOT NULL" +
                                  "\r\n             UNION" +
                                  // -- 入金（集計ＣＤ使用フラグON）
                                  "\r\n             SELECT" +
                                  "\r\n                 SW.部署ＣＤ" +
                                  "\r\n                 ,NM.入金日 AS 伝票日付" +
                                  "\r\n                 ,SW.仕訳ＣＤ" +
                                  "\r\n                 ,ISNULL(NM.金額, 0) AS 金額" +
                                  "\r\n             FROM" +
                                  "\r\n                 仕訳マスタ AS SW" +
                                  "\r\n                 LEFT JOIN" +
                                  "\r\n                     (SELECT" +
                                  "\r\n                         M.企業ＣＤ" +
                                  "\r\n                         ,M.部署ＣＤ" +
                                  "\r\n                         ,M.入金日" +
                                  "\r\n                         ,T.集計ＣＤ" +
                                  "\r\n                         ,M.入金方法" +
                                  "\r\n                         ,SUM(M.入金額) AS 金額" +
                                  "\r\n                     FROM" +
                                  "\r\n                         入金明細 AS M" +
                                  "\r\n                         INNER JOIN 得意先マスタ AS T" +
                                  "\r\n                         ON M.企業ＣＤ = T.企業ＣＤ" +
                                  "\r\n                         AND M.得意先ＣＤ = T.得意先ＣＤ" +
                                  "\r\n                     WHERE" +
                                  "\r\n                         M.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                         AND M.入金日 >= @出力対象月一日" +
                                  "\r\n                         AND M.入金日 < @集計用出力対象日" +     // -- 当日の差分を抽出する場合は 入金日 <= @出力対象日 つまり 入金日 < @出力対象日の翌日 となる
                                  "\r\n                     GROUP BY" +
                                  "\r\n                         M.企業ＣＤ" +
                                  "\r\n                         ,M.部署ＣＤ" +
                                  "\r\n                         ,M.入金日" +
                                  "\r\n                         ,T.集計ＣＤ" +
                                  "\r\n                         ,M.入金方法" +
                                  "\r\n                     ) AS NM" +
                                  "\r\n                     ON SW.企業ＣＤ = NM.企業ＣＤ" +
                                  "\r\n                     AND SW.部署ＣＤ = NM.部署ＣＤ" +
                                  "\r\n                     AND SW.集計ＣＤ = NM.集計ＣＤ" +
                                  "\r\n                     AND SW.入金方法 = NM.入金方法" +
                                  "\r\n             WHERE" +
                                  "\r\n                 SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                 AND SW.集計対象区分 = @集計対象区分入金" +
                                  "\r\n                 AND ISNULL(SW.集計ＣＤ使用フラグ, 0) = 1" +     // -- 得意先マスタの集計CDと結合する
                                  "\r\n                 AND NM.入金日 IS NOT NULL" +
                                  "\r\n             ) AS A1" +
                                  "\r\n             ON A2.部署ＣＤ = A1.部署ＣＤ" +
                                  "\r\n             AND A2.伝票日付 = A1.伝票日付" +
                                  "\r\n             AND A2.仕訳ＣＤ = A1.仕訳ＣＤ" +
                                  "\r\n     WHERE" +
                                  "\r\n         A1.仕訳ＣＤ IS NULL" +
                                  "\r\n         AND A2.金額 <> 0" +
                                  "\r\n     ) AS A" +
                                  "\r\n     INNER JOIN 仕訳マスタ AS SW" +
                                  "\r\n         ON SW.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n         AND A.部署ＣＤ = SW.部署ＣＤ" +
                                  "\r\n         AND A.仕訳ＣＤ = SW.仕訳ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                cmd.Parameters.Add("@集計用出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.集計用出力対象日;
                cmd.Parameters.Add("@集計対象区分入金", SqlDbType.Int).Value = (int)Consts.SyukeiTaisyoKubun.Nyukin;
                cmd.Parameters.Add("@出力対象月一日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象月一日;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                // 格納先のデータテーブルが空っぽであれば取得（行数だと0件の場合があるので列数で判断）
                if (syukeiDt.Columns.Count == 0)
                {
                    adp.Fill(syukeiDt);
                }
                else
                {
                    // データを取得して、格納先のデータテーブルに追加
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        DataRow newDr = syukeiDt.NewRow();
                        newDr.ItemArray = dr.ItemArray;
                        syukeiDt.Rows.Add(newDr);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("前日までの入金の集計に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 差分フラグを設定する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="syukeiDt">格納先のデータテーブル</param>
        private void SetDiffFlg(SqlConnection con, SqlTransaction tran, ref DataTable syukeiDt)
        {
            try
            {
                // 対象月の最大集計日を取得
                DateTime? maxSyukeiDate;

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT ");
                sql.AppendLine("   MAX(集計日) AS 最大集計日 ");
                sql.AppendLine(" FROM ");
                sql.AppendLine("   会計連携集計テーブル ");
                sql.AppendLine(" WHERE ");
                sql.AppendLine("   企業ＣＤ = @企業ＣＤ ");
                sql.AppendLine("   AND 伝票日付 >= @出力対象月一日 ");
                sql.AppendLine("   AND 伝票日付 <= @出力対象月末日 ");
                cmd.CommandText = sql.ToString();

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@出力対象月一日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象月一日;
                cmd.Parameters.Add("@出力対象月末日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象月末日;

                log.Debug(cmd.CommandTextWithParameters());

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    maxSyukeiDate = reader["最大集計日"] as DateTime?;
                }

                foreach (DataRow dr in syukeiDt.Rows)
                {
                    if ((int)dr["売上入金のみフラグ"] == 1)
                    {
                        if (maxSyukeiDate == null)
                        {
                            dr["差分フラグ"] = 0;
                        }
                        else if ((DateTime)dr["伝票日付"] > maxSyukeiDate)
                        {
                            dr["差分フラグ"] = 0;
                        }
                        else
                        {
                            dr["差分フラグ"] = 1;
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
        /// 会計連動集計テーブルに出力
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="maxDenpyoNo">出力対象日の最大伝票番号</param>
        /// <param name="syukeiDt">格納先のデータテーブル</param>
        /// <returns>成功した場合は True</returns>
        private bool OutputKaikeiRenkeiSyukei(SqlConnection con, SqlTransaction tran, int maxDenpyoNo, ref DataTable syukeiDt)
        {
            try
            {
                int denpyoNo = maxDenpyoNo;       // 伝票番号

                // 結果を格納した集計テーブルをループし、伝票番号の採番と借方／貸方コードの調整をしながら、会計連動集計テーブルに出力する
                DataRow[] drs = syukeiDt.Select("", "部署ＣＤ, 差分フラグ, 伝票日付, 仕訳ＣＤ");
                foreach (DataRow dr in drs)
                {
                    KaikeiRenkeiSyukei kaikeiRenkeiSyukei = new KaikeiRenkeiSyukei();
                    kaikeiRenkeiSyukei.Import(dr);
                    kaikeiRenkeiSyukei.SetMinusFlg();
                    denpyoNo++;
                    kaikeiRenkeiSyukei.伝票番号 = denpyoNo;
                    if (kaikeiRenkeiSyukei.Insert(con, tran) == false)
                    {
                        return false;
                    }
                }

                // 操作履歴に保存
                History.Save(con, tran, this.Text, "会計連携集計テーブル", string.Format("{0:yyyy/MM/dd}", _kaikeiRenkei.出力対象日), History.ProcessType.Create);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("会計連動集計テーブルの出力に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// この日だけを再集計する
        /// </summary>
        /// <returns>成功した場合は True</returns>
        private bool SaiSyukei()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    // 出力対象日に該当する会計連携集計テーブルを削除する
                    if (DeleteKaikeiRenkeiSyukei(con, tran) == false)
                    {
                        return false;
                    }

                    DataTable syukeiDt = new DataTable();   // それぞれの取得結果を格納するデータテーブル

                    //*********************************************************************************************************************
                    // 売上・入金のそれぞれについて、当日分の集計を取得して会計連動集計テーブルに出力するが、
                    // ・伝票番号を部署・差分フラグ・伝票日付・仕訳ＣＤの順に採番したい
                    // ・金額がマイナスの場合は借方と貸方（のコード）を逆にする必要がある（その処理は１箇所にまとめたい）
                    // 以上の理由により、取得結果を１つのデータテーブルに格納し、そのデータテーブルのレコードを列挙して処理を行うようにする
                    // したがって、各取得処理は同じ項目(=列)を取得すること
                    //*********************************************************************************************************************

                    // 売上の集計（[売上日]＝出力対象日 の分）
                    if (SyukeiUriageToday(con, tran, ref syukeiDt) == false)
                    {
                        return false;
                    }

                    // 入金の集計（[入金日]＝出力対象日 の分）
                    if (SyukeiNyukinToday(con, tran, ref syukeiDt) == false)
                    {
                        return false;
                    }

                    // 結果を格納した集計テーブルをループし、伝票番号の採番と借方／貸方コードの調整をしながら、会計連動集計テーブルに出力する
                    if (OutputKaikeiRenkeiSyukei(con, tran, 0, ref syukeiDt) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();

                    log.InfoFormat("会計連携集計テーブルを{0}しました。", History.ProcessType.Create.DisplayName());
                    return true;
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
        /// 出力対象日に該当する会計連携集計テーブルを削除する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteKaikeiRenkeiSyukei(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE FROM 会計連携集計テーブル" +
                                  "\r\n WHERE" +
                                  "\r\n     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND 集計日 = @出力対象日";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                log.InfoFormat("会計連携集計テーブルを{0}しました。", History.ProcessType.Delete.DisplayName());
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("出力対象日に該当する会計連携集計テーブルの削除に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// CSV出力処理
        /// </summary>
        /// <param name="isSyukeibi">会計連動集計テーブル.集計日=出力対象日のレコードを取得する場合はtrue。falseの場合は会計連動集計テーブル.伝票日付=出力対象日のレコードを取得する</param>
        /// <param name="maxDenpyoNo">出力対象日の最大伝票番号</param>
        /// <param name="filePath">出力ファイルパス</param>
        private bool OutputCSV(bool isSyukeibi, int maxDenpyoNo, string filePath)
        {
            try
            {
                // [集計日]または[伝票日付]=出力対象日に該当する会計連携集計テーブルのレコードを取得
                DataTable dt = GetKaikeiRenkeiSyukei(isSyukeibi, maxDenpyoNo);

                if (dt.Rows.Count == 0)
                {
                    // 再出力の場合にデータがない場合があるので…
                    MessageBox.Show(this, "出力対象データはありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 出力内容のデータテーブルとは関連付かない内容を格納したマップを生成
                Dictionary<string, object> valueMap = new Dictionary<string, object>();
                UserData userData = UserData.getInstance();
                valueMap["担当者ＣＤ"] = userData.担当者ＣＤ;

                // CSVWriterを用いてCSVを出力
                CSVWriter.Output(Consts.CSVTemplates.KeikeiRenkei, dt, valueMap, filePath);

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
        /// CSV出力用の会計連動集計テーブルを取得
        /// </summary>
        /// <param name="isSyukeibi">会計連動集計テーブル.集計日=出力対象日のレコードを取得する場合はtrue。falseの場合は会計連動集計テーブル.伝票日付=出力対象日のレコードを取得する</param>
        /// <param name="maxDenpyoNo">出力対象日の最大伝票番号</param>
        /// <returns>CSV出力用の会計連動集計テーブル</returns>
        private DataTable GetKaikeiRenkeiSyukei(bool isSyukeibi, int maxDenpyoNo)
        {
            try
            {
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     K.企業ＣＤ");
                    sql.AppendLine("     ,K.集計日");
                    sql.AppendLine("     ,K.部署ＣＤ");
                    sql.AppendLine("     ,K.伝票番号");
                    sql.AppendLine("     ,K.伝票日付");
                    sql.AppendLine("     ,K.仕訳ＣＤ");
                    sql.AppendLine("     ,K.集計ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.借方ＣＤ     ELSE K.貸方ＣＤ     END AS 借方ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.借方部門ＣＤ ELSE K.貸方部門ＣＤ END AS 借方部門ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.借方補助ＣＤ ELSE K.貸方補助ＣＤ END AS 借方補助ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.貸方ＣＤ     ELSE K.借方ＣＤ     END AS 貸方ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.貸方部門ＣＤ ELSE K.借方部門ＣＤ END AS 貸方部門ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.貸方補助ＣＤ ELSE K.借方補助ＣＤ END AS 貸方補助ＣＤ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(K.マイナスフラグ, 0) = 0 THEN K.金額         ELSE (K.金額 * -1)  END AS 金額");
                    sql.AppendLine("     ,K.差分フラグ");
                    sql.AppendLine("     ,K.マイナスフラグ");
                    sql.AppendLine("     ,K.税率");
                    sql.AppendLine("     ,SW.処理内容");
                    sql.AppendLine("     ,SW.摘要");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     会計連携集計テーブル AS K");
                    sql.AppendLine("     INNER JOIN 仕訳マスタ AS SW");
                    sql.AppendLine("         ON K.企業ＣＤ = SW.企業ＣＤ");
                    sql.AppendLine("         AND K.部署ＣＤ = SW.部署ＣＤ");
                    sql.AppendLine("         AND K.仕訳ＣＤ = SW.仕訳ＣＤ");
                    sql.AppendLine("     INNER JOIN 部署マスタ AS B");
                    sql.AppendLine("         ON K.企業ＣＤ = B.企業ＣＤ");
                    sql.AppendLine("         AND K.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     K.企業ＣＤ = @企業ＣＤ");

                    // 出力対象は集計日か伝票日付のどちらか
                    if (isSyukeibi)
                    {
                        sql.AppendLine("     AND K.集計日 = @出力対象日");
                        sql.AppendLine("     AND K.伝票番号 > @前回最大伝票番号");  // -- 差分をCSVに出力する場合は、今回追加したレコードのみを対象にする
                    }
                    else
                    {
                        // 履歴から再出力の場合も集計日に変更
                        sql.AppendLine("     AND K.集計日 = @出力対象日");
                    }
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     K.部署ＣＤ");
                    sql.AppendLine("     ,K.差分フラグ");
                    sql.AppendLine("     ,K.伝票番号");

                    cmd.CommandText = sql.ToString();

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                    cmd.Parameters.Add("@前回最大伝票番号", SqlDbType.Int).Value = maxDenpyoNo;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("会計連携集計テーブル");
                    adp.Fill(dt);
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
        /// 会計連携の集計対象外を抽出・表示
        /// </summary>
        private void ShowSyukeiTaisyogai()
        {
            try
            {
                // 売上の集計対象外を取得
                string uriage = GetSyukeiTaisyogaiUriage();

                // 入金の集計対象外を取得
                string nyukin = GetSyukeiTaisyogaiNyukin();

                if (string.IsNullOrEmpty(uriage) == false || string.IsNullOrEmpty(nyukin) == false)
                {
                    // 内容を編集し、レポート画面を表示
                    StringBuilder content = new StringBuilder();
                    content.AppendLine("以下の売上・入金が集計対象外です。");
                    content.Append(uriage);
                    content.Append(nyukin);
                    Form980 form = new Form980("会計連携集計対象外レポート", Properties.Resources.group_ico, content.ToString());
                    SubFormOpenInit(form);
                    form.Show();
                    form.FormClosed += new FormClosedEventHandler(Form980_FormClosed);
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
        /// レポート画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form980_FormClosed(object sender, FormClosedEventArgs e)
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
        /// 会計連携(売上)の集計対象外を抽出する
        /// </summary>
        private string GetSyukeiTaisyogaiUriage()
        {
            try
            {
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     U.売上日");
                    sql.AppendLine("     ,U.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,U.得意先ＣＤ");
                    sql.AppendLine("     ,U.得意先名");
                    sql.AppendLine("     ,U.商品ＣＤ");
                    sql.AppendLine("     ,S.商品名");
                    sql.AppendLine("     ,U.商品数量");
                    sql.AppendLine("     ,VU.売上区分名");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         UH.企業ＣＤ");
                    sql.AppendLine("         ,UH.売上日");
                    sql.AppendLine("         ,UH.部署ＣＤ");
                    sql.AppendLine("         ,UH.得意先ＣＤ");
                    sql.AppendLine("         ,UH.得意先名");
                    sql.AppendLine("         ,UM.明細番号");
                    sql.AppendLine("         ,UM.商品ＣＤ");
                    sql.AppendLine("         ,UM.商品数量");
                    sql.AppendLine("         ,UM.売上区分");
                    sql.AppendLine("         ,UM.チケット束フラグ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ AS UH");
                    sql.AppendLine("         INNER JOIN 売上明細 AS UM");
                    sql.AppendLine("             ON UH.企業ＣＤ = UM.企業ＣＤ");
                    sql.AppendLine("             AND UH.売上日 = UM.売上日");
                    sql.AppendLine("             AND UH.得意先ＣＤ = UM.得意先ＣＤ");
                    sql.AppendLine("             AND UH.部署ＣＤ = UM.部署ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         UH.企業ＣＤ = @企業ＣＤ");

                    if (_kaikeiRenkei.出力対象 == KaikeiRenkei.KaikeiRenkeiTarget.Recalc)
                    {
                        sql.AppendLine("         AND UH.売上日 = @出力対象日");         // -- この日だけを再集計 の場合
                    }
                    else
                    {
                        sql.AppendLine("         AND UH.売上日 >= @出力対象月一日");    // -- 差分 または 履歴から の場合
                        sql.AppendLine("         AND UH.売上日 <= @出力対象日");        // -- 差分 または 履歴から の場合
                    }

                    sql.AppendLine("     ) AS U");
                    sql.AppendLine("     LEFT JOIN 仕訳マスタ AS SW");
                    sql.AppendLine("         ON SW.企業ＣＤ = U.企業ＣＤ");
                    sql.AppendLine("         AND SW.部署ＣＤ = U.部署ＣＤ");
                    sql.AppendLine("         AND SW.売上区分 = U.売上区分");
                    sql.AppendLine("         AND SW.チケット束フラグ = U.チケット束フラグ");
                    sql.AppendLine("         AND SW.集計対象区分 = @集計対象区分売上");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B");
                    sql.AppendLine("         ON B.企業ＣＤ = U.企業ＣＤ");
                    sql.AppendLine("         AND B.部署ＣＤ = U.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS S");
                    sql.AppendLine("         ON U.企業ＣＤ = S.企業ＣＤ");
                    sql.AppendLine("         AND U.商品ＣＤ = S.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_売上区分 AS VU");
                    sql.AppendLine("         ON U.売上区分 = VU.売上区分CD");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     SW.仕訳ＣＤ IS NULL");     // -- 仕訳マスタにヒットしないもの
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     U.売上日");
                    sql.AppendLine("     ,U.部署ＣＤ");
                    sql.AppendLine("     ,U.得意先ＣＤ");
                    sql.AppendLine("     ,U.明細番号");

                    cmd.CommandText = sql.ToString();

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@出力対象月一日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象月一日;
                    cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                    cmd.Parameters.Add("@集計対象区分売上", SqlDbType.Int).Value = (int)Consts.SyukeiTaisyoKubun.Uriage;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("売上集計対象外");
                    adp.Fill(dt);
                }

                StringBuilder lines = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    // 明細を出力
                    lines.AppendFormat("{0:yyyy/MM/dd}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                                    dr["売上日"], dr["部署名"], dr["得意先ＣＤ"], dr["得意先名"], dr["商品名"], dr["商品数量"], dr["売上区分名"]).AppendLine();
                }
                StringBuilder ret = new StringBuilder();
                if (lines.Length != 0)
                {
                    // 見出しを追加
                    ret.AppendLine();
                    ret.AppendLine("【売上】").AppendLine("売上日\t部署名\t得意先ＣＤ\t得意先名\t商品名\t商品数量\t売上区分名").Append(lines.ToString());
                }
                return ret.ToString();
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
        /// 会計連携(入金)の集計対象外を抽出する
        /// </summary>
        private string GetSyukeiTaisyogaiNyukin()
        {
            try
            {
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     NM.入金日");
                    sql.AppendLine("     ,NM.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,NM.得意先ＣＤ");
                    sql.AppendLine("     ,NM.得意先名");
                    sql.AppendLine("     ,NM.入金方法");
                    sql.AppendLine("     ,VN.入金方法名");
                    sql.AppendLine("     ,NM.入金額");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,入金番号");
                    sql.AppendLine("         ,入金日");
                    sql.AppendLine("         ,部署ＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("         ,得意先名");
                    sql.AppendLine("         ,入金方法");
                    sql.AppendLine("         ,入金額");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         入金明細");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");

                    if (_kaikeiRenkei.出力対象 == KaikeiRenkei.KaikeiRenkeiTarget.Recalc)
                    {
                        sql.AppendLine("         AND 入金日 = @出力対象日");        // -- この日だけを再集計 の場合
                    }
                    else
                    {
                        sql.AppendLine("         AND 入金日 >= @出力対象月一日");   // -- 差分 または 履歴から の場合
                        sql.AppendLine("         AND 入金日 <= @出力対象日");       // -- 差分 または 履歴から の場合
                    }

                    sql.AppendLine("     ) AS NM");
                    sql.AppendLine("     LEFT JOIN 仕訳マスタ AS SW");
                    sql.AppendLine("         ON SW.企業ＣＤ = NM.企業ＣＤ");
                    sql.AppendLine("         AND SW.部署ＣＤ = NM.部署ＣＤ");
                    sql.AppendLine("         AND SW.入金方法 = NM.入金方法");
                    sql.AppendLine("         AND SW.集計対象区分 = @集計対象区分入金");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B");
                    sql.AppendLine("         ON B.企業ＣＤ = NM.企業ＣＤ");
                    sql.AppendLine("         AND B.部署ＣＤ = NM.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_入金方法 AS VN ON VN.入金方法CD = NM.入金方法");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     SW.仕訳ＣＤ IS NULL");     // -- 仕訳マスタにヒットしないもの
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     NM.入金日");
                    sql.AppendLine("     ,NM.部署ＣＤ");
                    sql.AppendLine("     ,NM.得意先ＣＤ");
                    sql.AppendLine("     ,NM.入金番号");

                    cmd.CommandText = sql.ToString();

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@出力対象月一日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象月一日;
                    cmd.Parameters.Add("@出力対象日", SqlDbType.Date).Value = _kaikeiRenkei.出力対象日;
                    cmd.Parameters.Add("@集計対象区分入金", SqlDbType.Int).Value = (int)Consts.SyukeiTaisyoKubun.Nyukin;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("入金集計対象外");
                    adp.Fill(dt);
                }

                StringBuilder lines = new StringBuilder();
                foreach (DataRow dr in dt.Rows)
                {
                    // 明細を出力
                    lines.AppendFormat("{0:yyyy/MM/dd}\t{1}\t{2}\t{3}\t{4}\t{5:N0}",
                                    dr["入金日"], dr["部署名"], dr["得意先ＣＤ"], dr["得意先名"], dr["入金方法名"], dr["入金額"]).AppendLine();
                }
                StringBuilder ret = new StringBuilder();
                if (lines.Length != 0)
                {
                    // 見出しを追加
                    ret.AppendLine();
                    ret.AppendLine("【入金】").AppendLine("入金日\t部署名\t得意先ＣＤ\t得意先名\t入金方法名\t入金額").Append(lines.ToString());
                }
                return ret.ToString();
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

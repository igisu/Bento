using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 月次集計
    /// </summary>
    public partial class Form810 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 月次集計条件
        /// </summary>
        private GetsujiSyukeiCriteria _getsujiSyukeiCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form810()
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
        private void Form810_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                // 権限によりボタンを制御
                btnExec.Enabled = this.HasAuthEdit;

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _getsujiSyukeiCriteria = new GetsujiSyukeiCriteria();
                getsujiSyukeiCriteriaBindingSource.DataSource = _getsujiSyukeiCriteria;
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
        private void Form810_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F9:                       // 集計実行ボタン押下
                        btnExec.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 集計実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "実行できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_getsujiSyukeiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_getsujiSyukeiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _getsujiSyukeiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 確認メッセージを表示
                if (MessageBox.Show(this, "月次集計処理を行います。よろしいですか？",
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (Exec())
                    {
                        MessageBox.Show(this, "月次集計処理を実行しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 月次集計処理
        /// </summary>
        /// <returns>成功した場合は True</returns>
        private bool Exec()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                int timeoutSec = SystemMaster.GetTimeoutSecForGetsujiSyukei();  // 月次集計タイムアウト秒数を取得

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    History.Save(con, tran, this.Text, "月次集計", "", History.ProcessType.Start);
                    log.Info("月次集計処理を開始しました。");

                    //*****************
                    // 月次売掛残高集計
                    //*****************

                    // 月次売掛残高集計を削除
                    if (DeleteUrikakeZandaka(con, tran) == false)
                    {
                        return false;
                    }

                    // #月次売掛残高集計ヘッダTempを作成
                    if (CreateHeaderTemp(con, tran, timeoutSec) == false)
                    {
                        return false;
                    }

                    // #月次売掛残高集計Tempを作成
                    if (CreateUrikakeZandakaTemp(con, tran, timeoutSec) == false)
                    {
                        return false;
                    }

                    // #月次売掛残高集計ヘッダTempが持つ値を合計して#月次売掛残高集計Tempを更新
                    if (UpdateUrikakeZandakaTemp(con, tran) == false)
                    {
                        return false;
                    }

                    // 月次売掛残高集計に出力
                    if (CreateUrikakeZandaka(con, tran) == false)
                    {
                        return false;
                    }


                    //*************
                    // 月次売上集計
                    //*************

                    // 月次売上集計を削除
                    if (DeleteUriageSyukei(con, tran) == false)
                    {
                        return false;
                    }

                    // 月次売上集計に出力
                    if (CreateUriageSyukei(con, tran) == false)
                    {
                        return false;
                    }

                    History.Save(con, tran, this.Text, "月次集計", "", History.ProcessType.Output);
                    tran.Commit();
                    log.Info("月次集計処理を実行しました。");

                    return true;
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 月次売掛残高集計を削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUrikakeZandaka(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE FROM 月次売掛残高集計" +
                                  "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND 集計年 = @集計年" +
                                  "\r\n AND 集計月 = @集計月" +
                                  "\r\n AND 部署ＣＤ = @部署ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象年;
                cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象月;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiSyukeiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売掛残高集計の{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #月次売掛残高集計ヘッダTempを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateHeaderTemp(SqlConnection con, SqlTransaction tran, int timeoutSec)
        {
            try
            {
                //*********************************************************************************************************************
                // 月次売掛残高集計ヘッダTempを作成
                // 月次売掛残高集計は請求先ごとに集計され、かつ、請求合計と同じ値になる必要がある。
                // 請求合計は、得意先ごとに集計した請求ヘッダとセットで作成され、請求ヘッダの内容を請求先ごとに集計した値が格納される。
                // つまり、月次売掛残高集計を集計するには、その前段階として得意先ごとに集計する必要がある。
                // したがって、月次売掛残高集計を請求合計と同様に集計するからには、請求ヘッダと同等のものが必要になる。
                //*********************************************************************************************************************

                // #月次売掛残高集計ヘッダTempを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n CREATE TABLE #月次売掛残高集計ヘッダTemp (" +
                                  "\r\n     部署ＣＤ    int NOT NULL," +
                                  "\r\n     得意先ＣＤ int NOT NULL," +
                                  "\r\n     得意先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     請求先ＣＤ  int NOT NULL," +
                                  "\r\n     請求先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     今月税区分 int NULL," +
                                  "\r\n     今月丸め処理区分 int NULL," +
                                  "\r\n     今月計算税率 decimal(3, 2) NULL," +
                                  "\r\n     今月売上金額 decimal(18, 0) NULL," +
                                  "\r\n     先月税区分 int NULL," +
                                  "\r\n     先月丸め処理区分 int NULL," +
                                  "\r\n     先月計算税率 decimal(3, 2) NULL," +
                                  "\r\n     先月売上金額 decimal(18, 0) NULL" +
                                  "\r\n )";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // #月次売掛残高集計ヘッダTempに出力
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO #月次売掛残高集計ヘッダTemp (" +
                                  "\r\n     部署ＣＤ" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,得意先名" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,今月税区分" +
                                  "\r\n     ,今月丸め処理区分" +
                                  "\r\n     ,今月計算税率" +
                                  "\r\n     ,今月売上金額" +
                                  "\r\n     ,先月税区分" +
                                  "\r\n     ,先月丸め処理区分" +
                                  "\r\n     ,先月計算税率" +
                                  "\r\n     ,先月売上金額" +
                                  "\r\n ) SELECT" +
                                  "\r\n       ISNULL(B.部署ＣＤ, T.部署ＣＤ) AS 部署ＣＤ" +
                                  "\r\n       ,B.得意先ＣＤ" +
                                  "\r\n       ,T.得意先名" +
                                  "\r\n       ,S.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n       ,S.得意先名 AS 請求先名" +
                                  "\r\n       ,B.今月税区分" +
                                  "\r\n       ,B.今月丸め処理区分" +
                                  "\r\n       ,B.今月計算税率" +
                                  "\r\n       ,B.今月売上金額" +
                                  "\r\n       ,B.先月税区分" +
                                  "\r\n       ,B.先月丸め処理区分" +
                                  "\r\n       ,B.先月計算税率" +
                                  "\r\n       ,B.先月売上金額" +
                                  "\r\n   FROM" +
                                  // -- 得意先マスタのみを駆動表にしようとしたが、同じ得意先でも、部署が異なる売上があり得るので、
                                  // -- 指定された部署の売上を、その得意先ごとに集計しないといけないので、請求ヘッダ同様の駆動表にならざるを得ない
                                  "\r\n       (SELECT" +
                                  "\r\n           D.得意先ＣＤ" +
                                  "\r\n           ,UA.部署ＣＤ" +
                                  "\r\n           ,UA.税区分 AS 今月税区分" +
                                  "\r\n           ,UA.丸め処理区分 AS 今月丸め処理区分" +
                                  "\r\n           ,UA.計算税率 AS 今月計算税率" +
                                  "\r\n           ,ISNULL(UA.売上金額, 0) AS 今月売上金額" +
                                  "\r\n           ,UB.税区分 AS 先月税区分" +
                                  "\r\n           ,UB.丸め処理区分 AS 先月丸め処理区分" +
                                  "\r\n           ,UB.計算税率 AS 先月計算税率" +
                                  "\r\n           ,ISNULL(UB.売上金額, 0) AS 先月売上金額" +
                                  "\r\n       FROM" +
                                  // -- 駆動表①－１ 今月（＝指定年月）の売上が存在する得意先と計算税率（部署合算請求フラグOFF）
                                  "\r\n           (SELECT" +
                                  "\r\n               U11.得意先ＣＤ" +
                                  "\r\n               ,UM11.計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U11" +
                                  "\r\n               INNER JOIN 売上明細 AS UM11" +
                                  "\r\n                   ON UM11.企業ＣＤ = U11.企業ＣＤ" +
                                  "\r\n                   AND UM11.売上日 = U11.売上日" +
                                  "\r\n                   AND UM11.得意先ＣＤ = U11.得意先ＣＤ" +
                                  "\r\n                   AND UM11.部署ＣＤ = U11.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T11 ON U11.企業ＣＤ = T11.企業ＣＤ AND U11.得意先ＣＤ = T11.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               U11.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                                  "\r\n               AND (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U11.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND U11.売上日 >= @対象年月一日" +
                                  "\r\n               AND U11.売上日 <= @対象年月末日" +
                                  "\r\n               AND UM11.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表①－２ 今月（＝指定年月）の売上が存在する得意先と計算税率（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               U12.得意先ＣＤ" +
                                  "\r\n               ,UM12.計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U12" +
                                  "\r\n               INNER JOIN 売上明細 AS UM12" +
                                  "\r\n                   ON UM12.企業ＣＤ = U12.企業ＣＤ" +
                                  "\r\n                   AND UM12.売上日 = U12.売上日" +
                                  "\r\n                   AND UM12.得意先ＣＤ = U12.得意先ＣＤ" +
                                  "\r\n                   AND UM12.部署ＣＤ = U12.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T12 ON U12.企業ＣＤ = T12.企業ＣＤ AND U12.得意先ＣＤ = T12.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               U12.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND U12.売上日 >= @対象年月一日" +
                                  "\r\n               AND U12.売上日 <= @対象年月末日" +
                                  "\r\n               AND UM12.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表②－１ 先月（＝前回締め以降）の売上が存在する得意先と計算税率（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               U21.得意先ＣＤ" +
                                  "\r\n               ,UM21.計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U21" +
                                  "\r\n               INNER JOIN 売上明細 AS UM21" +
                                  "\r\n                   ON UM21.企業ＣＤ = U21.企業ＣＤ" +
                                  "\r\n                   AND UM21.売上日 = U21.売上日" +
                                  "\r\n                   AND UM21.得意先ＣＤ = U21.得意先ＣＤ" +
                                  "\r\n                   AND UM21.部署ＣＤ = U21.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T21 ON U21.企業ＣＤ = T21.企業ＣＤ AND U21.得意先ＣＤ = T21.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = T21.企業ＣＤ AND S21.得意先ＣＤ = ISNULL(T21.請求先ＣＤ, T21.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                       ,MAX(H2.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H2" +
                                  "\r\n                       INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = H2.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(H2.請求先ＣＤ, H2.得意先ＣＤ)" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H2.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                   ) AS M21 ON" +
                                  "\r\n                       M21.得意先ＣＤ = U21.得意先ＣＤ" +
                                  "\r\n                       AND M21.部署ＣＤ = U21.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               U21.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                                  "\r\n               AND (ISNULL(S21.部署合算請求フラグ, 0) = 0 AND U21.部署ＣＤ = @部署ＣＤ)" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n               AND U21.売上日 > ISNULL(M21.前回締日, '2000/01/01')" +
                                  "\r\n               AND U21.売上日 < @対象年月一日" +
                                  "\r\n               AND UM21.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表②－２ 先月（＝前回締め以降）の売上が存在する得意先と計算税率（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               U22.得意先ＣＤ" +
                                  "\r\n               ,UM22.計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U22" +
                                  "\r\n               INNER JOIN 売上明細 AS UM22" +
                                  "\r\n                   ON UM22.企業ＣＤ = U22.企業ＣＤ" +
                                  "\r\n                   AND UM22.売上日 = U22.売上日" +
                                  "\r\n                   AND UM22.得意先ＣＤ = U22.得意先ＣＤ" +
                                  "\r\n                   AND UM22.部署ＣＤ = U22.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T22 ON U22.企業ＣＤ = T22.企業ＣＤ AND U22.得意先ＣＤ = T22.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = T22.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(T22.請求先ＣＤ, T22.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                       ,MAX(H2.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H2" +
                                  "\r\n                       INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = H2.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(H2.請求先ＣＤ, H2.得意先ＣＤ)" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H2.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                   ) AS M22 ON" +
                                  "\r\n                       M22.得意先ＣＤ = U22.得意先ＣＤ" +
                                  "\r\n                       AND M22.部署ＣＤ = U22.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               U22.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S22.部署合算請求フラグ, 0) <> 0 AND S22.部署ＣＤ = @部署ＣＤ)" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n               AND U22.売上日 > ISNULL(M22.前回締日, '2000/01/01')" +
                                  "\r\n               AND U22.売上日 < @対象年月一日" +
                                  "\r\n               AND UM22.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表③－１ 今月（＝指定年月）の入金が存在する請求先を持つ得意先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               T31.得意先ＣＤ" +
                                  "\r\n               ,NULL AS 計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N31" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S31 ON S31.企業ＣＤ = N31.企業ＣＤ AND S31.得意先ＣＤ = N31.得意先ＣＤ" +    // -- N31.得意先ＣＤは請求先ＣＤ
                                  "\r\n               INNER JOIN 得意先マスタ AS T31 ON T31.企業ＣＤ = S31.企業ＣＤ AND S31.得意先ＣＤ = ISNULL(T31.請求先ＣＤ, T31.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               N31.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                                  "\r\n               AND (ISNULL(S31.部署合算請求フラグ, 0) = 0 AND N31.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N31.入金日 >= @対象年月一日" +
                                  "\r\n               AND N31.入金日 <= @対象年月末日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表③－２ 今月（＝指定年月）の入金が存在する請求先を持つ得意先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               T32.得意先ＣＤ" +
                                  "\r\n               ,NULL AS 計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N32" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S32 ON S32.企業ＣＤ = N32.企業ＣＤ AND S32.得意先ＣＤ = N32.得意先ＣＤ" +    // -- N32.得意先ＣＤは請求先ＣＤ
                                  "\r\n               INNER JOIN 得意先マスタ AS T32 ON T32.企業ＣＤ = S32.企業ＣＤ AND S32.得意先ＣＤ = ISNULL(T32.請求先ＣＤ, T32.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               N32.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S32.部署合算請求フラグ, 0) <> 0 AND S32.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N32.入金日 >= @対象年月一日" +
                                  "\r\n               AND N32.入金日 <= @対象年月末日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表④－１ 先月（＝前回締め以降）の入金が存在する請求先を持つ得意先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               T41.得意先ＣＤ" +
                                  "\r\n               ,NULL AS 計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N41" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S41 ON S41.企業ＣＤ = N41.企業ＣＤ AND S41.得意先ＣＤ = N41.得意先ＣＤ" +    // -- N41.得意先ＣＤは請求先ＣＤ
                                  "\r\n               INNER JOIN 得意先マスタ AS T41 ON T41.企業ＣＤ = S41.企業ＣＤ AND S41.得意先ＣＤ = ISNULL(T41.請求先ＣＤ, T41.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G4.請求先ＣＤ" +
                                  "\r\n                       ,G4.部署ＣＤ" +
                                  "\r\n                       ,MAX(G4.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H4" +
                                  "\r\n                       INNER JOIN 請求合計 AS G4 ON H4.企業ＣＤ = G4.企業ＣＤ AND H4.請求番号 = G4.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = G4.企業ＣＤ AND S42.得意先ＣＤ = G4.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H4.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H4.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G4.請求先ＣＤ" +
                                  "\r\n                       ,G4.部署ＣＤ" +
                                  "\r\n                   ) AS M41 ON" +
                                  "\r\n                       M41.請求先ＣＤ = N41.得意先ＣＤ" +
                                  "\r\n                       AND M41.部署ＣＤ = N41.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               N41.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                                  "\r\n               AND (ISNULL(S41.部署合算請求フラグ, 0) = 0 AND N41.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N41.入金日 > ISNULL(M41.前回締日, '2000/01/01')" +
                                  "\r\n               AND N41.入金日 < @対象年月一日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表④－２ 先月（＝前回締め以降）の入金が存在する請求先を持つ得意先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               T42.得意先ＣＤ" +
                                  "\r\n               ,NULL AS 計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N42" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = N42.企業ＣＤ AND S42.得意先ＣＤ = N42.得意先ＣＤ" +    // -- N42.得意先ＣＤは請求先ＣＤ
                                  "\r\n               INNER JOIN 得意先マスタ AS T42 ON T42.企業ＣＤ = S42.企業ＣＤ AND S42.得意先ＣＤ = ISNULL(T42.請求先ＣＤ, T42.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H4.請求先ＣＤ" +
                                  "\r\n                       ,H4.部署ＣＤ" +
                                  "\r\n                       ,MAX(H4.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H4" +
                                  "\r\n                       INNER JOIN 請求合計 AS G4 ON H4.企業ＣＤ = G4.企業ＣＤ AND H4.請求番号 = G4.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = G4.企業ＣＤ AND S42.得意先ＣＤ = G4.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H4.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H4.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H4.請求先ＣＤ" +
                                  "\r\n                       ,H4.部署ＣＤ" +
                                  "\r\n                   ) AS M42 ON" +
                                  "\r\n                       M42.請求先ＣＤ = N42.得意先ＣＤ" +
                                  "\r\n                       AND M42.部署ＣＤ = N42.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               N42.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S42.部署合算請求フラグ, 0) <> 0 AND S42.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N42.入金日 > ISNULL(M42.前回締日, '2000/01/01')" +
                                  "\r\n               AND N42.入金日 < @対象年月一日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表⑤－１ 前回請求額が存在する請求先を持つ得意先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               T51.得意先ＣＤ" +
                                  "\r\n               ,NULL AS 計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               請求合計 AS A51" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S51 ON S51.企業ＣＤ = A51.企業ＣＤ AND S51.得意先ＣＤ = A51.請求先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T51 ON T51.企業ＣＤ = S51.企業ＣＤ AND S51.得意先ＣＤ = ISNULL(T51.請求先ＣＤ, T51.得意先ＣＤ)" +
                                  "\r\n               INNER JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                       ,MAX(G5.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H5" +
                                  "\r\n                       INNER JOIN 請求合計 AS G5 ON H5.企業ＣＤ = G5.企業ＣＤ AND H5.請求番号 = G5.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = G5.企業ＣＤ AND S52.得意先ＣＤ = G5.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H5.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H5.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                   ) AS M51 ON" +
                                  "\r\n                       M51.請求先ＣＤ = A51.請求先ＣＤ" +
                                  "\r\n                       AND M51.部署ＣＤ = A51.部署ＣＤ" +
                                  "\r\n                       AND M51.前回締日 = A51.請求日" +
                                  "\r\n           WHERE" +
                                  "\r\n               A51.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、請求合計の部署を対象にする
                                  "\r\n               AND (ISNULL(S51.部署合算請求フラグ, 0) = 0 AND A51.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n           UNION" +
                                  // -- 駆動表⑤－２ 前回請求額が存在する請求先を持つ得意先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               T52.得意先ＣＤ" +
                                  "\r\n               ,NULL AS 計算税率" +
                                  "\r\n           FROM" +
                                  "\r\n               請求合計 AS A52" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = A52.企業ＣＤ AND S52.得意先ＣＤ = A52.請求先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T52 ON T52.企業ＣＤ = S52.企業ＣＤ AND S52.得意先ＣＤ = ISNULL(T52.請求先ＣＤ, T52.得意先ＣＤ)" +
                                  "\r\n               INNER JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                       ,MAX(G5.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H5" +
                                  "\r\n                       INNER JOIN 請求合計 AS G5 ON H5.企業ＣＤ = G5.企業ＣＤ AND H5.請求番号 = G5.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = G5.企業ＣＤ AND S52.得意先ＣＤ = G5.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H5.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H5.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                   ) AS M52 ON" +
                                  "\r\n                       M52.請求先ＣＤ = A52.請求先ＣＤ" +
                                  "\r\n                       AND M52.部署ＣＤ = A52.部署ＣＤ" +
                                  "\r\n                       AND M52.前回締日 = A52.請求日" +
                                  "\r\n           WHERE" +
                                  "\r\n               A52.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S52.部署合算請求フラグ, 0) <> 0 AND S52.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n           ) AS D" +
                                  "\r\n           LEFT JOIN" +
                                  // -- 今月（＝指定年月）の売上（部署合算請求フラグOFF）
                                  "\r\n           (SELECT" +
                                  "\r\n               U11.得意先ＣＤ" +
                                  "\r\n               ,U11.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U11.税区分, @税区分内税) AS 税区分" +                         // -- 初期値は1:内税
                                  "\r\n               ,ISNULL(U11.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分" +   // -- 初期値は2:切り捨て
                                  "\r\n               ,UM11.計算税率" +
                                  "\r\n               ,SUM(UM11.値引後金額) AS 売上金額" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U11" +
                                  "\r\n               INNER JOIN 売上明細 AS UM11" +
                                  "\r\n                   ON UM11.企業ＣＤ = U11.企業ＣＤ" +
                                  "\r\n                   AND UM11.売上日 = U11.売上日" +
                                  "\r\n                   AND UM11.得意先ＣＤ = U11.得意先ＣＤ" +
                                  "\r\n                   AND UM11.部署ＣＤ = U11.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T11 ON U11.企業ＣＤ = T11.企業ＣＤ AND U11.得意先ＣＤ = T11.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               U11.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                                  "\r\n               AND (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U11.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND U11.売上日 >= @対象年月一日" +
                                  "\r\n               AND U11.売上日 <= @対象年月末日" +
                                  "\r\n               AND UM11.売上区分 = @売上区分売掛" +   // -- 売掛
                                  "\r\n           GROUP BY" +
                                  "\r\n               U11.得意先ＣＤ" +
                                  "\r\n               ,U11.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U11.税区分, @税区分内税)" +
                                  "\r\n               ,ISNULL(U11.丸め処理区分, @丸め処理区分切り捨て)" +
                                  "\r\n               ,UM11.計算税率" +
                                  "\r\n           UNION" +
                                  // -- 今月（＝指定年月）の売上（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               U12.得意先ＣＤ" +
                                  "\r\n               ,U12.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U12.税区分, @税区分内税) AS 税区分" +                         // -- 初期値は1:内税
                                  "\r\n               ,ISNULL(U12.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分" +   // -- 初期値は2:切り捨て
                                  "\r\n               ,UM12.計算税率" +
                                  "\r\n               ,SUM(UM12.値引後金額) AS 売上金額" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U12" +
                                  "\r\n               INNER JOIN 売上明細 AS UM12" +
                                  "\r\n                   ON UM12.企業ＣＤ = U12.企業ＣＤ" +
                                  "\r\n                   AND UM12.売上日 = U12.売上日" +
                                  "\r\n                   AND UM12.得意先ＣＤ = U12.得意先ＣＤ" +
                                  "\r\n                   AND UM12.部署ＣＤ = U12.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T12 ON U12.企業ＣＤ = T12.企業ＣＤ AND U12.得意先ＣＤ = T12.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               U12.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND U12.売上日 >= @対象年月一日" +
                                  "\r\n               AND U12.売上日 <= @対象年月末日" +
                                  "\r\n               AND UM12.売上区分 = @売上区分売掛" +   // -- 売掛
                                  "\r\n           GROUP BY" +
                                  "\r\n               U12.得意先ＣＤ" +
                                  "\r\n               ,U12.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U12.税区分, @税区分内税)" +
                                  "\r\n               ,ISNULL(U12.丸め処理区分, @丸め処理区分切り捨て)" +
                                  "\r\n               ,UM12.計算税率" +
                                  "\r\n           ) AS UA ON" +
                                  "\r\n               D.得意先ＣＤ = UA.得意先ＣＤ" +
                                  "\r\n               AND D.計算税率 = UA.計算税率" +   // -- 得意先ＣＤだけで結合するとUAとUBで異なる計算税率のレコードを結合して取得してしまう。計算税率を明示的に指定して結合することでそれを回避する
                                  "\r\n           LEFT JOIN" +
                                  // -- 先月（＝前回締め以降）の売上（部署合算請求フラグOFF）
                                  "\r\n           (SELECT" +
                                  "\r\n               U21.得意先ＣＤ" +
                                  "\r\n               ,U21.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U21.税区分, @税区分内税) AS 税区分" +                         // -- 初期値は1:内税
                                  "\r\n               ,ISNULL(U21.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分" +   // -- 初期値は2:切り捨て
                                  "\r\n               ,UM21.計算税率" +
                                  "\r\n               ,SUM(UM21.値引後金額) AS 売上金額" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U21" +
                                  "\r\n               INNER JOIN 売上明細 AS UM21" +
                                  "\r\n                   ON UM21.企業ＣＤ = U21.企業ＣＤ" +
                                  "\r\n                   AND UM21.売上日 = U21.売上日" +
                                  "\r\n                   AND UM21.得意先ＣＤ = U21.得意先ＣＤ" +
                                  "\r\n                   AND UM21.部署ＣＤ = U21.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T21 ON U21.企業ＣＤ = T21.企業ＣＤ AND U21.得意先ＣＤ = T21.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = T21.企業ＣＤ AND S21.得意先ＣＤ = ISNULL(T21.請求先ＣＤ, T21.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                       ,MAX(H2.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H2" +
                                  "\r\n                       INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = H2.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(H2.請求先ＣＤ, H2.得意先ＣＤ)" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H2.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                   ) AS M21 ON" +
                                  "\r\n                       M21.得意先ＣＤ = U21.得意先ＣＤ" +
                                  "\r\n                       AND M21.部署ＣＤ = U21.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               U21.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                                  "\r\n               AND (ISNULL(S21.部署合算請求フラグ, 0) = 0 AND U21.部署ＣＤ = @部署ＣＤ)" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n               AND U21.売上日 > ISNULL(M21.前回締日, '2000/01/01')" +
                                  "\r\n               AND U21.売上日 < @対象年月一日" +
                                  "\r\n               AND UM21.売上区分 = @売上区分売掛" +   // -- 売掛
                                  "\r\n           GROUP BY" +
                                  "\r\n               U21.得意先ＣＤ" +
                                  "\r\n               ,U21.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U21.税区分, @税区分内税)" +
                                  "\r\n               ,ISNULL(U21.丸め処理区分, @丸め処理区分切り捨て)" +
                                  "\r\n               ,UM21.計算税率" +
                                  "\r\n           UNION" +
                                  // -- 先月（＝前回締め以降）の売上（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               U22.得意先ＣＤ" +
                                  "\r\n               ,U22.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U22.税区分, @税区分内税) AS 税区分" +                         // -- 初期値は1:内税
                                  "\r\n               ,ISNULL(U22.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分" +   // -- 初期値は2:切り捨て
                                  "\r\n               ,UM22.計算税率" +
                                  "\r\n               ,SUM(UM22.値引後金額) AS 売上金額" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U22" +
                                  "\r\n               INNER JOIN 売上明細 AS UM22" +
                                  "\r\n                   ON UM22.企業ＣＤ = U22.企業ＣＤ" +
                                  "\r\n                   AND UM22.売上日 = U22.売上日" +
                                  "\r\n                   AND UM22.得意先ＣＤ = U22.得意先ＣＤ" +
                                  "\r\n                   AND UM22.部署ＣＤ = U22.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T22 ON U22.企業ＣＤ = T22.企業ＣＤ AND U22.得意先ＣＤ = T22.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = T22.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(T22.請求先ＣＤ, T22.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                       ,MAX(H2.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H2" +
                                  "\r\n                       INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = H2.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(H2.請求先ＣＤ, H2.得意先ＣＤ)" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H2.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                   ) AS M22 ON" +
                                  "\r\n                       M22.得意先ＣＤ = U22.得意先ＣＤ" +
                                  "\r\n                       AND M22.部署ＣＤ = U22.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               U22.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S22.部署合算請求フラグ, 0) <> 0 AND S22.部署ＣＤ = @部署ＣＤ)" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n               AND U22.売上日 > ISNULL(M22.前回締日, '2000/01/01')" +
                                  "\r\n               AND U22.売上日 < @対象年月一日" +
                                  "\r\n               AND UM22.売上区分 = @売上区分売掛" +   // -- 売掛
                                  "\r\n           GROUP BY" +
                                  "\r\n               U22.得意先ＣＤ" +
                                  "\r\n               ,U22.部署ＣＤ" +
                                  "\r\n               ,ISNULL(U22.税区分, @税区分内税)" +
                                  "\r\n               ,ISNULL(U22.丸め処理区分, @丸め処理区分切り捨て)" +
                                  "\r\n               ,UM22.計算税率" +
                                  "\r\n           ) AS UB ON" +
                                  "\r\n               D.得意先ＣＤ = UB.得意先ＣＤ" +
                                  "\r\n               AND D.計算税率 = UB.計算税率" +   // -- 得意先ＣＤだけで結合するとUAとUBで異なる計算税率のレコードを結合して取得してしまう。計算税率を明示的に指定して結合することでそれを回避する
                                  "\r\n       ) AS B" +
                                  "\r\n       INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = B.得意先ＣＤ" +
                                  "\r\n       INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.得意先ＣＤ = ISNULL(T.請求先ＣＤ, T.得意先ＣＤ)";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiSyukeiCriteria.部署ＣＤ;
                cmd.Parameters.Add("@対象年月一日", SqlDbType.Date).Value = _getsujiSyukeiCriteria.対象年月一日;
                cmd.Parameters.Add("@対象年月末日", SqlDbType.Date).Value = _getsujiSyukeiCriteria.対象年月末日;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;

                log.Debug(cmd.CommandTextWithParameters());

                // cmd.CommandTimeout = 180;           // サンワでタイムアウトが発生しているので延ばす
                cmd.CommandTimeout = timeoutSec;

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売掛残高集計ヘッダ一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #月次売掛残高集計Tempを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUrikakeZandakaTemp(SqlConnection con, SqlTransaction tran, int timeoutSec)
        {
            try
            {
                // #月次売掛残高集計Tempを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n CREATE TABLE #月次売掛残高集計Temp (" +
                                  "\r\n     部署ＣＤ    int NOT NULL," +
                                  "\r\n     請求先ＣＤ  int NOT NULL," +
                                  "\r\n     請求先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     先月請求額 decimal(18, 0) NULL," +
                                  "\r\n     先月入金額 decimal(18, 0) NULL," +
                                  "\r\n     先月売掛残高 decimal(18, 0) NULL," +
                                  "\r\n     今月入金額 decimal(18, 0) NULL," +
                                  "\r\n     今月売上額 decimal(18, 0) NULL," +
                                  "\r\n     今月売掛残高 decimal(18, 0) NULL" +
                                  "\r\n )";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // #月次売掛残高集計Tempに出力
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO #月次売掛残高集計Temp (" +
                                  "\r\n     部署ＣＤ" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,先月請求額" +
                                  "\r\n     ,先月入金額" +
                                  "\r\n     ,先月売掛残高" +
                                  "\r\n     ,今月入金額" +
                                  "\r\n     ,今月売上額" +
                                  "\r\n     ,今月売掛残高" +
                                  "\r\n ) SELECT" +
                                  "\r\n       @部署ＣＤ AS 部署ＣＤ" +
                                  "\r\n       ,B.請求先ＣＤ" +
                                  "\r\n       ,S.得意先名 AS 請求先名" +
                                  "\r\n       ,B.先月請求額" +      // -- 先月の請求合計から取得
                                  "\r\n       ,B.先月入金額" +      // -- 先月の入金から取得
                                  "\r\n       ,B.先月売掛残高" +    // -- 先月の月次売掛残高集計から取得
                                  "\r\n       ,B.今月入金額" +
                                  "\r\n       ,0 AS 今月売上額" +   // -- ヘッダTempから求めた値を格納する
                                  "\r\n       ,0 AS 今月売掛残高" + // -- 今月売上額を求めてからでないと求まらない
                                  "\r\n   FROM" +
                                  // -- ヘッダTempは得意先単位で取得しているので
                                  // -- こちらは請求先単位で取得する
                                  "\r\n       (SELECT" +
                                  "\r\n           D.請求先ＣＤ" +
                                  "\r\n           ,NB.入金額 AS 先月入金額" +
                                  "\r\n           ,ZB.今月売掛残高 AS 先月売掛残高" +
                                  "\r\n           ,NA.入金額 AS 今月入金額" +
                                  "\r\n           ,Z.今回請求額 AS 先月請求額" +
                                  "\r\n       FROM" +
                                  // -- 駆動表①－１ 今月（＝指定年月）の売上が存在する得意先の請求先（部署合算請求フラグOFF）
                                  "\r\n           (SELECT" +
                                  "\r\n               S11.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U11" +
                                  "\r\n               INNER JOIN 売上明細 AS UM11" +
                                  "\r\n                   ON UM11.企業ＣＤ = U11.企業ＣＤ" +
                                  "\r\n                   AND UM11.売上日 = U11.売上日" +
                                  "\r\n                   AND UM11.得意先ＣＤ = U11.得意先ＣＤ" +
                                  "\r\n                   AND UM11.部署ＣＤ = U11.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T11 ON U11.企業ＣＤ = T11.企業ＣＤ AND U11.得意先ＣＤ = T11.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               U11.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                                  "\r\n               AND (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U11.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND U11.売上日 >= @対象年月一日" +
                                  "\r\n               AND U11.売上日 <= @対象年月末日" +
                                  "\r\n               AND UM11.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表①－２ 今月（＝指定年月）の売上が存在する得意先の請求先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               S12.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U12" +
                                  "\r\n               INNER JOIN 売上明細 AS UM12" +
                                  "\r\n                   ON UM12.企業ＣＤ = U12.企業ＣＤ" +
                                  "\r\n                   AND UM12.売上日 = U12.売上日" +
                                  "\r\n                   AND UM12.得意先ＣＤ = U12.得意先ＣＤ" +
                                  "\r\n                   AND UM12.部署ＣＤ = U12.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T12 ON U12.企業ＣＤ = T12.企業ＣＤ AND U12.得意先ＣＤ = T12.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)" +
                                  "\r\n           WHERE" +
                                  "\r\n               U12.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND U12.売上日 >= @対象年月一日" +
                                  "\r\n               AND U12.売上日 <= @対象年月末日" +
                                  "\r\n               AND UM12.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表②－１ 先月（＝前回締め以降）の売上が存在する得意先の請求先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               S21.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U21" +
                                  "\r\n               INNER JOIN 売上明細 AS UM21" +
                                  "\r\n                   ON UM21.企業ＣＤ = U21.企業ＣＤ" +
                                  "\r\n                   AND UM21.売上日 = U21.売上日" +
                                  "\r\n                   AND UM21.得意先ＣＤ = U21.得意先ＣＤ" +
                                  "\r\n                   AND UM21.部署ＣＤ = U21.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T21 ON U21.企業ＣＤ = T21.企業ＣＤ AND U21.得意先ＣＤ = T21.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = T21.企業ＣＤ AND S21.得意先ＣＤ = ISNULL(T21.請求先ＣＤ, T21.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                       ,MAX(H2.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H2" +
                                  "\r\n                       INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = H2.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(H2.請求先ＣＤ, H2.得意先ＣＤ)" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H2.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                   ) AS M21 ON" +
                                  "\r\n                       M21.得意先ＣＤ = U21.得意先ＣＤ" +
                                  "\r\n                       AND M21.部署ＣＤ = U21.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               U21.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                                  "\r\n               AND (ISNULL(S21.部署合算請求フラグ, 0) = 0 AND U21.部署ＣＤ = @部署ＣＤ)" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n               AND U21.売上日 > ISNULL(M21.前回締日, '2000/01/01')" +
                                  "\r\n               AND U21.売上日 < @対象年月一日" +
                                  "\r\n               AND UM21.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表②－２ 先月（＝前回締め以降）の売上が存在する得意先の請求先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               S22.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               売上ヘッダ AS U22" +
                                  "\r\n               INNER JOIN 売上明細 AS UM22" +
                                  "\r\n                   ON UM22.企業ＣＤ = U22.企業ＣＤ" +
                                  "\r\n                   AND UM22.売上日 = U22.売上日" +
                                  "\r\n                   AND UM22.得意先ＣＤ = U22.得意先ＣＤ" +
                                  "\r\n                   AND UM22.部署ＣＤ = U22.部署ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS T22 ON U22.企業ＣＤ = T22.企業ＣＤ AND U22.得意先ＣＤ = T22.得意先ＣＤ" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = T22.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(T22.請求先ＣＤ, T22.得意先ＣＤ)" +
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                       ,MAX(H2.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H2" +
                                  "\r\n                       INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = H2.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(H2.請求先ＣＤ, H2.得意先ＣＤ)" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H2.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H2.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H2.得意先ＣＤ" +
                                  "\r\n                       ,H2.部署ＣＤ" +
                                  "\r\n                   ) AS M22 ON" +
                                  "\r\n                       M22.得意先ＣＤ = U22.得意先ＣＤ" +
                                  "\r\n                       AND M22.部署ＣＤ = U22.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               U22.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S22.部署合算請求フラグ, 0) <> 0 AND S22.部署ＣＤ = @部署ＣＤ)" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                                  "\r\n               AND U22.売上日 > ISNULL(M22.前回締日, '2000/01/01')" +
                                  "\r\n               AND U22.売上日 < @対象年月一日" +
                                  "\r\n               AND UM22.売上区分 = @売上区分売掛" +       // -- 売掛
                                  "\r\n           UNION" +
                                  // -- 駆動表③－１ 今月（＝指定年月）の入金が存在する請求先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               S31.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N31" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S31 ON S31.企業ＣＤ = N31.企業ＣＤ AND S31.得意先ＣＤ = N31.得意先ＣＤ" +    // -- N31.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n           WHERE" +
                                  "\r\n               N31.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                                  "\r\n               AND (ISNULL(S31.部署合算請求フラグ, 0) = 0 AND N31.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N31.入金日 >= @対象年月一日" +
                                  "\r\n               AND N31.入金日 <= @対象年月末日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表③－２ 今月（＝指定年月）の入金が存在する請求先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               S32.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N32" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S32 ON S32.企業ＣＤ = N32.企業ＣＤ AND S32.得意先ＣＤ = N32.得意先ＣＤ" +    // -- N32.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n           WHERE" +
                                  "\r\n               N32.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S32.部署合算請求フラグ, 0) <> 0 AND S32.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N32.入金日 >= @対象年月一日" +
                                  "\r\n               AND N32.入金日 <= @対象年月末日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表④－１ 先月（＝前回締め以降）の入金が存在する請求先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               S41.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N41" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S41 ON S41.企業ＣＤ = N41.企業ＣＤ AND S41.得意先ＣＤ = N41.得意先ＣＤ" +    // -- N41.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G4.請求先ＣＤ" +
                                  "\r\n                       ,G4.部署ＣＤ" +
                                  "\r\n                       ,MAX(G4.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H4" +
                                  "\r\n                       INNER JOIN 請求合計 AS G4 ON H4.企業ＣＤ = G4.企業ＣＤ AND H4.請求番号 = G4.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = G4.企業ＣＤ AND S42.得意先ＣＤ = G4.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H4.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H4.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G4.請求先ＣＤ" +
                                  "\r\n                       ,G4.部署ＣＤ" +
                                  "\r\n                   ) AS M41 ON" +
                                  "\r\n                       M41.請求先ＣＤ = N41.得意先ＣＤ" +
                                  "\r\n                       AND M41.部署ＣＤ = N41.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               N41.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                                  "\r\n               AND (ISNULL(S41.部署合算請求フラグ, 0) = 0 AND N41.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N41.入金日 > ISNULL(M41.前回締日, '2000/01/01')" +
                                  "\r\n               AND N41.入金日 < @対象年月一日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表④－２ 先月（＝前回締め以降）の入金が存在する請求先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               S42.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N42" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = N42.企業ＣＤ AND S42.得意先ＣＤ = N42.得意先ＣＤ" +    // -- N42.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H4.請求先ＣＤ" +
                                  "\r\n                       ,H4.部署ＣＤ" +
                                  "\r\n                       ,MAX(H4.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H4" +
                                  "\r\n                       INNER JOIN 請求合計 AS G4 ON H4.企業ＣＤ = G4.企業ＣＤ AND H4.請求番号 = G4.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = G4.企業ＣＤ AND S42.得意先ＣＤ = G4.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H4.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H4.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H4.請求先ＣＤ" +
                                  "\r\n                       ,H4.部署ＣＤ" +
                                  "\r\n                   ) AS M42 ON" +
                                  "\r\n                       M42.請求先ＣＤ = N42.得意先ＣＤ" +
                                  "\r\n                       AND M42.部署ＣＤ = N42.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               N42.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S42.部署合算請求フラグ, 0) <> 0 AND S42.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N42.入金日 > ISNULL(M42.前回締日, '2000/01/01')" +
                                  "\r\n               AND N42.入金日 < @対象年月一日" +
                                  "\r\n           UNION" +
                                  // -- 駆動表⑤－１ 前回請求額が存在する請求先（部署合算請求フラグOFF）
                                  "\r\n           SELECT" +
                                  "\r\n               A51.請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               請求合計 AS A51" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S51 ON S51.企業ＣＤ = A51.企業ＣＤ AND S51.得意先ＣＤ = A51.請求先ＣＤ" +
                                  // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               INNER JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                       ,MAX(G5.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H5" +
                                  "\r\n                       INNER JOIN 請求合計 AS G5 ON H5.企業ＣＤ = G5.企業ＣＤ AND H5.請求番号 = G5.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = G5.企業ＣＤ AND S52.得意先ＣＤ = G5.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H5.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H5.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                   ) AS M51 ON" +
                                  "\r\n                       M51.請求先ＣＤ = A51.請求先ＣＤ" +
                                  "\r\n                       AND M51.部署ＣＤ = A51.部署ＣＤ" +
                                  "\r\n                       AND M51.前回締日 = A51.請求日" +
                                  "\r\n           WHERE" +
                                  "\r\n               A51.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、請求合計の部署を対象にする
                                  "\r\n               AND (ISNULL(S51.部署合算請求フラグ, 0) = 0 AND A51.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n           UNION" +
                                  // -- 駆動表⑤－２ 前回請求額が存在する請求先（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               A52.請求先ＣＤ" +
                                  "\r\n           FROM" +
                                  "\r\n               請求合計 AS A52" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = A52.企業ＣＤ AND S52.得意先ＣＤ = A52.請求先ＣＤ" +
                                  // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               INNER JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                       ,MAX(G5.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H5" +
                                  "\r\n                       INNER JOIN 請求合計 AS G5 ON H5.企業ＣＤ = G5.企業ＣＤ AND H5.請求番号 = G5.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = G5.企業ＣＤ AND S52.得意先ＣＤ = G5.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H5.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H5.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                   ) AS M52 ON" +
                                  "\r\n                       M52.請求先ＣＤ = A52.請求先ＣＤ" +
                                  "\r\n                       AND M52.部署ＣＤ = A52.部署ＣＤ" +
                                  "\r\n                       AND M52.前回締日 = A52.請求日" +
                                  "\r\n           WHERE" +
                                  "\r\n               A52.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S52.部署合算請求フラグ, 0) <> 0 AND S52.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n           ) AS D" +
                                  "\r\n           LEFT JOIN" +
                                  // -- 先月（＝前回締め以降）の入金（部署合算請求フラグOFF）
                                  "\r\n           (SELECT" +
                                  "\r\n               S41.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n               ,SUM(N41.入金額) AS 入金額" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N41" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S41 ON S41.企業ＣＤ = N41.企業ＣＤ AND S41.得意先ＣＤ = N41.得意先ＣＤ" +    // -- N41.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G4.請求先ＣＤ" +
                                  "\r\n                       ,G4.部署ＣＤ" +
                                  "\r\n                       ,MAX(G4.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H4" +
                                  "\r\n                       INNER JOIN 請求合計 AS G4 ON H4.企業ＣＤ = G4.企業ＣＤ AND H4.請求番号 = G4.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = G4.企業ＣＤ AND S42.得意先ＣＤ = G4.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H4.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H4.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G4.請求先ＣＤ" +
                                  "\r\n                       ,G4.部署ＣＤ" +
                                  "\r\n                   ) AS M41 ON" +
                                  "\r\n                       M41.請求先ＣＤ = N41.得意先ＣＤ" +
                                  "\r\n                       AND M41.部署ＣＤ = N41.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               N41.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                                  "\r\n               AND (ISNULL(S41.部署合算請求フラグ, 0) = 0 AND N41.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N41.入金日 > ISNULL(M41.前回締日, '2000/01/01')" +
                                  "\r\n               AND N41.入金日 < @対象年月一日" +
                                  "\r\n           GROUP BY" +
                                  "\r\n             S41.得意先ＣＤ" +
                                  "\r\n           UNION" +
                                  // -- 先月（＝前回締め以降）の入金（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               S42.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n               ,SUM(N42.入金額) AS 入金額" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N42" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = N42.企業ＣＤ AND S42.得意先ＣＤ = N42.得意先ＣＤ" +    // -- N42.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       H4.請求先ＣＤ" +
                                  "\r\n                       ,H4.部署ＣＤ" +
                                  "\r\n                       ,MAX(H4.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H4" +
                                  "\r\n                       INNER JOIN 請求合計 AS G4 ON H4.企業ＣＤ = G4.企業ＣＤ AND H4.請求番号 = G4.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S42 ON S42.企業ＣＤ = G4.企業ＣＤ AND S42.得意先ＣＤ = G4.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H4.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H4.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       H4.請求先ＣＤ" +
                                  "\r\n                       ,H4.部署ＣＤ" +
                                  "\r\n                   ) AS M42 ON" +
                                  "\r\n                       M42.請求先ＣＤ = N42.得意先ＣＤ" +
                                  "\r\n                       AND M42.部署ＣＤ = N42.部署ＣＤ" +
                                  "\r\n           WHERE" +
                                  "\r\n               N42.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S42.部署合算請求フラグ, 0) <> 0 AND S42.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N42.入金日 > ISNULL(M42.前回締日, '2000/01/01')" +
                                  "\r\n               AND N42.入金日 < @対象年月一日" +
                                  "\r\n           GROUP BY" +
                                  "\r\n             S42.得意先ＣＤ" +
                                  "\r\n           ) AS NB ON" +
                                  "\r\n               D.請求先ＣＤ = NB.請求先ＣＤ" +
                                  "\r\n           LEFT JOIN" +
                                  // -- 直前の月次売掛残高集計.今月売掛残高
                                  "\r\n           (SELECT" +
                                  "\r\n             A.請求先ＣＤ" +
                                  "\r\n             ,A.部署ＣＤ" +
                                  "\r\n             ,A.今月売掛残高" +
                                  "\r\n           FROM" +
                                  "\r\n             月次売掛残高集計 AS A" +
                                  "\r\n             INNER JOIN" +
                                  "\r\n                 (SELECT" +
                                  "\r\n                     請求先ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                     ,max(集計年*100 + 集計月) AS 集計年月" +
                                  "\r\n                 FROM" +
                                  "\r\n                     月次売掛残高集計" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND 部署ＣＤ = @部署ＣＤ" +
                                  "\r\n                     AND (集計年*100 + 集計月) < (@集計年*100 + @集計月)" +
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     請求先ＣＤ" +
                                  "\r\n                     ,部署ＣＤ" +
                                  "\r\n                 ) AS B ON" +
                                  "\r\n                     A.請求先ＣＤ = B.請求先ＣＤ" +
                                  "\r\n                     AND A.部署ＣＤ = B.部署ＣＤ" +
                                  "\r\n                     AND (A.集計年*100 + A.集計月) = B.集計年月" +
                                  "\r\n           ) AS ZB ON" +
                                  "\r\n             D.請求先ＣＤ = ZB.請求先ＣＤ" +
                                  "\r\n           LEFT JOIN" +
                                  // -- 今月（＝指定年月）の入金（部署合算請求フラグOFF）
                                  "\r\n           (SELECT" +
                                  "\r\n               S31.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n               ,SUM(N31.入金額) AS 入金額" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N31" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S31 ON S31.企業ＣＤ = N31.企業ＣＤ AND S31.得意先ＣＤ = N31.得意先ＣＤ" +    // -- N31.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n           WHERE" +
                                  "\r\n               N31.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                                  "\r\n               AND (ISNULL(S31.部署合算請求フラグ, 0) = 0 AND N31.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N31.入金日 >= @対象年月一日" +
                                  "\r\n               AND N31.入金日 <= @対象年月末日" +
                                  "\r\n           GROUP BY" +
                                  "\r\n               S31.得意先ＣＤ" +
                                  "\r\n           UNION" +
                                  // -- 今月（＝指定年月）の入金（部署合算請求フラグON）
                                  "\r\n           SELECT" +
                                  "\r\n               S32.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n               ,SUM(N32.入金額) AS 入金額" +
                                  "\r\n           FROM" +
                                  "\r\n               入金明細 AS N32" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S32 ON S32.企業ＣＤ = N32.企業ＣＤ AND S32.得意先ＣＤ = N32.得意先ＣＤ" +    // -- N32.得意先ＣＤは請求先ＣＤ
                                  // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n           WHERE" +
                                  "\r\n               N32.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S32.部署合算請求フラグ, 0) <> 0 AND S32.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n               AND N32.入金日 >= @対象年月一日" +
                                  "\r\n               AND N32.入金日 <= @対象年月末日" +
                                  "\r\n           GROUP BY" +
                                  "\r\n               S32.得意先ＣＤ" +
                                  "\r\n           ) AS NA ON" +
                                  "\r\n               D.請求先ＣＤ = NA.請求先ＣＤ" +
                                  "\r\n           LEFT JOIN" +
                                  // -- 前回請求額（部署合算請求フラグOFF）
                                  // --（前回の請求が部署ごとに複数存在し、今回から部署合算請求フラグが立った場合、前回の請求を１つにまとめる。フラグが立ってない場合はもともと一つの前回請求を取得するはず）
                                  "\r\n           (SELECT" +
                                  "\r\n               A51.請求先ＣＤ" +
                                  "\r\n               ,MAX(A51.請求日) AS 前回締日" +
                                  "\r\n               ,SUM(A51.今回請求額) AS 今回請求額" +
                                  "\r\n           FROM" +
                                  "\r\n               請求合計 AS A51" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S51 ON S51.企業ＣＤ = A51.企業ＣＤ AND S51.得意先ＣＤ = A51.請求先ＣＤ" +
                                  // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               INNER JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                       ,MAX(G5.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H5" +
                                  "\r\n                       INNER JOIN 請求合計 AS G5 ON H5.企業ＣＤ = G5.企業ＣＤ AND H5.請求番号 = G5.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = G5.企業ＣＤ AND S52.得意先ＣＤ = G5.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H5.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H5.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                   ) AS M51 ON" +
                                  "\r\n                       M51.請求先ＣＤ = A51.請求先ＣＤ" +
                                  "\r\n                       AND M51.部署ＣＤ = A51.部署ＣＤ" +
                                  "\r\n                       AND M51.前回締日 = A51.請求日" +
                                  "\r\n           WHERE" +
                                  "\r\n               A51.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っていない場合は、請求合計の部署を対象にする
                                  "\r\n               AND (ISNULL(S51.部署合算請求フラグ, 0) = 0 AND A51.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n           GROUP BY" +
                                  "\r\n               A51.請求先ＣＤ" +
                                  "\r\n           UNION" +
                                  // -- 前回請求額（部署合算請求フラグON）
                                  // --（前回の請求が部署ごとに複数存在し、今回から部署合算請求フラグが立った場合、前回の請求を１つにまとめる。フラグが立ってない場合はもともと一つの前回請求を取得するはず）
                                  "\r\n           SELECT" +
                                  "\r\n               A52.請求先ＣＤ" +
                                  "\r\n               ,MAX(A52.請求日) AS 前回締日" +
                                  "\r\n               ,SUM(A52.今回請求額) AS 今回請求額" +
                                  "\r\n           FROM" +
                                  "\r\n               請求合計 AS A52" +
                                  "\r\n               INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = A52.企業ＣＤ AND S52.得意先ＣＤ = A52.請求先ＣＤ" +
                                  // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                                  "\r\n               INNER JOIN" +
                                  // -- 前回締日
                                  "\r\n                   (SELECT" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                       ,MAX(G5.請求日) AS 前回締日" +
                                  "\r\n                   FROM" +
                                  "\r\n                       請求ヘッダ AS H5" +
                                  "\r\n                       INNER JOIN 請求合計 AS G5 ON H5.企業ＣＤ = G5.企業ＣＤ AND H5.請求番号 = G5.請求番号" +
                                  "\r\n                       INNER JOIN 得意先マスタ AS S52 ON S52.企業ＣＤ = G5.企業ＣＤ AND S52.得意先ＣＤ = G5.請求先ＣＤ" +
                                  "\r\n                   WHERE" +
                                  "\r\n                       H5.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                       AND H5.請求日 < @対象年月一日" +
                                  "\r\n                   GROUP BY" +
                                  "\r\n                       G5.請求先ＣＤ" +
                                  "\r\n                       ,G5.部署ＣＤ" +
                                  "\r\n                   ) AS M52 ON" +
                                  "\r\n                       M52.請求先ＣＤ = A52.請求先ＣＤ" +
                                  "\r\n                       AND M52.部署ＣＤ = A52.部署ＣＤ" +
                                  "\r\n                       AND M52.前回締日 = A52.請求日" +
                                  "\r\n           WHERE" +
                                  "\r\n               A52.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n               AND (ISNULL(S52.部署合算請求フラグ, 0) <> 0 AND S52.部署ＣＤ = @部署ＣＤ)" +
                                  "\r\n           GROUP BY" +
                                  "\r\n               A52.請求先ＣＤ" +
                                  "\r\n           ) AS Z ON" +
                                  "\r\n               D.請求先ＣＤ = Z.請求先ＣＤ" +
                                  "\r\n       ) AS B" +
                                  "\r\n       INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND S.得意先ＣＤ = B.請求先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiSyukeiCriteria.部署ＣＤ;
                cmd.Parameters.Add("@対象年月一日", SqlDbType.Date).Value = _getsujiSyukeiCriteria.対象年月一日;
                cmd.Parameters.Add("@対象年月末日", SqlDbType.Date).Value = _getsujiSyukeiCriteria.対象年月末日;
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象年;
                cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象月;

                log.Debug(cmd.CommandTextWithParameters());

                // cmd.CommandTimeout = 180;           // サンワでタイムアウトが発生しているので延ばす
                cmd.CommandTimeout = timeoutSec;

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売掛残高集計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #月次売掛残高集計ヘッダTempが持つ値を合計して#月次売掛残高集計Tempを更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateUrikakeZandakaTemp(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // #月次売掛残高集計Tempの今月売上額は、#月次売掛残高集計ヘッダTempの合計ではなく、
                // #月次売掛残高集計ヘッダTempの今月売上金額の合計に対して税率を掛けて消費税を求め、今月売上額を求める必要がある。
                // 丸め処理をプログラムでやらないといけないので、SQLではできない。
                // #月次売掛残高集計ヘッダTempから請求先ＣＤ、税区分、丸め処理区分、計算税率ごとに今月売上金額 の合計を求め、
                // 請求先ＣＤごとの今月売上金額の合計を保持する。
                // ※先月売掛残高（先月の月次売掛残高集計.今月売掛残高）が無い場合は計算で求めるため、先月売上金額も上記同様に求める。
                // #月次売掛残高集計ヘッダTempの全てのレコード（＝全ての請求先ＣＤ）について算出が完了したら、
                // #月次売掛残高集計Tempに対してUpdateする。

                // まず今月から
                string selectSql = "\r\n SELECT" +
                                   "\r\n     A.請求先ＣＤ" +
                                   "\r\n     ,A.今月税区分        AS 税区分" +
                                   "\r\n     ,A.今月丸め処理区分  AS 丸め処理区分" +
                                   "\r\n     ,A.今月計算税率      AS 計算税率" +
                                   "\r\n     ,SUM(A.今月売上金額) AS 売上金額" +
                                   "\r\n FROM" +
                                   // -- 先月の税区分or丸め区分が混在していると今月が二重に集計されるのを回避すべく一旦DISTINCTする
                                   "\r\n     (SELECT DISTINCT" +
                                   "\r\n         得意先ＣＤ" +
                                   "\r\n         ,請求先ＣＤ" +
                                   "\r\n         ,今月税区分" +
                                   "\r\n         ,今月丸め処理区分" +
                                   "\r\n         ,今月計算税率" +
                                   "\r\n         ,今月売上金額" +
                                   "\r\n     FROM" +
                                   "\r\n         #月次売掛残高集計ヘッダTemp" +
                                   "\r\n     ) AS A" +
                                   "\r\n GROUP BY" +
                                   "\r\n     A.請求先ＣＤ" +
                                   "\r\n     ,A.今月税区分" +
                                   "\r\n     ,A.今月丸め処理区分" +
                                   "\r\n     ,A.今月計算税率" +
                                   "\r\n ORDER BY" +
                                   "\r\n     A.請求先ＣＤ";

                string updateSql = "\r\n UPDATE #月次売掛残高集計Temp" +
                                   "\r\n SET 今月売上額 = @売上額" +
                                   "\r\n WHERE 請求先ＣＤ = @請求先ＣＤ";

                if (UpdateUrikakeZandakaTempMain(con, tran, selectSql, updateSql) == false)
                {
                    return false;
                }

                // 続いて先月
                selectSql = "\r\n SELECT" +
                            "\r\n     A.請求先ＣＤ" +
                            "\r\n     ,A.先月税区分        AS 税区分" +
                            "\r\n     ,A.先月丸め処理区分  AS 丸め処理区分" +
                            "\r\n     ,A.先月計算税率      AS 計算税率" +
                            "\r\n     ,SUM(A.先月売上金額) AS 売上金額" +
                            "\r\n FROM" +
                            // -- 今月の税区分or丸め区分が混在していると先月が二重に集計されるのを回避すべく一旦DISTINCTする
                            "\r\n     (SELECT DISTINCT" +
                            "\r\n         得意先ＣＤ" +
                            "\r\n         ,請求先ＣＤ" +
                            "\r\n         ,先月税区分" +
                            "\r\n         ,先月丸め処理区分" +
                            "\r\n         ,先月計算税率" +
                            "\r\n         ,先月売上金額" +
                            "\r\n     FROM" +
                            "\r\n         #月次売掛残高集計ヘッダTemp" +
                            "\r\n     ) AS A" +
                            "\r\n GROUP BY" +
                            "\r\n     A.請求先ＣＤ" +
                            "\r\n     ,A.先月税区分" +
                            "\r\n     ,A.先月丸め処理区分" +
                            "\r\n     ,A.先月計算税率" +
                            "\r\n ORDER BY" +
                            "\r\n     A.請求先ＣＤ";

                // -- 先月の月次売掛残高集計.今月売掛残高 が無い場合（つまり月次売掛残高集計Temp.先月売掛残高 IS NULL）、先月請求額 - 先月入金額 + ヘッダTemp.先月売上金額の集計値
                updateSql = "\r\n UPDATE #月次売掛残高集計Temp" +
                            "\r\n SET 先月売掛残高 = ISNULL(先月請求額, 0) - ISNULL(先月入金額, 0) + @売上額" +
                            "\r\n WHERE 請求先ＣＤ = @請求先ＣＤ" +
                            "\r\n AND 先月売掛残高 IS NULL";

                if (UpdateUrikakeZandakaTempMain(con, tran, selectSql, updateSql) == false)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売掛残高集計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #月次売掛残高集計ヘッダTempが持つ値を合計して#月次売掛残高集計Tempを更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="selectSql">取得SQL</param>
        /// <param name="updateSql">更新SQL</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateUrikakeZandakaTempMain(SqlConnection con, SqlTransaction tran, string selectSql, string updateSql)
        {
            try
            {
                Dictionary<int, decimal> zeikomiMap = new Dictionary<int, decimal>();   // 売上金額税込マップ

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = selectSql;

                log.Debug(cmd.CommandTextWithParameters());
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("月次売掛残高集計ヘッダTemp");
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    int seikyusakiCD = (int)dr["請求先ＣＤ"];
                    // 売上が無い場合は税区分、丸め処理区分、計算税率はNull
                    int? zeiKubun = dr["税区分"] as int?;
                    int? hasuKubun = dr["丸め処理区分"] as int?;
                    decimal? taxRate = dr["計算税率"] as decimal?;
                    decimal kingaku = (decimal)dr["売上金額"];

                    decimal amountIncludeTax;
                    // 消費税額、売上金額税込を計算
                    if (zeiKubun == null)
                    {
                        amountIncludeTax = 0m;
                    }
                    else
                    {
                        amountIncludeTax = Syohizei.GetAmountIncludeTax(zeiKubun, kingaku, (decimal)taxRate, hasuKubun);
                    }

                    // log.DebugFormat("請求先={0} 税区分={1} 金額={2} 税率={3} 丸め区分={4} 税込み金額={5}", seikyusakiCD, zeiKubun, kingaku, taxRate, hasuKubun, amountIncludeTax);

                    if (zeikomiMap.ContainsKey(seikyusakiCD) == false)
                    {
                        zeikomiMap[seikyusakiCD] = 0m;
                    }
                    zeikomiMap[seikyusakiCD] += amountIncludeTax;
                }

                foreach (int seikyusakiCD in zeikomiMap.Keys)
                {
                    decimal amountIncludeTax = zeikomiMap[seikyusakiCD];

                    // #請求ヘッダTempが持つ値を合計して更新
                    SqlCommand cmdUpdate = con.CreateCommand();
                    cmdUpdate.Transaction = tran;
                    cmdUpdate.CommandText = updateSql;

                    cmdUpdate.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = seikyusakiCD;
                    cmdUpdate.Parameters.Add("@売上額", SqlDbType.Decimal).Value = amountIncludeTax;

                    log.Debug(cmdUpdate.CommandTextWithParameters());
                    cmdUpdate.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売掛残高集計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 月次売掛残高集計に出力
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUrikakeZandaka(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                UserData userData = UserData.getInstance();

                // 月次売掛残高集計に出力
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO 月次売掛残高集計 (" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,集計年" +
                                  "\r\n     ,集計月" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,先月売掛残高" +
                                  "\r\n     ,今月入金額" +
                                  "\r\n     ,今月売上額" +
                                  "\r\n     ,今月売掛残高" +
                                  "\r\n     ,作成担当者ＣＤ" +
                                  "\r\n     ,作成日" +
                                  "\r\n     ,修正担当者ＣＤ" +
                                  "\r\n     ,修正日" +
                                  "\r\n ) SELECT" +
                                  "\r\n     @企業ＣＤ" +
                                  "\r\n     ,@集計年" +
                                  "\r\n     ,@集計月" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,先月売掛残高" +
                                  "\r\n     ,今月入金額" +
                                  "\r\n     ,今月売上額" +
                                  "\r\n     ,ISNULL(先月売掛残高, 0) - ISNULL(今月入金額, 0) + ISNULL(今月売上額, 0) AS 今月売掛残高" +
                                  "\r\n     ,@作成担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n     ,@修正担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n FROM" +
                                  "\r\n     #月次売掛残高集計Temp" +
                                  "\r\n WHERE" +
                                  "\r\n     (ISNULL(先月売掛残高, 0) <> 0 OR ISNULL(今月入金額, 0) <> 0 OR ISNULL(今月売上額, 0) <> 0)";    // -- いずれも0なら作らない

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象年;
                cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象月;
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // 操作履歴に保存
                History.Save(con, tran, this.Text, "月次売掛残高集計", "", History.ProcessType.Create);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売掛残高集計の{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 月次売上集計を削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageSyukei(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n DELETE FROM 月次売上集計" +
                                  "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND 集計年 = @集計年" +
                                  "\r\n AND 集計月 = @集計月" +
                                  "\r\n AND 部署ＣＤ = @部署ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象年;
                cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象月;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiSyukeiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売上集計の{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 月次売上集計に出力
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUriageSyukei(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                UserData userData = UserData.getInstance();

                // 月次売掛残高集計に出力
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO 月次売上集計 (" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,集計年" +
                                  "\r\n     ,集計月" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,商品ＣＤ" +
                                  "\r\n     ,得意先名" +
                                  "\r\n     ,今月売上額" +
                                  "\r\n     ,今月数量" +
                                  "\r\n     ,今月値引額" +
                                  "\r\n     ,作成担当者ＣＤ" +
                                  "\r\n     ,作成日" +
                                  "\r\n     ,修正担当者ＣＤ" +
                                  "\r\n     ,修正日" +
                                  "\r\n )SELECT" +
                                  "\r\n     UM.企業ＣＤ" +
                                  "\r\n     ,@集計年" +
                                  "\r\n     ,@集計月" +
                                  "\r\n     ,UM.部署ＣＤ" +
                                  "\r\n     ,UM.得意先ＣＤ" +
                                  "\r\n     ,UM.商品ＣＤ" +
                                  "\r\n     ,T.得意先名" +
                                  "\r\n     ,UM.今月売上額" +
                                  "\r\n     ,UM.今月数量" +
                                  "\r\n     ,UM.今月値引額" +
                                  "\r\n     ,@作成担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n     ,@修正担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n FROM" +
                                  "\r\n     (SELECT" +
                                  "\r\n         企業ＣＤ" +
                                  "\r\n         ,部署ＣＤ" +
                                  "\r\n         ,得意先ＣＤ" +
                                  "\r\n         ,商品ＣＤ" +
                                  "\r\n         ,SUM((商品単価 * 商品数量) - ISNULL(値引額, 0)) AS 今月売上額" +
                                  "\r\n         ,SUM(商品数量) AS 今月数量" +
                                  "\r\n         ,SUM(値引額) AS 今月値引額" +
                                  "\r\n     FROM" +
                                  "\r\n         売上明細" +
                                  "\r\n     WHERE" +
                                  "\r\n         企業ＣＤ = @企業ＣＤ" +
                                  "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                  "\r\n         AND 売上日 >= @対象年月一日" +
                                  "\r\n         AND 売上日 <= @対象年月末日" +
                                  "\r\n         AND ISNULL(チケット束フラグ, 0) = 0" +
                                  "\r\n     GROUP BY" +
                                  "\r\n         企業ＣＤ" +
                                  "\r\n         ,部署ＣＤ" +
                                  "\r\n         ,得意先ＣＤ" +
                                  "\r\n         ,商品ＣＤ" +
                                  "\r\n     ) AS UM" +
                                  "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = UM.企業ＣＤ AND T.得意先ＣＤ = UM.得意先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiSyukeiCriteria.部署ＣＤ;
                cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象年;
                cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiSyukeiCriteria.対象月;
                cmd.Parameters.Add("@対象年月一日", SqlDbType.Date).Value = _getsujiSyukeiCriteria.対象年月一日;
                cmd.Parameters.Add("@対象年月末日", SqlDbType.Date).Value = _getsujiSyukeiCriteria.対象年月末日;
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // 操作履歴に保存
                History.Save(con, tran, this.Text, "月次売上集計", "", History.ProcessType.Create);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("月次売上集計の{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

    }
}

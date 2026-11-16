using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// 電子決済請求締め実行
    /// </summary>
    public static class PaySeikyuShimeExec
    {
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 締め処理実行（電子決済請求締め処理）
        /// </summary>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <param name="p">進行状況の更新のプロバイダー</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        public static string Run(PaySeikyuShimeCriteria paySeikyuShimeCriteria, IProgress<BaseForm.ProgressState> p)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    // 主処理実行
                    string result = RunMain(con, tran, paySeikyuShimeCriteria, p);
                    if (string.IsNullOrEmpty(result) == false)
                    {
                        return result;
                    }

                    tran.Commit();

                    return null;
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
        /// 締め主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">請求締め条件</param>
        /// <param name="p">進行状況の更新のプロバイダー</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string RunMain(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria, IProgress<BaseForm.ProgressState> p)
        {
            try
            {
                int timeoutSec = SystemMaster.GetTimeoutSecForPaySeikyuShimeExec(); // 電子決済請求締め処理タイムアウト秒数を取得
                string result;
                BaseForm.ProgressState progressState = new BaseForm.ProgressState();
                if (p != null)
                {
                    progressState.Style = ProgressBarStyle.Blocks;
                    progressState.Maximum = 7;
                    progressState.Value = 0;
                    progressState.Message = "処理中";
                    p.Report(progressState);
                }

                // 電子決済請求ヘッダを削除
                result = DeleteHeader(con, tran, paySeikyuShimeCriteria);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                // 電子決済請求合計を削除
                result = DeleteTotal(con, tran, paySeikyuShimeCriteria);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                // #電子決済請求ヘッダTempを作成
                result = CreateHeaderTemp(con, tran, paySeikyuShimeCriteria, timeoutSec);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                // #電子決済請求合計Tempを作成
                result = CreateTotalTemp(con, tran, paySeikyuShimeCriteria);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                // #電子決済請求ヘッダTempが持つ値を合計して#電子決済請求合計Tempを更新
                result = UpdateTotalTemp(con, tran, paySeikyuShimeCriteria);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                // 電子決済請求合計を作成
                result = CreateTotal(con, tran, paySeikyuShimeCriteria);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                // 電子決済請求ヘッダを作成
                result = CreateHeader(con, tran, paySeikyuShimeCriteria);
                if (string.IsNullOrEmpty(result) == false)
                {
                    return result;
                }

                if (p != null)
                {
                    progressState.Value++;
                    progressState.Message += ".";
                    p.Report(progressState);
                }

                log.InfoFormat("電子決済請求ヘッダと電子決済請求合計を{0}しました。", History.ProcessType.Create.DisplayName());

                return null;
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
        /// 電子決済請求ヘッダを削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string DeleteHeader(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                // -- 削除対象の電子決済請求合計と同じ請求番号を持つ電子決済請求ヘッダを削除する
                cmd.CommandText = "\r\n DELETE H" +
                                  "\r\n FROM" +
                                  "\r\n     電子決済請求ヘッダ AS H" +
                                  "\r\n     INNER JOIN 電子決済請求合計 AS G ON G.企業ＣＤ = H.企業ＣＤ AND G.請求番号 = H.請求番号" +
                                  "\r\n WHERE G.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND G.請求日 = @請求日" +           // -- 今回締日（入力された請求日）
                                  "\r\n AND G.締区分 = @締区分月締め" +     // -- 常に月締め
                                  "\r\n AND G.売上区分 = @売上区分" +       // -- 画面で指定された売上区分
                                  "\r\n AND G.請求先ＣＤ = @請求先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = paySeikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@締区分月締め", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;
                cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = paySeikyuShimeCriteria.売上区分;
                cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = paySeikyuShimeCriteria.請求先ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求ヘッダの{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// 電子決済請求合計を削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string DeleteTotal(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                cmd.CommandText = "\r\n DELETE G" +
                                  "\r\n FROM" +
                                  "\r\n     電子決済請求合計 AS G" +
                                  "\r\n WHERE G.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND G.請求日 = @請求日" +           // -- 今回締日（入力された請求日）
                                  "\r\n AND G.締区分 = @締区分月締め" +     // -- 常に月締め
                                  "\r\n AND G.売上区分 = @売上区分" +       // -- 画面で指定された売上区分
                                  "\r\n AND G.請求先ＣＤ = @請求先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = paySeikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@締区分月締め", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;
                cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = paySeikyuShimeCriteria.売上区分;
                cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = paySeikyuShimeCriteria.請求先ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// #電子決済請求ヘッダTempを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        /// <remarks>一時テーブルはセッションの切断で自動的に削除されます</remarks>
        private static string CreateHeaderTemp(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria, int timeoutSec)
        {
            try
            {
                //*******************************************************************************************
                // 分配については全く考慮しない（配送先・注文元と電子決済は両立しないので）。
                // 売上区分に該当する電子決済請求得意先マスタの得意先に対して作成する。
                // つまり部署ごとに作成することはない。部署ＣＤには売上の部署ではなく得意先の部署を格納する。
                //*******************************************************************************************

                //*******************************************************************************************
                // 通常の請求締めとは異なり、消費税を考慮する必要が無いので、税区分と丸め処理区分は持たない。
                //*******************************************************************************************

                // #電子決済請求ヘッダTempを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n CREATE TABLE #電子決済請求ヘッダTemp (" +
                                  "\r\n     得意先ＣＤ int NOT NULL," +
                                  "\r\n     部署ＣＤ int NOT NULL," +
                                  "\r\n     得意先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     請求日 date NOT NULL," +
                                  "\r\n     請求先ＣＤ int NULL," +
                                  "\r\n     請求先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     売上金額 decimal(18, 0) NULL," +
                                  "\r\n     入金額 decimal(18, 0) NULL" +
                                  "\r\n )";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                //**********************************************
                // #電子決済請求ヘッダTempに出力するデータを取得
                //**********************************************
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     B.得意先ＣＤ" +
                                  "\r\n     ,T.部署ＣＤ" +      // -- 売上の部署ではなく、得意先の部署
                                  "\r\n     ,T.得意先名" +
                                  "\r\n     ,@請求日 AS 請求日" +
                                  "\r\n     ,@請求先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n     ,@請求先名 AS 請求先名" +
                                  "\r\n     ,B.売上金額" +
                                  "\r\n     ,B.入金額" +
                                  "\r\n FROM" +
                                  // -- 売上と入金から電子決済請求ヘッダを作る
                                  // -- 売上については、前回請求が有るものは前回請求日以降のものを、無ければ2000/01/01以降（≒指定された売上区分の過去全て）の売上を抽出する。
                                  // -- 入金については、前回請求が有るものは前回請求日以降のものを、無ければ2000/01/01以降の入金を抽出する。
                                  // -- 通常の請求は請求先から得意先が取得できるので、請求合計があれば必ず請求ヘッダもあるが、
                                  // -- 電子決済は請求先と得意先が無関係なので、電子決済請求合計があっても電子決済請求ヘッダがあるとは限らない
                                  // -- ※とは言え、合計もヘッダも、売上と入金のいずれかが存在すれば作成するので、必ずどちらも存在することになるはず。
                                  "\r\n     (SELECT" +
                                  "\r\n         D.得意先ＣＤ" +
                                  "\r\n         ,ISNULL(UA.売上金額, 0) AS 売上金額" +
                                  "\r\n         ,ISNULL(NA.入金額, 0) AS 入金額" +
                                  "\r\n     FROM" +
                                  // -- 駆動表① 売上が存在する得意先
                                  "\r\n         (SELECT" +
                                  "\r\n             U1.得意先ＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             売上ヘッダ AS U1" +
                                  "\r\n             INNER JOIN 売上明細 AS UM1" +
                                  "\r\n                 ON UM1.企業ＣＤ = U1.企業ＣＤ" +
                                  "\r\n                 AND UM1.売上日 = U1.売上日" +
                                  "\r\n                 AND UM1.得意先ＣＤ = U1.得意先ＣＤ" +
                                  "\r\n                 AND UM1.部署ＣＤ = U1.部署ＣＤ" +
                                  "\r\n             LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                 (SELECT" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                     ,MAX(H1.請求日) AS 前回締日" +
                                  "\r\n                 FROM" +
                                  "\r\n                     電子決済請求ヘッダ AS H1" +
                                  "\r\n                     INNER JOIN 電子決済請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     H1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND H1.請求日 < @請求日" +      // -- 今回締日（入力された請求日）
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                 ) AS M1 ON" +
                                  "\r\n                     M1.得意先ＣＤ = U1.得意先ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             U1.企業ＣＤ = @企業ＣＤ" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  "\r\n             AND U1.売上日 > ISNULL(M1.前回締日, '2000/01/01')" +
                                  "\r\n             AND U1.売上日 <= @請求日" +         // -- 今回締日（入力された請求日）
                                  "\r\n             AND UM1.売上区分 = @売上区分" +     // -- 画面で指定された売上区分
                                  "\r\n             AND UM1.TP決済番号 IS NOT NULL" +   // -- Tapionで決済済み
                                  "\r\n             AND UM1.TP取消決済番号 IS NULL" +   // -- 取り消されていない
                                  "\r\n         UNION" +
                                  // -- 駆動表② 入金が存在する得意先
                                  "\r\n         SELECT" +
                                  "\r\n             N1.得意先ＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             入金明細 AS N1" +
                                  "\r\n             LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                 (SELECT" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                     ,MAX(H1.請求日) AS 前回締日" +
                                  "\r\n                 FROM" +
                                  "\r\n                     電子決済請求ヘッダ AS H1" +
                                  "\r\n                     INNER JOIN 電子決済請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     H1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND H1.請求日 < @請求日" +      // -- 今回締日（入力された請求日）
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                 ) AS M2 ON" +
                                  "\r\n                     M2.得意先ＣＤ = N1.得意先ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             N1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND N1.入金日 > ISNULL(M2.前回締日, '2000/01/01')" +
                                  "\r\n             AND N1.入金日 <= @請求日" +             // -- 今回締日（入力された請求日）
                                  "\r\n             AND N1.入金方法 = @入金方法Tapion" +    // -- 9:Tapion
                                  "\r\n             AND N1.TP決済番号 IS NOT NULL" +        // -- Tapionで決済済み
                                  "\r\n             AND N1.TP取消決済番号 IS NULL" +        // -- 取り消されていない
                                  "\r\n         ) AS D" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 売上
                                  "\r\n         (SELECT" +
                                  "\r\n             U1.得意先ＣＤ" +
                                  "\r\n             ,SUM(UM1.値引後金額) AS 売上金額" +
                                  "\r\n         FROM" +
                                  "\r\n             売上ヘッダ AS U1" +
                                  "\r\n             INNER JOIN 売上明細 AS UM1" +
                                  "\r\n                 ON UM1.企業ＣＤ = U1.企業ＣＤ" +
                                  "\r\n                 AND UM1.売上日 = U1.売上日" +
                                  "\r\n                 AND UM1.得意先ＣＤ = U1.得意先ＣＤ" +
                                  "\r\n                 AND UM1.部署ＣＤ = U1.部署ＣＤ" +
                                  "\r\n             LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                 (SELECT" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                     ,MAX(H1.請求日) AS 前回締日" +
                                  "\r\n                 FROM" +
                                  "\r\n                     電子決済請求ヘッダ AS H1" +
                                  "\r\n                     INNER JOIN 電子決済請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     H1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND H1.請求日 < @請求日" +      // -- 今回締日（入力された請求日）
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                 ) AS M1 ON" +
                                  "\r\n                     M1.得意先ＣＤ = U1.得意先ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             U1.企業ＣＤ = @企業ＣＤ" +
                                  // -- 前回請求が有るものは前回請求日以降
                                  // -- 無ければ過去の売上全て
                                  "\r\n             AND U1.売上日 > ISNULL(M1.前回締日, '2000/01/01')" +
                                  "\r\n             AND U1.売上日 <= @請求日" +         // -- 今回締日（入力された請求日）
                                  "\r\n             AND UM1.売上区分 = @売上区分" +     // -- 画面で指定された売上区分
                                  "\r\n             AND UM1.TP決済番号 IS NOT NULL" +   // -- Tapionで決済済み
                                  "\r\n             AND UM1.TP取消決済番号 IS NULL" +   // -- 取り消されていない
                                  "\r\n         GROUP BY" +
                                  "\r\n             U1.得意先ＣＤ" +
                                  "\r\n         ) AS UA ON" +
                                  "\r\n             D.得意先ＣＤ = UA.得意先ＣＤ" +
                                  "\r\n         LEFT JOIN" +
                                  // -- 入金
                                  "\r\n         (SELECT" +
                                  "\r\n             N1.得意先ＣＤ" +
                                  "\r\n             ,SUM(N1.入金額) AS 入金額" +
                                  "\r\n         FROM" +
                                  "\r\n             入金明細 AS N1" +
                                  "\r\n             LEFT JOIN" +
                                  // -- 前回締日
                                  "\r\n                 (SELECT" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                     ,MAX(H1.請求日) AS 前回締日" +
                                  "\r\n                 FROM" +
                                  "\r\n                     電子決済請求ヘッダ AS H1" +
                                  "\r\n                     INNER JOIN 電子決済請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号" +
                                  "\r\n                 WHERE" +
                                  "\r\n                     H1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n                     AND H1.請求日 < @請求日" +      // -- 今回締日（入力された請求日）
                                  "\r\n                 GROUP BY" +
                                  "\r\n                     H1.得意先ＣＤ" +
                                  "\r\n                 ) AS M2 ON" +
                                  "\r\n                     M2.得意先ＣＤ = N1.得意先ＣＤ" +
                                  "\r\n         WHERE" +
                                  "\r\n             N1.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND N1.入金日 > ISNULL(M2.前回締日, '2000/01/01')" +
                                  "\r\n             AND N1.入金日 <= @請求日" +             // -- 今回締日（入力された請求日）
                                  "\r\n             AND N1.入金方法 = @入金方法Tapion" +    // -- 9:Tapion
                                  "\r\n             AND N1.TP決済番号 IS NOT NULL" +        // -- Tapionで決済済み
                                  "\r\n             AND N1.TP取消決済番号 IS NULL" +        // -- 取り消されていない
                                  "\r\n         GROUP BY" +
                                  "\r\n             N1.得意先ＣＤ" +
                                  "\r\n         ) AS NA ON" +
                                  "\r\n             D.得意先ＣＤ = NA.得意先ＣＤ" +
                                  "\r\n     ) AS B" +
                                  "\r\n     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = B.得意先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = paySeikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = paySeikyuShimeCriteria.請求先ＣＤ;
                cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = paySeikyuShimeCriteria.請求先名;
                cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = paySeikyuShimeCriteria.売上区分;
                cmd.Parameters.Add("@入金方法Tapion", SqlDbType.Int).Value = (int)Consts.NyukinHoho.Tapion;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("電子決済請求ヘッダTemp用データ");
                adp.Fill(dt);

                //******************************
                // #電子決済請求ヘッダTempに出力
                //******************************
                string sqlIns = "\r\n INSERT INTO #電子決済請求ヘッダTemp (" +
                                "\r\n     得意先ＣＤ" +
                                "\r\n     ,部署ＣＤ" +
                                "\r\n     ,得意先名" +
                                "\r\n     ,請求日" +
                                "\r\n     ,請求先ＣＤ" +
                                "\r\n     ,請求先名" +
                                "\r\n     ,売上金額" +
                                "\r\n     ,入金額" +
                                "\r\n ) VALUES (" +
                                "\r\n     @得意先ＣＤ" +
                                "\r\n     ,@部署ＣＤ" +
                                "\r\n     ,@得意先名" +
                                "\r\n     ,@請求日" +
                                "\r\n     ,@請求先ＣＤ" +
                                "\r\n     ,@請求先名" +
                                "\r\n     ,@売上金額" +
                                "\r\n     ,@入金額" +
                                "\r\n )";

                foreach (DataRow dr in dt.Rows)
                {
                    cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sqlIns;

                    if (dr.IsNull("得意先ＣＤ"))
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Decimal).Value = dr["得意先ＣＤ"];
                    }

                    if (dr.IsNull("部署ＣＤ"))
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = dr["部署ＣＤ"];
                    }

                    if (dr.IsNull("得意先名"))
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = dr["得意先名"];
                    }

                    if (dr.IsNull("請求日"))
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = dr["請求日"];
                    }

                    if (dr.IsNull("請求先ＣＤ"))
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = dr["請求先ＣＤ"];
                    }

                    if (dr.IsNull("請求先名"))
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = dr["請求先名"];
                    }

                    if (dr.IsNull("売上金額"))
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = 0m;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = dr["売上金額"];
                    }

                    if (dr.IsNull("入金額"))
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = 0m;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = dr["入金額"];
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        return "別の端末で更新済みです。再度検索してください。";
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求ヘッダ一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// #電子決済請求合計Tempを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string CreateTotalTemp(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria)
        {
            try
            {
                // #電子決済請求合計Tempを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n CREATE TABLE #電子決済請求合計Temp (" +
                                  "\r\n     請求日 date NOT NULL," +
                                  "\r\n     締区分 int NOT NULL," +
                                  "\r\n     売上区分 int NOT NULL," +
                                  "\r\n     部署ＣＤ int NOT NULL," +
                                  "\r\n     請求先ＣＤ int NULL," +
                                  "\r\n     請求先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     入金額 decimal(20) NULL," +
                                  "\r\n     買上金額 decimal(20) NULL," +
                                  "\r\n     調整額 decimal(20) NULL," +
                                  "\r\n     今回請求額 decimal(20) NULL," +
                                  "\r\n     回収予定日 date NULL" +
                                  "\r\n )";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // #電子決済請求合計Tempに出力
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO #電子決済請求合計Temp (" +
                                  "\r\n     請求日" +
                                  "\r\n     ,締区分" +
                                  "\r\n     ,売上区分" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,入金額" +          // -- 電子決済請求ヘッダから
                                  "\r\n     ,買上金額" +        // -- 電子決済請求ヘッダから
                                  "\r\n     ,調整額" +
                                  "\r\n     ,今回請求額" +      // -- 電子決済請求ヘッダから
                                  "\r\n     ,回収予定日" +
                                  "\r\n ) SELECT" +
                                  "\r\n       @請求日 AS 請求日" +
                                  "\r\n       ,@締区分月締め AS 締区分" +       // -- 常に月締め
                                  "\r\n       ,@売上区分 AS 売上区分" +         // -- 画面で指定された売上区分
                                  "\r\n       ,S.部署ＣＤ" +
                                  "\r\n       ,S.得意先ＣＤ AS 請求先ＣＤ" +
                                  "\r\n       ,S.得意先名 AS 請求先名" +
                                  "\r\n       ,0 AS 入金額" +
                                  "\r\n       ,0 AS 買上金額" +
                                  "\r\n       ,0 AS 調整額" +
                                  "\r\n       ,0 AS 今回請求額" +
                                  "\r\n       ,NULL AS 回収予定日" +        // -- プログラム側にて算出する
                                  "\r\n   FROM" +
                                  // -- 電子決済請求ヘッダの売上金額と入金額の合計を取得するので、ここではそれを格納するレコードを作るだけ
                                  "\r\n       得意先マスタ AS S " +
                                  "\r\n   WHERE" +
                                  "\r\n       S.企業ＣＤ = @企業ＣＤ " +
                                  "\r\n       AND S.得意先ＣＤ = @請求先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = paySeikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = paySeikyuShimeCriteria.請求先ＣＤ;
                cmd.Parameters.Add("@締区分月締め", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;
                cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = paySeikyuShimeCriteria.売上区分;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求合計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// #電子決済請求ヘッダTempが持つ値を合計して#電子決済請求合計Tempを更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string UpdateTotalTemp(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria)
        {
            try
            {
                // 通常の請求締めとは異なり、消費税は考慮不要
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n UPDATE G" +
                                  "\r\n SET G.買上金額 = ISNULL(T.売上金額, 0)," +
                                  "\r\n     G.入金額 = ISNULL(T.入金額, 0)," +
                                  "\r\n     G.今回請求額 = ISNULL(T.入金額, 0) + ISNULL(T.売上金額, 0)" +
                                  "\r\n FROM" +
                                  "\r\n     #電子決済請求合計Temp AS G" +
                                  "\r\n     LEFT JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             請求先ＣＤ" +
                                  "\r\n             ,SUM(売上金額) AS 売上金額" +
                                  "\r\n             ,SUM(入金額) AS 入金額" +
                                  "\r\n         FROM" +
                                  "\r\n             #電子決済請求ヘッダTemp" +
                                  "\r\n         GROUP BY" +
                                  "\r\n             請求先ＣＤ" +
                                  "\r\n         ) AS T" +
                                  "\r\n         ON G.請求先ＣＤ = T.請求先ＣＤ";

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求合計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// 電子決済請求合計を作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">電子決済請求締め条件</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string CreateTotal(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria)
        {
            decimal seikyuNo;
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // 現在の請求番号を取得
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT ISNULL(MAX(請求番号), 0) AS 請求番号 FROM 電子決済請求合計 WHERE 企業ＣＤ = @企業ＣＤ";
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    seikyuNo = (decimal)reader["請求番号"];
                }

                // #電子決済請求合計Tempを取得
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     G.請求日" +
                                  "\r\n     ,G.締区分" +
                                  "\r\n     ,G.売上区分" +
                                  "\r\n     ,G.部署ＣＤ" +
                                  "\r\n     ,G.請求先ＣＤ" +
                                  "\r\n     ,G.請求先名" +
                                  "\r\n     ,G.入金額" +
                                  "\r\n     ,G.買上金額" +
                                  "\r\n     ,G.調整額" +
                                  "\r\n     ,G.今回請求額" +
                                  "\r\n     ,ISNULL(T.支払サイト, 0) AS 支払サイト" +
                                  "\r\n     ,T.支払日" +
                                  "\r\n FROM" +
                                  "\r\n     #電子決済請求合計Temp AS G" +
                                  "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ and T.得意先ＣＤ = G.請求先ＣＤ" +
                                  "\r\n WHERE" +
                                  // -- すべて0の場合は作成しない（ひとつでも0でなければ作成する）
                                  "\r\n     ISNULL(G.入金額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.買上金額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.調整額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.今回請求額, 0) <> 0" +
                                  "\r\n ORDER BY" +
                                  "\r\n     G.請求先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("電子決済請求合計Temp");
                adp.Fill(dt);

                // 電子決済請求合計に出力
                string sql = "\r\n INSERT INTO 電子決済請求合計(" +
                             "\r\n     企業ＣＤ" +
                             "\r\n     ,請求番号" +
                             "\r\n     ,請求日" +
                             "\r\n     ,締区分" +
                             "\r\n     ,売上区分" +
                             "\r\n     ,部署ＣＤ" +
                             "\r\n     ,請求先ＣＤ" +
                             "\r\n     ,請求先名" +
                             "\r\n     ,入金額" +
                             "\r\n     ,買上金額" +
                             "\r\n     ,調整額" +
                             "\r\n     ,今回請求額" +
                             "\r\n     ,回収予定日" +
                             "\r\n     ,印刷者ＣＤ" +
                             "\r\n     ,印刷日時" +
                             "\r\n     ,EB作成済フラグ" +
                             "\r\n     ,作成担当者ＣＤ" +
                             "\r\n     ,作成日" +
                             "\r\n     ,修正担当者ＣＤ" +
                             "\r\n     ,修正日" +
                             "\r\n ) VALUES (" +
                             "\r\n     @企業ＣＤ" +
                             "\r\n     ,@請求番号" +
                             "\r\n     ,@請求日" +
                             "\r\n     ,@締区分" +
                             "\r\n     ,@売上区分" +
                             "\r\n     ,@部署ＣＤ" +
                             "\r\n     ,@請求先ＣＤ" +
                             "\r\n     ,@請求先名" +
                             "\r\n     ,@入金額" +
                             "\r\n     ,@買上金額" +
                             "\r\n     ,@調整額" +
                             "\r\n     ,@今回請求額" +
                             "\r\n     ,@回収予定日" +
                             "\r\n     ,NULL" +
                             "\r\n     ,NULL" +
                             "\r\n     ,0" +
                             "\r\n     ,@作成担当者ＣＤ" +
                             "\r\n     ,getdate()" +
                             "\r\n     ,@修正担当者ＣＤ" +
                             "\r\n     ,getdate()" +
                             "\r\n )";

                UserData userData = UserData.getInstance();
                foreach (DataRow dr in dt.Rows)
                {
                    cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    seikyuNo++;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;

                    if (dr.IsNull("請求日"))
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = dr["請求日"];
                    }

                    if (dr.IsNull("締区分"))
                    {
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = dr["締区分"];
                    }

                    if (dr.IsNull("売上区分"))
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = dr["売上区分"];
                    }

                    if (dr.IsNull("部署ＣＤ"))
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = dr["部署ＣＤ"];
                    }

                    if (dr.IsNull("請求先ＣＤ"))
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = dr["請求先ＣＤ"];
                    }

                    if (dr.IsNull("請求先名"))
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = dr["請求先名"];
                    }

                    if (dr.IsNull("入金額"))
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = dr["入金額"];
                    }

                    if (dr.IsNull("買上金額"))
                    {
                        cmd.Parameters.Add("@買上金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@買上金額", SqlDbType.Decimal).Value = dr["買上金額"];
                    }

                    if (dr.IsNull("調整額"))
                    {
                        cmd.Parameters.Add("@調整額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@調整額", SqlDbType.Decimal).Value = dr["調整額"];
                    }

                    if (dr.IsNull("今回請求額"))
                    {
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = dr["今回請求額"];
                    }

                    // 回収予定日を計算
                    DateTime kaisyubi = GetKaisyubi((DateTime)paySeikyuShimeCriteria.請求日, (int)dr["支払サイト"], dr["支払日"] as int?);
                    cmd.Parameters.Add("@回収予定日", SqlDbType.Date).Value = kaisyubi;

                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        return "別の端末で更新済みです。再度検索してください。";
                    }

                    // 操作履歴に保存
                    History.Save(con, tran, "電子決済請求締処理", "電子決済請求合計", seikyuNo.ToString(), historyProcess);
                }

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// 電子決済請求ヘッダを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="paySeikyuShimeCriteria">請求締め条件</param>
        /// <returns>成功した場合は null。失敗した場合はエラーメッセージ</returns>
        private static string CreateHeader(SqlConnection con, SqlTransaction tran, PaySeikyuShimeCriteria paySeikyuShimeCriteria)
        {
            try
            {
                // 電子決済請求ヘッダに出力
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO 電子決済請求ヘッダ (" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,請求番号" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,得意先名" +
                                  "\r\n     ,請求日" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,売上金額" +
                                  "\r\n     ,入金額" +
                                  "\r\n     ,作成担当者ＣＤ" +
                                  "\r\n     ,作成日" +
                                  "\r\n     ,修正担当者ＣＤ" +
                                  "\r\n     ,修正日" +
                                  "\r\n ) SELECT" +
                                  "\r\n     C.企業ＣＤ" +
                                  "\r\n     ,C.請求番号" +
                                  "\r\n     ,A.得意先ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,A.得意先名" +
                                  "\r\n     ,A.請求日" +
                                  "\r\n     ,A.請求先ＣＤ" +
                                  "\r\n     ,A.請求先名" +
                                  "\r\n     ,A.売上金額" +
                                  "\r\n     ,A.入金額" +
                                  "\r\n     ,@作成担当者ＣＤ AS 作成担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n     ,@修正担当者ＣＤ AS 修正担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n FROM" +
                                  "\r\n     (SELECT" +
                                  "\r\n         得意先ＣＤ" +
                                  "\r\n         ,部署ＣＤ" +
                                  "\r\n         ,得意先名" +
                                  "\r\n         ,請求日" +
                                  "\r\n         ,請求先ＣＤ" +
                                  "\r\n         ,請求先名" +
                                  "\r\n         ,SUM(売上金額) AS 売上金額" +
                                  "\r\n         ,SUM(入金額) AS 入金額" +
                                  "\r\n     FROM" +
                                  "\r\n         #電子決済請求ヘッダTemp" +
                                  "\r\n     GROUP BY" +
                                  "\r\n         得意先ＣＤ" +
                                  "\r\n         ,部署ＣＤ" +
                                  "\r\n         ,得意先名" +
                                  "\r\n         ,請求日" +
                                  "\r\n         ,請求先ＣＤ" +
                                  "\r\n         ,請求先名" +
                                  "\r\n     ) AS A" +
                                  "\r\n     INNER JOIN #電子決済請求合計Temp AS B ON" +
                                  "\r\n         A.請求先ＣＤ = B.請求先ＣＤ AND A.請求日 = B.請求日" +
                                  "\r\n     INNER JOIN 電子決済請求合計 AS C ON" +
                                  "\r\n         B.請求先ＣＤ = C.請求先ＣＤ AND B.請求日 = C.請求日 AND C.企業ＣＤ = @企業ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // 操作履歴に保存
                History.Save(con, tran, "電子決済請求締処理", "電子決済請求ヘッダ", "", History.ProcessType.Create);

                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                return string.Format("電子決済請求ヘッダの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString());
            }
        }

        /// <summary>
        /// 回収予定日を算出
        /// </summary>
        /// <param name="seikyubi">請求日</param>
        /// <param name="shiharaiSite">支払サイト（0:当月～3:3ヶ月）</param>
        /// <param name="shiharaibi">支払日（1～28,99は末日）</param>
        /// <returns>
        /// 常に月締めとみなし、請求日に対して、支払サイトの月を加算し、日を支払日にする（支払日がnullの場合は日は請求日のまま）
        /// </returns>
        public static DateTime GetKaisyubi(DateTime seikyubi, int shiharaiSite, int? shiharaibi)
        {
            try
            {
                DateTime kaisyubi;
                // 請求日に対して支払サイトの値を(月と見なして)加算
                kaisyubi = seikyubi.AddMonths(shiharaiSite);

                // 支払日(day)をセット
                if (shiharaibi != null)     // 支払日がnullの場合、日は請求日のまま
                {
                    if (shiharaibi == 99)
                    {
                        kaisyubi = new DateTime(kaisyubi.Year, kaisyubi.Month, DateTime.DaysInMonth(kaisyubi.Year, kaisyubi.Month));
                    }
                    else
                    {
                        kaisyubi = new DateTime(kaisyubi.Year, kaisyubi.Month, (int)shiharaibi);
                    }
                }

                return kaisyubi;
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

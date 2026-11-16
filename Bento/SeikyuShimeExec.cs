using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// 請求締め実行
    /// </summary>
    public static class SeikyuShimeExec
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
        /// 締め処理実行（売上精算入力から未集金or回収現金の請求を作成）必ず(請求先単位ではなく)得意先単位で実行することになる
        /// </summary>
        /// <param name="uriageSeisanCriteria">売上精算入力検索条件</param>
        /// <param name="tokuisakiCDs">対象の得意先ＣＤの配列</param>
        /// <remarks>未集金or回収現金の請求というのは、現金の得意先に掛売したときの請求 と同じ意味</remarks>
        /// <returns>成功した場合は True</returns>
        public static bool Run(UriageSeisanCriteria uriageSeisanCriteria, string[] tokuisakiCDs)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    // オーバーロードメソッドを呼び出す
                    if (Run(con, tran, uriageSeisanCriteria, tokuisakiCDs) == false)
                    {
                        return false;
                    }

                    tran.Commit();

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
        }

        /// <summary>
        /// 締め処理実行（売上精算入力から未集金or回収現金の請求を作成）必ず(請求先単位ではなく)得意先単位で実行することになる
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="uriageSeisanCriteria">売上精算入力検索条件</param>
        /// <param name="tokuisakiCDs">対象の得意先ＣＤの配列</param>
        /// <remarks>未集金or回収現金の請求というのは、現金の得意先に掛売したときの請求 と同じ意味</remarks>
        /// <returns>成功した場合は True</returns>
        public static bool Run(SqlConnection con, SqlTransaction tran, UriageSeisanCriteria uriageSeisanCriteria, string[] tokuisakiCDs)
        {
            // 請求締め条件を生成
            SeikyuShimeCriteria seikyuShimeCriteria = new SeikyuShimeCriteria();
            seikyuShimeCriteria.請求日 = uriageSeisanCriteria.精算日;
            seikyuShimeCriteria.部署ＣＤ = uriageSeisanCriteria.部署ＣＤ;
            seikyuShimeCriteria.締区分 = (int)Consts.ShimeKubun.Tsuke;
            // 得意先ＣＤを配列で受け取っている（範囲で指定することはできない）ので、請求締め条件には格納しない

            // 配列をカンマ区切りに変換
            string joinedTokuisakiCD = null;
            if (tokuisakiCDs != null && tokuisakiCDs.Length != 0)
            {
                joinedTokuisakiCD = string.Join(",", tokuisakiCDs);
            }

            // 主処理実行
            return RunMain(con, tran, seikyuShimeCriteria, joinedTokuisakiCD);
        }

        /// <summary>
        /// 締め処理実行（請求締め処理）
        /// </summary>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <returns>成功した場合は True</returns>
        public static bool Run(SeikyuShimeCriteria seikyuShimeCriteria)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    // 主処理実行
                    if (RunMain(con, tran, seikyuShimeCriteria, null) == false)
                    {
                        return false;
                    }

                    tran.Commit();

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
        }

        /// <summary>
        /// 締め主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool RunMain(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                int timeoutSec = SystemMaster.GetTimeoutSecForSeikyuShimeExec();    // 請求締め処理タイムアウト秒数を取得

                // 請求ヘッダを削除
                if (DeleteHeader(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                // 請求合計を削除
                if (DeleteTotal(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                // #請求ヘッダTempを作成
                if (CreateHeaderTemp(con, tran, seikyuShimeCriteria, joinedTokuisakiCD, timeoutSec) == false)
                {
                    return false;
                }

                // #請求合計Tempを作成
                if (CreateTotalTemp(con, tran, seikyuShimeCriteria, joinedTokuisakiCD, timeoutSec) == false)
                {
                    return false;
                }

                // #請求ヘッダTempが持つ値を合計して#請求合計Tempを更新
                if (UpdateTotalTemp(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                // #Tempから追加するレコードを請求ヘッダから削除
                if (DeleteHeaderByTemp(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                // #Tempから追加するレコードを請求合計から削除
                if (DeleteTotalByTemp(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                // 請求合計を作成
                if (CreateTotal(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                // 請求ヘッダを作成
                if (CreateHeader(con, tran, seikyuShimeCriteria, joinedTokuisakiCD) == false)
                {
                    return false;
                }

                log.InfoFormat("請求ヘッダと請求合計を{0}しました。", History.ProcessType.Create.DisplayName());

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
        /// 請求ヘッダを削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool DeleteHeader(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                StringBuilder sql = new StringBuilder();
                // -- 削除対象の請求合計と同じ請求番号を持つ請求ヘッダを削除する
                sql.AppendLine(" DELETE H");
                sql.AppendLine(" FROM");
                sql.AppendLine("     請求ヘッダ AS H");
                sql.AppendLine("     INNER JOIN 請求合計 AS G ON G.企業ＣＤ = H.企業ＣＤ AND G.請求番号 = H.請求番号");
                // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = G.企業ＣＤ AND S.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = S.企業ＣＤ AND S.得意先ＣＤ = ISNULL(T.請求先ＣＤ, T.得意先ＣＤ)");
                sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                if (seikyuShimeCriteria.コースＣＤ != null)
                {
                    // -- コースによる絞り込み用
                    // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                    // -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
                    sql.AppendLine("     INNER JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.コースＣＤ;
                }

                // コースＣＤと同時に指定されることは無い
                if (seikyuShimeCriteria.コース外 == true)
                {
                    // -- コース外による絞り込み用
                    // -- 検索条件にコース外が指定された場合のみ、LEFT JOIN以下を追加すること
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                }

                sql.AppendLine(" WHERE G.企業ＣＤ = @企業ＣＤ");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine(" AND ((ISNULL(S.部署合算請求フラグ, 0) = 0 AND G.部署ＣＤ = @部署ＣＤ) OR (ISNULL(S.部署合算請求フラグ, 0) <> 0 AND S.部署ＣＤ = @部署ＣＤ))");
                sql.AppendLine(" AND G.請求日 = @請求日");
                sql.AppendLine(" AND G.締区分 = @締区分");

                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat(" AND T.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine(" AND G.請求先ＣＤ >= @請求先ＣＤFrom");
                        cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤFrom;
                    }

                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine(" AND G.請求先ＣＤ <= @請求先ＣＤTo");
                        cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤTo;
                    }
                }

                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine(" AND S.月締日 = @月締日");              // -- 月締めの場合
                    cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = seikyuShimeCriteria.月締日;
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine(" AND S.週締曜日 = @週締曜日");          // -- 週締めの場合
                    cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = seikyuShimeCriteria.週締曜日;
                }

                if (seikyuShimeCriteria.コース外 == true)
                {
                    sql.AppendLine(" AND C.コースＣＤ IS NULL");
                }

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.部署ＣＤ;
                cmd.Parameters.Add("@請求日", SqlDbType.DateTime).Value = seikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = seikyuShimeCriteria.締区分;

                cmd.CommandText = sql.ToString();
                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求ヘッダの{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 請求合計を削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool DeleteTotal(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" DELETE G");
                sql.AppendLine(" FROM ");
                sql.AppendLine("     請求合計 AS G");
                // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = G.企業ＣＤ AND S.得意先ＣＤ = G.請求先ＣＤ");
                sql.AppendLine("     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = S.企業ＣＤ AND S.得意先ＣＤ = ISNULL(T.請求先ＣＤ, T.得意先ＣＤ)");
                sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                if (seikyuShimeCriteria.コースＣＤ != null)
                {
                    // -- コースによる絞り込み用
                    // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                    // -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
                    sql.AppendLine("     INNER JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.コースＣＤ;
                }

                // コースＣＤと同時に指定されることは無い
                if (seikyuShimeCriteria.コース外 == true)
                {
                    // -- コース外による絞り込み用
                    // -- 検索条件にコース外が指定された場合のみ、LEFT JOIN以下を追加すること
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                }

                sql.AppendLine(" WHERE G.企業ＣＤ = @企業ＣＤ");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine(" AND ((ISNULL(S.部署合算請求フラグ, 0) = 0 AND G.部署ＣＤ = @部署ＣＤ) OR (ISNULL(S.部署合算請求フラグ, 0) <> 0 AND S.部署ＣＤ = @部署ＣＤ))");
                sql.AppendLine(" AND G.請求日 = @請求日");
                sql.AppendLine(" AND G.締区分 = @締区分");

                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat(" AND T.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine(" AND G.請求先ＣＤ >= @請求先ＣＤFrom");
                        cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤFrom;
                    }

                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine(" AND G.請求先ＣＤ <= @請求先ＣＤTo");
                        cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤTo;
                    }
                }

                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine(" AND S.月締日 = @月締日");              // -- 月締めの場合
                    cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = seikyuShimeCriteria.月締日;
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine(" AND S.週締曜日 = @週締曜日");          // -- 週締めの場合
                    cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = seikyuShimeCriteria.週締曜日;
                }

                if (seikyuShimeCriteria.コース外 == true)
                {
                    sql.AppendLine(" AND C.コースＣＤ IS NULL");
                }

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.部署ＣＤ;
                cmd.Parameters.Add("@請求日", SqlDbType.DateTime).Value = seikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = seikyuShimeCriteria.締区分;

                cmd.CommandText = sql.ToString();
                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #請求ヘッダTempを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>成功した場合は True</returns>
        /// <remarks>一時テーブルはセッションの切断で自動的に削除されます</remarks>
        private static bool CreateHeaderTemp(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD, int timeoutSec)
        {
            try
            {
                // ************************************************************************************
                // 請求ヘッダに登録する消費税／売上金額税込は、売上明細の合計から求める。
                // （売上明細の消費税額／売上金額税込の合計ではない）
                // したがって、売上明細とともに税率や丸め処理区分を（請求先の）得意先マスタから取得し、
                // 消費税額などを計算して#請求ヘッダTempへ登録する。
                // そのため、#請求ヘッダTempに、請求ヘッダにはない以下の項目を持たせる
                // ・税区分
                // ・丸め処理区分
                // ************************************************************************************

                // #請求ヘッダTempを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n CREATE TABLE #請求ヘッダTemp (" +
                                  "\r\n     得意先ＣＤ decimal(18, 0) NOT NULL," +
                                  "\r\n     部署ＣＤ int NOT NULL," +
                                  "\r\n     得意先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     請求日 date NOT NULL," +
                                  "\r\n     請求先ＣＤ int NULL," +
                                  "\r\n     請求先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     税区分 int NULL," +
                                  "\r\n     丸め処理区分 int NULL," +
                                  "\r\n     計算税率 decimal(3, 2) NULL," +
                                  "\r\n     売上金額 decimal(18, 0) NULL," +
                                  "\r\n     消費税 decimal(9, 0) NULL," +
                                  "\r\n     売上金額税込 decimal(18, 0) NULL" +
                                  "\r\n )";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                //**************************************
                // #請求ヘッダTempに出力するデータを取得
                //**************************************
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     B.得意先ＣＤ");
                sql.AppendLine("     ,ISNULL(B.部署ＣＤ, T.部署ＣＤ) AS 部署ＣＤ");
                sql.AppendLine("     ,T.得意先名");
                sql.AppendLine("     ,@請求日 AS 請求日");
                sql.AppendLine("     ,S.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("     ,S.得意先名 AS 請求先名");
                sql.AppendLine("     ,B.税区分");
                sql.AppendLine("     ,B.丸め処理区分");
                sql.AppendLine("     ,B.計算税率");
                sql.AppendLine("     ,B.売上金額");
                sql.AppendLine(" FROM");
                // -- 売上から請求ヘッダを作る
                // -- 売上については、前回請求が有るものは前回請求日以降のものを、無ければ売上区分が売掛の売上明細の最古の売上日以降のものを抽出する。
                // -- 請求合計があるのに請求ヘッダが無いという事態を避けるために、入金と前回請求がある得意先も駆動表に加える
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         D.得意先ＣＤ");
                sql.AppendLine("         ,UA.部署ＣＤ");
                sql.AppendLine("         ,UA.税区分");
                sql.AppendLine("         ,UA.丸め処理区分");
                sql.AppendLine("         ,UA.計算税率");
                sql.AppendLine("         ,ISNULL(UA.売上金額, 0) AS 売上金額");
                sql.AppendLine("     FROM");
                // -- 駆動表①－１ 売上が存在する得意先（部署合算請求フラグOFF）
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             U1.得意先ＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                     ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                 LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                         ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H1");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                     ) AS M11 ON");
                sql.AppendLine("                         M11.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                         AND M11.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                 AND UH.売上日 > ISNULL(M11.前回締日, '2000/01/01')");
                sql.AppendLine("                 AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                 AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                 AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("             ) AS U1");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T11 ON U1.企業ＣＤ = T11.企業ＣＤ AND U1.得意先ＣＤ = T11.得意先ＣＤ");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                sql.AppendLine("             (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S11.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S11.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S11.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S11.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S11.得意先ＣＤ >= @請求先ＣＤFrom");
                    }

                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S11.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         UNION");
                // -- 駆動表①－２ 売上が存在する得意先（部署合算請求フラグON）
                sql.AppendLine("         SELECT");
                sql.AppendLine("             U2.得意先ＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                     ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                 LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                         ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H1");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                     ) AS M12 ON");
                sql.AppendLine("                         M12.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                         AND M12.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                 AND UH.売上日 > ISNULL(M12.前回締日, '2000/01/01')");
                sql.AppendLine("                 AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                 AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                 AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("             ) AS U2");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T12 ON U2.企業ＣＤ = T12.企業ＣＤ AND U2.得意先ＣＤ = T12.得意先ＣＤ");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("             (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S12.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S12.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S12.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S12.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S12.得意先ＣＤ >= @請求先ＣＤFrom");
                    }

                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S12.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         UNION");
                // -- 駆動表②－１ 入金が存在する請求先を持つ得意先（部署合算請求フラグOFF）
                sql.AppendLine("         SELECT");
                sql.AppendLine("             T21.得意先ＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                 N.企業ＣＤ");
                sql.AppendLine("                 ,N.部署ＣＤ");
                sql.AppendLine("                 ,N.得意先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 入金明細 AS N");
                sql.AppendLine("                 LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         G2.請求先ＣＤ");
                sql.AppendLine("                         ,G2.部署ＣＤ");
                sql.AppendLine("                         ,MAX(G2.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H2");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H2.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H2.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         G2.請求先ＣＤ");
                sql.AppendLine("                         ,G2.部署ＣＤ");
                sql.AppendLine("                     ) AS M21 ON");
                sql.AppendLine("                         M21.請求先ＣＤ = N.得意先ＣＤ");
                sql.AppendLine("                         AND M21.部署ＣＤ = N.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 N.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND N.入金日 > ISNULL(M21.前回締日, '2000/01/01')");
                sql.AppendLine("                 AND N.入金日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("             ) AS N1");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = N1.企業ＣＤ AND S21.得意先ＣＤ = N1.得意先ＣＤ");   // -- N1.得意先ＣＤは請求先ＣＤ
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T21 ON T21.企業ＣＤ = S21.企業ＣＤ AND S21.得意先ＣＤ = ISNULL(T21.請求先ＣＤ, T21.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                sql.AppendLine("             (ISNULL(S21.部署合算請求フラグ, 0) = 0 AND N1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S21.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S21.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S21.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S21.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("             AND T21.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S21.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S21.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         UNION");
                // -- 駆動表②－２ 入金が存在する請求先を持つ得意先（部署合算請求フラグON）
                sql.AppendLine("         SELECT");
                sql.AppendLine("             T22.得意先ＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                 N.企業ＣＤ");
                sql.AppendLine("                 ,N.得意先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 入金明細 AS N");
                sql.AppendLine("                 LEFT JOIN");
                // -- 前回締日（部署合算請求フラグONのときの入金額は部署を無視する）
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         H2.請求先ＣＤ");
                sql.AppendLine("                         ,MAX(H2.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H2");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H2.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H2.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         H2.請求先ＣＤ");
                sql.AppendLine("                     ) AS M22 ON");
                sql.AppendLine("                         M22.請求先ＣＤ = N.得意先ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 N.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND N.入金日 > ISNULL(M22.前回締日, '2000/01/01')");
                sql.AppendLine("                 AND N.入金日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("             ) AS N2");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = N2.企業ＣＤ AND S22.得意先ＣＤ = N2.得意先ＣＤ");   // -- N2.得意先ＣＤは請求先ＣＤ
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T22 ON T22.企業ＣＤ = S22.企業ＣＤ AND S22.得意先ＣＤ = ISNULL(T22.請求先ＣＤ, T22.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("             (ISNULL(S22.部署合算請求フラグ, 0) <> 0 AND S22.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S22.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S22.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S22.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S22.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("             AND T22.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S22.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S22.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         UNION");
                // -- 駆動表③－１ 前回請求額が存在する請求先を持つ得意先（部署合算請求フラグOFF）
                sql.AppendLine("         SELECT");
                sql.AppendLine("             T31.得意先ＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                 A.企業ＣＤ");
                sql.AppendLine("                 ,A.部署ＣＤ");
                sql.AppendLine("                 ,A.請求先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求合計 AS A");
                sql.AppendLine("                 INNER JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         G3.請求先ＣＤ");
                sql.AppendLine("                         ,G3.部署ＣＤ");
                sql.AppendLine("                         ,MAX(G3.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H3");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G3 ON H3.企業ＣＤ = G3.企業ＣＤ AND H3.請求番号 = G3.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H3.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H3.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         G3.請求先ＣＤ");
                sql.AppendLine("                         ,G3.部署ＣＤ");
                sql.AppendLine("                     ) AS M31 ON");
                sql.AppendLine("                         M31.請求先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("                         AND M31.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("                         AND M31.前回締日 = A.請求日");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 A.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             ) AS A1");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S31 ON S31.企業ＣＤ = A1.企業ＣＤ AND S31.得意先ＣＤ = A1.請求先ＣＤ");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T31 ON T31.企業ＣＤ = S31.企業ＣＤ AND S31.得意先ＣＤ = ISNULL(T31.請求先ＣＤ, T31.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っていない場合は、請求合計の部署を対象にする
                sql.AppendLine("             (ISNULL(S31.部署合算請求フラグ, 0) = 0 AND A1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S31.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S31.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S31.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S31.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("             AND T31.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S31.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S31.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         UNION");
                // -- 駆動表③－２ 前回請求額が存在する請求先を持つ得意先（部署合算請求フラグON）
                sql.AppendLine("         SELECT");
                sql.AppendLine("             T32.得意先ＣＤ");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT DISTINCT");
                sql.AppendLine("                 A.企業ＣＤ");
                sql.AppendLine("                 ,A.請求先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 請求合計 AS A");
                sql.AppendLine("                 INNER JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         G3.請求先ＣＤ");
                sql.AppendLine("                         ,G3.部署ＣＤ");
                sql.AppendLine("                         ,MAX(G3.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H3");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G3 ON H3.企業ＣＤ = G3.企業ＣＤ AND H3.請求番号 = G3.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H3.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H3.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         G3.請求先ＣＤ");
                sql.AppendLine("                         ,G3.部署ＣＤ");
                sql.AppendLine("                     ) AS M32 ON");
                sql.AppendLine("                         M32.請求先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("                         AND M32.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("                         AND M32.前回締日 = A.請求日");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 A.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             ) AS A2");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S32 ON S32.企業ＣＤ = A2.企業ＣＤ AND S32.得意先ＣＤ = A2.請求先ＣＤ");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T32 ON T32.企業ＣＤ = S32.企業ＣＤ AND S32.得意先ＣＤ = ISNULL(T32.請求先ＣＤ, T32.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("             (ISNULL(S32.部署合算請求フラグ, 0) <> 0 AND S32.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S32.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S32.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S32.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S32.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("             AND T32.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S32.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S32.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         ) AS D");
                sql.AppendLine("         LEFT JOIN");
                // -- 売上（部署合算請求フラグOFF）
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             U1.得意先ＣＤ");
                sql.AppendLine("             ,U1.部署ＣＤ");
                sql.AppendLine("             ,ISNULL(S11.税区分, @税区分内税) AS 税区分");                          // -- 初期値は1:内税
                sql.AppendLine("             ,ISNULL(S11.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");    // -- 初期値は2:切り捨て
                sql.AppendLine("             ,U1.計算税率");
                sql.AppendLine("             ,SUM(U1.値引後金額) AS 売上金額");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("                 ,UM.値引後金額");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                     ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                 LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                         ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H1");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                     ) AS M11 ON");
                sql.AppendLine("                         M11.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                         AND M11.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                 AND UH.売上日 > ISNULL(M11.前回締日, '2000/01/01')");
                sql.AppendLine("                 AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                 AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                 AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("             ) AS U1");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T11 ON U1.企業ＣＤ = T11.企業ＣＤ AND U1.得意先ＣＤ = T11.得意先ＣＤ");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                sql.AppendLine("             (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S11.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S11.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S11.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S11.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S11.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S11.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             U1.得意先ＣＤ");
                sql.AppendLine("             ,U1.部署ＣＤ");
                sql.AppendLine("             ,ISNULL(S11.税区分, @税区分内税)");
                sql.AppendLine("             ,ISNULL(S11.丸め処理区分, @丸め処理区分切り捨て)");
                sql.AppendLine("             ,U1.計算税率");
                sql.AppendLine("         UNION");
                // -- 売上（部署合算請求フラグON）
                sql.AppendLine("         SELECT");
                sql.AppendLine("             U2.得意先ＣＤ");
                sql.AppendLine("             ,U2.部署ＣＤ");
                sql.AppendLine("             ,ISNULL(S12.税区分, @税区分内税) AS 税区分");                          // -- 初期値は1:内税
                sql.AppendLine("             ,ISNULL(S12.丸め処理区分, @丸め処理区分切り捨て) AS 丸め処理区分");    // -- 初期値は2:切り捨て
                sql.AppendLine("             ,U2.計算税率");
                sql.AppendLine("             ,SUM(U2.値引後金額) AS 売上金額");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 UH.企業ＣＤ");
                sql.AppendLine("                 ,UH.部署ＣＤ");
                sql.AppendLine("                 ,UH.得意先ＣＤ");
                sql.AppendLine("                 ,UM.計算税率");
                sql.AppendLine("                 ,UM.値引後金額");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上ヘッダ AS UH");
                sql.AppendLine("                 INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                     ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                     AND UM.売上日 = UH.売上日");
                sql.AppendLine("                     AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                     AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                 LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                     (SELECT");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                         ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                     FROM");
                sql.AppendLine("                         請求ヘッダ AS H1");
                sql.AppendLine("                         INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                     WHERE");
                sql.AppendLine("                         H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                         AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                     GROUP BY");
                sql.AppendLine("                         H1.得意先ＣＤ");
                sql.AppendLine("                         ,H1.部署ＣＤ");
                sql.AppendLine("                     ) AS M12 ON");
                sql.AppendLine("                         M12.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                         AND M12.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                 AND UH.売上日 > ISNULL(M12.前回締日, '2000/01/01')");
                sql.AppendLine("                 AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                 AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                 AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("             ) AS U2");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS T12 ON U2.企業ＣＤ = T12.企業ＣＤ AND U2.得意先ＣＤ = T12.得意先ＣＤ");
                sql.AppendLine("             INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)");
                sql.AppendLine("         WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("             (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("             AND S12.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("             AND S12.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("             AND S12.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("             AND S12.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                    // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("             AND S12.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("             AND S12.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             U2.得意先ＣＤ");
                sql.AppendLine("             ,U2.部署ＣＤ");
                sql.AppendLine("             ,ISNULL(S12.税区分, @税区分内税)");
                sql.AppendLine("             ,ISNULL(S12.丸め処理区分, @丸め処理区分切り捨て)");
                sql.AppendLine("             ,U2.計算税率");
                sql.AppendLine("         ) AS UA ON");
                sql.AppendLine("             D.得意先ＣＤ = UA.得意先ＣＤ");
                sql.AppendLine("     ) AS B");
                sql.AppendLine("     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = B.得意先ＣＤ");
                sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.得意先ＣＤ = ISNULL(T.請求先ＣＤ, T.得意先ＣＤ)");
                // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                if (seikyuShimeCriteria.コースＣＤ != null)
                {
                    // -- コースによる絞り込み用
                    // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                    // -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
                    sql.AppendLine("     INNER JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.コースＣＤ;
                }

                // コースＣＤと同時に指定されることは無い
                if (seikyuShimeCriteria.コース外 == true)
                {
                    // -- コース外による絞り込み用
                    // -- 検索条件にコース外が指定された場合のみ、LEFT JOIN以下を追加すること
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                    sql.AppendLine(" WHERE C.コースＣＤ IS NULL");
                }

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.部署ＣＤ;

                // 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                }
                else
                {
                    cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;
                }
                cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = seikyuShimeCriteria.請求日;
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = seikyuShimeCriteria.締区分;
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = seikyuShimeCriteria.月締日;
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = seikyuShimeCriteria.週締曜日;
                }
                if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                {
                    cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤFrom;
                }
                if (seikyuShimeCriteria.請求先ＣＤTo != null)
                {
                    cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤTo;
                }
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                cmd.Parameters.Add("@丸め処理区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;

                cmd.CommandText = sql.ToString();
                log.Debug(cmd.CommandTextWithParameters());

                // 得意先CDを指定＆売上明細の件数が多い(200件以上ある)場合にタイムアウトすることがあるので、それを回避する
                // ※なぜかSQL単体では1秒も掛からない
                //cmd.CommandTimeout = 60;
                // cmd.CommandTimeout = 180;           // サンワでタイムアウトが発生しているのでさらに延ばす
                cmd.CommandTimeout = timeoutSec;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("請求ヘッダTemp用データ");
                adp.Fill(dt);

                //**********************
                // #請求ヘッダTempに出力
                //**********************
                string sqlIns = "\r\n INSERT INTO #請求ヘッダTemp (" +
                                "\r\n     得意先ＣＤ" +
                                "\r\n     ,部署ＣＤ" +
                                "\r\n     ,得意先名" +
                                "\r\n     ,請求日" +
                                "\r\n     ,請求先ＣＤ" +
                                "\r\n     ,請求先名" +
                                "\r\n     ,税区分" +
                                "\r\n     ,丸め処理区分" +
                                "\r\n     ,計算税率" +
                                "\r\n     ,売上金額" +
                                "\r\n     ,消費税" +
                                "\r\n     ,売上金額税込" +
                                "\r\n ) VALUES (" +
                                "\r\n     @得意先ＣＤ" +
                                "\r\n     ,@部署ＣＤ" +
                                "\r\n     ,@得意先名" +
                                "\r\n     ,@請求日" +
                                "\r\n     ,@請求先ＣＤ" +
                                "\r\n     ,@請求先名" +
                                "\r\n     ,@税区分" +
                                "\r\n     ,@丸め処理区分" +
                                "\r\n     ,@計算税率" +
                                "\r\n     ,@売上金額" +
                                "\r\n     ,@消費税" +
                                "\r\n     ,@売上金額税込" +
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

                    if (dr.IsNull("税区分"))
                    {
                        cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = dr["税区分"];
                    }

                    if (dr.IsNull("丸め処理区分"))
                    {
                        cmd.Parameters.Add("@丸め処理区分", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@丸め処理区分", SqlDbType.Int).Value = dr["丸め処理区分"];
                    }

                    if (dr.IsNull("計算税率"))
                    {
                        cmd.Parameters.Add("@計算税率", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@計算税率", SqlDbType.Decimal).Value = dr["計算税率"];
                    }

                    if (dr.IsNull("売上金額"))
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = 0m;
                        cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = 0m;
                        cmd.Parameters.Add("@売上金額税込", SqlDbType.Decimal).Value = 0m;
                    }
                    else
                    {
                        // 税区分・丸め処理区分・計算税率が取得できていないのは売上明細が無いということ
                        if (dr.IsNull("税区分"))
                        {
                            cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = dr["売上金額"];  // おそらく0のはず
                            cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = 0m;
                            cmd.Parameters.Add("@売上金額税込", SqlDbType.Decimal).Value = 0m;
                        }
                        else
                        {
                            int zeiKubun = (int)dr["税区分"];
                            int hasuKubun = (int)dr["丸め処理区分"];
                            decimal taxRate = (decimal)dr["計算税率"];
                            decimal kingaku = (decimal)dr["売上金額"];

                            // 消費税額、売上金額税込を計算
                            decimal tax = Syohizei.GetTax(zeiKubun, kingaku, taxRate, hasuKubun);
                            decimal amountIncludeTax = Syohizei.GetAmountIncludeTax(zeiKubun, kingaku, taxRate, hasuKubun);

                            cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = kingaku;
                            cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = tax;
                            cmd.Parameters.Add("@売上金額税込", SqlDbType.Decimal).Value = amountIncludeTax;
                        }
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        MessageBox.Show("別の端末で更新済みです。再度検索してください。", "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求ヘッダ一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #請求合計Tempを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <param name="timeoutSec">タイムアウト秒数</param>
        /// <returns>成功した場合は True</returns>
        private static bool CreateTotalTemp(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD, int timeoutSec)
        {
            try
            {
                // #請求合計Tempを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n CREATE TABLE #請求合計Temp (" +
                                  "\r\n     請求日 date NOT NULL," +
                                  "\r\n     締区分 int NOT NULL," +
                                  "\r\n     部署ＣＤ int NOT NULL," +
                                  "\r\n     請求先ＣＤ int NULL," +
                                  "\r\n     請求先名 varchar(100) COLLATE database_default NULL," +
                                  "\r\n     前回請求額 decimal(20) NULL," +
                                  "\r\n     入金額 decimal(20) NULL," +
                                  "\r\n     繰越額 decimal(20) NULL," +
                                  "\r\n     買上金額 decimal(20) NULL," +
                                  "\r\n     調整額 decimal(20) NULL," +
                                  "\r\n     消費税 decimal(11) NULL," +
                                  "\r\n     今回請求額 decimal(20) NULL," +
                                  "\r\n     回収予定日 date NULL" +
                                  "\r\n )";
                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // #請求合計Tempに出力
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO #請求合計Temp (");
                sql.AppendLine("     請求日");
                sql.AppendLine("     ,締区分");
                sql.AppendLine("     ,部署ＣＤ");
                sql.AppendLine("     ,請求先ＣＤ");
                sql.AppendLine("     ,請求先名");
                sql.AppendLine("     ,前回請求額");
                sql.AppendLine("     ,入金額");
                sql.AppendLine("     ,繰越額");
                sql.AppendLine("     ,買上金額");       // -- 請求ヘッダから
                sql.AppendLine("     ,調整額");
                sql.AppendLine("     ,消費税");         // -- 請求ヘッダから
                sql.AppendLine("     ,今回請求額");     // -- 請求ヘッダから
                sql.AppendLine("     ,回収予定日");
                sql.AppendLine(" ) SELECT");
                sql.AppendLine("       @請求日 AS 請求日");
                sql.AppendLine("       ,@締区分 AS 締区分");
                sql.AppendLine("       ,@部署ＣＤ AS 部署ＣＤ");
                sql.AppendLine("       ,B.請求先ＣＤ");
                sql.AppendLine("       ,S.得意先名 AS 請求先名");
                sql.AppendLine("       ,B.前回請求額");
                sql.AppendLine("       ,B.入金額");
                sql.AppendLine("       ,B.前回請求額 - B.入金額 AS 繰越額");
                sql.AppendLine("       ,0 AS 買上金額");
                sql.AppendLine("       ,0 AS 調整額");
                sql.AppendLine("       ,0 AS 消費税");
                sql.AppendLine("       ,0 AS 今回請求額");
                sql.AppendLine("       ,NULL AS 回収予定日");   // -- プログラム側にて算出する
                sql.AppendLine("   FROM");
                // -- 売上と入金と前回請求から請求ヘッダを作る
                // -- 売上については、前回請求が有るものは前回請求日以降のものを、無ければ売上区分が売掛の売上明細の最古の売上日以降のものを抽出する。
                // -- 入金については、前回請求が有るものは前回請求日以降のものを、無ければ無限に過去から取得
                // -- 請求合計は請求先単位で作成する。売上に関する項目は得意先単位なので請求ヘッダに作られるので、ここでは作成しない。
                sql.AppendLine("       (SELECT");
                sql.AppendLine("           D.請求先ＣＤ");
                sql.AppendLine("           ,ISNULL(UA.値引額, 0) AS 値引額");
                sql.AppendLine("           ,ISNULL(UA.値引前金額, 0) AS 値引前金額");
                sql.AppendLine("           ,ISNULL(UA.値引後金額, 0) AS 値引後金額");
                sql.AppendLine("           ,ISNULL(UA.消費税額, 0) AS 消費税額");
                sql.AppendLine("           ,ISNULL(UA.商品金額合計税込, 0) AS 商品金額合計税込");
                sql.AppendLine("           ,ISNULL(UA.売上フラグ, 0) AS 売上フラグ");
                sql.AppendLine("           ,ISNULL(NA.入金額, 0) AS 入金額");
                sql.AppendLine("           ,ISNULL(NA.入金フラグ, 0) AS 入金フラグ");
                sql.AppendLine("           ,ISNULL(Z.今回請求額, 0) AS 前回請求額");
                sql.AppendLine("           ,ISNULL(Z.前回請求フラグ, 0) AS 前回請求フラグ");
                sql.AppendLine("       FROM");
                // -- 駆動表①－１ 売上が存在する得意先の請求先（部署合算請求フラグOFF）
                sql.AppendLine("           (SELECT");
                sql.AppendLine("               S11.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT DISTINCT");
                sql.AppendLine("                   UH.企業ＣＤ");
                sql.AppendLine("                   ,UH.部署ＣＤ");
                sql.AppendLine("                   ,UH.得意先ＣＤ");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   売上ヘッダ AS UH");
                sql.AppendLine("                   INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                       ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                       AND UM.売上日 = UH.売上日");
                sql.AppendLine("                       AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                       AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                           ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H1");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                       ) AS M11 ON");
                sql.AppendLine("                           M11.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                           AND M11.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降"
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                   AND UH.売上日 > ISNULL(M11.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                   AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("               ) AS U1");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS T11 ON U1.企業ＣＤ = T11.企業ＣＤ AND U1.得意先ＣＤ = T11.得意先ＣＤ");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                sql.AppendLine("               (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S11.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S11.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S11.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S11.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S11.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S11.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           UNION");
                // -- 駆動表①－２ 売上が存在する得意先の請求先（部署合算請求フラグON）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               S12.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT DISTINCT");
                sql.AppendLine("                   UH.企業ＣＤ");
                sql.AppendLine("                   ,UH.得意先ＣＤ");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   売上ヘッダ AS UH");
                sql.AppendLine("                   INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                       ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                       AND UM.売上日 = UH.売上日");
                sql.AppendLine("                       AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                       AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                           ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H1");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                       ) AS M12 ON");
                sql.AppendLine("                           M12.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                           AND M12.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降"
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                   AND UH.売上日 > ISNULL(M12.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                   AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("           ) AS U2");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS T12 ON U2.企業ＣＤ = T12.企業ＣＤ AND U2.得意先ＣＤ = T12.得意先ＣＤ");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("               (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S12.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S12.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S12.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S12.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S12.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S12.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           UNION");
                // -- 駆動表②－１ 入金が存在する請求先（部署合算請求フラグOFF）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               S21.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT DISTINCT");
                sql.AppendLine("                   N.企業ＣＤ");
                sql.AppendLine("                   ,N.部署ＣＤ");
                sql.AppendLine("                   ,N.得意先ＣＤ");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   入金明細 AS N");
                // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           G2.請求先ＣＤ");
                sql.AppendLine("                           ,G2.部署ＣＤ");
                sql.AppendLine("                           ,MAX(G2.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H2");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H2.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H2.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           G2.請求先ＣＤ");
                sql.AppendLine("                           ,G2.部署ＣＤ");
                sql.AppendLine("                       ) AS M21 ON");
                sql.AppendLine("                           M21.請求先ＣＤ = N.得意先ＣＤ");
                sql.AppendLine("                           AND M21.部署ＣＤ = N.部署ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   N.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                   AND N.入金日 > ISNULL(M21.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND N.入金日 <= @請求日");                  // -- 今回締日（入力された請求日）
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND N.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine(); // -- 現金の得意先に掛売したときは得意先に対する入金なので
                }
                sql.AppendLine("           ) AS N1");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = N1.企業ＣＤ AND S21.得意先ＣＤ = N1.得意先ＣＤ");   // -- N1.得意先ＣＤは請求先ＣＤ
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                sql.AppendLine("               (ISNULL(S21.部署合算請求フラグ, 0) = 0 AND N1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S21.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S21.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S21.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S21.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S21.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S21.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           UNION");
                // -- 駆動表②－２ 入金が存在する請求先（部署合算請求フラグON）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               S22.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT DISTINCT");
                sql.AppendLine("                   N.企業ＣＤ");
                sql.AppendLine("                   ,N.得意先ＣＤ");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   入金明細 AS N");
                // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日（部署合算請求フラグONのときの入金額は部署を無視する）
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           H2.請求先ＣＤ");
                sql.AppendLine("                           ,MAX(H2.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H2");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H2.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H2.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           H2.請求先ＣＤ");
                sql.AppendLine("                       ) AS M22 ON");
                sql.AppendLine("                           M22.請求先ＣＤ = N.得意先ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   N.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                   AND N.入金日 > ISNULL(M22.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND N.入金日 <= @請求日");                  // -- 今回締日（入力された請求日）
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND N.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine(); // -- 現金の得意先に掛売したときは得意先に対する入金なので
                }
                sql.AppendLine("               ) AS N2");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = N2.企業ＣＤ AND S22.得意先ＣＤ = N2.得意先ＣＤ");   // -- N2.得意先ＣＤは請求先ＣＤ
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("               (ISNULL(S22.部署合算請求フラグ, 0) <> 0 AND S22.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S22.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S22.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S22.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S22.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S22.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S22.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           UNION");
                // -- 駆動表③－１ 前回請求額が存在する請求先（部署合算請求フラグOFF）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               A1.請求先ＣＤ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT DISTINCT");
                sql.AppendLine("                   A.企業ＣＤ");
                sql.AppendLine("                   ,A.部署ＣＤ");
                sql.AppendLine("                   ,A.請求先ＣＤ");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   請求合計 AS A");
                // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   INNER JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                           ,MAX(G3.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H3");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G3 ON H3.企業ＣＤ = G3.企業ＣＤ AND H3.請求番号 = G3.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H3.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H3.請求日 < @請求日");                   // -- 今回締日（入力された請求日）

                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                           AND H3.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("                           AND H3.請求先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("                           AND H3.請求先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                       ) AS M31 ON");
                sql.AppendLine("                           M31.請求先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("                           AND M31.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("                           AND M31.前回締日 = A.請求日");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   A.企業ＣＤ = @企業ＣＤ");
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("               AND A.請求先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();     // -- 現金の得意先に掛売したときは得意先に対する請求なので
                }
                sql.AppendLine("               ) AS A1");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S31 ON S31.企業ＣＤ = A1.企業ＣＤ AND S31.得意先ＣＤ = A1.請求先ＣＤ");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っていない場合は、請求合計の部署を対象にする
                sql.AppendLine("               (ISNULL(S31.部署合算請求フラグ, 0) = 0 AND A1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S31.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S31.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S31.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S31.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S31.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S31.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           UNION");
                // -- 駆動表③－２ 前回請求額が存在する請求先（部署合算請求フラグON）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               A2.請求先ＣＤ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT DISTINCT");
                sql.AppendLine("                   A.企業ＣＤ");
                sql.AppendLine("                   ,A.請求先ＣＤ");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   請求合計 AS A");
                // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   INNER JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                           ,MAX(G3.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H3");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G3 ON H3.企業ＣＤ = G3.企業ＣＤ AND H3.請求番号 = G3.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H3.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H3.請求日 < @請求日");                   // -- 今回締日（入力された請求日）

                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                           AND H3.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                else
                {
                    // 通常の請求締めの場合は、請求先単位
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("                           AND H3.請求先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("                           AND H3.請求先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                       ) AS M32 ON");
                sql.AppendLine("                           M32.請求先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("                           AND M32.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("                           AND M32.前回締日 = A.請求日");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   A.企業ＣＤ = @企業ＣＤ");
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND A.請求先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();     // -- 現金の得意先に掛売したときは得意先に対する請求なので
                }
                sql.AppendLine("               ) AS A2");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S32 ON S32.企業ＣＤ = A2.企業ＣＤ AND S32.得意先ＣＤ = A2.請求先ＣＤ");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("               (ISNULL(S32.部署合算請求フラグ, 0) <> 0 AND S32.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S32.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S32.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S32.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S32.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S32.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S32.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           ) AS D");
                sql.AppendLine("           LEFT JOIN");
                // -- 売上（部署合算請求フラグOFF）
                sql.AppendLine("           (SELECT");
                sql.AppendLine("               S11.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("               ,SUM(U1.値引額) AS 値引額");
                sql.AppendLine("               ,SUM(U1.値引前金額) as 値引前金額");
                sql.AppendLine("               ,SUM(U1.値引後金額) as 値引後金額");
                sql.AppendLine("               ,SUM(U1.消費税額) as 消費税額");
                sql.AppendLine("               ,SUM(U1.商品金額合計税込) as 商品金額合計税込");
                sql.AppendLine("               ,1 AS 売上フラグ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT");
                sql.AppendLine("                   UH.企業ＣＤ");
                sql.AppendLine("                   ,UH.部署ＣＤ");
                sql.AppendLine("                   ,UH.得意先ＣＤ");
                sql.AppendLine("                   ,UM.値引額");
                sql.AppendLine("                   ,UM.値引前金額");
                sql.AppendLine("                   ,UM.値引後金額");
                sql.AppendLine("                   ,UM.消費税額");
                sql.AppendLine("                   ,UM.商品金額合計税込");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   売上ヘッダ AS UH");
                sql.AppendLine("                   INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                       ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                       AND UM.売上日 = UH.売上日");
                sql.AppendLine("                       AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                       AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                           ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H1");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                       ) AS M11 ON");
                sql.AppendLine("                           M11.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                           AND M11.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                   AND UH.売上日 > ISNULL(M11.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                   AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("               ) AS U1");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS T11 ON U1.企業ＣＤ = T11.企業ＣＤ AND U1.得意先ＣＤ = T11.得意先ＣＤ");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っていない場合は、売上の部署を対象にする
                sql.AppendLine("               (ISNULL(S11.部署合算請求フラグ, 0) = 0 AND U1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S11.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S11.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S11.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S11.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S11.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S11.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           GROUP BY");
                sql.AppendLine("               S11.得意先ＣＤ");
                sql.AppendLine("           UNION");
                // -- 売上（部署合算請求フラグON）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               S12.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("               ,SUM(U2.値引額) AS 値引額");
                sql.AppendLine("               ,SUM(U2.値引前金額) as 値引前金額");
                sql.AppendLine("               ,SUM(U2.値引後金額) as 値引後金額");
                sql.AppendLine("               ,SUM(U2.消費税額) as 消費税額");
                sql.AppendLine("               ,SUM(U2.商品金額合計税込) as 商品金額合計税込");
                sql.AppendLine("               ,1 AS 売上フラグ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT");
                sql.AppendLine("                   UH.企業ＣＤ");
                sql.AppendLine("                   ,UH.得意先ＣＤ");
                sql.AppendLine("                   ,UM.値引額");
                sql.AppendLine("                   ,UM.値引前金額");
                sql.AppendLine("                   ,UM.値引後金額");
                sql.AppendLine("                   ,UM.消費税額");
                sql.AppendLine("                   ,UM.商品金額合計税込");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   売上ヘッダ AS UH");
                sql.AppendLine("                   INNER JOIN 売上明細 AS UM");
                sql.AppendLine("                       ON UM.企業ＣＤ = UH.企業ＣＤ");
                sql.AppendLine("                       AND UM.売上日 = UH.売上日");
                sql.AppendLine("                       AND UM.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                       AND UM.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                           ,MAX(H1.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H1");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H1.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H1.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           H1.得意先ＣＤ");
                sql.AppendLine("                           ,H1.部署ＣＤ");
                sql.AppendLine("                       ) AS M12 ON");
                sql.AppendLine("                           M12.得意先ＣＤ = UH.得意先ＣＤ");
                sql.AppendLine("                           AND M12.部署ＣＤ = UH.部署ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   UH.企業ＣＤ = @企業ＣＤ");
                // -- 前回請求が有るものは前回請求日以降
                // -- 無ければ過去の売上全て
                // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく
                sql.AppendLine("                   AND UH.売上日 > ISNULL(M12.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND UH.売上日 <= @請求日");                  // -- 今回締日（入力された請求日）
                sql.AppendLine("                   AND UM.売上区分 = @売上区分売掛");          // -- 売掛
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND UH.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();
                }
                sql.AppendLine("               ) AS U2");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS T12 ON U2.企業ＣＤ = T12.企業ＣＤ AND U2.得意先ＣＤ = T12.得意先ＣＤ");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("               (ISNULL(S12.部署合算請求フラグ, 0) <> 0 AND S12.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S12.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S12.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S12.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S12.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S12.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S12.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           GROUP BY");
                sql.AppendLine("               S12.得意先ＣＤ");
                sql.AppendLine("           ) AS UA ON");
                sql.AppendLine("               D.請求先ＣＤ = UA.請求先ＣＤ");
                sql.AppendLine("           LEFT JOIN");
                // -- 入金（部署合算請求フラグOFF）
                sql.AppendLine("           (SELECT");
                sql.AppendLine("               S21.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("               ,SUM(N1.入金額) AS 入金額");
                sql.AppendLine("               ,1 AS 入金フラグ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT");
                sql.AppendLine("                   N.企業ＣＤ");
                sql.AppendLine("                   ,N.部署ＣＤ");
                sql.AppendLine("                   ,N.得意先ＣＤ");
                sql.AppendLine("                   ,N.入金額");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   入金明細 AS N");
                // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           G2.請求先ＣＤ");
                sql.AppendLine("                           ,G2.部署ＣＤ");
                sql.AppendLine("                           ,MAX(G2.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H2");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H2.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H2.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           G2.請求先ＣＤ");
                sql.AppendLine("                           ,G2.部署ＣＤ");
                sql.AppendLine("                       ) AS M21 ON");
                sql.AppendLine("                           M21.請求先ＣＤ = N.得意先ＣＤ");
                sql.AppendLine("                           AND M21.部署ＣＤ = N.部署ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   N.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                   AND N.入金日 > ISNULL(M21.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND N.入金日 <= @請求日");                  // -- 今回締日（入力された請求日）
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND N.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine(); // -- 現金の得意先に掛売したときは得意先に対する入金なので
                }
                sql.AppendLine("               ) AS N1");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = N1.企業ＣＤ AND S21.得意先ＣＤ = N1.得意先ＣＤ");   // -- N1.得意先ＣＤは請求先ＣＤ
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っていない場合は、入金の部署を対象にする
                sql.AppendLine("               (ISNULL(S21.部署合算請求フラグ, 0) = 0 AND N1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S21.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S21.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S21.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S21.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S21.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S21.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           GROUP BY");
                sql.AppendLine("               S21.得意先ＣＤ");
                sql.AppendLine("           UNION");
                // -- 入金（部署合算請求フラグON）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               S22.得意先ＣＤ AS 請求先ＣＤ");
                sql.AppendLine("               ,SUM(N2.入金額) AS 入金額");
                sql.AppendLine("               ,1 AS 入金フラグ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT");
                sql.AppendLine("                   N.企業ＣＤ");
                sql.AppendLine("                   ,N.得意先ＣＤ");
                sql.AppendLine("                   ,N.入金額");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   入金明細 AS N");
                // -- 請求先の入金明細が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   LEFT JOIN");
                // -- 前回締日（部署合算請求フラグONのときの入金額は部署を無視する）
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           H2.請求先ＣＤ");
                sql.AppendLine("                           ,MAX(H2.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H2");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H2.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H2.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           H2.請求先ＣＤ");
                sql.AppendLine("                       ) AS M22 ON");
                sql.AppendLine("                           M22.請求先ＣＤ = N.得意先ＣＤ");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   N.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                   AND N.入金日 > ISNULL(M22.前回締日, '2000/01/01')");
                sql.AppendLine("                   AND N.入金日 <= @請求日");                  // -- 今回締日（入力された請求日）
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND N.得意先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine(); // -- 現金の得意先に掛売したときは得意先に対する入金なので
                }
                sql.AppendLine("               ) AS N2");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = N2.企業ＣＤ AND S22.得意先ＣＤ = N2.得意先ＣＤ");   // -- N2.得意先ＣＤは請求先ＣＤ
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("               (ISNULL(S22.部署合算請求フラグ, 0) <> 0 AND S22.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S22.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S22.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S22.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S22.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S22.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S22.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           GROUP BY");
                sql.AppendLine("               S22.得意先ＣＤ");
                sql.AppendLine("           ) AS NA ON");
                sql.AppendLine("               D.請求先ＣＤ = NA.請求先ＣＤ");
                sql.AppendLine("           LEFT JOIN");
                // -- 前回請求額（部署合算請求フラグOFF）
                // -- （前回の請求が部署ごとに複数存在し、今回から部署合算請求フラグが立った場合、前回の請求を１つにまとめる。フラグが立ってない場合はもともと一つの前回請求を取得するはず）
                sql.AppendLine("           (SELECT");
                sql.AppendLine("               A1.請求先ＣＤ");
                sql.AppendLine("               ,A1.部署ＣＤ");
                sql.AppendLine("               ,MAX(A1.請求日) AS 前回締日");
                sql.AppendLine("               ,SUM(A1.今回請求額) AS 今回請求額");
                sql.AppendLine("               ,1 AS 前回請求フラグ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT");
                sql.AppendLine("                   A.企業ＣＤ");
                sql.AppendLine("                   ,A.請求先ＣＤ");
                sql.AppendLine("                   ,A.部署ＣＤ");
                sql.AppendLine("                   ,A.請求日");
                sql.AppendLine("                   ,A.今回請求額");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   請求合計 AS A");
                // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   INNER JOIN");
                // -- 前回締日");
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                           ,MAX(G3.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H3");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G3 ON H3.企業ＣＤ = G3.企業ＣＤ AND H3.請求番号 = G3.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H3.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H3.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                       ) AS M31 ON");
                sql.AppendLine("                           M31.請求先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("                           AND M31.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("                           AND M31.前回締日 = A.請求日");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   A.企業ＣＤ = @企業ＣＤ");
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND A.請求先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();     // -- 現金の得意先に掛売したときは得意先に対する請求なので
                }
                sql.AppendLine("               ) AS A1");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S31 ON S31.企業ＣＤ = A1.企業ＣＤ AND S31.得意先ＣＤ = A1.請求先ＣＤ");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っていない場合は、請求合計の部署を対象にする
                sql.AppendLine("               (ISNULL(S31.部署合算請求フラグ, 0) = 0 AND A1.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S31.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S31.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S31.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S31.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S31.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S31.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           GROUP BY");
                sql.AppendLine("               A1.請求先ＣＤ");
                sql.AppendLine("               ,A1.部署ＣＤ");
                sql.AppendLine("           UNION");
                // -- 前回請求額（部署合算請求フラグON）
                // -- （前回の請求が部署ごとに複数存在し、今回から部署合算請求フラグが立った場合、前回の請求を１つにまとめる。フラグが立ってない場合はもともと一つの前回請求を取得するはず）
                sql.AppendLine("           SELECT");
                sql.AppendLine("               A2.請求先ＣＤ");
                sql.AppendLine("               ,NULL AS 部署ＣＤ");
                sql.AppendLine("               ,MAX(A2.請求日) AS 前回締日");
                sql.AppendLine("               ,SUM(A2.今回請求額) AS 今回請求額");
                sql.AppendLine("               ,1 AS 前回請求フラグ");
                sql.AppendLine("           FROM");
                sql.AppendLine("               (SELECT");
                sql.AppendLine("                   A.企業ＣＤ");
                sql.AppendLine("                   ,A.請求先ＣＤ");
                sql.AppendLine("                   ,A.請求日");
                sql.AppendLine("                   ,A.今回請求額");
                sql.AppendLine("               FROM");
                sql.AppendLine("                   請求合計 AS A");
                // -- 請求先の請求合計が得意先ごとに抽出されるのを防ぐため、請求先から得意先を求めるための得意先マスタとの結合はしない
                sql.AppendLine("                   INNER JOIN");
                // -- 前回締日");
                sql.AppendLine("                       (SELECT");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                           ,MAX(G3.請求日) AS 前回締日");
                sql.AppendLine("                       FROM");
                sql.AppendLine("                           請求ヘッダ AS H3");
                sql.AppendLine("                           INNER JOIN 請求合計 AS G3 ON H3.企業ＣＤ = G3.企業ＣＤ AND H3.請求番号 = G3.請求番号");
                sql.AppendLine("                       WHERE");
                sql.AppendLine("                           H3.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                           AND H3.請求日 < @請求日");                   // -- 今回締日（入力された請求日）
                sql.AppendLine("                       GROUP BY");
                sql.AppendLine("                           G3.請求先ＣＤ");
                sql.AppendLine("                           ,G3.部署ＣＤ");
                sql.AppendLine("                       ) AS M32 ON");
                sql.AppendLine("                           M32.請求先ＣＤ = A.請求先ＣＤ");
                sql.AppendLine("                           AND M32.部署ＣＤ = A.部署ＣＤ");
                sql.AppendLine("                           AND M32.前回締日 = A.請求日");
                sql.AppendLine("               WHERE");
                sql.AppendLine("                   A.企業ＣＤ = @企業ＣＤ");
                // 現金の得意先に掛売したときの請求は、得意先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    sql.AppendFormat("                   AND A.請求先ＣＤ in ({0}) ", joinedTokuisakiCD).AppendLine();     // -- 現金の得意先に掛売したときは得意先に対する請求なので
                }
                sql.AppendLine("               ) AS A2");
                sql.AppendLine("               INNER JOIN 得意先マスタ AS S32 ON S32.企業ＣＤ = A2.企業ＣＤ AND S32.得意先ＣＤ = A2.請求先ＣＤ");
                sql.AppendLine("           WHERE");
                // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                sql.AppendLine("               (ISNULL(S32.部署合算請求フラグ, 0) <> 0 AND S32.部署ＣＤ = @部署ＣＤ)");
                sql.AppendLine("               AND S32.売掛現金区分 = @売掛現金区分");      // -- 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    sql.AppendLine("               AND S32.締区分 = @締区分");              // -- 現金の得意先に掛売したときの請求の場合はこの行は不要
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    sql.AppendLine("               AND S32.月締日 = @月締日");              // -- 月締めの場合
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    sql.AppendLine("               AND S32.週締曜日 = @週締曜日");          // -- 週締めの場合
                }
                // 通常の請求締めの場合は、請求先単位
                if (string.IsNullOrEmpty(joinedTokuisakiCD))
                {
                    if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("               AND S32.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("               AND S32.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                }
                sql.AppendLine("           GROUP BY");
                sql.AppendLine("               A2.請求先ＣＤ");
                sql.AppendLine("           ) AS Z ON");
                sql.AppendLine("               D.請求先ＣＤ = Z.請求先ＣＤ");
                sql.AppendLine("       ) AS B");
                sql.AppendLine("       INNER JOIN (");
                sql.AppendLine("             SELECT");
                sql.AppendLine("                 企業ＣＤ");
                sql.AppendLine("                 ,得意先ＣＤ");
                sql.AppendLine("                 ,得意先名");
                sql.AppendLine("                 ,CASE WHEN 受注得意先ＣＤ IS NULL THEN 得意先ＣＤ ELSE 受注得意先ＣＤ END AS 受注得意先ＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 得意先マスタ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             ) AS S ON S.得意先ＣＤ = B.請求先ＣＤ");
                // -- コースで絞り込む際、分配先の売上は分配元として抽出したいので、得意先ＣＤを分配元のそれにすり替える
                sql.AppendLine("       INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = S.受注得意先ＣＤ");

                if (seikyuShimeCriteria.コースＣＤ != null)
                {
                    // -- コースによる絞り込み用
                    // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                    // -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
                    sql.AppendLine("       INNER JOIN");
                    sql.AppendLine("           (SELECT DISTINCT");
                    sql.AppendLine("               CM.企業ＣＤ");
                    sql.AppendLine("               ,CM.部署ＣＤ");
                    sql.AppendLine("               ,CT.コースＣＤ");
                    sql.AppendLine("               ,CT.得意先ＣＤ");
                    sql.AppendLine("           FROM");
                    sql.AppendLine("               コースマスタ AS CM");
                    sql.AppendLine("               INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                   CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                   AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("           WHERE");
                    sql.AppendLine("               CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("           ) AS C");
                    sql.AppendLine("           ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("           AND J.得意先ＣＤ = C.得意先ＣＤ");
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.コースＣＤ;
                }

                // コースＣＤと同時に指定されることは無い
                if (seikyuShimeCriteria.コース外 == true)
                {
                    // -- コース外による絞り込み用
                    // -- 検索条件にコース外が指定された場合のみ、LEFT JOIN以下を追加すること
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             CM.企業ＣＤ");
                    sql.AppendLine("             ,CM.部署ＣＤ");
                    sql.AppendLine("             ,CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ AS CM");
                    sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                    sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                    sql.AppendLine("         ) AS C");
                    sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                    sql.AppendLine(" WHERE C.コースＣＤ IS NULL");
                }

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.部署ＣＤ;

                // 現金の得意先に掛売したときの請求の場合は0:現金
                if (string.IsNullOrEmpty(joinedTokuisakiCD) == false)
                {
                    cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                }
                else
                {
                    cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;
                }
                cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = seikyuShimeCriteria.請求日;
                cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = seikyuShimeCriteria.締区分;
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = seikyuShimeCriteria.月締日;
                }
                if (seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = seikyuShimeCriteria.週締曜日;
                }
                if (seikyuShimeCriteria.請求先ＣＤFrom != null)
                {
                    cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤFrom;
                }
                if (seikyuShimeCriteria.請求先ＣＤTo != null)
                {
                    cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = seikyuShimeCriteria.請求先ＣＤTo;
                }
                cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                cmd.CommandText = sql.ToString();
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
                MessageBox.Show(string.Format("請求合計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #請求ヘッダTempが持つ値を合計して#請求合計Tempを更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool UpdateTotalTemp(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                // #請求合計Tempの消費税と今回請求額は、#請求ヘッダTempの合計ではなく、
                // #請求ヘッダTempの売上金額の合計に対して税率を掛けて消費税を求め、今回請求額を求める必要がある。
                // 丸め処理をプログラムでやらないといけないので、SQLではできない。
                // #請求ヘッダTempから請求先ＣＤ、税区分、丸め処理区分、計算税率ごとに売上金額の合計を求め、
                // 請求先ＣＤごとの売上金額の合計と消費税額と今回請求額を保持する。
                // #請求ヘッダTempの全てのレコード（＝全ての請求先ＣＤ）について算出が完了したら、
                // #請求合計Tempに対してUpdateする。
                Dictionary<int, decimal> kingakuMap = new Dictionary<int, decimal>();   // 売上金額マップ
                Dictionary<int, decimal> taxMap = new Dictionary<int, decimal>();       // 消費税マップ
                Dictionary<int, decimal> zeikomiMap = new Dictionary<int, decimal>();   // 売上金額税込マップ

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     請求先ＣＤ" +
                                  "\r\n     ,税区分" +
                                  "\r\n     ,丸め処理区分" +
                                  "\r\n     ,計算税率" +
                                  "\r\n     ,SUM(売上金額) AS 売上金額" +
                                  "\r\n FROM" +
                                  "\r\n     #請求ヘッダTemp" +
                                  "\r\n GROUP BY" +
                                  "\r\n     請求先ＣＤ" +
                                  "\r\n     ,税区分" +
                                  "\r\n     ,丸め処理区分" +
                                  "\r\n     ,計算税率" +
                                  "\r\n ORDER BY" +
                                  "\r\n     請求先ＣＤ";

                log.Debug(cmd.CommandTextWithParameters());
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("請求ヘッダTemp");
                adp.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    int seikyusakiCD = (int)dr["請求先ＣＤ"];
                    // 売上が無い場合は税区分、丸め処理区分、計算税率はNull
                    int? zeiKubun = dr["税区分"] as int?;
                    int? hasuKubun = dr["丸め処理区分"] as int?;
                    decimal? taxRate = dr["計算税率"] as decimal?;
                    decimal kingaku = (decimal)dr["売上金額"];

                    decimal tax;
                    decimal amountIncludeTax;
                    // 消費税額、売上金額税込を計算
                    if (zeiKubun == null)
                    {
                        tax = 0m;
                        amountIncludeTax = 0m;
                    }
                    else
                    {
                        tax = Syohizei.GetTax(zeiKubun, kingaku, (decimal)taxRate, hasuKubun);
                        amountIncludeTax = Syohizei.GetAmountIncludeTax(zeiKubun, kingaku, (decimal)taxRate, hasuKubun);
                    }

                    // マップが持つ値に加算
                    if (kingakuMap.ContainsKey(seikyusakiCD) == false)
                    {
                        kingakuMap[seikyusakiCD] = 0m;
                    }
                    kingakuMap[seikyusakiCD] += kingaku;

                    if (taxMap.ContainsKey(seikyusakiCD) == false)
                    {
                        taxMap[seikyusakiCD] = 0m;
                    }
                    taxMap[seikyusakiCD] += tax;

                    if (zeikomiMap.ContainsKey(seikyusakiCD) == false)
                    {
                        zeikomiMap[seikyusakiCD] = 0m;
                    }
                    zeikomiMap[seikyusakiCD] += amountIncludeTax;
                }

                foreach (int seikyusakiCD in kingakuMap.Keys)
                {
                    decimal kingaku = kingakuMap[seikyusakiCD];
                    decimal tax = taxMap[seikyusakiCD];
                    decimal amountIncludeTax = zeikomiMap[seikyusakiCD];

                    // #請求ヘッダTempが持つ値を合計して更新
                    SqlCommand cmdUpdate = con.CreateCommand();
                    cmdUpdate.Transaction = tran;
                    cmdUpdate.CommandText = "\r\n UPDATE #請求合計Temp" +
                                            "\r\n SET 買上金額 = @売上金額" +
                                            "\r\n    ,消費税 = @消費税" +
                                            "\r\n    ,今回請求額 = 繰越額 + @売上金額税込" +
                                            "\r\n WHERE 請求先ＣＤ = @請求先ＣＤ";

                    cmdUpdate.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = seikyusakiCD;
                    cmdUpdate.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = kingaku;
                    cmdUpdate.Parameters.Add("@消費税", SqlDbType.Decimal).Value = tax;
                    cmdUpdate.Parameters.Add("@売上金額税込", SqlDbType.Decimal).Value = amountIncludeTax;

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
                MessageBox.Show(string.Format("請求合計一時テーブルの{0}に失敗しました。\n{1}", History.ProcessType.Update.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #Tempから追加するレコードを請求ヘッダから削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool DeleteHeaderByTemp(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                // -- #請求合計Tempにあるレコードと同じキー（請求日・締区分・部署ＣＤ・請求先ＣＤ）に該当する請求合計の請求番号と一致する請求ヘッダを削除
                cmd.CommandText = "\r\n DELETE H" +
                                  "\r\n FROM" +
                                  "\r\n     請求ヘッダ AS H" +
                                  "\r\n     INNER JOIN 請求合計 AS G" +
                                  "\r\n         ON H.請求番号 = G.請求番号" +
                                  "\r\n     INNER JOIN #請求合計Temp AS T" +
                                  "\r\n         ON G.請求日 = T.請求日" +
                                  "\r\n         AND G.締区分 = T.締区分" +
                                  "\r\n         AND G.部署ＣＤ = T.部署ＣＤ" +
                                  "\r\n         AND G.請求先ＣＤ = T.請求先ＣＤ";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求ヘッダの{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// #Tempから追加するレコードを請求合計から削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool DeleteTotalByTemp(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                // -- #請求合計Tempにあるレコードと同じキー（請求日・締区分・部署ＣＤ・請求先ＣＤ）に該当する請求合計を削除
                cmd.CommandText = "\r\n DELETE G" +
                                  "\r\n FROM" +
                                  "\r\n     請求合計 AS G" +
                                  "\r\n     INNER JOIN #請求合計Temp AS T" +
                                  "\r\n         ON G.請求日 = T.請求日" +
                                  "\r\n         AND G.締区分 = T.締区分" +
                                  "\r\n         AND G.部署ＣＤ = T.部署ＣＤ" +
                                  "\r\n         AND G.請求先ＣＤ = T.請求先ＣＤ";

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Delete.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 請求合計を作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool CreateTotal(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            decimal seikyuNo;
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // 現在の請求番号を取得
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT ISNULL(MAX(請求番号), 0) AS 請求番号 FROM 請求合計 WHERE 企業ＣＤ = @企業ＣＤ";
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    seikyuNo = (decimal)reader["請求番号"];
                }

                // #請求合計Tempを取得
                cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     G.請求日" +
                                  "\r\n     ,G.締区分" +
                                  "\r\n     ,G.部署ＣＤ" +
                                  "\r\n     ,G.請求先ＣＤ" +
                                  "\r\n     ,G.請求先名" +
                                  "\r\n     ,G.前回請求額" +
                                  "\r\n     ,G.入金額" +
                                  "\r\n     ,G.繰越額" +
                                  "\r\n     ,G.買上金額" +
                                  "\r\n     ,G.調整額" +
                                  "\r\n     ,G.消費税" +
                                  "\r\n     ,G.今回請求額" +
                                  "\r\n     ,ISNULL(T.支払サイト, 0) AS 支払サイト" +
                                  "\r\n     ,T.支払日" +
                                  "\r\n FROM" +
                                  "\r\n     #請求合計Temp AS G" +
                                  "\r\n     left join 得意先マスタ as T ON T.企業ＣＤ = @企業ＣＤ and T.得意先ＣＤ = G.請求先ＣＤ" +
                                  "\r\n WHERE" +
                                  // -- すべて0の場合は作成しない（ひとつでも0でなければ作成する）
                                  "\r\n     ISNULL(G.前回請求額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.入金額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.繰越額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.買上金額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.調整額, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.消費税, 0) <> 0" +
                                  "\r\n     OR ISNULL(G.今回請求額, 0) <> 0" +
                                  "\r\n ORDER BY" +
                                  "\r\n     G.請求先ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("請求合計Temp");
                adp.Fill(dt);

                // 請求合計に出力
                string sql = "\r\n INSERT INTO 請求合計(" +
                             "\r\n     企業ＣＤ" +
                             "\r\n     ,請求番号" +
                             "\r\n     ,請求日" +
                             "\r\n     ,締区分" +
                             "\r\n     ,部署ＣＤ" +
                             "\r\n     ,請求先ＣＤ" +
                             "\r\n     ,請求先名" +
                             "\r\n     ,前回請求額" +
                             "\r\n     ,入金額" +
                             "\r\n     ,繰越額" +
                             "\r\n     ,買上金額" +
                             "\r\n     ,調整額" +
                             "\r\n     ,消費税" +
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
                             "\r\n     ,@部署ＣＤ" +
                             "\r\n     ,@請求先ＣＤ" +
                             "\r\n     ,@請求先名" +
                             "\r\n     ,@前回請求額" +
                             "\r\n     ,@入金額" +
                             "\r\n     ,@繰越額" +
                             "\r\n     ,@買上金額" +
                             "\r\n     ,@調整額" +
                             "\r\n     ,@消費税" +
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

                    if (dr.IsNull("前回請求額"))
                    {
                        cmd.Parameters.Add("@前回請求額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@前回請求額", SqlDbType.Decimal).Value = dr["前回請求額"];
                    }

                    if (dr.IsNull("入金額"))
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = dr["入金額"];
                    }

                    if (dr.IsNull("繰越額"))
                    {
                        cmd.Parameters.Add("@繰越額", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@繰越額", SqlDbType.Decimal).Value = dr["繰越額"];
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

                    if (dr.IsNull("消費税"))
                    {
                        cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = dr["消費税"];
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
                    DateTime kaisyubi = GetKaisyubi((int)seikyuShimeCriteria.締区分, (DateTime)seikyuShimeCriteria.請求日, (int)dr["支払サイト"], dr["支払日"] as int?);
                    cmd.Parameters.Add("@回収予定日", SqlDbType.Date).Value = kaisyubi;

                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        MessageBox.Show("別の端末で更新済みです。再度検索してください。", "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    // 操作履歴に保存
                    History.Save(con, tran, "請求締処理", "請求合計", string.Format("請求番号=[{0}] 請求先名=[{1}]", seikyuNo, dr["請求先名"].ToString()), historyProcess);
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求合計の{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 請求ヘッダを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="seikyuShimeCriteria">請求締め条件</param>
        /// <param name="joinedTokuisakiCD">締め対象の得意先ＣＤ(カンマ区切り)</param>
        /// <returns>成功した場合は True</returns>
        private static bool CreateHeader(SqlConnection con, SqlTransaction tran, SeikyuShimeCriteria seikyuShimeCriteria, string joinedTokuisakiCD)
        {
            try
            {
                // 請求ヘッダに出力
                // #請求ヘッダTempには税率ごとに消費税額が出力されているので、ここで得意先単位に合計する
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO 請求ヘッダ (" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,請求番号" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,計算税率" +
                                  "\r\n     ,得意先名" +
                                  "\r\n     ,請求日" +
                                  "\r\n     ,請求先ＣＤ" +
                                  "\r\n     ,請求先名" +
                                  "\r\n     ,売上金額" +
                                  "\r\n     ,消費税" +
                                  "\r\n     ,売上金額税込" +
                                  "\r\n     ,作成担当者ＣＤ" +
                                  "\r\n     ,作成日" +
                                  "\r\n     ,修正担当者ＣＤ" +
                                  "\r\n     ,修正日" +
                                  "\r\n ) SELECT" +
                                  "\r\n     C.企業ＣＤ" +
                                  "\r\n     ,C.請求番号" +
                                  "\r\n     ,A.得意先ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,ISNULL(A.計算税率, 0)" +
                                  "\r\n     ,A.得意先名" +
                                  "\r\n     ,A.請求日" +
                                  "\r\n     ,A.請求先ＣＤ" +
                                  "\r\n     ,A.請求先名" +
                                  "\r\n     ,A.売上金額" +
                                  "\r\n     ,A.消費税" +
                                  "\r\n     ,A.売上金額税込" +
                                  "\r\n     ,@作成担当者ＣＤ AS 作成担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n     ,@修正担当者ＣＤ AS 修正担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n FROM" +
                                  "\r\n     (SELECT" +
                                  "\r\n         得意先ＣＤ" +
                                  "\r\n         ,部署ＣＤ" +
                                  "\r\n         ,計算税率" +
                                  "\r\n         ,得意先名" +
                                  "\r\n         ,請求日" +
                                  "\r\n         ,請求先ＣＤ" +
                                  "\r\n         ,請求先名" +
                                  "\r\n         ,SUM(売上金額) AS 売上金額" +
                                  "\r\n         ,SUM(消費税) AS 消費税" +
                                  "\r\n         ,SUM(売上金額税込) AS 売上金額税込" +
                                  "\r\n     FROM" +
                                  "\r\n         #請求ヘッダTemp" +
                                  "\r\n     GROUP BY" +
                                  "\r\n         得意先ＣＤ" +
                                  "\r\n         ,部署ＣＤ" +
                                  "\r\n         ,計算税率" +
                                  "\r\n         ,得意先名" +
                                  "\r\n         ,請求日" +
                                  "\r\n         ,請求先ＣＤ" +
                                  "\r\n         ,請求先名" +
                                  "\r\n     ) AS A" +
                                  "\r\n     INNER JOIN #請求合計Temp AS B ON" +
                                  "\r\n         A.請求先ＣＤ = B.請求先ＣＤ AND A.請求日 = B.請求日" +
                                  "\r\n     INNER JOIN 請求合計 AS C ON" +
                                  "\r\n         B.請求先ＣＤ = C.請求先ＣＤ AND B.請求日 = C.請求日 AND C.企業ＣＤ = @企業ＣＤ" +
                                  // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                  "\r\n     INNER JOIN 得意先マスタ AS S ON " +
                                  "\r\n         S.得意先ＣＤ = C.請求先ＣＤ AND ((ISNULL(S.部署合算請求フラグ, 0) = 0 AND B.部署ＣＤ = C.部署ＣＤ) OR (ISNULL(S.部署合算請求フラグ, 0) <> 0 AND S.部署ＣＤ = @部署ＣＤ))";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = seikyuShimeCriteria.部署ＣＤ;

                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                cmd.ExecuteNonQuery();

                // 操作履歴に保存
                History.Save(con, tran, "請求締処理", "請求ヘッダ", "", History.ProcessType.Create);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("請求ヘッダの{0}に失敗しました。\n{1}", History.ProcessType.Create.DisplayName(), ex.ToString()), "請求締処理", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 回収予定日を算出
        /// </summary>
        /// <param name="shimeKubn">締め区分</param>
        /// <param name="seikyubi">請求日</param>
        /// <param name="shiharaiSite">支払サイト（0:当月～3:3ヶ月）</param>
        /// <param name="shiharaibi">支払日（1～28,99は末日）</param>
        /// <returns>
        /// 月締めの場合は、請求日に対して、支払サイトの月を加算し、日を支払日にする（支払日がnullの場合は日は請求日のまま）
        /// 日締、週締の場合は請求日の翌日
        /// </returns>
        public static DateTime GetKaisyubi(int shimeKubn, DateTime seikyubi, int shiharaiSite, int? shiharaibi)
        {
            try
            {
                DateTime kaisyubi;
                // 月締め以外の場合は、請求日の翌日
                if (shimeKubn != (int)Consts.ShimeKubun.Monthly)
                {
                    kaisyubi = seikyubi.AddDays(1);
                    return kaisyubi;
                }

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

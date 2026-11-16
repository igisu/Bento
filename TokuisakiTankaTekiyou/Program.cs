using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using Emu.Bento.Data;
using log4net;

namespace Emu.TokuisakiTankaTekiyou
{
    /// <summary>
    /// 得意先単価適用バッチ
    /// </summary>
    class Program
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int RET_OK = 0;                   // 正常終了
        private const int RET_TARGET_NOTFOUND = -1;     // 処理対象のデータ無し
        private const int RET_UNKNOWN_ERROR = -2;       // 例外発生

        // 2026/11/13 適用開始日が対象日の直近かつ、単価適用日時がnullのレコードのみ取得するように変更
        /// <summary>
        /// 処理対象取得SQL
        /// </summary>
        private const string BASE_SQL = "\r\n     SELECT" +
                                        "\r\n         TA.企業ＣＤ" +
                                        "\r\n         ,TA.得意先ＣＤ" +
                                        "\r\n         ,TA.商品ＣＤ" +
                                        "\r\n         ,TA.商品単価税抜" +
                                        "\r\n         ,TA.商品単価税込" +
                                        "\r\n     FROM" +
                                        "\r\n         得意先単価適用マスタ AS TA " +
                                        // -- 企業ＣＤ、得意先ＣＤ、商品ＣＤごとに適用開始日が対象日の直近のレコードを取得する
                                        "\r\n     INNER JOIN " +
                                        "\r\n     ( " +
                                        "\r\n         SELECT " +
                                        "\r\n             企業ＣＤ " +
                                        "\r\n             ,得意先ＣＤ " +
                                        "\r\n             ,商品ＣＤ " +
                                        "\r\n             ,MAX(適用開始日) AS 適用開始日 " +
                                        "\r\n         FROM " +
                                        "\r\n             得意先単価適用マスタ " +
                                        "\r\n         WHERE " +
                                        "\r\n             企業ＣＤ = @企業ＣＤ " +
                                        "\r\n             AND 商品単価税抜 IS NOT NULL " +
                                        "\r\n             AND 商品単価税込 IS NOT NULL " +
                                        "\r\n             AND (無効フラグ = 0 OR 無効フラグ IS NULL) " +
                                        "\r\n             AND 適用開始日 <= @対象日 " +
                                        "\r\n         GROUP BY 企業ＣＤ, 得意先ＣＤ, 商品ＣＤ " +
                                        "\r\n     ) AS TAM ON TAM.企業ＣＤ = TA.企業ＣＤ AND TAM.得意先ＣＤ = TA.得意先ＣＤ AND TAM.商品ＣＤ = TA.商品ＣＤ AND TAM.適用開始日 = TA.適用開始日 " +
                                        // -- 単価適用日時がnullのレコードのみ取得する
                                        "\r\n     WHERE " +
                                        "\r\n         TA.単価適用日時 IS NULL ";

        /*
        private const string BASE_SQL = "\r\n     SELECT" +
                                        "\r\n         TA.企業ＣＤ" +
                                        "\r\n         ,TA.得意先ＣＤ" +
                                        "\r\n         ,TA.商品ＣＤ" +
                                        "\r\n         ,TA.商品単価税抜" +
                                        "\r\n         ,TA.商品単価税込" +
                                        "\r\n     FROM" +
                                        "\r\n         得意先単価適用マスタ AS TA" +
                                        "\r\n     WHERE" +
                                        "\r\n         TA.企業ＣＤ = @企業ＣＤ" +
                                        "\r\n         AND TA.商品単価税抜 IS NOT NULL" +
                                        "\r\n         AND TA.商品単価税込 IS NOT NULL" +
                                        "\r\n         AND TA.単価適用日時 IS NULL" +
                                        "\r\n         AND ISNULL(TA.無効フラグ, 0) = 0" +
                                        "\r\n         AND TA.適用開始日 = (SELECT" +
                                        "\r\n                                 MAX(TB.適用開始日)" +
                                        "\r\n                             FROM" +
                                        "\r\n                                 得意先単価適用マスタ AS TB" +
                                        "\r\n                             WHERE" +
                                        "\r\n                                 TB.企業ＣＤ = TA.企業ＣＤ" +
                                        "\r\n                                 AND TB.得意先ＣＤ = TA.得意先ＣＤ" +
                                        "\r\n                                 AND TB.商品ＣＤ = TA.商品ＣＤ" +
                                        "\r\n                                 AND TB.商品単価税抜 IS NOT NULL" +
                                        "\r\n                                 AND TB.商品単価税込 IS NOT NULL" +
                                        "\r\n                                 AND TB.単価適用日時 IS NULL" +
                                        "\r\n                                 AND ISNULL(TB.無効フラグ, 0) = 0" +
                                        "\r\n                                 AND TB.適用開始日 <= @対象日" +
                                        "\r\n                             )";
        */

        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// メイン
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            try
            {
                DateTime targetDate = DateTime.Today;                       // 対象日
                TableBase._企業ＣＤ = ConnectionInformation.CompanyCD;      // 企業ＣＤを取得

                // ログ出力用のUserDateを生成
                Tantousya tantousya = new Tantousya();
                tantousya.担当者ＣＤ = 9999;
                tantousya.担当者名 = "得意先単価適用バッチ";
                UserData.setInstance(tantousya);

                // 引数があれば日付とみなし、これを採用する
                if (args != null)
                {
                    if (args.Length > 0)
                    {
                        log.InfoFormat("引数が見つかりました。\"{0}\"", args[0]);
                        DateTime tmpDate;
                        if (DateTime.TryParse(args[0], out tmpDate))
                        {
                            targetDate = tmpDate;
                        }
                    }
                }

                log.InfoFormat("対象日付={0}", targetDate);

                // 件数を確認（無ければ終了）
                int cnt = GetCount(targetDate);
                if (cnt <= 0)
                {
                    return RET_TARGET_NOTFOUND;
                }

                // 更新処理
                if (Update(targetDate) == false)
                {
                    return RET_UNKNOWN_ERROR;
                }

                return RET_OK;
            }
            catch (BentoException)
            {
                return RET_UNKNOWN_ERROR;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                return RET_UNKNOWN_ERROR;
            }
        }

        /// <summary>
        /// 対象件数を取得
        /// </summary>
        /// <param name="targetDate">対象日付</param>
        /// <returns>処理対象になる得意先単価適用マスタの件数</returns>
        private static int GetCount(DateTime targetDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT COUNT(*) AS CNT FROM (" +
                                      BASE_SQL +
                                      "\r\n ) DMY";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@対象日", SqlDbType.Date).Value = targetDate;

                    log.Debug(cmd.CommandTextWithParameters());

                    int cnt = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        cnt = (int)reader["CNT"];
                    }

                    return cnt;
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
        /// 更新処理
        /// </summary>
        /// <param name="targetDate">対象日付</param>
        /// <returns>正常終了の場合は true。それ以外の場合は false</returns>
        private static bool Update(DateTime targetDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        // ********************
                        // 得意先単価マスタ更新
                        // ********************
                        SqlCommand cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = "\r\n WITH TAISHO AS (" +
                                          BASE_SQL +
                                          "\r\n )" +
                                          "\r\n UPDATE TANKA" +
                                          "\r\n SET" +
                                          "\r\n     TANKA.商品単価税込 = TAISHO.商品単価税込" +
                                          "\r\n     ,TANKA.商品単価税抜 = TAISHO.商品単価税抜" +
                                          "\r\n     ,TANKA.修正担当者ＣＤ = @担当者ＣＤ" +
                                          "\r\n     ,TANKA.修正日 = getdate()" +
                                          "\r\n FROM" +
                                          "\r\n     得意先単価マスタ AS TANKA" +
                                          "\r\n     INNER JOIN TAISHO ON" +
                                          "\r\n         TAISHO.企業ＣＤ = TANKA.企業ＣＤ" +
                                          "\r\n         AND TAISHO.得意先ＣＤ = TANKA.得意先ＣＤ" +
                                          "\r\n         AND TAISHO.商品ＣＤ = TANKA.商品ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     TANKA.商品単価税込 <> TAISHO.商品単価税込" +
                                          "\r\n     OR TANKA.商品単価税抜 <> TAISHO.商品単価税抜";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@対象日", SqlDbType.Date).Value = targetDate;
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;

                        log.Debug(cmd.CommandTextWithParameters());
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            History.Save(con, tran, "得意先単価適用バッチ", "得意先単価マスタ", "", History.ProcessType.Update);
                        }


                        // ********************
                        // 得意先単価マスタ追加
                        // ********************
                        cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = "\r\n WITH TAISHO AS (" +
                                          BASE_SQL +
                                          "\r\n )" +
                                          "\r\n INSERT INTO 得意先単価マスタ" +
                                          "\r\n SELECT" +
                                          "\r\n     TAISHO.企業ＣＤ" +
                                          "\r\n     ,TAISHO.得意先ＣＤ" +
                                          "\r\n     ,TAISHO.商品ＣＤ" +
                                          "\r\n     ,TAISHO.商品単価税込" +
                                          "\r\n     ,TAISHO.商品単価税抜" +
                                          "\r\n     ,0 AS 無効フラグ" +
                                          "\r\n     ,@担当者ＣＤ AS 作成担当者ＣＤ" +
                                          "\r\n     ,getdate() AS 作成日" +
                                          "\r\n     ,@担当者ＣＤ AS 修正担当者ＣＤ" +
                                          "\r\n     ,getdate() AS 修正日" +
                                          "\r\n FROM" +
                                          "\r\n     TAISHO" +
                                          "\r\n     LEFT JOIN 得意先単価マスタ TANKA ON" +
                                          "\r\n             TAISHO.企業ＣＤ = TANKA.企業ＣＤ" +
                                          "\r\n             AND TAISHO.得意先ＣＤ = TANKA.得意先ＣＤ" +
                                          "\r\n             AND TAISHO.商品ＣＤ = TANKA.商品ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     TANKA.得意先ＣＤ IS NULL";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@対象日", SqlDbType.Date).Value = targetDate;
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;

                        log.Debug(cmd.CommandTextWithParameters());
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            History.Save(con, tran, "得意先単価適用バッチ", "得意先単価マスタ", "", History.ProcessType.Create);
                        }


                        // ****************************
                        // 注文データ更新用レコード取得
                        // ****************************
                        cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = "\r\n WITH TAISHO AS (" +
                                          BASE_SQL +
                                          "\r\n )" +
                                          "\r\n SELECT" +
                                          "\r\n     M.企業ＣＤ" +
                                          "\r\n     ,M.注文区分" +
                                          "\r\n     ,M.喫食日" +
                                          "\r\n     ,M.得意先ＣＤ" +
                                          "\r\n     ,M.商品ＣＤ" +
                                          "\r\n     ,M.部署ＣＤ" +
                                          "\r\n     ,H.税区分" +
                                          "\r\n     ,H.丸め処理区分" +
                                          "\r\n     ,TAISHO.商品単価税抜" +
                                          "\r\n     ,TAISHO.商品単価税込" +
                                          "\r\n FROM" +
                                          "\r\n     注文明細 AS M" +
                                          "\r\n     INNER JOIN 注文ヘッダ AS H ON" +
                                          "\r\n         H.企業ＣＤ = M.企業ＣＤ" +
                                          "\r\n         AND H.注文区分 = M.注文区分" +
                                          "\r\n         AND H.喫食日 = M.喫食日" +
                                          "\r\n         AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         AND H.部署ＣＤ = M.部署ＣＤ" +
                                          "\r\n     INNER JOIN TAISHO ON" +
                                          "\r\n         TAISHO.企業ＣＤ = M.企業ＣＤ" +
                                          "\r\n         AND TAISHO.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         AND TAISHO.商品ＣＤ = M.商品ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     M.企業ＣＤ = TAISHO.企業ＣＤ" +
                                          "\r\n     AND M.喫食日 >= @対象日" +
                                          "\r\n     AND ((H.税区分 = @税区分内税   AND M.商品単価 <> TAISHO.商品単価税込)" +
                                          "\r\n       OR (H.税区分 = @税区分外税   AND M.商品単価 <> TAISHO.商品単価税抜)" +
                                          "\r\n       OR (H.税区分 = @税区分非課税 AND M.商品単価 <> TAISHO.商品単価税抜))";


                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@対象日", SqlDbType.Date).Value = targetDate;
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;
                        cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                        cmd.Parameters.Add("@税区分外税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Soto;
                        cmd.Parameters.Add("@税区分非課税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Hikazei;

                        log.Debug(cmd.CommandTextWithParameters());

                        SqlDataAdapter adp = new SqlDataAdapter(cmd);

                        DataTable dt = new DataTable("得意先単価適用マスタ");
                        adp.Fill(dt);

                        // 取得した注文明細をループ
                        foreach (DataRow dr in dt.Rows)
                        {
                            int chumonKbn = (int)dr["注文区分"];
                            DateTime kissyokubi  = (DateTime)dr["喫食日"];
                            int tokuisakiCD = (int)dr["得意先ＣＤ"];
                            int syohinCD = (int)dr["商品ＣＤ"];
                            int busyoCD = (int)dr["部署ＣＤ"];
                            int zeiKubun = (int)dr["税区分"];
                            int hasuKubun = (int)dr["丸め処理区分"];

                            // 注文明細取得
                            ChumonMeisai chumonMeisai = ChumonMeisai.GetChumonMeisai((Consts.ChumonKubun)chumonKbn,
                                                                                     kissyokubi,
                                                                                     tokuisakiCD,
                                                                                     syohinCD,
                                                                                     busyoCD
                                                                                     );
                            if (chumonMeisai == null)
                            {
                                chumonMeisai = new ChumonMeisai();      // ログ出力用に生成
                                chumonMeisai.注文区分 = chumonKbn;
                                chumonMeisai.喫食日 = kissyokubi;
                                chumonMeisai.得意先ＣＤ = tokuisakiCD;
                                chumonMeisai.商品ＣＤ = syohinCD;
                                chumonMeisai.部署ＣＤ = busyoCD;
                                log.WarnFormat("注文明細が見つかりません。{0}", chumonMeisai.Code);
                                continue;
                            }

                            // 商品金額や税額を算出
                            chumonMeisai.商品単価 = Syohizei.GetUnitPrice(zeiKubun, (decimal)dr["商品単価税込"], (decimal)dr["商品単価税抜"]);
                            chumonMeisai.商品金額合計 = Syohizei.GetAmount(chumonMeisai.商品数量, chumonMeisai.商品単価);
                            chumonMeisai.消費税額 = Syohizei.GetTax(zeiKubun, chumonMeisai.商品数量, chumonMeisai.商品単価, (decimal)chumonMeisai.計算税率, hasuKubun);
                            chumonMeisai.商品金額合計税込 = Syohizei.GetAmountIncludeTax(zeiKubun, chumonMeisai.商品数量, chumonMeisai.商品単価, (decimal)chumonMeisai.計算税率, hasuKubun);
                            bool ret = chumonMeisai.Update(con, tran);
                            if (ret == false)
                            {
                                return false;
                            }
                            History.Save(con, tran, "得意先単価適用バッチ", "注文明細", chumonMeisai.Code, History.ProcessType.Update);
                        }


                        // ****************************************
                        // 得意先単価適用マスタの単価適用日時を更新
                        // ****************************************
                        cmd = con.CreateCommand();
                        cmd.Transaction = tran;
                        cmd.CommandText = "\r\n UPDATE 得意先単価適用マスタ" +
                                          "\r\n SET" +
                                          "\r\n     単価適用日時 = getdate()" +
                                          "\r\n     ,修正担当者ＣＤ = @担当者ＣＤ" +
                                          "\r\n     ,修正日 = getdate()" +
                                          "\r\n FROM" +
                                          "\r\n     得意先単価適用マスタ AS TA" +
                                          "\r\n WHERE" +
                                          "\r\n     TA.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND TA.商品単価税抜 IS NOT NULL" +
                                          "\r\n     AND TA.商品単価税込 IS NOT NULL" +
                                          "\r\n     AND TA.単価適用日時 IS NULL" +
                                          "\r\n     AND ISNULL(TA.無効フラグ, 0) = 0" +
                                          // -- 得意先・商品が同一で適用開始日が異なるレコードが複数存在する場合、古い方も含めて適用済みとする
                                          "\r\n     AND TA.適用開始日 <= (SELECT" +
                                          "\r\n                             MAX(TB.適用開始日)" +
                                          "\r\n                         FROM" +
                                          "\r\n                             得意先単価適用マスタ AS TB" +
                                          "\r\n                         WHERE" +
                                          "\r\n                             TB.企業ＣＤ = TA.企業ＣＤ" +
                                          "\r\n                             AND TB.得意先ＣＤ = TA.得意先ＣＤ" +
                                          "\r\n                             AND TB.商品ＣＤ = TA.商品ＣＤ" +
                                          "\r\n                             AND TB.商品単価税抜 IS NOT NULL" +
                                          "\r\n                             AND TB.商品単価税込 IS NOT NULL" +
                                          "\r\n                             AND TB.単価適用日時 IS NULL" +
                                          "\r\n                             AND ISNULL(TB.無効フラグ, 0) = 0" +
                                          "\r\n                             AND TB.適用開始日 <= @対象日" +
                                          "\r\n                         )";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@対象日", SqlDbType.Date).Value = targetDate;
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;

                        log.Debug(cmd.CommandTextWithParameters());
                        cmd.ExecuteNonQuery();


                        tran.Commit();
                        log.Info("得意先単価適用処理を実行しました。");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        History.Error(ex);
                        return false;
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
    }
}

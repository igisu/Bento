using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 注文入力時残高表示
    /// </summary>
    public class ChumonZandaka
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChumonZandaka()
        {
            前回残高 = null;
            締後売上 = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public decimal? 前回残高 { get; private set; }
        public decimal? 締後売上 { get; private set; }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 残高を取得
        /// </summary>
        /// <param name="chumonMeisaiCriteria">注文入力検索条件</param>
        public void GetZandaka(ChumonMeisaiCriteria chumonMeisaiCriteria)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     SG.今回請求額 AS 前回残高" +
                                      "\r\n     ,SUM(UM.商品金額合計税込) AS 締後売上" +
                                      "\r\n FROM" +
                                      // -- ①得意先（と部署）に該当する請求先と最新の請求日を取得
                                      "\r\n     (SELECT" +
                                      "\r\n         T.企業ＣＤ" +
                                      "\r\n         ,T.得意先ＣＤ" +
                                      "\r\n         ,G.請求先ＣＤ" +
                                      "\r\n         ,G.部署ＣＤ" +
                                      "\r\n         ,S.部署合算請求フラグ" +
                                      "\r\n         ,MAX(G.請求日) AS 請求日" +
                                      "\r\n     FROM" +
                                      "\r\n         得意先マスタ AS T" +
                                      "\r\n         INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.得意先ＣＤ = ISNULL(T.請求先ＣＤ, T.得意先ＣＤ)" +
                                      "\r\n         INNER JOIN 請求合計 AS G ON G.企業ＣＤ = S.企業ＣＤ AND G.請求先ＣＤ = S.得意先ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND T.得意先ＣＤ = @得意先ＣＤ" +
                                      // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                      "\r\n         AND ((ISNULL(S.部署合算請求フラグ, 0) = 0 AND G.部署ＣＤ = @部署ＣＤ) OR (ISNULL(S.部署合算請求フラグ, 0) <> 0 AND G.部署ＣＤ = S.部署ＣＤ))" +
                                      "\r\n         AND G.締区分 <> @締区分現金未集金" +    // -- 未収金は除く
                                      "\r\n     GROUP BY" +
                                      "\r\n         T.企業ＣＤ" +
                                      "\r\n         ,T.得意先ＣＤ" +
                                      "\r\n         ,G.請求先ＣＤ" +
                                      "\r\n         ,G.部署ＣＤ" +
                                      "\r\n         ,S.部署合算請求フラグ" +
                                      "\r\n     ) AS A" +
                                      // -- ②請求先と最新請求日の今回請求額を取得
                                      "\r\n     LEFT JOIN 請求合計 AS SG" +
                                      "\r\n         ON SG.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n         AND SG.請求先ＣＤ = A.請求先ＣＤ" +
                                      "\r\n         AND SG.請求日 = A.請求日" +
                                      "\r\n         AND SG.部署ＣＤ = A.部署ＣＤ" +
                                      // -- ③得意先の最新請求日よりあとの売上金額の合計
                                      "\r\n     LEFT JOIN 売上ヘッダ AS UH" +
                                      "\r\n         ON UH.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n         AND UH.得意先ＣＤ = A.得意先ＣＤ" +
                                      // -- 部署合算請求フラグが立っている場合は、異なる部署の売上を得意先の部署でまとめる
                                      "\r\n         AND ((ISNULL(A.部署合算請求フラグ, 0) = 0 AND UH.部署ＣＤ = A.部署ＣＤ) OR (ISNULL(A.部署合算請求フラグ, 0) <> 0 AND UH.部署ＣＤ = UH.部署ＣＤ))" +
                                      "\r\n         AND UH.売上日 > ISNULL(A.請求日, '2000/01/01')" +
                                      "\r\n     LEFT JOIN 売上明細 AS UM" +
                                      "\r\n         ON UM.企業ＣＤ = UH.企業ＣＤ" +
                                      "\r\n         AND UM.売上日 = UH.売上日" +
                                      "\r\n         AND UM.得意先ＣＤ = UH.得意先ＣＤ" +
                                      "\r\n         AND UM.部署ＣＤ = UH.部署ＣＤ" +
                                      "\r\n         AND UM.売上区分 = @売上区分売掛" +      // -- 売掛
                                      "\r\n GROUP BY" +
                                      "\r\n     SG.今回請求額";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)chumonMeisaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)chumonMeisaiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@締区分現金未集金", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Tsuke;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            前回残高 = reader["前回残高"] as decimal?;
                            締後売上 = reader["締後売上"] as decimal?;
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
        /// クリア
        /// </summary>
        public void Clear()
        {
            前回残高 = null;
            締後売上 = null;
        }
        #endregion

    }
}

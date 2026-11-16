using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先が属するコースが存在するか（有効か）をコース区分(≒曜日)ごとに持つクラス
    /// </summary>
    /// <remarks>
    /// 曜日別固定数注文設定と曜日別固定数注文反映で使用する
    /// </remarks>
    public class YoubiChumonFlags
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
        //* プロパティ
        //**************************************
        #region プロパティ
        public int 得意先ＣＤ { get; set; }
        public bool 平日有効 { get; set; }
        public bool 土曜有効 { get; set; }
        public bool 日曜有効 { get; set; }
        public bool 祝日有効 { get; set; }
        public string 休日設定 { get; set; }
        public bool 祝日配送区分 { get; set; }

        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 得意先に該当するクラスを生成
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static YoubiChumonFlags GetYoubiChumonFlags(int tokuisakiCD)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetYoubiChumonFlags(con, null, tokuisakiCD);
            }
        }

        /// <summary>
        /// 得意先に該当するクラスを生成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static YoubiChumonFlags GetYoubiChumonFlags(SqlConnection con, SqlTransaction tran, int tokuisakiCD)
        {
            // 得意先だけを持って値は空っぽで初期化
            YoubiChumonFlags youbiChumonFlags = new YoubiChumonFlags();
            youbiChumonFlags.得意先ＣＤ = tokuisakiCD;

            string sql = "\r\n SELECT" +
                         "\r\n     A.得意先ＣＤ" +
                         "\r\n     ,WEK.有効フラグ AS 平日有効" +
                         "\r\n     ,SAT.有効フラグ AS 土曜有効" +
                         "\r\n     ,SUN.有効フラグ AS 日曜有効" +
                         "\r\n     ,HOL.有効フラグ AS 祝日有効" +
                         "\r\n     ,A.休日設定" +
                         "\r\n     ,A.祝日配送区分" +
                         "\r\n FROM" +
                         "\r\n     得意先マスタ AS A" +
                         "\r\n     LEFT JOIN" +
                         // -- 平日
                         "\r\n         (SELECT DISTINCT" +
                         "\r\n             CM.企業ＣＤ" +
                         "\r\n             ,CT.得意先ＣＤ" +
                         "\r\n             ,1 AS 有効フラグ" +
                         "\r\n         FROM" +
                         "\r\n             コースマスタ AS CM" +
                         "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                         "\r\n         WHERE" +
                         "\r\n             CM.コース区分 = @コース区分平日" +
                         "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                         "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                         "\r\n         ) AS WEK" +
                         "\r\n         ON WEK.企業ＣＤ = A.企業ＣＤ" +
                         "\r\n         AND WEK.得意先ＣＤ = A.得意先ＣＤ" +
                         "\r\n     LEFT JOIN" +
                         // -- 土曜
                         "\r\n         (SELECT DISTINCT" +
                         "\r\n             CM.企業ＣＤ" +
                         "\r\n             ,CT.得意先ＣＤ" +
                         "\r\n             ,1 AS 有効フラグ" +
                         "\r\n         FROM" +
                         "\r\n             コースマスタ AS CM" +
                         "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                         "\r\n         WHERE" +
                         "\r\n             CM.コース区分 = @コース区分土曜" +
                         "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                         "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                         "\r\n         ) AS SAT" +
                         "\r\n         ON SAT.企業ＣＤ = A.企業ＣＤ" +
                         "\r\n         AND SAT.得意先ＣＤ = A.得意先ＣＤ" +
                         "\r\n     LEFT JOIN" +
                         // -- 日曜
                         "\r\n         (SELECT DISTINCT" +
                         "\r\n             CM.企業ＣＤ" +
                         "\r\n             ,CT.得意先ＣＤ" +
                         "\r\n             ,1 AS 有効フラグ" +
                         "\r\n         FROM" +
                         "\r\n             コースマスタ AS CM" +
                         "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                         "\r\n         WHERE" +
                         "\r\n             CM.コース区分 = @コース区分日曜" +
                         "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                         "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                         "\r\n         ) AS SUN" +
                         "\r\n         ON SUN.企業ＣＤ = A.企業ＣＤ" +
                         "\r\n         AND SUN.得意先ＣＤ = A.得意先ＣＤ" +
                         "\r\n     LEFT JOIN" +
                         // -- 祝日
                         "\r\n         (SELECT DISTINCT" +
                         "\r\n             CM.企業ＣＤ" +
                         "\r\n             ,CT.得意先ＣＤ" +
                         "\r\n             ,1 AS 有効フラグ" +
                         "\r\n         FROM" +
                         "\r\n             コースマスタ AS CM" +
                         "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                         "\r\n         WHERE" +
                         "\r\n             CM.コース区分 = @コース区分祝日" +
                         "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                         "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                         "\r\n         ) AS HOL" +
                         "\r\n         ON HOL.企業ＣＤ = A.企業ＣＤ" +
                         "\r\n         AND HOL.得意先ＣＤ = A.得意先ＣＤ" +
                         "\r\n WHERE" +
                         "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                         "\r\n     AND A.得意先ＣＤ = @得意先ＣＤ";

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
            cmd.Parameters.Add("@コース区分平日", SqlDbType.Int).Value = (int)Consts.CourseKubun.Weekday;
            cmd.Parameters.Add("@コース区分土曜", SqlDbType.Int).Value = (int)Consts.CourseKubun.Saturday;
            cmd.Parameters.Add("@コース区分日曜", SqlDbType.Int).Value = (int)Consts.CourseKubun.Sunday;
            cmd.Parameters.Add("@コース区分祝日", SqlDbType.Int).Value = (int)Consts.CourseKubun.Holiday;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("曜日別注文設定マスタ");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                youbiChumonFlags = new YoubiChumonFlags(dt.Rows[0]);
            }

            return youbiChumonFlags;
        }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public YoubiChumonFlags()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public YoubiChumonFlags(YoubiChumonFlags src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            平日有効 = src.平日有効;
            土曜有効 = src.土曜有効;
            日曜有効 = src.日曜有効;
            祝日有効 = src.祝日有効;
            休日設定 = src.休日設定;
            祝日配送区分 = src.祝日配送区分;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public YoubiChumonFlags(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = (int)dr["得意先ＣＤ", version];

            if (dr.Table.Columns.Contains("平日有効"))
            {
                平日有効 = ConvertBool(dr["平日有効", version] as int?);
            }

            if (dr.Table.Columns.Contains("土曜有効"))
            {
                土曜有効 = ConvertBool(dr["土曜有効", version] as int?);
            }

            if (dr.Table.Columns.Contains("日曜有効"))
            {
                日曜有効 = ConvertBool(dr["日曜有効", version] as int?);
            }

            if (dr.Table.Columns.Contains("祝日有効"))
            {
                祝日有効 = ConvertBool(dr["祝日有効", version] as int?);
            }

            if (dr.Table.Columns.Contains("休日設定"))
            {
                休日設定 = dr["休日設定", version] as string;
            }

            if (dr.Table.Columns.Contains("祝日配送区分"))
            {
                祝日配送区分 = ConvertBool(dr["祝日配送区分", version] as int?);
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// int? から bool への変換
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool ConvertBool(int? i)
        {
            return (i == null ? false : i != 0);
        }
        #endregion
    }
}

using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 電子決済請求得意先マスタ
    /// </summary>
    /// <remarks>
    /// テーブルに対して追加・更新・削除はしないので、TableBase2は継承しない。
    /// エラーチェックも無いので、IDataErrorInfoも実装しない。
    /// </remarks>
    public class PaySeikyuTokuisaki
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
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 売上区分ＣＤに該当するマスタを返します
        /// </summary>
        /// <param name="uriageKubun">売上区分CD</param>
        /// <returns></returns>
        public static PaySeikyuTokuisaki GetPaySeikyuTokuisaki(int uriageKubun)
        {
            PaySeikyuTokuisaki paySeikyuTokuisaki = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT " +
                                  "\r\n     売上区分ＣＤ" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,締日" +
                                  "\r\n FROM" +
                                  "\r\n     電子決済請求得意先マスタ" +
                                  "\r\n WHERE " +
                                  "\r\n     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND 売上区分ＣＤ = @売上区分ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@売上区分ＣＤ", SqlDbType.Int).Value = uriageKubun;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                log.Debug(cmd.CommandTextWithParameters());

                DataTable dt = new DataTable("電子決済請求得意先");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    paySeikyuTokuisaki = new PaySeikyuTokuisaki(dt.Rows[0]);
                }
            }

            return paySeikyuTokuisaki;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _売上区分ＣＤ;
        private int? _得意先ＣＤ;
        private int? _締日;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PaySeikyuTokuisaki()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public PaySeikyuTokuisaki(PaySeikyuTokuisaki src)
        {
            売上区分ＣＤ = src.売上区分ＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
            締日 = src.締日;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public PaySeikyuTokuisaki(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            売上区分ＣＤ = dr["売上区分ＣＤ", version] as int?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            締日 = dr["締日", version] as int?;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 売上区分ＣＤ
        {
            get { return _売上区分ＣＤ; }
            set { _売上区分ＣＤ = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public int? 締日
        {
            get { return _締日; }
            set { _締日 = value; }
        }
        #endregion
    }
}

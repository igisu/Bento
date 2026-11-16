using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 出荷明細
    /// </summary>
    public class SyukkaMeisai : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 最新のレコードを返します。
        /// </summary>
        /// <returns>最新の出荷明細</returns>
        public static SyukkaMeisai GetCurrentSyukkaMeisai()
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetCurrentSyukkaMeisai(con, null);
            }
        }

        /// <summary>
        /// 最新のレコードを返します。
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>最新の出荷明細</returns>
        public static SyukkaMeisai GetCurrentSyukkaMeisai(SqlConnection con, SqlTransaction tran)
        {
            SyukkaMeisai syukkaMeisai = null;

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = "\r\n SELECT" +
                              "\r\n     A.企業ＣＤ" +
                              "\r\n     ,A.出荷番号" +
                              "\r\n     ,A.配送日" +
                              "\r\n     ,A.部署ＣＤ" +
                              "\r\n     ,A.コースＣＤ" +
                              "\r\n     ,A.コース名" +
                              "\r\n     ,A.コース順" +
                              "\r\n     ,A.得意先ＣＤ" +
                              "\r\n     ,A.得意先名" +
                              "\r\n     ,A.喫食日" +
                              "\r\n     ,A.食事区分" +
                              "\r\n     ,A.集計注文数" +
                              "\r\n     ,A.袋数表記" +
                              "\r\n     ,A.グループ表記" +
                              "\r\n     ,A.グループ表記比較用" +
                              "\r\n     ,A.個口数" +
                              "\r\n     ,A.作成担当者ＣＤ" +
                              "\r\n     ,A1.担当者名 AS 作成担当者名" +
                              "\r\n     ,A.作成日" +
                              "\r\n     ,A.修正担当者ＣＤ" +
                              "\r\n     ,A2.担当者名 AS 修正担当者名" +
                              "\r\n     ,A.修正日" +
                              "\r\n FROM" +
                              "\r\n     SP_T_出荷明細 AS A" +
                              "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                              "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                              "\r\n     AND A.出荷番号 = (SELECT MAX(出荷番号) FROM SP_T_出荷明細 WHERE 企業ＣＤ = @企業ＣＤ)";

            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable("SP_T_出荷明細");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                syukkaMeisai = new SyukkaMeisai(dt.Rows[0]);
            }

            return syukkaMeisai;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal? _出荷番号;
        private DateTime? _配送日;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private string _コース名;
        private int? _コース順;
        private int? _得意先ＣＤ;
        private string _得意先名;
        private DateTime? _喫食日;
        private int? _食事区分;
        private decimal? _集計注文数;
        private string _袋数表記;
        private string _グループ表記;
        private string _グループ表記比較用;
        private int? _個口数;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyukkaMeisai()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SyukkaMeisai(SyukkaMeisai src)
        {
            出荷番号 = src.出荷番号;
            配送日 = src.配送日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            コース名 = src.コース名;
            コース順 = src.コース順;
            得意先ＣＤ = src.得意先ＣＤ;
            得意先名 = src.得意先名;
            喫食日 = src.喫食日;
            食事区分 = src.食事区分;
            集計注文数 = src.集計注文数;
            袋数表記 = src.袋数表記;
            グループ表記 = src.グループ表記;
            グループ表記比較用 = src.グループ表記比較用;
            個口数 = src.個口数;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SyukkaMeisai(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            出荷番号 = dr["出荷番号", version] as decimal?;
            配送日 = dr["配送日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            コースＣＤ = dr["コースＣＤ", version] as int?;
            コース名 = dr["コース名", version] as string;
            コース順 = dr["コース順", version] as int?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            得意先名 = dr["得意先名", version] as string;
            喫食日 = dr["喫食日", version] as DateTime?;
            食事区分 = dr["食事区分", version] as int?;
            集計注文数 = dr["集計注文数", version] as decimal?;
            袋数表記 = dr["袋数表記", version] as string;
            グループ表記 = dr["グループ表記", version] as string;
            グループ表記比較用 = dr["グループ表記比較用", version] as string;
            個口数 = dr["個口数", version] as int?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 出荷番号
        {
            get { return _出荷番号; }
            set { _出荷番号 = value; }
        }

        public DateTime? 配送日
        {
            get { return _配送日; }
            set { _配送日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public int? コース順
        {
            get { return _コース順; }
            set { _コース順 = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public DateTime? 喫食日
        {
            get { return _喫食日; }
            set { _喫食日 = value; }
        }

        public int? 食事区分
        {
            get { return _食事区分; }
            set { _食事区分 = value; }
        }

        public decimal? 集計注文数
        {
            get { return _集計注文数; }
            set { _集計注文数 = value; }
        }

        public string 袋数表記
        {
            get { return _袋数表記; }
            set { _袋数表記 = value; }
        }

        public string グループ表記
        {
            get { return _グループ表記; }
            set { _グループ表記 = value; }
        }

        public string グループ表記比較用
        {
            get { return _グループ表記比較用; }
            set { _グループ表記比較用 = value; }
        }

        public int? 個口数
        {
            get { return _個口数; }
            set { _個口数 = value; }
        }

        public override string Code
        {
            get { return string.Format("出荷番号=[{0}]", 出荷番号); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get{ return null;}
        }

        public string this[string columnName]
        {
            get
            {
                return (_errors.ContainsKey(columnName) ? _errors[columnName] : null);
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(SyukkaMeisai target)
        {
            if (出荷番号 == target.出荷番号 &&
                配送日 == target.配送日 &&
                部署ＣＤ == target.部署ＣＤ &&
                コースＣＤ == target.コースＣＤ &&
                コース名 == target.コース名 &&
                コース順 == target.コース順 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                得意先名 == target.得意先名 &&
                喫食日 == target.喫食日 &&
                食事区分 == target.食事区分 &&
                集計注文数 == target.集計注文数 &&
                袋数表記 == target.袋数表記 &&
                グループ表記 == target.グループ表記 &&
                グループ表記比較用 == target.グループ表記比較用 &&
                個口数 == target.個口数)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO SP_T_出荷明細(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,配送日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,コース名" +
                         "\r\n     ,コース順" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,得意先名" +
                         "\r\n     ,喫食日" +
                         "\r\n     ,食事区分" +
                         "\r\n     ,集計注文数" +
                         "\r\n     ,袋数表記" +
                         "\r\n     ,グループ表記" +
                         "\r\n     ,グループ表記比較用" +
                         "\r\n     ,個口数" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@配送日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@コース名" +
                         "\r\n     ,@コース順" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@得意先名" +
                         "\r\n     ,@喫食日" +
                         "\r\n     ,@食事区分" +
                         "\r\n     ,@集計注文数" +
                         "\r\n     ,@袋数表記" +
                         "\r\n     ,@グループ表記" +
                         "\r\n     ,@グループ表記比較用" +
                         "\r\n     ,@個口数" +
                         "\r\n     ,@作成担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n     ,@修正担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n )";

            return Save(con, tran, sql, History.ProcessType.Create);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Update(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n UPDATE SP_T_出荷明細" +
                         "\r\n SET 配送日             = @配送日" +
                         "\r\n    ,部署ＣＤ           = @部署ＣＤ" +
                         "\r\n    ,コースＣＤ         = @コースＣＤ" +
                         "\r\n    ,コース名           = @コース名" +
                         "\r\n    ,コース順           = @コース順" +
                         "\r\n    ,得意先ＣＤ         = @得意先ＣＤ" +
                         "\r\n    ,得意先名           = @得意先名" +
                         "\r\n    ,喫食日             = @喫食日" +
                         "\r\n    ,食事区分           = @食事区分" +
                         "\r\n    ,集計注文数         = @集計注文数" +
                         "\r\n    ,袋数表記           = @袋数表記" +
                         "\r\n    ,グループ表記       = @グループ表記" +
                         "\r\n    ,グループ表記比較用 = @グループ表記比較用" +
                         "\r\n    ,個口数             = @個口数" +
                         "\r\n    ,修正担当者ＣＤ     = @修正担当者ＣＤ" +
                         "\r\n    ,修正日             = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 出荷番号 = @出荷番号" +
                         "\r\n   AND 修正日   = @修正日";

            return Save(con, tran, sql, History.ProcessType.Update);
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n DELETE FROM SP_T_出荷明細" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 出荷番号 = @出荷番号" +
                         "\r\n   AND 修正日   = @修正日";

            return Save(con, tran, sql, History.ProcessType.Delete);
        }

        /// <summary>
        /// DB更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        protected override bool Save(SqlConnection con, SqlTransaction tran, string sql, History.ProcessType historyProcess)
        {
            try
            {
                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (出荷番号 != null)
                    {
                        cmd.Parameters.Add("@出荷番号", SqlDbType.Decimal).Value = 出荷番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@出荷番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (配送日 != null)
                    {
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = 配送日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コース名 != null)
                    {
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = コース名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (コース順 != null)
                    {
                        cmd.Parameters.Add("@コース順", SqlDbType.Int).Value = コース順;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース順", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先名 != null)
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = 得意先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (喫食日 != null)
                    {
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = 喫食日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (食事区分 != null)
                    {
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = 食事区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (集計注文数 != null)
                    {
                        cmd.Parameters.Add("@集計注文数", SqlDbType.Decimal).Value = 集計注文数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計注文数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (袋数表記 != null)
                    {
                        cmd.Parameters.Add("@袋数表記", SqlDbType.VarChar).Value = 袋数表記;
                    }
                    else
                    {
                        cmd.Parameters.Add("@袋数表記", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (グループ表記 != null)
                    {
                        cmd.Parameters.Add("@グループ表記", SqlDbType.VarChar).Value = グループ表記;
                    }
                    else
                    {
                        cmd.Parameters.Add("@グループ表記", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (グループ表記比較用 != null)
                    {
                        cmd.Parameters.Add("@グループ表記比較用", SqlDbType.VarChar).Value = グループ表記比較用;
                    }
                    else
                    {
                        cmd.Parameters.Add("@グループ表記比較用", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (個口数 != null)
                    {
                        cmd.Parameters.Add("@個口数", SqlDbType.Int).Value = 個口数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@個口数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    // 更新の場合のみ編集日時を設定（新規の場合は設定しない）
                    if (修正日 != null)
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = 修正日;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("出荷明細を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "出荷番号が重複しています。";
                    }
                    else
                    {
                        History.Error(ex);
                        Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    }
                    return false;

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    History.Error(ex);
                    Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    return false;
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

        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先単価適用マスタ
    /// </summary>
    public class TokuisakiTankaTekiyou : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="tekiyoKaishiBi">適用開始日</param>
        /// <returns></returns>
        public static TokuisakiTankaTekiyou GetTokuisakiTankaTekiyou(int tokuisakiCD, int syohinCD, DateTime tekiyoKaishiBi)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetTokuisakiTankaTekiyou(con, null, tokuisakiCD, syohinCD, tekiyoKaishiBi, true);
            }
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="tekiyoKaishiBi">適用開始日</param>
        /// <returns></returns>
        public static TokuisakiTankaTekiyou GetTokuisakiTankaTekiyou(SqlConnection con, SqlTransaction tran, int tokuisakiCD, int syohinCD, DateTime tekiyoKaishiBi)
        {
            return GetTokuisakiTankaTekiyou(con, tran, tokuisakiCD, syohinCD, tekiyoKaishiBi, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="tekiyoKaishiBi">適用開始日</param>
        /// <returns></returns>
        public static TokuisakiTankaTekiyou GetTokuisakiTankaTekiyouReload(int tokuisakiCD, int syohinCD, DateTime tekiyoKaishiBi)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetTokuisakiTankaTekiyou(con, null, tokuisakiCD, syohinCD, tekiyoKaishiBi, false);
            }
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="tekiyoKaishiBi">適用開始日</param>
        /// <returns></returns>
        public static TokuisakiTankaTekiyou GetTokuisakiTankaTekiyouReload(SqlConnection con, SqlTransaction tran, int tokuisakiCD, int syohinCD, DateTime tekiyoKaishiBi)
        {
            return GetTokuisakiTankaTekiyou(con, tran, tokuisakiCD, syohinCD, tekiyoKaishiBi, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="tekiyoKaishiBi">適用開始日</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static TokuisakiTankaTekiyou GetTokuisakiTankaTekiyou(SqlConnection con, SqlTransaction tran, int tokuisakiCD, int syohinCD, DateTime tekiyoKaishiBi, bool isValid)
        {
            TokuisakiTankaTekiyou tokuisakiTankaTekiyou = null;

            string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.得意先ＣＤ" +
                         "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                         "\r\n     ,T.得意先名" +
                         "\r\n     ,A.商品ＣＤ" +
                         "\r\n     ,A.商品ＣＤ AS 商品ＣＤorg" +
                         "\r\n     ,S.商品名" +
                         "\r\n     ,A.適用開始日" +
                         "\r\n     ,A.適用開始日 AS 適用開始日org" +
                         "\r\n     ,A.商品単価税込" +
                         "\r\n     ,A.商品単価税抜" +
                         "\r\n     ,A.固定数" +
                         "\r\n     ,A.単価適用日時" +
                         "\r\n     ,A.最終注文日時" +
                         "\r\n     ,A.無効フラグ" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     得意先単価適用マスタ AS A" +
                         "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ" +
                         "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n WHERE A.企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND A.得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND A.商品ＣＤ   = @商品ＣＤ" +
                         "\r\n   AND A.適用開始日 = @適用開始日" +
                         "{0}";

            string where = "";
            if (isValid)
            {
                where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
            }

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = string.Format(sql, where);
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
            cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCD;
            cmd.Parameters.Add("@適用開始日", SqlDbType.Date).Value = tekiyoKaishiBi;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("得意先単価適用マスタ");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                tokuisakiTankaTekiyou = new TokuisakiTankaTekiyou(dt.Rows[0]);
            }

            return tokuisakiTankaTekiyou;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _得意先ＣＤ;
        private int? _得意先ＣＤorg;            // レコード読込直後の値（更新・削除時に使用する）
        private int? _商品ＣＤ;
        private int? _商品ＣＤorg;              // レコード読込直後の値（更新・削除時に使用する）
        private DateTime? _適用開始日;
        private DateTime? _適用開始日org;       // レコード読込直後の値（更新・削除時に使用する）
        private decimal? _商品単価税抜;
        private decimal? _商品単価税込;
        private decimal? _固定数;
        private DateTime? _単価適用日時;
        private DateTime? _最終注文日時;
        private bool? _無効フラグ;

        private string _得意先名;
        private string _商品名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiTankaTekiyou()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TokuisakiTankaTekiyou(TokuisakiTankaTekiyou src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            得意先ＣＤorg = src.得意先ＣＤorg;
            商品ＣＤ = src.商品ＣＤ;
            商品ＣＤorg = src.商品ＣＤorg;
            適用開始日 = src.適用開始日;
            適用開始日org = src.適用開始日org;
            商品単価税抜 = src.商品単価税抜;
            商品単価税込 = src.商品単価税込;
            固定数 = src.固定数;
            単価適用日時 = src.単価適用日時;
            最終注文日時 = src.最終注文日時;
            無効フラグ = src.無効フラグ;

            得意先名 = src.得意先名;
            商品名 = src.商品名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TokuisakiTankaTekiyou(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("得意先ＣＤorg"))
            {
                得意先ＣＤorg = dr["得意先ＣＤorg", version] as int?;
            }
            else
            {
                得意先ＣＤorg = dr["得意先ＣＤ", version] as int?;
            }

            商品ＣＤ = dr["商品ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("商品ＣＤorg"))
            {
                商品ＣＤorg = dr["商品ＣＤorg", version] as int?;
            }
            else
            {
                商品ＣＤorg = dr["商品ＣＤ", version] as int?;
            }

            適用開始日 = dr["適用開始日", version] as DateTime?;

            if (dr.Table.Columns.Contains("適用開始日org"))
            {
                適用開始日org = dr["適用開始日org", version] as DateTime?;
            }
            else
            {
                適用開始日org = dr["適用開始日", version] as DateTime?;
            }

            商品単価税抜 = dr["商品単価税抜", version] as decimal?;
            商品単価税込 = dr["商品単価税込", version] as decimal?;
            固定数 = dr["固定数", version] as decimal?;
            単価適用日時 = dr["単価適用日時", version] as DateTime?;
            最終注文日時 = dr["最終注文日時", version] as DateTime?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("得意先名"))
            {
                得意先名 = dr["得意先名", version] as string;
            }
            if (dr.Table.Columns.Contains("商品名"))
            {
                商品名 = dr["商品名", version] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
                }
            }
        }

        public int? 得意先ＣＤorg
        {
            get { return _得意先ＣＤorg; }
            set { _得意先ＣＤorg = value; }
        }

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set
            {
                _商品ＣＤ = value;
                _errors.Remove("商品ＣＤ");
                if (value == null)
                {
                    _errors["商品ＣＤ"] = "商品ＣＤを入力してください。";
                }
            }
        }

        public int? 商品ＣＤorg
        {
            get { return _商品ＣＤorg; }
            set { _商品ＣＤorg = value; }
        }

        public DateTime? 適用開始日
        {
            get { return _適用開始日; }
            set
            {
                _適用開始日 = value;
                _errors.Remove("適用開始日");
                if (value == null)
                {
                    _errors["適用開始日"] = "適用開始日を入力してください。";
                }
            }
        }

        public DateTime? 適用開始日org
        {
            get { return _適用開始日org; }
            set { _適用開始日org = value; }
        }

        public decimal? 商品単価税抜
        {
            get { return _商品単価税抜; }
            set { _商品単価税抜 = value; }
        }

        public decimal? 商品単価税込
        {
            get { return _商品単価税込; }
            set { _商品単価税込 = value; }
        }

        public decimal? 固定数
        {
            get { return _固定数; }
            set { _固定数 = value; }
        }

        public DateTime? 単価適用日時
        {
            get { return _単価適用日時; }
            set { _単価適用日時 = value; }
        }

        public DateTime? 最終注文日時
        {
            get { return _最終注文日時; }
            set { _最終注文日時 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public string 商品名
        {
            get { return _商品名; }
            set { _商品名 = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}] 商品ＣＤ=[{1}] 適用開始日=[{2}]",
                                        得意先ＣＤ,
                                        (商品ＣＤ == 商品ＣＤorg ? 商品ＣＤ : 商品ＣＤorg ?? 商品ＣＤ),
                                        (適用開始日 == 適用開始日org ? 適用開始日 : 適用開始日org ?? 適用開始日)
                                        ); }
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
        public bool Equals(TokuisakiTankaTekiyou target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                適用開始日 == target.適用開始日 &&
                商品単価税抜 == target.商品単価税抜 &&
                商品単価税込 == target.商品単価税込 &&
                固定数 == target.固定数 &&
                単価適用日時 == target.単価適用日時 &&
                最終注文日時 == target.最終注文日時 &&
                (無効フラグ ?? false) == (target.無効フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 得意先単価適用マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,適用開始日" +
                         "\r\n     ,商品単価税抜" +
                         "\r\n     ,商品単価税込" +
                         "\r\n     ,固定数" +
                         "\r\n     ,単価適用日時" +
                         "\r\n     ,最終注文日時" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@適用開始日" +
                         "\r\n     ,@商品単価税抜" +
                         "\r\n     ,@商品単価税込" +
                         "\r\n     ,@固定数" +
                         "\r\n     ,@単価適用日時" +
                         "\r\n     ,@最終注文日時" +
                         "\r\n     ,@無効フラグ" +
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
            string sql = "\r\n UPDATE 得意先単価適用マスタ" +
                         "\r\n SET 得意先ＣＤ     = @得意先ＣＤ" +
                         "\r\n    ,商品ＣＤ       = @商品ＣＤ" +
                         "\r\n    ,適用開始日     = @適用開始日" +
                         "\r\n    ,商品単価税抜   = @商品単価税抜" +
                         "\r\n    ,商品単価税込   = @商品単価税込" +
                         "\r\n    ,固定数         = @固定数" +
                         "\r\n    ,単価適用日時   = @単価適用日時" +
                         "\r\n    ,最終注文日時   = @最終注文日時" +
                         "\r\n    ,無効フラグ     = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤorg" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
                         "\r\n   AND 適用開始日 = @適用開始日org" +
                         "\r\n   AND 修正日     = @修正日";

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
            string sql = "\r\n DELETE FROM 得意先単価適用マスタ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤorg" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
                         "\r\n   AND 適用開始日 = @適用開始日org" +
                         "\r\n   AND 修正日     = @修正日";

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

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = 得意先ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤorg", SqlDbType.Int).Value = 商品ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (適用開始日 != null)
                    {
                        cmd.Parameters.Add("@適用開始日", SqlDbType.Date).Value = 適用開始日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@適用開始日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (適用開始日org != null)
                    {
                        cmd.Parameters.Add("@適用開始日org", SqlDbType.Date).Value = 適用開始日org;
                    }
                    else
                    {
                        cmd.Parameters.Add("@適用開始日org", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (商品単価税抜 != null)
                    {
                        cmd.Parameters.Add("@商品単価税抜", SqlDbType.Decimal).Value = 商品単価税抜;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価税抜", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (商品単価税込 != null)
                    {
                        cmd.Parameters.Add("@商品単価税込", SqlDbType.Decimal).Value = 商品単価税込;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価税込", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (固定数 != null)
                    {
                        cmd.Parameters.Add("@固定数", SqlDbType.Decimal).Value = 固定数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@固定数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (単価適用日時 != null)
                    {
                        cmd.Parameters.Add("@単価適用日時", SqlDbType.DateTime).Value = 単価適用日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@単価適用日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (最終注文日時 != null)
                    {
                        cmd.Parameters.Add("@最終注文日時", SqlDbType.DateTime).Value = 最終注文日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@最終注文日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("得意先単価適用マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "得意先ＣＤ、商品ＣＤ、適用開始日が重複しています。";
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

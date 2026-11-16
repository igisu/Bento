using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 担当者マスタ
    /// </summary>
    public class Tantousya : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="tancd">担当者ＣＤ</param>
        /// <returns></returns>
        public static Tantousya GetTantousya(int tancd)
        {
            return GetTantousya(tancd, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="tancd">担当者ＣＤ</param>
        /// <returns></returns>
        public static Tantousya GetTantousyaReload(int tancd)
        {
            return GetTantousya(tancd, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="tancd">担当者ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static Tantousya GetTantousya(int tancd, bool isValid)
        {
            Tantousya tantousya = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.担当者ＣＤ" +
                             "\r\n     ,A.担当者名" +
                             "\r\n     ,A.担当者名カナ" +
                             "\r\n     ,A.郵便番号" +
                             "\r\n     ,A.住所" +
                             "\r\n     ,A.電話番号" +
                             "\r\n     ,A.ＦＡＸ" +
                             "\r\n     ,A.所属部署ＣＤ" +
                             "\r\n     ,A.ユーザーＩＤ" +
                             "\r\n     ,A.ユーザーＰＷ" +
                             "\r\n     ,A.権限区分" +
                             "\r\n     ,A.営業業務区分" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n     ,B.支店ＣＤ" +
                             "\r\n FROM" +
                             "\r\n     担当者マスタ AS A" +
                             "\r\n     LEFT JOIN 部署マスタ   AS  B ON  B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.所属部署ＣＤ = B.部署ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.担当者ＣＤ = @担当者ＣＤ" +
                             "{0}";

                string where = "";
                if (isValid)
                {
                    where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
                }

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(sql, where);
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = tancd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("担当者マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    tantousya = new Tantousya(dt.Rows[0]);
                }
            }

            return tantousya;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _担当者ＣＤ;
        private string _担当者名;
        private string _担当者名カナ;
        private string _郵便番号;
        private string _住所;
        private string _電話番号;
        private string _ＦＡＸ;
        private int? _所属部署ＣＤ;
        private string _ユーザーＩＤ;
        private string _ユーザーＰＷ;
        private int? _権限区分;
        private int? _営業業務区分;
        private bool? _無効フラグ;

        private int? _支店ＣＤ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Tantousya()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Tantousya(Tantousya src)
        {
            担当者ＣＤ = src.担当者ＣＤ;
            担当者名 = src.担当者名;
            担当者名カナ = src.担当者名カナ;
            郵便番号 = src.郵便番号;
            住所 = src.住所;
            電話番号 = src.電話番号;
            ＦＡＸ = src.ＦＡＸ;
            所属部署ＣＤ = src.所属部署ＣＤ;
            ユーザーＩＤ = src.ユーザーＩＤ;
            ユーザーＰＷ = src.ユーザーＰＷ;
            権限区分 = src.権限区分;
            営業業務区分 = src.営業業務区分;
            無効フラグ = src.無効フラグ;

            支店ＣＤ = src.支店ＣＤ;
            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Tantousya(DataRow dr)
        {
            担当者ＣＤ = dr["担当者ＣＤ"] as int?;
            担当者名 = dr["担当者名"] as string;
            担当者名カナ = dr["担当者名カナ"] as string;
            郵便番号 = dr["郵便番号"] as string;
            住所 = dr["住所"] as string;
            電話番号 = dr["電話番号"] as string;
            ＦＡＸ = dr["ＦＡＸ"] as string;
            所属部署ＣＤ = dr["所属部署ＣＤ"] as int?;
            ユーザーＩＤ = dr["ユーザーＩＤ"] as string;
            ユーザーＰＷ = dr["ユーザーＰＷ"] as string;
            権限区分 = dr["権限区分"] as int?;
            営業業務区分 = dr["営業業務区分"] as int?;
            無効フラグ = dr["無効フラグ"] as bool?;

            if (dr.Table.Columns.Contains("支店ＣＤ"))
            {
                支店ＣＤ = dr["支店ＣＤ"] as int?;
            }

            base.FromDataRow(dr);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set
            {
                _担当者ＣＤ = value;
                _errors.Remove("担当者ＣＤ");
                if (value == null)
                {
                    _errors["担当者ＣＤ"] = "担当者ＣＤを入力してください。";
                }
            }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set
            {
                _担当者名 = value;
                _errors.Remove("担当者名");
                if (value == null)
                {
                    _errors["担当者名"] = "担当者名を入力してください。";
                }
            }
        }

        public string 担当者名カナ
        {
            get { return _担当者名カナ; }
            set { _担当者名カナ = value; }
        }

        public string 郵便番号
        {
            get { return _郵便番号; }
            set { _郵便番号 = value; }
        }

        public string 住所
        {
            get { return _住所; }
            set { _住所 = value; }
        }

        public string 電話番号
        {
            get { return _電話番号; }
            set { _電話番号 = value; }
        }

        public string ＦＡＸ
        {
            get { return _ＦＡＸ; }
            set { _ＦＡＸ = value; }
        }

        public int? 所属部署ＣＤ
        {
            get { return _所属部署ＣＤ; }
            set
            {
                _所属部署ＣＤ = value;
                _errors.Remove("所属部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["所属部署ＣＤ"] = "所属部署ＣＤを入力してください。";
                }
            }
        }

        public string ユーザーＩＤ
        {
            get { return _ユーザーＩＤ; }
            set
            {
                _ユーザーＩＤ = value;
                _errors.Remove("ユーザーＩＤ");
                if (value == null)
                {
                    _errors["ユーザーＩＤ"] = "ユーザーＩＤを入力してください。";
                }
            }
        }

        public string ユーザーＰＷ
        {
            get { return _ユーザーＰＷ; }
            set { _ユーザーＰＷ = value; }
        }

        public int? 権限区分
        {
            get { return _権限区分; }
            set { _権限区分 = value; }
        }

        public int? 営業業務区分
        {
            get { return _営業業務区分; }
            set { _営業業務区分 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public override string Code
        {
            get { return string.Format("担当者ＣＤ=[{0}]", 担当者ＣＤ); }
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
        public bool Equals(Tantousya target)
        {
            if (担当者ＣＤ == target.担当者ＣＤ &&
                担当者名 == target.担当者名 &&
                担当者名カナ == target.担当者名カナ &&
                郵便番号 == target.郵便番号 &&
                住所 == target.住所 &&
                電話番号 == target.電話番号 &&
                ＦＡＸ == target.ＦＡＸ &&
                (所属部署ＣＤ ?? -1) == (target.所属部署ＣＤ ?? -1) &&
                ユーザーＩＤ == target.ユーザーＩＤ &&
                ユーザーＰＷ == target.ユーザーＰＷ &&
                (権限区分 ?? -1) == (target.権限区分 ?? -1) &&
                (営業業務区分 ?? -1) == (target.営業業務区分 ?? -1) &&
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
            string sql = "\r\n INSERT INTO 担当者マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,担当者ＣＤ" +
                         "\r\n     ,担当者名" +
                         "\r\n     ,担当者名カナ" +
                         "\r\n     ,郵便番号" +
                         "\r\n     ,住所" +
                         "\r\n     ,電話番号" +
                         "\r\n     ,ＦＡＸ" +
                         "\r\n     ,所属部署ＣＤ" +
                         "\r\n     ,ユーザーＩＤ" +
                         "\r\n     ,ユーザーＰＷ" +
                         "\r\n     ,権限区分" +
                         "\r\n     ,営業業務区分" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@担当者ＣＤ" +
                         "\r\n     ,@担当者名" +
                         "\r\n     ,@担当者名カナ" +
                         "\r\n     ,@郵便番号" +
                         "\r\n     ,@住所" +
                         "\r\n     ,@電話番号" +
                         "\r\n     ,@ＦＡＸ" +
                         "\r\n     ,@所属部署ＣＤ" +
                         "\r\n     ,@ユーザーＩＤ" +
                         "\r\n     ,@ユーザーＰＷ" +
                         "\r\n     ,@権限区分" +
                         "\r\n     ,@営業業務区分" +
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
            string sql = "\r\n UPDATE 担当者マスタ" +
                         "\r\n SET 担当者名       = @担当者名" +
                         "\r\n    ,担当者名カナ   = @担当者名カナ" +
                         "\r\n    ,郵便番号       = @郵便番号" +
                         "\r\n    ,住所           = @住所" +
                         "\r\n    ,電話番号       = @電話番号" +
                         "\r\n    ,ＦＡＸ         = @ＦＡＸ" +
                         "\r\n    ,所属部署ＣＤ   = @所属部署ＣＤ" +
                         "\r\n    ,ユーザーＩＤ   = @ユーザーＩＤ" +
                         "\r\n    ,ユーザーＰＷ   = @ユーザーＰＷ" +
                         "\r\n    ,権限区分       = @権限区分" +
                         "\r\n    ,営業業務区分   = @営業業務区分" +
                         "\r\n    ,無効フラグ     = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getDate()" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND 担当者ＣＤ   = @担当者ＣＤ" +
                         "\r\n   AND 修正日       = @修正日";

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
            string sql = "\r\n DELETE FROM 担当者マスタ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 担当者ＣＤ = @担当者ＣＤ" +
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

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (担当者名 != null)
                    {
                        cmd.Parameters.Add("@担当者名", SqlDbType.VarChar).Value = 担当者名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (担当者名カナ != null)
                    {
                        cmd.Parameters.Add("@担当者名カナ", SqlDbType.VarChar).Value = 担当者名カナ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者名カナ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (郵便番号 != null)
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = 郵便番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (住所 != null)
                    {
                        cmd.Parameters.Add("@住所", SqlDbType.VarChar).Value = 住所;
                    }
                    else
                    {
                        cmd.Parameters.Add("@住所", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (ＦＡＸ != null)
                    {
                        cmd.Parameters.Add("@ＦＡＸ", SqlDbType.VarChar).Value = ＦＡＸ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＦＡＸ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (所属部署ＣＤ != null && 所属部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@所属部署ＣＤ", SqlDbType.Int).Value = 所属部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@所属部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (ユーザーＩＤ != null)
                    {
                        cmd.Parameters.Add("@ユーザーＩＤ", SqlDbType.VarChar).Value = ユーザーＩＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ユーザーＩＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (ユーザーＰＷ != null)
                    {
                        cmd.Parameters.Add("@ユーザーＰＷ", SqlDbType.VarChar).Value = ユーザーＰＷ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ユーザーＰＷ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (権限区分 != null && 権限区分 != -1)
                    {
                        cmd.Parameters.Add("@権限区分", SqlDbType.Int).Value = 権限区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@権限区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (営業業務区分 != null && 営業業務区分 != -1)
                    {
                        cmd.Parameters.Add("@営業業務区分", SqlDbType.Int).Value = 営業業務区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@営業業務区分", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("担当者マスタを{0}しました。", historyProcess.DisplayName());
                    return true;
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "担当者ＣＤが重複しています。";
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

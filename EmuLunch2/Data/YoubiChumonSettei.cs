using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 曜日別注文設定マスタ
    /// </summary>
    public class YoubiChumonSettei : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <returns></returns>
        public static YoubiChumonSettei GetYoubiChumonSettei(int tokuisakiCD, int syohinCD)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetYoubiChumonSettei(con, null, tokuisakiCD, syohinCD);
            }
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <returns></returns>
        public static YoubiChumonSettei GetYoubiChumonSettei(SqlConnection con, SqlTransaction tran, int tokuisakiCD, int syohinCD)
        {
            YoubiChumonSettei youbiChumonSettei = null;

            string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.得意先ＣＤ" +
                         "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                         "\r\n     ,T.得意先名" +
                         "\r\n     ,A.商品ＣＤ" +
                         "\r\n     ,A.商品ＣＤ AS 商品ＣＤorg" +
                         "\r\n     ,S.商品名" +
                         "\r\n     ,ISNULL(K.商品単価税抜, S.商品単価税抜) AS 商品単価税抜" +
                         "\r\n     ,ISNULL(K.商品単価税込, S.商品単価税込) AS 商品単価税込" +
                         "\r\n     ,S.軽減税率フラグ" +
                         "\r\n     ,A.注文数_日" +
                         "\r\n     ,A.注文数_月" +
                         "\r\n     ,A.注文数_火" +
                         "\r\n     ,A.注文数_水" +
                         "\r\n     ,A.注文数_木" +
                         "\r\n     ,A.注文数_金" +
                         "\r\n     ,A.注文数_土" +
                         "\r\n     ,A.注文数_祝" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     曜日別注文設定マスタ AS A" +
                         "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ" +
                         "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                         "\r\n     LEFT JOIN 得意先単価マスタ AS K ON K.企業ＣＤ = A.企業ＣＤ AND K.得意先ＣＤ = A.得意先ＣＤ AND K.商品ＣＤ = A.商品ＣＤ AND ISNULL(K.無効フラグ, 0) = 0" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n WHERE A.企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND A.得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND A.商品ＣＤ   = @商品ＣＤ";

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
            cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCD;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("曜日別注文設定マスタ");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                youbiChumonSettei = new YoubiChumonSettei(dt.Rows[0]);
            }

            return youbiChumonSettei;
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
        private decimal? _注文数_日;
        private decimal? _注文数_月;
        private decimal? _注文数_火;
        private decimal? _注文数_水;
        private decimal? _注文数_木;
        private decimal? _注文数_金;
        private decimal? _注文数_土;
        private decimal? _注文数_祝;

        private string _得意先名;
        private string _商品名;
        private decimal? _商品単価税抜;
        private decimal? _商品単価税込;
        private bool? _軽減税率フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public YoubiChumonSettei()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public YoubiChumonSettei(YoubiChumonSettei src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            注文数_日 = src.注文数_日;
            注文数_月 = src.注文数_月;
            注文数_火 = src.注文数_火;
            注文数_水 = src.注文数_水;
            注文数_木 = src.注文数_木;
            注文数_金 = src.注文数_金;
            注文数_土 = src.注文数_土;
            注文数_祝 = src.注文数_祝;

            得意先名 = src.得意先名;
            商品名 = src.商品名;
            商品単価税抜 = src.商品単価税抜;
            商品単価税込 = src.商品単価税込;
            軽減税率フラグ = src.軽減税率フラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public YoubiChumonSettei(DataRow dr)
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

            注文数_日 = dr["注文数_日", version] as decimal?;
            注文数_月 = dr["注文数_月", version] as decimal?;
            注文数_火 = dr["注文数_火", version] as decimal?;
            注文数_水 = dr["注文数_水", version] as decimal?;
            注文数_木 = dr["注文数_木", version] as decimal?;
            注文数_金 = dr["注文数_金", version] as decimal?;
            注文数_土 = dr["注文数_土", version] as decimal?;
            注文数_祝 = dr["注文数_祝", version] as decimal?;

            if (dr.Table.Columns.Contains("得意先名"))
            {
                得意先名 = dr["得意先名", version] as string;
            }
            if (dr.Table.Columns.Contains("商品名"))
            {
                商品名 = dr["商品名", version] as string;
            }
            if (dr.Table.Columns.Contains("商品単価税抜"))
            {
                商品単価税抜 = dr["商品単価税抜", version] as decimal?;
            }
            if (dr.Table.Columns.Contains("商品単価税込"))
            {
                商品単価税込 = dr["商品単価税込", version] as decimal?;
            }
            if (dr.Table.Columns.Contains("軽減税率フラグ"))
            {
                軽減税率フラグ = dr["軽減税率フラグ", version] as bool?;
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

        public decimal? 注文数_日
        {
            get { return _注文数_日; }
            set
            {
                _注文数_日 = value;
                if (value == 0)
                {
                    _注文数_日 = null;
                }
            }
        }

        public decimal? 注文数_月
        {
            get { return _注文数_月; }
            set
            {
                _注文数_月 = value;
                if (value == 0)
                {
                    _注文数_月 = null;
                }
            }
        }

        public decimal? 注文数_火
        {
            get { return _注文数_火; }
            set
            {
                _注文数_火 = value;
                if (value == 0)
                {
                    _注文数_火 = null;
                }
            }
        }

        public decimal? 注文数_水
        {
            get { return _注文数_水; }
            set
            {
                _注文数_水 = value;
                if (value == 0)
                {
                    _注文数_水 = null;
                }
            }
        }

        public decimal? 注文数_木
        {
            get { return _注文数_木; }
            set
            {
                _注文数_木 = value;
                if (value == 0)
                {
                    _注文数_木 = null;
                }
            }
        }

        public decimal? 注文数_金
        {
            get { return _注文数_金; }
            set
            {
                _注文数_金 = value;
                if (value == 0)
                {
                    _注文数_金 = null;
                }
            }
        }

        public decimal? 注文数_土
        {
            get { return _注文数_土; }
            set
            {
                _注文数_土 = value;
                if (value == 0)
                {
                    _注文数_土 = null;
                }
            }
        }

        public decimal? 注文数_祝
        {
            get { return _注文数_祝; }
            set
            {
                _注文数_祝 = value;
                if (value == 0)
                {
                    _注文数_祝 = null;
                }
            }
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

        public bool? 軽減税率フラグ
        {
            get { return _軽減税率フラグ; }
            set { _軽減税率フラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}] 商品ＣＤ=[{1}]",
                                        (得意先ＣＤ == 得意先ＣＤorg ? 得意先ＣＤ : 得意先ＣＤorg ?? 得意先ＣＤ),
                                        (商品ＣＤ == 商品ＣＤorg ? 商品ＣＤ : 商品ＣＤorg ?? 商品ＣＤ)
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
        public bool Equals(YoubiChumonSettei target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                注文数_日 == target.注文数_日 &&
                注文数_月 == target.注文数_月 &&
                注文数_火 == target.注文数_火 &&
                注文数_水 == target.注文数_水 &&
                注文数_木 == target.注文数_木 &&
                注文数_金 == target.注文数_金 &&
                注文数_土 == target.注文数_土 &&
                注文数_祝 == target.注文数_祝)
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
            string sql = "\r\n INSERT INTO 曜日別注文設定マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,注文数_日" +
                         "\r\n     ,注文数_月" +
                         "\r\n     ,注文数_火" +
                         "\r\n     ,注文数_水" +
                         "\r\n     ,注文数_木" +
                         "\r\n     ,注文数_金" +
                         "\r\n     ,注文数_土" +
                         "\r\n     ,注文数_祝" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@注文数_日" +
                         "\r\n     ,@注文数_月" +
                         "\r\n     ,@注文数_火" +
                         "\r\n     ,@注文数_水" +
                         "\r\n     ,@注文数_木" +
                         "\r\n     ,@注文数_金" +
                         "\r\n     ,@注文数_土" +
                         "\r\n     ,@注文数_祝" +
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
            string sql = "\r\n UPDATE 曜日別注文設定マスタ" +
                         "\r\n SET 得意先ＣＤ     = @得意先ＣＤ" +
                         "\r\n    ,商品ＣＤ       = @商品ＣＤ" +
                         "\r\n    ,注文数_日      = @注文数_日" +
                         "\r\n    ,注文数_月      = @注文数_月" +
                         "\r\n    ,注文数_火      = @注文数_火" +
                         "\r\n    ,注文数_水      = @注文数_水" +
                         "\r\n    ,注文数_木      = @注文数_木" +
                         "\r\n    ,注文数_金      = @注文数_金" +
                         "\r\n    ,注文数_土      = @注文数_土" +
                         "\r\n    ,注文数_祝      = @注文数_祝" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤorg" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
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
            string sql = "\r\n DELETE FROM 曜日別注文設定マスタ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤorg" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
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

                    if (注文数_日 != null)
                    {
                        cmd.Parameters.Add("@注文数_日", SqlDbType.Decimal).Value = 注文数_日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_日", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_月 != null)
                    {
                        cmd.Parameters.Add("@注文数_月", SqlDbType.Decimal).Value = 注文数_月;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_月", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_火 != null)
                    {
                        cmd.Parameters.Add("@注文数_火", SqlDbType.Decimal).Value = 注文数_火;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_火", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_水 != null)
                    {
                        cmd.Parameters.Add("@注文数_水", SqlDbType.Decimal).Value = 注文数_水;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_水", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_木 != null)
                    {
                        cmd.Parameters.Add("@注文数_木", SqlDbType.Decimal).Value = 注文数_木;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_木", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_金 != null)
                    {
                        cmd.Parameters.Add("@注文数_金", SqlDbType.Decimal).Value = 注文数_金;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_金", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_土 != null)
                    {
                        cmd.Parameters.Add("@注文数_土", SqlDbType.Decimal).Value = 注文数_土;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_土", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (注文数_祝 != null)
                    {
                        cmd.Parameters.Add("@注文数_祝", SqlDbType.Decimal).Value = 注文数_祝;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文数_祝", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("曜日別注文設定マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "商品ＣＤが重複しています。";
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

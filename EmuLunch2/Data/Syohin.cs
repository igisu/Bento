using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 商品マスタ
    /// </summary>
    public class Syohin : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="syoCd">商品ＣＤ</param>
        /// <returns></returns>
        public static Syohin GetSyohin(int syoCd)
        {
            return GetSyohin(syoCd, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="syoCd">商品ＣＤ</param>
        /// <returns></returns>
        public static Syohin GetSyohinReload(int syoCd)
        {
            return GetSyohin(syoCd, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="syoCd">商品ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static Syohin GetSyohin(int syoCd, bool isValid)
        {
            Syohin syohin = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.商品ＣＤ" +
                             "\r\n     ,A.部署グループ" +
                             "\r\n     ,BG.部署グループ名" +
                             "\r\n     ,A.商品名" +
                             "\r\n     ,A.商品名略称" +
                             "\r\n     ,A.商品名記号" +
                             "\r\n     ,A.商品単価税込" +
                             "\r\n     ,A.商品単価税抜" +
                             "\r\n     ,A.弁当区分" +
                             "\r\n     ,BK.弁当区分名" +
                             "\r\n     ,A.副食ＣＤ" +
                             "\r\n     ,A.主食ＣＤ" +
                             "\r\n     ,A.部数単位" +
                             "\r\n     ,A.食事区分" +
                             "\r\n     ,JK.食事区分名" +
                             "\r\n     ,A.軽減税率フラグ" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     商品マスタ AS A" +
                             "\r\n     LEFT JOIN V_部署グループ AS BG ON A.部署グループ = BG.部署グループCD" +
                             "\r\n     LEFT JOIN V_弁当区分 AS BK ON A.弁当区分 = BK.弁当区分CD" +
                             "\r\n     LEFT JOIN V_食事区分 AS JK ON A.食事区分 = JK.食事区分CD" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.商品ＣＤ = @商品ＣＤ" +
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
                cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syoCd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("商品マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    syohin = new Syohin(dt.Rows[0]);
                }
            }

            return syohin;
        }

        /// <summary>
        /// 商品マスタの商品を全件取得する
        /// </summary>
        /// <returns></returns>
        public static List<Syohin> GetSyohinList()
        {
            List<Syohin> syohinList = new List<Syohin>();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.商品ＣＤ" +
                             "\r\n     ,A.部署グループ" +
                             "\r\n     ,BG.部署グループ名" +
                             "\r\n     ,A.商品名" +
                             "\r\n     ,A.商品名略称" +
                             "\r\n     ,A.商品名記号" +
                             "\r\n     ,A.商品単価税込" +
                             "\r\n     ,A.商品単価税抜" +
                             "\r\n     ,A.弁当区分" +
                             "\r\n     ,BK.弁当区分名" +
                             "\r\n     ,A.副食ＣＤ" +
                             "\r\n     ,A.主食ＣＤ" +
                             "\r\n     ,A.部数単位" +
                             "\r\n     ,A.食事区分" +
                             "\r\n     ,JK.食事区分名" +
                             "\r\n     ,A.軽減税率フラグ" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     商品マスタ AS A" +
                             "\r\n     LEFT JOIN V_部署グループ AS BG ON A.部署グループ = BG.部署グループCD" +
                             "\r\n     LEFT JOIN V_弁当区分 AS BK ON A.弁当区分 = BK.弁当区分CD" +
                             "\r\n     LEFT JOIN V_食事区分 AS JK ON A.食事区分 = JK.食事区分CD" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ";
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("商品マスタ");
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    syohinList.Add(new Syohin(dr));
                }
            }

            return syohinList;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署グループ;
        private int? _商品ＣＤ;
        private string _商品名;
        private string _商品名略称;
        private string _商品名記号;
        private decimal? _商品単価税込;
        private decimal? _商品単価税抜;
        private int? _弁当区分;
        private int? _副食ＣＤ;
        private int? _主食ＣＤ;
        private int? _部数単位;
        private int? _食事区分;
        private bool? _軽減税率フラグ;
        private bool? _無効フラグ;

        private string _部署グループ名;
        private string _弁当区分名;
        private string _食事区分名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Syohin()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Syohin(Syohin src)
        {
            部署グループ = src.部署グループ;
            商品ＣＤ = src.商品ＣＤ;
            商品名 = src.商品名;
            商品名略称 = src.商品名略称;
            商品名記号 = src.商品名記号;
            商品単価税込 = src.商品単価税込;
            商品単価税抜 = src.商品単価税抜;
            弁当区分 = src.弁当区分;
            副食ＣＤ = src.副食ＣＤ;
            主食ＣＤ = src.主食ＣＤ;
            部数単位 = src.部数単位;
            食事区分 = src.食事区分;
            軽減税率フラグ = src.軽減税率フラグ;
            無効フラグ = src.無効フラグ;

            部署グループ名 = src.部署グループ名;
            弁当区分名 = src.弁当区分名;
            食事区分名 = src.食事区分名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Syohin(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署グループ = dr["部署グループ", version] as int?;
            商品ＣＤ = dr["商品ＣＤ", version] as int?;
            商品名 = dr["商品名", version] as string;
            商品名略称 = dr["商品名略称", version] as string;
            商品名記号 = dr["商品名記号", version] as string;
            商品単価税込 = dr["商品単価税込", version] as decimal?;
            商品単価税抜 = dr["商品単価税抜", version] as decimal?;
            弁当区分 = dr["弁当区分", version] as int?;
            副食ＣＤ = dr["副食ＣＤ", version] as int?;
            主食ＣＤ = dr["主食ＣＤ", version] as int?;
            部数単位 = dr["部数単位", version] as int?;
            食事区分 = dr["食事区分", version] as int?;
            軽減税率フラグ = dr["軽減税率フラグ", version] as bool?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("部署グループ名"))
            {
                部署グループ名 = dr["部署グループ名", version] as string;
            }
            if (dr.Table.Columns.Contains("弁当区分名"))
            {
                弁当区分名 = dr["弁当区分名", version] as string;
            }
            if (dr.Table.Columns.Contains("食事区分名"))
            {
                食事区分名 = dr["食事区分名", version] as string;
            }

            base.FromDataRow(dr);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署グループ
        {
            get { return _部署グループ; }
            set
            {
                _部署グループ = value;
                _errors.Remove("部署グループ");
                if (value == null || value < 0)
                {
                    _errors["部署グループ"] = "部署グループを入力してください。";
                }
            }
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
                else if(value < 0)
                {
                    _errors["商品ＣＤ"] = "商品ＣＤにマイナスは入力できません。";
                }
            }
        }

        public string 商品名
        {
            get { return _商品名; }
            set
            {
                _商品名 = value;
                _errors.Remove("商品名");
                if (value == null)
                {
                    _errors["商品名"] = "商品名を入力してください。";
                }
            }
        }

        public string 商品名略称
        {
            get { return _商品名略称; }
            set
            {
                _商品名略称 = value;
                _errors.Remove("商品名略称");
                if (value == null)
                {
                    _errors["商品名略称"] = "商品名略称を入力してください。";
                }
            }
        }

        public string 商品名記号
        {
            get { return _商品名記号; }
            set
            {
                _商品名記号 = value;
                _errors.Remove("商品名記号");
                if (value == null)
                {
                    _errors["商品名記号"] = "商品名記号を入力してください。";
                }
            }
        }

        public decimal? 商品単価税込
        {
            get { return _商品単価税込; }
            set
            {
                _商品単価税込 = value;
                _errors.Remove("商品単価税込");
                if (value == null)
                {
                    _errors["商品単価税込"] = "商品単価税込を入力してください。";
                }
            }
        }

        public decimal? 商品単価税抜
        {
            get { return _商品単価税抜; }
            set
            {
                _商品単価税抜 = value;
                _errors.Remove("商品単価税抜");
                if (value == null)
                {
                    _errors["商品単価税抜"] = "商品単価税抜を入力してください。";
                }
            }
        }

        public int? 弁当区分
        {
            get { return _弁当区分; }
            set
            {
                _弁当区分 = value;
                _errors.Remove("弁当区分");
                if (value == null || value < 0)
                {
                    _errors["弁当区分"] = "弁当区分を入力してください。";
                }
            }
        }

        public int? 副食ＣＤ
        {
            get { return _副食ＣＤ; }
            set
            {
                _副食ＣＤ = value;
            }
        }

        public int? 主食ＣＤ
        {
            get { return _主食ＣＤ; }
            set
            {
                _主食ＣＤ = value;
            }
        }

        public int? 部数単位
        {
            get { return _部数単位; }
            set
            {
                _部数単位 = value;
                _errors.Remove("部数単位");
                if (value == null)
                {
                    _errors["部数単位"] = "部数単位を入力してください。";
                }
                else
                {
                    if (value < 0)
                    {
                        _errors["部数単位"] = "部数単位にマイナスは入力できません。";
                    }
                }
            }
        }

        public int? 食事区分
        {
            get { return _食事区分; }
            set { _食事区分 = value; }
        }

        public bool? 軽減税率フラグ
        {
            get { return _軽減税率フラグ; }
            set { _軽減税率フラグ = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string 部署グループ名
        {
            get { return _部署グループ名; }
            set { _部署グループ名 = value; }
        }

        public string 弁当区分名
        {
            get { return _弁当区分名; }
            set { _弁当区分名 = value; }
        }

        public string 食事区分名
        {
            get { return _食事区分名; }
            set { _食事区分名 = value; }
        }

        public override string Code
        {
            get { return string.Format("商品ＣＤ=[{0}]", 商品ＣＤ); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get { return null; }
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
        public bool Equals(Syohin target)
        {
            if ((部署グループ ?? -1) == (target.部署グループ ?? -1) &&
                商品ＣＤ == target.商品ＣＤ &&
                商品名 == target.商品名 &&
                商品名略称 == target.商品名略称 &&
                商品名記号 == target.商品名記号 &&
                商品単価税込 == target.商品単価税込 &&
                商品単価税抜 == target.商品単価税抜 &&
                (弁当区分 ?? -1) == (target.弁当区分 ?? -1) &&
                (副食ＣＤ ?? -1) == (target.副食ＣＤ ?? -1) &&
                (主食ＣＤ ?? -1) == (target.主食ＣＤ ?? -1) &&
                部数単位 == target.部数単位 &&
                (食事区分 ?? -1) == (target.食事区分 ?? -1) &&
                (軽減税率フラグ ?? false) ==  (target.軽減税率フラグ ?? false) &&
                (無効フラグ ?? false) ==  (target.無効フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 商品マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,部署グループ" +
                         "\r\n     ,商品名" +
                         "\r\n     ,商品名略称" +
                         "\r\n     ,商品名記号" +
                         "\r\n     ,商品単価税込" +
                         "\r\n     ,商品単価税抜" +
                         "\r\n     ,弁当区分" +
                         "\r\n     ,副食ＣＤ" +
                         "\r\n     ,主食ＣＤ" +
                         "\r\n     ,部数単位" +
                         "\r\n     ,食事区分" +
                         "\r\n     ,軽減税率フラグ" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@部署グループ" +
                         "\r\n     ,@商品名" +
                         "\r\n     ,@商品名略称" +
                         "\r\n     ,@商品名記号" +
                         "\r\n     ,@商品単価税込" +
                         "\r\n     ,@商品単価税抜" +
                         "\r\n     ,@弁当区分" +
                         "\r\n     ,@副食ＣＤ" +
                         "\r\n     ,@主食ＣＤ" +
                         "\r\n     ,@部数単位" +
                         "\r\n     ,@食事区分" +
                         "\r\n     ,@軽減税率フラグ" +
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
            string sql = "\r\n UPDATE 商品マスタ" +
                         "\r\n SET 部署グループ   = @部署グループ" +
                         "\r\n    ,商品名         = @商品名" +
                         "\r\n    ,商品名略称     = @商品名略称" +
                         "\r\n    ,商品名記号     = @商品名記号" +
                         "\r\n    ,商品単価税込   = @商品単価税込" +
                         "\r\n    ,商品単価税抜   = @商品単価税抜" +
                         "\r\n    ,弁当区分       = @弁当区分" +
                         "\r\n    ,副食ＣＤ       = @副食ＣＤ" +
                         "\r\n    ,主食ＣＤ       = @主食ＣＤ" +
                         "\r\n    ,部数単位       = @部数単位" +
                         "\r\n    ,食事区分       = @食事区分" +
                         "\r\n    ,軽減税率フラグ = @軽減税率フラグ" +
                         "\r\n    ,無効フラグ     = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getDate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 商品ＣＤ = @商品ＣＤ" +
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
            string sql = "\r\n DELETE FROM 商品マスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 商品ＣＤ = @商品ＣＤ" +
                         "\r\n   AND 修正日 = @修正日";

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

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署グループ != null && 部署グループ != -1)
                    {
                        cmd.Parameters.Add("@部署グループ", SqlDbType.Int).Value = 部署グループ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署グループ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品名 != null)
                    {
                        cmd.Parameters.Add("@商品名", SqlDbType.VarChar).Value = 商品名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (商品名略称 != null)
                    {
                        cmd.Parameters.Add("@商品名略称", SqlDbType.VarChar).Value = 商品名略称;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品名略称", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (商品名記号 != null)
                    {
                        cmd.Parameters.Add("@商品名記号", SqlDbType.VarChar).Value = 商品名記号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品名記号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (商品単価税込 != null)
                    {
                        cmd.Parameters.Add("@商品単価税込", SqlDbType.Decimal).Value = 商品単価税込;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価税込", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (商品単価税抜 != null)
                    {
                        cmd.Parameters.Add("@商品単価税抜", SqlDbType.Decimal).Value = 商品単価税抜;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価税抜", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (弁当区分 != null && 弁当区分 != -1)
                    {
                        cmd.Parameters.Add("@弁当区分", SqlDbType.Int).Value = 弁当区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@弁当区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (副食ＣＤ != null &&  副食ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@副食ＣＤ", SqlDbType.Int).Value = 副食ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@副食ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (主食ＣＤ != null && 主食ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@主食ＣＤ", SqlDbType.Int).Value = 主食ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@主食ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部数単位 != null)
                    {
                        cmd.Parameters.Add("@部数単位", SqlDbType.Int).Value = 部数単位;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部数単位", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (食事区分 != null && 食事区分 != -1)
                    {
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = 食事区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (軽減税率フラグ != null)
                    {
                        cmd.Parameters.Add("@軽減税率フラグ", SqlDbType.Bit).Value = 軽減税率フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@軽減税率フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("商品マスタを{0}しました。", historyProcess.DisplayName());
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

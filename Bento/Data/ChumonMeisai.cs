using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 注文明細
    /// </summary>
    public class ChumonMeisai : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// キーに該当する次の明細番号を返します
        /// </summary>
        /// <param name="chumonKubun">注文区分（注文 または 予測）</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns>次の明細番号</returns>
        public static int GetNextMeisaiNo(Consts.ChumonKubun chumonKubun, DateTime kissyokubi, int tokuisakiCD, int busyoCD)
        {
            int meisaiNo = 1;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                meisaiNo = GetNextMeisaiNo(con, null, chumonKubun, kissyokubi, tokuisakiCD, busyoCD);
            }
            return meisaiNo;
        }

        /// <summary>
        /// キーに該当する次の明細番号を返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="chumonKubun">注文区分（注文 または 予測）</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns>次の明細番号</returns>
        public static int GetNextMeisaiNo(SqlConnection con, SqlTransaction tran, Consts.ChumonKubun chumonKubun, DateTime kissyokubi, int tokuisakiCD, int busyoCD)
        {
            int meisaiNo = 1;

            string sql = "\r\n SELECT" +
                         "\r\n     ISNULL(MAX(明細番号), 0) + 1 AS 明細番号" +
                         "\r\n FROM" +
                         "\r\n     注文明細" +
                         "\r\n WHERE" +
                         "\r\n     企業ＣＤ   = @企業ＣＤ" +
                         "\r\n AND 注文区分   = @注文区分" +
                         "\r\n AND 喫食日     = @喫食日" +
                         "\r\n AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n AND 部署ＣＤ   = @部署ＣＤ";

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)chumonKubun;
            cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;

            log.Debug(cmd.CommandTextWithParameters());

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    meisaiNo = (int)reader["明細番号"];
                }
            }

            return meisaiNo;
        }

        /// <summary>
        /// コードに該当するレコードを返します
        /// </summary>
        /// <param name="chumonKubun">注文区分（注文 または 予測）</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns>注文明細</returns>
        public static ChumonMeisai GetChumonMeisai(Consts.ChumonKubun chumonKubun, DateTime kissyokubi, int tokuisakiCD, int syohinCD, int busyoCD)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetChumonMeisai(con, null, chumonKubun, kissyokubi, tokuisakiCD, syohinCD, busyoCD);
            }
        }

        /// <summary>
        /// コードに該当するレコードを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="chumonKubun">注文区分（注文 または 予測）</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns>注文明細</returns>
        public static ChumonMeisai GetChumonMeisai(SqlConnection con, SqlTransaction tran, Consts.ChumonKubun chumonKubun, DateTime kissyokubi, int tokuisakiCD, int syohinCD, int busyoCD)
        {
            ChumonMeisai chumonMeisai = null;

            string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.注文区分" +
                         "\r\n     ,A.喫食日" +
                         "\r\n     ,A.得意先ＣＤ" +
                         "\r\n     ,A.商品ＣＤ" +
                         "\r\n     ,A.部署ＣＤ" +
                         "\r\n     ,A.明細番号" +
                         "\r\n     ,A.製造日" +
                         "\r\n     ,A.商品グループＣＤ" +
                         "\r\n     ,A.売掛現金区分" +
                         "\r\n     ,A.商品単価" +
                         "\r\n     ,A.商品数量" +
                         "\r\n     ,A.商品金額合計" +
                         "\r\n     ,A.消費税額" +
                         "\r\n     ,A.商品金額合計税込" +
                         "\r\n     ,A.計算税率" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     注文明細 AS A" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n WHERE" +
                         "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                         "\r\n AND A.注文区分   = @注文区分" +
                         "\r\n AND A.喫食日     = @喫食日" +
                         "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n AND A.商品ＣＤ   = @商品ＣＤ" +
                         "\r\n AND A.部署ＣＤ   = @部署ＣＤ";

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)chumonKubun;
            cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = kissyokubi;
            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
            cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCD;
            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("注文明細");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                chumonMeisai = new ChumonMeisai(dt.Rows[0]);
            }

            return chumonMeisai;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// ワーニング情報マップ
        /// </summary>
        protected Dictionary<string, string> _warns = new Dictionary<string, string>();

        private int? _注文区分;
        private DateTime? _喫食日;
        private int? _得意先ＣＤ;
        private int? _商品ＣＤ;
        private int? _商品ＣＤorg;              // レコード読込直後の値（更新・削除時に使用する）
        private int? _部署ＣＤ;
        private int? _明細番号;
        private DateTime? _製造日;
        private int? _商品グループＣＤ;
        private int? _売掛現金区分;
        private decimal? _商品単価;
        private decimal? _商品数量;
        private decimal? _商品金額合計;
        private decimal? _消費税額;
        private decimal? _商品金額合計税込;
        private decimal? _計算税率;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChumonMeisai()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public ChumonMeisai(ChumonMeisai src)
        {
            注文区分 = src.注文区分;
            喫食日 = src.喫食日;
            得意先ＣＤ = src.得意先ＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            商品ＣＤorg = src.商品ＣＤorg;
            部署ＣＤ = src.部署ＣＤ;
            明細番号 = src.明細番号;
            製造日 = src.製造日;
            商品グループＣＤ = src.商品グループＣＤ;
            売掛現金区分 = src.売掛現金区分;
            商品単価 = src.商品単価;
            商品数量 = src.商品数量;
            商品金額合計 = src.商品金額合計;
            消費税額 = src.消費税額;
            商品金額合計税込 = src.商品金額合計税込;
            計算税率 = src.計算税率;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public ChumonMeisai(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            注文区分 = dr["注文区分", version] as int?;
            喫食日 = dr["喫食日", version] as DateTime?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            商品ＣＤ = dr["商品ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("商品ＣＤorg"))
            {
                商品ＣＤorg = dr["商品ＣＤorg", version] as int?;
            }
            else
            {
                商品ＣＤorg = dr["商品ＣＤ", version] as int?;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            明細番号 = dr["明細番号", version] as int?;
            製造日 = dr["製造日", version] as DateTime?;
            商品グループＣＤ = dr["商品グループＣＤ", version] as int?;
            売掛現金区分 = dr["売掛現金区分", version] as int?;
            商品単価 = dr["商品単価", version] as decimal?;
            商品数量 = dr["商品数量", version] as decimal?;
            商品金額合計 = dr["商品金額合計", version] as decimal?;
            消費税額 = dr["消費税額", version] as decimal?;
            商品金額合計税込 = dr["商品金額合計税込", version] as decimal?;
            計算税率 = dr["計算税率", version] as decimal?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 注文区分
        {
            get { return _注文区分; }
            set
            {
                _注文区分 = value;
                _errors.Remove("注文区分");
                if (value == null || value < 0)
                {
                    _errors["注文区分"] = "注文区分を入力してください。";
                }
            }
        }

        public DateTime? 喫食日
        {
            get { return _喫食日; }
            set
            {
                _喫食日 = value;
                _errors.Remove("喫食日");
                if (value == null)
                {
                    _errors["喫食日"] = "喫食日（注文日）を入力してください。";
                }
            }
        }

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

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set { _商品ＣＤ = value; }
        }

        public int? 商品ＣＤorg
        {
            get { return _商品ＣＤorg; }
            set { _商品ＣＤorg = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public int? 明細番号
        {
            get { return _明細番号; }
            set { _明細番号 = value; }
        }

        public DateTime? 製造日
        {
            get { return _製造日; }
            set
            {
                _製造日 = value;
                _errors.Remove("製造日");
                _warns.Remove("製造日");
                if (value == null)
                {
                    // 商品ＣＤが未入力なら製造日が未入力でもいい
                    if (_商品ＣＤ != null)
                    {
                        _errors["製造日"] = "製造日を入力してください。";
                    }
                }
                else
                {
                    if (value < DateTime.Today)
                    {
                        _warns["製造日"] = "製造日を過ぎています。";
                    }
                }
            }
        }

        public int? 商品グループＣＤ
        {
            get { return _商品グループＣＤ; }
            set { _商品グループＣＤ = value; }
        }

        public int? 売掛現金区分
        {
            get { return _売掛現金区分; }
            set { _売掛現金区分 = value; }
        }

        public decimal? 商品単価
        {
            get { return _商品単価; }
            set { _商品単価 = value; }
        }

        public decimal? 商品数量
        {
            get { return _商品数量; }
            set { _商品数量 = value; }
        }

        public decimal? 商品金額合計
        {
            get { return _商品金額合計; }
            set { _商品金額合計 = value; }
        }

        public decimal? 消費税額
        {
            get { return _消費税額; }
            set { _消費税額 = value; }
        }

        public decimal? 商品金額合計税込
        {
            get { return _商品金額合計税込; }
            set { _商品金額合計税込 = value; }
        }

        public decimal? 計算税率
        {
            get { return _計算税率; }
            set { _計算税率 = value; }
        }

        public override string Code
        {
            get { return string.Format("注文区分=[{0}] 部署ＣＤ=[{1}] 喫食日=[{2}] 得意先ＣＤ=[{3}] 商品ＣＤ=[{4}] 数量=[{5}]",
                                        注文区分, 部署ＣＤ, 喫食日, 得意先ＣＤ, (商品ＣＤ == 商品ＣＤorg ? 商品ＣＤ : 商品ＣＤorg ?? 商品ＣＤ), 商品数量); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get
            {
                // 商品ＣＤが未入力で、製造日・商品単価・商品数量が入力されていればエラーとする
                _errors.Remove("商品ＣＤ");
                if (_商品ＣＤ == null)
                {
                    if (_製造日 != null ||
                        _商品単価 != null ||
                        _商品数量 != null)
                    {
                        _errors["商品ＣＤ"] = "商品ＣＤを入力してください。";
                        return "商品ＣＤを入力してください。";
                    }
                }
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                // 警告があればそれを、無ければエラーを返す
                string ret = (_warns.ContainsKey(columnName) ? _warns[columnName] : null);
                if (ret == null)
                {
                    ret = (_errors.ContainsKey(columnName) ? _errors[columnName] : null);
                }
                return ret;
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
        public bool Equals(ChumonMeisai target)
        {
            if ((注文区分 ?? -1) == (target.注文区分 ?? -1) &&
                喫食日 == target.喫食日 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                明細番号 == target.明細番号 &&
                製造日 == target.製造日 &&
                商品グループＣＤ == target.商品グループＣＤ &&
                (売掛現金区分 ?? -1) == (target.売掛現金区分 ?? -1) &&
                商品単価 == target.商品単価 &&
                商品数量 == target.商品数量 &&
                商品金額合計 == target.商品金額合計 &&
                消費税額 == target.消費税額 &&
                商品金額合計税込 == target.商品金額合計税込 &&
                計算税率 == target.計算税率)
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
            string sql = "\r\n INSERT INTO 注文明細(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,注文区分" +
                         "\r\n     ,喫食日" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,明細番号" +
                         "\r\n     ,製造日" +
                         "\r\n     ,商品グループＣＤ" +
                         "\r\n     ,売掛現金区分" +
                         "\r\n     ,商品単価" +
                         "\r\n     ,商品数量" +
                         "\r\n     ,商品金額合計" +
                         "\r\n     ,消費税額" +
                         "\r\n     ,商品金額合計税込" +
                         "\r\n     ,計算税率" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@注文区分" +
                         "\r\n     ,@喫食日" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@明細番号" +
                         "\r\n     ,@製造日" +
                         "\r\n     ,@商品グループＣＤ" +
                         "\r\n     ,@売掛現金区分" +
                         "\r\n     ,@商品単価" +
                         "\r\n     ,@商品数量" +
                         "\r\n     ,@商品金額合計" +
                         "\r\n     ,@消費税額" +
                         "\r\n     ,@商品金額合計税込" +
                         "\r\n     ,@計算税率" +
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
            string sql = "\r\n UPDATE 注文明細" +
                         "\r\n SET 商品ＣＤ         = @商品ＣＤ" +
                         "\r\n    ,明細番号         = @明細番号" +
                         "\r\n    ,製造日           = @製造日" +
                         "\r\n    ,商品グループＣＤ = @商品グループＣＤ" +
                         "\r\n    ,売掛現金区分     = @売掛現金区分" +
                         "\r\n    ,商品単価         = @商品単価" +
                         "\r\n    ,商品数量         = @商品数量" +
                         "\r\n    ,商品金額合計     = @商品金額合計" +
                         "\r\n    ,消費税額         = @消費税額" +
                         "\r\n    ,商品金額合計税込 = @商品金額合計税込" +
                         "\r\n    ,計算税率         = @計算税率" +
                         "\r\n    ,修正担当者ＣＤ   = @修正担当者ＣＤ" +
                         "\r\n    ,修正日           = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 注文区分   = @注文区分" +
                         "\r\n   AND 喫食日     = @喫食日" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
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
            string sql = "\r\n DELETE FROM 注文明細" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 注文区分   = @注文区分" +
                         "\r\n   AND 喫食日     = @喫食日" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
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

                    if (注文区分 != null && 注文区分 != -1)
                    {
                        cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = 注文区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (喫食日 != null)
                    {
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = 喫食日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
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

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (明細番号 != null)
                    {
                        cmd.Parameters.Add("@明細番号", SqlDbType.Int).Value = 明細番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@明細番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (製造日 != null)
                    {
                        cmd.Parameters.Add("@製造日", SqlDbType.DateTime).Value = 製造日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@製造日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (商品グループＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = 商品グループＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売掛現金区分 != null && 売掛現金区分 != -1)
                    {
                        cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = 売掛現金区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売掛現金区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品単価 != null)
                    {
                        cmd.Parameters.Add("@商品単価", SqlDbType.Decimal).Value = 商品単価;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (商品数量 != null)
                    {
                        cmd.Parameters.Add("@商品数量", SqlDbType.Decimal).Value = 商品数量;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品数量", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (商品金額合計 != null)
                    {
                        cmd.Parameters.Add("@商品金額合計", SqlDbType.Decimal).Value = 商品金額合計;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品金額合計", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (消費税額 != null)
                    {
                        cmd.Parameters.Add("@消費税額", SqlDbType.Decimal).Value = 消費税額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費税額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (商品金額合計税込 != null)
                    {
                        cmd.Parameters.Add("@商品金額合計税込", SqlDbType.Decimal).Value = 商品金額合計税込;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品金額合計税込", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (計算税率 != null)
                    {
                        cmd.Parameters.Add("@計算税率", SqlDbType.Decimal).Value = 計算税率;
                    }
                    else
                    {
                        cmd.Parameters.Add("@計算税率", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("注文明細を{0}しました。", historyProcess.DisplayName());
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

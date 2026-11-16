using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上明細
    /// </summary>
    public class UriageMeisai : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// キーに該当する次の明細番号を返します
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="uriagebi">売上日</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns>次の明細番号</returns>
        public static int GetNextMeisaiNo(int tokuisakiCD, DateTime uriagebi, int busyoCD)
        {
            int meisaiNo = 1;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                meisaiNo = GetNextMeisaiNo(con, null, tokuisakiCD, uriagebi, busyoCD);
            }
            return meisaiNo;
        }

        /// <summary>
        /// キーに該当する次の明細番号を返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="uriagebi">売上日</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns>次の明細番号</returns>
        public static int GetNextMeisaiNo(SqlConnection con, SqlTransaction tran, int tokuisakiCD, DateTime uriagebi, int busyoCD)
        {
            int meisaiNo = 1;

            string sql = "\r\n SELECT" +
                         "\r\n     ISNULL(MAX(明細番号), 0) + 1 AS 明細番号" +
                         "\r\n FROM" +
                         "\r\n     売上明細" +
                         "\r\n WHERE" +
                         "\r\n     企業ＣＤ   = @企業ＣＤ" +
                         "\r\n AND 売上日     = @売上日" +
                         "\r\n AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n AND 部署ＣＤ   = @部署ＣＤ";

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = uriagebi;
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
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _売上日;
        private int? _得意先ＣＤ;
        private int? _部署ＣＤ;
        private int? _明細番号;
        private int? _明細番号org;              // レコード読込直後の値（更新・削除時に使用する）
        private int? _商品ＣＤ;
        private int? _商品グループＣＤ;
        private int? _売上区分;
        private decimal? _商品単価;
        private decimal? _商品数量;
        private decimal? _値引額;
        private int? _値引事由区分;
        private decimal? _値引前金額;
        private decimal? _値引後金額;
        private decimal? _消費税額;
        private decimal? _商品金額合計税込;
        private decimal? _計算税率;
        private bool? _チケット束フラグ;
        private bool? _現金補填フラグ;
        private int? _主食ＣＤ;
        private int? _副食ＣＤ;
        private int? _食事区分;
        private int? _分配元数量;
        // TP決済番号とTP取消決済番号は、エムランチからは一切更新しない。
        // したがって、(取得はするが)追加/更新の対象外としている。
        private string _TP決済番号;
        private string _TP取消決済番号;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageMeisai()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageMeisai(UriageMeisai src)
        {
            売上日 = src.売上日;
            得意先ＣＤ = src.得意先ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            明細番号 = src.明細番号;
            明細番号org = src.明細番号org;
            商品ＣＤ = src.商品ＣＤ;
            商品グループＣＤ = src.商品グループＣＤ;
            売上区分 = src.売上区分;
            商品単価 = src.商品単価;
            商品数量 = src.商品数量;
            値引額 = src.値引額;
            値引事由区分 = src.値引事由区分;
            値引前金額 = src.値引前金額;
            値引後金額 = src.値引後金額;
            消費税額 = src.消費税額;
            商品金額合計税込 = src.商品金額合計税込;
            計算税率 = src.計算税率;
            チケット束フラグ = src.チケット束フラグ;
            現金補填フラグ = src.現金補填フラグ;
            主食ＣＤ = src.主食ＣＤ;
            副食ＣＤ = src.副食ＣＤ;
            食事区分 = src.食事区分;
            分配元数量 = src.分配元数量;
            TP決済番号 = src.TP決済番号;
            TP取消決済番号 = src.TP取消決済番号;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageMeisai(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            売上日 = dr["売上日", version] as DateTime?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            明細番号 = dr["明細番号", version] as int?;

            if (dr.Table.Columns.Contains("明細番号org"))
            {
                明細番号org = dr["明細番号org", version] as int?;
            }
            else
            {
                明細番号org = dr["明細番号", version] as int?;
            }

            商品ＣＤ = dr["商品ＣＤ", version] as int?;
            商品グループＣＤ = dr["商品グループＣＤ", version] as int?;
            売上区分 = dr["売上区分", version] as int?;
            商品単価 = dr["商品単価", version] as decimal?;
            商品数量 = dr["商品数量", version] as decimal?;
            値引額 = dr["値引額", version] as decimal?;
            値引事由区分 = dr["値引事由区分", version] as int?;
            値引前金額 = dr["値引前金額", version] as decimal?;
            値引後金額 = dr["値引後金額", version] as decimal?;
            消費税額 = dr["消費税額", version] as decimal?;
            商品金額合計税込 = dr["商品金額合計税込", version] as decimal?;
            計算税率 = dr["計算税率", version] as decimal?;
            チケット束フラグ = dr["チケット束フラグ", version] as bool?;
            現金補填フラグ = dr["現金補填フラグ", version] as bool?;
            主食ＣＤ = dr["主食ＣＤ", version] as int?;
            副食ＣＤ = dr["副食ＣＤ", version] as int?;
            食事区分 = dr["食事区分", version] as int?;
            分配元数量 = dr["分配元数量", version] as int?;

            if (dr.Table.Columns.Contains("TP決済番号"))
            {
                TP決済番号 = dr["TP決済番号", version] as string;
            }

            if (dr.Table.Columns.Contains("TP取消決済番号"))
            {
                TP取消決済番号 = dr["TP取消決済番号", version] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 売上日
        {
            get { return _売上日; }
            set
            {
                _売上日 = value;
                _errors.Remove("売上日");
                if (value == null)
                {
                    _errors["売上日"] = "売上日を入力してください。";
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

        public int? 明細番号org
        {
            get { return _明細番号org; }
            set { _明細番号org = value; }
        }

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set { _商品ＣＤ = value; }
        }

        public int? 商品グループＣＤ
        {
            get { return _商品グループＣＤ; }
            set { _商品グループＣＤ = value; }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set { _売上区分 = value; }
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

        public decimal? 値引額
        {
            get { return _値引額; }
            set
            {
                _値引額 = value;
                _errors.Remove("値引額");
                if ((value ?? 0m) != 0m)
                {
                    if ((商品金額合計税込 ?? 0m) != 0m)
                    {
                        _errors["値引額"] = "値引額が合計金額を超えています。";
                    }
                }
            }
        }

        public int? 値引事由区分
        {
            get { return _値引事由区分; }
            set { _値引事由区分 = value; }
        }

        public decimal? 値引前金額
        {
            get { return _値引前金額; }
            set { _値引前金額 = value; }
        }

        public decimal? 値引後金額
        {
            get { return _値引後金額; }
            set { _値引後金額 = value; }
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

        public bool? チケット束フラグ
        {
            get { return _チケット束フラグ; }
            set { _チケット束フラグ = value; }
        }

        public bool? 現金補填フラグ
        {
            get { return _現金補填フラグ; }
            set { _現金補填フラグ = value; }
        }

        public int? 主食ＣＤ
        {
            get { return _主食ＣＤ; }
            set
            {
                _主食ＣＤ = value;
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

        public int? 食事区分
        {
            get { return _食事区分; }
            set { _食事区分 = value; }
        }

        public int? 分配元数量
        {
            get { return _分配元数量; }
            set { _分配元数量 = value; }
        }

        public string TP決済番号
        {
            get { return _TP決済番号; }
            set { _TP決済番号 = value; }
        }

        public string TP取消決済番号
        {
            get { return _TP取消決済番号; }
            set { _TP取消決済番号 = value; }
        }

        public override string Code
        {
            get { return string.Format("売上日=[{0}] 得意先ＣＤ=[{1}] 部署ＣＤ=[{2}] 明細番号=[{3}] 商品ＣＤ=[{4}]",
                                        売上日, 得意先ＣＤ, 部署ＣＤ, (明細番号 == 明細番号org ? 明細番号 : 明細番号org ?? 明細番号), 商品ＣＤ); }
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
                string ret = null;
                _errors.Remove("商品ＣＤ");
                _errors.Remove("値引額");
                _errors.Remove("値引事由区分");

                // 商品ＣＤが未入力で、商品単価・商品数量・値引額が入力されていればエラーとする
                if (_商品ＣＤ == null)
                {
                    if (_商品単価 != null ||
                        _商品数量 != null ||
                        _値引額 != null)
                    {
                        ret = "商品ＣＤを入力してください。";
                        _errors["商品ＣＤ"] = "商品ＣＤを入力してください。";
                    }
                }

                // 値引額が入力されていれば、値引事由区分は必須
                if (値引額 != null && 値引額 != 0m)
                {
                    if (値引事由区分 == null || 値引事由区分 == -1)
                    {
                        ret = "値引事由を入力してください。";
                        _errors["値引事由区分"] = "値引事由区分を入力してください。";
                    }
                }
                else
                {
                    // 値引事由が入力されていれば、値引額は必須
                    if (値引事由区分 != null && 値引事由区分 != -1)
                    {
                        ret = "値引額を入力してください。";
                        _errors["値引額"] = "値引額を入力してください。";
                    }
                }
                return ret;
            }
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
        public bool Equals(UriageMeisai target)
        {
            if (売上日 == target.売上日 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                明細番号 == target.明細番号 &&
                商品ＣＤ == target.商品ＣＤ &&
                商品グループＣＤ == target.商品グループＣＤ &&
                (売上区分 ?? -1) == (target.売上区分 ?? -1) &&
                商品単価 == target.商品単価 &&
                商品数量 == target.商品数量 &&
                (値引額 ?? 0m) == (target.値引額 ?? 0m) &&
                値引事由区分 == target.値引事由区分 &&
                値引前金額 == target.値引前金額 &&
                値引後金額 == target.値引後金額 &&
                消費税額 == target.消費税額 &&
                商品金額合計税込 == target.商品金額合計税込 &&
                計算税率 == target.計算税率 &&
                (チケット束フラグ ?? false) == (target.チケット束フラグ ?? false) &&
                (現金補填フラグ ?? false) == (target.現金補填フラグ ?? false) &&
                (主食ＣＤ ?? -1) == (target.主食ＣＤ ?? -1) &&
                (副食ＣＤ ?? -1) == (target.副食ＣＤ ?? -1) &&
                (食事区分 ?? -1) == (target.食事区分 ?? -1) &&
                分配元数量 == target.分配元数量 &&
                TP決済番号 == target.TP決済番号 &&
                TP取消決済番号 == target.TP取消決済番号)
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
            string sql = "\r\n INSERT INTO 売上明細(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,売上日" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,明細番号" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,商品グループＣＤ" +
                         "\r\n     ,売上区分" +
                         "\r\n     ,商品単価" +
                         "\r\n     ,商品数量" +
                         "\r\n     ,値引額" +
                         "\r\n     ,値引事由区分" +
                         "\r\n     ,値引前金額" +
                         "\r\n     ,値引後金額" +
                         "\r\n     ,消費税額" +
                         "\r\n     ,商品金額合計税込" +
                         "\r\n     ,計算税率" +
                         "\r\n     ,チケット束フラグ" +
                         "\r\n     ,現金補填フラグ" +
                         "\r\n     ,主食ＣＤ" +
                         "\r\n     ,副食ＣＤ" +
                         "\r\n     ,食事区分" +
                         "\r\n     ,分配元数量" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@売上日" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@明細番号" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@商品グループＣＤ" +
                         "\r\n     ,@売上区分" +
                         "\r\n     ,@商品単価" +
                         "\r\n     ,@商品数量" +
                         "\r\n     ,@値引額" +
                         "\r\n     ,@値引事由区分" +
                         "\r\n     ,@値引前金額" +
                         "\r\n     ,@値引後金額" +
                         "\r\n     ,@消費税額" +
                         "\r\n     ,@商品金額合計税込" +
                         "\r\n     ,@計算税率" +
                         "\r\n     ,@チケット束フラグ" +
                         "\r\n     ,@現金補填フラグ" +
                         "\r\n     ,@主食ＣＤ" +
                         "\r\n     ,@副食ＣＤ" +
                         "\r\n     ,@食事区分" +
                         "\r\n     ,@分配元数量" +
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
            string sql = "\r\n UPDATE 売上明細" +
                         "\r\n SET 明細番号         = @明細番号" +
                         "\r\n    ,商品ＣＤ         = @商品ＣＤ" +
                         "\r\n    ,商品グループＣＤ = @商品グループＣＤ" +
                         "\r\n    ,売上区分         = @売上区分" +
                         "\r\n    ,商品単価         = @商品単価" +
                         "\r\n    ,商品数量         = @商品数量" +
                         "\r\n    ,値引額           = @値引額" +
                         "\r\n    ,値引事由区分     = @値引事由区分" +
                         "\r\n    ,値引前金額       = @値引前金額" +
                         "\r\n    ,値引後金額       = @値引後金額" +
                         "\r\n    ,消費税額         = @消費税額" +
                         "\r\n    ,商品金額合計税込 = @商品金額合計税込" +
                         "\r\n    ,計算税率         = @計算税率" +
                         "\r\n    ,チケット束フラグ = @チケット束フラグ" +
                         "\r\n    ,現金補填フラグ   = @現金補填フラグ" +
                         "\r\n    ,主食ＣＤ         = @主食ＣＤ" +
                         "\r\n    ,副食ＣＤ         = @副食ＣＤ" +
                         "\r\n    ,食事区分         = @食事区分" +
                         "\r\n    ,分配元数量       = @分配元数量" +
                         "\r\n    ,修正担当者ＣＤ   = @修正担当者ＣＤ" +
                         "\r\n    ,修正日           = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 売上日     = @売上日" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND 明細番号   = @明細番号org" +
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
            string sql = "\r\n DELETE FROM 売上明細" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 売上日     = @売上日" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND 明細番号   = @明細番号org" +
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

                    if (売上日 != null)
                    {
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = 売上日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
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

                    if (明細番号org != null)
                    {
                        cmd.Parameters.Add("@明細番号org", SqlDbType.Int).Value = 明細番号org;
                    }
                    else
                    {
                        cmd.Parameters.Add("@明細番号org", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品グループＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = 商品グループＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売上区分 != null && 売上区分 != -1)
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = 売上区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = DBNull.Value;
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

                    if (値引額 != null)
                    {
                        cmd.Parameters.Add("@値引額", SqlDbType.Decimal).Value = 値引額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@値引額", SqlDbType.Decimal).Value = 0m;
                    }

                    if (値引事由区分 != null && 値引事由区分 != -1)
                    {
                        cmd.Parameters.Add("@値引事由区分", SqlDbType.Int).Value = 値引事由区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@値引事由区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (値引前金額 != null)
                    {
                        cmd.Parameters.Add("@値引前金額", SqlDbType.Decimal).Value = 値引前金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@値引前金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (値引後金額 != null)
                    {
                        cmd.Parameters.Add("@値引後金額", SqlDbType.Decimal).Value = 値引後金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@値引後金額", SqlDbType.Decimal).Value = DBNull.Value;
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

                    if (チケット束フラグ != null)
                    {
                        cmd.Parameters.Add("@チケット束フラグ", SqlDbType.Bit).Value = チケット束フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@チケット束フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (現金補填フラグ != null)
                    {
                        cmd.Parameters.Add("@現金補填フラグ", SqlDbType.Bit).Value = 現金補填フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@現金補填フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (主食ＣＤ != null && 主食ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@主食ＣＤ", SqlDbType.Int).Value = 主食ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@主食ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (副食ＣＤ != null && 副食ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@副食ＣＤ", SqlDbType.Int).Value = 副食ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@副食ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (食事区分 != null && 食事区分 != -1)
                    {
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = 食事区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (分配元数量 != null)
                    {
                        cmd.Parameters.Add("@分配元数量", SqlDbType.Int).Value = 分配元数量;
                    }
                    else
                    {
                        cmd.Parameters.Add("@分配元数量", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("売上明細を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "明細番号が重複しています。";
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

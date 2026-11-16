using Emu.Bento.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Emu.Bento.Data
{
    /// <summary>
    /// TP決済情報
    /// </summary>
    public class TPSettlementInfo : TableBase2
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="tpNo">TP決済番号</param>
        /// <param name="kessaibi">決済日</param>
        /// <returns></returns>
        public static TPSettlementInfo GetTPSettlementInfo(string tpNo, DateTime kessaibi)
        {
            TPSettlementInfo tpSettlementInfo = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                tpSettlementInfo = GetTPSettlementInfo(con, null, tpNo, kessaibi);
            }

            return tpSettlementInfo;
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tpNo">TP決済番号</param>
        /// <param name="kessaibi">決済日</param>
        /// <returns></returns>
        public static TPSettlementInfo GetTPSettlementInfo(SqlConnection con, SqlTransaction tran, string tpNo, DateTime kessaibi)
        {
            TPSettlementInfo tpSettlementInfo = null;

            string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.TP決済番号" +
                         "\r\n     ,A.決済日" +
                         "\r\n     ,A.部署ＣＤ" +
                         "\r\n     ,A.得意先ＣＤ" +                         
                         "\r\n     ,A.売上明細番号" +
                         "\r\n     ,A.入金番号" +
                         "\r\n     ,A.端末ID" +
                         "\r\n     ,A.取引日時" +
                         "\r\n     ,A.取引金額" +
                         "\r\n     ,A.クレジットカード会社ID" +
                         "\r\n     ,A.クレジットカード会社名" +
                         "\r\n     ,A.アクワイアラID" +
                         "\r\n     ,A.アクワイアラ名" +
                         "\r\n     ,A.電子マネーブランドID" +
                         "\r\n     ,A.電子マネーブランド名" +
                         "\r\n     ,A.カード番号" +
                         "\r\n     ,A.伝票番号" +
                         "\r\n     ,A.承認番号" +
                         "\r\n     ,A.取引区分" +
                         "\r\n     ,A.支払区分" +
                         "\r\n     ,A.取扱区分" +
                         "\r\n     ,A.商品区分コード" +
                         "\r\n     ,A.元伝票番号" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     TP決済情報 AS A" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n WHERE" +
                         "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                         "\r\n     AND A.TP決済番号 = @TP決済番号" +
                         "\r\n     AND A.決済日 = @決済日";

            SqlCommand cmd = con.CreateCommand();

            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@TP決済番号", SqlDbType.VarChar).Value = tpNo;
            cmd.Parameters.Add("@決済日", SqlDbType.Date).Value = kessaibi;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("TP決済情報");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                tpSettlementInfo = new TPSettlementInfo(dt.Rows[0]);
            }

            return tpSettlementInfo;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private string _TP決済番号;
        private DateTime? _決済日;
        private int? _部署ＣＤ;
        private int? _得意先ＣＤ;
        private int? _売上明細番号;
        private decimal? _入金番号;
        private string _端末ID;
        private DateTime? _取引日時;
        private decimal? _取引金額;
        private int? _クレジットカード会社ID;
        private string _クレジットカード会社名;
        private long? _アクワイアラID;
        private string _アクワイアラ名;
        private int? _電子マネーブランドID;
        private string _電子マネーブランド名;
        private string _カード番号;
        private int? _伝票番号;
        private string _承認番号;
        private string _取引区分;
        private string _支払区分;
        private string _取扱区分;
        private string _商品区分コード;
        private int? _元伝票番号;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TPSettlementInfo()
        {
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TPSettlementInfo(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;

            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            TP決済番号 = dr["TP決済番号", version] as string;
            決済日 = dr["決済日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;            
            売上明細番号 = dr["売上明細番号", version] as int?;
            入金番号 = dr["入金番号", version] as decimal?;
            端末ID = dr["端末ID", version] as string;
            取引日時 = dr["取引日時", version] as DateTime?;
            取引金額 = dr["取引金額", version] as decimal?;
            クレジットカード会社ID = dr["クレジットカード会社ID", version] as int?;
            クレジットカード会社名 = dr["クレジットカード会社名", version] as string;
            アクワイアラID = dr["アクワイアラID", version] as long?;
            アクワイアラ名 = dr["アクワイアラ名", version] as string;
            電子マネーブランドID = dr["電子マネーブランドID", version] as int?;
            電子マネーブランド名 = dr["電子マネーブランド名", version] as string;
            カード番号 = dr["カード番号", version] as string;
            伝票番号 = dr["伝票番号", version] as int?;
            承認番号 = dr["承認番号", version] as string;
            取引区分 = dr["取引区分", version] as string;
            支払区分 = dr["支払区分", version] as string;
            取扱区分 = dr["取扱区分", version] as string;
            商品区分コード = dr["商品区分コード", version] as string;
            元伝票番号 = dr["元伝票番号", version] as int?;

            base.FromDataRow(dr, version);
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public string TP決済番号
        {
            get { return _TP決済番号; }
            set { _TP決済番号 = value; }
        }

        public DateTime? 決済日
        {
            get { return _決済日; }
            set { _決済日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }        

        public int? 売上明細番号
        {
            get { return _売上明細番号; }
            set { _売上明細番号 = value; }
        }

        public decimal? 入金番号
        {
            get { return _入金番号; }
            set { _入金番号 = value; }
        }

        public string 端末ID
        {
            get { return _端末ID; }
            set { _端末ID = value; }
        }

        public DateTime? 取引日時
        {
            get { return _取引日時; }
            set { _取引日時 = value; }
        }

        public decimal? 取引金額
        {
            get { return _取引金額; }
            set { _取引金額 = value; }
        }

        public int? クレジットカード会社ID
        {
            get { return _クレジットカード会社ID; }
            set { _クレジットカード会社ID = value; }
        }

        public string クレジットカード会社名
        {
            get { return _クレジットカード会社名; }
            set { _クレジットカード会社名 = value; }
        }

        public long? アクワイアラID
        {
            get { return _アクワイアラID; }
            set { _アクワイアラID = value; }
        }

        public string アクワイアラ名
        {
            get { return _アクワイアラ名; }
            set { _アクワイアラ名 = value; }
        }

        public int? 電子マネーブランドID
        {
            get { return _電子マネーブランドID; }
            set { _電子マネーブランドID = value; }
        }

        public string 電子マネーブランド名
        {
            get { return _電子マネーブランド名; }
            set { _電子マネーブランド名 = value; }
        }

        public string カード番号
        {
            get { return _カード番号; }
            set { _カード番号 = value; }
        }

        public int? 伝票番号
        {
            get { return _伝票番号; }
            set { _伝票番号 = value; }
        }

        public string 承認番号
        {
            get { return _承認番号; }
            set { _承認番号 = value; }
        }

        public string 取引区分
        {
            get { return _取引区分; }
            set { _取引区分 = value; }
        }

        public string 支払区分
        {
            get { return _支払区分; }
            set { _支払区分 = value; }
        }

        public string 取扱区分
        {
            get { return _取扱区分; }
            set { _取扱区分 = value; }
        }

        public string 商品区分コード
        {
            get { return _商品区分コード; }
            set { _商品区分コード = value; }
        }

        public int? 元伝票番号
        {
            get { return _元伝票番号; }
            set { _元伝票番号 = value; }
        }

        public string 決済方法名
        {
            get
            {
                return (string.IsNullOrEmpty(クレジットカード会社名) == false) ? クレジットカード会社名 : 電子マネーブランド名;
            }
        }

        public override string Code
        {
            get { return string.Format("TP決済番号={0}", TP決済番号); }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        
        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO TP決済情報(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,TP決済番号" +
                         "\r\n     ,決済日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +                         
                         "\r\n     ,売上明細番号" +
                         "\r\n     ,入金番号" +
                         "\r\n     ,端末ID" +
                         "\r\n     ,取引日時" +
                         "\r\n     ,取引金額" +
                         "\r\n     ,クレジットカード会社ID" +
                         "\r\n     ,クレジットカード会社名" +
                         "\r\n     ,アクワイアラID" +
                         "\r\n     ,アクワイアラ名" +
                         "\r\n     ,電子マネーブランドID" +
                         "\r\n     ,電子マネーブランド名" +
                         "\r\n     ,カード番号" +
                         "\r\n     ,伝票番号" +
                         "\r\n     ,承認番号" +
                         "\r\n     ,取引区分" +
                         "\r\n     ,支払区分" +
                         "\r\n     ,取扱区分" +
                         "\r\n     ,商品区分コード" +
                         "\r\n     ,元伝票番号" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@TP決済番号" +
                         "\r\n     ,@決済日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@売上明細番号" +
                         "\r\n     ,@入金番号" +
                         "\r\n     ,@端末ID" +
                         "\r\n     ,@取引日時" +
                         "\r\n     ,@取引金額" +
                         "\r\n     ,@クレジットカード会社ID" +
                         "\r\n     ,@クレジットカード会社名" +
                         "\r\n     ,@アクワイアラID" +
                         "\r\n     ,@アクワイアラ名" +
                         "\r\n     ,@電子マネーブランドID" +
                         "\r\n     ,@電子マネーブランド名" +
                         "\r\n     ,@カード番号" +
                         "\r\n     ,@伝票番号" +
                         "\r\n     ,@承認番号" +
                         "\r\n     ,@取引区分" +
                         "\r\n     ,@支払区分" +
                         "\r\n     ,@取扱区分" +
                         "\r\n     ,@商品区分コード" +
                         "\r\n     ,@元伝票番号" +
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
            string sql = "\r\n UPDATE TP決済情報" +
                         "\r\n SET 部署ＣＤ                = @部署ＣＤ" +
                         "\r\n     ,得意先ＣＤ             = @得意先ＣＤ" +
                         "\r\n     ,売上明細番号           = @売上明細番号" +
                         "\r\n     ,入金番号               = @入金番号" +
                         "\r\n     ,端末ID				   = @端末ID" +
                         "\r\n     ,取引日時			   = @取引日時" +
                         "\r\n     ,取引金額			   = @取引金額" +
                         "\r\n     ,クレジットカード会社ID = @クレジットカード会社ID" +
                         "\r\n     ,クレジットカード会社名 = @クレジットカード会社名" +
                         "\r\n     ,アクワイアラID         = @アクワイアラID" +
                         "\r\n     ,アクワイアラ名         = @アクワイアラ名" +
                         "\r\n     ,電子マネーブランドID   = @電子マネーブランドID" +
                         "\r\n     ,電子マネーブランド名   = @電子マネーブランド名" +
                         "\r\n     ,カード番号			   = @カード番号" +
                         "\r\n     ,伝票番号			   = @伝票番号" +
                         "\r\n     ,承認番号			   = @承認番号" +
                         "\r\n     ,取引区分			   = @取引区分" +
                         "\r\n     ,支払区分			   = @支払区分" +
                         "\r\n     ,取扱区分			   = @取扱区分" +
                         "\r\n     ,商品区分コード		   = @商品区分コード" +
                         "\r\n     ,元伝票番号			   = @元伝票番号" +
                         "\r\n     ,修正担当者ＣＤ         = @修正担当者ＣＤ" +
                         "\r\n     ,修正日                 = getdate()" +
                         "\r\n WHERE 企業ＣＤ              = @企業ＣＤ" +
                         "\r\n   AND TP決済番号            = @TP決済番号" +
                         "\r\n   AND 決済日                = @決済日" +                         
                         "\r\n   AND 修正日                = @修正日";

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
            string sql = "\r\n DELETE TP決済情報" +
                         "\r\n WHERE 企業ＣＤ              = @企業ＣＤ" +
                         "\r\n   AND TP決済番号            = @TP決済番号" +
                         "\r\n   AND 決済日                = @決済日" +
                         "\r\n   AND 修正日                = @修正日";

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

                    if (TP決済番号 != null)
                    {
                        cmd.Parameters.Add("@TP決済番号", SqlDbType.VarChar).Value = TP決済番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@TP決済番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (決済日 != null)
                    {
                        cmd.Parameters.Add("@決済日", SqlDbType.Date).Value = 決済日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@決済日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }                    

                    if (売上明細番号 != null)
                    {
                        cmd.Parameters.Add("@売上明細番号", SqlDbType.Int).Value = 売上明細番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上明細番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (入金番号 != null)
                    {
                        cmd.Parameters.Add("@入金番号", SqlDbType.Decimal).Value = 入金番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (端末ID != null)
                    {
                        cmd.Parameters.Add("@端末ID", SqlDbType.VarChar).Value = 端末ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@端末ID", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (取引日時 != null)
                    {
                        cmd.Parameters.Add("@取引日時", SqlDbType.DateTime).Value = 取引日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取引日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (取引金額 != null)
                    {
                        cmd.Parameters.Add("@取引金額", SqlDbType.Decimal).Value = 取引金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取引金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (クレジットカード会社ID != null)
                    {
                        cmd.Parameters.Add("@クレジットカード会社ID", SqlDbType.Int).Value = クレジットカード会社ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレジットカード会社ID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (クレジットカード会社名 != null)
                    {
                        cmd.Parameters.Add("@クレジットカード会社名", SqlDbType.VarChar).Value = クレジットカード会社名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレジットカード会社名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (アクワイアラID != null)
                    {
                        cmd.Parameters.Add("@アクワイアラID", SqlDbType.BigInt).Value = アクワイアラID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@アクワイアラID", SqlDbType.BigInt).Value = DBNull.Value;
                    }

                    if (アクワイアラ名 != null)
                    {
                        cmd.Parameters.Add("@アクワイアラ名", SqlDbType.VarChar).Value = アクワイアラ名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@アクワイアラ名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電子マネーブランドID != null)
                    {
                        cmd.Parameters.Add("@電子マネーブランドID", SqlDbType.Int).Value = 電子マネーブランドID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネーブランドID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電子マネーブランド名 != null)
                    {
                        cmd.Parameters.Add("@電子マネーブランド名", SqlDbType.VarChar).Value = 電子マネーブランド名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネーブランド名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (カード番号 != null)
                    {
                        cmd.Parameters.Add("@カード番号", SqlDbType.VarChar).Value = カード番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@カード番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (伝票番号 != null)
                    {
                        cmd.Parameters.Add("@伝票番号", SqlDbType.Int).Value = 伝票番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@伝票番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (承認番号 != null)
                    {
                        cmd.Parameters.Add("@承認番号", SqlDbType.VarChar).Value = 承認番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@承認番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (取引区分 != null)
                    {
                        cmd.Parameters.Add("@取引区分", SqlDbType.VarChar).Value = 取引区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取引区分", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (支払区分 != null)
                    {
                        cmd.Parameters.Add("@支払区分", SqlDbType.VarChar).Value = 支払区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支払区分", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (取扱区分 != null)
                    {
                        cmd.Parameters.Add("@取扱区分", SqlDbType.VarChar).Value = 取扱区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取扱区分", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (商品区分コード != null)
                    {
                        cmd.Parameters.Add("@商品区分コード", SqlDbType.VarChar).Value = 商品区分コード;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品区分コード", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (元伝票番号 != null)
                    {
                        cmd.Parameters.Add("@元伝票番号", SqlDbType.Int).Value = 元伝票番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@元伝票番号", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("TP決済情報を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "TP決済番号が重複しています。";
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

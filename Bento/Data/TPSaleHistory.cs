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
    /// TP売上履歴
    /// </summary>
    public class TPSaleHistory : TableBase2
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
        public static TPSaleHistory GetTPSaleHistory(string tpNo, DateTime kessaibi)
        {
            TPSaleHistory tpSaleHistory = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                tpSaleHistory = GetTPSaleHistory(con, null, tpNo, kessaibi);
            }

            return tpSaleHistory;
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tpNo">TP決済番号</param>
        /// <param name="kessaibi">決済日</param>
        /// <returns></returns>
        public static TPSaleHistory GetTPSaleHistory(SqlConnection con, SqlTransaction tran, string tpNo, DateTime kessaibi)
        {
            TPSaleHistory tpSaleHistory = null;

            string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.TP決済番号" +
                         "\r\n     ,A.決済日" +
                         "\r\n     ,A.売上履歴ID" +
                         "\r\n     ,A.売上日時" +
                         "\r\n     ,A.伝票番号" +
                         "\r\n     ,A.承認番号" +
                         "\r\n     ,A.商品区分コード" +
                         "\r\n     ,A.売上額" +
                         "\r\n     ,A.税額" +
                         "\r\n     ,A.割引後売上額" +
                         "\r\n     ,A.取消済フラグ" +
                         "\r\n     ,A.クレジットカード番号下4桁" +
                         "\r\n     ,A.トランスミッション日時" +
                         "\r\n     ,A.取消元売上履歴ID" +
                         "\r\n     ,A.取消元伝票番号" +
                         "\r\n     ,A.取消元トランスミッション日時" +
                         "\r\n     ,A.売上種別ID" +
                         "\r\n     ,CASE A.売上種別ID" +
                         "\r\n          WHEN 0 THEN '0：現金' " +
                         "\r\n          WHEN 1 THEN '1：クレジット' " +
                         "\r\n          WHEN 2 THEN '2：電子マネー' " +
                         "\r\n          ELSE '' " +
                         "\r\n      END AS 売上種別名" +
                         "\r\n     ,A.決済結果ID" +
                         "\r\n     ,CASE A.決済結果ID" +
                         "\r\n          WHEN 0 THEN '0：成立' " +
                         "\r\n          WHEN 1 THEN '1：不成立' " +
                         "\r\n          ELSE '' " +
                         "\r\n      END AS 決済結果名" +
                         "\r\n     ,A.キャンセル可能フラグ" +
                         "\r\n     ,A.トレーニングモード実行フラグ" +
                         "\r\n     ,A.レシートUUID" +
                         "\r\n     ,A.再印刷可能フラグ" +
                         "\r\n     ,A.電子マネー売上履歴ID" +
                         "\r\n     ,A.電子マネーブランドID" +
                         "\r\n     ,A.電子マネーブランド名" +
                         "\r\n     ,A.電子マネーステータスID" +
                         "\r\n     ,CASE A.電子マネーステータスID" +
                         "\r\n          WHEN 0 THEN '0：成立' " +
                         "\r\n          WHEN 1 THEN '1：不成立' " +
                         "\r\n          WHEN 2 THEN '2：処理未了' " +
                         "\r\n          ELSE '' " +
                         "\r\n      END AS 電子マネーステータス名" +
                         "\r\n     ,A.電子マネー残高照会結果ID" +
                         "\r\n     ,CASE A.電子マネー残高照会結果ID" +
                         "\r\n          WHEN 0 THEN '0：未了成立' " +
                         "\r\n          WHEN 1 THEN '1：未了不成立' " +
                         "\r\n          ELSE '' " +
                         "\r\n      END AS 電子マネー残高照会結果名" +
                         "\r\n     ,A.電子マネー上位端末ID" +
                         "\r\n     ,A.電子マネー決済ID" +
                         "\r\n     ,A.電子マネーカード番号" +
                         "\r\n     ,A.電子マネーエラーコード" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     TP売上履歴 AS A" +
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
            DataTable dt = new DataTable("TP売上履歴");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                tpSaleHistory = new TPSaleHistory(dt.Rows[0]);
            }

            return tpSaleHistory;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private string _TP決済番号;
        private DateTime? _決済日;
        private long? _売上履歴ID;
        private DateTime? _売上日時;
        private int? _伝票番号;
        private string _承認番号;
        private string _商品区分コード;
        private decimal? _売上額;
        private decimal? _税額;
        private decimal? _割引後売上額;
        private bool? _取消済フラグ;
        private string _クレジットカード番号下4桁;
        private DateTime? _トランスミッション日時;
        private long? _取消元売上履歴ID;
        private int? _取消元伝票番号;
        private DateTime? _取消元トランスミッション日時;
        private int? _売上種別ID;
        private int? _決済結果ID;
        private bool? _キャンセル可能フラグ;
        private bool? _トレーニングモード実行フラグ;
        private string _レシートUUID;
        private bool? _再印刷可能フラグ;
        private long? _電子マネー売上履歴ID;
        private int? _電子マネーブランドID;
        private string _電子マネーブランド名;
        private int? _電子マネーステータスID;
        private int? _電子マネー残高照会結果ID;
        private string _電子マネー上位端末ID;
        private int? _電子マネー決済ID;
        private string _電子マネーカード番号;
        private int? _電子マネーエラーコード;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TPSaleHistory()
        {
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TPSaleHistory(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;

            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            TP決済番号 = dr["TP決済番号", version] as string;
            決済日 = dr["決済日", version] as DateTime?;
            売上履歴ID = dr["売上履歴ID", version] as long?;
            売上日時 = dr["売上日時", version] as DateTime?;
            伝票番号 = dr["伝票番号", version] as int?;
            承認番号 = dr["承認番号", version] as string;
            商品区分コード = dr["商品区分コード", version] as string;
            売上額 = dr["売上額", version] as decimal?;
            税額 = dr["税額", version] as decimal?;
            割引後売上額 = dr["割引後売上額", version] as decimal?;
            取消済フラグ = dr["取消済フラグ", version] as bool?;
            クレジットカード番号下4桁 = dr["クレジットカード番号下4桁", version] as string;
            トランスミッション日時 = dr["トランスミッション日時", version] as DateTime?;
            取消元売上履歴ID = dr["取消元売上履歴ID", version] as long?;
            取消元伝票番号 = dr["取消元伝票番号", version] as int?;
            取消元トランスミッション日時 = dr["取消元トランスミッション日時", version] as DateTime?;
            売上種別ID = dr["売上種別ID", version] as int?;
            決済結果ID = dr["決済結果ID", version] as int?;
            キャンセル可能フラグ = dr["キャンセル可能フラグ", version] as bool?;
            トレーニングモード実行フラグ = dr["トレーニングモード実行フラグ", version] as bool?;
            レシートUUID = dr["レシートUUID", version] as string;
            再印刷可能フラグ = dr["再印刷可能フラグ", version] as bool?;
            電子マネー売上履歴ID = dr["電子マネー売上履歴ID", version] as long?;
            電子マネーブランドID = dr["電子マネーブランドID", version] as int?;
            電子マネーブランド名 = dr["電子マネーブランド名", version] as string;
            電子マネーステータスID = dr["電子マネーステータスID", version] as int?;
            電子マネー残高照会結果ID = dr["電子マネー残高照会結果ID", version] as int?;
            電子マネー上位端末ID = dr["電子マネー上位端末ID", version] as string;
            電子マネー決済ID = dr["電子マネー決済ID", version] as int?;
            電子マネーカード番号 = dr["電子マネーカード番号", version] as string;
            電子マネーエラーコード = dr["電子マネーエラーコード", version] as int?;

            if (dr.Table.Columns.Contains("売上種別名"))
            {
                売上種別名 = dr["売上種別名", version] as string;
            }

            if (dr.Table.Columns.Contains("決済結果名"))
            {
                決済結果名 = dr["決済結果名", version] as string;
            }

            if (dr.Table.Columns.Contains("電子マネーステータス名"))
            {
                電子マネーステータス名 = dr["電子マネーステータス名", version] as string;
            }

            if (dr.Table.Columns.Contains("電子マネー残高照会結果名"))
            {
                電子マネー残高照会結果名 = dr["電子マネー残高照会結果名", version] as string;
            }

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

        public long? 売上履歴ID
        {
            get { return _売上履歴ID; }
            set { _売上履歴ID = value; }
        }

        public DateTime? 売上日時
        {
            get { return _売上日時; }
            set { _売上日時 = value; }
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

        public string 商品区分コード
        {
            get { return _商品区分コード; }
            set { _商品区分コード = value; }
        }

        public decimal? 売上額
        {
            get { return _売上額; }
            set { _売上額 = value; }
        }

        public decimal? 税額
        {
            get { return _税額; }
            set { _税額 = value; }
        }

        public decimal? 割引後売上額
        {
            get { return _割引後売上額; }
            set { _割引後売上額 = value; }
        }

        public bool? 取消済フラグ
        {
            get { return _取消済フラグ; }
            set { _取消済フラグ = value; }
        }

        public string クレジットカード番号下4桁
        {
            get { return _クレジットカード番号下4桁; }
            set { _クレジットカード番号下4桁 = value; }
        }

        public DateTime? トランスミッション日時
        {
            get { return _トランスミッション日時; }
            set { _トランスミッション日時 = value; }
        }

        public long? 取消元売上履歴ID
        {
            get { return _取消元売上履歴ID; }
            set { _取消元売上履歴ID = value; }
        }

        public int? 取消元伝票番号
        {
            get { return _取消元伝票番号; }
            set { _取消元伝票番号 = value; }
        }

        public DateTime? 取消元トランスミッション日時
        {
            get { return _取消元トランスミッション日時; }
            set { _取消元トランスミッション日時 = value; }
        }

        public int? 売上種別ID
        {
            get { return _売上種別ID; }
            set { _売上種別ID = value; }
        }

        public int? 決済結果ID
        {
            get { return _決済結果ID; }
            set { _決済結果ID = value; }
        }

        public bool? キャンセル可能フラグ
        {
            get { return _キャンセル可能フラグ; }
            set { _キャンセル可能フラグ = value; }
        }

        public bool? トレーニングモード実行フラグ
        {
            get { return _トレーニングモード実行フラグ; }
            set { _トレーニングモード実行フラグ = value; }
        }

        public string レシートUUID
        {
            get { return _レシートUUID; }
            set { _レシートUUID = value; }
        }

        public bool? 再印刷可能フラグ
        {
            get { return _再印刷可能フラグ; }
            set { _再印刷可能フラグ = value; }
        }

        public long? 電子マネー売上履歴ID
        {
            get { return _電子マネー売上履歴ID; }
            set { _電子マネー売上履歴ID = value; }
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

        public int? 電子マネーステータスID
        {
            get { return _電子マネーステータスID; }
            set { _電子マネーステータスID = value; }
        }

        public int? 電子マネー残高照会結果ID
        {
            get { return _電子マネー残高照会結果ID; }
            set { _電子マネー残高照会結果ID = value; }
        }

        public string 電子マネー上位端末ID
        {
            get { return _電子マネー上位端末ID; }
            set { _電子マネー上位端末ID = value; }
        }

        public int? 電子マネー決済ID
        {
            get { return _電子マネー決済ID; }
            set { _電子マネー決済ID = value; }
        }

        public string 電子マネーカード番号
        {
            get { return _電子マネーカード番号; }
            set { _電子マネーカード番号 = value; }
        }

        public int? 電子マネーエラーコード
        {
            get { return _電子マネーエラーコード; }
            set { _電子マネーエラーコード = value; }
        }

        public string 売上種別名 { get; set; }

        public string 決済結果名 { get; set; }

        public string 電子マネーステータス名 { get; set; }

        public string 電子マネー残高照会結果名 { get; set; }

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
            string sql = "\r\n INSERT INTO TP売上履歴(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,TP決済番号" +
                         "\r\n     ,決済日" +
                         "\r\n     ,売上履歴ID" +
                         "\r\n     ,売上日時" +
                         "\r\n     ,伝票番号" +
                         "\r\n     ,承認番号" +
                         "\r\n     ,商品区分コード" +
                         "\r\n     ,売上額" +
                         "\r\n     ,税額" +
                         "\r\n     ,割引後売上額" +
                         "\r\n     ,取消済フラグ" +
                         "\r\n     ,クレジットカード番号下4桁" +
                         "\r\n     ,トランスミッション日時" +
                         "\r\n     ,取消元売上履歴ID" +
                         "\r\n     ,取消元伝票番号" +
                         "\r\n     ,取消元トランスミッション日時" +
                         "\r\n     ,売上種別ID" +
                         "\r\n     ,決済結果ID" +
                         "\r\n     ,キャンセル可能フラグ" +
                         "\r\n     ,トレーニングモード実行フラグ" +
                         "\r\n     ,レシートUUID" +
                         "\r\n     ,再印刷可能フラグ" +
                         "\r\n     ,電子マネー売上履歴ID" +
                         "\r\n     ,電子マネーブランドID" +
                         "\r\n     ,電子マネーブランド名" +
                         "\r\n     ,電子マネーステータスID" +
                         "\r\n     ,電子マネー残高照会結果ID" +
                         "\r\n     ,電子マネー上位端末ID" +
                         "\r\n     ,電子マネー決済ID" +
                         "\r\n     ,電子マネーカード番号" +
                         "\r\n     ,電子マネーエラーコード" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@TP決済番号" +
                         "\r\n     ,@決済日" +
                         "\r\n     ,@売上履歴ID" +
                         "\r\n     ,@売上日時" +
                         "\r\n     ,@伝票番号" +
                         "\r\n     ,@承認番号" +
                         "\r\n     ,@商品区分コード" +
                         "\r\n     ,@売上額" +
                         "\r\n     ,@税額" +
                         "\r\n     ,@割引後売上額" +
                         "\r\n     ,@取消済フラグ" +
                         "\r\n     ,@クレジットカード番号下4桁" +
                         "\r\n     ,@トランスミッション日時" +
                         "\r\n     ,@取消元売上履歴ID" +
                         "\r\n     ,@取消元伝票番号" +
                         "\r\n     ,@取消元トランスミッション日時" +
                         "\r\n     ,@売上種別ID" +
                         "\r\n     ,@決済結果ID" +
                         "\r\n     ,@キャンセル可能フラグ" +
                         "\r\n     ,@トレーニングモード実行フラグ" +
                         "\r\n     ,@レシートUUID" +
                         "\r\n     ,@再印刷可能フラグ" +
                         "\r\n     ,@電子マネー売上履歴ID" +
                         "\r\n     ,@電子マネーブランドID" +
                         "\r\n     ,@電子マネーブランド名" +
                         "\r\n     ,@電子マネーステータスID" +
                         "\r\n     ,@電子マネー残高照会結果ID" +
                         "\r\n     ,@電子マネー上位端末ID" +
                         "\r\n     ,@電子マネー決済ID" +
                         "\r\n     ,@電子マネーカード番号" +
                         "\r\n     ,@電子マネーエラーコード" +
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
            string sql = "\r\n UPDATE TP売上履歴" +
                         "\r\n SET 売上履歴ID                    = @売上履歴ID" +
                         "\r\n     ,売上日時                     = @売上日時" +
                         "\r\n     ,伝票番号                     = @伝票番号" +
                         "\r\n     ,承認番号                     = @承認番号" +
                         "\r\n     ,商品区分コード               = @商品区分コード" +
                         "\r\n     ,売上額                       = @売上額" +
                         "\r\n     ,税額                         = @税額" +
                         "\r\n     ,割引後売上額                 = @割引後売上額" +
                         "\r\n     ,取消済フラグ                 = @取消済フラグ" +
                         "\r\n     ,クレジットカード番号下4桁    = @クレジットカード番号下4桁" +
                         "\r\n     ,トランスミッション日時       = @トランスミッション日時" +
                         "\r\n     ,取消元売上履歴ID             = @取消元売上履歴ID" +
                         "\r\n     ,取消元伝票番号               = @取消元伝票番号" +
                         "\r\n     ,取消元トランスミッション日時 = @取消元トランスミッション日時" +
                         "\r\n     ,売上種別ID                   = @売上種別ID" +
                         "\r\n     ,決済結果ID                   = @決済結果ID" +
                         "\r\n     ,キャンセル可能フラグ         = @キャンセル可能フラグ" +
                         "\r\n     ,トレーニングモード実行フラグ = @トレーニングモード実行フラグ" +
                         "\r\n     ,レシートUUID                 = @レシートUUID" +
                         "\r\n     ,再印刷可能フラグ             = @再印刷可能フラグ" +
                         "\r\n     ,電子マネー売上履歴ID         = @電子マネー売上履歴ID" +
                         "\r\n     ,電子マネーブランドID         = @電子マネーブランドID" +
                         "\r\n     ,電子マネーブランド名         = @電子マネーブランド名" +
                         "\r\n     ,電子マネーステータスID       = @電子マネーステータスID" +
                         "\r\n     ,電子マネー残高照会結果ID     = @電子マネー残高照会結果ID" +
                         "\r\n     ,電子マネー上位端末ID         = @電子マネー上位端末ID" +
                         "\r\n     ,電子マネー決済ID             = @電子マネー決済ID" +
                         "\r\n     ,電子マネーカード番号         = @電子マネーカード番号" +
                         "\r\n     ,電子マネーエラーコード       = @電子マネーエラーコード" +
                         "\r\n     ,修正担当者ＣＤ               = @修正担当者ＣＤ" +
                         "\r\n     ,修正日                       = getdate()" +
                         "\r\n WHERE 企業ＣＤ                    = @企業ＣＤ" +
                         "\r\n   AND TP決済番号                  = @TP決済番号" +
                         "\r\n   AND 決済日                      = @決済日" +
                         "\r\n   AND 修正日                      = @修正日";

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
            string sql = "\r\n DELETE TP売上履歴" +
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

                    if (売上履歴ID != null)
                    {
                        cmd.Parameters.Add("@売上履歴ID", SqlDbType.BigInt).Value = 売上履歴ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上履歴ID", SqlDbType.BigInt).Value = DBNull.Value;
                    }

                    if (売上日時 != null)
                    {
                        cmd.Parameters.Add("@売上日時", SqlDbType.DateTime).Value = 売上日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上日時", SqlDbType.DateTime).Value = DBNull.Value;
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

                    if (商品区分コード != null)
                    {
                        cmd.Parameters.Add("@商品区分コード", SqlDbType.VarChar).Value = 商品区分コード;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品区分コード", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (売上額 != null)
                    {
                        cmd.Parameters.Add("@売上額", SqlDbType.Decimal).Value = 売上額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (税額 != null)
                    {
                        cmd.Parameters.Add("@税額", SqlDbType.Decimal).Value = 税額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@税額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (割引後売上額 != null)
                    {
                        cmd.Parameters.Add("@割引後売上額", SqlDbType.Decimal).Value = 割引後売上額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@割引後売上額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (取消済フラグ != null)
                    {
                        cmd.Parameters.Add("@取消済フラグ", SqlDbType.Bit).Value = 取消済フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取消済フラグ", SqlDbType.Bit).Value = DBNull.Value;
                    }

                    if (クレジットカード番号下4桁 != null)
                    {
                        cmd.Parameters.Add("@クレジットカード番号下4桁", SqlDbType.VarChar).Value = クレジットカード番号下4桁;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレジットカード番号下4桁", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (トランスミッション日時 != null)
                    {
                        cmd.Parameters.Add("@トランスミッション日時", SqlDbType.DateTime).Value = トランスミッション日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@トランスミッション日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (取消元売上履歴ID != null)
                    {
                        cmd.Parameters.Add("@取消元売上履歴ID", SqlDbType.BigInt).Value = 取消元売上履歴ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取消元売上履歴ID", SqlDbType.BigInt).Value = DBNull.Value;
                    }

                    if (取消元伝票番号 != null)
                    {
                        cmd.Parameters.Add("@取消元伝票番号", SqlDbType.Int).Value = 取消元伝票番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取消元伝票番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (取消元トランスミッション日時 != null)
                    {
                        cmd.Parameters.Add("@取消元トランスミッション日時", SqlDbType.DateTime).Value = 取消元トランスミッション日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取消元トランスミッション日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (売上種別ID != null)
                    {
                        cmd.Parameters.Add("@売上種別ID", SqlDbType.Int).Value = 売上種別ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上種別ID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (決済結果ID != null)
                    {
                        cmd.Parameters.Add("@決済結果ID", SqlDbType.Int).Value = 決済結果ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@決済結果ID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (キャンセル可能フラグ != null)
                    {
                        cmd.Parameters.Add("@キャンセル可能フラグ", SqlDbType.Bit).Value = キャンセル可能フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@キャンセル可能フラグ", SqlDbType.Bit).Value = DBNull.Value;
                    }

                    if (トレーニングモード実行フラグ != null)
                    {
                        cmd.Parameters.Add("@トレーニングモード実行フラグ", SqlDbType.Bit).Value = トレーニングモード実行フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@トレーニングモード実行フラグ", SqlDbType.Bit).Value = DBNull.Value;
                    }

                    if (レシートUUID != null)
                    {
                        cmd.Parameters.Add("@レシートUUID", SqlDbType.VarChar).Value = レシートUUID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@レシートUUID", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (再印刷可能フラグ != null)
                    {
                        cmd.Parameters.Add("@再印刷可能フラグ", SqlDbType.Bit).Value = 再印刷可能フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@再印刷可能フラグ", SqlDbType.Bit).Value = DBNull.Value;
                    }

                    if (電子マネー売上履歴ID != null)
                    {
                        cmd.Parameters.Add("@電子マネー売上履歴ID", SqlDbType.BigInt).Value = 電子マネー売上履歴ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネー売上履歴ID", SqlDbType.BigInt).Value = DBNull.Value;
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

                    if (電子マネーステータスID != null)
                    {
                        cmd.Parameters.Add("@電子マネーステータスID", SqlDbType.Int).Value = 電子マネーステータスID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネーステータスID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電子マネー残高照会結果ID != null)
                    {
                        cmd.Parameters.Add("@電子マネー残高照会結果ID", SqlDbType.Int).Value = 電子マネー残高照会結果ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネー残高照会結果ID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電子マネー上位端末ID != null)
                    {
                        cmd.Parameters.Add("@電子マネー上位端末ID", SqlDbType.VarChar).Value = 電子マネー上位端末ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネー上位端末ID", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電子マネー決済ID != null)
                    {
                        cmd.Parameters.Add("@電子マネー決済ID", SqlDbType.Int).Value = 電子マネー決済ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネー決済ID", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (電子マネーカード番号 != null)
                    {
                        cmd.Parameters.Add("@電子マネーカード番号", SqlDbType.VarChar).Value = 電子マネーカード番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネーカード番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電子マネーエラーコード != null)
                    {
                        cmd.Parameters.Add("@電子マネーエラーコード", SqlDbType.Int).Value = 電子マネーエラーコード;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電子マネーエラーコード", SqlDbType.Int).Value = DBNull.Value;
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

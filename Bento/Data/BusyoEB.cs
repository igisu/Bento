using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 部署EB設定マスタ
    /// </summary>
    public class BusyoEB : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="bankCD">銀行ＣＤ</param>
        /// <param name="branchCD">支店ＣＤ</param>
        /// <returns></returns>
        public static BusyoEB GetBusyoEB(int busyoCD, string bankCD, string branchCD)
        {
            BusyoEB busyoEB = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ AS 部署ＣＤorg" +
                                  "\r\n     ,A.銀行ＣＤ" +
                                  "\r\n     ,A.銀行ＣＤ AS 銀行ＣＤorg" +
                                  "\r\n     ,K.銀行名" +
                                  "\r\n     ,K.銀行名カナ" +
                                  "\r\n     ,A.支店ＣＤ" +
                                  "\r\n     ,A.支店ＣＤ AS 支店ＣＤorg" +
                                  "\r\n     ,KB.支店名" +
                                  "\r\n     ,KB.支店名カナ" +
                                  "\r\n     ,A.委託者コード" +
                                  "\r\n     ,A.委託者名" +
                                  "\r\n     ,A.預金種目" +
                                  "\r\n     ,A.口座番号" +
                                  "\r\n     ,A.引落日" +
                                  "\r\n     ,A.EBセット名" +
                                  "\r\n     ,A.EBセット名 AS EBセット名org" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     部署EB設定マスタ AS A" +
                                  "\r\n     LEFT JOIN 金融機関名称 AS K" +
                                  "\r\n         ON A.銀行ＣＤ = K.銀行ＣＤ" +
                                  "\r\n     LEFT JOIN 金融機関支店名称 AS KB" +
                                  "\r\n         ON A.銀行ＣＤ = KB.銀行ＣＤ" +
                                  "\r\n         AND A.支店ＣＤ = KB.支店ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                  "\r\n     AND A.銀行ＣＤ = @銀行ＣＤ" +
                                  "\r\n     AND A.支店ＣＤ = @支店ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                cmd.Parameters.Add("@銀行ＣＤ", SqlDbType.VarChar).Value = bankCD;
                cmd.Parameters.Add("@支店ＣＤ", SqlDbType.VarChar).Value = branchCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("部署EB設定マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    busyoEB = new BusyoEB(dt.Rows[0]);
                }
            }

            return busyoEB;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private int? _部署ＣＤorg;          // レコード読込直後の値（更新・削除時に使用する）
        private string _銀行ＣＤ;
        private string _銀行ＣＤorg;        // レコード読込直後の値（更新・削除時に使用する）
        private string _支店ＣＤ;
        private string _支店ＣＤorg;        // レコード読込直後の値（更新・削除時に使用する）
        private string _委託者コード;
        private string _委託者名;
        private int? _預金種目;
        private string _口座番号;
        private int? _引落日;
        private string _EBセット名;
        private string _EBセット名org;      // レコード読込直後の値（更新・削除時に使用する）

        private string _銀行名;
        private string _銀行名カナ;
        private string _支店名;
        private string _支店名カナ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BusyoEB()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public BusyoEB(BusyoEB src)
        {
            企業ＣＤ = src.企業ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            部署ＣＤorg = src.部署ＣＤorg;
            銀行ＣＤ = src.銀行ＣＤ;
            銀行ＣＤorg = src.銀行ＣＤorg;
            支店ＣＤ = src.支店ＣＤ;
            支店ＣＤorg = src.支店ＣＤorg;
            委託者コード = src.委託者コード;
            委託者名 = src.委託者名;
            預金種目 = src.預金種目;
            口座番号 = src.口座番号;
            引落日 = src.引落日;
            EBセット名 = src.EBセット名;
            EBセット名org = src.EBセット名org;

            銀行名 = src.銀行名;
            銀行名カナ = src.銀行名カナ;
            支店名 = src.支店名;
            支店名カナ = src.支店名カナ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public BusyoEB(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("部署ＣＤorg"))
            {
                部署ＣＤorg = dr["部署ＣＤorg", version] as int?;
            }
            else
            {
                部署ＣＤorg = dr["部署ＣＤ", version] as int?;
            }

            銀行ＣＤ = dr["銀行ＣＤ", version] as string;

            if (dr.Table.Columns.Contains("銀行ＣＤorg"))
            {
                銀行ＣＤorg = dr["銀行ＣＤorg", version] as string;
            }
            else
            {
                銀行ＣＤorg = dr["銀行ＣＤ", version] as string;
            }

            支店ＣＤ = dr["支店ＣＤ", version] as string;

            if (dr.Table.Columns.Contains("支店ＣＤorg"))
            {
                支店ＣＤorg = dr["支店ＣＤorg", version] as string;
            }
            else
            {
                支店ＣＤorg = dr["支店ＣＤ", version] as string;
            }

            委託者コード = dr["委託者コード", version] as string;
            委託者名 = dr["委託者名", version] as string;
            預金種目 = dr["預金種目", version] as int?;
            口座番号 = dr["口座番号", version] as string;
            引落日 = dr["引落日", version] as int?;
            EBセット名 = dr["EBセット名", version] as string;

            if (dr.Table.Columns.Contains("EBセット名org"))
            {
                EBセット名org = dr["EBセット名org", version] as string;
            }
            else
            {
                EBセット名org = dr["EBセット名", version] as string;
            }

            if (dr.Table.Columns.Contains("銀行名"))
            {
                銀行名 = dr["銀行名", version] as string;
            }

            if (dr.Table.Columns.Contains("銀行名カナ"))
            {
                銀行名カナ = dr["銀行名カナ", version] as string;
            }

            if (dr.Table.Columns.Contains("支店名"))
            {
                支店名 = dr["支店名", version] as string;
            }

            if (dr.Table.Columns.Contains("支店名カナ"))
            {
                支店名カナ = dr["支店名カナ", version] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 部署ＣＤorg
        {
            get { return _部署ＣＤorg; }
            set { _部署ＣＤorg = value; }
        }

        public string 銀行ＣＤ
        {
            get { return _銀行ＣＤ; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _銀行ＣＤ = value;
                _errors.Remove("銀行ＣＤ");
                支店ＣＤ = null;    // 支店名を消す
                if (value == null)
                {
                    _errors["銀行ＣＤ"] = "銀行ＣＤを入力してください。";
                }
            }
        }

        public string 銀行ＣＤorg
        {
            get { return _銀行ＣＤorg; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _銀行ＣＤorg = value;
            }
        }

        public string 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _支店ＣＤ = value;
                _errors.Remove("支店ＣＤ");
                if (value == null)
                {
                    _errors["支店ＣＤ"] = "支店ＣＤを入力してください。";
                }
            }
        }

        public string 支店ＣＤorg
        {
            get { return _支店ＣＤorg; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _支店ＣＤorg = value;
            }
        }

        public string 委託者コード
        {
            get { return _委託者コード; }
            set
            {
                _委託者コード = value;
                _errors.Remove("委託者コード");
                if (value == null)
                {
                    _errors["委託者コード"] = "委託者コードを入力してください。";
                }
            }
        }

        public string 委託者名
        {
            get { return _委託者名; }
            set
            {
                _委託者名 = value;
                _errors.Remove("委託者名");
                if (value == null)
                {
                    _errors["委託者名"] = "委託者名を入力してください。";
                }
                else
                {
                    if (Regex.IsMatch(value, @"^[0-9A-Zｦ-ﾟ\.\(\)\- ]*$") == false)  // 「ｦ」～「ｰ」は使えない
                    {
                        _errors["委託者名"] = "委託者名は半角英数か半角カタカナで入力してください。";
                    }
                }
            }
        }

        public int? 預金種目
        {
            get { return _預金種目; }
            set
            {
                _預金種目 = value;
                _errors.Remove("預金種目");
                if (value == null || value < 0)
                {
                    _errors["預金種目"] = "預金種目を入力してください。";
                }
            }
        }

        public string 口座番号
        {
            get { return _口座番号; }
            set
            {
                _口座番号 = value;
                _errors.Remove("口座番号");
                if (value == null)
                {
                    _errors["口座番号"] = "口座番号を入力してください。";
                }
            }
        }

        public int? 引落日
        {
            get { return _引落日; }
            set
            {
                _引落日 = value;
                _errors.Remove("引落日");
                if (value != null)
                {
                    if (0 <= value && value <= 28)
                    {
                    }
                    else if (value == 29 || value == 30 || value == 31)
                    {
                        _errors["引落日"] = "月末の場合は99を入力してください。";
                    }
                    else if (value == 99)
                    {
                    }
                    else
                    {
                        _errors["引落日"] = "1～28 または 月末の場合は99を入力してください。";
                    }
                }
            }
        }

        public string EBセット名
        {
            get { return _EBセット名; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _EBセット名 = value;
                _errors.Remove("EBセット名");
                if (value == null)
                {
                    _errors["EBセット名"] = "EBセット名を入力してください。";
                }
            }
        }

        public string EBセット名org
        {
            get { return _EBセット名org; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }
                _EBセット名org = value;
            }
        }

        public string 銀行名
        {
            get { return _銀行名; }
            set { _銀行名 = value; }
        }

        public string 銀行名カナ
        {
            get { return _銀行名カナ; }
            set { _銀行名カナ = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public string 支店名カナ
        {
            get { return _支店名カナ; }
            set { _支店名カナ = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 銀行ＣＤ=[{1}] 支店ＣＤ=[{2}]",
                                        (部署ＣＤ == 部署ＣＤorg ? 部署ＣＤ : 部署ＣＤorg ?? 部署ＣＤ),
                                        (銀行ＣＤ == 銀行ＣＤorg ? 銀行ＣＤ : 銀行ＣＤorg ?? 銀行ＣＤ),
                                        (支店ＣＤ == 支店ＣＤorg ? 支店ＣＤ : 支店ＣＤorg ?? 支店ＣＤ)
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
        /// プロパティの値をマップに格納します。
        /// </summary>
        /// <param name="valueMap"></param>
        public void GetProperties(Dictionary<string, string> valueMap)
        {
            valueMap["部署ＣＤ"] = Convert.ToString(部署ＣＤ);
            valueMap["銀行ＣＤ"] = 銀行ＣＤ;
            valueMap["支店ＣＤ"] = 支店ＣＤ;
            valueMap["委託者コード"] = 委託者コード;
            valueMap["委託者名"] = 委託者名;
            valueMap["預金種目"] = Convert.ToString(預金種目);
            valueMap["口座番号"] = 口座番号;
            valueMap["EBセット名"] = EBセット名;
            valueMap["銀行名"] = 銀行名;
            valueMap["銀行名カナ"] = 銀行名カナ;
            valueMap["支店名"] = 支店名;
            valueMap["支店名カナ"] = 支店名カナ;
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(BusyoEB target)
        {
            if (銀行ＣＤ == target.銀行ＣＤ &&
                支店ＣＤ == target.支店ＣＤ &&
                委託者コード == target.委託者コード &&
                委託者名 == target.委託者名 &&
                (預金種目 ?? -1) == (target.預金種目 ?? -1) &&
                口座番号 == target.口座番号 &&
                引落日 == target.引落日 &&
                EBセット名 == target.EBセット名)
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
            string sql = "\r\n INSERT INTO 部署EB設定マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,銀行ＣＤ" +
                         "\r\n     ,支店ＣＤ" +
                         "\r\n     ,委託者コード" +
                         "\r\n     ,委託者名" +
                         "\r\n     ,預金種目" +
                         "\r\n     ,口座番号" +
                         "\r\n     ,引落日" +
                         "\r\n     ,EBセット名" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@銀行ＣＤ" +
                         "\r\n     ,@支店ＣＤ" +
                         "\r\n     ,@委託者コード" +
                         "\r\n     ,@委託者名" +
                         "\r\n     ,@預金種目" +
                         "\r\n     ,@口座番号" +
                         "\r\n     ,@引落日" +
                         "\r\n     ,@EBセット名" +
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
            string sql = "\r\n UPDATE 部署EB設定マスタ" +
                         "\r\n SET  企業ＣＤ       = @企業ＣＤ" +
                         "\r\n     ,部署ＣＤ       = @部署ＣＤ" +
                         "\r\n     ,銀行ＣＤ       = @銀行ＣＤ" +
                         "\r\n     ,支店ＣＤ       = @支店ＣＤ" +
                         "\r\n     ,委託者コード   = @委託者コード" +
                         "\r\n     ,委託者名       = @委託者名" +
                         "\r\n     ,預金種目       = @預金種目" +
                         "\r\n     ,口座番号       = @口座番号" +
                         "\r\n     ,引落日         = @引落日" +
                         "\r\n     ,EBセット名     = @EBセット名" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = @修正日" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤorg" +
                         "\r\n   AND 銀行ＣＤ = @銀行ＣＤorg" +
                         "\r\n   AND 支店ＣＤ = @支店ＣＤorg" +
                         "\r\n   AND EBセット名 = @EBセット名org";

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
            string sql = "\r\n DELETE FROM 部署EB設定マスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤorg" +
                         "\r\n   AND 銀行ＣＤ = @銀行ＣＤorg" +
                         "\r\n   AND 支店ＣＤ = @支店ＣＤorg" +
                         "\r\n   AND EBセット名 = @EBセット名org";

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

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤorg", SqlDbType.Int).Value = 部署ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (銀行ＣＤ != null)
                    {
                        cmd.Parameters.Add("@銀行ＣＤ", SqlDbType.VarChar).Value = 銀行ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@銀行ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (銀行ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@銀行ＣＤorg", SqlDbType.VarChar).Value = 銀行ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@銀行ＣＤorg", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (支店ＣＤ != null)
                    {
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.VarChar).Value = 支店ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (支店ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@支店ＣＤorg", SqlDbType.VarChar).Value = 支店ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支店ＣＤorg", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (委託者コード != null)
                    {
                        cmd.Parameters.Add("@委託者コード", SqlDbType.VarChar).Value = 委託者コード;
                    }
                    else
                    {
                        cmd.Parameters.Add("@委託者コード", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (委託者名 != null)
                    {
                        cmd.Parameters.Add("@委託者名", SqlDbType.VarChar).Value = 委託者名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@委託者名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (預金種目 != null && 預金種目 != -1)
                    {
                        cmd.Parameters.Add("@預金種目", SqlDbType.Int).Value = 預金種目;
                    }
                    else
                    {
                        cmd.Parameters.Add("@預金種目", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (口座番号 != null)
                    {
                        cmd.Parameters.Add("@口座番号", SqlDbType.VarChar).Value = 口座番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@口座番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (引落日 != null)
                    {
                        cmd.Parameters.Add("@引落日", SqlDbType.Int).Value = 引落日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@引落日", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (EBセット名 != null)
                    {
                        cmd.Parameters.Add("@EBセット名", SqlDbType.VarChar).Value = EBセット名;
                    }
                    else
                    {
                        // セット名が未入力の場合は、銀行名と支店名をつなげたもので代用する（20文字までに限定）
                        string setName = string.Format("{0}({1})", 銀行名, 支店名);
                        int len = Math.Min(setName.Length, 20);
                        cmd.Parameters.Add("@EBセット名", SqlDbType.VarChar).Value = setName.Substring(0, len);
                    }

                    if (EBセット名org != null)
                    {
                        cmd.Parameters.Add("@EBセット名org", SqlDbType.VarChar).Value = EBセット名org;
                    }
                    else
                    {
                        cmd.Parameters.Add("@EBセット名org", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("部署EB設定マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "部署EB設定マスタが重複しています。";
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

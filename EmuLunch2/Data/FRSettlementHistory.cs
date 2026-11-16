using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// FRクレカ決済履歴
    /// </summary>
    public class FRSettlementHistory : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド

        /// <summary>
        /// 履歴番号の最大値+1を取得
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EmuException"></exception>
        public static decimal GetNextNo()
        {
            decimal tranNo = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT ISNULL(MAX(履歴番号), 0) AS 履歴番号 FROM FR_T_クレカ決済履歴 WHERE 企業ＣＤ = @企業ＣＤ";
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        tranNo = (decimal)reader["履歴番号"];
                    }
                }

                return ++tranNo;
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

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private decimal? _履歴番号;
        private decimal? _請求番号;
        private DateTime? _請求日;
        private int? _部署ＣＤ;
        private int? _請求先ＣＤ;
        private string _請求先名;
        private decimal? _今回請求額;
        private int? _処理タイプ;
        private int? _処理結果;
        private string _処理結果内容;
        private string _店舗ID;
        private string _伝票番号;
        private string _承認番号;
        private string _取引番号;
        private string _クレカ顧客番号;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FRSettlementHistory()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public FRSettlementHistory(FRSettlementHistory src)
        {
            履歴番号 = src.履歴番号;
            請求番号 = src.請求番号;
            請求日 = src.請求日;
            部署ＣＤ = src.部署ＣＤ;
            請求先ＣＤ = src.請求先ＣＤ;
            請求先名 = src.請求先名;
            今回請求額 = src.今回請求額;
            処理タイプ = src.処理タイプ;
            処理結果 = src.処理結果;
            処理結果内容 = src.処理結果内容;
            店舗ID = src.店舗ID;
            伝票番号 = src.伝票番号;
            承認番号 = src.承認番号;
            取引番号 = src.取引番号;
            クレカ顧客番号 = src.クレカ顧客番号;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public FRSettlementHistory(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            履歴番号 = dr["履歴番号", version] as decimal?;
            請求番号 = dr["請求番号", version] as decimal?;
            請求日 = dr["請求日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            請求先ＣＤ = dr["請求先ＣＤ", version] as int?;
            請求先名 = dr["請求先名", version] as string;
            今回請求額 = dr["今回請求額", version] as decimal?;
            処理タイプ = dr["処理タイプ", version] as int?;
            処理結果 = dr["処理結果", version] as int?;
            処理結果内容 = dr["処理結果内容", version] as string;
            店舗ID = dr["店舗ID", version] as string;
            伝票番号 = dr["伝票番号", version] as string;
            承認番号 = dr["承認番号", version] as string;
            取引番号 = dr["取引番号", version] as string;
            クレカ顧客番号 = dr["クレカ顧客番号", version] as string;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public decimal? 履歴番号
        {
            get { return _履歴番号; }
            set { _履歴番号 = value; }
        }

        public decimal? 請求番号
        {
            get { return _請求番号; }
            set { _請求番号 = value; }
        }

        public DateTime? 請求日
        {
            get { return _請求日; }
            set { _請求日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set { _請求先ＣＤ = value; }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
        }

        public int? 処理タイプ
        {
            get { return _処理タイプ; }
            set { _処理タイプ = value; }
        }

        public int? 処理結果
        {
            get { return _処理結果; }
            set { _処理結果 = value; }
        }

        public string 処理結果内容
        {
            get { return _処理結果内容; }
            set { _処理結果内容 = value; }
        }

        public string 店舗ID
        {
            get { return _店舗ID; }
            set { _店舗ID = value; }
        }

        public string 伝票番号
        {
            get { return _伝票番号; }
            set { _伝票番号 = value; }
        }

        public string 承認番号
        {
            get { return _承認番号; }
            set { _承認番号 = value; }
        }

        public string 取引番号
        {
            get { return _取引番号; }
            set { _取引番号 = value; }
        }

        public string クレカ顧客番号
        {
            get { return _クレカ顧客番号; }
            set { _クレカ顧客番号 = value; }
        }

        public string 処理タイプ名
        {
            get
            {
                if (処理タイプ == null)
                {
                    return "";
                }

                return 処理タイプ == (int)Consts.FRProcessType.Settle ? "決済" : "取消";
            }
        }

        public string 処理結果名
        {
            get
            {
                if (処理結果 == null)
                {
                    return "";
                }

                return 処理結果 == (int)Consts.FRProcessResult.Success ? "成功" : "失敗";
            }
        }

        public override string Code
        {
            get { return string.Format("履歴番号=[{0}]", 履歴番号); }
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
        public bool Equals(FRSettlementHistory target)
        {
            if (履歴番号 == target.履歴番号 &&
                請求番号 == target.請求番号 &&
                請求日 == target.請求日 &&
                部署ＣＤ == target.部署ＣＤ &&
                請求先ＣＤ == target.請求先ＣＤ &&
                請求先名 == target.請求先名 &&
                今回請求額 == target.今回請求額 &&
                処理タイプ == target.処理タイプ &&
                処理結果 == target.処理結果 &&
                処理結果内容 == target.処理結果内容 &&
                店舗ID == target.店舗ID &&
                伝票番号 == target.伝票番号 &&
                承認番号 == target.承認番号 &&
                取引番号 == target.取引番号 &&
                クレカ顧客番号 == target.クレカ顧客番号)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 指定した Object が、現在の Object と一致するかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と一致する場合は true。それ以外の場合は false。</returns>
        /// <remarks>中間サーバーの決済履歴との比較用</remarks>
        public bool IsMatch(FRSettlementHistory target)
        {
            // 履歴番号は中間サーバーと異なるので比較しない

            if (請求番号 == target.請求番号 &&
                請求日 == target.請求日 &&
                部署ＣＤ == target.部署ＣＤ &&
                請求先ＣＤ == target.請求先ＣＤ &&
                請求先名 == target.請求先名 &&
                今回請求額 == target.今回請求額 &&
                処理タイプ == target.処理タイプ &&
                処理結果 == target.処理結果 &&
                処理結果内容 == target.処理結果内容 &&
                店舗ID == target.店舗ID &&
                伝票番号 == target.伝票番号 &&
                承認番号 == target.承認番号 &&
                取引番号 == target.取引番号 &&
                クレカ顧客番号 == target.クレカ顧客番号)
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
            string sql = "\r\n INSERT INTO FR_T_クレカ決済履歴(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,履歴番号" +
                         "\r\n     ,請求番号" +
                         "\r\n     ,請求日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,請求先ＣＤ" +
                         "\r\n     ,請求先名" +
                         "\r\n     ,今回請求額" +
                         "\r\n     ,処理タイプ" +
                         "\r\n     ,処理結果" +
                         "\r\n     ,処理結果内容" +
                         "\r\n     ,店舗ID" +
                         "\r\n     ,伝票番号" +
                         "\r\n     ,承認番号" +
                         "\r\n     ,取引番号" +
                         "\r\n     ,クレカ顧客番号" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@履歴番号" +
                         "\r\n     ,@請求番号" +
                         "\r\n     ,@請求日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@請求先ＣＤ" +
                         "\r\n     ,@請求先名" +
                         "\r\n     ,@今回請求額" +
                         "\r\n     ,@処理タイプ" +
                         "\r\n     ,@処理結果" +
                         "\r\n     ,@処理結果内容" +
                         "\r\n     ,@店舗ID" +
                         "\r\n     ,@伝票番号" +
                         "\r\n     ,@承認番号" +
                         "\r\n     ,@取引番号" +
                         "\r\n     ,@クレカ顧客番号" +
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
            string sql = "\r\n UPDATE FR_T_クレカ決済履歴" +
                         "\r\n SET  請求番号 = @請求番号" +
                         "\r\n     ,請求日 = @請求日" +
                         "\r\n     ,部署ＣＤ = @部署ＣＤ" +
                         "\r\n     ,請求先ＣＤ = @請求先ＣＤ" +
                         "\r\n     ,請求先名 = @請求先名" +
                         "\r\n     ,今回請求額 = @今回請求額" +
                         "\r\n     ,処理タイプ = @処理タイプ" +
                         "\r\n     ,処理結果 = @処理結果" +
                         "\r\n     ,処理結果内容 = @処理結果内容" +
                         "\r\n     ,店舗ID = @店舗ID" +
                         "\r\n     ,伝票番号 = @伝票番号" +
                         "\r\n     ,承認番号 = @承認番号" +
                         "\r\n     ,取引番号 = @取引番号" +
                         "\r\n     ,クレカ顧客番号 = @クレカ顧客番号" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 履歴番号 = @履歴番号" +
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
            string sql = "\r\n DELETE FROM FR_T_クレカ決済履歴" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 履歴番号 = @履歴番号" +
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

                    if (履歴番号 != null)
                    {
                        cmd.Parameters.Add("@履歴番号", SqlDbType.Decimal).Value = 履歴番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@履歴番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (請求番号 != null)
                    {
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = 請求番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (請求日 != null)
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = 請求日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = 請求先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求先名 != null)
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = 請求先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (今回請求額 != null)
                    {
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = 今回請求額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (処理タイプ != null)
                    {
                        cmd.Parameters.Add("@処理タイプ", SqlDbType.Int).Value = 処理タイプ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理タイプ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理結果 != null)
                    {
                        cmd.Parameters.Add("@処理結果", SqlDbType.Int).Value = 処理結果;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理結果", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理結果内容 != null)
                    {
                        cmd.Parameters.Add("@処理結果内容", SqlDbType.VarChar).Value = 処理結果内容;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理結果内容", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (店舗ID != null)
                    {
                        cmd.Parameters.Add("@店舗ID", SqlDbType.VarChar).Value = 店舗ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@店舗ID", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (伝票番号 != null)
                    {
                        cmd.Parameters.Add("@伝票番号", SqlDbType.VarChar).Value = 伝票番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@伝票番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (承認番号 != null)
                    {
                        cmd.Parameters.Add("@承認番号", SqlDbType.VarChar).Value = 承認番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@承認番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (取引番号 != null)
                    {
                        cmd.Parameters.Add("@取引番号", SqlDbType.VarChar).Value = 取引番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@取引番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (クレカ顧客番号 != null)
                    {
                        cmd.Parameters.Add("@クレカ顧客番号", SqlDbType.VarChar).Value = クレカ顧客番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@クレカ顧客番号", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("FR_T_クレカ決済履歴を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "履歴番号が重複しています。";
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

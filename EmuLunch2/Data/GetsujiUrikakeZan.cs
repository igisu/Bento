using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 月次売掛残高
    /// </summary>
    public class GetsujiUrikakeZan : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _集計年;
        private int? _集計月;
        private int? _部署ＣＤ;
        private int? _請求先ＣＤ;
        private string _請求先名;
        private decimal? _先月売掛残高;
        private decimal? _今月入金額;
        private decimal? _今月売上額;
        private decimal? _今月売掛残高;

        private string _部署名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GetsujiUrikakeZan()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public GetsujiUrikakeZan(GetsujiUrikakeZan src)
        {
            集計年 = src.集計年;
            集計月 = src.集計月;
            部署ＣＤ = src.部署ＣＤ;
            請求先ＣＤ = src.請求先ＣＤ;
            請求先名 = src.請求先名;
            先月売掛残高 = src.先月売掛残高;
            今月入金額 = src.今月入金額;
            今月売上額 = src.今月売上額;
            今月売掛残高 = src.今月売掛残高;

            部署名 = src.部署名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public GetsujiUrikakeZan(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            集計年 = dr["集計年", version] as int?;
            集計月 = dr["集計月", version] as int?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            請求先ＣＤ = dr["請求先ＣＤ", version] as int?;
            請求先名 = dr["請求先名", version] as string;
            先月売掛残高 = dr["先月売掛残高", version] as decimal?;
            今月入金額 = dr["今月入金額", version] as decimal?;
            今月売上額 = dr["今月売上額", version] as decimal?;
            今月売掛残高 = dr["今月売掛残高", version] as decimal?;

            if (dr.Table.Columns.Contains("部署名"))
            {
                部署名 = dr["部署名"] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public int? 集計年
        {
            get { return _集計年; }
            set { _集計年 = value; }
        }

        public int? 集計月
        {
            get { return _集計月; }
            set { _集計月 = value; }
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

        public decimal? 先月売掛残高
        {
            get { return _先月売掛残高; }
            set { _先月売掛残高 = value; }
        }

        public decimal? 今月入金額
        {
            get { return _今月入金額; }
            set { _今月入金額 = value; }
        }

        public decimal? 今月売上額
        {
            get { return _今月売上額; }
            set { _今月売上額 = value; }
        }

        public decimal? 今月売掛残高
        {
            get { return _今月売掛残高; }
            set { _今月売掛残高 = value; }
        }


        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public override string Code
        {
            get { return string.Format("集計年=[{0}] 集計月=[{1}] 部署ＣＤ=[{2}] 請求先ＣＤ=[{3}]", 集計年, 集計月, 部署ＣＤ, 請求先ＣＤ); }
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
        public bool Equals(GetsujiUrikakeZan target)
        {
            if (集計年 == target.集計年 &&
                集計月 == target.集計月 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                請求先ＣＤ == target.請求先ＣＤ &&
                請求先名 == target.請求先名 &&
                先月売掛残高 == target.先月売掛残高 &&
                今月入金額 == target.今月入金額 &&
                今月売上額 == target.今月売上額 &&
                今月売掛残高 == target.今月売掛残高)
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
            string sql = "\r\n INSERT INTO 月次売掛残高集計(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,集計年" +
                         "\r\n     ,集計月" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,請求先ＣＤ" +
                         "\r\n     ,請求先名" +
                         "\r\n     ,先月売掛残高" +
                         "\r\n     ,今月入金額" +
                         "\r\n     ,今月売上額" +
                         "\r\n     ,今月売掛残高" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@集計年" +
                         "\r\n     ,@集計月" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@請求先ＣＤ" +
                         "\r\n     ,@請求先名" +
                         "\r\n     ,@先月売掛残高" +
                         "\r\n     ,@今月入金額" +
                         "\r\n     ,@今月売上額" +
                         "\r\n     ,@今月売掛残高" +
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
            string sql = "\r\n UPDATE 月次売掛残高集計" +
                         "\r\n SET 先月売掛残高   = @先月売掛残高" +
                         "\r\n    ,今月入金額     = @今月入金額" +
                         "\r\n    ,今月売上額     = @今月売上額" +
                         "\r\n    ,今月売掛残高   = @今月売掛残高" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 集計年     = @集計年" +
                         "\r\n   AND 集計月     = @集計月" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND 請求先ＣＤ = @請求先ＣＤ" +
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
            string sql = "\r\n DELETE FROM 月次売掛残高集計" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 集計年     = @集計年" +
                         "\r\n   AND 集計月     = @集計月" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND 請求先ＣＤ = @請求先ＣＤ" +
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

                    if (集計年 != null)
                    {
                        cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = 集計年;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (集計月 != null)
                    {
                        cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = 集計月;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求先ＣＤ != null && 請求先ＣＤ != -1)
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

                    if (先月売掛残高 != null)
                    {
                        cmd.Parameters.Add("@先月売掛残高", SqlDbType.Decimal).Value = 先月売掛残高;
                    }
                    else
                    {
                        cmd.Parameters.Add("@先月売掛残高", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (今月入金額 != null)
                    {
                        cmd.Parameters.Add("@今月入金額", SqlDbType.Decimal).Value = 今月入金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@今月入金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (今月売上額 != null)
                    {
                        cmd.Parameters.Add("@今月売上額", SqlDbType.Decimal).Value = 今月売上額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@今月売上額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (今月売掛残高 != null)
                    {
                        cmd.Parameters.Add("@今月売掛残高", SqlDbType.Decimal).Value = 今月売掛残高;
                    }
                    else
                    {
                        cmd.Parameters.Add("@今月売掛残高", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("月次売掛残高集計を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "月次売掛残高集計が重複しています。";
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

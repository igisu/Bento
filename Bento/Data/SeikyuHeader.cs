using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求ヘッダ
    /// </summary>
    public class SeikyuHeader : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal? _請求番号;
        private int? _得意先ＣＤ;
        private int? _部署ＣＤ;
        private decimal? _計算税率;
        private string _得意先名;
        private DateTime? _請求日;
        private int? _請求先ＣＤ;
        private string _請求先名;
        private decimal? _売上金額;
        private decimal? _消費税;
        private decimal? _売上金額税込;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuHeader()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuHeader(SeikyuHeader src)
        {
            請求番号 = src.請求番号;
            得意先ＣＤ = src.得意先ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            計算税率 = src.計算税率;
            得意先名 = src.得意先名;
            請求日 = src.請求日;
            請求先ＣＤ = src.請求先ＣＤ;
            請求先名 = src.請求先名;
            売上金額 = src.売上金額;
            消費税 = src.消費税;
            売上金額税込 = src.売上金額税込;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SeikyuHeader(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            請求番号 = dr["請求番号", version] as decimal?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            計算税率 = dr["計算税率", version] as decimal?;
            得意先名 = dr["得意先名", version] as string;
            請求日 = dr["請求日", version] as DateTime?;
            請求先ＣＤ = dr["請求先ＣＤ", version] as int?;
            請求先名 = dr["請求先名", version] as string;
            売上金額 = dr["売上金額", version] as decimal?;
            消費税 = dr["消費税", version] as decimal?;
            売上金額税込 = dr["売上金額税込", version] as decimal?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 請求番号
        {
            get { return _請求番号; }
            set { _請求番号 = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public decimal? 計算税率
        {
            get { return _計算税率; }
            set { _計算税率 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public DateTime? 請求日
        {
            get { return _請求日; }
            set { _請求日 = value; }
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

        public decimal? 売上金額
        {
            get { return _売上金額; }
            set { _売上金額 = value; }
        }

        public decimal? 消費税
        {
            get { return _消費税; }
            set { _消費税 = value; }
        }

        public decimal? 売上金額税込
        {
            get { return _売上金額税込; }
            set { _売上金額税込 = value; }
        }

        public override string Code
        {
            get { return string.Format("請求番号=[{0}] 得意先ＣＤ=[{1}] 部署ＣＤ=[{2}] 計算税率=[{3}]", 請求番号, 得意先ＣＤ, 部署ＣＤ, 計算税率); }
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
        public bool Equals(SeikyuHeader target)
        {
            if (請求番号 == target.請求番号 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                部署ＣＤ == target.部署ＣＤ &&
                計算税率 == target.計算税率 &&
                得意先名 == target.得意先名 &&
                請求日 == target.請求日 &&
                請求先ＣＤ == target.請求先ＣＤ &&
                請求先名 == target.請求先名 &&
                売上金額 == target.売上金額 &&
                消費税 == target.消費税 &&
                売上金額税込 == target.売上金額税込)
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
            string sql = "\r\n INSERT INTO 請求ヘッダ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,請求番号" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,計算税率" +
                         "\r\n     ,得意先名" +
                         "\r\n     ,請求日" +
                         "\r\n     ,請求先ＣＤ" +
                         "\r\n     ,請求先名" +
                         "\r\n     ,売上金額" +
                         "\r\n     ,消費税" +
                         "\r\n     ,売上金額税込" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@請求番号" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@計算税率" +
                         "\r\n     ,@得意先名" +
                         "\r\n     ,@請求日" +
                         "\r\n     ,@請求先ＣＤ" +
                         "\r\n     ,@請求先名" +
                         "\r\n     ,@売上金額" +
                         "\r\n     ,@消費税" +
                         "\r\n     ,@売上金額税込" +
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
            string sql = "\r\n UPDATE 請求ヘッダ" +
                         "\r\n SET 得意先名       = @得意先名" +
                         "\r\n    ,請求日         = @請求日" +
                         "\r\n    ,請求先ＣＤ     = @請求先ＣＤ" +
                         "\r\n    ,請求先名       = @請求先名" +
                         "\r\n    ,売上金額       = @売上金額" +
                         "\r\n    ,消費税         = @消費税" +
                         "\r\n    ,売上金額税込   = @売上金額税込" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 請求番号   = @請求番号" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND 計算税率   = @計算税率" +
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
            string sql = "\r\n DELETE FROM 請求ヘッダ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 請求番号   = @請求番号" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND 計算税率   = @計算税率" +
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

                    if (請求番号 != null)
                    {
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = 請求番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = DBNull.Value;
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

                    if (計算税率 != null)
                    {
                        cmd.Parameters.Add("@計算税率", SqlDbType.Decimal).Value = 計算税率;
                    }
                    else
                    {
                        cmd.Parameters.Add("@計算税率", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (得意先名 != null)
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = 得意先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (請求日 != null)
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = 請求日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = DBNull.Value;
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

                    if (売上金額 != null)
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = 売上金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (消費税 != null)
                    {
                        cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = 消費税;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費税", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (売上金額税込 != null)
                    {
                        cmd.Parameters.Add("@売上金額税込", SqlDbType.Decimal).Value = 売上金額税込;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上金額税込", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("請求ヘッダを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "キーが重複しています。";
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

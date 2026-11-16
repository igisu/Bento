using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 非顧客電話番号マスタ
    /// </summary>
    public class HiKokyakuTelNo : TableBase, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 電話番号に該当するマスタを返します
        /// </summary>
        /// <param name="telNo">電話番号</param>
        /// <returns></returns>
        public static HiKokyakuTelNo GetHiKokyakuTelNo(string telNo)
        {
            HiKokyakuTelNo hiKokyakuTelNo = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.電話番号" +
                                  "\r\n     ,A.非顧客名称" +
                                  "\r\n     ,A.非顧客カナ" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     非顧客電話番号マスタ AS A" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND A.電話番号 = @電話番号";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = telNo;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("非顧客電話番号マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    hiKokyakuTelNo = new HiKokyakuTelNo(dt.Rows[0]);
                }
            }

            return hiKokyakuTelNo;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _電話番号;
        private string _非顧客名称;
        private string _非顧客カナ;
        private int? _修正担当者ＣＤ;
        private DateTime? _修正日;

        private string _修正担当者名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HiKokyakuTelNo()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public HiKokyakuTelNo(HiKokyakuTelNo src)
        {
            電話番号 = src.電話番号;
            非顧客名称 = src.非顧客名称;
            非顧客カナ = src.非顧客カナ;
            修正担当者ＣＤ = src.修正担当者ＣＤ;
            修正日 = src.修正日;

            修正担当者名 = src.修正担当者名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public HiKokyakuTelNo(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            電話番号 = dr["電話番号", version] as string;
            非顧客名称 = dr["非顧客名称", version] as string;
            非顧客カナ = dr["非顧客カナ", version] as string;
            修正担当者ＣＤ = dr["修正担当者ＣＤ", version] as int?;
            修正日 = dr["修正日", version] as DateTime?;

            if (dr.Table.Columns.Contains("修正担当者名"))
            {
                修正担当者名 = dr["修正担当者名", version] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string 電話番号
        {
            get { return _電話番号; }
            set { _電話番号 = value; }
        }

        public string 非顧客名称
        {
            get { return _非顧客名称; }
            set { _非顧客名称 = value; }
        }

        public string 非顧客カナ
        {
            get { return _非顧客カナ; }
            set { _非顧客カナ = value; }
        }

        public int? 修正担当者ＣＤ
        {
            get { return _修正担当者ＣＤ; }
            set { _修正担当者ＣＤ = value; }
        }

        public DateTime? 修正日
        {
            get { return _修正日; }
            set { _修正日 = value; }
        }


        public string 修正担当者名
        {
            get { return _修正担当者名; }
            set { _修正担当者名 = value; }
        }

        public override string Code
        {
            get { return string.Format("電話番号=[{0}]", 電話番号); }
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
        /// データ行への設定
        /// </summary>
        /// <param name="dr"></param>
        public void SetDataRow(DataRow dr)
        {
            // DataRow への設定は SetField メソッドを使って書くと DBNull を意識しなくて済む
            dr.SetField<string>("電話番号", _電話番号);
            dr.SetField<string>("非顧客名称", _非顧客名称);
            dr.SetField<string>("非顧客カナ", _非顧客カナ);
            dr.SetField<int?>("修正担当者ＣＤ", _修正担当者ＣＤ);
            dr.SetField<DateTime?>("修正日", _修正日);

            base.ToDataRow(dr);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(HiKokyakuTelNo target)
        {
            if (電話番号 == target.電話番号 &&
                非顧客名称 == target.非顧客名称 &&
                非顧客カナ == target.非顧客カナ &&
                修正担当者ＣＤ == target.修正担当者ＣＤ &&
                修正日 == target.修正日)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// DB登録
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool Insert()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    return Insert(con);
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

        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool Insert(SqlConnection con)
        {
            try
            {
                SqlTransaction tran = con.BeginTransaction();
                bool ret = Insert(con, tran);
                if (ret)
                {
                    tran.Commit();
                }
                return ret;
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

        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO 非顧客電話番号マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,電話番号" +
                         "\r\n     ,非顧客名称" +
                         "\r\n     ,非顧客カナ" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@電話番号" +
                         "\r\n     ,@非顧客名称" +
                         "\r\n     ,@非顧客カナ" +
                         "\r\n     ,@修正担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n )";

            return Save(con, tran, sql, History.ProcessType.Create);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool Update()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    return Update(con);
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

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool Update(SqlConnection con)
        {
            try
            {
                SqlTransaction tran = con.BeginTransaction();
                bool ret = Update(con, tran);
                if (ret)
                {
                    tran.Commit();
                }
                return ret;
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

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Update(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n UPDATE 非顧客電話番号マスタ" +
                         "\r\n SET 非顧客名称      = @非顧客名称" +
                         "\r\n     ,非顧客カナ     = @非顧客カナ" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 電話番号 = @電話番号" +
                         "\r\n   AND 修正日   = @修正日";

            return Save(con, tran, sql, History.ProcessType.Update);
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool Delete()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    return Delete(con);
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

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool Delete(SqlConnection con)
        {
            try
            {
                SqlTransaction tran = con.BeginTransaction();
                bool ret = Delete(con, tran);
                if (ret)
                {
                    tran.Commit();
                }
                return ret;
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

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n DELETE FROM 非顧客電話番号マスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 電話番号 = @電話番号" +
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

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (非顧客名称 != null)
                    {
                        cmd.Parameters.Add("@非顧客名称", SqlDbType.VarChar).Value = 非顧客名称;
                    }
                    else
                    {
                        cmd.Parameters.Add("@非顧客名称", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (非顧客カナ != null)
                    {
                        cmd.Parameters.Add("@非顧客カナ", SqlDbType.VarChar).Value = 非顧客カナ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@非顧客カナ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    // 更新の場合のみ編集日時を設定（新規の場合は設定しない）
                    if (修正日 != null)
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = 修正日;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("非顧客電話番号マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "電話番号が重複しています。";
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

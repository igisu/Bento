using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 短冊発注メール設定
    /// </summary>
    public class TanzakuOrderMail : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 短冊発注メール設定のリストを返します
        /// </summary>
        /// <returns>このクラスのリスト</returns>
        public static List<TanzakuOrderMail> GetTanzakuOrderMailList()
        {
            List<TanzakuOrderMail> tanzakuOrderMailList = new List<TanzakuOrderMail>();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.メールアドレス" +
                                  "\r\n     ,A.宛名" +
                                  "\r\n     ,A.CCフラグ" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     短冊発注メール設定 AS A" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ";


                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("短冊発注メール設定");
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    tanzakuOrderMailList.Add(new TanzakuOrderMail(dr));
                }
            }

            return tanzakuOrderMailList;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _メールアドレス;
        private string _宛名;
        private bool? _CCフラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TanzakuOrderMail()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TanzakuOrderMail(TanzakuOrderMail src)
        {
            メールアドレス = src.メールアドレス;
            宛名 = src.宛名;
            CCフラグ = src.CCフラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TanzakuOrderMail(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            メールアドレス = dr["メールアドレス", version] as string;
            宛名 = dr["宛名", version] as string;
            CCフラグ = dr["CCフラグ", version] as bool?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string メールアドレス
        {
            get { return _メールアドレス; }
            set { _メールアドレス = value; }
        }

        public string 宛名
        {
            get { return _宛名; }
            set { _宛名 = value; }
        }

        public bool? CCフラグ
        {
            get { return _CCフラグ; }
            set { _CCフラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("メールアドレス=[{0}]", メールアドレス); }
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
        public bool Equals(TanzakuOrderMail target)
        {
            if (メールアドレス == target.メールアドレス &&
                宛名 == target.宛名 &&
                (CCフラグ ?? false) == (target.CCフラグ ?? false))
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
            string sql = "\r\n INSERT INTO 短冊発注メール設定(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,メールアドレス" +
                         "\r\n     ,宛名" +
                         "\r\n     ,CCフラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@メールアドレス" +
                         "\r\n     ,@宛名" +
                         "\r\n     ,@CCフラグ" +
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
            string sql = "\r\n UPDATE 短冊発注メール設定" +
                         "\r\n SET 宛名           = @宛名" +
                         "\r\n    ,CCフラグ       = @CCフラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ       = @企業ＣＤ" +
                         "\r\n   AND メールアドレス = @メールアドレス" +
                         "\r\n   AND 修正日         = @修正日";

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
            string sql = "\r\n DELETE FROM 短冊発注メール設定" +
                         "\r\n WHERE 企業ＣＤ       = @企業ＣＤ" +
                         "\r\n   AND メールアドレス = @メールアドレス" +
                         "\r\n   AND 修正日         = @修正日";

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

                    if (メールアドレス != null)
                    {
                        cmd.Parameters.Add("@メールアドレス", SqlDbType.VarChar).Value = メールアドレス;
                    }
                    else
                    {
                        cmd.Parameters.Add("@メールアドレス", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (宛名 != null)
                    {
                        cmd.Parameters.Add("@宛名", SqlDbType.VarChar).Value = 宛名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@宛名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (CCフラグ != null)
                    {
                        cmd.Parameters.Add("@CCフラグ", SqlDbType.Bit).Value = CCフラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@CCフラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("短冊発注メール設定を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "メールアドレスが重複しています。";
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

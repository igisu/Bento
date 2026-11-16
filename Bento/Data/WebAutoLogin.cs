using System;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web自動ログインテーブル
    /// </summary>
    public class WebAutoLogin : TableBase
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebAutoLogin()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebAutoLogin(WebAutoLogin src)
        {
            ID = src.ID;
            認証キー = src.認証キー;
            得意先ＣＤ = src.得意先ＣＤ;
            UA = src.UA;
            IPv4アドレス = src.IPv4アドレス;
            最終ログイン日時 = src.最終ログイン日時;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public WebAutoLogin(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            ID = dr["ID", version] as decimal?;
            認証キー = dr["認証キー", version] as string;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            UA = dr["UA", version] as string;
            IPv4アドレス = dr["IPv4アドレス", version] as string;
            最終ログイン日時 = dr["最終ログイン日時", version] as DateTime?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? ID { get; set; }
        public string 認証キー { get; set; }
        public int? 得意先ＣＤ { get; set; }
        public string UA { get; set; }
        public string IPv4アドレス { get; set; }
        public DateTime? 最終ログイン日時 { get; set; }

        public override string Code
        {
            get { return string.Format("ID=[{0}]", ID); }
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
            string sql = "\r\n INSERT INTO WEB_T_自動ログイン(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,認証キー" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,UA" +
                         "\r\n     ,IPv4アドレス" +
                         "\r\n     ,最終ログイン日時" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@認証キー" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@UA" +
                         "\r\n     ,@IPv4アドレス" +
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
            string sql = "\r\n UPDATE WEB_T_自動ログイン" +
                         "\r\n SET UA               = @UA" +
                         "\r\n    ,IPv4アドレス     = @IPv4アドレス" +
                         "\r\n    ,最終ログイン日時 = getdate()" +
                         "\r\n WHERE ID = @ID";

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
            string sql = "\r\n DELETE FROM WEB_T_自動ログイン WHERE ID = @ID";

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

                    if (ID != null)
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.Decimal).Value = ID;
                    }

                    if (認証キー != null)
                    {
                        cmd.Parameters.Add("@認証キー", SqlDbType.VarChar).Value = 認証キー;
                    }
                    else
                    {
                        cmd.Parameters.Add("@認証キー", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (UA != null)
                    {
                        cmd.Parameters.Add("@UA", SqlDbType.VarChar).Value = UA;
                    }
                    else
                    {
                        cmd.Parameters.Add("@UA", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (IPv4アドレス != null)
                    {
                        cmd.Parameters.Add("@IPv4アドレス", SqlDbType.VarChar).Value = IPv4アドレス;
                    }
                    else
                    {
                        cmd.Parameters.Add("@IPv4アドレス", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (最終ログイン日時 != null)
                    {
                        cmd.Parameters.Add("@最終ログイン日時", SqlDbType.DateTime).Value = 最終ログイン日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@最終ログイン日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("Web自動ログインテーブルを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "Web自動ログインテーブルが重複しています。";
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

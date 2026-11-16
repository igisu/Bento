using System;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web注文履歴
    /// </summary>
    public class WebOrderHistory : TableBase
    {
        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebOrderHistory()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebOrderHistory(WebOrderHistory src)
        {
            操作番号 = src.操作番号;
            認証キー = src.認証キー;
            得意先ＣＤ = src.得意先ＣＤ;
            UA = src.UA;
            IPv4アドレス = src.IPv4アドレス;
            WEB履歴区分ＣＤ = src.WEB履歴区分ＣＤ;
            操作日時 = src.操作日時;
            操作内容 = src.操作内容;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public WebOrderHistory(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            操作番号 = dr["操作番号", version] as decimal?;
            認証キー = dr["認証キー", version] as string;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            UA = dr["UA", version] as string;
            IPv4アドレス = dr["IPv4アドレス", version] as string;
            WEB履歴区分ＣＤ = dr["WEB履歴区分ＣＤ", version] as int?;
            操作日時 = dr["操作日時", version] as DateTime?;
            操作内容 = dr["操作内容", version] as string;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 操作番号 { get; set; }
        public string 認証キー { get; set; }
        public int? 得意先ＣＤ { get; set; }
        public string UA { get; set; }
        public string IPv4アドレス { get; set; }
        public int? WEB履歴区分ＣＤ { get; set; }
        public DateTime? 操作日時 { get; set; }
        public string 操作内容 { get; set; }

        public override string Code
        {
            get { return string.Format("操作番号=[{0}]", 操作番号); }
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
            string sql = "\r\n INSERT INTO WEB_T_注文履歴(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,認証キー" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,UA" +
                         "\r\n     ,IPv4アドレス" +
                         "\r\n     ,WEB履歴区分ＣＤ" +
                         "\r\n     ,操作日時" +
                         "\r\n     ,操作内容" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@認証キー" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@UA" +
                         "\r\n     ,@IPv4アドレス" +
                         "\r\n     ,@WEB履歴区分ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n     ,@操作内容" +
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
            string sql = "\r\n UPDATE WEB_T_注文履歴" +
                         "\r\n SET 認証キー        = @認証キー" +
                         "\r\n    ,得意先ＣＤ      = @得意先ＣＤ" +
                         "\r\n    ,UA              = @UA" +
                         "\r\n    ,IPv4アドレス    = @IPv4アドレス" +
                         "\r\n    ,WEB履歴区分ＣＤ = @WEB履歴区分ＣＤ" +
                         "\r\n    ,操作日時        = @操作日時" +
                         "\r\n    ,操作内容        = @操作内容" +
                         "\r\n WHERE 操作番号 = @操作番号";

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
            string sql = "\r\n DELETE FROM WEB_T_注文履歴 WHERE 操作番号 = @操作番号";

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

                    if (操作番号 != null)
                    {
                        cmd.Parameters.Add("@ID", SqlDbType.Decimal).Value = 操作番号;
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

                    if (WEB履歴区分ＣＤ != null)
                    {
                        cmd.Parameters.Add("@WEB履歴区分ＣＤ", SqlDbType.Int).Value = WEB履歴区分ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@WEB履歴区分ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (操作日時 != null)
                    {
                        cmd.Parameters.Add("@操作日時", SqlDbType.DateTime).Value = 操作日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@操作日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (操作内容 != null)
                    {
                        cmd.Parameters.Add("@操作内容", SqlDbType.VarChar).Value = 操作内容;
                    }
                    else
                    {
                        cmd.Parameters.Add("@操作内容", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("Web注文履歴テーブルを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "Web注文履歴テーブルが重複しています。";
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

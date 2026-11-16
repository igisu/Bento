using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;
namespace Emu.Bento.Data
{
    /// <summary>
    /// 出荷履歴
    /// </summary>
    public class SyukkaHistory : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal? _出荷番号;
        private int? _出荷個口番号;
        private int? _担当者ＣＤ;
        private int? _出荷入力区分;
        private DateTime? _出荷日時;
        private DateTime? _出荷完了日時;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyukkaHistory()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SyukkaHistory(SyukkaHistory src)
        {
            出荷番号 = src.出荷番号;
            出荷個口番号 = src.出荷個口番号;
            担当者ＣＤ = src.担当者ＣＤ;
            出荷入力区分 = src.出荷入力区分;
            出荷日時 = src.出荷日時;
            出荷完了日時 = src.出荷完了日時;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SyukkaHistory(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            出荷番号 = dr["出荷番号", version] as decimal?;
            出荷個口番号 = dr["出荷個口番号", version] as int?;
            担当者ＣＤ = dr["担当者ＣＤ", version] as int?;
            出荷入力区分 = dr["出荷入力区分", version] as int?;
            出荷日時 = dr["出荷日時", version] as DateTime?;
            出荷完了日時 = dr["出荷完了日時", version] as DateTime?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 出荷番号
        {
            get { return _出荷番号; }
            set { _出荷番号 = value; }
        }

        public int? 出荷個口番号
        {
            get { return _出荷個口番号; }
            set { _出荷個口番号 = value; }
        }

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public int? 出荷入力区分
        {
            get { return _出荷入力区分; }
            set { _出荷入力区分 = value; }
        }

        public DateTime? 出荷日時
        {
            get { return _出荷日時; }
            set { _出荷日時 = value; }
        }

        public DateTime? 出荷完了日時
        {
            get { return _出荷完了日時; }
            set { _出荷完了日時 = value; }
        }

        public override string Code
        {
            get { return string.Format("出荷番号=[{0}], 出荷個口番号=[{1}]", 出荷番号, 出荷個口番号); }
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
        public bool Equals(SyukkaHistory target)
        {
            if (出荷番号 == target.出荷番号 &&
                出荷個口番号 == target.出荷個口番号 &&
                担当者ＣＤ == target.担当者ＣＤ &&
                (出荷入力区分 ?? -1) == (target.出荷入力区分 ?? -1) &&
                出荷日時 == target.出荷日時 &&
                出荷完了日時 == target.出荷完了日時)
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
            string sql = "\r\n INSERT INTO SP_T_出荷履歴(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,出荷番号" +
                         "\r\n     ,出荷個口番号" +
                         "\r\n     ,担当者ＣＤ" +
                         "\r\n     ,出荷入力区分" +
                         "\r\n     ,出荷日時" +
                         "\r\n     ,出荷完了日時" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@出荷番号" +
                         "\r\n     ,@出荷個口番号" +
                         "\r\n     ,@担当者ＣＤ" +
                         "\r\n     ,@出荷入力区分" +
                         "\r\n     ,@出荷日時" +
                         "\r\n     ,@出荷完了日時" +
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
            string sql = "\r\n UPDATE SP_T_出荷履歴" +
                         "\r\n SET 出荷入力区分   = @出荷入力区分" +
                         "\r\n    ,出荷日時       = @出荷日時" +
                         "\r\n    ,出荷完了日時   = @出荷完了日時" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND 出荷番号     = @出荷番号" +
                         "\r\n   AND 出荷個口番号 = @出荷個口番号" +
                         "\r\n   AND 担当者ＣＤ   = @担当者ＣＤ" +
                         "\r\n   AND 修正日       = @修正日";

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
            string sql = "\r\n DELETE FROM SP_T_出荷履歴" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND 出荷番号     = @出荷番号" +
                         "\r\n   AND 出荷個口番号 = @出荷個口番号" +
                         "\r\n   AND 担当者ＣＤ   = @担当者ＣＤ" +
                         "\r\n   AND 修正日       = @修正日";

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

                    if (出荷番号 != null)
                    {
                        cmd.Parameters.Add("@出荷番号", SqlDbType.Decimal).Value = 出荷番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@出荷番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (出荷個口番号 != null)
                    {
                        cmd.Parameters.Add("@出荷個口番号", SqlDbType.Int).Value = 出荷個口番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@出荷個口番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (出荷入力区分 != null && 出荷入力区分 > -1)
                    {
                        cmd.Parameters.Add("@出荷入力区分", SqlDbType.Int).Value = 出荷入力区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@出荷入力区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (出荷日時 != null)
                    {
                        cmd.Parameters.Add("@出荷日時", SqlDbType.DateTime).Value = 出荷日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@出荷日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (出荷完了日時 != null)
                    {
                        cmd.Parameters.Add("@出荷完了日時", SqlDbType.DateTime).Value = 出荷完了日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@出荷完了日時", SqlDbType.DateTime).Value = DBNull.Value;
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

                    log.InfoFormat("出荷履歴を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "出荷履歴が重複しています。";
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

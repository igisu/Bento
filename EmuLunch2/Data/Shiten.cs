using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 支店マスタ
    /// </summary>
    public class Shiten : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _支店ＣＤ;
        private int? _支店ＣＤorg;
        private string _支店名;
        private bool? _無効フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Shiten()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Shiten(Shiten src)
        {
            支店ＣＤ = src.支店ＣＤ;
            支店ＣＤorg = src.支店ＣＤorg;
            支店名 = src.支店名;
            無効フラグ = src.無効フラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Shiten(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            支店ＣＤ = dr["支店ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("支店ＣＤorg"))
            {
                支店ＣＤorg = dr["支店ＣＤorg", version] as int?;
            }
            else
            {
                支店ＣＤorg = dr["支店ＣＤ", version] as int?;
            }

            支店名 = dr["支店名", version] as string;
            無効フラグ = dr["無効フラグ", version] as bool?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set
            {
                _支店ＣＤ = value;
                _errors.Remove("支店ＣＤ");
                if (value == null)
                {
                    _errors["支店ＣＤ"] = "支店ＣＤを入力してください。";
                }
            }
        }

        public int? 支店ＣＤorg
        {
            get { return _支店ＣＤorg; }
            set { _支店ＣＤorg = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("支店ＣＤ=[{0}]", (支店ＣＤ == 支店ＣＤorg ? 支店ＣＤ : 支店ＣＤorg ?? 支店ＣＤ)); }
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
        public bool Equals(Shiten target)
        {
            if (支店ＣＤ == target.支店ＣＤ &&
                支店名 == target.支店名 &&
                (無効フラグ ?? false) == (target.無効フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 支店マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,支店ＣＤ" +
                         "\r\n     ,支店名" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@支店ＣＤ" +
                         "\r\n     ,@支店名" +
                         "\r\n     ,@無効フラグ" +
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
            string sql = "\r\n UPDATE 支店マスタ" +
                         "\r\n SET  支店ＣＤ       = @支店ＣＤ" +
                         "\r\n     ,支店名         = @支店名" +
                         "\r\n     ,無効フラグ     = @無効フラグ" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n  WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n    AND 支店ＣＤ = @支店ＣＤorg" +
                         "\r\n    AND 修正日   = @修正日";

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
            string sql = "\r\n DELETE FROM 支店マスタ" +
                         "\r\n  WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n    AND 支店ＣＤ = @支店ＣＤorg" +
                         "\r\n    AND 修正日   = @修正日";

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

                    if (支店ＣＤ != null)
                    {
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = 支店ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (支店ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@支店ＣＤorg", SqlDbType.Int).Value = 支店ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支店ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (支店名 != null)
                    {
                        cmd.Parameters.Add("@支店名", SqlDbType.VarChar).Value = 支店名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@支店名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("支店マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "支店ＣＤが重複しています。";
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

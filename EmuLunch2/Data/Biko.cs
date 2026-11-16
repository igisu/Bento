using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 備考マスタ
    /// </summary>
    public class Biko : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _備考ＣＤ;
        private int? _備考ＣＤorg;
        private int? _備考区分;
        private string _備考;
        private bool? _無効フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Biko()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Biko(Biko src)
        {
            備考ＣＤ = src.備考ＣＤ;
            備考ＣＤorg = src.備考ＣＤorg;
            備考区分 = src.備考区分;
            備考 = src.備考;
            無効フラグ = src.無効フラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Biko(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            備考ＣＤ = dr["備考ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("備考ＣＤorg"))
            {
                備考ＣＤorg = dr["備考ＣＤorg", version] as int?;
            }
            else
            {
                備考ＣＤorg = dr["備考ＣＤ", version] as int?;
            }

            備考区分 = dr["備考区分", version] as int?;
            備考 = dr["備考", version] as string;
            無効フラグ = dr["無効フラグ", version] as bool?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 備考ＣＤ
        {
            get { return _備考ＣＤ; }
            set
            {
                _備考ＣＤ = value;
                _errors.Remove("備考ＣＤ");
                if (value == null)
                {
                    _errors["備考ＣＤ"] = "備考ＣＤを入力してください。";
                }
            }
        }

        public int? 備考ＣＤorg
        {
            get { return _備考ＣＤorg; }
            set { _備考ＣＤorg = value; }
        }

        public int? 備考区分
        {
            get { return _備考区分; }
            set { _備考区分 = value; }
        }

        public string 備考
        {
            get { return _備考; }
            set { _備考 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("備考ＣＤ=[{0}]", (備考ＣＤ == 備考ＣＤorg ? 備考ＣＤ : 備考ＣＤorg ?? 備考ＣＤ)); }
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
        public bool Equals(Biko target)
        {
            if (備考ＣＤ == target.備考ＣＤ &&
                備考区分 == target.備考区分 &&
                備考 == target.備考 &&
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
            string sql = "\r\n INSERT INTO 備考マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,備考ＣＤ" +
                         "\r\n     ,備考区分" +
                         "\r\n     ,備考" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@備考ＣＤ" +
                         "\r\n     ,@備考区分" +
                         "\r\n     ,@備考" +
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
            string sql = "\r\n UPDATE 備考マスタ" +
                         "\r\n SET  備考ＣＤ       = @備考ＣＤ" +
                         "\r\n     ,備考区分       = @備考区分" +
                         "\r\n     ,備考           = @備考" +
                         "\r\n     ,無効フラグ     = @無効フラグ" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n  WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n    AND 備考ＣＤ = @備考ＣＤorg" +
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
            string sql = "\r\n DELETE FROM 備考マスタ" +
                         "\r\n  WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n    AND 備考ＣＤ = @備考ＣＤorg" +
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

                    if (備考ＣＤ != null)
                    {
                        cmd.Parameters.Add("@備考ＣＤ", SqlDbType.Int).Value = 備考ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (備考ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@備考ＣＤorg", SqlDbType.Int).Value = 備考ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (備考区分 != null)
                    {
                        cmd.Parameters.Add("@備考区分", SqlDbType.Int).Value = 備考区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (備考 != null)
                    {
                        cmd.Parameters.Add("@備考", SqlDbType.VarChar).Value = 備考;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("備考マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "備考ＣＤが重複しています。";
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

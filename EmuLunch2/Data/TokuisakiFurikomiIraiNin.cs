using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先振込依頼人名テーブル
    /// </summary>
    public class TokuisakiFurikomiIraiNin : TableBase2, IDataErrorInfo
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
        private int? _得意先ＣＤ;
        private int? _依頼人ＣＤ;
        private string _振込依頼人名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiFurikomiIraiNin()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TokuisakiFurikomiIraiNin(TokuisakiFurikomiIraiNin src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            依頼人ＣＤ = src.依頼人ＣＤ;
            振込依頼人名 = src.振込依頼人名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TokuisakiFurikomiIraiNin(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            依頼人ＣＤ = dr["依頼人ＣＤ", version] as int?;
            振込依頼人名 = dr["振込依頼人名", version] as string;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public int? 依頼人ＣＤ
        {
            get { return _依頼人ＣＤ; }
            set { _依頼人ＣＤ = value; }
        }

        public string 振込依頼人名
        {
            get { return _振込依頼人名; }
            set { _振込依頼人名 = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}] 振込依頼人名=[{1}]", 得意先ＣＤ, 振込依頼人名); }
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
        public bool Equals(TokuisakiFurikomiIraiNin target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                依頼人ＣＤ == target.依頼人ＣＤ &&
                振込依頼人名 == target.振込依頼人名)
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
            string sql = "\r\n INSERT INTO 得意先振込依頼人名(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,依頼人ＣＤ" +
                         "\r\n     ,振込依頼人名" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,(SELECT ISNULL(MAX(依頼人ＣＤ), 0) + 1 FROM 得意先振込依頼人名 WHERE 企業ＣＤ = @企業ＣＤ AND 得意先ＣＤ = @得意先ＣＤ)" +
                         "\r\n     ,@振込依頼人名" +
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
            string sql = "\r\n UPDATE 得意先振込依頼人名" +
                         "\r\n SET 振込依頼人名   = @振込依頼人名" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日 = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 依頼人ＣＤ = @依頼人ＣＤ" +
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
            string sql = "\r\n DELETE FROM 得意先振込依頼人名" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 依頼人ＣＤ = @依頼人ＣＤ" +
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

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (依頼人ＣＤ != null)
                    {
                        cmd.Parameters.Add("@依頼人ＣＤ", SqlDbType.Int).Value = 依頼人ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@依頼人ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (振込依頼人名 != null)
                    {
                        cmd.Parameters.Add("@振込依頼人名", SqlDbType.VarChar).Value = 振込依頼人名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@振込依頼人名", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("得意先振込依頼人名テーブルを{0}しました。", historyProcess.DisplayName());
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

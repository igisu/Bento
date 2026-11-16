using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コース配送履歴
    /// </summary>
    public class CourseDeliveryHistory : TableBase2, IDataErrorInfo
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
        private DateTime? _配送日;
        private int? _得意先ＣＤ;
        private int? _部署ＣＤ;
        private string _得意先名;
        private int? _コースＣＤ;
        private int? _配送担当者ＣＤ;
        private DateTime? _コース配送日時;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CourseDeliveryHistory()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public CourseDeliveryHistory(CourseDeliveryHistory src)
        {
            配送日 = src.配送日;
            得意先ＣＤ = src.得意先ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            得意先名 = src.得意先名;
            コースＣＤ = src.コースＣＤ;
            配送担当者ＣＤ = src.配送担当者ＣＤ;
            コース配送日時 = src.コース配送日時;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public CourseDeliveryHistory(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            配送日 = dr["配送日", version] as DateTime?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            得意先名 = dr["得意先名", version] as string;
            コースＣＤ = dr["コースＣＤ", version] as int?;
            配送担当者ＣＤ = dr["配送担当者ＣＤ", version] as int?;
            コース配送日時 = dr["コース配送日時", version] as DateTime?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 配送日
        {
            get { return _配送日; }
            set
            {
                _配送日 = value;
                _errors.Remove("配送日");
                if (value == null)
                {
                    _errors["配送日"] = "配送日を入力してください。";
                }
            }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
                }
            }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                _errors.Remove("コースＣＤ");
                if (value == null)
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
            }
        }

        public int? 配送担当者ＣＤ
        {
            get { return _配送担当者ＣＤ; }
            set { _配送担当者ＣＤ = value; }
        }

        public DateTime? コース配送日時
        {
            get { return _コース配送日時; }
            set
            {
                _コース配送日時 = value;
                _errors.Remove("コース配送日時");
                if (value == null)
                {
                    _errors["コース配送日時"] = "コース配送日時を入力してください。";
                }
            }
        }

        public override string Code
        {
            get { return string.Format("配送日=[{0:yyyy/MM/dd}] 得意先ＣＤ=[{1}] 部署ＣＤ=[{2}]",  配送日, 得意先ＣＤ, 部署ＣＤ); }
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
        public bool Equals(CourseDeliveryHistory target)
        {
            if (配送日 == target.配送日 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                部署ＣＤ == target.部署ＣＤ &&
                得意先名 == target.得意先名 &&
                コースＣＤ == target.コースＣＤ &&
                配送担当者ＣＤ == target.配送担当者ＣＤ &&
                コース配送日時 == target.コース配送日時)
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
            string sql = "\r\n INSERT INTO コース配送履歴(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,配送日" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,得意先名" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,配送担当者ＣＤ" +
                         "\r\n     ,コース配送日時" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@配送日" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@得意先名" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@配送担当者ＣＤ" +
                         "\r\n     ,getdate()" +
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
            string sql = "\r\n UPDATE コース配送履歴" +
                         "\r\n SET 得意先名       = 得意先名" +
                         "\r\n    ,コースＣＤ     = コースＣＤ" +
                         "\r\n    ,配送担当者ＣＤ = 配送担当者ＣＤ" +
                         "\r\n    ,コース配送日時 = コース配送日時" +
                         "\r\n    ,修正担当者ＣＤ = 修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = 企業ＣＤ" +
                         "\r\n   AND 配送日     = 配送日" +
                         "\r\n   AND 得意先ＣＤ = 得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = 部署ＣＤ" +
                         "\r\n   AND 修正日     = 修正日";

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
            string sql = "\r\n DELETE FROM コース配送履歴" +
                         "\r\n WHERE 企業ＣＤ   = 企業ＣＤ" +
                         "\r\n   AND 配送日     = 配送日" +
                         "\r\n   AND 得意先ＣＤ = 得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = 部署ＣＤ" +
                         "\r\n   AND 修正日     = 修正日";

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

                    if (配送日 != null)
                    {
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = 配送日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = DBNull.Value;
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

                    if (得意先名 != null)
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = 得意先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@配送担当者ＣＤ", SqlDbType.Int).Value = 配送担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コース配送日時 != null)
                    {
                        cmd.Parameters.Add("@コース配送日時", SqlDbType.DateTime).Value = コース配送日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース配送日時", SqlDbType.DateTime).Value = DBNull.Value;
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

                    log.InfoFormat("コース配送履歴を{0}しました。", historyProcess.DisplayName());
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

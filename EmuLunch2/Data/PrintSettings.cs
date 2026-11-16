using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 帳票／プリンタ設定
    /// </summary>
    public class PrintSettings : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// テーブル名
        /// </summary>
        private const string TABLE_NAME = "帳票プリンタ設定マスタ";
        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// 帳票IDに該当するプリンタ設定を返します
        /// </summary>
        /// <param name="reportID">帳票ID</param>
        /// <returns>該当するレコードが有ればPrintSettingsクラスのインスタンス。無ければNull</returns>
        public static PrintSettings GetPrintSettings(string reportID)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                return GetPrintSettings(con, null, reportID);
            }
        }

        /// <summary>
        /// 帳票IDに該当するプリンタ設定を返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="reportID">帳票ID</param>
        /// <returns>該当するレコードが有ればPrintSettingsクラスのインスタンス。無ければNull</returns>
        public static PrintSettings GetPrintSettings(SqlConnection con, SqlTransaction tran, string reportID)
        {
            try
            {
                PrintSettings printSettings = null;

                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.担当者ＣＤ" +
                             "\r\n     ,A.帳票ID" +
                             "\r\n     ,A.プレビューフラグ" +
                             "\r\n     ,A.プリンタ名" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     帳票プリンタ設定マスタ AS A" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A1.担当者ＣＤ = A.作成担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A2.担当者ＣＤ = A.修正担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n     AND A.担当者ＣＤ = @担当者ＣＤ" +
                             "\r\n     AND A.帳票ID = @帳票ID";

                SqlCommand cmd = con.CreateCommand();
                if (tran != null)
                {
                    cmd.Transaction = tran;
                }
                cmd.CommandText = sql;
                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@帳票ID", SqlDbType.VarChar).Value = reportID ?? "";

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("帳票プリンタ設定マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    printSettings = new PrintSettings(dt.Rows[0]);
                }

                return printSettings;
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

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _担当者ＣＤ;
        private string _帳票ID;
        private bool? _プレビューフラグ;
        private string _プリンタ名;

        private string _帳票名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PrintSettings()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public PrintSettings(PrintSettings src)
        {
            担当者ＣＤ = src.担当者ＣＤ;
            帳票ID = src.帳票ID;
            プレビューフラグ = src.プレビューフラグ;
            プリンタ名 = src.プリンタ名;

            帳票名 = src.帳票名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public PrintSettings(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            担当者ＣＤ = dr["担当者ＣＤ", version] as int?;
            帳票ID = dr["帳票ID", version] as string;
            プレビューフラグ = (bool)dr["プレビューフラグ", version];
            プリンタ名 = dr["プリンタ名", version] as string;

            if (dr.Table.Columns.Contains("帳票名"))
            {
                帳票名 = dr["帳票名", version] as string;
            }

            base.FromDataRow(dr);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public string 帳票ID
        {
            get { return _帳票ID; }
            set { _帳票ID = value; }
        }

        public bool? プレビューフラグ
        {
            get { return _プレビューフラグ; }
            set { _プレビューフラグ = value; }
        }

        public string プリンタ名
        {
            get { return _プリンタ名; }
            set { _プリンタ名 = value; }
        }


        public string 帳票名
        {
            get { return _帳票名; }
            set { _帳票名 = value; }
        }

        public override string Code
        {
            get { return string.Format("担当者ＣＤ={0} 帳票ID={1}", 担当者ＣＤ, 帳票ID); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get {
                // プレビューフラグがチェックされている場合はプリンタ選択を必須とする
                if (this.プレビューフラグ != null && (bool)this.プレビューフラグ)
                {
                    if (string.IsNullOrEmpty(this.プリンタ名))
                    {
                        _errors["プリンタ名"] = "プリンタを選択してください。";
                        return "プリンタを選択してください。";
                    }
                }

                return null; 
            }
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
            dr.SetField<int?>("担当者ＣＤ", _担当者ＣＤ);
            dr.SetField<string>("帳票ID", _帳票ID);
            dr.SetField<bool?>("プレビューフラグ", _プレビューフラグ);
            dr.SetField<string>("プリンタ名", _プリンタ名);

            if (dr.Table.Columns.Contains("帳票名"))
            {
                dr.SetField<string>("帳票名", _帳票名);
            }
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(PrintSettings target)
        {
            if (担当者ＣＤ == target.担当者ＣＤ &&
                帳票ID == target.帳票ID &&
                (プレビューフラグ ?? false) == (target.プレビューフラグ ?? false) &&
                プリンタ名 == target.プリンタ名)
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
            string sql = "\r\n INSERT INTO " + TABLE_NAME + "(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,担当者ＣＤ" +
                         "\r\n     ,帳票ID" +
                         "\r\n     ,プレビューフラグ" +
                         "\r\n     ,プリンタ名" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@担当者ＣＤ" +
                         "\r\n     ,@帳票ID" +
                         "\r\n     ,@プレビューフラグ" +
                         "\r\n     ,@プリンタ名" +
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
            string sql = "\r\n UPDATE " + TABLE_NAME +
                         "\r\n SET プレビューフラグ = @プレビューフラグ" +
                         "\r\n    ,プリンタ名       = @プリンタ名" +
                         "\r\n    ,修正担当者ＣＤ   = @修正担当者ＣＤ" +
                         "\r\n    ,修正日           = getDate()" +
                         "\r\n WHERE 企業ＣＤ       = @企業ＣＤ" +
                         "\r\n   AND 担当者ＣＤ     = @担当者ＣＤ" +
                         "\r\n   AND 帳票ID         = @帳票ID" +
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
            string sql = "\r\n DELETE FROM " + TABLE_NAME +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 担当者ＣＤ = @担当者ＣＤ" +
                         "\r\n   AND 帳票ID     = @帳票ID" +
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

                    UserData userData = UserData.getInstance();

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    }

                    if (帳票ID != null)
                    {
                        cmd.Parameters.Add("@帳票ID", SqlDbType.NVarChar).Value = 帳票ID;
                    }
                    else
                    {
                        cmd.Parameters.Add("@帳票ID", SqlDbType.NVarChar).Value = DBNull.Value;
                    }

                    cmd.Parameters.Add("@プレビューフラグ", SqlDbType.Bit).Value = プレビューフラグ ?? false;

                    if (string.IsNullOrEmpty(プリンタ名) == false)
                    {
                        cmd.Parameters.Add("@プリンタ名", SqlDbType.NVarChar).Value = プリンタ名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@プリンタ名", SqlDbType.NVarChar).Value = DBNull.Value;
                    }

                    // 更新の場合のみ更新日時を設定（新規の場合は設定しない）
                    if (修正日 != null)
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = 修正日;
                    }

                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat(TABLE_NAME + "を{0}しました。", historyProcess.DisplayName());
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
                    log.Error(ex);
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

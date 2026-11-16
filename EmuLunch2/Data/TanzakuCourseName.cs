using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 短冊コース名マスタ
    /// </summary>
    public class TanzakuCourseName : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <returns></returns>
        public static TanzakuCourseName GetTanzakuCourseName(int? courseCD)
        {
            TanzakuCourseName tanzakuCourseName = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                // 短冊コース名マスタが存在すればそれから、存在しなければコースマスタからコース名を取得する
                // したがってコースマスタに対して短冊コース名マスタを外部結合する（短冊コース名マスタのレコードは必要な場合にしか作られないので）
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     CM.企業ＣＤ" +
                                  "\r\n     ,CM.コースＣＤ" +
                                  "\r\n     ,CM.コースＣＤ AS コースＣＤorg" +
                                  "\r\n     ,CM.コース名" +
                                  "\r\n     ,TC.短冊印刷コース名" +
                                  "\r\n     ,TC.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,TC.作成日" +
                                  "\r\n     ,TC.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,TC.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     コースマスタ AS CM" +
                                  "\r\n     LEFT JOIN 短冊コース名マスタ AS TC ON TC.企業ＣＤ = CM.企業ＣＤ AND TC.コースＣＤ = CM.コースＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ  AS A1 ON A1.企業ＣＤ = TC.企業ＣＤ AND TC.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ  AS A2 ON A2.企業ＣＤ = TC.企業ＣＤ AND TC.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     CM.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND CM.コースＣＤ = @コースＣＤ";


                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("短冊コース名マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    tanzakuCourseName = new TanzakuCourseName(dt.Rows[0]);
                }
            }

            return tanzakuCourseName;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _コースＣＤ;
        private int? _コースＣＤorg;        // レコード読込直後の値（更新・削除時に使用する）
        private string _短冊印刷コース名;

        private string _コース名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TanzakuCourseName()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TanzakuCourseName(TanzakuCourseName src)
        {
            コースＣＤ = src.コースＣＤ;
            コースＣＤorg = src.コースＣＤorg;
            短冊印刷コース名 = src.短冊印刷コース名;

            コース名 = src.コース名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TanzakuCourseName(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            コースＣＤ = dr["コースＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("コースＣＤorg"))
            {
                コースＣＤorg = dr["コースＣＤorg", version] as int?;
            }
            else
            {
                コースＣＤorg = dr["コースＣＤ", version] as int?;
            }

            短冊印刷コース名 = dr["短冊印刷コース名", version] as string;

            if (dr.Table.Columns.Contains("コース名"))
            {
                コース名 = dr["コース名", version] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                _errors.Remove("コースＣＤ");
                コース名 = null;
                if (value != null)
                {
                    // CDに該当する名称を取得
                    Course course = Course.GetCourse((int)_コースＣＤ);
                    if (course != null)
                    {
                        コース名 = course.コース名;
                    }
                }
                else
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
            }
        }

        public string 短冊印刷コース名
        {
            get { return _短冊印刷コース名; }
            set
            {
                _短冊印刷コース名 = value;
                _errors.Remove("短冊印刷コース名");
                if (value == null)
                {
                    _errors["短冊印刷コース名"] = "印刷名を入力してください。";
                }
            }
        }


        public int? コースＣＤorg
        {
            get { return _コースＣＤorg; }
            set { _コースＣＤorg = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public override string Code
        {
            get { return string.Format("コースＣＤ=[{0}]", (コースＣＤ == コースＣＤorg ? コースＣＤ : コースＣＤorg ?? コースＣＤ)); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get { return null;}
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
        public bool Equals(TanzakuCourseName target)
        {
            if (コースＣＤ == target.コースＣＤ &&
                短冊印刷コース名 == target.短冊印刷コース名)
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
            string sql = "\r\n INSERT INTO 短冊コース名マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,短冊印刷コース名" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@短冊印刷コース名" +
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
            string sql = "\r\n UPDATE 短冊コース名マスタ" +
                         "\r\n SET コースＣＤ       = @コースＣＤ" +
                         "\r\n    ,短冊印刷コース名 = @短冊印刷コース名" +
                         "\r\n    ,修正担当者ＣＤ   = @修正担当者ＣＤ" +
                         "\r\n    ,修正日           = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤorg" +
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
            string sql = "\r\n DELETE FROM 短冊コース名マスタ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤorg" +
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

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コースＣＤorg != null)
                    {
                        cmd.Parameters.Add("@コースＣＤorg", SqlDbType.Int).Value = コースＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (短冊印刷コース名 != null)
                    {
                        cmd.Parameters.Add("@短冊印刷コース名", SqlDbType.VarChar).Value = 短冊印刷コース名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@短冊印刷コース名", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("短冊コース名マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "コースＣＤが重複しています。";
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

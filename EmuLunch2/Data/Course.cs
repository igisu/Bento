using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コースマスタ
    /// </summary>
    public class Course : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="coursecd">コースＣＤ</param>
        /// <returns></returns>
        public static Course GetCourse(int coursecd)
        {
            return GetCourse(coursecd, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="coursecd">コースＣＤ</param>
        /// <returns></returns>
        public static Course GetCourseReload(int coursecd)
        {
            return GetCourse(coursecd, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="coursecd">コースＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static Course GetCourse(int coursecd, bool isValid)
        {
            Course course = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.コースＣＤ" +
                             "\r\n     ,A.コース区分" +
                             "\r\n     ,VC.コース区分名" +
                             "\r\n     ,A.コース名" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,B.部署名" +
                             "\r\n     ,A.担当者ＣＤ" +
                             "\r\n     ,A3.担当者名" +
                             "\r\n     ,A.工場フラグ" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     コースマスタ AS A" +
                             "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ AND ISNULL(B.無効フラグ, 0) = 0" +
                             "\r\n     LEFT JOIN V_コース区分 AS VC ON VC.コース区分CD = A.コース区分" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A.企業ＣＤ = A1.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A.企業ＣＤ = A2.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A.企業ＣＤ = A3.企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.コースＣＤ = @コースＣＤ" +
                             "{0}";

                string where = "";
                if (isValid)
                {
                    where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
                }

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(sql, where);
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = coursecd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コースマスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    course = new Course(dt.Rows[0]);
                }
            }

            return course;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _コースＣＤ;
        private int? _コース区分;
        private string _コース名;
        private int? _部署ＣＤ;
        private int? _担当者ＣＤ;
        private bool? _工場フラグ;
        private bool? _無効フラグ;

        private string _コース区分名;
        private string _部署名;
        private string _担当者名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Course()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Course(Course src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            コースＣＤ = src.コースＣＤ;
            コース区分 = src.コース区分;
            コース名 = src.コース名;
            部署ＣＤ = src.部署ＣＤ;
            担当者ＣＤ = src.担当者ＣＤ;
            工場フラグ = src.工場フラグ;
            無効フラグ = src.無効フラグ;

            コース区分名 = src.コース区分名;
            部署名 = src.部署名;
            担当者名 = src.担当者名;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Course(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            コースＣＤ = dr["コースＣＤ", version] as int?;
            コース区分 = dr["コース区分", version] as int?;
            コース名 = dr["コース名", version] as string;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            担当者ＣＤ = dr["担当者ＣＤ", version] as int?;
            工場フラグ = dr["工場フラグ", version] as bool?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("コース区分名"))
            {
                コース区分名 = dr["コース区分名", version] as string;
            }

            if (dr.Table.Columns.Contains("部署名"))
            {
                部署名 = dr["部署名", version] as string;
            }

            if (dr.Table.Columns.Contains("担当者名"))
            {
                担当者名 = dr["担当者名", version] as string;
            }

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
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
                if (value == null)
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
            }
        }

        public int? コース区分
        {
            get { return _コース区分; }
            set
            {
                _コース区分 = value;
                _errors.Remove("コース区分");
                if (value == null || value < 0)
                {
                    _errors["コース区分"] = "コース区分を入力してください。";
                }
            }
        }

        public string コース名
        {
            get { return _コース名; }
            set
            {
                _コース名 = value;
                _errors.Remove("コース名");
                if (value == null)
                {
                    _errors["コース名"] = "コース名を入力してください。";
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

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set
            {
                _担当者ＣＤ = value;
                _errors.Remove("担当者ＣＤ");
                担当者名 = null;
                if (value == null)
                {
                    _errors["担当者ＣＤ"] = "担当者ＣＤを入力してください。";
                }
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // 担当者ＣＤに該当する担当者名を取得
                        Tantousya tantousya = Tantousya.GetTantousya((int)_担当者ＣＤ);
                        if (tantousya != null)
                        {
                            担当者名 = tantousya.担当者名;
                        }
                        else
                        {
                            _errors["担当者ＣＤ"] = "担当者ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public bool? 工場フラグ
        {
            get { return _工場フラグ; }
            set { _工場フラグ = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string コース区分名
        {
            get { return _コース区分名; }
            set { _コース区分名 = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set { _担当者名 = value; }
        }

        public override string Code
        {
            get { return string.Format("コースＣＤ=[{0}]", コースＣＤ); }
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
        public bool Equals(Course target)
        {
            if (コースＣＤ == target.コースＣＤ &&
                (コース区分 ?? -1) == (target.コース区分 ?? -1) &&
                コース名 == target.コース名 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                担当者ＣＤ == target.担当者ＣＤ &&
                (工場フラグ ?? false) == (target.工場フラグ ?? false) &&
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
            string sql = "\r\n INSERT INTO コースマスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,コース区分" +
                         "\r\n     ,コース名" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,担当者ＣＤ" +
                         "\r\n     ,工場フラグ" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@コース区分" +
                         "\r\n     ,@コース名" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@担当者ＣＤ" +
                         "\r\n     ,@工場フラグ" +
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
            string sql = "\r\n UPDATE コースマスタ" +
                         "\r\n SET コース区分     = @コース区分" +
                         "\r\n    ,コース名       = @コース名" +
                         "\r\n    ,部署ＣＤ       = @部署ＣＤ" +
                         "\r\n    ,担当者ＣＤ     = @担当者ＣＤ" +
                         "\r\n    ,工場フラグ     = @工場フラグ" +
                         "\r\n    ,無効フラグ     = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ   = @コースＣＤ" +
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
            string sql = "\r\n DELETE FROM コースマスタ" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ   = @コースＣＤ" +
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

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コース区分 != null && コース区分 != -1)
                    {
                        cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = コース区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コース名 != null)
                    {
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = コース名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (工場フラグ != null)
                    {
                        cmd.Parameters.Add("@工場フラグ", SqlDbType.Bit).Value = 工場フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@工場フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("コースマスタを{0}しました。", historyProcess.DisplayName());
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

        /// <summary>
        /// 指定日に該当するコース区分を返す
        /// </summary>
        /// <param name="targetDate">コース区分取得対象日</param>
        /// <returns>指定日に該当するコース区分</returns>
        public static int GetCourseKubun(DateTime? targetDate)
        {
            try
            {
                // 注文日(の曜日)からコース区分を求める
                int courseKubun = (int)Consts.CourseKubun.Weekday;

                // 祝日を取得
                Shukujitsu shukujitsu = Shukujitsu.GetShukujitsu((DateTime)targetDate);
                if (shukujitsu != null)
                {
                    courseKubun = (int)Consts.CourseKubun.Holiday;  // 祝日
                }
                else
                {
                    switch (targetDate.Value.DayOfWeek)
                    {
                        case DayOfWeek.Saturday:        // 土曜
                            courseKubun = (int)Consts.CourseKubun.Saturday;
                            break;

                        case DayOfWeek.Sunday:          // 日曜
                            courseKubun = (int)Consts.CourseKubun.Sunday;
                            break;

                        default:                        // 平日
                            courseKubun = (int)Consts.CourseKubun.Weekday;
                            break;
                    }
                }

                return courseKubun;
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

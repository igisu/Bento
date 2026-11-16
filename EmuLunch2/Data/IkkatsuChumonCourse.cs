using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 一括注文入力コース入力
    /// </summary>
    public class IkkatsuChumonCourse : IDataErrorInfo
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 得意先ＣＤに該当する各区分のコースを返します
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static IkkatsuChumonCourse GetIkkatsuChumonCourse(int busyoCD, int tokuisakiCD)
        {
            IkkatsuChumonCourse ikkatsuChumonCourse = new IkkatsuChumonCourse();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     M.コースＣＤ" +
                             "\r\n     ,M.コース名" +
                             "\r\n     ,M.コース区分" +
                             "\r\n FROM" +
                             "\r\n     コースマスタ AS M " +
                             "\r\n     INNER JOIN コーステーブル AS T " +
                             "\r\n     ON M.企業ＣＤ = T.企業ＣＤ " +
                             "\r\n     AND M.コースＣＤ = T.コースＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     M.企業ＣＤ = @企業ＣＤ" +
                             "\r\n     AND M.部署ＣＤ = @部署ＣＤ" +
                             "\r\n     AND ISNULL(M.無効フラグ, 0) = 0" +
                             "\r\n     AND T.得意先ＣＤ = @得意先ＣＤ" +
                             "\r\n     AND ISNULL(T.無効フラグ, 0) = 0" +
                             "\r\n ORDER BY" +
                             "\r\n     M.コースＣＤ DESC";      // 降順で後優先（つまり昇順で先優先）

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コーステーブル");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    ikkatsuChumonCourse.stopOverwrite = true;
                    foreach (DataRow dr in dt.Rows)
                    {
                        switch ((Consts.CourseKubun)((int)dr["コース区分"]))
                        {
                            case Consts.CourseKubun.Weekday:        // 平日
                                ikkatsuChumonCourse.平日コースＣＤ = dr["コースＣＤ"] as int?;
                                ikkatsuChumonCourse.平日コース名 = dr["コース名"] as string;
                                break;

                            case Consts.CourseKubun.Saturday:       // 土曜
                                ikkatsuChumonCourse.土曜コースＣＤ = dr["コースＣＤ"] as int?;
                                ikkatsuChumonCourse.土曜コース名 = dr["コース名"] as string;
                                break;

                            case Consts.CourseKubun.Sunday:         // 日曜
                                ikkatsuChumonCourse.日曜コースＣＤ = dr["コースＣＤ"] as int?;
                                ikkatsuChumonCourse.日曜コース名 = dr["コース名"] as string;
                                break;

                            case Consts.CourseKubun.Holiday:        // 祝日
                                ikkatsuChumonCourse.祝日コースＣＤ = dr["コースＣＤ"] as int?;
                                ikkatsuChumonCourse.祝日コース名 = dr["コース名"] as string;
                                break;
                        }
                    }
                }
            }

            return ikkatsuChumonCourse;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        public bool stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _平日コースＣＤ;
        private string _平日コース名;
        private int? _土曜コースＣＤ;
        private string _土曜コース名;
        private int? _日曜コースＣＤ;
        private string _日曜コース名;
        private int? _祝日コースＣＤ;
        private string _祝日コース名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IkkatsuChumonCourse()
        {
            stopOverwrite = false;

            平日コースＣＤ = null;
            平日コース名 = null;
            土曜コースＣＤ = null;
            土曜コース名 = null;
            日曜コースＣＤ = null;
            日曜コース名 = null;
            祝日コースＣＤ = null;
            祝日コース名 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public IkkatsuChumonCourse(IkkatsuChumonCourse src)
        {
            stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            平日コースＣＤ = src.平日コースＣＤ;
            平日コース名 = src.平日コース名;
            土曜コースＣＤ = src.土曜コースＣＤ;
            土曜コース名 = src.土曜コース名;
            日曜コースＣＤ = src.日曜コースＣＤ;
            日曜コース名 = src.日曜コース名;
            祝日コースＣＤ = src.祝日コースＣＤ;
            祝日コース名 = src.祝日コース名;

            stopOverwrite = false;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 平日コースＣＤ
        {
            get { return _平日コースＣＤ; }
            set
            {
                _平日コースＣＤ = value;
                平日コース名 = null;
                _errors.Remove("平日コースＣＤ");
                if (value != null)
                {
                    if (stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_平日コースＣＤ);
                        if (course != null)
                        {
                            平日コース名 = course.コース名;
                        }
                        else
                        {
                            _errors["平日コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public string 平日コース名
        {
            get { return _平日コース名; }
            set { _平日コース名 = value; }
        }

        public int? 土曜コースＣＤ
        {
            get { return _土曜コースＣＤ; }
            set
            {
                _土曜コースＣＤ = value;
                土曜コース名 = null;
                _errors.Remove("土曜コースＣＤ");
                if (value != null)
                {
                    if (stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_土曜コースＣＤ);
                        if (course != null)
                        {
                            土曜コース名 = course.コース名;
                        }
                        else
                        {
                            _errors["土曜コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public string 土曜コース名
        {
            get { return _土曜コース名; }
            set { _土曜コース名 = value; }
        }

        public int? 日曜コースＣＤ
        {
            get { return _日曜コースＣＤ; }
            set
            {
                _日曜コースＣＤ = value;
                日曜コース名 = null;
                _errors.Remove("日曜コースＣＤ");
                if (value != null)
                {
                    if (stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_日曜コースＣＤ);
                        if (course != null)
                        {
                            日曜コース名 = course.コース名;
                        }
                        else
                        {
                            _errors["日曜コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public string 日曜コース名
        {
            get { return _日曜コース名; }
            set { _日曜コース名 = value; }
        }

        public int? 祝日コースＣＤ
        {
            get { return _祝日コースＣＤ; }
            set
            {
                _祝日コースＣＤ = value;
                祝日コース名 = null;
                _errors.Remove("祝日コースＣＤ");
                if (value != null)
                {
                    if (stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_祝日コースＣＤ);
                        if (course != null)
                        {
                            祝日コース名 = course.コース名;
                        }
                        else
                        {
                            _errors["祝日コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public string 祝日コース名
        {
            get { return _祝日コース名; }
            set { _祝日コース名 = value; }
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
        public bool Equals(IkkatsuChumonCourse target)
        {
            if (平日コースＣＤ == target.平日コースＣＤ &&
                土曜コースＣＤ == target.土曜コースＣＤ &&
                日曜コースＣＤ == target.日曜コースＣＤ &&
                祝日コースＣＤ == target.祝日コースＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

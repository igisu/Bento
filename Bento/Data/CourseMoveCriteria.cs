using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コース移動検索条件
    /// </summary>
    public class CourseMoveCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _コースＣＤ;

        private string _コース名;
        private int? _コース区分;
        private string _コース区分名;
        private int? _部署ＣＤ;
        private string _部署名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CourseMoveCriteria()
        {
            コースＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public CourseMoveCriteria(CourseMoveCriteria src)
        {
            コースＣＤ = src.コースＣＤ;
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
                コース名 = null;
                _errors.Remove("コースＣＤ");
                if (value == null)
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
                else
                {
                    // CDに該当する名称を取得
                    Course course = Course.GetCourse((int)_コースＣＤ);
                    if (course != null)
                    {
                        コース名 = course.コース名;
                        コース区分 = course.コース区分;
                        コース区分名 = course.コース区分名;
                        部署ＣＤ = course.部署ＣＤ;
                        部署名 = course.部署名;
                    }
                    else
                    {
                        _errors["コースＣＤ"] = "コースＣＤが見つかりません。";
                    }
                }
            }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public int? コース区分
        {
            get { return _コース区分; }
            set { _コース区分 = value; }
        }

        public string コース区分名
        {
            get { return _コース区分名; }
            set { _コース区分名 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get
            {
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
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(CourseMoveCriteria target)
        {
            if (コースＣＤ == target.コースＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

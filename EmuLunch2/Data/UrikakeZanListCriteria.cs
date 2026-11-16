using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売掛残高一覧検索条件
    /// </summary>
    public class UrikakeZanListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _請求年;
        private int? _請求月;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _得意先ＣＤ;
        private bool? _売掛残ゼロ;
        private int? _締区分;

        private string _コース名;
        private string _得意先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UrikakeZanListCriteria()
        {
            DateTime today = DateTime.Today;
            請求年 = today.Year;
            請求月 = today.Month;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            コースＣＤ = null;
            得意先ＣＤ = null;
            売掛残ゼロ = false;
            締区分 = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 請求年
        {
            get { return _請求年; }
            set
            {
                _請求年 = value;
                _errors.Remove("請求年");
                if (value == null)
                {
                    _errors["請求年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["請求年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 請求月
        {
            get { return _請求月; }
            set
            {
                _請求月 = value;
                _errors.Remove("請求月");
                if (value == null)
                {
                    _errors["請求月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["請求月"] = "1～12を入力してください。";
                    }
                }
            }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
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
            }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                得意先名 = null;
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                    if (tokuisaki != null)
                    {
                        得意先名 = tokuisaki.得意先名;
                    }
                }
            }
        }

        public bool? 売掛残ゼロ
        {
            get { return _売掛残ゼロ; }
            set { _売掛残ゼロ = value; }
        }

        public int? 締区分
        {
            get { return _締区分; }
            set { _締区分 = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public DateTime? 請求年月日From
        {
            get
            {
                DateTime? d = null;
                // 請求年・請求月にエラーが無ければ、その一日を求める
                if (_errors.ContainsKey("請求年") == false && _errors.ContainsKey("請求月") == false)
                {
                    d = new DateTime((int)_請求年, (int)_請求月, 1);
                }
                return d;
            }
        }

        public DateTime? 請求年月日To
        {
            get
            {
                DateTime? d = null;
                // 請求年・請求月にエラーが無ければ、その末日を求める
                if (_errors.ContainsKey("請求年") == false && _errors.ContainsKey("請求月") == false)
                {
                    d = new DateTime((int)_請求年, (int)_請求月, DateTime.DaysInMonth((int)_請求年, (int)_請求月));
                }
                return d;
            }
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
    }
}

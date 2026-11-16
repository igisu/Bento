using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 未回収金一覧検索条件
    /// </summary>
    public class MikaisyuListCriteria : IDataErrorInfo
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
        private int? _支払方法;
        private string _EB銀行支店ＣＤ;   // 銀行ＣＤ支店ＣＤ
        private int? _締区分;
        private int? _月締日;
        private bool? _売掛残ゼロ;

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
        public MikaisyuListCriteria()
        {
            DateTime today = DateTime.Today;
            請求年 = today.Year;
            請求月 = today.Month;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            コースＣＤ = null;
            得意先ＣＤ = null;
            支払方法 = null;
            EB銀行支店ＣＤ = null;
            締区分 = null;
            月締日 = null;
            売掛残ゼロ = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public MikaisyuListCriteria(MikaisyuListCriteria src)
        {
            請求年 = src.請求年;
            請求月 = src.請求月;
            部署ＣＤ = src.部署ＣＤ;
            _コースＣＤ = src.コースＣＤ;
            _得意先ＣＤ = src.得意先ＣＤ;
            支払方法 = src.支払方法;
            EB銀行支店ＣＤ = src.EB銀行支店ＣＤ;
            締区分 = src.締区分;
            月締日 = src.月締日;
            売掛残ゼロ = src.売掛残ゼロ;

            コース名 = src.コース名;
            得意先名 = src.得意先名;
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

        public int? 支払方法
        {
            get { return _支払方法; }
            set { _支払方法 = value; }
        }

        public string EB銀行支店ＣＤ
        {
            get { return _EB銀行支店ＣＤ; }
            set { _EB銀行支店ＣＤ = value;}
        }

        public int? 締区分
        {
            get { return _締区分; }
            set { _締区分 = value; }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (value != null)
                {
                    if (1 <= value && value <= 28)
                    {
                    }
                    else if (value == 29 || value == 30 || value == 31)
                    {
                        _errors["月締日"] = "月末の場合は99を入力してください。";
                    }
                    else if (value == 99)
                    {
                    }
                    else
                    {
                        _errors["月締日"] = "1～28 または 月末の場合は99を入力してください。";
                    }
                }
            }
        }

        public bool? 売掛残ゼロ
        {
            get { return _売掛残ゼロ; }
            set { _売掛残ゼロ = value; }
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

        public DateTime? 請求年月日
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(MikaisyuListCriteria target)
        {
            if (請求年 == target.請求年 &&
                請求月 == target.請求月 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                コースＣＤ == target.コースＣＤ &&
                得意先ＣＤ == target.得意先ＣＤ &&
                (支払方法 ?? -1) == (target.支払方法 ?? -1) &&
                EB銀行支店ＣＤ == target.EB銀行支店ＣＤ &&
                (締区分 ?? -1) == (target.締区分 ?? -1) &&
                (月締日 ?? -1) == (target.月締日 ?? -1) &&
                (売掛残ゼロ ?? false) == (target.売掛残ゼロ ?? false))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

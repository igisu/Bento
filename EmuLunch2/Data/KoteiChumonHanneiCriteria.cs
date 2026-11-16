using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 固定注文反映条件
    /// </summary>
    public class KoteiChumonHanneiCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _部署ＣＤ;
        private DateTime? _適用日From;
        private DateTime? _適用日To;
        private int? _コースＣＤFrom;
        private int? _コースＣＤTo;
        private int? _得意先ＣＤFrom;
        private int? _得意先ＣＤTo;
        private string _コース名From;
        private string _コース名To;
        private string _得意先名From;
        private string _得意先名To;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KoteiChumonHanneiCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            適用日From = DateTime.Today;
            適用日To = DateTime.Today;
            コースＣＤFrom = null;
            コースＣＤTo = null;
            得意先ＣＤFrom = null;
            得意先ＣＤTo = null;

            得意先名From = null;
            得意先名To = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
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

        public DateTime? 適用日From
        {
            get { return _適用日From; }
            set
            {
                _適用日From = value;
                _errors.Remove("適用日From");
                if (value == null)
                {
                    _errors["適用日From"] = "適用日Fromを入力してください。";
                }
            }
        }

        public DateTime? 適用日To
        {
            get { return _適用日To; }
            set
            {
                _適用日To = value;
                _errors.Remove("適用日To");
                if (value == null)
                {
                    _errors["適用日To"] = "適用日Toを入力してください。";
                }
            }
        }

        public int? コースＣＤFrom
        {
            get { return _コースＣＤFrom; }
            set
            {
                _コースＣＤFrom = value;
                コース名From = null;
                _errors.Remove("コースＣＤFrom");
                if (value != null)
                {
                    // コースＣＤに該当するコース名を取得
                    Course course = Course.GetCourse((int)_コースＣＤFrom);
                    if (course != null)
                    {
                        コース名From = course.コース名;
                    }
                    else
                    {
                        _errors["コースＣＤFrom"] = "コースＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? コースＣＤTo
        {
            get { return _コースＣＤTo; }
            set
            {
                _コースＣＤTo = value;
                コース名To = null;
                _errors.Remove("コースＣＤTo");
                if (value != null)
                {
                    // コースＣＤに該当するコース名を取得
                    Course course = Course.GetCourse((int)_コースＣＤTo);
                    if (course != null)
                    {
                        コース名To = course.コース名;
                    }
                    else
                    {
                        _errors["コースＣＤTo"] = "コースＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 得意先ＣＤFrom
        {
            get { return _得意先ＣＤFrom; }
            set {
                _得意先ＣＤFrom = value;
                得意先名From = null;
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤFrom);
                    if (tokuisaki != null)
                    {
                        得意先名From = tokuisaki.得意先名;
                    }
                }
            }
        }

        public int? 得意先ＣＤTo
        {
            get { return _得意先ＣＤTo; }
            set {
                _得意先ＣＤTo = value;
                得意先名To = null;
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤTo);
                    if (tokuisaki != null)
                    {
                        得意先名To = tokuisaki.得意先名;
                    }
                }
            }
        }


        public string コース名From
        {
            get { return _コース名From; }
            set { _コース名From = value; }
        }

        public string コース名To
        {
            get { return _コース名To; }
            set { _コース名To = value; }
        }

        public string 得意先名From
        {
            get { return _得意先名From; }
            set { _得意先名From = value; }
        }

        public string 得意先名To
        {
            get { return _得意先名To; }
            set { _得意先名To = value; }
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
                // 適用日のFrom～Toをチェック
                if (適用日From != null && 適用日To != null)
                {
                    if (適用日To < 適用日From)
                    {
                        return "適用日の指定が逆です。";
                    }
                }

                // コースＣＤのFrom～Toをチェック
                if (_コースＣＤFrom != null && _コースＣＤTo != null)
                {
                    if (_コースＣＤTo < _コースＣＤFrom)
                    {
                        return "コースＣＤの指定が逆です。";
                    }
                }

                // 得意先ＣＤのFrom～Toをチェック
                if (_得意先ＣＤFrom != null && _得意先ＣＤTo != null)
                {
                    if (_得意先ＣＤTo < _得意先ＣＤFrom)
                    {
                        return "得意先ＣＤの指定が逆です。";
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
    }
}

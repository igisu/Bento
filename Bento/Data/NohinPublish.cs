using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 納品書発行
    /// </summary>
    public class NohinPublish : IDataErrorInfo
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
        private bool _配送日指定;
        private bool _売上日指定;
        private DateTime? _配送日;
        private DateTime? _売上日;
        private int? _コースＣＤFrom;
        private int? _コースＣＤTo;
        private int? _得意先ＣＤFrom;
        private int? _得意先ＣＤTo;
        private bool? _印刷済みフラグ;              // 印刷済みのものも印刷する
        private bool? _金額印字フラグ;              // 納品書に金額を印字する
        private bool? _見出しフラグ;                // 見出しを「納品書兼請求書」にする
        private bool? _納品書発行区分使用フラグ;    // 得意先マスタの「納品書発行区分」に従う
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
        public NohinPublish()
        {
            _部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            _配送日指定 = true;
            _売上日指定 = false;
            _配送日 = DateTime.Today;
            _売上日 = DateTime.Today;
            _コースＣＤFrom = null;
            _コースＣＤTo = null;
            _得意先ＣＤFrom = null;
            _得意先ＣＤTo = null;
            _印刷済みフラグ = false;
            _金額印字フラグ = false;
            _見出しフラグ = false;
            _納品書発行区分使用フラグ = false;
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

        public bool 配送日指定
        {
            get { return _配送日指定; }
            set
            {
                _配送日指定 = value;
                _売上日指定 = !(value);
                _errors.Remove("配送日");
                _errors.Remove("売上日");
                if (value)
                {
                    // ↓ここでやると上手くいかないので画面にて実施
                    //_配送日 = _売上日;
                    //_売上日 = null;
                    if (_配送日 == null)
                    {
                        _errors["配送日"] = "配送日を入力してください。";
                    }
                }
            }
        }

        public bool 売上日指定
        {
            get { return _売上日指定; }
            set
            {
                _売上日指定 = value;
                _配送日指定 = !(value);
                _errors.Remove("配送日");
                _errors.Remove("売上日");
                if (value)
                {
                    // ↓ここでやると上手くいかないので画面にて実施
                    //_売上日 = _配送日;
                    //_配送日 = null;
                    if (_売上日 == null)
                    {
                        _errors["売上日"] = "売上日を入力してください。";
                    }
                }
            }
        }

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

        public DateTime? 売上日
        {
            get { return _売上日; }
            set
            {
                _売上日 = value;
                _errors.Remove("売上日");
                if (_売上日指定)
                {
                    if (value == null)
                    {
                        _errors["売上日"] = "売上日を入力してください。";
                    }
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
            set
            {
                _得意先ＣＤFrom = value;
                得意先名From = null;
                _errors.Remove("得意先ＣＤFrom");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤFrom);
                    if (tokuisaki != null)
                    {
                        得意先名From = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["得意先ＣＤFrom"] = "得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 得意先ＣＤTo
        {
            get { return _得意先ＣＤTo; }
            set
            {
                _得意先ＣＤTo = value;
                得意先名To = null;
                _errors.Remove("得意先ＣＤTo");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤTo);
                    if (tokuisaki != null)
                    {
                        得意先名To = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["得意先ＣＤTo"] = "得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public bool? 印刷済みフラグ
        {
            get { return _印刷済みフラグ; }
            set { _印刷済みフラグ = value; }
        }

        public bool? 金額印字フラグ
        {
            get { return _金額印字フラグ; }
            set { _金額印字フラグ = value; }
        }

        public bool? 見出しフラグ
        {
            get { return _見出しフラグ; }
            set { _見出しフラグ = value; }
        }

        public bool? 納品書発行区分使用フラグ
        {
            get { return _納品書発行区分使用フラグ; }
            set { _納品書発行区分使用フラグ = value; }
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

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求書発行
    /// </summary>
    public class SeikyuPublish : IDataErrorInfo
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
        private DateTime? _請求日;
        private int? _コースＣＤFrom;
        private int? _コースＣＤTo;
        private int? _請求先ＣＤFrom;
        private int? _請求先ＣＤTo;
        private int? _締区分;
        private bool? _印刷済みフラグ;      // 印刷済みのものも印刷する
        private bool? _ゼロ印刷フラグ;      // 請求金額が0の請求書も印刷する
        private string _コース名From;
        private string _コース名To;
        private string _請求先名From;
        private string _請求先名To;
        private bool? _コース外;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuPublish()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            請求日 = DateTime.Today;
            コースＣＤFrom = null;
            コースＣＤTo = null;
            請求先ＣＤFrom = null;
            請求先ＣＤTo = null;
            印刷済みフラグ = false;
            ゼロ印刷フラグ = false;
            コース外 = false;
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

        public DateTime? 請求日
        {
            get { return _請求日; }
            set
            {
                _請求日 = value;
                _errors.Remove("請求日");
                if (value == null)
                {
                    _errors["請求日"] = "請求日を入力してください。";
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

        public int? 請求先ＣＤFrom
        {
            get { return _請求先ＣＤFrom; }
            set
            {
                _請求先ＣＤFrom = value;
                請求先名From = null;
                _errors.Remove("請求先ＣＤFrom");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤFrom);
                    if (tokuisaki != null)
                    {
                        請求先名From = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["請求先ＣＤFrom"] = "得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 請求先ＣＤTo
        {
            get { return _請求先ＣＤTo; }
            set
            {
                _請求先ＣＤTo = value;
                請求先名To = null;
                _errors.Remove("請求先ＣＤTo");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤTo);
                    if (tokuisaki != null)
                    {
                        請求先名To = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["請求先ＣＤTo"] = "得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 締区分
        {
            get { return _締区分; }
            set { _締区分 = value; }
        }

        public bool? 印刷済みフラグ
        {
            get { return _印刷済みフラグ; }
            set { _印刷済みフラグ = value; }
        }

        public bool? ゼロ印刷フラグ
        {
            get { return _ゼロ印刷フラグ; }
            set { _ゼロ印刷フラグ = value; }
        }

        public bool? コース外
        {
            get { return _コース外; }
            set { _コース外 = value; }
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

        public string 請求先名From
        {
            get { return _請求先名From; }
            set { _請求先名From = value; }
        }

        public string 請求先名To
        {
            get { return _請求先名To; }
            set { _請求先名To = value; }
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
                // コース外の場合はコースＣＤをクリアする
                if (_コース外 == true)
                {
                    コースＣＤFrom = null;
                    コースＣＤTo = null;
                }
                else
                {
                    // コースＣＤのFrom～Toをチェック
                    if (_コースＣＤFrom != null && _コースＣＤTo != null)
                    {
                        if (_コースＣＤTo < _コースＣＤFrom)
                        {
                            return "コースＣＤの指定が逆です。";
                        }
                    }
                }

                // 得意先ＣＤのFrom～Toをチェック
                if (_請求先ＣＤFrom != null && _請求先ＣＤTo != null)
                {
                    if (_請求先ＣＤTo < _請求先ＣＤFrom)
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

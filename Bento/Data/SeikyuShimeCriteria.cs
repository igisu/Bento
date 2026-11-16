using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求締め条件
    /// </summary>
    public class SeikyuShimeCriteria : IDataErrorInfo
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
        private int? _締区分;
        private DateTime? _請求日;
        private int? _週締曜日;
        private bool? _週特締;
        private int? _月締年;
        private int? _月締月;
        private int? _月締日;
        private bool? _月特締;
        private int? _コースＣＤ;
        private int? _請求先ＣＤFrom;
        private int? _請求先ＣＤTo;
        private bool? _コース外;

        private string _コース名;
        private string _請求先名From;
        private string _請求先名To;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuShimeCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            締区分 = (int)Consts.ShimeKubun.Monthly;        // 初期値は月締
            DateTime today = DateTime.Today;
            請求日 = null;
            月締年 = today.Year;
            月締月 = today.Month;
            月締日 = 99;
            週締曜日 = null;
            週特締 = false;
            月特締 = false;
            コースＣＤ = null;
            請求先ＣＤFrom = null;
            請求先ＣＤTo = null;
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

        public int? 締区分
        {
            get { return _締区分; }
            set
            {
                _締区分 = value;
                _errors.Remove("締区分");
                if (value == null || value < 0)
                {
                    _errors["締区分"] = "締区分を入力してください。";
                }
                // 締区分に応じた項目のチェック
                ShimeKubunCheck();
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

        public int? 週締曜日
        {
            get { return _週締曜日; }
            set
            {
                _週締曜日 = value;
                _errors.Remove("週締曜日");
                if (value == null || value < 0)
                {
                    _errors["週締曜日"] = "週締曜日を入力してください。";
                }
            }
        }

        public bool? 週特締
        {
            get { return _週特締; }
            set { _週特締 = value; }
        }

        public int? 月締年
        {
            get { return _月締年; }
            set
            {
                _月締年 = value;
                _errors.Remove("月締年");
                if (value == null)
                {
                    _errors["月締年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["月締年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 月締月
        {
            get { return _月締月; }
            set
            {
                _月締月 = value;
                _errors.Remove("月締月");
                if (value == null)
                {
                    _errors["月締月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["月締月"] = "1～12を入力してください。";
                    }
                }
            }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (value == null)
                {
                    _errors["月締日"] = "締日を 1～28 または 月末の場合は99で入力してください。";
                }
                else
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

        public bool? 月特締
        {
            get { return _月特締; }
            set { _月特締 = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                _errors.Remove("コースＣＤ");
                if (value != null)
                {
                    // コースＣＤに該当するコース名を取得
                    Course course = Course.GetCourse((int)_コースＣＤ);
                    if (course != null)
                    {
                        コース名 = course.コース名;
                    }
                    else
                    {
                        _errors["コースＣＤ"] = "コースＣＤが見つかりません。";
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

        public bool? コース外
        {
            get { return _コース外; }
            set { _コース外 = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
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
                // 締区分に応じた項目のチェック
                ShimeKubunCheck();

                // 月締？
                if (_締区分 == (int)Consts.ShimeKubun.Monthly)
                {
                    // 年月と締日の妥当性をチェック
                    if (_月締年 != null && _errors.ContainsKey("月締年") == false
                        && 月締月 != null && _errors.ContainsKey("月締月") == false
                        && _月締日 != null && _errors.ContainsKey("月締日") == false)
                    {
                        if (_月締日 != 99)
                        {
                            if (_月締日 > DateTime.DaysInMonth((int)_月締年, (int)月締月))
                            {
                                return "年月に存在する締日を入力してください。";
                            }
                        }
                    }
                }

                // 週締？
                if (_締区分 == (int)Consts.ShimeKubun.Weekly)
                {
                    // 週締曜日の必須チェックだけなので、ここで行うエラーチェックは無い
                }

                // 得意先ＣＤのFrom～Toをチェック
                if (_請求先ＣＤFrom != null && _請求先ＣＤTo != null)
                {
                    if (_請求先ＣＤTo < _請求先ＣＤFrom)
                    {
                        return "得意先ＣＤの指定が逆です。";
                    }
                }

                // コース外の場合はコースＣＤをクリアする
                if (_コース外 == true)
                {
                    コースＣＤ = null;
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
        /// 締区分に応じた項目のチェック
        /// </summary>
        private void ShimeKubunCheck()
        {
            switch (_締区分)
            {
                case (int)Consts.ShimeKubun.Weekly:     // 週締
                    // 月締に関する項目のエラーをクリア
                    _errors.Remove("月締年");
                    _errors.Remove("月締月");
                    _errors.Remove("月締日");

                    // 週締めに関する項目のエラーチェックを実施するため、プロパティを再設定
                    週締曜日 = 週締曜日;
                    break;

                case (int)Consts.ShimeKubun.Monthly:    // 月締
                    // 週締に関する項目のエラーをクリア
                    _errors.Remove("週締曜日");

                    // 月締めに関する項目のエラーチェックを実施するため、プロパティを再設定
                    月締年 = 月締年;
                    月締月 = 月締月;
                    月締日 = 月締日;
                    break;

                default:                                // 上記以外
                    // 週締と月締に関する項目のエラーをクリア
                    _errors.Remove("週締曜日");
                    _errors.Remove("月締年");
                    _errors.Remove("月締月");
                    _errors.Remove("月締日");
                    break;
            }
        }
        #endregion
    }
}

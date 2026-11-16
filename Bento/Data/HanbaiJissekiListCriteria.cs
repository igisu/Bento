using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 販売実績一覧表印刷出力条件
    /// </summary>
    public class HanbaiJissekiListCriteria : IDataErrorInfo
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
        private bool _日計;
        private bool _月計;
        private DateTime? _精算日;
        private int? _対象年;
        private int? _対象月;
        int? _コース区分;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HanbaiJissekiListCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            DateTime today = DateTime.Today;
            精算日 = today;
            対象年 = today.Year;
            対象月 = today.Month;
            日計 = true;
            月計 = false;
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
                // nullを許容する
                if (value < 0)
                {
                    _部署ＣＤ = null;
                }
            }
        }

        public bool 日計
        {
            get { return _日計; }
            set
            {
                _日計 = value;
                _月計 = !(value);
                _errors.Remove("精算日");
                _errors.Remove("対象年");
                _errors.Remove("対象月");
                if (value)
                {
                    if (_精算日 == null)
                    {
                        _errors["精算日"] = "精算日を入力してください。";
                    }
                    else
                    {
                        // 精算日(の曜日)からコース区分を求める
                        _コース区分 = Course.GetCourseKubun((DateTime)_精算日);
                    }
                }
            }
        }

        public bool 月計
        {
            get { return _月計; }
            set
            {
                _月計 = value;
                _日計 = !(value);
                _errors.Remove("精算日");
                _errors.Remove("対象年");
                _errors.Remove("対象月");
                if (value)
                {
                    if (_対象年 == null)
                    {
                        _errors["対象年"] = "西暦を入力してください。";
                    }
                    else
                    {
                        if (_対象年 <= 2000)
                        {
                            _errors["対象年"] = "西暦を入力してください。";
                        }
                    }

                    if (_対象月 == null)
                    {
                        _errors["対象月"] = "月を 1～12 で入力してください。";
                    }
                    else
                    {
                        if (_対象月 < 1 || 12 < _対象月)
                        {
                            _errors["対象月"] = "1～12を入力してください。";
                        }
                    }
                }
            }
        }

        public DateTime? 精算日
        {
            get { return _精算日; }
            set
            {
                _精算日 = value;
                _コース区分 = null;
                _errors.Remove("精算日");
                if (_日計)
                {
                    if (value == null)
                    {
                        _errors["精算日"] = "精算日を入力してください。";
                    }
                    else
                    {
                        // 精算日(の曜日)からコース区分を求める
                        _コース区分 = Course.GetCourseKubun((DateTime)_精算日);
                    }
                }
            }
        }
        public int? 対象年
        {
            get { return _対象年; }
            set
            {
                _対象年 = value;
                _errors.Remove("対象年");
                if (_月計)
                {
                    if (value == null)
                    {
                        _errors["対象年"] = "西暦を入力してください。";
                    }
                    else
                    {
                        if (value <= 2000)
                        {
                            _errors["対象年"] = "西暦を入力してください。";
                        }
                    }
                }
            }
        }

        public int? 対象月
        {
            get { return _対象月; }
            set
            {
                _対象月 = value;
                _errors.Remove("対象月");
                if (月計)
                {
                    if (value == null)
                    {
                        _errors["対象月"] = "月を 1～12 で入力してください。";
                    }
                    else
                    {
                        if (value < 1 || 12 < value)
                        {
                            _errors["対象月"] = "1～12を入力してください。";
                        }
                    }
                }
            }
        }

        public int? コース区分
        {
            get { return _コース区分; }
            set { _コース区分 = value; }
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
    }
}

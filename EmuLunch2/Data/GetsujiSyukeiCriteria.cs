using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 月次集計対象
    /// </summary>
    public class GetsujiSyukeiCriteria : IDataErrorInfo
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
        private int? _対象年;
        private int? _対象月;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GetsujiSyukeiCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            //対象年月は先月
            DateTime today = DateTime.Today;
            DateTime lastMonth = new DateTime(today.Year, today.Month, 1);
            lastMonth = lastMonth.AddMonths(-1);
            対象年 = lastMonth.Year;
            対象月 = lastMonth.Month;
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

        public int? 対象年
        {
            get { return _対象年; }
            set
            {
                _対象年 = value;
                _errors.Remove("対象年");
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

        public int? 対象月
        {
            get { return _対象月; }
            set
            {
                _対象月 = value;
                _errors.Remove("対象月");
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

        public DateTime 対象年月一日
        {
            get
            {
                DateTime targetMonth = new DateTime((int)_対象年, (int)_対象月, 1);
                return targetMonth;
            }
        }

        public DateTime 対象年月末日
        {
            get
            {
                DateTime targetMonth = new DateTime((int)_対象年, (int)_対象月, 1);
                // 翌月の前日を求める
                targetMonth = targetMonth.AddMonths(1);
                targetMonth = targetMonth.AddDays(-1);
                return targetMonth;
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
                if (_対象年 != null && _対象月 != null)
                {
                    DateTime today = DateTime.Today;
                    DateTime thisMonth = new DateTime(today.Year, today.Month, 1);

                    // 対象年月は今月または過去年月のみ指定可能（未来は不可）
                    DateTime targetMonth = new DateTime((int)_対象年, (int)_対象月, 1);
                    if (thisMonth < targetMonth)
                    {
                        return "対象年月に未来年月は入力できません。";
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

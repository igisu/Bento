using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 生産予定入力別日読込条件
    /// </summary>
    public class SeisanYoteiFromBetsubi : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _注文日;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeisanYoteiFromBetsubi()
        {
            // 直前の平日を初期値とする
            DateTime d = DateTime.Today;
            bool isWeekday = false;
            while (isWeekday == false)
            {
                d = d.AddDays(-1);
                switch (d.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        isWeekday = true;
                        break;
                }
            }
            注文日 = d;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public DateTime? 注文日
        {
            get { return _注文日; }
            set
            {
                _注文日 = value;
                _errors.Remove("注文日");
                if (value == null)
                {
                    _errors["注文日"] = "注文日を入力してください。";
                }
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

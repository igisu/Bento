using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コース出荷状態検索条件
    /// </summary>
    public class ShippingStatusCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _配送日From;
        private DateTime? _配送日To;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;

        private string _コース名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShippingStatusCriteria()
        {
            配送日From = DateTime.Today;
            配送日To = DateTime.Today;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            コースＣＤ = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 配送日From
        {
            get { return _配送日From; }
            set { _配送日From = value; }
        }

        public DateTime? 配送日To
        {
            get { return _配送日To; }
            set { _配送日To = value; }
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
                _errors.Remove("コースＣＤ");
                if (value != null)
                {
                    // CDに該当する名称を取得
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

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
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
                // 配送日のFrom～Toをチェック
                if (配送日From != null && 配送日To != null)
                {
                    if (配送日To < 配送日From)
                    {
                        return "配送日の指定が逆です。";
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

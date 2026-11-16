using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 未分配一覧検索条件
    /// </summary>
    public class MiBunpaiListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _売上日From;
        private DateTime? _売上日To;
        private int? _部署ＣＤ;
        private int? _コースＣＤFrom;
        private int? _コースＣＤTo;
        private bool? _分配途中フラグ;
        private bool? _分配完了フラグ;

        private string _コース名From;
        private string _コース名To;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MiBunpaiListCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            売上日From = DateTime.Today;
            売上日To = DateTime.Today;
            コースＣＤFrom = null;
            コースＣＤTo = null;
            分配途中フラグ = true;
            分配完了フラグ = false;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 売上日From
        {
            get { return _売上日From; }
            set { _売上日From = value; }
        }

        public DateTime? 売上日To
        {
            get { return _売上日To; }
            set { _売上日To = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? コースＣＤFrom
        {
            get { return _コースＣＤFrom; }
            set
            {
                _コースＣＤFrom = value;
                コース名From = null;
                if (value != null)
                {
                    // コースＣＤに該当するコース名を取得
                    Course course = Course.GetCourse((int)_コースＣＤFrom);
                    if (course != null)
                    {
                        コース名From = course.コース名;
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
                if (value != null)
                {
                    // コースＣＤに該当するコース名を取得
                    Course course = Course.GetCourse((int)_コースＣＤTo);
                    if (course != null)
                    {
                        コース名To = course.コース名;
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

        public bool? 分配途中フラグ
        {
            get { return _分配途中フラグ; }
            set { _分配途中フラグ = value; }
        }

        public bool? 分配完了フラグ
        {
            get { return _分配完了フラグ; }
            set { _分配完了フラグ = value; }
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
                // 売上日のFrom～Toをチェック
                if (_売上日From != null && _売上日To != null)
                {
                    if (_売上日To < _売上日From)
                    {
                        return "売上日の指定が逆です。";
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

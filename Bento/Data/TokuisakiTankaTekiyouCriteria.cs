using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先単価適用マスタ検索条件
    /// </summary>
    public class TokuisakiTankaTekiyouCriteria : IDataErrorInfo
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
        private int? _得意先ＣＤ;
        private int? _商品ＣＤ;
        private DateTime? _適用日From;
        private DateTime? _適用日To;
        private bool? _固定注文;
        private int? _コースＣＤFrom;
        private int? _コースＣＤTo;

        private string _得意先名;
        private string _商品名;
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
        public TokuisakiTankaTekiyouCriteria()
        {
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
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

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set
            {
                _商品ＣＤ = value;
                商品名 = null;
                if (value != null)
                {
                    // CDに該当する名称を取得
                    Syohin syohin = Syohin.GetSyohin((int)_商品ＣＤ);
                    if (syohin != null)
                    {
                        商品名 = syohin.商品名;
                    }
                }
            }
        }

        public DateTime? 適用日From
        {
            get { return _適用日From; }
            set { _適用日From = value; }
        }

        public DateTime? 適用日To
        {
            get { return _適用日To; }
            set { _適用日To = value; }
        }

        public bool? 固定注文
        {
            get { return _固定注文; }
            set { _固定注文 = value; }
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

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public string 商品名
        {
            get { return _商品名; }
            set { _商品名 = value; }
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

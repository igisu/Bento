using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先マスタ（検索条件用）
    /// </summary>
    public class TokuisakiCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _得意先ＣＤFrom;
        private int? _得意先ＣＤTo;
        private string _得意先名;
        private int? _部署ＣＤ;
        private string _電話番号;
        private int? _月締日;
        private int? _週締曜日;
        private int? _状態区分;
        private int? _コースＣＤ;
        private int? _請求先ＣＤ;
        private int _誕生日From月;
        private int _誕生日From日;
        private int _誕生日To月;
        private int _誕生日To日;
        private int? _集計ＣＤ;
        private bool? _無効フラグ;

        private DateTime? _対象日;

        private string _コース名;
        private string _請求先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiCriteria()
        {
            _誕生日From月 = -1;
            _誕生日From日 = -1;
            _誕生日To月 = -1;
            _誕生日To日 = -1;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤFrom
        {
            get { return _得意先ＣＤFrom; }
            set { _得意先ＣＤFrom = value; }
        }

        public int? 得意先ＣＤTo
        {
            get { return _得意先ＣＤTo; }
            set { _得意先ＣＤTo = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 電話番号
        {
            get { return _電話番号; }
            set
            {
                if (value != null)
                {
                    // ハイフン・スペース・カッコを除く
                    value = value.Replace("-", "");
                    value = value.Replace(" ", "");
                    value = value.Replace("(", "");
                    value = value.Replace(")", "");
                }
                _電話番号 = value;
            }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (value != null)
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

        public int? 週締曜日
        {
            get { return _週締曜日; }
            set { _週締曜日 = value; }
        }

        public int? 状態区分
        {
            get { return _状態区分; }
            set { _状態区分 = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                if (value != null)
                {
                    // CDに該当する名称を取得
                    Course course = Course.GetCourse((int)_コースＣＤ);
                    if (course != null)
                    {
                        コース名 = course.コース名;
                    }
                }
            }
        }

        public int? 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set
            {
                _請求先ＣＤ = value;
                請求先名 = null;
                if (value != null)
                {
                    // CDに該当する名称を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                    if (tokuisaki != null)
                    {
                        請求先名 = tokuisaki.得意先名;
                    }
                }
            }
        }

        public int 誕生日From月
        {
            get { return _誕生日From月; }
            set { _誕生日From月 = value; }
        }

        public int 誕生日From日
        {
            get { return _誕生日From日; }
            set { _誕生日From日 = value; }
        }

        public int 誕生日To月
        {
            get { return _誕生日To月; }
            set { _誕生日To月 = value; }
        }

        public int 誕生日To日
        {
            get { return _誕生日To日; }
            set { _誕生日To日 = value; }
        }

        public int? 集計ＣＤ
        {
            get { return _集計ＣＤ; }
            set { _集計ＣＤ = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public DateTime? 対象日
        {
            get { return _対象日; }
            set { _対象日 = value; }
        }


        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public string 月日From
        {
            get
            {
                return _誕生日From月 <= 0 ? null : string.Format("{0:00}{1:00}", _誕生日From月, (_誕生日From日 <= 0 ? 1 : _誕生日From日));
            }
        }

        public string 月日To
        {
            get
            {
                return _誕生日To月 <= 0 ? null : string.Format("{0:00}{1:00}", _誕生日To月, (_誕生日To日 <= 0 ? 31 : _誕生日To日));
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
                // 得意先ＣＤのFrom～Toをチェック
                if (_得意先ＣＤFrom != null && _得意先ＣＤTo != null)
                {
                    if (_得意先ＣＤTo < _得意先ＣＤFrom)
                    {
                        return "得意先ＣＤの指定が逆です。";
                    }
                }

                // 誕生日のFrom～Toをチェック
                // 日のみの入力は不可
                if (_誕生日From月 <= 0 && _誕生日From日 > 0)
                {
                    return "誕生日は月も入力してください。日だけの入力はできません。";
                }
                if (_誕生日To月 <= 0 && _誕生日To日 > 0)
                {
                    return "誕生日は月も入力してください。日だけの入力はできません。";
                }

                // いずれかにのみ入力されている
                if (月日From != null ^ 月日To != null)
                {
                    return "誕生日の範囲を指定してください。";
                }

                // 両方に入力されている
                if (月日From != null && 月日To != null)
                {
                    if (int.Parse(月日To) < int.Parse(月日From))
                    {
                        return "誕生日の指定が逆です。";
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

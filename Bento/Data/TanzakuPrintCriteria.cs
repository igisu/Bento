using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 短冊印刷条件
    /// </summary>
    public class TanzakuPrintCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool _注文日指定;
        private bool _配送日指定;
        private DateTime? _注文日;
        private DateTime? _配送日;
        private int? _部署ＣＤ;
        private int? _印刷順From;       // 画面では「集計区分」
        private int? _印刷順To;         // 画面では「集計区分」
        private int? _コースＣＤ;
        private int? _得意先ＣＤ;
        private bool? _ゼロ注文非印字フラグ;
        private bool? _変更分のみ印字フラグ;

        private string _コース名;
        private string _得意先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TanzakuPrintCriteria()
        {
            _注文日指定 = true;
            _配送日指定 = false;
            _注文日 = DateTime.Today;
            _配送日 = null;
            _部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            _ゼロ注文非印字フラグ = true;
            _変更分のみ印字フラグ = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public bool 注文日指定
        {
            get { return _注文日指定; }
            set
            {
                _注文日指定 = value;
                _配送日指定 = !(value);
                _errors.Remove("注文日");
                _errors.Remove("配送日");
                if (value)
                {
                    // ↓ここでやると上手くいかないので画面にて実施
                    //_注文日 = _配送日;
                    //_配送日 = null;
                    if (_注文日 == null)
                    {
                        _errors["注文日"] = "注文日を入力してください。";
                    }
                }
            }
        }

        public bool 配送日指定
        {
            get { return _配送日指定; }
            set
            {
                _配送日指定 = value;
                _注文日指定 = !(value);
                _errors.Remove("注文日");
                _errors.Remove("配送日");
                if (value)
                {
                    // ↓ここでやると上手くいかないので画面にて実施
                    //_配送日 = _注文日;
                    //_注文日 = null;
                    if (_配送日 == null)
                    {
                        _errors["配送日"] = "配送日を入力してください。";
                    }
                }
            }
        }

        public DateTime? 注文日
        {
            get { return _注文日; }
            set
            {
                _注文日 = value;
                _errors.Remove("注文日");
                if (_注文日指定)
                {
                    if (value == null)
                    {
                        _errors["注文日"] = "注文日を入力してください。";
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
                if (_配送日指定)
                {
                    if (value == null)
                    {
                        _errors["配送日"] = "配送日を入力してください。";
                    }
                }
            }
        }

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

        public int? 印刷順From
        {
            get { return _印刷順From; }
            set { _印刷順From = value; }
        }

        public int? 印刷順To
        {
            get { return _印刷順To; }
            set { _印刷順To = value; }
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

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                得意先名 = null;
                _errors.Remove("得意先ＣＤ");
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

        public bool? ゼロ注文非印字フラグ
        {
            get { return _ゼロ注文非印字フラグ; }
            set { _ゼロ注文非印字フラグ = value; }
        }

        public bool? 変更分のみ印字フラグ
        {
            get { return _変更分のみ印字フラグ; }
            set { _変更分のみ印字フラグ = value; }
        }


        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
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
                // 印刷順のFrom～Toをチェック
                if (印刷順From != null && 印刷順To != null)
                {
                    if (印刷順To < 印刷順From)
                    {
                        return "集計区分の指定が逆です。";
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

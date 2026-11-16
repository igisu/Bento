using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上一覧検索条件
    /// </summary>
    public class UriageListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _支店ＣＤ;
        private int? _部署ＣＤ;
        private DateTime? _売上日From;
        private DateTime? _売上日To;
        private int? _商品ＣＤ;
        private int? _商品グループＣＤ;
        private int? _売上区分;
        private int? _食事区分;
        private int? _コースＣＤ;
        private int? _請求先ＣＤ;
        private int? _得意先ＣＤ;
        private int? _受注得意先ＣＤ;

        private string _商品名;
        private string _コース名;
        private string _請求先名;
        private string _得意先名;
        private string _受注得意先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageListCriteria()
        {
            _stopOverwrite = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageListCriteria(UriageListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            売上日From = src.売上日From;
            売上日To = src.売上日To;
            商品ＣＤ = src.商品ＣＤ;
            商品グループＣＤ = src.商品グループＣＤ;
            売上区分 = src.売上区分;
            食事区分 = src.食事区分;
            コースＣＤ = src.コースＣＤ;
            請求先ＣＤ = src.請求先ＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
            受注得意先ＣＤ = src.受注得意先ＣＤ;

            商品名 = src.商品名;
            コース名 = src.コース名;
            請求先名 = src.請求先名;
            得意先名 = src.得意先名;
            受注得意先名 = src.受注得意先名;

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
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
                    if (_stopOverwrite == false)
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
        }

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

        public int? 商品グループＣＤ
        {
            get { return _商品グループＣＤ; }
            set { _商品グループＣＤ = value; }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set { _売上区分 = value; }
        }

        public int? 食事区分
        {
            get { return _食事区分; }
            set { _食事区分 = value; }
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
                    if (_stopOverwrite == false)
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
                    if (_stopOverwrite == false)
                    {
                        // 請求先ＣＤに該当する得意先名を取得
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                        if (tokuisaki != null)
                        {
                            請求先名 = tokuisaki.得意先名;
                        }
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
                if (value != null)
                {
                    if (_stopOverwrite == false)
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
        }

        public int? 受注得意先ＣＤ
        {
            get { return _受注得意先ＣＤ; }
            set
            {
                _受注得意先ＣＤ = value;
                受注得意先名 = null;
                if (value != null)
                {
                    if (_stopOverwrite == false)
                    {
                        // 受注得意先ＣＤに該当する得意先名を取得
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_受注得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            受注得意先名 = tokuisaki.得意先名;
                        }
                    }
                }
            }
        }

        public string 商品名
        {
            get { return _商品名; }
            set { _商品名 = value; }
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

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public string 受注得意先名
        {
            get { return _受注得意先名; }
            set { _受注得意先名 = value; }
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
                if (売上日From != null && 売上日To != null)
                {
                    if (売上日To < 売上日From)
                    {
                        return "売上日の指定が逆です。";
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

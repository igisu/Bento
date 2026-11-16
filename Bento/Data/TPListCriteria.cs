using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// TP決済一覧検索条件
    /// </summary>
    public class TPListCriteria : IDataErrorInfo
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
        private DateTime? _決済日From;
        private DateTime? _決済日To;
        private int? _商品ＣＤ;
        private int? _コースＣＤ;
        private int? _得意先ＣＤ;
        private string _決済対象;
        private string _取引区分;
        private string _決済方法名;

        private string _商品名;
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
        public TPListCriteria()
        {
            _stopOverwrite = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TPListCriteria(TPListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            決済日From = src.決済日From;
            決済日To = src.決済日To;
            商品ＣＤ = src.商品ＣＤ;
            コースＣＤ = src.コースＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
            決済対象 = src.決済対象;
            取引区分 = src.取引区分;
            決済方法名 = src.決済方法名;

            商品名 = src.商品名;
            コース名 = src.コース名;
            得意先名 = src.得意先名;

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

        public DateTime? 決済日From
        {
            get { return _決済日From; }
            set { _決済日From = value; }
        }

        public DateTime? 決済日To
        {
            get { return _決済日To; }
            set { _決済日To = value; }
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

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public string 決済対象
        {
            get { return _決済対象; }
            set { _決済対象 = value; }
        }

        public string 取引区分
        {
            get { return _取引区分; }
            set { _取引区分 = value; }
        }

        public string 決済方法名
        {
            get { return _決済方法名; }
            set { _決済方法名 = value; }
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
                // 決済日のFrom～Toをチェック
                if (決済日From != null && 決済日To != null)
                {
                    if (決済日To < 決済日From)
                    {
                        return "決済日の指定が逆です。";
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

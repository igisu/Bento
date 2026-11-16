using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 注文一覧検索条件
    /// </summary>
    public class ChumonListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _支店ＣＤ;
        private int? _部署ＣＤ;
        private DateTime? _喫食日From;
        private DateTime? _喫食日To;
        private DateTime? _配送日From;
        private DateTime? _配送日To;
        private DateTime? _製造日From;
        private DateTime? _製造日To;
        private int? _商品ＣＤ;
        private int? _商品グループＣＤ;
        private int? _コースＣＤ;
        private int? _請求先ＣＤ;
        private int? _得意先ＣＤ;
        private int? _受注得意先ＣＤ;
        private int? _入力者ＣＤ;
        private bool _注文;
        private bool _予測;
        private DateTime? _作成日From;
        private DateTime? _作成時刻From;
        private DateTime? _作成日To;
        private DateTime? _作成時刻To;
        private DateTime? _修正日From;
        private DateTime? _修正時刻From;
        private DateTime? _修正日To;
        private DateTime? _修正時刻To;

        private string _商品名;
        private string _コース名;
        private string _請求先名;
        private string _得意先名;
        private string _受注得意先名;
        private string _入力者名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChumonListCriteria()
        {
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

        public DateTime? 喫食日From
        {
            get { return _喫食日From; }
            set { _喫食日From = value; }
        }

        public DateTime? 喫食日To
        {
            get { return _喫食日To; }
            set { _喫食日To = value; }
        }

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

        public DateTime? 製造日From
        {
            get { return _製造日From; }
            set { _製造日From = value; }
        }

        public DateTime? 製造日To
        {
            get { return _製造日To; }
            set { _製造日To = value; }
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

        public int? 商品グループＣＤ
        {
            get { return _商品グループＣＤ; }
            set { _商品グループＣＤ = value; }
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
                    // 請求先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                    if (tokuisaki != null)
                    {
                        請求先名 = tokuisaki.得意先名;
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
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                    if (tokuisaki != null)
                    {
                        得意先名 = tokuisaki.得意先名;
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
                    // 受注得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_受注得意先ＣＤ);
                    if (tokuisaki != null)
                    {
                        受注得意先名 = tokuisaki.得意先名;
                    }
                }
            }
        }

        public int? 入力者ＣＤ
        {
            get { return _入力者ＣＤ; }
            set
            {
                _入力者ＣＤ = value;
                入力者名 = null;
                if (value != null)
                {
                    // 入力者ＣＤに該当する担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_入力者ＣＤ);
                    if (tantousya != null)
                    {
                        入力者名 = tantousya.担当者名;
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

        public string 入力者名
        {
            get { return _入力者名; }
            set { _入力者名 = value; }
        }

        public bool 注文
        {
            get { return _注文; }
            set
            {
                _注文 = value;
                _予測 = !(value);
            }
        }

        public bool 予測
        {
            get { return _予測; }
            set
            {
                _予測 = value;
                _注文 = !(value);
            }
        }

        public DateTime? 作成日From
        {
            get { return _作成日From; }
            set { _作成日From = value; }
        }

        public DateTime? 作成時刻From
        {
            get { return _作成時刻From; }
            set { _作成時刻From = value; }
        }

        public DateTime? 作成日To
        {
            get { return _作成日To; }
            set { _作成日To = value; }
        }

        public DateTime? 作成時刻To
        {
            get { return _作成時刻To; }
            set { _作成時刻To = value; }
        }

        public DateTime? 作成日時From
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 作成日From.Value.Year;
                    int month = 作成日From.Value.Month;
                    int day = 作成日From.Value.Day;

                    //時間をパース
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    if (作成時刻From != null)
                    {
                        hour = 作成時刻From.Value.Hour;
                        minute = 作成時刻From.Value.Minute;
                        second = 作成時刻From.Value.Second;
                    }

                    //DateTimeとして返却
                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    return null;
                }
            }
        }

        public DateTime? 作成日時To
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 作成日To.Value.Year;
                    int month = 作成日To.Value.Month;
                    int day = 作成日To.Value.Day;

                    //時間をパース
                    int hour = 23;
                    int minute = 59;
                    int second = 59;
                    if (作成時刻To != null)
                    {
                        hour = 作成時刻To.Value.Hour;
                        minute = 作成時刻To.Value.Minute;
                        second = 作成時刻To.Value.Second;
                    }

                    //DateTimeとして返却
                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    return null;
                }
            }
        }

        public DateTime? 修正日From
        {
            get { return _修正日From; }
            set { _修正日From = value; }
        }

        public DateTime? 修正時刻From
        {
            get { return _修正時刻From; }
            set { _修正時刻From = value; }
        }

        public DateTime? 修正日To
        {
            get { return _修正日To; }
            set { _修正日To = value; }
        }

        public DateTime? 修正時刻To
        {
            get { return _修正時刻To; }
            set { _修正時刻To = value; }
        }

        public DateTime? 修正日時From
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 修正日From.Value.Year;
                    int month = 修正日From.Value.Month;
                    int day = 修正日From.Value.Day;

                    //時間をパース
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    if (修正時刻From != null)
                    {
                        hour = 修正時刻From.Value.Hour;
                        minute = 修正時刻From.Value.Minute;
                        second = 修正時刻From.Value.Second;
                    }

                    //DateTimeとして返却
                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    return null;
                }
            }
        }

        public DateTime? 修正日時To
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 修正日To.Value.Year;
                    int month = 修正日To.Value.Month;
                    int day = 修正日To.Value.Day;

                    //時間をパース
                    int hour = 23;
                    int minute = 59;
                    int second = 59;
                    if (修正時刻To != null)
                    {
                        hour = 修正時刻To.Value.Hour;
                        minute = 修正時刻To.Value.Minute;
                        second = 修正時刻To.Value.Second;
                    }

                    //DateTimeとして返却
                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    return null;
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
                // 喫食日のFrom～Toをチェック
                if (喫食日From != null && 喫食日To != null)
                {
                    if (喫食日To < 喫食日From)
                    {
                        return "注文日の指定が逆です。";
                    }
                }

                // 配送日のFrom～Toをチェック
                if (配送日From != null && 配送日To != null)
                {
                    if (配送日To < 配送日From)
                    {
                        return "配送日の指定が逆です。";
                    }
                }

                // 製造日のFrom～Toをチェック
                if (製造日From != null && 製造日To != null)
                {
                    if (製造日To < 製造日From)
                    {
                        return "製造日の指定が逆です。";
                    }
                }

                // 作成日時のFrom～Toをチェック
                if (作成日時From != null && 作成日時To != null)
                {
                    if (作成日時To < 作成日時From)
                    {
                        return "作成日時の指定が逆です。";
                    }
                }

                // 修正日時のFrom～Toをチェック
                if (修正日時From != null && 修正日時To != null)
                {
                    if (修正日時To < 修正日時From)
                    {
                        return "修正日時の指定が逆です。";
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

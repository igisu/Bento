using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 入金一覧検索条件
    /// </summary>
    public class NyukinListCriteria : IDataErrorInfo
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
        private DateTime? _入金日From;
        private DateTime? _入金日To;
        private int? _コースＣＤ;
        private int? _得意先ＣＤ;
        private int? _入金方法;
        private DateTime? _登録修正日From;
        private DateTime? _登録修正時刻From;
        private DateTime? _登録修正日To;
        private DateTime? _登録修正時刻To;

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
        public NyukinListCriteria()
        {
            支店ＣＤ = null;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            入金日From = DateTime.Today;
            入金日To = DateTime.Today;
            コースＣＤ = null;
            得意先ＣＤ = null;
            入金方法 = null;
            登録修正日From = null;
            登録修正時刻From = null;
            登録修正日To = null;
            登録修正時刻To = null;

            コース名 = null;
            得意先名 = null;
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

        public DateTime? 入金日From
        {
            get { return _入金日From; }
            set { _入金日From = value; }
        }

        public DateTime? 入金日To
        {
            get { return _入金日To; }
            set { _入金日To = value; }
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
            set {
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

        public int? 入金方法
        {
            get { return _入金方法; }
            set { _入金方法 = value; }
        }

        public DateTime? 登録修正日From
        {
            get { return _登録修正日From; }
            set { _登録修正日From = value; }
        }

        public DateTime? 登録修正時刻From
        {
            get { return _登録修正時刻From; }
            set { _登録修正時刻From = value; }
        }

        public DateTime? 登録修正日To
        {
            get { return _登録修正日To; }
            set { _登録修正日To = value; }
        }

        public DateTime? 登録修正時刻To
        {
            get { return _登録修正時刻To; }
            set { _登録修正時刻To = value; }
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

        public DateTime? 登録修正日時From
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 登録修正日From.Value.Year;
                    int month = 登録修正日From.Value.Month;
                    int day = 登録修正日From.Value.Day;

                    //時間をパース
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    if (登録修正時刻From != null)
                    {
                        hour = 登録修正時刻From.Value.Hour;
                        minute = 登録修正時刻From.Value.Minute;
                        second = 登録修正時刻From.Value.Second;
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

        public DateTime? 登録修正日時To
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 登録修正日To.Value.Year;
                    int month = 登録修正日To.Value.Month;
                    int day = 登録修正日To.Value.Day;

                    //時間をパース
                    int hour = 23;
                    int minute = 59;
                    int second = 59;
                    if (登録修正時刻To != null)
                    {
                        hour = 登録修正時刻To.Value.Hour;
                        minute = 登録修正時刻To.Value.Minute;
                        second = 登録修正時刻To.Value.Second;
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
                // 入金日のFrom～Toをチェック
                if (入金日From != null && 入金日To != null)
                {
                    if (入金日To < 入金日From)
                    {
                        return "入金日の指定が逆です。";
                    }
                }

                // 登録修正日時のFrom～Toをチェック
                if (登録修正日時From != null && 登録修正日時To != null)
                {
                    if (登録修正日時To < 登録修正日時From)
                    {
                        return "登録修正日時の指定が逆です。";
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

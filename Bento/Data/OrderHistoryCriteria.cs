using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web注文履歴画面の検索条件
    /// </summary>
    public class OrderHistoryCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _操作日From;
        private DateTime? _操作時刻From;
        private DateTime? _操作日To;
        private DateTime? _操作時刻To;
        private int? _得意先ＣＤ;
        private int? _注文区分;
        private string _IPアドレス;
        private string _UA;

        private string _得意先名;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 操作日From
        {
            get { return _操作日From; }
            set { _操作日From = value; }
        }

        public DateTime? 操作時刻From
        {
            get { return _操作時刻From; }
            set { _操作時刻From = value; }
        }

        public DateTime? 操作日To
        {
            get { return _操作日To; }
            set { _操作日To = value; }
        }

        public DateTime? 操作時刻To
        {
            get { return _操作時刻To; }
            set { _操作時刻To = value; }
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

        public int? 注文区分
        {
            get { return _注文区分; }
            set { _注文区分 = value; }
        }

        public string IPアドレス
        {
            get { return _IPアドレス; }
            set { _IPアドレス = value; }
        }

        public string UA
        {
            get { return _UA; }
            set { _UA = value; }
        }

        public DateTime? 操作日時From
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 操作日From.Value.Year;
                    int month = 操作日From.Value.Month;
                    int day = 操作日From.Value.Day;

                    //時間をパース
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    if (操作時刻From != null)
                    {
                        hour = 操作時刻From.Value.Hour;
                        minute = 操作時刻From.Value.Minute;
                        second = 操作時刻From.Value.Second;
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

        public DateTime? 操作日時To
        {
            get
            {
                try
                {
                    //日付をパース
                    int year = 操作日To.Value.Year;
                    int month = 操作日To.Value.Month;
                    int day = 操作日To.Value.Day;

                    //時間をパース
                    int hour = 23;
                    int minute = 59;
                    int second = 59;
                    if (操作時刻To != null)
                    {
                        hour = 操作時刻To.Value.Hour;
                        minute = 操作時刻To.Value.Minute;
                        second = 操作時刻To.Value.Second;
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
                // 操作日時のFrom～Toをチェック
                if (操作日時From != null && 操作日時To != null)
                {
                    if (操作日時To < 操作日時From)
                    {
                        return "操作日時の指定が逆です。";
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

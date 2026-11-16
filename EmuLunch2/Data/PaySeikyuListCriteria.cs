using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 電子決済請求一覧検索条件
    /// </summary>
    public class PaySeikyuListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _請求日From;
        private DateTime? _請求日To;
        private int? _売上区分;
        private DateTime? _回収予定日From;
        private DateTime? _回収予定日To;
        private DateTime? _処理日;
        private decimal? _今回請求額;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PaySeikyuListCriteria()
        {
            DateTime today = DateTime.Today;
            請求日From = new DateTime(today.Year, today.Month, 1);
            請求日To = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            売上区分 = null;
            回収予定日From = null;
            回収予定日To = null;
            処理日 = null;
            今回請求額 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public PaySeikyuListCriteria(PaySeikyuListCriteria src)
        {
            請求日From = src.請求日From;
            請求日To = src.請求日To;
            売上区分 = src.売上区分;
            回収予定日From = src.回収予定日From;
            回収予定日To = src.回収予定日To;
            処理日 = src.処理日;
            今回請求額 = src.今回請求額;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 請求日From
        {
            get { return _請求日From; }
            set { _請求日From = value; }
        }

        public DateTime? 請求日To
        {
            get { return _請求日To; }
            set { _請求日To = value; }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set { _売上区分 = value; }
        }

        public DateTime? 回収予定日From
        {
            get { return _回収予定日From; }
            set { _回収予定日From = value; }
        }

        public DateTime? 回収予定日To
        {
            get { return _回収予定日To; }
            set { _回収予定日To = value; }
        }

        public DateTime? 処理日
        {
            get { return _処理日; }
            set { _処理日 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
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
                // 請求日のFrom～Toをチェック
                if (_請求日From != null && _請求日To != null)
                {
                    if (_請求日To < _請求日From)
                    {
                        return "請求日の指定が逆です。";
                    }
                }

                // 回収予定日のFrom～Toをチェック
                if (_回収予定日From != null && _回収予定日To != null)
                {
                    if (_回収予定日To < _回収予定日From)
                    {
                        return "回収予定日の指定が逆です。";
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(PaySeikyuListCriteria target)
        {
            if ((売上区分 ?? -1) == (target.売上区分 ?? -1) &&
                請求日From == target.請求日From &&
                請求日To == target.請求日To &&
                回収予定日From == target.回収予定日From &&
                回収予定日To == target.回収予定日To &&
                処理日 == target.処理日 &&
                今回請求額 == target.今回請求額)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

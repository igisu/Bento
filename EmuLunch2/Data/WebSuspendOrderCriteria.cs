using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web注文停止日設定検索条件
    /// </summary>
    public class WebSuspendOrderCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _注文停止日From;
        private DateTime? _注文停止日To;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebSuspendOrderCriteria()
        {
            DateTime today = DateTime.Today;
            注文停止日From = new DateTime(today.Year, today.Month, 1);  // 今月一日から
            注文停止日To = ((DateTime)注文停止日From).AddMonths(2);     // ２ヶ月先まで
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebSuspendOrderCriteria(WebSuspendOrderCriteria src)
        {
            注文停止日From = src.注文停止日From;
            注文停止日To = src.注文停止日To;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 注文停止日From
        {
            get { return _注文停止日From; }
            set
            {
                _注文停止日From = value;
                _errors.Remove("注文停止日From");
                if (_注文停止日From == null)
                {
                    _errors["注文停止日From"] = "注文停止日Fromを入力してください。";
                }
            }
        }

        public DateTime? 注文停止日To
        {
            get { return _注文停止日To; }
            set
            {
                _注文停止日To = value;
                _errors.Remove("注文停止日To");
                if (_注文停止日To == null)
                {
                    _errors["注文停止日To"] = "注文停止日Toを入力してください。";
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
                // 注文停止日のFrom～Toをチェック
                if (_注文停止日From != null && _注文停止日To != null)
                {
                    if (_注文停止日To < _注文停止日From)
                    {
                        return "日付の指定が逆です。";
                    }
                }

                // ２ヶ月を超えてはダメ
                // ※実行するSQLの再帰CTE（共通テーブル式）の再帰数が100を超えたらエラーになる
                if (((DateTime)_注文停止日From).AddMonths(2) < _注文停止日To)
                {
                    return "２ヶ月を超える指定はできません。";
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
        public bool Equals(WebSuspendOrderCriteria target)
        {
            if (注文停止日From == target.注文停止日From &&
                注文停止日To == target.注文停止日To)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 祝日マスタ検索条件
    /// </summary>
    public class ShukujitsuCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _対象日From;
        private DateTime? _対象日To;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShukujitsuCriteria()
        {
            対象日From = null;
            対象日To = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public ShukujitsuCriteria(ShukujitsuCriteria src)
        {
            対象日From = src.対象日From;
            対象日To = src.対象日To;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 対象日From
        {
            get { return _対象日From; }
            set { _対象日From = value; }
        }

        public DateTime? 対象日To
        {
            get { return _対象日To; }
            set { _対象日To = value; }
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
                // 対象日のFrom～Toをチェック
                if (対象日From != null && 対象日To != null)
                {
                    if (対象日To < 対象日From)
                    {
                        return "対象日の指定が逆です。";
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

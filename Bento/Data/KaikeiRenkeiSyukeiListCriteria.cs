using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 会計連携集計一覧検索条件
    /// </summary>
    public class KaikeiRenkeiSyukeiListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _部署ＣＤ;
        private DateTime? _集計日From;
        private DateTime? _集計日To;
        private DateTime? _伝票日付From;
        private DateTime? _伝票日付To;
        private bool? _差分を含める;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KaikeiRenkeiSyukeiListCriteria()
        {
            _集計日From = DateTime.Today;
            _集計日To = DateTime.Today;
            _差分を含める = true;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署ＣＤ
        {
          get { return _部署ＣＤ; }
          set { _部署ＣＤ = value; }
        }

        public DateTime? 集計日From
        {
          get { return _集計日From; }
          set { _集計日From = value; }
        }

        public DateTime? 集計日To
        {
          get { return _集計日To; }
          set { _集計日To = value; }
        }

        public DateTime? 伝票日付From
        {
          get { return _伝票日付From; }
          set { _伝票日付From = value; }
        }

        public DateTime? 伝票日付To
        {
          get { return _伝票日付To; }
          set { _伝票日付To = value; }
        }

        public bool? 差分を含める
        {
            get { return _差分を含める; }
            set { _差分を含める = value; }
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
                // 集計日のFrom～Toをチェック
                if (集計日From != null && 集計日To != null)
                {
                    if (集計日To < 集計日From)
                    {
                        return "集計日の指定が逆です。";
                    }
                }

                // 伝票日付のFrom～Toをチェック
                if (伝票日付From != null && 伝票日付To != null)
                {
                    if (伝票日付To < 伝票日付From)
                    {
                        return "伝票日付の指定が逆です。";
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

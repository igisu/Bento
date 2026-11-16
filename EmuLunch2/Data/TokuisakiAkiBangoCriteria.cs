using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先空き番号検索条件
    /// </summary>
    public class TokuisakiAkiBangoCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _開始番号;
        private int? _件数;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiAkiBangoCriteria()
        {
            開始番号 = null;
            件数 = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 開始番号
        {
            get { return _開始番号; }
            set
            {
                _開始番号 = value;
                _errors.Remove("開始番号");
                if (value == null || value < 0)
                {
                    _errors["開始番号"] = "開始番号を入力してください。";
                }
            }
        }

        public int? 件数
        {
            get { return _件数; }
            set
            {
                _件数 = value;
                _errors.Remove("件数");
                if (value == null || value < 0)
                {
                    _errors["件数"] = "件数を入力してください。";
                }
            }
        }

        public int? 終了番号
        {
            get { return (_開始番号 ?? 0) + (_件数 ?? 0); }
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

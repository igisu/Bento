using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// ANSER-APIファイル取込条件
    /// </summary>
    public class AnserApiImportCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private string _取込ファイル;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnserApiImportCriteria()
        {
            取込ファイル = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public AnserApiImportCriteria(AnserApiImportCriteria src)
        {
            取込ファイル = src.取込ファイル;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public string 取込ファイル
        {
            get { return _取込ファイル; }
            set
            {
                _取込ファイル = value;
                _errors.Remove("取込ファイル");
                if (value == null)
                {
                    _errors["取込ファイル"] = "取込ファイルを入力してください。";
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
            get { return null; }
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
        public bool Equals(AnserApiImportCriteria target)
        {
            if (取込ファイル == target.取込ファイル)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

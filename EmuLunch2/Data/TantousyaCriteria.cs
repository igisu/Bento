using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 担当者マスタ（検索条件用）
    /// </summary>
    public class TantousyaCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _担当者ＣＤ;
        private string _担当者名;
        private int? _所属部署ＣＤ;
        private int? _権限区分;
        private int? _営業業務区分;
        private bool? _無効フラグ;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set { _担当者名 = value; }
        }

        public int? 所属部署ＣＤ
        {
            get { return _所属部署ＣＤ; }
            set { _所属部署ＣＤ = value; }
        }

        public int? 権限区分
        {
            get { return _権限区分; }
            set { _権限区分 = value; }
        }

        public int? 営業業務区分
        {
            get { return _営業業務区分; }
            set { _営業業務区分 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
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

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 仕訳マスタ一覧検索条件
    /// </summary>
    public class ShiwakeCriteria : IDataErrorInfo
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
        private int? _仕訳ＣＤ;
        private string _処理内容;
        private int? _集計ＣＤ;
        private string _借方ＣＤ;
        private string _貸方ＣＤ;
        private string _部門ＣＤ;

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

        public int? 仕訳ＣＤ
        {
            get { return _仕訳ＣＤ; }
            set { _仕訳ＣＤ = value; }
        }

        public string 処理内容
        {
            get { return _処理内容; }
            set { _処理内容 = value; }
        }

        public int? 集計ＣＤ
        {
            get { return _集計ＣＤ; }
            set { _集計ＣＤ = value; }
        }

        public string 借方ＣＤ
        {
            get { return _借方ＣＤ; }
            set { _借方ＣＤ = value; }
        }

        public string 貸方ＣＤ
        {
            get { return _貸方ＣＤ; }
            set { _貸方ＣＤ = value; }
        }

        public string 部門ＣＤ
        {
            get { return _部門ＣＤ; }
            set { _部門ＣＤ = value; }
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

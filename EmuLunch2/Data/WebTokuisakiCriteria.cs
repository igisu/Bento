using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web得意先マスタ（検索条件用）
    /// </summary>
    public class WebTokuisakiCriteria : IDataErrorInfo
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
        private int? _得意先ＣＤ;
        private DateTime? _最終ログイン日From;
        private DateTime? _最終ログイン日To;
        private string _最終ログインIPアドレス;
        private bool? _無効フラグ;
        private string _得意先名;

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

        public DateTime? 最終ログイン日From
        {
            get { return _最終ログイン日From; }
            set { _最終ログイン日From = value; }
        }

        public DateTime? 最終ログイン日To
        {
            get { return _最終ログイン日To; }
            set { _最終ログイン日To = value; }
        }

        public string 最終ログインIPアドレス
        {
            get { return _最終ログインIPアドレス; }
            set { _最終ログインIPアドレス = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
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
                // 最終ログイン日のFrom～Toをチェック
                if (最終ログイン日From != null && 最終ログイン日To != null)
                {
                    if (最終ログイン日To < 最終ログイン日From)
                    {
                        return "最終ログイン日の指定が逆です。";
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 短冊シール発注
    /// </summary>
    public class TanzakuSealOrder :IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _注文数;
        private int _注文単位枚数;      // ロールの枚数のこと

        // 短冊設定マスタの項目。このクラスで入力チェックを行なう
        private string _シール送付先;
        private string _メールアドレス;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TanzakuSealOrder()
        {
            _注文数 = 1;
            _注文単位枚数 = SystemMaster.GetSealMaisu();
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 注文数
        {
            get { return _注文数; }
            set
            {
                _注文数 = value;
                _errors.Remove("注文数");
                if (value == null)
                {
                    _errors["注文数"] = "注文数を入力してください。";
                }
                else
                {
                    if (value < 1)
                    {
                        _errors["注文数"] = "注文数には1以上を入力してください。";
                    }
                }
            }
        }

        public int 注文単位枚数
        {
            get { return _注文単位枚数; }
            set { _注文単位枚数 = value; }
        }

        public string 注文単位枚数編集
        {
            get
            {
                return string.Format("{0:N0}枚", _注文単位枚数);
            }
        }

        public string シール送付先
        {
            get { return _シール送付先; }
            set
            {
                _シール送付先 = value;
                _errors.Remove("シール送付先");
                if (value == null)
                {
                    _errors["シール送付先"] = "送付先を入力してください。";
                }
            }
        }

        public string メールアドレス
        {
            get { return _メールアドレス; }
            set
            {
                _メールアドレス = value;
                _errors.Remove("メールアドレス");
                if (value == null)
                {
                    _errors["メールアドレス"] = "メールアドレスを入力してください。";
                }
                else
                {
                    // http://dobon.net/vb/dotnet/internet/validatemailaddress.html#regex
                    // if (Regex.IsMatch(value,
                    //      @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z",
                    //     RegexOptions.IgnoreCase) == false)
                    // https://qiita.com/sakuro/items/1eaa307609ceaaf51123 をアレンジ
                    if (Regex.IsMatch(value,
                                      @"\A[A-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[A-Z0-9-]+(?:\.[A-Z0-9-]+)+\z",    // メールアドレス
                                      RegexOptions.IgnoreCase) == false)                                    // 大文字小文字を区別しない
                    {
                        _errors["メールアドレス"] = "メールアドレスが正しい形式ではありません。";
                    }
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
            get{ return null;}
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

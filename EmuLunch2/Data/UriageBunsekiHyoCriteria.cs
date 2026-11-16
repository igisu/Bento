using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 集計タイプ別売上分析表印刷条件
    /// </summary>
    public class UriageBunsekiHyoCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private Consts.JissekiSyukeiPrintType _集計タイプ;
        private int? _部署ＣＤ;
        private DateTime? _売上日From;
        private DateTime? _売上日To;
        private int? _分割集計商品ＣＤ;

        private string _分割集計商品名;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageBunsekiHyoCriteria()
        {
            _集計タイプ = Consts.JissekiSyukeiPrintType.Busyo;
            _部署ＣＤ = null;
            _売上日From = DateTime.Today;
            _売上日To = DateTime.Today;
            _分割集計商品ＣＤ = null;
            _分割集計商品名 = null;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public Consts.JissekiSyukeiPrintType 集計タイプ
        {
            get { return _集計タイプ; }
            set { _集計タイプ = value; }
        }

        public int 選択用集計タイプ
        {
            get { return (int)_集計タイプ; }
            set { _集計タイプ = (Consts.JissekiSyukeiPrintType)value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public DateTime? 売上日From
        {
            get { return _売上日From; }
            set
            {
                _売上日From = value;
                _errors.Remove("売上日From");
                if (value == null)
                {
                    _errors["売上日From"] = "売上日Fromを入力してください。";
                }
            }
        }

        public DateTime? 売上日To
        {
            get { return _売上日To; }
            set
            {
                _売上日To = value;
                _errors.Remove("売上日To");
                if (value == null)
                {
                    _errors["売上日To"] = "売上日Toを入力してください。";
                }
            }
        }

        public int? 分割集計商品ＣＤ
        {
            get { return _分割集計商品ＣＤ; }
            set
            {
                _分割集計商品ＣＤ = value;
                _分割集計商品名 = null;
                _errors.Remove("分割集計商品ＣＤ");
                if (value != null)
                {
                    // CDに該当する名称を取得
                    Syohin syohin = Syohin.GetSyohin((int)_分割集計商品ＣＤ);
                    if (syohin != null)
                    {
                        _分割集計商品名 = syohin.商品名;
                    }
                    else
                    {
                        _errors["分割集計商品ＣＤ"] = "商品ＣＤが見つかりません。";
                    }
                }
            }
        }


        public string 分割集計商品名
        {
            get { return _分割集計商品名; }
            set { _分割集計商品名 = value; }
        }

        public string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 売上日From=[{1:yyyy/MM:dd}] 売上日To=[{2:yyyy/MM:dd}] 分割集計商品ＣＤ=[{3}]",  部署ＣＤ, 売上日From, 売上日To, 分割集計商品ＣＤ); }
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
                // 売上日のFrom～Toをチェック
                if (売上日From != null && 売上日To != null)
                {
                    if (売上日To < 売上日From)
                    {
                        return "売上日の指定が逆です。";
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

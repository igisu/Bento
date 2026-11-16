using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web用注文不可日設定検索条件
    /// </summary>
    public class WebDisableOrderCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _得意先ＣＤ;
        private string _得意先名;
        private int? _注文年;
        private int? _注文月;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebDisableOrderCriteria()
        {
            _stopOverwrite = false;

            得意先ＣＤ = null;
            得意先名 = null;
            注文年 = DateTime.Today.Year;
            注文月 = DateTime.Today.Month;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebDisableOrderCriteria(WebDisableOrderCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            得意先ＣＤ = src.得意先ＣＤ;
            得意先名 = src.得意先名;
            注文年 = src.注文年;
            注文月 = src.注文月;

            _stopOverwrite = false;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
                得意先名 = null;
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
                }
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // 得意先ＣＤに該当する得意先名を取得
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            得意先名 = tokuisaki.得意先名;
                        }
                        else
                        {
                            _errors["得意先ＣＤ"] = "得意先が見つかりません。";
                        }
                    }
                }
            }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? 注文年
        {
            get { return _注文年; }
            set
            {
                _注文年 = value;
                _errors.Remove("注文年");
                if (value == null)
                {
                    _errors["注文年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["注文年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 注文月
        {
            get { return _注文月; }
            set
            {
                _注文月 = value;
                _errors.Remove("注文月");
                if (value == null)
                {
                    _errors["注文月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["注文月"] = "1～12を入力してください。";
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(WebDisableOrderCriteria target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                得意先名 == target.得意先名 &&
                注文年 == target.注文年 &&
                注文月 == target.注文月)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

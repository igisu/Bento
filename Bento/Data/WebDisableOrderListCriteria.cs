using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web用注文不可日設定一覧検索条件
    /// </summary>
    public class WebDisableOrderListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _部署ＣＤ;
        private DateTime? _注文不可日From;
        private DateTime? _注文不可日To;
        private int? _得意先ＣＤ;

        private string _得意先名;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebDisableOrderListCriteria()
        {
            _stopOverwrite = false;

            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            DateTime today = DateTime.Today;
            注文不可日From = new DateTime(today.Year, today.Month, 1);
            注文不可日To = null;
            得意先ＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebDisableOrderListCriteria(WebDisableOrderListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = src.部署ＣＤ;
            注文不可日From = src.注文不可日From;
            注文不可日To = src.注文不可日To;
            得意先ＣＤ = src.得意先ＣＤ;

            _stopOverwrite = false;
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

        public DateTime? 注文不可日From
        {
            get { return _注文不可日From; }
            set { _注文不可日From = value; }
        }

        public DateTime? 注文不可日To
        {
            get { return _注文不可日To; }
            set { _注文不可日To = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                得意先名 = null;
                _errors.Remove("得意先ＣＤ");
                if (value != null && _stopOverwrite == false)
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
                // 注文不可日のFrom～Toをチェック
                if (_注文不可日From != null && _注文不可日To != null)
                {
                    if (_注文不可日To < _注文不可日From)
                    {
                        return "日付の指定が逆です。";
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(WebDisableOrderListCriteria target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                注文不可日From == target.注文不可日From &&
                注文不可日To == target.注文不可日To &&
                得意先ＣＤ == target.得意先ＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// クレカ利用状況一覧検索条件
    /// </summary>
    public class FRUseSummaryListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _指定年;
        private int? _指定月;
        private int? _部署ＣＤ;
        private int? _請求先ＣＤ;
        private string _請求先名;
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
        public FRUseSummaryListCriteria()
        {
            // 指定年月は先月
            DateTime today = DateTime.Today;
            DateTime lastMonth = new DateTime(today.Year, today.Month, 1);
            lastMonth = lastMonth.AddMonths(-1);
            指定年 = lastMonth.Year;
            指定月 = lastMonth.Month;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            請求先ＣＤ = null;
            得意先ＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public FRUseSummaryListCriteria(FRUseSummaryListCriteria src)
        {
            指定年 = src.指定年;
            指定月 = src.指定月;
            部署ＣＤ = src.部署ＣＤ;
            請求先ＣＤ = src.請求先ＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public int? 指定年
        {
            get { return _指定年; }
            set
            {
                _指定年 = value;
                _errors.Remove("指定年");
                if (value == null)
                {
                    _errors["指定年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["指定年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 指定月
        {
            get { return _指定月; }
            set
            {
                _指定月 = value;
                _errors.Remove("指定月");
                if (value == null)
                {
                    _errors["指定月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["指定月"] = "1～12を入力してください。";
                    }
                }
            }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                if (value == null || value == -1)
                {
                    _errors["部署ＣＤ"] = "部署を選択してください。";
                }
            }
        }

        public int? 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set
            {
                _請求先ＣＤ = value;
                請求先名 = null;
                if (value != null)
                {
                    // 請求先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                    if (tokuisaki != null)
                    {
                        請求先名 = tokuisaki.得意先名;
                    }
                }
            }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
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

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public DateTime? 指定年月日From
        {
            get
            {
                DateTime? d = null;
                // 指定年・指定月にエラーが無ければ、その一日を求める
                if (_errors.ContainsKey("指定年") == false && _errors.ContainsKey("指定月") == false)
                {
                    d = new DateTime((int)_指定年, (int)_指定月, 1);
                }
                return d;
            }
        }

        public DateTime? 指定年月日To
        {
            get
            {
                DateTime? d = null;
                // 指定年・指定月にエラーが無ければ、その末日を求める
                if (_errors.ContainsKey("指定年") == false && _errors.ContainsKey("指定月") == false)
                {
                    d = new DateTime((int)_指定年, (int)_指定月, DateTime.DaysInMonth((int)_指定年, (int)_指定月));
                }
                return d;
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(FRUseSummaryListCriteria target)
        {
            if (指定年 == target.指定年 &&
                指定月 == target.指定月 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                請求先ＣＤ == target.請求先ＣＤ &&
                得意先ＣＤ == target.得意先ＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 電子決済請求締め条件
    /// </summary>
    public class PaySeikyuShimeCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _売上区分;
        private int? _請求先ＣＤ;
        private int? _月締年;
        private int? _月締月;
        private int? _月締日;
        private bool? _月特締;
        private DateTime? _請求日;

        private string _請求先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PaySeikyuShimeCriteria()
        {
            売上区分 = null;
            請求先ＣＤ = null;
            DateTime today = DateTime.Today;
            月締年 = today.Year;
            月締月 = today.Month;
            月締日 = null;
            月特締 = false;
            請求日 = null;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 売上区分
        {
            get { return _売上区分; }
            set
            {
                _売上区分 = value;
                請求先ＣＤ = null;
                月締日 = null;
                _errors.Remove("売上区分");
                if (value == null || value < 0)
                {
                    _errors["売上区分"] = "売上区分を入力してください。";
                }
                else
                {
                    // 売上区分に該当するレコードを電子決済請求得意先マスタから取得
                    PaySeikyuTokuisaki paySeikyuTokuisaki = PaySeikyuTokuisaki.GetPaySeikyuTokuisaki((int)_売上区分);
                    if (paySeikyuTokuisaki != null)
                    {
                        請求先ＣＤ = paySeikyuTokuisaki.得意先ＣＤ;
                        月締日 = paySeikyuTokuisaki.締日;
                    }
                    else
                    {
                        _errors["売上区分"] = "売上区分を入力してください。";
                    }
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
                _errors.Remove("請求先ＣＤ");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤ);
                    if (tokuisaki != null)
                    {
                        請求先名 = tokuisaki.得意先名;
                        if (tokuisaki.部署ＣＤ == null)
                        {
                            _errors["請求先ＣＤ"] = "この得意先の部署ＣＤが指定されていません。";
                        }
                    }
                    else
                    {
                        _errors["請求先ＣＤ"] = "得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public int? 月締年
        {
            get { return _月締年; }
            set
            {
                _月締年 = value;
                _errors.Remove("月締年");
                if (value == null)
                {
                    _errors["月締年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["月締年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 月締月
        {
            get { return _月締月; }
            set
            {
                _月締月 = value;
                _errors.Remove("月締月");
                if (value == null)
                {
                    _errors["月締月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["月締月"] = "1～12を入力してください。";
                    }
                }
            }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (value == null)
                {
                    _errors["月締日"] = "締日を 1～28 または 月末の場合は99で入力してください。";
                }
                else
                {
                    if (1 <= value && value <= 28)
                    {
                    }
                    else if (value == 29 || value == 30 || value == 31)
                    {
                        _errors["月締日"] = "月末の場合は99を入力してください。";
                    }
                    else if (value == 99)
                    {
                    }
                    else
                    {
                        _errors["月締日"] = "1～28 または 月末の場合は99を入力してください。";
                    }
                }
            }
        }

        public bool? 月特締
        {
            get { return _月特締; }
            set { _月特締 = value; }
        }

        public DateTime? 請求日
        {
            get { return _請求日; }
            set
            {
                _請求日 = value;
                _errors.Remove("請求日");
                if (value == null)
                {
                    _errors["請求日"] = "請求日を入力してください。";
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
            get
            {

                // 年月と締日の妥当性をチェック
                if (_月締年 != null && _errors.ContainsKey("月締年") == false
                    && 月締月 != null && _errors.ContainsKey("月締月") == false
                    && _月締日 != null && _errors.ContainsKey("月締日") == false)
                {
                    if (_月締日 != 99)
                    {
                        if (_月締日 > DateTime.DaysInMonth((int)_月締年, (int)月締月))
                        {
                            return "年月に存在する締日を入力してください。";
                        }
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
        #endregion
    }
}

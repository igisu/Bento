using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求EBデータ作成
    /// </summary>
    public class SeikyuDataEB : IDataErrorInfo
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
        private int? _対象年;
        private int? _対象月;
        private DateTime? _引落日;
        private string _EB先銀行支店ＣＤ;   // 銀行ＣＤ支店ＣＤ
        private int? _請求先ＣＤFrom;
        private int? _請求先ＣＤTo;
        private int? _月締日;
        private bool? _印刷済みフラグ;      // 印刷済みのものも印刷する
        private bool? _全部署出力フラグ;
        private string _請求先名From;
        private string _請求先名To;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuDataEB()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            //対象年月は先月、引落日はEB口座に依存する
            DateTime today = DateTime.Today;
            DateTime lastMaonth = new DateTime(today.Year, today.Month, 1);
            lastMaonth = lastMaonth.AddMonths(-1);
            対象年 = lastMaonth.Year;
            対象月 = lastMaonth.Month;
            月締日 = 99;
            請求先ＣＤFrom = null;
            請求先ＣＤTo = null;
            印刷済みフラグ = false;
            全部署出力フラグ = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public int? 対象年
        {
            get { return _対象年; }
            set
            {
                _対象年 = value;
                _errors.Remove("対象年");
                if (value == null)
                {
                    _errors["対象年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["対象年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 対象月
        {
            get { return _対象月; }
            set
            {
                _対象月 = value;
                _errors.Remove("対象月");
                if (value == null)
                {
                    _errors["対象月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["対象月"] = "1～12を入力してください。";
                    }
                }
            }
        }

        public DateTime? 引落日
        {
            get { return _引落日; }
            set
            {
                _引落日 = value;
                _errors.Remove("引落日");
                if (value == null)
                {
                    _errors["引落日"] = "引落日を入力してください。";
                }
            }
        }

        public string EB先銀行支店ＣＤ
        {
            get { return _EB先銀行支店ＣＤ; }
            set { _EB先銀行支店ＣＤ = value;}
        }

        public int? 請求先ＣＤFrom
        {
            get { return _請求先ＣＤFrom; }
            set
            {
                _請求先ＣＤFrom = value;
                請求先名From = null;
                _errors.Remove("請求先ＣＤFrom");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤFrom);
                    if (tokuisaki != null)
                    {
                        請求先名From = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["請求先ＣＤFrom"] = "得意先ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 請求先ＣＤTo
        {
            get { return _請求先ＣＤTo; }
            set
            {
                _請求先ＣＤTo = value;
                請求先名To = null;
                _errors.Remove("請求先ＣＤTo");
                if (value != null)
                {
                    // 得意先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤTo);
                    if (tokuisaki != null)
                    {
                        請求先名To = tokuisaki.得意先名;
                    }
                    else
                    {
                        _errors["請求先ＣＤTo"] = "得意先ＣＤが見つかりません。";
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

        public bool? 印刷済みフラグ
        {
            get { return _印刷済みフラグ; }
            set { _印刷済みフラグ = value; }
        }

        public bool? 全部署出力フラグ
        {
            get { return _全部署出力フラグ; }
            set { _全部署出力フラグ = value; }
        }


        public string 請求先名From
        {
            get { return _請求先名From; }
            set { _請求先名From = value; }
        }

        public string 請求先名To
        {
            get { return _請求先名To; }
            set { _請求先名To = value; }
        }

        public string 銀行ＣＤ
        {
            get
            {
                string ret = null;
                if (string.IsNullOrEmpty(_EB先銀行支店ＣＤ) == false)
                {
                    // 先頭4桁を銀行ＣＤとして取得
                    if (_EB先銀行支店ＣＤ.Length >= 4)
                    {
                        ret = _EB先銀行支店ＣＤ.Substring(0, 4);
                    }
                }
                return ret;
            }
        }

        public string 支店ＣＤ
        {
            get
            {
                string ret = null;
                if (string.IsNullOrEmpty(_EB先銀行支店ＣＤ) == false)
                {
                    // ５～７文字目を支店ＣＤとして取得
                    if (_EB先銀行支店ＣＤ.Length >= 7)
                    {
                        ret = _EB先銀行支店ＣＤ.Substring(4, 3);
                    }
                }
                return ret;
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
                // 得意先ＣＤのFrom～Toをチェック
                if (_請求先ＣＤFrom != null && _請求先ＣＤTo != null)
                {
                    if (_請求先ＣＤTo < _請求先ＣＤFrom)
                    {
                        return "請求先の指定が逆です。";
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

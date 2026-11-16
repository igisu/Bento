using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// FRクレカ決済一覧検索条件
    /// </summary>
    public class FRSettlementListCriteria : IDataErrorInfo
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

        private int? _支店ＣＤ;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _請求先ＣＤFrom;
        private int? _請求先ＣＤTo;
        private DateTime? _請求日From;
        private DateTime? _請求日To;
        private int? _決済状態;

        private string _コース名;
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
        public FRSettlementListCriteria()
        {
            _stopOverwrite = false;

            支店ＣＤ = null;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            DateTime today = DateTime.Today;
            請求日From = new DateTime(today.Year, today.Month, 1);
            請求日To = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            決済状態 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public FRSettlementListCriteria(FRSettlementListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            請求先ＣＤFrom = src.請求先ＣＤFrom;
            コースＣＤ = src.コースＣＤ;
            請求日From = src.請求日From;
            請求日To = src.請求日To;
            決済状態 = src.決済状態;

            請求先名From = src.請求先名From;
            コース名 = src.コース名;

            _stopOverwrite = false;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 請求先ＣＤFrom
        {
            get { return _請求先ＣＤFrom; }
            set
            {
                _請求先ＣＤFrom = value;
                請求先名From = null;
                _errors.Remove("請求先ＣＤFrom");
                if (value != null && _stopOverwrite == false)
                {
                    // 請求先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤFrom);
                    if (tokuisaki != null)
                    {
                        請求先名From = tokuisaki.得意先名;
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
                if (value != null && _stopOverwrite == false)
                {
                    // 請求先ＣＤに該当する得意先名を取得
                    Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_請求先ＣＤTo);
                    if (tokuisaki != null)
                    {
                        請求先名To = tokuisaki.得意先名;
                    }
                }
            }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                if (value != null && _stopOverwrite == false)
                {
                    // CDに該当する名称を取得
                    Course course = Course.GetCourse((int)_コースＣＤ);
                    if (course != null)
                    {
                        コース名 = course.コース名;
                    }
                }
            }
        }

        public DateTime? 請求日From
        {
            get { return _請求日From; }
            set { _請求日From = value; }
        }

        public DateTime? 請求日To
        {
            get { return _請求日To; }
            set { _請求日To = value; }
        }

        public int? 決済状態
        {
            get { return _決済状態; }
            set { _決済状態 = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
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

        // 以下は、決済処理画面のラベル用プロパティ

        public string 支店名L { get; set; }

        public string 部署名L { get; set; }

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
                // 請求日のFrom～Toをチェック
                if (_請求日From == null || _請求日To == null)
                {
                    return "請求日の指定は必須です。";
                }
                else
                {
                    if (_請求日To < _請求日From)
                    {
                        return "請求日の指定が逆です。";
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
        public bool Equals(FRSettlementListCriteria target)
        {
            if ((支店ＣＤ ?? -1) == (target.支店ＣＤ ?? -1) &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                コースＣＤ == target.コースＣＤ &&
                請求先ＣＤFrom == target.請求先ＣＤFrom &&
                請求日From == target.請求日From &&
                請求日To == target.請求日To &&
                決済状態 == target.決済状態)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

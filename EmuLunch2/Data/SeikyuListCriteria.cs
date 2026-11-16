using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求一覧検索条件
    /// </summary>
    public class SeikyuListCriteria : IDataErrorInfo
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
        private int? _得意先ＣＤ;
        private DateTime? _請求日From;
        private DateTime? _請求日To;
        private DateTime? _回収予定日From;
        private DateTime? _回収予定日To;
        private DateTime? _処理日;
        private decimal? _今回請求額;
        private int? _支払方法;
        private string _EB銀行支店ＣＤ;   // 銀行ＣＤ支店ＣＤ
        private int? _締区分;
        private int? _週締曜日;
        private int? _月締日;

        private string _コース名;
        private string _得意先名;

        // 領収書印刷で使用する条件。検索結果が変わるわけではないのでEquals()では検証しない
        private int _出力件数;
        private string _ソート順;
        private bool _昇順;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuListCriteria()
        {
            _stopOverwrite = false;

            支店ＣＤ = null;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            DateTime today = DateTime.Today;
            請求日From = new DateTime(today.Year, today.Month, 1);
            請求日To = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            回収予定日From = null;
            回収予定日To = null;
            処理日 = null;
            今回請求額 = null;
            支払方法 = null;
            EB銀行支店ＣＤ = null;
            締区分 = null;
            週締曜日 = null;
            月締日 = null;

            出力件数 = 100;
            ソート順 = "";
            昇順 = true;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuListCriteria(SeikyuListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
            コースＣＤ = src.コースＣＤ;
            請求日From = src.請求日From;
            請求日To = src.請求日To;
            回収予定日From = src.回収予定日From;
            回収予定日To = src.回収予定日To;
            処理日 = src.処理日;
            今回請求額 = src.今回請求額;
            支払方法 = src.支払方法;
            EB銀行支店ＣＤ = src.EB銀行支店ＣＤ;
            締区分 = src.締区分;
            週締曜日 = src.週締曜日;
            月締日 = src.月締日;

            得意先名 = src.得意先名;
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

        public DateTime? 回収予定日From
        {
            get { return _回収予定日From; }
            set { _回収予定日From = value; }
        }

        public DateTime? 回収予定日To
        {
            get { return _回収予定日To; }
            set { _回収予定日To = value; }
        }

        public DateTime? 処理日
        {
            get { return _処理日; }
            set { _処理日 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
        }

        public int? 支払方法
        {
            get { return _支払方法; }
            set { _支払方法 = value; }
        }

        public string EB銀行支店ＣＤ
        {
            get { return _EB銀行支店ＣＤ; }
            set { _EB銀行支店ＣＤ = value;}
        }

        public int? 締区分
        {
            get { return _締区分; }
            set { _締区分 = value; }
        }

        public int? 週締曜日
        {
            get { return _週締曜日; }
            set { _週締曜日 = value; }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (value != null)
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

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int 出力件数
        {
            get { return _出力件数; }
            set { _出力件数 = value; }
        }

        public string ソート順
        {
            get { return _ソート順; }
            set { _ソート順 = value; }
        }

        public bool 昇順
        {
            get { return _昇順; }
            set { _昇順 = value; }
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
                // 請求日のFrom～Toをチェック
                if (_請求日From != null && _請求日To != null)
                {
                    if (_請求日To < _請求日From)
                    {
                        return "請求日の指定が逆です。";
                    }
                }

                // 回収予定日のFrom～Toをチェック
                if (_回収予定日From != null && _回収予定日To != null)
                {
                    if (_回収予定日To < _回収予定日From)
                    {
                        return "回収予定日の指定が逆です。";
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
        public bool Equals(SeikyuListCriteria target)
        {
            if ((支店ＣＤ ?? -1) == (target.支店ＣＤ ?? -1) &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                コースＣＤ == target.コースＣＤ &&
                得意先ＣＤ == target.得意先ＣＤ &&
                請求日From == target.請求日From &&
                請求日To == target.請求日To &&
                回収予定日From == target.回収予定日From &&
                回収予定日To == target.回収予定日To &&
                処理日 == target.処理日 &&
                今回請求額 == target.今回請求額 &&
                (支払方法 ?? -1) == (target.支払方法 ?? -1) &&
                EB銀行支店ＣＤ == target.EB銀行支店ＣＤ &&
                (締区分 ?? -1) == (target.締区分 ?? -1) &&
                (週締曜日 ?? -1) == (target.週締曜日 ?? -1) &&
                月締日 == target.月締日)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

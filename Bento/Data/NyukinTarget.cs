using System;
using System.Collections.Generic;
using System.ComponentModel;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 入金入力の入金対象（入金入力検索条件）
    /// </summary>
    public class NyukinTarget : IDataErrorInfo
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

        private int? _得意先ＣＤ;
        private int? _締区分;
        private int? _月締日;
        private int? _週締曜日;

        private string _得意先名;
        private int? _支払サイト;
        private bool? _振込手数料フラグ;

        private int? _表示期間;
        private DateTime? _期間開始日;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NyukinTarget()
        {
            得意先ＣＤ = null;

            締区分 = null;
            月締日 = null;
            週締曜日 = null;

            表示期間 = SystemMaster.Get601ShowMonths();     // システム設定マスタから取得

            得意先名 = null;
            支払サイト = 0;
            振込手数料フラグ = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public NyukinTarget(NyukinTarget src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            得意先ＣＤ = src.得意先ＣＤ;
            締区分 = src.締区分;
            月締日 = src.月締日;
            週締曜日 = src.週締曜日;

            得意先名 = src.得意先名;
            支払サイト = src.支払サイト;
            振込手数料フラグ = src.振込手数料フラグ;

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
            set {
                _得意先ＣＤ = value;
                締区分 = null;
                月締日 = null;
                週締曜日 = null;
                得意先名 = null;
                支払サイト = 0;
                振込手数料フラグ = false;
                _errors.Remove("得意先ＣＤ");
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
                            締区分 = tokuisaki.締区分;
                            月締日 = tokuisaki.月締日;
                            週締曜日 = tokuisaki.週締曜日;
                            得意先名 = tokuisaki.得意先名;
                            支払サイト = tokuisaki.支払サイト;
                            振込手数料フラグ = tokuisaki.振込手数料フラグ;
                        }
                        else
                        {
                            _errors["得意先ＣＤ"] = "得意先ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public int? 締区分
        {
            get { return _締区分; }
            set { _締区分 = value; }
        }

        public string 締区分名
        {
            get
            {
                string s = null;
                if (締区分 == null)
                {
                    return s;
                }
                switch(締区分)
                {
                    case (int)Consts.ShimeKubun.Daily:
                        s = "日締";
                        break;
                    case (int)Consts.ShimeKubun.Weekly:
                        s = "週締";
                        break;
                    case (int)Consts.ShimeKubun.Monthly:
                        s = "月締";
                        break;
                }
                return s;
            }
        }

        public int? 月締日
        {
            get { return _月締日; }
            set
            {
                _月締日 = value;
                _errors.Remove("月締日");
                if (_締区分 == (int)Consts.ShimeKubun.Monthly && value == null)
                {
                    _errors["月締日"] = "月締日が指定されていません。";
                }
            }
        }

        public int? 週締曜日
        {
            get { return _週締曜日; }
            set
            {
                _週締曜日 = value;
                _errors.Remove("週締曜日名");
                if (_締区分 == (int)Consts.ShimeKubun.Weekly && value == null)
                {
                    _errors["週締曜日名"] = "週締曜日が指定されていません。";   // "週締曜日"ではなく"週締曜日名"であることに注意！
                }
            }
        }

        public string 週締曜日名
        {
            get
            {
                string s = null;
                if (週締曜日 == null)
                {
                    return s;
                }
                switch ((DayOfWeek)_週締曜日)
                {
                    case DayOfWeek.Sunday:
                        s = "日曜日";
                        break;
                    case DayOfWeek.Monday:
                        s = "月曜日";
                        break;
                    case DayOfWeek.Tuesday:
                        s = "火曜日";
                        break;
                    case DayOfWeek.Wednesday:
                        s = "水曜日";
                        break;
                    case DayOfWeek.Thursday:
                        s = "木曜日";
                        break;
                    case DayOfWeek.Friday:
                        s = "金曜日";
                        break;
                    case DayOfWeek.Saturday:
                        s = "土曜日";
                        break;
                }
                return s;
            }
        }

        public int? 表示期間
        {
            get { return _表示期間; }
            set
            {
                _表示期間 = value;
                _errors.Remove("表示期間");
                if (value == null)
                {
                    _errors["表示期間"] = "表示期間を入力してください。";
                }
                else
                {
                    if (value < 0)
                    {
                        _errors["表示期間"] = "表示期間にマイナスは入力できません。";
                    }
                }
            }
        }

        public DateTime? 期間開始日
        {
            get { return _期間開始日; }
            set { _期間開始日 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? 支払サイト
        {
            get { return _支払サイト; }
            set { _支払サイト = value; }
        }

        public bool? 振込手数料フラグ
        {
            get { return _振込手数料フラグ; }
            set { _振込手数料フラグ = value; }
        }

        public string 期間
        {
            get
            {
                if (_期間開始日 == null)
                {
                    return null;
                }
                // 期間開始日は条件に含まれていないので翌日にする
                return string.Format("{0:yyyy/MM/dd} ～", ((DateTime)_期間開始日).AddDays(1));
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
        public bool Equals(NyukinTarget target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}

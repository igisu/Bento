using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 月次売掛残高一覧検索条件
    /// </summary>
    public class GetsujiUrikakeZanListCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _集計年;
        private int? _集計月;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _得意先ＣＤ;

        private string _コース名;
        private string _得意先名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GetsujiUrikakeZanListCriteria()
        {
            //集計年月は先月
            DateTime today = DateTime.Today;
            DateTime lastMonth = new DateTime(today.Year, today.Month, 1);
            lastMonth = lastMonth.AddMonths(-1);
            集計年 = lastMonth.Year;
            集計月 = lastMonth.Month;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            コースＣＤ = null;
            得意先ＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public GetsujiUrikakeZanListCriteria(GetsujiUrikakeZanListCriteria src)
        {
            集計年 = src.集計年;
            集計月 = src.集計月;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 集計年
        {
            get { return _集計年; }
            set
            {
                _集計年 = value;
                _errors.Remove("集計年");
                if (value == null)
                {
                    _errors["集計年"] = "西暦を入力してください。";
                }
                else
                {
                    if (value <= 2000)
                    {
                        _errors["集計年"] = "西暦を入力してください。";
                    }
                }
            }
        }

        public int? 集計月
        {
            get { return _集計月; }
            set
            {
                _集計月 = value;
                _errors.Remove("集計月");
                if (value == null)
                {
                    _errors["集計月"] = "月を 1～12 で入力してください。";
                }
                else
                {
                    if (value < 1 || 12 < value)
                    {
                        _errors["集計月"] = "1～12を入力してください。";
                    }
                }
            }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                if (value != null)
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
        public bool Equals(GetsujiUrikakeZanListCriteria target)
        {
            if (集計年 == target.集計年 &&
                集計月 == target.集計月 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                コースＣＤ == target.コースＣＤ &&
                得意先ＣＤ == target.得意先ＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

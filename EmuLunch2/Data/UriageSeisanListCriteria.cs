using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上精算一覧検索条件
    /// </summary>
    public class UriageSeisanListCriteria : IDataErrorInfo
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
        private DateTime? _精算日From;
        private DateTime? _精算日To;
        private int? _コースＣＤ;
        private int? _担当者ＣＤ;

        private string _コース名;
        private string _担当者名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageSeisanListCriteria()
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            支店ＣＤ = null;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            精算日From = DateTime.Today;
            精算日To = DateTime.Today;
            コースＣＤ = null;
            担当者ＣＤ = null;

            _stopOverwrite = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanListCriteria(UriageSeisanListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            精算日From = src.精算日From;
            精算日To = src.精算日To;
            コースＣＤ = src.コースＣＤ;
            担当者ＣＤ = src.担当者ＣＤ;

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

        public DateTime? 精算日From
        {
            get { return _精算日From; }
            set { _精算日From = value; }
        }

        public DateTime? 精算日To
        {
            get { return _精算日To; }
            set { _精算日To = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                _errors.Remove("コースＣＤ");
                if (value != null)
                {
                    if (_stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_コースＣＤ);
                        if (course != null)
                        {
                            コース名 = course.コース名;
                        }
                        else
                        {
                            _errors["コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set
            {
                _担当者ＣＤ = value;
                _errors.Remove("担当者ＣＤ");
                担当者名 = null;
                if (value != null)
                {
                    // 担当者ＣＤに該当する担当者を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_担当者ＣＤ);
                    if (tantousya != null)
                    {
                        担当者名 = tantousya.担当者名;
                    }
                }
            }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set { _担当者名 = value; }
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
                // 精算日のFrom～Toをチェック
                if (精算日From != null && 精算日To != null)
                {
                    if (精算日To < 精算日From)
                    {
                        return "精算日の指定が逆です。";
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
        public bool Equals(UriageSeisanListCriteria target)
        {
            if ((支店ＣＤ ?? -1) == (target.支店ＣＤ ?? -1) &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                精算日From == target.精算日From &&
                精算日To == target.精算日To &&
                コースＣＤ == target.コースＣＤ &&
                担当者ＣＤ == target.担当者ＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

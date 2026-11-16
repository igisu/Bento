using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 過不足一覧検索条件
    /// </summary>
    public class KafusokuListCriteria : IDataErrorInfo
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
        private DateTime? _配送日;
        private int? _コース区分;
        private int? _グループ種別ＣＤ;
        private int? _コースＣＤFrom;
        private int? _コースＣＤTo;

        private string _コース名From;
        private string _コース名To;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KafusokuListCriteria()
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            コース区分 = null;
            配送日 = DateTime.Today;
            グループ種別ＣＤ = null;
            コースＣＤFrom = null;
            コースＣＤTo = null;

            _stopOverwrite = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public KafusokuListCriteria(KafusokuListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = src.部署ＣＤ;
            配送日 = src.配送日;
            コース区分 = src.コース区分;
            グループ種別ＣＤ = src.グループ種別ＣＤ;
            コースＣＤFrom = src.コースＣＤFrom;
            コースＣＤTo = src.コースＣＤTo;

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

        public DateTime? 配送日
        {
            get { return _配送日; }
            set
            {
                _配送日 = value;
                _errors.Remove("配送日");
                if (value == null)
                {
                    _errors["配送日"] = "配送日を入力してください。";
                }
            }
        }

        public int? コース区分
        {
            get { return _コース区分; }
            set
            {
                _コース区分 = value;
                _errors.Remove("コース区分");
                if (value == null || value < 0)
                {
                    _errors["コース区分"] = "コース区分を入力してください。";
                }
            }
        }

        public int? グループ種別ＣＤ
        {
            get { return _グループ種別ＣＤ; }
            set { _グループ種別ＣＤ = value; }
        }

        public int? コースＣＤFrom
        {
            get { return _コースＣＤFrom; }
            set
            {
                _コースＣＤFrom = value;
                コース名From = null;
                _errors.Remove("コースＣＤFrom");
                if (value != null)
                {
                    if (_stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_コースＣＤFrom);
                        if (course != null)
                        {
                            コース名From = course.コース名;
                        }
                        else
                        {
                            _errors["コースＣＤFrom"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public int? コースＣＤTo
        {
            get { return _コースＣＤTo; }
            set
            {
                _コースＣＤTo = value;
                コース名To = null;
                _errors.Remove("コースＣＤTo");
                if (value != null)
                {
                    if (_stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_コースＣＤTo);
                        if (course != null)
                        {
                            コース名To = course.コース名;
                        }
                        else
                        {
                            _errors["コースＣＤTo"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public string コース名From
        {
            get { return _コース名From; }
            set { _コース名From = value; }
        }

        public string コース名To
        {
            get { return _コース名To; }
            set { _コース名To = value; }
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
                // コースＣＤFrom～Toのチェック
                if (コースＣＤFrom != null && コースＣＤTo != null)
                {
                    if (コースＣＤTo < コースＣＤFrom)
                    {
                        return "コースＣＤの指定が逆です。";
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
        public bool Equals(KafusokuListCriteria target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                配送日 == target.配送日 &&
                (コース区分 ?? -1) == (target.コース区分 ?? -1) &&
                (グループ種別ＣＤ ?? -1) == (target.グループ種別ＣＤ ?? -1) &&
                コースＣＤFrom == target.コースＣＤFrom &&
                コースＣＤTo == target.コースＣＤTo)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

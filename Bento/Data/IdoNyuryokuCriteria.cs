using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 移動入力検索条件
    /// </summary>
    public class IdoNyuryokuCriteria : IDataErrorInfo
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
        private int? _コース区分;
        private DateTime? _配送日;
        private int? _グループ種別ＣＤ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IdoNyuryokuCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            配送日 = DateTime.Today;
            //コース区分 = null;                // 配送日に該当するコース区分を初期表示する
            グループ種別ＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public IdoNyuryokuCriteria(IdoNyuryokuCriteria src)
        {
            部署ＣＤ = src.部署ＣＤ;
            配送日 = src.配送日;
            コース区分 = src.コース区分;
            グループ種別ＣＤ = src.グループ種別ＣＤ;
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
                else
                {
                    コース区分 = Course.GetCourseKubun((DateTime)_配送日);
                }
            }
        }

        public int? グループ種別ＣＤ
        {
            get { return _グループ種別ＣＤ; }
            set { _グループ種別ＣＤ = value; }
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
        public bool Equals(IdoNyuryokuCriteria target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                (コース区分 ?? -1) == (target.コース区分 ?? -1) &&
                配送日 == target.配送日 &&
                (グループ種別ＣＤ ?? -1) == (target.グループ種別ＣＤ ?? -1))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 日配持出入力検索条件
    /// </summary>
    /// <remarks>
    /// 【前提】コースが決まれば、部署ＣＤは自動的に決まる（コースが部署ＣＤを持っているため）
    /// 【例外】注文入力時は、コースとは無関係の部署ＣＤを指定できる（日配の弁当はコースと部署は関連するが、ケータリングなどはこの限りではないため）
    /// 【方針】日配持出入力においては、指定された部署ＣＤを持つコースを一覧表示する。
    ///  注文ヘッダ／明細についても、指定された部署ＣＤに該当するものを表示する。
    /// 日配の弁当（＝ルートに沿った配達）についてのモバイル_持ち出し入力テーブルへの登録処理なので、コースと部署ＣＤは関連付く。
    ///  ※配送予定入力、注文一覧との違いはそれぞれの検索条件クラスを参照。
    /// </remarks>
    public class NippaiMochidashiCriteria : IDataErrorInfo
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
        private DateTime? _持出日;
        private int? _コース区分;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NippaiMochidashiCriteria()
        {
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            持出日 = DateTime.Today;
            //コース区分 = null;            // 持出日に該当するコース区分を初期表示する
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public NippaiMochidashiCriteria(NippaiMochidashiCriteria src)
        {
            部署ＣＤ = src.部署ＣＤ;
            持出日 = src.持出日;
            コース区分 = src.コース区分;
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

        public DateTime? 持出日
        {
            get { return _持出日; }
            set
            {
                _持出日 = value;
                _コース区分 = -1;
                _errors.Remove("持出日");
                if (value == null)
                {
                    _errors["持出日"] = "持出日を入力してください。";
                }
                else
                {
                    コース区分 = Course.GetCourseKubun((DateTime)_持出日);
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
        public bool Equals(NippaiMochidashiCriteria target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                持出日 == target.持出日 &&
                (コース区分 ?? -1) == (target.コース区分 ?? -1))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

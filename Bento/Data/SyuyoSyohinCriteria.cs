using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 主要商品リストマスタ（検索条件用）
    /// </summary>
    public class SyuyoSyohinCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _支店ＣＤ;
        private int? _部署ＣＤ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyuyoSyohinCriteria()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SyuyoSyohinCriteria(SyuyoSyohinCriteria src)
        {
            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SyuyoSyohinCriteria(DataRow dr)
        {
            支店ＣＤ = dr["支店ＣＤ"] as int?;
            部署ＣＤ = dr["部署ＣＤ"] as int?;
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
        public bool Equals(SyuyoSyohinCriteria target)
        {
            if ((支店ＣＤ ?? -1) == (target.支店ＣＤ ?? -1) &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 大袋ラベル表示条件
    /// </summary>
    public class BigBagLabelCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _注文日;
        private int? _部署ＣＤ;

        private string _部署名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BigBagLabelCriteria()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public BigBagLabelCriteria(BigBagLabelCriteria src)
        {
            注文日 = src.注文日;
            部署ＣＤ = src.部署ＣＤ;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 注文日
        {
            get { return _注文日; }
            set
            {
                _注文日 = value;
                _errors.Remove("注文日");
                if (value == null)
                {
                    _errors["注文日"] = "注文日を入力してください。";
                }
            }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                部署名 = null;
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
                else
                {
                    // 部署ＣＤに該当する部署名を取得
                    Busyo busyo = Busyo.GetBusyo((int)_部署ＣＤ);
                    if (busyo != null)
                    {
                        部署名 = busyo.部署名;
                    }
                }
            }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get{ return null;}
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
        public bool Equals(BigBagLabelCriteria target)
        {
            // if (注文日 == target.注文日 &&
            //     (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1))

            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1))    // 注文日は無視して比較する
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

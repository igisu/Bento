using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 出荷一覧検索条件
    /// </summary>
    public class ShippingHistoryCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private DateTime? _配送日;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private string _コース名;
        private int? _担当者ＣＤ;
        private string _担当者名;
        private DateTime? _出荷完了日時;
        private int? _出荷数;
        private int? _個数;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShippingHistoryCriteria()
        {
            配送日 = null;
            部署ＣＤ = null;
            コースＣＤ = null;
            コース名 = null;
            担当者ＣＤ = null;
            担当者名 = null;
            出荷完了日時 = null;
            出荷数 = null;
            個数 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public ShippingHistoryCriteria(ShippingHistoryCriteria src)
        {
            配送日 = src.配送日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            コース名 = src.コース名;
            担当者ＣＤ = src.担当者ＣＤ;
            担当者名 = src.担当者名;
            出荷完了日時 = src.出荷完了日時;
            出荷数 = src.出荷数;
            個数 = src.個数;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 配送日
        {
            get { return _配送日; }
            set { _配送日 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set { _担当者名 = value; }
        }

        public DateTime? 出荷完了日時
        {
            get { return _出荷完了日時; }
            set { _出荷完了日時 = value; }
        }

        public int? 出荷数
        {
            get { return _出荷数; }
            set { _出荷数 = value; }
        }

        public int? 個数
        {
            get { return _個数; }
            set { _個数 = value; }
        }

        public string 状態
        {
            get
            {
                return string.Format("{0}/{1}", _個数, _出荷数);
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

    }
}

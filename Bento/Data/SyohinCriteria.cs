using System.Collections.Generic;
using System.ComponentModel;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 商品マスタ（検索条件用）
    /// </summary>
    public class SyohinCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private int? _部署グループ;
        private int? _商品ＣＤ;
        private string _商品名;
        private decimal? _商品単価From;
        private decimal? _商品単価To;
        private int? _弁当区分;
        private int? _副食ＣＤ;
        private int? _主食ＣＤ;
        private int? _食事区分;
        private bool? _無効フラグ;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署グループ
        {
            get { return _部署グループ; }
            set { _部署グループ = value; }
        }

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set { _商品ＣＤ = value; }
        }

        public string 商品名
        {
            get { return _商品名; }
            set { _商品名 = value; }
        }

        public decimal? 商品単価From
        {
            get { return _商品単価From; }
            set { _商品単価From = value; }
        }

        public decimal? 商品単価To
        {
            get { return _商品単価To; }
            set { _商品単価To = value; }
        }

        public int? 弁当区分
        {
            get { return _弁当区分; }
            set { _弁当区分 = value; }
        }

        public int? 副食ＣＤ
        {
            get { return _副食ＣＤ; }
            set { _副食ＣＤ = value; }
        }

        public int? 主食ＣＤ
        {
            get { return _主食ＣＤ; }
            set { _主食ＣＤ = value; }
        }

        public int? 食事区分
        {
            get { return _食事区分; }
            set { _食事区分 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
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
                // 商品単価From～Toのチェック
                if (商品単価From != null && 商品単価To != null)
                {
                    if (商品単価To < 商品単価From)
                    {
                        return "商品単価の指定が逆です。";
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
    }
}

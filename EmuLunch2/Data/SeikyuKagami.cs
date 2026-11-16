using Emu.Bento.Common;
using System;
using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求鑑
    /// </summary>
    public class SeikyuKagami : SeikyuGokei
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _前々回請求日;
        private decimal? _前々回前回請求額;
        private decimal? _前々回入金額;
        private decimal? _前々回繰越額;
        private decimal? _前々回買上金額;
        private decimal? _前々回調整額;
        private decimal? _前々回消費税;
        private decimal? _前々回今回請求額;

        private DateTime? _前回請求日;
        private decimal? _前回前回請求額;
        private decimal? _前回入金額;
        private decimal? _前回繰越額;
        private decimal? _前回買上金額;
        private decimal? _前回調整額;
        private decimal? _前回消費税;
        private decimal? _前回今回請求額;

        private string _クレカ登録状況;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuKagami()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SeikyuKagami(SeikyuKagami src) : base(src)
        {
            前々回請求日 = src.前々回請求日;
            前々回前回請求額 = src.前々回前回請求額;
            前々回入金額 = src.前々回入金額;
            前々回繰越額 = src.前々回繰越額;
            前々回買上金額 = src.前々回買上金額;
            前々回調整額 = src.前々回調整額;
            前々回消費税 = src.前々回消費税;
            前々回今回請求額 = src.前々回今回請求額;

            前回請求日 = src.前回請求日;
            前回前回請求額 = src.前回前回請求額;
            前回入金額 = src.前回入金額;
            前回繰越額 = src.前回繰越額;
            前回買上金額 = src.前回買上金額;
            前回調整額 = src.前回調整額;
            前回消費税 = src.前回消費税;
            前回今回請求額 = src.前回今回請求額;

            クレカ登録状況 = src.クレカ登録状況;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SeikyuKagami(DataRow dr) : base(dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            前々回請求日 = dr["前々回請求日", version] as DateTime?;
            前々回前回請求額 = dr["前々回前回請求額", version] as decimal?;
            前々回入金額 = dr["前々回入金額", version] as decimal?;
            前々回繰越額 = dr["前々回繰越額", version] as decimal?;
            前々回買上金額 = dr["前々回買上金額", version] as decimal?;
            前々回調整額 = dr["前々回調整額", version] as decimal?;
            前々回消費税 = dr["前々回消費税", version] as decimal?;
            前々回今回請求額 = dr["前々回今回請求額", version] as decimal?;

            前回請求日 = dr["前回請求日", version] as DateTime?;
            前回前回請求額 = dr["前回前回請求額", version] as decimal?;
            前回入金額 = dr["前回入金額", version] as decimal?;
            前回繰越額 = dr["前回繰越額", version] as decimal?;
            前回買上金額 = dr["前回買上金額", version] as decimal?;
            前回調整額 = dr["前回調整額", version] as decimal?;
            前回消費税 = dr["前回消費税", version] as decimal?;
            前回今回請求額 = dr["前回今回請求額", version] as decimal?;

            クレカ登録状況 = dr["クレカ登録状況", version] as string;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 前々回請求日
        {
            get { return _前々回請求日; }
            set { _前々回請求日 = value; }
        }

        public decimal? 前々回前回請求額
        {
            get { return _前々回前回請求額; }
            set { _前々回前回請求額 = value; }
        }

        public decimal? 前々回入金額
        {
            get { return _前々回入金額; }
            set { _前々回入金額 = value; }
        }

        public decimal? 前々回繰越額
        {
            get { return _前々回繰越額; }
            set { _前々回繰越額 = value; }
        }

        public decimal? 前々回買上金額
        {
            get { return _前々回買上金額; }
            set { _前々回買上金額 = value; }
        }

        public decimal? 前々回調整額
        {
            get { return _前々回調整額; }
            set { _前々回調整額 = value; }
        }

        public decimal? 前々回消費税
        {
            get { return _前々回消費税; }
            set { _前々回消費税 = value; }
        }

        public decimal? 前々回今回請求額
        {
            get { return _前々回今回請求額; }
            set { _前々回今回請求額 = value; }
        }

        public DateTime? 前回請求日
        {
            get { return _前回請求日; }
            set { _前回請求日 = value; }
        }

        public decimal? 前回前回請求額
        {
            get { return _前回前回請求額; }
            set { _前回前回請求額 = value; }
        }

        public decimal? 前回入金額
        {
            get { return _前回入金額; }
            set { _前回入金額 = value; }
        }

        public decimal? 前回繰越額
        {
            get { return _前回繰越額; }
            set { _前回繰越額 = value; }
        }

        public decimal? 前回買上金額
        {
            get { return _前回買上金額; }
            set { _前回買上金額 = value; }
        }

        public decimal? 前回調整額
        {
            get { return _前回調整額; }
            set { _前回調整額 = value; }
        }

        public decimal? 前回消費税
        {
            get { return _前回消費税; }
            set { _前回消費税 = value; }
        }

        public decimal? 前回今回請求額
        {
            get { return _前回今回請求額; }
            set { _前回今回請求額 = value; }
        }

        public string クレカ登録状況
        {
            get { return _クレカ登録状況; }
            set { _クレカ登録状況 = value; }
        }

        public bool 編集可能フラグ
        {
            get
            {
                if (FR支払フラグ == false)
                {
                    return true;
                }

                if (クレカ決済完了フラグ == false && (クレカ決済エラー区分 == null || クレカ決済エラー区分 == (int)Consts.FRErrorType.None))
                {
                    return true;
                }

                return false;
            }
        }

        public new bool HasErrors
        {
            get
            {
                if (base.HasErrors)
                {
                    return true;
                }
                return (_errors.Count != 0 || Error != null);
            }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public new string Error
        {
            get
            {
                if (base.Error != null)
                {
                    return base.Error;
                }
                return null;
            }
        }

        public new string this[string columnName]
        {
            get
            {
                if (base[columnName] != null)
                {
                    return base[columnName];
                }
                return (_errors.ContainsKey(columnName) ? _errors[columnName] : null);
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        #endregion
    }
}

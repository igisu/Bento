using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 月次売掛残高編集
    /// </summary>
    public class GetsujiUrikakeZanEdit : GetsujiUrikakeZan
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _前々回集計年;
        private int? _前々回集計月;
        private decimal? _前々回先月売掛残高;
        private decimal? _前々回今月入金額;
        private decimal? _前々回今月売上額;
        private decimal? _前々回今月売掛残高;

        private int? _前回集計年;
        private int? _前回集計月;
        private decimal? _前回先月売掛残高;
        private decimal? _前回今月入金額;
        private decimal? _前回今月売上額;
        private decimal? _前回今月売掛残高;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GetsujiUrikakeZanEdit()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public GetsujiUrikakeZanEdit(GetsujiUrikakeZanEdit src) : base(src)
        {
            前々回集計年 = src.前々回集計年;
            前々回集計月 = src.前々回集計月;
            前々回先月売掛残高 = src.前々回先月売掛残高;
            前々回今月入金額 = src.前々回今月入金額;
            前々回今月売上額 = src.前々回今月売上額;
            前々回今月売掛残高 = src.前々回今月売掛残高;

            前回集計年 = src.前回集計年;
            前回集計月 = src.前回集計月;
            前回先月売掛残高 = src.前回先月売掛残高;
            前回今月入金額 = src.前回今月入金額;
            前回今月売上額 = src.前回今月売上額;
            前回今月売掛残高 = src.前回今月売掛残高;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public GetsujiUrikakeZanEdit(DataRow dr) : base(dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            前々回集計年 = dr["前々回集計年", version] as int?;
            前々回集計月 = dr["前々回集計月", version] as int?;
            前々回先月売掛残高 = dr["前々回先月売掛残高", version] as decimal?;
            前々回今月入金額 = dr["前々回今月入金額", version] as decimal?;
            前々回今月売上額 = dr["前々回今月売上額", version] as decimal?;
            前々回今月売掛残高 = dr["前々回今月売掛残高", version] as decimal?;

            前回集計年 = dr["前回集計年", version] as int?;
            前回集計月 = dr["前回集計月", version] as int?;
            前回先月売掛残高 = dr["前回先月売掛残高", version] as decimal?;
            前回今月入金額 = dr["前回今月入金額", version] as decimal?;
            前回今月売上額 = dr["前回今月売上額", version] as decimal?;
            前回今月売掛残高 = dr["前回今月売掛残高", version] as decimal?;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 前々回集計年
        {
            get { return _前々回集計年; }
            set { _前々回集計年 = value; }
        }

        public int? 前々回集計月
        {
            get { return _前々回集計月; }
            set { _前々回集計月 = value; }
        }

        public decimal? 前々回先月売掛残高
        {
            get { return _前々回先月売掛残高; }
            set { _前々回先月売掛残高 = value; }
        }

        public decimal? 前々回今月入金額
        {
            get { return _前々回今月入金額; }
            set { _前々回今月入金額 = value; }
        }

        public decimal? 前々回今月売上額
        {
            get { return _前々回今月売上額; }
            set { _前々回今月売上額 = value; }
        }

        public decimal? 前々回今月売掛残高
        {
            get { return _前々回今月売掛残高; }
            set { _前々回今月売掛残高 = value; }
        }


        public int? 前回集計年
        {
            get { return _前回集計年; }
            set { _前回集計年 = value; }
        }

        public int? 前回集計月
        {
            get { return _前回集計月; }
            set { _前回集計月 = value; }
        }

        public decimal? 前回先月売掛残高
        {
            get { return _前回先月売掛残高; }
            set { _前回先月売掛残高 = value; }
        }

        public decimal? 前回今月入金額
        {
            get { return _前回今月入金額; }
            set { _前回今月入金額 = value; }
        }

        public decimal? 前回今月売上額
        {
            get { return _前回今月売上額; }
            set { _前回今月売上額 = value; }
        }

        public decimal? 前回今月売掛残高
        {
            get { return _前回今月売掛残高; }
            set { _前回今月売掛残高 = value; }
        }

        public new bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public new string Error
        {
            get{ return null;}
        }

        public new string this[string columnName]
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
        #endregion
    }
}

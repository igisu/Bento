using System;
using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 請求書印刷対象
    /// </summary>
    public class SeikyuPrintTarget
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal _請求番号;
        private DateTime _請求日;
        private int _請求先ＣＤ;
        private string _請求先名;
        private decimal? _今回請求額;
        private int _請求書タイプ区分;
        private int? _コースＣＤ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SeikyuPrintTarget()
        {
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SeikyuPrintTarget(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            請求番号 = (decimal)dr["請求番号", version];
            請求日 = (DateTime)dr["請求日", version];
            請求先ＣＤ = (int)dr["請求先ＣＤ", version];
            請求先名 = dr["請求先名", version] as string;
            今回請求額 = dr["今回請求額", version] as decimal?;
            請求書タイプ区分 = (int)dr["請求書タイプ区分", version];
            コースＣＤ = dr["コースＣＤ", version] as int?;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal 請求番号
        {
            get { return _請求番号; }
            set { _請求番号 = value; }
        }

        public DateTime 請求日
        {
            get { return _請求日; }
            set { _請求日 = value; }
        }

        public int 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set { _請求先ＣＤ = value; }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
        }

        public int 請求書タイプ区分
        {
            get { return _請求書タイプ区分; }
            set { _請求書タイプ区分 = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }
        #endregion
    }
}

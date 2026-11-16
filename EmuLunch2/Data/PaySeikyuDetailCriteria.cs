using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 電子決済請求明細表示条件
    /// </summary>
    public class PaySeikyuDetailCriteria
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private decimal? _請求番号;
        private int? _売上区分;
        private string _売上区分名;
        private DateTime? _請求日;
        private DateTime? _前回請求日;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public PaySeikyuDetailCriteria(PaySeikyuDetailCriteria src)
        {
            請求番号 = src.請求番号;
            売上区分 = src.売上区分;
            売上区分名 = src.売上区分名;
            請求日 = src.請求日;
            前回請求日 = src.前回請求日;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public PaySeikyuDetailCriteria(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            請求番号 = dr["請求番号", version] as decimal?;
            売上区分 = dr["売上区分", version] as int?;
            売上区分名 = dr["売上区分名", version] as string;
            請求日 = dr["請求日", version] as DateTime?;
            前回請求日 = dr["前回請求日", version] as DateTime?;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 請求番号
        {
            get { return _請求番号; }
            set { _請求番号 = value; }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set { _売上区分 = value; }
        }

        public string 売上区分名
        {
            get { return _売上区分名; }
            set { _売上区分名 = value; }
        }

        public DateTime? 請求日
        {
            get { return _請求日; }
            set { _請求日 = value; }
        }

        public DateTime? 前回請求日
        {
            get { return _前回請求日; }
            set { _前回請求日 = value; }
        }

        public DateTime? 対象売上日From
        {
            get
            {
                // 前回請求日の翌日
                if (_前回請求日 == null)
                {
                    return null;
                }
                DateTime d = (DateTime)_前回請求日;
                d = d.AddDays(1);
                return d;
            }
        }

        public DateTime? 対象売上日To
        {
            get
            {
                return _請求日;
            }
        }

        public string 対象期間
        {
            get
            {
                return string.Format("{0:yyyy年M月d日} ～ {1:yyyy年M月d日}", 対象売上日From, 対象売上日To);
            }
        }
        #endregion
    }
}

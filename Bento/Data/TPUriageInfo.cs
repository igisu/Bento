using Emu.Bento.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Emu.Bento.Data
{
    /// <summary>
    /// TP売上入金情報
    /// </summary>
    public class TPUriageInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private DateTime? _決済日;
        private int? _得意先ＣＤ;
        private string _得意先名;
        private int? _部署ＣＤ;
        private string _部署名;
        private int? _支店ＣＤ;
        private string _支店名;
        private int? _コースＣＤ;
        private string _コース名;
        private int? _配送担当者ＣＤ;
        private string _配送担当者名;
        private int? _商品ＣＤ;
        private string _商品名;
        private int? _明細番号;
        private decimal? _入金番号;
        private string _決済対象;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TPUriageInfo()
        {
            決済日 = null;
            得意先ＣＤ = null;
            得意先名 = null;
            部署ＣＤ = null;
            部署名 = null;
            支店ＣＤ = null;
            支店名 = null;
            コースＣＤ = null;
            コース名 = null;
            配送担当者ＣＤ = null;
            配送担当者名 = null;
            商品ＣＤ = null;
            商品名 = null;
            明細番号 = null;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TPUriageInfo(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;

            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            決済日 = dr["決済日", version] as DateTime?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            得意先名 = dr["得意先名", version] as string;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            部署名 = dr["部署名", version] as string;
            支店ＣＤ = dr["支店ＣＤ", version] as int?;
            支店名 = dr["支店名", version] as string;
            決済対象 = dr["決済対象", version] as string;

            if (dr.Table.Columns.Contains("明細番号"))
            {
                明細番号 = dr["明細番号", version] as int?;
            }
            if (dr.Table.Columns.Contains("入金番号"))
            {
                入金番号 = dr["入金番号", version] as decimal?;
            }
            if (dr.Table.Columns.Contains("コースＣＤ"))
            {
                コースＣＤ = dr["コースＣＤ", version] as int?;
            }
            if (dr.Table.Columns.Contains("コース名"))
            {
                コース名 = dr["コース名", version] as string;
            }
            if (dr.Table.Columns.Contains("配送担当者ＣＤ"))
            {
                配送担当者ＣＤ = dr["配送担当者ＣＤ", version] as int?;
            }
            if (dr.Table.Columns.Contains("配送担当者名"))
            {
                配送担当者名 = dr["配送担当者名", version] as string;
            }
            if (dr.Table.Columns.Contains("商品ＣＤ"))
            {
                商品ＣＤ = dr["商品ＣＤ", version] as int?;
            }
            if (dr.Table.Columns.Contains("商品名"))
            {
                商品名 = dr["商品名", version] as string;
            }
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        public DateTime? 決済日
        {
            get { return _決済日; }
            set { _決済日 = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
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

        public int? 配送担当者ＣＤ
        {
            get { return _配送担当者ＣＤ; }
            set { _配送担当者ＣＤ = value; }
        }

        public string 配送担当者名
        {
            get { return _配送担当者名; }
            set { _配送担当者名 = value; }
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

        public int? 明細番号
        {
            get { return _明細番号; }
            set { _明細番号 = value; }
        }

        public decimal? 入金番号
        {
            get { return _入金番号; }
            set { _入金番号 = value; }
        }

        public string 決済対象
        {
            get { return _決済対象; }
            set { _決済対象 = value; }
        }

        #endregion
    }
}

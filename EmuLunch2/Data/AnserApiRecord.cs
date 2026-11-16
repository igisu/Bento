using System;
using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// ANSER-APIファイル読込結果（請求・入金データ含む）
    /// </summary>
    public class AnserApiRecord
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _振込依頼人名;
        private int? _得意先ＣＤ;
        private int? _部署ＣＤ;
        private string _部署名;
        private decimal? _取込入金額;
        private decimal? _手数料;
        private bool? _振込手数料フラグ;
        private decimal? _前回請求額;
        private decimal? _今回請求額;
        private decimal? _請求後入金;
        private DateTime? _入金日;
        private string _取引名;
        private bool? _無効フラグ;

        private decimal? _請求番号;
        private string _得意先名;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnserApiRecord()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public AnserApiRecord(AnserApiRecord src)
        {
            振込依頼人名 = src.振込依頼人名;
            得意先ＣＤ = src.得意先ＣＤ;
            取込入金額 = src.取込入金額;
            部署ＣＤ = src.部署ＣＤ;
            部署名 = src.部署名;
            手数料 = src.手数料;
            振込手数料フラグ = src.振込手数料フラグ;
            前回請求額 = src.前回請求額;
            今回請求額 = src.今回請求額;
            請求後入金 = src.請求後入金;
            入金日 = src.入金日;
            取引名 = src.取引名;
            無効フラグ = src.無効フラグ;

            請求番号 = src.請求番号;
            得意先名 = src.得意先名;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public AnserApiRecord(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            振込依頼人名 = dr["振込依頼人名", version] as string;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            取込入金額 = dr["取込入金額", version] as decimal?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            部署名 = dr["部署名", version] as string;
            手数料 = dr["手数料", version] as decimal?;
            振込手数料フラグ = dr["振込手数料フラグ", version] as bool?;
            前回請求額 = dr["前回請求額", version] as decimal?;
            今回請求額 = dr["今回請求額", version] as decimal?;
            請求後入金 = dr["請求後入金", version] as decimal?;
            入金日 = dr["入金日", version] as DateTime?;
            取引名 = dr["取引名", version] as string;
            無効フラグ = dr["無効フラグ", version] as bool?;

            請求番号 = dr["請求番号", version] as decimal?;
            得意先名 = dr["得意先名", version] as string;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string 振込依頼人名
        {
            get { return _振込依頼人名; }
            set { _振込依頼人名 = value; }
        }

        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
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

        public decimal? 取込入金額
        {
            get { return _取込入金額; }
            set { _取込入金額 = value; }
        }

        public decimal? 手数料
        {
            get { return _手数料; }
            set { _手数料 = value; }
        }

        public bool? 振込手数料フラグ
        {
            get { return _振込手数料フラグ; }
            set { _振込手数料フラグ = value; }
        }

        public decimal? 前回請求額
        {
            get { return _前回請求額; }
            set { _前回請求額 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
        }

        public decimal? 請求後入金
        {
            get { return _請求後入金; }
            set { _請求後入金 = value; }
        }

        public DateTime? 入金日
        {
            get { return _入金日; }
            set { _入金日 = value; }
        }

        public string 取引名
        {
            get { return _取引名; }
            set { _取引名 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }


        public decimal? 請求番号
        {
            get { return _請求番号; }
            set { _請求番号 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// データ行への設定
        /// </summary>
        /// <param name="dr"></param>
        public void SetDataRow(DataRow dr)
        {
            // DataRow への設定は SetField メソッドを使って書くと DBNull を意識しなくて済む
            dr.SetField<string>("振込依頼人名", _振込依頼人名);
            dr.SetField<int?>("得意先ＣＤ", _得意先ＣＤ);
            dr.SetField<decimal?>("取込入金額", _取込入金額);
            dr.SetField<int?>("部署ＣＤ", _部署ＣＤ);
            dr.SetField<string>("部署名", _部署名);
            dr.SetField<decimal?>("手数料", _手数料);
            dr.SetField<bool?>("振込手数料フラグ", _振込手数料フラグ);
            dr.SetField<decimal?>("前回請求額", _前回請求額);
            dr.SetField<decimal?>("今回請求額", _今回請求額);
            dr.SetField<decimal?>("請求後入金", _請求後入金);
            dr.SetField<DateTime?>("入金日", _入金日);
            dr.SetField<string>("取引名", _取引名);
            dr.SetField<bool?>("無効フラグ", _無効フラグ);
            dr.SetField<decimal?>("請求番号", _請求番号);
            dr.SetField<string>("得意先名", _得意先名);
        }

        #endregion
    }
}

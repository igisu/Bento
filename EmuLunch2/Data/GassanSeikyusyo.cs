using System;
using System.Data;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 合算請求書
    /// </summary>
    public class GassanSeikyusyo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal? _請求番号;
        private int? _部署ＣＤ;
        private string _請求先名;
        private string _得意先名;
        private int? _得意先名長さ;
        private decimal? _前回請求額;
        private decimal? _入金額;
        private decimal? _繰越額;
        private decimal? _買上金額;
        private decimal? _調整額;
        private decimal? _消費税;
        private decimal? _今回請求額;
        private DateTime? _回収予定日;
        private bool? _回収予定日印字フラグ;
        private string _得意先郵便番号;
        private string _得意先住所1;
        private string _得意先住所2;
        private string _得意先電話番号;
        private string _得意先ＦＡＸ;
        private bool? _振込手数料フラグ;
        private int _請求書タイプ区分;
        private string _自社名称;
        private string _自社郵便番号;
        private string _自社住所;
        private string _自社電話番号;
        private string _自社ＦＡＸ;
        private string _金融機関ＣＤ1;
        private string _銀行名1;
        private string _金融機関支店ＣＤ1;
        private string _支店名1;
        private int? _口座種別1;
        private string _口座種別名1;
        private string _口座番号1;
        private string _口座名義人1;
        private string _金融機関ＣＤ2;
        private string _銀行名2;
        private string _金融機関支店ＣＤ2;
        private string _支店名2;
        private int? _口座種別2;
        private string _口座種別名2;
        private string _口座番号2;
        private string _口座名義人2;
        private string _合算対象請求番号;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GassanSeikyusyo()
        {
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public GassanSeikyusyo(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            請求番号 = dr["請求番号", version] as decimal?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            請求先名 = dr["請求先名", version] as string;
            得意先名 = dr["得意先名", version] as string;
            得意先名長さ = dr["得意先名長さ", version] as int?;
            前回請求額 = dr["前回請求額", version] as decimal?;
            入金額 = dr["入金額", version] as decimal?;
            繰越額 = dr["繰越額", version] as decimal?;
            買上金額 = dr["買上金額", version] as decimal?;
            調整額 = dr["調整額", version] as decimal?;
            消費税 = dr["消費税", version] as decimal?;
            今回請求額 = dr["今回請求額", version] as decimal?;
            回収予定日 = dr["回収予定日", version] as DateTime?;
            回収予定日印字フラグ = dr["回収予定日印字フラグ", version] as bool?;
            得意先郵便番号 = dr["得意先郵便番号", version] as string;
            得意先住所1 = dr["得意先住所1", version] as string;
            得意先住所2 = dr["得意先住所2", version] as string;
            得意先電話番号 = dr["得意先電話番号", version] as string;
            得意先ＦＡＸ = dr["得意先ＦＡＸ", version] as string;
            振込手数料フラグ = dr["振込手数料フラグ", version] as bool?;
            請求書タイプ区分 = (int)dr["請求書タイプ区分", version];
            自社名称 = dr["自社名称", version] as string;
            自社郵便番号 = dr["自社郵便番号", version] as string;
            自社住所 = dr["自社住所", version] as string;
            自社電話番号 = dr["自社電話番号", version] as string;
            自社ＦＡＸ = dr["自社ＦＡＸ", version] as string;
            金融機関ＣＤ1 = dr["金融機関ＣＤ1", version] as string;
            銀行名1 = dr["銀行名1", version] as string;
            金融機関支店ＣＤ1 = dr["金融機関支店ＣＤ1", version] as string;
            支店名1 = dr["支店名1", version] as string;
            口座種別1 = dr["口座種別1", version] as int?;
            口座種別名1 = dr["口座種別名1", version] as string;
            口座番号1 = dr["口座番号1", version] as string;
            口座名義人1 = dr["口座名義人1", version] as string;
            金融機関ＣＤ2 = dr["金融機関ＣＤ2", version] as string;
            銀行名2 = dr["銀行名2", version] as string;
            金融機関支店ＣＤ2 = dr["金融機関支店ＣＤ2", version] as string;
            支店名2 = dr["支店名2", version] as string;
            口座種別2 = dr["口座種別2", version] as int?;
            口座種別名2 = dr["口座種別名2", version] as string;
            口座番号2 = dr["口座番号2", version] as string;
            口座名義人2 = dr["口座名義人2", version] as string;
            合算対象請求番号 = dr["合算対象請求番号", version] as string;
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

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? 得意先名長さ
        {
            get { return _得意先名長さ; }
            set { _得意先名長さ = value; }
        }

        public decimal? 前回請求額
        {
            get { return _前回請求額; }
            set { _前回請求額 = value; }
        }

        public decimal? 入金額
        {
            get { return _入金額; }
            set { _入金額 = value; }
        }

        public decimal? 繰越額
        {
            get { return _繰越額; }
            set { _繰越額 = value; }
        }

        public decimal? 買上金額
        {
            get { return _買上金額; }
            set { _買上金額 = value; }
        }

        public decimal? 調整額
        {
            get { return _調整額; }
            set { _調整額 = value; }
        }

        public decimal? 消費税
        {
            get { return _消費税; }
            set { _消費税 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
        }

        public DateTime? 回収予定日
        {
            get { return _回収予定日; }
            set { _回収予定日 = value; }
        }

        public bool? 回収予定日印字フラグ
        {
            get { return _回収予定日印字フラグ; }
            set { _回収予定日印字フラグ = value; }
        }

        public string 得意先郵便番号
        {
            get { return _得意先郵便番号; }
            set { _得意先郵便番号 = value; }
        }

        public string 得意先住所1
        {
            get { return _得意先住所1; }
            set { _得意先住所1 = value; }
        }

        public string 得意先住所2
        {
            get { return _得意先住所2; }
            set { _得意先住所2 = value; }
        }

        public string 得意先電話番号
        {
            get { return _得意先電話番号; }
            set { _得意先電話番号 = value; }
        }

        public string 得意先ＦＡＸ
        {
            get { return _得意先ＦＡＸ; }
            set { _得意先ＦＡＸ = value; }
        }

        public bool? 振込手数料フラグ
        {
            get { return _振込手数料フラグ; }
            set { _振込手数料フラグ = value; }
        }

        public int 請求書タイプ区分
        {
            get { return _請求書タイプ区分; }
            set { _請求書タイプ区分 = value; }
        }

        public string 自社名称
        {
            get { return _自社名称; }
            set { _自社名称 = value; }
        }

        public string 自社郵便番号
        {
            get { return _自社郵便番号; }
            set { _自社郵便番号 = value; }
        }

        public string 自社住所
        {
            get { return _自社住所; }
            set { _自社住所 = value; }
        }

        public string 自社電話番号
        {
            get { return _自社電話番号; }
            set { _自社電話番号 = value; }
        }

        public string 自社ＦＡＸ
        {
            get { return _自社ＦＡＸ; }
            set { _自社ＦＡＸ = value; }
        }

        public string 金融機関ＣＤ1
        {
            get { return _金融機関ＣＤ1; }
            set { _金融機関ＣＤ1 = value; }
        }

        public string 銀行名1
        {
            get { return _銀行名1; }
            set { _銀行名1 = value; }
        }

        public string 金融機関支店ＣＤ1
        {
            get { return _金融機関支店ＣＤ1; }
            set { _金融機関支店ＣＤ1 = value; }
        }

        public string 支店名1
        {
            get { return _支店名1; }
            set { _支店名1 = value; }
        }

        public int? 口座種別1
        {
            get { return _口座種別1; }
            set { _口座種別1 = value; }
        }

        public string 口座種別名1
        {
            get { return _口座種別名1; }
            set { _口座種別名1 = value; }
        }

        public string 口座番号1
        {
            get { return _口座番号1; }
            set { _口座番号1 = value; }
        }

        public string 口座名義人1
        {
            get { return _口座名義人1; }
            set { _口座名義人1 = value; }
        }

        public string 金融機関ＣＤ2
        {
            get { return _金融機関ＣＤ2; }
            set { _金融機関ＣＤ2 = value; }
        }

        public string 銀行名2
        {
            get { return _銀行名2; }
            set { _銀行名2 = value; }
        }

        public string 金融機関支店ＣＤ2
        {
            get { return _金融機関支店ＣＤ2; }
            set { _金融機関支店ＣＤ2 = value; }
        }

        public string 支店名2
        {
            get { return _支店名2; }
            set { _支店名2 = value; }
        }

        public int? 口座種別2
        {
            get { return _口座種別2; }
            set { _口座種別2 = value; }
        }

        public string 口座種別名2
        {
            get { return _口座種別名2; }
            set { _口座種別名2 = value; }
        }

        public string 口座番号2
        {
            get { return _口座番号2; }
            set { _口座番号2 = value; }
        }

        public string 口座名義人2
        {
            get { return _口座名義人2; }
            set { _口座名義人2 = value; }
        }

        public string 合算対象請求番号
        {
            get { return _合算対象請求番号; }
            set { _合算対象請求番号 = value; }
        }

        public string 銀行支店名1
        {
            get
            {
                return MakeBankName(_銀行名1, _支店名1);
            }
        }

        public string 銀行支店名2
        {
            get
            {
                return MakeBankName(_銀行名2, _支店名2);
            }
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
            dr.SetField<decimal?>("請求番号", _請求番号);
            dr.SetField<int?>("部署ＣＤ", _部署ＣＤ);
            dr.SetField<string>("請求先名", _請求先名);
            dr.SetField<string>("得意先名", _得意先名);
            dr.SetField<int?>("得意先名長さ", _得意先名長さ);
            dr.SetField<decimal?>("前回請求額", _前回請求額);
            dr.SetField<decimal?>("入金額", _入金額);
            dr.SetField<decimal?>("繰越額", _繰越額);
            dr.SetField<decimal?>("買上金額", _買上金額);
            dr.SetField<decimal?>("調整額", _調整額);
            dr.SetField<decimal?>("消費税", _消費税);
            dr.SetField<decimal?>("今回請求額", _今回請求額);
            dr.SetField<DateTime?>("回収予定日", _回収予定日);
            dr.SetField<bool?>("回収予定日印字フラグ", _回収予定日印字フラグ);
            dr.SetField<string>("得意先郵便番号", _得意先郵便番号);
            dr.SetField<string>("得意先住所1", _得意先住所1);
            dr.SetField<string>("得意先住所2", _得意先住所2);
            dr.SetField<string>("得意先電話番号", _得意先電話番号);
            dr.SetField<string>("得意先ＦＡＸ", _得意先ＦＡＸ);
            dr.SetField<bool?>("振込手数料フラグ", _振込手数料フラグ);
            dr.SetField<int?>("請求書タイプ区分", _請求書タイプ区分);
            dr.SetField<string>("自社名称", _自社名称);
            dr.SetField<string>("自社郵便番号", _自社郵便番号);
            dr.SetField<string>("自社住所", _自社住所);
            dr.SetField<string>("自社電話番号", _自社電話番号);
            dr.SetField<string>("自社ＦＡＸ", _自社ＦＡＸ);
            dr.SetField<string>("金融機関ＣＤ1", _金融機関ＣＤ1);
            dr.SetField<string>("銀行名1", _銀行名1);
            dr.SetField<string>("金融機関支店ＣＤ1", _金融機関支店ＣＤ1);
            dr.SetField<string>("支店名1", _支店名1);
            dr.SetField<int?>("口座種別1", _口座種別1);
            dr.SetField<string>("口座種別名1", _口座種別名1);
            dr.SetField<string>("口座番号1", _口座番号1);
            dr.SetField<string>("口座名義人1", _口座名義人1);
            dr.SetField<string>("金融機関ＣＤ2", _金融機関ＣＤ2);
            dr.SetField<string>("銀行名2", _銀行名2);
            dr.SetField<string>("金融機関支店ＣＤ2", _金融機関支店ＣＤ2);
            dr.SetField<string>("支店名2", _支店名2);
            dr.SetField<int?>("口座種別2", _口座種別2);
            dr.SetField<string>("口座種別名2", _口座種別名2);
            dr.SetField<string>("口座番号2", _口座番号2);
            dr.SetField<string>("口座名義人2", _口座名義人2);
            dr.SetField<string>("合算対象請求番号", _合算対象請求番号);
        }

        /// <summary>
        /// 銀行名と支店名から金融機関名を作る
        /// </summary>
        /// <param name="bankName">銀行名</param>
        /// <param name="branchName">支店名</param>
        /// <returns>銀行名と支店名を結合したもの</returns>
        private string MakeBankName(string bankName, string branchName)
        {
            string bank = "";
            if (string.IsNullOrEmpty(bankName) == false)
            {
                bank = "銀行";
                // 銀行名の末尾が以下に該当しない場合は、末尾に"銀行"を付加する
                string[] bankSuffix = { "新銀行東京", "金庫", "信金", "組連", "信組", "機構", "金連", "労金", "中金", "信連", "農協", "漁連", "漁協" };
                foreach (string s in bankSuffix)
                {
                    if (bankName.EndsWith(s))
                    {
                        bank = "";
                        break;
                    }
                }
            }

            string branch = "";
            if (string.IsNullOrEmpty(branchName) == false)
            {
                branch = "支店";
                // 支店名の末尾が以下に該当しない場合は、末尾に"支店"を付加する
                string[] branchSuffix = { "公務部", "法人部", "営業部", "営業本部", "統括本部", "出張所", "年金", "本店", "営業第一部", "営業第二部", "営業第三部", "営業第四部", "営業第五部" };
                foreach (string s in branchSuffix)
                {
                    if (branchName.EndsWith(s))
                    {
                        branch = "";
                        break;
                    }
                }
            }

            string targetName = string.Format("{0}{1} {2}{3}", bankName, bank, branchName, branch);
            return targetName;
        }
        #endregion

    }
}

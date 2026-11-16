using System;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// ANSER-APIファイルレコード（「明細」行）
    /// </summary>
    /// <remarks>
    /// http://faq.ncbank.co.jp/dispFile.php?id=13
    /// http://faq.ncbank.co.jp/dispFile.php?id=15
    /// </remarks>
    public class AnserApiDetail
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        const int COLUMNS_COUNT = 30;         // 項目数
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _レコード区分;
        private int? _日付月;
        private int? _日付日;
        private int? _時刻時;
        private int? _時刻分;
        private string _連絡先名;
        private string _金融機関名;
        private string _支店名;
        private string _口座番号区分;
        private string _口座種別;
        private string _口座番号;
        private string _再送表示;
        private string _取引名;
        private string _取引番号;
        private string _明細区分;
        private int? _取扱日付月;
        private int? _取扱日付日;
        private int? _起算日月;
        private int? _起算日日;
        private decimal? _金額;
        private string _小切手区分;
        private string _摘要;
        private int? _取立枚数;
        private string _取立番号;
        private string _需要家番号;
        private string _振込金融機関名;
        private string _振込金融機関支店名;
        private string _振込メッセージ;
        private string _備考;
        private int? _件数;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string レコード区分
        {
            get { return _レコード区分; }
            set { _レコード区分 = value; }
        }

        public int? 日付月
        {
            get { return _日付月; }
            set { _日付月 = value; }
        }

        public int? 日付日
        {
            get { return _日付日; }
            set { _日付日 = value; }
        }

        public int? 時刻時
        {
            get { return _時刻時; }
            set { _時刻時 = value; }
        }

        public int? 時刻分
        {
            get { return _時刻分; }
            set { _時刻分 = value; }
        }

        public string 連絡先名
        {
            get { return _連絡先名; }
            set { _連絡先名 = value; }
        }

        public string 金融機関名
        {
            get { return _金融機関名; }
            set { _金融機関名 = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public string 口座番号区分
        {
            get { return _口座番号区分; }
            set { _口座番号区分 = value; }
        }

        public string 口座種別
        {
            get { return _口座種別; }
            set { _口座種別 = value; }
        }

        public string 口座番号
        {
            get { return _口座番号; }
            set { _口座番号 = value; }
        }

        public string 再送表示
        {
            get { return _再送表示; }
            set { _再送表示 = value; }
        }

        public string 取引名
        {
            get { return _取引名; }
            set { _取引名 = value; }
        }

        public string 取引番号
        {
            get { return _取引番号; }
            set { _取引番号 = value; }
        }

        public string 明細区分
        {
            get { return _明細区分; }
            set { _明細区分 = value; }
        }

        public int? 取扱日付月
        {
            get { return _取扱日付月; }
            set { _取扱日付月 = value; }
        }

        public int? 取扱日付日
        {
            get { return _取扱日付日; }
            set { _取扱日付日 = value; }
        }

        public int? 起算日月
        {
            get { return _起算日月; }
            set { _起算日月 = value; }
        }

        public int? 起算日日
        {
            get { return _起算日日; }
            set { _起算日日 = value; }
        }

        public decimal? 金額
        {
            get { return _金額; }
            set { _金額 = value; }
        }

        public string 小切手区分
        {
            get { return _小切手区分; }
            set { _小切手区分 = value; }
        }

        public string 摘要
        {
            get { return _摘要; }
            set { _摘要 = value; }
        }

        public int? 取立枚数
        {
            get { return _取立枚数; }
            set { _取立枚数 = value; }
        }

        public string 取立番号
        {
            get { return _取立番号; }
            set { _取立番号 = value; }
        }

        public string 需要家番号
        {
            get { return _需要家番号; }
            set { _需要家番号 = value; }
        }

        public string 振込金融機関名
        {
            get { return _振込金融機関名; }
            set { _振込金融機関名 = value; }
        }

        public string 振込金融機関支店名
        {
            get { return _振込金融機関支店名; }
            set { _振込金融機関支店名 = value; }
        }

        public string 振込メッセージ
        {
            get { return _振込メッセージ; }
            set { _振込メッセージ = value; }
        }

        public string 備考
        {
            get { return _備考; }
            set { _備考 = value; }
        }

        public int? 件数
        {
            get { return _件数; }
            set { _件数 = value; }
        }

        public DateTime? 取扱日付
        {
            get
            {
                if (_取扱日付月 == null || _取扱日付日 == null)
                {
                    return null;
                }
                DateTime d = new DateTime(DateTime.Today.Year, (int)_取扱日付月, (int)_取扱日付日);
                // 未来の場合は前年にする（12/25を2019/1/23に読み込んだ場合は、2019/12/25ではなく2018/12/25とみなす）
                if (DateTime.Today < d)
                {
                    d = d.AddYears(-1);
                }
                return d;
            }
        }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="s">レコード文字列</param>
        public AnserApiDetail(string s)
        {
            try
            {
                string[] tokens = s.Split('\t');
                if (tokens.Length != COLUMNS_COUNT)
                {
                    throw new BentoException("APIファイルのフォーマット（項目数）が不正です。");
                }
                int i = 0;
                Parse(tokens[i++], ref _レコード区分);
                Parse(tokens[i++], ref _日付月);
                Parse(tokens[i++], ref _日付日);
                Parse(tokens[i++], ref _時刻時);
                Parse(tokens[i++], ref _時刻分);
                Parse(tokens[i++], ref _連絡先名);
                Parse(tokens[i++], ref _金融機関名);
                Parse(tokens[i++], ref _支店名);
                Parse(tokens[i++], ref _口座番号区分);
                Parse(tokens[i++], ref _口座種別);
                Parse(tokens[i++], ref _口座番号);
                Parse(tokens[i++], ref _再送表示);
                Parse(tokens[i++], ref _取引名);
                Parse(tokens[i++], ref _取引番号);
                Parse(tokens[i++], ref _明細区分);
                Parse(tokens[i++], ref _取扱日付月);
                Parse(tokens[i++], ref _取扱日付日);
                Parse(tokens[i++], ref _起算日月);
                Parse(tokens[i++], ref _起算日日);
                Parse(tokens[i++], ref _金額);
                Parse(tokens[i++], ref _小切手区分);
                Parse(tokens[i++], ref _摘要);
                Parse(tokens[i++], ref _取立枚数);
                Parse(tokens[i++], ref _取立番号);
                Parse(tokens[i++], ref _需要家番号);
                Parse(tokens[i++], ref _振込金融機関名);
                Parse(tokens[i++], ref _振込金融機関支店名);
                Parse(tokens[i++], ref _振込メッセージ);
                Parse(tokens[i++], ref _備考);
                Parse(tokens[i++], ref _件数);
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 文字列をパースする
        /// </summary>
        /// <param name="s">パース対象の文字列</param>
        /// <param name="prop">パース先変数</param>
        private void Parse(string s, ref int? prop)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                {
                    return;
                }
                prop = int.Parse(s);
            }
            catch (Exception)
            {
                throw new BentoException(string.Format("APIファイルのフォーマット（書式）が不正です。期待する書式=数値 項目値={0}", s));
            }
        }

        /// <summary>
        /// 文字列をパースする
        /// </summary>
        /// <param name="s">パース対象の文字列</param>
        /// <param name="prop">パース先変数</param>
        private void Parse(string s, ref decimal? prop)
        {
            try
            {
                if (string.IsNullOrEmpty(s))
                {
                    return;
                }
                prop = decimal.Parse(s);
            }
            catch (Exception)
            {
                throw new BentoException(string.Format("APIファイルのフォーマット（書式）が不正です。期待する書式=金額 項目値={0}", s));
            }
        }

        /// <summary>
        /// 文字列をパースする
        /// </summary>
        /// <param name="s">パース対象の文字列</param>
        /// <param name="prop">パース先変数</param>
        private void Parse(string s, ref string prop)
        {
            prop = string.Copy(s);
        }
        #endregion

    }
}

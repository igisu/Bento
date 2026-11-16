using System;
using System.Collections.Generic;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// ANSER-APIファイル（ANSER-APIファイルフォーマットの集合）
    /// </summary>
    public class AnserApiContainer
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// １行目の項目数
        /// </summary>
        private const int FIRST_LINE_COLUMN_COUNT = 3;
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
        /// <summary>
        /// ANSER-APIファイルレコードリスト
        /// </summary>
        private List<AnserApiDetail> _anserApiDetailList;

        private int? _入金摘要区分;
        private string _金融機関ＣＤ;
        private string _金融機関名;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnserApiContainer()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="contents">ファイル内容文字列</param>
        public AnserApiContainer(string contents)
        {
            _anserApiDetailList = new List<AnserApiDetail>();   // ANSER-APIファイルレコードリストを生成
            ReadAPI(contents);                                  // ANSER-APIファイルを読み込む
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 入金摘要区分
        {
            get { return _入金摘要区分; }
            set { _入金摘要区分 = value; }
        }

        public string 金融機関ＣＤ
        {
            get { return _金融機関ＣＤ; }
            set { _金融機関ＣＤ = value; }
        }

        public string 金融機関名
        {
            get { return _金融機関名; }
            set { _金融機関名 = value; }
        }

        /// <summary>
        /// ANSER-APIファイルレコードリスト
        /// </summary>
        public List<AnserApiDetail> AnserApiDetailList
        {
            get { return _anserApiDetailList; }
            set { _anserApiDetailList = value; }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// ANSER-APIファイル文字列を読み込む
        /// </summary>
        /// <param name="contents">ANSER-APIファイル内容文字列</param>
        private void ReadAPI(string contents)
        {
            try
            {
                _入金摘要区分 = -1;
                _金融機関名 = "";

                string[] lines = contents.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    // １行目は以下のようになっている
                    // "サービス情報"   金融機関コード  上２桁ANSERサービスコード、下２桁"00"
                    // このうち、金融機関コードを取得する
                    if (line.StartsWith("サービス情報"))
                    {
                        string[] tokens = line.Split('\t');
                        if (tokens.Length != FIRST_LINE_COLUMN_COUNT)
                        {
                            throw new BentoException("APIファイルのフォーマット（項目数）が不正です。");
                        }
                        try
                        {
                            int wk_bankCD = int.Parse(tokens[1]);
                            _金融機関ＣＤ = string.Format("{0:D4}", wk_bankCD);   // 190 → 0190 にする
                        }
                        catch (Exception)
                        {
                            throw new BentoException(string.Format("APIファイルのフォーマット（書式）が不正です。期待する書式=数値 項目値={0}", tokens[1]));
                        }

                        // 金融機関ＣＤに該当する銀行名を取得
                        Bank bank = Bank.GetBank(_金融機関ＣＤ);
                        if (bank != null)
                        {
                            _金融機関名 = bank.銀行名;
                        }

                        // 銀行名の末尾が以下に該当しない場合は、末尾に"銀行"を付加する
                        string bankSuffix = "銀行";
                        string[] bankSuffixArray = { "新銀行東京", "金庫", "信金", "組連", "信組", "機構", "金連", "労金", "中金", "信連", "農協", "漁連", "漁協" };
                        foreach (string s in bankSuffixArray)
                        {
                            if (_金融機関名.EndsWith(s))
                            {
                                bankSuffix = "";
                                break;
                            }
                        }
                        _金融機関名 += bankSuffix;
                    }

                    // 明細行と合計行があるが、明細行 かつ 取引名に「入金」を含んでいる行だけを読み込めばいい
                    if (line.StartsWith("明細"))
                    {
                        AnserApiDetail anserApiRecord = new AnserApiDetail(line);
                        if (anserApiRecord.取引名.Contains("入金"))
                        {
                            _anserApiDetailList.Add(anserApiRecord);

                            // 入金摘要区分は取扱日付月（入金月）の前月にする
                            if (_入金摘要区分 == -1)
                            {
                                if (anserApiRecord.取扱日付月 != null)
                                {
                                    if (anserApiRecord.取扱日付月 == 1)
                                    {
                                        _入金摘要区分 = 12;
                                    }
                                    else
                                    {
                                        _入金摘要区分 = (int)anserApiRecord.取扱日付月 - 1;
                                    }
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(_金融機関名) == false)
                {
                    金融機関名 += " API取込";
                }
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
        #endregion
    }
}

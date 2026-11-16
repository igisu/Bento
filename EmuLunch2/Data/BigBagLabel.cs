using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Emu.Bento.Common;
using log4net;
using System.Text;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 大袋ラベル
    /// </summary>
    /// <remarks>
    /// 以下の２種類として使用する
    /// ・DBからの取得結果
    /// ・ラベル出力用データ
    /// </remarks>
    public class BigBagLabel
    {
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
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// セット用のラベルデータリストを生成します
        /// </summary>
        /// <param name="record">DBからの取得結果</param>
        /// <returns>セット用のラベルデータリスト</returns>
        public static List<BigBagLabel> GetSetLabels(BigBagLabel record)
        {
            try
            {
                List<BigBagLabel> labelList = new List<BigBagLabel>();
                BigBagLabel label;

                //**********************************************************
                // 10食ごととその余りの1枚のラベルを出力する
                // （例えば24食だと10食が2枚と4食が1枚）
                // ただし以下の例外がある
                // 11,21,31…の場合は、n枚の10食と、1枚の5食と1枚の6食を作る
                // 12,22,32…の場合は、n枚の10食と、1枚の5食と1枚の7食を作る
                //**********************************************************

                // 11以上？
                if (record.食数 >= 11)
                {
                    // 1の位が1or2のものを処理する
                    int remain = record.食数 % 10;
                    if (remain == 1)
                    {
                        // 11の場合は5と6に分割する
                        label = new BigBagLabel();
                        label.喫食日 = record.喫食日;
                        label.大袋ラベル食事区分名 = record.大袋ラベル食事区分名;
                        label.食数 = 5;
                        labelList.Add(label);

                        label = new BigBagLabel();
                        label.喫食日 = record.喫食日;
                        label.大袋ラベル食事区分名 = record.大袋ラベル食事区分名;
                        label.食数 = 6;
                        labelList.Add(label);

                        record.食数 -= (5+6);
                    }

                    if (remain == 2)
                    {
                        // 12の場合は5と7に分割する
                        label = new BigBagLabel();
                        label.喫食日 = record.喫食日;
                        label.大袋ラベル食事区分名 = record.大袋ラベル食事区分名;
                        label.食数 = 5;
                        labelList.Add(label);

                        label = new BigBagLabel();
                        label.喫食日 = record.喫食日;
                        label.大袋ラベル食事区分名 = record.大袋ラベル食事区分名;
                        label.食数 = 7;
                        labelList.Add(label);

                        record.食数 -= (5+7);
                    }

                    // まだ残っている場合は、10の位を処理する
                    while (record.食数 >= 10)
                    {
                        // 10食のラベルを生成
                        label = new BigBagLabel();
                        label.喫食日 = record.喫食日;
                        label.大袋ラベル食事区分名 = record.大袋ラベル食事区分名;
                        label.食数 = 10;
                        labelList.Add(label);

                        record.食数 -= 10;
                    }
                }

                // (11以上を処理した結果)0以下なら終了
                if (record.食数 > 0)
                {
                    // (11以上は処理済みなので)今の値をそのまま格納する
                    // ただし3以下は3とみなす
                    if (record.食数 <= 3)
                    {
                        record.食数 = 3;
                    }
                    label = new BigBagLabel(record);
                    labelList.Add(label);
                }

                // if (log.IsDebugEnabled)
                // {
                //     string comma = "";
                //     StringBuilder s = new StringBuilder();
                //     foreach (BigBagLabel bbl in labelList)
                //     {
                //         s.AppendFormat("{0}{1}", comma, bbl.食数);
                //         comma = ", ";
                //     }
                //     log.DebugFormat("{0} → {1}", record.商品数量, s.ToString());
                // }

                return labelList;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// 昇順ソート用比較メソッド
        /// </summary>
        /// <param name="x">比較する最初のオブジェクト</param>
        /// <param name="y">比較する 2 番目のオブジェクト</param>
        /// <returns>x が y より小さい場合は 0 未満。x と y が等しい場合は 0。x が y より大きい場合は 0超過</returns>
        public static int CompareAscending(BigBagLabel x, BigBagLabel y)
        {
            // 食数の昇順で比較
            return x.食数 - y.食数;
        }

        /// <summary>
        /// 降順ソート用比較メソッド
        /// </summary>
        /// <param name="x">比較する最初のオブジェクト</param>
        /// <param name="y">比較する 2 番目のオブジェクト</param>
        /// <returns>x が y より大きい場合は 0 未満。x と y が等しい場合は 0。x が y より小さい場合は 0超過</returns>
        public static int CompareDescending(BigBagLabel x, BigBagLabel y)
        {
            // 食数の降順で比較
            return y.食数 - x.食数;
        }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BigBagLabel()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public BigBagLabel(BigBagLabel src)
        {
            喫食日 = src.喫食日;
            大袋ラベル食事区分名 = src.大袋ラベル食事区分名;
            商品数量 = src.商品数量;
            食数 = src.食数;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public BigBagLabel(DataRow dr)
        {
            喫食日 = dr["喫食日"] as DateTime?;
            大袋ラベル食事区分名 = dr["大袋ラベル食事区分名"] as string;
            商品数量 = (decimal)dr["商品数量"];
            食数 = (int)商品数量;       // DBから取得した場合のみ、商品数量で食数を初期化
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// 喫食日
        /// </summary>
        public DateTime? 喫食日 { get; set; }

        /// <summary>
        /// 大袋ラベル食事区分名
        /// </summary>
        public string 大袋ラベル食事区分名 { get; set; }

        /// <summary>
        /// 商品数量
        /// </summary>
        public decimal 商品数量 { get; set; }

        /// <summary>
        /// 食数（計算・出力用）
        /// </summary>
        public int 食数 { get; set; }

        /// <summary>
        /// 内訳テキスト
        /// </summary>
        public string 内訳テキスト
        {
            get
            {
                string text = "";
                switch (食数)
                {
                    case 3:
                        text = "3 x 1";
                        break;

                    case 4:
                        text = "4 x 1";
                        break;

                    case 5:
                        text = "5 x 1";
                        break;

                    case 6:
                        text = "3 x 2";
                        break;

                    case 7:
                        text = "4 x 1　3 x 1";
                        break;

                    case 8:
                        text = "4 x 2";
                        break;

                    case 9:
                        text = "5 x 1　4 x 1";
                        break;

                    case 10:
                        text = "5 x 2";
                        break;
                }

                return text;
            }
        }
        #endregion
    }
}

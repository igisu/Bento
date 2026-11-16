using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 消費税率マスタ
    /// </summary>
    public class Syohizei : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値
        /// <summary>
        /// 税区分
        /// </summary>
        public enum ZeiKubun
        {
            /// <summary>
            /// 内税
            /// </summary>
            Uchi = 1,

            /// <summary>
            /// 外税
            /// </summary>
            Soto = 2,

            /// <summary>
            /// 非課税
            /// </summary>
            Hikazei = 3,
        }
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数

        /// <summary>
        /// 消費税率マスタ
        /// </summary>
        private static DataTable _dtSyohizei = null;

        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private static readonly object _lockObj = new object();

        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 指定日に該当する消費税率マスタを返します。
        /// </summary>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns></returns>
        public static Syohizei GetSyohizei(DateTime targetDate)
        {
            lock (_lockObj)
            {
                Syohizei syohizei = null;

                // 未取得の場合のみ取得
                if (_dtSyohizei == null)
                {
                    using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = con.CreateCommand();
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     施行日" +
                                          "\r\n     ,消費税率" +
                                          "\r\n     ,軽減税率" +
                                          "\r\n FROM" +
                                          "\r\n     消費税率マスタ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;

                        SqlDataAdapter adp = new SqlDataAdapter(cmd);

                        log.Debug(cmd.CommandTextWithParameters());

                        _dtSyohizei = new DataTable("消費税率マスタ");
                        adp.Fill(_dtSyohizei);
                    }
                }

                // 適用日時点で有効な税率を取得
                DataRow[] drs = _dtSyohizei.Select(string.Format("施行日 <= '{0:yyyy/MM/dd}'", targetDate), "施行日 desc");
                if (drs != null && drs.Length != 0)
                {
                    syohizei = new Syohizei(drs[0]);
                }

                return syohizei;
            }
        }

        /// <summary>
        /// 税区分に応じた商品単価を返します
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="taxIncludedPrice">税込み単価</param>
        /// <param name="taxExcludedPrice">税抜き単価</param>
        /// <returns>税区分に応じた商品単価</returns>
        public static decimal GetUnitPrice(ZeiKubun zeiKubun, decimal? taxIncludedPrice, decimal? taxExcludedPrice)
        {
            if (zeiKubun == ZeiKubun.Uchi)
            {
                return taxIncludedPrice ?? 0m;  // 内税の場合は税込み単価を返す
            }
            return taxExcludedPrice ?? 0m;      // 外税・非課税の場合は税抜き単価を返す
        }

        /// <summary>
        /// 税区分に応じた商品単価を返します
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="taxIncludedPrice">税込み単価</param>
        /// <param name="taxExcludedPrice">税抜き単価</param>
        /// <returns>税区分に応じた商品単価</returns>
        public static decimal GetUnitPrice(int? zeiKubun, decimal? taxIncludedPrice, decimal? taxExcludedPrice)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetUnitPrice(zeiKubunEnum, taxIncludedPrice, taxExcludedPrice);
        }

        /// <summary>
        /// 商品金額合計（数量×商品単価）を求める
        /// </summary>
        /// <param name="suryo">数量</param>
        /// <param name="price">単価</param>
        /// <returns>商品金額合計</returns>
        public static decimal GetAmount(decimal? suryo, decimal? price)
        {
            // 商品金額合計＝数量×商品単価
            decimal wk_suryo = suryo ?? 0m;
            decimal wk_price = price ?? 0m;
            return wk_suryo * wk_price;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, int? hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, suryo, unitPrice, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, suryo, unitPrice, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal amount, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal amount, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal amount, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal amount, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal? amount, decimal taxRate, int? hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(ZeiKubun zeiKubun, decimal? amount, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal? amount, decimal taxRate, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, amount, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTax(int? zeiKubun, decimal? amount, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, amount, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）（丸め処理を除く）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">単価</param>
        /// <param name="taxRate">消費税率</param>
        /// <returns>税区分に応じた消費税額（丸め処理前）</returns>
        private static decimal GetTaxMain(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate)
        {
            // 内税・非課税の場合は、消費税額は0
            if (zeiKubun != ZeiKubun.Soto)
            {
                return 0m;
            }

            // 外税の場合は、消費税額＝商品金額合計×消費税率
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = amount * taxRate;
            return tax;
        }

        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）（丸め処理を除く）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">消費税率</param>
        /// <returns>税区分に応じた消費税額（丸め処理前）</returns>
        private static decimal GetTaxMain(ZeiKubun zeiKubun, decimal? amount, decimal taxRate)
        {
            // 内税・非課税の場合は、消費税額は0
            if (zeiKubun != ZeiKubun.Soto)
            {
                return 0m;
            }

            // 外税の場合は、消費税額＝金額×消費税率
            decimal tax = (amount ?? 0m) * taxRate;
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, int? hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, suryo, unitPrice, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, suryo, unitPrice, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal amount, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal amount, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal amount, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal amount, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxIncluded(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? amount, decimal taxRate, int? hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? amount, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal? amount, decimal taxRate, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, amount, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public static decimal GetTaxIncluded(int? zeiKubun, decimal? amount, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, amount, taxRate, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）（丸め処理を除く）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">単価</param>
        /// <param name="taxRate">消費税率</param>
        /// <returns>税区分に応じた消費税額（丸め処理前）</returns>
        private static decimal GetTaxIncludedMain(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate)
        {
            // 外税・非課税の場合は、金額に含まれる税額は0
            if (zeiKubun != ZeiKubun.Uchi)
            {
                return 0m;
            }

            // 内税の場合は、消費税額＝金額－(金額÷(１＋消費税率))
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = amount - (amount / (1m + taxRate));
            return tax;
        }

        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）（丸め処理を除く）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">消費税率</param>
        /// <returns>税区分に応じた消費税額（丸め処理前）</returns>
        private static decimal GetTaxIncludedMain(ZeiKubun zeiKubun, decimal? amount, decimal taxRate)
        {
            // 外税・非課税の場合は、金額に含まれる税額は0
            if (zeiKubun != ZeiKubun.Uchi)
            {
                return 0m;
            }

            // 内税の場合は、消費税額＝金額－(金額÷(１＋消費税率))
            decimal tax = (amount ?? 0m) - ((amount ?? 0m) / (1m + taxRate));
            return tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, int? hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, taxRate, hasuKubun);  // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, taxRate, hasuKubun);  // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, int? hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, taxRate, hasuKubun);  // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, taxRate, hasuKubun);  // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetAmountIncludeTax(zeiKubun, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? amount, decimal taxRate, int? hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, taxRate, hasuKubun);  // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? amount, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, taxRate, hasuKubun);  // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? amount, decimal taxRate, int? hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, taxRate, hasuKubun);  // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public static decimal GetAmountIncludeTax(int? zeiKubun, decimal? amount, decimal taxRate, Rounding.HasuKubun hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, taxRate, hasuKubun);  // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 消費税率適用日現在の消費税率（または軽減税率）を返します
        /// </summary>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>消費税率適用日現在の消費税率（または軽減税率）</returns>
        public static decimal GetTaxRate(bool? isKeigen, DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.GetTaxRate(isKeigen);
        }

        /// <summary>
        /// 消費税率適用日現在で軽減税率が適用可能かどうかを返します
        /// </summary>
        /// <param name="targetDate">消費税率適用日</param>
        /// <returns>消費税率適用日現在で軽減税率が適用可能な場合はtrue。それ以外はfalse。</returns>
        public static bool HasKeigen(DateTime targetDate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.HasKeigen();
        }

        /// <summary>
        /// 軽減税率対象商品かどうかを返します
        /// </summary>
        /// <param name="targetDate">消費税率適用日(売上日)</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <returns>売上明細が持っている商品が軽減税率対象(=適用済み)の場合はtrue。対象外(=非適用)の場合はfalse</returns>
        public static bool IsKeigen(DateTime targetDate, decimal taxRate)
        {
            Syohizei syohizei = Syohizei.GetSyohizei(targetDate);
            return syohizei.IsKeigen(taxRate);
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private DateTime? _施行日;
        private decimal? _消費税率;
        private decimal? _軽減税率;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Syohizei()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Syohizei(Syohizei src)
        {
            施行日 = src.施行日;
            消費税率 = src.消費税率;
            軽減税率 = src.軽減税率;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Syohizei(DataRow dr)
        {
            施行日 = dr["施行日"] as DateTime?;
            消費税率 = dr["消費税率"] as decimal?;
            軽減税率 = dr["軽減税率"] as decimal?;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 施行日
        {
            get { return _施行日; }
            set { _施行日 = value; }
        }

        public decimal? 消費税率
        {
            get { return _消費税率; }
            set { _消費税率 = value; }
        }

        public decimal? 軽減税率
        {
            get { return _軽減税率; }
            set { _軽減税率 = value; }
        }

        public override string Code
        {
            get { return string.Format("施行日=[{0}]", 施行日); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
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
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(Syohizei target)
        {
            if (施行日 == target.施行日 &&
                消費税率 == target.消費税率 &&
                軽減税率 == target.軽減税率)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO 消費税率マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,施行日" +
                         "\r\n     ,消費税率" +
                         "\r\n     ,軽減税率" +
                         "\r\n  ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@施行日" +
                         "\r\n     ,@消費税率" +
                         "\r\n     ,@軽減税率" +
                         "\r\n )";

            return Save(con, tran, sql, History.ProcessType.Create);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Update(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n UPDATE 消費税率マスタ(" +
                         "\r\n SET  消費税率 = @消費税率" +
                         "\r\n     ,軽減税率 = @軽減税率" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n AND 施行日   = @施行日";

            return Save(con, tran, sql, History.ProcessType.Update);
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n DELETE 消費税率マスタ(" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n AND   施行日 = @施行日";

            return Save(con, tran, sql, History.ProcessType.Delete);
        }

        /// <summary>
        /// DB更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        protected override bool Save(SqlConnection con, SqlTransaction tran, string sql, History.ProcessType historyProcess)
        {
            try
            {
                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (施行日 != null)
                    {
                        cmd.Parameters.Add("@施行日", SqlDbType.Date).Value = 施行日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@施行日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (消費税率 != null)
                    {
                        cmd.Parameters.Add("@消費税率", SqlDbType.Decimal).Value = 消費税率;
                    }
                    else
                    {
                        cmd.Parameters.Add("@消費税率", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (軽減税率 != null)
                    {
                        cmd.Parameters.Add("@軽減税率", SqlDbType.Decimal).Value = 軽減税率;
                    }
                    else
                    {
                        cmd.Parameters.Add("@軽減税率", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("消費税率マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "施行日が重複しています。";
                    }
                    else
                    {
                        History.Error(ex);
                        Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    }
                    return false;

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    History.Error(ex);
                    Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    return false;
                }
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
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(int? zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額×消費税率）を求める（外税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTax(int? zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTax(zeiKubunEnum, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, suryo, unitPrice, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（商品金額合計に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, suryo, unitPrice, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 消費税率を決定
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }

            // 丸め処理前の消費税額を求める
            decimal tax = GetTaxIncludedMain(zeiKubun, amount, taxRate);
            tax = Rounding.Round(tax, hasuKubun);   // 丸め処理
            return tax;
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(int? zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 税区分に応じた消費税額（値引後金額に含まれる消費税額）を求める（内税）
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた消費税額</returns>
        public decimal GetTaxIncluded(int? zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            // 税区分を決定
            ZeiKubun zeiKubunEnum = ZeiKubun.Uchi;
            if (zeiKubun != null)
            {
                zeiKubunEnum = (ZeiKubun)(int)zeiKubun;
            }

            // オーバーロードメソッドを呼び出す
            return GetTaxIncluded(zeiKubunEnum, amount, isKeigen, hasuKubun);
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);     // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);     // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, int? hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);     // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="suryo">数量</param>
        /// <param name="unitPrice">商品単価</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(int? zeiKubun, decimal? suryo, decimal? unitPrice, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            decimal amount = GetAmount(suryo, unitPrice);   // 商品金額合計を算出
            decimal tax = GetTax(zeiKubun, suryo, unitPrice, isKeigen, hasuKubun);     // 消費税額を取得
            return amount + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, isKeigen, hasuKubun);     // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(ZeiKubun zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, isKeigen, hasuKubun);     // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分（1:切り上げ 2:切り捨て 3:四捨五入）</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(int? zeiKubun, decimal? amount, bool? isKeigen, int? hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, isKeigen, hasuKubun);     // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 商品金額合計税込を求める
        /// </summary>
        /// <param name="zeiKubun">税区分（1:内税 2:外税 3:非課税）</param>
        /// <param name="amount">(値引後)金額</param>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <param name="hasuKubun">端数区分</param>
        /// <returns>税区分に応じた商品金額合計税込</returns>
        public decimal GetAmountIncludeTax(int? zeiKubun, decimal? amount, bool? isKeigen, Rounding.HasuKubun hasuKubun)
        {
            decimal tax = GetTax(zeiKubun, amount, isKeigen, hasuKubun);     // 消費税額を取得
            return (amount ?? 0m) + tax;
        }

        /// <summary>
        /// 消費税率（または軽減税率）を返します
        /// </summary>
        /// <param name="isKeigen">軽減税率を適用する場合は true</param>
        /// <returns>消費税率（または軽減税率）</returns>
        public decimal GetTaxRate(bool? isKeigen)
        {
            decimal taxRate = (decimal)this.消費税率;
            if ((isKeigen ?? false) == true && this.軽減税率 != null)
            {
                taxRate = (decimal)this.軽減税率;
            }
            return taxRate;
        }

        /// <summary>
        /// 軽減税率が適用可能かどうかを返します
        /// </summary>
        /// <returns>軽減税率が適用可能な場合はtrue。それ以外はfalse。</returns>
        public bool HasKeigen()
        {
            return (this.軽減税率 != null);
        }

        /// <summary>
        /// 軽減税率対象商品かどうかを返します
        /// </summary>
        /// <param name="targetDate">消費税率適用日(売上日)</param>
        /// <param name="taxRate">計算に使用する税率</param>
        /// <returns>売上明細が持っている商品が軽減税率対象(=適用済み)の場合はtrue。対象外(=非適用)の場合はfalse</returns>
        public bool IsKeigen(decimal taxRate)
        {
            if (this.軽減税率 == null)
            {
                return false;
            }

            // 通常税率と異なる場合に軽減税率と判定する（2027.04.01からの軽減税率減税により8％と1％が混在する場合があるため）
            if (this.消費税率 == taxRate)
            {
                return false;
            }

            return true;
            //return (this.軽減税率 == taxRate);
        }
        #endregion
    }
}

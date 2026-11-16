using Emu.Bento.Data;
using System;
using System.Collections.Generic;

namespace Emu.Bento.WebAPI
{
    //**************************************
    // JSONモデルクラス
    //**************************************
    #region JSONモデルクラス

    #region 売上処理

    /// <summary>
    /// 売上処理APIリクエストモデル
    /// </summary>
    public class SaleSettlementRequestModel
    {
        #region プロパティ

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 請求番号
        /// </summary>
        public decimal? billingNumber { get; set; }

        /// <summary>
        /// 請求日
        /// </summary>
        public DateTime? billingDate { get; set; }

        /// <summary>
        /// 部署CD
        /// </summary>
        public int? departmentCD { get; set; }

        /// <summary>
        /// 請求先CD
        /// </summary>
        public int? billingCustomerCD { get; set; }

        /// <summary>
        /// 請求先名
        /// </summary>
        public string billingCustomerName { get; set; }

        /// <summary>
        /// 請求金額
        /// </summary>
        public decimal? billingAmount { get; set; }

        #endregion
    }

    /// <summary>
    /// 売上処理APIレスポンスモデル
    /// </summary>
    public class SaleSettlementResponseModel
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        public FRResponseHeaderModel header { get; set; }

        /// <summary>
        /// ボディ
        /// </summary>
        public SaleSettlementResponseBodyModel body { get; set; }
    }

    /// <summary>
    /// 売上処理APIレスポンスボディモデル
    /// </summary>
    public class SaleSettlementResponseBodyModel
    {
        #region プロパティ

        /// <summary>
        /// 店舗ID
        /// </summary>
        public string shopID { get; set; }

        /// <summary>
        /// 伝票番号
        /// </summary>
        public string slipNo { get; set; }

        /// <summary>
        /// 処理タイプ
        /// </summary>
        public int? processType { get; set; }

        /// <summary>
        /// 処理結果
        /// </summary>
        public int? processResult { get; set; }

        /// <summary>
        /// 処理結果内容
        /// </summary>
        public string processResultText { get; set; }

        /// <summary>
        /// 承認番号
        /// </summary>
        public string authCode { get; set; }

        /// <summary>
        /// 取引番号
        /// </summary>
        public string seqNo { get; set; }

        /// <summary>
        /// クレカ顧客番号
        /// </summary>
        public string userID { get; set; }

        #endregion
    }

    #endregion

    #region 売上取消処理

    /// <summary>
    /// 売上取消処理APIリクエストモデル
    /// </summary>
    public class SaleCancelRequestModel
    {
        #region プロパティ

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 請求番号
        /// </summary>
        public decimal? billingNumber { get; set; }

        /// <summary>
        /// 請求日
        /// </summary>
        public DateTime? billingDate { get; set; }

        /// <summary>
        /// 部署CD
        /// </summary>
        public int? departmentCD { get; set; }

        /// <summary>
        /// 請求先CD
        /// </summary>
        public int? billingCustomerCD { get; set; }

        /// <summary>
        /// 請求先名
        /// </summary>
        public string billingCustomerName { get; set; }

        /// <summary>
        /// 取引番号
        /// </summary>
        public string seqNo { get; set; }

        #endregion
    }

    /// <summary>
    /// 売上取消処理APIレスポンスモデル
    /// </summary>
    public class SaleCancelResponseModel
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        public FRResponseHeaderModel header { get; set; }

        /// <summary>
        /// ボディ
        /// </summary>
        public SaleCancelResponseBodyModel body { get; set; }
    }

    /// <summary>
    /// 売上取消処理APIレスポンスボディモデル
    /// </summary>
    public class SaleCancelResponseBodyModel
    {
        #region プロパティ

        /// <summary>
        /// 店舗ID
        /// </summary>
        public string shopID { get; set; }

        /// <summary>
        /// 処理タイプ
        /// </summary>
        public int? processType { get; set; }

        /// <summary>
        /// 処理結果
        /// </summary>
        public int? processResult { get; set; }

        /// <summary>
        /// 処理結果内容
        /// </summary>
        public string processResultText { get; set; }

        /// <summary>
        /// 取消を実行した取引番号
        /// </summary>
        public string cancelSeqNo { get; set; }

        #endregion
    }

    #endregion

    #region 決済履歴取得

    /// <summary>
    /// 決済履歴取得APIリクエストモデル
    /// </summary>
    public class InfoGetSettlementRequestModel
    {
        #region プロパティ

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 請求番号
        /// </summary>
        public decimal? billingNumber { get; set; }

        /// <summary>
        /// 請求日
        /// </summary>
        public DateTime? billingDate { get; set; }

        /// <summary>
        /// 部署CD
        /// </summary>
        public int? departmentCD { get; set; }

        /// <summary>
        /// 請求先CD
        /// </summary>
        public int? billingCustomerCD { get; set; }

        /// <summary>
        /// 請求先名
        /// </summary>
        public string billingCustomerName { get; set; }

        /// <summary>
        /// 請求金額
        /// </summary>
        public decimal? billingAmount { get; set; }

        #endregion
    }

    /// <summary>
    /// 決済履歴取得APIレスポンスモデル
    /// </summary>
    public class InfoGetSettlementResponseModel
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        public FRResponseHeaderModel header { get; set; }

        /// <summary>
        /// ボディ
        /// </summary>
        public InfoGetSettlementResponseBodyModel body { get; set; }
    }

    /// <summary>
    /// 決済履歴取得APIレスポンスボディモデル
    /// </summary>
    public class InfoGetSettlementResponseBodyModel
    {
        /// <summary>
        /// 決済履歴モデル
        /// </summary>
        public SettlementHistoryModel settlementHistory { get; set; }
    }

    /// <summary>
    /// 決済履歴モデル
    /// </summary>
    public class SettlementHistoryModel
    {
        #region プロパティ

        /// <summary>
        /// 履歴番号
        /// </summary>
        public decimal? historyNo { get; set; }

        /// <summary>
        /// 店舗ID
        /// </summary>
        public string shopID { get; set; }

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 請求番号
        /// </summary>
        public decimal? billingNumber { get; set; }

        /// <summary>
        /// 請求日
        /// </summary>
        public DateTime? billingDate { get; set; }

        /// <summary>
        /// 部署CD
        /// </summary>
        public int? departmentCD { get; set; }

        /// <summary>
        /// 請求先CD
        /// </summary>
        public int? billingCustomerCD { get; set; }

        /// <summary>
        /// 請求先名
        /// </summary>
        public string billingCustomerName { get; set; }

        /// <summary>
        /// 今回請求額
        /// </summary>
        public decimal? billingAmount { get; set; }

        /// <summary>
        /// 処理タイプ
        /// </summary>
        public int? processType { get; set; }

        /// <summary>
        /// 処理結果
        /// </summary>
        public int? processResult { get; set; }

        /// <summary>
        /// 処理結果内容
        /// </summary>
        public string processResultText { get; set; }

        /// <summary>
        /// 伝票番号
        /// </summary>
        public string slipNo { get; set; }

        /// <summary>
        /// 承認番号
        /// </summary>
        public string authCode { get; set; }

        /// <summary>
        /// 取引番号
        /// </summary>
        public string seqNo { get; set; }

        /// <summary>
        /// クレカ顧客番号
        /// </summary>
        public string userID { get; set; }

        /// <summary>
        /// 作成日
        /// </summary>
        public DateTime? createDate { get; set; }

        /// <summary>
        /// 修正日
        /// </summary>
        public DateTime? lastmodify { get; set; }

        #endregion
    }

    #endregion

    #region 手数料率リスト取得

    /// <summary>
    /// 手数料率リスト取得APIリクエストモデル
    /// </summary>
    public class InfoGetFeeRateListRequestModel
    {
        #region プロパティ

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 部署CD
        /// </summary>
        public int? departmentCD { get; set; }

        /// <summary>
        /// 支払方法CD
        /// </summary>
        public int? paymentTypeCD { get; set; }

        #endregion
    }

    /// <summary>
    /// 手数料率リスト取得APIレスポンスモデル
    /// </summary>
    public class InfoGetFeeRateListResponseModel
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        public FRResponseHeaderModel header { get; set; }

        /// <summary>
        /// ボディ
        /// </summary>
        public InfoGetFeeRateListResponseBodyModel body { get; set; }
    }

    /// <summary>
    /// 手数料率リスト取得APIレスポンスボディモデル
    /// </summary>
    public class InfoGetFeeRateListResponseBodyModel
    {
        /// <summary>
        /// 手数料率リスト
        /// </summary>
        public List<InfoGetFeeRateListRowModel> feeRateList { get; set; }
    }

    /// <summary>
    /// 手数料率リストモデル
    /// </summary>
    public class InfoGetFeeRateListRowModel
    {
        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 部署CD
        /// </summary>
        public int? departmentCD { get; set; }

        /// <summary>
        /// 支払方法CD
        /// </summary>
        public int? paymentTypeCD { get; set; }

        /// <summary>
        /// 開始日
        /// </summary>
        public DateTime? startDate { get; set; }

        /// <summary>
        /// 店舗ID
        /// </summary>
        public string shopID { get; set; }

        /// <summary>
        /// 手数料率
        /// </summary>
        public decimal? feeRate { get; set; }

        /// <summary>
        /// 作成日
        /// </summary>
        public DateTime? createDate { get; set; }

        /// <summary>
        /// 修正日
        /// </summary>
        public DateTime? lastmodify { get; set; }

        #endregion
    }

    #endregion

    #region カード削除

    /// <summary>
    /// カード削除APIリクエストモデル
    /// </summary>
    public class InfoDeleteCardRequestModel
    {
        #region プロパティ

        /// <summary>
        /// 企業ＣＤ
        /// </summary>
        public int? companyCD { get; set; }

        /// <summary>
        /// 得意先CD
        /// </summary>
        public int? customerCD { get; set; }

        /// <summary>
        /// 得意先名
        /// </summary>
        public string customerName { get; set; }

        #endregion
    }

    /// <summary>
    /// カード削除APIレスポンスモデル
    /// </summary>
    public class InfoDeleteCardResponseModel
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        public FRResponseHeaderModel header { get; set; }

        /// <summary>
        /// ボディ
        /// </summary>
        public InfoDeleteCardResponseBodyModel body { get; set; }
    }

    /// <summary>
    /// カード削除APIレスポンスボディモデル
    /// </summary>
    public class InfoDeleteCardResponseBodyModel
    {
        #region プロパティ

        /// <summary>
        /// 処理結果
        /// </summary>
        public int? processResult { get; set; }

        /// <summary>
        /// 処理結果内容
        /// </summary>
        public string processResultText { get; set; }

        #endregion
    }

    #endregion

    #endregion

    /// <summary>
    /// FRクレカ決済用中間サーバAPI群
    /// </summary>
    public class FRSettlement : WebAPIBase
    {
        //**************************************
        //* ルーティング定数
        //**************************************
        #region ルーティング定数

        /// <summary>
        /// 売上API
        /// </summary>
        private const string URL_SALE_SETTLEMENT = "sale/settlement";

        /// <summary>
        /// 売上取消API
        /// </summary>
        private const string URL_SALE_CANCEL = "sale/cancel";

        /// <summary>
        /// 決済履歴取得API
        /// </summary>
        private const string URL_INFO_GETSETTLEMENT = "info/getSettlement";

        /// <summary>
        /// 手数料率リスト取得API
        /// </summary>
        private const string URL_INFO_GETFEERATELIST = "info/getFeeRateList";

        /// <summary>
        /// カード削除API
        /// </summary>
        private const string URL_INFO_DELETECARD = "info/deleteCard";

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// WebAPIコントローラーを取得します
        /// </summary>
        /// <returns>WebAPIコントローラー</returns>
        protected override string GetController()
        {
            // 未使用
            return null;
        }

        /// <summary>
        /// 売上処理API呼出
        /// </summary>
        /// <param name="seikyuGokei">請求合計</param>
        /// <param name="frHistory">クレカ決済履歴（レスポンス内容を格納する）</param>
        /// <param name="message">処理結果メッセージ</param>
        /// <returns>成功した場合はtrue。それ以外はfalse。</returns>
        internal bool SaleSettlement(SeikyuGokei seikyuGokei, out FRSettlementHistory frHistory, out string message)
        {
            SaleSettlementRequestModel request = new SaleSettlementRequestModel
            {
                companyCD = TableBase._企業ＣＤ,
                billingNumber = seikyuGokei.請求番号,
                billingDate = seikyuGokei.請求日,
                departmentCD = seikyuGokei.部署ＣＤ,
                billingCustomerCD = seikyuGokei.請求先ＣＤ,
                billingCustomerName = seikyuGokei.請求先名,
                billingAmount = seikyuGokei.今回請求額
            };

            // JSON化
            string jsonString = SerializeJson<SaleSettlementRequestModel>(request);

            // API呼び出し
            string retJson = CallMainFR(URL_SALE_SETTLEMENT, jsonString);

            // レスポンス取得
            SaleSettlementResponseModel response = DeserializeJson<SaleSettlementResponseModel>(retJson);

            // 結果を返却
            frHistory = new FRSettlementHistory();

            if (response.header.result)
            {
                frHistory.店舗ID = response.body.shopID;
                frHistory.伝票番号 = response.body.slipNo;
                frHistory.処理タイプ = response.body.processType;
                frHistory.処理結果 = response.body.processResult;
                frHistory.処理結果内容 = response.body.processResultText;
                frHistory.承認番号 = response.body.authCode;
                frHistory.取引番号 = response.body.seqNo;
                frHistory.クレカ顧客番号 = response.body.userID;

                message = frHistory.処理結果内容;
            }
            else
            {
                message = response.header.message;
            }

            return response.header.result;
        }

        /// <summary>
        /// 売上取消処理API呼出
        /// </summary>
        /// <param name="seikyuGokei">請求合計</param>
        /// <param name="frHistory">クレカ決済履歴（リクエスト時に取引番号を参照し、レスポンス内容を格納する）</param>
        /// <param name="message">処理結果メッセージ</param>
        /// <returns>成功した場合はtrue。それ以外はfalse。</returns>
        internal bool SaleCancel(SeikyuGokei seikyuGokei, ref FRSettlementHistory frHistory, out string message)
        {
            SaleCancelRequestModel request = new SaleCancelRequestModel
            {
                companyCD = TableBase._企業ＣＤ,
                billingNumber = seikyuGokei.請求番号,
                billingDate = seikyuGokei.請求日,
                departmentCD = seikyuGokei.部署ＣＤ,
                billingCustomerCD = seikyuGokei.請求先ＣＤ,
                billingCustomerName = seikyuGokei.請求先名,
                seqNo = frHistory.取引番号
            };

            // JSON化
            string jsonString = SerializeJson<SaleCancelRequestModel>(request);

            // API呼び出し
            string retJson = CallMainFR(URL_SALE_CANCEL, jsonString);

            // レスポンス取得
            SaleCancelResponseModel response = DeserializeJson<SaleCancelResponseModel>(retJson);

            // 結果を返却
            //frHistory = new FRSettlementHistory();    決済時の履歴を保持してレスポンス内容のみ上書きするので、ここでは新規作成しない

            if (response.header.result)
            {
                frHistory.店舗ID = response.body.shopID;
                frHistory.処理タイプ = response.body.processType;
                frHistory.処理結果 = response.body.processResult;
                frHistory.処理結果内容 = response.body.processResultText;
                frHistory.取引番号 = response.body.cancelSeqNo;

                message = frHistory.処理結果内容;
            }
            else
            {
                message = response.header.message;
            }

            return response.header.result;
        }

        /// <summary>
        /// 決済履歴取得処理API呼出
        /// </summary>
        /// <param name="seikyuGokei">請求合計</param>
        /// <param name="frHistory">クレカ決済履歴（レスポンス内容を格納する）</param>
        /// <param name="message">処理結果メッセージ</param>
        /// <returns>成功した場合はtrue。それ以外はfalse。</returns>
        internal bool InfoGetSettlement(SeikyuGokei seikyuGokei, out FRSettlementHistory frHistory, out string message)
        {
            InfoGetSettlementRequestModel request = new InfoGetSettlementRequestModel
            {
                companyCD = TableBase._企業ＣＤ,
                billingDate = seikyuGokei.請求日,
                billingCustomerCD = seikyuGokei.請求先ＣＤ,
            };

            // JSON化
            string jsonString = SerializeJson<InfoGetSettlementRequestModel>(request);

            // API呼び出し
            string retJson = CallMainFR(URL_INFO_GETSETTLEMENT, jsonString);

            // レスポンス取得
            InfoGetSettlementResponseModel response = DeserializeJson<InfoGetSettlementResponseModel>(retJson);

            // 結果を返却
            frHistory = new FRSettlementHistory();

            if (response.header.result && response.body.settlementHistory != null)
            {
                frHistory.履歴番号 = response.body.settlementHistory.historyNo;
                frHistory.店舗ID = response.body.settlementHistory.shopID;
                frHistory.企業ＣＤ = response.body.settlementHistory.companyCD;
                frHistory.請求番号 = response.body.settlementHistory.billingNumber;
                frHistory.請求日 = response.body.settlementHistory.billingDate;
                frHistory.部署ＣＤ = response.body.settlementHistory.departmentCD;
                frHistory.請求先ＣＤ = response.body.settlementHistory.billingCustomerCD;
                frHistory.請求先名 = response.body.settlementHistory.billingCustomerName;
                frHistory.今回請求額 = response.body.settlementHistory.billingAmount;
                frHistory.処理タイプ = response.body.settlementHistory.processType;
                frHistory.処理結果 = response.body.settlementHistory.processResult;
                frHistory.処理結果内容 = response.body.settlementHistory.processResultText;
                frHistory.伝票番号 = response.body.settlementHistory.slipNo;
                frHistory.承認番号 = response.body.settlementHistory.authCode;
                frHistory.取引番号 = response.body.settlementHistory.seqNo;
                frHistory.クレカ顧客番号 = response.body.settlementHistory.userID;
                frHistory.作成日 = response.body.settlementHistory.createDate;
                frHistory.修正日 = response.body.settlementHistory.lastmodify;

                message = "";
            }
            else
            {
                message = response.header.message;
            }

            return response.header.result;
        }

        /// <summary>
        /// 手数料率リスト取得処理API呼出
        /// </summary>
        /// <param name="seikyuGokei">請求合計</param>
        /// <param name="frHistory">クレカ決済履歴（レスポンス内容を格納する）</param>
        /// <param name="message">処理結果メッセージ</param>
        /// <returns>成功した場合はtrue。それ以外はfalse。</returns>
        internal bool InfoGetFeeRateList(int deptCD, int paymentTypeCD, out List<FRFeeRate> feeRateList, out string message)
        {
            InfoGetFeeRateListRequestModel request = new InfoGetFeeRateListRequestModel
            {
                companyCD = TableBase._企業ＣＤ,
                departmentCD = deptCD,
                paymentTypeCD = paymentTypeCD
            };

            // JSON化
            string jsonString = SerializeJson<InfoGetFeeRateListRequestModel>(request);

            // API呼び出し
            string retJson = CallMainFR(URL_INFO_GETFEERATELIST, jsonString);

            // レスポンス取得
            InfoGetFeeRateListResponseModel response = DeserializeJson<InfoGetFeeRateListResponseModel>(retJson);

            // 結果を返却
            feeRateList = new List<FRFeeRate>();

            if (response.body.feeRateList != null)
            {
                foreach (InfoGetFeeRateListRowModel row in response.body.feeRateList)
                {
                    feeRateList.Add(new FRFeeRate
                    {
                        企業ＣＤ = row.companyCD,
                        部署ＣＤ = row.departmentCD,
                        支払方法CD = row.paymentTypeCD,
                        開始日 = row.startDate,
                        店舗ID = row.shopID,
                        手数料率 = row.feeRate,
                        作成日 = row.createDate,
                        修正日 = row.lastmodify
                    });
                }
            }

            message = response.header.message;

            return response.header.result;
        }

        /// <summary>
        /// カード削除処理API呼出
        /// </summary>
        /// <param name="tokuisaki">得意先マスタ</param>
        /// <param name="message">処理結果メッセージ</param>
        /// <returns>成功した場合はtrue。それ以外はfalse。</returns>
        internal bool InfoDeleteCard(Tokuisaki tokuisaki, out string message)
        {
            InfoDeleteCardRequestModel request = new InfoDeleteCardRequestModel
            {
                companyCD = TableBase._企業ＣＤ,
                customerCD = tokuisaki.得意先ＣＤ,
                customerName = tokuisaki.得意先名
            };

            // JSON化
            string jsonString = SerializeJson<InfoDeleteCardRequestModel>(request);

            // API呼び出し
            string retJson = CallMainFR(URL_INFO_DELETECARD, jsonString);

            // レスポンス取得
            InfoDeleteCardResponseModel response = DeserializeJson<InfoDeleteCardResponseModel>(retJson);

            // 結果を返却
            if (response.header.result)
            {
                message = response.body.processResultText;
            }
            else
            {
                message = response.header.message;
            }

            return response.header.result;
        }

        #endregion
    }
}

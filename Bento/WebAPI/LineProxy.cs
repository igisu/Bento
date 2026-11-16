using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento.WebAPI
{
    /// <summary>
    /// LINE決済用中間サーバAPI群
    /// </summary>
    public class LineProxy : WebAPIBase
    {
        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// 配達完了受入APIリクエスト得意先モデル
        /// </summary>
        class SetArrivedRequestCustomerModel
        {
            //**************************************
            //* プロパティ
            //**************************************
            #region プロパティ
            /// <summary>
            /// 得意先ＣＤ
            /// </summary>
            public int? customer_cd { get; set; }
            #endregion
        }

        /// <summary>
        /// 配達完了受入APIリクエストモデル
        /// </summary>
        class SetArrivedRequestModel
        {
            //**************************************
            //* プロパティ
            //**************************************
            #region プロパティ
            /// <summary>
            /// 企業ＣＤ（エラーチェックのみに使用する）
            /// </summary>
            public int? company_cd { get; set; }

            /// <summary>
            /// 得意先ＣＤのリスト
            /// </summary>
            public List<SetArrivedRequestCustomerModel> customer_list { get; set; }

            /// <summary>
            /// 部署CD
            /// </summary>
            public int? division_cd { get; set; }

            /// <summary>
            /// 注文日
            /// </summary>
            public string order_date
            {
                get
                {
                    return string.Format("{0:yyyy-MM-dd}", order_date_value);
                }
                set
                {
                    DateTime d;
                    if (DateTime.TryParse(value, out d))
                    {
                        order_date_value = d;
                    }
                }
            }

            /// <summary>
            /// 注文日（JSON変換用）
            /// </summary>
            [JsonIgnore]
            public DateTime? order_date_value { get; set; }
            #endregion
        }

        /// <summary>
        /// 配達完了受入APIレスポンスモデル
        /// </summary>
        class SetArrivedResponseModel
        {
            /// <summary>
            /// ヘッダー
            /// </summary>
            public ResponseHeaderModel header { get; set; }
        }

        /// <summary>
        /// 注文金額取得APIリクエストモデル
        /// </summary>
        class GetEarningsRequestModel
        {
            //**************************************
            //* プロパティ
            //**************************************
            #region プロパティ
            /// <summary>
            /// 企業ＣＤ
            /// </summary>
            public int? company_cd { get; set; }

            /// <summary>
            /// 注文日From
            /// </summary>
            public string date_from
            {
                get
                {
                    return string.Format("{0:yyyy-MM-dd}", date_from_value);
                }
                set
                {
                    DateTime d;
                    if (DateTime.TryParse(value, out d))
                    {
                        date_from_value = d;
                    }
                }
            }

            /// <summary>
            /// 注文日From（JSON変換用）
            /// </summary>
            [JsonIgnore]
            public DateTime? date_from_value { get; set; }

            /// <summary>
            /// 注文日To
            /// </summary>
            public string date_to
            {
                get
                {
                    return string.Format("{0:yyyy-MM-dd}", date_to_value);
                }
                set
                {
                    DateTime d;
                    if (DateTime.TryParse(value, out d))
                    {
                        date_to_value = d;
                    }
                }
            }

            /// <summary>
            /// 注文日To（JSON変換用）
            /// </summary>
            [JsonIgnore]
            public DateTime? date_to_value { get; set; }
            #endregion
        }

        /// <summary>
        /// 注文金額取得APIレスポンス得意先モデル
        /// </summary>
        internal class GetEarningsResponseCustomerModel
        {
            //**************************************
            //* プロパティ
            //**************************************
            #region プロパティ
            /// <summary>
            /// 得意先ＣＤ
            /// </summary>
            public int customer_cd { get; set; }

            /// <summary>
            /// 注文金額合計
            /// </summary>
            public decimal amount { get; set; }
            #endregion
        }

        /// <summary>
        /// 注文金額取得APIレスポンスボディモデル
        /// </summary>
        class GetEarningsResponseBodyModel
        {
            //**************************************
            //* プロパティ
            //**************************************
            #region プロパティ
            /// <summary>
            /// 得意先リスト
            /// </summary>
            public List<GetEarningsResponseCustomerModel> customer_list { get; set; }
            #endregion
        }

        /// <summary>
        /// 注文金額取得APIレスポンスモデル
        /// </summary>
        class GetEarningsResponseModel
        {
            /// <summary>
            /// ヘッダー
            /// </summary>
            public ResponseHeaderModel header { get; set; }

            /// <summary>
            /// レスポンスボディ
            /// </summary>
            public GetEarningsResponseBodyModel body { get; set; }
        }
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
            string controller = SystemMaster.GetWebAPIController();
            if (string.IsNullOrEmpty(controller))
            {
                return null;
            }
            if (controller.EndsWith("/") == false)
            {
                controller += "/";
            }
            return controller;
        }

        /// <summary>
        /// 配達完了受入API呼び出し
        /// </summary>
        /// <param name="busyoCD">部署CD</param>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="tokuisakiCDList">得意先CDのリスト</param>
        /// <returns>成功した場合はtrue。それ以外はfalse。</returns>
        internal bool SetArrived(int busyoCD, DateTime kissyokubi, List<int> tokuisakiCDList)
        {
            log.Debug("");

            // 配達完了受入APIリクエストモデルを生成
            SetArrivedRequestModel request = new SetArrivedRequestModel();
            request.company_cd = TableBase._企業ＣＤ;
            request.division_cd = busyoCD;
            request.order_date_value = kissyokubi;
            request.customer_list = new List<SetArrivedRequestCustomerModel>();

            // 得意先モデルを生成し、リクエストモデルに追加
            foreach (int tokuisakiCD in tokuisakiCDList)
            {
                SetArrivedRequestCustomerModel customerModel = new SetArrivedRequestCustomerModel();
                customerModel.customer_cd = tokuisakiCD;
                request.customer_list.Add(customerModel);
            }

            // JSON化
            string jsonString = SerializeJson<SetArrivedRequestModel>(request);

            // API呼び出し
            string retJson = CallMain("set_arrived", jsonString);

            // レスポンス取得
            SetArrivedResponseModel response = DeserializeJson<SetArrivedResponseModel>(retJson);

            // 結果を返却
            return (response.header.result == (int)ResponseHeaderModel.ResultCode.OK);
        }

        /// <summary>
        /// 注文金額取得API呼び出し
        /// </summary>
        /// <param name="fromDate">取得開始日</param>
        /// <param name="toDate">取得終了日</param>
        /// <returns>得意先CDと注文金額合計のリスト</returns>
        internal List<GetEarningsResponseCustomerModel> GetEarnings(DateTime? fromDate, DateTime? toDate)
        {
            log.Debug("");

            // 注文金額取得APIリクエストモデルを生成
            GetEarningsRequestModel request = new GetEarningsRequestModel();
            request.company_cd = TableBase._企業ＣＤ;
            request.date_from_value = fromDate;
            request.date_to_value = toDate;

            // JSON化
            string jsonString = SerializeJson<GetEarningsRequestModel>(request);

            // API呼び出し
            string retJson = CallMain("get_earnings", jsonString);

            // レスポンス取得
            GetEarningsResponseModel response = DeserializeJson<GetEarningsResponseModel>(retJson);

            if (response.header.result != (int)ResponseHeaderModel.ResultCode.OK)
            {
                log.Warn(response.header.message);
            }

            // 結果を返却
            return response.body.customer_list;
        }
        #endregion
    }
}

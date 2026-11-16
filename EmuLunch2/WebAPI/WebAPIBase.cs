using Emu.Bento.Common;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Emu.Bento.WebAPI
{
    /// <summary>
    /// WebAPI基底クラス
    /// </summary>
    public abstract class WebAPIBase
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// WebAPIURL
        /// </summary>
        protected static string _url = GetURL(1);

        /// <summary>
        /// WebAPIURL（FRクレカ決済）
        /// </summary>
        protected static string _urlFR = GetURL(2);
        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// WebAPIURLを取得します
        /// </summary>
        /// <returns>電子決済WebAPIURL</returns>
        private static string GetURL(int apiType)
        {
            string url = SystemMaster.GetWebAPIURL(apiType);
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            if (url.EndsWith("/") == false)
            {
                url += "/";
            }
            return url;
        }
        #endregion

        //**************************************
        //* 抽象メソッド
        //**************************************
        #region 抽象メソッド
        /// <summary>
        /// WebAPIコントローラーを取得します
        /// </summary>
        /// <returns>WebAPIコントローラー</returns>
        protected abstract string GetController();
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// WebAPIが呼び出し可能かどうかを返します
        /// </summary>
        /// <returns>呼び出し可能な場合はtrue。それ以外はfalse。</returns>
        /// <remarks>
        /// CallMain（を呼ぶメソッド）を呼ぶ前にこのメソッドを呼んで、WebAPIをホントに実行してよいかどうかを判断してください。
        /// </remarks>
        public bool CanCall()
        {
            if (string.IsNullOrEmpty(_url))
            {
                return false;
            }
            string controller = GetController();
            if (string.IsNullOrEmpty(controller))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// JSON文字列からデシリアライズしたオブジェクトを返します
        /// </summary>
        /// <typeparam name="T">デシリアライズする型</typeparam>
        /// <param name="json">JSON文字列</param>
        /// <returns>JSONからデシリアライズされたオブジェクト</returns>
        protected T DeserializeJson<T>(string jsonString)
        {
            T jsonObject;

            jsonString = jsonString.Replace("\"\"", "null");
            jsonObject = JsonConvert.DeserializeObject<T>(jsonString);

            // 取得した文字列はフォーマットされていないので、ログ出力用に(フォーマットして)シリアライズする
            jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            log.Debug(jsonString);

            return jsonObject;
        }

        /// <summary>
        /// オブジェクトからシリアライズしたJson文字列を返します
        /// </summary>
        /// <param name="jsonObject">Json文字列に変換するオブジェクト</param>
        /// <returns>Json文字列</returns>
        protected string SerializeJson<T>(T jsonObject)
        {
            string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            log.Debug(jsonString);
            return jsonString;
        }

        /// <summary>
        /// 電子決済WebAPIを呼び出します。
        /// </summary>
        /// <param name="actionName">電子決済WebAPIのアクション名</param>
        /// <param name="jsonString">送信するJSON文字列</param>
        /// <returns>返却されたJSON文字列</returns>
        protected string CallMain(string actionName, string jsonString)
        {
            string controller = GetController();
            string url = _url + controller + actionName;

            string retJson = "";

            try
            {
                if (WebAccess.GetStringByPost(url, jsonString, out retJson) == false)
                {
                    log.ErrorFormat("WebAPI({0})の呼び出しに失敗しました。", actionName);
                    throw new Exception(string.Format("WebAPI({0})の呼び出しに失敗しました。", actionName));
                }

                if (string.IsNullOrEmpty(retJson))
                {
                    log.ErrorFormat("WebAPI({0})の呼び出しの応答がありません。", actionName);
                    throw new Exception(string.Format("WebAPI({0})の呼び出しの応答がありません。", actionName));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }

            return retJson;
        }

        /// <summary>
        /// クレカ決済WebAPIを呼び出します。
        /// </summary>
        /// <param name="actionName">電子決済WebAPIのアクション名</param>
        /// <param name="jsonString">送信するJSON文字列</param>
        /// <returns>返却されたJSON文字列</returns>
        /// <exception cref="EmuException"></exception>
        protected string CallMainFR(string actionName, string jsonString)
        {
            string url = _urlFR + actionName;

            string retJson = "";

            // App.configからリクエストヘッダに付与する情報を取得
            Dictionary<string, string> requestHeaderMap = new Dictionary<string, string>
            {
                { "api-key", System.Configuration.ConfigurationManager.AppSettings["FRApiKey"] },
                { "company-cd", System.Configuration.ConfigurationManager.AppSettings["CompanyCD"] },
            };

            try
            {
                if (WebAccess.GetStringByPost(url, requestHeaderMap, jsonString, out retJson) == false)
                {
                    log.ErrorFormat("WebAPI({0})の呼び出しに失敗しました。", actionName);
                    throw new Exception(string.Format("WebAPI({0})の呼び出しに失敗しました。", actionName));
                }

                if (string.IsNullOrEmpty(retJson))
                {
                    log.ErrorFormat("WebAPI({0})の呼び出しの応答がありません。", actionName);
                    throw new Exception(string.Format("WebAPI({0})の呼び出しの応答がありません。", actionName));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new EmuException(ex);
            }

            return retJson;
        }
        #endregion
    }
}

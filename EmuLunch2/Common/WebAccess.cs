using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;

namespace Emu.Bento.Common
{
    /// <summary>
    /// Webへのアクセス（主にWebからのダウンロード）をラップしたクラス
    /// </summary>
    public static class WebAccess
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="paramMap">パラメータ（キーと値）のマップ</param>
        /// <param name="ret">URLからの戻り値</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        /// <remarks>タイムアウト時間はデフォルト(100秒)</remarks>
        public static bool GetStringByPost(string url, Dictionary<string, string> paramMap, out string ret)
        {
            return GetStringByPostMain(url, null, paramMap, out ret);
        }

        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="paramMap">パラメータ（キーと値）のマップ</param>
        /// <param name="timeoutSec">タイムアウト時間(秒)</param>
        /// <param name="ret">URLからの戻り値</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        public static bool GetStringByPost(string url, int timeoutSec, Dictionary<string, string> paramMap, out string ret)
        {
            return GetStringByPostMain(url, timeoutSec, paramMap, out ret);
        }

        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="paramMap">パラメータ（キーと値）のマップ</param>
        /// <param name="timeoutSec">タイムアウト時間(秒)</param>
        /// <param name="ret">URLからの戻り値</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        private static bool GetStringByPostMain(string url, int? timeoutSec, Dictionary<string, string> paramMap, out string ret)
        {
            // POST送信する文字列を作成
            StringBuilder data = new StringBuilder();
            string amp = "";
            foreach (string key in paramMap.Keys)
            {
                data.AppendFormat("{0}{1}={2}", amp, key, paramMap[key]);
                amp = "&";
            }

            return GetStringByPostMain(url, timeoutSec, null, data.ToString(), out ret);
        }

        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="paramMap">パラメータ（キーと値）のマップ</param>
        /// <param name="jsonContent">JSON文字列</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        /// <remarks>タイムアウト時間はデフォルト(100秒)</remarks>
        public static bool GetStringByPost(string url, string jsonContent, out string ret)
        {
            return GetStringByPostMain(url, null, null, jsonContent, out ret);
        }

        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="jsonContent">JSON文字列</param>
        /// <param name="timeoutSec">タイムアウト時間(秒)</param>
        /// <param name="ret">URLからの戻り値</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        public static bool GetStringByPost(string url, int timeoutSec, string jsonContent, out string ret)
        {
            return GetStringByPostMain(url, timeoutSec, null, jsonContent, out ret);
        }

        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="requestHeaderMap">リクエストヘッダ（キーと値）のマップ</param>
        /// <param name="jsonContent">JSON文字列</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        /// <remarks>タイムアウト時間はデフォルト(100秒)</remarks>
        public static bool GetStringByPost(string url, Dictionary<string, string> requestHeaderMap, string jsonContent, out string ret)
        {
            return GetStringByPostMain(url, null, requestHeaderMap, jsonContent, out ret);
        }

        /// <summary>
        /// 指定したURLにパラメータをPOSTし、結果の文字列を返します
        /// </summary>
        /// <param name="url">POST送信先URL</param>
        /// <param name="content">送信文字列</param>
        /// <param name="timeoutSec">タイムアウト時間(秒)</param>
        /// <param name="requestHeaderMap">リクエストヘッダに付与するキーと値のマップ</param>
        /// <param name="ret">URLからの戻り値</param>
        /// <returns>URLから正常に結果を得られた場合はtrue。それ以外（タイムアウトなど）はfalse</returns>
        private static bool GetStringByPostMain(string url, int? timeoutSec, Dictionary<string, string> requestHeaderMap, string content, out string ret)
        {
            ret = "";

            WebClientEx wc = new WebClientEx();

            if (timeoutSec != null)
            {
                wc.Timeout = ((int)timeoutSec) * 1000;
            }
            // 文字コードを指定する
            wc.Encoding = System.Text.Encoding.UTF8;
            // ヘッダにContent-Typeを加える
            //wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            wc.Headers.Add("Content-Type", "application/json");

            // ヘッダに付与する情報（APIキーなど）があれば加える
            if (requestHeaderMap != null)
            {
                foreach (KeyValuePair<string, string> pair in requestHeaderMap)
                {
                    wc.Headers.Add(pair.Key, pair.Value);
                }
            }

            log.Debug("POST要求送信");
            // POST送信
            ret = wc.UploadString(url, content);
            log.Debug("POST結果受信");

            return true;
        }
        #endregion
    }
}

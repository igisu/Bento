using System;
using System.Net;

namespace Emu.Bento.Common
{
    /// <summary>
    /// タイムアウトを指定可能にしたWebClient
    /// </summary>
    class WebClientEx : WebClient
    {
        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// タイムアウト時間（ミリ秒）
        /// </summary>
        public int Timeout { get; set; }

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebClientEx()
        {
            // 指定なしなら100秒(たぶんデフォルト)；
            this.Timeout = 100 * 1000;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="timeout">タイムアウト時間（ミリ秒）</param>
        public WebClientEx(int timeout)
        {
            this.Timeout = timeout;
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定したリソースの WebRequest オブジェクトを返します。
        /// </summary>
        /// <param name="uri">要求するリソースを識別する Uri。</param>
        /// <returns>指定したリソースの新しい WebRequest オブジェクト。</returns>
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);

            // タイムアウト時間を指定する
            if (this.Timeout <= 0)
            {
                webRequest.Timeout = -1;    // タイムアウトしない
            }
            else
            {
                webRequest.Timeout = this.Timeout;
            }

            return webRequest;
        }
        #endregion
    }
}

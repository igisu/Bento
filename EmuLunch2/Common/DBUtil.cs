using System;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;

namespace Emu.Bento.Common
{
    /// <summary>
    /// DB関連共通クラス
    /// </summary>
    public static class DBUtil
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数

        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 接続文字列
        /// </summary>
        private static string _connectionString = null;

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 接続文字列
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    // Webサービスを呼び出して、接続情報を取得する
                    string connectionString = ConnectionInformation.Get();

                    //// 接続情報が取得できた場合は、Properties.Settingsに暗号化して保存する
                    //if (string.IsNullOrEmpty(connectionString) == false)
                    //{
                    //    ConnectionInformation.Save(connectionString);
                    //}

                    //// Properties.Settingsに保存されている暗号化済みの接続情報を復号して取得する
                    //connectionString = ConnectionInformation.Load();

                    // 取得できない場合は例外
                    if (string.IsNullOrEmpty(connectionString))
                    {
                        Exception ex = new Exception("接続情報が保存されていません");
                        log.Error(ex);
                        throw new EmuException(ex);
                    }
                    
                    _connectionString = connectionString;
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// 検索文字列をエスケープします
        /// </summary>
        /// <param name="target">エスケープ対象文字</param>
        /// <returns>エスケープ結果文字列</returns>
        public static string EscapeString(string target)
        {
            return "%" + Regex.Replace(target, "[_%\\[#]", "#$0") + "%";
        }
        #endregion
    }
}

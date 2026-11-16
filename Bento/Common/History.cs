using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento.Common
{
    /// <summary>
    /// ヒストリー保存
    /// </summary>
    public static class History
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値
        /// <summary>
        /// 処理タイプ
        /// </summary>
        public enum ProcessType
        {
            Start,      // 開始
            Create,     // 作成
            Update,     // 更新
            Delete,     // 削除
            Output      // 出力
        }

        /// <summary>
        /// 処理タイプの名称を返します。
        /// </summary>
        /// <param name="processType"></param>
        /// <returns></returns>
        public static string DisplayName(this ProcessType processType)
        {
            string[] names = { "開始", "作成", "更新", "削除", "出力" };
            return names[(int)processType];
        }
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// IPアドレス
        /// </summary>
        private static string _IPAddress = GetIPAddress();
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// IPv4アドレス
        /// </summary>
        public static string IPAddress
        {
            get
            {
                return _IPAddress;
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// ログイン履歴保存
        /// </summary>
        public static void Login()
        {
            UserData userData = UserData.getInstance();
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                string operationContent = string.Format("{0}さんがログインしました。(Version:{1})", userData.担当者名, Application.ProductVersion);
                SaveMain(con, null, userData, operationContent, false);
            }
        }

        /// <summary>
        /// ログアウト履歴保存
        /// </summary>
        public static void Logout()
        {
            UserData userData = UserData.getInstance();
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                string operationContent = string.Format("{0}さんがプログラムを終了しました。", userData.担当者名);
                SaveMain(con, null, userData, operationContent, false);
            }
        }

        /// <summary>
        /// 履歴保存
        /// </summary>
        /// <param name="title">機能名（フォームのタイトル）</param>
        /// <param name="content">データ名</param>
        /// <param name="code">コード（括弧で囲まれる）</param>
        /// <param name="process">処理タイプ(作成/更新/削除)</param>
        public static void Save(string title, string content, string code, ProcessType process)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                Save(con, title, content, code, process);
            }
        }

        /// <summary>
        /// 履歴保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="title">機能名（フォームのタイトル）</param>
        /// <param name="content">データ名</param>
        /// <param name="code">コード（括弧で囲まれる）</param>
        /// <param name="process">処理タイプ(作成/更新/削除)</param>
        public static void Save(SqlConnection con, string title, string content, string code, ProcessType process)
        {
            Save(con, null, title, content, code, process);
        }

        /// <summary>
        /// 履歴保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="title">機能名（フォームのタイトル）</param>
        /// <param name="content">データ名</param>
        /// <param name="code">コード（括弧で囲まれる）</param>
        /// <param name="process">処理タイプ(作成/更新/削除)</param>
        public static void Save(SqlConnection con, SqlTransaction tran, string title, string content, string code, ProcessType process)
        {
            UserData userData = UserData.getInstance();

            if (string.IsNullOrWhiteSpace(code) == false)
            {
                code = "（" + code + "）";
            }
            string operationContent = string.Format("{0}さんが{1}で{2}{3}を{4}しました。", userData.担当者名, title, content, code, process.DisplayName());

            SaveMain(con, tran, userData, operationContent, false);
        }

        /// <summary>
        /// 履歴保存
        /// </summary>
        /// <param name="content">出力内容</param>
        public static void Save(string content)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                Save(con, content);
            }
        }

        /// <summary>
        /// 履歴保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="content">出力内容</param>
        public static void Save(SqlConnection con, string content)
        {
            Save(con, null, content);
        }

        /// <summary>
        /// 履歴保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="content">出力内容</param>
        public static void Save(SqlConnection con, SqlTransaction tran, string content)
        {
            UserData userData = UserData.getInstance();

            string operationContent = string.Format("{0}さんが{1}", userData.担当者名, content);

            SaveMain(con, tran, userData, operationContent, false);
        }

        /// <summary>
        /// 例外保存
        /// </summary>
        /// <param name="ex">例外</param>
        public static void Error(Exception ex)
        {
            // 発生したファイルと行番号を取得すべく、StackTraceとStackFrameを取得
            StackTrace st = new StackTrace(ex, true);
            StackFrame sf = st.GetFrames().Where(x => string.IsNullOrEmpty(x.GetFileName()) == false).FirstOrDefault();
            string location = "";
            if (st != null)
            {
                location = string.Format("（{0}:{1}）", Path.GetFileNameWithoutExtension(sf.GetFileName()), sf.GetFileLineNumber());
            }

            UserData userData = UserData.getInstance();
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SaveMain(con, null, userData, ex.GetType().ToString() + location, true);     // 例外の名前と発生場所を出力
            }
        }

        /// <summary>
        /// 履歴保存メイン
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="userData">ログインユーザーデータ</param>
        /// <param name="operationContent">操作内容</param>
        private static void SaveMain(SqlConnection con, SqlTransaction tran, UserData userData, string operationContent, bool errorFlg)
        {
            try
            {
                string sql = "INSERT INTO システム操作履歴(" +
                             " 企業ＣＤ" +
                             " ,IPv4アドレス" +
                             " ,担当者ＣＤ" +
                             " ,操作日時" +
                             " ,操作内容" +
                             " ,エラーフラグ" +
                             ") VALUES (" +
                             " @企業ＣＤ" +
                             " ,@IPアドレス" +
                             " ,@担当者ＣＤ" +
                             " ,getdate()" +
                             " ,@操作内容" +
                             " ,@エラーフラグ" +
                             ")";

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@IPアドレス", SqlDbType.VarChar).Value = _IPAddress;
                cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@操作内容", SqlDbType.VarChar).Value = operationContent;
                cmd.Parameters.Add("@エラーフラグ", SqlDbType.Bit).Value = errorFlg;

                if (tran != null)
                {
                    cmd.Transaction = tran;
                }
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// IPアドレス取得
        /// </summary>
        /// <returns></returns>
        private static string GetIPAddress()
        {
            try
            {
                string ipAddress4 = "";
                string ipAddress6 = "";
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                foreach (IPAddress address in addresses)
                {
                    // IPv4のアドレスが見つかれば終了
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipAddress4 = address.ToString();
                        break;
                    }

                    // IPv4が見つからない場合に備えて、最初に見つかったIPv6のリンクローカルアドレスを取得
                    if (ipAddress6 == "")
                    {
                        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            if (address.IsIPv6LinkLocal)
                            {
                                ipAddress6 = address.ToString();
                            }
                        }
                    }
                }

                //return ipAddress4 != "" ? ipAddress4 : ipAddress6;        // IPv4が見つかればそれを、そうでなければIPv6を返す
                return ipAddress4;
            }
            catch (System.Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }
        #endregion
    }
}

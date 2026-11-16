using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Emu.Bento.Data;
using log4net;

namespace Emu.Bento.Common
{
    /// <summary>
    /// 同時ログイン数制御
    /// </summary>
    public static class LoginCountControler
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// MACアドレスの文字列長
        /// </summary>
        private const int MAC_ADDRESS_LENGTH = 12;
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// MACアドレスリスト
        /// </summary>
        private static SortedSet<string> _macAddressList = GetMACAddressList();
        #endregion

        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// ログイン可能かどうかを返す
        /// </summary>
        /// <returns>ログイン可能な場合は空文字列。それ以外はエラー文字列</returns>
        public static string CanLogin()
        {
            try
            {
                // システム設定マスタから「システム接続端末数」を取得
                // キーが存在しない、または、値が0以下の場合はログイン可能と見なす
                int licenseCount = SystemMaster.GetLicenseCount();
                if (licenseCount <= 0)
                {
                    return "";
                }

                // 「システム接続端末」テーブルから、削除用MACアドレスを持つレコードを全て削除
                DeleteData();

                // 「システム接続端末」テーブルの全レコード数（＝ログイン済み端末数）が、「システム接続端末数」に達していれば、ログイン不可を返す
                int loggedinCount = GetLoggedinCount();
                if (licenseCount <= loggedinCount)
                {
                    return "ログイン済みの端末が、購入済みライセンス数の上限に達しています。\n別の端末がログアウトするまで、これ以上のログインはできません。";
                }

                // 「システム接続端末」テーブルに、登録用MACアドレス（MACアドレスリストの先頭）を持つレコードを追加
                return InsertData(_macAddressList.Min, licenseCount);
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
        /// ログアウト処理
        /// </summary>
        public static void Logout()
        {
            try
            {
                // 「システム接続端末」テーブルから、削除用MACアドレスを持つレコードを全て削除
                DeleteData();
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
        /// MACアドレスのリストを返す
        /// </summary>
        /// <returns>MACアドレス(000000000000)のリスト</returns>
        private static SortedSet<string> GetMACAddressList()
        {
            try
            {
                SortedSet<string> macAddressList = new SortedSet<string>();

                // アダプタリストを取得する
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    // ネットワーク接続状態が UP のアダプタのみ対象
                    // …にするつもりだったが、これだと全てのアダプタを取得できない
                    //if (adapter.OperationalStatus != OperationalStatus.Up)
                    //{
                    //    continue;
                    //}

                    // MACアドレスを取得
                    PhysicalAddress pa = adapter.GetPhysicalAddress();
                    string macAddress = pa.ToString();

                    // 無効なMACアドレスは無視する
                    if (string.IsNullOrEmpty(macAddress) || macAddress.Length != MAC_ADDRESS_LENGTH)
                    {
                        continue;
                    }
                    macAddressList.Add(pa.ToString());
                }

                return macAddressList;
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
        /// 「システム接続端末」テーブルから、削除用MACアドレスを持つレコードを全て削除
        /// </summary>
        private static void DeleteData()
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 全てのMACアドレスをカンマで区切る
                    // 文字列なので前後を「'」で囲んでいる
                    string macAddrs = string.Format("'{0}'", string.Join("','", _macAddressList));

                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" DELETE FROM システム接続端末");
                    sql.AppendLine(" WHERE 企業ＣＤ = @企業ＣＤ");
                    sql.AppendFormat(" AND MACアドレス IN ({0})", macAddrs);

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    cmd.CommandText = sql.ToString();
                    log.Debug(cmd.CommandTextWithParameters());

                    cmd.ExecuteNonQuery();

                    tran.Commit();
                }
                catch (EmuException)
                {
                    tran.Rollback();
                    throw;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    log.Error(ex);
                    History.Error(ex);
                    throw new EmuException(ex);
                }
            }
        }

        /// <summary>
        /// 「システム接続端末」テーブルの全レコード数（＝ログイン済み端末数）を取得
        /// </summary>
        /// <returns>「システム接続端末」テーブルの全レコード数</returns>
        private static int GetLoggedinCount()
        {
            int cnt = 0;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT COUNT(*) AS 件数 FROM システム接続端末 WHERE 企業ＣＤ = @企業ＣＤ";
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        cnt = (int)reader["件数"];
                    }

                    return cnt;
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
        }

        /// <summary>
        /// 「システム接続端末」テーブルに、登録用MACアドレス（MACアドレスリストの先頭）を持つレコードを追加
        /// </summary>
        /// <param name="macAddr">登録用MACアドレス</param>
        /// <param name="licenseCount">ログイン上限数</param>
        /// <returns>成功した場合は True</returns>
        private static string InsertData(string macAddr, int licenseCount)
        {
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;

                    cmd.CommandText = "\r\n INSERT INTO システム接続端末(" +
                                      "\r\n     企業ＣＤ" +
                                      "\r\n     ,MACアドレス" +
                                      "\r\n     ,IPv4アドレス" +
                                      "\r\n     ,起動日時" +
                                      "\r\n     ,ログイン担当者ＣＤ" +
                                      "\r\n )SELECT" +
                                      "\r\n     @企業ＣＤ" +
                                      "\r\n     ,@MACアドレス" +
                                      "\r\n     ,@IPv4アドレス" +
                                      "\r\n     ,getdate()" +
                                      "\r\n     ,@ログイン担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     (SELECT COUNT(*) FROM システム接続端末 WHERE 企業ＣＤ = @企業ＣＤ) < @システム接続端末数";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@MACアドレス", SqlDbType.VarChar).Value = macAddr;
                    cmd.Parameters.Add("@IPv4アドレス", SqlDbType.VarChar).Value = History.IPAddress;
                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@ログイン担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@システム接続端末数", SqlDbType.Int).Value = licenseCount;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        return "ログイン済みの端末が、購入済みライセンス数の上限に達しました。\n別の端末がログアウトするまで、これ以上のログインはできません。";
                    }

                    tran.Commit();
                    return "";
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        return "MACアドレスが重複しています。";
                    }
                    else
                    {
                        History.Error(ex);
                        return string.Format("システム接続端末の登録に失敗しました。\n{0}", ex.ToString());
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    log.Error(ex);
                    History.Error(ex);
                    throw new EmuException(ex);
                }
            }
        }

        #endregion
    }
}

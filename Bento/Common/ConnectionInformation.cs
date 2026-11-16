using System.Data.SqlClient;
using Emu.Bento.Common.Encrypt;

namespace Emu.Bento.Common
{
    /// <summary>
    /// 接続情報クラス
    /// </summary>
    public static class ConnectionInformation
    {
        /// <summary>
        /// （configファイルに保存されている）企業CD
        /// </summary>
        public static int CompanyCD
        {
            get
            {
                // configファイルから企業CDを取得して復号する
                string encryptedCompanyCD = System.Configuration.ConfigurationManager.AppSettings["CompanyCD"];
                SecretData secretData = new SecretData();
                string decryptedCompanyCD = secretData.HashedDecrypt(encryptedCompanyCD);
                return int.Parse(decryptedCompanyCD);
            }
        }

        /// <summary>
        /// 企業CDをパラメータにしてWebサービスを呼び出して、接続情報を取得する
        /// </summary>
        /// <returns>接続文字列</returns>
        public static string Get()
        {
            // 企業CDを取得
            int companyCD = CompanyCD;

            // TODO:企業CDをパラメータにしてWebサービスを呼び出して、接続情報を取得する
            // 暫定的に取得したつもりにする
            // …つもりだったが、Webサービスから取得する方法採用せずに、configファイルから取得ことになった。
            //return "Data Source=EMU012SV;Initial Catalog=ebi;User ID=sa;Password=emu@12sv";

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = System.Configuration.ConfigurationManager.AppSettings["DataSource"];
            builder.InitialCatalog = System.Configuration.ConfigurationManager.AppSettings["InitialCatalog"];
            builder.UserID = System.Configuration.ConfigurationManager.AppSettings["UserID"];

            // 暗号化されているパスワードを復号
            string encryptedPassword = System.Configuration.ConfigurationManager.AppSettings["Password"];
            SecretData secretData = new SecretData();
            string decryptedPassword = secretData.HashedDecrypt(encryptedPassword);

            builder.Password = decryptedPassword;
            return builder.ConnectionString;
        }

        /// <summary>
        /// 取得した接続情報をProperties.Settingsに暗号化して保存する（接続情報が取得できた場合のみ実施する）
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        //public static void Save(string connectionString)
        //{
        //    SecretData secretData = new SecretData();
        //    string encryptedConnectionString = secretData.HashedEncrypt(connectionString);
        //    Properties.Settings.Default.ConnectionString = encryptedConnectionString;
        //    Properties.Settings.Default.Save();
        //}

        /// <summary>
        /// Properties.Settingsに保存されている暗号化済みの接続情報を復号して取得する
        /// </summary>
        /// <returns>接続文字列</returns>
        //public static string Load()
        //{
        //    string encryptedConnectionString = Properties.Settings.Default.ConnectionString;
        //    SecretData secretData = new SecretData();
        //    string decryptedConnectionString = secretData.HashedDecrypt(encryptedConnectionString);
        //    return decryptedConnectionString;
        //}
    }
}

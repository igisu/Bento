using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 金融機関名称マスタ
    /// </summary>
    public class Bank : IDataErrorInfo
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="bankCD"></param>
        /// <returns></returns>
        public static Bank GetBank(string bankCD)
        {
            Bank bank = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT " +
                                  "\r\n     銀行ＣＤ" +
                                  "\r\n     ,銀行名" +
                                  "\r\n     ,銀行名カナ " +
                                  "\r\n FROM " +
                                  "\r\n     金融機関名称 " +
                                  "\r\n WHERE " +
                                  "\r\n     銀行ＣＤ = @銀行ＣＤ ";

                cmd.Parameters.Add("@銀行ＣＤ", SqlDbType.VarChar).Value = bankCD;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                log.Debug(cmd.CommandTextWithParameters());

                DataTable dt = new DataTable("金融機関名称");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    bank = new Bank(dt.Rows[0]);
                }
            }

            return bank;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private string _銀行ＣＤ;
        private string _銀行名;
        private string _銀行名カナ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Bank()
        {
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Bank(DataRow dr)
        {
            銀行ＣＤ = dr["銀行ＣＤ"] as string;
            銀行名 = dr["銀行名"] as string;
            銀行名カナ = dr["銀行名カナ"] as string;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string 銀行ＣＤ
        {
            get { return _銀行ＣＤ; }
            set { _銀行ＣＤ = value; }
        }

        public string 銀行名
        {
            get { return _銀行名; }
            set { _銀行名 = value; }
        }

        public string 銀行名カナ
        {
            get { return _銀行名カナ; }
            set { _銀行名カナ = value; }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get{ return null;}
        }

        public string this[string columnName]
        {
            get
            {
                return (_errors.ContainsKey(columnName) ? _errors[columnName] : null);
            }
        }
        #endregion
    }
}

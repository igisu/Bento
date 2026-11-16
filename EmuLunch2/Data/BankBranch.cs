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
    /// 金融機関支店名称マスタ
    /// </summary>
    public class BankBranch : IDataErrorInfo
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
        /// <param name="branchCD"></param>
        /// <returns></returns>
        public static BankBranch GetBankBranch(string bankCD, string branchCD)
        {
            BankBranch bankBranch = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT " +
                                  "\r\n     銀行ＣＤ" +
                                  "\r\n     ,支店ＣＤ" +
                                  "\r\n     ,支店名" +
                                  "\r\n     ,支店名カナ " +
                                  "\r\n FROM " +
                                  "\r\n     金融機関支店名称 " +
                                  "\r\n WHERE " +
                                  "\r\n     銀行ＣＤ = @銀行ＣＤ " +
                                  "\r\n AND 支店ＣＤ = @支店ＣＤ ";

                cmd.Parameters.Add("@銀行ＣＤ", SqlDbType.VarChar).Value = bankCD;
                cmd.Parameters.Add("@支店ＣＤ", SqlDbType.VarChar).Value = branchCD;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                log.Debug(cmd.CommandTextWithParameters());

                DataTable dt = new DataTable("金融機関支店名称");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    bankBranch = new BankBranch(dt.Rows[0]);
                }
            }

            return bankBranch;
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
        private string _支店ＣＤ;
        private string _支店名;
        private string _支店名カナ;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BankBranch()
        {
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public BankBranch(DataRow dr)
        {
            銀行ＣＤ = dr["銀行ＣＤ"] as string;
            支店ＣＤ = dr["支店ＣＤ"] as string;
            支店名 = dr["支店名"] as string;
            支店名カナ = dr["支店名カナ"] as string;
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

        public string 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public string 支店名カナ
        {
            get { return _支店名カナ; }
            set { _支店名カナ = value; }
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

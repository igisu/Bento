using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 管理マスタ
    /// </summary>
    public class Kanri : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <returns></returns>
        public static Kanri GetKanri()
        {
            Kanri kanri = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.会社名" +
                                  "\r\n     ,A.会社名カナ" +
                                  "\r\n     ,A.郵便番号" +
                                  "\r\n     ,A.住所" +
                                  "\r\n     ,A.電話番号" +
                                  "\r\n     ,A.ＦＡＸ" +
                                  "\r\n     ,A.代表取締役" +
                                  "\r\n     ,A.自社締日" +
                                  "\r\n     ,A.チケット回収時売上フラグ" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     管理マスタ AS A" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("管理マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    kanri = new Kanri(dt.Rows[0]);
                }
            }

            return kanri;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private string _会社名;
        private string _会社名カナ;
        private string _郵便番号;
        private string _住所;
        private string _電話番号;
        private string _ＦＡＸ;
        private string _代表取締役;
        private int? _自社締日;
        private bool? _チケット回収時売上フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Kanri()
        {
            会社名 = null;
            会社名カナ = null;
            郵便番号 = null;
            住所 = null;
            電話番号 = null;
            ＦＡＸ = null;
            代表取締役 = null;
            自社締日 = null;
            チケット回収時売上フラグ = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Kanri(Kanri src)
        {
            会社名 = src.会社名;
            会社名カナ = src.会社名カナ;
            郵便番号 = src.郵便番号;
            住所 = src.住所;
            電話番号 = src.電話番号;
            ＦＡＸ = src.ＦＡＸ;
            代表取締役 = src.代表取締役;
            自社締日 = src.自社締日;
            チケット回収時売上フラグ = src.チケット回収時売上フラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Kanri(DataRow dr)
        {
            会社名 = dr["会社名"] as string;
            会社名カナ = dr["会社名カナ"] as string;
            郵便番号 = dr["郵便番号"] as string;
            住所 = dr["住所"] as string;
            電話番号 = dr["電話番号"] as string;
            ＦＡＸ = dr["ＦＡＸ"] as string;
            代表取締役 = dr["代表取締役"] as string;
            自社締日 = dr["自社締日"] as int?;
            チケット回収時売上フラグ = dr["チケット回収時売上フラグ"] as bool?;

            base.FromDataRow(dr);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public string 会社名
        {
            get { return _会社名; }
            set
            {
                _会社名 = value;
                _errors.Remove("会社名");
                if (value == null)
                {
                    _errors["会社名"] = "会社名を入力してください。";
                }
            }
        }

        public string 会社名カナ
        {
            get { return _会社名カナ; }
            set
            {
                _会社名カナ = value;
                _errors.Remove("会社名カナ");
                if (value == null)
                {
                    _errors["会社名カナ"] = "会社名カナを入力してください。";
                }
            }
        }

        public string 郵便番号
        {
            get { return _郵便番号; }
            set
            {
                _郵便番号 = value;
                _errors.Remove("郵便番号");
                if (value == null)
                {
                    _errors["郵便番号"] = "郵便番号を入力してください。";
                }
            }
        }

        public string 住所
        {
            get { return _住所; }
            set
            {
                _住所 = value;
                _errors.Remove("住所");
                if (value == null)
                {
                    _errors["住所"] = "住所を入力してください。";
                }
            }
        }

        public string 電話番号
        {
            get { return _電話番号; }
            set
            {
                _電話番号 = value;
                _errors.Remove("電話番号");
                if (value == null)
                {
                    _errors["電話番号"] = "電話番号を入力してください。";
                }
            }
        }

        public string ＦＡＸ
        {
            get { return _ＦＡＸ; }
            set
            {
                _ＦＡＸ = value;
                _errors.Remove("ＦＡＸ");
                if (value == null)
                {
                    _errors["ＦＡＸ"] = "ＦＡＸを入力してください。";
                }
            }
        }

        public string 代表取締役
        {
            get { return _代表取締役; }
            set
            {
                _代表取締役 = value;
                _errors.Remove("代表取締役");
                if (value == null)
                {
                    _errors["代表取締役"] = "代表取締役を入力してください。";
                }
            }
        }

        public int? 自社締日
        {
            get { return _自社締日; }
            set
            {
                _自社締日 = value;
                _errors.Remove("自社締日");
                if (value == null)
                {
                    _errors["自社締日"] = "自社締日を入力してください。";
                }
                else
                {
                    if (0 <= value && value <= 28)
                    {
                    }
                    else if (value == 29 || value == 30 || value == 31)
                    {
                        _errors["自社締日"] = "月末の場合は99を入力してください。";
                    }
                    else if (value == 99)
                    {
                    }
                    else
                    {
                        _errors["自社締日"] = "1～28 または 月末の場合は99を入力してください。";
                    }
                }
            }
        }

        public bool? チケット回収時売上フラグ
        {
            get { return _チケット回収時売上フラグ; }
            set { _チケット回収時売上フラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("企業ＣＤ=[{0}]", 企業ＣＤ); }
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

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(Kanri target)
        {
            if (企業ＣＤ == target.企業ＣＤ &&
                会社名 == target.会社名 &&
                会社名カナ == target.会社名カナ &&
                郵便番号 == target.郵便番号 &&
                住所 == target.住所 &&
                電話番号 == target.電話番号 &&
                ＦＡＸ == target.ＦＡＸ &&
                代表取締役 == target.代表取締役 &&
                自社締日 == target.自社締日 &&
                (チケット回収時売上フラグ ?? false) == (target.チケット回収時売上フラグ ?? false))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO 管理マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,会社名" +
                         "\r\n     ,会社名カナ" +
                         "\r\n     ,郵便番号" +
                         "\r\n     ,住所" +
                         "\r\n     ,電話番号" +
                         "\r\n     ,ＦＡＸ" +
                         "\r\n     ,代表取締役" +
                         "\r\n     ,自社締日" +
                         "\r\n     ,チケット回収時売上フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@会社名" +
                         "\r\n     ,@会社名カナ" +
                         "\r\n     ,@郵便番号" +
                         "\r\n     ,@住所" +
                         "\r\n     ,@電話番号" +
                         "\r\n     ,@ＦＡＸ" +
                         "\r\n     ,@代表取締役" +
                         "\r\n     ,@自社締日" +
                         "\r\n     ,@チケット回収時売上フラグ" +
                         "\r\n     ,@作成担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n     ,@修正担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n )";

            return Save(con, tran, sql, History.ProcessType.Create);
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Update(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n UPDATE 管理マスタ" +
                         "\r\n SET 会社名                   = @会社名" +
                         "\r\n    ,会社名カナ               = @会社名カナ" +
                         "\r\n    ,郵便番号                 = @郵便番号" +
                         "\r\n    ,住所                     = @住所" +
                         "\r\n    ,電話番号                 = @電話番号" +
                         "\r\n    ,ＦＡＸ                   = @ＦＡＸ" +
                         "\r\n    ,代表取締役               = @代表取締役" +
                         "\r\n    ,自社締日                 = @自社締日" +
                         "\r\n    ,チケット回収時売上フラグ = @チケット回収時売上フラグ" +
                         "\r\n    ,修正担当者ＣＤ           = @修正担当者ＣＤ" +
                         "\r\n    ,修正日                   = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 修正日   = @修正日";

            return Save(con, tran, sql, History.ProcessType.Update);
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n DELETE FROM 管理マスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 修正日   = @修正日";

            return Save(con, tran, sql, History.ProcessType.Delete);
        }

        /// <summary>
        /// DB更新主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="sql">SQL</param>
        /// <param name="historyProcess">処理タイプ</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        protected override bool Save(SqlConnection con, SqlTransaction tran, string sql, History.ProcessType historyProcess)
        {
            try
            {
                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (会社名 != null)
                    {
                        cmd.Parameters.Add("@会社名", SqlDbType.VarChar).Value = 会社名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@会社名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (会社名カナ != null)
                    {
                        cmd.Parameters.Add("@会社名カナ", SqlDbType.VarChar).Value = 会社名カナ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@会社名カナ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (郵便番号 != null)
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = 郵便番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@郵便番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (住所 != null)
                    {
                        cmd.Parameters.Add("@住所", SqlDbType.VarChar).Value = 住所;
                    }
                    else
                    {
                        cmd.Parameters.Add("@住所", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (ＦＡＸ != null)
                    {
                        cmd.Parameters.Add("@ＦＡＸ", SqlDbType.VarChar).Value = ＦＡＸ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＦＡＸ", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (代表取締役 != null)
                    {
                        cmd.Parameters.Add("@代表取締役", SqlDbType.VarChar).Value = 代表取締役;
                    }
                    else
                    {
                        cmd.Parameters.Add("@代表取締役", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (自社締日 != null)
                    {
                        cmd.Parameters.Add("@自社締日", SqlDbType.Int).Value = 自社締日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@自社締日", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (チケット回収時売上フラグ != null)
                    {
                        cmd.Parameters.Add("@チケット回収時売上フラグ", SqlDbType.Bit).Value = チケット回収時売上フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@チケット回収時売上フラグ", SqlDbType.Bit).Value = DBNull.Value;
                    }

                    // 更新の場合のみ編集日時を設定（新規の場合は設定しない）
                    if (修正日 != null)
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = 修正日;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("管理マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "企業ＣＤが重複しています。";
                    }
                    else
                    {
                        History.Error(ex);
                        Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    }
                    return false;

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    History.Error(ex);
                    Message = string.Format("{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString());
                    return false;
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        #endregion
    }
}

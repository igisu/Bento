using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// Web部署注文設定
    /// </summary>
    public class WebOrderSetting : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns></returns>
        public static WebOrderSetting GetWebOrderSetting(int busyoCD)
        {
            WebOrderSetting webOrderSetting = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,B.部署名" +
                                  "\r\n     ,A.受付期限月数" +
                                  "\r\n     ,A.注文可能期限日数" +
                                  "\r\n     ,A.注文締切時間" +
                                  "\r\n     ,A.注文締切分" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     WEB_M_部署注文設定 AS A" +
                                  "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND A.部署ＣＤ = B.部署ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND A.部署ＣＤ = @部署ＣＤ";


                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("WEB_M_部署注文設定");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    webOrderSetting = new WebOrderSetting(dt.Rows[0]);
                }
            }

            return webOrderSetting;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private int? _受付期限月数;
        private int? _注文可能期限日数;
        private int? _注文締切時間;
        private int? _注文締切分;

        private string _部署名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebOrderSetting()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public WebOrderSetting(WebOrderSetting src)
        {
            部署ＣＤ = src.部署ＣＤ;
            受付期限月数 = src.受付期限月数;
            注文可能期限日数 = src.注文可能期限日数;
            注文締切時間 = src.注文締切時間;
            注文締切分 = src.注文締切分;

            部署名 = src.部署名;

            base.Copy(src);
        }
        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public WebOrderSetting(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            受付期限月数 = dr["受付期限月数", version] as int?;
            注文可能期限日数 = dr["注文可能期限日数", version] as int?;
            注文締切時間 = dr["注文締切時間", version] as int?;
            注文締切分 = dr["注文締切分", version] as int?;

            if (dr.Table.Columns.Contains("部署名"))
            {
                部署名 = dr["部署名", version] as string;
            }

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value;  }
        }

        public int? 受付期限月数
        {
            get { return _受付期限月数; }
            set { _受付期限月数 = value; }
        }

        public int? 注文可能期限日数
        {
            get { return _注文可能期限日数; }
            set { _注文可能期限日数 = value; }
        }

        public int? 注文締切時間
        {
            get { return _注文締切時間; }
            set { _注文締切時間 = value; }
        }

        public int? 注文締切分
        {
            get { return _注文締切分; }
            set { _注文締切分 = value; }
        }


        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}]", 部署ＣＤ); }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get
            {
                // 注文締切分が入力されており、注文締切時間が未入力はエラーとする
                // ※注文締切時間が入力されており、注文締切分が未入力はOK(0分とみなす)(★)
                _errors.Remove("注文締切時間");
                if (_注文締切分 != null && _注文締切分 != -1)
                {
                    if (_注文締切時間 == null || _注文締切時間 < 0)
                    {
                        _errors["注文締切時間"] = "締切時間を入力してください。";
                        return "締切時間を入力してください。";
                    }
                }

                // 受付期限月数だけが未入力はエラーとする
                _errors.Remove("受付期限月数");
                if ((_注文可能期限日数 != null && 注文可能期限日数 != -1) ||
                    (_注文締切時間 != null && 注文締切時間 != -1) ||
                    (_注文締切分 != null && 注文締切分 != -1))
                {
                    if (_受付期限月数 == null || _受付期限月数 < 0)
                    {
                        _errors["受付期限月数"] = "受付期限月数を入力してください。";
                        return "受付期限月数を入力してください。";
                    }
                }

                return null;
            }
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
        public bool Equals(WebOrderSetting target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                受付期限月数 == target.受付期限月数 &&
                注文可能期限日数 == target.注文可能期限日数 &&
                注文締切時間 == target.注文締切時間 &&
                注文締切分 == target.注文締切分)
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
            string sql = "\r\n INSERT INTO WEB_M_部署注文設定(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,受付期限月数" +
                         "\r\n     ,注文可能期限日数" +
                         "\r\n     ,注文締切時間" +
                         "\r\n     ,注文締切分" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@受付期限月数" +
                         "\r\n     ,@注文可能期限日数" +
                         "\r\n     ,@注文締切時間" +
                         "\r\n     ,@注文締切分" +
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
            string sql = "\r\n UPDATE WEB_M_部署注文設定" +
                         "\r\n SET 受付期限月数 = @受付期限月数" +
                         "\r\n    ,注文可能期限日数 = @注文可能期限日数" +
                         "\r\n    ,注文締切時間     = @注文締切時間" +
                         "\r\n    ,注文締切分       = @注文締切分" +
                         "\r\n    ,修正担当者ＣＤ   = @修正担当者ＣＤ" +
                         "\r\n    ,修正日           = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
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
            string sql = "\r\n DELETE FROM WEB_M_部署注文設定" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
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

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (受付期限月数 != null && 受付期限月数 != -1)
                    {
                        cmd.Parameters.Add("@受付期限月数", SqlDbType.Int).Value = 受付期限月数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@受付期限月数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (注文可能期限日数 != null && 注文可能期限日数 != -1)
                    {
                        cmd.Parameters.Add("@注文可能期限日数", SqlDbType.Int).Value = 注文可能期限日数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文可能期限日数", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (注文締切時間 != null && 注文締切時間 != -1)
                    {
                        cmd.Parameters.Add("@注文締切時間", SqlDbType.Int).Value = 注文締切時間;

                        if (注文締切分 != null && 注文締切分 != -1)
                        {
                            cmd.Parameters.Add("@注文締切分", SqlDbType.Int).Value = 注文締切分;
                        }
                        else
                        {
                            cmd.Parameters.Add("@注文締切分", SqlDbType.Int).Value = 0;     // (★)
                        }
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文締切時間", SqlDbType.Int).Value = DBNull.Value;
                        cmd.Parameters.Add("@注文締切分", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("部署注文設定マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "部署ＣＤが重複しています。";
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

        #endregion
    }
}

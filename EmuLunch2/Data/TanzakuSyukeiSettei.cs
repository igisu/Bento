using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 短冊集計設定マスタ
    /// </summary>
    public class TanzakuSyukeiSettei : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 空いている最小の集計区分ＣＤを返します
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="count">登録するレコードの件数</param>
        /// <returns>部署ＣＤに該当する空いている最小の集計区分ＣＤ</returns>
        public static int GetSyukeiKubunCD(int busyoCD, int count)
        {
            int syukeiKubunCD = 1;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                syukeiKubunCD = GetSyukeiKubunCD(con, null, busyoCD, count);
            }
            return syukeiKubunCD;
        }

        /// <summary>
        /// 空いている最小の集計区分ＣＤを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="count">登録するレコードの件数</param>
        /// <returns>部署ＣＤに該当する空いている最小の集計区分ＣＤ</returns>
        public static int GetSyukeiKubunCD(SqlConnection con, SqlTransaction tran, int busyoCD, int count)
        {
            int syukeiKubunCD = 1;

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            // 再帰CTE（共通テーブル式）を用いて、空いている集計区分ＣＤを件数分取得する
            cmd.CommandText = "\r\n WITH CTE(集計区分ＣＤ) AS" +
                              "\r\n (SELECT" +
                              "\r\n     1" +
                              "\r\n UNION ALL" +
                              "\r\n SELECT" +
                              "\r\n     集計区分ＣＤ + 1" +
                              "\r\n FROM" +
                              "\r\n     CTE" +
                              "\r\n WHERE" +
                              "\r\n     集計区分ＣＤ < @件数" +
                              "\r\n )" +
                              "\r\n SELECT" +
                              "\r\n     T1.集計区分ＣＤ" +
                              "\r\n FROM" +
                              "\r\n     (SELECT" +
                              "\r\n        集計区分ＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         CTE" +
                              "\r\n     ) T1" +
                              "\r\n     LEFT JOIN" +
                              "\r\n         (SELECT" +
                              "\r\n             集計区分ＣＤ" +
                              "\r\n         FROM" +
                              "\r\n             短冊集計設定マスタ" +
                              "\r\n         WHERE" +
                              "\r\n             企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n         ) AS TSS ON T1.集計区分ＣＤ = TSS.集計区分ＣＤ" +
                              "\r\n WHERE" +
                              "\r\n     TSS.集計区分ＣＤ IS NULL" +
                              "\r\n ORDER BY" +
                              "\r\n     集計区分ＣＤ" +
                              "\r\n OPTION (MAXRECURSION 0)";

            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
            cmd.Parameters.Add("@件数", SqlDbType.Int).Value = count;

            log.Debug(cmd.CommandTextWithParameters());

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    syukeiKubunCD = (int)reader["集計区分ＣＤ"];
                }
            }
            return syukeiKubunCD;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private int? _集計区分ＣＤ;
        private string _集計印刷名;
        private int? _食事区分ＣＤ;
        private bool? _袋計算フラグ;
        private int? _印刷順;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TanzakuSyukeiSettei()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TanzakuSyukeiSettei(TanzakuSyukeiSettei src)
        {
            部署ＣＤ = src.部署ＣＤ;
            集計区分ＣＤ = src.集計区分ＣＤ;
            集計印刷名 = src.集計印刷名;
            食事区分ＣＤ = src.食事区分ＣＤ;
            袋計算フラグ = src.袋計算フラグ;
            印刷順 = src.印刷順;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TanzakuSyukeiSettei(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            集計区分ＣＤ = dr["集計区分ＣＤ", version] as int?;
            集計印刷名 = dr["集計印刷名", version] as string;
            食事区分ＣＤ = dr["食事区分ＣＤ", version] as int?;
            袋計算フラグ = dr["袋計算フラグ", version] as bool?;
            印刷順 = dr["印刷順", version] as int?;

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
            set { _部署ＣＤ = value; }
        }

        public int? 集計区分ＣＤ
        {
            get { return _集計区分ＣＤ; }
            set { _集計区分ＣＤ = value; }
        }

        public string 集計印刷名
        {
            get { return _集計印刷名; }
            set
            {
                _集計印刷名 = value;
                _errors.Remove("集計印刷名");
                if (value == null)
                {
                    _errors["集計印刷名"] = "集計印刷名を入力してください。";
                }
            }
        }

        public int? 食事区分ＣＤ
        {
            get { return _食事区分ＣＤ; }
            set
            {
                _食事区分ＣＤ = value;
                _errors.Remove("食事区分ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["食事区分ＣＤ"] = "食事区分を入力してください。";
                }
            }
        }

        public bool? 袋計算フラグ
        {
            get { return _袋計算フラグ; }
            set { _袋計算フラグ = value; }
        }

        public int? 印刷順
        {
            get { return _印刷順; }
            set
            {
                _印刷順 = value;
                _errors.Remove("印刷順");
                if (value == null || value < 1)
                {
                    _errors["印刷順"] = "印刷順を1以上で入力してください。";
                }
            }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 集計区分ＣＤ=[{1}]", 部署ＣＤ, 集計区分ＣＤ); }
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
        public bool Equals(TanzakuSyukeiSettei target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                集計区分ＣＤ == target.集計区分ＣＤ &&
                集計印刷名 == target.集計印刷名 &&
                (食事区分ＣＤ ?? -1) == (target.食事区分ＣＤ ?? -1) &&
                (袋計算フラグ ?? false) == (target.袋計算フラグ ?? false) &&
                印刷順 == target.印刷順)
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
            string sql = "\r\n INSERT INTO 短冊集計設定マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,集計区分ＣＤ" +
                         "\r\n     ,集計印刷名" +
                         "\r\n     ,食事区分ＣＤ" +
                         "\r\n     ,袋計算フラグ" +
                         "\r\n     ,印刷順" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@集計区分ＣＤ" +
                         "\r\n     ,@集計印刷名" +
                         "\r\n     ,@食事区分ＣＤ" +
                         "\r\n     ,@袋計算フラグ" +
                         "\r\n     ,@印刷順" +
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
            string sql = "\r\n UPDATE 短冊集計設定マスタ" +
                         "\r\n SET 集計印刷名     = @集計印刷名" +
                         "\r\n    ,食事区分ＣＤ   = @食事区分ＣＤ" +
                         "\r\n    ,袋計算フラグ   = @袋計算フラグ" +
                         "\r\n    ,印刷順         = @印刷順" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ    = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ    = @部署ＣＤ" +
                         "\r\n   AND 集計区分ＣＤ= @集計区分ＣＤ" +
                         "\r\n   AND 修正日      = @修正日";

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
            string sql = "\r\n DELETE FROM 短冊集計設定マスタ" +
                         "\r\n WHERE 企業ＣＤ    = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ    = @部署ＣＤ" +
                         "\r\n   AND 集計区分ＣＤ= @集計区分ＣＤ" +
                         "\r\n   AND 修正日      = @修正日";

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

                    if (集計区分ＣＤ != null)
                    {
                        cmd.Parameters.Add("@集計区分ＣＤ", SqlDbType.Int).Value = 集計区分ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計区分ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (集計印刷名 != null)
                    {
                        cmd.Parameters.Add("@集計印刷名", SqlDbType.VarChar).Value = 集計印刷名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@集計印刷名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (食事区分ＣＤ != null && 食事区分ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@食事区分ＣＤ", SqlDbType.Int).Value = 食事区分ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@食事区分ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (袋計算フラグ != null)
                    {
                        cmd.Parameters.Add("@袋計算フラグ", SqlDbType.Bit).Value = 袋計算フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@袋計算フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (印刷順 != null)
                    {
                        cmd.Parameters.Add("@印刷順", SqlDbType.Int).Value = 印刷順;
                    }
                    else
                    {
                        cmd.Parameters.Add("@印刷順", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("短冊集計設定マスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "集計区分ＣＤが重複しています。";
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

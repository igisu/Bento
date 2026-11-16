using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 商品グループマスタ
    /// </summary>
    public class SyohinGourp : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="syohinGourpCD">商品グループＣＤ</param>
        /// <returns></returns>
        public static SyohinGourp GetSyohinGourp(int syohinGourpCD)
        {
            return GetSyohinGourp(syohinGourpCD, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="syohinGourpCD">商品グループＣＤ</param>
        /// <returns></returns>
        public static SyohinGourp GetSyohinGourpReload(int syohinGourpCD)
        {
            return GetSyohinGourp(syohinGourpCD, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="syohinGourpCD">商品グループＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static SyohinGourp GetSyohinGourp(int syohinGourpCD, bool isValid)
        {
            SyohinGourp syohinGourp = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.商品グループＣＤ" +
                             "\r\n     ,A.商品グループ名" +
                             "\r\n     ,A.商品グループ名略称" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者CD" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     商品グループマスタ AS A" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.商品グループＣＤ = @商品グループＣＤ" +
                             "{0}";

                string where = "";
                if (isValid)
                {
                    where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
                }

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(sql, where);
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = syohinGourpCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("商品グループマスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    syohinGourp = new SyohinGourp(dt.Rows[0]);
                }
            }

            return syohinGourp;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _商品グループＣＤ;
        private int? _商品グループＣＤorg;
        private string _商品グループ名;
        private string _商品グループ名略称;
        private bool? _無効フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyohinGourp()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SyohinGourp(SyohinGourp src)
        {
            商品グループＣＤ = src.商品グループＣＤ;
            商品グループＣＤorg = src.商品グループＣＤorg;
            商品グループ名 = src.商品グループ名;
            商品グループ名略称 = src.商品グループ名略称;
            無効フラグ = src.無効フラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SyohinGourp(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            商品グループＣＤ = dr["商品グループＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("商品グループＣＤorg"))
            {
                商品グループＣＤorg = dr["商品グループＣＤorg", version] as int?;
            }
            else
            {
                商品グループＣＤorg = dr["商品グループＣＤ", version] as int?;
            }

            商品グループ名 = dr["商品グループ名", version] as string;
            商品グループ名略称 = dr["商品グループ名略称", version] as string;
            無効フラグ = dr["無効フラグ", version] as bool?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 商品グループＣＤ
        {
            get { return _商品グループＣＤ; }
            set
            {
                _商品グループＣＤ = value;
                _errors.Remove("商品グループＣＤ");
                if (value == null)
                {
                    _errors["商品グループＣＤ"] = "商品グループＣＤを入力してください。";
                }
                else if(value < 0)
                {
                    _errors["商品グループＣＤ"] = "商品グループＣＤにマイナスは入力できません。";
                }
            }
        }

        public int? 商品グループＣＤorg
        {
            get { return _商品グループＣＤorg; }
            set { _商品グループＣＤorg = value; }
        }

        public string 商品グループ名
        {
            get { return _商品グループ名; }
            set
            {
                _商品グループ名 = value;
                _errors.Remove("部署グループ");
                if (value == null)
                {
                    _errors["商品グループ名"] = "商品グループ名を入力してください。";
                }
            }
        }

        public string 商品グループ名略称
        {
            get { return _商品グループ名略称; }
            set
            {
                _商品グループ名略称 = value;
                _errors.Remove("商品グループ名略称");
                if (value == null)
                {
                    _errors["商品グループ名略称"] = "商品グループ名略称を入力してください。";
                }
            }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("商品グループＣＤ=[{0}]", (商品グループＣＤ == 商品グループＣＤorg ? 商品グループＣＤ : 商品グループＣＤorg ?? 商品グループＣＤ)); }
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
        public bool Equals(SyohinGourp target)
        {
            if (商品グループＣＤ == target.商品グループＣＤ &&
                商品グループ名 == target.商品グループ名 &&
                商品グループ名略称 == target.商品グループ名略称 &&
                (無効フラグ ?? false) == (target.無効フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 商品グループマスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,商品グループＣＤ" +
                         "\r\n     ,商品グループ名" +
                         "\r\n     ,商品グループ名略称" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@商品グループＣＤ" +
                         "\r\n     ,@商品グループ名" +
                         "\r\n     ,@商品グループ名略称" +
                         "\r\n     ,@無効フラグ" +
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
            string sql = "\r\n UPDATE 商品グループマスタ" +
                         "\r\n SET 商品グループＣＤ   = @商品グループＣＤ" +
                         "\r\n    ,商品グループ名     = @商品グループ名" +
                         "\r\n    ,商品グループ名略称 = @商品グループ名略称" +
                         "\r\n    ,無効フラグ         = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ     = @修正担当者ＣＤ" +
                         "\r\n    ,修正日             = getdate()" +
                         "\r\n WHERE 企業ＣＤ         = @企業ＣＤ" +
                         "\r\n   AND 商品グループＣＤ = @商品グループＣＤorg" +
                         "\r\n   AND 修正日           = @修正日";

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
            string sql = "\r\n DELETE FROM 商品グループマスタ" +
                         "\r\n WHERE 企業ＣＤ         = @企業ＣＤ" +
                         "\r\n   AND 商品グループＣＤ = @商品グループＣＤorg" +
                         "\r\n   AND 修正日           = @修正日";

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

                    if (商品グループＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = 商品グループＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品グループＣＤorg != null)
                    {
                        cmd.Parameters.Add("@商品グループＣＤorg", SqlDbType.Int).Value = 商品グループＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品グループ名 != null)
                    {
                        cmd.Parameters.Add("@商品グループ名", SqlDbType.VarChar).Value = 商品グループ名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループ名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (商品グループ名略称 != null)
                    {
                        cmd.Parameters.Add("@商品グループ名略称", SqlDbType.VarChar).Value = 商品グループ名略称;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品グループ名略称", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = DBNull.Value;
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

                    log.InfoFormat("商品グループマスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "商品グループＣＤが重複しています。";
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

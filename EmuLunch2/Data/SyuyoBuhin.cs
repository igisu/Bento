using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 主要部品リストマスタ
    /// </summary>
    public class SyuyoBuhin : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <param name="syohinCd">商品ＣＤ</param>
        /// <returns></returns>
        public static SyuyoBuhin GetSyuyoBuhin(int busyoCd, int syohinCd)
        {
            SyuyoBuhin syuyoBuhin = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,B.部署名" +
                                  "\r\n     ,A.商品ＣＤ" +
                                  "\r\n     ,S.商品名" +
                                  "\r\n     ,A.主要商品フラグ" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     主要部品リストマスタ AS A" +
                                  "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ" +
                                  "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND ISNULL(S.無効フラグ, 0) = 0 AND A.商品ＣＤ = S.商品ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND A.部署ＣＤ = @部署ＣＤ" +
                                  "\r\n AND A.商品ＣＤ = @商品ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCd;
                cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCd;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("主要部品リストマスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    syuyoBuhin = new SyuyoBuhin(dt.Rows[0]);
                }
            }

            return syuyoBuhin;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _部署ＣＤ;
        private int? _商品ＣＤ;
        private bool? _主要商品フラグ;

        private string _部署名;
        private string _商品名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SyuyoBuhin()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public SyuyoBuhin(SyuyoBuhin src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = src.部署ＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            主要商品フラグ = src.主要商品フラグ;

            部署名 = src.部署名;
            商品名 = src.商品名;
            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public SyuyoBuhin(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = dr["部署ＣＤ"] as int?;
            商品ＣＤ = dr["商品ＣＤ"] as int?;
            主要商品フラグ = dr["主要商品フラグ"] as bool?;

            if (dr.Table.Columns.Contains("部署名"))
            {
                部署名 = dr["部署名"] as string;
            }
            if (dr.Table.Columns.Contains("商品名"))
            {
                商品名 = dr["商品名"] as string;
            }
            base.FromDataRow(dr);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set
            {
                _部署ＣＤ = value;
                _errors.Remove("部署ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["部署ＣＤ"] = "部署ＣＤを入力してください。";
                }
            }
        }

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set
            {
                _商品ＣＤ = value;
                _errors.Remove("商品ＣＤ");
                if (value == null)
                {
                    _errors["商品ＣＤ"] = "商品ＣＤを入力してください。";
                }
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Syohin syohin = Syohin.GetSyohin((int)_商品ＣＤ);
                        if (syohin != null)
                        {
                            商品名 = syohin.商品名;
                        }
                        else
                        {
                            _errors["商品ＣＤ"] = "商品ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public bool? 主要商品フラグ
        {
            get { return _主要商品フラグ; }
            set
            {
                _主要商品フラグ = (value ?? false);
            }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public string 商品名
        {
            get { return _商品名; }
            set { _商品名 = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 商品ＣＤ=[{1}]", 部署ＣＤ, 商品ＣＤ); }
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
        public bool Equals(SyuyoBuhin target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                商品ＣＤ == target.商品ＣＤ &&
                (主要商品フラグ ?? false) == (target.主要商品フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 主要部品リストマスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,主要商品フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@主要商品フラグ" +
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
            string sql = "\r\n UPDATE 主要部品リストマスタ" +
                         "\r\n SET  主要商品フラグ  = @主要商品フラグ" +
                         "\r\n     ,修正担当者ＣＤ  = @修正担当者ＣＤ" +
                         "\r\n     ,修正日          = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 商品ＣＤ = @商品ＣＤ" +
                         "\r\n   AND 修正日 = @修正日";

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
            string sql = "\r\n DELETE 主要部品リストマスタ" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ = @部署ＣＤ" +
                         "\r\n   AND 商品ＣＤ = @商品ＣＤ" +
                         "\r\n   AND 修正日 = @修正日";

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

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (主要商品フラグ != null)
                    {
                        cmd.Parameters.Add("@主要商品フラグ", SqlDbType.Bit).Value = 主要商品フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@主要商品フラグ", SqlDbType.Bit).Value = DBNull.Value;
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

                    log.InfoFormat("主要部品リストマスタを{0}しました。", historyProcess.DisplayName());
                    return true;
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "商品ＣＤが重複しています。";
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

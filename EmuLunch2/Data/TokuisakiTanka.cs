using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先単価マスタ
    /// </summary>
    public class TokuisakiTanka : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <returns></returns>
        public static TokuisakiTanka GetTokuisakiTanka(int tokuisakiCD, int syohinCD)
        {
            TokuisakiTanka tokuisakiTanka = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.得意先ＣＤ" +
                                  "\r\n     ,A.商品ＣＤ" +
                                  "\r\n     ,A.商品ＣＤ AS 商品ＣＤorg" +
                                  "\r\n     ,S.商品名" +
                                  "\r\n     ,A.商品単価税込" +
                                  "\r\n     ,A.商品単価税抜" +
                                  "\r\n     ,A.無効フラグ" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     得意先単価マスタ AS A" +
                                  "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                                  "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n AND A.商品ＣＤ   = @商品ＣＤ" +
                                  "\r\n AND ISNULL(A.無効フラグ, 0) = 0";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("得意先単価マスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    tokuisakiTanka = new TokuisakiTanka(dt.Rows[0]);
                }
            }

            return tokuisakiTanka;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _得意先ＣＤ;
        private int? _商品ＣＤ;
        private int? _商品ＣＤorg;              // レコード読込直後の値（更新・削除時に使用する）
        private decimal? _商品単価税込;
        private decimal? _商品単価税抜;
        private bool? _無効フラグ;

        private string _商品名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiTanka()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TokuisakiTanka(TokuisakiTanka src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            得意先ＣＤ = src.得意先ＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            商品ＣＤorg = src.商品ＣＤorg;
            商品単価税込 = src.商品単価税込;
            商品単価税抜 = src.商品単価税抜;
            無効フラグ = src.無効フラグ;

            商品名 = src.商品名;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TokuisakiTanka(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            商品ＣＤ = dr["商品ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("商品ＣＤorg"))
            {
                商品ＣＤorg = dr["商品ＣＤorg", version] as int?;
            }
            else
            {
                商品ＣＤorg = dr["商品ＣＤ", version] as int?;
            }

            商品単価税込 = dr["商品単価税込", version] as decimal?;
            商品単価税抜 = dr["商品単価税抜", version] as decimal?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("商品名"))
            {
                商品名 = dr["商品名", version] as string;
            }

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set
            {
                _得意先ＣＤ = value;
                _errors.Remove("得意先ＣＤ");
                if (value == null)
                {
                    _errors["得意先ＣＤ"] = "得意先ＣＤを入力してください。";
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

        public decimal? 商品単価税込
        {
            get { return _商品単価税込; }
            set
            {
                _商品単価税込 = value;
                _errors.Remove("商品単価税込");
                if (value == null)
                {
                    _errors["商品単価税込"] = "商品単価税込を入力してください。";
                }
            }
        }

        public decimal? 商品単価税抜
        {
            get { return _商品単価税抜; }
            set
            {
                _商品単価税抜 = value;
                _errors.Remove("商品単価税抜");
                if (value == null)
                {
                    _errors["商品単価税抜"] = "商品単価税抜を入力してください。";
                }
            }
        }

        public int? 商品ＣＤorg
        {
            get { return _商品ＣＤorg; }
            set { _商品ＣＤorg = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string 商品名
        {
            get { return _商品名; }
            set { _商品名 = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}] 商品ＣＤ=[{1}]", 得意先ＣＤ, (商品ＣＤ == 商品ＣＤorg ? 商品ＣＤ : 商品ＣＤorg ?? 商品ＣＤ)); }
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
        public bool Equals(TokuisakiTanka target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                商品単価税込 == target.商品単価税込 &&
                商品単価税抜 == target.商品単価税抜 &&
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
            string sql = "\r\n INSERT INTO 得意先単価マスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,商品単価税込" +
                         "\r\n     ,商品単価税抜" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@商品単価税込" +
                         "\r\n     ,@商品単価税抜" +
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
            string sql = "\r\n UPDATE 得意先単価マスタ" +
                         "\r\n SET 商品ＣＤ       = @商品ＣＤ" +
                         "\r\n    ,商品単価税込   = @商品単価税込" +
                         "\r\n    ,商品単価税抜   = @商品単価税抜" +
                         "\r\n    ,無効フラグ     = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
                         "\r\n   AND 修正日     = @修正日";

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
            string sql = "\r\n DELETE FROM 得意先単価マスタ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤorg" +
                         "\r\n   AND 修正日     = @修正日";

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

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤorg", SqlDbType.Int).Value = 商品ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品単価税込 != null)
                    {
                        cmd.Parameters.Add("@商品単価税込", SqlDbType.Decimal).Value = 商品単価税込;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価税込", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (商品単価税抜 != null)
                    {
                        cmd.Parameters.Add("@商品単価税抜", SqlDbType.Decimal).Value = 商品単価税抜;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品単価税抜", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("得意先単価マスタを{0}しました。", historyProcess.DisplayName());
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

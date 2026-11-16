using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 大袋ラベル印刷設定
    /// </summary>
    public class BigBagLabelSettei : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        /// <summary>
        /// 指定された条件に該当する大袋ラベル印刷設定があるかどうかを返します。
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="bigBagPrintKind">大袋ラベル印刷種別ＣＤ</param>
        /// <param name="syokujiKbn">大袋ラベル食事区分ＣＤ</param>
        /// <returns>大袋ラベル印刷設定が有る場合はtrue。無ければfalse。</returns>
        public static bool HasBigBagLabelSettei(int busyoCD, Consts.BigBagPrintKind bigBagPrintKind, int syokujiKbn)
        {
            try
            {
                bool ret = false;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     企業ＣＤ" +
                                      "\r\n     ,部署ＣＤ" +
                                      "\r\n     ,大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     ,大袋ラベル食事区分ＣＤ" +
                                      "\r\n     ,商品ＣＤ" +
                                      "\r\n     ,作成担当者ＣＤ" +
                                      "\r\n     ,作成日" +
                                      "\r\n     ,修正担当者ＣＤ" +
                                      "\r\n     ,修正日" +
                                      "\r\n FROM" +
                                      "\r\n     大袋ラベル印刷設定" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                                      "\r\n     AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                    cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = (int)bigBagPrintKind;
                    cmd.Parameters.Add("@大袋ラベル食事区分ＣＤ", SqlDbType.Int).Value = syokujiKbn;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ret = reader.Read();
                    }
                }

                // レコードが読めたかどうかを返す
                return ret;
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

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _部署ＣＤ;
        private int? _大袋ラベル印刷種別ＣＤ;
        private int? _大袋ラベル食事区分ＣＤ;
        private int? _大袋ラベル食事区分ＣＤorg;    // レコード読込直後の値（更新・削除時に使用する）

        private int? _商品ＣＤ;
        private int? _商品ＣＤorg;                  // レコード読込直後の値（更新・削除時に使用する）

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
        public BigBagLabelSettei()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public BigBagLabelSettei(BigBagLabelSettei src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            部署ＣＤ = src.部署ＣＤ;
            大袋ラベル印刷種別ＣＤ = src.大袋ラベル印刷種別ＣＤ;
            大袋ラベル食事区分ＣＤ = src.大袋ラベル食事区分ＣＤ;
            大袋ラベル食事区分ＣＤorg = src.大袋ラベル食事区分ＣＤorg;
            商品ＣＤ = src.商品ＣＤ;
            商品ＣＤorg = src.商品ＣＤorg;

            部署名 = src.部署名;
            商品名 = src.商品名;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public BigBagLabelSettei(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            大袋ラベル印刷種別ＣＤ = dr["大袋ラベル印刷種別ＣＤ", version] as int?;
            大袋ラベル食事区分ＣＤ = dr["大袋ラベル食事区分ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("大袋ラベル食事区分ＣＤorg"))
            {
                大袋ラベル食事区分ＣＤorg = dr["大袋ラベル食事区分ＣＤorg", version] as int?;
            }
            else
            {
                大袋ラベル食事区分ＣＤorg = dr["大袋ラベル食事区分ＣＤ", version] as int?;
            }

            商品ＣＤ = dr["商品ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("商品ＣＤorg"))
            {
                商品ＣＤorg = dr["商品ＣＤorg", version] as int?;
            }
            else
            {
                商品ＣＤorg = dr["商品ＣＤ", version] as int?;
            }

            if (dr.Table.Columns.Contains("部署名"))
            {
                部署名 = dr["部署名", version] as string;
            }

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

        public int? 大袋ラベル印刷種別ＣＤ
        {
            get { return _大袋ラベル印刷種別ＣＤ; }
            set
            {
                _大袋ラベル印刷種別ＣＤ = value;
                _errors.Remove("大袋ラベル印刷種別ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["大袋ラベル印刷種別ＣＤ"] = "種別を入力してください。";
                }
            }
        }

        public int? 大袋ラベル食事区分ＣＤ
        {
            get { return _大袋ラベル食事区分ＣＤ; }
            set
            {
                _大袋ラベル食事区分ＣＤ = value;
                _errors.Remove("大袋ラベル食事区分ＣＤ");
                if (value == null || value < 0)
                {
                    _errors["大袋ラベル食事区分ＣＤ"] = "集計区分を入力してください。";
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

        public int? 大袋ラベル食事区分ＣＤorg
        {
            get { return _大袋ラベル食事区分ＣＤorg; }
            set { _大袋ラベル食事区分ＣＤorg = value; }
        }

        public int? 商品ＣＤorg
        {
            get { return _商品ＣＤorg; }
            set { _商品ＣＤorg = value; }
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
            get { return string.Format("部署ＣＤ=[{0}] 大袋ラベル印刷種別ＣＤ=[{1}] 大袋ラベル食事区分ＣＤ=[{2}] 商品ＣＤ=[{3}]",
                                        部署ＣＤ, 大袋ラベル印刷種別ＣＤ,
                                        (大袋ラベル食事区分ＣＤ == 大袋ラベル食事区分ＣＤorg ? 大袋ラベル食事区分ＣＤ : 大袋ラベル食事区分ＣＤorg ?? 大袋ラベル食事区分ＣＤ),
                                        (商品ＣＤ == 商品ＣＤorg ? 商品ＣＤ : 商品ＣＤorg ?? 商品ＣＤ)); }
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
        public bool Equals(BigBagLabelSettei target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                (大袋ラベル印刷種別ＣＤ ?? -1) == (target.大袋ラベル印刷種別ＣＤ ?? -1) &&
                (大袋ラベル食事区分ＣＤ ?? -1) == (target.大袋ラベル食事区分ＣＤ ?? -1) &&
                商品ＣＤ == target.商品ＣＤ)
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
            string sql = "\r\n INSERT INTO 大袋ラベル印刷設定(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,大袋ラベル印刷種別ＣＤ" +
                         "\r\n     ,大袋ラベル食事区分ＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@大袋ラベル印刷種別ＣＤ" +
                         "\r\n     ,@大袋ラベル食事区分ＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
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
            string sql = "\r\n UPDATE 大袋ラベル印刷設定" +
                         "\r\n SET 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ" +
                         "\r\n    ,商品ＣＤ               = @商品ＣＤ" +
                         "\r\n    ,修正担当者ＣＤ         = @修正担当者ＣＤ" +
                         "\r\n    ,修正日                 = getdate()" +
                         "\r\n WHERE 企業ＣＤ               = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ               = @部署ＣＤ" +
                         "\r\n   AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                         "\r\n   AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤorg" +
                         "\r\n   AND 商品ＣＤ               = @商品ＣＤorg" +
                         "\r\n   AND 修正日                 = @修正日";

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
            string sql = "\r\n DELETE FROM 大袋ラベル印刷設定" +
                         "\r\n WHERE 企業ＣＤ               = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ               = @部署ＣＤ" +
                         "\r\n   AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                         "\r\n   AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤorg" +
                         "\r\n   AND 商品ＣＤ               = @商品ＣＤorg" +
                         "\r\n   AND 修正日                 = @修正日";

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

                    if (大袋ラベル印刷種別ＣＤ != null && 大袋ラベル印刷種別ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = 大袋ラベル印刷種別ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@大袋ラベル印刷種別ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (大袋ラベル食事区分ＣＤ != null && 大袋ラベル食事区分ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@大袋ラベル食事区分ＣＤ", SqlDbType.Int).Value = 大袋ラベル食事区分ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@大袋ラベル食事区分ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (大袋ラベル食事区分ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@大袋ラベル食事区分ＣＤorg", SqlDbType.Int).Value = 大袋ラベル食事区分ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@大袋ラベル食事区分ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("大袋ラベル印刷設定を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "キーが重複しています。";
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

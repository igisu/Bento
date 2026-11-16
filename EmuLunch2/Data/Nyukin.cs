using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 入金明細
    /// </summary>
    public class Nyukin : TableBase2, IDataErrorInfo
    {

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal? _入金番号;
        private int? _入金入力区分;
        private DateTime? _入金日;
        private int? _部署ＣＤ;
        private int? _得意先ＣＤ;
        private string _得意先名;
        private int? _依頼人ＣＤ;
        private string _振込依頼人名;
        private int? _入金方法;
        private decimal? _入金額;
        private int? _入金摘要区分;
        private decimal? _対応請求番号;
        private string _備考;
        // TP決済番号とTP取消決済番号は、エムランチからは一切更新しない。
        // したがって、(取得はするが)追加/更新の対象外としている。
        private string _TP決済番号;
        private string _TP取消決済番号;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Nyukin()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public Nyukin(Nyukin src)
        {
            入金番号 = src.入金番号;
            入金入力区分 = src.入金入力区分;
            入金日 = src.入金日;
            部署ＣＤ = src.部署ＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
            得意先名 = src.得意先名;
            依頼人ＣＤ = src.依頼人ＣＤ;
            振込依頼人名 = src.振込依頼人名;
            入金方法 = src.入金方法;
            入金額 = src.入金額;
            入金摘要区分 = src.入金摘要区分;
            対応請求番号 = src.対応請求番号;
            TP決済番号 = src.TP決済番号;
            TP取消決済番号 = src.TP取消決済番号;
            備考 = src.備考;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public Nyukin(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            入金番号 = dr["入金番号", version] as decimal?;
            入金入力区分 = dr["入金入力区分", version] as int?;
            入金日 = dr["入金日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            得意先名 = dr["得意先名", version] as string;
            依頼人ＣＤ = dr["依頼人ＣＤ", version] as int?;
            振込依頼人名 = dr["振込依頼人名", version] as string;
            入金方法 = dr["入金方法", version] as int?;
            入金額 = dr["入金額", version] as decimal?;
            入金摘要区分 = dr["入金摘要区分", version] as int?;
            対応請求番号 = dr["対応請求番号", version] as decimal?;

            if (dr.Table.Columns.Contains("TP決済番号"))
            {
                TP決済番号 = dr["TP決済番号", version] as string;
            }

            if (dr.Table.Columns.Contains("TP取消決済番号"))
            {
                TP取消決済番号 = dr["TP取消決済番号", version] as string;
            }

            備考 = dr["備考", version] as string;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 入金番号
        {
            get { return _入金番号; }
            set
            {
                _入金番号 = value;
                _errors.Remove("入金番号");
                if (value == null)
                {
                    _errors["入金番号"] = "入金番号を入力してください。";
                }
            }
        }

        public int? 入金入力区分
        {
            get { return _入金入力区分; }
            set
            {
                _入金入力区分 = value;
                _errors.Remove("入金入力区分");
                if (value == null)
                {
                    _errors["入金入力区分"] = "入金入力区分を入力してください。";
                }
            }
        }

        public DateTime? 入金日
        {
            get { return _入金日; }
            set
            {
                _入金日 = value;
                _errors.Remove("入金日");
                if (value == null)
                {
                    _errors["入金日"] = "入金日を入力してください。";
                }
            }
        }

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

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? 依頼人ＣＤ
        {
            get { return _依頼人ＣＤ; }
            set { _依頼人ＣＤ = value; }
        }

        public string 振込依頼人名
        {
            get { return _振込依頼人名; }
            set { _振込依頼人名 = value; }
        }

        public int? 入金方法
        {
            get { return _入金方法; }
            set
            {
                _入金方法 = value;
                _errors.Remove("入金方法");
                if (value == null || value < 0)
                {
                    _errors["入金方法"] = "入金方法を入力してください。";
                }
            }
        }

        public decimal? 入金額
        {
            get { return _入金額; }
            set { _入金額 = value; }
        }

        public int? 入金摘要区分
        {
            get { return _入金摘要区分; }
            set { _入金摘要区分 = value; }
        }

        public decimal? 対応請求番号
        {
            get { return _対応請求番号; }
            set { _対応請求番号 = value; }
        }

        public string TP決済番号
        {
            get { return _TP決済番号; }
            set { _TP決済番号 = value; }
        }

        public string TP取消決済番号
        {
            get { return _TP取消決済番号; }
            set { _TP取消決済番号 = value; }
        }

        public string 備考
        {
            get { return _備考; }
            set { _備考 = value; }
        }

        public override string Code
        {
            get { return string.Format("入金番号=[{0}]", 入金番号); }
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
        public bool Equals(Nyukin target)
        {
            if (入金番号 == target.入金番号 &&
                (入金入力区分 ?? -1) == (target.入金入力区分 ?? -1) &&
                入金日 == target.入金日 &&
                (部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                得意先ＣＤ == target.得意先ＣＤ &&
                得意先名 == target.得意先名 &&
                (依頼人ＣＤ ?? -1) == (target.依頼人ＣＤ ?? -1) &&
                振込依頼人名 == target.振込依頼人名 &&
                (入金方法 ?? -1) == (target.入金方法 ?? -1) &&
                入金額 == target.入金額 &&
                (入金摘要区分 ?? -1) == (target.入金摘要区分 ?? -1) &&
                対応請求番号 == target.対応請求番号 &&
                TP決済番号 == target.TP決済番号 &&
                TP取消決済番号 == target.TP取消決済番号 &&
                備考 == target.備考)
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
            string sql = "\r\n INSERT INTO 入金明細(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,入金番号" +
                         "\r\n     ,入金入力区分" +
                         "\r\n     ,入金日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,得意先名" +
                         "\r\n     ,依頼人ＣＤ" +
                         "\r\n     ,振込依頼人名" +
                         "\r\n     ,入金方法" +
                         "\r\n     ,入金額" +
                         "\r\n     ,入金摘要区分" +
                         "\r\n     ,対応請求番号" +
                         "\r\n     ,備考" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@入金番号" +
                         "\r\n     ,@入金入力区分" +
                         "\r\n     ,@入金日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@得意先名" +
                         "\r\n     ,@依頼人ＣＤ" +
                         "\r\n     ,@振込依頼人名" +
                         "\r\n     ,@入金方法" +
                         "\r\n     ,@入金額" +
                         "\r\n     ,@入金摘要区分" +
                         "\r\n     ,@対応請求番号" +
                         "\r\n     ,@備考" +
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
            string sql = "\r\n UPDATE 入金明細" +
                         "\r\n SET  入金入力区分   = @入金入力区分" +
                         "\r\n     ,入金日         = @入金日" +
                         "\r\n     ,部署ＣＤ       = @部署ＣＤ" +
                         "\r\n     ,得意先ＣＤ     = @得意先ＣＤ" +
                         "\r\n     ,得意先名       = @得意先名" +
                         "\r\n     ,依頼人ＣＤ     = @依頼人ＣＤ" +
                         "\r\n     ,振込依頼人名   = @振込依頼人名" +
                         "\r\n     ,入金方法       = @入金方法" +
                         "\r\n     ,入金額         = @入金額" +
                         "\r\n     ,入金摘要区分   = @入金摘要区分" +
                         "\r\n     ,対応請求番号   = @対応請求番号" +
                         "\r\n     ,備考           = @備考" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 入金番号 = @入金番号" +
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
            string sql = "\r\n DELETE FROM 入金明細" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 入金番号 = @入金番号" +
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

                    if (入金番号 != null)
                    {
                        cmd.Parameters.Add("@入金番号", SqlDbType.Decimal).Value = 入金番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (入金入力区分 != null && 入金入力区分 != -1)
                    {
                        cmd.Parameters.Add("@入金入力区分", SqlDbType.Int).Value = 入金入力区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金入力区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (入金日 != null)
                    {
                        cmd.Parameters.Add("@入金日", SqlDbType.Date).Value = 入金日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先名 != null)
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = 得意先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (依頼人ＣＤ != null && 依頼人ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@依頼人ＣＤ", SqlDbType.Int).Value = 依頼人ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@依頼人ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (振込依頼人名 != null)
                    {
                        cmd.Parameters.Add("@振込依頼人名", SqlDbType.VarChar).Value = 振込依頼人名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@振込依頼人名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (入金方法 != null && 入金方法 != -1)
                    {
                        cmd.Parameters.Add("@入金方法", SqlDbType.Int).Value = 入金方法;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金方法", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (入金額 != null)
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = 入金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (入金摘要区分 != null && 入金摘要区分 != -1)
                    {
                        cmd.Parameters.Add("@入金摘要区分", SqlDbType.Int).Value = 入金摘要区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金摘要区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (対応請求番号 != null)
                    {
                        cmd.Parameters.Add("@対応請求番号", SqlDbType.Decimal).Value = 対応請求番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@対応請求番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (備考 != null)
                    {
                        cmd.Parameters.Add("@備考", SqlDbType.VarChar).Value = 備考;
                    }
                    else
                    {
                        cmd.Parameters.Add("@備考", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("入金明細を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "入金番号が重複しています。";
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

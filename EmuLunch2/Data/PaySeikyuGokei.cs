using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 電子決済請求合計
    /// </summary>
    public class PaySeikyuGokei : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* スタティックメソッド
        //**************************************
        #region スタティックメソッド
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private decimal? _請求番号;
        private DateTime? _請求日;
        private int? _締区分;
        private int? _売上区分;
        private int? _部署ＣＤ;
        private int? _請求先ＣＤ;
        private string _請求先名;
        private decimal? _入金額;
        private decimal? _買上金額;
        private decimal? _調整額;
        private decimal? _今回請求額;
        private DateTime? _回収予定日;
        private int? _印刷者ＣＤ;
        private DateTime? _印刷日時;
        private bool? _EB作成済フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PaySeikyuGokei()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public PaySeikyuGokei(PaySeikyuGokei src)
        {
            請求番号 = src.請求番号;
            請求日 = src.請求日;
            締区分 = src.締区分;
            売上区分 = src.売上区分;
            部署ＣＤ = src.部署ＣＤ;
            請求先ＣＤ = src.請求先ＣＤ;
            請求先名 = src.請求先名;
            入金額 = src.入金額;
            買上金額 = src.買上金額;
            調整額 = src.調整額;
            今回請求額 = src.今回請求額;
            回収予定日 = src.回収予定日;
            印刷者ＣＤ = src.印刷者ＣＤ;
            印刷日時 = src.印刷日時;
            EB作成済フラグ = src.EB作成済フラグ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public PaySeikyuGokei(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            請求番号 = dr["請求番号", version] as decimal?;
            請求日 = dr["請求日", version] as DateTime?;
            締区分 = dr["締区分", version] as int?;
            売上区分 = dr["売上区分", version] as int?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            請求先ＣＤ = dr["請求先ＣＤ", version] as int?;
            請求先名 = dr["請求先名", version] as string;
            入金額 = dr["入金額", version] as decimal?;
            買上金額 = dr["買上金額", version] as decimal?;
            調整額 = dr["調整額", version] as decimal?;
            今回請求額 = dr["今回請求額", version] as decimal?;
            回収予定日 = dr["回収予定日", version] as DateTime?;
            印刷者ＣＤ = dr["印刷者ＣＤ", version] as int?;
            印刷日時 = dr["印刷日時", version] as DateTime?;
            EB作成済フラグ = dr["EB作成済フラグ", version] as bool?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public decimal? 請求番号
        {
            get { return _請求番号; }
            set { _請求番号 = value; }
        }

        public DateTime? 請求日
        {
            get { return _請求日; }
            set { _請求日 = value; }
        }

        public int? 締区分
        {
            get { return _締区分; }
            set { _締区分 = value; }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set { _売上区分 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 請求先ＣＤ
        {
            get { return _請求先ＣＤ; }
            set { _請求先ＣＤ = value; }
        }

        public string 請求先名
        {
            get { return _請求先名; }
            set { _請求先名 = value; }
        }

        public decimal? 入金額
        {
            get { return _入金額; }
            set { _入金額 = value; }
        }

        public decimal? 買上金額
        {
            get { return _買上金額; }
            set { _買上金額 = value; }
        }

        public decimal? 調整額
        {
            get { return _調整額; }
            set { _調整額 = value; }
        }

        public decimal? 今回請求額
        {
            get { return _今回請求額; }
            set { _今回請求額 = value; }
        }

        public DateTime? 回収予定日
        {
            get { return _回収予定日; }
            set { _回収予定日 = value; }
        }

        public int? 印刷者ＣＤ
        {
            get { return _印刷者ＣＤ; }
            set { _印刷者ＣＤ = value; }
        }

        public DateTime? 印刷日時
        {
            get { return _印刷日時; }
            set { _印刷日時 = value; }
        }

        public bool? EB作成済フラグ
        {
            get { return _EB作成済フラグ; }
            set { _EB作成済フラグ = value; }
        }

        public override string Code
        {
            get { return string.Format("請求番号=[{0}]", 請求番号); }
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
        public bool Equals(PaySeikyuGokei target)
        {
            if (請求番号 == target.請求番号 &&
                請求日 == target.請求日 &&
                (締区分 ?? -1) == (target.締区分 ?? -1) &&
                (売上区分 ?? -1) == (target.売上区分 ?? -1) &&
                部署ＣＤ == target.部署ＣＤ &&
                請求先ＣＤ == target.請求先ＣＤ &&
                請求先名 == target.請求先名 &&
                入金額 == target.入金額 &&
                買上金額 == target.買上金額 &&
                調整額 == target.調整額 &&
                今回請求額 == target.今回請求額 &&
                回収予定日 == target.回収予定日 &&
                印刷者ＣＤ == target.印刷者ＣＤ &&
                印刷日時 == target.印刷日時 &&
                (EB作成済フラグ ?? false) == (target.EB作成済フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 電子決済請求合計(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,請求番号" +
                         "\r\n     ,請求日" +
                         "\r\n     ,締区分" +
                         "\r\n     ,売上区分" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,請求先ＣＤ" +
                         "\r\n     ,請求先名" +
                         "\r\n     ,入金額" +
                         "\r\n     ,買上金額" +
                         "\r\n     ,調整額" +
                         "\r\n     ,今回請求額" +
                         "\r\n     ,回収予定日" +
                         "\r\n     ,印刷者ＣＤ" +
                         "\r\n     ,印刷日時" +
                         "\r\n     ,EB作成済フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@請求番号" +
                         "\r\n     ,@請求日" +
                         "\r\n     ,@締区分" +
                         "\r\n     ,@売上区分" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@請求先ＣＤ" +
                         "\r\n     ,@請求先名" +
                         "\r\n     ,@入金額" +
                         "\r\n     ,@買上金額" +
                         "\r\n     ,@調整額" +
                         "\r\n     ,@今回請求額" +
                         "\r\n     ,@回収予定日" +
                         "\r\n     ,@印刷者ＣＤ" +
                         "\r\n     ,@印刷日時" +
                         "\r\n     ,@EB作成済フラグ" +
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
            string sql = "\r\n UPDATE 電子決済請求合計" +
                         "\r\n SET 請求日         = @請求日" +
                         "\r\n    ,締区分         = @締区分" +
                         "\r\n    ,売上区分       = @売上区分" +
                         "\r\n    ,部署ＣＤ       = @部署ＣＤ" +
                         "\r\n    ,請求先ＣＤ     = @請求先ＣＤ" +
                         "\r\n    ,請求先名       = @請求先名" +
                         "\r\n    ,入金額         = @入金額" +
                         "\r\n    ,買上金額       = @買上金額" +
                         "\r\n    ,調整額         = @調整額" +
                         "\r\n    ,今回請求額     = @今回請求額" +
                         "\r\n    ,回収予定日     = @回収予定日" +
                         "\r\n    ,印刷者ＣＤ     = @印刷者ＣＤ" +
                         "\r\n    ,印刷日時       = @印刷日時" +
                         "\r\n    ,EB作成済フラグ = @EB作成済フラグ" +
                         "\r\n    .修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 請求番号 = @請求番号" +
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
            string sql = "\r\n DELETE FROM 電子決済請求合計" +
                         "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                         "\r\n   AND 請求番号 = @請求番号" +
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

                    if (請求番号 != null)
                    {
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = 請求番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (請求日 != null)
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = 請求日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (締区分 != null)
                    {
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = 締区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売上区分 != null)
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = 売上区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = 請求先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (請求先名 != null)
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = 請求先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@請求先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (入金額 != null)
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = 入金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@入金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (買上金額 != null)
                    {
                        cmd.Parameters.Add("@買上金額", SqlDbType.Decimal).Value = 買上金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@買上金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (調整額 != null)
                    {
                        cmd.Parameters.Add("@調整額", SqlDbType.Decimal).Value = 調整額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@調整額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (今回請求額 != null)
                    {
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = 今回請求額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (回収予定日 != null)
                    {
                        cmd.Parameters.Add("@回収予定日", SqlDbType.Date).Value = 回収予定日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@回収予定日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (印刷者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@印刷者ＣＤ", SqlDbType.Int).Value = 印刷者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@印刷者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (印刷日時 != null)
                    {
                        cmd.Parameters.Add("@印刷日時", SqlDbType.DateTime).Value = 印刷日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@印刷日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (EB作成済フラグ != null)
                    {
                        cmd.Parameters.Add("@EB作成済フラグ", SqlDbType.Bit).Value = EB作成済フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@EB作成済フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("電子決済請求合計を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "請求番号が重複しています。";
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

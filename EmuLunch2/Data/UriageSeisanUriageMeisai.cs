using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上精算_売上明細
    /// </summary>
    public class UriageSeisanUriageMeisai : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _精算日;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _売上区分;
        private int? _商品ＣＤ;
        private decimal? _売上数量;
        private decimal? _売上金額;
        private decimal? _値引額;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageSeisanUriageMeisai()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanUriageMeisai(UriageSeisanUriageMeisai src)
        {
            精算日 = src.精算日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            売上区分 = src.売上区分;
            商品ＣＤ = src.商品ＣＤ;
            売上数量 = src.売上数量;
            売上金額 = src.売上金額;
            値引額 = src.値引額;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageSeisanUriageMeisai(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            精算日 = dr["精算日", version] as DateTime?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            コースＣＤ = dr["コースＣＤ", version] as int?;
            売上区分 = dr["売上区分", version] as int?;
            商品ＣＤ = dr["商品ＣＤ", version] as int?;
            売上数量 = dr["売上数量", version] as decimal?;
            売上金額 = dr["売上金額", version] as decimal?;
            値引額 = dr["値引額", version] as decimal?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 精算日
        {
            get { return _精算日; }
            set
            {
                _精算日 = value;
                _errors.Remove("精算日");
                if (value == null)
                {
                    _errors["精算日"] = "精算日を入力してください。";
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

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                _errors.Remove("コースＣＤ");
                if (value == null)
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
            }
        }

        public int? 売上区分
        {
            get { return _売上区分; }
            set
            {
                _売上区分 = value;
                _errors.Remove("売上区分");
                if (value == null)
                {
                    _errors["売上区分"] = "売上区分を入力してください。";
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
            }
        }

        public decimal? 売上数量
        {
            get { return _売上数量; }
            set { _売上数量 = value; }
        }

        public decimal? 売上金額
        {
            get { return _売上金額; }
            set { _売上金額 = value; }
        }

        public decimal? 値引額
        {
            get { return _値引額; }
            set { _値引額 = value; }
        }

        public override string Code
        {
            get { return string.Format("精算日=[{0}] 部署ＣＤ=[{1}] コースＣＤ=[{2}] 売上区分=[{3}] 商品ＣＤ=[{4}]", 精算日, 部署ＣＤ, コースＣＤ, 売上区分, 商品ＣＤ); }
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
        public bool Equals(UriageSeisanUriageMeisai target)
        {
            if (精算日 == target.精算日 &&
                部署ＣＤ == target.部署ＣＤ &&
                コースＣＤ == target.コースＣＤ &&
                売上区分 == target.売上区分 &&
                商品ＣＤ == target.商品ＣＤ &&
                売上数量 == target.売上数量 &&
                売上金額 == target.売上金額 &&
                値引額 == target.値引額)
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
            string sql = "\r\n INSERT INTO 売上精算_売上明細(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,精算日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,売上区分" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,売上数量" +
                         "\r\n     ,売上金額" +
                         "\r\n     ,値引額" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@精算日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@売上区分" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@売上数量" +
                         "\r\n     ,@売上金額" +
                         "\r\n     ,@値引額" +
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
            string sql = "\r\n UPDATE 売上精算_売上明細" +
                         "\r\n SET  売上数量       = @売上数量" +
                         "\r\n     ,売上金額       = @売上金額" +
                         "\r\n     ,値引額         = @値引額" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
                         "\r\n   AND 売上区分   = @売上区分" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤ" +
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
            string sql = "\r\n DELETE FROM 売上精算_売上明細" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
                         "\r\n   AND 売上区分   = @売上区分" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤ" +
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

                    if (精算日 != null)
                    {
                        cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = 精算日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = 商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売上区分 != null)
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = 売上区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (売上数量 != null)
                    {
                        cmd.Parameters.Add("@売上数量", SqlDbType.Decimal).Value = 売上数量;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上数量", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (売上金額 != null)
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = 売上金額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上金額", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (値引額 != null)
                    {
                        cmd.Parameters.Add("@値引額", SqlDbType.Decimal).Value = 値引額;
                    }
                    else
                    {
                        cmd.Parameters.Add("@値引額", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("売上精算_売上明細を{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "商品ＣＤ、売上区分が重複しています。";
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

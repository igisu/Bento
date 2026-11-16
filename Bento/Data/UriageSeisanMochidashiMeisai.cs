using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上精算_持出明細
    /// </summary>
    public class UriageSeisanMochidashiMeisai : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private DateTime? _精算日;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;
        private int? _商品ＣＤ;
        private decimal? _持出個数;
        private decimal? _工場追加数;
        private decimal? _もらった数;
        private decimal? _やった数;
        private decimal? _残数;
        private decimal? _売上予定実績数;
        private decimal? _ボーナス消費数;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageSeisanMochidashiMeisai()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageSeisanMochidashiMeisai(UriageSeisanMochidashiMeisai src)
        {
            精算日 = src.精算日;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            持出個数 = src.持出個数;
            工場追加数 = src.工場追加数;
            もらった数 = src.もらった数;
            やった数 = src.やった数;
            残数 = src.残数;
            売上予定実績数 = src.売上予定実績数;
            ボーナス消費数 = src.ボーナス消費数;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageSeisanMochidashiMeisai(DataRow dr)
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
            商品ＣＤ = dr["商品ＣＤ", version] as int?;
            持出個数 = dr["持出個数", version] as decimal?;
            工場追加数 = dr["工場追加数", version] as decimal?;
            もらった数 = dr["もらった数", version] as decimal?;
            やった数 = dr["やった数", version] as decimal?;
            残数 = dr["残数", version] as decimal?;
            売上予定実績数 = dr["売上予定実績数", version] as decimal?;
            ボーナス消費数 = dr["ボーナス消費数", version] as decimal?;

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

        public decimal? 持出個数
        {
          get { return _持出個数; }
          set { _持出個数 = value; }
        }

        public decimal? 工場追加数
        {
          get { return _工場追加数; }
          set { _工場追加数 = value; }
        }

        public decimal? もらった数
        {
          get { return _もらった数; }
          set { _もらった数 = value; }
        }

        public decimal? やった数
        {
          get { return _やった数; }
          set { _やった数 = value; }
        }

        public decimal? 残数
        {
          get { return _残数; }
          set { _残数 = value; }
        }

        public decimal? 売上予定実績数
        {
          get { return _売上予定実績数; }
          set { _売上予定実績数 = value; }
        }

        public decimal? ボーナス消費数
        {
          get { return _ボーナス消費数; }
          set { _ボーナス消費数 = value; }
        }

        public override string Code
        {
            get { return string.Format("精算日=[{0}] 部署ＣＤ=[{1}] コースＣＤ=[{2}] 商品ＣＤ=[{3}]", 精算日, 部署ＣＤ, コースＣＤ, 商品ＣＤ); }
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
        public bool Equals(UriageSeisanMochidashiMeisai target)
        {
            if (精算日 == target.精算日 &&
                部署ＣＤ == target.部署ＣＤ &&
                コースＣＤ == target.コースＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                持出個数 == target.持出個数 &&
                工場追加数 == target.工場追加数 &&
                もらった数 == target.もらった数 &&
                やった数 == target.やった数 &&
                残数 == target.残数 &&
                売上予定実績数 == target.売上予定実績数 &&
                ボーナス消費数 == target.ボーナス消費数)
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
            string sql = "\r\n INSERT INTO 売上精算_持出明細(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,精算日" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,持出個数" +
                         "\r\n     ,工場追加数" +
                         "\r\n     ,もらった数" +
                         "\r\n     ,やった数" +
                         "\r\n     ,残数" +
                         "\r\n     ,売上予定実績数" +
                         "\r\n     ,ボーナス消費数" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@精算日" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@持出個数" +
                         "\r\n     ,@工場追加数" +
                         "\r\n     ,@もらった数" +
                         "\r\n     ,@やった数" +
                         "\r\n     ,@残数" +
                         "\r\n     ,@売上予定実績数" +
                         "\r\n     ,@ボーナス消費数" +
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
            string sql = "\r\n UPDATE 売上精算_持出明細" +
                         "\r\n SET  持出個数       = @持出個数" +
                         "\r\n     ,工場追加数     = @工場追加数" +
                         "\r\n     ,もらった数     = @もらった数" +
                         "\r\n     ,やった数       = @やった数" +
                         "\r\n     ,残数           = @残数" +
                         "\r\n     ,売上予定実績数 = @売上予定実績数" +
                         "\r\n     ,ボーナス消費数 = @ボーナス消費数" +
                         "\r\n WHERE 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
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
            string sql = "\r\n DELETE FROM 売上精算_持出明細" +
                         "\r\n WHERE 精算日     = @精算日" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
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

                    if (持出個数 != null)
                    {
                        cmd.Parameters.Add("@持出個数", SqlDbType.Decimal).Value = 持出個数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@持出個数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (工場追加数 != null)
                    {
                        cmd.Parameters.Add("@工場追加数", SqlDbType.Decimal).Value = 工場追加数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@工場追加数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (もらった数 != null)
                    {
                        cmd.Parameters.Add("@もらった数", SqlDbType.Decimal).Value = もらった数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@もらった数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (やった数 != null)
                    {
                        cmd.Parameters.Add("@やった数", SqlDbType.Decimal).Value = やった数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@やった数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (残数 != null)
                    {
                        cmd.Parameters.Add("@残数", SqlDbType.Decimal).Value = 残数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@残数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (売上予定実績数 != null)
                    {
                        cmd.Parameters.Add("@売上予定実績数", SqlDbType.Decimal).Value = 売上予定実績数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上予定実績数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (ボーナス消費数 != null)
                    {
                        cmd.Parameters.Add("@ボーナス消費数", SqlDbType.Decimal).Value = ボーナス消費数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ボーナス消費数", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("売上精算_持出明細を{0}しました。", historyProcess.DisplayName());
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

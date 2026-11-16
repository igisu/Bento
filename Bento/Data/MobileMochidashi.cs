using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// モバイル_持ち出し入力
    /// </summary>
    public class MobileMochidashi : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private int? _コースＣＤ;
        private int? _商品ＣＤ;
        private DateTime? _持出日;
        private decimal? _個数;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MobileMochidashi()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public MobileMochidashi(MobileMochidashi src)
        {
            コースＣＤ = src.コースＣＤ;
            商品ＣＤ = src.商品ＣＤ;
            持出日 = src.持出日;
            個数 = src.個数;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public MobileMochidashi(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            コースＣＤ = dr["コースＣＤ", version] as int?;
            商品ＣＤ = dr["商品ＣＤ", version] as int?;
            持出日 = dr["持出日", version] as DateTime?;
            個数 = dr["個数", version] as decimal?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set { _商品ＣＤ = value; }
        }

        public DateTime? 持出日
        {
            get { return _持出日; }
            set { _持出日 = value; }
        }

        public decimal? 個数
        {
            get { return _個数; }
            set { _個数 = value; }
        }

        public override string Code
        {
            get { return string.Format("コースＣＤ=[{0}] 商品ＣＤ=[{1}] 持出日=[{2}]", コースＣＤ, 商品ＣＤ, 持出日); }
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
        public bool Equals(MobileMochidashi target)
        {
            if (コースＣＤ == target.コースＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                持出日 == target.持出日 &&
                個数 == target.個数)
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
            string sql = "\r\n INSERT INTO モバイル_持ち出し入力(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,持出日" +
                         "\r\n     ,個数" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@持出日" +
                         "\r\n     ,@個数" +
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
            string sql = "\r\n UPDATE モバイル_持ち出し入力" +
                         "\r\n SET 個数           = @個数" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤ" +
                         "\r\n   AND 持出日     = @持出日" +
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
            string sql = "\r\n DELETE FROM モバイル_持ち出し入力" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ = @コースＣＤ" +
                         "\r\n   AND 商品ＣＤ   = @商品ＣＤ" +
                         "\r\n   AND 持出日     = @持出日" +
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

                    if (持出日 != null)
                    {
                        cmd.Parameters.Add("@持出日", SqlDbType.Date).Value = 持出日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@持出日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (個数 != null)
                    {
                        cmd.Parameters.Add("@個数", SqlDbType.Decimal).Value = 個数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@個数", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("モバイル_持ち出し入力を{0}しました。", historyProcess.DisplayName());
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

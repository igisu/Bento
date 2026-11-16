using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// モバイル_移動入力
    /// </summary>
    public class MobileIdo : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        private int? _コースＣＤ;
        private int? _相手コースＣＤ;
        private int? _相手コースＣＤorg;        // レコード読込直後の値（更新・削除時に使用する）
        private int? _商品ＣＤ;
        private int? _商品ＣＤorg;              // レコード読込直後の値（更新・削除時に使用する）
        private DateTime? _移動日;
        private int? _ＳＥＱ;
        private int? _ＳＥＱorg;                // レコード読込直後の値（更新・削除時に使用する）
        private decimal? _個数;
        private bool? _確認フラグ;
        private DateTime? _確認日時;
        private bool? _相手確認フラグ;
        private DateTime? _相手確認日時;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MobileIdo()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public MobileIdo(MobileIdo src)
        {
            コースＣＤ = src.コースＣＤ;
            相手コースＣＤ = src.相手コースＣＤ;
            相手コースＣＤorg = src.相手コースＣＤorg;
            商品ＣＤ = src.商品ＣＤ;
            商品ＣＤorg = src.商品ＣＤorg;
            移動日 = src.移動日;
            ＳＥＱ = src.ＳＥＱ;
            ＳＥＱorg = src.ＳＥＱorg;
            個数 = src.個数;
            確認フラグ = src.確認フラグ;
            確認日時 = src.確認日時;
            相手確認フラグ = src.相手確認フラグ;
            相手確認日時 = src.相手確認日時;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public MobileIdo(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            コースＣＤ = dr["コースＣＤ", version] as int?;
            相手コースＣＤ = dr["相手コースＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("相手コースＣＤorg"))
            {
                相手コースＣＤorg = dr["相手コースＣＤorg", version] as int?;
            }
            else
            {
                相手コースＣＤorg = dr["相手コースＣＤ", version] as int?;
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

            移動日 = dr["移動日", version] as DateTime?;
            ＳＥＱ = dr["ＳＥＱ", version] as int?;

            if (dr.Table.Columns.Contains("ＳＥＱorg"))
            {
                ＳＥＱorg = dr["ＳＥＱorg", version] as int?;
            }
            else
            {
                ＳＥＱorg = dr["ＳＥＱ", version] as int?;
            }

            個数 = dr["個数", version] as decimal?;
            確認フラグ = dr["確認フラグ", version] as bool?;
            確認日時 = dr["確認日時", version] as DateTime?;
            相手確認フラグ = dr["相手確認フラグ", version] as bool?;
            相手確認日時 = dr["相手確認日時", version] as DateTime?;

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

        public int? 相手コースＣＤ
        {
            get { return _相手コースＣＤ; }
            set { _相手コースＣＤ = value; }
        }

        public int? 相手コースＣＤorg
        {
            get { return _相手コースＣＤorg; }
            set { _相手コースＣＤorg = value; }
        }

        public int? 商品ＣＤ
        {
            get { return _商品ＣＤ; }
            set { _商品ＣＤ = value; }
        }

        public int? 商品ＣＤorg
        {
            get { return _商品ＣＤorg; }
            set { _商品ＣＤorg = value; }
        }

        public DateTime? 移動日
        {
            get { return _移動日; }
            set { _移動日 = value; }
        }

        public int? ＳＥＱ
        {
            get { return _ＳＥＱ; }
            set { _ＳＥＱ = value; }
        }

        public int? ＳＥＱorg
        {
            get { return _ＳＥＱorg; }
            set { _ＳＥＱorg = value; }
        }

        public decimal? 個数
        {
            get { return _個数; }
            set { _個数 = value; }
        }

        public bool? 確認フラグ
        {
            get { return _確認フラグ; }
            set { _確認フラグ = value; }
        }

        public DateTime? 確認日時
        {
            get { return _確認日時; }
            set { _確認日時 = value; }
        }

        public bool? 相手確認フラグ
        {
            get { return _相手確認フラグ; }
            set { _相手確認フラグ = value; }
        }

        public DateTime? 相手確認日時
        {
            get { return _相手確認日時; }
            set { _相手確認日時 = value; }
        }

        public override string Code
        {
            get { return string.Format("コースＣＤ=[{0}] 相手コースＣＤ=[{1}] 商品ＣＤ=[{2}] 移動日=[{3}] ＳＥＱ=[{4}] 個数=[{5}]",
                                        コースＣＤ,
                                        (相手コースＣＤ == 相手コースＣＤorg ? 相手コースＣＤ : 相手コースＣＤorg ?? 相手コースＣＤ),
                                        (商品ＣＤ == 商品ＣＤorg ? 商品ＣＤ : 商品ＣＤorg ?? 商品ＣＤ),
                                        移動日,
                                        (ＳＥＱ == ＳＥＱorg ? ＳＥＱ : ＳＥＱorg ?? ＳＥＱ), 個数); }
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
        public bool Equals(MobileIdo target)
        {
            if (コースＣＤ == target.コースＣＤ &&
                相手コースＣＤ == target.相手コースＣＤ &&
                商品ＣＤ == target.商品ＣＤ &&
                移動日 == target.移動日 &&
                ＳＥＱ == target.ＳＥＱ &&
                個数 == target.個数 &&
                (確認フラグ ?? false) == (target.確認フラグ ?? false) &&
                確認日時 == target.確認日時 &&
                (相手確認フラグ ?? false) == (target.相手確認フラグ ?? false) &&
                相手確認日時 == target.相手確認日時)
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
            string sql = "\r\n INSERT INTO モバイル_移動入力(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,相手コースＣＤ" +
                         "\r\n     ,商品ＣＤ" +
                         "\r\n     ,移動日" +
                         "\r\n     ,ＳＥＱ" +
                         "\r\n     ,個数" +
                         "\r\n     ,確認フラグ" +
                         "\r\n     ,確認日時" +
                         "\r\n     ,相手確認フラグ" +
                         "\r\n     ,相手確認日時" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@相手コースＣＤ" +
                         "\r\n     ,@商品ＣＤ" +
                         "\r\n     ,@移動日" +
                         "\r\n     ,@ＳＥＱ" +
                         "\r\n     ,@個数" +
                         "\r\n     ,@確認フラグ" +
                         "\r\n     ,case when @確認フラグ = 1 then getdate() else null end" +
                         "\r\n     ,@相手確認フラグ" +
                         "\r\n     ,case when @相手確認フラグ = 1 then getdate() else null end" +
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
            string sql = "\r\n UPDATE モバイル_移動入力" +
                         "\r\n SET ＳＥＱ         = @ＳＥＱ" +
                         "\r\n    ,個数           = @個数" +
                         "\r\n    ,確認フラグ     = @確認フラグ" +
                         "\r\n    ,確認日時       = case when 確認フラグ = @確認フラグ then 確認日時 else" +
                         "\r\n                           case when @確認フラグ = 1 then getdate() else null end end" +
                         "\r\n    ,相手確認フラグ = @相手確認フラグ" +
                         "\r\n    ,相手確認日時   = case when 相手確認フラグ = @相手確認フラグ then 相手確認日時 else" +
                         "\r\n                           case when @相手確認フラグ = 1 then getdate() else null end end" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ       = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ     = @コースＣＤ" +
                         "\r\n   AND 相手コースＣＤ = @相手コースＣＤorg" +
                         "\r\n   AND 商品ＣＤ       = @商品ＣＤorg" +
                         "\r\n   AND 移動日         = @移動日" +
                         "\r\n   AND ＳＥＱ         = @ＳＥＱorg" +
                         "\r\n   AND 修正日         = @修正日";

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
            string sql = "\r\n DELETE FROM モバイル_移動入力" +
                         "\r\n WHERE 企業ＣＤ       = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ     = @コースＣＤ" +
                         "\r\n   AND 相手コースＣＤ = @相手コースＣＤorg" +
                         "\r\n   AND 商品ＣＤ       = @商品ＣＤorg" +
                         "\r\n   AND 移動日         = @移動日" +
                         "\r\n   AND ＳＥＱ         = @ＳＥＱorg" +
                         "\r\n   AND 修正日         = @修正日";

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

                    if (相手コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@相手コースＣＤ", SqlDbType.Int).Value = 相手コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@相手コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (相手コースＣＤorg != null)
                    {
                        cmd.Parameters.Add("@相手コースＣＤorg", SqlDbType.Int).Value = 相手コースＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@相手コースＣＤorg", SqlDbType.Int).Value = DBNull.Value;
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

                    if (移動日 != null)
                    {
                        cmd.Parameters.Add("@移動日", SqlDbType.Date).Value = 移動日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@移動日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (ＳＥＱ != null)
                    {
                        cmd.Parameters.Add("@ＳＥＱ", SqlDbType.Int).Value = ＳＥＱ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＳＥＱ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (ＳＥＱorg != null)
                    {
                        cmd.Parameters.Add("@ＳＥＱorg", SqlDbType.Int).Value = ＳＥＱorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ＳＥＱorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (個数 != null)
                    {
                        cmd.Parameters.Add("@個数", SqlDbType.Decimal).Value = 個数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@個数", SqlDbType.Decimal).Value = DBNull.Value;
                    }

                    if (確認フラグ != null)
                    {
                        cmd.Parameters.Add("@確認フラグ", SqlDbType.Bit).Value = 確認フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@確認フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (確認日時 != null)
                    {
                        cmd.Parameters.Add("@確認日時", SqlDbType.DateTime).Value = 確認日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@確認日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (相手確認フラグ != null)
                    {
                        cmd.Parameters.Add("@相手確認フラグ", SqlDbType.Bit).Value = 相手確認フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@相手確認フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (相手確認日時 != null)
                    {
                        cmd.Parameters.Add("@相手確認日時", SqlDbType.DateTime).Value = 相手確認日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@相手確認日時", SqlDbType.DateTime).Value = DBNull.Value;
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

                    log.InfoFormat("モバイル_移動入力を{0}しました。", historyProcess.DisplayName());
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

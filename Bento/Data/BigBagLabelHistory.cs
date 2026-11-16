using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 大袋ラベル印刷履歴
    /// </summary>
    public class BigBagLabelHistory : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _部署ＣＤ;
        private int? _大袋ラベル印刷種別ＣＤ;
        private int? _大袋ラベル食事区分ＣＤ;
        private DateTime? _喫食日;
        private int? _印刷実行番号;
        private decimal? _印刷枚数;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BigBagLabelHistory()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public BigBagLabelHistory(BigBagLabelHistory src)
        {
            部署ＣＤ = src.部署ＣＤ;
            大袋ラベル印刷種別ＣＤ = src.大袋ラベル印刷種別ＣＤ;
            大袋ラベル食事区分ＣＤ = src.大袋ラベル食事区分ＣＤ;
            喫食日 = src.喫食日;
            印刷実行番号 = src.印刷実行番号;
            印刷枚数 = src.印刷枚数;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public BigBagLabelHistory(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            大袋ラベル印刷種別ＣＤ = dr["大袋ラベル印刷種別ＣＤ", version] as int?;
            大袋ラベル食事区分ＣＤ = dr["大袋ラベル食事区分ＣＤ", version] as int?;
            喫食日 = dr["喫食日", version] as DateTime?;
            印刷実行番号 = dr["印刷実行番号", version] as int?;
            印刷枚数 = dr["印刷枚数", version] as decimal?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public int? 大袋ラベル印刷種別ＣＤ
        {
            get { return _大袋ラベル印刷種別ＣＤ; }
            set { _大袋ラベル印刷種別ＣＤ = value; }
        }

        public int? 大袋ラベル食事区分ＣＤ
        {
            get { return _大袋ラベル食事区分ＣＤ; }
            set { _大袋ラベル食事区分ＣＤ = value; }
        }

        public DateTime? 喫食日
        {
            get { return _喫食日; }
            set { _喫食日 = value; }
        }

        public int? 印刷実行番号
        {
            get { return _印刷実行番号; }
            set { _印刷実行番号 = value; }
        }

        public decimal? 印刷枚数
        {
            get { return _印刷枚数; }
            set { _印刷枚数 = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] 大袋ラベル印刷種別ＣＤ=[{1}] 大袋ラベル食事区分ＣＤ=[{2}] 喫食日=[{3}]",
                                        部署ＣＤ, 大袋ラベル印刷種別ＣＤ, 大袋ラベル食事区分ＣＤ, 喫食日); }
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
        public bool Equals(BigBagLabelHistory target)
        {
            if (部署ＣＤ == target.部署ＣＤ &&
                (大袋ラベル印刷種別ＣＤ ?? -1) == (target.大袋ラベル印刷種別ＣＤ ?? -1) &&
                (大袋ラベル食事区分ＣＤ ?? -1) == (target.大袋ラベル食事区分ＣＤ ?? -1) &&
                喫食日 == target.喫食日 &&
                印刷実行番号 == target.印刷実行番号 &&
                印刷枚数 == target.印刷枚数)
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
            string sql = "\r\n INSERT INTO 大袋ラベル印刷履歴(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,大袋ラベル印刷種別ＣＤ" +
                         "\r\n     ,大袋ラベル食事区分ＣＤ" +
                         "\r\n     ,喫食日" +
                         "\r\n     ,印刷実行番号" +
                         "\r\n     ,印刷枚数" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@大袋ラベル印刷種別ＣＤ" +
                         "\r\n     ,@大袋ラベル食事区分ＣＤ" +
                         "\r\n     ,@喫食日" +
                         "\r\n     ,(SELECT" +
                         "\r\n          ISNULL(MAX(印刷実行番号), 0) + 1" +
                         "\r\n      FROM" +
                         "\r\n          大袋ラベル印刷履歴" +
                         "\r\n      WHERE 企業ＣＤ               = @企業ＣＤ" +
                         "\r\n        AND 部署ＣＤ               = @部署ＣＤ" +
                         "\r\n        AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                         "\r\n        AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ" +
                         "\r\n        AND 喫食日                 = @喫食日" +
                         "\r\n      )" +
                         "\r\n     ,@印刷枚数" +
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
            string sql = "\r\n UPDATE 大袋ラベル印刷履歴" +
                         "\r\n SET 印刷枚数       = @印刷枚数" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ               = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ               = @部署ＣＤ" +
                         "\r\n   AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                         "\r\n   AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ" +
                         "\r\n   AND 喫食日                 = @喫食日" +
                         "\r\n   AND 印刷実行番号           = @印刷実行番号" +
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
            string sql = "\r\n DELETE FROM 大袋ラベル印刷履歴" +
                         "\r\n WHERE 企業ＣＤ               = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ               = @部署ＣＤ" +
                         "\r\n   AND 大袋ラベル印刷種別ＣＤ = @大袋ラベル印刷種別ＣＤ" +
                         "\r\n   AND 大袋ラベル食事区分ＣＤ = @大袋ラベル食事区分ＣＤ" +
                         "\r\n   AND 喫食日                 = @喫食日" +
                         "\r\n   AND 印刷実行番号           = @印刷実行番号" +
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

                    if (喫食日 != null)
                    {
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = 喫食日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (印刷実行番号 != null)
                    {
                        cmd.Parameters.Add("@印刷実行番号", SqlDbType.Int).Value = 印刷実行番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@印刷実行番号", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (印刷枚数 != null)
                    {
                        cmd.Parameters.Add("@印刷枚数", SqlDbType.Decimal).Value = 印刷枚数;
                    }
                    else
                    {
                        cmd.Parameters.Add("@印刷枚数", SqlDbType.Decimal).Value = DBNull.Value;
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

                    log.InfoFormat("大袋ラベル印刷履歴を{0}しました。", historyProcess.DisplayName());
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

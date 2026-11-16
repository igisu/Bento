using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先履歴テーブル
    /// </summary>
    public class TokuisakiRireki : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// 履歴テーブルへの登録対象かどうかを返します。
        /// </summary>
        /// <param name="tokuisaki">編集中の得意先マスタ</param>
        /// <param name="origin">初期状態の得意先マスタ</param>
        /// <returns>履歴テーブルの項目がどれか一つでも変更されている場合はtrue</returns>
        public static bool IsSaveTarget(Tokuisaki tokuisaki, Tokuisaki origin)
        {
            if (tokuisaki.得意先ＣＤ == origin.得意先ＣＤ &&
                (tokuisaki.状態区分 ?? -1) == (origin.状態区分 ?? -1) &&
                (tokuisaki.状態理由区分 ?? -1) == (origin.状態理由区分 ?? -1) &&
                tokuisaki.失客日 == origin.失客日 &&
                tokuisaki.承認日 == origin.承認日 &&
                (tokuisaki.承認者ＣＤ ?? -1) == (origin.承認者ＣＤ ?? -1) &&
                (tokuisaki.営業担当者ＣＤ ?? -1) == (origin.営業担当者ＣＤ ?? -1))
            {
                return false;
            }
            return true;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _得意先ＣＤ;
        private int? _状態区分;
        private int? _失客理由;
        private DateTime? _失客日;
        private DateTime? _承認日;
        private int? _承認者ＣＤ;
        private int? _営業担当者ＣＤ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiRireki()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TokuisakiRireki(TokuisakiRireki src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            状態区分 = src.状態区分;
            失客理由 = src.失客理由;
            失客日 = src.失客日;
            承認日 = src.承認日;
            承認者ＣＤ = src.承認者ＣＤ;
            営業担当者ＣＤ = src.営業担当者ＣＤ;

            base.Copy(src);
        }

        /// <summary>
        /// 得意先マスタからのコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TokuisakiRireki(Tokuisaki src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            状態区分 = src.状態区分;
            失客理由 = src.状態理由区分;
            失客日 = src.失客日;
            承認日 = src.承認日;
            承認者ＣＤ = src.承認者ＣＤ;
            営業担当者ＣＤ = src.営業担当者ＣＤ;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TokuisakiRireki(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            状態区分 = dr["状態区分", version] as int?;
            失客理由 = dr["失客理由", version] as int?;
            失客日 = dr["失客日", version] as DateTime?;
            承認日 = dr["承認日", version] as DateTime?;
            承認者ＣＤ = dr["承認者ＣＤ", version] as int?;
            営業担当者ＣＤ = dr["営業担当者ＣＤ", version] as int?;

            base.FromDataRow(dr, version);
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int? 得意先ＣＤ
        {
            get { return _得意先ＣＤ; }
            set { _得意先ＣＤ = value; }
        }

        public int? 状態区分
        {
            get { return _状態区分; }
            set { _状態区分 = value; }
        }

        public int? 失客理由
        {
            get { return _失客理由; }
            set { _失客理由 = value; }
        }

        public DateTime? 失客日
        {
            get { return _失客日; }
            set { _失客日 = value; }
        }

        public DateTime? 承認日
        {
            get { return _承認日; }
            set { _承認日 = value; }
        }

        public int? 承認者ＣＤ
        {
            get { return _承認者ＣＤ; }
            set { _承認者ＣＤ = value; }
        }

        public int? 営業担当者ＣＤ
        {
            get { return _営業担当者ＣＤ; }
            set { _営業担当者ＣＤ = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}]", 得意先ＣＤ); }
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
        /// DB登録
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Insert(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n INSERT INTO 得意先履歴テーブル(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,得意先別履歴番号" +
                         "\r\n     ,状態区分" +
                         "\r\n     ,失客理由" +
                         "\r\n     ,失客日" +
                         "\r\n     ,承認日" +
                         "\r\n     ,承認者ＣＤ" +
                         "\r\n     ,登録日" +
                         "\r\n     ,営業担当者ＣＤ" +
                         "\r\n     ,変更者ＣＤ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,(SELECT ISNULL(MAX(得意先別履歴番号), 0) + 1 FROM 得意先履歴テーブル WHERE 企業ＣＤ = @企業ＣＤ AND 得意先ＣＤ = @得意先ＣＤ)" +
                         "\r\n     ,@状態区分" +
                         "\r\n     ,@失客理由" +
                         "\r\n     ,@失客日" +
                         "\r\n     ,@承認日" +
                         "\r\n     ,@承認者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n     ,@営業担当者ＣＤ" +
                         "\r\n     ,@変更者ＣＤ" +
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
            throw new NotImplementedException("Update処理は存在しません。");
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            throw new NotImplementedException("Delete処理は存在しません。");
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

                    if (状態区分 != null && 状態区分 != -1)
                    {
                        cmd.Parameters.Add("@状態区分", SqlDbType.Int).Value = 状態区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@状態区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (失客理由 != null && 失客理由 != -1)
                    {
                        cmd.Parameters.Add("@失客理由", SqlDbType.Int).Value = 失客理由;
                    }
                    else
                    {
                        cmd.Parameters.Add("@失客理由", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (失客日 != null)
                    {
                        cmd.Parameters.Add("@失客日", SqlDbType.DateTime).Value = 失客日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@失客日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (承認日 != null)
                    {
                        cmd.Parameters.Add("@承認日", SqlDbType.DateTime).Value = 承認日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@承認日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (承認者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@承認者ＣＤ", SqlDbType.Int).Value = 承認者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@承認者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (営業担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@営業担当者ＣＤ", SqlDbType.Int).Value = 営業担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@営業担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@変更者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("得意先履歴テーブルを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "得意先別履歴番号が重複しています。";
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

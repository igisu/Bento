using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 売上ヘッダ
    /// </summary>
    public class UriageHeader : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="uriagebi">売上日</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns></returns>
        public static UriageHeader GetUriageHeader(int tokuisakiCD, DateTime uriagebi, int busyoCD)
        {
            UriageHeader uriageHeader = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                uriageHeader = GetUriageHeader(con, null, tokuisakiCD, uriagebi, busyoCD);
            }
            return uriageHeader;
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="uriagebi">売上日</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <returns></returns>
        public static UriageHeader GetUriageHeader(SqlConnection con, SqlTransaction tran, int tokuisakiCD, DateTime uriagebi, int busyoCD)
        {
            UriageHeader uriageHeader = null;

            string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.売上日" +
                         "\r\n     ,A.得意先ＣＤ" +
                         "\r\n     ,A.部署ＣＤ" +
                         "\r\n     ,A.得意先名" +
                         "\r\n     ,A.コースＣＤ" +
                         "\r\n     ,A.配送担当者ＣＤ" +
                         "\r\n     ,A3.担当者名 AS 配送担当者名" +
                         "\r\n     ,A.税区分" +
                         "\r\n     ,A.丸め処理区分" +
                         "\r\n     ,A.納品書伝票番号" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     売上ヘッダ AS A" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = @企業ＣＤ AND A.配送担当者ＣＤ = A3.担当者ＣＤ" +
                         "\r\n WHERE" +
                         "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                         "\r\n AND A.売上日     = @売上日" +
                         "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n AND A.部署ＣＤ   = @部署ＣＤ";

            SqlCommand cmd = con.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandText = sql;
            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
            cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = uriagebi;
            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;

            log.Debug(cmd.CommandTextWithParameters());

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("売上ヘッダ");
            adp.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                uriageHeader = new UriageHeader(dt.Rows[0]);
            }

            return uriageHeader;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private DateTime? _売上日;
        private int? _得意先ＣＤ;
        private int? _部署ＣＤ;
        private string _得意先名;
        private int? _コースＣＤ;
        private int? _配送担当者ＣＤ;
        private int? _税区分;
        private int? _丸め処理区分;
        private int? _納品書伝票番号;

        private string _配送担当者名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UriageHeader()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public UriageHeader(UriageHeader src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            売上日 = src.売上日;
            得意先ＣＤ = src.得意先ＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            得意先名 = src.得意先名;
            コースＣＤ = src.コースＣＤ;
            配送担当者ＣＤ = src.配送担当者ＣＤ;
            税区分 = src.税区分;
            丸め処理区分 = src.丸め処理区分;
            納品書伝票番号 = src.納品書伝票番号;

            配送担当者名 = src.配送担当者名;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public UriageHeader(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            売上日 = dr["売上日", version] as DateTime?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            得意先名 = dr["得意先名", version] as string;
            コースＣＤ = dr["コースＣＤ", version] as int?;
            配送担当者ＣＤ = dr["配送担当者ＣＤ", version] as int?;
            税区分 = dr["税区分", version] as int?;
            丸め処理区分 = dr["丸め処理区分", version] as int?;
            納品書伝票番号 = dr["納品書伝票番号", version] as int?;

            if (dr.Table.Columns.Contains("配送担当者名"))
            {
                配送担当者名 = dr["配送担当者名", version] as string;
            }

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 売上日
        {
            get { return _売上日; }
            set
            {
                _売上日 = value;
                _errors.Remove("売上日");
                if (value == null)
                {
                    _errors["売上日"] = "売上日を入力してください。";
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

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set { _コースＣＤ = value; }
        }

        public int? 配送担当者ＣＤ
        {
            get { return _配送担当者ＣＤ; }
            set
            {
                _配送担当者ＣＤ = value;
                _errors.Remove("配送担当者ＣＤ");
                配送担当者名 = null;
                if (value != null && _stopOverwrite == false)
                {
                    // 配送担当者ＣＤに該当する配送担当者名を取得
                    Tantousya tantousya = Tantousya.GetTantousya((int)_配送担当者ＣＤ);
                    if (tantousya != null)
                    {
                        配送担当者名 = tantousya.担当者名;
                    }
                    else
                    {
                        _errors["配送担当者ＣＤ"] = "配送担当者ＣＤが見つかりません。";
                    }
                }
            }
        }

        public int? 税区分
        {
            get { return _税区分; }
            set { _税区分 = value; }
        }

        public int? 丸め処理区分
        {
            get { return _丸め処理区分; }
            set { _丸め処理区分 = value; }
        }

        public int? 納品書伝票番号
        {
            get { return _納品書伝票番号; }
            set { _納品書伝票番号 = value; }
        }

        public string 配送担当者名
        {
            get { return _配送担当者名; }
            set { _配送担当者名 = value; }
        }

        public override string Code
        {
            get { return string.Format("売上日=[{0:yyyy/MM/dd}] 得意先ＣＤ=[{1}] 部署ＣＤ=[{2}]",  売上日, 得意先ＣＤ, 部署ＣＤ); }
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
        public bool Equals(UriageHeader target)
        {
            if (売上日 == target.売上日 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                部署ＣＤ == target.部署ＣＤ &&
                得意先名 == target.得意先名 &&
                (コースＣＤ ?? -1) == (target.コースＣＤ ?? -1) &&
                配送担当者ＣＤ == target.配送担当者ＣＤ &&
                (税区分 ?? -1) == (target.税区分 ?? -1) &&
                (丸め処理区分 ?? -1) == (target.丸め処理区分 ?? -1) &&
                納品書伝票番号 == target.納品書伝票番号)
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
            string sql = "\r\n INSERT INTO 売上ヘッダ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,売上日" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,得意先名" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,配送担当者ＣＤ" +
                         "\r\n     ,税区分" +
                         "\r\n     ,丸め処理区分" +
                         "\r\n     ,納品書伝票番号" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@売上日" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@得意先名" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@配送担当者ＣＤ" +
                         "\r\n     ,@税区分" +
                         "\r\n     ,@丸め処理区分" +
                         "\r\n     ,@納品書伝票番号" +
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
            string sql = "\r\n UPDATE 売上ヘッダ" +
                         "\r\n SET  得意先名       = @得意先名" +
                         "\r\n     ,コースＣＤ     = @コースＣＤ" +
                         "\r\n     ,配送担当者ＣＤ = @配送担当者ＣＤ" +
                         "\r\n     ,税区分         = @税区分" +
                         "\r\n     ,丸め処理区分   = @丸め処理区分" +
                         "\r\n     ,納品書伝票番号 = @納品書伝票番号" +
                         "\r\n     ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n     ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 売上日     = @売上日" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
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
            string sql = "\r\n DELETE FROM 売上ヘッダ" +
                         "\r\n WHERE 企業ＣＤ   = @企業ＣＤ" +
                         "\r\n   AND 売上日     = @売上日" +
                         "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                         "\r\n   AND 部署ＣＤ   = @部署ＣＤ" +
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

                    if (売上日 != null)
                    {
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = 売上日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (部署ＣＤ != null)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先名 != null)
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = 得意先名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (コースＣＤ != null && コースＣＤ != -1)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (配送担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@配送担当者ＣＤ", SqlDbType.Int).Value = 配送担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@配送担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (税区分 != null && 税区分 != -1)
                    {
                        cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = 税区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (丸め処理区分 != null && 丸め処理区分 != -1)
                    {
                        cmd.Parameters.Add("@丸め処理区分", SqlDbType.Int).Value = 丸め処理区分;
                    }
                    else
                    {
                        cmd.Parameters.Add("@丸め処理区分", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (納品書伝票番号 != null)
                    {
                        cmd.Parameters.Add("@納品書伝票番号", SqlDbType.Int).Value = 納品書伝票番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@納品書伝票番号", SqlDbType.Int).Value = DBNull.Value;
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

                    log.InfoFormat("売上ヘッダを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "得意先ＣＤが重複しています。";
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

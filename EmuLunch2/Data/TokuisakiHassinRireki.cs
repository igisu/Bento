using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 得意先発信履歴データ
    /// </summary>
    /// <remarks>
    /// このクラスは、得意先発信履歴データに対する受注発信一覧画面からの更新のみ（追加も削除もしない）を前提としています。
    /// さらに以下のような、他のテーブルとは異なる点があります。
    /// ・更新対象のレコードの処理中フラグが立っていない場合のみ更新可能としています。（フラグが立っていた場合は何もしない）
    /// ・更新は後優先（更新条件に修正日は加えない）
    /// ・不要フラグ／保留フラグ／コメント／最終発信日時／処理中フラグを更新するメソッドがある
    /// </remarks>
    public class TokuisakiHassinRireki : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private int? _得意先ＣＤ;
        private DateTime? _発信日;
        private string _電話番号;
        private DateTime? _最終発信日時;
        private int? _担当者ＣＤ;
        private bool? _処理中フラグ;
        private bool? _処理中フラグ条件;
        private bool? _不要フラグ;
        private bool? _保留フラグ;
        private string _コメント;
        private string _電話番号備考;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TokuisakiHassinRireki()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public TokuisakiHassinRireki(TokuisakiHassinRireki src)
        {
            得意先ＣＤ = src.得意先ＣＤ;
            発信日 = src.発信日;
            電話番号 = src.電話番号;
            最終発信日時 = src.最終発信日時;
            担当者ＣＤ = src.担当者ＣＤ;
            処理中フラグ = src.処理中フラグ;
            不要フラグ = src.不要フラグ;
            保留フラグ = src.保留フラグ;
            コメント = src.コメント;
            電話番号備考 = src.電話番号備考;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public TokuisakiHassinRireki(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;
            発信日 = dr["発信日", version] as DateTime?;
            電話番号 = dr["電話番号", version] as string;
            最終発信日時 = dr["最終発信日時", version] as DateTime?;
            担当者ＣＤ = dr["担当者ＣＤ", version] as int?;
            処理中フラグ = dr["処理中フラグ", version] as bool?;
            不要フラグ = dr["不要フラグ", version] as bool?;
            保留フラグ = dr["保留フラグ", version] as bool?;
            コメント = dr["コメント", version] as string;
            電話番号備考 = dr["電話番号備考", version] as string;

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

        public DateTime? 発信日
        {
            get { return _発信日; }
            set { _発信日 = value; }
        }

        public string 電話番号
        {
            get { return _電話番号; }
            set { _電話番号 = value; }
        }

        public DateTime? 最終発信日時
        {
            get { return _最終発信日時; }
            set { _最終発信日時 = value; }
        }

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public bool? 処理中フラグ
        {
            get { return _処理中フラグ; }
            set { _処理中フラグ = value; }
        }

        public bool? 処理中フラグ条件
        {
            get { return _処理中フラグ条件; }
            set { _処理中フラグ条件 = value; }
        }

        public bool? 不要フラグ
        {
            get { return _不要フラグ; }
            set { _不要フラグ = value; }
        }

        public bool? 保留フラグ
        {
            get { return _保留フラグ; }
            set { _保留フラグ = value; }
        }

        public string コメント
        {
            get { return _コメント; }
            set { _コメント = value; }
        }

        public string 電話番号備考
        {
            get { return _電話番号備考; }
            set { _電話番号備考 = value; }
        }

        public override string Code
        {
            get { return string.Format("得意先ＣＤ=[{0}] 発信日=[{1}]", 得意先ＣＤ, 発信日); }
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
        public bool Equals(TokuisakiHassinRireki target)
        {
            if (得意先ＣＤ == target.得意先ＣＤ &&
                発信日 == target.発信日 &&
                電話番号 == target.電話番号 &&
                最終発信日時 == target.最終発信日時 &&
                担当者ＣＤ == target.担当者ＣＤ &&
                (処理中フラグ ?? false) == (target.処理中フラグ ?? false) &&
                (不要フラグ ?? false) == (target.不要フラグ ?? false) &&
                (保留フラグ ?? false) == (target.保留フラグ ?? false) &&
                コメント == target.コメント &&
                電話番号備考 == target.電話番号備考)
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
            string sql = "\r\n INSERT INTO 得意先発信履歴データ (" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,発信日" +
                         "\r\n     ,電話番号" +
                         "\r\n     ,最終発信日時" +
                         "\r\n     ,担当者ＣＤ" +
                         "\r\n     ,処理中フラグ" +
                         "\r\n     ,不要フラグ" +
                         "\r\n     ,保留フラグ" +
                         "\r\n     ,コメント" +
                         "\r\n     ,電話番号備考" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@発信日" +
                         "\r\n     ,@電話番号" +
                         "\r\n     ,@最終発信日時" +
                         "\r\n     ,@担当者ＣＤ" +
                         "\r\n     ,@処理中フラグ" +
                         "\r\n     ,@不要フラグ" +
                         "\r\n     ,@保留フラグ" +
                         "\r\n     ,@コメント" +
                         "\r\n     ,@電話番号備考" +
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
            string sql = "\r\n UPDATE 得意先発信履歴データ" +
                         "\r\n SET 最終発信日時   =  @最終発信日時" +
                         "\r\n    ,担当者ＣＤ     =  @担当者ＣＤ" +
                         "\r\n    ,処理中フラグ   =  @処理中フラグ" +
                         "\r\n    ,不要フラグ     =  @不要フラグ" +
                         "\r\n    ,保留フラグ     =  @保留フラグ" +
                         "\r\n    ,コメント       =  @コメント" +
                         "\r\n    ,電話番号備考   =  @電話番号備考" +
                         "\r\n    ,修正担当者ＣＤ =  @修正担当者ＣＤ" +
                         "\r\n    ,修正日         =  getdate()" +
                         "\r\n WHERE 企業ＣＤ     =  @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ   =  @得意先ＣＤ" +
                         "\r\n   AND 発信日       =  @発信日" +
                         "\r\n   AND 電話番号     =  @電話番号" +
                         "\r\n   AND 処理中フラグ =  @処理中フラグ条件";

            return Save(con, tran, sql, History.ProcessType.Update);
        }

        /// <summary>
        /// DB更新（不要フラグ）
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool UpdateFuyoFlg()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" UPDATE 得意先発信履歴データ");
                    sql.AppendLine(" SET 担当者ＣＤ     =  @担当者ＣＤ");
                    sql.AppendLine("    ,処理中フラグ   =  @処理中フラグ");
                    sql.AppendLine("    ,不要フラグ     =  @不要フラグ");
                    sql.AppendLine("    ,修正担当者ＣＤ =  @修正担当者ＣＤ");
                    sql.AppendLine("    ,修正日         =  getdate()");
                    sql.AppendLine(" WHERE 企業ＣＤ     =  @企業ＣＤ");
                    sql.AppendLine("   AND 得意先ＣＤ   =  @得意先ＣＤ");
                    sql.AppendLine("   AND 発信日       =  @発信日");
                    sql.AppendLine("   AND 電話番号     =  @電話番号");
                    sql.AppendLine("   AND 処理中フラグ =  @処理中フラグ条件");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (発信日 != null)
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = 発信日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理中フラグ != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = 処理中フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理中フラグ条件 != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = 処理中フラグ条件;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = false;
                    }

                    if (不要フラグ != null)
                    {
                        cmd.Parameters.Add("@不要フラグ", SqlDbType.Bit).Value = 不要フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@不要フラグ", SqlDbType.Bit).Value = false;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        return false;
                    }
                    log.InfoFormat("得意先発信履歴データ（不要フラグ）を{0}しました。", History.ProcessType.Update.DisplayName());
                    return true;
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

        /// <summary>
        /// DB更新（保留フラグ）
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool UpdateHoryuFlg()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" UPDATE 得意先発信履歴データ");
                    sql.AppendLine(" SET 担当者ＣＤ     =  @担当者ＣＤ");
                    sql.AppendLine("    ,処理中フラグ   =  @処理中フラグ");
                    sql.AppendLine("    ,保留フラグ     =  @保留フラグ");
                    sql.AppendLine("    ,修正担当者ＣＤ =  @修正担当者ＣＤ");
                    sql.AppendLine("    ,修正日         =  getdate()");
                    sql.AppendLine(" WHERE 企業ＣＤ     =  @企業ＣＤ");
                    sql.AppendLine("   AND 得意先ＣＤ   =  @得意先ＣＤ");
                    sql.AppendLine("   AND 発信日       =  @発信日");
                    sql.AppendLine("   AND 電話番号     =  @電話番号");
                    sql.AppendLine("   AND 処理中フラグ =  @処理中フラグ条件");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (発信日 != null)
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = 発信日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理中フラグ != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = 処理中フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理中フラグ条件 != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = 処理中フラグ条件;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = false;
                    }

                    if (保留フラグ != null)
                    {
                        cmd.Parameters.Add("@保留フラグ", SqlDbType.Bit).Value = 保留フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@保留フラグ", SqlDbType.Bit).Value = false;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        return false;
                    }
                    log.InfoFormat("得意先発信履歴データ（保留フラグ）を{0}しました。", History.ProcessType.Update.DisplayName());
                    return true;
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

        /// <summary>
        /// DB更新（コメント）
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool UpdateComment()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" UPDATE 得意先発信履歴データ");
                    sql.AppendLine(" SET 担当者ＣＤ     =  @担当者ＣＤ");
                    sql.AppendLine("    ,処理中フラグ   =  @処理中フラグ");
                    sql.AppendLine("    ,コメント       =  @コメント");
                    sql.AppendLine("    ,修正担当者ＣＤ =  @修正担当者ＣＤ");
                    sql.AppendLine("    ,修正日         =  getdate()");
                    sql.AppendLine(" WHERE 企業ＣＤ     =  @企業ＣＤ");
                    sql.AppendLine("   AND 得意先ＣＤ   =  @得意先ＣＤ");
                    sql.AppendLine("   AND 発信日       =  @発信日");
                    sql.AppendLine("   AND 電話番号     =  @電話番号");
                    sql.AppendLine("   AND 処理中フラグ =  @処理中フラグ条件");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (発信日 != null)
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = 発信日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理中フラグ != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = 処理中フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理中フラグ条件 != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = 処理中フラグ条件;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = false;
                    }

                    if (コメント != null)
                    {
                        cmd.Parameters.Add("@コメント", SqlDbType.VarChar).Value = コメント;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コメント", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        return false;
                    }
                    log.InfoFormat("得意先発信履歴データ（コメント）を{0}しました。", History.ProcessType.Update.DisplayName());
                    return true;
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

        /// <summary>
        /// DB更新（最終発信日時）
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool UpdateLastUpdateTime()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" UPDATE 得意先発信履歴データ");
                    sql.AppendLine(" SET 担当者ＣＤ     =  @担当者ＣＤ");
                    sql.AppendLine("    ,処理中フラグ   =  @処理中フラグ");
                    sql.AppendLine("    ,最終発信日時   =  @最終発信日時");
                    sql.AppendLine("    ,修正担当者ＣＤ =  @修正担当者ＣＤ");
                    sql.AppendLine("    ,修正日         =  getdate()");
                    sql.AppendLine(" WHERE 企業ＣＤ     =  @企業ＣＤ");
                    sql.AppendLine("   AND 得意先ＣＤ   =  @得意先ＣＤ");
                    sql.AppendLine("   AND 発信日       =  @発信日");
                    sql.AppendLine("   AND 電話番号     =  @電話番号");
                    sql.AppendLine("   AND 処理中フラグ =  @処理中フラグ条件");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (発信日 != null)
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = 発信日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理中フラグ != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = 処理中フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理中フラグ条件 != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = 処理中フラグ条件;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = false;
                    }

                    if (最終発信日時 != null)
                    {
                        cmd.Parameters.Add("@最終発信日時", SqlDbType.DateTime).Value = 最終発信日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@最終発信日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        return false;
                    }
                    log.InfoFormat("得意先発信履歴データ（最終発信日時）を{0}しました。", History.ProcessType.Update.DisplayName());
                    return true;
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

        /// <summary>
        /// DB更新（処理中フラグ）
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public bool UpdateSyorichu()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" UPDATE 得意先発信履歴データ");
                    sql.AppendLine(" SET 処理中フラグ   =  @処理中フラグ");
                    sql.AppendLine("    ,修正担当者ＣＤ =  @修正担当者ＣＤ");
                    sql.AppendLine("    ,修正日         =  getdate()");
                    sql.AppendLine(" WHERE 企業ＣＤ     =  @企業ＣＤ");
                    sql.AppendLine("   AND 得意先ＣＤ   =  @得意先ＣＤ");
                    sql.AppendLine("   AND 発信日       =  @発信日");
                    sql.AppendLine("   AND 電話番号     =  @電話番号");
                    sql.AppendLine("   AND 処理中フラグ =  @処理中フラグ条件");

                    // 処理中フラグを落とす場合は、自分が立てたものだけを落とす（他人が立てたものは落としてはダメ）
                    if (処理中フラグ == null || 処理中フラグ == false)
                    {
                        sql.AppendLine("   AND 修正担当者ＣＤ =  @修正担当者ＣＤ");
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = 企業ＣＤ;

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (発信日 != null)
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = 発信日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (処理中フラグ != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = 処理中フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理中フラグ条件 != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = 処理中フラグ条件;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = false;
                    }

                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        return false;
                    }
                    log.InfoFormat("得意先発信履歴データ（処理中フラグ）を{0}しました。", History.ProcessType.Update.DisplayName());
                    return true;
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

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            string sql = "\r\n DELETE FROM 得意先発信履歴データ" +
                         "\r\n WHERE 企業ＣＤ   =  @企業ＣＤ" +
                         "\r\n   AND 得意先ＣＤ =  @得意先ＣＤ" +
                         "\r\n   AND 発信日     =  @発信日" +
                         "\r\n   AND 電話番号   =  @電話番号" +
                         "\r\n   AND 修正日     =  @修正日";

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

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (発信日 != null)
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = 発信日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@発信日", SqlDbType.Date).Value = DBNull.Value;
                    }

                    if (電話番号 != null)
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = 電話番号;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (最終発信日時 != null)
                    {
                        cmd.Parameters.Add("@最終発信日時", SqlDbType.DateTime).Value = 最終発信日時;
                    }
                    else
                    {
                        cmd.Parameters.Add("@最終発信日時", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = 担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (処理中フラグ != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = 処理中フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (処理中フラグ条件 != null)
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = 処理中フラグ条件;
                    }
                    else
                    {
                        cmd.Parameters.Add("@処理中フラグ条件", SqlDbType.Bit).Value = false;
                    }

                    if (不要フラグ != null)
                    {
                        cmd.Parameters.Add("@不要フラグ", SqlDbType.Bit).Value = 不要フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@不要フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (保留フラグ != null)
                    {
                        cmd.Parameters.Add("@保留フラグ", SqlDbType.Bit).Value = 保留フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@保留フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (コメント != null)
                    {
                        cmd.Parameters.Add("@コメント", SqlDbType.VarChar).Value = コメント;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コメント", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (電話番号備考 != null)
                    {
                        cmd.Parameters.Add("@電話番号備考", SqlDbType.VarChar).Value = 電話番号備考;
                    }
                    else
                    {
                        cmd.Parameters.Add("@電話番号備考", SqlDbType.VarChar).Value = DBNull.Value;
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

                    log.InfoFormat("得意先発信履歴データを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "得意先ＣＤと発信日が重複しています。";
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

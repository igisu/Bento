using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 臨時コーステーブル
    /// </summary>
    public class RinjiCourseTable : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="date">実施日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static RinjiCourseTable GetRinjiCourseTable(int courseCD, DateTime date, int tokuisakiCD)
        {
            return GetRinjiCourseTable(courseCD, date, tokuisakiCD, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="date">実施日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <returns></returns>
        public static RinjiCourseTable GetRinjiCourseTableReload(int courseCD, DateTime date, int tokuisakiCD)
        {
            return GetRinjiCourseTable(courseCD, date, tokuisakiCD, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="date">実施日</param>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static RinjiCourseTable GetRinjiCourseTable(int courseCD, DateTime date, int tokuisakiCD, bool isValid)
        {
            RinjiCourseTable courseTable = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.コースＣＤ" +
                             "\r\n     ,A.実施日" +
                             "\r\n     ,A.得意先ＣＤ" +
                             "\r\n     ,A.コース順" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     臨時コーステーブル AS A" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.コースＣＤ = @コースＣＤ" +
                             "\r\n AND A.実施日     = @実施日" +
                             "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                             "{0}";

                string where = "";
                if (isValid)
                {
                    where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
                }

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(sql, where);
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = date;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("臨時コーステーブル");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    courseTable = new RinjiCourseTable(dt.Rows[0]);
                }
            }

            return courseTable;
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _コースＣＤ;
        private DateTime? _実施日;
        private int? _得意先ＣＤ;
        private int? _得意先ＣＤorg;            // レコード読込直後の値（更新・削除時に使用する）
        private int? _コース順;
        private bool? _無効フラグ;

        private string _得意先名;
        private string _得意先名略称;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RinjiCourseTable()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public RinjiCourseTable(RinjiCourseTable src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            コースＣＤ = src.コースＣＤ;
            実施日 = src.実施日;
            得意先ＣＤ = src.得意先ＣＤ;
            得意先ＣＤorg = src.得意先ＣＤorg;
            コース順 = src.コース順;
            無効フラグ = src.無効フラグ;

            得意先名 = src.得意先名;
            得意先名略称 = src.得意先名略称;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// コピーコンストラクタ（コーステーブルから）
        /// </summary>
        /// <param name="src"></param>
        public RinjiCourseTable(CourseTable src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            コースＣＤ = src.コースＣＤ;
            得意先ＣＤ = src.得意先ＣＤ;
            得意先ＣＤorg = src.得意先ＣＤ;
            コース順 = src.コース順;
            無効フラグ = src.無効フラグ;

            得意先名 = src.得意先名;
            得意先名略称 = src.得意先名略称;

            base.Copy(src);

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public RinjiCourseTable(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            コースＣＤ = dr["コースＣＤ", version] as int?;
            実施日 = dr["実施日", version] as DateTime?;
            得意先ＣＤ = dr["得意先ＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("得意先ＣＤorg"))
            {
                得意先ＣＤorg = dr["得意先ＣＤorg", version] as int?;
            }
            else
            {
                得意先ＣＤorg = dr["得意先ＣＤ", version] as int?;
            }

            コース順 = dr["コース順", version] as int?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("得意先名"))
            {
                得意先名 = dr["得意先名", version] as string;
            }

            if (dr.Table.Columns.Contains("得意先名略称"))
            {
                得意先名略称 = dr["得意先名略称", version] as string;
            }

            base.FromDataRow(dr, version);

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
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

        public DateTime? 実施日
        {
            get { return _実施日; }
            set
            {
                _実施日 = value;
                _errors.Remove("実施日");
                if (value == null)
                {
                    _errors["実施日"] = "実施日を入力してください。";
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
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // 得意先ＣＤに該当する得意先名を取得
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            得意先名 = tokuisaki.得意先名;
                            得意先名略称 = tokuisaki.得意先名略称;
                        }
                        else
                        {
                            _errors["得意先ＣＤ"] = "得意先ＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public int? コース順
        {
            get { return _コース順; }
            set { _コース順 = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string 得意先名
        {
            get { return _得意先名; }
            set { _得意先名 = value; }
        }

        public string 得意先名略称
        {
            get { return _得意先名略称; }
            set { _得意先名略称 = value; }
        }

        public int? 得意先ＣＤorg
        {
            get { return _得意先ＣＤorg; }
            set { _得意先ＣＤorg = value; }
        }

        public override string Code
        {
            get { return string.Format("コースＣＤ=[{0}] 実施日=[{1}] 得意先ＣＤ=[{2}]", コースＣＤ, 実施日, (得意先ＣＤ == 得意先ＣＤorg ? 得意先ＣＤ : 得意先ＣＤorg ?? 得意先ＣＤ)); }
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
        /// データ行への設定
        /// </summary>
        /// <param name="dr"></param>
        public void SetDataRow(DataRow dr)
        {
            // DataRow への設定は SetField メソッドを使って書くと DBNull を意識しなくて済む
            dr.SetField<int?>("コースＣＤ", _コースＣＤ);
            dr.SetField<DateTime?>("実施日", _実施日);
            dr.SetField<int?>("得意先ＣＤ", _得意先ＣＤ);
            dr.SetField<int?>("コース順", _コース順);
            dr.SetField<bool?>("無効フラグ", _無効フラグ);

            if (dr.Table.Columns.Contains("得意先名"))
            {
                dr.SetField<string>("得意先名", _得意先名);
            }

            if (dr.Table.Columns.Contains("得意先名略称"))
            {
                dr.SetField<string>("得意先名略称", _得意先名略称);
            }

            base.ToDataRow(dr);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(RinjiCourseTable target)
        {
            if (コースＣＤ == target.コースＣＤ &&
                実施日 == target.実施日 &&
                得意先ＣＤ == target.得意先ＣＤ &&
                コース順 == target.コース順 &&
                (無効フラグ ?? false) == (target.無効フラグ ?? false))
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
            string sql = "\r\n INSERT INTO 臨時コーステーブル(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,実施日" +
                         "\r\n     ,得意先ＣＤ" +
                         "\r\n     ,コース順" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@実施日" +
                         "\r\n     ,@得意先ＣＤ" +
                         "\r\n     ,@コース順" +
                         "\r\n     ,@無効フラグ" +
                         "\r\n     ,@作成担当者ＣＤ" +
                         "\r\n     ,ISNULL(@作成日, getdate())" +
                         "\r\n     ,@修正担当者ＣＤ" +
                         "\r\n     ,getdate()" +
                         "\r\n ) ";

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
            string sql = "\r\n UPDATE 臨時コーステーブル" +
                         "\r\n SET 得意先ＣＤ     = @得意先ＣＤ" +
                         "\r\n    ,コース順       = @コース順" +
                         "\r\n    ,無効フラグ     = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ = @修正担当者ＣＤ" +
                         "\r\n    ,修正日         = getdate()" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ   = @コースＣＤ" +
                         "\r\n   AND 実施日       = @実施日" +
                         "\r\n   AND 得意先ＣＤ   = @得意先ＣＤorg" +
                         "\r\n   AND 修正日       = @修正日";

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
            string sql = "\r\n DELETE FROM 臨時コーステーブル" +
                         "\r\n WHERE 企業ＣＤ     = @企業ＣＤ" +
                         "\r\n   AND コースＣＤ   = @コースＣＤ" +
                         "\r\n   AND 実施日       = @実施日" +
                         "\r\n   AND 得意先ＣＤ   = @得意先ＣＤorg" +
                         "\r\n   AND 修正日       = @修正日";

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

                    if (実施日 != null)
                    {
                        cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = 実施日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@実施日", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤ != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = 得意先ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (得意先ＣＤorg != null)
                    {
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = 得意先ＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@得意先ＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コース順 != null)
                    {
                        cmd.Parameters.Add("@コース順", SqlDbType.Int).Value = コース順;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コース順", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = false;
                    }

                    if (作成日 != null)
                    {
                        cmd.Parameters.Add("@作成日", SqlDbType.DateTime).Value = 作成日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@作成日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    if (修正日 != null)
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = 修正日;
                    }
                    else
                    {
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = DBNull.Value;
                    }

                    UserData userData = UserData.getInstance();
                    if (作成担当者ＣＤ != null)
                    {
                        cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = 作成担当者ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                    }
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        tran.Rollback();
                        Message = "別の端末で更新済みです。再度検索してください。";
                        return false;
                    }

                    log.InfoFormat("臨時コーステーブルを{0}しました。", historyProcess.DisplayName());
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

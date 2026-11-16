using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コース別グループマスタ（明細部）
    /// </summary>
    public class CourseGroupDetail : CourseGroup, IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        private bool _stopOverwrite;            // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private int? _コースＣＤ;
        private int? _コースＣＤorg;            // レコード読込直後の値（更新・削除時に使用する）
        private int? _グループＳＥＱ;
        private bool? _無効フラグ;


        private string _コース名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CourseGroupDetail()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public CourseGroupDetail(CourseGroupDetail src) : base(src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            コースＣＤ = src.コースＣＤ;
            コースＣＤorg = src.コースＣＤorg;
            グループＳＥＱ = src.グループＳＥＱ;
            無効フラグ = src.無効フラグ;

            コース名 = src.コース名;

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public CourseGroupDetail(DataRow dr) : base(dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            コースＣＤ = dr["コースＣＤ", version] as int?;

            if (dr.Table.Columns.Contains("コースＣＤorg"))
            {
                コースＣＤorg = dr["コースＣＤorg", version] as int?;
            }
            else
            {
                コースＣＤorg = dr["コースＣＤ", version] as int?;
            }

            グループＳＥＱ = dr["グループＳＥＱ", version] as int?;
            無効フラグ = dr["無効フラグ", version] as bool?;

            if (dr.Table.Columns.Contains("コース名"))
            {
                コース名 = dr["コース名", version] as string;
            }

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
                else
                {
                    if (_stopOverwrite == false)
                    {
                        // コースＣＤに該当するコース名を取得
                        Course course = Course.GetCourse((int)_コースＣＤ);
                        if (course != null)
                        {
                            コース名 = course.コース名;
                        }
                        else
                        {
                            _errors["コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
            }
        }

        public int? コースＣＤorg
        {
            get { return _コースＣＤorg; }
            set { _コースＣＤorg = value; }
        }

        public int? グループＳＥＱ
        {
            get { return _グループＳＥＱ; }
            set { _グループＳＥＱ = value; }
        }

        public bool? 無効フラグ
        {
            get { return _無効フラグ; }
            set { _無効フラグ = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] グループ種別ＣＤ=[{1}] コースＣＤ=[{2}]", 部署ＣＤ, グループ種別ＣＤ, (コースＣＤ == コースＣＤorg ? コースＣＤ : コースＣＤorg ?? コースＣＤ)); }
        }

        public new bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public new string Error
        {
            get{ return null;}
        }

        public new string this[string columnName]
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
        public new void SetDataRow(DataRow dr)
        {
            base.SetDataRow(dr);

            // DataRow への設定は SetField メソッドを使って書くと DBNull を意識しなくて済む
            dr.SetField<int?>("コースＣＤ", _コースＣＤ);
            if (dr.Table.Columns.Contains("コースＣＤorg"))
            {
                dr.SetField<int?>("コースＣＤorg", _コースＣＤorg);
            }
            dr.SetField<int?>("グループＳＥＱ", _グループＳＥＱ);
            dr.SetField<bool?>("無効フラグ", _無効フラグ);

            if (dr.Table.Columns.Contains("コース名"))
            {
                dr.SetField<string>("コース名", _コース名);
            }

            base.ToDataRow(dr);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(CourseGroupDetail target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                グループ種別ＣＤ == target.グループ種別ＣＤ &&
                コースＣＤ == target.コースＣＤ &&
                グループ種別名 == target.グループ種別名 &&
                グループＳＥＱ == target.グループＳＥＱ &&
                無効フラグ == target.無効フラグ)
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
            string sql = "\r\n INSERT INTO コース別グループマスタ(" +
                         "\r\n     企業ＣＤ" +
                         "\r\n     ,部署ＣＤ" +
                         "\r\n     ,グループ種別ＣＤ" +
                         "\r\n     ,コースＣＤ" +
                         "\r\n     ,グループ種別名" +
                         "\r\n     ,グループＳＥＱ" +
                         "\r\n     ,無効フラグ" +
                         "\r\n     ,作成担当者ＣＤ" +
                         "\r\n     ,作成日" +
                         "\r\n     ,修正担当者ＣＤ" +
                         "\r\n     ,修正日" +
                         "\r\n ) VALUES (" +
                         "\r\n     @企業ＣＤ" +
                         "\r\n     ,@部署ＣＤ" +
                         "\r\n     ,@グループ種別ＣＤ" +
                         "\r\n     ,@コースＣＤ" +
                         "\r\n     ,@グループ種別名" +
                         "\r\n     ,@グループＳＥＱ" +
                         "\r\n     ,@無効フラグ" +
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
            string sql = "\r\n UPDATE コース別グループマスタ" +
                         "\r\n SET コースＣＤ         = @コースＣＤ" +
                         "\r\n    ,グループ種別名     = @グループ種別名" +
                         "\r\n    ,グループＳＥＱ     = @グループＳＥＱ" +
                         "\r\n    ,無効フラグ         = @無効フラグ" +
                         "\r\n    ,修正担当者ＣＤ     = @修正担当者ＣＤ" +
                         "\r\n    ,修正日             = getdate()" +
                         "\r\n WHERE 企業ＣＤ         = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ         = @部署ＣＤ" +
                         "\r\n   AND グループ種別ＣＤ = @グループ種別ＣＤ" +
                         "\r\n   AND コースＣＤ       = @コースＣＤorg" +
                         "\r\n   AND 修正日           = @修正日";

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
            string sql = "\r\n DELETE FROM コース別グループマスタ" +
                         "\r\n WHERE 企業ＣＤ         = @企業ＣＤ" +
                         "\r\n   AND 部署ＣＤ         = @部署ＣＤ" +
                         "\r\n   AND グループ種別ＣＤ = @グループ種別ＣＤ" +
                         "\r\n   AND コースＣＤ       = @コースＣＤorg" +
                         "\r\n   AND 修正日           = @修正日";

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

                    if (部署ＣＤ != null && 部署ＣＤ != -1)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = 部署ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (グループ種別ＣＤ != null)
                    {
                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = グループ種別ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コースＣＤ != null)
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = コースＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (コースＣＤorg != null)
                    {
                        cmd.Parameters.Add("@コースＣＤorg", SqlDbType.Int).Value = コースＣＤorg;
                    }
                    else
                    {
                        cmd.Parameters.Add("@コースＣＤorg", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (グループ種別名 != null)
                    {
                        cmd.Parameters.Add("@グループ種別名", SqlDbType.VarChar).Value = グループ種別名;
                    }
                    else
                    {
                        cmd.Parameters.Add("@グループ種別名", SqlDbType.VarChar).Value = DBNull.Value;
                    }

                    if (グループＳＥＱ != null)
                    {
                        cmd.Parameters.Add("@グループＳＥＱ", SqlDbType.Int).Value = グループＳＥＱ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@グループＳＥＱ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    if (無効フラグ != null)
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = 無効フラグ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@無効フラグ", SqlDbType.Bit).Value = false;
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

                    log.InfoFormat("コース別グループマスタを{0}しました。", historyProcess.DisplayName());
                    return true;

                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        Message = "コースＣＤが重複しています。";
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

using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// コース別グループマスタ（ヘッダ部）
    /// </summary>
    public class CourseGroup : TableBase2, IDataErrorInfo
    {
        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するマスタを返します（無効のものは取得しない）
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        /// <param name="gruopSyubetsuCD">グループ種別ＣＤ</param>
        /// <returns></returns>
        public static CourseGroup GetCourseGroup(int busyoCD, int gruopSyubetsuCD)
        {
            return GetCourseGroup(busyoCD, gruopSyubetsuCD, true);
        }

        /// <summary>
        /// コードに該当するマスタを返します（無効のものでも取得する）
        /// </summary>
        /// <param name="busyoCD">得意先ＣＤ</param>
        /// <param name="gruopSyubetsuCD">グループ種別ＣＤ</param>
        /// <returns></returns>
        public static CourseGroup GetCourseGroupReload(int busyoCD, int gruopSyubetsuCD)
        {
            return GetCourseGroup(busyoCD, gruopSyubetsuCD, false);
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="busyoCD">得意先ＣＤ</param>
        /// <param name="gruopSyubetsuCD">グループ種別ＣＤ</param>
        /// <param name="isValid">無効のものを取得しない場合はtrue</param>
        /// <returns></returns>
        private static CourseGroup GetCourseGroup(int busyoCD, int gruopSyubetsuCD, bool isValid)
        {
            CourseGroup courseGroup = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     G.企業ＣＤ" +
                             "\r\n     ,G.部署ＣＤ" +
                             "\r\n     ,G.グループ種別ＣＤ" +
                             "\r\n     ,G.グループ種別名" +
                             "\r\n     ,MAX(G.作成担当者ＣＤ) AS 作成担当者ＣＤ" +
                             "\r\n     ,MAX(A1.担当者名) AS 作成担当者名" +
                             "\r\n     ,MAX(G.作成日) AS 作成日" +
                             "\r\n     ,MAX(G.修正担当者ＣＤ) AS 修正担当者ＣＤ" +
                             "\r\n     ,MAX(A2.担当者名) AS 修正担当者名" +
                             "\r\n     ,MAX(G.修正日) AS 修正日" +
                             "\r\n FROM" +
                             "\r\n     コース別グループマスタ AS G" +
                             "\r\n     LEFT JOIN コースマスタ AS M ON M.企業ＣＤ = @企業ＣＤ AND G.コースＣＤ = M.コースＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND G.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND G.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n WHERE G.企業ＣＤ = @企業ＣＤ" +
                             "\r\n   AND G.部署ＣＤ = @部署ＣＤ" +
                             "\r\n   AND G.グループ種別ＣＤ = @グループ種別ＣＤ" +
                             "\r\n   {0}" +
                             "\r\n GROUP BY" +
                             "\r\n     G.企業ＣＤ" +
                             "\r\n     ,G.部署ＣＤ" +
                             "\r\n     ,G.グループ種別ＣＤ" +
                             "\r\n     ,G.グループ種別名";

                string where = "";
                if (isValid)
                {
                    where = "\r\n AND ISNULL(A.無効フラグ, 0) = 0";
                }

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = string.Format(sql, where);
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = _企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = gruopSyubetsuCD;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コース別グループマスタ");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    courseGroup = new CourseGroup(dt.Rows[0]);
                }
            }

            return courseGroup;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        protected int? _部署ＣＤ;
        protected int? _グループ種別ＣＤ;
        protected string _グループ種別名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CourseGroup()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public CourseGroup(CourseGroup src)
        {
            部署ＣＤ = src.部署ＣＤ;
            グループ種別ＣＤ = src.グループ種別ＣＤ;
            グループ種別名 = src.グループ種別名;

            base.Copy(src);
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public CourseGroup(DataRow dr)
        {
            // 削除済みの場合は元の値を取得する
            DataRowVersion version = DataRowVersion.Default;
            if (dr.RowState == DataRowState.Deleted)
            {
                version = DataRowVersion.Original;
            }

            部署ＣＤ = dr["部署ＣＤ", version] as int?;
            グループ種別ＣＤ = dr["グループ種別ＣＤ", version] as int?;
            グループ種別名 = dr["グループ種別名", version] as string;

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

        public int? グループ種別ＣＤ
        {
            get { return _グループ種別ＣＤ; }
            set
            {
                _グループ種別ＣＤ = value;
                _errors.Remove("グループ種別ＣＤ");
                if (value == null)
                {
                    _errors["グループ種別ＣＤ"] = "グループ種別ＣＤを入力してください。";
                }
            }
        }

        public string グループ種別名
        {
            get { return _グループ種別名; }
            set
            {
                _グループ種別名 = value;
                _errors.Remove("グループ種別名");
                if (value == null)
                {
                    _errors["グループ種別名"] = "グループ種別名を入力してください。";
                }
            }
        }

        public override string Code
        {
            get { return string.Format("部署ＣＤ=[{0}] グループ種別ＣＤ=[{1}] ", 部署ＣＤ, グループ種別ＣＤ); }
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
            dr.SetField<int?>("部署ＣＤ", _部署ＣＤ);
            dr.SetField<int?>("グループ種別ＣＤ", _グループ種別ＣＤ);
            dr.SetField<string>("グループ種別名", _グループ種別名);

            base.ToDataRow(dr);
        }

        /// <summary>
        /// 指定した Object が、現在の Object と等しいかどうかを判断します。
        /// </summary>
        /// <param name="target">現在のオブジェクトと比較するオブジェクト。 </param>
        /// <returns> 指定した Object が現在の Object と等しい場合は true。それ以外の場合は false。</returns>
        public bool Equals(CourseGroup target)
        {
            if ((部署ＣＤ ?? -1) == (target.部署ＣＤ ?? -1) &&
                グループ種別ＣＤ == target.グループ種別ＣＤ &&
                グループ種別名 == target.グループ種別名)
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
            throw new NotSupportedException("CourseGroupDetail#Insert()を使用してください。");
        }

        /// <summary>
        /// DB更新
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Update(SqlConnection con, SqlTransaction tran)
        {
            throw new NotSupportedException("CourseGroupDetail#Update()を使用してください。");
        }

        /// <summary>
        /// DB削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        public override bool Delete(SqlConnection con, SqlTransaction tran)
        {
            throw new NotSupportedException("CourseGroupDetail#Delete()を使用してください。");
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
            throw new NotSupportedException("CourseGroupDetail#Save()を使用してください。");
        }

        #endregion
    }
}

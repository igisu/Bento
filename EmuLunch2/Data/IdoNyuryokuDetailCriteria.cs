using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Emu.Bento.Common;
using log4net;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 移動入力詳細検索条件
    /// </summary>
    public class IdoNyuryokuDetailCriteria : IDataErrorInfo
    {
        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* 静的メソッド
        //**************************************
        #region 静的メソッド
        /// <summary>
        /// コードに該当するコース情報を返します
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="haisobi">配送日</param>
        /// <returns></returns>
        public static IdoNyuryokuDetailCriteria GetIdoNyuryokuDetailCriteria(int courseCD, DateTime haisobi)
        {
            IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteria = null;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.コースＣＤ" +
                             "\r\n     ,A.コース名" +
                             "\r\n     ,A.コース区分" +
                             "\r\n     ,VK.コース区分名" +
                             "\r\n     ,BM.支店ＣＤ" +
                             "\r\n     ,SM.支店名" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,BM.部署名" +
                             "\r\n FROM" +
                             // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                             "\r\n     (SELECT" +
                             "\r\n         C.企業ＣＤ" +
                             "\r\n         ,C.コースＣＤ" +
                             "\r\n         ,C.コース区分" +
                             "\r\n         ,C.コース名" +
                             "\r\n         ,C.部署ＣＤ" +
                             "\r\n     FROM" +
                             "\r\n         コースマスタ AS C" +
                             "\r\n         LEFT JOIN" +
                             "\r\n             (SELECT" +
                             "\r\n                 コースＣＤ" +
                             "\r\n             FROM" +
                             "\r\n                 臨時コースマスタ" +
                             "\r\n             WHERE" +
                             "\r\n                 企業ＣＤ = @企業ＣＤ" +
                             "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                             "\r\n                 AND コースＣＤ = @コースＣＤ" +
                             "\r\n                 AND 実施日 = @注文日" +
                             "\r\n             ) AS RM" +
                             "\r\n             ON RM.コースＣＤ = C.コースＣＤ" +
                             "\r\n     WHERE" +
                             "\r\n         C.企業ＣＤ = @企業ＣＤ" +
                             "\r\n         AND C.コースＣＤ = @コースＣＤ" +
                             "\r\n         AND ISNULL(C.無効フラグ, 0) = 0" +
                             "\r\n         AND RM.コースＣＤ IS NULL" +
                             // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                             "\r\n     UNION" +
                             "\r\n     SELECT" +
                             "\r\n         RM.企業ＣＤ" +
                             "\r\n         ,RM.コースＣＤ" +
                             "\r\n         ,RM.コース区分" +
                             "\r\n         ,RM.コース名" +
                             "\r\n         ,RM.部署ＣＤ" +
                             "\r\n     FROM" +
                             "\r\n         臨時コーステーブル AS RT" +
                             "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                             "\r\n     WHERE" +
                             "\r\n         RM.企業ＣＤ = @企業ＣＤ" +
                             "\r\n         AND RM.コースＣＤ = @コースＣＤ" +
                             "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                             "\r\n         AND RM.実施日 = @注文日" +
                             "\r\n     ) AS A" +
                             "\r\n LEFT JOIN V_コース区分 AS VK ON VK.コース区分CD = A.コース区分" +
                             "\r\n LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = A.企業ＣＤ  AND BM.部署ＣＤ = A.部署ＣＤ" +
                             "\r\n LEFT JOIN 支店マスタ AS SM ON SM.企業ＣＤ = BM.企業ＣＤ AND SM.支店ＣＤ = BM.支店ＣＤ";

                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = haisobi;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コース情報");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    idoNyuryokuDetailCriteria = new IdoNyuryokuDetailCriteria(dt.Rows[0]);
                }
            }

            return idoNyuryokuDetailCriteria;
        }

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private DateTime? _配送日;
        private int? _コースＣＤ;
        private int? _コース区分;
        private int? _支店ＣＤ;
        private int? _部署ＣＤ;

        private string _コース名;
        private string _コース区分名;
        private string _支店名;
        private string _部署名;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IdoNyuryokuDetailCriteria()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public IdoNyuryokuDetailCriteria(IdoNyuryokuDetailCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            配送日 = src.配送日;
            コースＣＤ = src.コースＣＤ;
            コース区分 = src.コース区分;
            支店ＣＤ = src.支店ＣＤ;
            部署ＣＤ = src.部署ＣＤ;

            コース名 = src.コース名;
            コース区分名 = src.コース区分名;
            支店名 = src.支店名;
            部署名 = src.部署名;

            _stopOverwrite = false;
        }

        /// <summary>
        /// データ行からのコンストラクタ
        /// </summary>
        /// <param name="dr"></param>
        public IdoNyuryokuDetailCriteria(DataRow dr)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            コースＣＤ = dr["コースＣＤ"] as int?;
            コース区分 = dr["コース区分"] as int?;
            支店ＣＤ = dr["支店ＣＤ"] as int?;
            部署ＣＤ = dr["部署ＣＤ"] as int?;

            コース名 = dr["コース名"] as string;
            コース区分名 = dr["コース区分名"] as string;
            支店名 = dr["支店名"] as string;
            部署名 = dr["部署名"] as string;

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 配送日
        {
            get { return _配送日; }
            set { _配送日 = value; }
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

        public int? コース区分
        {
            get { return _コース区分; }
            set { _コース区分 = value; }
        }

        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public string コース区分名
        {
            get { return _コース区分名; }
            set { _コース区分名 = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public bool HasErrors
        {
            get { return (_errors.Count != 0 || Error != null); }
        }

        // これより下は IDataErrorInfo のメソッドの実装
        // 全体整合チェック
        public string Error
        {
            get
            {
                return null;
            }
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
        public bool Equals(IdoNyuryokuDetailCriteria target)
        {
            // 画面で入力できるのはコースＣＤのみ
            if (コースＣＤ == target.コースＣＤ)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}

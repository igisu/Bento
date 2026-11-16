using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using Emu.Bento.Common;

namespace Emu.Bento.Data
{
    /// <summary>
    /// 一括コース注文入力検索条件
    /// </summary>
    public class IkkatsuCourseChumonCriteria : IDataErrorInfo
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        public bool stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private DateTime? _注文日;
        private int? _コースＣＤ;
        private string _コース名;

        private int? _コース区分;
        private string _コース区分名;
        private int? _支店ＣＤ;
        private string _支店名;
        private int? _部署ＣＤ;
        private string _部署名;
        private int? _担当者ＣＤ;
        private string _担当者名;

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public IkkatsuCourseChumonCriteria()
        {
            stopOverwrite = false;

            注文日 = DateTime.Today;;
            コースＣＤ = null;
            コース名 = null;

            コース区分 = null;
            コース区分名 = null;
            支店ＣＤ = null;
            支店名 = null;
            部署ＣＤ = null;
            部署名 = null;
            担当者ＣＤ = null;
            担当者名 = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public IkkatsuCourseChumonCriteria(IkkatsuCourseChumonCriteria src)
        {
            stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            注文日 = src.注文日;
            コースＣＤ = src.コースＣＤ;
            コース名 = src.コース名;

            コース区分 = src.コース区分;
            コース区分名 = src.コース区分名;
            支店ＣＤ = src.支店ＣＤ;
            支店名 = src.支店名;
            部署ＣＤ = src.部署ＣＤ;
            部署名 = src.部署名;
            担当者ＣＤ = src.担当者ＣＤ;
            担当者名 = src.担当者名;

            stopOverwrite = false;
        }

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 注文日
        {
            get { return _注文日; }
            set
            {
                _注文日 = value;
                _errors.Remove("注文日");
                if (value == null)
                {
                    _errors["注文日"] = "注文日を入力してください。";
                }

                if (stopOverwrite == false)
                {
                    // CDに該当する名称を取得
                    GetCourse(_注文日, _コースＣＤ);
                }
            }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                _errors.Remove("コースＣＤ");
                if (value == null)
                {
                    _errors["コースＣＤ"] = "コースＣＤを入力してください。";
                }
                else
                {
                    if (stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
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

                if (stopOverwrite == false)
                {
                    // CDに該当する名称を取得
                    GetCourse(_注文日, _コースＣＤ);
                }
            }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
        }

        public int? コース区分
        {
            get { return _コース区分; }
            set { _コース区分 = value; }
        }

        public string コース区分名
        {
            get { return _コース区分名; }
            set { _コース区分名 = value; }
        }

        public int? 支店ＣＤ
        {
            get { return _支店ＣＤ; }
            set { _支店ＣＤ = value; }
        }

        public string 支店名
        {
            get { return _支店名; }
            set { _支店名 = value; }
        }

        public int? 部署ＣＤ
        {
            get { return _部署ＣＤ; }
            set { _部署ＣＤ = value; }
        }

        public string 部署名
        {
            get { return _部署名; }
            set { _部署名 = value; }
        }

        public int? 担当者ＣＤ
        {
            get { return _担当者ＣＤ; }
            set { _担当者ＣＤ = value; }
        }

        public string 担当者名
        {
            get { return _担当者名; }
            set { _担当者名 = value; }
        }

        #endregion

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
                if (_部署ＣＤ == null)
                {
                    return "部署が見つかりません。";
                }
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
        public bool Equals(IkkatsuCourseChumonCriteria target)
        {
            if (注文日 == target.注文日 &&
                コースＣＤ == target.コースＣＤ)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="chumonbi">注文日</param>
        /// <param name="coursecd">コースＣＤ</param>
        /// <returns></returns>
        private void GetCourse(DateTime? chumonbi, int? coursecd)
        {
            コース名 = null;
            コース区分 = null;
            コース区分名 = null;
            支店ＣＤ = null;
            支店名 = null;
            部署ＣＤ = null;
            部署名 = null;
            担当者ＣＤ = null;
            担当者名 = null;

            // どちらかが未入力の場合は検索できないので、全てクリアする
            if (chumonbi == null || coursecd == null)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                string sql = "\r\n SELECT" +
                             "\r\n     A.コース名" +
                             "\r\n     ,A.コース区分" +
                             "\r\n     ,VK.コース区分名" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,B.部署名" +
                             "\r\n     ,B.支店ＣＤ" +
                             "\r\n     ,S.支店名" +
                             "\r\n     ,A.担当者ＣＤ" +
                             "\r\n     ,A3.担当者名" +
                             "\r\n FROM" +
                             // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                             "\r\n     (SELECT" +
                             "\r\n         C.企業ＣＤ" +
                             "\r\n         ,C.コースＣＤ" +
                             "\r\n         ,C.コース区分" +
                             "\r\n         ,C.コース名" +
                             "\r\n         ,C.部署ＣＤ" +
                             "\r\n         ,C.担当者ＣＤ" +
                             "\r\n         ,C.工場フラグ" +
                             "\r\n         ,C.無効フラグ" +
                             "\r\n         ,C.作成担当者ＣＤ" +
                             "\r\n         ,C.作成日" +
                             "\r\n         ,C.修正担当者ＣＤ" +
                             "\r\n         ,C.修正日" +
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
                             "\r\n         ,RM.担当者ＣＤ" +
                             "\r\n         ,RM.工場フラグ" +
                             "\r\n         ,RM.無効フラグ" +
                             "\r\n         ,RM.作成担当者ＣＤ" +
                             "\r\n         ,RM.作成日" +
                             "\r\n         ,RM.修正担当者ＣＤ" +
                             "\r\n         ,RM.修正日" +
                             "\r\n     FROM" +
                             "\r\n         臨時コーステーブル AS RT" +
                             "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                             "\r\n     WHERE" +
                             "\r\n         RM.企業ＣＤ = @企業ＣＤ" +
                             "\r\n         AND RM.コースＣＤ = @コースＣＤ" +
                             "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                             "\r\n         AND RM.実施日 = @注文日" +
                             "\r\n ) AS A" +
                             "\r\n LEFT JOIN V_コース区分 AS VK ON VK.コース区分CD = A.コース区分" +
                             "\r\n LEFT JOIN 部署マスタ AS B ON A.企業ＣＤ = B.企業ＣＤ AND A.部署ＣＤ = B.部署ＣＤ" +
                             "\r\n LEFT JOIN 支店マスタ AS S ON S.企業ＣＤ = B.企業ＣＤ AND S.支店ＣＤ = B.支店ＣＤ" +
                             "\r\n LEFT JOIN 担当者マスタ AS A3 ON A3.企業ＣＤ = A.企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ";


                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = coursecd;
                cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = chumonbi;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コース");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    DataRow dr = dt.Rows[0];
                    コース名 = dr["コース名"] as string;
                    コース区分 = dr["コース区分"] as int?;
                    コース区分名 = dr["コース区分名"] as string;
                    支店ＣＤ = dr["支店ＣＤ"] as int?;
                    支店名 = dr["支店名"] as string;
                    部署ＣＤ = dr["部署ＣＤ"] as int?;
                    部署名 = dr["部署名"] as string;
                    担当者ＣＤ = dr["担当者ＣＤ"] as int?;
                    担当者名 = dr["担当者名"] as string;
                }
            }
        }

        #endregion
    }
}

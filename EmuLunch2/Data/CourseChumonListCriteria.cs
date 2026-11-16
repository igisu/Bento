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
    /// コース注文一覧検索条件
    /// </summary>
    /// <remarks>
    /// 【前提】コースが決まれば、部署ＣＤは自動的に決まる（コースが部署ＣＤを持っているため）
    /// 【例外】注文入力時は、コースとは無関係の部署ＣＤを指定できる（日配の弁当はコースと部署は関連するが、ケータリングなどはこの限りではないため）
    /// 【方針】コース注文一覧はどのような注文も一覧として表示したいので、コースとは無関係の部署ＣＤでも条件に指定可能とする。
    ///  むしろ、部署ＣＤは注文ヘッダ／明細のキーなので、こちらを必須条件として、コースは任意の条件とする。
    ///  ※配送予定入力、日配持出入力との違いはそれぞれの検索条件クラスを参照。
    /// （2019/10/09）そもそもこの画面では部署とコースが紐付く（＝コースの部署と入力された部署が一致する）ことを必要としていない。
    ///  したがって、配送予定入力画面がしているように、コースが持つ部署を初期表示する。
    /// </remarks>
    public class CourseChumonListCriteria : IDataErrorInfo
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
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// エラー情報マップ
        /// </summary>
        protected Dictionary<string, string> _errors = new Dictionary<string, string>();

        private bool _stopOverwrite;              // コードに該当する名称を直接設定するので、マスタから取得しないようにするフラグ

        private DateTime? _喫食日;
        private bool? _配送日フラグ;
        private int? _部署ＣＤ;
        private int? _コースＣＤ;

        private string _コース名;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CourseChumonListCriteria()
        {
            _stopOverwrite = false;
            喫食日 = null;
            配送日フラグ = false;
            部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
            コースＣＤ = null;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public CourseChumonListCriteria(CourseChumonListCriteria src)
        {
            _stopOverwrite = true;          // レコードの内容を取得したときなので、CD設定時の名称取得処理を防止する

            喫食日 = src.喫食日;
            配送日フラグ = src.配送日フラグ;
            部署ＣＤ = src.部署ＣＤ;
            コースＣＤ = src.コースＣＤ;

            _stopOverwrite = false;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public DateTime? 喫食日
        {
            get { return _喫食日; }
            set
            {
                _喫食日 = value;
                _errors.Remove("喫食日");
                if (value == null)
                {
                    _errors["喫食日"] = "喫食日を入力してください。";
                }
                GetCourse(_喫食日, _コースＣＤ);    // CDに該当する名称を取得
            }
        }

        public bool? 配送日フラグ
        {
            get { return _配送日フラグ; }
            set { _配送日フラグ = value; }
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

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                _errors.Remove("コースＣＤ");
                if (value != null)
                {
                    if (_stopOverwrite == false)
                    {
                        // CDに該当する名称を取得
                        Course course = Course.GetCourse((int)_コースＣＤ);
                        if (course == null)
                        {
                            _errors["コースＣＤ"] = "コースＣＤが見つかりません。";
                        }
                    }
                }
                GetCourse(_喫食日, _コースＣＤ);    // CDに該当する名称を取得
            }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
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
        public bool Equals(CourseChumonListCriteria target)
        {
            if (喫食日 == target.喫食日 &&
                (配送日フラグ ?? false) == (target.配送日フラグ ?? false) &&
                部署ＣＤ == target.部署ＣＤ &&
                コースＣＤ == target.コースＣＤ)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// コードに該当するマスタを返します
        /// </summary>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="coursecd">コースＣＤ</param>
        private void GetCourse(DateTime? kissyokubi, int? coursecd)
        {
            コース名 = null;

            // どちらかが未入力の場合は検索できない
            if (kissyokubi == null || coursecd == null)
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
                cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = kissyokubi;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コース");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    DataRow dr = dt.Rows[0];
                    コース名 = dr["コース名"] as string;
                    部署ＣＤ = dr["部署ＣＤ"] as int?;
                }
            }
        }
        #endregion
    }
}

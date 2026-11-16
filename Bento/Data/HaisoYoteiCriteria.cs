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
    /// 配送予定入力検索条件
    /// </summary>
    /// <remarks>
    /// 【前提】コースが決まれば、部署ＣＤは自動的に決まる（コースが部署ＣＤを持っているため）
    /// 【例外】注文入力時は、コースとは無関係の部署ＣＤを指定できる（日配の弁当はコースと部署は関連するが、ケータリングなどはこの限りではないため）
    /// 【方針】配送予定入力は、日配の弁当（＝ルートに沿った配達）の注文入力を簡素化を目的としているので、コースが持つ部署ＣＤを初期表示する。
    ///  ただし、コースとは無関係の部署ＣＤの入力も許可する。
    ///  ※注文一覧、日配持出入力との違いはそれぞれの検索条件クラスを参照。
    /// </remarks>
    public class HaisoYoteiCriteria : IDataErrorInfo
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

        private DateTime? _注文日;
        private int? _コースＣＤ;
        private int? _部署ＣＤ;

        private string _コース名;
        private int? _コース区分;
        private int? _担当者ＣＤ;
        private string _担当者名;

        private bool _コース区分注文日不一致フラグ;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HaisoYoteiCriteria()
        {
            注文日 = DateTime.Today;
            コースＣＤ = null;
            部署ＣＤ = null;
            担当者ＣＤ = null;
            担当者名 = null;
            コース区分注文日不一致フラグ = false;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="src"></param>
        public HaisoYoteiCriteria(HaisoYoteiCriteria src)
        {
            注文日 = src.注文日;
            コースＣＤ = src.コースＣＤ;
            部署ＣＤ = src.部署ＣＤ;
            担当者ＣＤ = src.担当者ＣＤ;
            担当者名 = src.担当者名;
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
                GetCourse(_注文日, _コースＣＤ);    // CDに該当する名称を取得
            }
        }

        public int? コースＣＤ
        {
            get { return _コースＣＤ; }
            set
            {
                _コースＣＤ = value;
                コース名 = null;
                部署ＣＤ = null;
                _errors.Remove("コースＣＤ");
                if (value != null)
                {
                    // CDに該当する名称を取得
                    Course course = Course.GetCourse((int)_コースＣＤ);
                    if (course == null)
                    {
                        _errors["コースＣＤ"] = "コースＣＤが見つかりません。";
                    }
                }
                GetCourse(_注文日, _コースＣＤ);    // CDに該当する名称を取得
            }
        }

        public string コース名
        {
            get { return _コース名; }
            set { _コース名 = value; }
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

        public int? コース区分
        {
            get { return _コース区分; }
            set { _コース区分 = value; }
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

        public bool コース区分注文日不一致フラグ
        {
            get { return _コース区分注文日不一致フラグ; }
            set { _コース区分注文日不一致フラグ = value; }
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
        public bool Equals(HaisoYoteiCriteria target)
        {
            if (注文日 == target.注文日 &&
                コースＣＤ == target.コースＣＤ &&
                部署ＣＤ == target.部署ＣＤ)
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
            部署ＣＤ = null;
            担当者ＣＤ = null;
            担当者名 = null;

            _コース区分注文日不一致フラグ = false;

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

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コース");
                adp.Fill(dt);

                if (dt.Rows.Count != 0)
                {
                    DataRow dr = dt.Rows[0];
                    コース名 = dr["コース名"] as string;
                    コース区分 = dr["コース区分"] as int?;
                    部署ＣＤ = dr["部署ＣＤ"] as int?;
                    担当者ＣＤ = dr["担当者ＣＤ"] as int?;
                    担当者名 = dr["担当者名"] as string;
                }
            }

            // ついでにコース区分と注文日が一致しているかをチェックする
            if (_注文日 != null && _コース区分 != null)
            {
                // コース区分と注文日の曜日が一致していなければフラグをON
                if (_コース区分 != Course.GetCourseKubun(_注文日))
                {
                    _コース区分注文日不一致フラグ = true;
                }
            }
        }
        #endregion
    }
}

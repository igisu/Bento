using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Data;

namespace Emu.Bento.Common
{
    /// <summary>
    /// コンボボックス関連共通クラス
    /// </summary>
    public static class ComboUtil
    {
        //**************************************
        //* 列挙値
        //**************************************
        #region 列挙値
        /// <summary>
        /// テキストに表示する名称
        /// </summary>
        public enum NameType
        {
            Long,       // 名称
            Short       // 略称
        }

        /// <summary>
        /// コースタイプ
        /// </summary>
        public enum CourseType
        {
            All,            // 全て
            Course,         // コース
            Kaisyu          // 回収コース
        }

        #endregion

        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体

        /// <summary>
        /// テーブルコンテナ
        /// </summary>
        /// <remarks>コンボボックス生成用データテーブルと実テーブル名・列名の要素を格納する（主にV_XXXテーブル取得用）</remarks>
        private class TableContainer
        {
            public string Name { get; set; }
            public DataTable Table { get; set; }

            public TableContainer(string name)
            {
                Name = name;
                Table = null;
            }
        }
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// コース区分コンボ用データテーブル（全て）
        /// </summary>
        private static DataTable _dtCourseAll = null;

        /// <summary>
        /// コース区分コンボ用データテーブル（コース）
        /// </summary>
        private static DataTable _dtCourseCourse = null;

        /// <summary>
        /// コース区分コンボ用データテーブル（回収コース）
        /// </summary>
        private static DataTable _dtCourseKaisyu = null;

        /// <summary>
        /// 部署グループ用テーブルコンテナ
        /// </summary>
        private static TableContainer BusyoGroup = new TableContainer("部署グループ");

        /// <summary>
        /// 弁当区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer BentoKubun = new TableContainer("弁当区分");

        /// <summary>
        /// 食事区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer SyokujiKubun = new TableContainer("食事区分");

        /// <summary>
        /// 営業業務区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer EigyoKubun = new TableContainer("営業業務区分");

        /// <summary>
        /// 権限区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer Kengen = new TableContainer("権限区分");

        /// <summary>
        /// 工場コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer Kojo = new TableContainer("工場");

        /// <summary>
        /// 支店コンボ用データテーブル
        /// </summary>
        private static DataTable _dtShiten = null;

        /// <summary>
        /// 口座種別コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer KozaSyubetsu = new TableContainer("口座種別");

        /// <summary>
        /// 工場区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer KojoKubun = new TableContainer("工場区分");

        /// <summary>
        /// 状態区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer JotaiKubun = new TableContainer("状態区分");

        /// <summary>
        /// 状態理由区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer JotaiRiyuKubun = new TableContainer("状態理由区分");

        /// <summary>
        /// 売掛現金区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer UriGenKubun = new TableContainer("売掛現金区分");

        /// <summary>
        /// 締め区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer ShimeKubun = new TableContainer("締め区分");

        /// <summary>
        /// 支払サイトコンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer ShiharaiSight = new TableContainer("支払サイト区分");

        /// <summary>
        /// 支払方法用テーブルコンテナ
        /// </summary>
        private static TableContainer ShiharaiHoho = new TableContainer("支払方法区分");

        /// <summary>
        /// 集金区分用テーブルコンテナ
        /// </summary>
        private static TableContainer SyukinKubun = new TableContainer("集金区分");

        /// <summary>
        /// 受注方法用テーブルコンテナ
        /// </summary>
        private static TableContainer JuchuHoho = new TableContainer("受注方法区分");

        /// <summary>
        /// 空き容器回収区分用テーブルコンテナ
        /// </summary>
        private static TableContainer YokiKaisyu = new TableContainer("空き容器回収区分");

        /// <summary>
        /// 請求書敬称用テーブルコンテナ
        /// </summary>
        private static TableContainer Keisyo = new TableContainer("請求書敬称");

        /// <summary>
        /// 税区分用テーブルコンテナ
        /// </summary>
        private static TableContainer ZeiKubun = new TableContainer("税区分");

        /// <summary>
        /// 丸め処理区分用テーブルコンテナ
        /// </summary>
        private static TableContainer MarumeKubun = new TableContainer("丸め処理区分");

        /// <summary>
        /// 売上区分用テーブルコンテナ
        /// </summary>
        private static TableContainer UriageKubun = new TableContainer("売上区分");

        /// <summary>
        /// 売上区分（LINE決済を含まない）用テーブルコンテナ
        /// </summary>
        private static TableContainer UriageKubunWithoutLine = new TableContainer("売上区分");

        /// <summary>
        /// 売上区分（電子決済請求締め）用テーブルコンテナ
        /// </summary>
        private static TableContainer UriageKubunPaySeikyuShime = new TableContainer("売上区分");

        /// <summary>
        /// 値引事由区分用テーブルコンテナ
        /// </summary>
        private static TableContainer NebikiKubun = new TableContainer("値引事由区分");

        /// <summary>
        /// 請求書タイプ区分用テーブルコンテナ
        /// </summary>
        private static TableContainer SeikyusyoTypeKubun = new TableContainer("請求書タイプ区分");

        /// <summary>
        /// 入金方法用テーブルコンテナ
        /// </summary>
        private static TableContainer NyukinHoho = new TableContainer("入金方法");

        /// <summary>
        /// 集計対象区分用テーブルコンテナ
        /// </summary>
        private static TableContainer SyukeiTaisyoKubun = new TableContainer("集計対象区分");

        /// <summary>
        /// Web履歴区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer WebHistoryKubun = new TableContainer("WEB履歴区分");

        /// <summary>
        /// Web受付期限月数コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer WebCanOrderMonths = new TableContainer("WEB受付期限月数");

        /// <summary>
        /// Web注文可能期限日数コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer WebOrderLimitDays = new TableContainer("WEB注文可能期限日数");

        /// <summary>
        /// 大袋ラベル食事区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer BigBagSyokujiKubun = new TableContainer("大袋ラベル食事区分");

        /// <summary>
        /// 配送集計表出力区分コンボ用テーブルコンテナ
        /// </summary>
        private static TableContainer HaisoSyukeihyoKubun = new TableContainer("配送集計表出力区分");


        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// コンボボックスのデータソースを設定します。
        /// </summary>
        /// <param name="cbo">コンボボックス</param>
        /// <param name="displayMember">表示する値の配列</param>
        /// <param name="valueMember">実際の値の配列</param>
        public static void Set<T>(ComboBox cbo, string[] displayMember, T[] valueMember)
        {
            int loopMax = Math.Min(displayMember.Length, valueMember.Length);
            DataTable dt = new DataTable();
            dt.Columns.Add("displayMember", typeof(string));
            dt.Columns.Add("valueMember", typeof(T));

            for (int i = 0; i < loopMax; i++)
            {
                DataRow dr = dt.NewRow();
                dr["displayMember"] = displayMember[i];
                dr["valueMember"] = valueMember[i];
                dt.Rows.Add(dr);
            }

            cbo.DisplayMember = "displayMember";
            cbo.ValueMember = "valueMember";
            cbo.DataSource = dt;
        }

        /// <summary>
        /// コンボボックスのデータソースを設定します。
        /// </summary>
        /// <param name="cbo">スプレッド用コンボボックス</param>
        /// <param name="displayMember">表示する値の配列</param>
        /// <param name="valueMember">実際の値の配列</param>
        public static void Set<T>(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo, string[] displayMember, T[] valueMember)
        {
            int loopMax = Math.Min(displayMember.Length, valueMember.Length);
            List<string> itemList = new List<string>();         // 表示用リスト
            List<string> itemDataList = new List<string>();     // 値用リスト

            for (int i = 0; i < loopMax; i++)
            {
                itemList.Add(displayMember[i]);
                itemDataList.Add(valueMember[i].ToString());
            }

            cbo.Items = itemList.ToArray();
            cbo.ItemData = itemDataList.ToArray();
            cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
        }

        /// <summary>
        /// コース区分コンボ作成
        /// </summary>
        /// <param name="cbo">コース区分コンボ</param>
        public static void SetCourseKubun(ComboBox cbo)
        {
            SetCourseKubun(cbo, CourseType.All);
        }

        /// <summary>
        /// コース区分コンボ作成
        /// </summary>
        /// <param name="cbo">コース区分コンボ</param>
        /// <param name="courseType">コースタイプ</param>
        public static void SetCourseKubun(ComboBox cbo, CourseType courseType)
        {
            GetCourseKubun(courseType);

            cbo.DisplayMember = "コース区分名";
            cbo.ValueMember = "コース区分CD";

            DataTable dt = null;
            switch (courseType)
            {
                case CourseType.All:
                    dt = _dtCourseAll;
                    break;

                case CourseType.Course:
                    dt = _dtCourseCourse;
                    break;

                case CourseType.Kaisyu:
                    dt = _dtCourseKaisyu;
                    break;
            }
            cbo.DataSource = dt.Copy();
        }

        /// <summary>
        /// コース区分コンボ作成
        /// </summary>
        /// <param name="cbo">コースコンボ</param>
        public static void SetCourseKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            SetCourseKubun(cbo, CourseType.All);
        }

        /// <summary>
        /// コース区分コンボ作成
        /// </summary>
        /// <param name="cbo">コースコンボ</param>
        /// <param name="courseType">コースタイプ</param>
        public static void SetCourseKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo, CourseType courseType)
        {
            GetCourseKubun(courseType);

            DataTable dt = null;
            switch (courseType)
            {
                case CourseType.All:
                    dt = _dtCourseAll;
                    break;

                case CourseType.Course:
                    dt = _dtCourseCourse;
                    break;

                case CourseType.Kaisyu:
                    dt = _dtCourseKaisyu;
                    break;
            }

            List<string> itemList = new List<string>();         // 表示用リスト
            List<string> itemDataList = new List<string>();     // 値用リスト

            foreach (DataRow dr in dt.Rows)
            {
                itemList.Add(dr["コース区分名"].ToString());
                itemDataList.Add(dr["コース区分CD"].ToString());
            }

            cbo.Items = itemList.ToArray();
            cbo.ItemData = itemDataList.ToArray();
            cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

        }

        /// <summary>
        /// コース区分取得
        /// </summary>
        /// <param name="courceType">コースタイプ</param>
        private static void GetCourseKubun(CourseType courceType)
        {
            DataTable dt = null;
            string where = null;

            switch (courceType)
            {
                case CourseType.All:
                    dt = _dtCourseAll;
                    where = "";
                    break;

                case CourseType.Course:
                    dt = _dtCourseCourse;
                    where = " WHERE ISNULL(回収フラグ, 0) = 0";
                    break;

                case CourseType.Kaisyu:
                    dt = _dtCourseKaisyu;
                    where = " WHERE ISNULL(回収フラグ, 0) = 1";
                    break;
            }

            if (dt == null)
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     -1 AS コース区分CD");
                    sql.AppendLine("     ,'' AS コース区分名");
                    sql.AppendLine("     ,-1 AS 表示順");
                    sql.AppendLine(" UNION ");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     コース区分CD");
                    sql.AppendLine("     ,REPLACE(STR(コース区分CD, (SELECT LEN(MAX(コース区分CD)) FROM V_コース区分)), ' ', '0') + ':' + コース区分名 AS コース区分名");
                    sql.AppendLine("     ,表示順");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     V_コース区分");
                    sql.AppendLine(where);
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     表示順");

                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = sql.ToString();

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    adp.Fill(dt);
                }

                switch (courceType)
                {
                    case CourseType.All:
                        _dtCourseAll = dt;
                        break;

                    case CourseType.Course:
                        _dtCourseCourse = dt;
                        break;

                    case CourseType.Kaisyu:
                        _dtCourseKaisyu = dt;
                        break;
                }
            }
        }

        /// <summary>
        /// コース別グループ種別コンボ作成
        /// </summary>
        /// <param name="cbo">コース別グループ種別コンボ</param>
        /// <param name="busyoCD">コース別グループ種別を絞り込む部署ＣＤ</param>
        public static void SetCourseGroup(ComboBox cbo, int? busyoCD)
        {
            cbo.DisplayMember = "グループ種別名";
            cbo.ValueMember = "グループ種別ＣＤ";

            // 部署ＣＤに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            cbo.DataSource = GetCourseGroup(busyoCD);
        }

        /// <summary>
        /// コース別グループ種別取得
        /// </summary>
        /// <param name="busyoCD">コース別グループ種別を絞り込む部署ＣＤ</param>
        private static DataTable GetCourseGroup(int? busyoCD)
        {
            DataTable dt = null;

            // 部署ＣＤに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     -1 AS グループ種別ＣＤ");
                sql.AppendLine("     ,'' AS グループ種別名 ");
                sql.AppendLine(" UNION ");
                sql.AppendLine(" SELECT");
                sql.AppendLine("     グループ種別CD");
                sql.AppendLine("     ,REPLACE(STR(グループ種別ＣＤ, (SELECT LEN(MAX(グループ種別ＣＤ)) FROM コース別グループマスタ)), ' ', '0') + ':' + グループ種別名 AS グループ種別名");
                sql.AppendLine(" FROM");
                sql.AppendLine("     コース別グループマスタ");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine(" AND 企業ＣＤ = @企業ＣＤ");

                if (busyoCD != null && busyoCD != -1)
                {
                    sql.AppendLine(" AND 部署ＣＤ = @部署ＣＤ");
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD;
                }

                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     グループ種別ＣＤ");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.CommandText = sql.ToString();

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// 部署コンボ作成
        /// </summary>
        /// <param name="cbo">部署コンボ</param>
        public static void SetBusyo(ComboBox cbo)
        {
            SetBusyo(cbo, null);
        }

        /// <summary>
        /// 部署コンボ作成
        /// </summary>
        /// <param name="cbo">部署コンボ</param>
        /// <param name="shitenCD">部署を絞り込む支店ＣＤ</param>
        public static void SetBusyo(ComboBox cbo, int? shitenCD)
        {
            cbo.DisplayMember = "部署名";
            cbo.ValueMember = "部署ＣＤ";

            // 支店ＣＤに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            cbo.DataSource = GetBusyo(shitenCD);
        }

        /// <summary>
        /// 部署コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">部署コンボ</param>
        public static void SetBusyo(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            SetBusyo(cbo, null);
        }

        /// <summary>
        /// 部署コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">部署コンボ</param>
        /// <param name="shitenCD">部署を絞り込む支店ＣＤ</param>
        public static void SetBusyo(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo, int? shitenCD)
        {
            List<string> itemList = new List<string>();         // 表示用リスト
            List<string> itemDataList = new List<string>();     // 値用リスト

            // 支店ＣＤに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            DataTable dt = GetBusyo(shitenCD);
            foreach (DataRow dr in dt.Rows)
            {
                itemList.Add(dr["部署名"].ToString());
                itemDataList.Add(dr["部署ＣＤ"].ToString());
            }

            cbo.Items = itemList.ToArray();
            cbo.ItemData = itemDataList.ToArray();
            cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
        }

        /// <summary>
        /// 部署取得
        /// </summary>
        /// <param name="shitenCD">部署を絞り込む支店ＣＤ</param>
        private static DataTable GetBusyo(int? shitenCD)
        {
            DataTable dt = null;

            // 支店ＣＤに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" SELECT");
                sql.AppendLine("     -1 AS 部署ＣＤ");
                sql.AppendLine("     ,'' AS 部署名 ");
                sql.AppendLine(" UNION ");
                sql.AppendLine(" SELECT");
                sql.AppendLine("     部署CD");
                sql.AppendLine("     ,REPLACE(STR(部署ＣＤ, (SELECT LEN(MAX(部署ＣＤ)) FROM 部署マスタ)), ' ', '0') + ':' + 部署名 AS 部署名");
                sql.AppendLine(" FROM");
                sql.AppendLine("     部署マスタ");
                sql.AppendLine(" WHERE");
                sql.AppendLine("     ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine(" AND 企業ＣＤ = @企業ＣＤ");

                if (shitenCD != null && shitenCD != -1)
                {
                    sql.AppendLine(" AND 支店ＣＤ = @支店ＣＤ");
                    cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = shitenCD;
                }

                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     部署ＣＤ");

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.CommandText = sql.ToString();

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// 部署グループコンボ作成
        /// </summary>
        /// <param name="cbo">部署グループコンボ</param>
        public static void SetBusyoGroup(ComboBox cbo)
        {
            GetTable(BusyoGroup);
            SetCombo(cbo, BusyoGroup);
        }

        /// <summary>
        /// 弁当区分コンボ作成
        /// </summary>
        /// <param name="cbo">弁当区分コンボ</param>
        public static void SetBentoKubun(ComboBox cbo)
        {
            GetTable(BentoKubun);
            SetCombo(cbo, BentoKubun);
        }

        /// <summary>
        /// 副食CDコンボ作成
        /// </summary>
        /// <param name="cbo">副食CDコンボ</param>
        public static void SetSubDishCD(ComboBox cbo)
        {
            cbo.DisplayMember = "商品名";
            cbo.ValueMember = "商品ＣＤ";

            // 商品マスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            cbo.DataSource = GetDishCD(Consts.BentoKubun.SubDish);
        }

        /// <summary>
        /// 主食CDコンボ作成
        /// </summary>
        /// <param name="cbo">主食CDコンボ</param>
        public static void SetMainDishCD(ComboBox cbo)
        {
            cbo.DisplayMember = "商品名";
            cbo.ValueMember = "商品ＣＤ";

            // 商品マスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            cbo.DataSource = GetDishCD(Consts.BentoKubun.MainDish);
        }

        /// <summary>
        /// 主食／副食CD取得
        /// </summary>
        private static DataTable GetDishCD(Consts.BentoKubun bentoKubun)
        {
            DataTable dt = null;

            // 商品マスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();

                string sql = "\r\n SELECT" +
                             "\r\n      -1  AS 商品ＣＤ" +
                             "\r\n     ,'' AS 商品名 " +
                             "\r\n UNION " +
                             "\r\n SELECT" +
                             "\r\n     商品ＣＤ" +
                             "\r\n     ,REPLACE(STR(商品ＣＤ, (SELECT LEN(MAX(商品ＣＤ)) FROM 商品マスタ)), ' ', '0') + ':' + 商品名 AS 商品名" +
                             "\r\n FROM" +
                             "\r\n     商品マスタ" +
                             "\r\n WHERE" +
                             "\r\n     弁当区分 = @弁当区分" +
                             "\r\n ORDER BY" +
                             "\r\n     商品ＣＤ";

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@弁当区分", SqlDbType.Int).Value = (int)bentoKubun;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// 食事区分コンボ作成
        /// </summary>
        /// <param name="cbo">食事区分コンボ</param>
        public static void SetSyokujiKubun(ComboBox cbo)
        {
            GetTable(SyokujiKubun);
            SetCombo(cbo, SyokujiKubun);
        }

        /// <summary>
        /// 食事区分コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">食事区分コンボ</param>
        public static void SetSyokujiKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(SyokujiKubun);
            SetCombo(cbo, SyokujiKubun);
        }

        /// <summary>
        /// 営業業務区分コンボ作成
        /// </summary>
        /// <param name="cbo">営業業務区分コンボ</param>
        public static void SetEigyoKubun(ComboBox cbo)
        {
            GetTable(EigyoKubun);
            SetCombo(cbo, EigyoKubun);
        }

        /// <summary>
        /// 権限区分コンボ作成
        /// </summary>
        /// <param name="cbo">権限区分コンボ</param>
        public static void SetKengen(ComboBox cbo)
        {
            GetTable(Kengen);
            SetCombo(cbo, Kengen);
        }

        /// <summary>
        /// 工場コンボ作成
        /// </summary>
        /// <param name="cbo">工場コンボ</param>
        public static void SetKojo(ComboBox cbo)
        {
            GetTable(Kojo);
            SetCombo(cbo, Kojo);
        }

        /// <summary>
        /// 支店コンボ作成
        /// </summary>
        /// <param name="cbo">支店コンボ</param>
        public static void SetShiten(ComboBox cbo)
        {
            GetShiten();

            cbo.DisplayMember = "支店名";
            cbo.ValueMember = "支店ＣＤ";
            cbo.DataSource = _dtShiten.Copy();
        }

        /// <summary>
        /// 支店取得
        /// </summary>
        private static void GetShiten()
        {
            if (_dtShiten == null)
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    string sql = "\r\n SELECT" +
                                 "\r\n     -1 AS 支店ＣＤ" +
                                 "\r\n     ,'' AS 支店名 " +
                                 "\r\n UNION " +
                                 "\r\n SELECT" +
                                 "\r\n     支店ＣＤ" +
                                 "\r\n     ,REPLACE(STR(支店ＣＤ, (SELECT LEN(MAX(支店ＣＤ)) FROM 支店マスタ)), ' ', '0') + ':' + 支店名 AS 支店名" +
                                 "\r\n FROM" +
                                 "\r\n     支店マスタ" +
                                 "\r\n WHERE" +
                                 "\r\n     ISNULL(無効フラグ, 0) = 0" +
                                 "\r\n AND 企業ＣＤ = @企業ＣＤ" +
                                 "\r\n ORDER BY" +
                                 "\r\n     支店ＣＤ";

                    SqlCommand cmd = con.CreateCommand();
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.CommandText = sql;

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    _dtShiten = new DataTable();
                    adp.Fill(_dtShiten);
                }
            }
        }

        /// <summary>
        /// 口座種別コンボ作成
        /// </summary>
        /// <param name="cbo">口座種別コンボ</param>
        public static void SetKozaSyubetsu(ComboBox cbo)
        {
            GetTable(KozaSyubetsu);
            SetCombo(cbo, KozaSyubetsu);
        }

        /// <summary>
        /// 口座種別コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">口座種別コンボ</param>
        public static void SetKozaSyubetsu(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(KozaSyubetsu);
            SetCombo(cbo, KozaSyubetsu);
        }

        /// <summary>
        /// 工場区分コンボ作成
        /// </summary>
        /// <param name="cbo">工場区分コンボ</param>
        public static void SetKojoKubun(ComboBox cbo)
        {
            GetTable(KojoKubun);
            SetCombo(cbo, KojoKubun);
        }

        /// <summary>
        /// 状態区分コンボ作成
        /// </summary>
        /// <param name="cbo">状態区分コンボ</param>
        public static void SetJotaiKubun(ComboBox cbo)
        {
            GetTable(JotaiKubun);
            SetCombo(cbo, JotaiKubun);
        }

        /// <summary>
        /// 状態理由区分コンボ作成
        /// </summary>
        /// <param name="cbo">状態区分コンボ</param>
        public static void SetJotaiRiyuKubun(ComboBox cbo)
        {
            GetTable(JotaiRiyuKubun);
            SetCombo(cbo, JotaiRiyuKubun);
        }

        /// <summary>
        /// 売掛現金区分コンボ作成
        /// </summary>
        /// <param name="cbo">売掛現金区分コンボ</param>
        public static void SetUriGenKubun(ComboBox cbo)
        {
            GetTable(UriGenKubun);
            SetCombo(cbo, UriGenKubun);
        }

        /// <summary>
        /// 締め区分コンボ作成
        /// </summary>
        /// <param name="cbo">締め区分コンボ</param>
        public static void SetShimeKubun(ComboBox cbo)
        {
            GetTable(ShimeKubun);
            SetCombo(cbo, ShimeKubun);
        }

        /// <summary>
        /// 支払サイト区分コンボ作成
        /// </summary>
        /// <param name="cbo">支払サイト区分コンボ</param>
        public static void SetShiharaiSight(ComboBox cbo)
        {
            GetTable(ShiharaiSight);
            SetCombo(cbo, ShiharaiSight);
        }

        /// <summary>
        /// 支払方法区分コンボ作成
        /// </summary>
        /// <param name="cbo">支払方法区分コンボ</param>
        public static void SetShiharaiHoho(ComboBox cbo)
        {
            GetTable(ShiharaiHoho);
            SetCombo(cbo, ShiharaiHoho);
        }

        /// <summary>
        /// 集金区分コンボ作成
        /// </summary>
        /// <param name="cbo">集金区分コンボ</param>
        public static void SetSyukinKubun(ComboBox cbo)
        {
            GetTable(SyukinKubun);
            SetCombo(cbo, SyukinKubun);
        }

        /// <summary>
        /// 受注方法区分コンボ作成
        /// </summary>
        /// <param name="cbo">受注方法区分コンボ</param>
        public static void SetJuchuHoho(ComboBox cbo)
        {
            GetTable(JuchuHoho);
            SetCombo(cbo, JuchuHoho);
        }

        /// <summary>
        /// 空き容器回収区分コンボ作成
        /// </summary>
        /// <param name="cbo">空き容器回収区分コンボ</param>
        public static void SetYokiKaisyu(ComboBox cbo)
        {
            GetTable(YokiKaisyu);
            SetCombo(cbo, YokiKaisyu);
        }

        /// <summary>
        /// 請求書敬称コンボ作成
        /// </summary>
        /// <param name="cbo">請求書敬称コンボ</param>
        public static void SetKeisyo(ComboBox cbo)
        {
            GetTable(Keisyo);
            SetCombo(cbo, Keisyo);
        }

        /// <summary>
        /// 税区分コンボ作成
        /// </summary>
        /// <param name="cbo">税区分コンボ</param>
        public static void SetZeiKubun(ComboBox cbo)
        {
            GetTable(ZeiKubun);
            SetCombo(cbo, ZeiKubun);
        }

        /// <summary>
        /// 丸め処理区分コンボ作成
        /// </summary>
        /// <param name="cbo">丸め処理区分コンボ</param>
        public static void SetMarumeKubun(ComboBox cbo)
        {
            GetTable(MarumeKubun);
            SetCombo(cbo, MarumeKubun);
        }

        /// <summary>
        /// 売上区分コンボ作成
        /// </summary>
        /// <param name="cbo">売上区分コンボ</param>
        public static void SetUriageKubun(ComboBox cbo)
        {
            GetTable(UriageKubun);
            SetCombo(cbo, UriageKubun);
        }

        /// <summary>
        /// 売上区分コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">売上区分コンボ</param>
        public static void SetUriageKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(UriageKubun);
            SetCombo(cbo, UriageKubun);
        }

        /// <summary>
        /// 売上区分（LINE決済を含まない）コンボ作成
        /// </summary>
        /// <param name="cbo">売上区分コンボ</param>
        public static void SetUriageKubunWithoutLine(ComboBox cbo)
        {
            // 売上区分が「5:LINE決済」未満のみに限定
            GetTable(UriageKubunWithoutLine, string.Format("売上区分CD < {0}", (int)Consts.UriageKubun.Line));
            SetCombo(cbo, UriageKubunWithoutLine);
        }

        /// <summary>
        /// 売上区分（LINE決済を含まない）コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">売上区分コンボ</param>
        public static void SetUriageKubunWithoutLine(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            // 売上区分が「5:LINE決済」未満のみに限定
            GetTable(UriageKubunWithoutLine, string.Format("売上区分CD < {0}", (int)Consts.UriageKubun.Line));
            SetCombo(cbo, UriageKubunWithoutLine);
        }

        /// <summary>
        /// 売上区分（電子決済請求締め）コンボ作成
        /// </summary>
        /// <param name="cbo">売上区分コンボ</param>
        public static void SetUriageKubunPaySeikyuShime(ComboBox cbo)
        {
            // 電子決済請求得意先マスタにある売上区分を取得する
            GetPaySeikyuUriageKubun(UriageKubunPaySeikyuShime);
            SetCombo(cbo, UriageKubunPaySeikyuShime);
        }

        /// <summary>
        /// 売上区分（電子決済請求締め）コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">売上区分コンボ</param>
        public static void SetUriageKubunPaySeikyuShime(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            // 電子決済請求得意先マスタにある売上区分を取得する
            GetPaySeikyuUriageKubun(UriageKubunPaySeikyuShime);
            SetCombo(cbo, UriageKubunPaySeikyuShime);
        }

        /// <summary>
        /// 電子決済請求得意先マスタにある売上区分を取得
        /// </summary>
        /// <param name="tc">テーブルコンテナ</param>
        private static void GetPaySeikyuUriageKubun(TableContainer tc)
        {
            if (tc.Table == null)
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    string sql = "\r\n SELECT" +
                                 "\r\n     -1 AS {0}CD" +
                                 "\r\n     ,'' AS {0}名 " +
                                 "\r\n UNION " +
                                 "\r\n SELECT" +
                                 "\r\n     A.{0}CD" +
                                 "\r\n     ,REPLACE(STR(A.{0}CD, (SELECT LEN(MAX({0}CD)) FROM 電子決済請求得意先マスタ)), ' ', '0') + ':' + B.{0}名 AS {0}名" +
                                 "\r\n FROM" +
                                 "\r\n     電子決済請求得意先マスタ AS A" +
                                 "\r\n     INNER JOIN V_{0} AS B ON A.{0}CD = B.{0}CD" +
                                 "\r\n WHERE" +
                                 "\r\n     企業ＣＤ = @企業ＣＤ" +
                                 "\r\n ORDER BY" +
                                 "\r\n     {0}CD";

                    SqlCommand cmd = con.CreateCommand();
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.CommandText = string.Format(sql, tc.Name);

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    tc.Table = new DataTable();
                    adp.Fill(tc.Table);
                }
            }
        }

        /// <summary>
        /// 値引事由区分コンボ作成
        /// </summary>
        /// <param name="cbo">値引事由区分コンボ</param>
        public static void SetNebikiKubun(ComboBox cbo)
        {
            GetTable(NebikiKubun);
            SetCombo(cbo, NebikiKubun);
        }

        /// <summary>
        /// 値引事由区分コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">値引事由区分コンボ</param>
        public static void SetNebikiKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(NebikiKubun);
            SetCombo(cbo, NebikiKubun);
        }

        /// <summary>
        /// 請求書タイプ区分コンボ作成
        /// </summary>
        /// <param name="cbo">請求書タイプ区分コンボ</param>
        public static void SetSeikyusyoTypeKubun(ComboBox cbo)
        {
            GetTable(SeikyusyoTypeKubun);
            SetCombo(cbo, SeikyusyoTypeKubun);
        }

        /// <summary>
        /// 入金方法コンボ作成
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetNyukinHoho(ComboBox cbo)
        {
            GetTable(NyukinHoho);
            SetCombo(cbo, NyukinHoho);
        }

        /// <summary>
        /// 入金方法コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetNyukinHoho(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(NyukinHoho);
            SetCombo(cbo, NyukinHoho);
        }

        /// <summary>
        /// 集計対象区分コンボ作成
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetSyukeiTaisyoKubun(ComboBox cbo)
        {
            GetTable(SyukeiTaisyoKubun);
            SetCombo(cbo, SyukeiTaisyoKubun);
        }

        /// <summary>
        /// 集計対象区分コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetSyukeiTaisyoKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(SyukeiTaisyoKubun);
            SetCombo(cbo, SyukeiTaisyoKubun);
        }

        /// <summary>
        /// Web履歴区分コンボ作成
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetWebHistoryKubun(ComboBox cbo)
        {
            GetTable(WebHistoryKubun);
            SetCombo(cbo, WebHistoryKubun);
        }

        /// <summary>
        /// Web履歴区分コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetWebHistoryKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(WebHistoryKubun);
            SetCombo(cbo, WebHistoryKubun);
        }

        /// <summary>
        /// Web受付期限月数コンボ作成
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetWebCanOrderMonths(ComboBox cbo)
        {
            GetTable(WebCanOrderMonths);
            SetCombo(cbo, WebCanOrderMonths);
        }

        /// <summary>
        /// Web受付期限月数コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetWebCanOrderMonths(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(WebCanOrderMonths);
            SetCombo(cbo, WebCanOrderMonths);
        }

        /// <summary>
        /// Web注文可能期限日数コンボ作成
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetWebOrderLimitDays(ComboBox cbo)
        {
            GetTable(WebOrderLimitDays);
            SetCombo(cbo, WebOrderLimitDays);
        }

        /// <summary>
        /// Web注文可能期限日数コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">入金方法コンボ</param>
        public static void SetWebOrderLimitDays(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(WebOrderLimitDays);
            SetCombo(cbo, WebOrderLimitDays);
        }

        /// <summary>
        /// 大袋ラベル食事区分コンボ作成
        /// </summary>
        /// <param name="cbo">食事区分コンボ</param>
        public static void SetBigBagSyokujiKubun(ComboBox cbo)
        {
            GetTable(BigBagSyokujiKubun);
            SetCombo(cbo, BigBagSyokujiKubun);
        }

        /// <summary>
        /// 大袋ラベル食事区分コンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo">食事区分コンボ</param>
        public static void SetBigBagSyokujiKubun(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            GetTable(BigBagSyokujiKubun);
            SetCombo(cbo, BigBagSyokujiKubun);
        }

        /// <summary>
        /// 配送集計表出力区分コンボ作成
        /// </summary>
        /// <param name="cbo">配送集計表出力区分コンボ</param>
        public static void SetHaisoSyukeihyoKubun(ComboBox cbo)
        {
            GetTable(HaisoSyukeihyoKubun);
            SetCombo(cbo, HaisoSyukeihyoKubun);
        }

        /// <summary>
        /// 商品グループコンボ作成
        /// </summary>
        /// <param name="cbo"></param>
        public static void SetSyohinGroup(ComboBox cbo)
        {
            cbo.DisplayMember = "商品グループ名";
            cbo.ValueMember = "商品グループＣＤ";

            // 商品グループマスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            cbo.DataSource = GetSyohinGroup();
        }

        /// <summary>
        /// 商品グループコンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo"></param>
        public static void SetSyohinGroup(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            List<string> itemList = new List<string>();         // 表示用リスト
            List<string> itemDataList = new List<string>();     // 値用リスト

            // 商品グループマスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            DataTable dt = GetSyohinGroup();
            foreach (DataRow dr in dt.Rows)
            {
                itemList.Add(dr["商品グループ名"].ToString());
                itemDataList.Add(dr["商品グループＣＤ"].ToString());
            }

            cbo.Items = itemList.ToArray();
            cbo.ItemData = itemDataList.ToArray();
            cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
        }

        /// <summary>
        /// プリンタコンボ作成
        /// </summary>
        /// <param name="cbo"></param>
        public static void SetPrinter(ComboBox cbo)
        {
            cbo.DataSource = Printers.PrinterNameOptions;     // 未検証
        }

        /// <summary>
        /// プリンタコンボ作成（スプレッド用）
        /// </summary>
        /// <param name="cbo"></param>
        public static void SetPrinter(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            List<string> itemList = new List<string>();         // 表示用リスト
            List<string> itemDataList = new List<string>();     // 値用リスト

            foreach (String s in Printers.PrinterNameOptions)
            {
                itemList.Add(s);
                itemDataList.Add(s);
            }

            cbo.Items = itemList.ToArray();
            cbo.ItemData = itemDataList.ToArray();
            cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
        }

        /// <summary>
        /// 商品グループデータテーブル取得
        /// </summary>
        private static DataTable GetSyohinGroup()
        {
            DataTable dt = null;

            // 商品グループマスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();

                string sql = "\r\n SELECT" +
                             "\r\n     -1 AS 商品グループＣＤ" +
                             "\r\n     ,'' AS 商品グループ名 " +
                             "\r\n UNION " +
                             "\r\n SELECT" +
                             "\r\n     商品グループＣＤ" +
                             "\r\n     ,REPLACE(STR(商品グループＣＤ, (SELECT LEN(MAX(商品グループＣＤ)) FROM 商品グループマスタ)), ' ', '0') + ':' + 商品グループ名 AS 商品グループ名" +
                             "\r\n FROM" +
                             "\r\n     商品グループマスタ" +
                             "\r\n WHERE" +
                             "\r\n     ISNULL(無効フラグ, 0) = 0" +
                             "\r\n AND 企業ＣＤ = @企業ＣＤ" +
                             "\r\n ORDER BY" +
                             "\r\n     商品グループＣＤ";

                SqlCommand cmd = con.CreateCommand();
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.CommandText = sql;

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// 振替用EB口座コンボボックス作成
        /// </summary>
        /// <param name="cbo">振替用EB口座コンボボックス</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        public static void SetBusyoEB(ComboBox cbo, int? busyoCD)
        {
            cbo.DisplayMember = "EBセット名";
            cbo.ValueMember = "銀行支店ＣＤ";

            // 部署EB設定マスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            cbo.DataSource = GetBusyoEB(busyoCD);
        }

        /// <summary>
        /// 部署EB設定マスタ取得
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        private static DataTable GetBusyoEB(int? busyoCD)
        {
            DataTable dt = null;

            // 部署EB設定マスタに依存するので、一度読み込んだものを保持しておくのではなく、常に読み込み直す
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     '' AS 銀行ＣＤ" +
                                  "\r\n     ,'' AS 支店ＣＤ" +
                                  "\r\n     ,'' AS 銀行支店ＣＤ" +
                                  "\r\n     ,'' AS EBセット名" +
                                  "\r\n UNION" +
                                  "\r\n SELECT" +
                                  "\r\n     A.銀行ＣＤ" +
                                  "\r\n     ,A.支店ＣＤ" +
                                  "\r\n     ,A.銀行ＣＤ + A.支店ＣＤ AS 銀行支店ＣＤ" +
                                  "\r\n     ,A.EBセット名" +
                                  "\r\n FROM" +
                                  "\r\n     部署EB設定マスタ AS A" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                  "\r\n ORDER BY" +
                                  "\r\n     銀行ＣＤ" +
                                  "\r\n     ,支店ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// テーブルの内容取得
        /// </summary>
        /// <param name="tc">テーブルコンテナ</param>
        private static void GetTable(TableContainer tc)
        {
            GetTable(tc, "");
        }

        /// <summary>
        /// テーブルの内容取得
        /// </summary>
        /// <param name="tc">テーブルコンテナ</param>
        /// <param name="criteria">WHERE条件文</param>
        private static void GetTable(TableContainer tc, string criteria)
        {
            if (tc.Table == null)
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    string sql = "\r\n SELECT" +
                                 "\r\n     -1 AS {0}CD" +
                                 "\r\n     ,'' AS {0}名 " +
                                 "\r\n     ,-1 AS 表示順" +
                                 "\r\n UNION " +
                                 "\r\n SELECT" +
                                 "\r\n     {0}CD" +
                                 "\r\n     ,REPLACE(STR({0}CD, (SELECT LEN(MAX({0}CD)) FROM V_{0})), ' ', '0') + ':' + {0}名 AS {0}名" +
                                 "\r\n     ,表示順" +
                                 "\r\n FROM" +
                                 "\r\n     V_{0}" +
                                 "\r\n {1}" +
                                 "\r\n ORDER BY" +
                                 "\r\n     表示順";

                    SqlCommand cmd = con.CreateCommand();
                    if (string.IsNullOrEmpty(criteria) == false)
                    {
                        criteria = "WHERE " + criteria;
                    }
                    cmd.CommandText = string.Format(sql, tc.Name, criteria);

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    tc.Table = new DataTable();
                    adp.Fill(tc.Table);
                }
            }
        }

        /// <summary>
        /// コンボボックスのプロパティを設定
        /// </summary>
        /// <param name="cbo">コンボボックス</param>
        /// <param name="tc">テーブルコンテナ</param>
        private static void SetCombo(ComboBox cbo, TableContainer tc)
        {
            cbo.DisplayMember = tc.Name + "名";
            cbo.ValueMember = tc.Name + "CD";
            cbo.DataSource = tc.Table.Copy();
        }

        /// <summary>
        /// コンボボックスのプロパティを設定
        /// </summary>
        /// <param name="cbo">スプレッド用コンボボックス</param>
        /// <param name="tc">テーブルコンテナ</param>
        private static void SetCombo(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo, TableContainer tc)
        {
            List<string> itemList = new List<string>();         // 表示用リスト
            List<string> itemDataList = new List<string>();     // 値用リスト

            DataTable dt = tc.Table;
            string itemName = tc.Name + "名";
            string itemDataName = tc.Name + "CD";

            foreach (DataRow dr in dt.Rows)
            {
                itemList.Add(dr[itemName].ToString());
                itemDataList.Add(dr[itemDataName].ToString());
            }

            cbo.Items = itemList.ToArray();
            cbo.ItemData = itemDataList.ToArray();
            cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
        }

        #endregion
    }
}

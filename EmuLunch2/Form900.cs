using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 得意先選択ダイアログ
    /// </summary>
    public partial class Form900 : BaseSpreadForm, IDialog
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN = 0;   // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 1;   // 得意先名列インデックス
        private const int TOKUISAKI_RYAKU_COLUMN = 2;   // 得意先名略称列インデックス
        private const int TEL_COLUMN = 3;   // 電話番号列インデックス
        private const int BUSYO_COLUMN = 4;   // 部署名列インデックス
        private const int COURSE_COLUMN = 5;   // コース列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用得意先マスタ
        /// </summary>
        private TokuisakiCriteria _tokuisakiCriteria;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// コース特定用の対象日
        /// </summary>
        public DateTime? TargetDate { get; set; }

        /// <summary>
        /// 選択得意先
        /// </summary>
        public Tokuisaki SelectedTokuisaki { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form900()
        {
            InitializeComponent();
            TargetDate = DateTime.Today;    // システム日付をデフォルト
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.magnifier_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetJotaiKubun(cboJotaiKbn);
                ComboUtil.Set(cboSyushimeYobi,
                            new string[] { "",
                                           string.Format("{0}:日曜日", (int)DayOfWeek.Sunday),
                                           string.Format("{0}:月曜日", (int)DayOfWeek.Monday),
                                           string.Format("{0}:火曜日", (int)DayOfWeek.Tuesday),
                                           string.Format("{0}:水曜日", (int)DayOfWeek.Wednesday),
                                           string.Format("{0}:木曜日", (int)DayOfWeek.Thursday),
                                           string.Format("{0}:金曜日", (int)DayOfWeek.Friday),
                                           string.Format("{0}:土曜日", (int)DayOfWeek.Saturday) },
                            new int[] { -1,
                                        (int)DayOfWeek.Sunday,
                                        (int)DayOfWeek.Monday,
                                        (int)DayOfWeek.Tuesday,
                                        (int)DayOfWeek.Wednesday,
                                        (int)DayOfWeek.Thursday,
                                        (int)DayOfWeek.Friday,
                                        (int)DayOfWeek.Saturday});

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _tokuisakiCriteria = new TokuisakiCriteria();

                if (TargetDate == null)
                {
                    TargetDate = DateTime.Today;
                }

                _tokuisakiCriteria.対象日 = TargetDate;
                tokuisakiBindingSource.DataSource = _tokuisakiCriteria;

                // 対象日のコース区分を取得
                int courseKubun = Course.GetCourseKubun((DateTime)TargetDate);
                lblCourseKubun.Text = ((Consts.CourseKubun)courseKubun).DisplayName();

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "得意先名", "得意先名略称", "部署", "コース" },
                    new string[] { "A.得意先ＣＤ {0}", "A.得意先名 {0}, A.得意先ＣＤ", "A.得意先名略称 {0}, A.得意先ＣＤ",
                                   "A.部署ＣＤ {0}, A.得意先ＣＤ", "C.コースＣＤ {0}, A.得意先ＣＤ" });

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                    // 終了はフォームのCancelButtonなのでEscキー
            }
        }

        /// <summary>
        /// コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCD_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    this.ProcessTabKey(true);                   // 次のコントロールへ
                }
                else
                {
                    // テキストボックスのF1キーにて表示したダイアログが閉じられたときは、フォーカスは元のテキストボックスのままだが
                    // 背景色が消えているので、一旦ボタンにフォーカスを移したのち、元のテキストボックスにフォーカスを戻す
                    ((Button)sender).Select();
                    activeControl.Select();
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// コース選択ダイアログクローズ（コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCD_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 親画面を閉じた後に子画面で閉じられた場合、親画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCD.Text = course.コースＣＤ.ToString();
                    _tokuisakiCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_tokuisakiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tokuisakiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tokuisakiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    if ((int)cboListMax.SelectedValue < 0)
                    {
                        sql.AppendLine("SELECT DISTINCT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT DISTINCT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,A.得意先名");
                    sql.AppendLine("     ,A.得意先名カナ");
                    sql.AppendLine("     ,A.得意先名略称");
                    sql.AppendLine("     ,A.郵便番号");
                    sql.AppendLine("     ,A.住所1");
                    sql.AppendLine("     ,A.住所2");
                    sql.AppendLine("     ,A.電話番号1");
                    sql.AppendLine("     ,A.ＦＡＸ1");
                    sql.AppendLine("     ,A.電話番号2");
                    sql.AppendLine("     ,A.ＦＡＸ2");
                    sql.AppendLine("     ,A.お届け先郵便番号");
                    sql.AppendLine("     ,A.お届け先住所1");
                    sql.AppendLine("     ,A.お届け先住所2");
                    sql.AppendLine("     ,A.お届け先電話番号1");
                    sql.AppendLine("     ,A.お届け先ＦＡＸ1");
                    sql.AppendLine("     ,A.お届け先電話番号2");
                    sql.AppendLine("     ,A.お届け先ＦＡＸ2");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.売掛現金区分");
                    sql.AppendLine("     ,A.電話確認区分");
                    sql.AppendLine("     ,A.締区分");
                    sql.AppendLine("     ,A.月締日");
                    sql.AppendLine("     ,A.週締曜日");
                    sql.AppendLine("     ,A.支払サイト");
                    sql.AppendLine("     ,A.支払日");
                    sql.AppendLine("     ,A.請求先ＣＤ");
                    sql.AppendLine("     ,A.支払方法");
                    sql.AppendLine("     ,A.クレカ顧客番号");
                    sql.AppendLine("     ,A.クレカ番号下4桁");
                    sql.AppendLine("     ,A.クレカ有効期限");
                    sql.AppendLine("     ,A.クレカ無効フラグ");
                    sql.AppendLine("     ,A.クレカ有効性確認日");
                    sql.AppendLine("     ,A.振込手数料フラグ");
                    sql.AppendLine("     ,A.部署合算請求フラグ");
                    sql.AppendLine("     ,A.金融機関ＣＤ");
                    sql.AppendLine("     ,A.金融機関支店ＣＤ");
                    sql.AppendLine("     ,A.記号番号");
                    sql.AppendLine("     ,A.口座種別");
                    sql.AppendLine("     ,A.口座番号");
                    sql.AppendLine("     ,A.口座名義人");
                    sql.AppendLine("     ,A.チケット枚数");
                    sql.AppendLine("     ,A.サービスチケット枚数");
                    sql.AppendLine("     ,A.営業担当者ＣＤ");
                    sql.AppendLine("     ,A.獲得営業者ＣＤ");
                    sql.AppendLine("     ,A.登録担当者ＣＤ");
                    sql.AppendLine("     ,A.受注得意先ＣＤ");
                    sql.AppendLine("     ,A.受注方法");
                    sql.AppendLine("     ,A.電話確認時間_時");
                    sql.AppendLine("     ,A.電話確認時間_分");
                    sql.AppendLine("     ,A.味噌汁区分");
                    sql.AppendLine("     ,A.ふりかけ区分");
                    sql.AppendLine("     ,A.納品書発行区分");
                    sql.AppendLine("     ,A.空き容器回収区分");
                    sql.AppendLine("     ,A.請求書敬称");
                    sql.AppendLine("     ,A.備考１");
                    sql.AppendLine("     ,A.備考２");
                    sql.AppendLine("     ,A.備考３");
                    sql.AppendLine("     ,A.状態区分");
                    sql.AppendLine("     ,A.承認日");
                    sql.AppendLine("     ,A.承認者ＣＤ");
                    sql.AppendLine("     ,A.状態理由区分");
                    sql.AppendLine("     ,A.休日設定");
                    sql.AppendLine("     ,A.新規登録日");
                    sql.AppendLine("     ,A.税区分");
                    sql.AppendLine("     ,A.丸め処理区分");
                    sql.AppendLine("     ,A.祝日配送区分");
                    sql.AppendLine("     ,A.誕生日1");
                    sql.AppendLine("     ,A.誕生日2");
                    sql.AppendLine("     ,A.失客日");
                    sql.AppendLine("     ,A.EB銀行支店ＣＤ");
                    sql.AppendLine("     ,A.受注時特記事項");
                    sql.AppendLine("     ,A.請求書郵便マークフラグ");
                    sql.AppendLine("     ,A.集計ＣＤ");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.配送得意先ＣＤ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,C.コースＣＤ");
                    sql.AppendLine("     ,C.コース名");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     得意先マスタ AS A");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ   AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ   AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ     AS B  ON  B.企業ＣＤ = A.企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0  AND A.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN C_TelToCust    AS TC ON TC.企業ＣＤ = A.企業ＣＤ AND TC.Tel_CustNo = A.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN (");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             CT.コースＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("             ,CM.コース名");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コーステーブル AS CT");
                    sql.AppendLine("             INNER JOIN コースマスタ AS CM ");
                    sql.AppendLine("                 ON CM.企業ＣＤ = CT.企業ＣＤ ");
                    sql.AppendLine("                 AND CM.コースＣＤ = CT.コースＣＤ ");
                    sql.AppendLine("                 AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     臨時コースマスタ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND 実施日 = @対象日");
                    sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND コース区分 = @コース区分");
                    sql.AppendLine("                 ) AS R");
                    sql.AppendLine("                 ON R.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND CM.コース区分 = @コース区分");
                    sql.AppendLine("             AND R.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             RT.コースＣＤ");
                    sql.AppendLine("             ,RT.得意先ＣＤ");
                    sql.AppendLine("             ,RM.コース名");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コーステーブル AS RT");
                    sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM ");
                    sql.AppendLine("                 ON RM.企業ＣＤ = RT.企業ＣＤ ");
                    sql.AppendLine("                 AND RM.コースＣＤ = RT.コースＣＤ ");
                    sql.AppendLine("                 AND RT.実施日 = RM.実施日 ");
                    sql.AppendLine("                 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND RT.実施日 = @対象日");
                    sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RM.コース区分 = @コース区分");
                    sql.AppendLine(" ) AS C");
                    sql.AppendLine(" ON C.得意先ＣＤ = A.得意先ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(A.無効フラグ, 0) = 0");
                    sql.AppendLine(" AND A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_tokuisakiCriteria.得意先ＣＤFrom != null)
                    {
                        sql.AppendLine(" and A.得意先ＣＤ = @得意先ＣＤFrom ");
                        cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _tokuisakiCriteria.得意先ＣＤFrom;
                    }

                    if (_tokuisakiCriteria.得意先名 != null)
                    {
                        // 名称そのままでの検索と、半角/全角スペースを除いた名称でも検索
                        sql.AppendLine(" and (A.得意先名 like @得意先名 escape '#' OR A.得意先名カナ like @得意先名カナ escape '#' OR A.得意先名略称 like @得意先名略称 escape '#'");
                        sql.AppendLine(" OR replace(replace(A.得意先名, ' ', ''), '　', '') like @得意先名 escape '#'");
                        sql.AppendLine(" OR replace(replace(A.得意先名カナ, ' ', ''), '　', '') like @得意先名カナ escape '#'");
                        sql.AppendLine(" OR replace(replace(A.得意先名略称, ' ', ''), '　', '') like @得意先名略称 escape '#')");
                        cmd.Parameters.Add("@得意先名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.得意先名);
                        cmd.Parameters.Add("@得意先名カナ", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.得意先名);
                        cmd.Parameters.Add("@得意先名略称", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.得意先名);
                    }

                    if (_tokuisakiCriteria.電話番号 != null)
                    {
                        // 電話番号はハイフン・スペース・カッコを除いたもので検索
                        sql.AppendLine(" and (replace(replace(replace(replace(A.電話番号1, '-', ''), ' ', ''), '(', ''), ')', '') like @電話番号 escape '#'");
                        sql.AppendLine(" OR replace(replace(replace(replace(A.電話番号2, '-', ''), ' ', ''), '(', ''), ')', '') like @電話番号 escape '#'");
                        sql.AppendLine(" OR replace(replace(replace(replace(TC.Tel_TelNo, '-', ''), ' ', ''), '(', ''), ')', '') like @電話番号 escape '#')");
                        cmd.Parameters.Add("@電話番号", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tokuisakiCriteria.電話番号);
                    }

                    if (_tokuisakiCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine(" and ISNULL(C.コースＣＤ, 0) = @コースＣＤ ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tokuisakiCriteria.コースＣＤ;
                    }

                    if (_tokuisakiCriteria.部署ＣＤ != null && _tokuisakiCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tokuisakiCriteria.部署ＣＤ;
                    }

                    if (_tokuisakiCriteria.状態区分 != null && _tokuisakiCriteria.状態区分 != -1)
                    {
                        sql.AppendLine(" and A.状態区分 = @状態区分 ");
                        cmd.Parameters.Add("@状態区分", SqlDbType.Int).Value = _tokuisakiCriteria.状態区分;
                    }

                    if (_tokuisakiCriteria.月締日 != null)
                    {
                        sql.AppendLine(" and A.月締日 = @月締日");
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _tokuisakiCriteria.月締日;
                    }

                    if (_tokuisakiCriteria.週締曜日 != null && _tokuisakiCriteria.週締曜日 != -1)
                    {
                        sql.AppendLine(" and A.週締曜日 = @週締曜日");
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = _tokuisakiCriteria.週締曜日;
                    }

                    // 対象日(の曜日)からコース区分を求める
                    cmd.Parameters.Add("@対象日", SqlDbType.Date).Value = _tokuisakiCriteria.対象日;
                    int courseKubun = Course.GetCourseKubun((DateTime)_tokuisakiCriteria.対象日);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "得意先マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("得意先マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_RYAKU_COLUMN, "得意先名略称");
                fpSpread1_Sheet1.BindDataColumn(TEL_COLUMN, "電話番号1");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_COLUMN, "コース名");

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    // 検索→行を選択→検索→同じ行を選択 こうすると、bindingSource.Positionが反映されずに0のまま。
                    // （なので詳細画面を開くと、選択した行とは異なるレコードが表示される）
                    // どうも(暗示的／明示的に関わらず)bindingSource.Position=0の設定が無視されているようなので、いったん最終行をカレント行に設定する
                    fpSpread1_Sheet1.ActiveRowIndex = fpSpread1_Sheet1.Rows.Count - 1;
                    bindingSource1.Position = fpSpread1_Sheet1.Rows.Count - 1;

                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    bindingSource1.Position = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _tokuisakiCriteria = new TokuisakiCriteria();

                if (TargetDate == null)
                {
                    TargetDate = DateTime.Today;
                }

                _tokuisakiCriteria.対象日 = TargetDate;
                tokuisakiBindingSource.DataSource = _tokuisakiCriteria;

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount != 0)
                {
                    ShowDetail();       // 詳細表示（既存レコード）
                }
                else
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuit_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Spreadセルダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                // 列ヘッダー or 列フッター or 行ヘッダー？
                if (e.ColumnHeader || e.ColumnFooter || e.RowHeader)
                {
                    return;
                }

                ShowDetail();       // 詳細表示
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Spreadキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    if (fpSpread1_Sheet1.SelectionCount != 0)
                    {
                        ShowDetail();       // 詳細表示
                    }
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド

        /// <summary>
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                SelectedTokuisaki = new Tokuisaki(dr);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

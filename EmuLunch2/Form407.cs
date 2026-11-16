using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 未分配一覧
    /// </summary>
    public partial class Form407 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int URIAGEBI_COLUMN       = 0;    // 売上日列インデックス
        private const int BUSYO_CD_COLUMN       = 1;    // 部署ＣＤ列インデックス
        private const int BUSYO_NAME_COLUMN     = 2;    // 部署名列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 3;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 4;    // 得意先名列インデックス
        private const int COURSE_CD_COLUMN      = 5;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 6;    // コース名列インデックス
        private const int TANTO_CD_COLUMN       = 7;    // コースＣＤ列インデックス
        private const int TANTO_NAME_COLUMN     = 8;    // コース名列インデックス
        private const int SYOHIN_CD_COLUMN      = 9;    // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN    = 10;   // 商品名列インデックス
        private const int URI_KOSU_COLUMN       = 11;   // 売上数数列インデックス
        private const int MIBUNPAI_KOSU_COLUMN  = 12;   // 未分配個数列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 未分配一覧検索条件入力
        /// </summary>
        private MiBunpaiListCriteria _miBunpaiListCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form407()
        {
            InitializeComponent();
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
        private void Form407_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.calculator_ico;

                lblCount.Text = "";

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _miBunpaiListCriteria = new MiBunpaiListCriteria();
                miBunpaiListCriteriaBindingSource.DataSource = _miBunpaiListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "売上日", "部署CD", "得意先CD", "コースCD", "商品CD", "売上数", "未分配個数" },
                    new string[] { "M.売上日 {0}, M.部署ＣＤ, M.得意先ＣＤ, H.コースＣＤ, M.商品ＣＤ",
                                   "M.部署ＣＤ {0}, M.売上日, M.得意先ＣＤ, H.コースＣＤ, M.商品ＣＤ",
                                   "M.得意先ＣＤ {0}, M.売上日, M.部署ＣＤ, H.コースＣＤ, M.商品ＣＤ",
                                   "H.コースＣＤ {0}, M.売上日, M.部署ＣＤ, M.得意先ＣＤ, M.商品ＣＤ",
                                   "M.商品ＣＤ {0}, M.売上日, M.部署ＣＤ, M.得意先ＣＤ, H.コースＣＤ",
                                   "M.売上数 {0}, M.売上日, M.部署ＣＤ, M.得意先ＣＤ, H.コースＣＤ, M.商品ＣＤ",
                                   "M.未分配個数 {0}, M.売上日, M.部署ＣＤ, M.得意先ＣＤ, H.コースＣＤ, M.商品ＣＤ"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                rdoDesc.Checked = true;
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
        private void Form407_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }


        /// <summary>
        /// コースＣＤFromキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤFrom参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDFrom_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCDFrom_FormClosed);
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
        /// コース選択ダイアログクローズ（コースＣＤFrom）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCDFrom_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtCourseCDFrom, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDFrom.Text = course.コースＣＤ.ToString();
                    _miBunpaiListCriteria.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameFrom.Text = course.コース名;
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
        ///コースＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDTo.PerformClick();
            }
        }

        /// <summary>
        ///コースＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDTo_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCDTo_FormClosed);
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
        /// コース選択ダイアログクローズ（コースＣＤTo）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCDTo_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtCourseCDTo, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDTo.Text = course.コースＣＤ.ToString();
                    _miBunpaiListCriteria.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameTo.Text = course.コース名;
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
                if (_miBunpaiListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_miBunpaiListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _miBunpaiListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    if ((int)cboListMax.SelectedValue < 0)
                    {
                        sql.AppendLine("SELECT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     M.売上日");
                    sql.AppendLine("     ,M.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,M.得意先ＣＤ");
                    sql.AppendLine("     ,H.得意先名");
                    sql.AppendLine("     ,H.コースＣＤ");
                    sql.AppendLine("     ,C.コース名");
                    sql.AppendLine("     ,H.配送担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 配送担当者名");
                    sql.AppendLine("     ,M.商品ＣＤ");
                    sql.AppendLine("     ,S.商品名");
                    sql.AppendLine("     ,M.売上数");
                    sql.AppendLine("     ,M.未分配個数");
                    sql.AppendLine("     ,M.分配未着手");
                    sql.AppendLine("     ,M.分配未完了");
                    sql.AppendLine("     ,M.分配完了");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT DISTINCT");
                    sql.AppendLine("         UM.企業ＣＤ");
                    sql.AppendLine("         ,UM.売上日");
                    sql.AppendLine("         ,UM.得意先ＣＤ");
                    sql.AppendLine("         ,UM.部署ＣＤ");
                    sql.AppendLine("         ,UM.明細番号");
                    sql.AppendLine("         ,UM.商品ＣＤ");
                    sql.AppendLine("         ,UM.商品グループＣＤ");
                    sql.AppendLine("         ,UM.売上区分");
                    sql.AppendLine("         ,UM.商品単価");
                    sql.AppendLine("         ,UM.商品数量");
                    sql.AppendLine("         ,UM.値引額");
                    sql.AppendLine("         ,UM.値引事由区分");
                    sql.AppendLine("         ,UM.値引前金額");
                    sql.AppendLine("         ,UM.値引後金額");
                    sql.AppendLine("         ,UM.消費税額");
                    sql.AppendLine("         ,UM.商品金額合計税込");
                    sql.AppendLine("         ,UM.計算税率");
                    sql.AppendLine("         ,UM.チケット束フラグ");
                    sql.AppendLine("         ,UM.現金補填フラグ");
                    sql.AppendLine("         ,UM.主食ＣＤ");
                    sql.AppendLine("         ,UM.副食ＣＤ");
                    sql.AppendLine("         ,UM.食事区分");
                    sql.AppendLine("         ,UM.分配元数量");
                    sql.AppendLine("         ,UM.作成担当者ＣＤ");
                    sql.AppendLine("         ,UM.作成日");
                    sql.AppendLine("         ,UM.修正担当者ＣＤ");
                    sql.AppendLine("         ,UM.修正日");
                    sql.AppendLine("         ,ISNULL(UM.分配元数量, 0) + UM.商品数量 AS 売上数");
                    sql.AppendLine("         ,UM.商品数量 AS 未分配個数");
                    sql.AppendLine("         ,CASE WHEN ISNULL(UM.分配元数量, 0) = 0 THEN 1 ELSE 0 END AS 分配未着手");
                    sql.AppendLine("         ,CASE WHEN ISNULL(UM.商品数量, 0) <> 0 AND ISNULL(UM.分配元数量, 0) > 0 THEN 1 ELSE 0 END AS 分配未完了");
                    sql.AppendLine("         ,CASE WHEN ISNULL(UM.商品数量, 0) = 0 AND ISNULL(UM.分配元数量, 0) > 0 THEN 1 ELSE 0 END AS 分配完了");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上明細 AS UM");
                    // --分配先が存在する得意先（＝分配元）を抽出する（受注得意先として指定されている得意先を表示対象とする）
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T");
                    sql.AppendLine("             ON T.企業ＣＤ = UM.企業ＣＤ");
                    sql.AppendLine("             AND T.受注得意先ＣＤ = UM.得意先ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         UM.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND UM.売上区分 = @売上区分売掛");
                    sql.AppendLine("         AND ISNULL(UM.商品数量, 0) + ISNULL(UM.分配元数量, 0) <> 0");  // -- そもそも売上が無ければ表示する必要は無い
                    sql.AppendLine("         AND T.受注得意先ＣＤ IS NOT NULL");
                    sql.AppendLine("     ) AS M");
                    sql.AppendLine("     INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("         ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("         AND H.売上日     = M.売上日");
                    sql.AppendLine("         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("         AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ   AS B  ON B.企業ＣＤ  = M.企業ＣＤ AND B.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS C  ON C.企業ＣＤ  = H.企業ＣＤ AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = H.企業ＣＤ AND H.配送担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ   AS S  ON S.企業ＣＤ  = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;

                    if (_miBunpaiListCriteria.部署ＣＤ != null && _miBunpaiListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("     AND M.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _miBunpaiListCriteria.部署ＣＤ;
                    }

                    if (_miBunpaiListCriteria.売上日From != null)
                    {
                        sql.AppendLine("     AND M.売上日 >= @売上日From");
                        cmd.Parameters.Add("@売上日From", SqlDbType.Date).Value = _miBunpaiListCriteria.売上日From;
                    }

                    if (_miBunpaiListCriteria.売上日To != null)
                    {
                        sql.AppendLine("     AND M.売上日 <= @売上日To");
                        cmd.Parameters.Add("@売上日To", SqlDbType.Date).Value = _miBunpaiListCriteria.売上日To;
                    }

                    if (_miBunpaiListCriteria.コースＣＤFrom != null)
                    {
                        sql.AppendLine("     AND H.コースＣＤ >= @コースＣＤFrom");
                        cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _miBunpaiListCriteria.コースＣＤFrom;
                    }

                    if (_miBunpaiListCriteria.コースＣＤTo != null)
                    {
                        sql.AppendLine("     AND H.コースＣＤ <= @コースＣＤTo");
                        cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _miBunpaiListCriteria.コースＣＤTo;
                    }

                    // 分配途中を表示する（分配途中フラグ）がONの場合は、分配未完了を検索条件から除外する
                    if (_miBunpaiListCriteria.分配途中フラグ != true)
                    {
                        sql.AppendLine("     AND M.分配未完了 = 0");
                    }

                    // 分配済みを表示する（分配完了フラグ）がONの場合は、分配完了を検索条件から除外する
                    if (_miBunpaiListCriteria.分配完了フラグ != true)
                    {
                        sql.AppendLine("     AND M.分配完了 = 0");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("未分配一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "未分配一覧";

                    log.Info("未分配一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(URIAGEBI_COLUMN,       "売上日");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN,       "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,     "部署名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TANTO_CD_COLUMN,       "配送担当者ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TANTO_NAME_COLUMN,     "配送担当者名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,      "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN,    "商品名");
                fpSpread1_Sheet1.BindDataColumn(URI_KOSU_COLUMN,       "売上数");
                fpSpread1_Sheet1.BindDataColumn(MIBUNPAI_KOSU_COLUMN,  "未分配個数");

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    // 検索→行を選択→検索→同じ行を選択 こうすると、bindingSource.Positionが反映されずに0のまま。
                    // （なので詳細画面を開くと、選択した行とは異なるレコードが表示される）
                    // どうも(暗示的／明示的に関わらず)bindingSource.Position=0の設定が無視されているようなので、いったん最終行をカレント行に設定する
                    // ※この画面はなぜか上記現象は発生しないが、処理を統一したいので適用している
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
                this.MdiParent.Cursor = Cursors.Default;
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
                _miBunpaiListCriteria = new MiBunpaiListCriteria();
                miBunpaiListCriteriaBindingSource.DataSource = _miBunpaiListCriteria;

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
        /// 分配入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBunpai_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount == 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowBunpai();       // 分配入力表示（既存レコード）
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
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                saveFileDialog1.FileName = this.Text;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = saveFileDialog1.FileName;
                    FarPoint.Excel.ExcelSaveFlags excelSaveFlags = FarPoint.Excel.ExcelSaveFlags.SaveCustomColumnHeaders;
                    if (fileName.ToLower().EndsWith("xlsx"))
                    {
                        excelSaveFlags |= FarPoint.Excel.ExcelSaveFlags.UseOOXMLFormat;
                    }
                    fpSpread1.SaveExcel(fileName, excelSaveFlags);
                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

                ShowBunpai();       // 詳細表示
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
                        ShowBunpai();       // 分配入力表示
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
        /// 売掛分配入力表示（既存レコード）
        /// </summary>
        private void ShowBunpai()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                UrikakeBunpaiCriteria urikakeBunpaiCriteria = new UrikakeBunpaiCriteria();
                urikakeBunpaiCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                urikakeBunpaiCriteria.部署ＣＤ = dr["部署ＣＤ"] as int?;
                urikakeBunpaiCriteria.売上日 = dr["売上日"] as DateTime?;
                Form406 form = new Form406(urikakeBunpaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form406_FormClosed);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売掛分配入力画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form406_FormClosed(object sender, FormClosedEventArgs e)
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

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 再検索

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread1.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = activeRowIndex;
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
                    fpSpread1_Sheet1.AddSelection(activeRowIndex, -1, 1, -1);   // 単一選択行を復元
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
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
    }
}

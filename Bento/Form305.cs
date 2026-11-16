using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// コース出荷状態
    /// </summary>
    public partial class Form305 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int HAISOBI_COLUMN        = 0;    // 配送日列インデックス
        private const int COURSE_CD_COLUMN      = 1;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 2;    // コース名列インデックス
        private const int SYUKKASU_COLUMN       = 3;    // 出荷数列インデックス
        private const int COMPLETE_COLUMN       = 4;    // 出荷完了日時列インデックス
        private const int KOSU_COLUMN           = 5;    // 個数列インデックス
        private const int TANTOUSYA_NAME_COLUMN = 6;    // 担当者名列インデックス
        private const int DETAIL_COLUMN         = 7;    // 詳細ボタン列インデックス

        /// <summary>
        /// 出荷数と個口数が異なる場合の背景色
        /// </summary>
        private readonly Color CHUMON_NOT_EQUAL_KOSU_BACK_COLOR = Color.PapayaWhip;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// コース出荷状態検索条件
        /// </summary>
        private ShippingStatusCriteria _shippingStatusCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form305()
        {
            try
            {
                InitializeComponent();
            }
            catch (BentoException ex)
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
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form305_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread1.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _shippingStatusCriteria = new ShippingStatusCriteria();
                shippingStatusCriteriaBindingSource.DataSource = _shippingStatusCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "配送日", "コースCD" },
                    new string[] { "A.配送日 {0}, A.コースＣＤ, B.出荷完了日時 DESC",
                                   "A.コースＣＤ {0}, A.配送日, B.出荷完了日時 DESC"});
            }
            catch (BentoException ex)
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
        private void Form305_KeyDown(object sender, KeyEventArgs e)
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
            catch (BentoException ex)
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
                    _shippingStatusCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtCourseCD);
                    ReadBinding(lblCourseName);
                }
            }
            catch (BentoException ex)
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_shippingStatusCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_shippingStatusCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _shippingStatusCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("    A.配送日");
                    sql.AppendLine("    ,A.部署ＣＤ");
                    sql.AppendLine("    ,A.コースＣＤ");
                    sql.AppendLine("    ,A.コース名");
                    sql.AppendLine("    ,A.出荷数");
                    sql.AppendLine("    ,B.担当者ＣＤ");
                    sql.AppendLine("    ,B.担当者名");
                    sql.AppendLine("    ,B.出荷完了日時");
                    sql.AppendLine("    ,B.個数");
                    sql.AppendLine("FROM");
                    // -- 駆動表兼出荷数（個口数の合計）
                    sql.AppendLine("    (SELECT");
                    sql.AppendLine("        SM.企業ＣＤ");
                    sql.AppendLine("        ,SM.配送日");
                    sql.AppendLine("        ,SM.部署ＣＤ");
                    sql.AppendLine("        ,SM.コースＣＤ");
                    sql.AppendLine("        ,SM.コース名");
                    sql.AppendLine("        ,SUM(SM.個口数) AS 出荷数");
                    sql.AppendLine("    FROM");
                    // -- 指定した配送日・コースで、同じ得意先・食事区分の最新のレコードを抽出する
                    // -- ※短冊を複数回出力することがあるので、最新のレコードを優先する必要がある
                    sql.AppendLine("        (SELECT");
                    sql.AppendLine("            MAX(出荷番号) AS 出荷番号");
                    sql.AppendLine("            ,企業ＣＤ");
                    sql.AppendLine("            ,部署ＣＤ");
                    sql.AppendLine("            ,得意先ＣＤ");
                    sql.AppendLine("            ,食事区分");
                    sql.AppendLine("        FROM");
                    sql.AppendLine("            SP_T_出荷明細");
                    sql.AppendLine("        WHERE");
                    sql.AppendLine("            企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_shippingStatusCriteria.配送日From != null)
                    {
                        sql.AppendLine("            AND 配送日 >= @配送日From");
                        cmd.Parameters.Add("@配送日From", SqlDbType.Date).Value = _shippingStatusCriteria.配送日From;
                    }

                    if (_shippingStatusCriteria.配送日To != null)
                    {
                        sql.AppendLine("            AND 配送日 <= @配送日To");
                        cmd.Parameters.Add("@配送日To", SqlDbType.Date).Value = _shippingStatusCriteria.配送日To;
                    }

                    if (_shippingStatusCriteria.部署ＣＤ != null && _shippingStatusCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("            AND 部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _shippingStatusCriteria.部署ＣＤ;
                    }

                    if (_shippingStatusCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("            AND コースＣＤ = @コースＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _shippingStatusCriteria.コースＣＤ;
                    }

                    sql.AppendLine("        GROUP BY");
                    sql.AppendLine("            企業ＣＤ");
                    sql.AppendLine("            ,部署ＣＤ");
                    sql.AppendLine("            ,得意先ＣＤ");
                    sql.AppendLine("            ,食事区分");
                    sql.AppendLine("        ) AS M1");
                    sql.AppendLine("        INNER JOIN SP_T_出荷明細 SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号");
                    sql.AppendLine("    GROUP BY");
                    sql.AppendLine("        SM.企業ＣＤ");
                    sql.AppendLine("        ,SM.配送日");
                    sql.AppendLine("        ,SM.部署ＣＤ");
                    sql.AppendLine("        ,SM.コースＣＤ");
                    sql.AppendLine("        ,SM.コース名");
                    sql.AppendLine("    ) AS A");
                    sql.AppendLine("    LEFT JOIN");
                    // -- 出荷履歴
                    sql.AppendLine("        (SELECT");
                    sql.AppendLine("            SH.企業ＣＤ");
                    sql.AppendLine("            ,SM.配送日");
                    sql.AppendLine("            ,SM.コースＣＤ");
                    sql.AppendLine("            ,SM.コース名");
                    sql.AppendLine("            ,SH.担当者ＣＤ");
                    sql.AppendLine("            ,T.担当者名");
                    sql.AppendLine("            ,MAX(SH.出荷完了日時) AS 出荷完了日時");
                    sql.AppendLine("            ,COUNT(SH.出荷個口番号) AS 個数");
                    sql.AppendLine("        FROM");
                    // -- 指定した配送日・コースで、同じ得意先・食事区分の最新のレコードを抽出する
                    // -- ※短冊を複数回出力することがあるので、最新のレコードを優先する必要がある
                    sql.AppendLine("            (SELECT");
                    sql.AppendLine("                MAX(出荷番号) AS 出荷番号");
                    sql.AppendLine("                ,企業ＣＤ");
                    sql.AppendLine("                ,部署ＣＤ");
                    sql.AppendLine("                ,得意先ＣＤ");
                    sql.AppendLine("                ,食事区分");
                    sql.AppendLine("            FROM");
                    sql.AppendLine("                SP_T_出荷明細");
                    sql.AppendLine("            WHERE");
                    sql.AppendLine("                企業ＣＤ = @企業ＣＤ");

                    if (_shippingStatusCriteria.配送日From != null)
                    {
                        sql.AppendLine("                AND 配送日 >= @配送日From");
                    }

                    if (_shippingStatusCriteria.配送日To != null)
                    {
                        sql.AppendLine("                AND 配送日 <= @配送日To");
                    }

                    if (_shippingStatusCriteria.部署ＣＤ != null && _shippingStatusCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("                AND 部署ＣＤ = @部署ＣＤ");
                    }

                    if (_shippingStatusCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("                AND コースＣＤ = @コースＣＤ");
                    }

                    sql.AppendLine("            GROUP BY");
                    sql.AppendLine("                企業ＣＤ");
                    sql.AppendLine("                ,部署ＣＤ");
                    sql.AppendLine("                ,得意先ＣＤ");
                    sql.AppendLine("                ,食事区分");
                    sql.AppendLine("            ) AS M1");
                    sql.AppendLine("            INNER JOIN SP_T_出荷明細 AS SM ON SM.企業ＣＤ = M1.企業ＣＤ AND SM.出荷番号 = M1.出荷番号");
                    sql.AppendLine("            INNER JOIN SP_T_出荷履歴 AS SH ON SH.企業ＣＤ = SM.企業ＣＤ AND SH.出荷番号 = SM.出荷番号");
                    sql.AppendLine("            LEFT JOIN 担当者マスタ AS T ON SH.企業ＣＤ = T.企業ＣＤ AND SH.担当者ＣＤ = T.担当者ＣＤ");
                    sql.AppendLine("        GROUP BY");
                    sql.AppendLine("            SH.企業ＣＤ");
                    sql.AppendLine("            ,SM.配送日");
                    sql.AppendLine("            ,SM.コースＣＤ");
                    sql.AppendLine("            ,SM.コース名");
                    sql.AppendLine("            ,SH.担当者ＣＤ");
                    sql.AppendLine("            ,T.担当者名");
                    sql.AppendLine("        ) AS B ON A.企業ＣＤ = B.企業ＣＤ AND A.配送日 = B.配送日 AND A.コースＣＤ = B.コースＣＤ");
                    sql.AppendLine("ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コース出荷状態");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    //lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "コース出荷状態";

                    log.Info("コース出荷状態を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(HAISOBI_COLUMN,        "配送日");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(SYUKKASU_COLUMN,       "出荷数");
                fpSpread1_Sheet1.BindDataColumn(COMPLETE_COLUMN,       "出荷完了日時");
                fpSpread1_Sheet1.BindDataColumn(KOSU_COLUMN,           "個数");
                fpSpread1_Sheet1.BindDataColumn(TANTOUSYA_NAME_COLUMN, "担当者名");

                // 同じ配送日・コースCD・コース名が連続する場合はセルを行方向に結合する
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    int startRowIndex = 0;
                    DateTime haisobi = (DateTime)(fpSpread1_Sheet1.Cells[0, HAISOBI_COLUMN].Value);
                    int? courseCD = fpSpread1_Sheet1.Cells[0, COURSE_CD_COLUMN].Value as int?;
                    string courseName = fpSpread1_Sheet1.Cells[0, COURSE_NAME_COLUMN].Text;

                    int rowIndex = 0;
                    for (; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                    {
                        DateTime haisobi2 = (DateTime)(fpSpread1_Sheet1.Cells[rowIndex, HAISOBI_COLUMN].Value);
                        int? courseCD2 = fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Value as int?;
                        string courseName2 = fpSpread1_Sheet1.Cells[rowIndex, COURSE_NAME_COLUMN].Text;

                        if (haisobi != haisobi2 || courseCD != courseCD2 || courseName != courseName2)
                        {
                            // ２行以上なら結合する
                            if (rowIndex - startRowIndex >= 2)
                            {
                                fpSpread1_Sheet1.AddSpanCell(startRowIndex, HAISOBI_COLUMN, rowIndex - startRowIndex, 1);
                                fpSpread1_Sheet1.AddSpanCell(startRowIndex, COURSE_CD_COLUMN, rowIndex - startRowIndex, 1);
                                fpSpread1_Sheet1.AddSpanCell(startRowIndex, COURSE_NAME_COLUMN, rowIndex - startRowIndex, 1);
                            }
                            haisobi = haisobi2;
                            courseCD = courseCD2;
                            courseName = courseName2;
                            startRowIndex = rowIndex;
                        }

                        // ついでに出荷数＞個数なら個数を着色
                        int syukkasu = (int)(fpSpread1_Sheet1.Cells[rowIndex, SYUKKASU_COLUMN].Value);
                        int? kosu = fpSpread1_Sheet1.Cells[rowIndex, KOSU_COLUMN].Value as int?;
                        if (kosu != null && syukkasu > kosu)
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, KOSU_COLUMN].ForeColor = Color.Red;
                            fpSpread1_Sheet1.Cells[rowIndex, COMPLETE_COLUMN].BackColor = CHUMON_NOT_EQUAL_KOSU_BACK_COLOR;
                            fpSpread1_Sheet1.Cells[rowIndex, KOSU_COLUMN].BackColor = CHUMON_NOT_EQUAL_KOSU_BACK_COLOR;
                            fpSpread1_Sheet1.Cells[rowIndex, TANTOUSYA_NAME_COLUMN].BackColor = CHUMON_NOT_EQUAL_KOSU_BACK_COLOR;
                        }
                    }

                    // 最終行を含めて結合
                    if (rowIndex - startRowIndex > 1)
                    {
                        fpSpread1_Sheet1.AddSpanCell(startRowIndex, HAISOBI_COLUMN, rowIndex - startRowIndex, 1);
                        fpSpread1_Sheet1.AddSpanCell(startRowIndex, COURSE_CD_COLUMN, rowIndex - startRowIndex, 1);
                        fpSpread1_Sheet1.AddSpanCell(startRowIndex, COURSE_NAME_COLUMN, rowIndex - startRowIndex, 1);
                        startRowIndex = rowIndex;
                    }
                }

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
                    fpSpread1_Sheet1.SetActiveCell(0, 0);
                }
            }
            catch (BentoException ex)
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
        /// Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 詳細ボタン
                if (e.Column == DETAIL_COLUMN)
                {
                    ShowDetail();       // 詳細表示
                }
            }
            catch (BentoException ex)
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
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= DETAIL_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread1_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
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
            catch (BentoException ex)
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
            catch (BentoException ex)
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
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                ShippingHistoryCriteria shippingHistoryCriteria = new ShippingHistoryCriteria();
                shippingHistoryCriteria.配送日 = dr["配送日"] as DateTime?;
                shippingHistoryCriteria.部署ＣＤ = dr["部署ＣＤ"] as int?;
                shippingHistoryCriteria.コースＣＤ = dr["コースＣＤ"] as int?;
                shippingHistoryCriteria.コース名 = dr["コース名"] as string;
                shippingHistoryCriteria.担当者ＣＤ = dr["担当者ＣＤ"] as int?;
                shippingHistoryCriteria.担当者名 = dr["担当者名"] as string;
                shippingHistoryCriteria.出荷完了日時 = dr["出荷完了日時"] as DateTime?;
                shippingHistoryCriteria.出荷数 = dr["出荷数"] as int?;
                shippingHistoryCriteria.個数 = dr["個数"] as int?;

                // 担当者ＣＤが無い（＝出荷履歴が無い）？
                if (shippingHistoryCriteria.担当者ＣＤ == null)
                {
                    MessageBox.Show(this, "出荷履歴がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
                    return;
                }

                Form306 form = new Form306(shippingHistoryCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form306_FormClosed);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form306_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Form form = (Form)sender;
                form.Dispose();
                form = null;

                // 一覧画面を閉じた後に詳細画面で閉じられた場合、一覧画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();
            }
            catch (BentoException ex)
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

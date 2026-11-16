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
    /// TP決済一覧
    /// </summary>
    public partial class Form512 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int TP_NO_COLUMN = 0;             // TP決済番号列インデックス
        private const int KESSAIBI_COLUMN = 1;          // 決済日列インデックス
        private const int KESSAITAISYOU_COLUMN = 2;     // 決済対象列インデックス
        private const int TORIHIKINICHIJI_COLUMN = 3;   // 取引日時列インデックス
        private const int COURSE_CD_COLUMN = 4;         // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN = 5;       // コース名列インデックス
        private const int TOKUISAKI_CD_COLUMN = 6;      // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 7;    // 得意先名列インデックス
        private const int SYOHIN_CD_COLUMN = 8;         // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 9;       // 商品名列インデックス
        private const int CARDCO_COLUMN = 10;           // 決済方法名列インデックス
        private const int TORIHIKI_KBN_COLUMN = 11;     // 取引区分列インデックス
        private const int KINGAKU_COLUMN = 12;          // 金額列インデックス

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// TP決済検索条件入力
        /// </summary>
        private TPListCriteria _tpListCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        public Form512()
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
        private void Form512_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                cboShiten.SelectedValueChanged -= cboShiten_SelectedValueChanged;   // 支店ＣＤコンボイベントをいったん削除

                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                if (_tpListCriteria == null)
                {
                    _tpListCriteria = new TPListCriteria();
                    _tpListCriteria.決済日From = DateTime.Today;
                    _tpListCriteria.決済日To = DateTime.Today;

                    // 部署CDコンボの初期値を設定
                    _tpListCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
                }

                TPListBindingSource.DataSource = _tpListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "取引日時", "決済日", "コースCD", "得意先CD", "商品CD" },
                    new string[] { "TP.取引日時 {0}, TP.得意先ＣＤ, TP.決済日",
                                   "TP.決済日 {0}, TP.得意先ＣＤ, TP.取引日時",
                                   "C.コースCD {0}, TP.得意先ＣＤ, TP.取引日時, TP.決済日",
                                   "TP.得意先CD {0}, TP.取引日時, TP.決済日",
                                   "D.商品CD {0}, TP.得意先ＣＤ, TP.取引日時, TP.決済日"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // 取引区分の選択肢生成
                ComboUtil.Set(cboTorihikiKbn,
                    new string[] { "", "売上", "取消" },
                    new string[] { "", "売上", "取消" });

                // 決済対象の選択肢生成
                ComboUtil.Set(cboUriageNyukin,
                    new string[] { "", "売上", "入金" },
                    new string[] { "", "売上", "入金" });

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
            finally
            {
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboShiten.SelectedValueChanged += new EventHandler(cboShiten_SelectedValueChanged);  // 支店ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form512_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F8:                       // 詳細画面ボタン押下
                    btnDetail.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form512_Shown(object sender, EventArgs e)
        {
            // 決済日にフォーカス
            dtpKessaibiFrom.Focus();
        }

        /// <summary>
        /// 支店コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShiten_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 部署コンボの選択肢を、支店にぶら下がるものに再設定する
                ComboUtil.SetBusyo(cboBusyo, (int?)cboShiten.SelectedValue);

                // 部署コンボを初期化
                _tpListCriteria.部署ＣＤ = -1;
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
        /// 商品ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSyohinCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSyohinCD.PerformClick();
            }
        }

        /// <summary>
        /// 商品参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyohinCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;

                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCD_FormClosed);
                formSyohinDlg.ShowDialog();

                if (formSyohinDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 商品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form901SyohinCD_FormClosed(object sender, FormClosedEventArgs e)
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

                Form901 form = (Form901)sender;

                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSyohinCD, null);  // エラーを消す

                    Syohin syohin = form.SelectedSyohin;
                    txtSyohinCD.Text = syohin.商品ＣＤ.ToString();
                    _tpListCriteria.商品ＣＤ = syohin.商品ＣＤ;       // 入力内容を確定させる
                    lblSyohinName.Text = syohin.商品名;
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
                    _tpListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
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
        /// 得意先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCD.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;

                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();

                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCD_FormClosed(object sender, FormClosedEventArgs e)
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

                Form900 form = (Form900)sender;

                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _tpListCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;
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
                if (_tpListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tpListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tpListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

                    sql.AppendLine("    TP.企業ＣＤ");
                    sql.AppendLine("    ,TP.TP決済番号");
                    sql.AppendLine("    ,TP.決済日");
                    sql.AppendLine("    ,TP.部署ＣＤ");
                    sql.AppendLine("    ,TP.得意先ＣＤ");
                    sql.AppendLine("    ,TP.売上明細番号");
                    sql.AppendLine("    ,TP.入金番号");
                    sql.AppendLine("    ,TP.取引日時");
                    sql.AppendLine("    ,TP.取引区分");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN H.得意先名 ");
                    sql.AppendLine("        WHEN ISNULL(TP.入金番号, 0) <> 0 THEN N.得意先名 ");
                    sql.AppendLine("        ELSE NULL ");
                    sql.AppendLine("     END AS 得意先名");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN H.コースＣＤ ");
                    sql.AppendLine("        ELSE NULL ");
                    sql.AppendLine("     END AS コースＣＤ");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN C.コース名 ");
                    sql.AppendLine("        ELSE NULL ");
                    sql.AppendLine("     END AS コース名");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN D.商品ＣＤ ");
                    sql.AppendLine("        ELSE NULL ");
                    sql.AppendLine("     END AS 商品ＣＤ");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN S.商品名 ");
                    sql.AppendLine("        ELSE NULL ");
                    sql.AppendLine("     END AS 商品名");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(クレジットカード会社名, '') <> '' THEN クレジットカード会社名");
                    sql.AppendLine("        WHEN ISNULL(電子マネーブランド名, '') <> '' THEN 電子マネーブランド名");
                    sql.AppendLine("        ELSE NULL");
                    sql.AppendLine("     END AS 決済方法名");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN ISNULL(TP.売上明細番号, 0) <> 0 THEN '売上' ");
                    sql.AppendLine("        WHEN ISNULL(TP.入金番号, 0) <> 0 THEN '入金' ");
                    sql.AppendLine("        ELSE NULL ");
                    sql.AppendLine("     END AS 決済対象");
                    sql.AppendLine("    ,CASE");
                    sql.AppendLine("        WHEN TP.取引区分 IN ('売上', 'CL 売上') THEN TP.取引金額 ");
                    sql.AppendLine("        WHEN TP.取引区分 IN ('取消', 'CL 取消') THEN TP.取引金額 * -1 ");
                    sql.AppendLine("        ELSE TP.取引金額 ");
                    sql.AppendLine("     END AS 取引金額");
                    sql.AppendLine(" FROM ");
                    sql.AppendLine("    TP決済情報 AS TP");
                    sql.AppendLine("    LEFT JOIN 売上明細     AS D ON D.企業ＣＤ = TP.企業ＣＤ AND D.売上日     = TP.決済日 AND D.得意先ＣＤ = TP.得意先ＣＤ AND D.部署ＣＤ = TP.部署ＣＤ AND D.明細番号 = TP.売上明細番号");
                    sql.AppendLine("    LEFT JOIN 売上ヘッダ   AS H ON H.企業ＣＤ = TP.企業ＣＤ AND H.売上日     = D.売上日  AND H.得意先ＣＤ = D.得意先ＣＤ  AND H.部署ＣＤ = D.部署ＣＤ");
                    sql.AppendLine("    LEFT JOIN 入金明細     AS N ON N.企業ＣＤ = TP.企業ＣＤ AND N.入金日     = TP.決済日 AND N.得意先ＣＤ = TP.得意先ＣＤ AND N.部署ＣＤ = TP.部署ＣＤ AND N.入金番号 = TP.入金番号");
                    sql.AppendLine("    LEFT JOIN 商品マスタ   AS S ON S.企業ＣＤ = D.企業ＣＤ  AND S.商品ＣＤ   = D.商品ＣＤ");
                    sql.AppendLine("    LEFT JOIN コースマスタ AS C ON C.企業ＣＤ = H.企業ＣＤ  AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("    LEFT JOIN 部署マスタ   AS B ON B.企業ＣＤ = TP.企業ＣＤ AND B.部署ＣＤ   = TP.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("    TP.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_tpListCriteria.支店ＣＤ != null && _tpListCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine("    AND B.支店ＣＤ = @支店ＣＤ ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _tpListCriteria.支店ＣＤ;
                    }

                    if (_tpListCriteria.部署ＣＤ != null && _tpListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("    AND TP.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tpListCriteria.部署ＣＤ;
                    }

                    if (_tpListCriteria.決済日From != null)
                    {
                        sql.AppendLine("    AND TP.決済日 >= @決済日From ");
                        cmd.Parameters.Add("@決済日From", SqlDbType.Date).Value = _tpListCriteria.決済日From;
                    }

                    if (_tpListCriteria.決済日To != null)
                    {
                        sql.AppendLine("    AND TP.決済日 <= @決済日To ");
                        cmd.Parameters.Add("@決済日To", SqlDbType.Date).Value = _tpListCriteria.決済日To;
                    }

                    if (_tpListCriteria.商品ＣＤ != null)
                    {
                        sql.AppendLine("    AND D.商品ＣＤ = @商品ＣＤ ");
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _tpListCriteria.商品ＣＤ;
                    }

                    if (_tpListCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine("    AND C.コースＣＤ = @コースＣＤ ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _tpListCriteria.コースＣＤ;
                    }

                    if (_tpListCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("    AND TP.得意先ＣＤ = @得意先ＣＤ ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tpListCriteria.得意先ＣＤ;
                    }

                    if (_tpListCriteria.決済対象 == "売上")
                    {
                        sql.AppendLine("    AND TP.売上明細番号 > 0 ");
                    }

                    if (_tpListCriteria.決済対象 == "入金")
                    {
                        sql.AppendLine("    AND TP.入金番号 > 0 ");
                    }

                    if (_tpListCriteria.取引区分 != null && _tpListCriteria.取引区分 != "")
                    {
                        //sql.AppendLine("    AND TP.取引区分 = @取引区分 ");
                        //cmd.Parameters.Add("@取引区分", SqlDbType.VarChar).Value = _tpListCriteria.取引区分;

                        sql.AppendLine("    AND TP.取引区分 LIKE @取引区分 escape '#' ");
                        cmd.Parameters.Add("@取引区分", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tpListCriteria.取引区分);
                    }

                    if (_tpListCriteria.決済方法名 != null && _tpListCriteria.決済方法名 != "")
                    {
                        // 名称そのままでの検索と、半角/全角スペースを除いた名称でも検索
                        sql.AppendLine("    AND (TP.クレジットカード会社名 LIKE @決済方法名 escape '#' OR TP.電子マネーブランド名 LIKE @決済方法名 escape '#'");
                        sql.AppendLine("        OR replace(replace(TP.クレジットカード会社名, ' ', ''), '　', '') LIKE @決済方法名 escape '#'");
                        sql.AppendLine("        OR replace(replace(TP.電子マネーブランド名, ' ', ''), '　', '') LIKE @決済方法名 escape '#')");
                        cmd.Parameters.Add("@決済方法名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tpListCriteria.決済方法名);
                    }

                    sql.AppendLine(" ORDER BY");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("TP決済一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 合計行を追加する
                    dt.Rows.Add(dt.NewRow());

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "TP決済一覧";

                    log.Info("TP決済一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TP_NO_COLUMN, "TP決済番号");
                fpSpread1_Sheet1.BindDataColumn(KESSAIBI_COLUMN, "決済日");
                fpSpread1_Sheet1.BindDataColumn(KESSAITAISYOU_COLUMN, "決済対象");
                fpSpread1_Sheet1.BindDataColumn(TORIHIKINICHIJI_COLUMN, "取引日時");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN, "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");
                fpSpread1_Sheet1.BindDataColumn(CARDCO_COLUMN, "決済方法名");
                fpSpread1_Sheet1.BindDataColumn(TORIHIKI_KBN_COLUMN, "取引区分");
                fpSpread1_Sheet1.BindDataColumn(KINGAKU_COLUMN, "取引金額");

                // 背景色をリセット
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, TORIHIKI_KBN_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, KINGAKU_COLUMN];
                    FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, KINGAKU_COLUMN];
                    FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, KINGAKU_COLUMN];
                    sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
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
                _tpListCriteria = new TPListCriteria();
                _tpListCriteria.決済日From = DateTime.Today;
                _tpListCriteria.決済日To = DateTime.Today;

                // 部署CDコンボの初期値を設定
                _tpListCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                TPListBindingSource.DataSource = _tpListCriteria;

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
        /// 照会ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInquiry_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount == 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.ActiveRowIndex == fpSpread1_Sheet1.Rows.Count - 1)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DataRow dr = ((DataRowView)bindingSource1.Current).Row;

                if ((int)dr["売上明細番号"] > 0)
                {
                    // 売上照会
                    ShowUriage();
                }
                else
                {
                    // 入金照会
                    ShowNyukin();
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
        /// 詳細画面ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount == 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.ActiveRowIndex == fpSpread1_Sheet1.Rows.Count - 1)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail();       // 詳細表示（既存レコード）
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
        /// 一覧印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListPrint_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                PrintTPList();          // TP決済一覧印刷
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

                // 合計行？
                if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
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

                    if (fpSpread1_Sheet1.SelectionCount == 0)
                    {
                        MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    // 合計行？
                    if (fpSpread1_Sheet1.ActiveRowIndex == fpSpread1_Sheet1.Rows.Count - 1)
                    {
                        MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    ShowDetail();       // 詳細表示
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
        /// Spreadセル移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            // 最終行(=合計行)には移動しない
            if (e.NewRow >= fpSpread1_Sheet1.Rows.Count - 1)
            {
                e.Cancel = true;
            }
        }

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド

        /// <summary>
        /// 売上照会表示
        /// </summary>
        private void ShowUriage()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataRow dr = ((DataRowView)bindingSource1.Current).Row;

                // 売上照会
                UriageMeisaiCriteria uriageMeisaiCriteria = new UriageMeisaiCriteria();
                uriageMeisaiCriteria.stopOverwrite = true;
                uriageMeisaiCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                uriageMeisaiCriteria.得意先名 = dr["得意先名"] as string;
                uriageMeisaiCriteria.部署ＣＤ = dr["部署ＣＤ"] as int?;
                uriageMeisaiCriteria.売上日 = dr["決済日"] as DateTime?;

                Form401 form = new Form401(uriageMeisaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "売上照会画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form401_FormClosed);
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
        /// 売上照会画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form401_FormClosed(object sender, FormClosedEventArgs e)
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

        /// <summary>
        /// 入金照会表示
        /// </summary>
        private void ShowNyukin()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataRow dr = ((DataRowView)bindingSource1.Current).Row;

                // 入金照会
                NyukinTarget nyukinTarget = new NyukinTarget();
                nyukinTarget.得意先ＣＤ = dr["得意先ＣＤ"] as int?;

                // この入金明細レコードが表示できる表示期間を求める
                // 入金日が未来の場合はシステムマスタから取得した値をそのまま使用する
                DateTime today = DateTime.Today;
                DateTime nyukinbi = (DateTime)dr["決済日"];

                if (nyukinbi < today)
                {
                    // 入金日が当月（システム月と同じ）の場合、システム月－入金日の月＝０になるので＋１している
                    int elapsedMonths = ((today.Year - nyukinbi.Year) * 12) + (today.Month - nyukinbi.Month) + 1;
                    nyukinTarget.表示期間 = elapsedMonths;
                }

                Form601 form = new Form601(nyukinTarget);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "入金照会画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form601_FormClosed);
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
        /// 入金照会画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form601_FormClosed(object sender, FormClosedEventArgs e)
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

        /// <summary>
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                TPSettlementInfo tpSettlementInfo = new TPSettlementInfo();
                tpSettlementInfo.TP決済番号 = dr["TP決済番号"].ToString();
                tpSettlementInfo.決済日 = dr["決済日"] as DateTime?;

                //tpSettlementInfo.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                //tpSettlementInfo.部署ＣＤ = dr["部署ＣＤ"] as int?;
                //tpSettlementInfo.売上明細番号 = dr["売上明細番号"] as int?;

                Form512D form = new Form512D(tpSettlementInfo);

                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form512D_FormClosed);
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
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form512D_FormClosed(object sender, FormClosedEventArgs e)
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

        /// <summary>
        /// TP決済一覧印刷処理
        /// </summary>
        private void PrintTPList()
        {
            try
            {
                Report512A report = new Report512A();   // TP決済一覧
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["TP決済一覧"];

                report.DataSource = dt;

                // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                PrintReport(report, printSettings);

                History.Save(this.Text, "TP決済一覧", "", History.ProcessType.Output);

                log.Info("TP決済一覧を印刷しました。");
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }
        #endregion
    }
}

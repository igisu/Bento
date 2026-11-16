using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 請求一覧ダイアログ
    /// </summary>
    public partial class Form907 : BaseSpreadForm, IDialog
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEIKYU_NO_COLUMN      = 0;    // 請求番号列インデックス
        private const int SEIKYUBI_COLUMN       = 1;    // 請求日列インデックス
        private const int YOTEIBI_COLUMN        = 2;    // 回収予定日列インデックス
        private const int COURSE_CD_COLUMN      = 3;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 4;    // コース名列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 5;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 6;    // 得意先名列インデックス
        private const int ZENKAI_SEIKYU_COLUMN  = 7;    // 前回請求額列インデックス
        private const int NYUKIN_COLUMN         = 8;    // 入金額列インデックス
        private const int KURIKOSHI_COLUMN      = 9;    // 繰越額列インデックス
        private const int KAIAGE_COLUMN         = 10;   // 買上額列インデックス
        private const int ZEI_COLUMN            = 11;   // 消費税列インデックス
        private const int KONKAI_SEIKYU_COLUMN  = 12;   // 今回請求額列インデックス
        private const int SHIMEKUBUN_COLUMN     = 13;   // 締区分列インデックス
        private const int SEIKYU_DATE_COLUMN    = 14;   // 請求合計.修正日(処理日)列インデックス
        private const int URIAGE_DATE_COLUMN    = 15;   // 売上編集日時列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求一覧検索条件
        /// </summary>
        private SeikyuListCriteria _seikyuListCriteria;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public SeikyuGokei SelectedSeikyuGokei { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form907()
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
        private void Form907_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.magnifier_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除
                cboShiten.SelectedValueChanged -= cboShiten_SelectedValueChanged;   // 支店ＣＤコンボイベントをいったん削除

                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetShiharaiHoho(cboShiharaiHoho);
                //ComboUtil.SetShimeKubun(cboShimeKubun);
                ComboUtil.Set<int>(cboShimeKubun,
                    new string[] { "", Consts.ShimeKubun.Daily.OptionDisplay(), Consts.ShimeKubun.Weekly.OptionDisplay(), Consts.ShimeKubun.Monthly.OptionDisplay(), Consts.ShimeKubun.Tsuke.OptionDisplay() },
                    new int[] { -1, (int)Consts.ShimeKubun.Daily, (int)Consts.ShimeKubun.Weekly, (int)Consts.ShimeKubun.Monthly, (int)Consts.ShimeKubun.Tsuke });

                ComboUtil.Set(cboYoubi,
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
                _seikyuListCriteria = new SeikyuListCriteria();
                seikyuListBindingSource.DataSource = _seikyuListCriteria;

                // 振替用EB口座の選択肢を、部署にぶら下がるものに再設定する
                ComboUtil.SetBusyoEB(cboEBKoza, (int)cboBusyo.SelectedValue);

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求番号", "請求日", "請求先CD", "コースCD" },
                    new string[] { "A.請求番号 {0}, A.請求先ＣＤ",
                                   "A.請求日 {0}, A.請求先ＣＤ",
                                   "A.請求先ＣＤ {0}, A.請求番号",
                                   "C.コースＣＤ {0}, C.コース順"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // スプレッドのヘッダ編集日時列、売上編集日時列、保険区分CD列を非表示にする
                fpSpread1_Sheet1.Columns[SEIKYU_DATE_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[URIAGE_DATE_COLUMN].Visible = false;
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
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboShiten.SelectedValueChanged += new EventHandler(cboShiten_SelectedValueChanged);  // 支店ＣＤコンボイベントを追加
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);      // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form907_KeyDown(object sender, KeyEventArgs e)
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form907_Shown(object sender, EventArgs e)
        {
            // 得意先にフォーカス
            txtTokuisakiCD.Focus();
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
                _seikyuListCriteria.部署ＣＤ = -1;
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
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 振替用EB口座の選択肢を、部署にぶら下がるものに再設定する
                ComboUtil.SetBusyoEB(cboEBKoza, (int)cboBusyo.SelectedValue);
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
                    txtCourseCD.Text = course.コースＣＤ.ToString();
                    _seikyuListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;
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
                    _seikyuListCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;
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
        /// 支払方法コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShiharaiHoho_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 支払方法が振替の場合は、振替用EB口座を入力可能
                cboEBKoza.Enabled = ((int?)cboShiharaiHoho.SelectedValue == (int)Consts.ShiharaiHohoKubun.Furikae);
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        sql.AppendLine("SELECT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.請求番号");
                    sql.AppendLine("     ,A.請求日");
                    sql.AppendLine("     ,A.締区分");
                    sql.AppendLine("     ,CASE A.締区分 ");
                    // V_締め区分に「未収金」が無いので結合できない
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Daily,   Consts.ShimeKubun.Daily.DisplayName()).AppendLine();
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Weekly,  Consts.ShimeKubun.Weekly.DisplayName()).AppendLine();
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Monthly, Consts.ShimeKubun.Monthly.DisplayName()).AppendLine();
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Tsuke,   Consts.ShimeKubun.Tsuke.DisplayName()).AppendLine();
                    sql.AppendLine("     ELSE '' ");
                    sql.AppendLine("      END AS 締め区分名");
                    sql.AppendLine("     ,C.コースＣＤ");
                    sql.AppendLine("     ,C.コース名");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.請求先ＣＤ");
                    sql.AppendLine("     ,A.請求先名");
                    sql.AppendLine("     ,A.前回請求額");
                    sql.AppendLine("     ,A.入金額");
                    sql.AppendLine("     ,A.繰越額");
                    sql.AppendLine("     ,A.買上金額");
                    sql.AppendLine("     ,A.調整額");
                    sql.AppendLine("     ,A.消費税");
                    sql.AppendLine("     ,A.今回請求額");
                    sql.AppendLine("     ,A.回収予定日");
                    sql.AppendLine("     ,A.印刷者ＣＤ");
                    sql.AppendLine("     ,A.印刷日時");
                    sql.AppendLine("     ,A.EB作成済フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine("     ,A.売上編集日時");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         T.企業ＣＤ");
                    sql.AppendLine("         ,T.請求番号");
                    sql.AppendLine("         ,T.請求日");
                    sql.AppendLine("         ,T.締区分");
                    sql.AppendLine("         ,T.部署ＣＤ");
                    sql.AppendLine("         ,T.請求先ＣＤ");
                    sql.AppendLine("         ,T.請求先名");
                    sql.AppendLine("         ,T.前回請求額");
                    sql.AppendLine("         ,T.入金額");
                    sql.AppendLine("         ,T.繰越額");
                    sql.AppendLine("         ,T.買上金額");
                    sql.AppendLine("         ,T.調整額");
                    sql.AppendLine("         ,T.消費税");
                    sql.AppendLine("         ,T.今回請求額");
                    sql.AppendLine("         ,T.回収予定日");
                    sql.AppendLine("         ,T.印刷者ＣＤ");
                    sql.AppendLine("         ,T.印刷日時");
                    sql.AppendLine("         ,T.EB作成済フラグ");
                    sql.AppendLine("         ,T.作成担当者ＣＤ");
                    sql.AppendLine("         ,T.作成日");
                    sql.AppendLine("         ,T.修正担当者ＣＤ");
                    sql.AppendLine("         ,T.修正日");
                    sql.AppendLine("         ,MAX(U.修正日) AS 売上編集日時");
                    sql.AppendLine("     FROM");
                    // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             T1.企業ＣＤ");
                    sql.AppendLine("             ,T1.請求番号 AS 今回請求番号");
                    sql.AppendLine("             ,T1.請求先ＣＤ");
                    sql.AppendLine("             ,MAX(P1.請求日) AS 前回請求日");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             請求合計 AS T1");
                    sql.AppendLine("             LEFT JOIN 請求合計 AS P1");
                    sql.AppendLine("                 ON T1.企業ＣＤ = P1.企業ＣＤ");
                    sql.AppendLine("                 AND T1.請求先ＣＤ = P1.請求先ＣＤ");
                    sql.AppendLine("                 AND P1.請求日 < T1.請求日");
                    sql.AppendLine("             LEFT JOIN 部署マスタ   AS B ON B.企業ＣＤ = T1.企業ＣＤ AND B.部署ＣＤ = T1.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = T1.企業ＣＤ AND T.得意先ＣＤ = T1.請求先ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             T1.企業ＣＤ = @企業ＣＤ");
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_seikyuListCriteria.支店ＣＤ != null && _seikyuListCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine("             AND B.支店ＣＤ = @支店ＣＤ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.支店ＣＤ;
                    }

                    if (_seikyuListCriteria.部署ＣＤ != null && _seikyuListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("             AND T1.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.部署ＣＤ;
                    }

                    if (_seikyuListCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine("             AND T1.請求先ＣＤ = @請求先ＣＤ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.得意先ＣＤ;
                    }

                    if (_seikyuListCriteria.請求日From != null)
                    {
                        sql.AppendLine("             AND T1.請求日 >= @請求日From");
                        cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _seikyuListCriteria.請求日From;
                    }

                    if (_seikyuListCriteria.請求日To != null)
                    {
                        sql.AppendLine("             AND T1.請求日 <= @請求日To");
                        cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _seikyuListCriteria.請求日To;
                    }

                    if (_seikyuListCriteria.回収予定日From != null)
                    {
                        sql.AppendLine("             AND T1.回収予定日 >= @回収予定日From");
                        cmd.Parameters.Add("@回収予定日From", SqlDbType.Date).Value = _seikyuListCriteria.回収予定日From;
                    }

                    if (_seikyuListCriteria.回収予定日To != null)
                    {
                        sql.AppendLine("             AND T1.回収予定日 <= @回収予定日To");
                        cmd.Parameters.Add("@回収予定日To", SqlDbType.Date).Value = _seikyuListCriteria.回収予定日To;
                    }

                    if (_seikyuListCriteria.処理日 != null)
                    {
                        sql.AppendLine("             AND CONVERT(date, T1.修正日) = @処理日");
                        cmd.Parameters.Add("@処理日", SqlDbType.Date).Value = _seikyuListCriteria.処理日;
                    }

                    if (_seikyuListCriteria.今回請求額 != null)
                    {
                        sql.AppendLine("             AND T1.今回請求額 >= @今回請求額");
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = _seikyuListCriteria.今回請求額;
                    }

                    if (_seikyuListCriteria.支払方法 != null && _seikyuListCriteria.支払方法 != -1)
                    {
                        sql.AppendLine("             AND T.支払方法 = @支払方法");
                        cmd.Parameters.Add("@支払方法", SqlDbType.Int).Value = _seikyuListCriteria.支払方法;

                        // 支払方法が振替であれば、EB銀行支店ＣＤを条件に追加する
                        if (_seikyuListCriteria.支払方法 == (int)Consts.ShiharaiHohoKubun.Furikae)
                        {
                            if (string.IsNullOrEmpty(_seikyuListCriteria.EB銀行支店ＣＤ) == false)
                            {
                                sql.AppendLine("             AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ");
                                cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = _seikyuListCriteria.EB銀行支店ＣＤ;
                            }
                        }
                    }

                    if (_seikyuListCriteria.締区分 != null && _seikyuListCriteria.締区分 != -1)
                    {
                        sql.AppendLine("             AND T1.締区分 = @締区分");
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = _seikyuListCriteria.締区分;
                    }

                    if (_seikyuListCriteria.週締曜日 != null && _seikyuListCriteria.週締曜日 != -1)
                    {
                        sql.AppendLine("             AND T.週締曜日 = @週締曜日");
                        cmd.Parameters.Add("@週締曜日", SqlDbType.Int).Value = _seikyuListCriteria.週締曜日;
                    }

                    if (_seikyuListCriteria.月締日 != null)
                    {
                        sql.AppendLine("             AND T.月締日 = @月締日");
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _seikyuListCriteria.月締日;
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             T1.企業ＣＤ");
                    sql.AppendLine("             ,T1.請求番号");
                    sql.AppendLine("             ,T1.請求先ＣＤ");
                    sql.AppendLine("         ) AS P");
                    // -- ②今回請求の詳細
                    sql.AppendLine("         INNER JOIN 請求合計 T ON");
                    sql.AppendLine("             T.企業ＣＤ = P.企業ＣＤ");
                    sql.AppendLine("             AND T.請求番号 = P.今回請求番号");
                    // -- ③今回の請求ヘッダ
                    sql.AppendLine("         INNER JOIN 請求ヘッダ H ON");
                    sql.AppendLine("             H.企業ＣＤ = T.企業ＣＤ");
                    sql.AppendLine("             AND H.請求番号 = T.請求番号");
                    // -- ④今回の請求に対する最新の売上
                    sql.AppendLine("         LEFT JOIN 売上明細 AS U ON");
                    sql.AppendLine("             U.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             AND U.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("             AND U.部署ＣＤ = H.部署ＣＤ");
                    sql.AppendLine("             AND U.売上日 > ISNULL(P.前回請求日, DATEADD(month, -3, getdate()))");  // -- 前回請求が無い場合は、3ヶ月前までを探す
                    sql.AppendLine("             AND U.売上日 <= T.請求日");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         T.企業ＣＤ");
                    sql.AppendLine("         ,T.請求番号");
                    sql.AppendLine("         ,T.請求日");
                    sql.AppendLine("         ,T.締区分");
                    sql.AppendLine("         ,T.部署ＣＤ");
                    sql.AppendLine("         ,T.請求先ＣＤ");
                    sql.AppendLine("         ,T.請求先名");
                    sql.AppendLine("         ,T.前回請求額");
                    sql.AppendLine("         ,T.入金額");
                    sql.AppendLine("         ,T.繰越額");
                    sql.AppendLine("         ,T.買上金額");
                    sql.AppendLine("         ,T.調整額");
                    sql.AppendLine("         ,T.消費税");
                    sql.AppendLine("         ,T.今回請求額");
                    sql.AppendLine("         ,T.回収予定日");
                    sql.AppendLine("         ,T.印刷者ＣＤ");
                    sql.AppendLine("         ,T.印刷日時");
                    sql.AppendLine("         ,T.EB作成済フラグ");
                    sql.AppendLine("         ,T.作成担当者ＣＤ");
                    sql.AppendLine("         ,T.作成日");
                    sql.AppendLine("         ,T.修正担当者ＣＤ");
                    sql.AppendLine("         ,T.修正日");
                    sql.AppendLine("     ) AS A");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                    if (_seikyuListCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ,");
                        sql.AppendLine("             CM.部署ＣＤ,");
                        sql.AppendLine("             CM.コース名,");
                        sql.AppendLine("             CT.コースＣＤ,");
                        sql.AppendLine("             CT.得意先ＣＤ,");
                        sql.AppendLine("             CT.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コースマスタ AS CM");
                        sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                        sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _seikyuListCriteria.コースＣＤ;
                    }
                    else
                    {
                        // -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
                        sql.AppendLine("     LEFT JOIN");
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT1.コースＣＤ");
                        sql.AppendLine("             ,CT1.得意先ＣＤ");
                        sql.AppendLine("             ,CT1.コース順");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コーステーブル AS CT1");
                        sql.AppendLine("             INNER JOIN");
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                     ,MIN(T.コースＣＤ) AS コースＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     コースマスタ AS M");
                        sql.AppendLine("                     LEFT JOIN コーステーブル AS T ON");
                        sql.AppendLine("                         T.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND T.コースＣＤ = M.コースＣＤ");
                        sql.AppendLine("                 GROUP BY");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,T.得意先ＣＤ");
                        sql.AppendLine("                 ) AS CT2");
                        sql.AppendLine("                     ON CT2.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                     AND CT2.得意先ＣＤ = CT1.得意先ＣＤ");
                        sql.AppendLine("                     AND CT2.コースＣＤ = CT1.コースＣＤ");
                        sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                        sql.AppendLine("                 ON CM.企業ＣＤ = CT1.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT1.コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("             ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("             AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("             AND J.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("請求一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "請求一覧";

                    log.Info("請求一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN,      "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN,       "請求日");
                fpSpread1_Sheet1.BindDataColumn(YOTEIBI_COLUMN,        "回収予定日");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(ZENKAI_SEIKYU_COLUMN,  "前回請求額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,         "入金額");
                fpSpread1_Sheet1.BindDataColumn(KURIKOSHI_COLUMN,      "繰越額");
                fpSpread1_Sheet1.BindDataColumn(KAIAGE_COLUMN,         "買上金額");
                fpSpread1_Sheet1.BindDataColumn(ZEI_COLUMN,            "消費税");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN,  "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(SHIMEKUBUN_COLUMN,     "締め区分名");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_DATE_COLUMN,    "修正日");
                fpSpread1_Sheet1.BindDataColumn(URIAGE_DATE_COLUMN,    "売上編集日時");

                // 修正日≦売上編集日時 の場合は行の文字色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    DateTime? updateDateTimeHeader = fpSpread1_Sheet1.Cells[rowIndex, SEIKYU_DATE_COLUMN].Value as DateTime?;
                    DateTime? updateDateTimeTotal = fpSpread1_Sheet1.Cells[rowIndex, URIAGE_DATE_COLUMN].Value as DateTime?;
                    if (updateDateTimeHeader != null && updateDateTimeTotal != null && updateDateTimeHeader <= updateDateTimeTotal)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = Color.Red;
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
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
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
                _seikyuListCriteria = new SeikyuListCriteria();
                seikyuListBindingSource.DataSource = _seikyuListCriteria;

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);
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
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                SelectedSeikyuGokei = new SeikyuGokei(dr);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
                throw new BentoException(ex);
            }
        }
        #endregion
    }
}

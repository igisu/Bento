using System;
using System.Collections.Generic;
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
    /// FRクレカ決済一覧
    /// </summary>
    public partial class Form513 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int SETTLEMENT_STATUS_COLUMN = 0;    // 決済状態列インデックス
        private const int SEIKYU_NO_COLUMN = 1;    // 請求番号列インデックス
        private const int SEIKYUBI_COLUMN = 2;    // 請求日列インデックス
        private const int COURSE_CD_COLUMN = 3;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN = 4;    // コース名列インデックス
        private const int SEIKYUSAKI_CD_COLUMN = 5;    // 請求先ＣＤ列インデックス
        private const int SEIKYUSAKI_NAME_COLUMN = 6;    // 請求先名列インデックス
        private const int KONKAI_SEIKYU_COLUMN = 7;    // 今回請求額列インデックス

        // 非表示列
        private const int SETTLED_FLG_COLUMN = 8;    // クレカ決済完了フラグ列インデックス
        private const int ERROR_TYPE_COLUMN = 9;    // クレカ決済エラー区分列インデックス
        private const int CARD_DISABLE_FLG_COLUMN = 10;    // クレカ無効フラグ列インデックス

        // 決済金額下限
        private const decimal SETTLE_MIN_AMOUNT = 100m;

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// FRクレカ決済一覧検索条件
        /// </summary>
        private FRSettlementListCriteria _frSettlementListCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ

        public Form513(FRSettlementListCriteria fRSettlementListCriteria)
        {
            InitializeComponent();
            _frSettlementListCriteria = fRSettlementListCriteria;
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
        private void Form513_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                //btnExec.Enabled = this.HasAuthEdit;
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                cboShiten.SelectedValueChanged -= cboShiten_SelectedValueChanged;   // 支店ＣＤコンボイベントをいったん削除
                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.Set<int>(cboSettlementStatus,
                    new string[] { "", "未決済", "決済完了" },
                    new int[] { -1, 0, 1 });

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求番号", "請求日", "請求先CD", "コースCD" },
                    new string[] { "A.請求番号 {0}, A.請求先ＣＤ",
                                   "A.請求日 {0}, A.請求先ＣＤ",
                                   "A.請求先ＣＤ {0}, C.コースＣＤ, C.コース順",
                                   "C.コースＣＤ {0}, C.コース順, A.請求先ＣＤ"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // デフォルトは無制限
                cboListMax.SelectedValue = -1;

                if (_frSettlementListCriteria == null)
                {
                    // 遷移元からの条件指定なし
                    _frSettlementListCriteria = new FRSettlementListCriteria();
                }
                else
                {
                    // 遷移元から条件指定がある場合はそのまま検索へ
                    btnQuery.PerformClick();
                }

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                frSettlementListBindingSource.DataSource = _frSettlementListCriteria;

                // 列を非表示にする
                fpSpread1_Sheet1.Columns[SETTLED_FLG_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[ERROR_TYPE_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[CARD_DISABLE_FLG_COLUMN].Visible = false;
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
                cboBusyo.SelectedValueChanged += new EventHandler(cboBusyo_SelectedValueChanged);    // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form513_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

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
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form513_Shown(object sender, EventArgs e)
        {
            // 請求先にフォーカス
            txtSeikyusakiCDFrom.Focus();
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
                _frSettlementListCriteria.部署ＣＤ = -1;
                _frSettlementListCriteria.部署名L = cboBusyo.Text;

                // 支店コンボの選択肢を、支店名に反映する
                _frSettlementListCriteria.支店名L = cboShiten.Text;
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
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 部署コンボの選択肢を、部署名に反映する
                _frSettlementListCriteria.部署名L = cboBusyo.Text;
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
                    _frSettlementListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
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
        /// 請求先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSeikyusakiCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSeikyusakiCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// 請求先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyusakiCDFrom_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDFrom_FormClosed);
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
        /// 得意先選択ダイアログクローズ（請求先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDFrom_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSeikyusakiCDFrom, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtSeikyusakiCDFrom.Text = tokuisaki.得意先ＣＤ.ToString();
                    _frSettlementListCriteria.請求先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblSeikyusakiNameFrom.Text = tokuisaki.得意先名;
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
        /// 請求先ＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSeikyusakiCDTo_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.F1)
            {
                btnSeikyusakiCDTo.PerformClick();
            }
        }

        /// <summary>
        /// 請求先ＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyusakiCDTo_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDTo_FormClosed);
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
        /// 得意先選択ダイアログクローズ（請求先ＣＤTo）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDTo_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSeikyusakiCDTo, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtSeikyusakiCDTo.Text = tokuisaki.得意先ＣＤ.ToString();
                    _frSettlementListCriteria.請求先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblSeikyusakiNameTo.Text = tokuisaki.得意先名;
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
                if (_frSettlementListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_frSettlementListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _frSettlementListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.請求番号");
                    sql.AppendLine("     ,A.請求日");
                    sql.AppendLine("     ,A.締区分");
                    sql.AppendLine("     ,CASE A.締区分 ");
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Daily, Consts.ShimeKubun.Daily.DisplayName()).AppendLine();
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Weekly, Consts.ShimeKubun.Weekly.DisplayName()).AppendLine();
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Monthly, Consts.ShimeKubun.Monthly.DisplayName()).AppendLine();
                    sql.AppendFormat("     WHEN {0} THEN '{1}' ", (int)Consts.ShimeKubun.Tsuke, Consts.ShimeKubun.Tsuke.DisplayName()).AppendLine();
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
                    sql.AppendLine("     ,A.FR支払フラグ");
                    sql.AppendLine("     ,A.クレカ決済完了フラグ");
                    sql.AppendLine("     ,A.クレカ決済エラー区分");
                    sql.AppendLine("     ,CASE ");
                    sql.AppendLine("       WHEN A.クレカ決済エラー区分 IS NOT NULL AND A.クレカ決済エラー区分 <> 0 THEN '???'");
                    sql.AppendLine("       WHEN A.クレカ決済完了フラグ = 0 THEN '未決済'");
                    sql.AppendLine("       WHEN A.クレカ決済完了フラグ = 1 THEN '決済完了'");
                    sql.AppendLine("       ELSE ''");
                    sql.AppendLine("      END AS 決済状態");
                    sql.AppendLine("     ,S.クレカ無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     請求合計 AS A");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");

                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                    if (_frSettlementListCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CM.コース名");
                        sql.AppendLine("             ,CT.コースＣＤ");
                        sql.AppendLine("             ,CT.得意先ＣＤ");
                        sql.AppendLine("             ,CT.コース順");
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
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _frSettlementListCriteria.コースＣＤ;
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

                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND A.FR支払フラグ = 1");   // FR支払のみ

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_frSettlementListCriteria.支店ＣＤ != null && _frSettlementListCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine("     AND B.支店ＣＤ = @支店ＣＤ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _frSettlementListCriteria.支店ＣＤ;
                    }

                    if (_frSettlementListCriteria.部署ＣＤ != null && _frSettlementListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("     AND A.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _frSettlementListCriteria.部署ＣＤ;
                    }

                    if (_frSettlementListCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("     AND A.請求先ＣＤ >= @請求先ＣＤFrom");
                        cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = _frSettlementListCriteria.請求先ＣＤFrom;
                    }

                    if (_frSettlementListCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("     AND A.請求先ＣＤ <= @請求先ＣＤTo");
                        cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = _frSettlementListCriteria.請求先ＣＤTo;
                    }

                    if (_frSettlementListCriteria.請求日From != null)
                    {
                        sql.AppendLine("     AND A.請求日 >= @請求日From");
                        cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _frSettlementListCriteria.請求日From;
                    }

                    if (_frSettlementListCriteria.請求日To != null)
                    {
                        sql.AppendLine("     AND A.請求日 <= @請求日To");
                        cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _frSettlementListCriteria.請求日To;
                    }

                    if (_frSettlementListCriteria.決済状態 != null && _frSettlementListCriteria.決済状態 != -1)
                    {
                        sql.AppendLine("     AND A.クレカ決済完了フラグ = @クレカ決済完了フラグ");
                        cmd.Parameters.Add("@クレカ決済完了フラグ", SqlDbType.Bit).Value = _frSettlementListCriteria.決済状態;
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    //cmd.CommandTimeout = 60;

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("FRクレカ決済一覧");
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
                    bindingSource1.DataMember = "FRクレカ決済一覧";

                    log.Info("FRクレカ決済一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SETTLEMENT_STATUS_COLUMN, "決済状態");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN, "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN, "請求日");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "コース名");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_CD_COLUMN, "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN, "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(SETTLED_FLG_COLUMN, "クレカ決済完了フラグ");
                fpSpread1_Sheet1.BindDataColumn(ERROR_TYPE_COLUMN, "クレカ決済エラー区分");
                fpSpread1_Sheet1.BindDataColumn(CARD_DISABLE_FLG_COLUMN, "クレカ無効フラグ");

                // 決済状態で文字色を変更する
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();

                    bool? settledFlg = fpSpread1_Sheet1.Cells[rowIndex, SETTLED_FLG_COLUMN].Value as bool?;
                    int? errorType = fpSpread1_Sheet1.Cells[rowIndex, ERROR_TYPE_COLUMN].Value as int?;
                    bool disableFlg = fpSpread1_Sheet1.Cells[rowIndex, CARD_DISABLE_FLG_COLUMN].Value as bool? ?? false;

                    if ((errorType != null && errorType != (int)Consts.FRErrorType.None) || (settledFlg != true && disableFlg == true))
                    {
                        // 不整合orクレカ無効の未決済 の場合は行の背景色と文字色を変更
                        //fpSpread1_Sheet1.Rows[rowIndex].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = Color.MistyRose;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = System.Drawing.Color.Red;
                    }
                    else if (settledFlg == true)
                    {
                        // 決済完了の場合は文字色を青
                        fpSpread1_Sheet1.Cells[rowIndex, SETTLEMENT_STATUS_COLUMN].ForeColor = Color.Blue;
                    }
                    else if (settledFlg == false)
                    {
                        // 未決済の場合は文字色を赤
                        fpSpread1_Sheet1.Cells[rowIndex, SETTLEMENT_STATUS_COLUMN].ForeColor = Color.Red;
                    }

                    // nullの場合はそのまま
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, SEIKYUSAKI_NAME_COLUMN].Text = "合計";
                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, KONKAI_SEIKYU_COLUMN];
                    FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, KONKAI_SEIKYU_COLUMN];
                    FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, KONKAI_SEIKYU_COLUMN];
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

                LockCriteria(true);
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
                _frSettlementListCriteria = new FRSettlementListCriteria();
                frSettlementListBindingSource.DataSource = _frSettlementListCriteria;

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);

                LockCriteria(false);
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
        /// 詳細ボタン
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
        /// 決済処理ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    MessageBox.Show(this, "対象のレコードがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;
                List<SeikyuGokei> seikyuGokeis = new List<SeikyuGokei>();

                for (int i = 0; i < fpSpread1_Sheet1.Rows.Count - 1; i++)
                {
                    DataRow dr = ((DataRowView)bindingSource1[i]).Row;
                    SeikyuGokei seikyuGokei = new SeikyuGokei(dr);

                    // 合計行を除外
                    if (i == fpSpread1_Sheet1.Rows.Count - 1)
                    {
                        continue;
                    }
                    // 決済完了とエラーを除外
                    if (seikyuGokei.クレカ決済完了フラグ == true || (seikyuGokei.クレカ決済エラー区分 != null && seikyuGokei.クレカ決済エラー区分 != 0))
                    {
                        continue;
                    }
                    // 100円未満を除外
                    if ((seikyuGokei.今回請求額 ?? 0m) < SETTLE_MIN_AMOUNT)
                    {
                        continue;
                    }
                    // クレカ無効を除外
                    if ((dr["クレカ無効フラグ"] as bool? ?? false) == true)
                    {
                        continue;
                    }

                    // 決済対象
                    seikyuGokeis.Add(seikyuGokei);
                }

                Form513B form = new Form513B(_frSettlementListCriteria, seikyuGokeis);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "決済画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form513_FormClosed);
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
        /// 詳細表示
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                SeikyuGokei seikyuGokei = new SeikyuGokei(dr);
                Form513D form = new Form513D(seikyuGokei);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form513_FormClosed);
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
        private void Form513_FormClosed(object sender, FormClosedEventArgs e)
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

                btnQuery.Enabled = true;
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
        /// 検索条件のロック／アンロック
        /// </summary>
        /// <param name="isLocked"></param>
        private void LockCriteria(bool isLocked)
        {
            foreach (Control ctrl in groupBox1.Controls)
            {
                if ((ctrl is Label) == false)
                {
                    ctrl.Enabled = !isLocked;
                }
            }

            int count = 0;

            if (bindingSource1.DataSource != null)
            {
                // 決済可能なデータがある場合のみボタンを活性する
                DataTable dt = ((DataSet)bindingSource1.DataSource).Tables[0];
                count = dt.Select(string.Format("クレカ無効フラグ <> 1 AND クレカ決済完了フラグ = 0 AND (クレカ決済エラー区分 IS NULL OR クレカ決済エラー区分 = 0) AND 今回請求額 >= {0}", SETTLE_MIN_AMOUNT)).Length;
            }

            if (this.HasAuthEdit && count > 0)
            {
                btnExec.Enabled = true;
            }
            else
            {
                btnExec.Enabled = false;
            }
        }

        #endregion
    }
}

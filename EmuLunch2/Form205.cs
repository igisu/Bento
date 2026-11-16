using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 固定注文反映
    /// </summary>
    public partial class Form205 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 固定注文反映条件
        /// </summary>
        private KoteiChumonHanneiCriteria _koteiChumonHanneiCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form205()
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
        private void Form205_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                // 権限によりボタンを制御
                btnExec.Enabled = this.HasAuthEdit;

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _koteiChumonHanneiCriteria = new KoteiChumonHanneiCriteria();
                koteiChumonHanneiBindingSource.DataSource = _koteiChumonHanneiCriteria;
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
        private void Form205_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F9:                       // 処理開始ボタン押下
                        btnExec.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
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
                    _koteiChumonHanneiCriteria.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtCourseCDFrom);
                    ReadBinding(lblCourseNameFrom);
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
        /// コースＣＤToキー押下（ファンクションキー）
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
        /// コースＣＤTo参照ボタン
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
                    _koteiChumonHanneiCriteria.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtCourseCDTo);
                    ReadBinding(lblCourseNameTo);
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
        /// 得意先ＣＤFromキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCDFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCDFrom.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤFrom参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCDFrom_Click(object sender, EventArgs e)
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤFrom）
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
                    errorProvider1.SetError(txtTokuisakiCDFrom, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    _koteiChumonHanneiCriteria.得意先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCDFrom);
                    ReadBinding(lblTokuisakiNameFrom);
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
        /// 得意先ＣＤToキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCDTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCDTo.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤTo参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCDTo_Click(object sender, EventArgs e)
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤTo）
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
                    errorProvider1.SetError(txtTokuisakiCDTo, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    _koteiChumonHanneiCriteria.得意先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCDTo);
                    ReadBinding(lblTokuisakiNameTo);
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
        /// 処理開始ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "実行できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_koteiChumonHanneiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_koteiChumonHanneiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _koteiChumonHanneiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 適用日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_koteiChumonHanneiCriteria.適用日From > warnDate || _koteiChumonHanneiCriteria.適用日To > warnDate)
                {
                    if (MessageBox.Show(this, "適用日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 確認メッセージを表示
                if (MessageBox.Show(this, "固定注文反映処理を行います。よろしいですか？",
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (Exec())
                    {
                        MessageBox.Show(this, "固定注文反映処理を実行しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 固定注文反映処理
        /// </summary>
        /// <returns>成功した場合は True</returns>
        private bool Exec()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                // 処理対象の有無チェック
                if (HasTarget() == false)
                {
                    MessageBox.Show(this, "対象となる得意先、または、得意先単価適用マスタが存在しません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
                    return false;
                }

                // 固定注文作成
                bool ret = KoteiChumonHannei();

                return ret;
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

        /// <summary>
        /// 処理対象の有無チェック
        /// </summary>
        /// <returns>処理対象が有る場合は True</returns>
        private bool HasTarget()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     COUNT(*) AS CNT");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     得意先マスタ AS T");
                    sql.AppendLine("     INNER JOIN 得意先単価適用マスタ AS TA ON");
                    sql.AppendLine("         TA.企業ＣＤ = T.企業ＣＤ");
                    sql.AppendLine("         AND TA.得意先ＣＤ = T.得意先ＣＤ");

                    if (_koteiChumonHanneiCriteria.コースＣＤFrom != null || _koteiChumonHanneiCriteria.コースＣＤTo != null)
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
                        sql.AppendLine("             1 = 1");
                        if (_koteiChumonHanneiCriteria.コースＣＤFrom != null)
                        {
                            sql.AppendLine("             AND CT.コースＣＤ >= @コースＣＤFrom");
                            cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.コースＣＤFrom;
                        }
                        if (_koteiChumonHanneiCriteria.コースＣＤTo != null)
                        {
                            sql.AppendLine("             AND CT.コースＣＤ <= @コースＣＤTo");
                            cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.コースＣＤTo;
                        }
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON T.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND T.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     T.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND T.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     AND ISNULL(T.無効フラグ, 0) = 0");
                    sql.AppendLine("     AND (T.受注得意先CD IS NULL OR (T.受注得意先ＣＤ = T.得意先ＣＤ))");
                    sql.AppendLine("     AND ISNULL(TA.無効フラグ, 0) = 0");

                    if (_koteiChumonHanneiCriteria.得意先ＣＤFrom != null)
                    {
                        sql.AppendLine("     AND TA.得意先ＣＤ >= @得意先ＣＤFrom");
                        cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.得意先ＣＤFrom;
                    }

                    if (_koteiChumonHanneiCriteria.得意先ＣＤTo != null)
                    {
                        sql.AppendLine("     AND TA.得意先ＣＤ <= @得意先ＣＤTo");
                        cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.得意先ＣＤTo;
                    }
                    sql.AppendLine("     AND ISNULL(TA.固定数, 0) <> 0");
                    sql.AppendLine("     AND ISNULL(TA.商品単価税抜, -1) <> -1");
                    sql.AppendLine("     AND ISNULL(TA.商品単価税込, -1) <> -1");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.部署ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    int cnt = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        cnt = (int)reader["CNT"];
                    }

                    // レコードの有無を返す
                    return (cnt > 0);
                }
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
        }

        /// <summary>
        /// 固定注文反映処理
        /// </summary>
        /// <returns>成功した場合は True</returns>
        private bool KoteiChumonHannei()
        {
            bool ret = false;
            History.ProcessType historyProcess = History.ProcessType.Update;
            Busyo busyo = Busyo.GetBusyo((int)_koteiChumonHanneiCriteria.部署ＣＤ);     // 部署マスタを取得（製造日／配送日算出日数の取得用）
            UserData userData = UserData.getInstance();
            Dictionary<int, Tokuisaki> tokuisakiMap = new Dictionary<int, Tokuisaki>();

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        History.Save(con, tran, this.Text, "固定注文反映", "", History.ProcessType.Start);
                        log.Info("固定注文反映処理を開始しました。");

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT DISTINCT");         // -- コースによる絞り込みの際、同じ得意先に複数のコースが設定されていると複数のレコードを取得してしまうのでDISTINCTでそれを回避する
                        sql.AppendLine("     T.得意先ＣＤ");
                        sql.AppendLine("     ,TA.商品ＣＤ");
                        sql.AppendLine("     ,TA.適用開始日");
                        sql.AppendLine("     ,TA.固定数");
                        sql.AppendLine("     ,TA.商品単価税抜");
                        sql.AppendLine("     ,TA.商品単価税込");
                        sql.AppendLine("     ,S.軽減税率フラグ");
                        sql.AppendLine("     ,ISNULL(C.明細番号, 0) AS 明細番号");
                        sql.AppendLine("     ,CASE WHEN SK.対象日 IS NOT NULL THEN ISNULL(T.祝日配送区分, 0) ELSE 1 END AS 祝日配送区分");
                        sql.AppendLine(" FROM");
                        sql.AppendLine("     得意先マスタ AS T");
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             B1.企業ＣＤ");
                        sql.AppendLine("             ,B1.得意先ＣＤ");
                        sql.AppendLine("             ,B1.商品ＣＤ");
                        sql.AppendLine("             ,B1.適用開始日");
                        sql.AppendLine("             ,ISNULL(B1.固定数, 0) AS 固定数");
                        sql.AppendLine("             ,B1.商品単価税抜");
                        sql.AppendLine("             ,B1.商品単価税込");
                        sql.AppendLine("          FROM");
                        sql.AppendLine("             得意先単価適用マスタ AS B1");
                        sql.AppendLine("             INNER JOIN");
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     企業ＣＤ");
                        sql.AppendLine("                     ,得意先ＣＤ");
                        sql.AppendLine("                     ,商品ＣＤ");
                        sql.AppendLine("                     ,MAX(適用開始日) AS 適用開始日");
                        sql.AppendLine("                  FROM");
                        sql.AppendLine("                     得意先単価適用マスタ");
                        sql.AppendLine("                  WHERE");
                        sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");

                        if (_koteiChumonHanneiCriteria.得意先ＣＤFrom != null)
                        {
                            sql.AppendLine("                     AND 得意先ＣＤ >= @得意先ＣＤFrom");
                        }

                        if (_koteiChumonHanneiCriteria.得意先ＣＤTo != null)
                        {
                            sql.AppendLine("                     AND 得意先ＣＤ <= @得意先ＣＤTo");
                        }

                        sql.AppendLine("                     AND 適用開始日 <= @適用日");
                        sql.AppendLine("                     AND ISNULL(商品単価税抜, -1) <> -1");
                        sql.AppendLine("                     AND ISNULL(商品単価税込, -1) <> -1");
                        sql.AppendLine("                  GROUP BY");
                        sql.AppendLine("                     企業ＣＤ");
                        sql.AppendLine("                     ,得意先ＣＤ");
                        sql.AppendLine("                     ,商品ＣＤ");
                        sql.AppendLine("                 ) B2 ON");
                        sql.AppendLine("                 B1.企業ＣＤ = B2.企業ＣＤ");
                        sql.AppendLine("                 AND B1.得意先ＣＤ = B2.得意先ＣＤ");
                        sql.AppendLine("                 AND B1.商品ＣＤ   = B2.商品ＣＤ");
                        sql.AppendLine("                 AND B1.適用開始日 = B2.適用開始日");
                        sql.AppendLine("         ) AS TA ON");
                        sql.AppendLine("         TA.企業ＣＤ = T.企業ＣＤ");
                        sql.AppendLine("         AND TA.得意先ＣＤ = T.得意先ＣＤ");

                        if (_koteiChumonHanneiCriteria.コースＣＤFrom != null || _koteiChumonHanneiCriteria.コースＣＤTo != null)
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
                            sql.AppendLine("             1 = 1");
                            if (_koteiChumonHanneiCriteria.コースＣＤFrom != null)
                            {
                                sql.AppendLine("             AND CT.コースＣＤ >= @コースＣＤFrom");
                            }
                            if (_koteiChumonHanneiCriteria.コースＣＤTo != null)
                            {
                                sql.AppendLine("             AND CT.コースＣＤ <= @コースＣＤTo");
                            }
                            sql.AppendLine("         ) AS CS");
                            sql.AppendLine("         ON T.企業ＣＤ = CS.企業ＣＤ");
                            sql.AppendLine("         AND T.得意先ＣＤ = CS.得意先ＣＤ");
                        }

                        sql.AppendLine("     INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = TA.企業ＣＤ AND S.商品ＣＤ = TA.商品ＣＤ");
                        sql.AppendLine("     LEFT JOIN (");
                        sql.AppendLine("         SELECT");
                        sql.AppendLine("             得意先ＣＤ");
                        sql.AppendLine("             ,MAX(明細番号) AS 明細番号");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             注文明細");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("             AND 注文区分 = @注文区分注文");
                        sql.AppendLine("             AND 喫食日 = @適用日");
                        sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("         GROUP BY");
                        sql.AppendLine("             得意先ＣＤ");
                        sql.AppendLine("         ) AS C ON C.得意先ＣＤ = TA.得意先ＣＤ");
                        sql.AppendLine("     LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = T.企業ＣＤ AND SK.対象日 = @適用日");
                        sql.AppendLine(" WHERE");
                        sql.AppendLine("     T.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("     AND T.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("     AND ISNULL(T.無効フラグ, 0) = 0");
                        sql.AppendLine("     AND (T.受注得意先CD IS NULL OR (T.受注得意先ＣＤ = T.得意先ＣＤ))");
                        sql.AppendLine("     AND ISNULL(S.無効フラグ, 0) = 0");
                        sql.AppendLine("     AND TA.固定数 <> 0");
                        sql.AppendLine(" ORDER BY");
                        sql.AppendLine("     T.得意先ＣＤ");
                        sql.AppendLine("     ,TA.商品ＣＤ");

                        //*****************************
                        // 適用開始日～終了日までループ
                        //*****************************
                        DateTime dateFrom = (DateTime)_koteiChumonHanneiCriteria.適用日From;
                        DateTime dateTo = (DateTime)_koteiChumonHanneiCriteria.適用日To;
                        for (; dateFrom <= dateTo; dateFrom = dateFrom.AddDays(1))
                        {
                            // 処理対象の得意先単価適用マスタ取得(A)
                            SqlCommand cmd = con.CreateCommand();
                            cmd.Transaction = tran;

                            cmd.CommandText = sql.ToString();
                            cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                            cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                            cmd.Parameters.Add("@適用日", SqlDbType.Date).Value = dateFrom;
                            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.部署ＣＤ;

                            if (_koteiChumonHanneiCriteria.得意先ＣＤFrom != null)
                            {
                                cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.得意先ＣＤFrom;
                            }
                            if (_koteiChumonHanneiCriteria.得意先ＣＤTo != null)
                            {
                                cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.得意先ＣＤTo;
                            }
                            if (_koteiChumonHanneiCriteria.コースＣＤFrom != null)
                            {
                                cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.コースＣＤFrom;
                            }
                            if (_koteiChumonHanneiCriteria.コースＣＤTo != null)
                            {
                                cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _koteiChumonHanneiCriteria.コースＣＤTo;
                            }

                            log.Debug(cmd.CommandTextWithParameters());

                            SqlDataAdapter adp = new SqlDataAdapter(cmd);

                            DataTable dt = new DataTable("得意先単価適用マスタ");
                            adp.Fill(dt);

                            // レコードが無い場合はスキップ
                            if (dt.Rows.Count <= 0)
                            {
                                continue;
                            }

                            //***************************
                            // 適用日に該当する諸々を取得
                            //***************************
                            Syohizei syohizei = Syohizei.GetSyohizei(dateFrom);     // 消費税を取得
                            Tokuisaki tokuisaki = null;                             // 得意先マスタ

                            // 先頭の得意先ＣＤを取得
                            int tokuisakiCD = -1;
                            tokuisakiCD = (int)dt.Rows[0]["得意先ＣＤ"];
                            if (tokuisakiMap.ContainsKey(tokuisakiCD) == false)
                            {
                                tokuisaki = Tokuisaki.GetTokuisaki(tokuisakiCD);    // 得意先マスタ取得
                                if (tokuisaki == null)
                                {
                                    continue;
                                }
                                tokuisakiMap[tokuisakiCD] = tokuisaki;
                            }
                            else
                            {
                                tokuisaki = tokuisakiMap[tokuisakiCD];
                            }

                            DateTime haisobi = CalcHaisobi(dateFrom, busyo, tokuisaki);                 // 配送日を算出
                            DateTime seizobi = CalcSeizobi(dateFrom, busyo);                            // 製造日を算出
                            DateTime? date = AdjustSeizobiHaisobi(seizobi, haisobi, busyo, tokuisaki);  // 配送日＜製造日の場合は調整
                            if (date != null)
                            {
                                seizobi = (DateTime)date;
                                haisobi = (DateTime)date;
                            }

                            ChumonHeader chumonHeader = null;       // 注文ヘッダ
                            int meisaiNo = -1;                      // 注文明細の明細番号
                            //*********************************************
                            // 適用日に該当する得意先単価適用マスタをループ
                            //*********************************************
                            foreach (DataRow dr in dt.Rows)
                            {
                                // 祝日の場合、祝日配送区分がOFFであればスキップ
                                // （祝日では無い場合は常にON）
                                if ((int)dr["祝日配送区分"] == 0)
                                {
                                    continue;
                                }

                                // 直前のレコードと得意先が変わった？
                                if (tokuisakiCD != (int)dr["得意先ＣＤ"])
                                {
                                    // 注文ヘッダがnullでなければ追加or更新
                                    if (chumonHeader != null)
                                    {
                                        if (chumonHeader.作成日 == null)
                                        {
                                            historyProcess = History.ProcessType.Create;
                                            ret = chumonHeader.Insert(con, tran);
                                        }
                                        else
                                        {
                                            historyProcess = History.ProcessType.Update;
                                            ret = chumonHeader.Update(con, tran);
                                        }
                                        if (ret == false)
                                        {
                                            MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return false;
                                        }
                                        History.Save(con, tran, this.Text, "注文ヘッダ", chumonHeader.Code, historyProcess);

                                        // 得意先が変わったので、注文ヘッダを取得し直すことを指示する
                                        chumonHeader = null;
                                    }

                                    // 得意先マスタ取得（無ければスキップ）
                                    tokuisakiCD = (int)dr["得意先ＣＤ"];
                                    if (tokuisakiMap.ContainsKey(tokuisakiCD) == false)
                                    {
                                        tokuisaki = Tokuisaki.GetTokuisaki((int)dr["得意先ＣＤ"]);
                                        if (tokuisaki == null)
                                        {
                                            continue;
                                        }
                                        tokuisakiMap[tokuisakiCD] = tokuisaki;
                                    }
                                    else
                                    {
                                        tokuisaki = tokuisakiMap[tokuisakiCD];
                                    }

                                    haisobi = CalcHaisobi(dateFrom, busyo, tokuisaki);                  // 配送日を算出
                                    seizobi = CalcSeizobi(dateFrom, busyo);                             // 製造日を算出
                                    date = AdjustSeizobiHaisobi(seizobi, haisobi, busyo, tokuisaki);    // 配送日＜製造日の場合は調整
                                    if (date != null)
                                    {
                                        seizobi = (DateTime)date;
                                        haisobi = (DateTime)date;
                                    }
                                }

                                // その得意先の休日なら生成せずに次の得意先へ
                                if (tokuisaki.GetHoliday(dateFrom.DayOfWeek))
                                {
                                    continue;

                                }
                                // その日に該当するコースが無い場合は次の得意先へ
                                Course course = GetCourse(tokuisakiCD, dateFrom);
                                if (course == null)
                                {
                                    continue;
                                }

                                if (chumonHeader == null)
                                {
                                    // 注文ヘッダ取得
                                    chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Chuumon, dateFrom, tokuisakiCD, (int)_koteiChumonHanneiCriteria.部署ＣＤ);
                                    // 無ければ生成
                                    if (chumonHeader == null)
                                    {
                                        chumonHeader = new ChumonHeader();
                                        chumonHeader.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                        chumonHeader.喫食日 = dateFrom;
                                        chumonHeader.得意先ＣＤ = tokuisakiCD;
                                        chumonHeader.部署ＣＤ = _koteiChumonHanneiCriteria.部署ＣＤ;
                                        chumonHeader.納品書伝票番号 = null;
                                    }
                                    chumonHeader.コースＣＤ = course.コースＣＤ;
                                    chumonHeader.配送日 = haisobi;
                                    chumonHeader.得意先名 = tokuisaki.得意先名;
                                    chumonHeader.担当者ＣＤ = course.担当者ＣＤ;
                                    chumonHeader.配送先ＣＤ = tokuisaki.受注得意先ＣＤ;
                                    chumonHeader.税区分 = tokuisaki.税区分 ?? (int)Syohizei.ZeiKubun.Uchi;
                                    chumonHeader.丸め処理区分 = tokuisaki.丸め処理区分 ?? (int)Rounding.HasuKubun.Floor;
                                    chumonHeader.備考1 = tokuisaki.備考１;
                                    chumonHeader.備考2 = tokuisaki.備考２;
                                    chumonHeader.備考3 = tokuisaki.備考３;
                                }

                                // 注文明細取得
                                ChumonMeisai chumonMeisai = ChumonMeisai.GetChumonMeisai(Consts.ChumonKubun.Chuumon, dateFrom, tokuisakiCD,
                                                                                         (int)dr["商品ＣＤ"], (int)_koteiChumonHanneiCriteria.部署ＣＤ);
                                if (chumonMeisai == null)
                                {
                                    // 生成
                                    chumonMeisai = new ChumonMeisai();
                                    // 明細番号は(A)で取得したもの＋１を初期値とする
                                    if (meisaiNo < 0)
                                    {
                                        meisaiNo = (int)dr["明細番号"];
                                    }
                                    meisaiNo++;

                                    chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                    chumonMeisai.喫食日 = dateFrom;
                                    chumonMeisai.得意先ＣＤ = tokuisakiCD;
                                    chumonMeisai.部署ＣＤ = chumonHeader.部署ＣＤ;
                                    chumonMeisai.商品ＣＤ = (int)dr["商品ＣＤ"];
                                    chumonMeisai.明細番号 = meisaiNo;
                                    chumonMeisai.製造日 = seizobi;

                                    // 得意先マスタから
                                    chumonMeisai.売掛現金区分 = tokuisaki.売掛現金区分;

                                    // 主要商品リストマスタから
                                    SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)chumonMeisai.部署ＣＤ, (int)chumonMeisai.商品ＣＤ);
                                    if (syuyoSyohin != null)
                                    {
                                        chumonMeisai.商品グループＣＤ = syuyoSyohin.商品グループＣＤ;
                                    }
                                }
                                // 商品金額や税額を算出
                                bool? isKeigen = dr["軽減税率フラグ"] as bool?;
                                chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, (decimal)dr["商品単価税込"], (decimal)dr["商品単価税抜"]);
                                chumonMeisai.商品数量 = (decimal)dr["固定数"];
                                chumonMeisai.商品金額合計 = Syohizei.GetAmount(chumonMeisai.商品数量, chumonMeisai.商品単価);
                                chumonMeisai.消費税額 = syohizei.GetTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, isKeigen, chumonHeader.丸め処理区分);
                                chumonMeisai.商品金額合計税込 = syohizei.GetAmountIncludeTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, isKeigen, chumonHeader.丸め処理区分);
                                chumonMeisai.計算税率 = syohizei.GetTaxRate(isKeigen);

                                // 注文明細を追加or更新
                                if (chumonMeisai.作成日 == null)
                                {
                                    historyProcess = History.ProcessType.Create;
                                    ret = chumonMeisai.Insert(con, tran);
                                }
                                else
                                {
                                    historyProcess = History.ProcessType.Update;
                                    ret = chumonMeisai.Update(con, tran);
                                }
                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);

                                // 得意先単価適用マスタの最終注文日時を更新
                                SqlCommand cmdUpd = con.CreateCommand();
                                cmdUpd.Transaction = tran;
                                cmdUpd.CommandText = "\r\n UPDATE 得意先単価適用マスタ" +
                                                     "\r\n SET 最終注文日時 = getdate()" +
                                                     "\r\n     ,修正担当者ＣＤ = @担当者ＣＤ" +
                                                     "\r\n     ,修正日 = getdate()" +
                                                     "\r\n WHERE 企業ＣＤ = @企業ＣＤ" +
                                                     "\r\n   AND 得意先ＣＤ = @得意先ＣＤ" +
                                                     "\r\n   AND 商品ＣＤ   = @商品ＣＤ" +
                                                     "\r\n   AND 適用開始日 = @適用開始日";
                                cmdUpd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                                cmdUpd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)dr["得意先ＣＤ"];
                                cmdUpd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = (int)dr["商品ＣＤ"];
                                cmdUpd.Parameters.Add("@適用開始日", SqlDbType.Date).Value = (DateTime)dr["適用開始日"];
                                cmdUpd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                                log.Debug(cmdUpd.CommandTextWithParameters());
                                cmdUpd.ExecuteNonQuery();
                            }
                            // ループ終了
                            // 注文ヘッダがnullでなければ追加or更新
                            if (chumonHeader != null)
                            {
                                if (chumonHeader.作成日 == null)
                                {
                                    historyProcess = History.ProcessType.Create;
                                    ret = chumonHeader.Insert(con, tran);
                                }
                                else
                                {
                                    historyProcess = History.ProcessType.Update;
                                    ret = chumonHeader.Update(con, tran);
                                }
                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                History.Save(con, tran, this.Text, "注文ヘッダ", chumonHeader.Code, historyProcess);
                            }
                        }

                        History.Save(con, tran, this.Text, "固定注文反映", "", History.ProcessType.Output);
                        tran.Commit();
                        log.Info("固定注文反映処理を実行しました。");
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            MessageBox.Show("注文明細が重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            History.Error(ex);
                            MessageBox.Show(string.Format("固定注文反映処理に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return false;

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        History.Error(ex);
                        MessageBox.Show(string.Format("固定注文反映処理に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
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
        }

        /// <summary>
        /// 得意先CDに該当するコースCDを返す
        /// </summary>
        /// <param name="tokuisakiCD">コースＣＤ</param>
        /// <param name="targetDate">対象日</param>
        /// <returns>得意先CDに該当するコース</returns>
        private Course GetCourse(int tokuisakiCD, DateTime targetDate)
        {
            try
            {
                Course course = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +               // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n     1 AS 区分" +
                                      "\r\n     ,CT.コースＣＤ" +
                                      "\r\n     ,CM.コース区分" +
                                      "\r\n     ,CM.コース名" +
                                      "\r\n     ,CT.得意先ＣＤ" +
                                      "\r\n     ,CM.部署ＣＤ" +
                                      "\r\n     ,CM.担当者ＣＤ" +
                                      "\r\n     ,CM.工場フラグ" +
                                      "\r\n     ,CM.無効フラグ" +
                                      "\r\n     ,CM.作成担当者ＣＤ" +
                                      "\r\n     ,CM.作成日" +
                                      "\r\n     ,CM.修正担当者ＣＤ" +
                                      "\r\n     ,CM.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     コーステーブル AS CT" +
                                      "\r\n     INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             T.コースＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コーステーブル AS T" +
                                      "\r\n             INNER JOIN 臨時コースマスタ AS M ON M.企業ＣＤ = T.企業ＣＤ AND M.コースＣＤ = T.コースＣＤ AND T.実施日 = M.実施日 AND ISNULL(M.無効フラグ, 0) = 0" +
                                      "\r\n         WHERE" +
                                      "\r\n             T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND T.実施日 = @実施日" +
                                      "\r\n             AND T.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n             AND ISNULL(T.無効フラグ, 0) = 0" +
                                      "\r\n             AND M.コース区分 = @コース区分" +
                                      "\r\n         ) AS R" +
                                      "\r\n         ON R.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n     AND CT.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND CM.コース区分 = @コース区分" +
                                      "\r\n     AND R.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 区分" +
                                      "\r\n     ,RT.コースＣＤ" +
                                      "\r\n     ,RM.コース区分" +
                                      "\r\n     ,RM.コース名" +
                                      "\r\n     ,RT.得意先ＣＤ" +
                                      "\r\n     ,RM.部署ＣＤ" +
                                      "\r\n     ,RM.担当者ＣＤ" +
                                      "\r\n     ,RM.工場フラグ" +
                                      "\r\n     ,RM.無効フラグ" +
                                      "\r\n     ,RM.作成担当者ＣＤ" +
                                      "\r\n     ,RM.作成日" +
                                      "\r\n     ,RM.修正担当者ＣＤ" +
                                      "\r\n     ,RM.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     臨時コーステーブル AS RT" +
                                      "\r\n     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n WHERE" +
                                      "\r\n     RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND RT.実施日 = @実施日" +
                                      "\r\n     AND RT.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n     AND RM.コース区分 = @コース区分" +
                                      "\r\n ORDER BY" +
                                      "\r\n     区分" +
                                      "\r\n     ,コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = targetDate;

                    // 対象日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun(targetDate);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("コースマスタ");
                    adp.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        course = new Course(dr);
                        // 部署が一致するコースで決定する
                        if (_koteiChumonHanneiCriteria.部署ＣＤ == course.部署ＣＤ)
                        {
                            break;
                        }
                    }
                    return course;
                }
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
        }

        #endregion
    }
}

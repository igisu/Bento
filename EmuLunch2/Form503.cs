using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emu.Bento
{
    /// <summary>
    /// 請求書発行
    /// </summary>
    public partial class Form503 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求書発行
        /// </summary>
        private SeikyuPublish _seikyuPublish;

        /// <summary>
        /// 請求書出力条件
        /// </summary>
        private SeikyuCriteria _seikyuCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form503()
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
        private void Form503_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                progressBar1.Visible = false;
                lblProgress.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                //ソートキーのリストを設定する
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求先CD", "コースCD" },
                    new string[] { "G.請求先ＣＤ {0}, C.コースＣＤ, C.コース順",
                                   "C.コースＣＤ {0}, C.コース順, G.請求先ＣＤ" });

                //初期値設定
                this.cboSortKey.Text = "請求先CD";

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _seikyuPublish = new SeikyuPublish();
                seikyuPublishBindingSource.DataSource = _seikyuPublish;

                _seikyuCriteria = new SeikyuCriteria();
                seikyuCriteriaBindingSource.DataSource = _seikyuCriteria;
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
        private void Form503_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F10:                      // 処理開始ボタン押下
                        if (btnPrint.Enabled)           // 非活性の場合はNOP
                        {
                            btnPrint.PerformClick();
                        }
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
                    txtCourseCDFrom.Text = course.コースＣＤ.ToString();
                    _seikyuPublish.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
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
                    txtCourseCDTo.Text = course.コースＣＤ.ToString();
                    _seikyuPublish.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
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
                    txtTokuisakiCDFrom.Text = tokuisaki.得意先ＣＤ.ToString();
                    _seikyuPublish.請求先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameFrom.Text = tokuisaki.得意先名;
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
                    txtTokuisakiCDTo.Text = tokuisaki.得意先ＣＤ.ToString();
                    _seikyuPublish.請求先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameTo.Text = tokuisaki.得意先名;
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
        /// コース外のみチェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkOutOfCourse_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // コース外のみの場合は、コースの指定は不可
                txtCourseCDFrom.Enabled = !(chkOutOfCourse.Checked);
                lblCourseNameFrom.Enabled = !(chkOutOfCourse.Checked);
                btnCourseCDFrom.Enabled = !(chkOutOfCourse.Checked);
                txtCourseCDTo.Enabled = !(chkOutOfCourse.Checked);
                lblCourseNameTo.Enabled = !(chkOutOfCourse.Checked);
                btnCourseCDTo.Enabled = !(chkOutOfCourse.Checked);
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
        /// PDF保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuPublish.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuPublish.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuPublish.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (_seikyuCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // フォルダ選択ダイアログを表示
                string pdfPath;
                using (var dlg = new CommonOpenFileDialog())
                {
                    // フォルダ選択ダイアログ（falseにするとファイル選択ダイアログ）
                    dlg.IsFolderPicker = true;
                    dlg.Title = "請求書PDFの保存先を選択してください";
                    if (dlg.ShowDialog() == CommonFileDialogResult.Cancel)
                    {
                        return;
                    }
                    pdfPath = dlg.FileName;
                }

                // コース外のみの場合はコースをクリアする(☆)
                ReadBinding(txtCourseCDFrom);
                ReadBinding(lblCourseNameFrom);
                ReadBinding(txtCourseCDTo);
                ReadBinding(lblCourseNameTo);

                // 締区分（連結されていないラジオボタン）から値を取得
                if (rdoShimeAll.Checked)
                {
                    _seikyuPublish.締区分 = null;
                }

                if (rdoShimeDaily.Checked)
                {
                    _seikyuPublish.締区分 = (int)Consts.ShimeKubun.Daily;
                }

                if (rdoShimeWeekly.Checked)
                {
                    _seikyuPublish.締区分 = (int)Consts.ShimeKubun.Weekly;
                }

                if (rdoShimeMonthly.Checked)
                {
                    _seikyuPublish.締区分 = (int)Consts.ShimeKubun.Monthly;
                }

                OutputPdf(pdfPath);         // PDF出力処理
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
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuPublish.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuPublish.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuPublish.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (_seikyuCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // コース外のみの場合はコースをクリアする(☆)
                ReadBinding(txtCourseCDFrom);
                ReadBinding(lblCourseNameFrom);
                ReadBinding(txtCourseCDTo);
                ReadBinding(lblCourseNameTo);

                // 締区分（連結されていないラジオボタン）から値を取得
                if (rdoShimeAll.Checked)
                {
                    _seikyuPublish.締区分 = null;
                }

                if (rdoShimeDaily.Checked)
                {
                    _seikyuPublish.締区分 = (int)Consts.ShimeKubun.Daily;
                }

                if (rdoShimeWeekly.Checked)
                {
                    _seikyuPublish.締区分 = (int)Consts.ShimeKubun.Weekly;
                }

                if (rdoShimeMonthly.Checked)
                {
                    _seikyuPublish.締区分 = (int)Consts.ShimeKubun.Monthly;
                }

                PrintSeikyusho();       // 請求書印刷
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
        /// 請求書印刷処理
        /// </summary>
        private async void PrintSeikyusho()
        {
            try
            {
                SeikyuPrint seikyuPrint = new SeikyuPrint(this.Text);       // 請求書発行

                seikyuPrint.Report000 = new Report000A();     // 請求書マージ用ダミー帳票
                PrintSettings printSettings = this.GetPrintSettings(seikyuPrint.Report000);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 印刷対象を取得
                List<SeikyuPrintTarget> seikyuPrintTargetList = GetTarget();
                if (seikyuPrintTargetList == null || seikyuPrintTargetList.Count == 0)
                {
                    MessageBox.Show(this, "出力条件に合う請求書データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                btnPDF.Enabled = false;
                btnPrint.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;
                lblProgress.Text = "";

                // 請求書回収予定日印字フラグを取得
                bool isPrintSeikyuKaisyuYotei = SystemMaster.IsPrintSeikyuKaisyuYotei();

                // 帳票マージ開始
                seikyuPrint.PrintMergeInit();

                // Progress作成時に表示更新用のメソッドを指定する
                // これを時間のかかる処理に渡す
                var p = new Progress<ProgressState>(ShowProgress);

                // 請求書印刷（１つの帳票として出力）
                string errMsg = await Task.Run(() =>
                                    seikyuPrint.PrintSeikyuWhole((int)_seikyuPublish.部署ＣＤ, seikyuPrintTargetList, false, isPrintSeikyuKaisyuYotei, _seikyuCriteria, null, p));
                if (string.IsNullOrEmpty(errMsg))
                {
                    // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                    PrintReport(seikyuPrint.Report000, printSettings);
                }
                else
                {
                    MessageBox.Show(this, errMsg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            finally
            {
                btnPDF.Enabled = true;
                btnPrint.Enabled = true;
                progressBar1.Visible = false;
                lblProgress.Text = "";
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// PDF出力処理
        /// </summary>
        /// <param name="pdfPath">PDF保存先フォルダ</param>
        private async void OutputPdf(string pdfPath)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 印刷対象を取得
                List<SeikyuPrintTarget> seikyuPrintTargetList = GetTarget();
                if (seikyuPrintTargetList == null || seikyuPrintTargetList.Count == 0)
                {
                    MessageBox.Show(this, "出力条件に合う請求書データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                SeikyuPrint seikyuPrint = new SeikyuPrint(this.Text);       // 請求書発行

                btnPDF.Enabled = false;
                btnPrint.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;
                lblProgress.Text = "";

                // 請求書回収予定日印字フラグを取得
                bool isPrintSeikyuKaisyuYotei = SystemMaster.IsPrintSeikyuKaisyuYotei();

                // Progress作成時に表示更新用のメソッドを指定する
                // これを時間のかかる処理に渡す
                var p = new Progress<ProgressState>(ShowProgress);

                // 請求書印刷（請求番号ごとに出力）
                string errMsg = await Task.Run(() =>
                                    // seikyuPrint.PrintSeikyuDivide((int)_seikyuPublish.部署ＣＤ, seikyuPrintTargetList, false, isPrintSeikyuKaisyuYotei, _seikyuCriteria, pdfPath, p));
                                    seikyuPrint.PrintSeikyuDivide((int)_seikyuPublish.部署ＣＤ, seikyuPrintTargetList, isPrintSeikyuKaisyuYotei, _seikyuCriteria, pdfPath, p));
                if (string.IsNullOrEmpty(errMsg))
                {
                    MessageBox.Show(this, "請求書のPDFを出力しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, errMsg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            finally
            {
                btnPDF.Enabled = true;
                btnPrint.Enabled = true;
                progressBar1.Visible = false;
                lblProgress.Text = "";
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 印刷対象を取得
        /// </summary>
        /// <returns>請求書印刷対象リスト</returns>
        private List<SeikyuPrintTarget> GetTarget()
        {
            try
            {
                DataTable dt = new DataTable();
                List<SeikyuPrintTarget> seikyuPrintTargetList = new List<SeikyuPrintTarget>();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    SqlCommand cmd = con.CreateCommand();

                    // 印刷対象を取得
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT DISTINCT");         // -- DISTINCTは念のため
                    sql.AppendLine("     G.請求番号");
                    sql.AppendLine("     ,G.請求日");
                    sql.AppendLine("     ,G.請求先ＣＤ");
                    sql.AppendLine("     ,G.請求先名");
                    if (_seikyuCriteria.今回売上のみを出力 != true)
                    {
                        sql.AppendLine("     ,G.今回請求額");
                    }
                    else
                    {
                        sql.AppendLine("     ,ISNULL(G.買上金額, 0) + ISNULL(G.調整額, 0) + ISNULL(G.消費税, 0) AS 今回請求額");
                    }
                    sql.AppendLine("     ,ISNULL(B.請求書タイプ区分, @請求書タイプ区分明細) AS 請求書タイプ区分");  // 明細をデフォルトとする
                    sql.AppendLine("     ,C.コースＣＤ");
                    sql.AppendLine("     ,C.コース順");
                    sql.AppendLine(" FROM");
                    // -- ①今回請求のキーと、それに対する前回請求のキーを求める（請求一覧と同様に取得しているため）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         G1.企業ＣＤ");
                    sql.AppendLine("         ,G1.請求番号 AS 今回請求番号");
                    sql.AppendLine("         ,G1.請求先ＣＤ");
                    sql.AppendLine("         ,MAX(P1.請求日) AS 前回請求日");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         請求合計 AS G1");
                    sql.AppendLine("         LEFT JOIN 請求合計 AS P1");
                    sql.AppendLine("             ON G1.企業ＣＤ = P1.企業ＣＤ");
                    sql.AppendLine("             AND G1.請求先ＣＤ = P1.請求先ＣＤ");
                    sql.AppendLine("             AND P1.請求日 < G1.請求日");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         G1.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND G1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND G1.請求日 = @請求日");
                    sql.AppendLine("         AND (G1.クレカ決済エラー区分 IS NULL OR G1.クレカ決済エラー区分 = 0)");    // 決済エラーは除外

                    if (_seikyuPublish.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("         AND G1.請求先ＣＤ >= @請求先ＣＤFrom");
                        cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = _seikyuPublish.請求先ＣＤFrom;
                    }
                    if (_seikyuPublish.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("         AND G1.請求先ＣＤ <= @請求先ＣＤTo");
                        cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = _seikyuPublish.請求先ＣＤTo;
                    }
                    if (_seikyuPublish.印刷済みフラグ == null || (bool)_seikyuPublish.印刷済みフラグ == false)
                    {
                        sql.AppendLine("         AND G1.印刷日時 IS NULL");
                    }
                    if (_seikyuPublish.クレカフラグ == null || (bool)_seikyuPublish.クレカフラグ == false)
                    {
                        sql.AppendLine("         AND G1.FR支払フラグ = 0");
                    }
                    if (_seikyuPublish.締区分 != null)
                    {
                        sql.AppendLine("         AND G1.締区分 = @締区分");
                        cmd.Parameters.Add("@締区分", SqlDbType.Int).Value = _seikyuPublish.締区分;
                    }
                    sql.AppendLine("         AND G1.締区分 <> @締区分現金未集金");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         G1.企業ＣＤ");
                    sql.AppendLine("         ,G1.請求番号");
                    sql.AppendLine("         ,G1.請求先ＣＤ");
                    sql.AppendLine("     ) AS P");
                    // -- ②今回請求の詳細
                    sql.AppendLine("     INNER JOIN 請求合計 G ON");
                    sql.AppendLine("         G.企業ＣＤ = P.企業ＣＤ");
                    sql.AppendLine("         AND G.請求番号 = P.今回請求番号");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = G.企業ＣＤ AND S.得意先ＣＤ = G.請求先ＣＤ");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");
                    sql.AppendLine("     INNER JOIN 部署マスタ AS B ON");
                    sql.AppendLine("         B.企業ＣＤ = G.企業ＣＤ");
                    sql.AppendLine("         AND B.部署ＣＤ = G.部署ＣＤ");

                    // コース外がONの場合は、コースＣＤFrom/Toともにnullになっている(☆)
                    if (_seikyuPublish.コースＣＤFrom != null || _seikyuPublish.コースＣＤTo != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.AppendLine("     INNER JOIN");
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
                        // -- 指定した範囲にヒットするコースＣＤのうち、最小のものを取得
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
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                    1 = 1");
                        if (_seikyuPublish.コースＣＤFrom != null)
                        {
                            sql.AppendLine("                    AND T.コースＣＤ >= @コースＣＤFrom");
                            cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _seikyuPublish.コースＣＤFrom;
                        }
                        if (_seikyuPublish.コースＣＤTo != null)
                        {
                            sql.AppendLine("                    AND T.コースＣＤ <= @コースＣＤTo");
                            cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _seikyuPublish.コースＣＤTo;
                        }
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
                        sql.AppendLine("         ON J.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND G.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("         AND J.得意先ＣＤ = C.得意先ＣＤ");
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
                        sql.AppendLine("                 WHERE  ");
                        sql.AppendLine("                     ISNULL(T.無効フラグ, 0) = 0");
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
                        sql.AppendLine("             AND G.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                        sql.AppendLine("             AND J.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    StringBuilder sqlWhere = new StringBuilder();
                    string and = "";
                    if (_seikyuPublish.ゼロ印刷フラグ != true)
                    {
                        sqlWhere.Append(and).AppendLine(" G.今回請求額 != 0");
                        and = " AND ";
                    }
                    if (_seikyuPublish.コース外 == true)
                    {
                        sqlWhere.Append(and).AppendLine(" C.コースＣＤ IS NULL");
                        and = " AND ";
                    }
                    if (sqlWhere.Length != 0)
                    {
                        sql.Append(" WHERE").Append(sqlWhere.ToString());
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@請求書タイプ区分明細", SqlDbType.Int).Value = (int)Consts.SeikyusyoType.Detail;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuPublish.部署ＣＤ;
                    cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = _seikyuPublish.請求日;
                    cmd.Parameters.Add("@締区分現金未集金", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Tsuke;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);
                }

                // データテーブルの内容をリストに詰め直す
                foreach (DataRow dr in dt.Rows)
                {
                    SeikyuPrintTarget seikyuPrintTarget = new SeikyuPrintTarget(dr);
                    seikyuPrintTargetList.Add(seikyuPrintTarget);
                }
                return seikyuPrintTargetList;
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
        /// 進捗状況更新
        /// </summary>
        /// <param name="progressState">プログレスバーを操作するパラメータ</param>
        private void ShowProgress(ProgressState progressState)
        {
            ShowProgress(progressBar1, lblProgress, progressState);
        }
        #endregion
    }
}

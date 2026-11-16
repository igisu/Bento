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
    /// 請求締処理
    /// </summary>
    public partial class Form500 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求締め
        /// </summary>
        private SeikyuShimeCriteria _seikyuShimeCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form500()
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
        private void Form500_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                // コンボボックスの選択肢生成
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

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetShimeKubun(cboShime);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _seikyuShimeCriteria = new SeikyuShimeCriteria();
                seikyuShimeBindingSource.DataSource = _seikyuShimeCriteria;
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
        private void Form500_KeyDown(object sender, KeyEventArgs e)
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
        /// 締め区分コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShime_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 請求締め条件が未生成の場合（フォームロード中）はNOP
                if (_seikyuShimeCriteria == null)
                {
                    return;
                }

                int? shimeKubun = cboShime.SelectedValue as int?;
                if (shimeKubun == null)
                {
                    return;
                }

                switch (shimeKubun)
                {
                    case (int)Consts.ShimeKubun.Weekly:     // 週締め
                        // 週締め曜日のみ入力可能
                        grpWeek.Enabled = true;
                        grpMonth.Enabled = false;

                        // 請求日は特締めの場合に入力可能
                        dtpSeikyubi.Enabled = chkTokushimeWeek.Checked;
                        break;

                    case (int)Consts.ShimeKubun.Monthly:    // 月締め
                        // 月締め日のみ入力可能
                        grpWeek.Enabled = false;
                        grpMonth.Enabled = true;

                        // 請求日は特締めの場合に入力可能
                        dtpSeikyubi.Enabled = chkTokushimeMonth.Checked;
                        break;

                    default:                                // 上記以外（日締め・未選択）
                        // どちらも入力不可
                        grpWeek.Enabled = false;
                        grpMonth.Enabled = false;

                        // 請求日は入力可能
                        dtpSeikyubi.Enabled = true;
                        break;
                }

                // Validatedを待たずにバインド先にセット（締区分プロパティにてエラーチェックを実施している）
                _seikyuShimeCriteria.締区分 = shimeKubun;

                // 締区分に応じたエラーのセット
                SetErrorByShimeKubun(shimeKubun);

                // 請求日を算出
                GenerateSeikyubi(_seikyuShimeCriteria.締区分);
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
        /// 曜日コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboYoubi_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 請求締め条件が未生成の場合（フォームロード中）はNOP
                if (_seikyuShimeCriteria == null)
                {
                    return;
                }

                // 請求日を算出
                GenerateSeikyubi(_seikyuShimeCriteria.締区分);
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
        /// 特締チェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTokushime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // 週締・月締を問わず、チェックONの場合は請求日を入力可能にする
                CheckBox chkTokushime = (CheckBox)sender;
                dtpSeikyubi.Enabled = chkTokushime.Checked;

                // 特締めがOFFになった場合は請求日を算出
                if (chkTokushime.Checked == false)
                {
                    GenerateSeikyubi(_seikyuShimeCriteria.締区分);
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
        /// 月締年月と締日 検証後イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTsukishime_Validated(object sender, EventArgs e)
        {
            try
            {
                // 請求日を算出
                GenerateSeikyubi(_seikyuShimeCriteria.締区分);
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
                    _seikyuShimeCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
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
        /// コース外のみチェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkOutOfCourse_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // コース外のみの場合は、コースの指定は不可
                txtCourseCD.Enabled = !(chkOutOfCourse.Checked);
                lblCourseName.Enabled = !(chkOutOfCourse.Checked);
                btnCourseCD.Enabled = !(chkOutOfCourse.Checked);
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
                    _seikyuShimeCriteria.請求先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameFrom.Text = tokuisaki.得意先名;
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
                    _seikyuShimeCriteria.請求先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiNameTo.Text = tokuisaki.得意先名;
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
        /// 処理開始ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuShimeCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuShimeCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuShimeCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // コース外のみの場合はコースをクリアする
                ReadBinding(txtCourseCD);
                ReadBinding(lblCourseName);

                // 確認メッセージを表示
                string attention = "";
                if (chkTokushimeWeek.Checked || chkTokushimeMonth.Checked)
                {
                    attention = "\n\n【注意】\n特締めの前に通常締めを行っている場合、特締めの請求データを対象にEBデータを作成すると、通常締め分と特締め分の２つがEBデータに出力されます。二重請求にならないよう注意してください。";
                }
                if (MessageBox.Show(this, "請求締め処理を行います。よろしいですか？" + attention,
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;
                    if (SeikyuShimeExec.Run(_seikyuShimeCriteria))
                    {
                        //***************************************************************************************************************************************
                        // 現  象：売掛現金区分が現金の得意先で売掛の売上がある（＝未収金）場合、売上精算にて請求レコードを作成後、
                        //         同じ日に入金（★）があると、請求一覧の残高が減らない。
                        // 原  因：入金入力では、請求日＜入金日≦次の請求日 が成立する入金日の入金を今回の入金としている。
                        //         つまり、請求日の翌日以降を入金の対象にしているため（同日の入金は今回の請求の入金とは見なしていない）
                        // 解決策：★の操作後、売上精算を削除して再度登録し直すと請求レコードを作り直すと、残高は０になっている。
                        //         （ここでは 前回の請求日＜入金日≦請求日 が成立する入金日の入金を、今回の入金としている）
                        //         これを利用して、請求締め処理にて日締が指定されたときは
                        //         （通常通りの処理に加えて）日締＆現金＆前回締日～請求日に売掛の売上がある得意先を対象にして、売上精算の請求締め処理を呼び出す。
                        //***************************************************************************************************************************************
                        if (_seikyuShimeCriteria.締区分 == (int)Consts.ShimeKubun.Daily)
                        {
                            // 日締＆現金に該当する得意先を取得
                            List<string> tokuisakiCDList = GetTokuisakiCD();
                            if (tokuisakiCDList != null && tokuisakiCDList.Count != 0)
                            {
                                UriageSeisanCriteria uriageSeisanCriteria = new UriageSeisanCriteria();
                                uriageSeisanCriteria.精算日 = _seikyuShimeCriteria.請求日;
                                uriageSeisanCriteria.部署ＣＤ = _seikyuShimeCriteria.部署ＣＤ;
                                string[] tokuisakiCDs = tokuisakiCDList.ToArray();
                                if (SeikyuShimeExec.Run(uriageSeisanCriteria, tokuisakiCDs) == false)
                                {
                                    MessageBox.Show(this, "未収金分の締め処理に失敗しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        MessageBox.Show(this, "締め処理を実行しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
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
        /// 締区分に応じたエラーのセット
        /// </summary>
        /// <param name="shimeKubun">締区分</param>
        private void SetErrorByShimeKubun(int? shimeKubun)
        {
            try
            {
                //*************************************************************************************
                // エラーチェック自体は_seikyuShimeの締区分プロパティ設定時に実施しているので、
                // ここでは_seikyuShimeが持っているエラーメッセージをerrorProvider1に対して設定するだけ
                //*************************************************************************************

                // 締区分が有効な場合は締区分のエラーをクリア
                if (shimeKubun != null && shimeKubun >= 0)
                {
                    errorProvider1.SetError(cboShime, null);
                }

                switch (shimeKubun)
                {
                    case (int)Consts.ShimeKubun.Weekly:     // 週締
                        // 月締に関する項目のエラーをクリア
                        errorProvider1.SetError(txtYear, null);
                        errorProvider1.SetError(txtMonth, null);
                        errorProvider1.SetError(txtTsukishimebi, null);

                        // エラーを表示
                        errorProvider1.SetError(cboYoubi, _seikyuShimeCriteria["週締曜日"]);
                        break;

                    case (int)Consts.ShimeKubun.Monthly:    // 月締
                        // 週締に関する項目のエラーをクリア
                        errorProvider1.SetError(cboYoubi, null);

                        // エラーを表示
                        errorProvider1.SetError(txtYear, _seikyuShimeCriteria["月締年"]);
                        errorProvider1.SetError(txtMonth, _seikyuShimeCriteria["月締月"]);
                        errorProvider1.SetError(txtTsukishimebi, _seikyuShimeCriteria["月締日"]);
                        break;

                    default:
                        // 週締と月締に関する項目のエラーをクリア
                        errorProvider1.SetError(cboYoubi, null);
                        errorProvider1.SetError(txtYear, null);
                        errorProvider1.SetError(txtMonth, null);
                        errorProvider1.SetError(txtTsukishimebi, null);
                        break;
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 締区分と特締めフラグに応じた請求日を算出
        /// </summary>
        /// <param name="shimeKubun">締区分</param>
        private void GenerateSeikyubi(int? shimeKubun)
        {
            try
            {
                DateTime? seikyubi = null;

                switch (shimeKubun)
                {
                    case (int)Consts.ShimeKubun.Weekly:     // 週締
                        if (chkTokushimeWeek.Checked)       // 特締チェックがONの場合は、入力済みの請求日を優先する
                        {
                            return;
                        }

                        // 選択されている曜日に該当する、直近の日付
                        int? iYoubi = cboYoubi.SelectedValue as int?;
                        if (iYoubi != null && iYoubi >= 0)
                        {
                            DayOfWeek youbi = (DayOfWeek)iYoubi;
                            DateTime target = DateTime.Today;
                            while (target.DayOfWeek != youbi)
                            {
                                target = target.AddDays(-1);
                            }
                            seikyubi = target;
                        }
                        break;

                    case (int)Consts.ShimeKubun.Monthly:    // 月締
                        if (chkTokushimeMonth.Checked)      // 特締チェックがONの場合は、入力済みの請求日を優先する
                        {
                            return;
                        }

                        // 入力されている年月と締日から求める
                        if (_seikyuShimeCriteria["月締年"] == null &&
                            _seikyuShimeCriteria["月締月"] == null &&
                            _seikyuShimeCriteria["月締日"] == null)
                        {
                            if (_seikyuShimeCriteria.月締日 == 99)
                            {
                                seikyubi = new DateTime((int)_seikyuShimeCriteria.月締年, (int)_seikyuShimeCriteria.月締月, DateTime.DaysInMonth((int)_seikyuShimeCriteria.月締年, (int)_seikyuShimeCriteria.月締月));
                            }
                            else
                            {
                                seikyubi = new DateTime((int)_seikyuShimeCriteria.月締年, (int)_seikyuShimeCriteria.月締月, (int)_seikyuShimeCriteria.月締日);
                            }
                        }
                        break;

                    default:                                // 日締or未選択
                        seikyubi = DateTime.Today;
                        break;
                }

                // 求めた請求日を表示
                _seikyuShimeCriteria.請求日 = seikyubi;
                ReadBinding(dtpSeikyubi);       // コントロールに反映
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 日締＆現金＆売掛の売上がある得意先を返します
        /// </summary>
        /// <returns>締区分=日締＆売掛現金区分=現金＆前回請求～入力された請求日に売掛の売上がある得意先ＣＤのリスト</returns>
        private List<string> GetTokuisakiCD()
        {
            List<string> tokuisakiCDList = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     A.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    // -- 現金の得意先＆掛売の売上明細の金額の合計（売上精算入力_未集金がベース）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         H.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ AS H");
                    sql.AppendLine("         INNER JOIN 売上明細 AS M ON M.企業ＣＤ = H.企業ＣＤ AND M.売上日 = H.売上日 AND M.得意先ＣＤ = H.得意先ＣＤ AND M.部署ＣＤ = H.部署ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T11 ON H.企業ＣＤ = T11.企業ＣＤ AND H.得意先ＣＤ = T11.得意先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS S11 ON S11.企業ＣＤ = T11.企業ＣＤ AND S11.得意先ＣＤ = ISNULL(T11.請求先ＣＤ, T11.得意先ＣＤ)");
                    sql.AppendLine("         LEFT JOIN");
                    // -- 前回締日
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H1.得意先ＣＤ,");
                    sql.AppendLine("                 MAX(H1.請求日) AS 前回締日");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 請求ヘッダ AS H1");
                    sql.AppendLine("                 INNER JOIN 請求合計 AS G1 ON H1.企業ＣＤ = G1.企業ＣＤ AND H1.請求番号 = G1.請求番号");
                    sql.AppendLine("                 INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = H1.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(H1.請求先ＣＤ, H1.得意先ＣＤ)");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H1.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND H1.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                 AND S12.売掛現金区分 = @売掛現金区分現金");    // -- 現金の得意先に掛売したときの請求の場合は0:現金
                    sql.AppendLine("                 AND G1.締区分 = @締区分現金未集金");           // -- 現金の得意先に掛売したときの請求
                    sql.AppendLine("                 AND H1.請求日 < @請求日");                     // -- 今回締日（入力された請求日）
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H1.得意先ＣＤ");
                    sql.AppendLine("             ) AS M11 ON");
                    sql.AppendLine("                 M11.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("         LEFT JOIN");
                    // -- 最古の売掛の売上日（前回締日より未来 と同じ条件式にしたいので、-1日している）
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 U12.得意先ＣＤ,");
                    sql.AppendLine("                 DATEADD(d, -1, MIN(U12.売上日)) AS 最古売上日");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS U12");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS UM12");
                    sql.AppendLine("                 ON UM12.企業ＣＤ = U12.企業ＣＤ");
                    sql.AppendLine("                 AND UM12.売上日 = U12.売上日");
                    sql.AppendLine("                 AND UM12.得意先ＣＤ = U12.得意先ＣＤ");
                    sql.AppendLine("                 AND UM12.部署ＣＤ = U12.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 得意先マスタ AS T12 ON U12.企業ＣＤ = T12.企業ＣＤ AND U12.得意先ＣＤ = T12.得意先ＣＤ");
                    sql.AppendLine("                 INNER JOIN 得意先マスタ AS S12 ON S12.企業ＣＤ = T12.企業ＣＤ AND S12.得意先ＣＤ = ISNULL(T12.請求先ＣＤ, T12.得意先ＣＤ)");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 U12.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND U12.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                 AND UM12.売上区分 = @売上区分売掛");   // -- 売掛
                    sql.AppendLine("                 AND U12.売上日 <= @請求日");           // -- 今回締日（入力された請求日）
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 U12.得意先ＣＤ");
                    sql.AppendLine("             ) AS M12 ON");
                    sql.AppendLine("                 M12.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         H.企業ＣＤ       = @企業ＣＤ");
                    // -- 前回請求が有るものは前回請求日以降");
                    // -- 無ければ売上区分が売掛の売上明細のうち、最古の売上日以降のもの");
                    // -- ※請求締処理の対象ということは必ず掛売になっているはずだが、売上が無かったときのガードの日付を設けておく");
                    sql.AppendLine("         AND H.売上日 > ISNULL(M11.前回締日, ISNULL(M12.最古売上日, '2000/01/01'))");
                    sql.AppendLine("         AND H.売上日 <= @請求日");             // -- 今回締日（入力された請求日）
                    sql.AppendLine("         AND H.部署ＣＤ   = @部署ＣＤ");
                    sql.AppendLine("         AND ISNULL(T11.売掛現金区分, 0) = 0"); // -- 現金
                    sql.AppendLine("         AND M.売上区分   = @売上区分売掛");
                    sql.AppendLine("         AND T11.売掛現金区分 = @売掛現金区分現金");
                    sql.AppendLine("         AND T11.締区分 = @締区分日締");

                    if (_seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("         AND S11.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (_seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("         AND S11.得意先ＣＤ <= @請求先ＣＤTo");
                    }

                    sql.AppendLine("     UNION");
                    // -- 入金入力区分=1＆入金方法=現金の入金明細の入金額（売上精算入力_回収現金がベース）
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         N.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         入金明細 AS N");
                    // -- 請求ヘッダ作成の駆動表②とは異なり、売掛ではなく現金の得意先が対象なので、入金した得意先は請求先ＣＤではない
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T21 ON T21.企業ＣＤ = N.企業ＣＤ AND T21.得意先ＣＤ = N.得意先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS S21 ON S21.企業ＣＤ = T21.企業ＣＤ AND S21.得意先ＣＤ = ISNULL(T21.請求先ＣＤ, T21.得意先ＣＤ)");
                    sql.AppendLine("         LEFT JOIN");
                    // -- 前回締日
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 G2.請求先ＣＤ,");
                    sql.AppendLine("                 MAX(G2.請求日) AS 前回締日");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 請求ヘッダ AS H2");
                    sql.AppendLine("                 INNER JOIN 請求合計 AS G2 ON H2.企業ＣＤ = G2.企業ＣＤ AND H2.請求番号 = G2.請求番号");
                    sql.AppendLine("                 INNER JOIN 得意先マスタ AS S22 ON S22.企業ＣＤ = G2.企業ＣＤ AND S22.得意先ＣＤ = G2.請求先ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H2.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND H2.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                 AND S22.売掛現金区分 = @売掛現金区分現金");    // -- 現金の得意先に掛売したときの請求の場合は0:現金
                    sql.AppendLine("                 AND G2.締区分 = @締区分現金未集金");           // -- 現金の得意先に掛売したときの請求
                    sql.AppendLine("                 AND H2.請求日 < @請求日");                     // -- 今回締日（入力された請求日）
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 G2.請求先ＣＤ");
                    sql.AppendLine("             ) AS M21 ON");
                    sql.AppendLine("                 M21.請求先ＣＤ = N.得意先ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         N.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND N.入金日 > ISNULL(M21.前回締日, '2000/01/01')");
                    sql.AppendLine("         AND N.入金日 <= @請求日");             // -- 今回締日（入力された請求日）
                    sql.AppendLine("         AND N.部署ＣＤ   = @部署ＣＤ");
                    sql.AppendLine("         AND ISNULL(N.入金額, 0) <> 0");
                    sql.AppendLine("         AND T21.売掛現金区分 = @売掛現金区分現金");
                    sql.AppendLine("         AND T21.締区分 = @締区分日締");

                    if (_seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        sql.AppendLine("         AND S21.得意先ＣＤ >= @請求先ＣＤFrom");
                    }
                    if (_seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        sql.AppendLine("         AND S21.得意先ＣＤ <= @請求先ＣＤTo");
                    }
                    sql.AppendLine("     ) AS A");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ");
                    sql.AppendLine("     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = T.企業ＣＤ AND S.得意先ＣＤ = ISNULL(T.請求先ＣＤ, T.得意先ＣＤ)");

                    if (_seikyuShimeCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        // -- 検索条件にコースが指定された場合のみ、INNER JOIN以下を追加すること
                        sql.AppendLine("     INNER JOIN");
                        sql.AppendLine("         (SELECT DISTINCT");
                        sql.AppendLine("             CM.企業ＣＤ");
                        sql.AppendLine("             ,CM.部署ＣＤ");
                        sql.AppendLine("             ,CT.コースＣＤ");
                        sql.AppendLine("             ,CT.得意先ＣＤ");
                        sql.AppendLine("         FROM");
                        sql.AppendLine("             コースマスタ AS CM");
                        sql.AppendLine("             INNER JOIN コーステーブル AS CT ON");
                        sql.AppendLine("                 CM.企業ＣＤ = CT.企業ＣＤ");
                        sql.AppendLine("                 AND CM.コースＣＤ =  CT.コースＣＤ");
                        sql.AppendLine("         WHERE");
                        sql.AppendLine("             CT.コースＣＤ = @コースＣＤ");
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON S.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND S.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _seikyuShimeCriteria.コースＣＤ;
                    }

                    cmd.CommandText = sql.ToString();

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求日", SqlDbType.Date).Value = (DateTime)_seikyuShimeCriteria.請求日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_seikyuShimeCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                    cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                    cmd.Parameters.Add("@締区分日締", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Daily;
                    cmd.Parameters.Add("@締区分現金未集金", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Tsuke;
                    if (_seikyuShimeCriteria.請求先ＣＤFrom != null)
                    {
                        cmd.Parameters.Add("@請求先ＣＤFrom", SqlDbType.Int).Value = _seikyuShimeCriteria.請求先ＣＤFrom;
                    }
                    if (_seikyuShimeCriteria.請求先ＣＤTo != null)
                    {
                        cmd.Parameters.Add("@請求先ＣＤTo", SqlDbType.Int).Value = _seikyuShimeCriteria.請求先ＣＤTo;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    cmd.CommandTimeout = 180;           // サンワでタイムアウトが発生しているので延ばす

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tokuisakiCDList.Add(reader["得意先ＣＤ"].ToString());
                        }
                    }
                    log.Info("未収金締め対象の得意先を検索しました。");
                }

                return tokuisakiCDList;
            }
            catch (BentoException)
            {
                throw;
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

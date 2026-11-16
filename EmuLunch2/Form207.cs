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
    /// 注文一覧
    /// </summary>
    public partial class Form207 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int URIAGEBI_COLUMN       = 0;    // 売上日列インデックス
        private const int COURSE_CD_COLUMN      = 1;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 2;    // コース名列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 3;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 4;    // 得意先名列インデックス
        private const int SYOHIN_CD_COLUMN      = 5;    // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN    = 6;    // 商品名列インデックス
        private const int SYOHIN_GROUP_COLUMN   = 7;    // 商品グループ列インデックス
        private const int TANKA_COLUMN          = 8;    // 単価列インデックス
        private const int KOSU_COLUMN           = 9;    // 個数列インデックス
        private const int KINGAKU_COLUMN        = 10;   // 金額列インデックス
        private const int ZEIRITSU_COLUMN       = 11;   // 税率列インデックス
        private const int KEIGEN_COLUMN         = 12;   // 軽減税率列インデックス
        private const int ZEIKUBUN_COLUMN       = 13;   // 税区分列インデックス
        private const int HAISOBI_COLUMN        = 14;   // 配送日列インデックス
        private const int SEIZOBI_COLUMN        = 15;   // 製造日インデックス
        private const int NYURYOKUSYA_COLUMN    = 16;   // 入力者列インデックス
        private const int CREATE_COLUMN         = 17;   // 作成日時列インデックス
        private const int UPDATE_COLUMN         = 18;   // 修正日時列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 注文一覧検索条件入力
        /// </summary>
        private ChumonListCriteria _chumonListCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form207()
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
        private void Form207_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnNew.Enabled = this.HasAuthEdit;
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                cboShiten.SelectedValueChanged -= cboShiten_SelectedValueChanged;   // 支店ＣＤコンボイベントをいったん削除

                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetSyohinGroup(cboSyohinGroup);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _chumonListCriteria = new ChumonListCriteria();
                _chumonListCriteria.喫食日From = DateTime.Today;
                _chumonListCriteria.喫食日To = DateTime.Today;
                _chumonListCriteria.注文 = true;

                // 部署CDコンボの初期値を設定
                _chumonListCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                chumonListBindingSource.DataSource = _chumonListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "注文日", "コースCD", "得意先CD", "商品CD", "商品グループ", "作成日時", "修正日時" },
                    new string[] { "M.喫食日 {0}, M.得意先ＣＤ, M.明細番号",
                                   "C.コースCD {0}, M.得意先ＣＤ, M.喫食日, M.明細番号",
                                   "M.得意先CD {0}, M.喫食日, M.明細番号",
                                   "M.商品CD {0}, M.得意先ＣＤ, M.喫食日, M.明細番号",
                                   "M.商品グループCD {0}, M.得意先ＣＤ, M.喫食日, M.明細番号",
                                   "H.作成日 {0}, M.得意先ＣＤ, M.喫食日, M.明細番号",
                                   "H.修正日 {0}, M.得意先ＣＤ, M.喫食日, M.明細番号"});

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
        private void Form207_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F6:                       // 新規作成ボタン押下
                    btnNew.PerformClick();
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
        private void Form207_Shown(object sender, EventArgs e)
        {
            // 注文日にフォーカス
            dtpChumonbiFrom.Focus();

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
                _chumonListCriteria.部署ＣＤ = -1;
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
                    _chumonListCriteria.商品ＣＤ = syohin.商品ＣＤ;       // 入力内容を確定させる
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
                    _chumonListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
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
        private void txtSeikyuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSeikyuCD.PerformClick();
            }
        }

        /// <summary>
        /// 請求先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900SeikyuCD_FormClosed);
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
        private void Form900SeikyuCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSeikyuCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtSeikyuCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _chumonListCriteria.請求先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblSeikyuName.Text = tokuisaki.得意先名;
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
                    _chumonListCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
        /// 受注得意先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtJuchuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnJuchuCD.PerformClick();
            }
        }

        /// <summary>
        /// 受注得意先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJuchuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900JuchuCD_FormClosed);
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
        /// 得意先選択ダイアログクローズ（受注得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900JuchuCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtJuchuCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtJuchuCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _chumonListCriteria.受注得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblJuchuName.Text = tokuisaki.得意先名;
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
        /// 入力者ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNyuryokuCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnNyuryokuCD.PerformClick();
            }
        }

        /// <summary>
        /// 入力者ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNyuryokuCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904TantosyaCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 担当者選択ダイアログクローズ（入力者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904TantosyaCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtNyuryokuCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtNyuryokuCD.Text = tantousya.担当者ＣＤ.ToString();
                    _chumonListCriteria.入力者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblNyuryokuName.Text = tantousya.担当者名;
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
                if (_chumonListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_chumonListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _chumonListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     M.企業ＣＤ");
                    sql.AppendLine("     ,H.コースＣＤ");
                    sql.AppendLine("     ,C.コース名");
                    sql.AppendLine("     ,M.喫食日");
                    sql.AppendLine("     ,M.得意先ＣＤ");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,M.部署ＣＤ");
                    sql.AppendLine("     ,M.明細番号");
                    sql.AppendLine("     ,H.配送日");
                    sql.AppendLine("     ,M.製造日");
                    sql.AppendLine("     ,M.商品ＣＤ");
                    sql.AppendLine("     ,S.商品名");
                    sql.AppendLine("     ,M.商品グループＣＤ");
                    sql.AppendLine("     ,G.商品グループ名略称 AS 商品グループ名");
                    sql.AppendLine("     ,M.売掛現金区分");
                    sql.AppendLine("     ,VU.売掛現金区分名");
                    sql.AppendLine("     ,M.商品単価");
                    sql.AppendLine("     ,M.商品数量");
                    sql.AppendLine("     ,M.商品金額合計");
                    sql.AppendLine("     ,M.消費税額");
                    sql.AppendLine("     ,M.商品金額合計税込");
                    sql.AppendLine("     ,M.計算税率");
                    sql.AppendLine("     ,case when isnull(S.軽減税率フラグ, 0) <> 0 then '○' else null end as 軽減税率対象");
                    sql.AppendLine("     ,H.税区分");
                    sql.AppendLine("     ,VZ.税区分名");
                    sql.AppendLine("     ,M.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,M.作成日");
                    sql.AppendLine("     ,M.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,M.修正日");
                    sql.AppendLine("     ,H.作成担当者ＣＤ AS ヘッダ作成担当者ＣＤ");
                    sql.AppendLine("     ,A3.担当者名 AS ヘッダ作成担当者名");
                    sql.AppendLine("     ,H.作成日 AS ヘッダ作成日時");
                    sql.AppendLine("     ,H.修正担当者ＣＤ AS ヘッダ修正担当者ＣＤ");
                    sql.AppendLine("     ,A4.担当者名 AS ヘッダ修正担当者名");
                    sql.AppendLine("     ,H.修正日 AS ヘッダ修正日時");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     注文明細 AS M");
                    sql.AppendLine("     LEFT JOIN 注文ヘッダ         AS H  ON H.企業ＣＤ = M.企業ＣＤ AND H.注文区分         = M.注文区分 AND H.喫食日 = M.喫食日 AND H.得意先ＣＤ = M.得意先ＣＤ AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN コースマスタ       AS C  ON C.企業ＣＤ = H.企業ＣＤ AND C.コースＣＤ       = H.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ         AS B  ON B.企業ＣＤ = M.企業ＣＤ AND B.部署ＣＤ         = M.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ       AS T  ON T.企業ＣＤ = M.企業ＣＤ AND T.得意先ＣＤ       = M.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_税区分           AS VZ ON VZ.税区分CD = H.税区分");
                    sql.AppendLine("     LEFT JOIN 商品マスタ         AS S  ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ         = M.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品グループマスタ AS G  ON G.企業ＣＤ = M.企業ＣＤ AND G.商品グループＣＤ = M.商品グループＣＤ");
                    sql.AppendLine("     LEFT JOIN V_売掛現金区分     AS VU ON VU.売掛現金区分CD = M.売掛現金区分");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ       AS A1 ON A1.企業ＣＤ = M.企業ＣＤ AND M.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ       AS A2 ON A2.企業ＣＤ = M.企業ＣＤ AND M.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ       AS A3 ON A3.企業ＣＤ = H.企業ＣＤ AND H.作成担当者ＣＤ = A3.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ       AS A4 ON A4.企業ＣＤ = H.企業ＣＤ AND H.修正担当者ＣＤ = A4.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine(" AND M.注文区分 = @注文区分");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_chumonListCriteria.支店ＣＤ != null && _chumonListCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine(" and B.支店ＣＤ = @支店ＣＤ ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.支店ＣＤ;
                    }

                    if (_chumonListCriteria.部署ＣＤ != null && _chumonListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and M.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.部署ＣＤ;
                    }

                    if (_chumonListCriteria.喫食日From != null)
                    {
                        sql.AppendLine(" and M.喫食日 >= @喫食日From ");
                        cmd.Parameters.Add("@喫食日From", SqlDbType.Date).Value = _chumonListCriteria.喫食日From;
                    }

                    if (_chumonListCriteria.喫食日To != null)
                    {
                        sql.AppendLine(" and M.喫食日 <= @喫食日To ");
                        cmd.Parameters.Add("@喫食日To", SqlDbType.Date).Value = _chumonListCriteria.喫食日To;
                    }

                    if (_chumonListCriteria.配送日From != null)
                    {
                        sql.AppendLine(" and H.配送日 >= @配送日From ");
                        cmd.Parameters.Add("@配送日From", SqlDbType.Date).Value = _chumonListCriteria.配送日From;
                    }

                    if (_chumonListCriteria.配送日To != null)
                    {
                        sql.AppendLine(" and H.配送日 <= @配送日To ");
                        cmd.Parameters.Add("@配送日To", SqlDbType.Date).Value = _chumonListCriteria.配送日To;
                    }

                    if (_chumonListCriteria.製造日From != null)
                    {
                        sql.AppendLine(" and M.製造日 >= @製造日From ");
                        cmd.Parameters.Add("@製造日From", SqlDbType.Date).Value = _chumonListCriteria.製造日From;
                    }

                    if (_chumonListCriteria.製造日To != null)
                    {
                        sql.AppendLine(" and M.製造日 <= @製造日To ");
                        cmd.Parameters.Add("@製造日To", SqlDbType.Date).Value = _chumonListCriteria.製造日To;
                    }

                    if (_chumonListCriteria.商品ＣＤ != null)
                    {
                        sql.AppendLine(" and M.商品ＣＤ = @商品ＣＤ ");
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.商品ＣＤ;
                    }

                    if (_chumonListCriteria.商品グループＣＤ != null && _chumonListCriteria.商品グループＣＤ != -1)
                    {
                        sql.AppendLine(" and M.商品グループＣＤ = @商品グループＣＤ ");
                        cmd.Parameters.Add("@商品グループＣＤ", SqlDbType.Int).Value = _chumonListCriteria.商品グループＣＤ;
                    }

                    if (_chumonListCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine(" and C.コースＣＤ = @コースＣＤ ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _chumonListCriteria.コースＣＤ;
                    }

                    if (_chumonListCriteria.請求先ＣＤ != null)
                    {
                        sql.AppendLine(" and ISNULL(T.請求先ＣＤ, T.得意先ＣＤ) = @請求先ＣＤ ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.請求先ＣＤ;
                    }

                    if (_chumonListCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine(" and M.得意先ＣＤ = @得意先ＣＤ ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.得意先ＣＤ;
                    }

                    if (_chumonListCriteria.受注得意先ＣＤ != null)
                    {
                        sql.AppendLine(" and ISNULL(T.受注得意先ＣＤ, T.得意先ＣＤ) = @受注得意先ＣＤ ");
                        cmd.Parameters.Add("@受注得意先ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.受注得意先ＣＤ;
                    }

                    if (_chumonListCriteria.入力者ＣＤ != null)
                    {
                        sql.AppendLine(" and (H.作成担当者ＣＤ = @入力者ＣＤ OR H.修正担当者ＣＤ = @入力者ＣＤ)");
                        cmd.Parameters.Add("@入力者ＣＤ", SqlDbType.Int).Value = _chumonListCriteria.入力者ＣＤ;
                    }

                    if (_chumonListCriteria.注文 == true)
                    {
                        cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    }
                    else
                    {
                        cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    }

                    if (_chumonListCriteria.作成日時From != null)
                    {
                        sql.AppendLine(" and H.作成日 >= @作成日時From ");
                        cmd.Parameters.Add("@作成日時From", SqlDbType.DateTime).Value = _chumonListCriteria.作成日時From;
                    }

                    if (_chumonListCriteria.作成日時To != null)
                    {
                        sql.AppendLine(" and H.作成日 <= @作成日時To ");
                        cmd.Parameters.Add("@作成日時To", SqlDbType.DateTime).Value = _chumonListCriteria.作成日時To;
                    }

                    if (_chumonListCriteria.修正日時From != null)
                    {
                        sql.AppendLine(" and H.修正日 >= @修正日時From ");
                        cmd.Parameters.Add("@修正日時From", SqlDbType.DateTime).Value = _chumonListCriteria.修正日時From;
                    }

                    if (_chumonListCriteria.修正日時To != null)
                    {
                        sql.AppendLine(" and H.修正日 <= @修正日時To ");
                        cmd.Parameters.Add("@修正日時To", SqlDbType.DateTime).Value = _chumonListCriteria.修正日時To;
                    }

                    if (this.rdoKeigenYes.Checked == true)
                    {
                        sql.AppendLine(" and ISNULL(S.軽減税率フラグ, 0) = 1 ");
                    }
                    if (this.rdoKeigenNo.Checked == true)
                    {
                        sql.AppendLine(" and ISNULL(S.軽減税率フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("注文一覧");
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
                    bindingSource1.DataMember = "注文一覧";

                    log.Info("注文一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(URIAGEBI_COLUMN,       "喫食日");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,      "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN,    "商品名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_GROUP_COLUMN,   "商品グループ名");
                fpSpread1_Sheet1.BindDataColumn(TANKA_COLUMN,          "商品単価");
                fpSpread1_Sheet1.BindDataColumn(KOSU_COLUMN,           "商品数量");
                fpSpread1_Sheet1.BindDataColumn(KINGAKU_COLUMN,        "商品金額合計税込");
                fpSpread1_Sheet1.BindDataColumn(ZEIRITSU_COLUMN,       "計算税率");
                fpSpread1_Sheet1.BindDataColumn(KEIGEN_COLUMN,         "軽減税率対象");
                fpSpread1_Sheet1.BindDataColumn(ZEIKUBUN_COLUMN,       "税区分名");
                fpSpread1_Sheet1.BindDataColumn(HAISOBI_COLUMN,        "配送日");
                fpSpread1_Sheet1.BindDataColumn(SEIZOBI_COLUMN,        "製造日");
                fpSpread1_Sheet1.BindDataColumn(NYURYOKUSYA_COLUMN,    "ヘッダ修正担当者名");
                fpSpread1_Sheet1.BindDataColumn(CREATE_COLUMN,         "ヘッダ作成日時");
                fpSpread1_Sheet1.BindDataColumn(UPDATE_COLUMN,         "ヘッダ修正日時");

                // 背景色をリセット
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;

                    // 予測を検索した場合は背景色を変える
                    if (_chumonListCriteria.予測 == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                    }
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, SYOHIN_GROUP_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = KOSU_COLUMN; columnIndex <= KINGAKU_COLUMN; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
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
                _chumonListCriteria = new ChumonListCriteria();
                _chumonListCriteria.喫食日From = DateTime.Today;
                _chumonListCriteria.喫食日To = DateTime.Today;
                _chumonListCriteria.注文 = true;

                // 部署CDコンボの初期値を設定
                _chumonListCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                chumonListBindingSource.DataSource = _chumonListCriteria;

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
        /// 新規作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowNew();      // 詳細表示（新規レコード）
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
        /// 詳細表示（新規レコード）
        /// </summary>
        private void ShowNew()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                ChumonMeisaiCriteria chumonMeisaiCriteria = new ChumonMeisaiCriteria();
                Form200 form = new Form200(chumonMeisaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form200_FormClosed);
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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                ChumonMeisaiCriteria chumonMeisaiCriteria = new ChumonMeisaiCriteria();
                chumonMeisaiCriteria.stopOverwrite = true;
                chumonMeisaiCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                chumonMeisaiCriteria.得意先名 = dr["得意先名"] as string;
                chumonMeisaiCriteria.部署ＣＤ = dr["部署ＣＤ"] as int?;
                chumonMeisaiCriteria.注文日 = dr["喫食日"] as DateTime?;
                Form200 form = new Form200(chumonMeisaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form200_FormClosed);
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
        private void Form200_FormClosed(object sender, FormClosedEventArgs e)
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

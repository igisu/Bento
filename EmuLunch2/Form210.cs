using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 曜日別固定数注文反映
    /// </summary>
    public partial class Form210 : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYOHIN_CD_COLUMN   = 0;   // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 1;   // 商品参照ボタン列インデックス
        private const int TARGET_CHK_COLUMN  = 2;   // 対象チェックボックス列インデックス
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 曜日別固定数注文反映条件
        /// </summary>
        private YoubiChumonHanneiCriteria _youbiChumonHanneiCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="youbiChumonHanneiCriteria">曜日別固定数注文反映条件</param>
        public Form210(YoubiChumonHanneiCriteria youbiChumonHanneiCriteria)
        {
            InitializeComponent();

            _youbiChumonHanneiCriteria = new YoubiChumonHanneiCriteria(youbiChumonHanneiCriteria);
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
        private void Form210_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                progressBar1.Visible = false;
                lblProgress.Text = "";

                // 権限によりボタンを制御
                btnExec.Enabled = this.HasAuthEdit;

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                youbiChumonHanneiCriteriaBindingSource.DataSource = _youbiChumonHanneiCriteria;

                GetTargetSyohin();      // 作成対象商品の一覧を取得
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
        private void Form210_KeyDown(object sender, KeyEventArgs e)
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
                    _youbiChumonHanneiCriteria.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
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
                    _youbiChumonHanneiCriteria.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
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
                    _youbiChumonHanneiCriteria.得意先ＣＤFrom = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
                    _youbiChumonHanneiCriteria.得意先ＣＤTo = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
        /// 全選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dt = ds.Tables["商品一覧"];
                for (int i = 0 ; i < dt.Rows.Count ; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr["対象"] = 1;
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
        /// 全解除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dt = ds.Tables["商品一覧"];
                for (int i = 0 ; i < dt.Rows.Count ; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr["対象"] = 0;
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
        private async void btnExec_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "実行できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_youbiChumonHanneiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_youbiChumonHanneiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _youbiChumonHanneiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 選択されている商品のセットを取得
                string message = null;
                HashSet<int> syohinCdSet = new HashSet<int>();
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["商品一覧"];
                if (dt.Rows.Count == 0)
                {
                    message = "曜日注文設定にレコードがありません。";
                }
                else
                {
                    var list = dt.Rows.Cast<DataRow>().Where(row => row["対象"] as int? == 1).ToList();
                    foreach (var row in list)
                    {
                        syohinCdSet.Add((int)row["商品ＣＤ"]);
                    }
                    if (syohinCdSet.Count == 0)
                    {
                        message = "対象商品が未選択です。";
                    }
                }
                if (string.IsNullOrEmpty(message) == false)
                {
                    if (MessageBox.Show(message + "\nこのまま続行すると注文が作成されません。（削除するだけです）\n続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 適用日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                int warningFutureDays = SystemMaster.GetWarningFutureDays();
                DateTime warnDate = DateTime.Today.AddDays(warningFutureDays);
                if (_youbiChumonHanneiCriteria.適用日From > warnDate || _youbiChumonHanneiCriteria.適用日To > warnDate)
                {
                    if (MessageBox.Show(this, "適用日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 確認メッセージを表示
                if (MessageBox.Show(this, "曜日別固定数注文反映処理を行います。よろしいですか？",
                    this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                    lblProgress.Text = "";
                    this.MdiParent.Cursor = Cursors.WaitCursor;

                    // Progress作成時に表示更新用のメソッドを指定する
                    // これを時間のかかる処理に渡す
                    var p = new Progress<ProgressState>(ShowProgress);

                    // 曜日別固定数注文反映処理を非同期で呼ぶ
                    bool ret = await Task.Run(() => Exec(syohinCdSet, p));

                    if (ret)
                    {
                        MessageBox.Show(this, "曜日別固定数注文反映処理を実行しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            finally
            {
                progressBar1.Visible = false;
                lblProgress.Text = "";
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
        /// 作成対象商品の一覧を取得
        /// </summary>
        private void GetTargetSyohin()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT DISTINCT" +
                                      "\r\n     A.商品ＣＤ" +
                                      "\r\n     ,S.商品名" +
                                      "\r\n     ,CASE WHEN @商品ＣＤ IS NULL THEN 1 ELSE" +                 // -- 商品CDが未設定なら対象はすべてON
                                      "\r\n         CASE WHEN A.商品ＣＤ = @商品ＣＤ THEN 1 ELSE 0 END" +   // -- 商品CDが設定されていれば、その商品のみON
                                      "\r\n     END AS 対象" +
                                      "\r\n FROM" +
                                      "\r\n     曜日別注文設定マスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_youbiChumonHanneiCriteria.商品ＣＤ != null)
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.商品ＣＤ;
                    }
                    else
                    {
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("商品一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                }

                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");
                fpSpread1_Sheet1.BindDataColumn(TARGET_CHK_COLUMN,  "対象");
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
        /// 曜日別固定数注文反映処理
        /// </summary>
        /// <param name="syohinCdSet">作成対象の商品CDのセット</param>
        /// <param name="p">進行状況の更新のプロバイダー</param>
        /// <returns>成功した場合は True</returns>
        private bool Exec(HashSet<int> syohinCdSet, IProgress<BaseForm.ProgressState> p)
        {
            bool ret = false;
            History.ProcessType historyProcess = History.ProcessType.Update;
            Busyo busyo = Busyo.GetBusyo((int)_youbiChumonHanneiCriteria.部署ＣＤ);     // 部署マスタを取得（製造日／配送日算出日数の取得用）
            UserData userData = UserData.getInstance();
            Dictionary<int, Tokuisaki> tokuisakiMap = new Dictionary<int, Tokuisaki>();

            try
            {
                // 処理対象の曜日別注文設定マスタと曜日フラグマップを取得
                List<YoubiChumonSettei> youbiChumonSetteiList;
                Dictionary<int, YoubiChumonFlags> youbiChumonFlagsMap;
                GetData(out youbiChumonSetteiList, out youbiChumonFlagsMap);
                if (youbiChumonSetteiList.Count == 0)
                {
                    MessageBox.Show("条件に該当する注文設定がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                DateTime dateFrom = (DateTime)_youbiChumonHanneiCriteria.適用日From;
                DateTime dateTo = (DateTime)_youbiChumonHanneiCriteria.適用日To;

                BaseForm.ProgressState progressState = new BaseForm.ProgressState();
                progressState.Style = ProgressBarStyle.Blocks;
                progressState.Maximum = youbiChumonSetteiList.Count * ((dateTo - dateFrom).Days + 1);   // 曜日別注文設定マスタの件数×適用日の日数
                progressState.Value = 0;
                progressState.Message = string.Format("{0}/{1}", progressState.Value, progressState.Maximum);
                p.Report(progressState);

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        History.Save(con, tran, this.Text, "曜日別固定数注文反映", "", History.ProcessType.Start);
                        log.Info("曜日別固定数注文反映処理を開始しました。");

                        // 注文ヘッダ削除
                        if (DeleteChumon(con, tran, "注文ヘッダ") == false)
                        {
                            return false;
                        }

                        // 注文明細削除
                        if (DeleteChumon(con, tran, "注文明細") == false)
                        {
                            return false;
                        }

                        //*****************************
                        // 適用開始日～終了日までループ
                        //*****************************
                        for (; dateFrom <= dateTo; dateFrom = dateFrom.AddDays(1))
                        {
                            //***************************
                            // 適用日に該当する諸々を取得
                            //***************************
                            Shukujitsu shukujitsu = GetShukujitsu(dateFrom);        // 祝日マスタを取得
                            Syohizei syohizei = Syohizei.GetSyohizei(dateFrom);     // 消費税を取得
                            Tokuisaki tokuisaki = null;                             // 得意先マスタ

                            // 先頭の得意先ＣＤを取得
                            int tokuisakiCD = -1;
                            tokuisakiCD = (int)youbiChumonSetteiList[0].得意先ＣＤ;
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
                            int meisaiNo = 0;                       // 注文明細の明細番号
                            //*****************************
                            // 曜日別注文設定マスタをループ
                            //*****************************
                            foreach (YoubiChumonSettei youbiChumonSettei in youbiChumonSetteiList)
                            {
                                // 進捗状況更新
                                progressState.Value++;
                                progressState.Message = string.Format("{0}/{1}", progressState.Value, progressState.Maximum);
                                p.Report(progressState);

                                // 得意先に該当する曜日フラグを取得
                                YoubiChumonFlags youbiChumonFlags = youbiChumonFlagsMap[(int)youbiChumonSettei.得意先ＣＤ];

                                // その得意先の休日なら生成せずに次の得意先or商品へ
                                if (Tokuisaki.GetHoliday(youbiChumonFlags.休日設定, dateFrom.DayOfWeek))
                                {
                                    continue;
                                }

                                // 曜日ごとのフラグと注文数を取得
                                bool flg = false;
                                decimal? suryo = null;
                                if (shukujitsu != null)
                                {
                                    flg = youbiChumonFlags.祝日有効;
                                    suryo = youbiChumonSettei.注文数_祝;
                                }
                                else
                                {
                                    switch (dateFrom.DayOfWeek)
                                    {
                                        case DayOfWeek.Sunday:      // 日曜
                                            flg = youbiChumonFlags.日曜有効;
                                            suryo = youbiChumonSettei.注文数_日;
                                            break;

                                        case DayOfWeek.Monday:      // 月曜
                                            flg = youbiChumonFlags.平日有効;
                                            suryo = youbiChumonSettei.注文数_月;
                                            break;

                                        case DayOfWeek.Tuesday:     // 火曜
                                            flg = youbiChumonFlags.平日有効;
                                            suryo = youbiChumonSettei.注文数_火;
                                            break;

                                        case DayOfWeek.Wednesday:   // 水曜
                                            flg = youbiChumonFlags.平日有効;
                                            suryo = youbiChumonSettei.注文数_水;
                                            break;

                                        case DayOfWeek.Thursday:    // 木曜
                                            flg = youbiChumonFlags.平日有効;
                                            suryo = youbiChumonSettei.注文数_木;
                                            break;

                                        case DayOfWeek.Friday:      // 金曜
                                            flg = youbiChumonFlags.平日有効;
                                            suryo = youbiChumonSettei.注文数_金;
                                            break;

                                        case DayOfWeek.Saturday:    // 土曜
                                            flg = youbiChumonFlags.土曜有効;
                                            suryo = youbiChumonSettei.注文数_土;
                                            break;
                                    }
                                }

                                // その日に該当するコースが無い場合は次の得意先or商品へ
                                if (flg == false)
                                {
                                    continue;
                                }

                                // 数量未設定なら次の得意先or商品へ
                                if ((suryo ?? 0m) == 0m)
                                {
                                    continue;
                                }

                                // 作成対象商品でない場合は次の得意先or商品へ
                                if (syohinCdSet.Contains((int)youbiChumonSettei.商品ＣＤ) == false)
                                {
                                    continue;
                                }

                                // 直前のレコードと得意先が変わった？
                                if (tokuisakiCD != (int)youbiChumonSettei.得意先ＣＤ)
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
                                    tokuisakiCD = (int)youbiChumonSettei.得意先ＣＤ;
                                    if (tokuisakiMap.ContainsKey(tokuisakiCD) == false)
                                    {
                                        tokuisaki = Tokuisaki.GetTokuisaki((int)youbiChumonSettei.得意先ＣＤ);
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

                                // 曜日フラグがONでも実際のコースが無ければスキップ
                                Course course = GetCourse(tokuisakiCD, dateFrom);
                                if (course == null)
                                {
                                    continue;
                                }

                                if (chumonHeader == null)
                                {
                                    // 注文ヘッダ取得
                                    chumonHeader = ChumonHeader.GetChumonHeader(con, tran, Consts.ChumonKubun.Chuumon, dateFrom, tokuisakiCD, (int)_youbiChumonHanneiCriteria.部署ＣＤ);
                                    // 無ければ生成
                                    if (chumonHeader == null)
                                    {
                                        chumonHeader = new ChumonHeader();
                                        chumonHeader.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                        chumonHeader.喫食日 = dateFrom;
                                        chumonHeader.得意先ＣＤ = tokuisakiCD;
                                        chumonHeader.部署ＣＤ = _youbiChumonHanneiCriteria.部署ＣＤ;
                                        chumonHeader.納品書伝票番号 = null;

                                        meisaiNo = 0;
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
                                ChumonMeisai chumonMeisai = ChumonMeisai.GetChumonMeisai(con, tran, Consts.ChumonKubun.Chuumon, dateFrom, tokuisakiCD,
                                                                                         (int)youbiChumonSettei.商品ＣＤ, (int)_youbiChumonHanneiCriteria.部署ＣＤ);
                                if (chumonMeisai == null)
                                {
                                    // 生成
                                    chumonMeisai = new ChumonMeisai();
                                    meisaiNo++;

                                    chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                    chumonMeisai.喫食日 = dateFrom;
                                    chumonMeisai.得意先ＣＤ = tokuisakiCD;
                                    chumonMeisai.部署ＣＤ = chumonHeader.部署ＣＤ;
                                    chumonMeisai.商品ＣＤ = (int)youbiChumonSettei.商品ＣＤ;
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
                                bool? isKeigen = youbiChumonSettei.軽減税率フラグ;
                                chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, (decimal)youbiChumonSettei.商品単価税込, (decimal)youbiChumonSettei.商品単価税抜);
                                chumonMeisai.商品数量 = (decimal)suryo;
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

                        History.Save(con, tran, this.Text, "曜日別固定数注文反映", "", History.ProcessType.Output);
                        tran.Commit();
                        log.Info("曜日別固定数注文反映処理を実行しました。");
                        p.Report(progressState);        // 最後に念のため進捗状況更新
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
                            MessageBox.Show(string.Format("曜日別固定数注文反映処理に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return false;

                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        tran.Rollback();
                        History.Error(ex);
                        MessageBox.Show(string.Format("曜日別固定数注文反映処理に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 処理対象の曜日別注文設定マスタ取得
        /// </summary>
        /// <returns>処理対象の曜日別注文設定マスタのリスト</returns>
        /// <summary>
        /// 処理対象の曜日別注文設定マスタ取得
        /// </summary>
        /// <param name="youbiChumonSetteiList">処理対象の曜日別注文設定マスタのリスト</param>
        /// <param name="youbiChumonFlagsMap">得意先CDをキーとして、曜日有効フラグを値に持つマップ</param>
        private void GetData(out List<YoubiChumonSettei> youbiChumonSetteiList, out Dictionary<int, YoubiChumonFlags> youbiChumonFlagsMap)
        {
            try
            {
                youbiChumonSetteiList = new List<YoubiChumonSettei>();
                youbiChumonFlagsMap = new Dictionary<int, YoubiChumonFlags>();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();
                    sql.Append("\r\n SELECT DISTINCT" +     // -- コースによる絞り込みの際、同じ得意先に複数のコースが設定されていると複数のレコードを取得してしまうのでDISTINCTでそれを回避する
                               "\r\n     A.企業ＣＤ" +
                               "\r\n     ,A.得意先ＣＤ" +
                               "\r\n     ,T.得意先名" +
                               "\r\n     ,A.商品ＣＤ" +
                               "\r\n     ,ISNULL(K.商品単価税抜, S.商品単価税抜) AS 商品単価税抜" +
                               "\r\n     ,ISNULL(K.商品単価税込, S.商品単価税込) AS 商品単価税込" +
                               "\r\n     ,S.軽減税率フラグ" +
                               "\r\n     ,A.注文数_日" +
                               "\r\n     ,A.注文数_月" +
                               "\r\n     ,A.注文数_火" +
                               "\r\n     ,A.注文数_水" +
                               "\r\n     ,A.注文数_木" +
                               "\r\n     ,A.注文数_金" +
                               "\r\n     ,A.注文数_土" +
                               "\r\n     ,A.注文数_祝" +
                               "\r\n     ,A.作成担当者ＣＤ" +
                               "\r\n     ,A1.担当者名 AS 作成担当者名" +
                               "\r\n     ,A.作成日" +
                               "\r\n     ,A.修正担当者ＣＤ" +
                               "\r\n     ,A2.担当者名 AS 修正担当者名" +
                               "\r\n     ,A.修正日" +
                               "\r\n     ,WEK.有効フラグ AS 平日有効" +
                               "\r\n     ,SAT.有効フラグ AS 土曜有効" +
                               "\r\n     ,SUN.有効フラグ AS 日曜有効" +
                               "\r\n     ,HOL.有効フラグ AS 祝日有効" +
                               "\r\n     ,T.休日設定" +
                               "\r\n     ,T.祝日配送区分" +
                               "\r\n FROM" +
                               "\r\n     曜日別注文設定マスタ AS A" +
                               "\r\n     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ" +
                               "\r\n     INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                               "\r\n     LEFT JOIN 得意先単価マスタ AS K ON K.企業ＣＤ = A.企業ＣＤ AND K.得意先ＣＤ = A.得意先ＣＤ AND K.商品ＣＤ = A.商品ＣＤ AND ISNULL(K.無効フラグ, 0) = 0" +
                               "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                               "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                               "\r\n     LEFT JOIN" +
                               // -- 平日
                               "\r\n         (SELECT DISTINCT" +
                               "\r\n             CM.企業ＣＤ" +
                               "\r\n             ,CT.得意先ＣＤ" +
                               "\r\n             ,1 AS 有効フラグ" +
                               "\r\n         FROM" +
                               "\r\n             コースマスタ AS CM" +
                               "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                               "\r\n         WHERE" +
                               "\r\n             CM.コース区分 = @コース区分平日" +
                               "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                               "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                               "\r\n         ) AS WEK" +
                               "\r\n         ON WEK.企業ＣＤ = A.企業ＣＤ" +
                               "\r\n         AND WEK.得意先ＣＤ = A.得意先ＣＤ" +
                               "\r\n     LEFT JOIN" +
                               // -- 土曜
                               "\r\n         (SELECT DISTINCT" +
                               "\r\n             CM.企業ＣＤ" +
                               "\r\n             ,CT.得意先ＣＤ" +
                               "\r\n             ,1 AS 有効フラグ" +
                               "\r\n         FROM" +
                               "\r\n             コースマスタ AS CM" +
                               "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                               "\r\n         WHERE" +
                               "\r\n             CM.コース区分 = @コース区分土曜" +
                               "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                               "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                               "\r\n         ) AS SAT" +
                               "\r\n         ON SAT.企業ＣＤ = A.企業ＣＤ" +
                               "\r\n         AND SAT.得意先ＣＤ = A.得意先ＣＤ" +
                               "\r\n     LEFT JOIN" +
                               // -- 日曜
                               "\r\n         (SELECT DISTINCT" +
                               "\r\n             CM.企業ＣＤ" +
                               "\r\n             ,CT.得意先ＣＤ" +
                               "\r\n             ,1 AS 有効フラグ" +
                               "\r\n         FROM" +
                               "\r\n             コースマスタ AS CM" +
                               "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                               "\r\n         WHERE" +
                               "\r\n             CM.コース区分 = @コース区分日曜" +
                               "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                               "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                               "\r\n         ) AS SUN" +
                               "\r\n         ON SUN.企業ＣＤ = A.企業ＣＤ" +
                               "\r\n         AND SUN.得意先ＣＤ = A.得意先ＣＤ" +
                               "\r\n     LEFT JOIN" +
                               // -- 祝日
                               "\r\n         (SELECT DISTINCT" +
                               "\r\n             CM.企業ＣＤ" +
                               "\r\n             ,CT.得意先ＣＤ" +
                               "\r\n             ,1 AS 有効フラグ" +
                               "\r\n         FROM" +
                               "\r\n             コースマスタ AS CM" +
                               "\r\n             INNER JOIN コーステーブル CT ON CT.企業ＣＤ = CM.企業ＣＤ AND CT.コースＣＤ = CM.コースＣＤ" +
                               "\r\n         WHERE" +
                               "\r\n             CM.コース区分 = @コース区分祝日" +
                               "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                               "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                               "\r\n         ) AS HOL" +
                               "\r\n         ON HOL.企業ＣＤ = A.企業ＣＤ" +
                               "\r\n         AND HOL.得意先ＣＤ = A.得意先ＣＤ");

                    if (_youbiChumonHanneiCriteria.コースＣＤFrom != null || _youbiChumonHanneiCriteria.コースＣＤTo != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.Append("\r\n     INNER JOIN" +
                                   "\r\n         (SELECT DISTINCT" +
                                   "\r\n             CM.企業ＣＤ" +
                                   "\r\n             ,CM.部署ＣＤ" +
                                   "\r\n             ,CM.コース名" +
                                   "\r\n             ,CT.コースＣＤ" +
                                   "\r\n             ,CT.得意先ＣＤ" +
                                   "\r\n             ,CT.コース順" +
                                   "\r\n         FROM" +
                                   "\r\n             コースマスタ AS CM" +
                                   "\r\n             INNER JOIN コーステーブル AS CT ON" +
                                   "\r\n                 CM.企業ＣＤ = CT.企業ＣＤ" +
                                   "\r\n                 AND CM.コースＣＤ =  CT.コースＣＤ" +
                                   "\r\n         WHERE" +
                                   "\r\n             1 = 1");

                        if (_youbiChumonHanneiCriteria.コースＣＤFrom != null)
                        {
                            sql.Append("\r\n             AND CT.コースＣＤ >= @コースＣＤFrom");
                            cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.コースＣＤFrom;
                        }

                        if (_youbiChumonHanneiCriteria.コースＣＤTo != null)
                        {
                            sql.Append("\r\n             AND CT.コースＣＤ <= @コースＣＤTo");
                            cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.コースＣＤTo;
                        }
                        sql.Append("\r\n         ) AS C" +
                                   "\r\n         ON A.企業ＣＤ = C.企業ＣＤ" +
                                   "\r\n         AND A.得意先ＣＤ = C.得意先ＣＤ");
                    }
                    // 「Web得意先の注文を含める」がチェックされていない場合のみ条件を追加する
                    if (!chkContainsWebTokuisaki.Checked)
                    {
                        sql.Append("\r\n     LEFT JOIN WEB_M_得意先 AS WT ON WT.企業ＣＤ = A.企業ＣＤ AND WT.得意先ＣＤ = A.得意先ＣＤ AND ISNULL(WT.無効フラグ, 0) = 0");
                    }

                    sql.Append("\r\n WHERE" +
                               "\r\n     A.企業ＣＤ = @企業ＣＤ");

                    if (_youbiChumonHanneiCriteria.部署ＣＤ != null && _youbiChumonHanneiCriteria.部署ＣＤ != -1)
                    {
                        sql.Append("\r\n     AND T.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.部署ＣＤ;
                    }
                    sql.Append("\r\n     AND ISNULL(T.無効フラグ, 0) = 0" +
                               "\r\n     AND (T.受注得意先CD IS NULL OR (T.受注得意先ＣＤ = T.得意先ＣＤ))" +
                               "\r\n     AND ISNULL(S.無効フラグ, 0) = 0");

                    // 「Web得意先の注文を含める」がチェックされていない場合のみ条件を追加する
                    if (!chkContainsWebTokuisaki.Checked)
                    {
                        sql.Append("\r\n     AND WT.得意先ＣＤ IS NULL");
                    }

                    if (_youbiChumonHanneiCriteria.得意先ＣＤFrom != null)
                    {
                        sql.Append("\r\n     AND A.得意先ＣＤ >= @得意先ＣＤFrom");
                        cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.得意先ＣＤFrom;
                    }

                    if (_youbiChumonHanneiCriteria.得意先ＣＤTo != null)
                    {
                        sql.Append("\r\n     AND A.得意先ＣＤ <= @得意先ＣＤTo");
                        cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.得意先ＣＤTo;
                    }

                    sql.Append("\r\n ORDER BY" +
                               "\r\n     A.得意先ＣＤ" +
                               "\r\n     ,A.商品ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コース区分平日", SqlDbType.Int).Value = (int)Consts.CourseKubun.Weekday;
                    cmd.Parameters.Add("@コース区分土曜", SqlDbType.Int).Value = (int)Consts.CourseKubun.Saturday;
                    cmd.Parameters.Add("@コース区分日曜", SqlDbType.Int).Value = (int)Consts.CourseKubun.Sunday;
                    cmd.Parameters.Add("@コース区分祝日", SqlDbType.Int).Value = (int)Consts.CourseKubun.Holiday;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("曜日別注文設定マスタ");
                    adp.Fill(dt);

                    // 曜日有効フラグマップを生成
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 曜日別注文設定マスタ
                        YoubiChumonSettei youbiChumonSettei = new YoubiChumonSettei(dr);
                        youbiChumonSetteiList.Add(youbiChumonSettei);

                        // 曜日有効フラグ
                        YoubiChumonFlags youbiChumonFlags = new YoubiChumonFlags(dr);
                        youbiChumonFlagsMap[youbiChumonFlags.得意先ＣＤ] = youbiChumonFlags;
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
        /// 注文ヘッダ／明細削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tableName">テーブル名</param>
        /// <returns></returns>
        private bool DeleteChumon(SqlConnection con, SqlTransaction tran, string tableName)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                try
                {
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;

                    StringBuilder sql = new StringBuilder();
                    // 曜日別注文設定マスタに該当する得意先の注文を削除
                    sql.Append("\r\n DELETE H" +
                               "\r\n FROM" +
                               "\r\n     {0} AS H" +
                               "\r\n     INNER JOIN 曜日別注文設定マスタ AS A ON" +
                               "\r\n         A.企業ＣＤ = H.企業ＣＤ" +
                               "\r\n         AND A.得意先ＣＤ = H.得意先ＣＤ");
                    // 「Web得意先の注文を含める」がチェックされていない場合のみ条件を追加する
                    if (!chkContainsWebTokuisaki.Checked)
                    {
                        sql.Append("\r\n     LEFT JOIN WEB_M_得意先 AS WT" +
                               "\r\n         ON WT.企業ＣＤ = A.企業ＣＤ" +
                               "\r\n         AND WT.得意先ＣＤ = A.得意先ＣＤ" +
                               "\r\n         AND ISNULL(WT.無効フラグ, 0) = 0");
                    }

                    if (_youbiChumonHanneiCriteria.コースＣＤFrom != null || _youbiChumonHanneiCriteria.コースＣＤTo != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先に複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
                        sql.Append("\r\n     INNER JOIN" +
                                   "\r\n         (SELECT DISTINCT" +
                                   "\r\n             CM.企業ＣＤ" +
                                   "\r\n             ,CM.部署ＣＤ" +
                                   "\r\n             ,CM.コース名" +
                                   "\r\n             ,CT.コースＣＤ" +
                                   "\r\n             ,CT.得意先ＣＤ" +
                                   "\r\n             ,CT.コース順" +
                                   "\r\n         FROM" +
                                   "\r\n             コースマスタ AS CM" +
                                   "\r\n             INNER JOIN コーステーブル AS CT ON" +
                                   "\r\n                 CM.企業ＣＤ = CT.企業ＣＤ" +
                                   "\r\n                 AND CM.コースＣＤ =  CT.コースＣＤ" +
                                   "\r\n         WHERE" +
                                   "\r\n             1 = 1");

                        if (_youbiChumonHanneiCriteria.コースＣＤFrom != null)
                        {
                            sql.Append("\r\n             AND CT.コースＣＤ >= @コースＣＤFrom");
                            cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.コースＣＤFrom;
                        }

                        if (_youbiChumonHanneiCriteria.コースＣＤTo != null)
                        {
                            sql.Append("\r\n             AND CT.コースＣＤ <= @コースＣＤTo");
                            cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.コースＣＤTo;
                        }
                        sql.Append("\r\n         ) AS C" +
                                   "\r\n         ON A.企業ＣＤ = C.企業ＣＤ" +
                                   "\r\n         AND A.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    sql.Append("\r\n WHERE" +
                               "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                               "\r\n     AND H.注文区分 = @注文区分注文");

                    if (_youbiChumonHanneiCriteria.適用日From != null)
                    {
                        sql.Append("\r\n     AND H.喫食日 >= @適用日From");
                        cmd.Parameters.Add("@適用日From", SqlDbType.Date).Value = _youbiChumonHanneiCriteria.適用日From;
                    }

                    if (_youbiChumonHanneiCriteria.適用日To != null)
                    {
                        sql.Append("\r\n     AND H.喫食日 <= @適用日To");
                        cmd.Parameters.Add("@適用日To", SqlDbType.Date).Value = _youbiChumonHanneiCriteria.適用日To;
                    }

                    if (_youbiChumonHanneiCriteria.得意先ＣＤFrom != null)
                    {
                        sql.Append("\r\n     AND H.得意先ＣＤ >= @得意先ＣＤFrom");
                        cmd.Parameters.Add("@得意先ＣＤFrom", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.得意先ＣＤFrom;
                    }

                    if (_youbiChumonHanneiCriteria.得意先ＣＤTo != null)
                    {
                        sql.Append("\r\n     AND H.得意先ＣＤ <= @得意先ＣＤTo");
                        cmd.Parameters.Add("@得意先ＣＤTo", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.得意先ＣＤTo;
                    }

                    if (_youbiChumonHanneiCriteria.部署ＣＤ != null && _youbiChumonHanneiCriteria.部署ＣＤ != -1)
                    {
                        sql.Append("\r\n     AND H.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _youbiChumonHanneiCriteria.部署ＣＤ;
                    }

                    // 「Web得意先の注文を含める」がチェックされていない場合のみ条件を追加する
                    if (!chkContainsWebTokuisaki.Checked)
                    {
                        sql.Append("\r\n     AND WT.得意先ＣＤ IS NULL");
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;

                    cmd.CommandText = string.Format(sql.ToString(), tableName);

                    log.Debug(cmd.CommandTextWithParameters());
                    cmd.ExecuteNonQuery();

                    return true;
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("{0}の{1}に失敗しました。\n{1}", tableName, historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("{0}の{1}に失敗しました。\n{1}", tableName, historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
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
                        if (_youbiChumonHanneiCriteria.部署ＣＤ == course.部署ＣＤ)
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

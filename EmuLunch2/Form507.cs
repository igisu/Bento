using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 請求EBデータ再出力
    /// </summary>
    public partial class Form507 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEIKYU_NO_COLUMN      = 0;    // 請求番号列インデックス
        private const int SEIKYUBI_COLUMN       = 1;    // 請求日列インデックス
        private const int BUSYO_NAME_COLUMN     = 2;    // 部署列インデックス
        private const int COURSE_CD_COLUMN      = 3;    // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN    = 4;    // コース名列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 5;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 6;    // 得意先名列インデックス
        private const int KONKAI_SEIKYU_COLUMN  = 7;    // 今回請求額列インデックス
        private const int NYUKIN_COLUMN         = 8;    // 入金額列インデックス
        private const int URIKAKEZAN_COLUMN     = 9;    // 売掛残高列インデックス
        private const int OUTPUT_CHK_COLUMN     = 10;   // EB出力対象チェックボックス列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求EBデータ再作成_共通条件
        /// </summary>
        private SeikyuDataEBReCreate _seikyuDataEBReCreate;

        /// <summary>
        /// 請求EBデータ再作成_共通条件 EB結果ファイル指定時スナップショット
        /// </summary>
        private SeikyuDataEBReCreate _seikyuDataEBReCreateSnapShotImport = null;

        /// <summary>
        /// 請求EBデータ再作成_共通条件 未回収金検索時スナップショット
        /// </summary>
        private SeikyuDataEBReCreate _seikyuDataEBReCreateSnapShotCriteria = null;

        /// <summary>
        /// 請求EBデータ再作成_EB結果ファイル読み込み
        /// </summary>
        private SeikyuDataEBReCreateImport _seikyuDataEBReCreateImport;

        /// <summary>
        /// 請求EBデータ再作成_未回収金検索条件
        /// </summary>
        private SeikyuDataEBReCreateCriteria _seikyuDataEBReCreateCriteria;

        /// <summary>
        /// 請求EBデータ（パース結果）
        /// </summary>
        private EBContainer _container;

        /// <summary>
        /// EB結果ファイルから取得した請求データ
        /// </summary>
        private DataTable _dtFile = null;

        /// <summary>
        /// 未回収金を検索した請求データ
        /// </summary>
        private DataTable _dtMiKaisyu = null;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form507()
        {
            InitializeComponent();

            _seikyuDataEBReCreateImport = new SeikyuDataEBReCreateImport();
            _seikyuDataEBReCreateCriteria = new SeikyuDataEBReCreateCriteria();
            _seikyuDataEBReCreate = new SeikyuDataEBReCreate(_seikyuDataEBReCreateImport, _seikyuDataEBReCreateCriteria);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mikaisyuListCriteria">未回収金一覧検索条件</param>
        public Form507(MikaisyuListCriteria mikaisyuListCriteria) : this()
        {
            // 未回収金一覧検索条件を、当画面の条件に設定する
            _seikyuDataEBReCreate.部署ＣＤ = mikaisyuListCriteria.部署ＣＤ;
            _seikyuDataEBReCreate.EB銀行支店ＣＤ = mikaisyuListCriteria.EB銀行支店ＣＤ;

            _seikyuDataEBReCreateCriteria.請求年 = mikaisyuListCriteria.請求年;
            _seikyuDataEBReCreateCriteria.請求月 = mikaisyuListCriteria.請求月;
            _seikyuDataEBReCreateCriteria.コースＣＤ = mikaisyuListCriteria.コースＣＤ;
            _seikyuDataEBReCreateCriteria.得意先ＣＤ = mikaisyuListCriteria.得意先ＣＤ;
            if (mikaisyuListCriteria.締区分 == (int)Consts.ShimeKubun.Monthly)
            {
                _seikyuDataEBReCreateCriteria.月締日 = mikaisyuListCriteria.月締日;
            }
            else
            {
                _seikyuDataEBReCreateCriteria.月締日 = null;
            }
            _seikyuDataEBReCreateCriteria.売掛残ゼロ = mikaisyuListCriteria.売掛残ゼロ;
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
        private void Form507_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                lblCount.Text = "";

                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

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
                seikyuDataEBReCreateImportBindingSource.DataSource = _seikyuDataEBReCreateImport;
                seikyuDataEBReCreateCriteriaBindingSource.DataSource = _seikyuDataEBReCreateCriteria;
                seikyuDataEBReCreateBindingSource.DataSource = _seikyuDataEBReCreate;

                // 出力先EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(lstBusyoEB, (int)cboBusyo.SelectedValue);

                // 未回収金一覧から開かれた場合は、EB口座を設定する
                if (string.IsNullOrEmpty(_seikyuDataEBReCreate.EB銀行支店ＣＤ) == false)
                {
                    lstBusyoEB.SelectedValue = _seikyuDataEBReCreate.EB銀行支店ＣＤ;
                }

                // ソートキー・表示数の選択肢生成
                // DataTableに対してソートするので、テーブルのエイリアスは不要
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求番号", "請求日", "コースCD", "得意先CD" },
                    new string[] { "請求番号 {0}, 請求先ＣＤ",
                                   "請求日 {0}, 請求先ＣＤ",
                                   "コースＣＤ {0}, 請求先ＣＤ, 請求日",
                                   "請求先ＣＤ {0}, 請求日"});

                cboSortKey.Text = "請求日";
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
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);      // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form507_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F10:
                        btnOutput.PerformClick();       // 出力ボタン押下
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
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
                // 出力先EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(lstBusyoEB, (int)cboBusyo.SelectedValue);
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
        /// 取込ファイルキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSelectFile.PerformClick();
            }
        }

        /// <summary>
        /// 取込ファイルファイル参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFile.Text = openFileDialog1.FileName;     // データソース更新モードがOnPropertyChanged
            }
        }

        /// <summary>
        /// ファイル読込ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportFile_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuDataEBReCreateImport.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuDataEBReCreateImport.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuDataEBReCreateImport.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // EB銀行支店ＣＤは出力先EB口座リストボックスにバインドしていない（※）ので、ここで取得する
                // ※部署を選択し直すとリストの先頭が選択状態になり、部署を確定すると以前に記憶していたEB銀行支店ＣＤが選択状態になる(選択位置が変化する)ので
                _seikyuDataEBReCreate.EB銀行支店ＣＤ = (string)lstBusyoEB.SelectedValue;

                // EB銀行支店ＣＤが未選択？
                if (string.IsNullOrEmpty(_seikyuDataEBReCreate.EB銀行支店ＣＤ))
                {
                    MessageBox.Show(this, "ＥＢ口座を選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 未回収金一覧検索時と共通条件が変化している？
                if (_seikyuDataEBReCreateSnapShotCriteria != null)
                {
                    if (_seikyuDataEBReCreate.Equals(_seikyuDataEBReCreateSnapShotCriteria) == false)
                    {
                        MessageBox.Show(this, "共通の条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // ファイル読み込み
                if (InputEBData(txtFile.Text) == false)
                {
                    return;
                }

                // EBによる入金に失敗した請求データの取得
                GetDataByFile();

                // 読み込み実行時の検索条件を保存（出力時に使用する）
                // ただし、検索結果が0件の場合は（マージに影響が無いので）検索していないことにする
                if (_dtFile.Rows.Count == 0)
                {
                    _seikyuDataEBReCreateSnapShotImport = null;
                }
                else
                {
                    _seikyuDataEBReCreateSnapShotImport = new SeikyuDataEBReCreate(_seikyuDataEBReCreate);
                }

                // 未回収金の検索結果とマージして表示
                ShowData();
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
                    _seikyuDataEBReCreateCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtCourseCD);
                    ReadBinding(lblCourseName);
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
                    _seikyuDataEBReCreateCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCD);
                    ReadBinding(lblTokuisakiName);
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
                if (_seikyuDataEBReCreateCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuDataEBReCreateCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuDataEBReCreateCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // EB銀行支店ＣＤは出力先EB口座リストボックスにバインドしていない（※）ので、ここで取得する
                // ※部署を選択し直すとリストの先頭が選択状態になり、部署を確定すると以前に記憶していたEB銀行支店ＣＤが選択状態になる(選択位置が変化する)ので
                _seikyuDataEBReCreate.EB銀行支店ＣＤ = (string)lstBusyoEB.SelectedValue;

                // EB銀行支店ＣＤが未選択？
                if (string.IsNullOrEmpty(_seikyuDataEBReCreate.EB銀行支店ＣＤ))
                {
                    MessageBox.Show(this, "ＥＢ口座を選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // EB結果ファイル読み込み時と共通条件が変化している？
                if (_seikyuDataEBReCreateSnapShotImport != null)
                {
                    if (_seikyuDataEBReCreate.Equals(_seikyuDataEBReCreateSnapShotImport) == false)
                    {
                        MessageBox.Show(this, "共通の条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    // 請求EBデータ出力に必要な項目も取得する
                    sql.AppendLine("SELECT");
                    sql.AppendLine("     D.請求番号");
                    sql.AppendLine("     ,D.請求日");
                    sql.AppendLine("     ,D.締区分");
                    sql.AppendLine("     ,D.部署ＣＤ");
                    sql.AppendLine("     ,D.部署名");
                    sql.AppendLine("     ,D.コースＣＤ");
                    sql.AppendLine("     ,D.コース名");
                    sql.AppendLine("     ,D.請求先ＣＤ");
                    sql.AppendLine("     ,D.請求先名");
                    sql.AppendLine("     ,D.前回請求額");
                    sql.AppendLine("     ,D.入金額");
                    sql.AppendLine("     ,D.繰越額");
                    sql.AppendLine("     ,D.買上金額");
                    sql.AppendLine("     ,D.調整額");
                    sql.AppendLine("     ,D.消費税");
                    sql.AppendLine("     ,D.今回請求額");
                    sql.AppendLine("     ,D.当月入金額");
                    sql.AppendLine("     ,(D.今回請求額 - D.当月入金額) AS 売掛残高");
                    sql.AppendLine("     ,0 AS EB出力対象");
                    sql.AppendLine("     ,D.得意先名");
                    sql.AppendLine("     ,D.銀行ＣＤ");
                    sql.AppendLine("     ,D.銀行名カナ");
                    sql.AppendLine("     ,D.支店ＣＤ");
                    sql.AppendLine("     ,D.支店名カナ");
                    sql.AppendLine("     ,D.口座種別");
                    sql.AppendLine("     ,D.口座番号");
                    sql.AppendLine("     ,D.口座名義人");
                    sql.AppendLine("     ,(D.今回請求額 - D.当月入金額) AS 請求金額");
                    sql.AppendLine("     ,D.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         A.請求番号");
                    sql.AppendLine("         ,A.請求日");
                    sql.AppendLine("         ,A.締区分");
                    sql.AppendLine("         ,A.部署ＣＤ");
                    sql.AppendLine("         ,B.部署名");
                    sql.AppendLine("         ,C.コースＣＤ");
                    sql.AppendLine("         ,C.コース名");
                    sql.AppendLine("         ,A.請求先ＣＤ");
                    sql.AppendLine("         ,A.請求先名");
                    sql.AppendLine("         ,A.前回請求額");
                    sql.AppendLine("         ,A.入金額");
                    sql.AppendLine("         ,A.繰越額");
                    sql.AppendLine("         ,A.買上金額");
                    sql.AppendLine("         ,A.調整額");
                    sql.AppendLine("         ,A.消費税");
                    sql.AppendLine("         ,A.今回請求額");
                    sql.AppendLine("         ,T.得意先名");
                    sql.AppendLine("         ,T.金融機関ＣＤ AS 銀行ＣＤ");
                    sql.AppendLine("         ,K.銀行名カナ");
                    sql.AppendLine("         ,T.金融機関支店ＣＤ AS 支店ＣＤ");
                    sql.AppendLine("         ,KB.支店名カナ");
                    sql.AppendLine("         ,T.口座種別");
                    sql.AppendLine("         ,T.口座番号");
                    sql.AppendLine("         ,T.口座名義人");
                    sql.AppendLine("         ,A.今回請求額 AS 請求金額");
                    sql.AppendLine("         ,A.請求先ＣＤ AS 得意先ＣＤ");
                    // -- 入金明細から入金額の合計を取得する相関サブクエリ（スカラサブクエリ）
                    sql.AppendLine("         ,(SELECT");
                    sql.AppendLine("             ISNULL(SUM(N.入金額), 0) AS 入金額");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             入金明細 AS N");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             N.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("             AND N.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("             AND N.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("             AND N.入金日 > A.請求日");
                    sql.AppendLine("             AND N.入金日 <= @請求年月日");
                    sql.AppendLine("         ) AS 当月入金額");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         請求合計 AS A");
                    sql.AppendLine("         INNER JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 G.企業ＣＤ");
                    sql.AppendLine("                 ,G.部署ＣＤ");
                    sql.AppendLine("                 ,G.請求先ＣＤ");
                    sql.AppendLine("                 ,MAX(G.請求日) AS 請求日");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 請求合計 AS G");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 G.企業ＣＤ = @企業ＣＤ");

                    if (_seikyuDataEBReCreate.全部署出力フラグ != true)
                    {
                        sql.AppendLine("                 AND G.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuDataEBReCreateCriteria.部署ＣＤ;
                    }

                    if (_seikyuDataEBReCreateCriteria.得意先ＣＤ != null && _seikyuDataEBReCreateCriteria.得意先ＣＤ != -1)
                    {
                        sql.AppendLine("                 AND G.請求先ＣＤ = @請求先ＣＤ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _seikyuDataEBReCreateCriteria.得意先ＣＤ;
                    }

                    sql.AppendLine("                 AND G.請求日 <= @請求年月日");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 G.企業ＣＤ");
                    sql.AppendLine("                 ,G.部署ＣＤ");
                    sql.AppendLine("                 ,G.請求先ＣＤ");
                    sql.AppendLine("             ) AS SG");
                    sql.AppendLine("             ON A.企業ＣＤ = SG.企業ＣＤ");
                    sql.AppendLine("             AND A.部署ＣＤ = SG.部署ＣＤ");
                    sql.AppendLine("             AND A.請求先ＣＤ = SG.請求先ＣＤ");
                    sql.AppendLine("             AND A.請求日 = SG.請求日");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("         INNER JOIN 金融機関名称 AS K ON K.銀行ＣＤ = T.金融機関ＣＤ");
                    sql.AppendLine("         INNER JOIN 金融機関支店名称 AS KB ON KB.銀行ＣＤ = T.金融機関ＣＤ AND KB.支店ＣＤ = T.金融機関支店ＣＤ");
                    sql.AppendLine("         LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");

                    if (_seikyuDataEBReCreateCriteria.コースＣＤ != null)
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
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _seikyuDataEBReCreateCriteria.コースＣＤ;
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
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         A.締区分 = @締区分月締");          // 締区分は月締固定
                    sql.AppendLine("         AND T.支払方法 = @支払方法振替");  // 支払方法も振替固定
                    sql.AppendLine("         AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ");

                    if (_seikyuDataEBReCreateCriteria.月締日 != null)
                    {
                        sql.AppendLine("         AND T.月締日 = @月締日");
                        cmd.Parameters.Add("@月締日", SqlDbType.Int).Value = _seikyuDataEBReCreateCriteria.月締日;
                    }

                    sql.AppendLine("     ) AS D");

                    // 売掛残が0円でも表示する場合は、売掛残を条件に追加しない（0円の売掛残を表示しない場合は条件に追加する）
                    if (_seikyuDataEBReCreateCriteria.売掛残ゼロ == null || _seikyuDataEBReCreateCriteria.売掛残ゼロ == false)
                    {
                        sql.AppendLine(" WHERE (D.今回請求額 - D.当月入金額) <> 0");
                    }
                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求年月日", SqlDbType.Date).Value = _seikyuDataEBReCreateCriteria.請求年月日;
                    cmd.Parameters.Add("@締区分月締", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;
                    cmd.Parameters.Add("@支払方法振替", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Furikae;
                    cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = _seikyuDataEBReCreateCriteria.EB銀行支店ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    _dtMiKaisyu = new DataTable();
                    adp.Fill(_dtMiKaisyu);

                    log.Info("請求EBデータ再出力（未回収金一覧）を検索しました。");
                }

                // 検索実行時の検索条件を保存（出力時に使用する）
                // ただし、検索結果が0件の場合は（マージに影響が無いので）検索していないことにする
                if (_dtMiKaisyu.Rows.Count == 0)
                {
                    _seikyuDataEBReCreateSnapShotCriteria = null;
                }
                else
                {
                    _seikyuDataEBReCreateSnapShotCriteria = new SeikyuDataEBReCreate(_seikyuDataEBReCreate);
                }

                // 未回収金の検索結果とマージして表示
                ShowData();
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
                DataTable dt = ds.Tables["請求EBデータ再出力"];
                // 合計行は除く
                for (int i = 0 ; i < dt.Rows.Count - 1 ; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr["EB出力対象"] = 1;
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
                DataTable dt = ds.Tables["請求EBデータ再出力"];
                // 合計行は除く
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    DataRow dr = dt.Rows[i];
                    dr["EB出力対象"] = 0;
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

        /// <summary>
        /// Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)bindingSource1.Current).Row;
            dr.EndEdit();
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
                _seikyuDataEBReCreateImport = new SeikyuDataEBReCreateImport();
                seikyuDataEBReCreateImportBindingSource.DataSource = _seikyuDataEBReCreateImport;

                _seikyuDataEBReCreateCriteria = new SeikyuDataEBReCreateCriteria();
                seikyuDataEBReCreateCriteriaBindingSource.DataSource = _seikyuDataEBReCreateCriteria;

                _seikyuDataEBReCreate = new SeikyuDataEBReCreate(_seikyuDataEBReCreateImport, _seikyuDataEBReCreateCriteria);
                seikyuDataEBReCreateBindingSource.DataSource = _seikyuDataEBReCreate;

                // スナップショットもクリア
                _seikyuDataEBReCreateSnapShotImport = null;
                _seikyuDataEBReCreateSnapShotCriteria = null;

                // 出力先EB口座の選択肢を、部署にぶら下がるものに再設定する
                SetBusyoEB(lstBusyoEB, (int)cboBusyo.SelectedValue);

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // 検索結果もクリアする
                _dtFile = null;
                _dtMiKaisyu = null;

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
        /// 出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutput_Click(object sender, EventArgs e)
        {
            try
            {
                // 口座引落EBデータが出力可能？
                if (CanOutput() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                OutputEB();       // EBデータ出力
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
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 出力先EB口座リスト設定
        /// </summary>
        /// <param name="lst">出力先EB口座リスト</param>
        /// <param name="busyoCD">部署ＣＤ</param>
        private void SetBusyoEB(ListBox lst, int? busyoCD)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.銀行ＣＤ" +
                                      "\r\n     ,A.支店ＣＤ" +
                                      "\r\n     ,A.銀行ＣＤ + A.支店ＣＤ AS 銀行支店ＣＤ" +
                                      "\r\n     ,A.引落日" +
                                      "\r\n     ,A.EBセット名" +
                                      "\r\n FROM" +
                                      "\r\n     部署EB設定マスタ AS A" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.銀行ＣＤ" +
                                      "\r\n     ,A.支店ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = busyoCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("部署EB設定マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    lst.BeginUpdate();      // 描画を一旦停止

                    busyoEBbindingSource.DataSource = ds;
                    busyoEBbindingSource.DataMember = "部署EB設定マスタ";

                    lst.DisplayMember = "EBセット名";
                    lst.ValueMember = "銀行支店ＣＤ";

                    log.Info("部署EB設定マスタを検索しました。");
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
                lst.EndUpdate();        // 描画再開
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 請求EBデータ入力
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>正常終了の場合はtrue</returns>
        private bool InputEBData(string filePath)
        {
            try
            {
                if (File.Exists(filePath) == false)
                {
                    MessageBox.Show(this, "ファイルが見つかりません - " + filePath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    try
                    {
                        _container = new EBContainer(_seikyuDataEBReCreate.銀行ＣＤ, reader.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        DataRow dr = ((DataRowView)busyoEBbindingSource.Current).Row;
                        string formatName = dr["EBセット"].ToString();
                        MessageBox.Show(this, string.Format("取込ファイルのフォーマットが {0} のものではありません。", formatName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                log.InfoFormat("取込ファイル:{0}", filePath);
                return true;
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
        /// EBによる入金に失敗した請求データの取得
        /// </summary>
        private void GetDataByFile()
        {
            try
            {
                List<decimal> seikyuNoList = new List<decimal>();                                   // 振替に失敗した請求番号のリスト

                // 振替結果を判定
                foreach (Dictionary<string, string> map in _container.DataList)
                {
                    string resultCD = map["振替結果コード"];
                    // 失敗？
                    if (resultCD != "0")
                    {
                        seikyuNoList.Add(decimal.Parse(map["予備"]));                               // 振替に失敗した請求番号のリストに追加
                    }
                }

                // エラーが1件も無い場合は、空のレコードを読み込むため、存在しない請求番号をリストに追加する
                if (seikyuNoList.Count == 0)
                {
                    log.Warn("失敗した入金データ無し");
                    seikyuNoList.Add((decimal)-1);
                }

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // 請求EBデータ出力に必要な項目も取得する
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     D.請求番号");
                    sql.AppendLine("     ,D.請求日");
                    sql.AppendLine("     ,D.締区分");
                    sql.AppendLine("     ,D.部署ＣＤ");
                    sql.AppendLine("     ,D.部署名");
                    sql.AppendLine("     ,D.コースＣＤ");
                    sql.AppendLine("     ,D.コース名");
                    sql.AppendLine("     ,D.請求先ＣＤ");
                    sql.AppendLine("     ,D.請求先名");
                    sql.AppendLine("     ,D.前回請求額");
                    sql.AppendLine("     ,D.入金額");
                    sql.AppendLine("     ,D.繰越額");
                    sql.AppendLine("     ,D.買上金額");
                    sql.AppendLine("     ,D.調整額");
                    sql.AppendLine("     ,D.消費税");
                    sql.AppendLine("     ,D.今回請求額");
                    sql.AppendLine("     ,NULL AS 当月入金額");
                    sql.AppendLine("     ,NULL AS 売掛残高");
                    sql.AppendLine("     ,1 AS EB出力対象");
                    sql.AppendLine("     ,D.得意先名");
                    sql.AppendLine("     ,D.銀行ＣＤ");
                    sql.AppendLine("     ,D.銀行名カナ");
                    sql.AppendLine("     ,D.支店ＣＤ");
                    sql.AppendLine("     ,D.支店名カナ");
                    sql.AppendLine("     ,D.口座種別");
                    sql.AppendLine("     ,D.口座番号");
                    sql.AppendLine("     ,D.口座名義人");
                    sql.AppendLine("     ,D.請求金額");
                    sql.AppendLine("     ,D.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         A.請求番号");
                    sql.AppendLine("         ,A.請求日");
                    sql.AppendLine("         ,A.締区分");
                    sql.AppendLine("         ,A.部署ＣＤ");
                    sql.AppendLine("         ,B.部署名");
                    sql.AppendLine("         ,C.コースＣＤ");
                    sql.AppendLine("         ,C.コース名");
                    sql.AppendLine("         ,A.請求先ＣＤ");
                    sql.AppendLine("         ,A.請求先名");
                    sql.AppendLine("         ,A.前回請求額");
                    sql.AppendLine("         ,A.入金額");
                    sql.AppendLine("         ,A.繰越額");
                    sql.AppendLine("         ,A.買上金額");
                    sql.AppendLine("         ,A.調整額");
                    sql.AppendLine("         ,A.消費税");
                    sql.AppendLine("         ,A.今回請求額");
                    sql.AppendLine("         ,T.得意先名");
                    sql.AppendLine("         ,T.金融機関ＣＤ AS 銀行ＣＤ");
                    sql.AppendLine("         ,K.銀行名カナ");
                    sql.AppendLine("         ,T.金融機関支店ＣＤ AS 支店ＣＤ");
                    sql.AppendLine("         ,KB.支店名カナ");
                    sql.AppendLine("         ,T.口座種別");
                    sql.AppendLine("         ,T.口座番号");
                    sql.AppendLine("         ,T.口座名義人");
                    sql.AppendLine("         ,A.今回請求額 AS 請求金額");
                    sql.AppendLine("         ,A.請求先ＣＤ AS 得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         請求合計 AS A");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("         INNER JOIN 金融機関名称 AS K ON K.銀行ＣＤ = T.金融機関ＣＤ");
                    sql.AppendLine("         INNER JOIN 金融機関支店名称 AS KB ON KB.銀行ＣＤ = T.金融機関ＣＤ AND KB.支店ＣＤ = T.金融機関支店ＣＤ");
                    sql.AppendLine("         LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");
                    // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.得意先ＣＤ = A.請求先ＣＤ");
                    sql.AppendLine("         INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)");
                    // -- 検索条件にコースが指定されていない場合は、ヒットした得意先が持つコースのうち最小値を取得する
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 CM.企業ＣＤ");
                    sql.AppendLine("                 ,CM.部署ＣＤ");
                    sql.AppendLine("                 ,CM.コース名");
                    sql.AppendLine("                 ,CT1.コースＣＤ");
                    sql.AppendLine("                 ,CT1.得意先ＣＤ");
                    sql.AppendLine("                 ,CT1.コース順");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 コーステーブル AS CT1");
                    sql.AppendLine("                 INNER JOIN");
                    sql.AppendLine("                     (SELECT");
                    sql.AppendLine("                         M.企業ＣＤ");
                    sql.AppendLine("                         ,M.部署ＣＤ");
                    sql.AppendLine("                         ,T.得意先ＣＤ");
                    sql.AppendLine("                         ,MIN(T.コースＣＤ) AS コースＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         コースマスタ AS M");
                    sql.AppendLine("                         LEFT JOIN コーステーブル AS T ON");
                    sql.AppendLine("                             T.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                             AND T.コースＣＤ = M.コースＣＤ");
                    sql.AppendLine("                     GROUP BY");
                    sql.AppendLine("                         M.企業ＣＤ");
                    sql.AppendLine("                         ,M.部署ＣＤ");
                    sql.AppendLine("                         ,T.得意先ＣＤ");
                    sql.AppendLine("                     ) AS CT2");
                    sql.AppendLine("                         ON CT2.企業ＣＤ = CT1.企業ＣＤ");
                    sql.AppendLine("                         AND CT2.得意先ＣＤ = CT1.得意先ＣＤ");
                    sql.AppendLine("                         AND CT2.コースＣＤ = CT1.コースＣＤ");
                    sql.AppendLine("                 INNER JOIN コースマスタ AS CM");
                    sql.AppendLine("                     ON CM.企業ＣＤ = CT1.企業ＣＤ");
                    sql.AppendLine("                     AND CM.コースＣＤ =  CT1.コースＣＤ");
                    sql.AppendLine("             ) AS C");
                    sql.AppendLine("             ON J.企業ＣＤ = C.企業ＣＤ");
                    sql.AppendLine("             AND A.部署ＣＤ = C.部署ＣＤ");     // 請求の部署と一致するものに限る
                    sql.AppendLine("             AND J.得意先ＣＤ = C.得意先ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         A.企業ＣＤ = @企業ＣＤ");

                    if (_seikyuDataEBReCreate.全部署出力フラグ != true)
                    {
                        sql.AppendLine("         AND A.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seikyuDataEBReCreateImport.部署ＣＤ;
                    }

                    sql.AppendLine("         AND A.締区分 = @締区分月締");
                    sql.AppendLine("         AND A.請求番号 IN (");

                    // 請求番号のリストから生成
                    string comma = "";
                    foreach (decimal seikyuNo in seikyuNoList)
                    {
                        sql.AppendFormat("{0}{1}", comma, seikyuNo);
                        comma = ",";
                    }

                    sql.AppendLine(")");
                    sql.AppendLine("         AND T.支払方法 = @支払方法振替");
                    sql.AppendLine("         AND T.EB銀行支店ＣＤ = @EB銀行支店ＣＤ");
                    sql.AppendLine("     ) AS D");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@締区分月締", SqlDbType.Int).Value = (int)Consts.ShimeKubun.Monthly;
                    cmd.Parameters.Add("@支払方法振替", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Furikae;
                    cmd.Parameters.Add("@EB銀行支店ＣＤ", SqlDbType.VarChar).Value = _seikyuDataEBReCreate.EB銀行支店ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    _dtFile = new DataTable();
                    adp.Fill(_dtFile);
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
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// EBデータと未回収金の検索結果をマージして表示
        /// </summary>
        private void ShowData()
        {
            try
            {
                //***************************************************************
                // EBによる入金に失敗した請求データと、未回収金の検索結果をマージ
                //***************************************************************
                DataTable dt = MergeData();
                dt.TableName = "請求EBデータ再出力";

                //*****************
                // マージ結果を表示
                //*****************
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                // 合計行を追加する
                dt.Rows.Add(dt.NewRow());

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "請求EBデータ再出力";

                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN,      "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN,       "請求日");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,     "部署名");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN,  "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,         "当月入金額");
                fpSpread1_Sheet1.BindDataColumn(URIKAKEZAN_COLUMN,     "売掛残高");
                fpSpread1_Sheet1.BindDataColumn(OUTPUT_CHK_COLUMN,     "EB出力対象");

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
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, TOKUISAKI_NAME_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = KONKAI_SEIKYU_COLUMN; columnIndex <= URIKAKEZAN_COLUMN; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    fpSpread1_Sheet1.SetActiveCell(0, SEIKYU_NO_COLUMN);
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
        /// EBによる入金に失敗した請求データと、未回収金の検索結果をマージ
        /// </summary>
        /// <returns>マージ結果のDataTbale</returns>
        private DataTable MergeData()
        {
            try
            {
                DataView dv;
                DataTable dt = new DataTable();

                //***********************************************************************************************
                // EB結果ファイルを読み込んだ＆未回収金が検索されていない場合は、EB結果ファイル読み込み結果を返す
                //***********************************************************************************************
                if (_dtFile != null && _dtMiKaisyu == null)
                {
                    // ソートして返す
                    dv = new DataView(_dtFile);
                    dv.Sort = string.Format((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");
                    dt = dv.ToTable();
                    return dt;
                }

                //*******************************************************************************
                // 未回収金を検索＆EB結果ファイルを読み込んでいない場合は、未回収金検索結果を返す
                //*******************************************************************************
                if (_dtMiKaisyu != null && _dtFile == null)
                {
                    // ソートして返す
                    dv = new DataView(_dtMiKaisyu);
                    dv.Sort = string.Format((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");
                    dt = dv.ToTable();
                    return dt;
                }

                //*************************************************************************************
                // 両方を読み込んでいる場合は、
                // 片方のみに存在するレコードは、それを表示
                // 両方に存在するレコードは、EB出力対象をONにして、それ以外は未回収金検索結果を採用する
                //*************************************************************************************

                // マージ先のデータテーブルを生成
                dt = _dtMiKaisyu.Clone();

                // 1.まず、双方の請求番号を１つのリスト(重複しない)に集める
                List<decimal> seikyuNoList = new List<decimal>();
                foreach (DataRow dr in _dtFile.Rows)
                {
                    decimal seikyuNo = (decimal)dr["請求番号"];
                    if (seikyuNoList.Contains(seikyuNo) == false)
                    {
                        seikyuNoList.Add(seikyuNo);
                    }
                }
                foreach (DataRow dr in _dtMiKaisyu.Rows)
                {
                    decimal seikyuNo = (decimal)dr["請求番号"];
                    if (seikyuNoList.Contains(seikyuNo) == false)
                    {
                        seikyuNoList.Add(seikyuNo);
                    }
                }

                // 2.リストの請求番号に該当するレコードを、双方のDataTableから取得する
                foreach (decimal seikyuNo in seikyuNoList)
                {
                    DataRow[] drsFile = _dtFile.Select(string.Format("請求番号={0}", seikyuNo));
                    DataRow[] drsMiKaisyu = _dtMiKaisyu.Select(string.Format("請求番号={0}", seikyuNo));

                    // 3.片方のみに存在する場合はそれを採用し、両方に存在する場合は１つのレコードにマージする
                    bool hasFile = (drsFile != null && drsFile.Length != 0);
                    bool hasMiKaisyu = (drsMiKaisyu != null && drsMiKaisyu.Length != 0);

                    // EB結果ファイル読み込み結果のみに存在？
                    if (hasFile == true && hasMiKaisyu == false)
                    {
                        DataRow dr = dt.NewRow();
                        dr.ItemArray = drsFile[0].ItemArray;
                        dt.Rows.Add(dr);
                    }

                    // 未回収金検索結果のみに存在？
                    if (hasFile == false && hasMiKaisyu == true)
                    {
                        DataRow dr = dt.NewRow();
                        dr.ItemArray = drsMiKaisyu[0].ItemArray;
                        dt.Rows.Add(dr);
                    }

                    // 両方に存在？
                    if (hasFile == true && hasMiKaisyu == true)
                    {
                        DataRow dr = dt.NewRow();
                        // 未回収金検索結果を採用するが、EB出力対象はONにする
                        dr.ItemArray = drsMiKaisyu[0].ItemArray;
                        dr["EB出力対象"] = 1;
                        dt.Rows.Add(dr);
                    }
                }

                // 4.改めてソート
                dv = new DataView(dt);
                dv.Sort = string.Format((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");
                dt = dv.ToTable();
                return dt;
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
        /// 口座引落EBデータが出力可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanOutput()
        {
            try
            {
                // 検索未実行？
                if (_seikyuDataEBReCreateSnapShotImport == null && _seikyuDataEBReCreateSnapShotCriteria == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if ((_seikyuDataEBReCreateSnapShotImport != null && _seikyuDataEBReCreate.Equals(_seikyuDataEBReCreateSnapShotImport) == false) ||
                    (_seikyuDataEBReCreateSnapShotCriteria != null && _seikyuDataEBReCreate.Equals(_seikyuDataEBReCreateSnapShotCriteria) == false))
                {
                    MessageBox.Show(this, "共通の条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 引落日が未入力？
                if (_seikyuDataEBReCreate.引落日 == null)
                {
                    MessageBox.Show(this, "引落日を入力してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 引落日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_seikyuDataEBReCreate.引落日 > warnDate)
                {
                    if (MessageBox.Show(this, "引落日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return false;
                    }
                }

                return true;
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
        /// EBデータ出力
        /// </summary>
        private void OutputEB()
        {
            try
            {
                // 請求データ取得
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["請求EBデータ再出力"];
                DataRow[] drs = dt.Select("EB出力対象=1");      // EB出力対象のみを抽出する
                if (drs == null || drs.Length == 0)
                {
                    log.Warn("EBデータ無し");
                    MessageBox.Show(this, "出力対象が選択されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                DataTable dtSeikyu = drs.CopyToDataTable();     // Selectで取得したDataRowの配列を別のDataTableに変換

                // 請求EBデータファイルを生成
                SeikyuEBWriter seikyuEBWriter = new SeikyuEBWriter((int)_seikyuDataEBReCreate.部署ＣＤ, _seikyuDataEBReCreate.銀行ＣＤ, _seikyuDataEBReCreate.支店ＣＤ, (DateTime)_seikyuDataEBReCreate.引落日);
                if (seikyuEBWriter.Generate(dtSeikyu) == false)
                {
                    MessageBox.Show(this, seikyuEBWriter.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // ファイル名を決定
                string fileName = string.Format("EBデータ_{0}{1:00}月再請求分({2}_{3})", lstBusyoEB.Text, DateTime.Today.Month, seikyuEBWriter.件数, seikyuEBWriter.金額);
                saveFileDialog2.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                saveFileDialog2.FileName = fileName;
                if (saveFileDialog2.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                fileName = saveFileDialog2.FileName;    // ファイルパスとして取得

                // 請求EBデータファイルを出力
                seikyuEBWriter.Output(fileName);

                // 請求合計のEB出力済みフラグを更新
                if (seikyuEBWriter.UpdateSeikyuTotal(this.Text) == false)
                {
                    MessageBox.Show(this, seikyuEBWriter.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 操作履歴に保存
                History.Save(this.Text, "請求EBデータ", null, History.ProcessType.Output);

                MessageBox.Show(this, "請求EBデータ出力が完了しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
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

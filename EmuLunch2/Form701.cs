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
    /// 得意先単価適用マスタメンテ
    /// </summary>
    public partial class Form701 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN         = 0;  // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_REFERENCE_COLUMN  = 1;  // 得意先参照列インデックス
        private const int TOKUISAKI_NAME_COLUMN       = 2;  // 得意先名列インデックス
        private const int BUSYO_NAME_COLUMN           = 3;  // 部署名列インデックス
        private const int SYOHIN_CD_COLUMN            = 4;  // 商品ＣＤ列インデックス
        private const int SYOHIN_REFERENCE_COLUMN     = 5;  // 商品参照ボタン列インデックス
        private const int SYOHIN_NAME_COLUMN          = 6;  // 商品名列インデックス
        private const int KEIGENZEIRITSU_COLUMN       = 7;  // 軽減税率列インデックス
        private const int TANKA_HYOJUN_KOMI_COLUMN    = 8;  // 標準単価税込列インデックス
        private const int TANKA_HYOJUN_NUKI_COLUMN    = 9;  // 標準単価税抜列インデックス
        private const int TANKA_TOKUISAKI_KOMI_COLUMN = 10; // 得意先単価税込列インデックス
        private const int TANKA_TOKUISAKI_NUKI_COLUMN = 11; // 得意先単価税抜列インデックス
        private const int KOTEISU_COLUMN              = 12; // 固定数列インデックス
        private const int TEKIYOU_KAISHI_COLUMN       = 13; // 適用開始日列インデックス
        private const int TEKIYOUBI_COLUMN            = 14; // 単価適用日列インデックス
        private const int LAST_CHUMON_COLUMN          = 15; // 最終注文日列インデックス
        private const int INVALID_COLUMN              = 16; // 無効フラグ列インデックス
        private const int DELETE_COLUMN               = 17; // 削除ボタン列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 得意先単価適用マスタ検索条件
        /// </summary>
        private TokuisakiTankaTekiyouCriteria _tokuisakiTankaTekiyouCriteria;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form701()
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
        private void Form701_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

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

                // 先頭3列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = 3;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "商品CD", "適用開始日", "単価適用日", "最終更新日" },
                    new string[] { "A.得意先ＣＤ {0}, A.商品ＣＤ, A.適用開始日",
                                   "A.商品ＣＤ {0}, A.得意先ＣＤ, A.適用開始日",
                                   "A.適用開始日 {0}, A.得意先ＣＤ, A.商品ＣＤ",
                                   "A.単価適用日時 {0}, A.得意先ＣＤ, A.商品ＣＤ, A.適用開始日",
                                   "A.最終更新日 {0}, A.得意先ＣＤ, A.商品ＣＤ, A.適用開始日"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _tokuisakiTankaTekiyouCriteria = new TokuisakiTankaTekiyouCriteria();

                // 部署CDコンボの初期値を設定
                _tokuisakiTankaTekiyouCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                tokuisakiTankaTekiyouBindingSource.DataSource = _tokuisakiTankaTekiyouCriteria;

                // 得意先単価適用マスタを0件取得（空のスプレッドを表示する）
                GetTokuisakiTankaTekiyou(false);
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
        private void Form701_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }

                    btnSave.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form701_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // このフォーム以外の要因で閉じられようとしている場合は閉じる
                if (e.CloseReason != CloseReason.UserClosing)
                {
                    return;
                }

                // 登録or削除済み？
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                // フォームが閉じられようとするとき、スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                if (fpSpread1.EditMode == true)
                {
                    fpSpread1.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                else
                {
                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                }

                // 初期状態と異なる？
                if (HasChange())
                {
                    DialogResult r = MessageBox.Show(this, "変更されています。更新しますか？", this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            if (CanSave() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (Save())
                            {
                                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            }
                            else
                            {
                                e.Cancel = true;
                            }
                            return;

                        case DialogResult.Cancel:
                            e.Cancel = true;
                            return;
                    }
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
                    _tokuisakiTankaTekiyouCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
                    _tokuisakiTankaTekiyouCriteria.商品ＣＤ = syohin.商品ＣＤ;       // 入力内容を確定させる
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
                    _tokuisakiTankaTekiyouCriteria.コースＣＤFrom = course.コースＣＤ;       // 入力内容を確定させる
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
                    _tokuisakiTankaTekiyouCriteria.コースＣＤTo = course.コースＣＤ;       // 入力内容を確定させる
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_tokuisakiTankaTekiyouCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tokuisakiTankaTekiyouCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tokuisakiTankaTekiyouCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 得意先単価適用マスタ取得
                GetTokuisakiTankaTekiyou(true);
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
        /// データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                TokuisakiTankaTekiyou tokuisakiTankaTekiyou = new TokuisakiTankaTekiyou(e.Row);

                // 得意先CDの場合は得意先名を取得
                if (e.Column.ColumnName == "得意先ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["得意先名"] = null;
                    e.Row["丸め処理区分"] = DBNull.Value;
                    if (tokuisakiTankaTekiyou.得意先ＣＤ != null)
                    {
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)tokuisakiTankaTekiyou.得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            e.Row["得意先名"] = tokuisaki.得意先名;
                            e.Row.SetField<int?>("丸め処理区分", tokuisaki.丸め処理区分);
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "得意先が見つかりません。");
                        }
                    }
                }

                // 商品CDの場合は商品名を取得
                if (e.Column.ColumnName == "商品ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["商品名"] = null;
                    e.Row["標準単価税込"] = DBNull.Value;
                    e.Row["標準単価税抜"] = DBNull.Value;
                    e.Row["商品単価税込"] = DBNull.Value;
                    e.Row["商品単価税抜"] = DBNull.Value;
                    e.Row["軽減税率フラグ"] = DBNull.Value;
                    e.Row["軽減税率"] = "";
                    if (tokuisakiTankaTekiyou.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)tokuisakiTankaTekiyou.商品ＣＤ);
                        if (syohin != null)
                        {
                            e.Row["商品名"] = syohin.商品名;
                            e.Row.SetField<decimal?>("標準単価税込", syohin.商品単価税込);
                            e.Row.SetField<decimal?>("標準単価税抜", syohin.商品単価税抜);
                            e.Row.SetField<bool?>("軽減税率フラグ", syohin.軽減税率フラグ);
                            if (syohin.軽減税率フラグ == true)
                            {
                                e.Row["軽減税率"] = "対象";
                            }
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

                // 単価の場合はエラーチェック
                if (e.Column.ColumnName == "商品単価税込")
                {
                    e.Row.SetColumnError(e.Column, tokuisakiTankaTekiyou["商品単価税込"]);
                    if (tokuisakiTankaTekiyou.商品単価税込 != null)
                    {
                        // 税抜き単価を算出
                        CalcTokuisakiTankaZeiNuki(e.Row);
                    }
                }

                if (e.Column.ColumnName == "商品単価税抜")
                {
                    e.Row.SetColumnError(e.Column, tokuisakiTankaTekiyou["商品単価税抜"]);
                }

                if (e.Column.ColumnName == "固定数")
                {
                    e.Row.SetColumnError(e.Column, tokuisakiTankaTekiyou["固定数"]);
                }

                if (e.Column.ColumnName == "適用開始日")
                {
                    e.Row.SetColumnError(e.Column, tokuisakiTankaTekiyou["適用開始日"]);
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
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            // F1？
            if (e.KeyCode == Keys.F1)
            {
                // F1が押された時点の列で判断する
                // （ダイアログが閉じたときに列を移動しているので、ActiveColumnIndexでは判断しない）
                int column = fpSpread1_Sheet1.ActiveColumnIndex;

                if (column == TOKUISAKI_CD_COLUMN ||
                    column == TOKUISAKI_REFERENCE_COLUMN)
                {
                    OpenForm900BySpread();          // 得意先選択ダイアログを表示
                }

                if (column == SYOHIN_CD_COLUMN ||
                    column == SYOHIN_REFERENCE_COLUMN)
                {
                    OpenForm901BySpread();          // 商品選択ダイアログを表示
                }
            }

            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread1.EditMode)
                {
                    fpSpread1.CancelCellEditing();
                }
                else
                {
                    fpSpread1.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= DELETE_COLUMN - 1)
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

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread1_Sheet1.ActiveColumnIndex)
                {
                    case TOKUISAKI_CD_COLUMN:           // 得意先ＣＤ
                    case SYOHIN_CD_COLUMN:              // 商品ＣＤ
                    case TANKA_TOKUISAKI_KOMI_COLUMN:   // 得意先単価税込
                    case TANKA_TOKUISAKI_NUKI_COLUMN:   // 得意先単価税抜
                    case KOTEISU_COLUMN:                // 固定数
                    case TEKIYOU_KAISHI_COLUMN:         // 適用開始日
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }
        }

        /// <summary>
        /// Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread1.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread1, fpSpread1_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread1.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread1Edit_KeyDown);
            }
        }

        /// <summary>
        /// Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1Edit_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合も呼ばれるので、その場合は何もしない
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread1.EditMode)
                    {
                        fpSpread1.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread1.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread1_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= DELETE_COLUMN - 1)
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
        /// Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 得意先参照ボタン？
                if (e.Column == TOKUISAKI_REFERENCE_COLUMN)
                {
                    OpenForm900BySpread();
                }

                // 商品参照ボタン？
                if (e.Column == SYOHIN_REFERENCE_COLUMN)
                {
                    OpenForm901BySpread();
                }

                // 無効チェックボックス？
                if (e.Column == INVALID_COLUMN)
                {
                    if (fpSpread1_Sheet1.Cells[e.Row, INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[e.Row].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[e.Row].ForeColor = SystemColors.GrayText;
                    }
                    else
                    {
                        fpSpread1_Sheet1.Rows[e.Row].ResetBackColor();
                        fpSpread1_Sheet1.Rows[e.Row].ResetForeColor();
                    }
                }

                // 削除ボタン？
                if (e.Column == DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                    dr.Delete();
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
        /// Spreadから得意先選択ダイアログを表示する
        /// </summary>
        private void OpenForm900BySpread()
        {
            try
            {
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuisakiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
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
        private void Form900TokuisakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, newColumnIndex);

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, TOKUISAKI_CD_COLUMN].Value = tokuisaki.得意先ＣＤ;   // 得意先CDセルに値を設定→dt_ColumnChanged()が呼ばれる
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
        /// Spreadから商品選択ダイアログを表示する
        /// </summary>
        private void OpenForm901BySpread()
        {
            try
            {
                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCDBySpread_FormClosed);
                formSyohinDlg.ShowDialog();
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
        /// Spreadから開いた商品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form901SyohinCDBySpread_FormClosed(object sender, FormClosedEventArgs e)
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
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, newColumnIndex);

                    Syohin syohin = form.SelectedSyohin;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, SYOHIN_CD_COLUMN].Value = syohin.商品ＣＤ;   // 商品CDセルに値を設定→dt_ColumnChanged()が呼ばれる
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
        /// 更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (CanSave() == false)
                {
                    return;
                }

                if (Save())
                {
                    // 再読み込み
                    Reload();
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
        /// Excelから入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileName = openFileDialog1.FileName;
                    Import(fileName);
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
        /// 得意先単価適用マスタ取得
        /// </summary>
        /// <param name="useCriteria">検索条件を使用する場合はtrue</param>
        private void GetTokuisakiTankaTekiyou(bool useCriteria)
        {
            try
            {
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
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,A.得意先ＣＤ AS 得意先ＣＤorg");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,T.丸め処理区分");
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,A.商品ＣＤ AS 商品ＣＤorg");
                    sql.AppendLine("     ,S.商品名");
                    sql.AppendLine("     ,ISNULL(S.軽減税率フラグ, 0) AS 軽減税率フラグ");
                    sql.AppendLine("     ,CASE WHEN ISNULL(S.軽減税率フラグ, 0) <> 0 THEN '対象' ELSE '' END AS 軽減税率");
                    sql.AppendLine("     ,S.商品単価税込 AS 標準単価税込");
                    sql.AppendLine("     ,S.商品単価税抜 AS 標準単価税抜");
                    sql.AppendLine("     ,A.適用開始日");
                    sql.AppendLine("     ,A.適用開始日 AS 適用開始日org");
                    sql.AppendLine("     ,A.商品単価税込");
                    sql.AppendLine("     ,A.商品単価税抜");
                    sql.AppendLine("     ,A.固定数");
                    sql.AppendLine("     ,A.単価適用日時");
                    sql.AppendLine("     ,A.最終注文日時");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     得意先単価適用マスタ AS A");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = T.企業ＣＤ AND B.部署ＣＤ = T.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");

                    if (useCriteria == true && (_tokuisakiTankaTekiyouCriteria.コースＣＤFrom != null || _tokuisakiTankaTekiyouCriteria.コースＣＤTo != null))
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
                        if (_tokuisakiTankaTekiyouCriteria.コースＣＤFrom != null)
                        {
                            sql.AppendLine("             AND CT.コースＣＤ >= @コースＣＤFrom");
                            cmd.Parameters.Add("@コースＣＤFrom", SqlDbType.Int).Value = _tokuisakiTankaTekiyouCriteria.コースＣＤFrom;
                        }
                        if (_tokuisakiTankaTekiyouCriteria.コースＣＤTo != null)
                        {
                            sql.AppendLine("             AND CT.コースＣＤ <= @コースＣＤTo");
                            cmd.Parameters.Add("@コースＣＤTo", SqlDbType.Int).Value = _tokuisakiTankaTekiyouCriteria.コースＣＤTo;
                        }
                        sql.AppendLine("         ) AS C");
                        sql.AppendLine("         ON A.企業ＣＤ = C.企業ＣＤ");
                        sql.AppendLine("         AND A.得意先ＣＤ = C.得意先ＣＤ");
                    }

                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (useCriteria == false)
                    {
                        // 検索条件を使わない場合は空のレコードを取得する
                        sql.AppendLine("     AND A.得意先ＣＤ = @得意先ＣＤ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = -1;
                    }
                    else
                    {
                        if (_tokuisakiTankaTekiyouCriteria.部署ＣＤ != null && _tokuisakiTankaTekiyouCriteria.部署ＣＤ != -1)
                        {
                            sql.AppendLine("     AND T.部署ＣＤ = @部署ＣＤ");
                            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tokuisakiTankaTekiyouCriteria.部署ＣＤ;
                        }

                        if (_tokuisakiTankaTekiyouCriteria.得意先ＣＤ != null)
                        {
                            sql.AppendLine("     AND A.得意先ＣＤ = @得意先ＣＤ");
                            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _tokuisakiTankaTekiyouCriteria.得意先ＣＤ;
                        }

                        if (_tokuisakiTankaTekiyouCriteria.商品ＣＤ != null)
                        {
                            sql.AppendLine("    AND A.商品ＣＤ = @商品ＣＤ");
                            cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _tokuisakiTankaTekiyouCriteria.商品ＣＤ;
                        }

                        if (_tokuisakiTankaTekiyouCriteria.適用日From != null)
                        {
                            sql.AppendLine("    AND A.適用開始日  >= @適用日From");
                            cmd.Parameters.Add("@適用日From", SqlDbType.Date).Value = _tokuisakiTankaTekiyouCriteria.適用日From;
                        }

                        if (_tokuisakiTankaTekiyouCriteria.適用日To != null)
                        {
                            sql.AppendLine("    AND A.適用開始日  <= @適用日To");
                            cmd.Parameters.Add("@適用日To", SqlDbType.Date).Value = _tokuisakiTankaTekiyouCriteria.適用日To;
                        }

                        if (_tokuisakiTankaTekiyouCriteria.固定注文 != null && _tokuisakiTankaTekiyouCriteria.固定注文 == true)
                        {
                            sql.AppendLine("    AND ISNULL(A.固定数, 0) <> 0 ");
                        }
                    }

                    sql.AppendLine(" ORDER BY");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("得意先単価適用マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "得意先単価適用マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("得意先単価適用マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,         "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN,       "得意先名");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,           "部署名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,            "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN,          "商品名");
                fpSpread1_Sheet1.BindDataColumn(KEIGENZEIRITSU_COLUMN,       "軽減税率");
                fpSpread1_Sheet1.BindDataColumn(TANKA_HYOJUN_KOMI_COLUMN,    "標準単価税込");
                fpSpread1_Sheet1.BindDataColumn(TANKA_HYOJUN_NUKI_COLUMN,    "標準単価税抜");
                fpSpread1_Sheet1.BindDataColumn(TANKA_TOKUISAKI_KOMI_COLUMN, "商品単価税込");
                fpSpread1_Sheet1.BindDataColumn(TANKA_TOKUISAKI_NUKI_COLUMN, "商品単価税抜");
                fpSpread1_Sheet1.BindDataColumn(KOTEISU_COLUMN,              "固定数");
                fpSpread1_Sheet1.BindDataColumn(TEKIYOU_KAISHI_COLUMN,       "適用開始日");
                fpSpread1_Sheet1.BindDataColumn(TEKIYOUBI_COLUMN,            "単価適用日時");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN,              "無効フラグ");
                fpSpread1_Sheet1.BindDataColumn(LAST_CHUMON_COLUMN,          "最終注文日時");

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
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
        /// 得意先単価マスタの税抜き単価を算出
        /// </summary>
        /// <param name="dr">得意先単価マスタレコード</param>
        private void CalcTokuisakiTankaZeiNuki(DataRow dr)
        {
            try
            {
                // 消費税の丸め区分を決定
                int? marumeKubun = dr["丸め処理区分"] as int?;
                if (marumeKubun == null || marumeKubun < 0)
                {
                    marumeKubun = (int)Rounding.HasuKubun.Floor;    // 初期値は切り捨て
                }

                // 単価の丸め区分を決定（消費税を切り捨てたら、単価は切り上げる）
                int hasuKubun = (int)Rounding.HasuKubun.Round;
                switch (marumeKubun)
                {
                    case (int)Rounding.HasuKubun.Ceiling:
                        hasuKubun = (int)Rounding.HasuKubun.Floor;
                        break;

                    case (int)Rounding.HasuKubun.Floor:
                        hasuKubun = (int)Rounding.HasuKubun.Ceiling;
                        break;
                }

                // 適用開始日（未入力ならシステム日付）時点の消費税率を取得
                Syohizei syohizei = Syohizei.GetSyohizei((dr["適用開始日"] as DateTime?) ?? DateTime.Today);

                decimal taxRate = syohizei.GetTaxRate(dr["軽減税率フラグ"] as bool?);
                decimal? price = dr["商品単価税込"] as decimal?;
                if (price == null)
                {
                    dr["商品単価税抜"] = DBNull.Value;
                    return;
                }
                price = price / (1m + taxRate);
                price = Rounding.Round((decimal)price, hasuKubun);
                dr["商品単価税抜"] = price;
                dr.SetColumnError("商品単価税抜", null);   // 税抜き単価のエラーをクリア
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
        /// 得意先単価適用マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 得意先単価適用マスタを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["得意先単価適用マスタ"];

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                    case DataRowState.Modified:     // 変更
                    case DataRowState.Deleted:      // 削除
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSave()
        {
            bool hasError = false;

            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["得意先単価適用マスタ"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    TokuisakiTankaTekiyou tokuisakiTankaTekiyou = new TokuisakiTankaTekiyou(dr);

                    // エラーがある？
                    if (tokuisakiTankaTekiyou.HasErrors)
                    {
                        dr.SetColumnError("得意先ＣＤ", tokuisakiTankaTekiyou["得意先ＣＤ"]);
                        dr.SetColumnError("商品ＣＤ", tokuisakiTankaTekiyou["商品ＣＤ"]);
                        dr.SetColumnError("商品単価税込", tokuisakiTankaTekiyou["商品単価税込"]);
                        dr.SetColumnError("商品単価税抜", tokuisakiTankaTekiyou["商品単価税抜"]);
                        dr.SetColumnError("適用開始日", tokuisakiTankaTekiyou["適用開始日"]);
                        hasError = true;
                    }
                    else
                    {
                        // 得意先ＣＤに該当するマスタが存在するか？
                        if (tokuisakiTankaTekiyou.得意先ＣＤ != null)
                        {
                            Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)tokuisakiTankaTekiyou.得意先ＣＤ);
                            if (tokuisaki == null)
                            {
                                dr.SetColumnError("得意先ＣＤ", "得意先が見つかりません。");
                                hasError = true;
                            }
                        }

                        // 商品ＣＤに該当するマスタが存在するか？
                        if (tokuisakiTankaTekiyou.商品ＣＤ != null)
                        {
                            Syohin syohin = Syohin.GetSyohin((int)tokuisakiTankaTekiyou.商品ＣＤ);
                            if (syohin == null)
                            {
                                dr.SetColumnError("商品ＣＤ", "商品が見つかりません。");
                                hasError = true;
                            }
                        }
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
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
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        DataTable dt = ds.Tables["得意先単価適用マスタ"];

                        foreach (DataRow dr in dt.Rows)
                        {
                            TokuisakiTankaTekiyou tokuisakiTankaTekiyou = new TokuisakiTankaTekiyou(dr);
                            switch (dr.RowState)
                            {
                                case DataRowState.Added:        // 追加
                                    historyProcess = History.ProcessType.Create;
                                    ret = tokuisakiTankaTekiyou.Insert(con, tran);
                                    break;

                                case DataRowState.Modified:     // 変更
                                    historyProcess = History.ProcessType.Update;
                                    tokuisakiTankaTekiyou.単価適用日時 = null;      // 単価未適用とする
                                    ret = tokuisakiTankaTekiyou.Update(con, tran);
                                    break;

                                case DataRowState.Deleted:      // 削除
                                    historyProcess = History.ProcessType.Delete;
                                    ret = tokuisakiTankaTekiyou.Delete(con, tran);
                                    break;

                                default:
                                    continue;
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, tokuisakiTankaTekiyou.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            History.Save(con, tran, this.Text, "得意先単価適用マスタ", tokuisakiTankaTekiyou.Code, historyProcess);
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("得意先単価適用マスタを{0}しました。", historyProcess.DisplayName());
                        MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (EmuException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        log.Error(ex);
                        History.Error(ex);
                        throw new EmuException(ex);
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            btnQuery.PerformClick();        // 検索ボタン押下
        }

        /// <summary>
        /// Excelからインポート
        /// </summary>
        /// <param name="fileName">Excelファイル名</param>
        private void Import(string fileName)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 開始ログを出力
                string msg = string.Format("{0}でExcel取り込みを開始しました。 {1}", this.Text, fileName);
                History.Save(msg);

                object[,] table = null;

                // Excel読み込み
                using (ExcelManager excel = new ExcelManager())
                {
                    excel.Open(fileName);
                    table = excel.GetValue();
                    excel.Quit();
                }

                if (table == null)
                {
                    MessageBox.Show(this, "何も入力されていないExcelを取り込んだか、取り込みに失敗しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                int rowStart = table.GetLowerBound(0);  // 行開始インデックス
                int rowEnd = table.GetUpperBound(0);    // 行終了インデックス
                int colStart = table.GetLowerBound(1);  // 列開始インデックス
                int colEnd = table.GetUpperBound(1);    // 列終了インデックス

                // 列名をキーとして、そのインデックスを値に持つマップ
                Dictionary<string, int> columnIndexMap = new Dictionary<string, int>();

                // 取得対象の列の名前（Spreadの列ヘッダ（=Excel出力の際にも適用）の表記揺れも考慮する）(★)
                string[,] targetColumnNames = { {"得意先ＣＤ", "得意先CD"},
                                                {"商品ＣＤ", "商品CD"},
                                                {"適用開始日", "適用開始"},
                                                {"商品単価税抜", "得意先単価税抜"},
                                                {"商品単価税込", "得意先単価税込"},
                                                {"固定数", "固定数"} };

                // 得意先単価適用マスタリスト
                List<TokuisakiTankaTekiyou> tokuisakiTankaTekiyouList = new List<TokuisakiTankaTekiyou>();
                int failCount = 0;

                for (int rowIndex = rowStart; rowIndex <= rowEnd; rowIndex++)
                {
                    // 全ての取得対象列のインデックスを取得済み？
                    if (columnIndexMap.Count >= targetColumnNames.GetLength(0))
                    {
                        // 取得対象列の検証結果を取得
                        TokuisakiTankaTekiyou tokuisakiTankaTekiyou = ParseTokuisakiTankaTekiyou(table, rowIndex, columnIndexMap);
                        if (tokuisakiTankaTekiyou != null)
                        {
                            tokuisakiTankaTekiyouList.Add(tokuisakiTankaTekiyou);
                        }
                        else
                        {
                            failCount++;
                        }
                    }
                    else
                    {
                        // １行目から列インデックスを取得
                        if (rowIndex == rowStart)
                        {
                            // 取得対象列のインデックスを取得
                            for (int colIndex = colStart; colIndex <= colEnd; colIndex++)
                            {
                                GetColumnIndex(table, rowIndex, colIndex, targetColumnNames, columnIndexMap);
                            }
                        }

                        // ２行目なのに全ての列インデックスが取得できていなければ、列ヘッダが２行に渡っているので、２行目からも取得する
                        if (rowIndex == rowStart + 1)
                        {
                            if (columnIndexMap.Count != targetColumnNames.GetLength(0))
                            {
                                for (int colIndex = colStart; colIndex <= colEnd; colIndex++)
                                {
                                    // ２行に跨がっている列名を２行目に作る
                                    GenerateColumnName(table, rowIndex, colIndex);

                                    // 取得対象列のインデックスを取得
                                    GetColumnIndex(table, rowIndex, colIndex, targetColumnNames, columnIndexMap);
                                }
                            }
                        }

                        // ３行目以降（＝２行目完了後）なのに全ての列インデックスが取得できていなければ、取得対象列が不足している
                        if (rowIndex > rowStart + 1)
                        {
                            msg = string.Format("取り込もうとしているファイル" +
                                                "\n{0}" +
                                                "\nに列が不足しています。以下の列が全て揃っているか確認してください。" +
                                                "\n・得意先CD" +
                                                "\n・商品CD" +
                                                "\n・適用開始日" +
                                                "\n・商品単価税抜（得意先単価税抜）" +
                                                "\n・商品単価税込（得意先単価税込）" +
                                                "\n・固定数" +
                                                "\n・単価適用日",
                                                "\n\n取り込みはできませんでした。",
                                                fileName);
                            MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }

                // DB登録後、成功した件数と失敗した件数を表示する
                if (SaveByImport(tokuisakiTankaTekiyouList))
                {
                    string failMsg = "";
                    if (failCount != 0)
                    {
                        failMsg = "\n\n失敗の詳細は操作履歴一覧を参照してください。";
                    }
                    MessageBox.Show(this, string.Format("取込が完了しました。\n成功：{0}件\n失敗：{1}件{2}",tokuisakiTankaTekiyouList.Count, failCount, failMsg),
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 取得対象列のインデックスを取得
        /// </summary>
        /// <param name="table">Excelから取得した二次元配列</param>
        /// <param name="rowIndex">二次元配列のカレント行</param>
        /// <param name="colIndex">二次元配列のカレント列</param>
        /// <param name="targetColumnNames">取得対象の列の名前をが格納された二次元配列</param>
        /// <param name="columnIndexMap">列名をキーとして、そのインデックスを値に持つマップ</param>
        private void GetColumnIndex(object[,] table, int rowIndex, int colIndex, string[,] targetColumnNames, Dictionary<string, int> columnIndexMap)
        {
            try
            {
                // カレント行・列の値を取得
                object value = table[rowIndex, colIndex];
                if (value == null)
                {
                    value = "";
                }
                string columnName = value.ToString();

                // 取得対象の列名？
                if (ArrayContains(targetColumnNames, columnName))
                {
                    columnIndexMap[columnName] = colIndex;
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
        /// ２行に跨がっている列名を２行目に作る
        /// </summary>
        /// <param name="table">Excelから取得した二次元配列</param>
        /// <param name="rowIndex">二次元配列のカレント行（２行目）</param>
        /// <param name="colIndex">二次元配列のカレント列</param>
        private void GenerateColumnName(object[,] table, int rowIndex, int colIndex)
        {
            try
            {
                //***********************************
                // Excelの見た目はこうなっていても…
                // ┌─────┐
                // │得意先単価│
                // ├──┬──┤
                // │税込│税抜│
                // └──┴──┘
                //
                // 実際のセル内容はこうなっている。
                // ┌─────┬──┐
                // │得意先単価│    │
                // ├─────┼──┤
                // │税込      │税抜│
                // └─────┴──┘
                //
                // これをまずこのようにして…
                // ┌─────┬─────┐
                // │得意先単価│得意先単価│
                // ├─────┼─────┤
                // │税込      │税抜      │
                // └─────┴─────┘
                //
                // 最終的にこうする。
                // ┌───────┬───────┐
                // │得意先単価    │得意先単価    │
                // ├───────┼───────┤
                // │得意先単価税込│得意先単価税抜│
                // └───────┴───────┘
                //***********************************

                // まず直前の行の要素がブランクであれば、そのすぐ左の要素で埋める
                object value = table[rowIndex - 1, colIndex];
                if (value == null)
                {
                    value = "";
                }
                string above = value.ToString();

                if (string.IsNullOrEmpty(above))
                {
                    table[rowIndex - 1, colIndex] = table[rowIndex - 1, colIndex - 1];
                    above = table[rowIndex - 1, colIndex].ToString();
                }

                // 直前の行の要素と結合した内容を、自身の要素の内容とする
                value = table[rowIndex, colIndex];
                if (value == null)
                {
                    value = "";
                }
                table[rowIndex, colIndex] = above + value.ToString();
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
        /// 指定した文字列が、二次元配列に含まれているかどうかを返します
        /// </summary>
        /// <param name="targetColumnNames">検索対象の二次元配列</param>
        /// <param name="value">検索対象文字列</param>
        /// <returns>含まれている場合は true</returns>
        private bool ArrayContains(string[,] targetColumnNames, string value)
        {
            bool ret = false;
            foreach (string s in targetColumnNames)
            {
                if (s == value)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 取得対象列の値を検証して TokuisakiTankaTekiyou インスタンスを返す
        /// </summary>
        /// <param name="table">Excelから取得した二次元配列</param>
        /// <param name="rowIndex">二次元配列のカレント行</param>
        /// <param name="columnIndexMap">列名をキーとして、そのインデックスを値に持つマップ</param>
        /// <returns>rowIndex行の、取得対象列の値がすべて有効の場合は 、その値を格納した TokuisakiTankaTekiyouインスタンス。ひとつでも無効な場合は null</returns>
        private TokuisakiTankaTekiyou ParseTokuisakiTankaTekiyou(object[,] table, int rowIndex, Dictionary<string, int> columnIndexMap)
        {
            TokuisakiTankaTekiyou tokuisakiTankaTekiyou = new TokuisakiTankaTekiyou();
            List<string> nullColumns = new List<string>();      // 値がnullの列名リスト
            List<string> invalidColumns = new List<string>();   // 値が無効の列名リスト

            try
            {
                // マップから列名と列インデックスを取得
                foreach (string columnName in columnIndexMap.Keys)
                {
                    int columnIndex = columnIndexMap[columnName];
                    object v = table[rowIndex, columnIndex];
                    string value = (v == null ? "" : v.ToString());

                    // 固定数以外はNull不可
                    if (columnName != "固定数")
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            nullColumns.Add(columnName);
                            continue;
                        }
                    }

                    // (★)
                    switch (columnName)
                    {
                        case "得意先ＣＤ":
                        case "得意先CD":
                            int tokuisakiCD;
                            if (int.TryParse(value, out tokuisakiCD) == false)
                            {
                                invalidColumns.Add(columnName);
                            }
                            else
                            {
                                // 得意先CDに該当するマスタが存在するか
                                Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki(tokuisakiCD);
                                if (tokuisaki == null)
                                {
                                    if (invalidColumns.Contains(columnName) == false)
                                    {
                                        invalidColumns.Add(columnName);
                                    }
                                }
                                else
                                {
                                    tokuisakiTankaTekiyou.得意先ＣＤ = tokuisakiCD;
                                }
                            }
                            break;

                        case "商品ＣＤ":
                        case "商品CD":
                            int syohinCD;
                            if (int.TryParse(value, out syohinCD) == false)
                            {
                                invalidColumns.Add(columnName);
                            }
                            else
                            {
                                // 商品CDに該当するマスタが存在するか
                                Syohin syohin = Syohin.GetSyohin(syohinCD);
                                if (syohin == null)
                                {
                                    if (invalidColumns.Contains(columnName) == false)
                                    {
                                        invalidColumns.Add(columnName);
                                    }
                                }
                                else
                                {
                                    tokuisakiTankaTekiyou.商品ＣＤ = syohinCD;
                                }
                            }
                            break;

                        case "適用開始日":
                        case "適用開始":
                            DateTime tekiyoKaishiBi;
                            if (DateTime.TryParse(value, out tekiyoKaishiBi) == false)
                            {
                                invalidColumns.Add(columnName);
                            }
                            else
                            {
                                tokuisakiTankaTekiyou.適用開始日 = tekiyoKaishiBi;
                            }
                            break;

                        case "商品単価税抜":
                        case "得意先単価税抜":
                            decimal tanka;
                            if (decimal.TryParse(value, out tanka) == false)
                            {
                                invalidColumns.Add(columnName);
                            }
                            else
                            {
                                // 小数はエラー
                                if (tanka != decimal.Truncate(tanka))
                                {
                                    if (invalidColumns.Contains(columnName) == false)
                                    {
                                        invalidColumns.Add(columnName);
                                    }
                                }
                                else
                                {
                                    tokuisakiTankaTekiyou.商品単価税抜 = tanka;
                                }
                            }
                            break;

                        case "商品単価税込":
                        case "得意先単価税込":
                            decimal tankaKomi;
                            if (decimal.TryParse(value, out tankaKomi) == false)
                            {
                                invalidColumns.Add(columnName);
                            }
                            else
                            {
                                // 小数はエラー
                                if (tankaKomi != decimal.Truncate(tankaKomi))
                                {
                                    if (invalidColumns.Contains(columnName) == false)
                                    {
                                        invalidColumns.Add(columnName);
                                    }
                                }
                                else
                                {
                                    tokuisakiTankaTekiyou.商品単価税込 = tankaKomi;
                                }
                            }
                            break;

                        case "固定数":
                            decimal koteisu;
                            // 固定数はnull可（なので入力されている場合のみ検証する）
                            if (string.IsNullOrEmpty(value) == false)
                            {
                                if (decimal.TryParse(value, out koteisu) == false)
                                {
                                    invalidColumns.Add(columnName);
                                }
                                else
                                {
                                    // 小数はエラー
                                    if (koteisu != decimal.Truncate(koteisu))
                                    {
                                        if (invalidColumns.Contains(columnName) == false)
                                        {
                                            invalidColumns.Add(columnName);
                                        }
                                    }
                                    else
                                    {
                                        // マイナスは不可
                                        if (koteisu < 0)
                                        {
                                            if (invalidColumns.Contains(columnName) == false)
                                            {
                                                invalidColumns.Add(columnName);
                                            }
                                        }
                                        else
                                        {
                                            tokuisakiTankaTekiyou.固定数 = koteisu;
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }

                // 値に不備がある場合、その内容を操作履歴に出力
                if (nullColumns.Count != 0)
                {
                    tokuisakiTankaTekiyou = null;   // 検証結果
                    string columns = string.Join(",", nullColumns);
                    string msg = string.Format("{0}でExcelから取り込みましたが {1}行目の次の項目が未入力です。{2}",this.Text, rowIndex, columns);
                    History.Save(msg);
                }

                if (invalidColumns.Count != 0)
                {
                    tokuisakiTankaTekiyou = null;   // 検証結果
                    string columns = string.Join(",", invalidColumns);
                    string msg = string.Format("{0}でExcelから取り込みましたが {1}行目の次の項目に不備があります。{2}", this.Text, rowIndex, columns);
                    History.Save(msg);
                }

                return tokuisakiTankaTekiyou;
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
        /// DB保存（インポート）
        /// </summary>
        /// <param name="tokuisakiTankaTekiyouList">得意先単価適用マスタリスト</param>
        /// <returns>成功した場合はtrue</returns>
        private bool SaveByImport(List<TokuisakiTankaTekiyou> tokuisakiTankaTekiyouList)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        foreach (TokuisakiTankaTekiyou tokuisakiTankaTekiyou in tokuisakiTankaTekiyouList)
                        {
                            // 同じキーを持つレコードが存在する場合は更新。無ければ追加
                            TokuisakiTankaTekiyou target = TokuisakiTankaTekiyou.GetTokuisakiTankaTekiyou(con, tran,
                                                                                                          (int)tokuisakiTankaTekiyou.得意先ＣＤ,
                                                                                                          (int)tokuisakiTankaTekiyou.商品ＣＤ,
                                                                                                          (DateTime)tokuisakiTankaTekiyou.適用開始日);
                            if (target != null)
                            {
                                target.商品単価税抜 = tokuisakiTankaTekiyou.商品単価税抜;
                                target.商品単価税込 = tokuisakiTankaTekiyou.商品単価税込;
                                target.固定数 = tokuisakiTankaTekiyou.固定数;
                                target.無効フラグ = false;
                                ret = target.Update(con, tran);
                            }
                            else
                            {
                                historyProcess = History.ProcessType.Create;
                                ret = tokuisakiTankaTekiyou.Insert(con, tran);
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, tokuisakiTankaTekiyou.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            History.Save(con, tran, this.Text, "得意先単価適用マスタ", tokuisakiTankaTekiyou.Code, historyProcess);
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("得意先単価適用マスタを{0}しました。", historyProcess.DisplayName());
                        return true;
                    }
                    catch (EmuException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        log.Error(ex);
                        History.Error(ex);
                        throw new EmuException(ex);
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
        #endregion
    }
}

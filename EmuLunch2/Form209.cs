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
    /// 曜日別固定数注文設定
    /// </summary>
    public partial class Form209 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN         = 0;  // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_REFERENCE_COLUMN  = 1;  // 得意先参照列インデックス
        private const int TOKUISAKI_NAME_COLUMN       = 2;  // 得意先名列インデックス
        private const int SYOHIN_CD_COLUMN            = 3;  // 商品ＣＤ列インデックス
        private const int SYOHIN_REFERENCE_COLUMN     = 4;  // 商品参照ボタン列インデックス
        private const int SYOHIN_NAME_COLUMN          = 5;  // 商品名列インデックス
        private const int TANKA_TOKUISAKI_KOMI_COLUMN = 6;  // 得意先単価税込列インデックス
        private const int SUNDAY_COLUMN               = 7;  // 注文数_日列インデックス
        private const int MONDAY_COLUMN               = 8;  // 注文数_月列インデックス
        private const int TUESDAY_COLUMN              = 9;  // 注文数_火列インデックス
        private const int WEDNESDAY_COLUMN            = 10; // 注文数_水列インデックス
        private const int THURSDAY_COLUMN             = 11; // 注文数_木列インデックス
        private const int FRIDAY_COLUMN               = 12; // 注文数_金列インデックス
        private const int SATURDAY_COLUMN             = 13; // 注文数_土列インデックス
        private const int HOLIDAY_COLUMN              = 14; // 注文数_祝列インデックス
        private const int DELETE_COLUMN               = 15; // 削除ボタン列インデックス

        /// <summary>
        /// 得意先の休日の背景色
        /// </summary>
        private readonly Color HOLIDAY_BACK_COLOR = Color.LightPink;

        /// <summary>
        /// 注文数が無い場合の得意先の背景色
        /// </summary>
        private readonly Color NO_CHUMON_BACK_COLOR = Color.LightYellow;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 曜日別注文設定マスタ検索条件
        /// </summary>
        private YoubiChumonSetteiCriteria _youbiChumonSetteiCriteria;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// 得意先CDをキーとして、曜日有効フラグを値に持つマップ
        /// </summary>
        private Dictionary<int, YoubiChumonFlags> _youbiChumonFlagsMap;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form209()
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
        private void Form209_Load(object sender, EventArgs e)
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

                // クリップボードからの貼り付けを値のみにする（書式を含めない）
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.V, Keys.Control), FarPoint.Win.Spread.SpreadActions.ClipboardPasteValues);

                im = fpSpread1.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 先頭3列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = 3;

                //***********************************************************************************
                // Spreadの行のロック(編集不可)を有効にするためにシートのプロテクト(保護)を有効にする
                // ただし、Excel出力するとシートのあらゆる書式や設定が変更できなくなるので、
                // Excel出力の前後でプロテクト状態を切り替える(□)
                //***********************************************************************************
                fpSpread1_Sheet1.Protect = true;

                // Static または Readonly の列のセルをロックする(□)
                LockCellsStaticOrReadonly(fpSpread1_Sheet1);

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "商品CD", "適用開始日" },
                    new string[] { "A.得意先ＣＤ {0}, A.商品ＣＤ",
                                   "A.商品ＣＤ {0}, A.得意先ＣＤ"});

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _youbiChumonSetteiCriteria = new YoubiChumonSetteiCriteria();

                // 部署CDコンボの初期値を設定
                _youbiChumonSetteiCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                youbiChumonSetteiCriteriaBindingSource.DataSource = _youbiChumonSetteiCriteria;

                // 曜日別注文設定マスタ取得
                GetYoubiChumonSettei();
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
        private void Form209_KeyDown(object sender, KeyEventArgs e)
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
        private void Form209_FormClosing(object sender, FormClosingEventArgs e)
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
                    _youbiChumonSetteiCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
                    _youbiChumonSetteiCriteria.商品ＣＤ = syohin.商品ＣＤ;       // 入力内容を確定させる
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
                    _youbiChumonSetteiCriteria.コースＣＤ= course.コースＣＤ;       // 入力内容を確定させる
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_youbiChumonSetteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_youbiChumonSetteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _youbiChumonSetteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 曜日別注文設定マスタ取得
                GetYoubiChumonSettei();
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
        /// Spread クリップボードから貼り付けたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ClipboardPasted(object sender, FarPoint.Win.Spread.ClipboardPastedEventArgs e)
        {
            // クリップボードの中身が複数行または複数列ではない（つまり改行もタブも無い）場合は何もしない
            // ※改行(=複数行)またはタブ(=複数列)があれば、注文曜日のセルの見え方を制御する
            IDataObject iData = Clipboard.GetDataObject();
            if (iData != null)
            {
                if (iData.GetDataPresent(typeof(string)))
                {
                    string s = (string)iData.GetData(DataFormats.StringFormat);
                    if (s.Contains("\n") || s.Contains("\r") || s.Contains("\t"))
                    {
                        SetYoubiCellColor();        // 注文曜日のセルの見え方を制御
                    }
                }
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
                fpSpread1_Sheet1.AddSelection(fpSpread1_Sheet1.ActiveRowIndex, fpSpread1_Sheet1.ActiveColumnIndex, 1, 1);
                fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                // 選択範囲のロックされていないセルをまとめてクリアする(□)
                ClearUnlockedCells(fpSpread1_Sheet1);               // dt_ColumnChangedが発生

                // 選択範囲が複数？
                if (fpSpread1_Sheet1.GetSelections().Length > 1)
                {
                    SetYoubiCellColor();        // 注文曜日のセルの見え方を制御
                }
                else
                {
                    // 選択範囲が１つだけの場合は、複数セルが選択されているか？
                    FarPoint.Win.Spread.Model.CellRange cellRange = fpSpread1_Sheet1.GetSelection(0);
                    if (cellRange.ColumnCount > 1 || cellRange.RowCount > 1)
                    {
                        SetYoubiCellColor();        // 注文曜日のセルの見え方を制御
                    }
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
        /// 注文作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChumon_Click(object sender, EventArgs e)
        {
            try
            {
                if (HasChange())
                {
                    MessageBox.Show(this, "変更されています。更新しないと注文の作成はできません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowHanei();        // 曜日別固定数注文反映画面を表示
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
        /// Excelから取込ボタン
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
                fpSpread1_Sheet1.Protect = false;       // シートのプロテクトを解除する(□)

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
            finally
            {
                fpSpread1_Sheet1.Protect = true;        // シートのプロテクトを有効にする(□)
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
        /// 曜日別注文設定マスタ取得
        /// </summary>
        private void GetYoubiChumonSettei()
        {
            try
            {
                DataTable dt = new DataTable("曜日別注文設定マスタ");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    // 曜日別注文設定マスタと曜日ごとの有効フラグを同時に取得する
                    sql.Append("\r\n SELECT" +
                               "\r\n     A.企業ＣＤ" +
                               "\r\n     ,A.得意先ＣＤ" +
                               "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                               "\r\n     ,T.得意先名" +
                               "\r\n     ,A.商品ＣＤ" +
                               "\r\n     ,A.商品ＣＤ AS 商品ＣＤorg" +
                               "\r\n     ,S.商品名" +
                               "\r\n     ,ISNULL(K.商品単価税抜, S.商品単価税抜) AS 商品単価税抜" +
                               "\r\n     ,ISNULL(K.商品単価税込, S.商品単価税込) AS 商品単価税込" +
                               "\r\n     ,S.軽減税率フラグ" +
                               "\r\n     ,CASE WHEN K.商品単価税込 IS NULL THEN 1 ELSE 0 END AS 単価淡色フラグ" +
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
                               "\r\n     ,WT.得意先ＣＤ AS WEB得意先ＣＤ" +
                               "\r\n FROM" +
                               "\r\n     曜日別注文設定マスタ AS A" +
                               "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ" +
                               "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                               "\r\n     LEFT JOIN 得意先単価マスタ AS K ON K.企業ＣＤ = A.企業ＣＤ AND K.得意先ＣＤ = A.得意先ＣＤ AND K.商品ＣＤ = A.商品ＣＤ AND ISNULL(K.無効フラグ, 0) = 0" +
                               "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                               "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                               "\r\n     LEFT JOIN WEB_M_得意先 AS WT ON WT.企業ＣＤ = A.企業ＣＤ AND WT.得意先ＣＤ = A.得意先ＣＤ AND ISNULL(WT.無効フラグ, 0) = 0" +
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

                    if (_youbiChumonSetteiCriteria.コースＣＤ != null)
                    {
                        // -- コースによる絞り込み用
                        // -- 同じ得意先が複数のコースが設定されている場合、そのいずれのコースを条件に指定しても、そのコースにぶら下がる得意先がヒットしないといけない。
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
                                   "\r\n             CT.コースＣＤ = @コースＣＤ" +
                                   "\r\n         ) AS C" +
                                   "\r\n         ON A.企業ＣＤ = C.企業ＣＤ" +
                                   "\r\n         AND A.得意先ＣＤ = C.得意先ＣＤ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _youbiChumonSetteiCriteria.コースＣＤ;
                    }

                    sql.Append("\r\n WHERE" +
                               "\r\n     A.企業ＣＤ = @企業ＣＤ");

                    if (_youbiChumonSetteiCriteria.部署ＣＤ != null && _youbiChumonSetteiCriteria.部署ＣＤ != -1)
                    {
                        sql.Append("\r\n     AND T.部署ＣＤ = @部署ＣＤ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _youbiChumonSetteiCriteria.部署ＣＤ;
                    }

                    if (_youbiChumonSetteiCriteria.得意先ＣＤ != null)
                    {
                        sql.Append("\r\n     AND A.得意先ＣＤ = @得意先ＣＤ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _youbiChumonSetteiCriteria.得意先ＣＤ;
                    }

                    if (_youbiChumonSetteiCriteria.商品ＣＤ != null)
                    {
                        sql.Append("\r\n     AND A.商品ＣＤ = @商品ＣＤ");
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _youbiChumonSetteiCriteria.商品ＣＤ;
                    }

                    sql.Append("\r\n ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コース区分平日", SqlDbType.Int).Value = (int)Consts.CourseKubun.Weekday;
                    cmd.Parameters.Add("@コース区分土曜", SqlDbType.Int).Value = (int)Consts.CourseKubun.Saturday;
                    cmd.Parameters.Add("@コース区分日曜", SqlDbType.Int).Value = (int)Consts.CourseKubun.Sunday;
                    cmd.Parameters.Add("@コース区分祝日", SqlDbType.Int).Value = (int)Consts.CourseKubun.Holiday;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    // 曜日有効フラグマップを生成
                    _youbiChumonFlagsMap = new Dictionary<int, YoubiChumonFlags>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        YoubiChumonFlags youbiChumonFlags = new YoubiChumonFlags(dr);
                        _youbiChumonFlagsMap[youbiChumonFlags.得意先ＣＤ] = youbiChumonFlags;
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "曜日別注文設定マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("曜日別注文設定マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,         "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN,       "得意先名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,            "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN,          "商品名");
                fpSpread1_Sheet1.BindDataColumn(TANKA_TOKUISAKI_KOMI_COLUMN, "商品単価税込");
                fpSpread1_Sheet1.BindDataColumn(SUNDAY_COLUMN,               "注文数_日");
                fpSpread1_Sheet1.BindDataColumn(MONDAY_COLUMN,               "注文数_月");
                fpSpread1_Sheet1.BindDataColumn(TUESDAY_COLUMN,              "注文数_火");
                fpSpread1_Sheet1.BindDataColumn(WEDNESDAY_COLUMN,            "注文数_水");
                fpSpread1_Sheet1.BindDataColumn(THURSDAY_COLUMN,             "注文数_木");
                fpSpread1_Sheet1.BindDataColumn(FRIDAY_COLUMN,               "注文数_金");
                fpSpread1_Sheet1.BindDataColumn(SATURDAY_COLUMN,             "注文数_土");
                fpSpread1_Sheet1.BindDataColumn(HOLIDAY_COLUMN,              "注文数_祝");

                // 商品単価税込が(得意先単価マスタからではなく)商品マスタから取得した場合は色を薄くする
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                {
                    DataRow dr = dt.Rows[rowIndex];
                    if (((int)dr["単価淡色フラグ"]) == 1)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, TANKA_TOKUISAKI_KOMI_COLUMN].ForeColor = SystemColors.GrayText;
                    }

                    // Web得意先にも存在する場合はエラーとして見せる
                    if (dr["WEB得意先ＣＤ"] as int? != null)
                    {
                        dr.SetColumnError("得意先ＣＤ", "Web得意先にも登録されています。");
                    }
                }

                SetYoubiCellColor();        // 注文曜日のセルの見え方を制御

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
                    fpSpread1_Sheet1.SetActiveCell(0, TOKUISAKI_CD_COLUMN);
                    // ↑これだけでいいはずなのに、なぜか↓がないとうまく表示されない
                    fpSpread1.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Top, FarPoint.Win.Spread.HorizontalPosition.Left);
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

                YoubiChumonSettei youbiChumonSettei = new YoubiChumonSettei(e.Row);

                // 得意先CDの場合は得意先名を取得
                if (e.Column.ColumnName == "得意先ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["得意先名"] = null;
                    if (youbiChumonSettei.得意先ＣＤ != null)
                    {
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)youbiChumonSettei.得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            e.Row["得意先名"] = tokuisaki.得意先名;

                            // 指定した行の注文曜日のセルの見え方を制御
                            SetYoubiCellColor(fpSpread1_Sheet1.ActiveRowIndex, (int)youbiChumonSettei.得意先ＣＤ);

                            // Web得意先マスタが存在すればエラーとして見せる
                            WebTokuisaki webTokuisaki = WebTokuisaki.GetWebTokuisaki((int)youbiChumonSettei.得意先ＣＤ);
                            if (webTokuisaki != null)
                            {
                                e.Row.SetColumnError(e.Column, "Web得意先にも登録されています。");
                            }
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
                    e.Row["商品単価税込"] = DBNull.Value;
                    if (youbiChumonSettei.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)youbiChumonSettei.商品ＣＤ);
                        if (syohin != null)
                        {
                            e.Row["商品名"] = syohin.商品名;

                            // 得意先単価マスタから
                            TokuisakiTanka tokuisakiTanka = TokuisakiTanka.GetTokuisakiTanka(youbiChumonSettei.得意先ＣＤ ?? -1, (int)youbiChumonSettei.商品ＣＤ);
                            if (tokuisakiTanka != null)
                            {
                                e.Row.SetField<decimal?>("商品単価税込", tokuisakiTanka.商品単価税込);
                                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, TANKA_TOKUISAKI_KOMI_COLUMN].ResetForeColor();
                            }
                            else
                            {
                                // 商品マスタから
                                e.Row.SetField<decimal?>("商品単価税込", syohin.商品単価税込);
                                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, TANKA_TOKUISAKI_KOMI_COLUMN].ForeColor = SystemColors.GrayText;
                            }
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

                if (e.Column.ColumnName.StartsWith("注文数_"))
                {
                    e.Row.SetColumnError(e.Column, null);
                    // 0は表示しない（クラス側でも同様にしている）
                    if (e.Row[e.Column.ColumnName] as decimal? == 0m)
                    {
                        e.Row[e.Column.ColumnName] = DBNull.Value;
                    }

                    // 複数選択されている場合のDeleteキーは、fpSpread1_KeyDown の Deleteキーに任せる
                    // ここではセルに値が入力された場合
                    if (fpSpread1_Sheet1.GetSelections().Length == 0 && youbiChumonSettei.得意先ＣＤ != null)
                    {
                        // 指定した行の注文曜日のセルの見え方を制御
                        SetYoubiCellColor(fpSpread1_Sheet1.ActiveRowIndex, (int)youbiChumonSettei.得意先ＣＤ);
                    }

                    // ここでは１つのセルの操作に対する処理
                    if (fpSpread1_Sheet1.GetSelections().Length == 1 && youbiChumonSettei.得意先ＣＤ != null)
                    {
                        FarPoint.Win.Spread.Model.CellRange cellRange = fpSpread1_Sheet1.GetSelection(0);
                        if (cellRange.ColumnCount == 1 && cellRange.RowCount == 1)
                        {
                            // 指定した行の注文曜日のセルの見え方を制御
                            SetYoubiCellColor(fpSpread1_Sheet1.ActiveRowIndex, (int)youbiChumonSettei.得意先ＣＤ);
                        }
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
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// すべの行の注文曜日のセルの見え方を制御します
        /// </summary>
        private void SetYoubiCellColor()
        {
            for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
            {
                SetYoubiCellColor(rowIndex);
            }
        }

        /// <summary>
        /// 指定した行の注文曜日のセルの見え方を制御します
        /// </summary>
        /// <param name="rowIndex">行インデックス</param>
        private void SetYoubiCellColor(int rowIndex)
        {
            int? tokuisakiCD = fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_CD_COLUMN].Value as int?;
            SetYoubiCellColor(rowIndex, tokuisakiCD);
        }

        /// <summary>
        /// 指定した行の注文曜日のセルの見え方を得意先にしたがって制御します
        /// </summary>
        /// <param name="rowIndex">行インデックス</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        private void SetYoubiCellColor(int rowIndex, int? tokuisakiCD)
        {
            YoubiChumonFlags youbiChumonFlags = new YoubiChumonFlags();

            // 得意先CDに該当する曜日有効フラグを取得
            if (tokuisakiCD != null)
            {
                if (_youbiChumonFlagsMap.ContainsKey((int)tokuisakiCD) == false)
                {
                    // マップに無ければ生成
                    youbiChumonFlags = YoubiChumonFlags.GetYoubiChumonFlags((int)tokuisakiCD);
                    _youbiChumonFlagsMap[(int)tokuisakiCD] = youbiChumonFlags;
                }
                youbiChumonFlags = _youbiChumonFlagsMap[(int)tokuisakiCD];
            }

            // フラグに従って入力制御
            // 日曜
            SetYoubiCellColor(rowIndex, SUNDAY_COLUMN, youbiChumonFlags.日曜有効, youbiChumonFlags.休日設定, youbiChumonFlags.祝日配送区分);
            for (int columnIndex = MONDAY_COLUMN; columnIndex <= FRIDAY_COLUMN; columnIndex++)
            {
                // 平日(月～金)
                SetYoubiCellColor(rowIndex, columnIndex, youbiChumonFlags.平日有効, youbiChumonFlags.休日設定, youbiChumonFlags.祝日配送区分);
            }
            // 土曜
            SetYoubiCellColor(rowIndex, SATURDAY_COLUMN, youbiChumonFlags.土曜有効, youbiChumonFlags.休日設定, youbiChumonFlags.祝日配送区分);
            // 祝日
            SetYoubiCellColor(rowIndex, HOLIDAY_COLUMN, youbiChumonFlags.祝日有効, youbiChumonFlags.休日設定, youbiChumonFlags.祝日配送区分);

            // 得意先の背景を着色
            SetTokuisakiCellColor(rowIndex);
        }

        /// <summary>
        /// 指定した行と列の注文曜日のセルの見え方を変える
        /// </summary>
        /// <param name="rowIndex">行インデックス</param>
        /// <param name="columnIndex">列インデックス</param>
        /// <param name="flg">フラグ</param>
        private void SetYoubiCellColor(int rowIndex, int columnIndex, bool flg, string kyujitsuSettei, bool isHolidayDelivery)
        {
            // まずリセット
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ResetForeColor();
            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ResetBackColor();

            // 得意先の休日の場合の色をセット
            if (columnIndex == HOLIDAY_COLUMN)
            {
                // 祝日列の場合、祝日配送区分がOFFなら着色
                if (isHolidayDelivery == false)
                {
                    fpSpread1_Sheet1.Cells[rowIndex, columnIndex].BackColor = HOLIDAY_BACK_COLOR;
                }
            }
            else
            {
                // 祝日以外
                DayOfWeek dayOfWeek = (DayOfWeek)(columnIndex - SUNDAY_COLUMN); // 日:0～土:6としてDayOfWeekに変換
                if (Tokuisaki.GetHoliday(kyujitsuSettei, dayOfWeek))
                {
                    fpSpread1_Sheet1.Cells[rowIndex, columnIndex].BackColor = HOLIDAY_BACK_COLOR;

                    // 得意先の休日に数量が入力されている場合はエラーとして見せる
                    fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ErrorText = null;
                    if (fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value != null)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ErrorText = "得意先の休日です。入力された数量は無視します。";
                    }
                }
            }

            // フラグがOFF(=無効)ならグレーにする
            if (flg == false)
            {
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = SystemColors.GrayText;
                fpSpread1_Sheet1.Cells[rowIndex, columnIndex].BackColor = SystemColors.ControlLight;
            }
        }

        /// <summary>
        /// 指定した行の注文数が未設定の場合は得意先の背景を着色する
        /// </summary>
        /// <param name="rowIndex">行インデックス</param>
        private void SetTokuisakiCellColor(int rowIndex)
        {
            fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_CD_COLUMN].ResetBackColor();
            fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_NAME_COLUMN].ResetBackColor();
            fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_CD_COLUMN].ResetBackColor();
            fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_NAME_COLUMN].ResetBackColor();

            for (int columnIndex = SUNDAY_COLUMN; columnIndex <= HOLIDAY_COLUMN; columnIndex++)
            {
                if (fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Value != null)
                {
                    return;
                }
            }

            fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_CD_COLUMN].BackColor = NO_CHUMON_BACK_COLOR;
            fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_NAME_COLUMN].BackColor = NO_CHUMON_BACK_COLOR;
            fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_CD_COLUMN].BackColor = NO_CHUMON_BACK_COLOR;
            fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_NAME_COLUMN].BackColor = NO_CHUMON_BACK_COLOR;
        }

        /// <summary>
        /// 曜日別固定数注文反映画面表示
        /// </summary>
        private void ShowHanei()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 部署、コース、得意先を曜日別固定数注文反映画面の条件に渡す
                YoubiChumonHanneiCriteria youbiChumonHanneiCriteria = new YoubiChumonHanneiCriteria();
                youbiChumonHanneiCriteria.部署ＣＤ = _youbiChumonSetteiCriteria.部署ＣＤ;
                youbiChumonHanneiCriteria.コースＣＤFrom = _youbiChumonSetteiCriteria.コースＣＤ;
                youbiChumonHanneiCriteria.コースＣＤTo = _youbiChumonSetteiCriteria.コースＣＤ;
                youbiChumonHanneiCriteria.得意先ＣＤFrom = _youbiChumonSetteiCriteria.得意先ＣＤ;
                youbiChumonHanneiCriteria.得意先ＣＤTo = _youbiChumonSetteiCriteria.得意先ＣＤ;
                youbiChumonHanneiCriteria.商品ＣＤ = _youbiChumonSetteiCriteria.商品ＣＤ;

                Form210 form = new Form210(youbiChumonHanneiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form210_FormClosed);
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
        /// 曜日別固定数注文反映画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form210_FormClosed(object sender, FormClosedEventArgs e)
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
        /// 曜日別注文設定マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 曜日別注文設定マスタを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["曜日別注文設定マスタ"];

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
                DataTable dt = ds.Tables["曜日別注文設定マスタ"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    YoubiChumonSettei youbiChumonSettei = new YoubiChumonSettei(dr);

                    // エラーがある？
                    if (youbiChumonSettei.HasErrors)
                    {
                        dr.SetColumnError("得意先ＣＤ", youbiChumonSettei["得意先ＣＤ"]);
                        dr.SetColumnError("商品ＣＤ", youbiChumonSettei["商品ＣＤ"]);
                        hasError = true;
                    }
                    else
                    {
                        // 得意先ＣＤに該当するマスタが存在するか？
                        if (youbiChumonSettei.得意先ＣＤ != null)
                        {
                            Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)youbiChumonSettei.得意先ＣＤ);
                            if (tokuisaki == null)
                            {
                                dr.SetColumnError("得意先ＣＤ", "得意先が見つかりません。");
                                hasError = true;
                            }
                        }

                        // 商品ＣＤに該当するマスタが存在するか？
                        if (youbiChumonSettei.商品ＣＤ != null)
                        {
                            Syohin syohin = Syohin.GetSyohin((int)youbiChumonSettei.商品ＣＤ);
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
                        DataTable dt = ds.Tables["曜日別注文設定マスタ"];

                        foreach (DataRow dr in dt.Rows)
                        {
                            YoubiChumonSettei youbiChumonSettei = new YoubiChumonSettei(dr);
                            switch (dr.RowState)
                            {
                                case DataRowState.Added:        // 追加
                                    historyProcess = History.ProcessType.Create;
                                    ret = youbiChumonSettei.Insert(con, tran);
                                    break;

                                case DataRowState.Modified:     // 変更
                                    historyProcess = History.ProcessType.Update;
                                    ret = youbiChumonSettei.Update(con, tran);
                                    break;

                                case DataRowState.Deleted:      // 削除
                                    historyProcess = History.ProcessType.Delete;
                                    ret = youbiChumonSettei.Delete(con, tran);
                                    break;

                                default:
                                    continue;
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, youbiChumonSettei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            History.Save(con, tran, this.Text, "曜日別注文設定マスタ", youbiChumonSettei.Code, historyProcess);
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("曜日別注文設定マスタを{0}しました。", historyProcess.DisplayName());
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
                                                {"注文曜日日", "注文曜日日"},
                                                {"注文曜日月", "注文曜日月"},
                                                {"注文曜日火", "注文曜日火"},
                                                {"注文曜日水", "注文曜日水"},
                                                {"注文曜日木", "注文曜日木"},
                                                {"注文曜日金", "注文曜日金"},
                                                {"注文曜日土", "注文曜日土"},
                                                {"注文曜日祝", "注文曜日祝"} };

                // 曜日別注文設定マスタリスト
                List<YoubiChumonSettei> youbiChumonSetteiList = new List<YoubiChumonSettei>();
                int failCount = 0;

                for (int rowIndex = rowStart; rowIndex <= rowEnd; rowIndex++)
                {
                    // 全ての取得対象列のインデックスを取得済み？
                    if (columnIndexMap.Count >= targetColumnNames.GetLength(0))
                    {
                        // 取得対象列の検証結果を取得
                        YoubiChumonSettei youbiChumonSettei = ParseYoubiChumonSettei(table, rowIndex, columnIndexMap);
                        if (youbiChumonSettei != null)
                        {
                            youbiChumonSetteiList.Add(youbiChumonSettei);
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
                                                "\n・注文曜日日～注文曜日祝" +
                                                "\n\n取り込みはできませんでした。",
                                                fileName);
                            MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }

                // DB登録後、成功した件数と失敗した件数を表示する
                if (SaveByImport(youbiChumonSetteiList))
                {
                    string failMsg = "";
                    if (failCount != 0)
                    {
                        failMsg = "\n\n失敗の詳細は操作履歴一覧を参照してください。";
                    }
                    MessageBox.Show(this, string.Format("取込が完了しました。\n成功：{0}件\n失敗：{1}件{2}",youbiChumonSetteiList.Count, failCount, failMsg),
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
        /// 取得対象列の値を検証して YoubiChumonSettei インスタンスを返す
        /// </summary>
        /// <param name="table">Excelから取得した二次元配列</param>
        /// <param name="rowIndex">二次元配列のカレント行</param>
        /// <param name="columnIndexMap">列名をキーとして、そのインデックスを値に持つマップ</param>
        /// <returns>rowIndex行の、取得対象列の値がすべて有効の場合は 、その値を格納した YoubiChumonSetteiインスタンス。ひとつでも無効な場合は null</returns>
        private YoubiChumonSettei ParseYoubiChumonSettei(object[,] table, int rowIndex, Dictionary<string, int> columnIndexMap)
        {
            YoubiChumonSettei youbiChumonSettei = new YoubiChumonSettei();
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

                    // 注文数以外はNull不可
                    if (columnName.StartsWith("注文曜日") == false)
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
                                    youbiChumonSettei.得意先ＣＤ = tokuisakiCD;
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
                                    youbiChumonSettei.商品ＣＤ = syohinCD;
                                }
                            }
                            break;

                        case "注文曜日日":
                        case "注文曜日月":
                        case "注文曜日火":
                        case "注文曜日水":
                        case "注文曜日木":
                        case "注文曜日金":
                        case "注文曜日土":
                        case "注文曜日祝":
                            decimal suryo;
                            // 注文数はnull可（なので入力されている場合のみ検証する）
                            if (string.IsNullOrEmpty(value) == false)
                            {
                                if (decimal.TryParse(value, out suryo) == false)
                                {
                                    invalidColumns.Add(columnName);
                                }
                                else
                                {
                                    // 小数はエラー
                                    if (suryo != decimal.Truncate(suryo))
                                    {
                                        if (invalidColumns.Contains(columnName) == false)
                                        {
                                            invalidColumns.Add(columnName);
                                        }
                                    }
                                    else
                                    {
                                        // マイナスは不可
                                        if (suryo < 0)
                                        {
                                            if (invalidColumns.Contains(columnName) == false)
                                            {
                                                invalidColumns.Add(columnName);
                                            }
                                        }
                                        else
                                        {
                                            switch (columnName)
                                            {
                                                case "注文曜日日":youbiChumonSettei.注文数_日 = suryo;break;
                                                case "注文曜日月":youbiChumonSettei.注文数_月 = suryo;break;
                                                case "注文曜日火":youbiChumonSettei.注文数_火 = suryo;break;
                                                case "注文曜日水":youbiChumonSettei.注文数_水 = suryo;break;
                                                case "注文曜日木":youbiChumonSettei.注文数_木 = suryo;break;
                                                case "注文曜日金":youbiChumonSettei.注文数_金 = suryo;break;
                                                case "注文曜日土":youbiChumonSettei.注文数_土 = suryo;break;
                                                case "注文曜日祝":youbiChumonSettei.注文数_祝 = suryo;break;
                                            }
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
                    youbiChumonSettei = null;   // 検証結果
                    string columns = string.Join(",", nullColumns);
                    string msg = string.Format("{0}でExcelから取り込みましたが {1}行目の次の項目が未入力です。{2}",this.Text, rowIndex, columns);
                    History.Save(msg);
                }

                if (invalidColumns.Count != 0)
                {
                    youbiChumonSettei = null;   // 検証結果
                    string columns = string.Join(",", invalidColumns);
                    string msg = string.Format("{0}でExcelから取り込みましたが {1}行目の次の項目に不備があります。{2}", this.Text, rowIndex, columns);
                    History.Save(msg);
                }

                return youbiChumonSettei;
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
        /// <param name="youbiChumonSetteiList">曜日別注文設定マスタリスト</param>
        /// <returns>成功した場合はtrue</returns>
        private bool SaveByImport(List<YoubiChumonSettei> youbiChumonSetteiList)
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
                        foreach (YoubiChumonSettei youbiChumonSettei in youbiChumonSetteiList)
                        {
                            // 同じキーを持つレコードが存在する場合は更新。無ければ追加
                            YoubiChumonSettei target = YoubiChumonSettei.GetYoubiChumonSettei(con, tran,
                                                                                              (int)youbiChumonSettei.得意先ＣＤ,
                                                                                              (int)youbiChumonSettei.商品ＣＤ);
                            if (target != null)
                            {
                                target.注文数_日 = youbiChumonSettei.注文数_日;
                                target.注文数_月 = youbiChumonSettei.注文数_月;
                                target.注文数_火 = youbiChumonSettei.注文数_火;
                                target.注文数_水 = youbiChumonSettei.注文数_水;
                                target.注文数_木 = youbiChumonSettei.注文数_木;
                                target.注文数_金 = youbiChumonSettei.注文数_金;
                                target.注文数_土 = youbiChumonSettei.注文数_土;
                                target.注文数_祝 = youbiChumonSettei.注文数_祝;
                                ret = target.Update(con, tran);
                            }
                            else
                            {
                                historyProcess = History.ProcessType.Create;
                                ret = youbiChumonSettei.Insert(con, tran);
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, youbiChumonSettei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            History.Save(con, tran, this.Text, "曜日別注文設定マスタ", youbiChumonSettei.Code, historyProcess);
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("曜日別注文設定マスタを{0}しました。", historyProcess.DisplayName());
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

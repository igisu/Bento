using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// コースマスタ詳細
    /// </summary>
    public partial class Form708D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int NO_COLUMN             = 0;    // コース順列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 1;    // 得意先ＣＤ列インデックス
        private const int REFERENCE_COLUMN      = 2;    // 参照ボタン列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 3;    // 得意先名列インデックス
        private const int INVALID_COLUMN        = 4;    // 無効フラグ列インデックス
        private const int DELETE_COLUMN         = 5;    // 削除ボタン列インデックス
        private const int GUID_COLUMN           = 6;    // ユニークキー列インデックス
        private const int UPDATED_COLUMN        = 7;    // 変更フラグ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用コースマスタ
        /// </summary>
        private Course _course;

        /// <summary>
        /// 初期状態用コースマスタ
        /// </summary>
        private Course _origin;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// コーステーブルを格納するデータテーブル
        /// </summary>
        private DataTable _dtCourseTable;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// Spreadの行移動機能ヘルパー
        /// </summary>
        private SpreadRowMoveHelper _spreadRowMoveHelper;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="course"></param>
        public Form708D(Course course)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _course = new Course(course);

                // コース区分、部署ＣＤ（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _course.コース区分 = _course.コース区分 ?? -1;
                _course.部署ＣＤ = _course.部署ＣＤ ?? -1;

                _IsNewRecord = false;
                if (_course.コースＣＤ == null)
                {
                    _IsNewRecord = true;
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
        private void Form708D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;
                btnCourseMove.Enabled = this.HasAuthEdit;

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

                // スプレッドのユニークキー列と変更フラグ列を非表示にする
                fpSpread1_Sheet1.Columns[GUID_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[UPDATED_COLUMN].Visible = false;

                // コンボボックスの選択肢生成
                ComboUtil.SetCourseKubun(cboCourseKbn);
                ComboUtil.SetBusyo(cboBusyo);

                // 新規の場合は削除不可
                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnDelete.Enabled = false;
                    btnCourseMove.Enabled = false;      // コースの移動も不可

                    // 部署CDコンボの初期値を設定
                    _course.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnSave.Text = "更新(F9)";
                    // 既存の場合はコースＣＤ変更不可
                    txtCourseCD.Enabled = false;
                }

                // 初期状態として保持
                _origin = new Course(_course);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                courseBindingSource.DataSource = _course;

                // Spreadの行移動機能ヘルパー
                _spreadRowMoveHelper = new SpreadRowMoveHelper(fpSpread1, courseTableBindingSource, GUID_COLUMN, UPDATED_COLUMN);

                chkRowDrag.Checked = false;     // 初期値は行ドラッグしない

                // コーステーブルを取得
                GetCourseTable(_course.コースＣＤ);
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708D_Shown(object sender, EventArgs e)
        {
            // 編集の場合はスプレッドにフォーカス
            if (_IsNewRecord == false)
            {
                fpSpread1.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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

                case Keys.F11:              // 削除
                    btnDelete.Focus();
                    btnDelete.PerformClick();
                    break;

                // 終了はフォームのCancelButtonなのでEscキーとしていたが、
                // (ボタンのクリックではなく)キー押下でフォームを閉じたときに親フォームがアクティブにならないという問題がある。
                // これを回避するには当該キー押下時に当該ボタンにフォーカスすればよいのだが、
                // CancelButtonだとイベントが発生せずEscキー押下を検知できず、ボタンにフォーカスする契機が無い。
                case Keys.Escape:
                    btnQuit.Focus();
                    btnQuit.PerformClick();
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708D_FormClosing(object sender, FormClosingEventArgs e)
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
                if (HasChangeCourse() || HasChangeCourseTable())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // コースマスタの入力チェック
                            if (CanSaveCourse() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // コーステーブルの入力チェック
                            if (CanSaveCourseTable() == false)
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
        /// 担当者CDキー押下（F1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTantosyaCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTantosyaCD.PerformClick();
            }
        }

        /// <summary>
        /// 担当者参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTantosyaCD_Click(object sender, EventArgs e)
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
        /// 担当者選択ダイアログクローズ（担当者ＣＤ）
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
                    errorProvider1.SetError(txtTantosyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtTantosyaCD.Text = tantousya.担当者ＣＤ.ToString();
                    _course.担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblTantosyaName.Text = tantousya.担当者名;
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
        /// Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
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
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
            // F1？
            if (e.KeyCode == Keys.F1)
            {
                OpenForm900BySpread();          // 得意先選択ダイアログを表示
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
                    case TOKUISAKI_CD_COLUMN:      // 得意先ＣＤ
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dtCourseTable_ColumnChangedが発生
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
                // 参照ボタン？
                if (e.Column == REFERENCE_COLUMN)
                {
                    OpenForm900BySpread();
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

                    fpSpread1_Sheet1.Rows.Remove(e.Row, 1);
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
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, TOKUISAKI_CD_COLUMN].Value = tokuisaki.得意先ＣＤ;   // 得意先CDセルに値を設定→dtCourseTable_ColumnChanged()が呼ばれる
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
            DataRow dr = ((DataRowView)courseTableBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// いちばん上へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTop_Click(object sender, EventArgs e)
        {
            try
            {
                _spreadRowMoveHelper.MoveTop();
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
        /// 上へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                _spreadRowMoveHelper.MoveUp();
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
        /// 下へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                _spreadRowMoveHelper.MoveDown();
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
        /// いちばん下へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBottom_Click(object sender, EventArgs e)
        {
            try
            {
                _spreadRowMoveHelper.MoveBottom();
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
        /// 行挿入ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                // 最終行の場合はNOP
                if (fpSpread1_Sheet1.Rows.Count - 1 <= fpSpread1_Sheet1.ActiveRowIndex)
                {
                    return;
                }

                // Spreadに1行追加
                fpSpread1_Sheet1.Rows.Add(fpSpread1_Sheet1.ActiveRowIndex, 1);      // dtCourseTable_TableNewRow()が発生
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
        /// 行ヘッダのドラッグで移動するチェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkRowDrag_CheckedChanged(object sender, EventArgs e)
        {
            fpSpread1.AllowRowMove = chkRowDrag.Checked;
        }

        /// <summary>
        /// コース移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseMove_Click(object sender, EventArgs e)
        {
            try
            {
                // 初期状態と異なる？
                if (HasChangeCourse() || HasChangeCourseTable())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // コースマスタの入力チェック
                            if (CanSaveCourse() == false)
                            {
                                return;
                            }

                            // コーステーブルの入力チェック
                            if (CanSaveCourseTable() == false)
                            {
                                return;
                            }

                            if (Save() == false)
                            {
                                return;
                            }
                            break;

                        case DialogResult.Cancel:
                            return;
                    }
                }

                ShowCourseMove();           // コース移動表示
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
        /// 登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // コースマスタの入力チェック
                if (CanSaveCourse() == false)
                {
                    return;
                }

                // コーステーブルの入力チェック
                if (CanSaveCourseTable() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

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
        /// 削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 削除対象のルートがほかのテーブルで使用されているかチェック
                if (CanDelete() == false)
                {
                    return;
                }

                if (MessageBox.Show(this, "このコースを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                // 直近１ヶ月にこのコースの売上があるか？
                if (HasLastMonthUriage())
                {
                    if (MessageBox.Show(this, "このコースに対して直近１ヶ月間に売上が存在します。本当にこのコースを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                if (Delete())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
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
                saveFileDialog1.FileName = string.Format("{0}({1})", this.Text, _course.コースＣＤ);
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
        /// コーステーブル取得
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        private void GetCourseTable(int? courseCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.コースＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先ＣＤ AS 得意先ＣＤorg" +
                                      "\r\n     ,T.得意先名略称" +
                                      "\r\n     ,A.コース順" +
                                      "\r\n     ,A.無効フラグ" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     コーステーブル AS A" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = @企業ＣＤ AND A.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND A.コースＣＤ = @コースＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.コース順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    _dtCourseTable = new DataTable("コーステーブル");
                    adp.Fill(_dtCourseTable);
                }

                _dtCourseTable.ColumnChanged += new DataColumnChangeEventHandler(dtCourseTable_ColumnChanged);

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                courseTableBindingSource.DataSource = null;
                courseTableBindingSource.DataSource = _dtCourseTable;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", _dtCourseTable.Rows.Count);

                log.Info("コーステーブルを検索しました。");

                fpSpread1_Sheet1.BindDataColumn(NO_COLUMN,             "コース順");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名略称");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN,        "無効フラグ");

                fpSpread1.Focus();

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
        private void dtCourseTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                CourseTable courseTable = new CourseTable(e.Row);

                // 得意先CDの場合は得意先名を取得
                if (e.Column.ColumnName == "得意先ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["得意先名略称"] = null;
                    if (courseTable.得意先ＣＤ != null)
                    {
                        Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)courseTable.得意先ＣＤ);
                        if (tokuisaki != null)
                        {
                            e.Row["得意先名略称"] = tokuisaki.得意先名略称;
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "得意先が見つかりません。");
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
        /// コースマスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeCourse()
        {
            if (_course.Equals(_origin))
            {
                return false;
            }
            return true;
        }

        /// コーステーブルが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeCourseTable()
        {
            // コーステーブルを比較
            if (_dtCourseTable == null)
            {
                return false;
            }

            foreach (DataRow dr in _dtCourseTable.Rows)
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
        /// コースマスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveCourse()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_course.HasErrors)
                {
                    MessageBox.Show(this, _course.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// コーステーブルが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveCourseTable()
        {
            bool hasError = false;

            try
            {
                // 行数分ループ
                List<DataRow> drList = _spreadRowMoveHelper.GetRowsInSpreadOrder();
                foreach (DataRow dr in drList)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    CourseTable courseTable = new CourseTable(dr);

                    // コースCDがない場合は新規扱い
                    if (courseTable.コースＣＤ == null)
                    {
                        courseTable.コースＣＤ = _course.コースＣＤ;
                    }

                    // エラーがある？
                    if (courseTable.HasErrors)
                    {
                        dr.SetColumnError("得意先ＣＤ", courseTable["得意先ＣＤ"]);
                        hasError = true;
                    }
                    else
                    {
                        // 得意先ＣＤに該当するマスタが存在するか？
                        if (courseTable.得意先ＣＤ != null)
                        {
                            Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)courseTable.得意先ＣＤ);
                            if (tokuisaki == null)
                            {
                                dr.SetColumnError("得意先ＣＤ", "得意先が見つかりません。");
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
            History.ProcessType historyProcess = History.ProcessType.Create;

            if (_IsNewRecord == false)
            {
                historyProcess = History.ProcessType.Update;
            }

            List<DataRow> drList = _spreadRowMoveHelper.GetRowsInSpreadOrder();  // Spreadの順でDataRowのリストを取得

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // コースマスタ保存
                    if (SaveCourse(con, tran) == false)
                    {
                        return false;
                    }

                    // コーステーブル保存
                    if (SaveCourseTable(con, tran, drList) == false)
                    {
                        return false;
                    }

                    // 当該コースにぶら下がる得意先に対して、コースＣＤを注文に反映する
                    if (UpdateChumonCourseOriginal(con, tran, drList, false) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("コースマスタとコーステーブルを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// コースマスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveCourse(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _course.Insert(con, tran);
                }
                else
                {
                    ret = _course.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _course.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "コースマスタ", _course.Code, historyProcess);
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
        /// コーステーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="drList">Spreadの順に沿ったDataRowのリスト</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveCourseTable(SqlConnection con, SqlTransaction tran, List<DataRow> drList)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Create;
                int seq = 1;
                bool ret = false;

                // 全削除→全追加
                SqlCommand cmdDel = con.CreateCommand();
                cmdDel.CommandText = "DELETE FROM コーステーブル WHERE 企業ＣＤ = @企業ＣＤ AND コースＣＤ = @コースＣＤ";
                cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmdDel.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_course.コースＣＤ;
                cmdDel.Transaction = tran;
                log.Debug(cmdDel.CommandTextWithParameters());
                cmdDel.ExecuteNonQuery();

                // 行数分ループ
                foreach (DataRow dr in drList)
                {
                    CourseTable courseTable = new CourseTable(dr);

                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    courseTable.コースＣＤ = _course.コースＣＤ;
                    courseTable.コース順 = seq;
                    seq++;
                    ret = courseTable.Insert(con, tran);
                    historyProcess = History.ProcessType.Create;

                    if (ret == false)
                    {
                        MessageBox.Show(this, courseTable.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "コーステーブル", courseTable.Code, historyProcess);
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
        /// コースにぶら下がる得意先に対して、コースＣＤを注文に反映する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="drList">Spreadの順に沿ったDataRowのリスト</param>
        /// <param name="isDelete">コースマスタ削除の場合はtrue</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateChumonCourseOriginal(SqlConnection con, SqlTransaction tran, List<DataRow> drList, bool isDelete)
        {
            History.ProcessType historyProcess = History.ProcessType.Update;
            bool ret = true;

            try
            {
                // コース臨時組み替えのような一度のSQLでは実現できない。
                // ※コーステーブルから削除された得意先については注文が取得できないので
                // そこで、データソースのレコードの内容に応じて、以下のように処理を振り分ける。
                // ただし、得意先・配送日に該当する臨時コースがあれば、そちらを優先する。
                // ┌────────────┬───┬───┬──┐
                // │      注文のコースＣＤ→│等しい│異なる│Null│
                // │↓コーステーブルの得意先│      │      │    │
                // ├────────────┼───┼───┼──┤
                // │追加                    │  NOP │  NOP │ ① │
                // ├────────────┼───┼───┼──┤
                // │存在                    │  NOP │  NOP │ ① │
                // ├────────────┼───┼───┼──┤
                // │無効                    │  ②  │  NOP │ NOP│
                // ├────────────┼───┼───┼──┤
                // │削除                    │  ②  │  NOP │ NOP│
                // └────────────┴───┴───┴──┘
                // ①配送日から求まるコース区分が同じなら更新。異なるならNOP
                // ②Nullに更新

                // ※コース移動と異なり、注文のコースＣＤが異なる場合は更新しないことに注意
                // ┌────────┬───────────────────────────────────────────┐
                // │コースマスタ詳細│追加された得意先が別のコースに属していることになる。そのコースＣＤを勝手に変えてはダメ│
                // ├────────┼───────────────────────────────────────────┤
                // │コース移動      │指定されたコースを互いに交換している                                                  │
                // └────────┴───────────────────────────────────────────┘

                // 行数分ループ
                foreach (DataRow dr in drList)
                {
                    CourseTable courseTable = new CourseTable(dr);

                    // 当該得意先の未来日の注文ヘッダを取得
                    string sql = "\r\n SELECT" +
                                 "\r\n     A.企業ＣＤ" +
                                 "\r\n     ,A.注文区分" +
                                 "\r\n     ,A.喫食日" +
                                 "\r\n     ,A.得意先ＣＤ" +
                                 "\r\n     ,A.部署ＣＤ" +
                                 "\r\n     ,A.配送日" +
                                 "\r\n     ,A.得意先名" +
                                 "\r\n     ,A.担当者ＣＤ" +
                                 "\r\n     ,A3.担当者名 AS 担当者名" +
                                 "\r\n     ,A.コースＣＤ" +
                                 "\r\n     ,A.配送先ＣＤ" +
                                 "\r\n     ,A.税区分" +
                                 "\r\n     ,A.丸め処理区分" +
                                 "\r\n     ,A.備考1" +
                                 "\r\n     ,A.備考2" +
                                 "\r\n     ,A.備考3" +
                                 "\r\n     ,A.備考4" +
                                 "\r\n     ,A.備考5" +
                                 "\r\n     ,A.納品書伝票番号" +
                                 "\r\n     ,A.作成担当者ＣＤ" +
                                 "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                 "\r\n     ,A.作成日" +
                                 "\r\n     ,A.修正担当者ＣＤ" +
                                 "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                 "\r\n     ,A.修正日" +
                                 "\r\n FROM" +
                                 "\r\n     注文ヘッダ AS A" +
                                 "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                 "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                 "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = @企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ" +
                                 "\r\n WHERE" +
                                 "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                                 "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                 // コースの部署と注文の部署は一致しないことがあるので、コースの部署で注文を取得しないのが前提だが、
                                 // ここでは関連する部署のコースを更新するのが自然であると判断したので、検索条件としている。
                                 "\r\n AND A.部署ＣＤ   = @部署ＣＤ" +
                                 "\r\n AND A.配送日     > cast(getdate() as date)";     // 未来の日付

                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = courseTable.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _course.部署ＣＤ;

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dtChumonHeader = new DataTable("注文ヘッダ");
                    adp.Fill(dtChumonHeader);

                    // 削除 or 無効？
                    if (isDelete == true ||
                        dr.RowState == DataRowState.Deleted ||
                        courseTable.無効フラグ == true)
                    {
                        // 削除（RowState == DataRowState.Deleted）されていても、
                        // その得意先が別の行で追加されていれば、削除はなかったことにする
                        if (dr.RowState == DataRowState.Deleted)
                        {
                            if (drList.Any(r => r.RowState != DataRowState.Deleted && r.Field<int>("得意先ＣＤ") == courseTable.得意先ＣＤ))
                            {
                                continue;
                            }
                        }

                        foreach (DataRow drChumonHeader in dtChumonHeader.Rows)
                        {
                            ChumonHeader chumonHeader = new ChumonHeader(drChumonHeader);
                            // コースＣＤが等しい場合はNullで更新する
                            if (chumonHeader.コースＣＤ == _course.コースＣＤ)
                            {
                                // ただし臨時コースが存在すれば、それが適用されているはずなので更新しない
                                ret = IsExistRinjiCourse(con, tran, chumonHeader);
                                if (ret == false)
                                {
                                    chumonHeader.コースＣＤ = null;
                                    ret = chumonHeader.UpdateCourse(con, tran);
                                    if (ret == false)
                                    {
                                        MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }
                                    History.Save(con, tran, this.Text, "未来の注文ヘッダのコース", chumonHeader.Code, historyProcess);
                                }
                            }
                        }
                    }
                    else
                    {
                        // 存在 or 追加
                        foreach (DataRow drChumonHeader in dtChumonHeader.Rows)
                        {
                            ChumonHeader chumonHeader = new ChumonHeader(drChumonHeader);
                            // コースＣＤがNullの場合、配送日から求まるコース区分が同じならコースＣＤを更新
                            if (chumonHeader.コースＣＤ == null)
                            {
                                int courseKubun = Course.GetCourseKubun((DateTime)chumonHeader.配送日);
                                if (courseKubun == _course.コース区分)
                                {
                                    // ただし臨時コースが存在すれば、それが適用されているはずなので更新しない
                                    ret = IsExistRinjiCourse(con, tran, chumonHeader);
                                    if (ret == false)
                                    {
                                        chumonHeader.コースＣＤ = _course.コースＣＤ;
                                        ret = chumonHeader.UpdateCourse(con, tran);
                                        if (ret == false)
                                        {
                                            MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return false;
                                        }
                                        History.Save(con, tran, this.Text, "未来の注文ヘッダのコース", chumonHeader.Code, historyProcess);
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("注文ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 注文の得意先・配送日に該当する臨時コースが存在するか（存在すれば適用されているはずなので、つまり臨時コースが適用されているか）を返す
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="chumonHeader">更新対象の注文ヘッダ</param>
        /// <returns>臨時コースが存在する場合は True</returns>
        private bool IsExistRinjiCourse(SqlConnection con, SqlTransaction tran, ChumonHeader chumonHeader)
        {
            bool ret = false;

            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     count(企業ＣＤ) AS CNT" +
                                  "\r\n FROM" +
                                  "\r\n     臨時コーステーブル" +
                                  "\r\n WHERE" +
                                  "\r\n     企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND 実施日 = @実施日" +
                                  "\r\n     AND 得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n     AND ISNULL(無効フラグ, 0) = 0";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = chumonHeader.配送日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = chumonHeader.得意先ＣＤ;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    if ((int)reader["CNT"] != 0)
                    {
                        ret = true;
                    }
                }
                return ret;
            }
            catch (EmuException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                _course = Course.GetCourseReload((int)_course.コースＣＤ);

                // コース区分、部署ＣＤ（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _course.コース区分 = _course.コース区分 ?? -1;
                _course.部署ＣＤ = _course.部署ＣＤ ?? -1;

                _origin = new Course(_course);

                _IsNewRecord = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                courseBindingSource.DataSource = _course;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
                // 既存の場合はコースＣＤ変更不可
                txtCourseCD.Enabled = false;

                btnDelete.Enabled = this.HasAuthEdit;
                btnCourseMove.Enabled = this.HasAuthEdit;

                // コーステーブルを取得
                GetCourseTable(_course.コースＣＤ);
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
        /// 削除可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanDelete()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 直近１ヶ月にこのコースの売上があるかを返す
        /// </summary>
        /// <returns>直近１ヶ月にこのコースの売上がある場合は True</returns>
        private bool HasLastMonthUriage()
        {
            bool ret = false;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(H.得意先ＣＤ) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ AS H" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.売上日 >= DateAdd(Month, -1, getDate())" +
                                      "\r\n     AND H.売上日 <= getDate()" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = (int)_course.コースＣＤ;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] != 0)
                        {
                            ret = true;
                        }
                    }
                    return ret;
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
        /// DB削除
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Delete()
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            List<DataRow> drList = _spreadRowMoveHelper.GetRowsInSpreadOrder();  // Spreadの順でDataRowのリストを取得

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // コースマスタ削除
                    if (DeleteCourse(con, tran) == false)
                    {
                        return false;
                    }

                    // コーステーブル削除
                    if (DeleteCourseTable(con, tran, drList) == false)
                    {
                        return false;
                    }

                    // 当該コースにぶら下がる得意先に対して、コースＣＤを注文に反映する
                    if (UpdateChumonCourseOriginal(con, tran, drList, true) == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("コースマスタとコーステーブルを{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        /// <summary>
        /// コースマスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteCourse(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _course.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, _course.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "コースマスタ", _course.Code, historyProcess);
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
        /// コーステーブル削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="drList">Spreadの順に沿ったDataRowのリスト</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteCourseTable(SqlConnection con, SqlTransaction tran, List<DataRow> drList)
        {
            try
            {
                // 全削除
                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 行数分ループ
                foreach (DataRow dr in drList)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは削除しない
                    {
                        continue;
                    }

                    CourseTable courseTable = new CourseTable(dr);

                    bool ret = courseTable.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, courseTable.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "コーステーブル", courseTable.Code, historyProcess);
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
        /// コース移動
        /// </summary>
        private void ShowCourseMove()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Form708M form = new Form708M(_course);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "コース移動画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form708M_FormClosed);
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
        /// コース移動画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form708M_FormClosed(object sender, FormClosedEventArgs e)
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
                GetCourseTable(_course.コースＣＤ);     // コーステーブルを取得
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

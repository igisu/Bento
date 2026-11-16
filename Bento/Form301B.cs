using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using System.Collections.Generic;

namespace Emu.Bento
{
    /// <summary>
    /// 一括移動入力
    /// </summary>
    public partial class Form301B : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYOHIN_CD_COLUMN        = 0;  // 商品ＣＤ列インデックス
        private const int SYOHIN_REFERENCE_COLUMN = 1;  // 参照ボタン列インデックス
        private const int SYOHIN_NAME_COLUMN      = 2;  // 商品名列インデックス
        private const int COURSE_CD_COLUMN        = 3;  // コースＣＤ列インデックス
        private const int COURSE_REFERENCE_COLUMN = 4;  // 参照ボタン列インデックス
        private const int COURSE_NAME_COLUMN      = 5;  // コース名列インデックス
        private const int IDOSU_COLUMN            = 6;  // 移動数列インデックス
        private const int CHECK_1_COLUMN          = 7;  // 確認フラグ列インデックス
        private const int CHECK_2_COLUMN          = 8;  // 相手確認フラグ列インデックス
        private const int DELETE_COLUMN           = 9;  // 削除ボタン列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 移動元コース情報
        /// </summary>
        private IdoNyuryokuDetailCriteria _idoNyuryokuDetailCriteria;

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
        /// <param name="idoNyuryokuDetailCriteria">移動元コース情報</param>
        /// <returns></returns>
        public Form301B(IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteria)
        {
            try
            {
                InitializeComponent();

                _idoNyuryokuDetailCriteria = new IdoNyuryokuDetailCriteria(idoNyuryokuDetailCriteria);
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
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301B_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

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

                //***********************************************************************************
                // Spreadの行のロック(編集不可)を有効にするためにシートのプロテクト(保護)を有効にする
                // ただし、Excel出力するとシートのあらゆる書式や設定が変更できなくなるので、
                // Excel出力の前後でプロテクト状態を切り替える(□)
                //***********************************************************************************
                fpSpread1_Sheet1.Protect = true;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                IdoNyuryokuDetailBindingSource.DataSource = _idoNyuryokuDetailCriteria;

                // データ取得
                GetData();
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
        private void Form301B_KeyDown(object sender, KeyEventArgs e)
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

                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                    btnSave.PerformClick();
                    break;

                case Keys.F12:
                    btnQuit.Focus();
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301B_FormClosing(object sender, FormClosingEventArgs e)
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
                            // 入力チェック
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
                int column = fpSpread1_Sheet1.ActiveColumnIndex;

                if (column == SYOHIN_CD_COLUMN ||
                    column == SYOHIN_REFERENCE_COLUMN)
                {
                    // Locked=true なセルについては、ダイアログを表示しない(□)
                    if (fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, column].Locked == false)
                    {
                        OpenForm908BySpread();          // 主要部品選択ダイアログを表示
                    }
                }

                if (column == COURSE_CD_COLUMN ||
                    column == COURSE_REFERENCE_COLUMN)
                {
                    // Locked=true なセルについては、ダイアログを表示しない(□)
                    if (fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, column].Locked == false)
                    {
                        OpenForm902BySpread();          // コース選択ダイアログを表示
                    }
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
                    case SYOHIN_CD_COLUMN:      // 商品ＣＤ
                    case COURSE_CD_COLUMN:      // コースＣＤ
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    // Locked=true なセルについては、その値を消さないようにする(□)
                    if (fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Locked == false)
                    {
                        fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
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
                // 商品参照ボタン？
                if (e.Column == SYOHIN_REFERENCE_COLUMN)
                {
                    OpenForm908BySpread();
                }

                // コース参照ボタン？
                if (e.Column == COURSE_REFERENCE_COLUMN)
                {
                    OpenForm902BySpread();
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
        /// Spreadから主要部品選択ダイアログを表示する
        /// </summary>
        private void OpenForm908BySpread()
        {
            try
            {
                // 主要部品選択ダイアログを表示
                Form908 formSyuyoBuhinDlg = new Form908((int)_idoNyuryokuDetailCriteria.部署ＣＤ);
                SubFormOpenInit(formSyuyoBuhinDlg);
                formSyuyoBuhinDlg.FormClosed += new FormClosedEventHandler(Form908SyohinCDBySpread_FormClosed);
                formSyuyoBuhinDlg.ShowDialog();
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
        /// Spreadから開いた主要部品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form908SyohinCDBySpread_FormClosed(object sender, FormClosedEventArgs e)
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
                Form908 form = (Form908)sender;
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
        /// Spreadからコース選択ダイアログを表示する
        /// </summary>
        private void OpenForm902BySpread()
        {
            try
            {
                // コース選択ダイアログを表示
                Form902 formTokuisakiDlg = new Form902();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form902TokuisakiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
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
        private void Form902TokuisakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, newColumnIndex);

                    Course course = form.SelectedCourse;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, COURSE_CD_COLUMN].Value = course.コースＣＤ;   // コースCDセルに値を設定→dt_ColumnChanged()が呼ばれる
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
        /// 登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 入力チェック
                if (CanSave() == false)
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
        /// データ取得
        /// </summary>
        private void GetData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     MI.企業ＣＤ" +
                                      "\r\n     ,MI.コースＣＤ" +
                                      "\r\n     ,MI.相手コースＣＤ" +
                                      "\r\n     ,MI.相手コースＣＤ AS 相手コースＣＤorg" +
                                      "\r\n     ,CM.コース名 AS 相手コース名" +
                                      "\r\n     ,MI.商品ＣＤ" +
                                      "\r\n     ,MI.商品ＣＤ AS 商品ＣＤorg" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,MI.移動日" +
                                      "\r\n     ,MI.ＳＥＱ" +
                                      "\r\n     ,MI.ＳＥＱ AS ＳＥＱorg" +
                                      "\r\n     ,MI.個数" +
                                      "\r\n     ,MI.確認フラグ" +
                                      "\r\n     ,MI.確認日時" +
                                      "\r\n     ,MI.相手確認フラグ" +
                                      "\r\n     ,MI.相手確認日時" +
                                      "\r\n     ,MI.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,MI.作成日" +
                                      "\r\n     ,MI.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,MI.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     モバイル_移動入力 AS MI" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = MI.企業ＣＤ AND S.商品ＣＤ = MI.商品ＣＤ" +
                                      "\r\n     LEFT JOIN コースマスタ AS CM ON MI.企業ＣＤ = CM.企業ＣＤ AND MI.相手コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = MI.企業ＣＤ AND MI.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = MI.企業ＣＤ AND MI.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     MI.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND MI.コースＣＤ = @移動元コースＣＤ" +
                                      "\r\n     AND MI.移動日 = @移動日" +
                                      "\r\n ORDER BY" +
                                      "\r\n     MI.相手コースＣＤ" +
                                      "\r\n     ,MI.ＳＥＱ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@移動元コースＣＤ", SqlDbType.Int).Value = _idoNyuryokuDetailCriteria.コースＣＤ;
                    cmd.Parameters.Add("@移動日", SqlDbType.Date).Value = _idoNyuryokuDetailCriteria.配送日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("モバイル_移動入力");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "モバイル_移動入力";

                    log.Info("モバイル_移動入力を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,   "相手コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN, "相手コース名");
                fpSpread1_Sheet1.BindDataColumn(IDOSU_COLUMN,       "個数");
                fpSpread1_Sheet1.BindDataColumn(CHECK_1_COLUMN,     "確認フラグ");
                fpSpread1_Sheet1.BindDataColumn(CHECK_2_COLUMN,     "相手確認フラグ");

                // 既存のレコードについては、商品CDとコースCDを変更不可にする(◆)
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                {
                    // 商品ＣＤ
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_CD_COLUMN].Locked = true;       // 入力不可にする(□)
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_CD_COLUMN].ForeColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_CD_COLUMN].BackColor = SystemColors.ControlLight;

                    // 商品の参照ボタンを取得
                    FarPoint.Win.Spread.CellType.ButtonCellType buttonCellType =
                        (FarPoint.Win.Spread.CellType.ButtonCellType)fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_REFERENCE_COLUMN].CellType;
                    if (buttonCellType == null)
                    {
                        buttonCellType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                        buttonCellType.Text = "参照";
                    }

                    // 参照ボタンを非活性（したように見せる）
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_REFERENCE_COLUMN].CanFocus = false;
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_REFERENCE_COLUMN].Locked = true;
                    buttonCellType.TextColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_REFERENCE_COLUMN].CellType = buttonCellType;

                    // 商品名（もともとReadOnly）
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_NAME_COLUMN].ForeColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.Cells[rowIndex, SYOHIN_NAME_COLUMN].BackColor = SystemColors.ControlLight;

                    // コースＣＤ
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Locked = true;       // 入力不可にする(□)
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].ForeColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].BackColor = SystemColors.ControlLight;

                    // コースの参照ボタンを取得
                    buttonCellType = (FarPoint.Win.Spread.CellType.ButtonCellType)fpSpread1_Sheet1.Cells[rowIndex, COURSE_REFERENCE_COLUMN].CellType;
                    if (buttonCellType == null)
                    {
                        buttonCellType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                        buttonCellType.Text = "参照";
                    }

                    // 参照ボタンを非活性（したように見せる）
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_REFERENCE_COLUMN].CanFocus = false;
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_REFERENCE_COLUMN].Locked = true;
                    buttonCellType.TextColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_REFERENCE_COLUMN].CellType = buttonCellType;

                    // コース名（もともとReadOnly）
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_NAME_COLUMN].ForeColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.Cells[rowIndex, COURSE_NAME_COLUMN].BackColor = SystemColors.ControlLight;
                }

                fpSpread1.Focus();
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

                MobileIdo mobileIdo = new MobileIdo(e.Row);

                // 商品CDの場合は商品名を取得
                if (e.Column.ColumnName == "商品ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["商品名略称"] = null;
                    if (mobileIdo.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)mobileIdo.商品ＣＤ);
                        if (syohin != null)
                        {
                            // 主要部品リストマスタに存在すること
                            // 移動元（のコースが持つ部署）の主要部品と、移動先（のコースが持つ部署）の主要部品で、双方ともに持つ部品に限り、移動ができる。
                            // 少なくとも移動元（のコースが持つ部署）の主要部品であることをここではチェックしている
                            SyuyoBuhin syuyoBuhin = SyuyoBuhin.GetSyuyoBuhin((int)_idoNyuryokuDetailCriteria.部署ＣＤ, (int)mobileIdo.商品ＣＤ);
                            if (syuyoBuhin != null)
                            {
                                e.Row["商品名略称"] = syohin.商品名略称;
                            }
                            else
                            {
                                e.Row.SetColumnError(e.Column, "商品が移動元コースの主要部品ではありません。");
                            }
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

                // コースCDの場合はコース名を取得
                if (e.Column.ColumnName == "相手コースＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["相手コース名"] = null;
                    if (mobileIdo.相手コースＣＤ != null)
                    {
                        Course course = Course.GetCourse((int)mobileIdo.相手コースＣＤ);
                        if (course != null)
                        {
                            e.Row["相手コース名"] = course.コース名;

                            // 移動元と同じ？
                            if (mobileIdo.相手コースＣＤ == _idoNyuryokuDetailCriteria.コースＣＤ)
                            {
                                e.Row.SetColumnError(e.Column, "移動元と同じコースは指定できません。");
                            }
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "コースが見つかりません。");
                        }
                    }
                }

                // 商品と相手コース(=移動先コース)が決定すれば、
                // その商品が移動元（のコースが持つ部署）の主要部品と、移動先（のコースが持つ部署）の主要部品に共通する部品であることをチェック
                if (e.Column.ColumnName == "商品ＣＤ" || e.Column.ColumnName == "相手コースＣＤ")
                {
                    // 商品CDとコースCDが入力されている？
                    if (mobileIdo.商品ＣＤ != null && mobileIdo.相手コースＣＤ != null)
                    {
                        // その両方にエラーが無い？
                        if (string.IsNullOrEmpty(e.Row.GetColumnError("商品ＣＤ")) &&
                            string.IsNullOrEmpty(e.Row.GetColumnError("相手コースＣＤ")))
                        {
                            bool ret = IsSyohinContainedBothCourse((int)mobileIdo.商品ＣＤ, (int)_idoNyuryokuDetailCriteria.コースＣＤ, (int)mobileIdo.相手コースＣＤ);
                            if (ret == false)
                            {
                                e.Row.SetColumnError("商品ＣＤ", "商品が移動先コースと共通する主要部品ではありません。");
                            }
                        }
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
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 指定された商品が、移動元と移動先のコース（が持つ部署）の主要部品に含まれているかどうかを返す
        /// </summary>
        /// <param name="syohinCd">商品CD</param>
        /// <param name="fromCourseCD">移動元コースCD</param>
        /// <param name="toCourseCD">移動先コースCD</param>
        /// <returns>指定された商品が、移動元と移動先のコース（が持つ部署）の主要部品に含まれていればtrue。それ以外はfalse。</returns>
        private bool IsSyohinContainedBothCourse(int syohinCd, int fromCourseCD, int toCourseCD)
        {
            try
            {
                bool ret = false;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     MOTO.商品ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         P.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         コースマスタ AS CM" +
                                      "\r\n         INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ" +
                                      "\r\n         INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND CM.コースＣＤ = @移動元コースＣＤ" +
                                      "\r\n         AND P.商品ＣＤ = @商品ＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n     ) AS MOTO" +
                                      // -- 双方の条件で検索した結果(=商品CD)で結合する
                                      "\r\n     INNER JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             P.商品ＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             コースマスタ AS CM" +
                                      "\r\n             INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ" +
                                      "\r\n             INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND CM.コースＣＤ = @移動先コースＣＤ" +
                                      "\r\n             AND P.商品ＣＤ = @商品ＣＤ" +
                                      "\r\n             AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n         ) AS SAKI" +
                                      "\r\n         ON MOTO.商品ＣＤ = SAKI.商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@移動元コースＣＤ", SqlDbType.Int).Value = fromCourseCD;
                    cmd.Parameters.Add("@移動先コースＣＤ", SqlDbType.Int).Value = toCourseCD;
                    cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = syohinCd;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ret = reader.Read();
                    }

                    // レコードが読めたかどうかを返す
                    return ret;
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
        /// 一括移動入力が変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // モバイル_移動入力を比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["モバイル_移動入力"];

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
            HashSet<string> syohinCourseSet = new HashSet<string>();    // 商品ＣＤとコースＣＤのセット（組み合わせがユニークかどうかの確認用）

            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["モバイル_移動入力"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    MobileIdo mobileIdo = new MobileIdo(dr);

                    // エラーがある？
                    if (mobileIdo.HasErrors)
                    {
                        dr.SetColumnError("個数", mobileIdo["個数"]);
                        hasError = true;
                        continue;
                    }

                    bool isInputed = true;          // 有効な商品ＣＤとコースＣＤが入力済みかどうか

                    // 既存のレコードについては、商品ＣＤとコースＣＤのチェックはスキップする（変更不可なので）(◆)
                    if (dr.RowState == DataRowState.Added)
                    {
                        // 商品ＣＤに該当するマスタが存在するか？
                        if (mobileIdo.商品ＣＤ == null)
                        {
                            dr.SetColumnError("商品ＣＤ", "商品ＣＤを入力してください。");
                            hasError = true;
                            isInputed = false;
                        }
                        else
                        {
                            Syohin syohin = Syohin.GetSyohin((int)mobileIdo.商品ＣＤ);
                            if (syohin == null)
                            {
                                dr.SetColumnError("商品ＣＤ", "商品が見つかりません。");
                                hasError = true;
                                isInputed = false;
                            }
                        }

                        // コースＣＤに該当するマスタが存在するか？
                        if (mobileIdo.相手コースＣＤ == null)
                        {
                            dr.SetColumnError("相手コースＣＤ", "コースＣＤを入力してください。");
                            hasError = true;
                            isInputed = false;
                        }
                        else
                        {
                            Course course = Course.GetCourse((int)mobileIdo.相手コースＣＤ);
                            if (course == null)
                            {
                                dr.SetColumnError("相手コースＣＤ", "コースが見つかりません。");
                                hasError = true;
                                isInputed = false;
                            }
                            else
                            {
                                // 移動元と同じ？
                                if (mobileIdo.相手コースＣＤ == _idoNyuryokuDetailCriteria.コースＣＤ)
                                {
                                    dr.SetColumnError("相手コースＣＤ", "移動元と同じコースは指定できません。");
                                    hasError = true;
                                    isInputed = false;
                                }
                            }
                        }

                        if (isInputed)
                        {
                            // その商品が移動元（のコースが持つ部署）の主要部品と、移動先（のコースが持つ部署）の主要部品に共通する部品か？
                            bool ret = IsSyohinContainedBothCourse((int)mobileIdo.商品ＣＤ, (int)_idoNyuryokuDetailCriteria.コースＣＤ, (int)mobileIdo.相手コースＣＤ);
                            if (ret == false)
                            {
                                dr.SetColumnError("商品ＣＤ", "商品が移動先コースと共通する主要部品ではありません。");
                                hasError = true;
                            }
                        }
                    }

                    // 移動数が入力されているか？
                    if (mobileIdo.個数 == null)
                    {
                        dr.SetColumnError("個数", "移動数を入力してください。");
                        hasError = true;
                    }

                    if (isInputed)
                    {
                        // 商品とコースの組み合わせがユニークか？
                        string key = string.Format("{0},{1}", mobileIdo.商品ＣＤ, mobileIdo.相手コースＣＤ);
                        if (syohinCourseSet.Contains(key))
                        {
                            dr.SetColumnError("商品ＣＤ", "商品とコースの組み合わせが重複しています。");
                            dr.SetColumnError("相手コースＣＤ", "商品とコースの組み合わせが重複しています。");
                            hasError = true;
                        }
                        else
                        {
                            syohinCourseSet.Add(key);
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
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                bool ret = false;
                Dictionary<int?, int> seqMap = new Dictionary<int?, int>();     // 移動先コースごとのＳＥＱ
                int seq;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        DataTable dt = ds.Tables["モバイル_移動入力"];

                        // まず削除対象を削除
                        historyProcess = History.ProcessType.Delete;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState == DataRowState.Deleted)
                            {
                                MobileIdo mobileIdo = new MobileIdo(dr);
                                ret = mobileIdo.Delete(con, tran);
                                if (ret == false)
                                {
                                    MessageBox.Show(this, mobileIdo.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                History.Save(con, tran, this.Text, "モバイル_移動入力", mobileIdo.Code, historyProcess);
                            }
                        }

                        // 続いて追加or更新
                        foreach (DataRow dr in dt.Rows)
                        {
                            MobileIdo mobileIdo = new MobileIdo(dr);
                            switch (dr.RowState)
                            {
                                case DataRowState.Added:        // 追加
                                    if (seqMap.ContainsKey(mobileIdo.相手コースＣＤ) == false)
                                    {
                                        seqMap[mobileIdo.相手コースＣＤ] = 1;
                                    }
                                    seq = seqMap[mobileIdo.相手コースＣＤ];
                                    mobileIdo.コースＣＤ = _idoNyuryokuDetailCriteria.コースＣＤ;
                                    mobileIdo.移動日 = _idoNyuryokuDetailCriteria.配送日;
                                    mobileIdo.ＳＥＱ = seq;
                                    ret = mobileIdo.Insert(con, tran);
                                    historyProcess = History.ProcessType.Create;
                                    seq++;
                                    seqMap[mobileIdo.相手コースＣＤ] = seq;
                                    break;

                                case DataRowState.Modified:     // 変更
                                case DataRowState.Unchanged:    // 変更なし→ＳＥＱのみ更新
                                    if (seqMap.ContainsKey(mobileIdo.相手コースＣＤ) == false)
                                    {
                                        seqMap[mobileIdo.相手コースＣＤ] = 1;
                                    }
                                    seq = seqMap[mobileIdo.相手コースＣＤ];
                                    mobileIdo.ＳＥＱ = seq;
                                    ret = mobileIdo.Update(con, tran);
                                    historyProcess = History.ProcessType.Update;
                                    seq++;
                                    seqMap[mobileIdo.相手コースＣＤ] = seq;
                                    break;

                                default:
                                    continue;
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, mobileIdo.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            History.Save(con, tran, this.Text, "モバイル_移動入力", mobileIdo.Code, historyProcess);
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("モバイル_移動入力マスタを{0}しました。", historyProcess.DisplayName());
                        MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (BentoException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        log.Error(ex);
                        History.Error(ex);
                        throw new BentoException(ex);
                    }
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            // データ取得
            GetData();
        }
        #endregion
    }
}

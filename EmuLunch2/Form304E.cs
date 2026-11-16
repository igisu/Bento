using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 短冊印刷設定
    /// </summary>
    public partial class Form304E : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // 集計設定
        private const int SYUKEI_PRINT_ORDER_COLUMN = 0;    // 印刷順列インデックス
        private const int SYUKEI_PRINT_NAME_COLUMN  = 1;    // 集計印刷名列インデックス
        private const int SYUKEI_SYOKUJI_KBN_COLUMN = 2;    // 食事区分列インデックス
        private const int SYUKEI_BAG_COLUMN         = 3;    // 袋列インデックス
        private const int SYUKEI_DELETE_COLUMN      = 4;    // 削除ボタン列インデックス

        // コース名変換設定
        private const int COURSE_CD_COLUMN         = 0;     // コースCD列インデックス
        private const int COURSE_REFERENCE_COLUMN  = 1;     // 参照ボタン列インデックス
        private const int COURSE_NAME_COLUMN       = 2;     // コース名列インデックス
        private const int COURSE_PRINT_NAME_COLUMN = 3;     // 印刷名列インデックス
        private const int COURSE_DELETE_COLUMN     = 4;     // 削除ボタン列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 短冊設定マスタ
        /// </summary>
        private TanzakuSettei _tanzakuSettei;

        /// <summary>
        /// 初期状態用短冊設定マスタ
        /// </summary>
        private TanzakuSettei _origin;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// スプレッドのコンボボックス（F2,Enter制御用）
        /// </summary>
        private FarPoint.Win.FpCombo _fpCombo;

        /// <summary>
        /// スプレッドのコンボボックスでEnterキーが押下されたか
        /// </summary>
        private bool _isEnterforFpCombo = false;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        public Form304E(int busyoCD)
        {
            try
            {
                InitializeComponent();

                // 部署に該当する短冊設定マスタを取得
                _tanzakuSettei = TanzakuSettei.GetTanzakuSettei(busyoCD);
                if (_tanzakuSettei == null)
                {
                    _IsNewRecord = true;
                    _tanzakuSettei = new TanzakuSettei();
                    _tanzakuSettei.部署ＣＤ = busyoCD;
                }
                else
                {
                    _IsNewRecord = false;
                }

                // 初期状態として保持
                _origin = new TanzakuSettei(_tanzakuSettei);
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
        private void Form304S_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_TanzakuSyukei.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_TanzakuSyukei.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_TanzakuSyukei.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_TanzakuCourse.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                im = fpSpread_TanzakuCourse.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_TanzakuCourse.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                tanzakuSetteiBindingSource.DataSource = _tanzakuSettei;

                // スプレッドの食事区分のコンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbSyokujiKubun = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetSyokujiKubun(cbSyokujiKubun);
                cbSyokujiKubun.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbSyokujiKubun.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread_TanzakuSyukei_Sheet1.Columns[SYUKEI_SYOKUJI_KBN_COLUMN].CellType = cbSyokujiKubun;

                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnSave.Text = "更新(F9)";
                }

                GetData();      // データ取得
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
        private void Form304S_Shown(object sender, EventArgs e)
        {
            txtBagPattern.Focus();      // 袋パターンにフォーカス
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form304S_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_TanzakuSyukei.EditMode == true)
                    {
                        fpSpread_TanzakuSyukei.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }
                    btnSave.PerformClick();
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
        private void Form304S_FormClosing(object sender, FormClosingEventArgs e)
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
                if (fpSpread_TanzakuSyukei.EditMode == true)
                {
                    fpSpread_TanzakuSyukei.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                else
                {
                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                }

                // 初期状態と異なる？
                if (HasChangeTanzakuSettei() || HasChangeTanzakuSyukeiSettei() || HasChangeTanzakuCourseName())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 短冊設定マスタの入力チェック
                            if (CanSaveTanzakuSettei() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 短冊集計設定マスタの入力チェック
                            if (CanSaveTanzakuSyukeiSettei() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 短冊コース名マスタの入力チェック
                            if (CanSaveTanzakuCourseName() == false)
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
        /// 集計設定Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
        /// 集計設定Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_TanzakuSyukei.EditMode)
                {
                    fpSpread_TanzakuSyukei.CancelCellEditing();
                }
                else
                {
                    fpSpread_TanzakuSyukei.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_TanzakuSyukei_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_TanzakuSyukei_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= SYUKEI_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_TanzakuSyukei_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_TanzakuSyukei_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_TanzakuSyukei_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_TanzakuSyukei.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_TanzakuSyukei_Sheet1.ActiveColumnIndex)
                {
                    case SYUKEI_PRINT_ORDER_COLUMN:     // 印刷順
                    case SYUKEI_PRINT_NAME_COLUMN:      // 集計印刷名
                        colIndex = fpSpread_TanzakuSyukei_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_TanzakuSyukei_Sheet1.Cells[fpSpread_TanzakuSyukei_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_TanzakuSyukei_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 集計設定Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_TanzakuSyukei.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TanzakuSyukei.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_TanzakuSyukei.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_TanzakuSyukei_Edit_KeyDown);
            }

            if (fpSpread_TanzakuSyukei.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread_TanzakuSyukei.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(fpSpread_TanzakuSyukei_FpCombo_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(fpSpread_TanzakuSyukei_FpCombo_DropDown);
            }
        }

        /// <summary>
        /// 集計設定Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_TanzakuSyukei.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_TanzakuSyukei, fpSpread_TanzakuSyukei_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_TanzakuSyukei.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_TanzakuSyukei_Edit_KeyDown);
            }

            if (fpSpread_TanzakuSyukei.EditingControl is FarPoint.Win.FpCombo)
            {
                // ドロップダウンについても各イベントのハンドルを解除します
                _fpCombo.KeyDown -= new KeyEventHandler(fpSpread_TanzakuSyukei_FpCombo_KeyDown);
                _fpCombo.DropDown -= new FarPoint.Win.DropDownEventHandler(fpSpread_TanzakuSyukei_FpCombo_DropDown);
            }
        }

        /// <summary>
        /// 集計設定Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_Edit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_TanzakuSyukei.EditMode)
                    {
                        fpSpread_TanzakuSyukei.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_TanzakuSyukei.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_TanzakuSyukei_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_TanzakuSyukei_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= SYUKEI_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_TanzakuSyukei_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_TanzakuSyukei_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_TanzakuSyukei_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_TanzakuSyukei.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 集計設定Spreadコンボボックスキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_FpCombo_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                // 編集モードに移行するF2キーが押下された場合に呼ばれた場合は、リストを表示する
                if (_isF2forSpreadEdit)
                {
                    _isF2forSpreadEdit = false;
                    _fpCombo.DroppedDown = true;
                }
                else
                {
                    // ↓キー押下されたことにするが、あまり意味が無い
                    //e.SuppressKeyPress = true;
                    //e.Handled = true;

                    // 編集モードから非編集モードへの移行なので、常にifが成立
                    if (fpSpread_TanzakuSyukei.EditMode)
                    {
                        fpSpread_TanzakuSyukei.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_TanzakuSyukei.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            //Enterキーが押された場合にフラグを立てる
            _isEnterforFpCombo = (e.KeyCode == Keys.Enter);
        }

        /// <summary>
        /// 集計設定Spreadコンボボックスドロップダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_FpCombo_DropDown(object sender, FarPoint.Win.DropDownEventArgs e)
        {
            //Enterキーを契機とする場合は、DropDown処理をキャンセルする
            if (_isEnterforFpCombo)
            {
                _isEnterforFpCombo = false;
                e.Cancel = true;
                // 次のセルへ移動
                SendKeys.Send("{Tab}");
            }
        }

        /// <summary>
        /// 集計設定Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 削除ボタン？
                if (e.Column == SYUKEI_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_TanzakuSyukei_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)tanzakuSyukeiBindingSource.Current).Row;
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
        /// 集計設定Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuSyukei_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)tanzakuSyukeiBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// コース名変換設定Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
        /// コース名変換設定Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_KeyDown(object sender, KeyEventArgs e)
        {
            // F1？
            if (e.KeyCode == Keys.F1)
            {
                OpenForm902BySpread();          // コース選択ダイアログを表示
            }

            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_TanzakuCourse.EditMode)
                {
                    fpSpread_TanzakuCourse.CancelCellEditing();
                }
                else
                {
                    fpSpread_TanzakuCourse.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TanzakuCourse.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_TanzakuCourse_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_TanzakuCourse_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= COURSE_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_TanzakuCourse_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_TanzakuCourse_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_TanzakuCourse_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_TanzakuCourse.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_TanzakuCourse_Sheet1.ActiveColumnIndex)
                {
                    case COURSE_CD_COLUMN:          // コースＣＤ
                    case COURSE_PRINT_NAME_COLUMN:  // 印刷名
                        colIndex = fpSpread_TanzakuCourse_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_TanzakuCourse_Sheet1.Cells[fpSpread_TanzakuCourse_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_TanzakuCourse_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// コース名変換設定Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_TanzakuCourse.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TanzakuCourse.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_TanzakuCourse.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_TanzakuCourse_Edit_KeyDown);
            }
        }

        /// <summary>
        /// コース名変換設定Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_TanzakuCourse.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_TanzakuCourse, fpSpread_TanzakuCourse_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_TanzakuCourse.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_TanzakuCourse_Edit_KeyDown);
            }
        }

        /// <summary>
        /// コース名変換設定Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_Edit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_TanzakuCourse.EditMode)
                    {
                        fpSpread_TanzakuCourse.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_TanzakuCourse.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_TanzakuCourse.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_TanzakuCourse_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_TanzakuCourse_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= COURSE_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_TanzakuCourse_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_TanzakuCourse_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_TanzakuCourse_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_TanzakuCourse.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// コース名変換設定Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 参照ボタン？
                if (e.Column == COURSE_REFERENCE_COLUMN)
                {
                    OpenForm902BySpread();
                }

                // 削除ボタン？
                if (e.Column == COURSE_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_TanzakuCourse_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)tanzakuCourseBindingSource.Current).Row;
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
        /// Spreadからコース選択ダイアログを表示する
        /// </summary>
        private void OpenForm902BySpread()
        {
            try
            {
                // コース選択ダイアログを表示
                Form902 formTokuisakiDlg = new Form902();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form902CourseCD_FormClosed);
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
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    int newColumnIndex = fpSpread_TanzakuCourse_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread_TanzakuCourse_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread_TanzakuCourse_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread_TanzakuCourse_Sheet1.SetActiveCell(fpSpread_TanzakuCourse_Sheet1.ActiveRowIndex, newColumnIndex);

                    Course course = form.SelectedCourse;
                    fpSpread_TanzakuCourse_Sheet1.Cells[fpSpread_TanzakuCourse_Sheet1.ActiveRowIndex, COURSE_CD_COLUMN].Value = course.コースＣＤ;   // コースCDセルに値を設定→dt_TanzakuCourse_ColumnChanged()が呼ばれる
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
        /// コース名変換設定Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_TanzakuCourse_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)tanzakuCourseBindingSource.Current).Row;
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
                // 短冊設定マスタの入力チェック
                if (CanSaveTanzakuSettei() == false)
                {
                    return;
                }

                // 短冊集計設定マスタの入力チェック
                if (CanSaveTanzakuSyukeiSettei() == false)
                {
                    return;
                }

                // 短冊コース名マスタの入力チェック
                if (CanSaveTanzakuCourseName() == false)
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
        /// データ取得
        /// </summary>
        private void GetData()
        {
            try
            {
                // 短冊集計設定マスタ取得
                GetTanzakuSyukeiSettei();

                // 短冊コース名マスタ取得
                GetTanzakuCourseName();
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
        /// 短冊集計設定マスタ取得
        /// </summary>
        private void GetTanzakuSyukeiSettei()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     TS.企業ＣＤ" +
                                      "\r\n     ,TS.部署ＣＤ" +
                                      "\r\n     ,TS.集計区分ＣＤ" +
                                      "\r\n     ,TS.集計印刷名" +
                                      "\r\n     ,TS.食事区分ＣＤ" +
                                      "\r\n     ,VS.食事区分名" +
                                      "\r\n     ,TS.袋計算フラグ" +
                                      "\r\n     ,TS.印刷順" +
                                      "\r\n     ,TS.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,TS.作成日" +
                                      "\r\n     ,TS.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,TS.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     短冊集計設定マスタ AS TS" +
                                      "\r\n     LEFT JOIN V_食事区分   AS VS ON VS.食事区分CD = TS.食事区分CD" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = TS.企業ＣＤ AND TS.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = TS.企業ＣＤ AND TS.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     TS.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND TS.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     TS.印刷順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _tanzakuSettei.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("短冊集計設定マスタ");
                    adp.Fill(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_TanzakuSyukei_ColumnChanged);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    tanzakuSyukeiBindingSource.DataSource = ds;
                    tanzakuSyukeiBindingSource.DataMember = "短冊集計設定マスタ";

                    log.Info("短冊集計設定マスタを検索しました。");
                }

                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_PRINT_ORDER_COLUMN, "印刷順");
                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_PRINT_NAME_COLUMN,  "集計印刷名");
                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_SYOKUJI_KBN_COLUMN, "食事区分ＣＤ");
                fpSpread_TanzakuSyukei_Sheet1.BindDataColumn(SYUKEI_BAG_COLUMN,         "袋計算フラグ");
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
        /// 集計設定データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TanzakuSyukei_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
        /// 短冊コース名マスタ取得
        /// </summary>
        private void GetTanzakuCourseName()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     TC.企業ＣＤ" +
                                      "\r\n     ,TC.コースＣＤ" +
                                      "\r\n     ,TC.コースＣＤ AS コースＣＤorg" +
                                      "\r\n     ,CM.コース名" +
                                      "\r\n     ,TC.短冊印刷コース名" +
                                      "\r\n     ,TC.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,TC.作成日" +
                                      "\r\n     ,TC.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,TC.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     短冊コース名マスタ AS TC" +
                                      "\r\n     INNER JOIN コースマスタ AS CM ON TC.企業ＣＤ = CM.企業ＣＤ AND TC.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ  AS A1 ON A1.企業ＣＤ = TC.企業ＣＤ AND TC.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ  AS A2 ON A2.企業ＣＤ = TC.企業ＣＤ AND TC.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     TC.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     TC.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("短冊コース名マスタ");
                    adp.Fill(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_TanzakuCourse_ColumnChanged);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    tanzakuCourseBindingSource.DataSource = ds;
                    tanzakuCourseBindingSource.DataMember = "短冊コース名マスタ";

                    log.Info("短冊コース名マスタを検索しました。");
                }

                fpSpread_TanzakuCourse_Sheet1.BindDataColumn(COURSE_CD_COLUMN,         "コースＣＤ");
                fpSpread_TanzakuCourse_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,       "コース名");
                fpSpread_TanzakuCourse_Sheet1.BindDataColumn(COURSE_PRINT_NAME_COLUMN, "短冊印刷コース名");
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
        /// コース名変換設定データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TanzakuCourse_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                _isDtColumnChanging = true;

                TanzakuCourseName tanzakuCourseName = new TanzakuCourseName(e.Row);

                // コースCDの場合はコース名を取得
                if (e.Column.ColumnName == "コースＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["コース名"] = null;
                    if (tanzakuCourseName.コースＣＤ != null)
                    {
                        Course course = Course.GetCourse((int)tanzakuCourseName.コースＣＤ);
                        if (course != null)
                        {
                            e.Row["コース名"] = course.コース名;
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "コースが見つかりません。");
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
        /// 短冊設定マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTanzakuSettei()
        {
            if (_tanzakuSettei.Equals(_origin) == false)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 短冊集計設定マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTanzakuSyukeiSettei()
        {
            // 短冊集計設定マスタを比較
            DataSet ds = (DataSet)tanzakuSyukeiBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["短冊集計設定マスタ"];

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
        /// 短冊コース名マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeTanzakuCourseName()
        {
            // 短冊コース名マスタを比較
            DataSet ds = (DataSet)tanzakuCourseBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["短冊コース名マスタ"];

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
        /// 短冊設定マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTanzakuSettei()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_tanzakuSettei.HasErrors)
                {
                    MessageBox.Show(this, _tanzakuSettei.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 短冊集計設定マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTanzakuSyukeiSettei()
        {
            bool hasError = false;
            bool isDuplicate = false;
            HashSet<int> syokujiKubunSet = new HashSet<int>();

            try
            {
                DataSet ds = (DataSet)tanzakuSyukeiBindingSource.DataSource;
                DataTable dt = ds.Tables["短冊集計設定マスタ"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    TanzakuSyukeiSettei tanzakuSyukeiSettei = new TanzakuSyukeiSettei(dr);

                    // エラーがある？
                    if (tanzakuSyukeiSettei.HasErrors)
                    {
                        dr.SetColumnError("印刷順", tanzakuSyukeiSettei["印刷順"]);
                        dr.SetColumnError("集計印刷名", tanzakuSyukeiSettei["集計印刷名"]);
                        dr.SetColumnError("食事区分ＣＤ", tanzakuSyukeiSettei["食事区分ＣＤ"]);
                        hasError = true;
                    }

                    // 食事区分が重複していればエラー
                    if (tanzakuSyukeiSettei.食事区分ＣＤ != null)
                    {
                        if (syokujiKubunSet.Contains((int)tanzakuSyukeiSettei.食事区分ＣＤ))
                        {
                            isDuplicate = true;
                            dr.SetColumnError("食事区分ＣＤ", "食事区分が重複しています。");
                        }
                        syokujiKubunSet.Add((int)tanzakuSyukeiSettei.食事区分ＣＤ);
                    }
                }

                if (hasError || isDuplicate)
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
        /// 短冊コース名マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveTanzakuCourseName()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)tanzakuCourseBindingSource.DataSource;
                DataTable dt = ds.Tables["短冊コース名マスタ"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    TanzakuCourseName tanzakuCourseName = new TanzakuCourseName(dr);

                    // エラーがある？
                    if (tanzakuCourseName.HasErrors)
                    {
                        dr.SetColumnError("コースＣＤ", tanzakuCourseName["コースＣＤ"]);
                        dr.SetColumnError("短冊印刷コース名", tanzakuCourseName["短冊印刷コース名"]);
                        hasError = true;
                    }
                    else
                    {
                        // コースＣＤに該当するマスタが存在するか？
                        if (tanzakuCourseName.コースＣＤ != null)
                        {
                            Course course = Course.GetCourse((int)tanzakuCourseName.コースＣＤ);
                            if (course == null)
                            {
                                dr.SetColumnError("コースＣＤ", "コースが見つかりません。");
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

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 短冊設定マスタ保存
                    if (SaveTanzakuSettei(con, tran) == false)
                    {
                        return false;
                    }

                    // 短冊集計設定マスタ保存
                    if (SaveTanzakuSyukeiSettei(con, tran) == false)
                    {
                        return false;
                    }

                    // 短冊コース名マスタ保存
                    if (SaveTanzakuCourseName(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("短冊設定マスタと短冊集計設定マスタと短冊コース名マスタを{0}しました。", historyProcess.DisplayName());
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
        /// 短冊設定マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTanzakuSettei(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _tanzakuSettei.Insert(con, tran);
                }
                else
                {
                    ret = _tanzakuSettei.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _tanzakuSettei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "短冊設定マスタ", _tanzakuSettei.Code, historyProcess);
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
        /// 短冊集計設定マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTanzakuSyukeiSettei(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)tanzakuSyukeiBindingSource.DataSource;
                DataTable dt = ds.Tables["短冊集計設定マスタ"];

                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    TanzakuSyukeiSettei tanzakuSyukeiSettei = new TanzakuSyukeiSettei(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            tanzakuSyukeiSettei.部署ＣＤ = _tanzakuSettei.部署ＣＤ;
                            // 空いている最小の集計区分ＣＤを取得
                            tanzakuSyukeiSettei.集計区分ＣＤ = TanzakuSyukeiSettei.GetSyukeiKubunCD(con, tran, (int)tanzakuSyukeiSettei.部署ＣＤ, fpSpread_TanzakuSyukei_Sheet1.Rows.Count - 1);
                            ret = tanzakuSyukeiSettei.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            break;

                        case DataRowState.Modified:     // 変更
                            ret = tanzakuSyukeiSettei.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = tanzakuSyukeiSettei.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, tanzakuSyukeiSettei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "短冊集計設定マスタ", tanzakuSyukeiSettei.Code, historyProcess);
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
        /// 短冊コース名マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveTanzakuCourseName(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)tanzakuCourseBindingSource.DataSource;
                DataTable dt = ds.Tables["短冊コース名マスタ"];

                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    TanzakuCourseName tanzakuCourseName = new TanzakuCourseName(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            ret = tanzakuCourseName.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            break;

                        case DataRowState.Modified:     // 変更
                            ret = tanzakuCourseName.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = tanzakuCourseName.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, tanzakuCourseName.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "短冊コース名マスタ", tanzakuCourseName.Code, historyProcess);
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                // 短冊設定マスタを取得
                _tanzakuSettei = TanzakuSettei.GetTanzakuSettei(_tanzakuSettei.部署ＣＤ);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                tanzakuSetteiBindingSource.DataSource = _tanzakuSettei;

                // 初期状態として保持
                _origin = new TanzakuSettei(_tanzakuSettei);

                GetData();      // データ取得

                _IsNewRecord = false;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
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

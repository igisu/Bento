using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emu.Bento
{
    /// <summary>
    /// 合算請求書印刷
    /// </summary>
    /// <remarks>
    /// Spreadを用いているのでBaseSpreadFormを継承したかったが、請求書の印刷もするのでSeikyuPrintFormを優先して継承しないといけない（多重継承はできない）。
    /// したがってBaseSpreadFormが持つメソッドは使わないことにした(★)
    /// </remarks>
    public partial class Form506 : SeikyuPrintForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEIKYU_NO_COLUMN      = 0;    // 請求番号列インデックス
        private const int REFERENCE_COLUMN      = 1;    // 参照ボタン列インデックス
        private const int SEIKYUBI_COLUMN       = 2;    // 請求日列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 3;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 4;    // 得意先名列インデックス
        private const int ZENKAI_SEIKYU_COLUMN  = 5;    // 前回請求額列インデックス
        private const int NYUKIN_COLUMN         = 6;    // 入金額列インデックス
        private const int KURIKOSHI_COLUMN      = 7;    // 繰越額列インデックス
        private const int KAIAGE_COLUMN         = 8;    // 買上額列インデックス
        private const int ZEI_COLUMN            = 9;    // 消費税列インデックス
        private const int KONKAI_SEIKYU_COLUMN  = 10;   // 今回請求額列インデックス
        private const int DELETE_COLUMN         = 11;   // 削除ボタン列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 請求書回収予定日印字フラグ
        /// </summary>
        private bool _isPrintSeikyuKaisyuYotei;

        /// <summary>
        /// 合算請求書
        /// </summary>
        private GassanSeikyusyo _gassanSeikyusyo;

        /// <summary>
        /// 請求書出力条件
        /// </summary>
        private SeikyuCriteria _seikyuCriteria;

        /// <summary>
        /// 帳票用合算請求書
        /// </summary>
        private DataTable _dtGassanSeikyusyo;

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
        /// <param name="seikyuNo">請求番号</param>
        public Form506(decimal seikyuNo)
        {
            try
            {
                InitializeComponent();

                // 合算請求書（のベースになる請求合計）を取得
                _dtGassanSeikyusyo = GetGassanSeikyusyo(seikyuNo);
                _gassanSeikyusyo = new GassanSeikyusyo(_dtGassanSeikyusyo.Rows[0]);
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
        private void Form506_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

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
                gassanSeikyusyoBindingSource.DataSource = _gassanSeikyusyo;

                _seikyuCriteria = new SeikyuCriteria();
                seikyuCriteriaBindingSource.DataSource = _seikyuCriteria;

                // 請求番号に該当する請求合計をスプレッドの１行目に読み込む
                GetSeikyuGokei((decimal)_gassanSeikyusyo.請求番号);
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
        private void Form506_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:                      // 印刷ボタン押下
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
                OpenForm907BySpread();          // 請求一覧ダイアログを表示
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
                    case SEIKYU_NO_COLUMN:      // 請求番号
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                    CheckDupSeikuiNo(); // fpSpread1_Changeイベントが呼ばれないので、CheckDupSeikuiNo()はここで呼ぶ
                    CalcTotal();        // fpSpread1_Changeイベントが呼ばれないので、CalcTotal()はここで呼ぶ
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
                //ParseDateText(fpSpread1, fpSpread1_Sheet1);   // (★)

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
                    OpenForm907BySpread();          // 請求一覧ダイアログを表示
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
                    CheckDupSeikuiNo(); // 請求番号の重複チェック
                    CalcTotal();        // 合算値計算
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
        /// Spreadから請求一覧ダイアログを表示する
        /// </summary>
        private void OpenForm907BySpread()
        {
            try
            {
                // 請求一覧ダイアログを表示
                Form907 formSeikyuDlg = new Form907();
                SubFormOpenInit(formSeikyuDlg);
                formSeikyuDlg.FormClosed += new FormClosedEventHandler(Form907SeikyuNo_FormClosed);
                formSeikyuDlg.ShowDialog();
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

        /// 請求一覧ダイアログクローズ（請求番号）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form907SeikyuNo_FormClosed(object sender, FormClosedEventArgs e)
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
                Form907 form = (Form907)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                    //int newColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                    //do
                    //{
                    //    newColumnIndex++;
                    //} while (fpSpread1_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                    //         fpSpread1_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    //fpSpread1_Sheet1.SetActiveCell(fpSpread1_Sheet1.ActiveRowIndex, newColumnIndex);

                    SeikyuGokei seikyuGokei = form.SelectedSeikyuGokei;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, SEIKYU_NO_COLUMN].Value = seikyuGokei.請求番号;   // 請求番号セルに値を設定→dt_ColumnChanged()が呼ばれる
                    CheckDupSeikuiNo(); // 請求番号の重複チェック
                    CalcTotal();        // 合算値計算
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
            CheckDupSeikuiNo(); // 請求番号の重複チェック
            CalcTotal();        // 合算値計算
        }

        /// <summary>
        /// Spreadセル移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            // 先頭行には移動しない
            if (e.NewRow == 0)
            {
                e.Cancel = true;
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
                if (_seikyuCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 請求番号にエラーがある？
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["請求合計"];
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr.GetColumnError("請求番号")) == false)
                    {
                        MessageBox.Show(this, "請求番号に不備があります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
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

                OutputPdf(pdfPath);         // PDF出力処理

                MessageBox.Show(this, "合算請求書のPDFを出力しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seikyuCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seikyuCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seikyuCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 請求番号にエラーがある？
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["請求合計"];
                foreach (DataRow dr in dt.Rows)
                {
                    if (string.IsNullOrEmpty(dr.GetColumnError("請求番号")) == false)
                    {
                        MessageBox.Show(this, "請求番号に不備があります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                // 2回連続してクリックすると、プレビューが二つ開いてしまう。
                // 印刷ボタンを非活性にすることでそれを回避する。
                btnPrint.Enabled = false;

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.Seikyusho, PrintMain);
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
                btnPrint.Enabled = true;
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
        /// 合算請求書取得
        /// </summary>
        /// <param name="seikyuNo">請求番号</param>
        private DataTable GetGassanSeikyusyo(decimal seikyuNo)
        {
            try
            {
                DataTable dt = null;

                // 請求書回収予定日印字フラグを取得
                _isPrintSeikyuKaisyuYotei = SystemMaster.IsPrintSeikyuKaisyuYotei();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     G.請求番号" +
                                      "\r\n     ,G.部署ＣＤ" +
                                      "\r\n     ,G.請求先名" +
                                      "\r\n     ,G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '様') AS 得意先名" +
                                      "\r\n     ,DATALENGTH(G.請求先名 + ' ' + ISNULL(VS.請求書敬称名, '')) AS 得意先名長さ" +
                                      "\r\n     ,G.前回請求額" +
                                      "\r\n     ,G.入金額" +
                                      "\r\n     ,G.繰越額" +
                                      "\r\n     ,G.買上金額" +
                                      "\r\n     ,G.調整額" +
                                      "\r\n     ,G.消費税" +
                                      "\r\n     ,G.今回請求額" +
                                      "\r\n     ,G.回収予定日" +
                                      "\r\n     ,@回収予定日印字フラグ AS 回収予定日印字フラグ" +
                                      "\r\n     ,T.郵便番号 AS 得意先郵便番号" +
                                      "\r\n     ,T.住所1 AS 得意先住所1" +
                                      "\r\n     ,T.住所2 AS 得意先住所2" +
                                      "\r\n     ,T.電話番号1 AS 得意先電話番号" +
                                      "\r\n     ,T.ＦＡＸ1 AS 得意先ＦＡＸ" +
                                      "\r\n     ,T.振込手数料フラグ" +
                                      "\r\n     ,T.請求書郵便マークフラグ" +
                                      "\r\n     ,ISNULL(B.請求書タイプ区分, @請求書タイプ区分明細) AS 請求書タイプ区分" +   // 明細をデフォルトとする
                                      "\r\n     ,B.会社名称 AS 自社名称" +
                                      "\r\n     ,B.郵便番号 AS 自社郵便番号" +
                                      "\r\n     ,B.住所 AS 自社住所" +
                                      "\r\n     ,B.電話番号 AS 自社電話番号" +
                                      "\r\n     ,B.ＦＡＸ AS 自社ＦＡＸ" +
                                      "\r\n     ,B.登録番号" +
                                      "\r\n     ,B.金融機関ＣＤ1" +
                                      "\r\n     ,KKM1.銀行名 AS 銀行名1" +
                                      "\r\n     ,B.金融機関支店ＣＤ1" +
                                      "\r\n     ,KKSM1.支店名 AS 支店名1" +
                                      "\r\n     ,B.口座種別1" +
                                      "\r\n     ,VK1.口座種別名 AS 口座種別名1" +
                                      "\r\n     ,B.口座番号1" +
                                      "\r\n     ,B.口座名義人1" +
                                      "\r\n     ,B.金融機関ＣＤ2" +
                                      "\r\n     ,KKM2.銀行名 AS 銀行名2" +
                                      "\r\n     ,B.金融機関支店ＣＤ2" +
                                      "\r\n     ,KKSM2.支店名 AS 支店名2" +
                                      "\r\n     ,B.口座種別2" +
                                      "\r\n     ,VK2.口座種別名 AS 口座種別名2" +
                                      "\r\n     ,B.口座番号2" +
                                      "\r\n     ,B.口座名義人2" +
                                      "\r\n     ,B.請求書備考1" +
                                      "\r\n     ,B.請求書備考2" +
                                      "\r\n     ,'' AS 合算対象請求番号" +      // 取得後のDataTableのカラムに文字列を格納するので、nullではなく空文字列(string)を取得している
                                      "\r\n FROM" +
                                      "\r\n     請求合計 AS G" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T" +
                                      "\r\n         ON  T.企業ＣＤ = G.企業ＣＤ" +
                                      "\r\n         AND T.得意先ＣＤ = G.請求先ＣＤ" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON" +
                                      "\r\n         B.企業ＣＤ = G.企業ＣＤ" +
                                      "\r\n         AND B.部署ＣＤ = G.部署ＣＤ" +
                                      "\r\n     LEFT JOIN V_請求書敬称 AS VS ON" +
                                      "\r\n         VS.請求書敬称CD = T.請求書敬称" +
                                      "\r\n     LEFT JOIN 金融機関名称 AS KKM1 ON" +
                                      "\r\n         KKM1.銀行ＣＤ = B.金融機関ＣＤ1" +
                                      "\r\n     LEFT JOIN 金融機関支店名称 AS KKSM1 ON" +
                                      "\r\n         KKSM1.銀行ＣＤ = B.金融機関ＣＤ1" +
                                      "\r\n         AND KKSM1.支店ＣＤ = B.金融機関支店ＣＤ1" +
                                      "\r\n     LEFT JOIN V_口座種別 AS VK1 ON" +
                                      "\r\n         VK1.口座種別CD = B.口座種別1" +
                                      "\r\n     LEFT JOIN 金融機関名称 AS KKM2 ON" +
                                      "\r\n         KKM2.銀行ＣＤ = B.金融機関ＣＤ2" +
                                      "\r\n     LEFT JOIN 金融機関支店名称 AS KKSM2 ON" +
                                      "\r\n         KKSM2.銀行ＣＤ = B.金融機関ＣＤ2" +
                                      "\r\n         AND KKSM2.支店ＣＤ = B.金融機関支店ＣＤ2" +
                                      "\r\n     LEFT JOIN V_口座種別 AS VK2 ON" +
                                      "\r\n         VK2.口座種別CD = B.口座種別2" +
                                      "\r\n WHERE" +
                                      "\r\n     G.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND G.請求番号 = @請求番号";

                    cmd.Parameters.Add("@回収予定日印字フラグ", SqlDbType.Bit).Value = _isPrintSeikyuKaisyuYotei;
                    cmd.Parameters.Add("@請求書タイプ区分明細", SqlDbType.Int).Value = (int)Consts.SeikyusyoType.Detail;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("合算請求書");
                    adp.Fill(dt);

                    log.Info("合算請求書を取得しました。");
                }

                return dt;
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
        /// 請求合計取得・表示
        /// </summary>
        /// <param name="seikyuNo">請求番号</param>
        private void GetSeikyuGokei(decimal seikyuNo)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataTable dt = GetSeikyuGokeiTable(seikyuNo);

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "請求合計";

                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN,      "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN,       "請求日");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(ZENKAI_SEIKYU_COLUMN,  "前回請求額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,         "入金額");
                fpSpread1_Sheet1.BindDataColumn(KURIKOSHI_COLUMN,      "繰越額");
                fpSpread1_Sheet1.BindDataColumn(KAIAGE_COLUMN,         "買上金額");
                fpSpread1_Sheet1.BindDataColumn(ZEI_COLUMN,            "消費税");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN,  "今回請求額");

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.Rows[0].BackColor = SystemColors.ControlLight;
                    fpSpread1_Sheet1.Rows[0].ForeColor = SystemColors.GrayText;
                    fpSpread1_Sheet1.ActiveRowIndex = 1;
                    fpSpread1_Sheet1.SetActiveCell(1, 0);
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
                fpSpread1.SuspendLayout();      // 請求番号を入力しても単価が描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                SeikyuGokei seikyuGokei = new SeikyuGokei(e.Row);

                // 商品CDの場合は商品マスタなどから取得
                if (e.Column.ColumnName == "請求番号")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["請求日"] = DBNull.Value;
                    e.Row["請求先ＣＤ"] = DBNull.Value;
                    e.Row["請求先名"] = null;
                    e.Row["前回請求額"] = DBNull.Value;
                    e.Row["入金額"] = DBNull.Value;
                    e.Row["繰越額"] = DBNull.Value;
                    e.Row["買上金額"] = DBNull.Value;
                    e.Row["調整額"] = DBNull.Value;
                    e.Row["消費税"] = DBNull.Value;
                    e.Row["今回請求額"] = DBNull.Value;
                    if (seikyuGokei.請求番号 != null)
                    {
                        DataTable dt = GetSeikyuGokeiTable((decimal)seikyuGokei.請求番号);
                        if (dt != null && dt.Rows.Count != 0)
                        {
                            e.Row["請求日"] = dt.Rows[0]["請求日"];
                            e.Row["請求先ＣＤ"] = dt.Rows[0]["請求先ＣＤ"];
                            e.Row["請求先名"] = dt.Rows[0]["請求先名"];
                            e.Row["前回請求額"] = dt.Rows[0]["前回請求額"];
                            e.Row["入金額"] = dt.Rows[0]["入金額"];
                            e.Row["繰越額"] = dt.Rows[0]["繰越額"];
                            e.Row["買上金額"] = dt.Rows[0]["買上金額"];
                            e.Row["調整額"] = dt.Rows[0]["調整額"];
                            e.Row["消費税"] = dt.Rows[0]["消費税"];
                            e.Row["今回請求額"] = dt.Rows[0]["今回請求額"];
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "請求番号が見つかりません。");
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
                fpSpread1.ResumeLayout(true);       // 請求番号を入力しても単価が描画されないことがあるのでそれを回避する
            }
        }

        /// <summary>
        /// 請求合計取得(データテーブル)
        /// </summary>
        /// <param name="seikyuNo">請求番号</param>
        /// <returns>請求合計テーブル</returns>
        private DataTable GetSeikyuGokeiTable(decimal seikyuNo)
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     企業ＣＤ" +
                                      "\r\n     ,請求番号" +
                                      "\r\n     ,請求日" +
                                      "\r\n     ,締区分" +
                                      "\r\n     ,部署ＣＤ" +
                                      "\r\n     ,請求先ＣＤ" +
                                      "\r\n     ,請求先名" +
                                      "\r\n     ,前回請求額" +
                                      "\r\n     ,入金額" +
                                      "\r\n     ,繰越額" +
                                      "\r\n     ,買上金額" +
                                      "\r\n     ,調整額" +
                                      "\r\n     ,消費税" +
                                      "\r\n     ,今回請求額" +
                                      "\r\n     ,回収予定日" +
                                      "\r\n     ,印刷者ＣＤ" +
                                      "\r\n     ,印刷日時" +
                                      "\r\n     ,EB作成済フラグ" +
                                      "\r\n     ,作成担当者ＣＤ" +
                                      "\r\n     ,作成日" +
                                      "\r\n     ,修正担当者ＣＤ" +
                                      "\r\n     ,修正日" +
                                      "\r\n FROM" +
                                      "\r\n     請求合計" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 請求番号 = @請求番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("請求合計");
                    adp.Fill(dt);

                    log.Info("請求合計を取得しました。");
                }

                return dt;
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
        /// 請求番号の重複チェック
        /// </summary>
        private void CheckDupSeikuiNo()
        {
            try
            {
                HashSet<decimal?> seikyuNoMap = new HashSet<decimal?>();      // 請求番号重複チェック用マップ
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["請求合計"];
                foreach (DataRow dr in dt.Rows)
                {
                    decimal? seikyuNo = dr["請求番号"] as decimal?;

                    // 請求番号が未入力？（消された？）
                    if (seikyuNo == null)
                    {
                        dr.SetColumnError("請求番号", null);
                        continue;
                    }

                    if (seikyuNoMap.Contains(seikyuNo))
                    {
                        dr.SetColumnError("請求番号", "請求番号が重複しています。");
                    }
                    else
                    {
                        seikyuNoMap.Add(seikyuNo);
                        // "重複しています"のエラーなら消す（"請求番号が見つかりません"エラーは消さない）
                        if (dr.GetColumnError("請求番号").StartsWith("請求番号が重複しています"))
                        {
                            dr.SetColumnError("請求番号", null);
                        }
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
        /// 合算値計算
        /// </summary>
        private void CalcTotal()
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["請求合計"];

                _gassanSeikyusyo.前回請求額 = dt.Compute("SUM(前回請求額)", "") as decimal?;
                _gassanSeikyusyo.入金額 = dt.Compute("SUM(入金額)", "") as decimal?;
                _gassanSeikyusyo.繰越額 = dt.Compute("SUM(繰越額)", "") as decimal?;
                _gassanSeikyusyo.買上金額 = dt.Compute("SUM(買上金額)", "") as decimal?;
                _gassanSeikyusyo.調整額 = dt.Compute("SUM(調整額)", "") as decimal?;
                _gassanSeikyusyo.消費税 = dt.Compute("SUM(消費税)", "") as decimal?;
                _gassanSeikyusyo.今回請求額 = dt.Compute("SUM(今回請求額)", "") as decimal?;

                gassanSeikyusyoBindingSource.ResetBindings(false);  // データソースの内容を画面に反映
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
        /// 印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintMain(string printerNameA4)
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

                // 帳票マージ開始
                PrintMergeInit(printerNameA4);

                //*****************
                // 合算請求書を印刷
                //*****************
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (_seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    _gassanSeikyusyo.回収予定日 = null;
                    _gassanSeikyusyo.回収予定日印字フラグ = _isPrintSeikyuKaisyuYotei;
                }
                else
                {
                    _gassanSeikyusyo.回収予定日印字フラグ = true;
                }

                // 前回請求額～今回請求額の合計値をデータテーブルに格納する
                _gassanSeikyusyo.SetDataRow(_dtGassanSeikyusyo.Rows[0]);
                DataTable dt = _dtGassanSeikyusyo.Copy();
                PrintGassanSeikyu(printerNameA4, dt);       // 合算請求書を印刷

                // **********
                // 請求書印刷
                // **********
                // ここから印刷される請求書については、回収予定日は印字しない。
                // PrintSeikyuWhole()には、コピーして「回収予定日を出力しない」を true にしたSeikyuCriteriaを渡す。
                SeikyuCriteria seikyuCriteria = new SeikyuCriteria(_seikyuCriteria);
                seikyuCriteria.回収予定日を出力しない = true;
                PrintSeikyuWhole(printerNameA4, (int)_gassanSeikyusyo.部署ＣＤ, seikyuPrintTargetList, true, false, seikyuCriteria, null);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// PDF出力処理
        /// </summary>
        /// <param name="pdfPath">PDF保存先フォルダ</param>
        private void OutputPdf(string pdfPath)
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

                // 帳票マージ開始
                PrintMergeInit(null);

                //*****************
                // 合算請求書を印刷
                //*****************
                // 「支払期限を印字しない」チェックボックスが
                // ONなら、枠とテキストは、システム設定マスタ.請求書回収予定日印字フラグ に依存する。
                // OFFなら、枠もテキストも回収予定日も印字する（システム設定マスタ.請求書回収予定日印字フラグは無視する）。
                if (_seikyuCriteria.回収予定日を出力しない ?? false)
                {
                    _gassanSeikyusyo.回収予定日 = null;
                    _gassanSeikyusyo.回収予定日印字フラグ = _isPrintSeikyuKaisyuYotei;
                }
                else
                {
                    _gassanSeikyusyo.回収予定日印字フラグ = true;
                }

                // 前回請求額～今回請求額の合計値をデータテーブルに格納する
                _gassanSeikyusyo.SetDataRow(_dtGassanSeikyusyo.Rows[0]);
                DataTable dt = _dtGassanSeikyusyo.Copy();
                PrintGassanSeikyu(null, dt);       // 合算請求書を印刷

                // **********
                // 請求書印刷
                // **********
                // ここから印刷される請求書については、回収予定日は印字しない。
                // PrintSeikyuWhole()には、コピーして「回収予定日を出力しない」を true にしたSeikyuCriteriaを渡す。
                SeikyuCriteria seikyuCriteria = new SeikyuCriteria(_seikyuCriteria);
                seikyuCriteria.回収予定日を出力しない = true;
                PrintSeikyuWhole(null, (int)_gassanSeikyusyo.部署ＣＤ, seikyuPrintTargetList, true, false, seikyuCriteria, pdfPath);
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
            finally
            {
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
                List<SeikyuPrintTarget> seikyuPrintTargetList = new List<SeikyuPrintTarget>();
                StringBuilder targetSeikyuNos = new StringBuilder();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();

                    // スプレッドの請求番号から、請求書印刷対象を取得する
                    DataSet ds = (DataSet)bindingSource1.DataSource;
                    DataTable dt = ds.Tables["請求合計"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        decimal? seikyuNo = dr["請求番号"] as decimal?;
                        if (seikyuNo == null)
                        {
                            continue;
                        }

                        // ついでに合算対象請求番号を作成
                        targetSeikyuNos.AppendFormat("－請求番号 {0}", seikyuNo).AppendLine();

                        // 印刷対象を取得
                        SqlCommand cmd = con.CreateCommand();
                        StringBuilder sql = new StringBuilder();
                        sql.Append("\r\n SELECT" +
                                   "\r\n     G.請求番号" +
                                   "\r\n     ,G.請求日" +
                                   "\r\n     ,G.請求先ＣＤ" +
                                   "\r\n     ,G.請求先名");

                        if (_seikyuCriteria.今回売上のみを出力 != true)
                        {
                            sql.Append("\r\n     ,G.今回請求額");
                        }
                        else
                        {
                            sql.Append("\r\n     ,ISNULL(G.買上金額, 0) + ISNULL(G.調整額, 0) + ISNULL(G.消費税, 0) AS 今回請求額");
                        }

                        sql.AppendLine("\r\n     ,ISNULL(B.請求書タイプ区分, @請求書タイプ区分明細) AS 請求書タイプ区分" +  // 明細をデフォルトとする
                                       "\r\n     ,C.コースＣＤ" +
                                       "\r\n     ,C.コース順" +
                                       "\r\n FROM" +
                                       "\r\n     請求合計 AS G" +
                                       // -- 分配先の売上は分配元として計上したいので、得意先ＣＤを分配元のそれにすり替える
                                       "\r\n     INNER JOIN 得意先マスタ AS S ON S.企業ＣＤ = G.企業ＣＤ AND S.得意先ＣＤ = G.請求先ＣＤ" +
                                       "\r\n     INNER JOIN 得意先マスタ AS J ON J.企業ＣＤ = S.企業ＣＤ AND J.得意先ＣＤ = ISNULL(S.受注得意先ＣＤ, S.得意先ＣＤ)" +
                                       "\r\n     INNER JOIN 部署マスタ AS B ON" +
                                       "\r\n         B.企業ＣＤ = G.企業ＣＤ" +
                                       "\r\n         AND B.部署ＣＤ = G.部署ＣＤ" +
                                       // -- ヒットした得意先が持つコースのうち最小値を取得する
                                       "\r\n     LEFT JOIN" +
                                       "\r\n         (SELECT" +
                                       "\r\n             CM.企業ＣＤ" +
                                       "\r\n             ,CM.部署ＣＤ" +
                                       "\r\n             ,CM.コース名" +
                                       "\r\n             ,CT1.コースＣＤ" +
                                       "\r\n             ,CT1.得意先ＣＤ" +
                                       "\r\n             ,CT1.コース順" +
                                       "\r\n         FROM" +
                                       "\r\n             コーステーブル AS CT1" +
                                       "\r\n             INNER JOIN" +
                                       "\r\n                 (SELECT" +
                                       "\r\n                     M.企業ＣＤ" +
                                       "\r\n                     ,M.部署ＣＤ" +
                                       "\r\n                     ,T.得意先ＣＤ" +
                                       "\r\n                     ,MIN(T.コースＣＤ) AS コースＣＤ" +
                                       "\r\n                 FROM" +
                                       "\r\n                     コースマスタ AS M" +
                                       "\r\n                     LEFT JOIN コーステーブル AS T ON" +
                                       "\r\n                         T.企業ＣＤ = M.企業ＣＤ" +
                                       "\r\n                         AND T.コースＣＤ = M.コースＣＤ" +
                                       "\r\n                 WHERE" +
                                       "\r\n                     ISNULL(T.無効フラグ, 0) = 0" +
                                       "\r\n                 GROUP BY" +
                                       "\r\n                     M.企業ＣＤ" +
                                       "\r\n                     ,M.部署ＣＤ" +
                                       "\r\n                     ,T.得意先ＣＤ" +
                                       "\r\n                 ) AS CT2" +
                                       "\r\n                     ON CT2.企業ＣＤ = CT1.企業ＣＤ" +
                                       "\r\n                     AND CT2.得意先ＣＤ = CT1.得意先ＣＤ" +
                                       "\r\n                     AND CT2.コースＣＤ = CT1.コースＣＤ" +
                                       "\r\n             INNER JOIN コースマスタ AS CM" +
                                       "\r\n                 ON CM.企業ＣＤ = CT1.企業ＣＤ" +
                                       "\r\n                 AND CM.コースＣＤ =  CT1.コースＣＤ" +
                                       "\r\n         ) AS C" +
                                       "\r\n             ON J.企業ＣＤ = C.企業ＣＤ" +
                                       "\r\n             AND J.得意先ＣＤ = C.得意先ＣＤ" +
                                       "\r\n WHERE" +
                                       "\r\n     G.企業ＣＤ = @企業ＣＤ" +
                                       "\r\n     AND G.部署ＣＤ = C.部署ＣＤ" +      // 請求の部署と一致するものに限る
                                       "\r\n     AND G.請求番号 = @請求番号");

                        cmd.Parameters.Add("@請求書タイプ区分明細", SqlDbType.Int).Value = (int)Consts.SeikyusyoType.Detail;
                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@請求番号", SqlDbType.Decimal).Value = seikyuNo;

                        cmd.CommandText = sql.ToString();

                        log.Debug(cmd.CommandTextWithParameters());

                        SqlDataAdapter adp = new SqlDataAdapter(cmd);

                        DataTable dtSeikyuPrintTarget = new DataTable();
                        adp.Fill(dtSeikyuPrintTarget);
                        // データテーブルの内容をリストに詰め直す
                        foreach (DataRow drSeikyuPrintTarget in dtSeikyuPrintTarget.Rows)
                        {
                            SeikyuPrintTarget seikyuPrintTarget = new SeikyuPrintTarget(drSeikyuPrintTarget);
                            seikyuPrintTargetList.Add(seikyuPrintTarget);
                        }
                    }
                }

                // 作成した合算対象請求番号を設定
                _gassanSeikyusyo.合算対象請求番号 = targetSeikyuNos.ToString();

                return seikyuPrintTargetList;
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
        /// 合算請求書を印刷します
        /// </summary>
        /// <param name="printerName">プリンタ名</param>
        /// <param name="dt">合算請求書印刷用データテーブル</param>
        private void PrintGassanSeikyu(string printerName, DataTable dt)
        {
            byte[] logoImageBytes = null;   // 指定した部署のロゴ画像のバイト配列
            byte[] inkanImageBytes = null;  // 指定した部署CDの印鑑画像のバイト配列

            try
            {
                // ロゴ・印鑑画像を取得
                BusyoImage busyoImage = BusyoImage.GetBusyoImage((int)_gassanSeikyusyo.部署ＣＤ);
                logoImageBytes = busyoImage.ロゴ;
                inkanImageBytes = busyoImage.印影;

                // 銀行名と支店名から作る金融機関名列を追加
                dt.Columns.Add("金融機関名称1", typeof(string));
                dt.Columns.Add("金融機関名称2", typeof(string));

                // ロゴ・印鑑画像の列を追加
                dt.Columns.Add("ロゴ画像", typeof(byte[]));
                dt.Columns.Add("印鑑画像", typeof(byte[]));

                // 全ての行に対して…
                foreach (DataRow dr in dt.Rows)
                {
                    // 銀行名と支店名から金融機関名を作る
                    MakeBankName(dr, "銀行名1", "支店名1", "金融機関名称1");
                    MakeBankName(dr, "銀行名2", "支店名2", "金融機関名称2");

                    // ロゴ・印鑑画像の列に画像を設定
                    dr["ロゴ画像"] = logoImageBytes;
                    dr["印鑑画像"] = inkanImageBytes;
                }

                // 帳票タイプで分岐
                switch (_gassanSeikyusyo.請求書タイプ区分)
                {
                     case (int)Consts.SeikyusyoType.Detail:         // 明細
                     case (int)Consts.SeikyusyoType.CalendarTate:   // カレンダー縦
                        Report506TateA report506TateA = new Report506TateA();
                        report506TateA.TopOffsetCM = _topOffset;
                        report506TateA.LeftOffsetCM = _leftOffset;
                        report506TateA.DataSource = dt;
                        report506TateA.Document.Printer.DocumentName = string.Format("合算請求書 - {0}({1})", _gassanSeikyusyo.請求先名, _gassanSeikyusyo.請求番号);
                        if (string.IsNullOrEmpty(printerName) == false)
                        {
                            report506TateA.Document.Printer.PrinterName = printerName;
                        }
                        report506TateA.Run(false);

                        // 追加先のActiveReportsドキュメントにページを追加
                        PrintMergeAdd(report506TateA);
                        break;

                     case (int)Consts.SeikyusyoType.CalendarYoko:    // カレンダー横
                        Report506YokoA report506YokoA = new Report506YokoA();
                        report506YokoA.TopOffsetCM = _topOffset;
                        report506YokoA.LeftOffsetCM = _leftOffset;
                        report506YokoA.DataSource = dt;
                        report506YokoA.Document.Printer.DocumentName = string.Format("合算請求書 - {0}({1})", _gassanSeikyusyo.請求先名, _gassanSeikyusyo.請求番号);
                        if (string.IsNullOrEmpty(printerName) == false)
                        {
                            report506YokoA.Document.Printer.PrinterName = printerName;
                        }
                        report506YokoA.Run(false);

                        // 追加先のActiveReportsドキュメントにページを追加
                        PrintMergeAdd(report506YokoA);
                        break;
                }

                string code = string.Format("請求番号=[{0}]", _gassanSeikyusyo.請求番号);
                History.Save(this.Text, "合算請求書", code, History.ProcessType.Output);

                log.InfoFormat("合算請求書を印刷しました。請求番号={0}", _gassanSeikyusyo.請求番号);
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

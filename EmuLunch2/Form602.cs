using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 入金EBデータ取込
    /// </summary>
    public partial class Form602 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEIKYU_NO_COLUMN      = 0;    // 請求番号列インデックス
        private const int NYUKINBI_COLUMN       = 1;    // 入金日列インデックス
        private const int BUSYO_NAME_COLUMN     = 2;    // 部署列インデックス
        private const int TOKUISAKI_CD_COLUMN   = 3;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 4;    // 得意先名列インデックス
        private const int SEIKYU_COLUMN         = 5;    // 請求額列インデックス
        private const int NYUKIN_COLUMN         = 6;    // 入金額列インデックス
        private const int NYUKIN_TEKIYO_COLUMN  = 7;    // 入金摘要区分列インデックス
        private const int ZAN_COLUMN            = 8;    // 売掛残列インデックス

        private const string SEIKYU_COLUMN_NAME = "F";  // スプレッドの請求額列名
        private const string NYUKIN_COLUMN_NAME = "G";  // スプレッドの入金額列名

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 入金EBデータ取込条件
        /// </summary>
        private NyukinEBImport _nyukinEBImport;

        /// <summary>
        /// 入金EBデータ取込条件検索実行時スナップショット
        /// </summary>
        private NyukinEBImport _nyukinEBImportSnapShot;

        /// <summary>
        /// 請求EBデータ（パース結果）
        /// </summary>
        private EBContainer _container;

        /// <summary>
        /// 請求番号ごとの入金額マップ
        /// </summary>
        private Dictionary<decimal, decimal> _nyukinMap;

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
        public Form602()
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
        private void Form602_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.coins_ico;

                lblCount.Text = "";

                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthBrowse;

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

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbTekiyo = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.Set(cbTekiyo,
                    new string[] { "", "1月分", "2月分", "3月分", "4月分", "5月分", "6月分", "7月分", "8月分", "9月分", "10月分", "11月分", "12月分" },
                    new int[] { -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
                cbTekiyo.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbTekiyo.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[NYUKIN_TEKIYO_COLUMN].CellType = cbTekiyo;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _nyukinEBImport = new NyukinEBImport();
                nyukinImportBindingSource.DataSource = _nyukinEBImport;

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
        private void Form602_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:                       // 登録ボタン押下
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form602_Shown(object sender, EventArgs e)
        {
            // 入金日にフォーカス
            dtpNyukinbi.Focus();
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
                if (_nyukinEBImport.HasErrors)
                {
                    if (string.IsNullOrEmpty(_nyukinEBImport.Error) == false)
                    {
                        MessageBox.Show(this, _nyukinEBImport.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 入金日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_nyukinEBImport.入金日 > warnDate)
                {
                    if (MessageBox.Show(this, "入金日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // EB先銀行支店ＣＤは出力先EB口座リストボックスにバインドしていない（※）ので、ここで取得する
                // ※部署を選択し直すとリストの先頭が選択状態になり、部署を確定すると以前に記憶していたEB先銀行支店ＣＤが選択状態になる(選択位置が変化する)ので
                _nyukinEBImport.EB先銀行支店ＣＤ = (string)lstBusyoEB.SelectedValue;

                // EB先銀行支店ＣＤが未選択？
                if (string.IsNullOrEmpty(_nyukinEBImport.EB先銀行支店ＣＤ))
                {
                    MessageBox.Show(this, "ＥＢ口座を選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                bindingSource1.DataSource = null;   // 表示している内容をリセットして、スプレッドの表示が崩れるのを回避する
                _nyukinMap = new Dictionary<decimal, decimal>();    // 入金額マップ初期化

                // ファイル読み込み
                if (InputEBData(txtFile.Text) == false)
                {
                    return;
                }

                // 請求データ取得・表示
                ShowSeikyuData();
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
        /// Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyDown(object sender, KeyEventArgs e)
        {
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
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= ZAN_COLUMN)
                    {
                        newColumnIndex = SEIKYU_NO_COLUMN;
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
                    case NYUKIN_TEKIYO_COLUMN:      // 入金摘要区分
                        colIndex = -1;
                        break;

                    default:
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
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
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(fpSpread1Edit_KeyDown);
            }

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread1.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
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

            if (fpSpread1.EditingControl is FarPoint.Win.FpCombo)
            {
                // ドロップダウンについても各イベントのハンドルを解除します
                _fpCombo.KeyDown -= new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown -= new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
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
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= ZAN_COLUMN)
                    {
                        newColumnIndex = SEIKYU_NO_COLUMN;
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
        /// Spreadコンボボックスキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_KeyDown(object sender, KeyEventArgs e)
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

            //Enterキーが押された場合にフラグを立てる
            _isEnterforFpCombo = (e.KeyCode == Keys.Enter);
        }

        /// <summary>
        /// Spreadコンボボックスドロップダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FpCombo_DropDown(object sender, FarPoint.Win.DropDownEventArgs e)
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
                if (CanSave() == false)
                {
                    return;
                }

                if (Save())
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
                        _container = new EBContainer(_nyukinEBImport.銀行ＣＤ, reader.ReadToEnd());
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
        /// 請求データ表示
        /// </summary>
        private void ShowSeikyuData()
        {
            try
            {
                DataTable dt;
                List<decimal> seikyuNoList = new List<decimal>();                                   // (正常に振替済みの)請求番号のリスト
                SortedDictionary<string, int> failedCountMap = new SortedDictionary<string, int>(); // 正常に振り替えできなかったEBデータの結果コードとその件数

                // 振替結果を判定
                foreach (Dictionary<string, string> map in _container.DataList)
                {
                    string resultCD = map["振替結果コード"];
                    // 正常？
                    if (resultCD == "0")
                    {
                        seikyuNoList.Add(decimal.Parse(map["予備"]));                               // (正常に振替済みの)請求番号のリストに追加
                        _nyukinMap[decimal.Parse(map["予備"])] = decimal.Parse(map["引落金額"]);    // 請求番号ごとに入金額(=請求金額)を保持する
                    }
                    else
                    {
                        // 異常の場合は、その原因ごとに件数を保持する
                        if (failedCountMap.ContainsKey(resultCD) == false)
                        {
                            failedCountMap.Add(resultCD, 1);
                        }
                        else
                        {
                            failedCountMap[resultCD]++;
                        }
                    }
                }

                // 異常がある場合はそれを表示
                if (failedCountMap.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("振替に失敗しているデータがあります。");
                    foreach (KeyValuePair<string, int> kvp in failedCountMap)
                    {
                        sb.AppendFormat("{0}:{1}件", EBContainer.GetReuslt(kvp.Key), kvp.Value).AppendLine();
                    }
                    log.Warn(sb.ToString());
                    MessageBox.Show(this, sb.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // 正常が1件も無い場合はここで終了
                if (seikyuNoList.Count == 0)
                {
                    log.Warn("取り込む入金データ無し");
                    MessageBox.Show(this, "取り込む入金データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendFormat("     CAST('{0}' AS DATETIME) as 入金日", _nyukinEBImport.入金日);
                    sql.AppendLine("     ,A.請求番号");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,A.請求先ＣＤ");
                    sql.AppendLine("     ,A.請求先名");
                    sql.AppendLine("     ,A.今回請求額");
                    sql.AppendLine("     ,CAST (0 AS DECIMAL) AS 入金額");
                    sql.AppendLine("     ,NULL AS 入金摘要区分");
                    sql.AppendLine("     ,CAST (0 AS DECIMAL) AS 売掛残");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     請求合計 AS A");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND A.請求番号 IN (");

                    // 請求番号のリストから生成
                    string comma = "";
                    foreach (decimal seikyuNo in seikyuNoList)
                    {
                        sql.AppendFormat("{0}{1}", comma, seikyuNo);
                        comma = ",";
                    }

                    sql.AppendLine(")");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     A.請求番号");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("請求合計");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    // 取得したレコードの入金額を設定
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["入金額"] = (decimal)_nyukinMap[(decimal)dr["請求番号"]];
                    }

                    dt.AcceptChanges();       // 入金額の変更をコミット

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "請求合計";
                }

                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN,      "請求番号");
                fpSpread1_Sheet1.BindDataColumn(NYUKINBI_COLUMN,       "入金日");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,     "部署名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_COLUMN,         "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,         "入金額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_TEKIYO_COLUMN,  "入金摘要区分");

                // 売掛残を数式で表す
                // 同時に入金額の合計をラベルに表示する
                decimal amount = 0;
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.SetFormula(rowIndex, ZAN_COLUMN, string.Format("{0}{2}-{1}{2}", SEIKYU_COLUMN_NAME, NYUKIN_COLUMN_NAME, rowIndex + 1));
                    amount += (decimal)fpSpread1_Sheet1.Cells[rowIndex, NYUKIN_COLUMN].Value;
                }
                lblNyukinTotal.Text = amount.ToString("N0");

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.ActiveColumnIndex = NYUKIN_TEKIYO_COLUMN;
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _nyukinEBImportSnapShot = new NyukinEBImport(_nyukinEBImport);

                // 請求EBデータには存在するが、請求合計には存在しない請求番号を抽出
                // 該当する請求番号があれば汎用レポート画面を使って表示
                ShowLostSeikyuNo(dt);
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
        /// 登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSave()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_nyukinEBImport.HasErrors)
                {
                    MessageBox.Show(this, _nyukinEBImport.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_nyukinEBImportSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_nyukinEBImport.Equals(_nyukinEBImportSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {

                this.MdiParent.Cursor = Cursors.WaitCursor;

                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    DataSet ds = (DataSet)bindingSource1.DataSource;
                    DataTable dt = ds.Tables["請求合計"];

                    decimal newNyukinNo = -1;       // 追加用入金番号

                    foreach (DataRow dr in dt.Rows)
                    {
                        Nyukin nyukin = new Nyukin();

                        SqlCommand cmdMax = con.CreateCommand();
                        cmdMax.Transaction = tran;
                        cmdMax.CommandText = "SELECT ISNULL(MAX(入金番号), 0) + 1 as 入金番号 FROM 入金明細 WHERE 企業ＣＤ = @企業ＣＤ";
                        cmdMax.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        using (SqlDataReader reader = cmdMax.ExecuteReader())
                        {
                            reader.Read();
                            newNyukinNo = (decimal)reader["入金番号"];
                        }

                        nyukin.入金番号 = newNyukinNo;
                        nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.EB;
                        nyukin.入金日 = _nyukinEBImport.入金日;
                        nyukin.部署ＣＤ = dr["部署ＣＤ"] as int?;
                        nyukin.得意先ＣＤ = dr["請求先ＣＤ"] as int?;
                        nyukin.得意先名 = dr["請求先名"] as string;
                        nyukin.依頼人ＣＤ = null;
                        nyukin.振込依頼人名 = null;
                        nyukin.入金方法 = (int)Consts.NyukinHoho.Furikae;
                        nyukin.入金額 = dr["入金額"] as decimal?;
                        nyukin.入金摘要区分 = dr["入金摘要区分"] as int?;
                        nyukin.対応請求番号 = dr["請求番号"] as decimal?;
                        nyukin.備考 = lstBusyoEB.Text;

                        bool ret = nyukin.Insert(con, tran);
                        if (ret == false)
                        {
                            MessageBox.Show(this, nyukin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        History.Save(con, tran, this.Text, "入金明細", nyukin.Code, History.ProcessType.Create);
                    }

                    tran.Commit();
                    log.Info("入金明細テーブルを登録しました。");
                    MessageBox.Show(this, "登録しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                finally
                {
                    this.MdiParent.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// 請求EBデータには存在するが、請求合計には存在しない請求番号を抽出・表示
        /// </summary>
        /// <param name="dt">請求合計データテーブル</param>
        private void ShowLostSeikyuNo(DataTable dt)
        {
            try
            {
                StringBuilder lostSeikyu = new StringBuilder();

                // 請求EBデータのデータレコードをループ
                foreach (Dictionary<string, string> map in _container.DataList)
                {
                    string resultCD = map["振替結果コード"];
                    // 正常ではない？
                    if (resultCD != "0")
                    {
                        continue;
                    }

                    // 請求番号に該当するレコードを抽出し、無ければリストに追加
                    decimal seikyuNo = decimal.Parse(map["予備"]);
                    DataRow[] drs  = dt.Select(string.Format("請求番号={0}", seikyuNo));
                    if (drs == null || drs.Length == 0)
                    {
                        decimal nyukin = decimal.Parse(map["引落金額"]);    // 入金額を取得

                        // 得意先を取得
                        string s_tokuisakiCD = map["顧客番号"];
                        int tokuisakiCD;
                        string tokuisakiName = "";
                        if (int.TryParse(s_tokuisakiCD, out tokuisakiCD))
                        {
                            s_tokuisakiCD = tokuisakiCD.ToString();
                            Tokuisaki tokuisaki = Tokuisaki.GetTokuisakiReload(tokuisakiCD);
                            if (tokuisaki != null)
                            {
                                tokuisakiName = tokuisaki.得意先名;
                            }
                        }
                        // 得意先ＣＤ、得意先名、請求番号、入金額
                        lostSeikyu.AppendFormat("{0}\t{1}\t{2}\t{3}", s_tokuisakiCD, tokuisakiName, seikyuNo, nyukin).AppendLine();
                    }
                }

                if (lostSeikyu.Length != 0)
                {
                    StringBuilder content = new StringBuilder();
                    content.AppendLine("以下の入金データは請求データが存在しないので入金処理ができません。");
                    content.AppendLine("入金入力画面で入金してください。").AppendLine();
                    content.AppendLine("得意先ＣＤ\t得意先名\t請求番号\t入金額");
                    content.Append(lostSeikyu.ToString());

                    Form980 form = new Form980(string.Format("{0}アラート", this.Text), Properties.Resources.coins_ico, content.ToString());
                    SubFormOpenInit(form);
                    form.Show();
                    form.FormClosed += new FormClosedEventHandler(Form980_FormClosed);
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
        /// レポート画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form980_FormClosed(object sender, FormClosedEventArgs e)
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

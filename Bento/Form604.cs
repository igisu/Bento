using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 銀行APIファイル取込
    /// </summary>
    public partial class Form604 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int IRAININ_COLUMN             = 0;    // 振込依頼人名列インデックス
        private const int TOKUISAKI_REFERENCE_COLUMN = 1;    // 得意先選択ボタン列インデックス
        private const int TOKUISAKI_COLUMN           = 2;    // 得意先コンボ列インデックス
        private const int BUSYO_NAME_COLUMN          = 3;    // 部署名コンボ列インデックス
        private const int TORIKOMI_NYUKIN_COLUMN     = 4;    // 取込入金額列インデックス
        private const int TESURYO_COLUMN             = 5;    // 手数料列インデックス
        private const int FUTAN_COLUMN               = 6;    // 振込手数料負担チェックボックス列インデックス
        private const int ZENKAI_SEIKYU_COLUMN       = 7;    // 前回請求額列インデックス
        private const int KONKAI_SEIKYU_COLUMN       = 8;    // 今回請求額列インデックス
        private const int SEIKYU_GO_NYUKIN_COLUMN    = 9;    // 請求後入金列インデックス
        private const int ZAN_COLUMN                 = 10;   // 売掛残列インデックス
        private const int NYUKINBI_COLUMN            = 11;   // 入金日列インデックス
        private const int TORIHIKI_NAME_COLUMN       = 12;   // 取引名列インデックス
        private const int INVALID_COLUMN             = 13;   // 無効フラグ列インデックス

        private const string KONKAI_SEIKYU_COLUMN_NAME    = "I";    // スプレッドの今回請求額列名
        private const string SEIKYU_GO_NYUKIN_COLUMN_NAME = "J";    // スプレッドの請求後入金列名
        private const string TORIKOMI_NYUKIN_COLUMN_NAME  = "E";    // スプレッドの取込入金額列名
        private const string TESURYO_COLUMN_NAME          = "F";    // スプレッドの手数料列名

        /// <summary>
        /// 振込依頼人名から得意先が抽出できなかった行の背景色
        /// </summary>
        private readonly Color TOKUISAKI_NOT_FOUND_BACK_COLOR = Color.Yellow;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 銀行APIファイル取込条件
        /// </summary>
        private AnserApiImportCriteria _anserApiImportCriteria;

        /// <summary>
        /// 銀行APIファイル取込条件検索実行時スナップショット
        /// </summary>
        private AnserApiImportCriteria _anserApiImportCriteriaSnapShot;

        /// <summary>
        /// ANSER-APIファイル（パース結果）
        /// </summary>
        private AnserApiContainer _container;

        /// <summary>
        /// ANSER-APIファイル読込結果が格納されたデータテーブル格納用データセット
        /// </summary>
        private DataSet _dsApiRecord = new DataSet();

        /// <summary>
        /// ANSER-APIファイル読込結果が格納されたデータテーブル
        /// </summary>
        private DataTable _dtApiRecord;

        /// <summary>
        /// 振込依頼人名に該当する得意先マスタのマップ
        /// </summary>
        private Dictionary<int, Tokuisaki> _tokuisakiMap = new Dictionary<int, Tokuisaki>();

        /// <summary>
        /// 部署ＣＤをキーとして部署名を値とするマップ
        /// </summary>
        private Dictionary<int, string> _busyoNameMap = null;

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
        public Form604()
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
        private void Form604_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.coins_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                ComboUtil.Set(cboTekiyo,
                    new string[] { "", "1月分", "2月分", "3月分", "4月分", "5月分", "6月分", "7月分", "8月分", "9月分", "10月分", "11月分", "12月分" },
                    new int[] { -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

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

                // 取込入金額までを固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = TORIKOMI_NYUKIN_COLUMN + 1;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _anserApiImportCriteria = new AnserApiImportCriteria();
                anserApiImportCriteria.DataSource = _anserApiImportCriteria;
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
        private void Form604_KeyDown(object sender, KeyEventArgs e)
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
        /// 取込ファイル参照ボタン
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
                if (_anserApiImportCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_anserApiImportCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _anserApiImportCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                bindingSource1.DataSource = null;   // 表示している内容をリセットして、スプレッドの表示が崩れるのを回避する

                // 金融機関名称をクリア
                anserApiContainerBindingSource.DataSource = new AnserApiContainer();

                // ファイル読み込み
                if (InputAnserApi(txtFile.Text) == false)
                {
                    return;
                }

                // 金融機関名称を表示
                anserApiContainerBindingSource.DataSource = _container;

                // ファイル読み込み結果と請求・入金データ取得・表示
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
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= INVALID_COLUMN)
                    {
                        newColumnIndex = IRAININ_COLUMN;
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
                    if (newColumnIndex >= INVALID_COLUMN)
                    {
                        newColumnIndex = IRAININ_COLUMN;
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
        /// Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 参照ボタン？
                if (e.Column == TOKUISAKI_REFERENCE_COLUMN)
                {
                    OpenForm900BySpread();
                }

                // 振込手数料負担チェックボックス？
                if (e.Column == FUTAN_COLUMN)
                {
                    // デフォルトでは別の行・列・セルへ移動しないとスプレッドからデータソースへの値が反映されない。
                    // そのため、別の行・列・セルへの移動を待たずに、カレントレコードの振込手数料フラグの値を強制的に変える
                    DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                    AnserApiRecord anserApiRecord = new AnserApiRecord(dr);
                    if (fpSpread1_Sheet1.Cells[e.Row, FUTAN_COLUMN].Value as bool? == true)
                    {
                        anserApiRecord.振込手数料フラグ = true;
                    }
                    else
                    {
                        anserApiRecord.振込手数料フラグ = false;
                    }

                    CalcTesuryo(anserApiRecord);        // 手数料を算出

                    anserApiRecord.SetDataRow(dr);      // クラスの内容をDataRowにセット
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
                        fpSpread1_Sheet1.Rows[e.Row].ResetForeColor();
                        // 得意先の選択肢が無い場合は、背景色は黄色
                        FarPoint.Win.Spread.CellType.ComboBoxCellType cbOption = (FarPoint.Win.Spread.CellType.ComboBoxCellType)fpSpread1_Sheet1.Cells[e.Row, TOKUISAKI_COLUMN].CellType;
                        if (cbOption.Items == null || cbOption.Items.Length == 0)
                        {
                            fpSpread1_Sheet1.Rows[e.Row].BackColor = TOKUISAKI_NOT_FOUND_BACK_COLOR;
                        }
                        else
                        {
                            fpSpread1_Sheet1.Rows[e.Row].ResetBackColor();
                        }
                    }

                    CalcTotal();        // 合計値を算出
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

                    // コンボボックスの選択肢にこの得意先を追加
                    FarPoint.Win.Spread.CellType.ComboBoxCellType cbOption = (FarPoint.Win.Spread.CellType.ComboBoxCellType)fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, TOKUISAKI_COLUMN].CellType;

                    List<string> itemList = new List<string>();         // 表示用リスト
                    List<string> itemDataList = new List<string>();     // 値用リスト

                    // 現在の選択肢の内容をいったんリストに取り込む
                    itemList.AddRange(cbOption.Items);
                    itemDataList.AddRange(cbOption.ItemData);

                    // 選択した得意先が既に選択肢に含まれていれば追加しない
                    if (itemDataList.Contains(tokuisaki.得意先ＣＤ.ToString()) == false)
                    {
                        itemList.Add(tokuisaki.得意先名);
                        itemDataList.Add(tokuisaki.得意先ＣＤ.ToString());

                        cbOption.Items = itemList.ToArray();
                        cbOption.ItemData = itemDataList.ToArray();

                        // 背景色を初期化
                        fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.ActiveRowIndex].ResetBackColor();
                    }

                    // 得意先マスタのマップにも追加
                    if (_tokuisakiMap.ContainsKey((int)tokuisaki.得意先ＣＤ) == false)
                    {
                        _tokuisakiMap[(int)tokuisaki.得意先ＣＤ] = tokuisaki;
                    }

                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, TOKUISAKI_COLUMN].Value = tokuisaki.得意先ＣＤ;   // 得意先CDセルに値を設定→dt_ColumnChanged()が呼ばれる
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
                if (CanSave() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (Save())
                {
                    // 画面をクリア
                    // 取り込んだファイル名を見せたままにしたい、また、摘要を変えて同じファイルを再度取り込むことが有り得るので、ファイル名はクリアしない

                    _anserApiImportCriteriaSnapShot = null;     // 検索未実行にする

                    _dtApiRecord = new DataTable();             // ANSER-APIファイル読込結果が格納されたデータテーブルをクリア（念のため）

                    anserApiContainerBindingSource.DataSource = new AnserApiContainer();    // 金融機関名称をクリア
                    _container = null;

                    bindingSource1.DataSource = null;           // 表示している内容をリセット
                    fpSpread1_Sheet1.Rows.Count = 0;            // 数式を消すため、全行削除する

                    lblNyukinTotal.Text = "";                   // 取込入金額合計をクリア
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
        /// 銀行APIファイル入力
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>正常終了の場合はtrue</returns>
        private bool InputAnserApi(string filePath)
        {
            try
            {
                if (File.Exists(filePath) == false)
                {
                    MessageBox.Show(this, "ファイルが見つかりません - " + filePath, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                _container = null;
                using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    try
                    {
                        _container = new AnserApiContainer(reader.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                log.InfoFormat("取込ファイル:{0}", filePath);
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
        /// ファイル読み込み結果と請求・入金データ取得・表示
        /// </summary>
        private void GetData()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 空のレコード群を取得する
                _dtApiRecord = GetEmptyRecords();
                _dsApiRecord.Tables.Clear();
                _dsApiRecord.Tables.Add(_dtApiRecord);

                // _container.AnserApiDetailListの件数分のDataRowを作成
                foreach (AnserApiDetail detail in _container.AnserApiDetailList)
                {
                    DataRow dr = _dtApiRecord.NewRow();
                    AnserApiRecord record = new AnserApiRecord(dr);
                    record.振込依頼人名 = detail.摘要;
                    record.取込入金額 = detail.金額;
                    record.取引名 = detail.取引名;
                    record.入金日 = detail.取扱日付;
                    record.SetDataRow(dr);
                    _dtApiRecord.Rows.Add(dr);
                }

                // 読込結果をスプレッドに表示（同時に得意先コンボボックスの選択肢も生成し、請求額・入金額も取得）
                ShowData();

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.ActiveColumnIndex = TOKUISAKI_COLUMN;
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _anserApiImportCriteriaSnapShot = new AnserApiImportCriteria(_anserApiImportCriteria);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// ANSER-APIファイル読込結果を格納するデータテーブルを取得する
        /// </summary>
        /// <returns>ANSER-APIファイル読込結果を格納するデータテーブル</returns>
        private DataTable GetEmptyRecords()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // データ型を取得するだけなので、取得内容にあまり意味は無い
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     D.振込依頼人名" +
                                      "\r\n     ,D.得意先ＣＤ" +
                                      "\r\n     ,D.得意先名" +
                                      "\r\n     ,D.部署ＣＤ" +
                                      "\r\n     ,D.部署名" +
                                      "\r\n     ,D.取込入金額" +
                                      "\r\n     ,D.手数料" +
                                      "\r\n     ,D.振込手数料フラグ" +
                                      "\r\n     ,D.前回請求額" +
                                      "\r\n     ,D.今回請求額" +
                                      "\r\n     ,D.請求後入金" +
                                      "\r\n     ,(D.今回請求額 - D.請求後入金) AS 売掛残高" +
                                      "\r\n     ,D.入金日" +
                                      "\r\n     ,D.取引名" +
                                      "\r\n     ,D.無効フラグ" +
                                      "\r\n     ,D.請求番号" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         F.振込依頼人名" +
                                      "\r\n         ,A.請求番号" +
                                      "\r\n         ,A.前回請求額 AS 取込入金額" +
                                      "\r\n         ,A.前回請求額 AS 手数料" +
                                      "\r\n         ,A.前回請求額" +
                                      "\r\n         ,A.今回請求額" +
                                      "\r\n         ,A.請求日 AS 入金日" +
                                      "\r\n         ,T.得意先ＣＤ" +
                                      "\r\n         ,T.得意先名" +
                                      "\r\n         ,T.振込手数料フラグ" +
                                      "\r\n         ,T.得意先名 AS 取引名" +
                                      "\r\n         ,T.無効フラグ" +
                                      "\r\n         ,B.部署ＣＤ" +
                                      "\r\n         ,B.部署名" +
                                      // -- 入金明細から入金額の合計を取得する相関サブクエリ（スカラサブクエリ）
                                      "\r\n         ,(SELECT" +
                                      "\r\n             ISNULL(SUM(N.入金額), 0) AS 入金額" +
                                      "\r\n         FROM" +
                                      "\r\n             入金明細 AS N" +
                                      "\r\n         WHERE" +
                                      "\r\n             N.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n             AND N.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n             AND N.得意先ＣＤ = A.請求先ＣＤ" +
                                      "\r\n             AND N.入金日 > A.請求日" +
                                      "\r\n         ) AS 請求後入金" +
                                      "\r\n     FROM" +
                                      "\r\n         得意先マスタ AS T" +
                                      "\r\n         LEFT JOIN 請求合計 AS A ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.請求先ＣＤ" +
                                      "\r\n         LEFT JOIN 得意先振込依頼人名 AS F ON T.企業ＣＤ = F.企業ＣＤ AND T.得意先ＣＤ = F.得意先ＣＤ" +
                                      "\r\n         LEFT JOIN 部署マスタ AS B ON T.企業ＣＤ = B.企業ＣＤ AND T.部署ＣＤ = B.部署ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND T.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     ) AS D";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = -1;        // 常に空のレコードを取得

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("APIファイル読み込み結果");
                    adp.Fill(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);
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

                AnserApiRecord anserApiRecord = new AnserApiRecord(e.Row);

                // 得意先ＣＤの場合
                if (e.Column.ColumnName == "得意先ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);

                    anserApiRecord.部署ＣＤ = null;
                    anserApiRecord.部署名 = null;
                    anserApiRecord.前回請求額 = null;
                    anserApiRecord.今回請求額 = null;
                    anserApiRecord.請求後入金 = null;
                    anserApiRecord.手数料 = null;
                    anserApiRecord.振込手数料フラグ = false;
                    anserApiRecord.請求番号 = null;
                    anserApiRecord.得意先名 = null;

                    if (anserApiRecord.得意先ＣＤ != null)
                    {
                        // 振込手数料負担チェックボックスに反映
                        Tokuisaki tokuisaki = _tokuisakiMap[(int)anserApiRecord.得意先ＣＤ];
                        anserApiRecord.振込手数料フラグ = tokuisaki.振込手数料フラグ;

                        // 得意先名も保持する
                        anserApiRecord.得意先名 = tokuisaki.得意先名;

                        // 得意先が持つ部署を取得
                        anserApiRecord.部署ＣＤ = tokuisaki.部署ＣＤ;
                        if (anserApiRecord.部署ＣＤ != null)
                        {
                            anserApiRecord.部署名 = GetBusyoName((int)anserApiRecord.部署ＣＤ);
                        }

                        // 前回請求額、今回請求額、請求後入金を取得
                        GetSeikyu(anserApiRecord);

                        // 請求合計に複数の部署があれば警告
                        if (HasMultiBusyo(anserApiRecord))
                        {
                            e.Row.SetColumnError(e.Column, "部署が異なる請求が存在します。入金先の部署が正しくない場合は入金入力画面を使用してください。");
                        }
                    }

                    // 得意先マスタ.振込手数料フラグ（負担させない）がONの場合は、手数料を算出する
                    CalcTesuryo(anserApiRecord);
                }

                // クラスの内容をDataRowにセット
                anserApiRecord.SetDataRow(e.Row);
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
        /// ANSER-APIファイル読込結果を表示
        /// </summary>
        private void ShowData()
        {
            try
            {
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                bindingSource1.DataSource = null;           // 表示している内容をリセットして、スプレッドの表示が崩れるのを回避する
                bindingSource1.DataSource = _dsApiRecord;
                bindingSource1.DataMember = _dtApiRecord.TableName;

                fpSpread1_Sheet1.BindDataColumn(IRAININ_COLUMN,         "振込依頼人名");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_COLUMN,       "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,      "部署名");
                fpSpread1_Sheet1.BindDataColumn(TORIKOMI_NYUKIN_COLUMN, "取込入金額");
                fpSpread1_Sheet1.BindDataColumn(TESURYO_COLUMN,         "手数料");
                fpSpread1_Sheet1.BindDataColumn(FUTAN_COLUMN,           "振込手数料フラグ");
                fpSpread1_Sheet1.BindDataColumn(ZENKAI_SEIKYU_COLUMN,   "前回請求額");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN,   "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_GO_NYUKIN_COLUMN,"請求後入金");
                fpSpread1_Sheet1.BindDataColumn(NYUKINBI_COLUMN,        "入金日");
                fpSpread1_Sheet1.BindDataColumn(TORIHIKI_NAME_COLUMN,   "取引名");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN,         "無効フラグ");

                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    // 振込依頼人名に該当する得意先をすべて取得
                    string irianinName = fpSpread1_Sheet1.Cells[rowIndex, IRAININ_COLUMN].Value.ToString();
                    List<Tokuisaki> tokuisakiList = GetTokuisakList(irianinName);

                    // 得意先コンボボックスの選択肢を作成する
                    SetSpreadCombo(rowIndex, tokuisakiList);

                    // 選択肢が一つだけの場合は、それを選択状態にする
                    if (tokuisakiList.Count == 1)
                    {
                        Tokuisaki tokuisaki = tokuisakiList[0];
                        fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_COLUMN].Value = tokuisaki.得意先ＣＤ;   // dt_ColumnChangedが発生
                    }

                    // 選択肢が抽出できなかった場合は背景色を変える
                    if (tokuisakiList.Count == 0)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = TOKUISAKI_NOT_FOUND_BACK_COLOR;
                    }

                    // 売掛残を数式で表す
                    fpSpread1_Sheet1.SetFormula(rowIndex, ZAN_COLUMN,
                                                string.Format("{0}{4}-{1}{4}-({2}{4}+{3}{4})",
                                                KONKAI_SEIKYU_COLUMN_NAME, SEIKYU_GO_NYUKIN_COLUMN_NAME,
                                                TORIKOMI_NYUKIN_COLUMN_NAME, TESURYO_COLUMN_NAME, rowIndex + 1));

                    // 得意先が選択されていて、売掛残がマイナスの場合は、デフォルトで無効にする
                    if (fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_COLUMN].Value != null &&
                        decimal.Parse(fpSpread1_Sheet1.Cells[rowIndex, ZAN_COLUMN].Value.ToString()) < 0m)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value = true;
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                }

                CalcTotal();        // 合計値を算出
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
        /// 振込依頼人名に該当する得意先のリストを返す
        /// </summary>
        /// <param name="irianinName">振込依頼人名</param>
        /// <returns>振込依頼人名に該当する得意先のリスト</returns>
        private List<Tokuisaki> GetTokuisakList(string irianinName)
        {
            try
            {
                List<Tokuisaki> tokuisakiList = new List<Tokuisaki>();

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     得意先ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     得意先振込依頼人名" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 振込依頼人名 = @振込依頼人名" +
                                      "\r\n ORDER BY" +
                                      "\r\n     得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@振込依頼人名", SqlDbType.VarChar).Value = irianinName;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int tokuisakiCD = (int)reader["得意先ＣＤ"];
                            Tokuisaki tokuisaki = null;
                            // 得意先ＣＤが取得済みマップに存在すれば、それを取得する
                            if (_tokuisakiMap.ContainsKey(tokuisakiCD))
                            {
                                tokuisaki = _tokuisakiMap[tokuisakiCD];
                            }
                            else
                            {
                                // マップに無い場合は追加
                                tokuisaki = Tokuisaki.GetTokuisaki(tokuisakiCD);
                                if (tokuisaki != null)
                                {
                                    _tokuisakiMap[tokuisakiCD] = tokuisaki;
                                }
                            }
                            // 返却するリストに追加
                            if (tokuisaki != null)
                            {
                                tokuisakiList.Add(tokuisaki);
                            }
                        }
                    }
                }

                return tokuisakiList;
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
        /// 得意先コンボボックスの選択肢を作成する
        /// </summary>
        /// <param name="rowIndex">対象行</param>
        /// <param name="tokuisakiList">振込依頼人名に該当する得意先のリスト</param>
        private void SetSpreadCombo(int rowIndex, List<Tokuisaki> tokuisakiList)
        {
            try
            {
                // セルのCellTypeを取得
                // できない場合は生成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbOption = (FarPoint.Win.Spread.CellType.ComboBoxCellType)fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_COLUMN].CellType;
                if (cbOption == null)
                {
                    cbOption = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                    cbOption.AutoCompleteSource = AutoCompleteSource.ListItems;
                    cbOption.AutoCompleteMode = AutoCompleteMode.Suggest;
                    fpSpread1_Sheet1.Cells[rowIndex, TOKUISAKI_COLUMN].CellType = cbOption;
                }

                List<string> itemList = new List<string>();         // 表示用リスト
                List<string> itemDataList = new List<string>();     // 値用リスト

                foreach (Tokuisaki tokuisaki in tokuisakiList)
                {
                    itemList.Add(tokuisaki.得意先名);
                    itemDataList.Add(tokuisaki.得意先ＣＤ.ToString());
                }

                cbOption.Items = itemList.ToArray();
                cbOption.ItemData = itemDataList.ToArray();
                cbOption.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;
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
        /// 得意先に該当する請求額・入金額を取得
        /// </summary>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        private void GetSeikyu(AnserApiRecord anserApiRecord)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.請求番号" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,A.前回請求額" +
                                      "\r\n     ,A.今回請求額" +
                                      // -- 入金明細から入金額の合計を取得する相関サブクエリ（スカラサブクエリ）
                                      "\r\n     ,(SELECT" +
                                      "\r\n         ISNULL(SUM(N.入金額), 0) AS 入金額" +
                                      "\r\n     FROM" +
                                      "\r\n         入金明細 AS N" +
                                      "\r\n     WHERE" +
                                      "\r\n         N.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n         AND N.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n         AND N.得意先ＣＤ = A.請求先ＣＤ" +
                                      "\r\n         AND N.入金日 > A.請求日" +
                                      "\r\n     ) AS 請求後入金" +
                                      "\r\n FROM" +
                                      "\r\n     請求合計 AS A" +
                                      "\r\n     INNER JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             G.企業ＣＤ" +
                                      "\r\n             ,G.部署ＣＤ" +
                                      "\r\n             ,G.請求先ＣＤ" +
                                      "\r\n             ,MAX(G.請求日) AS 請求日" +
                                      "\r\n         FROM" +
                                      "\r\n             請求合計 AS G" +
                                      "\r\n             INNER JOIN 得意先マスタ AS T" +
                                      "\r\n                 ON G.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n                 AND G.請求先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n                 AND G.部署ＣＤ = T.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             G.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND G.請求先ＣＤ = @得意先ＣＤ" +
                                      "\r\n             AND G.請求日 < @入金日" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             G.企業ＣＤ" +
                                      "\r\n             ,G.部署ＣＤ" +
                                      "\r\n             ,G.請求先ＣＤ" +
                                      "\r\n         ) AS SG" +
                                      "\r\n         ON A.企業ＣＤ = SG.企業ＣＤ" +
                                      "\r\n         AND A.部署ＣＤ = SG.部署ＣＤ" +
                                      "\r\n         AND A.請求先ＣＤ = SG.請求先ＣＤ" +
                                      "\r\n         AND A.請求日 = SG.請求日" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = anserApiRecord.得意先ＣＤ;
                    cmd.Parameters.Add("@入金日", SqlDbType.DateTime).Value = anserApiRecord.入金日;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            anserApiRecord.前回請求額 = (decimal)reader["前回請求額"];
                            anserApiRecord.今回請求額 = (decimal)reader["今回請求額"];
                            anserApiRecord.請求後入金 = (decimal)reader["請求後入金"];
                            anserApiRecord.請求番号 = (decimal)reader["請求番号"];
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
        /// 得意先に該当する請求合計に部署が複数存在するかどうかを返す
        /// </summary>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        /// <returns>得意先に該当する請求合計の部署が複数ある場合はtrue。</returns>
        private bool HasMultiBusyo(AnserApiRecord anserApiRecord)
        {
            try
            {
                int cnt = 0;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT DISTINCT" +
                                      "\r\n     A.部署ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     請求合計 AS A" +
                                      "\r\n     INNER JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             G.企業ＣＤ" +
                                      "\r\n             ,G.部署ＣＤ" +
                                      "\r\n             ,G.請求先ＣＤ" +
                                      "\r\n             ,MAX(G.請求日) AS 請求日" +
                                      "\r\n         FROM" +
                                      "\r\n             請求合計 AS G" +
                                      "\r\n             INNER JOIN 得意先マスタ AS T" +
                                      "\r\n                 ON G.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n                 AND G.請求先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n                 AND G.部署ＣＤ = T.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             G.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND G.請求先ＣＤ = @得意先ＣＤ" +
                                      "\r\n             AND G.請求日 < @入金日" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             G.企業ＣＤ" +
                                      "\r\n             ,G.部署ＣＤ" +
                                      "\r\n             ,G.請求先ＣＤ" +
                                      "\r\n         ) AS SG" +
                                      "\r\n         ON A.企業ＣＤ = SG.企業ＣＤ" +
                                      "\r\n         AND A.請求先ＣＤ = SG.請求先ＣＤ" +
                                      "\r\n         AND YEAR(A.請求日) = YEAR(SG.請求日)" +
                                      "\r\n         AND MONTH(A.請求日) = MONTH(SG.請求日)";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = anserApiRecord.得意先ＣＤ;
                    cmd.Parameters.Add("@入金日", SqlDbType.DateTime).Value = anserApiRecord.入金日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("請求合計");
                    adp.Fill(dt);
                    cnt = dt.Rows.Count;
                }

                return (1 < cnt);
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
        /// 合計値を算出
        /// </summary>
        private void CalcTotal()
        {
            try
            {
                decimal amount = 0;
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    // 無効以外の取込入金額を集計
                    if (fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value as bool? != true)
                    {
                        amount += (decimal)fpSpread1_Sheet1.Cells[rowIndex, TORIKOMI_NYUKIN_COLUMN].Value;
                    }
                }
                lblNyukinTotal.Text = amount.ToString("N0");
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
        /// 手数料を算出する
        /// </summary>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        private void CalcTesuryo(AnserApiRecord anserApiRecord)
        {
            try
            {
                // 得意先マスタ.振込手数料フラグ（負担させない）がONの場合は、手数料を算出する
                if ((anserApiRecord.振込手数料フラグ ?? false) == true)
                {
                    // 今回請求額と取込入金額の差が1000未満であれば、それを手数料とする
                    decimal tesuryo = (anserApiRecord.今回請求額 ?? 0m) - (anserApiRecord.取込入金額 ?? 0m);
                    if (tesuryo < 1000)
                    {
                        if (tesuryo < 0m) tesuryo = 0m;     // マイナスの手数料は０にする
                        anserApiRecord.手数料 = tesuryo;
                    }
                }
                else
                {
                    anserApiRecord.手数料 = null;
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
                if (_anserApiImportCriteria.HasErrors)
                {
                    MessageBox.Show(this, _anserApiImportCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_anserApiImportCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_anserApiImportCriteria.Equals(_anserApiImportCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                int validCount = 0;
                foreach (DataRow dr in _dtApiRecord.Rows)
                {
                    AnserApiRecord anserApiRecord = new AnserApiRecord(dr);

                    // 無効？
                    if (anserApiRecord.無効フラグ == true)
                    {
                        validCount++;
                    }
                    else
                    {
                        // 得意先が未選択？
                        if (anserApiRecord.得意先ＣＤ == null)
                        {
                            MessageBox.Show(this, string.Format("振込依頼欄名が「{0}」の得意先が選択されていません。", anserApiRecord.振込依頼人名), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }

                // すべてが無効？
                if (validCount == _dtApiRecord.Rows.Count)
                {
                    MessageBox.Show(this, "有効なレコードがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {

                this.MdiParent.Cursor = Cursors.WaitCursor;

                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    foreach (DataRow dr in _dtApiRecord.Rows)
                    {
                        AnserApiRecord anserApiRecord = new AnserApiRecord(dr);

                        if (anserApiRecord.無効フラグ == true)
                        {
                            continue;
                        }

                        // 振込依頼人名に該当する依頼人ＣＤを得意先振込依頼人名テーブルから取得
                        int irianinCD = GetFurikomiIraiNinCD(con, tran, anserApiRecord);
                        if (irianinCD < 0)
                        {
                            return false;
                        }

                        // 入金明細テーブル保存
                        if (SaveNyukin(con, tran, anserApiRecord, irianinCD) == false)
                        {
                            return false;
                        }
                    }

                    tran.Commit();
                    log.Info("入金明細テーブルを登録しました。");
                    MessageBox.Show(this, "登録しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                finally
                {
                    this.MdiParent.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// 振込依頼人名に該当する依頼人ＣＤを得意先振込依頼人名テーブルから取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        /// <returns>依頼人ＣＤ（エラーの場合は-1）</returns>
        private int GetFurikomiIraiNinCD(SqlConnection con, SqlTransaction tran, AnserApiRecord anserApiRecord)
        {
            try
            {
                // 得意先ＣＤと振込依頼人名に該当する得意先振込依頼人名テーブルを検索
                TokuisakiFurikomiIraiNin tokuisakiFurikomiIraiNin = GetTokuisakiFurikomiIraiNin(con, tran, anserApiRecord);

                // 存在する場合はその依頼人ＣＤを返す
                if (tokuisakiFurikomiIraiNin != null)
                {
                    return (int)tokuisakiFurikomiIraiNin.依頼人ＣＤ;
                }

                // 存在しない場合は追加して再検索
                if (AddTokuisakiFurikomiIraiNin(con, tran, anserApiRecord) == false)
                {
                    return -1;
                }

                tokuisakiFurikomiIraiNin = GetTokuisakiFurikomiIraiNin(con, tran, anserApiRecord);
                return (int)tokuisakiFurikomiIraiNin.依頼人ＣＤ;
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
        /// 得意先振込依頼人名テーブル取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        /// <returns>得意先振込依頼人名テーブルクラスのインスタンス</returns>
        private TokuisakiFurikomiIraiNin GetTokuisakiFurikomiIraiNin(SqlConnection con, SqlTransaction tran, AnserApiRecord anserApiRecord)
        {
            try
            {
                TokuisakiFurikomiIraiNin tokuisakiFurikomiIraiNin = null;

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.得意先ＣＤ" +
                                  "\r\n     ,A.依頼人ＣＤ" +
                                  "\r\n     ,A.振込依頼人名" +
                                  "\r\n     ,A.作成担当者ＣＤ" +
                                  "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                  "\r\n     ,A.作成日" +
                                  "\r\n     ,A.修正担当者ＣＤ" +
                                  "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                  "\r\n     ,A.修正日" +
                                  "\r\n FROM" +
                                  "\r\n     得意先振込依頼人名 AS A" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                  "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n AND A.振込依頼人名 = @振込依頼人名" +
                                  "\r\n ORDER BY" +
                                  "\r\n     A.依頼人ＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = anserApiRecord.得意先ＣＤ;
                cmd.Parameters.Add("@振込依頼人名", SqlDbType.VarChar).Value = anserApiRecord.振込依頼人名;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                DataTable dt = new DataTable("得意先振込依頼人名テーブル");
                adp.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    tokuisakiFurikomiIraiNin = new TokuisakiFurikomiIraiNin(dt.Rows[0]);
                }

                log.Info("得意先振込依頼人名テーブルを検索しました。");

                return tokuisakiFurikomiIraiNin;
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
        /// 得意先振込依頼人名テーブル追加
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        /// <returns>成功した場合は True</returns>
        private bool AddTokuisakiFurikomiIraiNin(SqlConnection con, SqlTransaction tran, AnserApiRecord anserApiRecord)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Create;

                TokuisakiFurikomiIraiNin tokuisakiFurikomiIraiNin = new TokuisakiFurikomiIraiNin();

                tokuisakiFurikomiIraiNin.得意先ＣＤ = anserApiRecord.得意先ＣＤ;
                tokuisakiFurikomiIraiNin.振込依頼人名 = anserApiRecord.振込依頼人名;
                bool ret = tokuisakiFurikomiIraiNin.Insert(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, tokuisakiFurikomiIraiNin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "得意先振込依頼人名テーブル", tokuisakiFurikomiIraiNin.Code, historyProcess);
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
        /// 入金テーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="anserApiRecord">ANSER-APIファイル読込結果（請求・入金データ含む）</param>
        /// <param name="irianinCD">振込依頼人ＣＤ</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveNyukin(SqlConnection con, SqlTransaction tran, AnserApiRecord anserApiRecord, int irianinCD)
        {
            try
            {
                // 次の入金番号を取得
                decimal newNyukinNo = GetNewNyukinNo(con, tran);

                Nyukin nyukin = new Nyukin();

                nyukin.入金番号 = newNyukinNo;
                nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.PC;
                nyukin.入金日 = anserApiRecord.入金日;
                nyukin.部署ＣＤ = anserApiRecord.部署ＣＤ;
                nyukin.得意先ＣＤ = anserApiRecord.得意先ＣＤ;
                nyukin.得意先名 = anserApiRecord.得意先名;
                nyukin.依頼人ＣＤ = irianinCD;
                nyukin.振込依頼人名 = anserApiRecord.振込依頼人名;
                nyukin.入金方法 = (int)Consts.NyukinHoho.Furikomi;
                nyukin.入金額 = anserApiRecord.取込入金額;
                nyukin.入金摘要区分 = _container.入金摘要区分;
                nyukin.対応請求番号 = anserApiRecord.請求番号;
                nyukin.備考 = _container.金融機関名;

                bool ret = nyukin.Insert(con, tran);
                if (ret == false)
                {
                    MessageBox.Show(this, nyukin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "入金明細", nyukin.Code, History.ProcessType.Create);

                // 手数料がある場合は、そのレコードも追加
                if ((anserApiRecord.手数料 ?? 0m) > 0m)
                {
                    // 次の入金番号を取得
                    newNyukinNo = GetNewNyukinNo(con, tran);

                    nyukin = new Nyukin();

                    nyukin.入金番号 = newNyukinNo;
                    nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.PC;
                    nyukin.入金日 = anserApiRecord.入金日;
                    nyukin.部署ＣＤ = anserApiRecord.部署ＣＤ;
                    nyukin.得意先ＣＤ = anserApiRecord.得意先ＣＤ;
                    nyukin.得意先名 = anserApiRecord.得意先名;
                    nyukin.依頼人ＣＤ = irianinCD;
                    nyukin.振込依頼人名 = anserApiRecord.振込依頼人名;
                    nyukin.入金方法 = (int)Consts.NyukinHoho.Tesuryo;
                    nyukin.入金額 = anserApiRecord.手数料;
                    nyukin.入金摘要区分 = _container.入金摘要区分;
                    nyukin.対応請求番号 = anserApiRecord.請求番号;
                    nyukin.備考 = _container.金融機関名;

                    ret = nyukin.Insert(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, nyukin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "入金明細", nyukin.Code, History.ProcessType.Create);
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
        /// 新しい入金番号を返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>入金明細に追加する際の入金番号</returns>
        private decimal GetNewNyukinNo(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                decimal newNyukinNo = -1;       // 追加用入金番号

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

                return newNyukinNo;
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
        /// 部署ＣＤに該当する部署名を返す
        /// </summary>
        /// <param name="busyoCd">部署ＣＤ</param>
        /// <returns>部署名</returns>
        private string GetBusyoName(int busyoCd)
        {
            try
            {
                // マップが無ければ生成
                if (_busyoNameMap == null)
                {
                    _busyoNameMap = new Dictionary<int, string>();

                    using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = con.CreateCommand();
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     部署ＣＤ" +
                                          "\r\n     ,部署名" +
                                          "\r\n FROM" +
                                          "\r\n     部署マスタ" +
                                          "\r\n WHERE" +
                                          "\r\n     企業ＣＤ = @企業ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                        log.Debug(cmd.CommandTextWithParameters());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                _busyoNameMap[(int)reader["部署ＣＤ"]] = reader["部署名"].ToString();
                            }
                        }
                    }
                }

                // 部署ＣＤに該当する部署名をマップから取得して返す
                string busyoName = null;
                if (_busyoNameMap.ContainsKey(busyoCd))
                {
                    busyoName = _busyoNameMap[busyoCd];
                }

                return busyoName;
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

using System;
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
    /// 生産予定数入力
    /// </summary>
    public partial class Form102 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMONBI_COLUMN    = 0;   // 注文日列インデックス(Excel出力まで非表示)
        private const int SYOHIN_CD_COLUMN   = 1;   // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 2;   // 商品名列インデックス
        private const int YOTEISU_COLUMN     = 3;   // 生産予定数列インデックス
        // これ以降の列は動的に追加される
        private const int COURSE_START_COLUMN = 4;  // コース名列開始列インデックス

        /// <summary>
        /// 売上文字色
        /// </summary>
        private readonly Color URIAGE_FORE_COLOR = Color.Red;

        /// <summary>
        /// 入力文字色
        /// </summary>
        private readonly Color INPUT_FORE_COLOR = Color.FromArgb(0, 100, 255);     // Blueだと濃すぎて黒と見分けが付きづらい

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 生産予定数入力検索条件
        /// </summary>
        private SeisanYoteiCriteria _seisanYoteiCriteria;

        /// <summary>
        /// 生産予定数入力検索条件検索実行時スナップショット
        /// </summary>
        private SeisanYoteiCriteria _seisanYoteiCriteriaSnapShot;

        /// <summary>
        /// 生産予定数入力別日読込条件
        /// </summary>
        private SeisanYoteiFromBetsubi _seisanYoteiFromBetsubi;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        // Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        private bool _isF2forSpreadEdit = false;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = COURSE_START_COLUMN;

        /// <summary>
        /// 列として採用するコース名の一覧（コースＣＤ、コース名）を持つデータテーブル
        /// </summary>
        private DataTable _courseDt = null;

        /// <summary>
        /// 商品を行に、コースを列に持ち、入力フラグをセルに持つデータテーブル
        /// </summary>
        private ColumnListDataTable _cldtInputFlg = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form102()
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
        private void Form102_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_flatbed_ico;

                lblInput.ForeColor = INPUT_FORE_COLOR;
                lblUriage.ForeColor = URIAGE_FORE_COLOR;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnMochidashiHanei.Enabled = this.HasAuthEdit;

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

                cboBusyo.SelectedValueChanged -= cboBusyo_SelectedValueChanged;     // 部署ＣＤコンボイベントをいったん削除

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _seisanYoteiCriteria = new SeisanYoteiCriteria();
                seisanYoteiBindingSource.DataSource = _seisanYoteiCriteria;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _seisanYoteiFromBetsubi = new SeisanYoteiFromBetsubi();
                seisanYoteiFromBetsubiBindingSource.DataSource = _seisanYoteiFromBetsubi;

                // 部署ＣＤの初期値を設定したので、コース別グループ種別コンボボックスの選択肢を設定する
                ComboUtil.SetCourseGroup(cboGroupSyubetsu, (int)cboBusyo.SelectedValue);

                // スプレッドの注文日列を非表示にする
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                // 動的に追加される列以外を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = COURSE_START_COLUMN;
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
                cboBusyo.SelectedValueChanged +=  new EventHandler(cboBusyo_SelectedValueChanged);   // 部署ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form102_Shown(object sender, EventArgs e)
        {
            // 検索実行していればスプレッドにフォーカスを当てる
            if (_seisanYoteiCriteriaSnapShot != null)
            {
                fpSpread1.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form102_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F9:                       // 登録ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
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
        private void Form102_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 部署コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // グループ種別コンボの選択肢を、部署にぶら下がるものに再設定する
                if ((int)cboBusyo.SelectedValue < 0)
                {
                    cboGroupSyubetsu.DataSource = null;
                }
                else
                {
                    ComboUtil.SetCourseGroup(cboGroupSyubetsu, (int)cboBusyo.SelectedValue);
                }

                // グループ種別コンボを初期化
                _seisanYoteiCriteria.グループ種別ＣＤ = -1;
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
                if (_seisanYoteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seisanYoteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seisanYoteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_seisanYoteiCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        _seisanYoteiCriteriaSnapShot = null;
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 生産予定を売上から生成
                GenerateSeisanYotei();

                // 生産予定一覧を表示
                GetSeisanList(_seisanYoteiCriteria.注文日);
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
        /// 注文予測取込ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seisanYoteiCriteria.HasErrors)
                {
                    MessageBox.Show(this, _seisanYoteiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索未実行？
                if (_seisanYoteiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_seisanYoteiCriteria.Equals(_seisanYoteiCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 注文から取得して生産予定を表示
                GetSeisanListFromChumon();

                // 生産予定テーブルの内容を表示しているわけではない。
                // そのまま閉じてしまわないように、変更したことにして保存を促す。
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds != null)
                {
                    DataTable dt = ds.Tables["生産予定数入力"];
                    if (dt != null && dt.Rows.Count >= 1)
                    {
                        // 1行目を変更したことにする
                        dt.Rows[0].SetModified();
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 別日読込ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seisanYoteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seisanYoteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seisanYoteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 別日読込条件の日付のエラー状態を更新
                _seisanYoteiFromBetsubi.注文日 = _seisanYoteiFromBetsubi.注文日;

                // 別日読込条件の入力にエラーがある？
                if (_seisanYoteiFromBetsubi.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seisanYoteiFromBetsubi.Error) == false)
                    {
                        MessageBox.Show(this, _seisanYoteiFromBetsubi.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 別日の生産予定一覧を表示
                GetSeisanList(_seisanYoteiFromBetsubi.注文日);

                // 生産予定テーブルの内容を表示しているわけではない。
                // そのまま閉じてしまわないように、変更したことにして保存を促す。
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds != null)
                {
                    DataTable dt = ds.Tables["生産予定数入力"];
                    if (dt != null && dt.Rows.Count >= 1)
                    {
                        // 1行目を変更したことにする
                        dt.Rows[0].SetModified();
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
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = COURSE_START_COLUMN;
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 1)
                        {
                            newRowIndex++;
                        }
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
                    case CHUMONBI_COLUMN:       // 注文日
                    case SYOHIN_CD_COLUMN:      // 商品ＣＤ
                    case SYOHIN_NAME_COLUMN:    // 商品名
                    case YOTEISU_COLUMN:        // 生産予定数
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
                    // 最終列の場合は次の行へ移動する
                    if (newColumnIndex >= MAX_COLUMN_INDEX)
                    {
                        newColumnIndex = COURSE_START_COLUMN;
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 1)
                        {
                            newRowIndex++;
                        }
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
        /// 計算対象週数 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWeeks_Validated(object sender, EventArgs e)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 計算対象週数を変更すると、期間は自動的に再表示される。
                // ここでやるのは、新しい期間に基づく再表示

                OnChangeWeeks();        // 計算対象週数変更時処理
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
        /// 増加ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                // 計算対象週数をインクリメント
                if (_seisanYoteiCriteria.計算対象週数 < 99)
                {
                    _seisanYoteiCriteria.計算対象週数++;
                    ReadBinding(txtWeeks);      // コントロールに反映
                    ReadBinding(lblKikan);      // コントロールに反映

                    this.MdiParent.Cursor = Cursors.WaitCursor;

                    OnChangeWeeks();        // 計算対象週数変更時処理
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
        /// 減少ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                // 計算対象週数をデクリメント
                if (_seisanYoteiCriteria.計算対象週数 > 1)
                {
                    _seisanYoteiCriteria.計算対象週数--;
                    ReadBinding(txtWeeks);      // コントロールに反映
                    ReadBinding(lblKikan);      // コントロールに反映

                    this.MdiParent.Cursor = Cursors.WaitCursor;

                    OnChangeWeeks();        // 計算対象週数変更時処理
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
        /// 日配持出に反映ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMochidashiHanei_Click(object sender, EventArgs e)
        {
            try
            {
                if (HasChange())
                {
                    MessageBox.Show(this, "日配持出に反映するには、生産予定数を保存（登録）してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索未実行？
                if (_seisanYoteiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_seisanYoteiCriteria.Equals(_seisanYoteiCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_seisanYoteiCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 日配持出処理
                SaveMochidashi();
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

                    // 非表示にしていたスプレッドの注文日列を表示する
                    fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = true;

                    fpSpread1.SaveExcel(fileName, excelSaveFlags);

                    // 表示したスプレッドの注文日列を再び非表示にする
                    fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (EmuException ex)
            {
                // 表示したスプレッドの注文日列を再び非表示にする(メッセージボックス表示前に非表示にしたいのでfinallyだと間に合わない)
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                // 表示したスプレッドの注文日列を再び非表示にする(メッセージボックス表示前に非表示にしたいのでfinallyだと間に合わない)
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                // 表示したスプレッドの注文日列を再び非表示にする(メッセージボックス表示前に非表示にしたいのでfinallyだと間に合わない)
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

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
        /// 生産予定を売上から生成する
        /// </summary>
        private void GenerateSeisanYotei()
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Create;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        SqlCommand cmd = con.CreateCommand();
                        // ************************************************************************************************************************
                        // 抽出した売上に紐付く生産予定のレコードが無ければ追加する。
                        // ということは、生産予定を生成した後にそれまで無かったコースに対する売上が追加された場合は、
                        // 過去の日付を開いた場合、生成済みの生産予定にもそのコースのレコードが追加される。
                        // しかし、過去の売上に対してそれまで無かったコースの売上を立てたり、過去の生産予定を開くことはほとんどないと思われるため、
                        // 過去に生成済みの生産予定に対する考慮は特にしない。
                        // ************************************************************************************************************************
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" INSERT INTO 生産予定 (");
                        sql.AppendLine("     企業ＣＤ");
                        sql.AppendLine("     ,部署ＣＤ");
                        sql.AppendLine("     ,喫食日");
                        sql.AppendLine("     ,商品ＣＤ");
                        sql.AppendLine("     ,コースＣＤ");
                        sql.AppendLine("     ,入力フラグ");
                        sql.AppendLine("     ,数量");
                        sql.AppendLine("     ,作成担当者ＣＤ");
                        sql.AppendLine("     ,作成日");
                        sql.AppendLine("     ,修正担当者ＣＤ");
                        sql.AppendLine("     ,修正日");
                        sql.AppendLine(" ) SELECT");
                        sql.AppendLine("     A.企業ＣＤ");
                        sql.AppendLine("     ,A.部署ＣＤ");
                        sql.AppendLine("     ,A.喫食日");
                        sql.AppendLine("     ,A.商品ＣＤ");
                        sql.AppendLine("     ,A.コースＣＤ");
                        sql.AppendLine("     ,0 AS 入力フラグ");
                        sql.AppendLine("     ,A.数量");
                        sql.AppendLine("     ,@作成担当者ＣＤ AS 作成担当者ＣＤ");
                        sql.AppendLine("     ,getdate()");
                        sql.AppendLine("     ,@修正担当者ＣＤ AS 修正担当者ＣＤ");
                        sql.AppendLine("     ,getdate()");
                        sql.AppendLine(" FROM");
                        sql.AppendLine("     (SELECT");
                        sql.AppendLine("         U.企業ＣＤ");
                        sql.AppendLine("         ,U.部署ＣＤ");
                        sql.AppendLine("         ,U.喫食日");
                        sql.AppendLine("         ,U.商品ＣＤ");
                        sql.AppendLine("         ,U.コースＣＤ");
                        sql.AppendLine("         ,0 AS 入力フラグ");
                        sql.AppendLine("         ,U.数量");
                        sql.AppendLine("     FROM");
                        // -- ①売上明細からコースと商品ごとの数量を取得
                        sql.AppendLine("         (SELECT");
                        sql.AppendLine("             C.企業ＣＤ");
                        sql.AppendLine("             ,C.部署ＣＤ");
                        sql.AppendLine("             ,@注文日 AS 喫食日");
                        sql.AppendLine("             ,C.コースＣＤ");
                        sql.AppendLine("             ,C.部品ＣＤ AS 商品ＣＤ");
                        sql.AppendLine("             ,CEILING(AVG(C.数量)) AS 数量");
                        sql.AppendLine("         FROM");
                        // -- ごはんとしての商品と、弁当の部品(=主食)のごはんとを、同じ売上日で合計する（副食も同様）
                        sql.AppendLine("             (SELECT");
                        sql.AppendLine("                 B.企業ＣＤ");
                        sql.AppendLine("                 ,B.部署ＣＤ");
                        sql.AppendLine("                 ,B.コースＣＤ");
                        sql.AppendLine("                 ,B.部品ＣＤ");
                        sql.AppendLine("                 ,B.売上日");
                        sql.AppendLine("                 ,SUM(B.商品数量) AS 数量");
                        sql.AppendLine("             FROM");
                        // -- ①－１ 弁当の主食
                        sql.AppendLine("                 (SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,H.コースＣＤ");
                        sql.AppendLine("                     ,I.主食ＣＤ AS 部品ＣＤ");
                        sql.AppendLine("                     ,M.売上日");
                        sql.AppendLine("                     ,M.商品数量");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     売上明細 AS M");
                        sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                        sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND H.売上日 = M.売上日");
                        sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                        sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                        sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                        sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND SK.対象日 = M.売上日");
                        sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                        sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                        sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                        sql.AppendLine("                     INNER JOIN 商品マスタ AS P");          // -- 部品本体が無効でないこと
                        sql.AppendLine("                         ON P.企業ＣＤ = I.企業ＣＤ");
                        sql.AppendLine("                         AND ISNULL(P.無効フラグ, 0) = 0");
                        sql.AppendLine("                         AND P.商品ＣＤ = I.主食ＣＤ");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                        sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                        sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                        sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                        sql.AppendLine("                     AND SK.対象日 IS NULL");                       // -- 祝日は除く
                        sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                        sql.AppendLine("                     AND ISNULL(M.主食ＣＤ, 0) <> 0");              // -- 主食がある
                        sql.AppendLine("                     AND ISNULL(M.主食ＣＤ, 0) <> M.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                        sql.AppendLine("                 UNION ALL");       // -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                        // -- ①－２ 弁当の副食
                        sql.AppendLine("                 SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,H.コースＣＤ");
                        sql.AppendLine("                     ,I.副食ＣＤ AS 部品ＣＤ");
                        sql.AppendLine("                     ,M.売上日");
                        sql.AppendLine("                     ,M.商品数量");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     売上明細 AS M");
                        sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                        sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND H.売上日 = M.売上日");
                        sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                        sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                        sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                        sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND SK.対象日 = M.売上日");
                        sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                        sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                        sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                        sql.AppendLine("                     INNER JOIN 商品マスタ AS P");          // -- 部品本体が無効でないこと
                        sql.AppendLine("                         ON P.企業ＣＤ = I.企業ＣＤ");
                        sql.AppendLine("                         AND ISNULL(P.無効フラグ, 0) = 0");
                        sql.AppendLine("                         AND P.商品ＣＤ = I.副食ＣＤ");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                        sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                        sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                        sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                        sql.AppendLine("                     AND SK.対象日 IS NULL");                       // -- 祝日は除く
                        sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                        sql.AppendLine("                     AND ISNULL(M.副食ＣＤ, 0) <> 0");              // -- 副食がある
                        sql.AppendLine("                     AND ISNULL(M.副食ＣＤ, 0) <> M.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                        sql.AppendLine("                 UNION ALL");
                        // -- ①－３ 主食のみ
                        sql.AppendLine("                 SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,H.コースＣＤ");
                        sql.AppendLine("                     ,I.商品ＣＤ AS 部品ＣＤ");
                        sql.AppendLine("                     ,M.売上日");
                        sql.AppendLine("                     ,M.商品数量");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     売上明細 AS M");
                        sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                        sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND H.売上日 = M.売上日");
                        sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                        sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                        sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                        sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND SK.対象日 = M.売上日");
                        sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                        sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                        sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                        sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                        sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                        sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                        sql.AppendLine("                     AND SK.対象日 IS NULL");       // -- 祝日は除く
                        sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                        sql.AppendLine("                 UNION ALL");
                        // -- ①－４ 副食のみ
                        sql.AppendLine("                 SELECT");
                        sql.AppendLine("                     M.企業ＣＤ");
                        sql.AppendLine("                     ,M.部署ＣＤ");
                        sql.AppendLine("                     ,H.コースＣＤ");
                        sql.AppendLine("                     ,I.商品ＣＤ AS 部品ＣＤ");
                        sql.AppendLine("                     ,M.売上日");
                        sql.AppendLine("                     ,M.商品数量");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     売上明細 AS M");
                        sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                        sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND H.売上日 = M.売上日");
                        sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                        sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                        sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                        sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND SK.対象日 = M.売上日");
                        sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                        sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                        sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                        sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                        sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                        sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                        sql.AppendLine("                     AND SK.対象日 IS NULL");       // -- 祝日は除く
                        sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                        sql.AppendLine("                 ) AS B");
                        sql.AppendLine("             GROUP BY");
                        sql.AppendLine("                 B.企業ＣＤ");
                        sql.AppendLine("                 ,B.部署ＣＤ");
                        sql.AppendLine("                 ,B.コースＣＤ");
                        sql.AppendLine("                 ,B.部品ＣＤ");
                        sql.AppendLine("                 ,B.売上日");
                        sql.AppendLine("             ) AS C");
                        sql.AppendLine("         GROUP BY");
                        sql.AppendLine("             C.企業ＣＤ");
                        sql.AppendLine("             ,C.部署ＣＤ");
                        sql.AppendLine("             ,C.コースＣＤ");
                        sql.AppendLine("             ,C.部品ＣＤ");
                        sql.AppendLine("         ) AS U");
                        // -- ②生産予定に無いレコードを抽出するための結合
                        sql.AppendLine("         LEFT JOIN 生産予定 AS S");
                        sql.AppendLine("             ON  S.企業ＣＤ   = U.企業ＣＤ");
                        sql.AppendLine("             AND S.部署ＣＤ   = U.部署ＣＤ");
                        sql.AppendLine("             AND S.喫食日     = U.喫食日");
                        sql.AppendLine("             AND S.商品ＣＤ   = U.商品ＣＤ");
                        sql.AppendLine("             AND S.コースＣＤ = U.コースＣＤ");
                        // -- ③コース区分の条件用
                        sql.AppendLine("         INNER JOIN コースマスタ AS C");
                        sql.AppendLine("             ON  C.企業ＣＤ   = U.企業ＣＤ");
                        sql.AppendLine("             AND C.コースＣＤ = U.コースＣＤ");

                        if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                        {
                            // -- ④コース別グループと結合
                            sql.AppendLine("         INNER JOIN コース別グループマスタ AS CG ON");
                            sql.AppendLine("             CG.企業ＣＤ = @企業ＣＤ");
                            sql.AppendLine("             AND CG.部署ＣＤ = @部署ＣＤ");
                            sql.AppendLine("             AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                            sql.AppendLine("             AND CG.コースＣＤ = U.コースＣＤ");

                            cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                        }

                        sql.AppendLine("          WHERE S.作成担当者ＣＤ IS NULL");
                        sql.AppendLine("          AND   C.コース区分 = @コース区分");
                        sql.AppendLine("          AND   ISNULL(C.工場フラグ, 0) = 0");
                        sql.AppendLine("          AND   ISNULL(C.無効フラグ, 0) = 0");
                        sql.AppendLine("      ) AS A");

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                        cmd.Parameters.Add("@期間開始日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間開始日;
                        cmd.Parameters.Add("@期間終了日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間終了日;
                        cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                        cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                        cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                        cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);
                        UserData userData = UserData.getInstance();
                        cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                        cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                        cmd.CommandText = sql.ToString();
                        cmd.Transaction = tran;
                        log.Debug(cmd.CommandTextWithParameters());
                        cmd.ExecuteNonQuery();

                        tran.Commit();
                        log.InfoFormat("生産予定入力を{0}しました。", historyProcess.DisplayName());
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
        /// 生産予定一覧を表示
        /// </summary>
        private void GetSeisanList(DateTime? targetDate)
        {
            try
            {
                //*****************************************************************
                // コースマスタ、生産予定、売上ヘッダにあるコースを列として追加し、
                // 生産予定と商品マスタにある商品を行とする
                // データそのものは生産予定から取得する
                //*****************************************************************

                // データ本体を取得
                DataTable dt = new DataTable("生産予定");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     @日付 AS 日付");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,SM.商品名");
                    sql.AppendLine("     ,ISNULL(Y.入力フラグ, 1) AS 入力フラグ");  // -- 生産予定が無い場合は、これから入力するということなので、入力フラグのデフォルトはON
                    sql.AppendLine("     ,Y.数量");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    // -- コースマスタと生産予定テーブルから取得したコース（＝全コース）と、商品マスタと生産予定テーブルから取得した商品（＝全商品）の組み合わせ
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         C.コースＣＤ");
                    sql.AppendLine("         ,I.商品ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND コース区分 = @コース区分");
                    sql.AppendLine("             AND ISNULL(工場フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         ) AS C,");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             商品マスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("             AND 弁当区分 IN (@弁当区分副食, @弁当区分主食)");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         ) AS I");
                    sql.AppendLine("     ) AS A");
                    // -- ②生産予定から数量を取得
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("             ,コースＣＤ");
                    sql.AppendLine("             ,入力フラグ");
                    sql.AppendLine("             ,数量");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         ) AS Y");
                    sql.AppendLine("         ON Y.コースＣＤ = A.コースＣＤ");
                    sql.AppendLine("         AND Y.商品ＣＤ = A.商品ＣＤ");
                    // -- ③名称を取得するためのマスタを結合
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = A.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = @企業ＣＤ AND SM.商品ＣＤ = A.商品ＣＤ");

                    if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                    {
                        // -- ④コース別グループと結合
                        sql.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                        sql.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                        sql.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                        sql.AppendLine("         AND CG.コースＣＤ = A.コースＣＤ");

                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                    }

                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     SM.弁当区分");
                    sql.AppendLine("     ,A.商品ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@日付", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;    // 検索条件の注文日を取得・表示
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = targetDate;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    // 表示するコースは、検索条件の注文日に該当するコース区分のもの（指定された注文日に該当するコース区分のものではない）
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("生産予定数入力リストを検索しました。");
                }

                // 表示主処理を呼び出す
                GetSeisanListMain(dt);
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
        /// 生産予定一覧を表示（主処理）
        /// </summary>
        /// <param name="dt">データ本体</param>
        private void GetSeisanListMain(DataTable dt)
        {
            try
            {
                // 列として採用するコース名の一覧をデータテーブルとして取得する
                _courseDt = GetCourseTable();

                // コース名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_courseDt, "コースＣＤ", "コース名");

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("生産予定数入力");
                // コース名列を作成
                cldt.AddColumns(_courseDt, "コース名", "コースＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["日付"]);
                cldt.AddFixedColumn(dt.Columns["商品ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["商品名"]);
                // インポート
                cldt.Import(dt, "コースＣＤ", "数量");

                // 同様に入力フラグを持つデータテーブルを作成する
                _cldtInputFlg = new ColumnListDataTable();
                // コース名列を作成
                _cldtInputFlg.AddColumns(_courseDt, "コース名", "コースＣＤ", typeof(bool));
                // 固定列を作成
                _cldtInputFlg.AddFixedColumn(dt.Columns["商品ＣＤ"]);
                // インポート
                _cldtInputFlg.Import(dt, "コースＣＤ", "入力フラグ");

                // データソース列値変更イベントを設定
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "生産予定数入力";

                fpSpread1_Sheet1.BindDataColumn(CHUMONBI_COLUMN,    "日付");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");

                // 商品名を列名としてバインド
                int columnIndex = COURSE_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // スプレッドの文字色を以下のように変更
                // 入力フラグON（生産予定から） ：青
                // 入力フラグOFF（売上から）：赤
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();               // いったん黒に戻す

                    // 背景色もリセット
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;

                    // 生産予定数の数式を設定する
                    FarPoint.Win.Spread.Cell firstCell = fpSpread1_Sheet1.Cells[rowIndex, COURSE_START_COLUMN];
                    FarPoint.Win.Spread.Cell lastCell = fpSpread1_Sheet1.Cells[rowIndex, MAX_COLUMN_INDEX];
                    fpSpread1_Sheet1.Cells[rowIndex, YOTEISU_COLUMN].Formula = string.Format("SUM({0}:{1})", firstCell.ToString(), lastCell.ToString());

                    for (columnIndex = COURSE_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = INPUT_FORE_COLOR;   // 手入力したときの色にする

                        // スプレッドの各行の先頭列から商品ＣＤを取得
                        //                     各列の商品ＣＤはコース一覧のデータテーブルから取得
                        int courseCD = (int)_courseDt.Rows[columnIndex - COURSE_START_COLUMN]["コースＣＤ"];
                        if (_cldtInputFlg[rowIndex, courseCD] as bool? != true)     // false or null
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = URIAGE_FORE_COLOR;
                        }
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, COURSE_START_COLUMN);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _seisanYoteiCriteriaSnapShot = new SeisanYoteiCriteria(_seisanYoteiCriteria);
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

                // 固定列以外（つまり全てのコースの列）について、このイベントが発生する

                // セルの文字色を青にする
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, fpSpread1_Sheet1.ActiveColumnIndex].ForeColor = INPUT_FORE_COLOR;

                // 入力フラグをONにする
                _cldtInputFlg.Rows[fpSpread1_Sheet1.ActiveRowIndex][e.Column.ColumnName] = true;
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
        /// スプレッドの列として採用するコース名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetCourseTable()
        {
            DataTable dt = new DataTable("コース一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     C.コースＣＤ");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine(" FROM");
                    // -- コースマスタから
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コースマスタ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND コース区分 = @コース区分");
                    sql.AppendLine("         AND ISNULL(工場フラグ, 0) = 0");
                    sql.AppendLine("         AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("     UNION");
                    // -- 生産予定から
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         生産予定");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("     UNION");
                    // -- 売上から
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         H.コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ AS H");
                    sql.AppendLine("         LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = H.企業ＣＤ AND SK.対象日 = H.売上日");
                    sql.AppendLine("         INNER JOIN コースマスタ AS C");
                    sql.AppendLine("             ON  C.企業ＣＤ   = H.企業ＣＤ");
                    sql.AppendLine("             AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("         AND H.売上日 >= @期間開始日");
                    sql.AppendLine("         AND H.売上日 <= @期間終了日");
                    sql.AppendLine("         AND DATEPART(WEEKDAY, H.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("         AND SK.対象日 IS NULL");
                    sql.AppendLine("         AND C.コース区分 = @コース区分");
                    sql.AppendLine("         AND ISNULL(C.工場フラグ, 0) = 0");
                    sql.AppendLine("         AND ISNULL(C.無効フラグ, 0) = 0");
                    sql.AppendLine("     ) AS C");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ");

                    if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                    {
                        // -- コース別グループと結合
                        sql.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                        sql.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                        sql.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                        sql.AppendLine("         AND CG.コースＣＤ = C.コースＣＤ");

                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                    }

                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     C.コースＣＤ");
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                    cmd.Parameters.Add("@期間開始日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間開始日;
                    cmd.Parameters.Add("@期間終了日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間終了日;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース一覧を取得しました。");
                }

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
        /// コース名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="courseDt">コース名一覧データテーブル</param>
        /// <param name="labelColumnCD">コースCDが格納されている列の名前</param>
        /// <param name="labelColumnName">コース名が格納されている列の名前</param>
        private void SetSpreadColumn(DataTable courseDt, string labelColumnCD, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = COURSE_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = COURSE_START_COLUMN + courseDt.Rows.Count;

                // 固定列に続いて、コース名の列の属性をセット
                int columnIndex = COURSE_START_COLUMN;
                foreach (DataRow dr in courseDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = 0D;

                    fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex).Value = dr[labelColumnCD].ToString();
                    fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex).Value = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
                    columnIndex++;
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
        /// 計算対象週数変更時処理（売上から取得して生産予定一覧を表示）
        /// </summary>
        private void OnChangeWeeks()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_seisanYoteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seisanYoteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seisanYoteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 売上から取得した内容で再表示する
                GetSeisanListFromUriage();

                // 生産予定テーブルの内容を表示しているわけではない。
                // そのまま閉じてしまわないように、変更したことにして保存を促す。
                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds != null)
                {
                    DataTable dt = ds.Tables["生産予定数入力"];
                    if (dt != null && dt.Rows.Count >= 1)
                    {
                        // 1行目を変更したことにする
                        dt.Rows[0].SetModified();
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
        /// 売上から取得した生産予定一覧を表示
        /// </summary>
        private void GetSeisanListFromUriage()
        {
            try
            {
                //*****************************************************************
                // コースマスタ、生産予定、売上ヘッダにあるコースを列として追加し、
                // 生産予定と商品マスタと売上明細にある商品を行とする
                // データそのものは売上から取得する
                //*****************************************************************

                // データ本体を取得
                DataTable dt = new DataTable("生産予定");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     @日付 AS 日付");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,SM.商品名");
                    sql.AppendLine("     ,ISNULL(U.入力フラグ, 1) AS 入力フラグ");      // -- 売上に無い場合は手入力するので入力フラグのデフォルトはON
                    sql.AppendLine("     ,U.数量");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         C.コースＣＤ");
                    sql.AppendLine("         ,I.商品ＣＤ");
                    sql.AppendLine("     FROM");
                    // -- コースマスタと生産予定テーブルと売上から取得したコース（＝全コース）と、商品マスタと生産予定テーブルと売上から取得した商品（＝全商品）の組み合わせ
                    // -- ①コース
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND コース区分 = @コース区分");
                    sql.AppendLine("             AND ISNULL(工場フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = H.企業ＣＤ AND SK.対象日 = H.売上日");
                    sql.AppendLine("             INNER JOIN コースマスタ AS C");
                    sql.AppendLine("                 ON  C.企業ＣＤ   = H.企業ＣＤ");
                    sql.AppendLine("                 AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND H.売上日 >= @期間開始日");
                    sql.AppendLine("             AND H.売上日 <= @期間終了日");
                    sql.AppendLine("             AND DATEPART(WEEKDAY, H.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("             AND SK.対象日 IS NULL");           // -- 祝日は除く
                    sql.AppendLine("             AND C.コース区分 = @コース区分");  // -- 曜日に応じたコース
                    sql.AppendLine("             AND ISNULL(C.工場フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(C.無効フラグ, 0) = 0");
                    sql.AppendLine("         ) AS C,");
                    // -- ②商品
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             商品マスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("             AND 弁当区分 IN (@弁当区分副食, @弁当区分主食)");      // -- 副食と主食のみ
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         UNION");
                    // -- ②－１ 弁当の主食
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.主食ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS M");
                    sql.AppendLine("             INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                 ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND SK.対象日 = M.売上日");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS P");          // -- 部品本体が無効でないこと
                    sql.AppendLine("                 ON P.企業ＣＤ = I.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(P.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND P.商品ＣＤ = I.主食ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.売上日 >= @期間開始日");
                    sql.AppendLine("             AND M.売上日 <= @期間終了日");
                    sql.AppendLine("             AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("             AND SK.対象日 IS NULL");                       // -- 祝日は除く
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("             AND ISNULL(M.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("             AND ISNULL(M.主食ＣＤ, 0) <> M.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("         UNION");
                    // -- ②－２ 弁当の副食
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.副食ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS M");
                    sql.AppendLine("             INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                 ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND SK.対象日 = M.売上日");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS P");          // -- 部品本体が無効でないこと
                    sql.AppendLine("                 ON P.企業ＣＤ = I.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(P.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND P.商品ＣＤ = I.副食ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.売上日 >= @期間開始日");
                    sql.AppendLine("             AND M.売上日 <= @期間終了日");
                    sql.AppendLine("             AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("             AND SK.対象日 IS NULL");                       // -- 祝日は除く
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("             AND ISNULL(M.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("             AND ISNULL(M.副食ＣＤ, 0) <> M.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("         UNION");
                    // -- ②－３ 主食のみ
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.商品ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS M");
                    sql.AppendLine("             INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                 ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND SK.対象日 = M.売上日");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.売上日 >= @期間開始日");
                    sql.AppendLine("             AND M.売上日 <= @期間終了日");
                    sql.AppendLine("             AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("             AND SK.対象日 IS NULL");       // -- 祝日は除く
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("         UNION");
                    // -- ②－４ 副食のみ
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.商品ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS M");
                    sql.AppendLine("             INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                 ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND SK.対象日 = M.売上日");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.売上日 >= @期間開始日");
                    sql.AppendLine("             AND M.売上日 <= @期間終了日");
                    sql.AppendLine("             AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("             AND SK.対象日 IS NULL");       // -- 祝日は除く
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("         ) AS I");
                    sql.AppendLine("     ) AS A");
                    // -- ③売上明細からコースと商品ごとの数量を取得
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.コースＣＤ");
                    sql.AppendLine("             ,C.商品ＣＤ");
                    sql.AppendLine("             ,0 AS 入力フラグ");
                    sql.AppendLine("             ,CEILING(AVG(C.数量)) AS 数量");
                    sql.AppendLine("         FROM");
                    // -- ごはんとしての商品と、弁当の部品(=主食)のごはんとを、同じ売上日で合計する（副食も同様）
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 B.コースＣＤ");
                    sql.AppendLine("                 ,B.部品ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("                 ,B.売上日");
                    sql.AppendLine("                 ,SUM(B.商品数量) AS 数量");
                    sql.AppendLine("             FROM");
                    // -- ③－１ 弁当の主食
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,I.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,M.売上日");
                    sql.AppendLine("                     ,M.商品数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上明細 AS M");
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.売上日 = M.売上日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND SK.対象日 = M.売上日");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS P");          // -- 部品本体が無効でないこと
                    sql.AppendLine("                         ON P.企業ＣＤ = I.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(P.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND P.商品ＣＤ = I.主食ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                    sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                    sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("                     AND SK.対象日 IS NULL");                       // -- 祝日は除く
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(M.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                     AND ISNULL(M.主食ＣＤ, 0) <> M.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 UNION ALL");               // -- SUM()の対象にするので、同一コースＣＤ、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                    // -- ③－２ 弁当の副食
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,I.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,M.売上日");
                    sql.AppendLine("                     ,M.商品数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上明細 AS M");
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.売上日 = M.売上日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND SK.対象日 = M.売上日");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS P");          // -- 部品本体が無効でないこと
                    sql.AppendLine("                         ON P.企業ＣＤ = I.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(P.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND P.商品ＣＤ = I.副食ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                    sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                    sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("                     AND SK.対象日 IS NULL");                       // -- 祝日は除く
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(M.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                     AND ISNULL(M.副食ＣＤ, 0) <> M.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 UNION ALL");
                    // -- ③－３ 主食のみ
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,M.売上日");
                    sql.AppendLine("                     ,M.商品数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上明細 AS M");
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.売上日 = M.売上日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND SK.対象日 = M.売上日");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                    sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                    sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("                     AND SK.対象日 IS NULL");       // -- 祝日は除く
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("                 UNION ALL");
                    // -- ③－４ 副食のみ
                    sql.AppendLine("                 SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,M.売上日");
                    sql.AppendLine("                     ,M.商品数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     売上明細 AS M");
                    sql.AppendLine("                     INNER JOIN 売上ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.売上日 = M.売上日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     LEFT JOIN 祝日マスタ AS SK");
                    sql.AppendLine("                         ON SK.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND SK.対象日 = M.売上日");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.売上日 >= @期間開始日");
                    sql.AppendLine("                     AND M.売上日 <= @期間終了日");
                    sql.AppendLine("                     AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("                     AND SK.対象日 IS NULL");       // -- 祝日は除く
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("                 ) AS B");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 B.コースＣＤ");
                    sql.AppendLine("                 ,B.部品ＣＤ");
                    sql.AppendLine("                 ,B.売上日");
                    sql.AppendLine("             ) AS C");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             C.コースＣＤ");
                    sql.AppendLine("             ,C.商品ＣＤ");
                    sql.AppendLine("         ) AS U");
                    sql.AppendLine("         ON U.コースＣＤ = A.コースＣＤ");
                    sql.AppendLine("         AND U.商品ＣＤ = A.商品ＣＤ");
                    // -- ④名称を取得するためのマスタを結合
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = A.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = @企業ＣＤ AND SM.商品ＣＤ = A.商品ＣＤ");

                    if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                    {
                        // -- ⑤コース別グループと結合
                        sql.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                        sql.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                        sql.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                        sql.AppendLine("         AND CG.コースＣＤ = A.コースＣＤ");

                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                    }

                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     SM.弁当区分");
                    sql.AppendLine("     ,A.商品ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@日付", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                    cmd.Parameters.Add("@期間開始日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間開始日;
                    cmd.Parameters.Add("@期間終了日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間終了日;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("売上からの生産予定数入力リストを検索しました。");
                }

                // 表示主処理を呼び出す
                GetSeisanListMain(dt);
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
        /// 注文から取得した生産予定一覧を表示
        /// </summary>
        private void GetSeisanListFromChumon()
        {
            try
            {
                //*****************************************************************
                // コースマスタ、生産予定、注文ヘッダにあるコースを列として追加し、
                // 生産予定と商品マスタと注文明細にある商品を行とする
                // データそのものは注文から取得する（予測よりも注文を優先する）
                //*****************************************************************

                // データ本体を取得
                DataTable dt = new DataTable("生産予定");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     @日付 AS 日付");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,SM.商品名");
                    sql.AppendLine("     ,ISNULL(U.入力フラグ, 1) AS 入力フラグ");      // -- 売上に無い場合は手入力するので入力フラグのデフォルトはON
                    sql.AppendLine("     ,U.数量");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         C.コースＣＤ");
                    sql.AppendLine("         ,I.商品ＣＤ");
                    sql.AppendLine("     FROM");
                    // -- コースマスタと生産予定テーブルと注文から取得したコース（＝全コース）と、商品マスタと生産予定テーブルと注文から取得した商品（＝全商品）の組み合わせ
                    // -- ①コース
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コースマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND コース区分 = @コース区分");
                    sql.AppendLine("             AND ISNULL(工場フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN コースマスタ AS C");
                    sql.AppendLine("                 ON  C.企業ＣＤ   = H.企業ＣＤ");
                    sql.AppendLine("                 AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND H.喫食日 >= @注文日");
                    sql.AppendLine("             AND C.コース区分 = @コース区分");      // -- 曜日に応じたコース
                    sql.AppendLine("             AND ISNULL(C.工場フラグ, 0) = 0");
                    sql.AppendLine("             AND ISNULL(C.無効フラグ, 0) = 0");
                    sql.AppendLine("         ) AS C,");
                    // -- ②商品
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             商品マスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("             AND 弁当区分 IN (@弁当区分副食, @弁当区分主食)");      // -- 副食と主食のみ
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             生産予定");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("         UNION");
                    // -- ②－１ 弁当の主食
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.主食ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文明細 AS M");
                    sql.AppendLine("             INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.喫食日 = @注文日");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("             AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("             AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("         UNION");
                    // -- ②－２ 弁当の副食
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.副食ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文明細 AS M");
                    sql.AppendLine("             INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.喫食日 = @注文日");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("             AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("             AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("         UNION");
                    // -- ②－３ 主食のみ
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.商品ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文明細 AS M");
                    sql.AppendLine("             INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.喫食日 = @注文日");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("         UNION");
                    // -- ②－４ 副食のみ
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             I.商品ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文明細 AS M");
                    sql.AppendLine("             INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                 ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("             AND M.喫食日 = @注文日");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("         ) AS I");
                    sql.AppendLine("     ) AS A");
                    // -- ③注文明細からコースと商品ごとの数量を取得
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             B.コースＣＤ");
                    sql.AppendLine("             ,B.部品ＣＤ AS 商品ＣＤ");
                    sql.AppendLine("             ,0 AS 入力フラグ");
                    sql.AppendLine("             ,SUM(B.商品注文予測数量) AS 数量");
                    sql.AppendLine("         FROM");
                    // -- 注文数が0の場合、予測を採用するのではなく、0の注文数を採用する（注文のレコードがあれば必ず採用する）
                    // -- ③－１ 弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 PART1.コースＣＤ");
                    sql.AppendLine("                 ,PART1.得意先ＣＤ");
                    sql.AppendLine("                 ,PART1.部品ＣＤ");
                    sql.AppendLine("                 ,SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文明細 AS M");
                    sql.AppendLine("                     INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                         AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.喫食日 = @注文日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.主食ＣＤ");
                    sql.AppendLine("                 ) AS PART1");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART1.コースＣＤ");
                    sql.AppendLine("                 ,PART1.得意先ＣＤ");
                    sql.AppendLine("                 ,PART1.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");       // -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                    // -- ③－２ 弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART2.コースＣＤ");
                    sql.AppendLine("                 ,PART2.得意先ＣＤ");
                    sql.AppendLine("                 ,PART2.部品ＣＤ");
                    sql.AppendLine("                 ,SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文明細 AS M");
                    sql.AppendLine("                     INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                         AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.喫食日 = @注文日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.副食ＣＤ");
                    sql.AppendLine("                 ) AS PART2");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART2.コースＣＤ");
                    sql.AppendLine("                 ,PART2.得意先ＣＤ");
                    sql.AppendLine("                 ,PART2.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－３ 主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART3.コースＣＤ");
                    sql.AppendLine("                 ,PART3.得意先ＣＤ");
                    sql.AppendLine("                 ,PART3.部品ＣＤ");
                    sql.AppendLine("                 ,SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文明細 AS M");
                    sql.AppendLine("                     INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                         AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.喫食日 = @注文日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.商品ＣＤ");
                    sql.AppendLine("                 ) AS PART3");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART3.コースＣＤ");
                    sql.AppendLine("                 ,PART3.得意先ＣＤ");
                    sql.AppendLine("                 ,PART3.部品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－４ 副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 PART4.コースＣＤ");
                    sql.AppendLine("                 ,PART4.得意先ＣＤ");
                    sql.AppendLine("                 ,PART4.部品ＣＤ");
                    sql.AppendLine("                 ,SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 0 THEN M.商品数量 ELSE null END) AS 商品注文数量");
                    sql.AppendLine("                     ,SUM(CASE WHEN M.注文区分 = 1 THEN M.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     注文明細 AS M");
                    sql.AppendLine("                     INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                         ON H.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND H.注文区分 = M.注文区分");
                    sql.AppendLine("                         AND H.喫食日 = M.喫食日");
                    sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                         AND H.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                         ON I.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                         AND I.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("                     AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("                     AND M.喫食日 = @注文日");
                    sql.AppendLine("                     AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     H.コースＣＤ");
                    sql.AppendLine("                     ,H.得意先ＣＤ");
                    sql.AppendLine("                     ,I.商品ＣＤ");
                    sql.AppendLine("                 ) AS PART4");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 PART4.コースＣＤ");
                    sql.AppendLine("                 ,PART4.得意先ＣＤ");
                    sql.AppendLine("                 ,PART4.部品ＣＤ");
                    sql.AppendLine("             ) AS B");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             B.コースＣＤ");
                    sql.AppendLine("             ,B.部品ＣＤ");
                    sql.AppendLine("         ) AS U");
                    sql.AppendLine("         ON U.コースＣＤ = A.コースＣＤ");
                    sql.AppendLine("         AND U.商品ＣＤ = A.商品ＣＤ");
                    // -- ④名称を取得するためのマスタを結合
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = A.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = @企業ＣＤ AND SM.商品ＣＤ = A.商品ＣＤ");

                    if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                    {
                        // -- ⑤コース別グループと結合
                        sql.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                        sql.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                        sql.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                        sql.AppendLine("         AND CG.コースＣＤ = A.コースＣＤ");

                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                    }

                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     SM.弁当区分");
                    sql.AppendLine("     ,A.商品ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@日付", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("注文からの生産予定数入力リストを検索しました。");
                }

                // 表示主処理を呼び出す
                GetSeisanListMain(dt);
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
        /// 生産予定数入力リストが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 生産予定数入力リストを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["生産予定数入力"];

            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                DataRow dr = dt.Rows[rowIndex];
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
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_seisanYoteiCriteria.HasErrors)
                {
                    MessageBox.Show(this, _seisanYoteiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_seisanYoteiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_seisanYoteiCriteria.Equals(_seisanYoteiCriteriaSnapShot) == false)
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
            try
            {
                bool? ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        //********************************************************************************************************
                        // ・部署に該当する生産予定を削除（なので部署が指定されていないと保存できない）
                        // ・cldtの行をループしながら以下を実施
                        //   ・コース一覧データテーブルをループしつつコースＣＤを取得
                        //   ・cldtの行（商品ＣＤ）の、コースＣＤに該当する列の値を取得
                        //   ・生産予定を追加
                        //********************************************************************************************************

                        //*****************************
                        // 部署に該当する生産予定を削除
                        //******************************
                        SqlCommand cmdDel = con.CreateCommand();
                        cmdDel.Transaction = tran;

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" DELETE A FROM 生産予定 AS A");

                        if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                        {
                            // -- コース別グループと結合
                            sql.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                            sql.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                            sql.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                            sql.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                            sql.AppendLine("         AND CG.コースＣＤ = A.コースＣＤ");

                            cmdDel.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                        }

                        sql.AppendLine(" WHERE A.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine(" AND A.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine(" AND A.喫食日 = @注文日");

                        cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmdDel.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                        cmdDel.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;

                        cmdDel.CommandText = sql.ToString();

                        log.Debug(cmdDel.CommandTextWithParameters());

                        cmdDel.ExecuteNonQuery();

                        //*******************************************************************************
                        // cldtの行をループしつつ、コース一覧データテーブルをループしつつコースＣＤを取得
                        //*******************************************************************************
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["生産予定数入力"];

                        for (int rowIndex = 0; rowIndex < cldt.Rows.Count; rowIndex++)
                        {
                            int syohinCD = (int)cldt.Rows[rowIndex]["商品ＣＤ"];

                            foreach (DataRow courseDr in _courseDt.Rows)
                            {
                                int courseCD = (int)courseDr["コースＣＤ"];     // 列に該当するコースＣＤを取得

                                //*****************************************************
                                // cldtの行（商品）の、コースＣＤに該当する列の値を取得
                                //*****************************************************
                                int? suryo = (int?)cldt[rowIndex, courseCD];

                                if (suryo == null)
                                {
                                    continue;
                                }

                                bool? inputFlg = _cldtInputFlg[rowIndex, courseCD] as bool?;    // 入力フラグも取得

                                //***************
                                // 生産予定を追加
                                //***************
                                SeisanYotei seisanYotei = new SeisanYotei();
                                seisanYotei.部署ＣＤ = _seisanYoteiCriteria.部署ＣＤ;
                                seisanYotei.喫食日 = _seisanYoteiCriteria.注文日;
                                seisanYotei.商品ＣＤ = syohinCD;
                                seisanYotei.コースＣＤ = courseCD;
                                seisanYotei.入力フラグ = inputFlg ?? true;      // デフォルトは手入力なので
                                seisanYotei.数量 = suryo;

                                ret = seisanYotei.Insert(con, tran);
                                if (ret == false)
                                {
                                    MessageBox.Show(this, seisanYotei.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                historyProcess = History.ProcessType.Create;
                                History.Save(con, tran, this.Text, "生産予定", seisanYotei.Code, historyProcess);
                            }
                        }

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("生産予定一覧を{0}しました。", historyProcess.DisplayName());
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
            try
            {
                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 検索ボタン押下

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread1.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = activeRowIndex;
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
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
        /// 日配持出反映処理
        /// </summary>
        private void SaveMochidashi()
        {
            try
            {
                bool? ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        //********************************************************************************************************
                        // ・部署に該当するコース・商品を持つモバイル_持ち出し入力を削除
                        // ・cldtの行をループしながら以下を実施
                        //   ・コース一覧データテーブルをループしつつコースＣＤを取得
                        //   ・cldtの行（商品ＣＤ）の、コースＣＤに該当する列の値を取得
                        //   ・モバイル_持ち出し入力を追加
                        //********************************************************************************************************

                        //************************************************************
                        // 部署に該当するコース・商品を持つモバイル_持ち出し入力を削除
                        //************************************************************
                        SqlCommand cmdDel = con.CreateCommand();
                        cmdDel.Transaction = tran;
                        string sql = "\r\n DELETE FROM M" +
                                     "\r\n FROM" +
                                     "\r\n     モバイル_持ち出し入力 AS M" +
                                     "\r\n     INNER JOIN" +
                                     "\r\n         (SELECT" +
                                     "\r\n             @企業ＣＤ AS 企業ＣＤ" +
                                     "\r\n             ,C.コースＣＤ" +
                                     "\r\n             ,I.商品ＣＤ" +
                                     "\r\n             ,@注文日 AS 持出日" +
                                     "\r\n         FROM" +
                                     // -- コースマスタと生産予定テーブルと売上から取得したコース（＝全コース）と、商品マスタと生産予定テーブルと売上から取得した商品（＝全商品）の組み合わせ
                                     // -- ①コース
                                     "\r\n             (SELECT" +
                                     "\r\n                 コースＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 コースマスタ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND 部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND コース区分 = @コース区分" +
                                     "\r\n                 AND ISNULL(工場フラグ, 0) = 0" +
                                     "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                                     "\r\n             UNION" +
                                     "\r\n             SELECT" +
                                     "\r\n                 コースＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 生産予定" +
                                     "\r\n             WHERE" +
                                     "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND 部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND 喫食日 = @注文日" +
                                     "\r\n             UNION" +
                                     "\r\n             SELECT" +
                                     "\r\n                 H.コースＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 売上ヘッダ AS H" +
                                     "\r\n                 LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = H.企業ＣＤ AND SK.対象日 = H.売上日" +
                                     "\r\n                 INNER JOIN コースマスタ AS C" +
                                     "\r\n                     ON  C.企業ＣＤ   = H.企業ＣＤ" +
                                     "\r\n                     AND C.コースＣＤ = H.コースＣＤ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 H.企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND H.部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND H.コースＣＤ IS NOT NULL" +
                                     "\r\n                 AND H.売上日 >= @期間開始日" +
                                     "\r\n                 AND H.売上日 <= @期間終了日" +
                                     "\r\n                 AND DATEPART(WEEKDAY, H.売上日) = DATEPART(WEEKDAY, @期間終了日)" +
                                     "\r\n                 AND SK.対象日 IS NULL" +             // -- 祝日は除く
                                     "\r\n                 AND C.コース区分 = @コース区分" +    // -- 曜日に応じたコース
                                     "\r\n                 AND ISNULL(C.工場フラグ, 0) = 0" +
                                     "\r\n                 AND ISNULL(C.無効フラグ, 0) = 0" +
                                     "\r\n             ) AS C," +
                                     // -- ②商品
                                     "\r\n             (SELECT" +
                                     "\r\n                 商品ＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 商品マスタ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                                     "\r\n                 AND 弁当区分 IN (@弁当区分副食, @弁当区分主食)" +    // -- 副食と主食のみ
                                     "\r\n             UNION" +
                                     "\r\n             SELECT" +
                                     "\r\n                 商品ＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 生産予定" +
                                     "\r\n             WHERE" +
                                     "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND 部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND 喫食日 = @注文日" +
                                     "\r\n             UNION" +
                                     // -- ②－１ 弁当の主食
                                     "\r\n             SELECT" +
                                     "\r\n                 I.主食ＣＤ AS 商品ＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 売上明細 AS M" +
                                     "\r\n                 INNER JOIN 売上ヘッダ AS H" +
                                     "\r\n                     ON H.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND H.売上日 = M.売上日" +
                                     "\r\n                     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                     "\r\n                     AND H.部署ＣＤ = M.部署ＣＤ" +
                                     "\r\n                 LEFT JOIN 祝日マスタ AS SK" +
                                     "\r\n                     ON SK.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND SK.対象日 = M.売上日" +
                                     "\r\n                 INNER JOIN 商品マスタ AS I" +
                                     "\r\n                     ON I.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                     "\r\n                     AND I.商品ＣＤ = M.商品ＣＤ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 M.企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND M.部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND H.コースＣＤ IS NOT NULL" +
                                     "\r\n                 AND M.売上日 >= @期間開始日" +
                                     "\r\n                 AND M.売上日 <= @期間終了日" +
                                     "\r\n                 AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)" +
                                     "\r\n                 AND SK.対象日 IS NULL" +                         // -- 祝日は除く
                                     "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +     // -- 弁当
                                     "\r\n                 AND ISNULL(M.主食ＣＤ, 0) <> 0" +                // -- 主食がある
                                     "\r\n                 AND ISNULL(M.主食ＣＤ, 0) <> M.商品ＣＤ" +       // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                     "\r\n             UNION" +
                                     // -- ②－２ 弁当の副食
                                     "\r\n             SELECT" +
                                     "\r\n                 I.副食ＣＤ AS 商品ＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 売上明細 AS M" +
                                     "\r\n                 INNER JOIN 売上ヘッダ AS H" +
                                     "\r\n                     ON H.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND H.売上日 = M.売上日" +
                                     "\r\n                     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                     "\r\n                     AND H.部署ＣＤ = M.部署ＣＤ" +
                                     "\r\n                 LEFT JOIN 祝日マスタ AS SK" +
                                     "\r\n                     ON SK.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND SK.対象日 = M.売上日" +
                                     "\r\n                 INNER JOIN 商品マスタ AS I" +
                                     "\r\n                     ON I.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                     "\r\n                     AND I.商品ＣＤ = M.商品ＣＤ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 M.企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND M.部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND H.コースＣＤ IS NOT NULL" +
                                     "\r\n                 AND M.売上日 >= @期間開始日" +
                                     "\r\n                 AND M.売上日 <= @期間終了日" +
                                     "\r\n                 AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)" +
                                     "\r\n                 AND SK.対象日 IS NULL" +                         // -- 祝日は除く
                                     "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +     // -- 弁当
                                     "\r\n                 AND ISNULL(M.副食ＣＤ, 0) <> 0" +                // -- 副食がある
                                     "\r\n                 AND ISNULL(M.副食ＣＤ, 0) <> M.商品ＣＤ" +       // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                     "\r\n             UNION" +
                                     // -- ②－３ 主食のみ
                                     "\r\n             SELECT" +
                                     "\r\n                 I.商品ＣＤ AS 商品ＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 売上明細 AS M" +
                                     "\r\n                 INNER JOIN 売上ヘッダ AS H" +
                                     "\r\n                     ON H.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND H.売上日 = M.売上日" +
                                     "\r\n                     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                     "\r\n                     AND H.部署ＣＤ = M.部署ＣＤ" +
                                     "\r\n                 LEFT JOIN 祝日マスタ AS SK" +
                                     "\r\n                     ON SK.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND SK.対象日 = M.売上日" +
                                     "\r\n                 INNER JOIN 商品マスタ AS I" +
                                     "\r\n                     ON I.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                     "\r\n                     AND I.商品ＣＤ = M.商品ＣＤ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 M.企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND M.部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND H.コースＣＤ IS NOT NULL" +
                                     "\r\n                 AND M.売上日 >= @期間開始日" +
                                     "\r\n                 AND M.売上日 <= @期間終了日" +
                                     "\r\n                 AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)" +
                                     "\r\n                 AND SK.対象日 IS NULL" +         // -- 祝日は除く
                                     "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食" +
                                     "\r\n             UNION" +
                                     // -- ②－４ 副食のみ
                                     "\r\n             SELECT" +
                                     "\r\n                 I.商品ＣＤ AS 商品ＣＤ" +
                                     "\r\n             FROM" +
                                     "\r\n                 売上明細 AS M" +
                                     "\r\n                 INNER JOIN 売上ヘッダ AS H" +
                                     "\r\n                     ON H.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND H.売上日 = M.売上日" +
                                     "\r\n                     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                     "\r\n                     AND H.部署ＣＤ = M.部署ＣＤ" +
                                     "\r\n                 LEFT JOIN 祝日マスタ AS SK" +
                                     "\r\n                     ON SK.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND SK.対象日 = M.売上日" +
                                     "\r\n                 INNER JOIN 商品マスタ AS I" +
                                     "\r\n                     ON I.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                     "\r\n                     AND I.商品ＣＤ = M.商品ＣＤ" +
                                     "\r\n             WHERE" +
                                     "\r\n                 M.企業ＣＤ = @企業ＣＤ" +
                                     "\r\n                 AND M.部署ＣＤ = @部署ＣＤ" +
                                     "\r\n                 AND H.コースＣＤ IS NOT NULL" +
                                     "\r\n                 AND M.売上日 >= @期間開始日" +
                                     "\r\n                 AND M.売上日 <= @期間終了日" +
                                     "\r\n                 AND DATEPART(WEEKDAY, M.売上日) = DATEPART(WEEKDAY, @期間終了日)" +
                                     "\r\n                 AND SK.対象日 IS NULL" +         // -- 祝日は除く
                                     "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食" +
                                     "\r\n             ) AS I" +
                                     "\r\n         ) AS A" +
                                     "\r\n         ON A.企業ＣＤ = M.企業ＣＤ" +
                                     "\r\n         AND A.コースＣＤ = M.コースＣＤ" +
                                     "\r\n         AND A.商品ＣＤ = M.商品ＣＤ" +
                                     "\r\n         AND A.持出日 = M.持出日";

                        StringBuilder sqlB = new StringBuilder(sql);
                        if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                        {
                            // -- ④コース別グループと結合
                            sqlB.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                            sqlB.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                            sqlB.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                            sqlB.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                            sqlB.AppendLine("         AND CG.コースＣＤ = A.コースＣＤ");

                            cmdDel.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                        }

                        cmdDel.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmdDel.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                        cmdDel.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                        cmdDel.Parameters.Add("@期間開始日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間開始日;
                        cmdDel.Parameters.Add("@期間終了日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間終了日;
                        cmdDel.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                        cmdDel.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                        cmdDel.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                        cmdDel.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);

                        cmdDel.CommandText = sqlB.ToString();

                        log.Debug(cmdDel.CommandTextWithParameters());

                        cmdDel.ExecuteNonQuery();

                        //*******************************************************************************
                        // cldtの行をループしつつ、コース一覧データテーブルをループしつつコースＣＤを取得
                        //*******************************************************************************
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["生産予定数入力"];

                        for (int rowIndex = 0; rowIndex < cldt.Rows.Count; rowIndex++)
                        {
                            int syohinCD = (int)cldt.Rows[rowIndex]["商品ＣＤ"];

                            foreach (DataRow courseDr in _courseDt.Rows)
                            {
                                int courseCD = (int)courseDr["コースＣＤ"];     // 列に該当するコースＣＤを取得

                                //*****************************************************
                                // cldtの行（商品）の、コースＣＤに該当する列の値を取得
                                //*****************************************************
                                int? suryo = (int?)cldt[rowIndex, courseCD];

                                if (suryo == null)
                                {
                                    continue;
                                }

                                //****************************
                                // モバイル_持ち出し入力を追加
                                //****************************
                                MobileMochidashi mobileMochidashi = new MobileMochidashi();
                                mobileMochidashi.コースＣＤ = courseCD;
                                mobileMochidashi.商品ＣＤ = syohinCD;
                                mobileMochidashi.持出日 = _seisanYoteiCriteria.注文日;
                                mobileMochidashi.個数 = suryo;

                                ret = mobileMochidashi.Insert(con, tran);
                                if (ret == false)
                                {
                                    MessageBox.Show(this, mobileMochidashi.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                historyProcess = History.ProcessType.Create;
                                History.Save(con, tran, this.Text, "モバイル_持ち出し入力", mobileMochidashi.Code, historyProcess);
                            }
                        }

                        historyProcess = History.ProcessType.Create;

                        tran.Commit();
                        log.InfoFormat("日配持出を{0}しました。", historyProcess.DisplayName());
                        MessageBox.Show(this, "日配持出に反映しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

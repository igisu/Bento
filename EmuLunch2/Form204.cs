using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 受注発信一覧
    /// </summary>
    public partial class Form204 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_BUSYO_INDEX = 0;       // 部署名列インデックス
        private const int COURSE_CD_INDEX    = 1;       // コースＣＤ列インデックス
        private const int COURSE_NAME_INDEX  = 2;       // コース名列インデックス

        private const int HASSIN_FUYO_COLUMN           = 0;    // 不要チェックボックス列インデックス
        private const int HASSIN_CHUMON_UMU_COLUMN     = 1;    // 注文有無列インデックス
        private const int HAISO_CHUMON_UMU_COLUMN      = 2;    // 配送有無列インデックス
        private const int HASSIN_SHOSAI_COLUMN         = 3;    // 詳細ボタン列インデックス
        private const int HASSIN_COURSE_KUBUN_COLUMN   = 4;    // コース区分列インデックス
        private const int HASSIN_COURSE_CD_COLUMN      = 5;    // コースＣＤ列インデックス
        private const int HASSIN_COURSE_NAME_COLUMN    = 6;    // コース名列インデックス
        private const int HASSIN_TOKUISAKI_CD_COLUMN   = 7;    // 得意先ＣＤ列インデックス
        private const int HASSIN_TOKUISAKI_NAME_COLUMN = 8;    // 得意先名列インデックス
        private const int HASSIN_TOKUISAKI_TEL_COLUMN  = 9;    // 得意先電話番号列インデックス
        private const int HASSIN_TEL_BIKO_COLUMN       = 10;   // 電話番号備考列インデックス
        private const int HASSIN_YOTEI_TIME_COLUMN     = 11;   // 発信予定時刻列インデックス
        private const int HASSIN_HORYU_COLUMN          = 12;   // 保留チェックボックス列インデックス
        private const int HASSIN_COMMENT_COLUMN        = 13;   // コメント列インデックス
        private const int HASSIN_HASSIN_COLUMN         = 14;   // 発信ボタン列インデックス
        private const int HASSIN_LAST_TIME_COLUMN      = 15;   // 最終発信時刻列インデックス
        private const int HASSIN_SYORICHU_COLUMN       = 16;   // 処理中フラグ列インデックス

        /// <summary>
        /// 保留の背景色
        /// </summary>
        private readonly Color HORYU_BACK_COLOR = Color.FromArgb(255, 150, 139);

        /// <summary>
        /// 注文済みの背景色（薄い緑）
        /// </summary>
        private readonly Color CHUMON_BACK_COLOR = Color.FromArgb(177, 249, 169);

        /// <summary>
        /// 配送済みの背景色（濃い緑）
        /// </summary>
        private readonly Color HAISO_BACK_COLOR = Color.FromArgb(111, 206, 101);

        /// <summary>
        /// 不要の背景色
        /// </summary>
        private readonly Color FUYO_BACK_COLOR = SystemColors.ControlLight;

        /// <summary>
        /// 処理中の背景色
        /// </summary>
        private readonly Color SYORICHU_BACK_COLOR = Color.Red;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 受注発信一覧検索条件
        /// </summary>
        private HassinListCriteria _hassinListCriteria;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// 再表示許可フラグ
        /// </summary>
        private bool _isAllowReload = true;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form204()
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
        private void Form204_Load(object sender, EventArgs e)
        {
            try
            {

                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                lblHoryu.BackColor = HORYU_BACK_COLOR;
                lblChumon.BackColor = CHUMON_BACK_COLOR;
                lblHaiso.BackColor = HAISO_BACK_COLOR;
                lblSyorichu.BackColor = SYORICHU_BACK_COLOR;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "発信時刻", "コース" },
                    new string[] { "T.電話確認時間_時 {0}, T.電話確認時間_分 {0}, CT.コースＣＤ, CT.コース順, H.電話番号",
                                   "CT.コースＣＤ {0}, CT.コース順, H.電話番号, T.電話確認時間_時, T.電話確認時間_分"});

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_Hassin.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_Hassin.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_Hassin.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // スプレッドの処理中フラグを非表示にする
                fpSpread_Hassin_Sheet1.Columns[HASSIN_SYORICHU_COLUMN].Visible = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _hassinListCriteria = new HassinListCriteria();
                hassinListBindingSource.DataSource = _hassinListCriteria;

                // 担当者架電コースマスタを取得
                GetTantoKadenCourse();

                //btnQuery.PerformClick();    // 検索ボタン押下
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
        private void Form204_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F12:
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_Hassin.EditMode == true)
                    {
                        fpSpread_Hassin.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// コース選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourse_Click(object sender, EventArgs e)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                Form204C form = new Form204C();
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form204C_FormClosed);
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
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form204C_FormClosed(object sender, FormClosedEventArgs e)
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

                // 担当者架電コースマスタを取得
                GetTantoKadenCourse();

                btnQuery.PerformClick();    // 検索ボタン押下
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
        /// 再表示間隔 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInterval_Validated(object sender, EventArgs e)
        {
            try
            {
                // 再表示間隔が有効ならタイマーを開始する
                if (_hassinListCriteria.再表示間隔 != null && _hassinListCriteria.再表示間隔 > 0)
                {
                    timer1.Interval = (int)_hassinListCriteria.再表示間隔 * 1000;
                    timer1.Enabled = true;
                }
                else
                {
                    timer1.Enabled = false;
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
                if (_hassinListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_hassinListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _hassinListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_hassinListCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        timer1.Enabled = false;
                        // 一覧初期化
                        hassinBindingSource.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // （不要フラグなどの）初期状態の得意先発信履歴データを作成
                InsertInitialHassin();

                // 得意先発信履歴データを取得
                GetJuchuHassinList(true);

                // 再表示間隔が有効ならタイマーを開始する
                if (_hassinListCriteria.再表示間隔 != null && _hassinListCriteria.再表示間隔 > 0)
                {
                    timer1.Interval = (int)_hassinListCriteria.再表示間隔 * 1000;
                    timer1.Enabled = true;
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
        /// 発信Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Hassin_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 不要チェックボックス？
                if (e.Column == HASSIN_FUYO_COLUMN)
                {
                    ChangeRowColor(e.Row);
                }

                // 詳細ボタン？
                if (e.Column == HASSIN_SHOSAI_COLUMN)
                {
                    // 注文が存在しない場合は処理中フラグを立てる
                    bool isUpdateSyorichuOnClose = false;
                    if (fpSpread_Hassin_Sheet1.Cells[e.Row, HASSIN_CHUMON_UMU_COLUMN].Value.ToString() != "済")
                    {
                        isUpdateSyorichuOnClose = true;
                        if (UpdateSyorichu(true, false) == false)
                        {
                            // 既に立っていた場合は再読込
                            Reload(true);
                            return;
                        }
                    }

                    OpenForm200BySpread(isUpdateSyorichuOnClose);   // 注文入力を表示
                }

                // 保留チェックボックス？
                if (e.Column == HASSIN_HORYU_COLUMN)
                {
                    ChangeRowColor(e.Row);
                }

                // 発信ボタン？
                if (e.Column == HASSIN_HASSIN_COLUMN)
                {
                    // 処理中フラグを立てる
                    if (UpdateSyorichu(true, false) == false)
                    {
                        // 既に立っていた場合は再読込
                        Reload(true);
                        return;
                    }

                    // CTIを用いて得意先に電話を掛ける
                    if (CallToTokuisaki(e.Row))
                    {
                        fpSpread_Hassin_Sheet1.Cells[fpSpread_Hassin_Sheet1.ActiveRowIndex, HASSIN_LAST_TIME_COLUMN].Value = DateTime.Now;  // dt_ColumnChangedが発生
                        OpenForm200BySpread(true);      // 注文入力を表示
                    }
                    else
                    {
                        UpdateSyorichu(false, true);    // 処理中フラグを落とす
                    }
                }
            }
            catch (EmuException ex)
            {
                UpdateSyorichu(false, true);            // 処理中フラグを落とす
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                UpdateSyorichu(false, true);            // 処理中フラグを落とす
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Spreadから注文入力画面を表示する
        /// </summary>
        /// <param name="isUpdateSyorichuOnClose">注文画面が閉じられたときに処理中フラグを更新する場合はtrue</param>
        private void OpenForm200BySpread(bool isUpdateSyorichuOnClose)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                DataRow dr = ((DataRowView)hassinBindingSource.Current).Row;
                ChumonMeisaiCriteria chumonCriteria = new ChumonMeisaiCriteria();
                chumonCriteria.stopOverwrite = true;
                chumonCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                chumonCriteria.得意先名 = dr["得意先名"] as string;
                chumonCriteria.部署ＣＤ = dr["部署ＣＤ"] as int?;
                chumonCriteria.注文日 = _hassinListCriteria.注文日;
                chumonCriteria.コースＣＤ = dr["コースＣＤ"] as int?;   // デフォルト表示するコース

                // 無効な得意先CDの場合はNOP
                if (chumonCriteria.得意先ＣＤ == null || chumonCriteria.得意先ＣＤ < 0)
                {
                    return;
                }

                Form200 formChumon = new Form200(chumonCriteria);
                if (formChumon.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 処理中フラグを落とす対象の得意先発信履歴データを注文入力画面に憶えさせる
                TokuisakiHassinRireki tokuisakiHassinRireki = new TokuisakiHassinRireki(dr);
                formChumon.TokuisakiHassinRireki = tokuisakiHassinRireki;

                // 2016/08/23 REP S モーダルにせず、複数開けるようにする
                //SubFormOpenInit(formChumon);
                formChumon.MdiParent = this.MdiParent;
                // 2016/08/23 REP E
                formChumon.ListOpened = true;
                formChumon.Show();
                if(isUpdateSyorichuOnClose)
                {
                    // 注文画面が閉じられた場合に処理中フラグを更新する（落とす）
                    formChumon.FormClosed += new FormClosedEventHandler(Form200_FormClosedAndUpdateSyorichu);
                }
                else
                {
                    formChumon.FormClosed += new FormClosedEventHandler(Form200_FormClosed);
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
        /// 注文入力画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form200_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 一覧画面を閉じた後に詳細画面で閉じられた場合、一覧画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();

                // 再読込
                Reload(true);
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
        /// 注文入力画面クローズ（処理中フラグを更新する（落とす））
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form200_FormClosedAndUpdateSyorichu(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 処理中フラグを落とす対象の得意先発信履歴データを注文入力画面から取得
                Form200 formChumon = (Form200)sender;
                TokuisakiHassinRireki tokuisakiHassinRireki = formChumon.TokuisakiHassinRireki;
                UpdateSyorichu(tokuisakiHassinRireki, false, true); // 処理中フラグを落とす

                // 一覧画面を閉じた後に詳細画面で閉じられた場合、一覧画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();

                Reload(true);                                       // 再読込
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
        /// 発信Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Hassin_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_Hassin.EditMode)
                {
                    fpSpread_Hassin.CancelCellEditing();
                }
                else
                {
                    fpSpread_Hassin.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Hassin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Hassin_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Hassin_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終発信時刻列には移動しない（次の行へ移動する）
                    if (newColumnIndex >= HASSIN_LAST_TIME_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Hassin_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Hassin_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Hassin_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);
                fpSpread_Hassin.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_Hassin_Sheet1.ActiveColumnIndex)
                {
                    case HASSIN_COMMENT_COLUMN:    // コメント
                        colIndex = fpSpread_Hassin_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    log.Debug("！！！セル削除！！！");
                    // いったん処理中フラグを立てる
                    if (UpdateSyorichu(true, false) == false)
                    {
                        // 既に立っていた場合は再読込
                        Reload(true);
                        return;
                    }

                    fpSpread_Hassin_Sheet1.Cells[fpSpread_Hassin_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 発信Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Hassin_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_Hassin.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                //***************************************************************
                // セル編集開始のタイミングで処理中フラグを立てる。
                // 既に立てられていた場合はセルの編集をキャンセルして読み込み直す
                //***************************************************************
                log.Debug("！！！セル編集開始！！！");
                // 処理中フラグを立てる
                if (UpdateSyorichu(true, false) == false)
                {
                    // 既に立っていた場合は再読込
                    Reload(true);
                    return;
                }

                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Hassin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_Hassin.EditingControl.KeyDown += new KeyEventHandler(fpSpread_HassinEdit_KeyDown);
            }
        }

        /// <summary>
        /// 発信Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Hassin_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_Hassin.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_Hassin, fpSpread_Hassin_Sheet1);

                //***************************************************************
                // セル編集終了のタイミングで処理中フラグを落とす。
                // （セル（コメント）編集完了時に処理中フラグを落としている）
                // ここでは特に何もしない
                //***************************************************************
                log.Debug("！！！セル編集終了！！！");
                UpdateSyorichu(false, true);            // 処理中フラグを落とす

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_Hassin.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_HassinEdit_KeyDown);
            }
        }

        /// <summary>
        /// Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_HassinEdit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_Hassin.EditMode)
                    {
                        fpSpread_Hassin.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Hassin.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Hassin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Hassin_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Hassin_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終発信時刻列には移動しない（次の行へ移動する）
                    if (newColumnIndex >= HASSIN_LAST_TIME_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Hassin_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Hassin_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Hassin_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);
                fpSpread_Hassin.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 発信Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Hassin_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            if (hassinBindingSource != null)
            {
                DataRow dr = ((DataRowView)hassinBindingSource.Current).Row;
                dr.EndEdit();
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

        /// <summary>
        /// タイマー発生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // 再表示処理停止中？（得意先発信履歴データの処理中フラグが立っている）
                if (_isAllowReload == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 自分がアクティブではない場合、アクティブなフォームのフォーカスが消える（奪われる？）ことがあるのでそれを回避する
                // 自分がアクティブな場合は非活性にする必要はない（むしろ非活性にすると別のフォームがアクティブになる）
                if (this.MdiParent.ActiveMdiChild != this)
                {
                    this.Enabled = false;
                }

                log.Debug("タイマーによる再読込");
                Reload(false);  // 再読込

                this.Enabled = true;

                this.MdiParent.Cursor = Cursors.Default;
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
        /// 担当者架電コースマスタに該当するコースマスタ取得
        /// </summary>
        private void GetTantoKadenCourse()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     B.部署名" +
                                      "\r\n     ,K.担当者ＣＤ" +
                                      "\r\n     ,K.コースＣＤ" +
                                      "\r\n     ,C.コース名" +
                                      "\r\n     ,K.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,K.作成日" +
                                      "\r\n     ,K.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,K.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     担当者架電コースマスタ AS K" +
                                      "\r\n     LEFT JOIN コースマスタ AS C ON C.企業ＣＤ = K.企業ＣＤ AND C.コースＣＤ = K.コースＣＤ" +
                                      "\r\n     LEFT JOIN 部署マスタ   AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = C.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = K.企業ＣＤ AND K.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = K.企業ＣＤ AND K.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     K.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND K.担当者ＣＤ = @担当者ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     K.コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("担当者架電コースマスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    courseBindingSource.DataSource = ds;
                    courseBindingSource.DataMember = "担当者架電コースマスタ";

                    log.Info("担当者架電コースマスタを検索しました。");
                }

                fpSpread_Course_Sheet1.BindDataColumn(COURSE_BUSYO_INDEX, "部署名");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_CD_INDEX,    "コースＣＤ");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_NAME_INDEX,  "コース名");
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
        /// 初期状態の得意先発信履歴データを作成
        /// </summary>
        /// <returns>正常終了の場合は true。それ以外は false</returns>
        private bool InsertInitialHassin()
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n INSERT INTO 得意先発信履歴データ (" +
                                      "\r\n     企業ＣＤ" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n     ,発信日" +
                                      "\r\n     ,電話番号" +
                                      "\r\n     ,最終発信日時" +
                                      "\r\n     ,担当者ＣＤ" +
                                      "\r\n     ,処理中フラグ" +
                                      "\r\n     ,不要フラグ" +
                                      "\r\n     ,保留フラグ" +
                                      "\r\n     ,コメント" +
                                      "\r\n     ,電話番号備考" +
                                      "\r\n     ,作成担当者ＣＤ" +
                                      "\r\n     ,作成日" +
                                      "\r\n     ,修正担当者ＣＤ" +
                                      "\r\n     ,修正日" +
                                      "\r\n ) SELECT DISTINCT" +
                                      "\r\n     K.企業ＣＤ," +
                                      "\r\n     T.得意先ＣＤ," +
                                      "\r\n     @注文日 AS 発信日," +
                                      "\r\n     TC.Tel_TelNo AS 電話番号," +
                                      "\r\n     NULL AS 最終発信日時," +
                                      "\r\n     1 AS 担当者ＣＤ," +
                                      "\r\n     0 AS 処理中フラグ," +
                                      "\r\n     ISNULL(CASE WHEN SK.対象日 IS NOT NULL THEN T.祝日配送区分 ELSE SUBSTRING(T.休日設定, DATEPART(WEEKDAY, @注文日), 1) END, 0) AS 不要フラグ," +
                                      "\r\n     0 AS 保留フラグ," +
                                      "\r\n     NULL AS コメント," +
                                      "\r\n     TC.Tel_Remarks AS 電話番号備考," +
                                      "\r\n     @担当者ＣＤ AS 作成担当者ＣＤ," +
                                      "\r\n     getdate() AS 作成日," +
                                      "\r\n     @担当者ＣＤ AS 修正担当者ＣＤ," +
                                      "\r\n     getdate() AS 修正日" +
                                      "\r\n FROM" +
                                      "\r\n     担当者架電コースマスタ AS K" +
                                      "\r\n     INNER JOIN" +
                                      // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n         (SELECT" +
                                      "\r\n             C.企業ＣＤ" +
                                      "\r\n             ,C.コースＣＤ" +
                                      "\r\n             ,C.得意先ＣＤ" +
                                      "\r\n             ,C.コース順" +
                                      "\r\n         FROM" +
                                      "\r\n             コーステーブル AS C" +
                                      "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = C.企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             LEFT JOIN" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     コースＣＤ" +
                                      "\r\n                 FROM" +
                                      "\r\n                     臨時コースマスタ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                     AND 実施日 = @注文日" +
                                      "\r\n                     AND コース区分 = @コース区分" +
                                      "\r\n                 ) AS RM" +
                                      "\r\n                 ON RM.コースＣＤ = C.コースＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             C.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(C.無効フラグ, 0) = 0" +
                                      "\r\n             AND CM.コース区分 = @コース区分" +
                                      "\r\n             AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             RT.企業ＣＤ" +
                                      "\r\n             ,RT.コースＣＤ" +
                                      "\r\n             ,RT.得意先ＣＤ" +
                                      "\r\n             ,RT.コース順" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コーステーブル AS RT" +
                                      "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n         WHERE" +
                                      "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n             AND RT.実施日 = @注文日" +
                                      "\r\n             AND RM.コース区分 = @コース区分" +
                                      "\r\n         ) AS CT" +
                                      "\r\n         ON CT.コースＣＤ = K.コースＣＤ" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.得意先ＣＤ = CT.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN C_TelToCust  AS TC ON TC.企業ＣＤ = T.企業ＣＤ AND TC.Tel_CustNo = T.得意先ＣＤ AND TC.Tel_RepFlg = 0" +
                                      "\r\n     LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = T.企業ＣＤ AND SK.対象日 = @注文日" +
                                      "\r\n     LEFT JOIN 得意先発信履歴データ AS H ON H.企業ＣＤ = K.企業ＣＤ AND H.得意先ＣＤ = CT.得意先ＣＤ AND H.発信日 = @注文日" +
                                      "\r\n WHERE" +
                                      "\r\n     K.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND K.担当者ＣＤ = @担当者ＣＤ" +
                                      "\r\n     AND T.受注方法 = @受注区分電話要" +
                                      "\r\n     AND H.発信日 IS NULL" +
                                      "\r\n     AND TC.Tel_TelNo IS NOT NULL";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _hassinListCriteria.注文日;
                    cmd.Parameters.Add("@受注区分電話要", SqlDbType.Int).Value = (int)Consts.JuchuKubun.Call;

                    // 注文日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun((DateTime)_hassinListCriteria.注文日);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    cmd.ExecuteNonQuery();

                    log.Info("得意先発信履歴データの初期値を作成しました。");
                }

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("得意先発信履歴データが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("得意先発信履歴データの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                MessageBox.Show(string.Format("医院単価マスタの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 受注発信一覧（得意先発信履歴データ）を取得
        /// </summary>
        /// <param name="isAllowLog">ログを出力する場合はtrue</param>
        private void GetJuchuHassinList(bool isAllowLog)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     H.企業ＣＤ");
                    sql.AppendLine("     ,CT.コースＣＤ");
                    sql.AppendLine("     ,H.得意先ＣＤ");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,H.電話番号");
                    sql.AppendLine("     ,CASE WHEN CH.配送日 IS NULL THEN '未' ELSE '済' END AS 注文状況");
                    sql.AppendLine("     ,CASE WHEN UH.売上日 IS NULL THEN '未' ELSE '済' END AS 配送状況");
                    sql.AppendLine("     ,VC.コース区分名");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine("     ,CM.部署ＣＤ");
                    sql.AppendLine("     ,T.電話確認時間_時");
                    sql.AppendLine("     ,T.電話確認時間_分");
                    sql.AppendLine("     ,RIGHT('00' + CONVERT(varchar, T.電話確認時間_時), 2) + ':' + RIGHT('00' + CONVERT(varchar, T.電話確認時間_分), 2) AS 電話確認時間");
                    sql.AppendLine("     ,H.発信日");
                    sql.AppendLine("     ,H.最終発信日時");
                    sql.AppendLine("     ,H.担当者ＣＤ");
                    sql.AppendLine("     ,H.処理中フラグ");
                    sql.AppendLine("     ,H.不要フラグ");
                    sql.AppendLine("     ,H.保留フラグ");
                    sql.AppendLine("     ,H.コメント");
                    sql.AppendLine("     ,H.電話番号備考");
                    sql.AppendLine("     ,H.作成担当者ＣＤ");
                    sql.AppendLine("     ,H.作成日");
                    sql.AppendLine("     ,H.修正担当者ＣＤ");
                    sql.AppendLine("     ,H.修正日");
                    sql.AppendLine(" FROM");
                    // -- ①得意先発信履歴データは得意先ＣＤ、担当者架電コースマスタはコースＣＤしか持たないので、完全外部結合して②と内部結合する
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         H.企業ＣＤ");
                    sql.AppendLine("         ,H.得意先ＣＤ");
                    sql.AppendLine("         ,H.発信日");
                    sql.AppendLine("         ,H.電話番号");
                    sql.AppendLine("         ,H.最終発信日時");
                    sql.AppendLine("         ,H.担当者ＣＤ");
                    sql.AppendLine("         ,H.処理中フラグ");
                    sql.AppendLine("         ,H.不要フラグ");
                    sql.AppendLine("         ,H.保留フラグ");
                    sql.AppendLine("         ,H.コメント");
                    sql.AppendLine("         ,H.電話番号備考");
                    sql.AppendLine("         ,H.作成担当者ＣＤ");
                    sql.AppendLine("         ,H.作成日");
                    sql.AppendLine("         ,H.修正担当者ＣＤ");
                    sql.AppendLine("         ,H.修正日");
                    sql.AppendLine("         ,K.コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         得意先発信履歴データ AS H");
                    sql.AppendLine("         LEFT JOIN 担当者架電コースマスタ AS K ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND H.発信日 = @注文日");
                    sql.AppendLine("         AND K.担当者ＣＤ = @担当者ＣＤ");
                    sql.AppendLine("     ) AS H");
                    // -- ②コースＣＤと得意先ＣＤの組み合わせ
                    sql.AppendLine("     INNER JOIN");
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.企業ＣＤ");
                    sql.AppendLine("             ,C.コースＣＤ");
                    sql.AppendLine("             ,C.得意先ＣＤ");
                    sql.AppendLine("             ,C.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コーステーブル AS C");
                    sql.AppendLine("             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = C.企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     臨時コースマスタ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND 実施日 = @注文日");
                    sql.AppendLine("                     AND コース区分 = @コース区分");
                    sql.AppendLine("                 ) AS RM");
                    sql.AppendLine("                 ON RM.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             C.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(C.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND CM.コース区分 = @コース区分");
                    sql.AppendLine("             AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             RT.企業ＣＤ");
                    sql.AppendLine("             ,RT.コースＣＤ");
                    sql.AppendLine("             ,RT.得意先ＣＤ");
                    sql.AppendLine("             ,RT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コーステーブル AS RT");
                    sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RT.実施日 = @注文日");
                    sql.AppendLine("             AND RM.コース区分 = @コース区分");
                    sql.AppendLine("         ) AS CT");
                    sql.AppendLine("         ON CT.コースＣＤ = H.コースＣＤ AND CT.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T  ON  T.企業ＣＤ = H.企業ＣＤ AND T.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN V_コース区分 AS VC ON VC.コース区分CD = CM.コース区分");
                    sql.AppendLine("     LEFT JOIN 注文ヘッダ   AS CH ON CH.企業ＣＤ = H.企業ＣＤ AND CH.注文区分 = @注文区分注文 AND CH.喫食日 = H.発信日 AND CH.得意先ＣＤ = H.得意先ＣＤ AND CH.部署ＣＤ = CM.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 売上ヘッダ   AS UH ON UH.企業ＣＤ = H.企業ＣＤ AND UH.得意先ＣＤ = H.得意先ＣＤ AND UH.売上日 = H.発信日 AND UH.部署ＣＤ = CM.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");
                    if (_hassinListCriteria.不要非表示 != null && _hassinListCriteria.不要非表示 == true)
                    {
                        sql.AppendLine("     AND ISNULL(H.不要フラグ, 0) = 0");     // 不要フラグが立っていないものだけを表示
                    }
                    if (_hassinListCriteria.注文済み非表示 != null && _hassinListCriteria.注文済み非表示 == true)
                    {
                        sql.AppendLine("     AND CH.配送日 IS NULL");               // 注文が無いものだけを表示
                    }
                    if (_hassinListCriteria.配送済み非表示 != null && _hassinListCriteria.配送済み非表示 == true)
                    {
                        sql.AppendLine("     AND UH.売上日 IS NULL");               // 売上が無いものだけを表示
                    }
                    sql.AppendLine(" ORDER BY");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _hassinListCriteria.注文日;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;

                    // 注文日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun((DateTime)_hassinListCriteria.注文日);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    cmd.CommandText = sql.ToString();

                    if (isAllowLog)
                    {
                        log.Debug(cmd.CommandTextWithParameters());
                    }

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("受注発信一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    hassinBindingSource.DataSource = ds;
                    hassinBindingSource.DataMember = "受注発信一覧";

                    if (isAllowLog)
                    {
                        log.Info("受注発信一覧リストを検索しました。");
                    }
                }

                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_FUYO_COLUMN,           "不要フラグ");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_CHUMON_UMU_COLUMN,     "注文状況");
                fpSpread_Hassin_Sheet1.BindDataColumn(HAISO_CHUMON_UMU_COLUMN,      "配送状況");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_COURSE_KUBUN_COLUMN,   "コース区分名");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_COURSE_NAME_COLUMN,    "コース名");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_TOKUISAKI_TEL_COLUMN,  "電話番号");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_TEL_BIKO_COLUMN,       "電話番号備考");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_YOTEI_TIME_COLUMN,     "電話確認時間");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_HORYU_COLUMN,          "保留フラグ");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_COMMENT_COLUMN,        "コメント");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_LAST_TIME_COLUMN,      "最終発信日時");
                fpSpread_Hassin_Sheet1.BindDataColumn(HASSIN_SYORICHU_COLUMN,       "処理中フラグ");

                // 行の内容に応じて背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread_Hassin_Sheet1.Rows.Count; rowIndex++)
                {
                    ChangeRowColor(rowIndex);
                }

                fpSpread_Hassin.Focus();
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

                bool syorichuFlgUpdate = false;         // 処理中フラグ更新内容
                bool syorichuFlgCondition = false;      // 処理中フラグ更新条件
                switch (e.Column.ColumnName)
                {
                    case "不要フラグ":
                    case "保留フラグ":
                        syorichuFlgUpdate = false;
                        syorichuFlgCondition = false;
                        break;

                    case "コメント":
                        log.Debug("！！！コメント編集完了！！！");
                        syorichuFlgUpdate = false;
                        syorichuFlgCondition = true;
                        break;

                    case "最終発信日時":
                        syorichuFlgUpdate = true;
                        syorichuFlgCondition = true;    // 発信ボタンを押下した時点で処理中フラグを立てているので
                        break;
                }

                // 得意先発信履歴データを更新
                Save(syorichuFlgUpdate, syorichuFlgCondition, e.Column.ColumnName);

                // 再読込
                Reload(true);
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
        /// 発信Spreadの行の背景色を変更する
        /// </summary>
        /// <param name="rowIndex">スプレッドの行インデックス</param>
        private void ChangeRowColor(int rowIndex)
        {
            try
            {
                // 行の背景色を初期化
                fpSpread_Hassin_Sheet1.Rows[rowIndex].ResetForeColor();
                fpSpread_Hassin_Sheet1.Rows[rowIndex].BackColor = Color.Empty;  // 念のため

                // 保留？
                if (fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HORYU_COLUMN].Value as bool? == true)
                {
                    fpSpread_Hassin_Sheet1.Rows[rowIndex].BackColor = HORYU_BACK_COLOR;
                }

                // 注文済み？
                if (fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_CHUMON_UMU_COLUMN].Value as string == "済")
                {
                    // 背景色を変える
                    fpSpread_Hassin_Sheet1.Rows[rowIndex].BackColor = CHUMON_BACK_COLOR;
                }

                // 配送済み？
                if (fpSpread_Hassin_Sheet1.Cells[rowIndex, HAISO_CHUMON_UMU_COLUMN].Value as string == "済")
                {
                    // 背景色を変える
                    fpSpread_Hassin_Sheet1.Rows[rowIndex].BackColor = HAISO_BACK_COLOR;
                }

                // 発信ボタンを取得
                FarPoint.Win.Spread.CellType.ButtonCellType buttonCellType =
                    (FarPoint.Win.Spread.CellType.ButtonCellType)fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HASSIN_COLUMN].CellType;
                if (buttonCellType == null)
                {
                    buttonCellType = new FarPoint.Win.Spread.CellType.ButtonCellType();
                    buttonCellType.Text = "発信";
                }

                // 不要？
                if (fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_FUYO_COLUMN].Value as bool? == true)
                {
                    // 背景色を変える
                    fpSpread_Hassin_Sheet1.Rows[rowIndex].BackColor = FUYO_BACK_COLOR;

                    // 発信ボタンを非活性（したように見せる）
                    fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HASSIN_COLUMN].CanFocus = false;
                    fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HASSIN_COLUMN].Locked = true;
                    buttonCellType.TextColor = SystemColors.GrayText;
                }
                else
                {
                    //詳細ボタンを活性化（したように見せる）
                    fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HASSIN_COLUMN].CanFocus = true;
                    fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HASSIN_COLUMN].Locked = false;
                    buttonCellType.TextColor = SystemColors.ControlText;
                }
                fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_HASSIN_COLUMN].CellType = buttonCellType;

                // 処理中？
                if (fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_SYORICHU_COLUMN].Value as bool? == true)
                {
                    // 背景色を変える
                    fpSpread_Hassin_Sheet1.Rows[rowIndex].BackColor = SYORICHU_BACK_COLOR;
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
        /// 得意先に電話を掛ける
        /// </summary>
        /// <param name="rowIndex">スプレッドの行インデックス</param>
        /// <returns>電話を掛けた場合は True</returns>
        private bool CallToTokuisaki(int rowIndex)
        {
            try
            {
                // 発信済み？
                if (fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_LAST_TIME_COLUMN].Value != null)
                {
                    if (MessageBox.Show(this, "この得意先は発信済みです。\n再度発信しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return false;
                    }
                }

                string telNo = fpSpread_Hassin_Sheet1.Cells[rowIndex, HASSIN_TOKUISAKI_TEL_COLUMN].Value as string;
                CallTo(telNo);      // 発信

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
        /// <param name="syorichuFlgUpdate">処理中フラグ更新内容</param>
        /// <param name="syorichuFlgCondition">処理中フラグ更新条件</param>
        /// <param name="columnName">更新対象の列名</param>
        /// <returns>成功した場合は True</returns>
        private bool Save(bool syorichuFlgUpdate, bool syorichuFlgCondition, string columnName)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Update;

                DataRow dr = ((DataRowView)hassinBindingSource.Current).Row;

                TokuisakiHassinRireki tokuisakiHassinRireki = new TokuisakiHassinRireki(dr);

                tokuisakiHassinRireki.処理中フラグ = syorichuFlgUpdate;
                tokuisakiHassinRireki.処理中フラグ条件 = syorichuFlgCondition;

                bool ret = false;
                switch (columnName)
                {
                    case "不要フラグ":
                        ret = tokuisakiHassinRireki.UpdateFuyoFlg();
                        break;

                    case "保留フラグ":
                        ret = tokuisakiHassinRireki.UpdateHoryuFlg();
                        break;

                    case "コメント":
                        ret = tokuisakiHassinRireki.UpdateComment();
                        break;

                    case "最終発信日時":
                        ret = tokuisakiHassinRireki.UpdateLastUpdateTime();
                        break;
                }

                if (ret == true)
                {
                    History.Save(this.Text, "得意先発信履歴データ", tokuisakiHassinRireki.Code, historyProcess);
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
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// 得意先発信履歴データの処理中フラグを更新
        /// </summary>
        /// <param name="syorichuFlgUpdate">処理中フラグ更新内容</param>
        /// <param name="syorichuFlgCondition">処理中フラグ更新条件</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateSyorichu(bool syorichuFlgUpdate, bool syorichuFlgCondition)
        {
            try
            {
                if (hassinBindingSource == null)
                {
                    return false;
                }
                // カレント行を処理対象の得意先発信履歴データとする
                DataRow dr = ((DataRowView)hassinBindingSource.Current).Row;
                TokuisakiHassinRireki tokuisakiHassinRireki = new TokuisakiHassinRireki(dr);

                return UpdateSyorichu(tokuisakiHassinRireki, syorichuFlgUpdate, syorichuFlgCondition);
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
        /// 得意先発信履歴データの処理中フラグを更新
        /// </summary>
        /// <param name="tokuisakiHassinRireki">処理対象の得意先発信履歴データ</param>
        /// <param name="syorichuFlgUpdate">処理中フラグ更新内容</param>
        /// <param name="syorichuFlgCondition">処理中フラグ更新条件</param>
        /// <returns>成功した場合は True</returns>
        private bool UpdateSyorichu(TokuisakiHassinRireki tokuisakiHassinRireki, bool syorichuFlgUpdate, bool syorichuFlgCondition)
        {
            try
            {
                tokuisakiHassinRireki.処理中フラグ = syorichuFlgUpdate;
                tokuisakiHassinRireki.処理中フラグ条件 = syorichuFlgCondition;

                // 処理中フラグを立てる場合は、再表示を許可しない
                // （処理中でなくなれば再表示してよい）
                _isAllowReload = !(syorichuFlgUpdate);

                bool ret = tokuisakiHassinRireki.UpdateSyorichu();
                if (ret == false)
                {
                    // 処理中フラグの更新に失敗した場合は再表示を許可する
                    // ※処理中フラグが立てば再表示しないが、立てられなければ再表示しないといけない
                    // ※処理中フラグを落とすのに失敗した場合は、すでに落ちているか、別の誰かが立てたものなので、やはり再表示しないといけない
                    _isAllowReload = true;
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
                History.Error(ex);
                throw new EmuException(ex);
            }
        }

        /// <summary>
        /// 再読み込み
        /// </summary>
        /// <param name="isAllowLog">ログを出力する場合はtrue</param>
        private void Reload(bool isAllowLog)
        {
            try
            {
                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread_Hassin.GetViewportTopRow(0);
                int activeRowIndex = fpSpread_Hassin_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread_Hassin_Sheet1.ActiveColumnIndex;

                // 得意先発信履歴データを取得
                GetJuchuHassinList(isAllowLog);

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread_Hassin.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread_Hassin_Sheet1.Rows.Count - fpSpread_Hassin_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread_Hassin_Sheet1.Rows.Count - fpSpread_Hassin_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread_Hassin_Sheet1.ActiveRowIndex = activeRowIndex;
                    hassinBindingSource.Position = activeRowIndex;      // Currentプロパティがずれる場合があるのを回避
                }
                fpSpread_Hassin_Sheet1.ActiveColumnIndex = activeColumnIndex;
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 一括コース注文入力
    /// </summary>
    public partial class Form208 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN   = 0;     // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 1;     // 得意先名列インデックス
        private const int URIGEN_KUBUN_COLUMN   = 2;     // 売上現金区分列インデックス
        private const int CHUMON_TOTAL_COLUMN   = 3;     // 注文計列インデックス
        private const int BIKO_COLUMN           = 4;     // 備考列インデックス
        private const int CHUMON_UMU_COLUMN     = 5;     // 注文有無フラグ列インデックス
        private const int COURSE_CD_COLUMN      = 6;     // コースＣＤ列インデックス
        private const int KYUJITSU_COLUMN       = 7;     // 休日設定列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN   = 8;     // 商品名列開始列インデックス

        /// <summary>
        /// 注文有りの背景色
        /// </summary>
        private readonly Color CHUMON_BACK_COLOR = Color.PapayaWhip;

        /// <summary>
        /// 別コースで注文有りの背景色
        /// </summary>
        private readonly Color OTHER_COURSE_BACK_COLOR = Color.Lavender;

        /// <summary>
        /// 得意先の休日の背景色
        /// </summary>
        private readonly Color HOLIDAY_BACK_COLOR = Color.LightPink;

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 一括コース注文入力検索条件
        /// </summary>
        private IkkatsuCourseChumonCriteria _ikkatsuCourseChumonCriteria;

        /// <summary>
        /// 一括コース注文入力検索条件検索実行時スナップショット
        /// </summary>
        private IkkatsuCourseChumonCriteria _ikkatsuCourseChumonCriteriaSnapShot;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        // Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        private bool _isF2forSpreadEdit = false;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = SYOHIN_START_COLUMN;

        // デフォルトデータモデル（セルデータ一括クリア用）
        private FarPoint.Win.Spread.Model.DefaultSheetDataModel _dataModel;

        /// <summary>
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称、商品単価）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        /// <summary>
        /// 得意先単価マスタ（単価取得用）データテーブル
        /// </summary>
        private DataTable _tokuisakiTankaDt = null;

        /// <summary>
        /// （注文日・配送日時点の）消費税
        /// </summary>
        private Syohizei _syohizei;

        /// <summary>
        /// （製造日／配送日算出日数取得用の）部署マスタ
        /// </summary>
        private Busyo _busyo;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form208()
        {
            try
            {
                InitializeComponent();
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
        private void Form208_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

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
                _ikkatsuCourseChumonCriteria = new IkkatsuCourseChumonCriteria();
                ikkatsuCourseChumonCriteriaBindingSource.DataSource = _ikkatsuCourseChumonCriteria;

                //***********************************************************************************
                // Spreadの行のロック(編集不可)を有効にするためにシートのプロテクト(保護)を有効にする
                // ただし、Excel出力するとシートのあらゆる書式や設定が変更できなくなるので、
                // Excel出力の前後でプロテクト状態を切り替える(□)
                //***********************************************************************************
                fpSpread1_Sheet1.Protect = true;

                // スプレッドの得意先ＣＤ列・注文有無フラグ列・コースＣＤ列・休日設定列を非表示にする
                //fpSpread1_Sheet1.Columns[TOKUISAKI_CD_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[CHUMON_UMU_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[COURSE_CD_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[KYUJITSU_COLUMN].Visible = false;

                // 動的に追加される列以外を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = SYOHIN_START_COLUMN;

                // 常に更新
                _IsNewRecord = false;

                // デフォルトデータモデルを取得
                _dataModel = (FarPoint.Win.Spread.Model.DefaultSheetDataModel)fpSpread1_Sheet1.Models.Data;

                // 「本日ではありません」ラベルのVisibleを設定
                ToggleChumonbiIsNotToday();
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
        private void Form208_Shown(object sender, EventArgs e)
        {
            // コースにフォーカス
            txtCourseCD.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form208_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F9:
                    if (fpSpread1.EditMode == true)
                    {
                        fpSpread1.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }

                    btnSave.PerformClick();         // 更新ボタン押下
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
        private void Form208_FormClosing(object sender, FormClosingEventArgs e)
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
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
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
        /// 注文日テキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpChumonbi_Validated(object sender, EventArgs e)
        {
            try
            {
                // 「本日ではありません」ラベルのVisibleを設定
                ToggleChumonbiIsNotToday();

                // 検索条件が変わったら画面をクリア
                if (_ikkatsuCourseChumonCriteriaSnapShot != null && _ikkatsuCourseChumonCriteria.Equals(_ikkatsuCourseChumonCriteriaSnapShot) == false)
                {
                    Clear();
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
        /// コースＣＤテキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // 検索条件が変わったら画面をクリア
                if (_ikkatsuCourseChumonCriteriaSnapShot != null && _ikkatsuCourseChumonCriteria.Equals(_ikkatsuCourseChumonCriteriaSnapShot) == false)
                {
                    Clear();
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
                    txtCourseCD.Text = course.コースＣＤ.ToString();
                    _ikkatsuCourseChumonCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;

                    // 以下、IkkatsuCourseChumonCriteriaによって設定された内容を画面に反映する
                    ReadBinding(lblShitenName);
                    ReadBinding(lblBusyoName);
                    ReadBinding(txtTantosyaCD);
                    ReadBinding(lblTantosyaName);
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
                if (_ikkatsuCourseChumonCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_ikkatsuCourseChumonCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _ikkatsuCourseChumonCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_ikkatsuCourseChumonCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        Clear();

                        // 未検索状態にする
                        _ikkatsuCourseChumonCriteriaSnapShot = null;

                        return;
                    }
                }

                // タイトルバーにコース名を表示
                this.Text = string.Format("{0} - 一括コース注文入力", lblCourseName.Text);

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 消費税を取得
                _syohizei = Syohizei.GetSyohizei((DateTime)_ikkatsuCourseChumonCriteria.注文日);

                // 部署マスタを取得（製造日／配送日算出日数の取得用）
                _busyo = Busyo.GetBusyo((int)_ikkatsuCourseChumonCriteria.部署ＣＤ);

                // 注文一覧を表示
                GetChumon();

                //*************************************************************************************************************
                // 注文日(=喫食日)と配送日がずれる場合はどうする？
                // 算出した配送日に該当するコースで注文を登録すると、検索条件として表示されているコースと異なることになるので、
                // 再表示したときに入力した値が表示されないことになる
                // （算出したコースで検索すると表示される）
                // だからといって検索条件のコースをそのまま登録すると、配送日とは異なるコースになってしまう。
                // →配送日が異なる設定の部署については、登録できないようにする（表示ボタン押下時に更新ボタンを非活性にする）
                //   具体的には、以下の条件を全て満たす場合のみ登録可能とする
                //   ・製造日算出日数=0
                //   ・配送日算出日数=0
                //   ・注文日と同じ曜日（祝日の場合は祝日）の製造稼働日がON
                //   ・注文日と同じ曜日（祝日の場合は祝日）の配送稼働日がON
                //   （得意先休日考慮フラグ=trueの場合は、その日が休日の得意先については入力不可とする）
                //*************************************************************************************************************
                Consts.EmuDayOfWeek dayOfWeek = (Consts.EmuDayOfWeek)((DateTime)_ikkatsuCourseChumonCriteria.注文日).DayOfWeek;
                Shukujitsu shukujitsu = GetShukujitsu((DateTime)_ikkatsuCourseChumonCriteria.注文日);  // 祝日を取得
                if (shukujitsu != null)
                {
                    dayOfWeek = Consts.EmuDayOfWeek.Holiday;
                }
                if (_busyo.製造日算出日数 == 0 &&
                    _busyo.配送日算出日数 == 0 &&
                    _busyo.GetSeizo(dayOfWeek) == true &&
                    _busyo.GetHaiso(dayOfWeek) == true)
                {
                    btnSave.Enabled = this.HasAuthEdit;
                }
                else
                {
                    btnSave.Enabled = false;
                    MessageBox.Show(this, "この画面では注文日と配送日が一致するコース／部署のみ注文を入力できます。\n" +
                                          "指定したコース／部署は注文日と配送日が一致しない場合があるため、注文を入力することができません。\n" +
                                          "代わりに他の注文入力画面を使用してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
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
                        newColumnIndex = SYOHIN_START_COLUMN;
                        // ただし最終行は合計行なので移動しない
                        if (newRowIndex < fpSpread1_Sheet1.Rows.Count - 2)
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
                // 選択範囲をまとめてクリアする
                // GetSelections()を使用して選択情報を取得する
                // ※ISheetSelectionModelインタフェースを利用して選択情報を取得すると、列の選択と先頭セルのみの選択とが区別できないが、
                //   いったんこれを利用して取得しておくと、単一セルでもGetSelections()で選択範囲が取得できる。
                //   （ISheetSelectionModelを使用せずにGetSelections()を使うと、単一セルの場合に要素ゼロのセル配列が返ってくる）
                int anchorRow = fpSpread1_Sheet1.Models.Selection.AnchorRow;
                FarPoint.Win.Spread.Model.CellRange[] cells = fpSpread1_Sheet1.GetSelections();
                if (cells != null && cells.Length > 0)
                {
                    FarPoint.Win.Spread.Model.CellRange cell = cells[0];
                    int row = cell.Row;
                    int rowCount = cell.RowCount;
                    int column = cell.Column;
                    int columnCount = cell.ColumnCount;

                    // ClearDataはマイナス値も受け入れるが、セルの表示が更新されない（＝消えずに残ったように見える）ことがある（別のセルをクリックすると再表示される）
                    // 明示的に指定するとちゃんと消える
                    if (row < 0) row = 0;
                    if (rowCount < 0) rowCount = fpSpread1_Sheet1.Rows.Count;
                    if (columnCount < 0) columnCount = fpSpread1_Sheet1.Columns.Count;
                    if (column < SYOHIN_START_COLUMN)
                    {
                        columnCount = (column + columnCount) - SYOHIN_START_COLUMN;     // 列位置をSYOHIN_START_COLUMNにするため、列数を調整する
                        column = SYOHIN_START_COLUMN;
                    }

                    // _dataModel.ClearDataは、セルのプロパティに関わらず、その値を消してしまう
                    // Locked=true なセルについては、その値を消さないようにする
                    //_dataModel.ClearData(row, column, rowCount, columnCount);
                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            if (fpSpread1_Sheet1.Cells[row + i, column + j].Locked == false)
                            {
                                fpSpread1_Sheet1.Cells[row + i, column + j].Value = null;       // // dt_ColumnChangedが発生
                            }
                        }
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
                        newColumnIndex = SYOHIN_START_COLUMN;
                        // ただし最終行は合計行なので移動しない
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
        /// 更新ボタン
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
                    // 再読込
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
        /// 注文一覧を表示
        /// </summary>
        private void GetChumon()
        {
            try
            {
                // 注文日の曜日を取得（得意先の休日判定に使用する）
                DayOfWeek dayOfWeek = ((DateTime)_ikkatsuCourseChumonCriteria.注文日).DayOfWeek;

                // Spreadの内部状態が壊れてColumnCountやRowCountを設定するとIndexOutOfRangeExceptionが発生することがあるので、それを回避すべく、DataSourceをリセットする
                bindingSource1.DataSource = null;

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                //***************************************************
                // 主要商品リストマスタにある商品名を列として追加し、
                // コーステーブルにある得意先を行とする
                // データそのものは注文明細から取得する
                //***************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 同じタイミングで得意先単価マスタも取得する
                _tokuisakiTankaDt = GetTokuisakiTankaTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetChumonSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                DataTable dt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     C.得意先ＣＤ" +
                                      "\r\n     ,T.得意先名" +
                                      "\r\n     ,ISNULL(T.税区分, @税区分内税) AS 税区分" +
                                      "\r\n     ,ISNULL(T.丸め処理区分, @端数区分切り捨て) AS 丸め処理区分" +
                                      "\r\n     ,VK.売掛現金区分名 AS 種類" +
                                      "\r\n     ,ISNULL(CK.注文計, 0) AS 注文計" +
                                      "\r\n     ,ISNULL(H.備考1, T.備考1) AS 備考" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,A.商品数量" +
                                      "\r\n     ,CASE WHEN CK.注文計 IS NULL THEN 0 ELSE 1 END AS 注文有無フラグ" +
                                      "\r\n     ,CASE WHEN A.商品数量 IS NULL THEN C.コースＣＤ ELSE A.コースＣＤ END AS コースＣＤ" +  // -- 駆動表に紐付く予測注文レコードのコースＣＤがNULLの場合はそのまま取得したい
                                      "\r\n     ,T.休日設定" +
                                      "\r\n     ,CO.コース順" +
                                      "\r\n FROM" +
                                      // -- ①駆動表
                                      // -- ①－１（指定されたコースにぶら下がる得意先）
                                      // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n     (SELECT" +
                                      "\r\n         CT.企業ＣＤ" +
                                      "\r\n         ,CT.コースＣＤ" +
                                      "\r\n         ,CT.得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         コーステーブル AS CT" +
                                      "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         LEFT JOIN" +
                                      "\r\n             (SELECT" +
                                      "\r\n                 コースＣＤ" +
                                      "\r\n             FROM" +
                                      "\r\n                 臨時コースマスタ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                 AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                 AND 実施日 = @注文日" +
                                      "\r\n             ) AS RM" +
                                      "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND CT.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n         AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         RT.企業ＣＤ" +
                                      "\r\n         ,RT.コースＣＤ" +
                                      "\r\n         ,RT.得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         臨時コーステーブル AS RT" +
                                      "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n     WHERE" +
                                      "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND RT.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n         AND RT.実施日 = @注文日" +
                                      // -- ①－２（指定されたコースを持つ注文の得意先）
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         企業ＣＤ" +
                                      "\r\n         ,コースＣＤ" +
                                      "\r\n         ,得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         注文ヘッダ" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND 喫食日 = @注文日" +
                                      "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     ) AS C" +
                                      "\r\n     INNER JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN V_売掛現金区分 AS VK ON VK.売掛現金区分CD = ISNULL(T.売掛現金区分, 0)" +      // -- デフォルトは現金
                                      // -- ②駆動表に紐付く予測と注文
                                      // -- 商品ＣＤのアンダーバー以降は
                                      // -- 0:予測 1:注文
                                      // -- コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する。ただし、検索条件のコースかどうかを示すためにコースCDは取得する(△)
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             ISNULL(HC.コースＣＤ, @コースＣＤ) AS コースＣＤ" +     // -- コースＣＤは注文のものを優先し、予測から取得する場合は検索条件のコースとする
                                      "\r\n             ,M.得意先ＣＤ" +
                                      "\r\n             ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n             ,M.商品数量" +
                                      "\r\n         FROM" +
                                      "\r\n             注文ヘッダ AS HY" +
                                      "\r\n             INNER JOIN 注文明細 AS M" +
                                      "\r\n                 ON  HY.企業ＣＤ   = M.企業ＣＤ" +
                                      "\r\n                 AND HY.注文区分   = M.注文区分" +
                                      "\r\n                 AND HY.喫食日     = M.喫食日" +
                                      "\r\n                 AND HY.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n                 AND HY.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n             LEFT JOIN 注文ヘッダ AS HC" +
                                      "\r\n                 ON  HY.企業ＣＤ   = HC.企業ＣＤ" +
                                      "\r\n                 AND HY.喫食日     = HC.喫食日" +
                                      "\r\n                 AND HY.得意先ＣＤ = HC.得意先ＣＤ" +
                                      "\r\n                 AND HY.部署ＣＤ   = HC.部署ＣＤ" +
                                      "\r\n                 AND HC.注文区分   = @注文区分注文" +
                                      "\r\n         WHERE" +
                                      "\r\n             HY.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND HY.喫食日 = @注文日" +
                                      "\r\n             AND HY.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND HY.注文区分 = @注文区分予測" +       // -- 予測
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             H.コースＣＤ" +
                                      "\r\n             ,M.得意先ＣＤ" +
                                      "\r\n             ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n             ,M.商品数量" +
                                      "\r\n         FROM" +
                                      "\r\n             注文ヘッダ AS H" +
                                      "\r\n             INNER JOIN 注文明細 AS M" +
                                      "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                      "\r\n                 AND H.注文区分   = M.注文区分" +
                                      "\r\n                 AND H.喫食日     = M.喫食日" +
                                      "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND H.喫食日 = @注文日" +
                                      "\r\n             AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND H.注文区分 = @注文区分注文" +       // -- 注文
                                      "\r\n         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ" +
                                      // -- ③備考取得用（コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する）
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             得意先ＣＤ" +
                                      "\r\n             ,備考1" +
                                      "\r\n         FROM" +
                                      "\r\n             注文ヘッダ" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND 注文区分 = @注文区分注文" +     // -- 注文
                                      "\r\n             AND 喫食日 = @注文日" +
                                      "\r\n             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n         ) AS H" +
                                      "\r\n         ON H.得意先ＣＤ = C.得意先ＣＤ" +
                                      // -- ④注文計（コース注文入力とは異なり、コースを無視して(コースの結合はせずに)注文を取得する）
                                      // -- 注文画面で単価が入力されることがあるので、この画面で求めた注文計とは異なることがある(★)
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             M.得意先ＣＤ" +
                                      "\r\n             ,SUM(M.商品金額合計税込) AS 注文計" +
                                      "\r\n         FROM" +
                                      "\r\n             注文ヘッダ AS H" +
                                      "\r\n             INNER JOIN 注文明細 AS M" +
                                      "\r\n                 ON  H.企業ＣＤ   = M.企業ＣＤ" +
                                      "\r\n                 AND H.注文区分   = M.注文区分" +
                                      "\r\n                 AND H.喫食日     = M.喫食日" +
                                      "\r\n                 AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n                 AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND H.喫食日 = @注文日" +
                                      "\r\n             AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND M.注文区分 = @注文区分注文" +   // -- 注文
                                      "\r\n         GROUP BY" +
                                      "\r\n             M.得意先ＣＤ" +
                                      "\r\n         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ" +
                                      // -- ⑤コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             CT.得意先ＣＤ" +
                                      "\r\n             ,CT.コース順" +
                                      "\r\n         FROM" +
                                      "\r\n             コーステーブル AS CT" +
                                      "\r\n             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n             LEFT JOIN" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     コースＣＤ" +
                                      "\r\n                 FROM" +
                                      "\r\n                     臨時コースマスタ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                     AND 実施日 = @注文日" +
                                      "\r\n                 ) AS RM" +
                                      "\r\n                 ON RM.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND CT.コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n             AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             RT.得意先ＣＤ" +
                                      "\r\n             ,RT.コース順" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コーステーブル AS RT" +
                                      "\r\n             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n         WHERE" +
                                      "\r\n             RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND RT.コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n             AND RT.実施日 = @注文日" +
                                      "\r\n         ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ" +
                                      // -- 合計行
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     NULL AS 得意先ＣＤ" +
                                      "\r\n     ,'合計' AS 得意先名" +
                                      "\r\n     ,NULL AS 税区分" +
                                      "\r\n     ,NULL AS 丸め処理区分" +
                                      "\r\n     ,NULL AS 種類" +
                                      "\r\n     ,NULL AS 注文計" +
                                      "\r\n     ,NULL AS 備考" +
                                      "\r\n     ,NULL AS 商品ＣＤ" +
                                      "\r\n     ,NULL AS 商品数量" +
                                      "\r\n     ,NULL AS 注文有無フラグ" +
                                      "\r\n     ,NULL AS コースＣＤ" +
                                      "\r\n     ,NULL AS 休日設定" +
                                      "\r\n     ,999999999 AS コース順" +
                                      "\r\n ORDER BY" +
                                      "\r\n     CO.コース順" +
                                      "\r\n     ,C.得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _ikkatsuCourseChumonCriteria.注文日;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    cmd.Parameters.Add("@税区分内税", SqlDbType.Int).Value = (int)Syohizei.ZeiKubun.Uchi;
                    cmd.Parameters.Add("@端数区分切り捨て", SqlDbType.Int).Value = (int)Rounding.HasuKubun.Floor;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("一括コース注文入力リストを検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("一括コース注文入力");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["得意先ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["得意先名"]);
                cldt.AddFixedColumn(dt.Columns["税区分"]);
                cldt.AddFixedColumn(dt.Columns["丸め処理区分"]);
                cldt.AddFixedColumn(dt.Columns["種類"]);
                cldt.AddFixedColumn(dt.Columns["注文計"]);
                cldt.AddFixedColumn(dt.Columns["備考"]);
                cldt.AddFixedColumn(dt.Columns["注文有無フラグ"]);
                cldt.AddFixedColumn(dt.Columns["コースＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["休日設定"]);
                // インポート
                cldt.Import(dt, "商品ＣＤ", "商品数量");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                fpSpread1.SuspendLayout();      // 描画を一時停止

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "一括コース注文入力";

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(URIGEN_KUBUN_COLUMN,   "種類");
                fpSpread1_Sheet1.BindDataColumn(CHUMON_TOTAL_COLUMN,   "注文計");
                fpSpread1_Sheet1.BindDataColumn(BIKO_COLUMN,           "備考");
                fpSpread1_Sheet1.BindDataColumn(CHUMON_UMU_COLUMN,     "注文有無フラグ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KYUJITSU_COLUMN,       "休日設定");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                {
                    // 注文がある場合の背景色をセット
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;
                    if ((int)(fpSpread1_Sheet1.Cells[rowIndex, CHUMON_UMU_COLUMN].Value) == 1)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = CHUMON_BACK_COLOR;
                    }

                    // コースが異なる場合の背景色をセット(△)
                    if (_ikkatsuCourseChumonCriteria.コースＣＤ != fpSpread1_Sheet1.Cells[rowIndex, COURSE_CD_COLUMN].Value as int?)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = OTHER_COURSE_BACK_COLOR;

                        // レコードを取得してエラーを設定
                        DataRow dr = cldt.Rows[rowIndex];
                        dr.SetColumnError("得意先名", "この得意先の注文は別のコースで登録されています。");
                    }

                    // 得意先の休日の場合の色をセット
                    string kyujitsuSettei = (string)fpSpread1_Sheet1.Cells[rowIndex, KYUJITSU_COLUMN].Value;
                    if (Tokuisaki.GetHoliday(kyujitsuSettei, dayOfWeek))
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = HOLIDAY_BACK_COLOR;

                        for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                        {
                            // 入力不可にする(□)
                            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Locked = true;
                        }

                        // レコードを取得してエラーを設定
                        DataRow dr = cldt.Rows[rowIndex];
                        dr.SetColumnError("得意先名", "この得意先の休日なのでこの画面では入力できません。");
                    }
                }

                // 予測列については入力不可に(ロック)する
                for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                {
                    if (columnIndex % 2 == 0)
                    {
                        for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                        {
                            // 入力不可にする(□)
                            fpSpread1_Sheet1.Cells[rowIndex, columnIndex].Locked = true;
                        }
                    }
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                    }
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _ikkatsuCourseChumonCriteriaSnapShot = new IkkatsuCourseChumonCriteria(_ikkatsuCourseChumonCriteria);

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
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
                fpSpread1.ResumeLayout(true);       // 描画再開
            }
        }

        /// <summary>
        /// 注文スプレッドの列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns>対象の商品名が格納されたデータテーブル</returns>
        private DataTable GetSyohinTable()
        {
            DataTable dt = new DataTable("商品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // 主要商品リストマスタにある商品と、予測と注文の商品を以下のように並べる
                    // ┌─────┬─────┬─────┐
                    // │  商品Ａ  │  商品Ｂ  │  商品Ｃ  │
                    // ├──┬──┼──┬──┼──┬──┤
                    // │予測│注文│予測│注文│予測│注文│
                    // └──┴──┴──┴──┴──┴──┘
                    // 商品ＣＤのアンダーバー以降は
                    // 0:予測 1:注文
                    // -- ①主要商品リストマスタから(予測用)
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,S.商品単価税込" +
                                      "\r\n     ,S.商品単価税抜" +
                                      "\r\n     ,S.軽減税率フラグ" +
                                      "\r\n     ,'予測' AS 予測注文" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ②注文レコードから(予測用)
                                      "\r\n SELECT" +
                                      "\r\n     M.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,S.商品単価税込" +
                                      "\r\n     ,S.商品単価税抜" +
                                      "\r\n     ,S.軽減税率フラグ" +
                                      "\r\n     ,'予測' AS 予測注文" +
                                      "\r\n FROM" +
                                      "\r\n     注文ヘッダ AS H" +
                                      "\r\n     INNER JOIN 注文明細 AS M" +
                                      "\r\n         ON H.企業ＣＤ = M.企業ＣＤ" +
                                      "\r\n         AND H.注文区分 = M.注文区分" +
                                      "\r\n         AND H.喫食日 = M.喫食日" +
                                      "\r\n         AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n         AND H.部署ＣＤ = M.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.喫食日 = @注文日" +
                                      "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n UNION" +
                                      // -- ③主要商品リストマスタから(注文用)
                                      "\r\n SELECT" +
                                      "\r\n     A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,S.商品単価税込" +
                                      "\r\n     ,S.商品単価税抜" +
                                      "\r\n     ,S.軽減税率フラグ" +
                                      "\r\n     ,'注文' AS 予測注文" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ④注文レコードから(注文用)
                                      "\r\n SELECT" +
                                      "\r\n     M.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,S.商品単価税込" +
                                      "\r\n     ,S.商品単価税抜" +
                                      "\r\n     ,S.軽減税率フラグ" +
                                      "\r\n     ,'注文' AS 予測注文" +
                                      "\r\n FROM" +
                                      "\r\n     注文ヘッダ AS H" +
                                      "\r\n     INNER JOIN 注文明細 AS M" +
                                      "\r\n         ON H.企業ＣＤ = M.企業ＣＤ" +
                                      "\r\n         AND H.注文区分 = M.注文区分" +
                                      "\r\n         AND H.喫食日 = M.喫食日" +
                                      "\r\n         AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n         AND H.部署ＣＤ = M.部署ＣＤ" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.喫食日 = @注文日" +
                                      "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     ソート用商品ＣＤ" +
                                      "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _ikkatsuCourseChumonCriteria.注文日;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.コースＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("商品一覧を取得しました。");
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
        /// 表示対象の得意先に該当する得意先単価マスタをデータテーブルとして取得する
        /// </summary>
        /// <returns>対象の得意先単価マスタが格納されたデータテーブル</returns>
        private DataTable GetTokuisakiTankaTable()
        {
            DataTable dt = new DataTable("得意先単価マスタ");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    // 指定されたコースにぶら下がる得意先と指定されたコースを持つ注文の得意先に該当する得意先単価マスタを取得する
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     TT.企業ＣＤ" +
                                      "\r\n     ,TT.得意先ＣＤ" +
                                      "\r\n     ,TT.商品ＣＤ" +
                                      "\r\n     ,TT.商品単価税込" +
                                      "\r\n     ,TT.商品単価税抜" +
                                      "\r\n     ,TT.無効フラグ" +
                                      "\r\n     ,TT.作成担当者ＣＤ" +
                                      "\r\n     ,TT.作成日" +
                                      "\r\n     ,TT.修正担当者ＣＤ" +
                                      "\r\n     ,TT.修正日" +
                                      "\r\n FROM" +
                                      // -- ①駆動表
                                      // -- ①－１（指定されたコースにぶら下がる得意先）
                                      // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n     (SELECT" +
                                      "\r\n         CT.企業ＣＤ" +
                                      "\r\n         ,CT.得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         コーステーブル AS CT" +
                                      "\r\n         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         LEFT JOIN" +
                                      "\r\n             (SELECT" +
                                      "\r\n                 コースＣＤ" +
                                      "\r\n             FROM" +
                                      "\r\n                 臨時コースマスタ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n                 AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                 AND 実施日 = @注文日" +
                                      "\r\n             ) AS RM" +
                                      "\r\n             ON RM.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND CT.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n         AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         RT.企業ＣＤ" +
                                      "\r\n         ,RT.得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         臨時コーステーブル AS RT" +
                                      "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n     WHERE" +
                                      "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND RT.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n         AND RT.実施日 = @注文日" +
                                      // -- ①－２（指定されたコースを持つ注文の得意先）
                                      "\r\n     UNION" +
                                      "\r\n     SELECT" +
                                      "\r\n         企業ＣＤ" +
                                      "\r\n         ,得意先ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         注文ヘッダ" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND 喫食日 = @注文日" +
                                      "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     ) AS C" +
                                      // -- ②駆動表に紐付く得意先単価マスタ
                                      "\r\n     INNER JOIN 得意先単価マスタ AS TT ON TT.企業ＣＤ = C.企業ＣＤ AND TT.得意先ＣＤ = C.得意先ＣＤ AND ISNULL(TT.無効フラグ, 0) = 0";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _ikkatsuCourseChumonCriteria.注文日;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.コースＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("得意先単価マスタを取得しました。");
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
        /// 商品名一覧のデータテーブルを基にして、注文スプレッドに列を追加する
        /// </summary>
        /// <param name="syohinDt">商品名一覧データテーブル</param>
        /// <param name="labelColumnName">商品名が格納されている列の名前</param>
        private void SetChumonSpreadColumn(DataTable syohinDt, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    if (columnIndex % 2 == 0)
                    {
                        // 予測
                        FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                        numberCellType.DecimalPlaces = 0;
                        numberCellType.EnableSubEditor = false;
                        numberCellType.MaximumValue = 99999D;
                        numberCellType.MinimumValue = 0D;
                        numberCellType.ReadOnly = true;     // 読み取り専用にするが…
                        numberCellType.Static = true;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Label = "予測";
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Width = 50F;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).BackColor = Color.WhiteSmoke;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
                    }
                    else
                    {
                        // 注文
                        FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                        numberCellType.DecimalPlaces = 0;
                        numberCellType.EnableSubEditor = false;
                        numberCellType.MaximumValue = 99999D;
                        numberCellType.MinimumValue = 0D;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Label = "注文";
                        fpSpread1_Sheet1.Columns.Get(columnIndex).Width = 50F;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ForeColor = Color.Red;        // 赤
                        fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                        fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);

                        // 予測と注文が揃ったので、列ヘッダーを設定
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex - 1).ColumnSpan = 2;
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex - 1).Value = dr[labelColumnName].ToString();
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex - 1).Value = "予測";
                        fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex).Value = "注文";

                        // TODO:商品ごとに列の背景色を変える？
                    }

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
                fpSpread1.SuspendLayout();      // 個数を消したときに注文計が再描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                // 得意先の金額の合計を算出
                DataSet ds = (DataSet)bindingSource1.DataSource;
                ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["一括コース注文入力"];
                CalcRow(cldt, e.Row);
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
                fpSpread1.ResumeLayout(true);       // 個数を消したときに注文計が再描画されないことがあるのでそれを回避する
            }
        }

        /// <summary>
        /// 行を計算
        /// </summary>
        /// <param name="cldt">商品の列位置(=数量)取得用</param>
        /// <param name="dr">計算対象行</param>
        private void CalcRow(ColumnListDataTable cldt, DataRow dr)
        {
            try
            {
                decimal chumonTotal = 0m;
                int? tokuisakiCD = dr["得意先ＣＤ"] as int?;
                if (tokuisakiCD == null)
                {
                    return;
                }
                int zeiKubun = (int)dr["税区分"];
                int hasuKubun = (int)dr["丸め処理区分"];

                // 取得済みの商品について
                for (int i = 0; i < _syohinDt.Rows.Count; i++)
                {
                    // 商品CDと商品単価(税込/税抜)と軽減税率フラグを取得
                    string syohinCDCode = (string)_syohinDt.Rows[i]["商品ＣＤ"];
                    if (syohinCDCode.EndsWith("_1") == false)     // 注文以外(=予測)は無視する
                    {
                        continue;
                    }
                    string targetSyohinCD = syohinCDCode.Substring(0, syohinCDCode.LastIndexOf("_"));    // 商品ＣＤのアンダーバー以降を除去
                    int syohinCD = int.Parse(targetSyohinCD);
                    decimal? taxIncludedPrice = _syohinDt.Rows[i]["商品単価税込"] as decimal?;
                    decimal? taxExcludedPrice = _syohinDt.Rows[i]["商品単価税抜"] as decimal?;
                    bool? isKeigen = _syohinDt.Rows[i]["軽減税率フラグ"] as bool?;

                    // 商品マスタから単価を決定
                    decimal? tanka = Syohizei.GetUnitPrice(zeiKubun, taxIncludedPrice, taxExcludedPrice);

                    // 得意先単価マスタから
                    TokuisakiTanka tokuisakiTanka = GetTokuisakiTanka((int)tokuisakiCD, syohinCD);
                    if (tokuisakiTanka != null)
                    {
                        taxIncludedPrice = tokuisakiTanka.商品単価税込;
                        taxExcludedPrice = tokuisakiTanka.商品単価税抜;
                        tanka = Syohizei.GetUnitPrice(zeiKubun, taxIncludedPrice, taxExcludedPrice);
                    }

                    // その商品と一致する列の数量を取得
                    int codeIndex = cldt.CodeToColumnIndex(syohinCDCode);
                    if (codeIndex < 0)
                    {
                        continue;
                    }
                    int? suryo = dr[codeIndex] as int?;     // 商品に該当する数量を取得

                    // 商品金額合計税込を算出
                    decimal kingaku = _syohizei.GetAmountIncludeTax(zeiKubun, suryo, tanka, isKeigen, hasuKubun);

                    chumonTotal += kingaku;
                }

                // 一覧表示時に取得した注文計は、注文画面で直接入力された単価を用いて算出されていることがあるので
                // ここで求めた注文計とは異なることがある（ここでは商品マスタor得意先単価マスタの単価を用いているので）(★)
                dr["注文計"] = chumonTotal;
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
        /// 得意先と商品に該当する得意先単価マスタを取得する
        /// </summary>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <param name="syohinCD">商品CD</param>
        /// <returns>得意先と商品に該当する得意先単価マスタ（該当するものが無い場合はnull）</returns>
        private TokuisakiTanka GetTokuisakiTanka(int tokuisakiCD, int syohinCD)
        {
            try
            {
                TokuisakiTanka tokuisakiTanka = null;
                DataRow[] drs = _tokuisakiTankaDt.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ={1}", tokuisakiCD, syohinCD));
                if (drs != null && drs.Length != 0)
                {
                    tokuisakiTanka = new TokuisakiTanka(drs[0]);
                }
                return tokuisakiTanka;
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
        /// 一括注文入力リストが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 一括注文入力リストを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["一括コース注文入力"];

            // 最終行(=合計行)を除いて、変更をチェック
            for (int rowIndex = 0; rowIndex < dt.Rows.Count - 1; rowIndex++)
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

                if (btnSave.Enabled == false)
                {
                    MessageBox.Show(this, "この画面では注文日と配送日が一致するコース／部署のみ注文を入力できます。\n" +
                                          "指定したコース／部署は注文日と配送日が一致しない場合があるため、注文を入力することができません。\n" +
                                          "代わりに他の注文入力画面を使用してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_ikkatsuCourseChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _ikkatsuCourseChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_ikkatsuCourseChumonCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_ikkatsuCourseChumonCriteria.Equals(_ikkatsuCourseChumonCriteriaSnapShot) == false)
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
                        // ・cldtの行をループしながら以下を実施
                        //   ・得意先に該当する予測レコードを全て取得
                        //   ・得意先に該当する注文レコードを全て取得
                        //   ・得意先に該当する注文明細を全て削除
                        //   ・cldtの行（得意先）の、注文ヘッダを取得
                        //   ・商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //   ・cldtの行（得意先）の、商品ＣＤに該当する列の値を取得
                        //   ・注文数が未入力の場合、注文日と商品ＣＤに該当する予測レコードがあれば、注文数を0とみなす（予測のみ存在する場合は0注文を作成する）
                        //   ・注文明細を追加
                        //   ・注文ヘッダが有れば更新、無ければ追加
                        // ・最後に、注文明細がない注文ヘッダを削除
                        //********************************************************************************************************

                        //***************************************************************************
                        // cldtの行をループしつつ、商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //***************************************************************************
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["一括コース注文入力"];

                        for (int rowIndex = 0; rowIndex < cldt.Rows.Count - 1; rowIndex++)      // 最終行は合計行なので-1
                        {
                            int tokuisakiCD = (int)cldt.Rows[rowIndex]["得意先ＣＤ"];           // 行に該当する得意先ＣＤを取得
                            Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki(tokuisakiCD);          // 得意先マスタ取得
                            if (tokuisaki == null)
                            {
                                log.WarnFormat("得意先マスタ無し({0})", tokuisakiCD);
                                continue;
                            }

                            //*****************************
                            // 注文明細の予測レコードを取得
                            //*****************************
                            DataTable dtYosoku = GetChumonMeisai(tokuisakiCD, Consts.ChumonKubun.Yosoku);

                            //***************************************************
                            // 注文明細の注文レコードを取得（入力内容との比較用）
                            //***************************************************
                            DataTable dtChumon = GetChumonMeisai(tokuisakiCD, Consts.ChumonKubun.Chuumon);
                            DataRow[] drs;      // 上記テーブルに対してSelectした結果

                            //*******************
                            // 注文明細を全て削除
                            //*******************
                            SqlCommand cmdDelM = con.CreateCommand();
                            cmdDelM.CommandText = "\r\n DELETE M" +
                                                  "\r\n FROM 注文明細 AS M" +
                                                  "\r\n INNER JOIN 注文ヘッダ AS H" +
                                                  "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                                  "\r\n     AND H.注文区分   = M.注文区分" +
                                                  "\r\n     AND H.喫食日     = M.喫食日" +
                                                  "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                                  "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                                  "\r\n WHERE M.企業ＣＤ = @企業ＣＤ" +
                                                  "\r\n   AND M.注文区分 = @注文区分注文" +
                                                  "\r\n   AND M.喫食日 = @喫食日" +
                                                  "\r\n   AND M.得意先ＣＤ = @得意先ＣＤ" +
                                                  "\r\n   AND M.部署ＣＤ = @部署ＣＤ";

                            cmdDelM.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                            cmdDelM.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                            cmdDelM.Parameters.Add("@喫食日", SqlDbType.Date).Value = _ikkatsuCourseChumonCriteria.注文日;
                            cmdDelM.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                            cmdDelM.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                            cmdDelM.Transaction = tran;
                            log.Debug(cmdDelM.CommandTextWithParameters());
                            cmdDelM.ExecuteNonQuery();

                            DateTime kissyokubi = (DateTime)_ikkatsuCourseChumonCriteria.注文日;
                            DateTime haisobi = CalcHaisobi(kissyokubi, _busyo, tokuisaki);           // 配送日を算出
                            int seq = 0;

                            // 注文ヘッダ更新フラグを初期化
                            bool needChumonHeaderUpdate = false;

                            //*****************
                            // 注文ヘッダを取得
                            //*****************
                            ChumonHeader chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Chuumon, kissyokubi, tokuisakiCD, (int)_ikkatsuCourseChumonCriteria.部署ＣＤ);
                            if (chumonHeader == null)
                            {
                                chumonHeader = new ChumonHeader();
                                chumonHeader.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                chumonHeader.喫食日 = kissyokubi;
                                chumonHeader.得意先ＣＤ = tokuisakiCD;
                                chumonHeader.部署ＣＤ = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                                chumonHeader.配送日 = haisobi;
                                chumonHeader.納品書伝票番号 = null;

                                // 得意先マスタから
                                chumonHeader.得意先名 = tokuisaki.得意先名;
                                chumonHeader.配送先ＣＤ = tokuisaki.受注得意先ＣＤ;
                                chumonHeader.税区分 = tokuisaki.税区分 ?? (int)Syohizei.ZeiKubun.Uchi;
                                chumonHeader.丸め処理区分 = tokuisaki.丸め処理区分 ?? (int)Rounding.HasuKubun.Floor;

                                chumonHeader.備考1 = tokuisaki.備考１;
                                chumonHeader.備考2 = tokuisaki.備考２;
                                chumonHeader.備考3 = tokuisaki.備考３;
                                chumonHeader.備考4 = null;
                                chumonHeader.備考5 = null;
                            }

                            //***************************************************
                            // 商品一覧データテーブルをループしつつ商品ＣＤを取得
                            //***************************************************
                            foreach (DataRow syohinDr in _syohinDt.Rows)
                            {
                                // 列に該当する商品ＣＤを取得
                                string syohinCDCode = (string)syohinDr["商品ＣＤ"];
                                if (syohinCDCode.EndsWith("_1") == false)     // 注文以外(=予測)は無視する
                                {
                                    continue;
                                }
                                string targetSyohinCD = syohinCDCode.Substring(0, syohinCDCode.LastIndexOf("_"));    // 商品ＣＤのアンダーバー以降を除去
                                int syohinCD = int.Parse(targetSyohinCD);

                                //*****************************************************
                                // cldtの行（得意先）の、商品ＣＤに該当する列の値を取得
                                //*****************************************************
                                int? suryo = (int?)cldt[rowIndex, syohinCDCode];

                                if (suryo == null)
                                {
                                    //**************************************************************
                                    // 注文数が未入力の場合、予測レコードがあれば、注文数を0とみなす
                                    // （予測のみ存在する場合は0注文を作成する）
                                    // 予測が存在しない場合は何もしない（次のレコードへ）
                                    //**************************************************************
                                    drs = dtYosoku.Select(string.Format("喫食日=#{0:yyyy/MM/dd}# and 商品ＣＤ={1}", kissyokubi, syohinCD));
                                    if (drs == null || drs.Length == 0)
                                    {
                                        // 既存の注文明細が存在していれば、注文を削除しようとしていることなので、注文ヘッダ（の修正日）を更新する(フラグを立てる)
                                        drs = dtChumon.Select(string.Format("喫食日=#{0:yyyy/MM/dd}# and 商品ＣＤ={1}", kissyokubi, syohinCD));
                                        if (drs != null && drs.Length != 0)
                                        {
                                            needChumonHeaderUpdate = true;
                                        }

                                        continue;
                                    }
                                    suryo = 0;
                                }

                                // 注文明細を追加または更新することが確定したので、既存の注文明細が存在しないか異なっていれば、注文ヘッダ（の修正日）を更新する(フラグを立てる)
                                drs = dtChumon.Select(string.Format("喫食日=#{0:yyyy/MM/dd}# and 商品ＣＤ={1}", kissyokubi, syohinCD));
                                if (drs == null || drs.Length == 0)
                                {
                                    // 存在しないので追加しようとしている
                                    needChumonHeaderUpdate = true;
                                }
                                else
                                {
                                    // 存在していて、数量が異なる？
                                    if (((decimal)drs[0]["商品数量"]) != (decimal)suryo)
                                    {
                                        needChumonHeaderUpdate = true;
                                    }
                                }

                                //***************
                                // 注文明細を追加
                                //***************
                                // 日配品の登録を主とするので、製造日＝配送日＝喫食日
                                ChumonMeisai chumonMeisai = new ChumonMeisai();
                                chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                chumonMeisai.喫食日 = kissyokubi;
                                chumonMeisai.得意先ＣＤ = tokuisakiCD;
                                chumonMeisai.部署ＣＤ = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                                chumonMeisai.商品ＣＤ = syohinCD;
                                seq++;
                                chumonMeisai.明細番号 = seq;

                                DateTime seizobi = CalcSeizobi(kissyokubi, _busyo);                          // 製造日を算出
                                DateTime? date = AdjustSeizobiHaisobi(seizobi, haisobi, _busyo, tokuisaki);  // 配送日＜製造日の場合は調整
                                if (date != null)
                                {
                                    seizobi = (DateTime)date;
                                    haisobi = (DateTime)date;
                                }
                                chumonMeisai.製造日 = seizobi;
                                chumonMeisai.商品数量 = suryo;

                                // 商品マスタから
                                Syohin syohin = Syohin.GetSyohin(syohinCD);
                                if (syohin != null)
                                {
                                    chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, syohin.商品単価税込, syohin.商品単価税抜);
                                }

                                // 得意先単価マスタから
                                TokuisakiTanka tokuisakiTanka = GetTokuisakiTanka(tokuisakiCD, syohinCD);
                                if (tokuisakiTanka != null)
                                {
                                    chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, tokuisakiTanka.商品単価税込, tokuisakiTanka.商品単価税抜);
                                }

                                // 得意先マスタから
                                chumonMeisai.売掛現金区分 = tokuisaki.売掛現金区分 ?? (int)Consts.UriGenKubun.Genkin;

                                // 主要商品リストマスタから
                                SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)_ikkatsuCourseChumonCriteria.部署ＣＤ, syohinCD);
                                if (syuyoSyohin != null)
                                {
                                    chumonMeisai.商品グループＣＤ = syuyoSyohin.商品グループＣＤ;
                                }

                                // 商品金額や税額を算出
                                chumonMeisai.商品金額合計 = Syohizei.GetAmount(chumonMeisai.商品数量, chumonMeisai.商品単価);
                                chumonMeisai.消費税額 = _syohizei.GetTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                chumonMeisai.商品金額合計税込 = _syohizei.GetAmountIncludeTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                chumonMeisai.計算税率 = _syohizei.GetTaxRate(syohin.軽減税率フラグ);

                                ret = chumonMeisai.Insert(con, tran);
                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                historyProcess = History.ProcessType.Create;
                                History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);
                            }

                            // 注文明細を追加していれば、注文ヘッダを追加or更新する
                            if (seq != 0)
                            {
                                // ****************************************************************
                                // 算出した配送日を設定した注文ヘッダを更新。新規作成していれば追加
                                // ****************************************************************
                                chumonHeader.配送日 = haisobi;

                                //**********************************************************************************************
                                // 得意先が複数のコース(もちろんコース区分は同じ)に跨がる場合、
                                // デフォルトのコース(=複数のコースCDの中で若い方)を採用するのではなく検索条件のコースを採用する
                                // 別のコースの注文を表示している場合(△)は、コースが書き換わることになる
                                //**********************************************************************************************

                                //// 配送日(の曜日)に応じたコースを取得
                                //Course course = GetCourse(tokuisakiCD, haisobi);
                                //if (course == null)
                                //{
                                //    MessageBox.Show(this, string.Format("{0}のコースが見つかりません。注文の登録ができません。", tokuisaki.得意先名), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //    tran.Rollback();
                                //    return false;
                                //}

                                //// 求めたコースが検索条件と異なる場合はエラーとする
                                //if (course.コースＣＤ != _ikkatsuCourseChumonCriteria.コースＣＤ)
                                //{
                                //    MessageBox.Show(this, string.Format("{0}のコースが異なります。注文の登録ができません。", tokuisaki.得意先名), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //    tran.Rollback();
                                //    return false;
                                //}

                                //chumonHeader.コースＣＤ = course.コースＣＤ;

                                //// コースマスタから
                                //chumonHeader.担当者ＣＤ = course.担当者ＣＤ;

                                // 検索条件のコースを設定
                                chumonHeader.コースＣＤ = _ikkatsuCourseChumonCriteria.コースＣＤ;
                                chumonHeader.担当者ＣＤ = _ikkatsuCourseChumonCriteria.担当者ＣＤ;

                                if (chumonHeader.作成日 == null)
                                {
                                    historyProcess = History.ProcessType.Create;
                                    ret = chumonHeader.Insert(con, tran);
                                }
                                else
                                {
                                    // 注文ヘッダ更新フラグに従って更新
                                    if (needChumonHeaderUpdate)
                                    {
                                        historyProcess = History.ProcessType.Update;
                                        ret = chumonHeader.Update(con, tran);
                                    }
                                    else
                                    {
                                        continue;       // 全く変化がないので何もしない
                                    }
                                }

                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                History.Save(con, tran, this.Text, "注文ヘッダ", chumonHeader.Code, historyProcess);
                            }
                            else
                            {
                                //*************************************
                                // 注文明細が存在しない注文ヘッダを削除
                                //*************************************
                                SqlCommand cmdDelH = con.CreateCommand();
                                cmdDelH.CommandText = "\r\n DELETE H" +
                                                      "\r\n FROM 注文ヘッダ AS H" +
                                                      "\r\n LEFT JOIN 注文明細 AS M" +
                                                      "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                                      "\r\n     AND H.注文区分   = M.注文区分" +
                                                      "\r\n     AND H.喫食日     = M.喫食日" +
                                                      "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                                      "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                                      "\r\n WHERE H.企業ＣＤ = @企業ＣＤ" +
                                                      "\r\n AND H.注文区分 = @注文区分注文" +
                                                      "\r\n AND H.喫食日 = @喫食日 "+
                                                      "\r\n AND H.得意先ＣＤ = @得意先ＣＤ" +
                                                      "\r\n AND H.部署ＣＤ = @部署ＣＤ" +
                                                      "\r\n AND M.企業ＣＤ IS NULL";
                                cmdDelH.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                                cmdDelH.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                                cmdDelH.Parameters.Add("@喫食日", SqlDbType.Date).Value = _ikkatsuCourseChumonCriteria.注文日;
                                cmdDelH.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                                cmdDelH.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;
                                cmdDelH.Transaction = tran;
                                log.Debug(cmdDelH.CommandTextWithParameters());
                                cmdDelH.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        historyProcess = History.ProcessType.Update;
                        log.InfoFormat("一括コース注文入力を{0}しました。", historyProcess.DisplayName());
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
        /// 注文明細レコード取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <param name="chumonKubun">注文区分</param>
        /// <returns>注文明細レコードを格納したデータテーブル</returns>
        private DataTable GetChumonMeisai(int tokuisakiCD, Consts.ChumonKubun chumonKubun)
        {
            try
            {
                string tableName = "注文明細予測";
                if (chumonKubun == Consts.ChumonKubun.Chuumon)
                {
                    tableName = "注文明細注文";
                }
                DataTable dt = new DataTable(tableName);

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M.企業ＣＤ" +
                                      "\r\n     ,M.注文区分" +
                                      "\r\n     ,M.喫食日" +
                                      "\r\n     ,M.得意先ＣＤ" +
                                      "\r\n     ,M.商品ＣＤ" +
                                      "\r\n     ,M.部署ＣＤ" +
                                      "\r\n     ,M.明細番号" +
                                      "\r\n     ,M.製造日" +
                                      "\r\n     ,M.商品グループＣＤ" +
                                      "\r\n     ,M.売掛現金区分" +
                                      "\r\n     ,M.商品単価" +
                                      "\r\n     ,M.商品数量" +
                                      "\r\n     ,M.商品金額合計" +
                                      "\r\n     ,M.消費税額" +
                                      "\r\n     ,M.商品金額合計税込" +
                                      "\r\n     ,M.計算税率" +
                                      "\r\n     ,M.作成担当者ＣＤ" +
                                      "\r\n     ,M.作成日" +
                                      "\r\n     ,M.修正担当者ＣＤ" +
                                      "\r\n     ,M.修正日" +
                                      "\r\n FROM 注文明細 AS M" +
                                      "\r\n INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                      "\r\n     AND H.注文区分   = M.注文区分" +
                                      "\r\n     AND H.喫食日     = M.喫食日" +
                                      "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                      "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                      "\r\n WHERE M.企業ＣＤ = @企業ＣＤ " +
                                      "\r\n   AND M.注文区分 = @注文区分 " +
                                      "\r\n   AND M.喫食日 = @喫食日 " +
                                      "\r\n   AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n   AND M.部署ＣＤ = @部署ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)chumonKubun;
                    cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _ikkatsuCourseChumonCriteria.注文日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.InfoFormat("{0}を検索しました。", tableName);
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
        /// 得意先CDに該当するコースCDを返す
        /// </summary>
        /// <param name="tokuisakiCD">コースＣＤ</param>
        /// <param name="targetDate">対象日</param>
        /// <returns>得意先CDに該当するコース</returns>
        private Course GetCourse(int tokuisakiCD, DateTime targetDate)
        {
            try
            {
                Course course = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +               // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n     1 AS 区分" +
                                      "\r\n     ,CM.企業ＣＤ" +
                                      "\r\n     ,CM.コースＣＤ" +
                                      "\r\n     ,CM.コース区分" +
                                      "\r\n     ,CM.コース名" +
                                      "\r\n     ,CM.部署ＣＤ" +
                                      "\r\n     ,CM.担当者ＣＤ" +
                                      "\r\n     ,CM.工場フラグ" +
                                      "\r\n     ,CM.無効フラグ" +
                                      "\r\n     ,CM.作成担当者ＣＤ" +
                                      "\r\n     ,CM.作成日" +
                                      "\r\n     ,CM.修正担当者ＣＤ" +
                                      "\r\n     ,CM.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     コーステーブル AS CT" +
                                      "\r\n     INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             T.コースＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コーステーブル AS T" +
                                      "\r\n             INNER JOIN 臨時コースマスタ AS M ON M.企業ＣＤ = T.企業ＣＤ AND M.コースＣＤ = T.コースＣＤ AND T.実施日 = M.実施日 AND ISNULL(M.無効フラグ, 0) = 0" +
                                      "\r\n         WHERE" +
                                      "\r\n             T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND T.実施日 = @実施日" +
                                      "\r\n             AND T.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n             AND ISNULL(T.無効フラグ, 0) = 0" +
                                      "\r\n             AND M.コース区分 = @コース区分" +
                                      "\r\n         ) AS R" +
                                      "\r\n         ON R.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n     AND CT.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND CM.コース区分 = @コース区分" +
                                      "\r\n     AND CM.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND R.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     0 AS 区分" +
                                      "\r\n     ,RM.企業ＣＤ" +
                                      "\r\n     ,RM.コースＣＤ" +
                                      "\r\n     ,RM.コース区分" +
                                      "\r\n     ,RM.コース名" +
                                      "\r\n     ,RM.部署ＣＤ" +
                                      "\r\n     ,RM.担当者ＣＤ" +
                                      "\r\n     ,RM.工場フラグ" +
                                      "\r\n     ,RM.無効フラグ" +
                                      "\r\n     ,RM.作成担当者ＣＤ" +
                                      "\r\n     ,RM.作成日" +
                                      "\r\n     ,RM.修正担当者ＣＤ" +
                                      "\r\n     ,RM.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     臨時コーステーブル AS RT" +
                                      "\r\n     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n WHERE" +
                                      "\r\n     RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND RT.実施日 = @実施日" +
                                      "\r\n     AND RT.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n     AND RM.コース区分 = @コース区分" +
                                      "\r\n     AND RM.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     区分" +
                                      "\r\n     ,コースＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = targetDate;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuCourseChumonCriteria.部署ＣＤ;

                    // 対象日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun(targetDate);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    if (dt.Rows.Count != 0)
                    {
                        course = new Course(dt.Rows[0]);
                    }
                    return course;
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
        /// 画面クリア
        /// </summary>
        private void Clear()
        {
            try
            {
                this.Text = "コース注文入力";

                // スプレッドをクリア
                bindingSource1.DataSource = null;
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
                btnQuery.PerformClick();
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
        /// 「本日ではありません」ラベルのVisibleを設定
        /// </summary>
        private void ToggleChumonbiIsNotToday()
        {
            lblChumonbiIsNotToday.Visible = (DateTime.Today != _ikkatsuCourseChumonCriteria.注文日);
        }
        #endregion
    }
}

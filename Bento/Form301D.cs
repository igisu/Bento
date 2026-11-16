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
    /// 移動入力詳細
    /// </summary>
    public partial class Form301D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // 移動元残数（左上／右下）
        private const int FROM_SYOHIN_NAME_COLUMN = 0;      // 商品名列インデックス
        private const int FROM_ZANSU_COLUMN       = 1;      // 残数列インデックス

        // 移動先個数（右上／左下）
        private const int TO_SYOHIN_NAME_COLUMN = 0;        // 商品名列インデックス
        private const int TO_KOSU_COLUMN        = 1;        // 個数列インデックス
        private const int TO_TIME_COLUMN        = 2;        // 確認時刻列インデックス
        private const int TO_CHECK_1_COLUMN     = 3;        // 確認フラグ列インデックス
        private const int TO_CHECK_2_COLUMN     = 4;        // 相手確認フラグ列インデックス
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 移動元コース情報
        /// </summary>
        private IdoNyuryokuDetailCriteria _idoNyuryokuDetailCriteriaLeft;

        /// <summary>
        /// 移動先コース情報
        /// </summary>
        private IdoNyuryokuDetailCriteria _idoNyuryokuDetailCriteriaRight;

        /// <summary>
        /// 移動先コース情報スナップショット
        /// </summary>
        private IdoNyuryokuDetailCriteria _courseMoveCriteriaSnapShotRight = null;

        /// <summary>
        /// 左上移動元データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isUpperLeftDtColumnChanging = false;

        /// <summary>
        /// 右上移動先データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isUpperRightDtColumnChanging = false;

        /// <summary>
        /// 右下移動元データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isLowerRightDtColumnChanging = false;

        /// <summary>
        /// 左下移動先データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isLowerLeftDtColumnChanging = false;

        /// <summary>
        /// Spreadにて非編集モードから編集モードへのF2キーが押下されたか
        /// </summary>
        private bool _isF2forSpreadEdit = false;

        /// <summary>
        /// 登録後の再読込の場合はtrue（警告の表示を抑制する）
        /// </summary>
        private bool _isReload = false;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="idoNyuryokuDetailCriteriaLeft"></param>
        public Form301D(IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaLeft)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _idoNyuryokuDetailCriteriaLeft = new IdoNyuryokuDetailCriteria(idoNyuryokuDetailCriteriaLeft);
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
        private void Form301D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                lblGroupLeft.Text = "";
                lblGroupRight.Text = "";

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_UpperRight.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_UpperRight.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_UpperRight.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                fpSpread_LowerLeft.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                im = fpSpread_LowerLeft.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_LowerLeft.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                IdoNyuryokuDetailLeftBindingSource.DataSource = _idoNyuryokuDetailCriteriaLeft;

                // 移動元コース別グループマスタを取得
                GetCourseGroup(lblGroupLeft, (int)_idoNyuryokuDetailCriteriaLeft.コースＣＤ);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _idoNyuryokuDetailCriteriaRight = new IdoNyuryokuDetailCriteria();
                _idoNyuryokuDetailCriteriaRight.コースＣＤ = null;
                _idoNyuryokuDetailCriteriaRight.配送日 = _idoNyuryokuDetailCriteriaLeft.配送日;
                IdoNyuryokuDetailRightBindingSource.DataSource = _idoNyuryokuDetailCriteriaRight;

                // タイトルバーに移動元コース名を表示
                this.Text = string.Format("{0} - 移動入力詳細", _idoNyuryokuDetailCriteriaLeft.コース名);

                // 左上移動元を表示
                GetFrom(_idoNyuryokuDetailCriteriaLeft, _idoNyuryokuDetailCriteriaRight, fpSpread_UpperLeft, new DataColumnChangeEventHandler(dt_UpperLeft_ColumnChanged));
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301D_Shown(object sender, EventArgs e)
        {
            // 移動先コースにフォーカス
            txtCourseCDRight.Focus();
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form301D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_UpperRight.EditMode == true)
                    {
                        fpSpread_UpperRight.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    if (fpSpread_LowerLeft.EditMode == true)
                    {
                        fpSpread_LowerLeft.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }

                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

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
        private void Form301D_FormClosing(object sender, FormClosingEventArgs e)
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
                if (fpSpread_UpperRight.EditMode == true)
                {
                    fpSpread_UpperRight.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                if (fpSpread_LowerLeft.EditMode == true)
                {
                    fpSpread_LowerLeft.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }

                WriteBinding(this.ActiveControl);

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
        /// コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCDRight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCDRight.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCDRight_Click(object sender, EventArgs e)
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
                    errorProvider1.SetError(txtCourseCDRight, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCDRight.Text = course.コースＣＤ.ToString();
                    _idoNyuryokuDetailCriteriaRight.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseNameRight.Text = course.コース名;
                    _idoNyuryokuDetailCriteriaRight.コース名 = course.コース名;
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // タイトルバーに移動元コース名を表示
                // （移動先のエラーに備えて、移動先コース名を消す）
                this.Text = string.Format("{0} - 移動入力詳細", _idoNyuryokuDetailCriteriaLeft.コース名);

                // 検索条件の入力にエラーがある？
                if (_idoNyuryokuDetailCriteriaRight.HasErrors)
                {
                    if (string.IsNullOrEmpty(_idoNyuryokuDetailCriteriaRight.Error) == false)
                    {
                        MessageBox.Show(this, _idoNyuryokuDetailCriteriaRight.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 移動元と同じ？
                if (_idoNyuryokuDetailCriteriaLeft.コースＣＤ == _idoNyuryokuDetailCriteriaRight.コースＣＤ)
                {
                    MessageBox.Show(this, "移動元と同じコースは指定できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (_isReload == false)
                {
                    // 初期状態と異なる？
                    if (HasChange())
                    {
                        if (MessageBox.Show(this, "移動先の変更内容を破棄して再読み込みします。\n\n※移動元も再読み込みします。", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // コース情報を取得・表示
                IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteria = IdoNyuryokuDetailCriteria.GetIdoNyuryokuDetailCriteria((int)_idoNyuryokuDetailCriteriaRight.コースＣＤ, (DateTime)_idoNyuryokuDetailCriteriaRight.配送日);
                _idoNyuryokuDetailCriteriaRight.コース区分 = idoNyuryokuDetailCriteria.コース区分;
                _idoNyuryokuDetailCriteriaRight.コース区分名 = idoNyuryokuDetailCriteria.コース区分名;
                _idoNyuryokuDetailCriteriaRight.支店ＣＤ = idoNyuryokuDetailCriteria.支店ＣＤ;
                _idoNyuryokuDetailCriteriaRight.支店名 = idoNyuryokuDetailCriteria.支店名;
                _idoNyuryokuDetailCriteriaRight.部署ＣＤ = idoNyuryokuDetailCriteria.部署ＣＤ;
                _idoNyuryokuDetailCriteriaRight.部署名 = idoNyuryokuDetailCriteria.部署名;
                IdoNyuryokuDetailRightBindingSource.ResetBindings(false);   // データソースの内容を画面に反映

                // タイトルバーに移動元コース名と移動先コース名を表示
                this.Text = string.Format("{0} ～ {1} - 移動入力詳細", _idoNyuryokuDetailCriteriaLeft.コース名, _idoNyuryokuDetailCriteriaRight.コース名);

                // 移動先コース別グループマスタを取得
                GetCourseGroup(lblGroupRight, (int)_idoNyuryokuDetailCriteriaRight.コースＣＤ);

                // 左上移動元を再表示
                GetFrom(_idoNyuryokuDetailCriteriaLeft, _idoNyuryokuDetailCriteriaRight, fpSpread_UpperLeft, new DataColumnChangeEventHandler(dt_UpperLeft_ColumnChanged));

                // 右上移動先を表示
                GetTo(_idoNyuryokuDetailCriteriaLeft, _idoNyuryokuDetailCriteriaRight, fpSpread_UpperRight, new DataColumnChangeEventHandler(dt_UpperRight_ColumnChanged));

                //************************************************************************************************************************
                // ある商品を移動元のコースＡが11個持っていたとする。
                // 移動先を表示していない場合は、移動元（=コースＡ）の残数には11が表示されている。
                // この商品をコースＸに1個渡そうとした場合、移動元は11-1の10が表示され、移動先には1が表示される。
                // 2個渡そうとすると、移動元は11-2の9が表示され、移動先には2が表示される。
                //
                // さらにコースＹに3個渡そうとした場合、移動元は11-((2)+3)の6が表示され、移動先には3が表示される。
                // 4個渡そうとすると、移動元は11-((2)+4)の5が表示され、移動先には4が表示される。
                // 5個渡そうとすると、移動元は11-((2)+5)の4が表示され、移動先には5が表示される。
                //
                // さらにコースＺに1個渡そうとした場合、移動元は11-(2+5+1)の3が表示され、移動先には1が表示される。
                // 2個渡そうとすると、移動元は11-((2+5)+2)の2が表示され、移動先には2が表示される。
                // 3個渡そうとすると、移動元は11-((2+5)+3)の1が表示され、移動先には3が表示される。
                //
                // つまり、移動元の残数に表示する値は、元の商品数－((移動先以外に渡した個数)＋移動先の個数) とすればよいことがわかる。
                // したがって、移動元を取得するSQLの渡した個数には移動先に渡した個数は含めておらず、データテーブル内で残数を算出している。
                //************************************************************************************************************************
                KosuSet(kosuRightBindingSource, zansuLeftBindingSource);    // 移動先個数を移動元データソースにセットして移動元の残数を算出

                // 同様に下の部分の移動元／先を表示
                // 右下移動元を再表示
                GetFrom(_idoNyuryokuDetailCriteriaRight, _idoNyuryokuDetailCriteriaLeft, fpSpread_LowerRight, new DataColumnChangeEventHandler(dt_LowerRight_ColumnChanged));
                // 左上移動先を表示
                GetTo(_idoNyuryokuDetailCriteriaRight, _idoNyuryokuDetailCriteriaLeft, fpSpread_LowerLeft, new DataColumnChangeEventHandler(dt_LowerLeft_ColumnChanged));
                KosuSet(kosuLeftBindingSource, zansuRightBindingSource);    // 移動先個数を移動元データソースにセットして移動元の残数を算出

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _courseMoveCriteriaSnapShotRight = new IdoNyuryokuDetailCriteria(_idoNyuryokuDetailCriteriaRight);
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
                _isReload = false;
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 上移動先Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UpperRight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
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
        /// 上移動先Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UpperRight_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_UpperRight.EditMode)
                {
                    fpSpread_UpperRight.CancelCellEditing();
                }
                else
                {
                    fpSpread_UpperRight.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Right.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_UpperRight_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_UpperRight_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TO_CHECK_2_COLUMN)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_UpperRight_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_UpperRight_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_UpperRight_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_UpperRight.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_UpperRight_Sheet1.ActiveColumnIndex)
                {
                    case TO_KOSU_COLUMN:           // 個数
                        colIndex = fpSpread_UpperRight_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_UpperRight_Sheet1.Cells[fpSpread_UpperRight_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_Right_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 上移動先Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UpperRight_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_UpperRight.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_UpperRight.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_UpperRight.EditingControl.KeyDown += new KeyEventHandler(this.fpSpreadUpperRightEdit_KeyDown);
            }
        }

        /// <summary>
        /// 上移動先Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UpperRight_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_UpperRight.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_UpperRight, fpSpread_UpperRight_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_UpperRight.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpreadUpperRightEdit_KeyDown);
            }
        }

        /// <summary>
        /// 上移動先Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpreadUpperRightEdit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_UpperRight.EditMode)
                    {
                        fpSpread_UpperRight.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_UpperRight.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Right.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_UpperRight_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_UpperRight_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TO_CHECK_2_COLUMN)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_UpperRight_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_UpperRight_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_UpperRight_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_UpperRight.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 上移動先Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_UpperRight_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)kosuRightBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 右移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeftToRight_Click(object sender, EventArgs e)
        {
            try
            {
                // 移動元から移動先に移動
                MoveTo(zansuLeftBindingSource, kosuRightBindingSource, fpSpread_UpperRight_Sheet1);

                // 移動先個数を移動元データソースにセットして移動元の残数を算出
                KosuSet(kosuRightBindingSource, zansuLeftBindingSource);
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
        /// 下移動先Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_LowerLeft_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
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
        /// 下移動先Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_LowerLeft_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_LowerLeft.EditMode)
                {
                    fpSpread_LowerLeft.CancelCellEditing();
                }
                else
                {
                    fpSpread_LowerLeft.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Right.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_LowerLeft_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_LowerLeft_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TO_CHECK_2_COLUMN)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_LowerLeft_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_LowerLeft_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_LowerLeft_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_LowerLeft.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_LowerLeft_Sheet1.ActiveColumnIndex)
                {
                    case TO_KOSU_COLUMN:           // 個数
                        colIndex = fpSpread_LowerLeft_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_LowerLeft_Sheet1.Cells[fpSpread_LowerLeft_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_Right_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 下移動先Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_LowerLeft_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_LowerLeft.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_LowerLeft.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_LowerLeft.EditingControl.KeyDown += new KeyEventHandler(this.fpSpreadLowerLeftEdit_KeyDown);
            }
        }

        /// <summary>
        /// 下移動先Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_LowerLeft_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_LowerLeft.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_LowerLeft, fpSpread_LowerLeft_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_LowerLeft.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpreadLowerLeftEdit_KeyDown);
            }
        }

        /// <summary>
        /// 下Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpreadLowerLeftEdit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_LowerLeft.EditMode)
                    {
                        fpSpread_LowerLeft.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_LowerLeft.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Right.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_LowerLeft_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_LowerLeft_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= TO_CHECK_2_COLUMN)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_LowerLeft_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_LowerLeft_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_LowerLeft_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_LowerLeft.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 下移動先Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_LowerLeft_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)kosuLeftBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 左移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightToLeft_Click(object sender, EventArgs e)
        {
            try
            {
                // 移動元から移動先に移動
                MoveTo(zansuRightBindingSource, kosuLeftBindingSource, fpSpread_LowerLeft_Sheet1);

                // 移動先個数を移動元データソースにセットして移動元の残数を算出
                KosuSet(kosuLeftBindingSource, zansuRightBindingSource);
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
        /// コース別グループマスタ取得
        /// </summary>
        /// <param name="label">ラベル</param>
        /// <param name="courseCD">コースＣＤ</param>
        private void GetCourseGroup(Label label, int courseCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     CG.グループ種別名" +
                                      "\r\n FROM" +
                                      "\r\n     コースマスタ AS CM" +
                                      "\r\n     LEFT JOIN コース別グループマスタ AS CG" +
                                      "\r\n         ON CG.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n         AND CG.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n         AND CG.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND CM.コースＣＤ = @コースＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     CG.グループ種別ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;

                    log.Debug(cmd.CommandTextWithParameters());

                    string crlf = "";
                    StringBuilder sb = new StringBuilder();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sb.AppendFormat("{0}{1}", crlf, reader["グループ種別名"]);
                            crlf = Environment.NewLine;
                        }
                    }

                    // AutoSizeをTrueにしたLabelをPanelに貼り付けておき、そのLabelに対して文字列を出力する。
                    // PanelにはAutoScrollをTrueにしている。
                    // Panelに文字列を出力するとAutoSizeが効かないのでこうしている。
                    label.Text = sb.ToString();

                    log.Info("コース別グループマスタを検索しました。");
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
        /// 移動元取得
        /// </summary>
        /// <param name="idoNyuryokuDetailCriteriaFrom">移動元コース情報</param>
        /// <param name="idoNyuryokuDetailCriteriaTo">移動先コース情報</param>
        /// <param name="fpSpread">表示対象スプレッド</param>
        /// <param name="dataColumnChangeEventHandler">データテーブル列値変更イベント</param>
        private void GetFrom(IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaFrom, IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaTo,
                             FarPoint.Win.Spread.FpSpread fpSpread, DataColumnChangeEventHandler dataColumnChangeEventHandler)
        {
            DataTable dt;

            try
            {
                FarPoint.Win.Spread.SheetView fpSpread_Sheet1 = fpSpread.Sheets[0];

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     MAIN.コースＣＤ," +
                                      "\r\n     MAIN.コース名," +
                                      "\r\n     MAIN.商品ＣＤ," +
                                      "\r\n     MAIN.商品名略称," +
                                      "\r\n     MCDS.個数 AS 持ち出し数," +
                                      "\r\n     INIT.数量 AS 売上予定," +
                                      "\r\n     TAKE_KOJO.個数 AS 受取数_工場," +
                                      "\r\n     TAKE_IPPAN.個数 AS 受取数_一般," +
                                      "\r\n     GIVE.個数 AS 引渡数," +
                                      // -- 持出の個数＋工場から受け取った個数＋工場以外から受け取った個数－渡した個数－販売数 を残数とする
                                      "\r\n     ISNULL(MCDS.個数, 0) +" +
                                      "\r\n     ISNULL(TAKE_KOJO.個数, 0) +" +
                                      "\r\n     ISNULL(TAKE_IPPAN.個数, 0) -" +
                                      "\r\n     ISNULL(GIVE.個数, 0) -" +
                                      "\r\n     ISNULL(INIT.数量, 0)" +
                                      "\r\n     AS 初期残数," +
                                      "\r\n     0 AS 移動数," +
                                      "\r\n     TARGET.商品ＣＤ AS 移動先商品" +
                                      "\r\n FROM" +
                                      // -- ①駆動表（コースに該当する商品）
                                      "\r\n     (SELECT" +
                                      "\r\n         CM.コースＣＤ," +
                                      "\r\n         CM.コース名," +
                                      "\r\n         P.商品ＣＤ," +
                                      "\r\n         S.商品名略称" +
                                      "\r\n     FROM" +
                                      "\r\n         コースマスタ AS CM" +
                                      "\r\n         INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ" +
                                      "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND CM.コースＣＤ = @移動元コースＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n     ) AS MAIN" +
                                      "\r\n     LEFT JOIN" +
                                      "\r\n     (SELECT" +
                                      "\r\n         PARTSUM.コースＣＤ," +
                                      "\r\n         PARTSUM.部品ＣＤ" +
                                      "\r\n         ,SUM(PARTSUM.数量) AS 数量" +
                                      "\r\n     FROM" +
                                      // -- ②予測と注文と売上の部品（各商品を分割した部品ごとに合計する）
                                      "\r\n         (SELECT" +
                                      "\r\n             PART.コースＣＤ," +
                                      "\r\n             PART.得意先ＣＤ," +
                                      "\r\n             PART.部品ＣＤ," +
                                      "\r\n             CASE WHEN ISNULL(SUM(PART.商品売上数量), -1) >= 0 THEN SUM(PART.商品売上数量)" +
                                      "\r\n                  ELSE ISNULL(SUM(PART.商品注文予測数量), 0) END" +
                                      "\r\n             AS 数量" +
                                      "\r\n         FROM" +
                                      // -- ②－１ 予測と注文の弁当の主食
                                      "\r\n             (SELECT" +
                                      "\r\n                 PART1.コースＣＤ," +
                                      "\r\n                 PART1.得意先ＣＤ," +
                                      "\r\n                 PART1.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> 0" +               // -- 主食がある
                                      "\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.主食ＣＤ" +
                                      "\r\n                 ) AS PART1" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART1.コースＣＤ" +
                                      "\r\n                 ,PART1.得意先ＣＤ" +
                                      "\r\n                 ,PART1.部品ＣＤ" +
                                      "\r\n             UNION ALL" +    // -- SUM()の対象にするので、同一コース、同一得意先、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                                      // -- ②－２ 予測と注文の弁当の副食
                                      "\r\n             SELECT" +
                                      "\r\n                 PART2.コースＣＤ," +
                                      "\r\n                 PART2.得意先ＣＤ," +
                                      "\r\n                 PART2.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> 0" +               // -- 副食がある
                                      "\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.副食ＣＤ" +
                                      "\r\n                 ) AS PART2" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART2.コースＣＤ" +
                                      "\r\n                 ,PART2.得意先ＣＤ" +
                                      "\r\n                 ,PART2.部品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－３ 予測と注文の主食のみ
                                      "\r\n             SELECT" +
                                      "\r\n                 PART3.コースＣＤ," +
                                      "\r\n                 PART3.得意先ＣＤ," +
                                      "\r\n                 PART3.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分主食" +    // -- 弁当区分主食
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.主食ＣＤ" +
                                      "\r\n                 ) AS PART3" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART3.コースＣＤ" +
                                      "\r\n                 ,PART3.得意先ＣＤ" +
                                      "\r\n                 ,PART3.部品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－４ 予測と注文の副食のみ
                                      "\r\n             SELECT" +
                                      "\r\n                 PART4.コースＣＤ," +
                                      "\r\n                 PART4.得意先ＣＤ," +
                                      "\r\n                 PART4.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分副食" +    // -- 弁当区分副食
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.副食ＣＤ" +
                                      "\r\n                 ) AS PART4" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART4.コースＣＤ" +
                                      "\r\n                 ,PART4.得意先ＣＤ" +
                                      "\r\n                 ,PART4.部品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－５ 予測と注文の主食でも副食でもないもの
                                      "\r\n             SELECT" +
                                      "\r\n                 PART5.コースＣＤ," +
                                      "\r\n                 PART5.得意先ＣＤ," +
                                      "\r\n                 PART5.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART5.商品注文数量, PART5.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.商品ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.主食ＣＤ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.副食ＣＤ, 0) = 0" +
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.商品ＣＤ" +
                                      "\r\n                 ) AS PART5" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART5.コースＣＤ" +
                                      "\r\n                 ,PART5.得意先ＣＤ" +
                                      "\r\n                 ,PART5.部品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－６ 予測と注文のその他の主食
                                      "\r\n             SELECT" +
                                      "\r\n                 PART6.コースＣＤ," +
                                      "\r\n                 PART6.得意先ＣＤ," +
                                      "\r\n                 PART6.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART6.商品注文数量, PART6.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分その他" +  // -- その他
                                      "\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> 0" +               // -- 主食がある
                                      "\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.主食ＣＤ" +
                                      "\r\n                 ) AS PART6" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART6.コースＣＤ" +
                                      "\r\n                 ,PART6.得意先ＣＤ" +
                                      "\r\n                 ,PART6.部品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－７ 予測と注文のその他の副食
                                      "\r\n             SELECT" +
                                      "\r\n                 PART7.コースＣＤ," +
                                      "\r\n                 PART7.得意先ＣＤ," +
                                      "\r\n                 PART7.部品ＣＤ," +
                                      "\r\n                 SUM(ISNULL(PART7.商品注文数量, PART7.商品予測数量)) AS 商品注文予測数量," +
                                      "\r\n                 null AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 (SELECT" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量," +
                                      "\r\n                     SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     コースマスタ AS CM" +
                                      "\r\n                     INNER JOIN 注文ヘッダ AS H" +
                                      "\r\n                         ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND H.配送日     = @移動日" +
                                      "\r\n                         AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                     INNER JOIN 注文明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                         AND D.注文区分   = H.注文区分" +
                                      "\r\n                         AND D.喫食日     = H.喫食日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                     LEFT JOIN" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                            H.企業ＣＤ," +
                                      "\r\n                            H.売上日," +
                                      "\r\n                            H.得意先ＣＤ," +
                                      "\r\n                            H.部署ＣＤ," +
                                      "\r\n                            H.コースＣＤ," +
                                      "\r\n                            D.商品ＣＤ," +
                                      "\r\n                            D.明細番号" +
                                      "\r\n                         FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                             (SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                 INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                     T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                     AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                     AND T1.売上日     = T2.売上日" +
                                      "\r\n                                     AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                             UNION" +
                                      "\r\n                             SELECT" +
                                      "\r\n                                 T1.企業ＣＤ," +
                                      "\r\n                                 T1.売上日," +
                                      "\r\n                                 T1.得意先ＣＤ," +
                                      "\r\n                                 T1.部署ＣＤ," +
                                      "\r\n                                 T1.コースＣＤ" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 売上ヘッダ T1" +
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                 AND T1.売上日   = @移動日" +
                                      "\r\n                                 AND NOT EXISTS (" +
                                      "\r\n                                     SELECT 'X'" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         (SELECT" +
                                      "\r\n                                             企業ＣＤ," +
                                      "\r\n                                             部署ＣＤ," +
                                      "\r\n                                             コースＣＤ," +
                                      "\r\n                                             得意先ＣＤ," +
                                      "\r\n                                             喫食日 AS 売上日" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             注文ヘッダ" +
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                             AND 配送日   = @移動日" +
                                      "\r\n                                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                             ) AS H" +
                                      "\r\n                             INNER JOIN 売上明細 AS D" +
                                      "\r\n                                 ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                 AND D.売上日     = H.売上日" +
                                      "\r\n                                 AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                 AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                         ) AS KHD" +
                                      "\r\n                         ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                         AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                         AND D.喫食日 = KHD.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                         AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                     AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分その他" +  // -- その他
                                      "\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> 0" +               // -- 副食がある
                                      "\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                     AND KHD.明細番号 IS NULL" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     CM.コースＣＤ," +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     I.副食ＣＤ" +
                                      "\r\n                 ) AS PART7" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART7.コースＣＤ" +
                                      "\r\n                 ,PART7.得意先ＣＤ" +
                                      "\r\n                 ,PART7.部品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－８ 売上の弁当の主食
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 INNER JOIN 商品マスタ AS I" +
                                      "\r\n                     ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                     AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                 AND ISNULL(D.主食ＣＤ, 0) <> 0" +               // -- 主食がある
                                      "\r\n                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ" +      // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.主食ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－９ 売上の弁当の副食
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 INNER JOIN 商品マスタ AS I" +
                                      "\r\n                     ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                     AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                 AND ISNULL(D.副食ＣＤ, 0) <> 0" +               // -- 副食がある
                                      "\r\n                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ" +      // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.副食ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－１０ 売上の主食のみ
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 INNER JOIN 商品マスタ AS I" +
                                      "\r\n                     ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                     AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.主食ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－１１ 売上の副食のみ
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 INNER JOIN 商品マスタ AS I" +
                                      "\r\n                     ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                     AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.副食ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－１２ 売上の主食でも副食でもないもの
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.商品ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(D.主食ＣＤ, 0) = 0" +
                                      "\r\n                 AND ISNULL(D.副食ＣＤ, 0) = 0" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.商品ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－１３ 売上のその他の主食
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.主食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 INNER JOIN 商品マスタ AS I" +
                                      "\r\n                     ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                     AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他" +  // -- その他
                                      "\r\n                 AND ISNULL(D.主食ＣＤ, 0) <> 0" +               // -- 主食がある
                                      "\r\n                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ" +      // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.主食ＣＤ" +
                                      "\r\n             UNION ALL" +
                                      // -- ②－１４ 売上のその他の副食
                                      "\r\n             SELECT" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.副食ＣＤ AS 部品ＣＤ," +
                                      "\r\n                 null AS 商品注文予測数量," +
                                      "\r\n                 SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n             FROM" +
                                      "\r\n                 コースマスタ AS CM" +
                                      "\r\n                 INNER JOIN" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         (SELECT" +
                                      "\r\n                             企業ＣＤ," +
                                      "\r\n                             部署ＣＤ," +
                                      "\r\n                             コースＣＤ," +
                                      "\r\n                             得意先ＣＤ," +
                                      "\r\n                             喫食日 AS 売上日" +
                                      "\r\n                         FROM" +
                                      "\r\n                             注文ヘッダ" +
                                      "\r\n                         WHERE" +
                                      "\r\n                             企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                             AND 配送日   = @移動日" +
                                      "\r\n                         ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                         INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                             AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                     UNION" +
                                      "\r\n                     SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ," +
                                      "\r\n                         T1.コースＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @移動日" +
                                      "\r\n                         AND NOT EXISTS (" +
                                      "\r\n                             SELECT 'X'" +
                                      "\r\n                             FROM" +
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     企業ＣＤ," +
                                      "\r\n                                     部署ＣＤ," +
                                      "\r\n                                     コースＣＤ," +
                                      "\r\n                                     得意先ＣＤ," +
                                      "\r\n                                     喫食日 AS 売上日" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     注文ヘッダ" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND 配送日   = @移動日" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     ON H.企業ＣＤ    = CM.企業ＣＤ" +
                                      "\r\n                     AND H.売上日     = @移動日" +
                                      "\r\n                     AND H.部署ＣＤ   = CM.部署ＣＤ" +
                                      "\r\n                     AND H.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                     AND D.売上日     = H.売上日" +
                                      "\r\n                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 INNER JOIN 商品マスタ AS I" +
                                      "\r\n                     ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                     AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                     AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n             WHERE" +
                                      "\r\n                 CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他" +  // -- その他
                                      "\r\n                 AND ISNULL(D.副食ＣＤ, 0) <> 0" +               // -- 副食がある
                                      "\r\n                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ" +      // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n             GROUP BY" +
                                      "\r\n                 CM.コースＣＤ," +
                                      "\r\n                 H.得意先ＣＤ," +
                                      "\r\n                 D.副食ＣＤ" +
                                      "\r\n             ) AS PART" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             PART.コースＣＤ," +
                                      "\r\n             PART.得意先ＣＤ," +
                                      "\r\n             PART.部品ＣＤ" +
                                      "\r\n         ) AS PARTSUM" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         PARTSUM.コースＣＤ," +
                                      "\r\n         PARTSUM.部品ＣＤ" +
                                      "\r\n     ) AS INIT ON MAIN.コースＣＤ = INIT.コースＣＤ AND MAIN.商品ＣＤ = INIT.部品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ③持ち出し数
                                      "\r\n     (SELECT" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n         ,SUM(個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_持ち出し入力" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n         持出日 = @移動日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n     ) AS MCDS ON MAIN.コースＣＤ = MCDS.コースＣＤ AND MAIN.商品ＣＤ = MCDS.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ④工場からもらった個数
                                      "\r\n     (SELECT" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n         ,SUM(MI.個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_移動入力 AS MI" +
                                      "\r\n         LEFT JOIN コースマスタ AS CM" +
                                      "\r\n         ON MI.企業ＣＤ = CM.企業ＣＤ" +
                                      "\r\n         AND MI.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(CM.工場フラグ, 0) <> 0" +
                                      "\r\n         AND MI.移動日 = @移動日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n     ) AS TAKE_KOJO ON MAIN.コースＣＤ = TAKE_KOJO.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_KOJO.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ⑤工場以外からもらった個数
                                      "\r\n     (SELECT" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n         ,SUM(MI.個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_移動入力 AS MI" +
                                      "\r\n         LEFT JOIN コースマスタ AS CM" +
                                      "\r\n         ON MI.企業ＣＤ = CM.企業ＣＤ" +
                                      "\r\n         AND MI.コースＣＤ = CM.コースＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(CM.工場フラグ, 0) = 0" +
                                      "\r\n         AND MI.移動日 = @移動日" +
                                      "\r\n     GROUP BY" +
                                      "\r\n         MI.相手コースＣＤ" +
                                      "\r\n         ,MI.商品ＣＤ" +
                                      "\r\n     ) AS TAKE_IPPAN ON MAIN.コースＣＤ = TAKE_IPPAN.相手コースＣＤ AND MAIN.商品ＣＤ = TAKE_IPPAN.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ⑥渡した個数
                                      "\r\n     (SELECT" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n         ,SUM(個数) AS 個数" +
                                      "\r\n     FROM" +
                                      "\r\n         モバイル_移動入力" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 移動日 = @移動日" +
                                      "\r\n         AND 相手コースＣＤ <> @移動先コースＣＤ" +  // -- ！！！移動先を表示した場合は、移動先のコースに渡した個数は含めない！！！
                                      "\r\n     GROUP BY" +
                                      "\r\n         コースＣＤ" +
                                      "\r\n         ,商品ＣＤ" +
                                      "\r\n     ) AS GIVE ON MAIN.コースＣＤ = GIVE.コースＣＤ AND MAIN.商品ＣＤ = GIVE.商品ＣＤ" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ⑦移動先コースの商品
                                      "\r\n     (SELECT" +
                                      "\r\n         P.商品ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         コースマスタ AS CM" +
                                      "\r\n         INNER JOIN 主要部品リストマスタ AS P ON P.企業ＣＤ = CM.企業ＣＤ AND P.部署ＣＤ = CM.部署ＣＤ" +
                                      "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n     WHERE" +
                                      "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND CM.コースＣＤ = @移動元先コースＣＤ" +  // -- 移動先を表示した場合は移動先のコースＣＤ、移動先未表示の場合は移動元のコースＣＤとする
                                      "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n         AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n     ) AS TARGET ON MAIN.商品ＣＤ = TARGET.商品ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@移動元コースＣＤ", SqlDbType.Int).Value = idoNyuryokuDetailCriteriaFrom.コースＣＤ;
                    cmd.Parameters.Add("@移動先コースＣＤ", SqlDbType.Int).Value = idoNyuryokuDetailCriteriaTo.コースＣＤ ?? -1;
                    cmd.Parameters.Add("@移動元先コースＣＤ", SqlDbType.Int).Value = idoNyuryokuDetailCriteriaTo.コースＣＤ ?? idoNyuryokuDetailCriteriaFrom.コースＣＤ;
                    cmd.Parameters.Add("@移動日", SqlDbType.Date).Value = idoNyuryokuDetailCriteriaFrom.配送日;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("移動元");
                    adp.Fill(dt);

                    // 初期残数－移動数を格納する列を追加する(★)
                    dt.Columns.Add("残数", typeof(int), "初期残数-移動数");

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += dataColumnChangeEventHandler;

                    BindingSource bindingSource = (BindingSource)fpSpread_Sheet1.DataSource;

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = "移動元";

                    log.Info("移動元を検索しました。");
                }

                fpSpread_Sheet1.BindDataColumn(FROM_SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread_Sheet1.BindDataColumn(FROM_ZANSU_COLUMN,       "残数");

                // 背景色をリセット
                int rowIndex;
                for (rowIndex = 0; rowIndex < fpSpread_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread_Sheet1.Rows[rowIndex].ResetForeColor();
                }

                // 移動先にも同じ商品があれば（＝移動先商品がNullでなければ）移動可能
                // それ以外は移動不可能として背景色を変える
                rowIndex = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.IsNull("移動先商品"))
                    {
                        fpSpread_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
                    }
                    rowIndex++;
                }

                fpSpread.Focus();
                if (fpSpread_Sheet1.Rows.Count != 0)
                {
                    fpSpread_Sheet1.ActiveRowIndex = 0;
                    fpSpread_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
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
        /// 左上移動元データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_UpperLeft_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isUpperLeftDtColumnChanging)
            {
                return;
            }

            try
            {
                _isUpperLeftDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
                _isUpperLeftDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 右下移動元データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_LowerRight_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isLowerRightDtColumnChanging)
            {
                return;
            }

            try
            {
                _isLowerRightDtColumnChanging = true;

                // 特に何もしない
                // 将来必要になったときのためにメソッドだけ用意している
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
                _isLowerRightDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 移動先取得
        /// </summary>
        /// <param name="idoNyuryokuDetailCriteriaFrom">移動元コース情報</param>
        /// <param name="idoNyuryokuDetailCriteriaTo">移動先コース情報</param>
        /// <param name="fpSpread">表示対象スプレッド</param>
        /// <param name="dataColumnChangeEventHandler">データテーブル列値変更イベント</param>
        private void GetTo(IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaFrom, IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaTo,
                           FarPoint.Win.Spread.FpSpread fpSpread, DataColumnChangeEventHandler dataColumnChangeEventHandler)
        {
            DataTable dt;

            try
            {
                FarPoint.Win.Spread.SheetView fpSpread_Sheet1 = fpSpread.Sheets[0];

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     MI.企業ＣＤ" +
                                      "\r\n     ,MI.コースＣＤ" +
                                      "\r\n     ,MI.相手コースＣＤ" +
                                      "\r\n     ,MI.商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,MI.個数" +
                                      "\r\n     ,MI.移動日" +
                                      "\r\n     ,MI.ＳＥＱ" +
                                      "\r\n     ,MI.確認フラグ" +
                                      "\r\n     ,MI.確認日時" +
                                      "\r\n     ,MI.相手確認フラグ" +
                                      "\r\n     ,MI.相手確認日時" +
                                      "\r\n     ,CASE WHEN ISNULL(MI.相手確認日時, '2000/01/01') >= ISNULL(MI.確認日時, '2000/01/01') THEN MI.相手確認日時 ELSE MI.確認日時 END AS 表示用確認日時" +
                                      "\r\n     ,MI.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,MI.作成日" +
                                      "\r\n     ,MI.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,MI.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     モバイル_移動入力 AS MI" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = MI.企業ＣＤ AND S.商品ＣＤ = MI.商品ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = MI.企業ＣＤ AND MI.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = MI.企業ＣＤ AND MI.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     MI.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND MI.コースＣＤ = @移動元コースＣＤ" +
                                      "\r\n     AND MI.相手コースＣＤ = @移動先コースＣＤ" +
                                      "\r\n     AND MI.移動日 = @移動日" +
                                      "\r\n ORDER BY" +
                                      "\r\n     MI.ＳＥＱ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@移動元コースＣＤ", SqlDbType.Int).Value = idoNyuryokuDetailCriteriaFrom.コースＣＤ;
                    cmd.Parameters.Add("@移動先コースＣＤ", SqlDbType.Int).Value = idoNyuryokuDetailCriteriaTo.コースＣＤ;
                    cmd.Parameters.Add("@移動日", SqlDbType.Date).Value = idoNyuryokuDetailCriteriaTo.配送日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("移動先");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += dataColumnChangeEventHandler;

                    BindingSource bindingSource = (BindingSource)fpSpread_Sheet1.DataSource;

                    bindingSource.DataSource = ds;
                    bindingSource.DataMember = "移動先";

                    log.Info("移動元を検索しました。");
                }

                fpSpread_Sheet1.BindDataColumn(TO_SYOHIN_NAME_COLUMN, "商品名略称");
                fpSpread_Sheet1.BindDataColumn(TO_KOSU_COLUMN,        "個数");
                fpSpread_Sheet1.BindDataColumn(TO_TIME_COLUMN,        "表示用確認日時");
                fpSpread_Sheet1.BindDataColumn(TO_CHECK_1_COLUMN,     "確認フラグ");
                fpSpread_Sheet1.BindDataColumn(TO_CHECK_2_COLUMN,     "相手確認フラグ");

                // 確認済みの場合、背景色を変える
                int rowIndex = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    fpSpread_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread_Sheet1.Rows[rowIndex].ResetForeColor();
                    FarPoint.Win.Spread.CellType.NumberCellType kosu = (FarPoint.Win.Spread.CellType.NumberCellType)fpSpread_Sheet1.GetCellType(rowIndex, TO_KOSU_COLUMN);
                    kosu.ReadOnly = false;
                    kosu.Static = false;
                    fpSpread_Sheet1.Cells[rowIndex, TO_KOSU_COLUMN].CanFocus = true;

                    //*******************************************************************************************************************
                    // 自分または相手が商品の受け渡し（移動）を確認した（チェックした）あとは、その個数の変更はできないようにしていたが、
                    // やはりチェックしたあとでも個数の変更ができるようにしてほしいとの要望があった
                    //*******************************************************************************************************************
                    // if (dr.IsNull("表示用確認日時") == false)
                    // {
                    //     kosu.ReadOnly = true;
                    //     kosu.Static = true;
                    //     fpSpread_Sheet1.Cells[rowIndex, TO_KOSU_COLUMN].CanFocus = false;
                    //     fpSpread_Sheet1.Rows[rowIndex].BackColor = Color.LightYellow;
                    //     fpSpread_Sheet1.Rows[rowIndex].ForeColor = Color.Olive;
                    // }
                    rowIndex++;
                }

                if (fpSpread_Sheet1.Rows.Count != 0)
                {
                    fpSpread_Sheet1.ActiveRowIndex = 0;
                    fpSpread_Sheet1.ActiveColumnIndex = TO_KOSU_COLUMN;
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
        /// 右上移動先データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_UpperRight_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isUpperRightDtColumnChanging)
            {
                return;
            }

            try
            {
                _isUpperRightDtColumnChanging = true;

                // 個数の場合は移動元の残数を再計算
                if (e.Column.ColumnName == "個数")
                {
                    e.Row.SetColumnError(e.Column, null);
                    if (KosuSet(kosuRightBindingSource, zansuLeftBindingSource, (int)e.Row["商品ＣＤ"]) == false)
                    {
                        // 残数がマイナスになる場合はエラー
                        // …と思っていたが、(◆)に記載の通りエラーにはしない
                        //e.Row.SetColumnError(e.Column, "移動した個数が残数を超えています。");
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
                _isUpperRightDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 左下移動先データソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_LowerLeft_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isLowerLeftDtColumnChanging)
            {
                return;
            }

            try
            {
                _isLowerLeftDtColumnChanging = true;

                // 個数の場合は移動元の残数を再計算
                if (e.Column.ColumnName == "個数")
                {
                    e.Row.SetColumnError(e.Column, null);
                    if (KosuSet(kosuLeftBindingSource, zansuRightBindingSource, (int)e.Row["商品ＣＤ"]) == false)
                    {
                        // 残数がマイナスになる場合はエラー
                        // …と思っていたが、(◆)に記載の通りエラーにはしない
                        //e.Row.SetColumnError(e.Column, "移動した個数が残数を超えています。");
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
                _isLowerLeftDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 移動先個数を移動元データソースにセットして移動元の残数を算出します
        /// </summary>
        /// <param name="kosuBindingSource">移動先個数BindingSource</param>
        /// <param name="zansuBindingSource">移動元残数BindingSource</param>
        private void KosuSet(BindingSource kosuBindingSource, BindingSource zansuBindingSource)
        {
            try
            {
                // 移動先未検索？
                DataSet ds = (DataSet)kosuBindingSource.DataSource;
                if (ds == null)
                {
                    return;
                }

                DataTable dt = ds.Tables["移動先"];
                foreach (DataRow dr in dt.Rows)
                {
                    int syohinCD = (int)dr["商品ＣＤ"];
                    KosuSet(kosuBindingSource, zansuBindingSource, syohinCD);
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
        /// 移動先個数を移動元スプレッドにセットして移動元の残数を算出します
        /// </summary>
        /// <param name="syohinCD">商品ＣＤ</param>
        /// <returns>移動元の残数がマイナスになる場合はfalse</returns>
        /// <param name="kosuBindingSource">移動先個数BindingSource</param>
        /// <param name="zansuBindingSource">移動元残数BindingSource</param>
        private bool KosuSet(BindingSource kosuBindingSource, BindingSource zansuBindingSource, int syohinCD)
        {
            bool ret = true;

            try
            {
                // 移動先に対して、指定された商品ＣＤに該当する個数の合計を取得
                DataSet dsRight = (DataSet)kosuBindingSource.DataSource;
                DataTable dtRight = dsRight.Tables["移動先"];
                object value = dtRight.Compute("SUM(個数)", string.Format("商品ＣＤ={0}", syohinCD));

                // 移動元に対して、指定された商品ＣＤに該当するレコードを取得して移動数を反映
                DataSet dsLeft = (DataSet)zansuBindingSource.DataSource;
                DataTable dtLeft = dsLeft.Tables["移動元"];
                DataRow[] drsLeft = dtLeft.Select(string.Format("商品ＣＤ={0}", syohinCD));

                if (drsLeft != null && drsLeft.Length > 0)
                {
                    drsLeft[0]["移動数"] = value;   // 残数は(★)で算出済み
                    if (drsLeft[0].IsNull("残数") == false)
                    {
                        if (((int)drsLeft[0]["残数"]) < 0)
                        {
                            ret = false;
                        }
                    }
                }
                return ret;
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
        /// 移動元から移動先への移動
        /// </summary>
        /// <param name="zansuBindingSource">移動元残数BindingSource</param>
        /// <param name="kosuBindingSource">移動先個数BindingSource</param>
        /// <param name="kosuSheet">移動先シート</param>
        private void MoveTo(BindingSource zansuBindingSource, BindingSource kosuBindingSource, FarPoint.Win.Spread.SheetView kosuSheet)
        {
            int rowIndex;
            try
            {
                // 移動元未選択？
                if (zansuBindingSource.Position < 0)
                {
                    return;
                }

                // 移動先未検索？
                DataSet ds = (DataSet)kosuBindingSource.DataSource;
                if (ds == null)
                {
                    return;
                }

                // 移動元行を取得
                DataRow drCurrent = ((DataRowView)zansuBindingSource.Current).Row;

                // 移動先商品がNullでなければ移動可能
                if (drCurrent.IsNull("移動先商品"))
                {
                    MessageBox.Show(this, "移動先のコースで取り扱っていない商品なので移動できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 残数が0の商品は移動不可
                // …と思っていたが、ルートＢの商品の不足が予想されていて工場からもらう際、工場→ルートＢだと間に合わない場合、
                // 付近のルートＡからルートＢに渡し、ルートＡは工場からもらうことがある。
                // ルートＡがルートＢに渡す数によっては、ルートＡの予想売上数（注文数）を超えて渡すことがある。このとき、ルートＡの残数は一時的にマイナスになり得る。(◆)
                //if (((int)drCurrent["残数"]) <= 0)
                //{
                //    MessageBox.Show(this, "商品が残っていないので移動できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    return;
                //}

                // 移動先に同じ商品があり、確認済みでなければ、それを有効にする
                int syohinCD = (int)drCurrent["商品ＣＤ"];
                DataRow drTarget;
                kosuBindingSource.MoveFirst();
                for (rowIndex = 0; rowIndex < kosuBindingSource.Count; rowIndex++)
                {
                    drTarget = ((DataRowView)kosuBindingSource.Current).Row;
                    //*******************************************************************************************************************
                    // 自分または相手が商品の受け渡し（移動）を確認した（チェックした）あとは、その個数の変更はできないようにしていたが、
                    // やはりチェックしたあとでも個数の変更ができるようにしてほしいとの要望があった
                    //*******************************************************************************************************************
                    // if (((int)drTarget["商品ＣＤ"]) == syohinCD && drTarget.IsNull("表示用確認日時"))
                    if (((int)drTarget["商品ＣＤ"]) == syohinCD)
                    {
                        return;
                    }
                    kosuBindingSource.MoveNext();
                }

                // 移動先の新しい行を生成
                DataTable dt = ds.Tables["移動先"];
                drTarget = dt.NewRow();

                // 移動元を移動先にセット
                drTarget["商品ＣＤ"] = drCurrent["商品ＣＤ"];
                drTarget["商品名略称"] = drCurrent["商品名略称"];
                drTarget["個数"] = 1;
                drTarget["表示用確認日時"] = DBNull.Value;

                dt.Rows.Add(drTarget);  // 移動先を追加

                // 新規行をアクティブ
                rowIndex = kosuSheet.RowCount - 1;
                FarPoint.Win.Spread.CellType.NumberCellType kosu = (FarPoint.Win.Spread.CellType.NumberCellType)kosuSheet.GetCellType(rowIndex, TO_KOSU_COLUMN);
                kosu.ReadOnly = false;
                kosu.Static = false;
                kosuSheet.Cells[rowIndex, TO_KOSU_COLUMN].CanFocus = true;
                kosuSheet.ActiveRowIndex = rowIndex;
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
        /// コーステーブルが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 右上移動先のテーブルを比較
            if (HasChange(kosuRightBindingSource) == true)
            {
                return true;
            }

            // 左下移動先のテーブルを比較
            return HasChange(kosuLeftBindingSource);
        }

        /// <summary>
        /// コーステーブルが変更されているかを返します。
        /// </summary>
        /// <param name="kosuBindingSource">移動先個数BindingSource</param>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange(BindingSource kosuBindingSource)
        {
            // 移動先のテーブルを比較
            DataSet ds = (DataSet)kosuBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables[0];

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

                // 検索されていない？
                if (_courseMoveCriteriaSnapShotRight == null)
                {
                    MessageBox.Show(this, "移動先が表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_idoNyuryokuDetailCriteriaRight.Equals(_courseMoveCriteriaSnapShotRight) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 右上移動先のエラーチェック
                hasError = HasError(kosuRightBindingSource);
                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 左下移動先のエラーチェック
                hasError = HasError(kosuLeftBindingSource);
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
        /// エラーがあるかを返します
        /// </summary>
        /// <param name="kosuBindingSource">移動先個数BindingSource</param>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasError(BindingSource kosuBindingSource)
        {
            bool hasError = false;

            DataSet ds = (DataSet)kosuBindingSource.DataSource;
            DataTable dt = ds.Tables["移動先"];

            // 行数分ループ
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.HasErrors)
                {
                    hasError = true;
                }
                else
                {
                    MobileIdo mobileIdo = new MobileIdo(dr);
                    // エラーがある？
                    if (mobileIdo.HasErrors)
                    {
                        dr.SetColumnError("個数", mobileIdo["個数"]);
                        hasError = true;
                    }
                }
            }

            return hasError;
        }

        /// <summary>
        /// DB保存
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Save()
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        // 右上移動先を保存
                        if (SaveMain(con, tran, kosuRightBindingSource, _idoNyuryokuDetailCriteriaLeft, _idoNyuryokuDetailCriteriaRight) == false)
                        {
                            return false;
                        }

                        // 左下移動先を保存
                        if (SaveMain(con, tran, kosuLeftBindingSource, _idoNyuryokuDetailCriteriaRight, _idoNyuryokuDetailCriteriaLeft) == false)
                        {
                            return false;
                        }

                        historyProcess = History.ProcessType.Update;

                        // コミット
                        tran.Commit();
                        log.InfoFormat("モバイル_移動入力を{0}しました。", historyProcess.DisplayName());
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
        /// DB保存主処理
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="kosuBindingSource">移動先個数BindingSource</param>
        /// <param name="idoNyuryokuDetailCriteriaFrom">移動元コース情報</param>
        /// <param name="idoNyuryokuDetailCriteriaTo">移動先コース情報</param>
        /// <returns>正常終了の場合はtrue</returns>
        private bool SaveMain(SqlConnection con, SqlTransaction tran, BindingSource kosuBindingSource,
                              IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaFrom, IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteriaTo)
        {
            bool ret = false;
            int seq = 1;
            History.ProcessType historyProcess = History.ProcessType.Update;

            DataSet ds = (DataSet)kosuBindingSource.DataSource;
            DataTable dt = ds.Tables["移動先"];

            foreach (DataRow dr in dt.Rows)
            {
                MobileIdo mobileIdo = new MobileIdo(dr);

                switch (dr.RowState)
                {
                    case DataRowState.Added:        // 追加
                        if (mobileIdo.個数 == null || mobileIdo.個数 == 0)
                        {
                            continue;
                        }
                        mobileIdo.コースＣＤ = idoNyuryokuDetailCriteriaFrom.コースＣＤ;
                        mobileIdo.相手コースＣＤ = idoNyuryokuDetailCriteriaTo.コースＣＤ;
                        mobileIdo.移動日 = idoNyuryokuDetailCriteriaTo.配送日;
                        mobileIdo.ＳＥＱ = seq;
                        ret = mobileIdo.Insert(con, tran);
                        historyProcess = History.ProcessType.Create;
                        seq++;
                        break;

                    case DataRowState.Modified:     // 変更
                    case DataRowState.Unchanged:    // 変更なし→ＳＥＱのみ更新
                        // 個数が0の場合は削除する
                        if (mobileIdo.個数 == null || mobileIdo.個数 == 0)
                        {
                            ret = mobileIdo.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                        }
                        else
                        {
                            mobileIdo.ＳＥＱ = seq;
                            ret = mobileIdo.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            seq++;
                        }
                        break;

                    default:
                        ret = true;
                        break;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, mobileIdo.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                History.Save(con, tran, this.Text, "モバイル_移動入力", mobileIdo.Code, historyProcess);
            }

            return true;
        }

        /// <summary>
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                _isReload = true;
                btnQuery.PerformClick();        // 検索ボタン押下
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
        #endregion
    }
}

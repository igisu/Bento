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
    /// 一括注文入力
    /// </summary>
    public partial class Form202 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMONBI_COLUMN = 0;      // 注文日列インデックス
        private const int HOLIDAY_COLUMN  = 1;      // 祝日名列インデックス
        private const int BIKO_COLUMN     = 2;      // 備考列インデックス
        // これ以降の列は動的に追加される
        private const int SYOHIN_START_COLUMN = 3;  // 商品名列開始列インデックス

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
        /// 一括注文入力検索条件
        /// </summary>
        private IkkatsuChumonCriteria _ikkatsuChumonCriteria;

        /// <summary>
        /// 一括注文入力検索条件検索実行時スナップショット
        /// </summary>
        private IkkatsuChumonCriteria _ikkatsuChumonCriteriaSnapShot;

        /// <summary>
        /// 区分ごとのコース
        /// </summary>
        private IkkatsuChumonCourse _ikkatsuChumonCourse;

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
        /// 列として採用する商品名の一覧（商品ＣＤ、商品名略称）を持つデータテーブル
        /// </summary>
        private DataTable _syohinDt = null;

        /// <summary>
        /// （製造日／配送日算出日数取得用の）部署マスタ
        /// </summary>
        private Busyo _busyo;

        /// <summary>
        /// （休日取得用の）得意先マスタ
        /// </summary>
        private Tokuisaki _tokuisaki;

        /// <summary>
        /// 検索条件を保持する場合はtrue
        /// </summary>
        private bool _keepCriteria = false;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ikkatsuChumonCriteria">一括注文入力検索条件</param>
        public Form202(IkkatsuChumonCriteria ikkatsuChumonCriteria)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _ikkatsuChumonCriteria = new IkkatsuChumonCriteria(ikkatsuChumonCriteria);
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
        private void Form202_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

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

                // 先頭3列を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = SYOHIN_START_COLUMN;

                //***********************************************************************************
                // Spreadの行のロック(編集不可)を有効にするためにシートのプロテクト(保護)を有効にする
                // ただし、Excel出力するとシートのあらゆる書式や設定が変更できなくなるので、
                // Excel出力の前後でプロテクト状態を切り替える(□)
                //***********************************************************************************
                fpSpread1_Sheet1.Protect = true;

                // 常に更新
                _IsNewRecord = false;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                ikkatsuChumonBindingSource.DataSource = _ikkatsuChumonCriteria;

                _ikkatsuChumonCourse = new IkkatsuChumonCourse();
                ikkatsuChumonCourseBindingSource.DataSource = _ikkatsuChumonCourse;

                // デフォルトデータモデルを取得
                _dataModel = (FarPoint.Win.Spread.Model.DefaultSheetDataModel)fpSpread1_Sheet1.Models.Data;

                // 過去注文ラベルを非表示
                lblPast.Visible = false;

                // 検索条件が指定されている場合は検索
                if (_ikkatsuChumonCriteria.得意先ＣＤ != null)
                {
                    _keepCriteria = true;       // 注文入力から開いた場合（検索条件が指定されている場合）は、登録後も検索条件を保持する(△)
                    btnQuery.PerformClick();
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form202_Shown(object sender, EventArgs e)
        {
            // 検索済みならスプレッドにフォーカス
            if (_ikkatsuChumonCriteriaSnapShot != null)
            {
                fpSpread1.Focus();
            }
            else
            {
                // 得意先ＣＤにフォーカス
                txtTokuisakiCD.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form202_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F7:
                    btnCopy.PerformClick();         // コピーボタン押下
                    break;

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
        private void Form202_FormClosing(object sender, FormClosingEventArgs e)
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
        /// 得意先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTokuisakiCD.PerformClick();
            }
        }

        /// <summary>
        /// 得意先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTokuisakiCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuisakiCD_FormClosed);
                formTokuisakiDlg.ShowDialog();
                if (formTokuisakiDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
                    errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _ikkatsuChumonCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;

                    _ikkatsuChumonCriteria.部署ＣＤ = tokuisaki.部署ＣＤ;
                    ReadBinding(cboBusyo);      // コントロールに反映

                    _ikkatsuChumonCriteria.受注時特記事項 = tokuisaki.受注時特記事項;
                    ReadBinding(lblNotice);     // コントロールに反映
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
        /// 平日コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWeekdayCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnWeekdayCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// 平日コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWeekdayCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902WeekdayCourseCD_FormClosed);
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
        /// コース選択ダイアログクローズ（平日コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902WeekdayCourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtWeekdayCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtWeekdayCourseCD.Text = course.コースＣＤ.ToString();
                    _ikkatsuChumonCourse.平日コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblWeekdayCourseName.Text = course.コース名;
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
        /// 土曜コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSaturdayCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSaturdayCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// 土曜コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaturdayCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902SaturdayCourseCD_FormClosed);
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
        /// コース選択ダイアログクローズ（土曜コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902SaturdayCourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSaturdayCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtSaturdayCourseCD.Text = course.コースＣＤ.ToString();
                    _ikkatsuChumonCourse.土曜コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblSaturdayCourseName.Text = course.コース名;
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
        /// 日曜コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSundayCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSundayCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// 日曜コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSundayCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902SundayCourseCD_FormClosed);
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
        /// コース選択ダイアログクローズ（日曜コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902SundayCourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSundayCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtSundayCourseCD.Text = course.コースＣＤ.ToString();
                    _ikkatsuChumonCourse.日曜コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblSundayCourseName.Text = course.コース名;
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
        /// 祝日コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHolidayCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnHolidayCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// 祝日コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHolidayCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902HolidayCourseCD_FormClosed);
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
        /// コース選択ダイアログクローズ（祝日コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902HolidayCourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtHolidayCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtHolidayCourseCD.Text = course.コースＣＤ.ToString();
                    _ikkatsuChumonCourse.祝日コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblHolidayCourseName.Text = course.コース名;
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
                // 検索条件の入力にエラーがある？
                if (_ikkatsuChumonCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_ikkatsuChumonCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _ikkatsuChumonCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                DateTime chumonMonth = new DateTime((int)_ikkatsuChumonCriteria.注文年, (int)_ikkatsuChumonCriteria.注文月, 1);
                if (chumonMonth > warnDate)
                {
                    if (MessageBox.Show(this, "注文年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        ikkatsuChumonCourseBindingSource.DataSource = new IkkatsuChumonCourse();
                        bindingSource1.DataSource = null;
                        _ikkatsuChumonCriteriaSnapShot = null;
                        return;
                    }
                }

                // 過去の場合はラベルを表示する
                lblPast.Visible = (chumonMonth < currentMonth);

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //***************************************
                // 得意先に該当する区分ごとのコースを表示
                //***************************************
                errorProvider1.SetError(txtWeekdayCourseCD, null);  // エラーを消す
                errorProvider1.SetError(txtSaturdayCourseCD, null);
                errorProvider1.SetError(txtSundayCourseCD, null);
                errorProvider1.SetError(txtHolidayCourseCD, null);
                _ikkatsuChumonCourse = IkkatsuChumonCourse.GetIkkatsuChumonCourse((int)_ikkatsuChumonCriteria.部署ＣＤ, (int)_ikkatsuChumonCriteria.得意先ＣＤ);
                ikkatsuChumonCourseBindingSource.DataSource = _ikkatsuChumonCourse;

                // 部署マスタを取得（製造日／配送日算出日数の取得用）
                _busyo = Busyo.GetBusyo((int)_ikkatsuChumonCriteria.部署ＣＤ);

                // 得意先マスタを取得（休日取得用）
                _tokuisaki = Tokuisaki.GetTokuisaki((int)_ikkatsuChumonCriteria.得意先ＣＤ);

                // タイトルバーに得意先名略称を表示
                if (string.IsNullOrEmpty(_tokuisaki.得意先名略称))
                {
                    this.Text = "一括注文入力";
                }
                else
                {
                    this.Text = string.Format("{0} - 一括注文入力", _tokuisaki.得意先名略称);
                }

                // Spreadの内部状態が壊れてColumnCountやRowCountを設定するとIndexOutOfRangeExceptionが発生することがあるので、それを回避すべく、DataSourceをリセットする
                bindingSource1.DataSource = null;

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                //************************************************************
                // 主要商品リストマスタにある商品名を列として追加し、
                // 注文年月に該当する日を行とする
                // データそのものは注文明細から取得する
                //************************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                DataTable dt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n With T(I) AS" +         // -- 0～30までを取得する再帰CTE（共通テーブル式）
                                      "\r\n (SELECT" +
                                      "\r\n     0 AS I" +
                                      "\r\n UNION ALL" +
                                      "\r\n SELECT" +
                                      "\r\n     I + 1" +
                                      "\r\n FROM" +
                                      "\r\n     T" +
                                      "\r\n WHERE" +
                                      "\r\n     I < 30" +
                                      "\r\n )" +
                                      "\r\n SELECT" +               // -- ② ①で取得した日付に該当する注文を取得。同時にwhere句で年月を指定する
                                      "\r\n     A.日付" +
                                      "\r\n     ,S.名称 AS 祝日" +
                                      "\r\n     ,B.コースＣＤ" +
                                      "\r\n     ,CASE WHEN B.商品ＣＤ IS NULL THEN TM.備考1 ELSE B.備考1 END AS 備考1" +    // -- 注文が無ければ得意先の備考1を初期表示
                                      "\r\n     ,B.商品ＣＤ" +
                                      "\r\n     ,B.商品数量" +
                                      "\r\n     ,0 AS 合計行" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +          // -- ① 0～30までを取得する再帰CTEを用いて、指定した日付以降の日付を取得
                                      "\r\n         CONVERT(date, DateAdd(day, I, @注文日From)) AS 日付" +
                                      "\r\n     FROM" +
                                      "\r\n         T" +
                                      "\r\n     ) AS A" +
                                      "\r\n     LEFT JOIN" +
                                      // -- ②－１ 予測を取得
                                      "\r\n         (SELECT" +
                                      "\r\n             HC.コースＣＤ" +
                                      "\r\n             ,HY.喫食日" +
                                      "\r\n             ,HC.備考1" +                // -- (予測ではなく)注文の備考
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
                                      "\r\n             AND HY.喫食日 >= @注文日From" +
                                      "\r\n             AND HY.喫食日 <= @注文日To" +
                                      "\r\n             AND HY.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n             AND HY.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND HY.注文区分 = @注文区分予測" +
                                      // -- ②－２ 注文を取得
                                      "\r\n         UNION" +
                                      "\r\n         SELECT" +
                                      "\r\n             H.コースＣＤ" +
                                      "\r\n             ,H.喫食日" +
                                      "\r\n             ,H.備考1" +
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
                                      "\r\n             AND H.喫食日 >= @注文日From" +
                                      "\r\n             AND H.喫食日 <= @注文日To" +
                                      "\r\n             AND H.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n             AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND H.注文区分 = @注文区分注文" +
                                      "\r\n     ) AS B" +
                                      "\r\n         ON B.喫食日 = A.日付" +
                                      "\r\n     LEFT JOIN 祝日マスタ AS S" +
                                      "\r\n         ON S.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND S.対象日 = A.日付" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS TM" +     // -- 備考取得用の得意先マスタ
                                      "\r\n         ON TM.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND TM.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     YEAR(A.日付) = @注文年" +
                                      "\r\n AND MONTH(A.日付) = @注文月" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     NULL AS 日付" +
                                      "\r\n     ,'合計' AS 祝日" +
                                      "\r\n     ,-1 AS コースＣＤ" +
                                      "\r\n     ,NULL AS 備考1" +
                                      "\r\n     ,NULL AS 商品ＣＤ" +
                                      "\r\n     ,-1 AS 商品数量" +
                                      "\r\n     ,1 AS 合計行" +
                                      "\r\n ORDER BY" +
                                      "\r\n     合計行" +
                                      "\r\n     ,日付" +
                                      "\r\n     ,商品ＣＤ";

                    int year = (int)_ikkatsuChumonCriteria.注文年;
                    int month = (int)_ikkatsuChumonCriteria.注文月;
                    cmd.Parameters.Add("@注文日From", SqlDbType.Date).Value = new DateTime(year, month, 1);
                    cmd.Parameters.Add("@注文日To", SqlDbType.Date).Value = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    cmd.Parameters.Add("@注文年", SqlDbType.Int).Value = _ikkatsuChumonCriteria.注文年;
                    cmd.Parameters.Add("@注文月", SqlDbType.Int).Value = _ikkatsuChumonCriteria.注文月;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("一括注文入力リストを検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("一括注文入力");
                // 商品名列を作成
                cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["日付"]);
                cldt.AddFixedColumn(dt.Columns["祝日"]);
                cldt.AddFixedColumn(dt.Columns["備考1"]);
                // インポート
                cldt.Import(dt, "商品ＣＤ", "商品数量");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);
                cldt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "一括注文入力";

                fpSpread1_Sheet1.BindDataColumn(CHUMONBI_COLUMN, "日付");
                fpSpread1_Sheet1.BindDataColumn(HOLIDAY_COLUMN, "祝日");
                fpSpread1_Sheet1.BindDataColumn(BIKO_COLUMN, "備考1");

                // 商品名を列名としてバインド
                int columnIndex = SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // 休日・祝日に着色する
                // 日曜・祝日は日付の文字色を赤にする
                // 得意先の休日は行全体をピンクにする
                // 祝日 かつ 祝日配送区分がOFFの場合は行全体をピンクにする
                // 最終行は合計行なので無視する
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();

                    DataRow dr = cldt.Rows[rowIndex];
                    DateTime? date = dr["日付"] as DateTime?;
                    string holiday = dr["祝日"] as string;

                    if (date != null)
                    {
                        // 文字色の設定
                        // 日曜 または 祝日？
                        if (((DateTime)date).DayOfWeek == DayOfWeek.Sunday || string.IsNullOrEmpty(holiday) == false)
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, CHUMONBI_COLUMN].ForeColor = Color.Red;
                        }
                        else
                        {
                            fpSpread1_Sheet1.Cells[rowIndex, CHUMONBI_COLUMN].ForeColor = Color.Empty;  // ResetForeColor()が効かないので
                        }

                        // 背景色の設定
                        // 得意先の休日？
                        bool isHoliday = _tokuisaki.GetHoliday(((DateTime)date).DayOfWeek);
                        if (isHoliday)
                        {
                            fpSpread1_Sheet1.Rows[rowIndex].BackColor = HOLIDAY_BACK_COLOR;
                        }
                        else
                        {
                            // 祝日 かつ 祝日配送区分がOFF？
                            if (string.IsNullOrEmpty(holiday) == false && (_tokuisaki.祝日配送区分 ?? false) == false)
                            {
                                fpSpread1_Sheet1.Rows[rowIndex].BackColor = HOLIDAY_BACK_COLOR;
                            }
                        }
                    }
                }

                // 予測列については入力不可に(ロック)する
                for (columnIndex = SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                {
                    if (columnIndex % 2 == 1)
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

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, SYOHIN_START_COLUMN);
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _ikkatsuChumonCriteriaSnapShot = new IkkatsuChumonCriteria(_ikkatsuChumonCriteria);
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
        /// SPREAD編集モードON
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
        /// SPREAD編集モードOFF
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
        /// コピーボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                // 未検索の場合はNOP
                if (fpSpread1_Sheet1.Rows.Count == 0)
                {
                    return;
                }

                // カレントセルが注文日・祝日・予測の場合はNOP
                int currentColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                if (currentColumnIndex < SYOHIN_START_COLUMN)
                {
                    return;
                }
                if (currentColumnIndex % 2 == 1)
                {
                    return;
                }

                DataSet ds = (DataSet)bindingSource1.DataSource;
                if (ds == null)
                {
                    return;
                }
                DataTable dt = ds.Tables["一括注文入力"];

                // カレントセルに入力されている値を取得
                int currentRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int? suryo = null;
                object value = fpSpread1_Sheet1.Cells[currentRowIndex, fpSpread1_Sheet1.ActiveColumnIndex].Value;
                if (value != null)
                {
                    suryo = Convert.ToInt32(value);
                }

                // 取得した値を、カレント行以降（ただし合計行は除く）のセルに設定
                // ただし、得意先の休日は除く
                for (int rowIndex = currentRowIndex + 1; rowIndex < fpSpread1_Sheet1.Rows.Count - 1; rowIndex++)
                {
                    if (fpSpread1_Sheet1.Rows[rowIndex].BackColor != HOLIDAY_BACK_COLOR)
                    {
                        // SpreadのセルのValueを変更しただけでは、データソースとして紐付いているDataRowのRowStateが変わらないので強制的に変更する
                        // しかもValueをセットする前にSetModifiedしないといけない（これが逆だとValueの値が元に戻ってしまう）
                        if (dt.Rows[rowIndex].RowState == DataRowState.Unchanged)
                        {
                            dt.Rows[rowIndex].SetModified();
                        }

                        fpSpread1_Sheet1.Cells[rowIndex, fpSpread1_Sheet1.ActiveColumnIndex].Value = suryo;
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
                    // 注文入力から開いた場合（検索条件が指定されている場合）は、登録後も検索条件を保持する(△)
                    if (_keepCriteria)
                    {
                        Reload();
                    }
                    else
                    {
                        Clear();
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
            catch (BentoException ex)
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
        /// 列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
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
                                      "\r\n     ,'予測' AS 予測注文" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n     AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ②注文レコードから(予測用)
                                      "\r\n SELECT" +
                                      "\r\n     M.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_0' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
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
                                      "\r\n     H.喫食日 >= @注文日From" +
                                      "\r\n     AND H.喫食日 <= @注文日To" +
                                      "\r\n     AND H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n UNION" +
                                      // -- ③主要商品リストマスタから(注文用)
                                      "\r\n SELECT" +
                                      "\r\n     A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n     ,'注文' AS 予測注文" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     INNER JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n     AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      // -- ④注文レコードから(注文用)
                                      "\r\n SELECT" +
                                      "\r\n     M.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, M.商品ＣＤ) + '_1' AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
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
                                      "\r\n     H.喫食日 >= @注文日From" +
                                      "\r\n     AND H.喫食日 <= @注文日To" +
                                      "\r\n     AND H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     ソート用商品ＣＤ" +
                                      "\r\n     ,商品ＣＤ";

                    int year = (int)_ikkatsuChumonCriteria.注文年;
                    int month = (int)_ikkatsuChumonCriteria.注文月;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@注文日From", SqlDbType.Date).Value = new DateTime(year, month, 1);
                    cmd.Parameters.Add("@注文日To", SqlDbType.Date).Value = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.得意先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("商品一覧を取得しました。");
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
        /// 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="dt"></param>
        private void SetSpreadColumn(DataTable syohinDt, string labelColumnName)
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
                    if (columnIndex % 2 == 1)
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
                        //fpSpread1_Sheet1.Columns.Get(columnIndex).CanFocus = false;
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
                    }

                    columnIndex++;
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

            DataTable dt = ds.Tables["一括注文入力"];

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

                // 検索条件の入力にエラーがある？
                if (_ikkatsuChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _ikkatsuChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実行？
                if (_ikkatsuChumonCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_ikkatsuChumonCriteria.Equals(_ikkatsuChumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 部署と関連しないコースの場合は警告を表示
                bool isErrorWeekday = HasErrorCourseBusyo(_ikkatsuChumonCourse.平日コースＣＤ, txtWeekdayCourseCD);
                bool isErrorSaturday = HasErrorCourseBusyo(_ikkatsuChumonCourse.土曜コースＣＤ, txtSaturdayCourseCD);
                bool isErrorSunday = HasErrorCourseBusyo(_ikkatsuChumonCourse.日曜コースＣＤ, txtSundayCourseCD);
                bool isErrorHoliday = HasErrorCourseBusyo(_ikkatsuChumonCourse.祝日コースＣＤ, txtHolidayCourseCD);
                if (isErrorWeekday || isErrorSaturday || isErrorSunday || isErrorHoliday)
                {
                    if (MessageBox.Show(this, "部署と関連しないコースが指定されています。続行しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        return false;
                    }
                }

                errorProvider1.SetError(txtWeekdayCourseCD, null);  // エラーを消す
                errorProvider1.SetError(txtSaturdayCourseCD, null);
                errorProvider1.SetError(txtSundayCourseCD, null);
                errorProvider1.SetError(txtHolidayCourseCD, null);

                // 得意先が属さないコースの場合はエラー
                isErrorWeekday = HasErrorCourseTokuisaki(_ikkatsuChumonCourse.平日コースＣＤ, txtWeekdayCourseCD);
                isErrorSaturday = HasErrorCourseTokuisaki(_ikkatsuChumonCourse.土曜コースＣＤ, txtSaturdayCourseCD);
                isErrorSunday = HasErrorCourseTokuisaki(_ikkatsuChumonCourse.日曜コースＣＤ, txtSundayCourseCD);
                isErrorHoliday = HasErrorCourseTokuisaki(_ikkatsuChumonCourse.祝日コースＣＤ, txtHolidayCourseCD);
                if (isErrorWeekday || isErrorSaturday || isErrorSunday || isErrorHoliday)
                {
                    MessageBox.Show(this, "コースの中に得意先が見つかりません。得意先が属するコースを指定してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// コースと部署をチェック
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="txt">エラー表示対象のテキストボックス</param>
        /// <returns>部署と関連しないコースの場合はtrue。それ以外はFalse</returns>
        private bool HasErrorCourseBusyo(int? courseCD, TextBox txt)
        {
            try
            {
                // コースが未入力の場合はエラーとはしない
                if (courseCD == null)
                {
                    return false;
                }

                // 部署と関連しないコース？
                Course course = Course.GetCourse((int)courseCD);
                if (course.部署ＣＤ != _ikkatsuChumonCriteria.部署ＣＤ)
                {
                    errorProvider1.SetError(txt, "部署と無関係のコースです。");
                    return true;
                }
                return false;
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
        /// コースと得意先をチェック
        /// </summary>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="txt">エラー表示対象のテキストボックス</param>
        /// <returns>得意先が属さないコースの場合はtrue。それ以外はFalse</returns>
        private bool HasErrorCourseTokuisaki(int? courseCD, TextBox txt)
        {
            try
            {
                // コースが未入力の場合はエラーとはしない
                if (courseCD == null)
                {
                    return false;
                }

                // 得意先が属さないコース？
                CourseTable courseTable = CourseTable.GetCourseTable((int)courseCD, (int)_ikkatsuChumonCriteria.得意先ＣＤ);
                if (courseTable == null)
                {
                    errorProvider1.SetError(txt, "得意先と無関係のコースです。");
                    return true;
                }
                return false;
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
                bool? ret = false;
                History.ProcessType historyProcess = History.ProcessType.Update;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();
                    try
                    {
                        //********************************************************************************************************************************
                        // ・条件に合致する予測レコードを全て取得
                        // ・得意先に該当する注文明細を全て削除
                        // ・cldtの行をループしながら以下を実施
                        //   ・cldtの行（注文日）の、注文ヘッダを取得
                        //   ・商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //   ・cldtの行（注文日）の、商品ＣＤに該当する列の値を取得
                        //   ・注文明細を追加
                        //   ・注文ヘッダが有れば更新、無ければ追加
                        //   ・その日のいずれかの商品に注文が入力された場合は、その日の予測レコードのみがある商品に対して注文数が0の注文レコードを追加する
                        //     （予測のみ存在する場合は0注文を作成する）
                        //     （注文明細だけならSQL１発で作成できるが、注文ヘッダを作成しないといけないので、SQLではやらない）
                        // ・最後に、注文明細がない注文ヘッダを削除
                        //********************************************************************************************************************************

                        // 0注文を追加する条件は以下の通り
                        //
                        // 初期状態
                        // ┌──┬─────┬─────┬─────┐
                        // │    │  商品Ａ  │  商品Ｂ  │  商品Ｃ  │
                        // ├──┼──┬──┼──┬──┼──┬──┤
                        // │    │予測│注文│予測│注文│予測│注文│
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/15│  10│    │    │    │  11│    │
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/16│  12│    │  13│    │    │    │
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/17│  14│    │  15│    │    │    │
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/18│    │    │    │    │    │    │
                        // └──┴──┴──┴──┴──┴──┴──┘
                        //
                        // 一括注文入力した状態(更新前)
                        // ┌──┬─────┬─────┬─────┐
                        // │    │  商品Ａ  │  商品Ｂ  │  商品Ｃ  │
                        // ├──┼──┬──┼──┬──┼──┬──┤
                        // │    │予測│注文│予測│注文│予測│注文│
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/15│  10│  21│    │    │  11│    │←①予測がある商品に注文を入力
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/16│  12│    │  13│    │    │  22│←②予測が無い商品に注文を入力
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/17│  14│    │  15│    │    │    │←③注文を入力しない
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/18│    │  23│    │    │    │    │←④予測が無い日に注文を入力
                        // └──┴──┴──┴──┴──┴──┴──┘
                        //
                        // 一括注文入力後(更新後)
                        // ┌──┬─────┬─────┬─────┐
                        // │    │  商品Ａ  │  商品Ｂ  │  商品Ｃ  │
                        // ├──┼──┬──┼──┬──┼──┬──┤
                        // │    │予測│注文│予測│注文│予測│注文│
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/15│  10│  21│    │    │  11│   0│←①同じ日の予測がある商品に0注文を作る
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/16│  12│   0│  13│   0│    │  22│←②同じ日の予測がある商品に0注文を作る
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/17│  14│    │  15│    │    │    │←③注文を入力しなかった日には、予測があっても0注文は作成しない
                        // ├──┼──┼──┼──┼──┼──┼──┤
                        // │4/18│    │  23│    │    │    │    │←④予測が無いので0注文は作成しない
                        // └──┴──┴──┴──┴──┴──┴──┘

                        //*****************************
                        // 注文明細の予測レコードを取得
                        //*****************************
                        DataTable dtYosoku = GetChumonMeisai(Consts.ChumonKubun.Yosoku);

                        //***************************************************
                        // 注文明細の注文レコードを取得（入力内容との比較用）
                        //***************************************************
                        DataTable dtChumon = GetChumonMeisai(Consts.ChumonKubun.Chuumon);

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
                                              "\r\n WHERE M.企業ＣＤ = @企業ＣＤ " +
                                              "\r\n AND M.注文区分 = @注文区分 " +
                                              "\r\n AND M.喫食日 >= @喫食日From " +
                                              "\r\n AND M.喫食日 <= @喫食日To " +
                                              "\r\n AND M.得意先ＣＤ = @得意先ＣＤ" +
                                              "\r\n AND M.部署ＣＤ = @部署ＣＤ";

                        int year = (int)_ikkatsuChumonCriteria.注文年;
                        int month = (int)_ikkatsuChumonCriteria.注文月;
                        cmdDelM.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmdDelM.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                        cmdDelM.Parameters.Add("@喫食日From", SqlDbType.Date).Value = new DateTime(year, month, 1);
                        cmdDelM.Parameters.Add("@喫食日To", SqlDbType.Date).Value = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                        cmdDelM.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.得意先ＣＤ;
                        cmdDelM.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.部署ＣＤ;
                        cmdDelM.Transaction = tran;
                        log.Debug(cmdDelM.CommandTextWithParameters());
                        cmdDelM.ExecuteNonQuery();

                        //***************************************************************************
                        // cldtの行をループしつつ、商品一覧データテーブルをループしつつ商品ＣＤを取得
                        //***************************************************************************
                        DataSet ds = (DataSet)bindingSource1.DataSource;
                        ColumnListDataTable cldt = (ColumnListDataTable)ds.Tables["一括注文入力"];

                        for (int rowIndex = 0; rowIndex < cldt.Rows.Count - 1; rowIndex++)      // 最終行は合計行なので-1
                        {
                            DateTime kissyokubi = (DateTime)cldt.Rows[rowIndex]["日付"];        // 行に該当する注文日を取得
                            DateTime haisobi = CalcHaisobi(kissyokubi, _busyo, _tokuisaki);     // 配送日を算出
                            string biko = cldt.Rows[rowIndex]["備考1"] as string;               // 行に該当する備考を取得
                            int seq = 0;

                            // 消費税を取得
                            Syohizei syohizei = Syohizei.GetSyohizei(kissyokubi);

                            // 注文ヘッダ更新フラグを初期化
                            bool needChumonHeaderUpdate = false;

                            //*****************
                            // 注文ヘッダを取得
                            //*****************
                            ChumonHeader chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Chuumon, kissyokubi, (int)_ikkatsuChumonCriteria.得意先ＣＤ, (int)_ikkatsuChumonCriteria.部署ＣＤ);
                            if (chumonHeader == null)
                            {
                                chumonHeader = new ChumonHeader();
                                chumonHeader.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                chumonHeader.喫食日 = kissyokubi;
                                chumonHeader.得意先ＣＤ = _ikkatsuChumonCriteria.得意先ＣＤ;
                                chumonHeader.部署ＣＤ = _ikkatsuChumonCriteria.部署ＣＤ;
                                chumonHeader.配送日 = haisobi;
                                chumonHeader.納品書伝票番号 = null;

                                // 得意先マスタから
                                chumonHeader.得意先名 = _tokuisaki.得意先名;
                                chumonHeader.配送先ＣＤ = _tokuisaki.受注得意先ＣＤ;
                                chumonHeader.税区分 = _tokuisaki.税区分 ?? (int)Syohizei.ZeiKubun.Uchi;
                                chumonHeader.丸め処理区分 = _tokuisaki.丸め処理区分 ?? (int)Rounding.HasuKubun.Floor;

                                chumonHeader.備考1 = biko;                  // 備考1は画面で入力されたものを採用する
                                chumonHeader.備考2 = _tokuisaki.備考２;
                                chumonHeader.備考3 = _tokuisaki.備考３;
                                chumonHeader.備考4 = null;
                                chumonHeader.備考5 = null;
                            }
                            else
                            {
                                if (chumonHeader.備考1 != biko)
                                {
                                    chumonHeader.備考1 = biko;
                                    needChumonHeaderUpdate = true;
                                }
                            }

                            List<int> syohinCDList = new List<int>();       // 0注文追加用の商品CDリスト
                            bool hasSuryo = false;                          // 数量が入力されていればtrue

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

                                // 注文未入力？
                                if (suryo == null)
                                {
                                    // 既存の注文明細が存在していれば、注文を削除しようとしていることなので、注文ヘッダ（の修正日）を更新する(フラグを立てる)
                                    drs = dtChumon.Select(string.Format("喫食日=#{0:yyyy/MM/dd}# and 商品ＣＤ={1}", kissyokubi, syohinCD));
                                    if (drs != null && drs.Length != 0)
                                    {
                                        needChumonHeaderUpdate = true;
                                    }

                                    // 0注文追加用の商品CDを憶えておく
                                    syohinCDList.Add(syohinCD);
                                    continue;
                                }
                                else
                                {
                                    hasSuryo = true;        // 数量が入力されている
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

                                //***************************
                                // 注文明細を追加（ここから）
                                //***************************
                                ChumonMeisai chumonMeisai = new ChumonMeisai();
                                chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                chumonMeisai.喫食日 = kissyokubi;
                                chumonMeisai.得意先ＣＤ = _ikkatsuChumonCriteria.得意先ＣＤ;
                                chumonMeisai.部署ＣＤ = _ikkatsuChumonCriteria.部署ＣＤ;
                                chumonMeisai.商品ＣＤ = syohinCD;
                                seq++;
                                chumonMeisai.明細番号 = seq;

                                DateTime seizobi = CalcSeizobi(kissyokubi, _busyo);                     // 製造日を算出
                                DateTime? date = AdjustSeizobiHaisobi(seizobi, haisobi, _busyo, _tokuisaki);    // 配送日＜製造日の場合は調整
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
                                TokuisakiTanka tokuisakiTanka = TokuisakiTanka.GetTokuisakiTanka((int)_ikkatsuChumonCriteria.得意先ＣＤ, syohinCD);
                                if (tokuisakiTanka != null)
                                {
                                    chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, tokuisakiTanka.商品単価税込, tokuisakiTanka.商品単価税抜);
                                }

                                // 得意先マスタから
                                chumonMeisai.売掛現金区分 = _tokuisaki.売掛現金区分 ?? (int)Consts.UriGenKubun.Genkin;

                                // 主要商品リストマスタから
                                SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)_ikkatsuChumonCriteria.部署ＣＤ, syohinCD);
                                if (syuyoSyohin != null)
                                {
                                    chumonMeisai.商品グループＣＤ = syuyoSyohin.商品グループＣＤ;
                                }

                                // 商品金額や税額を算出
                                chumonMeisai.商品金額合計 = Syohizei.GetAmount(chumonMeisai.商品数量, chumonMeisai.商品単価);
                                chumonMeisai.消費税額 = syohizei.GetTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                chumonMeisai.商品金額合計税込 = syohizei.GetAmountIncludeTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                chumonMeisai.計算税率 = syohizei.GetTaxRate(syohin.軽減税率フラグ);

                                ret = chumonMeisai.Insert(con, tran);
                                if (ret == false)
                                {
                                    MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                historyProcess = History.ProcessType.Create;
                                History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);
                                //***************************
                                // 注文明細を追加（ここまで）
                                //***************************
                            }

                            // 数量が入力されており、注文日に該当する予測レコードが１件でも有る場合は、0注文を追加する
                            // ※その日の注文数がまったく入力されなかった場合、または、すべて削除された場合は、0注文を作成しない（予測を優先する）
                            if (hasSuryo)
                            {
                                foreach (int syohinCDYosoku in syohinCDList)
                                {
                                    //**************************************************************
                                    // 注文数が未入力の場合、予測レコードがあれば、注文数を0とみなす
                                    // （予測のみ存在する場合は0注文を作成する）
                                    // 予測が存在しない場合は何もしない（次のレコードへ）
                                    //**************************************************************
                                    drs = dtYosoku.Select(string.Format("喫食日=#{0:yyyy/MM/dd}# and 商品ＣＤ={1}", kissyokubi, syohinCDYosoku));
                                    if (drs == null || drs.Length == 0)
                                    {
                                        continue;
                                    }

                                    // 0注文を追加することが確定したので、注文ヘッダ（の修正日）を更新する(フラグを立てる)
                                    needChumonHeaderUpdate = true;

                                    //***************************
                                    // 注文明細を追加（ここから）
                                    //***************************
                                    ChumonMeisai chumonMeisai = new ChumonMeisai();
                                    chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                    chumonMeisai.喫食日 = kissyokubi;
                                    chumonMeisai.得意先ＣＤ = _ikkatsuChumonCriteria.得意先ＣＤ;
                                    chumonMeisai.部署ＣＤ = _ikkatsuChumonCriteria.部署ＣＤ;
                                    chumonMeisai.商品ＣＤ = syohinCDYosoku;
                                    seq++;
                                    chumonMeisai.明細番号 = seq;

                                    DateTime seizobi = CalcSeizobi(kissyokubi, _busyo);                     // 製造日を算出
                                    DateTime? date = AdjustSeizobiHaisobi(seizobi, haisobi, _busyo, _tokuisaki);    // 配送日＜製造日の場合は調整
                                    if (date != null)
                                    {
                                        seizobi = (DateTime)date;
                                        haisobi = (DateTime)date;
                                    }
                                    chumonMeisai.製造日 = seizobi;
                                    chumonMeisai.商品数量 = 0;          // 0注文

                                    // 商品マスタから
                                    Syohin syohin = Syohin.GetSyohin(syohinCDYosoku);
                                    if (syohin != null)
                                    {
                                        chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, syohin.商品単価税込, syohin.商品単価税抜);
                                    }

                                    // 得意先単価マスタから
                                    TokuisakiTanka tokuisakiTanka = TokuisakiTanka.GetTokuisakiTanka((int)_ikkatsuChumonCriteria.得意先ＣＤ, syohinCDYosoku);
                                    if (tokuisakiTanka != null)
                                    {
                                        chumonMeisai.商品単価 = Syohizei.GetUnitPrice(chumonHeader.税区分, tokuisakiTanka.商品単価税込, tokuisakiTanka.商品単価税抜);
                                    }

                                    // 得意先マスタから
                                    chumonMeisai.売掛現金区分 = _tokuisaki.売掛現金区分 ?? (int)Consts.UriGenKubun.Genkin;

                                    // 主要商品リストマスタから
                                    SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)_ikkatsuChumonCriteria.部署ＣＤ, syohinCDYosoku);
                                    if (syuyoSyohin != null)
                                    {
                                        chumonMeisai.商品グループＣＤ = syuyoSyohin.商品グループＣＤ;
                                    }

                                    // 商品金額や税額を算出
                                    chumonMeisai.商品金額合計 = Syohizei.GetAmount(chumonMeisai.商品数量, chumonMeisai.商品単価);
                                    chumonMeisai.消費税額 = syohizei.GetTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                    chumonMeisai.商品金額合計税込 = syohizei.GetAmountIncludeTax(chumonHeader.税区分, chumonMeisai.商品数量, chumonMeisai.商品単価, syohin.軽減税率フラグ, chumonHeader.丸め処理区分);
                                    chumonMeisai.計算税率 = syohizei.GetTaxRate(syohin.軽減税率フラグ);

                                    ret = chumonMeisai.Insert(con, tran);
                                    if (ret == false)
                                    {
                                        MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return false;
                                    }
                                    historyProcess = History.ProcessType.Create;
                                    History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);
                                    //***************************
                                    // 注文明細を追加（ここまで）
                                    //***************************
                                }
                            }

                            // 注文明細を追加していなければ、注文ヘッダを追加する必要がない
                            if (seq == 0)
                            {
                                continue;
                            }

                            // ****************************************************************
                            // 算出した配送日を設定した注文ヘッダを更新。新規作成していれば追加
                            // ****************************************************************
                            chumonHeader.配送日 = haisobi;

                            // 配送日の曜日に応じたコースを取得
                            Consts.CourseKubun courseKubun = (Consts.CourseKubun)Course.GetCourseKubun(haisobi);
                            switch(courseKubun)
                            {
                                case Consts.CourseKubun.Weekday:        // 平日
                                    chumonHeader.コースＣＤ = _ikkatsuChumonCourse.平日コースＣＤ;
                                    break;

                                case Consts.CourseKubun.Saturday:       // 土曜
                                    chumonHeader.コースＣＤ = _ikkatsuChumonCourse.土曜コースＣＤ;
                                    break;

                                case Consts.CourseKubun.Sunday:         // 日曜
                                    chumonHeader.コースＣＤ = _ikkatsuChumonCourse.日曜コースＣＤ;
                                    break;

                                case Consts.CourseKubun.Holiday:        // 祝日
                                    chumonHeader.コースＣＤ = _ikkatsuChumonCourse.祝日コースＣＤ;
                                    break;
                            }
                            // コースマスタから
                            chumonHeader.担当者ＣＤ = GetCourseTanto(con, tran, chumonHeader.コースＣＤ, haisobi);

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
                                    continue;
                                }
                            }

                            if (ret == false)
                            {
                                MessageBox.Show(this, chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                            History.Save(con, tran, this.Text, "注文ヘッダ", chumonHeader.Code, historyProcess);
                        }

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
                                              "\r\n AND H.注文区分 = @注文区分" +
                                              "\r\n AND H.喫食日 >= @喫食日From "+
                                              "\r\n AND H.喫食日 <= @喫食日To " +
                                              "\r\n AND H.得意先ＣＤ = @得意先ＣＤ" +
                                              "\r\n AND H.部署ＣＤ = @部署ＣＤ" +
                                              "\r\n AND M.企業ＣＤ IS NULL";
                        year = (int)_ikkatsuChumonCriteria.注文年;
                        month = (int)_ikkatsuChumonCriteria.注文月;
                        cmdDelH.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmdDelH.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                        cmdDelH.Parameters.Add("@喫食日From", SqlDbType.Date).Value = new DateTime(year, month, 1);
                        cmdDelH.Parameters.Add("@喫食日To", SqlDbType.Date).Value = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                        cmdDelH.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.得意先ＣＤ;
                        cmdDelH.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.部署ＣＤ;
                        cmdDelH.Transaction = tran;
                        log.Debug(cmdDelH.CommandTextWithParameters());
                        cmdDelH.ExecuteNonQuery();

                        historyProcess = History.ProcessType.Update;

                        tran.Commit();
                        log.InfoFormat("一括注文入力を{0}しました。", historyProcess.DisplayName());
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
        /// 注文明細レコード取得
        /// </summary>
        /// <param name="chumonKubun">注文区分</param>
        private DataTable GetChumonMeisai(Consts.ChumonKubun chumonKubun)
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
                                      "\r\n AND M.注文区分 = @注文区分 " +
                                      "\r\n AND M.喫食日 >= @喫食日From " +
                                      "\r\n AND M.喫食日 <= @喫食日To " +
                                      "\r\n AND M.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n AND M.部署ＣＤ = @部署ＣＤ";

                    int year = (int)_ikkatsuChumonCriteria.注文年;
                    int month = (int)_ikkatsuChumonCriteria.注文月;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)chumonKubun;
                    cmd.Parameters.Add("@喫食日From", SqlDbType.Date).Value = new DateTime(year, month, 1);
                    cmd.Parameters.Add("@喫食日To", SqlDbType.Date).Value = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _ikkatsuChumonCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.InfoFormat("{0}を検索しました。", tableName);
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
        /// コースの担当者を取得
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="courseCD">コースＣＤ</param>
        /// <param name="targetDate">対象日</param>
        /// <returns>対象日のコースＣＤに該当する担当者ＣＤ</returns>
        private int? GetCourseTanto(SqlConnection con, SqlTransaction tran, int? courseCD, DateTime? targetDate)
        {
            try
            {
                // コースが無い場合は担当者も無い
                if (courseCD == null)
                {
                    return null;
                }

                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                                  // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     C.コースＣＤ" +
                                  "\r\n     ,C.コース区分" +
                                  "\r\n     ,C.コース名" +
                                  "\r\n     ,C.部署ＣＤ" +
                                  "\r\n     ,C.担当者ＣＤ" +
                                  "\r\n FROM" +
                                  "\r\n     コースマスタ AS C" +
                                  "\r\n     LEFT JOIN" +
                                  "\r\n         (SELECT" +
                                  "\r\n             コースＣＤ" +
                                  "\r\n         FROM" +
                                  "\r\n             臨時コースマスタ" +
                                  "\r\n         WHERE" +
                                  "\r\n             企業ＣＤ = @企業ＣＤ" +
                                  "\r\n             AND ISNULL(無効フラグ, 0) = 0" +
                                  "\r\n             AND コースＣＤ = @コースＣＤ" +
                                  "\r\n             AND 実施日 = @実施日" +
                                  "\r\n         ) AS RM" +
                                  "\r\n         ON RM.コースＣＤ = C.コースＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     C.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND C.コースＣＤ = @コースＣＤ" +
                                  "\r\n     AND ISNULL(C.無効フラグ, 0) = 0" +
                                  "\r\n     AND RM.コースＣＤ IS NULL" +
                                  // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）"
                                  "\r\n UNION" +
                                  "\r\n SELECT" +
                                  "\r\n     RM.コースＣＤ" +
                                  "\r\n     ,RM.コース区分" +
                                  "\r\n     ,RM.コース名" +
                                  "\r\n     ,RM.部署ＣＤ" +
                                  "\r\n     ,RM.担当者ＣＤ" +
                                  "\r\n FROM" +
                                  "\r\n     臨時コーステーブル AS RT" +
                                  "\r\n     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                  "\r\n WHERE" +
                                  "\r\n     RM.企業ＣＤ = @企業ＣＤ" +
                                  "\r\n     AND RM.コースＣＤ = @コースＣＤ" +
                                  "\r\n     AND ISNULL(RT.無効フラグ, 0) = 0" +
                                  "\r\n     AND RM.実施日 = @実施日";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = courseCD;
                cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = targetDate;

                log.Debug(cmd.CommandTextWithParameters());

                int? tantosyaCD = null;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tantosyaCD = reader["担当者ＣＤ"] as int?;
                    }
                }
                return tantosyaCD;
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
        /// 画面初期化
        /// </summary>
        private void Clear()
        {
            try
            {
                // 検索条件初期化（支店・部署は残す）
                _ikkatsuChumonCriteria.得意先ＣＤ = null;
                _ikkatsuChumonCriteria.得意先名 = null;
                ikkatsuChumonBindingSource.ResetBindings(false);

                _ikkatsuChumonCourse.stopOverwrite = true;
                _ikkatsuChumonCourse.平日コースＣＤ = null;
                _ikkatsuChumonCourse.平日コース名 = null;
                _ikkatsuChumonCourse.土曜コースＣＤ = null;
                _ikkatsuChumonCourse.土曜コース名 = null;
                _ikkatsuChumonCourse.日曜コースＣＤ = null;
                _ikkatsuChumonCourse.日曜コース名 = null;
                _ikkatsuChumonCourse.祝日コースＣＤ = null;
                _ikkatsuChumonCourse.祝日コース名 = null;
                _ikkatsuChumonCourse.stopOverwrite = false;
                ikkatsuChumonCourseBindingSource.ResetBindings(false);

                // 検索条件スナップショットクリア（未検索状態にする）
                _ikkatsuChumonCriteriaSnapShot = null;

                // 一覧初期化
                bindingSource1.DataSource = null;
                fpSpread1_Sheet1.ColumnCount = SYOHIN_START_COLUMN; // 商品名列を削除（リセット）

                txtTokuisakiCD.Focus();
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
            try
            {
                btnQuery.PerformClick();
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

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
    /// 部署マスタメンテ詳細
    /// </summary>
    public partial class Form707D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SET_NAME_COLUMN      = 0;     // セット名列インデックス
        private const int BANK_CD_COLUMN       = 1;     // 金融機関コンボ列インデックス
        private const int BRANCH_CD_COLUMN     = 2;     // 支店ＣＤ列インデックス
        private const int REFERENCE_COLUMN     = 3;     // 参照ボタン列インデックス
        private const int BRANCH_NAME_COLUMN   = 4;     // 支店名列インデックス
        private const int SYUBETSU_COLUMN      = 5;     // 口座種別列インデックス
        private const int BANGO_COLUMN         = 6;     // 口座番号列インデックス
        private const int HIKIOTOSHIBI_COLUMN  = 7;     // 引落日インデックス
        private const int ITAKUSHA_CODE_COLUMN = 8;     // 委託者コード列インデックス
        private const int ITAKUSHA_NAME_COLUMN = 9;     // 委託者名列インデックス
        private const int DELETE_COLUMN        = 10;    // 削除ボタン列インデックス

        /// <summary>
        /// 登録可能な画像ファイルの最大サイズ
        /// </summary>
        private const int IMAGE_FILE_SIZE_MAX = 100000;

        /// <summary>
        /// 登録可能な画像の最大サイズ（面積）
        /// </summary>
        private const int IMAGE_SIZE_MAX = (450 * 450);
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用部署マスタ
        /// </summary>
        private Busyo _busyo;

        /// <summary>
        /// 初期状態用部署マスタ
        /// </summary>
        private Busyo _origin;

        /// <summary>
        /// 保存用部署画像マスタ
        /// </summary>
        private BusyoImage _busyoImage;

        /// <summary>
        /// 初期状態用部署画像マスタ
        /// </summary>
        private BusyoImage _busyoImageOrigin;

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

        private int _logoImageFilterIndex = 0;      // ロゴ画像のファイル選択ダイアログのフィルターインデックス
        private int _inkanImageFilterIndex = 0;     // 印影画像のファイル選択ダイアログのフィルターインデックス
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="busyo"></param>
        public Form707D(Busyo busyo)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _busyo = new Busyo(busyo);

                // 部署グループ、支店ＣＤ、、口座種別（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _busyo.部署グループ = _busyo.部署グループ ?? -1;
                _busyo.支店ＣＤ = _busyo.支店ＣＤ ?? -1;
                _busyo.口座種別1 = _busyo.口座種別1 ?? -1;
                _busyo.口座種別2 = _busyo.口座種別2 ?? -1;
                _busyo.請求書タイプ区分 = _busyo.請求書タイプ区分 ?? -1;

                _IsNewRecord = false;
                if (_busyo.部署ＣＤ == null)
                {
                    _IsNewRecord = true;
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
        private void Form707D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;

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

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyoGroup(cboBusyoGroup);
                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetHaisoSyukeihyoKubun(cboHaisoSyukeihyoSyohin);
                ComboUtil.SetKozaSyubetsu(cboKozaSyubetsu1);
                ComboUtil.SetKozaSyubetsu(cboKozaSyubetsu2);
                ComboUtil.SetSeikyusyoTypeKubun(cboSeikyusyoType);

                // スプレッドの金融機関と口座種別のコンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbBank = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                SetEBName(cbBank);
                cbBank.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbBank.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[BANK_CD_COLUMN].CellType = cbBank;

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbKozaSyubetsu = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetKozaSyubetsu(cbKozaSyubetsu);
                cbKozaSyubetsu.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbKozaSyubetsu.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[SYUBETSU_COLUMN].CellType = cbKozaSyubetsu;

                // 新規の場合は削除不可
                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnDelete.Enabled = false;

                    // 支店CDコンボの初期値を設定
                    _busyo.支店ＣＤ = UserData.getInstance().支店ＣＤ ?? -1;
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnSave.Text = "更新(F9)";
                    // 既存の場合は商品CD変更不可
                    txtBusyoCD.Enabled = false;
                }

                // 初期状態として保持
                _origin = new Busyo(_busyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                busyoBindingSource.DataSource = _busyo;

                // 部署EB設定マスタを取得
                GetBusyoEB(_busyo.部署ＣＤ);

                // 部署画像マスタを取得
                GetBusyoImage(_busyo.部署ＣＤ);
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
        private void Form707D_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F9:               // 登録or更新
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

                    case Keys.F11:              // 削除
                        btnDelete.Focus();
                        btnDelete.PerformClick();
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
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form707D_FormClosing(object sender, FormClosingEventArgs e)
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
                if (HasChangeBusyo() || HasChangeBusyoEB())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 部署マスタの入力チェック
                            if (CanSaveBusyo() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 部署EB設定マスタの入力チェック
                            if (CanSaveBusyoEB() == false)
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
        /// 金融機関ＣＤ1キー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBankCD1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnBankCD1.PerformClick();
            }
        }

        /// <summary>
        /// 金融機関ＣＤ1参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBankCD1_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 金融機関選択ダイアログを表示
                Form905 formBankDlg = new Form905();
                SubFormOpenInit(formBankDlg);
                formBankDlg.FormClosed += new FormClosedEventHandler(Form905BankCD1_FormClosed);
                formBankDlg.ShowDialog();
                if (formBankDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 金融機関選択ダイアログクローズ（金融機関ＣＤ1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form905BankCD1_FormClosed(object sender, FormClosedEventArgs e)
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
                Form905 form = (Form905)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtBankCD1, null);  // エラーを消す

                    // 金融機関支店CDも消す
                    errorProvider1.SetError(txtBranchCD1, null);
                    txtBranchCD1.Text = "";
                    lblBranchName1.Text = "";
                    _busyo.金融機関支店ＣＤ1 = null;

                    Bank bank = form.SelectedBank;
                    txtBankCD1.Text = bank.銀行ＣＤ;
                    _busyo.金融機関ＣＤ1 = bank.銀行ＣＤ;       // 入力内容を確定させる
                    lblBankName1.Text = bank.銀行名;
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
        /// 金融機関支店CD1キー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBranchCD1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnBranchCD1.PerformClick();
            }
        }

        /// <summary>
        /// 金融機関支店ＣＤ1参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBranchCD1_Click(object sender, EventArgs e)
        {
            try
            {
                // 金融機関CD1が未入力の場合は何もしない
                if (string.IsNullOrEmpty(txtBankCD1.Text) == false && string.IsNullOrEmpty(lblBankName1.Text) == false)
                {
                    Control activeControl = this.ActiveControl;
                    // 金融機関支店選択ダイアログを表示
                    Form906 formBranchDlg = new Form906(txtBankCD1.Text, lblBankName1.Text);
                    SubFormOpenInit(formBranchDlg);
                    formBranchDlg.FormClosed += new FormClosedEventHandler(Form906BranchCD1_FormClosed);
                    formBranchDlg.ShowDialog();
                    if (formBranchDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 金融機関支店選択ダイアログクローズ（金融機関支店ＣＤ1）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form906BranchCD1_FormClosed(object sender, FormClosedEventArgs e)
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
                Form906 form = (Form906)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtBranchCD1, null);   // エラーを消す
                    BankBranch bankBranch = form.SelectedBankBranch;
                    txtBranchCD1.Text = bankBranch.支店ＣＤ;
                    _busyo.金融機関支店ＣＤ1 = bankBranch.支店ＣＤ;         // 入力内容を確定させる
                    lblBranchName1.Text = bankBranch.支店名; ;
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
        /// 金融機関ＣＤ2キー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBankCD2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnBankCD2.PerformClick();
            }
        }

        /// <summary>
        /// 金融機関ＣＤ2参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBankCD2_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 金融機関選択ダイアログを表示
                Form905 formBankDlg = new Form905();
                SubFormOpenInit(formBankDlg);
                formBankDlg.FormClosed += new FormClosedEventHandler(Form905BankCD2_FormClosed);
                formBankDlg.ShowDialog();
                if (formBankDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 金融機関選択ダイアログクローズ（金融機関ＣＤ2）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form905BankCD2_FormClosed(object sender, FormClosedEventArgs e)
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
                Form905 form = (Form905)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtBankCD2, null);  // エラーを消す

                    // 金融機関支店CDも消す
                    errorProvider1.SetError(txtBranchCD2, null);
                    txtBranchCD2.Text = "";
                    lblBranchName2.Text = "";
                    _busyo.金融機関支店ＣＤ2 = null;

                    Bank bank = form.SelectedBank;
                    txtBankCD2.Text = bank.銀行ＣＤ;
                    _busyo.金融機関ＣＤ2 = bank.銀行ＣＤ;       // 入力内容を確定させる
                    lblBankName2.Text = bank.銀行名;
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
        /// 金融機関支店CD2キー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBranchCD2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnBranchCD2.PerformClick();
            }
        }

        /// <summary>
        /// 金融機関支店ＣＤ2参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBranchCD2_Click(object sender, EventArgs e)
        {
            try
            {
                // 金融機関CD2が未入力の場合は何もしない
                if (string.IsNullOrEmpty(txtBankCD2.Text) == false && string.IsNullOrEmpty(lblBankName2.Text) == false)
                {
                    Control activeControl = this.ActiveControl;
                    // 金融機関支店選択ダイアログを表示
                    Form906 formBranchDlg = new Form906(txtBankCD2.Text, lblBankName2.Text);
                    SubFormOpenInit(formBranchDlg);
                    formBranchDlg.FormClosed += new FormClosedEventHandler(Form906BranchCD2_FormClosed);
                    formBranchDlg.ShowDialog();
                    if (formBranchDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 金融機関支店選択ダイアログクローズ（金融機関支店ＣＤ2）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form906BranchCD2_FormClosed(object sender, FormClosedEventArgs e)
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
                Form906 form = (Form906)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtBranchCD2, null);   // エラーを消す
                    BankBranch bankBranch = form.SelectedBankBranch;
                    txtBranchCD2.Text = bankBranch.支店ＣＤ;
                    _busyo.金融機関支店ＣＤ2 = bankBranch.支店ＣＤ;         // 入力内容を確定させる
                    lblBranchName2.Text = bankBranch.支店名; ;
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
                // （ダイアログが閉じたときに列を移動しているので、ActiveColumnIndexでは判断しない）
                int column = fpSpread1_Sheet1.ActiveColumnIndex;

                if (column == BRANCH_CD_COLUMN ||
                    column == REFERENCE_COLUMN)
                {
                    DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                    BusyoEB busyoEB = new BusyoEB(dr);
                    OpenForm906BySpread(busyoEB);   // 金融機関支店選択ダイアログを表示
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
                    case SET_NAME_COLUMN:       // セット名
                    case BRANCH_CD_COLUMN:      // 支店ＣＤ
                    case BANGO_COLUMN:          // 口座番号
                    case HIKIOTOSHIBI_COLUMN:   // 引落日
                    case ITAKUSHA_CODE_COLUMN:  // 委託者コード
                    case ITAKUSHA_NAME_COLUMN:  // 委託者名
                        colIndex = fpSpread1_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
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
                fpSpread1.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread1Edit_KeyDown);
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
                if (e.Column == REFERENCE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                    BusyoEB busyoEB = new BusyoEB(dr);
                    OpenForm906BySpread(busyoEB);
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
        /// Spreadから金融機関支店選択ダイアログを表示する
        /// </summary>
        /// <param name="BusyoEB"部署EB設定マスタ></param>
        private void OpenForm906BySpread(BusyoEB busyoEB)
        {
            try
            {
                // 銀行が未入力の場合は何もしない
                if (busyoEB.銀行ＣＤ == null)
                {
                    return;
                }

                // 金融機関支店選択ダイアログを表示
                Form906 formBranchDlg = new Form906(busyoEB.銀行ＣＤ, busyoEB.銀行名);
                SubFormOpenInit(formBranchDlg);
                formBranchDlg.FormClosed += new FormClosedEventHandler(Form906Spread_FormClosed);
                formBranchDlg.ShowDialog();
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
        /// 金融機関支店選択ダイアログクローズ（スプレッド）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form906Spread_FormClosed(object sender, FormClosedEventArgs e)
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
                Form906 form = (Form906)sender;
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

                    BankBranch bankBranch = form.SelectedBankBranch;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, BRANCH_CD_COLUMN].Value = bankBranch.支店ＣＤ;   // 支店ＣＤセルに値を設定→dt_ColumnChanged()が呼ばれる
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
        /// ロゴ画像参照ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogoImage_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                openFileDialog1.FilterIndex = _logoImageFilterIndex;
                openFileDialog1.Title = "ファイルを開く（ロゴ画像）";
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;

                    // 画像ファイルチェック
                    if (CheckImage(fileName) == false)
                    {
                        return;
                    }

                    picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                    picLogo.ImageLocation = fileName;
                    picLogo.Load();                             // ピクチャーボックスに表示
                    _busyoImage.SetLogoImage(picLogo.Image);    // ロゴ画像をマスタに設定
                    _logoImageFilterIndex = openFileDialog1.FilterIndex;
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show(this, string.Format("{0}\n画像ファイルではありません。", fileName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// ロゴ画像クリアボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogoClear_Click(object sender, EventArgs e)
        {
            try
            {
                picLogo.Image = null;
                _busyoImage.SetLogoImage(picLogo.Image);    // ロゴ画像をマスタに設定
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
        /// 印影画像参照ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInkanImage_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                openFileDialog1.FilterIndex = _inkanImageFilterIndex;
                openFileDialog1.Title = "ファイルを開く（印鑑画像）";
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;

                    // 画像ファイルチェック
                    if (CheckImage(fileName) == false)
                    {
                        return;
                    }

                    picInkan.SizeMode = PictureBoxSizeMode.Zoom;
                    picInkan.ImageLocation = fileName;
                    picInkan.Load();                            // ピクチャーボックスに表示
                    _busyoImage.SetInkanImage(picInkan.Image);  // 印影画像をマスタに設定
                    _inkanImageFilterIndex = openFileDialog1.FilterIndex;
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show(this, string.Format("{0}\n画像ファイルではありません。", fileName), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 印影画像クリアボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInkanClear_Click(object sender, EventArgs e)
        {
            try
            {
                picInkan.Image = null;
                _busyoImage.SetInkanImage(picInkan.Image);  // 印影画像をマスタに設定
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
        /// 登録ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 部署マスタの入力チェック
                if (CanSaveBusyo() == false)
                {
                    return;
                }

                // 部署EB設定マスタの入力チェック
                if (CanSaveBusyoEB() == false)
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
        /// 削除ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 削除可能かチェック
                if (CanDelete() == false)
                {
                    return;
                }

                if (MessageBox.Show(this, "このデータを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                if (Delete())
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
        /// 終了ボタンクリック
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
        /// EBの金融機関名のコンボボックスを作成
        /// </summary>
        /// <param name="cbo">スプレッドのコンボボックス</param>
        private void SetEBName(FarPoint.Win.Spread.CellType.ComboBoxCellType cbo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     '' AS 銀行ＣＤ," +
                                      "\r\n     '' AS 銀行名," +
                                      "\r\n      0 AS 表示順" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     A.銀行ＣＤ," +
                                      "\r\n     B.銀行名," +
                                      "\r\n     1 AS 表示順" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT DISTINCT" +
                                      "\r\n         銀行ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         EBレコードフォーマット" +
                                      "\r\n     ) AS A " +
                                      "\r\n     INNER JOIN 金融機関名称 AS B ON A.銀行ＣＤ = B.銀行ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     表示順," +
                                      "\r\n     銀行ＣＤ";

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("EB対象銀行");
                    adp.Fill(dt);

                    List<string> itemList = new List<string>();         // 表示用リスト
                    List<string> itemDataList = new List<string>();     // 値用リスト

                    foreach (DataRow dr in dt.Rows)
                    {
                        itemList.Add(dr["銀行名"].ToString());
                        itemDataList.Add(dr["銀行ＣＤ"].ToString());
                    }

                    cbo.Items = itemList.ToArray();
                    cbo.ItemData = itemDataList.ToArray();
                    cbo.EditorValue = FarPoint.Win.Spread.CellType.EditorValue.ItemData;

                    log.Info("EB対象銀行を検索しました。");
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
        /// 部署EB設定マスタ取得
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        private void GetBusyoEB(int? busyoCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.部署ＣＤ AS 部署ＣＤorg" +
                                      "\r\n     ,A.銀行ＣＤ" +
                                      "\r\n     ,A.銀行ＣＤ AS 銀行ＣＤorg" +
                                      "\r\n     ,K.銀行名" +
                                      "\r\n     ,K.銀行名カナ" +
                                      "\r\n     ,A.支店ＣＤ" +
                                      "\r\n     ,A.支店ＣＤ AS 支店ＣＤorg" +
                                      "\r\n     ,KB.支店名" +
                                      "\r\n     ,KB.支店名カナ" +
                                      "\r\n     ,A.委託者コード" +
                                      "\r\n     ,A.委託者名" +
                                      "\r\n     ,A.預金種目" +
                                      "\r\n     ,A.口座番号" +
                                      "\r\n     ,A.引落日" +
                                      "\r\n     ,A.EBセット名" +
                                      "\r\n     ,A.EBセット名 AS EBセット名org" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     部署EB設定マスタ AS A" +
                                      "\r\n     LEFT JOIN 金融機関名称 AS K" +
                                      "\r\n         ON A.銀行ＣＤ = K.銀行ＣＤ" +
                                      "\r\n     LEFT JOIN 金融機関支店名称 AS KB" +
                                      "\r\n         ON A.銀行ＣＤ = KB.銀行ＣＤ" +
                                      "\r\n         AND A.支店ＣＤ = KB.支店ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
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
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "部署EB設定マスタ";

                    log.Info("部署EB設定マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SET_NAME_COLUMN, "EBセット名");
                fpSpread1_Sheet1.BindDataColumn(BANK_CD_COLUMN, "銀行ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BRANCH_CD_COLUMN, "支店ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BRANCH_NAME_COLUMN, "支店名");
                fpSpread1_Sheet1.BindDataColumn(SYUBETSU_COLUMN, "預金種目");
                fpSpread1_Sheet1.BindDataColumn(BANGO_COLUMN, "口座番号");
                fpSpread1_Sheet1.BindDataColumn(HIKIOTOSHIBI_COLUMN, "引落日");
                fpSpread1_Sheet1.BindDataColumn(ITAKUSHA_CODE_COLUMN, "委託者コード");
                fpSpread1_Sheet1.BindDataColumn(ITAKUSHA_NAME_COLUMN, "委託者名");
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

                BusyoEB busyoEB = new BusyoEB(e.Row);

                // 銀行ＣＤの場合は銀行名を取得
                if (e.Column.ColumnName == "銀行ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row.SetColumnError("支店ＣＤ", null);
                    e.Row["支店ＣＤ"] = null;
                    e.Row["支店名"] = null;
                    if (busyoEB.銀行ＣＤ != null)
                    {
                        Bank bank = Bank.GetBank(busyoEB.銀行ＣＤ);
                        if (bank != null)
                        {
                            e.Row["銀行名"] = bank.銀行名;
                        }
                    }
                }

                // 支店ＣＤの場合は支店名を取得
                if (e.Column.ColumnName == "支店ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["支店名"] = null;
                    if (busyoEB.銀行ＣＤ != null && busyoEB.支店ＣＤ != null)
                    {
                        BankBranch bankBranch = BankBranch.GetBankBranch(busyoEB.銀行ＣＤ, busyoEB.支店ＣＤ);
                        if (bankBranch != null)
                        {
                            e.Row["支店名"] = bankBranch.支店名;
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "支店が見つかりません。");
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
        /// 部署画像マスタ取得
        /// </summary>
        /// <param name="busyoCD">部署ＣＤ</param>
        private void GetBusyoImage(int? busyoCD)
        {
            try
            {
                picLogo.Image = null;
                picInkan.Image = null;

                if (busyoCD == null)
                {
                    _busyoImage = new BusyoImage();
                }
                else
                {
                    // 部署画像マスタを取得
                    _busyoImage = BusyoImage.GetBusyoImage((int)busyoCD);

                    // ピクチャーボックスに表示
                    picLogo.SizeMode = PictureBoxSizeMode.Zoom;
                    picLogo.Image = _busyoImage.GetLogoImage();

                    picInkan.SizeMode = PictureBoxSizeMode.Zoom;
                    picInkan.Image = _busyoImage.GetInkanImage();
                }

                // 初期状態として保持
                _busyoImageOrigin = new BusyoImage(_busyoImage);
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
        /// 画像チェック
        /// </summary>
        /// <param name="filePath">画像ファイルパス</param>
        /// <returns>登録可能な画像の場合はtrue。それ以外はfalse</returns>
        private bool CheckImage(string filePath)
        {
            try
            {
                // ファイルサイズをチェック
                FileInfo fi = new FileInfo(filePath);
                if (fi.Length > IMAGE_FILE_SIZE_MAX)
                {
                    MessageBox.Show(this, string.Format("{0}\nファイルサイズが大きすぎます。\n{1:n0}バイト以下の画像ファイルを指定してください。", filePath, IMAGE_FILE_SIZE_MAX), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // イメージのサイズ(面積)をチェック
                int imageHeight = 0;
                int imageWidth = 0;
                using (Image image = Image.FromFile(filePath))
                {
                    imageHeight = image.Height;
                    imageWidth = image.Width;
                }
                if (imageHeight * imageWidth > IMAGE_SIZE_MAX)
                {
                    MessageBox.Show(this, string.Format("{0}\n画像のサイズが大きすぎます。\n面積が{1:n0}ピクセル以下の画像ファイルを指定してください。", filePath, IMAGE_SIZE_MAX), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                return true;
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show(this, string.Format("{0}\n有効な画像ファイルではありません。", filePath), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
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
        /// 部署マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeBusyo()
        {
            if (_busyo.Equals(_origin))
            {
                if (_busyoImage.Equals(_busyoImageOrigin))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 部署EB設定マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeBusyoEB()
        {
            // 部署EB設定マスタを比較
            DataSet ds = (DataSet)bindingSource1.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["部署EB設定マスタ"];

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
        /// 部署マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveBusyo()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_busyo.HasErrors)
                {
                    MessageBox.Show(this, _busyo.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 部署EB設定マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveBusyoEB()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["部署EB設定マスタ"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    BusyoEB busyoEB = new BusyoEB(dr);

                    // エラーがある？
                    if (busyoEB.HasErrors)
                    {
                        dr.SetColumnError("銀行ＣＤ", busyoEB["銀行ＣＤ"]);
                        dr.SetColumnError("支店ＣＤ", busyoEB["支店ＣＤ"]);
                        dr.SetColumnError("預金種目", busyoEB["預金種目"]);
                        dr.SetColumnError("口座番号", busyoEB["口座番号"]);
                        dr.SetColumnError("引落日", busyoEB["引落日"]);
                        dr.SetColumnError("EBセット名", busyoEB["EBセット名"]);
                        dr.SetColumnError("委託者コード", busyoEB["委託者コード"]);
                        dr.SetColumnError("委託者名", busyoEB["委託者名"]);
                        hasError = true;
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "EB口座の入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    // 部署マスタ保存
                    if (SaveBusyo(con, tran) == false)
                    {
                        return false;
                    }

                    // 部署EB設定マスタ保存
                    if (SaveBusyoEB(con, tran) == false)
                    {
                        return false;
                    }

                    // 部署画像マスタ保存
                    _busyoImage.部署ＣＤ = _busyo.部署ＣＤ;
                    if (SaveBusyoImage(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("部署マスタと部署EB設定マスタを{0}しました。", historyProcess.DisplayName());
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
        /// 部署マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBusyo(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _busyo.Insert(con, tran);
                }
                else
                {
                    ret = _busyo.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _busyo.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "部署マスタ", _busyo.Code, historyProcess);
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
        /// 部署EB設定マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBusyoEB(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["部署EB設定マスタ"];

                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    BusyoEB busyoEB = new BusyoEB(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            busyoEB.部署ＣＤ = _busyo.部署ＣＤ;
                            ret = busyoEB.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            break;

                        case DataRowState.Modified:     // 変更
                            ret = busyoEB.Update(con, tran);
                            historyProcess = History.ProcessType.Update;
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = busyoEB.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, busyoEB.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "部署EB設定マスタ", busyoEB.Code, historyProcess);
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
        /// 部署画像マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveBusyoImage(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _busyoImage.Insert(con, tran);
                }
                else
                {
                    ret = _busyoImage.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _busyoImage.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "部署画像マスタ", _busyoImage.Code, historyProcess);
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
                _busyo = Busyo.GetBusyoReload((int)_busyo.部署ＣＤ);

                // 部署グループ、支店ＣＤ、、口座種別（つまりコンボボックス）については、Null を -1（先頭の選択肢） にしておく
                _busyo.部署グループ = _busyo.部署グループ ?? -1;
                _busyo.支店ＣＤ = _busyo.支店ＣＤ ?? -1;
                _busyo.口座種別1 = _busyo.口座種別1 ?? -1;
                _busyo.口座種別2 = _busyo.口座種別2 ?? -1;
                _busyo.請求書タイプ区分 = _busyo.請求書タイプ区分 ?? -1;

                _origin = new Busyo(_busyo);

                _IsNewRecord = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                busyoBindingSource.DataSource = _busyo;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
                // 既存の場合は商品CD変更不可
                txtBusyoCD.Enabled = false;

                btnDelete.Enabled = this.HasAuthEdit;

                // 部署EB設定マスタを取得
                GetBusyoEB(_busyo.部署ＣＤ);

                // 部署画像マスタを取得
                GetBusyoImage(_busyo.部署ＣＤ);
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
        /// 削除可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanDelete()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// DB削除
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Delete()
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 部署マスタ削除
                    if (DeleteBusyo(con, tran) == false)
                    {
                        return false;
                    }

                    // 部署EB設定マスタ削除
                    if (DeleteBusyoEB(con, tran) == false)
                    {
                        return false;
                    }

                    // 部署画像マスタ削除
                    if (DeleteBusyoImage(con, tran) == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("部署マスタと部署EB設定マスタを{0}しました。", historyProcess.DisplayName());
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
        /// 部署マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteBusyo(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _busyo.Delete(con, tran);
                if (ret == false)
                {
                    MessageBox.Show(this, _busyo.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "部署マスタ", _busyo.Code, historyProcess);
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
        /// 部署EB設定マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteBusyoEB(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 全削除
                DataSet ds = (DataSet)bindingSource1.DataSource;
                DataTable dt = ds.Tables["部署EB設定マスタ"];

                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは削除しない
                    {
                        continue;
                    }

                    BusyoEB busyoEB = new BusyoEB(dr);

                    bool ret = busyoEB.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, busyoEB.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "部署EB設定マスタ", busyoEB.Code, historyProcess);
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
        /// 部署画像マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteBusyoImage(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _busyoImage.Delete(con, tran);
                if (ret == false)
                {
                    MessageBox.Show(this, _busyoImage.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "部署画像マスタ", _busyoImage.Code, historyProcess);
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

        #endregion
    }
}

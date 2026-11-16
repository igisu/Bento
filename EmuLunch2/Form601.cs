using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 入金入力
    /// </summary>
    public partial class Form601 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // 請求合計スプレッド
        private const int SEIKYU_TOTAL_DATE_COLUMN           = 0;       // 年月日
        private const int SEIKYU_TOTAL_SEIKYU_COLUMN         = 1;       // 請求額
        private const int SEIKYU_TOTAL_NYUKIN_COLUMN         = 2;       // 入金額
        private const int SEIKYU_TOTAL_ZANDAKA_COLUMN        = 3;       // 残高

        private const string SEIKYU_TOTAL_SEIKYU_COLUMN_NAME = "B";     // 請求額列名
        private const string SEIKYU_TOTAL_NYUKIN_COLUMN_NAME = "C";     // 入金額列名

        // 入金入力スプレッド
        private const int NYUKIN_DATE_COLUMN        = 0;    // 入金日
        private const int NYUKIN_BUSYO_COLUMN       = 1;    // 部署
        private const int NYUKIN_NYUKIN_HOHO_COLUMN = 2;    // 入金方法
        private const int NYUKIN_AMOUNT_COLUMN      = 3;    // 入金額
        private const int NYUKIN_TEKIYO_COLUMN      = 4;    // 摘要
        private const int NYUKIN_IRAININ_COLUMN     = 5;    // 振込依頼人名
        private const int NYUKIN_BIKO_COLUMN        = 6;    // 備考
        private const int NYUKIN_DELETE_COLUMN      = 7;    // 削除

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 入金入力検索条件
        /// </summary>
        private NyukinTarget _nyukinTarget;

        /// <summary>
        /// 入金入力検索条件検索実行時スナップショット
        /// </summary>
        private NyukinTarget _nyukinTargetSnapShot;

        /// <summary>
        /// 入力補助
        /// </summary>
        private NyukinAssist _nyukinAssist;

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

        /// <summary>
        /// 既存の入金データを変更した場合はtrue
        /// </summary>
        private bool _isOldDataChanged = false;

        /// <summary>
        /// 今回合計入金額の初期値
        /// </summary>
        private decimal _initialTotalAmount = 0;

        /// <summary>
        /// 表示ボタン処理中フラグ（画面クリア処理をしないフラグ）
        /// </summary>
        private bool _stopClear = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nyukinTarget"></param>
        public Form601(NyukinTarget nyukinTarget)
        {
            InitializeComponent();
            _nyukinTarget = nyukinTarget;
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
        private void Form601_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.coins_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.SetNyukinHoho(cboNyukinHoho);
                // 振込依頼人コンボは得意先が決定してから生成

                // スプレッドは検索が実施されるまで非活性
                fpSpread_SeikyuTotal.Enabled = false;
                fpSpread_Nyukin.Enabled = false;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_Nyukin.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_Nyukin.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_Nyukin.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 入金入力スプレッドの部署、入金方法、摘要のコンボボックス列を作成
                // 振込依頼人名のコンボボックス列は得意先が決定してから生成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbBusyo = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetBusyo(cbBusyo);
                cbBusyo.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbBusyo.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread_Nyukin_Sheet1.Columns[NYUKIN_BUSYO_COLUMN].CellType = cbBusyo;

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbNyukin = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetNyukinHoho(cbNyukin);
                cbNyukin.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbNyukin.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread_Nyukin_Sheet1.Columns[NYUKIN_NYUKIN_HOHO_COLUMN].CellType = cbNyukin;

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbTekiyo = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.Set(cbTekiyo,
                    new string[] { "", "1月分", "2月分", "3月分", "4月分", "5月分", "6月分", "7月分", "8月分", "9月分", "10月分", "11月分", "12月分" },
                    new int[] { -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
                cbTekiyo.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbTekiyo.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread_Nyukin_Sheet1.Columns[NYUKIN_TEKIYO_COLUMN].CellType = cbTekiyo;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                nyukinTargetBindingSource.DataSource = _nyukinTarget;

                _nyukinAssist = new NyukinAssist();
                nyukinAssistBindingSource.DataSource = _nyukinAssist;

                // 渡された入金入力検索条件に得意先CDが有る場合に検索する
                if (_nyukinTarget.得意先ＣＤ != null)
                {
                    // 検索ボタン押下
                    btnQuery.PerformClick();
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
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form601_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    btnClear.PerformClick();        // クリアボタン押下
                    break;

                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F9:
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_Nyukin.EditMode == true)
                    {
                        fpSpread_Nyukin.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }
                    btnSave.PerformClick();         // 更新ボタン押下
                    break;

                case Keys.F12:
                    btnQuit.Focus();
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
            }
        }

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form601_FormClosing(object sender, FormClosingEventArgs e)
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
                if (fpSpread_Nyukin.EditMode == true)
                {
                    fpSpread_Nyukin.StopCellEditing();
                    Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                }
                else
                {
                    WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
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
        /// 得意先ＣＤ 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // 検索条件が変わったら画面をクリア
                if (_nyukinTargetSnapShot != null && _nyukinTarget.Equals(_nyukinTargetSnapShot) == false)
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
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCDFrom_FormClosed);
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCDFrom_FormClosed(object sender, FormClosedEventArgs e)
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
                    _nyukinTarget.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;

                    // 以下、NyukinTargetによって設定された内容を画面に反映する
                    ReadBinding(lblShimeKubun);
                    ReadBinding(lblShimeYoubi);
                    ReadBinding(lblShimebi);
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
        /// 得意先ＣＤ参照（振込依頼人で探す）ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIrainin_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択（振込依頼人）ダイアログを表示
                Form900F formIraininDlg = new Form900F();
                SubFormOpenInit(formIraininDlg);
                formIraininDlg.FormClosed += new FormClosedEventHandler(Form900FIrainin_FormClosed);
                formIraininDlg.ShowDialog();
                if (formIraininDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 得意先選択（振込依頼人）ダイアログクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900FIrainin_FormClosed(object sender, FormClosedEventArgs e)
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
                Form900F form = (Form900F)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    txtTokuisakiCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _nyukinTarget.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;

                    // 以下、NyukinTargetによって設定された内容を画面に反映する
                    ReadBinding(lblShimeKubun);
                    ReadBinding(lblShimeYoubi);
                    ReadBinding(lblShimebi);
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
                _nyukinTarget.期間開始日 = null;
                ReadBinding(lblKikan);        // コントロールに反映

                // 検索条件の入力にエラーがある？
                if (_nyukinTarget.HasErrors)
                {
                    if (string.IsNullOrEmpty(_nyukinTarget.Error) == false)
                    {
                        MessageBox.Show(this, _nyukinTarget.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                _stopClear = true;

                // 表示期間（○ヶ月）前の締日を算出
                // 以前は翌月の締日も算出していたが、未来は無制限に取得するように改めたため検索条件としては未使用となった
                DateTime wkDate = DateTime.Today;
                wkDate = wkDate.AddMonths((int)_nyukinTarget.表示期間 * -1);
                DateTime fromDate = wkDate;
                DateTime toDate = DateTime.Today;
                toDate = toDate.AddMonths(1);
                switch (_nyukinTarget.締区分 ?? 0)
                {
                    case (int)Consts.ShimeKubun.Daily:      // 日締
                        // システム日付の表示期間（○ヶ月）前～システム日付の1ヶ月後
                        // つまりどちらも上で求めたそのまま
                        fromDate = wkDate;
                        break;

                    case (int)Consts.ShimeKubun.Weekly:     // 週締
                        // システム日付の表示期間（○ヶ月）前の直後の締曜日～システム日付の1ヶ月後の直後の締曜日
                        // 曜日の値が0:日曜日～6:土曜日の場合、
                        // 締曜日≧対象日：締曜日－対象日
                        // 締曜日＜対象日：７－（対象日－締曜日）
                        // 上記の値を対象日に加算すると、直後の締曜日の日付が求まる
                        fromDate = wkDate;
                        int gap = 0;
                        if ((int)_nyukinTarget.週締曜日 >= (int)fromDate.DayOfWeek)
                        {
                            gap = (int)_nyukinTarget.週締曜日 - (int)fromDate.DayOfWeek;
                        }
                        else
                        {
                            gap = 7 - ((int)fromDate.DayOfWeek - (int)_nyukinTarget.週締曜日);
                        }
                        fromDate = fromDate.AddDays(gap);

                        if ((int)_nyukinTarget.週締曜日 >= (int)toDate.DayOfWeek)
                        {
                            gap = (int)_nyukinTarget.週締曜日 - (int)toDate.DayOfWeek;
                        }
                        else
                        {
                            gap = 7 - ((int)toDate.DayOfWeek - (int)_nyukinTarget.週締曜日);
                        }
                        toDate = toDate.AddDays(gap);

                        break;

                    case (int)Consts.ShimeKubun.Monthly:    // 月締
                        if (_nyukinTarget.月締日 == 99)
                        {
                            // 締日が99の場合は、締日を末日とする
                            fromDate = new DateTime(wkDate.Year, wkDate.Month, DateTime.DaysInMonth(wkDate.Year, wkDate.Month));
                            toDate = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));
                        }
                        else
                        {
                            fromDate = new DateTime(wkDate.Year, wkDate.Month, (int)_nyukinTarget.月締日);
                            toDate = new DateTime(toDate.Year, toDate.Month, (int)_nyukinTarget.月締日);
                        }
                        break;
                }

                _nyukinTarget.期間開始日 = fromDate;
                ReadBinding(lblKikan);        // コントロールに反映

                // 入力補助の部署のデフォルト値を設定（得意先マスタの部署）
                Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_nyukinTarget.得意先ＣＤ);
                _nyukinAssist.部署ＣＤ = tokuisaki.部署ＣＤ;
                // 画面にも反映
                ReadBinding(cboBusyo);

                // 入力補助の入金方法のデフォルト値を設定（得意先マスタの支払方法）
                _nyukinAssist.入金方法 = tokuisaki.支払方法;
                // 画面にも反映
                ReadBinding(cboNyukinHoho);

                // 振込依頼人名コンボを生成
                SetIraininCombo();

                // 検索したので活性化
                fpSpread_SeikyuTotal.Enabled = true;
                fpSpread_Nyukin.Enabled = true;

                // 入金明細テーブル取得
                GetNyukinMeisai(fromDate);

                // 請求合計テーブル取得
                DateTime fromDateWithSight = fromDate.AddMonths((_nyukinTarget.支払サイト ?? 0) * (-1)); // 支払いサイトの月数を遡る
                GetSeikyuTotal(fromDateWithSight, toDate);

                // 入金額を請求合計Spreadに反映
                UpdateSeikyuTotalNyukin();

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _nyukinTargetSnapShot = new NyukinTarget(_nyukinTarget);

                _isOldDataChanged = false;      // 既存データ変更フラグをリセット
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
                _stopClear = false;
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 入金入力Spreadプレビューキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Nyukin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // 非編集状態からの日本語入力動作が不正になるのを回避する
            AvoidEnterJapaneseFirstKeyConfirm(fpSpread_Nyukin, NYUKIN_BIKO_COLUMN, e);
        }

        /// <summary>
        /// 入金入力Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Nyukin_KeyDown(object sender, KeyEventArgs e)
        {
            // F2？
            if (e.KeyCode == Keys.F2)
            {
                _isF2forSpreadEdit = true;      // 編集モードに移行するF2キーが押下された

                // ↓キー押下されたことにするが、あまり意味が無い
                //e.SuppressKeyPress = true;
                //e.Handled = true;

                // 非編集モードから編集モードへの移行なので、常にelseが成立
                if (fpSpread_Nyukin.EditMode)
                {
                    fpSpread_Nyukin.CancelCellEditing();
                }
                else
                {
                    fpSpread_Nyukin.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Nyukin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Nyukin_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Nyukin_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= NYUKIN_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Nyukin_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Nyukin_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Nyukin_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Nyukin.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_Nyukin_Sheet1.ActiveColumnIndex)
                {
                    case NYUKIN_DATE_COLUMN:        // 入金日
                    case NYUKIN_AMOUNT_COLUMN:      // 入金額
                    case NYUKIN_BIKO_COLUMN:        // 備考
                        colIndex = fpSpread_Nyukin_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_Nyukin_Sheet1.Cells[fpSpread_Nyukin_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                    fpSpread_Nyukin.Refresh();      // 自動設定した値が表示されない場合があるので強制的に再描画
                    UpdateSeikyuTotalNyukin();      // fpSpread_Nyukin_Changeイベントが呼ばれないので、UpdateSeikyuTotalNyukin()はここで呼ぶ
                }
            }
        }

        /// <summary>
        /// 入金入力Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Nyukin_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_Nyukin.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Nyukin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_Nyukin.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_NyukinEdit_KeyDown);
            }

            if (fpSpread_Nyukin.EditingControl is FarPoint.Win.FpCombo)
            {
                // コンボボックスの場合は、Enterキーでのドロップダウンを制御する
                _fpCombo = (FarPoint.Win.FpCombo)fpSpread_Nyukin.EditingControl;
                _fpCombo.KeyDown += new KeyEventHandler(FpCombo_KeyDown);
                _fpCombo.DropDown += new FarPoint.Win.DropDownEventHandler(FpCombo_DropDown);
            }
        }

        /// <summary>
        /// 入金入力Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Nyukin_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_Nyukin.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_Nyukin, fpSpread_Nyukin_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_Nyukin.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_NyukinEdit_KeyDown);
            }

            if (fpSpread_Nyukin.EditingControl is FarPoint.Win.FpCombo)
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
        private void fpSpread_NyukinEdit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_Nyukin.EditMode)
                    {
                        fpSpread_Nyukin.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Nyukin.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Nyukin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_Nyukin_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_Nyukin_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= NYUKIN_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_Nyukin_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_Nyukin_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_Nyukin_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_Nyukin.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
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
                    if (fpSpread_Nyukin.EditMode)
                    {
                        fpSpread_Nyukin.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_Nyukin.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_Nyukin.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
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
        /// 入金入力Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Nyukin_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 削除ボタン？
                if (e.Column == NYUKIN_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_Nyukin_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)nyukinBindingSource.Current).Row;

                    // 既存のデータを変更？
                    if (dr.RowState == DataRowState.Unchanged)
                    {
                        _isOldDataChanged = IsOldDataChanged(dr);
                    }

                    dr.Delete();

                    UpdateSeikyuTotalNyukin();  // 入金額を請求合計Spreadに反映
                    CalcTotal(false);           // 合計値を算出
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
        /// 入金入力Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Nyukin_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            try
            {
                DataSet ds = (DataSet)nyukinBindingSource.DataSource;
                if (ds == null) return;

                // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
                DataRow dr = ((DataRowView)nyukinBindingSource.Current).Row;
                dr.EndEdit();

                fpSpread_Nyukin.Refresh();      // 自動設定した値が表示されない場合があるので強制的に再描画

                // 入金日or入金額の場合は請求合計Spreadに反映
                if (e.Column == NYUKIN_DATE_COLUMN || e.Column == NYUKIN_AMOUNT_COLUMN)
                {
                    UpdateSeikyuTotalNyukin();
                }

                // 追加された行 かつ 最終行 かつ 得意先マスタ.振込手数料フラグ（負担させない）がON かつ 入金方法が振込 かつ 入金日と入金額が入力済みであれば、手数料の行を追加する
                if (dr.RowState == DataRowState.Added &&
                    fpSpread_Nyukin_Sheet1.ActiveRowIndex == fpSpread_Nyukin_Sheet1.Rows.Count - 2 &&       // Count - 1 は実際に表示されている新規行
                    (_nyukinTarget.振込手数料フラグ ?? false) == true &&
                    (dr["入金方法"] != null && dr["入金方法"] != DBNull.Value && (int)dr["入金方法"] == (int)Consts.NyukinHoho.Furikomi) &&
                    (dr["入金日"] != null && dr["入金日"] != DBNull.Value) &&
                    (dr["入金額"] != null && dr["入金額"] != DBNull.Value))
                {
                    AddTesuryo();                   // 手数料を追加
                    UpdateSeikyuTotalNyukin();      // 入金額(=手数料)を請求合計Spreadに反映
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
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 得意先CDをクリア（得意先名～月締日もクリアされる）
                // ※newすれば手っ取り早いが、表示期間は維持したいので
                _nyukinTarget.得意先ＣＤ = null;
                ReadBinding(txtTokuisakiCD);        // コントロールに反映
                ReadBinding(lblTokuisakiName);
                ReadBinding(lblShimeKubun);
                ReadBinding(lblShimeYoubi);
                ReadBinding(lblShimebi);

                // 画面クリア
                Clear();

                txtTokuisakiCD.Focus();
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
        /// 合計値を算出
        /// </summary>
        /// <param name="isInit">初回合計値を算出する場合はtrue</param>
        private void CalcTotal(bool isInit)
        {
            try
            {
                decimal amount = 0;
                for (int rowIndex = 0; rowIndex < fpSpread_Nyukin_Sheet1.Rows.Count; rowIndex++)
                {
                    amount += ((decimal?)fpSpread_Nyukin_Sheet1.Cells[rowIndex, NYUKIN_AMOUNT_COLUMN].Value) ?? 0;
                }
                if (isInit)
                {
                    _initialTotalAmount = amount;       // 今回合計入金額の初期値
                    amount = 0;
                }
                else
                {
                    amount -= _initialTotalAmount;      // 今回入金した金額(=初期値との差額)を求める
                }
                lblNyukinTotal.Text = amount.ToString("N0");
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
        /// 振込依頼人コンボ（フォーム・スプレッド）を生成
        /// </summary>
        private void SetIraininCombo()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     -1 AS 依頼人ＣＤ" +
                                      "\r\n    ,'' AS 振込依頼人名" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     依頼人ＣＤ" +
                                      "\r\n     ,振込依頼人名" +
                                      "\r\n FROM" +
                                      "\r\n     得意先振込依頼人名" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     依頼人ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _nyukinTarget.得意先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("振込依頼人名");
                    adp.Fill(dt);

                    log.Info("振込依頼人名を検索しました。");
                }

                string[] displayMember = new string[dt.Rows.Count];
                int[] valueMember = new int[dt.Rows.Count];
                for (int i = 0 ; i < dt.Rows.Count ; i++)
                {
                    displayMember[i] = dt.Rows[i]["振込依頼人名"] as string;
                    valueMember[i] = (int)dt.Rows[i]["依頼人ＣＤ"];
                }

                // 入力補助の振込依頼人コンボ
                ComboUtil.Set(cboIrainin, displayMember, valueMember);
                if (dt.Rows.Count >= 2)
                {
                    // 未選択以外の選択肢があればそれを選択状態にする
                    _nyukinAssist.依頼人ＣＤ = (int)dt.Rows[1]["依頼人ＣＤ"];
                    ReadBinding(cboIrainin);        // コントロールに反映
                }

                // スプレッドの振込依頼人コンボ
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbIrainin = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.Set(cbIrainin, displayMember, valueMember);
                cbIrainin.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbIrainin.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread_Nyukin_Sheet1.Columns[NYUKIN_IRAININ_COLUMN].CellType = cbIrainin;
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
        /// 入金明細テーブル取得
        /// </summary>
        /// <param name="fromDate">表示期間（○ヶ月）前の締日の日付</param>
        private void GetNyukinMeisai(DateTime fromDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.入金番号" +
                                      "\r\n     ,A.入金入力区分" +
                                      "\r\n     ,A.入金日" +
                                      "\r\n     ,A.入金日 AS 入金日org" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.得意先名" +
                                      "\r\n     ,A.依頼人ＣＤ" +
                                      "\r\n     ,A.振込依頼人名" +
                                      "\r\n     ,A.入金方法" +
                                      "\r\n     ,VN.入金方法名" +
                                      "\r\n     ,ISNULL(A.入金額, 0) AS 入金額" +
                                      "\r\n     ,A.入金摘要区分" +
                                      "\r\n     ,CONVERT(varchar, A.入金摘要区分) + '月分' AS 摘要" +
                                      "\r\n     ,A.対応請求番号" +
                                      "\r\n     ,A.TP決済番号" +
                                      "\r\n     ,A.TP取消決済番号" +
                                      "\r\n     ,A.備考" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     入金明細 AS A" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n     LEFT JOIN V_入金方法 AS VN ON VN.入金方法CD = A.入金方法" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND A.入金日 > @入金日From " +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.入金日";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _nyukinTarget.得意先ＣＤ;
                    cmd.Parameters.Add("@入金日From", SqlDbType.DateTime).Value = fromDate;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("入金明細");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.TableNewRow += new DataTableNewRowEventHandler(nyukin_dt_TableNewRow);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(nyukin_dt_ColumnChanged);

                    nyukinBindingSource.DataSource = ds;
                    nyukinBindingSource.DataMember = "入金明細";

                    log.Info("入金明細を検索しました。");
                }

                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_DATE_COLUMN,        "入金日");
                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_BUSYO_COLUMN,       "部署ＣＤ");
                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_NYUKIN_HOHO_COLUMN, "入金方法");
                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_AMOUNT_COLUMN,      "入金額");
                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_TEKIYO_COLUMN,      "入金摘要区分");
                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_IRAININ_COLUMN,     "依頼人ＣＤ");
                fpSpread_Nyukin_Sheet1.BindDataColumn(NYUKIN_BIKO_COLUMN,        "備考");

                // 入金額の合計(初期値)をラベルに表示する
                CalcTotal(true);

                fpSpread_Nyukin.Focus();
                if (fpSpread_Nyukin_Sheet1.Rows.Count != 0)
                {
                    int rowIndex = fpSpread_Nyukin_Sheet1.Rows.Count - 1;
                    fpSpread_Nyukin_Sheet1.ActiveRowIndex = rowIndex;

                    // 表示できる行数を算出
                    int visibleRowCount = (int)Math.Floor(fpSpread_Nyukin.Height / fpSpread_Nyukin_Sheet1.RowHeader.Rows[0].Height);
                    fpSpread_Nyukin.SetViewportTopRow(0, fpSpread_Nyukin_Sheet1.Rows.Count - visibleRowCount + 2);   // 先頭行のインデックスを指定（そこにスクロール）確実に表示するために２行加算している

                    fpSpread_Nyukin_Sheet1.AddSelection(rowIndex, 0, 1, 1); // 最終行を選択
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
        /// 入金明細テーブル行追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nyukin_dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            try
            {
                log.Debug("nyukin_dt_TableNewRow START");

                // ここで設定した値ではなく、入力した内容が優先される
                if (_nyukinAssist.入金日 == null)
                {
                    e.Row["入金日"] = DBNull.Value;
                }
                else
                {
                    e.Row["入金日"] = _nyukinAssist.入金日;
                }

                if (_nyukinAssist.部署ＣＤ == null)
                {
                    e.Row["部署ＣＤ"] = DBNull.Value;
                }
                else
                {
                    e.Row["部署ＣＤ"] = _nyukinAssist.部署ＣＤ;
                }

                if (_nyukinAssist.入金方法 == null)
                {
                    e.Row["入金方法"] = DBNull.Value;
                }
                else
                {
                    e.Row["入金方法"] = _nyukinAssist.入金方法;
                }

                if (_nyukinAssist.依頼人ＣＤ == null)
                {
                    e.Row["依頼人ＣＤ"] = DBNull.Value;
                }
                else
                {
                    e.Row["依頼人ＣＤ"] = _nyukinAssist.依頼人ＣＤ;
                }

                log.Debug("nyukin_dt_TableNewRow END");
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
        /// 入金テーブルデータソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nyukin_dt_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            // イベントの再帰呼び出しを回避する
            if (_isDtColumnChanging)
            {
                return;
            }

            try
            {
                log.Debug("nyukin_dt_ColumnChanged START - " + e.Column.ColumnName);

                _isDtColumnChanging = true;

                // 既存のデータを変更？
                if (e.Row.RowState == DataRowState.Unchanged)
                {
                    _isOldDataChanged = IsOldDataChanged(e.Row);
                }

                // 依頼人ＣＤの場合
                if (e.Column.ColumnName == "依頼人ＣＤ")
                {
                    // 依頼人ＣＤに該当する振込依頼人名を取得・反映
                    FarPoint.Win.Spread.CellType.ComboBoxCellType cbIrainin =
                        (FarPoint.Win.Spread.CellType.ComboBoxCellType)fpSpread_Nyukin_Sheet1.Columns[NYUKIN_IRAININ_COLUMN].CellType;
                    int index = Array.IndexOf(cbIrainin.ItemData, e.Row["依頼人ＣＤ"].ToString());   // cbIrainin.ItemDataにはstringが格納されていることに注意
                    if (0 <= index)
                    {
                        e.Row["振込依頼人名"] = cbIrainin.Items[index];
                    }
                }

                // 金額の場合
                if (e.Column.ColumnName == "入金額")
                {
                    CalcTotal(false);           // 合計を算出
                }

                log.Debug("nyukin_dt_ColumnChanged END");
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
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 請求合計テーブル取得
        /// </summary>
        /// <param name="fromDate">表示期間（○ヶ月）前の締日の日付</param>
        /// <param name="toDate">翌月の締日の日付</param>
        private void GetSeikyuTotal(DateTime fromDate, DateTime toDate)
        {
            //************************************************************************************************************************
            // 入金入力Spreadに入力された金額の合計を請求合計Spreadの入金額に反映するため、ここでは入金額を0として取得しておき、
            // 請求年月日From～Toの範囲における入金入力Spread(=入金テーブル)の入金額の合計値を計算して請求合計Spreadの入金額に反映する
            //************************************************************************************************************************
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.請求番号" +
                                      "\r\n     ,A.請求日 AS 請求日From" +
                                      "\r\n     ,(SELECT" +
                                      "\r\n         ISNULL(MIN(B.請求日), @請求日To)" +
                                      "\r\n     FROM" +
                                      "\r\n         請求合計 AS B" +
                                      "\r\n     WHERE" +
                                      "\r\n         B.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n         AND B.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n         AND B.請求先ＣＤ = A.請求先ＣＤ" +
                                      "\r\n         AND A.請求日 < B.請求日" +
                                      "\r\n     ) AS 請求日To" +
                                      "\r\n     ,A.締区分" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.請求先ＣＤ" +
                                      "\r\n     ,A.請求先名" +
                                      "\r\n     ,A.前回請求額" +
                                      "\r\n     ,cast(0 AS decimal) AS 入金額" +
                                      "\r\n     ,A.繰越額" +
                                      "\r\n     ,A.買上金額" +
                                      "\r\n     ,A.調整額" +
                                      "\r\n     ,A.消費税" +
                                      "\r\n     ,A.今回請求額" +
                                      "\r\n     ,A.回収予定日" +
                                      "\r\n     ,A.印刷者ＣＤ" +
                                      "\r\n     ,A.印刷日時" +
                                      "\r\n FROM" +
                                      "\r\n     請求合計 AS A" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.請求日 > @請求日From" +
                                      "\r\n     AND A.請求先ＣＤ = @請求先ＣＤ" +
                                      "\r\n UNION" +
                                      // -- 電子決済請求合計からも取得する
                                      "\r\n SELECT" +
                                      "\r\n     A.請求番号" +
                                      "\r\n     ,A.請求日 AS 請求日From" +
                                      "\r\n     ,(SELECT" +
                                      "\r\n         ISNULL(MIN(B.請求日), @請求日To)" +
                                      "\r\n     FROM" +
                                      "\r\n         電子決済請求合計 AS B" +
                                      "\r\n     WHERE" +
                                      "\r\n         B.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n         AND B.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n         AND B.請求先ＣＤ = A.請求先ＣＤ" +
                                      "\r\n         AND A.請求日 < B.請求日" +
                                      "\r\n     ) AS 請求日To" +
                                      "\r\n     ,A.締区分" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.請求先ＣＤ" +
                                      "\r\n     ,A.請求先名" +
                                      "\r\n     ,0 AS 前回請求額" +
                                      "\r\n     ,cast(0 AS decimal) AS 入金額" +
                                      "\r\n     ,0 AS 繰越額" +
                                      "\r\n     ,A.買上金額" +
                                      "\r\n     ,A.調整額" +
                                      "\r\n     ,0 AS 消費税" +
                                      "\r\n     ,A.今回請求額" +
                                      "\r\n     ,A.回収予定日" +
                                      "\r\n     ,A.印刷者ＣＤ" +
                                      "\r\n     ,A.印刷日時" +
                                      "\r\n FROM" +
                                      "\r\n     電子決済請求合計 AS A" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.請求日 > @請求日From" +
                                      "\r\n     AND A.請求先ＣＤ = @請求先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     請求日 DESC";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求日From", SqlDbType.DateTime).Value = fromDate;
                    cmd.Parameters.Add("@請求日To", SqlDbType.DateTime).Value = toDate;
                    cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _nyukinTarget.得意先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("請求合計");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    seikyuTotalBindingSource.DataSource = ds;
                    seikyuTotalBindingSource.DataMember = "請求合計";

                    log.Info("請求合計を検索しました。");
                }

                fpSpread_SeikyuTotal_Sheet1.BindDataColumn(SEIKYU_TOTAL_DATE_COLUMN,    "請求日From");
                fpSpread_SeikyuTotal_Sheet1.BindDataColumn(SEIKYU_TOTAL_SEIKYU_COLUMN,  "今回請求額");
                fpSpread_SeikyuTotal_Sheet1.BindDataColumn(SEIKYU_TOTAL_NYUKIN_COLUMN,  "入金額");

                // 残高の数式を設定する
                for (int row = 0; row < fpSpread_SeikyuTotal_Sheet1.Rows.Count; row++)
                {
                    fpSpread_SeikyuTotal_Sheet1.SetFormula(row, SEIKYU_TOTAL_ZANDAKA_COLUMN,
                                                            string.Format("{0}{2}-{1}{2}",
                                                            SEIKYU_TOTAL_SEIKYU_COLUMN_NAME,
                                                            SEIKYU_TOTAL_NYUKIN_COLUMN_NAME,
                                                            row + 1));
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
        /// 入金入力Spreadに入力した入金額を請求合計Spreadの入金額に反映する
        /// </summary>
        private void UpdateSeikyuTotalNyukin()
        {
            try
            {
                log.Debug("UpdateSeikyuTotalNyukin START");

                DataSet nyukinDs = (DataSet)nyukinBindingSource.DataSource;
                DataTable nyukinDt = nyukinDs.Tables["入金明細"];

                DataSet ds = (DataSet)seikyuTotalBindingSource.DataSource;
                DataTable dt = ds.Tables["請求合計"];

                foreach (DataRow dr in dt.Rows)
                {
                    object value = nyukinDt.Compute("SUM(入金額)",
                                                    string.Format("入金日 > #{0:yyyy/MM/dd}# and 入金日 <= #{1:yyyy/MM/dd}# and 部署ＣＤ = {2}", dr["請求日From"], dr["請求日To"], dr["部署ＣＤ"]));
                    if (value != null && value != DBNull.Value)
                    {
                        // 値が異なる場合のみ反映
                        if ((decimal)dr["入金額"] != (decimal)value)
                        {
                            dr["入金額"] = value;
                        }
                    }
                    else
                    {
                        dr["入金額"] = 0m;
                    }
                }

                log.Debug("UpdateSeikyuTotalNyukin END");
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
        /// 手数料を追加する
        /// </summary>
        private void AddTesuryo()
        {
            try
            {
                log.Debug("AddTesuryo START");

                DataRow dr = ((DataRowView)nyukinBindingSource.Current).Row;
                DateTime nyukinDate = (DateTime)dr["入金日"];

                // 入金日に該当する請求合計レコードを取得
                DataSet ds = (DataSet)seikyuTotalBindingSource.DataSource;
                DataTable dt = ds.Tables["請求合計"];
                DataRow[] drs = dt.Select(string.Format("請求日From < #{0:yyyy/MM/dd}# and 請求日To >= #{0:yyyy/MM/dd}#", nyukinDate));
                if (drs != null && drs.Length != 0)
                {
                    // 請求合計レコードの請求額と、入金額の差が1000未満であれば、それを手数料として追加する
                    decimal seikyuZandaka = (drs[0]["今回請求額"] as decimal?) ?? 0m;
                    decimal nyukinAmount = (drs[0]["入金額"] as decimal?) ?? 0m;
                    decimal tesuryo = seikyuZandaka - nyukinAmount;
                    if (tesuryo < 1000)
                    {
                        if (tesuryo < 0) tesuryo = 0;   // マイナスの手数料は０にする

                        // 手数料の行を追加（この時点で nyukin_dt_TableNewRow が呼ばれている）
                        fpSpread_Nyukin_Sheet1.AddUnboundRows(fpSpread_Nyukin_Sheet1.RowCount - 1, 1);
                        fpSpread_Nyukin_Sheet1.SetValue(fpSpread_Nyukin_Sheet1.RowCount - 2, NYUKIN_NYUKIN_HOHO_COLUMN, (int)Consts.NyukinHoho.Tesuryo); // 入金方法=手数料
                        fpSpread_Nyukin_Sheet1.SetValue(fpSpread_Nyukin_Sheet1.RowCount - 2, NYUKIN_AMOUNT_COLUMN, tesuryo);
                        fpSpread_Nyukin_Sheet1.AddRowToDataSource(fpSpread_Nyukin_Sheet1.RowCount - 2, true);

                        log.Debug("AddTesuryo 手数料 = " + tesuryo.ToString());
                    }
                }

                log.Debug("AddTesuryo END");
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
        /// 既存の入金データを変更したかどうかを返します。
        /// </summary>
        /// <param name="row">チェック対象行</param>
        /// <returns>入金日が先月の場合はTrue</returns>
        private bool IsOldDataChanged(DataRow row)
        {
            bool isOldDataChanged = false;
            if (row.IsNull("入金日org") == false)
            {
                DateTime nyukinDate = (DateTime)row["入金日org"];

                // 201609 < 201610 みたいな感じで比較
                int nyukinYM = (nyukinDate.Year * 100) + (nyukinDate.Month);
                int currentYM = (DateTime.Today.Year * 100) + (DateTime.Today.Month);
                if (nyukinYM < currentYM)
                {
                    isOldDataChanged = true;
                }
            }
            return isOldDataChanged;
        }

        /// <summary>
        /// 入金入力が変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChange()
        {
            // 入金明細テーブルを取得
            DataSet ds = (DataSet)nyukinBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["入金明細"];

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
        /// 入金明細テーブルが登録可能か？
        /// </summary>
        /// <returns></returns>
        private bool CanSave()
        {
            bool hasError = false;
            bool overTesuryo = false;
            bool isFutureDate = false;

            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_nyukinTargetSnapShot == null)
                {
                    MessageBox.Show(this, "入金対象が検索されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_nyukinTarget.Equals(_nyukinTargetSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                DataSet ds = (DataSet)nyukinBindingSource.DataSource;
                if (ds == null)
                {
                    MessageBox.Show(this, "入金対象が検索されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                DataTable dt = ds.Tables["入金明細"];

                // 入金日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());

                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    Nyukin nyukin = new Nyukin(dr);

                    // 追加したレコードの場合はダミーの値を格納しておく
                    if (nyukin.得意先ＣＤ == null || nyukin.入金番号 == null || nyukin.入金入力区分 == null)
                    {
                        nyukin.得意先ＣＤ = _nyukinTarget.得意先ＣＤ;
                        nyukin.入金番号 = 0;
                        nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.PC;
                    }
                    if (nyukin.HasErrors)
                    {
                        dr.SetColumnError("入金日", nyukin["入金日"]);
                        dr.SetColumnError("部署ＣＤ", nyukin["部署ＣＤ"]);
                        dr.SetColumnError("入金方法", nyukin["入金方法"]);
                        hasError = true;
                    }
                    else
                    {
                        // 入金日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                        if (nyukin.入金日 > warnDate)
                        {
                            isFutureDate = true;
                        }
                    }

                    // 手数料が1000円を超えている場合はメッセージを表示する
                    if (nyukin.入金方法 == (int)Consts.NyukinHoho.Tesuryo && nyukin.入金額 >= 1000)
                    {
                        overTesuryo = true;
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (isFutureDate)
                {
                    if (MessageBox.Show(this, "入金日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return false;
                    }
                }

                if (overTesuryo)
                {
                    if (MessageBox.Show(this, "手数料が1000円を超えています。よろしいですか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    {
                        return false;
                    }
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

            if (_isOldDataChanged)
            {
                if (MessageBox.Show(this, "過去データが編集されています。編集後、請求締め処理を再度実行する必要があります。\n処理を実行してよろしいですか？",
                                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return false;
                }
            }

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                // トランザクション開始
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 削除された入金明細も取得するため、入金明細をループし、該当する請求合計をその都度取得する
                    DataSet nyukinDs = (DataSet)nyukinBindingSource.DataSource;
                    DataTable nyukinDt = nyukinDs.Tables["入金明細"];

                    DataSet ds = (DataSet)seikyuTotalBindingSource.DataSource;
                    DataTable dt = ds.Tables["請求合計"];

                    foreach (DataRow nyukinDr in nyukinDt.Rows)
                    {
                        Nyukin nyukin = new Nyukin(nyukinDr);
                        SeikyuGokei seikyuGokei = new SeikyuGokei();

                        DataRow[] drs = dt.Select(string.Format("請求日From < #{0:yyyy/MM/dd}# and #{0:yyyy/MM/dd}# <= 請求日To", nyukin.入金日));
                        if (drs != null && drs.Length != 0)
                        {
                            // 請求合計クラスに詰める
                            seikyuGokei.請求番号 = (decimal)drs[0]["請求番号"];
                            seikyuGokei.請求先ＣＤ = (int)drs[0]["請求先ＣＤ"];
                            seikyuGokei.請求先名 = (string)drs[0]["請求先名"];
                        }

                        // 入金明細テーブル保存
                        if (SaveNyukin(con, tran, nyukinDr, seikyuGokei) == false)
                        {
                            return false;
                        }
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("入金明細テーブルを{0}しました。", historyProcess.DisplayName());
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
        /// 入金テーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="nyukinDr">入金レコード</param>
        /// <param name="seikyuTotal">請求合計クラス</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveNyukin(SqlConnection con, SqlTransaction tran, DataRow nyukinDr, SeikyuGokei seikyuGokei)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;
                decimal newNyukinNo = -1;       // 追加用入金番号

                Nyukin nyukin = new Nyukin(nyukinDr);

                nyukin.対応請求番号 = seikyuGokei.請求番号;

                switch (nyukinDr.RowState)
                {
                    case DataRowState.Added:        // 追加
                        SqlCommand cmdMax = con.CreateCommand();
                        cmdMax.Transaction = tran;
                        cmdMax.CommandText = "SELECT ISNULL(MAX(入金番号), 0) + 1 as 入金番号 FROM 入金明細 WHERE 企業ＣＤ = @企業ＣＤ";
                        cmdMax.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        using (SqlDataReader reader = cmdMax.ExecuteReader())
                        {
                            reader.Read();
                            newNyukinNo = (decimal)reader["入金番号"];
                        }

                        nyukin.得意先ＣＤ = _nyukinTarget.得意先ＣＤ;
                        nyukin.得意先名 = _nyukinTarget.得意先名;
                        nyukin.入金番号 = newNyukinNo;
                        nyukin.入金入力区分 = (int)Consts.NyukinNyuryokuKubun.PC;
                        ret = nyukin.Insert(con, tran);
                        break;

                    case DataRowState.Modified:     // 変更
                        ret = nyukin.Update(con, tran);
                        historyProcess = History.ProcessType.Update;
                        break;

                    case DataRowState.Deleted:      // 削除
                        ret = nyukin.Delete(con, tran);
                        historyProcess = History.ProcessType.Delete;
                        break;

                    default:
                        return true;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, nyukin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "入金明細", nyukin.Code, historyProcess);

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
        /// 画面クリア
        /// </summary>
        private void Clear()
        {
            try
            {
                if (_stopClear)
                {
                    return;
                }

                // 期間をクリア
                _nyukinTarget.期間開始日 = null;
                ReadBinding(lblKikan);        // コントロールに反映

                //*************
                // 一覧を初期化
                //*************
                seikyuTotalBindingSource.DataSource = null;
                nyukinBindingSource.DataSource = null;
                // スプレッドは検索が実施されるまで非活性
                fpSpread_SeikyuTotal.Enabled = false;
                fpSpread_Nyukin.Enabled = false;

                // 入金額の合計(初期値)をラベルに表示する
                CalcTotal(true);
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

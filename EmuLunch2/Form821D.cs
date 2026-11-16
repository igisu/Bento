using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// Web得意先マスタ詳細
    /// </summary>
    public partial class Form821D : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        // セッション一覧
        private const int SESSION_TOKEN_COLUMN           = 0;   // 認証キー(トークン)列インデックス
        private const int SESSION_UA_COLUMN              = 1;   // UA列インデックス
        private const int SESSION_LAST_LOGIN_DATE_COLUMN = 2;   // 最終ログイン日時列インデックス
        private const int SESSION_LAST_LOGIN_IP_COLUMN   = 3;   // 最終ログインIP列インデックス
        private const int SESSION_DELETE_COLUMN          = 4;   // 削除ボタン列インデックス

        // 注文履歴
        private const int HISTORY_OPERATION_DATETIME_COLUMN = 0;    // 操作日時列インデックス
        private const int HISTORY_ORDER_TYPE_COLUMN         = 1;    // 注文タイプ列インデックス
        private const int HISTORY_OPERATION_DETAIL_COLUMN   = 2;    // 操作内容列インデックス
        private const int HISTORY_TOKEN_COLUMN              = 3;    // 認証キー(トークン)列インデックス
        private const int HISTORY_IP_ADDRESS_COLUMN         = 4;    // IPアドレス列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用Web得意先マスタ
        /// </summary>
        private WebTokuisaki _webTokuisaki;

        /// <summary>
        /// 初期状態用得意先マスタ
        /// </summary>
        private WebTokuisaki _origin;

        /// <summary>
        /// Web部署注文設定
        /// </summary>
        private WebOrderSetting _webOrderSetting;

        /// <summary>
        /// 新規レコードか否か
        /// </summary>
        private bool _IsNewRecord;

        /// <summary>
        /// データソース列値変更イベント実行中フラグ
        /// </summary>
        private bool _isDtColumnChanging = false;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="webTokuisaki"></param>
        public Form821D(WebTokuisaki webTokuisaki)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _webTokuisaki = new WebTokuisaki(webTokuisaki);

                _IsNewRecord = false;
                if (_webTokuisaki.得意先ＣＤ == null)
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
        private void Form821D_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.world_ico;

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.Set(cboCharCnt,
                    new string[] { "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20" },
                    new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });

                ComboUtil.Set(cboHour,
                    new string[] { "", "0時", "1時", "2時", "3時", "4時", "5時", "6時", "7時", "8時", "9時", "10時", "11時", "12時", "13時", "14時", "15時", "16時", "17時", "18時", "19時", "20時", "21時", "22時", "23時" },
                    new int[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 });

                ComboUtil.Set(cboMinute,
                    new string[] { "", "0分", "10分", "15分", "20分", "30分", "40分", "45分", "50分" },
                    new int[] { -1, 0, 10, 15, 20, 30, 40, 45, 50 });

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_AutoLogin.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_AutoLogin.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_AutoLogin.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 新規の場合は削除不可
                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnDelete.Enabled = false;
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnSave.Text = "更新(F9)";
                    // 既存の場合は得意先ＣＤ変更不可
                    txtTokuisakiCD.Enabled = false;
                    btnTokuisakiCD.Enabled = false;
                }

                // 初期状態として保持
                _origin = new WebTokuisaki(_webTokuisaki);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                webTokuisakiBindingSource.DataSource = _webTokuisaki;

                // Web部署注文設定を取得（部署の注文締切時刻を表示）
                ShowWebOrderLimit();

                // 自動ログインテーブルを取得
                GetAutoLogin(_webTokuisaki.得意先ＣＤ);

                // Web注文履歴を取得
                GetOrderHistory(_webTokuisaki.得意先ＣＤ);

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
        private void Form821D_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F9:                       // 更新ボタン押下
                    WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
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

        /// <summary>
        /// フォームクロージング
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form821D_FormClosing(object sender, FormClosingEventArgs e)
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
                if (HasChangeWebTokuisaki() || HasChangeAutoLogin())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // Web得意先マスタの入力チェック
                            if (CanSaveWebTokuisaki() == false)
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
        /// 得意先番号 キー押下（F1）
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
        /// 得意先番号 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // Web部署注文設定を取得（部署の注文締切時刻を表示）
                ShowWebOrderLimit();
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
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCD_FormClosed);
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
        private void Form900TokuiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    _webTokuisaki.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCD);
                    ReadBinding(lblTokuisakiName);

                    // Web部署注文設定を取得（部署の注文締切時刻を表示）
                    ShowWebOrderLimit(tokuisaki);
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
        /// パスワード自動生成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGeneratePW_Click(object sender, EventArgs e)
        {
            // 自動生成したパスワードを設定する
            _webTokuisaki.ログインＰＷ = WebTokuisaki.GeneratePassword((int)cboCharCnt.SelectedValue);
            ReadBinding(txtPassword);
        }

        /// <summary>
        /// セッション一覧Spread KeyDownイベント前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_AutoLogin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                // 通常モード（＝セル編集ができる）SpreadはEscapeキーのKeyDownイベントが発生しないのでこのイベントで処理する
                if (e.KeyCode == Keys.Escape)
                {
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
        /// セッション一覧Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_AutoLogin_KeyDown(object sender, KeyEventArgs e)
        {
            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_AutoLogin_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_AutoLogin_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= SESSION_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_AutoLogin_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_AutoLogin_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_AutoLogin_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_AutoLogin.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// セッション一覧Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_AutoLogin_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 削除ボタン？
                if (e.Column == SESSION_DELETE_COLUMN)
                {
                    DataRow dr = ((DataRowView)autoLoginBindingSource.Current).Row;
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
        /// セッション一覧Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_AutoLogin_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)autoLoginBindingSource.Current).Row;
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
                // Web得意先マスタの入力チェック
                if (CanSaveWebTokuisaki() == false)
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
        /// 削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 削除対象のルートがほかのテーブルで使用されているかチェック
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
        /// セッション一覧（自動ログイン）取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetAutoLogin(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     ID" +
                                      "\r\n     ,認証キー" +
                                      "\r\n     ,企業ＣＤ" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n     ,UA" +
                                      "\r\n     ,IPv4アドレス" +
                                      "\r\n     ,最終ログイン日時" +
                                      "\r\n FROM" +
                                      "\r\n     WEB_T_自動ログイン" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND 得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     最終ログイン日時 DESC";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("自動ログインテーブル");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_AutoLogin_ColumnChanged);

                    autoLoginBindingSource.DataSource = ds;
                    autoLoginBindingSource.DataMember = "自動ログインテーブル";

                    log.Info("自動ログインテーブルを検索しました。");
                }

                fpSpread_AutoLogin_Sheet1.BindDataColumn(SESSION_TOKEN_COLUMN,           "認証キー");
                fpSpread_AutoLogin_Sheet1.BindDataColumn(SESSION_UA_COLUMN,              "UA");
                fpSpread_AutoLogin_Sheet1.BindDataColumn(SESSION_LAST_LOGIN_DATE_COLUMN, "最終ログイン日時");
                fpSpread_AutoLogin_Sheet1.BindDataColumn(SESSION_LAST_LOGIN_IP_COLUMN,   "IPv4アドレス");
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
        /// 自動ログインテーブルデータソース列値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_AutoLogin_ColumnChanged(object sender, DataColumnChangeEventArgs e)
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
        /// Web部署注文設定を取得（部署の注文締切時刻を表示）
        /// </summary>
        private void ShowWebOrderLimit()
        {
            try
            {
                if (_webTokuisaki == null || _webTokuisaki.得意先ＣＤ == null)
                {
                    cboHour.Enabled = false;
                    cboMinute.Enabled = false;
                    lblBushoLimit.Text = "";
                    return;
                }

                // 部署を取得するために、まず得意先を取得
                Tokuisaki tokuisaki = Tokuisaki.GetTokuisaki((int)_webTokuisaki.得意先ＣＤ);
                ShowWebOrderLimit(tokuisaki);
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
        /// Web部署注文設定を取得（部署の注文締切時刻を表示）
        /// </summary>
        /// <param name="tokuisaki">得意先マスタ</param>
        private void ShowWebOrderLimit(Tokuisaki tokuisaki)
        {
            try
            {
                cboHour.Enabled = false;
                cboMinute.Enabled = false;
                lblBushoLimit.Text = "";

                if (tokuisaki == null || tokuisaki.部署ＣＤ == null)
                {
                    return;
                }
                // 得意先の部署に該当するWeb部署注文設定を取得
                _webOrderSetting = WebOrderSetting.GetWebOrderSetting((int)tokuisaki.部署ＣＤ);
                if (_webOrderSetting == null)
                {
                    return;
                }

                if (_webOrderSetting.注文締切時間 == null)
                {
                    lblBushoLimit.Text = "部署設定  --:--";
                }
                else
                {
                    cboHour.Enabled = true;
                    cboMinute.Enabled = true;
                    lblBushoLimit.Text = string.Format("部署設定  {0:00}:{1:00}", _webOrderSetting.注文締切時間, (_webOrderSetting.注文締切分 ?? 0));
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
        /// Web注文履歴取得
        /// </summary>
        /// <param name="tokuisakiCD">得意先ＣＤ</param>
        private void GetOrderHistory(int? tokuisakiCD)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT TOP(20)" +
                                      "\r\n     H.操作番号" +
                                      "\r\n     ,H.企業ＣＤ" +
                                      "\r\n     ,H.認証キー" +
                                      "\r\n     ,H.得意先ＣＤ" +
                                      "\r\n     ,T.得意先名" +
                                      "\r\n     ,H.UA" +
                                      "\r\n     ,H.IPv4アドレス" +
                                      "\r\n     ,H.WEB履歴区分ＣＤ" +
                                      "\r\n     ,VW.WEB履歴区分名" +
                                      "\r\n     ,H.操作日時" +
                                      "\r\n     ,H.操作内容" +
                                      "\r\n FROM" +
                                      "\r\n     WEB_T_注文履歴 AS H" +
                                      "\r\n     LEFT JOIN 得意先マスタ as T" +
                                      "\r\n         ON H.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n         AND H.得意先ＣＤ = T.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN V_WEB履歴区分 AS VW" +
                                      "\r\n         ON VW.WEB履歴区分ＣＤ = H.WEB履歴区分ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND H.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     操作日時 DESC" +
                                      "\r\n     ,操作番号 DESC";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD ?? -1;  // null の場合は -1 で検索（新規の場合は0件を取得）

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("Web注文履歴");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    orderHistoryBindingSource.DataSource = ds;
                    orderHistoryBindingSource.DataMember = "Web注文履歴";

                    log.Info("Web注文履歴を検索しました。");
                }

                fpSpread_OrderHistory_Sheet1.BindDataColumn(HISTORY_OPERATION_DATETIME_COLUMN, "操作日時");
                fpSpread_OrderHistory_Sheet1.BindDataColumn(HISTORY_ORDER_TYPE_COLUMN,         "WEB履歴区分名");
                fpSpread_OrderHistory_Sheet1.BindDataColumn(HISTORY_OPERATION_DETAIL_COLUMN,   "操作内容");
                fpSpread_OrderHistory_Sheet1.BindDataColumn(HISTORY_TOKEN_COLUMN,              "認証キー");
                fpSpread_OrderHistory_Sheet1.BindDataColumn(HISTORY_IP_ADDRESS_COLUMN,         "IPv4アドレス");
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
        /// Web得意先マスタが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeWebTokuisaki()
        {
            if (_webTokuisaki.Equals(_origin))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 自動ログインテーブルが変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeAutoLogin()
        {
            // 自動ログインテーブルを比較
            DataSet ds = (DataSet)autoLoginBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["自動ログインテーブル"];

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
        /// Web得意先マスタが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveWebTokuisaki()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_webTokuisaki.HasErrors)
                {
                    MessageBox.Show(this, _webTokuisaki.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    // Web得意先マスタ保存
                    if (SaveWebTokuisaki(con, tran) == false)
                    {
                        return false;
                    }

                    // 自動ログインテーブル保存
                    if (SaveChangeAutoLogin(con, tran) == false)
                    {
                        return false;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("Web得意先マスタを{0}しました。", historyProcess.DisplayName());
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
        /// Web得意先マスタ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveWebTokuisaki(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _webTokuisaki.Insert(con, tran);
                }
                else
                {
                    ret = _webTokuisaki.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _webTokuisaki.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "得意先マスタ", _webTokuisaki.Code, historyProcess);
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
        /// 自動ログインテーブル保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveChangeAutoLogin(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)autoLoginBindingSource.DataSource;
                DataTable dt = ds.Tables["自動ログインテーブル"];

                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    WebAutoLogin webAutoLogin = new WebAutoLogin(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Deleted:      // 削除
                            ret = webAutoLogin.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, webAutoLogin.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "自動ログインテーブル", webAutoLogin.Code, historyProcess);
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
        /// （保存後の）再読み込み
        /// </summary>
        private void Reload()
        {
            try
            {
                _webTokuisaki = WebTokuisaki.GetWebTokuisakiReload((int)_webTokuisaki.得意先ＣＤ);

                _origin = new WebTokuisaki(_webTokuisaki);

                _IsNewRecord = false;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                webTokuisakiBindingSource.DataSource = _webTokuisaki;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";
                // 既存の場合は得意先ＣＤ変更不可
                txtTokuisakiCD.Enabled = false;
                btnTokuisakiCD.Enabled = false;

                btnDelete.Enabled = this.HasAuthEdit;

                // Web部署注文設定を取得（部署の注文締切時刻を表示）
                ShowWebOrderLimit();

                // 自動ログインテーブルを取得
                GetAutoLogin(_webTokuisaki.得意先ＣＤ);

                // Web注文履歴を取得
                GetOrderHistory(_webTokuisaki.得意先ＣＤ);
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
                    // Web得意先マスタ削除
                    if (DeleteWebTokuisaki(con, tran) == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("Web得意先マスタを{0}しました。", historyProcess.DisplayName());
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
        /// 得意先マスタ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteWebTokuisaki(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _webTokuisaki.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, _webTokuisaki.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "得意先マスタ", _webTokuisaki.Code, historyProcess);
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

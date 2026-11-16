using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 売上入力
    /// </summary>
    public partial class Form401 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYOHIN_CD_COLUMN       = 0;   // 商品ＣＤ列インデックス
        private const int REFERENCE_COLUMN       = 1;   // 参照ボタン列インデックス
        private const int SYOHIN_NAME_COLUMN     = 2;   // 商品名列インデックス
        private const int TANKA_COLUMN           = 3;   // 単価列インデックス
        private const int KOSU_COLUMN            = 4;   // 個数列インデックス
        private const int KINGAKU_COLUMN         = 5;   // 金額列インデックス
        private const int NEBIKI_COLUMN          = 6;   // 値引列インデックス
        private const int NEBIKI_JIYU_COLUMN     = 7;   // 値引事由列インデックス
        private const int SHIHARAI_KBN_COLUMN    = 8;   // 支払区分列インデックス
        private const int SYOKUJI_KBN_COLUMN     = 9;   // 食事区分列インデックス
        private const int DELETE_COLUMN          = 10;  // 削除ボタン列インデックス

        #endregion

        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// コースの担当者
        /// </summary>
        /// <remarks>コース選択時に表示する担当者（コースコンボ作成時に取得する）</remarks>
        private class CourseTanto
        {
            public int? 担当者ＣＤ { get; set; }
            public string 担当者名 { get; set; }

            public CourseTanto(int? cd, string name)
            {
                担当者ＣＤ = cd;
                担当者名 = name;
            }
        }
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用売上ヘッダ
        /// </summary>
        private UriageHeader _uriageHeader = null;

        /// <summary>
        /// 初期状態用売上ヘッダ
        /// </summary>
        private UriageHeader _origin;

        /// <summary>
        /// 売上入力検索条件
        /// </summary>
        private UriageMeisaiCriteria _uriageMeisaiCriteria;

        /// <summary>
        /// 売上入力検索条件検索実行時スナップショット
        /// </summary>
        private UriageMeisaiCriteria _uriageMeisaiCriteriaSnapShot = null;

        /// <summary>
        /// 売上入力得意先情報
        /// </summary>
        private UriageTokuisaki _uriageTokuisaki;

        /// <summary>
        /// （売上日時点の）消費税
        /// </summary>
        private Syohizei _syohizei;

        /// <summary>
        /// 管理マスタ（チケット回収時売上フラグ）
        /// </summary>
        private Kanri _kanri;

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

        /// <summary>
        /// 支払方法「チケット」
        /// </summary>
        private int _shiharaiHohoTicket = -2;

        /// <summary>
        /// 売上区分「チケット」
        /// </summary>
        private int _uriageKubunTicket = -2;

        /// <summary>
        /// コースＣＤをキーとして、その担当者ＣＤ,名を値に持つマップ（コースコンボで使用）
        /// </summary>
        private Dictionary<int, CourseTanto> _courseTantoMap = new Dictionary<int, CourseTanto>();

        /// <summary>
        /// 表示ボタン処理中にコースコンボの変更イベントの処理をしないフラグ（メッセージボックスの表示を抑制）
        /// </summary>
        private bool _stopCboCourseSelectedIndexChanged = false;

        /// <summary>
        /// 表示ボタン処理中に未来日警告をしないフラグ
        /// </summary>
        private bool _stopWarningFuture = false;

        /// <summary>
        /// 部署ＣＤに該当するコースＣＤ（これを初期表示する）
        /// </summary>
        private int _defaultCourseCD = -1;

        /// <summary>
        /// コースコンボ選択インデックス（同じ選択肢を選んでもSelectedIndexChanged(SelectedValueChanged)イベントが発生するのでそれを回避するため）
        /// </summary>
        private int _CboCourseSelectedIndex = -1;

        /// <summary>
        /// フォームロード中の場合はtrue
        /// </summary>
        private bool _isFormLoading = true;

        /// <summary>
        /// 部署ＣＤ,精算日,コースＣＤをキーとして、売上精算入力画面を値に持つマップ
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Form403> _form403Map = new Dictionary<string, Form403>();

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="course"></param>
        public Form401(UriageMeisaiCriteria uriageMeisaiCriteria)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _uriageMeisaiCriteria = new UriageMeisaiCriteria(uriageMeisaiCriteria);

                _IsNewRecord = true;

                if (_uriageMeisaiCriteria.売上日 == null)
                {
                    _uriageMeisaiCriteria.売上日 = DateTime.Today;
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
        private void Form401_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.calculator_ico;

                // 売掛現金区分、税区分、丸め処理区分を初期化
                toggleLabelColor(lblGenkin, GENKIN_COLOR, false);
                toggleLabelColor(lblKakeuri, KAKEURI_COLOR, false);
                toggleLabelColor(lblSotozei, SOTOZEI_COLOR, false);
                toggleLabelColor(lblHikazei, HIKAZEI_COLOR, false);
                toggleLabelColor(lblCeiling, CEILING_COLOR, false);
                toggleLabelColor(lblFloor, FLOOR_COLOR, false);
                toggleLabelColor(lblRound, ROUND_COLOR, false);

                // 権限によりボタンを制御
                btnSave.Enabled = this.HasAuthEdit;
                btnDelete.Enabled = this.HasAuthEdit;

                // スプレッドは検索が実施されるまで非活性
                fpSpread1.Enabled = false;

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

                // 列フッターを表示
                fpSpread1_Sheet1.ColumnFooterVisible = true;
                fpSpread1_Sheet1.ColumnFooter.Cells[0, TANKA_COLUMN].Text = "合計";

                // 個数
                fpSpread1_Sheet1.ColumnFooter.Columns[KOSU_COLUMN].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                fpSpread1_Sheet1.ColumnFooter.SetAggregationType(0, KOSU_COLUMN, FarPoint.Win.Spread.Model.AggregationType.Sum);
                fpSpread1_Sheet1.ColumnFooter.SetAggregationFormat(0, KOSU_COLUMN, "{0:N0}");

                // 金額
                fpSpread1_Sheet1.ColumnFooter.Columns[KINGAKU_COLUMN].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                fpSpread1_Sheet1.ColumnFooter.SetAggregationType(0, KINGAKU_COLUMN, FarPoint.Win.Spread.Model.AggregationType.Sum);
                fpSpread1_Sheet1.ColumnFooter.SetAggregationFormat(0, KINGAKU_COLUMN, "{0:C}");

                // 値引
                fpSpread1_Sheet1.ColumnFooter.Columns[NEBIKI_COLUMN].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                fpSpread1_Sheet1.ColumnFooter.SetAggregationType(0, NEBIKI_COLUMN, FarPoint.Win.Spread.Model.AggregationType.Sum);
                fpSpread1_Sheet1.ColumnFooter.SetAggregationFormat(0, NEBIKI_COLUMN, "{0:C}");


                // ****************************************************************************************
                // 得意先の支払方法が「チケット」の場合は、支払方法（売上区分）の初期値を「チケット」にする
                // ****************************************************************************************
                // 不可視の支払方法コンボ作成を利用して、支払方法区分CDをV_支払方法区分から取得
                ComboUtil.SetShiharaiHoho(cboShiharaiHohoDummy);
                DataTable dt = (DataTable)cboShiharaiHohoDummy.DataSource;
                DataRow[] drs = dt.Select("支払方法区分名 like '*チケット'");
                if (drs != null && drs.Length != 0)
                {
                    _shiharaiHohoTicket = (int)drs[0]["支払方法区分CD"];  // 「チケット」の支払方法を取得
                }

                // 不可視の売上区分コンボ作成を利用して、売上区分CDをV_売上区分から取得
                ComboUtil.SetUriageKubun(cboUriageKubunDummy);
                dt = (DataTable)cboUriageKubunDummy.DataSource;
                drs = dt.Select("売上区分名 like '*チケット'");
                if (drs != null && drs.Length != 0)
                {
                    _uriageKubunTicket = (int)drs[0]["売上区分CD"];  // 「チケット」の売上区分を取得
                }

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 管理マスタ取得
                _kanri = Kanri.GetKanri();

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
                }

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                uriageMeisaiCriteriaBindingSource.DataSource = _uriageMeisaiCriteria;

                // スプレッドの値引事由、支払区分(売上区分)、食事区分の各コンボボックス列を作成
                FarPoint.Win.Spread.CellType.ComboBoxCellType cbNebiki = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetNebikiKubun(cbNebiki);
                cbNebiki.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbNebiki.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[NEBIKI_JIYU_COLUMN].CellType = cbNebiki;

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbUriageKubun = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetUriageKubun(cbUriageKubun);
                cbUriageKubun.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbUriageKubun.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[SHIHARAI_KBN_COLUMN].CellType = cbUriageKubun;

                FarPoint.Win.Spread.CellType.ComboBoxCellType cbSyokujiKubun = new FarPoint.Win.Spread.CellType.ComboBoxCellType();
                ComboUtil.SetSyokujiKubun(cbSyokujiKubun);
                cbSyokujiKubun.AutoCompleteSource = AutoCompleteSource.ListItems;
                cbSyokujiKubun.AutoCompleteMode = AutoCompleteMode.Suggest;
                fpSpread1_Sheet1.Columns[SYOKUJI_KBN_COLUMN].CellType = cbSyokujiKubun;

                // 既存のデータを取得
                _stopWarningFuture = true;
                btnQuery.PerformClick();
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
                _isFormLoading = false;
            }
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form401_Shown(object sender, EventArgs e)
        {
            // 精算日(=売上日)、部署ＣＤ、コースＣＤに該当する売上精算_ヘッダがあれば（＝売上精算済み）、売上明細の変更はできないものとする
            HasUriageSeisanHeader();

            // スプレッドが活性化していればフォーカスする
            // ※一覧から得意先ＣＤが渡されるなどして検索されていれば、結果の有無にかかわらずスプレッドを活性化している
            if (fpSpread1.Enabled)
            {
                fpSpread1.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form401_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    btnClear.PerformClick();        // クリアボタン押下
                    break;

                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
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

                case Keys.F11:                      // 削除
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
                    btnDelete.Focus();
                    btnDelete.PerformClick();
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
        private void Form401_FormClosing(object sender, FormClosingEventArgs e)
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
                if (HasChangeHeader() || HasChangeDetail())
                {
                    DialogResult r = MessageBox.Show(this, string.Format("変更されています。{0}しますか？", _IsNewRecord ? "登録" : "更新"), this.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (r)
                    {
                        case DialogResult.Yes:
                            // 売上ヘッダの入力チェック
                            if (CanSaveUriageHeader() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 売上明細の入力チェック
                            if (CanSaveUriageMeisai() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (Save() >= 0)
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
        /// 得意先テキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // 検索条件が変わったら画面をクリア
                if (_uriageMeisaiCriteriaSnapShot != null && _uriageMeisaiCriteria.Equals(_uriageMeisaiCriteriaSnapShot) == false)
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
                formTokuisakiDlg.TargetDate = _uriageMeisaiCriteria.売上日;
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
                    _uriageMeisaiCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;

                    _uriageMeisaiCriteria.部署ＣＤ = tokuisaki.部署ＣＤ;
                    ReadBinding(cboBusyo);      // コントロールに反映
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
        /// 部署コンボ 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboBusyo_Validated(object sender, EventArgs e)
        {
            try
            {
                // 検索条件が変わったら画面をクリア
                if (_uriageMeisaiCriteriaSnapShot != null && _uriageMeisaiCriteria.Equals(_uriageMeisaiCriteriaSnapShot) == false)
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
        /// 注文日テキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpUriagebi_Validated(object sender, EventArgs e)
        {
            try
            {
                // 検索条件が変わったら画面をクリア
                if (_uriageMeisaiCriteriaSnapShot != null && _uriageMeisaiCriteria.Equals(_uriageMeisaiCriteriaSnapShot) == false)
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
        /// 表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageMeisaiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageMeisaiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _uriageMeisaiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 売上日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                if (_stopWarningFuture == false)
                {
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_uriageMeisaiCriteria.売上日 > warnDate)
                    {
                        if (MessageBox.Show(this, "売上日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            Clear2();
                            _IsNewRecord = true;

                            btnSave.Text = "登録(F9)";
                            btnDelete.Enabled = false;

                            // 未検索状態にする
                            _uriageMeisaiCriteriaSnapShot = null;

                            return;
                        }
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                cboCourse.Enabled = true;               // (★)

                // コースコンボ変更イベント処理を抑制
                // ※cboCourse.SelectedValueChanged -= cboCourse_SelectedValueChanged; は上手くいかない
                _stopCboCourseSelectedIndexChanged = true;

                // コースコンボをクリア（担当者も同時にクリア）
                cboCourse.DataSource = null;

                // 消費税を取得
                _syohizei = Syohizei.GetSyohizei((DateTime)_uriageMeisaiCriteria.売上日);

                // 売上入力得意先情報を取得
                GetUriageTokuisaki();

                // 売上ヘッダを取得
                GetUriageHeader();

                // 売上入力コース情報を取得
                GetUriageCourse();

                // コースコンボ初期値設定
                SetComboCourse();

                uriageHeaderBindingSource.DataSource = _uriageHeader;

                // 売上明細を取得
                GetUriageMeisai();

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
                    btnDelete.Enabled = this.HasAuthEdit;
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _uriageMeisaiCriteriaSnapShot = new UriageMeisaiCriteria(_uriageMeisaiCriteria);

                // 初期状態として保持
                _origin = new UriageHeader(_uriageHeader);

                // 精算日(=売上日)、部署ＣＤ、コースＣＤに該当する売上精算_ヘッダがあれば（＝売上精算済み）、売上明細の変更はできないものとする
                HasUriageSeisanHeader();

                // 分配入力済み（の分配元）（＝分配元数量>1の売上明細が有る）であれば、売上明細の変更はできないものとする
                HasBnupai();
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
                _stopWarningFuture = false;

                // コースコンボ変更イベントを処理を再開
                // ※cboCourse.SelectedValueChanged += cboCourse_SelectedValueChanged; は上手くいかない
                _stopCboCourseSelectedIndexChanged = false;
                _CboCourseSelectedIndex = cboCourse.SelectedIndex;
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// コースコンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_stopCboCourseSelectedIndexChanged == false)
            {
                if (_CboCourseSelectedIndex != cboCourse.SelectedIndex)
                {
                    MessageBox.Show(this, "コースが変更されました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            _CboCourseSelectedIndex = cboCourse.SelectedIndex;
        }

        /// <summary>
        /// コースコンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboCourse_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_stopCboCourseSelectedIndexChanged == true)
                {
                    return;
                }

                txtCourseTantosyaCD.Text = "";
                lblCourseTantosyaName.Text = "";

                // コースに該当する担当者があれば、得意先の担当者CD,名を設定する
                if (cboCourse.SelectedValue == null)
                {
                    return;
                }

                if (_courseTantoMap.ContainsKey((int)cboCourse.SelectedValue))
                {
                    CourseTanto courseTanto = _courseTantoMap[(int)cboCourse.SelectedValue];
                    if (courseTanto.担当者ＣＤ != null)
                    {
                        txtCourseTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                        lblCourseTantosyaName.Text = courseTanto.担当者名;

                        // 同じ内容を売上ヘッダにも設定する
                        txtTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                        if (_uriageHeader != null)
                        {
                            _uriageHeader.配送担当者ＣＤ = courseTanto.担当者ＣＤ;
                        }
                        lblTantosyaName.Text = courseTanto.担当者名;
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
        /// 担当者ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTantosyaCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTantosyaCD.PerformClick();
            }
        }

        /// <summary>
        /// 担当者ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTantosyaCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904TantosyaCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 担当者選択ダイアログクローズ（担当者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904TantosyaCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtTantosyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtTantosyaCD.Text = tantousya.担当者ＣＤ.ToString();
                    if (_uriageHeader != null)
                    {
                        _uriageHeader.配送担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    }
                    lblTantosyaName.Text = tantousya.担当者名;
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
                OpenForm901BySpread();          // 商品選択ダイアログを表示
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
                    case SYOHIN_CD_COLUMN:      // 商品ＣＤ
                    case TANKA_COLUMN:          // 単価
                    case KOSU_COLUMN:           // 個数
                    case NEBIKI_COLUMN:         // 値引
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
                    OpenForm901BySpread();          // 商品選択ダイアログを表示
                }

                // 削除ボタン？
                if (e.Column == DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)uriageMeisaiBindingSource.Current).Row;
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
        /// Spreadから商品選択ダイアログを表示する
        /// </summary>
        private void OpenForm901BySpread()
        {
            try
            {
                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCD_FormClosed);
                formSyohinDlg.ShowDialog();
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

        /// 商品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form901SyohinCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form901 form = (Form901)sender;
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

                    Syohin syohin = form.SelectedSyohin;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.ActiveRowIndex, SYOHIN_CD_COLUMN].Value = syohin.商品ＣＤ;   // 商品CDセルに値を設定→dt_ColumnChanged()が呼ばれる
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
            DataRow dr = ((DataRowView)uriageMeisaiBindingSource.Current).Row;
            dr.EndEdit();
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
                // 画面をクリアし、新規レコードとする
                Clear2();
                _IsNewRecord = true;

                btnSave.Text = "登録(F9)";
                btnDelete.Enabled = false;

                // 売上入力検索条件を初期化
                _uriageMeisaiCriteria = new UriageMeisaiCriteria();
                _uriageMeisaiCriteria.売上日 = DateTime.Today;
                uriageMeisaiCriteriaBindingSource.DataSource = _uriageMeisaiCriteria;

                // 売上入力得意先情報を初期化
                _uriageTokuisaki = new UriageTokuisaki();
                uriageTokuisakiBindingSource.DataSource = _uriageTokuisaki;

                // 未検索状態にする
                _uriageMeisaiCriteriaSnapShot = null;

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
        /// 登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 売上ヘッダの入力チェック
                if (CanSaveUriageHeader() == false)
                {
                    return;
                }

                // 売上明細の入力チェック
                if (CanSaveUriageMeisai() == false)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (Save() >= 0)
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
        /// 売上入力得意先情報を取得
        /// </summary>
        private void GetUriageTokuisaki()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     T.得意先ＣＤ" +
                                      "\r\n     ,T.得意先名略称" +
                                      "\r\n     ,T.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,B.支店ＣＤ" +
                                      "\r\n     ,S.支店名" +
                                      "\r\n     ,ISNULL(T.税区分, 1) AS 税区分" +                   // 初期値は1:内税
                                      "\r\n     ,ISNULL(T.丸め処理区分, 2) AS 丸め処理区分" +       // 初期値は2:切り捨て
                                      "\r\n     ,ISNULL(T.売掛現金区分, 0) AS 売掛現金区分" +       // 初期値は0:現金
                                      "\r\n     ,T.支払方法" +
                                      "\r\n FROM" +
                                      "\r\n     得意先マスタ AS T" +
                                      "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = T.企業ＣＤ AND B.部署ＣＤ = T.部署ＣＤ AND ISNULL(B.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN 支店マスタ AS S ON S.企業ＣＤ = B.企業ＣＤ AND S.支店ＣＤ = B.支店ＣＤ AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n WHERE" +
                                      "\r\n     T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND ISNULL(T.無効フラグ, 0) = 0" +
                                      "\r\n     AND T.得意先ＣＤ = @得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _uriageMeisaiCriteria.得意先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("売上入力得意先情報");
                    adp.Fill(dt);

                    _uriageTokuisaki = new UriageTokuisaki();
                    if (dt.Rows.Count != 0)
                    {
                        _uriageTokuisaki = new UriageTokuisaki(dt.Rows[0]);
                    }

                    uriageTokuisakiBindingSource.DataSource = _uriageTokuisaki;

                    // タイトルバーに得意先名略称を表示
                    if (string.IsNullOrEmpty(_uriageTokuisaki.得意先名略称))
                    {
                        this.Text = "売上入力";
                    }
                    else
                    {
                        this.Text = string.Format("{0} - 売上入力", _uriageTokuisaki.得意先名略称);
                    }

                    // 売掛現金区分、税区分、丸め処理区分を表示
                    toggleLabelColor(lblGenkin, GENKIN_COLOR, (_uriageTokuisaki.売掛現金区分 == (int)Consts.UriGenKubun.Genkin));
                    toggleLabelColor(lblKakeuri, KAKEURI_COLOR, (_uriageTokuisaki.売掛現金区分 == (int)Consts.UriGenKubun.Urikake));

                    toggleLabelColor(lblSotozei, SOTOZEI_COLOR, (_uriageTokuisaki.税区分 == (int)Syohizei.ZeiKubun.Soto));
                    toggleLabelColor(lblHikazei, HIKAZEI_COLOR, (_uriageTokuisaki.税区分 == (int)Syohizei.ZeiKubun.Hikazei));

                    // 丸め区分は外税の場合のみ表示
                    if (_uriageTokuisaki.税区分 == (int)Syohizei.ZeiKubun.Soto)
                    {
                        toggleLabelColor(lblCeiling, CEILING_COLOR, (_uriageTokuisaki.丸め処理区分 == (int)Rounding.HasuKubun.Ceiling));
                        toggleLabelColor(lblFloor, FLOOR_COLOR, (_uriageTokuisaki.丸め処理区分 == (int)Rounding.HasuKubun.Floor));
                        toggleLabelColor(lblRound, ROUND_COLOR, (_uriageTokuisaki.丸め処理区分 == (int)Rounding.HasuKubun.Round));
                    }
                    else
                    {
                        toggleLabelColor(lblCeiling, CEILING_COLOR, false);
                        toggleLabelColor(lblFloor, FLOOR_COLOR, false);
                        toggleLabelColor(lblRound, ROUND_COLOR, false);
                    }

                    log.Info("売上入力得意先情報を検索しました。");
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
        /// 売上ヘッダ取得
        /// </summary>
        private void GetUriageHeader()
        {
            try
            {
                // 売上ヘッダを取得
                _uriageHeader = UriageHeader.GetUriageHeader((int)_uriageMeisaiCriteria.得意先ＣＤ, (DateTime)_uriageMeisaiCriteria.売上日, (int)_uriageMeisaiCriteria.部署ＣＤ);
                if (_uriageHeader == null)
                {
                    _IsNewRecord = true;        // 新規レコード

                    // 存在しない場合は初期化
                    _uriageHeader = new UriageHeader();
                    _uriageHeader.売上日 = _uriageMeisaiCriteria.売上日;
                    _uriageHeader.得意先ＣＤ = _uriageMeisaiCriteria.得意先ＣＤ;
                    _uriageHeader.部署ＣＤ = _uriageMeisaiCriteria.部署ＣＤ;
                    _uriageHeader.得意先名 = _uriageMeisaiCriteria.得意先名;
                    _uriageHeader.コースＣＤ = null;
                    _uriageHeader.配送担当者ＣＤ = null;
                    _uriageHeader.税区分 = _uriageTokuisaki.税区分;
                    _uriageHeader.丸め処理区分 = _uriageTokuisaki.丸め処理区分;
                }
                else
                {
                    _IsNewRecord = false;
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
        /// 売上入力コース情報を取得
        /// </summary>
        private void GetUriageCourse()
        {
            try
            {
                // 臨時コーステーブル／マスタから取得
                string sql = "\r\n SELECT" +
                             "\r\n     CT.コースＣＤ" +
                             "\r\n     ,CM.コース名" +
                             "\r\n     ,CM.部署ＣＤ" +
                             "\r\n     ,CM.担当者ＣＤ" +
                             "\r\n     ,T.担当者名" +
                             "\r\n FROM" +
                             "\r\n     臨時コーステーブル AS CT" +
                             "\r\n     INNER JOIN 臨時コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND CT.実施日 = CM.実施日 AND ISNULL(CM.無効フラグ, 0) = 0" +
                             "\r\n     LEFT JOIN 担当者マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.担当者ＣＤ = CM.担当者ＣＤ AND ISNULL(T.無効フラグ, 0) = 0" +
                             "\r\n WHERE" +
                             "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                             "\r\n     AND CT.実施日 = @実施日" +
                             "\r\n     AND CT.得意先ＣＤ = @得意先ＣＤ" +
                             "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                             "\r\n     AND CM.コース区分 = @コース区分" +
                             "\r\n ORDER BY" +
                             "\r\n     CT.コースＣＤ";

                DataTable dt = GetUriageCourseMain(sql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    // 存在しない場合はコーステーブル／マスタから取得
                    sql = "\r\n SELECT" +
                          "\r\n     CT.コースＣＤ" +
                          "\r\n     ,CM.コース名" +
                          "\r\n     ,CM.部署ＣＤ" +
                          "\r\n     ,CM.担当者ＣＤ" +
                          "\r\n     ,T.担当者名" +
                          "\r\n FROM" +
                          "\r\n     コーステーブル AS CT" +
                          "\r\n     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                          "\r\n     LEFT JOIN 担当者マスタ AS T ON T.企業ＣＤ = CT.企業ＣＤ AND T.担当者ＣＤ = CM.担当者ＣＤ AND ISNULL(T.無効フラグ, 0) = 0" +
                          "\r\n WHERE" +
                          "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                          "\r\n     AND CT.得意先ＣＤ = @得意先ＣＤ" +
                          "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                          "\r\n     AND CM.コース区分 = @コース区分" +
                          "\r\n ORDER BY" +
                          "\r\n     CT.コースＣＤ";
                    dt = GetUriageCourseMain(sql);
                }

                // コースに該当する担当者のマップを生成
                // ※ついでに、検索条件の部署ＣＤと一致するコースＣＤを取得しておく
                _defaultCourseCD = -1;
                _courseTantoMap.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    CourseTanto courseTanto = new CourseTanto(dr["担当者ＣＤ"] as int?, dr["担当者名"] as string);
                    _courseTantoMap[(int)dr["コースＣＤ"]] = courseTanto;

                    if (_defaultCourseCD == -1 && _uriageMeisaiCriteria.部署ＣＤ == (int)dr["部署ＣＤ"])
                    {
                        _defaultCourseCD = (int)dr["コースＣＤ"];
                    }
                }

                // 空白の選択肢を追加
                DataRow drNew = dt.NewRow();
                drNew["コースＣＤ"] = -1;
                drNew["コース名"] = "";
                dt.Rows.Add(drNew);

                // コースコンボの選択肢を作成
                cboCourse.DisplayMember = "コース名";
                cboCourse.ValueMember = "コースＣＤ";
                cboCourse.DataSource = dt;

                log.Info("売上入力コース情報を検索しました。");
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
        /// 売上入力コース情報取得主処理
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>コース情報のデータテーブル</returns>
        private DataTable GetUriageCourseMain(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _uriageMeisaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = _uriageMeisaiCriteria.売上日;

                    // 売上日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun((DateTime)_uriageMeisaiCriteria.売上日);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("売上入力コース情報");
                    adp.Fill(dt);
                    return dt;
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
        /// コースコンボ初期値設定
        /// </summary>
        private void SetComboCourse()
        {
            try
            {
                if (_IsNewRecord)
                {
                    // 得意先から求めたコースを初期値として持つ
                    // コースから取得した担当者を初期値として持つ
                    if (_defaultCourseCD > 0)
                    {
                        _uriageHeader.コースＣＤ = _defaultCourseCD;

                        if (_courseTantoMap.ContainsKey((int)_uriageHeader.コースＣＤ))
                        {
                            CourseTanto courseTanto = _courseTantoMap[(int)_uriageHeader.コースＣＤ];
                            if (courseTanto.担当者ＣＤ != null)
                            {
                                _uriageHeader.配送担当者ＣＤ = courseTanto.担当者ＣＤ;
                            }
                        }
                    }

                    // コース注文入力／コース売上一覧から売上入力を開いたときは、指定されたコースをデフォルト表示する
                    if (_uriageMeisaiCriteria.コースＣＤ != null)
                    {
                        _uriageHeader.コースＣＤ = _uriageMeisaiCriteria.コースＣＤ;
                        if (_courseTantoMap.ContainsKey((int)_uriageHeader.コースＣＤ))
                        {
                            CourseTanto courseTanto = _courseTantoMap[(int)_uriageHeader.コースＣＤ];
                            if (courseTanto.担当者ＣＤ != null)
                            {
                                _uriageHeader.配送担当者ＣＤ = courseTanto.担当者ＣＤ;
                            }
                        }
                    }
                }

                if (_uriageHeader.コースＣＤ != null)
                {
                    Course course = Course.GetCourse((int)_uriageHeader.コースＣＤ);
                    if (course != null)
                    {
                        // 取得したコースＣＤがコースコンボに存在していなければ追加する
                        // ※一括注文入力においてその得意先とは無関係のコースを指定された注文が売上に反映されるとあり得る
                        DataTable dt = (DataTable)cboCourse.DataSource;
                        DataRow[] drs = dt.Select(string.Format("コースＣＤ='{0}'", course.コースＣＤ));
                        if (drs == null || drs.Length == 0)
                        {
                            DataRow dr = dt.NewRow();
                            dr["コースＣＤ"] = course.コースＣＤ;
                            dr["コース名"] = course.コース名;
                            dt.Rows.Add(dr);

                            // コースに該当する担当者のマップに追加
                            CourseTanto courseTanto = new CourseTanto(course.担当者ＣＤ, course.担当者名);
                            _courseTantoMap[(int)course.コースＣＤ] = courseTanto;
                        }
                    }

                    // コースに該当する担当者があれば、その担当者CD,名を表示する
                    if (_courseTantoMap.ContainsKey((int)_uriageHeader.コースＣＤ))
                    {
                        CourseTanto courseTanto = _courseTantoMap[(int)_uriageHeader.コースＣＤ];
                        if (courseTanto.担当者ＣＤ != null)
                        {
                            txtCourseTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                            lblCourseTantosyaName.Text = courseTanto.担当者名;

                            // 同じ内容を売上ヘッダにも設定する
                            txtTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                            if (_uriageHeader != null)
                            {
                                _uriageHeader.配送担当者ＣＤ = courseTanto.担当者ＣＤ;
                            }
                            lblTantosyaName.Text = courseTanto.担当者名;
                        }
                    }

                    cboCourse.SelectedValue = (int)_uriageHeader.コースＣＤ;
                }
                else
                {
                    cboCourse.SelectedValue = -1;
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
        /// 売上明細取得
        /// </summary>
        private void GetUriageMeisai()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.企業ＣＤ" +
                                      "\r\n     ,A.売上日" +
                                      "\r\n     ,A.得意先ＣＤ" +
                                      "\r\n     ,A.部署ＣＤ" +
                                      "\r\n     ,A.明細番号" +
                                      "\r\n     ,A.商品ＣＤ" +
                                      "\r\n     ,S.商品名" +
                                      "\r\n     ,A.商品グループＣＤ" +
                                      "\r\n     ,A.売上区分" +
                                      "\r\n     ,A.商品単価" +
                                      "\r\n     ,A.商品数量" +
                                      "\r\n     ,A.値引額" +
                                      "\r\n     ,A.値引事由区分" +
                                      "\r\n     ,A.値引前金額" +
                                      "\r\n     ,A.値引後金額" +
                                      "\r\n     ,A.消費税額" +
                                      "\r\n     ,A.商品金額合計税込" +
                                      "\r\n     ,A.計算税率" +
                                      "\r\n     ,A.チケット束フラグ" +
                                      "\r\n     ,A.現金補填フラグ" +
                                      "\r\n     ,A.主食ＣＤ" +
                                      "\r\n     ,A.副食ＣＤ" +
                                      "\r\n     ,A.食事区分" +
                                      "\r\n     ,A.分配元数量" +
                                      "\r\n     ,A.作成担当者ＣＤ" +
                                      "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                      "\r\n     ,A.作成日" +
                                      "\r\n     ,A.修正担当者ＣＤ" +
                                      "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                      "\r\n     ,A.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     売上明細 AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ   AS S  ON S.企業ＣＤ = A.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                      "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND A.売上日 = @売上日" +
                                      "\r\n AND A.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.明細番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = (DateTime)_uriageMeisaiCriteria.売上日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)_uriageMeisaiCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageMeisaiCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("売上明細");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.TableNewRow += new DataTableNewRowEventHandler(dt_TableNewRow);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    uriageMeisaiBindingSource.DataSource = ds;
                    uriageMeisaiBindingSource.DataMember = "売上明細";

                    log.Info("売上明細を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,    "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN,  "商品名");
                fpSpread1_Sheet1.BindDataColumn(TANKA_COLUMN,        "商品単価");
                fpSpread1_Sheet1.BindDataColumn(KOSU_COLUMN,         "商品数量");
                fpSpread1_Sheet1.BindDataColumn(KINGAKU_COLUMN,      "値引前金額");
                fpSpread1_Sheet1.BindDataColumn(NEBIKI_COLUMN,       "値引額");
                fpSpread1_Sheet1.BindDataColumn(NEBIKI_JIYU_COLUMN,  "値引事由区分");
                fpSpread1_Sheet1.BindDataColumn(SHIHARAI_KBN_COLUMN, "売上区分");
                fpSpread1_Sheet1.BindDataColumn(SYOKUJI_KBN_COLUMN,  "食事区分");

                // 検索したので活性化
                fpSpread1.Enabled = true;
                fpSpread1.Focus();
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
        /// テーブル行追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            try
            {
                // 支払区分（売上区分）の初期値を設定
                // 得意先の支払方法がチケット？
                if (_uriageTokuisaki.支払方法 == _shiharaiHohoTicket)
                {
                    e.Row["売上区分"] = _uriageKubunTicket;
                }
                else
                {
                    // 得意先の支払方法がチケットではない場合は売掛現金区分
                    e.Row["売上区分"] = _uriageTokuisaki.売掛現金区分;
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
                fpSpread1.SuspendLayout();      // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                UriageMeisai uriageMeisai = new UriageMeisai(e.Row);

                // 商品CDの場合は商品マスタなどから取得
                if (e.Column.ColumnName == "商品ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["商品名"] = null;
                    e.Row["商品グループＣＤ"] = DBNull.Value;
                    e.Row["商品単価"] = DBNull.Value;
                    e.Row["商品数量"] = DBNull.Value;
                    e.Row["値引額"] = DBNull.Value;
                    e.Row["値引事由区分"] = DBNull.Value;
                    e.Row["値引前金額"] = DBNull.Value;
                    e.Row["値引後金額"] = DBNull.Value;
                    e.Row["消費税額"] = DBNull.Value;
                    e.Row["商品金額合計税込"] = DBNull.Value;
                    e.Row["計算税率"] = _syohizei.消費税率;
                    e.Row["チケット束フラグ"] = DBNull.Value;
                    e.Row["主食ＣＤ"] = DBNull.Value;
                    e.Row["副食ＣＤ"] = DBNull.Value;
                    //e.Row["売上区分"] = _uriageTokuisaki.売掛現金区分;
                    //e.Row["売上区分"] = DBNull.Value;
                    e.Row["食事区分"] = DBNull.Value;
                    e.Row["分配元数量"] = DBNull.Value;
                    if (uriageMeisai.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)uriageMeisai.商品ＣＤ);
                        if (syohin != null)
                        {
                            e.Row["商品名"] = syohin.商品名;
                            e.Row["商品単価"] = Syohizei.GetUnitPrice(_uriageHeader.税区分, syohin.商品単価税込, syohin.商品単価税抜);
                            e.Row["計算税率"] = _syohizei.GetTaxRate(syohin.軽減税率フラグ);
                            e.Row.SetField<int?>("主食ＣＤ", syohin.主食ＣＤ);
                            e.Row.SetField<int?>("副食ＣＤ", syohin.副食ＣＤ);
                            e.Row.SetField<int?>("食事区分", syohin.食事区分);

                            // チケット？
                            if (syohin.弁当区分 == (int)Consts.BentoKubun.Ticket)
                            {
                                e.Row["チケット束フラグ"] = true;
                            }

                            // 得意先単価マスタから
                            TokuisakiTanka tokuisakiTanka = TokuisakiTanka.GetTokuisakiTanka((int)_uriageHeader.得意先ＣＤ, (int)syohin.商品ＣＤ);
                            if (tokuisakiTanka != null)
                            {
                                e.Row["商品単価"] = Syohizei.GetUnitPrice(_uriageHeader.税区分, tokuisakiTanka.商品単価税込, tokuisakiTanka.商品単価税抜);
                                //*****************************************************************************
                                // 分配入力では、分配先の商品単価は分配元のそれを採用している。
                                // 言い換えると、分配元の単価を採用するのは分配入力のみ。
                                // 売上入力では（分配入力ではないので）商品単価は常に自分の商品単価を採用する。
                                //*****************************************************************************
                            }

                            // 主要商品リストマスタから
                            SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)_uriageHeader.部署ＣＤ, (int)syohin.商品ＣＤ);
                            if (syuyoSyohin != null)
                            {
                                e.Row["商品グループＣＤ"] = syuyoSyohin.商品グループＣＤ;
                            }
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

                // 単価・個数の場合は金額を算出
                if (e.Column.ColumnName == "商品単価" || e.Column.ColumnName == "商品数量")
                {
                    if (uriageMeisai.商品ＣＤ != null)
                    {
                        CalcRow(e.Row);         // 行を計算
                    }
                }

                // 値引の場合はエラーチェック
                if (e.Column.ColumnName == "値引額")
                {
                    e.Row.SetColumnError(e.Column, uriageMeisai["値引額"]);
                    if (uriageMeisai.商品ＣＤ != null)
                    {
                        CalcRow(e.Row);         // 行を計算
                    }
                }

                // 支払区分（売上区分）がチケットまたはボーナスの場合は金額を0にする
                if (e.Column.ColumnName == "売上区分")
                {
                    if (uriageMeisai.商品ＣＤ != null)
                    {
                        CalcRow(e.Row);         // 行を計算
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
                fpSpread1.ResumeLayout(true);       // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する
            }
        }

        /// <summary>
        /// 行を計算
        /// </summary>
        /// <param name="dr">計算対象行</param>
        private void CalcRow(DataRow dr)
        {
            try
            {
                decimal? suryo = dr["商品数量"] as decimal?;
                decimal? tanka = dr["商品単価"] as decimal?;
                decimal? nebiki = dr["値引額"] as decimal?;
                int? uriageKubun  = dr["売上区分"] as int?;

                // 管理マスタのチケット回収時売上フラグがOFF かつ 売上区分がチケットorボーナスの場合は、金額を0にする
                // ※回収時売上フラグがONの場合は、売上区分がチケットorボーナスの場合でも金額を算出する
                if (_kanri.チケット回収時売上フラグ != true &&
                    (uriageKubun == (int)Consts.UriageKubun.Ticket ||
                    uriageKubun == (int)Consts.UriageKubun.Bonus))
                {
                    dr["値引前金額"] = 0m;
                    dr["値引後金額"] = 0m;
                    dr["消費税額"] = 0m;
                    dr["商品金額合計税込"] = 0m;
                }
                else
                {
                    // 値引前金額・値引後金額・消費税額・商品金額合計税込を算出
                    decimal kingaku = Syohizei.GetAmount(suryo, tanka);
                    dr["値引前金額"] = kingaku;
                    decimal amount = kingaku - (nebiki ?? 0m);
                    dr["値引後金額"] = amount;
                    dr["消費税額"] = Syohizei.GetTax(_uriageHeader.税区分, amount, (decimal)dr["計算税率"], _uriageHeader.丸め処理区分);
                    dr["商品金額合計税込"] = Syohizei.GetAmountIncludeTax(_uriageHeader.税区分, amount, (decimal)dr["計算税率"], _uriageHeader.丸め処理区分);
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
        /// 売上ヘッダ・売上明細が変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeHeader()
        {
            // 売上ヘッダを比較
            if (_uriageHeader == null)
            {
                return false;
            }

            if (_uriageHeader.Equals(_origin))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 売上明細が変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeDetail()
        {
            // 売上明細を比較
            DataSet ds = (DataSet)uriageMeisaiBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["売上明細"];

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
        /// 売上ヘッダが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveUriageHeader()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_uriageMeisaiCriteria.HasErrors)
                {
                    MessageBox.Show(this, _uriageMeisaiCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実施？
                if (_uriageMeisaiCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_uriageMeisaiCriteria.Equals(_uriageMeisaiCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_uriageHeader.HasErrors)
                {
                    MessageBox.Show(this, _uriageHeader.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 売上明細が登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveUriageMeisai()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)uriageMeisaiBindingSource.DataSource;
                if (ds == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                DataTable dt = ds.Tables["売上明細"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    UriageMeisai uriageMeisai = new UriageMeisai(dr);

                    // 売上日がない場合は新規扱い
                    if (uriageMeisai.売上日 == null)
                    {
                        uriageMeisai.売上日 = _uriageMeisaiCriteria.売上日;
                        uriageMeisai.得意先ＣＤ = _uriageMeisaiCriteria.得意先ＣＤ;
                        uriageMeisai.部署ＣＤ = _uriageMeisaiCriteria.部署ＣＤ;
                    }

                    // エラーがある？
                    if (uriageMeisai.HasErrors)
                    {
                        dr.SetColumnError("商品ＣＤ", uriageMeisai["商品ＣＤ"]);
                        dr.SetColumnError("値引額", uriageMeisai["値引額"]);
                        dr.SetColumnError("値引事由区分", uriageMeisai["値引事由区分"]);
                        hasError = true;
                    }
                    else
                    {
                        // 商品ＣＤに該当するマスタが存在するか？
                        if (uriageMeisai.商品ＣＤ != null)
                        {
                            Syohin syohin = Syohin.GetSyohin((int)uriageMeisai.商品ＣＤ);
                            if (syohin == null)
                            {
                                dr.SetColumnError("商品ＣＤ", "商品が見つかりません。");
                                hasError = true;
                            }
                        }
                    }
                }

                if (hasError)
                {
                    MessageBox.Show(this, "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// <returns>
        /// 正常終了の場合は、0
        /// 注文ヘッダにぶら下がる注文明細が無い（ので注文ヘッダも削除した）場合は、1
        /// エラーの場合は、-1
        /// </returns>
        private int Save()
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            // 精算日(=売上日)、部署ＣＤ、コースＣＤに該当する売上精算_ヘッダがあれば（＝売上精算済み）、売上明細の変更はできないものとする
            if (HasUriageSeisanHeader())
            {
                return -1;
            }

            // 分配入力済み（の分配元）（＝分配元数量>1の売上明細が有る）であれば、売上明細の変更はできないものとする
            if (HasBnupai())
            {
                return -1;
            }

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
                    // 売上ヘッダ保存
                    if (SaveUriageHeader(con, tran) == false)
                    {
                        return -1;
                    }

                    // 売上明細保存
                    if (SaveUriageMeisai(con, tran) == false)
                    {
                        return -1;
                    }

                    // 売上明細が存在しなければ売上ヘッダを削除する
                    int ret = DeleteUriageHeaderWhenNoMeisai(con, tran);
                    if (ret < 0)
                    {
                        return -1;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("売上ヘッダ・売上明細を{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 初期状態として保持
                    _origin = new UriageHeader(_uriageHeader);

                    if (ret != 0)
                    {
                        // 登録した結果、売上明細が0件なので売上ヘッダも削除した場合
                        log.Info("売上明細の登録件数が0件なので、売上ヘッダを削除しました。");
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
        }

        /// <summary>
        /// 売上ヘッダ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                // 売上ヘッダの納品書伝票番号が未設定の場合は、注文ヘッダを取得してその納品書伝票番号を設定する
                // （設定済みの場合は売上から納品書を出力したということなので、その納品書伝票番号を尊重する）
                if (_uriageHeader.納品書伝票番号 == null)
                {
                    ChumonHeader chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Chuumon, (DateTime)_uriageHeader.売上日, (int)_uriageHeader.得意先ＣＤ, (int)_uriageHeader.部署ＣＤ);
                    if (chumonHeader != null)
                    {
                        _uriageHeader.納品書伝票番号 = chumonHeader.納品書伝票番号;
                    }
                }

                if (_IsNewRecord)
                {
                    ret = _uriageHeader.Insert(con, tran);
                }
                else
                {
                    ret = _uriageHeader.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _uriageHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "売上ヘッダ", _uriageHeader.Code, historyProcess);
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
        /// 売上明細保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveUriageMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)uriageMeisaiBindingSource.DataSource;
                DataTable dt = ds.Tables["売上明細"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                int seq = 1;
                bool ret = false;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    UriageMeisai uriageMeisai = new UriageMeisai(dr);

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            // 商品ＣＤまたは個数が未入力の場合は追加しない
                            if (uriageMeisai.商品ＣＤ == null ||
                                uriageMeisai.商品数量 == null)
                            {
                                continue;
                            }
                            uriageMeisai.売上日 = _uriageMeisaiCriteria.売上日;
                            uriageMeisai.得意先ＣＤ = _uriageMeisaiCriteria.得意先ＣＤ;
                            uriageMeisai.部署ＣＤ = _uriageMeisaiCriteria.部署ＣＤ;
                            uriageMeisai.明細番号 = seq;
                            ret = uriageMeisai.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            seq++;
                            break;

                        case DataRowState.Modified:     // 変更
                        case DataRowState.Unchanged:    // 変更なし→明細番号のみ更新
                            // 商品ＣＤまたは個数が未入力の場合は削除する
                            if (uriageMeisai.商品ＣＤ != null &&
                                uriageMeisai.商品数量 != null)
                            {
                                uriageMeisai.明細番号 = seq;
                                ret = uriageMeisai.Update(con, tran);
                                historyProcess = History.ProcessType.Update;
                                seq++;
                            }
                            else
                            {
                                ret = uriageMeisai.Delete(con, tran);
                                historyProcess = History.ProcessType.Delete;
                            }
                            break;

                        case DataRowState.Deleted:      // 削除
                            ret = uriageMeisai.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "売上明細", uriageMeisai.Code, historyProcess);
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
        /// 売上明細が存在しなければ売上ヘッダを削除する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>削除した件数(0か1)。削除に失敗した場合はマイナス値</returns>
        private int DeleteUriageHeaderWhenNoMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                try
                {
                    //*************************************
                    // 売上明細が存在しない売上ヘッダを削除
                    //*************************************
                    SqlCommand cmdDelH = con.CreateCommand();
                    cmdDelH.CommandText = "\r\n DELETE 売上ヘッダ" +
                                          "\r\n FROM" +
                                          "\r\n     売上ヘッダ AS H" +
                                          "\r\n     LEFT JOIN 売上明細 AS M" +
                                          "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                          "\r\n     AND H.売上日     = M.売上日" +
                                          "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                          "\r\n     AND H.売上日     = @売上日 "+
                                          "\r\n     AND H.得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                          "\r\n     AND M.企業ＣＤ IS NULL";
                    cmdDelH.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmdDelH.Parameters.Add("@売上日", SqlDbType.Date).Value = _uriageMeisaiCriteria.売上日;
                    cmdDelH.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _uriageMeisaiCriteria.得意先ＣＤ;
                    cmdDelH.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageMeisaiCriteria.部署ＣＤ;
                    cmdDelH.Transaction = tran;
                    log.Debug(cmdDelH.CommandTextWithParameters());
                    int ret = cmdDelH.ExecuteNonQuery();

                    return ret;
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
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
                _IsNewRecord = false;

                lblMode.Text = MODE_CAPTION_EDIT;
                lblMode.BackColor = BACK_COLOR_EDIT;
                btnSave.Text = "更新(F9)";

                btnDelete.Enabled = this.HasAuthEdit;

                _stopWarningFuture = true;
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

                // 検索実行時の条件が変わっている？
                if (_uriageMeisaiCriteria.Equals(_uriageMeisaiCriteriaSnapShot) == false)
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
        /// DB削除
        /// </summary>
        /// <returns>ダイアログを閉じる場合はtrue</returns>
        private bool Delete()
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            // 分配入力済み（の分配元）（＝分配元数量>1の売上明細が有る）であっても、削除は可能

            // 精算日(=売上日)、部署ＣＤ、コースＣＤに該当する売上精算_ヘッダがあれば（＝売上精算済み）、売上明細の変更はできないものとする
            if (HasUriageSeisanHeader())
            {
                return false;
            }

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 売上ヘッダ削除
                    if (DeleteUriageHeader(con, tran) == false)
                    {
                        return false;
                    }

                    // 売上明細削除
                    if (DeleteUriageMeisai(con, tran) == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("売上ヘッダ・売上明細を{0}しました。", historyProcess.DisplayName());
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
        /// 売上ヘッダ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _uriageHeader.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, _uriageHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "売上ヘッダ", _uriageHeader.Code, historyProcess);
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
        /// 売上明細削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteUriageMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 全削除
                DataSet ds = (DataSet)uriageMeisaiBindingSource.DataSource;
                DataTable dt = ds.Tables["売上明細"];

                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは削除しない
                    {
                        continue;
                    }

                    UriageMeisai uriageMeisai = new UriageMeisai(dr);
                    if (uriageMeisai.作成日 == null)
                    {
                        continue;
                    }
                    bool ret = uriageMeisai.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, uriageMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "売上明細", uriageMeisai.Code, historyProcess);
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
        /// 売上精算_ヘッダの有無（＝売上精算済みかどうか）を返す
        /// </summary>
        /// <returns></returns>
        private bool HasUriageSeisanHeader()
        {
            try
            {
                // 表示ボタンクリック(btnQuery_Click)時は処理を行なう（メッセージを表示する）が、フォームロード(Form_Load)時は行なわない。
                // 代わりにフォーム初めて表示(Form_Shown)時にメッセージを表示する。
                if (_isFormLoading)
                {
                    return false;
                }

                // コースCDがNullの売上精算は存在しない
                if (_uriageHeader == null || _uriageHeader.コースＣＤ == null)
                {
                    return false;
                }

                // 精算日(=売上日)、部署ＣＤ、コースＣＤに該当する売上精算_ヘッダを取得
                UriageSeisanHeader uriageSeisanHeader = UriageSeisanHeader.GetUriageSeisanHeader((DateTime)_uriageHeader.売上日,
                                                                                                 (int)_uriageHeader.部署ＣＤ,
                                                                                                 (int)_uriageHeader.コースＣＤ);
                if (uriageSeisanHeader != null)
                {
                    // コースが変更されると売上精算に矛盾が生じるので、コースを非活性にする(★)
                    cboCourse.Enabled = false;

                    DialogResult r =  MessageBox.Show(this, "この売上日、部署、コースの売上は、売上精算入力画面で精算済みです。" +
                                                      "\nこの売上を追加または変更する前に、該当する売上精算データを売上精算入力画面で削除してください。" +
                                                      "\n\n 売上精算入力画面を開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                    if (r == System.Windows.Forms.DialogResult.Yes)
                    {
                        // 売上精算入力画面を開く
                        Form403 formUriageSeisan;
                        string key = string.Format("{0},{1:yyyyMMddHHmmss},{2}", _uriageHeader.部署ＣＤ, _uriageHeader.売上日, _uriageHeader.コースＣＤ);
                        if (_form403Map.ContainsKey(key))
                        {
                            formUriageSeisan = _form403Map[key];
                            // 売上精算画面が閉じられている？
                            if (formUriageSeisan == null || formUriageSeisan.IsDisposed)
                            {
                                formUriageSeisan = null;
                            }
                        }
                        else
                        {
                            formUriageSeisan = null;    // 売上精算画面未生成
                        }

                        // 売上精算画面を生成
                        if (formUriageSeisan == null)
                        {
                            UriageSeisanCriteria uriageSeisanCriteria = new UriageSeisanCriteria();
                            uriageSeisanCriteria.部署ＣＤ = _uriageHeader.部署ＣＤ;
                            uriageSeisanCriteria.精算日 = _uriageHeader.売上日;
                            uriageSeisanCriteria.コースＣＤ = _uriageHeader.コースＣＤ;
                            formUriageSeisan = new Form403(uriageSeisanCriteria);
                            if (formUriageSeisan.HasAuthBrowse == false)
                            {
                                MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return true;
                            }
                            _form403Map[key] = formUriageSeisan;
                        }

                        // モーダルにはしない。閉じても再検索しない。
                        formUriageSeisan.MdiParent = this.MdiParent;
                        formUriageSeisan.Show();
                        formUriageSeisan.Activate();
                    }
                    return true;
                }
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
        /// 分配入力済み（の分配元）かどうかを返す
        /// </summary>
        /// <returns>分配元数量が1以上の売上明細が有ればtrue</returns>
        private bool HasBnupai()
        {
            try
            {
                DataSet ds = (DataSet)uriageMeisaiBindingSource.DataSource;
                if (ds == null)
                {
                    return false;
                }
                DataTable dt = ds.Tables["売上明細"];

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    UriageMeisai uriageMeisai = new UriageMeisai(dr);
                    if ((uriageMeisai.分配元数量 ?? 0) > 0)
                    {
                        MessageBox.Show(this, "この売上日、部署、得意先の売上は、分配入力済みです。" +
                                              "\nこの売上を変更する前に、分配先の売上をすべて削除してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return true;
                    }
                }
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
        /// 画面クリア
        /// </summary>
        private void Clear()
        {
            try
            {
                // 注文入力と同様のタイミングの処理だが、画面のクリアはしない

                // コース注文入力／コース売上一覧から引き継いだコースをクリアする
                _uriageMeisaiCriteria.コースＣＤ = null;
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
        private void Clear2()
        {
            try
            {
                this.Text = "売上入力";

                lblMode.Text = MODE_CAPTION_NEW;
                lblMode.BackColor = BACK_COLOR_NEW;

                // スプレッドは検索が実施されるまで非活性
                fpSpread1.Enabled = false;
                uriageMeisaiBindingSource.DataSource = null;

                // 売掛現金区分、税区分、丸め処理区分を初期化
                toggleLabelColor(lblGenkin, GENKIN_COLOR, false);
                toggleLabelColor(lblKakeuri, KAKEURI_COLOR, false);
                toggleLabelColor(lblSotozei, SOTOZEI_COLOR, false);
                toggleLabelColor(lblHikazei, HIKAZEI_COLOR, false);
                toggleLabelColor(lblCeiling, CEILING_COLOR, false);
                toggleLabelColor(lblFloor, FLOOR_COLOR, false);
                toggleLabelColor(lblRound, ROUND_COLOR, false);

                // コースコンボ変更イベント処理を抑制
                _stopCboCourseSelectedIndexChanged = true;

                // コースコンボをクリア（担当者も同時にクリア）
                cboCourse.DataSource = null;
                txtCourseTantosyaCD.Text = "";
                lblCourseTantosyaName.Text = "";

                // コース注文入力／コース売上一覧から引き継いだコースをクリアする
                _uriageMeisaiCriteria.コースＣＤ = null;

                // 売上ヘッダーを初期化
                _uriageHeader = new UriageHeader();
                uriageHeaderBindingSource.DataSource = _uriageHeader;

                // 初期状態として保持
                _origin = new UriageHeader(_uriageHeader);
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
        /// 税区分・丸め区分・現金掛売区分のラベルの前景色・背景色を切り替える
        /// </summary>
        /// <param name="lbl">対象のラベルコントロール</param>
        /// <param name="backColor">ラベル固有の背景色</param>
        /// <param name="enabled">見せる場合はtrue</param>
        private void toggleLabelColor(Label lbl, Color backColor, bool enabled)
        {
            lbl.ForeColor = GetAlphaBlendColor(this.BackColor, Color.Black, enabled ? ENABLE_ALPHA : DISABLE_ALPHA);
            lbl.BackColor = GetAlphaBlendColor(this.BackColor, backColor, enabled ? ENABLE_ALPHA : DISABLE_ALPHA);
        }
        #endregion
    }
}

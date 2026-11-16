using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 注文入力
    /// </summary>
    public partial class Form200 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMON_SYOHIN_CD_COLUMN   = 0;    // 商品ＣＤ列インデックス
        private const int CHUMON_REFERENCE_COLUMN   = 1;    // 参照ボタン列インデックス
        private const int CHUMON_SYOHIN_NAME_COLUMN = 2;    // 商品名列インデックス
        private const int CHUMON_SEIZOBI_COLUMN     = 3;    // 製造日列インデックス
        private const int CHUMON_TANKA_COLUMN       = 4;    // 単価列インデックス
        private const int CHUMON_YOTEISU_COLUMN     = 5;    // 予定数列インデックス
        private const int CHUMON_KOSU_COLUMN        = 6;    // 個数列インデックス
        private const int CHUMON_KINGAKU_COLUMN     = 7;    // 金額列インデックス
        private const int CHUMON_DELETE_COLUMN      = 8;    // 削除ボタン列インデックス

        private const int COURSE_TOKUISAKI_CD_COLUMN   = 0; // 得意先ＣＤ列インデックス
        private const int COURSE_TOKUISAKI_NAME_COLUMN = 1; // 得意先名列インデックス
        private const int COURSE_URIAGE_COLUMN         = 2; // 売上有無列インデックス
        private const int COURSE_HAISO_COLUMN          = 3; // 配送有無列インデックス
        private const int COURSE_JUCYUHOHO_COLUMN      = 4; // 受注方法列インデックス

        private const int WINDOW_WIDTH_DEFAULT = 851;               // コース状況非表示フォーム幅
        private const int WINDOW_WIDTH_COURSE_CONDITION = 1291;     // コース状況表示フォーム幅

        /// <summary>
        /// 売上がある得意先の背景色（薄いブルーグレー）
        /// </summary>
        public readonly Color BACK_COLOR_URIAGE = Color.FromArgb(214, 241, 241);

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
        //* スタティック変数
        //**************************************
        #region スタティック変数

        /// <summary>
        /// 得意先ＣＤをキーとして、注文入力画面を値に持つマップ（子フォームマップ）
        /// </summary>
        private static SortedList<int, Form200> _form200GroupMap = new SortedList<int,Form200>();

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 保存用注文ヘッダ
        /// </summary>
        private ChumonHeader _chumonHeader = null;

        /// <summary>
        /// グループ検索で開かれた際の得意先ＣＤ
        /// </summary>
        private int groupOpenedTokuisakiCD = -1;

        /// <summary>
        /// 初期状態用注文ヘッダ
        /// </summary>
        private ChumonHeader _origin;

        /// <summary>
        /// 注文入力検索条件
        /// </summary>
        private ChumonMeisaiCriteria _chumonCriteria;

        /// <summary>
        /// 注文入力検索条件検索実行時スナップショット
        /// </summary>
        private ChumonMeisaiCriteria _chumonCriteriaSnapShot = null;

        /// <summary>
        /// 注文入力得意先情報
        /// </summary>
        private ChumonTokuisaki _chumonTokuisaki;

        /// <summary>
        /// 注文入力時残高表示
        /// </summary>
        private ChumonZandaka _chumonZandaka;

        /// <summary>
        /// （配送日時点の）消費税
        /// </summary>
        private Syohizei _syohizei;

        /// <summary>
        /// （製造日／配送日算出日数取得用の）部署マスタ
        /// </summary>
        private Busyo _busyo;

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
        /// コースＣＤをキーとして、その担当者ＣＤ,名を値に持つマップ（コースコンボで使用）
        /// </summary>
        private Dictionary<int, CourseTanto> _courseTantoMap = new Dictionary<int, CourseTanto>();

        /// <summary>
        /// 備考コンボ用データテーブル
        /// </summary>
        private DataTable _dtBiko = null;

        /// <summary>
        /// 表示ボタン処理中にコースコンボの変更イベントの処理をしないフラグ（メッセージボックスの表示を抑制）
        /// </summary>
        private bool _stopCboCourseSelectedIndexChanged = false;

        /// <summary>
        /// 表示ボタン処理中に注文のみ／得意先単価を含めるの変更イベントの処理をしないフラグ（GetChumonMeisai()を二重に呼ばないようにする）
        /// </summary>
        private bool _stopRdoChumonClick = false;

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

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ

        /// <summary>
        /// CTIからで開かれた場合はtrue
        /// </summary>
        public bool CTIOpened { get; set; }

        /// <summary>
        /// グループ検索で開かれた場合はtrue
        /// </summary>
        public bool GroupOpened { get; set; }

        /// <summary>
        /// 注文一覧から開かれた場合はtrue
        /// </summary>
        public bool ListOpened { get; set; }

        /// <summary>
        /// 受注発信一覧から架電したときの得意先発信履歴データ
        /// </summary>
        public TokuisakiHassinRireki TokuisakiHassinRireki { get; set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="chumonCriteria">注文入力検索条件</param>
        public Form200(ChumonMeisaiCriteria chumonCriteria)
        {
            try
            {
                InitializeComponent();

                // データをいったんコピーしておく
                _chumonCriteria = new ChumonMeisaiCriteria(chumonCriteria);

                _IsNewRecord = true;

                if (_chumonCriteria.注文日 == null)
                {
                    _chumonCriteria.注文日 = DateTime.Today;
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
        private void Form200_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

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
                fpSpread_ChumonMeisai.Enabled = false;

                // 配送日も検索が実施されるまで非活性
                dtpHaisobi.Enabled = false;

                // コース状況の表示はそのチェックボックスに依存
                chkCourseCondition.Checked = false;

                //*****************
                // スプレッドの設定
                //*****************
                // （※）デザイン時にExcel互換の入力マップを読み込んでいる
                // Spreadデフォルトの動作は、非編集モードでの入力は常にセルへの挿入なので、これを上書きにする
                fpSpread_ChumonMeisai.EditModeReplace = true;                           // 編集開始時に入力済みデータを選択状態にします(☆)
                // この設定により、非編集モード時のF2キー押下でセル値が選択されるようになった。
                // しかし、ExcelのF2キーの動作はセル値の末尾にカーソルがあるので、Spreadでも同様に動作にすべく
                // F2キーのデフォルト動作を無効にして処理を実装する
                FarPoint.Win.Spread.InputMap im = fpSpread_ChumonMeisai.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenFocused);  // 非編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);   // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                //im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生しない）

                im = fpSpread_ChumonMeisai.GetInputMap(FarPoint.Win.Spread.InputMapMode.WhenAncestorOfFocused);                     // 編集モードのインプットマップ
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.F2, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // F2キーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Enter, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Enterキーを無効
                im.Put(new FarPoint.Win.Spread.Keystroke(Keys.Delete, Keys.None), FarPoint.Win.Spread.SpreadActions.None);  // Deleteキーを無効（アクティブセルでも編集中でもイベントが発生する）

                // 列フッターを表示
                fpSpread_ChumonMeisai_Sheet1.ColumnFooterVisible = true;
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.Cells[0, CHUMON_YOTEISU_COLUMN].Text = "合計";

                // 個数
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.Columns[CHUMON_KOSU_COLUMN].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.SetAggregationType(0, CHUMON_KOSU_COLUMN, FarPoint.Win.Spread.Model.AggregationType.Sum);
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.SetAggregationFormat(0, CHUMON_KOSU_COLUMN, "{0:N0}");

                // 金額
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.Columns[CHUMON_KINGAKU_COLUMN].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Right;
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.SetAggregationType(0, CHUMON_KINGAKU_COLUMN, FarPoint.Win.Spread.Model.AggregationType.Sum);
                fpSpread_ChumonMeisai_Sheet1.ColumnFooter.SetAggregationFormat(0, CHUMON_KINGAKU_COLUMN, "{0:C}");

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                SetBikoCombo();

                // 新規の場合は削除不可
                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnPrev.Enabled = true;     // 前回入力ボタンを活性
                    btnNext.Enabled = false;    // 次回入力ボタンを非活性
                    btnDelete.Enabled = false;
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnPrev.Enabled = true;     // 前回／次回入力ボタンを活性
                    btnNext.Enabled = true;
                    btnSave.Text = "更新(F9)";
                    btnDelete.Enabled = this.HasAuthEdit;
                }

                // グループ検索で開かれたのであれば、グループ検索は使用不可とする（孫フォームは開かないようにする）
                if (GroupOpened)
                {
                    btnGroupOpen.Enabled = false;
                    groupOpenedTokuisakiCD = (int)_chumonCriteria.得意先ＣＤ;
                }

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                chumonCriteriaBindingSource.DataSource = _chumonCriteria;

                // 「本日ではありません」ラベルのVisibleを設定
                ToggleChumonbiIsNotToday();

                // 注文入力時残高表示を初期表示（クリア）
                btnZandaka.Enabled = false;
                _chumonZandaka = new ChumonZandaka();
                chumonZandakaBindingSource.DataSource = _chumonZandaka;

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
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form200_Shown(object sender, EventArgs e)
        {
            // スプレッドが活性化していればフォーカスする
            // ※一覧から得意先ＣＤが渡されるなどして検索されていれば、結果の有無にかかわらずスプレッドを活性化している
            if (fpSpread_ChumonMeisai.Enabled)
            {
                fpSpread_ChumonMeisai.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form200_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    // スプレッドにフォーカスがあるときにクリアボタン押下で
                    // スプレッド非活性→配送日にフォーカス移動→配送日も非活性→dtpHaisobi_Validated()→コースが変わる→"コースが変更されました"メッセージボックスが表示されてしまう。
                    // これを回避すべく、クリアボタンにフォーカスしておく。
                    btnClear.Focus();
                    btnClear.PerformClick();        // クリアボタン押下
                    break;

                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F7:
                    btnUriage.PerformClick();        // 売上入力ボタン押下
                    break;

                case Keys.F9:                       // 更新ボタン押下
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_ChumonMeisai.EditMode == true)
                    {
                        fpSpread_ChumonMeisai.StopCellEditing();
                        Application.DoEvents();     // SetColumnError()によるエラー状態をスプレッドに反映 など
                    }
                    else
                    {
                        WriteBinding(this.ActiveControl);   // 入力中の内容を強制的に反映する
                    }
                    btnSave.Focus();
                    btnSave.PerformClick();
                    break;

                case Keys.F11:                      // 削除
                    // スプレッドが編集途中の場合は強制的に編集を終了(確定)する
                    if (fpSpread_ChumonMeisai.EditMode == true)
                    {
                        fpSpread_ChumonMeisai.StopCellEditing();
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
        private void Form200_FormClosing(object sender, FormClosingEventArgs e)
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
                if (fpSpread_ChumonMeisai.EditMode == true)
                {
                    fpSpread_ChumonMeisai.StopCellEditing();
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
                            // 注文ヘッダの入力チェック
                            if (CanSaveChumonHeader() == false)
                            {
                                e.Cancel = true;
                                return;
                            }

                            // 注文明細の入力チェック
                            if (CanSaveChumonMeisai() == false)
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
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form200_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // 注文入力画面マップ（子フォームマップ）から、自分を削除する
                if (_form200GroupMap.ContainsKey(groupOpenedTokuisakiCD))
                {
                    _form200GroupMap.Remove(groupOpenedTokuisakiCD);
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
        /// 得意先テキスト変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // 「得意先単価を含める」にする
                rdoTokuisakiTanka.Checked = true;
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
        /// 得意先テキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTokuisakiCD_Validated(object sender, EventArgs e)
        {
            try
            {
                // 検索条件が変わったら画面をクリア
                if (_chumonCriteriaSnapShot != null && _chumonCriteria.Equals(_chumonCriteriaSnapShot) == false)
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
                formTokuisakiDlg.TargetDate = _chumonCriteria.注文日;       // ホントは配送日を渡したいが、表示ボタンを押下しないと配送日が取得できないので無理
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
                    _chumonCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;

                    _chumonCriteria.部署ＣＤ = tokuisaki.部署ＣＤ;
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
                if (_chumonCriteriaSnapShot != null && _chumonCriteria.Equals(_chumonCriteriaSnapShot) == false)
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
        /// 注文日テキスト変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpChumonbi_TextDateChanged(object sender, EventArgs e)
        {
            try
            {
                // 「得意先単価を含める」にする
                rdoTokuisakiTanka.Checked = true;
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
                if (_chumonCriteriaSnapShot != null && _chumonCriteria.Equals(_chumonCriteriaSnapShot) == false)
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
                if (_chumonCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_chumonCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _chumonCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                if (_stopWarningFuture == false)
                {
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_chumonCriteria.注文日 > warnDate)
                    {
                        if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            Clear();
                            _IsNewRecord = true;

                            btnSave.Text = "登録(F9)";
                            btnPrev.Enabled = true;     // 前回入力ボタンは活性
                            btnNext.Enabled = false;    // 次回入力ボタンは非活性
                            btnDelete.Enabled = false;

                            // 未検索状態にする
                            _chumonCriteriaSnapShot = null;

                            return;
                        }
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // コースコンボ変更イベント処理を抑制
                // ※cboCourse.SelectedValueChanged -= cboCourse_SelectedValueChanged; は上手くいかない
                _stopCboCourseSelectedIndexChanged = true;

                // ラジオボタンチェックイベント処理を抑制
                _stopRdoChumonClick = true;

                // コースコンボをクリア（担当者も同時にクリア）
                cboCourse.DataSource = null;

                // 消費税を取得
                _syohizei = Syohizei.GetSyohizei((DateTime)_chumonCriteria.注文日);

                // 部署マスタを取得（製造日／配送日算出日数の取得用）
                _busyo = Busyo.GetBusyo((int)_chumonCriteria.部署ＣＤ);

                // 備考コンボをリセット
                ResetBiko(cboBiko1);
                ResetBiko(cboBiko2);
                ResetBiko(cboBiko3);
                ResetBiko(cboBiko4);
                ResetBiko(cboBiko5);

                // 注文入力得意先情報を取得
                GetChumonTokuisaki();

                // 注文ヘッダを取得
                GetChumonHeader();

                // 注文入力コース情報を取得
                GetChumonCourse();

                // コースコンボ初期値設定
                SetComboCourse(false);

                // コース状況を取得
                GetCourseCondition();

                chumonHeaderBindingSource.DataSource = _chumonHeader;       // (★)

                // 注文明細を取得
                GetChumonMeisai();

                // 新規の場合は削除不可
                if (_IsNewRecord)
                {
                    lblMode.Text = MODE_CAPTION_NEW;
                    lblMode.BackColor = BACK_COLOR_NEW;
                    btnSave.Text = "登録(F9)";
                    btnPrev.Enabled = true;     // 前回入力ボタンを活性
                    btnNext.Enabled = false;    // 次回入力ボタンを非活性
                    btnDelete.Enabled = false;

                    // 得意先マスタが持っている備考をデフォルト表示する
                    cboBiko1.Text = _chumonTokuisaki.備考1;
                    _chumonHeader.備考1 = _chumonTokuisaki.備考1;

                    cboBiko2.Text = _chumonTokuisaki.備考2;
                    _chumonHeader.備考2 = _chumonTokuisaki.備考2;

                    cboBiko3.Text = _chumonTokuisaki.備考3;
                    _chumonHeader.備考3 = _chumonTokuisaki.備考3;
                }
                else
                {
                    lblMode.Text = MODE_CAPTION_EDIT;
                    lblMode.BackColor = BACK_COLOR_EDIT;
                    btnPrev.Enabled = true;    // 前回／次回入力ボタンを活性
                    btnNext.Enabled = true;
                    btnDelete.Enabled = this.HasAuthEdit;
                    btnSave.Text = "更新(F9)";
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _chumonCriteriaSnapShot = new ChumonMeisaiCriteria(_chumonCriteria);

                // 初期状態として保持
                _origin = new ChumonHeader(_chumonHeader);

                // 検索したので活性化
                fpSpread_ChumonMeisai.Enabled = true;
                fpSpread_ChumonMeisai.Focus();

                // 検索したので配送日も活性化
                dtpHaisobi.Enabled = true;

                // 「注文日と異なります」ラベルのVisibleを設定
                ToggleHaisobiIsDifferentFromChumonbi();

                // 注文入力時残高表示を初期表示（クリア）
                btnZandaka.Enabled = true;
                _chumonZandaka.Clear();
                chumonZandakaBindingSource.ResetBindings(false);
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

                // ラジオボタンチェックイベント処理を再開
                _stopRdoChumonClick = false;

                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 発信ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCall_Click(object sender, EventArgs e)
        {
            try
            {
                // CTIを用いて得意先に電話を掛ける
                CallToTokuisaki();
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

                    if (chkCourseCondition.Checked)
                    {
                        GetCourseCondition();           // コース状況を取得
                    }
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

                if (chkCourseCondition.Checked)
                {
                    GetCourseCondition();           // コース状況を取得
                }

                txtCourseTantosyaCD.Text = "";
                lblCourseTantosyaName.Text = "";

                // コースに該当する担当者があれば、その担当者CD,名を表示する
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

                        // 同じ内容を注文ヘッダにも設定する
                        txtTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                        if (_chumonHeader != null)
                        {
                            _chumonHeader.担当者ＣＤ = courseTanto.担当者ＣＤ;
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
        /// コース状況チェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCourseCondition_CheckedChanged(object sender, EventArgs e)
        {
            grpCourseCondition.Visible = chkCourseCondition.Checked;
            if (chkCourseCondition.Checked)
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Width = WINDOW_WIDTH_COURSE_CONDITION;
                }
                GetCourseCondition();           // コース状況を取得
            }
            else
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Width = WINDOW_WIDTH_DEFAULT;
                }
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
                    if (_chumonHeader != null)
                    {
                        _chumonHeader.担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
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
        /// 前回入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                // 同一ログインユーザーがシステム日付に修正したレコードのうち、カレントレコードの直前に編集したレコードを取得
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    string sql = "\r\n SELECT" +
                                 "\r\n     喫食日" +
                                 "\r\n     ,得意先ＣＤ" +
                                 "\r\n     ,得意先名" +
                                 "\r\n     ,部署ＣＤ" +
                                 "\r\n FROM" +
                                 "\r\n     注文ヘッダ" +
                                 "\r\n WHERE" +
                                 "\r\n     企業ＣＤ = @企業ＣＤ AND" +
                                 "\r\n     注文区分 = @注文区分注文 AND" +
                                 "\r\n     修正担当者ＣＤ = @修正担当者ＣＤ AND" +
                                 "\r\n     修正日 = (SELECT" +
                                 "\r\n                     MAX(修正日) AS 修正日" +
                                 "\r\n                 FROM" +
                                 "\r\n                     注文ヘッダ" +
                                 "\r\n                 WHERE" +
                                 "\r\n                     企業ＣＤ = @企業ＣＤ AND" +
                                 "\r\n                     注文区分 = @注文区分注文 AND" +
                                 "\r\n                     修正担当者ＣＤ = @修正担当者ＣＤ AND" +
                                 "\r\n                     CONVERT(date, 修正日) = CONVERT(date, getdate())" +
                                 "\r\n                     {0}" +
                                 "\r\n                 )";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;;

                    string where = "";
                    // 修正日がある(=変更の)場合は、その修正日の直前のレコードを探す
                    if (_chumonHeader != null && _chumonHeader.修正日 != null)
                    {
                        where = "AND 修正日 < @修正日";
                        cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = (DateTime)_chumonHeader.修正日;
                    }
                    cmd.CommandText = string.Format(sql, where);

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("注文ヘッダ");
                    adp.Fill(dt);

                    log.Info("前回編集注文ヘッダを検索しました。");
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "前回データが見つかりませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 得意先ＣＤと部署ＣＤと喫食日を検索条件へ
                errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す
                errorProvider1.SetError(cboBusyo, null);
                errorProvider1.SetError(dtpChumonbi, null);

                _chumonCriteria.得意先ＣＤ = dt.Rows[0]["得意先ＣＤ"] as int?;
                txtTokuisakiCD.Text = _chumonCriteria.得意先ＣＤ.ToString();
                lblTokuisakiName.Text = dt.Rows[0]["得意先名"] as string;

                _chumonCriteria.部署ＣＤ = dt.Rows[0]["部署ＣＤ"] as int?;
                cboBusyo.SelectedValue = _chumonCriteria.部署ＣＤ;

                _chumonCriteria.注文日 = dt.Rows[0]["喫食日"] as DateTime?;
                dtpChumonbi.Text = string.Format("{0:yyyy/MM/dd（ddd）}", _chumonCriteria.注文日);
                ToggleChumonbiIsNotToday();     // 「本日ではありません」ラベルのVisibleを設定

                // 検索実行
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
        }

        /// <summary>
        /// 次回入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                // 修正日が無い(=新規)の場合はNOP
                if (_chumonHeader == null || _chumonHeader.修正日 == null)
                {
                    return;
                }

                // 同一ログインユーザーがシステム日付に修正したレコードのうち、カレントレコードの直後に編集したレコードを取得
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     喫食日" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n     ,得意先名" +
                                      "\r\n     ,部署ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     注文ヘッダ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n     注文区分 = @注文区分注文 AND" +
                                      "\r\n     修正担当者ＣＤ = @修正担当者ＣＤ AND" +
                                      "\r\n     修正日 = (SELECT" +
                                      "\r\n                     MIN(修正日) AS 修正日" +
                                      "\r\n                 FROM" +
                                      "\r\n                     注文ヘッダ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n                     注文区分 = @注文区分注文 AND" +
                                      "\r\n                     修正担当者ＣＤ = @修正担当者ＣＤ AND" +
                                      "\r\n                     CONVERT(date, 修正日) = CONVERT(date, getdate()) AND" +
                                      "\r\n                     修正日 > @修正日" +
                                      "\r\n                 )";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = UserData.getInstance().担当者ＣＤ;;
                    cmd.Parameters.Add("@修正日", SqlDbType.DateTime).Value = (DateTime)_chumonHeader.修正日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("注文ヘッダ");
                    adp.Fill(dt);

                    log.Info("次回編集注文ヘッダを検索しました。");
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "次回データが見つかりませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 得意先ＣＤと部署ＣＤと喫食日を検索条件へ
                errorProvider1.SetError(txtTokuisakiCD, null);  // エラーを消す
                errorProvider1.SetError(cboBusyo, null);
                errorProvider1.SetError(dtpChumonbi, null);

                _chumonCriteria.得意先ＣＤ = dt.Rows[0]["得意先ＣＤ"] as int?;
                txtTokuisakiCD.Text = _chumonCriteria.得意先ＣＤ.ToString();
                lblTokuisakiName.Text = dt.Rows[0]["得意先名"] as string;

                _chumonCriteria.部署ＣＤ = dt.Rows[0]["部署ＣＤ"] as int?;
                cboBusyo.SelectedValue = _chumonCriteria.部署ＣＤ;

                _chumonCriteria.注文日 = dt.Rows[0]["喫食日"] as DateTime?;
                dtpChumonbi.Text = string.Format("{0:yyyy/MM/dd（ddd）}", _chumonCriteria.注文日);
                ToggleChumonbiIsNotToday();     // 「本日ではありません」ラベルのVisibleを設定

                // 検索実行
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
        }

        /// <summary>
        /// 注文のみラジオボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoChumon_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stopRdoChumonClick == true)
                {
                    return;
                }

                // 明細表示を切り替える
                ToggleMeisai();
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
        /// 得意先単価を含めるラジオボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoTokuisakiTanka_Click(object sender, EventArgs e)
        {
            try
            {
                if (_stopRdoChumonClick == true)
                {
                    return;
                }

                // 明細表示を切り替える
                ToggleMeisai();
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
        /// 注文明細Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_ChumonMeisai_KeyDown(object sender, KeyEventArgs e)
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
                if (fpSpread_ChumonMeisai.EditMode)
                {
                    fpSpread_ChumonMeisai.CancelCellEditing();
                }
                else
                {
                    fpSpread_ChumonMeisai.StartCellEditing(e, false);   // セル編集を開始

                    // ↓ここでやっても意味が無い
                    //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_ChumonMeisai_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_ChumonMeisai_Sheet1.ActiveRowIndex;

                // 最終行(=新規行)で個数列以降であれば次のコントロールへフォーカスを移す
                if (newRowIndex >= fpSpread_ChumonMeisai_Sheet1.Rows.Count - 1 && newColumnIndex >= CHUMON_KOSU_COLUMN)
                {
                    this.SelectNextControl(fpSpread_ChumonMeisai, true, true, true, true);
                    return;
                }

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= CHUMON_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_ChumonMeisai_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_ChumonMeisai_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_ChumonMeisai_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_ChumonMeisai.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }

            // Deleteキー
            if (e.KeyCode == Keys.Delete)
            {
                int colIndex = -1;
                switch (fpSpread_ChumonMeisai_Sheet1.ActiveColumnIndex)
                {
                    case CHUMON_SYOHIN_CD_COLUMN:      // 商品ＣＤ
                    case CHUMON_SEIZOBI_COLUMN:        // 製造日
                    case CHUMON_TANKA_COLUMN:          // 単価
                    case CHUMON_KOSU_COLUMN:           // 個数
                        colIndex = fpSpread_ChumonMeisai_Sheet1.ActiveColumnIndex;
                        break;

                    default:
                        colIndex = -1;
                        break;
                }

                if (colIndex >= 0)
                {
                    fpSpread_ChumonMeisai_Sheet1.Cells[fpSpread_ChumonMeisai_Sheet1.ActiveRowIndex, colIndex].Value = null;     // dt_ColumnChangedが発生
                }
            }
        }

        /// <summary>
        /// 注文明細Spread編集モードON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_ChumonMeisai_EditModeOn(object sender, EventArgs e)
        {
            if (fpSpread_ChumonMeisai.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 編集モードに移行するF2キーが押下されたことによる編集モード開始？
                if (_isF2forSpreadEdit)
                {
                    // ☆で編集開始時に入力済みデータを選択状態にしているので、F2キーでのセル編集開始時は選択状態では無く末尾にカーソルを表示する
                    ((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread_ChumonMeisai.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                }

                //セルの編集開始のタイミングで各Keyイベントをハンドルします
                fpSpread_ChumonMeisai.EditingControl.KeyDown += new KeyEventHandler(this.fpSpread_ChumonMeisaiEdit_KeyDown);
            }
        }

        /// <summary>
        /// 注文明細Spread編集モードOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_ChumonMeisai_EditModeOff(object sender, EventArgs e)
        {
            if (fpSpread_ChumonMeisai.EditingControl is FarPoint.Win.Spread.CellType.GeneralEditor)
            {
                // 省略された日付文字列を補正する
                ParseDateText(fpSpread_ChumonMeisai, fpSpread_ChumonMeisai_Sheet1);

                //セルの編集終了のタイミングで各Keyイベントのハンドルを解除します
                fpSpread_ChumonMeisai.EditingControl.KeyDown -= new KeyEventHandler(this.fpSpread_ChumonMeisaiEdit_KeyDown);
            }
        }

        /// <summary>
        /// 注文明細Spread編集モードでのキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_ChumonMeisaiEdit_KeyDown(object sender, KeyEventArgs e)
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
                    if (fpSpread_ChumonMeisai.EditMode)
                    {
                        fpSpread_ChumonMeisai.CancelCellEditing();
                    }
                    else
                    {
                        fpSpread_ChumonMeisai.StartCellEditing(e, false);
                        //((FarPoint.Win.Spread.CellType.GeneralEditor)fpSpread1.EditingControl).EditModeCursorPosition = FarPoint.Win.EditModeCursorPosition.End;
                    }
                }
            }

            // Enterキー
            if (e.KeyCode == Keys.Enter)
            {
                int newColumnIndex = fpSpread_ChumonMeisai_Sheet1.ActiveColumnIndex;
                int newRowIndex = fpSpread_ChumonMeisai_Sheet1.ActiveRowIndex;

                // ボタン以外のセルタイプ かつ フォーカス可能なセルに移動する
                do
                {
                    // 最終列を超える場合は次の行へ移動する
                    if (newColumnIndex >= CHUMON_DELETE_COLUMN - 1)
                    {
                        newColumnIndex = 0;
                        newRowIndex++;
                        break;
                    }
                    newColumnIndex++;
                } while (fpSpread_ChumonMeisai_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                         fpSpread_ChumonMeisai_Sheet1.Columns[newColumnIndex].CanFocus == false);
                fpSpread_ChumonMeisai_Sheet1.SetActiveCell(newRowIndex, newColumnIndex);

                fpSpread_ChumonMeisai.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
            }
        }

        /// <summary>
        /// 注文明細Spreadボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_ChumonMeisai_ButtonClicked(object sender, FarPoint.Win.Spread.EditorNotifyEventArgs e)
        {
            try
            {
                // 参照ボタン？
                if (e.Column == CHUMON_REFERENCE_COLUMN)
                {
                    OpenForm901BySpread();          // 商品選択ダイアログを表示
                }

                // 削除ボタン？
                if (e.Column == CHUMON_DELETE_COLUMN)
                {
                    // 最終行（=新規行）の場合は何もしない
                    if (e.Row == fpSpread_ChumonMeisai_Sheet1.Rows.Count - 1)
                    {
                        return;
                    }
                    DataRow dr = ((DataRowView)chumonMeisaiBindingSource.Current).Row;
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
        /// 注文明細Spreadから商品選択ダイアログを表示する
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
                    int newColumnIndex = fpSpread_ChumonMeisai_Sheet1.ActiveColumnIndex;
                    do
                    {
                        newColumnIndex++;
                    } while (fpSpread_ChumonMeisai_Sheet1.Columns[newColumnIndex].CellType is FarPoint.Win.Spread.CellType.ButtonCellType ||
                             fpSpread_ChumonMeisai_Sheet1.Columns[newColumnIndex].CanFocus == false);
                    fpSpread_ChumonMeisai_Sheet1.SetActiveCell(fpSpread_ChumonMeisai_Sheet1.ActiveRowIndex, newColumnIndex);

                    Syohin syohin = form.SelectedSyohin;
                    fpSpread_ChumonMeisai_Sheet1.Cells[fpSpread_ChumonMeisai_Sheet1.ActiveRowIndex, CHUMON_SYOHIN_CD_COLUMN].Value = syohin.商品ＣＤ;   // 商品CDセルに値を設定→dt_ColumnChanged()が呼ばれる
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
        /// 注文明細Spread編集完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_ChumonMeisai_Change(object sender, FarPoint.Win.Spread.ChangeEventArgs e)
        {
            // 既存の行のRowStateが、アクティブセルが他の行に移らないと変わらないのを、強制的に変更する
            DataRow dr = ((DataRowView)chumonMeisaiBindingSource.Current).Row;
            dr.EndEdit();
        }

        /// <summary>
        /// 配送日テキスト 検証後
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpHaisobi_Validated(object sender, EventArgs e)
        {
            try
            {
                // 「注文日と異なります」ラベルのVisibleを設定
                ToggleHaisobiIsDifferentFromChumonbi();

                //*****************************
                // コースコンボの選択肢を再取得
                //*****************************

                // コースコンボ変更イベント処理を抑制
                // ※cboCourse.SelectedValueChanged -= cboCourse_SelectedValueChanged; は上手くいかない
                _stopCboCourseSelectedIndexChanged = true;

                // 配送日が未入力の場合は何もしない（注文入力コース情報を取得でのエラーを回避する）
                if (_chumonHeader.配送日 == null)
                {
                    return;
                }

                // 注文入力コース情報を取得
                GetChumonCourse();

                // コースコンボ初期値設定
                SetComboCourse(true);

                // コース状況を取得
                GetCourseCondition();
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
                // コースコンボ変更イベントを処理を再開
                // ※cboCourse.SelectedValueChanged += cboCourse_SelectedValueChanged; は上手くいかない
                _stopCboCourseSelectedIndexChanged = false;
                _CboCourseSelectedIndex = cboCourse.SelectedIndex;
            }
        }

        /// <summary>
        /// 注文入力時残高表示ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZandaka_Click(object sender, EventArgs e)
        {
            try
            {
                // 残高を取得
                _chumonZandaka.GetZandaka(_chumonCriteria);
                chumonZandakaBindingSource.ResetBindings(false);
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
        /// 登録後に画面をクリアするチェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkClearWhenSave_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // 部署と注文日をスキップする
                // ※登録→クリアして再び新規入力する際に、得意先だけを変えて（部署と注文日は変えずに）新規入力する
                cboBusyo.TabStop = !(chkClearWhenSave.Checked);
                dtpChumonbi.TabStop = !(chkClearWhenSave.Checked);
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
        /// グループ検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGroupOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_chumonCriteriaSnapShot == null)
                {
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_chumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _chumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_chumonCriteria.Equals(_chumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 入力中の得意先と同じ電話番号を持つ得意先をC_TelToCustから取得し、その分の注文入力画面を表示する
                DataTable dt = null;
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT DISTINCT" +
                                      "\r\n     Tel_CustNo" +
                                      "\r\n FROM" +
                                      "\r\n     C_TelToCust" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n     Tel_DelFlg = 0 AND" +
                                      "\r\n     Tel_CustNo <> @得意先ＣＤ AND" +
                                      "\r\n     Tel_TelNo in (SELECT" +
                                      "\r\n                         Tel_TelNo" +
                                      "\r\n                     FROM" +
                                      "\r\n                         C_TelToCust" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         企業ＣＤ = @企業ＣＤ AND" +
                                      "\r\n                         Tel_CustNo = @得意先ＣＤ AND" +
                                      "\r\n                         Tel_DelFlg = 0" +
                                      "\r\n                     )" +
                                      "\r\n ORDER BY" +
                                      "\r\n     Tel_CustNo";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _chumonCriteria.得意先ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("C_TelToCust");
                    adp.Fill(dt);

                    log.Info("C_TelToCustを検索しました。");
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "他の得意先が見つかりません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 取得した得意先の分だけ注文入力画面を開く
                // ・同じ得意先の画面を開くのは一度だけ。つまりボタンが複数回クリックされても、閉じた分のみ開く
                // ・ここで開かれたフォームでは、グループ検索ボタンは使用不可とする
                foreach (DataRow dr in dt.Rows)
                {
                    ChumonMeisaiCriteria chumonCriteria;
                    Form200 formChumon;
                    int tokuisakiCD = (int)dr["Tel_CustNo"];

                    // 子フォーム生成済み？
                    if (_form200GroupMap.ContainsKey(tokuisakiCD))
                    {
                        formChumon = _form200GroupMap[tokuisakiCD];

                        // 子フォームが閉じられている？
                        if (formChumon == null || formChumon.IsDisposed)
                        {
                            formChumon = null;
                        }
                    }
                    else
                    {
                        formChumon = null;     // 子フォーム未生成
                    }

                    // 子フォームを生成
                    if (formChumon == null)
                    {
                        chumonCriteria = new ChumonMeisaiCriteria();
                        chumonCriteria.得意先ＣＤ = tokuisakiCD;
                        chumonCriteria.注文日 = _chumonCriteria.注文日;
                        formChumon = new Form200(chumonCriteria);
                        formChumon.GroupOpened = true;
                        _form200GroupMap[tokuisakiCD] = formChumon;     // マップに格納
                    }

                    // 子フォームを表示
                    formChumon.MdiParent = this.MdiParent;
                    formChumon.StartPosition = FormStartPosition.WindowsDefaultLocation;
                    formChumon.Show();
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
        /// グループ終了ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGroupClose_Click(object sender, EventArgs e)
        {
            try
            {
                // 注文入力画面マップ（子フォームマップ）に残っている子フォームを全て閉じる
                // コレクションをループして取得した子フォームを閉じると、子フォーム側でコレクションから自分の要素を削除している。
                // したがって InvalidOperationException が発生してしまう。
                // コレクションの末尾から要素を取得して削除することで、これを回避する。
                for (int i = _form200GroupMap.Count - 1; i >= 0; i--)
                {
                    Form200 formChumon = _form200GroupMap.Values[i];
                    formChumon.Close();
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
                // 画面をクリアし、新規レコードとする
                Clear();
                _IsNewRecord = true;

                btnSave.Text = "登録(F9)";
                btnPrev.Enabled = true;     // 前回入力ボタンは活性
                btnNext.Enabled = false;    // 次回入力ボタンは非活性
                btnDelete.Enabled = false;

                // 「登録後に画面をクリアする」は得意先ＣＤのみクリアする（部署と注文日はそのまま残す）が、
                // ここでは部署と得意先ＣＤを消して注文日はシステム日付にする
                _chumonCriteria = new ChumonMeisaiCriteria();
                _chumonCriteria.注文日 = DateTime.Today;
                chumonCriteriaBindingSource.DataSource = _chumonCriteria;

                // 未検索状態にする
                _chumonCriteriaSnapShot = null;

                // 「本日ではありません」ラベルのVisibleを設定
                lblChumonbiIsNotToday.Visible = false;

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
        /// 一括入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIkkatsu_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_chumonCriteriaSnapShot == null)
                {
                    return;
                }

                // 一括注文入力を開く
                IkkatsuChumonCriteria ikkatsuChumonCriteria = new IkkatsuChumonCriteria();
                ikkatsuChumonCriteria.得意先ＣＤ = _chumonCriteriaSnapShot.得意先ＣＤ;
                ikkatsuChumonCriteria.部署ＣＤ = _chumonCriteriaSnapShot.部署ＣＤ;
                ikkatsuChumonCriteria.注文年 = ((DateTime)_chumonCriteriaSnapShot.注文日).Year;
                ikkatsuChumonCriteria.注文月 = ((DateTime)_chumonCriteriaSnapShot.注文日).Month;
                Form202 form = new Form202(ikkatsuChumonCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "一括注文入力画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
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
        /// 売上入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUriage_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_chumonCriteriaSnapShot == null)
                {
                    return;
                }

                // 関連する売上を開く
                UriageMeisaiCriteria uriageMeisaiCriteria = new UriageMeisaiCriteria();
                uriageMeisaiCriteria.stopOverwrite = true;
                uriageMeisaiCriteria.得意先ＣＤ = _chumonCriteriaSnapShot.得意先ＣＤ;
                uriageMeisaiCriteria.得意先名 = _chumonCriteriaSnapShot.得意先名;
                uriageMeisaiCriteria.部署ＣＤ = _chumonCriteriaSnapShot.部署ＣＤ;
                uriageMeisaiCriteria.売上日 = _chumonCriteriaSnapShot.注文日;
                uriageMeisaiCriteria.コースＣＤ = _chumonHeader.コースＣＤ;       // デフォルト表示するコース
                Form401 form = new Form401(uriageMeisaiCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
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
                // 注文ヘッダの入力チェック
                if (CanSaveChumonHeader() == false)
                {
                    return;
                }

                // 注文明細の入力チェック
                if (CanSaveChumonMeisai() == false)
                {
                    return;
                }

                // 既に売上がある場合は警告メッセージを表示
                // ※ただし、配送済みであることが分かっている場合はNOP
                if (_chumonTokuisaki.配送状態 != "配達済")
                {
                    // 売上の存在をチェック
                    UriageHeader uriageHeader = UriageHeader.GetUriageHeader((int)_chumonHeader.得意先ＣＤ, (DateTime)_chumonHeader.喫食日, (int)_chumonHeader.部署ＣＤ);
                    if (uriageHeader != null)
                    {
                        // 売上があれば「配達済」を表示
                        _chumonTokuisaki.配送状態 = "配達済";
                        ReadBinding(lblHaisoJotai);     // コントロールに反映
                        // 配送状態を着色
                        SetHaisoJotaiColor(_chumonTokuisaki.配送状態);

                        if (MessageBox.Show(this, "既に売上が存在します。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                int ret = Save();

                this.MdiParent.Cursor = Cursors.Default;

                if (ret >= 0)
                {
                    if (chkClearWhenSave.Checked)
                    {
                        // 画面をクリアし、新規レコードとする
                        Clear();
                        _IsNewRecord = true;

                        btnSave.Text = "登録(F9)";
                        btnPrev.Enabled = true;     // 前回入力ボタンは活性
                        btnNext.Enabled = false;    // 次回入力ボタンは非活性
                        btnDelete.Enabled = false;

                        // 得意先ＣＤのみクリアする（部署と注文日はそのまま残す）
                        _chumonCriteria.得意先ＣＤ = null;
                        // 得意先ＣＤと名称を表示に反映
                        ReadBinding(txtTokuisakiCD);
                        ReadBinding(lblTokuisakiName);

                        // 未検索状態にする
                        _chumonCriteriaSnapShot = null;

                        txtTokuisakiCD.Focus();
                    }
                    else
                    {
                        // 画面を閉じるが、その前に再読込して「変更があります」通知を出さないようにする
                        Reload();
                        this.Close();
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
        /// 備考コンボ作成
        /// </summary>
        private void SetBikoCombo()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     -1 AS 備考CD" +
                                      "\r\n     ,'' AS 備考 " +
                                      "\r\n UNION " +
                                      "\r\n SELECT" +
                                      "\r\n     備考CD" +
                                      "\r\n    ,備考" +
                                      "\r\n FROM" +
                                      "\r\n     備考マスタ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n ORDER BY" +
                                      "\r\n     備考CD";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    _dtBiko = new DataTable();
                    adp.Fill(_dtBiko);
                }

                cboBiko1.DisplayMember = "備考";
                cboBiko1.ValueMember = "備考";
                cboBiko1.DataSource = _dtBiko.Copy();

                cboBiko2.DisplayMember = "備考";
                cboBiko2.ValueMember = "備考";
                cboBiko2.DataSource = _dtBiko.Copy();

                cboBiko3.DisplayMember = "備考";
                cboBiko3.ValueMember = "備考";
                cboBiko3.DataSource = _dtBiko.Copy();

                cboBiko4.DisplayMember = "備考";
                cboBiko4.ValueMember = "備考";
                cboBiko4.DataSource = _dtBiko.Copy();

                cboBiko5.DisplayMember = "備考";
                cboBiko5.ValueMember = "備考";
                cboBiko5.DataSource = _dtBiko.Copy();
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
        /// 注文入力得意先情報を取得
        /// </summary>
        private void GetChumonTokuisaki()
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
                                      "\r\n     ,ISNULL(TC.Tel_TelNo, ISNULL(T.電話番号1, T.電話番号2)) AS 電話番号" +
                                      "\r\n     ,TC.Tel_Remarks AS 電話番号備考" +
                                      "\r\n     ,T.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n     ,B.支店ＣＤ" +
                                      "\r\n     ,S.支店名" +
                                      "\r\n     ,ISNULL(T.税区分, 1) AS 税区分" +                   // 初期値は1:内税
                                      "\r\n     ,ISNULL(T.丸め処理区分, 2) AS 丸め処理区分" +       // 初期値は2:切り捨て
                                      "\r\n     ,ISNULL(T.売掛現金区分, 0) AS 売掛現金区分" +       // 初期値は0:現金
                                      "\r\n     ,T.受注得意先ＣＤ" +
                                      "\r\n     ,CASE WHEN H.売上日 IS NULL THEN '未配達' ELSE '配達済' END AS 配送状態" +
                                      "\r\n     ,T.備考1" +
                                      "\r\n     ,T.備考2" +
                                      "\r\n     ,T.備考3" +
                                      "\r\n     ,T.受注時特記事項" +
                                      "\r\n     ,T.休日設定" +
                                      "\r\n FROM" +
                                      "\r\n     得意先マスタ AS T" +
                                      // -- 主であり、最も若い電話番号(Tel_TelNo)をC_TelToCust(AS TTC)から取得する。
                                      // -- その備考(Tel_Remarks)を取得すべく、再度C_TelToCust(AS TC)と結合している。
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             企業ＣＤ" +
                                      "\r\n             ,Tel_CustNo" +
                                      "\r\n             ,MIN(Tel_TelNo) AS Tel_TelNo" +
                                      "\r\n         FROM" +
                                      "\r\n            C_TelToCust" +
                                      "\r\n         WHERE" +
                                      "\r\n             Tel_RepFlg = 0" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             企業ＣＤ" +
                                      "\r\n             ,Tel_CustNo" +
                                      "\r\n        ) AS TTC" +
                                      "\r\n            ON TTC.企業ＣＤ = T.企業ＣＤ" +
                                      "\r\n            AND TTC.Tel_CustNo = T.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN C_TelToCust AS TC ON TC.企業ＣＤ = TTC.企業ＣＤ AND TC.Tel_CustNo = TTC.Tel_CustNo AND TC.Tel_TelNo = TTC.Tel_TelNo" +
                                      "\r\n     LEFT JOIN 部署マスタ  AS B  ON B.企業ＣＤ  = T.企業ＣＤ AND B.部署ＣＤ = T.部署ＣＤ AND ISNULL(B.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN 支店マスタ  AS S  ON S.企業ＣＤ  = B.企業ＣＤ AND S.支店ＣＤ = B.支店ＣＤ AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN 売上ヘッダ  AS H  ON H.企業ＣＤ  = T.企業ＣＤ AND H.得意先ＣＤ = T.得意先ＣＤ AND H.売上日 = @売上日 AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     T.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND ISNULL(T.無効フラグ, 0) = 0" +
                                      "\r\n     AND T.得意先ＣＤ = @得意先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _chumonCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@売上日", SqlDbType.Date).Value = _chumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_chumonCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("注文入力得意先情報");
                    adp.Fill(dt);

                    _chumonTokuisaki = new ChumonTokuisaki();
                    if (dt.Rows.Count != 0)
                    {
                        _chumonTokuisaki = new ChumonTokuisaki(dt.Rows[0]);
                    }

                    chumonTokuisakiBindingSource.DataSource = _chumonTokuisaki;

                    // タイトルバーに得意先名略称を表示
                    if (string.IsNullOrEmpty(_chumonTokuisaki.得意先名略称))
                    {
                        this.Text = "注文入力";
                    }
                    else
                    {
                        this.Text = string.Format("{0} - 注文入力", _chumonTokuisaki.得意先名略称);
                    }

                    // 売掛現金区分、税区分、丸め処理区分を表示
                    toggleLabelColor(lblGenkin, GENKIN_COLOR, (_chumonTokuisaki.売掛現金区分 == (int)Consts.UriGenKubun.Genkin));
                    toggleLabelColor(lblKakeuri, KAKEURI_COLOR, (_chumonTokuisaki.売掛現金区分 == (int)Consts.UriGenKubun.Urikake));
                    toggleLabelColor(lblSotozei, SOTOZEI_COLOR, (_chumonTokuisaki.税区分 == (int)Syohizei.ZeiKubun.Soto));
                    toggleLabelColor(lblHikazei, HIKAZEI_COLOR, (_chumonTokuisaki.税区分 == (int)Syohizei.ZeiKubun.Hikazei));

                    // 丸め区分は外税の場合のみ表示
                    if (_chumonTokuisaki.税区分 == (int)Syohizei.ZeiKubun.Soto)
                    {
                        toggleLabelColor(lblCeiling, CEILING_COLOR, (_chumonTokuisaki.丸め処理区分 == (int)Rounding.HasuKubun.Ceiling));
                        toggleLabelColor(lblFloor, FLOOR_COLOR, (_chumonTokuisaki.丸め処理区分 == (int)Rounding.HasuKubun.Floor));
                        toggleLabelColor(lblRound, ROUND_COLOR, (_chumonTokuisaki.丸め処理区分 == (int)Rounding.HasuKubun.Round));
                    }
                    else
                    {
                        toggleLabelColor(lblCeiling, CEILING_COLOR, false);
                        toggleLabelColor(lblFloor, FLOOR_COLOR, false);
                        toggleLabelColor(lblRound, ROUND_COLOR, false);
                    }

                    // 配送状態を着色
                    SetHaisoJotaiColor(_chumonTokuisaki.配送状態);

                    // 取得した備考1,2,3を、該当するコンボに追加
                    if (string.IsNullOrEmpty(_chumonTokuisaki.備考1) == false)
                    {
                        AddBiko(cboBiko1, _chumonTokuisaki.備考1);
                    }

                    if (string.IsNullOrEmpty(_chumonTokuisaki.備考2) == false)
                    {
                        AddBiko(cboBiko2, _chumonTokuisaki.備考2);
                    }

                    if (string.IsNullOrEmpty(_chumonTokuisaki.備考3) == false)
                    {
                        AddBiko(cboBiko3, _chumonTokuisaki.備考3);
                    }

                    log.Info("注文入力得意先情報を検索しました。");
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
        /// 注文ヘッダ取得
        /// </summary>
        private void GetChumonHeader()
        {
            try
            {
                // 注文ヘッダを取得
                _chumonHeader = ChumonHeader.GetChumonHeader(Consts.ChumonKubun.Chuumon, (DateTime)_chumonCriteria.注文日, (int)_chumonCriteria.得意先ＣＤ, (int)_chumonCriteria.部署ＣＤ);
                if (_chumonHeader == null)
                {
                    _IsNewRecord = true;        // 新規レコード

                    // 存在しない場合は初期化
                    _chumonHeader = new ChumonHeader();
                    _chumonHeader.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                    _chumonHeader.喫食日 = _chumonCriteria.注文日;
                    _chumonHeader.得意先ＣＤ = _chumonCriteria.得意先ＣＤ;
                    _chumonHeader.部署ＣＤ = _chumonCriteria.部署ＣＤ;
                    _chumonHeader.配送日 = CalcHaisobi((DateTime)_chumonCriteria.注文日, _busyo, _chumonTokuisaki);
                    _chumonHeader.得意先名 = _chumonCriteria.得意先名;
                    _chumonHeader.担当者ＣＤ = null;
                    _chumonHeader.コースＣＤ = null;
                    _chumonHeader.配送先ＣＤ = _chumonTokuisaki.受注得意先ＣＤ;
                    _chumonHeader.税区分 = _chumonTokuisaki.税区分;
                    _chumonHeader.丸め処理区分 = _chumonTokuisaki.丸め処理区分;
                    _chumonHeader.納品書伝票番号 = null;
                }
                else
                {
                    _IsNewRecord = false;
                    // 「注文のみ」にする
                    rdoChumon.Checked = true;
                }

                // 取得した備考を、該当するコンボに追加
                if (string.IsNullOrEmpty(_chumonHeader.備考1) == false)
                {
                    AddBiko(cboBiko1, _chumonHeader.備考1);
                }

                if (string.IsNullOrEmpty(_chumonHeader.備考2) == false)
                {
                    AddBiko(cboBiko2, _chumonHeader.備考2);
                }

                if (string.IsNullOrEmpty(_chumonHeader.備考3) == false)
                {
                    AddBiko(cboBiko3, _chumonHeader.備考3);
                }

                if (string.IsNullOrEmpty(_chumonHeader.備考4) == false)
                {
                    AddBiko(cboBiko4, _chumonHeader.備考4);
                }

                if (string.IsNullOrEmpty(_chumonHeader.備考5) == false)
                {
                    AddBiko(cboBiko5, _chumonHeader.備考5);
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
        /// 注文入力コース情報を取得
        /// </summary>
        private void GetChumonCourse()
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

                DataTable dt = GetChumonCourseMain(sql);
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
                    dt = GetChumonCourseMain(sql);
                }

                // コースに該当する担当者のマップを生成
                // ※ついでに、検索条件の部署ＣＤと一致するコースＣＤを取得しておく
                _defaultCourseCD = -1;
                _courseTantoMap.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    CourseTanto courseTanto = new CourseTanto(dr["担当者ＣＤ"] as int?, dr["担当者名"] as string);
                    _courseTantoMap[(int)dr["コースＣＤ"]] = courseTanto;

                    if (_defaultCourseCD == -1 && _chumonCriteria.部署ＣＤ == (int)dr["部署ＣＤ"])
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

                log.Info("注文入力コース情報を検索しました。");
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
        /// 注文入力コース情報取得主処理
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>コース情報のデータテーブル</returns>
        private DataTable GetChumonCourseMain(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _chumonHeader.得意先ＣＤ;
                    cmd.Parameters.Add("@実施日", SqlDbType.Date).Value = _chumonHeader.配送日;

                    // 配送日(の曜日)からコース区分を求める
                    int courseKubun = Course.GetCourseKubun((DateTime)_chumonHeader.配送日);
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = courseKubun;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("注文入力コース情報");
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
        /// <param name="isDtpHaisobiValidated">配送日が変わったイベントによる呼び出しの場合はtrue</param>
        private void SetComboCourse(bool isDtpHaisobiValidated)
        {
            try
            {
                if (_IsNewRecord)
                {
                    // 得意先から求めたコースを初期値として持つ
                    // コースから取得した担当者を初期値として持つ
                    if (_defaultCourseCD > 0)
                    {
                        _chumonHeader.コースＣＤ = _defaultCourseCD;

                        if (_courseTantoMap.ContainsKey((int)_chumonHeader.コースＣＤ))
                        {
                            CourseTanto courseTanto = _courseTantoMap[(int)_chumonHeader.コースＣＤ];
                            if (courseTanto.担当者ＣＤ != null)
                            {
                                _chumonHeader.担当者ＣＤ = courseTanto.担当者ＣＤ;
                            }
                        }
                    }

                    // コース注文入力／コース注文一覧／受注発信一覧から注文入力を開いたときは、指定されたコースをデフォルト表示する
                    if (_chumonCriteria.コースＣＤ != null)
                    {
                        _chumonHeader.コースＣＤ = _chumonCriteria.コースＣＤ;
                        if (_courseTantoMap.ContainsKey((int)_chumonHeader.コースＣＤ))
                        {
                            CourseTanto courseTanto = _courseTantoMap[(int)_chumonHeader.コースＣＤ];
                            if (courseTanto.担当者ＣＤ != null)
                            {
                                _chumonHeader.担当者ＣＤ = courseTanto.担当者ＣＤ;
                            }
                        }
                    }
                }

                if (_chumonHeader.コースＣＤ != null)
                {
                    Course course = Course.GetCourse((int)_chumonHeader.コースＣＤ);
                    if (course != null)
                    {
                        // 取得したコースＣＤがコースコンボに存在していなければ追加する
                        // ※一括注文入力で、その得意先とは無関係のコースを指定できるので
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
                    if (_courseTantoMap.ContainsKey((int)_chumonHeader.コースＣＤ))
                    {
                        CourseTanto courseTanto = _courseTantoMap[(int)_chumonHeader.コースＣＤ];
                        if (courseTanto.担当者ＣＤ != null)
                        {
                            txtCourseTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                            lblCourseTantosyaName.Text = courseTanto.担当者名;

                            // 配送日が変わったイベントによる呼び出しの場合のみ
                            // （既存のデータを表示する場合はそれを優先する）
                            if (isDtpHaisobiValidated)
                            {
                                // 同じ内容を注文ヘッダにも設定する
                                txtTantosyaCD.Text = courseTanto.担当者ＣＤ.ToString();
                                if (_chumonHeader != null)
                                {
                                    _chumonHeader.担当者ＣＤ = courseTanto.担当者ＣＤ;
                                }
                                lblTantosyaName.Text = courseTanto.担当者名;
                            }
                        }
                    }

                    cboCourse.SelectedValue = (int)_chumonHeader.コースＣＤ;
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
        /// 注文明細取得
        /// </summary>
        private void GetChumonMeisai()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // 注文のみ？
                    if (rdoChumon.Checked)
                    {
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n     M.企業ＣＤ" +
                                          "\r\n     ,0 AS 注文区分" +
                                          "\r\n     ,M.喫食日" +
                                          "\r\n     ,M.得意先ＣＤ" +
                                          "\r\n     ,M.商品ＣＤ" +
                                          "\r\n     ,M.商品ＣＤ AS 商品ＣＤorg" +
                                          "\r\n     ,S.商品名" +
                                          "\r\n     ,M.部署ＣＤ" +
                                          "\r\n     ,ISNULL(M0.明細番号         ,M1.明細番号)         AS 明細番号" +
                                          "\r\n     ,M0.製造日" +
                                          "\r\n     ,ISNULL(M0.商品グループＣＤ ,M1.商品グループＣＤ) AS 商品グループＣＤ" +
                                          "\r\n     ,ISNULL(M0.売掛現金区分     ,ISNULL(K.売掛現金区分, 0))      AS 売掛現金区分" +
                                          "\r\n     ,ISNULL(M0.商品単価         ,M1.商品単価)         AS 商品単価" +
                                          "\r\n     ,M1.商品数量 AS 予定数" +
                                          "\r\n     ,M0.商品数量" +
                                          "\r\n     ,M0.商品金額合計" +
                                          "\r\n     ,M0.消費税額" +
                                          "\r\n     ,M0.商品金額合計税込" +
                                          "\r\n     ,ISNULL(M0.計算税率, CASE WHEN ISNULL(S.軽減税率フラグ, 0) = 0 THEN TAX.消費税率 ELSE ISNULL(TAX.軽減税率, TAX.消費税率) END) AS 計算税率" +
                                          "\r\n     ,M0.作成担当者ＣＤ" +
                                          "\r\n     ,A1.担当者名 AS 作成担当者名" +
                                          "\r\n     ,M0.作成日" +
                                          "\r\n     ,M0.修正担当者ＣＤ" +
                                          "\r\n     ,A2.担当者名 AS 修正担当者名" +
                                          "\r\n     ,M0.修正日" +
                                          "\r\n FROM" +
                                          // -- ①駆動表（注文と予測の注文明細）
                                          "\r\n     (SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,喫食日" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n         ,商品ＣＤ" +
                                          "\r\n         ,部署ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文明細" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ       = @企業ＣＤ" +
                                          "\r\n         AND 注文区分   = @注文区分注文" +
                                          "\r\n         AND 喫食日     = @喫食日" +
                                          "\r\n         AND 得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n         AND 部署ＣＤ   = @部署ＣＤ" +
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         企業ＣＤ" +
                                          "\r\n         ,喫食日" +
                                          "\r\n         ,得意先ＣＤ" +
                                          "\r\n         ,商品ＣＤ" +
                                          "\r\n         ,部署ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文明細" +
                                          "\r\n     WHERE" +
                                          "\r\n         企業ＣＤ       = @企業ＣＤ" +
                                          "\r\n         AND 注文区分   = @注文区分予測" +
                                          "\r\n         AND 喫食日     = @喫食日" +
                                          "\r\n         AND 得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n         AND 部署ＣＤ   = @部署ＣＤ" +
                                          "\r\n     ) AS M" +
                                          // -- ②注文明細（注文）
                                          "\r\n     LEFT JOIN" +
                                          "\r\n         (SELECT" +
                                          "\r\n             企業ＣＤ" +
                                          "\r\n             ,注文区分" +
                                          "\r\n             ,喫食日" +
                                          "\r\n             ,得意先ＣＤ" +
                                          "\r\n             ,商品ＣＤ" +
                                          "\r\n             ,部署ＣＤ" +
                                          "\r\n             ,明細番号" +
                                          "\r\n             ,製造日" +
                                          "\r\n             ,商品グループＣＤ" +
                                          "\r\n             ,売掛現金区分" +
                                          "\r\n             ,商品単価" +
                                          "\r\n             ,商品数量" +
                                          "\r\n             ,商品金額合計" +
                                          "\r\n             ,消費税額" +
                                          "\r\n             ,商品金額合計税込" +
                                          "\r\n             ,計算税率" +
                                          "\r\n             ,作成担当者ＣＤ" +
                                          "\r\n             ,作成日" +
                                          "\r\n             ,修正担当者ＣＤ" +
                                          "\r\n             ,修正日" +
                                          "\r\n         FROM" +
                                          "\r\n             注文明細" +
                                          "\r\n         WHERE" +
                                          "\r\n             企業ＣＤ       = @企業ＣＤ" +
                                          "\r\n             AND 注文区分   = @注文区分注文" +
                                          "\r\n             AND 喫食日     = @喫食日" +
                                          "\r\n             AND 得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n             AND 部署ＣＤ   = @部署ＣＤ" +
                                          "\r\n         ) AS M0" +
                                          "\r\n         ON M0.企業ＣＤ    = M.企業ＣＤ" +
                                          "\r\n         AND M0.喫食日     = M.喫食日" +
                                          "\r\n         AND M0.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         AND M0.商品ＣＤ   = M.商品ＣＤ" +
                                          "\r\n         AND M0.部署ＣＤ   = M.部署ＣＤ" +
                                          // -- ②注文明細（予測）
                                          "\r\n     LEFT JOIN" +
                                          "\r\n         (SELECT" +
                                          "\r\n             企業ＣＤ" +
                                          "\r\n             ,注文区分" +
                                          "\r\n             ,喫食日" +
                                          "\r\n             ,得意先ＣＤ" +
                                          "\r\n             ,商品ＣＤ" +
                                          "\r\n             ,部署ＣＤ" +
                                          "\r\n             ,明細番号" +
                                          "\r\n             ,製造日" +
                                          "\r\n             ,商品グループＣＤ" +
                                          "\r\n             ,売掛現金区分" +
                                          "\r\n             ,商品単価" +
                                          "\r\n             ,商品数量" +
                                          "\r\n             ,商品金額合計" +
                                          "\r\n             ,消費税額" +
                                          "\r\n             ,商品金額合計税込" +
                                          "\r\n             ,計算税率" +
                                          "\r\n             ,作成担当者ＣＤ" +
                                          "\r\n             ,作成日" +
                                          "\r\n             ,修正担当者ＣＤ" +
                                          "\r\n             ,修正日" +
                                          "\r\n         FROM" +
                                          "\r\n             注文明細" +
                                          "\r\n         WHERE" +
                                          "\r\n             企業ＣＤ       = @企業ＣＤ" +
                                          "\r\n             AND 注文区分   = @注文区分予測" +
                                          "\r\n             AND 喫食日     = @喫食日" +
                                          "\r\n             AND 得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n             AND 部署ＣＤ   = @部署ＣＤ" +
                                          "\r\n         ) AS M1" +
                                          "\r\n         ON M1.企業ＣＤ    = M.企業ＣＤ" +
                                          "\r\n         AND M1.喫食日     = M.喫食日" +
                                          "\r\n         AND M1.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n         AND M1.商品ＣＤ   = M.商品ＣＤ" +
                                          "\r\n         AND M1.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n     LEFT JOIN 得意先マスタ AS K  ON K.企業ＣＤ = M.企業ＣＤ AND K.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n     LEFT JOIN 商品マスタ   AS S  ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ" +
                                          "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = M0.企業ＣＤ AND M0.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                          "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = M0.企業ＣＤ AND M0.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             消費税率," +
                                          "\r\n             軽減税率" +
                                          "\r\n         FROM" +
                                          "\r\n             消費税率マスタ" +
                                          "\r\n         WHERE" +
                                          "\r\n             施行日 = (SELECT MAX(施行日) FROM 消費税率マスタ WHERE 施行日 <= @喫食日)" +
                                          "\r\n     ) AS TAX ON 1 = 1" +
                                          "\r\n ORDER BY" +
                                          "\r\n     明細番号";
                    }
                    else
                    {
                        // 得意先単価を含める
                        cmd.CommandText = "\r\n SELECT" +
                                          "\r\n      A.企業ＣＤ" +
                                          "\r\n      ,A.注文区分" +
                                          "\r\n      ,A.喫食日" +
                                          "\r\n      ,A.得意先ＣＤ" +
                                          "\r\n      ,X.商品ＣＤ" +
                                          "\r\n      ,X.商品ＣＤ AS 商品ＣＤorg" +
                                          "\r\n      ,ST.商品名" +
                                          "\r\n      ,ST.弁当区分" +
                                          "\r\n      ,A.部署ＣＤ" +
                                          "\r\n      ,ISNULL(A.明細番号, B.明細番号) AS 明細番号" +
                                          "\r\n      ,A.製造日" +
                                          "\r\n      ,ISNULL(ISNULL(A.商品グループＣＤ, B.商品グループＣＤ), SS.商品グループＣＤ) AS 商品グループＣＤ" +
                                          "\r\n      ,ISNULL(ISNULL(A.売掛現金区分, B.売掛現金区分), ISNULL(K.売掛現金区分, 0)) AS 売掛現金区分" +  // 初期値は0:現金
                                          "\r\n      ,ISNULL(ISNULL(A.商品単価, B.商品単価), CASE @税区分 WHEN 1 THEN T.商品単価税込 ELSE T.商品単価税抜 END) AS 商品単価" +
                                          "\r\n      ,A.商品数量" +
                                          "\r\n      ,A.商品金額合計" +
                                          "\r\n      ,B.商品数量 AS 予定数" +
                                          "\r\n      ,A.消費税額" +
                                          "\r\n      ,A.商品金額合計税込" +
                                          "\r\n      ,ISNULL(A.計算税率, CASE WHEN ISNULL(ST.軽減税率フラグ, 0) = 0 THEN TAX.消費税率 ELSE ISNULL(TAX.軽減税率, TAX.消費税率) END) AS 計算税率" +
                                          "\r\n      ,A.作成担当者ＣＤ" +
                                          "\r\n      ,A1.担当者名 AS 作成担当者名" +
                                          "\r\n      ,A.作成日" +
                                          "\r\n      ,A.修正担当者ＣＤ" +
                                          "\r\n      ,A2.担当者名 AS 修正担当者名" +
                                          "\r\n      ,A.修正日" +
                                          "\r\n FROM" +
                                          // -- ①得意先単価マスタと注文明細から、対象となる商品ＣＤを取得
                                          "\r\n     (SELECT" +
                                          "\r\n         TT.商品ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         得意先単価マスタ AS TT" +
                                          "\r\n         INNER JOIN 商品マスタ AS ST ON" +
                                          "\r\n             ST.企業ＣＤ = TT.企業ＣＤ" +
                                          "\r\n             AND ST.商品ＣＤ = TT.商品ＣＤ" +
                                          "\r\n     WHERE" +
                                          "\r\n         TT.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         TT.得意先ＣＤ = @得意先ＣＤ AND" +
                                          "\r\n         ISNULL(TT.無効フラグ, 0) = 0 AND" +
                                          "\r\n         ISNULL(ST.無効フラグ, 0) = 0" +
                                          "\r\n     UNION" +
                                          "\r\n     SELECT" +
                                          "\r\n         AA.商品ＣＤ" +
                                          "\r\n     FROM" +
                                          "\r\n         注文明細 AS AA" +
                                          "\r\n     WHERE" +
                                          "\r\n         AA.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n     AND AA.喫食日 = @喫食日" +
                                          "\r\n     AND AA.得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n     AND AA.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n     ) AS X" +
                                          // -- ② ①と得意先単価マスタを外部結合
                                          "\r\n     LEFT JOIN 得意先単価マスタ AS T ON" +
                                          "\r\n         T.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND T.得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n         AND ISNULL(T.無効フラグ, 0) = 0" +
                                          "\r\n         AND T.商品ＣＤ = X.商品ＣＤ" +
                                          // -- ③ ①と注文明細（注文）を外部結合
                                          "\r\n     LEFT JOIN 注文明細 AS A ON" +
                                          "\r\n         A.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND A.注文区分 = @注文区分注文" +
                                          "\r\n         AND A.喫食日 = @喫食日" +
                                          "\r\n         AND A.得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n         AND A.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         AND A.商品ＣＤ = X.商品ＣＤ" +
                                          // -- ④ ①と注文明細（予測）を外部結合
                                          "\r\n     LEFT JOIN 注文明細 AS B  ON" +
                                          "\r\n         B.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND B.注文区分 = @注文区分予測" +
                                          "\r\n         AND B.喫食日 = @喫食日" +
                                          "\r\n         AND B.得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n         AND B.部署ＣＤ = @部署ＣＤ" +
                                          "\r\n         AND B.商品ＣＤ = X.商品ＣＤ" +
                                          // -- ⑤ ①と商品マスタを外部結合
                                          "\r\n     LEFT JOIN 商品マスタ AS ST ON" +
                                          "\r\n         ST.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND ST.商品ＣＤ = X.商品ＣＤ" +
                                          // -- ⑥ 税率取得用に消費税率マスタを外部結合
                                          "\r\n     LEFT JOIN (" +
                                          "\r\n         SELECT" +
                                          "\r\n             消費税率 ," +
                                          "\r\n             軽減税率" +
                                          "\r\n         FROM" +
                                          "\r\n             消費税率マスタ" +
                                          "\r\n         WHERE" +
                                          "\r\n             施行日 = (SELECT max(施行日) FROM 消費税率マスタ WHERE 施行日 <= @喫食日)" +
                                          "\r\n     ) AS TAX ON 1 = 1" +
                                          // -- ⑦ ①の得意先単価マスタと得意先マスタを外部結合
                                          "\r\n     LEFT JOIN 得意先マスタ AS K ON" +
                                          "\r\n         K.企業ＣＤ = @企業ＣＤ" +
                                          "\r\n         AND K.得意先ＣＤ = @得意先ＣＤ" +
                                          // -- ⑧ ①の得意先単価マスタと主要商品マスタを外部結合
                                          "\r\n     LEFT JOIN 主要商品リストマスタ AS SS ON" +
                                          "\r\n         SS.企業ＣＤ = @企業ＣＤ AND" +
                                          "\r\n         SS.部署ＣＤ = @部署ＣＤ AND" +
                                          "\r\n         SS.商品ＣＤ = X.商品ＣＤ" +
                                          "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                                          "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                                          "\r\n ORDER BY" +
                                          "\r\n     CASE WHEN A.明細番号 IS NULL THEN 1 ELSE 0 END" +   // -- 注文→得意先単価のみの順
                                          "\r\n     ,A.明細番号" +
                                          "\r\n     ,ST.弁当区分" +
                                          "\r\n     ,X.商品ＣＤ";
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = (DateTime)_chumonCriteria.注文日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = (int)_chumonCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_chumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@税区分", SqlDbType.Int).Value = _chumonHeader.税区分 ?? (int)Syohizei.ZeiKubun.Uchi;   // 初期値は1:内税

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("注文明細");
                    adp.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        // 製造日が算出されていなければ算出
                        DateTime? seizobi = dr["製造日"] as DateTime?;
                        if (seizobi == null)
                        {
                            seizobi = CalcSeizobi((DateTime)_chumonCriteria.注文日, _busyo);

                            // 配送日＜製造日の場合は調整
                            DateTime? date = AdjustSeizobiHaisobi((DateTime)seizobi, (DateTime)_chumonHeader.配送日, _busyo, _chumonTokuisaki);
                            if (date != null)
                            {
                                seizobi = date;

                                // 新規作成状態であれば、配送日も更新（登録済みであれば更新しない）
                                if (_chumonHeader.作成日 == null)
                                {
                                    _chumonHeader.配送日 = date;
                                    dtpHaisobi.Text = string.Format("{0:yyyy/MM/dd（ddd）}", date);
                                }
                            }
                            dr["製造日"] = seizobi;
                            dr.AcceptChanges();
                        }
                        // 製造日のエラー（ワーニング）チェック
                        ChumonMeisai chumonMeisai = new ChumonMeisai(dr);
                        dr.SetColumnError("製造日", chumonMeisai["製造日"]);
                    }

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    dt.ColumnChanged += new DataColumnChangeEventHandler(dt_ColumnChanged);

                    chumonMeisaiBindingSource.DataSource = ds;
                    chumonMeisaiBindingSource.DataMember = "注文明細";

                    log.Info("注文明細を検索しました。");
                }

                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_SYOHIN_NAME_COLUMN, "商品名");
                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_SEIZOBI_COLUMN,     "製造日");
                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_TANKA_COLUMN,       "商品単価");
                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_YOTEISU_COLUMN,     "予定数");
                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_KOSU_COLUMN,        "商品数量");
                fpSpread_ChumonMeisai_Sheet1.BindDataColumn(CHUMON_KINGAKU_COLUMN,     "商品金額合計税込");

                fpSpread_ChumonMeisai.Focus();

                // スプレッドにレコードがある（つまり商品ＣＤが入っている）場合は、数量をアクティブセルにする
                if (fpSpread_ChumonMeisai_Sheet1.Rows.Count > 1)
                {
                    fpSpread_ChumonMeisai_Sheet1.SetActiveCell(0, CHUMON_KOSU_COLUMN);
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
                fpSpread_ChumonMeisai.SuspendLayout();      // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する

                _isDtColumnChanging = true;

                ChumonMeisai chumonMeisai = new ChumonMeisai(e.Row);

                // 商品CDの場合は商品マスタなどから取得
                if (e.Column.ColumnName == "商品ＣＤ")
                {
                    e.Row.SetColumnError(e.Column, null);
                    e.Row["商品名"] = null;
                    e.Row["製造日"] = DBNull.Value;
                    e.Row["商品グループＣＤ"] = DBNull.Value;
                    e.Row["売掛現金区分"] = _chumonTokuisaki.売掛現金区分;
                    e.Row["商品単価"] = DBNull.Value;
                    e.Row["商品数量"] = DBNull.Value;
                    e.Row["商品金額合計"] = DBNull.Value;
                    e.Row["予定数"] = DBNull.Value;
                    e.Row["消費税額"] = DBNull.Value;
                    e.Row["商品金額合計税込"] = DBNull.Value;
                    e.Row["計算税率"] = _syohizei.消費税率;
                    if (chumonMeisai.商品ＣＤ != null)
                    {
                        Syohin syohin = Syohin.GetSyohin((int)chumonMeisai.商品ＣＤ);
                        if (syohin != null)
                        {
                            e.Row["商品名"] = syohin.商品名;
                            e.Row["商品単価"] = Syohizei.GetUnitPrice(_chumonHeader.税区分, syohin.商品単価税込, syohin.商品単価税抜);
                            e.Row["計算税率"] = _syohizei.GetTaxRate(syohin.軽減税率フラグ);

                            // 得意先単価マスタから
                            TokuisakiTanka tokuisakiTanka = TokuisakiTanka.GetTokuisakiTanka((int)_chumonHeader.得意先ＣＤ, (int)syohin.商品ＣＤ);
                            if (tokuisakiTanka != null)
                            {
                                e.Row["商品単価"] = Syohizei.GetUnitPrice(_chumonHeader.税区分, tokuisakiTanka.商品単価税込, tokuisakiTanka.商品単価税抜);
                            }

                            // 主要商品リストマスタから
                            SyuyoSyohin syuyoSyohin = SyuyoSyohin.GetSyuyoSyohin((int)_chumonHeader.部署ＣＤ, (int)syohin.商品ＣＤ);
                            if (syuyoSyohin != null)
                            {
                                e.Row["商品グループＣＤ"] = syuyoSyohin.商品グループＣＤ;
                            }

                            // 予定数（注文明細から）
                            ChumonMeisai chumonMeisaiYosoku = ChumonMeisai.GetChumonMeisai(Consts.ChumonKubun.Yosoku, (DateTime)_chumonHeader.喫食日,
                                                                                    (int)_chumonHeader.得意先ＣＤ, (int)syohin.商品ＣＤ, (int)_chumonHeader.部署ＣＤ);
                            if (chumonMeisaiYosoku != null)
                            {
                                e.Row["予定数"] = chumonMeisaiYosoku.商品数量;
                            }

                            // 製造日（と配送日）を算出
                            DateTime? seizobi = CalcSeizobi((DateTime)_chumonHeader.喫食日, _busyo);
                            DateTime? date = AdjustSeizobiHaisobi((DateTime)seizobi, (DateTime)_chumonHeader.配送日, _busyo, _chumonTokuisaki);
                            if (date != null)
                            {
                                seizobi = date;

                                // 新規作成状態であれば、配送日も更新（登録済みであれば更新しない）
                                if (_chumonHeader.作成日 == null)
                                {
                                    _chumonHeader.配送日 = date;
                                    dtpHaisobi.Text = string.Format("{0:yyyy/MM/dd（ddd）}", date);

                                    // 「注文日と異なります」ラベルのVisibleを設定
                                    ToggleHaisobiIsDifferentFromChumonbi();
                                }
                            }
                            e.Row["製造日"] = seizobi;

                            // 製造日のエラー（ワーニング）チェック
                            chumonMeisai = new ChumonMeisai(e.Row);
                            e.Row.SetColumnError("製造日", chumonMeisai["製造日"]);
                        }
                        else
                        {
                            e.Row.SetColumnError(e.Column, "商品が見つかりません。");
                        }
                    }
                }

                // 製造日
                if (e.Column.ColumnName == "製造日")
                {
                    e.Row.SetColumnError(e.Column, chumonMeisai["製造日"]);
                }

                // 単価・個数の場合は金額を算出
                if (e.Column.ColumnName == "商品単価" || e.Column.ColumnName == "商品数量")
                {
                    if (chumonMeisai.商品ＣＤ != null)
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
                fpSpread_ChumonMeisai.ResumeLayout(true);       // 商品ＣＤを入力しても単価が描画されないことがあるのでそれを回避する
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

                // 商品金額合計・消費税額・商品金額合計税込を算出
                dr["商品金額合計"] = Syohizei.GetAmount(suryo, tanka);
                dr["消費税額"] = Syohizei.GetTax(_chumonHeader.税区分, suryo, tanka, (decimal)dr["計算税率"], _chumonHeader.丸め処理区分);
                dr["商品金額合計税込"] = Syohizei.GetAmountIncludeTax(_chumonHeader.税区分, suryo, tanka, (decimal)dr["計算税率"], _chumonHeader.丸め処理区分);
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
        /// 明細表示を「注文のみ」か「得意先単価を含める」かに切り替える
        /// </summary>
        private void ToggleMeisai()
        {
            try
            {
                // 検索未実施？
                if (_chumonCriteriaSnapShot == null)
                {
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_chumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _chumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_chumonCriteria.Equals(_chumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // スプレッド内の個数を保存
                DataSet ds = (DataSet)chumonMeisaiBindingSource.DataSource;
                DataTable dtSave = null;
                if (ds != null)
                {
                    dtSave = ds.Tables["注文明細"].Copy();
                }

                // 注文明細を取得（この中で「注文のみ」か「得意先単価を含める」かで分岐している）
                GetChumonMeisai();

                // 「注文日と異なります」ラベルのVisibleを設定
                ToggleHaisobiIsDifferentFromChumonbi();

                // スプレッド内の個数を復元
                ds = (DataSet)chumonMeisaiBindingSource.DataSource;
                DataTable dt = ds.Tables["注文明細"];

                _isDtColumnChanging = true;         // データソース列値変更イベントの呼び出しを回避

                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] drSave = null;
                    if (dr.IsNull("明細番号"))
                    {
                        // スプレッドに追加した商品が得意先単価マスタから取得した商品の場合に該当する
                        drSave = dtSave.Select(string.Format("商品ＣＤ={0} AND 明細番号 IS NULL", dr["商品ＣＤ"]));
                    }
                    else
                    {
                        drSave = dtSave.Select(string.Format("商品ＣＤ={0} AND 明細番号={1}", dr["商品ＣＤ"], dr["明細番号"]));
                    }

                    if (drSave != null && drSave.Length != 0)
                    {
                        dr["商品数量"] = drSave[0]["商品数量"];
                        CalcRow(dr);            // 行を計算
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
            finally
            {
                _isDtColumnChanging = false;
            }
        }

        /// <summary>
        /// 得意先に電話を掛ける
        /// </summary>
        /// <returns>電話を掛けた場合は True</returns>
        private bool CallToTokuisaki()
        {
            try
            {
                if (_chumonTokuisaki == null)
                {
                    return false;
                }

                // 有効な電話番号のみ架電する
                if (string.IsNullOrEmpty(_chumonTokuisaki.電話番号))
                {
                    return false;
                }

                string telNo = Regex.Replace(_chumonTokuisaki.電話番号, "[^0-9]", "");
                if (string.IsNullOrEmpty(telNo))
                {
                    return false;
                }

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
        /// コース状況を取得
        /// </summary>
        private void GetCourseCondition()
        {
            try
            {
                // コース状況表示がOFF？
                if (chkCourseCondition.Checked == false)
                {
                    return;
                }

                // 数式を消すため、全行削除する
                fpSpread_CourseCondition_Sheet1.Rows.Count = 0;
                courseConditionBindingSource.DataSource = null;

                if (_chumonHeader == null || _chumonHeader.配送日 == null || _chumonHeader.喫食日 == null)
                {
                    return;
                }

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     C.得意先ＣＤ" +
                                      "\r\n     ,T.得意先名略称" +
                                      "\r\n     ,CASE WHEN UH.得意先ＣＤ IS NULL THEN '' ELSE '○' END AS 売上" +
                                      "\r\n     ,CASE WHEN CH.得意先ＣＤ IS NULL THEN '' ELSE '○' END AS 配送" +
                                      "\r\n     ,VJ.受注方法区分名" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT" +
                                      "\r\n         CT.企業ＣＤ" +
                                      "\r\n         ,CT.得意先ＣＤ" +
                                      "\r\n         ,CT.コース順" +
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
                                      "\r\n                 AND 実施日 = @配送日" +
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
                                      "\r\n         ,RT.コース順" +
                                      "\r\n     FROM" +
                                      "\r\n         臨時コーステーブル AS RT" +
                                      "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n     WHERE" +
                                      "\r\n         RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND RT.コースＣＤ = @コースＣＤ" +
                                      "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n         AND RT.実施日 = @配送日" +
                                      "\r\n     ) AS C" +
                                      "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ" +
                                      "\r\n     LEFT JOIN V_受注方法区分 AS VJ ON VJ.受注方法区分CD = T.受注方法" +
                                      "\r\n     LEFT JOIN 売上ヘッダ AS UH ON" +
                                      "\r\n         UH.企業ＣＤ = C.企業ＣＤ " +
                                      "\r\n         AND UH.売上日 = @注文日" +
                                      "\r\n         AND UH.得意先ＣＤ = C.得意先ＣＤ" +
                                      "\r\n         AND UH.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     LEFT JOIN コース配送履歴 AS CH ON" +
                                      "\r\n         CH.企業ＣＤ = C.企業ＣＤ" +
                                      "\r\n         AND CH.配送日 = @配送日" +
                                      "\r\n         AND CH.得意先ＣＤ = C.得意先ＣＤ" +
                                      "\r\n         AND CH.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     C.コース順";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@配送日", SqlDbType.Date).Value = (DateTime)_chumonHeader.配送日;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = (DateTime)_chumonHeader.喫食日;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = ((cboCourse.SelectedValue as int?) ?? -1);  // cboCourseのValidated前呼ばれることがある（_chumonHeader.コースＣＤには格納されていない）
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_chumonHeader.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コース売上状況");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    courseConditionBindingSource.DataSource = ds;
                    courseConditionBindingSource.DataMember = "コース売上状況";

                    log.Info("コース売上状況を検索しました。");
                }

                fpSpread_CourseCondition_Sheet1.BindDataColumn(COURSE_TOKUISAKI_CD_COLUMN,   "得意先ＣＤ");
                fpSpread_CourseCondition_Sheet1.BindDataColumn(COURSE_TOKUISAKI_NAME_COLUMN, "得意先名略称");
                fpSpread_CourseCondition_Sheet1.BindDataColumn(COURSE_URIAGE_COLUMN,         "売上");
                fpSpread_CourseCondition_Sheet1.BindDataColumn(COURSE_HAISO_COLUMN,          "配送");
                fpSpread_CourseCondition_Sheet1.BindDataColumn(COURSE_JUCYUHOHO_COLUMN,      "受注方法区分名");

                // 行の背景色を変えつつ、表示している得意先の行をカレントにする
                for (int rowIndex = 0 ; rowIndex < fpSpread_CourseCondition_Sheet1.Rows.Count ; rowIndex++)
                {
                    // 売上OR配送がある？
                    if (string.IsNullOrEmpty(fpSpread_CourseCondition_Sheet1.Cells[rowIndex, COURSE_URIAGE_COLUMN].Text) == false ||
                        string.IsNullOrEmpty(fpSpread_CourseCondition_Sheet1.Cells[rowIndex, COURSE_HAISO_COLUMN].Text) == false)
                    {
                        fpSpread_CourseCondition_Sheet1.Rows[rowIndex].BackColor = BACK_COLOR_URIAGE;
                    }

                    // 表示中の得意先？
                    if (_chumonHeader.得意先ＣＤ == fpSpread_CourseCondition_Sheet1.Cells[rowIndex, COURSE_TOKUISAKI_CD_COLUMN].Value as int?)
                    {
                        fpSpread_CourseCondition_Sheet1.Rows[rowIndex].BackColor = Color.PaleGreen;
                        fpSpread_CourseCondition_Sheet1.SetActiveCell(rowIndex, COURSE_TOKUISAKI_CD_COLUMN);
                        fpSpread_CourseCondition_Sheet1.AddSelection(rowIndex, 0, 1, 1);  // その行を選択
                        fpSpread_CourseCondition.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Bottom, FarPoint.Win.Spread.HorizontalPosition.Nearest);
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
        /// 注文ヘッダ・注文明細が変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeHeader()
        {
            // 注文ヘッダを比較
            if (_chumonHeader == null)
            {
                return false;
            }

            if (_chumonHeader.Equals(_origin))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注文明細が変更されているかを返します。
        /// </summary>
        /// <returns>変更されている場合はtrue</returns>
        private bool HasChangeDetail()
        {
            // 注文明細を比較
            DataSet ds = (DataSet)chumonMeisaiBindingSource.DataSource;
            if (ds == null)
            {
                return false;
            }

            DataTable dt = ds.Tables["注文明細"];

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
        /// 注文ヘッダが登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveChumonHeader()
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索条件の入力にエラーがある？
                if (_chumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _chumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索未実施？
                if (_chumonCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // 検索実行時の条件が変わっている？
                if (_chumonCriteria.Equals(_chumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                if (_chumonHeader.HasErrors)
                {
                    MessageBox.Show(this, _chumonHeader.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                // コースが未選択？
                if (_chumonHeader.コースＣＤ == null || _chumonHeader.コースＣＤ < 0)
                {
                    // キャンセルボタンがデフォルト
                    if (MessageBox.Show(this, "コースが選択されていません。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Cancel)
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
        /// 注文明細が登録可能か？
        /// </summary>
        /// <returns>登録可能な場合はtrue</returns>
        private bool CanSaveChumonMeisai()
        {
            bool hasError = false;

            try
            {
                DataSet ds = (DataSet)chumonMeisaiBindingSource.DataSource;
                if (ds == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                DataTable dt = ds.Tables["注文明細"];

                _chumonHeader.配送日エラー = null;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    dr.ClearErrors();

                    // 削除された行についてはエラーチェックは不要
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    ChumonMeisai chumonMeisai = new ChumonMeisai(dr);

                    // 注文区分がない場合は新規扱い
                    if (chumonMeisai.注文区分 == null)
                    {
                        chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                        chumonMeisai.喫食日 = _chumonCriteria.注文日;
                        chumonMeisai.得意先ＣＤ = _chumonCriteria.得意先ＣＤ;
                        chumonMeisai.部署ＣＤ = _chumonCriteria.部署ＣＤ;
                        if (dr.RowState == DataRowState.Unchanged)
                        {
                            dr.SetAdded();      // 得意先マスタから取得したレコードの場合があるので新規扱いとする
                        }
                    }

                    // エラーがある？
                    if (chumonMeisai.HasErrors)
                    {
                        dr.SetColumnError("商品ＣＤ", chumonMeisai["商品ＣＤ"]);
                        dr.SetColumnError("製造日", chumonMeisai["製造日"]);
                        hasError = true;
                    }
                    else
                    {
                        // 商品ＣＤに該当するマスタが存在するか？
                        if (chumonMeisai.商品ＣＤ != null)
                        {
                            Syohin syohin = Syohin.GetSyohin((int)chumonMeisai.商品ＣＤ);
                            if (syohin == null)
                            {
                                dr.SetColumnError("商品ＣＤ", "商品が見つかりません。");
                                hasError = true;
                            }

                            // 注文ヘッダー.配送日＜製造日の場合はエラー（配送日をエラーとする）
                            if ((DateTime)_chumonHeader.配送日 < (DateTime)chumonMeisai.製造日)
                            {
                                _chumonHeader.配送日エラー = "配送日は製造日より過去にはできません。";
                                dtpHaisobi.Focus();
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
                    // 注文ヘッダ保存
                    if (SaveChumonHeader(con, tran) == false)
                    {
                        return -1;
                    }

                    // 注文明細マージ
                    MergeChumonMeisai();

                    // 注文明細保存
                    if (SaveChumonMeisai(con, tran) == false)
                    {
                        return -1;
                    }

                    // 予測があって注文が入力されなかった明細について、数量=0の注文の明細レコードを作る
                    if (AddChumonMeisai(con, tran) == false)
                    {
                        return -1;
                    }

                    // 注文明細が存在しなければ注文ヘッダを削除する
                    int ret = DeleteChumonHeaderWhenNoMeisai(con, tran);
                    if (ret < 0)
                    {
                        return -1;
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("注文ヘッダ・注文明細を{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 初期状態として保持
                    _origin = new ChumonHeader(_chumonHeader);

                    if (ret != 0)
                    {
                        // 登録した結果、注文明細が0件なので注文ヘッダも削除した場合
                        log.Info("注文明細の登録件数が0件なので、注文ヘッダを削除しました。");
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
        /// 注文ヘッダ保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveChumonHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                bool ret = false;
                History.ProcessType historyProcess = History.ProcessType.Create;

                if (_IsNewRecord)
                {
                    ret = _chumonHeader.Insert(con, tran);
                }
                else
                {
                    ret = _chumonHeader.Update(con, tran);
                    historyProcess = History.ProcessType.Update;
                }

                if (ret == false)
                {
                    MessageBox.Show(this, _chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "注文ヘッダ", _chumonHeader.Code, historyProcess);
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
        /// 注文明細マージ
        /// </summary>
        private void MergeChumonMeisai()
        {
            try
            {
                DataSet ds = (DataSet)chumonMeisaiBindingSource.DataSource;
                DataTable dt = ds.Tables["注文明細"];

                // レコードが1件のみの場合はマージ不要
                if (dt.Rows.Count <= 1)
                {
                    return;
                }

                //***************************************************************************************
                // 同じ商品CDのレコードを１つにまとめる（つまり同じ商品を別の行に入力することを許可する）
                // 先に入力された方に個数を加算する
                // 入力された単価は無視する
                //***************************************************************************************

                // マージ先（先頭～末尾の１つ前）のループ
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    DataRow drTo = dt.Rows[i];
                    if (drTo.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    ChumonMeisai chumonMeisaiTo = new ChumonMeisai(drTo);

                    // マージ元（末尾～マージ先の次まで）のループ
                    // マージ先の次～末尾までだと、マージ元を削除したときに「末尾」の値（dt.Rows.Count）が変わってしまうので
                    for (int j = (dt.Rows.Count - 1); j > i; j--)
                    {
                        DataRow drFrom = dt.Rows[j];
                        if (drFrom.RowState == DataRowState.Deleted)
                        {
                            continue;
                        }
                        ChumonMeisai chumonMeisaiFrom = new ChumonMeisai(drFrom);

                        // 同じ商品？
                        if (chumonMeisaiFrom.商品ＣＤ != null && chumonMeisaiFrom.商品ＣＤ == chumonMeisaiTo.商品ＣＤ)
                        {
                            // マージ元の数量をマージ先に加算
                            chumonMeisaiTo.商品数量 = (chumonMeisaiTo.商品数量 ?? 0m) + (chumonMeisaiFrom.商品数量 ?? 0m);
                            drTo.SetField<decimal?>("商品数量", chumonMeisaiTo.商品数量);

                            // マージ元を削除
                            drFrom.Delete();
                        }
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
        /// 注文明細保存
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool SaveChumonMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                DataSet ds = (DataSet)chumonMeisaiBindingSource.DataSource;
                DataTable dt = ds.Tables["注文明細"];

                History.ProcessType historyProcess = History.ProcessType.Create;
                int seq = 1;
                bool ret = true;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    ChumonMeisai chumonMeisai = new ChumonMeisai(dr);

                    ret = true;

                    switch (dr.RowState)
                    {
                        case DataRowState.Added:        // 追加
                            // 商品ＣＤまたは個数が未入力の場合は追加しない
                            if (chumonMeisai.商品ＣＤ == null ||
                                chumonMeisai.商品数量 == null)
                            {
                                continue;
                            }
                            chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                            chumonMeisai.喫食日 = _chumonCriteria.注文日;
                            chumonMeisai.得意先ＣＤ = _chumonCriteria.得意先ＣＤ;
                            chumonMeisai.部署ＣＤ = _chumonCriteria.部署ＣＤ;
                            chumonMeisai.明細番号 = seq;
                            ret = chumonMeisai.Insert(con, tran);
                            historyProcess = History.ProcessType.Create;
                            seq++;
                            break;

                        case DataRowState.Modified:     // 変更
                        case DataRowState.Unchanged:    // 変更なし→明細番号のみ更新
                            if (chumonMeisai.修正日 == null)    // 得意先単価マスタから取得したレコードの場合は修正日がnull
                            {
                                // 商品ＣＤまたは個数が未入力の場合は登録しない
                                if (chumonMeisai.商品ＣＤ != null &&
                                    chumonMeisai.商品数量 != null)
                                {
                                    chumonMeisai.明細番号 = seq;
                                    seq++;
                                    chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;
                                    chumonMeisai.喫食日 = _chumonCriteria.注文日;
                                    chumonMeisai.得意先ＣＤ = _chumonCriteria.得意先ＣＤ;
                                    chumonMeisai.部署ＣＤ = _chumonCriteria.部署ＣＤ;
                                    ret = chumonMeisai.Insert(con, tran);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                // 商品ＣＤまたは個数が未入力の場合は削除する
                                if (chumonMeisai.商品ＣＤ != null &&
                                    chumonMeisai.商品数量 != null)
                                {
                                    chumonMeisai.明細番号 = seq;
                                    seq++;
                                    ret = chumonMeisai.Update(con, tran);
                                    historyProcess = History.ProcessType.Update;
                                }
                                else
                                {
                                    ret = chumonMeisai.Delete(con, tran);
                                    historyProcess = History.ProcessType.Delete;
                                }
                            }
                            break;

                        case DataRowState.Deleted:      // 削除
                            if (chumonMeisai.修正日 == null)    // 得意先単価マスタから取得したレコードの場合は修正日がnull
                            {
                                continue;
                            }
                            ret = chumonMeisai.Delete(con, tran);
                            historyProcess = History.ProcessType.Delete;
                            break;

                        default:
                            continue;
                    }

                    if (ret == false)
                    {
                        MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);
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
        /// 予測があって注文が入力されなかった明細について、数量=0の注文の明細レコードを作る
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool AddChumonMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                DataTable dt = null;
                int seq = 1;
                bool ret = true;

                try
                {
                    // 次の明細番号を取得
                    seq = ChumonMeisai.GetNextMeisaiNo(con, tran, Consts.ChumonKubun.Chuumon, (DateTime)_chumonCriteria.注文日, (int)_chumonCriteria.得意先ＣＤ, (int)_chumonCriteria.部署ＣＤ);

                    //*********************
                    // 注文が無い予測を取得
                    //*********************
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     M1.企業ＣＤ" +
                                      "\r\n     ,M1.注文区分" +
                                      "\r\n     ,M1.喫食日" +
                                      "\r\n     ,M1.得意先ＣＤ" +
                                      "\r\n     ,M1.商品ＣＤ" +
                                      "\r\n     ,M1.部署ＣＤ" +
                                      "\r\n     ,M1.明細番号" +
                                      "\r\n     ,M1.製造日" +
                                      "\r\n     ,M1.商品グループＣＤ" +
                                      "\r\n     ,M1.売掛現金区分" +
                                      "\r\n     ,M1.商品単価" +
                                      "\r\n     ,M1.商品数量" +
                                      "\r\n     ,M1.商品金額合計" +
                                      "\r\n     ,M1.消費税額" +
                                      "\r\n     ,M1.商品金額合計税込" +
                                      "\r\n     ,M1.計算税率" +
                                      "\r\n     ,M1.作成担当者ＣＤ" +
                                      "\r\n     ,M1.作成日" +
                                      "\r\n     ,M1.修正担当者ＣＤ" +
                                      "\r\n     ,M1.修正日" +
                                      "\r\n FROM" +
                                      "\r\n     注文明細 AS M1" +
                                      "\r\n     LEFT JOIN 注文明細 AS M0" +
                                      "\r\n         ON M0.企業ＣＤ    = M1.企業ＣＤ" +
                                      "\r\n         AND M0.注文区分   = @注文区分注文" +
                                      "\r\n         AND M0.喫食日     = M1.喫食日" +
                                      "\r\n         AND M0.得意先ＣＤ = M1.得意先ＣＤ" +
                                      "\r\n         AND M0.商品ＣＤ   = M1.商品ＣＤ" +
                                      "\r\n         AND M0.部署ＣＤ   = M1.部署ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     M1.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND M1.注文区分 = @注文区分予測" +
                                      "\r\n     AND M1.喫食日 = @喫食日" +
                                      "\r\n     AND M1.得意先ＣＤ = @得意先ＣＤ" +
                                      "\r\n     AND M1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND M0.明細番号 IS NULL";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _chumonCriteria.注文日;
                    cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _chumonCriteria.得意先ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonCriteria.部署ＣＤ;
                    cmd.Transaction = tran;
                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("注文明細");
                    adp.Fill(dt);
                }
                catch (SqlException ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("予測のみの注文明細の取得に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("予測のみの注文明細の取得に失敗しました。\n{0}", ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //*****************************
                // 取得した予測を注文にして追加
                //*****************************
                foreach (DataRow dr in dt.Rows)
                {
                    ChumonMeisai chumonMeisai = new ChumonMeisai(dr);
                    chumonMeisai.注文区分 = (int)Consts.ChumonKubun.Chuumon;    // 注文として作成する
                    chumonMeisai.明細番号 = seq;
                    // 数量・金額は0
                    chumonMeisai.商品数量 = 0m;
                    chumonMeisai.商品金額合計 = 0m;
                    chumonMeisai.消費税額 = 0m;
                    chumonMeisai.商品金額合計税込 = 0m;
                    ret = chumonMeisai.Insert(con, tran);
                    historyProcess = History.ProcessType.Create;
                    seq++;

                    if (ret == false)
                    {
                        MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "注文明細(予測に対する注文)", chumonMeisai.Code, historyProcess);
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
        /// 注文明細が存在しなければ注文ヘッダを削除する
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>削除した件数(0か1)。削除に失敗した場合はマイナス値</returns>
        private int DeleteChumonHeaderWhenNoMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                try
                {
                    //*************************************
                    // 注文明細が存在しない注文ヘッダを削除
                    //*************************************
                    SqlCommand cmdDelH = con.CreateCommand();
                    cmdDelH.CommandText = "\r\n DELETE 注文ヘッダ" +
                                          "\r\n FROM" +
                                          "\r\n     注文ヘッダ AS H" +
                                          "\r\n     LEFT JOIN 注文明細 AS M" +
                                          "\r\n     ON H.企業ＣＤ    = M.企業ＣＤ" +
                                          "\r\n     AND H.注文区分   = M.注文区分" +
                                          "\r\n     AND H.喫食日     = M.喫食日" +
                                          "\r\n     AND H.得意先ＣＤ = M.得意先ＣＤ" +
                                          "\r\n     AND H.部署ＣＤ   = M.部署ＣＤ" +
                                          "\r\n WHERE" +
                                          "\r\n     H.企業ＣＤ       = @企業ＣＤ" +
                                          "\r\n     AND H.注文区分   = @注文区分" +
                                          "\r\n     AND H.喫食日     = @喫食日 "+
                                          "\r\n     AND H.得意先ＣＤ = @得意先ＣＤ" +
                                          "\r\n     AND H.部署ＣＤ   = @部署ＣＤ" +
                                          "\r\n     AND M.企業ＣＤ IS NULL";
                    cmdDelH.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmdDelH.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmdDelH.Parameters.Add("@喫食日", SqlDbType.Date).Value = _chumonCriteria.注文日;
                    cmdDelH.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _chumonCriteria.得意先ＣＤ;
                    cmdDelH.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _chumonCriteria.部署ＣＤ;
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
                    MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    tran.Rollback();
                    History.Error(ex);
                    MessageBox.Show(string.Format("注文ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    // 注文ヘッダ削除
                    if (DeleteChumonHeader(con, tran) == false)
                    {
                        return false;
                    }

                    // 注文明細削除
                    if (DeleteChumonMeisai(con, tran) == false)
                    {
                        return false;
                    }

                    tran.Commit();
                    log.InfoFormat("注文ヘッダ・注文明細を{0}しました。", historyProcess.DisplayName());
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
        /// 注文ヘッダ削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteChumonHeader(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                History.ProcessType historyProcess = History.ProcessType.Delete;

                bool ret = _chumonHeader.Delete(con, tran);

                if (ret == false)
                {
                    MessageBox.Show(this, _chumonHeader.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                History.Save(con, tran, this.Text, "注文ヘッダ", _chumonHeader.Code, historyProcess);
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
        /// 注文明細削除
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <returns>成功した場合は True</returns>
        private bool DeleteChumonMeisai(SqlConnection con, SqlTransaction tran)
        {
            try
            {
                // 全削除
                DataSet ds = (DataSet)chumonMeisaiBindingSource.DataSource;
                DataTable dt = ds.Tables["注文明細"];

                History.ProcessType historyProcess = History.ProcessType.Delete;

                // 行数分ループ
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)  // 画面上にて追加されたばかりのレコードは削除しない
                    {
                        continue;
                    }

                    ChumonMeisai chumonMeisai = new ChumonMeisai(dr);
                    if (chumonMeisai.作成日 == null)
                    {
                        continue;
                    }
                    bool ret = chumonMeisai.Delete(con, tran);
                    if (ret == false)
                    {
                        MessageBox.Show(this, chumonMeisai.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    History.Save(con, tran, this.Text, "注文明細", chumonMeisai.Code, historyProcess);
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
        /// 備考コンボの選択肢をリセット
        /// </summary>
        /// <param name="cbo">リセット対象の備考コンボボックス</param>
        private void ResetBiko(ComboBox cbo)
        {
            try
            {
                cbo.DataSource = _dtBiko.Copy();
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
        /// 備考コンボの選択肢を追加する
        /// </summary>
        /// <param name="cbo">追加対象の備考コンボボックス</param>
        /// <param name="biko">追加する備考</param>
        private void AddBiko(ComboBox cbo, string biko)
        {
            try
            {
                if (string.IsNullOrEmpty(biko))
                {
                    return;
                }

                // 選択肢が存在していれば何もしない
                DataTable dt = (DataTable)cbo.DataSource;
                DataRow[] drs = dt.Select(string.Format("備考='{0}'", dt.Escape(biko)));
                if (drs == null || drs.Length == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["備考CD"] = 0;
                    dr["備考"] = biko;
                    dt.Rows.InsertAt(dr, 1);        // 空白の選択肢の次
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
                this.Text = "注文入力";

                lblMode.Text = MODE_CAPTION_NEW;
                lblMode.BackColor = BACK_COLOR_NEW;

                // スプレッドは検索が実施されるまで非活性
                fpSpread_ChumonMeisai.Enabled = false;
                chumonMeisaiBindingSource.DataSource = null;

                // 配送日も検索が実施されるまで非活性
                dtpHaisobi.Enabled = false;

                // 「注文日と異なります」ラベルのVisibleを設定
                ToggleHaisobiIsDifferentFromChumonbi();

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

                // コース注文入力／コース注文一覧／受注発信一覧から引き継いだコースもクリアする
                _chumonCriteria.コースＣＤ = null;

                // コース状況を取得
                GetCourseCondition();

                // 注文ヘッダーを初期化
                _chumonHeader = new ChumonHeader();
                chumonHeaderBindingSource.DataSource = _chumonHeader;

                // 初期状態として保持
                _origin = new ChumonHeader(_chumonHeader);

                // 注文得意先を初期化
                _chumonTokuisaki = new ChumonTokuisaki();
                chumonTokuisakiBindingSource.DataSource = _chumonTokuisaki;

                // 配送状態を着色
                SetHaisoJotaiColor(_chumonTokuisaki.配送状態);

                // 備考コンボをリセット
                ResetBiko(cboBiko1);
                ResetBiko(cboBiko2);
                ResetBiko(cboBiko3);
                ResetBiko(cboBiko4);
                ResetBiko(cboBiko5);

                // 注文入力時残高表示を初期表示（クリア）
                btnZandaka.Enabled = false;
                _chumonZandaka.Clear();
                chumonZandakaBindingSource.ResetBindings(false);
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
                // コースコンボ変更イベントを処理を再開
                _stopCboCourseSelectedIndexChanged = false;
                _CboCourseSelectedIndex = cboCourse.SelectedIndex;
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

        /// <summary>
        /// 「本日ではありません」ラベルのVisibleを設定
        /// </summary>
        private void ToggleChumonbiIsNotToday()
        {
            lblChumonbiIsNotToday.Visible = (DateTime.Today != _chumonCriteria.注文日);
        }

        /// <summary>
        /// 「注文日と異なります」ラベルのVisibleを設定
        /// </summary>
        private void ToggleHaisobiIsDifferentFromChumonbi()
        {
            if (dtpHaisobi.Enabled)
            {
                lblHaisobiIsDifferentFromChumonbi.Visible = (_chumonCriteria.注文日 != _chumonHeader.配送日);
            }
            else
            {
                lblHaisobiIsDifferentFromChumonbi.Visible = false;
            }
        }

        /// <summary>
        /// 配送状態を着色
        /// </summary>
        /// <param name="haisoJotai">配送状態の文字列</param>
        private void SetHaisoJotaiColor(string haisoJotai)
        {
            try
            {
                if (haisoJotai == "配達済")
                {
                    lblHaisoJotai.ForeColor = Color.White;
                    lblHaisoJotai.BackColor = Color.Red;
                }
                else
                {
                    lblHaisoJotai.ForeColor = SystemColors.ControlText;
                    lblHaisoJotai.BackColor = SystemColors.Control;
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

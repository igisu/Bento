using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;

namespace Emu.Bento
{
    /// <summary>
    /// コース注文入力
    /// </summary>
    public partial class Form201 : BaseSpreadForm
    {
        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// 選択された子ビューの位置情報
        /// </summary>
        private class SelectedChild
        {
            /// <summary>
            /// 子ビューのインデックス
            /// </summary>
            public int ViewIndex { get; set; }

            /// <summary>
            /// 行インデックス
            /// </summary>
            public int RowIndex { get; set; }

            /// <summary>
            /// 列インデックス
            /// </summary>
            public int ColumnIndex { get; set; }

            /// <summary>
            /// 親の行インデックス
            /// </summary>
            public int ParentRowIndex { get; set; }
        }
        #endregion

        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMON_CALL_TIME_COLUMN = 0;     // 発信時刻列インデックス
        private const int CHUMON_TOKUISAKI_CD_COLUMN = 1;     // 得意先ＣＤ列インデックス
        private const int CHUMON_TOKUISAKI_NAME_COLUMN = 2;     // 得意先名列インデックス
        private const int CHUMON_RENRAKU_COLUMN = 3;     // 連絡列インデックス
        private const int CHUMON_URIGEN_KUBUN_COLUMN = 4;     // 売上現金区分列インデックス
        private const int CHUMON_CHUMON_TOTAL_COLUMN = 5;     // 注文計列インデックス
        private const int CHUMON_GENKIN_TOTAL_COLUMN = 6;     // 現金計列インデックス
        private const int CHUMON_BIKO_COLUMN = 7;     // 備考列インデックス
        private const int CHUMON_CHUMON_UMU_COLUMN = 8;     // 注文有無フラグ列インデックス
        // これ以降の列は動的に追加される
        private const int CHUMON_SYOHIN_START_COLUMN = 9;     // 商品名列開始列インデックス

        private const int BUHIN_TITLE_COLUMN = 0;     // 見出し列インデックス
        // これ以降の列は動的に追加される
        private const int BUHIN_SYOHIN_START_COLUMN = 1;     // 商品名列開始列インデックス

        private const int BUHIN_MOCHIDASHI_ROW = 0;     // 持出行
        private const int BUHIN_IDO_ROW = 1;     // 移動行
        private const int BUHIN_JUCHU_ROW = 2;     // 受注行
        private const int BUHIN_SA_ROW = 3;     // 差行

        private const int PRINT_GET_SYOHINS = 16;       // 配送集計表に出力するために取得する商品の個数（その他を含めない）
        private const int PRINT_SYOHIN_COLUMNS = PRINT_GET_SYOHINS + 1; // 配送集計表に出力する商品の最大列数（その他を含める）

        /// <summary>
        /// 持出＜受注がマイナスの色
        /// </summary>
        private readonly Color MINUS_BACK_COLOR = Color.LightPink;

        /// <summary>
        /// 注文有りの背景色
        /// </summary>
        private readonly Color CHUMON_BACK_COLOR = Color.PapayaWhip;

        /// <summary>
        /// 売上文字色
        /// </summary>
        private readonly Color URIAGE_FORE_COLOR = Color.FromArgb(0, 100, 255);     // Blueだと濃すぎて黒と見分けが付きづらい

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// コース注文入力検索条件
        /// </summary>
        private CourseChumonCriteria _courseChumonCriteria;

        /// <summary>
        /// コース注文入力検索条件検索実行時スナップショット
        /// </summary>
        private CourseChumonCriteria _courseChumonCriteriaSnapShot;

        /// <summary>
        /// 階層表示をするかしないか
        /// </summary>
        private bool _isHierarchicalView;

        /// <summary>
        /// 列として採用する商品名を格納したデータテーブル
        /// </summary>
        private DataTable _syohinDt;

        /// <summary>
        /// コース注文入力のデータ本体（GetChumonData()で取得するデータテーブル）
        /// </summary>
        private DataTable _dtChumon;

        /// <summary>
        /// 商品名を列に持つデータテーブル
        /// </summary>
        private ColumnListDataTable _cldt;

        /// <summary>
        /// 選択された子ビューの位置情報
        /// </summary>
        private SelectedChild _selectedChild;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = CHUMON_SYOHIN_START_COLUMN;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="courseChumonCriteria"></param>
        public Form201(CourseChumonCriteria courseChumonCriteria)
        {
            try
            {
                InitializeComponent();
                _courseChumonCriteria = new CourseChumonCriteria(courseChumonCriteria);
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
        private void Form201_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.page_ico;

                lblChumon.ForeColor = Color.Red;
                lblUriage.ForeColor = URIAGE_FORE_COLOR;

                // 階層表示をするかしないか
                _isHierarchicalView = SystemMaster.GetHierarchicalViewOn201();

                // コンボボックスの選択肢生成
                ComboUtil.Set(cboChumon,
                    new string[] { "全て", "有", "無" },
                    new string[] { "", "AND CK.注文計 IS NOT NULL", "AND CK.注文計 IS NULL" });
                ComboUtil.Set(cboRenraku,
                    new string[] { "全て", "未", "済", "保留" },
                    new string[] { "", "AND THR.連絡 = '未'", "AND THR.連絡 = '済'", "AND THR.連絡 = '保留'" });

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                courseChumonBindingSource.DataSource = _courseChumonCriteria;

                // スプレッドの得意先ＣＤ列はフラグに応じて表示
                if (SystemMaster.GetDispTokuisakiCD() == false)
                {
                    fpSpread_Chumon_Sheet1.Columns[CHUMON_TOKUISAKI_CD_COLUMN].Visible = false;
                }

                // スプレッドの注文有無フラグを非表示にする
                fpSpread_Chumon_Sheet1.Columns[CHUMON_CHUMON_UMU_COLUMN].Visible = false;

                // 動的に追加される列以外を固定列にする
                fpSpread_Chumon_Sheet1.FrozenColumnCount = CHUMON_SYOHIN_START_COLUMN;

                // 階層表示をする場合は行ヘッダの幅を広げて左寄せにする
                if (_isHierarchicalView)
                {
                    fpSpread_Chumon_Sheet1.RowHeader.Columns[0].Width = 50;
                    fpSpread_Chumon_Sheet1.RowHeader.Columns[0].HorizontalAlignment = FarPoint.Win.Spread.CellHorizontalAlignment.Left;
                }

                // 検索条件が設定されていれば検索実行
                if (_courseChumonCriteria.HasErrors == false)
                {
                    // 既存のデータを取得
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form201_Shown(object sender, EventArgs e)
        {
            // 検索実行していればスプレッドにフォーカスを当てる
            if (_courseChumonCriteriaSnapShot != null)
            {
                fpSpread_Chumon.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form201_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F8:                       // 注文入力ボタン押下
                    btnChumon.PerformClick();
                    break;

                case Keys.F10:
                    btnPrint.PerformClick();        // 配送集計表印刷ボタン押下
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
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
                // 検索条件が変わったら画面をクリア
                if (_courseChumonCriteriaSnapShot != null && _courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
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
                if (_courseChumonCriteriaSnapShot != null && _courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
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
                    _courseChumonCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;

                    // 以下、CourseChumonCriteriaによって設定された内容を画面に反映する
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
                if (_courseChumonCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_courseChumonCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _courseChumonCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_courseChumonCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // タイトルバーにコース名を表示
                this.Text = string.Format("{0} - コース注文入力", lblCourseName.Text);

                this.MdiParent.Cursor = Cursors.WaitCursor;

                log.Debug("取得開始");

                // 注文一覧を表示
                if (_isHierarchicalView)
                {
                    GetChumonHierarchical();
                }
                else
                {
                    GetChumon();
                }

                // 部品状況を表示
                GetBuhin();

                GetTicket();       // 食券枚数計を表示
                GetMisyuTicket();  // 未収金券計を表示
                GetGenkin();       // 現金合計を表示
                GetMisyukin();     // 未収金計を表示

                log.Debug("表示完了");
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
        /// 子ワークブック作成時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Chumon_ChildWorkbookCreated(object sender, FarPoint.Win.Spread.ChildWorkbookCreatedEventArgs e)
        {
            e.Workbook.HorizontalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.Never;
            e.Workbook.VerticalScrollBarPolicy = FarPoint.Win.Spread.ScrollBarPolicy.AsNeeded;
        }

        /// <summary>
        /// 子ビュー作成時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Chumon_ChildViewCreated(object sender, FarPoint.Win.Spread.ChildViewCreatedEventArgs e)
        {
            try
            {
                e.SheetView.Columns.Count = fpSpread_Chumon_Sheet1.Columns.Count;

                e.SheetView.AutoGenerateColumns = false;    // データバインドを明示的に行う

                // 子ビューも親と同じ固定列にする
                e.SheetView.FrozenColumnCount = CHUMON_SYOHIN_START_COLUMN;

                e.SheetView.BindDataColumn(CHUMON_CALL_TIME_COLUMN, "発信");
                e.SheetView.BindDataColumn(CHUMON_TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                e.SheetView.BindDataColumn(CHUMON_TOKUISAKI_NAME_COLUMN, "得意先名");
                e.SheetView.BindDataColumn(CHUMON_RENRAKU_COLUMN, "連絡");
                e.SheetView.BindDataColumn(CHUMON_URIGEN_KUBUN_COLUMN, "種類");
                e.SheetView.BindDataColumn(CHUMON_CHUMON_TOTAL_COLUMN, "注文計");
                e.SheetView.BindDataColumn(CHUMON_GENKIN_TOTAL_COLUMN, "現金計");
                e.SheetView.BindDataColumn(CHUMON_BIKO_COLUMN, "備考");
                e.SheetView.BindDataColumn(CHUMON_CHUMON_UMU_COLUMN, "注文有無フラグ");

                // 商品名を列名としてバインド
                int columnIndex = CHUMON_SYOHIN_START_COLUMN;
                foreach (string columnName in _cldt.ColumnNames)
                {
                    e.SheetView.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }

                // スタイル情報設定
                e.SheetView.DataAutoSizeColumns = false;
                e.SheetView.DataAutoCellTypes = false;
                e.SheetView.DataAutoHeadings = false;

                e.SheetView.ColumnHeader.Visible = false;
                e.SheetView.RowHeader.Visible = false;

                // スプレッドの文字色を以下のように変更
                // 予測：黒
                // 注文：赤
                // 実績：青
                for (int rowIndex = 0; rowIndex < e.SheetView.Rows.Count; rowIndex++)
                {
                    e.SheetView.Rows[rowIndex].ResetForeColor();     // 黒に戻す

                    // ついでに注文がある場合の背景色をセット
                    e.SheetView.Rows[rowIndex].ResetBackColor();
                    e.SheetView.Rows[rowIndex].CanFocus = true;
                    if ((int)(e.SheetView.Cells[rowIndex, CHUMON_CHUMON_UMU_COLUMN].Value) == 1)
                    {
                        e.SheetView.Rows[rowIndex].BackColor = CHUMON_BACK_COLOR;
                    }

                    for (columnIndex = CHUMON_SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        // スプレッドの各行の先頭列から得意先ＣＤを取得
                        //                     各列の商品ＣＤは商品名一覧のデータテーブルから取得
                        DataRow[] drs = _dtChumon.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ='{1}'",
                                                    e.SheetView.Cells[rowIndex, CHUMON_TOKUISAKI_CD_COLUMN].Value,
                                                    _syohinDt.Rows[columnIndex - CHUMON_SYOHIN_START_COLUMN]["商品ＣＤ"]));
                        if (drs != null && drs.Length != 0)
                        {
                            if ((int)(drs[0]["注文フラグ"]) == 1)
                            {
                                e.SheetView.Cells[rowIndex, columnIndex].ForeColor = Color.Red;
                            }
                            if ((int)(drs[0]["売上フラグ"]) == 1)
                            {
                                e.SheetView.Cells[rowIndex, columnIndex].ForeColor = URIAGE_FORE_COLOR;
                            }
                        }
                    }
                }

                // 列情報設定
                for (int i = 0; i < fpSpread_Chumon_Sheet1.Columns.Count; i++)
                {
                    e.SheetView.Columns[i].Width = fpSpread_Chumon_Sheet1.Columns[i].Width;
                    e.SheetView.Columns[i].Visible = fpSpread_Chumon_Sheet1.Columns[i].Visible;
                    e.SheetView.Columns[i].HorizontalAlignment = fpSpread_Chumon_Sheet1.Columns[i].HorizontalAlignment;

                    // セルタイプ設定
                    if (fpSpread_Chumon_Sheet1.Columns[i].CellType is FarPoint.Win.Spread.CellType.EmptyCellType)
                    {
                        FarPoint.Win.Spread.CellType.EmptyCellType currentCellType = (FarPoint.Win.Spread.CellType.EmptyCellType)fpSpread_Chumon_Sheet1.Columns[i].CellType;
                        FarPoint.Win.Spread.CellType.EmptyCellType newCellType = new FarPoint.Win.Spread.CellType.EmptyCellType();

                        newCellType.EnableSubEditor = currentCellType.EnableSubEditor;
                        newCellType.Static = currentCellType.Static;
                        newCellType.ReadOnly = currentCellType.ReadOnly;

                        e.SheetView.Columns[i].CellType = newCellType;
                    }

                    if (fpSpread_Chumon_Sheet1.Columns[i].CellType is FarPoint.Win.Spread.CellType.GeneralCellType)
                    {
                        FarPoint.Win.Spread.CellType.GeneralCellType currentCellType = (FarPoint.Win.Spread.CellType.GeneralCellType)fpSpread_Chumon_Sheet1.Columns[i].CellType;
                        FarPoint.Win.Spread.CellType.GeneralCellType newCellType = new FarPoint.Win.Spread.CellType.GeneralCellType();

                        newCellType.EnableSubEditor = currentCellType.EnableSubEditor;
                        newCellType.FormatString = currentCellType.FormatString;
                        newCellType.IsDateFormat = currentCellType.IsDateFormat;
                        newCellType.Static = currentCellType.Static;
                        newCellType.ReadOnly = currentCellType.ReadOnly;

                        e.SheetView.Columns[i].CellType = newCellType;
                    }

                    if (fpSpread_Chumon_Sheet1.Columns[i].CellType is FarPoint.Win.Spread.CellType.NumberCellType)
                    {
                        FarPoint.Win.Spread.CellType.NumberCellType currentCellType = (FarPoint.Win.Spread.CellType.NumberCellType)fpSpread_Chumon_Sheet1.Columns[i].CellType;
                        FarPoint.Win.Spread.CellType.NumberCellType newCellType = new FarPoint.Win.Spread.CellType.NumberCellType();

                        newCellType.DecimalPlaces = currentCellType.DecimalPlaces;
                        newCellType.EnableSubEditor = currentCellType.EnableSubEditor;
                        newCellType.MaximumValue = currentCellType.MaximumValue;
                        newCellType.MinimumValue = currentCellType.MinimumValue;
                        newCellType.ShowSeparator = currentCellType.ShowSeparator;

                        e.SheetView.Columns[i].CellType = newCellType;
                    }

                    if (fpSpread_Chumon_Sheet1.Columns[i].CellType is FarPoint.Win.Spread.CellType.CurrencyCellType)
                    {
                        FarPoint.Win.Spread.CellType.CurrencyCellType currentCellType = (FarPoint.Win.Spread.CellType.CurrencyCellType)fpSpread_Chumon_Sheet1.Columns[i].CellType;
                        FarPoint.Win.Spread.CellType.CurrencyCellType newCellType = new FarPoint.Win.Spread.CellType.CurrencyCellType();

                        newCellType.CurrencySymbol = currentCellType.CurrencySymbol;
                        newCellType.DecimalPlaces = currentCellType.DecimalPlaces;
                        newCellType.EnableSubEditor = currentCellType.EnableSubEditor;
                        newCellType.MaximumValue = currentCellType.MaximumValue;
                        newCellType.MinimumValue = currentCellType.MinimumValue;
                        newCellType.ReadOnly = currentCellType.ReadOnly;
                        newCellType.Separator = currentCellType.Separator;
                        newCellType.ShowSeparator = currentCellType.ShowSeparator;
                        newCellType.Static = currentCellType.Static;

                        e.SheetView.Columns[i].CellType = newCellType;
                    }
                }

                // 親を横スクロール→子を開くと、横スクロール位置が同期していないので子のスクロール位置を同期する
                foreach (FarPoint.Win.Spread.SpreadView childView in fpSpread_Chumon.GetRootWorkbook().GetChildWorkbooks())
                {
                    childView.SetViewportLeftColumn(0, fpSpread_Chumon.GetViewportLeftColumn(0));
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
        /// Spread列幅変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Chumon_ColumnWidthChanged(object sender, FarPoint.Win.Spread.ColumnWidthChangedEventArgs e)
        {
            // すべての子ビュー
            foreach (FarPoint.Win.Spread.SheetView childView in fpSpread_Chumon_Sheet1.GetChildSheets())
            {
                foreach (FarPoint.Win.Spread.ColumnWidthChangeExtents c in e.ColumnList)
                {
                    for (int columnIndex = c.FirstColumn; columnIndex <= c.LastColumn; columnIndex++)
                    {
                        childView.Columns[columnIndex].Width = fpSpread_Chumon_Sheet1.Columns[columnIndex].Width;
                    }
                }
            }
        }

        /// <summary>
        /// Spread左端の列が変更（横スクロール発生時）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Chumon_LeftChange(object sender, FarPoint.Win.Spread.LeftChangeEventArgs e)
        {
            // 親スプレッドをスクロール（子がスクロールした場合）
            fpSpread_Chumon.SetViewportLeftColumn(0, e.NewLeft);

            // 全ての子スプレッドビュー
            foreach (FarPoint.Win.Spread.SpreadView childView in fpSpread_Chumon.GetRootWorkbook().GetChildWorkbooks())
            {
                childView.SetViewportLeftColumn(0, e.NewLeft);
            }
        }

        /// <summary>
        /// 注文Spreadセルダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Chumon_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                // 列ヘッダー or 列フッター or 行ヘッダー？
                if (e.ColumnHeader || e.ColumnFooter || e.RowHeader)
                {
                    return;
                }

                ChumonMeisaiCriteria chumonMeisaiCriteria = GetChumonMeisaiCriteria();
                if (chumonMeisaiCriteria != null)
                {
                    ShowDetail(chumonMeisaiCriteria);       // 詳細表示
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
        /// 注文Spreadキーダウン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread_Chumon_KeyDown(object sender, KeyEventArgs e)
        {
            // OperationMode と キーイベントの関係は以下の通り
            // ┌───────┬────────┬────────┬────────┬────────┐
            // │OperationMode │親ビューKeyPress│親ビューKeyDown │子ビューKeyPress│子ビューKeyDown │
            // ├───────┼────────┼────────┼────────┼────────┤
            // │Normal        │       ○       │       ○       │       ×       │       ○       │
            // ├───────┼────────┼────────┼────────┼────────┤
            // │SingleSelect  │       ○       │      不明      │       ○       │      不明      │
            // └───────┴────────┴────────┴────────┴────────┘
            // SingleSelect なら KeyPress だけでキーイベントが取得できるが、
            // Normal の場合は KeyDown のみを使う必要がある。

            try
            {
                // Enterキー
                if (e.KeyCode == Keys.Enter)
                {
                    log.Debug("");
                    ChumonMeisaiCriteria chumonMeisaiCriteria = GetChumonMeisaiCriteria();
                    if (chumonMeisaiCriteria != null)
                    {
                        ShowDetail(chumonMeisaiCriteria);       // 詳細表示
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
        /// 移動入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIdo_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_courseChumonCriteriaSnapShot == null)
                {
                    return;
                }

                ShowIdo();       // 移動入力詳細表示
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
        /// 注文入力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChumon_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_courseChumonCriteriaSnapShot == null)
                {
                    return;
                }

                if (fpSpread_Chumon_Sheet1.Rows.Count <= 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ChumonMeisaiCriteria chumonMeisaiCriteria = GetChumonMeisaiCriteria();
                if (chumonMeisaiCriteria != null)
                {
                    ShowDetail(chumonMeisaiCriteria);       // 詳細表示（既存レコード）
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
                MessageBox.Show(this, "例外が発生しました。" + ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 売上反映ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUriageHanei_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索未実施？
                if (_courseChumonCriteriaSnapShot == null)
                {
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_courseChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _courseChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show(this, "このコースの注文を売上に反映しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 注文から売上を生成
                CreateUriage();

                btnQuery.PerformClick();        // 再読み込み
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
        /// 配送集計表印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _courseChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_courseChumonCriteriaSnapShot != null)
                {
                    if (_courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
                    {
                        MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                PrintHaisoSyukeihyo();      // 配送集計表印刷
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
        /// 配送明細表印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetailPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _courseChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_courseChumonCriteriaSnapShot != null)
                {
                    if (_courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
                    {
                        MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                PrintHaisoMeisaihyo();      // 配送明細表印刷
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
        /// 注文一覧を表示（階層表示用）
        /// </summary>
        private void GetChumonHierarchical()
        {
            try
            {
                //***************************************************
                // 主要商品リストマスタにある商品名を列として追加し、
                // コーステーブルにある得意先を行とする
                // データそのものは注文明細から取得する
                //***************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetChumonSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                _dtChumon = GetChumonData();

                // 列を指定したデータテーブルを作成
                _cldt = new ColumnListDataTable("コース注文入力");
                // 商品名列を作成
                _cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                _cldt.AddFixedColumn(_dtChumon.Columns["発信"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["得意先ＣＤ"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["得意先名"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["連絡"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["種類"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["注文計"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["現金計"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["備考"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["注文有無フラグ"]);
                // インポート
                _cldt.Import(_dtChumon, "商品ＣＤ", "商品数量");

                //**********************************************************************************************************************************
                // 注文元の注文を配送先の注文に集約する（スプレッドを階層表示する）。
                // SQLの修正を最小限にすべく、元々のコース注文入力のSQLは変更せずに、
                // 自分(=得意先)が注文元なのか配送先なのかを判断して、親テーブルに含めるのか子テーブルに含めるのかを決定する。
                // 例えばコースに属する得意先が 43101～43107の場合（つまり43109は同じコースではない場合）
                // ┌─┬─────────┬───────┬───────┐
                // │No│コースの得意先ＣＤ│配送得意先ＣＤ│含めるテーブル│
                // ├─┼─────────┼───────┼───────┤
                // │ 1│             43101│              │親            │
                // ├─┼─────────┼───────┼───────┤
                // │ 2│             43104│              │親            │
                // ├─┼─────────┼───────┼───────┤
                // │ 3│             43105│         43104│子＆親        │
                // ├─┼─────────┼───────┼───────┤
                // │ 4│             43107│         43109│親            │
                // └─┴─────────┴───────┴───────┘
                // ・コースの得意先ＣＤが配送先を持たず（つまり注文元ではない）かつ配送先にもなっていない場合は、親テーブルのみに含める（1）
                // ・コースの得意先ＣＤが配送先を持たない（つまり注文元ではない）が、配送先になってる場合は、親テーブルに含めて集計対象になる（2）
                // ・コースの得意先ＣＤが配送先を持つ（つまり注文元になる）場合は、子テーブルに含めて、親テーブルの配送得意先レコードに加算する（3）
                // ・コースの得意先ＣＤが配送先を持つ（つまり注文元になる）が、配送先がコースに属していない場合は、親テーブルのみに含める（4）
                //**********************************************************************************************************************************
                DataTable dtHaisosaki = GetHaisosaki();     // 配送先を取得

                // _cldtと同じ構造を持つデータテーブルを２つ用意し、それぞれ親テーブルと子テーブルとする。
                // １．_cldtのレコードの得意先ＣＤと一致するレコードをdtHaisosakiから探す。
                // ２．無い場合（合計行以外はあり得ないはず）は配送先が無いとみなし、_cldtのレコードをそのまま親テーブルにセットする。子テーブルにレコードは作成しない。
                // ３．ある場合は、dtHaisosakiの得意先ＣＤと配送先ＣＤを比較し、
                //   ４．一致する場合は、配送先が無いとみなし、_cldtのレコードをそのまま親テーブルにセットする。子テーブルにレコードは作成しない。
                //   ５．一致しない場合は、配送先があるとみなし、_cldtのレコードを親テーブルの配送先レコードに集計する（レコードが無い場合は作成する）。_cldtのレコードを子テーブルに追加する。
                // このようにしたいが、「配送先レコードが無い場合は作成する」というのが面倒なので、ループを２回実施し、
                // 初回のループでは１～４を実行し、２回目のループでは５のみを実行する。

                _cldt.Columns.Add("配送先ＣＤ", typeof(int));        // 親子の結合用として追加（実際は子にのみ持たせる）
                DataTable dtParent = _cldt.Clone();
                dtParent.TableName = "親";
                DataTable dtChild = _cldt.Clone();
                dtChild.TableName = "子";

                // 親テーブル作成ループ
                foreach (DataRow dr in _cldt.Rows)
                {
                    // １．得意先ＣＤと一致するレコードをdtHaisosakiから探す。
                    int? tokuisakiCD = dr["得意先ＣＤ"] as int?;
                    DataRow drHaisosaki = dtHaisosaki.AsEnumerable().Where(r => r["得意先ＣＤ"] as int? == tokuisakiCD).FirstOrDefault();
                    if (drHaisosaki == null)
                    {
                        // ２．無い場合は、_cldtのレコードをそのまま親テーブルにセットする。
                        dtParent.ImportRow(dr);
                    }
                    else
                    {
                        int? haisosakiCD = drHaisosaki["配送先ＣＤ"] as int?;
                        if (tokuisakiCD == haisosakiCD)
                        {
                            // ４．得意先ＣＤと配送先ＣＤが一致する場合は、_cldtのレコードをそのまま親テーブルにセットする。
                            dtParent.ImportRow(dr);
                        }
                    }
                }

                // 子テーブル作成ループ
                foreach (DataRow dr in _cldt.Rows)
                {
                    // 得意先ＣＤと一致するレコードをdtHaisosakiから探す。
                    int? tokuisakiCD = dr["得意先ＣＤ"] as int?;
                    DataRow drHaisosaki = dtHaisosaki.AsEnumerable().Where(r => r["得意先ＣＤ"] as int? == tokuisakiCD).FirstOrDefault();
                    if (drHaisosaki != null)
                    {
                        int? haisosakiCD = drHaisosaki["配送先ＣＤ"] as int?;
                        if (tokuisakiCD != haisosakiCD)
                        {
                            // ５．一致しない場合
                            // _cldtのレコードを親テーブルの配送先レコードに集計する
                            DataRow drParent = dtParent.AsEnumerable().Where(r => r["得意先ＣＤ"] as int? == haisosakiCD).FirstOrDefault();
                            if (drParent != null)
                            {
                                drParent["注文計"] = (decimal)drParent["注文計"] + (decimal)dr["注文計"];
                                drParent["現金計"] = (decimal)drParent["現金計"] + (decimal)dr["現金計"];
                                foreach (string columnNames in _cldt.ColumnNames)
                                {
                                    if (dr.IsNull(columnNames) == false)
                                    {
                                        drParent[columnNames] = ((drParent[columnNames] as int?) ?? 0) + (int)dr[columnNames];
                                    }
                                }

                                // _cldtのレコードを子テーブルに追加する。
                                dr["配送先ＣＤ"] = haisosakiCD;     // 子のレコードに配送先を持たせる
                                dtChild.ImportRow(dr);
                            }
                        }
                    }
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dtParent);
                ds.Tables.Add(dtChild);

                // 親子のテーブル間でリレーションを作成
                ds.Relations.Add("注文リレーション",
                    new DataColumn[] { dtParent.Columns["得意先ＣＤ"] },
                    new DataColumn[] { dtChild.Columns["配送先ＣＤ"] },
                    false);

                // 数式を消すため、全行削除する
                fpSpread_Chumon_Sheet1.Rows.Count = 0;

                chumonBindingSource.DataSource = null;   // 表示している内容をリセットして、スプレッドの表示が崩れるのを回避する
                chumonBindingSource.DataSource = ds;
                chumonBindingSource.DataMember = "親";

                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_CALL_TIME_COLUMN, "発信");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_RENRAKU_COLUMN, "連絡");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_URIGEN_KUBUN_COLUMN, "種類");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_CHUMON_TOTAL_COLUMN, "注文計");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_GENKIN_TOTAL_COLUMN, "現金計");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_BIKO_COLUMN, "備考");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_CHUMON_UMU_COLUMN, "注文有無フラグ");

                // 商品名を列名としてバインド
                int columnIndex = CHUMON_SYOHIN_START_COLUMN;
                foreach (string columnName in _cldt.ColumnNames)
                {
                    fpSpread_Chumon_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // スプレッドの文字色を以下のように変更
                // 予測：黒
                // 注文：赤
                // 実績：青
                for (int rowIndex = 0; rowIndex < fpSpread_Chumon_Sheet1.Rows.Count - 2; rowIndex++)
                {
                    fpSpread_Chumon_Sheet1.Rows[rowIndex].ResetForeColor();     // 黒に戻す

                    // ついでに注文がある場合の背景色をセット
                    fpSpread_Chumon_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread_Chumon_Sheet1.Rows[rowIndex].CanFocus = true;
                    if ((int)(fpSpread_Chumon_Sheet1.Cells[rowIndex, CHUMON_CHUMON_UMU_COLUMN].Value) == 1)
                    {
                        fpSpread_Chumon_Sheet1.Rows[rowIndex].BackColor = CHUMON_BACK_COLOR;
                    }

                    for (columnIndex = CHUMON_SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        // スプレッドの各行の先頭列から得意先ＣＤを取得
                        //                     各列の商品ＣＤは商品名一覧のデータテーブルから取得
                        DataRow[] drs = _dtChumon.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ='{1}'",
                                                    fpSpread_Chumon_Sheet1.Cells[rowIndex, CHUMON_TOKUISAKI_CD_COLUMN].Value,
                                                    _syohinDt.Rows[columnIndex - CHUMON_SYOHIN_START_COLUMN]["商品ＣＤ"]));
                        if (drs != null && drs.Length != 0)
                        {
                            if ((int)(drs[0]["注文フラグ"]) == 1)
                            {
                                fpSpread_Chumon_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.Red;
                            }
                            if ((int)(drs[0]["売上フラグ"]) == 1)
                            {
                                fpSpread_Chumon_Sheet1.Cells[rowIndex, columnIndex].ForeColor = URIAGE_FORE_COLOR;
                            }
                        }
                    }
                }

                // 最終２行（注文合計と注文＋予測合計）を固定行にする
                fpSpread_Chumon_Sheet1.FrozenTrailingRowCount = 2;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 2].BackColor = TOTAL_BACK_COLOR;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 2].CanFocus = false;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                for (columnIndex = CHUMON_SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                {
                    decimal? chumonTotal = null;
                    decimal? jissekiTotal = null;

                    string syohinCD = (string)_syohinDt.Rows[columnIndex - CHUMON_SYOHIN_START_COLUMN]["商品ＣＤ"];

                    // 注文合計（注文が存在し、売上がないもの）
                    var chumonQuery = _dtChumon.AsEnumerable().Where(r => r["商品ＣＤ"] as string == syohinCD &&
                                                                          r["注文フラグ"] as int? == 1 &&
                                                                          r["売上フラグ"] as int? != 1);
                    if (chumonQuery.Any())
                    {
                        chumonTotal = chumonQuery.Sum(r => r["商品数量"] as decimal?);
                    }

                    // 売上合計（注文の存在は問わず、売上があるもの）
                    var jissekiQuery = _dtChumon.AsEnumerable().Where(r => r["商品ＣＤ"] as string == syohinCD &&
                                                                           r["売上フラグ"] as int? == 1);
                    if (jissekiQuery.Any())
                    {
                        jissekiTotal = jissekiQuery.Sum(r => r["商品数量"] as decimal?);
                    }

                    fpSpread_Chumon_Sheet1.Cells[fpSpread_Chumon_Sheet1.Rows.Count - 2, columnIndex].Value = chumonTotal;   // 注文合計
                    fpSpread_Chumon_Sheet1.Cells[fpSpread_Chumon_Sheet1.Rows.Count - 1, columnIndex].Value = jissekiTotal;  // 実績合計
                }

                // 子テーブルにレコードが無い場合は展開不可とする
                // for (int rowIndex = 0; rowIndex < fpSpread_Chumon_Sheet1.Rows.Count - 2; rowIndex++)
                // {
                //     // 各親行に関連する子ビューを取得
                //     FarPoint.Win.Spread.SheetView sheetView = fpSpread_Chumon_Sheet1.GetChildView(rowIndex, 0);
                //     if (sheetView.RowCount == 0)
                //     {
                //         fpSpread_Chumon_Sheet1.SetRowExpandable(rowIndex, false);   // 展開不可
                //     }
                // }
                // ヘルプに記載されていた↑のやり方だとかなり遅い。↓のやり方だと遜色なく速い。
                for (int rowIndex = 0; rowIndex < fpSpread_Chumon_Sheet1.Rows.Count - 2; rowIndex++)
                {
                    // 自分を配送先とする得意先が（自分を含めて）２件以上あれば子テーブルにレコードがあるとみなす
                    int? tokuisakiCD = fpSpread_Chumon_Sheet1.Cells[rowIndex, CHUMON_TOKUISAKI_CD_COLUMN].Value as int?;
                    if (dtHaisosaki.AsEnumerable().Where(r => r["配送先ＣＤ"] as int? == tokuisakiCD).Count() < 2)
                    {
                        fpSpread_Chumon_Sheet1.SetRowExpandable(rowIndex, false);   // 展開不可
                    }
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _courseChumonCriteriaSnapShot = new CourseChumonCriteria(_courseChumonCriteria);

                fpSpread_Chumon.Focus();
                if (fpSpread_Chumon_Sheet1.Rows.Count != 0)
                {
                    fpSpread_Chumon_Sheet1.ActiveRowIndex = 0;
                    fpSpread_Chumon_Sheet1.SetActiveCell(0, CHUMON_TOKUISAKI_NAME_COLUMN);
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
        /// 注文一覧を表示
        /// </summary>
        private void GetChumon()
        {
            try
            {
                //***************************************************
                // 主要商品リストマスタにある商品名を列として追加し、
                // コーステーブルにある得意先を行とする
                // データそのものは注文明細から取得する
                //***************************************************

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                _syohinDt = GetSyohinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetChumonSpreadColumn(_syohinDt, "商品名略称");

                // データ本体を取得
                _dtChumon = GetChumonData();

                // 列を指定したデータテーブルを作成
                _cldt = new ColumnListDataTable("コース注文入力");
                // 商品名列を作成
                _cldt.AddColumns(_syohinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                _cldt.AddFixedColumn(_dtChumon.Columns["発信"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["得意先ＣＤ"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["得意先名"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["連絡"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["種類"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["注文計"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["現金計"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["備考"]);
                _cldt.AddFixedColumn(_dtChumon.Columns["注文有無フラグ"]);
                // インポート
                _cldt.Import(_dtChumon, "商品ＣＤ", "商品数量");

                DataSet ds = new DataSet();
                ds.Tables.Add(_cldt);

                // 数式を消すため、全行削除する
                fpSpread_Chumon_Sheet1.Rows.Count = 0;

                chumonBindingSource.DataSource = null;   // 表示している内容をリセットして、スプレッドの表示が崩れるのを回避する
                chumonBindingSource.DataSource = ds;
                chumonBindingSource.DataMember = "コース注文入力";

                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_CALL_TIME_COLUMN, "発信");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_RENRAKU_COLUMN, "連絡");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_URIGEN_KUBUN_COLUMN, "種類");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_CHUMON_TOTAL_COLUMN, "注文計");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_GENKIN_TOTAL_COLUMN, "現金計");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_BIKO_COLUMN, "備考");
                fpSpread_Chumon_Sheet1.BindDataColumn(CHUMON_CHUMON_UMU_COLUMN, "注文有無フラグ");

                // 商品名を列名としてバインド
                int columnIndex = CHUMON_SYOHIN_START_COLUMN;
                foreach (string columnName in _cldt.ColumnNames)
                {
                    fpSpread_Chumon_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // スプレッドの文字色を以下のように変更
                // 予測：黒
                // 注文：赤
                // 実績：青
                for (int rowIndex = 0; rowIndex < fpSpread_Chumon_Sheet1.Rows.Count - 2; rowIndex++)
                {
                    fpSpread_Chumon_Sheet1.Rows[rowIndex].ResetForeColor();     // 黒に戻す

                    // ついでに注文がある場合の背景色をセット
                    fpSpread_Chumon_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread_Chumon_Sheet1.Rows[rowIndex].CanFocus = true;
                    if ((int)(fpSpread_Chumon_Sheet1.Cells[rowIndex, CHUMON_CHUMON_UMU_COLUMN].Value) == 1)
                    {
                        fpSpread_Chumon_Sheet1.Rows[rowIndex].BackColor = CHUMON_BACK_COLOR;
                    }

                    for (columnIndex = CHUMON_SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        // スプレッドの各行の先頭列から得意先ＣＤを取得
                        //                     各列の商品ＣＤは商品名一覧のデータテーブルから取得
                        DataRow[] drs = _dtChumon.Select(string.Format("得意先ＣＤ={0} and 商品ＣＤ='{1}'",
                                                    fpSpread_Chumon_Sheet1.Cells[rowIndex, CHUMON_TOKUISAKI_CD_COLUMN].Value,
                                                    _syohinDt.Rows[columnIndex - CHUMON_SYOHIN_START_COLUMN]["商品ＣＤ"]));
                        if (drs != null && drs.Length != 0)
                        {
                            if ((int)(drs[0]["注文フラグ"]) == 1)
                            {
                                fpSpread_Chumon_Sheet1.Cells[rowIndex, columnIndex].ForeColor = Color.Red;
                            }
                            if ((int)(drs[0]["売上フラグ"]) == 1)
                            {
                                fpSpread_Chumon_Sheet1.Cells[rowIndex, columnIndex].ForeColor = URIAGE_FORE_COLOR;
                            }
                        }
                    }
                }

                // 最終２行（注文合計と注文＋予測合計）を固定行にする
                fpSpread_Chumon_Sheet1.FrozenTrailingRowCount = 2;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 2].BackColor = TOTAL_BACK_COLOR;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 2].CanFocus = false;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                for (columnIndex = CHUMON_SYOHIN_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                {
                    decimal? chumonTotal = null;
                    decimal? jissekiTotal = null;

                    string syohinCD = (string)_syohinDt.Rows[columnIndex - CHUMON_SYOHIN_START_COLUMN]["商品ＣＤ"];

                    // 注文合計（注文が存在し、売上がないもの）
                    var chumonQuery = _dtChumon.AsEnumerable().Where(r => r["商品ＣＤ"] as string == syohinCD &&
                                                                          r["注文フラグ"] as int? == 1 &&
                                                                          r["売上フラグ"] as int? != 1);
                    if (chumonQuery.Any())
                    {
                        chumonTotal = chumonQuery.Sum(r => r["商品数量"] as decimal?);
                    }

                    // 売上合計（注文の存在は問わず、売上があるもの）
                    var jissekiQuery = _dtChumon.AsEnumerable().Where(r => r["商品ＣＤ"] as string == syohinCD &&
                                                                           r["売上フラグ"] as int? == 1);
                    if (jissekiQuery.Any())
                    {
                        jissekiTotal = jissekiQuery.Sum(r => r["商品数量"] as decimal?);
                    }

                    fpSpread_Chumon_Sheet1.Cells[fpSpread_Chumon_Sheet1.Rows.Count - 2, columnIndex].Value = chumonTotal;   // 注文合計
                    fpSpread_Chumon_Sheet1.Cells[fpSpread_Chumon_Sheet1.Rows.Count - 1, columnIndex].Value = jissekiTotal;  // 実績合計
                }

                // 検索実行時の検索条件を保存（DB登録時に使用する）
                _courseChumonCriteriaSnapShot = new CourseChumonCriteria(_courseChumonCriteria);

                fpSpread_Chumon.Focus();
                if (fpSpread_Chumon_Sheet1.Rows.Count != 0)
                {
                    fpSpread_Chumon_Sheet1.ActiveRowIndex = 0;
                    fpSpread_Chumon_Sheet1.SetActiveCell(0, CHUMON_TOKUISAKI_NAME_COLUMN);
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
        /// 注文スプレッドの列として採用する商品名の一覧をデータテーブルとして取得する
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

                    // -- 以下のように並べる
                    // -- ┌───┬───┬───┐
                    // -- │商品Ａ│商品Ｂ│その他│
                    // -- └───┴───┴───┘
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     0 AS その他区分" +
                                      "\r\n     ,A.商品ＣＤ AS ソート用商品ＣＤ" +
                                      "\r\n     ,CONVERT(varchar, A.商品ＣＤ) AS 商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n FROM" +
                                      "\r\n     主要商品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n     AND ISNULL(A.無効フラグ, 0) = 0" +
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     1 AS その他区分" +
                                      "\r\n     ,-1 AS ソート用商品ＣＤ" +
                                      "\r\n     ,'XXX' AS 商品ＣＤ" +
                                      "\r\n     ,'その他' AS 商品名略称" +
                                      "\r\n ORDER BY" +
                                      "\r\n     その他区分" +
                                      "\r\n     ,ソート用商品ＣＤ" +
                                      "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;

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
        /// 商品名一覧のデータテーブルを基にして、注文スプレッドに列を追加する
        /// </summary>
        /// <param name="syohinDt">商品名一覧データテーブル</param>
        /// <param name="labelColumnName">商品名が格納されている列の名前</param>
        private void SetChumonSpreadColumn(DataTable syohinDt, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread_Chumon_Sheet1.ColumnCount = CHUMON_SYOHIN_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread_Chumon_Sheet1.ColumnCount = CHUMON_SYOHIN_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = CHUMON_SYOHIN_START_COLUMN;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = 0D;
                    numberCellType.ReadOnly = true;
                    numberCellType.Static = true;
                    fpSpread_Chumon_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread_Chumon_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread_Chumon_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread_Chumon_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread_Chumon_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
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
        /// コース注文入力のデータ本体を取得
        /// </summary>
        /// <returns>データ本体になるデータテーブル</returns>
        private DataTable GetChumonData()
        {
            try
            {
                DataTable dt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     0 AS 明細合計区分,");
                    sql.AppendLine("     C.得意先ＣＤ,");
                    sql.AppendLine("     CASE T.受注方法 WHEN 1 THEN RIGHT('00' + CONVERT(varchar, T.電話確認時間_時), 2) + ':' + RIGHT('00' + CONVERT(varchar, T.電話確認時間_分), 2)");
                    sql.AppendLine("                     ELSE '不' END AS 発信,");
                    if (SystemMaster.GetDispTokuisakiRyaku())
                    {
                        sql.AppendLine("     T.得意先名略称 AS 得意先名,");
                    }
                    else
                    {
                        sql.AppendLine("     T.得意先名,");
                    }
                    sql.AppendLine("     THR.連絡,");
                    sql.AppendLine("     VK.売掛現金区分名 AS 種類,");
                    sql.AppendLine("     ISNULL(CK.注文計, 0) AS 注文計,");
                    sql.AppendLine("     ISNULL(UK.現金計, 0) AS 現金計,");
                    sql.AppendLine("     ISNULL(H.備考1, T.備考1) AS 備考,");
                    sql.AppendLine("     A.商品ＣＤ,");
                    sql.AppendLine("     A.商品予測数量,");
                    sql.AppendLine("     A.商品注文数量,");
                    sql.AppendLine("     A.商品売上数量,");
                    sql.AppendLine("     CASE WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN 1 ELSE 0 END AS 注文フラグ,");
                    sql.AppendLine("     CASE WHEN ISNULL(A.商品売上数量, -1) >= 0 THEN 1 ELSE 0 END AS 売上フラグ,");
                    sql.AppendLine("     CASE WHEN ISNULL(A.商品売上数量, -1) >= 0 THEN A.商品売上数量");
                    sql.AppendLine("          WHEN ISNULL(A.商品注文数量, -1) >= 0 THEN A.商品注文数量");
                    sql.AppendLine("          ELSE A.商品予測数量 END AS 商品数量,");
                    sql.AppendLine("     CASE WHEN CK.注文計 IS NULL THEN 0 ELSE 1 END AS 注文有無フラグ,");
                    sql.AppendLine("     CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順,");
                    sql.AppendLine("     CO.コース順");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    // -- ①－１（指定されたコースにぶら下がる得意先）
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         CT.企業ＣＤ");
                    sql.AppendLine("         ,CT.コースＣＤ");
                    sql.AppendLine("         ,CT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コーステーブル AS CT");
                    sql.AppendLine("         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 コースＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 臨時コースマスタ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                 AND 実施日 = @注文日");
                    sql.AppendLine("             ) AS RM");
                    sql.AppendLine("             ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         RT.企業ＣＤ");
                    sql.AppendLine("         ,RT.コースＣＤ");
                    sql.AppendLine("         ,RT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         臨時コーステーブル AS RT");
                    sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RT.実施日 = @注文日");
                    // -- ①－２（指定されたコースを持つ注文の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         注文ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    // -- ①－３（指定されたコースを持つ売上の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 売上日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     ) AS C");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_売掛現金区分 AS VK ON VK.売掛現金区分CD = ISNULL(T.売掛現金区分, @売掛現金区分現金)");      // -- デフォルトは現金
                    // -- ②駆動表に紐付く注文と売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             A1.コースＣＤ");
                    sql.AppendLine("             ,A1.得意先ＣＤ");
                    sql.AppendLine("             ,A1.商品ＣＤ");
                    sql.AppendLine("             ,SUM(A1.商品注文数量) AS 商品注文数量");
                    sql.AppendLine("             ,SUM(A1.商品予測数量) AS 商品予測数量");
                    sql.AppendLine("             ,SUM(A1.商品売上数量) AS 商品売上数量");
                    sql.AppendLine("         FROM");
                    // -- ②－１駆動表に紐付く注文
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.コースＣＤ");
                    sql.AppendLine("                 ,M.得意先ＣＤ");
                    sql.AppendLine("                 ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') AS 商品ＣＤ");
                    sql.AppendLine("                 ,SUM(CASE WHEN M.注文区分 = @注文区分注文 THEN M.商品数量 ELSE null END) AS 商品注文数量");
                    sql.AppendLine("                 ,SUM(CASE WHEN M.注文区分 = @注文区分予測 THEN M.商品数量 ELSE null END) AS 商品予測数量");
                    sql.AppendLine("                 ,null AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 注文ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                     ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                     AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                     AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                     AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("                 LEFT JOIN 主要商品リストマスタ AS A");
                    sql.AppendLine("                     ON A.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                     AND A.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                     AND A.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     AND ISNULL(A.無効フラグ, 0) = 0");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND H.喫食日 = @注文日");
                    sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ");
                    sql.AppendLine("                 ,M.得意先ＣＤ");
                    sql.AppendLine("                 ,A.商品ＣＤ");
                    sql.AppendLine("             UNION");
                    // -- ②－２駆動表に紐付く売上
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.コースＣＤ");
                    sql.AppendLine("                 ,M.得意先ＣＤ");
                    sql.AppendLine("                 ,ISNULL(CONVERT(varchar, A.商品ＣＤ), 'XXX') AS 商品ＣＤ");
                    sql.AppendLine("                 ,null AS 商品注文数量");
                    sql.AppendLine("                 ,null AS 商品予測数量");
                    sql.AppendLine("                 ,SUM(M.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS M");
                    sql.AppendLine("                     ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                     AND H.売上日     = M.売上日");
                    sql.AppendLine("                     AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                     AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("                 LEFT JOIN 主要商品リストマスタ AS A");
                    sql.AppendLine("                     ON A.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                     AND A.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                     AND A.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("                     AND ISNULL(A.無効フラグ, 0) = 0");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND M.売上日 = @注文日");
                    sql.AppendLine("                 AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.コースＣＤ");
                    sql.AppendLine("                 ,M.得意先ＣＤ");
                    sql.AppendLine("                 ,A.商品ＣＤ");
                    sql.AppendLine("             ) AS A1");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             A1.コースＣＤ");
                    sql.AppendLine("             ,A1.得意先ＣＤ");
                    sql.AppendLine("             ,A1.商品ＣＤ");
                    sql.AppendLine("         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ AND A.コースＣＤ = C.コースＣＤ");
                    // -- ③備考取得用
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             得意先ＣＤ");
                    sql.AppendLine("             ,備考1");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 注文区分 = 0");
                    sql.AppendLine("             AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("             AND 喫食日 = @注文日");
                    sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         ) AS H");
                    sql.AppendLine("         ON H.得意先ＣＤ = C.得意先ＣＤ");
                    // -- ④注文計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 注文計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ");
                    // -- ⑤現金計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 現金計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.売上日 = @注文日");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.売上区分 = @売上区分現金");
                    // 2016/08/23 DEL S チケット束売りも含める
                    //sql.AppendLine("             AND ISNULL(M.チケット束フラグ, 0) = 0");   // -- チケット束売りは除く
                    // 2016/08/23 DEL E
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ");
                    // -- ⑥連絡（得意先発信履歴データ）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT DISTINCT");
                    sql.AppendLine("             THR_A.得意先ＣＤ");
                    sql.AppendLine("             ,THR_A.最終発信日時");
                    sql.AppendLine("             ,THR_A.不要フラグ");
                    sql.AppendLine("             ,THR_A.保留フラグ");
                    sql.AppendLine("             ,CASE WHEN THR_A.保留フラグ = 1 THEN '保留'");
                    sql.AppendLine("                   WHEN THR_A.最終発信日時 IS NOT NULL THEN '済'");
                    sql.AppendLine("                   ELSE '未'");
                    sql.AppendLine("              END AS 連絡");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             得意先発信履歴データ AS THR_A");
                    sql.AppendLine("             INNER JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     得意先ＣＤ");
                    sql.AppendLine("                     ,MAX(修正日) AS 修正日");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     得意先発信履歴データ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND 発信日 = @注文日");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     得意先ＣＤ");
                    sql.AppendLine("             ) AS THR_M ON THR_M.得意先ＣＤ = THR_A.得意先ＣＤ AND THR_M.修正日 = THR_A.修正日");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             THR_A.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND THR_A.発信日 = @注文日");
                    sql.AppendLine("         ) AS THR ON THR.得意先ＣＤ = C.得意先ＣＤ");
                    // -- ⑦コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             CT.得意先ＣＤ");
                    sql.AppendLine("             ,CT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コーステーブル AS CT");
                    sql.AppendLine("             INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     臨時コースマスタ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                     AND 実施日 = @注文日");
                    sql.AppendLine("                 ) AS RM");
                    sql.AppendLine("                 ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("             AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RM.コースＣＤ IS NULL");
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             RT.得意先ＣＤ");
                    sql.AppendLine("             ,RT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コーステーブル AS RT");
                    sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RT.実施日 = @注文日");
                    sql.AppendLine("         ) AS CO ON C.得意先ＣＤ = CO.得意先ＣＤ");
                    sql.AppendLine(" WHERE ");
                    sql.AppendLine("     1 = 1 ");
                    sql.AppendLine((string)cboChumon.SelectedValue);
                    sql.AppendLine((string)cboRenraku.SelectedValue);
                    // -- ⑧注文合計
                    sql.AppendLine(" UNION");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     1 AS 明細合計区分,");
                    sql.AppendLine("     NULL AS 得意先ＣＤ,");
                    sql.AppendLine("     NULL AS 発信,");
                    sql.AppendLine("     '受注合計' AS 得意先名,");
                    sql.AppendLine("     NULL AS 連絡,");
                    sql.AppendLine("     NULL AS 種類,");
                    sql.AppendLine("     NULL AS 注文計,");
                    sql.AppendLine("     NULL AS 現金計,");
                    sql.AppendLine("     NULL AS 備考,");
                    sql.AppendLine("     NULL AS 商品ＣＤ,");
                    sql.AppendLine("     NULL AS 商品予測数量,");
                    sql.AppendLine("     NULL AS 商品注文数量,");
                    sql.AppendLine("     NULL AS 商品売上数量,");
                    sql.AppendLine("     NULL AS 注文フラグ,");
                    sql.AppendLine("     NULL AS 売上フラグ,");
                    sql.AppendLine("     NULL AS 商品数量,");
                    sql.AppendLine("     -1 AS 注文有無フラグ,");
                    sql.AppendLine("     -1 AS コースに含む順,");
                    sql.AppendLine("     -1 AS コース順");
                    // -- ⑨実績合計
                    sql.AppendLine(" UNION");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     2 AS 明細合計区分,");
                    sql.AppendLine("     NULL AS 得意先ＣＤ,");
                    sql.AppendLine("     NULL AS 発信,");
                    sql.AppendLine("     '実績合計' AS 得意先名,");
                    sql.AppendLine("     NULL AS 連絡,");
                    sql.AppendLine("     NULL AS 種類,");
                    sql.AppendLine("     NULL AS 注文計,");
                    sql.AppendLine("     NULL AS 現金計,");
                    sql.AppendLine("     NULL AS 備考,");
                    sql.AppendLine("     NULL AS 商品ＣＤ,");
                    sql.AppendLine("     NULL AS 商品予測数量,");
                    sql.AppendLine("     NULL AS 商品注文数量,");
                    sql.AppendLine("     NULL AS 商品売上数量,");
                    sql.AppendLine("     NULL AS 注文フラグ,");
                    sql.AppendLine("     NULL AS 売上フラグ,");
                    sql.AppendLine("     NULL AS 商品数量,");
                    sql.AppendLine("     -1 AS 注文有無フラグ,");
                    sql.AppendLine("     -1 AS コースに含む順,");
                    sql.AppendLine("     -1 AS コース順");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     明細合計区分");
                    sql.AppendLine("     ,コースに含む順");
                    sql.AppendLine("     ,コース順");
                    sql.AppendLine("     ,得意先ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@注文区分予測", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Yosoku;
                    cmd.Parameters.Add("@売上区分現金", SqlDbType.Int).Value = (int)Consts.UriageKubun.Genkin;
                    cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース注文入力リストを検索しました。");
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
        /// 配送先を取得
        /// </summary>
        /// <returns>得意先と配送先を含むデータテーブル</returns>
        private DataTable GetHaisosaki()
        {
            try
            {
                DataTable dt = new DataTable("集計先");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n WITH 対象コース得意先 AS (" +
                                      // -- ①－１（指定されたコースにぶら下がる得意先）
                                      // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                                      "\r\n SELECT" +
                                      "\r\n     CT.企業ＣＤ" +
                                      "\r\n     ,CT.コースＣＤ" +
                                      "\r\n     ,CT.得意先ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     コーステーブル AS CT" +
                                      "\r\n     INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0" +
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             コースＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             臨時コースマスタ" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND ISNULL(無効フラグ, 0) = 0" +
                                      "\r\n             AND コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND 実施日 = @注文日" +
                                      "\r\n         ) AS RM" +
                                      "\r\n         ON RM.コースＣＤ = CT.コースＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     CT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND CT.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND ISNULL(CT.無効フラグ, 0) = 0" +
                                      "\r\n     AND RM.コースＣＤ IS NULL" +
                                      // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     RT.企業ＣＤ" +
                                      "\r\n     ,RT.コースＣＤ" +
                                      "\r\n     ,RT.得意先ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     臨時コーステーブル AS RT" +
                                      "\r\n     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                                      "\r\n WHERE" +
                                      "\r\n     RT.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND RT.コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND ISNULL(RT.無効フラグ, 0) = 0" +
                                      "\r\n     AND RT.実施日 = @注文日" +
                                      // -- ①－２（指定されたコースを持つ注文の得意先）
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     企業ＣＤ" +
                                      "\r\n     ,コースＣＤ" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     注文ヘッダ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND 喫食日 = @注文日" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      // -- ①－３（指定されたコースを持つ売上の得意先）
                                      "\r\n UNION" +
                                      "\r\n SELECT" +
                                      "\r\n     企業ＣＤ" +
                                      "\r\n     ,コースＣＤ" +
                                      "\r\n     ,得意先ＣＤ" +
                                      "\r\n FROM" +
                                      "\r\n     売上ヘッダ" +
                                      "\r\n WHERE" +
                                      "\r\n     企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n     AND 売上日 = @注文日" +
                                      "\r\n     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n )" +
                                      "\r\n SELECT DISTINCT" +
                                      "\r\n     D.企業ＣＤ" +
                                      "\r\n     ,D.コースＣＤ" +
                                      "\r\n     ,D.得意先ＣＤ" +
                                      "\r\n     ,D.配送先ＣＤ" +
                                      "\r\n FROM" +
                                      // -- ①－１（指定されたコースにぶら下がる得意先(集約先)）
                                      "\r\n     (SELECT" +
                                      "\r\n         TH.企業ＣＤ" +
                                      "\r\n         ,TH.コースＣＤ" +
                                      "\r\n         ,TH.得意先ＣＤ" +
                                      "\r\n         ,TH.配送得意先ＣＤ" +
                                      "\r\n         ,SY.集約先ＣＤ" +
                                      "\r\n         ,ISNULL(SY.集約先ＣＤ, TH.得意先ＣＤ) AS 配送先ＣＤ" +
                                      "\r\n     FROM" +
                                      // -- ①－１－１（指定されたコースにぶら下がる得意先と配送先）
                                      "\r\n         (SELECT" +
                                      "\r\n             W.企業ＣＤ" +
                                      "\r\n             ,W.得意先ＣＤ" +
                                      "\r\n             ,T.配送得意先ＣＤ" +
                                      "\r\n             ,W.コースＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             対象コース得意先 AS W" +
                                      // -- 得意先と配送先を取得したいのでLEFT JOINしている
                                      "\r\n             LEFT JOIN 得意先マスタ AS T" +
                                      "\r\n                 ON T.企業ＣＤ = W.企業ＣＤ" +
                                      "\r\n                 AND T.得意先ＣＤ = W.得意先ＣＤ" +
                                      "\r\n         ) AS TH" +
                                      // -- ①－１－２（指定されたコースにぶら下がる得意先のうち、注文の集約先になりうる得意先を抽出）
                                      "\r\n         LEFT JOIN" +
                                      "\r\n             (SELECT DISTINCT" +
                                      "\r\n                 W.企業ＣＤ" +
                                      "\r\n                 ,W.得意先ＣＤ" +
                                      "\r\n                 ,T.配送得意先ＣＤ AS 集約先ＣＤ" +
                                      "\r\n                 ,W.コースＣＤ" +
                                      "\r\n             FROM" +
                                      "\r\n                 対象コース得意先 AS W" +
                                      // -- 集約先になりうる得意先のみを抽出したいのでINNER JOINしている
                                      // -- ホントは 「ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ」 として取得したかったが、インデックスが効かなくなるので
                                      // -- T.配送得意先ＣＤ には NULL を格納せず、代わりに得意先ＣＤを格納している
                                      "\r\n                 INNER JOIN 得意先マスタ AS T" +
                                      "\r\n                     ON T.企業ＣＤ = W.企業ＣＤ" +
                                      "\r\n                     AND T.配送得意先ＣＤ = W.得意先ＣＤ" +
                                      "\r\n             ) AS SY" +
                                      "\r\n             ON ISNULL(TH.配送得意先ＣＤ, TH.得意先ＣＤ) = SY.集約先ＣＤ" +      // -- 集約先と結合するのは配送先を優先する（集約先≒配送先なので）
                                      "\r\n     ) AS D" +
                                      "\r\n ORDER BY" +
                                      "\r\n     D.企業ＣＤ" +
                                      "\r\n     ,D.コースＣＤ" +
                                      "\r\n     ,D.得意先ＣＤ" +
                                      "\r\n     ,D.配送先ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース注文入力リストの集計先を検索しました。");
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
        /// 部品状況を表示
        /// </summary>
        private void GetBuhin()
        {
            try
            {
                //************************************************************
                // 主要部品リストマスタにある商品名を列として追加し、
                // 持出・受注・差を行とする
                // データそのものはモバイル_持ち出し入力と注文明細から取得する
                //************************************************************
                DataTable buhinDt = GetBuhinTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetBuhinSpreadColumn(buhinDt, "商品名略称");

                // データ本体を取得
                DataTable dt = new DataTable("部品状況");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT " +
                                      "\r\n     A.行番号" +
                                      "\r\n     ,A.見出し" +
                                      "\r\n     ,B.商品ＣＤ" +
                                      "\r\n     ,B.商品名略称" +
                                      "\r\n     ,B.数量" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT " +
                                      "\r\n         0 AS 行番号" +
                                      "\r\n         ,'持出' AS 見出し" +
                                      "\r\n     UNION" +
                                      "\r\n     SELECT " +
                                      "\r\n         1 AS 行番号" +
                                      "\r\n         ,'移動' AS 見出し" +
                                      "\r\n     UNION" +
                                      "\r\n     SELECT " +
                                      "\r\n         2 AS 行番号" +
                                      "\r\n         ,'受注' AS 見出し" +
                                      "\r\n     ) AS A" +
                                      "\r\n     LEFT JOIN" +
                                      // -- 持出行
                                      "\r\n     (SELECT" +
                                      "\r\n         0 AS 行番号" +
                                      "\r\n         ,'持出' AS 見出し" +
                                      "\r\n         ,MAIN.商品ＣＤ" +
                                      "\r\n         ,MAIN.商品名略称" +
                                      "\r\n         ,ISNULL(MCDS.個数, 0) AS 数量" +
                                      "\r\n     FROM" +
                                      // -- ①駆動表（指定されたコースの商品）
                                      "\r\n         (SELECT" +
                                      "\r\n             P.商品ＣＤ" +
                                      "\r\n             ,S.商品名略称" +
                                      "\r\n         FROM" +
                                      "\r\n             主要部品リストマスタ AS P" +
                                      "\r\n             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             P.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND P.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n         ) AS MAIN" +
                                      "\r\n         LEFT JOIN" +
                                      // -- ③持ち出し数
                                      "\r\n         (SELECT" +
                                      "\r\n             商品ＣＤ" +
                                      "\r\n             ,SUM(個数) AS 個数" +
                                      "\r\n         FROM" +
                                      "\r\n             モバイル_持ち出し入力" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND 持出日 = @注文日" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             商品ＣＤ" +
                                      "\r\n         ) AS MCDS ON MAIN.商品ＣＤ = MCDS.商品ＣＤ" +
                                      "\r\n     UNION" +
                                      // -- 移動行
                                      "\r\n     SELECT" +
                                      "\r\n         1 AS 行番号" +
                                      "\r\n         ,'移動' AS 見出し" +
                                      "\r\n         ,MAIN.商品ＣＤ" +
                                      "\r\n         ,MAIN.商品名略称" +
                                      // -- 受け取った個数－渡した個数を移動数とする
                                      "\r\n         ,ISNULL(TAKE.個数, 0) - ISNULL(GIVE.個数, 0) AS 数量" +
                                      "\r\n     FROM" +
                                      // -- ①駆動表（指定されたコースの商品）
                                      "\r\n         (SELECT" +
                                      "\r\n             P.商品ＣＤ" +
                                      "\r\n             ,S.商品名略称" +
                                      "\r\n         FROM" +
                                      "\r\n             主要部品リストマスタ AS P" +
                                      "\r\n             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             P.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND P.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n         ) AS MAIN" +
                                      "\r\n         LEFT JOIN" +
                                      // -- ④もらった個数
                                      "\r\n         (SELECT" +
                                      "\r\n             商品ＣＤ" +
                                      "\r\n             ,SUM(個数) AS 個数" +
                                      "\r\n         FROM" +
                                      "\r\n             モバイル_移動入力" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND 相手コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND 移動日 = @注文日" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             商品ＣＤ" +
                                      "\r\n         ) AS TAKE ON MAIN.商品ＣＤ = TAKE.商品ＣＤ" +
                                      "\r\n         LEFT JOIN" +
                                      // -- ⑥渡した個数
                                      "\r\n         (SELECT" +
                                      "\r\n             コースＣＤ" +
                                      "\r\n             ,商品ＣＤ" +
                                      "\r\n             ,SUM(個数) AS 個数" +
                                      "\r\n         FROM" +
                                      "\r\n             モバイル_移動入力" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND コースＣＤ = @コースＣＤ" +
                                      "\r\n             AND 移動日 = @注文日" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             コースＣＤ" +
                                      "\r\n             ,商品ＣＤ" +
                                      "\r\n         ) AS GIVE ON MAIN.商品ＣＤ = GIVE.商品ＣＤ" +
                                      "\r\n     UNION" +
                                      // -- 受注行
                                      "\r\n     SELECT" +
                                      "\r\n         2 AS 行番号" +
                                      "\r\n         ,'受注' AS 見出し" +
                                      "\r\n         ,MAIN.商品ＣＤ" +
                                      "\r\n         ,MAIN.商品名略称" +
                                      "\r\n         ,ISNULL(INIT.数量, 0) AS 数量" +
                                      "\r\n     FROM" +
                                      // -- ①駆動表（指定されたコースの商品）
                                      "\r\n         (SELECT" +
                                      "\r\n             P.商品ＣＤ" +
                                      "\r\n             ,S.商品名略称" +
                                      "\r\n         FROM" +
                                      "\r\n             主要部品リストマスタ AS P" +
                                      "\r\n             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = P.企業ＣＤ AND S.商品ＣＤ = P.商品ＣＤ" +
                                      "\r\n         WHERE" +
                                      "\r\n             P.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n             AND P.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n             AND ISNULL(S.無効フラグ, 0) = 0" +
                                      "\r\n         ) AS MAIN" +
                                      "\r\n         LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             PARTSUM.部品ＣＤ" +
                                      "\r\n             ,SUM(PARTSUM.数量) AS 数量" +
                                      "\r\n         FROM" +
                                      // -- ②予測と注文と売上の部品（各商品を分割した部品ごとに合計する）
                                      "\r\n             (SELECT" +
                                      "\r\n                 PART.得意先ＣＤ" +
                                      "\r\n                 ,PART.部品ＣＤ" +
                                      "\r\n                 ,CASE WHEN ISNULL(SUM(PART.商品売上数量), -1) >= 0 THEN SUM(PART.商品売上数量)" +
                                      "\r\n                       ELSE ISNULL(SUM(PART.商品注文予測数量), 0) END" +
                                      "\r\n                  AS 数量" +
                                      "\r\n             FROM" +
                                      // -- ②－１ 予測と注文の弁当の主食
                                      "\r\n                 (SELECT" +
                                      "\r\n                     PART1.得意先ＣＤ" +
                                      "\r\n                     ,PART1.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART1.商品注文数量, PART1.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.主食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                              (SELECT" +
                                      "\r\n                                 H.企業ＣＤ," +
                                      "\r\n                                 H.売上日," +
                                      "\r\n                                 H.得意先ＣＤ," +
                                      "\r\n                                 H.部署ＣＤ," +
                                      "\r\n                                 H.コースＣＤ," +
                                      "\r\n                                 D.商品ＣＤ," +
                                      "\r\n                                 D.明細番号" +
                                      "\r\n                              FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                  (SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      (SELECT" +
                                      "\r\n                                          企業ＣＤ," +
                                      "\r\n                                          部署ＣＤ," +
                                      "\r\n                                          コースＣＤ," +
                                      "\r\n                                          得意先ＣＤ," +
                                      "\r\n                                          喫食日 AS 売上日" +
                                      "\r\n                                      FROM" +
                                      "\r\n                                          注文ヘッダ" +
                                      "\r\n                                      WHERE" +
                                      "\r\n                                          企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                          AND 配送日   = @注文日" +
                                      "\r\n                                          AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                          AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                      INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                          T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                          AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                          AND T1.売上日     = T2.売上日" +
                                      "\r\n                                          AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                          AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                  UNION" +
                                      "\r\n                                  SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      売上ヘッダ T1" +
                                      "\r\n                                  WHERE" +
                                      "\r\n                                      T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                      AND T1.売上日   = @注文日" +
                                      "\r\n                                      AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                      AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      AND NOT EXISTS (" +
                                      "\r\n                                          SELECT 'X'" +
                                      "\r\n                                          FROM" +
                                      "\r\n                                              (SELECT" +
                                      "\r\n                                                  企業ＣＤ," +
                                      "\r\n                                                  部署ＣＤ," +
                                      "\r\n                                                  コースＣＤ," +
                                      "\r\n                                                  得意先ＣＤ," +
                                      "\r\n                                                  喫食日 AS 売上日" +
                                      "\r\n                                              FROM" +
                                      "\r\n                                                  注文ヘッダ" +
                                      "\r\n                                              WHERE" +
                                      "\r\n                                                  企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                  AND 配送日   = @注文日" +
                                      "\r\n                                                  AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                  AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                              ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                          WHERE" +
                                      "\r\n                                              T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                              AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                              AND T1.売上日     = T2.売上日" +
                                      "\r\n                                              AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                              AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                  ) AS H" +
                                      "\r\n                                  INNER JOIN 売上明細 AS D" +
                                      "\r\n                                      ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                      AND D.売上日     = H.売上日" +
                                      "\r\n                                      AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                      AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                              ) AS KHD" +
                                      "\r\n                              ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                              AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                              AND D.喫食日 = KHD.売上日" +
                                      "\r\n                              AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                              AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                              AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                         AND ISNULL(I.主食ＣＤ, 0) <> 0" +               // -- 主食がある
                                      "\r\n                         AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.主食ＣＤ" +
                                      "\r\n                     ) AS PART1" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART1.得意先ＣＤ" +
                                      "\r\n                     ,PART1.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +    // -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                                                                            // -- ②－２ 予測と注文の弁当の副食
                                      "\r\n                 SELECT" +
                                      "\r\n                     PART2.得意先ＣＤ" +
                                      "\r\n                     ,PART2.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART2.商品注文数量, PART2.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.副食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                              (SELECT" +
                                      "\r\n                                 H.企業ＣＤ," +
                                      "\r\n                                 H.売上日," +
                                      "\r\n                                 H.得意先ＣＤ," +
                                      "\r\n                                 H.部署ＣＤ," +
                                      "\r\n                                 H.コースＣＤ," +
                                      "\r\n                                 D.商品ＣＤ," +
                                      "\r\n                                 D.明細番号" +
                                      "\r\n                              FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                  (SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      (SELECT" +
                                      "\r\n                                          企業ＣＤ," +
                                      "\r\n                                          部署ＣＤ," +
                                      "\r\n                                          コースＣＤ," +
                                      "\r\n                                          得意先ＣＤ," +
                                      "\r\n                                          喫食日 AS 売上日" +
                                      "\r\n                                      FROM" +
                                      "\r\n                                          注文ヘッダ" +
                                      "\r\n                                      WHERE" +
                                      "\r\n                                          企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                          AND 配送日   = @注文日" +
                                      "\r\n                                          AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                          AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                      INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                          T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                          AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                          AND T1.売上日     = T2.売上日" +
                                      "\r\n                                          AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                          AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                  UNION" +
                                      "\r\n                                  SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      売上ヘッダ T1" +
                                      "\r\n                                  WHERE" +
                                      "\r\n                                      T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                      AND T1.売上日   = @注文日" +
                                      "\r\n                                      AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                      AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      AND NOT EXISTS (" +
                                      "\r\n                                          SELECT 'X'" +
                                      "\r\n                                          FROM" +
                                      "\r\n                                              (SELECT" +
                                      "\r\n                                                  企業ＣＤ," +
                                      "\r\n                                                  部署ＣＤ," +
                                      "\r\n                                                  コースＣＤ," +
                                      "\r\n                                                  得意先ＣＤ," +
                                      "\r\n                                                  喫食日 AS 売上日" +
                                      "\r\n                                              FROM" +
                                      "\r\n                                                  注文ヘッダ" +
                                      "\r\n                                              WHERE" +
                                      "\r\n                                                  企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                  AND 配送日   = @注文日" +
                                      "\r\n                                                  AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                  AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                              ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                          WHERE" +
                                      "\r\n                                              T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                              AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                              AND T1.売上日     = T2.売上日" +
                                      "\r\n                                              AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                              AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                  ) AS H" +
                                      "\r\n                                  INNER JOIN 売上明細 AS D" +
                                      "\r\n                                      ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                      AND D.売上日     = H.売上日" +
                                      "\r\n                                      AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                      AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                              ) AS KHD" +
                                      "\r\n                              ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                              AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                              AND D.喫食日 = KHD.売上日" +
                                      "\r\n                              AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                              AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                              AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                         AND ISNULL(I.副食ＣＤ, 0) <> 0" +               // -- 副食がある
                                      "\r\n                         AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.副食ＣＤ" +
                                      "\r\n                     ) AS PART2" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART2.得意先ＣＤ" +
                                      "\r\n                     ,PART2.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－３ 予測と注文の主食のみ
                                      "\r\n                 SELECT" +
                                      "\r\n                     PART3.得意先ＣＤ" +
                                      "\r\n                     ,PART3.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART3.商品注文数量, PART3.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.主食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                              (SELECT" +
                                      "\r\n                                 H.企業ＣＤ," +
                                      "\r\n                                 H.売上日," +
                                      "\r\n                                 H.得意先ＣＤ," +
                                      "\r\n                                 H.部署ＣＤ," +
                                      "\r\n                                 H.コースＣＤ," +
                                      "\r\n                                 D.商品ＣＤ," +
                                      "\r\n                                 D.明細番号" +
                                      "\r\n                              FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                  (SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      (SELECT" +
                                      "\r\n                                          企業ＣＤ," +
                                      "\r\n                                          部署ＣＤ," +
                                      "\r\n                                          コースＣＤ," +
                                      "\r\n                                          得意先ＣＤ," +
                                      "\r\n                                          喫食日 AS 売上日" +
                                      "\r\n                                      FROM" +
                                      "\r\n                                          注文ヘッダ" +
                                      "\r\n                                      WHERE" +
                                      "\r\n                                          企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                          AND 配送日   = @注文日" +
                                      "\r\n                                          AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                          AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                      INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                          T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                          AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                          AND T1.売上日     = T2.売上日" +
                                      "\r\n                                          AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                          AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                  UNION" +
                                      "\r\n                                  SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      売上ヘッダ T1" +
                                      "\r\n                                  WHERE" +
                                      "\r\n                                      T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                      AND T1.売上日   = @注文日" +
                                      "\r\n                                      AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                      AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      AND NOT EXISTS (" +
                                      "\r\n                                          SELECT 'X'" +
                                      "\r\n                                          FROM" +
                                      "\r\n                                              (SELECT" +
                                      "\r\n                                                  企業ＣＤ," +
                                      "\r\n                                                  部署ＣＤ," +
                                      "\r\n                                                  コースＣＤ," +
                                      "\r\n                                                  得意先ＣＤ," +
                                      "\r\n                                                  喫食日 AS 売上日" +
                                      "\r\n                                              FROM" +
                                      "\r\n                                                  注文ヘッダ" +
                                      "\r\n                                              WHERE" +
                                      "\r\n                                                  企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                  AND 配送日   = @注文日" +
                                      "\r\n                                                  AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                  AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                              ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                          WHERE" +
                                      "\r\n                                              T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                              AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                              AND T1.売上日     = T2.売上日" +
                                      "\r\n                                              AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                              AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                  ) AS H" +
                                      "\r\n                                  INNER JOIN 売上明細 AS D" +
                                      "\r\n                                      ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                      AND D.売上日     = H.売上日" +
                                      "\r\n                                      AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                      AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                              ) AS KHD" +
                                      "\r\n                              ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                              AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                              AND D.喫食日 = KHD.売上日" +
                                      "\r\n                              AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                              AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                              AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.弁当区分, 0) = @弁当区分主食" +   // -- 弁当区分主食
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.主食ＣＤ" +
                                      "\r\n                     ) AS PART3" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART3.得意先ＣＤ" +
                                      "\r\n                     ,PART3.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－４ 予測と注文の副食のみ
                                      "\r\n                 SELECT" +
                                      "\r\n                     PART4.得意先ＣＤ" +
                                      "\r\n                     ,PART4.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART4.商品注文数量, PART4.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.副食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                              (SELECT" +
                                      "\r\n                                 H.企業ＣＤ," +
                                      "\r\n                                 H.売上日," +
                                      "\r\n                                 H.得意先ＣＤ," +
                                      "\r\n                                 H.部署ＣＤ," +
                                      "\r\n                                 H.コースＣＤ," +
                                      "\r\n                                 D.商品ＣＤ," +
                                      "\r\n                                 D.明細番号" +
                                      "\r\n                              FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                  (SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      (SELECT" +
                                      "\r\n                                          企業ＣＤ," +
                                      "\r\n                                          部署ＣＤ," +
                                      "\r\n                                          コースＣＤ," +
                                      "\r\n                                          得意先ＣＤ," +
                                      "\r\n                                          喫食日 AS 売上日" +
                                      "\r\n                                      FROM" +
                                      "\r\n                                          注文ヘッダ" +
                                      "\r\n                                      WHERE" +
                                      "\r\n                                          企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                          AND 配送日   = @注文日" +
                                      "\r\n                                          AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                          AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                      INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                          T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                          AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                          AND T1.売上日     = T2.売上日" +
                                      "\r\n                                          AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                          AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                  UNION" +
                                      "\r\n                                  SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      売上ヘッダ T1" +
                                      "\r\n                                  WHERE" +
                                      "\r\n                                      T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                      AND T1.売上日   = @注文日" +
                                      "\r\n                                      AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                      AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      AND NOT EXISTS (" +
                                      "\r\n                                          SELECT 'X'" +
                                      "\r\n                                          FROM" +
                                      "\r\n                                              (SELECT" +
                                      "\r\n                                                  企業ＣＤ," +
                                      "\r\n                                                  部署ＣＤ," +
                                      "\r\n                                                  コースＣＤ," +
                                      "\r\n                                                  得意先ＣＤ," +
                                      "\r\n                                                  喫食日 AS 売上日" +
                                      "\r\n                                              FROM" +
                                      "\r\n                                                  注文ヘッダ" +
                                      "\r\n                                              WHERE" +
                                      "\r\n                                                  企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                  AND 配送日   = @注文日" +
                                      "\r\n                                                  AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                  AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                              ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                          WHERE" +
                                      "\r\n                                              T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                              AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                              AND T1.売上日     = T2.売上日" +
                                      "\r\n                                              AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                              AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                  ) AS H" +
                                      "\r\n                                  INNER JOIN 売上明細 AS D" +
                                      "\r\n                                      ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                      AND D.売上日     = H.売上日" +
                                      "\r\n                                      AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                      AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                              ) AS KHD" +
                                      "\r\n                              ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                              AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                              AND D.喫食日 = KHD.売上日" +
                                      "\r\n                              AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                              AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                              AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.弁当区分, 0) = @弁当区分副食" +   // -- 弁当区分副食
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.副食ＣＤ" +
                                      "\r\n                     ) AS PART4" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART4.得意先ＣＤ" +
                                      "\r\n                     ,PART4.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－５ 予測と注文の主食でも副食でもないもの
                                      "\r\n                 SELECT" +
                                      "\r\n                     PART5.得意先ＣＤ" +
                                      "\r\n                     ,PART5.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART5.商品注文数量, PART5.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.商品ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                              (SELECT" +
                                      "\r\n                                 H.企業ＣＤ," +
                                      "\r\n                                 H.売上日," +
                                      "\r\n                                 H.得意先ＣＤ," +
                                      "\r\n                                 H.部署ＣＤ," +
                                      "\r\n                                 H.コースＣＤ," +
                                      "\r\n                                 D.商品ＣＤ," +
                                      "\r\n                                 D.明細番号" +
                                      "\r\n                              FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                  (SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      (SELECT" +
                                      "\r\n                                          企業ＣＤ," +
                                      "\r\n                                          部署ＣＤ," +
                                      "\r\n                                          コースＣＤ," +
                                      "\r\n                                          得意先ＣＤ," +
                                      "\r\n                                          喫食日 AS 売上日" +
                                      "\r\n                                      FROM" +
                                      "\r\n                                          注文ヘッダ" +
                                      "\r\n                                      WHERE" +
                                      "\r\n                                          企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                          AND 配送日   = @注文日" +
                                      "\r\n                                          AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                          AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                      INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                          T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                          AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                          AND T1.売上日     = T2.売上日" +
                                      "\r\n                                          AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                          AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                  UNION" +
                                      "\r\n                                  SELECT" +
                                      "\r\n                                      T1.企業ＣＤ," +
                                      "\r\n                                      T1.売上日," +
                                      "\r\n                                      T1.得意先ＣＤ," +
                                      "\r\n                                      T1.部署ＣＤ," +
                                      "\r\n                                      T1.コースＣＤ" +
                                      "\r\n                                  FROM" +
                                      "\r\n                                      売上ヘッダ T1" +
                                      "\r\n                                  WHERE" +
                                      "\r\n                                      T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                      AND T1.売上日   = @注文日" +
                                      "\r\n                                      AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                      AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                      AND NOT EXISTS (" +
                                      "\r\n                                          SELECT 'X'" +
                                      "\r\n                                          FROM" +
                                      "\r\n                                              (SELECT" +
                                      "\r\n                                                  企業ＣＤ," +
                                      "\r\n                                                  部署ＣＤ," +
                                      "\r\n                                                  コースＣＤ," +
                                      "\r\n                                                  得意先ＣＤ," +
                                      "\r\n                                                  喫食日 AS 売上日" +
                                      "\r\n                                              FROM" +
                                      "\r\n                                                  注文ヘッダ" +
                                      "\r\n                                              WHERE" +
                                      "\r\n                                                  企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                  AND 配送日   = @注文日" +
                                      "\r\n                                                  AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                  AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                              ) T2 " +   // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                          WHERE" +
                                      "\r\n                                              T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                              AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                              AND T1.売上日     = T2.売上日" +
                                      "\r\n                                              AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                              AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                  ) AS H" +
                                      "\r\n                                  INNER JOIN 売上明細 AS D" +
                                      "\r\n                                      ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                      AND D.売上日     = H.売上日" +
                                      "\r\n                                      AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                      AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                              ) AS KHD" +
                                      "\r\n                              ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                              AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                              AND D.喫食日 = KHD.売上日" +
                                      "\r\n                              AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                              AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                              AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.主食ＣＤ, 0) = 0" +
                                      "\r\n                         AND ISNULL(I.副食ＣＤ, 0) = 0" +
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.商品ＣＤ" +
                                      "\r\n                     ) AS PART5" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART5.得意先ＣＤ" +
                                      "\r\n                     ,PART5.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－６ 予測と注文のその他の主食
                                      "\r\n                 SELECT" +
                                      "\r\n                     PART6.得意先ＣＤ" +
                                      "\r\n                     ,PART6.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART6.商品注文数量, PART6.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.主食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                             (SELECT" +
                                      "\r\n                                H.企業ＣＤ," +
                                      "\r\n                                H.売上日," +
                                      "\r\n                                H.得意先ＣＤ," +
                                      "\r\n                                H.部署ＣＤ," +
                                      "\r\n                                H.コースＣＤ," +
                                      "\r\n                                D.商品ＣＤ," +
                                      "\r\n                                D.明細番号" +
                                      "\r\n                             FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     T1.企業ＣＤ," +
                                      "\r\n                                     T1.売上日," +
                                      "\r\n                                     T1.得意先ＣＤ," +
                                      "\r\n                                     T1.部署ＣＤ," +
                                      "\r\n                                     T1.コースＣＤ" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     (SELECT" +
                                      "\r\n                                         企業ＣＤ," +
                                      "\r\n                                         部署ＣＤ," +
                                      "\r\n                                         コースＣＤ," +
                                      "\r\n                                         得意先ＣＤ," +
                                      "\r\n                                         喫食日 AS 売上日" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         注文ヘッダ" +
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                         AND 配送日   = @注文日" +
                                      "\r\n                                         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                         AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                     ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                 UNION" +
                                      "\r\n                                 SELECT" +
                                      "\r\n                                     T1.企業ＣＤ," +
                                      "\r\n                                     T1.売上日," +
                                      "\r\n                                     T1.得意先ＣＤ," +
                                      "\r\n                                     T1.部署ＣＤ," +
                                      "\r\n                                     T1.コースＣＤ" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     売上ヘッダ T1" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND T1.売上日   = @注文日" +
                                      "\r\n                                     AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                     AND NOT EXISTS (" +
                                      "\r\n                                         SELECT 'X'" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             (SELECT" +
                                      "\r\n                                                 企業ＣＤ," +
                                      "\r\n                                                 部署ＣＤ," +
                                      "\r\n                                                 コースＣＤ," +
                                      "\r\n                                                 得意先ＣＤ," +
                                      "\r\n                                                 喫食日 AS 売上日" +
                                      "\r\n                                             FROM" +
                                      "\r\n                                                 注文ヘッダ" +
                                      "\r\n                                             WHERE" +
                                      "\r\n                                                 企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                 AND 配送日   = @注文日" +
                                      "\r\n                                                 AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                 AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                             ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                             AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                 ) AS H" +
                                      "\r\n                                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                     AND D.売上日     = H.売上日" +
                                      "\r\n                                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                             ) AS KHD" +
                                      "\r\n                             ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                             AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                             AND D.喫食日 = KHD.売上日" +
                                      "\r\n                             AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                             AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) " + // -- チケットとその他
                                      "\r\n                         AND ISNULL(I.主食ＣＤ, 0) <> 0" +               // -- 主食がある
                                      "\r\n                         AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.主食ＣＤ" +
                                      "\r\n                     ) AS PART6" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART6.得意先ＣＤ" +
                                      "\r\n                     ,PART6.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－７ 予測と注文のその他の副食
                                      "\r\n                 SELECT" +
                                      "\r\n                     PART7.得意先ＣＤ" +
                                      "\r\n                     ,PART7.部品ＣＤ" +
                                      "\r\n                     ,SUM(ISNULL(PART7.商品注文数量, PART7.商品予測数量)) AS 商品注文予測数量" +
                                      "\r\n                     ,null AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      "\r\n                     (SELECT" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.副食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 0 THEN D.商品数量 ELSE null END) AS 商品注文数量" +
                                      "\r\n                         ,SUM(CASE WHEN D.注文区分 = 1 THEN D.商品数量 ELSE null END) AS 商品予測数量" +
                                      "\r\n                     FROM" +
                                      "\r\n                         注文ヘッダ AS H" +
                                      "\r\n                         INNER JOIN 注文明細 AS D" +
                                      "\r\n                             ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                             AND D.注文区分   = H.注文区分" +
                                      "\r\n                             AND D.喫食日     = H.喫食日" +
                                      "\r\n                             AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      // -- 売上には存在しないこと（売上に存在する注文は取得しない）
                                      // -- 喫食日売上ヘッダと売上明細を内部結合して、さらに注文明細と外部結合して、売上明細がISNULLであること
                                      "\r\n                         LEFT JOIN" +
                                      "\r\n                             (SELECT" +
                                      "\r\n                                H.企業ＣＤ," +
                                      "\r\n                                H.売上日," +
                                      "\r\n                                H.得意先ＣＤ," +
                                      "\r\n                                H.部署ＣＤ," +
                                      "\r\n                                H.コースＣＤ," +
                                      "\r\n                                D.商品ＣＤ," +
                                      "\r\n                                D.明細番号" +
                                      "\r\n                             FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                                 (SELECT" +
                                      "\r\n                                     T1.企業ＣＤ," +
                                      "\r\n                                     T1.売上日," +
                                      "\r\n                                     T1.得意先ＣＤ," +
                                      "\r\n                                     T1.部署ＣＤ," +
                                      "\r\n                                     T1.コースＣＤ" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     (SELECT" +
                                      "\r\n                                         企業ＣＤ," +
                                      "\r\n                                         部署ＣＤ," +
                                      "\r\n                                         コースＣＤ," +
                                      "\r\n                                         得意先ＣＤ," +
                                      "\r\n                                         喫食日 AS 売上日" +
                                      "\r\n                                     FROM" +
                                      "\r\n                                         注文ヘッダ" +
                                      "\r\n                                     WHERE" +
                                      "\r\n                                         企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                         AND 配送日   = @注文日" +
                                      "\r\n                                         AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                         AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                     ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                     INNER JOIN 売上ヘッダ T1 ON" +
                                      "\r\n                                         T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                         AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                         AND T1.売上日     = T2.売上日" +
                                      "\r\n                                         AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                         AND T1.コースＣＤ = T2.コースＣＤ" +
                                      "\r\n                                 UNION" +
                                      "\r\n                                 SELECT" +
                                      "\r\n                                     T1.企業ＣＤ," +
                                      "\r\n                                     T1.売上日," +
                                      "\r\n                                     T1.得意先ＣＤ," +
                                      "\r\n                                     T1.部署ＣＤ," +
                                      "\r\n                                     T1.コースＣＤ" +
                                      "\r\n                                 FROM" +
                                      "\r\n                                     売上ヘッダ T1" +
                                      "\r\n                                 WHERE" +
                                      "\r\n                                     T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                     AND T1.売上日   = @注文日" +
                                      "\r\n                                     AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND T1.コースＣＤ = @コースＣＤ" +
                                      "\r\n                                     AND NOT EXISTS (" +
                                      "\r\n                                         SELECT 'X'" +
                                      "\r\n                                         FROM" +
                                      "\r\n                                             (SELECT" +
                                      "\r\n                                                 企業ＣＤ," +
                                      "\r\n                                                 部署ＣＤ," +
                                      "\r\n                                                 コースＣＤ," +
                                      "\r\n                                                 得意先ＣＤ," +
                                      "\r\n                                                 喫食日 AS 売上日" +
                                      "\r\n                                             FROM" +
                                      "\r\n                                                 注文ヘッダ" +
                                      "\r\n                                             WHERE" +
                                      "\r\n                                                 企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                                                 AND 配送日   = @注文日" +
                                      "\r\n                                                 AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                                 AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                             ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                                         WHERE" +
                                      "\r\n                                             T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                             AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                             AND T1.売上日     = T2.売上日" +
                                      "\r\n                                             AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                             AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                                 ) AS H" +
                                      "\r\n                                 INNER JOIN 売上明細 AS D" +
                                      "\r\n                                     ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                                     AND D.売上日     = H.売上日" +
                                      "\r\n                                     AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                                     AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                             ) AS KHD" +
                                      "\r\n                             ON H.企業ＣＤ = KHD.企業ＣＤ" +
                                      "\r\n                             AND H.コースＣＤ = KHD.コースＣＤ" +
                                      "\r\n                             AND D.喫食日 = KHD.売上日" +
                                      "\r\n                             AND D.得意先ＣＤ = KHD.得意先ＣＤ" +
                                      "\r\n                             AND D.部署ＣＤ = KHD.部署ＣＤ" +
                                      "\r\n                             AND D.商品ＣＤ = KHD.商品ＣＤ" +
                                      "\r\n                         INNER JOIN 商品マスタ AS I" +
                                      "\r\n                             ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                             AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                             AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         H.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n                         AND H.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND H.配送日 = @注文日" +
                                      "\r\n                         AND H.コースＣＤ = @コースＣＤ" +
                                      "\r\n                         AND ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) " + // -- チケットとその他
                                      "\r\n                         AND ISNULL(I.副食ＣＤ, 0) <> 0" +               // -- 副食がある
                                      "\r\n                         AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ" +      // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                         AND KHD.明細番号 IS NULL" +
                                      "\r\n                     GROUP BY" +
                                      "\r\n                         H.得意先ＣＤ" +
                                      "\r\n                         ,I.副食ＣＤ" +
                                      "\r\n                     ) AS PART7" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     PART7.得意先ＣＤ" +
                                      "\r\n                     ,PART7.部品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－８ 売上の弁当の主食
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.主食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                     AND ISNULL(D.主食ＣＤ, 0) <> 0" +           // -- 主食がある
                                      "\r\n                     AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ" +  // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.主食ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－９ 売上の弁当の副食
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.副食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(I.弁当区分, 0) = @弁当区分弁当" +    // -- 弁当
                                      "\r\n                     AND ISNULL(D.副食ＣＤ, 0) <> 0" +           // -- 副食がある
                                      "\r\n                     AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ" +  // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.副食ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－１０ 売上の主食のみ
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.主食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(I.弁当区分, 0) = @弁当区分主食" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.主食ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－１１ 売上の副食のみ
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.副食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(I.弁当区分, 0) = @弁当区分副食" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.副食ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－１２ 売上の主食でも副食でもないもの
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.商品ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2" +     // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(D.主食ＣＤ, 0) = 0" +
                                      "\r\n                     AND ISNULL(D.副食ＣＤ, 0) = 0" +
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.商品ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－１３ 売上のその他の主食
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.主食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) " + // -- チケットとその他
                                      "\r\n                     AND ISNULL(D.主食ＣＤ, 0) <> 0" +           // -- 主食がある
                                      "\r\n                     AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ" +  // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.主食ＣＤ" +
                                      "\r\n                 UNION ALL" +
                                      // -- ②－１４ 売上のその他の副食
                                      "\r\n                 SELECT" +
                                      "\r\n                     H.得意先ＣＤ" +
                                      "\r\n                     ,D.副食ＣＤ AS 部品ＣＤ" +
                                      "\r\n                     ,null AS 商品注文予測数量" +
                                      "\r\n                     ,SUM(D.商品数量) AS 商品売上数量" +
                                      "\r\n                 FROM" +
                                      // -- 喫食日売上ヘッダ
                                      // -- 指定した配送日の注文ヘッダ.喫食日＝売上ヘッダ.売上日の売上
                                      // -- ※上記が存在しない場合は売上ヘッダ.売上日＝指定日の売上
                                      "\r\n                     (SELECT" +
                                      "\r\n                         T1.企業ＣＤ," +
                                      "\r\n                         T1.売上日," +
                                      "\r\n                         T1.得意先ＣＤ," +
                                      "\r\n                         T1.部署ＣＤ" +
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
                                      "\r\n                             AND 配送日   = @注文日" +
                                      "\r\n                             AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                             AND コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                         T1.部署ＣＤ" +
                                      "\r\n                     FROM" +
                                      "\r\n                         売上ヘッダ T1" +
                                      "\r\n                     WHERE" +
                                      "\r\n                         T1.企業ＣＤ     = @企業ＣＤ" +
                                      "\r\n                         AND T1.売上日   = @注文日" +
                                      "\r\n                         AND T1.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                         AND T1.コースＣＤ = @コースＣＤ" +
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
                                      "\r\n                                     AND 配送日   = @注文日" +
                                      "\r\n                                     AND 部署ＣＤ = @部署ＣＤ" +
                                      "\r\n                                     AND コースＣＤ = @コースＣＤ" +
                                      "\r\n                                 ) T2 " +    // -- 注文ヘッダ（配送日から喫食日を売上とした注文ヘッダ）
                                      "\r\n                             WHERE" +
                                      "\r\n                                 T1.企業ＣＤ       = T2.企業ＣＤ" +
                                      "\r\n                                 AND T1.得意先ＣＤ = T2.得意先ＣＤ" +
                                      "\r\n                                 AND T1.売上日     = T2.売上日" +
                                      "\r\n                                 AND T1.部署ＣＤ   = T2.部署ＣＤ" +
                                      "\r\n                                 AND T1.コースＣＤ = T2.コースＣＤ)" +
                                      "\r\n                     ) AS H" +
                                      "\r\n                     INNER JOIN 売上明細 AS D" +
                                      "\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ" +
                                      "\r\n                         AND D.売上日     = H.売上日" +
                                      "\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ" +
                                      "\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ" +
                                      "\r\n                     INNER JOIN 商品マスタ AS I" +
                                      "\r\n                         ON I.企業ＣＤ = D.企業ＣＤ" +
                                      "\r\n                         AND ISNULL(I.無効フラグ, 0) = 0" +
                                      "\r\n                         AND I.商品ＣＤ = D.商品ＣＤ" +
                                      "\r\n                 WHERE" +
                                      "\r\n                     ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他) " + // -- チケットとその他
                                      "\r\n                     AND ISNULL(D.副食ＣＤ, 0) <> 0" +           // -- 副食がある
                                      "\r\n                     AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ" +  // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                                      "\r\n                 GROUP BY" +
                                      "\r\n                     H.得意先ＣＤ," +
                                      "\r\n                     D.副食ＣＤ" +
                                      "\r\n                 ) AS PART" +
                                      "\r\n             GROUP BY" +
                                      "\r\n                 PART.得意先ＣＤ," +
                                      "\r\n                 PART.部品ＣＤ" +
                                      "\r\n             ) AS PARTSUM" +
                                      "\r\n         GROUP BY" +
                                      "\r\n             PARTSUM.部品ＣＤ" +
                                      "\r\n         ) AS INIT ON MAIN.商品ＣＤ = INIT.部品ＣＤ" +
                                      "\r\n     ) AS B" +
                                      "\r\n     ON A.行番号 = B.行番号" +
                                      "\r\n ORDER BY" +
                                      "\r\n     行番号" +
                                      "\r\n     ,商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース部品状況リストを検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("コース部品状況");
                // 商品名列を作成
                cldt.AddColumns(buhinDt, "商品名略称", "商品ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["見出し"]);
                // インポート
                cldt.Import(dt, "商品ＣＤ", "数量");

                // 差の行を追加する
                cldt.Rows.Add(cldt.NewRow());
                cldt.Rows[BUHIN_SA_ROW]["見出し"] = "差";

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                buhinBindingSource.DataSource = ds;
                buhinBindingSource.DataMember = "コース部品状況";

                fpSpread_Buhin_Sheet1.BindDataColumn(BUHIN_TITLE_COLUMN, "見出し");

                // 商品名を列名としてバインド
                int columnIndex = BUHIN_SYOHIN_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread_Buhin_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                int maxColumnIndex = columnIndex - 1;

                // 先頭列を固定列にする
                fpSpread_Buhin_Sheet1.FrozenColumnCount = 1;

                // 差の行に数式を設定する
                for (columnIndex = BUHIN_SYOHIN_START_COLUMN; columnIndex <= maxColumnIndex; columnIndex++)
                {
                    FarPoint.Win.Spread.Cell topCell = fpSpread_Buhin_Sheet1.Cells[BUHIN_MOCHIDASHI_ROW, columnIndex];
                    FarPoint.Win.Spread.Cell middleCell = fpSpread_Buhin_Sheet1.Cells[BUHIN_IDO_ROW, columnIndex];
                    FarPoint.Win.Spread.Cell bottomCell = fpSpread_Buhin_Sheet1.Cells[BUHIN_JUCHU_ROW, columnIndex];
                    FarPoint.Win.Spread.Cell diffCell = fpSpread_Buhin_Sheet1.Cells[BUHIN_SA_ROW, columnIndex];
                    diffCell.Formula = string.Format("{0}+{1}-{2}", topCell.ToString(), middleCell.ToString(), bottomCell.ToString());

                    // 差がマイナスのセルを着色
                    fpSpread_Buhin_Sheet1.Cells[BUHIN_SA_ROW, columnIndex].ResetBackColor();
                    if ((int)(fpSpread_Buhin_Sheet1.Cells[BUHIN_SA_ROW, columnIndex].Value) < 0)
                    {
                        fpSpread_Buhin_Sheet1.Cells[BUHIN_SA_ROW, columnIndex].BackColor = MINUS_BACK_COLOR;
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
        /// 部品スプレッドの列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetBuhinTable()
        {
            DataTable dt = new DataTable("部品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // -- 以下のように並べる
                    // -- ┌───┬───┬───┐
                    // -- │商品Ａ│商品Ｂ│商品Ｃ│
                    // -- └───┴───┴───┘
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.商品ＣＤ" +
                                      "\r\n     ,S.商品名略称" +
                                      "\r\n FROM" +
                                      "\r\n     主要部品リストマスタ AS A" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = @企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND A.部署ＣＤ = @部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     商品ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("部品一覧を取得しました。");
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
        /// 部品名一覧のデータテーブルを基にして、部品スプレッドに列を追加する
        /// </summary>
        /// <param name="syohinDt">部品名一覧データテーブル</param>
        /// <param name="labelColumnName">部品名が格納されている列の名前</param>
        private void SetBuhinSpreadColumn(DataTable buhinDt, string labelColumnName)
        {
            try
            {
                // 部品名列を削除（リセット）
                fpSpread_Buhin_Sheet1.ColumnCount = BUHIN_SYOHIN_START_COLUMN;

                // 今回追加する部品名の列数をセット
                fpSpread_Buhin_Sheet1.ColumnCount = BUHIN_SYOHIN_START_COLUMN + buhinDt.Rows.Count;

                // 固定列に続いて、商品名の列の属性をセット
                int columnIndex = BUHIN_SYOHIN_START_COLUMN;
                foreach (DataRow dr in buhinDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = 0D;
                    numberCellType.ReadOnly = true;
                    numberCellType.Static = true;
                    fpSpread_Buhin_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread_Buhin_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread_Buhin_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread_Buhin_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread_Buhin_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
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
        /// 食券枚数計を表示
        /// </summary>
        private void GetTicket()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     ISNULL(SUM(UK.商品数量), 0) AS 食券枚数計");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    // -- ①－１（指定されたコースにぶら下がる得意先）
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         CT.企業ＣＤ");
                    sql.AppendLine("         ,CT.コースＣＤ");
                    sql.AppendLine("         ,CT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コーステーブル AS CT");
                    sql.AppendLine("         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 コースＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 臨時コースマスタ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                 AND 実施日 = @注文日");
                    sql.AppendLine("             ) AS RM");
                    sql.AppendLine("             ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         RT.企業ＣＤ");
                    sql.AppendLine("         ,RT.コースＣＤ");
                    sql.AppendLine("         ,RT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         臨時コーステーブル AS RT");
                    sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RT.実施日 = @注文日");
                    // -- ①－２（指定されたコースを持つ注文の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         注文ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    // -- ①－３（指定されたコースを持つ売上の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 売上日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     ) AS C");
                    // -- ④注文計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 注文計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ");
                    // -- ⑤食券枚数計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品数量) AS 商品数量");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.売上日 = @注文日");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.売上区分 in (@売上区分チケット, @売上区分バークレー, @売上区分ボーナス)");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");
                    sql.AppendLine((string)cboChumon.SelectedValue);

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                    cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                    cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    lblTicket.Text = "";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        lblTicket.Text = string.Format("{0:N0}", reader["食券枚数計"]);
                    }

                    log.Info("食券枚数計を検索しました。");
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
        /// 未収金券計を表示
        /// </summary>
        private void GetMisyuTicket()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     ISNULL(SUM(UK.商品数量), 0) AS 未収金券計");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    // -- ①－１（指定されたコースにぶら下がる得意先）
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         CT.企業ＣＤ");
                    sql.AppendLine("         ,CT.コースＣＤ");
                    sql.AppendLine("         ,CT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コーステーブル AS CT");
                    sql.AppendLine("         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 コースＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 臨時コースマスタ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                 AND 実施日 = @注文日");
                    sql.AppendLine("             ) AS RM");
                    sql.AppendLine("             ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         RT.企業ＣＤ");
                    sql.AppendLine("         ,RT.コースＣＤ");
                    sql.AppendLine("         ,RT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         臨時コーステーブル AS RT");
                    sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RT.実施日 = @注文日");
                    // -- ①－２（指定されたコースを持つ注文の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         注文ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    // -- ①－３（指定されたコースを持つ売上の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 売上日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     ) AS C");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    // -- ④注文計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 注文計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ");
                    // -- ⑤未収金券計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品数量) AS 商品数量");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.売上日 = @注文日");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.売上区分 = @売上区分売掛");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(T.支払方法, 0) in (@支払方法チケット, @支払方法金券)");
                    sql.AppendLine((string)cboChumon.SelectedValue);

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                    cmd.Parameters.Add("@支払方法チケット", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Ticket;
                    cmd.Parameters.Add("@支払方法金券", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Kinken;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    lblMisyuTicket.Text = "";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        lblMisyuTicket.Text = string.Format("{0:N0}", reader["未収金券計"]);
                    }

                    log.Info("未収金券計を検索しました。");
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
        /// 現金合計を表示
        /// </summary>
        private void GetGenkin()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     ISNULL(SUM(UK.現金計), 0) AS 現金合計");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    // -- ①－１（指定されたコースにぶら下がる得意先）
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         CT.企業ＣＤ");
                    sql.AppendLine("         ,CT.コースＣＤ");
                    sql.AppendLine("         ,CT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コーステーブル AS CT");
                    sql.AppendLine("         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 コースＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 臨時コースマスタ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                 AND 実施日 = @注文日");
                    sql.AppendLine("             ) AS RM");
                    sql.AppendLine("             ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         RT.企業ＣＤ");
                    sql.AppendLine("         ,RT.コースＣＤ");
                    sql.AppendLine("         ,RT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         臨時コーステーブル AS RT");
                    sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RT.実施日 = @注文日");
                    // -- ①－２（指定されたコースを持つ注文の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         注文ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    // -- ①－３（指定されたコースを持つ売上の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 売上日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     ) AS C");
                    // -- ④注文計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 注文計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ");
                    // -- ⑤現金計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 現金計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.売上日 = @注文日");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.売上区分 = @売上区分現金");
                    // 2016/08/22 DEL S チケット束売りも含める
                    //sql.AppendLine("             AND ISNULL(M.チケット束フラグ, 0) = 0");   // -- チケット束売りは除く
                    // 2016/08/22 DEL E
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     1 = 1");
                    sql.AppendLine((string)cboChumon.SelectedValue);

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@売上区分現金", SqlDbType.Int).Value = (int)Consts.UriageKubun.Genkin;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    lblGenkin.Text = "";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        lblGenkin.Text = string.Format("{0:C0}", reader["現金合計"]);
                    }

                    log.Info("現金合計を検索しました。");
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
        /// 未収金計を表示
        /// </summary>
        private void GetMisyukin()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     ISNULL(SUM(UK.商品金額合計税込), 0) AS 未収金計");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    // -- ①－１（指定されたコースにぶら下がる得意先）
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         CT.企業ＣＤ");
                    sql.AppendLine("         ,CT.コースＣＤ");
                    sql.AppendLine("         ,CT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コーステーブル AS CT");
                    sql.AppendLine("         INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 コースＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 臨時コースマスタ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                 AND 実施日 = @注文日");
                    sql.AppendLine("             ) AS RM");
                    sql.AppendLine("             ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         RT.企業ＣＤ");
                    sql.AppendLine("         ,RT.コースＣＤ");
                    sql.AppendLine("         ,RT.得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         臨時コーステーブル AS RT");
                    sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("         AND RT.実施日 = @注文日");
                    // -- ①－２（指定されたコースを持つ注文の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         注文ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    // -- ①－３（指定されたコースを持つ売上の得意先）
                    sql.AppendLine("     UNION");
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,得意先ＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("         AND 売上日 = @注文日");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     ) AS C");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    // -- ④注文計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 注文計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             注文ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ");
                    // -- ⑤未収金計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 商品金額合計税込");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日     = M.売上日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.売上日 = @注文日");
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.売上区分 = @売上区分売掛");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,M.得意先ＣＤ");
                    sql.AppendLine("         ) AS UK ON UK.得意先ＣＤ = C.得意先ＣＤ AND UK.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(T.売掛現金区分, 0) = @売掛現金区分現金");
                    sql.AppendLine((string)cboChumon.SelectedValue);

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                    cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    lblMisyukin.Text = "";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        lblMisyukin.Text = string.Format("{0:C0}", reader["未収金計"]);
                    }

                    log.Info("未収金計を検索しました。");
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
        /// 選択行に該当する注文入力検索条件を返します
        /// </summary>
        /// <returns>注文入力検索条件</returns>
        private ChumonMeisaiCriteria GetChumonMeisaiCriteria()
        {
            try
            {
                ChumonMeisaiCriteria chumonCriteria = null;
                _selectedChild = null;

                // 親レコードが選択？
                if (fpSpread_Chumon_Sheet1.Rows.Count > 0 && fpSpread_Chumon_Sheet1.SelectionCount != 0)
                {
                    // 合計行ではない場合
                    if (fpSpread_Chumon_Sheet1.Rows[fpSpread_Chumon_Sheet1.ActiveRowIndex].BackColor != TOTAL_BACK_COLOR)
                    {
                        DataRow dr = ((DataRowView)chumonBindingSource.Current).Row;
                        chumonCriteria = new ChumonMeisaiCriteria();
                        chumonCriteria.得意先ＣＤ = dr["得意先ＣＤ"] as int?;
                        chumonCriteria.得意先名 = dr["得意先名"] as string;
                    }
                }
                else
                {
                    // 子レコードが選択？
                    int index = 0;
                    foreach (FarPoint.Win.Spread.SheetView childView in fpSpread_Chumon_Sheet1.GetChildSheets())
                    {
                        if (childView.SelectionCount != 0)
                        {
                            // データバインドしていないので、セルの値を直接参照する
                            chumonCriteria = new ChumonMeisaiCriteria();
                            chumonCriteria.得意先ＣＤ = childView.Cells[childView.ActiveRowIndex, CHUMON_TOKUISAKI_CD_COLUMN].Value as int?;
                            chumonCriteria.得意先名 = childView.Cells[childView.ActiveRowIndex, CHUMON_TOKUISAKI_NAME_COLUMN].Value as string;

                            // 選択された子ビューの位置情報を記憶(☆)
                            _selectedChild = new SelectedChild();
                            _selectedChild.ViewIndex = index;
                            _selectedChild.RowIndex = childView.ActiveRowIndex;
                            _selectedChild.ColumnIndex = childView.ActiveColumnIndex;
                            _selectedChild.ParentRowIndex = childView.ParentRowIndex;
                            break;
                        }
                        index++;
                    }
                }

                if (chumonCriteria != null)
                {
                    chumonCriteria.stopOverwrite = true;
                    chumonCriteria.部署ＣＤ = _courseChumonCriteria.部署ＣＤ;
                    chumonCriteria.注文日 = _courseChumonCriteria.注文日;
                    chumonCriteria.コースＣＤ = _courseChumonCriteria.コースＣＤ;       // デフォルト表示するコース
                }
                return chumonCriteria;
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
        /// 詳細表示（既存レコード）
        /// </summary>
        /// </summary>
        /// <param name="chumonCriteria">注文入力検索条件</param>
        private void ShowDetail(ChumonMeisaiCriteria chumonCriteria)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _courseChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

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
                // 2016/08/22 REP S モーダルにせず、複数開けるようにする
                //SubFormOpenInit(formChumon);
                formChumon.MdiParent = this.MdiParent;
                // 2016/08/22 REP E
                formChumon.ListOpened = true;
                formChumon.Show();
                formChumon.FormClosed += new FormClosedEventHandler(Form200_FormClosed);
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
                Form form = (Form)sender;
                form.Dispose();
                form = null;

                // 一覧画面を閉じた後に詳細画面で閉じられた場合、一覧画面では何もしない
                if (this.IsDisposed)
                {
                    return;
                }

                this.Enabled = true;
                this.Activate();

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread_Chumon.GetViewportTopRow(0);
                int activeRowIndex = fpSpread_Chumon_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread_Chumon_Sheet1.ActiveColumnIndex;

                // 子ビューの選択行のインデックスを記憶
                int childActiveRowIndex = -1;
                int childActiveColumnIndex = -1;
                // 注文入力画面を表示したときに記憶していた位置情報から取得する(☆)
                if (_selectedChild != null)
                {
                    childActiveRowIndex = _selectedChild.RowIndex;
                    childActiveColumnIndex = _selectedChild.ColumnIndex;
                    activeRowIndex = _selectedChild.ParentRowIndex;      // 親の行インデックスを取得
                    activeColumnIndex = childActiveColumnIndex;
                }
                // ホントは↓で実現したいが、セルをダブルクリックしたときに限り childView.SelectionCount が 0 になっている
                // foreach (FarPoint.Win.Spread.SheetView childView in fpSpread_Chumon_Sheet1.GetChildSheets())
                // {
                //     if (childView.SelectionCount != 0)
                //     {
                //         childActiveRowIndex = childView.ActiveRowIndex;
                //         childActiveColumnIndex = childView.ActiveColumnIndex;
                //         activeRowIndex = childView.ParentRowIndex;      // 親の行インデックスを取得
                //         activeColumnIndex = childActiveColumnIndex;
                //     }
                // }

                btnQuery.PerformClick();        // 再表示

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread_Chumon.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread_Chumon_Sheet1.Rows.Count - fpSpread_Chumon_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread_Chumon_Sheet1.Rows.Count - fpSpread_Chumon_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread_Chumon_Sheet1.ActiveRowIndex = activeRowIndex;
                    chumonBindingSource.Position = activeRowIndex;      // Currentプロパティがずれる場合があるのを回避
                }
                fpSpread_Chumon_Sheet1.ActiveColumnIndex = activeColumnIndex;

                // 子ビューが選択されていた？
                if (childActiveRowIndex >= 0)
                {
                    fpSpread_Chumon_Sheet1.ExpandRow(activeRowIndex, true);     // 子ビューを展開

                    // 親ワークブック上のアクティブ行インデックスに関連付けられている子ワークブックを取得
                    FarPoint.Win.Spread.SpreadView childView = fpSpread_Chumon.GetRootWorkbook().GetSpreadView(fpSpread_Chumon_Sheet1.GetChildView(activeRowIndex, 0), 0, 0);

                    // 子ワークブックをアクティブにする
                    fpSpread_Chumon.GetRootWorkbook().SetActiveWorkbook(childView);

                    // 子ワークブック上のセルをアクティブにする
                    fpSpread_Chumon.GetRootWorkbook().Sheets[0].ClearSelection();
                    childView.Sheets[0].SetActiveCell(childActiveRowIndex, childActiveColumnIndex);
                    childView.ShowActiveCell(FarPoint.Win.Spread.VerticalPosition.Nearest, FarPoint.Win.Spread.HorizontalPosition.Nearest);
                    fpSpread_Chumon.GetRootWorkbook().Invalidate();
                    childView.Invalidate();
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
        /// 移動入力詳細表示
        /// </summary>
        private void ShowIdo()
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_courseChumonCriteria.HasErrors)
                {
                    MessageBox.Show(this, _courseChumonCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索実行時の条件が変わっている？
                if (_courseChumonCriteria.Equals(_courseChumonCriteriaSnapShot) == false)
                {
                    MessageBox.Show(this, "検索条件が変わっています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                IdoNyuryokuDetailCriteria idoNyuryokuDetailCriteria = new IdoNyuryokuDetailCriteria();
                idoNyuryokuDetailCriteria.配送日 = _courseChumonCriteria.注文日;
                idoNyuryokuDetailCriteria.コースＣＤ = _courseChumonCriteria.コースＣＤ;
                idoNyuryokuDetailCriteria.コース区分 = _courseChumonCriteria.コース区分;
                idoNyuryokuDetailCriteria.支店ＣＤ = _courseChumonCriteria.支店ＣＤ;
                idoNyuryokuDetailCriteria.部署ＣＤ = _courseChumonCriteria.部署ＣＤ;
                idoNyuryokuDetailCriteria.コース名 = _courseChumonCriteria.コース名;
                idoNyuryokuDetailCriteria.コース区分名 = _courseChumonCriteria.コース区分名;
                idoNyuryokuDetailCriteria.支店名 = _courseChumonCriteria.支店名;
                idoNyuryokuDetailCriteria.部署名 = _courseChumonCriteria.部署名;
                Form301D form = new Form301D(idoNyuryokuDetailCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form200_FormClosed);      // 注文入力画面が閉じたときと同じ処理
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
        /// 画面クリア
        /// </summary>
        private void Clear()
        {
            try
            {
                this.Text = "コース注文入力";

                // 各スプレッドをクリア
                chumonBindingSource.DataSource = null;
                buhinBindingSource.DataSource = null;

                lblTicket.Text = "";            // 食券枚数計をクリア
                lblMisyuTicket.Text = "";       // 未収金券計をクリア
                lblGenkin.Text = "";            // 現金合計をクリア
                lblMisyukin.Text = "";          // 未収金計をクリア
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
        /// 注文から売上を生成する（既存の売上はスキップする）
        /// </summary>
        private void CreateUriage()
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                LineProxy lineProxy = new LineProxy();                  // 配達完了受入API（呼び出し判定用）
                bool canApiCall = lineProxy.CanCall();
                List<int> apiTargetTokuisakiCDList = new List<int>();   // 配達完了受入API用得意先CDリスト

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    // トランザクション開始
                    con.Open();
                    SqlTransaction tran = con.BeginTransaction();

                    DataSet ds = (DataSet)chumonBindingSource.DataSource;
                    DataTable dt = new DataTable();

                    // システム設定の「コース注文入力で階層表示する」に応じてDataTableの取得元を切り替える
                    if (_isHierarchicalView)
                    {
                        dt = ds.Tables["親"];
                    }
                    else
                    {
                        dt = ds.Tables["コース注文入力"];
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        // 整数以外のものが得意先ＣＤに含まれている場合はcontinue
                        //if (!int.TryParse(dr["得意先ＣＤ"].ToString(),out int res))    // VS2015でエラーになるので書換
                        int res;
                        if (!int.TryParse(dr["得意先ＣＤ"].ToString(), out res))
                        {
                            continue;
                        }

                        int tokuisakiCD = (int)dr["得意先ＣＤ"];
                        // 無効な得意先CDの場合はNOP
                        if (tokuisakiCD < 0)
                        {
                            continue;
                        }

                        // 売上が存在すればスキップ
                        UriageHeader uriageHeader = UriageHeader.GetUriageHeader(tokuisakiCD, (DateTime)_courseChumonCriteria.注文日, (int)_courseChumonCriteria.部署ＣＤ);
                        if (uriageHeader != null)
                        {
                            continue;
                        }

                        // 売上ヘッダを作成
                        if (CreateUriageHeader(con, tran, tokuisakiCD) == false)
                        {
                            return;
                        }

                        // 売上明細を作成
                        if (CreateUriageMeisai(con, tran, tokuisakiCD) == false)
                        {
                            return;
                        }

                        History.Save(con, tran, this.Text, "注文から売上", string.Format("得意先ＣＤ=[{0}] 売上日=[{1:yyyy/MM/dd}]", tokuisakiCD, _courseChumonCriteria.注文日), historyProcess);

                        // 配達完了受入APIを呼び出す必要がある場合は、LINEによる注文があればその得意先を対象にする
                        if (canApiCall)
                        {
                            if (HasOrderFromLine(con, tran, tokuisakiCD))
                            {
                                apiTargetTokuisakiCDList.Add(tokuisakiCD);
                            }
                        }
                    }

                    // コミット
                    tran.Commit();
                    log.InfoFormat("売上ヘッダ・売上明細を{0}しました。", historyProcess.DisplayName());
                }

                // LINE決済用中間サーバの配達完了受入APIを非同期で呼ぶ
                if (canApiCall)
                {
                    CallSetArrived(apiTargetTokuisakiCDList);
                }

                MessageBox.Show(this, "売上に反映しました。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 売上ヘッダを作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUriageHeader(SqlConnection con, SqlTransaction tran, int tokuisakiCD)
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // 注文ヘッダから売上ヘッダを作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO 売上ヘッダ(" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,売上日" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,得意先名" +
                                  "\r\n     ,コースＣＤ" +
                                  "\r\n     ,配送担当者ＣＤ" +
                                  "\r\n     ,税区分" +
                                  "\r\n     ,丸め処理区分" +
                                  "\r\n     ,納品書伝票番号" +
                                  "\r\n     ,作成担当者ＣＤ" +
                                  "\r\n     ,作成日" +
                                  "\r\n     ,修正担当者ＣＤ" +
                                  "\r\n     ,修正日" +
                                  "\r\n ) SELECT" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,喫食日" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,得意先名" +
                                  "\r\n     ,コースＣＤ" +
                                  "\r\n     ,担当者ＣＤ" +
                                  "\r\n     ,税区分" +
                                  "\r\n     ,丸め処理区分" +
                                  "\r\n     ,納品書伝票番号" +
                                  "\r\n     ,@作成担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n     ,@修正担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n FROM" +
                                  "\r\n     注文ヘッダ" +
                                  "\r\n WHERE " +
                                  "\r\n     企業ＣＤ   = @企業ＣＤ" +
                                  "\r\n AND 注文区分   = @注文区分" +
                                  "\r\n AND 喫食日     = @喫食日" +
                                  "\r\n AND 得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n AND 部署ＣＤ   = @部署ＣＤ" +
                                  "\r\n AND コースＣＤ = @コースＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上ヘッダが重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上ヘッダの{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 売上明細を作成
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <returns>成功した場合は True</returns>
        private bool CreateUriageMeisai(SqlConnection con, SqlTransaction tran, int tokuisakiCD)
        {
            History.ProcessType historyProcess = History.ProcessType.Create;

            try
            {
                // 注文明細から売上明細を作成
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;
                cmd.CommandText = "\r\n INSERT INTO 売上明細(" +
                                  "\r\n     企業ＣＤ" +
                                  "\r\n     ,売上日" +
                                  "\r\n     ,得意先ＣＤ" +
                                  "\r\n     ,部署ＣＤ" +
                                  "\r\n     ,明細番号" +
                                  "\r\n     ,商品ＣＤ" +
                                  "\r\n     ,商品グループＣＤ" +
                                  "\r\n     ,売上区分" +
                                  "\r\n     ,商品単価" +
                                  "\r\n     ,商品数量" +
                                  "\r\n     ,値引額" +
                                  "\r\n     ,値引事由区分" +
                                  "\r\n     ,値引前金額" +
                                  "\r\n     ,値引後金額" +
                                  "\r\n     ,消費税額" +
                                  "\r\n     ,商品金額合計税込" +
                                  "\r\n     ,計算税率" +
                                  "\r\n     ,チケット束フラグ" +
                                  "\r\n     ,現金補填フラグ" +
                                  "\r\n     ,主食ＣＤ" +
                                  "\r\n     ,副食ＣＤ" +
                                  "\r\n     ,食事区分" +
                                  "\r\n     ,分配元数量" +
                                  "\r\n     ,作成担当者ＣＤ" +
                                  "\r\n     ,作成日" +
                                  "\r\n     ,修正担当者ＣＤ" +
                                  "\r\n     ,修正日" +
                                  "\r\n ) SELECT" +
                                  "\r\n     A.企業ＣＤ" +
                                  "\r\n     ,A.喫食日" +
                                  "\r\n     ,A.得意先ＣＤ" +
                                  "\r\n     ,A.部署ＣＤ" +
                                  "\r\n     ,A.明細番号" +
                                  "\r\n     ,A.商品ＣＤ" +
                                  "\r\n     ,A.商品グループＣＤ" +
                                  "\r\n     ,CASE WHEN T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN @売上区分チケット ELSE A.売掛現金区分 END AS 売上区分" +
                                  "\r\n     ,A.商品単価" +
                                  "\r\n     ,A.商品数量" +
                                  "\r\n     ,0" +
                                  "\r\n     ,NULL" +
                                  "\r\n     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.商品金額合計     END AS 値引前金額" +
                                  "\r\n     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.商品金額合計     END AS 値引後金額" +
                                  "\r\n     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.消費税額         END AS 消費税額" +
                                  "\r\n     ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0 AND T.支払方法 = @支払方法チケット AND S.弁当区分 <> @弁当区分チケット THEN 0 ELSE A.商品金額合計税込 END AS 商品金額合計税込" +
                                  "\r\n     ,A.計算税率" +
                                  "\r\n     ,CASE WHEN S.弁当区分 = @弁当区分チケット THEN 1 ELSE 0 END AS チケット束フラグ" +
                                  "\r\n     ,0" +
                                  "\r\n     ,S.主食ＣＤ" +
                                  "\r\n     ,S.副食ＣＤ" +
                                  "\r\n     ,S.食事区分" +
                                  "\r\n     ,NULL" +
                                  "\r\n     ,@作成担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n     ,@修正担当者ＣＤ" +
                                  "\r\n     ,getdate()" +
                                  "\r\n FROM" +
                                  "\r\n     注文明細 AS A" +
                                  "\r\n     INNER JOIN 注文ヘッダ AS H" +
                                  "\r\n         ON  H.企業ＣＤ   = A.企業ＣＤ" +
                                  "\r\n         AND H.注文区分   = A.注文区分" +
                                  "\r\n         AND H.喫食日     = A.喫食日" +
                                  "\r\n         AND H.得意先ＣＤ = A.得意先ＣＤ" +
                                  "\r\n         AND H.部署ＣＤ   = A.部署ＣＤ" +
                                  "\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                                  "\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = A.企業ＣＤ AND T.得意先ＣＤ = A.得意先ＣＤ" +
                                  "\r\n     INNER JOIN 管理マスタ AS K ON K.企業ＣＤ = A.企業ＣＤ" +
                                  "\r\n WHERE " +
                                  "\r\n     H.企業ＣＤ   = @企業ＣＤ" +
                                  "\r\n AND H.注文区分   = @注文区分" +
                                  "\r\n AND H.喫食日     = @喫食日" +
                                  "\r\n AND H.得意先ＣＤ = @得意先ＣＤ" +
                                  "\r\n AND H.部署ＣＤ   = @部署ＣＤ" +
                                  "\r\n AND H.コースＣＤ = @コースＣＤ";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                cmd.Parameters.Add("@支払方法チケット", SqlDbType.Int).Value = (int)Consts.ShiharaiHohoKubun.Ticket;
                cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                UserData userData = UserData.getInstance();
                cmd.Parameters.Add("@作成担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;
                cmd.Parameters.Add("@修正担当者ＣＤ", SqlDbType.Int).Value = userData.担当者ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (SqlException ex)
            {
                log.Error(ex);
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("売上明細が重複しています。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    History.Error(ex);
                    MessageBox.Show(string.Format("売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                tran.Rollback();
                History.Error(ex);
                MessageBox.Show(string.Format("売上明細の{0}に失敗しました。\n{1}", historyProcess.DisplayName(), ex.ToString()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// LINEによる注文の有無を返します
        /// </summary>
        /// <param name="con">DB接続</param>
        /// <param name="tran">トランザクション</param>
        /// <param name="tokuisakiCD">得意先CD</param>
        /// <returns>LINEによる注文が有ればTrue</returns>
        private bool HasOrderFromLine(SqlConnection con, SqlTransaction tran, int tokuisakiCD)
        {
            try
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.Transaction = tran;

                cmd.CommandText = "\r\n SELECT" +
                                  "\r\n     COUNT(A.明細番号) AS CNT" +
                                  "\r\n FROM" +
                                  "\r\n     注文明細 AS A" +
                                  "\r\n     INNER JOIN 注文ヘッダ AS H" +
                                  "\r\n         ON  H.企業ＣＤ   = A.企業ＣＤ" +
                                  "\r\n         AND H.注文区分   = A.注文区分" +
                                  "\r\n         AND H.喫食日     = A.喫食日" +
                                  "\r\n         AND H.得意先ＣＤ = A.得意先ＣＤ" +
                                  "\r\n         AND H.部署ＣＤ   = A.部署ＣＤ" +
                                  "\r\n WHERE" +
                                  "\r\n     H.企業ＣＤ     = @企業ＣＤ" +
                                  "\r\n AND H.注文区分     = @注文区分" +
                                  "\r\n AND H.喫食日       = @喫食日" +
                                  "\r\n AND H.得意先ＣＤ   = @得意先ＣＤ" +
                                  "\r\n AND H.部署ＣＤ     = @部署ＣＤ" +
                                  "\r\n AND H.コースＣＤ   = @コースＣＤ" +
                                  "\r\n AND A.売掛現金区分 = @売掛現金区分LINE";

                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = tokuisakiCD;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                cmd.Parameters.Add("@売掛現金区分LINE", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Line;

                log.Debug(cmd.CommandTextWithParameters());

                int cnt = 0;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    cnt = (int)reader["CNT"];
                }

                // レコードの有無を返す
                return (cnt > 0);
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
        /// LINE決済用中間サーバの配達完了受入APIを非同期で呼びだす
        /// </summary>
        /// <param name="apiTargetTokuisakiCDList">得意先CDのリスト</param>
        private void CallSetArrived(List<int> apiTargetTokuisakiCDList)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // 配達完了受入API
                        LineProxy lineProxy = new LineProxy();
                        lineProxy.SetArrived((int)_courseChumonCriteria.部署ＣＤ, (DateTime)_courseChumonCriteria.注文日, apiTargetTokuisakiCDList);
                    }
                    catch (AggregateException ex)   // 非同期のメソッドで発生した例外をCatchする
                    {
                        log.Error(ex);
                        History.Error(ex);
                        // ここで throw new EmuException(ex) しても上位には伝播されないので、ここでメッセージボックスを表示する
                        MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }).ContinueWith(_ =>
                {
                    // 非同期処理の完了後の処理

                    // 特に何もしない

                }, TaskScheduler.FromCurrentSynchronizationContext());  // UIスレッドで動作させる
        }

        /// <summary>
        /// 配送集計表印刷
        /// </summary>
        private void PrintHaisoSyukeihyo()
        {
            try
            {
                Report500A report = new Report500A();       // 配送集計表
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 部署マスタから配送集計表出力区分を取得
                Busyo busyo = Busyo.GetBusyo((int)_courseChumonCriteria.部署ＣＤ);

                // 列として採用する商品名の一覧をデータテーブルとして取得する
                DataTable syohinDt = GetSyohinTableForPrint(busyo);

                // データ本体を取得
                DataTable dt = new DataTable("注文明細");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" WITH 対象コース得意先 AS (");
                    // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     CT.企業ＣＤ");
                    sql.AppendLine("     ,CT.コースＣＤ");
                    sql.AppendLine("     ,CT.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     コーステーブル AS CT");
                    sql.AppendLine("     INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コースマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("             AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("             AND 実施日 = @注文日");
                    sql.AppendLine("         ) AS RM");
                    sql.AppendLine("         ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("     AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("     AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine(" UNION");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     RT.企業ＣＤ");
                    sql.AppendLine("     ,RT.コースＣＤ");
                    sql.AppendLine("     ,RT.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     臨時コーステーブル AS RT");
                    sql.AppendLine("     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("     AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("     AND RT.実施日 = @注文日");
                    sql.AppendLine(" )");

                    // -- 駆動表
                    sql.AppendLine(" ,駆動表 AS (");
                    sql.AppendLine(" SELECT DISTINCT");
                    sql.AppendLine("     D.企業ＣＤ");
                    sql.AppendLine("     ,D.コースＣＤ");
                    sql.AppendLine("     ,D.コース得意先 AS コース得意先ＣＤ");
                    sql.AppendLine("     ,D.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    // -- ①－１（指定されたコースにぶら下がる得意先(集約先)）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         TH.企業ＣＤ");
                    sql.AppendLine("         ,TH.コースＣＤ");
                    sql.AppendLine("         ,TH.得意先ＣＤ AS コース得意先");
                    sql.AppendLine("         ,TH.配送得意先ＣＤ");
                    sql.AppendLine("         ,SY.集約先ＣＤ");
                    sql.AppendLine("         ,ISNULL(SY.集約先ＣＤ, TH.得意先ＣＤ) AS 得意先ＣＤ");     // -- 他SQLに提供する得意先ＣＤ
                    sql.AppendLine("     FROM");
                    // -- ①－１－１（指定されたコースにぶら下がる得意先と配送先）
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             W.企業ＣＤ");
                    sql.AppendLine("             ,W.得意先ＣＤ");
                    sql.AppendLine("             ,T.配送得意先ＣＤ");
                    sql.AppendLine("             ,W.コースＣＤ");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             対象コース得意先 AS W");
                    // -- 得意先と配送先を取得したいのでLEFT JOINしている
                    // -- ホントは 「ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ」 として取得したかったが、インデックスが効かなくなるので
                    // -- T.配送得意先ＣＤ には NULL を格納せず、代わりに得意先ＣＤを格納している
                    sql.AppendLine("             LEFT JOIN 得意先マスタ AS T");
                    sql.AppendLine("                 ON T.企業ＣＤ = W.企業ＣＤ");
                    sql.AppendLine("                 AND T.得意先ＣＤ = W.得意先ＣＤ");
                    sql.AppendLine("         ) AS TH");
                    // -- ①－１－２（指定されたコースにぶら下がる得意先のうち、注文の集約先になりうる得意先を抽出）
                    sql.AppendLine("         LEFT JOIN");
                    sql.AppendLine("             (SELECT DISTINCT");
                    sql.AppendLine("                 W.企業ＣＤ");
                    sql.AppendLine("                 ,W.得意先ＣＤ");
                    sql.AppendLine("                 ,T.配送得意先ＣＤ AS 集約先ＣＤ");
                    sql.AppendLine("                 ,W.コースＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 対象コース得意先 AS W");
                    // -- 集約先になりうる得意先のみを抽出したいのでINNER JOINしている
                    sql.AppendLine("                 INNER JOIN (");
                    sql.AppendLine("                     SELECT");
                    sql.AppendLine("                         企業ＣＤ");
                    sql.AppendLine("                         ,得意先ＣＤ");
                    sql.AppendLine("                         ,ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ");
                    sql.AppendLine("                     FROM");
                    sql.AppendLine("                         得意先マスタ");
                    sql.AppendLine("                     ) AS T");
                    sql.AppendLine("                     ON T.企業ＣＤ = W.企業ＣＤ");
                    sql.AppendLine("                     AND T.配送得意先ＣＤ = W.得意先ＣＤ");
                    sql.AppendLine("             ) AS SY");
                    sql.AppendLine("             ON ISNULL(TH.配送得意先ＣＤ, TH.得意先ＣＤ) = SY.集約先ＣＤ");     // -- 集約先と結合するのは配送先を優先する（集約先≒配送先なので）
                    sql.AppendLine("     ) AS D");
                    // -- ①－２（指定されたコースを持つ注文の(ただしコーステーブルに無い)得意先）
                    sql.AppendLine(" UNION");
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     CH.企業ＣＤ");
                    sql.AppendLine("     ,CH.コースＣＤ");
                    sql.AppendLine("     ,CH.得意先ＣＤ AS コース得意先ＣＤ");
                    sql.AppendLine("     ,CH.得意先ＣＤ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     注文ヘッダ AS CH");
                    sql.AppendLine("     LEFT JOIN 対象コース得意先 AS W");
                    sql.AppendLine("         ON CH.企業ＣＤ = W.企業ＣＤ");
                    sql.AppendLine("         AND CH.得意先ＣＤ = W.得意先ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     CH.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("     AND CH.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("     AND CH.喫食日 = @注文日");
                    sql.AppendLine("     AND CH.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     AND W.得意先ＣＤ IS NULL");
                    sql.AppendLine(" )");

                    // -- 配送集計表
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     B.部署名,");
                    sql.AppendLine("     B.会社名称,");
                    sql.AppendLine("     B.電話番号,");
                    sql.AppendLine("     B.ＦＡＸ,");
                    sql.AppendLine(string.Format("     '{0:yyyy/MM/dd(ddd)}' AS 配送日,", _courseChumonCriteria.注文日));
                    sql.AppendLine("     CO.コースＣＤ,");
                    sql.AppendLine("     CO.コース名,");
                    sql.AppendLine("     D.担当者名,");
                    sql.AppendLine("     CO.コース順,");
                    sql.AppendLine("     T.得意先ＣＤ,");
                    if (SystemMaster.GetDispTokuisakiRyaku())
                    {
                        sql.AppendLine("     T.得意先名略称 AS 得意先名,");
                    }
                    else
                    {
                        sql.AppendLine("     T.得意先名,");
                    }
                    sql.AppendLine("     T.電話番号1 AS 得意先電話番号,");
                    sql.AppendLine("     CASE T.締区分");
                    sql.AppendLine("         WHEN 0 THEN '日'");
                    sql.AppendLine("         WHEN 1 THEN '週'");
                    sql.AppendLine("         WHEN 2 THEN CASE T.月締日 WHEN 99 THEN '末' ELSE CONVERT(varchar, T.月締日) END");
                    sql.AppendLine("     END AS 締日,");
                    sql.AppendLine("     VK.売掛現金区分名 AS 売掛現金区分,");
                    sql.AppendLine("     A.商品ＣＤ,");
                    sql.AppendLine("     A.商品数量,");
                    sql.AppendLine("     CK.注文計 AS 合計金額,");
                    sql.AppendLine("     CASE T.売掛現金区分 WHEN @売掛現金区分売掛 THEN 0 ELSE CK.注文計 END AS 現金計");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表
                    sql.AppendLine("     駆動表 AS C");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    // -- ②駆動表に紐付く注文（注文元の数量は配送先に集約する）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,C.得意先ＣＤ");
                    sql.AppendLine("             ,ISNULL(CONVERT(varchar, S.商品ＣＤ), 'XXX') AS 商品ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品数量) AS 商品数量");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             駆動表 AS C");
                    sql.AppendLine("             INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                 ON C.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("                 AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("                 AND C.コース得意先ＣＤ = H.得意先ＣＤ");   // -- 注文元の数量は配送先として計上する
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("             LEFT JOIN");

                    // -- 出力できる商品の最大列数は16（その他(XXX)を入れて17）
                    if (busyo.配送集計表出力区分 == (int)Consts.HaisoSyukeihyoSyohin.SyuyoSyohin)
                    {
                        // 主要商品マスタの商品を表示する場合
                        sql.AppendFormat("                 (SELECT TOP({0})", PRINT_GET_SYOHINS).AppendLine();
                        sql.AppendLine("                     SS.企業ＣＤ");
                        sql.AppendLine("                     ,SS.商品ＣＤ");
                        sql.AppendLine("                     ,SS.部署ＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     主要商品リストマスタ AS SS");
                        sql.AppendLine("                     LEFT JOIN 商品マスタ AS SM ON SM.企業ＣＤ = SS.企業ＣＤ AND SM.商品ＣＤ = SS.商品ＣＤ");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     SS.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND SS.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("                     AND ISNULL(SS.無効フラグ, 0) = 0");
                        sql.AppendLine("                 ORDER BY");
                        sql.AppendLine("                     CASE WHEN SS.商品グループＣＤ IS NULL THEN 1 ELSE 0 END");
                        sql.AppendLine("                     ,SS.商品グループＣＤ");
                        sql.AppendLine("                     ,SM.弁当区分");
                        sql.AppendLine("                     ,SS.商品ＣＤ");
                        sql.AppendLine("                 ) AS S");
                    }
                    else
                    {
                        // 注文に存在する商品を表示する場合
                        sql.AppendFormat("                 (SELECT DISTINCT TOP({0})", PRINT_GET_SYOHINS).AppendLine();
                        sql.AppendLine("                     H.企業ＣＤ");
                        sql.AppendLine("                     ,H.部署ＣＤ");
                        sql.AppendLine("                     ,CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END AS 有効主要商品優先");
                        sql.AppendLine("                     ,A.商品グループＣＤ");
                        sql.AppendLine("                     ,S.弁当区分");
                        sql.AppendLine("                     ,M.商品ＣＤ");
                        sql.AppendLine("                 FROM");
                        sql.AppendLine("                     注文ヘッダ AS H");
                        sql.AppendLine("                     INNER JOIN 注文明細 AS M");
                        sql.AppendLine("                         ON  H.企業ＣＤ   = M.企業ＣＤ");
                        sql.AppendLine("                         AND H.注文区分   = M.注文区分");
                        sql.AppendLine("                         AND H.喫食日     = M.喫食日");
                        sql.AppendLine("                         AND H.得意先ＣＤ = M.得意先ＣＤ");
                        sql.AppendLine("                         AND H.部署ＣＤ   = M.部署ＣＤ");
                        sql.AppendLine("                     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                        sql.AppendLine("                     LEFT JOIN 主要商品リストマスタ AS A ON A.企業ＣＤ = S.企業ＣＤ AND A.部署ＣＤ = @部署ＣＤ AND A.商品ＣＤ = S.商品ＣＤ AND ISNULL(A.無効フラグ, 0) = 0");
                        sql.AppendLine("                 WHERE");
                        sql.AppendLine("                     H.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("                     AND H.喫食日 = @注文日");
                        sql.AppendLine("                     AND H.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("                     AND H.コースＣＤ = @コースＣＤ");
                        sql.AppendLine("                     AND M.注文区分 = @注文区分注文");
                        sql.AppendLine("                 ORDER BY");
                        sql.AppendLine("                     CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END");
                        sql.AppendLine("                     ,A.商品グループＣＤ");
                        sql.AppendLine("                     ,S.弁当区分");
                        sql.AppendLine("                     ,M.商品ＣＤ");
                        sql.AppendLine("                 ) AS S");
                    }

                    sql.AppendLine("                 ON S.企業ＣＤ = M.企業ＣＤ");
                    sql.AppendLine("                 AND S.商品ＣＤ = M.商品ＣＤ");
                    sql.AppendLine("                 AND S.部署ＣＤ = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,C.得意先ＣＤ");
                    sql.AppendLine("             ,S.商品ＣＤ");
                    sql.AppendLine("         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ AND A.コースＣＤ = C.コースＣＤ");
                    // -- ③注文計
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,C.得意先ＣＤ");
                    sql.AppendLine("             ,SUM(M.商品金額合計税込) AS 注文計");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             駆動表 AS C");
                    sql.AppendLine("             INNER JOIN 注文ヘッダ AS H");
                    sql.AppendLine("                 ON C.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("                 AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("                 AND C.コース得意先ＣＤ = H.得意先ＣＤ");// -- 注文元の数量は配送先として計上する
                    sql.AppendLine("             INNER JOIN 注文明細 AS M");
                    sql.AppendLine("                 ON  H.企業ＣＤ   = M.企業ＣＤ");
                    sql.AppendLine("                 AND H.注文区分   = M.注文区分");
                    sql.AppendLine("                 AND H.喫食日     = M.喫食日");
                    sql.AppendLine("                 AND H.得意先ＣＤ = M.得意先ＣＤ");
                    sql.AppendLine("                 AND H.部署ＣＤ   = M.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND H.喫食日 = @注文日");
                    sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("             AND M.注文区分 = @注文区分注文");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.コースＣＤ");
                    sql.AppendLine("             ,C.得意先ＣＤ");
                    sql.AppendLine("         ) AS CK ON CK.得意先ＣＤ = C.得意先ＣＤ AND CK.コースＣＤ = C.コースＣＤ");
                    // -- ④コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             CT.企業ＣＤ");
                    sql.AppendLine("             ,CM.コース名");
                    sql.AppendLine("             ,CM.コースＣＤ");
                    sql.AppendLine("             ,CM.担当者ＣＤ");
                    sql.AppendLine("             ,CT.得意先ＣＤ");
                    sql.AppendLine("             ,CT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             コーステーブル AS CT");
                    sql.AppendLine("             INNER JOIN コースマスタ AS CM");
                    sql.AppendLine("                 ON CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.AppendLine("                 AND CM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("                 AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.AppendLine("             LEFT JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     コースＣＤ");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     臨時コースマスタ");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND コースＣＤ = @コースＣＤ");
                    sql.AppendLine("                     AND 実施日 = @注文日");
                    sql.AppendLine("                 ) AS RM");
                    sql.AppendLine("                 ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             CT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND CT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("             AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.AppendLine("         UNION");
                    sql.AppendLine("         SELECT");
                    sql.AppendLine("             RT.企業ＣＤ");
                    sql.AppendLine("             ,RM.コース名");
                    sql.AppendLine("             ,RM.コースＣＤ");
                    sql.AppendLine("             ,RM.担当者ＣＤ");
                    sql.AppendLine("             ,RT.得意先ＣＤ");
                    sql.AppendLine("             ,RT.コース順");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             臨時コーステーブル AS RT");
                    sql.AppendLine("             INNER JOIN 臨時コースマスタ AS RM");
                    sql.AppendLine("                 ON RM.企業ＣＤ = RT.企業ＣＤ");
                    sql.AppendLine("                 AND RM.コースＣＤ = RT.コースＣＤ");
                    sql.AppendLine("                 AND RM.実施日 = RT.実施日");
                    sql.AppendLine("                 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             RT.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND RT.コースＣＤ = @コースＣＤ");
                    sql.AppendLine("             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.AppendLine("             AND RT.実施日 = @注文日");
                    sql.AppendLine("         ) AS CO ON C.企業ＣＤ = CO.企業ＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS D ON D.企業ＣＤ = CO.企業ＣＤ AND D.担当者ＣＤ = CO.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_売掛現金区分 AS VK ON VK.売掛現金区分CD = ISNULL(T.売掛現金区分, @売掛現金区分現金)");     // -- デフォルトは現金
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     CO.コース順,");
                    sql.AppendLine("     T.得意先ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@売掛現金区分現金", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Genkin;
                    cmd.Parameters.Add("@売掛現金区分売掛", SqlDbType.Int).Value = (int)Consts.UriGenKubun.Urikake;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("配送集計表");
                // 商品名列を作成（最大17列）（不定列ではなく、普通の列として追加する）
                int i;
                for (i = 1; i <= PRINT_SYOHIN_COLUMNS; i++)
                {
                    cldt.Columns.Add(string.Format("商品名{0:00}", i), typeof(string));
                }

                // 固定列を作成
                foreach (DataColumn column in dt.Columns)
                {
                    // 不定列として追加する商品ＣＤと商品数量は除く
                    if (column.ColumnName == "商品ＣＤ" ||
                        column.ColumnName == "商品数量")
                    {
                        continue;
                    }
                    cldt.AddFixedColumn(column);
                }

                // 不定列（を持つデータテーブル）を作成
                DataTable dtSuryo = new DataTable();
                dtSuryo.Columns.Add("数量列名", typeof(string));
                dtSuryo.Columns.Add("商品ＣＤ", typeof(string));
                // 取得した商品名の一覧からレコードを作成
                for (i = 1; i <= syohinDt.Rows.Count; i++)
                {
                    DataRow wkDr = dtSuryo.NewRow();
                    wkDr["数量列名"] = string.Format("数量{0:00}", i);
                    wkDr["商品ＣＤ"] = syohinDt.Rows[i - 1]["商品ＣＤ"];
                    dtSuryo.Rows.Add(wkDr);
                }
                // PRINT_SYOHIN_COLUMNSに満たない分を追加する
                for (; i <= PRINT_SYOHIN_COLUMNS; i++)
                {
                    DataRow wkDr = dtSuryo.NewRow();
                    wkDr["数量列名"] = string.Format("数量{0:00}", i);
                    wkDr["商品ＣＤ"] = string.Format("blank{0:00}", i);
                    dtSuryo.Rows.Add(wkDr);
                }
                cldt.AddColumns(dtSuryo, "数量列名", "商品ＣＤ", typeof(decimal));

                // インポート
                cldt.Import(dt, "商品ＣＤ", "商品数量");

                // 合計行を追加
                DataRow drTotal = cldt.NewRow();
                drTotal["得意先名"] = "【合計】";
                object value;
                for (i = 1; i <= PRINT_SYOHIN_COLUMNS; i++)
                {
                    string columnName = string.Format("数量{0:00}", i);
                    value = cldt.Compute(string.Format("SUM({0})", columnName), "");
                    drTotal[columnName] = value;
                }
                value = cldt.Compute("SUM(現金計)", "");
                drTotal["売掛現金区分"] = "現金計";
                drTotal["合計金額"] = value;
                cldt.Rows.Add(drTotal);

                // 全ての行に対して…
                foreach (DataRow dr in cldt.Rows)
                {
                    // 商品名を埋める
                    for (i = 1; i <= syohinDt.Rows.Count; i++)
                    {
                        string columnName = string.Format("商品名{0:00}", i);
                        string syohinName = (string)syohinDt.Rows[i - 1]["商品名略称"];
                        dr[columnName] = syohinName;
                    }
                    for (; i <= PRINT_SYOHIN_COLUMNS; i++)
                    {
                        string columnName = string.Format("商品名{0:00}", i);
                        dr[columnName] = "";
                    }
                }

                report.DataSource = cldt;

                // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                PrintReport(report, printSettings);

                string code = string.Format("注文日=[{0:yyyy/MM/dd}] コースＣＤ=[{1}]", _courseChumonCriteria.注文日, _courseChumonCriteria.コースＣＤ);

                History.Save(this.Text, "配送集計表", code, History.ProcessType.Output);

                log.InfoFormat("配送集計表を印刷しました。{0}", code);
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 配送集計表の列として採用する商品名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <param name="busyo">部署マスタ</param>
        /// <returns>配送集計表の列として採用する商品名が格納されたデータテーブル</returns>
        private DataTable GetSyohinTableForPrint(Busyo busyo)
        {
            DataTable dt = new DataTable("商品一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    string sql = "";

                    // -- 以下のように並べる
                    // -- ┌───┬───┬───┐
                    // -- │商品Ａ│商品Ｂ│その他│
                    // -- └───┴───┴───┘
                    if (busyo.配送集計表出力区分 == (int)Consts.HaisoSyukeihyoSyohin.SyuyoSyohin)
                    {
                        // 主要商品マスタから商品を取得
                        sql = "\r\n SELECT" +
                              "\r\n     0 AS その他区分" +
                              "\r\n     ,B.商品グループＣＤ" +
                              "\r\n     ,B.商品ＣＤ AS ソート用商品ＣＤ" +
                              "\r\n     ,CONVERT(varchar, B.商品ＣＤ) AS 商品ＣＤ" +
                              "\r\n     ,B.商品名略称" +
                              "\r\n FROM" +
                              // -- 出力できる商品の最大列数は16（その他を入れて17）" +
                              "\r\n     (SELECT TOP({0})" +
                              "\r\n         A.商品グループＣＤ" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n         ,S.商品名略称" +
                              "\r\n     FROM" +
                              "\r\n         主要商品リストマスタ AS A" +
                              "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = A.企業ＣＤ AND A.商品ＣＤ = S.商品ＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         A.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND A.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     ORDER BY" +
                              "\r\n         CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END" +
                              "\r\n         ,A.商品グループＣＤ" +
                              "\r\n         ,S.弁当区分" +
                              "\r\n         ,A.商品ＣＤ" +
                              "\r\n     ) AS B" +
                              "\r\n UNION" +
                              "\r\n SELECT" +
                              "\r\n     1 AS その他区分" +
                              "\r\n     ,-1 AS 商品グループＣＤ" +
                              "\r\n     ,-1 AS ソート用商品ＣＤ" +
                              "\r\n     ,'XXX' AS 商品ＣＤ" +
                              "\r\n     ,'他' AS 商品名略称" +
                              "\r\n ORDER BY" +
                              "\r\n     その他区分" +
                              "\r\n     ,商品グループＣＤ" +
                              "\r\n     ,ソート用商品ＣＤ" +
                              "\r\n     ,商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    }
                    else
                    {
                        // 注文から商品を取得
                        sql = "\r\n SELECT" +
                              "\r\n     0 AS その他区分" +
                              "\r\n     ,CASE WHEN B.商品グループＣＤ IS NULL THEN 1 ELSE 0 END AS 有効主要商品優先" +
                              "\r\n     ,B.商品グループＣＤ" +
                              "\r\n     ,B.商品ＣＤ AS ソート用商品ＣＤ" +
                              "\r\n     ,CONVERT(varchar, B.商品ＣＤ) AS 商品ＣＤ" +
                              "\r\n     ,B.商品名略称" +
                              "\r\n FROM" +
                              // -- 出力できる商品の最大列数は16（その他を入れて17）
                              "\r\n     (SELECT DISTINCT TOP({0})" +
                              "\r\n         CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END AS 有効主要商品優先" +
                              "\r\n         ,A.商品グループＣＤ" +
                              "\r\n         ,S.弁当区分" +
                              "\r\n         ,M.商品ＣＤ" +
                              "\r\n         ,S.商品名略称" +
                              "\r\n     FROM" +
                              "\r\n         注文ヘッダ AS H" +
                              "\r\n         INNER JOIN 注文明細 AS M" +
                              "\r\n             ON  H.企業ＣＤ   = M.企業ＣＤ" +
                              "\r\n             AND H.注文区分   = M.注文区分" +
                              "\r\n             AND H.喫食日     = M.喫食日" +
                              "\r\n             AND H.得意先ＣＤ = M.得意先ＣＤ" +
                              "\r\n             AND H.部署ＣＤ   = M.部署ＣＤ" +
                              "\r\n         LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ" +
                              "\r\n         LEFT JOIN 主要商品リストマスタ AS A ON A.企業ＣＤ = S.企業ＣＤ AND A.部署ＣＤ = @部署ＣＤ AND A.商品ＣＤ = S.商品ＣＤ AND ISNULL(A.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         H.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND H.喫食日 = @注文日" +
                              "\r\n         AND H.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         AND H.コースＣＤ = @コースＣＤ" +
                              "\r\n         AND M.注文区分 = @注文区分注文" +
                              "\r\n     ORDER BY" +
                              "\r\n         CASE WHEN A.商品グループＣＤ IS NULL THEN 1 ELSE 0 END" +
                              "\r\n         ,A.商品グループＣＤ" +
                              "\r\n         ,S.弁当区分" +
                              "\r\n         ,M.商品ＣＤ" +
                              "\r\n     ) AS B" +
                              "\r\n UNION" +
                              "\r\n SELECT" +
                              "\r\n     1 AS その他区分" +
                              "\r\n     ,0 AS 有効主要商品優先" +
                              "\r\n     ,-1 AS 商品グループＣＤ" +
                              "\r\n     ,-1 AS ソート用商品ＣＤ" +
                              "\r\n     ,'XXX' AS 商品ＣＤ" +
                              "\r\n     ,'他' AS 商品名略称" +
                              "\r\n ORDER BY" +
                              "\r\n     その他区分" +
                              "\r\n     ,有効主要商品優先" +
                              "\r\n     ,商品グループＣＤ" +
                              "\r\n     ,ソート用商品ＣＤ" +
                              "\r\n     ,商品ＣＤ";

                        cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                        cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                        cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    }

                    cmd.CommandText = string.Format(sql, PRINT_GET_SYOHINS);

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
        /// 配送明細表印刷
        /// </summary>
        private void PrintHaisoMeisaihyo()
        {
            try
            {
                Report211A report = new Report211A();       // 配送明細表
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 配送集計表本体と得意先備考の取得で使用するCTE（共通テーブル式）を作成する
                StringBuilder sql = new StringBuilder();
                sql.Append("\r\n WITH 対象コース得意先 AS (");
                // -- コーステーブルから取得（実施日に該当する臨時コースマスタがあればコーステーブルからは取得しない。無ければ取得する）
                sql.Append("\r\n SELECT");
                sql.Append("\r\n     CT.企業ＣＤ");
                sql.Append("\r\n     ,CT.コースＣＤ");
                sql.Append("\r\n     ,CT.得意先ＣＤ");
                sql.Append("\r\n FROM");
                sql.Append("\r\n     コーステーブル AS CT");
                sql.Append("\r\n     INNER JOIN コースマスタ AS CM ON CM.企業ＣＤ = CT.企業ＣＤ AND CM.コースＣＤ = CT.コースＣＤ AND ISNULL(CM.無効フラグ, 0) = 0");
                sql.Append("\r\n     LEFT JOIN");
                sql.Append("\r\n         (SELECT");
                sql.Append("\r\n             コースＣＤ");
                sql.Append("\r\n         FROM");
                sql.Append("\r\n             臨時コースマスタ");
                sql.Append("\r\n         WHERE");
                sql.Append("\r\n             企業ＣＤ = @企業ＣＤ");
                sql.Append("\r\n             AND ISNULL(無効フラグ, 0) = 0");
                sql.Append("\r\n             AND コースＣＤ = @コースＣＤ");
                sql.Append("\r\n             AND 実施日 = @注文日");
                sql.Append("\r\n         ) AS RM");
                sql.Append("\r\n         ON RM.コースＣＤ = CT.コースＣＤ");
                sql.Append("\r\n WHERE");
                sql.Append("\r\n     CT.企業ＣＤ = @企業ＣＤ");
                sql.Append("\r\n     AND CT.コースＣＤ = @コースＣＤ");
                sql.Append("\r\n     AND ISNULL(CT.無効フラグ, 0) = 0");
                sql.Append("\r\n     AND RM.コースＣＤ IS NULL");
                // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                sql.Append("\r\n UNION");
                sql.Append("\r\n SELECT");
                sql.Append("\r\n     RT.企業ＣＤ");
                sql.Append("\r\n     ,RT.コースＣＤ");
                sql.Append("\r\n     ,RT.得意先ＣＤ");
                sql.Append("\r\n FROM");
                sql.Append("\r\n     臨時コーステーブル AS RT");
                sql.Append("\r\n     INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                sql.Append("\r\n WHERE");
                sql.Append("\r\n     RT.企業ＣＤ = @企業ＣＤ");
                sql.Append("\r\n     AND RT.コースＣＤ = @コースＣＤ");
                sql.Append("\r\n     AND ISNULL(RT.無効フラグ, 0) = 0");
                sql.Append("\r\n     AND RT.実施日 = @注文日");
                sql.Append("\r\n )");

                // -- 駆動表
                sql.Append("\r\n ,駆動表 AS (");
                sql.Append("\r\n SELECT DISTINCT");
                sql.Append("\r\n     D.企業ＣＤ");
                sql.Append("\r\n     ,D.コースＣＤ");
                sql.Append("\r\n     ,D.コース得意先 AS コース得意先ＣＤ");
                sql.Append("\r\n     ,D.得意先ＣＤ");
                sql.Append("\r\n FROM");
                // -- ①－１（指定されたコースにぶら下がる得意先(集約先)）
                sql.Append("\r\n     (SELECT");
                sql.Append("\r\n         TH.企業ＣＤ");
                sql.Append("\r\n         ,TH.コースＣＤ");
                sql.Append("\r\n         ,TH.得意先ＣＤ AS コース得意先");
                sql.Append("\r\n         ,TH.配送得意先ＣＤ");
                sql.Append("\r\n         ,SY.集約先ＣＤ");
                sql.Append("\r\n         ,ISNULL(SY.集約先ＣＤ, TH.得意先ＣＤ) AS 得意先ＣＤ");     // -- 他SQLに提供する得意先ＣＤ
                sql.Append("\r\n     FROM");
                // -- ①－１－１（指定されたコースにぶら下がる得意先と配送先）
                sql.Append("\r\n         (SELECT");
                sql.Append("\r\n             W.企業ＣＤ");
                sql.Append("\r\n             ,W.得意先ＣＤ");
                sql.Append("\r\n             ,T.配送得意先ＣＤ");
                sql.Append("\r\n             ,W.コースＣＤ");
                sql.Append("\r\n         FROM");
                sql.Append("\r\n             対象コース得意先 AS W");
                // -- 得意先と配送先を取得したいのでLEFT JOINしている
                sql.Append("\r\n             LEFT JOIN 得意先マスタ AS T");
                sql.Append("\r\n                 ON T.企業ＣＤ = W.企業ＣＤ");
                sql.Append("\r\n                 AND T.得意先ＣＤ = W.得意先ＣＤ");
                sql.Append("\r\n         ) AS TH");
                // -- ①－１－２（指定されたコースにぶら下がる得意先のうち、注文の集約先になりうる得意先を抽出）
                sql.Append("\r\n         LEFT JOIN");
                sql.Append("\r\n             (SELECT DISTINCT");
                sql.Append("\r\n                 W.企業ＣＤ");
                sql.Append("\r\n                 ,W.得意先ＣＤ");
                sql.Append("\r\n                 ,T.配送得意先ＣＤ AS 集約先ＣＤ");
                sql.Append("\r\n                 ,W.コースＣＤ");
                sql.Append("\r\n             FROM");
                sql.Append("\r\n                 対象コース得意先 AS W");
                // -- 集約先になりうる得意先のみを抽出したいのでINNER JOINしている
                // -- ホントは 「ISNULL(配送得意先ＣＤ, 得意先ＣＤ) AS 配送得意先ＣＤ」 として取得したかったが、インデックスが効かなくなるので
                // -- T.配送得意先ＣＤ には NULL を格納せず、代わりに得意先ＣＤを格納している
                sql.Append("\r\n                 INNER JOIN 得意先マスタ AS T");
                sql.Append("\r\n                     ON T.企業ＣＤ = W.企業ＣＤ");
                sql.Append("\r\n                     AND T.配送得意先ＣＤ = W.得意先ＣＤ");
                sql.Append("\r\n             ) AS SY");
                sql.Append("\r\n             ON ISNULL(TH.配送得意先ＣＤ, TH.得意先ＣＤ) = SY.集約先ＣＤ");     // -- 集約先と結合するのは配送先を優先する（集約先≒配送先なので）
                sql.Append("\r\n     ) AS D");
                // -- ①－２（指定されたコースを持つ注文の(ただしコーステーブルに無い)得意先）
                sql.Append("\r\n UNION");
                sql.Append("\r\n SELECT");
                sql.Append("\r\n     CH.企業ＣＤ");
                sql.Append("\r\n     ,CH.コースＣＤ");
                sql.Append("\r\n     ,CH.得意先ＣＤ AS コース得意先ＣＤ");
                sql.Append("\r\n     ,CH.得意先ＣＤ");
                sql.Append("\r\n FROM");
                sql.Append("\r\n     注文ヘッダ AS CH");
                sql.Append("\r\n     LEFT JOIN 対象コース得意先 AS W");
                sql.Append("\r\n         ON CH.企業ＣＤ = W.企業ＣＤ");
                sql.Append("\r\n         AND CH.得意先ＣＤ = W.得意先ＣＤ");
                sql.Append("\r\n WHERE");
                sql.Append("\r\n     CH.企業ＣＤ = @企業ＣＤ");
                sql.Append("\r\n     AND CH.コースＣＤ = @コースＣＤ");
                sql.Append("\r\n     AND CH.喫食日 = @注文日");
                sql.Append("\r\n     AND CH.部署ＣＤ = @部署ＣＤ");
                sql.Append("\r\n     AND W.得意先ＣＤ IS NULL");
                sql.Append("\r\n )");

                string cte = sql.ToString();

                DataTable dt = GetHaisoMeisai(cte);             // データ本体を取得
                DataTable dtBiko = GetHaisoMeisaiBiko(cte);     // 備考を取得

                //*********************************
                // データ本体に合計とその内訳を追加
                //*********************************
                // まず先頭行から配送日～担当者名を取得する
                string busyoName = null;
                string haisobi = null;
                int? courseCD = null;
                string courseName = null;
                string tantouName = null;
                if (dt.Rows.Count != 0)
                {
                    DataRow dr = dt.Rows[0];
                    busyoName = (string)dr["部署名"];
                    haisobi = (string)dr["配送日"];
                    courseCD = dr["コースＣＤ"] as int?;
                    courseName = dr["コース名"].ToString();
                    tantouName = dr["担当者名"].ToString();
                }

                // 【合計】副食の合計を取得して新しいレコードとして追加
                object subTotal = dt.Compute("SUM(商品数量)", string.Format("弁当区分={0}", (int)Consts.BentoKubun.SubDish));
                DataRow newDr = dt.NewRow();
                if (busyoName != null)
                {
                    newDr["部署名"] = busyoName;
                }
                else
                {
                    newDr["部署名"] = DBNull.Value;
                }
                if (haisobi != null)
                {
                    newDr["配送日"] = haisobi;
                }
                else
                {
                    newDr["配送日"] = DBNull.Value;
                }
                if (courseCD != null)
                {
                    newDr["コースＣＤ"] = courseCD;
                }
                else
                {
                    newDr["コースＣＤ"] = DBNull.Value;
                }
                if (courseName != null)
                {
                    newDr["コース名"] = courseName;
                }
                else
                {
                    newDr["コース名"] = DBNull.Value;
                }
                if (tantouName != null)
                {
                    newDr["担当者名"] = tantouName;
                }
                else
                {
                    newDr["担当者名"] = DBNull.Value;
                }
                newDr["得意先ＣＤ"] = Report211A.TOTAL_TOKUISAKI_CD;
                newDr["得意先名"] = "　【合計】";
                newDr["弁当区分"] = (int)Consts.BentoKubun.SubDish;
                newDr["商品数量"] = subTotal == DBNull.Value ? 0m : Convert.ToDecimal(subTotal);
                dt.Rows.Add(newDr);

                // 【合計】主食の合計を取得して新しいレコードとして追加
                object mainTotal = dt.Compute("SUM(商品数量)", string.Format("弁当区分={0}", (int)Consts.BentoKubun.MainDish));
                newDr = dt.NewRow();
                if (busyoName != null)
                {
                    newDr["部署名"] = busyoName;
                }
                else
                {
                    newDr["部署名"] = DBNull.Value;
                }
                if (haisobi != null)
                {
                    newDr["配送日"] = haisobi;
                }
                else
                {
                    newDr["配送日"] = DBNull.Value;
                }
                if (courseCD != null)
                {
                    newDr["コースＣＤ"] = courseCD;
                }
                else
                {
                    newDr["コースＣＤ"] = DBNull.Value;
                }
                if (courseName != null)
                {
                    newDr["コース名"] = courseName;
                }
                else
                {
                    newDr["コース名"] = DBNull.Value;
                }
                if (tantouName != null)
                {
                    newDr["担当者名"] = tantouName;
                }
                else
                {
                    newDr["担当者名"] = DBNull.Value;
                }
                newDr["得意先ＣＤ"] = Report211A.TOTAL_TOKUISAKI_CD;
                newDr["得意先名"] = "　【合計】";
                newDr["弁当区分"] = (int)Consts.BentoKubun.MainDish;
                newDr["商品数量"] = mainTotal == DBNull.Value ? 0m : Convert.ToDecimal(mainTotal);
                dt.Rows.Add(newDr);

                // 【内訳】商品ＣＤごとに集計して新しいレコードとして追加
                var totalList = dt.Rows.Cast<DataRow>()
                                    .Where(row => row["商品ＣＤ"] as int? != null)  // 合計行は除く
                                    .GroupBy(row => new
                                    {
                                        弁当区分 = row["弁当区分"] as int?,
                                        商品ＣＤ = row["商品ＣＤ"] as int?,
                                        商品名 = row["商品名"] as string
                                    })
                                    .Select(g => new
                                    {
                                        弁当区分 = g.Key.弁当区分,
                                        商品ＣＤ = g.Key.商品ＣＤ,
                                        商品名 = g.Key.商品名,
                                        商品数量 = g.Sum(r => r["商品数量"] as decimal?)
                                    });

                foreach (var item in totalList)
                {
                    newDr = dt.NewRow();
                    if (busyoName != null)
                    {
                        newDr["部署名"] = busyoName;
                    }
                    else
                    {
                        newDr["部署名"] = DBNull.Value;
                    }
                    if (haisobi != null)
                    {
                        newDr["配送日"] = haisobi;
                    }
                    else
                    {
                        newDr["配送日"] = DBNull.Value;
                    }
                    if (courseCD != null)
                    {
                        newDr["コースＣＤ"] = courseCD;
                    }
                    else
                    {
                        newDr["コースＣＤ"] = DBNull.Value;
                    }
                    if (courseName != null)
                    {
                        newDr["コース名"] = courseName;
                    }
                    else
                    {
                        newDr["コース名"] = DBNull.Value;
                    }
                    if (tantouName != null)
                    {
                        newDr["担当者名"] = tantouName;
                    }
                    else
                    {
                        newDr["担当者名"] = DBNull.Value;
                    }
                    newDr["得意先ＣＤ"] = Report211A.SUMMARY_TOKUISAKI_CD;
                    newDr["得意先名"] = "　【内訳】";
                    newDr["弁当区分"] = item.弁当区分;
                    newDr["商品数量"] = item.商品数量;
                    newDr["商品名"] = item.商品名;
                    dt.Rows.Add(newDr);
                }

                // ユニークに取得した得意先を新しいデータテーブルとして生成する（親テーブル）
                var tokuisakiList = dt.Rows.Cast<DataRow>()
                                        .Select(row => new
                                        {
                                            コース順 = row["コース順"] as int?,
                                            得意先ＣＤ = row["得意先ＣＤ"] as int?,
                                            得意先名 = row["得意先名"] as string
                                        })
                                        .Distinct();

                DataTable dtTokuisaki = dt.Clone();
                foreach (var item in tokuisakiList)
                {
                    newDr = dtTokuisaki.NewRow();
                    if (busyoName != null)
                    {
                        newDr["部署名"] = busyoName;
                    }
                    else
                    {
                        newDr["部署名"] = DBNull.Value;
                    }
                    if (haisobi != null)
                    {
                        newDr["配送日"] = haisobi;
                    }
                    else
                    {
                        newDr["配送日"] = DBNull.Value;
                    }
                    if (courseCD != null)
                    {
                        newDr["コースＣＤ"] = courseCD;
                    }
                    else
                    {
                        newDr["コースＣＤ"] = DBNull.Value;
                    }
                    if (courseName != null)
                    {
                        newDr["コース名"] = courseName;
                    }
                    else
                    {
                        newDr["コース名"] = DBNull.Value;
                    }
                    if (tantouName != null)
                    {
                        newDr["担当者名"] = tantouName;
                    }
                    else
                    {
                        newDr["担当者名"] = DBNull.Value;
                    }
                    if (item.コース順 != null)
                    {
                        newDr["コース順"] = item.コース順;
                    }
                    else
                    {
                        newDr["コース順"] = DBNull.Value;
                    }
                    newDr["得意先ＣＤ"] = item.得意先ＣＤ;
                    newDr["得意先名"] = item.得意先名;
                    dtTokuisaki.Rows.Add(newDr);
                }

                report.DataSource = dtTokuisaki;
                report.DetailDataSource = dt;
                report.BikoDataSource = dtBiko;

                // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                PrintReport(report, printSettings);

                string code = string.Format("注文日=[{0:yyyy/MM/dd}] コースＣＤ=[{1}]", _courseChumonCriteria.注文日, _courseChumonCriteria.コースＣＤ);

                History.Save(this.Text, "配送明細表", code, History.ProcessType.Output);

                log.InfoFormat("配送明細表を印刷しました。{0}", code);
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
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 配送明細表のデータを取得
        /// </summary>
        /// <param name="cte">CTE（共通テーブル式）</param>
        /// <returns>配送明細表の一覧が格納されたデータテーブル</returns>
        private DataTable GetHaisoMeisai(string cte)
        {
            DataTable dt = new DataTable("配送明細表");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();
                    sql.Append(cte);
                    sql.Append("\r\n SELECT DISTINCT");
                    sql.Append("\r\n     B.部署名");
                    sql.AppendFormat("\r\n     ,'{0:yyyy/MM/dd(ddd)}' AS 配送日", _courseChumonCriteria.注文日);
                    sql.Append("\r\n     ,CM.コースＣＤ");
                    sql.Append("\r\n     ,CM.コース名");
                    sql.Append("\r\n     ,D.担当者名");
                    sql.Append("\r\n     ,CO.コース順");
                    sql.Append("\r\n     ,T.得意先ＣＤ");
                    sql.Append("\r\n     ,T.得意先名");
                    sql.Append("\r\n     ,A.商品ＣＤ");
                    sql.Append("\r\n     ,S.商品名略称 AS 商品名");
                    sql.Append("\r\n     ,A.弁当区分");
                    sql.Append("\r\n     ,A.商品数量");
                    sql.Append("\r\n     ,CASE WHEN CO.コース順 IS NULL THEN 1 ELSE 0 END AS コースに含む順");
                    sql.Append("\r\n FROM");
                    // -- ①駆動表
                    sql.Append("\r\n     駆動表 AS C");
                    sql.Append("\r\n     LEFT JOIN 得意先マスタ AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    sql.Append("\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = C.企業ＣＤ AND B.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = C.企業ＣＤ AND CM.コースＣＤ = @コースＣＤ");
                    // -- ②駆動表に紐付く注文（注文元の数量は配送先に集約する）
                    sql.Append("\r\n     LEFT JOIN");
                    // -- ②注文の部品（各商品を分割した部品ごとに合計する）
                    sql.Append("\r\n         (SELECT");
                    sql.Append("\r\n             C.得意先ＣＤ");
                    sql.Append("\r\n             ,PART.弁当区分");
                    sql.Append("\r\n             ,PART.コースＣＤ");
                    sql.Append("\r\n             ,PART.部品ＣＤ AS 商品ＣＤ");
                    sql.Append("\r\n             ,SUM(PART.商品数量) AS 商品数量");
                    sql.Append("\r\n         FROM");
                    sql.Append("\r\n             駆動表 AS C");
                    sql.Append("\r\n             INNER JOIN");
                    // -- ②－１ 弁当の主食
                    sql.Append("\r\n                 (SELECT");
                    sql.Append("\r\n                     @弁当区分主食 AS 弁当区分");
                    sql.Append("\r\n                     ,H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.主食ＣＤ AS 部品ＣＤ");
                    sql.Append("\r\n                     ,SUM(D.商品数量) AS 商品数量");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     注文ヘッダ AS H");
                    sql.Append("\r\n                     INNER JOIN 注文明細 AS D");
                    sql.Append("\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.Append("\r\n                         AND D.注文区分   = H.注文区分");
                    sql.Append("\r\n                         AND D.喫食日     = H.喫食日");
                    sql.Append("\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.Append("\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.Append("\r\n                     INNER JOIN 商品マスタ AS I");
                    sql.Append("\r\n                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.Append("\r\n                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.Append("\r\n                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     H.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND H.注文区分 = @注文区分注文");
                    sql.Append("\r\n                     AND H.喫食日 = @注文日");
                    sql.Append("\r\n                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n                     AND H.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.Append("\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.Append("\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.Append("\r\n                 GROUP BY");
                    sql.Append("\r\n                     H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.主食ＣＤ");
                    sql.Append("\r\n                 UNION ALL");       // -- SUM()の対象にするので、同一部署、同一コース、同一部品ＣＤ、同一売上日、同一商品数量を同一視せずに全て抽出する（以下同様）
                    // -- ③－２ 弁当の副食
                    sql.Append("\r\n                 SELECT");
                    sql.Append("\r\n                     @弁当区分副食 AS 弁当区分");
                    sql.Append("\r\n                     ,H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.副食ＣＤ AS 部品ＣＤ");
                    sql.Append("\r\n                     ,SUM(D.商品数量) AS 商品数量");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     注文ヘッダ AS H");
                    sql.Append("\r\n                     INNER JOIN 注文明細 AS D");
                    sql.Append("\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.Append("\r\n                         AND D.注文区分   = H.注文区分");
                    sql.Append("\r\n                         AND D.喫食日     = H.喫食日");
                    sql.Append("\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.Append("\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.Append("\r\n                     INNER JOIN 商品マスタ AS I");
                    sql.Append("\r\n                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.Append("\r\n                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.Append("\r\n                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     H.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND H.注文区分 = @注文区分注文");
                    sql.Append("\r\n                     AND H.喫食日 = @注文日");
                    sql.Append("\r\n                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n                     AND H.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.Append("\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.Append("\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.Append("\r\n                 GROUP BY");
                    sql.Append("\r\n                     H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.副食ＣＤ");
                    sql.Append("\r\n                 UNION ALL");
                    // -- ②－３ 主食のみ
                    sql.Append("\r\n                 SELECT");
                    sql.Append("\r\n                     @弁当区分主食 AS 弁当区分");
                    sql.Append("\r\n                     ,H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.Append("\r\n                     ,SUM(D.商品数量) AS 商品数量");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     注文ヘッダ AS H");
                    sql.Append("\r\n                     INNER JOIN 注文明細 AS D");
                    sql.Append("\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.Append("\r\n                         AND D.注文区分   = H.注文区分");
                    sql.Append("\r\n                         AND D.喫食日     = H.喫食日");
                    sql.Append("\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.Append("\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.Append("\r\n                     INNER JOIN 商品マスタ AS I");
                    sql.Append("\r\n                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.Append("\r\n                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.Append("\r\n                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     H.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND H.注文区分 = @注文区分注文");
                    sql.Append("\r\n                     AND H.喫食日 = @注文日");
                    sql.Append("\r\n                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n                     AND H.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.Append("\r\n                 GROUP BY");
                    sql.Append("\r\n                     H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ");
                    sql.Append("\r\n                 UNION ALL");
                    // -- ②－４ 副食のみ
                    sql.Append("\r\n                 SELECT");
                    sql.Append("\r\n                     @弁当区分副食 AS 弁当区分");
                    sql.Append("\r\n                     ,H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.Append("\r\n                     ,SUM(D.商品数量) AS 商品数量");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     注文ヘッダ AS H");
                    sql.Append("\r\n                     INNER JOIN 注文明細 AS D");
                    sql.Append("\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.Append("\r\n                         AND D.注文区分   = H.注文区分");
                    sql.Append("\r\n                         AND D.喫食日     = H.喫食日");
                    sql.Append("\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.Append("\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.Append("\r\n                     INNER JOIN 商品マスタ AS I");
                    sql.Append("\r\n                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.Append("\r\n                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.Append("\r\n                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     H.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND H.注文区分 = @注文区分注文");
                    sql.Append("\r\n                     AND H.喫食日 = @注文日");
                    sql.Append("\r\n                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n                     AND H.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.Append("\r\n                 GROUP BY");
                    sql.Append("\r\n                     H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ");
                    sql.Append("\r\n                 UNION ALL");
                    // -- ②－６ その他の主食
                    sql.Append("\r\n                 SELECT");
                    sql.Append("\r\n                     @弁当区分主食 AS 弁当区分");
                    sql.Append("\r\n                     ,H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.Append("\r\n                     ,SUM(D.商品数量) AS 商品数量");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     注文ヘッダ AS H");
                    sql.Append("\r\n                     INNER JOIN 注文明細 AS D");
                    sql.Append("\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.Append("\r\n                         AND D.注文区分   = H.注文区分");
                    sql.Append("\r\n                         AND D.喫食日     = H.喫食日");
                    sql.Append("\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.Append("\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.Append("\r\n                     INNER JOIN 商品マスタ AS I");
                    sql.Append("\r\n                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.Append("\r\n                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.Append("\r\n                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     H.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND H.注文区分 = @注文区分注文");
                    sql.Append("\r\n                     AND H.喫食日 = @注文日");
                    sql.Append("\r\n                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n                     AND H.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他)");       // -- チケットとその他
                    sql.Append("\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.Append("\r\n                     AND ISNULL(I.主食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.Append("\r\n                 GROUP BY");
                    sql.Append("\r\n                     H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ");
                    sql.Append("\r\n                 UNION ALL");
                    // -- ②－７ その他の副食
                    sql.Append("\r\n                 SELECT");
                    sql.Append("\r\n                     @弁当区分副食 AS 弁当区分");
                    sql.Append("\r\n                     ,H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ AS 部品ＣＤ");
                    sql.Append("\r\n                     ,SUM(D.商品数量) AS 商品数量");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     注文ヘッダ AS H");
                    sql.Append("\r\n                     INNER JOIN 注文明細 AS D");
                    sql.Append("\r\n                         ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.Append("\r\n                         AND D.注文区分   = H.注文区分");
                    sql.Append("\r\n                         AND D.喫食日     = H.喫食日");
                    sql.Append("\r\n                         AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.Append("\r\n                         AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.Append("\r\n                     INNER JOIN 商品マスタ AS I");
                    sql.Append("\r\n                         ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.Append("\r\n                         AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.Append("\r\n                         AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     H.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND H.注文区分 = @注文区分注文");
                    sql.Append("\r\n                     AND H.喫食日 = @注文日");
                    sql.Append("\r\n                     AND H.部署ＣＤ = @部署ＣＤ");
                    sql.Append("\r\n                     AND H.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND ISNULL(I.弁当区分, 0) IN (@弁当区分チケット, @弁当区分その他)");       // -- チケットとその他
                    sql.Append("\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.Append("\r\n                     AND ISNULL(I.副食ＣＤ, 0) <> I.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.Append("\r\n                 GROUP BY");
                    sql.Append("\r\n                     H.コースＣＤ");
                    sql.Append("\r\n                     ,H.得意先ＣＤ");
                    sql.Append("\r\n                     ,I.商品ＣＤ");
                    sql.Append("\r\n                 ) AS PART");
                    sql.Append("\r\n                     ON PART.コースＣＤ = C.コースＣＤ AND PART.得意先ＣＤ = C.コース得意先ＣＤ");      // -- 注文元の数量は配送先として計上する
                    sql.Append("\r\n         GROUP BY");
                    sql.Append("\r\n             C.得意先ＣＤ");
                    sql.Append("\r\n             ,PART.弁当区分");
                    sql.Append("\r\n             ,PART.コースＣＤ");
                    sql.Append("\r\n             ,PART.部品ＣＤ");
                    sql.Append("\r\n         ) AS A ON A.得意先ＣＤ = C.得意先ＣＤ AND A.コースＣＤ = C.コースＣＤ");
                    sql.Append("\r\n     LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = C.企業ＣＤ AND S.商品ＣＤ = A.商品ＣＤ");
                    // -- ④コース順（①で取得していないのはなぜ？①－２からはコース順が取得できないので代わりにNULLにするとUNIONしたときに複数行できてしまうため）
                    sql.Append("\r\n     LEFT JOIN");
                    sql.Append("\r\n         (SELECT");
                    sql.Append("\r\n             CT.企業ＣＤ");
                    sql.Append("\r\n             ,CM.コース名");
                    sql.Append("\r\n             ,CM.コースＣＤ");
                    sql.Append("\r\n             ,CM.担当者ＣＤ");
                    sql.Append("\r\n             ,CT.得意先ＣＤ");
                    sql.Append("\r\n             ,CT.コース順");
                    sql.Append("\r\n         FROM");
                    sql.Append("\r\n             コーステーブル AS CT");
                    sql.Append("\r\n             INNER JOIN コースマスタ AS CM");
                    sql.Append("\r\n                 ON CM.企業ＣＤ = CT.企業ＣＤ");
                    sql.Append("\r\n                 AND CM.コースＣＤ = CT.コースＣＤ");
                    sql.Append("\r\n                 AND ISNULL(CM.無効フラグ, 0) = 0");
                    sql.Append("\r\n             LEFT JOIN");
                    sql.Append("\r\n                 (SELECT");
                    sql.Append("\r\n                     コースＣＤ");
                    sql.Append("\r\n                 FROM");
                    sql.Append("\r\n                     臨時コースマスタ");
                    sql.Append("\r\n                 WHERE");
                    sql.Append("\r\n                     企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n                     AND ISNULL(無効フラグ, 0) = 0");
                    sql.Append("\r\n                     AND コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n                     AND 実施日 = @注文日");
                    sql.Append("\r\n                 ) AS RM");
                    sql.Append("\r\n                 ON RM.コースＣＤ = CT.コースＣＤ");
                    sql.Append("\r\n         WHERE");
                    sql.Append("\r\n             CT.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n             AND CT.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n             AND ISNULL(CT.無効フラグ, 0) = 0");
                    sql.Append("\r\n             AND RM.コースＣＤ IS NULL");
                    // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                    sql.Append("\r\n         UNION");
                    sql.Append("\r\n         SELECT");
                    sql.Append("\r\n             RT.企業ＣＤ");
                    sql.Append("\r\n             ,RM.コース名");
                    sql.Append("\r\n             ,RM.コースＣＤ");
                    sql.Append("\r\n             ,RM.担当者ＣＤ");
                    sql.Append("\r\n             ,RT.得意先ＣＤ");
                    sql.Append("\r\n             ,RT.コース順");
                    sql.Append("\r\n         FROM");
                    sql.Append("\r\n             臨時コーステーブル AS RT");
                    sql.Append("\r\n             INNER JOIN 臨時コースマスタ AS RM");
                    sql.Append("\r\n                 ON RM.企業ＣＤ = RT.企業ＣＤ");
                    sql.Append("\r\n                 AND RM.コースＣＤ = RT.コースＣＤ");
                    sql.Append("\r\n                 AND RM.実施日 = RT.実施日");
                    sql.Append("\r\n                 AND ISNULL(RM.無効フラグ, 0) = 0");
                    sql.Append("\r\n         WHERE");
                    sql.Append("\r\n             RT.企業ＣＤ = @企業ＣＤ");
                    sql.Append("\r\n             AND RT.コースＣＤ = @コースＣＤ");
                    sql.Append("\r\n             AND ISNULL(RT.無効フラグ, 0) = 0");
                    sql.Append("\r\n             AND RT.実施日 = @注文日");
                    sql.Append("\r\n         ) AS CO ON C.企業ＣＤ = CO.企業ＣＤ AND C.得意先ＣＤ = CO.得意先ＣＤ");
                    sql.Append("\r\n     LEFT JOIN 担当者マスタ AS D ON D.企業ＣＤ = CO.企業ＣＤ AND D.担当者ＣＤ = CO.担当者ＣＤ");
                    sql.Append("\r\n ORDER BY");
                    sql.Append("\r\n     コースに含む順");
                    sql.Append("\r\n     ,CO.コース順");
                    sql.Append("\r\n     ,T.得意先ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;
                    cmd.Parameters.Add("@注文区分注文", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("配送明細表を取得しました。");
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
        /// 配送明細表の備考を取得
        /// </summary>
        /// <param name="cte">CTE（共通テーブル式）</param>
        /// <returns>配送明細表の備考が格納されたデータテーブル</returns>
        private DataTable GetHaisoMeisaiBiko(string cte)
        {
            DataTable dt = new DataTable("配送明細表備考");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();
                    sql.Append(cte);
                    sql.Append("\r\n SELECT DISTINCT");
                    sql.Append("\r\n     T.得意先ＣＤ");
                    sql.Append("\r\n     ,T.ソート順");
                    sql.Append("\r\n     ,T.備考");
                    sql.Append("\r\n FROM");
                    // -- ①駆動表
                    sql.Append("\r\n     駆動表 AS C");
                    sql.Append("\r\n     INNER JOIN");
                    sql.Append("\r\n         (SELECT");
                    sql.Append("\r\n             企業ＣＤ");
                    sql.Append("\r\n             ,得意先ＣＤ");
                    sql.Append("\r\n             ,備考1 AS 備考");
                    sql.Append("\r\n             ,1 AS ソート順");
                    sql.Append("\r\n         FROM");
                    sql.Append("\r\n             得意先マスタ");
                    sql.Append("\r\n         WHERE");
                    sql.Append("\r\n             LEN(ISNULL(備考1, '')) <> 0");
                    sql.Append("\r\n         UNION");
                    sql.Append("\r\n         SELECT");
                    sql.Append("\r\n             企業ＣＤ");
                    sql.Append("\r\n             ,得意先ＣＤ");
                    sql.Append("\r\n             ,備考2 AS 備考");
                    sql.Append("\r\n             ,2 AS ソート順");
                    sql.Append("\r\n         FROM");
                    sql.Append("\r\n             得意先マスタ");
                    sql.Append("\r\n         WHERE");
                    sql.Append("\r\n             LEN(ISNULL(備考2, '')) <> 0");
                    sql.Append("\r\n         UNION");
                    sql.Append("\r\n         SELECT");
                    sql.Append("\r\n             企業ＣＤ");
                    sql.Append("\r\n             ,得意先ＣＤ");
                    sql.Append("\r\n             ,備考3 AS 備考");
                    sql.Append("\r\n             ,3 AS ソート順");
                    sql.Append("\r\n         FROM");
                    sql.Append("\r\n             得意先マスタ");
                    sql.Append("\r\n         WHERE");
                    sql.Append("\r\n             LEN(ISNULL(備考3, '')) <> 0");
                    sql.Append("\r\n         ) AS T ON T.企業ＣＤ = C.企業ＣＤ AND T.得意先ＣＤ = C.得意先ＣＤ");
                    sql.Append("\r\n ORDER BY");
                    sql.Append("\r\n     T.得意先ＣＤ");
                    sql.Append("\r\n     ,T.ソート順");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _courseChumonCriteria.注文日;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseChumonCriteria.コースＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("配送明細表備考を取得しました。");
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
        #endregion
    }
}

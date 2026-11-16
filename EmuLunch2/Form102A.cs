using Emu.Bento.Common;
using Emu.Bento.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Emu.Bento
{
    /// <summary>
    /// 注文予定数出力
    /// </summary>
    public partial class Form102A : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMONBI_COLUMN    = 0;   // 注文日列インデックス(Excel出力まで非表示)
        private const int SYOHIN_CD_COLUMN   = 1;   // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 2;   // 商品名列インデックス
        private const int YOTEISU_COLUMN     = 3;   // 生産予定数列インデックス
        // これ以降の列は動的に追加される
        private const int COURSE_START_COLUMN = 4;  // コース名列開始列インデックス

        /// <summary>
        /// 売上文字色
        /// </summary>
        private readonly Color URIAGE_FORE_COLOR = Color.Red;

        /// <summary>
        /// 入力文字色
        /// </summary>
        private readonly Color INPUT_FORE_COLOR = Color.FromArgb(0, 100, 255);     // Blueだと濃すぎて黒と見分けが付きづらい

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 生産予定数入力検索条件
        /// </summary>
        private SeisanYoteiCriteria _seisanYoteiCriteria;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = COURSE_START_COLUMN;

        /// <summary>
        /// 列として採用するコース名の一覧（コースＣＤ、コース名）を持つデータテーブル
        /// </summary>
        private DataTable _courseDt = null;

        /// <summary>
        /// 商品を行に、コースを列に持ち、入力フラグをセルに持つデータテーブル
        /// </summary>
        private ColumnListDataTable _cldtInputFlg = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form102A()
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
        private void Form102A_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_flatbed_ico;

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
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _seisanYoteiCriteria = new SeisanYoteiCriteria();
                seisanYoteiBindingSource.DataSource = _seisanYoteiCriteria;

                // スプレッドの注文日列を非表示にする
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                // 動的に追加される列以外を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = COURSE_START_COLUMN;
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
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form102A_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F12:
                    btnQuit.PerformClick();         // 終了ボタン押下
                    break;
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
                if (_seisanYoteiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seisanYoteiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seisanYoteiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_seisanYoteiCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 生産予定一覧を表示
                // GetSeisanList(_seisanYoteiCriteria.注文日);
                GetSeisanListFromChumon();
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
                saveFileDialog1.FileName = this.Text;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = saveFileDialog1.FileName;
                    FarPoint.Excel.ExcelSaveFlags excelSaveFlags = FarPoint.Excel.ExcelSaveFlags.SaveCustomColumnHeaders;
                    if (fileName.ToLower().EndsWith("xlsx"))
                    {
                        excelSaveFlags |= FarPoint.Excel.ExcelSaveFlags.UseOOXMLFormat;
                    }

                    // 非表示にしていたスプレッドの注文日列を表示する
                    fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = true;

                    fpSpread1.SaveExcel(fileName, excelSaveFlags);

                    // 表示したスプレッドの注文日列を再び非表示にする
                    fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (EmuException ex)
            {
                // 表示したスプレッドの注文日列を再び非表示にする(メッセージボックス表示前に非表示にしたいのでfinallyだと間に合わない)
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ApplicationException)
            {
                // 表示したスプレッドの注文日列を再び非表示にする(メッセージボックス表示前に非表示にしたいのでfinallyだと間に合わない)
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                // 表示したスプレッドの注文日列を再び非表示にする(メッセージボックス表示前に非表示にしたいのでfinallyだと間に合わない)
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

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
        /// 注文から取得した生産予定一覧を表示(予測を含めない)
        /// </summary>
        private void GetSeisanListFromChumon()
        {
            try
            {
                // 表示用のDataTable
                DataTable dt = new DataTable("生産予定");
                dt.Columns.Add("日付", typeof(DateTime));
                dt.Columns.Add("コースＣＤ", typeof(int));
                dt.Columns.Add("コース名", typeof(string));
                dt.Columns.Add("商品ＣＤ", typeof(int));
                dt.Columns.Add("商品名", typeof(string));
                dt.Columns.Add("入力フラグ", typeof(int));
                dt.Columns.Add("数量", typeof(int));

                // 主キー設定(dt.Select()を使用するときに必要)
                DataColumn[] primaryKeyArray = new DataColumn[2];
                primaryKeyArray[0] = dt.Columns["コースＣＤ"];
                primaryKeyArray[1] = dt.Columns["商品ＣＤ"];
                dt.PrimaryKey = primaryKeyArray;

                // 注文日と部署に該当する注文ヘッダを取得する
                List<ChumonHeader> chumonHeaderList = GetChumoonHeaderList();

                // 注文明細を取得する
                List<ChumonMeisai> chumonMeisaiList = GetChumoonMeisaiList();

                // コース一覧を取得する
                List<Course> courseList = GetCourseList();

                // 商品を全件取得する
                List<Syohin> syohinList = Syohin.GetSyohinList().OrderBy(x => x.弁当区分)
                     .ThenBy(x => x.商品ＣＤ)
                     .ToList();

                // 表示用DataTableに行を追加する
                foreach (Syohin syohin in syohinList)
                {
                    if(syohin.弁当区分 != (int)Consts.BentoKubun.SubDish && syohin.弁当区分 != (int)Consts.BentoKubun.MainDish)
                    {
                        continue;
                    }
                    foreach (Course course in courseList)
                    {
                        DataRow dr = dt.NewRow();
                        dr["日付"] = _seisanYoteiCriteria.注文日;
                        dr["コースＣＤ"] = course.コースＣＤ;
                        dr["コース名"] = course.コース名;
                        dr["商品ＣＤ"] = syohin.商品ＣＤ;
                        dr["商品名"] = syohin.商品名;
                        dr["入力フラグ"] = 0;
                        dr["数量"] = DBNull.Value;
                        dt.Rows.Add(dr);
                    }
                }
                
                // 注文に含まれる商品をコース毎に集計する
                foreach (Course course in courseList)
                {
                    // コースに当てはまる注文ヘッダを取得する
                    List<ChumonHeader> chumonHeaderListInCourse = chumonHeaderList.FindAll(listContent => listContent.コースＣＤ == course.コースＣＤ);

                    foreach (ChumonHeader chumonHeader in chumonHeaderListInCourse)
                    {
                        // 得意先の注文明細を取得する
                        List<ChumonMeisai> chumonMeisaiListByTokuisaki = chumonMeisaiList.FindAll(listContent => listContent.得意先ＣＤ == chumonHeader.得意先ＣＤ);

                        // 表示用DataTableの対応する箇所に注文数を加算する
                        foreach (ChumonMeisai chumonMeisai in chumonMeisaiListByTokuisaki)
                        {
                            // 注文した商品を取得する
                            Syohin syohin = syohinList.Find(listContent => listContent.商品ＣＤ == chumonMeisai.商品ＣＤ);

                            // 注文明細の商品がマスタに存在しない場合はcontinue
                            if (syohin == null)
                            {
                                continue;
                            }

                            // 商品を構成する部品が設定されていない場合 (注文された商品の主食CD、副食CDがどちらもnull)
                            if (syohin.主食ＣＤ == null && syohin.副食ＣＤ == null)
                            {
                                // 商品の弁当区分が主食でも副食でもない場合は何もしない
                                if (syohin.弁当区分 != (int)Consts.BentoKubun.MainDish && syohin.弁当区分 != (int)Consts.BentoKubun.SubDish)
                                {
                                    continue;
                                }

                                // 表示用DataTableからコースCDと商品CDに当てはまる行を取得する
                                DataRow[] dr = dt.Select(string.Format("コースＣＤ = {0} AND 商品ＣＤ = {1}", course.コースＣＤ, syohin.商品ＣＤ));

                                // 加算
                                int amount = dr[0]["数量"] != DBNull.Value ? (int)dr[0]["数量"] : 0;
                                dr[0]["数量"] = amount + chumonMeisai.商品数量;
                            }
                            // 部品を注文数文加算する
                            else
                            {
                                if (syohin.主食ＣＤ != null)
                                {
                                    // 主食の情報を取得
                                    Syohin syusyoku = syohinList.Find(listContent => listContent.商品ＣＤ == syohin.主食ＣＤ && listContent.弁当区分 == (int)Consts.BentoKubun.MainDish);

                                    if (syusyoku != null)
                                    {
                                        DataRow[] dr = dt.Select(string.Format("コースＣＤ = {0} AND 商品ＣＤ = {1}", course.コースＣＤ, syusyoku.商品ＣＤ));

                                        int amount = dr[0]["数量"] != DBNull.Value ? (int)dr[0]["数量"] : 0;
                                        dr[0]["数量"] = amount + chumonMeisai.商品数量;
                                    }
                                }

                                if (syohin.副食ＣＤ != null)
                                {
                                    // 副食の情報を取得
                                    Syohin fukusyoku = syohinList.Find(listContent => listContent.商品ＣＤ == syohin.副食ＣＤ && listContent.弁当区分 == (int)Consts.BentoKubun.SubDish);

                                    if (fukusyoku != null)
                                    {
                                        DataRow[] dr = dt.Select(string.Format("コースＣＤ = {0} AND 商品ＣＤ = {1}", course.コースＣＤ, fukusyoku.商品ＣＤ));

                                        int amount = dr[0]["数量"] != DBNull.Value ? (int)dr[0]["数量"] : 0;
                                        dr[0]["数量"] = amount + chumonMeisai.商品数量;
                                    }
                                }
                            }
                        }
                    }
                }

                // 表示主処理を呼び出す
                GetSeisanListMain(dt);
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
        /// 生産予定一覧を表示（主処理）
        /// </summary>
        /// <param name="dt">データ本体</param>
        private void GetSeisanListMain(DataTable dt)
        {
            try
            {
                // 列として採用するコース名の一覧をデータテーブルとして取得する
                _courseDt = GetCourseTable();

                // コース名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_courseDt, "コースＣＤ", "コース名");

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("生産予定数入力");
                // コース名列を作成
                cldt.AddColumns(_courseDt, "コース名", "コースＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["日付"]);
                cldt.AddFixedColumn(dt.Columns["商品ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["商品名"]);
                // インポート
                cldt.Import(dt, "コースＣＤ", "数量");

                // 同様に入力フラグを持つデータテーブルを作成する
                _cldtInputFlg = new ColumnListDataTable();
                // コース名列を作成
                _cldtInputFlg.AddColumns(_courseDt, "コース名", "コースＣＤ", typeof(bool));
                // 固定列を作成
                _cldtInputFlg.AddFixedColumn(dt.Columns["商品ＣＤ"]);
                // インポート
                _cldtInputFlg.Import(dt, "コースＣＤ", "入力フラグ");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "生産予定数入力";

                fpSpread1_Sheet1.BindDataColumn(CHUMONBI_COLUMN,    "日付");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");

                // 商品名を列名としてバインド
                int columnIndex = COURSE_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // 文字色と読み取り専用属性を設定
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    // 生産予定数の数式を設定する
                    FarPoint.Win.Spread.Cell firstCell = fpSpread1_Sheet1.Cells[rowIndex, COURSE_START_COLUMN];
                    FarPoint.Win.Spread.Cell lastCell = fpSpread1_Sheet1.Cells[rowIndex, MAX_COLUMN_INDEX];
                    fpSpread1_Sheet1.Cells[rowIndex, YOTEISU_COLUMN].Formula = string.Format("SUM({0}:{1})", firstCell.ToString(), lastCell.ToString());

                    for (columnIndex = COURSE_START_COLUMN; columnIndex <= MAX_COLUMN_INDEX; columnIndex++)
                    {
                        fpSpread1_Sheet1.Cells[rowIndex, columnIndex].ForeColor = URIAGE_FORE_COLOR;

                        // 読み取り専用にする
                        FarPoint.Win.Spread.CellType.EmptyCellType cellType = new FarPoint.Win.Spread.CellType.EmptyCellType();
                        //cellType.ReadOnly = true; //無くてもいい
                        fpSpread1_Sheet1.Cells[rowIndex, columnIndex].CellType = cellType;
                    }
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, COURSE_START_COLUMN);
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
        /// スプレッドの列として採用するコース名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetCourseTable()
        {
            DataTable dt = new DataTable("コース一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     C.コースＣＤ");
                    sql.AppendLine("     ,CM.コース名");
                    sql.AppendLine(" FROM");
                    // -- コースマスタから
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         コースマスタ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND コース区分 = @コース区分");
                    sql.AppendLine("         AND ISNULL(工場フラグ, 0) = 0");
                    sql.AppendLine("         AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("     UNION");
                    // -- 生産予定から
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         生産予定");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND 喫食日 = @注文日");
                    sql.AppendLine("     UNION");
                    // -- 売上から
                    sql.AppendLine("     SELECT");
                    sql.AppendLine("         H.コースＣＤ");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ AS H");
                    sql.AppendLine("         LEFT JOIN 祝日マスタ AS SK ON SK.企業ＣＤ = H.企業ＣＤ AND SK.対象日 = H.売上日");
                    sql.AppendLine("         INNER JOIN コースマスタ AS C");
                    sql.AppendLine("             ON  C.企業ＣＤ   = H.企業ＣＤ");
                    sql.AppendLine("             AND C.コースＣＤ = H.コースＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");
                    sql.AppendLine("         AND H.コースＣＤ IS NOT NULL");
                    sql.AppendLine("         AND H.売上日 >= @期間開始日");
                    sql.AppendLine("         AND H.売上日 <= @期間終了日");
                    sql.AppendLine("         AND DATEPART(WEEKDAY, H.売上日) = DATEPART(WEEKDAY, @期間終了日)");
                    sql.AppendLine("         AND SK.対象日 IS NULL");
                    sql.AppendLine("         AND C.コース区分 = @コース区分");
                    sql.AppendLine("         AND ISNULL(C.工場フラグ, 0) = 0");
                    sql.AppendLine("         AND ISNULL(C.無効フラグ, 0) = 0");
                    sql.AppendLine("     ) AS C");
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ");

                    if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                    {
                        // -- コース別グループと結合
                        sql.AppendLine("     INNER JOIN コース別グループマスタ AS CG ON ");
                        sql.AppendLine("         CG.企業ＣＤ = @企業ＣＤ ");
                        sql.AppendLine("         AND CG.部署ＣＤ = @部署ＣＤ");
                        sql.AppendLine("         AND CG.グループ種別ＣＤ = @グループ種別ＣＤ");
                        sql.AppendLine("         AND CG.コースＣＤ = C.コースＣＤ");

                        cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                    }

                    sql.AppendLine(" WHERE CM.コース区分 = @コース区分");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     C.コースＣＤ");
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                    cmd.Parameters.Add("@期間開始日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間開始日;
                    cmd.Parameters.Add("@期間終了日", SqlDbType.Date).Value = _seisanYoteiCriteria.期間終了日;
                    cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = Course.GetCourseKubun(_seisanYoteiCriteria.注文日);

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("コース一覧を取得しました。");
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
        /// コース名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="courseDt">コース名一覧データテーブル</param>
        /// <param name="labelColumnCD">コースCDが格納されている列の名前</param>
        /// <param name="labelColumnName">コース名が格納されている列の名前</param>
        private void SetSpreadColumn(DataTable courseDt, string labelColumnCD, string labelColumnName)
        {
            try
            {
                // 商品名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = COURSE_START_COLUMN;

                // 今回追加する商品名の列数をセット
                fpSpread1_Sheet1.ColumnCount = COURSE_START_COLUMN + courseDt.Rows.Count;

                // 固定列に続いて、コース名の列の属性をセット
                int columnIndex = COURSE_START_COLUMN;
                foreach (DataRow dr in courseDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = 0D;

                    fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, columnIndex).Value = dr[labelColumnCD].ToString();
                    fpSpread1_Sheet1.ColumnHeader.Cells.Get(1, columnIndex).Value = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
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
        /// 指定された注文日と部署に該当する注文ヘッダを取得する(実際の注文のみ)
        /// </summary>
        /// <returns></returns>
        private List<ChumonHeader> GetChumoonHeaderList()
        {
            List<ChumonHeader> chumonHeaderList = new List<ChumonHeader>();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.注文区分" +
                         "\r\n     ,A.喫食日" +
                         "\r\n     ,A.得意先ＣＤ" +
                         "\r\n     ,A.部署ＣＤ" +
                         "\r\n     ,A.配送日" +
                         "\r\n     ,A.得意先名" +
                         "\r\n     ,A.担当者ＣＤ" +
                         "\r\n     ,A3.担当者名 AS 担当者名" +
                         "\r\n     ,A.コースＣＤ" +
                         "\r\n     ,A.配送先ＣＤ" +
                         "\r\n     ,A.税区分" +
                         "\r\n     ,A.丸め処理区分" +
                         "\r\n     ,A.備考1" +
                         "\r\n     ,A.備考2" +
                         "\r\n     ,A.備考3" +
                         "\r\n     ,A.備考4" +
                         "\r\n     ,A.備考5" +
                         "\r\n     ,A.納品書伝票番号" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     注文ヘッダ AS A" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = @企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ" +
                         "\r\n WHERE" +
                         "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                         "\r\n AND A.注文区分   = @注文区分" +
                         "\r\n AND A.喫食日     = @喫食日" +
                         "\r\n AND A.部署ＣＤ   = @部署ＣＤ";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon; //実際の注文のみ
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("注文ヘッダ");
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    chumonHeaderList.Add(new ChumonHeader(dr));
                }

                return chumonHeaderList;
            }
        }

        /// <summary>
        /// 指定された注文日と部署に該当する注文明細を取得する(実際の注文のみ)
        /// </summary>
        /// <returns></returns>
        private List<ChumonMeisai> GetChumoonMeisaiList()
        {
            List<ChumonMeisai> chumonMeisaiList = new List<ChumonMeisai>();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                string sql = "\r\n SELECT" +
                         "\r\n     A.企業ＣＤ" +
                         "\r\n     ,A.注文区分" +
                         "\r\n     ,A.喫食日" +
                         "\r\n     ,A.得意先ＣＤ" +
                         "\r\n     ,A.商品ＣＤ" +
                         "\r\n     ,A.部署ＣＤ" +
                         "\r\n     ,A.明細番号" +
                         "\r\n     ,A.製造日" +
                         "\r\n     ,A.商品グループＣＤ" +
                         "\r\n     ,A.売掛現金区分" +
                         "\r\n     ,A.商品単価" +
                         "\r\n     ,A.商品数量" +
                         "\r\n     ,A.商品金額合計" +
                         "\r\n     ,A.消費税額" +
                         "\r\n     ,A.商品金額合計税込" +
                         "\r\n     ,A.計算税率" +
                         "\r\n     ,A.作成担当者ＣＤ" +
                         "\r\n     ,A1.担当者名 AS 作成担当者名" +
                         "\r\n     ,A.作成日" +
                         "\r\n     ,A.修正担当者ＣＤ" +
                         "\r\n     ,A2.担当者名 AS 修正担当者名" +
                         "\r\n     ,A.修正日" +
                         "\r\n FROM" +
                         "\r\n     注文明細 AS A" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                         "\r\n     LEFT JOIN 商品マスタ AS M ON  M.企業ＣＤ = @企業ＣＤ AND A.商品ＣＤ = M.商品ＣＤ" +
                         "\r\n WHERE" +
                         "\r\n     A.企業ＣＤ   = @企業ＣＤ" +
                         "\r\n AND A.注文区分   = @注文区分" +
                         "\r\n AND A.喫食日     = @喫食日" +
                         "\r\n AND A.部署ＣＤ   = @部署ＣＤ" +
                         "\r\n AND M.弁当区分 IN (0,1,2)";  //弁当or主食or副食のみ

                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@注文区分", SqlDbType.Int).Value = (int)Consts.ChumonKubun.Chuumon; //実際の注文のみ
                cmd.Parameters.Add("@喫食日", SqlDbType.Date).Value = _seisanYoteiCriteria.注文日;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("注文明細");
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    chumonMeisaiList.Add(new ChumonMeisai(dr));
                }

                return chumonMeisaiList;
            }
        }

        /// <summary>
        /// コースの一覧を取得する
        /// </summary>
        /// <returns></returns>
        private List<Course> GetCourseList()
        {
            List<Course> courseList = new List<Course>();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                string sql = "\r\n SELECT" +
                             "\r\n     A.企業ＣＤ" +
                             "\r\n     ,A.コースＣＤ" +
                             "\r\n     ,A.コース区分" +
                             "\r\n     ,VC.コース区分名" +
                             "\r\n     ,A.コース名" +
                             "\r\n     ,A.部署ＣＤ" +
                             "\r\n     ,B.部署名" +
                             "\r\n     ,A.担当者ＣＤ" +
                             "\r\n     ,A3.担当者名" +
                             "\r\n     ,A.工場フラグ" +
                             "\r\n     ,A.無効フラグ" +
                             "\r\n     ,A.作成担当者ＣＤ" +
                             "\r\n     ,A1.担当者名 AS 作成担当者名" +
                             "\r\n     ,A.作成日" +
                             "\r\n     ,A.修正担当者ＣＤ" +
                             "\r\n     ,A2.担当者名 AS 修正担当者名" +
                             "\r\n     ,A.修正日" +
                             "\r\n FROM" +
                             "\r\n     コースマスタ AS A" +
                             "\r\n     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ AND ISNULL(B.無効フラグ, 0) = 0" +
                             "\r\n     LEFT JOIN V_コース区分 AS VC ON VC.コース区分CD = A.コース区分" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A1 ON A.企業ＣＤ = A1.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A2 ON A.企業ＣＤ = A2.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ" +
                             "\r\n     LEFT JOIN 担当者マスタ AS A3 ON A.企業ＣＤ = A3.企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ";

                if (_seisanYoteiCriteria.グループ種別ＣＤ != null && _seisanYoteiCriteria.グループ種別ＣＤ != -1)
                {
                    sql += "    INNER JOIN コース別グループマスタ AS CG ON CG.企業ＣＤ = @企業ＣＤ" +
                        "AND CG.部署ＣＤ = @部署ＣＤ " +
                        "AND CG.グループ種別ＣＤ = @グループ種別ＣＤ " +
                        "AND CG.コースＣＤ = A.コースＣＤ";
                    cmd.Parameters.Add("@グループ種別ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.グループ種別ＣＤ;
                }

                sql += "\r\n WHERE" +
                             "\r\n     A.企業ＣＤ = @企業ＣＤ" +
                             "\r\n AND A.部署ＣＤ = @部署ＣＤ";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _seisanYoteiCriteria.部署ＣＤ;

                log.Debug(cmd.CommandTextWithParameters());

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("コースマスタ");
                adp.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    courseList.Add(new Course(dr));
                }

                return courseList;
            }
        }

        #endregion
    }
}

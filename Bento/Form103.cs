using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 生産予定総数一覧
    /// </summary>
    public partial class Form103 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int CHUMONBI_COLUMN    = 0;       // 注文日列インデックス(Excel出力まで非表示)
        private const int SYOHIN_CD_COLUMN   = 1;       // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 2;       // 商品名列インデックス
        private const int TOTAL_COLUMN       = 3;       // 総生産数列インデックス
        // これ以降の列は動的に追加される
        private const int BUSYO_START_COLUMN = 4;       // 部署名開始列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 生産予定総数一覧検索条件
        /// </summary>
        private SeisanYoteiTotalCriteria _seisanYoteiTotalCriteria;

        // 不定列追加後の最後の列インデックス
        private int MAX_COLUMN_INDEX = BUSYO_START_COLUMN;

        /// <summary>
        /// 列として採用する部署名の一覧（部署ＣＤ、部署名）を持つデータテーブル
        /// </summary>
        private DataTable _busyoDt = null;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form103()
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
        private void Form103_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_flatbed_ico;

                lblCount.Text = "";

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
                _seisanYoteiTotalCriteria = new SeisanYoteiTotalCriteria();
                seisanYoteiTotalCriteriaBindingSource.DataSource = _seisanYoteiTotalCriteria;

                // スプレッドの注文日列を非表示にする
                fpSpread1_Sheet1.Columns[CHUMONBI_COLUMN].Visible = false;

                // 動的に追加される列以外を固定列にする
                fpSpread1_Sheet1.FrozenColumnCount = BUSYO_START_COLUMN;
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
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form103_KeyDown(object sender, KeyEventArgs e)
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
                if (_seisanYoteiTotalCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_seisanYoteiTotalCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _seisanYoteiTotalCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 注文日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_seisanYoteiTotalCriteria.注文日 > warnDate)
                {
                    if (MessageBox.Show(this, "注文日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        bindingSource1.DataSource = null;
                        return;
                    }
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                //*************************************
                // 部署の一覧名を列として追加し、
                // 生産予定にある商品を行とする
                // データそのものは生産予定から取得する
                //*************************************

                // 列として採用する部署名の一覧をデータテーブルとして取得する
                _busyoDt = GetBusyoTable();

                // 商品名一覧のデータテーブルを基にして、スプレッドに列を追加する
                SetSpreadColumn(_busyoDt, "部署名");

                // データ本体を取得
                DataTable dt = new DataTable("生産予定");
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     Y.喫食日" +
                                      "\r\n     ,Y.商品ＣＤ" +
                                      "\r\n     ,Y.部署ＣＤ" +
                                      "\r\n     ,S.商品名" +
                                      "\r\n     ,SUM(Y.数量) AS 数量" +
                                      "\r\n FROM" +
                                      "\r\n     生産予定 AS Y" +
                                      "\r\n     LEFT JOIN 商品マスタ AS S" +
                                      "\r\n         ON Y.企業ＣＤ = S.企業ＣＤ" +
                                      "\r\n         AND Y.商品ＣＤ = S.商品ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     Y.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n     AND Y.喫食日 = @注文日" +
                                      "\r\n GROUP BY" +
                                      "\r\n     Y.喫食日" +
                                      "\r\n     ,Y.商品ＣＤ" +
                                      "\r\n     ,Y.部署ＣＤ" +
                                      "\r\n     ,S.商品名" +
                                      "\r\n ORDER BY" +
                                      "\r\n     Y.商品ＣＤ" +
                                      "\r\n     ,Y.部署ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiTotalCriteria.注文日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("生産予定総数一覧リストを検索しました。");
                }

                // 列を指定したデータテーブルを作成
                ColumnListDataTable cldt = new ColumnListDataTable("生産予定総数一覧");
                // 商品名列を作成
                cldt.AddColumns(_busyoDt, "部署名", "部署ＣＤ", typeof(int));
                // 固定列を作成
                cldt.AddFixedColumn(dt.Columns["喫食日"]);
                cldt.AddFixedColumn(dt.Columns["商品ＣＤ"]);
                cldt.AddFixedColumn(dt.Columns["商品名"]);
                // インポート
                cldt.Import(dt, "部署ＣＤ", "数量");

                DataSet ds = new DataSet();
                ds.Tables.Add(cldt);

                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "生産予定総数一覧";

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", bindingSource1.Count);

                fpSpread1_Sheet1.BindDataColumn(CHUMONBI_COLUMN,    "喫食日");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");

                // 商品名を列名としてバインド
                int columnIndex = BUSYO_START_COLUMN;
                foreach (string columnName in cldt.ColumnNames)
                {
                    fpSpread1_Sheet1.BindDataColumn(columnIndex, columnName);
                    columnIndex++;
                }
                MAX_COLUMN_INDEX = columnIndex - 1;

                // 総生産数列に数式を設定する
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    FarPoint.Win.Spread.Cell firstCell = fpSpread1_Sheet1.Cells[rowIndex, BUSYO_START_COLUMN];
                    FarPoint.Win.Spread.Cell lastCell = fpSpread1_Sheet1.Cells[rowIndex, MAX_COLUMN_INDEX];
                    fpSpread1_Sheet1.Cells[rowIndex, TOTAL_COLUMN].Formula = string.Format("SUM({0}:{1})", firstCell.ToString(), lastCell.ToString());
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.SetActiveCell(0, BUSYO_START_COLUMN);
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
            catch (BentoException ex)
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
        /// 列として採用する部署名の一覧をデータテーブルとして取得する
        /// </summary>
        /// <returns></returns>
        private DataTable GetBusyoTable()
        {
            DataTable dt = new DataTable("部署一覧");
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     A.部署ＣＤ" +
                                      "\r\n     ,B.部署名" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT DISTINCT" +
                                      "\r\n         企業ＣＤ" +
                                      "\r\n         ,部署ＣＤ" +
                                      "\r\n     FROM" +
                                      "\r\n         生産予定" +
                                      "\r\n     WHERE" +
                                      "\r\n         企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         AND 喫食日 = @注文日" +
                                      "\r\n     ) AS A" +
                                      "\r\n     INNER JOIN 部署マスタ AS B" +
                                      "\r\n         ON B.企業ＣＤ = A.企業ＣＤ" +
                                      "\r\n         AND B.部署ＣＤ = A.部署ＣＤ" +
                                      "\r\n ORDER BY" +
                                      "\r\n     A.部署ＣＤ";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@注文日", SqlDbType.Date).Value = _seisanYoteiTotalCriteria.注文日;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    adp.Fill(dt);

                    log.Info("部署一覧を取得しました。");
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
        /// 部署名一覧のデータテーブルを基にして、スプレッドに列を追加する
        /// </summary>
        /// <param name="dt"></param>
        private void SetSpreadColumn(DataTable syohinDt, string labelColumnName)
        {
            try
            {
                // 部署名列を削除（リセット）
                fpSpread1_Sheet1.ColumnCount = BUSYO_START_COLUMN;

                // 今回追加する部署名の列数をセット
                fpSpread1_Sheet1.ColumnCount = BUSYO_START_COLUMN + syohinDt.Rows.Count;

                // 固定列に続いて、部署名の列の属性をセット
                int columnIndex = BUSYO_START_COLUMN;
                foreach (DataRow dr in syohinDt.Rows)
                {
                    FarPoint.Win.Spread.CellType.NumberCellType numberCellType = new FarPoint.Win.Spread.CellType.NumberCellType();
                    numberCellType.DecimalPlaces = 0;
                    numberCellType.EnableSubEditor = false;
                    numberCellType.MaximumValue = 99999D;
                    numberCellType.MinimumValue = 0D;
                    numberCellType.ReadOnly = true;
                    numberCellType.Separator = ",";
                    numberCellType.ShowSeparator = true;
                    numberCellType.Static = true;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Label = dr[labelColumnName].ToString();
                    fpSpread1_Sheet1.Columns.Get(columnIndex).Width = 100F;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).CellType = numberCellType;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).ImeMode = System.Windows.Forms.ImeMode.Disable;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorPosition = FarPoint.Win.Spread.NoteIndicatorPosition.TopLeft;
                    fpSpread1_Sheet1.Columns.Get(columnIndex).NoteIndicatorSize = new System.Drawing.Size(8, 8);
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

        #endregion
    }
}

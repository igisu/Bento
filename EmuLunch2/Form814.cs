using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 月次商品別売上一覧
    /// </summary>
    public partial class Form814 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_CD_COLUMN    = 0;    // 部署ＣＤ列インデックス
        private const int BUSYO_NAME_COLUMN  = 1;    // 部署名列インデックス
        private const int SYOHIN_CD_COLUMN   = 2;    // 商品ＣＤ列インデックス
        private const int SYOHIN_NAME_COLUMN = 3;    // 商品名列インデックス
        private const int QUANTITY_COLUMN    = 4;    // 数量列インデックス
        private const int AMOUNT_COLUMN      = 5;    // 金額列インデックス
        private const int AVERAGE_COLUMN     = 6;    // 平均単価列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 月次商品別売上一覧検索条件
        /// </summary>
        private GetsujiSyohinListCriteria _getsujiSyohinListCriteria;

        /// <summary>
        /// 月次商品別売上一覧検索条件検索実行時スナップショット
        /// </summary>
        private GetsujiSyohinListCriteria _getsujiSyohinListCriteriaSnapShot;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form814()
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
        private void Form814_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _getsujiSyohinListCriteria = new GetsujiSyohinListCriteria();
                getsujiSyohinListCriteriaBindingSource.DataSource = _getsujiSyohinListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "部署CD", "商品CD", "数量", "金額", "平均単価" },
                    new string[] { "U.部署ＣＤ {0}, U.商品ＣＤ",
                                   "U.商品ＣＤ {0}, U.部署ＣＤ",
                                   "U.数量 {0}, U.部署ＣＤ, U.商品ＣＤ",
                                   "U.金額 {0}, U.部署ＣＤ, U.商品ＣＤ",
                                   "U.平均単価 {0}, U.部署ＣＤ, U.商品ＣＤ"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });
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
        private void Form814_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

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
        }

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form814_Shown(object sender, EventArgs e)
        {
            // 商品ＣＤにフォーカス
            txtSyohinCD.Focus();
        }

        /// <summary>
        /// 商品ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSyohinCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSyohinCD.PerformClick();
            }
        }

        /// <summary>
        /// 商品参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyohinCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCD_FormClosed);
                formSyohinDlg.ShowDialog();
                if (formSyohinDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
                    errorProvider1.SetError(txtSyohinCD, null);  // エラーを消す

                    Syohin syohin = form.SelectedSyohin;
                    _getsujiSyohinListCriteria.商品ＣＤ = syohin.商品ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtSyohinCD);
                    ReadBinding(lblSyohinName);
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
        /// 部署ごとに集計チェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSummaryByBusyo_CheckedChanged(object sender, EventArgs e)
        {
            // 部署コンボの活性を切り替える
            cboBusyo.Enabled = chkSummaryByBusyo.Checked;
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_getsujiSyohinListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_getsujiSyohinListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _getsujiSyohinListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    if ((int)cboListMax.SelectedValue < 0)
                    {
                        sql.AppendLine("SELECT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     U.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,U.商品ＣＤ");
                    sql.AppendLine("     ,S.商品名");
                    sql.AppendLine("     ,U.数量");
                    sql.AppendLine("     ,U.金額");
                    sql.AppendLine("     ,FLOOR(U.平均単価) AS 平均単価");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         企業ＣＤ");
                    if (_getsujiSyohinListCriteria.部署ごとに集計 == true)
                    {
                        sql.AppendLine("         ,部署ＣＤ");           // -- 部署ごとに集計する場合はこちら
                    }
                    else
                    {
                        sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                    }
                    sql.AppendLine("         ,商品ＣＤ");
                    sql.AppendLine("         ,SUM(今月数量) AS 数量");
                    sql.AppendLine("         ,SUM(今月売上額) AS 金額");
                    sql.AppendLine("         ,CASE WHEN SUM(今月数量) = 0 THEN 0 ELSE SUM(今月売上額) / SUM(今月数量) END AS 平均単価");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         月次売上集計");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("         AND 集計年 = @集計年");
                    sql.AppendLine("         AND 集計月 = @集計月");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@集計年", SqlDbType.Int).Value = _getsujiSyohinListCriteria.集計年;
                    cmd.Parameters.Add("@集計月", SqlDbType.Int).Value = _getsujiSyohinListCriteria.集計月;

                    if (_getsujiSyohinListCriteria.部署ごとに集計 == true && _getsujiSyohinListCriteria.部署ＣＤ != null && _getsujiSyohinListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");    // -- 部署ごとに集計する場合のみ有効
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _getsujiSyohinListCriteria.部署ＣＤ;
                    }

                    if (_getsujiSyohinListCriteria.商品ＣＤ != null)
                    {
                        sql.AppendLine("         AND 商品ＣＤ = @商品ＣＤ");
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _getsujiSyohinListCriteria.商品ＣＤ;
                    }

                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         企業ＣＤ");

                    if (_getsujiSyohinListCriteria.部署ごとに集計 == true)
                    {
                        sql.AppendLine("         ,部署ＣＤ");
                    }

                    sql.AppendLine("         ,商品ＣＤ");
                    sql.AppendLine("     ) AS U");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B");
                    sql.AppendLine("         ON B.企業ＣＤ = U.企業ＣＤ");
                    sql.AppendLine("         AND B.部署ＣＤ = U.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS S");
                    sql.AppendLine("         ON S.企業ＣＤ = U.企業ＣＤ");
                    sql.AppendLine("         AND S.商品ＣＤ = U.商品ＣＤ");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("月次商品別売上一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 合計行を追加する
                    dt.Rows.Add(dt.NewRow());

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "月次商品別売上一覧";

                    log.Info("月次商品別売上一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN,    "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,  "部署名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");
                fpSpread1_Sheet1.BindDataColumn(QUANTITY_COLUMN,    "数量");
                fpSpread1_Sheet1.BindDataColumn(AMOUNT_COLUMN,      "金額");
                fpSpread1_Sheet1.BindDataColumn(AVERAGE_COLUMN,     "平均単価");

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, SYOHIN_NAME_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = QUANTITY_COLUMN; columnIndex <= AVERAGE_COLUMN; columnIndex++)
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
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                // 検索実行時の検索条件を保存（Excel出力時に使用する）
                _getsujiSyohinListCriteriaSnapShot = new GetsujiSyohinListCriteria(_getsujiSyohinListCriteria);
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
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _getsujiSyohinListCriteria = new GetsujiSyohinListCriteria();
                getsujiSyohinListCriteriaBindingSource.DataSource = _getsujiSyohinListCriteria;

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);
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
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                if (_getsujiSyohinListCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string busyoName = "";
                if (_getsujiSyohinListCriteriaSnapShot.部署ごとに集計 == true && _getsujiSyohinListCriteriaSnapShot.部署ＣＤ != null && _getsujiSyohinListCriteriaSnapShot.部署ＣＤ != -1)
                {
                    Busyo busyo = Busyo.GetBusyo((int)_getsujiSyohinListCriteriaSnapShot.部署ＣＤ);
                    if (busyo != null)
                    {
                        busyoName = string.Format("({0})", busyo.部署名);
                    }
                }

                saveFileDialog1.FileName = string.Format("{0}_{1}年{2}月{3}",  this.Text,_getsujiSyohinListCriteriaSnapShot.集計年, _getsujiSyohinListCriteriaSnapShot.集計月, busyoName);
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
            catch (EmuException ex)
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
        #endregion
    }
}

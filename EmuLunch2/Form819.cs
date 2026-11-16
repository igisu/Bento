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
    /// クレカ利用明細
    /// </summary>
    public partial class Form819 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int SEIKYUSAKI_CD_COLUMN = 0;    // 請求先ＣＤ列インデックス
        private const int SEIKYUSAKI_NAME_COLUMN = 1;    // 請求先名列インデックス
        private const int CARDNO_COLUMN = 2;    // カード番号列インデックス
        private const int SEIKYU_KIKAN_COLUMN = 3;    // 利用期間列インデックス
        private const int SEIKYU_AMOUNT_COLUMN = 4;    // ご請求金額列インデックス

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// クレカ利用明細一覧検索条件
        /// </summary>
        private FRUseDetailListCriteria _frUseDetailListCriteria;

        /// <summary>
        /// クレカ利用明細一覧検索条件検索実行時スナップショット
        /// </summary>
        private FRUseDetailListCriteria _frUseDetailListCriteriaSnapShot;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form819()
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
        private void Form819_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求日", "利用者CD" },
                    new string[] { "SD.請求日 {0}, SD.請求先ＣＤ",
                                   "SD.請求先ＣＤ {0}"});

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _frUseDetailListCriteria = new FRUseDetailListCriteria();
                frUseDetailListCriteriaBindingSource.DataSource = _frUseDetailListCriteria;
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
        private void Form819_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F10:
                        btnPrint.PerformClick();        // 印刷ボタン押下
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
        private void Form819_Shown(object sender, EventArgs e)
        {
            // 指定年にフォーカス
            txtYear.Focus();
        }

        /// <summary>
        /// 請求先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSeikyusakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSeikyusakiCD.PerformClick();
            }
        }

        /// <summary>
        /// 請求先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyusakiCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900SeikyusakiCD_FormClosed);
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
        /// 得意先選択ダイアログクローズ（請求先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900SeikyusakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSeikyusakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    _frUseDetailListCriteria.請求先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtSeikyusakiCD);
                    ReadBinding(lblSeikyusakiName);
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_frUseDetailListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_frUseDetailListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _frUseDetailListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 検索
                Search();
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
                _frUseDetailListCriteria = new FRUseDetailListCriteria();
                frUseDetailListCriteriaBindingSource.DataSource = _frUseDetailListCriteria;

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
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件をデータソースに反映
                // 　(検索条件入力して、すぐに印刷ボタンを押した場合に、検索条件が確定されない)
                WriteBinding(cboBusyo);
                //WriteBinding(txtYear);
                //WriteBinding(txtMonth);
                //WriteBinding(txtSeikyusakiCD);

                DataTable dt = GetPrint();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "印刷するデータがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Report819A report = new Report819A();
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                report.DataSource = dt;

                // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                PrintReport(report, printSettings);
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
                if (_frUseDetailListCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

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

        /// <summary>
        /// 検索
        /// </summary>
        /// <exception cref="EmuException"></exception>
        private void Search()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("   SD.請求番号, ");
                    sql.AppendLine("   SD.請求先ＣＤ, ");
                    sql.AppendLine("   SD.請求先名, ");
                    sql.AppendLine("   SD.前回請求日, ");
                    sql.AppendLine("   SD.今回請求額, ");
                    sql.AppendLine("   CONCAT( ");
                    sql.AppendLine("     FORMAT(DATEADD(day, 1, SD.前回請求日), 'yyyy年M月d日'), ");
                    sql.AppendLine("     ' ～ ', ");
                    sql.AppendLine("     FORMAT(SD.請求日, 'yyyy年M月d日') ");
                    sql.AppendLine("   ) AS 請求期間, ");
                    sql.AppendLine("   CONCAT('****-****-****-', TSM.クレカ番号下4桁) AS クレカ番号 ");
                    sql.AppendLine(" FROM ");
                    sql.AppendLine("   ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       A.企業ＣＤ, ");
                    sql.AppendLine("       A.請求番号, ");
                    sql.AppendLine("       A.請求日, ");
                    sql.AppendLine("       A.部署ＣＤ, ");
                    sql.AppendLine("       A.請求先ＣＤ, ");
                    sql.AppendLine("       A.請求先名, ");
                    sql.AppendLine("       A.今回請求額, ");
                    sql.AppendLine("       ( ");
                    sql.AppendLine("         SELECT ");
                    sql.AppendLine("           MAX(P.請求日) ");
                    sql.AppendLine("         FROM ");
                    sql.AppendLine("           請求合計 AS P ");
                    sql.AppendLine("         WHERE ");
                    sql.AppendLine("           P.企業ＣＤ = A.企業ＣＤ ");
                    sql.AppendLine("           AND P.請求先ＣＤ = A.請求先ＣＤ ");
                    sql.AppendLine("           AND P.請求日 < A.請求日 ");
                    sql.AppendLine("       ) AS 前回請求日 ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       請求合計 AS A ");
                    sql.AppendLine("     WHERE ");
                    sql.AppendLine("       A.企業ＣＤ = @企業ＣＤ ");
                    sql.AppendLine("       AND A.部署ＣＤ = @部署ＣＤ ");
                    sql.AppendLine("       AND A.FR支払フラグ = 1 ");
                    sql.AppendLine("       AND A.クレカ決済完了フラグ = 1 ");
                    sql.AppendLine("       AND A.クレカ決済エラー区分 = 0 ");
                    sql.AppendLine("       AND A.請求日 >= @請求日From ");
                    sql.AppendLine("       AND A.請求日 <= @請求日To ");
                    sql.AppendLine("   ) AS SD ");
                    sql.AppendLine("   LEFT JOIN 得意先マスタ AS TSM ON TSM.企業ＣＤ = SD.企業ＣＤ AND TSM.得意先ＣＤ = SD.請求先ＣＤ ");
                    sql.AppendLine(" WHERE ");
                    sql.AppendLine("   1 = 1 ");

                    if (_frUseDetailListCriteria.請求先ＣＤ != null && _frUseDetailListCriteria.請求先ＣＤ != -1)
                    {
                        sql.AppendLine("   AND SD.請求先ＣＤ = @請求先ＣＤ ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _frUseDetailListCriteria.請求先ＣＤ;
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    // 必須条件
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _frUseDetailListCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _frUseDetailListCriteria.指定年月日From;
                    cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _frUseDetailListCriteria.指定年月日To;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("クレカ利用明細");
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
                    bindingSource1.DataMember = "クレカ利用明細";

                    log.Info("クレカ利用明細を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_CD_COLUMN, "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(CARDNO_COLUMN, "クレカ番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_KIKAN_COLUMN, "請求期間");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_AMOUNT_COLUMN, "今回請求額");

                // 合計行を追加
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, SEIKYU_KIKAN_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, SEIKYU_AMOUNT_COLUMN];
                    FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, SEIKYU_AMOUNT_COLUMN];
                    FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, SEIKYU_AMOUNT_COLUMN];
                    sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                // 検索実行時の検索条件を保存（Excel出力時に使用する）
                _frUseDetailListCriteriaSnapShot = new FRUseDetailListCriteria(_frUseDetailListCriteria);
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
        /// 印刷対象データを取得
        /// </summary>
        /// <returns></returns>
        private DataTable GetPrint()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("   SD.請求番号, ");
                    sql.AppendLine("   SD.請求先ＣＤ, ");
                    sql.AppendLine("   SD.請求先名, ");
                    sql.AppendLine("   SD.今回請求額, ");
                    sql.AppendLine("   SD.前回請求日, ");
                    sql.AppendLine("   CONCAT( ");
                    sql.AppendLine("     FORMAT(DATEADD(day, 1, SD.前回請求日), 'yyyy年M月d日'), ");
                    sql.AppendLine("     ' ～ ', ");
                    sql.AppendLine("     FORMAT(SD.請求日, 'yyyy年M月d日') ");
                    sql.AppendLine("   ) AS 請求期間, ");
                    sql.AppendLine("   CONCAT('****-****-****-', TSM.クレカ番号下4桁) AS クレカ番号, ");
                    sql.AppendLine("   SH.得意先ＣＤ, ");
                    sql.AppendLine("   SH.得意先名, ");
                    sql.AppendLine("   UM.売上日, ");
                    sql.AppendLine("   UM.商品ＣＤ, ");
                    sql.AppendLine("   S.商品名, ");
                    sql.AppendLine("   UM.部署ＣＤ, ");
                    sql.AppendLine("   B.部署名, ");
                    sql.AppendLine("   UM.値引後金額, ");
                    sql.AppendLine("   0 AS 手数料等, ");
                    sql.AppendLine("   UM.値引後金額 + 0 AS 支払金額 ");

                    sql.AppendLine(" FROM ");
                    sql.AppendLine("   ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       A.企業ＣＤ, ");
                    sql.AppendLine("       A.請求番号, ");
                    sql.AppendLine("       A.請求日, ");
                    sql.AppendLine("       A.部署ＣＤ, ");
                    sql.AppendLine("       A.請求先ＣＤ, ");
                    sql.AppendLine("       A.請求先名, ");
                    sql.AppendLine("       A.今回請求額, ");
                    sql.AppendLine("       ( ");
                    sql.AppendLine("         SELECT ");
                    sql.AppendLine("           MAX(P.請求日) ");
                    sql.AppendLine("         FROM ");
                    sql.AppendLine("           請求合計 AS P ");
                    sql.AppendLine("         WHERE ");
                    sql.AppendLine("           P.企業ＣＤ = A.企業ＣＤ ");
                    sql.AppendLine("           AND P.請求先ＣＤ = A.請求先ＣＤ ");
                    sql.AppendLine("           AND P.請求日 < A.請求日 ");
                    sql.AppendLine("       ) AS 前回請求日 ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       請求合計 AS A ");
                    sql.AppendLine("     WHERE ");
                    sql.AppendLine("       A.企業ＣＤ = @企業ＣＤ ");
                    sql.AppendLine("       AND A.部署ＣＤ = @部署ＣＤ ");
                    sql.AppendLine("       AND A.FR支払フラグ = 1 ");
                    sql.AppendLine("       AND A.クレカ決済完了フラグ = 1 ");
                    sql.AppendLine("       AND A.クレカ決済エラー区分 = 0 ");
                    sql.AppendLine("       AND A.請求日 >= @請求日From ");
                    sql.AppendLine("       AND A.請求日 <= @請求日To ");
                    sql.AppendLine("   ) AS SD ");

                    sql.AppendLine("   INNER JOIN ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       企業ＣＤ, ");
                    sql.AppendLine("       請求番号, ");
                    sql.AppendLine("       得意先ＣＤ, ");
                    sql.AppendLine("       MAX(得意先名) AS 得意先名 ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       請求ヘッダ ");
                    sql.AppendLine("     WHERE ");
                    sql.AppendLine("       企業ＣＤ = @企業ＣＤ ");
                    sql.AppendLine("       AND 売上金額税込 <> 0 ");
                    sql.AppendLine("       AND 請求日 >= @請求日From ");
                    sql.AppendLine("       AND 請求日 <= @請求日To ");
                    sql.AppendLine("     GROUP BY ");
                    sql.AppendLine("       企業ＣＤ, ");
                    sql.AppendLine("       請求番号, ");
                    sql.AppendLine("       得意先ＣＤ ");
                    sql.AppendLine("   ) AS SH ON SH.企業ＣＤ = SD.企業ＣＤ AND SH.請求番号 = SD.請求番号 ");

                    sql.AppendLine("   LEFT JOIN 売上明細 AS UM ON UM.企業ＣＤ = SD.企業ＣＤ AND UM.得意先ＣＤ = SH.得意先ＣＤ AND UM.売上日 > SD.前回請求日 AND UM.売上日 <= SD.請求日");

                    sql.AppendLine("   LEFT JOIN 得意先マスタ AS TSM ON TSM.企業ＣＤ = SD.企業ＣＤ AND TSM.得意先ＣＤ = SD.請求先ＣＤ ");
                    sql.AppendLine("   LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = UM.企業ＣＤ AND S.商品ＣＤ = UM.商品ＣＤ ");
                    sql.AppendLine("   LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = UM.企業ＣＤ AND B.部署ＣＤ = UM.部署ＣＤ ");

                    sql.AppendLine(" WHERE ");
                    sql.AppendLine("   1 = 1 ");

                    if (_frUseDetailListCriteria.請求先ＣＤ != null && _frUseDetailListCriteria.請求先ＣＤ != -1)
                    {
                        sql.AppendLine("   AND SD.請求先ＣＤ = @請求先ＣＤ ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _frUseDetailListCriteria.請求先ＣＤ;
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendLine("   SD.請求先ＣＤ ASC, ");
                    sql.AppendLine("   SH.得意先ＣＤ ASC, ");
                    sql.AppendLine("   SD.請求日 ASC ");

                    // 必須条件
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _frUseDetailListCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _frUseDetailListCriteria.指定年月日From;
                    cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _frUseDetailListCriteria.指定年月日To;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    log.Info("カードご利用状況表（兼 ご利用明細）を検索しました。");

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

        #endregion
    }
}

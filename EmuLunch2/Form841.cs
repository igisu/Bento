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
    /// 会計連携集計一覧
    /// </summary>
    public partial class Form841 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYUKEIBI_COLUMN    = 0;   // 集計日列インデックス
        private const int BUSYO_CD_COLUMN    = 1;   // 部署ＣＤ列インデックス
        private const int BUSYO_NAME_COLUMN  = 2;   // 部署名列インデックス
        private const int DENPHO_NO_COLUMN   = 3;   // 伝票番号列インデックス
        private const int DENPHO_BI_COLUMN   = 4;   // 伝票日付列インデックス
        private const int KINGAKU_COLUMN     = 5;   // 金額列インデックス
        private const int SYORI_NAIYO_COLUMN = 6;   // 処理内容列インデックス
        private const int SHIWAKE_CD_COLUMN  = 7;   // 仕訳ＣＤ列インデックス
        private const int SYUKEI_CD_COLUMN   = 8;   // 集計ＣＤ列インデックス
        private const int KARI_CD_COLUMN     = 9;   // 借方ＣＤ列インデックス
        private const int KARI_BUMON_COLUMN  = 10;  // 借方部門ＣＤ列インデックス
        private const int KARI_HOJO_COLUMN   = 11;  // 借方補助ＣＤ列インデックス
        private const int KASHI_CD_COLUMN    = 12;  // 貸方ＣＤ列インデックス
        private const int KASHI_BUMON_COLUMN = 13;  // 貸方部門列インデックス
        private const int KASHI_HOJO_COLUMN  = 14;  // 貸方補助列インデックス
        private const int ZEIRITSU_COLUMN    = 15;  // 税率列インデックス
        private const int SABUN_FLG_COLUMN   = 16;  // 差分フラグ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 会計連携集計一覧検索条件
        /// </summary>
        private KaikeiRenkeiSyukeiListCriteria _kaikeiRenkeiSyukeiListCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form841()
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
        private void Form841_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.group_ico;

                lblCount.Text = "";

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _kaikeiRenkeiSyukeiListCriteria = new KaikeiRenkeiSyukeiListCriteria();
                kaikeiRenkeiSyukeiListCriteriaBindingSource.DataSource = _kaikeiRenkeiSyukeiListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "集計日", "伝票番号", "伝票日付", "仕訳CD" },
                    new string[] { "K.集計日 {0}, K.部署ＣＤ, K.差分フラグ, K.伝票番号",
                                   "K.伝票番号 {0}, K.集計日, K.部署ＣＤ, K.差分フラグ",
                                   "K.伝票日付 {0}, K.部署ＣＤ, K.差分フラグ, K.伝票番号",
                                   "K.仕訳ＣＤ {0}, K.集計日, K.部署ＣＤ, K.差分フラグ, K.伝票番号"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                rdoDesc.Checked = true;
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
        private void Form841_KeyDown(object sender, KeyEventArgs e)
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form841_Shown(object sender, EventArgs e)
        {
            // 売上日にフォーカス
            dtpSyukeibiFrom.Focus();
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
                if (_kaikeiRenkeiSyukeiListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_kaikeiRenkeiSyukeiListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _kaikeiRenkeiSyukeiListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     K.企業ＣＤ");
                    sql.AppendLine("     ,K.集計日");
                    sql.AppendLine("     ,K.部署ＣＤ");
                    sql.AppendLine("     ,K.伝票番号");
                    sql.AppendLine("     ,K.伝票日付");
                    sql.AppendLine("     ,K.仕訳ＣＤ");
                    sql.AppendLine("     ,K.集計ＣＤ");
                    sql.AppendLine("     ,K.借方ＣＤ");
                    sql.AppendLine("     ,K.借方部門ＣＤ");
                    sql.AppendLine("     ,K.借方補助ＣＤ");
                    sql.AppendLine("     ,K.貸方ＣＤ");
                    sql.AppendLine("     ,K.貸方部門ＣＤ");
                    sql.AppendLine("     ,K.貸方補助ＣＤ");
                    sql.AppendLine("     ,K.金額");
                    sql.AppendLine("     ,K.差分フラグ");
                    sql.AppendLine("     ,K.マイナスフラグ");
                    sql.AppendLine("     ,K.税率");
                    sql.AppendLine("     ,SW.処理内容");
                    sql.AppendLine("     ,SW.摘要");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     会計連携集計テーブル AS K");
                    sql.AppendLine("     INNER JOIN 仕訳マスタ AS SW");
                    sql.AppendLine("         ON K.企業ＣＤ = SW.企業ＣＤ");
                    sql.AppendLine("         AND K.部署ＣＤ = SW.部署ＣＤ");
                    sql.AppendLine("         AND K.仕訳ＣＤ = SW.仕訳ＣＤ");
                    sql.AppendLine("     INNER JOIN 部署マスタ AS B");
                    sql.AppendLine("         ON K.企業ＣＤ = B.企業ＣＤ");
                    sql.AppendLine("         AND K.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     K.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_kaikeiRenkeiSyukeiListCriteria.部署ＣＤ != null && _kaikeiRenkeiSyukeiListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and K.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _kaikeiRenkeiSyukeiListCriteria.部署ＣＤ;
                    }

                    if (_kaikeiRenkeiSyukeiListCriteria.集計日From != null)
                    {
                        sql.AppendLine(" and K.集計日 >= @集計日From ");
                        cmd.Parameters.Add("@集計日From", SqlDbType.Date).Value = _kaikeiRenkeiSyukeiListCriteria.集計日From;
                    }

                    if (_kaikeiRenkeiSyukeiListCriteria.集計日To != null)
                    {
                        sql.AppendLine(" and K.集計日 <= @集計日To ");
                        cmd.Parameters.Add("@集計日To", SqlDbType.Date).Value = _kaikeiRenkeiSyukeiListCriteria.集計日To;
                    }

                    if (_kaikeiRenkeiSyukeiListCriteria.伝票日付From != null)
                    {
                        sql.AppendLine(" and K.伝票日付 >= @伝票日付From ");
                        cmd.Parameters.Add("@伝票日付From", SqlDbType.Date).Value = _kaikeiRenkeiSyukeiListCriteria.伝票日付From;
                    }

                    if (_kaikeiRenkeiSyukeiListCriteria.伝票日付To != null)
                    {
                        sql.AppendLine(" and K.伝票日付 <= @伝票日付To ");
                        cmd.Parameters.Add("@伝票日付To", SqlDbType.Date).Value = _kaikeiRenkeiSyukeiListCriteria.伝票日付To;
                    }

                    // 差分を含めない場合は、差分以外を取得
                    if (_kaikeiRenkeiSyukeiListCriteria.差分を含める != true)
                    {
                        sql.AppendLine(" and ISNULL(K.差分フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("会計連携集計一覧");
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
                    bindingSource1.DataMember = "会計連携集計一覧";

                    log.Info("会計連携集計一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SYUKEIBI_COLUMN,    "集計日");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN,    "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN,  "部署名");
                fpSpread1_Sheet1.BindDataColumn(DENPHO_NO_COLUMN,   "伝票番号");
                fpSpread1_Sheet1.BindDataColumn(DENPHO_BI_COLUMN,   "伝票日付");
                fpSpread1_Sheet1.BindDataColumn(KINGAKU_COLUMN,     "金額");
                fpSpread1_Sheet1.BindDataColumn(SYORI_NAIYO_COLUMN, "処理内容");
                fpSpread1_Sheet1.BindDataColumn(SHIWAKE_CD_COLUMN,  "仕訳ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SYUKEI_CD_COLUMN,   "集計ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KARI_CD_COLUMN,     "借方ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KARI_BUMON_COLUMN,  "借方部門ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KARI_HOJO_COLUMN,   "借方補助ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KASHI_CD_COLUMN,    "貸方ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KASHI_BUMON_COLUMN, "貸方部門ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(KASHI_HOJO_COLUMN,  "貸方補助ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(ZEIRITSU_COLUMN,    "税率");
                fpSpread1_Sheet1.BindDataColumn(SABUN_FLG_COLUMN,   "差分フラグ");

                // 背景色をリセット
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = KINGAKU_COLUMN; columnIndex <= KINGAKU_COLUMN; columnIndex++)
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
                    // 検索→行を選択→検索→同じ行を選択 こうすると、bindingSource.Positionが反映されずに0のまま。
                    // （なので詳細画面を開くと、選択した行とは異なるレコードが表示される）
                    // どうも(暗示的／明示的に関わらず)bindingSource.Position=0の設定が無視されているようなので、いったん最終行をカレント行に設定する
                    fpSpread1_Sheet1.ActiveRowIndex = fpSpread1_Sheet1.Rows.Count - 1;
                    bindingSource1.Position = fpSpread1_Sheet1.Rows.Count - 1;

                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    bindingSource1.Position = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
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
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _kaikeiRenkeiSyukeiListCriteria = new KaikeiRenkeiSyukeiListCriteria();
                kaikeiRenkeiSyukeiListCriteriaBindingSource.DataSource = _kaikeiRenkeiSyukeiListCriteria;

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

        /// <summary>
        /// Spreadセル移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            // 最終行(=合計行)には移動しない
            if (e.NewRow >= fpSpread1_Sheet1.Rows.Count - 1)
            {
                e.Cancel = true;
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

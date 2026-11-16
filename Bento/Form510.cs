using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 電子決済請求一覧
    /// </summary>
    public partial class Form510 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEIKYU_NO_COLUMN      = 0;    // 請求番号列インデックス
        private const int SEIKYUBI_COLUMN       = 1;    // 請求日列インデックス
        private const int URIAGE_KBN_COLUMN     = 2;    // 売上区分列インデックス
        private const int YOTEIBI_COLUMN        = 3;    // 回収予定日列インデックス
        private const int KAIAGE_COLUMN         = 4;    // 買上額列インデックス
        private const int NYUKIN_COLUMN         = 5;    // 入金額列インデックス
        private const int KONKAI_SEIKYU_COLUMN  = 6;    // 今回請求額列インデックス
        private const int SEIKYU_DATE_COLUMN    = 7;    // 電子決済請求合計.修正日(処理日)列インデックス
        private const int URIAGE_DATE_COLUMN    = 8;    // 売上編集日時列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 電子決済請求一覧検索条件
        /// </summary>
        private PaySeikyuListCriteria _paySeikyuListCriteria;

        /// <summary>
        /// 電子決済請求一覧検索条件スナップショット
        /// </summary>
        private PaySeikyuListCriteria _paySeikyuListCriteriaSnapShot = null;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form510()
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
        private void Form510_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.money_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnDetail.Enabled = this.HasAuthBrowse;
                btnDetail.Enabled = this.HasAuthBrowse;
                btnDelete.Enabled = this.HasAuthEdit;

                // コンボボックスの選択肢生成
                ComboUtil.SetUriageKubunPaySeikyuShime(cboUriageKbn);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _paySeikyuListCriteria = new PaySeikyuListCriteria();
                paySeikyuListCriteriaBindingSource.DataSource = _paySeikyuListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求番号", "請求日"},
                    new string[] { "A.請求番号 {0}, A.請求先ＣＤ",
                                   "A.請求日 {0}, A.請求先ＣＤ"});

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // スプレッドのヘッダ編集日時列、売上編集日時列、保険区分CD列を非表示にする
                fpSpread1_Sheet1.Columns[SEIKYU_DATE_COLUMN].Visible = false;
                fpSpread1_Sheet1.Columns[URIAGE_DATE_COLUMN].Visible = false;
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
        private void Form510_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F8:                       // 明細画面ボタン押下
                        btnDetail.PerformClick();
                        break;

                    case Keys.F11:                      // 締め解除ボタン押下
                        btnDelete.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
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
                if (_paySeikyuListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_paySeikyuListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _paySeikyuListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.請求番号");
                    sql.AppendLine("     ,A.請求日");
                    sql.AppendLine("     ,A.締区分");
                    sql.AppendLine("     ,A.売上区分");
                    sql.AppendLine("     ,VU.売上区分名");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.請求先ＣＤ");
                    sql.AppendLine("     ,A.請求先名");
                    sql.AppendLine("     ,A.入金額");
                    sql.AppendLine("     ,A.買上金額");
                    sql.AppendLine("     ,A.調整額");
                    sql.AppendLine("     ,A.今回請求額");
                    sql.AppendLine("     ,A.回収予定日");
                    sql.AppendLine("     ,A.印刷者ＣＤ");
                    sql.AppendLine("     ,A.印刷日時");
                    sql.AppendLine("     ,A.EB作成済フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine("     ,A.前回請求日");
                    // -- 今回の請求に対する最新の売上
                    sql.AppendLine("      ,(SELECT");
                    sql.AppendLine("         MAX(U.修正日)");
                    sql.AppendLine("      FROM");
                    sql.AppendLine("          電子決済請求合計 AS G");
                    sql.AppendLine("          LEFT JOIN 電子決済請求ヘッダ H ON");
                    sql.AppendLine("              H.企業ＣＤ = G.企業ＣＤ");
                    sql.AppendLine("              AND H.請求番号 = G.請求番号");
                    sql.AppendLine("          LEFT JOIN 売上明細 AS U ON");
                    sql.AppendLine("              U.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("              AND U.売上区分 = A.売上区分");
                    sql.AppendLine("              AND U.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("              AND U.売上日 > ISNULL(A.前回請求日, DATEADD(month, -3, getdate()))");  // -- 前回請求が無い場合は、3ヶ月前までを探す
                    sql.AppendLine("              AND U.売上日 <= G.請求日");
                    sql.AppendLine("      WHERE");
                    sql.AppendLine("          G.企業ＣＤ = A.企業ＣＤ");
                    sql.AppendLine("          AND G.請求番号 = A.請求番号");
                    sql.AppendLine("      ) AS 売上編集日時");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         T.企業ＣＤ");
                    sql.AppendLine("         ,T.請求番号");
                    sql.AppendLine("         ,T.請求日");
                    sql.AppendLine("         ,T.締区分");
                    sql.AppendLine("         ,T.売上区分");
                    sql.AppendLine("         ,T.部署ＣＤ");
                    sql.AppendLine("         ,T.請求先ＣＤ");
                    sql.AppendLine("         ,T.請求先名");
                    sql.AppendLine("         ,T.入金額");
                    sql.AppendLine("         ,T.買上金額");
                    sql.AppendLine("         ,T.調整額");
                    sql.AppendLine("         ,T.今回請求額");
                    sql.AppendLine("         ,T.回収予定日");
                    sql.AppendLine("         ,T.印刷者ＣＤ");
                    sql.AppendLine("         ,T.印刷日時");
                    sql.AppendLine("         ,T.EB作成済フラグ");
                    sql.AppendLine("         ,T.作成担当者ＣＤ");
                    sql.AppendLine("         ,T.作成日");
                    sql.AppendLine("         ,T.修正担当者ＣＤ");
                    sql.AppendLine("         ,T.修正日");
                    sql.AppendLine("         ,P.前回請求日");
                    sql.AppendLine("     FROM");
                    // -- ①今回請求のキーと、それに対する前回請求のキーを求める
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             T1.企業ＣＤ");
                    sql.AppendLine("             ,T1.請求番号 AS 今回請求番号");
                    sql.AppendLine("             ,T1.請求先ＣＤ");
                    sql.AppendLine("             ,MAX(P1.請求日) AS 前回請求日");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             電子決済請求合計 AS T1");
                    sql.AppendLine("             LEFT JOIN 電子決済請求合計 AS P1");
                    sql.AppendLine("                 ON T1.企業ＣＤ = P1.企業ＣＤ");
                    sql.AppendLine("                 AND T1.請求先ＣＤ = P1.請求先ＣＤ");
                    sql.AppendLine("                 AND P1.請求日 < T1.請求日");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             T1.企業ＣＤ = @企業ＣＤ");
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_paySeikyuListCriteria.売上区分 != null && _paySeikyuListCriteria.売上区分 != -1)
                    {
                        sql.AppendLine("             AND T1.売上区分 = @売上区分");
                        cmd.Parameters.Add("@売上区分", SqlDbType.Int).Value = _paySeikyuListCriteria.売上区分;
                    }

                    if (_paySeikyuListCriteria.請求日From != null)
                    {
                        sql.AppendLine("             AND T1.請求日 >= @請求日From");
                        cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _paySeikyuListCriteria.請求日From;
                    }

                    if (_paySeikyuListCriteria.請求日To != null)
                    {
                        sql.AppendLine("             AND T1.請求日 <= @請求日To");
                        cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _paySeikyuListCriteria.請求日To;
                    }

                    if (_paySeikyuListCriteria.回収予定日From != null)
                    {
                        sql.AppendLine("             AND T1.回収予定日 >= @回収予定日From");
                        cmd.Parameters.Add("@回収予定日From", SqlDbType.Date).Value = _paySeikyuListCriteria.回収予定日From;
                    }

                    if (_paySeikyuListCriteria.回収予定日To != null)
                    {
                        sql.AppendLine("             AND T1.回収予定日 <= @回収予定日To");
                        cmd.Parameters.Add("@回収予定日To", SqlDbType.Date).Value = _paySeikyuListCriteria.回収予定日To;
                    }

                    if (_paySeikyuListCriteria.処理日 != null)
                    {
                        sql.AppendLine("             AND CONVERT(date, T1.修正日) = @処理日");
                        cmd.Parameters.Add("@処理日", SqlDbType.Date).Value = _paySeikyuListCriteria.処理日;
                    }

                    if (_paySeikyuListCriteria.今回請求額 != null)
                    {
                        sql.AppendLine("             AND T1.今回請求額 >= @今回請求額");
                        cmd.Parameters.Add("@今回請求額", SqlDbType.Decimal).Value = _paySeikyuListCriteria.今回請求額;
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             T1.企業ＣＤ");
                    sql.AppendLine("             ,T1.請求番号");
                    sql.AppendLine("             ,T1.請求先ＣＤ");
                    sql.AppendLine("         ) AS P");
                    // -- ②今回請求の詳細
                    sql.AppendLine("         INNER JOIN 電子決済請求合計 T ON");
                    sql.AppendLine("             T.企業ＣＤ = P.企業ＣＤ");
                    sql.AppendLine("             AND T.請求番号 = P.今回請求番号");
                    sql.AppendLine("     ) AS A");
                    sql.AppendLine("     LEFT JOIN V_売上区分 AS VU ON A.売上区分 = VU.売上区分ＣＤ");
                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("電子決済請求一覧");
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
                    bindingSource1.DataMember = "電子決済請求一覧";

                    log.Info("電子決済請求一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SEIKYU_NO_COLUMN,      "請求番号");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN,       "請求日");
                fpSpread1_Sheet1.BindDataColumn(URIAGE_KBN_COLUMN,     "売上区分名");
                fpSpread1_Sheet1.BindDataColumn(YOTEIBI_COLUMN,        "回収予定日");
                fpSpread1_Sheet1.BindDataColumn(KAIAGE_COLUMN,         "買上金額");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,         "入金額");
                fpSpread1_Sheet1.BindDataColumn(KONKAI_SEIKYU_COLUMN,  "今回請求額");
                fpSpread1_Sheet1.BindDataColumn(SEIKYU_DATE_COLUMN,    "修正日");
                fpSpread1_Sheet1.BindDataColumn(URIAGE_DATE_COLUMN,    "売上編集日時");

                // 修正日≦売上編集日時 の場合は行の文字色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    DateTime? updateDateTimeHeader = fpSpread1_Sheet1.Cells[rowIndex, SEIKYU_DATE_COLUMN].Value as DateTime?;
                    DateTime? updateDateTimeTotal = fpSpread1_Sheet1.Cells[rowIndex, URIAGE_DATE_COLUMN].Value as DateTime?;
                    if (updateDateTimeHeader != null && updateDateTimeTotal != null && updateDateTimeHeader <= updateDateTimeTotal)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = Color.Red;
                    }
                }

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
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, URIAGE_KBN_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = KAIAGE_COLUMN; columnIndex <= KONKAI_SEIKYU_COLUMN; columnIndex++)
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

                // 検索実行時の検索条件を保存
                _paySeikyuListCriteriaSnapShot = new PaySeikyuListCriteria(_paySeikyuListCriteria);
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
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件を初期化
                _paySeikyuListCriteria = new PaySeikyuListCriteria();
                paySeikyuListCriteriaBindingSource.DataSource = _paySeikyuListCriteria;

                // 一覧もクリアする
                // 数式を消すため、全行削除する
                fpSpread1_Sheet1.Rows.Count = 0;
                bindingSource1.DataSource = null;

                // この時点の件数を表示する
                lblCount.Text = String.Format("{0}件", fpSpread1_Sheet1.Rows.Count);
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
        /// 明細画面ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount == 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.ActiveRowIndex == fpSpread1_Sheet1.Rows.Count - 1)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail();       // 詳細表示（既存レコード）
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
        /// 締め解除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount == 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.ActiveRowIndex == fpSpread1_Sheet1.Rows.Count - 1)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                PaySeikyuGokei paySeikyuGokei = new PaySeikyuGokei(dr);

                // 削除可能かチェック
                if (CanDelete(paySeikyuGokei) == false)
                {
                    return;
                }

                if (MessageBox.Show(this, "このデータを削除しますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                if( Delete(paySeikyuGokei) == false)
                {
                    return;
                }

                btnQuery.PerformClick();        // 再検索
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
                    fpSpread1.SaveExcel(fileName, excelSaveFlags);
                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (BentoException ex)
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
        /// Spreadセルダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_CellDoubleClick(object sender, FarPoint.Win.Spread.CellClickEventArgs e)
        {
            try
            {
                // 列ヘッダー or 列フッター or 行ヘッダー？
                if (e.ColumnHeader || e.ColumnFooter || e.RowHeader)
                {
                    return;
                }

                // 合計行？
                if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
                {
                    return;
                }

                ShowDetail();       // 詳細表示
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
        /// Spreadキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Return)
                {
                    if (fpSpread1_Sheet1.SelectionCount != 0)
                    {
                        ShowDetail();       // 詳細表示
                    }
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
        /// <summary>
        /// 明細表示
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                PaySeikyuDetailCriteria paySeikyuDetailCriteria = new PaySeikyuDetailCriteria(dr);
                Form511 form = new Form511(paySeikyuDetailCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form511_FormClosed);
            }
            catch (BentoException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 明細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form511_FormClosed(object sender, FormClosedEventArgs e)
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

                // 先頭行のインデックスを記憶
                int rowIndex = fpSpread1.GetViewportTopRow(0);
                int activeRowIndex = fpSpread1_Sheet1.ActiveRowIndex;
                int activeColumnIndex = fpSpread1_Sheet1.ActiveColumnIndex;

                btnQuery.PerformClick();        // 再検索

                // 先頭行のインデックスを復元（そこにスクロール）
                fpSpread1.SetViewportTopRow(0, rowIndex);

                // アクティブセルも可能な限り復元する
                if (activeRowIndex >= fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount)
                {
                    activeRowIndex = fpSpread1_Sheet1.Rows.Count - fpSpread1_Sheet1.FrozenTrailingRowCount - 1;
                }
                if (0 <= activeRowIndex)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = activeRowIndex;
                    bindingSource1.Position = activeRowIndex;       // Currentプロパティがずれる場合があるのを回避
                    fpSpread1_Sheet1.AddSelection(activeRowIndex, -1, 1, -1);   // 単一選択行を復元
                }
                fpSpread1_Sheet1.ActiveColumnIndex = activeColumnIndex;
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
        /// 削除可能か？
        /// </summary>
        /// <param name="paySeikyuGokei">電子決済請求合計</param>
        /// <returns>削除可能な場合はTrue</returns>
        private bool CanDelete(PaySeikyuGokei paySeikyuGokei)
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "編集できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    // 指定した請求番号より未来のレコードがあれば削除禁止
                    cmd.CommandText = "\r\n SELECT" +
                                      "\r\n     COUNT(F1.請求番号) AS CNT" +
                                      "\r\n FROM" +
                                      "\r\n     電子決済請求合計 G1" +
                                      "\r\n     INNER JOIN 電子決済請求合計 F1 ON" +
                                      "\r\n                  G1.企業ＣＤ = F1.企業ＣＤ AND" +
                                      "\r\n                  G1.請求先ＣＤ = F1.請求先ＣＤ AND" +
                                      "\r\n                  F1.請求日 > G1.請求日" +
                                      "\r\n WHERE" +
                                      "\r\n     G1.企業ＣＤ = @企業ＣＤ" +
                                      "\r\n AND G1.請求番号 = @請求番号";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = paySeikyuGokei.請求番号;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if ((int)reader["CNT"] != 0)
                        {
                            MessageBox.Show(this, "最新の請求ではありません。削除できません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }
                }

                return true;
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
        /// DB削除
        /// </summary>
        /// <param name="paySeikyuGokei">電子決済請求合計</param>
        /// <returns>削除できた場合はTrue</returns>
        private bool Delete(PaySeikyuGokei paySeikyuGokei)
        {
            History.ProcessType historyProcess = History.ProcessType.Delete;

            SqlCommand cmd;
            UserData userData = UserData.getInstance();

            using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                try
                {
                    // 電子決済請求ヘッダを削除
                    cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = "DELETE FROM 電子決済請求ヘッダ WHERE 企業ＣＤ = @企業ＣＤ AND 請求番号 = @請求番号";
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = paySeikyuGokei.請求番号;
                    log.Debug(cmd.CommandTextWithParameters());
                    cmd.ExecuteNonQuery();

                    // 操作履歴に保存
                    History.Save(con, tran, this.Text, "電子決済請求ヘッダ", paySeikyuGokei.請求番号.ToString(), historyProcess);

                    // 電子決済請求合計を削除
                    cmd = con.CreateCommand();
                    cmd.Transaction = tran;
                    cmd.CommandText = "DELETE FROM 電子決済請求合計 WHERE 企業ＣＤ = @企業ＣＤ AND 請求番号 = @請求番号";
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@請求番号", SqlDbType.Int).Value = paySeikyuGokei.請求番号;
                    log.Debug(cmd.CommandTextWithParameters());
                    cmd.ExecuteNonQuery();

                    // 操作履歴に保存
                    History.Save(con, tran, this.Text, "電子決済請求合計", paySeikyuGokei.請求番号.ToString(), historyProcess);

                    tran.Commit();
                    log.InfoFormat("電子決済請求ヘッダ・電子決済請求合計を{0}しました。", historyProcess.DisplayName());
                    MessageBox.Show(this, string.Format("{0}しました。", historyProcess.DisplayName()), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
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
        }
        #endregion
    }
}

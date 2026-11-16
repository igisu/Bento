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
    /// 部署マスタ一覧
    /// </summary>
    public partial class Form707 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_CD_COLUMN    = 0;   // 部署ＣＤ列インデックス
        private const int BUSYO_GROUP_COLUMN = 1;   // 部署グループ列インデックス
        private const int BUSYO_NAME_COLUMN  = 2;   // 部署名列インデックス
        private const int TEL_COLUMN         = 3;   // 電話番号列インデックス
        private const int FAX_COLUMN         = 4;   // FAX列インデックス
        private const int SHITEN_COLUMN      = 5;   // 支店列インデックス
        private const int INVALID_COLUMN     = 6;   // 無効フラグ列インデックス
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用部署データ
        /// </summary>
        private BusyoCriteria _busyoCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form707()
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
        private void Form707_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnNew.Enabled = this.HasAuthEdit;
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyoGroup(cboBusyoGroup);
                ComboUtil.SetShiten(cboShiten);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _busyoCriteria = new BusyoCriteria();

                // 支店CDコンボの初期値を設定
                _busyoCriteria.支店ＣＤ = UserData.getInstance().支店ＣＤ ?? -1;

                busyoBindingSource.DataSource = _busyoCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "部署CD", "部署グループ", "部署名", "支店" },
                    new string[] { "A.部署ＣＤ {0}", "A.部署グループ {0}, A.部署ＣＤ", "A.部署名 {0}, A.部署ＣＤ",
                                   "A.支店ＣＤ {0}, A.部署ＣＤ" });

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // スプレッドの最終列（無効フラグ）を非表示にする
                fpSpread1_Sheet1.Columns[INVALID_COLUMN].Visible = false;
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
        private void Form707_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F5:
                        btnQuery.PerformClick();        // 検索ボタン押下
                        break;

                    case Keys.F6:                       // 新規作成ボタン押下
                        btnNew.PerformClick();
                        break;

                    case Keys.F8:                       // 詳細画面ボタン押下
                        btnDetail.PerformClick();
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
                if (_busyoCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_busyoCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _busyoCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.部署グループ");
                    sql.AppendLine("     ,BG.部署グループ名");
                    sql.AppendLine("     ,A.部署名");
                    sql.AppendLine("     ,A.部署名カナ");
                    sql.AppendLine("     ,A.会社名称");
                    sql.AppendLine("     ,A.郵便番号");
                    sql.AppendLine("     ,A.住所");
                    sql.AppendLine("     ,A.電話番号");
                    sql.AppendLine("     ,A.ＦＡＸ");
                    sql.AppendLine("     ,A.登録番号");
                    sql.AppendLine("     ,A.モバイル_メッセージ");
                    sql.AppendLine("     ,A.金融機関ＣＤ1");
                    sql.AppendLine("     ,B1.銀行名 AS 金融機関名称1");
                    sql.AppendLine("     ,A.金融機関支店ＣＤ1");
                    sql.AppendLine("     ,BR1.支店名 AS 金融機関支店名称1");
                    sql.AppendLine("     ,A.口座種別1");
                    sql.AppendLine("     ,A.口座番号1");
                    sql.AppendLine("     ,A.口座名義人1");
                    sql.AppendLine("     ,A.金融機関ＣＤ2");
                    sql.AppendLine("     ,B2.銀行名 AS 金融機関名称2");
                    sql.AppendLine("     ,A.金融機関支店ＣＤ2");
                    sql.AppendLine("     ,BR2.支店名 AS 金融機関支店名称2");
                    sql.AppendLine("     ,A.口座種別2");
                    sql.AppendLine("     ,A.口座番号2");
                    sql.AppendLine("     ,A.口座名義人2");
                    sql.AppendLine("     ,A.請求書タイプ区分");
                    sql.AppendLine("     ,A.請求書備考1");
                    sql.AppendLine("     ,A.請求書備考2");
                    sql.AppendLine("     ,A.納品書備考1");
                    sql.AppendLine("     ,A.納品書備考2");
                    sql.AppendLine("     ,A.支店ＣＤ");
                    sql.AppendLine("     ,SI.支店名");
                    sql.AppendLine("     ,A.配送日算出日数");
                    sql.AppendLine("     ,A.配送稼働日");
                    sql.AppendLine("     ,A.得意先休日考慮フラグ");
                    sql.AppendLine("     ,A.製造日算出日数");
                    sql.AppendLine("     ,A.製造稼働日");
                    sql.AppendLine("     ,A.配送集計表出力区分");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者CD");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     部署マスタ AS A");
                    sql.AppendLine("     LEFT JOIN V_部署グループ AS BG ON A.部署グループ = BG.部署グループCD");
                    sql.AppendLine("     LEFT JOIN 金融機関名称     AS B1  ON A.金融機関ＣＤ1 = B1.銀行ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS BR1 ON A.金融機関ＣＤ1 = BR1.銀行ＣＤ AND A.金融機関支店ＣＤ1 = BR1.支店ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関名称     AS B2  ON A.金融機関ＣＤ2 = B2.銀行ＣＤ");
                    sql.AppendLine("     LEFT JOIN 金融機関支店名称 AS BR2 ON A.金融機関ＣＤ2 = BR2.銀行ＣＤ AND A.金融機関支店ＣＤ2 = BR2.支店ＣＤ");
                    sql.AppendLine("     LEFT JOIN 支店マスタ AS SI ON SI.企業ＣＤ = @企業ＣＤ AND A.支店ＣＤ = SI.支店ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_busyoCriteria.部署ＣＤ != null)
                    {
                        sql.AppendLine(" and A.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _busyoCriteria.部署ＣＤ;
                    }

                    if (_busyoCriteria.部署グループ != null && _busyoCriteria.部署グループ != -1)
                    {
                        sql.AppendLine(" and A.部署グループ = @部署グループ ");
                        cmd.Parameters.Add("@部署グループ", SqlDbType.Int).Value = _busyoCriteria.部署グループ;
                    }

                    if (_busyoCriteria.部署名 != null)
                    {
                        sql.AppendLine(" and (A.部署名 like @部署名 escape '#' OR A.部署名カナ like @部署名 escape '#')");
                        cmd.Parameters.Add("@部署名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_busyoCriteria.部署名);
                    }

                    if (_busyoCriteria.支店ＣＤ != null && _busyoCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.支店ＣＤ = @支店ＣＤ ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _busyoCriteria.支店ＣＤ;
                    }

                    // チェックONの場合は全て検索するので条件に加えない（つまりチェックOFFの場合は無効フラグが0のものだけを検索）
                    if (_busyoCriteria.無効フラグ == null || _busyoCriteria.無効フラグ == false)
                    {
                        sql.AppendLine(" and ISNULL(A.無効フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("部署マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "部署マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("部署マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN, "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_GROUP_COLUMN, "部署グループ名");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(TEL_COLUMN, "電話番号");
                fpSpread1_Sheet1.BindDataColumn(FAX_COLUMN, "ＦＡＸ");
                fpSpread1_Sheet1.BindDataColumn(SHITEN_COLUMN, "支店名");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN, "無効フラグ");

                // 無効フラグの値に応じて行の背景色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, INVALID_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = SystemColors.ControlLight;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = SystemColors.GrayText;
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
                _busyoCriteria = new BusyoCriteria();

                // 支店CDコンボの初期値を設定
                _busyoCriteria.支店ＣＤ = UserData.getInstance().支店ＣＤ ?? -1;

                busyoBindingSource.DataSource = _busyoCriteria;

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
        /// 新規作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ShowNew();      // 詳細表示（新規レコード）
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
        /// 詳細画面ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (fpSpread1_Sheet1.SelectionCount != 0)
                {
                    ShowDetail();       // 詳細表示（既存レコード）
                }
                else
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        /// <summary>
        /// 詳細表示（新規レコード）
        /// </summary>
        private void ShowNew()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                Busyo busyo = new Busyo();
                Form707D form = new Form707D(busyo);
                if (form.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form707D_FormClosed);
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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                Busyo busyo = new Busyo(dr);
                Form707D form = new Form707D(busyo);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form707D_FormClosed);
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
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form707D_FormClosed(object sender, FormClosedEventArgs e)
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
        #endregion
    }
}

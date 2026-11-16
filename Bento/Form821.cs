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
    /// Web得意先マスタ一覧
    /// </summary>
    public partial class Form821 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TOKUISAKI_CD_COLUMN    = 0;   // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN  = 1;   // 得意先名列インデックス
        private const int LAST_LOGIN_DATE_COLUMN = 2;   // 最終ログイン日時列インデックス
        private const int LAST_LOGIN_IP_COLUMN   = 3;   // 最終ログインIP列インデックス
        private const int INVALID_COLUMN         = 4;   // 無効フラグ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用Web得意先マスタ
        /// </summary>
        private WebTokuisakiCriteria _webTokuisakiCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form821()
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
        private void Form821_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.world_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnNew.Enabled = this.HasAuthEdit;
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _webTokuisakiCriteria = new WebTokuisakiCriteria();

                // 部署CDコンボの初期値を設定
                _webTokuisakiCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                webTokuisakiCriteriaBindingSource.DataSource = _webTokuisakiCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "得意先名", "最終ログイン日時", "最終ログインIP" },
                    new string[] { "A.得意先ＣＤ {0}, L.最終ログイン日時 DESC",
                                   "T.得意先名 {0}, A.得意先ＣＤ, L.最終ログイン日時 DESC",
                                   "L.最終ログイン日時 {0}, A.得意先ＣＤ",
                                   "L.IPv4アドレス {0}, A.得意先ＣＤ, L.最終ログイン日時 DESC"});

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
        private void Form821_KeyDown(object sender, KeyEventArgs e)
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
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuiCD_FormClosed);
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
        /// 得意先選択ダイアログクローズ（得意先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900TokuiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    _webTokuisakiCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCD);
                    ReadBinding(lblTokuisakiName);
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_webTokuisakiCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_webTokuisakiCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _webTokuisakiCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     ,A.得意先ＣＤ");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,A.ログインＰＷ");
                    sql.AppendLine("     ,A.メールアドレス");
                    sql.AppendLine("     ,A.個別注文締切時間");
                    sql.AppendLine("     ,A.個別注文締切分");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine("     ,L.IPv4アドレス");
                    sql.AppendLine("     ,L.最終ログイン日時");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     WEB_M_得意先 AS A");
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             LA.ID");
                    sql.AppendLine("             ,LA.企業ＣＤ");
                    sql.AppendLine("             ,LA.得意先ＣＤ");
                    sql.AppendLine("             ,LA.IPv4アドレス");
                    sql.AppendLine("             ,LA.最終ログイン日時");
                    sql.AppendLine("             ,cast(LA.最終ログイン日時 AS date) AS 最終ログイン日");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             WEB_T_自動ログイン AS LA");
                    sql.AppendLine("             INNER JOIN");
                    sql.AppendLine("                 (SELECT");
                    sql.AppendLine("                     得意先ＣＤ");
                    sql.AppendLine("                     ,企業ＣＤ");
                    sql.AppendLine("                     ,max(最終ログイン日時) AS 最終ログイン日時");
                    sql.AppendLine("                 FROM");
                    sql.AppendLine("                     WEB_T_自動ログイン");
                    sql.AppendLine("                 WHERE");
                    sql.AppendLine("                     企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 GROUP BY");
                    sql.AppendLine("                     得意先ＣＤ");
                    sql.AppendLine("                     ,企業ＣＤ");
                    sql.AppendLine("                 ) AS LB");
                    sql.AppendLine("                 ON LA.得意先ＣＤ = LB.得意先ＣＤ");
                    sql.AppendLine("                 AND LA.企業ＣＤ = LB.企業ＣＤ");
                    sql.AppendLine("                 AND LA.最終ログイン日時 = LB.最終ログイン日時");
                    sql.AppendLine("            ) AS L ON A.企業ＣＤ = L.企業ＣＤ AND A.得意先ＣＤ = L.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON A.企業ＣＤ = T.企業ＣＤ AND A.得意先ＣＤ = T.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_webTokuisakiCriteria.部署ＣＤ != null && _webTokuisakiCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" AND T.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _webTokuisakiCriteria.部署ＣＤ;
                    }

                    if (_webTokuisakiCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine(" AND A.得意先ＣＤ = @得意先ＣＤ ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _webTokuisakiCriteria.得意先ＣＤ;
                    }

                    if (_webTokuisakiCriteria.最終ログイン日From != null)
                    {
                        sql.AppendLine(" AND L.最終ログイン日 >= @最終ログイン日From ");
                        cmd.Parameters.Add("@最終ログイン日From", SqlDbType.Date).Value = _webTokuisakiCriteria.最終ログイン日From;
                    }

                    if (_webTokuisakiCriteria.最終ログイン日To != null)
                    {
                        sql.AppendLine(" AND L.最終ログイン日 <= @最終ログイン日To ");
                        cmd.Parameters.Add("@最終ログイン日To", SqlDbType.Date).Value = _webTokuisakiCriteria.最終ログイン日To;
                    }

                    if (_webTokuisakiCriteria.最終ログインIPアドレス != null)
                    {
                        sql.AppendLine(" AND L.IPv4アドレス = @IPアドレス ");
                        cmd.Parameters.Add("@IPアドレス", SqlDbType.VarChar).Value = _webTokuisakiCriteria.最終ログインIPアドレス;
                    }

                    // チェックONの場合は全て検索するので条件に加えない（つまりチェックOFFの場合は無効フラグが0のものだけを検索）
                    if (_webTokuisakiCriteria.無効フラグ == null || _webTokuisakiCriteria.無効フラグ == false)
                    {
                        sql.AppendLine(" AND ISNULL(A.無効フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("Web得意先マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "Web得意先マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("Web得意先マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,    "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN,  "得意先名");
                fpSpread1_Sheet1.BindDataColumn(LAST_LOGIN_DATE_COLUMN, "最終ログイン日時");
                fpSpread1_Sheet1.BindDataColumn(LAST_LOGIN_IP_COLUMN,   "IPv4アドレス");
                fpSpread1_Sheet1.BindDataColumn(INVALID_COLUMN,         "無効フラグ");

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
                _webTokuisakiCriteria = new WebTokuisakiCriteria();

                // 部署CDコンボの初期値を設定
                _webTokuisakiCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                webTokuisakiCriteriaBindingSource.DataSource = _webTokuisakiCriteria;

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
                WebTokuisaki webTokuisaki = new WebTokuisaki();
                Form821D form = new Form821D(webTokuisaki);
                if (form.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form821D_FormClosed);
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
                WebTokuisaki webTokuisaki = new WebTokuisaki(dr);
                Form821D form = new Form821D(webTokuisaki);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form821D_FormClosed);
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
        private void Form821D_FormClosed(object sender, FormClosedEventArgs e)
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

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
    /// Web注文履歴一覧
    /// </summary>
    public partial class Form820 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int OPERATION_DATETIME_COLUMN = 0;    // 操作日時列インデックス
        private const int TOKUISAKI_CD_COLUMN       = 1;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN     = 2;    // 得意先名列インデックス
        private const int ORDER_TYPE_COLUMN         = 3;    // 注文タイプ列インデックス
        private const int OPERATION_DETAIL_COLUMN   = 4;    // 操作内容列インデックス
        private const int TOKEN_COLUMN              = 5;    // 認証キー(トークン)列インデックス
        private const int IP_ADDRESS_COLUMN         = 6;    // IPアドレス列インデックス
        private const int UA_COLUMN                 = 7;    // UA列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用Web注文履歴
        /// </summary>
        private OrderHistoryCriteria _orderHistoryCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form820()
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
        private void Form820_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.world_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetWebHistoryKubun(cboOrderKbn);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _orderHistoryCriteria = new OrderHistoryCriteria();

                //操作時間
                _orderHistoryCriteria.操作日From = DateTime.Today;
                _orderHistoryCriteria.操作時刻From = DateTime.Today;
                _orderHistoryCriteria.操作日To = DateTime.Today;
                _orderHistoryCriteria.操作時刻To = new DateTime(1,1,1,23,59,59);

                orderHistoryCriteriaBindingSource.DataSource = _orderHistoryCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "操作日時", "得意先CD", "IPアドレス", "注文区分" },
                    new string[] { "操作日時 {0}, 操作番号 {0}",
                                   "得意先ＣＤ {0}, 操作日時, 操作番号",
                                   "IPv4アドレス {0}, 操作日時, 操作番号",
                                   "WEB履歴区分ＣＤ {0}, 操作日時, 操作番号" });

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
        private void Form820_KeyDown(object sender, KeyEventArgs e)
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
                    _orderHistoryCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTokuisakiCD);
                    ReadBinding(lblTokuisakiName);
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
                if (_orderHistoryCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_orderHistoryCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _orderHistoryCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     H.操作番号");
                    sql.AppendLine("     ,H.企業ＣＤ");
                    sql.AppendLine("     ,H.認証キー");
                    sql.AppendLine("     ,H.得意先ＣＤ");
                    sql.AppendLine("     ,T.得意先名");
                    sql.AppendLine("     ,H.UA");
                    sql.AppendLine("     ,H.IPv4アドレス");
                    sql.AppendLine("     ,H.WEB履歴区分ＣＤ");
                    sql.AppendLine("     ,VW.WEB履歴区分名");
                    sql.AppendLine("     ,H.操作日時");
                    sql.AppendLine("     ,H.操作内容");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     WEB_T_注文履歴 AS H");
                    sql.AppendLine("     LEFT JOIN 得意先マスタ as T");
                    sql.AppendLine("         ON H.企業ＣＤ = T.企業ＣＤ");
                    sql.AppendLine("         AND H.得意先ＣＤ = T.得意先ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_WEB履歴区分 AS VW");
                    sql.AppendLine("         ON VW.WEB履歴区分ＣＤ = H.WEB履歴区分ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     H.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_orderHistoryCriteria.操作日時From != null)
                    {
                        sql.AppendLine(" AND H.操作日時 >= @操作日時From ");
                        cmd.Parameters.Add("@操作日時From", SqlDbType.DateTime).Value = _orderHistoryCriteria.操作日時From;
                    }

                    if (_orderHistoryCriteria.操作日時To != null)
                    {
                        sql.AppendLine(" AND H.操作日時 <= @操作日時To ");
                        cmd.Parameters.Add("@操作日時To", SqlDbType.DateTime).Value = _orderHistoryCriteria.操作日時To;
                    }

                    if (_orderHistoryCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine(" AND H.得意先ＣＤ = @得意先ＣＤ ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _orderHistoryCriteria.得意先ＣＤ;
                    }

                    if (_orderHistoryCriteria.注文区分 != null && _orderHistoryCriteria.注文区分 >= 0)
                    {
                        sql.AppendLine(" AND H.WEB履歴区分ＣＤ = @WEB履歴区分ＣＤ ");
                        cmd.Parameters.Add("@WEB履歴区分ＣＤ", SqlDbType.Int).Value = _orderHistoryCriteria.注文区分;
                    }

                    if (_orderHistoryCriteria.UA != null)
                    {
                        sql.AppendLine(" AND H.UA LIKE @UA escape '#' ");
                        cmd.Parameters.Add("@UA", SqlDbType.VarChar).Value = DBUtil.EscapeString(_orderHistoryCriteria.UA);
                    }

                    if (_orderHistoryCriteria.IPアドレス != null)
                    {
                        sql.AppendLine(" AND H.IPv4アドレス = @IPアドレス ");
                        cmd.Parameters.Add("@IPアドレス", SqlDbType.VarChar).Value = _orderHistoryCriteria.IPアドレス;
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("Web注文履歴");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "Web注文履歴";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("Web注文履歴を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(OPERATION_DATETIME_COLUMN, "操作日時");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN,       "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN,     "得意先名");
                fpSpread1_Sheet1.BindDataColumn(ORDER_TYPE_COLUMN,         "WEB履歴区分名");
                fpSpread1_Sheet1.BindDataColumn(OPERATION_DETAIL_COLUMN,   "操作内容");
                fpSpread1_Sheet1.BindDataColumn(TOKEN_COLUMN,              "認証キー");
                fpSpread1_Sheet1.BindDataColumn(IP_ADDRESS_COLUMN,         "IPv4アドレス");
                fpSpread1_Sheet1.BindDataColumn(UA_COLUMN,                 "UA");

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
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
                _orderHistoryCriteria = new OrderHistoryCriteria();

                //操作時間
                _orderHistoryCriteria.操作日From = DateTime.Today;
                _orderHistoryCriteria.操作時刻From = DateTime.Today;
                _orderHistoryCriteria.操作日To = DateTime.Today;
                _orderHistoryCriteria.操作時刻To = new DateTime(1,1,1,23,59,59);

                orderHistoryCriteriaBindingSource.DataSource = _orderHistoryCriteria;

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

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        #endregion
    }
}

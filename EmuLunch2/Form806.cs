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
    /// 操作履歴一覧
    /// </summary>
    public partial class Form806 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int OPERATION_DATETIME_COLUMN = 0;    // 操作日時列インデックス
        private const int IP_ADDRESS_COLUMN         = 1;    // IPアドレス列インデックス
        private const int OPERATION_DETAIL_COLUMN   = 2;    // 操作内容列インデックス
        private const int HISTORY_TYPE_COLUMN       = 3;    // 履歴タイプ列インデックス
        private const int ERROR_FLG_COLUMN          = 4;    // エラーフラグ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用操作履歴
        /// </summary>
        private OperationHistoryCriteria _operationHistoryCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form806()
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
        private void Form806_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.group_ico;

                lblCount.Text = "";

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _operationHistoryCriteria = new OperationHistoryCriteria();

                //操作時間
                _operationHistoryCriteria.操作日From = DateTime.Today;
                _operationHistoryCriteria.操作時刻From = DateTime.Today;
                _operationHistoryCriteria.操作日To = DateTime.Today;
                _operationHistoryCriteria.操作時刻To = new DateTime(1,1,1,23,59,59);

                courseBindingSource.DataSource = _operationHistoryCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "操作日時", "IPアドレス", "履歴タイプ" },
                    new string[] { "操作日時 {0}, 操作番号 {0}", "IPv4アドレス {0}, 操作日時, 操作番号", "履歴タイプ {0}, 操作日時, 操作番号" });

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

                // スプレッドの最終列（エラーフラグ）を非表示にする
                fpSpread1_Sheet1.Columns[ERROR_FLG_COLUMN].Visible = false;
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
        private void Form806_KeyDown(object sender, KeyEventArgs e)
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_operationHistoryCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_operationHistoryCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _operationHistoryCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     操作番号");
                    sql.AppendLine("     ,企業ＣＤ");
                    sql.AppendLine("     ,IPv4アドレス");
                    sql.AppendLine("     ,担当者ＣＤ");
                    sql.AppendLine("     ,操作日時");
                    sql.AppendLine("     ,操作内容");
                    sql.AppendLine("     ,エラーフラグ");
                    sql.AppendLine("     ,CASE ISNULL(エラーフラグ, 0) WHEN 0 THEN '正常' ELSE 'エラー' END AS 履歴タイプ");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     システム操作履歴");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_operationHistoryCriteria.操作日時From != null)
                    {
                        sql.AppendLine(" and 操作日時 >= @操作日時From ");
                        cmd.Parameters.Add("@操作日時From", SqlDbType.DateTime).Value = _operationHistoryCriteria.操作日時From;
                    }

                    if (_operationHistoryCriteria.操作日時To != null)
                    {
                        sql.AppendLine(" and 操作日時 <= @操作日時To ");
                        cmd.Parameters.Add("@操作日時To", SqlDbType.DateTime).Value = _operationHistoryCriteria.操作日時To;
                    }

                    // 検索ワード（スペースで複数ワードをANDでつなげる）
                    if(_operationHistoryCriteria.操作内容 != null) {

                        //全角スペースをすべて半角スペースへ（これがセパレーターとなる）
                        string word = _operationHistoryCriteria.操作内容.Replace("　", " ");

                        //セパレーターで配列へ変換
                        string[] words = word.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

                        //SQL作成
                        if (words.Length > 0)
                        {
                            int cnt = 0;
                            foreach (string value in words)
                            {
                                sql.AppendFormat(" AND 操作内容 LIKE @操作内容{0}  escape '#' ",cnt);
                                cmd.Parameters.Add("@操作内容" + cnt.ToString(), SqlDbType.VarChar).Value = DBUtil.EscapeString(value);
                                cnt++;
                            }
                        }
                    }

                    if(this.rdoHistoryTypeClean.Checked == true)
                    {
                        sql.Append(" AND エラーフラグ = 0 ");
                    } else if(this.rdoHistoryTypeError.Checked == true)
                    {
                        sql.Append(" AND エラーフラグ = 1 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("操作履歴");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "操作履歴";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("操作履歴を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(OPERATION_DATETIME_COLUMN, "操作日時");
                fpSpread1_Sheet1.BindDataColumn(IP_ADDRESS_COLUMN,         "IPv4アドレス");
                fpSpread1_Sheet1.BindDataColumn(OPERATION_DETAIL_COLUMN,   "操作内容");
                fpSpread1_Sheet1.BindDataColumn(HISTORY_TYPE_COLUMN,       "履歴タイプ");
                fpSpread1_Sheet1.BindDataColumn(ERROR_FLG_COLUMN,          "エラーフラグ");

                // エラーフラグの値に応じて行の色を変える
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].ResetForeColor();
                    if (fpSpread1_Sheet1.Cells[rowIndex, ERROR_FLG_COLUMN].Value as bool? == true)
                    {
                        fpSpread1_Sheet1.Rows[rowIndex].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                        fpSpread1_Sheet1.Rows[rowIndex].ForeColor = System.Drawing.Color.DarkRed;
                    }
                }

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
                _operationHistoryCriteria = new OperationHistoryCriteria();

                //操作時間
                _operationHistoryCriteria.操作日From = DateTime.Today;
                _operationHistoryCriteria.操作時刻From = DateTime.Today;
                _operationHistoryCriteria.操作日To = DateTime.Today;
                _operationHistoryCriteria.操作時刻To = new DateTime(1,1,1,23,59,59);

                courseBindingSource.DataSource = _operationHistoryCriteria;

                rdoHistoryTypeAll.Checked = true;

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

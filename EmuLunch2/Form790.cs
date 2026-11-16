using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// マスタエクスポート
    /// </summary>
    public partial class Form790 : BaseForm
    {
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// マスタエクスポート条件
        /// </summary>
        private MasterExportCriteria _masterExportCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form790()
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
        private void Form790_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                // 権限によりボタンを制御
                btnExport.Enabled = this.HasAuthEdit;

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _masterExportCriteria = new MasterExportCriteria();
                masterExportCriteriaBindingSource.DataSource = _masterExportCriteria;
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
        private void Form790_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// エクスポートボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_masterExportCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_masterExportCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _masterExportCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                Export();       // エクスポート
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
        /// エクスポート
        /// </summary>
        private void Export()
        {
            try
            {
                if (_masterExportCriteria.部署マスタ == true)
                {
                    ExportMain("部署マスタ");
                }

                if (_masterExportCriteria.得意先マスタ == true)
                {
                    ExportMain("得意先マスタ");
                }

                if (_masterExportCriteria.得意先単価マスタ == true)
                {
                    ExportMain("得意先単価マスタ");
                }

                if (_masterExportCriteria.得意先単価適用マスタ == true)
                {
                    ExportMain("得意先単価適用マスタ");
                }

                if (_masterExportCriteria.商品マスタ == true)
                {
                    ExportMain("商品マスタ");
                }

                if (_masterExportCriteria.商品グループマスタ == true)
                {
                    ExportMain("商品グループマスタ");
                }

                if (_masterExportCriteria.主要商品リストマスタ == true)
                {
                    ExportMain("主要商品リストマスタ");
                }

                if (_masterExportCriteria.主要部品リストマスタ == true)
                {
                    ExportMain("主要部品リストマスタ");
                }

                if (_masterExportCriteria.コースマスタ == true)
                {
                    ExportMain("コースマスタ");
                }

                if (_masterExportCriteria.コース別グループマスタ == true)
                {
                    ExportMain("コース別グループマスタ");
                }

                if (_masterExportCriteria.担当者マスタ == true)
                {
                    ExportMain("担当者マスタ");
                }

                if (_masterExportCriteria.担当者架電コースマスタ == true)
                {
                    ExportMain("担当者架電コースマスタ");
                }

                if (_masterExportCriteria.コーステーブル == true)
                {
                    ExportMain("コーステーブル");
                }

                MessageBox.Show(this, "エクスポートが完了しました。", this.Text, MessageBoxButtons.OK);
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
        /// エクスポート主処理
        /// </summary>
        /// <param name="targetName">テーブル名</param>
        private void ExportMain(string targetName)
        {
            try
            {
                // 出力ファイル名を取得
                saveFileDialog1.FileName = targetName;
                if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                string filePath = saveFileDialog1.FileName;

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 対象テーブル取得
                DataTable dt = GetTable(targetName);

                // CSV出力
                OutputCSV(dt, targetName, filePath, chkOutputHeader.Checked);

                log.InfoFormat("{0}をエクスポートしました。", targetName);
                History.Save(this.Text, targetName, "", History.ProcessType.Output);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 対象テーブル取得
        /// </summary>
        /// <param name="targetName">テーブル名</param>
        /// <returns>対象テーブルの内容を格納したデータテーブル</returns>
        private DataTable GetTable(string targetName)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    string sql = "\r\n SELECT" +
                                 "\r\n     *" +
                                 "\r\n FROM" +
                                 "\r\n     {0}" +
                                 "\r\n WHERE" +
                                 "\r\n     企業ＣＤ = @企業ＣＤ";
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(sql, targetName);

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable(targetName);
                    adp.Fill(dt);
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
        /// CSV出力
        /// </summary>
        /// <param name="dt">対象テーブル</param>
        /// <param name="targetName">テーブル名</param>
        /// <param name="filePath">出力ファイルパス</param>
        /// <param name="isWriteHeader">ヘッダを出力する場合はtrue</param>
        private void OutputCSV(DataTable dt, string targetName, string filePath, bool isWriteHeader)
        {
            try
            {
                // CSVファイルに書き込むときに使うEncoding
                Encoding enc = System.Text.Encoding.GetEncoding("Shift_JIS");

                // 書き込むファイルを開く
                StreamWriter sr = new System.IO.StreamWriter(filePath, false, enc);

                int colCount = dt.Columns.Count;
                int lastColIndex = colCount - 1;

                // ヘッダを書き込む
                if (isWriteHeader)
                {
                    for (int i = 0; i < colCount; i++)
                    {
                        string field = dt.Columns[i].ColumnName;    // 列名の取得
                        if (field == "企業ＣＤ")
                        {
                            continue;
                        }
                        field = EncloseDoubleQuotes(field);     // "で囲む
                        sr.Write(field);                        // フィールドを書き込む
                        if (lastColIndex > i)                   // カンマを書き込む
                        {
                            sr.Write(',');
                        }
                    }
                    sr.Write("\r\n");   // 改行する
                }

                //レコードを書き込む
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < colCount; i++)
                    {
                        string field = dt.Columns[i].ColumnName;    // 列名の取得
                        if (field == "企業ＣＤ")
                        {
                            continue;
                        }
                        field = row[i].ToString();   // フィールドの取得
                        // 文字列であれば、"で囲む
                        if (dt.Columns[i].DataType == typeof(string))
                        {
                            if (string.IsNullOrEmpty(field) == false)
                            {
                                field = EncloseDoubleQuotes(field);
                            }
                        }
                        sr.Write(field);        // フィールドを書き込む
                        if (lastColIndex > i)   // カンマを書き込む
                        {
                            sr.Write(',');
                        }
                    }
                    sr.Write("\r\n");   // 改行する
                }

                sr.Close();     // 閉じる
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
        /// 文字列をダブルクォートで囲む
        /// </summary>
        private string EncloseDoubleQuotes(string field)
        {
            if (field.IndexOf('"') > -1)
            {
                // "を""とする
                field = field.Replace("\"", "\"\"");
            }
            return "\"" + field + "\"";
        }

        #endregion
    }
}

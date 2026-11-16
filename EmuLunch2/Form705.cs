using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 主要商品リストマスタ一覧
    /// </summary>
    public partial class Form705 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SHITEN_CD_COLUMN   = 0;   // 部署列インデックス
        private const int SHITEN_NAME_COLUMN = 1;   // 商品ＣＤ列インデックス
        private const int BUSYO_CD_COLUMN    = 2;   // 商品名列インデックス
        private const int BUSYO_NAME_COLUMN  = 3;   // 主要商品列インデックス
        private const int GROUP_CNT_COLUMN   = 4;   // グループ数列インデックス
        private const int SYOHIN_CNT_COLUMN  = 5;   // 商品数列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用モバイル主要商品リストデータ
        /// </summary>
        private SyuyoSyohinCriteria _syuyoSyohinCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form705()
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
        private void Form705_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                ComboUtil.SetShiten(cboShiten);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _syuyoSyohinCriteria = new SyuyoSyohinCriteria();

                // 支店CDコンボの初期値を設定
                _syuyoSyohinCriteria.支店ＣＤ = UserData.getInstance().支店ＣＤ ?? -1;

                syuyoSyohinBindingSource.DataSource = _syuyoSyohinCriteria;

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
        private void Form705_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                case Keys.F8:                       // 詳細画面ボタン押下
                    btnDetail.PerformClick();
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
                if (_syuyoSyohinCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_syuyoSyohinCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _syuyoSyohinCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     B.支店ＣＤ,");
                    sql.AppendLine("     S.支店名,");
                    sql.AppendLine("     B.部署ＣＤ ,");
                    sql.AppendLine("     B.部署名,");
                    sql.AppendLine("     ISNULL(A1.グループ数, 0) AS グループ数,");
                    sql.AppendLine("     ISNULL(A2.商品数, 0) AS 商品数");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     支店マスタ AS S");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = S.企業ＣＤ AND B.支店ＣＤ = S.支店ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 ");
                    sql.AppendLine("     LEFT JOIN ");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             A.部署ＣＤ ,");
                    sql.AppendLine("             COUNT(A.商品グループＣＤ) AS グループ数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             (SELECT DISTINCT");
                    sql.AppendLine("                 部署ＣＤ ,");
                    sql.AppendLine("                 商品グループＣＤ");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 主要商品リストマスタ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("             ) AS A");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             A.部署ＣＤ");
                    sql.AppendLine("     ) AS A1 ON A1.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             部署ＣＤ ,");
                    sql.AppendLine("             COUNT(商品ＣＤ) AS 商品数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             主要商品リストマスタ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND ISNULL(無効フラグ, 0) = 0");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             部署ＣＤ");
                    sql.AppendLine("     ) AS A2 ON A1.部署ＣＤ = A2.部署ＣＤ");
                    sql.AppendLine(" ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     S.企業ＣＤ = @企業ＣＤ ");

                    if (_syuyoSyohinCriteria.支店ＣＤ != null && _syuyoSyohinCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine("     AND B.支店ＣＤ = @支店ＣＤ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _syuyoSyohinCriteria.支店ＣＤ;
                    }

                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     B.支店ＣＤ,");
                    sql.AppendLine("     B.部署ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("主要商品リストマスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "主要商品リストマスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("主要商品リストマスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SHITEN_CD_COLUMN, "支店ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SHITEN_NAME_COLUMN, "支店名");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN, "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(GROUP_CNT_COLUMN, "グループ数");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CNT_COLUMN, "商品数");

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
                    fpSpread1_Sheet1.SetActiveCell(0, SHITEN_CD_COLUMN);
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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                SyuyoSyohinCriteria syuyoSyohinCriteria = new SyuyoSyohinCriteria(dr);
                Form705D form = new Form705D(syuyoSyohinCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                SubFormOpenInit(form);
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form705D_FormClosed);
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// 詳細画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form705D_FormClosed(object sender, FormClosedEventArgs e)
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

    }
}

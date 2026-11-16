using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 空き番号検索ダイアログ
    /// </summary>
    public partial class Form700DN : BaseSpreadForm, IDialog
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int AKI_BANGO_COLUMN        = 0;   // 担当者ＣＤ列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件
        /// </summary>
        private TokuisakiAkiBangoCriteria _tokuisakiAkiBangoCriteria;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public int 空き番号 { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form700DN()
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
        private void Form700DN_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.database_gear_ico;

                lblCount.Text = "";

                // 検索ボタンが押下されるまでは非活性
                btnPrev.Enabled = false;
                btnNext.Enabled = false;

                // 件数の選択肢生成
                ComboUtil.Set(cboCount,
                    new string[] { "100", "500", "1000", "5000", "10000" },
                    new int[] { 100, 500, 1000, 5000, 10000 });

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _tokuisakiAkiBangoCriteria = new TokuisakiAkiBangoCriteria();
                _tokuisakiAkiBangoCriteria.件数 = 500;
                akiBangoBindingSource.DataSource = _tokuisakiAkiBangoCriteria;
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
        private void Form700DN_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    btnQuery.PerformClick();        // 検索ボタン押下
                    break;

                // 終了はフォームのCancelButtonなのでEscキー
            }
        }

        /// <summary>
        /// 開始番号テキスト 変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtStart_TextChanged(object sender, EventArgs e)
        {
            // 検索ボタンが押下されるまでは非活性
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
        }

        /// <summary>
        /// 件数コンボボックス 変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 検索ボタンが押下されるまでは非活性
            btnPrev.Enabled = false;
            btnNext.Enabled = false;

            if (_tokuisakiAkiBangoCriteria != null)
            {
                btnPrev.Text = String.Format("前の{0}件", _tokuisakiAkiBangoCriteria.件数);
                btnNext.Text = String.Format("次の{0}件", _tokuisakiAkiBangoCriteria.件数);
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
                if (_tokuisakiAkiBangoCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tokuisakiAkiBangoCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tokuisakiAkiBangoCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = "\r\n WITH CTE(行連番) AS" +
                                      "\r\n (SELECT" +
                                      "\r\n     @開始番号" +
                                      "\r\n UNION ALL" +
                                      "\r\n SELECT" +
                                      "\r\n     行連番 + 1" +
                                      "\r\n FROM" +
                                      "\r\n     CTE" +
                                      "\r\n )" +
                                      "\r\n SELECT" +
                                      "\r\n     T1.digits" +
                                      "\r\n FROM" +
                                      "\r\n     (SELECT TOP (@件数)" +
                                      "\r\n         行連番 AS digits" +
                                      "\r\n     FROM" +
                                      "\r\n         CTE" +
                                      "\r\n     ) T1" +
                                      "\r\n     LEFT JOIN" +
                                      "\r\n         (SELECT" +
                                      "\r\n             得意先ＣＤ" +
                                      "\r\n         FROM" +
                                      "\r\n             得意先マスタ" +
                                      "\r\n         WHERE" +
                                      "\r\n             企業ＣＤ = @企業ＣＤ" +
                                      "\r\n         ) AS TOK ON T1.digits = TOK.得意先ＣＤ" +
                                      "\r\n WHERE" +
                                      "\r\n     T1.digits BETWEEN @開始番号 AND @終了番号" +
                                      "\r\n     AND T1.digits <= 999999999" +       // -- 取得するのは999999999(9桁)まで
                                      "\r\n     AND TOK.得意先ＣＤ IS NULL" +
                                      "\r\n ORDER BY" +
                                      "\r\n     digits" +
                                      "\r\n OPTION (MAXRECURSION 0)";

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@開始番号", SqlDbType.Int).Value = _tokuisakiAkiBangoCriteria.開始番号;
                    cmd.Parameters.Add("@件数", SqlDbType.Int).Value = _tokuisakiAkiBangoCriteria.件数;
                    cmd.Parameters.Add("@終了番号", SqlDbType.Int).Value = _tokuisakiAkiBangoCriteria.終了番号;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("空き番号");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "空き番号";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("得意先マスタの空き番号を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(AKI_BANGO_COLUMN, "digits");

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                btnPrev.Enabled = true;
                btnNext.Enabled = true;
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
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 前のn件ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            _tokuisakiAkiBangoCriteria.開始番号 -= _tokuisakiAkiBangoCriteria.件数;
            if (_tokuisakiAkiBangoCriteria.開始番号 < 0)
            {
                _tokuisakiAkiBangoCriteria.開始番号 = 0;
            }
            ReadBinding(txtStart);
            btnQuery.PerformClick();
        }

        /// <summary>
        /// 次のn件ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            _tokuisakiAkiBangoCriteria.開始番号 += _tokuisakiAkiBangoCriteria.件数;
            if (_tokuisakiAkiBangoCriteria.開始番号 > 999999999)
            {
                _tokuisakiAkiBangoCriteria.開始番号 = 999999999;
            }
            ReadBinding(txtStart);
            btnQuery.PerformClick();
        }

        /// <summary>
        /// 選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
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
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                空き番号 = (int)dr["digits"];
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
                throw new EmuException(ex);
            }
        }

        #endregion
    }
}

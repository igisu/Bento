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
    /// 商品選択ダイアログ
    /// </summary>
    public partial class Form901 : BaseSpreadForm, IDialog
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYOHIN_CD_COLUMN   = 0;   // 商品ＣＤ列インデックス
        private const int BUSYO_GROUP_COLUMN = 1;   // 部署グループ列インデックス
        private const int SYOHIN_NAME_COLUMN = 2;   // 商品名列インデックス
        private const int TANKA_KOMI_COLUMN  = 3;   // 商品単価税込列インデックス
        private const int TANKA_NUKI_COLUMN  = 4;   // 商品単価税抜列インデックス
        private const int BENTO_KBN_COLUMN   = 5;   // 弁当区分列インデックス
        private const int SUBDIDH_COLUMN     = 6;   // 副食ＣＤ列インデックス
        private const int MAINDISH_COLUMN    = 7;   // 主食ＣＤ列インデックス
        private const int SYOKUJI_COLUMN     = 8;   // 食事区分列インデックス
        private const int KEIGEN_COLUMN      = 9;   // 軽減税率列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用商品データ
        /// </summary>
        private SyohinCriteria _syohinCriteria;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public Syohin SelectedSyohin { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form901()
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
        private void Form901_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.magnifier_ico;

                lblCount.Text = "";

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _syohinCriteria = new SyohinCriteria();
                syohinBindingSource.DataSource = _syohinCriteria;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyoGroup(cboBusyoGroup);
                ComboUtil.SetBentoKubun(cboBentoKbn);
                ComboUtil.SetSubDishCD(cboSubDishCD);
                ComboUtil.SetMainDishCD(cboMainDishCD);
                ComboUtil.SetSyokujiKubun(cboSyokujiKbn);

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "商品CD", "部署グループ", "商品名", "商品単価", "弁当区分", "副食", "主食", "食事区分" },
                    new string[] { "A.商品CD {0}", "A.部署グループ {0}, A.商品CD", "A.商品名 {0}, A.商品CD", "A.商品単価税込 {0}, A.商品CD",
                                   "A.弁当区分 {0}, A.商品CD", "A.副食ＣＤ {0}, A.商品CD", "A.主食ＣＤ {0}, A.商品CD", "A.食事区分 {0}, A.商品CD" });

                ComboUtil.Set(cboListMax,
                    new string[] { "100", "500", "1000", "無制限" },
                    new int[] { 100, 500, 1000, -1 });

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
        private void Form901_KeyDown(object sender, KeyEventArgs e)
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
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_syohinCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_syohinCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _syohinCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

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
                    sql.AppendLine("     ,A.商品ＣＤ");
                    sql.AppendLine("     ,A.部署グループ");
                    sql.AppendLine("     ,BG.部署グループ名");
                    sql.AppendLine("     ,A.商品名");
                    sql.AppendLine("     ,A.商品名略称");
                    sql.AppendLine("     ,A.商品名記号");
                    sql.AppendLine("     ,A.商品単価税込");
                    sql.AppendLine("     ,A.商品単価税抜");
                    sql.AppendLine("     ,A.弁当区分");
                    sql.AppendLine("     ,BK.弁当区分名");
                    sql.AppendLine("     ,A.副食ＣＤ");
                    sql.AppendLine("     ,S1.商品名 AS 副食名");
                    sql.AppendLine("     ,A.主食ＣＤ");
                    sql.AppendLine("     ,S2.商品名 AS 主食名");
                    sql.AppendLine("     ,A.部数単位");
                    sql.AppendLine("     ,A.食事区分");
                    sql.AppendLine("     ,JK.食事区分名");
                    sql.AppendLine("     ,A.軽減税率フラグ");
                    sql.AppendLine("     ,case when isnull(A.軽減税率フラグ, 0) <> 0 then '○' else null end as 軽減税率対象");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     商品マスタ AS A");
                    sql.AppendLine("     LEFT JOIN V_部署グループ AS BG ON A.部署グループ = BG.部署グループCD");
                    sql.AppendLine("     LEFT JOIN V_弁当区分 AS BK ON A.弁当区分 = BK.弁当区分CD");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS S1 ON S1.企業ＣＤ = @企業ＣＤ AND A.副食ＣＤ = S1.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN 商品マスタ AS S2 ON S2.企業ＣＤ = @企業ＣＤ AND A.主食ＣＤ = S2.商品ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_食事区分 AS JK ON A.食事区分 = JK.食事区分CD");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(A.無効フラグ, 0) = 0");
                    sql.AppendLine(" AND A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_syohinCriteria.商品ＣＤ != null)
                    {
                        sql.AppendLine(" and A.商品ＣＤ = @商品ＣＤ ");
                        cmd.Parameters.Add("@商品ＣＤ", SqlDbType.Int).Value = _syohinCriteria.商品ＣＤ;
                    }

                    if (_syohinCriteria.部署グループ != null && _syohinCriteria.部署グループ != -1)
                    {
                        sql.AppendLine(" and A.部署グループ = @部署グループ ");
                        cmd.Parameters.Add("@部署グループ", SqlDbType.Int).Value = _syohinCriteria.部署グループ;
                    }

                    if (_syohinCriteria.商品名 != null)
                    {
                        sql.AppendLine(" and (A.商品名 like @商品名 escape '#' OR A.商品名略称 like @商品名略称 escape '#' OR A.商品名記号 like @商品名記号 escape '#')");
                        cmd.Parameters.Add("@商品名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_syohinCriteria.商品名);
                        cmd.Parameters.Add("@商品名略称", SqlDbType.VarChar).Value = DBUtil.EscapeString(_syohinCriteria.商品名);
                        cmd.Parameters.Add("@商品名記号", SqlDbType.VarChar).Value = DBUtil.EscapeString(_syohinCriteria.商品名);
                    }

                    if (_syohinCriteria.商品単価From != null)
                    {
                        sql.AppendLine(" and (A.商品単価税込 >= @商品単価From OR A.商品単価税抜 >= @商品単価From)");
                        cmd.Parameters.Add("@商品単価From", SqlDbType.Decimal).Value = _syohinCriteria.商品単価From;
                    }

                    if (_syohinCriteria.商品単価To != null)
                    {
                        sql.AppendLine(" and (A.商品単価税込 <= @商品単価To OR A.商品単価税抜 <= @商品単価To)");
                        cmd.Parameters.Add("@商品単価To", SqlDbType.Decimal).Value = _syohinCriteria.商品単価To;
                    }

                    if (_syohinCriteria.弁当区分 != null && _syohinCriteria.弁当区分 != -1)
                    {
                        sql.AppendLine(" and A.弁当区分 = @弁当区分 ");
                        cmd.Parameters.Add("@弁当区分", SqlDbType.Int).Value = _syohinCriteria.弁当区分;
                    }

                    if (_syohinCriteria.副食ＣＤ != null && _syohinCriteria.副食ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.副食ＣＤ = @副食ＣＤ ");
                        cmd.Parameters.Add("@副食ＣＤ", SqlDbType.Int).Value = _syohinCriteria.副食ＣＤ;
                    }

                    if (_syohinCriteria.主食ＣＤ != null && _syohinCriteria.主食ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.主食ＣＤ = @主食ＣＤ ");
                        cmd.Parameters.Add("@主食ＣＤ", SqlDbType.Int).Value = _syohinCriteria.主食ＣＤ;
                    }

                    if (_syohinCriteria.食事区分 != null && _syohinCriteria.食事区分 != -1)
                    {
                        sql.AppendLine(" and A.食事区分 = @食事区分 ");
                        cmd.Parameters.Add("@食事区分", SqlDbType.Int).Value = _syohinCriteria.食事区分;
                    }

                    if (this.rdoKeigenYes.Checked == true)
                    {
                        sql.AppendLine(" and ISNULL(A.軽減税率フラグ, 0) = 1 ");
                    }
                    if (this.rdoKeigenNo.Checked == true)
                    {
                        sql.AppendLine(" and ISNULL(A.軽減税率フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("商品マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "商品マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("商品マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN,   "商品ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_GROUP_COLUMN, "部署グループ名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "商品名");
                fpSpread1_Sheet1.BindDataColumn(TANKA_KOMI_COLUMN,  "商品単価税込");
                fpSpread1_Sheet1.BindDataColumn(TANKA_NUKI_COLUMN,  "商品単価税抜");
                fpSpread1_Sheet1.BindDataColumn(BENTO_KBN_COLUMN,   "弁当区分名");
                fpSpread1_Sheet1.BindDataColumn(SUBDIDH_COLUMN,     "副食名");
                fpSpread1_Sheet1.BindDataColumn(MAINDISH_COLUMN,    "主食名");
                fpSpread1_Sheet1.BindDataColumn(SYOKUJI_COLUMN,     "食事区分名");
                fpSpread1_Sheet1.BindDataColumn(KEIGEN_COLUMN,      "軽減税率対象");

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
                this.Cursor = Cursors.Default;
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
                _syohinCriteria = new SyohinCriteria();
                syohinBindingSource.DataSource = _syohinCriteria;

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
        /// 詳細表示（既存レコード）
        /// </summary>
        private void ShowDetail()
        {
            try
            {
                DataRow dr = ((DataRowView)bindingSource1.Current).Row;
                SelectedSyohin = new Syohin(dr);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
                throw new BentoException(ex);
            }
        }

        #endregion
    }
}

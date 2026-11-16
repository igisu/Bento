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
    /// 担当者選択ダイアログ
    /// </summary>
    public partial class Form904 : BaseSpreadForm, IDialog
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int TANTOUSYA_CD_COLUMN        = 0;   // 担当者ＣＤ列インデックス
        private const int TANTOUSYA_NAME_COLUMN      = 1;   // 担当者名列インデックス
        private const int TANTOUSYA_NAME_KANA_COLUMN = 2;   // 担当者名カナ名列インデックス
        private const int BUSYO_COLUMN               = 3;   // 部署列インデックス
        private const int KENGEN_COLUMN              = 4;   // 権限列インデックス
        private const int EIGYO_KBN_COLUMN           = 5;   // 営業業務区分列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用担当者データ
        /// </summary>
        private TantousyaCriteria _tantousyaCriteria;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public Tantousya SelectedTantousya { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form904()
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
        private void Form904_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.magnifier_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);
                ComboUtil.SetEigyoKubun(cboEigyoKbn);
                ComboUtil.SetKengen(cboKengen);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _tantousyaCriteria = new TantousyaCriteria();

                // 部署CDコンボの初期値を設定
                _tantousyaCriteria.所属部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                tantousyaBindingSource.DataSource = _tantousyaCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "担当者CD", "担当者名", "担当者名カナ", "所属部署", "権限区分", "営業業務区分" },
                    new string[] { "A.担当者CD {0}", "A.担当者名 {0}, A.担当者CD", "A.担当者名カナ {0}, A.担当者CD",
                                   "B.部署CD {0}, A.担当者CD", "KK.表示順 {0}, A.担当者CD", "EK.表示順 {0}, A.担当者CD" });

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
        private void Form904_KeyDown(object sender, KeyEventArgs e)
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
                if (_tantousyaCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_tantousyaCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _tantousyaCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     ,A.担当者ＣＤ");
                    sql.AppendLine("     ,A.担当者名");
                    sql.AppendLine("     ,A.担当者名カナ");
                    sql.AppendLine("     ,A.郵便番号");
                    sql.AppendLine("     ,A.住所");
                    sql.AppendLine("     ,A.電話番号");
                    sql.AppendLine("     ,A.ＦＡＸ");
                    sql.AppendLine("     ,A.所属部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,A.ユーザーＩＤ");
                    sql.AppendLine("     ,A.ユーザーＰＷ");
                    sql.AppendLine("     ,A.権限区分");
                    sql.AppendLine("     ,KK.権限区分名");
                    sql.AppendLine("     ,A.営業業務区分");
                    sql.AppendLine("     ,EK.営業業務区分名");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     担当者マスタ AS A");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.所属部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN V_権限区分 AS KK ON A.権限区分 = KK.権限区分CD");
                    sql.AppendLine("     LEFT JOIN V_営業業務区分 AS EK ON A.営業業務区分 = EK.営業業務区分CD");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(A.無効フラグ, 0) = 0");
                    sql.AppendLine(" AND A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_tantousyaCriteria.担当者ＣＤ != null)
                    {
                        sql.AppendLine(" and A.担当者ＣＤ = @担当者ＣＤ ");
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = _tantousyaCriteria.担当者ＣＤ;
                    }

                    if (_tantousyaCriteria.担当者名 != null)
                    {
                        sql.AppendLine(" and (A.担当者名 like @担当者名 escape '#' OR A.担当者名カナ like @担当者名カナ escape '#')");
                        cmd.Parameters.Add("@担当者名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tantousyaCriteria.担当者名);
                        cmd.Parameters.Add("@担当者名カナ", SqlDbType.VarChar).Value = DBUtil.EscapeString(_tantousyaCriteria.担当者名);
                    }

                    if (_tantousyaCriteria.所属部署ＣＤ != null && _tantousyaCriteria.所属部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.所属部署ＣＤ = @所属部署ＣＤ ");
                        cmd.Parameters.Add("@所属部署ＣＤ", SqlDbType.Int).Value = _tantousyaCriteria.所属部署ＣＤ;
                    }

                    if (_tantousyaCriteria.権限区分 != null && _tantousyaCriteria.権限区分 != -1)
                    {
                        sql.AppendLine(" and A.権限区分 = @権限区分 ");
                        cmd.Parameters.Add("@権限区分", SqlDbType.Int).Value = _tantousyaCriteria.権限区分;
                    }

                    if (_tantousyaCriteria.営業業務区分 != null && _tantousyaCriteria.営業業務区分 != -1)
                    {
                        sql.AppendLine(" and A.営業業務区分 = @営業業務区分 ");
                        cmd.Parameters.Add("@営業業務区分", SqlDbType.Int).Value = _tantousyaCriteria.営業業務区分;
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("担当者マスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "担当者マスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("担当者マスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(TANTOUSYA_CD_COLUMN, "担当者ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TANTOUSYA_NAME_COLUMN, "担当者名");
                fpSpread1_Sheet1.BindDataColumn(TANTOUSYA_NAME_KANA_COLUMN, "担当者名カナ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(KENGEN_COLUMN, "権限区分名");
                fpSpread1_Sheet1.BindDataColumn(EIGYO_KBN_COLUMN, "営業業務区分名");

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
                _tantousyaCriteria = new TantousyaCriteria();

                // 部署CDコンボの初期値を設定
                _tantousyaCriteria.所属部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                tantousyaBindingSource.DataSource = _tantousyaCriteria;

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
                SelectedTantousya = new Tantousya(dr);
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

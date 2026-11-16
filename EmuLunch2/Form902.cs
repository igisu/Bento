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
    /// コース選択ダイアログ
    /// </summary>
    public partial class Form902 : BaseSpreadForm, IDialog
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SYOHIN_CD_COLUMN   = 0;   // コースＣＤ列インデックス
        private const int BUSYO_GROUP_COLUMN = 1;   // コース区分列インデックス
        private const int SYOHIN_NAME_COLUMN = 2;   // コース名列インデックス
        private const int SYOHIN_KBNCOLUMN   = 3;   // 部署名列インデックス
        private const int TANKA_COLUMN       = 4;   // 担当者列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 検索条件入力用コースマスタ
        /// </summary>
        private CourseCriteria _courseCriteria;

        /// <summary>
        /// 検索対象部署ＣＤ
        /// </summary>
        private int? _busyoCD = null;
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        public Course SelectedCourse { get; private set; }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form902()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="busyoCD">検索対象部署CD</param>
        public Form902(int busyoCD) : this()
        {
            _busyoCD = busyoCD;
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
        private void Form902_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.magnifier_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetCourseKubun(cboCourseKbn);
                ComboUtil.SetBusyo(cboBusyo);

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _courseCriteria = new CourseCriteria();

                // 部署CDコンボの初期値を設定
                if (_busyoCD == null)
                {
                    _courseCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
                }
                else
                {
                    _courseCriteria.部署ＣＤ = _busyoCD;
                }

                courseBindingSource.DataSource = _courseCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "コースCD", "コース区分", "コース名", "部署", "担当者" },
                    new string[] { "A.コースＣＤ {0}", "VC.表示順 {0}, A.コースＣＤ", "A.コース名 {0}, A.コースＣＤ",
                                   "A.部署ＣＤ {0}, A.コースＣＤ", "A.担当者ＣＤ {0}, A.コースＣＤ" });

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
        private void Form902_KeyDown(object sender, KeyEventArgs e)
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
        /// 担当者ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTantosyaCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnTantosyaCD.PerformClick();
            }
        }

        /// <summary>
        /// 担当者ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTantosyaCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 担当者選択ダイアログを表示
                Form904 formTantosyaDlg = new Form904();
                SubFormOpenInit(formTantosyaDlg);
                formTantosyaDlg.FormClosed += new FormClosedEventHandler(Form904TantosyaCD_FormClosed);
                formTantosyaDlg.ShowDialog();
                if (formTantosyaDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 担当者選択ダイアログクローズ（担当者ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form904TantosyaCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form904 form = (Form904)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtTantosyaCD, null);  // エラーを消す

                    Tantousya tantousya = form.SelectedTantousya;
                    txtTantosyaCD.Text = tantousya.担当者ＣＤ.ToString();
                    _courseCriteria.担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    lblTantosyaName.Text = tantousya.担当者名;
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
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900TokuisakiCD_FormClosed);
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
        private void Form900TokuisakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    txtTokuisakiCD.Text = tokuisaki.得意先ＣＤ.ToString();
                    _courseCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    lblTokuisakiName.Text = tokuisaki.得意先名;
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
                if (_courseCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_courseCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _courseCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        sql.AppendLine("SELECT DISTINCT");
                    }
                    else
                    {
                        sql.AppendFormat("SELECT DISTINCT TOP({0})", cboListMax.SelectedValue);
                    }
                    sql.AppendLine("     A.企業ＣＤ");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,A.コース区分");
                    sql.AppendLine("     ,VC.コース区分名");
                    sql.AppendLine("     ,VC.表示順");
                    sql.AppendLine("     ,A.コース名");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,B.部署名");
                    sql.AppendLine("     ,A.担当者ＣＤ");
                    sql.AppendLine("     ,A3.担当者名");
                    sql.AppendLine("     ,A.工場フラグ");
                    sql.AppendLine("     ,A.無効フラグ");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     コースマスタ AS A");
                    sql.AppendLine("     LEFT JOIN コーステーブル AS T ON T.企業ＣＤ = @企業ＣＤ AND ISNULL(T.無効フラグ, 0) = 0 AND A.コースＣＤ = T.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN V_コース区分 AS VC ON A.コース区分 = VC.コース区分CD");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON B.企業ＣＤ = @企業ＣＤ AND ISNULL(B.無効フラグ, 0) = 0 AND A.部署ＣＤ = B.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = @企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = @企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A3 ON A3.企業ＣＤ = @企業ＣＤ AND A.担当者ＣＤ = A3.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     ISNULL(A.無効フラグ, 0) = 0");
                    sql.AppendLine(" AND A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_courseCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine(" and A.コースＣＤ = @コースＣＤ ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _courseCriteria.コースＣＤ;
                    }

                    if (_courseCriteria.コース区分 != null && _courseCriteria.コース区分 != -1)
                    {
                        sql.AppendLine(" and A.コース区分 = @コース区分 ");
                        cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = _courseCriteria.コース区分;
                    }

                    if (_courseCriteria.コース名 != null)
                    {
                        sql.AppendLine(" and A.コース名 like @コース名 escape '#'");
                        cmd.Parameters.Add("@コース名", SqlDbType.VarChar).Value = DBUtil.EscapeString(_courseCriteria.コース名);
                    }

                    if (_courseCriteria.部署ＣＤ != null && _courseCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _courseCriteria.部署ＣＤ;
                    }

                    if (_courseCriteria.担当者ＣＤ != null)
                    {
                        sql.AppendLine(" and A.担当者ＣＤ = @担当者ＣＤ ");
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = _courseCriteria.担当者ＣＤ;
                    }

                    if (_courseCriteria.得意先ＣＤ != null)
                    {
                        sql.AppendLine(" and T.得意先ＣＤ = @得意先ＣＤ ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _courseCriteria.得意先ＣＤ;
                    }

                    // チェックONの場合は全て検索するので条件に加えない（つまりチェックOFFの場合は工場フラグが0のものだけを検索）
                    if (_courseCriteria.工場フラグ != null && _courseCriteria.工場フラグ !=false)
                    {
                        sql.AppendLine(" and ISNULL(A.工場フラグ, 0) = 0 ");
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("コースマスタ");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    fpSpread1_Sheet1.Rows.Count = 0;
                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "コースマスタ";

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    log.Info("コースマスタを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SYOHIN_CD_COLUMN, "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_GROUP_COLUMN, "コース区分名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_NAME_COLUMN, "コース名");
                fpSpread1_Sheet1.BindDataColumn(SYOHIN_KBNCOLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(TANKA_COLUMN, "担当者名");

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
                _courseCriteria = new CourseCriteria();

                // 部署CDコンボの初期値を設定
                if (_busyoCD == null)
                {
                    _courseCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;
                }
                else
                {
                    _courseCriteria.部署ＣＤ = _busyoCD;
                }

                courseBindingSource.DataSource = _courseCriteria;

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
                SelectedCourse = new Course(dr);
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

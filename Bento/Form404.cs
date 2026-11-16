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
    /// 売上精算一覧
    /// </summary>
    public partial class Form404 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int SEISANBI_COLUMN          = 0;   // 精算日列インデックス
        private const int COURSE_CD_COLUMN         = 1;   // コースＣＤ列インデックス
        private const int COURSE_NAME_COLUMN       = 2;   // コース名列インデックス
        private const int TANTOSYA_CD_COLUMN       = 3;   // 得意先ＣＤ列インデックス
        private const int TANTOSYA_NAME_COLUMN     = 4;   // 得意先名列インデックス
        private const int GENKIN_TOTAL_COLUMN      = 5;   // 現金合計列インデックス
        private const int GENKIN_NEBIKI_COLUMN     = 6;   // 現金値引列インデックス
        private const int KAKEURI_TOTAL_COLUMN     = 7;   // 掛売合計列インデックス
        private const int KAKEURI_NEBIKI_COLUMN    = 8;   // 掛売値引列インデックス
        private const int PAY_TOTAL_COLUMN         = 9;   // 電子決済合計列インデックス
        private const int PAY_NEBIKI_COLUMN        = 10;  // 電子決済値引列インデックス
        private const int TOTAL_SALE_COLUMN        = 11;  // 総売上額列インデックス
        private const int TOTAL_NEBIKI_COLUMN      = 12;  // 総値引額列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 売上精算一覧条件入力
        /// </summary>
        private UriageSeisanListCriteria _uriageSeisanListCriteria;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form404()
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
        private void Form404_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.lorry_go_ico;

                lblCount.Text = "";

                // 権限によりボタンを制御
                btnNew.Enabled = this.HasAuthEdit;
                btnDetail.Enabled = this.HasAuthBrowse;

                // コンボボックスの選択肢生成
                cboShiten.SelectedValueChanged -= cboShiten_SelectedValueChanged;   // 支店ＣＤコンボイベントをいったん削除

                ComboUtil.SetShiten(cboShiten);
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _uriageSeisanListCriteria = new UriageSeisanListCriteria();

                // 部署CDコンボの初期値を設定
                _uriageSeisanListCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                uriageSeisanListBindingSource.DataSource = _uriageSeisanListCriteria;

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "精算日", "コースCD", "担当者CD" },
                    new string[] { "A.精算日 {0}, A.コースＣＤ",
                                   "A.コースＣＤ {0}, A.精算日",
                                   "A.配送担当者ＣＤ {0}, A.精算日"});

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
            finally
            {
                Application.DoEvents();         // バインドしたことで発生するSelectedValueChangedをメッセージキューから消す
                cboShiten.SelectedValueChanged += new EventHandler(cboShiten_SelectedValueChanged);  // 支店ＣＤコンボイベントを追加
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form404_KeyDown(object sender, KeyEventArgs e)
        {
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

        /// <summary>
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form404_Shown(object sender, EventArgs e)
        {
            // 精算日にフォーカス
            dtpSeisanbiFrom.Focus();
        }

        /// <summary>
        /// 支店コンボ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboShiten_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // 部署コンボの選択肢を、支店にぶら下がるものに再設定する
                ComboUtil.SetBusyo(cboBusyo, (int?)cboShiten.SelectedValue);

                // 部署コンボを初期化
                _uriageSeisanListCriteria.部署ＣＤ = -1;
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
        /// コースＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCourseCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnCourseCD.PerformClick();
            }
        }

        /// <summary>
        /// コースＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCourseCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // コース選択ダイアログを表示
                Form902 formCourseDlg = new Form902();
                SubFormOpenInit(formCourseDlg);
                formCourseDlg.FormClosed += new FormClosedEventHandler(Form902CourseCD_FormClosed);
                formCourseDlg.ShowDialog();
                if (formCourseDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// コース選択ダイアログクローズ（コースＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form902CourseCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form902 form = (Form902)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtCourseCD, null);  // エラーを消す

                    Course course = form.SelectedCourse;
                    txtCourseCD.Text = course.コースＣＤ.ToString();
                    _uriageSeisanListCriteria.コースＣＤ = course.コースＣＤ;       // 入力内容を確定させる
                    lblCourseName.Text = course.コース名;
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
                    //if (_chumonHeader != null)
                    //{
                    //    _chumonHeader.担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    //}
                    lblTantosyaName.Text = tantousya.担当者名;
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
                if (_uriageSeisanListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageSeisanListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _uriageSeisanListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    sql.AppendLine("     ,A.精算日");
                    sql.AppendLine("     ,A.部署ＣＤ");
                    sql.AppendLine("     ,A.コースＣＤ");
                    sql.AppendLine("     ,A.配送担当者ＣＤ");
                    sql.AppendLine("     ,A.配送担当者名");
                    sql.AppendLine("     ,A.部署名");
                    sql.AppendLine("     ,A.コース名");
                    sql.AppendLine("     ,A.得意先数");
                    sql.AppendLine("     ,A.現金_売上合計");
                    sql.AppendLine("     ,A.現金_値引合計");
                    sql.AppendLine("     ,A.現金_回収現金件数");
                    sql.AppendLine("     ,A.現金_回収現金");
                    sql.AppendLine("     ,A.現金_チケット束代");
                    sql.AppendLine("     ,A.現金_本日現金合計");
                    sql.AppendLine("     ,A.現金_食数");
                    sql.AppendLine("     ,A.掛売_売上合計");
                    sql.AppendLine("     ,A.掛売_値引合計");
                    sql.AppendLine("     ,A.掛売_未集金");
                    sql.AppendLine("     ,A.掛売_未集金外売上");
                    sql.AppendLine("     ,A.掛売_本日掛売合計");
                    sql.AppendLine("     ,A.掛売_食数");
                    sql.AppendLine("     ,A.E_売上合計");
                    sql.AppendLine("     ,A.E_値引合計");
                    sql.AppendLine("     ,A.E_本日E合計");
                    sql.AppendLine("     ,A.E_食数");
                    sql.AppendLine("     ,A.E_回収金額");
                    sql.AppendLine("     ,A.合計_本日総売上");
                    sql.AppendLine("     ,A.合計_本日総値引");
                    sql.AppendLine("     ,A.合計_チケット売上");
                    sql.AppendLine("     ,A.合計_バークレー売上");
                    sql.AppendLine("     ,A.合計_実質総売上");
                    sql.AppendLine("     ,A.消費チケ枚数_チケット");
                    sql.AppendLine("     ,A.消費チケ枚数_ボーナス");
                    sql.AppendLine("     ,A.消費チケ枚数_ＢＶ300");
                    sql.AppendLine("     ,A.消費チケ枚数_ＢＶ200");
                    sql.AppendLine("     ,A.消費チケ金額_ＢＶ300");
                    sql.AppendLine("     ,A.消費チケ金額_ＢＶ200");
                    sql.AppendLine("     ,A.金銭内訳_10000円");
                    sql.AppendLine("     ,A.金銭内訳_5000円");
                    sql.AppendLine("     ,A.金銭内訳_2000円");
                    sql.AppendLine("     ,A.金銭内訳_1000円");
                    sql.AppendLine("     ,A.金銭内訳_500円");
                    sql.AppendLine("     ,A.金銭内訳_100円");
                    sql.AppendLine("     ,A.金銭内訳_50円");
                    sql.AppendLine("     ,A.金銭内訳_10円");
                    sql.AppendLine("     ,A.金銭内訳_5円");
                    sql.AppendLine("     ,A.金銭内訳_1円");
                    sql.AppendLine("     ,A.金銭内訳_小切手");
                    sql.AppendLine("     ,A.金銭内訳_領収書");
                    sql.AppendLine("     ,A.金銭内訳_合計");
                    sql.AppendLine("     ,A.金銭内訳_過剰金");
                    sql.AppendLine("     ,A.チケット束_売上枚数");
                    sql.AppendLine("     ,A.チケット束_売上金額");
                    sql.AppendLine("     ,A.作成担当者ＣＤ");
                    sql.AppendLine("     ,A1.担当者名 AS 作成担当者名");
                    sql.AppendLine("     ,A.作成日");
                    sql.AppendLine("     ,A.修正担当者ＣＤ");
                    sql.AppendLine("     ,A2.担当者名 AS 修正担当者名");
                    sql.AppendLine("     ,A.修正日");
                    sql.AppendLine(" FROM");
                    sql.AppendLine("     売上精算_ヘッダ AS A");
                    sql.AppendLine("     LEFT JOIN 部署マスタ   AS B  ON B.企業ＣＤ = A.企業ＣＤ AND B.部署ＣＤ = A.部署ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A1 ON A1.企業ＣＤ = A.企業ＣＤ AND A.作成担当者ＣＤ = A1.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A2 ON A2.企業ＣＤ = A.企業ＣＤ AND A.修正担当者ＣＤ = A2.担当者ＣＤ");
                    sql.AppendLine("     LEFT JOIN 担当者マスタ AS A3 ON A2.企業ＣＤ = A.企業ＣＤ AND A.配送担当者ＣＤ = A3.担当者ＣＤ");
                    sql.AppendLine(" WHERE");
                    sql.AppendLine("     A.企業ＣＤ = @企業ＣＤ");

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;

                    if (_uriageSeisanListCriteria.支店ＣＤ != null && _uriageSeisanListCriteria.支店ＣＤ != -1)
                    {
                        sql.AppendLine(" and B.支店ＣＤ = @支店ＣＤ ");
                        cmd.Parameters.Add("@支店ＣＤ", SqlDbType.Int).Value = _uriageSeisanListCriteria.支店ＣＤ;
                    }

                    if (_uriageSeisanListCriteria.部署ＣＤ != null && _uriageSeisanListCriteria.部署ＣＤ != -1)
                    {
                        sql.AppendLine(" and A.部署ＣＤ = @部署ＣＤ ");
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageSeisanListCriteria.部署ＣＤ;
                    }

                    if (_uriageSeisanListCriteria.精算日From != null)
                    {
                        sql.AppendLine(" and A.精算日 >= @精算日From ");
                        cmd.Parameters.Add("@精算日From", SqlDbType.Date).Value = _uriageSeisanListCriteria.精算日From;
                    }

                    if (_uriageSeisanListCriteria.精算日To != null)
                    {
                        sql.AppendLine(" and A.精算日 <= @精算日To ");
                        cmd.Parameters.Add("@精算日To", SqlDbType.Date).Value = _uriageSeisanListCriteria.精算日To;
                    }

                    if (_uriageSeisanListCriteria.コースＣＤ != null)
                    {
                        sql.AppendLine(" and A.コースＣＤ = @コースＣＤ ");
                        cmd.Parameters.Add("@コースＣＤ", SqlDbType.Int).Value = _uriageSeisanListCriteria.コースＣＤ;
                    }

                    if (_uriageSeisanListCriteria.担当者ＣＤ != null)
                    {
                        sql.AppendLine(" and A.配送担当者ＣＤ = @担当者ＣＤ ");
                        cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = _uriageSeisanListCriteria.担当者ＣＤ;
                    }

                    sql.AppendLine("order by ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("売上精算一覧");
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 合計行を追加する
                    dt.Rows.Add(dt.NewRow());

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "売上精算一覧";

                    log.Info("売上精算一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SEISANBI_COLUMN,       "精算日");
                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(TANTOSYA_CD_COLUMN,    "配送担当者ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TANTOSYA_NAME_COLUMN,  "配送担当者名");
                fpSpread1_Sheet1.BindDataColumn(GENKIN_TOTAL_COLUMN,   "現金_売上合計");
                fpSpread1_Sheet1.BindDataColumn(GENKIN_NEBIKI_COLUMN,  "現金_値引合計");
                fpSpread1_Sheet1.BindDataColumn(KAKEURI_TOTAL_COLUMN,  "掛売_売上合計");
                fpSpread1_Sheet1.BindDataColumn(KAKEURI_NEBIKI_COLUMN, "掛売_値引合計");
                fpSpread1_Sheet1.BindDataColumn(PAY_TOTAL_COLUMN,      "E_売上合計");
                fpSpread1_Sheet1.BindDataColumn(PAY_NEBIKI_COLUMN,     "E_値引合計");
                fpSpread1_Sheet1.BindDataColumn(TOTAL_SALE_COLUMN,     "合計_本日総売上");
                fpSpread1_Sheet1.BindDataColumn(TOTAL_NEBIKI_COLUMN,   "合計_本日総値引");

                // 背景色をリセット
                for (int rowIndex = 0; rowIndex < fpSpread1_Sheet1.Rows.Count; rowIndex++)
                {
                    fpSpread1_Sheet1.Rows[rowIndex].ResetBackColor();
                    fpSpread1_Sheet1.Rows[rowIndex].CanFocus = true;
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, TANTOSYA_NAME_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = GENKIN_TOTAL_COLUMN; columnIndex <= TOTAL_NEBIKI_COLUMN; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
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
                _uriageSeisanListCriteria = new UriageSeisanListCriteria();

                // 部署CDコンボの初期値を設定
                _uriageSeisanListCriteria.部署ＣＤ = UserData.getInstance().所属部署ＣＤ ?? -1;

                uriageSeisanListBindingSource.DataSource = _uriageSeisanListCriteria;

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
                if (fpSpread1_Sheet1.SelectionCount == 0)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 合計行？
                if (fpSpread1_Sheet1.ActiveRowIndex == fpSpread1_Sheet1.Rows.Count - 1)
                {
                    MessageBox.Show(this, "レコードを選択してください。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ShowDetail();       // 詳細表示（既存レコード）
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

                // 合計行？
                if (e.Row == fpSpread1_Sheet1.Rows.Count - 1)
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

        /// <summary>
        /// Spreadセル移動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fpSpread1_LeaveCell(object sender, FarPoint.Win.Spread.LeaveCellEventArgs e)
        {
            // 最終行(=合計行)には移動しない
            if (e.NewRow >= fpSpread1_Sheet1.Rows.Count - 1)
            {
                e.Cancel = true;
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
                UriageSeisanCriteria uriageSeisanCriteria = new UriageSeisanCriteria();
                // 精算日以外の検索条件をセットする（精算日は範囲があるので特定できない）
                uriageSeisanCriteria.部署ＣＤ = _uriageSeisanListCriteria.部署ＣＤ;
                uriageSeisanCriteria.精算日 = null;
                uriageSeisanCriteria.コースＣＤ = _uriageSeisanListCriteria.コースＣＤ;
                Form403 form = new Form403(uriageSeisanCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                // モーダルにせず、複数開けるようにする
                // SubFormOpenInit(form);
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form403_FormClosed);
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
                UriageSeisanCriteria uriageSeisanCriteria = new UriageSeisanCriteria();

                uriageSeisanCriteria.stopOverwrite = true;
                uriageSeisanCriteria.部署ＣＤ = dr["部署ＣＤ"] as int?;
                uriageSeisanCriteria.精算日 = dr["精算日"] as DateTime?;
                uriageSeisanCriteria.コースＣＤ = dr["コースＣＤ"] as int?;
                uriageSeisanCriteria.コース名 = dr["コース名"] as string;
                Form403 form = new Form403(uriageSeisanCriteria);
                if (form.HasAuthBrowse == false)
                {
                    MessageBox.Show(this, "詳細画面を開く権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                // モーダルにせず、複数開けるようにする
                //SubFormOpenInit(form);
                form.MdiParent = this.MdiParent;
                form.Show();
                form.FormClosed += new FormClosedEventHandler(Form403_FormClosed);
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
        private void Form403_FormClosed(object sender, FormClosedEventArgs e)
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

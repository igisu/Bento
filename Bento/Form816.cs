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
    /// 随時売上入金一覧
    /// </summary>
    public partial class Form816 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_CD_COLUMN   = 0;    // 部署ＣＤ列インデックス
        private const int BUSYO_NAME_COLUMN = 1;    // 部署名列インデックス
        private const int CD_COLUMN         = 2;    // 得意先／担当者ＣＤ列インデックス
        private const int NAME_COLUMN       = 3;    // 得意先名／担当者名列インデックス
        private const int URIAGE_COLUMN     = 4;    // 数量列インデックス
        private const int NYUKIN_COLUMN     = 5;    // 入金列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 随時売上入金一覧検索条件
        /// </summary>
        private ZuijiUriNyuListCriteria _zuijiUriNyuListCriteria;

        /// <summary>
        /// 随時売上入金一覧検索条件検索実行時スナップショット
        /// </summary>
        private ZuijiUriNyuListCriteria _zuijiUriNyuListCriteriaSnapShot;
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form816()
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
        private void Form816_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _zuijiUriNyuListCriteria = new ZuijiUriNyuListCriteria();
                zuijiUriNyuListCriteriaBindingSource.DataSource = _zuijiUriNyuListCriteria;

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
        private void Form816_KeyDown(object sender, KeyEventArgs e)
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form816_Shown(object sender, EventArgs e)
        {
            // 指定年にフォーカス
            txtYear.Focus();
        }

        /// <summary>
        /// 集計モードラジオボタン変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSyukeiMode_CheckedChanged(object sender, EventArgs e)
        {
            // テキストボックス・参照ボタンの活性状態を、該当するラジオボタンの状態に合わせる
            txtTokuisakiCD.Enabled = rdoTokuisaki.Checked;
            lblTokuisakiName.Enabled = rdoTokuisaki.Checked;
            btnTokuisakiCD.Enabled = rdoTokuisaki.Checked;

            txtTantosyaCD.Enabled = rdoTantosya.Checked;
            lblTantosyaName.Enabled = rdoTantosya.Checked;
            btnTantosyaCD.Enabled = rdoTantosya.Checked;

            if (rdoTokuisaki.Checked)
            {
                // ソートキーの選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "得意先CD", "売上", "入金" },
                    new string[] { "A.得意先ＣＤ {0}",
                                   "U.売上 {0}, A.得意先ＣＤ",
                                   "N.入金 {0}, A.得意先ＣＤ"});
            }
            else
            {
                // ソートキーの選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "担当者CD", "売上" },
                    new string[] { "A.配送担当者ＣＤ {0}",
                                   "A.売上 {0}, A.配送担当者ＣＤ"});
            }
        }

        /// <summary>
        /// 部署ごとに集計チェックボックス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSummaryByBusyo_CheckedChanged(object sender, EventArgs e)
        {
            // 部署コンボの活性を切り替える
            cboBusyo.Enabled = chkSummaryByBusyo.Checked;
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
                    _zuijiUriNyuListCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
                    _zuijiUriNyuListCriteria.担当者ＣＤ = tantousya.担当者ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtTantosyaCD);
                    ReadBinding(lblTantosyaName);
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
                if (_zuijiUriNyuListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_zuijiUriNyuListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _zuijiUriNyuListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    if (_zuijiUriNyuListCriteria.得意先別)
                    {
                        sql.AppendLine(" SELECT");
                        sql.AppendLine("     A.部署ＣＤ");
                        sql.AppendLine("     ,B.部署名");
                        sql.AppendLine("     ,A.得意先ＣＤ AS ＣＤ");
                        sql.AppendLine("     ,T.得意先名 AS 名前");
                        sql.AppendLine("     ,U.売上");
                        sql.AppendLine("     ,N.入金");
                        sql.AppendLine(" FROM");
                        // -- ①駆動表（売上明細と入金明細のいずれかまたは両方にあるもの）
                        sql.AppendLine("     (SELECT");
                        sql.AppendLine("         企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,部署ＣＤ");           // -- 部署ごとに集計する場合はこちら
                        }
                        else
                        {
                            sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                        }
                        sql.AppendLine("         ,得意先ＣＤ");
                        sql.AppendLine("     FROM");
                        sql.AppendLine("         売上明細");
                        sql.AppendLine("     WHERE");
                        sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("         AND 売上日 >= @指定年月日From");
                        sql.AppendLine("         AND 売上日 <= @指定年月日To");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");    // -- 部署ごとに集計する場合のみ有効
                        }

                        if (_zuijiUriNyuListCriteria.得意先ＣＤ != null)
                        {
                            sql.AppendLine("         AND 得意先ＣＤ = @得意先ＣＤ");
                        }

                        sql.AppendLine("     UNION");
                        sql.AppendLine("     SELECT");
                        sql.AppendLine("         企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,部署ＣＤ");           // -- 部署ごとに集計する場合はこちら
                        }
                        else
                        {
                            sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                        }
                        sql.AppendLine("         ,得意先ＣＤ");
                        sql.AppendLine("     FROM");
                        sql.AppendLine("         入金明細");
                        sql.AppendLine("     WHERE");
                        sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("         AND 入金日 >= @指定年月日From");
                        sql.AppendLine("         AND 入金日 <= @指定年月日To");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");    // -- 部署ごとに集計する場合のみ有効
                        }

                        if (_zuijiUriNyuListCriteria.得意先ＣＤ != null)
                        {
                            sql.AppendLine("         AND 得意先ＣＤ = @得意先ＣＤ");
                        }

                        sql.AppendLine("     ) AS A");
                        sql.AppendLine("     LEFT JOIN");
                        // -- ②売上明細
                        sql.AppendLine("     (SELECT");
                        sql.AppendLine("         企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,部署ＣＤ");           // -- 部署ごとに集計する場合はこちら
                        }
                        else
                        {
                            sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                        }
                        sql.AppendLine("         ,得意先ＣＤ");
                        sql.AppendLine("         ,SUM(値引後金額) AS 売上");
                        sql.AppendLine("     FROM");
                        sql.AppendLine("         売上明細");
                        sql.AppendLine("     WHERE");
                        sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("         AND 売上日 >= @指定年月日From");
                        sql.AppendLine("         AND 売上日 <= @指定年月日To");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");    // -- 部署ごとに集計する場合のみ有効
                        }

                        if (_zuijiUriNyuListCriteria.得意先ＣＤ != null)
                        {
                            sql.AppendLine("         AND 得意先ＣＤ = @得意先ＣＤ");
                        }

                        sql.AppendLine("     GROUP BY");
                        sql.AppendLine("         企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,部署ＣＤ");       // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                        }
                        sql.AppendLine("         ,得意先ＣＤ");
                        sql.AppendLine("     ) AS U ON A.企業ＣＤ = U.企業ＣＤ ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("            AND A.部署ＣＤ = U.部署ＣＤ");  // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                        }
                        sql.AppendLine("            AND A.得意先ＣＤ = U.得意先ＣＤ");
                        sql.AppendLine("     LEFT JOIN");
                        // -- ③入金明細
                        sql.AppendLine("     (SELECT");
                        sql.AppendLine("         企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,部署ＣＤ");           // -- 部署ごとに集計する場合はこちら
                        }
                        else
                        {
                            sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                        }
                        sql.AppendLine("         ,得意先ＣＤ");
                        sql.AppendLine("         ,SUM(入金額) AS 入金");
                        sql.AppendLine("     FROM");
                        sql.AppendLine("         入金明細");
                        sql.AppendLine("     WHERE");
                        sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("         AND 入金日 >= @指定年月日From");
                        sql.AppendLine("         AND 入金日 <= @指定年月日To");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");    // -- 部署ごとに集計する場合のみ有効
                        }

                        if (_zuijiUriNyuListCriteria.得意先ＣＤ != null)
                        {
                            sql.AppendLine("         AND 得意先ＣＤ = @得意先ＣＤ");
                        }

                        sql.AppendLine("     GROUP BY");
                        sql.AppendLine("         企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,部署ＣＤ");       // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                        }
                        sql.AppendLine("         ,得意先ＣＤ");
                        sql.AppendLine("     ) AS N ON A.企業ＣＤ = N.企業ＣＤ ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("            AND A.部署ＣＤ = N.部署ＣＤ");  // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                        }

                        sql.AppendLine("            AND A.得意先ＣＤ = N.得意先ＣＤ");
                        sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON A.企業ＣＤ = B.企業ＣＤ AND A.部署ＣＤ = B.部署ＣＤ");
                        sql.AppendLine("     LEFT JOIN 得意先マスタ AS T ON A.企業ＣＤ = T.企業ＣＤ AND A.得意先ＣＤ = T.得意先ＣＤ");
                        sql.AppendLine(" ORDER BY ");
                        sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _zuijiUriNyuListCriteria.部署ＣＤ;
                        }
                        if (_zuijiUriNyuListCriteria.得意先ＣＤ != null)
                        {
                            cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _zuijiUriNyuListCriteria.得意先ＣＤ;
                        }
                    }
                    else
                    {
                        sql.AppendLine(" SELECT");
                        sql.AppendLine("     A.部署ＣＤ");
                        sql.AppendLine("     ,B.部署名");
                        sql.AppendLine("     ,A.配送担当者ＣＤ AS ＣＤ");
                        sql.AppendLine("     ,T.担当者名 AS 名前");
                        sql.AppendLine("     ,A.売上");
                        sql.AppendLine("     ,NULL AS 入金");
                        sql.AppendLine(" FROM");
                        // -- ①駆動表
                        sql.AppendLine("     (SELECT");
                        sql.AppendLine("         H.企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,H.部署ＣＤ");         // -- 部署ごとに集計する場合はこちら
                        }
                        else
                        {
                            sql.AppendLine("         ,NULL AS 部署ＣＤ");   // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこちら
                        }
                        sql.AppendLine("         ,H.配送担当者ＣＤ");
                        sql.AppendLine("         ,SUM(M.値引後金額) AS 売上");
                        sql.AppendLine("     FROM");
                        sql.AppendLine("         売上ヘッダ AS H");
                        sql.AppendLine("         INNER JOIN 売上明細 AS M");
                        sql.AppendLine("             ON H.企業ＣＤ = M.企業ＣＤ");
                        sql.AppendLine("             AND H.売上日 = M.売上日");
                        sql.AppendLine("             AND H.得意先ＣＤ = M.得意先ＣＤ");
                        sql.AppendLine("             AND H.部署ＣＤ = M.部署ＣＤ");
                        sql.AppendLine("     WHERE");
                        sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");
                        sql.AppendLine("         AND H.売上日 >= @指定年月日From");
                        sql.AppendLine("         AND H.売上日 <= @指定年月日To");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");      // -- 部署ごとに集計する場合のみ有効
                        }
                        if (_zuijiUriNyuListCriteria.担当者ＣＤ != null)
                        {
                            sql.AppendLine("         AND H.配送担当者ＣＤ = @担当者ＣＤ");
                        }
                        sql.AppendLine("     GROUP BY");
                        sql.AppendLine("         H.企業ＣＤ");
                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true)
                        {
                            sql.AppendLine("         ,H.部署ＣＤ");     // -- 部署ごとに集計しない（つまり部署を跨がって集計する）場合はこの行は不要
                        }
                        sql.AppendLine("         ,H.配送担当者ＣＤ");
                        sql.AppendLine("     ) AS A");
                        sql.AppendLine("     LEFT JOIN 部署マスタ AS B ON A.企業ＣＤ = B.企業ＣＤ AND A.部署ＣＤ = B.部署ＣＤ");
                        sql.AppendLine("     LEFT JOIN 担当者マスタ AS T ON A.企業ＣＤ = T.企業ＣＤ AND A.配送担当者ＣＤ = T.担当者ＣＤ");
                        sql.AppendLine(" ORDER BY ");
                        sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                        if (_zuijiUriNyuListCriteria.部署ごとに集計 == true && _zuijiUriNyuListCriteria.部署ＣＤ != null && _zuijiUriNyuListCriteria.部署ＣＤ != -1)
                        {
                            cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _zuijiUriNyuListCriteria.部署ＣＤ;
                        }
                        if (_zuijiUriNyuListCriteria.担当者ＣＤ != null)
                        {
                            cmd.Parameters.Add("@担当者ＣＤ", SqlDbType.Int).Value = _zuijiUriNyuListCriteria.担当者ＣＤ;
                        }
                    }

                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@指定年月日From", SqlDbType.Date).Value = _zuijiUriNyuListCriteria.指定年月日From;
                    cmd.Parameters.Add("@指定年月日To", SqlDbType.Date).Value = _zuijiUriNyuListCriteria.指定年月日To;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    DataTable dt = new DataTable("随時売上入金一覧");
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
                    bindingSource1.DataMember = "随時売上入金一覧";

                    log.Info("随時売上入金一覧を検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(BUSYO_CD_COLUMN,   "部署ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(BUSYO_NAME_COLUMN, "部署名");
                fpSpread1_Sheet1.BindDataColumn(CD_COLUMN,         "ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(NAME_COLUMN,       "名前");
                fpSpread1_Sheet1.BindDataColumn(URIAGE_COLUMN,     "売上");
                fpSpread1_Sheet1.BindDataColumn(NYUKIN_COLUMN,     "入金");

                if (_zuijiUriNyuListCriteria.得意先別)
                {
                    fpSpread1_Sheet1.Columns.Get(CD_COLUMN).Label = "得意先CD";
                    fpSpread1_Sheet1.Columns.Get(NAME_COLUMN).Label = "得意先名";
                    fpSpread1_Sheet1.Columns[NYUKIN_COLUMN].Visible = true;     // スプレッドの入金列を表示する
                }
                else
                {
                    fpSpread1_Sheet1.Columns.Get(CD_COLUMN).Label = "担当者CD";
                    fpSpread1_Sheet1.Columns.Get(NAME_COLUMN).Label = "担当者名";
                    fpSpread1_Sheet1.Columns[NYUKIN_COLUMN].Visible = false;        // スプレッドの入金列を非表示にする
                }

                // 最終行を固定行にする
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, NAME_COLUMN].Text = "合計";

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = URIAGE_COLUMN; columnIndex <= NYUKIN_COLUMN; columnIndex++)
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
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                // 検索実行時の検索条件を保存（Excel出力時に使用する）
                _zuijiUriNyuListCriteriaSnapShot = new ZuijiUriNyuListCriteria(_zuijiUriNyuListCriteria);
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
                _zuijiUriNyuListCriteria = new ZuijiUriNyuListCriteria();
                zuijiUriNyuListCriteriaBindingSource.DataSource = _zuijiUriNyuListCriteria;

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
        /// Excel出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {
                if (_zuijiUriNyuListCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                string busyoName = "";
                if (_zuijiUriNyuListCriteriaSnapShot.部署ごとに集計 == true && _zuijiUriNyuListCriteriaSnapShot.部署ＣＤ != null && _zuijiUriNyuListCriteriaSnapShot.部署ＣＤ != -1)
                {
                    Busyo busyo = Busyo.GetBusyo((int)_zuijiUriNyuListCriteriaSnapShot.部署ＣＤ);
                    if (busyo != null)
                    {
                        busyoName = string.Format("({0})", busyo.部署名);
                    }
                }

                saveFileDialog1.FileName = string.Format("{0}({1})_{2}年{3}月{4}",
                                                         this.Text,
                                                         _zuijiUriNyuListCriteria.得意先別 ? "得意先別" : "担当者別",
                                                         _zuijiUriNyuListCriteriaSnapShot.指定年,
                                                         _zuijiUriNyuListCriteriaSnapShot.指定月,
                                                         busyoName);
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

        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド
        #endregion
    }
}

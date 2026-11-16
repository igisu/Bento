using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using Emu.Bento.WebAPI;

namespace Emu.Bento
{
    /// <summary>
    /// クレカ利用状況サマリー
    /// </summary>
    public partial class Form818 : BaseSpreadForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数

        private const int SEIKYUSAKI_CD_COLUMN = 0;    // 請求先ＣＤ列インデックス
        private const int SEIKYUSAKI_NAME_COLUMN = 1;    // 請求先名列インデックス
        private const int SEIKYUBI_COLUMN = 2;    // 請求日列インデックス
        private const int TOKUISAKI_CD_COLUMN = 3;    // 得意先ＣＤ列インデックス
        private const int TOKUISAKI_NAME_COLUMN = 4;    // 得意先名名列インデックス
        private const int USE_COUNT_COLUMN = 5;    // 利用件数列インデックス
        private const int USE_AMOUNT_COLUMN = 6;    // 利用金額列インデックス

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数

        /// <summary>
        /// クレカ利用状況サマリー検索条件
        /// </summary>
        private FRUseSummaryListCriteria _frUseSummaryListCriteria;

        /// <summary>
        /// クレカ利用状況サマリー検索条件検索実行時スナップショット
        /// </summary>
        private FRUseSummaryListCriteria _frUseSummaryListCriteriaSnapShot;

        /// <summary>
        /// 手数料率リスト
        /// </summary>
        private List<FRFeeRate> _feeRateList;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form818()
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
        private void Form818_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                lblCount.Text = "";

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // ソートキー・表示数の選択肢生成
                ComboUtil.Set(cboSortKey,
                    new string[] { "請求日", "利用者CD" },
                    new string[] { "SD.請求日 {0}, SD.請求先ＣＤ",
                                   "SD.請求先ＣＤ {0}"});

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _frUseSummaryListCriteria = new FRUseSummaryListCriteria();
                frUseSummaryListCriteriaBindingSource.DataSource = _frUseSummaryListCriteria;
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
        private void Form818_KeyDown(object sender, KeyEventArgs e)
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
        private void Form818_Shown(object sender, EventArgs e)
        {
            // 指定年にフォーカス
            txtYear.Focus();
        }

        /// <summary>
        /// 請求先ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSeikyusakiCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSeikyusakiCD.PerformClick();
            }
        }

        /// <summary>
        /// 請求先ＣＤ参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSeikyusakiCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 得意先選択ダイアログを表示
                Form900 formTokuisakiDlg = new Form900();
                SubFormOpenInit(formTokuisakiDlg);
                formTokuisakiDlg.FormClosed += new FormClosedEventHandler(Form900SeikyusakiCD_FormClosed);
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
        /// 得意先選択ダイアログクローズ（請求先ＣＤ）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form900SeikyusakiCD_FormClosed(object sender, FormClosedEventArgs e)
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
                    errorProvider1.SetError(txtSeikyusakiCD, null);  // エラーを消す

                    Tokuisaki tokuisaki = form.SelectedTokuisaki;
                    _frUseSummaryListCriteria.請求先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtSeikyusakiCD);
                    ReadBinding(lblSeikyusakiName);
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
                    _frUseSummaryListCriteria.得意先ＣＤ = tokuisaki.得意先ＣＤ;       // 入力内容を確定させる
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
        private async void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_frUseSummaryListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_frUseSummaryListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _frUseSummaryListCriteria.Error, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // 手数料率取得
                await GetFeeRate();

                // 検索
                Search();
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
                _frUseSummaryListCriteria = new FRUseSummaryListCriteria();
                frUseSummaryListCriteriaBindingSource.DataSource = _frUseSummaryListCriteria;

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
                if (_frUseSummaryListCriteriaSnapShot == null)
                {
                    MessageBox.Show(this, "データが表示されていません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

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

        /// <summary>
        /// 検索
        /// </summary>
        /// <exception cref="EmuException"></exception>
        private void Search()
        {
            try
            {
                DataTable dt = new DataTable("クレカ利用状況サマリー");

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT");
                    sql.AppendLine("   SD.請求番号, ");
                    sql.AppendLine("   SD.請求日, ");
                    sql.AppendLine("   SD.部署ＣＤ, ");
                    sql.AppendLine("   SD.請求先ＣＤ, ");
                    sql.AppendLine("   SD.請求先名, ");
                    sql.AppendLine("   SD.繰越額, ");
                    sql.AppendLine("   SD.調整額, ");
                    sql.AppendLine("   SD.消費税, ");
                    sql.AppendLine("   SD.今回請求額, ");
                    sql.AppendLine("   SH.得意先ＣＤ, ");
                    sql.AppendLine("   SH.得意先名, ");
                    sql.AppendLine("   SH.売上金額税込 AS 利用金額, ");
                    sql.AppendLine("   FR.履歴番号, ");
                    sql.AppendLine("   FR.店舗ID, ");
                    sql.AppendLine("   U.利用件数 ");

                    // 今回の請求合計と前回請求日を取得
                    sql.AppendLine(" FROM ");
                    sql.AppendLine("   ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       A.*, ");
                    sql.AppendLine("       ( ");
                    sql.AppendLine("         SELECT ");
                    sql.AppendLine("           MAX(P.請求日) ");
                    sql.AppendLine("         FROM ");
                    sql.AppendLine("           請求合計 AS P ");
                    sql.AppendLine("         WHERE ");
                    sql.AppendLine("           P.企業ＣＤ = A.企業ＣＤ ");
                    sql.AppendLine("           AND P.請求先ＣＤ = A.請求先ＣＤ ");
                    sql.AppendLine("           AND P.請求日 < A.請求日 ");
                    sql.AppendLine("       ) AS 前回請求日 ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       請求合計 AS A ");
                    sql.AppendLine("     WHERE ");
                    sql.AppendLine("       A.企業ＣＤ = @企業ＣＤ ");
                    sql.AppendLine("       AND A.部署ＣＤ = @部署ＣＤ ");
                    sql.AppendLine("       AND A.FR支払フラグ = 1 ");
                    sql.AppendLine("       AND A.クレカ決済完了フラグ = 1 ");
                    sql.AppendLine("       AND A.クレカ決済エラー区分 = 0 ");
                    sql.AppendLine("       AND A.請求日 >= @請求日From ");
                    sql.AppendLine("       AND A.請求日 <= @請求日To ");

                    if (_frUseSummaryListCriteria.請求先ＣＤ != null && _frUseSummaryListCriteria.請求先ＣＤ != -1)
                    {
                        sql.AppendLine("       AND A.請求先ＣＤ = @請求先ＣＤ ");
                        cmd.Parameters.Add("@請求先ＣＤ", SqlDbType.Int).Value = _frUseSummaryListCriteria.請求先ＣＤ;
                    }

                    sql.AppendLine("   ) AS SD ");

                    // 得意先でグループ化した請求ヘッダを結合（請求ヘッダは消費税率ごとにレコードが分かれる）
                    sql.AppendLine("   INNER JOIN ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       企業ＣＤ, ");
                    sql.AppendLine("       請求番号, ");
                    sql.AppendLine("       得意先ＣＤ, ");
                    sql.AppendLine("       MAX(得意先名) AS 得意先名, ");
                    sql.AppendLine("       SUM(売上金額税込) AS 売上金額税込 ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       請求ヘッダ ");
                    sql.AppendLine("     WHERE ");
                    sql.AppendLine("       企業ＣＤ = @企業ＣＤ ");
                    sql.AppendLine("       AND 売上金額税込 <> 0 ");
                    sql.AppendLine("       AND 請求日 >= @請求日From ");
                    sql.AppendLine("       AND 請求日 <= @請求日To ");
                    sql.AppendLine("     GROUP BY ");
                    sql.AppendLine("       企業ＣＤ, ");
                    sql.AppendLine("       請求番号, ");
                    sql.AppendLine("       得意先ＣＤ ");
                    sql.AppendLine("   ) AS SH ON SH.企業ＣＤ = SD.企業ＣＤ AND SH.請求番号 = SD.請求番号 ");

                    // 最新決済履歴の店舗IDを取得
                    sql.AppendLine("   LEFT JOIN ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       CH1.履歴番号, ");
                    sql.AppendLine("       CH1.企業ＣＤ, ");
                    sql.AppendLine("       CH1.請求先ＣＤ, ");
                    sql.AppendLine("       CH1.請求日, ");
                    sql.AppendLine("       CH2.店舗ID ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       ( ");
                    sql.AppendLine("         SELECT ");
                    sql.AppendLine("           企業ＣＤ, ");
                    sql.AppendLine("           請求先ＣＤ, ");
                    sql.AppendLine("           請求日, ");
                    sql.AppendLine("           MAX(履歴番号) AS 履歴番号 ");
                    sql.AppendLine("         FROM ");
                    sql.AppendLine("           FR_T_クレカ決済履歴 ");
                    sql.AppendLine("         WHERE ");
                    sql.AppendLine("           企業ＣＤ = @企業ＣＤ ");
                    sql.AppendLine("           AND 部署ＣＤ = @部署ＣＤ ");
                    sql.AppendLine("           AND 処理タイプ = 1 ");
                    sql.AppendLine("           AND 請求日 >= @請求日From ");
                    sql.AppendLine("           AND 請求日 <= @請求日To ");

                    if (_frUseSummaryListCriteria.請求先ＣＤ != null && _frUseSummaryListCriteria.請求先ＣＤ != -1)
                    {
                        sql.AppendLine("           AND 請求先ＣＤ = @請求先ＣＤ ");
                    }

                    sql.AppendLine("         GROUP BY ");
                    sql.AppendLine("           企業ＣＤ, ");
                    sql.AppendLine("           請求先ＣＤ, ");
                    sql.AppendLine("           請求日 ");
                    sql.AppendLine("       ) AS CH1 ");
                    sql.AppendLine("       INNER JOIN FR_T_クレカ決済履歴 AS CH2 ON CH2.企業ＣＤ = CH1.企業ＣＤ AND CH2.履歴番号 = CH1.履歴番号 ");
                    sql.AppendLine("   ) AS FR ON FR.企業ＣＤ = SD.企業ＣＤ AND FR.請求先ＣＤ = SD.請求先ＣＤ AND FR.請求日 = SD.請求日 ");

                    // 今回請求分の売上ヘッダ数（=利用件数）を取得
                    sql.AppendLine("   OUTER APPLY ( ");
                    sql.AppendLine("     SELECT ");
                    sql.AppendLine("       COUNT(*) AS 利用件数 ");
                    sql.AppendLine("     FROM ");
                    sql.AppendLine("       売上ヘッダ AS S ");
                    sql.AppendLine("     WHERE ");
                    sql.AppendLine("       S.企業CD = SD.企業ＣＤ ");
                    sql.AppendLine("       AND S.部署CD = SD.部署ＣＤ ");
                    sql.AppendLine("       AND S.得意先ＣＤ = SH.得意先ＣＤ ");
                    sql.AppendLine("       AND S.売上日 > ISNULL(SD.前回請求日, '2000-01-01') ");   // 前回請求日がNULLの場合の日付は、請求締め処理と同じく2000-01-01とする
                    sql.AppendLine("       AND S.売上日 <= SD.請求日 ");
                    sql.AppendLine("   ) AS U ");
                    sql.AppendLine(" WHERE ");
                    sql.AppendLine("   1 = 1 ");

                    if (_frUseSummaryListCriteria.得意先ＣＤ != null && _frUseSummaryListCriteria.得意先ＣＤ != -1)
                    {
                        sql.AppendLine("   AND 得意先ＣＤ = @得意先ＣＤ ");
                        cmd.Parameters.Add("@得意先ＣＤ", SqlDbType.Int).Value = _frUseSummaryListCriteria.得意先ＣＤ;
                    }

                    sql.AppendLine(" ORDER BY ");
                    sql.AppendFormat((string)cboSortKey.SelectedValue, rdoAsc.Checked ? "ASC" : "DESC");

                    // 必須条件
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _frUseSummaryListCriteria.部署ＣＤ;
                    cmd.Parameters.Add("@請求日From", SqlDbType.Date).Value = _frUseSummaryListCriteria.指定年月日From;
                    cmd.Parameters.Add("@請求日To", SqlDbType.Date).Value = _frUseSummaryListCriteria.指定年月日To;

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);

                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    // 売上金額以外の金額を親レコード（請求先ＣＤ=得意先ＣＤ）の利用金額に加算する
                    int prevSeikyuCD = -1;
                    DataRow[] drs = dt.Select("繰越額 <> 0 OR 調整額 <> 0");

                    foreach (DataRow dr in drs)
                    {
                        int seikyuCD = dr["請求先ＣＤ"] as int? ?? 0;

                        if (seikyuCD == prevSeikyuCD)
                        {
                            continue;
                        }

                        DataRow[] seikyu = dt.Select($"得意先ＣＤ = {seikyuCD}");

                        if (seikyu.Length > 0)
                        {
                            // 親の請求がある

                            // 繰越額、調整額を加算して利用金額を更新する
                            decimal uriage = seikyu[0]["利用金額"] as decimal? ?? 0m;
                            decimal kurikosi = seikyu[0]["繰越額"] as decimal? ?? 0m;
                            decimal chosei = seikyu[0]["調整額"] as decimal? ?? 0m;
                            seikyu[0]["利用金額"] = uriage + kurikosi + chosei;
                        }
                        else
                        {
                            // 親の請求がない

                            // コピー元のインデックスを取得
                            int index = dt.Rows.IndexOf(dr);

                            // 新しい行を作成してコピー
                            DataRow newRow = dt.NewRow();
                            newRow.ItemArray = (object[])dr.ItemArray.Clone();

                            // 親の情報に変更
                            newRow["得意先ＣＤ"] = dr["請求先ＣＤ"];
                            newRow["得意先名"] = dr["請求先名"];
                            newRow["利用件数"] = 0;

                            // 繰越額、調整額を利用金額として更新する
                            decimal kurikosi = dr["繰越額"] as decimal? ?? 0m;
                            decimal chosei = dr["調整額"] as decimal? ?? 0m;
                            newRow["利用金額"] = kurikosi + chosei;

                            // コピー元の直前に挿入
                            dt.Rows.InsertAt(newRow, index);
                        }

                        prevSeikyuCD = seikyuCD;
                    }

                    // この時点の件数を表示する
                    lblCount.Text = String.Format("{0}件", dt.Rows.Count);

                    // 合計行を追加する
                    dt.Rows.Add(dt.NewRow());

                    // 数式を消すため、全行削除する
                    fpSpread1_Sheet1.Rows.Count = 0;

                    bindingSource1.DataSource = ds;
                    bindingSource1.DataMember = "クレカ利用状況サマリー";

                    log.Info("クレカ利用状況サマリーを検索しました。");
                }

                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_CD_COLUMN, "請求先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUSAKI_NAME_COLUMN, "請求先名");
                fpSpread1_Sheet1.BindDataColumn(SEIKYUBI_COLUMN, "請求日");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_CD_COLUMN, "得意先ＣＤ");
                fpSpread1_Sheet1.BindDataColumn(TOKUISAKI_NAME_COLUMN, "得意先名");
                fpSpread1_Sheet1.BindDataColumn(USE_COUNT_COLUMN, "利用件数");
                fpSpread1_Sheet1.BindDataColumn(USE_AMOUNT_COLUMN, "利用金額");

                // 合計行を追加
                fpSpread1_Sheet1.FrozenTrailingRowCount = 1;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, TOKUISAKI_NAME_COLUMN].Text = "合計";

                // 合計金額
                decimal totalAmount = 0m;

                // 合計行に数式を設定する
                if (fpSpread1_Sheet1.Rows.Count > 1)
                {
                    for (int columnIndex = USE_COUNT_COLUMN; columnIndex <= USE_AMOUNT_COLUMN; columnIndex++)
                    {
                        FarPoint.Win.Spread.Cell topCell = fpSpread1_Sheet1.Cells[0, columnIndex];
                        FarPoint.Win.Spread.Cell bottomCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 2, columnIndex];
                        FarPoint.Win.Spread.Cell sumCell = fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, columnIndex];
                        sumCell.Formula = string.Format("SUM({0}:{1})", topCell.ToString(), bottomCell.ToString());
                        if (columnIndex == USE_AMOUNT_COLUMN)
                        {
                            totalAmount += (decimal)sumCell.Value;
                        }
                    }
                }

                // 合計手数料額
                decimal totalFee = 0m;

                if (_feeRateList != null)
                {
                    foreach (FRFeeRate feeRate in _feeRateList)
                    {
                        DataRow[] drs = dt.Select(string.Format("店舗ID = {0}", feeRate.店舗ID));

                        if (drs.Length > 0)
                        {
                            // 合計金額を算出
                            decimal amount = (decimal)dt.Compute("SUM(利用金額)", string.Format("店舗ID = {0}", feeRate.店舗ID));

                            // 手数料を算出
                            decimal fee = Math.Truncate(amount * (feeRate.手数料率 ?? 0m) / 100m);

                            // 手数料行を追加
                            fpSpread1_Sheet1.FrozenTrailingRowCount += 1;
                            fpSpread1_Sheet1.AddRows(fpSpread1_Sheet1.Rows.Count, 1);
                            fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                            fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                            fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, TOKUISAKI_NAME_COLUMN].Value = string.Format("決済手数料 {0}%", feeRate.手数料率);
                            fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, USE_AMOUNT_COLUMN].Value = fee.ToString();
                            totalFee += fee;
                        }
                    }
                }

                if (dt.Rows.Count > 1)
                {
                    // 振込代金行を追加
                    fpSpread1_Sheet1.FrozenTrailingRowCount += 1;
                    fpSpread1_Sheet1.AddRows(fpSpread1_Sheet1.Rows.Count, 1);
                    fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].BackColor = TOTAL_BACK_COLOR;
                    fpSpread1_Sheet1.Rows[fpSpread1_Sheet1.Rows.Count - 1].CanFocus = false;
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, TOKUISAKI_NAME_COLUMN].Value = "お振込代金";
                    fpSpread1_Sheet1.Cells[fpSpread1_Sheet1.Rows.Count - 1, USE_AMOUNT_COLUMN].Value = totalAmount - totalFee;
                }

                fpSpread1.Focus();
                if (fpSpread1_Sheet1.Rows.Count != 0)
                {
                    fpSpread1_Sheet1.ActiveRowIndex = 0;
                    fpSpread1_Sheet1.AddSelection(0, 0, 1, 1);  // 先頭行を選択
                }

                // 検索実行時の検索条件を保存（Excel出力時に使用する）
                _frUseSummaryListCriteriaSnapShot = new FRUseSummaryListCriteria(_frUseSummaryListCriteria);
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
        /// 手数料率リスト取得
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EmuException"></exception>
        private async Task GetFeeRate()
        {
            try
            {
                bool result = false;
                string message = null;
                _feeRateList = new List<FRFeeRate>();

                await Task.Run(() =>
                {
                    FRSettlement fRSettlement = new FRSettlement();
                    result = fRSettlement.InfoGetFeeRateList((int)_frUseSummaryListCriteria.部署ＣＤ, (int)Consts.ShiharaiHohoKubun.CreditCard, out _feeRateList, out message);
                });

                if (!result)
                {
                    MessageBox.Show(this, "手数料率の取得に失敗しました。\n" + message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        #endregion
    }
}

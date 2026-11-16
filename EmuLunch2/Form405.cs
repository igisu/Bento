using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;

namespace Emu.Bento
{
    /// <summary>
    /// 売上集計表印刷
    /// </summary>
    public partial class Form405 : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_CD_COLUMN      = 0;        // コースCD列インデックス
        private const int COURSE_NAME_COLUMN    = 1;        // コース名列インデックス
        private const int HAISO_NAME_COLUMN     = 2;        // 配送者列インデックス
        private const int BENTO_COLUMN          = 3;        // 弁当列インデックス
        private const int MAINDISH_COLUMN       = 4;        // 主食列インデックス
        private const int SUBDISH_COLUMN        = 5;        // 副食列インデックス
        private const int KENSU_COLUMN          = 6;        // 件数列インデックス
        private const int SYOKUSU_COLUMN        = 7;        // 食数列インデックス
        private const int GEN_URI_COLUMN        = 8;        // 現金売上列インデックス
        private const int TICKET_CNT_COLUMN     = 9;        // チケット列インデックス
        private const int TICKET_PRICE_COLUMN   = 10;       // チケット代列インデックス
        private const int KAISYU_COLUMN         = 11;       // 回収現金列インデックス
        private const int GEN_TOTAL_COLUMN      = 12;       // 現金合計列インデックス
        private const int KAKE_TOTAL_COLUMN     = 13;       // 掛売合計列インデックス
        private const int PAY_TOTAL_COLUMN      = 14;       // 電子決済合計列インデックス
        private const int GROSS_COLUMN          = 15;       // 総売上列インデックス
        private const int NEBIKI_COLUMN         = 16;       // 総値引列インデックス
        private const int BARCLAY_COLUMN        = 17;       // バークレー列インデックス
        private const int NET_COLUMN            = 18;       // 実質売上列インデックス
        private const int AVERAGE_COLUMN        = 19;       // 平均単価列インデックス

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 売上集計表印刷
        /// </summary>
        private UriageNikkeiHyo _uriageNikkeiHyo;

        /// <summary>
        /// 管理マスタ（チケット回収時売上フラグ）
        /// </summary>
        private Kanri _kanri;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form405()
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
        private void Form405_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.calculator_ico;

                // コンボボックスの選択肢生成
                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _uriageNikkeiHyo = new UriageNikkeiHyo();
                uriageNikkeiHyoBindingSource.DataSource = _uriageNikkeiHyo;

                // 管理マスタ取得
                _kanri = Kanri.GetKanri();

                // 精算日、年月の活性状態を範囲のラジオボタンに合わせる
                dtpSeisanbi.Enabled = rdoDaily.Checked;
                txtYear.Enabled = rdoMonthly.Checked;
                lblYear.Enabled = rdoMonthly.Checked;
                txtMonth.Enabled = rdoMonthly.Checked;
                lblMonth.Enabled = rdoMonthly.Checked;
                dtpSeisanbiFrom.Enabled = rdoFromTo.Checked;
                dtpSeisanbiTo.Enabled = rdoFromTo.Checked;
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
        /// フォーム 初めて表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form405_Shown(object sender, EventArgs e)
        {
            if (rdoDaily.Checked)
            {
                // 日計の場合は精算日にフォーカス
                dtpSeisanbi.Focus();
            }
            if (rdoMonthly.Checked)
            {
                // 月計の場合は年にフォーカス
                txtYear.Focus();
            }
            if (rdoFromTo.Checked)
            {
                // 日付範囲指定の場合は開始日にフォーカス
                dtpSeisanbiFrom.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form405_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F10:                      // 処理開始ボタン押下
                        btnPrint.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 範囲ラジオボタン変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoRange_CheckedChanged(object sender, EventArgs e)
        {
            // 精算日、年月の活性状態を範囲のラジオボタンに合わせる
            dtpSeisanbi.Enabled = rdoDaily.Checked;
            txtYear.Enabled = rdoMonthly.Checked;
            lblYear.Enabled = rdoMonthly.Checked;
            txtMonth.Enabled = rdoMonthly.Checked;
            lblMonth.Enabled = rdoMonthly.Checked;
            dtpSeisanbiFrom.Enabled = rdoFromTo.Checked;
            dtpSeisanbiTo.Enabled = rdoFromTo.Checked;

            // ３つ以上のラジオボタンをデータバインドすると正しく反映されないので自力でやっている。
            _uriageNikkeiHyo.日計 = rdoDaily.Checked;
            _uriageNikkeiHyo.月計 = rdoMonthly.Checked;
            _uriageNikkeiHyo.日付範囲指定 = rdoFromTo.Checked;

            // 各ラジオボタンに連動するコントロールのエラーアイコンを表示する
            // ※エラーアイコンはバインドしているデータが変わったタイミングで表示されるが、ここはそうできないので自力で表示する
            errorProvider1.SetError(dtpSeisanbi, _uriageNikkeiHyo[dtpSeisanbi.DataBindings[0].BindingMemberInfo.BindingField]);
            errorProvider1.SetError(txtYear, _uriageNikkeiHyo[txtYear.DataBindings[0].BindingMemberInfo.BindingField]);
            errorProvider1.SetError(txtMonth, _uriageNikkeiHyo[txtMonth.DataBindings[0].BindingMemberInfo.BindingField]);
            errorProvider1.SetError(dtpSeisanbiFrom, _uriageNikkeiHyo[dtpSeisanbiFrom.DataBindings[0].BindingMemberInfo.BindingField]);
            errorProvider1.SetError(dtpSeisanbiTo, _uriageNikkeiHyo[dtpSeisanbiTo.DataBindings[0].BindingMemberInfo.BindingField]);
        }

        /// <summary>
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageNikkeiHyo.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageNikkeiHyo.Error) == false)
                    {
                        MessageBox.Show(this, _uriageNikkeiHyo.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (_uriageNikkeiHyo.日計)
                {
                    // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_uriageNikkeiHyo.精算日 > warnDate)
                    {
                        if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                if (_uriageNikkeiHyo.月計)
                {
                    // 精算年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                    DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                    DateTime seisanMonth = new DateTime((int)_uriageNikkeiHyo.対象年, (int)_uriageNikkeiHyo.対象月, 1);
                    if (seisanMonth > warnDate)
                    {
                        if (MessageBox.Show(this, "精算年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                if (_uriageNikkeiHyo.日付範囲指定)
                {
                    // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_uriageNikkeiHyo.精算日From > warnDate || _uriageNikkeiHyo.精算日To > warnDate)
                    {
                        if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                PrintUriageSyukeihyo();         // 売上集計表印刷
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
            DateTime? targetDate = null;
            string title = "";
            string code = "";
            string logmsg = "";

            try
            {
                // 検索条件の入力にエラーがある？
                if (_uriageNikkeiHyo.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageNikkeiHyo.Error) == false)
                    {
                        MessageBox.Show(this, _uriageNikkeiHyo.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (_uriageNikkeiHyo.日計)
                {
                    // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_uriageNikkeiHyo.精算日 > warnDate)
                    {
                        if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                if (_uriageNikkeiHyo.月計)
                {
                    // 精算年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                    DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                    DateTime seisanMonth = new DateTime((int)_uriageNikkeiHyo.対象年, (int)_uriageNikkeiHyo.対象月, 1);
                    if (seisanMonth > warnDate)
                    {
                        if (MessageBox.Show(this, "精算年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                if (_uriageNikkeiHyo.日付範囲指定)
                {
                    // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_uriageNikkeiHyo.精算日From > warnDate || _uriageNikkeiHyo.精算日To > warnDate)
                    {
                        if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                // 売上集計表を取得してSpreadに表示する
                GetUriageSyukei();

                if (_uriageNikkeiHyo.日計)
                {
                    title = "売上日計表";
                    targetDate = _uriageNikkeiHyo.精算日;
                    code = string.Format("部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("売上日計表(Excel)を出力しました。部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                }
                if (_uriageNikkeiHyo.月計)
                {
                    title = "売上月計表";
                    targetDate = new DateTime((int)_uriageNikkeiHyo.対象年, (int)_uriageNikkeiHyo.対象月, 1);
                    code = string.Format("部署ＣＤ={0} 年月={1:yyyy/MM}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("売上月計表(Excel)を出力しました。部署ＣＤ={0} 年月={1:yyyy/MM}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                }
                if (_uriageNikkeiHyo.日付範囲指定)
                {
                    title = "売上集計表";
                    code = string.Format("部署ＣＤ={0} 精算日={1:yyyy/MM/dd}～{2:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, _uriageNikkeiHyo.精算日From, _uriageNikkeiHyo.精算日To);
                    logmsg = string.Format("売上月計表を印刷しました。部署ＣＤ={0} 精算日={1:yyyy/MM/dd}～{2:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, _uriageNikkeiHyo.精算日From, _uriageNikkeiHyo.精算日To);
                }

                saveFileDialog1.FileName = title;
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

                    title += "(Excel)";
                    History.Save(this.Text, title, code, History.ProcessType.Output);

                    log.Info(logmsg);
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
        /// 売上集計表印刷処理
        /// </summary>
        private void PrintUriageSyukeihyo()
        {
            DateTime? targetDate = null;
            string title = "";
            string code = "";
            string logmsg = "";

            try
            {
                Report405A report = new Report405A();
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // データソースとなるデータテーブルを取得
                DataTable dt = GetDataTable();

                if (_uriageNikkeiHyo.日計)
                {
                    title = "売上日計表";
                    targetDate = _uriageNikkeiHyo.精算日;
                    code = string.Format("部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("売上日計表を印刷しました。部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                }
                if (_uriageNikkeiHyo.月計)
                {
                    title = "売上月計表";
                    targetDate = new DateTime((int)_uriageNikkeiHyo.対象年, (int)_uriageNikkeiHyo.対象月, 1);
                    code = string.Format("部署ＣＤ={0} 年月={1:yyyy/MM}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("売上月計表を印刷しました。部署ＣＤ={0} 年月={1:yyyy/MM}", _uriageNikkeiHyo.部署ＣＤ, (DateTime)targetDate);
                }
                if (_uriageNikkeiHyo.日付範囲指定)
                {
                    title = "売上集計表";
                    code = string.Format("部署ＣＤ={0} 精算日={1:yyyy/MM/dd}～{2:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, _uriageNikkeiHyo.精算日From, _uriageNikkeiHyo.精算日To);
                    logmsg = string.Format("売上月計表を印刷しました。部署ＣＤ={0} 精算日={1:yyyy/MM/dd}～{2:yyyy/MM/dd}", _uriageNikkeiHyo.部署ＣＤ, _uriageNikkeiHyo.精算日From, _uriageNikkeiHyo.精算日To);
                }

                report.DataSource = dt;

                // 帳票／プリンタ設定マスタにしたがって印刷またはプレビュー
                PrintReport(report, printSettings);

                History.Save(this.Text, title, code, History.ProcessType.Output);

                log.Info(logmsg);
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
        /// 売上集計表を取得してSpreadに表示する
        /// </summary>
        private void GetUriageSyukei()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                if (_kanri.チケット回収時売上フラグ != true)
                {
                    fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, GROSS_COLUMN).Value = "①+③+⑤+⑥ A";       // チケット束代を売上とする場合は、現金売上＋チケット代＋掛売合計
                }
                else
                {
                    fpSpread1_Sheet1.ColumnHeader.Cells.Get(0, GROSS_COLUMN).Value = "①+②+⑤+⑥ A";       // チケット回収時に売上とする場合は、現金売上＋チケット＋掛売合計
                }

                // データソースとなるデータテーブルを取得
                DataTable dt = GetDataTable();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "売上集計表";

                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,      "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,    "コース名");
                fpSpread1_Sheet1.BindDataColumn(HAISO_NAME_COLUMN,     "担当者名");
                fpSpread1_Sheet1.BindDataColumn(BENTO_COLUMN,          "弁当数量");
                fpSpread1_Sheet1.BindDataColumn(MAINDISH_COLUMN,       "主食数量");
                fpSpread1_Sheet1.BindDataColumn(SUBDISH_COLUMN,        "副食数量");
                fpSpread1_Sheet1.BindDataColumn(KENSU_COLUMN,          "件数");
                fpSpread1_Sheet1.BindDataColumn(SYOKUSU_COLUMN,        "食数");
                fpSpread1_Sheet1.BindDataColumn(GEN_URI_COLUMN,        "現金売上");
                fpSpread1_Sheet1.BindDataColumn(TICKET_CNT_COLUMN,     "チケット");
                fpSpread1_Sheet1.BindDataColumn(TICKET_PRICE_COLUMN,   "チケット代");
                fpSpread1_Sheet1.BindDataColumn(KAISYU_COLUMN,         "回収現金");
                fpSpread1_Sheet1.BindDataColumn(GEN_TOTAL_COLUMN,      "現金合計");
                fpSpread1_Sheet1.BindDataColumn(KAKE_TOTAL_COLUMN,     "掛売合計");
                fpSpread1_Sheet1.BindDataColumn(PAY_TOTAL_COLUMN,      "E合計");
                fpSpread1_Sheet1.BindDataColumn(GROSS_COLUMN,          "総売上");
                fpSpread1_Sheet1.BindDataColumn(NEBIKI_COLUMN,         "総値引");
                fpSpread1_Sheet1.BindDataColumn(BARCLAY_COLUMN,        "バークレー");
                fpSpread1_Sheet1.BindDataColumn(NET_COLUMN,            "実質売上");
                fpSpread1_Sheet1.BindDataColumn(AVERAGE_COLUMN,        "平均単価");
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
        /// 帳票・Excel共通のデータソースになるデータテーブルを返す
        /// </summary>
        /// <returns>データテーブル</returns>
        private DataTable GetDataTable()
        {
            string sql = "";
            string targetDate = "";
            string title = "";

            try
            {
                DataTable dt;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    if (_uriageNikkeiHyo.日計)
                    {
                        sql = "\r\n SELECT" +
                              "\r\n     @チケット回収時売上フラグ AS 総売上見出しフラグ" +
                              "\r\n     ,'{0}' AS 精算日" +
                              "\r\n     ,'{1}' AS タイトル" +
                              "\r\n     ,C.コースＣＤ" +
                              "\r\n     ,C.コース名" +
                              "\r\n     ,UB.配送担当者ＣＤ AS 担当者ＣＤ" +
                              "\r\n     ,UB.配送担当者名 AS 担当者名" +
                              "\r\n     ,UA.弁当数量" +
                              "\r\n     ,UA.主食数量" +
                              "\r\n     ,UA.副食数量" +
                              "\r\n     ,UB.件数" +
                              "\r\n     ,UA.弁当数量 + UA.副食数量 AS 食数" +
                              "\r\n     ,UB.現金売上" +
                              "\r\n     ,UB.チケット" +
                              "\r\n     ,UB.チケット代" +
                              "\r\n     ,UB.回収現金" +
                              "\r\n     ,UB.現金合計" +
                              "\r\n     ,UB.掛売合計" +
                              "\r\n     ,UB.E合計" +
                              "\r\n     ,UB.総売上" +
                              "\r\n     ,UB.総値引" +
                              "\r\n     ,UB.バークレー" +
                              "\r\n     ,UB.実質売上" +
                              "\r\n     ,CASE WHEN (UA.弁当数量 + UA.副食数量) = 0 THEN 0 ELSE FLOOR(UB.総売上 / (UA.弁当数量 + UA.副食数量)) END AS 平均単価" +
                              "\r\n     ,0 AS 合計行フラグ" +
                              "\r\n FROM" +
                              // -- ①駆動表" +
                              // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                              "\r\n     (SELECT" +
                              "\r\n         CM.コースＣＤ" +
                              "\r\n         ,CM.コース名" +
                              "\r\n     FROM" +
                              "\r\n         コースマスタ AS CM" +
                              "\r\n         LEFT JOIN" +
                              "\r\n             (SELECT" +
                              "\r\n                 コースＣＤ" +
                              "\r\n             FROM" +
                              "\r\n                 臨時コースマスタ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND コース区分 = @コース区分" +
                              "\r\n                 AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n                 AND ISNULL(工場フラグ, 0) = 0" +
                              "\r\n                 AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n                 AND 実施日 = @精算日" +
                              "\r\n             ) AS RM" +
                              "\r\n             ON RM.コースＣＤ = CM.コースＣＤ" +
                              "\r\n     WHERE" +
                              "\r\n         CM.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND CM.コース区分 = @コース区分" +
                              "\r\n         AND CM.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         AND ISNULL(CM.工場フラグ, 0) = 0" +
                              "\r\n         AND ISNULL(CM.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.コースＣＤ IS NULL" +
                              // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         RM.コースＣＤ" +
                              "\r\n         ,RM.コース名" +
                              "\r\n     FROM" +
                              "\r\n         臨時コーステーブル AS RT" +
                              "\r\n         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         RM.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND RM.コース区分 = @コース区分" +
                              "\r\n         AND RM.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         AND ISNULL(RM.工場フラグ, 0) = 0" +
                              "\r\n         AND ISNULL(RT.無効フラグ, 0) = 0" +
                              "\r\n         AND RM.実施日 = @精算日" +
                              // -- 集計対象の売上精算_ヘッダにあるコースも取得
                              "\r\n     UNION" +
                              "\r\n     SELECT" +
                              "\r\n         UH.コースＣＤ" +
                              "\r\n         ,ISNULL(RM.コース名, CM.コース名) AS コース名" +        //  -- 臨時コース名を優先して取得
                              "\r\n     FROM" +
                              "\r\n         売上精算_ヘッダ AS UH" +
                              "\r\n         INNER JOIN コースマスタ AS CM ON UH.コースＣＤ = CM.コースＣＤ" +
                              "\r\n         LEFT JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = UH.企業ＣＤ AND RM.コースＣＤ = UH.コースＣＤ AND RM.実施日 = UH.精算日 AND ISNULL(RM.無効フラグ, 0) = 0" +
                              "\r\n     WHERE" +
                              "\r\n         UH.企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND UH.精算日 = @精算日" +
                              "\r\n         AND UH.部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              // -- ②売上精算_売上明細から弁当・主食・副食の数量を取得
                              "\r\n     LEFT JOIN" +
                              "\r\n         (SELECT" +
                              "\r\n             UM.コースＣＤ" +
                              "\r\n             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量" +
                              "\r\n             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分主食 THEN UM.売上数量 ELSE 0 END) AS 主食数量" +
                              "\r\n             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量" +
                              "\r\n         FROM" +
                              "\r\n             売上精算_売上明細 AS UM" +
                              "\r\n             INNER JOIN 商品マスタ AS SM" +
                              "\r\n                 ON UM.企業ＣＤ = SM.企業ＣＤ" +
                              "\r\n                 AND UM.商品ＣＤ = SM.商品ＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             UM.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND UM.精算日 = @精算日" +
                              "\r\n             AND UM.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             UM.コースＣＤ" +
                              "\r\n         ) AS UA" +
                              "\r\n     ON UA.コースＣＤ = C.コースＣＤ" +
                              // -- ③売上精算_ヘッダから金額を取得
                              "\r\n     LEFT JOIN" +
                              "\r\n         (SELECT" +
                              "\r\n             B.コースＣＤ" +
                              "\r\n             ,B.配送担当者ＣＤ" +
                              "\r\n             ,B.配送担当者名" +
                              "\r\n             ,B.件数" +
                              "\r\n             ,B.現金売上" +
                              "\r\n             ,B.チケット" +
                              "\r\n             ,B.チケット代" +
                              "\r\n             ,B.回収現金" +
                              "\r\n             ,B.現金合計" +
                              "\r\n             ,B.掛売合計" +
                              "\r\n             ,B.E合計 + B.E_チケット束代 AS E合計" +
                              "\r\n             ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0" +
                              "\r\n              THEN B.現金売上 + B.チケット代 + B.掛売合計 + B.E合計 + B.E_チケット束代" +
                              "\r\n              ELSE B.現金売上 + B.チケット + B.掛売合計 + B.E合計 + B.E_チケット束代" +      // -- 電子決済のチケット束代は常に加算する
                              "\r\n              END AS 総売上" +
                              "\r\n             ,B.総値引" +
                              "\r\n             ,B.バークレー" +
                              "\r\n             ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0" +
                              "\r\n              THEN B.現金売上 + B.チケット代 + B.掛売合計 + B.E合計 + B.E_チケット束代 - B.総値引 + B.バークレー" +
                              "\r\n              ELSE B.現金売上 + B.チケット + B.掛売合計 + B.E合計 + B.E_チケット束代 - B.総値引 + B.バークレー" +    // -- 電子決済のチケット束代は常に加算する
                              "\r\n              END AS 実質売上" +
                              "\r\n         FROM" +
                              "\r\n             (SELECT" +
                              "\r\n                 企業ＣＤ" +
                              "\r\n                 ,コースＣＤ" +
                              "\r\n                 ,配送担当者ＣＤ" +
                              "\r\n                 ,配送担当者名" +
                              "\r\n                 ,ISNULL(得意先数, 0) AS 件数" +
                              "\r\n                 ,ISNULL(現金_売上合計, 0) AS 現金売上" +
                              "\r\n                 ,ISNULL(現金_チケット束代, 0) AS チケット代" +
                              "\r\n                 ,ISNULL(現金_回収現金, 0) AS 回収現金" +
                              "\r\n                 ,ISNULL(現金_売上合計, 0) + ISNULL(現金_チケット束代, 0) + ISNULL(現金_回収現金, 0) AS 現金合計" +
                              "\r\n                 ,ISNULL(掛売_売上合計, 0) AS 掛売合計" +
                              "\r\n                 ,ISNULL(E_売上合計, 0) AS E合計" +
                              "\r\n                 ,ISNULL(E_チケット束代, 0) AS E_チケット束代" +
                              "\r\n                 ,ISNULL(現金_値引合計, 0) + ISNULL(掛売_値引合計, 0) + ISNULL(E_値引合計, 0) AS 総値引" +
                              "\r\n                 ,ISNULL(合計_チケット売上, 0) AS チケット" +
                              "\r\n                 ,ISNULL(合計_バークレー売上, 0) AS バークレー" +
                              "\r\n             FROM" +
                              "\r\n                 売上精算_ヘッダ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND 精算日 = @精算日" +
                              "\r\n                 AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n             ) AS B" +
                              "\r\n             INNER JOIN 管理マスタ AS K ON K.企業ＣＤ = B.企業ＣＤ" +
                              "\r\n         ) AS UB" +
                              "\r\n     ON UB.コースＣＤ = C.コースＣＤ" +
                              "\r\n ORDER BY" +
                              "\r\n     C.コースＣＤ";

                        targetDate = string.Format("{0:yyyy年MM月dd日(ddd曜日)}", _uriageNikkeiHyo.精算日);
                        cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_uriageNikkeiHyo.精算日;
                        cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = (int)_uriageNikkeiHyo.コース区分;
                        title = "売上日計表";
                    }
                    else
                    {
                        // 月計と日付範囲指定
                        sql = "\r\n SELECT" +
                              "\r\n     @チケット回収時売上フラグ AS 総売上見出しフラグ" +
                              "\r\n     ,'{0}' AS 精算日" +
                              "\r\n     ,'{1}' AS タイトル" +
                              "\r\n     ,C.コースＣＤ" +
                              "\r\n     ,CM.コース名" +
                              "\r\n     ,CM.担当者ＣＤ" +
                              "\r\n     ,TM.担当者名" +
                              "\r\n     ,UA.弁当数量" +
                              "\r\n     ,UA.主食数量" +
                              "\r\n     ,UA.副食数量" +
                              "\r\n     ,UB.件数" +
                              "\r\n     ,UA.弁当数量 + UA.副食数量 AS 食数" +
                              "\r\n     ,UB.現金売上" +
                              "\r\n     ,UB.チケット" +
                              "\r\n     ,UB.チケット代" +
                              "\r\n     ,UB.回収現金" +
                              "\r\n     ,UB.現金合計" +
                              "\r\n     ,UB.掛売合計" +
                              "\r\n     ,UB.E合計" +
                              "\r\n     ,UB.総売上" +
                              "\r\n     ,UB.総値引" +
                              "\r\n     ,UB.バークレー" +
                              "\r\n     ,UB.実質売上" +
                              "\r\n     ,CASE WHEN (UA.弁当数量 + UA.副食数量) = 0 THEN 0 ELSE FLOOR(UB.総売上 / (UA.弁当数量 + UA.副食数量)) END AS 平均単価" +
                              "\r\n     ,0 AS 合計行フラグ" +
                              "\r\n FROM" +
                              // -- ①駆動表
                              // -- コースマスタから取得（売上日計表とは異なり、臨時コーステーブルからは取得しない）
                              "\r\n     (SELECT" +
                              "\r\n         コースＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         コースマスタ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n         AND ISNULL(工場フラグ, 0) = 0" +
                              "\r\n         AND ISNULL(無効フラグ, 0) = 0" +
                              "\r\n     UNION" +
                              // -- 集計対象の売上精算_ヘッダにあるコースも取得
                              "\r\n     SELECT" +
                              "\r\n         コースＣＤ" +
                              "\r\n     FROM" +
                              "\r\n         売上精算_ヘッダ" +
                              "\r\n     WHERE" +
                              "\r\n         企業ＣＤ = @企業ＣＤ" +
                              "\r\n         AND 精算日 >= @精算日From" +
                              "\r\n         AND 精算日 <= @精算日To" +
                              "\r\n         AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n     ) AS C" +
                              // -- ②売上精算_売上明細から弁当・主食・副食の数量を取得
                              "\r\n     LEFT JOIN" +
                              "\r\n         (SELECT" +
                              "\r\n             UM.コースＣＤ" +
                              "\r\n             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量" +
                              "\r\n             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分主食 THEN UM.売上数量 ELSE 0 END) AS 主食数量" +
                              "\r\n             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量" +
                              "\r\n         FROM" +
                              "\r\n             売上精算_売上明細 AS UM" +
                              "\r\n             INNER JOIN 商品マスタ AS SM" +
                              "\r\n                 ON UM.企業ＣＤ = SM.企業ＣＤ" +
                              "\r\n                 AND UM.商品ＣＤ = SM.商品ＣＤ" +
                              "\r\n         WHERE" +
                              "\r\n             UM.企業ＣＤ = @企業ＣＤ" +
                              "\r\n             AND UM.精算日 >= @精算日From" +
                              "\r\n             AND UM.精算日 <= @精算日To" +
                              "\r\n             AND UM.部署ＣＤ = @部署ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             UM.コースＣＤ" +
                              "\r\n         ) AS UA" +
                              "\r\n     ON UA.コースＣＤ = C.コースＣＤ" +
                              // -- ③売上精算_ヘッダから金額を取得
                              "\r\n     LEFT JOIN" +
                              "\r\n         (SELECT" +
                              "\r\n             B.コースＣＤ" +
                              "\r\n             ,SUM(B.件数) AS 件数" +
                              "\r\n             ,SUM(B.現金売上) AS 現金売上" +
                              "\r\n             ,SUM(B.チケット) AS チケット" +
                              "\r\n             ,SUM(B.チケット代) AS チケット代" +
                              "\r\n             ,SUM(B.回収現金) AS 回収現金" +
                              "\r\n             ,SUM(B.現金合計) AS 現金合計" +
                              "\r\n             ,SUM(B.掛売合計) AS 掛売合計" +
                              "\r\n             ,SUM(B.E合計) + SUM(B.E_チケット束代) AS E合計" +
                              "\r\n             ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0" +
                              "\r\n              THEN SUM(B.現金売上) + SUM(B.チケット代) + SUM(B.掛売合計) + SUM(B.E合計) + SUM(B.E_チケット束代)" +
                              "\r\n              ELSE SUM(B.現金売上) + SUM(B.チケット) + SUM(B.掛売合計) + SUM(B.E合計) + SUM(B.E_チケット束代)" +     // -- 電子決済のチケット束代は常に加算する
                              "\r\n              END AS 総売上" +
                              "\r\n             ,SUM(B.総値引) AS 総値引" +
                              "\r\n             ,SUM(B.バークレー) AS バークレー" +
                              "\r\n             ,CASE WHEN ISNULL(K.チケット回収時売上フラグ, 0) = 0" +
                              "\r\n              THEN SUM(B.現金売上) + SUM(B.チケット代) + SUM(B.掛売合計) + SUM(B.E合計) + SUM(B.E_チケット束代) - SUM(B.総値引) + SUM(B.バークレー)" +
                              "\r\n              ELSE SUM(B.現金売上) + SUM(B.チケット) + SUM(B.掛売合計) + SUM(B.E合計) + SUM(B.E_チケット束代) - SUM(B.総値引) + SUM(B.バークレー)" +     // -- 電子決済のチケット束代は常に加算する
                              "\r\n              END AS 実質売上" +
                              "\r\n         FROM" +
                              "\r\n             (SELECT" +
                              "\r\n                 企業ＣＤ" +
                              "\r\n                 ,コースＣＤ" +
                              "\r\n                 ,ISNULL(得意先数, 0) AS 件数" +
                              "\r\n                 ,ISNULL(現金_売上合計, 0) AS 現金売上" +
                              "\r\n                 ,ISNULL(現金_チケット束代, 0) AS チケット代" +
                              "\r\n                 ,ISNULL(現金_回収現金, 0) AS 回収現金" +
                              "\r\n                 ,ISNULL(現金_売上合計, 0) + ISNULL(現金_チケット束代, 0) + ISNULL(現金_回収現金, 0) AS 現金合計" +
                              "\r\n                 ,ISNULL(掛売_売上合計, 0) AS 掛売合計" +
                              "\r\n                 ,ISNULL(E_売上合計, 0) AS E合計" +
                              "\r\n                 ,ISNULL(E_チケット束代, 0) AS E_チケット束代" +
                              "\r\n                 ,ISNULL(現金_値引合計, 0) + ISNULL(掛売_値引合計, 0) + ISNULL(E_値引合計, 0) AS 総値引" +
                              "\r\n                 ,ISNULL(合計_チケット売上, 0) AS チケット" +
                              "\r\n                 ,ISNULL(合計_バークレー売上, 0) AS バークレー" +
                              "\r\n             FROM" +
                              "\r\n                 売上精算_ヘッダ" +
                              "\r\n             WHERE" +
                              "\r\n                 企業ＣＤ = @企業ＣＤ" +
                              "\r\n                 AND 精算日 >= @精算日From" +
                              "\r\n                 AND 精算日 <= @精算日To" +
                              "\r\n                 AND 部署ＣＤ = @部署ＣＤ" +
                              "\r\n             ) AS B" +
                              "\r\n             INNER JOIN 管理マスタ AS K ON K.企業ＣＤ = B.企業ＣＤ" +
                              "\r\n         GROUP BY" +
                              "\r\n             B.企業ＣＤ" +
                              "\r\n             ,B.コースＣＤ" +
                              "\r\n             ,K.チケット回収時売上フラグ" +
                              "\r\n         ) AS UB" +
                              "\r\n     ON UB.コースＣＤ = C.コースＣＤ" +
                              "\r\n     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ" +
                              "\r\n     LEFT JOIN 担当者マスタ AS TM ON TM.企業ＣＤ = CM.企業ＣＤ AND TM.担当者ＣＤ = CM.担当者ＣＤ" +
                              "\r\n ORDER BY" +
                              "\r\n     C.コースＣＤ";

                        DateTime fromDate = new DateTime((int)_uriageNikkeiHyo.対象年, (int)_uriageNikkeiHyo.対象月, 1);
                        DateTime toDate = new DateTime(fromDate.Year, fromDate.Month, DateTime.DaysInMonth(fromDate.Year, fromDate.Month));
                        targetDate = string.Format("{0:yyyy年MM月分}", fromDate);
                        title = "売上月計表";

                        if (_uriageNikkeiHyo.日付範囲指定)
                        {
                            fromDate = (DateTime)_uriageNikkeiHyo.精算日From;
                            toDate = (DateTime)_uriageNikkeiHyo.精算日To;
                            targetDate = string.Format("{0:yyyy年MM月dd日(ddd曜日)}～{1:yyyy年MM月dd日(ddd曜日)}", fromDate, toDate);
                            title = "売上集計表";
                        }
                        cmd.Parameters.Add("@精算日From", SqlDbType.Date).Value = fromDate;
                        cmd.Parameters.Add("@精算日To", SqlDbType.Date).Value = toDate;
                    }

                    cmd.Parameters.Add("@チケット回収時売上フラグ", SqlDbType.Int).Value = (_kanri.チケット回収時売上フラグ != true ? 0 : 1);
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_uriageNikkeiHyo.部署ＣＤ;
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;

                    cmd.CommandText = string.Format(sql, targetDate, title);

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("売上集計表");
                    adp.Fill(dt);
                }

                // 合計行を追加
                DataRow drTotal = dt.NewRow();
                drTotal["精算日"] = targetDate;
                drTotal["タイトル"] = title;
                drTotal["合計行フラグ"] = 1;
                drTotal["コース名"] = "　　　　合　計";
                drTotal["弁当数量"] = dt.Compute("SUM(弁当数量)", "");
                drTotal["主食数量"] = dt.Compute("SUM(主食数量)", "");
                drTotal["副食数量"] = dt.Compute("SUM(副食数量)", "");
                drTotal["件数"] = dt.Compute("SUM(件数)", "");
                drTotal["食数"] = dt.Compute("SUM(食数)", "");
                drTotal["現金売上"] = dt.Compute("SUM(現金売上)", "");
                drTotal["チケット代"] = dt.Compute("SUM(チケット代)", "");
                drTotal["回収現金"] = dt.Compute("SUM(回収現金)", "");
                drTotal["現金合計"] = dt.Compute("SUM(現金合計)", "");
                drTotal["掛売合計"] = dt.Compute("SUM(掛売合計)", "");
                drTotal["E合計"] = dt.Compute("SUM(E合計)", "");
                drTotal["総売上"] = dt.Compute("SUM(総売上)", "");
                drTotal["総値引"] = dt.Compute("SUM(総値引)", "");
                drTotal["チケット"] = dt.Compute("SUM(チケット)", "");
                drTotal["バークレー"] = dt.Compute("SUM(バークレー)", "");
                drTotal["実質売上"] = dt.Compute("SUM(実質売上)", "");
                if (drTotal.IsNull("弁当数量") == false && drTotal.IsNull("副食数量") == false)
                {
                    object value = drTotal["弁当数量"];
                    decimal bento = decimal.Parse(value.ToString());
                    value = drTotal["副食数量"];
                    decimal fuku = decimal.Parse(value.ToString());
                    if (bento == 0m && fuku == 0m)
                    {
                        drTotal["平均単価"] = 0;
                    }
                    else
                    {
                        if (drTotal.IsNull("総売上") == false)
                        {
                            value = drTotal["総売上"];
                            decimal total = decimal.Parse(value.ToString());
                            drTotal["平均単価"] = Math.Floor(total / (bento + fuku));
                        }
                    }
                }

                dt.Rows.Add(drTotal);

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
        #endregion
    }
}

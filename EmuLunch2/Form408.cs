using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using System.Text;

namespace Emu.Bento
{
    /// <summary>
    /// 販売実績一覧表印刷
    /// </summary>
    public partial class Form408 : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int COURSE_CD_COLUMN    = 0;          // コースCD列インデックス
        private const int COURSE_NAME_COLUMN  = 1;          // コース名列インデックス
        private const int HAISO_NAME_COLUMN   = 2;          // 配送者列インデックス
        private const int MCDS_SUB_COLUMN     = 3;          // 持出数副食列インデックス
        private const int MCDS_MAIN_COLUMN    = 4;          // 持出数主食列インデックス
        private const int TAKE_SUB_COLUMN     = 5;          // 貰い副食列インデックス
        private const int TAKE_MAIN_COLUMN    = 6;          // 貰い主食列インデックス
        private const int GIVE_SUB_COLUMN     = 7;          // 渡し副食列インデックス
        private const int GIVE_MAIN_COLUMN    = 8;          // 渡し主食列インデックス
        private const int SALE_SUB_COLUMN     = 9;          // 販売数副食列インデックス
        private const int SALE_MAIN_COLUMN    = 10;         // 販売数主食代列インデックス
        private const int ZAN_SUB_COLUMN      = 11;         // 残数副食列インデックス
        private const int ZAN_MAIN_COLUMN     = 12;         // 残数主食列インデックス
        private const int SYOKUSU_COLUMN      = 13;         // 食数列インデックス
        private const int GEN_URI_COLUMN      = 14;         // 現金売上列インデックス
        private const int KAKE_URI_COLUMN     = 15;         // 掛売売上列インデックス
        private const int PAY_URI_COLUMN      = 16;         // 電子決済売上列インデックス
        private const int SONOTA_URI_COLUMN   = 17;         // その他列インデックス
        private const int NET_COLUMN          = 18;         // 実質売上列インデックス
        private const int AVERAGE_COLUMN      = 19;         // 平均単価列インデックス

        public const int DELIMITER_COURSE_CD = 999999999;   // 合計用コースＣＤ
        public const int DELIMITER_BUSHO_CD = 999999999;    // 合計用部署ＣＤ

        #endregion
        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 販売実績一覧表印刷出力条件
        /// </summary>
        private HanbaiJissekiListCriteria _hanbaiJissekiListCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form408()
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
        private void Form408_Load(object sender, EventArgs e)
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
                _hanbaiJissekiListCriteria = new HanbaiJissekiListCriteria();
                hanbaiJissekiListCriteriaBindingSource.DataSource = _hanbaiJissekiListCriteria;

                // 精算日、年月の活性状態を範囲のラジオボタンに合わせる
                dtpSeisanbi.Enabled = rdoDaily.Checked;
                txtYear.Enabled = rdoMonthly.Checked;
                lblYear.Enabled = rdoMonthly.Checked;
                txtMonth.Enabled = rdoMonthly.Checked;
                lblMonth.Enabled = rdoMonthly.Checked;
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
        private void Form408_Shown(object sender, EventArgs e)
        {
            if (rdoDaily.Checked)
            {
                // 日計の場合は精算日にフォーカス
                dtpSeisanbi.Focus();
            }
            else
            {
                // 月計の場合は年にフォーカス
                txtYear.Focus();
            }
        }

        /// <summary>
        /// フォームキー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form408_KeyDown(object sender, KeyEventArgs e)
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
                if (_hanbaiJissekiListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_hanbaiJissekiListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _hanbaiJissekiListCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (_hanbaiJissekiListCriteria.日計)
                {
                    // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_hanbaiJissekiListCriteria.精算日 > warnDate)
                    {
                        if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    // 精算年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                    DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                    DateTime seisanMonth = new DateTime((int)_hanbaiJissekiListCriteria.対象年, (int)_hanbaiJissekiListCriteria.対象月, 1);
                    if (seisanMonth > warnDate)
                    {
                        if (MessageBox.Show(this, "精算年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                PrintHanbaiJissekiList();       // 販売実績一覧表印刷
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
                if (_hanbaiJissekiListCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_hanbaiJissekiListCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _hanbaiJissekiListCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                if (_hanbaiJissekiListCriteria.日計)
                {
                    // 精算日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                    DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                    if (_hanbaiJissekiListCriteria.精算日 > warnDate)
                    {
                        if (MessageBox.Show(this, "精算日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    // 精算年月が（システム日付＋未来月数警告閾値）を超えていれば警告メッセージを表示
                    DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    DateTime warnDate = currentMonth.AddMonths(SystemMaster.GetWarningFutureMonths());
                    DateTime seisanMonth = new DateTime((int)_hanbaiJissekiListCriteria.対象年, (int)_hanbaiJissekiListCriteria.対象月, 1);
                    if (seisanMonth > warnDate)
                    {
                        if (MessageBox.Show(this, "精算年月として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                // 販売実績一覧表を取得してSpreadに表示する
                GetHanbaiJissekiList();

                if (_hanbaiJissekiListCriteria.日計)
                {
                    title = "販売実績一覧表(日計)";
                    targetDate = _hanbaiJissekiListCriteria.精算日;
                    code = string.Format("部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("販売実績一覧表(日計)(Excel)を出力しました。部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
                }
                else
                {
                    title = "販売実績一覧表(月計)";
                    targetDate = new DateTime((int)_hanbaiJissekiListCriteria.対象年, (int)_hanbaiJissekiListCriteria.対象月, 1);
                    code = string.Format("部署ＣＤ={0} 年月={1:yyyy/MM}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("販売実績一覧表(月計)(Excel)を出力しました。部署ＣＤ={0} 年月={1:yyyy/MM}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
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
        /// 販売実績一覧表印刷処理
        /// </summary>
        private void PrintHanbaiJissekiList()
        {
            DateTime? targetDate = null;
            string title = "";
            string code = "";
            string logmsg = "";

            try
            {
                Report408A report = new Report408A();
                PrintSettings printSettings = this.GetPrintSettings(report);
                if (printSettings == null)
                {
                    return;
                }

                this.MdiParent.Cursor = Cursors.WaitCursor;

                // データソースとなるデータテーブルを取得
                DataTable dt = GetDataTable();

                if (_hanbaiJissekiListCriteria.日計)
                {
                    title = "販売実績一覧表(日計)";
                    targetDate = _hanbaiJissekiListCriteria.精算日;
                    code = string.Format("部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("販売実績一覧表(日計)を印刷しました。部署ＣＤ={0} 精算日={1:yyyy/MM/dd}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
                }
                else
                {
                    title = "販売実績一覧表(月計)";
                    targetDate = new DateTime((int)_hanbaiJissekiListCriteria.対象年, (int)_hanbaiJissekiListCriteria.対象月, 1);
                    code = string.Format("部署ＣＤ={0} 年月={1:yyyy/MM}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
                    logmsg = string.Format("販売実績一覧表(月計)を印刷しました。部署ＣＤ={0} 年月={1:yyyy/MM}", _hanbaiJissekiListCriteria.部署ＣＤ, (DateTime)targetDate);
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
        /// 販売実績一覧表を取得してSpreadに表示する
        /// </summary>
        private void GetHanbaiJissekiList()
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                // データソースとなるデータテーブルを取得
                DataTable dt = GetDataTable();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                bindingSource1.DataSource = ds;
                bindingSource1.DataMember = "販売実績一覧表";

                fpSpread1_Sheet1.BindDataColumn(COURSE_CD_COLUMN,    "コースＣＤ");
                fpSpread1_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,  "コース名");
                fpSpread1_Sheet1.BindDataColumn(HAISO_NAME_COLUMN,   "担当者名");
                fpSpread1_Sheet1.BindDataColumn(MCDS_SUB_COLUMN,     "持出数副食");
                fpSpread1_Sheet1.BindDataColumn(MCDS_MAIN_COLUMN,    "持出数主食");
                fpSpread1_Sheet1.BindDataColumn(TAKE_SUB_COLUMN,     "貰い副食");
                fpSpread1_Sheet1.BindDataColumn(TAKE_MAIN_COLUMN,    "貰い主食");
                fpSpread1_Sheet1.BindDataColumn(GIVE_SUB_COLUMN,     "渡し副食");
                fpSpread1_Sheet1.BindDataColumn(GIVE_MAIN_COLUMN,    "渡し主食");
                fpSpread1_Sheet1.BindDataColumn(SALE_SUB_COLUMN,     "販売数副食");
                fpSpread1_Sheet1.BindDataColumn(SALE_MAIN_COLUMN,    "販売数主食");
                fpSpread1_Sheet1.BindDataColumn(ZAN_SUB_COLUMN,      "残数副食");
                fpSpread1_Sheet1.BindDataColumn(ZAN_MAIN_COLUMN,     "残数主食");
                fpSpread1_Sheet1.BindDataColumn(SYOKUSU_COLUMN,      "食数");
                fpSpread1_Sheet1.BindDataColumn(GEN_URI_COLUMN,      "現金売上");
                fpSpread1_Sheet1.BindDataColumn(KAKE_URI_COLUMN,     "掛売売上");
                fpSpread1_Sheet1.BindDataColumn(PAY_URI_COLUMN,      "E売上");
                fpSpread1_Sheet1.BindDataColumn(SONOTA_URI_COLUMN,   "その他");
                fpSpread1_Sheet1.BindDataColumn(NET_COLUMN,          "実質売上");
                fpSpread1_Sheet1.BindDataColumn(AVERAGE_COLUMN,      "平均単価");
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
            try
            {
                DataTable dt;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    string sql;
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    if (_hanbaiJissekiListCriteria.日計)
                    {
                        sql = GetDailySQL(cmd);     // 販売実績一覧表(日計)用SQLを取得
                    }
                    else
                    {
                        sql = GetMonthlySQL(cmd);   // 販売実績一覧表(月計)用SQLを取得
                    }

                    cmd.CommandText = sql;

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("販売実績一覧表");
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
        /// 販売実績一覧表(日計)用SQL作成
        /// </summary>
        /// <param name="cmd">パラメータ設定用SqlCommand</param>
        /// <returns>販売実績一覧表(日計)用SQL</returns>
        private string GetDailySQL(SqlCommand cmd)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                // -------
                // -- 明細
                // -------
                sql.AppendLine(" SELECT");
                sql.AppendFormat("     '{0:yyyy年MM月dd日(ddd曜日)}' AS 精算日", _hanbaiJissekiListCriteria.精算日).AppendLine();
                sql.AppendLine("     ,'販売実績一覧表（日計）' AS タイトル");
                sql.AppendLine("     ,C.部署ＣＤ");
                sql.AppendLine("     ,BM.部署名");
                sql.AppendLine("     ,C.コースＣＤ");
                sql.AppendLine("     ,C.コース名");
                sql.AppendLine("     ,UB.配送担当者ＣＤ AS 担当者ＣＤ");
                sql.AppendLine("     ,UB.配送担当者名 AS 担当者名");
                sql.AppendLine("     ,UC.持出数副食");
                sql.AppendLine("     ,UC.持出数主食");
                sql.AppendLine("     ,UC.工場追加数副食 + もらった数副食 AS 貰い副食");
                sql.AppendLine("     ,UC.工場追加数主食 + もらった数主食 AS 貰い主食");
                sql.AppendLine("     ,UC.やった数副食 AS 渡し副食");
                sql.AppendLine("     ,UC.やった数主食 AS 渡し主食");
                sql.AppendLine("     ,UC.販売数副食");
                sql.AppendLine("     ,UC.販売数主食");
                sql.AppendLine("     ,UC.残数副食");
                sql.AppendLine("     ,UC.残数主食");
                sql.AppendLine("     ,UA.弁当数量 + UA.副食数量 AS 食数");
                sql.AppendLine("     ,UB.現金売上");
                sql.AppendLine("     ,UB.掛売売上");
                sql.AppendLine("     ,UB.E売上");
                sql.AppendLine("     ,UB.その他");
                sql.AppendLine("     ,UB.実質売上");
                sql.AppendLine("     ,CASE WHEN (UA.弁当数量 + UA.副食数量) = 0 THEN 0 ELSE FLOOR(UB.実質売上 / (UA.弁当数量 + UA.副食数量)) END AS 平均単価");
                sql.AppendLine("     ,0 AS 合計行フラグ");
                sql.AppendLine("     ,C.部署ＣＤ AS ソート用部署ＣＤ");
                sql.AppendLine("     ,C.コースＣＤ AS ソート用コースＣＤ");
                sql.AppendLine(" FROM");
                // -- ①駆動表
                // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         CM.部署ＣＤ");
                sql.AppendLine("         ,CM.コースＣＤ");
                sql.AppendLine("         ,CM.コース名");
                sql.AppendLine("         ,CM.担当者ＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         コースマスタ AS CM");
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 コースＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 臨時コースマスタ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("                 AND ISNULL(工場フラグ, 0) = 0");
                sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine("                 AND 実施日 = @精算日");
                sql.AppendLine("             ) AS RM");
                sql.AppendLine("             ON RM.コースＣＤ = CM.コースＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         CM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND CM.コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND CM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(CM.工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(CM.無効フラグ, 0) = 0");
                sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                sql.AppendLine("     UNION");
                sql.AppendLine("     SELECT");
                sql.AppendLine("         RM.部署ＣＤ");
                sql.AppendLine("         ,RM.コースＣＤ");
                sql.AppendLine("         ,RM.コース名");
                sql.AppendLine("         ,RM.担当者ＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         臨時コーステーブル AS RT");
                sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         RM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND RM.コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND RM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(RM.工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                sql.AppendLine("         AND RM.実施日 = @精算日");
                sql.AppendLine("     ) AS C");
                // -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_売上明細 AS UM");
                sql.AppendLine("             INNER JOIN 商品マスタ AS SM");
                sql.AppendLine("                 ON UM.企業ＣＤ = SM.企業ＣＤ");
                sql.AppendLine("                 AND UM.商品ＣＤ = SM.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             UM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND UM.精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND UM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("         ) AS UA");
                sql.AppendLine("     ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ");
                // -- ③売上精算_ヘッダから金額を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("             ,B.配送担当者ＣＤ");
                sql.AppendLine("             ,B.配送担当者名");
                sql.AppendLine("             ,B.現金売上");
                sql.AppendLine("             ,B.掛売売上");
                sql.AppendLine("             ,B.E売上");
                sql.AppendLine("             ,B.その他");
                sql.AppendLine("             ,B.現金売上 + B.掛売売上 + B.E売上 + B.その他 AS 実質売上");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 部署ＣＤ");
                sql.AppendLine("                 ,コースＣＤ");
                sql.AppendLine("                 ,配送担当者ＣＤ");
                sql.AppendLine("                 ,配送担当者名");
                sql.AppendLine("                 ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上");
                sql.AppendLine("                 ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上");
                sql.AppendLine("                 ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上");
                sql.AppendLine("                 ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上精算_ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND 精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("             ) AS B");
                sql.AppendLine("         ) AS UB");
                sql.AppendLine("     ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ");
                // -- ④売上精算_持出明細から持ち出し数を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             M.精算日");
                sql.AppendLine("             ,M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_持出明細 AS M");
                sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND M.精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             M.精算日");
                sql.AppendLine("             ,M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("         ) AS UC");
                sql.AppendLine("     ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = C.部署ＣＤ");
                sql.AppendLine(" UNION");
                // -----------------
                // -- 部署ごとの小計
                // -----------------
                sql.AppendLine(" SELECT");
                sql.AppendFormat("     '{0:yyyy年MM月dd日(ddd曜日)}' AS 精算日", _hanbaiJissekiListCriteria.精算日).AppendLine();
                sql.AppendLine("     ,'販売実績一覧表（日計）' AS タイトル");
                sql.AppendLine("     ,C.部署ＣＤ");
                sql.AppendLine("     ,BM.部署名");
                sql.AppendLine("     ,NULL AS コースＣＤ");
                sql.AppendLine("     ,BM.部署名 + ' 計'  AS コース名");
                sql.AppendLine("     ,NULL AS 担当者ＣＤ");
                sql.AppendLine("     ,NULL AS 担当者名");
                sql.AppendLine("     ,SUM(UC.持出数副食) AS 持出数副食");
                sql.AppendLine("     ,SUM(UC.持出数主食) AS 持出数主食");
                sql.AppendLine("     ,SUM(UC.工場追加数副食 + もらった数副食) AS 貰い副食");
                sql.AppendLine("     ,SUM(UC.工場追加数主食 + もらった数主食) AS 貰い主食");
                sql.AppendLine("     ,SUM(UC.やった数副食) AS 渡し副食");
                sql.AppendLine("     ,SUM(UC.やった数主食) AS 渡し主食");
                sql.AppendLine("     ,SUM(UC.販売数副食) AS 販売数副食");
                sql.AppendLine("     ,SUM(UC.販売数主食) AS 販売数主食");
                sql.AppendLine("     ,SUM(UC.残数副食) AS 残数副食");
                sql.AppendLine("     ,SUM(UC.残数主食) AS 残数主食");
                sql.AppendLine("     ,SUM(UA.弁当数量 + UA.副食数量) AS 食数");
                sql.AppendLine("     ,SUM(UB.現金売上) AS 現金売上");
                sql.AppendLine("     ,SUM(UB.掛売売上) AS 掛売売上");
                sql.AppendLine("     ,SUM(UB.E売上) AS E売上");
                sql.AppendLine("     ,SUM(UB.その他) AS その他");
                sql.AppendLine("     ,SUM(UB.実質売上) AS 実質売上");
                sql.AppendLine("     ,CASE WHEN (SUM(UA.弁当数量 + UA.副食数量)) = 0 THEN 0 ELSE FLOOR(SUM(UB.実質売上) / SUM(UA.弁当数量 + UA.副食数量)) END AS 平均単価");
                sql.AppendLine("     ,1 AS 合計行フラグ");
                sql.AppendLine("     ,C.部署ＣＤ AS ソート用部署ＣＤ");
                sql.AppendFormat("     ,{0} AS ソート用コースＣＤ", DELIMITER_COURSE_CD);
                sql.AppendLine(" FROM");
                // -- ①駆動表
                // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         CM.部署ＣＤ");
                sql.AppendLine("         ,CM.コースＣＤ");
                sql.AppendLine("         ,CM.コース名");
                sql.AppendLine("         ,CM.担当者ＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         コースマスタ AS CM");
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 コースＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 臨時コースマスタ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("                 AND ISNULL(工場フラグ, 0) = 0");
                sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine("                 AND 実施日 = @精算日");
                sql.AppendLine("             ) AS RM");
                sql.AppendLine("             ON RM.コースＣＤ = CM.コースＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         CM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND CM.コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND CM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(CM.工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(CM.無効フラグ, 0) = 0");
                sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                sql.AppendLine("     UNION");
                sql.AppendLine("     SELECT");
                sql.AppendLine("         RM.部署ＣＤ");
                sql.AppendLine("         ,RM.コースＣＤ");
                sql.AppendLine("         ,RM.コース名");
                sql.AppendLine("         ,RM.担当者ＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         臨時コーステーブル AS RT");
                sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         RM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND RM.コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND RM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(RM.工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                sql.AppendLine("         AND RM.実施日 = @精算日");
                sql.AppendLine("     ) AS C");
                // -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_売上明細 AS UM");
                sql.AppendLine("             INNER JOIN 商品マスタ AS SM");
                sql.AppendLine("                 ON UM.企業ＣＤ = SM.企業ＣＤ");
                sql.AppendLine("                 AND UM.商品ＣＤ = SM.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             UM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND UM.精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND UM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("         ) AS UA");
                sql.AppendLine("     ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ");
                // -- ③売上精算_ヘッダから金額を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("             ,B.配送担当者ＣＤ");
                sql.AppendLine("             ,B.配送担当者名");
                sql.AppendLine("             ,B.現金売上");
                sql.AppendLine("             ,B.掛売売上");
                sql.AppendLine("             ,B.E売上");
                sql.AppendLine("             ,B.その他");
                sql.AppendLine("             ,B.現金売上 + B.掛売売上 + B.E売上 + B.その他 AS 実質売上");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 部署ＣＤ");
                sql.AppendLine("                 ,コースＣＤ");
                sql.AppendLine("                 ,配送担当者ＣＤ");
                sql.AppendLine("                 ,配送担当者名");
                sql.AppendLine("                 ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上");
                sql.AppendLine("                 ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上");
                sql.AppendLine("                 ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上");
                sql.AppendLine("                 ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上精算_ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND 精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("             ) AS B");
                sql.AppendLine("         ) AS UB");
                sql.AppendLine("     ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ");
                // -- ④売上精算_持出明細から持ち出し数を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             M.精算日");
                sql.AppendLine("             ,M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_持出明細 AS M");
                sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND M.精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             M.精算日");
                sql.AppendLine("             ,M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("         ) AS UC");
                sql.AppendLine("     ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = C.部署ＣＤ");
                sql.AppendLine(" GROUP BY");
                sql.AppendLine("     C.部署ＣＤ");
                sql.AppendLine("     ,BM.部署名");
                sql.AppendLine(" UNION");
                // ---------
                // -- 総合計
                // ---------
                sql.AppendLine(" SELECT");
                sql.AppendFormat("     '{0:yyyy年MM月dd日(ddd曜日)}' AS 精算日", _hanbaiJissekiListCriteria.精算日).AppendLine();
                sql.AppendLine("     ,'販売実績一覧表（日計）' AS タイトル");
                sql.AppendLine("     ,NULL AS 部署ＣＤ");
                sql.AppendLine("     ,NULL AS 部署名");
                sql.AppendLine("     ,NULL AS コースＣＤ");
                sql.AppendLine("     ,'総 合 計'  AS コース名");
                sql.AppendLine("     ,NULL AS 担当者ＣＤ");
                sql.AppendLine("     ,NULL AS 担当者名");
                sql.AppendLine("     ,SUM(UC.持出数副食) AS 持出数副食");
                sql.AppendLine("     ,SUM(UC.持出数主食) AS 持出数主食");
                sql.AppendLine("     ,SUM(UC.工場追加数副食 + もらった数副食) AS 貰い副食");
                sql.AppendLine("     ,SUM(UC.工場追加数主食 + もらった数主食) AS 貰い主食");
                sql.AppendLine("     ,SUM(UC.やった数副食) AS 渡し副食");
                sql.AppendLine("     ,SUM(UC.やった数主食) AS 渡し主食");
                sql.AppendLine("     ,SUM(UC.販売数副食) AS 販売数副食");
                sql.AppendLine("     ,SUM(UC.販売数主食) AS 販売数主食");
                sql.AppendLine("     ,SUM(UC.残数副食) AS 残数副食");
                sql.AppendLine("     ,SUM(UC.残数主食) AS 残数主食");
                sql.AppendLine("     ,SUM(UA.弁当数量 + UA.副食数量) AS 食数");
                sql.AppendLine("     ,SUM(UB.現金売上) AS 現金売上");
                sql.AppendLine("     ,SUM(UB.掛売売上) AS 掛売売上");
                sql.AppendLine("     ,SUM(UB.E売上) AS E売上");
                sql.AppendLine("     ,SUM(UB.その他) AS その他");
                sql.AppendLine("     ,SUM(UB.実質売上) AS 実質売上");
                sql.AppendLine("     ,CASE WHEN (SUM(UA.弁当数量 + UA.副食数量)) = 0 THEN 0 ELSE FLOOR(SUM(UB.実質売上) / SUM(UA.弁当数量 + UA.副食数量)) END AS 平均単価");
                sql.AppendLine("     ,1 AS 合計行フラグ");
                sql.AppendFormat("     ,{0} AS ソート用部署ＣＤ", DELIMITER_BUSHO_CD);
                sql.AppendFormat("     ,{0} AS ソート用コースＣＤ", DELIMITER_COURSE_CD);
                sql.AppendLine(" FROM");
                // -- ①駆動表
                // -- コースマスタから取得（実施日に該当する臨時コースマスタがあればコースマスタからは取得しない。無ければ取得する）
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         CM.部署ＣＤ");
                sql.AppendLine("         ,CM.コースＣＤ");
                sql.AppendLine("         ,CM.コース名");
                sql.AppendLine("         ,CM.担当者ＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         コースマスタ AS CM");
                sql.AppendLine("         LEFT JOIN");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 コースＣＤ");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 臨時コースマスタ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("                 AND ISNULL(工場フラグ, 0) = 0");
                sql.AppendLine("                 AND ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine("                 AND 実施日 = @精算日");
                sql.AppendLine("             ) AS RM");
                sql.AppendLine("             ON RM.コースＣＤ = CM.コースＣＤ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         CM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND CM.コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND CM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(CM.工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(CM.無効フラグ, 0) = 0");
                sql.AppendLine("         AND RM.コースＣＤ IS NULL");
                // -- 臨時コーステーブルから取得（実施日に該当する臨時コーステーブルがあれば取得する。無ければ取得しない）
                sql.AppendLine("     UNION");
                sql.AppendLine("     SELECT");
                sql.AppendLine("         RM.部署ＣＤ");
                sql.AppendLine("         ,RM.コースＣＤ");
                sql.AppendLine("         ,RM.コース名");
                sql.AppendLine("         ,RM.担当者ＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         臨時コーステーブル AS RT");
                sql.AppendLine("         INNER JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = RT.企業ＣＤ AND RM.コースＣＤ = RT.コースＣＤ AND RT.実施日 = RM.実施日 AND ISNULL(RM.無効フラグ, 0) = 0");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         RM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND RM.コース区分 = @コース区分");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND RM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(RM.工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(RT.無効フラグ, 0) = 0");
                sql.AppendLine("         AND RM.実施日 = @精算日");
                sql.AppendLine("     ) AS C");
                // -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_売上明細 AS UM");
                sql.AppendLine("             INNER JOIN 商品マスタ AS SM");
                sql.AppendLine("                 ON UM.企業ＣＤ = SM.企業ＣＤ");
                sql.AppendLine("                 AND UM.商品ＣＤ = SM.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             UM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND UM.精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND UM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("         ) AS UA");
                sql.AppendLine("     ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ");
                // -- ③売上精算_ヘッダから金額を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("             ,B.配送担当者ＣＤ");
                sql.AppendLine("             ,B.配送担当者名");
                sql.AppendLine("             ,B.現金売上");
                sql.AppendLine("             ,B.掛売売上");
                sql.AppendLine("             ,B.E売上");
                sql.AppendLine("             ,B.その他");
                sql.AppendLine("             ,B.現金売上 + B.掛売売上 + B.E売上 + B.その他 AS 実質売上");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 部署ＣＤ");
                sql.AppendLine("                 ,コースＣＤ");
                sql.AppendLine("                 ,配送担当者ＣＤ");
                sql.AppendLine("                 ,配送担当者名");
                sql.AppendLine("                 ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上");
                sql.AppendLine("                 ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上");
                sql.AppendLine("                 ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上");
                sql.AppendLine("                 ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上精算_ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND 精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("             ) AS B");
                sql.AppendLine("         ) AS UB");
                sql.AppendLine("     ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ");
                // -- ④売上精算_持出明細から持ち出し数を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             M.精算日");
                sql.AppendLine("             ,M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_持出明細 AS M");
                sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND M.精算日 = @精算日");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             M.精算日");
                sql.AppendLine("             ,M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("         ) AS UC");
                sql.AppendLine("     ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     ソート用部署ＣＤ");
                sql.AppendLine("     ,ソート用コースＣＤ");

                // パラメータを設定
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = (int)_hanbaiJissekiListCriteria.コース区分;
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_hanbaiJissekiListCriteria.部署ＣＤ;
                }
                cmd.Parameters.Add("@精算日", SqlDbType.Date).Value = (DateTime)_hanbaiJissekiListCriteria.精算日;
                cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;

                return sql.ToString();
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
        /// 販売実績一覧表(月計)用SQL作成
        /// </summary>
        /// <param name="cmd">パラメータ設定用SqlCommand</param>
        /// <returns>販売実績一覧表(日計)用SQL</returns>
        private string GetMonthlySQL(SqlCommand cmd)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                DateTime targetDate = new DateTime((int)_hanbaiJissekiListCriteria.対象年, (int)_hanbaiJissekiListCriteria.対象月, 1);

                // -------
                // -- 明細
                // -------
                sql.AppendLine(" SELECT");
                sql.AppendFormat("     '{0:yyyy年MM月分}' AS 精算日", targetDate).AppendLine();
                sql.AppendLine("     ,'販売実績一覧表（月計）' AS タイトル");
                sql.AppendLine("     ,C.部署ＣＤ");
                sql.AppendLine("     ,BM.部署名");
                sql.AppendLine("     ,C.コースＣＤ");
                sql.AppendLine("     ,CM.コース名");
                sql.AppendLine("     ,CM.担当者ＣＤ");
                sql.AppendLine("     ,TM.担当者名");
                sql.AppendLine("     ,UC.持出数副食");
                sql.AppendLine("     ,UC.持出数主食");
                sql.AppendLine("     ,UC.工場追加数副食 + もらった数副食 AS 貰い副食");
                sql.AppendLine("     ,UC.工場追加数主食 + もらった数主食 AS 貰い主食");
                sql.AppendLine("     ,UC.やった数副食 AS 渡し副食");
                sql.AppendLine("     ,UC.やった数主食 AS 渡し主食");
                sql.AppendLine("     ,UC.販売数副食");
                sql.AppendLine("     ,UC.販売数主食");
                sql.AppendLine("     ,UC.残数副食");
                sql.AppendLine("     ,UC.残数主食");
                sql.AppendLine("     ,UA.弁当数量 + UA.副食数量 AS 食数");
                sql.AppendLine("     ,UB.現金売上");
                sql.AppendLine("     ,UB.掛売売上");
                sql.AppendLine("     ,UB.E売上");
                sql.AppendLine("     ,UB.その他");
                sql.AppendLine("     ,UB.実質売上");
                sql.AppendLine("     ,CASE WHEN (UA.弁当数量 + UA.副食数量) = 0 THEN 0 ELSE FLOOR(UB.実質売上 / (UA.弁当数量 + UA.副食数量)) END AS 平均単価");
                sql.AppendLine("     ,0 AS 合計行フラグ");
                sql.AppendLine("     ,C.部署ＣＤ AS ソート用部署ＣＤ");
                sql.AppendLine("     ,C.コースＣＤ AS ソート用コースＣＤ");
                sql.AppendLine(" FROM");
                // -- ①駆動表
                // -- コースマスタから取得（売上日計表とは異なり、臨時コーステーブルからは取得しない）
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         部署ＣＤ");
                sql.AppendLine("         ,コースＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         コースマスタ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine("     UNION");
                // -- 集計対象の売上精算_ヘッダにあるコースも取得
                sql.AppendLine("     SELECT");
                sql.AppendLine("         部署ＣＤ");
                sql.AppendLine("         ,コースＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         売上精算_ヘッダ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND 精算日 >= @精算日From");
                sql.AppendLine("         AND 精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("     ) AS C");
                // -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_売上明細 AS UM");
                sql.AppendLine("             INNER JOIN 商品マスタ AS SM");
                sql.AppendLine("                 ON UM.企業ＣＤ = SM.企業ＣＤ");
                sql.AppendLine("                 AND UM.商品ＣＤ = SM.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             UM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND UM.精算日 >= @精算日From");
                sql.AppendLine("             AND UM.精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND UM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("         ) AS UA");
                sql.AppendLine("     ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ");
                // -- ③売上精算_ヘッダから金額を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("             ,SUM(B.現金売上) AS 現金売上");
                sql.AppendLine("             ,SUM(B.掛売売上) AS 掛売売上");
                sql.AppendLine("             ,SUM(B.E売上) AS E売上");
                sql.AppendLine("             ,SUM(B.その他) AS その他");
                sql.AppendLine("             ,SUM(B.現金売上 + B.掛売売上 + B.E売上 + B.その他) AS 実質売上");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 部署ＣＤ");
                sql.AppendLine("                 ,コースＣＤ");
                sql.AppendLine("                 ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上");
                sql.AppendLine("                 ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上");
                sql.AppendLine("                 ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上");
                sql.AppendLine("                 ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上精算_ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND 精算日 >= @精算日From");
                sql.AppendLine("                 AND 精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("             ) AS B");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("         ) AS UB");
                sql.AppendLine("     ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ");
                // -- ④売上精算_持出明細から持ち出し数を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_持出明細 AS M");
                sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND M.精算日 >= @精算日From");
                sql.AppendLine("             AND M.精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("         ) AS UC");
                sql.AppendLine("     ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = C.部署ＣＤ");
                sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = @企業ＣＤ AND CM.コースＣＤ = C.コースＣＤ");
                sql.AppendLine("     LEFT JOIN 担当者マスタ AS TM ON TM.企業ＣＤ = CM.企業ＣＤ AND TM.担当者ＣＤ = CM.担当者ＣＤ");
                sql.AppendLine(" UNION");
                // -----------------
                // -- 部署ごとの小計
                // -----------------
                sql.AppendLine(" SELECT");
                sql.AppendFormat("     '{0:yyyy年MM月分}' AS 精算日", targetDate).AppendLine();
                sql.AppendLine("     ,'販売実績一覧表（月計）' AS タイトル");
                sql.AppendLine("     ,C.部署ＣＤ");
                sql.AppendLine("     ,BM.部署名");
                sql.AppendLine("     ,NULL AS コースＣＤ");
                sql.AppendLine("     ,BM.部署名 + ' 計'  AS コース名");
                sql.AppendLine("     ,NULL AS 担当者ＣＤ");
                sql.AppendLine("     ,NULL AS 担当者名");
                sql.AppendLine("     ,SUM(UC.持出数副食) AS 持出数副食");
                sql.AppendLine("     ,SUM(UC.持出数主食) AS 持出数主食");
                sql.AppendLine("     ,SUM(UC.工場追加数副食 + もらった数副食) AS 貰い副食");
                sql.AppendLine("     ,SUM(UC.工場追加数主食 + もらった数主食) AS 貰い主食");
                sql.AppendLine("     ,SUM(UC.やった数副食) AS 渡し副食");
                sql.AppendLine("     ,SUM(UC.やった数主食) AS 渡し主食");
                sql.AppendLine("     ,SUM(UC.販売数副食) AS 販売数副食");
                sql.AppendLine("     ,SUM(UC.販売数主食) AS 販売数主食");
                sql.AppendLine("     ,SUM(UC.残数副食) AS 残数副食");
                sql.AppendLine("     ,SUM(UC.残数主食) AS 残数主食");
                sql.AppendLine("     ,SUM(UA.弁当数量 + UA.副食数量) AS 食数");
                sql.AppendLine("     ,SUM(UB.現金売上) AS 現金売上");
                sql.AppendLine("     ,SUM(UB.掛売売上) AS 掛売売上");
                sql.AppendLine("     ,SUM(UB.E売上) AS E売上");
                sql.AppendLine("     ,SUM(UB.その他) AS その他");
                sql.AppendLine("     ,SUM(UB.実質売上) AS 実質売上");
                sql.AppendLine("     ,CASE WHEN (SUM(UA.弁当数量 + UA.副食数量)) = 0 THEN 0 ELSE FLOOR(SUM(UB.実質売上) / SUM(UA.弁当数量 + UA.副食数量)) END AS 平均単価");
                sql.AppendLine("     ,1 AS 合計行フラグ");
                sql.AppendLine("     ,C.部署ＣＤ AS ソート用部署ＣＤ");
                sql.AppendFormat("     ,{0} AS ソート用コースＣＤ", DELIMITER_COURSE_CD);
                sql.AppendLine(" FROM");
                // -- ①駆動表
                // -- コースマスタから取得（売上日計表とは異なり、臨時コーステーブルからは取得しない）
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         部署ＣＤ");
                sql.AppendLine("         ,コースＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         コースマスタ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine("     UNION");
                // -- 集計対象の売上精算_ヘッダにあるコースも取得
                sql.AppendLine("     SELECT");
                sql.AppendLine("         部署ＣＤ");
                sql.AppendLine("         ,コースＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         売上精算_ヘッダ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND 精算日 >= @精算日From");
                sql.AppendLine("         AND 精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("     ) AS C");
                // -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_売上明細 AS UM");
                sql.AppendLine("             INNER JOIN 商品マスタ AS SM");
                sql.AppendLine("                 ON UM.企業ＣＤ = SM.企業ＣＤ");
                sql.AppendLine("                 AND UM.商品ＣＤ = SM.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             UM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND UM.精算日 >= @精算日From");
                sql.AppendLine("             AND UM.精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND UM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("         ) AS UA");
                sql.AppendLine("     ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ");
                // -- ③売上精算_ヘッダから金額を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("             ,SUM(B.現金売上) AS 現金売上");
                sql.AppendLine("             ,SUM(B.掛売売上) AS 掛売売上");
                sql.AppendLine("             ,SUM(B.E売上) AS E売上");
                sql.AppendLine("             ,SUM(B.その他) AS その他");
                sql.AppendLine("             ,SUM(B.現金売上 + B.掛売売上 + B.E売上 + B.その他) AS 実質売上");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 部署ＣＤ");
                sql.AppendLine("                 ,コースＣＤ");
                sql.AppendLine("                 ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上");
                sql.AppendLine("                 ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上");
                sql.AppendLine("                 ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上");
                sql.AppendLine("                 ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上精算_ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND 精算日 >= @精算日From");
                sql.AppendLine("                 AND 精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("             ) AS B");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("         ) AS UB");
                sql.AppendLine("     ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ");
                // -- ④売上精算_持出明細から持ち出し数を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_持出明細 AS M");
                sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND M.精算日 >= @精算日From");
                sql.AppendLine("             AND M.精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("         ) AS UC");
                sql.AppendLine("     ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ");
                sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = C.部署ＣＤ");
                sql.AppendLine(" GROUP BY");
                sql.AppendLine("     C.部署ＣＤ");
                sql.AppendLine("     ,BM.部署名");
                sql.AppendLine(" UNION");
                // -----------------
                // -- 総合計
                // -----------------
                sql.AppendLine(" SELECT");
                sql.AppendFormat("     '{0:yyyy年MM月分}' AS 精算日", targetDate).AppendLine();
                sql.AppendLine("     ,'販売実績一覧表（月計）' AS タイトル");
                sql.AppendLine("     ,NULL AS 部署ＣＤ");
                sql.AppendLine("     ,NULL AS 部署名");
                sql.AppendLine("     ,NULL AS コースＣＤ");
                sql.AppendLine("     ,'総 合 計'  AS コース名");
                sql.AppendLine("     ,NULL AS 担当者ＣＤ");
                sql.AppendLine("     ,NULL AS 担当者名");
                sql.AppendLine("     ,SUM(UC.持出数副食) AS 持出数副食");
                sql.AppendLine("     ,SUM(UC.持出数主食) AS 持出数主食");
                sql.AppendLine("     ,SUM(UC.工場追加数副食 + もらった数副食) AS 貰い副食");
                sql.AppendLine("     ,SUM(UC.工場追加数主食 + もらった数主食) AS 貰い主食");
                sql.AppendLine("     ,SUM(UC.やった数副食) AS 渡し副食");
                sql.AppendLine("     ,SUM(UC.やった数主食) AS 渡し主食");
                sql.AppendLine("     ,SUM(UC.販売数副食) AS 販売数副食");
                sql.AppendLine("     ,SUM(UC.販売数主食) AS 販売数主食");
                sql.AppendLine("     ,SUM(UC.残数副食) AS 残数副食");
                sql.AppendLine("     ,SUM(UC.残数主食) AS 残数主食");
                sql.AppendLine("     ,SUM(UA.弁当数量 + UA.副食数量) AS 食数");
                sql.AppendLine("     ,SUM(UB.現金売上) AS 現金売上");
                sql.AppendLine("     ,SUM(UB.掛売売上) AS 掛売売上");
                sql.AppendLine("     ,SUM(UB.E売上) AS E売上");
                sql.AppendLine("     ,SUM(UB.その他) AS その他");
                sql.AppendLine("     ,SUM(UB.実質売上) AS 実質売上");
                sql.AppendLine("     ,CASE WHEN (SUM(UA.弁当数量 + UA.副食数量)) = 0 THEN 0 ELSE FLOOR(SUM(UB.実質売上) / SUM(UA.弁当数量 + UA.副食数量)) END AS 平均単価");
                sql.AppendLine("     ,1 AS 合計行フラグ");
                sql.AppendFormat("     ,{0} AS ソート用部署ＣＤ", DELIMITER_BUSHO_CD);
                sql.AppendFormat("     ,{0} AS ソート用コースＣＤ", DELIMITER_COURSE_CD);
                sql.AppendLine(" FROM");
                // -- ①駆動表
                // -- コースマスタから取得（売上日計表とは異なり、臨時コーステーブルからは取得しない）
                sql.AppendLine("     (SELECT");
                sql.AppendLine("         部署ＣＤ");
                sql.AppendLine("         ,コースＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         コースマスタ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         AND ISNULL(工場フラグ, 0) = 0");
                sql.AppendLine("         AND ISNULL(無効フラグ, 0) = 0");
                sql.AppendLine("     UNION");
                // -- 集計対象の売上精算_ヘッダにあるコースも取得
                sql.AppendLine("     SELECT");
                sql.AppendLine("         部署ＣＤ");
                sql.AppendLine("         ,コースＣＤ");
                sql.AppendLine("     FROM");
                sql.AppendLine("         売上精算_ヘッダ");
                sql.AppendLine("     WHERE");
                sql.AppendLine("         企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("         AND 精算日 >= @精算日From");
                sql.AppendLine("         AND 精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("     ) AS C");
                // -- ②売上精算_売上明細から弁当・副食の数量（＝食数）を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分弁当 THEN UM.売上数量 ELSE 0 END) AS 弁当数量");
                sql.AppendLine("             ,SUM(CASE WHEN SM.弁当区分 = @弁当区分副食 THEN UM.売上数量 ELSE 0 END) AS 副食数量");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_売上明細 AS UM");
                sql.AppendLine("             INNER JOIN 商品マスタ AS SM");
                sql.AppendLine("                 ON UM.企業ＣＤ = SM.企業ＣＤ");
                sql.AppendLine("                 AND UM.商品ＣＤ = SM.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             UM.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND UM.精算日 >= @精算日From");
                sql.AppendLine("             AND UM.精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND UM.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             UM.部署ＣＤ");
                sql.AppendLine("             ,UM.コースＣＤ");
                sql.AppendLine("         ) AS UA");
                sql.AppendLine("     ON UA.部署ＣＤ = C.部署ＣＤ AND UA.コースＣＤ = C.コースＣＤ");
                // -- ③売上精算_ヘッダから金額を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("             ,SUM(B.現金売上) AS 現金売上");
                sql.AppendLine("             ,SUM(B.掛売売上) AS 掛売売上");
                sql.AppendLine("             ,SUM(B.E売上) AS E売上");
                sql.AppendLine("             ,SUM(B.その他) AS その他");
                sql.AppendLine("             ,SUM(B.現金売上 + B.掛売売上 + B.E売上 + B.その他) AS 実質売上");
                sql.AppendLine("         FROM");
                sql.AppendLine("             (SELECT");
                sql.AppendLine("                 部署ＣＤ");
                sql.AppendLine("                 ,コースＣＤ");
                sql.AppendLine("                 ,ISNULL(現金_売上合計, 0) - ISNULL(現金_値引合計, 0) AS 現金売上");
                sql.AppendLine("                 ,ISNULL(掛売_売上合計, 0) - ISNULL(掛売_値引合計, 0) AS 掛売売上");
                sql.AppendLine("                 ,ISNULL(E_売上合計, 0)    - ISNULL(E_値引合計, 0)    AS E売上");
                sql.AppendLine("                 ,ISNULL(合計_チケット売上, 0) + ISNULL(合計_バークレー売上, 0) AS その他");
                sql.AppendLine("             FROM");
                sql.AppendLine("                 売上精算_ヘッダ");
                sql.AppendLine("             WHERE");
                sql.AppendLine("                 企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("                 AND 精算日 >= @精算日From");
                sql.AppendLine("                 AND 精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("                 AND 部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("             ) AS B");
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             B.部署ＣＤ");
                sql.AppendLine("             ,B.コースＣＤ");
                sql.AppendLine("         ) AS UB");
                sql.AppendLine("     ON UB.部署ＣＤ = C.部署ＣＤ AND UB.コースＣＤ = C.コースＣＤ");
                // -- ④売上精算_持出明細から持ち出し数を取得
                sql.AppendLine("     LEFT JOIN");
                sql.AppendLine("         (SELECT");
                sql.AppendLine("             M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.持出個数, 0)       ELSE 0 END) AS 持出数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.工場追加数, 0)     ELSE 0 END) AS 工場追加数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.もらった数, 0)     ELSE 0 END) AS もらった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.やった数, 0)       ELSE 0 END) AS やった数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.残数, 0)           ELSE 0 END) AS 残数主食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分副食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数副食");
                sql.AppendLine("             ,SUM(CASE WHEN S.弁当区分 = @弁当区分主食 THEN ISNULL(M.売上予定実績数, 0) ELSE 0 END) AS 販売数主食");
                sql.AppendLine("         FROM");
                sql.AppendLine("             売上精算_持出明細 AS M");
                sql.AppendLine("             LEFT JOIN 商品マスタ AS S ON S.企業ＣＤ = M.企業ＣＤ AND S.商品ＣＤ = M.商品ＣＤ");
                sql.AppendLine("         WHERE");
                sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                sql.AppendLine("             AND M.精算日 >= @精算日From");
                sql.AppendLine("             AND M.精算日 <= @精算日To");
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    sql.AppendLine("             AND M.部署ＣＤ = @部署ＣＤ");
                }
                sql.AppendLine("         GROUP BY");
                sql.AppendLine("             M.部署ＣＤ");
                sql.AppendLine("             ,M.コースＣＤ");
                sql.AppendLine("         ) AS UC");
                sql.AppendLine("     ON UC.部署ＣＤ = C.部署ＣＤ AND UC.コースＣＤ = C.コースＣＤ");
                sql.AppendLine(" ORDER BY");
                sql.AppendLine("     ソート用部署ＣＤ");
                sql.AppendLine("     ,ソート用コースＣＤ");

                // パラメータを設定
                DateTime fromDate = (DateTime)targetDate;
                DateTime toDate = new DateTime(fromDate.Year, fromDate.Month, DateTime.DaysInMonth(fromDate.Year, fromDate.Month));
                cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                cmd.Parameters.Add("@コース区分", SqlDbType.Int).Value = (int)_hanbaiJissekiListCriteria.コース区分;
                if (_hanbaiJissekiListCriteria.部署ＣＤ != null && _hanbaiJissekiListCriteria.部署ＣＤ >= 0)
                {
                    cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = (int)_hanbaiJissekiListCriteria.部署ＣＤ;
                }
                cmd.Parameters.Add("@精算日From", SqlDbType.Date).Value = fromDate;
                cmd.Parameters.Add("@精算日To", SqlDbType.Date).Value = toDate;
                cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;

                return sql.ToString();
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

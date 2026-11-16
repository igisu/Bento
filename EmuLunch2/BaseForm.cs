using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Forms;
using Emu.Bento.Common;
using Emu.Bento.Data;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Export.Pdf.Section;
using log4net;

namespace Emu.Bento
{
    public partial class BaseForm : Form
    {
        //**************************************
        // 内部クラス／構造体
        //**************************************
        #region 内部クラス／構造体
        /// <summary>
        /// Progressクラスに渡す型パラメータークラス（プログレスバーを操作するパラメータ）
        /// </summary>
        public class ProgressState
        {
            /// <summary>
            /// ラベルに表示するメッセージ
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// プログレスバーの上限
            /// </summary>
            public int Maximum { get; set; }

            /// <summary>
            /// プログレスバーの値
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// プログレスバーのスタイル
            /// </summary>
            public ProgressBarStyle Style { get; set; }
        }
        #endregion

        //**************************************
        //* 定数
        //**************************************
        #region 定数
        /// <summary>
        /// 新規モードキャプション
        /// </summary>
        public const string MODE_CAPTION_NEW = "新規モード";

        /// <summary>
        /// 編集モードキャプション
        /// </summary>
        public const string MODE_CAPTION_EDIT = "編集モード";

        /// <summary>
        /// 新規モードの背景色
        /// </summary>
        public readonly Color BACK_COLOR_NEW = Color.FromArgb(51, 153, 153);

        /// <summary>
        /// 編集モードの背景色
        /// </summary>
        public readonly Color BACK_COLOR_EDIT = Color.FromArgb(255, 153, 0);

        /// <summary>
        /// 合計行の背景色
        /// </summary>
        public readonly Color TOTAL_BACK_COLOR = Color.LightYellow;

        /// <summary>
        /// 現金のイメージカラー（明るいピンク）
        /// </summary>
        public readonly Color GENKIN_COLOR = Color.FromArgb(255, 209, 209);

        /// <summary>
        /// 掛売のイメージカラー（明るい黄緑）
        /// </summary>
        public readonly Color KAKEURI_COLOR = Color.FromArgb(203, 242, 102);

        /// <summary>
        /// 外税のイメージカラー（クリーム）
        /// </summary>
        public readonly Color SOTOZEI_COLOR = Color.FromArgb(255, 255, 153);

        /// <summary>
        /// 非課税のイメージカラー（明るい緑）
        /// </summary>
        public readonly Color HIKAZEI_COLOR = Color.FromArgb(135, 231, 176);

        /// <summary>
        /// 切り上げのイメージカラー（明るい空色）
        /// </summary>
        public readonly Color CEILING_COLOR = Color.FromArgb(180, 235, 250);

        /// <summary>
        /// 切り捨てイメージカラー（ベージュ）
        /// </summary>
        public readonly Color FLOOR_COLOR = Color.FromArgb(237, 197, 143);

        /// <summary>
        /// 四捨五入イメージカラー（明るい紫）
        /// </summary>
        public readonly Color ROUND_COLOR = Color.FromArgb(199, 178, 222);

        /// <summary>
        /// クレジットイメージカラー（オレンジ）
        /// </summary>
        public readonly Color CREDIT_COLOR = Color.FromArgb(255, 190, 72);

        /// <summary>
        /// 電子マネーイメージカラー（暗め空色）
        /// </summary>
        public readonly Color EMONEY_COLOR = Color.FromArgb(147, 205, 221);

        public const double DISABLE_ALPHA = 0.2d;   // 非活性時の不透明度
        public const double ENABLE_ALPHA = 1d;      // 活性時の不透明度(1固定)

        private const int ON_PROGRESS_INTERVAL_MS = 15;     // 進捗状況を更新する間隔
        #endregion

        //**************************************
        //* スタティック変数
        //**************************************
        #region スタティック変数
        /// <summary>
        /// ロガー
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 権限読み込み済みか
        /// </summary>
        protected bool _hasAuth = false;

        /// <summary>
        /// このフォームに対する閲覧権限があるか
        /// </summary>
        protected bool? _canBrowse = false;

        /// <summary>
        /// このフォームに対する編集権限があるか
        /// </summary>
        protected bool? _canEdit = false;

        /// <summary>
        /// 日付ごとの祝日マップ
        /// </summary>
        private Dictionary<DateTime, Shukujitsu> _shukujitsuMap = new Dictionary<DateTime, Shukujitsu>();

        /// <summary>
        /// 前回 ShowProgress した時刻
        /// </summary>
        private DateTime _lastProgressTime = DateTime.Now;

        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// このフォームに対する閲覧権限があるか
        /// </summary>
        /// <returns>閲覧権限がある場合はtrue</returns>
        public bool HasAuthBrowse
        {
            get
            {
                if (_hasAuth == false)
                {
                    GetAuth();
                }
                return (bool)_canBrowse;
            }
        }

        /// <summary>
        /// このフォームに対する編集権限があるか
        /// </summary>
        /// <returns>編集権限がある場合はtrue</returns>
        public bool HasAuthEdit
        {
            get
            {
                if (_hasAuth == false)
                {
                    GetAuth();
                }
                return (bool)_canEdit;
            }
        }
        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BaseForm()
        {
            InitializeComponent();
        }
        #endregion

        //**************************************
        //* メソッド
        //**************************************
        #region メソッド

        /// <summary>
        /// 画面権限マスタを読み込む
        /// </summary>
        protected void GetAuth()
        {
            try
            {
                _canBrowse = true;
                _canEdit = true;
                _hasAuth = true;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();

                    cmd.CommandText = "\r\n SELECT " +
                                      "\r\n     画面権限No, " +
                                      "\r\n     メニュー名, " +
                                      "\r\n     画面名, " +
                                      "\r\n     閲覧可能フラグ, " +
                                      "\r\n     編集可能フラグ " +
                                      "\r\n FROM " +
                                      "\r\n     画面権限マスタ " +
                                      "\r\n WHERE " +
                                      "\r\n     画面名 = @画面名 AND " +
                                      "\r\n     権限CD = @権限CD ";
                    cmd.Parameters.Add("@画面名", SqlDbType.VarChar).Value = this.Text;
                    UserData userData = UserData.getInstance();
                    cmd.Parameters.Add("@権限CD", SqlDbType.Int).Value = userData.権限区分 ?? -1;

                    log.Debug(cmd.CommandTextWithParameters());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _canBrowse = reader["閲覧可能フラグ"] as bool?;
                            _canEdit = reader["編集可能フラグ"] as bool?;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 子画面を開く際の準備
        /// </summary>
        /// <param name="target">子画面</param>
        protected void SubFormOpenInit(Form target)
        {
            try
            {
                // 選択ダイアログの場合はモーダルダイアログで開くのでMDIの子供にできない（例外が発生する）
                if (target is IDialog)
                {
                    // 親の中央
                    target.StartPosition = FormStartPosition.CenterParent;
                }
                else
                {
                    this.Enabled = false;                   // 子画面をモーダルフォームっぽく開くため、親画面を非活性にする
                    target.MdiParent = this.MdiParent;
                    // 子画面を親画面の中央に表示
                    target.Left = this.Left + (this.Width - target.Width) / 2;
                    if (target.Left < 0) target.Left = 0;
                    target.Top = this.Top + (this.Height - target.Height) / 2;
                    if (target.Top < 0) target.Top = 0;
                }

                this.BringToFront();                    // 親画面を非活性にすると別の画面の裏に移動するのでそれを回避
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
        /// 指定したフォームが開かれているかを返します。
        /// </summary>
        /// <param name="target">フォームインスタンス</param>
        /// <returns>指定したフォームが開かれている場合は true。それ以外は false</returns>
        protected bool IsOpenForm(Form target)
        {
            if (target == null)
            {
                return false;
            }

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == target.Name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 自分のインスタンスが他にあれば、それをアクティブにする
        /// </summary>
        /// <returns>自分のインスタンスが他にあればTrue</returns>
        public bool ActivateAnotherMe()
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == this.Name)
                {
                    form.Activate();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// コントロールにバインドされているデータを読み込み直します
        /// </summary>
        /// <param name="ctrl">対象コントロール</param>
        protected void ReadBinding(Control ctrl)
        {
            if (ctrl == null)
            {
                return;
            }

            if (ctrl.DataBindings != null && ctrl.DataBindings.Count > 0)
            {
                ctrl.DataBindings[0].ReadValue();
            }
        }

        /// <summary>
        /// コントロールにバインドされているデータをデータソースに反映します
        /// </summary>
        /// <param name="ctrl">対象コントロール</param>
        protected void WriteBinding(Control ctrl)
        {
            if (ctrl == null)
            {
                return;
            }

            if (ctrl.DataBindings != null && ctrl.DataBindings.Count > 0)
            {
                ctrl.DataBindings[0].WriteValue();
            }
        }

        /// <summary>
        /// 指定した不透明度の前景色を、背景色に重ねたときの色を返します。
        /// </summary>
        /// <param name="BackColor">背景色</param>
        /// <param name="ForeColor">前景色</param>
        /// <param name="alpha">不透明度(0～1)</param>
        /// <returns>指定した不透明度の前景色を、背景色に重ねたときの色</returns>
        protected Color GetAlphaBlendColor(Color BackColor, Color ForeColor, double alpha)
        {
            byte r = GetAlphaBlendColor(BackColor.R, ForeColor.R, alpha);
            byte g = GetAlphaBlendColor(BackColor.G, ForeColor.G, alpha);
            byte b = GetAlphaBlendColor(BackColor.B, ForeColor.B, alpha);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 半透明計算
        /// </summary>
        /// <param name="BackColor">背景色</param>
        /// <param name="ForeColor">前景色</param>
        /// <param name="alpha">不透明度(0～1)</param>
        /// <returns>半透明値</returns>
        private byte GetAlphaBlendColor(byte backColor, byte foreColor, double alpha)
        {
            // Cr = Cd * (1 - As) + Cs * As
            return (byte)((double)backColor * (1d - alpha) + (double)foreColor * alpha);
        }

        /// <summary>
        /// プリンタ選択ダイアログを表示して、帳票に該当する帳票／プリンタ設定マスタを返します
        /// </summary>
        /// <param name="baseReport">レポートのベース</param>
        /// <returns>帳票／プリンタ設定マスタのレコードが存在する または プリンタ選択ダイアログでOKボタンが押下されたら PrintSettings のインスタンス。キャンセルが押下されたらnull</returns>
        protected PrintSettings GetPrintSettings(BaseReport baseReport)
        {
            //********************************************************************************************************************
            // 帳票を印刷またはプレビューする場合は、以下の手順に従ってください。
            // 1.BaseReport を継承した ActiveReport オブジェクトをインスタンス化し、GetPrintSettings()（このメソッド）を呼び出す。
            // 2.GetPrintSettings()（このメソッド）の戻り値が null の場合は印刷処理をキャンセルする。
            // 3.GetPrintSettings()（このメソッド）の戻り値が null ではない場合は、印刷データを取得し、
            //   ActiveReport オブジェクトの DataSource に設定したあと、PrintReport() を呼び出す。
            // ※上記の手順に従えば、ReportPrint／ReportPreview を直接呼び出す必要はありません。
            //********************************************************************************************************************
            try
            {
                // 帳票IDに該当する帳票／プリンタ設定マスタを取得
                Type type = baseReport.GetType();
                string reportID = type.Name;
                PrintSettings printSettings = PrintSettings.GetPrintSettings(reportID);
                if (printSettings != null)
                {
                    // プレビュー または プリンタが設定済み（かつインストールされている）ならOK
                    if (printSettings.プレビューフラグ == true)
                    {
                        // プリンタ名が保存されているが、そのプリンタがインストールされていない場合は
                        // プレビューに失敗するので、プリンタ名を消す
                        if (string.IsNullOrEmpty(printSettings.プリンタ名) == false)
                        {
                            if (Printers.HasPrinter(printSettings.プリンタ名) == false)
                            {
                                printSettings.プリンタ名 = null;
                            }
                        }
                        return printSettings;
                    }
                    if (string.IsNullOrEmpty(printSettings.プリンタ名) == false)
                    {
                        if (Printers.HasPrinter(printSettings.プリンタ名))
                        {
                            return printSettings;
                        }
                    }
                }

                // マスタにレコードが無い または プリンタが未設定 または 設定済みのプリンタが未インストール の場合はプリンタ選択ダイアログを表示
                FormPrinterSelect form = new FormPrinterSelect(reportID, baseReport.TitleWithSize);
                SubFormOpenInit(form);
                form.ShowDialog();
                if (form.DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return null;
                }
                return form.PrintSettings;
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
        /// 帳票／プリンタ設定マスタの内容に従って、帳票を印刷またはプレビューします。
        /// </summary>
        /// <param name="baseReport">レポートのベース</param>
        /// <param name="printSettings">帳票／プリンタ設定マスタ</param>
        protected void PrintReport(BaseReport baseReport, PrintSettings printSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(printSettings.プリンタ名) == false)
                {
                    if (Printers.HasPrinter(printSettings.プリンタ名))
                    {
                        baseReport.Document.Printer.PrinterName = printSettings.プリンタ名;
                    }
                }

                if (printSettings.プレビューフラグ == true)
                {
                    ReportPreview(baseReport);
                }
                else
                {
                    ReportPrint(baseReport);
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

        /// <summary>
        /// ActiveReportをPDFに出力する
        /// </summary>
        /// <param name="report">ActiveReportオブジェクト</param>
        /// <param name="filePath">出力ファイルパス</param>
        protected void ReportPdf(BaseReport report, string filePath)
        {
            try
            {
                report.Run();
                PdfExport pdfExport = new PdfExport();
                pdfExport.Options.Title = report.Title;
                pdfExport.Export(report.Document, filePath);
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
        /// ActiveReportを印刷する
        /// </summary>
        /// <param name="report">ActiveReportオブジェクト</param>
        protected void ReportPrint(BaseReport report)
        {
            try
            {
                report.Document.Printer.DocumentName = report.Title;

                // 請求書については異なるページをマージする必要があるのでRunしているが、
                // それ以外はRunしていないので、ここでやる
                if (report.State == SectionReport.ReportState.Idle)
                {
                    report.Run();
                }

                // 引数は以下の通り
                // 1.印刷ダイアログを表示するか
                // 2.印刷状態の情報を示すダイアログを表示するか
                // 3.印刷を個別のスレッドで行うか
                report.Document.Print(false, false, true);
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
        /// ActiveReportをプレビュー表示する
        /// </summary>
        /// <param name="report">ActiveReportオブジェクト</param>
        protected void ReportPreview(BaseReport report)
        {
            try
            {
                report.Document.Printer.DocumentName = report.Title;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = report.Title;
                form.Show();

                // 請求書については異なるページをマージする必要があるのでRunしているので、LoadDocumentだとダメ（マージされた帳票が反映されない）
                // それ以外はLoadDocument()でViewerにロード中のアニメーションを表示させる
                if (report.State == SectionReport.ReportState.Idle)
                {
                    form.ActiveReportViewer.LoadDocument(report);
                }
                else
                {
                    form.ActiveReportViewer.Document = report.Document;
                }

                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);
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
        /// プレビュー画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void formPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormPreview form = (FormPreview)sender;
                form.Dispose();
                form = null;
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
        /// 喫食日と製造日算出日数から製造日を算出します。
        /// </summary>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="busyo">部署マスタ</param>
        /// <returns>喫食日と製造日算出日数から求めた製造日</returns>
        protected DateTime CalcSeizobi(DateTime kissyokubi, Busyo busyo)
        {
            try
            {
                // 製造日算出日数をマイナス値に補正
                int absDays = busyo.製造日算出日数 ?? 0;
                absDays = Math.Abs(absDays) * -1;

                // 喫食日と製造日算出日数から製造日を求める
                DateTime seizobi = kissyokubi.AddDays(absDays);
                DateTime seizobi_org = seizobi;     // 算出した製造日を保持

                // 製造稼働日ではない場合は前日に遡る
                while (true)
                {
                    // ７日遡っても見つからない場合は、これ以上遡っても見つからないと判断し、最初に求めた配送日を採用する
                    // →９連休があるので10日までとするが、念のために11日遡る
                    if (seizobi.AddDays(11).CompareTo(seizobi_org) <= 0)
                    {
                        log.WarnFormat("適切な製造日が見つかりませんでした。部署ＣＤ={0} 喫食日={1:yyyy/MM/dd} 製造日={2:yyyy/MM/dd} 部署.製造稼働日={3}",
                                        busyo.部署ＣＤ, kissyokubi, seizobi_org, busyo.製造稼働日);
                        seizobi = seizobi_org;
                        break;
                    }

                    Consts.EmuDayOfWeek dayOfWeek = (Consts.EmuDayOfWeek)seizobi.DayOfWeek;
                    Shukujitsu shukujitsu = GetShukujitsu(seizobi);  // 祝日を取得
                    if (shukujitsu != null)
                    {
                        dayOfWeek = Consts.EmuDayOfWeek.Holiday;
                    }
                    // 求めた製造日は部署の製造稼働日ではない？
                    if (busyo.GetSeizo(dayOfWeek) == false)
                    {
                        seizobi = seizobi.AddDays(-1);  // 前日に遡る
                        continue;
                    }
                    break;
                }
                return seizobi;
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
        /// 喫食日と配送日算出日数から配送日を算出します。
        /// </summary>
        /// <param name="kissyokubi">喫食日</param>
        /// <param name="busyo">部署マスタ</param>
        /// <param name="tokuisaki">得意先マスタ</param>
        /// <returns>喫食日と配送日算出日数から求めた配送日（ただし得意先の休日ではない）</returns>
        protected DateTime CalcHaisobi(DateTime kissyokubi, Busyo busyo, ITokuisaki tokuisaki)
        {
            try
            {
                // 配送日算出日数をマイナス値に補正
                int absDays = busyo.配送日算出日数 ?? 0;
                absDays = Math.Abs(absDays) * -1;

                // 喫食日と配送日算出日数から配送日を求める
                DateTime haisobi = kissyokubi.AddDays(absDays);
                DateTime haisobi_org = haisobi;     // 算出した配送日を保持

                // 得意先の休日 あるいは 部署の配送稼働日ではない場合は前日に遡る
                while (true)
                {
                    // ７日遡っても見つからない場合は、これ以上遡っても見つからないと判断し、最初に求めた配送日を採用する
                    // →９連休があるので10日までとするが、念のために11日遡る
                    if (haisobi.AddDays(11).CompareTo(haisobi_org) <= 0)
                    {
                        log.WarnFormat("適切な配送日が見つかりませんでした。部署ＣＤ={0} 得意先ＣＤ={1} 喫食日={2:yyyy/MM/dd} 配送日={3:yyyy/MM/dd} 得意先休日設定={4} 部署.得意先休日考慮フラグ={5} 部署.配送稼働日={6}",
                                        busyo.部署ＣＤ, tokuisaki.得意先ＣＤ, kissyokubi, haisobi_org, tokuisaki.休日設定, busyo.得意先休日考慮フラグ, busyo.配送稼働日);
                        haisobi = haisobi_org;
                        break;
                    }

                    //*******************
                    // 得意先の平日を探す
                    //*******************
                    // 配送日算出の際に得意先の休日を考慮する？
                    if (busyo.得意先休日考慮フラグ == true)
                    {
                        // 求めた配送日は得意先の休日？
                        if (tokuisaki.GetHoliday(haisobi.DayOfWeek) == true)
                        {
                            haisobi = haisobi.AddDays(-1);  // 前日に遡る
                            continue;
                        }
                    }

                    //*****************************************
                    // 求めた配送日が部署の配送稼働日であればOK
                    //*****************************************
                    Consts.EmuDayOfWeek dayOfWeek = (Consts.EmuDayOfWeek)haisobi.DayOfWeek;
                    Shukujitsu shukujitsu = GetShukujitsu(haisobi);  // 祝日を取得
                    if (shukujitsu != null)
                    {
                        dayOfWeek = Consts.EmuDayOfWeek.Holiday;
                    }
                    // 求めた配送日は部署の配送稼働日ではない？
                    if (busyo.GetHaiso(dayOfWeek) == false)
                    {
                        haisobi = haisobi.AddDays(-1);  // 前日に遡る
                        continue;
                    }
                    break;
                }

                return haisobi;
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
        /// 製造日と配送日が逆転（配送日が過去）していれば、祝日でもなく得意先の休日でもない日を返します。
        /// </summary>
        /// <param name="seizobi">製造日</param>
        /// <param name="haisobi">配送日</param>
        /// <param name="busyo">部署マスタ</param>
        /// <param name="tokuisaki">得意先マスタ</param>
        /// <returns>配送日を基準として、配送稼働日かつ製造稼働日かつ得意先の休日ではない日</returns>
        protected DateTime? AdjustSeizobiHaisobi(DateTime seizobi, DateTime haisobi, Busyo busyo, ITokuisaki tokuisaki)
        {
            try
            {
                // 製造日≦配送日であればOK
                if (seizobi <= haisobi)
                {
                    return null;
                }

                // 配送日＜製造日の場合は、祝日でもなく得意先の休日でもない日を探す
                DateTime date = haisobi;
                DateTime date_org = date;     // 算出した日を保持

                while (true)
                {
                    // ７日遡っても見つからない場合は、これ以上遡っても見つからないと判断し、最初に求めた日を採用する
                    // →９連休があるので10日までとするが、念のために11日遡る
                    if (date.AddDays(11).CompareTo(date_org) <= 0)
                    {
                        log.WarnFormat("適切な配送日・製造日が見つかりませんでした。部署ＣＤ={0} 得意先ＣＤ={1} 配送日={2:yyyy/MM/dd} 製造日={3:yyyy/MM/dd} 得意先休日設定={4} 部署.得意先休日考慮フラグ={5} 部署.配送稼働日={6}, 部署.製造稼働日={7}",
                                        busyo.部署ＣＤ, tokuisaki.得意先ＣＤ, haisobi, seizobi, tokuisaki.休日設定, busyo.得意先休日考慮フラグ, busyo.配送稼働日, busyo.製造稼働日);
                        date = date_org;
                        break;
                    }

                    Consts.EmuDayOfWeek dayOfWeek = (Consts.EmuDayOfWeek)date.DayOfWeek;
                    Shukujitsu shukujitsu = GetShukujitsu(date);     // 祝日を取得
                    if (shukujitsu != null)
                    {
                        dayOfWeek = Consts.EmuDayOfWeek.Holiday;
                    }

                    // 得意先の休日を取得
                    bool isHoliday = tokuisaki.GetHoliday(date.DayOfWeek);

                    // 配送稼働日かつ製造稼働日かつ得意先の休日ではない？
                    if (busyo.GetHaiso(dayOfWeek) && busyo.GetSeizo(dayOfWeek) && isHoliday == false)
                    {
                        break;
                    }
                    date = date.AddDays(-1);
                }
                return date;
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
        /// 指定した日に該当する祝日を返します。
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>日付に該当する祝日があればその祝日クラスのインスタンス。祝日が無ければnull</returns>
        protected Shukujitsu GetShukujitsu(DateTime date)
        {
            try
            {
                // 日付に該当する値があればそれを返す
                if (_shukujitsuMap.ContainsKey(date))
                {
                    return _shukujitsuMap[date];
                }

                Shukujitsu shukujitsu = Shukujitsu.GetShukujitsu(date);     // 祝日を取得
                _shukujitsuMap[date] = shukujitsu;                          // マップに追加
                return shukujitsu;
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
        /// 指定した電話番号に電話を掛ける（発信処理）
        /// </summary>
        /// <param name="telNo">電話番号</param>
        protected void CallTo(string telNo)
        {
            int ctiCallPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["CtiCallPort"]);
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            Random random = new Random(DateTime.Now.Millisecond);
            UdpClient udpClient = null;
            int port = Consts.EPHEMERAL_PORT_MIN + Consts.PORT_STEP + random.Next(Consts.PORT_STEP);
            string message = string.Format("CALL:{0}", telNo);

            // 適切なポートで送信できるまでループ
            while(true)
            {
                if (Consts.EPHEMERAL_PORT_MAX < port)
                {
                    // 定数だと衝突しそうなので乱数を足す
                    port = Consts.EPHEMERAL_PORT_MIN + Consts.PORT_STEP + random.Next(Consts.PORT_STEP);
                }

                try
                {
                    // Ctiに向けてメッセージを送信
                    IPEndPoint ep = new IPEndPoint(System.Net.IPAddress.Loopback, port);
                    udpClient = new UdpClient(ep);
                    byte[] sendBytes = enc.GetBytes(message);
                    udpClient.Send(sendBytes, sendBytes.Length, "localhost", ctiCallPort);
                    break;
                }
                catch (SocketException exc)
                {
                    if (exc.SocketErrorCode != SocketError.AddressAlreadyInUse)
                    {
                        throw exc;
                    }

                    // エフェメラルポートが衝突したら適当な数を足して開き直す
                    port += Consts.PORT_STEP + random.Next(Consts.PORT_STEP);
                }
                catch (Exception ex)
                {
                    throw new EmuException("発信時にエラーが発生しました。\n" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// 進捗状況更新
        /// </summary>
        /// <param name="progressBar">プログレスバー</param>
        /// <param name="label">ラベル</param>
        /// <param name="progressState">プログレスバーを操作するパラメータ</param>
        protected void ShowProgress(ProgressBar progressBar, Label label, ProgressState progressState)
        {
            // 短すぎる間隔での進捗状況更新を抑制する
            DateTime now = DateTime.Now;
            DateTime elapsed = _lastProgressTime.AddMilliseconds(ON_PROGRESS_INTERVAL_MS);
            if (now < elapsed)
            {
                return;
            }
            _lastProgressTime = now;

            if (progressBar != null)
            {
                // プログレスバーのスタイルを変更する
                if (progressBar.Style != progressState.Style)
                {
                    progressBar.Style = progressState.Style;
                }

                // プログレスバーの上限を変更する
                if (progressBar.Maximum != progressState.Maximum)
                {
                    progressBar.Maximum = progressState.Maximum;
                }

                // プログレスバーの値を変更する
                if (progressState.Value < progressBar.Minimum)
                {
                    progressBar.Value = progressBar.Minimum;
                }
                else if (progressBar.Maximum < progressState.Value)
                {
                    progressBar.Value = progressBar.Maximum;
                }
                else
                {
                    // バーが徐々に伸びるアニメーションを無効にする
                    if (progressBar.Value < progressState.Value)
                    {
                        // 値を増やすとき
                        if (progressState.Value < progressBar.Maximum)
                        {
                            // 目的の値より一つ大きくしてから、目的の値にする
                            progressBar.Value = progressState.Value + 1;
                            progressBar.Value = progressState.Value;
                        }
                        else
                        {
                            // 最大値にするとき
                            // 最大値を1つ増やしてから、元に戻す
                            progressBar.Maximum++;
                            progressBar.Value = progressState.Value + 1;
                            progressBar.Value = progressState.Value;
                            progressBar.Maximum--;
                        }
                    }
                    else
                    {
                        // 値を減らす時は、そのまま
                        progressBar.Value = progressState.Value;
                    }
                }
            }

            // メッセージのテキストを変更する
            if (label != null)
            {
                label.Text = progressState.Message;
            }
        }
        #endregion
    }
}

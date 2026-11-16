using System;
using System.Collections.Generic;
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
    /// 集計タイプ別売上分析表印刷
    /// </summary>
    public partial class Form817 : BaseForm
    {
        //**************************************
        //* 定数
        //**************************************
        #region 定数
        private const int BUSYO_DATE_COLUMN                    = 0;     // 部署売上日列インデックス
        private const int BUSYO_BUSYO_CD_COLUMN                = 1;     // 部署CD列インデックス
        private const int BUSYO_BUSYO_NAME_COLUMN              = 2;     // 部署名列インデックス
        private const int BUSYO_SYUKEI_JISSEKI_COLUMN          = 3;     // 部署集計実績列インデックス
        private const int BUSYO_SYUKEI_KOSU_COLUMN             = 4;     // 部署集計個数列インデックス
        private const int BUSYO_BUNKATSU_SYUKEI_JISSEKI_COLUMN = 5;     // 部署分割集計実績列インデックス
        private const int BUSYO_BUNKATSU_SYUKEI_KOSU_COLUMN    = 6;     // 部署分割集計個数列インデックス
        private const int BUSYO_LOSS_COLUMN                    = 7;     // 部署ロス列インデックス
        private const int BUSYO_GEN_TOTAL_COLUMN               = 8;     // 部署現金合計数列インデックス
        private const int BUSYO_TICKET_KENGAKU_COLUMN          = 9;     // 部署チケット金額列インデックス
        private const int BUSYO_TICKET_MAISU_COLUMN            = 10;    // 部署チケット枚数現金売上列インデックス
        private const int BUSYO_KAKE_TOTAL_COLUMN              = 11;    // 部署掛売合計列インデックス
        private const int BUSYO_TAPION_TOTAL_COLUMN            = 12;    // 部署電子決済列インデックス
        private const int BUSYO_GROSS_COLUMN                   = 13;    // 部署総売上列インデックス
        private const int BUSYO_SONOTA_COLUMN                  = 14;    // 部署その他売上列インデックス
        private const int BUSYO_MOCHIDASHI_COLUMN              = 15;    // 部署持出数列インデックス

        private const int COURSE_DATE_COLUMN                    = 0;    // コース売上日列インデックス
        private const int COURSE_BUSYO_CD_COLUMN                = 1;    // コース部署CD列インデックス
        private const int COURSE_BUSYO_NAME_COLUMN              = 2;    // コース部署名列インデックス
        private const int COURSE_CD_COLUMN                      = 3;    // コースCD列インデックス
        private const int COURSE_NAME_COLUMN                    = 4;    // コース名列インデックス
        private const int COURSE_HAISO_NAME_COLUMN              = 5;    // コース配送者列インデックス
        private const int COURSE_SYUKEI_JISSEKI_COLUMN          = 6;    // コース集計実績列インデックス
        private const int COURSE_SYUKEI_KOSU_COLUMN             = 7;    // コース集計個数列インデックス
        private const int COURSE_BUNKATSU_SYUKEI_JISSEKI_COLUMN = 8;    // コース分割集計実績列インデックス
        private const int COURSE_BUNKATSU_SYUKEI_KOSU_COLUMN    = 9;    // コース分割集計個数列インデックス
        private const int COURSE_LOSS_COLUMN                    = 10;   // コースロス列インデックス
        private const int COURSE_GEN_TOTAL_COLUMN               = 11;   // コース現金合計数列インデックス
        private const int COURSE_TICKET_KENGAKU_COLUMN          = 12;   // コースチケット金額列インデックス
        private const int COURSE_TICKET_MAISU_COLUMN            = 13;   // コースチケット枚数現金売上列インデックス
        private const int COURSE_KAKE_TOTAL_COLUMN              = 14;   // コース掛売合計列インデックス
        private const int COURSE_TAPION_TOTAL_COLUMN            = 15;   // コース電子決済列インデックス
        private const int COURSE_GROSS_COLUMN                   = 16;   // コース総売上列インデックス
        private const int COURSE_SONOTA_COLUMN                  = 17;   // コースその他売上列インデックス
        private const int COURSE_MOCHIDASHI_COLUMN              = 18;   // コース持出数列インデックス
        private const int COURSE_IDO_COLUMN                     = 19;   // コース移動列インデックス

        private const int TANTOUSYA_BUSYO_CD_COLUMN                = 0;    // 担当者部署CD列インデックス
        private const int TANTOUSYA_BUSYO_NAME_COLUMN              = 1;    // 担当者部署名列インデックス
        private const int TANTOUSYA_CD_COLUMN                      = 2;    // 担当者CD列インデックス
        private const int TANTOUSYA_NAME_COLUMN                    = 3;    // 担当者名列インデックス
        private const int TANTOUSYA_SYUKEI_JISSEKI_COLUMN          = 4;    // 担当者集計実績列インデックス
        private const int TANTOUSYA_SYUKEI_KOSU_COLUMN             = 5;    // 担当者集計個数列インデックス
        private const int TANTOUSYA_BUNKATSU_SYUKEI_JISSEKI_COLUMN = 6;    // 担当者分割集計実績列インデックス
        private const int TANTOUSYA_BUNKATSU_SYUKEI_KOSU_COLUMN    = 7;    // 担当者分割集計個数列インデックス
        private const int TANTOUSYA_LOSS_COLUMN                    = 8;    // 担当者ロス列インデックス
        private const int TANTOUSYA_GEN_TOTAL_COLUMN               = 9;    // 担当者現金合計数列インデックス
        private const int TANTOUSYA_TICKET_KENGAKU_COLUMN          = 10;   // 担当者チケット金額列インデックス
        private const int TANTOUSYA_TICKET_MAISU_COLUMN            = 11;   // 担当者チケット枚数現金売上列インデックス
        private const int TANTOUSYA_KAKE_TOTAL_COLUMN              = 12;   // 担当者掛売合計列インデックス
        private const int TANTOUSYA_TAPION_TOTAL_COLUMN            = 13;   // 担当者電子決済列インデックス
        private const int TANTOUSYA_GROSS_COLUMN                   = 14;   // 担当者総売上列インデックス
        private const int TANTOUSYA_SONOTA_COLUMN                  = 15;   // 担当者その他売上列インデックス
        private const int TANTOUSYA_MOCHIDASHI_COLUMN              = 16;   // 担当者持出数列インデックス

        #endregion

        //**************************************
        //* インスタンス変数
        //**************************************
        #region インスタンス変数
        /// <summary>
        /// 集計タイプ別売上分析表印刷条件
        /// </summary>
        private UriageBunsekiHyoCriteria _uriageBunsekiHyoCriteria;

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public Form817()
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
        private void Form817_Load(object sender, EventArgs e)
        {
            try
            {
                // アイコンを設定
                this.Icon = Properties.Resources.chart_curve_ico;

                ComboUtil.SetBusyo(cboBusyo);           // デフォルトの支店CDは指定しない
                ComboUtil.Set(cboJissekiSyukeiPrintType,
                            new string[] { Consts.JissekiSyukeiPrintType.Busyo.DisplayName(),
                                           Consts.JissekiSyukeiPrintType.Course.DisplayName(),
                                           Consts.JissekiSyukeiPrintType.Tantousya.DisplayName()},
                            new int[] { (int)Consts.JissekiSyukeiPrintType.Busyo,
                                        (int)Consts.JissekiSyukeiPrintType.Course,
                                        (int)Consts.JissekiSyukeiPrintType.Tantousya});

                // 各テキストボックスには予めデータソースのフィールドをバインド済み
                // ErrorProviderにもデータソースとして BindingSource をバインド済み
                // BindingSource に データソースをバインドすると、IDataErrorInfo を元に自動的にエラー表示してくれる
                _uriageBunsekiHyoCriteria = new UriageBunsekiHyoCriteria();
                uriageBunsekiHyoCriteriaBindingSource.DataSource = _uriageBunsekiHyoCriteria;
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
        private void Form817_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
            {
                WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する

                switch (e.KeyCode)
                {
                    case Keys.F10:                      // 印刷実行ボタン押下
                        WriteBinding(this.ActiveControl);       // ファンクションキーが押下されたとき、入力中の内容を強制的に反映する
                        btnPrint.PerformClick();
                        break;

                    case Keys.F12:
                        btnQuit.PerformClick();         // 終了ボタン押下
                        break;
                }
            }
        }

        /// <summary>
        /// 商品ＣＤキー押下（ファンクションキー）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSyohinCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                btnSyohinCD.PerformClick();
            }
        }

        /// <summary>
        /// 商品参照ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyohinCD_Click(object sender, EventArgs e)
        {
            try
            {
                Control activeControl = this.ActiveControl;
                // 商品選択ダイアログを表示
                Form901 formSyohinDlg = new Form901();
                SubFormOpenInit(formSyohinDlg);
                formSyohinDlg.FormClosed += new FormClosedEventHandler(Form901SyohinCD_FormClosed);
                formSyohinDlg.ShowDialog();
                if (formSyohinDlg.DialogResult == System.Windows.Forms.DialogResult.OK)
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
        /// 商品選択ダイアログクローズ（商品CD）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form901SyohinCD_FormClosed(object sender, FormClosedEventArgs e)
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
                Form901 form = (Form901)sender;
                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    errorProvider1.SetError(txtSyohinCD, null);  // エラーを消す

                    Syohin syohin = form.SelectedSyohin;
                    _uriageBunsekiHyoCriteria.分割集計商品ＣＤ = syohin.商品ＣＤ;       // 入力内容を確定させる
                    ReadBinding(txtSyohinCD);
                    ReadBinding(lblSyohinName);
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
        /// 印刷ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "実行できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_uriageBunsekiHyoCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageBunsekiHyoCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _uriageBunsekiHyoCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 売上日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_uriageBunsekiHyoCriteria.売上日From > warnDate || _uriageBunsekiHyoCriteria.売上日To > warnDate)
                {
                    if (MessageBox.Show(this, "売上日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // 帳票に該当するプリンタを取得。未選択ならダイアログを開く。そのあと印刷処理を実行
                DecidePrinter(Consts.Reports.UriageBunsekiHyo, PrintMain);
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
            try
            {
                if (this.HasAuthEdit == false)
                {
                    MessageBox.Show(this, "実行できる権限がありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 検索条件の入力にエラーがある？
                if (_uriageBunsekiHyoCriteria.HasErrors)
                {
                    if (string.IsNullOrEmpty(_uriageBunsekiHyoCriteria.Error) == false)
                    {
                        MessageBox.Show(this, _uriageBunsekiHyoCriteria.Error ?? "入力にエラーがあります。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }

                // 売上日が（システム日付＋未来日警告閾値）を超えていれば警告メッセージを表示
                DateTime warnDate = DateTime.Today.AddDays(SystemMaster.GetWarningFutureDays());
                if (_uriageBunsekiHyoCriteria.売上日From > warnDate || _uriageBunsekiHyoCriteria.売上日To > warnDate)
                {
                    if (MessageBox.Show(this, "売上日として適切ではない日付が入力されているおそれがあります。続行しますか？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }

                ExportMain();       // Excel出力処理
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
        /// <summary>
        /// Excel出力処理
        /// </summary>
        private void ExportMain()
        {
            string fileName = "";
            try
            {
                int cnt = 0;
                string title = "";
                string logmsg = "";
                FarPoint.Win.Spread.FpSpread fpSpread = null;

                this.MdiParent.Cursor = Cursors.WaitCursor;

                switch (_uriageBunsekiHyoCriteria.集計タイプ)
                {
                    case Consts.JissekiSyukeiPrintType.Busyo:       // 部署別実績日計
                        cnt = ExportBusyo();
                        title = "部署別実績日計";
                        logmsg = string.Format("部署別実績日計(Excel)を出力しました。件数={0}", cnt);
                        fpSpread = fpSpread_Busyo;
                        break;

                    case Consts.JissekiSyukeiPrintType.Course:      // コース別実績日計
                        cnt = ExportCourse();
                        title = "コース別実績日計";
                        logmsg = string.Format("コース別実績日計(Excel)を出力しました。件数={0}", cnt);
                        fpSpread = fpSpread_Course;
                        break;

                    case Consts.JissekiSyukeiPrintType.Tantousya:   // 担当者別実績集計
                        cnt = ExportTantousya();
                        title = "担当者別実績集計";
                        logmsg = string.Format("担当者別実績集計(Excel)を出力しました。件数={0}", cnt);
                        fpSpread = fpSpread_Tantousya;
                        break;
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
                    fpSpread.SaveExcel(fileName, excelSaveFlags);
                    if (MessageBox.Show(this, "Excel出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }

                    title += "(Excel)";
                    History.Save(this.Text, title, _uriageBunsekiHyoCriteria.Code, History.ProcessType.Output);

                    log.Info(logmsg);
                }
            }
            catch (BentoException)
            {
                throw;
            }
            catch (ApplicationException)
            {
                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
        /// 部署別実績日計Excel出力処理
        /// </summary>
        /// <returns>取得したレコードの件数</returns>
        private int ExportBusyo()
        {
            try
            {
                int cnt = 0;

                // 部署別実績日計のデータテーブルを取得
                DataTable dt = GetBusyoDataTable();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                // 分割集計商品名をスプレッドのヘッダにセット
                string bunkatsuSyohinName = "";
                if (dt != null && dt.Rows.Count != 0)
                {
                    bunkatsuSyohinName = dt.Rows[0]["分割集計商品名"].ToString();
                }
                fpSpread_Busyo_Sheet1.ColumnHeader.Cells.Get(0, BUSYO_BUNKATSU_SYUKEI_JISSEKI_COLUMN).Value = bunkatsuSyohinName;

                busyoBindingSource.DataSource = ds;
                busyoBindingSource.DataMember = "部署別実績日計";

                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_DATE_COLUMN,                    "売上日");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_BUSYO_CD_COLUMN,                "部署ＣＤ");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_BUSYO_NAME_COLUMN,              "部署名");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_SYUKEI_JISSEKI_COLUMN,          "集計実績");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_SYUKEI_KOSU_COLUMN,             "集計個数");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_BUNKATSU_SYUKEI_JISSEKI_COLUMN, "分割集計実績");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_BUNKATSU_SYUKEI_KOSU_COLUMN,    "分割集計個数");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_LOSS_COLUMN,                    "ロス");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_GEN_TOTAL_COLUMN,               "現金合計");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_TICKET_KENGAKU_COLUMN,          "チケット金額");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_TICKET_MAISU_COLUMN,            "チケット枚数");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_KAKE_TOTAL_COLUMN,              "売掛合計");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_TAPION_TOTAL_COLUMN,            "電子決済");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_GROSS_COLUMN,                   "総売上");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_SONOTA_COLUMN,                  "その他売上");
                fpSpread_Busyo_Sheet1.BindDataColumn(BUSYO_MOCHIDASHI_COLUMN,              "持出数");

                if (dt != null)
                {
                    cnt = dt.Rows.Count;
                }
                return cnt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// コース別実績日計Excel出力処理
        /// </summary>
        /// <returns>取得したレコードの件数</returns>
        private int ExportCourse()
        {
            try
            {
                int cnt = 0;

                // コース別実績日計のデータテーブルを取得
                DataTable dt = GetCourseDataTable();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                // 分割集計商品名をスプレッドのヘッダにセット
                string bunkatsuSyohinName = "";
                if (dt != null && dt.Rows.Count != 0)
                {
                    bunkatsuSyohinName = dt.Rows[0]["分割集計商品名"].ToString();
                }
                fpSpread_Course_Sheet1.ColumnHeader.Cells.Get(0, COURSE_BUNKATSU_SYUKEI_JISSEKI_COLUMN).Value = bunkatsuSyohinName;

                courseBindingSource.DataSource = ds;
                courseBindingSource.DataMember = "コース別実績日計";

                fpSpread_Course_Sheet1.BindDataColumn(COURSE_DATE_COLUMN,                    "売上日");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_BUSYO_CD_COLUMN,                "部署ＣＤ");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_BUSYO_NAME_COLUMN,              "部署名");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_CD_COLUMN,                      "コースＣＤ");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_NAME_COLUMN,                    "コース名");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_HAISO_NAME_COLUMN,              "担当者名");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_SYUKEI_JISSEKI_COLUMN,          "集計実績");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_SYUKEI_KOSU_COLUMN,             "集計個数");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_BUNKATSU_SYUKEI_JISSEKI_COLUMN, "分割集計実績");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_BUNKATSU_SYUKEI_KOSU_COLUMN,    "分割集計個数");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_LOSS_COLUMN,                    "ロス");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_GEN_TOTAL_COLUMN,               "現金合計");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_TICKET_KENGAKU_COLUMN,          "チケット金額");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_TICKET_MAISU_COLUMN,            "チケット枚数");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_KAKE_TOTAL_COLUMN,              "売掛合計");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_TAPION_TOTAL_COLUMN,            "電子決済");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_GROSS_COLUMN,                   "総売上");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_SONOTA_COLUMN,                  "その他売上");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_MOCHIDASHI_COLUMN,              "持出数");
                fpSpread_Course_Sheet1.BindDataColumn(COURSE_IDO_COLUMN,                     "移動数");

                if (dt != null)
                {
                    cnt = dt.Rows.Count;
                }
                return cnt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 担当者別実績集計Excel出力処理
        /// </summary>
        /// <returns>取得したレコードの件数</returns>
        private int ExportTantousya()
        {
            try
            {
                int cnt = 0;

                // 担当者別実績集計のデータテーブルを取得
                DataTable dt = GetTantousyaDataTable();

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                // 分割集計商品名をスプレッドのヘッダにセット
                string bunkatsuSyohinName = "";
                if (dt != null && dt.Rows.Count != 0)
                {
                    bunkatsuSyohinName = dt.Rows[0]["分割集計商品名"].ToString();
                }
                fpSpread_Tantousya_Sheet1.ColumnHeader.Cells.Get(0, TANTOUSYA_BUNKATSU_SYUKEI_JISSEKI_COLUMN).Value = bunkatsuSyohinName;

                courseBindingSource.DataSource = ds;
                courseBindingSource.DataMember = "担当者別実績集計";

                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_BUSYO_CD_COLUMN,                "部署ＣＤ");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_BUSYO_NAME_COLUMN,              "部署名");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_CD_COLUMN,                      "配送担当者ＣＤ");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_NAME_COLUMN,                    "配送担当者名");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_SYUKEI_JISSEKI_COLUMN,          "集計実績");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_SYUKEI_KOSU_COLUMN,             "集計個数");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_BUNKATSU_SYUKEI_JISSEKI_COLUMN, "分割集計実績");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_BUNKATSU_SYUKEI_KOSU_COLUMN,    "分割集計個数");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_LOSS_COLUMN,                    "ロス");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_GEN_TOTAL_COLUMN,               "現金合計");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_TICKET_KENGAKU_COLUMN,          "チケット金額");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_TICKET_MAISU_COLUMN,            "チケット枚数");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_KAKE_TOTAL_COLUMN,              "売掛合計");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_TAPION_TOTAL_COLUMN,            "電子決済");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_GROSS_COLUMN,                   "総売上");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_SONOTA_COLUMN,                  "その他売上");
                fpSpread_Tantousya_Sheet1.BindDataColumn(TANTOUSYA_MOCHIDASHI_COLUMN,              "持出数");

                if (dt != null)
                {
                    cnt = dt.Rows.Count;
                }
                return cnt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintMain(string printerNameA4)
        {
            try
            {
                this.MdiParent.Cursor = Cursors.WaitCursor;

                switch (_uriageBunsekiHyoCriteria.集計タイプ)
                {
                    case Consts.JissekiSyukeiPrintType.Busyo:       // 部署別実績日計
                        PrintBusyo(printerNameA4);
                        break;

                    case Consts.JissekiSyukeiPrintType.Course:      // コース別実績日計
                        PrintCourse(printerNameA4);
                        break;

                    case Consts.JissekiSyukeiPrintType.Tantousya:   // 担当者別実績集計
                        PrintTantousya(printerNameA4);
                        break;
                }
            }
            catch (BentoException)
            {
                throw;
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
        /// 部署別実績日計印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintBusyo(string printerNameA4)
        {
            try
            {
                // 部署別実績日計のデータテーブルを取得
                DataTable dt = GetBusyoDataTable();

                // レコードが無い？
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "出力条件に合う売上データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 総合計を算出
                DataRow grossRow = dt.NewRow();   // 総合計格納用のレコード
                grossRow["売上日From"] = _uriageBunsekiHyoCriteria.売上日From;
                grossRow["売上日To"] = _uriageBunsekiHyoCriteria.売上日To;
                grossRow["合計見出し"] = "＊＊＊ 総合計 ＊＊＊";
                grossRow["部署ソート順"] = 1;
                grossRow["売上日ソート順"] = 1;
                grossRow["集計実績"] = dt.Compute("SUM(集計実績)", "");
                grossRow["集計個数"] = dt.Compute("SUM(集計個数)", "");
                grossRow["集計個数"] = dt.Compute("SUM(集計個数)", "");
                grossRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", "");
                grossRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", "");
                grossRow["ロス"] = dt.Compute("SUM(ロス)", "");
                grossRow["現金合計"] = dt.Compute("SUM(現金合計)", "");
                grossRow["チケット金額"] = dt.Compute("SUM(チケット金額)", "");
                grossRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", "");
                grossRow["売掛合計"] = dt.Compute("SUM(売掛合計)", "");
                grossRow["電子決済"] = dt.Compute("SUM(電子決済)", "");
                grossRow["総売上"] = dt.Compute("SUM(総売上)", "");
                grossRow["その他売上"] = dt.Compute("SUM(その他売上)", "");
                grossRow["持出数"] = dt.Compute("SUM(持出数)", "");

                int busyoCD = 0;
                string busyoName = "";
                string bunkatsuSyohinName = "";
                // 部署計を算出・追加
                // ユニークな部署ＣＤだけを抽出したデータテーブルを生成
                DataView dvBusyo = new DataView(dt);
                DataTable dtBusyo = dvBusyo.ToTable(true, new string[] { "部署ＣＤ", "部署名", "分割集計商品名" });
                foreach (DataRow dr in dtBusyo.Rows)
                {
                    DataRow newRow = dt.NewRow();   // 部署計格納用のレコード
                    newRow["売上日From"] = _uriageBunsekiHyoCriteria.売上日From;
                    newRow["売上日To"] = _uriageBunsekiHyoCriteria.売上日To;
                    busyoCD = (int)dr["部署ＣＤ"];
                    newRow["部署ＣＤ"] = busyoCD;
                    busyoName = dr["部署名"].ToString();
                    newRow["部署名"] = busyoName;
                    newRow["合計見出し"] = "部署計";
                    newRow["部署ソート順"] = 0;
                    newRow["売上日ソート順"] = 1;
                    string filter = string.Format("部署ＣＤ={0}", busyoCD);
                    newRow["集計実績"] = dt.Compute("SUM(集計実績)", filter);
                    newRow["集計個数"] = dt.Compute("SUM(集計個数)", filter);
                    bunkatsuSyohinName = dr["分割集計商品名"].ToString();
                    newRow["分割集計商品名"] = bunkatsuSyohinName;
                    newRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", filter);
                    newRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", filter);
                    newRow["ロス"] = dt.Compute("SUM(ロス)", filter);
                    newRow["現金合計"] = dt.Compute("SUM(現金合計)", filter);
                    newRow["チケット金額"] = dt.Compute("SUM(チケット金額)", filter);
                    newRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", filter);
                    newRow["売掛合計"] = dt.Compute("SUM(売掛合計)", filter);
                    newRow["電子決済"] = dt.Compute("SUM(電子決済)", filter);
                    newRow["総売上"] = dt.Compute("SUM(総売上)", filter);
                    newRow["その他売上"] = dt.Compute("SUM(その他売上)", filter);
                    newRow["持出数"] = dt.Compute("SUM(持出数)", filter);
                    dt.Rows.Add(newRow);
                }

                // 総合計行を追加
                grossRow["部署ＣＤ"] = busyoCD;     // 総合計行は最後の部署のページに含める
                grossRow["部署名"] = busyoName;
                grossRow["分割集計商品名"] = bunkatsuSyohinName;
                dt.Rows.Add(grossRow);

                // 部署計と総合計を含めてソートし直す
                DataRow[] drs = dt.Select(null, "部署ソート順, 部署ＣＤ, 売上日ソート順, 売上日");
                dt = dt.Clone();
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }

                Report817BusyoA report817BusyoA = new Report817BusyoA();
                report817BusyoA.DataSource = dt;
                report817BusyoA.Document.Printer.DocumentName = "部署別実績日計";
                report817BusyoA.Document.Printer.PrinterName = printerNameA4;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "部署別実績日計";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report817BusyoA);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                log.InfoFormat("部署別実績日計を印刷しました。件数={0}", dt.Rows.Count);
                History.Save(this.Text, "部署別実績日計", _uriageBunsekiHyoCriteria.Code, History.ProcessType.Output);
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// コース別実績日計印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintCourse(string printerNameA4)
        {
            try
            {
                // コース別実績日計のデータテーブルを取得
                DataTable dt = GetCourseDataTable();

                // レコードが無い？
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "出力条件に合う売上データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 総合計を算出
                DataRow grossRow = dt.NewRow();   // 総合計格納用のレコード
                grossRow["コース名"] = "＊＊＊ 総合計 ＊＊＊";
                grossRow["部署ソート順"] = 1;
                grossRow["売上日ソート順"] = 1;
                grossRow["コースＣＤソート順"] = 1;
                grossRow["集計実績"] = dt.Compute("SUM(集計実績)", "");
                grossRow["集計個数"] = dt.Compute("SUM(集計個数)", "");
                grossRow["集計個数"] = dt.Compute("SUM(集計個数)", "");
                grossRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", "");
                grossRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", "");
                grossRow["ロス"] = dt.Compute("SUM(ロス)", "");
                grossRow["現金合計"] = dt.Compute("SUM(現金合計)", "");
                grossRow["チケット金額"] = dt.Compute("SUM(チケット金額)", "");
                grossRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", "");
                grossRow["売掛合計"] = dt.Compute("SUM(売掛合計)", "");
                grossRow["電子決済"] = dt.Compute("SUM(電子決済)", "");
                grossRow["総売上"] = dt.Compute("SUM(総売上)", "");
                grossRow["その他売上"] = dt.Compute("SUM(その他売上)", "");
                grossRow["持出数"] = dt.Compute("SUM(持出数)", "");
                grossRow["移動数"] = dt.Compute("SUM(移動数)", "");

                int busyoCD = 0;
                DateTime uriagebi = DateTime.Today;
                // 日計を算出
                // ユニークな部署と売上日を抽出したデータテーブルを生成
                DataView dvDaily = new DataView(dt);
                DataTable dtDaily = dvDaily.ToTable(true, new string[] { "部署ＣＤ", "部署名", "売上日", "分割集計商品名" });
                List<DataRow> dailyRowList = new List<DataRow>();   // 日計行格納用リスト
                Dictionary<int, DateTime> busyoUriagebiMap = new Dictionary<int, DateTime>();   // 部署ごとの最終売上日のマップ
                foreach (DataRow dr in dtDaily.Rows)
                {
                    DataRow newRow = dt.NewRow();   // 日計格納用のレコード
                    busyoCD = (int)dr["部署ＣＤ"];
                    newRow["部署ＣＤ"] = busyoCD;
                    newRow["部署名"] = dr["部署名"];
                    uriagebi = (DateTime)dr["売上日"];
                    busyoUriagebiMap[busyoCD] = uriagebi;   // 部署ごとの最終売上日を保持
                    newRow["売上日"] = uriagebi;
                    newRow["コース名"] = "日計";
                    newRow["部署ソート順"] = 0;
                    newRow["売上日ソート順"] = 0;
                    newRow["コースＣＤソート順"] = 1;
                    string filter = string.Format("部署ＣＤ={0} AND 売上日=#{1:yyyy/MM/dd}#", busyoCD, uriagebi);
                    newRow["集計実績"] = dt.Compute("SUM(集計実績)", filter);
                    newRow["集計個数"] = dt.Compute("SUM(集計個数)", filter);
                    newRow["分割集計商品名"] = dr["分割集計商品名"];
                    newRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", filter);
                    newRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", filter);
                    newRow["ロス"] = dt.Compute("SUM(ロス)", filter);
                    newRow["現金合計"] = dt.Compute("SUM(現金合計)", filter);
                    newRow["チケット金額"] = dt.Compute("SUM(チケット金額)", filter);
                    newRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", filter);
                    newRow["売掛合計"] = dt.Compute("SUM(売掛合計)", filter);
                    newRow["電子決済"] = dt.Compute("SUM(電子決済)", filter);
                    newRow["総売上"] = dt.Compute("SUM(総売上)", filter);
                    newRow["その他売上"] = dt.Compute("SUM(その他売上)", filter);
                    newRow["持出数"] = dt.Compute("SUM(持出数)", filter);
                    newRow["移動数"] = dt.Compute("SUM(移動数)", filter);
                    dailyRowList.Add(newRow);
                }

                string busyoName = "";
                string bunkatsuSyohinName = "";
                // 部署計を算出
                // ユニークな部署ＣＤだけを抽出したデータテーブルを生成
                DataView dvBusyo = new DataView(dt);
                DataTable dtBusyo = dvBusyo.ToTable(true, new string[] { "部署ＣＤ", "部署名", "分割集計商品名" });
                foreach (DataRow dr in dtBusyo.Rows)
                {
                    DataRow newRow = dt.NewRow();   // 部署計格納用のレコード
                    busyoCD = (int)dr["部署ＣＤ"];
                    newRow["部署ＣＤ"] = busyoCD;
                    busyoName = dr["部署名"].ToString();
                    newRow["部署名"] = busyoName;
                    newRow["売上日"] = busyoUriagebiMap[busyoCD];  // 部署計は最後の売上日に含める
                    newRow["コース名"] = "部署計";
                    newRow["部署ソート順"] = 0;
                    newRow["売上日ソート順"] = 1;
                    newRow["コースＣＤソート順"] = 1;
                    string filter = string.Format("部署ＣＤ={0}", busyoCD);
                    newRow["集計実績"] = dt.Compute("SUM(集計実績)", filter);
                    newRow["集計個数"] = dt.Compute("SUM(集計個数)", filter);
                    bunkatsuSyohinName = dr["分割集計商品名"].ToString();
                    newRow["分割集計商品名"] = bunkatsuSyohinName;
                    newRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", filter);
                    newRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", filter);
                    newRow["ロス"] = dt.Compute("SUM(ロス)", filter);
                    newRow["現金合計"] = dt.Compute("SUM(現金合計)", filter);
                    newRow["チケット金額"] = dt.Compute("SUM(チケット金額)", filter);
                    newRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", filter);
                    newRow["売掛合計"] = dt.Compute("SUM(売掛合計)", filter);
                    newRow["電子決済"] = dt.Compute("SUM(電子決済)", filter);
                    newRow["総売上"] = dt.Compute("SUM(総売上)", filter);
                    newRow["その他売上"] = dt.Compute("SUM(その他売上)", filter);
                    newRow["持出数"] = dt.Compute("SUM(持出数)", filter);
                    newRow["移動数"] = dt.Compute("SUM(移動数)", filter);
                    dt.Rows.Add(newRow);
                }

                // 日計を追加
                foreach (DataRow dr in dailyRowList)
                {
                    dt.Rows.Add(dr);
                }

                // 総合計行を追加
                grossRow["部署ＣＤ"] = busyoCD;     // 総合計行は最後の部署のページに含める
                grossRow["部署名"] = busyoName;
                grossRow["売上日"] = uriagebi;
                grossRow["分割集計商品名"] = bunkatsuSyohinName;
                dt.Rows.Add(grossRow);

                // 日計と部署計と総合計を含めてソートし直す
                DataRow[] drs = dt.Select(null, "部署ソート順, 部署ＣＤ, 売上日ソート順, 売上日, コースＣＤソート順, コースＣＤ");
                dt = dt.Clone();
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }

                Report817CourseA report817CourseA = new Report817CourseA();
                report817CourseA.DataSource = dt;
                report817CourseA.Document.Printer.DocumentName = "コース別実績日計";
                report817CourseA.Document.Printer.PrinterName = printerNameA4;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "コース別実績日計";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report817CourseA);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                log.InfoFormat("コース別実績日計を印刷しました。件数={0}", dt.Rows.Count);
                History.Save(this.Text, "コース別実績日計", _uriageBunsekiHyoCriteria.Code, History.ProcessType.Output);
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 担当者別実績集計印刷処理
        /// </summary>
        /// <param name="printerNameA4">A4プリンタ名</param>
        private void PrintTantousya(string printerNameA4)
        {
            try
            {
                // 担当者別実績集計のデータテーブルを取得
                DataTable dt = GetTantousyaDataTable();

                // レコードが無い？
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show(this, "出力条件に合う売上データがありません。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                // 総合計を算出
                DataRow grossRow = dt.NewRow();   // 総合計格納用のレコード
                grossRow["売上日From"] = _uriageBunsekiHyoCriteria.売上日From;
                grossRow["売上日To"] = _uriageBunsekiHyoCriteria.売上日To;
                grossRow["配送担当者名"] = "＊＊＊ 総合計 ＊＊＊";
                grossRow["部署ソート順"] = 1;
                grossRow["配送担当者ＣＤソート順"] = 1;
                grossRow["集計実績"] = dt.Compute("SUM(集計実績)", "");
                grossRow["集計個数"] = dt.Compute("SUM(集計個数)", "");
                grossRow["集計個数"] = dt.Compute("SUM(集計個数)", "");
                grossRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", "");
                grossRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", "");
                grossRow["ロス"] = dt.Compute("SUM(ロス)", "");
                grossRow["現金合計"] = dt.Compute("SUM(現金合計)", "");
                grossRow["チケット金額"] = dt.Compute("SUM(チケット金額)", "");
                grossRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", "");
                grossRow["売掛合計"] = dt.Compute("SUM(売掛合計)", "");
                grossRow["電子決済"] = dt.Compute("SUM(電子決済)", "");
                grossRow["総売上"] = dt.Compute("SUM(総売上)", "");
                grossRow["その他売上"] = dt.Compute("SUM(その他売上)", "");
                grossRow["持出数"] = dt.Compute("SUM(持出数)", "");

                int busyoCD = 0;
                string busyoName = "";
                string bunkatsuSyohinName = "";
                // 部署計を算出・追加
                // ユニークな部署ＣＤだけを抽出したデータテーブルを生成
                DataView dvBusyo = new DataView(dt);
                DataTable dtBusyo = dvBusyo.ToTable(true, new string[] { "部署ＣＤ", "部署名", "分割集計商品名" });
                foreach (DataRow dr in dtBusyo.Rows)
                {
                    DataRow newRow = dt.NewRow();   // 部署計格納用のレコード
                    newRow["売上日From"] = _uriageBunsekiHyoCriteria.売上日From;
                    newRow["売上日To"] = _uriageBunsekiHyoCriteria.売上日To;
                    busyoCD = (int)dr["部署ＣＤ"];
                    newRow["部署ＣＤ"] = busyoCD;
                    busyoName = dr["部署名"].ToString();
                    newRow["部署名"] = busyoName;
                    newRow["配送担当者名"] = "部署計";
                    newRow["部署ソート順"] = 0;
                    newRow["配送担当者ＣＤソート順"] = 1;
                    string filter = string.Format("部署ＣＤ={0}", busyoCD);
                    newRow["集計実績"] = dt.Compute("SUM(集計実績)", filter);
                    newRow["集計個数"] = dt.Compute("SUM(集計個数)", filter);
                    bunkatsuSyohinName = dr["分割集計商品名"].ToString();
                    newRow["分割集計商品名"] = bunkatsuSyohinName;
                    newRow["分割集計実績"] = dt.Compute("SUM(分割集計実績)", filter);
                    newRow["分割集計個数"] = dt.Compute("SUM(分割集計個数)", filter);
                    newRow["ロス"] = dt.Compute("SUM(ロス)", filter);
                    newRow["現金合計"] = dt.Compute("SUM(現金合計)", filter);
                    newRow["チケット金額"] = dt.Compute("SUM(チケット金額)", filter);
                    newRow["チケット枚数"] = dt.Compute("SUM(チケット枚数)", filter);
                    newRow["売掛合計"] = dt.Compute("SUM(売掛合計)", filter);
                    newRow["電子決済"] = dt.Compute("SUM(電子決済)", filter);
                    newRow["総売上"] = dt.Compute("SUM(総売上)", filter);
                    newRow["その他売上"] = dt.Compute("SUM(その他売上)", filter);
                    newRow["持出数"] = dt.Compute("SUM(持出数)", filter);
                    dt.Rows.Add(newRow);
                }

                // 総合計行を追加
                grossRow["部署ＣＤ"] = busyoCD;     // 総合計行は最後の部署のページに含める
                grossRow["部署名"] = busyoName;
                grossRow["分割集計商品名"] = bunkatsuSyohinName;
                dt.Rows.Add(grossRow);

                // 部署計と総合計を含めてソートし直す
                DataRow[] drs = dt.Select(null, "部署ソート順, 部署ＣＤ, 配送担当者ＣＤソート順, 配送担当者ＣＤ");
                dt = dt.Clone();
                foreach (DataRow dr in drs)
                {
                    dt.ImportRow(dr);
                }

                Report817TantousyaA report817TantousyaA = new Report817TantousyaA();
                report817TantousyaA.DataSource = dt;
                report817TantousyaA.Document.Printer.DocumentName = "担当者別実績集計";
                report817TantousyaA.Document.Printer.PrinterName = printerNameA4;

                // プレビューを表示
                FormPreview form = new FormPreview();
                form.MdiParent = this.MdiParent;
                form.Title = "担当者別実績集計";
                form.Show();
                form.ActiveReportViewer.LoadDocument(report817TantousyaA);
                form.FormClosed += new FormClosedEventHandler(formPreview_FormClosed);

                log.InfoFormat("担当者別実績集計を印刷しました。件数={0}", dt.Rows.Count);
                History.Save(this.Text, "担当者別実績集計", _uriageBunsekiHyoCriteria.Code, History.ProcessType.Output);
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// プレビュー画面クローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formPreview_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FormPreview form = (FormPreview)sender;
                form.Dispose();
                form = null;
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
        /// 部署別実績日計の帳票・Excel共通のデータソースになるデータテーブルを返す
        /// </summary>
        /// <returns>部署別実績日計用のデータテーブル</returns>
        private DataTable GetBusyoDataTable()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     @売上日From AS 売上日From");
                    sql.AppendLine("     ,@売上日To AS 売上日To");
                    sql.AppendLine("     ,MAIN.部署ＣＤ");
                    sql.AppendLine("     ,BM.部署名");
                    sql.AppendLine("     ,MAIN.売上日");
                    sql.AppendLine("     ,'' AS 合計見出し");
                    sql.AppendLine("     ,0 AS 部署ソート順");
                    sql.AppendLine("     ,0 AS 売上日ソート順");
                    sql.AppendLine("     ,ISNULL(SYUKEI_KIN.集計実績, 0) + ISNULL(TICKET_KIN.集計実績, 0) AS 集計実績");
                    sql.AppendLine("     ,ISNULL(SYUKEI_KOSU.集計個数, 0) AS 集計個数");
                    sql.AppendLine("     ,(SELECT 商品名略称 FROM 商品マスタ WHERE 企業ＣＤ = @企業ＣＤ AND 商品ＣＤ = ISNULL(@分割集計商品ＣＤ, -1)) AS 分割集計商品名");
                    sql.AppendLine("     ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(BUNKATSU_KIN.分割集計実績, 0) END AS 分割集計実績");
                    sql.AppendLine("     ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(BUNKATSU_KOSU.分割集計個数, 0) END AS 分割集計個数");
                    sql.AppendLine("     ,ISNULL(MCDS.持出数, 0) - (ISNULL(SYUKEI_KOSU.集計個数, 0) + ISNULL(BUNKATSU_KOSU.分割集計個数, 0)) AS ロス");
                    sql.AppendLine("     ,ISNULL(MAIN.現金合計, 0) AS 現金合計");
                    sql.AppendLine("     ,ISNULL(TICKET.金額, 0) AS チケット金額");
                    sql.AppendLine("     ,ISNULL(TICKET.枚数, 0) AS チケット枚数");
                    sql.AppendLine("     ,ISNULL(MAIN.売掛合計, 0) AS 売掛合計");
                    sql.AppendLine("     ,ISNULL(MAIN.電子決済, 0) AS 電子決済");
                    sql.AppendLine("     ,ISNULL(MAIN.総売上, 0) AS 総売上");
                    sql.AppendLine("     ,ISNULL(SONOTA.その他売上, 0) AS その他売上");
                    sql.AppendLine("     ,ISNULL(MCDS.持出数, 0) AS 持出数");
                    sql.AppendLine(" FROM");
                    // -- ①駆動表（売上日＆金額合計）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         部署ＣＤ");
                    sql.AppendLine("         ,売上日");
                    sql.AppendLine("         ,SUM(CASE WHEN 売上区分 IN (@売上区分現金, @売上区分バークレー)   THEN 商品金額合計税込 ELSE 0 END) AS 現金合計");
                    sql.AppendLine("         ,SUM(CASE WHEN 売上区分 IN (@売上区分売掛)                        THEN 商品金額合計税込 ELSE 0 END) AS 売掛合計");
                    sql.AppendLine("         ,SUM(CASE WHEN 売上区分 IN (@売上区分LINE)                        THEN 商品金額合計税込 ELSE 0 END) AS 電子決済");
                    sql.AppendLine("         ,SUM(CASE WHEN 売上区分 IN (@売上区分チケット, @売上区分ボーナス) THEN 0 ELSE 商品金額合計税込 END) AS 総売上");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上明細");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("         AND 売上日 >= @売上日From");
                    sql.AppendLine("         AND 売上日 <= @売上日To");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         部署ＣＤ");
                    sql.AppendLine("         ,売上日");
                    sql.AppendLine("     ) AS MAIN");
                    // -- ②集計個数（分割集計商品ＣＤ以外を対象とし、弁当or主食or副食のみが集計対象）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,SUM(PART.商品売上数量) AS 集計個数");
                    sql.AppendLine("         FROM");
                    // -- ②－８ 売上の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１５ 売上のチケットの主食
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
                    // -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                     ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");   // -- チケット
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");                  // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");         // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１６ 売上のチケットの副食
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
                    // -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                     ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");   // -- チケット
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");                  // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");         // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("         ) AS SYUKEI_KOSU ON MAIN.部署ＣＤ = SYUKEI_KOSU.部署ＣＤ AND MAIN.売上日 = SYUKEI_KOSU.売上日");
                    // -- ③分割集計個数（分割集計商品ＣＤのみを対象とし、（弁当or主食or副食に加え）その他も含める点が②集計個数とは異なる）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(PART.商品売上数量) END AS 分割集計個数");
                    sql.AppendLine("         FROM");
                    // -- ③－８ 売上の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１２ 売上の主食でも副食でもないもの
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) = 0");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.商品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１３ 売上のその他の主食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１４ 売上のその他の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("         ) AS BUNKATSU_KOSU ON MAIN.部署ＣＤ = BUNKATSU_KOSU.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KOSU.売上日");
                    // -- ④集計金額（弁当or副食or主食）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS D");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             D.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND D.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND D.売上日 >= @売上日From");
                    sql.AppendLine("             AND D.売上日 <= @売上日To");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) IN (@弁当区分弁当, @弁当区分副食, @弁当区分主食)");
                    sql.AppendLine("             AND D.売上区分 <> @売上区分ボーナス");     // -- チケット回収時売上フラグに関係なくボーナスは含めない
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("         ) AS SYUKEI_KIN ON MAIN.部署ＣＤ = SYUKEI_KIN.部署ＣＤ AND MAIN.売上日 = SYUKEI_KIN.売上日");
                    // -- ⑤集計金額（チケット束売り）
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよい。
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS D");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                 ON K.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             D.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND D.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND D.売上日 >= @売上日From");
                    sql.AppendLine("             AND D.売上日 <= @売上日To");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");
                    sql.AppendLine("             AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");       // -- チケット回収時に売上にしない
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("         ) AS TICKET_KIN ON MAIN.部署ＣＤ = TICKET_KIN.部署ＣＤ AND MAIN.売上日 = TICKET_KIN.売上日");
                    // -- ⑥分割集計金額
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("             ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(D.商品金額合計税込) END AS 分割集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS D");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             D.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND D.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND D.売上日 >= @売上日From");
                    sql.AppendLine("             AND D.売上日 <= @売上日To");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("         ) AS BUNKATSU_KIN ON MAIN.部署ＣＤ = BUNKATSU_KIN.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KIN.売上日");
                    // -- ⑦チケット払
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             部署ＣＤ");
                    sql.AppendLine("             ,売上日");
                    sql.AppendLine("             ,SUM(商品数量) AS 枚数");
                    sql.AppendLine("             ,SUM(商品金額合計税込) AS 金額");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND 部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND 売上日 >= @売上日From");
                    sql.AppendLine("             AND 売上日 <= @売上日To");
                    sql.AppendLine("             AND 売上区分 IN (@売上区分チケット, @売上区分ボーナス)");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             部署ＣＤ");
                    sql.AppendLine("             ,売上日");
                    sql.AppendLine("         ) AS TICKET ON MAIN.部署ＣＤ = TICKET.部署ＣＤ AND MAIN.売上日 = TICKET.売上日");
                    // -- ⑧その他売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS その他売上");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上明細 AS D");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             D.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND D.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND D.売上日 >= @売上日From");
                    sql.AppendLine("             AND D.売上日 <= @売上日To");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分その他");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             D.部署ＣＤ");
                    sql.AppendLine("             ,D.売上日");
                    sql.AppendLine("         ) AS SONOTA ON MAIN.部署ＣＤ = SONOTA.部署ＣＤ AND MAIN.売上日 = SONOTA.売上日");
                    // -- ⑨持出数
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.部署ＣＤ");
                    sql.AppendLine("             ,M.持出日");
                    sql.AppendLine("             ,SUM(M.個数) AS 持出数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_持ち出し入力 AS M");
                    sql.AppendLine("             INNER JOIN コースマスタ AS C ON M.企業ＣＤ = C.企業ＣＤ AND M.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.持出日 >= @売上日From");
                    sql.AppendLine("             AND M.持出日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND C.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             C.部署ＣＤ");
                    sql.AppendLine("             ,M.持出日");
                    sql.AppendLine("         ) AS MCDS ON MAIN.部署ＣＤ = MCDS.部署ＣＤ AND MAIN.売上日 = MCDS.持出日");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = @企業ＣＤ AND BM.部署ＣＤ = MAIN.部署ＣＤ");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     部署ソート順");
                    sql.AppendLine("     ,MAIN.部署ＣＤ");
                    sql.AppendLine("     ,売上日ソート順");
                    sql.AppendLine("     ,MAIN.売上日");

                    // パラメータを設定
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日From", SqlDbType.Date).Value = (DateTime)_uriageBunsekiHyoCriteria.売上日From;
                    cmd.Parameters.Add("@売上日To", SqlDbType.Date).Value = (DateTime)_uriageBunsekiHyoCriteria.売上日To;
                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ == null)
                    {
                        cmd.Parameters.Add("@分割集計商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@分割集計商品ＣＤ", SqlDbType.Int).Value = _uriageBunsekiHyoCriteria.分割集計商品ＣＤ;
                    }
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;
                    cmd.Parameters.Add("@売上区分現金", SqlDbType.Int).Value = (int)Consts.UriageKubun.Genkin;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                    cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                    cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;
                    cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageBunsekiHyoCriteria.部署ＣＤ;
                    }

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("部署別実績日計");
                    adp.Fill(dt);
                }

                return dt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// コース別実績日計の帳票・Excel共通のデータソースになるデータテーブルを返す
        /// </summary>
        /// <returns>コース別実績日計のデータテーブル</returns>
        private DataTable GetCourseDataTable()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     MAIN.部署ＣＤ");
                    sql.AppendLine("     ,BM.部署名");
                    sql.AppendLine("     ,MAIN.売上日");
                    sql.AppendLine("     ,MAIN.コースＣＤ");
                    sql.AppendLine("     ,0 AS 部署ソート順");
                    sql.AppendLine("     ,0 AS 売上日ソート順");
                    sql.AppendLine("     ,0 AS コースＣＤソート順");
                    sql.AppendLine("     ,ISNULL(RM.コース名, CM.コース名) AS コース名");
                    sql.AppendLine("     ,USH.配送担当者名 AS 担当者名");
                    sql.AppendLine("     ,ISNULL(SYUKEI_KIN.集計実績, 0) + ISNULL(TICKET_KIN.集計実績, 0) AS 集計実績");
                    sql.AppendLine("     ,ISNULL(SYUKEI_KOSU.集計個数, 0) AS 集計個数");
                    sql.AppendLine("     ,(SELECT 商品名略称 FROM 商品マスタ WHERE 企業ＣＤ = @企業ＣＤ AND 商品ＣＤ = ISNULL(@分割集計商品ＣＤ, -1)) AS 分割集計商品名");
                    sql.AppendLine("     ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(BUNKATSU_KIN.分割集計実績, 0) END AS 分割集計実績");
                    sql.AppendLine("     ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(BUNKATSU_KOSU.分割集計個数, 0) END AS 分割集計個数");
                    sql.AppendLine("     ,ISNULL(MCDS.持出数, 0) - (ISNULL(SYUKEI_KOSU.集計個数, 0) + ISNULL(BUNKATSU_KOSU.分割集計個数, 0)) AS ロス");
                    sql.AppendLine("     ,ISNULL(MAIN.現金合計, 0) AS 現金合計");
                    sql.AppendLine("     ,ISNULL(TICKET.金額, 0) AS チケット金額");
                    sql.AppendLine("     ,ISNULL(TICKET.枚数, 0) AS チケット枚数");
                    sql.AppendLine("     ,ISNULL(MAIN.売掛合計, 0) AS 売掛合計");
                    sql.AppendLine("     ,ISNULL(MAIN.電子決済, 0) AS 電子決済");
                    sql.AppendLine("     ,ISNULL(MAIN.総売上, 0) AS 総売上");
                    sql.AppendLine("     ,ISNULL(SONOTA.その他売上, 0) AS その他売上");
                    sql.AppendLine("     ,ISNULL(MCDS.持出数, 0) AS 持出数");
                    sql.AppendLine("     ,ISNULL(TAKE.個数, 0) - ISNULL(GIVE.個数, 0) AS 移動数");  // -- もらった個数－渡した個数
                    sql.AppendLine(" FROM");
                    // -- ①駆動表（売上日＆金額合計）
                    sql.AppendLine("     (SELECT");
                    sql.AppendLine("         H.企業ＣＤ");
                    sql.AppendLine("         ,H.部署ＣＤ");
                    sql.AppendLine("         ,H.売上日");
                    sql.AppendLine("         ,H.コースＣＤ");
                    sql.AppendLine("         ,SUM(CASE WHEN D.売上区分 IN (@売上区分現金, @売上区分バークレー)   THEN D.商品金額合計税込 ELSE 0 END) AS 現金合計");
                    sql.AppendLine("         ,SUM(CASE WHEN D.売上区分 IN (@売上区分売掛)                        THEN D.商品金額合計税込 ELSE 0 END) AS 売掛合計");
                    sql.AppendLine("         ,SUM(CASE WHEN D.売上区分 IN (@売上区分LINE)                        THEN D.商品金額合計税込 ELSE 0 END) AS 電子決済");
                    sql.AppendLine("         ,SUM(CASE WHEN D.売上区分 IN (@売上区分チケット, @売上区分ボーナス) THEN 0 ELSE D.商品金額合計税込 END) AS 総売上");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上ヘッダ AS H");
                    sql.AppendLine("         INNER JOIN 売上明細 AS D");
                    sql.AppendLine("             ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("             AND D.売上日     = H.売上日");
                    sql.AppendLine("             AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("             AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("         AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("         AND H.売上日 >= @売上日From");
                    sql.AppendLine("         AND H.売上日 <= @売上日To");
                    sql.AppendLine("     GROUP BY");
                    sql.AppendLine("         H.企業ＣＤ");
                    sql.AppendLine("         ,H.部署ＣＤ");
                    sql.AppendLine("         ,H.売上日");
                    sql.AppendLine("         ,H.コースＣＤ");
                    sql.AppendLine("     ) AS MAIN");
                    // -- ②集計個数（分割集計商品ＣＤ以外を対象とし、弁当or主食or副食のみが集計対象）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("             ,SUM(PART.商品売上数量) AS 集計個数");
                    sql.AppendLine("         FROM");
                    // -- ②－８ 売上の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１５ 売上のチケットの主食
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
                    // -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                     ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");   // -- チケット
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");                  // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");         // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１６ 売上のチケットの副食
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
                    // -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                     ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");   // -- チケット
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");                  // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");         // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("         ) AS SYUKEI_KOSU ON MAIN.部署ＣＤ = SYUKEI_KOSU.部署ＣＤ AND MAIN.売上日 = SYUKEI_KOSU.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SYUKEI_KOSU.コースＣＤ, -1)");
                    // -- ③分割集計個数（分割集計商品ＣＤのみを対象とし、（弁当or主食or副食に加え）その他も含める点が②集計個数とは異なる）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("             ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(PART.商品売上数量) END AS 分割集計個数");
                    sql.AppendLine("         FROM");
                    // -- ③－８ 売上の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１２ 売上の主食でも副食でもないもの
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) = 0");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.商品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１３ 売上のその他の主食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１４ 売上のその他の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("         ) AS BUNKATSU_KOSU ON MAIN.部署ＣＤ = BUNKATSU_KOSU.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KOSU.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(BUNKATSU_KOSU.コースＣＤ, -1)");
                    // -- ④集計金額（弁当or副食or主食）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) IN (@弁当区分弁当, @弁当区分副食, @弁当区分主食)");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS SYUKEI_KIN ON MAIN.部署ＣＤ = SYUKEI_KIN.部署ＣＤ AND MAIN.売上日 = SYUKEI_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SYUKEI_KIN.コースＣＤ, -1)");
                    // -- ⑤集計金額（チケット束売り）
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよい。
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                 ON K.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");
                    sql.AppendLine("             AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS TICKET_KIN ON MAIN.部署ＣＤ = TICKET_KIN.部署ＣＤ AND MAIN.売上日 = TICKET_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TICKET_KIN.コースＣＤ, -1)");
                    // -- ⑥分割集計金額
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(D.商品金額合計税込) END AS 分割集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS BUNKATSU_KIN ON MAIN.部署ＣＤ = BUNKATSU_KIN.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(BUNKATSU_KIN.コースＣＤ, -1)");
                    // -- ⑦チケット払
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品数量) AS 枚数");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 金額");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");
                    sql.AppendLine("             AND D.売上区分 IN (@売上区分チケット, @売上区分ボーナス)");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS TICKET ON MAIN.部署ＣＤ = TICKET.部署ＣＤ AND MAIN.売上日 = TICKET.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TICKET.コースＣＤ, -1)");
                    // -- ⑧その他売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS その他売上");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分その他");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS SONOTA ON MAIN.部署ＣＤ = SONOTA.部署ＣＤ AND MAIN.売上日 = SONOTA.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SONOTA.コースＣＤ, -1)");
                    // -- ⑨持出数
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.部署ＣＤ");
                    sql.AppendLine("             ,M.持出日");
                    sql.AppendLine("             ,C.コースＣＤ");
                    sql.AppendLine("             ,SUM(M.個数) AS 持出数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_持ち出し入力 AS M");
                    sql.AppendLine("             INNER JOIN コースマスタ AS C ON M.企業ＣＤ = C.企業ＣＤ AND M.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.持出日 >= @売上日From");
                    sql.AppendLine("             AND M.持出日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND C.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             C.部署ＣＤ");
                    sql.AppendLine("             ,M.持出日");
                    sql.AppendLine("             ,C.コースＣＤ");
                    sql.AppendLine("         ) AS MCDS ON MAIN.部署ＣＤ = MCDS.部署ＣＤ AND MAIN.売上日 = MCDS.持出日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(MCDS.コースＣＤ, -1)");
                    // -- ⑩もらった個数
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             移動日 AS 売上日");
                    sql.AppendLine("             ,相手コースＣＤ");
                    sql.AppendLine("             ,SUM(個数) AS 個数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力 AS MI");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 移動日 >= @売上日From");
                    sql.AppendLine("             AND 移動日 <= @売上日To");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             移動日");
                    sql.AppendLine("             ,相手コースＣＤ");
                    sql.AppendLine("         ) AS TAKE ON MAIN.売上日 = TAKE.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TAKE.相手コースＣＤ, -1)");
                    // -- ⑪渡した個数
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             移動日 AS 売上日");
                    sql.AppendLine("             ,コースＣＤ");
                    sql.AppendLine("             ,SUM(個数) AS 個数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_移動入力");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND 移動日 >= @売上日From");
                    sql.AppendLine("             AND 移動日 <= @売上日To");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             移動日");
                    sql.AppendLine("             ,コースＣＤ");
                    sql.AppendLine("         ) AS GIVE ON MAIN.売上日 = GIVE.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(GIVE.コースＣＤ, -1)");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = MAIN.企業ＣＤ AND BM.部署ＣＤ = MAIN.部署ＣＤ");
                    // -- コースマスタ
                    sql.AppendLine("     LEFT JOIN コースマスタ AS CM ON CM.企業ＣＤ = MAIN.企業ＣＤ AND CM.コースＣＤ = MAIN.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 売上精算_ヘッダ AS USH ON USH.企業ＣＤ = MAIN.企業ＣＤ AND USH.部署ＣＤ = MAIN.部署ＣＤ AND USH.精算日 = MAIN.売上日 AND USH.コースＣＤ = MAIN.コースＣＤ");
                    sql.AppendLine("     LEFT JOIN 臨時コースマスタ AS RM ON RM.企業ＣＤ = MAIN.企業ＣＤ AND RM.コースＣＤ = MAIN.コースＣＤ AND RM.実施日 = MAIN.売上日");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     部署ソート順");
                    sql.AppendLine("     ,MAIN.部署ＣＤ");
                    sql.AppendLine("     ,売上日ソート順");
                    sql.AppendLine("     ,MAIN.売上日");
                    sql.AppendLine("     ,コースＣＤソート順");
                    sql.AppendLine("     ,MAIN.コースＣＤ");

                    // パラメータを設定
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日From", SqlDbType.Date).Value = (DateTime)_uriageBunsekiHyoCriteria.売上日From;
                    cmd.Parameters.Add("@売上日To", SqlDbType.Date).Value = (DateTime)_uriageBunsekiHyoCriteria.売上日To;
                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ == null)
                    {
                        cmd.Parameters.Add("@分割集計商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@分割集計商品ＣＤ", SqlDbType.Int).Value = _uriageBunsekiHyoCriteria.分割集計商品ＣＤ;
                    }
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;
                    cmd.Parameters.Add("@売上区分現金", SqlDbType.Int).Value = (int)Consts.UriageKubun.Genkin;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                    cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                    cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;
                    cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageBunsekiHyoCriteria.部署ＣＤ;
                    }

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("コース別実績日計");
                    adp.Fill(dt);
                }

                return dt;
            }
            catch (BentoException)
            {
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                History.Error(ex);
                throw new BentoException(ex);
            }
        }

        /// <summary>
        /// 担当者別実績集計の帳票・Excel共通のデータソースになるデータテーブルを返す
        /// </summary>
        /// <returns>担当者別実績集計のデータテーブル</returns>
        private DataTable GetTantousyaDataTable()
        {
            try
            {
                DataTable dt = null;

                using (SqlConnection con = new SqlConnection(DBUtil.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = con.CreateCommand();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT");
                    sql.AppendLine("     @売上日From AS 売上日From");
                    sql.AppendLine("     ,@売上日To AS 売上日To");
                    sql.AppendLine("     ,USH.部署ＣＤ");
                    sql.AppendLine("     ,BM.部署名");
                    sql.AppendLine("     ,USH.配送担当者ＣＤ");
                    sql.AppendLine("     ,USH.配送担当者名");
                    sql.AppendLine("     ,0 AS 部署ソート順");
                    sql.AppendLine("     ,0 AS 配送担当者ＣＤソート順");
                    sql.AppendLine("     ,ISNULL(SUM(SYUKEI_KIN.集計実績), 0) + ISNULL(SUM(TICKET_KIN.集計実績), 0) AS 集計実績");
                    sql.AppendLine("     ,ISNULL(SUM(SYUKEI_KOSU.集計個数), 0) AS 集計個数");
                    sql.AppendLine("     ,(SELECT 商品名略称 FROM 商品マスタ WHERE 企業ＣＤ = @企業ＣＤ AND 商品ＣＤ = ISNULL(@分割集計商品ＣＤ, -1)) AS 分割集計商品名");
                    sql.AppendLine("     ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(SUM(BUNKATSU_KIN.分割集計実績), 0) END AS 分割集計実績");
                    sql.AppendLine("     ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE ISNULL(SUM(BUNKATSU_KOSU.分割集計個数), 0) END AS 分割集計個数");
                    sql.AppendLine("     ,ISNULL(SUM(MCDS.持出数), 0) - (ISNULL(SUM(SYUKEI_KOSU.集計個数), 0) + ISNULL(SUM(BUNKATSU_KOSU.分割集計個数), 0)) AS ロス");
                    sql.AppendLine("     ,ISNULL(SUM(MAIN.現金合計), 0) AS 現金合計");
                    sql.AppendLine("     ,ISNULL(SUM(TICKET.金額), 0) AS チケット金額");
                    sql.AppendLine("     ,ISNULL(SUM(TICKET.枚数), 0) AS チケット枚数");
                    sql.AppendLine("     ,ISNULL(SUM(MAIN.売掛合計), 0) AS 売掛合計");
                    sql.AppendLine("     ,ISNULL(SUM(MAIN.電子決済), 0) AS 電子決済");
                    sql.AppendLine("     ,ISNULL(SUM(MAIN.総売上), 0) AS 総売上");
                    sql.AppendLine("     ,ISNULL(SUM(SONOTA.その他売上), 0) AS その他売上");
                    sql.AppendLine("     ,ISNULL(SUM(MCDS.持出数), 0) AS 持出数");
                    sql.AppendLine(" FROM");
                    // -- キー（一人の配送担当者が複数のコースを担当することがある。このタイプの売上分析表ではそれを配送担当者ごとに集計する）
                    sql.AppendLine("     (SELECT DISTINCT");
                    sql.AppendLine("         企業ＣＤ");
                    sql.AppendLine("         ,精算日 AS 売上日");
                    sql.AppendLine("         ,部署ＣＤ");
                    sql.AppendLine("         ,コースＣＤ");
                    sql.AppendLine("         ,配送担当者ＣＤ");
                    sql.AppendLine("         ,配送担当者名");
                    sql.AppendLine("     FROM");
                    sql.AppendLine("         売上精算_ヘッダ");
                    sql.AppendLine("     WHERE");
                    sql.AppendLine("         企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("         AND 部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("         AND 精算日 >= @売上日From");
                    sql.AppendLine("         AND 精算日 <= @売上日To");
                    sql.AppendLine("     ) AS USH");
                    // -- ①駆動表（売上日＆金額合計）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.企業ＣＤ");
                    sql.AppendLine("             ,H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(CASE WHEN D.売上区分 IN (@売上区分現金, @売上区分バークレー)   THEN D.商品金額合計税込 ELSE 0 END) AS 現金合計");
                    sql.AppendLine("             ,SUM(CASE WHEN D.売上区分 IN (@売上区分売掛)                        THEN D.商品金額合計税込 ELSE 0 END) AS 売掛合計");
                    sql.AppendLine("             ,SUM(CASE WHEN D.売上区分 IN (@売上区分LINE)                        THEN D.商品金額合計税込 ELSE 0 END) AS 電子決済");
                    sql.AppendLine("             ,SUM(CASE WHEN D.売上区分 IN (@売上区分チケット, @売上区分ボーナス) THEN 0 ELSE D.商品金額合計税込 END) AS 総売上");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.企業ＣＤ");
                    sql.AppendLine("             ,H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS MAIN ON USH.企業ＣＤ = MAIN.企業ＣＤ AND USH.部署ＣＤ = MAIN.部署ＣＤ AND USH.売上日 = MAIN.売上日 AND ISNULL(USH.コースＣＤ, -1) = ISNULL(MAIN.コースＣＤ, -1)");
                    // -- ②集計個数（分割集計商品ＣＤ以外を対象とし、弁当or主食or副食のみが集計対象）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("             ,SUM(PART.商品売上数量) AS 集計個数");
                    sql.AppendLine("         FROM");
                    // -- ②－８ 売上の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１５ 売上のチケットの主食
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
                    // -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                     ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");   // -- チケット
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");                  // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");         // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ②－１６ 売上のチケットの副食
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額と個数を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよいが、個数は主食と副食をそれぞれ集計する必要がある。
                    // -- チケット束売りに主食・副食が設定されていることはほぼないが、もしあればそれを集計する。
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("                 INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                     ON K.企業ＣＤ = H.企業ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");   // -- チケット
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");                  // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");         // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("                 AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");   // -- チケット回収時に売上にしない
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("         ) AS SYUKEI_KOSU ON MAIN.部署ＣＤ = SYUKEI_KOSU.部署ＣＤ AND MAIN.売上日 = SYUKEI_KOSU.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SYUKEI_KOSU.コースＣＤ, -1)");
                    // -- ③分割集計個数（分割集計商品ＣＤのみを対象とし、（弁当or主食or副食に加え）その他も含める点が②集計個数とは異なる）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("             ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(PART.商品売上数量) END AS 分割集計個数");
                    sql.AppendLine("         FROM");
                    // -- ③－８ 売上の弁当の主食
                    sql.AppendLine("             (SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－９ 売上の弁当の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分弁当");   // -- 弁当
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１０ 売上の主食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分主食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１１ 売上の副食のみ
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分副食");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１２ 売上の主食でも副食でもないもの
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.商品ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) = 0");
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) = 0");
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.商品ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１３ 売上のその他の主食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> 0");              // -- 主食がある
                    sql.AppendLine("                 AND ISNULL(D.主食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(主食の場合、主食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.主食ＣＤ");
                    sql.AppendLine("             UNION ALL");
                    // -- ③－１４ 売上のその他の副食
                    sql.AppendLine("             SELECT");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ AS 部品ＣＤ");
                    sql.AppendLine("                 ,SUM(D.商品数量) AS 商品売上数量");
                    sql.AppendLine("             FROM");
                    sql.AppendLine("                 売上ヘッダ AS H");
                    sql.AppendLine("                 INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                     ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                     AND D.売上日     = H.売上日");
                    sql.AppendLine("                     AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                     AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("                 INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                     ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                     AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                     AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             WHERE");
                    sql.AppendLine("                 H.企業ＣＤ    = @企業ＣＤ");
                    sql.AppendLine("                 AND H.売上日 >= @売上日From");
                    sql.AppendLine("                 AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("                 AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("                 AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("                 AND ISNULL(I.弁当区分, 0) = @弁当区分その他"); // -- その他
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> 0");              // -- 副食がある
                    sql.AppendLine("                 AND ISNULL(D.副食ＣＤ, 0) <> D.商品ＣＤ");     // -- 自分以外(副食の場合、副食ＣＤに自分自身の商品ＣＤが入っていることがあるので念のため)
                    sql.AppendLine("             GROUP BY");
                    sql.AppendLine("                 H.部署ＣＤ");
                    sql.AppendLine("                 ,H.売上日");
                    sql.AppendLine("                 ,H.コースＣＤ");
                    sql.AppendLine("                 ,H.得意先ＣＤ");
                    sql.AppendLine("                 ,D.副食ＣＤ");
                    sql.AppendLine("             ) AS PART");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             PART.部署ＣＤ");
                    sql.AppendLine("             ,PART.売上日");
                    sql.AppendLine("             ,PART.コースＣＤ");
                    sql.AppendLine("         ) AS BUNKATSU_KOSU ON MAIN.部署ＣＤ = BUNKATSU_KOSU.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KOSU.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(BUNKATSU_KOSU.コースＣＤ, -1)");
                    // -- ④集計金額（弁当or副食or主食）
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) IN (@弁当区分弁当, @弁当区分副食, @弁当区分主食)");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS SYUKEI_KIN ON MAIN.部署ＣＤ = SYUKEI_KIN.部署ＣＤ AND MAIN.売上日 = SYUKEI_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SYUKEI_KIN.コースＣＤ, -1)");
                    // -- ⑤集計金額（チケット束売り）
                    // -- 管理マスタ.チケット回収時売上フラグがOFFの場合は、売上区分がチケットorボーナスの売上金額は0になっている。
                    // -- つまりONの場合は金額が計上されているので、OFFの場合の金額を計上する必要がある。
                    // -- →OFFの場合は、チケット束売り（弁当区分=8:チケット）の金額を計上する。
                    // -- 金額はチケット束売りの売上明細の商品金額合計税込を集計すればよい。
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("             INNER JOIN 管理マスタ AS K");
                    sql.AppendLine("                 ON K.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ <> @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分チケット");
                    sql.AppendLine("             AND ISNULL(K.チケット回収時売上フラグ, 0) = 0");       // -- チケット回収時に売上にしない
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS TICKET_KIN ON MAIN.部署ＣＤ = TICKET_KIN.部署ＣＤ AND MAIN.売上日 = TICKET_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TICKET_KIN.コースＣＤ, -1)");
                    // -- ⑥分割集計金額
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,CASE WHEN @分割集計商品ＣＤ IS NULL THEN NULL ELSE SUM(D.商品金額合計税込) END AS 分割集計実績");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ != null)
                    {
                        sql.AppendLine("             AND D.商品ＣＤ = @分割集計商品ＣＤ");
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS BUNKATSU_KIN ON MAIN.部署ＣＤ = BUNKATSU_KIN.部署ＣＤ AND MAIN.売上日 = BUNKATSU_KIN.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(BUNKATSU_KIN.コースＣＤ, -1)");
                    // -- ⑦チケット払
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品数量) AS 枚数");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS 金額");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");
                    sql.AppendLine("             AND D.売上区分 IN (@売上区分チケット, @売上区分ボーナス)");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS TICKET ON MAIN.部署ＣＤ = TICKET.部署ＣＤ AND MAIN.売上日 = TICKET.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(TICKET.コースＣＤ, -1)");
                    // -- ⑧その他売上
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("             ,SUM(D.商品金額合計税込) AS その他売上");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             売上ヘッダ AS H");
                    sql.AppendLine("             INNER JOIN 売上明細 AS D");
                    sql.AppendLine("                 ON D.企業ＣＤ    = H.企業ＣＤ");
                    sql.AppendLine("                 AND D.売上日     = H.売上日");
                    sql.AppendLine("                 AND D.得意先ＣＤ = H.得意先ＣＤ");
                    sql.AppendLine("                 AND D.部署ＣＤ   = H.部署ＣＤ");
                    sql.AppendLine("             INNER JOIN 商品マスタ AS I");
                    sql.AppendLine("                 ON I.企業ＣＤ = D.企業ＣＤ");
                    sql.AppendLine("                 AND ISNULL(I.無効フラグ, 0) = 0");
                    sql.AppendLine("                 AND I.商品ＣＤ = D.商品ＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             H.企業ＣＤ = @企業ＣＤ");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND H.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("             AND H.売上日 >= @売上日From");
                    sql.AppendLine("             AND H.売上日 <= @売上日To");
                    sql.AppendLine("             AND ISNULL(I.弁当区分, 0) = @弁当区分その他");
                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             H.部署ＣＤ");
                    sql.AppendLine("             ,H.売上日");
                    sql.AppendLine("             ,H.コースＣＤ");
                    sql.AppendLine("         ) AS SONOTA ON MAIN.部署ＣＤ = SONOTA.部署ＣＤ AND MAIN.売上日 = SONOTA.売上日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(SONOTA.コースＣＤ, -1)");
                    // -- ⑨持出数
                    sql.AppendLine("     LEFT JOIN");
                    sql.AppendLine("         (SELECT");
                    sql.AppendLine("             C.部署ＣＤ");
                    sql.AppendLine("             ,M.持出日");
                    sql.AppendLine("             ,C.コースＣＤ");
                    sql.AppendLine("             ,SUM(M.個数) AS 持出数");
                    sql.AppendLine("         FROM");
                    sql.AppendLine("             モバイル_持ち出し入力 AS M");
                    sql.AppendLine("             INNER JOIN コースマスタ AS C ON M.企業ＣＤ = C.企業ＣＤ AND M.コースＣＤ = C.コースＣＤ");
                    sql.AppendLine("         WHERE");
                    sql.AppendLine("             M.企業ＣＤ = @企業ＣＤ");
                    sql.AppendLine("             AND M.持出日 >= @売上日From");
                    sql.AppendLine("             AND M.持出日 <= @売上日To");

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        sql.AppendLine("             AND C.部署ＣＤ = @部署ＣＤ");
                    }

                    sql.AppendLine("         GROUP BY");
                    sql.AppendLine("             C.部署ＣＤ");
                    sql.AppendLine("             ,M.持出日");
                    sql.AppendLine("             ,C.コースＣＤ");
                    sql.AppendLine("         ) AS MCDS ON MAIN.部署ＣＤ = MCDS.部署ＣＤ AND MAIN.売上日 = MCDS.持出日 AND ISNULL(MAIN.コースＣＤ, -1) = ISNULL(MCDS.コースＣＤ, -1)");
                    sql.AppendLine("     LEFT JOIN 部署マスタ AS BM ON BM.企業ＣＤ = USH.企業ＣＤ AND BM.部署ＣＤ = USH.部署ＣＤ");
                    sql.AppendLine(" GROUP BY");
                    sql.AppendLine("     USH.部署ＣＤ");
                    sql.AppendLine("     ,BM.部署名");
                    sql.AppendLine("     ,USH.配送担当者ＣＤ");
                    sql.AppendLine("     ,USH.配送担当者名");
                    sql.AppendLine(" ORDER BY");
                    sql.AppendLine("     部署ソート順");
                    sql.AppendLine("     ,USH.部署ＣＤ");
                    sql.AppendLine("     ,配送担当者ＣＤソート順");
                    sql.AppendLine("     ,USH.配送担当者ＣＤ");

                    // パラメータを設定
                    cmd.Parameters.Add("@企業ＣＤ", SqlDbType.Int).Value = TableBase._企業ＣＤ;
                    cmd.Parameters.Add("@売上日From", SqlDbType.Date).Value = (DateTime)_uriageBunsekiHyoCriteria.売上日From;
                    cmd.Parameters.Add("@売上日To", SqlDbType.Date).Value = (DateTime)_uriageBunsekiHyoCriteria.売上日To;
                    if (_uriageBunsekiHyoCriteria.分割集計商品ＣＤ == null)
                    {
                        cmd.Parameters.Add("@分割集計商品ＣＤ", SqlDbType.Int).Value = DBNull.Value;
                    }
                    else
                    {
                        cmd.Parameters.Add("@分割集計商品ＣＤ", SqlDbType.Int).Value = _uriageBunsekiHyoCriteria.分割集計商品ＣＤ;
                    }
                    cmd.Parameters.Add("@弁当区分弁当", SqlDbType.Int).Value = (int)Consts.BentoKubun.Bento;
                    cmd.Parameters.Add("@弁当区分主食", SqlDbType.Int).Value = (int)Consts.BentoKubun.MainDish;
                    cmd.Parameters.Add("@弁当区分副食", SqlDbType.Int).Value = (int)Consts.BentoKubun.SubDish;
                    cmd.Parameters.Add("@弁当区分チケット", SqlDbType.Int).Value = (int)Consts.BentoKubun.Ticket;
                    cmd.Parameters.Add("@弁当区分その他", SqlDbType.Int).Value = (int)Consts.BentoKubun.Other;
                    cmd.Parameters.Add("@売上区分現金", SqlDbType.Int).Value = (int)Consts.UriageKubun.Genkin;
                    cmd.Parameters.Add("@売上区分売掛", SqlDbType.Int).Value = (int)Consts.UriageKubun.Urikake;
                    cmd.Parameters.Add("@売上区分チケット", SqlDbType.Int).Value = (int)Consts.UriageKubun.Ticket;
                    cmd.Parameters.Add("@売上区分バークレー", SqlDbType.Int).Value = (int)Consts.UriageKubun.Barclay;
                    cmd.Parameters.Add("@売上区分ボーナス", SqlDbType.Int).Value = (int)Consts.UriageKubun.Bonus;
                    cmd.Parameters.Add("@売上区分LINE", SqlDbType.Int).Value = (int)Consts.UriageKubun.Line;

                    if (_uriageBunsekiHyoCriteria.部署ＣＤ != null && _uriageBunsekiHyoCriteria.部署ＣＤ >= 0)
                    {
                        cmd.Parameters.Add("@部署ＣＤ", SqlDbType.Int).Value = _uriageBunsekiHyoCriteria.部署ＣＤ;
                    }

                    cmd.CommandText = sql.ToString();

                    log.Debug(cmd.CommandTextWithParameters());

                    SqlDataAdapter adp = new SqlDataAdapter(cmd);

                    dt = new DataTable("担当者別実績集計");
                    adp.Fill(dt);
                }

                return dt;
            }
            catch (BentoException)
            {
                throw;
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

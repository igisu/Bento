using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Emu.Bento.Common;
using GrapeCity.ActiveReports.Export.Pdf.Section;
using log4net;

namespace Emu.Bento
{
    /// <summary>
    /// アクティブレポートプレビュー
    /// </summary>
    public partial class FormPreview : Form
    {
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
        /// タイトル
        /// </summary>
        private string _title = "";

        #endregion

        //**************************************
        //* コンストラクタ
        //**************************************
        #region コンストラクタ
        public FormPreview()
        {
            InitializeComponent();

            // PDF出力ボタンを追加
            Image pdfICon = Properties.Resources.PDF;
            ToolStripButton tsbPDF = new ToolStripButton(pdfICon);
            tsbPDF.ToolTipText = "PDF出力";
            ActiveReportViewer.Toolbar.ToolStrip.Items.Insert(3, tsbPDF);   // 印刷ボタンの右
            tsbPDF.Click += TsbPDF_Click;
        }
        #endregion

        //**************************************
        //* プロパティ
        //**************************************
        #region プロパティ
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                this.Text = string.Format("プレビュー - {0}", _title);
            }
        }
        #endregion

        //**************************************
        //* イベント
        //**************************************
        #region イベント
        /// <summary>
        /// PDF出力ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsbPDF_Click(object sender, System.EventArgs e)
        {
            string fileName = "";
            try
            {
                saveFileDialog1.FileName = _title;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.MdiParent.Cursor = Cursors.WaitCursor;

                    fileName = saveFileDialog1.FileName;
                    PdfExport pdfExport = new PdfExport();
                    pdfExport.Options.Title = _title;
                    pdfExport.Export(ActiveReportViewer.Document, fileName);

                    this.MdiParent.Cursor = Cursors.Default;

                    if (MessageBox.Show(this, "PDF出力が完了しました。\n今すぐ開きますか？", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (EmuException ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(this, fileName + "\nファイルが開かれています。保存できませんでした。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            finally
            {
                this.MdiParent.Cursor = Cursors.Default;
            }
        }
        #endregion
    }
}

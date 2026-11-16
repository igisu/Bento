namespace Emu.Bento
{
    /// <summary>
    /// Report307A の概要の説明です。
    /// </summary>
    partial class Report307A
    {
        private GrapeCity.ActiveReports.SectionReportModel.PageHeader pageHeader;
        private GrapeCity.ActiveReports.SectionReportModel.Detail detail;
        private GrapeCity.ActiveReports.SectionReportModel.PageFooter pageFooter;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #region ActiveReport Designer generated code
        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report307A));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtChumonbi = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtSyokujiKbn = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtSyokusu = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChumonbi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokujiKbn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokusu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Height = 0F;
            this.pageHeader.Name = "pageHeader";
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.label1,
            this.txtChumonbi,
            this.txtSyokujiKbn,
            this.txtSyokusu,
            this.label2});
            this.detail.Height = 1.968504F;
            this.detail.Name = "detail";
            // 
            // label1
            // 
            this.label1.Height = 0.4602363F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.08346433F;
            this.label1.Name = "label1";
            this.label1.Style = "font-family: Meiryo UI; font-size: 22pt; font-weight: bold; vertical-align: middl" +
    "e";
            this.label1.Text = "喫食日";
            this.label1.Top = 0.1043307F;
            this.label1.Width = 1.01063F;
            // 
            // txtChumonbi
            // 
            this.txtChumonbi.DataField = "喫食日";
            this.txtChumonbi.Height = 0.4602363F;
            this.txtChumonbi.Left = 0.9901575F;
            this.txtChumonbi.Name = "txtChumonbi";
            this.txtChumonbi.OutputFormat = resources.GetString("txtChumonbi.OutputFormat");
            this.txtChumonbi.Style = "font-family: Meiryo UI; font-size: 22pt; font-weight: bold; text-align: right; ve" +
    "rtical-align: middle";
            this.txtChumonbi.Text = "12月31日(月)";
            this.txtChumonbi.Top = 0.1043307F;
            this.txtChumonbi.Width = 2.218111F;
            // 
            // txtSyokujiKbn
            // 
            this.txtSyokujiKbn.DataField = "大袋ラベル食事区分名";
            this.txtSyokujiKbn.Height = 1.181102F;
            this.txtSyokujiKbn.Left = 0.08346457F;
            this.txtSyokujiKbn.Name = "txtSyokujiKbn";
            this.txtSyokujiKbn.OutputFormat = resources.GetString("txtSyokujiKbn.OutputFormat");
            this.txtSyokujiKbn.ShrinkToFit = true;
            this.txtSyokujiKbn.Style = "font-family: Meiryo UI; font-size: 74pt; font-weight: bold; text-align: center; v" +
    "ertical-align: middle; white-space: inherit; ddo-shrink-to-fit: true; ddo-wrap-m" +
    "ode: char-wrap";
            this.txtSyokujiKbn.Text = "朝";
            this.txtSyokujiKbn.Top = 0.6078743F;
            this.txtSyokujiKbn.Width = 1.052362F;
            // 
            // txtSyokusu
            // 
            this.txtSyokusu.DataField = "食数";
            this.txtSyokusu.Height = 1.087795F;
            this.txtSyokusu.Left = 1.094095F;
            this.txtSyokusu.Name = "txtSyokusu";
            this.txtSyokusu.OutputFormat = resources.GetString("txtSyokusu.OutputFormat");
            this.txtSyokusu.ShrinkToFit = true;
            this.txtSyokusu.Style = "font-family: Meiryo UI; font-size: 63pt; font-weight: bold; text-align: right; ve" +
    "rtical-align: middle; ddo-shrink-to-fit: true";
            this.txtSyokusu.Text = "999";
            this.txtSyokusu.Top = 0.7011815F;
            this.txtSyokusu.Width = 1.823622F;
            // 
            // label2
            // 
            this.label2.Height = 0.3688976F;
            this.label2.HyperLink = null;
            this.label2.Left = 2.853544F;
            this.label2.Name = "label2";
            this.label2.Style = "font-family: Meiryo UI; font-size: 24pt; font-weight: bold; vertical-align: middl" +
    "e";
            this.label2.Text = "食";
            this.label2.Top = 1.420079F;
            this.label2.Width = 0.3547244F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // Report307A
            // 
            this.MasterReport = false;
            this.PageSettings.Collate = GrapeCity.ActiveReports.PageSettings.PrinterCollate.DontCollate;
            this.PageSettings.DefaultPaperSize = false;
            this.PageSettings.Duplex = System.Drawing.Printing.Duplex.Simplex;
            this.PageSettings.Margins.Bottom = 0F;
            this.PageSettings.Margins.Left = 0F;
            this.PageSettings.Margins.Right = 0F;
            this.PageSettings.Margins.Top = 0F;
            this.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait;
            this.PageSettings.PaperHeight = 1.968504F;
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.PageSettings.PaperName = "ユーザー定義のサイズ";
            this.PageSettings.PaperWidth = 3.35F;
            this.PrintWidth = 3.251969F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; font-size: 10pt; " +
            "color: Black; font-family: \"MS UI Gothic\"; ddo-char-set: 128", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; font-family: \"MS UI Gothic\"; ddo-char-set: 12" +
            "8", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: inherit; font-family: \"MS UI Goth" +
            "ic\"; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChumonbi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokujiKbn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokusu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtChumonbi;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSyokujiKbn;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSyokusu;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
    }
}

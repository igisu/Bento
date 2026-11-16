namespace Emu.Bento
{
    /// <summary>
    /// Report304A の概要の説明です。
    /// </summary>
    partial class Report304A
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report304A));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line5 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtChumonbi = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTokuisakiName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtSyokujiKubun = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCourseName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtSyokusu = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtBag = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line4 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtCourseOrder = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.barcode1 = new GrapeCity.ActiveReports.SectionReportModel.Barcode();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.txtChumonbi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokuisakiName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokujiKubun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokusu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseOrder)).BeginInit();
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
            this.line1,
            this.line2,
            this.line5,
            this.txtChumonbi,
            this.label1,
            this.txtTokuisakiName,
            this.label2,
            this.txtSyokujiKubun,
            this.txtCourseName,
            this.txtSyokusu,
            this.txtBag,
            this.line3,
            this.line4,
            this.txtCourseOrder,
            this.barcode1});
            this.detail.Height = 1.968504F;
            this.detail.Name = "detail";
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0.02362205F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.3267717F;
            this.line1.Width = 3.125984F;
            this.line1.X1 = 0.02362205F;
            this.line1.X2 = 3.149606F;
            this.line1.Y1 = 0.3267717F;
            this.line1.Y2 = 0.3267717F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0.02362191F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.6574804F;
            this.line2.Width = 3.125984F;
            this.line2.X1 = 0.02362191F;
            this.line2.X2 = 3.149606F;
            this.line2.Y1 = 0.6574804F;
            this.line2.Y2 = 0.6574804F;
            // 
            // line5
            // 
            this.line5.Height = 0F;
            this.line5.Left = 0.02362205F;
            this.line5.LineWeight = 1F;
            this.line5.Name = "line5";
            this.line5.Top = 1.641732F;
            this.line5.Width = 2.521654F;
            this.line5.X1 = 0.02362205F;
            this.line5.X2 = 2.545276F;
            this.line5.Y1 = 1.641732F;
            this.line5.Y2 = 1.641732F;
            // 
            // txtChumonbi
            // 
            this.txtChumonbi.CanGrow = false;
            this.txtChumonbi.DataField = "喫食日";
            this.txtChumonbi.Height = 0.2519685F;
            this.txtChumonbi.Left = 0.6952757F;
            this.txtChumonbi.Name = "txtChumonbi";
            this.txtChumonbi.OutputFormat = resources.GetString("txtChumonbi.OutputFormat");
            this.txtChumonbi.Style = "font-size: 16pt; ddo-char-set: 1";
            this.txtChumonbi.Text = "12月31日（月）";
            this.txtChumonbi.Top = 0.03661418F;
            this.txtChumonbi.Width = 1.364567F;
            // 
            // label1
            // 
            this.label1.Height = 0.2519685F;
            this.label1.HyperLink = null;
            this.label1.Left = 2.07756F;
            this.label1.Name = "label1";
            this.label1.Style = "font-size: 16pt; ddo-char-set: 1";
            this.label1.Text = "喫食";
            this.label1.Top = 0.03661418F;
            this.label1.Width = 0.4791338F;
            // 
            // txtTokuisakiName
            // 
            this.txtTokuisakiName.CanGrow = false;
            this.txtTokuisakiName.DataField = "得意先名";
            this.txtTokuisakiName.Height = 0.2519685F;
            this.txtTokuisakiName.Left = 0.3240158F;
            this.txtTokuisakiName.Name = "txtTokuisakiName";
            this.txtTokuisakiName.OutputFormat = resources.GetString("txtTokuisakiName.OutputFormat");
            this.txtTokuisakiName.ShrinkToFit = true;
            this.txtTokuisakiName.Style = "font-size: 16pt; text-align: center; vertical-align: middle; white-space: inherit" +
    "; ddo-char-set: 1; ddo-shrink-to-fit: true; ddo-wrap-mode: char-wrap";
            this.txtTokuisakiName.Text = "得意先名";
            this.txtTokuisakiName.Top = 0.3712599F;
            this.txtTokuisakiName.Width = 2.541732F;
            // 
            // label2
            // 
            this.label2.Height = 0.2519685F;
            this.label2.HyperLink = null;
            this.label2.Left = 2.891339F;
            this.label2.Name = "label2";
            this.label2.Style = "font-size: 16pt; ddo-char-set: 1";
            this.label2.Text = "様";
            this.label2.Top = 0.3712599F;
            this.label2.Width = 0.2291337F;
            // 
            // txtSyokujiKubun
            // 
            this.txtSyokujiKubun.CanGrow = false;
            this.txtSyokujiKubun.DataField = "食事区分別食数";
            this.txtSyokujiKubun.Height = 0.2519685F;
            this.txtSyokujiKubun.Left = 0.07401575F;
            this.txtSyokujiKubun.Name = "txtSyokujiKubun";
            this.txtSyokujiKubun.OutputFormat = resources.GetString("txtSyokujiKubun.OutputFormat");
            this.txtSyokujiKubun.ShrinkToFit = true;
            this.txtSyokujiKubun.Style = "font-size: 16pt; text-align: center; vertical-align: middle; ddo-char-set: 1; ddo" +
    "-shrink-to-fit: true";
            this.txtSyokujiKubun.Text = "朝 11    昼 12    夕 0";
            this.txtSyokujiKubun.Top = 0.6901575F;
            this.txtSyokujiKubun.Width = 3.051969F;
            // 
            // txtCourseName
            // 
            this.txtCourseName.CanGrow = false;
            this.txtCourseName.DataField = "コース名";
            this.txtCourseName.Height = 0.2519685F;
            this.txtCourseName.Left = 0.02362205F;
            this.txtCourseName.MultiLine = false;
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.OutputFormat = resources.GetString("txtCourseName.OutputFormat");
            this.txtCourseName.ShrinkToFit = true;
            this.txtCourseName.Style = "color: Black; font-size: 16pt; text-align: center; vertical-align: middle; white-" +
    "space: nowrap; ddo-char-set: 1; ddo-shrink-to-fit: true; ddo-wrap-mode: nowrap";
            this.txtCourseName.Text = "＿＿＿＿＿＿＿＿１０１２３４５";
            this.txtCourseName.Top = 0.9814962F;
            this.txtCourseName.Width = 2.719685F;
            // 
            // txtSyokusu
            // 
            this.txtSyokusu.CanGrow = false;
            this.txtSyokusu.DataField = "食数";
            this.txtSyokusu.Height = 0.3661422F;
            this.txtSyokusu.Left = 0.07401574F;
            this.txtSyokusu.Name = "txtSyokusu";
            this.txtSyokusu.OutputFormat = resources.GetString("txtSyokusu.OutputFormat");
            this.txtSyokusu.ShrinkToFit = true;
            this.txtSyokusu.Style = "font-size: 26pt; font-weight: normal; text-align: center; vertical-align: middle;" +
    " ddo-char-set: 1; ddo-shrink-to-fit: true";
            this.txtSyokusu.Text = "朝  11 食";
            this.txtSyokusu.Top = 1.26063F;
            this.txtSyokusu.Width = 2.489371F;
            // 
            // txtBag
            // 
            this.txtBag.CanGrow = false;
            this.txtBag.DataField = "袋数";
            this.txtBag.Height = 0.2519685F;
            this.txtBag.Left = 0.07401574F;
            this.txtBag.Name = "txtBag";
            this.txtBag.OutputFormat = resources.GetString("txtBag.OutputFormat");
            this.txtBag.ShrinkToFit = true;
            this.txtBag.Style = "font-size: 16pt; text-align: center; vertical-align: middle; ddo-char-set: 1; ddo" +
    "-shrink-to-fit: true";
            this.txtBag.Text = "3人×1　4人×1　5人×99";
            this.txtBag.Top = 1.67441F;
            this.txtBag.Width = 2.489765F;
            // 
            // line3
            // 
            this.line3.Height = 0F;
            this.line3.Left = 0.02362205F;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0.9566929F;
            this.line3.Width = 3.125984F;
            this.line3.X1 = 0.02362205F;
            this.line3.X2 = 3.149606F;
            this.line3.Y1 = 0.9566929F;
            this.line3.Y2 = 0.9566929F;
            // 
            // line4
            // 
            this.line4.Height = 0F;
            this.line4.Left = 0.02362205F;
            this.line4.LineWeight = 1F;
            this.line4.Name = "line4";
            this.line4.Top = 1.244094F;
            this.line4.Width = 3.125984F;
            this.line4.X1 = 0.02362205F;
            this.line4.X2 = 3.149606F;
            this.line4.Y1 = 1.244094F;
            this.line4.Y2 = 1.244094F;
            // 
            // txtCourseOrder
            // 
            this.txtCourseOrder.CanGrow = false;
            this.txtCourseOrder.DataField = "コース順";
            this.txtCourseOrder.Height = 0.2519685F;
            this.txtCourseOrder.Left = 2.76063F;
            this.txtCourseOrder.MultiLine = false;
            this.txtCourseOrder.Name = "txtCourseOrder";
            this.txtCourseOrder.OutputFormat = resources.GetString("txtCourseOrder.OutputFormat");
            this.txtCourseOrder.ShrinkToFit = true;
            this.txtCourseOrder.Style = "color: Black; font-size: 16pt; text-align: right; vertical-align: middle; white-s" +
    "pace: nowrap; ddo-char-set: 1; ddo-shrink-to-fit: true; ddo-wrap-mode: nowrap";
            this.txtCourseOrder.Text = "1234";
            this.txtCourseOrder.Top = 0.9830709F;
            this.txtCourseOrder.Width = 0.3759842F;
            // 
            // barcode1
            // 
            this.barcode1.CheckSumEnabled = false;
            this.barcode1.DataField = "出荷番号";
            this.barcode1.Font = new System.Drawing.Font("Courier New", 8F);
            this.barcode1.Height = 0.5513889F;
            this.barcode1.Left = 2.593701F;
            this.barcode1.Name = "barcode1";
            this.barcode1.QuietZoneBottom = 0F;
            this.barcode1.QuietZoneLeft = 0F;
            this.barcode1.QuietZoneRight = 0F;
            this.barcode1.QuietZoneTop = 0F;
            this.barcode1.Style = GrapeCity.ActiveReports.SectionReportModel.BarCodeStyle.QRCode;
            this.barcode1.SupplementDataField = "";
            this.barcode1.Text = "12345678901234567890";
            this.barcode1.Top = 1.333465F;
            this.barcode1.Width = 0.5513889F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // Report304A
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
            ((System.ComponentModel.ISupportInitialize)(this.txtChumonbi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokuisakiName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokujiKubun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyokusu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.Line line1;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.Line line5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtChumonbi;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTokuisakiName;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSyokujiKubun;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCourseName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSyokusu;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBag;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.Line line4;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCourseOrder;
        private GrapeCity.ActiveReports.SectionReportModel.Barcode barcode1;
    }
}

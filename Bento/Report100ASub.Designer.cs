namespace Emu.Bento
{
    /// <summary>
    /// Report100ASub の概要の説明です。
    /// </summary>
    partial class Report100ASub
    {
        private GrapeCity.ActiveReports.SectionReportModel.Detail detail;

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report100ASub));
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtSyohinName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtKissyokuBi = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtSuryo = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTanka = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtKingaku = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyohinName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKissyokuBi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSuryo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTanka)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKingaku)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.CanGrow = false;
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.txtSyohinName,
            this.txtKissyokuBi,
            this.txtSuryo,
            this.txtTanka,
            this.txtKingaku});
            this.detail.Height = 0.2716535F;
            this.detail.Name = "detail";
            // 
            // txtSyohinName
            // 
            this.txtSyohinName.DataField = "商品名";
            this.txtSyohinName.Height = 0.15625F;
            this.txtSyohinName.Left = 0.05833335F;
            this.txtSyohinName.Name = "txtSyohinName";
            this.txtSyohinName.Text = "商品名";
            this.txtSyohinName.Top = 0.05833335F;
            this.txtSyohinName.Width = 2.702778F;
            // 
            // txtKissyokuBi
            // 
            this.txtKissyokuBi.DataField = "喫食日";
            this.txtKissyokuBi.Height = 0.15625F;
            this.txtKissyokuBi.Left = 2.819445F;
            this.txtKissyokuBi.Name = "txtKissyokuBi";
            this.txtKissyokuBi.OutputFormat = resources.GetString("txtKissyokuBi.OutputFormat");
            this.txtKissyokuBi.Style = "text-align: center";
            this.txtKissyokuBi.Text = "喫食日";
            this.txtKissyokuBi.Top = 0.05833335F;
            this.txtKissyokuBi.Width = 0.4861112F;
            // 
            // txtSuryo
            // 
            this.txtSuryo.DataField = "数量";
            this.txtSuryo.Height = 0.15625F;
            this.txtSuryo.Left = 3.344445F;
            this.txtSuryo.Name = "txtSuryo";
            this.txtSuryo.Style = "text-align: right";
            this.txtSuryo.Text = "数量";
            this.txtSuryo.Top = 0.05833335F;
            this.txtSuryo.Width = 0.4986113F;
            // 
            // txtTanka
            // 
            this.txtTanka.DataField = "単価";
            this.txtTanka.Height = 0.15625F;
            this.txtTanka.Left = 3.908334F;
            this.txtTanka.Name = "txtTanka";
            this.txtTanka.OutputFormat = resources.GetString("txtTanka.OutputFormat");
            this.txtTanka.Style = "text-align: right";
            this.txtTanka.Text = "単価";
            this.txtTanka.Top = 0.05833335F;
            this.txtTanka.Width = 0.6541669F;
            // 
            // txtKingaku
            // 
            this.txtKingaku.DataField = "金額";
            this.txtKingaku.Height = 0.15625F;
            this.txtKingaku.Left = 4.630557F;
            this.txtKingaku.Name = "txtKingaku";
            this.txtKingaku.OutputFormat = resources.GetString("txtKingaku.OutputFormat");
            this.txtKingaku.Style = "text-align: right";
            this.txtKingaku.Text = "金額";
            this.txtKingaku.Top = 0.05833335F;
            this.txtKingaku.Width = 0.6666669F;
            // 
            // Report100ASub
            // 
            this.MasterReport = false;
            this.PageSettings.DefaultPaperSize = false;
            this.PageSettings.Margins.Bottom = 0.2519685F;
            this.PageSettings.Margins.Left = 0.2519685F;
            this.PageSettings.Margins.Right = 0.2519685F;
            this.PageSettings.Margins.Top = 0.25F;
            this.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait;
            this.PageSettings.PaperHeight = 11.69291F;
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.PageSettings.PaperWidth = 8.267716F;
            this.PrintWidth = 7.768752F;
            this.Sections.Add(this.detail);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; color: Black; fon" +
            "t-family: \"ＭＳ Ｐ明朝\"; font-size: 11pt; ddo-char-set: 128", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; font-family: \"MS UI Gothic\"; ddo-char-set: 12" +
            "8", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: inherit; font-family: \"MS UI Goth" +
            "ic\"; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtSyohinName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKissyokuBi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSuryo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTanka)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKingaku)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSyohinName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKissyokuBi;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSuryo;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTanka;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKingaku;
    }
}

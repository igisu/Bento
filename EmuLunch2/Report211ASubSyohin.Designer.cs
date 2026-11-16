namespace Emu.Bento
{
    /// <summary>
    /// Report211ASubSyohin の概要の説明です。
    /// </summary>
    partial class Report211ASubSyohin
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report211ASubSyohin));
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtSyohinName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtQuantity = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lineDetailBottom = new GrapeCity.ActiveReports.SectionReportModel.Line();
            ((System.ComponentModel.ISupportInitialize)(this.txtSyohinName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.txtSyohinName,
            this.txtQuantity,
            this.lineDetailBottom});
            this.detail.Height = 0.1968504F;
            this.detail.Name = "detail";
            // 
            // txtSyohinName
            // 
            this.txtSyohinName.CanGrow = false;
            this.txtSyohinName.DataField = "商品名";
            this.txtSyohinName.Height = 0.1574803F;
            this.txtSyohinName.Left = 0.01968504F;
            this.txtSyohinName.Name = "txtSyohinName";
            this.txtSyohinName.Style = "text-align: left; vertical-align: top; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.txtSyohinName.Text = "商品名";
            this.txtSyohinName.Top = 0.01968504F;
            this.txtSyohinName.Width = 0.5905512F;
            // 
            // txtQuantity
            // 
            this.txtQuantity.CanGrow = false;
            this.txtQuantity.DataField = "商品数量";
            this.txtQuantity.Height = 0.1574803F;
            this.txtQuantity.Left = 0.6496063F;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Style = "text-align: right; vertical-align: top; white-space: nowrap; ddo-wrap-mode: nowra" +
    "p";
            this.txtQuantity.Text = "商品数量";
            this.txtQuantity.Top = 0.01968504F;
            this.txtQuantity.Width = 0.3149606F;
            // 
            // lineDetailBottom
            // 
            this.lineDetailBottom.Height = 0F;
            this.lineDetailBottom.Left = 0F;
            this.lineDetailBottom.LineWeight = 1F;
            this.lineDetailBottom.Name = "lineDetailBottom";
            this.lineDetailBottom.Top = 0.1968504F;
            this.lineDetailBottom.Width = 0.984252F;
            this.lineDetailBottom.X1 = 0F;
            this.lineDetailBottom.X2 = 0.984252F;
            this.lineDetailBottom.Y1 = 0.1968504F;
            this.lineDetailBottom.Y2 = 0.1968504F;
            // 
            // Report211ASubSyohin
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0F;
            this.PageSettings.Margins.Left = 0F;
            this.PageSettings.Margins.Right = 0F;
            this.PageSettings.Margins.Top = 0F;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 0.984252F;
            this.Sections.Add(this.detail);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; color: Black; fon" +
            "t-family: \"ＭＳ Ｐゴシック\"; font-size: 9pt; ddo-char-set: 161", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; font-family: \"MS UI Gothic\"; ddo-char-set: 12" +
            "8", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: inherit; font-family: \"MS UI Goth" +
            "ic\"; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtSyohinName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSyohinName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailBottom;
    }
}

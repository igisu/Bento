namespace Emu.Bento
{
    /// <summary>
    /// Report211ASubBiko の概要の説明です。
    /// </summary>
    partial class Report211ASubBiko
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report211ASubBiko));
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtBiko = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtBiko)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.txtBiko});
            this.detail.Height = 0.1968504F;
            this.detail.Name = "detail";
            // 
            // txtBiko
            // 
            this.txtBiko.CanGrow = false;
            this.txtBiko.DataField = "備考";
            this.txtBiko.Height = 0.1574803F;
            this.txtBiko.Left = 0F;
            this.txtBiko.Name = "txtBiko";
            this.txtBiko.ShrinkToFit = true;
            this.txtBiko.Style = "text-align: left; vertical-align: top; white-space: nowrap; ddo-shrink-to-fit: tr" +
    "ue; ddo-wrap-mode: nowrap";
            this.txtBiko.Text = "備考";
            this.txtBiko.Top = 0.01968504F;
            this.txtBiko.Width = 3.582677F;
            // 
            // Report211ASubBiko
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0F;
            this.PageSettings.Margins.Left = 0F;
            this.PageSettings.Margins.Right = 0F;
            this.PageSettings.Margins.Top = 0F;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 3.582677F;
            this.Sections.Add(this.detail);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; color: Black; fon" +
            "t-family: \"ＭＳ Ｐゴシック\"; font-size: 9pt; ddo-char-set: 128", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; font-family: \"MS UI Gothic\"; ddo-char-set: 12" +
            "8", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: inherit; font-family: \"MS UI Goth" +
            "ic\"; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtBiko)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBiko;
    }
}

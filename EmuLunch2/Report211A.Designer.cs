namespace Emu.Bento
{
    /// <summary>
    /// Report211A の概要の説明です。
    /// </summary>
    partial class Report211A
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report211A));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.lblBiko = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMainQuantity = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMain = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblSubQuantity = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblSub = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTokuisakiName = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblOrder = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBusyoName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtHaisobi = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCourseName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTantouName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lineHeader = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblTitle = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblPublish = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.reportInfoPublish = new GrapeCity.ActiveReports.SectionReportModel.ReportInfo();
            this.lblPage = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.reportInfoPage = new GrapeCity.ActiveReports.SectionReportModel.ReportInfo();
            this.lineHeaderLeft = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderTop = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderTokuisaki = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderSub = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderSubQuantity = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderMain = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderMainQuantity = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderBiko = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderRight = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineHeaderBottom = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.subReportBiko = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.subReportMain = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.subReportSub = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.txtTokuisakiName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtOrder = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lineDetailLeft = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailTokuisaki = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailSub = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailSubQuantity = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailMain = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailMainQuantity = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailBiko = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailRight = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailBottom = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineDetailHeader = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.lblBiko)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMainQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSubQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTokuisakiName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusyoName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHaisobi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTantouName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPublish)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfoPublish)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfoPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokuisakiName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.lblBiko,
            this.lblMainQuantity,
            this.lblMain,
            this.lblSubQuantity,
            this.lblSub,
            this.lblTokuisakiName,
            this.lblOrder,
            this.txtBusyoName,
            this.txtHaisobi,
            this.txtCourseName,
            this.txtTantouName,
            this.lineHeader,
            this.lblTitle,
            this.lblPublish,
            this.reportInfoPublish,
            this.lblPage,
            this.reportInfoPage,
            this.lineHeaderLeft,
            this.lineHeaderTop,
            this.lineHeaderTokuisaki,
            this.lineHeaderSub,
            this.lineHeaderSubQuantity,
            this.lineHeaderMain,
            this.lineHeaderMainQuantity,
            this.lineHeaderBiko,
            this.lineHeaderRight,
            this.lineHeaderBottom});
            this.pageHeader.Height = 0.7480315F;
            this.pageHeader.Name = "pageHeader";
            // 
            // lblBiko
            // 
            this.lblBiko.Height = 0.1574803F;
            this.lblBiko.HyperLink = null;
            this.lblBiko.Left = 4.055118F;
            this.lblBiko.Name = "lblBiko";
            this.lblBiko.Style = "text-align: center; vertical-align: middle";
            this.lblBiko.Text = "備考";
            this.lblBiko.Top = 0.5708662F;
            this.lblBiko.Width = 3.582677F;
            // 
            // lblMainQuantity
            // 
            this.lblMainQuantity.Height = 0.1574803F;
            this.lblMainQuantity.HyperLink = null;
            this.lblMainQuantity.Left = 3.700787F;
            this.lblMainQuantity.Name = "lblMainQuantity";
            this.lblMainQuantity.Style = "text-align: center; vertical-align: middle";
            this.lblMainQuantity.Text = "数量";
            this.lblMainQuantity.Top = 0.5708662F;
            this.lblMainQuantity.Width = 0.3149606F;
            // 
            // lblMain
            // 
            this.lblMain.Height = 0.1574803F;
            this.lblMain.HyperLink = null;
            this.lblMain.Left = 3.070866F;
            this.lblMain.Name = "lblMain";
            this.lblMain.Style = "text-align: center; vertical-align: middle";
            this.lblMain.Text = "主食";
            this.lblMain.Top = 0.5708662F;
            this.lblMain.Width = 0.5905512F;
            // 
            // lblSubQuantity
            // 
            this.lblSubQuantity.Height = 0.1574803F;
            this.lblSubQuantity.HyperLink = null;
            this.lblSubQuantity.Left = 2.716536F;
            this.lblSubQuantity.Name = "lblSubQuantity";
            this.lblSubQuantity.Style = "text-align: center; vertical-align: middle";
            this.lblSubQuantity.Text = "数量";
            this.lblSubQuantity.Top = 0.5708662F;
            this.lblSubQuantity.Width = 0.3149606F;
            // 
            // lblSub
            // 
            this.lblSub.Height = 0.1574803F;
            this.lblSub.HyperLink = null;
            this.lblSub.Left = 2.086614F;
            this.lblSub.Name = "lblSub";
            this.lblSub.Style = "text-align: center; vertical-align: middle";
            this.lblSub.Text = "副食";
            this.lblSub.Top = 0.5708662F;
            this.lblSub.Width = 0.5905512F;
            // 
            // lblTokuisakiName
            // 
            this.lblTokuisakiName.Height = 0.1574803F;
            this.lblTokuisakiName.HyperLink = null;
            this.lblTokuisakiName.Left = 0.472441F;
            this.lblTokuisakiName.Name = "lblTokuisakiName";
            this.lblTokuisakiName.Style = "text-align: center; vertical-align: middle";
            this.lblTokuisakiName.Text = "得意先名";
            this.lblTokuisakiName.Top = 0.5708662F;
            this.lblTokuisakiName.Width = 1.574803F;
            // 
            // lblOrder
            // 
            this.lblOrder.Height = 0.1574803F;
            this.lblOrder.HyperLink = null;
            this.lblOrder.Left = 0.1968504F;
            this.lblOrder.Name = "lblOrder";
            this.lblOrder.Style = "text-align: center; vertical-align: middle";
            this.lblOrder.Text = "順";
            this.lblOrder.Top = 0.5708662F;
            this.lblOrder.Width = 0.2362205F;
            // 
            // txtBusyoName
            // 
            this.txtBusyoName.CanGrow = false;
            this.txtBusyoName.DataField = "部署名";
            this.txtBusyoName.Height = 0.1417323F;
            this.txtBusyoName.Left = 0.2161417F;
            this.txtBusyoName.MultiLine = false;
            this.txtBusyoName.Name = "txtBusyoName";
            this.txtBusyoName.Style = "vertical-align: middle; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtBusyoName.Text = "部署名";
            this.txtBusyoName.Top = 0.07795276F;
            this.txtBusyoName.Width = 1.583334F;
            // 
            // txtHaisobi
            // 
            this.txtHaisobi.CanGrow = false;
            this.txtHaisobi.DataField = "配送日";
            this.txtHaisobi.Height = 0.1417323F;
            this.txtHaisobi.Left = 0.2161417F;
            this.txtHaisobi.MultiLine = false;
            this.txtHaisobi.Name = "txtHaisobi";
            this.txtHaisobi.Style = "vertical-align: middle; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtHaisobi.Text = "配送日";
            this.txtHaisobi.Top = 0.2720473F;
            this.txtHaisobi.Width = 1.022878F;
            // 
            // txtCourseName
            // 
            this.txtCourseName.CanGrow = false;
            this.txtCourseName.DataField = "コース名";
            this.txtCourseName.Height = 0.1417323F;
            this.txtCourseName.Left = 1.382677F;
            this.txtCourseName.MultiLine = false;
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.Style = "vertical-align: middle; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCourseName.Text = "コース名";
            this.txtCourseName.Top = 0.2720473F;
            this.txtCourseName.Width = 1.380556F;
            // 
            // txtTantouName
            // 
            this.txtTantouName.CanGrow = false;
            this.txtTantouName.DataField = "担当者名";
            this.txtTantouName.Height = 0.1417323F;
            this.txtTantouName.Left = 2.821654F;
            this.txtTantouName.MultiLine = false;
            this.txtTantouName.Name = "txtTantouName";
            this.txtTantouName.Style = "vertical-align: middle; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtTantouName.Text = "担当者名";
            this.txtTantouName.Top = 0.2720473F;
            this.txtTantouName.Width = 1.380556F;
            // 
            // lineHeader
            // 
            this.lineHeader.Height = 0F;
            this.lineHeader.Left = 0.1771654F;
            this.lineHeader.LineWeight = 1F;
            this.lineHeader.Name = "lineHeader";
            this.lineHeader.Top = 0.411811F;
            this.lineHeader.Width = 4.060629F;
            this.lineHeader.X1 = 0.1771654F;
            this.lineHeader.X2 = 4.237795F;
            this.lineHeader.Y1 = 0.411811F;
            this.lineHeader.Y2 = 0.411811F;
            // 
            // lblTitle
            // 
            this.lblTitle.Height = 0.1751969F;
            this.lblTitle.HyperLink = null;
            this.lblTitle.Left = 2.834449F;
            this.lblTitle.MultiLine = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Style = "font-size: 12pt; text-align: center; vertical-align: middle; white-space: nowrap;" +
    " ddo-wrap-mode: nowrap";
            this.lblTitle.Text = "＊＊＊  配送明細表  ＊＊＊";
            this.lblTitle.Top = 0.01929134F;
            this.lblTitle.Width = 2.1F;
            // 
            // lblPublish
            // 
            this.lblPublish.Height = 0.1299213F;
            this.lblPublish.HyperLink = null;
            this.lblPublish.Left = 5.27441F;
            this.lblPublish.MultiLine = false;
            this.lblPublish.Name = "lblPublish";
            this.lblPublish.Style = "font-size: 9pt; text-align: left; vertical-align: middle; white-space: nowrap; dd" +
    "o-wrap-mode: nowrap";
            this.lblPublish.Text = "作成日";
            this.lblPublish.Top = 0.2720473F;
            this.lblPublish.Width = 0.388889F;
            // 
            // reportInfoPublish
            // 
            this.reportInfoPublish.CanGrow = false;
            this.reportInfoPublish.FormatString = "{RunDateTime:yyyy年M月d日}";
            this.reportInfoPublish.Height = 0.1299213F;
            this.reportInfoPublish.Left = 5.686221F;
            this.reportInfoPublish.MultiLine = false;
            this.reportInfoPublish.Name = "reportInfoPublish";
            this.reportInfoPublish.Style = "color: Black; font-size: 9pt; vertical-align: middle; white-space: nowrap; ddo-wr" +
    "ap-mode: nowrap";
            this.reportInfoPublish.SummaryGroup = "";
            this.reportInfoPublish.Top = 0.2720473F;
            this.reportInfoPublish.Width = 0.8944445F;
            // 
            // lblPage
            // 
            this.lblPage.Height = 0.1299212F;
            this.lblPage.HyperLink = null;
            this.lblPage.Left = 6.752756F;
            this.lblPage.MultiLine = false;
            this.lblPage.Name = "lblPage";
            this.lblPage.Style = "font-size: 9pt; text-align: left; vertical-align: middle; white-space: nowrap; dd" +
    "o-wrap-mode: nowrap";
            this.lblPage.Text = "PAGE";
            this.lblPage.Top = 0.2720473F;
            this.lblPage.Width = 0.3305556F;
            // 
            // reportInfoPage
            // 
            this.reportInfoPage.CanGrow = false;
            this.reportInfoPage.FormatString = "{PageNumber} / {PageCount}";
            this.reportInfoPage.Height = 0.1299212F;
            this.reportInfoPage.Left = 7.083465F;
            this.reportInfoPage.MultiLine = false;
            this.reportInfoPage.Name = "reportInfoPage";
            this.reportInfoPage.Style = "color: Black; font-size: 9pt; text-align: right; vertical-align: middle; white-sp" +
    "ace: nowrap; ddo-wrap-mode: nowrap";
            this.reportInfoPage.SummaryGroup = "";
            this.reportInfoPage.Top = 0.2720473F;
            this.reportInfoPage.Width = 0.5740157F;
            // 
            // lineHeaderLeft
            // 
            this.lineHeaderLeft.Height = 0.1968504F;
            this.lineHeaderLeft.Left = 0.1771654F;
            this.lineHeaderLeft.LineWeight = 1F;
            this.lineHeaderLeft.Name = "lineHeaderLeft";
            this.lineHeaderLeft.Top = 0.5511811F;
            this.lineHeaderLeft.Width = 0F;
            this.lineHeaderLeft.X1 = 0.1771654F;
            this.lineHeaderLeft.X2 = 0.1771654F;
            this.lineHeaderLeft.Y1 = 0.5511811F;
            this.lineHeaderLeft.Y2 = 0.7480315F;
            // 
            // lineHeaderTop
            // 
            this.lineHeaderTop.Height = 0F;
            this.lineHeaderTop.Left = 0.1771654F;
            this.lineHeaderTop.LineWeight = 1F;
            this.lineHeaderTop.Name = "lineHeaderTop";
            this.lineHeaderTop.Top = 0.5511811F;
            this.lineHeaderTop.Width = 7.480316F;
            this.lineHeaderTop.X1 = 0.1771654F;
            this.lineHeaderTop.X2 = 7.657481F;
            this.lineHeaderTop.Y1 = 0.5511811F;
            this.lineHeaderTop.Y2 = 0.5511811F;
            // 
            // lineHeaderTokuisaki
            // 
            this.lineHeaderTokuisaki.Height = 0.1968504F;
            this.lineHeaderTokuisaki.Left = 0.4527559F;
            this.lineHeaderTokuisaki.LineWeight = 1F;
            this.lineHeaderTokuisaki.Name = "lineHeaderTokuisaki";
            this.lineHeaderTokuisaki.Top = 0.5511811F;
            this.lineHeaderTokuisaki.Width = 0F;
            this.lineHeaderTokuisaki.X1 = 0.4527559F;
            this.lineHeaderTokuisaki.X2 = 0.4527559F;
            this.lineHeaderTokuisaki.Y1 = 0.5511811F;
            this.lineHeaderTokuisaki.Y2 = 0.7480315F;
            // 
            // lineHeaderSub
            // 
            this.lineHeaderSub.Height = 0.1968502F;
            this.lineHeaderSub.Left = 2.066929F;
            this.lineHeaderSub.LineWeight = 1F;
            this.lineHeaderSub.Name = "lineHeaderSub";
            this.lineHeaderSub.Top = 0.5511811F;
            this.lineHeaderSub.Width = 0F;
            this.lineHeaderSub.X1 = 2.066929F;
            this.lineHeaderSub.X2 = 2.066929F;
            this.lineHeaderSub.Y1 = 0.5511811F;
            this.lineHeaderSub.Y2 = 0.7480313F;
            // 
            // lineHeaderSubQuantity
            // 
            this.lineHeaderSubQuantity.Height = 0.1968502F;
            this.lineHeaderSubQuantity.Left = 2.69685F;
            this.lineHeaderSubQuantity.LineWeight = 1F;
            this.lineHeaderSubQuantity.Name = "lineHeaderSubQuantity";
            this.lineHeaderSubQuantity.Top = 0.5511811F;
            this.lineHeaderSubQuantity.Width = 0F;
            this.lineHeaderSubQuantity.X1 = 2.69685F;
            this.lineHeaderSubQuantity.X2 = 2.69685F;
            this.lineHeaderSubQuantity.Y1 = 0.5511811F;
            this.lineHeaderSubQuantity.Y2 = 0.7480313F;
            // 
            // lineHeaderMain
            // 
            this.lineHeaderMain.Height = 0.1968502F;
            this.lineHeaderMain.Left = 3.051181F;
            this.lineHeaderMain.LineWeight = 1F;
            this.lineHeaderMain.Name = "lineHeaderMain";
            this.lineHeaderMain.Top = 0.5511811F;
            this.lineHeaderMain.Width = 0F;
            this.lineHeaderMain.X1 = 3.051181F;
            this.lineHeaderMain.X2 = 3.051181F;
            this.lineHeaderMain.Y1 = 0.5511811F;
            this.lineHeaderMain.Y2 = 0.7480313F;
            // 
            // lineHeaderMainQuantity
            // 
            this.lineHeaderMainQuantity.Height = 0.1968502F;
            this.lineHeaderMainQuantity.Left = 3.681103F;
            this.lineHeaderMainQuantity.LineWeight = 1F;
            this.lineHeaderMainQuantity.Name = "lineHeaderMainQuantity";
            this.lineHeaderMainQuantity.Top = 0.5511811F;
            this.lineHeaderMainQuantity.Width = 0F;
            this.lineHeaderMainQuantity.X1 = 3.681103F;
            this.lineHeaderMainQuantity.X2 = 3.681103F;
            this.lineHeaderMainQuantity.Y1 = 0.5511811F;
            this.lineHeaderMainQuantity.Y2 = 0.7480313F;
            // 
            // lineHeaderBiko
            // 
            this.lineHeaderBiko.Height = 0.1968502F;
            this.lineHeaderBiko.Left = 4.035433F;
            this.lineHeaderBiko.LineWeight = 1F;
            this.lineHeaderBiko.Name = "lineHeaderBiko";
            this.lineHeaderBiko.Top = 0.5511811F;
            this.lineHeaderBiko.Width = 0F;
            this.lineHeaderBiko.X1 = 4.035433F;
            this.lineHeaderBiko.X2 = 4.035433F;
            this.lineHeaderBiko.Y1 = 0.5511811F;
            this.lineHeaderBiko.Y2 = 0.7480313F;
            // 
            // lineHeaderRight
            // 
            this.lineHeaderRight.Height = 0.1968502F;
            this.lineHeaderRight.Left = 7.657481F;
            this.lineHeaderRight.LineWeight = 1F;
            this.lineHeaderRight.Name = "lineHeaderRight";
            this.lineHeaderRight.Top = 0.5511811F;
            this.lineHeaderRight.Width = 0F;
            this.lineHeaderRight.X1 = 7.657481F;
            this.lineHeaderRight.X2 = 7.657481F;
            this.lineHeaderRight.Y1 = 0.5511811F;
            this.lineHeaderRight.Y2 = 0.7480313F;
            // 
            // lineHeaderBottom
            // 
            this.lineHeaderBottom.Height = 0F;
            this.lineHeaderBottom.Left = 0.1771654F;
            this.lineHeaderBottom.LineWeight = 1F;
            this.lineHeaderBottom.Name = "lineHeaderBottom";
            this.lineHeaderBottom.Top = 0.7480315F;
            this.lineHeaderBottom.Width = 7.480316F;
            this.lineHeaderBottom.X1 = 0.1771654F;
            this.lineHeaderBottom.X2 = 7.657481F;
            this.lineHeaderBottom.Y1 = 0.7480315F;
            this.lineHeaderBottom.Y2 = 0.7480315F;
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.subReportBiko,
            this.subReportMain,
            this.subReportSub,
            this.txtTokuisakiName,
            this.txtOrder,
            this.lineDetailLeft,
            this.lineDetailTokuisaki,
            this.lineDetailSub,
            this.lineDetailSubQuantity,
            this.lineDetailMain,
            this.lineDetailMainQuantity,
            this.lineDetailBiko,
            this.lineDetailRight,
            this.lineDetailBottom,
            this.lineDetailHeader});
            this.detail.Height = 0.1968504F;
            this.detail.KeepTogether = true;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            this.detail.BeforePrint += new System.EventHandler(this.detail_BeforePrint);
            // 
            // subReportBiko
            // 
            this.subReportBiko.CloseBorder = false;
            this.subReportBiko.Height = 0.1968504F;
            this.subReportBiko.Left = 4.055118F;
            this.subReportBiko.Name = "subReportBiko";
            this.subReportBiko.Report = null;
            this.subReportBiko.ReportName = "subReportBiko";
            this.subReportBiko.Top = 0F;
            this.subReportBiko.Width = 3.582677F;
            // 
            // subReportMain
            // 
            this.subReportMain.CloseBorder = false;
            this.subReportMain.Height = 0.1968504F;
            this.subReportMain.Left = 3.051181F;
            this.subReportMain.Name = "subReportMain";
            this.subReportMain.Report = null;
            this.subReportMain.ReportName = "subReportMain";
            this.subReportMain.Top = 0F;
            this.subReportMain.Width = 0.984252F;
            // 
            // subReportSub
            // 
            this.subReportSub.CloseBorder = false;
            this.subReportSub.Height = 0.1968504F;
            this.subReportSub.Left = 2.066929F;
            this.subReportSub.Name = "subReportSub";
            this.subReportSub.Report = null;
            this.subReportSub.ReportName = "subReportSub";
            this.subReportSub.Top = 0F;
            this.subReportSub.Width = 0.984252F;
            // 
            // txtTokuisakiName
            // 
            this.txtTokuisakiName.CanGrow = false;
            this.txtTokuisakiName.DataField = "得意先名";
            this.txtTokuisakiName.Height = 0.1574803F;
            this.txtTokuisakiName.Left = 0.472441F;
            this.txtTokuisakiName.Name = "txtTokuisakiName";
            this.txtTokuisakiName.Style = "text-align: left; vertical-align: top; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.txtTokuisakiName.Text = "得意先名";
            this.txtTokuisakiName.Top = 0.01968504F;
            this.txtTokuisakiName.Width = 1.574803F;
            // 
            // txtOrder
            // 
            this.txtOrder.CanGrow = false;
            this.txtOrder.DataField = "コース順";
            this.txtOrder.Height = 0.1574803F;
            this.txtOrder.Left = 0.1968504F;
            this.txtOrder.Name = "txtOrder";
            this.txtOrder.Style = "text-align: right; vertical-align: top; white-space: nowrap; ddo-wrap-mode: nowra" +
    "p";
            this.txtOrder.Text = "コース順";
            this.txtOrder.Top = 0.01968504F;
            this.txtOrder.Width = 0.2362205F;
            // 
            // lineDetailLeft
            // 
            this.lineDetailLeft.AnchorBottom = true;
            this.lineDetailLeft.Height = 0.1968504F;
            this.lineDetailLeft.Left = 0.1771654F;
            this.lineDetailLeft.LineWeight = 1F;
            this.lineDetailLeft.Name = "lineDetailLeft";
            this.lineDetailLeft.Top = 0F;
            this.lineDetailLeft.Width = 0F;
            this.lineDetailLeft.X1 = 0.1771654F;
            this.lineDetailLeft.X2 = 0.1771654F;
            this.lineDetailLeft.Y1 = 0F;
            this.lineDetailLeft.Y2 = 0.1968504F;
            // 
            // lineDetailTokuisaki
            // 
            this.lineDetailTokuisaki.AnchorBottom = true;
            this.lineDetailTokuisaki.Height = 0.1968504F;
            this.lineDetailTokuisaki.Left = 0.4527559F;
            this.lineDetailTokuisaki.LineWeight = 1F;
            this.lineDetailTokuisaki.Name = "lineDetailTokuisaki";
            this.lineDetailTokuisaki.Top = 0F;
            this.lineDetailTokuisaki.Width = 0F;
            this.lineDetailTokuisaki.X1 = 0.4527559F;
            this.lineDetailTokuisaki.X2 = 0.4527559F;
            this.lineDetailTokuisaki.Y1 = 0F;
            this.lineDetailTokuisaki.Y2 = 0.1968504F;
            // 
            // lineDetailSub
            // 
            this.lineDetailSub.AnchorBottom = true;
            this.lineDetailSub.Height = 0.1968504F;
            this.lineDetailSub.Left = 2.066929F;
            this.lineDetailSub.LineWeight = 1F;
            this.lineDetailSub.Name = "lineDetailSub";
            this.lineDetailSub.Top = 0F;
            this.lineDetailSub.Width = 0F;
            this.lineDetailSub.X1 = 2.066929F;
            this.lineDetailSub.X2 = 2.066929F;
            this.lineDetailSub.Y1 = 0F;
            this.lineDetailSub.Y2 = 0.1968504F;
            // 
            // lineDetailSubQuantity
            // 
            this.lineDetailSubQuantity.AnchorBottom = true;
            this.lineDetailSubQuantity.Height = 0.1968504F;
            this.lineDetailSubQuantity.Left = 2.69685F;
            this.lineDetailSubQuantity.LineWeight = 1F;
            this.lineDetailSubQuantity.Name = "lineDetailSubQuantity";
            this.lineDetailSubQuantity.Top = 0F;
            this.lineDetailSubQuantity.Width = 0F;
            this.lineDetailSubQuantity.X1 = 2.69685F;
            this.lineDetailSubQuantity.X2 = 2.69685F;
            this.lineDetailSubQuantity.Y1 = 0F;
            this.lineDetailSubQuantity.Y2 = 0.1968504F;
            // 
            // lineDetailMain
            // 
            this.lineDetailMain.AnchorBottom = true;
            this.lineDetailMain.Height = 0.1968504F;
            this.lineDetailMain.Left = 3.051181F;
            this.lineDetailMain.LineWeight = 1F;
            this.lineDetailMain.Name = "lineDetailMain";
            this.lineDetailMain.Top = 0F;
            this.lineDetailMain.Width = 0F;
            this.lineDetailMain.X1 = 3.051181F;
            this.lineDetailMain.X2 = 3.051181F;
            this.lineDetailMain.Y1 = 0F;
            this.lineDetailMain.Y2 = 0.1968504F;
            // 
            // lineDetailMainQuantity
            // 
            this.lineDetailMainQuantity.AnchorBottom = true;
            this.lineDetailMainQuantity.Height = 0.1968504F;
            this.lineDetailMainQuantity.Left = 3.681103F;
            this.lineDetailMainQuantity.LineWeight = 1F;
            this.lineDetailMainQuantity.Name = "lineDetailMainQuantity";
            this.lineDetailMainQuantity.Top = 0F;
            this.lineDetailMainQuantity.Width = 0F;
            this.lineDetailMainQuantity.X1 = 3.681103F;
            this.lineDetailMainQuantity.X2 = 3.681103F;
            this.lineDetailMainQuantity.Y1 = 0F;
            this.lineDetailMainQuantity.Y2 = 0.1968504F;
            // 
            // lineDetailBiko
            // 
            this.lineDetailBiko.AnchorBottom = true;
            this.lineDetailBiko.Height = 0.1968504F;
            this.lineDetailBiko.Left = 4.035433F;
            this.lineDetailBiko.LineWeight = 1F;
            this.lineDetailBiko.Name = "lineDetailBiko";
            this.lineDetailBiko.Top = 0F;
            this.lineDetailBiko.Width = 0F;
            this.lineDetailBiko.X1 = 4.035433F;
            this.lineDetailBiko.X2 = 4.035433F;
            this.lineDetailBiko.Y1 = 0F;
            this.lineDetailBiko.Y2 = 0.1968504F;
            // 
            // lineDetailRight
            // 
            this.lineDetailRight.AnchorBottom = true;
            this.lineDetailRight.Height = 0.1968504F;
            this.lineDetailRight.Left = 7.657481F;
            this.lineDetailRight.LineWeight = 1F;
            this.lineDetailRight.Name = "lineDetailRight";
            this.lineDetailRight.Top = 0F;
            this.lineDetailRight.Width = 0F;
            this.lineDetailRight.X1 = 7.657481F;
            this.lineDetailRight.X2 = 7.657481F;
            this.lineDetailRight.Y1 = 0F;
            this.lineDetailRight.Y2 = 0.1968504F;
            // 
            // lineDetailBottom
            // 
            this.lineDetailBottom.Height = 0F;
            this.lineDetailBottom.Left = 0.1771654F;
            this.lineDetailBottom.LineWeight = 1F;
            this.lineDetailBottom.Name = "lineDetailBottom";
            this.lineDetailBottom.Top = 0.1968504F;
            this.lineDetailBottom.Width = 7.480316F;
            this.lineDetailBottom.X1 = 0.1771654F;
            this.lineDetailBottom.X2 = 7.657481F;
            this.lineDetailBottom.Y1 = 0.1968504F;
            this.lineDetailBottom.Y2 = 0.1968504F;
            // 
            // lineDetailHeader
            // 
            this.lineDetailHeader.Height = 0F;
            this.lineDetailHeader.Left = 0.1771654F;
            this.lineDetailHeader.LineWeight = 1F;
            this.lineDetailHeader.Name = "lineDetailHeader";
            this.lineDetailHeader.Top = 0.007874016F;
            this.lineDetailHeader.Width = 7.480316F;
            this.lineDetailHeader.X1 = 0.1771654F;
            this.lineDetailHeader.X2 = 7.657481F;
            this.lineDetailHeader.Y1 = 0.007874016F;
            this.lineDetailHeader.Y2 = 0.007874016F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // Report211A
            // 
            this.MasterReport = false;
            this.PageSettings.DefaultPaperSize = false;
            this.PageSettings.Margins.Bottom = 0.3543307F;
            this.PageSettings.Margins.Left = 0.2519685F;
            this.PageSettings.Margins.Right = 0.2519685F;
            this.PageSettings.Margins.Top = 0.4370079F;
            this.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait;
            this.PageSettings.PaperHeight = 11.69291F;
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.PageSettings.PaperWidth = 8.267716F;
            this.PrintWidth = 7.768898F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; color: Black; fon" +
            "t-family: \"ＭＳ Ｐゴシック\"; ddo-char-set: 128; font-size: 9pt", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; font-family: \"MS UI Gothic\"; ddo-char-set: 12" +
            "8", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: inherit; font-family: \"MS UI Goth" +
            "ic\"; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            this.FetchData += new GrapeCity.ActiveReports.SectionReport.FetchEventHandler(this.Report211A_FetchData);
            this.ReportStart += new System.EventHandler(this.Report211A_ReportStart);
            ((System.ComponentModel.ISupportInitialize)(this.lblBiko)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMainQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSubQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTokuisakiName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusyoName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHaisobi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTantouName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPublish)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfoPublish)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfoPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTokuisakiName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBusyoName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtHaisobi;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCourseName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTantouName;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeader;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTitle;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblPublish;
        private GrapeCity.ActiveReports.SectionReportModel.ReportInfo reportInfoPublish;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblPage;
        private GrapeCity.ActiveReports.SectionReportModel.ReportInfo reportInfoPage;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderLeft;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderTop;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderTokuisaki;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderSub;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderSubQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderMain;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderMainQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderBiko;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderRight;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblOrder;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTokuisakiName;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblSub;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblSubQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMain;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMainQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBiko;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHeaderBottom;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailLeft;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailTokuisaki;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailSub;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailSubQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailMain;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailMainQuantity;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailBiko;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailRight;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtOrder;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTokuisakiName;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportSub;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportMain;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportBiko;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailBottom;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineDetailHeader;
    }
}

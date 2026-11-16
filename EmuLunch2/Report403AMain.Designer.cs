namespace Emu.Bento
{
    /// <summary>
    /// Report403AMain の概要の説明です。
    /// </summary>
    partial class Report403AMain
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Report403AMain));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.lblTitle = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.shapeHizuke = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.shapeBusyo = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.shapePublish = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineHizukeBusyoTate = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineBusyoTate = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineSakuseibiYoko = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineCourseYoko = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineSakuseibiTate = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePageTate1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePageTate2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineCourseTate = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblHizuke = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtSeisanbi = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblBusyo = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBusyoCD = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtBusyoName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblPublish = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtPublish = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblPage = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.reportInfoPage = new GrapeCity.ActiveReports.SectionReportModel.ReportInfo();
            this.lblCourse = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTantosya = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtCourseCD = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCourseName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTantosyaCD = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTantosyaName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.subReportMeisaiPay = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.subReportMochidashi = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.shapeTicket = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineTicket = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTicketMochidashi = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTicketMochiKaeri = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTicketUriMai = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTicketUriKin = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblTicketSeisan = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTicket = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTicketMochidashi = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTicketMochiKaeri = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTicketUriMai = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTicketUriKin = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTicketMochidashi = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTicketMochiKaeri = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTicketUriMai = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTicketUriKin = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.subReportMeisaiGenkin = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.lineGenkinLeft = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineGenkinNebiki = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineGenkinUrikake = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineGenkinTicket = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineGenkinTotal = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineGenkinRight = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineGenkinUnder = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblGenkinTotal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblGenkinNebiki = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtGenkinUri = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtGenkinNebiki = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtGenkinUrikake = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtGenkinTicket = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtGenkinTotal = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.subReportMeisaiKakeuri = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.lineKakeuriLeft = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriNebiki = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriMisyukin = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriTicket = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriTotal = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriRight = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriUnder = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblKakeuriTotal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblKakeuriNebiki = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtKakeuriUri = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtKakeuriNebiki = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtKakeuriMisyukin = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtKakeuriTotal = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.shapeTotal = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineTotalYoko1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTotalYoko2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTotalNebiki = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTotalTicket = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTotalBarclay = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineTotalTotal = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblTotalUriKin = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTotalUriKinFormula = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTotalUriKin = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblTotalNebiki = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTotalNebikiFormula = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTotalNebiki = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblTotalTicket = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTotalTicket = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblTotalBarclay = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTotalBarclay = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblTotalTotal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTotalTotal = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblKinsyuTable = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.shapeBill = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineBill5k = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineBill2k = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineBill1k = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblBill10k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblBill5k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblBill2k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblBill1k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBill10k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtBill5k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtBill2k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtBill1k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.shapeCoin = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineCoin100 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineCoin50 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineCoin10 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineCoin5 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineCoin1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblCoin500 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtCoin500 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblCoin100 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblCoin50 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblCoin10 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblCoin5 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblCoin1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtCoin100 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCoin50 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCoin10 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCoin5 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtCoin1 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.shapeKogitte = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineKogitteTate = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineRyosyusyo = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKinsyuTotal = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblKogitte = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblRyosyusyo = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblKinsyuTotal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtKogitte = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtRyosyusyo = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtKinsyuTotal = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lineKajyokinUnder = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKajyokinRight = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblKajokin = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtKajokin = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.shapeTicketFooter = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.lineTicketFooterTate = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblTicketFooterTitle = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblTicketFooterKake = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTicketMai = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblTicketMai = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblBonusFooterKake = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBonusMai = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblBonusMai = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblBarclayFooterTitle = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblBV300Kake = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBV300Mai = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblBV300Equal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBV300Kingaku = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblBV200Kake = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBV200Mai = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblBV200Equal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtBV200Kingaku = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lineGenkinUpper = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lineKakeuriUpper = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.lblMai10k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai5k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai2k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai1k = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtAmount10k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount5k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount2k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount1k = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblMai500 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai100 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai50 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai10 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai5 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblMai1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtAmount500 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount100 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount50 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount10 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount5 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtAmount1 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblPayTotal = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtPayUri = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblPayNebiki = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtPayNebiki = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPayTotal = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.linePayUnder = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayLeft = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayNebiki = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayMisyukin = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayTicket = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayTotal = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayRight = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.linePayUpper = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtKaisyuPay = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPayTicket = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblHizuke)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeisanbi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBusyo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusyoCD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusyoName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPublish)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPublish)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfoPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCourse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTantosya)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseCD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTantosyaCD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTantosyaName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketSeisan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketMochidashi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketMochiKaeri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketUriMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketUriKin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketMochidashi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketMochiKaeri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketUriMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketUriKin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblGenkinTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblGenkinNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinUri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinUrikake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinTicket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKakeuriTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKakeuriNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriUri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriMisyukin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalUriKin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalUriKinFormula)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalUriKin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalNebikiFormula)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalTicket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalTicket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalBarclay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalBarclay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKinsyuTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill10k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill5k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill2k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill1k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill10k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill5k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill2k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill1k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin500)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin500)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin100)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin100)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKogitte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblRyosyusyo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKinsyuTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKogitte)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRyosyusyo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKinsyuTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKajokin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKajokin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketFooterTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketFooterKake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBonusFooterKake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBonusMai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBarclayFooterTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV300Kake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV300Mai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV300Equal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV300Kingaku)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV200Kake)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV200Mai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV200Equal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV200Kingaku)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai10k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai5k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai2k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai1k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount10k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount5k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount2k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount1k)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai500)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai100)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount500)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount100)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount50)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPayTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayUri)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPayNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayNebiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKaisyuPay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayTicket)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.lblTitle,
            this.shapeHizuke,
            this.shapeBusyo,
            this.shapePublish,
            this.lineHizukeBusyoTate,
            this.lineBusyoTate,
            this.lineSakuseibiYoko,
            this.lineCourseYoko,
            this.lineSakuseibiTate,
            this.linePageTate1,
            this.linePageTate2,
            this.lineCourseTate,
            this.lblHizuke,
            this.txtSeisanbi,
            this.lblBusyo,
            this.txtBusyoCD,
            this.txtBusyoName,
            this.lblPublish,
            this.txtPublish,
            this.lblPage,
            this.reportInfoPage,
            this.lblCourse,
            this.lblTantosya,
            this.txtCourseCD,
            this.txtCourseName,
            this.txtTantosyaCD,
            this.txtTantosyaName});
            this.pageHeader.Height = 0.927778F;
            this.pageHeader.Name = "pageHeader";
            // 
            // lblTitle
            // 
            this.lblTitle.Height = 0.1889764F;
            this.lblTitle.HyperLink = null;
            this.lblTitle.Left = 2.814584F;
            this.lblTitle.MultiLine = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Style = "font-size: 12pt; text-align: center; white-space: nowrap; ddo-char-set: 1; ddo-wr" +
    "ap-mode: nowrap";
            this.lblTitle.Text = "＊＊＊  売上明細表  ＊＊＊";
            this.lblTitle.Top = 0.05833335F;
            this.lblTitle.Width = 2.138889F;
            // 
            // shapeHizuke
            // 
            this.shapeHizuke.Height = 0.2090278F;
            this.shapeHizuke.Left = 0.2952756F;
            this.shapeHizuke.Name = "shapeHizuke";
            this.shapeHizuke.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeHizuke.Top = 0.5104168F;
            this.shapeHizuke.Width = 1.477778F;
            // 
            // shapeBusyo
            // 
            this.shapeBusyo.Height = 0.2090278F;
            this.shapeBusyo.Left = 0.2952756F;
            this.shapeBusyo.Name = "shapeBusyo";
            this.shapeBusyo.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeBusyo.Top = 0.7118057F;
            this.shapeBusyo.Width = 2.011806F;
            // 
            // shapePublish
            // 
            this.shapePublish.Height = 0.620139F;
            this.shapePublish.Left = 4.900001F;
            this.shapePublish.Name = "shapePublish";
            this.shapePublish.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapePublish.Top = 0.3000001F;
            this.shapePublish.Width = 2.72014F;
            // 
            // lineHizukeBusyoTate
            // 
            this.lineHizukeBusyoTate.Height = 0.4104168F;
            this.lineHizukeBusyoTate.Left = 0.6562502F;
            this.lineHizukeBusyoTate.LineWeight = 1F;
            this.lineHizukeBusyoTate.Name = "lineHizukeBusyoTate";
            this.lineHizukeBusyoTate.Top = 0.5104168F;
            this.lineHizukeBusyoTate.Width = 0F;
            this.lineHizukeBusyoTate.X1 = 0.6562502F;
            this.lineHizukeBusyoTate.X2 = 0.6562502F;
            this.lineHizukeBusyoTate.Y1 = 0.5104168F;
            this.lineHizukeBusyoTate.Y2 = 0.9208336F;
            // 
            // lineBusyoTate
            // 
            this.lineBusyoTate.Height = 0.2090279F;
            this.lineBusyoTate.Left = 1.036806F;
            this.lineBusyoTate.LineWeight = 1F;
            this.lineBusyoTate.Name = "lineBusyoTate";
            this.lineBusyoTate.Top = 0.7118057F;
            this.lineBusyoTate.Width = 0F;
            this.lineBusyoTate.X1 = 1.036806F;
            this.lineBusyoTate.X2 = 1.036806F;
            this.lineBusyoTate.Y1 = 0.7118057F;
            this.lineBusyoTate.Y2 = 0.9208336F;
            // 
            // lineSakuseibiYoko
            // 
            this.lineSakuseibiYoko.Height = 0F;
            this.lineSakuseibiYoko.Left = 4.900001F;
            this.lineSakuseibiYoko.LineWeight = 1F;
            this.lineSakuseibiYoko.Name = "lineSakuseibiYoko";
            this.lineSakuseibiYoko.Top = 0.5055557F;
            this.lineSakuseibiYoko.Width = 2.72014F;
            this.lineSakuseibiYoko.X1 = 4.900001F;
            this.lineSakuseibiYoko.X2 = 7.620141F;
            this.lineSakuseibiYoko.Y1 = 0.5055557F;
            this.lineSakuseibiYoko.Y2 = 0.5055557F;
            // 
            // lineCourseYoko
            // 
            this.lineCourseYoko.Height = 0F;
            this.lineCourseYoko.Left = 4.9F;
            this.lineCourseYoko.LineWeight = 1F;
            this.lineCourseYoko.Name = "lineCourseYoko";
            this.lineCourseYoko.Top = 0.7194446F;
            this.lineCourseYoko.Width = 2.720139F;
            this.lineCourseYoko.X1 = 4.9F;
            this.lineCourseYoko.X2 = 7.620139F;
            this.lineCourseYoko.Y1 = 0.7194446F;
            this.lineCourseYoko.Y2 = 0.7194446F;
            // 
            // lineSakuseibiTate
            // 
            this.lineSakuseibiTate.Height = 0.620139F;
            this.lineSakuseibiTate.Left = 5.386112F;
            this.lineSakuseibiTate.LineWeight = 1F;
            this.lineSakuseibiTate.Name = "lineSakuseibiTate";
            this.lineSakuseibiTate.Top = 0.3000001F;
            this.lineSakuseibiTate.Width = 0F;
            this.lineSakuseibiTate.X1 = 5.386112F;
            this.lineSakuseibiTate.X2 = 5.386112F;
            this.lineSakuseibiTate.Y1 = 0.3000001F;
            this.lineSakuseibiTate.Y2 = 0.9201391F;
            // 
            // linePageTate1
            // 
            this.linePageTate1.Height = 0.2055118F;
            this.linePageTate1.Left = 6.475002F;
            this.linePageTate1.LineWeight = 1F;
            this.linePageTate1.Name = "linePageTate1";
            this.linePageTate1.Top = 0.3F;
            this.linePageTate1.Width = 0F;
            this.linePageTate1.X1 = 6.475002F;
            this.linePageTate1.X2 = 6.475002F;
            this.linePageTate1.Y1 = 0.3F;
            this.linePageTate1.Y2 = 0.5055118F;
            // 
            // linePageTate2
            // 
            this.linePageTate2.Height = 0.2055118F;
            this.linePageTate2.Left = 6.961113F;
            this.linePageTate2.LineWeight = 1F;
            this.linePageTate2.Name = "linePageTate2";
            this.linePageTate2.Top = 0.3F;
            this.linePageTate2.Width = 0F;
            this.linePageTate2.X1 = 6.961113F;
            this.linePageTate2.X2 = 6.961113F;
            this.linePageTate2.Y1 = 0.3F;
            this.linePageTate2.Y2 = 0.5055118F;
            // 
            // lineCourseTate
            // 
            this.lineCourseTate.Height = 0.414523F;
            this.lineCourseTate.Left = 5.911807F;
            this.lineCourseTate.LineWeight = 1F;
            this.lineCourseTate.Name = "lineCourseTate";
            this.lineCourseTate.Top = 0.5055557F;
            this.lineCourseTate.Width = 0F;
            this.lineCourseTate.X1 = 5.911807F;
            this.lineCourseTate.X2 = 5.911807F;
            this.lineCourseTate.Y1 = 0.5055557F;
            this.lineCourseTate.Y2 = 0.9200787F;
            // 
            // lblHizuke
            // 
            this.lblHizuke.Height = 0.1496063F;
            this.lblHizuke.HyperLink = null;
            this.lblHizuke.Left = 0.3111112F;
            this.lblHizuke.MultiLine = false;
            this.lblHizuke.Name = "lblHizuke";
            this.lblHizuke.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblHizuke.Text = "日付";
            this.lblHizuke.Top = 0.5393701F;
            this.lblHizuke.Width = 0.3305556F;
            // 
            // txtSeisanbi
            // 
            this.txtSeisanbi.CanGrow = false;
            this.txtSeisanbi.DataField = "精算日";
            this.txtSeisanbi.Height = 0.1496063F;
            this.txtSeisanbi.Left = 0.6805557F;
            this.txtSeisanbi.MultiLine = false;
            this.txtSeisanbi.Name = "txtSeisanbi";
            this.txtSeisanbi.OutputFormat = resources.GetString("txtSeisanbi.OutputFormat");
            this.txtSeisanbi.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; white-space: nowrap; ddo-wrap-mode: nowra" +
    "p";
            this.txtSeisanbi.Text = "精算日";
            this.txtSeisanbi.Top = 0.5393701F;
            this.txtSeisanbi.Width = 1.011111F;
            // 
            // lblBusyo
            // 
            this.lblBusyo.Height = 0.1496063F;
            this.lblBusyo.HyperLink = null;
            this.lblBusyo.Left = 0.3110236F;
            this.lblBusyo.MultiLine = false;
            this.lblBusyo.Name = "lblBusyo";
            this.lblBusyo.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblBusyo.Text = "部署";
            this.lblBusyo.Top = 0.7409449F;
            this.lblBusyo.Width = 0.3305556F;
            // 
            // txtBusyoCD
            // 
            this.txtBusyoCD.CanGrow = false;
            this.txtBusyoCD.DataField = "部署ＣＤ";
            this.txtBusyoCD.Height = 0.1496063F;
            this.txtBusyoCD.Left = 0.7000002F;
            this.txtBusyoCD.MultiLine = false;
            this.txtBusyoCD.Name = "txtBusyoCD";
            this.txtBusyoCD.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; white-space: nowrap; ddo-wrap-mode: nowra" +
    "p";
            this.txtBusyoCD.Text = "部署ＣＤ";
            this.txtBusyoCD.Top = 0.7409449F;
            this.txtBusyoCD.Width = 0.3111112F;
            // 
            // txtBusyoName
            // 
            this.txtBusyoName.CanGrow = false;
            this.txtBusyoName.DataField = "部署名";
            this.txtBusyoName.Height = 0.1496063F;
            this.txtBusyoName.Left = 1.069445F;
            this.txtBusyoName.MultiLine = false;
            this.txtBusyoName.Name = "txtBusyoName";
            this.txtBusyoName.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; white-space: nowrap; ddo-wrap-mode: nowra" +
    "p";
            this.txtBusyoName.Text = "部署名";
            this.txtBusyoName.Top = 0.7409449F;
            this.txtBusyoName.Width = 1.186111F;
            // 
            // lblPublish
            // 
            this.lblPublish.Height = 0.1496063F;
            this.lblPublish.HyperLink = null;
            this.lblPublish.Left = 4.93889F;
            this.lblPublish.MultiLine = false;
            this.lblPublish.Name = "lblPublish";
            this.lblPublish.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblPublish.Text = "作成日";
            this.lblPublish.Top = 0.3291339F;
            this.lblPublish.Width = 0.4330709F;
            // 
            // txtPublish
            // 
            this.txtPublish.CanGrow = false;
            this.txtPublish.DataField = "修正日";
            this.txtPublish.Height = 0.1496063F;
            this.txtPublish.Left = 5.425001F;
            this.txtPublish.MultiLine = false;
            this.txtPublish.Name = "txtPublish";
            this.txtPublish.OutputFormat = resources.GetString("txtPublish.OutputFormat");
            this.txtPublish.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; white-space: nowrap; ddo-wrap-mode: nowra" +
    "p";
            this.txtPublish.Text = "修正日";
            this.txtPublish.Top = 0.3291339F;
            this.txtPublish.Width = 1.011111F;
            // 
            // lblPage
            // 
            this.lblPage.Height = 0.1496063F;
            this.lblPage.HyperLink = null;
            this.lblPage.Left = 6.513891F;
            this.lblPage.MultiLine = false;
            this.lblPage.Name = "lblPage";
            this.lblPage.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblPage.Text = "PAGE";
            this.lblPage.Top = 0.3291339F;
            this.lblPage.Width = 0.4083335F;
            // 
            // reportInfoPage
            // 
            this.reportInfoPage.CanGrow = false;
            this.reportInfoPage.FormatString = "{PageNumber}";
            this.reportInfoPage.Height = 0.1496063F;
            this.reportInfoPage.Left = 7.000002F;
            this.reportInfoPage.MultiLine = false;
            this.reportInfoPage.Name = "reportInfoPage";
            this.reportInfoPage.Style = "color: Black; font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-sp" +
    "ace: nowrap; ddo-wrap-mode: nowrap";
            this.reportInfoPage.SummaryGroup = "";
            this.reportInfoPage.Top = 0.3291339F;
            this.reportInfoPage.Width = 0.5875002F;
            // 
            // lblCourse
            // 
            this.lblCourse.Height = 0.1496063F;
            this.lblCourse.HyperLink = null;
            this.lblCourse.Left = 4.938976F;
            this.lblCourse.MultiLine = false;
            this.lblCourse.Name = "lblCourse";
            this.lblCourse.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblCourse.Text = "コース";
            this.lblCourse.Top = 0.5346457F;
            this.lblCourse.Width = 0.4330709F;
            // 
            // lblTantosya
            // 
            this.lblTantosya.Height = 0.1496063F;
            this.lblTantosya.HyperLink = null;
            this.lblTantosya.Left = 4.938976F;
            this.lblTantosya.MultiLine = false;
            this.lblTantosya.Name = "lblTantosya";
            this.lblTantosya.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblTantosya.Text = "担当者";
            this.lblTantosya.Top = 0.7484252F;
            this.lblTantosya.Width = 0.4330709F;
            // 
            // txtCourseCD
            // 
            this.txtCourseCD.CanGrow = false;
            this.txtCourseCD.DataField = "コースＣＤ";
            this.txtCourseCD.Height = 0.1496063F;
            this.txtCourseCD.Left = 5.425001F;
            this.txtCourseCD.MultiLine = false;
            this.txtCourseCD.Name = "txtCourseCD";
            this.txtCourseCD.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtCourseCD.Text = "コースＣＤ";
            this.txtCourseCD.Top = 0.5346457F;
            this.txtCourseCD.Width = 0.4472224F;
            // 
            // txtCourseName
            // 
            this.txtCourseName.CanGrow = false;
            this.txtCourseName.DataField = "コース名";
            this.txtCourseName.Height = 0.1496063F;
            this.txtCourseName.Left = 5.950002F;
            this.txtCourseName.MultiLine = false;
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: left; white-space: nowrap; dd" +
    "o-wrap-mode: nowrap";
            this.txtCourseName.Text = "コース名";
            this.txtCourseName.Top = 0.5346457F;
            this.txtCourseName.Width = 1.583334F;
            // 
            // txtTantosyaCD
            // 
            this.txtTantosyaCD.CanGrow = false;
            this.txtTantosyaCD.DataField = "配送担当者ＣＤ";
            this.txtTantosyaCD.Height = 0.1496063F;
            this.txtTantosyaCD.Left = 5.425197F;
            this.txtTantosyaCD.MultiLine = false;
            this.txtTantosyaCD.Name = "txtTantosyaCD";
            this.txtTantosyaCD.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTantosyaCD.Text = "配送担当者ＣＤ";
            this.txtTantosyaCD.Top = 0.7484252F;
            this.txtTantosyaCD.Width = 0.4472224F;
            // 
            // txtTantosyaName
            // 
            this.txtTantosyaName.CanGrow = false;
            this.txtTantosyaName.DataField = "配送担当者名";
            this.txtTantosyaName.Height = 0.1496063F;
            this.txtTantosyaName.Left = 5.950001F;
            this.txtTantosyaName.MultiLine = false;
            this.txtTantosyaName.Name = "txtTantosyaName";
            this.txtTantosyaName.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: left; white-space: nowrap; dd" +
    "o-wrap-mode: nowrap";
            this.txtTantosyaName.Text = "配送担当者名";
            this.txtTantosyaName.Top = 0.7484252F;
            this.txtTantosyaName.Width = 1.583333F;
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.subReportMeisaiPay,
            this.subReportMochidashi,
            this.shapeTicket,
            this.lineTicket,
            this.lineTicketMochidashi,
            this.lineTicketMochiKaeri,
            this.lineTicketUriMai,
            this.lineTicketUriKin,
            this.lblTicketSeisan,
            this.lblTicket,
            this.lblTicketMochidashi,
            this.lblTicketMochiKaeri,
            this.lblTicketUriMai,
            this.lblTicketUriKin,
            this.txtTicketMochidashi,
            this.txtTicketMochiKaeri,
            this.txtTicketUriMai,
            this.txtTicketUriKin,
            this.subReportMeisaiGenkin,
            this.lineGenkinLeft,
            this.lineGenkinNebiki,
            this.lineGenkinUrikake,
            this.lineGenkinTicket,
            this.lineGenkinTotal,
            this.lineGenkinRight,
            this.lineGenkinUnder,
            this.lblGenkinTotal,
            this.lblGenkinNebiki,
            this.txtGenkinUri,
            this.txtGenkinNebiki,
            this.txtGenkinUrikake,
            this.txtGenkinTicket,
            this.txtGenkinTotal,
            this.subReportMeisaiKakeuri,
            this.lineKakeuriLeft,
            this.lineKakeuriNebiki,
            this.lineKakeuriMisyukin,
            this.lineKakeuriTicket,
            this.lineKakeuriTotal,
            this.lineKakeuriRight,
            this.lineKakeuriUnder,
            this.lblKakeuriTotal,
            this.lblKakeuriNebiki,
            this.txtKakeuriUri,
            this.txtKakeuriNebiki,
            this.txtKakeuriMisyukin,
            this.txtKakeuriTotal,
            this.shapeTotal,
            this.lineTotalYoko1,
            this.lineTotalYoko2,
            this.lineTotalNebiki,
            this.lineTotalTicket,
            this.lineTotalBarclay,
            this.lineTotalTotal,
            this.lblTotalUriKin,
            this.lblTotalUriKinFormula,
            this.txtTotalUriKin,
            this.lblTotalNebiki,
            this.lblTotalNebikiFormula,
            this.txtTotalNebiki,
            this.lblTotalTicket,
            this.txtTotalTicket,
            this.lblTotalBarclay,
            this.txtTotalBarclay,
            this.lblTotalTotal,
            this.label1,
            this.txtTotalTotal,
            this.lblKinsyuTable,
            this.shapeBill,
            this.lineBill5k,
            this.lineBill2k,
            this.lineBill1k,
            this.lblBill10k,
            this.lblBill5k,
            this.lblBill2k,
            this.lblBill1k,
            this.txtBill10k,
            this.txtBill5k,
            this.txtBill2k,
            this.txtBill1k,
            this.shapeCoin,
            this.lineCoin100,
            this.lineCoin50,
            this.lineCoin10,
            this.lineCoin5,
            this.lineCoin1,
            this.lblCoin500,
            this.txtCoin500,
            this.lblCoin100,
            this.lblCoin50,
            this.lblCoin10,
            this.lblCoin5,
            this.lblCoin1,
            this.txtCoin100,
            this.txtCoin50,
            this.txtCoin10,
            this.txtCoin5,
            this.txtCoin1,
            this.shapeKogitte,
            this.lineKogitteTate,
            this.lineRyosyusyo,
            this.lineKinsyuTotal,
            this.lblKogitte,
            this.lblRyosyusyo,
            this.lblKinsyuTotal,
            this.txtKogitte,
            this.txtRyosyusyo,
            this.txtKinsyuTotal,
            this.lineKajyokinUnder,
            this.lineKajyokinRight,
            this.lblKajokin,
            this.txtKajokin,
            this.shapeTicketFooter,
            this.lineTicketFooterTate,
            this.lblTicketFooterTitle,
            this.lblTicketFooterKake,
            this.txtTicketMai,
            this.lblTicketMai,
            this.lblBonusFooterKake,
            this.txtBonusMai,
            this.lblBonusMai,
            this.lblBarclayFooterTitle,
            this.lblBV300Kake,
            this.txtBV300Mai,
            this.lblBV300Equal,
            this.txtBV300Kingaku,
            this.lblBV200Kake,
            this.txtBV200Mai,
            this.lblBV200Equal,
            this.txtBV200Kingaku,
            this.lineGenkinUpper,
            this.lineKakeuriUpper,
            this.lblMai10k,
            this.lblMai5k,
            this.lblMai2k,
            this.lblMai1k,
            this.txtAmount10k,
            this.txtAmount5k,
            this.txtAmount2k,
            this.txtAmount1k,
            this.lblMai500,
            this.lblMai100,
            this.lblMai50,
            this.lblMai10,
            this.lblMai5,
            this.lblMai1,
            this.txtAmount500,
            this.txtAmount100,
            this.txtAmount50,
            this.txtAmount10,
            this.txtAmount5,
            this.txtAmount1,
            this.lblPayTotal,
            this.txtPayUri,
            this.lblPayNebiki,
            this.txtPayNebiki,
            this.txtPayTotal,
            this.linePayUnder,
            this.linePayLeft,
            this.linePayNebiki,
            this.linePayMisyukin,
            this.linePayTicket,
            this.linePayTotal,
            this.linePayRight,
            this.linePayUpper,
            this.txtKaisyuPay,
            this.txtPayTicket});
            this.detail.Height = 5.141339F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // subReportMeisaiPay
            // 
            this.subReportMeisaiPay.CloseBorder = false;
            this.subReportMeisaiPay.Height = 0.2137795F;
            this.subReportMeisaiPay.Left = 0F;
            this.subReportMeisaiPay.Name = "subReportMeisaiPay";
            this.subReportMeisaiPay.Report = null;
            this.subReportMeisaiPay.ReportName = "subReportMeisaiPay";
            this.subReportMeisaiPay.Top = 1.771654F;
            this.subReportMeisaiPay.Width = 7.768896F;
            // 
            // subReportMochidashi
            // 
            this.subReportMochidashi.CloseBorder = false;
            this.subReportMochidashi.Height = 0.2137795F;
            this.subReportMochidashi.Left = 0F;
            this.subReportMochidashi.Name = "subReportMochidashi";
            this.subReportMochidashi.Report = null;
            this.subReportMochidashi.ReportName = "subReportMochidashi";
            this.subReportMochidashi.Top = 0F;
            this.subReportMochidashi.Width = 7.768898F;
            // 
            // shapeTicket
            // 
            this.shapeTicket.Height = 0.3937008F;
            this.shapeTicket.Left = 0.2952756F;
            this.shapeTicket.Name = "shapeTicket";
            this.shapeTicket.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeTicket.Top = 0.2526685F;
            this.shapeTicket.Width = 3.937008F;
            // 
            // lineTicket
            // 
            this.lineTicket.Height = 0F;
            this.lineTicket.Left = 0.2952756F;
            this.lineTicket.LineWeight = 1F;
            this.lineTicket.Name = "lineTicket";
            this.lineTicket.Top = 0.4496063F;
            this.lineTicket.Width = 3.937008F;
            this.lineTicket.X1 = 0.2952756F;
            this.lineTicket.X2 = 4.232284F;
            this.lineTicket.Y1 = 0.4496063F;
            this.lineTicket.Y2 = 0.4496063F;
            // 
            // lineTicketMochidashi
            // 
            this.lineTicketMochidashi.Height = 0.3937008F;
            this.lineTicketMochidashi.Left = 1.082677F;
            this.lineTicketMochidashi.LineWeight = 1F;
            this.lineTicketMochidashi.Name = "lineTicketMochidashi";
            this.lineTicketMochidashi.Top = 0.2527559F;
            this.lineTicketMochidashi.Width = 0F;
            this.lineTicketMochidashi.X1 = 1.082677F;
            this.lineTicketMochidashi.X2 = 1.082677F;
            this.lineTicketMochidashi.Y1 = 0.2527559F;
            this.lineTicketMochidashi.Y2 = 0.6464567F;
            // 
            // lineTicketMochiKaeri
            // 
            this.lineTicketMochiKaeri.Height = 0.3937008F;
            this.lineTicketMochiKaeri.Left = 1.870079F;
            this.lineTicketMochiKaeri.LineWeight = 1F;
            this.lineTicketMochiKaeri.Name = "lineTicketMochiKaeri";
            this.lineTicketMochiKaeri.Top = 0.2527559F;
            this.lineTicketMochiKaeri.Width = 0F;
            this.lineTicketMochiKaeri.X1 = 1.870079F;
            this.lineTicketMochiKaeri.X2 = 1.870079F;
            this.lineTicketMochiKaeri.Y1 = 0.2527559F;
            this.lineTicketMochiKaeri.Y2 = 0.6464567F;
            // 
            // lineTicketUriMai
            // 
            this.lineTicketUriMai.Height = 0.3937008F;
            this.lineTicketUriMai.Left = 2.65748F;
            this.lineTicketUriMai.LineWeight = 1F;
            this.lineTicketUriMai.Name = "lineTicketUriMai";
            this.lineTicketUriMai.Top = 0.2527559F;
            this.lineTicketUriMai.Width = 0F;
            this.lineTicketUriMai.X1 = 2.65748F;
            this.lineTicketUriMai.X2 = 2.65748F;
            this.lineTicketUriMai.Y1 = 0.2527559F;
            this.lineTicketUriMai.Y2 = 0.6464567F;
            // 
            // lineTicketUriKin
            // 
            this.lineTicketUriKin.Height = 0.3937008F;
            this.lineTicketUriKin.Left = 3.444882F;
            this.lineTicketUriKin.LineWeight = 1F;
            this.lineTicketUriKin.Name = "lineTicketUriKin";
            this.lineTicketUriKin.Top = 0.2527559F;
            this.lineTicketUriKin.Width = 0F;
            this.lineTicketUriKin.X1 = 3.444882F;
            this.lineTicketUriKin.X2 = 3.444882F;
            this.lineTicketUriKin.Y1 = 0.2527559F;
            this.lineTicketUriKin.Y2 = 0.6464567F;
            // 
            // lblTicketSeisan
            // 
            this.lblTicketSeisan.Height = 0.1496063F;
            this.lblTicketSeisan.HyperLink = null;
            this.lblTicketSeisan.Left = 0.322441F;
            this.lblTicketSeisan.MultiLine = false;
            this.lblTicketSeisan.Name = "lblTicketSeisan";
            this.lblTicketSeisan.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTicketSeisan.Text = "チケット精算";
            this.lblTicketSeisan.Top = 0.276378F;
            this.lblTicketSeisan.Width = 0.7322835F;
            // 
            // lblTicket
            // 
            this.lblTicket.Height = 0.1496063F;
            this.lblTicket.HyperLink = null;
            this.lblTicket.Left = 0.322441F;
            this.lblTicket.MultiLine = false;
            this.lblTicket.Name = "lblTicket";
            this.lblTicket.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTicket.Text = "チケット";
            this.lblTicket.Top = 0.4732284F;
            this.lblTicket.Width = 0.7322834F;
            // 
            // lblTicketMochidashi
            // 
            this.lblTicketMochidashi.Height = 0.1496063F;
            this.lblTicketMochidashi.HyperLink = null;
            this.lblTicketMochidashi.Left = 1.109843F;
            this.lblTicketMochidashi.MultiLine = false;
            this.lblTicketMochidashi.Name = "lblTicketMochidashi";
            this.lblTicketMochidashi.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTicketMochidashi.Text = "持出枚数";
            this.lblTicketMochidashi.Top = 0.276378F;
            this.lblTicketMochidashi.Width = 0.7322834F;
            // 
            // lblTicketMochiKaeri
            // 
            this.lblTicketMochiKaeri.Height = 0.1496063F;
            this.lblTicketMochiKaeri.HyperLink = null;
            this.lblTicketMochiKaeri.Left = 1.897244F;
            this.lblTicketMochiKaeri.MultiLine = false;
            this.lblTicketMochiKaeri.Name = "lblTicketMochiKaeri";
            this.lblTicketMochiKaeri.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTicketMochiKaeri.Text = "持帰枚数";
            this.lblTicketMochiKaeri.Top = 0.276378F;
            this.lblTicketMochiKaeri.Width = 0.7322834F;
            // 
            // lblTicketUriMai
            // 
            this.lblTicketUriMai.Height = 0.1496063F;
            this.lblTicketUriMai.HyperLink = null;
            this.lblTicketUriMai.Left = 2.684646F;
            this.lblTicketUriMai.MultiLine = false;
            this.lblTicketUriMai.Name = "lblTicketUriMai";
            this.lblTicketUriMai.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTicketUriMai.Text = "売上枚数";
            this.lblTicketUriMai.Top = 0.276378F;
            this.lblTicketUriMai.Width = 0.7322834F;
            // 
            // lblTicketUriKin
            // 
            this.lblTicketUriKin.Height = 0.1496063F;
            this.lblTicketUriKin.HyperLink = null;
            this.lblTicketUriKin.Left = 3.472047F;
            this.lblTicketUriKin.MultiLine = false;
            this.lblTicketUriKin.Name = "lblTicketUriKin";
            this.lblTicketUriKin.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTicketUriKin.Text = "売上金額";
            this.lblTicketUriKin.Top = 0.276378F;
            this.lblTicketUriKin.Width = 0.7322834F;
            // 
            // txtTicketMochidashi
            // 
            this.txtTicketMochidashi.CanGrow = false;
            this.txtTicketMochidashi.DataField = "チケット持出枚数";
            this.txtTicketMochidashi.Height = 0.1496063F;
            this.txtTicketMochidashi.Left = 1.109843F;
            this.txtTicketMochidashi.MultiLine = false;
            this.txtTicketMochidashi.Name = "txtTicketMochidashi";
            this.txtTicketMochidashi.OutputFormat = resources.GetString("txtTicketMochidashi.OutputFormat");
            this.txtTicketMochidashi.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTicketMochidashi.Text = "チケット持出枚数";
            this.txtTicketMochidashi.Top = 0.4732284F;
            this.txtTicketMochidashi.Width = 0.7322835F;
            // 
            // txtTicketMochiKaeri
            // 
            this.txtTicketMochiKaeri.CanGrow = false;
            this.txtTicketMochiKaeri.DataField = "チケット持帰枚数";
            this.txtTicketMochiKaeri.Height = 0.1496063F;
            this.txtTicketMochiKaeri.Left = 1.897244F;
            this.txtTicketMochiKaeri.MultiLine = false;
            this.txtTicketMochiKaeri.Name = "txtTicketMochiKaeri";
            this.txtTicketMochiKaeri.OutputFormat = resources.GetString("txtTicketMochiKaeri.OutputFormat");
            this.txtTicketMochiKaeri.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTicketMochiKaeri.Text = "チケット持帰枚数";
            this.txtTicketMochiKaeri.Top = 0.4732284F;
            this.txtTicketMochiKaeri.Width = 0.7322834F;
            // 
            // txtTicketUriMai
            // 
            this.txtTicketUriMai.CanGrow = false;
            this.txtTicketUriMai.DataField = "チケット売上枚数";
            this.txtTicketUriMai.Height = 0.1496063F;
            this.txtTicketUriMai.Left = 2.684646F;
            this.txtTicketUriMai.MultiLine = false;
            this.txtTicketUriMai.Name = "txtTicketUriMai";
            this.txtTicketUriMai.OutputFormat = resources.GetString("txtTicketUriMai.OutputFormat");
            this.txtTicketUriMai.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTicketUriMai.Text = "チケット売上枚数";
            this.txtTicketUriMai.Top = 0.4732284F;
            this.txtTicketUriMai.Width = 0.7322834F;
            // 
            // txtTicketUriKin
            // 
            this.txtTicketUriKin.CanGrow = false;
            this.txtTicketUriKin.DataField = "チケット売上金額";
            this.txtTicketUriKin.Height = 0.1496063F;
            this.txtTicketUriKin.Left = 3.472047F;
            this.txtTicketUriKin.MultiLine = false;
            this.txtTicketUriKin.Name = "txtTicketUriKin";
            this.txtTicketUriKin.OutputFormat = resources.GetString("txtTicketUriKin.OutputFormat");
            this.txtTicketUriKin.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTicketUriKin.Text = "チケット売上金額";
            this.txtTicketUriKin.Top = 0.4732284F;
            this.txtTicketUriKin.Width = 0.7322834F;
            // 
            // subReportMeisaiGenkin
            // 
            this.subReportMeisaiGenkin.CloseBorder = false;
            this.subReportMeisaiGenkin.Height = 0.2137795F;
            this.subReportMeisaiGenkin.Left = 0F;
            this.subReportMeisaiGenkin.Name = "subReportMeisaiGenkin";
            this.subReportMeisaiGenkin.Report = null;
            this.subReportMeisaiGenkin.ReportName = "subReportMeisaiGenkin";
            this.subReportMeisaiGenkin.Top = 0.7598425F;
            this.subReportMeisaiGenkin.Width = 7.768896F;
            // 
            // lineGenkinLeft
            // 
            this.lineGenkinLeft.Height = 0.2137793F;
            this.lineGenkinLeft.Left = 0.2952756F;
            this.lineGenkinLeft.LineWeight = 1F;
            this.lineGenkinLeft.Name = "lineGenkinLeft";
            this.lineGenkinLeft.Top = 0.9740158F;
            this.lineGenkinLeft.Width = 0F;
            this.lineGenkinLeft.X1 = 0.2952756F;
            this.lineGenkinLeft.X2 = 0.2952756F;
            this.lineGenkinLeft.Y1 = 0.9740158F;
            this.lineGenkinLeft.Y2 = 1.187795F;
            // 
            // lineGenkinNebiki
            // 
            this.lineGenkinNebiki.Height = 0.213779F;
            this.lineGenkinNebiki.Left = 2.535669F;
            this.lineGenkinNebiki.LineWeight = 1F;
            this.lineGenkinNebiki.Name = "lineGenkinNebiki";
            this.lineGenkinNebiki.Top = 0.974016F;
            this.lineGenkinNebiki.Width = 0F;
            this.lineGenkinNebiki.X1 = 2.535669F;
            this.lineGenkinNebiki.X2 = 2.535669F;
            this.lineGenkinNebiki.Y1 = 0.974016F;
            this.lineGenkinNebiki.Y2 = 1.187795F;
            // 
            // lineGenkinUrikake
            // 
            this.lineGenkinUrikake.Height = 0.213779F;
            this.lineGenkinUrikake.Left = 4.233937F;
            this.lineGenkinUrikake.LineWeight = 1F;
            this.lineGenkinUrikake.Name = "lineGenkinUrikake";
            this.lineGenkinUrikake.Top = 0.974016F;
            this.lineGenkinUrikake.Width = 0F;
            this.lineGenkinUrikake.X1 = 4.233937F;
            this.lineGenkinUrikake.X2 = 4.233937F;
            this.lineGenkinUrikake.Y1 = 0.974016F;
            this.lineGenkinUrikake.Y2 = 1.187795F;
            // 
            // lineGenkinTicket
            // 
            this.lineGenkinTicket.Height = 0.2137793F;
            this.lineGenkinTicket.Left = 5.292441F;
            this.lineGenkinTicket.LineWeight = 1F;
            this.lineGenkinTicket.Name = "lineGenkinTicket";
            this.lineGenkinTicket.Top = 0.9740158F;
            this.lineGenkinTicket.Width = 0F;
            this.lineGenkinTicket.X1 = 5.292441F;
            this.lineGenkinTicket.X2 = 5.292441F;
            this.lineGenkinTicket.Y1 = 0.9740158F;
            this.lineGenkinTicket.Y2 = 1.187795F;
            // 
            // lineGenkinTotal
            // 
            this.lineGenkinTotal.Height = 0.213779F;
            this.lineGenkinTotal.Left = 6.491339F;
            this.lineGenkinTotal.LineWeight = 1F;
            this.lineGenkinTotal.Name = "lineGenkinTotal";
            this.lineGenkinTotal.Top = 0.974016F;
            this.lineGenkinTotal.Width = 0F;
            this.lineGenkinTotal.X1 = 6.491339F;
            this.lineGenkinTotal.X2 = 6.491339F;
            this.lineGenkinTotal.Y1 = 0.974016F;
            this.lineGenkinTotal.Y2 = 1.187795F;
            // 
            // lineGenkinRight
            // 
            this.lineGenkinRight.Height = 0.2137793F;
            this.lineGenkinRight.Left = 7.616536F;
            this.lineGenkinRight.LineWeight = 1F;
            this.lineGenkinRight.Name = "lineGenkinRight";
            this.lineGenkinRight.Top = 0.9740158F;
            this.lineGenkinRight.Width = 0F;
            this.lineGenkinRight.X1 = 7.616536F;
            this.lineGenkinRight.X2 = 7.616536F;
            this.lineGenkinRight.Y1 = 0.9740158F;
            this.lineGenkinRight.Y2 = 1.187795F;
            // 
            // lineGenkinUnder
            // 
            this.lineGenkinUnder.Height = 0F;
            this.lineGenkinUnder.Left = 0.2952756F;
            this.lineGenkinUnder.LineWeight = 1F;
            this.lineGenkinUnder.Name = "lineGenkinUnder";
            this.lineGenkinUnder.Top = 1.187795F;
            this.lineGenkinUnder.Width = 7.32126F;
            this.lineGenkinUnder.X1 = 0.2952756F;
            this.lineGenkinUnder.X2 = 7.616536F;
            this.lineGenkinUnder.Y1 = 1.187795F;
            this.lineGenkinUnder.Y2 = 1.187795F;
            // 
            // lblGenkinTotal
            // 
            this.lblGenkinTotal.Height = 0.1496063F;
            this.lblGenkinTotal.HyperLink = null;
            this.lblGenkinTotal.Left = 0.3149606F;
            this.lblGenkinTotal.MultiLine = false;
            this.lblGenkinTotal.Name = "lblGenkinTotal";
            this.lblGenkinTotal.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblGenkinTotal.Text = "現金売合計";
            this.lblGenkinTotal.Top = 1.005512F;
            this.lblGenkinTotal.Width = 0.9165355F;
            // 
            // lblGenkinNebiki
            // 
            this.lblGenkinNebiki.Height = 0.1496063F;
            this.lblGenkinNebiki.HyperLink = null;
            this.lblGenkinNebiki.Left = 2.556142F;
            this.lblGenkinNebiki.MultiLine = false;
            this.lblGenkinNebiki.Name = "lblGenkinNebiki";
            this.lblGenkinNebiki.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblGenkinNebiki.Text = "値引額合計";
            this.lblGenkinNebiki.Top = 1.005512F;
            this.lblGenkinNebiki.Width = 0.7322834F;
            // 
            // txtGenkinUri
            // 
            this.txtGenkinUri.CanGrow = false;
            this.txtGenkinUri.DataField = "現金_売上合計";
            this.txtGenkinUri.Height = 0.1496063F;
            this.txtGenkinUri.Left = 1.274409F;
            this.txtGenkinUri.MultiLine = false;
            this.txtGenkinUri.Name = "txtGenkinUri";
            this.txtGenkinUri.OutputFormat = resources.GetString("txtGenkinUri.OutputFormat");
            this.txtGenkinUri.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtGenkinUri.Text = "現金_売上合計";
            this.txtGenkinUri.Top = 1.005512F;
            this.txtGenkinUri.Width = 1.226772F;
            // 
            // txtGenkinNebiki
            // 
            this.txtGenkinNebiki.CanGrow = false;
            this.txtGenkinNebiki.DataField = "現金_値引合計";
            this.txtGenkinNebiki.Height = 0.1496063F;
            this.txtGenkinNebiki.Left = 3.31874F;
            this.txtGenkinNebiki.MultiLine = false;
            this.txtGenkinNebiki.Name = "txtGenkinNebiki";
            this.txtGenkinNebiki.OutputFormat = resources.GetString("txtGenkinNebiki.OutputFormat");
            this.txtGenkinNebiki.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtGenkinNebiki.Text = "現金_値引合計";
            this.txtGenkinNebiki.Top = 1.005512F;
            this.txtGenkinNebiki.Width = 0.880315F;
            // 
            // txtGenkinUrikake
            // 
            this.txtGenkinUrikake.CanGrow = false;
            this.txtGenkinUrikake.DataField = "現金_回収現金";
            this.txtGenkinUrikake.Height = 0.1496063F;
            this.txtGenkinUrikake.Left = 4.268504F;
            this.txtGenkinUrikake.MultiLine = false;
            this.txtGenkinUrikake.Name = "txtGenkinUrikake";
            this.txtGenkinUrikake.OutputFormat = resources.GetString("txtGenkinUrikake.OutputFormat");
            this.txtGenkinUrikake.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtGenkinUrikake.Text = "現金_回収現金";
            this.txtGenkinUrikake.Top = 1.005512F;
            this.txtGenkinUrikake.Width = 0.9893701F;
            // 
            // txtGenkinTicket
            // 
            this.txtGenkinTicket.CanGrow = false;
            this.txtGenkinTicket.DataField = "現金_チケット束代";
            this.txtGenkinTicket.Height = 0.1496063F;
            this.txtGenkinTicket.Left = 5.312205F;
            this.txtGenkinTicket.MultiLine = false;
            this.txtGenkinTicket.Name = "txtGenkinTicket";
            this.txtGenkinTicket.OutputFormat = resources.GetString("txtGenkinTicket.OutputFormat");
            this.txtGenkinTicket.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtGenkinTicket.Text = "現金_チケット束代";
            this.txtGenkinTicket.Top = 1.005512F;
            this.txtGenkinTicket.Width = 1.144488F;
            // 
            // txtGenkinTotal
            // 
            this.txtGenkinTotal.CanGrow = false;
            this.txtGenkinTotal.DataField = "現金_本日現金合計";
            this.txtGenkinTotal.Height = 0.1496063F;
            this.txtGenkinTotal.Left = 6.511024F;
            this.txtGenkinTotal.MultiLine = false;
            this.txtGenkinTotal.Name = "txtGenkinTotal";
            this.txtGenkinTotal.OutputFormat = resources.GetString("txtGenkinTotal.OutputFormat");
            this.txtGenkinTotal.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtGenkinTotal.Text = "現金_本日現金合計";
            this.txtGenkinTotal.Top = 1.005512F;
            this.txtGenkinTotal.Width = 1.070866F;
            // 
            // subReportMeisaiKakeuri
            // 
            this.subReportMeisaiKakeuri.CloseBorder = false;
            this.subReportMeisaiKakeuri.Height = 0.2137795F;
            this.subReportMeisaiKakeuri.Left = 0F;
            this.subReportMeisaiKakeuri.Name = "subReportMeisaiKakeuri";
            this.subReportMeisaiKakeuri.Report = null;
            this.subReportMeisaiKakeuri.ReportName = "subReportMeisaiKakeuri";
            this.subReportMeisaiKakeuri.Top = 1.265573F;
            this.subReportMeisaiKakeuri.Width = 7.768896F;
            // 
            // lineKakeuriLeft
            // 
            this.lineKakeuriLeft.Height = 0.2137791F;
            this.lineKakeuriLeft.Left = 0.2952756F;
            this.lineKakeuriLeft.LineWeight = 1F;
            this.lineKakeuriLeft.Name = "lineKakeuriLeft";
            this.lineKakeuriLeft.Top = 1.479528F;
            this.lineKakeuriLeft.Width = 0F;
            this.lineKakeuriLeft.X1 = 0.2952756F;
            this.lineKakeuriLeft.X2 = 0.2952756F;
            this.lineKakeuriLeft.Y1 = 1.479528F;
            this.lineKakeuriLeft.Y2 = 1.693307F;
            // 
            // lineKakeuriNebiki
            // 
            this.lineKakeuriNebiki.Height = 0.2137791F;
            this.lineKakeuriNebiki.Left = 2.535669F;
            this.lineKakeuriNebiki.LineWeight = 1F;
            this.lineKakeuriNebiki.Name = "lineKakeuriNebiki";
            this.lineKakeuriNebiki.Top = 1.479528F;
            this.lineKakeuriNebiki.Width = 0F;
            this.lineKakeuriNebiki.X1 = 2.535669F;
            this.lineKakeuriNebiki.X2 = 2.535669F;
            this.lineKakeuriNebiki.Y1 = 1.479528F;
            this.lineKakeuriNebiki.Y2 = 1.693307F;
            // 
            // lineKakeuriMisyukin
            // 
            this.lineKakeuriMisyukin.Height = 0.2137791F;
            this.lineKakeuriMisyukin.Left = 4.233937F;
            this.lineKakeuriMisyukin.LineWeight = 1F;
            this.lineKakeuriMisyukin.Name = "lineKakeuriMisyukin";
            this.lineKakeuriMisyukin.Top = 1.479528F;
            this.lineKakeuriMisyukin.Width = 0F;
            this.lineKakeuriMisyukin.X1 = 4.233937F;
            this.lineKakeuriMisyukin.X2 = 4.233937F;
            this.lineKakeuriMisyukin.Y1 = 1.479528F;
            this.lineKakeuriMisyukin.Y2 = 1.693307F;
            // 
            // lineKakeuriTicket
            // 
            this.lineKakeuriTicket.Height = 0.2137791F;
            this.lineKakeuriTicket.Left = 5.292441F;
            this.lineKakeuriTicket.LineWeight = 1F;
            this.lineKakeuriTicket.Name = "lineKakeuriTicket";
            this.lineKakeuriTicket.Top = 1.479528F;
            this.lineKakeuriTicket.Width = 0F;
            this.lineKakeuriTicket.X1 = 5.292441F;
            this.lineKakeuriTicket.X2 = 5.292441F;
            this.lineKakeuriTicket.Y1 = 1.479528F;
            this.lineKakeuriTicket.Y2 = 1.693307F;
            // 
            // lineKakeuriTotal
            // 
            this.lineKakeuriTotal.Height = 0.2137791F;
            this.lineKakeuriTotal.Left = 6.491339F;
            this.lineKakeuriTotal.LineWeight = 1F;
            this.lineKakeuriTotal.Name = "lineKakeuriTotal";
            this.lineKakeuriTotal.Top = 1.479528F;
            this.lineKakeuriTotal.Width = 0F;
            this.lineKakeuriTotal.X1 = 6.491339F;
            this.lineKakeuriTotal.X2 = 6.491339F;
            this.lineKakeuriTotal.Y1 = 1.479528F;
            this.lineKakeuriTotal.Y2 = 1.693307F;
            // 
            // lineKakeuriRight
            // 
            this.lineKakeuriRight.Height = 0.2137791F;
            this.lineKakeuriRight.Left = 7.616536F;
            this.lineKakeuriRight.LineWeight = 1F;
            this.lineKakeuriRight.Name = "lineKakeuriRight";
            this.lineKakeuriRight.Top = 1.479528F;
            this.lineKakeuriRight.Width = 0F;
            this.lineKakeuriRight.X1 = 7.616536F;
            this.lineKakeuriRight.X2 = 7.616536F;
            this.lineKakeuriRight.Y1 = 1.479528F;
            this.lineKakeuriRight.Y2 = 1.693307F;
            // 
            // lineKakeuriUnder
            // 
            this.lineKakeuriUnder.Height = 0F;
            this.lineKakeuriUnder.Left = 0.2952756F;
            this.lineKakeuriUnder.LineWeight = 1F;
            this.lineKakeuriUnder.Name = "lineKakeuriUnder";
            this.lineKakeuriUnder.Top = 1.693307F;
            this.lineKakeuriUnder.Width = 7.321259F;
            this.lineKakeuriUnder.X1 = 0.2952756F;
            this.lineKakeuriUnder.X2 = 7.616534F;
            this.lineKakeuriUnder.Y1 = 1.693307F;
            this.lineKakeuriUnder.Y2 = 1.693307F;
            // 
            // lblKakeuriTotal
            // 
            this.lblKakeuriTotal.Height = 0.1496063F;
            this.lblKakeuriTotal.HyperLink = null;
            this.lblKakeuriTotal.Left = 0.3149606F;
            this.lblKakeuriTotal.MultiLine = false;
            this.lblKakeuriTotal.Name = "lblKakeuriTotal";
            this.lblKakeuriTotal.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblKakeuriTotal.Text = "掛売合計";
            this.lblKakeuriTotal.Top = 1.511024F;
            this.lblKakeuriTotal.Width = 0.9165355F;
            // 
            // lblKakeuriNebiki
            // 
            this.lblKakeuriNebiki.Height = 0.1496063F;
            this.lblKakeuriNebiki.HyperLink = null;
            this.lblKakeuriNebiki.Left = 2.556142F;
            this.lblKakeuriNebiki.MultiLine = false;
            this.lblKakeuriNebiki.Name = "lblKakeuriNebiki";
            this.lblKakeuriNebiki.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblKakeuriNebiki.Text = "値引額合計";
            this.lblKakeuriNebiki.Top = 1.511024F;
            this.lblKakeuriNebiki.Width = 0.7322834F;
            // 
            // txtKakeuriUri
            // 
            this.txtKakeuriUri.CanGrow = false;
            this.txtKakeuriUri.DataField = "掛売_売上合計";
            this.txtKakeuriUri.Height = 0.1496063F;
            this.txtKakeuriUri.Left = 1.274409F;
            this.txtKakeuriUri.MultiLine = false;
            this.txtKakeuriUri.Name = "txtKakeuriUri";
            this.txtKakeuriUri.OutputFormat = resources.GetString("txtKakeuriUri.OutputFormat");
            this.txtKakeuriUri.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtKakeuriUri.Text = "掛売_売上合計";
            this.txtKakeuriUri.Top = 1.511024F;
            this.txtKakeuriUri.Width = 1.226772F;
            // 
            // txtKakeuriNebiki
            // 
            this.txtKakeuriNebiki.CanGrow = false;
            this.txtKakeuriNebiki.DataField = "掛売_値引合計";
            this.txtKakeuriNebiki.Height = 0.1496063F;
            this.txtKakeuriNebiki.Left = 3.31874F;
            this.txtKakeuriNebiki.MultiLine = false;
            this.txtKakeuriNebiki.Name = "txtKakeuriNebiki";
            this.txtKakeuriNebiki.OutputFormat = resources.GetString("txtKakeuriNebiki.OutputFormat");
            this.txtKakeuriNebiki.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtKakeuriNebiki.Text = "掛売_値引合計";
            this.txtKakeuriNebiki.Top = 1.511024F;
            this.txtKakeuriNebiki.Width = 0.880315F;
            // 
            // txtKakeuriMisyukin
            // 
            this.txtKakeuriMisyukin.CanGrow = false;
            this.txtKakeuriMisyukin.DataField = "掛売_未集金";
            this.txtKakeuriMisyukin.Height = 0.1496063F;
            this.txtKakeuriMisyukin.Left = 4.268504F;
            this.txtKakeuriMisyukin.MultiLine = false;
            this.txtKakeuriMisyukin.Name = "txtKakeuriMisyukin";
            this.txtKakeuriMisyukin.OutputFormat = resources.GetString("txtKakeuriMisyukin.OutputFormat");
            this.txtKakeuriMisyukin.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtKakeuriMisyukin.Text = "掛売_未集金";
            this.txtKakeuriMisyukin.Top = 1.511024F;
            this.txtKakeuriMisyukin.Width = 0.9893701F;
            // 
            // txtKakeuriTotal
            // 
            this.txtKakeuriTotal.CanGrow = false;
            this.txtKakeuriTotal.DataField = "掛売_本日掛売合計";
            this.txtKakeuriTotal.Height = 0.1496063F;
            this.txtKakeuriTotal.Left = 6.511024F;
            this.txtKakeuriTotal.MultiLine = false;
            this.txtKakeuriTotal.Name = "txtKakeuriTotal";
            this.txtKakeuriTotal.OutputFormat = resources.GetString("txtKakeuriTotal.OutputFormat");
            this.txtKakeuriTotal.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtKakeuriTotal.Text = "掛売_本日掛売合計";
            this.txtKakeuriTotal.Top = 1.511024F;
            this.txtKakeuriTotal.Width = 1.070866F;
            // 
            // shapeTotal
            // 
            this.shapeTotal.Height = 0.6413386F;
            this.shapeTotal.Left = 0.2917323F;
            this.shapeTotal.Name = "shapeTotal";
            this.shapeTotal.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeTotal.Top = 2.277559F;
            this.shapeTotal.Width = 7.328347F;
            // 
            // lineTotalYoko1
            // 
            this.lineTotalYoko1.Height = 0F;
            this.lineTotalYoko1.Left = 0.2917323F;
            this.lineTotalYoko1.LineWeight = 1F;
            this.lineTotalYoko1.Name = "lineTotalYoko1";
            this.lineTotalYoko1.Top = 2.491339F;
            this.lineTotalYoko1.Width = 7.328347F;
            this.lineTotalYoko1.X1 = 0.2917323F;
            this.lineTotalYoko1.X2 = 7.620079F;
            this.lineTotalYoko1.Y1 = 2.491339F;
            this.lineTotalYoko1.Y2 = 2.491339F;
            // 
            // lineTotalYoko2
            // 
            this.lineTotalYoko2.Height = 0F;
            this.lineTotalYoko2.Left = 0.2917323F;
            this.lineTotalYoko2.LineWeight = 1F;
            this.lineTotalYoko2.Name = "lineTotalYoko2";
            this.lineTotalYoko2.Top = 2.705118F;
            this.lineTotalYoko2.Width = 7.328347F;
            this.lineTotalYoko2.X1 = 0.2917323F;
            this.lineTotalYoko2.X2 = 7.620079F;
            this.lineTotalYoko2.Y1 = 2.705118F;
            this.lineTotalYoko2.Y2 = 2.705118F;
            // 
            // lineTotalNebiki
            // 
            this.lineTotalNebiki.Height = 0.6413391F;
            this.lineTotalNebiki.Left = 2.535827F;
            this.lineTotalNebiki.LineWeight = 1F;
            this.lineTotalNebiki.Name = "lineTotalNebiki";
            this.lineTotalNebiki.Top = 2.277559F;
            this.lineTotalNebiki.Width = 0F;
            this.lineTotalNebiki.X1 = 2.535827F;
            this.lineTotalNebiki.X2 = 2.535827F;
            this.lineTotalNebiki.Y1 = 2.277559F;
            this.lineTotalNebiki.Y2 = 2.918898F;
            // 
            // lineTotalTicket
            // 
            this.lineTotalTicket.Height = 0.6413391F;
            this.lineTotalTicket.Left = 4.233858F;
            this.lineTotalTicket.LineWeight = 1F;
            this.lineTotalTicket.Name = "lineTotalTicket";
            this.lineTotalTicket.Top = 2.277559F;
            this.lineTotalTicket.Width = 0F;
            this.lineTotalTicket.X1 = 4.233858F;
            this.lineTotalTicket.X2 = 4.233858F;
            this.lineTotalTicket.Y1 = 2.277559F;
            this.lineTotalTicket.Y2 = 2.918898F;
            // 
            // lineTotalBarclay
            // 
            this.lineTotalBarclay.Height = 0.6413391F;
            this.lineTotalBarclay.Left = 5.29252F;
            this.lineTotalBarclay.LineWeight = 1F;
            this.lineTotalBarclay.Name = "lineTotalBarclay";
            this.lineTotalBarclay.Top = 2.277559F;
            this.lineTotalBarclay.Width = 0F;
            this.lineTotalBarclay.X1 = 5.29252F;
            this.lineTotalBarclay.X2 = 5.29252F;
            this.lineTotalBarclay.Y1 = 2.277559F;
            this.lineTotalBarclay.Y2 = 2.918898F;
            // 
            // lineTotalTotal
            // 
            this.lineTotalTotal.Height = 0.6413391F;
            this.lineTotalTotal.Left = 6.491339F;
            this.lineTotalTotal.LineWeight = 1F;
            this.lineTotalTotal.Name = "lineTotalTotal";
            this.lineTotalTotal.Top = 2.277559F;
            this.lineTotalTotal.Width = 0F;
            this.lineTotalTotal.X1 = 6.491339F;
            this.lineTotalTotal.X2 = 6.491339F;
            this.lineTotalTotal.Y1 = 2.277559F;
            this.lineTotalTotal.Y2 = 2.918898F;
            // 
            // lblTotalUriKin
            // 
            this.lblTotalUriKin.Height = 0.1496063F;
            this.lblTotalUriKin.HyperLink = null;
            this.lblTotalUriKin.Left = 0.3149606F;
            this.lblTotalUriKin.MultiLine = false;
            this.lblTotalUriKin.Name = "lblTotalUriKin";
            this.lblTotalUriKin.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTotalUriKin.Text = "⑫本日総売上金額";
            this.lblTotalUriKin.Top = 2.309055F;
            this.lblTotalUriKin.Width = 2.201181F;
            // 
            // lblTotalUriKinFormula
            // 
            this.lblTotalUriKinFormula.Height = 0.1496063F;
            this.lblTotalUriKinFormula.HyperLink = null;
            this.lblTotalUriKinFormula.Left = 0.8622047F;
            this.lblTotalUriKinFormula.MultiLine = false;
            this.lblTotalUriKinFormula.Name = "lblTotalUriKinFormula";
            this.lblTotalUriKinFormula.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblTotalUriKinFormula.Text = "①+④+⑤+⑧+⑪=";
            this.lblTotalUriKinFormula.Top = 2.522835F;
            this.lblTotalUriKinFormula.Width = 1.104331F;
            // 
            // txtTotalUriKin
            // 
            this.txtTotalUriKin.CanGrow = false;
            this.txtTotalUriKin.DataField = "=現金_チケット束代+E_チケット束代+合計_本日総売上";
            this.txtTotalUriKin.Height = 0.1496063F;
            this.txtTotalUriKin.Left = 0.7641733F;
            this.txtTotalUriKin.MultiLine = false;
            this.txtTotalUriKin.Name = "txtTotalUriKin";
            this.txtTotalUriKin.OutputFormat = resources.GetString("txtTotalUriKin.OutputFormat");
            this.txtTotalUriKin.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTotalUriKin.Text = "=現金_チケット束代+合計_本日総売上";
            this.txtTotalUriKin.Top = 2.736614F;
            this.txtTotalUriKin.Width = 1.737008F;
            // 
            // lblTotalNebiki
            // 
            this.lblTotalNebiki.Height = 0.1496063F;
            this.lblTotalNebiki.HyperLink = null;
            this.lblTotalNebiki.Left = 2.555512F;
            this.lblTotalNebiki.MultiLine = false;
            this.lblTotalNebiki.Name = "lblTotalNebiki";
            this.lblTotalNebiki.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTotalNebiki.Text = "⑬総値引金額";
            this.lblTotalNebiki.Top = 2.309055F;
            this.lblTotalNebiki.Width = 1.658661F;
            // 
            // lblTotalNebikiFormula
            // 
            this.lblTotalNebikiFormula.Height = 0.1496063F;
            this.lblTotalNebikiFormula.HyperLink = null;
            this.lblTotalNebikiFormula.Left = 2.970866F;
            this.lblTotalNebikiFormula.MultiLine = false;
            this.lblTotalNebikiFormula.Name = "lblTotalNebikiFormula";
            this.lblTotalNebikiFormula.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblTotalNebikiFormula.Text = "②+⑥+⑨=";
            this.lblTotalNebikiFormula.Top = 2.522835F;
            this.lblTotalNebikiFormula.Width = 0.7740159F;
            // 
            // txtTotalNebiki
            // 
            this.txtTotalNebiki.CanGrow = false;
            this.txtTotalNebiki.DataField = "合計_本日総値引";
            this.txtTotalNebiki.Height = 0.1496063F;
            this.txtTotalNebiki.Left = 2.882677F;
            this.txtTotalNebiki.MultiLine = false;
            this.txtTotalNebiki.Name = "txtTotalNebiki";
            this.txtTotalNebiki.OutputFormat = resources.GetString("txtTotalNebiki.OutputFormat");
            this.txtTotalNebiki.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTotalNebiki.Text = "合計_本日総値引";
            this.txtTotalNebiki.Top = 2.736614F;
            this.txtTotalNebiki.Width = 1.316535F;
            // 
            // lblTotalTicket
            // 
            this.lblTotalTicket.Height = 0.1496063F;
            this.lblTotalTicket.HyperLink = null;
            this.lblTotalTicket.Left = 4.253543F;
            this.lblTotalTicket.MultiLine = false;
            this.lblTotalTicket.Name = "lblTotalTicket";
            this.lblTotalTicket.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTotalTicket.Text = "⑭チケット払";
            this.lblTotalTicket.Top = 2.309055F;
            this.lblTotalTicket.Width = 1.019291F;
            // 
            // txtTotalTicket
            // 
            this.txtTotalTicket.CanGrow = false;
            this.txtTotalTicket.DataField = "合計_チケット売上";
            this.txtTotalTicket.Height = 0.1496063F;
            this.txtTotalTicket.Left = 4.268504F;
            this.txtTotalTicket.MultiLine = false;
            this.txtTotalTicket.Name = "txtTotalTicket";
            this.txtTotalTicket.OutputFormat = resources.GetString("txtTotalTicket.OutputFormat");
            this.txtTotalTicket.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTotalTicket.Text = "合計_チケット売上";
            this.txtTotalTicket.Top = 2.736614F;
            this.txtTotalTicket.Width = 0.9893701F;
            // 
            // lblTotalBarclay
            // 
            this.lblTotalBarclay.Height = 0.1496063F;
            this.lblTotalBarclay.HyperLink = null;
            this.lblTotalBarclay.Left = 5.312205F;
            this.lblTotalBarclay.MultiLine = false;
            this.lblTotalBarclay.Name = "lblTotalBarclay";
            this.lblTotalBarclay.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTotalBarclay.Text = "⑮バークレー";
            this.lblTotalBarclay.Top = 2.309055F;
            this.lblTotalBarclay.Width = 1.159449F;
            // 
            // txtTotalBarclay
            // 
            this.txtTotalBarclay.CanGrow = false;
            this.txtTotalBarclay.DataField = "合計_バークレー売上";
            this.txtTotalBarclay.Height = 0.1496063F;
            this.txtTotalBarclay.Left = 5.312205F;
            this.txtTotalBarclay.MultiLine = false;
            this.txtTotalBarclay.Name = "txtTotalBarclay";
            this.txtTotalBarclay.OutputFormat = resources.GetString("txtTotalBarclay.OutputFormat");
            this.txtTotalBarclay.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTotalBarclay.Text = "合計_バークレー売上";
            this.txtTotalBarclay.Top = 2.736614F;
            this.txtTotalBarclay.Width = 1.144488F;
            // 
            // lblTotalTotal
            // 
            this.lblTotalTotal.Height = 0.1496063F;
            this.lblTotalTotal.HyperLink = null;
            this.lblTotalTotal.Left = 6.511024F;
            this.lblTotalTotal.MultiLine = false;
            this.lblTotalTotal.Name = "lblTotalTotal";
            this.lblTotalTotal.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.lblTotalTotal.Text = "本日実質総売上";
            this.lblTotalTotal.Top = 2.309055F;
            this.lblTotalTotal.Width = 1.085827F;
            // 
            // label1
            // 
            this.label1.Height = 0.1496063F;
            this.label1.HyperLink = null;
            this.label1.Left = 6.511024F;
            this.label1.MultiLine = false;
            this.label1.Name = "label1";
            this.label1.Style = "font-size: 9.75pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap" +
    "";
            this.label1.Text = "⑫-⑬+⑮=";
            this.label1.Top = 2.522835F;
            this.label1.Width = 1.070866F;
            // 
            // txtTotalTotal
            // 
            this.txtTotalTotal.CanGrow = false;
            this.txtTotalTotal.DataField = "合計_実質総売上";
            this.txtTotalTotal.Height = 0.1496063F;
            this.txtTotalTotal.Left = 6.511024F;
            this.txtTotalTotal.MultiLine = false;
            this.txtTotalTotal.Name = "txtTotalTotal";
            this.txtTotalTotal.OutputFormat = resources.GetString("txtTotalTotal.OutputFormat");
            this.txtTotalTotal.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTotalTotal.Text = "合計_実質総売上";
            this.txtTotalTotal.Top = 2.736614F;
            this.txtTotalTotal.Width = 1.070866F;
            // 
            // lblKinsyuTable
            // 
            this.lblKinsyuTable.Height = 0.1850394F;
            this.lblKinsyuTable.HyperLink = null;
            this.lblKinsyuTable.Left = 1.006108F;
            this.lblKinsyuTable.MultiLine = false;
            this.lblKinsyuTable.Name = "lblKinsyuTable";
            this.lblKinsyuTable.Style = "font-size: 12pt; text-align: center; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblKinsyuTable.Text = "金銭明細表";
            this.lblKinsyuTable.Top = 3.035433F;
            this.lblKinsyuTable.Width = 0.9138891F;
            // 
            // shapeBill
            // 
            this.shapeBill.Height = 0.8888891F;
            this.shapeBill.Left = 0.2917323F;
            this.shapeBill.Name = "shapeBill";
            this.shapeBill.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeBill.Top = 3.268898F;
            this.shapeBill.Width = 2.342531F;
            // 
            // lineBill5k
            // 
            this.lineBill5k.Height = 0F;
            this.lineBill5k.Left = 0.2917323F;
            this.lineBill5k.LineWeight = 1F;
            this.lineBill5k.Name = "lineBill5k";
            this.lineBill5k.Top = 3.490945F;
            this.lineBill5k.Width = 2.342532F;
            this.lineBill5k.X1 = 0.2917323F;
            this.lineBill5k.X2 = 2.634264F;
            this.lineBill5k.Y1 = 3.490945F;
            this.lineBill5k.Y2 = 3.490945F;
            // 
            // lineBill2k
            // 
            this.lineBill2k.Height = 0F;
            this.lineBill2k.Left = 0.2917323F;
            this.lineBill2k.LineWeight = 1F;
            this.lineBill2k.Name = "lineBill2k";
            this.lineBill2k.Top = 3.713386F;
            this.lineBill2k.Width = 2.342532F;
            this.lineBill2k.X1 = 0.2917323F;
            this.lineBill2k.X2 = 2.634264F;
            this.lineBill2k.Y1 = 3.713386F;
            this.lineBill2k.Y2 = 3.713386F;
            // 
            // lineBill1k
            // 
            this.lineBill1k.Height = 0F;
            this.lineBill1k.Left = 0.2917323F;
            this.lineBill1k.LineWeight = 1F;
            this.lineBill1k.Name = "lineBill1k";
            this.lineBill1k.Top = 3.935433F;
            this.lineBill1k.Width = 2.342532F;
            this.lineBill1k.X1 = 0.2917323F;
            this.lineBill1k.X2 = 2.634264F;
            this.lineBill1k.Y1 = 3.935433F;
            this.lineBill1k.Y2 = 3.935433F;
            // 
            // lblBill10k
            // 
            this.lblBill10k.Height = 0.1574803F;
            this.lblBill10k.HyperLink = null;
            this.lblBill10k.Left = 0.3339019F;
            this.lblBill10k.MultiLine = false;
            this.lblBill10k.Name = "lblBill10k";
            this.lblBill10k.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblBill10k.Text = "10,000 ×";
            this.lblBill10k.Top = 3.307874F;
            this.lblBill10k.Width = 0.6416668F;
            // 
            // lblBill5k
            // 
            this.lblBill5k.Height = 0.1574803F;
            this.lblBill5k.HyperLink = null;
            this.lblBill5k.Left = 0.3339019F;
            this.lblBill5k.MultiLine = false;
            this.lblBill5k.Name = "lblBill5k";
            this.lblBill5k.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblBill5k.Text = "5,000 ×";
            this.lblBill5k.Top = 3.529921F;
            this.lblBill5k.Width = 0.6416668F;
            // 
            // lblBill2k
            // 
            this.lblBill2k.Height = 0.1574803F;
            this.lblBill2k.HyperLink = null;
            this.lblBill2k.Left = 0.3339019F;
            this.lblBill2k.MultiLine = false;
            this.lblBill2k.Name = "lblBill2k";
            this.lblBill2k.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblBill2k.Text = "2,000 ×";
            this.lblBill2k.Top = 3.752362F;
            this.lblBill2k.Width = 0.6416668F;
            // 
            // lblBill1k
            // 
            this.lblBill1k.Height = 0.1574803F;
            this.lblBill1k.HyperLink = null;
            this.lblBill1k.Left = 0.3339019F;
            this.lblBill1k.MultiLine = false;
            this.lblBill1k.Name = "lblBill1k";
            this.lblBill1k.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblBill1k.Text = "1,000 ×";
            this.lblBill1k.Top = 3.97441F;
            this.lblBill1k.Width = 0.6416668F;
            // 
            // txtBill10k
            // 
            this.txtBill10k.CanGrow = false;
            this.txtBill10k.DataField = "金銭内訳_10000円";
            this.txtBill10k.Height = 0.1574803F;
            this.txtBill10k.Left = 1.06676F;
            this.txtBill10k.MultiLine = false;
            this.txtBill10k.Name = "txtBill10k";
            this.txtBill10k.OutputFormat = resources.GetString("txtBill10k.OutputFormat");
            this.txtBill10k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtBill10k.Text = "金銭内訳_10000円";
            this.txtBill10k.Top = 3.307874F;
            this.txtBill10k.Width = 0.3937008F;
            // 
            // txtBill5k
            // 
            this.txtBill5k.CanGrow = false;
            this.txtBill5k.DataField = "金銭内訳_5000円";
            this.txtBill5k.Height = 0.1574803F;
            this.txtBill5k.Left = 1.06676F;
            this.txtBill5k.MultiLine = false;
            this.txtBill5k.Name = "txtBill5k";
            this.txtBill5k.OutputFormat = resources.GetString("txtBill5k.OutputFormat");
            this.txtBill5k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtBill5k.Text = "金銭内訳_5000円";
            this.txtBill5k.Top = 3.529921F;
            this.txtBill5k.Width = 0.3937008F;
            // 
            // txtBill2k
            // 
            this.txtBill2k.CanGrow = false;
            this.txtBill2k.DataField = "金銭内訳_2000円";
            this.txtBill2k.Height = 0.1574803F;
            this.txtBill2k.Left = 1.06676F;
            this.txtBill2k.MultiLine = false;
            this.txtBill2k.Name = "txtBill2k";
            this.txtBill2k.OutputFormat = resources.GetString("txtBill2k.OutputFormat");
            this.txtBill2k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtBill2k.Text = "金銭内訳_2000円";
            this.txtBill2k.Top = 3.752362F;
            this.txtBill2k.Width = 0.3937008F;
            // 
            // txtBill1k
            // 
            this.txtBill1k.CanGrow = false;
            this.txtBill1k.DataField = "金銭内訳_1000円";
            this.txtBill1k.Height = 0.1574803F;
            this.txtBill1k.Left = 1.06676F;
            this.txtBill1k.MultiLine = false;
            this.txtBill1k.Name = "txtBill1k";
            this.txtBill1k.OutputFormat = resources.GetString("txtBill1k.OutputFormat");
            this.txtBill1k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtBill1k.Text = "金銭内訳_1000円";
            this.txtBill1k.Top = 3.97441F;
            this.txtBill1k.Width = 0.3937008F;
            // 
            // shapeCoin
            // 
            this.shapeCoin.Height = 1.333334F;
            this.shapeCoin.Left = 2.707098F;
            this.shapeCoin.Name = "shapeCoin";
            this.shapeCoin.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeCoin.Top = 3.055118F;
            this.shapeCoin.Width = 2.17324F;
            // 
            // lineCoin100
            // 
            this.lineCoin100.Height = 0F;
            this.lineCoin100.Left = 2.707098F;
            this.lineCoin100.LineWeight = 1F;
            this.lineCoin100.Name = "lineCoin100";
            this.lineCoin100.Top = 3.277166F;
            this.lineCoin100.Width = 2.17324F;
            this.lineCoin100.X1 = 2.707098F;
            this.lineCoin100.X2 = 4.880338F;
            this.lineCoin100.Y1 = 3.277166F;
            this.lineCoin100.Y2 = 3.277166F;
            // 
            // lineCoin50
            // 
            this.lineCoin50.Height = 0F;
            this.lineCoin50.Left = 2.707098F;
            this.lineCoin50.LineWeight = 1F;
            this.lineCoin50.Name = "lineCoin50";
            this.lineCoin50.Top = 3.499606F;
            this.lineCoin50.Width = 2.17324F;
            this.lineCoin50.X1 = 2.707098F;
            this.lineCoin50.X2 = 4.880338F;
            this.lineCoin50.Y1 = 3.499606F;
            this.lineCoin50.Y2 = 3.499606F;
            // 
            // lineCoin10
            // 
            this.lineCoin10.Height = 0F;
            this.lineCoin10.Left = 2.707098F;
            this.lineCoin10.LineWeight = 1F;
            this.lineCoin10.Name = "lineCoin10";
            this.lineCoin10.Top = 3.721654F;
            this.lineCoin10.Width = 2.17324F;
            this.lineCoin10.X1 = 2.707098F;
            this.lineCoin10.X2 = 4.880338F;
            this.lineCoin10.Y1 = 3.721654F;
            this.lineCoin10.Y2 = 3.721654F;
            // 
            // lineCoin5
            // 
            this.lineCoin5.Height = 0F;
            this.lineCoin5.Left = 2.707098F;
            this.lineCoin5.LineWeight = 1F;
            this.lineCoin5.Name = "lineCoin5";
            this.lineCoin5.Top = 3.944094F;
            this.lineCoin5.Width = 2.17324F;
            this.lineCoin5.X1 = 2.707098F;
            this.lineCoin5.X2 = 4.880338F;
            this.lineCoin5.Y1 = 3.944094F;
            this.lineCoin5.Y2 = 3.944094F;
            // 
            // lineCoin1
            // 
            this.lineCoin1.Height = 0F;
            this.lineCoin1.Left = 2.707098F;
            this.lineCoin1.LineWeight = 1F;
            this.lineCoin1.Name = "lineCoin1";
            this.lineCoin1.Top = 4.166142F;
            this.lineCoin1.Width = 2.17324F;
            this.lineCoin1.X1 = 2.707098F;
            this.lineCoin1.X2 = 4.880338F;
            this.lineCoin1.Y1 = 4.166142F;
            this.lineCoin1.Y2 = 4.166142F;
            // 
            // lblCoin500
            // 
            this.lblCoin500.Height = 0.1574803F;
            this.lblCoin500.HyperLink = null;
            this.lblCoin500.Left = 2.749268F;
            this.lblCoin500.MultiLine = false;
            this.lblCoin500.Name = "lblCoin500";
            this.lblCoin500.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblCoin500.Text = "500 ×";
            this.lblCoin500.Top = 3.094095F;
            this.lblCoin500.Width = 0.472441F;
            // 
            // txtCoin500
            // 
            this.txtCoin500.CanGrow = false;
            this.txtCoin500.DataField = "金銭内訳_500円";
            this.txtCoin500.Height = 0.1574803F;
            this.txtCoin500.Left = 3.312834F;
            this.txtCoin500.MultiLine = false;
            this.txtCoin500.Name = "txtCoin500";
            this.txtCoin500.OutputFormat = resources.GetString("txtCoin500.OutputFormat");
            this.txtCoin500.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCoin500.Text = "金銭内訳_500円";
            this.txtCoin500.Top = 3.094095F;
            this.txtCoin500.Width = 0.3937008F;
            // 
            // lblCoin100
            // 
            this.lblCoin100.Height = 0.1574803F;
            this.lblCoin100.HyperLink = null;
            this.lblCoin100.Left = 2.749213F;
            this.lblCoin100.MultiLine = false;
            this.lblCoin100.Name = "lblCoin100";
            this.lblCoin100.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblCoin100.Text = "100 ×";
            this.lblCoin100.Top = 3.316142F;
            this.lblCoin100.Width = 0.472441F;
            // 
            // lblCoin50
            // 
            this.lblCoin50.Height = 0.1574803F;
            this.lblCoin50.HyperLink = null;
            this.lblCoin50.Left = 2.749213F;
            this.lblCoin50.MultiLine = false;
            this.lblCoin50.Name = "lblCoin50";
            this.lblCoin50.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblCoin50.Text = "50 ×";
            this.lblCoin50.Top = 3.538189F;
            this.lblCoin50.Width = 0.472441F;
            // 
            // lblCoin10
            // 
            this.lblCoin10.Height = 0.1574803F;
            this.lblCoin10.HyperLink = null;
            this.lblCoin10.Left = 2.749213F;
            this.lblCoin10.MultiLine = false;
            this.lblCoin10.Name = "lblCoin10";
            this.lblCoin10.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblCoin10.Text = "10 ×";
            this.lblCoin10.Top = 3.76063F;
            this.lblCoin10.Width = 0.472441F;
            // 
            // lblCoin5
            // 
            this.lblCoin5.Height = 0.1574803F;
            this.lblCoin5.HyperLink = null;
            this.lblCoin5.Left = 2.749213F;
            this.lblCoin5.MultiLine = false;
            this.lblCoin5.Name = "lblCoin5";
            this.lblCoin5.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblCoin5.Text = "5 ×";
            this.lblCoin5.Top = 3.982677F;
            this.lblCoin5.Width = 0.472441F;
            // 
            // lblCoin1
            // 
            this.lblCoin1.Height = 0.1574803F;
            this.lblCoin1.HyperLink = null;
            this.lblCoin1.Left = 2.749213F;
            this.lblCoin1.MultiLine = false;
            this.lblCoin1.Name = "lblCoin1";
            this.lblCoin1.Style = "font-size: 10.5pt; text-align: right; vertical-align: middle; white-space: nowrap" +
    "; ddo-wrap-mode: nowrap";
            this.lblCoin1.Text = "1 ×";
            this.lblCoin1.Top = 4.205118F;
            this.lblCoin1.Width = 0.472441F;
            // 
            // txtCoin100
            // 
            this.txtCoin100.CanGrow = false;
            this.txtCoin100.DataField = "金銭内訳_100円";
            this.txtCoin100.Height = 0.1574803F;
            this.txtCoin100.Left = 3.312834F;
            this.txtCoin100.MultiLine = false;
            this.txtCoin100.Name = "txtCoin100";
            this.txtCoin100.OutputFormat = resources.GetString("txtCoin100.OutputFormat");
            this.txtCoin100.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCoin100.Text = "金銭内訳_100円";
            this.txtCoin100.Top = 3.316142F;
            this.txtCoin100.Width = 0.3937008F;
            // 
            // txtCoin50
            // 
            this.txtCoin50.CanGrow = false;
            this.txtCoin50.DataField = "金銭内訳_50円";
            this.txtCoin50.Height = 0.1574803F;
            this.txtCoin50.Left = 3.312834F;
            this.txtCoin50.MultiLine = false;
            this.txtCoin50.Name = "txtCoin50";
            this.txtCoin50.OutputFormat = resources.GetString("txtCoin50.OutputFormat");
            this.txtCoin50.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCoin50.Text = "金銭内訳_50円";
            this.txtCoin50.Top = 3.538189F;
            this.txtCoin50.Width = 0.3937008F;
            // 
            // txtCoin10
            // 
            this.txtCoin10.CanGrow = false;
            this.txtCoin10.DataField = "金銭内訳_10円";
            this.txtCoin10.Height = 0.1574803F;
            this.txtCoin10.Left = 3.312834F;
            this.txtCoin10.MultiLine = false;
            this.txtCoin10.Name = "txtCoin10";
            this.txtCoin10.OutputFormat = resources.GetString("txtCoin10.OutputFormat");
            this.txtCoin10.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCoin10.Text = "金銭内訳_10円";
            this.txtCoin10.Top = 3.76063F;
            this.txtCoin10.Width = 0.3937008F;
            // 
            // txtCoin5
            // 
            this.txtCoin5.CanGrow = false;
            this.txtCoin5.DataField = "金銭内訳_5円";
            this.txtCoin5.Height = 0.1574803F;
            this.txtCoin5.Left = 3.312834F;
            this.txtCoin5.MultiLine = false;
            this.txtCoin5.Name = "txtCoin5";
            this.txtCoin5.OutputFormat = resources.GetString("txtCoin5.OutputFormat");
            this.txtCoin5.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCoin5.Text = "金銭内訳_5円";
            this.txtCoin5.Top = 3.982677F;
            this.txtCoin5.Width = 0.3937008F;
            // 
            // txtCoin1
            // 
            this.txtCoin1.CanGrow = false;
            this.txtCoin1.DataField = "金銭内訳_1円";
            this.txtCoin1.Height = 0.1574803F;
            this.txtCoin1.Left = 3.312834F;
            this.txtCoin1.MultiLine = false;
            this.txtCoin1.Name = "txtCoin1";
            this.txtCoin1.OutputFormat = resources.GetString("txtCoin1.OutputFormat");
            this.txtCoin1.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtCoin1.Text = "金銭内訳_1円";
            this.txtCoin1.Top = 4.205118F;
            this.txtCoin1.Width = 0.3937008F;
            // 
            // shapeKogitte
            // 
            this.shapeKogitte.Height = 0.6665574F;
            this.shapeKogitte.Left = 4.953172F;
            this.shapeKogitte.Name = "shapeKogitte";
            this.shapeKogitte.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeKogitte.Top = 3.055118F;
            this.shapeKogitte.Width = 1.683432F;
            // 
            // lineKogitteTate
            // 
            this.lineKogitteTate.Height = 0.6665359F;
            this.lineKogitteTate.Left = 5.671484F;
            this.lineKogitteTate.LineWeight = 1F;
            this.lineKogitteTate.Name = "lineKogitteTate";
            this.lineKogitteTate.Top = 3.055118F;
            this.lineKogitteTate.Width = 0F;
            this.lineKogitteTate.X1 = 5.671484F;
            this.lineKogitteTate.X2 = 5.671484F;
            this.lineKogitteTate.Y1 = 3.055118F;
            this.lineKogitteTate.Y2 = 3.721654F;
            // 
            // lineRyosyusyo
            // 
            this.lineRyosyusyo.Height = 0F;
            this.lineRyosyusyo.Left = 4.955326F;
            this.lineRyosyusyo.LineWeight = 1F;
            this.lineRyosyusyo.Name = "lineRyosyusyo";
            this.lineRyosyusyo.Top = 3.277166F;
            this.lineRyosyusyo.Width = 2.66477F;
            this.lineRyosyusyo.X1 = 4.955326F;
            this.lineRyosyusyo.X2 = 7.620096F;
            this.lineRyosyusyo.Y1 = 3.277166F;
            this.lineRyosyusyo.Y2 = 3.277166F;
            // 
            // lineKinsyuTotal
            // 
            this.lineKinsyuTotal.Height = 0F;
            this.lineKinsyuTotal.Left = 4.955326F;
            this.lineKinsyuTotal.LineWeight = 1F;
            this.lineKinsyuTotal.Name = "lineKinsyuTotal";
            this.lineKinsyuTotal.Top = 3.499606F;
            this.lineKinsyuTotal.Width = 2.66477F;
            this.lineKinsyuTotal.X1 = 4.955326F;
            this.lineKinsyuTotal.X2 = 7.620096F;
            this.lineKinsyuTotal.Y1 = 3.499606F;
            this.lineKinsyuTotal.Y2 = 3.499606F;
            // 
            // lblKogitte
            // 
            this.lblKogitte.Height = 0.1574803F;
            this.lblKogitte.HyperLink = null;
            this.lblKogitte.Left = 4.976795F;
            this.lblKogitte.MultiLine = false;
            this.lblKogitte.Name = "lblKogitte";
            this.lblKogitte.Style = "font-size: 9.75pt; text-align: center; vertical-align: middle; white-space: nowra" +
    "p; ddo-wrap-mode: nowrap";
            this.lblKogitte.Text = "小切手";
            this.lblKogitte.Top = 3.094095F;
            this.lblKogitte.Width = 0.6625328F;
            // 
            // lblRyosyusyo
            // 
            this.lblRyosyusyo.Height = 0.1574803F;
            this.lblRyosyusyo.HyperLink = null;
            this.lblRyosyusyo.Left = 4.976585F;
            this.lblRyosyusyo.MultiLine = false;
            this.lblRyosyusyo.Name = "lblRyosyusyo";
            this.lblRyosyusyo.Style = "font-size: 9.75pt; text-align: center; vertical-align: middle; white-space: nowra" +
    "p; ddo-wrap-mode: nowrap";
            this.lblRyosyusyo.Text = "領収書";
            this.lblRyosyusyo.Top = 3.316142F;
            this.lblRyosyusyo.Width = 0.6625327F;
            // 
            // lblKinsyuTotal
            // 
            this.lblKinsyuTotal.Height = 0.1574803F;
            this.lblKinsyuTotal.HyperLink = null;
            this.lblKinsyuTotal.Left = 4.976585F;
            this.lblKinsyuTotal.MultiLine = false;
            this.lblKinsyuTotal.Name = "lblKinsyuTotal";
            this.lblKinsyuTotal.Style = "font-size: 9.75pt; text-align: center; vertical-align: middle; white-space: nowra" +
    "p; ddo-wrap-mode: nowrap";
            this.lblKinsyuTotal.Text = "合  計";
            this.lblKinsyuTotal.Top = 3.538189F;
            this.lblKinsyuTotal.Width = 0.6625327F;
            // 
            // txtKogitte
            // 
            this.txtKogitte.CanGrow = false;
            this.txtKogitte.DataField = "金銭内訳_小切手";
            this.txtKogitte.Height = 0.1574803F;
            this.txtKogitte.Left = 5.730519F;
            this.txtKogitte.MultiLine = false;
            this.txtKogitte.Name = "txtKogitte";
            this.txtKogitte.OutputFormat = resources.GetString("txtKogitte.OutputFormat");
            this.txtKogitte.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtKogitte.Text = "金銭内訳_小切手";
            this.txtKogitte.Top = 3.094095F;
            this.txtKogitte.Width = 0.874016F;
            // 
            // txtRyosyusyo
            // 
            this.txtRyosyusyo.CanGrow = false;
            this.txtRyosyusyo.DataField = "金銭内訳_領収書";
            this.txtRyosyusyo.Height = 0.1574803F;
            this.txtRyosyusyo.Left = 5.730519F;
            this.txtRyosyusyo.MultiLine = false;
            this.txtRyosyusyo.Name = "txtRyosyusyo";
            this.txtRyosyusyo.OutputFormat = resources.GetString("txtRyosyusyo.OutputFormat");
            this.txtRyosyusyo.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtRyosyusyo.Text = "金銭内訳_領収書";
            this.txtRyosyusyo.Top = 3.316142F;
            this.txtRyosyusyo.Width = 0.874016F;
            // 
            // txtKinsyuTotal
            // 
            this.txtKinsyuTotal.CanGrow = false;
            this.txtKinsyuTotal.DataField = "金銭内訳_合計";
            this.txtKinsyuTotal.Height = 0.1574803F;
            this.txtKinsyuTotal.Left = 5.730519F;
            this.txtKinsyuTotal.MultiLine = false;
            this.txtKinsyuTotal.Name = "txtKinsyuTotal";
            this.txtKinsyuTotal.OutputFormat = resources.GetString("txtKinsyuTotal.OutputFormat");
            this.txtKinsyuTotal.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtKinsyuTotal.Text = "金銭内訳_合計";
            this.txtKinsyuTotal.Top = 3.538189F;
            this.txtKinsyuTotal.Width = 0.874016F;
            // 
            // lineKajyokinUnder
            // 
            this.lineKajyokinUnder.Height = 0F;
            this.lineKajyokinUnder.Left = 6.64579F;
            this.lineKajyokinUnder.LineWeight = 1F;
            this.lineKajyokinUnder.Name = "lineKajyokinUnder";
            this.lineKajyokinUnder.Top = 3.718504F;
            this.lineKajyokinUnder.Width = 0.9743061F;
            this.lineKajyokinUnder.X1 = 6.64579F;
            this.lineKajyokinUnder.X2 = 7.620096F;
            this.lineKajyokinUnder.Y1 = 3.718504F;
            this.lineKajyokinUnder.Y2 = 3.718504F;
            // 
            // lineKajyokinRight
            // 
            this.lineKajyokinRight.Height = 0.441339F;
            this.lineKajyokinRight.Left = 7.620079F;
            this.lineKajyokinRight.LineWeight = 1F;
            this.lineKajyokinRight.Name = "lineKajyokinRight";
            this.lineKajyokinRight.Top = 3.280315F;
            this.lineKajyokinRight.Width = 0F;
            this.lineKajyokinRight.X1 = 7.620079F;
            this.lineKajyokinRight.X2 = 7.620079F;
            this.lineKajyokinRight.Y1 = 3.280315F;
            this.lineKajyokinRight.Y2 = 3.721654F;
            // 
            // lblKajokin
            // 
            this.lblKajokin.Height = 0.1574803F;
            this.lblKajokin.HyperLink = null;
            this.lblKajokin.Left = 6.676192F;
            this.lblKajokin.MultiLine = false;
            this.lblKajokin.Name = "lblKajokin";
            this.lblKajokin.Style = "font-size: 9.75pt; text-align: center; vertical-align: middle; white-space: nowra" +
    "p; ddo-wrap-mode: nowrap";
            this.lblKajokin.Text = "過剰金";
            this.lblKajokin.Top = 3.316142F;
            this.lblKajokin.Width = 0.9054189F;
            // 
            // txtKajokin
            // 
            this.txtKajokin.CanGrow = false;
            this.txtKajokin.DataField = "金銭内訳_過剰金";
            this.txtKajokin.Height = 0.1574803F;
            this.txtKajokin.Left = 6.707595F;
            this.txtKajokin.MultiLine = false;
            this.txtKajokin.Name = "txtKajokin";
            this.txtKajokin.OutputFormat = resources.GetString("txtKajokin.OutputFormat");
            this.txtKajokin.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtKajokin.Text = "金銭内訳_過剰金";
            this.txtKajokin.Top = 3.538189F;
            this.txtKajokin.Width = 0.874016F;
            // 
            // shapeTicketFooter
            // 
            this.shapeTicketFooter.Height = 0.613889F;
            this.shapeTicketFooter.Left = 0.4472224F;
            this.shapeTicketFooter.Name = "shapeTicketFooter";
            this.shapeTicketFooter.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(10F, null, null, null, null);
            this.shapeTicketFooter.Top = 4.452362F;
            this.shapeTicketFooter.Width = 6.708335F;
            // 
            // lineTicketFooterTate
            // 
            this.lineTicketFooterTate.Height = 0.61378F;
            this.lineTicketFooterTate.Left = 3.80139F;
            this.lineTicketFooterTate.LineWeight = 1F;
            this.lineTicketFooterTate.Name = "lineTicketFooterTate";
            this.lineTicketFooterTate.Top = 4.452362F;
            this.lineTicketFooterTate.Width = 0F;
            this.lineTicketFooterTate.X1 = 3.80139F;
            this.lineTicketFooterTate.X2 = 3.80139F;
            this.lineTicketFooterTate.Y1 = 4.452362F;
            this.lineTicketFooterTate.Y2 = 5.066142F;
            // 
            // lblTicketFooterTitle
            // 
            this.lblTicketFooterTitle.Height = 0.1496063F;
            this.lblTicketFooterTitle.HyperLink = null;
            this.lblTicketFooterTitle.Left = 0.4861112F;
            this.lblTicketFooterTitle.MultiLine = false;
            this.lblTicketFooterTitle.Name = "lblTicketFooterTitle";
            this.lblTicketFooterTitle.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblTicketFooterTitle.Text = "チケット";
            this.lblTicketFooterTitle.Top = 4.502362F;
            this.lblTicketFooterTitle.Width = 0.4666668F;
            // 
            // lblTicketFooterKake
            // 
            this.lblTicketFooterKake.Height = 0.1496063F;
            this.lblTicketFooterKake.HyperLink = null;
            this.lblTicketFooterKake.Left = 1.127778F;
            this.lblTicketFooterKake.MultiLine = false;
            this.lblTicketFooterKake.Name = "lblTicketFooterKake";
            this.lblTicketFooterKake.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblTicketFooterKake.Text = "チケット ×";
            this.lblTicketFooterKake.Top = 4.677559F;
            this.lblTicketFooterKake.Width = 0.7000002F;
            // 
            // txtTicketMai
            // 
            this.txtTicketMai.CanGrow = false;
            this.txtTicketMai.DataField = "消費チケ枚数_チケット";
            this.txtTicketMai.Height = 0.1496063F;
            this.txtTicketMai.Left = 1.886112F;
            this.txtTicketMai.MultiLine = false;
            this.txtTicketMai.Name = "txtTicketMai";
            this.txtTicketMai.OutputFormat = resources.GetString("txtTicketMai.OutputFormat");
            this.txtTicketMai.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtTicketMai.Text = "消費チケ枚数_チケット";
            this.txtTicketMai.Top = 4.677559F;
            this.txtTicketMai.Width = 0.5381945F;
            // 
            // lblTicketMai
            // 
            this.lblTicketMai.Height = 0.1496063F;
            this.lblTicketMai.HyperLink = null;
            this.lblTicketMai.Left = 2.469445F;
            this.lblTicketMai.MultiLine = false;
            this.lblTicketMai.Name = "lblTicketMai";
            this.lblTicketMai.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblTicketMai.Text = "枚";
            this.lblTicketMai.Top = 4.677559F;
            this.lblTicketMai.Width = 0.1750001F;
            // 
            // lblBonusFooterKake
            // 
            this.lblBonusFooterKake.Height = 0.1496063F;
            this.lblBonusFooterKake.HyperLink = null;
            this.lblBonusFooterKake.Left = 1.127953F;
            this.lblBonusFooterKake.MultiLine = false;
            this.lblBonusFooterKake.Name = "lblBonusFooterKake";
            this.lblBonusFooterKake.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBonusFooterKake.Text = "ボーナス ×";
            this.lblBonusFooterKake.Top = 4.852362F;
            this.lblBonusFooterKake.Width = 0.7F;
            // 
            // txtBonusMai
            // 
            this.txtBonusMai.CanGrow = false;
            this.txtBonusMai.DataField = "消費チケ枚数_ボーナス";
            this.txtBonusMai.Height = 0.1496063F;
            this.txtBonusMai.Left = 1.886221F;
            this.txtBonusMai.MultiLine = false;
            this.txtBonusMai.Name = "txtBonusMai";
            this.txtBonusMai.OutputFormat = resources.GetString("txtBonusMai.OutputFormat");
            this.txtBonusMai.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtBonusMai.Text = "消費チケ枚数_ボーナス";
            this.txtBonusMai.Top = 4.852362F;
            this.txtBonusMai.Width = 0.5381945F;
            // 
            // lblBonusMai
            // 
            this.lblBonusMai.Height = 0.1496063F;
            this.lblBonusMai.HyperLink = null;
            this.lblBonusMai.Left = 2.469292F;
            this.lblBonusMai.MultiLine = false;
            this.lblBonusMai.Name = "lblBonusMai";
            this.lblBonusMai.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBonusMai.Text = "枚";
            this.lblBonusMai.Top = 4.852362F;
            this.lblBonusMai.Width = 0.1750001F;
            // 
            // lblBarclayFooterTitle
            // 
            this.lblBarclayFooterTitle.Height = 0.1496063F;
            this.lblBarclayFooterTitle.HyperLink = null;
            this.lblBarclayFooterTitle.Left = 3.947223F;
            this.lblBarclayFooterTitle.MultiLine = false;
            this.lblBarclayFooterTitle.Name = "lblBarclayFooterTitle";
            this.lblBarclayFooterTitle.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBarclayFooterTitle.Text = "バークレーヴァウチャーズチケット（弁当用）";
            this.lblBarclayFooterTitle.Top = 4.502362F;
            this.lblBarclayFooterTitle.Width = 2.488889F;
            // 
            // lblBV300Kake
            // 
            this.lblBV300Kake.Height = 0.1496063F;
            this.lblBV300Kake.HyperLink = null;
            this.lblBV300Kake.Left = 4.822224F;
            this.lblBV300Kake.MultiLine = false;
            this.lblBV300Kake.Name = "lblBV300Kake";
            this.lblBV300Kake.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBV300Kake.Text = "300 ×";
            this.lblBV300Kake.Top = 4.677559F;
            this.lblBV300Kake.Width = 0.4083335F;
            // 
            // txtBV300Mai
            // 
            this.txtBV300Mai.CanGrow = false;
            this.txtBV300Mai.DataField = "消費チケ枚数_ＢＶ300";
            this.txtBV300Mai.Height = 0.1496063F;
            this.txtBV300Mai.Left = 5.250001F;
            this.txtBV300Mai.MultiLine = false;
            this.txtBV300Mai.Name = "txtBV300Mai";
            this.txtBV300Mai.OutputFormat = resources.GetString("txtBV300Mai.OutputFormat");
            this.txtBV300Mai.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtBV300Mai.Text = "消費チケ枚数_ＢＶ300";
            this.txtBV300Mai.Top = 4.677559F;
            this.txtBV300Mai.Width = 0.4277779F;
            // 
            // lblBV300Equal
            // 
            this.lblBV300Equal.Height = 0.1496063F;
            this.lblBV300Equal.HyperLink = null;
            this.lblBV300Equal.Left = 5.736113F;
            this.lblBV300Equal.MultiLine = false;
            this.lblBV300Equal.Name = "lblBV300Equal";
            this.lblBV300Equal.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBV300Equal.Text = "＝";
            this.lblBV300Equal.Top = 4.677559F;
            this.lblBV300Equal.Width = 0.1750001F;
            // 
            // txtBV300Kingaku
            // 
            this.txtBV300Kingaku.CanGrow = false;
            this.txtBV300Kingaku.DataField = "消費チケ金額_ＢＶ300";
            this.txtBV300Kingaku.Height = 0.1496063F;
            this.txtBV300Kingaku.Left = 5.930557F;
            this.txtBV300Kingaku.MultiLine = false;
            this.txtBV300Kingaku.Name = "txtBV300Kingaku";
            this.txtBV300Kingaku.OutputFormat = resources.GetString("txtBV300Kingaku.OutputFormat");
            this.txtBV300Kingaku.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtBV300Kingaku.Text = "消費チケ金額_ＢＶ300";
            this.txtBV300Kingaku.Top = 4.677559F;
            this.txtBV300Kingaku.Width = 0.6458335F;
            // 
            // lblBV200Kake
            // 
            this.lblBV200Kake.Height = 0.1496063F;
            this.lblBV200Kake.HyperLink = null;
            this.lblBV200Kake.Left = 4.822048F;
            this.lblBV200Kake.MultiLine = false;
            this.lblBV200Kake.Name = "lblBV200Kake";
            this.lblBV200Kake.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBV200Kake.Text = "200 ×";
            this.lblBV200Kake.Top = 4.852362F;
            this.lblBV200Kake.Width = 0.4083335F;
            // 
            // txtBV200Mai
            // 
            this.txtBV200Mai.CanGrow = false;
            this.txtBV200Mai.DataField = "消費チケ枚数_ＢＶ200";
            this.txtBV200Mai.Height = 0.1496063F;
            this.txtBV200Mai.Left = 5.25F;
            this.txtBV200Mai.MultiLine = false;
            this.txtBV200Mai.Name = "txtBV200Mai";
            this.txtBV200Mai.OutputFormat = resources.GetString("txtBV200Mai.OutputFormat");
            this.txtBV200Mai.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtBV200Mai.Text = "消費チケ枚数_ＢＶ200";
            this.txtBV200Mai.Top = 4.852362F;
            this.txtBV200Mai.Width = 0.4277779F;
            // 
            // lblBV200Equal
            // 
            this.lblBV200Equal.Height = 0.1496063F;
            this.lblBV200Equal.HyperLink = null;
            this.lblBV200Equal.Left = 5.73622F;
            this.lblBV200Equal.MultiLine = false;
            this.lblBV200Equal.Name = "lblBV200Equal";
            this.lblBV200Equal.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblBV200Equal.Text = "＝";
            this.lblBV200Equal.Top = 4.852362F;
            this.lblBV200Equal.Width = 0.1750001F;
            // 
            // txtBV200Kingaku
            // 
            this.txtBV200Kingaku.CanGrow = false;
            this.txtBV200Kingaku.DataField = "消費チケ金額_ＢＶ200";
            this.txtBV200Kingaku.Height = 0.1496063F;
            this.txtBV200Kingaku.Left = 5.930709F;
            this.txtBV200Kingaku.MultiLine = false;
            this.txtBV200Kingaku.Name = "txtBV200Kingaku";
            this.txtBV200Kingaku.OutputFormat = resources.GetString("txtBV200Kingaku.OutputFormat");
            this.txtBV200Kingaku.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtBV200Kingaku.Text = "消費チケ金額_ＢＶ200";
            this.txtBV200Kingaku.Top = 4.852362F;
            this.txtBV200Kingaku.Width = 0.6458334F;
            // 
            // lineGenkinUpper
            // 
            this.lineGenkinUpper.Height = 0F;
            this.lineGenkinUpper.Left = 0.2952756F;
            this.lineGenkinUpper.LineWeight = 1F;
            this.lineGenkinUpper.Name = "lineGenkinUpper";
            this.lineGenkinUpper.Top = 0.9740158F;
            this.lineGenkinUpper.Width = 7.32126F;
            this.lineGenkinUpper.X1 = 0.2952756F;
            this.lineGenkinUpper.X2 = 7.616536F;
            this.lineGenkinUpper.Y1 = 0.9740158F;
            this.lineGenkinUpper.Y2 = 0.9740158F;
            // 
            // lineKakeuriUpper
            // 
            this.lineKakeuriUpper.Height = 0F;
            this.lineKakeuriUpper.Left = 0.2952756F;
            this.lineKakeuriUpper.LineWeight = 1F;
            this.lineKakeuriUpper.Name = "lineKakeuriUpper";
            this.lineKakeuriUpper.Top = 1.479528F;
            this.lineKakeuriUpper.Width = 7.321259F;
            this.lineKakeuriUpper.X1 = 0.2952756F;
            this.lineKakeuriUpper.X2 = 7.616534F;
            this.lineKakeuriUpper.Y1 = 1.479528F;
            this.lineKakeuriUpper.Y2 = 1.479528F;
            // 
            // lblMai10k
            // 
            this.lblMai10k.Height = 0.1574803F;
            this.lblMai10k.HyperLink = null;
            this.lblMai10k.Left = 1.519516F;
            this.lblMai10k.MultiLine = false;
            this.lblMai10k.Name = "lblMai10k";
            this.lblMai10k.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai10k.Text = "枚 ＝";
            this.lblMai10k.Top = 3.307874F;
            this.lblMai10k.Width = 0.3149606F;
            // 
            // lblMai5k
            // 
            this.lblMai5k.Height = 0.1574803F;
            this.lblMai5k.HyperLink = null;
            this.lblMai5k.Left = 1.519516F;
            this.lblMai5k.MultiLine = false;
            this.lblMai5k.Name = "lblMai5k";
            this.lblMai5k.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai5k.Text = "枚 ＝";
            this.lblMai5k.Top = 3.529921F;
            this.lblMai5k.Width = 0.3149606F;
            // 
            // lblMai2k
            // 
            this.lblMai2k.Height = 0.1574803F;
            this.lblMai2k.HyperLink = null;
            this.lblMai2k.Left = 1.519516F;
            this.lblMai2k.MultiLine = false;
            this.lblMai2k.Name = "lblMai2k";
            this.lblMai2k.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai2k.Text = "枚 ＝";
            this.lblMai2k.Top = 3.752362F;
            this.lblMai2k.Width = 0.3149606F;
            // 
            // lblMai1k
            // 
            this.lblMai1k.Height = 0.1574803F;
            this.lblMai1k.HyperLink = null;
            this.lblMai1k.Left = 1.519516F;
            this.lblMai1k.MultiLine = false;
            this.lblMai1k.Name = "lblMai1k";
            this.lblMai1k.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai1k.Text = "枚 ＝";
            this.lblMai1k.Top = 3.97441F;
            this.lblMai1k.Width = 0.3149606F;
            // 
            // txtAmount10k
            // 
            this.txtAmount10k.CanGrow = false;
            this.txtAmount10k.DataField = "=金銭内訳_10000円*10000";
            this.txtAmount10k.Height = 0.1574803F;
            this.txtAmount10k.Left = 1.893531F;
            this.txtAmount10k.MultiLine = false;
            this.txtAmount10k.Name = "txtAmount10k";
            this.txtAmount10k.OutputFormat = resources.GetString("txtAmount10k.OutputFormat");
            this.txtAmount10k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount10k.Text = "金銭合計_10000円";
            this.txtAmount10k.Top = 3.307874F;
            this.txtAmount10k.Width = 0.7086614F;
            // 
            // txtAmount5k
            // 
            this.txtAmount5k.CanGrow = false;
            this.txtAmount5k.DataField = "=金銭内訳_5000円*5000";
            this.txtAmount5k.Height = 0.1574803F;
            this.txtAmount5k.Left = 1.893531F;
            this.txtAmount5k.MultiLine = false;
            this.txtAmount5k.Name = "txtAmount5k";
            this.txtAmount5k.OutputFormat = resources.GetString("txtAmount5k.OutputFormat");
            this.txtAmount5k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount5k.Text = "金銭合計_5000円";
            this.txtAmount5k.Top = 3.529921F;
            this.txtAmount5k.Width = 0.7086614F;
            // 
            // txtAmount2k
            // 
            this.txtAmount2k.CanGrow = false;
            this.txtAmount2k.DataField = "=金銭内訳_2000円*2000";
            this.txtAmount2k.Height = 0.1574803F;
            this.txtAmount2k.Left = 1.893531F;
            this.txtAmount2k.MultiLine = false;
            this.txtAmount2k.Name = "txtAmount2k";
            this.txtAmount2k.OutputFormat = resources.GetString("txtAmount2k.OutputFormat");
            this.txtAmount2k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount2k.Text = "金銭合計_2000円";
            this.txtAmount2k.Top = 3.752362F;
            this.txtAmount2k.Width = 0.7086614F;
            // 
            // txtAmount1k
            // 
            this.txtAmount1k.CanGrow = false;
            this.txtAmount1k.DataField = "=金銭内訳_1000円*1000";
            this.txtAmount1k.Height = 0.1574803F;
            this.txtAmount1k.Left = 1.893531F;
            this.txtAmount1k.MultiLine = false;
            this.txtAmount1k.Name = "txtAmount1k";
            this.txtAmount1k.OutputFormat = resources.GetString("txtAmount1k.OutputFormat");
            this.txtAmount1k.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount1k.Text = "金銭合計_1000円";
            this.txtAmount1k.Top = 3.97441F;
            this.txtAmount1k.Width = 0.7086614F;
            // 
            // lblMai500
            // 
            this.lblMai500.Height = 0.1574803F;
            this.lblMai500.HyperLink = null;
            this.lblMai500.Left = 3.76559F;
            this.lblMai500.MultiLine = false;
            this.lblMai500.Name = "lblMai500";
            this.lblMai500.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai500.Text = "枚 ＝";
            this.lblMai500.Top = 3.094095F;
            this.lblMai500.Width = 0.3149606F;
            // 
            // lblMai100
            // 
            this.lblMai100.Height = 0.1574803F;
            this.lblMai100.HyperLink = null;
            this.lblMai100.Left = 3.765749F;
            this.lblMai100.MultiLine = false;
            this.lblMai100.Name = "lblMai100";
            this.lblMai100.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai100.Text = "枚 ＝";
            this.lblMai100.Top = 3.316142F;
            this.lblMai100.Width = 0.3149606F;
            // 
            // lblMai50
            // 
            this.lblMai50.Height = 0.1574803F;
            this.lblMai50.HyperLink = null;
            this.lblMai50.Left = 3.765749F;
            this.lblMai50.MultiLine = false;
            this.lblMai50.Name = "lblMai50";
            this.lblMai50.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai50.Text = "枚 ＝";
            this.lblMai50.Top = 3.538189F;
            this.lblMai50.Width = 0.3149606F;
            // 
            // lblMai10
            // 
            this.lblMai10.Height = 0.1574803F;
            this.lblMai10.HyperLink = null;
            this.lblMai10.Left = 3.765749F;
            this.lblMai10.MultiLine = false;
            this.lblMai10.Name = "lblMai10";
            this.lblMai10.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai10.Text = "枚 ＝";
            this.lblMai10.Top = 3.76063F;
            this.lblMai10.Width = 0.3149606F;
            // 
            // lblMai5
            // 
            this.lblMai5.Height = 0.1574803F;
            this.lblMai5.HyperLink = null;
            this.lblMai5.Left = 3.765749F;
            this.lblMai5.MultiLine = false;
            this.lblMai5.Name = "lblMai5";
            this.lblMai5.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai5.Text = "枚 ＝";
            this.lblMai5.Top = 3.982677F;
            this.lblMai5.Width = 0.3149606F;
            // 
            // lblMai1
            // 
            this.lblMai1.Height = 0.1574803F;
            this.lblMai1.HyperLink = null;
            this.lblMai1.Left = 3.765749F;
            this.lblMai1.MultiLine = false;
            this.lblMai1.Name = "lblMai1";
            this.lblMai1.Style = "font-size: 9pt; text-align: left; vertical-align: bottom; white-space: nowrap; dd" +
    "o-char-set: 1; ddo-wrap-mode: nowrap";
            this.lblMai1.Text = "枚 ＝";
            this.lblMai1.Top = 4.205118F;
            this.lblMai1.Width = 0.3149606F;
            // 
            // txtAmount500
            // 
            this.txtAmount500.CanGrow = false;
            this.txtAmount500.DataField = "=金銭内訳_500円*500";
            this.txtAmount500.Height = 0.1574803F;
            this.txtAmount500.Left = 4.139606F;
            this.txtAmount500.MultiLine = false;
            this.txtAmount500.Name = "txtAmount500";
            this.txtAmount500.OutputFormat = resources.GetString("txtAmount500.OutputFormat");
            this.txtAmount500.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount500.Text = "金銭合計_500円";
            this.txtAmount500.Top = 3.094095F;
            this.txtAmount500.Width = 0.7086614F;
            // 
            // txtAmount100
            // 
            this.txtAmount100.CanGrow = false;
            this.txtAmount100.DataField = "=金銭内訳_100円*100";
            this.txtAmount100.Height = 0.1574803F;
            this.txtAmount100.Left = 4.139764F;
            this.txtAmount100.MultiLine = false;
            this.txtAmount100.Name = "txtAmount100";
            this.txtAmount100.OutputFormat = resources.GetString("txtAmount100.OutputFormat");
            this.txtAmount100.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount100.Text = "金銭合計_100円";
            this.txtAmount100.Top = 3.316142F;
            this.txtAmount100.Width = 0.7086614F;
            // 
            // txtAmount50
            // 
            this.txtAmount50.CanGrow = false;
            this.txtAmount50.DataField = "=金銭内訳_50円*50";
            this.txtAmount50.Height = 0.1574803F;
            this.txtAmount50.Left = 4.139764F;
            this.txtAmount50.MultiLine = false;
            this.txtAmount50.Name = "txtAmount50";
            this.txtAmount50.OutputFormat = resources.GetString("txtAmount50.OutputFormat");
            this.txtAmount50.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount50.Text = "金銭合計_50円";
            this.txtAmount50.Top = 3.538189F;
            this.txtAmount50.Width = 0.7086614F;
            // 
            // txtAmount10
            // 
            this.txtAmount10.CanGrow = false;
            this.txtAmount10.DataField = "=金銭内訳_10円*10";
            this.txtAmount10.Height = 0.1574803F;
            this.txtAmount10.Left = 4.139764F;
            this.txtAmount10.MultiLine = false;
            this.txtAmount10.Name = "txtAmount10";
            this.txtAmount10.OutputFormat = resources.GetString("txtAmount10.OutputFormat");
            this.txtAmount10.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount10.Text = "金銭合計_10円";
            this.txtAmount10.Top = 3.76063F;
            this.txtAmount10.Width = 0.7086614F;
            // 
            // txtAmount5
            // 
            this.txtAmount5.CanGrow = false;
            this.txtAmount5.DataField = "=金銭内訳_5円*5";
            this.txtAmount5.Height = 0.1574803F;
            this.txtAmount5.Left = 4.139764F;
            this.txtAmount5.MultiLine = false;
            this.txtAmount5.Name = "txtAmount5";
            this.txtAmount5.OutputFormat = resources.GetString("txtAmount5.OutputFormat");
            this.txtAmount5.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount5.Text = "金銭合計_5円";
            this.txtAmount5.Top = 3.982677F;
            this.txtAmount5.Width = 0.7086614F;
            // 
            // txtAmount1
            // 
            this.txtAmount1.CanGrow = false;
            this.txtAmount1.DataField = "金銭内訳_1円";
            this.txtAmount1.Height = 0.1574803F;
            this.txtAmount1.Left = 4.139764F;
            this.txtAmount1.MultiLine = false;
            this.txtAmount1.Name = "txtAmount1";
            this.txtAmount1.OutputFormat = resources.GetString("txtAmount1.OutputFormat");
            this.txtAmount1.Style = "font-family: ＭＳ Ｐ明朝; font-size: 10.5pt; text-align: right; vertical-align: middle" +
    "; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.txtAmount1.Text = "金銭合計_1円";
            this.txtAmount1.Top = 4.205118F;
            this.txtAmount1.Width = 0.7086614F;
            // 
            // lblPayTotal
            // 
            this.lblPayTotal.Height = 0.1496063F;
            this.lblPayTotal.HyperLink = null;
            this.lblPayTotal.Left = 0.3149607F;
            this.lblPayTotal.MultiLine = false;
            this.lblPayTotal.Name = "lblPayTotal";
            this.lblPayTotal.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblPayTotal.Text = "電子決済合計";
            this.lblPayTotal.Top = 2.016929F;
            this.lblPayTotal.Width = 0.9165354F;
            // 
            // txtPayUri
            // 
            this.txtPayUri.CanGrow = false;
            this.txtPayUri.DataField = "E_売上合計";
            this.txtPayUri.Height = 0.1496063F;
            this.txtPayUri.Left = 1.27441F;
            this.txtPayUri.MultiLine = false;
            this.txtPayUri.Name = "txtPayUri";
            this.txtPayUri.OutputFormat = resources.GetString("txtPayUri.OutputFormat");
            this.txtPayUri.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtPayUri.Text = "E_売上合計";
            this.txtPayUri.Top = 2.016929F;
            this.txtPayUri.Width = 1.226772F;
            // 
            // lblPayNebiki
            // 
            this.lblPayNebiki.Height = 0.1496063F;
            this.lblPayNebiki.HyperLink = null;
            this.lblPayNebiki.Left = 2.5563F;
            this.lblPayNebiki.MultiLine = false;
            this.lblPayNebiki.Name = "lblPayNebiki";
            this.lblPayNebiki.Style = "font-size: 9.75pt; text-align: left; white-space: nowrap; ddo-wrap-mode: nowrap";
            this.lblPayNebiki.Text = "値引額合計";
            this.lblPayNebiki.Top = 2.016929F;
            this.lblPayNebiki.Width = 0.7322834F;
            // 
            // txtPayNebiki
            // 
            this.txtPayNebiki.CanGrow = false;
            this.txtPayNebiki.DataField = "E_値引合計";
            this.txtPayNebiki.Height = 0.1496063F;
            this.txtPayNebiki.Left = 3.318897F;
            this.txtPayNebiki.MultiLine = false;
            this.txtPayNebiki.Name = "txtPayNebiki";
            this.txtPayNebiki.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtPayNebiki.Text = "E_値引合計";
            this.txtPayNebiki.Top = 2.016929F;
            this.txtPayNebiki.Width = 0.880315F;
            // 
            // txtPayTotal
            // 
            this.txtPayTotal.CanGrow = false;
            this.txtPayTotal.DataField = "E_本日E合計";
            this.txtPayTotal.Height = 0.1496063F;
            this.txtPayTotal.Left = 6.511024F;
            this.txtPayTotal.MultiLine = false;
            this.txtPayTotal.Name = "txtPayTotal";
            this.txtPayTotal.OutputFormat = resources.GetString("txtPayTotal.OutputFormat");
            this.txtPayTotal.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtPayTotal.Text = "E_本日E合計";
            this.txtPayTotal.Top = 2.016929F;
            this.txtPayTotal.Width = 1.070866F;
            // 
            // linePayUnder
            // 
            this.linePayUnder.Height = 0F;
            this.linePayUnder.Left = 0.2952756F;
            this.linePayUnder.LineWeight = 1F;
            this.linePayUnder.Name = "linePayUnder";
            this.linePayUnder.Top = 2.199213F;
            this.linePayUnder.Width = 7.32126F;
            this.linePayUnder.X1 = 0.2952756F;
            this.linePayUnder.X2 = 7.616536F;
            this.linePayUnder.Y1 = 2.199213F;
            this.linePayUnder.Y2 = 2.199213F;
            // 
            // linePayLeft
            // 
            this.linePayLeft.Height = 0.21378F;
            this.linePayLeft.Left = 0.2952756F;
            this.linePayLeft.LineWeight = 1F;
            this.linePayLeft.Name = "linePayLeft";
            this.linePayLeft.Top = 1.985433F;
            this.linePayLeft.Width = 0F;
            this.linePayLeft.X1 = 0.2952756F;
            this.linePayLeft.X2 = 0.2952756F;
            this.linePayLeft.Y1 = 1.985433F;
            this.linePayLeft.Y2 = 2.199213F;
            // 
            // linePayNebiki
            // 
            this.linePayNebiki.Height = 0.21378F;
            this.linePayNebiki.Left = 2.535827F;
            this.linePayNebiki.LineWeight = 1F;
            this.linePayNebiki.Name = "linePayNebiki";
            this.linePayNebiki.Top = 1.985433F;
            this.linePayNebiki.Width = 0F;
            this.linePayNebiki.X1 = 2.535827F;
            this.linePayNebiki.X2 = 2.535827F;
            this.linePayNebiki.Y1 = 1.985433F;
            this.linePayNebiki.Y2 = 2.199213F;
            // 
            // linePayMisyukin
            // 
            this.linePayMisyukin.Height = 0.21378F;
            this.linePayMisyukin.Left = 4.233858F;
            this.linePayMisyukin.LineWeight = 1F;
            this.linePayMisyukin.Name = "linePayMisyukin";
            this.linePayMisyukin.Top = 1.985433F;
            this.linePayMisyukin.Width = 0F;
            this.linePayMisyukin.X1 = 4.233858F;
            this.linePayMisyukin.X2 = 4.233858F;
            this.linePayMisyukin.Y1 = 1.985433F;
            this.linePayMisyukin.Y2 = 2.199213F;
            // 
            // linePayTicket
            // 
            this.linePayTicket.Height = 0.21378F;
            this.linePayTicket.Left = 5.29252F;
            this.linePayTicket.LineWeight = 1F;
            this.linePayTicket.Name = "linePayTicket";
            this.linePayTicket.Top = 1.985433F;
            this.linePayTicket.Width = 0F;
            this.linePayTicket.X1 = 5.29252F;
            this.linePayTicket.X2 = 5.29252F;
            this.linePayTicket.Y1 = 1.985433F;
            this.linePayTicket.Y2 = 2.199213F;
            // 
            // linePayTotal
            // 
            this.linePayTotal.Height = 0.21378F;
            this.linePayTotal.Left = 6.491339F;
            this.linePayTotal.LineWeight = 1F;
            this.linePayTotal.Name = "linePayTotal";
            this.linePayTotal.Top = 1.985433F;
            this.linePayTotal.Width = 0F;
            this.linePayTotal.X1 = 6.491339F;
            this.linePayTotal.X2 = 6.491339F;
            this.linePayTotal.Y1 = 1.985433F;
            this.linePayTotal.Y2 = 2.199213F;
            // 
            // linePayRight
            // 
            this.linePayRight.Height = 0.21378F;
            this.linePayRight.Left = 7.616536F;
            this.linePayRight.LineWeight = 1F;
            this.linePayRight.Name = "linePayRight";
            this.linePayRight.Top = 1.985433F;
            this.linePayRight.Width = 0F;
            this.linePayRight.X1 = 7.616536F;
            this.linePayRight.X2 = 7.616536F;
            this.linePayRight.Y1 = 1.985433F;
            this.linePayRight.Y2 = 2.199213F;
            // 
            // linePayUpper
            // 
            this.linePayUpper.Height = 0F;
            this.linePayUpper.Left = 0.2917323F;
            this.linePayUpper.LineWeight = 1F;
            this.linePayUpper.Name = "linePayUpper";
            this.linePayUpper.Top = 1.985433F;
            this.linePayUpper.Width = 7.321259F;
            this.linePayUpper.X1 = 0.2917323F;
            this.linePayUpper.X2 = 7.612991F;
            this.linePayUpper.Y1 = 1.985433F;
            this.linePayUpper.Y2 = 1.985433F;
            // 
            // txtKaisyuPay
            // 
            this.txtKaisyuPay.CanGrow = false;
            this.txtKaisyuPay.DataField = "E_回収金額";
            this.txtKaisyuPay.Height = 0.1496063F;
            this.txtKaisyuPay.Left = 4.268504F;
            this.txtKaisyuPay.MultiLine = false;
            this.txtKaisyuPay.Name = "txtKaisyuPay";
            this.txtKaisyuPay.OutputFormat = resources.GetString("txtKaisyuPay.OutputFormat");
            this.txtKaisyuPay.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtKaisyuPay.Text = "E_回収金額";
            this.txtKaisyuPay.Top = 2.01693F;
            this.txtKaisyuPay.Width = 0.9893701F;
            // 
            // txtPayTicket
            // 
            this.txtPayTicket.CanGrow = false;
            this.txtPayTicket.DataField = "E_チケット束代";
            this.txtPayTicket.Height = 0.1496063F;
            this.txtPayTicket.Left = 5.312205F;
            this.txtPayTicket.MultiLine = false;
            this.txtPayTicket.Name = "txtPayTicket";
            this.txtPayTicket.OutputFormat = resources.GetString("txtPayTicket.OutputFormat");
            this.txtPayTicket.Style = "font-family: ＭＳ Ｐ明朝; font-size: 9.75pt; text-align: right; white-space: nowrap; d" +
    "do-wrap-mode: nowrap";
            this.txtPayTicket.Text = "E_チケット束代";
            this.txtPayTicket.Top = 2.016929F;
            this.txtPayTicket.Width = 1.144488F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // Report403AMain
            // 
            this.MasterReport = false;
            this.PageSettings.DefaultPaperSize = false;
            this.PageSettings.Margins.Bottom = 0.353937F;
            this.PageSettings.Margins.Left = 0.25F;
            this.PageSettings.Margins.Right = 0.25F;
            this.PageSettings.Margins.Top = 0.4374016F;
            this.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait;
            this.PageSettings.PaperHeight = 11.69291F;
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.PageSettings.PaperWidth = 8.267716F;
            this.PrintWidth = 7.768896F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; font-size: 10pt; " +
            "color: Black; font-family: \"ＭＳ Ｐゴシック\"; ddo-char-set: 128", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; font-family: \"MS UI Gothic\"; ddo-char-set: 12" +
            "8", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: inherit; font-family: \"MS UI Goth" +
            "ic\"; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            this.ReportStart += new System.EventHandler(this.Report403AMain_ReportStart);
            ((System.ComponentModel.ISupportInitialize)(this.lblTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblHizuke)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeisanbi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBusyo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusyoCD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBusyoName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPublish)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPublish)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfoPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCourse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTantosya)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseCD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCourseName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTantosyaCD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTantosyaName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketSeisan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketMochidashi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketMochiKaeri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketUriMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketUriKin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketMochidashi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketMochiKaeri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketUriMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketUriKin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblGenkinTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblGenkinNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinUri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinUrikake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinTicket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGenkinTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKakeuriTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKakeuriNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriUri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriMisyukin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKakeuriTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalUriKin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalUriKinFormula)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalUriKin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalNebikiFormula)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalTicket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalTicket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalBarclay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalBarclay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTotalTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKinsyuTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill10k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill5k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill2k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBill1k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill10k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill5k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill2k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBill1k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin500)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin500)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin100)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCoin1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin100)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCoin1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKogitte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblRyosyusyo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKinsyuTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKogitte)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRyosyusyo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKinsyuTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblKajokin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKajokin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketFooterTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketFooterKake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTicketMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTicketMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBonusFooterKake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBonusMai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBarclayFooterTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV300Kake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV300Mai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV300Equal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV300Kingaku)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV200Kake)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV200Mai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblBV200Equal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBV200Kingaku)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai10k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai5k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai2k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai1k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount10k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount5k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount2k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount1k)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai500)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai100)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMai1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount500)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount100)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount50)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPayTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayUri)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPayNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayNebiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKaisyuPay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayTicket)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.Label lblTitle;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeHizuke;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeBusyo;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapePublish;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineHizukeBusyoTate;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineBusyoTate;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineSakuseibiYoko;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCourseYoko;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineSakuseibiTate;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePageTate1;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePageTate2;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCourseTate;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblHizuke;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtSeisanbi;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBusyo;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBusyoCD;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBusyoName;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblPublish;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPublish;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblPage;
        private GrapeCity.ActiveReports.SectionReportModel.ReportInfo reportInfoPage;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCourse;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTantosya;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCourseCD;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCourseName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTantosyaCD;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTantosyaName;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportMochidashi;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTicketMochidashi;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTicketMochiKaeri;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTicketUriMai;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTicketUriKin;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketSeisan;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketMochidashi;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketMochiKaeri;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketUriMai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketUriKin;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTicketMochidashi;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTicketMochiKaeri;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTicketUriMai;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTicketUriKin;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportMeisaiGenkin;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinLeft;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinUrikake;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinRight;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinUnder;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblGenkinTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblGenkinNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtGenkinUri;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtGenkinNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtGenkinUrikake;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtGenkinTicket;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtGenkinTotal;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportMeisaiKakeuri;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriLeft;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriMisyukin;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriRight;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriUnder;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblKakeuriTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblKakeuriNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKakeuriUri;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKakeuriNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKakeuriMisyukin;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKakeuriTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTotalYoko1;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTotalYoko2;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTotalNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTotalTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTotalBarclay;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTotalTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalUriKin;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTotalUriKin;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalNebikiFormula;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTotalNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalTicket;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTotalTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalBarclay;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTotalBarclay;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTotalTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblKinsyuTable;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeBill;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineBill5k;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineBill2k;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineBill1k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTotalUriKinFormula;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBill10k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBill5k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBill2k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBill1k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBill10k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBill5k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBill2k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBill1k;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeCoin;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCoin100;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCoin50;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCoin10;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCoin5;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineCoin1;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCoin500;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCoin500;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCoin100;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCoin50;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCoin10;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCoin5;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblCoin1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCoin100;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCoin50;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCoin10;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCoin5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCoin1;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeKogitte;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKogitteTate;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineRyosyusyo;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKinsyuTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblKogitte;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblRyosyusyo;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblKinsyuTotal;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKogitte;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtRyosyusyo;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKinsyuTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKajyokinUnder;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKajyokinRight;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblKajokin;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKajokin;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shapeTicketFooter;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineTicketFooterTate;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketFooterTitle;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketFooterKake;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTicketMai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblTicketMai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBonusFooterKake;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBonusMai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBonusMai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBarclayFooterTitle;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBV300Kake;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBV300Mai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBV300Equal;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBV300Kingaku;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBV200Kake;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBV200Mai;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblBV200Equal;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBV200Kingaku;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineGenkinUpper;
        private GrapeCity.ActiveReports.SectionReportModel.Line lineKakeuriUpper;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai10k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai5k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai2k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai1k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount10k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount5k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount2k;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount1k;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai500;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai100;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai50;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai10;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai5;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblMai1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount500;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount100;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount50;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount10;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtAmount1;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblPayTotal;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPayUri;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblPayNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPayNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPayTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayUnder;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayLeft;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayNebiki;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayMisyukin;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayTicket;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayTotal;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayRight;
        private GrapeCity.ActiveReports.SectionReportModel.Line linePayUpper;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReportMeisaiPay;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtKaisyuPay;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPayTicket;
    }
}

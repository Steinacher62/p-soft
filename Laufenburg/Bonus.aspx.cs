using ch.appl.psoft.Common;
using ch.appl.psoft.Contact;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Laufenburg
{
    public partial class Bonus : PsoftDetailPage
    {


        protected PsoftPageLayout PsoftPageLayout;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Setting default breadcrumb caption
            this.BreadcrumbCaption = "Parameter Prämie";
            if (!IsPostBack)
            {
                LBVon.SelectedDate = System.DateTime.Parse("01.01." + DateTime.Now.Year.ToString());
                LBBis.SelectedDate = System.DateTime.Parse("31.12." + DateTime.Now.Year.ToString());
            }
        }

        protected void CalculateBonus_Click(object sender, EventArgs e)
        {
            string von_enc = Global.EncodeTo64(LBVon.SelectedDate.ToString());
            string bis_enc = Global.EncodeTo64(LBBis.SelectedDate.ToString());
            string p1 = Global.EncodeTo64(BV1.Text.ToString());
            string p2 = Global.EncodeTo64(BV2.Text.ToString());
            string p3 = Global.EncodeTo64(BV3.Text.ToString());
            string p4 = Global.EncodeTo64(BV4.Text.ToString());
            string p5 = Global.EncodeTo64(BV5.Text.ToString());
            string p6 = Global.EncodeTo64(BV6.Text.ToString());
            string p7 = Global.EncodeTo64(BV7.Text.ToString());
            string p8 = Global.EncodeTo64(BV8.Text.ToString());
            string p9 = Global.EncodeTo64(BV9.Text.ToString());
            string p10 = Global.EncodeTo64(BV10.Text.ToString());
            string p11 = Global.EncodeTo64(BV11.Text.ToString());
            string p12 = Global.EncodeTo64(BV12.Text.ToString());
            Response.Redirect("../report/CrystalReportViewer.aspx?alias=PraemieLaufenburg.rpt&paramVon=" + von_enc + "&paramBis=" + bis_enc + "&paramP1=" + p1 + "&paramP2=" + p2 + "&paramP3=" + p3 + "&paramP4=" + p4 + "&paramP5=" + p5 + "&paramP6=" + p6 + "&paramP7=" + p7 + "&paramP8=" + p8 + "&paramP9=" + p9 + "&paramP10=" + p10 + "&paramP11=" + p11 + "&paramP12=" + p12);
        }
    }
}
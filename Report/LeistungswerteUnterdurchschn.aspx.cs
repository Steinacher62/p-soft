using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report.Controls;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Report
{
    /// <summary>
    /// Summary description for Report LeistungswerteUnterdurchschn.
    /// </summary>
    public partial class LeistungswerteUnterdurchschn : PsoftContentPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = _mapper.get("LBUnterdurchschn", "title");
            BreadcrumbCaption = _mapper.get("LBUnterdurchschn", "title");

            // Setting content layout of page layout
            SearchContentLayout contentLayout = (SearchContentLayout)LoadPSOFTControl(SearchContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            ((SearchContentLayout)PageLayoutControl.ContentLayoutControl).SearchHeight = Unit.Percentage(100);

            LeistungswerteUnterdurchschnCtrl search = (LeistungswerteUnterdurchschnCtrl)this.LoadPSOFTControl(LeistungswerteUnterdurchschnCtrl.Path, "_search");
            search.OnSearchClick += new SearchClickHandler(onSearchClick);
            SetPageLayoutContentControl(SearchContentLayout.SEARCH, search);

            Control custom = Page.FindControl("_ctrl2");

            Control ctrl1;
            foreach (Control ctrl in Page.Controls)
            {
                ctrl1 = FindControlRecursive(ctrl, "_search");
            }
        }

        private static Control FindControlRecursive(Control root, string Id)
        {
            //if (root.ID == Id)
            if (root.ID != null && root.ID.Contains(Id))
            {
                return root;
            }

            foreach (System.Web.UI.Control Ctl in root.Controls)
            {
                System.Web.UI.Control temp = FindControlRecursive(Ctl, Id);

                if (temp != null)
                {
                    return temp;
                }
            }
            return null;
        }




        private void onSearchClick(object Sender, SearchEventArgs e)
        {
            Control ctrl1;
            DateTime LbFrom = new DateTime(1900, 01, 01);
            DateTime LbTo = new DateTime(2099, 12, 31);
            string test = Culture.ToString();
            foreach (Control ctrl in Page.Controls)
            {
                ctrl1 = FindControlRecursive(ctrl, "_search");
                if (ctrl1 != null)
                {

                    if (((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBFrom) != "")
                        LbFrom = DateTime.Parse((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBFrom);
                    if (((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBTo) != "")
                        LbTo = DateTime.Parse((ctrl1 as LeistungswerteUnterdurchschnCtrl).LBTo);
                }
            }



            DBData db = DBData.getDBData(Session);
            db.connect();
            string performanceRatingBase = Global.Config.getModuleParam("performance", "performanceRatingBase", "40");

            // use temporary table (needed for calculation of leistungswert) / 04.11.09 / mkr
            // delete temporary table if exists
            string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[leistungswerte_%userid%]') "
                              + "AND type in (N'U')) "
                                + "DROP TABLE [dbo].[leistungswerte_%userid%]";
            db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

            // create table
            string tbl_create = "CREATE TABLE [dbo].[leistungswerte_%userid%]("
                                + "[PNr] [varchar](64) NULL, "
                                + "[Name] [varchar](64) NULL, "
                                + "[Vorname] [varchar](64) NULL, "
                                + "[TITLE_DE] [varchar](128) NULL, "
                                + "[Kst] [varchar](64) NULL, "
                                + "[Abteilung] [varchar](128) NULL, "
                                + "[Bewertungsdatum] [datetime] NULL, "
                                + "[Leistungswert] [float] NULL) "
                                + "ON [PRIMARY];";

            db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));
            // fill table
            string sql = "INSERT INTO leistungswerte_" + db.userId.ToString() + " SELECT PNr, Name, Vorname, TITLE_DE, Kst,"
                         + " Abteilung, Bewertungsdatum, Leistungswert "
                         + "FROM f_schlechteLeistungswerte(" + performanceRatingBase + ") WHERE Bewertungsdatum >= '" + LbFrom.ToString("MM.dd.yyyy") + "' and Bewertungsdatum < '" + LbTo.ToString("MM.dd.yyyy") + "';";
            db.execute(sql);
            db.disconnect();
            Response.Redirect("CrystalReportViewer.aspx?alias=UnterdurchschnittlicheLeistungswerte&startdat=" + LbFrom.ToShortDateString() + "&enddat=" + LbTo.ToShortDateString(),true);
        }

        //private string EncodeTo64(string toEncode)
        //{
        //    byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(toEncode);

        //    string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);

        //    return returnValue;
        //}

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}

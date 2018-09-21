using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.SBS
{
    public partial class SBSSeminarView : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PlaceHolder ph = new PlaceHolder();
            form1.Controls.Add(ph);
            SBS.SBSStartPage startPage = new SBSStartPage();
            if (Request.QueryString["seminarID"] != null)
            {
                startPage.seminarID = Convert.ToInt32(Request.QueryString["seminarID"]);
            }
            startPage.buildPage(ph);
        }
    }
}
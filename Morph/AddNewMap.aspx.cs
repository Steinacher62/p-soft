using System;
using System.Web.UI;

namespace ch.appl.psoft.Morph
{
    public partial class AddNewMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             WebService.SokratesService SokratesService = new WebService.SokratesService();
             string IdUid = SokratesService.addNewSokrates(Page.Request.QueryString.Get("template"));
            if (IdUid.Equals("OrderNeed"))
            {
                Response.Redirect(Global.Config.baseURL + "/Payment/Order.aspx");
            }
             Response.Redirect(Global.Config.baseURL + "/Morph/MatrixDetail.aspx?MatrixId=" + IdUid);
        }
    }
}


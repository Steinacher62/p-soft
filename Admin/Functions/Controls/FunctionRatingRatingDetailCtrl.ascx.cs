using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class FunctionRatingRatingDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string imageUrl = "../../images/";
            RadMenu1.Items[0].ImageUrl = imageUrl + "delete_enable.gif";
            RadMenu1.Items[1].ImageUrl = imageUrl + "document.gif";
            RadMenu1.Items[2].ImageUrl = imageUrl + "info.gif";
        }
    }
}
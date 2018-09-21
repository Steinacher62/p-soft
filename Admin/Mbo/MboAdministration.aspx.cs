using ch.appl.psoft.Admin.Mbo.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace ch.appl.psoft.Admin.Mbo
{
    public partial class MboAdministration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            L_Layout.pageViewL.Controls.Add((MboAdminDetailCtrl)LoadControl("Controls/MboAdminDetailCtrl.ascx"));
        }
    }
}
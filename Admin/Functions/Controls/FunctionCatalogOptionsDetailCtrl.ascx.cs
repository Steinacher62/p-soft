using ch.appl.psoft.Common;
using ch.appl.psoft.Contact;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.appl.psoft.Person.Controls;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class FunctionCatalogOptionsDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            detailTitle.Text = _map.get("functionRating", "catalogOption");
            averageTypTitle.Text = _map.get("functionRating", "averageTyp");
            maxValueTitle.Text = _map.get("functionRating", "maxValue");
            averageTypData.DataSource = Adminutilities.GetIListFromXML(Session, "functionRating", "averageTypFlag", false);
            averageTypData.DataValueField = "ID";
            averageTypData.DataTextField = "ENTRY";
            averageTypData.DataBind();
        }
    }
}
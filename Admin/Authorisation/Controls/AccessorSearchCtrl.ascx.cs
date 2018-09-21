using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Authorisation.Controls
{
    public partial class AccessorSearchCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            LabelTitle.Text = _map.get("authorisations", "accessor"); 
            LabelAccessor.Text = _map.get("authorisations", "usersGroups");
            LabelTyp.Text = _map.get("authorisations", "typ");
            LabelIsEditable.Text = _map.get("authorisations", "isEditable");
            DDTyp.DataSource = Adminutilities.GetIListFromXML(Session, "authorisations", "accessorTyps", true);
            DDTyp.DataValueField = "ID";
            DDTyp.DataTextField = "ENTRY";
            DDTyp.DataBind();
            DDIsEditable.DataSource =  Adminutilities.GetIListFromXML(Session, "authorisations", "editable", true);
            DDIsEditable.DataValueField = "ID";
            DDIsEditable.DataTextField = "ENTRY";
            DDIsEditable.DataBind();
        }
    }
}
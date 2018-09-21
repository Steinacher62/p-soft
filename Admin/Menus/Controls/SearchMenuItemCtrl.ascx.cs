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


namespace ch.appl.psoft.Admin.Menus.Controls
{
    public partial class SearchMenuItemCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LabelTitle.Text = _map.get("application", "Shortcut");
            LabelTyp.Text = _map.get("application", "ShortcutTyp");
            LabelAlias.Text = _map.get("application", "Alias");
            DDTyp.DataSource =  Adminutilities.GetIListFromXML(Session, "organisation", "shortcutTyp", true);
            DDTyp.DataValueField = "ID";
            DDTyp.DataTextField = "ENTRY";
            DDTyp.DataBind();
        }
    }
}
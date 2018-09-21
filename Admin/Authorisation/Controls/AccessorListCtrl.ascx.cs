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
    public partial class AccessorListCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            AccessorTitle.Text = _map.get("authorisations", "usersGroups");
            EditableTitle.Text = _map.get("authorisations", "isEditable");
            AccessorListContextMenu.Items[0].Text = _map.get("authorisations", "addAccessor");
        }
    }
}
using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Admin.Authorisation.Controls
{
    public partial class AuthorisationPermissionCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            ApplicationPermissionsTitle.Text = _map.get("authorisations", "applicationRights");
            PermissionTitle.Text = _map.get("authorisations", "permissions");
            ReadTitle.Text = _map.get("authorisations", "authRead");
            InsertTitle.Text = _map.get("authorisations", "authInsert");
            EditTitle.Text = _map.get("authorisations", "authUpdate");
            DeleteTitle.Text = _map.get("authorisations", "authDelete");
            AdminTitle.Text = _map.get("authorisations", "authAdmin");
            ExecuteTitle.Text = _map.get("authorisations", "authExecute");
            TakeInheritedTitle.Text = _map.get("authorisations", "inheritFromSuperior");
            

        }

        
    }
}
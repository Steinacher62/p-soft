using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Authorisation.Controls
{
    public partial class AccessorGroupDetailCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            TabStrip.Tabs[0].Text = _map.get("authorisations", "tabMembers");
            TabStrip.Tabs[1].Text = _map.get("authorisations", "tabMemberOf");
            GroupMembersAccessorTitle.Text = _map.get("authorisations", "usersGroups");
            GroupMembersEditableTitle.Text = _map.get("authorisations", "isEditable");
            MemberOfAccessorTitle.Text = _map.get("authorisations", "usersGroups");
            MemberOfEditableTitle.Text = _map.get("authorisations", "isEditable");
            TabStrip.SelectedIndex = 1;
            GroupMembersListContextMenu.Items[0].Text = _map.get("authorisations", "dropAccessor");
        }
    }
}
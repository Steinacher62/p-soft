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
    public partial class AccessorDetail : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LabelTitle.Text = _map.get("authorisations", "accessor");
            LabelGroup.Text = _map.get("authorisations", "group");
            LabelAccessor.Text = _map.get("authorisations", "accessors");

            NewAccessorgroupWindow.Title = _map.get("authorisations", "addAccessorGroup");
            LabelNewGroupTitle.Text = _map.get("authorisations", "group");
        }
    }
}
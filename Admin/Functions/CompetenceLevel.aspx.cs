using ch.appl.psoft.Admin.Functions.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Functions
{
    public partial class CompetenceLevel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORU_Layout.MainTitle  = _map.get("competences", "competencLevels");
            LRORU_Layout.pageViewL.Controls.Add((CompetenceDetailCtrl)LoadControl("Controls/CompetenceDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((CompetenceSearchCtrl)LoadControl("Controls/CompetenceSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((CompetenceListCtrl)LoadControl("Controls/CompetenceListCtrl.ascx"));
        }
    }
}
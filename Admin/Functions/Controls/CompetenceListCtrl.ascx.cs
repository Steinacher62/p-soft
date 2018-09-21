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
namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class CompetenceListCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
           Label title = (Label)CompetenceListBox.Header.FindControl("CompetenceListBoxHeadertitle");
            title.Text = _map.get("competences", "competencLevels");
        }
    }
}
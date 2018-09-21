using ch.appl.psoft.Admin.Chart.Controls;
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

namespace ch.appl.psoft.Admin.Mbo.Controls
{
    public partial class ObjectiveRoundDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            RoundDetailTitle.Text = _map.get("mbo", "objectitveRound");
            NameTitle.Text = _map.get("mbo", "title");
            DesacriptionTitle.Text = _map.get("mbo", "description");
            StartFromTitle.Text = _map.get("mbo", "from");
            EndToTitle.Text = _map.get("mbo", "to");

        }
    }
}
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

namespace ch.appl.psoft.Admin.Wage.Controls
{
    public partial class VariantDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            VariantTitle.Text = _map.get("wage", "variant");
            NameTitle.Text = _map.get("wage", "title");
            ActiveTitle.Text = _map.get("wage", "active");
            ValuePointFixTitle.Text = _map.get("wage", "fixPointValue");
            ExclusionFromTitle.Text = _map.get("wage", "exclusionFrom");
            ExclusionToTitle.Text = _map.get("wage", "exclusionTo");
        }
    }
}
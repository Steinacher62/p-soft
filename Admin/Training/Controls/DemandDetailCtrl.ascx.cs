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

namespace ch.appl.psoft.Admin.Training.Controls
{
    public partial class DemandDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DemandDetailTitle.Text = _map.get("training", "demandTraining");
            NameTitle.Text = _map.get("training", "title");
            OrdnumberTitle.Text =_map.get("training", "ordNumber");
            MnemoTitle.Text = _map.get("training", "mnemo");
            DesacriptionTitle.Text = _map.get("training", "description");
        }
    }
}
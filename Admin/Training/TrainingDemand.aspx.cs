using ch.appl.psoft.Admin.Training.Controls;
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

namespace ch.appl.psoft.Admin.Training
{
    public partial class TrainingDemand : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORU_Layout.MainTitle = _map.get("training", "demandTraining");
            LRORU_Layout.pageViewL.Controls.Add((DemandDetailCtrl)LoadControl("Controls/DemandDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((DemandSearchCtrl)LoadControl("Controls/DemandSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((DemandSearchListCtrl)LoadControl("Controls/DemandSearchListCtrl.ascx"));
        }
    }
}
using ch.appl.psoft.Admin.Mbo.Controls;
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

namespace ch.appl.psoft.Admin.Mbo
{
    public partial class ObjectiveRound : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LRORU_Layout.MainTitle = _map.get("mbo", "objectitveRound");

            LRORU_Layout.pageViewL.Controls.Add((ObjectiveRoundDetailCtrl)LoadControl("Controls/ObjectiveRoundDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((ObjectiveRoundSearchCtrl)LoadControl("Controls/ObjectiveRoundSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((ObjectiveRoundSearchListCtrl)LoadControl("Controls/ObjectiveRoundSearchListCtrl.ascx"));
        }
    }
}
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
    public partial class MboAdminDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            AdminDetailTitle.Text = _map.get("mbo", "configuration");
            ObjectiveFilterTitle.Text = _map.get("mbo", "objectiveFilter");
            ObjectiveRoundTitle.Text = _map.get("mbo", "objectiveRound");
            StartFromTitle.Text = _map.get("mbo", "evaluationFrom");
            EndToTitle.Text = _map.get("mbo", "evaluationTo");

            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable objectiveRounds = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM OBJECTIVE_TURN ORDER BY TITLE_" + lang);
            ObjectiveRoundData.DataSource = objectiveRounds;
            ObjectiveRoundData.DataValueField = "ID";
            ObjectiveRoundData.DataTextField = "TITLE";
            ObjectiveRoundData.DataBind();

            ObjectiveFilterData.DataSource = Adminutilities.GetIListFromXML(Session, "mbo", "objectiveFilters", true);
            ObjectiveFilterData.DataValueField = "ID";
            ObjectiveFilterData.DataTextField = "ENTRY";
            ObjectiveFilterData.DataBind();

            db.disconnect();
        }
    }
}
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

namespace ch.appl.psoft.Admin.Chart.Controls
{
    public partial class ChartDetailCtrl : System.Web.UI.UserControl
    {

        private Organisation.Chart _chart = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            ContextMenuChartItem.Items[0].Text = _map.get("chartnode", "deletenodemenu");
            ContextMenuChartItem.Items[1].Text = _map.get("chartnode", "deleteSubOe");
            ContextMenuChartItem.Items[2].Text = _map.get("chartnode", "moveNodeBefore");
            ContextMenuChartItem.Items[3].Text = _map.get("chartnode", "moveNodeAfter");
            ContextMenuChartItem.Items[4].Text = _map.get("chartnode", "insertMissigItem");

            LinkTitle.Text = _map.get("chartnode", "links");
            LabelTextTitle.Text = _map.get("chartnode", "text");
            LinkTitle1.Text = _map.get("chartnode", "link");
            ButtonListText.Items[0].Text = _map.get("chartnode", "leader");
            ButtonListText.Items[1].Text = _map.get("chartnode", "nameOrgentity");
            ButtonListText.Items[0].Selected = true;

            ButtonListLink.Items[0].Text = _map.get("chartnode", "noLink");
            ButtonListLink.Items[1].Text = _map.get("chartnode", "leader");
            ButtonListLink.Items[2].Text = _map.get("chartnode", "personOrgentityList");
            ButtonListLink.Items[3].Text = _map.get("chartnode", "clipboardOrgentity");
            ButtonListLink.Items[4].Text = _map.get("chartnode", "chartLink");
            ButtonListLink.Items[0].Selected = true;
            OrganisationLinkTitle.Text = _map.get("chartnode", "organisation");
            ChartLinkTitle.Text = _map.get("chartnode", "chart");

            OpenNewWindowTitle.Text = _map.get("chartnode", "openInNewWindow");
            LayoutTitle.Text = _map.get("chartnode", "layout");

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable layoutTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTTEXTLAYOUT ORDER BY TITLE_" + lang);

            DataRow layoutRow = layoutTable.NewRow();
            layoutRow["ID"] = 0;
            layoutRow["TITLE"] = "";
            layoutTable.Rows.Add(layoutRow);

            LayoutData.DataSource = layoutTable;
            LayoutData.DataValueField = "ID";
            LayoutData.DataTextField = "TITLE";
            LayoutData.DataBind();
            LayoutData.Items.Sort();

            DataTable organisationTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM ORGANISATION ORDER BY TITLE_" + lang);
            long mainId = db.lookup("ID", "ORGANISATION", "MAINORGANISATION = 1", 0L);
            OrganisationData.DataSource = organisationTable;
            OrganisationData.DataValueField = "ID";
            OrganisationData.DataTextField = "TITLE";
            OrganisationData.DataBind();
            OrganisationData.SelectedValue = mainId.ToString();

            PiktoTitle.Text = _map.get("chartnode", "linksPikto");
            LabelTextPiktoTitle.Text = _map.get("chartnode", "text");
            FreeTextTitel.Text = _map.get("chartnode", "freeText");
            LinkTitlePikto1.Text = _map.get("chartnode", "link");

            ButtonLisPiktotText.Items[0].Text = _map.get("chartnode", "leader");
            ButtonLisPiktotText.Items[1].Text = _map.get("chartnode", "nameOrgentity");
            //ButtonLisPiktotText.Items[0].Selected = true;

            ButtonListPikto.Items[0].Text = _map.get("chartnode", "noLink");
            ButtonListPikto.Items[1].Text = _map.get("chartnode", "leader");
            ButtonListPikto.Items[2].Text = _map.get("chartnode", "personOrgentityList");
            ButtonListPikto.Items[3].Text = _map.get("chartnode", "clipboardOrgentity");
            ButtonListPikto.Items[4].Text = _map.get("chartnode", "chartLink");
            ButtonListPikto.Items[0].Selected = true;

            OrganisationPiktoTitle.Text = _map.get("chartnode", "organisation");
            ChartPiktoTitle.Text = _map.get("chartnode", "chart");

            OpenNewWindowPiktoTitle.Text = _map.get("chartnode", "openInNewWindow");
            PiktoTitle1.Text = _map.get("chartnode", "piktogramm");

            DataTable piktoTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTPIKTOLAYOUT ORDER BY TITLE_" + lang);

            OrganisationPiktoData.DataSource = organisationTable;
            OrganisationPiktoData.DataValueField = "ID";
            OrganisationPiktoData.DataTextField = "TITLE";
            OrganisationPiktoData.DataBind();
            OrganisationPiktoData.SelectedValue = mainId.ToString();

            DataRow piktoRow = piktoTable.NewRow();
            piktoRow["ID"] = 0;
            piktoRow["TITLE"] = "";
            piktoTable.Rows.Add(piktoRow);

            PiktoData1.DataSource = piktoTable;
            PiktoData1.DataValueField = "ID";
            PiktoData1.DataTextField = "TITLE";
            PiktoData1.DataBind();
            PiktoData1.Items.Sort();
        }



    }
}
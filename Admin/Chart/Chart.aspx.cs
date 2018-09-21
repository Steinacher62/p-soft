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


namespace ch.appl.psoft.Admin.Chart
{
    public partial class Chart : System.Web.UI.Page
    {
        private LanguageMapper _map;

        private DataTable oeTree = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {

            _map = LanguageMapper.getLanguageMapper(Session);
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LRORU_Layout.MainTitle = _map.get("chart", "chart");

            LRORU_Layout.pageViewL.Controls.Add((ChartDetailCtrl)LoadControl("Controls/ChartDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((ChartSearchCtrl)LoadControl("Controls/ChartSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((ChartSearchListCtrl)LoadControl("Controls/ChartSearchListCtrl.ascx"));
            LabelTitle.Text = _map.get("chart", "chart");
            OrganisationTitle.Text = _map.get("chart", "organisation");
            LayoutTitle.Text = _map.get("chart", "layout");
            TextLayoutTitle.Text = _map.get("chart", "textLayout");
            AligmentTitle.Text = _map.get("chart", "aligen");
            NameTitle.Text = _map.get("chart", "name");
            LabelTitleChart.Text = _map.get("chart", "chartOrgentity");
            ChartTitle.Text = _map.get("chart", "chart");
            OrgentityTitle.Text = _map.get("chart", "orgentity");
            NodeLayoutTitle.Text = _map.get("chart", "layout");
            SubNodeLayoutTitle.Text = _map.get("chart", "laoutSubOrgentities");
            NodeAligmentTitle.Text = _map.get("chart", "aligen");
            NodeTextLayoutTitle.Text = _map.get("chart", "textLayout");
            NodeTypTitle.Text = _map.get("chart", "typ");
            NodeShowPersonTitle.Text = _map.get("chart", "showPerson");
            NodeOffsetVerticalLineTitle.Text = _map.get("chart", "offsetVertical");
            NodeHorizontalSpaceTitle.Text = _map.get("chart", "spaceHorizontal");
            NodeVerticalSpaceTitle.Text = _map.get("chart", "spaceVertical");
            LabelTitleNewChart.Text = _map.get("chart", "newChart");
            NewChartTitle.Text = _map.get("chart", "organisation");
            NewChartNodeLayoutTitle.Text = _map.get("chart", "layout");
            NewChartNodeTextLayoutTitle.Text = _map.get("chart", "textLayout");
            NewChartTextAligmnetTitle.Text = _map.get("chart", "aligen");
            NewChartNameTitle.Text = _map.get("chart", "title");

            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable layoutTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTNODELAYOUT ORDER BY TITLE_" + lang);
            LayoutData.DataSource = layoutTable;
            LayoutData.DataValueField = "ID";
            LayoutData.DataTextField = "TITLE";
            LayoutData.DataBind();

            NewChartNodeLayoutData.DataSource = layoutTable;
            NewChartNodeLayoutData.DataValueField = "ID";
            NewChartNodeLayoutData.DataTextField = "TITLE";
            NewChartNodeLayoutData.DataBind();

            DataTable textLayoutTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTTEXTLAYOUT ORDER BY TITLE_" + lang);
            TextLayoutData.DataSource = textLayoutTable;
            TextLayoutData.DataValueField = "ID";
            TextLayoutData.DataTextField = "TITLE";
            TextLayoutData.DataBind();

            NewChartNodeTextLayoutData.DataSource = textLayoutTable;
            NewChartNodeTextLayoutData.DataValueField = "ID";
            NewChartNodeTextLayoutData.DataTextField = "TITLE";
            NewChartNodeTextLayoutData.DataBind();

            DataTable aligmentTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTALIGNMENT ORDER BY TITLE_" + lang);
            AligmentData.DataSource = aligmentTable;
            AligmentData.DataValueField = "ID";
            AligmentData.DataTextField = "TITLE";
            AligmentData.DataBind();

            NewChartTextAligmnetData.DataSource = aligmentTable;
            NewChartTextAligmnetData.DataValueField = "ID";
            NewChartTextAligmnetData.DataTextField = "TITLE";
            NewChartTextAligmnetData.DataBind();
            NewChartTextAligmnetData.Items[2].Selected = true;


            DataRow layoutRow = layoutTable.NewRow();
            layoutRow["ID"] = 0;
            layoutRow["TITLE"] = "";
            layoutTable.Rows.Add(layoutRow);

            NodeLayoutData.DataSource = layoutTable;
            NodeLayoutData.DataValueField = "ID";
            NodeLayoutData.DataTextField = "TITLE";
            NodeLayoutData.DataBind();
            NodeLayoutData.Items.Sort();

            SubNodeLayoutData.DataSource = layoutTable;
            SubNodeLayoutData.DataValueField = "ID";
            SubNodeLayoutData.DataTextField = "TITLE";
            SubNodeLayoutData.DataBind();
            SubNodeLayoutData.Items.Sort();

            DataRow textLayoutRow = textLayoutTable.NewRow();
            textLayoutRow["ID"] = 0;
            textLayoutRow["TITLE"] = "";
            textLayoutTable.Rows.Add(textLayoutRow);

            NodeTextLayoutData.DataSource = textLayoutTable;
            NodeTextLayoutData.DataValueField = "ID";
            NodeTextLayoutData.DataTextField = "TITLE";
            NodeTextLayoutData.DataBind();
            NodeTextLayoutData.Items.Sort();

            DataRow aligmentRow = aligmentTable.NewRow();
            aligmentRow["ID"] = 0;
            aligmentRow["TITLE"] = "";
            aligmentTable.Rows.Add(aligmentRow);

            NodeAligmentData.DataSource = aligmentTable;
            NodeAligmentData.DataValueField = "ID";
            NodeAligmentData.DataTextField = "TITLE";
            NodeAligmentData.DataBind();
            NodeAligmentData.Items.Sort();

            NodeTypData.DataSource = Adminutilities.GetIListFromXML(Session, "chartnode", "typ", true);
            NodeTypData.DataValueField = "ID";
            NodeTypData.DataTextField = "ENTRY";
            NodeTypData.DataBind();
            db.disconnect();
            TabStrip.Tabs[0].Text = _map.get("chartnode", "tabTexts");
            TabStrip.Tabs[1].Text = _map.get("chartnode", "tabPiktos");
            TabStrip.SelectedIndex = 0;

            SqlConnection sqlcon = (SqlConnection)db.connection;
            SqlDataAdapter dataAdapterOes = new SqlDataAdapter("SELECT ID AS OeId, PARENT_ID AS OeParentId, TITLE_DE AS OeTitleDe, TITLE_FR AS OeTitleFr, TITLE_EN AS OeTitleEn, CASE WHEN PARENT_ID IS NULL THEN 'FIRM' ELSE 'ORGENTITY' END AS TYP FROM ORGENTITY", sqlcon);

            dataAdapterOes.Fill(oeTree);
            OETree1.DataSource = oeTree;
            OETree1.DataFieldID = "OeId";
            OETree1.DataValueField = "OeId";
            OETree1.DataFieldParentID = "OeParentId";
            OETree1.DataTextField = "OeTitleDe";
            OETree1.DataBind();

            NewChartWindow.Title = _map.get("chart", "newChart");
            TreeTitle.Text = _map.get("chart", "organisation");

            contextMenuLink.Items[0].Text = _map.get("chartnode", "newLink");
            contextMenuLink.Items[1].Text = _map.get("chartnode", "editLink");
            contextMenuLink.Items[2].Text = _map.get("chartnode", "deleteLink");
            contextMenuLink.Items[3].Text = _map.get("chartnode", "moveUp");
            contextMenuLink.Items[4].Text = _map.get("chartnode", "moveDown");
            contextMenuLink.Items[5].Text = _map.get("chartnode", "copyLink");

            contextMenuPikto.Items[0].Text = _map.get("chartnode", "newPikto");
            contextMenuPikto.Items[1].Text = _map.get("chartnode", "editPikto");
            contextMenuPikto.Items[2].Text = _map.get("chartnode", "deletePikto");
            contextMenuPikto.Items[3].Text = _map.get("chartnode", "moveUp");
            contextMenuPikto.Items[4].Text = _map.get("chartnode", "moveDown");
            contextMenuPikto.Items[5].Text = _map.get("chartnode", "copyPikto");
        }

        protected void OETree_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            OrganisationTitle.Text = "Organisation";
            RadTreeNode orgNode = e.Node;
            DataRow[] oetyp = oeTree.Select("OeId=" + e.Node.Value);
            e.Node.Attributes.Add("TYP", oetyp[0]["TYP"].ToString());
            addNodeImage(orgNode, oetyp[0]["TYP"].ToString());


        }

        private void addNodeImage(RadTreeNode node, string typ)
        {
            switch (typ)
            {
                case "FIRM":
                    node.ImageUrl = "../Images/og_organisation.gif";
                    break;
                case "ORGENTITY":
                    node.ImageUrl = "../Images/og_abteilung.gif";
                    break;
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            RadContextMenu menu = TextLinkGrid.HeaderContextMenu;
            menu.Items.Clear();
            RadMenuItem item = new RadMenuItem();
            item.Text = _map.get("chartnode", "newLink");
            item.Value = "newLink";
            item.Attributes["ColumnName"] = string.Empty;
            item.Attributes["TableID"] = string.Empty;
            item.PostBack = false;
            menu.OnClientItemClicked = "HeaderContextMenuLinkClicked";
            menu.Items.Add(item);

            RadContextMenu menu1 = IconLinkGrid.HeaderContextMenu;
            menu1.Items.Clear();
            RadMenuItem item1 = new RadMenuItem();
            item1.Text = _map.get("chartnode", "newPikto");
            item1.Value = "newPikto";
            item1.Attributes["ColumnName"] = string.Empty;
            item1.Attributes["TableID"] = string.Empty;
            item1.PostBack = false;
            menu1.OnClientItemClicked = "HeaderContextMenuPiktoClicked";
            menu1.Items.Add(item1);
            base.OnPreRenderComplete(e);
        }
    }
}

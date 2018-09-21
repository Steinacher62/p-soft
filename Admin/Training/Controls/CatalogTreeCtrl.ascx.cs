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
    public partial class CatalogTreeCtrl : System.Web.UI.UserControl
    {
        private DataSet groupsItems = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            CatalogContextMenu.Items[0].Text = _map.get("training", "addTraining");
            CatalogContextMenu.Items[1].Text = _map.get("training", "addTrainingCategory");
            CatalogContextMenu.Items[2].Text = _map.get("training", "delete");

            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable groups = new DataTable();
            DataTable items = new DataTable();
            SqlConnection sqlcon = (SqlConnection)db.connection;
            long rootId = (long)db.lookup("ID", "TRAININGGROUP", "PARENT_ID IS NULL");
            SqlDataAdapter dataAdapterGroups = new SqlDataAdapter("SELECT ID AS GROUP_ID, PARENT_ID AS GROUP_PARENT_ID, TITLE_" + lang + " AS GROUP_TITLE, 'GROUP' AS  TYP, ORDNUMBER AS GROUP_ORDNUMBER FROM TRAININGGROUP WHERE ROOT_ID =" + rootId + " ORDER BY ORDNUMBER", sqlcon);
            SqlDataAdapter dataAdapterItems = new SqlDataAdapter("SELECT ID, TRAININGGROUP_ID AS ITEM_GROUP_ID, TITLE_" + lang + " AS TITLE, 'ITEM' AS TYP, ORDNUMBER AS ITEM_ORDNUMBER FROM TRAINING ORDER BY ORDNUMBER", sqlcon);
            dataAdapterGroups.Fill(groups);
            dataAdapterItems.Fill(items);

            groupsItems.Tables.Add(groups);
            groupsItems.Tables.Add(items);
            groupsItems.Tables[0].TableName = "Groups";
            groupsItems.Tables[1].TableName = "Items";
            groupsItems.Relations.Add("GROUP_PARENT_ID", groupsItems.Tables["Groups"].Columns["GROUP_ID"], groupsItems.Tables["Groups"].Columns["GROUP_PARENT_ID"]);
            groupsItems.Relations.Add("ITEM_GROUPID", groupsItems.Tables["Groups"].Columns["GROUP_ID"], groupsItems.Tables["items"].Columns["ITEM_GROUP_ID"]);
            CatalogTree.DataSource = groupsItems.Tables[0];
            CatalogTree.DataFieldID = "GROUP_ID";
            CatalogTree.DataValueField = "GROUP_ID";
            CatalogTree.DataFieldParentID = "GROUP_PARENT_ID";
            CatalogTree.DataTextField = "GROUP_TITLE";
            CatalogTree.DataBind();

            db.disconnect();
        }

        protected void CatalogTree_NodeDataBound(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            RadTreeNode groupNode = e.Node;
            DataRow[] grouptyp = groupsItems.Tables["Groups"].Select("GROUP_ID=" + e.Node.Value);
            e.Node.Attributes.Add("TYP", grouptyp[0]["TYP"].ToString());
            e.Node.Attributes.Add("ORDNUMBER", grouptyp[0]["GROUP_ORDNUMBER"].ToString());
            addNodeImage(groupNode, grouptyp[0]["TYP"].ToString());
            DataRow[] groupRow = groupsItems.Tables["items"].Select("ITEM_GROUP_ID=" + e.Node.Value);
            foreach (DataRow itemRow in groupRow)
            {
                RadTreeNode itemNode = new RadTreeNode(itemRow["TITLE"].ToString(), itemRow["ID"].ToString());
                itemNode.Attributes.Add("TYP", itemRow["TYP"].ToString());
                itemNode.Attributes.Add("ORDNUMBER", itemRow["ITEM_ORDNUMBER"].ToString());
                addNodeImage(itemNode, itemRow["TYP"].ToString());
                groupNode.Nodes.Add(itemNode);
            }
        }

        private void addNodeImage(RadTreeNode node, string typ)
        {
            switch (typ)
            {
                case "GROUP":
                    node.ImageUrl = "../../Images/m_menuegruppe.gif";
                    break;
                case "ITEM":
                    node.ImageUrl = "../../Images/m_menue.gif";
                    break;
            }
        }
    }
}
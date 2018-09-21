using ch.appl.psoft.Common;
using ch.appl.psoft.Contact;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.appl.psoft.Person.Controls;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Menus.Controls
{
    public partial class KnowledgeMenuTreeCtrl : System.Web.UI.UserControl
    {
        private DataSet groupsItems = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable groups = new DataTable();
            DataTable items = new DataTable();
            SqlConnection sqlcon = (SqlConnection)db.connection;
            long rootId = (long)db.lookup("ID", "MENUGROUP", "MNEMO = 'KNOWLEDGE' AND PARENT_ID IS NULL");
            SqlDataAdapter dataAdapterGroups = new SqlDataAdapter("SELECT ID AS GROUP_ID, PARENT_ID AS GROUP_PARENT_ID, TITLE_" + lang + " AS GROUP_TITLE, 'MENUGROUP' AS  TYP, ORDNUMBER AS GROUP_ORDNUMBER, INHERIT AS GROUP_INHERIT FROM MENUGROUP WHERE ROOT_ID =" + rootId + " ORDER BY ORDNUMBER", sqlcon);
            SqlDataAdapter dataAdapterItems = new SqlDataAdapter("SELECT ID, MENUGROUP_ID AS ITEM_GROUP_ID, TITLE_" + lang + " AS TITLE, 'MENUITEM' AS TYP, ORDNUMBER AS ITEM_ORDNUMBER, ISSTARTPAGE, LINK FROM MENUITEM WHERE MENUGROUP_ID IN(SELECT ID FROM MENUGROUP WHERE ROOT_ID =" + rootId + ") ORDER BY ORDNUMBER", sqlcon);
            dataAdapterGroups.Fill(groups);
            dataAdapterItems.Fill(items);

            groupsItems.Tables.Add(groups);
            groupsItems.Tables.Add(items);
            groupsItems.Tables[0].TableName = "Groups";
            groupsItems.Tables[1].TableName = "Items";
            groupsItems.Relations.Add("GROUP_PARENT_ID", groupsItems.Tables["Groups"].Columns["GROUP_ID"], groupsItems.Tables["Groups"].Columns["GROUP_PARENT_ID"]);
            groupsItems.Relations.Add("ITEM_GROUPID", groupsItems.Tables["Groups"].Columns["GROUP_ID"], groupsItems.Tables["items"].Columns["ITEM_GROUP_ID"]);
            MenuTree.DataSource = groupsItems.Tables[0];
            MenuTree.DataFieldID = "GROUP_ID";
            MenuTree.DataValueField = "GROUP_ID";
            MenuTree.DataFieldParentID = "GROUP_PARENT_ID";
            MenuTree.DataTextField = "GROUP_TITLE";
            MenuTree.DataBind();

            db.disconnect();
        }

        protected void MenuTree_NodeDataBound(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
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
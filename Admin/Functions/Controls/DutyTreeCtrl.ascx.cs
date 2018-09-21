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

namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class DutyTreeCtrl : System.Web.UI.UserControl
    {
        private DataSet treeDuties = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable dutyGroups = new DataTable();
            DataTable duties = new DataTable();
            SqlConnection sqlcon = (SqlConnection)db.connection;
            SqlDataAdapter dataAdapterGroups = new SqlDataAdapter("SELECT ID, PARENT_ID, TITLE_" + lang + " AS TITLE, 'GROUP' AS  TYP, ORDNUMBER FROM DUTYGROUP ORDER BY ORDNUMBER", sqlcon);
            SqlDataAdapter dataAdapterItems = new SqlDataAdapter("SELECT ID, DUTYGROUP_ID AS GROUP_ID, ORDNUMBER, NUMBER + ' ' + TITLE_" + lang + " AS TITLE, 'DUTY' AS TYP FROM DUTYV ORDER BY ORDNUMBER", sqlcon);
            dataAdapterGroups.Fill(dutyGroups);
            dataAdapterItems.Fill(duties);

            treeDuties.Tables.Add(dutyGroups);
            treeDuties.Tables.Add(duties);
            treeDuties.Tables[0].TableName = "dutyGroups";
            treeDuties.Tables[1].TableName = "Duties";
            treeDuties.Relations.Add("GroupID_ParentId", treeDuties.Tables["dutyGroups"].Columns["ID"], treeDuties.Tables["dutyGroups"].Columns["PARENT_ID"]);
            treeDuties.Relations.Add("Id_GroupId", treeDuties.Tables["dutyGroups"].Columns["ID"], treeDuties.Tables["duties"].Columns["GROUP_ID"]);
            DutyTree.DataSource = treeDuties.Tables[0];
            DutyTree.DataFieldID = "ID";
            DutyTree.DataValueField = "ID";
            DutyTree.DataFieldParentID = "PARENT_ID";
            DutyTree.DataTextField = "TITLE";
            DutyTree.DataBind();

            db.disconnect();
        }

        protected void DutyTree_NodeDataBound(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            RadTreeNode groupNode = e.Node;
            DataRow[] grouptyp = treeDuties.Tables["dutyGroups"].Select("ID=" + e.Node.Value);
            e.Node.Attributes.Add("TYP", grouptyp[0]["TYP"].ToString());
            e.Node.Attributes.Add("ORDNUMBER", grouptyp[0]["ORDNUMBER"].ToString());
            addNodeImage(groupNode, grouptyp[0]["TYP"].ToString());
            DataRow[] groupRow = treeDuties.Tables["Duties"].Select("GROUP_ID=" + e.Node.Value);
            foreach (DataRow row in groupRow)
            {
                RadTreeNode Node = new RadTreeNode(row["TITLE"].ToString(), row["ID"].ToString());
                Node.Attributes.Add("TYP", row["TYP"].ToString());
                Node.Attributes.Add("ORDNUMBER", row["ORDNUMBER"].ToString());

                addNodeImage(Node, row["TYP"].ToString());
                groupNode.Nodes.Add(Node);
            }
        }

        private void addNodeImage(RadTreeNode node, string typ)
        {
            switch (typ)
            {
                case "GROUP":
                    node.ImageUrl = "../../Images/fb_aufgabengruppe.gif";
                    break;
                case "DUTY":
                    node.ImageUrl = "../../Images/fb_aufgaben.gif";
                    break;
            }
        }
    }
}
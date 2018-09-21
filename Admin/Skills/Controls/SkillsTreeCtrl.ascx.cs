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

namespace ch.appl.psoft.Admin.Skills.Controls
{
    public partial class SkillsTreeCtrl : System.Web.UI.UserControl
    {
        private DataSet treeSkills = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable skillGroups = new DataTable();
            DataTable skills = new DataTable();
            SqlConnection sqlcon = (SqlConnection)db.connection;
            SqlDataAdapter dataAdapterSkillGroups = new SqlDataAdapter("SELECT ID AS SKILLGROUP_ID, PARENT_ID AS SKILLGROUP_PARENT_ID, TITLE_" + lang + " AS SKILLGROUP_TITLE, 'GROUP' AS  TYP FROM SKILLGROUP ORDER BY TITLE_" + lang, sqlcon);
            SqlDataAdapter dataAdapterSkills = new SqlDataAdapter("SELECT SKILL_ID AS ID, SKILLGROUP_ID, ORDNUMBER, NUMBER, TITLE_" + lang + " AS TITLE, 'SKILL' AS TYP FROM SKILL_VALIDITY_V ORDER BY ORDNUMBER" , sqlcon);
            dataAdapterSkillGroups.Fill(skillGroups);
            dataAdapterSkills.Fill(skills);

            treeSkills.Tables.Add(skillGroups);
            treeSkills.Tables.Add(skills);
            treeSkills.Tables[0].TableName = "skillGroups";
            treeSkills.Tables[1].TableName = "skills";
            treeSkills.Relations.Add("SkillGroupID_ParentId", treeSkills.Tables["skillGroups"].Columns["SKILLGROUP_ID"], treeSkills.Tables["skillGroups"].Columns["SKILLGROUP_PARENT_ID"]);
            treeSkills.Relations.Add("SkillId_SkillGroupId", treeSkills.Tables["skillGroups"].Columns["SKILLGROUP_ID"], treeSkills.Tables["skills"].Columns["SKILLGROUP_ID"]);
            SkillTree.DataSource = treeSkills.Tables[0];
            SkillTree.DataFieldID = "SKILLGROUP_ID";
            SkillTree.DataValueField = "SKILLGROUP_ID";
            SkillTree.DataFieldParentID = "SKILLGROUP_PARENT_ID";
            SkillTree.DataTextField = "SKILLGROUP_TITLE";
            SkillTree.DataBind();

            db.disconnect();
        }

        protected void SkillTree_NodeDataBound(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            RadTreeNode groupNode = e.Node;
            DataRow[] grouptyp = treeSkills.Tables["skillGroups"].Select("SKILLGROUP_ID=" + e.Node.Value);
            e.Node.Attributes.Add("TYP", grouptyp[0]["TYP"].ToString());
            addNodeImage(groupNode, grouptyp[0]["TYP"].ToString());
            DataRow[] groupRow = treeSkills.Tables["skills"].Select("SKILLGROUP_ID=" + e.Node.Value);
            foreach (DataRow functionRow in groupRow)
            {
                RadTreeNode functionNode = new RadTreeNode(functionRow["TITLE"].ToString(), functionRow["ID"].ToString());
                functionNode.Attributes.Add("TYP", functionRow["TYP"].ToString());

                addNodeImage(functionNode, functionRow["TYP"].ToString());
                groupNode.Nodes.Add(functionNode);
            }
        }

        private void addNodeImage(RadTreeNode node, string typ)
        {
            switch (typ)
            {
                case "GROUP":
                    node.ImageUrl = "../../Images/sk_skillgruppe.gif";
                    break;
                case "SKILL":
                    node.ImageUrl = "../../Images/sk_skill.gif";
                    break;
            }
        }
    }
}
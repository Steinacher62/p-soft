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

namespace ch.appl.psoft.Admin.Controls
{
    public partial class OrganisationTreeCtrl : System.Web.UI.UserControl
    {
        private DataSet treeJobs = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable oeTree = new DataTable();
            DataTable jobs = new DataTable();
            SqlConnection sqlcon = (SqlConnection)db.connection;
            SqlDataAdapter dataAdapterOes = new SqlDataAdapter("SELECT ID AS OeId, PARENT_ID AS OeParentId, TITLE_DE AS OeTitleDe, TITLE_FR AS OeTitleFr, TITLE_EN AS OeTitleEn, CASE WHEN PARENT_ID IS NULL THEN 'FIRM' ELSE 'ORGENTITY' END AS TYP FROM ORGENTITY", sqlcon);
            SqlDataAdapter dataAdapterJobs = new SqlDataAdapter("SELECT PERSON.ID AS PersonId, PERSON.PNAME + ' ' + PERSON.FIRSTNAME AS Name, JOB.ID AS JobId, JOB.TITLE_DE AS JobDe, JOB.TITLE_FR AS JobFr, JOB.TITLE_EN AS JobEn, JOB.ORGENTITY_ID AS OeId, 'JOB' AS TYP "
                                                                + "FROM EMPLOYMENT INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID RIGHT OUTER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID ORDER BY JOB.TITLE_DE, PERSON.PNAME ", sqlcon);
            dataAdapterOes.Fill(oeTree);
            dataAdapterJobs.Fill(jobs);

            treeJobs.Tables.Add(oeTree);
            treeJobs.Tables.Add(jobs);
            treeJobs.Tables[0].TableName = "oeTree";
            treeJobs.Tables[1].TableName = "jobs";
            treeJobs.Relations.Add("OrgentityIdParentId", treeJobs.Tables["oeTree"].Columns["OeId"], treeJobs.Tables["oeTree"].Columns["OeParentId"]);
            treeJobs.Relations.Add("OrgentityIdJobs", treeJobs.Tables["oeTree"].Columns["OeId"], treeJobs.Tables["jobs"].Columns["OeId"]);
            OETree.DataSource = treeJobs.Tables[0];
            OETree.DataFieldID = "OeId";
            OETree.DataValueField = "OeId";
            OETree.DataFieldParentID = "OeParentId";
            OETree.DataTextField = "OeTitleDe";
            OETree.DataBind();

            db.disconnect();
        }

        protected void OETree_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            OrganisationTitle.Text = "Organisation";
            RadTreeNode orgNode = e.Node;
            DataRow[] oetyp = treeJobs.Tables["oetree"].Select("OeId=" + e.Node.Value);
            e.Node.Attributes.Add("TYP", oetyp[0]["TYP"].ToString());
            addNodeImage(orgNode, oetyp[0]["TYP"].ToString());
            DataRow[] oeRow = treeJobs.Tables["jobs"].Select("OeId=" + e.Node.Value);
            foreach (DataRow jobRow in oeRow)
            {
                RadTreeNode jobNode = new RadTreeNode(jobRow["JobDe"].ToString() + " " + jobRow["Name"].ToString(), jobRow["JobId"].ToString());
                jobNode.Attributes.Add("TYP", jobRow["TYP"].ToString());
                if (jobRow["PersonId"].ToString().Equals(""))
                {
                    jobNode.Attributes.Add("VAKANT", "TRUE");
                }
                else
                {
                    jobNode.Attributes.Add("VAKANT", "FALSE");
                }
                addNodeImage(jobNode, jobRow["TYP"].ToString());
                orgNode.Nodes.Add(jobNode);
            }
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
                case "JOB":
                    if (node.Attributes["VAKANT"].Equals("FALSE"))
                    {
                        node.ImageUrl = "../Images/og_stelle.gif";
                    }
                    else
                    {
                        node.ImageUrl = "../Images/og_stelle_vakant.gif";
                    }

                    break;
            }
        }
    }
}
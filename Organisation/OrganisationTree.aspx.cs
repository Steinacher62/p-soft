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



namespace ch.appl.psoft.Organisation
{
    public partial class OrganisationTree : PsoftDetailPage
    {
        private DataSet treeJobs = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            // Setting default breadcrumb caption
            this.BreadcrumbCaption = "Organisation";
            if (!IsPostBack)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                DataTable oeTree = new DataTable();
                DataTable jobs = new DataTable();
                SqlConnection sqlcon = (SqlConnection)db.connection;
                SqlDataAdapter dataAdapterOes = new SqlDataAdapter("SELECT ID AS OeId, PARENT_ID AS OeParentId, TITLE_DE AS OeTitleDe, TITLE_FR AS OeTitleFr, TITLE_EN AS OeTitleEn  FROM ORGENTITY", sqlcon);
                SqlDataAdapter dataAdapterJobs = new SqlDataAdapter("SELECT PERSON.ID AS PersonId, PERSON.PNAME + ' ' + PERSON.FIRSTNAME AS Name, JOB.ID AS JobId, JOB.TITLE_DE AS JobDe, JOB.TITLE_FR AS JobFr, JOB.TITLE_EN AS JobEn, JOB.ORGENTITY_ID AS OeId "
                                                  + "FROM EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID", sqlcon);
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
                //foreach (DataRow oeRow in (DataRow)OETree.DataSourceObject.GetType())
                //{
                //    RadTreeNode orgentityNode = new RadTreeNode(orgentityRow["OeTitleDe"].ToString());
                //    OETree.Nodes.Add(orgentityNode);
                //    foreach (DataRow jobRow in orgentityRow.GetChildRows("OeId"))
                //    {
                //        RadTreeNode jobNode = new RadTreeNode(jobRow["Name"].ToString());
                //        orgentityNode.Nodes.Add(jobNode);
                //    }
                //}

            }
            
            
        }

        protected void OETree_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            RadTreeNode orgNode = e.Node;
            DataRow[] oeRow = treeJobs.Tables["jobs"].Select("OeId=" + e.Node.Value);
            foreach (DataRow jobRow in oeRow)
            {
                RadTreeNode jobNode = new RadTreeNode(jobRow["Name"].ToString() + " " + jobRow["JobDe"].ToString(),jobRow["JobId"].ToString());
                
                orgNode.Nodes.Add(jobNode);
            }
        }
    }
}
﻿using ch.appl.psoft.Common;
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
    public partial class FunctionTreeCtrl : System.Web.UI.UserControl
    {
        private DataSet treeFunctions = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable functionGroups = new DataTable();
            DataTable functions = new DataTable();
            SqlConnection sqlcon = (SqlConnection)db.connection;
            SqlDataAdapter dataAdapterFunctionGroups = new SqlDataAdapter("SELECT ID AS FUNCTIONGROUP_ID, PARENT_ID AS FUNCTIONGROUP_PARENT_ID, TITLE_" + lang + " AS FUNCTIONGROUP_TITLE, 'GROUP' AS  TYP FROM FUNKTION_GROUP ORDER BY TITLE_" + lang, sqlcon);
            SqlDataAdapter dataAdapterFunctions = new SqlDataAdapter("SELECT ID, FUNKTION_GROUP_ID, TITLE_" + lang + " AS TITLE, 'FUNCTION' AS TYP FROM FUNKTION ORDER BY TITLE_" + lang, sqlcon);
            dataAdapterFunctionGroups.Fill(functionGroups);
            dataAdapterFunctions.Fill(functions);

            treeFunctions.Tables.Add(functionGroups);
            treeFunctions.Tables.Add(functions);
            treeFunctions.Tables[0].TableName = "functionGroups";
            treeFunctions.Tables[1].TableName = "functions";
            treeFunctions.Relations.Add("FunctionGroupID_ParentId", treeFunctions.Tables["functionGroups"].Columns["FUNCTIONGROUP_ID"], treeFunctions.Tables["functionGroups"].Columns["FUNCTIONGROUP_PARENT_ID"]);
            treeFunctions.Relations.Add("FunctionId_FunctionGroupId", treeFunctions.Tables["functionGroups"].Columns["FUNCTIONGROUP_ID"], treeFunctions.Tables["functions"].Columns["FUNKTION_GROUP_ID"]);
            FunctionTree.DataSource = treeFunctions.Tables[0];
            FunctionTree.DataFieldID = "FUNCTIONGROUP_ID";
            FunctionTree.DataValueField = "FUNCTIONGROUP_ID";
            FunctionTree.DataFieldParentID = "FUNCTIONGROUP_PARENT_ID";
            FunctionTree.DataTextField = "FUNCTIONGROUP_TITLE";
            FunctionTree.DataBind();

            db.disconnect();
        }

        protected void FunctionTree_NodeDataBound(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            RadTreeNode groupNode = e.Node;
            DataRow[] grouptyp = treeFunctions.Tables["functionGroups"].Select("FUNCTIONGROUP_ID=" + e.Node.Value);
            e.Node.Attributes.Add("TYP", grouptyp[0]["TYP"].ToString());
            addNodeImage(groupNode, grouptyp[0]["TYP"].ToString());
            DataRow[] groupRow = treeFunctions.Tables["functions"].Select("FUNKTION_GROUP_ID=" + e.Node.Value);
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
                    node.ImageUrl = "../../Images/fx_funktionsgruppe.gif";
                    break;
                case "FUNCTION":
                    node.ImageUrl = "../../Images/fx_funktion.gif";
                    break;
            }
        }
    }
}
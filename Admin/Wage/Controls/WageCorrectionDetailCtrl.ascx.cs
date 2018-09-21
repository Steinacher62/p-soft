using ch.appl.psoft.Admin.Chart.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Wage.Controls
{
    public partial class WageCorrectionDetailCtrl : System.Web.UI.UserControl
    {
        protected DataTable solllohnkorrektur;
        protected string sql = "";
        protected string anzMonatsloehne = Global.Config.getModuleParam("report", "anzMonatsloehne", "13");
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientDataSource.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/PsoftService1.asmx/";

            if (!Page.IsPostBack)
            {
                TableSalaryCorrection.Columns[6].HeaderText = Global.Config.getModuleParam("report", "korr1", "korr1");
                TableSalaryCorrection.Columns[7].HeaderText = Global.Config.getModuleParam("report", "korr2", "korr2");
                TableSalaryCorrection.Columns[8].HeaderText = Global.Config.getModuleParam("report", "korr3", "korr3");
                TableSalaryCorrection.Columns[9].HeaderText = Global.Config.getModuleParam("report", "korr4", "korr4");
                TableSalaryCorrection.Columns[10].HeaderText = Global.Config.getModuleParam("report", "fix", "fix");
            }

            TableSalaryCorrection.CommandItemStyle.CssClass = "FileExplorerToolbar";
            SolllohnkorrekturLabel.Text = "Solllohnkorrekturen";
        }
    }
}
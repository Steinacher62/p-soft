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

namespace ch.appl.psoft.Admin.Wage
{
    public partial class WageListDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }
            ClientDataSource.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/PsoftService1.asmx/";

            WageGridLabel.Text = map.get("WageTable", "WAGELIST");
            WageGrid.MasterTableView.Columns[1].HeaderText = map.get("WageTable", "OrgentityTitle");
            WageGrid.MasterTableView.Columns[2].HeaderText = map.get("WageTable", "PERSONNELNUMBER");
            WageGrid.MasterTableView.Columns[3].HeaderText = map.get("WageTable", "PNAME");
            WageGrid.MasterTableView.Columns[4].HeaderText = map.get("WageTable", "FIRSTNAME");
            WageGrid.MasterTableView.Columns[5].HeaderText = map.get("WageTable", "JobTitle");
            WageGrid.MasterTableView.Columns[6].HeaderText = map.get("WageTable", "WAGE");
            WageGrid.MasterTableView.Columns[7].HeaderText = map.get("WageTable", "AUSSCHLUSS");
            WageGrid.MasterTableView.Columns[8].HeaderText = map.get("WageTable", "AUSSCHLUSSLOHN");
            WageGrid.MasterTableView.Columns[9].HeaderText = map.get("WageTable", "AUSSCHLUSSBIS");





            CultureInfo newCulture = CultureInfo.CreateSpecificCulture("de-DE");
            WageGrid.Culture = newCulture;
            WageGrid.CommandItemStyle.CssClass = "FileExplorerToolbar";
        }
    }
}
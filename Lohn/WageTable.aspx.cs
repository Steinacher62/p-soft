using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;



namespace ch.appl.psoft.Lohn
{
    public partial class WageTable : System.Web.UI.Page
    {
        private DataTable wage = new DataTable();
        private string sql;
        private LanguageMapper map;
        protected void Page_Load(object sender, EventArgs e)
        {
            //HtmlMeta tag = new HtmlMeta();
            //tag.Name = "Pragma";
            //tag.Content = "no-cache";
            //Header.Controls.Add(tag);
            //HtmlMeta tag1 = new HtmlMeta();
            //tag1.Name = "Cache-Control";
            //tag1.Content = "no-cache";
            //Header.Controls.Add(tag1);
            //HtmlMeta tag2 = new HtmlMeta();
            //tag2.Name = "Expires";
            //tag2.Content = "-1";
            //Header.Controls.Add(tag2);


            //Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();



            // apply language
            map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }
            ClientDataSource.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/PsoftService1.asmx/";


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
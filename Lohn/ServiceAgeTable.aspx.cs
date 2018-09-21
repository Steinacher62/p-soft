using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Data;
using Telerik.Web.UI;

namespace ch.appl.psoft.Lohn
{
    public partial class ServiceAgeTable : System.Web.UI.Page
    {
        protected DataTable dienstalter;
        protected string sql = "";

        protected void Page_Load(object sender, EventArgs e)
        {

                // apply language
                LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
                if (map == null)
                {
                    map = LanguageMapper.getLanguageMapper(Application);
                }
                ServiceAgeLabel.Text = map.get("AgeTables", "table") + " " + map.get("AgeTables", "ServiceAge");
        }

        protected void ServiceAgeGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();

            dienstalter = db.getDataTable("SELECT Schluessel,Wert FROM DIENSTALTER");
            dienstalter.PrimaryKey = new DataColumn[] { dienstalter.Columns[0] };
            db.disconnect();
            this.ServiceAgeGrid.DataSource = dienstalter;
        }

        protected void ServiceAgeGrid_UpdateCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            var editableItem = ((GridEditableItem)e.Item);
            Hashtable newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editableItem);
            db.execute("UPDATE DIENSTALTER SET WERT = " + newValues["WERT"].ToString().Replace(",", ".") + "WHERE SCHLUESSEL=" + editableItem.GetDataKeyValue("SCHLUESSEL"));
        }
    }
}

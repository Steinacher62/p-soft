using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;

namespace ch.appl.psoft.Lohn
{
    public partial class AgeTable : System.Web.UI.Page
    {
        protected DataTable lebensalter;
        protected string sql = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // apply language
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            if (map == null)
            {
                map = LanguageMapper.getLanguageMapper(Application);
            }

            AgeTableLabel.Text = map.get("AgeTables", "table") + " " + map.get("AgeTables", "LiveAge");
        }

        protected void AgeTableGrid_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            var editableItem = ((GridEditableItem)e.Item);
            Hashtable newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editableItem);
            db.execute("UPDATE LEBENSALTER SET WERT = " + newValues["WERT"].ToString().Replace(",",".") + "WHERE SCHLUESSEL=" + editableItem.GetDataKeyValue("SCHLUESSEL") );
        }

        protected void AgeTableGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();

            lebensalter = db.getDataTable("SELECT Schluessel,Wert FROM Lebensalter");
            lebensalter.PrimaryKey = new DataColumn[] { lebensalter.Columns[0] };
            db.disconnect();
            this.AgeTableGrid.DataSource = lebensalter;
            
        }

        protected void AgeTableLabelDataSource_Selecting(object sender, System.Web.UI.WebControls.EntityDataSourceSelectingEventArgs e)
        {

        }
    }
}

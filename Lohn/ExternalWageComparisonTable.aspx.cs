using ch.appl.psoft.db;
using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;

namespace ch.appl.psoft.Lohn
{
    public partial class ExternalWageComparisonTable : System.Web.UI.Page
    {
        protected DataTable externalComparison;
        protected string sql = "";
        protected string anzMonatsloehne = Global.Config.getModuleParam("report", "anzMonatsloehne", "13");

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ExternalComparison_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            externalComparison = db.getDataTable("SELECT LOHNVERGLEICH_EXTERN.ID, FUNKTION.ID AS Funktion_ID, FUNKTION.TITLE_DE, LOHNVERGLEICH_EXTERN.EXTERN_FUNKTION_ID, LOHNVERGLEICH_EXTERN.BEZEICHNUNG, LOHNVERGLEICH_EXTERN.EXTERN_SOLL1, LOHNVERGLEICH_EXTERN.EXTERN_SOLL2, LOHNVERGLEICH_EXTERN.EXTERN_SOLL3, LOHNVERGLEICH_EXTERN.EXTERN_SOLL4 FROM FUNKTION LEFT OUTER JOIN LOHNVERGLEICH_EXTERN ON FUNKTION.ID = LOHNVERGLEICH_EXTERN.FUNKTION_ID");
            db.disconnect();
            TableExternalComparison.DataSource = externalComparison;
        }

        protected void ExternalComparison_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            //save changes to db
            DBData db = DBData.getDBData(Session);
            db.connect();
            var editableItem = ((GridEditableItem)e.Item);
            Hashtable newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editableItem);
            long update = (long)db.lookup("FUNKTION_ID", "LOHNVERGLEICH_EXTERN", "FUNKTION_ID =" + newValues["Funktion_Id"], 0L);
            if (update == 0)
                db.execute("INSERT INTO LOHNVERGLEICH_EXTERN (FUNKTION_ID, EXTERN_FUNKTION_ID, BEZEICHNUNG, EXTERN_SOLL1, EXTERN_SOLL2, EXTERN_SOLL3, EXTERN_SOLL4) VALUES (" + newValues["Funktion_Id"] + ", '" + newValues["Extern_Funktion_Id"] + "', '" + newValues["Bezeichnung"] + "', '" + newValues["Extern_Soll1"] + "', '" + newValues["Extern_Soll2"] + "', '" + newValues["Extern_Soll3"] + "', '" + newValues["Extern_Soll4"] + "')");
            else
                db.execute("Update LOHNVERGLEICH_EXTERN SET FUNKTION_ID = " + newValues["Funktion_Id"] + ", EXTERN_FUNKTION_ID = '" + newValues["Extern_Funktion_Id"] + "', BEZEICHNUNG = '" + newValues["Bezeichnung"] + "', EXTERN_SOLL1 = '" + newValues["Extern_Soll1"] + "', EXTERN_SOLL2 = '" + newValues["Extern_Soll2"] + "', EXTERN_SOLL3 = '" + newValues["Extern_Soll3"] + "', EXTERN_SOLL4 = '" + newValues["Extern_Soll4"] + "' WHERE ID =  " + newValues["ID"]);
            db.disconnect();
        }
    }
}

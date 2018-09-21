using ch.appl.psoft.db;
using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;

namespace ch.appl.psoft.Lohn
{
    public partial class SalaryCorrection : System.Web.UI.Page
    {
        protected DataTable solllohnkorrektur;
        protected string sql = "";
        protected string anzMonatsloehne = Global.Config.getModuleParam("report", "anzMonatsloehne", "13");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TableSalaryCorrection.Columns[5].HeaderText = Global.Config.getModuleParam("report", "korr1", "korr1");
                TableSalaryCorrection.Columns[6].HeaderText =  Global.Config.getModuleParam("report", "korr2", "korr2");
                TableSalaryCorrection.Columns[7].HeaderText = Global.Config.getModuleParam("report", "korr3", "korr3");
                TableSalaryCorrection.Columns[8].HeaderText = Global.Config.getModuleParam("report", "korr4", "korr4");
                TableSalaryCorrection.Columns[9].HeaderText = Global.Config.getModuleParam("report", "fix", "fix");
            }
        }

        protected void TableSalaryCorrection_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();


            solllohnkorrektur = db.getDataTable("SELECT PERSONNELNUMBER,JobTitel, PNAME,FIRSTNAME,IstLohn,Korr1,Korr2,Korr3,Korr4,fix FROM f_Solllohnkorrektur(" + anzMonatsloehne + ") ORDER BY JobTitel, PNAME");
            solllohnkorrektur.PrimaryKey = new DataColumn[] { solllohnkorrektur.Columns[0] };

            db.disconnect();
            TableSalaryCorrection.DataSource = solllohnkorrektur;
        }

        protected void TableSalaryCorrection_UpdateCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            var editableItem = ((GridEditableItem)e.Item);
            Hashtable newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editableItem);
            string schluessel = "-1";
            try
            {
                schluessel = editableItem.GetDataKeyValue("PERSONNELNUMBER").ToString();
            }
            catch
            {
                return;
            }
            DBData db = DBData.getDBData(Session);
            db.connect();
            if (db.lookup("PERSONNELNUMBER", "SOLLLOHNKORREKTUR", "PERSONNELNUMBER = '" + editableItem.GetDataKeyValue("PERSONNELNUMBER") + "'", "NULL") != "NULL")
            {
                sql += "UPDATE SOLLLOHNKORREKTUR SET Korr1 = '" + newValues["Korr1"] + "', Korr2 =  '" + newValues["Korr2"] + "', Korr3 =  '" + newValues["Korr3"] + "', Korr4 =  '" + newValues["Korr4"] + "', fix =  '" + newValues["fix"] + "'  WHERE PERSONNELNUMBER = '" + editableItem.GetDataKeyValue("PERSONNELNUMBER") + "'";
            }
            else
            {
                sql += "INSERT INTO SOLLLOHNKORREKTUR (PERSONNELNUMBER, Korr1, Korr2, Korr3, Korr4, fix) VALUES('" + editableItem.GetDataKeyValue("PERSONNELNUMBER") + "', '" + newValues["Korr1"] + "', '" + newValues["Korr2"] + "', '" + newValues["Korr3"] + "', '" + newValues["Korr4"] + "', '" + newValues["fix"] + "')";
            }

            db.execute(sql);

            db.disconnect();
        }
    }
}

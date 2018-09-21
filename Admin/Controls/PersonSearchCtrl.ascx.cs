using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Controls
{
    public partial class PersonSearchCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            LabelTitle.Text = "Person";
            LabelName.Text = "Name";
            LabelFirstname.Text = "Vorname";
            LabelTBMNEMO.Text = "Kürzel";
            LabelPersonnelnumber.Text = "Personalnummer";
            LabelOrgentity.Text = "Abteilung";
            DataTable tblOE = db.getDataTableExt("SELECT ID," + db.langAttrName("ORGENTITY", "TITLE") + " AS TITLE FROM ORGENTITY ORDER BY TITLE_DE", new object[0]);
            DataRow emptyRow = tblOE.NewRow();
            emptyRow["ID"] = 0;
            emptyRow["TITLE"] = "";
            tblOE.Rows.Add(emptyRow);
            tblOE.DefaultView.Sort = "TITLE";
            DDOrgentity.DataTextField = "Title";
            DDOrgentity.DataValueField = "ID";
            DDOrgentity.DataSource = tblOE;
            DDOrgentity.DataBind();
           
            //foreach (DataRow aktRow in tblOE.Rows)
            //{
            //    //lstOE.Items.Add(new ListItem(aktRow["TITLE_DE"].ToString(), aktRow["ID"].ToString()));
            //}

            LabelShowFormer.Text = "Ehemalige anzeigen";
        }
    }
}
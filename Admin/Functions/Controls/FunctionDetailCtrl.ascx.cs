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

namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class FunctionDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LabelTitle.Text = _map.get("function", "function");
            LabelGroup.Text = _map.get("function", "group");
            LabelName.Text = _map.get("function", "name");
            LabelNameShort.Text = _map.get("function", "nameShort");
            LabelDescription.Text = _map.get("function", "description");
            LabelDefault.Text = _map.get("function", "default");
            LabelTyp.Text = _map.get("function", "typ");
            LabelFBWRevision.Text = _map.get("function", "fbwRevision");
            LabelBonusPart.Text = _map.get("function", "BonusPart");
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable functionTypes = db.getDataTable("SELECT ID, BEZEICHNUNG FROM FUNKTION_TYP ORDER BY BEZEICHNUNG");
            DataRow emtyRow = functionTypes.NewRow();
            emtyRow["ID"] = 0;
            functionTypes.Rows.Add(emtyRow);
            functionTypes.DefaultView.Sort = "[BEZEICHNUNG] ASC";
            
            db.disconnect();
            TypeData.DataTextField = "BEZEICHNUNG";
            TypeData.DataValueField = "ID";
            TypeData.DataSource = functionTypes;
            TypeData.DataBind();
        }
    }
}
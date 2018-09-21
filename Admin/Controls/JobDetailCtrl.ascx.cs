using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Admin.Controls
{
    public partial class JobDetailCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();

            JobFormTitle.Text = _map.get("organisation", "job");
            JobTitle.Text = _map.get("JOB", "TITLE");
            OeTitle.Text = _map.get("organisation", "OE");
            FunktionTitle.Text = _map.get("JOB", "FUNKTION_ID");
            FunktionData.DataSource = db.getDataTable("SELECT ID, TITLE_"+_map.LanguageCode + " AS TITLE FROM FUNKTION");
            FunktionData.DataTextField = "TITLE";
            FunktionData.DataValueField = "ID";
            FunktionData.DataBind();
            FunktionData.Items.Insert(0, "");

            EmploymentTitle.Text = _map.get("JOB", "EMPLOYMENT_ID");
            EmploymentButton.Text = _map.get("JOB", "vacant");
            JobSetVacant.Text = _map.get("JOB", "SET_VACANT");
            SubstituteTitle.Text = _map.get("JOB", "PROXY_PERSON_ID");
            SubstituteButton.Text = _map.get("JOB", "vacant");
            SubstituteButton.Value = "vacant";
            SubstituteSetVacant.Text = _map.get("JOB", "SET_VACANT");
            SubstituteSetVacant.Enabled = false;

            FromTitle.Text = _map.get("JOB", "FROM_DATE");
            ToTitle.Text = _map.get("JOB", "TO_DATE");
            MnemonicJobTitle.Text = _map.get("JOB", "MNEMONIC");
            EngagementTitle.Text = _map.get("JOB", "ENGAGEMENT");
            DescriptionJobTitle.Text = _map.get("JOB", "DESCRIPTION");
            TypeTitle.Text = _map.get("JOB", "TYP");
            TypeData.DataSource = Adminutilities.GetIListFromXML(Session, "job", "typ", true);
            TypeData.DataValueField = "ID";
            TypeData.DataTextField = "ENTRY";
            TypeData.DataBind();
            MainFunctionTitle.Text = _map.get("JOB", "HAUPTFUNKTION");
        }
        public class typList
        {
            public int Id { get; set; }
            public string Typ { get; set; }
        }

    }
}
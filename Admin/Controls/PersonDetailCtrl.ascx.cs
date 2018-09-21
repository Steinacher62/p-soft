using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Controls
{
    public partial class PersonDetailCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();
            PersonTitle.Text = "Person";
            FirmTitle.Text = _map.get("PERSON", "FIRM");
            ClipboardTitle.Text = _map.get("PERSON", "CLIPBOARD_ID");
            NameTitle.Text = _map.get("PERSON", "PNAME");
            FirstnameTitle.Text = _map.get("PERSON", "FIRSTNAME");
            MNEMOTitle.Text = _map.get("PERSON", "MNEMO");
            TitleTitle.Text = _map.get("PERSON", "TITLE");
            PersonnelnumberTitle.Text = _map.get("PERSON", "PERSONNELNUMBER");
            if (Global.isModuleEnabled("morph"))
            {
                RequiredFieldValidatorPersonnelnumber.Visible = false;
            }
            SexTitle.Text = _map.get("PERSON", "SEX");
            MartialTitle.Text = _map.get("PERSON", "MARTIAL");
            DateOfBirthTitle.Text = _map.get("PERSON", "DATEOFBIRTH");
            Entrytitle.Text = _map.get("PERSON", "ENTRY");
            LeavingTitle.Text = _map.get("PERSON", "LEAVING");
            LoginTitle.Text = _map.get("PERSON", "LOGIN");
            PasswordTitle.Text = _map.get("PERSON", "PASSWORD");
            EMailTitle.Text = _map.get("PERSON", "EMAIL");
            PhoneTitle.Text = _map.get("PERSON", "PHONE");
            MobileTitle.Text = _map.get("PERSON", "MOBILE");
            PhotoTitle.Text = _map.get("PERSON", "PHOTO");
            SalutationAddressTitle.Text = _map.get("PERSON", "SALUTATION_ADDRESS");
            SalutationLetterTitle.Text = _map.get("PERSON", "SALUTATION_LETTER");
            BeschGradTitle.Text = _map.get("PERSON", "LEVELOFEMPLOYMENT");
            BerufserfahrungTitle.Text = _map.get("PERSON", "WORKEXPERIENCE");
            LeaderShipTitle.Text = _map.get("PERSON", "LEADERSHIP");
            CommentTitle.Text = _map.get("PERSON", "COMMENT");

            DataTable FirmTable = db.getDataTable("SELECT ID, TITLE FROM FIRM");
            FirmData.DataSource = Adminutilities.GetDropDownTable(FirmTable, "ID", "TITLE", true);
            FirmData.DataValueField = "ID";
            FirmData.DataTextField = "TITLE";
            FirmData.DataBind();

            MartialData.DataSource = Adminutilities.GetIListFromXML(Session, "person", "martial", false);
            MartialData.DataValueField = "ID";
            MartialData.DataTextField = "ENTRY";
            MartialData.DataBind();

            SexData.DataSource = Adminutilities.GetIListFromXML(Session, "person", "sex", false);
            SexData.DataValueField = "ID";
            SexData.DataTextField = "ENTRY";
            SexData.DataBind();





            db.disconnect();

        }
    }
}
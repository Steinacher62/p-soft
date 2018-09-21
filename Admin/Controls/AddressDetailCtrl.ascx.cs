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
    public partial class AddressDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            AddressTitle.Text = _map.get("ADDRESS", "NAME");
            AddressLabel1.Text = _map.get("ADDRESS", "ADDRESS1");
            AddressLabel2.Text = _map.get("ADDRESS", "ADDRESS2");
            AddressLabel3.Text = _map.get("ADDRESS", "ADDRESS3");
            AddressLabelZip.Text = _map.get("ADDRESS", "ZIP");
            AddressLabelCity.Text = _map.get("ADDRESS", "CITY");
            AddressLabelCountry.Text = _map.get("ADDRESS", "COUNTRY");
            AddressLabelPhone.Text = _map.get("ADDRESS", "PHONE");
            AddressLabelFax.Text = _map.get("ADDRESS", "FAX");
            AddressLabelMobil.Text = _map.get("ADDRESS", "MOBILE");
            AddressLabelEMail.Text = _map.get("ADDRESS", "EMAIL_PRIVATE");
            AddessDataCountry.DataSource = Adminutilities.GetIListFromXML(Session, "ADDRESS", "COUNTRYLIST", false);
            AddessDataCountry.DataValueField = "ID";
            AddessDataCountry.DataTextField = "ENTRY";
            AddessDataCountry.DataBind();
        }
    }
}
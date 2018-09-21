using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Payment
{
    public partial class Pay : PsoftMainPage
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            productId.Value = Session["OrderItem"].ToString();
            this.BreadcrumbCaption = _mapper.get("ModuleNames", "pay");
            this.BreadcrumbLink = Global.Config.baseURL + "/Payment/Pay.aspx";
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable productTable = db.getDataTable("Select * FROM PRODUCTS WHERE ID =" + Session["OrderItem"]);

            ProductTitle.Text = productTable.Rows[0]["TITLE"].ToString();
            Productdescription.Text = productTable.Rows[0]["DESCRIPTION"].ToString();
            ProductPrice.Text = productTable.Rows[0]["CURRENCY"].ToString() + " " + productTable.Rows[0]["PRICE"].ToString();

            ImageCreditCards.ImageUrl = Global.Config.baseURL + "/IMAGES/Creditcards.png";
            Imageprepayment.ImageUrl = Global.Config.baseURL + "/IMAGES/Prepayment.png";
            ImagePaypal.ImageUrl = Global.Config.baseURL + "/IMAGES/PaypalButton.png";
            string name = db.lookup("PNAME", "PERSON", "ID=" + db.userId).ToString();
            string firstname = db.lookup("FIRSTNAME", "PERSON", "ID=" + db.userId).ToString();
            tbName.Text = name;
            tbNameAddress.Text = name;
            tbFirstname.Text = firstname;
            tbFirstnameAddress.Text = firstname;

            ExpirationDate.MinDate = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 01);

            ExpirationDate.MonthYearNavigationSettings.TodayButtonCaption = "Heute";
            ExpirationDate.MonthYearNavigationSettings.EnableTodayButtonSelection = false;
            ExpirationDate.MonthYearNavigationSettings.CancelButtonCaption = "Abbrechen";
            ExpirationDate.MonthYearNavigationSettings.NavigationNextToolTip = "Weiter";
            ExpirationDate.MonthYearNavigationSettings.NavigationPrevToolTip = "Zurück";

            DataTable addressTable = db.getDataTable("SELECT ADDRESS.ADDRESS1, ADDRESS.ZIP, ADDRESS.CITY, ADDRESS.COUNTRY, FIRM.TITLE AS FIRM, FIRM.ID AS FIRM_ID "
                                                + "FROM ADDRESS LEFT OUTER JOIN "
                                                + "dbo.FIRM ON ADDRESS.FIRM_ID = FIRM.ID WHERE ADDRESS.PERSON_ID =" + db.userId);
            string defaultCountry = "";
            string countryCode = "";
            if (addressTable.Rows.Count > 0)
            { 
                tbFirmAddress.Text = addressTable.Rows[0]["FIRM"].ToString();
                tbStreetAddress.Text = addressTable.Rows[0]["ADDRESS1"].ToString();
                tbZipCodeAddress.Text = addressTable.Rows[0]["ZIP"].ToString();
                tbCityAddress.Text = addressTable.Rows[0]["CITY"].ToString();
                countryCode = addressTable.Rows[0]["Country"].ToString();
            }
            List<CountryAndCode> CountryList = new List<CountryAndCode>();
            CountryList = GetCoutries(defaultCountry);

            foreach (CountryAndCode CAC in CountryList)
            {
                DropDownListItem Dditem = new DropDownListItem();
                Dditem.Text = CAC.Country;
                Dditem.Value = CAC.ISOcode;
                tbCountryAddress.Items.Add(Dditem);
            }
           tbCountryAddress.Items.Sort();
            DropDownListItem de = new DropDownListItem("Deutschland", "DE");
            DropDownListItem at = new DropDownListItem("Östereich", "AT");
            DropDownListItem ch = new DropDownListItem("Schweiz", "CH");

            tbCountryAddress.Items.Insert(1, de);
            tbCountryAddress.Items.Insert(2, at);
            tbCountryAddress.Items.Insert(3, ch);

            if (countryCode != "")
            {
                tbCountryAddress.FindItemByValue(countryCode).Selected=true;
            } 

        }

        private List<CountryAndCode> GetCoutries(string defaultCountry)
        {
            List<CountryAndCode> list = new List<CountryAndCode>();
            if (defaultCountry.Equals(""))
            {
                list.Add(new CountryAndCode(" ", " "));
            }
     
            foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo inforeg = new RegionInfo(info.LCID);

                CountryAndCode tCountryAndcode = new CountryAndCode(inforeg.DisplayName.ToString(), inforeg.TwoLetterISORegionName.ToString());

                bool addRec = true;
                foreach (CountryAndCode CAC in list)
                {
                    if (CAC.Country == inforeg.DisplayName)
                    {
                        addRec = false;
                    }
                }

                if (addRec)
                {
                    list.Add(new CountryAndCode(inforeg.DisplayName.ToString(), inforeg.TwoLetterISORegionName.ToString()));
                }
            }

            return list.Distinct().ToList();
        }

        class CountryAndCode
        {
            private string v1;
            private string v2;

            public CountryAndCode(string v1, string v2)
            {
                this.Country = v1;
                this.ISOcode = v2;
            }

            [DataMember]
            public string Country { get; set; }

            [DataMember]
            public string ISOcode { get; set; }
        }
    }
}
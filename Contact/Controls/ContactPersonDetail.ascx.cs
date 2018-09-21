namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.IO;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for ContactPersonDetail.
    /// </summary>
    public partial class ContactPersonDetail : PSOFTMapperUserControl
	{

		private long _ID = -1;
        private long _contactGroupID = -1;

        protected System.Web.UI.HtmlControls.HtmlTableCell photoCell;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactPersonDetail.ascx";}
		}

		#region Properties
        public long PersonID
        {
            get {return _ID;}
            set {_ID = value;}
        }

        public long ContactGroupID
        {
            get {return _contactGroupID;}
            set {_contactGroupID = value;}
        }

        public string PersonName
		{
			get 
			{
				return GetPersonName();
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Execute();
		}

		protected override void DoExecute()
		{
			base.DoExecute();
            string sqlStatement = "SELECT * FROM PERSON WHERE ID = " + _ID;

			//get data from database
			DBData db = DBData.getDBData(Session);
			System.Data.DataTable data = null;
			System.Data.DataTable dataAddress = null;
			System.Data.DataTable dataCompany = null;

			try 
			{
				db.connect();
				data = db.getDataTable(sqlStatement);

				//fill dynamic person table information 
				if (data.Rows.Count > 0)
				{
                    AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON","PHONE"), DBData.getValue(data, 0, "PHONE").ToString());
                    AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON","MOBILE"), DBData.getValue(data, 0, "MOBILE").ToString());

                    string email = DBData.getValue(data, 0, "EMAIL").ToString();
                    if (email != ""){
                        EMailLink emailLink = new EMailLink();
                        emailLink.Text = emailLink.NavigateUrl = email;
                        AddressBuilder.AppendDetailControl(personDetailTable, null, emailLink);
                    }

                    string photo = DBData.getValue(data, 0, "PHOTO").ToString();
					if (photo.Equals(""))
						photoCell.Visible = false;
					else 
					{
						FileInfo photoFile = new FileInfo(Global.Config.personPhotoDirectory + "//" + photo);

                        if (photoFile.Exists)
                        {
                            PHOTO.ImageUrl = "../photo/" + photo;
                            PHOTO.AlternateText = db.Person.getWholeName(data.Rows[0]["ID"].ToString(), false, true, false);
                        }
                        else
                            photoCell.Visible = false;
					}

					//get company address information
                    long firmID = ch.psoft.Util.Validate.GetValid(DBData.getValue(data, 0, "FIRM_ID").ToString(), -1);
					sqlStatement = "SELECT * FROM FIRM WHERE ID=" + firmID;
					dataCompany = db.getDataTable(sqlStatement);

                    if (dataCompany.Rows.Count > 0) {
                        HyperLink companyLink = new HyperLink();
                        companyLink.Text = DBData.getValue(dataCompany, 0, "TITLE").ToString();
                        companyLink.CssClass = "detail_link";
                        if(!DBData.getValue(dataCompany, 0, "WEBSITE").ToString().Equals("")) {
                            companyLink.NavigateUrl = Global.Config.baseURL + "/Contact/ContactDetail.aspx?ID=" + firmID;
                        }
                        AddressBuilder.AppendDetailControl(companyAddressTable, null, companyLink);

                        //get company address detail info
                        sqlStatement = "SELECT * FROM ADDRESS WHERE FIRM_ID = " + DBData.getValue(dataCompany, 0, "ID").ToString();
                        dataAddress = db.getDataTable(sqlStatement);

                        if (dataAddress.Rows.Count > 0) {
                            AddressBuilder.AppendAddressBlock(companyAddressTable, dataAddress, 0);
                            AddressBuilder.AppendAddressExtensions(companyAddressTable, dataAddress, 0, _mapper);
                        }

                        string companyEmail = DBData.getValue(dataCompany, 0, "EMAIL").ToString();
                        if (companyEmail != ""){
                            EMailLink companyEmailLink = new EMailLink();
                            companyEmailLink.Text = companyEmailLink.NavigateUrl = companyEmail;
                            AddressBuilder.AppendDetailControl(companyAddressTable, null, companyEmailLink);
                        }
                    }

					//fill dynamic person address information
					sqlStatement = "SELECT * FROM ADDRESS WHERE PERSON_ID = " + _ID;
					dataAddress = db.getDataTable(sqlStatement);
					for (int i = 0; i < dataAddress.Rows.Count; i++)
						AddressBuilder.BuildAddressTable(addressTable, dataAddress, i, _mapper);

				}
			
			}
			catch (Exception ex) 
			{
				DoOnException(ex);
			}
			finally 
			{
				db.disconnect();
			}
		}

        private string GetPersonName()
		{
			//get data from database
			DBData db = DBData.getDBData(Session);

            try 
			{
				db.connect();
				return db.Person.getWholeName(_ID.ToString(), false);
			}
			catch (Exception ex) 
			{
				DoOnException(ex);
			}
			finally 
			{
				db.disconnect();
			}
			return "No contact";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

        }
		#endregion
	}
}

namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for ContactFirmDetail.
    /// </summary>
    public partial class ContactFirmDetail : PSOFTMapperUserControl
	{

		private long _ID = -1;
        private long _contactGroupID = -1;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Contact/Controls/ContactFirmDetail.ascx";}
		}

		#region Properties
		public long FirmID
		{
			get {return _ID;}
			set {_ID = value;}
		}

        public long ContactGroupID
        {
            get {return _contactGroupID;}
            set {_contactGroupID = value;}
        }

        public string FirmName
		{
			get 
			{
				return GetFirmName();
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

            string sqlStatement = "SELECT * FROM FIRM WHERE ID=" + _ID;

			//get data from database
			DBData db = DBData.getDBData(Session);
			System.Data.DataTable data = null;
			System.Data.DataTable dataAddress = null;

			try 
			{
				db.connect();
				data = db.getDataTable(sqlStatement);

				//fill dynamic person table information 
				if (data.Rows.Count > 0)
				{
                    string email = DBData.getValue(data, 0, "EMAIL").ToString();
                    EMailLink emailLink = null;
                    if (email != ""){
                        emailLink = new EMailLink();
                        emailLink.NavigateUrl = emailLink.Text = email;
                    }
                    string website = DBData.getValue(data, 0, "WEBSITE").ToString();
                    HyperLink websiteLink = null;
                    if (website != ""){
                        websiteLink = new HyperLink();
                        websiteLink.Text = website;
						websiteLink.NavigateUrl = (website.StartsWith("http://")? "" : "http://") + website;
						websiteLink.Target = "_blank";
					}
                    AddressBuilder.AppendControls(addressTable, websiteLink, emailLink);

                    AddressBuilder.AppendDetail(addressTable, null, DBData.getValue(data, 0, "PURPOSE").ToString());

					//fill dynamic person address information
					sqlStatement = "SELECT * FROM ADDRESS WHERE FIRM_ID = " + _ID;
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

		private string GetFirmName()
		{
			//get data from database
			DBData db = DBData.getDBData(Session);

            try 
			{
				db.connect();
				return db.lookup("TITLE", "FIRM", "ID=" + _ID, false);
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

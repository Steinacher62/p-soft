namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.IO;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PersonDetailView.
    /// </summary>
    public partial class PersonDetailView : PSOFTMapperUserControl
	{
		private long _ID = -1;
        private bool _showName = true;

        protected System.Web.UI.WebControls.TableRow rowCostcenter;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Person/Controls/PersonDetailView.ascx";}
		}

		#region Properties
        public long PersonID {
            get {return _ID;}
            set {_ID = value;}
        }

        public bool ShowName {
            get {return _showName;}
            set {_showName = value;}
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


            // find out if the current person is leader of the viewed person or is administrator
            long groupAccessorId = DBColumn.GetValid(
                    db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                    (long)-1
                    );
            bool canAccessSensibleData = (db.userId == this._ID) || // a user can always access his/her own data
                                          db.Person.isLeaderOfPerson(db.userId, this._ID, true) ||
                                          (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)); 

			try 
			{
				db.connect();
				data = db.getDataTable(sqlStatement);

				//fill dynamic person table information 
				if (data.Rows.Count > 0)
				{
                    if (_showName)
                    {
                        HEADER.Text = db.Person.getWholeName(data.Rows[0]["ID"].ToString(), false, true, false);
                    }
                    else
                    {
                        HEADER.Visible = false;
                    }

                    TITLE_VALUE.Text = DBData.getValue(data, 0, "TITLE").ToString();
                    if (TITLE_VALUE.Text.Length > 0)
                        TITLE_VALUE.Text += " ";

                    bool showMainJobOnly = Global.Config.showMainJobOnly;
                    string mainJobId = "";

                    if (showMainJobOnly)
                    {
                        //is main job defined?
                        if (Convert.ToInt32(db.lookup("COUNT(JOB.ID)", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _ID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)")) == 0)
                        {
                            //no main job defined, show all jobs
                            showMainJobOnly = false;
                        }
                        else
                        {
                            //get main job
                            mainJobId = db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _ID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString();
                        }
                    }

                    DataTable orgInfo = db.Person.getMOJobs(_ID);
                    string orgUnit = "";
                    foreach (DataRow row in orgInfo.Rows)
                    {
                        // show main job only set and active job available? => display only main job
                        if (mainJobId != "" && row["ID"].ToString() != mainJobId)
                        {
                            break;
                        }

                        
                        if (Global.isModuleEnabled("spz"))
                        {
                            if (ORG_INFO.Text.Length > 0) ORG_INFO.Text += ", ";
                            if (canAccessSensibleData)
                            {
                                ORG_INFO.Text += db.GetDisplayValue(orgInfo.Columns[db.langAttrName("JOB", "TITLE")], row[db.langAttrName("JOB", "TITLE")], true);
                            }
                            else
                            {
                                ORG_INFO.Text += db.lookup("title_de", "employment", "id = " + row["EMPLOYMENT_ID"].ToString());
                            }
                            orgUnit = db.GetDisplayValue(orgInfo.Columns[db.langAttrName("ORGENTITY", "MNEMONIC")], row[db.langAttrName("ORGENTITY", "MNEMONIC")], true);
                        }
                        else
                        {
                            if (ORG_INFO.Text.Length > 0) ORG_INFO.Text += ", ";
                            ORG_INFO.Text += db.GetDisplayValue(orgInfo.Columns[db.langAttrName("JOB", "TITLE")], row[db.langAttrName("JOB", "TITLE")], true);
                            orgUnit = db.GetDisplayValue(orgInfo.Columns[db.langAttrName("ORGENTITY", "MNEMONIC")], row[db.langAttrName("ORGENTITY", "MNEMONIC")], true);
                        }
                        

                        if (orgUnit != null && orgUnit != "")
                        {
                            ORG_INFO.Text += " - " + orgUnit;
                        }
                        // percent of employment
                        string engagementlevel = db.GetDisplayValue(orgInfo.Columns["ENGAGEMENT"], row["ENGAGEMENT"], true);
                        ORG_INFO.Text += " (" + engagementlevel + "%)";
                    }
                    
                    //person-details...


                    if (Global.isModuleEnabled("ahb"))   // if ahb activate funktion
                    {
                        String IpsFunktion;
                        if (DBData.getValue(data, 0, "LEAVING").ToString() == "")
                        {
                            IpsFunktion = db.lookup("FUNKTION.TITLE_DE", "FUNKTION INNER JOIN JOB ON FUNKTION.ID = JOB.FUNKTION_ID", "JOB.ID = " + mainJobId).ToString();
                        }
                        else
                        {
                            IpsFunktion = "Austritt";
                        }
                        AddressBuilder.AppendDetail(personDetailTable,"IPS-Funktion", IpsFunktion);
                        
                    }
                    if (Global.isModuleEnabled("energiedienst") && canAccessSensibleData)
                    {
                        AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "PERSONNELNUMBER"), DBData.getValue(data, 0, "PERSONNELNUMBER").ToString());
                    }
                    else
                    {
                        if (!Global.isModuleEnabled("energiedienst"))
                        {
                            AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "PERSONNELNUMBER"), DBData.getValue(data, 0, "PERSONNELNUMBER").ToString());
                        }
                    }
                    string phone = DBData.getValue(data, 0, "PHONE").ToString();
                    string shortPhone = Person.getShortPhoneNumber(phone);
                    if (shortPhone.Length > 0)
                        phone += " (" + shortPhone + ")";
                    if (Global.isModuleEnabled("skandia"))
                    {
                        // Text-Mapping speziell für Skandia
                        AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "PHONEEXT"), phone);
                    }
                    else
                    {
                        AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "PHONE"), phone);
                        if ((Global.isModuleEnabled("ahb")) & (DBData.getValue(data, 0, "PHONE").ToString()==""))
                        {
                            AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "PHONE"), " ");
                        }
                    }
                    AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON","MOBILE"), DBData.getValue(data, 0, "MOBILE").ToString());
                    if (Global.isModuleEnabled("feller")){
                        AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON","COSTCENTER"), DBData.getValue(data, 0, "COSTCENTER").ToString());
                    }
                    string email = DBData.getValue(data, 0, "EMAIL").ToString();
                    if (email != ""){
                      //  EMailLink emailLink = new EMailLink();
                      //  emailLink.Text = emailLink.NavigateUrl = email;
                      //  AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "EMAIL"), DBData.getValue(data, 0, "EMAIL").ToString());
                        AddressBuilder.AppendEmail(personDetailTable, _mapper.get("PERSON", "EMAIL"), DBData.getValue(data, 0, "EMAIL").ToString());
                    }

                    //photo
                    string photo = DBData.getValue(data, 0, "PHOTO").ToString();
					if (photo.Equals(""))
						PHOTO.Visible = false;
					else 
					{
						FileInfo photoFile = new FileInfo(Global.Config.personPhotoDirectory + "//" + photo);

                        if (photoFile.Exists)
                        {
                            //PHOTO.ImageUrl = Global.Config.personPhotoDirectory + "\\" + photo;

                            PHOTO.ImageUrl =  "//"+Global.Config.domain +"/"+ Global.Config.baseURL +"/person/photo/" + photo;
                            PHOTO.AlternateText = db.Person.getWholeName(data.Rows[0]["ID"].ToString(), false, true, false);
                        }
                        else
                            PHOTO.Visible = false;
					}

                    //date of birth
                    if (canAccessSensibleData)
                    {
                        DateTime dateOfBirth = DBColumn.GetValid(data.Rows[0]["DATEOFBIRTH"], DateTime.MinValue);
                        if (dateOfBirth != DateTime.MinValue)
                        {
                            string date = dateOfBirth.ToString("dd.MM.yyyy");
                            AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "DATEOFBIRTH"), date);
                        }
                        DateTime entry;
                        if (Global.isModuleEnabled("ahb"))
                        {
                           entry = DBColumn.GetValid(data.Rows[0]["LAST_ENTRY"], DateTime.MinValue);
                        }
                        else
                        {
                           entry = DBColumn.GetValid(data.Rows[0]["ENTRY"], DateTime.MinValue);
                        }
                        if (entry != DateTime.MinValue)
                        {
                            string date = entry.ToString("dd.MM.yyyy");
                            if (Global.isModuleEnabled("ahb"))
                            {
                                AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "LAST_ENTRY"), date);
                            }
                            else
                            {
                                AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "ENTRY"), date);
                            }
                            
                        }

                        DateTime leaving = DBColumn.GetValid(data.Rows[0]["LEAVING"], DateTime.MinValue);
                        if (leaving != DateTime.MinValue)
                        {
                            string date = leaving.ToString("dd.MM.yyyy");
                            AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "LEAVING"), date);
                        }
                        else  // if AHB activate Fielname 'LEAVING'
                        {
                            if (Global.isModuleEnabled("ahb") & (leaving == DateTime.MinValue)) 
                            {
                                AddressBuilder.AppendDetail(personDetailTable, _mapper.get("PERSON", "LEAVING"), " ");
                            }
                        }
                        // TODO: Lohn anzeigen if required
                    }

                   



					//get company address information
					sqlStatement = "SELECT * FROM FIRM WHERE ID = " + ch.psoft.Util.Validate.GetValid(DBData.getValue(data, 0, "FIRM_ID").ToString(),"0");
					dataCompany = db.getDataTable(sqlStatement);

                    if (dataCompany.Rows.Count > 0) {
                        HyperLink companyLink = new HyperLink();
                        companyLink.Text = DBData.getValue(dataCompany, 0, "TITLE").ToString();
                        companyLink.CssClass = "detail_link";
                        if(!DBData.getValue(dataCompany, 0, "WEBSITE").ToString().Equals("")) {
                            companyLink.NavigateUrl = DBData.getValue(dataCompany, 0, "WEBSITE").ToString();
                            companyLink.NavigateUrl = (companyLink.NavigateUrl.StartsWith("http://")? "" : "http://") + companyLink.NavigateUrl;
                            companyLink.Target = "_blank";
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

                        if (Global.isModuleEnabled("skandia"))
                        {
                            // Skandia will Firmen-Mailadresse nicht angezeigt haben
                            companyEmail = "";
                        }

                        if (companyEmail != ""){
                            EMailLink companyEmailLink = new EMailLink();
                            companyEmailLink.Text = companyEmailLink.NavigateUrl = companyEmail;
                            AddressBuilder.AppendDetailControl(companyAddressTable, null, companyEmailLink);
                        }
                    }

					//fill dynamic person address information
                    if (Global.isModuleEnabled("ahb"))
                    {
                        sqlStatement = "SELECT * FROM ADDRESS WHERE PERSON_ID = " + _ID;
                        dataAddress = db.getDataTable(sqlStatement);
                        for (int i = 0; i < dataAddress.Rows.Count; i++)
                        {
                            AddressBuilder.BuildAddressTable(addressTable, dataAddress, i, _mapper);
                        }
                    }


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
			return "No person";
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

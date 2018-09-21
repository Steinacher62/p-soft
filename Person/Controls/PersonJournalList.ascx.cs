namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial class PersonJournalList : PSOFTListViewUserControl {
		
        private DBData _db = null;
		
        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path {
            get {return Global.Config.baseURL + "/Person/Controls/PersonJournalList.ascx";}
        }

        public PersonJournalList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            //EditEnabled = false;
            EditEnabled = true;
            //EditURL = "PersonJournalEdit.aspx?journalID=%ID&backURL=" + HttpUtility.UrlEncode(Request.RawUrl);
            EditURL = "PersonJournalEdit.aspx?journalID=%ID&contextID=%contextID%&backURL=";
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
			
        }

		#region Properties
		public long _contextID {
			get {return GetLong("contextID");}
			set {SetParam("contextID", value);}
		}
		
		public long _journalID {
			get {return GetLong("journalID");}
			set {SetParam("journalID", value);}
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            _db = DBData.getDBData(Session);
      
			try {
                _db.connect();

                EditURL = EditURL.Replace("%contextID%", _contextID.ToString());

				//select isnull(firstname + ' ','') + isnull(pname,'') from person where id=?
                pageTitle.Text = _mapper.get("journal", "personJournalList").Replace("#1", _db.lookup("isnull(firstname + ' ','') + isnull(pname,'')", "PERSON", "ID=" + _contextID, false).ToString());

                string sql = "";
                if (Global.isModuleEnabled("spz"))
                {
                    Int32 maMainJobId = Convert.ToInt32(_db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _contextID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                    long maJobOrgId = Convert.ToInt32(_db.lookup("ORGENTITY_ID", "JOB", "ID =" + maMainJobId.ToString()));
                    Int32 userJobId = Convert.ToInt32(_db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _db.userId + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                    long userJobOrgId = Convert.ToInt32(_db.lookup("ORGENTITY_ID", "JOB", "ID =" + userJobId.ToString()));

                    // If orgentity "Pflege" User can read all journals
                    string idsPflege = _db.Orgentity.addAllSubOEIDs("92007"); //OeId Pflege
                    if (idsPflege.IndexOf(userJobOrgId.ToString()) > 0 & (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", maMainJobId, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)))
                    {
                        sql = "select * from PERSON_JOURNAL where PERSON_ID=" + _contextID;
                    }
                    else
                    {
                        sql = "select * from PERSON_JOURNAL where PERSON_ID=" + _contextID + " AND CREATOR_PERSON_ID=" + _db.userId;
                    }
                }
                else
                {

                    if (Global.Config.showJournalOnlyCreator)
                    {
                        sql = "select * from PERSON_JOURNAL where PERSON_ID=" + _contextID + " AND CREATOR_PERSON_ID=" + _db.userId;
                    }
                    else
                    {
                        sql = "select * from PERSON_JOURNAL where PERSON_ID=" + _contextID;
                    }
                }

                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, "PERSON_JOURNAL");
				DataTable journalTypes = _db.getDataTable("select ID," + _db.langAttrName("PERSON_JOURNAL_TYPE", "TITLE") + " from PERSON_JOURNAL_TYPE order by " + _db.langAttrName("PERSON_JOURNAL_TYPE", "TITLE"));
                table.Columns["PERSON_JOURNAL_TYPE_ID"].ExtendedProperties["In"] = journalTypes;
                table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["In"] =  _db.Person.getWholeNameMATable(true);
                table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%CREATOR_PERSON_ID", "mode","oe");
				table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
				table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                
				IDColumn = "ID";

				if (_journalID > 0) {
					HighlightRecordID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "PERSON_JOURNAL", "ID=" + _journalID + " and PERSON_ID=" + _contextID, false), -1);
				}

                LoadList(_db, table, listTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
			if (ListBuilder.IsInfoCell(c)){
				long journalTypeID = DBColumn.GetValid(row["PERSON_JOURNAL_TYPE_ID"], -1L);

				if (journalTypeID > 0L){
					string icon = _db.lookup("ICON", "PERSON_JOURNAL_TYPE", "ID=" + journalTypeID, false);
				    if (icon != "" || icon != null) {
						string imgURL = ((System.Web.UI.WebControls.Image) c.Controls[0]).ImageUrl;
                        imgURL = imgURL.Replace("images/icon_info.png","images/journaltypes/" + icon);
						((System.Web.UI.WebControls.Image) c.Controls[0]).ImageUrl = imgURL;
                    }
                }
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}

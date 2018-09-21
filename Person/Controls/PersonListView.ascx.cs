namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    ///		Summary description for PersonListView.
    /// </summary>
    public partial class PersonListView : PSOFTListViewUserControl
    {
        private long _personID = -1;
        private long _xID = -1;
        private string _mode = "";
        private string _sql;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path
        {
            get { return Global.Config.baseURL + "/Person/Controls/PersonListView.ascx"; }
        }

        public PersonListView()
            : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

        #region Properities
        public long PersonID
        {
            get { return _personID; }
            set { _personID = value; }
        }

        public long xID
        {
            get { return _xID; }
            set { _xID = value; }
        }

        public string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public string sql
        {
            get { return _sql; }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                View = "PERSONOEV";
                string columns = "";
                // find out if the current person is leader of the viewed person or is administrator or is HR
                long groupAccessorId = DBColumn.GetValid(
                    db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                    (long)-1
                    );
                bool canAccessSensibleData = (db.userId == _personID) || // a user can always access his/her own data
                                              db.Person.isLeaderOfPerson(db.userId, this._personID, true) ||
                                              (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)
                                              );
                {
                    // spz will kein telefon aber pfs, Eintritt und Geburtsdatum
                    if (Global.isModuleEnabled("spz") && canAccessSensibleData)
                    {
                        columns = "distinct ID, PNAME, FIRSTNAME, PERSONNELNUMBER, PFS, ENTRY, DATEOFBIRTH, JOB_TYP,  " + db.langAttrName("PERSONOEV", "FUNKTION_TITLE") + ", " + db.langAttrName("PERSONOEV", "OE_TITLE") + ",WAGES, JOB_ID, Kaderstufe";
                    }
                    else
                    {
                        columns = "distinct ID, PNAME, FIRSTNAME, PERSONNELNUMBER, JOB_TYP, " + db.langAttrName("PERSONOEV", "OE_TITLE") + ", JOB_ID";
                    }

                    if (Global.isModuleEnabled("spz") == false)
                    {
                        if (Global.Config.showVageField)
                        {
                            columns = "distinct ID, PNAME, FIRSTNAME, PERSONNELNUMBER, JOB_TYP, PHONE, " + db.langAttrName("PERSONOEV", "OE_TITLE") + ",WAGES, JOB_ID";
                        }
                        else
                        {
                            columns = "distinct ID, PNAME, FIRSTNAME, PERSONNELNUMBER, JOB_TYP, PHONE, " + db.langAttrName("PERSONOEV", "OE_TITLE") + ", JOB_ID";
                        }
                    }

                }

                switch (_mode.ToLower())
                {
                    case "oe":
                    case "oeleader":
                        pageTitle.Text = _mapper.get("person", "personListOE");
                        // Build person list sql (person in the same orgentities)
                        _sql = "select " + columns + " from PERSONOEV";
                        if (_xID > 0)
                        {
                            _sql += " where OE_ID = " + _xID;
                            pageTitle.Text += " " + db.lookup("TITLE", "ORGENTITY", "ID=" + _xID, false);
                        }
                        else if (_personID > 0)
                        {
                            _sql += " where OE_ID in (select OE_ID from PERSONOEV where ID=" + _personID + "and MAINORGANISATION=1)";
                            View = "PERSONOEV_WITH_OE_TITLE";
                        }
                        break;

                    case "searchresult":
                        pageTitle.Text = _mapper.get("person", "personListSearchResult");
                        _sql = "select " + columns + " from PERSONOEV WHERE ID IN (select ROW_ID from SEARCHRESULT where TABLENAME = 'PERSON' AND ID = "
                            + _xID + ") and (MAINORGANISATION=1 or MAINORGANISATION is null)";
                        View = "PERSONOEV_WITH_OE_TITLE";
                        break;

                    default:
                        if (_personID > 0)
                            _sql = "select " + columns + " from PERSONOEV where ID=" + _personID + " and (MAINORGANISATION=1 or MAINORGANISATION is null)";
                        else
                            _sql = Session["PersonSQLSearch"] as string;
                        View = "PERSONOEV_WITH_OE_TITLE";
                        break;
                }

                Session["PersonSQLSearch"] = _sql;

                _sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = db.getDataTableExt(_sql, "PERSONOEV");

                // delete wages (if no rights)
                if (table.Columns.Contains("WAGES"))
                {
                    table.Columns["Wages"].ReadOnly = false;
                }
                if (Global.isModuleEnabled("spz"))
                {
                    table.Columns["JOB_TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                   
                    if (canAccessSensibleData)
                    {
                        bool isPersonAdmin = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", _personID, true, true);

                        foreach (DataRow row in table.Rows)
                        {
                            bool isLohnRead = db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", Convert.ToInt32(row["JOB_ID"]), DBData.APPLICATION_RIGHT.MODULE_LOHN, true, true);
                            if (isLohnRead == false && Global.Config.showVageField)
                            {
                                row["WAGES"] = DBNull.Value;
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in table.Rows)
                    {
                        bool isLohnRead = db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", Convert.ToInt32(row["JOB_ID"]), DBData.APPLICATION_RIGHT.MODULE_LOHN, true, true);
                        if (isLohnRead == false && Global.Config.showVageField)
                        {
                            row["WAGES"] = DBNull.Value;

                        }
                    }
                }

                IDColumn = "ID";
                if (_personID > 0)
                    HighlightRecordID = _personID;
                ArrayList jobTypes = new ArrayList(_mapper.getEnum("jobType", true));
                table.Columns["JOB_TYP"].ExtendedProperties["In"] = jobTypes;
                CheckOrder = true;
                LoadList(db, table, listTab);
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

using ch.appl.psoft.Common;
using ch.appl.psoft.Contact;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.appl.psoft.Person.Controls;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Person
{
    /// <summary>
    /// Summary description for DetailFrame1.
    /// </summary>
    public partial class DetailFrame : PsoftDetailPage
    {
        private const string PAGE_URL = "/Person/DetailFrame.aspx";

        static DetailFrame()
        {
            SetPageParams(PAGE_URL, "ID", "UID", "xID", "xUID", "index", "mode", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public DetailFrame()
            : base()
        {
            PageURL = PAGE_URL;
        }

        // User controls variables
        private PsoftLinksControl _links = null;

        // Query string variables
        private long _personID = -1;
        private long _personUID = -1;
        private long _xID = -1;
        private long _xUID = -1;
        private int _index = 0;
        private string _mode = "";
        private string _orderColumn = "PNAME";
        private string _orderDir = "asc";

        #region Protected overrided methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _personID = GetQueryValue("ID", -1);
            _personUID = GetQueryValue("UID", -1);
            _xID = GetQueryValue("xID", -1);
            _xUID = GetQueryValue("xUID", -1);
            _index = GetQueryValue("index", _index);
            _mode = GetQueryValue("mode", "");
            if (_mode.ToLower() == "oeleader")
            {
                _orderColumn = "JOB_TYP";
                _orderDir = "desc";
            }
            _orderColumn = GetQueryValue("orderColumn", _orderColumn);
            _orderDir = GetQueryValue("orderDir", _orderDir);
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Setting default breadcrumb caption
            this.BreadcrumbCaption = "Person details";

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            ((DGLContentLayout)PageLayoutControl.ContentLayoutControl).DetailHeight = Unit.Pixel(230);

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();

                if (_personUID > 0)
                    _personID = db.UID2ID(_personUID);

                if (_xUID > 0)
                    _xID = db.UID2ID(_xUID);

                if (_personID > 0 && _mode.ToLower() == "searchresult")
                {
                    //check if ID is valid...
                    _personID = ch.psoft.Util.Validate.GetValid(db.lookup("ROW_ID", "SEARCHRESULT", "ID=" + _xID + " and ROW_ID=" + _personID, false), -1L);
                }

                if (_personID <= 0)
                {
                    switch (_mode.ToLower())
                    {
                        case "oe":
                            _personID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "PERSONOEV", "OE_ID=" + _xID + " order by PNAME asc", false), -1);
                            break;
                        case "oeleader":
                            DataTable temp = db.getDataTable("select ID from PERSONOEV where OE_ID=" + _xID + " and JOB_TYP=1 order by PNAME, FIRSTNAME asc");
                            if (temp.Rows.Count > _index)
                            {
                                _personID = DBColumn.GetValid(temp.Rows[_index][0], -1L);
                            }
                            break;
                        case "searchresult":
                            _personID = ch.psoft.Util.Validate.GetValid(db.lookup("row_id", "searchresult", "ID=" + _xID, false), -1);
                            break;
                    }
                }

                // Setting links control for given person 
                _links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                _links.LinkGroup1.Caption = _mapper.get("actions");
                _links.LinkGroup2.Caption = _mapper.get("importantLinks");


                // do not show owen links spz
                bool showOwenLinks = true;
                if ((db.userId == _personID) && Global.isModuleEnabled("spz") && !(db.userId == 901 ))
                {
                    showOwenLinks = false;
                }


                if (_personID > 0)
                {

                    int typ = DBColumn.GetValid(db.lookup("typ", "person", "id=" + _personID), 0);
                    if ((typ & ch.appl.psoft.Interface.DBObjects.Person.TYP.CONTACT) > 0) Response.Redirect(Global.Config.baseURL + "/Contact/ContactDetail.aspx?ID=" + _personID + "&type=" + ContactModule.TYPE_PERSON);
                    // Loading and setting properities of content user controls
                    PersonDetailView pd = (PersonDetailView)this.LoadPSOFTControl(PersonDetailView.Path, "_pd");
                    pd.PersonID = _personID;
                    pd.ShowName = false;

                    // Setting breadcrumb caption
                    this.BreadcrumbCaption = pd.PersonName;

                    // Setting page-title
                    ((PsoftPageLayout)PageLayoutControl).PageTitle = pd.PersonName;

                    if (Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.LOGIN && db.userId == _personID && db.Person.canChangePassword(_personID))
                    {
                        _links.LinkGroup1.AddLink(_mapper.get("administration"), _mapper.get("administration", "editPassword"), psoft.Person.EditPassword.GetURL("ID", _personID, "backURL", Request.RawUrl));
                    }

                    if (showOwenLinks)
                    {
                        long clipboardID = db.Person.getClipboardID(_personID);
                        //Clipboard wird bei Energiedienst später hinzugefügt
                        if (!Global.isModuleEnabled("energiedienst") && clipboardID > 0 && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardID, true, true))
                        {
                            _links.LinkGroup2.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ownerTable", "PERSON", "ID", clipboardID));
                        }

                        if (db.userId != _personID)
                        {

                            //Int32 mainJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                            String mainJobId = db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)", 0L).ToString();
                            Int32 mainJobId1 = Convert.ToInt32(mainJobId);
                            if (!Global.isModuleEnabled("energiedienst"))
                            {
                            if ((db.Person.isLeaderOfPerson(db.userId, _personID, true) && db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", mainJobId1, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)) || (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", mainJobId1, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)) && (String.Compare(db.lookup("LEAVING", "PERSON", "ID=" + _personID, "31.12.2099 00:00:00"), String.Format("{0:dd/MM/yyyy}", DateTime.Now)) == 1))
                            {
                                _links.LinkGroup2.AddLink(
                                    _mapper.get("journal", "journal"),
                                    _mapper.get("journal", "journalAdd"),
                                    "/Person/PersonJournalAdd.aspx?view=detail&context=PERSON&contextId=" + _personID
                                    );
                            }


                            string countstr = "";
                            if (Global.isModuleEnabled("spz") && !db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", db.userId, true, true))
                            {
                                Int32 maMainJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                                long maJobOrgId = Convert.ToInt32(db.lookup("ORGENTITY_ID", "JOB", "ID =" + maMainJobId.ToString()));
                                Int32 userJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + db.userId + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                                long userJobOrgId = Convert.ToInt32(db.lookup("ORGENTITY_ID", "JOB", "ID =" + userJobId.ToString()));

                                // If orgentity "Pflege" User can read all journals
                                string idsPflege = db.Orgentity.addAllSubOEIDs("92007"); //OeId Pflege
                                if (idsPflege.IndexOf(userJobOrgId.ToString()) > 0 & (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", maMainJobId, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)))
                                {
                                    countstr = db.lookup("COUNT(ID)", "PERSON_JOURNAL", "PERSON_ID=" + _personID, false);
                                }
                                else
                                {
                                    countstr = db.lookup("COUNT(ID)", "PERSON_JOURNAL", "PERSON_ID=" + _personID + "AND CREATOR_PERSON_ID=" + db.userId, false);
                                }
                            }
                            else
                            {
                                
                                if(db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", mainJobId1, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))
                                {
                                if (Global.Config.showJournalOnlyCreator && (db.Person.isLeaderOfPerson(db.userId, _personID, true)))
                                {
                                    countstr = db.lookup("COUNT(ID)", "PERSON_JOURNAL", "PERSON_ID=" + _personID + "AND CREATOR_PERSON_ID=" + db.userId, false);
                                }
                                else
                                {
                                    countstr = db.lookup("COUNT(ID)", "PERSON_JOURNAL", "PERSON_ID=" + _personID , false);
                                }
                                }
                            }
                            int count=0;
                                if (!countstr.Equals(""))
                                {
                                    count = Convert.ToInt32(countstr);
                                }
                                if (count > 0)
                                {
                                    _links.LinkGroup2.AddLink(
                                        _mapper.get("journal", "journal"),
                                        _mapper.get("journal", "journalView"),
                                        "/Person/PersonJournalDetail.aspx?&contextID=" + _personID
                                        );
                                }
                            }
                        }




                        if (Global.isModuleEnabled("mbo") && (String.Compare(db.lookup("LEAVING", "PERSON", "ID=" + _personID, "31.12.2099 00:00:00"), String.Format("{0:dd/MM/yyyy}", DateTime.Now)) == 1))
                        {
                            int type = ch.psoft.Util.Validate.GetValid(db.lookup("TYP", "PERSON", "ID=" + db.userId, false), 0);

                            String mainJobId = db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)", 0L).ToString();
                            Int32 mainJobId1 = Convert.ToInt32(mainJobId);
                            if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", mainJobId1, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
                            {
                                int emp = db.Orgentity.getEmployment(_personID);
                                // person objectives (Ziele)
                                if (!Global.isModuleEnabled("spz"))
                                {
                                    if (!Global.isModuleEnabled("energiedienst")) // Bei Energiedienst wird MbO Link später hinzugefügt (andere Sortierung
                                    {
                                        _links.LinkGroup2.AddLink(_mapper.get("mbo", "objectives"), _mapper.get("mbo", "objectives"), "/MbO/Detail.aspx?view=detail&context=PERSON&contextId=" + _personID);
                                    }

                                    }
                                else
                                {
                                    _links.LinkGroup2.AddLink(_mapper.get("mbo", "objectives"), _mapper.get("mbo", "objectives"), "/SPZ/Detail.aspx?view=detail&context=PERSON&contextId=" + _personID);
                                }
                                     if (!db.Objective.isPersonFilterOnly && emp == 1)
                                {
                                    if (!Global.isModuleEnabled("spz"))
                                    {
                                        _links.LinkGroup2.AddLink(_mapper.get("mbo", "objectives"), _mapper.get("mbo", "supervisorObjectives"), "/MbO/Detail.aspx?view=detail&context=SUPERVISOR&contextId=" + _personID);
                                    }
                                    else
                                    {
                                        _links.LinkGroup2.AddLink(_mapper.get("mbo", "objectives"), _mapper.get("mbo", "supervisorObjectives"), "/SPZ/Detail.aspx?view=detail&context=SUPERVISOR&contextId=" + _personID);
                                    }
                                    }
                            }
                        }

                        if (Global.isModuleEnabled("project"))
                        {
                            // person projects (Projects)
                            _links.LinkGroup2.AddLink(
                                _mapper.get("navigation", "listProjects"),
                                _mapper.get("navigation", "listProjects"),
                                psoft.Project.ProjectDetail.GetURL("context", "person", "xID", _personID)
                                );
                        }

                        if (Global.isModuleEnabled("tasklist"))
                        {
                            // person pendenzen (Pendenzen)
                            _links.LinkGroup2.AddLink(
                                _mapper.get("navigation", "listMeasures"),
                                _mapper.get("navigation", "listMeasures"),
                                psoft.Tasklist.MeasureDetail.GetURL("context", "responsible", "xID", _personID)
                            );
                        }
                        if ((String.Compare(db.lookup("LEAVING", "PERSON", "ID=" + _personID, "31.12.2099 00:00:00"), String.Format("{0:dd/MM/yyyy}", DateTime.Now)) == -1))
                        {
                            long groupAccessorId = DBColumn.GetValid(
                            db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                            (long)-1);
                            if (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true))
                            {
                                // show journals
                                _links.LinkGroup2.AddLink(
                                _mapper.get("journal", "journal"),
                                _mapper.get("journal", "journalAdd"),
                                "/Person/PersonJournalAdd.aspx?view=detail&context=PERSON&contextId=" + _personID);
                                // show performancerating
                                if (Global.isModuleEnabled("performance"))
                                {
                                    long employmentID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "EMPLOYMENT", "PERSON_ID=" + _personID.ToString(), false), -1);
                                    _links.LinkGroup2.AddLink("Ausgetreten", _mapper.get("performance", "performanceRatingLeader"), "/Performance/EmploymentRating.aspx?mode=history&type=leader&employmentID=" + employmentID + "&jobID=" + "NULL" + "&orderDir=desc");
                                }
                                // show training
                                if (Global.isModuleEnabled("training"))
                                {
                                    if (!Global.isModuleEnabled("energiedienst"))
                                    {
                                        _links.LinkGroup2.AddLink(_mapper.get("training", "cmtTraining"), _mapper.get("training", "cmAdvancment"), "/Training/Advancement.aspx?personID=" + _personID);
                                    }
                                    else
                                    {
                                        _links.LinkGroup2.AddLink(_mapper.get("training", "cmtTraining"), _mapper.get("training", "cmAdvancment"), "/Energiedienst/Advancement.aspx?personID=" + _personID);
                                    }
                                }
                            }
                        }


                        bool showActualSkills = false;
                        bool showSkillsAppraisals = false;
                        bool showNewSkillsAppraisal = false;
                        bool showTraining = false;

                        bool showMainJobOnly = Global.Config.showMainJobOnly;
                        string sql = "select ORGENTITY." + db.langAttrName("ORGENTITY", "MNEMONIC") + ", JOB.ORGENTITY_ID, JOB.FUNKTION_ID, ORGENTITY.CLIPBOARD_ID, JOB." + db.langAttrName("JOB", "TITLE") + ", JOB.ID, JOB.EMPLOYMENT_ID from ORGANISATION inner join (ORGENTITY inner join (JOB inner join EMPLOYMENT on JOB.EMPLOYMENT_ID=EMPLOYMENT.ID and EMPLOYMENT.PERSON_ID=" + _personID + ") on JOB.ORGENTITY_ID=ORGENTITY.ID) on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID where ORGANISATION.MAINORGANISATION=1";

                        if (showMainJobOnly)
                        {
                            //is main job defined?
                            if (Convert.ToInt32(db.lookup("COUNT(JOB.ID)", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)")) == 0)
                            {
                                //no main job defined, show all jobs
                                showMainJobOnly = false;
                            }
                            else
                            {
                                //get main job
                                string mainJobId = db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString();
                                sql += " AND JOB.ID = " + mainJobId;
                            }
                        }

                        int currentJobCount = 1;

                        DataTable jobTable = db.getDataTable(sql);
                        foreach (DataRow row in jobTable.Rows)
                        {
                            string orgentity = db.GetDisplayValue(jobTable.Columns[db.langAttrName("ORGENTITY", "MNEMONIC")], row[db.langAttrName("ORGENTITY", "MNEMONIC")], true);
                            string linkGroupName = db.GetDisplayValue(jobTable.Columns[db.langAttrName("JOB", "TITLE")], row[db.langAttrName("JOB", "TITLE")], true) + " (" + orgentity + ")";
                            if (jobTable.Rows.Count > 1)
                                linkGroupName = "" + currentJobCount + ". " + linkGroupName;

                            long jobID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L);
                            long orgentityID = ch.psoft.Util.Validate.GetValid(row["ORGENTITY_ID"].ToString(), -1L);
                            clipboardID = ch.psoft.Util.Validate.GetValid(row["CLIPBOARD_ID"].ToString(), -1L);

                            if (clipboardID > 0 && (db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "ORGENTITY", orgentityID, true, true) || db.userId == _personID || db.Person.isLeaderOfJob(jobID, true)))
                            {
                                //clipboard of orgentity
                                _links.LinkGroup2.AddLink(_mapper.get("shelf"), orgentity, psoft.Document.Clipboard.GetURL("ownerTable", "ORGENTITY", "ID", clipboardID));
                            }

                            // function-rating...
                            if (Global.isModuleEnabled("fbw"))
                            {
                                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.FBW, true, true))
                                {
                                    long functionID = ch.psoft.Util.Validate.GetValid(row["FUNKTION_ID"].ToString(), -1L);
                                    _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("fbw", "cmtFunctionRating"), "/FBW/FunctionRating.aspx?functionID=" + functionID);
                                }
                            }

                            if (Global.isModuleEnabled("fbs"))
                            {
                                // job-description
                                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true))
                                {
                                    if(!Global.isModuleEnabled("energiedienst"))
                                    {
                                         _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("fbs", "cmtJobDescription"), "/FBS/JobDescription.aspx?jobID=" + jobID);
                                    }
                                    else
                                    {
                                        bool test = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "JOB", jobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true);
                                        if (db.userId == _personID && !db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", jobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true))
                                        {
                                            _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("fbs", "cmtJobDescription"), "/Energiedienst/PrintJobDescription.aspx?jobID=" + jobID);
                                        }
                                        else
                                        {
                                            _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("fbs", "cmtJobDescription"), "/Energiedienst/JobDescription.aspx?jobID=" + jobID);
                                        }
                                    }
                                }
                            }

                            if (Global.isModuleEnabled("skills"))
                            {
                                // job-skills
                                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.SKILLS, true, true))
                                {
                                    _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("skills", "cmtJobSkills"), "/Skills/XSkills.aspx?jobID=" + jobID);
                                    showActualSkills = true;
                                    showSkillsAppraisals = true;
                                }

                                if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", jobID, DBData.APPLICATION_RIGHT.SKILLS, true, true))
                                {
                                    showNewSkillsAppraisal = true;
                                }
                            }

                            if (Global.isModuleEnabled("performance"))
                            {
                                long employmentID = ch.psoft.Util.Validate.GetValid(db.lookup("EMPLOYMENT_ID", "JOB", "ID=" + jobID.ToString(), false), -1);

                                // job-expectation
                                if (!Global.isModuleEnabled("foampartner") && !Global.isModuleEnabled("laufenburg"))
                                {
                                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.JOB_EXPECTATION, true, true))
                                    {
                                        if (!Global.isModuleEnabled("ahb") && !Global.isModuleEnabled("energiedienst"))
                                        {
                                            _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("performance", "jobExpectation"), "/Performance/JobExpectation.aspx?ID=" + jobID);
                                        }
                                    }
                                }

                                //


                                // job-performance rating
                                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))
                                {
                                    // rating
                                    bool isPersonAdmin = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", _personID, true, true);

                                    // HACK: respect rights, don't check for isLeaderOfPersion
                                    if (true || db.Person.isLeaderOfPerson(_personID, true) || isPersonAdmin)
                                    {
                                        if (!Global.isModuleEnabled("energiedienst"))
                                        {
                                            _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("performance", "performanceRatingLeader"), "/Performance/EmploymentRating.aspx?mode=history&type=leader&employmentID=" + employmentID + "&jobID=" + jobID + "&orderDir=desc");
                                        }
                                        else
                                        {
                                            _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("performance", "performanceRatingLeader"), "/Energiedienst/EmploymentRating.aspx?mode=history&type=leader&employmentID=" + employmentID + "&jobID=" + jobID + "&orderDir=desc");
                                        }
                                    }
                                    // self-rating
                                    if ((db.userId == _personID || isPersonAdmin) && (Global.Config.getModuleParam("performance", "showPerformanceRatingSelf", "yes").Equals("yes")))
                                    {
                                        _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("performance", "performanceRatingSelf"), "/Performance/EmploymentRating.aspx?mode=history&type=self&employmentID=" + employmentID + "&jobID=" + jobID + "&orderDir=desc");
                                    }

                                    // job-performance rating global report
                                    if (Global.Config.getModuleParam("performance", "showGlobalPerformanceReport", "0").Equals("1"))
                                    {
                                        _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("performance", "performanceRatingGlobalReport"), "/Performance/SelectDate.aspx?employmentID=" + employmentID);
                                    }
                                }

                                if (Global.isModuleEnabled("energiedienst") && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)", 0L)), DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
                                {
                                    _links.LinkGroup2.AddLink(_mapper.get("mbo", "objectives"), "Zielvereinbarung / Zielerreichung", "/Energiedienst/Detail.aspx?view=detail&context=PERSON&contextId=" + _personID);
                                }
                            }

                            // show "Stammdaten" if enabled in config
                            // display only if it's the user's own page, the user is leader of the person or the user is admin
                            if (Global.Config.showReportPersonData)
                            {
                                bool isPersonAdmin = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", _personID, true, true);
                                if (db.userId == _personID || db.Person.isLeaderOfPerson(_personID, true) || isPersonAdmin || db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.MODULE_LOHN, true, true))
                                {
                                    // encode person ID
                                    byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(_personID.ToString());
                                    string base64Value = System.Convert.ToBase64String(toEncodeAsBytes);

                                    _links.LinkGroup2.AddLink(linkGroupName, _mapper.get("showReportPersonData"), "/Report/CrystalReportViewer.aspx?alias=Personalstammdaten&param0=" + base64Value);
                                }
                            }

                            //// history
                            //if ((String.Compare(db.lookup("LEAVING", "PERSON", "ID=" + _personID, "31.12.2099 00:00:00"), String.Format("{0:dd/MM/yyyy}", DateTime.Now)) == -1))
                            //{

                            //    bool isPersonAdmin = db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, "PERSON", _personID, true, true);
                            //    bool isJobRead = db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true);
                            //    bool somethingSet = false;

                            //    //check authorisation for displaying history for jobs
                            //    if (isJobRead || isPersonAdmin)
                            //    {
                            //        if (db.Person.isLeaderOfPerson(_personID, true) || isPersonAdmin || _personID == db.userId )
                            //        {
                            //            somethingSet = true;
                            //            // HACK: don't show Job History
                            //            //_links.LinkGroup1.AddLink(_mapper.get("person", "historyTitle"), _mapper.get("person", "jobHistory"), "/Person/DetailFrame.aspx?mode=jobDevHistory" + "&xID=" + this._xID + "&ID=" + this._personID);
                            //            if (Global.isModuleEnabled("performance"))
                            //            {
                            //                // HACK: don't show Performancerating History
                            //                //_links.LinkGroup1.AddLink(_mapper.get("person", "historyTitle"), _mapper.get("performance", "performanceRatingHistory"), "/Person/DetailFrame.aspx?mode=performanceLeaderHistory&ID=" + this._personID);

                            //            }
                            //            if (Global.isModuleEnabled("lohn"))
                            //            {
                            //                //TODO!!!
                            //                //_links.LinkGroup3.AddLink(_mapper.get("person", "historyTitle"), _mapper.get("lohn", "salaryHistory"), "/Person/DetailFrame.aspx?mode=performanceLeaderHistory&personID=" + this._personID);
                            //            }
                            //        }
                            //    }
                            //    if (Global.isModuleEnabled("performance") && _personID == db.userId)
                            //    {
                            //        somethingSet = true;
                            //        // HACK: don't show Selfrating History
                            //        //_links.LinkGroup1.AddLink(_mapper.get("person", "historyTitle"), _mapper.get("performance", "performanceSelfRatingHistory"), "/Person/DetailFrame.aspx?mode=performanceSelfHistory&ID=" + this._personID);
                            //    }
                            //    if (somethingSet)
                            //    {
                            //        // HACK: don't show department employees
                            //        //_links.LinkGroup1.AddLink(_mapper.get("person", "navigationTitle"), _mapper.get("performance", "departmentEmployeesList"), Psoft.Person.DetailFrame.GetURL("ID", this._personID, "mode", "oe"));
                            //    }

                            //}


                            if (Global.isModuleEnabled("training") && (String.Compare(db.lookup("LEAVING", "PERSON", "ID=" + _personID, "31.12.2099 00:00:00"), String.Format("{0:dd/MM/yyyy}", DateTime.Now)) == 1))
                            {
                                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jobID, DBData.APPLICATION_RIGHT.TRAINING, true, true))
                                {
                                    showTraining = true;
                                }
                            }

                            
                            currentJobCount++;

                            //if (true)
                            //{
                            //    break;
                            //}
                        }

                        if (showActualSkills)
                        {
                            // actual-skills (person)
                            _links.LinkGroup2.AddLink(_mapper.get("skills", "cmtActualSkills"), _mapper.get("skills", "cmtActualSkills"), "/Skills/XSkills.aspx?personID=" + _personID);
                        }

                        if (showSkillsAppraisals)
                        {
                            // skills-appraisals
                            if (ch.psoft.Util.Validate.GetValid(db.lookup("count (*)", "SKILLS_APPRAISAL", "PERSON_ID=" + _personID, false), -1) > 0)
                            {
                                _links.LinkGroup2.AddLink(_mapper.get("skills", "cmtSkillsAppraisal"), _mapper.get("skills", "cmtSkillsAppraisals"), "/Skills/AppraisalDetail.aspx?personID=" + _personID);
                            }
                        }

                        if (showNewSkillsAppraisal)
                        {
                            _links.LinkGroup2.AddLink(_mapper.get("skills", "cmtSkillsAppraisal"), _mapper.get("skills", "cmtNewSkillsAppraisal"), "/Skills/AppraisalAdd.aspx?personID=" + _personID);
                        }

                        if (showTraining)
                        {
                             //training
                            if (!Global.isModuleEnabled("energiedienst"))
                            {
                                _links.LinkGroup2.AddLink(_mapper.get("training", "cmtTraining"), _mapper.get("training", "cmAdvancment"), "/Training/Advancement.aspx?personID=" + _personID);
                            }
                            else
                            {
                                _links.LinkGroup2.AddLink(_mapper.get("training", "cmtTraining"), _mapper.get("training", "cmAdvancment"), "/Energiedienst/Advancement.aspx?personID=" + _personID);
                            }
                        }

                        //Clipboard Energiedienst hinzufügen
                        long clipboardIDEd = db.Person.getClipboardID(_personID);
                        if (Global.isModuleEnabled("energiedienst") && clipboardIDEd > 0 && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardIDEd, true, true) )
                        {
                            _links.LinkGroup2.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ownerTable", "PERSON", "ID", clipboardIDEd));
                        }
                        //Bei Energiedienst und ausgetretenen Ma für Gruppe HR Ziele anzeigen
                        long groupAccessorId1 = DBColumn.GetValid(
                           db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                           (long)-1);
                        String mainJobId2 = db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _personID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)", 0L).ToString();
                        if (Global.isModuleEnabled("energiedienst") && db.isAccessorGroupMember(db.userAccessorID, groupAccessorId1, true) && mainJobId2 == "0")
                        {
                            _links.LinkGroup2.AddLink(_mapper.get("mbo", "objectives"), "Zielvereinbarung / Zielerreichung", "/Energiedienst/Detail.aspx?view=detail&context=PERSON&contextId=" + _personID);
                        }
                    }
                    //Setting content layout user controls
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, pd);

                    // HACK: don't show Linkgroup 1
                    //_links.LinkGroup1.Visible = false;

                    SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally
            {
                db.disconnect();
            }
            if (_mode == "jobDevHistory")
            {
                PersonJobDevelopmentList pList = (PersonJobDevelopmentList)this.LoadPSOFTControl(PersonJobDevelopmentList.Path, "_plst");
                pList.PersonID = _personID;


                //pList.SortURL = GetURL("id",_personID, "xID",_xID, "mode",_mode);
                //pList.DetailURL = GetURL("id","%ID", "xID",_xID, "mode",_mode, "orderColumn",_orderColumn, "orderDir",_orderDir);
                //pList.DetailEnabled = true;

                //Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.GROUP, pList);

            }
            else if (_mode == "performanceLeaderHistory" && Global.isModuleEnabled("performance"))
            {
                PerformanceRatingList pList = (PerformanceRatingList)this.LoadPSOFTControl(PerformanceRatingList.Path, "_erht");

                pList.ContextSearch = "subnavHistoryLeaderPersonWithRating";
                pList.PersonID = this._personID;
                pList.DetailURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=%ID&type=%IS_SELFRATING";
                pList.DetailEnabled = true;
                pList.OrderColumn = "RATING_DATE";
                pList.DeleteEnabled = false;
                SetPageLayoutContentControl(DGLContentLayout.GROUP, pList);

            }
            else if (_mode == "performanceSelfHistory" && Global.isModuleEnabled("performance"))
            {
                PerformanceRatingList pList = (PerformanceRatingList)this.LoadPSOFTControl(PerformanceRatingList.Path, "_erht");

                pList.ContextSearch = "subnavHistorySelfPersonWithRating";
                pList.PersonID = this._personID;
                pList.DetailURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=%ID&type=%IS_SELFRATING";
                pList.DetailEnabled = true;
                pList.OrderColumn = "RATING_DATE";
                pList.DeleteEnabled = false;
                SetPageLayoutContentControl(DGLContentLayout.GROUP, pList);

            }
            else if ((_mode != "") || (_xID > 0))
            {
                PersonListView pList = (PersonListView)this.LoadPSOFTControl(PersonListView.Path, "_plst");
                pList.PersonID = _personID;
                pList.xID = _xID;
                pList.Mode = _mode;
                pList.OrderColumn = _orderColumn;
                pList.OrderDir = _orderDir;
                pList.SortURL = GetURL("id", _personID, "xID", _xID, "mode", _mode);
                pList.DetailURL = GetURL("id", "%ID", "xID", _xID, "mode", _mode, "orderColumn", _orderColumn, "orderDir", _orderDir);
                pList.DetailEnabled = true;

                //Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.GROUP, pList);

                //list-button
                if (_xID > 0 && _mode != "")
                {
                    string alias = "";
                    string aliasExcel = "";
                    switch (_mode.ToLower())
                    {
                        case "oe":
                        case "oeleader":
                            alias = "PhoneListOE";
                            aliasExcel = "PhoneListOEExcel";
                            break;

                        case "searchresult":
                            alias = "PhoneListSearchResult";
                            aliasExcel = "PhoneListSearchResultExcel";
                            _links.LinkGroup3.Caption = _mapper.get("person", "cmtSelectionPersons");
                            //     add to selection
                            _links.LinkGroup3.AddLink(_mapper.get("actions"), _mapper.get("person", "cmAddToSelection"), psoft.Person.SearchFrame.GetURL("searchResultID", _xID));
                            if (_personID > 0)
                            {
                                //     delete from selection
                                _links.LinkGroup3.AddLink(_mapper.get("actions"), _mapper.get("person", "cmDeleteFromSelection"), psoft.Common.DeleteFromSearchResult.GetURL("searchResultID", _xID, "rowID", _personID, "tablename", "PERSON"));
                            }
                            break;
                    }
                    if (alias != "")
                    {
                        PsoftPageLayout.ButtonListAttributes.Add("onClick", "javascript: window.open('" + psoft.Goto.GetURL("alias", alias) + "&param0=" + _xID + "');");
                        PsoftPageLayout.ButtonListVisible = true;
                    }
                    if (aliasExcel != "")
                    {
                        PsoftPageLayout.ButtonExcelAttributes.Add("onClick", "javascript: window.open('" + psoft.Goto.GetURL("alias", aliasExcel) + "&param0=" + _xID + "');");
                        PsoftPageLayout.ButtonExcelVisible = true;
                    }

                }
            }
            if (Global.isModuleEnabled("energiedienst"))
            {
                if (db.userId == _personID && Global.Config.authenticationMode == Config.AUTHENTICATION_MODE.LOGIN)
                {
                    Control tab_temp = _links.LinkGroup2.Controls[0];
                    Control lit_temp = _links.LinkGroup2.Controls[1];
                    Control tab_temp1 = _links.LinkGroup1.Controls[0];
                    Control lit_temp1 = _links.LinkGroup1.Controls[1];
                    _links.LinkGroup1.Controls.Clear();
                    _links.LinkGroup2.Controls.Clear();
                    _links.LinkGroup1.Controls.Add(tab_temp);
                    _links.LinkGroup1.Controls.Add(lit_temp);
                    _links.LinkGroup2.Controls.Add(tab_temp1);
                    _links.LinkGroup2.Controls.Add(lit_temp1);
                }
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}

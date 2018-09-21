using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for EmploymentRating.
    /// </summary>
    public partial class EmploymentRating : PsoftDetailPage
    {

        // User controls variables
        private PsoftLinksControl _links = null;
        private EmploymentRatingItemListView _pList = null;
        private EmploymentRatingHistoryListView _pHistoryList = null;
		private EmploymentRatingEditView _ratingEditView = null;
        private EmploymentRatingEditViewFoamPartner _ratingEditViewFoamPartner = null;
        private EmploymentRatingDetailView _ratingDetailView = null;
        private EmploymentRatingReportView _ratingPrintView = null;

		// Query string variables
        private long _employmentID = -1;
        private long _performanceRatingID = -1;
		private long _criteriaID = -1;

		//private long _job_expactationID = -1;
		private long ratingItemId = -1;
		private long _jobID = -1;
		private string _mode = "detail";	//add, edit or delete
		private string _type = "";	//self or leader: this only used in case of a new rating request.
		private string _orderColumn = "ID";
		private string _orderDir = "asc";
        private bool _locked = false;
        private bool _lockedInGeneral = false;
        private bool _canEdit = false;
        private bool _canDelete = false;

        public EmploymentRating() : base()
        {
            ShowProgressBar = false;
        }

		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

			// Retrieving query string values
            _employmentID = ch.psoft.Util.Validate.GetValid(Request.QueryString["employmentID"], -1);
            _performanceRatingID = ch.psoft.Util.Validate.GetValid(Request.QueryString["performanceRatingID"], -1);
			_criteriaID = ch.psoft.Util.Validate.GetValid(Request.QueryString["criteriaID"], -1);
			ratingItemId = ch.psoft.Util.Validate.GetValid(Request.QueryString["RatingItemID"], -1);
			_jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], -1);
			_mode = ch.psoft.Util.Validate.GetValid(Request.QueryString["mode"], _mode);
			_type = ch.psoft.Util.Validate.GetValid(Request.QueryString["type"], _type);
			_orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
			_orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
		}

		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
			DBData db = DBData.getDBData(Session);

			try
			{
				db.connect();

				switch(_mode.ToLower())
				{
					case "add":
                        if (_jobID == -1 && _employmentID != -1) {                           
                            //this code part should never be reached
                            DataTable jobs = db.getDataTable("select * from JOB where EMPLOYMENT_ID = " + _employmentID);
                            if (jobs.Rows.Count == 1)
                            {
                                //if there is only one job associated with this user switch to this job
                                _jobID = (long)jobs.Rows[0]["ID"];
                            }
                            else if (jobs.Rows.Count > 1)
                            {
                                _jobID = (long)jobs.Rows[0]["ID"];
                            }
                            else
                            {
                                //no jobs found
                            }
                        }
						_performanceRatingID = db.Performance.createNewPerformanceRating(_employmentID,_jobID,_type.ToLower().Equals("self"));
                        _mode = "detail";
						break;
					case "edit":
						break;
					case "delete":
						db.Performance.delete(_performanceRatingID,false);
						break;
                    case "detail":
                        break;
                    case "report":
                        break;
					default:
						break;
				}
                // page-titles var
                string title_ext = "";

                //set-up unset variables
                if (_performanceRatingID > 0)
                {
                    bool isSelfrating = false;
                    _locked = (ch.psoft.Util.Validate.GetValid(db.lookup("LOCK", "PERFORMANCERATING", "ID=" + _performanceRatingID, false), 0) > 0) ? true : false;
                    isSelfrating = (ch.psoft.Util.Validate.GetValid(db.lookup("IS_SELFRATING", "PERFORMANCERATING", "ID=" + _performanceRatingID, false), 0) > 0);
                    if (isSelfrating)
                    {
                        _type = "self"; // this is a self rating
                    }
                    else
                    {
                        _type = "leader"; // this is a rating that has been performed by a leader for an employee
                    }
                    if (_employmentID <= 0)
                    {
                        _employmentID = ch.psoft.Util.Validate.GetValid(db.lookup("EMPLOYMENT_REF", "PERFORMANCERATING", "ID=" + _performanceRatingID.ToString(), false), -1);
                    }

                    if (_jobID <= 0)
                    {
                        _jobID = ch.psoft.Util.Validate.GetValid(db.lookup("JOB_ID", "PERFORMANCERATING", "ID=" + _performanceRatingID, false), -1);
                    }

                    title_ext = db.Performance.getPerformanceRatingJobTitle(_performanceRatingID);
                    //string tmp = db.Performance.getPerformanceRatingFunctionTitle(_performanceRatingID);
                    //if (tmp != "")
                    //{
                    //    title_ext += ", " + tmp; 
                    //}
                }
                else if (_jobID > 0)
                {
                    title_ext = db.Job.getTitle(_jobID);
                    
                    // Funktion nicht anzeigen 28.10.2010 MSR

                    //string tmp = db.Job.getFunctionTitle(_jobID);
                    //if (tmp != "")
                    //{
                    //    title_ext += ", " + tmp;
                    //}
                    if (_mode == "history")
                    {
                        title_ext += " (" + _mapper.get("performance", "historyRating") + ")";
                    }
                }
                if (_jobID > 0)
                {
                    string tmp = db.Job.getDepartmentName(_jobID);
                    if (tmp != "")
                    {
                        title_ext += ", " + tmp;
                    }
                }

                if (_jobID != -1
                    && 
                    !db.Person.isLeaderOfPerson(db.Employment.getPersonId(_employmentID), true) 
                    &&
                    !db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)
                    )
                {
                    // Keine Leserechte!
                    Response.Redirect(NotFound.GetURL(), false);
                    return;
                }

                // Setting main page layout
                PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
                PageLayoutControl = PsoftPageLayout;

                if (_type.ToLower().Equals("self"))
                {
                    ((PsoftPageLayout)PageLayoutControl).PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING_SELF)
                        + " " + db.Performance.getRatingPageNameTitle(_employmentID);
                }
                else
                {
                    ((PsoftPageLayout)PageLayoutControl).PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING)
                        + " " + db.Performance.getRatingPageNameTitle(_employmentID);
                }
                if (title_ext != "") ((PsoftPageLayout)PageLayoutControl).PageTitle += ", " + title_ext;
                if (_performanceRatingID > 0) ((PsoftPageLayout)PageLayoutControl).PageTitle += " (" + db.Performance.getRatingPageDateTitle(_performanceRatingID) + ")";


                
                if (_type.ToLower().Equals("leader")) {
                    //TODO: really needed?
                    _canEdit = db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true);
                    _canDelete = db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true);
                } else {
                    _canEdit = true;
                    _canDelete = true;
                }

                _canEdit = (_locked) ? false : _canEdit;
                _canDelete = (_locked) ? false : _canDelete;

                long turnId = ch.psoft.Util.Validate.GetValid(db.lookup("WERT","PROPERTY","gruppe='mbo' and title='turn'",false),0L);
                long persId = DBColumn.GetValid(db.lookup("person_id","employment","id="+_employmentID),0L);

                
                DataTable propertyTable = db.getDataTable("select 1 from PROPERTY where gruppe='performance' and title='lock' and not (getdate() > dateadd(d, +1, datum_wert) or datum_wert is null)");
                if (propertyTable.Rows.Count > 0)
                {
                    _lockedInGeneral = true;
                }
                

				// Setting content layout of page layout
				PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
				((DGLContentLayout) PageLayoutControl.ContentLayoutControl).GroupHeight = Unit.Percentage(25);

				// Setting breadcrumb caption
                if (_type.ToLower().Equals("self")) //"self" if an employee select to edit his/her own rating (self rating)
                {
                    this.BreadcrumbCaption = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING_SELF);
                }
                else
                {
                    this.BreadcrumbCaption = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING);
                }

				// Setting links control for given person 
				_links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
				_links.LinkGroup1.Caption = _mapper.get("actions");
				_links.LinkGroup2.Caption = _mapper.get("performance","actualRating");
				_links.LinkGroup3.Caption = _mapper.get("reportLinks");

				//add new performance rating record
                DataTable empljobs = db.getDataTable("select * from JOB where EMPLOYMENT_ID = " + _employmentID);
                if (_jobID != -1)
                {
                    if (!_lockedInGeneral 
                        //&& db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)
                        )
                    {
                        if (_type.ToLower().Equals("self") 
                            && db.userId == db.Employment.getPersonId(_employmentID))
                        {
                            _links.LinkGroup1.AddLink(_mapper.get("performance", "performanceRating"),
                                _mapper.get("performance", "newSelfRating"),
                                "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID.ToString() + "&jobID=" + _jobID + "&mode=add&type=" + _type.ToString());
                        }
                        else if(db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))
                        //else if((_type.ToLower().Equals("leader") && (db.Person.isLeaderOfPerson(db.Employment.getPersonId(_employmentID), true)
                        //        && db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))
                        //        || db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", _jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))) 
                        {
                            _links.LinkGroup1.AddLink(_mapper.get("performance", "performanceRating"),
                                _mapper.get("performance", "newRating"),
                                "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID.ToString() + "&jobID=" + _jobID + "&mode=add&type=" + _type.ToString());
                        }
                    }
                }
                else //if(_jobID == -1 || empljobs.Rows.Count > 1)
                {
                    //provide links to other options (available jobs for the current user, if any)
                    if (_employmentID != -1)
                    {
                        _links.LinkGroup3.Caption = _mapper.get("performance", "performanceRatingMoreOptionsTitle");
                        int k = 0;
                        foreach (DataRow job in empljobs.Rows)
                        {
                            k++;
                            if (_type.ToLower().Equals("self") 
                                && db.userId == db.Employment.getPersonId(_employmentID))
                            {
                                _links.LinkGroup3.AddLink(k + ". " + db.Employment.getPersonName(_employmentID) + " (" + job[db.langAttrName("JOB", "TITLE")] + ", " + db.Job.getDepartmentName((long)job["ID"]) + ")",
                                    _mapper.get("performance", "newSelfRating"),
                                    "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID.ToString() + "&jobID=" + job["ID"] + "&mode=add&type=" + _type.ToString());
                            }
                            else if(_type.ToLower().Equals("leader") 
                                    && db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", (long)job["ID"], DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)) 
                            {
                                _links.LinkGroup3.AddLink(k + ". " + db.Employment.getPersonName(_employmentID) + " (" + job[db.langAttrName("JOB", "TITLE")] + ", " + db.Job.getDepartmentName((long)job["ID"]) + ")",
                                    _mapper.get("performance", "newRating"),
                                    "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID.ToString() + "&jobID=" + job["ID"] + "&mode=add&type=" + _type.ToString());
                            }
                        }
                    }
                }

				//add rating history link
                string historyRatingLinkname = _type.ToLower().Equals("self") ? _mapper.get("performance", "historyRatingSelf") : _mapper.get("performance", "historyRatingLeader");

				_links.LinkGroup1.AddLink(_mapper.get("performance","performanceRating"), 
					historyRatingLinkname, 
					"/Performance/EmploymentRating.aspx?type="+ _type +"&employmentID=" + _employmentID + "&jobID="+_jobID + "&mode=history&orderDir=desc");

				switch(_mode.ToLower())
				{
					case "history":
						_pHistoryList = (EmploymentRatingHistoryListView)this.LoadPSOFTControl(EmploymentRatingHistoryListView.Path, "_erht");
						_pHistoryList.EmploymentID = _employmentID;
						_pHistoryList.JobID = _jobID;
						_pHistoryList.SelfRating = _type.ToLower().Equals("self");
                        if (_orderColumn != "ID"){
                            _pHistoryList.OrderColumn = _orderColumn;
                        }
						_pHistoryList.OrderDir = _orderDir;
						_pHistoryList.SortURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID + "&performanceRatingID=" + _performanceRatingID + "&jobID="+_jobID +"&mode=" + _mode + "&type=" + _type;
                        _pHistoryList.DeleteEnabled = _canDelete;
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, _pHistoryList);
						break;
					case "edit":
                        if (Global.isModuleEnabled("foampartner")&& (DateTime)db.lookup("RATING_DATE", "PERFORMANCERATING", "ID=" + _performanceRatingID) > Convert.ToDateTime("01.10.2016 00:00:00"))
                        {
                            _ratingEditViewFoamPartner = (EmploymentRatingEditViewFoamPartner)this.LoadPSOFTControl(EmploymentRatingEditViewFoamPartner.Path, "_erev");
                            _ratingEditViewFoamPartner.CriteriaID = _criteriaID;
                            _ratingEditViewFoamPartner.EmploymentRatingID = _performanceRatingID;
                            _ratingEditViewFoamPartner.RatingTypeSelf = _type.ToLower().Equals("self");
                            _ratingEditViewFoamPartner.EmploymentID = _employmentID;
                            _ratingEditViewFoamPartner.OrderColumn = _orderColumn;
                            _ratingEditViewFoamPartner.RatingItemId = ratingItemId;
                            _ratingEditViewFoamPartner.OrderDir = _orderDir;
                            SetPageLayoutContentControl(DGLContentLayout.DETAIL, _ratingEditViewFoamPartner);
                        }
                        else
                        {
                            _ratingEditView = (EmploymentRatingEditView)this.LoadPSOFTControl(EmploymentRatingEditView.Path, "_erev");
                            _ratingEditView.CriteriaID = _criteriaID;
                            _ratingEditView.EmploymentRatingID = _performanceRatingID;
                            _ratingEditView.RatingTypeSelf = _type.ToLower().Equals("self");
                            _ratingEditView.EmploymentID = _employmentID;
                            _ratingEditView.OrderColumn = _orderColumn;
                            _ratingEditView.RatingItemId = ratingItemId;
                            _ratingEditView.OrderDir = _orderDir;
                            SetPageLayoutContentControl(DGLContentLayout.DETAIL, _ratingEditView);
                        }
						

						_pList = (EmploymentRatingItemListView)this.LoadPSOFTControl(EmploymentRatingItemListView.Path, "_erat");
						_pList.RatingID = _performanceRatingID;
                        if (_orderColumn != "ID"){
                            _pList.OrderColumn = _orderColumn;
                        }
						_pList.OrderDir = _orderDir;
                        _pList.CriteriaID = _criteriaID;
						_pList.RatingItemId = ratingItemId;
                        _pList.SortURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID + "&performanceRatingID=" + _performanceRatingID + "&mode=" + _mode + "&type=" + _type;
                        _pList.DetailURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID + "&criteriaID=%CRITERIA_REF&jobexpactionsID=%EXPECTATION_REF&RatingItemID=%ID&mode=detail&type=" + _type + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                        _pList.EditURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID + "&criteriaID=%CRITERIA_REF&mode=edit&type=" + _type + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                        _pList.EditEnabled = _canEdit;
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, _pList);
						break;
                    case "detail":
                        _ratingDetailView = (EmploymentRatingDetailView)this.LoadPSOFTControl(EmploymentRatingDetailView.Path, "_erev");
                        _ratingDetailView.CriteriaID = _criteriaID;
                        _ratingDetailView.EmploymentRatingID = _performanceRatingID;
                        _ratingDetailView.RatingTypeSelf = _type.ToLower().Equals("self");
                        _ratingDetailView.EmploymentID = _employmentID;
						_ratingDetailView.RatingItemId = ratingItemId;

                        SetPageLayoutContentControl(DGLContentLayout.DETAIL, _ratingDetailView);

                        _pList = (EmploymentRatingItemListView)this.LoadPSOFTControl(EmploymentRatingItemListView.Path, "_erat");
                        _pList.RatingID = _performanceRatingID;
                        if (_orderColumn != "ID"){
                            _pList.OrderColumn = _orderColumn;
                        }
                        _pList.OrderDir = _orderDir;
                        _pList.CriteriaID = _criteriaID;
						_pList.RatingItemId = ratingItemId;
                        _pList.SortURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?employmentID=" + _employmentID + "&performanceRatingID=" + _performanceRatingID + "&mode=" + _mode + "&type=" + _type;
                        _pList.DetailURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID + "&criteriaID=%CRITERIA_REF&jobexpactionsID=%EXPECTATION_REF&RatingItemID=%ID&mode=detail&type=" + _type + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                        _pList.EditURL = Global.Config.baseURL + "/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID + "&criteriaID=%CRITERIA_REF&mode=edit&type=" + _type + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                        _pList.EditEnabled = _canEdit;
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, _pList);
                        break;
                    case "report":   
                        _ratingPrintView = (EmploymentRatingReportView)this.LoadPSOFTControl(EmploymentRatingReportView.Path, "_sC");
                        //PageLayoutControl.ContentLayoutControl = (MainContentLayout)this.LoadPSOFTControl(MainContentLayout.Path, "mC");
                        
                        SetPageLayoutContentControl(DGLContentLayout.DETAIL, _ratingPrintView);
                        
                        //SetPageLayoutContentControl(SimpleContentLayout.CONTENT, _ratingPrintView);
                        break;
					default:
						break;
				}

				

				//criteria rating
				DataTable criteriaRatingTable = db.Performance.getRatingCriteriaTitleTable(_performanceRatingID);
//                bool editing = true;
//                if (_type.ToLower().Equals("leader") && ch.psoft.Util.Validate.GetValid(db.lookup("IS_SELFRATING","PERFORMANCERATING","ID="+_performanceRatingID,false),0) > 0)
//                {
//                    editing = false; // leader cannot edit selfratings
//                }
//                editing = (_locked) ? false : editing;
                foreach (DataRow row in criteriaRatingTable.Rows) 
				{
					String criteriaId = ch.psoft.Util.Validate.GetValid(row["CRITERIA_REF"].ToString());

					/*
					string expactationSql = "select ID,"+ db.langAttrName("JOB_EXPECTATION","TITLE") +" FROM JOB_EXPECTATION WHERE CRITERIA_REF=" +criteriaId 
						                   + " AND ID IN (SELECT EXPECTATION_REF FROM PERFORMANCERATING_ITEMS WHERE PERFORMANCERATING_REF= "+ _performanceRatingID + ")"
					                       + " AND JOB_REF = "+ _jobID;
					*/

					
					string expactationSql = "select ID, EXPECTATION_REF,"+ db.langAttrName("PERFORMANCERATING_ITEMS","EXPECTATION_TITLE") +" FROM PERFORMANCERATING_ITEMS WHERE CRITERIA_REF=" +criteriaId 
						+ " AND PERFORMANCERATING_REF= "+ _performanceRatingID;
                    

					DataTable expectationsTable = db.getDataTable(expactationSql,"PERFORMANCERATING_ITEMS");
                    					
					long expId = ch.psoft.Util.Validate.GetValid( expectationsTable.Rows[0]["EXPECTATION_REF"].ToString(),-1L);
					if( expectationsTable.Rows.Count == 1 && expId < 0)
					{
                        //note: it is it possible to edit the performance even if an expectation is missing
						_links.LinkGroup2.AddLink(_mapper.get("performance","executeRating"), 
							_mapper.get("performance",ch.psoft.Util.Validate.GetValid(row[db.langAttrName("PERFORMANCERATING_ITEMS","CRITERIA_TITLE")].ToString(), "")),
                            "/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID + "&criteriaID=" + ch.psoft.Util.Validate.GetValid(row["CRITERIA_REF"].ToString(), "") + "&RatingItemID=" + ch.psoft.Util.Validate.GetValid(expectationsTable.Rows[0]["ID"].ToString(), "") + "&mode=" + (_canEdit ? "edit" : "detail") + "&type=" + _type.ToString() + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir);
					}
					else
					{
						_links.LinkGroup2.AddLink(_mapper.get("performance","executeRating"), 
							_mapper.get("performance",ch.psoft.Util.Validate.GetValid(row[db.langAttrName("PERFORMANCERATING_ITEMS","CRITERIA_TITLE")].ToString(), "")), 
							"");

						foreach (DataRow expRow in expectationsTable.Rows) 
						{
							string linkName = ch.psoft.Util.Validate.GetValid(expRow[db.langAttrName("PERFORMANCERATING_ITEMS","EXPECTATION_TITLE")].ToString());
							if(linkName.Length > 25)
								linkName = linkName.Substring(0,25);

                            string relWeight = db.lookup("RELATIV_WEIGHT", "PERFORMANCERATING_ITEMS", "ID = " + ch.psoft.Util.Validate.GetValid(expRow["ID"].ToString(), "")).ToString();
                            if (relWeight == "-1")
                            {
                                // noch nicht bewertet (rote Ampel)
                                linkName = "<img src=\"" + Global.Config.baseURL + "/images/ampelRot.gif\" border=0>&nbsp;" + linkName;
                            }
                            else
                            {
                                // schon bewertet (grüne Ampel)
                                linkName = "<img src=\"" + Global.Config.baseURL + "/images/ampelGruen.gif\" border=0>&nbsp;" + linkName;
                            }

							_links.LinkGroup2.AddLink(_mapper.get("performance","executeRating"), 
								"&nbsp;&nbsp;&nbsp;"+ linkName, 
								"/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID + "&criteriaID=" + ch.psoft.Util.Validate.GetValid(row["CRITERIA_REF"].ToString(), "") + "&RatingItemID=" + ch.psoft.Util.Validate.GetValid(expRow["ID"].ToString(), "") + "&mode=" + (_canEdit ? "edit" : "detail") + "&type="+_type.ToString() + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir);							
						}
					} 										
				}

				//add report link
				switch(_performanceRatingID)
				{
					case -1:
						break;

					default:
                        _links.LinkGroup2.AddLink(_mapper.get("performance","reportRating"), 
							_mapper.get("performance","performanceRatingReport"), 
                            "/Performance/EmploymentRating.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID +"&mode=report", "", "_self");

                        if (PerformanceModule.showGlobalPerformanceReport) {
							_links.LinkGroup2.AddLink(_mapper.get("performance","reportRating"), 
								_mapper.get("performance","performanceRatingGlobalReport"), 
								"/Performance/PrintGlobalPerformance.aspx?performanceRatingID=" + _performanceRatingID + "&employmentID=" + _employmentID, "", "_new");
                        }
						break;
				}

				SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);
			}
			catch(Exception ex)
			{
				Logger.Log(ex, Logger.ERROR);
				ShowError(ex.Message);
			}
			finally
			{
				db.disconnect();
			}
		}

        /// <summary>
        /// Returns all applicable employments for the current user, including himself/herself
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static string employmentTableSql(DBData db)
        {
            string emplTableSql = "select e.ID, p.PNAME + ' ' + isnull(p.FIRSTNAME,'') + ' (' + RTRIM(isnull(p.MNEMO,'')) + ')' + ', ' + "
                                              + "e." + db.langAttrName("EMPLOYMENT", "TITLE")
                                              + " from PERSON p, EMPLOYMENT e where p.ID = e.PERSON_ID and "
                                              + "("
                                              + "e.ID in (" + db.Performance.getRateableEmploymentIDs() + ") "
                                              + " or e.ID in (select distinct ID from EMPLOYMENT where PERSON_ID = " + db.userId + ")"//+ " and ORGENTITY_ID is not null)"
                                              + ")"
                                              + " order by p.PNAME, p.FIRSTNAME, "
                                              + "e." + db.langAttrName("EMPLOYMENT", "TITLE");
            return emplTableSql;
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

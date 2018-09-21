using System;
using System.Data;

namespace ch.appl.psoft.MbO
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.MbO.Controls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface.DBObjects;

    /// <summary>
    /// 
    /// </summary>
    public partial class Detail : PsoftTreeViewPage {
        private string _context = "";  // see Objective
        private long _contextId = 0;
        private string _view = "detail";  // trace, detail, add, edit, copy, link, replace
        private long _id = 0; // objective
        private int _typ = Objective.UNDEFINED_TYP;
        private string _myURL = "";
        private DBData _db = null;

        #region Protected overrided methods from parent class
        protected override void Initialize() {
            // base initialize
            base.Initialize();

            _context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"],_context);
            string ids = Request.QueryString["id"];
            if (ids != null) {
                _id = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"], 0L);
                if (_id == 0 && ids.Length > 2) _id = ch.psoft.Util.Validate.GetValid(ids.Substring(2), 0L);
            }
            _contextId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], 0L);
            base.SubNavMenuUrl = "/MbO/SubNavMenu.aspx?context="+_context+"&contextId="+_contextId+"','addObjective','"+_id;
        }
        protected override void AppendBodyOnLoad(System.Text.StringBuilder bodyOnLoad) {
            base.AppendBodyOnLoad (bodyOnLoad);
            bodyOnLoad.Append("if (typeof(checkFlags) == 'function') checkFlags();");
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            int typ = ch.psoft.Util.Validate.GetValid(Request.QueryString["typ"],-1);
            _view = ch.psoft.Util.Validate.GetValid(Request.QueryString["view"],_view);
            _myURL = this.Request.RawUrl;

            base.BreadcrumbCaption = base._mapper.get("mbo",_view+"Objective");
            base.BreadcrumbName += _view;
            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PsoftPageLayout pageLayout = PageLayoutControl as PsoftPageLayout;
            pageLayout.PageTitle = base._mapper.get("mbo",_view+"Objective");
            //

            _db = DBData.getDBData(Session);
            _db.connect();

            // get turn id from querystring, if not set, use default
            string turnid = _db.Objective.turnId.ToString();
            if (Request.QueryString["turnid"] != null && Request.QueryString["turnid"] != "")
            {
                turnid = Request.QueryString["turnid"];
            }

            if (Request.QueryString["context"] == "SELECTION")
            {
                turnid = _db.lookup("OBJECTIVE_TURN_ID", "OBJECTIVE", "ID = " + Request.QueryString["contextid"]).ToString();
            }
            pageLayout.ButtonPrintAttributes.Add("onClick", 
                "javascript: window.open('ObjectiveReport.aspx?context=" + _context + "&contextId="+_contextId+"&turnid=" + turnid + "','ObjectiveReport')");
            pageLayout.ButtonPrintToolTip = _mapper.get("mbo","personReportTP");
            pageLayout.ButtonPrintVisible = _context == Objective.PERSON && _view == "detail";

            try	{
                // deprecated, new method is to check for row authorisation / 22.06.10 / mkr
                int objectiveAccess = _db.getTableAuthorisations(_db.userId,"OBJECTIVE",true);

                DataTable table = _db.getDataTable("select * from OBJECTIVE where id = "+_id);
                DataRow row = table.Rows.Count > 0 ? table.Rows[0] : null;
                long taskId = row == null ? 0 : DBColumn.GetValid(row["TASKLIST_ID"],0L);
                int competence = row == null ? 0 : _db.Objective.getCompetence(row);
                if (row != null && typ < 0) _typ = (int) row["TYP"];
                else if (typ >= 0) _typ = typ;

                if (_view == "trace") {
                    SimpleContentLayout control = (SimpleContentLayout)this.LoadPSOFTControl(SimpleContentLayout.Path, "_cl");
                    PageLayoutControl.ContentLayoutControl = control;
                    TraceView trace = (TraceView)this.LoadPSOFTControl(TraceView.Path, "_trace");
                    trace.id = _id;
                    trace.context = _context;
                    trace.contextId = _contextId;
                    SetPageLayoutContentControl(SimpleContentLayout.CONTENT, trace);	
                }
                else {
                    DDGLContentLayout control = (DDGLContentLayout)this.LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
                    PageLayoutControl.ContentLayoutControl = control;

                    if (_view == "detail") {

                        // links
                        PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                        links.LinkGroup1.Caption = _mapper.get("actions");

                        // selection of turn
                        links.LinkGroup2.Caption = "Zielrunden";

                        string turn_link = _myURL;

                        // remove id from link to leave detail view empty / 18.10.10 / mkr
                        turn_link = System.Text.RegularExpressions.Regex.Replace(turn_link, @"&id=[0-9]*", "&id=0");

                        if (turn_link.Contains("turnid"))
                        {
                            turn_link = turn_link.Substring(0, turn_link.IndexOf("&turnid="));
                        }

                        DataTable zielrunden = _db.getDataTable("SELECT ID, TITLE_DE FROM OBJECTIVE_TURN", new object[0]);
                        foreach (DataRow zielrunde in zielrunden.Rows)
                        {
                            links.LinkGroup2.AddLink("", zielrunde["TITLE_DE"].ToString(), turn_link + "&turnid=" + zielrunde["ID"]);
                        }

                        if (_id == 0)
                        {
                            links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "newObjective"), "/MbO/Detail.aspx?typ=5&view=add&id=" + _id + "&personid=" + _contextId);
                            links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "copyObjectiveFrom"), "/MbO/CopyObjective.aspx?contextId=" + _contextId);
                        }


                        if (_id > 0) {
                            Objective.State state = (Objective.State) DBColumn.GetValid(row["STATE"],0);
                            if (_typ != Objective.PROJECT_TYP) {
                                //// tree
                                //TreeView tree = (TreeView)this.LoadPSOFTControl(TreeView.Path, "_tree");
                                //tree.id = _id;
                                //tree.context = _context;
                                //tree.contextId = _contextId;
                                ////tree.rootId = _id;
                                //SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);	
                            }
                            // detail
                            DetailView detail = (DetailView)this.LoadPSOFTControl(DetailView.Path, "_detail");
                            detail.id = _id;
                            detail.backURL = _myURL;
                            SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);		
                            
                            // placeholder for links
                            
                            
                            
                            if (_typ != Objective.JOB_TYP && _typ != Objective.PERSON_TYP && _typ != Objective.PROJECT_TYP && !_db.Objective.isPersonFilterOnly) {
                                //if ((DBData.AUTHORISATION.INSERT & objectiveAccess) > 0) {
                                if(true) {
                                    switch (state) {
                                    case Objective.State.RELEASED:
                                    case Objective.State.CONDITION:
                                    case Objective.State.ACCEPTED:
                                    case Objective.State.DRAFTED:
                                    case Objective.State.REFUSED:
                                    case Objective.State.DELETE:
                                        links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "newObjective"),	"/MbO/Detail.aspx?typ=0&view=add&id="+ _id);
                                        links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "copyObjective"), "/MbO/Detail.aspx?typ=0&view=copy&id=" +_id);
                                        links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "linkOE"),	"/MbO/Detail.aspx?view=link&context="+_context+"&contextId="+_contextId+"&id=" + _id);
                                        break;
                                    }
                                }
                            }
                            //if ((DBData.AUTHORISATION.INSERT & objectiveAccess) > 0 || (competence & 2) == 2) { 
                            Int32 mainJobId = Convert.ToInt32(_db.lookup("JOB.ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN " +
                                                                         "PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID INNER JOIN OBJECTIVE ON PERSON.ID = OBJECTIVE.PERSON_ID", "OBJECTIVE.ID = " + _id));
                            
                            if(_db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "JOB", mainJobId, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true)) {
                                if (!_db.Objective.isPersonFilterOnly) {
                                    switch (state) {
                                    case Objective.State.DRAFTED:
                                    case Objective.State.REFUSED:
                                    case Objective.State.DELETE:
                                        links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "editObjective"), "/MbO/Detail.aspx?view=edit&context="+_context+"&contextId="+_contextId+"&id="+_id);
                                        break;
                                    case Objective.State.ACCEPTED:
                                    case Objective.State.CONDITION:
                                        links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "replaceObjective"), "/MbO/Detail.aspx?view=replace&context="+_context+"&contextId="+_contextId+"&id="+_id);
                                        break;
                                    }
                                }
                                links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "newObjective"), "/MbO/Detail.aspx?typ=5&view=add&id=" + _id + "&personid=" + _contextId);
                                links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "editObjective"), "/MbO/Detail.aspx?view=edit&context=" + _context + "&contextId=" + _contextId + "&id=" + _id);                       
                                links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "deleteObjective"), "javascript:	deleteDetailConfirm(" +	_id + ")");

                                // add link to copy objective / 27.09.10 / mkr
                                // Martin: Kommentar auf folgender Zeile ausblenden um Link anzuzeigen
                                links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "copyObjectiveFrom"), "/MbO/CopyObjective.aspx?contextId=" + _contextId);

                                // add link to copy objective to OE / 26.10.10 / mkr
                                links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"), _mapper.get("mbo", "copyObjectiveTo"), "/MbO/CopyObjectiveToOE.aspx?Id=" + _id);

                            }
                            if (_typ == Objective.ORGENTITY_TYP) {
                                bool access = (DBData.AUTHORISATION.INSERT & objectiveAccess) > 0;
                                if (!access && (_context == Objective.PERSON || _context == Objective.SUPERVISOR)) {
                                    access = _db.Person.isLeaderOfPerson(_db.userId,_contextId,true);
                                }
                                if (access) {
                                    links.LinkGroup1.AddLink(_mapper.get("mbo", "objective"),	_mapper.get("mbo", "traceObjective"), "/MbO/Detail.aspx?view=trace&context="+_context+"&contextId="+_contextId+"&id="+_id);
                                }
                            }
                       
                            if (_typ == Objective.ORGENTITY_TYP &&
                                ((DBData.AUTHORISATION.INSERT & objectiveAccess) > 0 || (competence & 5) == 5)) {
                                string url = "/MbO/Detail.aspx?view=copy&id="+ _id+"&context="+_context+"&contextId="+_contextId;
                                switch (state) {
                                case Objective.State.RELEASED:
                                case Objective.State.CONDITION:
                                case Objective.State.ACCEPTED:
                                case Objective.State.DRAFTED:
                                case Objective.State.REFUSED:
                                case Objective.State.DELETE:
                                  //  if (_db.Orgentity.hasChildJobs((long) row["ORGENTITY_ID"])) links.LinkGroup1.AddLink(_mapper.get("mbo", "organisation"),	_mapper.get("mbo", "copyJob"),url+"&typ="+Objective.JOB_TYP);
                                  //  if (_db.Orgentity.hasChildOEs((long) row["ORGENTITY_ID"])) links.LinkGroup1.AddLink(_mapper.get("mbo", "organisation"),	_mapper.get("mbo", "copyOE"), url+"&typ="+Objective.ORGENTITY_TYP);
                                    break;
                                }
                            }
                            if (_db.Objective.tasklistEnable && taskId > 0)
							{
                                string url = psoft.Tasklist.TaskDetail.GetURL(
										"showRoot", "False",
										"modifTasklist", "disable",
										"assignPerson", "disable",
										"ID", taskId
									);
                                
								switch (state) {
                                case Objective.State.RELEASED:
                                case Objective.State.CONDITION:
                                case Objective.State.ACCEPTED:
                                case Objective.State.DRAFTED:
                                case Objective.State.REFUSED:
                                case Objective.State.DELETE:
                                    links.LinkGroup1.AddLink(_mapper.get("mbo", "tasklist"),	_mapper.get("mbo", "showTasklist"), url);
                                    break;
                                }
                            }
                            //SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);	
                        }
                        SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);
                        
                        // group
                        control.GroupWide = true;
                        addList();
                    
                    }
                    else if (_view == "link") {
                        if ((DBData.AUTHORISATION.INSERT & objectiveAccess) > 0) {
                            switch (_typ) {
                            case Objective.UNDEFINED_TYP:
                            case Objective.ORGANISATION_TYP:
                            case Objective.ORGENTITY_TYP:
                                // objective
                                //TreeView tree = (TreeView)this.LoadPSOFTControl(TreeView.Path, "_tree");
                                //tree.id = _id;
                                //tree.context = _context;
                                //tree.contextId = _contextId;
                                //SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);
                                //// OE
                                OrganisationView detail = (OrganisationView)this.LoadPSOFTControl(OrganisationView.Path, "_oeTree");
                                detail.contextId = _context == Objective.ORGANISATION ? _contextId : 0;
                                detail.id = _id;
                                SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);		
                                break;
                            
                            default:
                                break;
                            }
                        }
                        // group
                        control.GroupWide = true;
                        addList();
                    }
                    else { // edit, add, copy, replace
                        if (_context == Objective.PROJECT) _typ = Objective.PROJECT_TYP;
                        switch (_typ) {
                        case Objective.PERSON_TYP:
                        case Objective.PROJECT_TYP:
                            break;
                        default:
                            if (_view == "edit") {
                                // tree
                                //TreeView tree = (TreeView)this.LoadPSOFTControl(TreeView.Path, "_tree");
                                //tree.id = _id;
                                //tree.rootId = _id;
                                //tree.checkBoxEnable = true;
                                //tree.detailEnable = false;
                                //tree.treeTitle = _mapper.get("mbo","siblingObjectives");
                                //SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, tree);	
                            }
                            break;
                        }
                        // edit
                        EditView edit = (EditView)this.LoadPSOFTControl(EditView.Path, "_detail");
                        edit.action = _view;
                        edit.typ = _typ;
                        edit.id = _id;
                        edit.context = _context;
                        edit.contextId = _contextId;
                        SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, edit);	
                    }
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                _db.disconnect(); 
            }

        }

        private void addList() {
            ListView list = (ListView)this.LoadPSOFTControl(ListView.Path, "_list");
            list.context = _context;
            list.contextId = _contextId;
            list.id = _id;
            list.backURL = _myURL;
            list.Visible = _id > 0;
            list.view = "detail";
            list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "TITLE");
            list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");
            SetPageLayoutContentControl(DDGLContentLayout.GROUP, list);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    

        }
		#endregion
    }
}


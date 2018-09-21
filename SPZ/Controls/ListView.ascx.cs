namespace ch.appl.psoft.SPZ.Controls
{
    using ch.appl.psoft.Interface.DBObjects;
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using Telerik.Web.UI;



    /// <summary>
    ///		Summary description for ListView.
    /// </summary>
    public partial  class ListView : PSOFTSearchListUserControl {
        protected string _deleteMessage = "";
        protected string _backURL = "";
        protected string _deleteURL = "";
        protected string _context = "";
        protected string _view = "";
        protected long _contextId = 0;
        protected long _id = 0;


        private DBData _db = null;
        private DataTable _table = null;
        private DateTime _validationFrom;
        private DateTime _validationTo;
        private bool _workflow;
        private int _objectiveTabAccess = 0;
        private string _sql = "";
        private string turnid;

        public const string PARAM_QUERY = "PARAM_QUERY";

        public static string Path {
            get {return Global.Config.baseURL + "/SPZ/Controls/ListView.ascx";}
        }

        #region Properities


        /// <summary>
        /// Get/Set context id
        /// </summary>
        public long contextId {
            get {return _contextId;}
            set {_contextId = value;}
        }
        /// <summary>
        /// Get/Set query for the list
        /// </summary>
        public string query {
            get {return GetString(PARAM_QUERY);}
            set {SetParam(PARAM_QUERY, value);}
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }
        /// <summary>
        /// Get/Set view
        /// </summary>
        public string view {
            get {return _view;}
            set {_view = value;}
        }

        /// <summary>
        /// Get/set current list element id
        /// </summary>
        public long id {
            get {return _id;}
            set {_id = value;}
        }
        /// <summary>
        /// Get/set context
        /// </summary>
        public string context {
            get {return _context;}
            set {_context = value;}
        }
        #endregion
    
        protected void Page_Load(object sender, System.EventArgs e) {
            base.Execute();
        }
        protected override void DoExecute() {
            apply.Text = _mapper.get("apply");
            apply.Visible = false;
            next.Text = _mapper.get("next");
            next.Visible = false;
            title.Visible = false;
            rating.Text = _mapper.get("mbo","rating");
            rating.Visible = false;
            if (!Visible && _view == "") return;
            loadList();
        }

        private void loadList() {
            _db = DBData.getDBData(Session);
                
            DetailEnabled = true;
            DeleteEnabled = true;
            EditEnabled = true;
            InfoBoxEnabled = true;
            HeaderEnabled = true;
            string url = Global.Config.baseURL + "/SPZ/Detail.aspx?context="+_context+"&contextId="+_contextId+"&id=%ID&view=%V&orderColumn="+base.OrderColumn+"&orderDir="+base.OrderDir;
            int idx = _backURL.IndexOf("orderColumn");
            if (idx > 0) base.SortURL = _backURL.Substring(0,idx-1);
            else base.SortURL = _backURL;

            _db.connect();
            try {
                switch (_context) {
                case Objective.SEARCH:
                    //DetailEnabled = false;
                    url = Global.Config.baseURL + "/SPZ/Detail.aspx?id=%ID&view=%V&orderColumn="+base.OrderColumn+"&orderDir="+base.OrderDir;
                    DeleteEnabled = false;
                    EditEnabled = false;
                    CheckBoxEnabled = true;
                    SortURL = "";
                    next.Visible = true;
                    if (query != "")
                    {
                        _sql = query;

                        // show only objectives from employed persons / 22.06.10 / mkr
                        _sql += " AND PERSON_ID IN (select top(1) person.id from person where (person.id = OBJECTIVE.PERSON_ID) and (PERSON.LEAVING IS NULL))";
                    }
                    else
                    {
                        _sql = "select * from OBJECTIVEV where OBJECTIVE_TURN_ID=" + _db.Objective.turnId;
                    }
                    break;

                case Objective.SELECTION:
                    title.Visible = true;
                    title.Text = _mapper.get("mbo","selectedObjectives");
                    if (_contextId > 0) _sql = "select * from OBJECTIVEV where ID in (select ROW_ID from SEARCHRESULT where TABLENAME='OBJECTIVE' and ID=" + _contextId + ")";
                    else if (query != "") _sql = query;
                    else _sql = "select * from OBJECTIVEV where OBJECTIVE_TURN_ID="+_db.Objective.turnId;
                    break;

                case Objective.PERSON:
                    // get turn id from querystring, if not set, use default
                    string turnid = _db.Objective.turnId.ToString();
                    // get turn id from querystring, if exist objectiveId  

                    if ((Request.QueryString["id"] != null && Request.QueryString["id"].Replace("0,0,", "").Replace("0,", "").Length > 5) && (Request.QueryString["turnid"] == "" || Request.QueryString["turnid"] == null))
                    {
                        turnid = _db.lookup("OBJECTIVE_TURN_ID", "OBJECTIVE", "ID = " + Request.QueryString["id"].Substring(Request.QueryString["id"].LastIndexOf(',') + 1, Request.QueryString["id"].Length - Request.QueryString["id"].LastIndexOf(',') - 1), "").ToString();
                    }

                    if (Request.QueryString["turnid"] != null && Request.QueryString["turnid"] != "")
                    {
                        turnid = Request.QueryString["turnid"];
                    }

                    _sql = "select O.*,R.ID RID,R.RATING_WEIGHT,R.RATING,R.RATING_DATE from OBJECTIVEPERSONV O left join OBJECTIVE_PERSON_RATING R"
                        + " on O.PERSONID = R.PERSON_ID"
                        + " and O.ID = R.OBJECTIVE_ID"
                        //+" where O.OBJECTIVE_TURN_ID="+_db.Objective.turnId+" and O.PERSONID="+_contextId+" and O.PERSON_ID="+_contextId
                        + " where O.OBJECTIVE_TURN_ID=" + turnid + " and O.PERSON_ID=" + _contextId + " and O.PERSONID=" + _contextId;
                        //+" and O.TYP in ("+_db.Objective.objectiveFilter+")";
                    if (!_db.Objective.isPersonFilterOnly && Global.Config.isModuleEnabled("project")) {
                        string projectIds = _db.Project.getInvolvedProjects(_contextId);
                        _sql = "("+_sql;
                        _sql += " union ";
                        _sql += "select *,NULL PERSONID,NULL RID,NULL RATING,NULL RATING_WEIGHT,NULL RATING_DATE from OBJECTIVE where OBJECTIVE_TURN_ID="
                            +_db.Objective.turnId+" and PROJECT_ID IN ("+(projectIds == "" ? "0" : projectIds)+")";
                        _sql += ")";
                    }

                    title.Visible = true;
                    title.Text = _mapper.get("mbo","objectives")+" "+_db.lookup("pname+' '+firstname","person","id="+_contextId,true);
                    break;

                case Objective.SUPERVISOR:
                    _sql = "select * from OBJECTIVELEADERV where OBJECTIVE_TURN_ID="+_db.Objective.turnId+" and LEADERID="+_contextId;
                    if (Global.Config.isModuleEnabled("project")) {
                        //TODO: an neues Modell anpassen...
//                        _sql = "("+_sql;
//                        _sql += " union ";
//                        _sql += "select O.*,0,P.LEADER_PERSON_ID LEADERID from OBJECTIVE O inner join PROJECT P on O.PROJECT_ID = P.ID where O.OBJECTIVE_TURN_ID="+_db.Objective.turnId+" and P.LEADER_PERSON_ID = "+_contextId;
//                        _sql += ")";
                    }

                    title.Visible = true;
                    title.Text = _mapper.get("mbo","supervisorObjectives")+" "+_db.lookup("pname+' '+firstname","person","id="+_contextId,true);
                    break;

                case Objective.PROJECT:
                    // project
                    _sql =  "select * from OBJECTIVEV where OBJECTIVE_TURN_ID="+_db.Objective.turnId+" and PROJECT_ID="+_contextId;
                    break;
                default:
                    return;
                }
                base.DetailURL = url.Replace("%25V","%V").Replace("%V","detail");
                _deleteURL = base.DetailURL.Replace("%25ID","%ID").Replace("%ID","0");
                base.EditURL = url.Replace("%25V","%V").Replace("%V","edit");
                _objectiveTabAccess = _db.getTableAuthorisations(_db.userId,"OBJECTIVEV",true);
                _validationFrom = _db.Objective.validationFrom;
                _validationTo = _db.Objective.validationTo;
                _workflow = _db.Objective.workflowEnable;

                _deleteMessage = PSOFTConvert.ToJavascript(_mapper.get("mbo", "deleteObjective"));

                _sql += " order by TYP," + OrderColumn + " " + OrderDir;

                loadTable();

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }


        private long _firstRowId = 0;
        private void loadTable() {

            _table = _db.getDataTableExt(_sql,"OBJECTIVEV");   
            _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo","typ",true));
            _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo","state",false));
            _table.Columns["ARGUMENT_ID"].ExtendedProperties["In"] = _db.Objective.arguments;
            _table.Columns["OBJECTIVE_TURN_ID"].ExtendedProperties["In"] = _db.Objective.turns;
            _table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["In"] = _db.Objective.mnemonics;
            _table.Columns["ORGENTITY_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("mode","oe", "ID","%?ID");
            _table.Columns["CURRENTVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["CURRENTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["ARGUMENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

            switch (_context) {
            case Objective.SEARCH:
                    _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;
                    break;
            case Objective.SELECTION:
            case Objective.SUPERVISOR:
               // _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.LIST;
                break;
            case Objective.PERSON:
                //_table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                //_table.Columns["RATING"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST+(int) DBColumn.Visibility.INFO;
                //_table.Columns["RATING"].ExtendedProperties["OrdNum"] = 176;
                //_table.Columns["RATING"].ExtendedProperties["Unit"] = "%";
                //_table.Columns["RATING_WEIGHT"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.LIST+(int) DBColumn.Visibility.INFO;
                //_table.Columns["RATING_WEIGHT"].ExtendedProperties["OrdNum"] = 175;
                //_table.Columns["RATING_WEIGHT"].ExtendedProperties["Unit"] = "%";
                //_table.Columns["RATING_DATE"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.INFO;
                //_table.Columns["RATING_DATE"].ExtendedProperties["OrdNum"] = 177;
                break;
            default:
                break;
            }

            if(! _context.Equals("SEARCH"))
            {
            _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
            _table.Columns["RATING"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.LIST + (int)DBColumn.Visibility.INFO;
            _table.Columns["RATING"].ExtendedProperties["OrdNum"] = 176;
            _table.Columns["RATING"].ExtendedProperties["Unit"] = "%";
            _table.Columns["RATING_WEIGHT"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.LIST + (int)DBColumn.Visibility.INFO;
            _table.Columns["RATING_WEIGHT"].ExtendedProperties["OrdNum"] = 175;
            _table.Columns["RATING_WEIGHT"].ExtendedProperties["Unit"] = "%";
            _table.Columns["RATING_DATE"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.INFO;
            _table.Columns["RATING_DATE"].ExtendedProperties["OrdNum"] = 177;
            }


            base.IDColumn = "ID";
            if (_id > 0) HighlightRecordID = _id;
            base.View = "OBJECTIVE_OE";
            base.CheckOrder = true;
            listTab.Rows.Clear();
            //_listBuilder.checkOrder = true;
            //_listBuilder.UseJavaScriptToSort = true;
            base.LoadList(_db, _table, listTab);
            if (_id <= 0 && _view == "detail" && _firstRowId > 0) Response.Redirect(_backURL+"&id="+_firstRowId,false);
        }

        protected override bool onRowAccess(DataTable table, DataRow row, bool accessPermitted, int access) {
            bool ok = accessPermitted;


            try {
                if (ok) {

                    //if (!_db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, table.TableName, (long)row[0], true, false))
                    //if ((long)_db.lookup("Authorisation", "ACCESS_RIGHT_RT", "TABLENAME = 'OBJECTIVE' AND ACCESSOR_ID = " + _db.userId.ToString()+ " AND ROW_ID = " + row[0].ToString(), 0) < 2L)
                    Int32 mainJobId = Convert.ToInt32(_db.lookup("JOB.ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN " +
                                                                "PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID INNER JOIN OBJECTIVE ON PERSON.ID = OBJECTIVE.PERSON_ID", "OBJECTIVE.ID = " + row[0].ToString()));
                    if(!_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", mainJobId, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
                    {
                        ok = false;
                    }

                                    }
            }
            finally {
                if (ok && _firstRowId == 0) _firstRowId = DBColumn.GetValid(row["ID"],0L);
            }
            return ok;
        }
        protected override void onAddRow(DataRow row, TableRow r) {
        }
        protected override void onAddHeaderCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
            if (col != null) {
                switch (col.ColumnName) {
                case "MEASUREMENT_TYPE_ID":
                    c.Text = "";
                    break;
                case "ORGENTITY_ID":
                    c.Text = _mapper.get("mbo","respPerson");
                    break;
                }
            }
        }
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
            int typ =  DBColumn.GetValid(row["TYP"],0);
            if (col == null && ListBuilder.IsEditCell(c) && !DBColumn.IsNull(row["DATEOFREACHING"])) {
                //DateTime end = (DateTime) row["DATEOFREACHING"];
                //if (end < DateTime.Now) c.Controls.Clear();
                
            }
            if (col == null && ListBuilder.IsInfoCell(c)) {                
                string image = "";
                System.Web.UI.WebControls.Image img = null;

                if (c.Controls.Count > 0 && c.Controls[0] is System.Web.UI.WebControls.Image) img = c.Controls[0] as System.Web.UI.WebControls.Image;

                switch (typ) {
                case Objective.PERSON_TYP:
                    image = "user.gif";
                    break;
                case Objective.JOB_TYP:
                    image = "stelle.gif";
                    break;
                case Objective.ORGANISATION_TYP:
                    image = "organisation.gif";
                    break;
                case Objective.ORGENTITY_TYP:
                    image = "abteilung.gif";
                    break;
                case Objective.PROJECT_TYP:
                    image = "projekt.gif";
                    break;
                default:
                    break;
                }
                if (image != "") {
                    if (img != null) img.ImageUrl = img.ImageUrl.Replace(base._listBuilder.infoImage,image);
                    else c.Text = c.Text.Replace(base._listBuilder.infoImage,image);
                }
                //c.Width = 30;
            }
            if (col != null) {
                if (_context != Objective.SEARCH && 
                    (col.ColumnName == "CURRENTVALUE"
                    || col.ColumnName == "RATING"
                    || col.ColumnName == "RATING_WEIGHT")) 
                {
                    Objective.State state = (Objective.State) row["STATE"];
                    int competence = _db.Objective.getCompetence(row);
                    bool ok = false;

                    if (col.ColumnName == "CURRENTVALUE") ok = (_objectiveTabAccess >= DBData.AUTHORISATION.RU 
                                                              || competence > 0) && DBColumn.GetValid(row["VALUEIMPLICIT"],0) == 0;
                    else ok = _objectiveTabAccess >= DBData.AUTHORISATION.RU || (competence & 6) > 0;
                    switch (state) {
                    case Objective.State.ACCEPTED:
                    case Objective.State.CONDITION:   
                        if (!ok) break;
                        string text = HttpUtility.HtmlDecode(c.Text);
                        RadNumericTextBox fld = new RadNumericTextBox();
                        fld.ID = col.ColumnName+"-"+row["ID"];
                        fld.Text = text;
                        fld.NumberFormat.DecimalDigits = 0;
                       
                        
                        if (col.ColumnName != "CURRENTVALUE") {
                            Label lbl = new Label();
                            lbl.Text = "%";
                            fld.Width = 50;
                            c.Controls.Add(fld);
                            c.Controls.Add(lbl);
                            apply.Visible = true;
                            rating.Visible = true;
                        }
                        else if (DBColumn.GetValid(row["RATING"],-1) <= 0 || (competence & 6) > 0) {
                            c.Controls.Add(fld);
                            apply.Visible = true;
                        }
                        break;
                    }
                }
                else if (col.ColumnName == "ARGUMENT_ID" && _context != Objective.SEARCH) {
                    string text = c.Text;
                    if (text != "") c.ToolTip = DBColumn.GetValid(row["ARGUMENT"],"");
                    if (DateTime.Now >= _validationFrom && DateTime.Now <= _validationTo) {
                        if (_objectiveTabAccess >= DBData.AUTHORISATION.RU || (_db.Objective.getCompetence(row) & 1) > 0) {
                            int sem = Objective.RED;
                            int percent = 100;
                            _db.Objective.getSemaphore((long) row["ID"], out sem, out percent);
                            if (sem == Objective.RED) {
                                HyperLink link = new HyperLink();
                                c.Controls.Add(link);
                                link.NavigateUrl = "javascript: openArgument(\"Argument.aspx?id="+row["ID"]+"\")";
                                if (text == "") link.Text = _mapper.get("mbo","argument");
                                else {
                                    link.Text = text;
                                    link.ToolTip = DBColumn.GetValid(row["ARGUMENT"],"");
                                }
                            }
                        }
                    }
                }
                else if (col.ColumnName == "STATE"  && _context != Objective.SEARCH) {
                    if (_workflow) {
                        int competence = _db.Objective.getCompetence(row);
                        Objective.State state = (Objective.State) row["STATE"];
                        DropDownList list = new DropDownList();
                        int idx = (int) state;
                        
                        list.Items.Add(new ListItem(_mapper.getToken("mbo","state",idx.ToString()),idx.ToString()));
                        list.ID = "STATE_"+row["ID"];
                        if (_objectiveTabAccess >= DBData.AUTHORISATION.RU || (competence & 1) > 0) {                    
                            switch (state) {
                            case Objective.State.RELEASED:
                                idx = (int) Objective.State.ACCEPTED;
                                list.Items.Add(new ListItem(_mapper.getToken("mbo","stateAction",idx.ToString()),idx.ToString()));
                                idx = (int) Objective.State.CONDITION;
                                list.Items.Add(new ListItem(_mapper.getToken("mbo","stateAction",idx.ToString()),idx.ToString()));
                                idx = (int) Objective.State.REFUSED;
                                list.Items.Add(new ListItem(_mapper.getToken("mbo","stateAction",idx.ToString()),idx.ToString()));
                                idx = (int) Objective.State.DELETE;
                                list.Items.Add(new ListItem(_mapper.getToken("mbo","stateAction",idx.ToString()),idx.ToString()));
                                break;
                            default:
                                break;
                            }
                        }
                        if (_objectiveTabAccess >= DBData.AUTHORISATION.RU || (competence & 2) > 0) {
                            switch (state) {
                            case Objective.State.DRAFTED:
                            case Objective.State.REFUSED:
                                idx = (int) Objective.State.RELEASED;
                                list.Items.Add(new ListItem(_mapper.getToken("mbo","stateAction",idx.ToString()),idx.ToString()));
                                break;
                            case Objective.State.ACCEPTED:
                            case Objective.State.CONDITION:
                                idx = (int) Objective.State.NOTACTIVE;
                                list.Items.Add(new ListItem(_mapper.getToken("mbo","stateAction",idx.ToString()),idx.ToString()));
                                break;
                            default:
                                break;
                            }
                        }
                        if (idx != (int) state) {
                            c.Controls.Add(list);
                            apply.Visible = true;
                            rating.Visible = true;
                        }
                    }
                }
                else if (col.ColumnName == "ORGENTITY_ID") {
                    HyperLink link = (HyperLink) c.Controls[0];
                    string name = Person.getWholeNameSQL(false,true,false);
                    switch (typ) {
                    case Objective.ORGENTITY_TYP:
                        if (_context == Objective.SUPERVISOR) {
                            if (DBColumn.IsNull(row["MEMBERJOB_ID"])) {
                                link.Text = "";
                                link.NavigateUrl = "";
                            }
                            else {
                                //link.Text = _db.lookup(name,"person inner join jobemploymentv job on person.id = job.person_id","job.typ = 1 and job.id="+row["MEMBERJOB_ID"],true);
                                //link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("person_id","jobemploymentv","typ = 1 and id="+row["MEMBERJOB_ID"],false));
                                link.Text = _db.lookup(name, "person", "id=" + row["PERSON_ID"], true);
                                link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID", "%?ID").Replace("%?ID", _db.lookup("id", "person", "id=" + row["PERSON_ID"], false));
                            }
                        }
                        else {
                            //link.Text = _db.lookup(name,"person inner join jobemploymentv job on person.id = job.person_id","job.typ = 1 and job.orgentity_id="+row["ORGENTITY_ID"],true);
                            //link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("person_id","jobemploymentv","typ = 1 and orgentity_id="+row["ORGENTITY_ID"],false));
                            link.Text = _db.lookup(name, "person", "id=" + row["PERSON_ID"], true);
                            link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID", "%?ID").Replace("%?ID", _db.lookup("id", "person", "id=" + row["PERSON_ID"], false));
                        }
                        break;
                    case Objective.PERSON_TYP:
                        link.Text = _db.lookup(name,"person","id="+row["PERSON_ID"],true);
                        link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("id","person","id="+row["PERSON_ID"],false));
                        break;
                    case Objective.JOB_TYP:
                        //link.Text = _db.lookup(name,"person inner join jobemploymentv job on person.id = job.person_id","job.id="+row["JOB_ID"],true);
                        //link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("person_id","jobemploymentv","id="+row["JOB_ID"],false));
                            link.Text = _db.lookup(name,"person","id="+row["PERSON_ID"],true);
                        link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("id","person","id="+row["PERSON_ID"],false));
                        break;
                    case Objective.ORGANISATION_TYP:
                        link.Text = _db.lookup(name,"person","id="+row["PERSON_ID"],true);
                        link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("id","person","id="+row["PERSON_ID"],false));

                    break;
                    case Objective.PROJECT_TYP:
                        //TODO: an neues Projekt-Modell anpassen...
                        //link.Text = _db.lookup(name,"person inner join project pr on person.id = pr.leader_person_id","pr.id="+row["PROJECT_ID"],true);
                        //link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("leader_person_id","project","id="+row["PROJECT_ID"],false));
                            link.Text = _db.lookup(name,"person","id="+row["PERSON_ID"],true);
                        link.NavigateUrl = link.NavigateUrl.Replace("%25%3fID","%?ID").Replace("%?ID",_db.lookup("id","person","id="+row["PERSON_ID"],false));
                        break;
                    default:
                        break;
                    }
                }
                if (col.ColumnName == "STATE") {
                    // add semaphore
                    TableCell cell = new TableCell();
                    System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                    string[] images = {"red","orange","green","gray"};
                    string[] tp = {_mapper.get("mbo","red"),_mapper.get("mbo","orange"),_mapper.get("mbo","green"),_mapper.get("mbo","gray")};
  
                    r.Cells.Add(cell);
                    cell.Controls.Add(img);
                    cell.Width = 40;
                    cell.HorizontalAlign = HorizontalAlign.Center;
                    img.ImageUrl = "../images/";
                    int sem = Objective.GRAY;
                    int percent = 100;
                    _db.Objective.getSemaphore(DBColumn.GetValid(row["ID"],0L),out sem,out percent);
                    int idx = (percent+10) / 20 * 20;
                    img.ImageUrl += images[sem]+idx+".jpg";
                    img.ToolTip += tp[sem];
                }
            }
        }

        private void applyClick(object sender, System.EventArgs e) {
            string error = "";
            int totWeight = 0;
            _db.connect();
            _db.beginTransaction();
            try {
                string sql = "update objective set %A=%V where id = %ID";
                string rins = "insert into objective_person_rating (person_id,objective_id,%A) values ("
                    +_contextId+",%ID,%V)";
                string rupd = "update objective_person_rating set %A=%V where person_id = "
                    +_contextId+" and objective_id = %ID";
                string tmpSql = "";
                string[] ids = {};

                foreach (TableRow r in listTab.Rows) {
                    foreach (TableCell c in r.Cells) {
                        if (c.Controls.Count == 0 || ch.psoft.Util.Validate.GetValid(c.Controls[0].ID,"") == "") continue;
                        ids = c.Controls[0].ID.Split('-');
                        switch (ids[0]) {
                        //case "RID":
                        //case "RATING":
                        case "RATING_WEIGHT":
                            RadNumericTextBox tempTextBox = (RadNumericTextBox)c.Controls[0];
                            totWeight += int.Parse(tempTextBox.Text);
                            if (totWeight > 100)
                                error = "errorRatingWeightSum";

                            break;
                        //case "RATING_DATE":
                            //break;
                        }

                            if (c.Controls[0] is RadNumericTextBox && sender == rating) {
                            RadNumericTextBox tb = c.Controls[0] as RadNumericTextBox;

                                tmpSql = rupd.Replace("%A",ids[0]).Replace("%ID",ids[1]);
                                string valStr = _db.dbColumn.AddToSql("",_table.Columns[ids[0]],tb.Text);
                                if (valStr != "null") {
                                    try {
                                        double val = double.Parse(tb.Text);
                                        if (val < 0.0 || val > 100.0) {
                                            error = "errorRatingVal";
                                            tb.BackColor = Color.Red;
                                        }
                                    }
                                    catch {
                                        error = "errorRatingVal";
                                        tb.BackColor = Color.Red;
                                    }
                                }
                                tmpSql = tmpSql.Replace("%V",valStr);
                                if (_db.execute(tmpSql) == 0) {
                                    tmpSql = rins.Replace("%A",ids[0]).Replace("%ID",ids[1]);
                                    tmpSql = tmpSql.Replace("%V",valStr);
                                    _db.execute(tmpSql);
                                }
                            }
                       

                            //if (c.Controls[0] is TextBox)
                            //{
                            //    TextBox tb = c.Controls[0] as TextBox;

                            //    tmpSql = sql.Replace("%A", ids[0]).Replace("%ID", ids[1]);
                            //    tmpSql = tmpSql.Replace("%V", _db.dbColumn.AddToSql("", _table.Columns[ids[0]], tb.Text));
                            //    _db.execute(tmpSql);
                            //}
                            //else if (c.Controls[0] is DropDownList)
                            //{
                            //    DropDownList ddl = c.Controls[0] as DropDownList;

                            //    ids = ddl.ID.Split('_');
                            //    tmpSql = sql.Replace("%A", ids[0]).Replace("%ID", ids[1]);
                            //    if (ddl.SelectedItem != null) tmpSql = tmpSql.Replace("%V", ddl.SelectedItem.Value);
                            //    else tmpSql = tmpSql.Replace("%V", "null");
                            //    _db.execute(tmpSql);
                            //}
                            continue;
                    }
                }

                if (sender == rating && error == "")
                {
                    if (Request.QueryString["turnid"] == "" || Request.QueryString["turnid"] == null)
                    {
                        turnid = _db.lookup("OBJECTIVE_TURN_ID", "OBJECTIVE", "ID=" + Request.QueryString["ID"], "").ToString();
                    }
                    else 
                    {
                        turnid = Request.QueryString["turnid"];
                    }

                    //string where = "r.person_id = "+_contextId+" and o.objective_turn_id = "+_db.Objective.turnId
                    string where = "r.person_id = " + _contextId + " and o.objective_turn_id = " + turnid;
                    //+" and o.typ in ("+_db.Objective.objectiveFilter+")";
                    double val = _db.lookup("sum(r.rating_weight)", "objective_person_rating r inner join objective o on r.objective_id = o.id", where, 0.0);
                    if ((int)Math.Round(val, 0) != 100) error = "errorRatingWeightSum";
                    else
                    {
                        sql = "update objective_person_rating set rating = cast(cast(isnull(o.currentvalue,'0') as float) * 100 / cast(o.targetvalue as float) as integer) "
                            + " from (objective o inner join measurement_type t on o.measurement_type_id = t.id and t.number = 1) inner join objective_person_rating r on o.id = r.objective_id and r.person_id = " + _contextId
                            + " and o.targetvalue is not null and o.objective_turn_id = " + _db.Objective.turnId
                            + " where r.rating is null"
                            + " and o.typ in (" + _db.Objective.objectiveFilter + ")";
                        _db.execute(sql);
                        sql = "update objective_person_rating set rating = 100"
                            + " from (objective o inner join measurement_type t on o.measurement_type_id = t.id and t.number = 0) inner join objective_person_rating r on o.id = r.objective_id and r.person_id = " + _contextId
                            + " and o.targetvalue is not null and o.objective_turn_id = " + _db.Objective.turnId
                            + " where o.currentvalue = o.targetvalue and r.rating is null"
                            + " and o.typ in (" + _db.Objective.objectiveFilter + ")";
                        _db.execute(sql);
                        sql = "update objective_person_rating set rating_date = getdate() from objective_person_rating r inner join objective o on r.objective_id = o.id"
                            + " where r.person_id = " + _contextId
                            + " and o.objective_turn_id = " + turnid + " AND NOT r.rating IS NULL";
                        //  +" and o.typ in ("+_db.Objective.objectiveFilter+")";
                        _db.execute(sql);
                    }
                   
                }
                    if (error != "") throw new System.Exception(_mapper.get("mbo", error));
                    _db.commit();
                    loadTable();
                    //Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex) {
                DoOnException(ex);
                _db.rollback();
            }
            finally {
                _db.disconnect();
            }
        }
        private void nextClick(object sender, System.EventArgs e) {
            long searchResultID = SaveInSearchResult(listTab, "OBJECTIVE");

            _nextArgs.LoadUrl = "Detail.aspx?view=detail&context="+Objective.SELECTION+"&contextId=" + searchResultID;

            DoOnNextClick(next);
        }
        private void mapControls() {
            next.Click += new System.EventHandler(this.nextClick);
            apply.Click += new System.EventHandler(this.applyClick);
            rating.Click += new System.EventHandler(this.applyClick);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            mapControls();
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}

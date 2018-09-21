
using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Report;
using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;

namespace ch.appl.psoft.Project.Controls
{

    public partial class ProjectSummaryCtrl : PSOFTSummaryViewUserControl {
        public const string PARAM_X_ID = "PARAM_X_ID";
        public const string PARAM_KONTEXT = "PARAM_KONTEXT";
        public const string PARAM_IS_PRINT_REQUEST = "PARAM_IS_PRINT_REQUEST";
        public const string PARAM_IS_EXPORTPROJECT_REQUEST = "PARAM_IS_EXPORTPROJECT_REQUEST";

        /// <summary>
        /// Used for global project report. Default is: only root projects are considered.
        /// Changing this value to false subprojects are also considered.
        /// </summary>
        bool DO_NOT_INCLUDE_SUBPROJECTS = true;

        enum ReportType
        {
            SUMMARY_REPORT,
            OVERVIEW_REPORT
        }
        ReportType _isExportProjectRequest = ReportType.SUMMARY_REPORT;

        
        private DBData _db = null;
        protected ArrayList _statesPr;
		protected ArrayList _statesPh;
        protected string [] _values = new string[6];

        protected System.Web.UI.WebControls.Table detailTab;

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectSummaryCtrl.ascx";}
        }

		#region Properties
        public long xID {
            get {return GetLong(PARAM_X_ID);}
            set {SetParam(PARAM_X_ID, value);}
        }

        public string Kontext {
            get {return GetString(PARAM_KONTEXT);}
            set {SetParam(PARAM_KONTEXT, value);}
        }

        public string BaseURL {
            get {return Global.Config.baseURL;}
        }

        public bool IsPrintRequest
        {
            get { return GetBool(PARAM_IS_PRINT_REQUEST); }
            set { SetParam(PARAM_IS_PRINT_REQUEST, value); }
        }

		#endregion

        private ProjectGenericReportXML _projectReportStructure = null;


        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            CellPadding = 3;
            CellSpacing = 0;
            ButtonWidth = 15;
            ColumnCount = _values.Length;
            _values[0] = _mapper.get("PROJECT", "TITLE");
            _values[1] = _mapper.get("project", "leaders");
            _values[2] = _mapper.get("PROJECT", "STARTDATE");
            _values[3] = _mapper.get("PROJECT", "DUEDATE");
            _values[4] = _mapper.get("PROJECT", "STATE");
            _values[5] = "";
            ColumnWidths = new int[] {250, 200, 70, 70, 60, 30};
            HeaderColumnAligns = new HorizontalAlign [] {HorizontalAlign.Left, HorizontalAlign.Left, HorizontalAlign.Right, HorizontalAlign.Right, HorizontalAlign.Right, HorizontalAlign.Center};
            ColumnAligns = new HorizontalAlign [] {HorizontalAlign.Left, HorizontalAlign.Left, HorizontalAlign.Right, HorizontalAlign.Right, HorizontalAlign.Right, HorizontalAlign.Center};

            _statesPr = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PROJECT);
			_statesPh = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE);

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                BuildHeader(SummaryTable, _values);

                string sql = "select distinct ID from PROJECT" + _db.getAccessRightsRowInnerJoinSQL("PROJECT", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true);
                switch (Kontext){
                    case ProjectList.CONTEXT_PERSON:
                        string involvedProjects = _db.Project.getInvolvedProjects(xID);
                        if (involvedProjects != ""){
                            sql += " where ID in (" + involvedProjects + ")";
                        }
                        else {
                            sql += " where ID=-1";
                        }
                        break;

                    case ProjectList.CONTEXT_SEARCHRESULT:
                        sql += " where ID in (select ROW_ID from SEARCHRESULT where ID=" + xID + ")";
                        break;

                    case ProjectList.CONTEXT_PROJECTGROUP:
                        sql += " where ID in (select PROJECT_ID from PROJECT_GROUP_PROJECT where PROJECT_GROUP_ID=" + xID + ")";
                        break;
                    case ProjectList.CONTEXT_EXPORT_PROJECT_OVERVIEW:
                        //Overview of all projects (no selection of projects)
                        int idpos = sql.IndexOf("ID");
                        sql = sql.Insert(idpos+"ID".Length,",TITLE");
                        if (!DO_NOT_INCLUDE_SUBPROJECTS)
                        {
                            sql += " where TEMPLATE=0 order by TITLE";
                        }
                        else
                        {
                            sql += " where TEMPLATE=0 and PARENT_ID is null order by TITLE";
                        }
                        _isExportProjectRequest = ReportType.OVERVIEW_REPORT;
                        break;
                }

                DataTable table = _db.getDataTable(sql);

                
                if (this.IsPrintRequest)
                {
                   //print request
                    XmlDocument doc = new XmlDocument();
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                    XmlElement elm = null;

                    string reportTitle = "report";

                    switch (_isExportProjectRequest)
                    {
                        case ReportType.OVERVIEW_REPORT:
                            _projectReportStructure = new ProjectGenericReportXML(this._db, this.Session, ProjectGenericReportXML.ReportType.OVERVIEW_EXPORT);
                            reportTitle = "projectExport";
                            elm = doc.CreateElement(reportTitle) as XmlElement;
                            overview(table, doc, elm);
                            break;
                        default:
                            _projectReportStructure = new ProjectGenericReportXML(this._db, this.Session, ProjectGenericReportXML.ReportType.SUMMARY);
                            reportTitle = "projectSummary";
                            elm = doc.CreateElement(reportTitle) as XmlElement;
                            summary(table, doc, elm);
                            break;
                    }

                    XmlElement translation = _projectReportStructure.createDataToXML().generateTranslationStructure(doc);
                    XmlElement merged = DataToXml.mergeDataWithTranslation(doc, elm, translation, reportTitle);
                    
                    //doc.AppendChild(elm);
                    doc.AppendChild(merged);
                    string filenameAbs = XMLReport.getOutputfileAbsolutePath(reportTitle);
                    doc.Save(filenameAbs);
                    string filenameRel = XMLReport.getOutputfileRelativePath(reportTitle);
                    Response.Redirect(filenameRel);

                }
                else
                { 
                    //display page request
                    foreach (DataRow row in table.Rows)
                    {
                        BuildProject(SummaryTable, ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), 1);
                    }
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }


        private void overview(DataTable table, XmlDocument doc, XmlElement elm)
        {
            // add total number of project for each department
            // add department node
            // - foreach department number of projects + information 

            // 1. ALL (root department)
            // total number of projects all departments
            int totalNumberOfProjects = table.Rows.Count;
            XmlElement dep = doc.CreateElement("department") as XmlElement;
            dep.SetAttributeNode("depName", "");
            dep.SetAttribute("depName", "all");
            dep.SetAttributeNode("numberOfProjects", "");
            dep.SetAttribute("numberOfProjects", totalNumberOfProjects.ToString());
            elm = elm.AppendChild(dep) as XmlElement;
            // all departments 
            System.Text.StringBuilder pridlist = new System.Text.StringBuilder();

            foreach (DataRow row in table.Rows)
            {
                XmlElement sequence = _projectReportStructure.createDataToXML().extractData(doc, ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L));
                elm.AppendChild(sequence);
                //fill in the project id list (used next)
                pridlist.Append("," + row[0].ToString());
            }
            if (pridlist.Length > 0)
            {
                pridlist.Remove(0, 1);
            }
            else
            {
                return;
            }
            //any department
    
            // 2. OTHER departments        
            string depsql = "select ID,TITLE_DE,PARENT_ID,ROOT_ID from REGISTRY where ID in (select REGISTRY_ID from REGISTRY_ENTRY where OBJECT_UID in (select UID from PROJECT where ID in(" +
                    pridlist.ToString() + ") AND TEMPLATE=0";
            if (DO_NOT_INCLUDE_SUBPROJECTS)
            {
                depsql = depsql + " AND PARENT_ID is null";
            }
            depsql = depsql + "))"; 

            DataTable depTable = _db.getDataTable(depsql);

            DepartmentXmlContainer departmentXmlContainer = new DepartmentXmlContainer(this, doc);
            

            foreach (DataRow department in depTable.Rows)
            {
                //is this a sub department? i.e. department which is child of a non root department
                long departmentId = ch.psoft.Util.Validate.GetValid(department[0].ToString(), -1L);


                //create a new department element 
                XmlElement depElm = doc.CreateElement("department") as XmlElement;
                long rootParentDepartmentId = departmentXmlContainer.requestXmlElementIndex(departmentId, elm, depElm);
                string parentDepName = _db.lookup("TITLE_DE", "REGISTRY", "ID = " + rootParentDepartmentId).ToString();

                //find out projects in the current department
                string projdepSql = "select distinct ID from PROJECT where UID in (select OBJECT_UID from REGISTRY_ENTRY where REGISTRY_ID = " + department[0] + ") AND TEMPLATE=0";
                if (DO_NOT_INCLUDE_SUBPROJECTS)
                {
                    projdepSql = projdepSql + " AND PARENT_ID is null";
                }

                DataTable depPrTable = _db.getDataTable(projdepSql);
                depElm.SetAttributeNode("depName", "");
                ///depElm.SetAttribute("depName", department[1].ToString());
                depElm.SetAttribute("depName", parentDepName);
                depElm.SetAttributeNode("numberOfProjects", "");
                depElm.SetAttribute("numberOfProjects", depPrTable.Rows.Count.ToString());
                foreach (DataRow dprow in depPrTable.Rows)
                {
                    XmlElement sequence = _projectReportStructure.createDataToXML().extractData(doc, ch.psoft.Util.Validate.GetValid(dprow[0].ToString(), -1L));
                    departmentXmlContainer.addSequence(sequence);
                }
            }

        }

        /// <summary>
        /// only the root department and in the first sub-root departments are 
        /// considered for the visualisation of project data
        /// 
        /// root
        ///   |
        ///    -- * Project in this departmens are also displayed in root dep.
        ///       |
        ///        --(**) Projects in this sub dep. goes together with dep *
        ///           |
        ///            -- (***) Projects in this sub dep. goes together with dep *
        ///           |
        ///            -- 
        ///    
        /// </summary>
        private class DepartmentXmlContainer
        {
            public DepartmentXmlContainer(ProjectSummaryCtrl parent, XmlDocument document)
            {
                this.parent = parent;
                doc = document;
            }

            ProjectSummaryCtrl parent;
            XmlDocument doc;
            long index;

            Hashtable elements = new Hashtable();

            private XmlElement createXmlElm(XmlElement elm, XmlElement depElm)
            {
                //create a new department element 
                XmlElement pelm_ = elm.ParentNode as XmlElement;

                XmlElement ppelm = pelm_.AppendChild(depElm) as XmlElement;
                return ppelm;               
            }

            public long requestXmlElementIndex(long id, XmlElement elm, XmlElement depElm)
            {
                long rootParentDepartmentId = parent.findRootParentAt("REGISTRY", id);

                if (elements.ContainsKey(rootParentDepartmentId))
                {
                    //do nothing
                }
                else
                {
                    //create a new department element 
                    XmlElement ppelm = createXmlElm(elm, depElm);
                    elements.Add(rootParentDepartmentId, ppelm);
                }
                index = rootParentDepartmentId;
                return rootParentDepartmentId;
            }

            public void addSequence(XmlElement sequence)
            {
                ((XmlElement)elements[index]).AppendChild(sequence);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="doc"></param>
        /// <param name="elm"></param>
        private void summary(DataTable table, XmlDocument doc, XmlElement elm)
        {
            foreach (DataRow row in table.Rows)
            {
                XmlElement sequence = _projectReportStructure.createDataToXML().extractData(doc, ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L));
                elm.AppendChild(sequence);
            }
        }


   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentTable"></param>
        /// <param name="projectID"></param>
        /// <param name="indentLevel"></param>
        protected void BuildProject(Table parentTable, long projectID, int indentLevel){
            DataTable table = _db.getDataTableExt("select * from PROJECT where ID=" + projectID, "PROJECT");
            if (table.Rows.Count > 0){
                int criticalDays = ch.psoft.Util.Validate.GetValid(DBData.getValue(table, 0, "CRITICALDAYS").ToString(), 1);
                _values[0] = DBData.getValue(table, 0, "TITLE").ToString() + " (" + _db.Project.getOpenPhasesCount(projectID, true) + ")";
                _values[1] = "";
                long[] projectLeaders = _db.Project.getLeaderPersonIDs(projectID);
                foreach (long personID in projectLeaders){
                    if (_values[1].Length > 0){
                        _values[1] += ", ";
                    }
                    _values[1] += _db.Person.getWholeName(personID);
                }
                _values[2] = _db.GetDisplayValue(table.Columns["STARTDATE"], DBData.getValue(table, 0, "STARTDATE"), false);
                _values[3] = _db.GetDisplayValue(table.Columns["DUEDATE"], DBData.getValue(table, 0, "DUEDATE"), false);
                _values[4] = DBData.getValue(table, 0, "STATE").ToString().Equals("0")? _statesPr[0].ToString() : _statesPr[1].ToString();
                _values[5] = _db.Project.getSemaphore(projectID, true).ToString();
                BuildEntry(parentTable, "Pr", projectID.ToString(), indentLevel, _values);
                table = _db.getDataTable("select distinct ID from PROJECT" + _db.getAccessRightsRowInnerJoinSQL("PROJECT", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " where PARENT_ID=" + projectID);
                Table childTable = null;
                foreach (DataRow row in table.Rows){
                    if (childTable == null){
                        childTable = BuildChildTable(parentTable, "Pr", projectID.ToString(), indentLevel);
                    }
                    BuildProject(childTable, ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), indentLevel+1);
                }
                table = _db.getDataTableExt("select distinct ID, TITLE, LEADER_PERSON_ID, STARTDATE, DUEDATE, STATE from PHASE" + _db.getAccessRightsRowInnerJoinSQL("PHASE", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " where PROJECT_ID=" + projectID, "PHASE");
                for (int i=0; i<table.Rows.Count; i++){
                    if (childTable == null){
                        childTable = BuildChildTable(parentTable, "Pr", projectID.ToString(), indentLevel);
                    }
                    long phaseID = ch.psoft.Util.Validate.GetValid(DBData.getValue(table, i, "ID").ToString(), -1L);
                    _values[0] = DBData.getValue(table, i, "TITLE").ToString();
                    _values[1] = _db.Person.getWholeName(DBColumn.GetValid(DBData.getValue(table, i, "LEADER_PERSON_ID"), -1L));
                    _values[2] = _db.GetDisplayValue(table.Columns["STARTDATE"], DBData.getValue(table, i, "STARTDATE"), false);
                    _values[3] = _db.GetDisplayValue(table.Columns["DUEDATE"], DBData.getValue(table, i, "DUEDATE"), false);
                    _values[4] = DBData.getValue(table, i, "STATE").ToString().Equals("0")? _statesPh[0].ToString() : _statesPh[1].ToString();
                    _values[5] = _db.Phase.getSemaphore(phaseID, criticalDays).ToString();
                    BuildEntry(childTable, "Ph", phaseID.ToString(), indentLevel+1, _values);
                }
            }
        }

        protected override void onAddCell(TableRow r, TableCell cell, int columnIndex, int indentLevel) {
            if (columnIndex == 5) {
                System.Web.UI.WebControls.Image im = new System.Web.UI.WebControls.Image();
                switch(cell.Text) {
                    case "0":
                        im.ImageUrl = "../../images/ampelRot.gif";
                        break;
                    case "1":
                        im.ImageUrl = "../../images/ampelOrange.gif";
                        break;
                    case "2":
                        im.ImageUrl = "../../images/ampelGruen.gif";
                        break;
                    case "3":
                        im.ImageUrl = "../../images/ampelGrau.gif";
                        break;
					case "4":
						im.ImageUrl = "../../images/ampelBlau.gif";
						break;
                }
				string id = r.ID.ToString();
				long xId = long.Parse(id.Substring(5));
				if (id.StartsWith("rowPr"))
				{	
					im.ToolTip = ProjectModule.getSemaphoreProjectComment(Session, int.Parse(cell.Text), _db.Project.getCriticalDays(xId));
				}
				else if (id.StartsWith("rowPh"))
				{
					im.ToolTip = ProjectModule.getSemaphorePhaseComment(Session, int.Parse(cell.Text), _db.Project.getCriticalDays(ch.psoft.Util.Validate.GetValid(_db.lookup("PROJECT_ID", "PHASE", "ID="+xId,false), -1L)));
				}

                cell.Text = "";
                cell.Controls.Add(im);
            }
        }


     
        /// <summary>
        /// Go down up to the given level intended from the root.
        /// </summary>
        /// <param name="levelFromRoot"></param>
        /// <param name="departmentId"></param>
        /// <returns>id of the first parent node after the root node, -1 if the given department id is the root node</returns>
        private long findRootParentAt(string tablename, long departmentId)
        {
            long currentId = departmentId;
            //System.Collections.Stack idstack = new System.Collections.Stack();

            int watchdog = 0; //avoid endless loop
            while (watchdog < 20)
            {
                object[] objs = _db.lookup(new string[] { "PARENT_ID", "ROOT_ID" }, tablename, "ID = " + currentId);
                if (objs[0] == null)
                {
                    //no parent id, this is the root node
                    return -1;
                }
                long parentId = (long)objs[0];
                long rootId = (long)objs[1];
                if (parentId == rootId)
                {
                    return currentId;
                }
                currentId = parentId;
                watchdog++;
            }

            return -1;
        }
        
        /// <summary>
        /// Currently not used or deprecated.
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private long[] findChilds(string tablename, long parentId)
        {
            if (parentId == -1) return null;
            string sql = "select ID, PARENT_ID from " + tablename + " where PARENT_ID = " + parentId;
            DataTable data = _db.getDataTable(sql);
            long[] ids = new long[data.Rows.Count];
            for (int k = 0; k < ids.Length; k++)
            {
                ids[k] = (long)data.Rows[k][0];
            }
            
            long[] rec = null;
            ArrayList retlist = new ArrayList();
            int next = 0;
            foreach (DataRow row in data.Rows)
            {
                rec = findChilds(tablename, (long)row[0]);
                foreach (long elm in rec)
                {
                    retlist.Add(elm);
                }
            }
            rec = new long[ retlist.Count ];
            retlist.CopyTo(rec);
            if (rec == null || rec.Length == 0) return ids;

            long[] dest = new long[ids.Length + rec.Length];
            System.Array.Copy(ids, dest, ids.Length);
            System.Array.Copy(rec, 0, dest, ids.Length, rec.Length);
            return dest;
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
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}

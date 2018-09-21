namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class ProjectDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";

        private DBData _db = null;
        private DataTable _table;
        private string _postDeleteURL;


        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/ProjectDetailCtrl.ascx";}
        }

		#region Properties
        public long ProjectID {
            get {return GetLong(PARAM_PROJECT_ID);}
            set {SetParam(PARAM_PROJECT_ID, value);}
        }

        public string PostDeleteURL {
            get {return _postDeleteURL;}
            set {_postDeleteURL = value;}
        }

		protected string deleteMessage 
		{
			get { return _mapper.get("project","deleteProjectConfirm"); }
		}
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            _db = DBData.getDBData(Session);
            _db.connect();
            try {
                DataTable parentTable = _db.getDataTable("select ID, TITLE from PROJECT");

                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select * from PROJECT where ID=" + ProjectID, "PROJECT");				
				_table.Columns["PARENT_ID"].ExtendedProperties["In"] = parentTable;
                _table.Columns["TEMPLATE_PROJECT_ID"].ExtendedProperties["In"] = _db.Project.getTemplatesTable();
                _table.Columns["STATE"].ExtendedProperties["In"] = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PROJECT);
				_table.Columns["PROJECT_TYPE_ID"].ExtendedProperties["In"] = _db.Project.getProjectTypes();
				_table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
				_table.Columns["SPEC_PROBLEM"].ExtendedProperties["Views"] = "PROJECT";
				_table.Columns["SPEC_COMMENT"].ExtendedProperties["Views"] = "PROJECT";
				_table.Columns["SPEC_MODIFY_DATE"].ExtendedProperties["Views"] = "PROJECT";

                if (Global.Config.getModuleParam("project", "enableMainObjectiveField", "0").Equals("1"))
                { 
                    //enabled
                    _table.Columns["IS_MAIN_OBJECTIVE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                    _table.Columns["IS_MAIN_OBJECTIVE"].ExtendedProperties["In"] = _mapper.getEnum("project", "isMainObjective");
                }

                CheckOrder = true;
                LoadDetail(_db, _table, detailTab);

                long[] personIDs = _db.Project.getCommiteePersonIDs(ProjectID);
                if (personIDs.Length > 0){
                    detailTab.Rows.Add(buildPersonlistRow(_db, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_COMMITEE), personIDs));
                }
                personIDs = _db.Project.getLeaderPersonIDs(ProjectID);
                if (personIDs.Length > 0){
                    detailTab.Rows.Add(buildPersonlistRow(_db, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_LEADERS), personIDs));
                }
                personIDs = _db.Project.getMemberPersonIDs(ProjectID);
                if (personIDs.Length > 0){
                    detailTab.Rows.Add(buildPersonlistRow(_db, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_MEMBERS), personIDs));
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        public static TableRow buildPersonlistRow(DBData db, string labelText, long[] personIDs){
            TableRow row = new TableRow();
            row.CssClass = "Detail";
            row.VerticalAlign = VerticalAlign.Top;

            TableCell labelCell = new TableCell();
            row.Cells.Add(labelCell);
            labelCell.CssClass = "Detail_Label";
            labelCell.Text = labelText;
            
            TableCell valueCell = new TableCell();
            row.Cells.Add(valueCell);
            valueCell.CssClass = "Detail_Value";

            bool isFirst = true;
            foreach(long personID in personIDs){
                if (isFirst){
                    isFirst = false;
                }
                else{
                    Label label = new Label();
                    valueCell.Controls.Add(label);
                    label.CssClass = "Detail_Value";
                    label.Text = ", ";
                }
                HyperLink link = new HyperLink();
                valueCell.Controls.Add(link);
                link.CssClass = "Detail_Value";
                link.Text = db.Person.getWholeName(personID);
                link.NavigateUrl = psoft.Person.DetailFrame.GetURL("ID",personID, "mode","oe");
            }
            return row;
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

namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class PhaseList : PSOFTListViewUserControl
    {
        private long _projectID = -1;
        private long _phaseID = -1;
        private string _postDeleteURL;

        protected DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        protected string _redComment;
        protected string _orangeComment;
        protected string _greenComment;
        protected string _doneComment;
        protected ArrayList _states;
        protected int _criticalDays = 1;

        public static string Path
        {
            get {return Global.Config.baseURL + "/Project/Controls/PhaseList.ascx";}
        }

        public PhaseList() : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            DetailEnabled = true;
            EditEnabled = true;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
        }

		#region Properities
        public long ProjectID {
            get {return _projectID;}
            set {_projectID = value;}
        }

        public long PhaseID {
            get {return _phaseID;}
            set {_phaseID = value;}
        }

        public string PostDeleteURL {
            get {return _postDeleteURL;}
            set {_postDeleteURL = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            loadList();
        }

        protected void loadList(){
            _db = DBData.getDBData(Session);
            _states = ProjectModule.getStates(_mapper,ProjectModule.LANG_ENUM_STATE_PHASE);
            listTab.Rows.Clear();
            try 
            {
                _db.connect();
                pageTitle.Text = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CT_PHASELIST).Replace("#1", _db.lookup("TITLE", "PROJECT", "ID=" + ProjectID, false));
                _criticalDays = _db.Project.getCriticalDays(ProjectID);

                _redComment = ProjectModule.getSemaphorePhaseComment(Session, 0, _criticalDays);
                _orangeComment = ProjectModule.getSemaphorePhaseComment(Session, 1, _criticalDays);
                _greenComment = ProjectModule.getSemaphorePhaseComment(Session, 2, _criticalDays);
                _doneComment = ProjectModule.getSemaphorePhaseComment(Session, 3, _criticalDays);

                DataTable table = _db.getDataTableExt("select * from PHASE where PROJECT_ID=" + ProjectID + " order by " + OrderColumn + " " + OrderDir, "PHASE");
                IDColumn = "ID";
                if (_phaseID > 0)
                    HighlightRecordID = _phaseID;

                table.Columns["LEADER_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                table.Columns["LEADER_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%LEADER_PERSON_ID", "mode","oe");
                table.Columns["STATE"].ExtendedProperties["In"] = _states;

                LoadList(_db, table, listTab);
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (col != null) {
                switch(col.ColumnName) {
                    case "STATE":
                        // create dropdown
                        cell.Controls.Clear();
                        DropDownList dd = new DropDownCtrl();
                        cell.Controls.Add(dd);
                        dd.Items.Add(new ListItem(_states[0] as string, "0"));
                        dd.Items.Add(new ListItem(_states[1] as string, "1"));
                        dd.SelectedIndex = int.Parse(row[col].ToString());
                        dd.ID = "DDSTATE" + row["ID"].ToString();
                        dd.SelectedIndexChanged += new EventHandler(ddState_SelectedIndexChanged);
                        dd.AutoPostBack = true;

                        // append semaphore...
                        long ID = DBColumn.GetValid(row["ID"],0L);
                        TableCell c = new TableCell();
                        r.Cells.Add(c);
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        switch (_db.Phase.getSemaphore(ID, _criticalDays)) {
                            case 0:
                                image.ImageUrl = "../../images/ampelRot.gif";
                                image.ToolTip = _redComment;
                                break;
                            case 1:
                                image.ImageUrl = "../../images/ampelOrange.gif";
                                image.ToolTip = _orangeComment;
                                break;
                            case 2:
                                image.ImageUrl = "../../images/ampelGruen.gif";
                                image.ToolTip = _greenComment;
                                break;
                            case 3:
                                image.ImageUrl = "../../images/ampelGrau.gif";
                                image.ToolTip = _doneComment;
                                break;
                        }

                        c.Controls.Add(image);
                        break;
                }
            }
            else if (cell.Text.StartsWith("<img")) {
                string id = ListBuilder.getID(r);
                cell.Text = "<img id=\""+id+"\" ondragstart=\"listDragStart('Phase')\" ondragend=\"listDragEnd()\""+cell.Text.Substring(4);
            }
        }

        private void ddState_SelectedIndexChanged(object sender, System.EventArgs e) {
            if (sender is DropDownList) {
                DropDownList dd = (sender as DropDownList);
                try {
                    int phaseID = int.Parse(dd.ID.Substring(7));
                    _db.connect();
                    _db.execute("update PHASE set STATE=" + dd.SelectedItem.Value + " where ID=" + phaseID);
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    _db.disconnect();
                }
                loadList();
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

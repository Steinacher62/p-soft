namespace ch.appl.psoft.Project.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class TeamMemberEditCtrl : PSOFTInputViewUserControl {
        public const string PARAM_PROJECT_ID = "PARAM_PROJECT_ID";
        public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";

        protected DataTable _table;
        protected DBData _db = null;


        protected DropDownCtrl _ddPerson = null;
        protected DropDownCtrl _ddFunction = null;
        protected DropDownCtrl _ddOE = null;

        protected long _functionID = -1L;
        protected long _oeID = -1L;
        protected long _personID = -1L;
        protected string _oe = "members";
        protected long _commiteeOEID = -1L;
        protected long _leaderOEID = -1L;

        public static string Path {
            get {return Global.Config.baseURL + "/Project/Controls/TeamMemberEditCtrl.ascx";}
        }

		#region Properities
        public long ProjectID {
            get {return GetLong(PARAM_PROJECT_ID);}
            set {SetParam(PARAM_PROJECT_ID, value);}
        }

        public long JobID {
            get {return GetLong(PARAM_JOB_ID);}
            set {SetParam(PARAM_JOB_ID, value);}
        }

        public string NextURL{
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
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
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                    personLabelCell.Text = _mapper.get("JOB_PERS_FUNC_V", "PERSON_ID");
                    functionLabelCell.Text = _mapper.get("JOB_PERS_FUNC_V", "FUNKTION_ID");
                    oeLabelCell.Text = _mapper.get("JOB_PERS_FUNC_V", "ORGENTITY_ID");
                }

                _commiteeOEID = _db.Project.getCommiteeOrgentityID(ProjectID);
                _leaderOEID = _db.Project.getLeaderOrgentityID(ProjectID);

                _ddOE = new DropDownCtrl();
                _ddOE.Items.Add(new ListItem(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_MEMBERS), "members"));
                _ddOE.Items.Add(new ListItem(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_LEADERS), "leaders"));
                _ddOE.Items.Add(new ListItem(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_COMMITEE), "commitee"));
                oeValueCell.Controls.Add(_ddOE);

                if (InputType == InputMaskBuilder.InputType.Add){
                    _ddPerson = new DropDownCtrl();
                    addDropDownCtrl(_ddPerson, personValueCell, _db.Person.getWholeNameMATable(false), -1L);
                }
                else if (InputType == InputMaskBuilder.InputType.Edit){
                    _personID = ch.psoft.Util.Validate.GetValid(_db.lookup("PERSON_ID", "JOB_PERS_FUNC_V", "ID=" + JobID, false), -1L);
                    personValueCell.Text = _db.Person.getWholeName(_personID);

                    _functionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + JobID, false), -1L);
                    _oeID = ch.psoft.Util.Validate.GetValid(_db.lookup("ORGENTITY_ID", "JOB", "ID=" + JobID, false), -1L);
                    if (_oeID == _commiteeOEID){
                        _oe = "commitee";
                        if (!IsPostBack) {
                            _ddOE.SelectedIndex = 2;
                        }
                    }
                    else if (_oeID == _leaderOEID){
                        int jobTyp = ch.psoft.Util.Validate.GetValid(_db.lookup("TYP", "JOB", "ID=" + JobID, false), 0);
                        if (jobTyp == 0){
                            _oe = "members";
                            if (!IsPostBack) {
                                _ddOE.SelectedIndex = 0;
                            }
                        }
                        else if (jobTyp == 1){
                            _oe = "leaders";
                            if (!IsPostBack) {
                                _ddOE.SelectedIndex = 1;
                            }
                        }
                    }                    
                }

                DataTable table = _db.getDataTable("select ID, " + _db.langAttrName("FUNKTION", "TITLE") + " from FUNKTION order by " + _db.langAttrName("FUNKTION", "TITLE"));
                _ddFunction = new DropDownCtrl();
                addDropDownCtrl(_ddFunction, functionValueCell, table, _functionID);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected void addDropDownCtrl(DropDownCtrl dd, TableCell cell, DataTable table, long selectedID){
            foreach (DataRow row in table.Rows) {
                long id = DBColumn.GetValid(row[0], -1L);
                dd.Items.Add(new ListItem(row[1].ToString(),id.ToString()));
                if (id == selectedID){
                    dd.SelectedIndex = dd.Items.Count - 1;
                }
            }
            cell.Controls.Add(dd);
        }

        private void mapControls () {
            this.apply.Click += new System.EventHandler(apply_Click);
        }

        private void apply_Click(object sender, System.EventArgs e) {
            _db.connect();
            try {
                _db.beginTransaction();

                if (InputType == InputMaskBuilder.InputType.Edit){
                    switch (_oe){
                        case "members":
                            _db.Project.removeMemberPerson(ProjectID, _personID);
                            break;

                        case "leaders":
                            _db.Project.removeLeaderPerson(ProjectID, _personID);
                            break;
                                
                        case "commitee":
                            _db.Project.removeCommiteePerson(ProjectID, _personID);
                            break;
                    }
                }
                else if (InputType == InputMaskBuilder.InputType.Add){
                    _personID = ch.psoft.Util.Validate.GetValid(_ddPerson.SelectedItem.Value, -1L);
                }

                long functionID = ch.psoft.Util.Validate.GetValid(_ddFunction.SelectedItem.Value, -1L);
                switch (_ddOE.SelectedItem.Value){
                    case "members":
                        _db.Project.addMemberPerson(ProjectID, _personID, functionID);
                        break;

                    case "leaders":
                        _db.Project.addLeaderPerson(ProjectID, _personID, functionID);
                        break;
                    
                    case "commitee":
                        _db.Project.addCommiteePerson(ProjectID, _personID, functionID);
                        break;
                }

                _db.commit();

                if (NextURL != ""){
                    Response.Redirect(NextURL, false);
                }
                else{
                    Response.Redirect(psoft.Project.ManageTeam.GetURL("projectID",ProjectID), false);
                }
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }

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
		
        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}

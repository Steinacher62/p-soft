namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial class SkillLevelValidityList : PSOFTListViewUserControl {
        private long _skillID = -1;
        private long _jobID = -1;
        private long _personID = -1;
        private long _skillLevelValidityID = -1;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path {
            get {return Global.Config.baseURL + "/Skills/Controls/SkillLevelValidityList.ascx";}
        }

        public SkillLevelValidityList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            OrderColumn = "VALID_FROM";
            OrderDir = "asc";
        }

		#region Properities
        public long SkillID {
            get {return _skillID;}
            set {_skillID = value;}
        }

        public long JobID {
            get {return _jobID;}
            set {_jobID = value;}
        }

        public long PersonID {
            get {return _personID;}
            set {_personID = value;}
        }

        public long SkillLevelValidityID {
            get {return _skillLevelValidityID;}
            set {_skillLevelValidityID = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();

            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                pageTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CT_SLV_LIST).Replace("#1", db.lookup("TITLE", "SKILLV", "ID=" + SkillID, false).ToString());

                string sql = "select * from SKILL_LEVEL_VALIDITY where SKILL_ID=" + _skillID;
                if (_jobID > 0){
                    long funktionID = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID=" + _jobID, false), -1);
                    sql += " and (FUNKTION_ID=" + funktionID + " or JOB_ID=" + _jobID + ")";
                }
                else if (_personID > 0){
                    sql += " and PERSON_ID=" + _personID;
                }
                sql += " order by " + OrderColumn + " " + OrderDir;
                DataTable table = db.getDataTableExt(sql, "SKILL_LEVEL_VALIDITY");

                if (_skillLevelValidityID > 0) {
                    HighlightRecordID = _skillLevelValidityID;
                    LoadList(db, table, listTab);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) {
            if (ListBuilder.IsDeleteCell(cell) && ch.psoft.Util.Validate.GetValid(row["FUNKTION_ID"].ToString(), -1) > 0) {
                r.Cells.Remove(cell);
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

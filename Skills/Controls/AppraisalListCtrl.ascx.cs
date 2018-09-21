namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;

    public partial class AppraisalListCtrl : PSOFTListViewUserControl {
        private long _appraisalID = -1;
        private long _personID = -1;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path {
            get {return Global.Config.baseURL + "/Skills/Controls/AppraisalListCtrl.ascx";}
        }

        public AppraisalListCtrl() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            OrderColumn = "APPRAISALDATE";
            OrderDir = "desc";
        }

		#region Properities
        public long AppraisalID {
            get {return _appraisalID;}
            set {_appraisalID = value;}
        }

        public long PersonID {
            get {return _personID;}
            set {_personID = value;}
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
                pageTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CT_APPRAISAL_LIST);

                string sql = "select * from SKILLS_APPRAISAL where PERSON_ID=" + _personID;
                sql += " order by " + OrderColumn + " " + OrderDir;
                DataTable table = db.getDataTableExt(sql, "SKILLS_APPRAISAL");

                if (_appraisalID > 0) {
                    HighlightRecordID = _appraisalID;
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

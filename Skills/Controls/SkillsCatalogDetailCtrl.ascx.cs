namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for SkillsCatalogDetailCtrl.
    /// </summary>
    public partial class SkillsCatalogDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_SKILLVALIDITY_ID = "PARAM_SKILLVALIDITY_ID";
        

        protected Config _config = null;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Skills/Controls/SkillsCatalogDetailCtrl.ascx";}
        }

		#region Properities
        public long SkillValidityID {
            get {return GetLong(PARAM_SKILLVALIDITY_ID);}
            set {SetParam(PARAM_SKILLVALIDITY_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _config = Global.Config;
            
            _db = DBData.getDBData(Session);
            
            try {
                _db.connect();

                if (!IsPostBack) {
                    skillsCatalogDetailTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CT_SKILLSCATALOG_S_DETAIL);
                }

                DataTable table = null;
                skillsCatalogDetail.Rows.Clear();
                if (SkillValidityID > 0) {
                    table = _db.getDataTableExt("select * from SKILL_VALIDITY_V where ID=" + SkillValidityID + " and (VALID_FROM<=GetDate() and (VALID_TO>=GetDate() or VALID_TO is null))", "SKILL_VALIDITY_V");
                    base.LoadDetail(_db, table, skillsCatalogDetail);
                }
            }
            catch (Exception ex) {
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

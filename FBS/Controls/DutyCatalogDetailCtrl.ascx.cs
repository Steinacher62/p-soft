namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for DutyCatalogDetailCtrl.
    /// </summary>
    public partial class DutyCatalogDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_DUTYVALIDITY_ID = "PARAM_DUTYVALIDITY_ID";
        

        protected Config _config = null;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/FBS/Controls/DutyCatalogDetailCtrl.ascx";}
        }

		#region Properities
        public long DutyValidityID {
            get {return GetLong(PARAM_DUTYVALIDITY_ID);}
            set {SetParam(PARAM_DUTYVALIDITY_ID, value);}
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
                    dutyCatalogDetailTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CT_DUTYCATALOG_D_DETAIL);
                }

                DataTable table = null;
                dutyCatalogDetail.Rows.Clear();
                if (DutyValidityID > 0) {
                    table = _db.getDataTableExt("select * from DUTY_VALIDITY_V where ID=" + DutyValidityID + " and (VALID_FROM<=GetDate() and (VALID_TO>=GetDate() or VALID_TO is null))", "DUTY_VALIDITY_V");
                    base.LoadDetail(_db, table, dutyCatalogDetail);
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

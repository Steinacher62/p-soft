namespace ch.appl.psoft.Training.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for TrainingCatalogDetailCtrl.
    /// </summary>
    public partial class TrainingCatalogDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_TRAINING_ID = "PARAM_TRAINING_ID";
        

        protected Config _config = null;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Training/Controls/TrainingCatalogDetailCtrl.ascx";}
        }

		#region Properities
        public long TrainingID 
        {
            get {return GetLong(PARAM_TRAINING_ID);}
            set {SetParam(PARAM_TRAINING_ID, value);}
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

                if (!IsPostBack)
                {
                    trainingCatalogDetailTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING,TrainingModule.LANG_MNEMO_CT_TRAININGCATALOG_T_DETAIL);
                }

                DataTable table = null;
                trainingCatalogDetail.Rows.Clear();
                if (TrainingID > 0)
                {
                    table = _db.getDataTableExt("select * from TRAINING where ID=" + TrainingID,"TRAINING");                            
                    base.LoadDetail(_db, table, trainingCatalogDetail);
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

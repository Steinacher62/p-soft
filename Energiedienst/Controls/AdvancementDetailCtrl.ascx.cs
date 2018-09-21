namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Training;
    using db;
    using Interface;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for AdvancementDetailCtrl.
    /// </summary>
    public partial class AdvancementDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_ADVANCEMENT_ID = "PARAM_ADVANCEMENT_ID";
        public const string PARAM_TRAINING_ID = "PARAM_TRAINING_ID";
        

        protected Config _config = null;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Energiedienst/Controls/AdvancementDetailCtrl.ascx";}
        }

		#region Properities
        public long PersonID {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }

        public long AdvancementID 
        {
            get {return GetLong(PARAM_ADVANCEMENT_ID);}
            set {SetParam(PARAM_ADVANCEMENT_ID, value);}
        }
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
                    advancementDetailTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING,TrainingModule.LANG_MNEMO_CT_ADVANCEMENT_DETAIL);
                }
                if (PersonID > 0)
                {
                    DataTable table = null;
                    advancementDetail.Rows.Clear();
                    if (AdvancementID > 0)
                    {
                        table = _db.getDataTableExt("select * from TRAININGADVANCEMENTTRAININGV where ID=" + AdvancementID,"TRAININGADVANCEMENTTRAININGV");                            
                        table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                        table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%RESPONSIBLE_PERSON_ID", "mode","oe");
                        table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = TrainingModule.getTrainingDemandTable(_db);
                        if (TrainingID <= 0)
                        {
                            table.Columns["VALID_FROM"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            table.Columns["PARTICIPANT_NUMBER"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        }
                        base.LoadDetail(_db, table, advancementDetail);
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

        public bool hasAuthorisation(DBData db, int authorisation) {
            bool ret = false;
            DataTable jobTable = db.getDataTable("select ID from JOBPERSONV where PERSON_ID=" + PersonID);
            foreach (DataRow row in jobTable.Rows){
                if (db.hasRowAuthorisation(authorisation, "JOB", ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L), DBData.APPLICATION_RIGHT.TRAINING, true, true))
                    ret = true;
            }
            return ret;
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

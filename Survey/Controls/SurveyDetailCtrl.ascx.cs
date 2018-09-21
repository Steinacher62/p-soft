namespace ch.appl.psoft.Survey.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Collections;
    using System.Data;


    public partial class SurveyDetailCtrl : PSOFTDetailViewUserControl {
        public const string PARAM_SURVEY_ID = "PARAM_SURVEY_ID";

        private DBData _db = null;
        private DataTable _table;


        public static string Path {
            get {return Global.Config.baseURL + "/Survey/Controls/SurveyDetailCtrl.ascx";}
        }

		#region Properities
        public long SurveyID {
            get {return GetLong(PARAM_SURVEY_ID);}
            set {SetParam(PARAM_SURVEY_ID, value);}
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
                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select * from SURVEY where ID=" + SurveyID, "SURVEY");
                _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);;
                _table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                ArrayList booleans = SurveyModule.getBooleans(_mapper);
                _table.Columns["ISANONYMOUS"].ExtendedProperties["In"] = booleans;
                _table.Columns["ISCORRECTABLE"].ExtendedProperties["In"] = booleans;
                _table.Columns["ISPUBLIC"].ExtendedProperties["In"] = booleans;

                LoadDetail(_db, _table, detailTab);
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

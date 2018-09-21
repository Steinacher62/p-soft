namespace ch.appl.psoft.Contact.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    public partial class JournalDetailCtrl : PSOFTMapperUserControl {
        public const string PARAM_ID = "PARAM_ID";
        public const string PARAM_CONTACTID = "PARAM_ID";

        private DetailBuilder _detailBuilder;
        private DataTable _table;
        private DBData _db;


        public static string Path {
            get {return Global.Config.baseURL + "/Contact/Controls/JournalDetailCtrl.ascx";}
        }

		#region Properities
        public long JournalID {
            get {return GetLong(PARAM_ID);}
            set {SetParam(PARAM_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _detailBuilder = new DetailBuilder();
            
            /*
			//cst, 090713: commented out, unused if-block
			if (!IsPostBack) {
            }
			*/

            _db = DBData.getDBData(Session);
            try {
                _db.connect();

                _table = _db.getDataTableExt("select * from JOURNAL where ID=" + JournalID, "JOURNAL");
                
                _table.Columns["JOURNAL_TYPE_ID"].ExtendedProperties["In"] =  ContactModule.getJournaltTypes(_db);
                _table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                _table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%CREATOR_PERSON_ID", "mode","oe");
                
                _detailBuilder.load(_db, _table, journalTab, _mapper);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        public string getTitle() {
            _db = DBData.getDBData(Session);
            try {
                _db.connect();
                return _db.lookup("TITLE", "JOURNAL", "ID=" + JournalID, false);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

            return "";
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

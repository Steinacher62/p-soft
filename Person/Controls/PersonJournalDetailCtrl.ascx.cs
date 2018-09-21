namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;

    public partial class PersonJournalDetailCtrl : PSOFTMapperUserControl {

		private DetailBuilder _detailBuilder;
        private DataTable _table;
        private DBData _db;


        public static string Path {
            get {return Global.Config.baseURL + "/Person/Controls/PersonJournalDetailCtrl.ascx";}
        }

		#region Properities
		public long _contextID {
			get {return GetLong("contextID");}
			set {SetParam("contextID", value);}
		}

		public long _journalID {
			get {return GetLong("journalID");}
			set {SetParam("journalID", value);}
		}
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

			_detailBuilder = new DetailBuilder();
            _db = DBData.getDBData(Session);

			try {
                _db.connect();

                _table = _db.getDataTableExt("select * from PERSON_JOURNAL where ID=" + _journalID, "PERSON_JOURNAL");
                
				DataTable journalTypes = _db.getDataTable("select ID," + _db.langAttrName("PERSON_JOURNAL_TYPE", "TITLE") + " from PERSON_JOURNAL_TYPE order by " + _db.langAttrName("PERSON_JOURNAL_TYPE", "TITLE"));
                _table.Columns["PERSON_JOURNAL_TYPE_ID"].ExtendedProperties["In"] =  journalTypes;
                _table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                _table.Columns["CREATOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%CREATOR_PERSON_ID", "mode","oe");
				_table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
				_table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                
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
                return _db.lookup("TITLE", "PERSON_JOURNAL", "ID=" + _journalID, false);
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

namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenQuelleDetail.
    /// </summary>
    public partial class PendenzenTaskDetail : PSOFTDetailViewUserControl {

        protected long _ID = 0;

        private DBData _db = null;
        private DataTable _table;
        private bool _template;


        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenTaskDetail.ascx";}
        }

		#region Properities
        public long TasklistID {
            get {return _ID;}
            set {_ID = value;}
        }
        
        public bool Template 
        {
            get {return _template;}
            set {_template = value;}
        }

        protected string deleteMessage {
            get { return _mapper.get("tasklist","deleteTasklistConfirm"); }
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            loadList();
        }

        private void loadList() {
            _db = DBData.getDBData(Session);

            _db.connect();
            try {
                DataTable personTable = _db.Person.getWholeNameMATable(true);
                DataTable parentTable = _db.getDataTable("select id,title from TASKLIST");

                // load details of tasklist
                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select * from TASKLIST where ID=" + _ID, "TASKLIST");
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%AUTHOR_PERSON_ID", "mode","oe");
                _table.Columns["PARENT_ID"].ExtendedProperties["In"] = parentTable;
                _table.Columns["CLIPBOARD_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (Template)
                {
                    _table.Columns["CREATIONDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                else
                {
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.DETAIL;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("boolean", true));
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["In"] = _db.Tasklist.getTemplatesTable();
                }

                View = "TASKLIST_DETAIL";
                base.LoadDetail(_db, _table, detailTab);
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

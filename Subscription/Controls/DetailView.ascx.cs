namespace ch.appl.psoft.Subscription.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class DetailView : PSOFTDetailViewUserControl {
        protected long _id = 0;
        protected string _backURL = "";
        private DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Subscription/Controls/DetailView.ascx";}
        }

		#region Properities

        /// <summary>
        /// Get/Set current id
        /// </summary>
        public long id {
            get {return _id;}
            set {
                _id = value;
            }
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
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
                DataTable table = _db.getDataTableExt("select * from SUBSCRIPTION where ID=" + _id,"SUBSCRIPTION");
                table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                table.Columns["ACTIVE"].ExtendedProperties["DetailControlType"] = typeof(CheckBox);
                table.Columns["EMAILENABLE"].ExtendedProperties["DetailControlType"] = typeof(CheckBox);
                table.Columns["TRIGGER_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                detailTab.Rows.Clear();
                base.LoadDetail(_db, table, detailTab);

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
            if (col != null && col.ColumnName == "EVENTS") {
                TableCell c = r.Cells[1];
                int ev = ch.psoft.Util.Validate.GetValid(c.Text,0);
                
                c.Text = "";
                if (((int) News.ACTION.DELETE & ev) > 0) c.Text = _mapper.get("delete");
                if (((int) News.ACTION.EDIT & ev) > 0) {
                    if (c.Text != "") c.Text += ", ";
                    c.Text += _mapper.get("edit");
                }
                if (((int) News.ACTION.NEW & ev) > 0) {
                    if (c.Text != "") c.Text += ", ";
                    c.Text += _mapper.get("add");
                }
            }
            else if (col != null && col.ColumnName == "TRIGGER_ID") {
                TableCell c = r.Cells[1];
                long id = ch.psoft.Util.Validate.GetValid(c.Text,0L);
                
                c.Text = _mapper.get("news",_db.News.getTriggerName(row));
                c.Text += ": ";

				if(_db.News.getTriggerName(row) == "KNOWLEDGE")
				{					
					c.Text += _db.Knowledge.getTitle((long) row["TRIGGER_ID"]);
				}
				else
				{
					c.Text += _db.News.getTriggerValue(row);
				}
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

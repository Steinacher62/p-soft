namespace ch.appl.psoft.SPZ.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for ListView.
    /// </summary>
    public partial  class TraceView : PSOFTListViewUserControl {
        protected string _backURL = "";
        protected string _context = "";
        protected string _view = "";
        protected long _contextId = 0;
        protected long _id = 0;


        private DBData _db = null;
        private const string Indention = ". . . . . . . . . . . . . . . ";
        private static int MaxIndentLength = Indention.Length;
        private const string Select = "select O.* from OBJECTIVE O inner join SECONDARY_OBJECTIVE S on O.ID = S.SECONDARY_OBJECTIVE_ID where S.OBJECTIVE_ID=";
        private int _indent = 0;

        public static string Path {
            get {return Global.Config.baseURL + "/SPZ/Controls/TraceView.ascx";}
        }

        #region Properities


        /// <summary>
        /// Get/Set context id
        /// </summary>
        public long contextId {
            get {return _contextId;}
            set {_contextId = value;}
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }
        /// <summary>
        /// Get/Set view
        /// </summary>
        public string view {
            get {return _view;}
            set {_view = value;}
        }

        /// <summary>
        /// Get/set root element id
        /// </summary>
        public long id {
            get {return _id;}
            set {_id = value;}
        }
        /// <summary>
        /// Get/set context
        /// </summary>
        public string context {
            get {return _context;}
            set {_context = value;}
        }
        #endregion
    
        protected void Page_Load(object sender, System.EventArgs e) {
            base.Execute();
        }
        protected override void DoExecute() {
            loadList();
        }

        private void loadList() {
            _db = DBData.getDBData(Session);
                
            DetailEnabled = false;
            DeleteEnabled = false;
            EditEnabled = false;
            InfoBoxEnabled = false;
            HeaderEnabled = true;
            SortURL = "";
            _db.connect();
            try {
                DataTable table = _db.getDataTable("select * from OBJECTIVE where ID="+_id);
                if (table.Rows.Count == 0) return;
                if (_db.Objective.hasAuthorisation(DBData.AUTHORISATION.READ) ||
                    (_db.Objective.getCompetence(table.Rows[0]) & 6) > 0) 
                {
                    foreach (DataRow row in table.Rows) {
                        listRow(table,row);
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

        private void listRow(DataTable table, DataRow row) {
            DataTable tab = _db.getDataTable(Select+row["ID"]);

            showRow(table,row);
            foreach (DataRow r in tab.Rows) {
                showRow(tab,r,true);
            }

            tab = _db.getDataTable("select * from OBJECTIVE where PARENT_ID="+row["ID"]);
            _indent += 2;
            foreach (DataRow r in tab.Rows) {
                listRow(tab,r);
            }
            _indent -= 2;
        }
        private void showRow(DataTable table, DataRow row) {
            showRow(table,row,false);
        }
        private void showRow(DataTable table, DataRow row, bool secondary) {
            TableRow r = new TableRow();
            TableCell c = new TableCell();
            Label lbl;

            r.CssClass = listTab.Rows.Count%2 == 1 ? "ListEven" : "ListOdd";
            listTab.Rows.Add(r);
            c.CssClass = "List";
            if (secondary) c.Style.Add("FONT-STYLE","italic");
            r.Cells.Add(c);
            //
            // indent
            lbl = new Label();
            c.Controls.Add(lbl);
            lbl.Text = Indention.Substring(0,Math.Min(_indent,MaxIndentLength));
            //
            // typ
            int typ =  DBColumn.GetValid(row["TYP"],0);
            if (secondary) {
                lbl = new Label();
                lbl.Width = 20;
                c.Controls.Add(lbl);
            }
            else {
                System.Web.UI.WebControls.Image img = new  System.Web.UI.WebControls.Image();
                img.ImageUrl = Global.Config.baseURL + "/images/";
                img.Attributes.Add("align","absmiddle");
                c.Controls.Add(img);
                switch (typ) {
                case Objective.PERSON_TYP:
                    img.ImageUrl += "user.gif";
                    break;
                case Objective.JOB_TYP:
                    img.ImageUrl += "stelle.gif";
                    break;
                case Objective.ORGANISATION_TYP:
                    img.ImageUrl += "organisation.gif";
                    break;
                case Objective.ORGENTITY_TYP:
                    img.ImageUrl += "abteilung.gif";
                    break;
                case Objective.PROJECT_TYP:
                    img.ImageUrl += "projekt.gif";
                    break;
                default:
                    c.Controls.Clear();
                    c.Text = "?";
                    break;
                }
            }
            //
            // title, number
            lbl = newLabel(c);
            lbl.CssClass = "List_Bold";
            if (secondary) lbl.Style.Add("FONT-STYLE","italic");
            lbl.Text = _db.dbColumn.GetDisplayValue(table.Columns["TITLE"],row["TITLE"],true);
            lbl = newLabel(c);
            lbl.Text = _db.dbColumn.GetDisplayValue(table.Columns["NUMBER"],row["NUMBER"],true);
            //
            // owner
            lbl = newLabel(c);
            string name = "p.pname+' '+p.firstname";
            string title = _db.langAttrName("JOB","TITLE");
            object[] objs = null;
            switch (typ) {
            case Objective.ORGENTITY_TYP:
                objs = _db.lookup(new string[] {name,"job."+title,"oe."+title},"person p inner join (jobemploymentv job right join orgentity oe on job.orgentity_id = oe.id) on p.id = job.person_id","job.typ = 1 and oe.id="+row["ORGENTITY_ID"]);
                lbl.Text = DBColumn.GetValid(objs[0],"");
                if (lbl.Text != "") addLabel(c,", ");
                if (addLabel(c,DBColumn.GetValid(objs[1],""))) addLabel(c," ");
                addLabel(c,DBColumn.GetValid(objs[2],""));
                break;
            case Objective.PERSON_TYP:
                lbl.Text = _db.lookup(name,"person p","p.id="+row["PERSON_ID"],true);
                break;
            case Objective.JOB_TYP:
                objs = _db.lookup(new string[] {name,"job."+title,"oe."+title},"person p inner join (jobemploymentv job inner join orgentity oe on job.orgentity_id = oe.id) on p.id = job.person_id","job.id="+row["JOB_ID"]);
                lbl.Text = DBColumn.GetValid(objs[0],"");
                if (lbl.Text != "") addLabel(c,", ");
                if (addLabel(c,DBColumn.GetValid(objs[1],""))) addLabel(c," ");
                addLabel(c,DBColumn.GetValid(objs[2],""));
                break;
            case Objective.PROJECT_TYP:
                //TODO: an neues Projekt-Modell anpassen...
                lbl.Text = _db.lookup(name,"person p inner join project pr on p.id = pr.leader_person_id","pr.id="+row["PROJECT_ID"],true);
                break;
            default:
                break;
            }
            //
            // state
            lbl = newLabel(c);
            lbl.Text = _mapper.getToken("mbo","state",row["STATE"].ToString()); 
        }

        private Label newLabel(TableCell c) {
            Label lbl = new Label();
            Label space = new Label();

            c.Controls.Add(space);
            space.Width = 10;
            c.Controls.Add(lbl);
            return lbl;
        }
        private bool addLabel(TableCell c, string text) {
            Label lbl = new Label();

            lbl.Text = text;
            if (text != "") {
                c.Controls.Add(lbl);
                return true;
            }
            return false;
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

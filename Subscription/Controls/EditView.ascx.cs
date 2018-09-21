namespace ch.appl.psoft.Subscription.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class EditView : PSOFTInputViewUserControl {
        protected long _id = 0;
        protected long _triggerId = 0;
        protected string _triggerName = "";
        protected string _triggerView = "";
        protected string _triggerAttributName = "TITLE";
        protected string _action = "edit";
        protected string _backURL = "";
        protected string _context = News.CONTEXT.SUBSCRIPTION.ToString();
        private DBData _db = null;
        private DataTable _table = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Subscription/Controls/EditView.ascx";}
        }

		#region Properities

        /// <summary>
        /// Get/Set current id
        /// </summary>
        public long id {
            get {return _id;}
            set {_id = value;}
        }

        /// <summary>
        /// Get/Set trigger object id
        /// </summary>
        public long triggerId {
            get {return _triggerId;}
            set {_triggerId = value;}
        }

        /// <summary>
        /// Get/Set trigger object view
        /// </summary>
        public string triggerView {
            get {return _triggerView;}
            set {_triggerView = value;}
        }
        /// <summary>
        /// Get/Set trigger object name
        /// </summary>
        public string triggerName {
            get {return _triggerName;}
            set {_triggerName = value;}
        }

        /// <summary>
        /// Get/Set trigger attribut name
        /// </summary>
        public string triggerAttributName {
            get {return _triggerAttributName;}
            set {_triggerAttributName = value;}
        }

        /// <summary>
        /// Get/Set action (edit,add,copy)
        /// </summary>
        public string action {
            get {return _action;}
            set {_action = value;}
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }
        /// <summary>
        /// Get/set context (objective, person, oe, job)
        /// </summary>
        public string context {
            get {return _context;}
            set {_context = value;}
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            apply.Text = _mapper.get("apply");
            _db = DBData.getDBData(Session);
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();

            _db.connect();
            try {
                _table = _db.getDataTableExt("select * from SUBSCRIPTION where TYP=0 and ID=" + (_action == "add" ? 0 : _id),"SUBSCRIPTION");
                detailTab.Rows.Clear();
                base.InputType = _action == "add" ? InputMaskBuilder.InputType.Add : InputMaskBuilder.InputType.Edit;
                if (_action == "edit") {
                }
                else if (_action == "add") {
                    if (_triggerId <= 0) _triggerId = _id;
                }
                else redirect();
                _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(false);
                _table.Columns["PERSON_ID"].ExtendedProperties["Nullable"] = false;
                _table.Columns["EVENTS"].ExtendedProperties["InputControlType"] = typeof(BitsetMapCtrl);
                _table.Columns["EVENTS"].ExtendedProperties["In"] = new ArrayList(new String[] {_mapper.get("delete"),_mapper.get("edit")});
                _table.Columns["ACTIVE"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
                _table.Columns["EMAILENABLE"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
                if (!Global.isModuleEnabled("dispatch"))
                {
                    switch (_action)
                    {
                        case "add":
                            _table.Columns["EMAILENABLE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["EMAIL"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            break;
                        case "edit":
                            if (_table.Rows.Count > 0 && !DBColumn.IsNull(_table.Rows[0]["EMAILENABLE"]))
                            {
                                if (DBColumn.GetValid(_table.Rows[0]["EMAILENABLE"],0) == 0)
                                {
                                    _table.Columns["EMAILENABLE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                                    _table.Columns["EMAIL"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                                }
                            }
                            break;
                    }
                    
                }
                base.LoadInput(_db, _table, detailTab);
                if (InputType == InputMaskBuilder.InputType.Add){
                    setInputValue(_table, detailTab, "PERSON_ID", _db.userId.ToString());
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        private void redirect() {
            if (_backURL == "") _backURL = psoft.Subscription.Detail.GetURL("view","detail", "context",_context, "id",_id);
            Response.Redirect(_backURL);
        }


        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
            if (col != null) {
                if (col.ColumnName == "EVENTS" && _action == "add") {
                    BitsetCtrl list = (BitsetCtrl) r.Cells[1].Controls[0];
                    foreach (CheckBox item in list.items) {
                        if (item != null) item.Checked = true;
                    }
                }
                else if (col.ColumnName == "ACTIVE" && _action == "add") {
                    CheckBox cb = (CheckBox) r.Cells[1].Controls[0];
                    cb.Checked = true;
                }
                else if (col.ColumnName == "EMAILENABLE" && _action == "edit") 
                {
                    if (!Global.isModuleEnabled("dispatch"))
                    {
                        if (DBColumn.GetValid(row["EMAILENABLE"],0) == 1)
                        {
                            CheckBox cb = (CheckBox) r.Cells[1].Controls[0];
                            cb.Text = _mapper.get("error","dispatchNotAviable");
                            cb.CssClass = "error";
                        }
                    }
                }
            }
        }


        private void applyClick(object sender, System.EventArgs e) {
            if (base.checkInputValue(_table, detailTab)) {
                _db.connect();
                _db.beginTransaction();
                try {
                    StringBuilder sql = base.getSql(_table, detailTab,true);

                    if (_action == "add") {
                        long newId = _db.newId("SUBSCRIPTION");
                        base.extendSql(sql,_table,"ID",newId);
                        base.extendSql(sql,_table,"TRIGGERNAME",_triggerName);
                        base.extendSql(sql,_table,"TRIGGER_ID",_triggerId);
                        base.extendSql(sql,_table,"TRIGGERATTRIBUT",_triggerAttributName);
                        _id = newId;
                    }
                    string s = base.endExtendSql(sql);
                    if (s.Length > 0) {
                        _db.execute(s);
                    }
                    _db.commit();
                    redirect();
                }
                catch (Exception ex) {
                    _db.rollback();
                    DoOnException(ex);
                }
                finally {
                    _db.disconnect();   
                }
            }		
        }

        private void mapControls() {
            apply.Click += new System.EventHandler(this.applyClick);
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            mapControls();
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}

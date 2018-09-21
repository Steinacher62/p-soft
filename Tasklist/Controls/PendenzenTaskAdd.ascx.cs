namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenTaskAdd.
    /// </summary>
    public partial class PendenzenTaskAdd : PSOFTInputViewUserControl {
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_PARENT_TASKLIST = "PARAM_PARENT_TASKLIST";
        public const string PARAM_OWNER_TABLE = "PARAM_OWNER_TABLE";
        public const string PARAM_OWNER_ID = "PARAM_OWNER_ID";
        public const string PARAM_TYP = "PARAM_TYP";
        public const string PARAM_TRIGGER_UID = "PARAM_TRIGGER_UID";
        public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";

        protected DataTable _table;
        protected DropDownList _templateChooser = null;
        protected DBData _db = null;


        public static string Path {
            get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenTaskAdd.ascx";}
        }

		#region Properities
        public string BackUrl {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }

        public string NextUrl {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long ParentTasklist {
            get {return GetLong(PARAM_PARENT_TASKLIST);}
            set {SetParam(PARAM_PARENT_TASKLIST, value);}
        }

        public string OwnerTable {
            get {return GetString(PARAM_OWNER_TABLE);}
            set {SetParam(PARAM_OWNER_TABLE, value);}
        }

        public long OwnerID {
            get {return GetLong(PARAM_OWNER_ID);}
            set {SetParam(PARAM_OWNER_ID, value);}
        }

        public int Typ {
            get {return GetInt(PARAM_TYP);}
            set {SetParam(PARAM_TYP, value);}
        }
        public long TriggerUID 
        {
            get {return GetID(PARAM_TRIGGER_UID);}
            set {SetParam(PARAM_TRIGGER_UID, value);}
        }
        public bool Template 
        {
            get {return GetBool(PARAM_TEMPLATE);}
            set {SetParam(PARAM_TEMPLATE, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);
            string sql = "select * from TASKLIST where id = -1";

            _db.connect();
            try {
                if (!IsPostBack) {
                    apply.Text = _mapper.get("apply");
                }
                _table = _db.getDataTableExt(sql,"TASKLIST");

                if (Template)
                {
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                    _table.Columns["STARTDATE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                    _table.Columns["DUEDATE"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                }
                else
                {
                    DataTable templateTable = _db.Tasklist.getTemplatesTable();
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["In"] = templateTable;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.ADD;
                    _table.Columns["KEEP_OPEN"].ExtendedProperties["InputControlType"] = typeof(CheckBox);
                }

                if (ParentTasklist <= 0) {
                    _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = SQLColumn.Visibility.INVISIBLE;
                }
                else {
                    _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                }

                base.LoadInput(_db,_table,addTab);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
            if (col != null && col.ColumnName == "TEMPLATE_TASKLIST_ID") {
                Control ctrl = r.Cells[1].Controls[0];

                if (ctrl is DropDownList) {
                    _templateChooser = ctrl as DropDownList;
					_templateChooser.EnableViewState = true;
					_templateChooser.AutoPostBack = true;
                    _templateChooser.SelectedIndexChanged += new System.EventHandler(this.templateSelectionChanged);
                }
            }
            else if (col != null && col.ColumnName == "PARENT_ID") {
                Label lbl = (Label) r.Cells[1].Controls[0];
                if (lbl != null) lbl.Text = _db.lookup("title","tasklist","id="+ParentTasklist,true);
            }
            else if (col != null && col.ColumnName == "CRITICALDAYS") {
                TextBox tx = (TextBox) r.Cells[1].Controls[0];
                tx.Text = Global.Config.getModuleParam("tasklist","criticalDays","5");
            }
        }
        private void mapControls () {
            this.apply.Click += new System.EventHandler(apply_Click);
        }


        private void templateSelectionChanged(object sender, System.EventArgs e)
		{
            string id = Validate.GetValid(_templateChooser.SelectedItem.Value, "0");

            _db.connect();
            try {
                string sql = "select * from TASKLIST where ID=" + id;
                DataTable table = _db.getDataTableExt(sql, "TASKLIST");
                
				if (table.Rows.Count == 0)
				{
					foreach (TableRow r in addTab.Rows) 
					{
						TableCell c = r.Cells[1];
						Control ctrl = c.Controls[0];

						if (ctrl is TextBox) 
						{
							string name = ctrl.ID;
							int idx = name.IndexOf("-", 6);

							if (idx >= 0  && name.Substring(idx + 1) == "CRITICALDAYS") 
							{
								((TextBox) ctrl).Text 
									= Global.Config.getModuleParam("tasklist", "criticalDays", "5");
							}
							else
							{
								((TextBox) ctrl).Text = "";
							}
						}
						else if (ctrl is CheckBox) 
						{
							((CheckBox)ctrl).Checked = false;
						}
					}
				}
				else
				{
					foreach (TableRow r in addTab.Rows) 
					{
						try
						{
							TableCell c = r.Cells[1];
							Control ctrl = c.Controls[0];

							if (ctrl is TextBox) {
								string name = ctrl.ID;
								int idx = name.IndexOf("-",6);
								if (idx >= 0) {
									name = name.Substring(idx+1);
									string textValue = _db.dbColumn.GetDisplayValue(table.Columns[name],table.Rows[0][name],false);
									((TextBox) ctrl).Text = textValue;
								}
							}
							else if (ctrl is CheckBox) {
								string name = ctrl.ID;
								int idx = name.IndexOf("-",6);
								if (idx >= 0) {
									name = name.Substring(idx+1);
									bool check = _db.dbColumn.GetDisplayValue(table.Columns[name],table.Rows[0][name],false) == "0" ? false : true;
									((CheckBox) ctrl).Checked = check;
								}
							}
						}
						catch {}
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

        private void apply_Click(object sender, System.EventArgs e) {
            if (!base.checkInputValue(_table,addTab))
                return;
            
            long newID = -1;
            long templateId = 0;

            if (!Template)
            {
                templateId = Validate.GetValid(_templateChooser.SelectedItem.Value, (long)0);
            }

            _db.connect();
            try {
                _db.beginTransaction();
                StringBuilder sb = base.getSql(_table, addTab, true);
                newID = _db.newId(_table.TableName);

                base.extendSql(sb, _table, "ID", newID);
                base.extendSql(sb, _table, "AUTHOR_PERSON_ID", SessionData.getUserID(Session));
                base.extendSql(sb, _table, "TEMPLATE", Template ? "1" : "0");
                if (TriggerUID > 0) base.extendSql(sb, _table, "TRIGGER_UID", TriggerUID);
                if (ParentTasklist <= 0){
                    base.extendSql(sb, _table, "ROOT_ID", newID);
                }
                else {
                    base.extendSql(sb, _table, "PARENT_ID", ParentTasklist);
                    base.extendSql(sb, _table, "ROOT_ID", _db.lookup("ROOT_ID","TASKLIST","ID="+ParentTasklist));
                    Typ = ch.psoft.Util.Validate.GetValid(_db.lookup("TYP","TASKLIST","ID="+ParentTasklist, false), Tasklist.TYPE_PUBLIC);
                }
                base.extendSql(sb, _table, "TYP", Typ);

                string sql = base.endExtendSql(sb);

                if (sql != "")
                {
                    if (Template)
                    {
                        _db.execute(sql);
                    }
                    else
                    {
                        _db.executeProcedure("MODIFYTABLEROW",
                            new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int)),
                            new ParameterCtx("USERID",_db.userId),
                            new ParameterCtx("TABLENAME","TASKLIST"),
                            new ParameterCtx("ROWID",newID),
                            new ParameterCtx("MODIFY",sql,ParameterDirection.Input,typeof(string),true),
                            new ParameterCtx("INHERIT",1)
                        );
                   }

                    if (templateId > 0) newID = _db.Tasklist.copy(templateId, newID, ParentTasklist, -1, true, true, Template, Typ, true);

                    //assign owner...
                    if (OwnerTable != "" && OwnerID > 0){
                        _db.execute("update " + OwnerTable + " set TASKLIST_ID=" + newID + " where ID=" + OwnerID);
                        //copy rights from owner...
                        _db.copyRowAuthorisations(OwnerTable, OwnerID, "TASKLIST", newID);
                    }

                    //grant base rights...
                    _db.Tasklist.refreshAccessRights(newID);

                    _db.commit();

					if (NextUrl == "")
					{
						Response.Redirect(psoft.Tasklist.TaskDetail.GetURL("ID", newID), false);
					}
					else
					{
                        Response.Redirect(NextUrl);
					}
                }
                else
                    _db.rollback();
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }

        }

        private void back_Click(object sender, System.EventArgs e) {
            if (BackUrl != "")
                Response.Redirect(BackUrl, false);
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
		
        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}

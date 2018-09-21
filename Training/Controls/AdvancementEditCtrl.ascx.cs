namespace ch.appl.psoft.Training.Controls
{
    using ch.appl.psoft.Interface;
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for AdvancementEditCtrl.
    /// </summary>
    public partial class AdvancementEditCtrl : PSOFTInputViewUserControl {
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_ADVANCEMENT_ID = "PARAM_ADVANCEMENT_ID";
        public const string PARAM_TRAINING_ID = "PARAM_TRAINING_ID";



        protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;        
        private ArrayList _enablingControls = new ArrayList();
        

        public static string Path {
            get {return Global.Config.baseURL + "/Training/Controls/AdvancementEditCtrl.ascx";}
        }

		#region Properities
        public long PersonID {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }

        public long AdvancementID {
            get {return GetLong(PARAM_ADVANCEMENT_ID);}
            set {SetParam(PARAM_ADVANCEMENT_ID, value);}
        }
        public long TrainingID 
        {
            get {return GetLong(PARAM_TRAINING_ID);}
            set {SetParam(PARAM_TRAINING_ID, value);}
        }

        private Control _treeCtrl = null;
        public Control TreeCtrl
        {
            get { return _treeCtrl;}
            set { _treeCtrl = value;}
        }
        #endregion 

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _config = Global.Config;
            base.CheckOrder = true;
            if (!IsPostBack) {
                apply.Text = _mapper.get("apply");
                advancementDetailTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_CT_ADVANCEMENT_DETAIL);
                apply.Visible = false;

                if (!Global.isModuleEnabled("energiedienst"))
                {
                    CBTreeFlag.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_FLAG_ADVANCEMENT_INHERIT_TREE);
                    CBTreeFlag.Checked = CBTreeFlag.Enabled = showTree();
                    CBTreeFlag.Visible = true;
                }
                else
                {
                    apply.Text = "Speichern";
                    CBTreeFlag.Visible = false;
                    CBTreeFlag.Checked = false;
                    CBTreeFlag.Enabled = false;
                }
            }

            _db = DBData.getDBData(Session);
            try {
                _db.connect();


                if (PersonID > 0) 
                {

                    _table = _db.getDataTableExt("select * from TRAINING_ADVANCEMENT where ID=" + AdvancementID, "TRAINING_ADVANCEMENT");
                    _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(false);
                    _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = TrainingModule.getTrainingDemandTable(_db);
                    _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    
                    View = "TRAINING_ADVANCEMENT";
                    if (AdvancementID > 0) 
                    {
                        InputType = InputMaskBuilder.InputType.Edit;
                        string [] states = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true);
                        if (!Global.isModuleEnabled("energiedienst"))
                        {
                            _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                            _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(states);
                        }
                        else
                        {
                            _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        }
                    }
                    else 
                    {
                        InputType = InputMaskBuilder.InputType.Add;
                        _table.Columns["STATE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }
                    if (Global.isModuleEnabled("energiedienst") && _table.Rows.Count > 0)
                    {
                        if ( _table.Rows[0]["VIEWED_FLAG"] == DBNull.Value)
                            VIEWED_FLAG.Checked = false;
                        else
                        {
                            VIEWED_FLAG.Checked = true;
                            VIEWED_FLAG.Enabled = false;
                        }
                    }
                    else
                    {
                        VIEWED_FLAG.Visible = false;
                    }

                    if ((_table.Rows.Count > 0 && _db.userId == (long)_table.Rows[0]["PERSON_ID"]) && (Global.isModuleEnabled("energiedienst")) && _table.Rows[0]["VIEWED_FLAG"]== DBNull.Value)
                    {
                        VIEWED_FLAG.CheckedChanged += new System.EventHandler(viewed_Changed);
                        VIEWED_FLAG.Enabled = true;
                       // _table.Columns["VIEWED_FLAG"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                
                        
                    }
                    LoadInput(_db, _table, advancementEdit);
                    if (InputType == InputMaskBuilder.InputType.Add){
                        setInputValue(_table, advancementEdit, "RESPONSIBLE_PERSON_ID", _db.userId.ToString());
                    }

                    if (TrainingID > 0)
                    {
                        DataTable table = _db.getDataTable("select * from TRAINING where ID=" + TrainingID);
                
                        foreach (TableRow r in advancementEdit.Rows) 
                        {
                            try 
                            {
                                TableCell c = r.Cells[1];
                                Control ctrl = c.Controls[0];

                                if (ctrl is TextBox) 
                                {
                                    string name = ctrl.ID;
                                    int idx = name.IndexOf("-",6);
                                    if (idx >= 0) 
                                    {
                                        name = name.Substring(idx+1);
                                        string textValue = _db.dbColumn.GetDisplayValue(table.Columns[name],table.Rows[0][name],false);
                                        ((TextBox) ctrl).Text = textValue;
                                    }
                                }
                            }
                            catch {}
                        }

                        
                    }
                    apply.Visible = true;
                }
                
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) 
        {
           ArrayList elems = new ArrayList();
           elems.Capacity = 2;
           elems.Add(col.ColumnName);
           elems.Add(r.Cells[1].Controls[0]);
           _enablingControls.Add(elems);     

           if (!IsPostBack) {
               controlEnabling(elems);
           }
        }

        private void controlEnabling(ArrayList elem)
        {
            bool enable = true;
            String colName = (String) elem[0];
            Control ctrl = (Control) elem[1];
 
            
            
            
            if (isNew()) // addAdvancement
            {
                if (CBTreeFlag.Enabled) // tree visible
                {
                    if (isInherit()) // training selected from tree
                    {
                        // all enabled except of ..
                        if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                        {
                            enable = false;
                        }
                        if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                        {
                            enable = false;
                        }
                        if (colName.Equals("COST_EXTERNAL"))
                        {
                            enable = false;
                        }
                        if (colName.Equals("COST_INTERNAL"))
                        {
                            enable = false;
                        }
                        if (colName.Equals("LOCATION"))
                        {
                            enable = false;
                        }
                        if (colName.Equals("INSTRUCTOR"))
                        {
                            enable = false;
                        }
                        
                    }
                    else
                    {
                        enable = false; // all disabled
                    }
                }
                else 
                {
                    enable = true; // all enabled
                }
            }
            else // editAdvancement
            {
                if (CBTreeFlag.Enabled) // tree visible
                {
                    // all enabled except of ..
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "DESCRIPTION")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COST_EXTERNAL"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COST_INTERNAL"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("LOCATION"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("INSTRUCTOR"))
                    {
                        enable = false;
                    }
                    
                }
                else 
                {
                    enable = true; // all enabled
                }

                if ((_db.userId == (long)_table.Rows[0]["PERSON_ID"]) && (Global.isModuleEnabled("energiedienst")))
                {
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "TITLE")))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "DESCRIPTION")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("TOBEDONE_DATE"))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "CONTROLLING")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("TOBEDONE_DATE1"))
                    {
                        enable = false;
                    }
                    if (colName.Equals(_db.langAttrName("TRAINING_ADVANCEMENT", "CONTROLLING")))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COMMENT"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("COMMENT"))
                    {
                        enable = false;
                    }
                    if (colName.Equals("STATE"))
                    {
                        enable = false;
                    }
                    if (colName.Equals(" VIEWED_FLAG"))
                    {
                        enable = true;
                    }
                   
                }
            }
            if (ctrl is TextBox) 
            {
                TextBox tx = (TextBox) ctrl;
                tx.Enabled = enable;
            }
            if (ctrl is DropDownCtrl && Global.isModuleEnabled("energiedienst"))
            {
                DropDownCtrl dd = (DropDownCtrl)ctrl;
                dd.Enabled = enable;
            }
            if (ctrl is Table && Global.isModuleEnabled("energiedienst"))
            {
                Table tab = (Table)ctrl;
                tab.Enabled = enable;
            }
        }
 

        private bool isNew()
        {
            if (AdvancementID > 0)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        private bool showTree()
        {
            if (isNew() || TrainingID > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isInherit()
        {
            if (TrainingID > 0)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        protected void viewed_Changed(object sender, System.EventArgs e)
        {
            _db.connect();
            _db.execute("Update TRAINING_ADVANCEMENT SET VIEWED_FLAG = 1 WHERE ID = " + AdvancementID);
            _db.disconnect();
            //Response.Redirect(Global.Config.baseURL + "/Training/AdvancementEdit.aspx?PersonID=" + PersonID + "&AdvancementID =" + AdvancementID);
            Response.Redirect(Global.Config.baseURL + "/Training/Advancement.aspx?advancementID=" + AdvancementID + "&personID=" + PersonID);

      
        }

        protected void apply_Click(object sender, System.EventArgs e) 
        {
            if (checkInputValue(_table, advancementEdit)) 
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    StringBuilder sqlB = getSql(_table, advancementEdit, true);
                    
                    if (InputType == InputMaskBuilder.InputType.Add) 
                    {
                        AdvancementID = _db.newId("TRAINING_ADVANCEMENT");
                        extendSql(sqlB, _table, "ID", AdvancementID);
                        if (PersonID > 0)
                        {
                            extendSql(sqlB, _table, "PERSON_ID", PersonID);
                            if(Global.isModuleEnabled("energiedienst"))
                            {
                                extendSql(sqlB, _table, "RESPONSIBLE_PERSON_ID", _db.userId);
                            }
                        }
                        if (TrainingID > 0 && CBTreeFlag.Enabled)
                        {
                            extendSql(sqlB, _table, "TRAINING_ID", TrainingID);
                        }
                    }
                    else if (InputType == InputMaskBuilder.InputType.Edit)
                    {
                        if (TrainingID > 0 && !CBTreeFlag.Enabled)
                        {
                            extendSql(sqlB, _table, "TRAINING_ID", null);
                        }
                        if (TrainingID > 0 && CBTreeFlag.Enabled)
                        {
                            extendSql(sqlB, _table, "TRAINING_ID", TrainingID);
                        }
                    }
                    string sql = endExtendSql(sqlB);
                    if (sql.Length > 0)
                        _db.execute(sql);

                    _db.commit();
                }
                catch (Exception ex) 
                {
                    _db.rollback();
                    DoOnException(ex);
                }
                finally 
                {
                    _db.disconnect();   
                }
                Response.Redirect(Global.Config.baseURL + "/Training/Advancement.aspx?advancementID=" + AdvancementID + "&personID=" + PersonID);
            }		
        }

        private void MapButtonMethods() 
        {
            this.CBTreeFlag.CheckedChanged += new System.EventHandler(this.CBTreeFlag_CheckedChanged);
        }

        private void CBTreeFlag_CheckedChanged(object sender, System.EventArgs e) 
        {
            CBTreeFlag.Enabled = CBTreeFlag.Checked;
            foreach (Object o in _enablingControls)
            {
                controlEnabling((ArrayList) o);
            }
            if (_treeCtrl != null)
            {
                _treeCtrl.Visible = CBTreeFlag.Enabled;
            }
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            MapButtonMethods();
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

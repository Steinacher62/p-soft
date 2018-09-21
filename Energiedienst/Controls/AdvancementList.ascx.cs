namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Training;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial class AdvancementList : PSOFTListViewUserControl {
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_ADVANCEMENT_ID = "PARAM_ADVANCEMENT_ID";
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";

        private const string _TABLE = "TRAINING_ADVANCEMENT";


        public static string Path {
            get {return Global.Config.baseURL + "/Energiedienst/Controls/AdvancementList.ascx";}
        }

        public AdvancementList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            OrderColumn = "TOBEDONE_DATE";
            OrderDir = "asc";
        }

		#region Properities
        public long PersonID 
        {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }
        public long AdvancementID 
        {
            get {return GetLong(PARAM_ADVANCEMENT_ID);}
            set {SetParam(PARAM_ADVANCEMENT_ID, value);}
        }
        public string ContextList 
        {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            if (!IsPostBack) 
            {
                switch (ContextList) 
                {
                    default:
                        CBShowDone.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_SHOW_DONE_ADVANCEMENT);          
                        CBShowDone.Checked = SessionData.showDoneAdvancements(Session);
                        CBShowDone.Visible = true;
                        break;
                    case "search":
                        CBShowDone.Visible = false;
                        DeleteEnabled = false;
                        break;
                }
            }
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute();
            loadList();
        }

        private void loadList()
        {

            DBData db = DBData.getDBData(Session);
            try 
            {
                db.connect();
                
                switch (ContextList) 
                {
                    default:
                        pageTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MENMO_CT_ADVANCEMENT_LIST).Replace("#1", db.Person.getWholeName(PersonID));
                        break;
                    case "search":
                        pageTitle.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_CT_ADVANCEMENT_SEARCH_LIST);
                        break;
                }

                HighlightRecordID = AdvancementID;
                listTab.Rows.Clear();
                DataTable table = getTable(db);
                if (table != null)
                {
                    summary.Text = getSummary(table);

                    string [] states = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true);
                    table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(states);
                    DataTable personTable = db.Person.getWholeNameMATable(true);
                    if (ContextList != "search"){
                        table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.DETAIL;
                    }
                    else{
                        table.Columns["PERSON_ID"].ExtendedProperties["In"] = personTable;
                        table.Columns["PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%PERSON_ID", "mode","oe");
                    }
                    table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;
                    table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["ContextLink"] = psoft.Person.DetailFrame.GetURL("id","%RESPONSIBLE_PERSON_ID", "mode","oe");
                    table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = TrainingModule.getTrainingDemandTable(db);
                    LoadList(db, table, listTab);
                }
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                db.disconnect();
            }
        }



        private DataTable getTable(DBData db)
        {
            string sql = null;
            DataTable table = null;
            string tableName = _TABLE;
            switch (ContextList) 
            {
                default:
                    sql = "select * from " + tableName + " where PERSON_ID=" + PersonID;
                    if (!SessionData.showDoneAdvancements(Session))
                    {
                        sql += " and (STATE = 0 or STATE is null)";
                    }
                    break;
                case "search":
                    sql = Session["AdvancementSQLSearch"] as string;
                    tableName = Session["AdvancementSQLTable"] as string;
                    break;
            }            
            if (sql != null)
            {
                sql += " order by " + OrderColumn + " " + OrderDir;
                table = db.getDataTableExt(sql, tableName);
            }
           
            return table;
        }


        private string getSummary(DataTable table)
        {
            int cnt = 0;
            double costE = 0;
            double costI = 0;
            bool noCostE = false;
            bool noCostI = false;
            string costExt = "-.--";
            string costInt = "-.--";
            string ret = "";
            if (table.TableName.Equals(_TABLE)){
                foreach (DataRow row in table.Rows)
                {
                    costE += ch.psoft.Util.Validate.GetValid(row["COST_EXTERNAL"].ToString(),(double)-1);
                    costI += ch.psoft.Util.Validate.GetValid(row["COST_INTERNAL"].ToString(),(double)-1);
                    if (costE < 0)
                    {
                        noCostE = true;
                    }
                    if (costI < 0)
                    {
                        noCostI = true;
                    }
                    cnt++;
                }
            }
            
            // if ahb module enabled dont show cost 

            if (cnt > 0 && (Global.isModuleEnabled("ahb") == false && Global.isModuleEnabled("energiedienst") == false))
            {
                if (!noCostE)
                {
                    costExt = costE.ToString();
                }
                if (!noCostI)
                {
                    costInt = costI.ToString();
                }
                ret = " (" + cnt + ") - " 
                    + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_LBL_SUMMARY_TITLE)
                    + ": " + costInt + " " 
                    + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_LBL_COST_CURRENCY)
                    + " ("
                    + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_LBL_COST_INTERN)
                    +") " + costExt + " "
                    + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_LBL_COST_CURRENCY)
                    + " ("
                    + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_LBL_COST_EXTERN)
                    +") ";
            }
            return ret;
        }


        public long getFirstAdvancementID()
        {
            DBData db = DBData.getDBData(Session);
            long id = -1;
            try {
                db.connect();              
                foreach (DataRow row in getTable(db).Rows)
                {
                    id = long.Parse(row[0].ToString());
                    break;                       
                }           
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                db.disconnect();             
            }
            return id;
        }

        private void MapButtonMethods() 
        {
            this.CBShowDone.CheckedChanged += new System.EventHandler(this.CBShowDone_CheckedChanged);
        }

        private void CBShowDone_CheckedChanged(object sender, System.EventArgs e) 
        {
            SessionData.setShowDoneAdvancements(Session, CBShowDone.Checked);
            loadList();
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            if (row != null && cell != null)
            {
                if (cell.ClientID.IndexOf("LB_DELETE_CELL") > 0)
                {
                    if (!ch.psoft.Util.Validate.GetValid(row["RELEASE"].ToString(), "").Equals(""))
                    {
                        cell.Visible = false;
                    }
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
            MapButtonMethods();
        }
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}

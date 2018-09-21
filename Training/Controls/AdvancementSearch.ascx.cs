namespace ch.appl.psoft.Training.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    ///		Summary description for AdvancementSearch.
    /// </summary>
    public partial class AdvancementSearch : PSOFTSearchUserControl
	{
        private const string _TABLE = "TRAINING_ADVANCEMENT";
        protected DataTable _table;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Training/Controls/AdvancementSearch.ascx";}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DBData db = DBData.getDBData(Session);
			string sql = "select * from " + _TABLE + " where id = -1";
            
			try
			{
				if (!IsPostBack)
				{
                    //Session["AdvancementSQLSearch"] = null;
					apply.Text = _mapper.get("search");
                    CBShowDone.Text = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_SHOW_DONE_ADVANCEMENT);          
                    CBShowDone.Checked = SessionData.showDoneAdvancements(Session);
                    ShowRelation = true;                  
                }
				db.connect();

				_table = db.getDataTableExt(sql, _TABLE);

                DataTable personTable = db.getDataTable("select PERSON.ID, " + Person.getWholeNameSQL(false, true, false) + " from PERSON where ID in (" + db.Training.getTrainingPersonIDs() + ") order by PERSON.PNAME, PERSON.FIRSTNAME");
                _table.Columns["PERSON_ID"].ExtendedProperties["In"] = personTable;
                _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                DataTable responsibleTable = db.Person.getWholeNameMATable(false);
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = responsibleTable;
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["Nullable"] = true;
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                DataTable demandTable  = TrainingModule.getTrainingDemandTable(db);
                _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = demandTable;
                _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                
                string [] states = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true);
                _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(states);
                
				base.CheckOrder = true;
                base.View = _TABLE;
				base.LoadInput(db,_table,searchTab);
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

		private void mapControls ()
		{
            this.CBShowDone.CheckedChanged += new System.EventHandler(this.CBShowDone_CheckedChanged);
			apply.Click += new System.EventHandler(apply_Click);
		}

		private void apply_Click(object sender, System.EventArgs e)
		{
            if (checkInputValue(_table, searchTab)) {
                string sql = base.getSql(_table, searchTab);
            
                if (sql == ""){
                    sql = "select * from " + _TABLE;
                }
                if (sql.IndexOf("where") > 0) {
                    sql += " and ";
                }
                else {
                    sql += " where ";
                }

                DBData db = DBData.getDBData(Session);
                db.connect();
                try{
                    sql += " person_id in ( " + db.Training.getTrainingPersonIDs() + ")";
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    db.disconnect();
                }

                if (!SessionData.showDoneAdvancements(Session)) {
                    sql += " and (STATE = 0 or STATE is null)";
                }
            
          
                Session["AdvancementSQLSearch"] = sql;
                Session["AdvancementSQLTable"] = _TABLE;
            
                _searchArgs.SearchSQL = sql;

                DoOnSearchClick(apply);
            }
		}

		private void CBShowDone_CheckedChanged(object sender, System.EventArgs e)
		{
			SessionData.setShowDoneAdvancements(Session, CBShowDone.Checked);
            if (Session["AdvancementSQLSearch"] != null)
            {
                apply_Click(sender, e);
            }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{

        }
		#endregion
	}
}

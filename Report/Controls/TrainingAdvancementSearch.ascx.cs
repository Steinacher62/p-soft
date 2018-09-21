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
    public partial class TrainingAdvancementSearch : PSOFTSearchUserControl
	{
        //private const string _TABLE = "TRAINING_ADVANCEMENT";
        //private const string _TABLE = "AusbildungsmassnahmenAHB";
        private const string _TABLE = "TRAININGADVANCEMENTTRAININGV";
        protected DataTable _table;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Report/Controls/TrainingAdvancementSearch.ascx";}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DBData db = DBData.getDBData(Session);
			string sql = "select * from " + _TABLE + " where id = -1";
            //string sql = "select * from " + _TABLE;
            
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
                _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;

                DataTable responsibleTable = db.Person.getWholeNameMATable(false);
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = responsibleTable;
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["Nullable"] = true;
                _table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                DataTable demandTable = TrainingModule.getTrainingDemandTable(db);
                _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["In"] = demandTable;
                _table.Columns["TRAINING_DEMAND_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                string[] states = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true);
                _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(states);

                DataTable oeTable = db.Orgentity.orgentities;
                _table.Columns["OE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["OE"].ExtendedProperties["In"] = oeTable;
                _table.Columns["OE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;

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
                sql = sql.Replace("OE =", "OEID =");
            
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
            
                //_searchArgs.SearchSQL = sql;
                //DoOnSearchClick(apply);

                //get comma seperated list of training IDs
                db.connect();
                DataTable trainingTable = db.getDataTable(sql);
                //db.disconnect();

                // if more than 1000 elements the array is splitted (crystal reports can not handle more than 1000 elements in an array) / 13.07.10 / mkr
                string trainingIDs = "";
                string trainingIDs1 = "";
                string trainingIDsTotal = "-1,";
                int idx = 0;
                foreach (DataRow trainingRow in trainingTable.Rows)
                {
                    idx++;
                    trainingIDsTotal += trainingRow["ID"].ToString() + ",";
                    if (idx < 1000)
                    {
                        trainingIDs += trainingRow["ID"].ToString() + ",";
                    }
                    else
                    {
                        trainingIDs1 += trainingRow["ID"].ToString() + ",";
                    }
                }
                //trainingIDs = trainingIDs.TrimEnd(new char[] { ',' });
                //trainingIDs1 = trainingIDs1.TrimEnd(new char[] { ',' });
                trainingIDsTotal = trainingIDsTotal.TrimEnd(new char[] { ',' });

                //Session["trainingIDs"] = trainingIDs;
                //Session["trainingIDs1"] = trainingIDs1;

                //save results in temporary table, then call report

                // delete temporary table if exists
                //db.connect();
                string tbl_del = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ausbildungsmassnahmen_%userid%]') "
                               + "AND type in (N'U')) "
                               + "DROP TABLE [dbo].[Ausbildungsmassnahmen_%userid%]";
                db.execute(tbl_del.Replace("%userid%", db.userId.ToString()));

                // create table
                string tbl_create = "CREATE TABLE [dbo].[Ausbildungsmassnahmen_%userid%]("
                                    + "[ID] [bigint] NULL,"
                                    + "[Organisationseinheit] [varchar](128) NULL,"
                                    + "[P-Nr] [varchar](64) NULL,"
                                    + "[Name] [varchar](64) NULL,"
                                    + "[Vorname] [varchar](64) NULL,"
                                    + "[Stellenbezeichnung] [varchar](128) NULL,"
                                    + "[Titel] [varchar](128) NULL,"
                                    + "[Beschreibung] [varchar](1000) NULL,"
                                    + "[Erstellt am] [datetime] NULL,"
                                    + "[Startdatum] [datetime] NULL,"
                                    + "[Erledigt bis] [datetime] NULL,"
                                    + "[Erledigt am] [datetime] NULL,"
                                    + "[Name Ersteller] [varchar](64) NULL,"
                                    + "[Vorname Ersteller] [varchar](64) NULL,"
                                    + "[Interne Kosten] [float] NULL,"
                                    + "[Ausbildner] [varchar](128) NULL,"
                                    + "[Name Verantwortlicher] [varchar](64) NULL,"
                                    + "[Vorname Verantwortlicher] [varchar](64) NULL,"
                                    + "[Erledigt] [int] NULL,"
                                    + "[PersonenID] [bigint] NULL,"
                                    + "[LEAVING] [datetime] NULL,"
                                    + "[Externe Kosten] [float] NULL,"
                                    + "[Kursort] [varchar](128) NULL,"
                                    + "[Kursanbieter] [varchar](128) NULL,"
                                    + "[Kostenbeteiligung] [float] NULL,"
                                    + "[Verpflichtung bis] [datetime] NULL"
                                    + ") ON [PRIMARY]";
                db.execute(tbl_create.Replace("%userid%", db.userId.ToString()));

                //fill table
                db.execute("INSERT INTO Ausbildungsmassnahmen_" + db.userId.ToString() + " SELECT * FROM AusbildungsmassnahmenAHB WHERE ID IN (" + trainingIDsTotal + ")");
                db.disconnect();

                if (Global.isModuleEnabled("ahb"))
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=AusbildungsmassnahmenAHB",true);
                }
                else
                {
                    Response.Redirect("CrystalReportViewer.aspx?alias=Ausbildungsmassnahmen,true");
                }

                
            }
		}

		private void CBShowDone_CheckedChanged(object sender, System.EventArgs e)
		{
			SessionData.setShowDoneAdvancements(Session, CBShowDone.Checked);
            if (Session["AdvancementSQLSearch"] != null)
            {
                //apply_Click(sender, e);
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

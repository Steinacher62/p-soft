namespace ch.appl.psoft.Tasklist.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for PendenzenQuellenSearch.
    /// </summary>
    public partial class PendenzenSearch : PSOFTSearchUserControl
	{
        public const string PARAM_BASE = "PARAM_BASE";
		public const string PARAM_MODE = "PARAM_MODE";
		public const string PARAM_TEMPLATE = "PARAM_TEMPLATE";
		public const string PARAM_XID = "PARAM_XID";

		protected DataTable _table;


		public static string Path
		{
			get {return Global.Config.baseURL + "/Tasklist/Controls/PendenzenSearch.ascx";}
		}

		#region Properities
        public string Base {
            get {return GetString(PARAM_BASE);}
            set {SetParam(PARAM_BASE, value);}
        }

        public string Mode {
            get {return GetString(PARAM_MODE);}
            set {SetParam(PARAM_MODE, value);}
        }

		public bool Template 
		{
			get {return GetBool(PARAM_TEMPLATE);}
			set {SetParam(PARAM_TEMPLATE, value);}
		}

        public long XID {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DBData db = DBData.getDBData(Session);
			string sql = "select * from " + Base + " where id = -1";
            
			try
			{
				if (!IsPostBack)
				{
                    Session["TasklistSQLSearch"] = null;
					apply.Text = _mapper.get("search");
					semaphore.Text = _mapper.get("tasklist", "semaphore");
					rbAlle.Text = _mapper.get("all");
					rbAlle.Checked = true;

					if (Base == "measure")
					{
						tdDone.Disabled = !(CBShowDone.Checked = SessionData.showDoneMeasures(Session));
						CBShowDone.Text = _mapper.get("tasklist", "showDoneMeasures");
						imRed.ToolTip = TaskListModule.getSemaphoreMeasureComment(Session, 0);
						imOrange.ToolTip = TaskListModule.getSemaphoreMeasureComment(Session, 1);
						imGreen.ToolTip = TaskListModule.getSemaphoreMeasureComment(Session, 2);
						imDone.ToolTip = TaskListModule.getSemaphoreMeasureComment(Session, 3);
					}
					else
					{
						tdDone.Disabled = !(CBShowDone.Checked = SessionData.showDoneTasklists(Session));
						CBShowDone.Text = _mapper.get("tasklist", "showDoneTasklists");
						imRed.ToolTip = TaskListModule.getSemaphoreTasklistComment(Session, 0);
						imOrange.ToolTip = TaskListModule.getSemaphoreTasklistComment(Session, 1);
						imGreen.ToolTip = TaskListModule.getSemaphoreTasklistComment(Session, 2);
						imDone.ToolTip = TaskListModule.getSemaphoreTasklistComment(Session, 3);
					}

					if (Template)
					{
						semaphore.Visible = false;
						rbAlle.Visible = false;
						tdDone.Visible = false;
						tdGreen.Visible = false;
						tdOrange.Visible = false;
						tdRed.Visible = false;
						CBShowDone.Visible = false;
					}
					else
					{
						tdDone.Attributes.Add("onclick",rbDone.ClientID+".checked=true");
						tdGreen.Attributes.Add("onclick",rbGreen.ClientID+".checked=true");
						tdOrange.Attributes.Add("onclick",rbOrange.ClientID+".checked=true");
						tdRed.Attributes.Add("onclick",rbRed.ClientID+".checked=true");
					}

                }

				db.connect();
				_table = db.getDataTableExt(sql, Base.ToUpper());
                
				DataTable personTable = db.Person.getWholeNameMATable(false);
				_table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
				_table.Columns["AUTHOR_PERSON_ID"].ExtendedProperties["In"] = personTable;

				if (Base == "measure")
				{
					string [] states = _mapper.getEnum("tasklist", "state", true);
					_table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
					_table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(states);

					_table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
					_table.Columns["RESPONSIBLE_PERSON_ID"].ExtendedProperties["In"] = personTable;
				}
				else if (Base == "tasklist")
				{
					_table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
					_table.Columns["TEMPLATE_TASKLIST_ID"].ExtendedProperties["In"] = db.Tasklist.getTemplatesTable();
				}

				base.CheckOrder = true;
                base.View = Base.ToUpper() + "_DETAIL";
				base.LoadInput(db,_table,searchTab);

                if (Base == "tasklist"){
                    // Checkbox for search in subordinate tasklists
                    TableRow r = new TableRow();
                    searchTab.Rows.Add(r);
                    TableCell c = new TableCell();
                    c.CssClass = "InputMask_Label";
                    c.ColumnSpan = 3;
                    r.Cells.Add(c);
                    CheckBox cb = new CheckBox();
                    cb.Text = _mapper.get("tasklist", "searchSubTasklists");
                    cb.ID = "INCLUDE_SUBORDINATE";
                    cb.Checked = false;
                    c.Controls.Add(cb);
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

		private void mapControls ()
		{
			apply.Click += new System.EventHandler(apply_Click);
		}

		private void apply_Click(object sender, System.EventArgs e)
		{
            if (checkInputValue(_table, searchTab))
			{
                string sql = base.getSql(_table, searchTab);
            
                if (sql == "")
                    sql = "select * from " + Base;

                if (sql.IndexOf("where") > 0)
                    sql += " and ";
                else
                    sql += " where ";

                sql += "TEMPLATE=" + (Template ? "1" : "0");

                switch (Base) {
                    case "tasklist":
                        sql += " and TYP=0"; // only public tasklists
                        CheckBox cb = (CheckBox) searchTab.FindControl("INCLUDE_SUBORDINATE");
                        bool onlyRoots = true;

						if (Template)
						{
                            sql += " and owner_person_id = " + SessionData.getUserID(Session);
						}

						if (cb != null)
						{
                            onlyRoots = !cb.Checked;
                        }

                        if (onlyRoots)
						{
                            sql += " and isnull(PARENT_ID,0) = 0";
                        }

                        DBData db = DBData.getDBData(Session);
                        db.connect();
                        try{
                            string inStr = "-";
                            if (rbRed.Checked) {
                                inStr = db.Tasklist.GetTasklistBySemaphore(0, onlyRoots);
                            }
                            else if (rbOrange.Checked) {
                                inStr = db.Tasklist.GetTasklistBySemaphore(1, onlyRoots);
                            }
                            else if (rbGreen.Checked) {
                                inStr = db.Tasklist.GetTasklistBySemaphore(2, onlyRoots);
                            }
                            else if (rbDone.Checked) {
                                inStr = db.Tasklist.GetTasklistBySemaphore(3, onlyRoots);
                            }

                            if (inStr != "-") {
                                if (inStr == ""){
                                    inStr = "null";
                                }
                                sql += " and id in (" + inStr + ")";
                            }

                            if (Mode == "assign"){
                                // Forbidden: Keine Tasklisten aus eigenem Tree, keine bereits 'assigned'
								// und keine übergeordnete von bereits 'assigned'
								string forbiddenTasklists = db.Tasklist.addAllParentTasklistIDs(db.Tasklist.addAllSubTasklistIDs(db.Tasklist.addAllParentTasklistIDs(XID.ToString())));
                                sql += " and id not in (" + forbiddenTasklists + ")";
                            }
                        }
                        catch (Exception ex) {
                            DoOnException(ex);
                        }
                        finally {
                            db.disconnect();
                        }
                        
                        break;

                    case "measure":
                        if (rbRed.Checked)
                            sql += " and STATE=0 and DUEDATE<GetDate()";
                        else if (rbOrange.Checked)
                            sql += " and STATE=0 and DUEDATE>GetDate() and DUEDATE<GetDate()+(select criticaldays from tasklist where id = tasklist_id)";
                        else if (rbGreen.Checked)
                            sql += " and STATE=0 and (DUEDATE>GetDate()+(select criticaldays from tasklist where id = tasklist_id) or DUEDATE is null)";
                        else if (rbDone.Checked)
                            sql += " and STATE=1";
                        
                        break;
                }
          
                Session["TasklistSQLSearch"] = sql;
            
                _searchArgs.SearchSQL = sql;

                DoOnSearchClick(apply);
            }
		}

		protected void CBShowDone_CheckedChanged(object sender, System.EventArgs e)
		{
			tdDone.Disabled = !CBShowDone.Checked;
			if (Base == "measure")
				SessionData.setShowDoneMeasures(Session, CBShowDone.Checked);
			else
				SessionData.setShowDoneTasklists(Session, CBShowDone.Checked);

			if (!CBShowDone.Checked && rbDone.Checked)
			{
				rbDone.Checked = false;
				rbAlle.Checked = true;
			}

            if (Session["TasklistSQLSearch"] != null)
            {
                DoOnSearchClick(apply);
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

namespace ch.appl.psoft.Person.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Summary description for Search.
    /// </summary>
    public partial class PersonSearch : PSOFTSearchUserControl 
	{
		private DataTable _table;
		private String _sql = "";

		protected System.Web.UI.WebControls.Table Table1;
       
		public static string Path
		{
			get {return Global.Config.baseURL + "/Person/Controls/PersonSearch.ascx";}
		}

		protected void Page_Load(object sender, System.EventArgs e) 
		{
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();
            DBData db = DBData.getDBData(Session);
            DataTable OETable;

            try
            {
                if (!IsPostBack)
                {
                    apply.Text = _mapper.get("search");
                }
                this.Page.Form.DefaultButton = apply.UniqueID;
                db.connect();
                long groupAccessorId = DBColumn.GetValid(
                db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),
                (long)-1);
                // Gruppe HR bei SPZ zusätzliche Felder anzeigen
                if (Global.isModuleEnabled("spz") & (db.isAccessorGroupMember(db.userAccessorID,groupAccessorId,true)))
                {
                    _sql = "select * from PERSONOEV where id = -1";
                    _table = db.getDataTableExt(_sql, "PERSONOEV");
                }
                else
                {
                    _sql = "select * from PERSON where id = -1";
                    _table = db.getDataTableExt(_sql, "PERSON");
                }

                // Personnelnumber invisible if energiedinst and not group hr
                if (Global.isModuleEnabled("energiedienst") & !(db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)))
                {
                    _table.Columns["PERSONNELNUMBER"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }
                _sql = "select ID,TITLE from FIRM where (TYP&1)>0 order by TITLE";
                DataTable firmTable = db.getDataTable(_sql);
                _table.Columns["FIRM_ID"].ExtendedProperties["In"] = firmTable;
                _table.Columns["FIRM_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                _table.Columns["EMPLOYMENT_CONDITION"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                //_table.Columns["LEAVING"].ExtendedProperties["Visibility"] = DBColumn.Visibility.SEARCH;
                _table.Columns["FIRM_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["PHONE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                
                
                //is modul sgv active?
                if (_table.Columns.Contains("EMPLOYMENT_REMARK"))
                {
                    _table.Columns["EMPLOYMENT_REMARK"].ExtendedProperties["In"] = _mapper.getEnum("person", "employmentRemark");
                    _table.Columns["EMPLOYMENT_REMARK"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                }

                View = "MA";
                LoadInput(db, _table, searchTab);

                // Dropdown for Function
                TableRow r = new TableRow();
                searchTab.Rows.Add(r);
                TableCell c = new TableCell();
                DropDownList fb = new DropDownCtrl();
                if (Global.isModuleEnabled("spz"))
                {
                    c.Text = _mapper.get("FUNKTION", db.langAttrName("FUNKTION", "TITLE"));
                    fb.ID = "EMPLOYMENT";
                }
                else
                {
                    c.Text = _mapper.get("FUNKTION", db.langAttrName("FUNKTION", "TITLE"));
                    fb.ID = "FUNKTION";
                }
                c.CssClass = "InputMask_Label";
                r.Cells.Add(c);
                r.Cells.Add(new TableCell());
                c = new TableCell();
                r.Cells.Add(c);
                
               
                c.Controls.Add(fb);
                if (!IsPostBack)
                {
                    fb.Items.Add("");

                    DataTable FBTable;
                    if (Global.isModuleEnabled("spz")& !(db.isAccessorGroupMember(db.userAccessorID,groupAccessorId,true)))
        
                    {
                        FBTable = db.getDataTable("select distinct " + db.langAttrName("EMPLOYMENT", "TITLE") + " from EMPLOYMENT order by " + db.langAttrName("EMPLOYMENT", "TITLE"));
                    }
                    else
                    {
                        FBTable = db.getDataTable("select distinct " + db.langAttrName("FUNKTION", "TITLE") + " from FUNKTION order by " + db.langAttrName("Funktion", "TITLE"));
                    }
                        foreach (DataRow row in FBTable.Rows)
                    {
                        fb.Items.Add(row[0].ToString());
                    }
                }

                // Dropdown for OrgEntity
                r = new TableRow();
                searchTab.Rows.Add(r);
                c = new TableCell();
                c.Text = _mapper.get("OEPERSONV", db.langAttrName("OEPERSONV", "OE_TITLE"));
                c.CssClass = "InputMask_Label";
                r.Cells.Add(c);
                r.Cells.Add(new TableCell());
                c = new TableCell();
                r.Cells.Add(c);
                DropDownList d = new DropDownCtrl();
                d.ID = "OE_TITLE";
                c.Controls.Add(d);
                if (!IsPostBack)
                {
                    d.Items.Add("");
                    string OE_IDs = db.Orgentity.addAllSubOEIDs(db.lookup("ORGENTITY_ID", "ORGANISATION", "MAINORGANISATION=1", false));
                    if (OE_IDs != "")
                    {
                        OETable = db.getDataTable("select distinct " + db.langAttrName("ORGENTITY", "TITLE") + " from ORGENTITY where ID in (" + OE_IDs + ") order by " + db.langAttrName("ORGENTITY", "TITLE"));
                        foreach (DataRow row in OETable.Rows)
                        {
                            d.Items.Add(row[0].ToString());
                        }
                    }
                }

                //r = new TableRow();
                //searchTab.Rows.Add(r);
                // Checkbox 'Orgentity recursive'
                {
                    c = new TableCell();
                    c.CssClass = "InputMask_Label";
                    r.Cells.Add(c);
                    CheckBox cb = new CheckBox();
                    cb.Text = _mapper.get("person", "orgEntityRecursive");
                    cb.ID = "OE_RECURSIVE";
                    cb.Checked = true;
                    c.Controls.Add(cb);
                }

                // Checkbox 'Display inactive employees'

                {
                    c = new TableCell();
                    c.CssClass = "InputMask_Label";
                    r.Cells.Add(c);
                    CheckBox cb = new CheckBox();
                    cb.Text = _mapper.get("person", "orgDisplayInactiveEmployee");
                    cb.ID = "OE_DISPLAYINACTIVEEMPLOYEES";
                    cb.Checked = false;
                    c.Controls.Add(cb);

                    // not show if enabled module spz
                    if (Global.isModuleEnabled("spz"))
                        cb.Visible = false;
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

        private string sqlAppendWhere(string sql, string clause)
        {
            sql += ((sql.ToLower().IndexOf(" where ") > 0) ? " and " : " where ") + clause;
            return sql;
        }
       
		private void apply_Click(object sender, System.EventArgs e)
		{
			if (!checkInputValue(_table, searchTab))
				return;

			_sql = getSql(_table, searchTab);

            if (_sql == "")
                _sql = "select distinct * from personoev";//+ _table.TableName;
            
            // if search personnelnumber 13.11.25 MSr

            if (Global.isModuleEnabled("energiedienst"))
            {
                if (searchTab.FindControl("PERSON-PERSONNELNUMBER") != null)
                {
                    TextBox CntpNr = (TextBox)searchTab.FindControl("PERSON-PERSONNELNUMBER").Controls[2].Controls[0];
                    if (!CntpNr.Text.Equals(""))
                    {
                        _sql = _sql.Replace("%", "");
                        _sql = _sql.Replace("like", "=");
                    }
                }
            }

            

            _sql = sqlAppendWhere(_sql, "(TYP & 3)=1"); //only employees

            CheckBox oeDisplayInactive = (CheckBox)searchTab.FindControl("OE_DISPLAYINACTIVEEMPLOYEES");
            if (oeDisplayInactive != null && !oeDisplayInactive.Checked)
            {
                //don't show inactive persons
                _sql = sqlAppendWhere(_sql, Person.ONLY_ACTIVE_SQL_RESTRICTION);
            }
			string selectedOE = "";
            string orgEntityIDs = "";
            DropDownList d = (DropDownList) searchTab.FindControl("OE_TITLE");
            if (d != null)
				selectedOE = d.SelectedItem.Text;

            string function = "";
            string employment = "";
            if (Global.isModuleEnabled("spz"))
            {
                DropDownCtrl emp = (DropDownCtrl)searchTab.FindControl("EMPLOYMENT");
                if (emp != null)
                    employment = emp.Text;
            }
            else
            {
                DropDownCtrl fb = (DropDownCtrl)searchTab.FindControl("FUNKTION");
                if (fb != null)
                    function = fb.Text;
            }

            DBData db = DBData.getDBData(Session);
            db.connect();
            try {
                if (selectedOE != "")
			    {
                    DataTable table = db.getDataTable("select ID from ORGENTITY where ROOT_ID in (select ORGENTITY_ID from ORGANISATION where MAINORGANISATION=1) and "+db.langAttrName("ORGANISATION","TITLE")+"='" + selectedOE + "'");
                    bool isFirst = true;
                    foreach (DataRow row in table.Rows)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            orgEntityIDs += ",";
                        orgEntityIDs += row[0].ToString();
                    }
                    CheckBox cb = (CheckBox) searchTab.FindControl("OE_RECURSIVE");
                    bool searchRecursive = false;
                    if (cb != null)
                    {
                        searchRecursive = cb.Checked;
                    }
                    if (searchRecursive)
                    {
                        orgEntityIDs = db.Orgentity.addAllSubOEIDs(orgEntityIDs);
                    }
    			}

                if (orgEntityIDs != "" || function != ""){
                    string addToSql = "ID in (select distinct PERSON.ID from JOB, EMPLOYMENT, PERSON where JOB.EMPLOYMENT_ID=EMPLOYMENT.ID and PERSON.ID=EMPLOYMENT.PERSON_ID" + (orgEntityIDs != ""? " and JOB.ORGENTITY_ID in (" + orgEntityIDs + ")" : "") + (function != ""? " and (JOB.FUNKTION_ID in (select ID from FUNKTION where " + db.langAttrName("FUNKTION", "TITLE") + "= '" + DBColumn.toSql(function) + "') or JOB." + db.langAttrName("JOB", "TITLE") + "= '" + DBColumn.toSql(function) + "')" : "") + ")";
                    _sql = sqlAppendWhere(_sql, addToSql);
                }

                if (orgEntityIDs != "" || employment != "")
                {
                    string addToSql = "ID in (select distinct PERSON.ID from JOB, EMPLOYMENT, PERSON where JOB.EMPLOYMENT_ID=EMPLOYMENT.ID and PERSON.ID=EMPLOYMENT.PERSON_ID" + (orgEntityIDs != "" ? " and JOB.ORGENTITY_ID in (" + orgEntityIDs + ")" : "") + (employment != "" ? " and (JOB.EMPLOYMENT_ID in (select ID from EMPLOYMENT where " + db.langAttrName("EMPLOYMENT", "TITLE") + " = '" + DBColumn.toSql(employment) + "') or JOB." + db.langAttrName("JOB", "TITLE") + " = '" + DBColumn.toSql(employment) + "')" : "") + ")";
                    _sql = sqlAppendWhere(_sql, addToSql);
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }

			// Setting search event args
			_searchArgs.ReloadList = true;
			_searchArgs.SearchSQL = _sql;

			DoOnSearchClick(apply);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() 
		{    
			ID = "Search";
		}
		#endregion
	}
}
namespace ch.appl.psoft.MbO.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class EditView : PSOFTInputViewUserControl
    {
        protected long _id = 0;
        protected long _contextId = 0;
        protected int _typ = Objective.UNDEFINED_TYP;
        protected string _action = "edit";
        protected string _backURL = "";
        protected string _context = "";
        protected string _checkFlags = "";
        private DBData _db = null;
        private DataTable _table = null;

        public static string Path
        {
            get { return Global.Config.baseURL + "/MbO/Controls/EditView.ascx"; }
        }

        #region Properities

        /// <summary>
        /// Get/Set current id
        /// </summary>
        public long id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Get/Set typ
        /// </summary>
        public int typ
        {
            get { return _typ; }
            set { _typ = value; }
        }
        /// <summary>
        /// Get/Set context id
        /// </summary>
        public long contextId
        {
            get { return _contextId; }
            set { _contextId = value; }
        }

        /// <summary>
        /// Get/Set action (edit,add,copy)
        /// </summary>
        public string action
        {
            get { return _action; }
            set
            {
                _action = value;
            }
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL
        {
            get { return _backURL; }
            set { _backURL = value; }
        }
        /// <summary>
        /// Get/set context (objective, person, oe, job)
        /// </summary>
        public string context
        {
            get { return _context; }
            set { _context = value; }
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            apply.Text = _mapper.get("apply");
            _db = DBData.getDBData(Session);
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            _db.connect();
            try
            {
                _table = _db.getDataTableExt("select * from OBJECTIVE where ID=" + (_action == "add" ? 0 : _id), "OBJECTIVE");
                if (_action == "edit" && _table.Rows.Count > 0 && _typ == Objective.UNDEFINED_TYP) _typ = (int)_table.Rows[0]["TYP"];
                {
                    switch (_typ)
                    {
                        case Objective.ORGANISATION_TYP:
                            _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                            _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                            _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                            _table.Columns["TARGETVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["VALUEIMPLICIT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                            _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ4", true));
                            _table.Columns["PARENT_ID"].ExtendedProperties["In"] = _db.Objective.organsationObjectives;
                            _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);

                            break;
                    //    case Objective.ORGENTITY_TYP:
                    //        _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["ORGENTITY_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = _db.Orgentity.orgentities;
                    //        break;

                    //    case Objective.JOB_TYP:
                    //        _table.Columns["JOB_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["JOB_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["JOB_ID"].ExtendedProperties["In"] = _db.Orgentity.jobs;
                    //        break;
                    //    case Objective.PERSON_TYP:
                    //        _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                    //        _table.Columns["TARGETVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    //        _table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    //        _table.Columns["VALUEIMPLICIT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    //        _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                    //        _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(DropDownList);
                    //        _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ", true));
                          
                    //        break;
                    //    case Objective.PROJECT_TYP:
                    //        _table.Columns["PROJECT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["PROJECT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        DataTable projTab = _db.getDataTable("select id,title from project");
                    //        _table.Columns["PROJECT_ID"].ExtendedProperties["In"] = projTab;
                            //break;
                        default:
                            _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            //_table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                            //_table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                            _table.Columns["TARGETVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            _table.Columns["VALUEIMPLICIT"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            //Objectivetyp off
                            //_table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                            _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(DropDownList);
                            _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ4", true));
                            _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                            
                        _table.Columns["PARENT_ID"].ExtendedProperties["In"] = _db.Objective.organsationObjectives;
                        _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                            _table.Columns["TYP"].ExtendedProperties["Nullable"] = false;
                             
                            break;
                    }
                    //_table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    //_table.Columns["MEASUREMENT_TYPE_ID"].ExtendedProperties["In"] = _db.Objective.types;
                    //_table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    //_table.Columns["STATE"].ExtendedProperties["Nullable"] = false;
                    //_table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "state2", false));




                    //if (!_db.Objective.tasklistEnable)
                    //{
                    //    _table.Columns["TASKLIST"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                    //}
                }
                detailTab.Rows.Clear();
                base.InputType = _action == "add" ? InputMaskBuilder.InputType.Add : InputMaskBuilder.InputType.Edit;
                if (_action == "edit")
                {
                    //switch (_typ)
                    //{
                    //    case Objective.ORGENTITY_TYP:
                    //        _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["ORGENTITY_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = _db.Orgentity.orgentities;
                    //        _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    //        break;

                    //    case Objective.JOB_TYP:
                    //        _table.Columns["JOB_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["JOB_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["JOB_ID"].ExtendedProperties["In"] = _db.Orgentity.jobs;
                    //        break;
                    //    case Objective.PERSON_TYP:
                    //        _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                    //        _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        _table.Columns["PARENT_ID"].ExtendedProperties["In"] = _db.Objective.organsationObjectives;
                    //        _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        break;
                    //    case Objective.PROJECT_TYP:
                    //        _table.Columns["PROJECT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //        _table.Columns["PROJECT_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    //        DataTable projTab = _db.getDataTable("select id,title from project");
                    //        _table.Columns["PROJECT_ID"].ExtendedProperties["In"] = projTab;
                    //        break;
                    //}

                    _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                    _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                    _table.Columns["TASKLIST_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                    if (_typ != Objective.ORGANISATION_TYP)
                    {
                        _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                        _table.Columns["PARENT_ID"].ExtendedProperties["In"] = _db.Objective.organsationObjectives;
                        _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        //Typ Invisible
                        //_table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                        _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(Label);
                        _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ4", true));
                    }

                    if (Global.isModuleEnabled("foampartner"))
                    {
                        _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }


                    //if (DBColumn.IsNull(_table.Rows[0]["TASKLIST_ID"]))
                    //{
                    //    _table.Columns["TASKLIST"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    //}
                }

                else if (_action == "add")
                {
                    if(_typ == Objective.ORGANISATION_TYP)
                    {
                        _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                        _table.Columns["PERSON_ID"].ExtendedProperties["Nullable"] = false;
                        _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        _table.Columns["PERSON_ID"].ExtendedProperties["In"] = perstab();
                    }

                    
                    if (_typ == Objective.PERSON_TYP)
                    {
                        //_table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        //_table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                        //_table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                        //_table.Columns["PERSON_ID"].ExtendedProperties["Nullable"] = false;
                        //_table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                        _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                        //_table.Columns["PARENT_ID"].ExtendedProperties["Nullable"] = false;
                        _table.Columns["PARENT_ID"].ExtendedProperties["In"] = _db.Objective.organsationObjectives;
                        _table.Columns["PARENT_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);


                        _table.Columns["PERSON_ID"].ExtendedProperties["In"] = perstab();
                    }
                    if (_id <= 0)
                    {
                       // _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }

                    if (Global.isModuleEnabled("foampartner"))
                    {
                        _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }


                    if (!_db.Objective.hasAuthorisation(DBData.AUTHORISATION.UPDATE) && false)
                    {
                        DataTable table = null;
                        string sql = "";
                        switch (_typ)
                        {
                            case Objective.ORGANISATION_TYP :
                                _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                                _table.Columns["PARENT_ID"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                                break;
                            
                            case Objective.ORGENTITY_TYP:
                                //_table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                                _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                                _table.Columns["ORGENTITY_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                                string name = _db.langAttrName("ORGENTITY", "TITLE");
                                sql = "select oe.id,oe." + name + " from organisation o inner join (orgentity oe inner join oepersonv p on oe.parent_id = p.oe_id) on o.orgentity_id = oe.root_id where o.mainorganisation = 1 and p.id = " + _db.userId + " order by oe." + name;
                                table = _db.getDataTable(sql);
                                if (table.Rows.Count == 0) redirect();
                                _table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = table;
                                _table.Columns["ORGENTITY_ID"].ExtendedProperties["Nullable"] = false;
                                break;

                            case Objective.JOB_TYP:
                               // _table.Columns["TYP"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                                _table.Columns["JOB_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                                _table.Columns["JOB_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                                name = "pers.pname+' '+pers.firstname+', '+JOB." + _db.langAttrName("JOB", "TITLE") + "+' '+OE." + _db.langAttrName("ORGENTITY", "TITLE") + " NAME ";
                                string join = "(JOBEMPLOYMENTV job inner join person pers on job.person_id = pers.id) inner join (ORGENTITY OE inner join ORGANISATION o on oe.ROOT_ID = o.ORGENTITY_ID) on job.ORGENTITY_ID = OE.ID";
                                sql = "(select job.ID," + name + " from " + join + " where o.MAINORGANISATION = 1 and OE.PARENT_ID in (select OE_ID from oepersonv where id = " + _db.userId + ")";
                                sql += " union ";
                                sql += "select job.ID," + name + " from " + join + " where o.MAINORGANISATION = 1 and JOB.TYP = 0 and OE.ID in (select OE_ID from oepersonv where id = " + _db.userId + ")) ";
                                sql += "order by NAME";
                                table = _db.getDataTable(sql);
                                if (table.Rows.Count == 0) redirect();
                                _table.Columns["JOB_ID"].ExtendedProperties["In"] = table;
                                _table.Columns["JOB_ID"].ExtendedProperties["Nullable"] = false;
                                break;
                        }
                    }
                }
                else if (_action == "replace")
                {
                    _table.Columns["STATE"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                    _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "state2", false)); ;
                    switch (_typ)
                    {
                        case Objective.ORGENTITY_TYP:
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = _db.getDataTable("select id," + _db.langAttrName("ORGENTITY", "TITLE") + " from ORGENTITY");
                            break;
                        case Objective.JOB_TYP:
                            _table.Columns["JOB_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                            _table.Columns["JOB_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                            _table.Columns["JOB_ID"].ExtendedProperties["In"] = _db.getDataTable("select id," + _db.langAttrName("JOB", "TITLE") + " from JOBPERSONV");
                            break;

                        case Objective.PERSON_TYP:
                            _table.Columns["PERSON_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                            _table.Columns["PERSON_ID"].ExtendedProperties["InputControlType"] = typeof(Label);
                            _table.Columns["PERSON_ID"].ExtendedProperties["In"] = _db.Person.getWholeNameMATable(true);
                            break;
                    }

                }
                else if (_action == "copy")
                {
                    DataTable table = null;
                    long oeId = _db.Objective.getOE(_id);
                    _table.Columns["STATE"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                    _table.Columns["STATE"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    _table.Columns["STATE"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "state2", false)); ;
                    switch (_typ)
                    {
                        case Objective.UNDEFINED_TYP:
                            _table.Columns["TYP"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                            break;
                        case Objective.ORGENTITY_TYP:
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                            if (oeId > 0)
                            {
                                string sql = "select ID," + _db.langAttrName("ORGENTITY", "TITLE") + " from ORGENTITY where PARENT_ID=" + oeId + " order by " + _db.langAttrName("ORGENTITY", "TITLE");
                                table = _db.getDataTable(sql);
                            }
                            if (table != null && table.Rows.Count == 0) redirect();
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["In"] = table;
                            _table.Columns["ORGENTITY_ID"].ExtendedProperties["Nullable"] = false;
                            break;
                        case Objective.JOB_TYP:
                            _table.Columns["JOB_ID"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.ADD + (int)DBColumn.Visibility.EDIT;
                            _table.Columns["JOB_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                            if (oeId > 0)
                            {
                                string join = "(JOB left join (employment emp inner join person pers on emp.person_id = pers.id) on job.employment_id = emp.id) inner join ORGENTITY OE on JOB.ORGENTITY_ID = OE.ID";
                                string name = "isnull(pers.pname,'vakant')+' '+isnull(pers.firstname,'')+', '+JOB." + _db.langAttrName("JOB", "TITLE") + "+' '+OE." + _db.langAttrName("ORGENTITY", "TITLE") + " NAME";
                                string sql = "(select JOB.ID," + name + " from " + join + " where OE.PARENT_ID=" + oeId;
                                sql += " union ";
                                sql += "select JOB.ID," + name + " from " + join + " where OE.ID=" + oeId + " and (JOB.TYP = 0 or EMP.PERSON_ID = " + _db.userId + ")) order by NAME";
                                table = _db.getDataTable(sql);
                            }
                            if (table != null && table.Rows.Count == 0) redirect();
                            _table.Columns["JOB_ID"].ExtendedProperties["In"] = table;
                            _table.Columns["JOB_ID"].ExtendedProperties["Nullable"] = false;
                            break;
                    }
                }
                else redirect();
                if (!_db.Objective.workflowEnable) _table.Columns["STATE"].ExtendedProperties["Visibility"] = (int)DBColumn.Visibility.INVISIBLE;

                //if (_db.isColumnVisible(DBColumn.Visibility.ADD, _table, "TYP", "OBJECTIVE_OE"))
                //{
                //    _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(DropDownList);
                //    _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ3", true));
                //}

                //if (_db.isColumnVisible(DBColumn.Visibility.ADD, _table, "TYP", "OBJECTIVE_PERSON"))
                //{
                //    _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(DropDownList);
                //    _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ", true));
                //}

                _table.Columns["TYP"].ExtendedProperties["InputControlType"] = typeof(DropDownList);
                _table.Columns["TYP"].ExtendedProperties["In"] = new ArrayList(_mapper.getEnum("mbo", "typ4", true));
               

                DataTable flags = _db.getDataTable("select secondary_objective_id from secondary_objective where objective_id=" + _id);
                bool first = true;
                foreach (DataRow row in flags.Rows)
                {
                    if (!first) _checkFlags += ",";
                    _checkFlags += row[0];
                    first = false;
                }
                base.View = "OBJECTIVE_OE";
                //hide unwanted fields
                _table.Columns["CRITICALDAYS"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["CRITICALVALUE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["TASKLIST"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                base.CheckOrder = true;
                base.LoadInput(_db, _table, detailTab);
            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally
            {
                _db.disconnect();
            }
        }

        private DataTable perstab()
        {
            //check rights
            long accessorID = SessionData.getUserAccessorID(Session);
            string accessorSQL = _db.getAccessorIDsSQLInClause(accessorID);
            DataTable tblJobs = _db.getDataTableExt("select distinct JOB.ID from JOB inner join ORGENTITY on JOB.ORGENTITY_ID=ORGENTITY.ID inner join ORGANISATION on ORGANISATION.ORGENTITY_ID=ORGENTITY.ROOT_ID and ORGANISATION.MAINORGANISATION=1 inner join ACCESS_RIGHT_RT on ACCESS_RIGHT_RT.TABLENAME='JOB' and (ACCESS_RIGHT_RT.ROW_ID=JOB.ID or ACCESS_RIGHT_RT.ROW_ID=0) and ACCESS_RIGHT_RT.APPLICATION_RIGHT = 60 and (ACCESS_RIGHT_RT.AUTHORISATION&4)=4 and ACCESS_RIGHT_RT.ACCESSOR_ID  " + accessorSQL, new object[0]);
            string jobsSQL = "IN (";
            bool start = true;
            foreach (DataRow aktJob in tblJobs.Rows)
            {
                if (start == true)
                {
                    start = false;
                }
                else
                {
                    jobsSQL += ", ";
                }
                jobsSQL += aktJob["ID"];
            }
            jobsSQL += ")";

            //list employees
            return  _db.getDataTableExt("SELECT DISTINCT PersonenID AS PERSON_ID,[Name] + ' ' + Vorname + ', ' + [Bezeichnung Job] AS NAME FROM Rep_Stellenerwartungen WHERE JobID " + jobsSQL + " ORDER BY [Name]", new object[0]);

        }
        private void redirect()
        {
            if (_backURL == "")
            {
                if (_contextId == 0)
                {
                    _contextId = Convert.ToInt32( Request.QueryString["personid"]);
                    _context = "PERSON";
                }

                _backURL = Global.Config.baseURL + "/MbO/Detail.aspx?view=detail&context=" + _context + "&contextId=" + _contextId + "&id=" + _id;
            }
            Response.Redirect(_backURL, false);
        }


        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r)
        {
            if (col != null && col.ColumnName == "PARENT_ID")
            {
               // if (_action == "edit") r.Cells[1].Text = _db.lookup("title", "objective", "id=" + DBColumn.GetValid(row["PARENT_ID"], 0L), false);
               // else if (_id > 0) r.Cells[1].Text = _db.lookup("title", "objective", "id=" + _id, false);
            }
        }

        //private DataTable getOrgObjectives(DBData db)
        //{
        //    string turnId = db.lookup("Wert", "PROPERTY", "Gruppe = 'mbo' and Title = 'turn'").ToString();
        //    return db.getDataTable("select * from OBJECTIVE where TYP = 1 and OBJECTIVE_TURN_ID = " + turnId);
        //}


        private void applyClick(object sender, System.EventArgs e)
        {
            if (base.checkInputValue(_table, detailTab))
            {
                _db.connect();
                _db.beginTransaction();
                string standardMeasuramentTyp = Global.Config.getModuleParam("mbo", "standardMeasuramentTyp", "");
                string standardMeasuramentTypId = "";
                if (String.Compare("",standardMeasuramentTyp) < 0)
                {
                    standardMeasuramentTypId = _db.lookup("ID", "MEASUREMENT_TYPE", "TITLE_DE = '" + standardMeasuramentTyp + "'").ToString();
                }

                try
                {
                    if (_action == "copy" || _action == "replace")
                    {
                        base._inputBuilder.inputType = InputMaskBuilder.InputType.Add;
                    }
                    StringBuilder sql = base.getSql(_table, detailTab, true);

                    if (!_db.Objective.workflowEnable) base.extendSql(sql, _table, "state", (int)Objective.State.ACCEPTED);

                    if (_action == "replace")
                    {
                        long newId = _db.newId("OBJECTIVE");
                        base.extendSql(sql, _table, "ID", newId);
                        base.extendSql(sql, _table, "PARENT_ID", _table.Rows[0]["PARENT_ID"]);
                        base.extendSql(sql, _table, "ROOT_ID", _table.Rows[0]["ROOT_ID"]);
                        base.extendSql(sql, _table, "ORGENTITY_ID", _table.Rows[0]["ORGENTITY_ID"]);
                        base.extendSql(sql, _table, "JOB_ID", _table.Rows[0]["JOB_ID"]);
                        base.extendSql(sql, _table, "PERSON_ID", _table.Rows[0]["PERSON_ID"]);
                        base.extendSql(sql, _table, "PROJECT_ID", _table.Rows[0]["PROJECT_ID"]);
                        _db.execute("update objective set state=" + ((int)Objective.State.NOTACTIVE) + " where id=" + _id);
                        _id = newId;
                    }
                    else if (_action == "add" || _action == "copy")
                    {
                        long newId = _db.newId("OBJECTIVE");
                        base.extendSql(sql, _table, "id", newId);
                        switch (_typ)
                        {
                            case Objective.ORGANISATION_TYP:
                                base.extendSql(sql, _table, "measurement_type_id", standardMeasuramentTypId);
                                base.extendSql(sql, _table, "targetvalue", "100");
                                base.extendSql(sql, _table, "typ", "1");
                                break;
                            case Objective.ORGENTITY_TYP:
                            case Objective.JOB_TYP:
                            case Objective.UNDEFINED_TYP:
                                if (_id > 0)
                                {
                                    long rootId = DBColumn.GetValid(_db.lookup("root_id", "objective", "id=" + _id), 0L);
                                    base.extendSql(sql, _table, "parent_id", _id);
                                    base.extendSql(sql, _table, "root_id", rootId);
                                }
                                else
                                {
                                    base.extendSql(sql, _table, "parent_id", null);
                                    base.extendSql(sql, _table, "root_id", newId);
                                }
                                break;
                            case Objective.PERSON_TYP:
                                base.extendSql(sql, _table, "measurement_type_id", standardMeasuramentTypId);
                                base.extendSql(sql, _table, "targetvalue", "100");
                                base.extendSql(sql, _table, "typ", "5");
                                base.extendSql(sql, _table, "person_id", Request.QueryString["personid"].ToString());
                                //base.extendSql(sql, _table, "ORGENTITY_ID", _table.Rows[0]["ORGENTITY_ID"]);
                                //base.extendSql(sql, _table, "parent_id", null);
                                //base.extendSql(sql, _table, "root_id", null);
                                //if (_backURL == "") _backURL = Global.Config.baseURL + "/MbO/Detail.aspx?context=PERSON&contextId="+_contextId+"&id="+_id;
                                break;
                            case Objective.PROJECT_TYP:
                                base.extendSql(sql, _table, "parent_id", null);
                                base.extendSql(sql, _table, "root_id", null);
                                base.extendSql(sql, _table, "project_id", _contextId);
                                //if (_backURL == "") _backURL = Global.Config.baseURL + "/MbO/Detail.aspx?context=PROJECT&contextId="+_contextId+"&id="+_id;
                                break;
                            default:
                                return;
                        }
                        _id = newId;
                    }
                    string s = base.endExtendSql(sql);
                    if (s.Length > 0)
                    {
                        _db.execute(s);
                        
                        long orgentityId = (long)_db.lookup("ORGENTITY.ID", "PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID "
                                                                +"INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID",
                                                                "(JOB.HAUPTFUNKTION = 1) AND (PERSON.ID = "+ (long)_db.lookup("PERSON_ID","OBJECTIVE","ID = "+ _id) +")");
                        _db.execute("UPDATE OBJECTIVE SET ORGENTITY_ID = "+ orgentityId + " WHERE ID = " + _id);

                        _db.commit();
                        redirect();
                    }
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
            }
        }

        private void mapControls()
        {
            apply.Click += new System.EventHandler(this.applyClick);
            apply.Attributes.Add("onclick", "javascript: if (typeof(getCheckFlags) == 'function') getCheckFlags('" + checkFld.ClientID + "');");
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

        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}

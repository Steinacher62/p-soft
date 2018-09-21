namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.appl.psoft.Training;
    using ch.appl.psoft.Training.Controls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Collections;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Web.UI.WebControls;
    using Telerik.Web.UI;

    /// <summary>
    ///		Summary description for EmploymentRatingEditView.
    /// </summary>
    public partial class EmploymentRatingEditViewFoamPartner : PSOFTInputViewUserControl
    {
        public const string PARAM_RATING_ID = "PARAM_RATING_ID";
        public const string PARAM_EMPLOYMENT_ID = "PARAM_EMPLOYMENT_ID";
        public const string PARAM_RATING_TYPE_SELF = "PARAM_RATING_TYPE_SELF";
        public const string PARAM_CRITERIA_ID = "PARAM_CRITERIA_ID";
        public const string PARAM_JOB_EXPACTATION_ID = "PARAM_JOB_EXPACTATION_ID";
        public const string PARAM_ORDERCOLUMN = "PARAM_ORDERCOLUMN";
        public const string PARAM_ORDERDIR = "PARAM_ORDERDIR";
        public const string PARAM_RATINGITEM_ID = "PARAM_RATINGITEM_ID";
        public const string PARAM_TRAINING_ID = "PARAM_TRAINING_ID";


        private DBData _db = null;
        private DataTable _ratingLevels = null;
        private DataTable _ratingItems = null;
        private DataTable _rating = null;
        private LanguageMapper _map; 
        private string totComment = " ";
        protected ArrayList _dropDownListList = new ArrayList();
        protected string _updateRatingSQL = "";
        protected string _updateArgumentsSQL = "";
        protected string _updateMeasureSQL = " ";
        private Interface.DBObjects.Tree _dbTree = null;

        #region Properities
        public long EmploymentRatingID
        {
            get { return GetLong(PARAM_RATING_ID); }
            set { SetParam(PARAM_RATING_ID, value); }
        }

        public long CriteriaID
        {
            get { return GetLong(PARAM_CRITERIA_ID); }
            set { SetParam(PARAM_CRITERIA_ID, value); }
        }

        public long EmploymentID
        {
            get { return GetLong(PARAM_EMPLOYMENT_ID); }
            set { SetParam(PARAM_EMPLOYMENT_ID, value); }
        }

        public bool RatingTypeSelf
        {
            get { return GetBool(PARAM_RATING_TYPE_SELF); }
            set { SetParam(PARAM_RATING_TYPE_SELF, value); }
        }

        public string OrderColumn
        {
            get { return GetString(PARAM_ORDERCOLUMN); }
            set { SetParam(PARAM_ORDERCOLUMN, value); }
        }

        public string OrderDir
        {
            get { return GetString(PARAM_ORDERDIR); }
            set { SetParam(PARAM_ORDERDIR, value); }
        }

        public long RatingItemId
        {
            get { return GetLong(PARAM_RATINGITEM_ID); }
            set { SetParam(PARAM_RATINGITEM_ID, value); }
        }

        public long TrainingId
        {
            get { return GetLong(PARAM_TRAINING_ID); }
            set { SetParam(PARAM_TRAINING_ID, value); }
        }


        #endregion

        private long _performanceRatingItem = -1;

        public static string Path
        {
            get { return Global.Config.baseURL + "/Performance/Controls/EmploymentRatingEditViewFoamPartner.ascx"; }
        }


        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }


        protected override void DoExecute()
        {
            base.DoExecute();

            if (!IsPostBack)
            {
                apply.Text = _mapper.get("apply");
                AddMesure.Text = "Entwicklungsmassnahme hinzufügen";
                TITLE_VALUE.Text = _mapper.get("");//_mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING);

            }

            _db = DBData.getDBData(Session);
            try
            {
                _db.connect();
                _map = LanguageMapper.getLanguageMapper(Session);
                _ratingLevels = _db.Performance.getPerformanceRatingLevels();
                _ratingItems = _db.Performance.getEmploymentRatingViewTable(EmploymentRatingID, CriteriaID, RatingItemId);
                _rating = _db.getDataTable("SELECT * FROM PERFORMANCERATING WHERE ID = " + EmploymentRatingID);


                if (_ratingItems.Rows.Count == 1)
                {
                    _performanceRatingItem = DBColumn.GetValid(_ratingItems.Rows[0]["ID"], -1L);
                }



                InputType = InputMaskBuilder.InputType.Edit;
                ////_ratingItems.Columns["LEVEL_REF"].ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                _ratingItems.Columns["LEVEL_REF"].ExtendedProperties["InputControlType"] = typeof(DropDownList);
                _ratingItems.Columns["LEVEL_REF"].ExtendedProperties["In"] = _ratingLevels;

                //if(_ratingItems.Rows.Count == 0)
                //{
                //    _ratingItems = _db.Performance.getEmploymentRatingViewTableFirstItem(EmploymentRatingID);
                //    CriteriaID = (long)_ratingItems.Rows[0]["ID"];
                //    CriteriaID = (long)_ratingItems.Rows[0]["CRITERIA_REF"];
                //    _performanceRatingItem = DBColumn.GetValid(_ratingItems.Rows[0]["ID"], -1L);
                //}

                

                if (_ratingItems.Rows.Count > 0 && ! Page.IsPostBack)
                {
                    TITLE_VALUE.Text = _ratingItems.Rows[0][_db.langAttrName("PERFORMANCERATING_ITEMS_V", "CRITERIA_TITLE")].ToString();
                    Color titleBackgroundColor = Color.FromArgb((Int32)_db.lookup("COLOR", "PERFORMANCE_CRITERIA", "TITLE_DE= '" + _ratingItems.Rows[0]["CRITERIA_TITLE_DE"].ToString() + "'"));
                    titleBackgroundColor = Color.FromArgb(255, titleBackgroundColor);
                    titleCell.BackColor = titleBackgroundColor;

                    RadTab clarificationTab = new RadTab();
                    clarificationTab.PageViewID = "clarificationView";
                    RadTab clarificationLeadershipTab = new RadTab();
                    clarificationLeadershipTab.PageViewID = "clarificationLeadershipView";
                    RadTab developmentMeasuresTab = new RadTab();
                    developmentMeasuresTab.PageViewID = "developmentMeasuresView";

                    DataTable helpTexts = new DataTable();
                    helpTexts = _db.getDataTable("SELECT TITLE_" + _mapper.LanguageCode + ", DESCRIPTION_" + _mapper.LanguageCode + ", DESCRIPTION_LEADERSHIP_" + _mapper.LanguageCode + ", QUESTIONS_" + _mapper.LanguageCode + ", QUESTIONS_LEADERSHIP_" + _mapper.LanguageCode + ",POSSIBLE_ACTION_" + _mapper.LanguageCode + " FROM JOB_EXPECTATION_DEFAULT WHERE CRITERIA_REF=" + _ratingItems.Rows[0]["CRITERIA_REF"].ToString());

                    clarificationText.Text = helpTexts.Rows[0]["DESCRIPTION_" + _mapper.LanguageCode].ToString();
                    questionText.Text = helpTexts.Rows[0]["QUESTIONS_" + _mapper.LanguageCode].ToString();
                    clarificationLeadershipText.Text = helpTexts.Rows[0]["DESCRIPTION_LEADERSHIP_" + _mapper.LanguageCode].ToString();
                    questionLeadershipText.Text = helpTexts.Rows[0]["QUESTIONS_LEADERSHIP_" + _mapper.LanguageCode].ToString();
                    developmentMeasuresText.Text = helpTexts.Rows[0]["POSSIBLE_ACTION_" + _mapper.LanguageCode].ToString();
                    developmentMeasuresText.Visible = true;

                    clarificationTab.Text = "Für Mitarbeitende";
                    developmentMeasuresTab.Text = "Entwicklungsmassnahmen";
                    clarificationLeadershipTab.Text = "Für Mitarbeitende mit Führungsaufgaben";
                    TabStripExpectations.Tabs.Add(clarificationTab);
                    TabStripExpectations.Tabs.Add(clarificationLeadershipTab);
                    TabStripExpectations.MultiPageID = "MultiPage";
                    TabStripExpectations.SelectedIndex = 0;
                    if(Session["SelectedTab"]== null || Session["SelectedTab"].Equals("Mitarbeiter"))
                    {
                        TabStripExpectations.SelectedIndex = 0;
                        clarificationText.Visible = true;
                        clarificationView.Selected = true;
                    }
                    else
                    {
                        TabStripExpectations.SelectedIndex = 1;
                        clarificationLeadershipText.Visible = true;
                        clarificationLeadershipView.Selected = true;
                    }
                    

                    clarificationTitle.Text = "Präzisierung";
                    questionTitle.Text = "Mögliche Fragen";
                    clarificationTitleLeader.Text = "Präzisierung";
                    questionTitleLeader.Text = "Mögliche Fragen";
                    develpomentTittle.Text = "Entwicklungs- massnahmen";
                    if (helpTexts.Rows[0]["TITLE_DE"].ToString().Equals("Persönliche Entwicklung"))
                    {
                        potentialTitle.Text = "Potenzialeinschätzung";
                        potentialTitle.Visible = true;
                        potentialList.DataSource = _db.getDataTable("SELECT TITLE_" + _mapper.LanguageCode.ToString() + " AS TITLE,LEVEL FROM PERFORMANCE_POTENTIAL_LEVEL");
                        potentialList.DataTextField = "TITLE";
                        potentialList.DataValueField = "LEVEL";

                        string potentialValue = _rating.Rows[0]["POTENTIAL_LEVEL"].ToString();
                        potentialList.DataBind();
                        if (!Page.IsPostBack)
                        {
                            if (!potentialValue.Equals(""))
                            {
                                potentialList.SelectedValue = potentialValue;
                            }
                            else
                            {
                                potentialList.Items.Add(new ListItem("", "0", true));
                                potentialList.SelectedValue = "0";
                            }
                        }
                        potentialList.Visible = true;
                    }
                    else
                    {
                        potentialTitle.Visible = false;
                        potentialList.Visible = false;
                    }


                    ErrorWindow.Title = "Fehler";
                    missingReasonText.Text = "Bewertung kann nicht ohne Begründung gespeichert weerden!";

                    _dbTree = DBData.getDBData(Session).Tree("TRAININGGROUP");
                    TrainingCatalogTree.LeafNodeUrl = "EmploymentRating.aspx?performanceRatingID=" + EmploymentRatingID + "&employmentID=" + EmploymentID + "&criteriaID=" + CriteriaID + "&RatingItemID=" + RatingItemId + "&mode=edit&type=leader&orderColumn=" + OrderColumn + "&orderDir=" + OrderDir + "&trainingID=%ID";
                    TrainingCatalogTree = (TrainingCatalogTreeCtrl)LoadPSOFTControl(TrainingCatalogTreeCtrl.Path, "_tree");
                }

                //_ratingItems.Columns[_db.langAttrName("PERFORMANCERATING_ITEMS_V", "CRITERIA_TITLE")].ExtendedProperties["InputControlType"] = typeof(Label);
                //_ratingItems.Columns[_db.langAttrName("PERFORMANCERATING_ITEMS_V", "EXPECTATION_DESCRIPTION")].ExtendedProperties["InputControlType"] = typeof(TabStripExpectation);
                //_ratingItems.Columns[_db.langAttrName("PERFORMANCERATING_ITEMS_V", "DESCRIPTION")].ExtendedProperties["InputControlType"] = typeof(Label);
                _ratingItems.Columns["CRITERIA_TITLE_DE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["CRITERIA_TITLE_EN"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["CRITERIA_TITLE_FR"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["EXPECTATION_DESCRIPTION_DE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["EXPECTATION_DESCRIPTION_EN"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["EXPECTATION_DESCRIPTION_FR"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["DESCRIPTION_DE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["DESCRIPTION_EN"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _ratingItems.Columns["DESCRIPTION_FR"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;

                if (!PerformanceModule.showMeasure)
                {
                    _ratingItems.Columns["MEASURE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                }

                if (PerformanceModule.showGlobalComment)
                {
                    DataColumn GLOBAL_COMMENT = new DataColumn();
                    foreach (DictionaryEntry prop in _ratingItems.Columns["MEASURE"].ExtendedProperties)
                    {
                        GLOBAL_COMMENT.ExtendedProperties.Add(prop.Key, prop.Value);
                    }
                    GLOBAL_COMMENT.ExtendedProperties["Visibility"] = DBColumn.Visibility.EDIT;
                    GLOBAL_COMMENT.ExtendedProperties["OrdNum"] = 9;
                    GLOBAL_COMMENT.MaxLength = 2000;
                    GLOBAL_COMMENT.ColumnName = "GLOBAL_COMMENT";
                    GLOBAL_COMMENT.Caption = "GLOBAL_COMMENT";

                    _ratingItems.Columns.Add(GLOBAL_COMMENT);
                    if (_ratingItems.Rows.Count > 0)
                    {
                        _ratingItems.Rows[0]["GLOBAL_COMMENT"] = _rating.Rows[0]["GLOBAL_COMMENT"].ToString();
                    }

                }

                base.CheckOrder = true;
                LoadInput(_db, _ratingItems, listTab);

                apply.Visible = _ratingItems.Rows.Count > 0;//!isFirst;
                AddMesure.Visible = _ratingItems.Rows.Count > 0;
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

        private void setTrainigFromCatalog(int TrainigId)
        {
            DataTable training = _db.getDataTable("SELECT * FROM TRAINING WHERE ID=" + TrainigId);
            designation.Text = training.Rows[0]["TITLE_" + _mapper.LanguageCode].ToString();
            description.Text = training.Rows[0]["DESCRIPTION_" + _mapper.LanguageCode].ToString();
            costExtern.Text = training.Rows[0]["COST_EXTERNAL"].ToString();
            costIntern.Text = training.Rows[0]["COST_INTERNAL"].ToString();
            courseLocation.Text = training.Rows[0]["LOCATION"].ToString();
            courseleader.Text = training.Rows[0]["INSTRUCTOR"].ToString();
        }

        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r)
        {
            if (col != null)
            {
                if (col.ColumnName.IndexOf("CRITERIA_TITLE_") >= 0)
                {
                    r.Cells[1].CssClass = "Detail_Label";
                }
                switch (col.ColumnName.ToUpper())
                {
                    case "LEVEL_REF":
                        if (r.Cells[1].Controls[0].GetType().Name == "DropDownList")
                        {
                            DropDownList rb = (DropDownList)r.Cells[1].Controls[0];
                            RadLabel missingRating = new RadLabel();
                            missingRating.Text = "Benötigt!";
                            missingRating.ID = "missingRating";
                            r.Cells[1].Controls.Add(missingRating);
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        protected override void onBuildSQL(StringBuilder build, System.Web.UI.Control control, DataColumn col, object val)
        {
            if (col.ColumnName == "LEVEL_REF")
            {
                _updateRatingSQL = "update PERFORMANCERATING_ITEMS";
                if (val.Equals(string.Empty))
                {
                    _updateRatingSQL += " set RELATIV_WEIGHT=-1, LEVEL_REF = null";
                    _updateRatingSQL += "," + _db.langExpand("LEVEL_TITLE%LANG%='-'", "PERFORMANCERATING_ITEMS", "LEVEL_TITLE");
                    _updateRatingSQL += "," + _db.langExpand("LEVEL_DESCRIPTION%LANG%='-'", "PERFORMANCERATING_ITEMS", "LEVEL_DESCRIPTION");
                    _updateRatingSQL += " where";
                }
                else
                {
                    _updateRatingSQL += " set PERFORMANCERATING_ITEMS.RELATIV_WEIGHT=PERFORMANCE_LEVEL.RELATIV_WEIGHT, PERFORMANCERATING_ITEMS.LEVEL_REF=" + val.ToString();
                    _updateRatingSQL += "," + _db.langExpand("PERFORMANCERATING_ITEMS.LEVEL_TITLE%LANG%=PERFORMANCE_LEVEL.TITLE%LANG%", "PERFORMANCERATING_ITEMS", "LEVEL_TITLE");
                    _updateRatingSQL += "," + _db.langExpand("PERFORMANCERATING_ITEMS.LEVEL_DESCRIPTION%LANG%=PERFORMANCE_LEVEL.DESCRIPTION%LANG%", "PERFORMANCERATING_ITEMS", "LEVEL_DESCRIPTION");
                    _updateRatingSQL += " from PERFORMANCERATING_ITEMS, PERFORMANCE_LEVEL where PERFORMANCE_LEVEL.ID=" + val + " and";
                }
                _updateRatingSQL += " PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF=" + EmploymentRatingID;
                _updateRatingSQL += " and PERFORMANCERATING_ITEMS.CRITERIA_REF=" + CriteriaID;
                if (RatingItemId > 0)
                {
                    _updateRatingSQL += " and PERFORMANCERATING_ITEMS.ID =" + RatingItemId;
                }

            }
            else if (col.ColumnName == "ARGUMENTS")
            {
                //convert string to sql string / 24.08.10 / mkr
                string strVal = val.ToString();
                strVal = strVal.Replace("'", "''");

                _updateArgumentsSQL = "update PERFORMANCERATING_ARGUMENTS";
                _updateArgumentsSQL += " set ARGUMENTS='" + strVal + "'";
                _updateArgumentsSQL += " where PERFORMANCERATING_REF=" + EmploymentRatingID;
                _updateArgumentsSQL += " and PERFORMANCERATING_CRITERIA_REF=" + CriteriaID;

                if (RatingItemId > 0)
                {
                    _updateArgumentsSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID=" + RatingItemId;
                }
                else
                {
                    _updateArgumentsSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID is null";
                }
            }
            if (col.ColumnName == "MEASURE")
            {
                string strVal = val.ToString();
                strVal = strVal.Replace("'", "''");

                _updateMeasureSQL = "update PERFORMANCERATING_ARGUMENTS";
                _updateMeasureSQL += " set MEASURE='" + strVal + "'";
                _updateMeasureSQL += " where PERFORMANCERATING_REF=" + EmploymentRatingID;
                _updateMeasureSQL += " and PERFORMANCERATING_CRITERIA_REF=" + CriteriaID;

                if (RatingItemId > 0)
                {
                    _updateMeasureSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID=" + RatingItemId;
                }
                else
                {
                    _updateMeasureSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID is null";
                }
            }

            if (col.ColumnName == "GLOBAL_COMMENT")
            {
                totComment = val.ToString().Replace("'", "''"); ;
            }

        }
        protected void apply_Click(object sender, System.EventArgs e)
        {
            _db.connect();
            try
            {
                if (checkInputValue(_ratingItems, listTab))
                {
                    _db.beginTransaction();

                    string sql = "";
                    StringBuilder sb = base.getSql(_ratingItems, listTab, true);

                    sql = base.endExtendSql(sb);

                    if (_updateRatingSQL != "")
                        _db.execute(_updateRatingSQL);

                    if (_updateArgumentsSQL != "")
                        _db.execute(_updateArgumentsSQL);
                    if (_updateMeasureSQL != "")
                        _db.execute(_updateMeasureSQL);

                    _db.commit();
                    if (!totComment.Equals(" "))
                    {
                        _db.execute("UPDATE PERFORMANCERATING SET GLOBAL_COMMENT ='" + totComment + "' WHERE ID =" + EmploymentRatingID);
                    }
                    if (Session["potlevel"] != null)
                    {
                        _db.execute("UPDATE PERFORMANCERATING SET POTENTIAL_LEVEL = " + Session["potlevel"] + " WHERE ID =" + EmploymentRatingID);
                        Session.Remove("potlevel");
                    }
                    if (Session["SelectedTab"].Equals("Vorgesetzter"))
                    {
                        _db.execute("UPDATE PERFORMANCERATING_ITEMS SET  EXPECTATION_DESCRIPTION_DE = (SELECT DESCRIPTION_LEADERSHIP_DE FROM JOB_EXPECTATION_DEFAULT WHERE CRITERIA_REF =" + _ratingItems.Rows[0]["CRITERIA_REF"].ToString() + ") WHERE ID = "+ _ratingItems.Rows[0]["ID"]);

                    }

                }
            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally
            {
                _db.disconnect();
                Response.Redirect(Request.Url.AbsoluteUri);
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
        }

        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        protected void potentialList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList levelList = (DropDownList)sender;
            Session.Add("potlevel",levelList.SelectedValue);
        }

        protected void TrainigWindow_Load(object sender, EventArgs e)
        {
            if (_ratingItems.Rows.Count > 0)
            {
                formtitle.Text = _map.get("TRAINING_ADVANCEMENT", "TRAINING");
                designationTitle.Text = _map.get("TRAINING_ADVANCEMENT", "TITLE_DE");
                descriptionTitle.Text = _map.get("TRAINING_ADVANCEMENT", "DESCRIPTION_DE");
                toBedoneDateTitle.Text = _map.get("TRAINING_ADVANCEMENT", "TOBEDONE_DATE");
                controllingTitle.Text = _map.get("TRAINING_ADVANCEMENT", "CONTROLLING_DE");
                costExternTitle.Text = _map.get("TRAINING_ADVANCEMENT", "COST_EXTERNAL");
                costInternTitle.Text = _map.get("TRAINING_ADVANCEMENT", "COST_INTERNAL");
                courseLocationTitle.Text = _map.get("TRAINING_ADVANCEMENT", "LOCATION");
                courseleaderTitle.Text = _map.get("TRAINING_ADVANCEMENT", "INSTRUCTOR");
                responsibileTitle.Text = _map.get("TRAINING_ADVANCEMENT", "RESPONSIBLE_PERSON_ID");
                costSharingTitle.Text = _map.get("TRAINING_ADVANCEMENT", "COST_PARTICIPATION");
                obligationTitle.Text = _map.get("TRAINING_ADVANCEMENT", "OBLIGATION");
                trainigneedsTitle.Text = _map.get("TRAINING_ADVANCEMENT", "TRAINING_DEMAND_ID");
                stateTitle.Text = _map.get("TRAINING_ADVANCEMENT", "STATE");
                _db.connect();

                
                responsibile.DataSource = _db.getDataTable("SELECT ID, PNAME + ' ' + FIRSTNAME AS NAME FROM PERSON WHERE TYP = 1 AND LEAVING IS NULL ORDER BY PNAME");
                responsibile.DataTextField = "NAME";
                responsibile.DataValueField = "ID";
                responsibile.DataBind();
                responsibile.Items.Insert(0, "");

                trainigneeds.DataSource = _db.getDataTable("SELECT ID, TITLE_" + _mapper.LanguageCode + " AS TITLE FROM TRAINING_DEMAND ORDER BY TITLE_" + _mapper.LanguageCode);
                trainigneeds.DataTextField = "TITLE";
                trainigneeds.DataValueField = "ID";
                trainigneeds.DataBind();
                trainigneeds.Items.Insert(0, "");
                DropDownListItem done = new DropDownListItem();
                DropDownListItem undone = new DropDownListItem();
                undone.Value = "0";
                undone.Text = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true)[0];
                done.Value = "1";
                done.Text = _mapper.getEnum(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_STATE, true)[1];
                state.Items.Add(undone);
                state.Items.Add(done);

                long trainingAdventsmentRef = _db.lookup("TRAINING_ADVANCEMENT_REF", "PERFORMANCERATING_ITEMS", "ID=" + _ratingItems.Rows[0]["ID"], 0);
                if (trainingAdventsmentRef > 0 && !Page.IsPostBack)
                {
                    DataTable trainingAdventsment = _db.getDataTable("SELECT * FROM TRAINING_ADVANCEMENT WHERE ID = " + trainingAdventsmentRef);
                    designation.Text = trainingAdventsment.Rows[0]["TITLE_" + _mapper.LanguageCode].ToString();
                    description.Text = trainingAdventsment.Rows[0]["DESCRIPTION_" + _mapper.LanguageCode].ToString();
                    toBedoneDate.DbSelectedDate = (DateTime)trainingAdventsment.Rows[0]["TOBEDONE_DATE"];
                    controlling.Text = trainingAdventsment.Rows[0]["CONTROLLING_" + _mapper.LanguageCode].ToString();
                    costExtern.Value = (double)trainingAdventsment.Rows[0]["COST_EXTERNAL"];
                    costIntern.Value = (double)trainingAdventsment.Rows[0]["COST_INTERNAL"];
                    courseLocation.Text = trainingAdventsment.Rows[0]["LOCATION"].ToString();
                    courseleader.Text = trainingAdventsment.Rows[0]["INSTRUCTOR"].ToString();
                    responsibile.SelectedValue = trainingAdventsment.Rows[0]["RESPONSIBLE_PERSON_ID"].ToString();
                    costSharing.Value = (double)trainingAdventsment.Rows[0]["COST_PARTICIPATION"];
                    if (trainingAdventsment.Rows[0]["OBLIGATION"].ToString().Length > 0)
                    {
                        obligation.DbSelectedDate = (DateTime)trainingAdventsment.Rows[0]["OBLIGATION"];
                    }
                    trainigneeds.SelectedValue = trainingAdventsment.Rows[0]["TRAINING_DEMAND_ID"].ToString();
                    state.SelectedValue= trainingAdventsment.Rows[0]["STATE"].ToString();
                }

                if (!string.IsNullOrEmpty(Request.Params["trainingID"]) && !Page.IsPostBack)
                {
                    setTrainigFromCatalog(Convert.ToInt32(Request.Params["trainingID"]));
                    TrainigWindow.VisibleOnPageLoad = true;
                }
                else
                {
                    TrainigWindow.VisibleOnPageLoad = false;
                }

                saveTraining.Text = "Speichern";
                cancel.Text = "Abbrechen";

                _db.disconnect();
            }
        }

        protected void saveTraining_Click(object sender, EventArgs e)
        {
            _db.connect();
            long trainingAdventsmentRef = _db.lookup("TRAINING_ADVANCEMENT_REF", "PERFORMANCERATING_ITEMS", "ID=" + _ratingItems.Rows[0]["ID"], 0L);

            if(trainingAdventsmentRef == 0) //add 
            {
                string sql = "INSERT INTO TRAINING_ADVANCEMENT (TITLE_" + _mapper.LanguageCode + ", DESCRIPTION_" + _mapper.LanguageCode + ", TOBEDONE_DATE, CONTROLLING_" + _mapper.LanguageCode +
                             ", COST_EXTERNAL, COST_INTERNAL, LOCATION, INSTRUCTOR, RESPONSIBLE_PERSON_ID, COST_PARTICIPATION, ";
                if(!obligation.DateInput.Text.Equals(""))
                {
                    sql += "OBLIGATION, ";
                }
                string trainigneedsVal = trainigneeds.SelectedText;
                if (trainigneedsVal.Length > 0)
                {
                    sql += "TRAINING_DEMAND_ID, ";
                }
                sql += " STATE, PERSON_ID) VALUES ('" + designation.Text.Replace("'", "''") + "', '" + description.Text.Replace("'", "''") + "', '" + Convert.ToDateTime(toBedoneDate.SelectedDate).ToString("MM/dd/yyyy") + "', '" + controlling.Text.Replace("'", "''") +
                            "', '" + costExtern.Value + "', '" + costIntern.Value + "', '" + courseLocation.Text.Replace("'", "''") + "', '" + courseleader.Text.Replace("'", "''") + "', '" + responsibile.SelectedValue +
                            "', '" + costSharing.Value + "', ";
                if (!obligation.DateInput.Text.Equals(""))
                {
                    sql += "'" + Convert.ToDateTime(obligation.SelectedDate).ToString("MM/dd/yyyy")+ "', ";
                }
                
                if(trainigneedsVal.Length > 0)
                {
                    sql += "'" + trainigneeds.SelectedValue + "', ";
                }
                sql += "'" + state.SelectedValue + "', '" + _rating.Rows[0]["PERSON_ID"] + "')";
                _db.execute(sql);

                _db.execute("UPDATE PERFORMANCERATING_ITEMS SET TRAINING_ADVANCEMENT_REF = (SELECT MAX(ID) FROM TRAINING_ADVANCEMENT) WHERE ID = " + _ratingItems.Rows[0]["ID"]);
            }

            if (trainingAdventsmentRef > 0) //update
            {
                string sql = "UPDATE TRAINING_ADVANCEMENT SET TITLE_" + _mapper.LanguageCode + " = '" + designation.Text.Replace("'", "''") + "', "
                                    + "DESCRIPTION_" + _mapper.LanguageCode + " = '" + description.Text.Replace("'", "''") + "', "
                                    + "TOBEDONE_DATE = '" + Convert.ToDateTime(toBedoneDate.SelectedDate).ToString("MM/dd/yyyy") + "', "
                                    + "CONTROLLING_" + _mapper.LanguageCode + " = '" + controlling.Text.Replace("'", "''") + "', "
                                    + "COST_EXTERNAL = '" + costExtern.Value + "', "
                                    + "COST_INTERNAL = '" + costIntern.Value + "', "
                                    + "LOCATION = '" + courseLocation.Text.Replace("'", "''") + "', "
                                    + "INSTRUCTOR  ='" + courseleader.Text.Replace("'", "''") + "', "
                                    + "RESPONSIBLE_PERSON_ID = '" + responsibile.SelectedValue + "', "
                                    + "COST_PARTICIPATION = '" + costSharing.Value + "'";
                if (obligation.SelectedDate.ToString().Length > 0)
                {
                    sql += ", OBLIGATION = '" + Convert.ToDateTime(obligation.SelectedDate).ToString("MM/dd/yyyy") + "'";
                }
                if (trainigneeds.SelectedText.Length > 0)
                {
                    sql += ", TRAINING_DEMAND_ID = '" + trainigneeds.SelectedValue + "'";
                }
                sql += ", STATE = '" + state.SelectedValue + "' WHERE ID = " + trainingAdventsmentRef;
                _db.execute(sql);
            }


                _db.disconnect();
        }

        protected void TabStripExpectations_TabClick(object sender, RadTabStripEventArgs e)
        {
            if (e.Tab.ID.Equals("i0")){
                Session.Add("SelectedTab", "Mitarbeiter");
            }
            if (e.Tab.ID.Equals("i1")){
                Session.Add("SelectedTab", "Vorgesetzter");
            }
        }
    }
}

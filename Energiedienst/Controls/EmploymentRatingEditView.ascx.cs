namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using db;
    using System;
    using System.Collections;
    using System.Data;
    using System.Net.Mail;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for EmploymentRatingEditView.
    /// </summary>
    public partial  class EmploymentRatingEditView : PSOFTInputViewUserControl
	{
		public const string PARAM_RATING_ID = "PARAM_RATING_ID";
		public const string PARAM_EMPLOYMENT_ID = "PARAM_EMPLOYMENT_ID";
		public const string PARAM_RATING_TYPE_SELF = "PARAM_RATING_TYPE_SELF";
        public const string PARAM_ORDERCOLUMN = "PARAM_ORDERCOLUMN";
        public const string PARAM_ORDERDIR = "PARAM_ORDERDIR";



		private DBData _db = null;
		private DataTable _ratingItems = null;
		protected ArrayList _dropDownListList = new ArrayList();
		protected string _updateRatingSQL = "";
		protected string _updateArgumentsSQL = "";
        protected string _updateMeasureSQL = "";

        public string ratingId;
        public ArrayList ratingList;
        public ArrayList commentList;
        public HtmlTextArea bottomCommentTextArea;
        public HtmlTextArea yearTextArea;
        public CheckBox employeeConfirmBox;
        public CheckBox interviewDoneBox;

		#region Properities
		public long EmploymentRatingID 
		{
			get {return GetLong(PARAM_RATING_ID);}
			set {SetParam(PARAM_RATING_ID, value);}
		}

		public long EmploymentID 
		{
			get {return GetLong(PARAM_EMPLOYMENT_ID);}
			set {SetParam(PARAM_EMPLOYMENT_ID, value);}
		}

		public bool RatingTypeSelf 
		{
			get {return GetBool(PARAM_RATING_TYPE_SELF);}
			set {SetParam(PARAM_RATING_TYPE_SELF, value);}
		}

        public string OrderColumn {
            get {return GetString(PARAM_ORDERCOLUMN);}
            set {SetParam(PARAM_ORDERCOLUMN, value);}
        }

        public string OrderDir {
            get {return GetString(PARAM_ORDERDIR);}
            set {SetParam(PARAM_ORDERDIR, value);}
        }

	
		#endregion

        private long _performanceRatingItem = -1;

		public static string Path 
		{
			get {return Global.Config.baseURL + "/Energiedienst/Controls/EmploymentRatingEditView.ascx";}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{


            Execute();

                
                //PsoftPageLayout pageLayout = PageLayoutControl as PsoftPageLayout;
                //pageLayout.ButtonPrintAttributes.Add("onClick",
                //    "javascript: window.open('ObjectiveReport.aspx?context=" + _context + "&contextId=" + _contextId + "&turnid=" + turnid + "','ObjectiveReport')");
                //pageLayout.ButtonPrintToolTip = _mapper.get("mbo", "personReportTP");
                //pageLayout.ButtonPrintVisible = _context == Objective.PERSON && _view == "detail";
            
           
		}


		protected override void DoExecute() 
		{
			base.DoExecute ();

			if (!IsPostBack) 
			{
                apply.Text = "Speichern"; _mapper.get("apply");
                TITLE_VALUE.Text =  _mapper.get("");//_mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_PERFORMANCERATING);
			}

			_db = DBData.getDBData(Session);
			try 
			{
                ratingList = new ArrayList();
                commentList = new ArrayList();
                ratingId = Request.QueryString["performanceRatingID"];
				_db.connect();
                DataTable criterias = _db.getDataTable("SELECT ID , TITLE_DE FROM PERFORMANCE_CRITERIA");
                
                HtmlTable mainTable = new HtmlTable();
                mainTable.ID = "MainTable";
                this.Controls.AddAt(0,mainTable);

                DataTable levels = _db.getDataTable("SELECT ID, TITLE_DE , RELATIV_WEIGHT FROM PERFORMANCE_LEVEL");
                HtmlTable levelTable = new HtmlTable();
                HtmlTableRow[] levelRow= new HtmlTableRow[6];
                HtmlTableCell[] levelCell = new HtmlTableCell[6];
                levelTable.ID = "levelTable";

                for (int i = 0; i < 6; i++)
                {
                    levelRow[i] = new HtmlTableRow();
                    levelTable.Controls.Add(levelRow[i]);
                    levelCell[i] = new HtmlTableCell();
                    levelRow[i].Controls.Add(levelCell[i]);
                    if (i > 0)
                    {
                        levelCell[i].InnerText = levels.Rows[i-1].ItemArray[1].ToString();
                    }
                    else
                    {
                        levelCell[i].InnerText = "Beurteilungsskala";
                    }
                }
                this.Controls.AddAt(0, levelTable);

                var title = new HtmlGenericControl("span");
                title.InnerText = "Kompetenzbeurteilung für ";
                title.Attributes["Class"] = "Yearspan";
                this.Controls.AddAt(1, title);

                HtmlTextArea yearArea = new HtmlTextArea();
                yearArea.InnerText = _db.lookup("PERFORMANCE_YEAR", "PERFORMANCERATING", "ID = '" + ratingId + "'").ToString();
                if (yearArea.InnerText.Equals(""))
                {
                    yearArea.InnerText = (DateTime.Now.Year-1).ToString();
                }
                yearTextArea = yearArea;
                title.Controls.Add(yearArea);

                HtmlTableRow headRow = new HtmlTableRow();
                mainTable.Controls.Add(headRow);
                HtmlTableCell criteriaCellHead = new HtmlTableCell();
                HtmlTableCell commentCellHead = new HtmlTableCell();
                headRow.Controls.Add(criteriaCellHead);
                headRow.Controls.Add(commentCellHead);

                HtmlTable criteriaTableHead = new HtmlTable();
                criteriaCellHead.ID = "criteriaTableHead";
                HtmlTableRow criteriaRowHead = new HtmlTableRow();
                criteriaCellHead.Controls.Add(criteriaTableHead);
                criteriaTableHead.Controls.Add(criteriaRowHead);

                HtmlTableCell[] criteriaHeadCells = new HtmlTableCell[7];
                for (int i = 0; i < 7; i++)
                {
                    criteriaHeadCells[i] = new HtmlTableCell();
                    criteriaRowHead.Controls.Add(criteriaHeadCells[i]);
                    if (i > 1)
                    {
                        criteriaHeadCells[i].Attributes["Class"] = "RatingHeadCell";
                    }
                }
                criteriaHeadCells[2].InnerText = "E";
                criteriaHeadCells[3].InnerText = "D";
                criteriaHeadCells[4].InnerText = "C";
                criteriaHeadCells[5].InnerText = "B";
                criteriaHeadCells[6].InnerText = "A";

                commentCellHead.InnerText = "Kommentar";

                int criterianumber = 0;
                foreach (DataRow criteria in criterias.Rows)
                {
                    if(_db.getDataTable("SELECT ID FROM PERFORMANCERATING_ITEMS WHERE PERFORMANCERATING_REF = '"+ratingId+"' AND CRITERIA_REF = '"+criteria.ItemArray[0]+"'").Rows.Count >0)
                    {
                    HtmlTableRow criteriaRow = new HtmlTableRow();
                    HtmlTableCell criteriaCell = new HtmlTableCell();
                    HtmlTableCell commentCell = new HtmlTableCell();
                    mainTable.Controls.Add(criteriaRow);
                    criteriaRow.Controls.Add(criteriaCell);
                    criteriaRow.Controls.Add(commentCell);

                    long criteriaID = long.Parse(criteria.ItemArray[0].ToString());
                    DataTable itemData = _db.getDataTable("SELECT ID, EXPECTATION_TITLE_DE, EXPECTATION_DESCRIPTION_DE, LEVEL_REF, RELATIV_WEIGHT FROM dbo.PERFORMANCERATING_ITEMS WHERE (PERFORMANCERATING_REF = " + ratingId + ") AND (CRITERIA_REF = " + criteriaID + ")");
                   

                    HtmlTable criteriaTable = new HtmlTable();
                    criteriaTable.Attributes["Class"] = "criteriaTable";

                    HtmlTableRow titleRow = new HtmlTableRow();
                    criteriaTable.Controls.Add(titleRow);
                    titleRow.Attributes["Class"] = "TitleRow";

                    HtmlTableCell[] rowCells = new HtmlTableCell[7];
                    for (int i = 0; i < 7; i++)
                    {
                        rowCells[i] = new HtmlTableCell();
                        titleRow.Controls.Add(rowCells[i]);
                    }

                    rowCells[0].InnerText = new string[]{"A","B","C","D","E"}[criterianumber];
                    rowCells[1].InnerText = criteria.ItemArray[1].ToString();
                    rowCells[0].Attributes["Class"] = "CriteriaHeadCell";
                    rowCells[1].Attributes["Class"] = "CriteriaHeadCell";

                    for (int i = 2; i < 7; i++)
                    {
                        rowCells[i].ID = "rating_" + criterianumber + "_" + (i - 2);
                        rowCells[i].Attributes["Class"] = "TotalBoxCriteria";
                    }

                    int itemNumber = 1;
                    foreach (DataRow item in itemData.Rows)
                    {
                        HtmlTableRow row = new HtmlTableRow();
                        criteriaTable.Controls.Add(row);
                        row.Attributes["Class"] = "defaultRow";

                        HtmlTableCell[] defaultRowCells = new HtmlTableCell[7];
                        for (int i = 0; i < 7; i++)
                        {
                            defaultRowCells[i] = new HtmlTableCell();
                            row.Controls.Add(defaultRowCells[i]);
                        }
                        defaultRowCells[0].InnerText = itemNumber.ToString();
                        
                        HtmlImage infoIcon = new HtmlImage();
                        infoIcon.Src = Global.Config.baseURL + "/images/icon_info.png";
                        infoIcon.Attributes["Class"] = "InfoIcon";
                        infoIcon.Attributes["Desc"] = item.ItemArray[2].ToString();                    
                        defaultRowCells[1].InnerText = item.ItemArray[1].ToString();
                        defaultRowCells[1].Controls.AddAt(0,infoIcon);
                        RadioButton[] radiobuttons = new RadioButton[5];
                        for (int i = 2; i < 7; i++)
                        {
                            RadioButton radioButton = new RadioButton();
                            radioButton.ID = "radioButton_" + criterianumber + "_" + itemNumber + "_" + (i - 2);
                            radioButton.Attributes["Class"] = "Radiobutton";
                            radiobuttons[i-2]=radioButton;
                            radioButton.Attributes["Value"] = levels.Rows[i - 2].ItemArray[0].ToString();
                            radioButton.GroupName = "RadioButtonGroup_" + criterianumber + "_" + itemNumber;
                            defaultRowCells[i].Controls.Add(radioButton);
                        }

                        if (itemData.Rows[itemNumber-1].ItemArray[4].ToString() != "-1")
                        {
                            radiobuttons[(int.Parse(itemData.Rows[itemNumber-1].ItemArray[4].ToString())/20)-1].Checked = true;
                        }
                        Rating rating = new Rating(radiobuttons, itemData.Rows[itemNumber-1].ItemArray[0].ToString());
                        ratingList.Add(rating);
                        itemNumber++;
                        
                    }
                    
                    criteriaCell.Controls.Add(criteriaTable);

                    HtmlTextArea commentArea = new HtmlTextArea();
                    commentArea.Value = _db.lookup("ARGUMENTS", "PERFORMANCERATING_ARGUMENTS", "PERFORMANCERATING_REF = '"+ratingId+"' AND PERFORMANCERATING_CRITERIA_REF = '"+criteriaID+"'").ToString();
                    commentCell.Controls.Add(commentArea);
                    commentCell.Attributes["Class"] = "CommentCell";
                    commentArea.Attributes.CssStyle.Add("height", (itemNumber * 30) + ((itemNumber + 1) * 2) +  "px");
                    criterianumber++;

                    commentList.Add(new Comment(commentArea,criteriaID.ToString()));
                }
                }


                HtmlTableRow TotalRow = new HtmlTableRow();
                mainTable.Controls.Add(TotalRow);
                HtmlTableCell[] TotalCells = new HtmlTableCell[2];
                for (int i = 0; i < 2; i++)
                {
                    TotalCells[i] = new HtmlTableCell();
                    TotalRow.Controls.Add(TotalCells[i]);
                }

                HtmlTable criteriaTotalTable = new HtmlTable();
                criteriaTotalTable.Attributes["Class"] = "criteriaTable";
                TotalCells[0].Controls.Add(criteriaTotalTable);

                HtmlTableRow titleTotalRow = new HtmlTableRow();
                criteriaTotalTable.Controls.Add(titleTotalRow);
                titleTotalRow.Attributes["Class"] = "TitleRow";

                HtmlTableCell[] rowTotalCells = new HtmlTableCell[7];
                for (int i = 0; i < 7; i++)
                {
                    rowTotalCells[i] = new HtmlTableCell();
                    titleTotalRow.Controls.Add(rowTotalCells[i]);
                }

                rowTotalCells[1].InnerText = "Beurteilung der Kompetenz (Verhalten)";
                rowTotalCells[0].Attributes["Class"] = "CriteriaHeadCell";
                rowTotalCells[1].Attributes["Class"] = "CriteriaHeadCell";

                for (int i = 2; i < 7; i++)
                {
                    rowTotalCells[i].ID = "rating_Total_" + (i - 2);
                    rowTotalCells[i].Attributes["Class"] = "TotalBoxCriteriaTotal";
                }

                HtmlTableRow bottomRow = new HtmlTableRow();
                HtmlTableCell bottomComment = new HtmlTableCell();
                HtmlTableCell bottomInfo = new HtmlTableCell();

                mainTable.Controls.Add(bottomRow);
                bottomRow.Controls.Add(bottomComment);
                bottomRow.Controls.Add(bottomInfo);

                HtmlTable bottomCommentTable = new HtmlTable();
                bottomComment.Controls.Add(bottomCommentTable);
                bottomCommentTable.Attributes["Class"] = "BottomCommentTable";
                HtmlTableRow[] bottomCommentRows = new HtmlTableRow[2];
                HtmlTableCell[] bottomCommentCells = new HtmlTableCell[2];

                for (int i = 0; i < 2;  i++)
                {
                    bottomCommentRows[i] = new HtmlTableRow();
                    bottomCommentCells[i] = new HtmlTableCell();
                    bottomCommentRows[i].Controls.Add(bottomCommentCells[i]);
                    bottomCommentTable.Controls.Add(bottomCommentRows[i]);
                }
                bottomCommentCells[0].InnerText = "Bemerkungen";
                HtmlTextArea bottomCommentArea = new HtmlTextArea();
                bottomCommentArea.Value = _db.lookup("ARGUMENTS", "PERFORMANCERATING", "ID = '" + ratingId + "'").ToString();
                bottomCommentCells[1].Controls.Add(bottomCommentArea);
                bottomCommentCells[1].Attributes["Class"] = "CommentCell";
                bottomCommentTextArea = bottomCommentArea;


                CheckBox interviewDone = new CheckBox();
                interviewDone.ID = "inteviewDone";
                bottomInfo.Controls.Add(interviewDone);
                interviewDone.Text = "Gespräch geführt";
                interviewDone.AutoPostBack = true;
                interviewDoneBox = interviewDone;
                interviewDone.CheckedChanged += new EventHandler(interviewDone_Checked);

             

                CheckBox employeeAccept = new CheckBox();
                employeeAccept.ID = "employeeAccept";
                bottomInfo.Controls.Add(employeeAccept);
                employeeAccept.Text = "Eingesehen von " + _db.lookup("PNAME", "PERSON", "ID = '" + _db.lookup("PERSON_ID", "PERFORMANCERATING", "ID = '" + ratingId + "'").ToString() + "'") + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = '" + _db.lookup("PERSON_ID", "PERFORMANCERATING", "ID = '" + ratingId + "'").ToString() + "'");
                employeeAccept.AutoPostBack = true;
                employeeConfirmBox = employeeAccept;
                employeeAccept.CheckedChanged += new EventHandler(employee_Checked);

                int jobID = int.Parse(_db.lookup("JOB_ID", "PERFORMANCERATING", "ID = '" + ratingId + "'",0L).ToString());

                if (!_db.lookup("INTERVIEW_DONE", "PERFORMANCERATING", "ID='" + ratingId + "'").ToString().Equals(""))
                {
                    interviewDone.Enabled = false;
                    interviewDone.Checked = true;
                }
                else
                {
                    employeeAccept.Enabled = false;
                }

                if (!_db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))
                {
                    interviewDone.Enabled = false;
                }

                var descDiv = new HtmlGenericControl("div");
                descDiv.Attributes["Class"] = "descDiv";
                this.Controls.Add(descDiv);

                // AUTHORISATION

                if ((_db.lookup("DATUM_WERT", "PROPERTY", "Gruppe = 'performance' and TITLE = 'lock'").ToString() != "" && DateTime.Parse(_db.lookup("DATUM_WERT", "PROPERTY", "Gruppe = 'performance' and TITLE = 'lock'").ToString()) > DateTime.Parse(_db.lookup("RATING_DATE", "PERFORMANCERATING", "ID='" + ratingId + "'").ToString())) || !_db.lookup("INTERVIEW_DONE", "PERFORMANCERATING", "ID='" + ratingId + "'").ToString().Equals("") || !_db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", jobID, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true))
                {
                    foreach (Rating rating in ratingList)
                    {
                        foreach (RadioButton radioB in rating.radioButtonList)
                        {
                            radioB.Enabled = false;
                        }
                    }

                    foreach (Comment comment in commentList)
                    {
                        comment.commentArea.Disabled = true;
                    }

                    bottomCommentArea.Disabled = true;
                    yearArea.Disabled = true;
                    apply.Enabled = false;

                    
                }

                if (!_db.userId.ToString().Equals( _db.lookup("PERSON_ID", "PERFORMANCERATING", "ID = '" + ratingId + "'").ToString()))
                {
                    employeeAccept.Enabled = false;
                }

                // viewed already checked?
                if (!_db.lookup("viewed", "PERFORMANCERATING", "ID='" + ratingId + "'").ToString().Equals(""))
                {
                    employeeAccept.Checked = true;
                    employeeAccept.Enabled = false;
                }

                if (employeeAccept.Enabled)
                {
                    bottomCommentArea.Disabled = false;
                }
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

		protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) 
		{
			if (col != null)
			{
				if(col.ColumnName.IndexOf("CRITERIA_TITLE_") >= 0)
				{
					r.Cells[1].CssClass = "Detail_Label";
				}
				switch (col.ColumnName.ToUpper())
				{						
					case "LEVEL_REF":
						RadioButtonList rb = (RadioButtonList) r.Cells[1].Controls[0];
						rb.RepeatDirection = System.Web.UI.WebControls.RepeatDirection.Vertical;
						break;
					default:
						break;
				}
			}
		}


		protected override void onBuildSQL (StringBuilder build, System.Web.UI.Control control, DataColumn col, object val) 
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
					_updateRatingSQL += " set PERFORMANCERATING_ITEMS.RELATIV_WEIGHT=PERFORMANCE_LEVEL.RELATIV_WEIGHT, PERFORMANCERATING_ITEMS.LEVEL_REF="+val.ToString();
					_updateRatingSQL += "," + _db.langExpand("PERFORMANCERATING_ITEMS.LEVEL_TITLE%LANG%=PERFORMANCE_LEVEL.TITLE%LANG%", "PERFORMANCERATING_ITEMS", "LEVEL_TITLE");
					_updateRatingSQL += "," + _db.langExpand("PERFORMANCERATING_ITEMS.LEVEL_DESCRIPTION%LANG%=PERFORMANCE_LEVEL.DESCRIPTION%LANG%", "PERFORMANCERATING_ITEMS", "LEVEL_DESCRIPTION");
					_updateRatingSQL += " from PERFORMANCERATING_ITEMS, PERFORMANCE_LEVEL where PERFORMANCE_LEVEL.ID=" + val + " and";
				}		
				_updateRatingSQL += " PERFORMANCERATING_ITEMS.PERFORMANCERATING_REF=" + EmploymentRatingID;
				//_updateRatingSQL += " and PERFORMANCERATING_ITEMS.CRITERIA_REF=" + CriteriaID;
                //if(RatingItemId > 0)
                //{
                //    _updateRatingSQL += " and PERFORMANCERATING_ITEMS.ID =" + RatingItemId;
                //}
				
			}
            else if (col.ColumnName == "ARGUMENTS")
			{
				//convert string to sql string / 24.08.10 / mkr
                string strVal = val.ToString();
                strVal = strVal.Replace("'", "''");
                
                _updateArgumentsSQL = "update PERFORMANCERATING_ARGUMENTS";
                _updateArgumentsSQL += " set ARGUMENTS='" + strVal + "'";
				_updateArgumentsSQL += " where PERFORMANCERATING_REF=" + EmploymentRatingID;
				//_updateArgumentsSQL += " and PERFORMANCERATING_CRITERIA_REF=" + CriteriaID;
				
                //if(RatingItemId > 0)
                //{
                //    _updateArgumentsSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID=" + RatingItemId;
                //}
                //else
                //{
                //    _updateArgumentsSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID is null";
                //}
			}
            if (col.ColumnName == "MEASURE")
            {
                string strVal = val.ToString();
                strVal = strVal.Replace("'", "''");
                
                _updateMeasureSQL = "update PERFORMANCERATING_ARGUMENTS";
                _updateMeasureSQL += " set MEASURE='" + strVal + "'";
				_updateMeasureSQL += " where PERFORMANCERATING_REF=" + EmploymentRatingID;
				//_updateMeasureSQL += " and PERFORMANCERATING_CRITERIA_REF=" + CriteriaID;
				
                //if(RatingItemId > 0)
                //{
                //    _updateMeasureSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID=" + RatingItemId;
                //}
                //else
                //{
                //    _updateMeasureSQL += " and PERFORMANCERATING_ARGUMENTS.PERFORMANCERATING_ITEM_ID is null";
                //}
            }
		}

        protected void employee_Checked(object sender, System.EventArgs e) {
            employeeConfirmBox.Enabled = false;
            DateTime time = System.DateTime.Now;
            _db.connect();
            _db.execute("UPDATE PERFORMANCERATING SET viewed = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID ='" + ratingId + "'");

            string toID = _db.lookup("RATING_PERSON_REF", "PERFORMANCERATING", "ID =" + ratingId).ToString();
            _db.disconnect();
            sendMail("Statusänderung Kompetenzbeurteilung", "<font face=\"Arial\" size=\"3\">Der Status der Kompetenzbeurteilung wurde von sender auf 'Eingesehen' geändert.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">https://srv132/p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>",toID);
        }

        protected void interviewDone_Checked(object sender, System.EventArgs e)
        {
            interviewDoneBox.Visible = false;
            
            DateTime time = System.DateTime.Now;
            _db.connect();
            _db.execute("UPDATE PERFORMANCERATING SET Interview_done = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID ='" + ratingId + "'");
            _db.execute("UPDATE PERFORMANCERATING SET RATING_PERSON_REF = '" + _db.userId + "' WHERE ID ='" + ratingId + "'");
            string toID = _db.lookup("PERSON_ID", "PERFORMANCERATING", "ID =" + ratingId).ToString();
            _db.disconnect();

            sendMail("Statusänderung Kompetenzbeurteilung", "<font face=\"Arial\" size=\"3\">Der Status Ihrer Kompetenzbeurteilung wurde von sender auf 'Gespräch geführt' geändert.<br><br>Bitte überprüfen Sie Ihre Kompetenzbeurteilung und visieren Sie diese mit einem Klick auf 'Eingesehen von receiver'.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">https://srv132/p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>",toID);

        }

        protected void sendMail(string subject, string message, string toID)
        {
            _db.connect();
            string _personId = _db.lookup("PERSON_ID", "PERFORMANCERATING", "ID = " + ratingId).ToString();

            MailMessage myMessage = new MailMessage();
            try
            {

            myMessage.From = new MailAddress(_db.lookup("EMAIL", "PERSON", "ID = " + _db.userId).ToString(), _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId,"").ToString());
            myMessage.To.Add(_db.lookup("EMAIL", "PERSON", "ID = " + toID).ToString());
                        }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

            string senderName = _db.lookup("FIRSTNAME", "PERSON", "ID = " + _db.userId).ToString() + " " + _db.lookup("PNAME", "PERSON", "ID = " + _db.userId).ToString();
            string receiverName = _db.lookup("PNAME", "PERSON", "ID = " + _personId).ToString() + " " + _db.lookup("FIRSTNAME", "PERSON", "ID = " + _personId).ToString();
            message = message.Replace("sender", senderName);
            message = message.Replace("receiver", receiverName);

            myMessage.Body = message;
            SmtpClient mySmtpClient = new SmtpClient();
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(Global.Config.getModuleParam("dispatch", "UserName", ""), Global.Config.getModuleParam("dispatch", "passwordFrom", ""));
            mySmtpClient.Host = Global.Config.getModuleParam("dispatch", "smtpServer", "");
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Credentials = myCredential;
            mySmtpClient.ServicePoint.MaxIdleTime = 1;

            try
            {
                mySmtpClient.Send(myMessage);
                ClientScriptManager cs = Page.ClientScript;
                cs.RegisterStartupScript(this.GetType(), "myalert", "alert('Eine e-Mail Benachrichtigung mit der Statusänderung wurde erfolgreich versandt.'); window.location = \"" + Request.Url.AbsoluteUri + "\";", true);
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }

            myMessage.Dispose();
            _db.disconnect();
        }


		protected void apply_Click(object sender, System.EventArgs e) 
		{
            bool inTransaction = false;
			_db.connect();
			try 
			{
                foreach (object ratingObject in ratingList)
                {
                    Rating rating = (Rating)ratingObject;
                    int index = -1;
                    for (int i = 0; i < 5; i++)
                    {
                        if (rating.radioButtonList[i].Checked)
                        {
                            index = i;
                            break;
                        }
                    }
                    if(index != -1){
                    _db.execute("UPDATE PERFORMANCERATING_ITEMS SET RELATIV_WEIGHT = '"+(index+1)*20+"' , LEVEL_REF = '"+(rating.radioButtonList[index].Attributes["Value"])+"' WHERE ID = '"+rating.itemId+"'");
                    }
                }

                foreach (object commentObject in commentList)
                {
                    Comment comment = (Comment)commentObject;
                    _db.execute("UPDATE PERFORMANCERATING_ARGUMENTS SET ARGUMENTS = '" + comment.commentArea.Value + "' WHERE PERFORMANCERATING_REF = '" + ratingId + "' AND PERFORMANCERATING_CRITERIA_REF = '" + comment.criteriaId + "'");
                }

                _db.execute("UPDATE PERFORMANCERATING SET PERFORMANCE_YEAR = '"+yearTextArea.InnerText+"', ARGUMENTS = '"+bottomCommentTextArea.Value+"' WHERE ID = '"+ratingId+"'");
                Button button = (Button)sender;
                button.Text = "Speichern";
                
            }
			catch (Exception ex) 
			{
                if (inTransaction){
                    _db.rollback();
                }
				DoOnException(ex);
			}
			finally 
			{
				_db.disconnect();   
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
	}

   public class Rating
{
       public RadioButton[] radioButtonList;
       public String itemId;
       public Rating(RadioButton[] radioButtons, string id)
        {
            radioButtonList = radioButtons;
            itemId = id;
        }
}


   public class Comment
{
       public HtmlTextArea commentArea;
       public String criteriaId;
       public Comment(HtmlTextArea textArea, string id)
        {
            commentArea = textArea;
            criteriaId = id;
        }
}
}

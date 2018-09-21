namespace ch.appl.psoft.Energiedienst.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using db;
    using System;
    using System.Data;
    using System.Net.Mail;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;



    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class DetailView : PSOFTDetailViewUserControl {
        protected long _id = 0;
        protected string _backURL = "";
        protected string _deleteDetailMessage = "";
        protected string _deleteDetailURL = "";

        public CheckBox employeeConfirmBox;
        public Button publishButton;
        public Button saveButton;
        public HtmlTextArea[] textBoxes;
        public DataTable objectives;
        public String personId;
        public String turnId;
        public DataTable ratings;
        public TableCell cell;
        public HtmlGenericControl alertMsg;

        public static string Path {
            get {return Global.Config.baseURL + "/Energiedienst/Controls/DetailView.ascx";}
        }

		#region Properities

        /// <summary>
        /// Get/Set current id
        /// </summary>
        public long id {
            get {return _id;}
            set {
                _id = value;
            }
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
             
            DBData db = DBData.getDBData(Session);

            db.connect();

            personId = Request.QueryString["contextId"];
            turnId = Request.QueryString["turnId"];
            if (turnId == null)
            {
                turnId = db.lookup("WERT", "PROPERTY", "TITLE = 'turn'").ToString();
            }
            TableRow tableRow = new TableRow();
            cell = new TableCell();
            detailTab.Controls.AddAt(0, tableRow);
            alertMsg = new HtmlGenericControl("div");
            alertMsg.Attributes["id"] = "alertMsg";
            
            cell.Controls.AddAt(0, alertMsg);
            alertMsg.Visible = false;
            tableRow.Controls.Add(cell);
            textBoxes = new HtmlTextArea[5 * 11];
            int textareaCounter = 0;

            HtmlGenericControl titleDiv = new HtmlGenericControl("div");
            titleDiv.Attributes["id"] = "ratingTitle";
            cell.Controls.AddAt(1, titleDiv);

            HtmlTable infoTable = new HtmlTable();
            cell.Controls.Add(infoTable);
            infoTable.Attributes["id"]= "infoTable";
           
            //Objectives
            ratings = new DataTable();
            ratings.Columns.Add("ID");
            ratings.Columns.Add("RATING");
            ratings.Columns.Add("RATING_WEIGHT");

            objectives = db.getDataTable("SELECT * FROM OBJECTIVE WHERE PERSON_ID = "+personId+" AND OBJECTIVE_TURN_ID = "+turnId);
            if (objectives.Rows.Count < 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    string sql = "INSERT INTO OBJECTIVE (TITLE , MEASUREMENT_TYPE_ID, TARGETVALUE, STARTDATE, STATE, TYP, FLAG, CREATOR, PERSON_ID ,OBJECTIVE_TURN_ID) ";
                    sql += "VALUES (";
                    if(i == 0)
                    sql += " ' ','0','100','" + System.DateTime.Now.ToString("MM.dd.yyyy") + "','3','5','65536','" + db.userId.ToString() + "','" + personId + "','" + turnId + "')";
                    else
                        sql += " ' ','0','100',NULL,'3','5','65536','" + db.userId.ToString() + "','" + personId + "','" + turnId + "')";
                    db.execute(sql);
                    objectives = db.getDataTable("SELECT * FROM OBJECTIVE WHERE PERSON_ID = " + personId + " AND OBJECTIVE_TURN_ID = " + turnId);
                    sql = "INSERT INTO OBJECTIVE_PERSON_RATING (ID,PERSON_ID, OBJECTIVE_ID,RATING, RATING_WEIGHT) VALUES ('" + objectives.Rows[i]["ID"] + "','" + personId + "','" + objectives.Rows[i]["ID"] + "','" + 0 + "','" + 0 + "')";
                    db.execute(sql);
                }
                
            }
            int ratingTot = 0;
            foreach (DataRow row in objectives.Rows)
            {
                DataRow rating = ratings.NewRow();
                rating["ID"] = db.lookup("ID", "OBJECTIVE_PERSON_RATING", "OBJECTIVE_ID = '" + row["ID"] + "'");
                rating["RATING"] = db.lookup("RATING", "OBJECTIVE_PERSON_RATING", "OBJECTIVE_ID = '" + row["ID"] + "'");
                if (!(rating["RATING"].ToString().Equals("")))
                {
                    ratingTot += Convert.ToInt32(rating["RATING"]);
                }
                rating["RATING_WEIGHT"] = db.lookup("RATING_WEIGHT", "OBJECTIVE_PERSON_RATING", "OBJECTIVE_ID = '" + row["ID"] + "'");
                ratings.Rows.Add(rating);
            }




            if (objectives.Rows.Count < 5)
            {
                for (int i = objectives.Rows.Count; i < 5; i++ )
                {
                    DataRow emptyRow   = objectives.NewRow();
                    emptyRow["ID"] = -i; // marks empty row
                    emptyRow["TITLE"] = " ";
                    emptyRow["MEASUREMENT_TYPE_ID"] = objectives.Rows[0]["MEASUREMENT_TYPE_ID"];
                    emptyRow["TARGETVALUE"] = "100";
                    emptyRow["STATE"] = "3";
                    emptyRow["TYP"] = "5";
                    emptyRow["FLAG"] = "65536";
                    emptyRow["CREATOR"] = objectives.Rows[0]["CREATOR"];
                   objectives.Rows.Add(emptyRow);
                   DataRow rating = ratings.NewRow();
                   rating["ID"] = "-1";
                   rating["RATING"] = "0";
                   rating["RATING_WEIGHT"] = "0";
                   ratings.Rows.Add(rating);
                }
            }


            if ((db.lookup("TOP 1 VIEWED", "OBJECTIVE", "PERSON_ID = " + personId + " AND OBJECTIVE_TURN_ID = " + turnId).ToString() == "")&& ratingTot == 0)
            {
                titleDiv.InnerText = "Zielvereinbarung " + db.lookup("TITLE_DE", "OBJECTIVE_TURN", "ID =" + turnId);
            }
            else
            {
                titleDiv.InnerText = "Zielerreichung " + db.lookup("TITLE_DE", "OBJECTIVE_TURN", "ID =" + turnId);
            }

           
            HtmlTableRow infoRow = new HtmlTableRow();
            infoTable.Controls.Add(infoRow);
            HtmlTableCell jobCell = new HtmlTableCell();
            infoRow.Controls.Add(jobCell);
            jobCell.InnerText = db.lookup("JOB.TITLE_DE", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + objectives.Rows[0]["PERSON_ID"].ToString() + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)","").ToString();
            HtmlTableCell nameCell = new HtmlTableCell();
            infoRow.Controls.Add(nameCell);
            nameCell.InnerText = db.lookup("FIRSTNAME", "PERSON", "ID = " + objectives.Rows[0]["PERSON_ID"].ToString()) + " " + db.lookup("PNAME", "PERSON", "ID = " + objectives.Rows[0]["PERSON_ID"].ToString());
            HtmlTableCell dateCell = new HtmlTableCell();
            infoRow.Controls.Add(dateCell);
            if (objectives.Rows[0]["STARTDATE"].ToString().Remove(10).Equals(objectives.Rows[0]["STATEDATE"].ToString().Remove(10)))
            {
                dateCell.InnerText = objectives.Rows[0]["STARTDATE"].ToString().Remove(10);
            }
            else
            {
                dateCell.InnerText = objectives.Rows[0]["STARTDATE"].ToString().Remove(10) + " / " + objectives.Rows[0]["STATEDATE"].ToString().Remove(10);
            }



            HtmlTable objectiveTable = new HtmlTable();
            objectiveTable.Attributes["id"] = "objectiveTable";

            cell.Controls.Add(objectiveTable);
            HtmlTableRow titleRow = new HtmlTableRow();
            objectiveTable.Controls.Add(titleRow);

            HtmlTableCell[] titleCells = new HtmlTableCell[11];
            for (int i = 0; i < 11; i++)
            {
                titleCells[i] = new HtmlTableCell();
                string title = "";
                switch (i)
                {
                    case 0:
                        title = "Nr.";
                        break;
                    case 1:
                        title = "Strategie- Bezug";
                        break;
                    case 2:
                        title = "Zielbeschreibung / Aufgabe";
                        break;
                    case 3:
                        title = "Massnahmen";
                        break;
                    case 4:
                        title = "Termin";
                        break;
                    case 5:
                        title = "nächstes Etappen- gespräch";
                        break;
                    case 6:
                        title = "Erfolgskriterien / Messgrösse";
                        break;
                    case 7:
                        title = "Aufwand / Nutzen";
                        break;
                    case 8:
                        title = "Eventuell negative Folgen";
                        break;
                    case 9:
                        title = "Gewichtung %";
                        break;
                    case 10:
                        title = "Ziel- erreichung %";
                        break;
                    default:
                        break;
                }
                titleCells[i].InnerText = title;
                titleRow.Controls.Add(titleCells[i]);
            }

            for(int rownumber = 0; rownumber<objectives.Rows.Count; rownumber++)
            {
                HtmlTableRow objectiveRow = new HtmlTableRow();
                objectiveTable.Controls.Add(objectiveRow);
                
                HtmlTableCell[] objectiveCells = new HtmlTableCell[11];
                for (int i = 0; i < 11; i++)
                {
                    objectiveCells[i] = new HtmlTableCell();
                    textBoxes[textareaCounter] = new HtmlTextArea();
                    objectiveCells[i].Controls.Add(textBoxes[textareaCounter]);
                    string text = "";
                    switch (i)
                    {
                        case 0:
                            text = objectives.Rows[rownumber]["NUMBER"].ToString();
                            break;
                        case 1:
                            text = objectives.Rows[rownumber]["STRATEGIE_RELATION"].ToString();
                            break;
                        case 2:
                            text = objectives.Rows[rownumber]["DESCRIPTION"].ToString();
                            break;
                        case 3:
                            text = objectives.Rows[rownumber]["ACTIONNEED"].ToString();
                            break;
                        case 4:
                            text = objectives.Rows[rownumber]["TERMIN"].ToString();
                            break;
                        case 5:
                            text = objectives.Rows[rownumber]["ARGUMENT"].ToString();
                            break;
                        case 6:
                            text = objectives.Rows[rownumber]["MEASUREKRIT"].ToString();
                            break;
                        case 7:
                            text = objectives.Rows[rownumber]["MEMO"].ToString();
                            break;
                        case 8:
                            text = objectives.Rows[rownumber]["TASKLIST"].ToString();
                            break;
                        case 10:
                            text = ratings.Rows[rownumber]["RATING"].ToString();
                            textBoxes[textareaCounter].Attributes["Class"] = "RatingBox";
                            break;
                        case 9:
                            text = ratings.Rows[rownumber]["RATING_WEIGHT"].ToString();
                            textBoxes[textareaCounter].Attributes["Class"] = "WeightBox";
                            break;

                        default:
                            break;
                    }
                    textBoxes[textareaCounter].InnerText = text;
                    objectiveRow.Controls.Add(objectiveCells[i]);
                    textareaCounter++;
                }
            }

            employeeConfirmBox = new CheckBox();
            publishButton = new Button();


            //
            HtmlGenericControl sumDiv = new HtmlGenericControl("div");
            sumDiv.Attributes["id"] = "sumDiv";
            cell.Controls.Add(sumDiv);

            HtmlTable sums = new HtmlTable();
            sumDiv.Controls.Add(sums);

            HtmlTableRow sumRow = new HtmlTableRow();
            sums.Controls.Add(sumRow);

            HtmlTableCell weightSum = new HtmlTableCell();
            sumRow.Controls.Add(weightSum);
            weightSum.Attributes["id"] = "weightSum";

            HtmlTableCell ratingSum = new HtmlTableCell();
            sumRow.Controls.Add(ratingSum);
            ratingSum.Attributes["id"] = "ratingSum";

            HtmlTable optionTable = new HtmlTable();
            optionTable.Attributes["id"] = "optionTable";
            cell.Controls.Add(optionTable);
            HtmlTableRow optionRow = new HtmlTableRow();
            optionTable.Controls.Add(optionRow);
            HtmlTableCell optionCell1 = new HtmlTableCell();
            optionRow.Controls.Add(optionCell1);
            HtmlTableCell optionCell2 = new HtmlTableCell();
            optionRow.Controls.Add(optionCell2);
            HtmlTableCell optionCell3 = new HtmlTableCell();
            optionRow.Controls.Add(optionCell3);

            saveButton = new Button();
            saveButton.Text = "Speichern";
            saveButton.Click += new EventHandler(apply_Click);
            optionCell1.Controls.Add(saveButton);
            
            publishButton.ID = "inteviewDone";
            optionCell2.Controls.Add(publishButton);
            publishButton.Text = "Freigeben";
            publishButton.Click += new EventHandler(interviewDone_Checked);

            //VIEWED
            employeeConfirmBox.ID = "employeeAccept";
            optionCell3.Controls.Add(employeeConfirmBox);
            employeeConfirmBox.Text = "Von "+ db.Person.getWholeName(personId) +" eingesehen";
            employeeConfirmBox.AutoPostBack = true;
            employeeConfirmBox.CheckedChanged += new EventHandler(employee_Checked);




            // AUTHORISATION       

            if (db.lookup("INTERVIEW_DONE", "OBJECTIVE", "ID='" + objectives.Rows[0]["ID"] + "'").ToString().Equals(""))
            
            {
                employeeConfirmBox.Enabled = false;
            }
            String mainJobId = db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + objectives.Rows[0]["PERSON_ID"].ToString() + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)", 0L).ToString();
            Int32 mainJobId1 = Convert.ToInt32(mainJobId);
            if (!db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", mainJobId1, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
            {
                publishButton.Enabled = false;
                saveButton.Enabled = false;
            }

            if (!db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", mainJobId1, DBData.APPLICATION_RIGHT.MODULE_MBO, true, true))
            {
                foreach (HtmlTextArea textArea in textBoxes)
                {
                    textArea.Disabled = true;
                }
                saveButton.Enabled = false;
            }

            if (!db.userId.ToString().Equals(db.lookup("PERSON_ID", "OBJECTIVE", "ID = '" + objectives.Rows[0]["ID"] + "'").ToString()))
            {
                employeeConfirmBox.Enabled = false;
            }

            // viewed already checked?
            if (!db.lookup("VIEWED", "OBJECTIVE", "ID='" + objectives.Rows[0]["ID"] + "'").ToString().Equals(""))
            {
                employeeConfirmBox.Checked = true;
                employeeConfirmBox.Enabled = false;
            }

            db.disconnect();
            
        }
        protected override void onAddProperty(DataRow row, DataColumn col, TableRow r) {
        }
        protected void apply_Click(object sender, System.EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                if (int.Parse(textBoxes[9].Value) + int.Parse(textBoxes[11 + 9].Value) + int.Parse(textBoxes[2 * 11 + 9].Value) + int.Parse(textBoxes[3 * 11 + 9].Value) + int.Parse(textBoxes[4 * 11 + 9].Value) == 100 || int.Parse(textBoxes[9].Value) + int.Parse(textBoxes[11 + 9].Value) + int.Parse(textBoxes[2 * 11 + 9].Value) + int.Parse(textBoxes[3 * 11 + 9].Value) + int.Parse(textBoxes[4 * 11 + 9].Value) == 0)
                {
                    if (0 <= int.Parse(textBoxes[10].Value) && 0 <= int.Parse(textBoxes[11 + 10].Value) && 0 <= int.Parse(textBoxes[11 * 2 + 10].Value) && 0 <= int.Parse(textBoxes[11 * 3 + 10].Value) && 0 <= int.Parse(textBoxes[11 * 4 + 10].Value) && 0 <= int.Parse(textBoxes[10].Value) && 120 >= int.Parse(textBoxes[10].Value) && 120 >= int.Parse(textBoxes[11 + 10].Value) && 120 >= int.Parse(textBoxes[11 * 2 + 10].Value) && 120 >= int.Parse(textBoxes[11 * 3 + 10].Value) && 120 >= int.Parse(textBoxes[11 * 4 + 10].Value))
                    {
                        for (int rownumber = 0; rownumber < 5; rownumber++)
                        {
                            if (!objectives.Rows[rownumber][0].ToString().StartsWith("-"))
                            {
                                string sql = "UPDATE OBJECTIVE SET ";
                                sql += "NUMBER = '" + textBoxes[rownumber * 11].Value.Replace("'", "''") + "'";
                                sql += ",STRATEGIE_RELATION = '" + textBoxes[rownumber * 11 + 1].Value.Replace("'", "''") + "'";
                                sql += ",DESCRIPTION = '" + textBoxes[rownumber * 11 + 2].Value.Replace("'", "''") + "'";
                                sql += ",ACTIONNEED = '" + textBoxes[rownumber * 11 + 3].Value.Replace("'", "''") + "'";
                                sql += ",TERMIN = '" + textBoxes[rownumber * 11 + 4].Value.Replace("'", "''") + "'";
                                sql += ",ARGUMENT = '" + textBoxes[rownumber * 11 + 5].Value.Replace("'", "''") + "'";
                                sql += ",MEASUREKRIT = '" + textBoxes[rownumber * 11 + 6].Value.Replace("'", "''") + "'";
                                sql += ",MEMO = '" + textBoxes[rownumber * 11 + 7].Value.Replace("'", "''") + "'";
                                sql += ",TASKLIST = '" + textBoxes[rownumber * 11 + 8].Value.Replace("'", "''") + "'";
                                sql += " WHERE ID = '" + objectives.Rows[rownumber][0].ToString().Replace("'", "''") + "'";
                                db.execute(sql);
                                sql = "UPDATE OBJECTIVE_PERSON_RATING SET ";
                                sql += "RATING = '" + textBoxes[rownumber * 11 + 10].Value.Replace("'", "''").Replace(Environment.NewLine, "") + "'";
                                sql += ",RATING_WEIGHT = '" + textBoxes[rownumber * 11 + 9].Value.Replace("'", "''").Replace(Environment.NewLine,"") + "'";
                                sql += " WHERE ID = '" + ratings.Rows[rownumber][0].ToString().Replace("'", "''") + "'";
                                db.execute(sql);
                            }
                            else if (!isRowEmpty(rownumber))
                            {
                                string sql = "INSERT INTO OBJECTIVE (NUMBER, STRATEGIE_RELATION, DESCRIPTION, ACTIONNEED, TERMIN, ARGUMENT, MEASUREKRIT, MEMO,TASKLIST, TITLE , MEASUREMENT_TYPE_ID, TARGETVALUE, STATE, TYP, FLAG, CREATOR, PERSON_ID ,OBJECTIVE_TURN_ID) ";
                                sql += "VALUES ('" + textBoxes[rownumber * 11].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 1].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 2].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 3].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 4].Value + "','" + textBoxes[rownumber * 11 + 5].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 6].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 7].Value.Replace("'", "''") + "','" + textBoxes[rownumber * 11 + 8].Value.Replace("'", "''") + "'";
                                sql += ", '" + objectives.Rows[rownumber]["TITLE"] + "','" + objectives.Rows[rownumber]["MEASUREMENT_TYPE_ID"] + "','" + objectives.Rows[rownumber]["TARGETVALUE"] + "','" + objectives.Rows[rownumber]["STATE"] + "','" + objectives.Rows[rownumber]["TYP"] + "','" + objectives.Rows[rownumber]["FLAG"] + "','" + objectives.Rows[0]["CREATOR"] + "','" + personId + "','" + turnId + "')";
                                db.execute(sql);
                                long objectiveID = (long)db.lookup("MAX(ID)", "OBJECTIVE", "PERSON_ID =" + personId);
                                sql = "INSERT INTO OBJECTIVE_PERSON_RATING (ID,PERSON_ID, OBJECTIVE_ID,RATING, RATING_WEIGHT) VALUES ('" + objectiveID + "','" + personId + "','" + objectiveID + "','" + textBoxes[rownumber * 11 + 10].Value + "','" + textBoxes[rownumber * 11 + 9].Value + "')";
                                db.execute(sql);
                            }
                        }
                        alertMsg.Visible = false;
                    }
                    else
                    {
                        alertMsg.Visible = true;
                        alertMsg.InnerText = "Zielerreichung liegt nicht zwischen 0 und 120.";
                    }
                }
                else
                {
                    alertMsg.Visible = true;
                    alertMsg.InnerText =  "Summe der Gewichtungen ergibt nicht 100";
                }
            }
            catch
            {
                alertMsg.Visible = true;
                alertMsg.InnerText = "ungültige Eingabe in Gewichtung oder Zielerreichung";
            }
            db.disconnect();
        }

        protected void employee_Checked(object sender, System.EventArgs e)
        {
            employeeConfirmBox.Enabled = false;
            DateTime time = System.DateTime.Now;
            DBData db = DBData.getDBData(Session);
            db.connect();
            db.execute("UPDATE OBJECTIVE SET VIEWED = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID ='" + objectives.Rows[0]["ID"] + "'");
            string toID = db.lookup("CREATOR", "OBJECTIVE", "ID =" + objectives.Rows[0]["ID"]).ToString();
            db.disconnect();
            sendMail("Statusänderung Zielvereinbarung", "<font face=\"Arial\" size=\"3\">sender hat Ihre Zielvereinbarung / Zielerreichung visiert.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">https://srv132/p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>", toID);
        }

        protected void interviewDone_Checked(object sender, System.EventArgs e)
        {
            publishButton.Enabled = false;

            DateTime time = System.DateTime.Now;
            DBData db = DBData.getDBData(Session);
            db.connect();
            db.execute("UPDATE OBJECTIVE SET INTERVIEW_DONE = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID ='" + objectives.Rows[0]["ID"] + "'");
            db.execute("UPDATE OBJECTIVE SET STATEDATE = '" + time.ToString("MM.dd.yyyy") + "' WHERE ID ='" + objectives.Rows[0]["ID"] + "'");
            db.execute("UPDATE OBJECTIVE SET VIEWED = NULL WHERE ID ='" + objectives.Rows[0]["ID"] + "'");
            db.execute("UPDATE OBJECTIVE SET CREATOR = " + db.userId + " WHERE OBJECTIVE_TURN_ID ='" + objectives.Rows[0]["OBJECTIVE_TURN_ID"] + "' AND PERSON_ID =" + objectives.Rows[0]["PERSON_ID"]);
           
            string toID = db.lookup("PERSON_ID", "OBJECTIVE", "ID =" + objectives.Rows[0]["ID"]).ToString();
            db.disconnect();
            apply_Click(null,null);

            sendMail("Statusänderung Zielvereinbarung", "<font face=\"Arial\" size=\"3\">Ihre Zielvereinbarung / Zielerreichung wurde von sender geändert.<br><br>Bitte überprüfen Sie die Zielvereinbarung / Zielerreichung und visieren Sie diese.<br><br></font><a href=\"https://srv132/p-flow\"><font face=\"Arial\" size=\"3\">https://srv132/p-flow</font></a><br><br><font face=\"Arial\" size=\"3\">Besten Dank für Ihre Unterstützung.</font>",toID);
        }

        protected void sendMail(string subject, string message, string toID)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
          

            MailMessage myMessage = new MailMessage();
            try
            {
            myMessage.From = new MailAddress(db.lookup("EMAIL", "PERSON", "ID = " + db.userId).ToString(), db.lookup("FIRSTNAME", "PERSON", "ID = " + db.userId).ToString() + " " + db.lookup("PNAME", "PERSON", "ID = " + db.userId,"").ToString());
            myMessage.To.Add(db.lookup("EMAIL", "PERSON", "ID = " + toID).ToString());
                        }
            catch (System.Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            myMessage.Subject = subject;
            myMessage.IsBodyHtml = true;

            string senderName = db.lookup("FIRSTNAME", "PERSON", "ID = " + db.userId).ToString() + " " + db.lookup("PNAME", "PERSON", "ID = " + db.userId).ToString();
            string receiverName = db.lookup("PNAME", "PERSON", "ID = " + personId).ToString() + " " + db.lookup("FIRSTNAME", "PERSON", "ID = " + personId).ToString();
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
        }

        protected Boolean isRowEmpty(int rownumber)
        {
            for (int i = 11 * rownumber; i < (rownumber + 1) * 11; i++)
            {
                if (textBoxes[i].Value != "" && textBoxes[i].Value != "0")
                {
                    return false;
                }
            }
            return true;
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
        private void InitializeComponent() {

        }
		#endregion
    }
}

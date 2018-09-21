namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for JobDescriptionCtrl.
    /// </summary>
    public partial class JobDescriptionCtrl : PSOFTMapperUserControl {
        public const string PARAM_DUTYGROUP_ID = "PARAM_DUTYGROUP_ID";




        protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _competenceLevels = null;
        protected long _funktionID = -1L;
        protected bool _showFunktion = false;
        protected bool _showSalary = false;
        protected long _jobID = -1L;
        protected long _OEID = -1L;


        public static string Path {
            get {return Global.Config.baseURL + "/FBS/Controls/JobDescriptionCtrl.ascx";}
        }

        #region Properities
        public long JobID {
            get {return _jobID;}
            set {
                _jobID = value; 
                if (_jobID > 0) {
                    _funktionID = -1L;
                    _showFunktion = false;
                    _showSalary = false;
                }
            }
        }
        public long FunktionID {
            get {return _funktionID;}
            set {
                _funktionID =  value; 
                if (_funktionID > 0) {
                    _jobID = -1L;
                    _showFunktion = Global.Config.isModuleEnabled("fbw");
                    _showSalary = Global.Config.isModuleEnabled("lohn");;
                }
            }
        }
        public long OEID {
            get {return _OEID;}
            set {_OEID =  value;}
        }
        public string deleteMessage {
            get { return _mapper.get("MESSAGES", "deleteConfirm"); }
        }

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            if (!IsPostBack) {
                CBShowValidDutyCompOnly.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_SHOW_VALID_ONLY);          
                CBShowValidDutyCompOnly.Checked = SessionData.showValidDutyCompOnly(Session);
                CBShowValidDutyCompOnly.Visible = true;
                if (_showFunktion) {
                    fktValueRow.Visible = true;
                    if (!_showSalary) {
                        fktValueRow.Height = (int) fktValueRow.Height.Value*2;
                        fktValueRow.VerticalAlign = VerticalAlign.Middle;
                    }
                    fktValueLbl.Text = _mapper.get("fbs", "functionValue:");
                }
                if (_showSalary) {
                    salaryValueRow.Visible = true;
                    if (!_showFunktion) {
                        salaryValueRow.Height = (int) salaryValueRow.Height.Value*2;
                        salaryValueRow.VerticalAlign = VerticalAlign.Middle;
                    }
                    salaryValueLbl.Text = _mapper.get("fbs", "salaryValue:");
                }
            }
            Execute();
        }

        protected string getRestriction(){
            if (SessionData.showValidDutyCompOnly(Session)) 
                return "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + JobID + " or FUNKTION_ID=" + _funktionID + ")";
            else 
                return "DUTY_VALIDITY_VALID_FROM<=GetDate() and DUTY_VALIDITY_VALID_TO>=GetDate() and (JOB_ID=" + JobID + " or FUNKTION_ID=" + _funktionID + ")";
        }

        protected override void DoExecute() {
            base.DoExecute();
            loadList();
        }

		bool isFirstGrp = true;

        private void loadList() {
            _config = Global.Config;          
            _db = DBData.getDBData(Session);
            competenceList.Rows.Clear();
            
            try {
                _db.connect();

                if (!IsPostBack) {
                    CompetenceListTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS,FBSModule.LANG_MNEMO_CT_COMPETENCE_LIST);
                    if (_showFunktion) {
                        fktValue.Text = _db.lookup("FUNKTIONSWERT","FUNKTIONSBEWERTUNGFUNKTIONV","FUNKTION_ID=" + _funktionID + " and GUELTIG_AB<GetDate() and GUELTIG_BIS>GetDate()",true);
                    }
                    if (_showSalary) {
                        salaryValue.Text = _db.Lohn.getBasislohn(_funktionID).ToString("c");
                    }
                }
                if (JobID > 0 || FunktionID > 0) {

                    _competenceLevels = FBSModule.getCompetenceLevels(_db);
                    if (_funktionID <= 0) _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + JobID, false), -1);

					// Neue Version sortiert die Skills dem Skillskatalog entsprechend (Tiefensuche)
					long rootId =  ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "DUTYGROUP", "PARENT_ID is null",""), -1);                   					
					addGroupsDepthFirst(null,rootId);

					/*
                    DataTable grpTable = _db.getDataTable("select * from DUTYGROUP order by " + _db.langAttrName("DUTYGROUP", "TITLE"), "DUTYGROUP");

                    bool isFirstGrp = true;
                    foreach (DataRow grpRow in grpTable.Rows) {
                        long dutyGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);                          
                            
                        string sql = "";
                        string sqlOrdr = " order by " + _db.langAttrName("DUTY_COMPETENCE__DUTY_VALIDITY_V", "NUM_TITLE");
                        DataTable table = null;
                        if (isFirstGrp) {
                            sql = "select * from DUTY_COMPETENCE__DUTY_VALIDITY_V where " + getRestriction() + " and DUTYGROUP_ID is null";
                            sql += sqlOrdr;
                            table = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
                            addGroup(table, ref isFirstGrp, null);
                        }

                        sql = "select * from DUTY_COMPETENCE__DUTY_VALIDITY_V where " + getRestriction() + " and DUTYGROUP_ID="+dutyGroupID;
                        sql += sqlOrdr;
                        table = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
                        addGroup(table, ref isFirstGrp, grpRow);
                    }
					*/
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

        }

		public void addGroupsDepthFirst(DataRow parentGrp, long parentId)
		{			
			long dutyGroupID = parentId;                    
                            
			string sql = "";
			string sqlOrdr = " order by ORDNUMBER";
			DataTable table2 = null;
			if (parentGrp == null)
			{
				sql = "select DUTY_COMPETENCE__DUTY_VALIDITY_V.*, DUTY.ORDNUMBER from DUTY_COMPETENCE__DUTY_VALIDITY_V INNER JOIN DUTY ON DUTY.ID = DUTY_ID where " + getRestriction() + " and DUTY_COMPETENCE__DUTY_VALIDITY_V.DUTYGROUP_ID is null";
				sql += sqlOrdr;
				table2 = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
				addGroup(table2, ref isFirstGrp, null);
			}

			sql = "select DUTY_COMPETENCE__DUTY_VALIDITY_V.*, DUTY.ORDNUMBER from DUTY_COMPETENCE__DUTY_VALIDITY_V INNER JOIN DUTY ON DUTY.ID = DUTY_ID where " + getRestriction() + " and DUTY_COMPETENCE__DUTY_VALIDITY_V.DUTYGROUP_ID="+dutyGroupID;
			sql += sqlOrdr;
			table2 = _db.getDataTable(sql, "DUTY_COMPETENCE__DUTY_VALIDITY_V");
			addGroup(table2, ref isFirstGrp, parentGrp);
			
			DataTable grpTable = _db.getDataTable("select * from DUTYGROUP where PARENT_ID =" + parentId + " order by ORDNUMBER", "DUTYGROUP");
			foreach(DataRow child in grpTable.Rows)
			{
				addGroupsDepthFirst(child, ch.psoft.Util.Validate.GetValid(child["ID"].ToString(), -1L));
			}		
		}

        private void addGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow) {
            TableRow r = null;
            TableCell c = null;
            bool isFirst = true;
            foreach (DataRow row in table.Rows) {
                if (isFirst) {
                    isFirst = false;
                    if (isFirstGrp) {
                        isFirstGrp = false;
                    }
                    else {
                        r = new TableRow();
                        c = new TableCell();
                        r.Cells.Add(c);
                        c.ColumnSpan = 3;
                        c.Height = 10;
                        competenceList.Rows.Add(r);
                    }
                    r = new TableRow();
                    c = new TableCell();
                    competenceList.Rows.Add(r);
                    r.CssClass = "Detail_mainTitle";
                    r.BackColor = System.Drawing.Color.LightGray;
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 3;
                    if (grpRow != null) {
                        c.Text = ch.psoft.Util.Validate.GetValid(grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(), "");
                    }
                    else {
                        c.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_REP_DUTYGROUP_FREE);
                    }
                    if (Global.isModuleEnabled("energiedienst"))
                    {
                        c = new TableCell();
                        r.Cells.Add(c);
                        c.Text = _mapper.get("DUTY_VALIDITY_V", "WEIGHTING");
                    }
                }
                else {
                    r = new TableRow();
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 3;
                    c.Height = 10;
                    competenceList.Rows.Add(r);
                }

                addDutyCompetence(row);
            }
        }

 
        private void addDutyCompetence(DataRow row) {
            long dutyCompetenceValidityID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
            TableRow r = new TableRow();
            TableCell c = null;
            if (FBSModule.showNumTitleInReport) {
                competenceList.Rows.Add(r);
                r.VerticalAlign = VerticalAlign.Top;
                c = new TableCell();
                r.Cells.Add(c);
                c.CssClass = "Detail_special";
                c.Width = Unit.Percentage(5);
                c.Text = row["NUMBER"].ToString();
                c = new TableCell();
                r.Cells.Add(c);
                c.CssClass = "Detail_special";
                c.Text = row[_db.langAttrName(row.Table.TableName, "TITLE")].ToString();
            }
           

            //Description
            r = new TableRow();
            competenceList.Rows.Add(r);
            r.VerticalAlign = VerticalAlign.Top;
            c = new TableCell();
            r.Cells.Add(c);
            c.ColumnSpan = 2;
            c.CssClass = "Detail";
            c.Text = row[_db.langAttrName(row.Table.TableName, "DESCRIPTION")].ToString();

            // Competence-levels...
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.Width = Unit.Pixel(40);
            bool isFirst = true;
            foreach (DataRow clRow in _competenceLevels.Rows) {
                if (ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "COMPETENCE", "DUTY_COMPETENCE_VALIDITY_ID=" + dutyCompetenceValidityID + " and COMPETENCE_LEVEL_ID=" + clRow["ID"], false), -1) > 0) {
                    if (isFirst) {
                        isFirst = false;
                    }
                    else {
                        c.Text += " ";
                        c.ToolTip += ", ";
                    }
                    c.Text += clRow[_db.langAttrName(clRow.Table.TableName, "MNEMO")].ToString();
                    c.ToolTip += clRow[_db.langAttrName(clRow.Table.TableName, "TITLE")].ToString();
                }
            }

            // Weighting if Energiedienst
            if (Global.isModuleEnabled("energiedienst"))
            {
                c = new TableCell();
                r.Cells.Add(c);
                c.CssClass = "Detail_special";
                c.Width = Unit.Pixel(40);
                c.Text = c.Text = row[_db.langAttrName(row.Table.TableName, "WEIGHTING")].ToString();
            }
            //Edit and Delete if jobDescription

                HyperLink url = null;
                bool hasOne = false;
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "JOB", JobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true))
                {
                    c = new TableCell();
                    r.Cells.Add(c);
                    url = new HyperLink();
                    url.CssClass = r.CssClass;
                    url.NavigateUrl = "../EditDutyCompetenceValidity.aspx?jobID=" + JobID + "&dutyID=" + ch.psoft.Util.Validate.GetValid(row["DUTY_ID"].ToString(), -1);
                    url.Text = "E";
                    url.ToolTip = _mapper.get("edit");
                    c.Controls.Add(url);
                    hasOne = true;
                }
                if (ch.psoft.Util.Validate.GetValid(row["JOB_ID"].ToString(), -1) > 0)
                {
                    if (_db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "JOB", JobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true))
                    {
                        if (hasOne)
                        {
                            c = new TableCell();
                            c.Text = "|";
                            r.Cells.Add(c);
                        }
                        c = new TableCell();
                        r.Cells.Add(c);
                        url = new HyperLink();
                        url.CssClass = r.CssClass;
                        url.NavigateUrl = "javascript: deleteRowConfirm('" + r.ClientID + "','" + dutyCompetenceValidityID + "')";
                        url.Text = "D";
                        url.ToolTip = _mapper.get("delete");
                        c.Controls.Add(url);
                    }
                }
                    }

        private void MapButtonMethods() {
            this.CBShowValidDutyCompOnly.CheckedChanged += new System.EventHandler(this.CBShowValidDutyCompOnly_CheckedChanged);
        }

        private void CBShowValidDutyCompOnly_CheckedChanged(object sender, System.EventArgs e) {
            SessionData.setShowValidDutyCompOnly(Session, CBShowValidDutyCompOnly.Checked);
            loadList();
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

namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for XSkillsCtrl.
    /// </summary>
    public partial class XSkillsCtrl : PSOFTMapperUserControl {
        public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";




        protected Config _config = null;
        protected DBData _db = null;
        protected long _funktionID = -1L;

        public static string Path {
            get {return Global.Config.baseURL + "/Skills/Controls/XSkillsCtrl.ascx";}
        }

		#region Properities
        public long JobID {
            get {return GetLong(PARAM_JOB_ID);}
            set {SetParam(PARAM_JOB_ID, value);}
        }

        public long PersonID {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }
        public string deleteMessage 
        {
            get { return _mapper.get("MESSAGES", "deleteConfirm"); }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            if (!IsPostBack) 
            {
                CBShowValidSkillLevelOnly.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_SHOW_VALID_ONLY);          
                CBShowValidSkillLevelOnly.Checked = SessionData.showValidSkillLevelOnly(Session);
                CBShowValidSkillLevelOnly.Visible = true;
            }
            Execute();
        }

        protected string getRestriction(){
            string retValue = "";
            if (SessionData.showValidSkillLevelOnly(Session)) 
                retValue = "VALID_FROM<=GetDate() and VALID_TO>=GetDate() and SKILL_VALIDITY_VALID_FROM<=GetDate() and SKILL_VALIDITY_VALID_TO>=GetDate()";
            else 
                retValue = "SKILL_VALIDITY_VALID_FROM<=GetDate() and SKILL_VALIDITY_VALID_TO>=GetDate()";
            if (JobID > 0)
                retValue += " and (JOB_ID=" + JobID + " or FUNKTION_ID=" + _funktionID + ")";
            else if (PersonID > 0)
                retValue += " and PERSON_ID=" + PersonID;
            return retValue;
        }


        public bool hasAuthorisation(DBData db, int typ, int authorisation)
        {
            bool ret = false;
            switch (typ)
            {
                case SkillsModule.JSKILL:
                    ret = db.hasRowAuthorisation(authorisation, "JOB", JobID, DBData.APPLICATION_RIGHT.SKILLS, true, true);
                    break;
                case SkillsModule.PSKILL:
                    DataTable jobs = db.getDataTable("select ID from JOBPERSONV where PERSON_ID=" + PersonID);
                    foreach (DataRow row in jobs.Rows)
                    {
                        if (db.hasRowAuthorisation(authorisation, "JOB", ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L), DBData.APPLICATION_RIGHT.SKILLS, true, true))
                        {
                            ret = true;
                            break;
                        }
                    }
                    break;
            }
            return ret;
        }

        protected override void DoExecute() 
        {
            base.DoExecute();
            loadList();
        }

		private bool isFirstGrp = true;
        private void loadList() 
        {
            _config = Global.Config;
            _db = DBData.getDBData(Session);
            skillLevelList.Rows.Clear();
            
            try {
                _db.connect();

                if (!IsPostBack) 
                {
                    SkillLevelListTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS,SkillsModule.LANG_MNEMO_CT_DEMAND_LEVEL_LIST);
                }
                if (JobID > 0 || PersonID > 0) 
                {
                    _funktionID = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "JOB", "ID=" + JobID, false), -1);

                    //DataTable grpTable = _db.getDataTable("select * from SKILLGROUP order by " + _db.langAttrName("SKILLGROUP", "TITLE"), "SKILLGROUP");
					//DataTable grpTable = _db.getDataTable("select * from SKILLGROUP order by ORDNUMBER", "SKILLGROUP");
                    long rootId =  ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "SKILLGROUP", "PARENT_ID is null",""), -1);
                   					
					addGroupsDepthFirst(null,rootId);

                   
					/*
                    foreach (DataRow grpRow in grpTable.Rows) 
                    {
                        long skillGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);                          
                            
                        string sql = "";
                        string sqlOrdr = " order by " + _db.langAttrName("SKILL_LEVEL__SKILL_VALIDITY_V", "NUM_TITLE");
                        DataTable table = null;
                        if (isFirstGrp)
                        {
                            sql = "select * from SKILL_LEVEL__SKILL_VALIDITY_V where " + getRestriction() + " and SKILLGROUP_ID is null";
                            sql += sqlOrdr;
                            table = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
                            addGroup(table, ref isFirstGrp, null);
                        }

                        sql = "select * from SKILL_LEVEL__SKILL_VALIDITY_V where " + getRestriction() + " and SKILLGROUP_ID="+skillGroupID;
                        sql += sqlOrdr;
                        table = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
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
			
			long skillGroupID = parentId;                    
                            
			string sql = "";
			string sqlOrdr = " order by ORDNUMBER";
			DataTable table = null;

			if (parentGrp == null)
			{
				sql = "select SKILL_LEVEL__SKILL_VALIDITY_V.*,SKILL.ORDNUMBER from SKILL_LEVEL__SKILL_VALIDITY_V INNER JOIN SKILL ON SKILL.ID = SKILL_ID where " + getRestriction() + " and SKILL_LEVEL__SKILL_VALIDITY_V.SKILLGROUP_ID is null";
				sql += sqlOrdr;
				table = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
				addGroup(table, ref isFirstGrp, null);
			}
			
			sql = "select SKILL_LEVEL__SKILL_VALIDITY_V.*,SKILL.ORDNUMBER from SKILL_LEVEL__SKILL_VALIDITY_V INNER JOIN SKILL ON SKILL.ID = SKILL_ID where " + getRestriction() + " and SKILL_LEVEL__SKILL_VALIDITY_V.SKILLGROUP_ID="+skillGroupID;
			sql += sqlOrdr;
			table = _db.getDataTable(sql, "SKILL_LEVEL__SKILL_VALIDITY_V");
			addGroup(table, ref isFirstGrp, parentGrp);		

			DataTable grpTable = _db.getDataTable("select * from SKILLGROUP where PARENT_ID =" + parentId + " order by ORDNUMBER", "SKILLGROUP");
			foreach(DataRow child in grpTable.Rows)
			{
				addGroupsDepthFirst(child, ch.psoft.Util.Validate.GetValid(child["ID"].ToString(), -1L));
			}		
		}

        private void addGroup(DataTable table, ref bool isFirstGrp, DataRow grpRow)
        {
            TableRow r = null;
            TableCell c = null;
            bool isFirst = true;
            foreach (DataRow row in table.Rows) 
            {
                if (isFirst)
                {
                    isFirst = false;
                    if (isFirstGrp)
                    {
                        isFirstGrp = false;
                    }
                    else 
                    {
                        r = new TableRow();
                        c = new TableCell();
                        r.Cells.Add(c);
                        c.ColumnSpan = 3;
                        c.Height = 10;
                        skillLevelList.Rows.Add(r);
                    }
                    r = new TableRow();
                    c = new TableCell();
                    skillLevelList.Rows.Add(r);
                    r.CssClass = "Detail_mainTitle";
                    r.BackColor = System.Drawing.Color.LightGray;
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 3;
                    if (grpRow != null)
                    {
                        c.Text = ch.psoft.Util.Validate.GetValid(grpRow[_db.langAttrName(grpRow.Table.TableName, "TITLE")].ToString(), "");
                    }
                    else 
                    {
                        c.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_REP_SKILLGROUP_FREE);
                    }
                }
                else
                {
                    r = new TableRow();
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 3;
                    c.Height = 10;
                    skillLevelList.Rows.Add(r);
                }
                addSkillLevel(row);
            }
        }

        protected void addSkillLevel(DataRow row) {
            long skillLevelValidityID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
            TableRow r = new TableRow();
            TableCell c = null;
            if (SkillsModule.showNumTitleInReport)
            {
                skillLevelList.Rows.Add(r);
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
            skillLevelList.Rows.Add(r);
            r.VerticalAlign = VerticalAlign.Top;
            c = new TableCell();
            r.Cells.Add(c);
            c.ColumnSpan = 2;
            c.CssClass = "Detail";
            c.Text = row[_db.langAttrName(row.Table.TableName, "DESCRIPTION")].ToString();

            // Demand-level...
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
//            c.Width = Unit.Pixel(240);
            c.Text +=  " (" + row[_db.langAttrName(row.Table.TableName, "DEMAND_LEVEL_TITLE")].ToString() + ")";

            //Edit and Delete if jobDescription
            int typ = -1;
            if(ch.psoft.Util.Validate.GetValid(row["JOB_ID"].ToString(), -1) > 0)
                typ = SkillsModule.JSKILL;
            if(ch.psoft.Util.Validate.GetValid(row["PERSON_ID"].ToString(), -1) > 0)
                typ = SkillsModule.PSKILL;

            switch (typ)
            {
                case SkillsModule.JSKILL:
                case SkillsModule.PSKILL:
                    HyperLink url = null;
                    bool hasOne = false;
                    if (hasAuthorisation(_db, typ, DBData.AUTHORISATION.UPDATE)){
                        c = new TableCell();
                        r.Cells.Add(c);
                        url = new HyperLink();
                        url.CssClass = r.CssClass;
                        switch (typ) {
                            case SkillsModule.JSKILL:
                                url.NavigateUrl = "../EditSkillLevelValidity.aspx?jobID=" + JobID + "&skillID=" + ch.psoft.Util.Validate.GetValid(row["SKILL_ID"].ToString(), -1);
                                break;
                            case SkillsModule.PSKILL:
                                url.NavigateUrl = "../EditSkillLevelValidity.aspx?personID=" + PersonID + "&skillID=" + ch.psoft.Util.Validate.GetValid(row["SKILL_ID"].ToString(), -1);
                                break;
                        }
                        url.Text = "E";
                        url.ToolTip = _mapper.get("edit");
                        c.Controls.Add(url);
                        hasOne = true;
                    }
                    if (hasAuthorisation(_db, typ, DBData.AUTHORISATION.DELETE)){
                        if (hasOne){
                            c = new TableCell();
                            c.Text = "|";
                            r.Cells.Add(c);
                        }
                        c = new TableCell();
                        r.Cells.Add(c);
                        url = new HyperLink();
                        url.CssClass = r.CssClass;
                        url.NavigateUrl = "javascript: deleteRowConfirm('"+r.ClientID+"','"+skillLevelValidityID+"')";
                        url.Text = "D";
                        url.ToolTip = _mapper.get("delete");
                        c.Controls.Add(url);
                    }
                    break;
                case SkillsModule.FSKILL:
                    break;
            }

        }

        private void MapButtonMethods() 
        {
            this.CBShowValidSkillLevelOnly.CheckedChanged += new System.EventHandler(this.CBShowValidSkillLevelOnly_CheckedChanged);
        }

        private void CBShowValidSkillLevelOnly_CheckedChanged(object sender, System.EventArgs e) 
        {
            SessionData.setShowValidSkillLevelOnly(Session, CBShowValidSkillLevelOnly.Checked);
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

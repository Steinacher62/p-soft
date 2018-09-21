namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for AppraisalDetailCtrl.
    /// </summary>
    public partial class AppraisalDetailCtrl : PSOFTMapperUserControl {
        public const string PARAM_APPRAISAL_ID = "PARAM_APPRAISAL_ID";
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";




        protected Config _config = null;
        protected DBData _db = null;

        public static string Path {
            get {return Global.Config.baseURL + "/Skills/Controls/AppraisalDetailCtrl.ascx";}
        }

		#region Properities
        public long AppraisalID {
            get {return GetLong(PARAM_APPRAISAL_ID);}
            set {SetParam(PARAM_APPRAISAL_ID, value);}
        }

        public long PersonID 
        {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        public bool hasAuthorisation(DBData db, int authorisation)
        {
            bool ret = false;
            DataTable jobTable = db.getDataTable("select ID from JOBPERSONV where PERSON_ID="+PersonID);
            foreach (DataRow row in jobTable.Rows){
                if (db.hasRowAuthorisation(authorisation, "JOB", ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1L), DBData.APPLICATION_RIGHT.SKILLS, true, true))
                    ret = true;
            }
            return ret;
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _config = Global.Config;

            _db = DBData.getDBData(Session);
            
            try {
                _db.connect();

                if (!IsPostBack) {
                    SkillRatingListTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CT_SKILLRATING_LIST);

                    if (AppraisalID > 0) 
                    {

                        DataTable grpTable = _db.getDataTable("select * from SKILLGROUP order by " + _db.langAttrName("SKILLGROUP", "TITLE"), "SKILLGROUP");

                        bool isFirstGrp = true;
                        foreach (DataRow grpRow in grpTable.Rows) 
                        {
                            long skillGroupID = ch.psoft.Util.Validate.GetValid(grpRow["ID"].ToString(), -1L);                          
                            
                            string sql = "";
                            string sqlOrdr = " order by SKILL_NUMBER, " + _db.langAttrName("SKILL_RATING", "SKILL_TITLE");
                            DataTable table = null;
                            if (isFirstGrp)
                            {
                                sql = "select * from SKILL_RATING where SKILLS_APPRAISAL_ID=" + AppraisalID + " and SKILLGROUP_ID  is null";
                                sql += sqlOrdr;
                                table = _db.getDataTable(sql, "SKILL_RATING");
                                addGroup(table, ref isFirstGrp, null);
                            }

                            sql = "select * from SKILL_RATING where SKILLS_APPRAISAL_ID=" + AppraisalID + " and SKILLGROUP_ID="+skillGroupID;
                            sql += sqlOrdr;
                            table = _db.getDataTable(sql, "SKILL_RATING");
                            addGroup(table, ref isFirstGrp, grpRow);

                        }
                    }
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
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
                        c.ColumnSpan = 4;
                        c.Height = 10;
                        skillRatingList.Rows.Add(r);
                    }
                    r = new TableRow();
                    c = new TableCell();
                    skillRatingList.Rows.Add(r);
                    r.CssClass = "Detail_mainTitle";
                    r.BackColor = System.Drawing.Color.LightGray;
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 4;
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
                    c.ColumnSpan = 4;
                    c.Height = 10;
                    skillRatingList.Rows.Add(r);
                }
                addSkillRating(row);
            }
        }

        protected void addSkillRating(DataRow row) {
            long skillRatingID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
            TableRow r = new TableRow();
            TableCell c = null;
            r.VerticalAlign = VerticalAlign.Top;

            // Skill...
            if (SkillsModule.showNumTitleInReport)
            {
                skillRatingList.Rows.Add(r);
                r.VerticalAlign = VerticalAlign.Top;
                c = new TableCell();
                r.Cells.Add(c);
                c.CssClass = "Detail_special";
                c.Width = Unit.Percentage(5);
                c.Text = row["SKILL_NUMBER"].ToString();
                c = new TableCell();
                r.Cells.Add(c);
                c.CssClass = "Detail_special";
                c.Text = row[_db.langAttrName(row.Table.TableName, "SKILL_TITLE")].ToString();
            }

            // Description
            r = new TableRow();
            skillRatingList.Rows.Add(r);
            r.VerticalAlign = VerticalAlign.Top;
            c = new TableCell();
            r.Cells.Add(c);
            c.ColumnSpan = 2;
            c.CssClass = "Detail";
            c.Text = row[_db.langAttrName(row.Table.TableName, "SKILL_DESCRIPTION")].ToString();

            // Demand-level...
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.Text +=  " (" + row[_db.langAttrName(row.Table.TableName, "DEMAND_LEVEL_TITLE")].ToString() + ")";

            // Rating-level...
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.HorizontalAlign = HorizontalAlign.Right;
            c.Text = row[_db.langAttrName(row.Table.TableName, "RATING_LEVEL_TITLE")].ToString();
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
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

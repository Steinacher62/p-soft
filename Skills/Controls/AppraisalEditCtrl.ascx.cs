namespace ch.appl.psoft.Skills.Controls
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
    ///		Summary description for AppraisalEditCtrl.
    /// </summary>
    public partial class AppraisalEditCtrl : PSOFTMapperUserControl {
        public const string PARAM_APPRAISAL_ID = "PARAM_APPRAISAL_ID";





        protected Config _config = null;
        protected DBData _db = null;

        protected DataTable _ratingLevels = null;
        protected ArrayList _dropDownListList = new ArrayList();

        public static string Path {
            get {return Global.Config.baseURL + "/Skills/Controls/AppraisalEditCtrl.ascx";}
        }

		#region Properities
        public long AppraisalID {
            get {return GetLong(PARAM_APPRAISAL_ID);}
            set {SetParam(PARAM_APPRAISAL_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _config = Global.Config;

            if (!IsPostBack) {
                apply.Text = _mapper.get("apply");
                SkillRatingListTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CT_SKILLRATING_LIST);
            }

            _db = DBData.getDBData(Session);
            try {
                _db.connect();

                _ratingLevels = SkillsModule.getRatingLevels(_db);

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
            skillRatingList.Rows.Add(r);
            r.VerticalAlign = VerticalAlign.Top;

            // Skill...
            TableCell c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.Text = row["SKILL_NUMBER"].ToString();
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.Text = row[_db.langAttrName(row.Table.TableName, "SKILL_TITLE")].ToString();

            // Demand-level...
            c.Text +=  " (" + row[_db.langAttrName(row.Table.TableName, "DEMAND_LEVEL_TITLE")].ToString() + ")";

            // Rating-level...
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            c.HorizontalAlign = HorizontalAlign.Right;
            DropDownList dd = new DropDownCtrl();
            c.Controls.Add(dd);
            _dropDownListList.Add(dd);
            dd.ID = skillRatingID.ToString();
            dd.Items.Add(new ListItem("-", "-1"));
            string ratingLevelPercentage = row["RATING_LEVEL_PERCENTAGE"].ToString();
            ListItem li = null;
            foreach (DataRow rlRow in _ratingLevels.Rows){
                li = new ListItem(rlRow[1].ToString(), rlRow[0].ToString());
                if (rlRow[2].ToString() == ratingLevelPercentage){
                    li.Selected = true;
                }
                dd.Items.Add(li);
            }

            // Description
            r = new TableRow();
            skillRatingList.Rows.Add(r);
            r.VerticalAlign = VerticalAlign.Top;
            c = new TableCell();
            r.Cells.Add(c);
            c.ColumnSpan = 3;
            c.CssClass = "Detail";
            c.Text = row[_db.langAttrName(row.Table.TableName, "SKILL_DESCRIPTION")].ToString();

        }

        protected void apply_Click(object sender, System.EventArgs e) {
            _db.connect();
            try {
                _db.beginTransaction();
                string sql = "";
                for (int i=0; i<_dropDownListList.Count; i++){
                    DropDownList dd = (DropDownList) _dropDownListList[i];
                    long skillRatingID = ch.psoft.Util.Validate.GetValid(dd.ID, -1L);
                    long ratingLevelID = ch.psoft.Util.Validate.GetValid(dd.SelectedItem.Value, -1L);
                    sql = "update SKILL_RATING";
                    if (ratingLevelID <= 0){
                        sql += " set RATING_LEVEL_PERCENTAGE=-1";
                        sql += "," + _db.langExpand("RATING_LEVEL_TITLE%LANG%='-'", "SKILL_RATING", "RATING_LEVEL_TITLE");
                        sql += "," + _db.langExpand("RATING_LEVEL_DESCRIPTION%LANG%='-'", "SKILL_RATING", "RATING_LEVEL_DESCRIPTION");
                        sql += " where";
                    }
                    else {
                        sql += " set SKILL_RATING.RATING_LEVEL_PERCENTAGE=RATING_LEVEL.PERCENTAGE";
                        sql += "," + _db.langExpand("SKILL_RATING.RATING_LEVEL_TITLE%LANG%=RATING_LEVEL.TITLE%LANG%", "SKILL_RATING", "RATING_LEVEL_TITLE");
                        sql += "," + _db.langExpand("SKILL_RATING.RATING_LEVEL_DESCRIPTION%LANG%=RATING_LEVEL.DESCRIPTION%LANG%", "SKILL_RATING", "RATING_LEVEL_DESCRIPTION");
                        sql += " from SKILL_RATING, RATING_LEVEL where RATING_LEVEL.ID=" + ratingLevelID + " and";
                    }
                    sql += " SKILL_RATING.ID=" + skillRatingID;
                    _db.execute(sql);
                }
                _db.commit();
            }
            catch (Exception ex) {
                _db.rollback();
                DoOnException(ex);
            }
            finally {
                _db.disconnect();   
            }
            Response.Redirect(Global.Config.baseURL + "/Skills/AppraisalDetail.aspx?appraisalID=" + AppraisalID);
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

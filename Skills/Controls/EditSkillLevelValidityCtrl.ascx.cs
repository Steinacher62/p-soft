namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for EditSkillLevelValidityCtrl.
    /// </summary>
    public partial class EditSkillLevelValidityCtrl : PSOFTInputViewUserControl
	{
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_SKILL_ID = "PARAM_SKILL_ID";
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_SKILL_LEVEL_VALIDITY_ID = "PARAM_SKILL_LEVEL_VALIDITY_ID";





		protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;

        protected DataTable _demandLevels = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Skills/Controls/EditSkillLevelValidityCtrl.ascx";}
		}

		#region Properities
        public long JobID
        {
            get {return GetLong(PARAM_JOB_ID);}
            set {SetParam(PARAM_JOB_ID, value);}
        }
        public long SkillID
        {
            get {return GetLong(PARAM_SKILL_ID);}
            set {SetParam(PARAM_SKILL_ID, value);}
        }
        public long PersonID 
        {
            get {return GetLong(PARAM_PERSON_ID);}
            set {SetParam(PARAM_PERSON_ID, value);}
        }
        public long SkillLevelValidityID
        {
            get {return GetLong(PARAM_SKILL_LEVEL_VALIDITY_ID);}
            set {SetParam(PARAM_SKILL_LEVEL_VALIDITY_ID, value);}
        }

        public string HighLightNode {
            get {
                string retValue = "";
                if (SkillID > 0){
                    retValue = "highlightNode(" + SkillID + ");";
                }
                return retValue;
            }
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();
            _config = Global.Config;
            
            _db = DBData.getDBData(Session);
            
            try
			{
                _db.connect();

                if (!IsPostBack)
                {
                    CompetenceTitle.Text = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS,SkillsModule.LANG_MNEMO_CT_SKILL);
                    apply.Text = _mapper.get("apply");
                    apply.Visible = false;
                }
                DataTable skillTable = _db.getDataTable("select * from SKILL_VALIDITY where SKILL_ID=" + SkillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate()", "SKILL_VALIDITY");
                if (skillTable.Rows.Count > 0)
                {
                    DataRow skillRow = skillTable.Rows[0];

                    TableRow r = new TableRow();
                    competenceCB.Rows.Add(r);
                    r.VerticalAlign = VerticalAlign.Top;
                    TableCell c = new TableCell();
                    r.Cells.Add(c);
                    c.CssClass = "Detail_special";
                    c.Text = skillRow["NUMBER"].ToString();
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.CssClass = "Detail_special";
                    c.Text = skillRow[_db.langAttrName(skillRow.Table.TableName, "TITLE")].ToString();

                    // Competence-levels...
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.RowSpan = 2;
                    c.VerticalAlign = VerticalAlign.Top;

                    bool isFunctionCompetence = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTION_ID", "SKILL_LEVEL_VALIDITY", "ID=" + SkillLevelValidityID, false), -1) > 0;


                    //Description
                    r = new TableRow();
                    competenceCB.Rows.Add(r);
                    r.VerticalAlign = VerticalAlign.Top;
                    c = new TableCell();
                    r.Cells.Add(c);
                    c.ColumnSpan = 2;
                    c.CssClass = "Detail";
                    c.Text = skillRow[_db.langAttrName(skillRow.Table.TableName, "DESCRIPTION")].ToString();

                    _table = _db.getDataTableExt("select * from SKILL_LEVEL_VALIDITY where ID=" + SkillLevelValidityID, "SKILL_LEVEL_VALIDITY");
                    View = "SKILL_LEVEL_VALIDITY";
                    if (SkillLevelValidityID > 0)
                    {
                        InputType = InputMaskBuilder.InputType.Edit;
                    }
                    else
                    {
                        InputType = InputMaskBuilder.InputType.Add;
                        _table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                    }
                    _table.Columns["DEMAND_LEVEL_ID"].ExtendedProperties["In"] = SkillsModule.getDemandLevels(_db);
                    _table.Columns["DEMAND_LEVEL_ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);
                    
                    LoadInput(_db, _table, competenceEdit);

                    if (isFunctionCompetence)
                    {
                        competenceEdit.Enabled = false;
                        apply.Enabled = false;
                    }

                    apply.Visible = true;
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


        protected void apply_Click(object sender, System.EventArgs e) 
        {
            if (checkInputValue(_table, competenceEdit))
            {
                _db.connect();
                try 
                {
                    _db.beginTransaction();
                    StringBuilder sqlB = getSql(_table, competenceEdit, true);
                    if (InputType == InputMaskBuilder.InputType.Add)
                    {
                        SkillLevelValidityID = _db.newId("SKILL_LEVEL_VALIDITY");
                        extendSql(sqlB, _table, "ID", SkillLevelValidityID);
                        if (JobID > 0)
                            extendSql(sqlB, _table, "JOB_ID", JobID);
                        if (PersonID > 0)
                            extendSql(sqlB, _table, "PERSON_ID", PersonID);
                        extendSql(sqlB, _table, "SKILL_ID", SkillID);
                    }
                    string sql = endExtendSql(sqlB);
                    if (sql.Length > 0)
                        _db.execute(sql);

                    _db.commit();
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
                Response.Redirect(Global.Config.baseURL + "/Skills/XSkills.aspx?jobID=" + JobID + "&personID=" + PersonID);
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

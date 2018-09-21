namespace ch.appl.psoft.Skills.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    using System.Text;

    /// <summary>
    ///		Summary description for AddSkillLevelValidityHandsFreeCtrl.
    /// </summary>
    public partial class AddSkillLevelValidityHandsFreeCtrl : PSOFTInputViewUserControl
	{
		public const string PARAM_JOB_ID = "PARAM_JOB_ID";
        public const string PARAM_SKILL_ID = "PARAM_SKILL_ID";
        public const string PARAM_PERSON_ID = "PARAM_PERSON_ID";
        public const string PARAM_SKILL_LEVEL_VALIDITY_ID = "PARAM_SKILL_LEVEL_VALIDITY_ID";




        protected System.Web.UI.WebControls.Table _cbTable;


		protected Config _config = null;
        protected DBData _db = null;
        protected DataTable _table = null;

        protected DataTable _competenceLevels = null;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Skills/Controls/AddSkillLevelValidityHandsFreeCtrl.ascx";}
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
                    apply.Visible = true;
                }
                _table = _db.getDataTableExt("select * from SKILL_VALIDITY where ID=-1", "SKILL_VALIDITY");
                _table.Columns["VALID_TO"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                _table.Columns["VALID_FROM"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;


                InputType = InputMaskBuilder.InputType.Add;


                LoadInput(_db, _table, competenceEdit);

                
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
                    SkillLevelValidityID = _db.newId("SKILL_LEVEL_VALIDITY");
                    SkillID = _db.newId("SKILL");
                    extendSql(sqlB, _table, "SKILL_ID", SkillID);
                    long id = _db.newId("SKILL_VALIDITY");
                    extendSql(sqlB, _table, "ID", id);

                    long demandLevelId = -1;
                    DataTable demandLevels = _db.getDataTable("select ID from DEMAND_LEVEL order by NUMBER");
                    foreach (DataRow row in demandLevels.Rows)
                    {
                        demandLevelId = ch.psoft.Util.Validate.GetValid(row[0].ToString(), -1L);
                        break;
                    }

                    string sql = endExtendSql(sqlB);
                    if (sql.Length > 0)
                    {
                        
                        if (JobID > 0)
                        {
                            _db.execute("insert into SKILL (ID) values ("+SkillID+")");
                            _db.execute("insert into SKILL_LEVEL_VALIDITY (ID,SKILL_ID,DEMAND_LEVEL_ID,JOB_ID) values ("+SkillLevelValidityID+","+SkillID+","+demandLevelId+","+JobID+")");
                            _db.execute(sql);
                        }
                        else if (PersonID > 0)
                        {
                            _db.execute("insert into SKILL (ID) values ("+SkillID+")");
                            _db.execute("insert into SKILL_LEVEL_VALIDITY (ID,SKILL_ID,DEMAND_LEVEL_ID,PERSON_ID) values ("+SkillLevelValidityID+","+SkillID+","+demandLevelId+","+PersonID+")");
                            _db.execute(sql);
                        }
                            
                    }

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
                Response.Redirect(Global.Config.baseURL + "/Skills/EditSkillLevelValidity.aspx?jobID=" + JobID + "&personID=" + PersonID + "&skillID=" + SkillID + "&skillLevelValidityID=" + SkillLevelValidityID);
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

using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for AddSkillLevelValidity.
    /// </summary>
    public partial class AddSkillLevelValidity : PsoftTreeViewPage
	{
        protected bool _actionNew = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ADD_SKILL_LEVEL);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			AddSkillLevelValidityCtrl skCtrl = (AddSkillLevelValidityCtrl)this.LoadPSOFTControl(AddSkillLevelValidityCtrl.Path, "_skAdd");
            skCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], skCtrl.JobID);
            skCtrl.SkillID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillID"], skCtrl.SkillID);
            skCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], skCtrl.PersonID);
            skCtrl.SkillLevelValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillLevelValidityID"], skCtrl.SkillLevelValidityID);

			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, skCtrl);
	
            _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";
            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (skCtrl.JobID > 0)
                {
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + skCtrl.JobID, false);
                }
                else if (skCtrl.PersonID > 0)
                {
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(skCtrl.PersonID) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ACTUALSKILLS);
                }

                if (skCtrl.SkillID <= 0)
                    skCtrl.SkillID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILLGROUPTREEV", "PARENT_ID is null", false), -1);

                if (skCtrl.SkillID > 0)
                {
                    if (skCtrl.SkillLevelValidityID <= 0 && !_actionNew)
                    {
                        if (skCtrl.JobID > 0)
                            skCtrl.SkillLevelValidityID = getSkillLevelValidityID(db, skCtrl.SkillID, skCtrl.JobID, SkillsModule.JSKILL);
                        else if (skCtrl.PersonID > 0)
                            skCtrl.SkillLevelValidityID = getSkillLevelValidityID(db, skCtrl.SkillID, skCtrl.PersonID, SkillsModule.PSKILL);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                db.disconnect();
            }

		}

        public static long getSkillLevelValidityID(DBData db, long skillID, long xid, int type)
        {
            long retValue = -1;
            switch (type)
            {
                case SkillsModule.JSKILL:
                    long funktionID = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID=" + xid, false), -1);
                    retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and FUNKTION_ID=" + funktionID, false), -1);
                    if (retValue <= 0)
                    {
                        // if no function-competence get job-competence
                        retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and JOB_ID=" + xid, false), -1);
                        if (retValue <= 0)
                        {
                            // if no competence-validity specified, get first job-competence
                            retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and JOB_ID=" + xid + " order by VALID_FROM asc", false), -1);
                            if (retValue <= 0)
                            {
                                // if no first job-competence get first function-competence
                                retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and FUNKTION_ID=" + funktionID + " order by VALID_FROM asc", false), -1);
                            }
                        }
                    }
                    break;
                case SkillsModule.PSKILL:
                    // if no function-competence get person-competence
                    retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and PERSON_ID=" + xid, false), -1);
                    if (retValue <= 0)
                    {
                        // if no competence-validity specified, get first person-competence
                        retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "SKILL_LEVEL_VALIDITY", "SKILL_ID=" + skillID + " and PERSON_ID=" + xid + " order by VALID_FROM asc", false), -1);
                    }
                    break;
            }
            
            return retValue;
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}

using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for EditSkillLevelValidity.
    /// </summary>
    public partial class EditSkillLevelValidity : PsoftEditPage
	{
		private PsoftLinksControl _links = null;
        protected bool _actionNew = false;

        public EditSkillLevelValidity() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_EDIT_SKILL_LEVEL);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			EditSkillLevelValidityCtrl skCtrl = (EditSkillLevelValidityCtrl)this.LoadPSOFTControl(EditSkillLevelValidityCtrl.Path, "_skEdit");
            skCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], skCtrl.JobID);
            skCtrl.SkillID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillID"], skCtrl.SkillID);
            skCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], skCtrl.PersonID);
            skCtrl.SkillLevelValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillLevelValidityID"], skCtrl.SkillLevelValidityID);

			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, skCtrl);
	
            _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";

			_links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (skCtrl.JobID > 0)
                {
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + skCtrl.JobID, false);
                    _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_JOBSKILLS);
                }
                else if (skCtrl.PersonID > 0)
                {
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(skCtrl.PersonID) + " - " + _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ACTUALSKILLS);
                    _links.LinkGroup1.Caption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CMT_ACUTALSKILLS);
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
                    if (skCtrl.SkillLevelValidityID > 0)
                    {
                        SkillLevelValidityList skList = (SkillLevelValidityList)this.LoadPSOFTControl(SkillLevelValidityList.Path, "_skList");
                        skList.SkillID = skCtrl.SkillID;
                        skList.JobID = skCtrl.JobID;
                        skList.PersonID = skCtrl.PersonID;
                        skList.SkillLevelValidityID = skCtrl.SkillLevelValidityID;
                        skList.DetailURL = Global.Config.baseURL + "/Skills/EditSkillLevelValidity.aspx?skillLevelValidityID=%ID&jobID=" + skCtrl.JobID + "&personID=" + skCtrl.PersonID + "&skillID=" + skCtrl.SkillID;
                        skList.DetailEnabled = true;
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, skList);
                        _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_NEW_SKILL_LEVEL), "/Skills/EditSkillLevelValidity.aspx?jobID=" + skCtrl.JobID + "&personID=" + skCtrl.PersonID + "&skillID=" + skCtrl.SkillID + "&action=new");
                        if (ch.psoft.Util.Validate.GetValid(db.lookup("SKILLGROUP_ID", "SKILL", "ID=" + skCtrl.SkillID, false), -1) < 0)
                            _links.LinkGroup1.AddLink(_mapper.get("edit"), _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_CM_EDIT_SKILL_FREE), "/Skills/EditSkillHandsFree.aspx?jobID=" + skCtrl.JobID + "&personID=" + skCtrl.PersonID + "&skillID=" + skCtrl.SkillID);
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

			SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);		
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

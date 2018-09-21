using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for AddSkillLevelValidityHandsFree.
    /// </summary>
    public partial class AddSkillLevelValidityHandsFree : PsoftEditPage
	{
//		private PsoftLinksControl _links = null;
        protected bool _actionNew = false;

        public AddSkillLevelValidityHandsFree() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_ADD_SKILL_LEVEL_FREE);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			AddSkillLevelValidityHandsFreeCtrl skCtrl = (AddSkillLevelValidityHandsFreeCtrl)this.LoadPSOFTControl(AddSkillLevelValidityHandsFreeCtrl.Path, "_dcvAdd");
            skCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], skCtrl.JobID);
            skCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], skCtrl.PersonID);

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

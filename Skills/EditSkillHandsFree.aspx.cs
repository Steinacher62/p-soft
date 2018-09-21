using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Skills.Controls;
using System;


namespace ch.appl.psoft.Skills
{
    /// <summary>
    /// Summary description for EditSkillHandsFree.
    /// </summary>
    public partial class EditSkillHandsFree : PsoftEditPage
	{
//		private PsoftLinksControl _links = null;

        public EditSkillHandsFree() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(SkillsModule.LANG_SCOPE_SKILLS, SkillsModule.LANG_MNEMO_BC_EDIT_SKILL_FREE);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			EditSkillHandsFreeCtrl skCtrl = (EditSkillHandsFreeCtrl)this.LoadPSOFTControl(EditSkillHandsFreeCtrl.Path, "_dcvAdd");
            skCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], skCtrl.JobID);
            skCtrl.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], skCtrl.PersonID);
            skCtrl.SkillID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillID"], skCtrl.SkillID);

			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, skCtrl);
            
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

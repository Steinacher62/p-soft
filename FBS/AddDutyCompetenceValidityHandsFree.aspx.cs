using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for AddDutyCompetenceValidityHandsFree.
    /// </summary>
    public partial class AddDutyCompetenceValidityHandsFree : PsoftEditPage
	{
//		private PsoftLinksControl _links = null;
        protected bool _actionNew = false;

        public AddDutyCompetenceValidityHandsFree() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_ADD_DUTY_COMPETENCE_FREE);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			AddDutyCompetenceValidityHandsFreeCtrl dcvCtrl = (AddDutyCompetenceValidityHandsFreeCtrl)this.LoadPSOFTControl(AddDutyCompetenceValidityHandsFreeCtrl.Path, "_dcvAdd");
            dcvCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], dcvCtrl.JobID);

			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, dcvCtrl);
	
            _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";

            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (dcvCtrl.JobID > 0)
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + dcvCtrl.JobID, false);

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

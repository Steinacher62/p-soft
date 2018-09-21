using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for EditDutyHandsFree.
    /// </summary>
    public partial class EditDutyHandsFree : PsoftEditPage
	{
//		private PsoftLinksControl _links = null;

        public EditDutyHandsFree() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_EDIT_DUTY_FREE);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            DBData db = DBData.getDBData(Session);
            try {
                db.connect();

                // Setting parameters
			    EditDutyHandsFreeCtrl jdCtrl = (EditDutyHandsFreeCtrl)this.LoadPSOFTControl(EditDutyHandsFreeCtrl.Path, "_dcvAdd");
                jdCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], jdCtrl.JobID);
                jdCtrl.DutyID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyID"], jdCtrl.DutyID);

                if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "JOB", jdCtrl.JobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true)){
                    //Setting content layout user controls
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, jdCtrl);
            
                    if (jdCtrl.JobID > 0) {
                        PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + jdCtrl.JobID, false);
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

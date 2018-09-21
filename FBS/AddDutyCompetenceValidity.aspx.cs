using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for AddDutyCompetenceValidity.
    /// </summary>
    public partial class AddDutyCompetenceValidity : PsoftTreeViewPage
	{
        protected bool _actionNew = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_ADD_DUTY_COMPETENCE);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			AddDutyCompetenceValidityCtrl dcvCtrl = (AddDutyCompetenceValidityCtrl)this.LoadPSOFTControl(AddDutyCompetenceValidityCtrl.Path, "_dcvAdd");
            dcvCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], dcvCtrl.JobID);
            dcvCtrl.DutyID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyID"], dcvCtrl.DutyID);
            dcvCtrl.DutyCompetenceValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyCompetenceValidityID"], dcvCtrl.DutyCompetenceValidityID);

			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, dcvCtrl);
	
            _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";
            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (dcvCtrl.JobID > 0)
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + dcvCtrl.JobID, false);

                if (dcvCtrl.DutyID <= 0)
                    dcvCtrl.DutyID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTYGROUPTREEV", "PARENT_ID is null", false), -1);

                if (dcvCtrl.DutyID > 0)
                {
                    if (dcvCtrl.DutyCompetenceValidityID <= 0 && !_actionNew)
                    {
                        dcvCtrl.DutyCompetenceValidityID = getDutyCompetenceValidityID(db, dcvCtrl.DutyID, dcvCtrl.JobID);
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

        public static long getDutyCompetenceValidityID(DBData db, long dutyID, long jobID)
        {
            long retValue = -1;
            long funktionID = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID=" + jobID, false), -1);
            retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTY_COMPETENCE_VALIDITY", "DUTY_ID=" + dutyID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and FUNKTION_ID=" + funktionID, false), -1);
            if (retValue <= 0)
            {
                // if no function-competence get job-competence
                retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTY_COMPETENCE_VALIDITY", "DUTY_ID=" + dutyID + " and VALID_FROM<=GetDate() and VALID_TO>=GetDate() and JOB_ID=" + jobID, false), -1);
                if (retValue <= 0)
                {
                    // if no competence-validity specified, get first job-competence
                    retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTY_COMPETENCE_VALIDITY", "DUTY_ID=" + dutyID + " and JOB_ID=" + jobID + " order by VALID_FROM asc", false), -1);
                    if (retValue <= 0)
                    {
                        // if no first job-competence get first function-competence
                        retValue = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTY_COMPETENCE_VALIDITY", "DUTY_ID=" + dutyID + " and FUNKTION_ID=" + funktionID + " order by VALID_FROM asc", false), -1);
                    }
                }
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

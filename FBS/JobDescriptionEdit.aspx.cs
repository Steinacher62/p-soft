using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for JobDescriptionEdit.
    /// </summary>
    public partial class JobDescriptionEdit : PsoftTreeViewPage
	{
		private PsoftLinksControl _links = null;
        protected bool _actionNew = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_EDIT_JOBDESCRIPTION);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			JobDescriptionEditCtrl jdCtrl = (JobDescriptionEditCtrl)this.LoadPSOFTControl(JobDescriptionEditCtrl.Path, "_jobDescEdit");
            jdCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], jdCtrl.JobID);
            jdCtrl.DutyID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyID"], jdCtrl.DutyID);
            jdCtrl.DutyCompetenceValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyCompetenceValidityID"], jdCtrl.DutyCompetenceValidityID);

			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, jdCtrl);
	
            _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";

			_links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            _links.LinkGroup1.Caption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CMT_JOBDESCRIPTION);
            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (jdCtrl.JobID > 0)
                    PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + jdCtrl.JobID, false);

                if (jdCtrl.DutyID <= 0)
                    jdCtrl.DutyID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTYGROUPTREEV", "PARENT_ID is null", false), -1);

                if (jdCtrl.DutyID > 0)
                {
                    if (jdCtrl.DutyCompetenceValidityID <= 0 && !_actionNew)
                    {
                        jdCtrl.DutyCompetenceValidityID = getDutyCompetenceValidityID(db, jdCtrl.DutyID, jdCtrl.JobID);
                    }
                    if (jdCtrl.DutyCompetenceValidityID > 0)
                    {
                        DutyCompetenceValidityList dcvList = (DutyCompetenceValidityList)this.LoadPSOFTControl(DutyCompetenceValidityList.Path, "_dcvList");
                        dcvList.DutyID = jdCtrl.DutyID;
                        dcvList.JobID = jdCtrl.JobID;
                        dcvList.DutyCompetenceValidityID = jdCtrl.DutyCompetenceValidityID;
                        dcvList.DetailURL = Global.Config.baseURL + "/FBS/JobDescriptionEdit.aspx?dutyCompetenceValidityID=%ID&jobID=" + jdCtrl.JobID + "&dutyID=" + jdCtrl.DutyID;
                        dcvList.DetailEnabled = true;
                        SetPageLayoutContentControl(DGLContentLayout.GROUP, dcvList);
                        _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CM_NEW_DUTY_COMPETENCE), "/FBS/JobDescriptionEdit.aspx?jobID=" + jdCtrl.JobID + "&dutyID=" + jdCtrl.DutyID + "&action=new");
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

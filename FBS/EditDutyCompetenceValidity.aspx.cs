using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for EditDutyCompetenceValidity.
    /// </summary>
    public partial class EditDutyCompetenceValidity : PsoftEditPage
	{
		private PsoftLinksControl _links = null;
        protected bool _actionNew = false;

        public EditDutyCompetenceValidity() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_EDIT_DUTY_COMPETENCE);

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
			    EditDutyCompetenceValidityCtrl jdCtrl = (EditDutyCompetenceValidityCtrl)this.LoadPSOFTControl(EditDutyCompetenceValidityCtrl.Path, "_dcvEdit");
                jdCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], jdCtrl.JobID);
                jdCtrl.DutyID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyID"], jdCtrl.DutyID);
                jdCtrl.DutyCompetenceValidityID = ch.psoft.Util.Validate.GetValid(Request.QueryString["dutyCompetenceValidityID"], jdCtrl.DutyCompetenceValidityID);

                if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "JOB", jdCtrl.JobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true)){

                    //Setting content layout user controls
                    SetPageLayoutContentControl(DGLContentLayout.DETAIL, jdCtrl);
    	
                    _actionNew = ch.psoft.Util.Validate.GetValid(Request.QueryString["action"], "").ToLower() == "new";

                    _links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    _links.LinkGroup1.Caption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CMT_JOBDESCRIPTION);
            
                    if (jdCtrl.JobID > 0)
                        PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + jdCtrl.JobID, false);

                    if (jdCtrl.DutyID <= 0)
                        jdCtrl.DutyID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "DUTYGROUPTREEV", "PARENT_ID is null", false), -1);

                    if (jdCtrl.DutyID > 0) {
                        if (jdCtrl.DutyCompetenceValidityID <= 0 && !_actionNew) {
                            jdCtrl.DutyCompetenceValidityID = getDutyCompetenceValidityID(db, jdCtrl.DutyID, jdCtrl.JobID);
                        }
                        if (jdCtrl.DutyCompetenceValidityID > 0) {
                            DutyCompetenceValidityList dcvList = (DutyCompetenceValidityList)this.LoadPSOFTControl(DutyCompetenceValidityList.Path, "_dcvList");
                            dcvList.DutyID = jdCtrl.DutyID;
                            dcvList.JobID = jdCtrl.JobID;
                            dcvList.DutyCompetenceValidityID = jdCtrl.DutyCompetenceValidityID;
                            dcvList.DetailURL = Global.Config.baseURL + "/FBS/EditDutyCompetenceValidity.aspx?dutyCompetenceValidityID=%ID&jobID=" + jdCtrl.JobID + "&dutyID=" + jdCtrl.DutyID;
                            dcvList.DetailEnabled = true;
                            SetPageLayoutContentControl(DGLContentLayout.GROUP, dcvList);
                            _links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CM_NEW_DUTY_COMPETENCE), "/FBS/EditDutyCompetenceValidity.aspx?jobID=" + jdCtrl.JobID + "&dutyID=" + jdCtrl.DutyID + "&action=new");
                            if (ch.psoft.Util.Validate.GetValid(db.lookup("DUTYGROUP_ID", "DUTY", "ID=" + jdCtrl.DutyID, false), -1) < 0)
                                _links.LinkGroup1.AddLink(_mapper.get("edit"), _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CM_EDIT_DUTY_FREE), "/FBS/EditDutyHandsFree.aspx?jobID=" + jdCtrl.JobID + "&dutyID=" + jdCtrl.DutyID);
                        }
                    }

                    SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);		
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

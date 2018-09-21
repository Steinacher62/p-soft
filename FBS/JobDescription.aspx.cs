using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBS.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBS
{
    /// <summary>
    /// Summary description for JobDescription.
    /// </summary>
    public partial class JobDescription : PsoftDetailPage
    {
        private PsoftLinksControl _links = null;

        public JobDescription()
            : base()
        {
            ShowProgressBar = false;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            BreadcrumbCaption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_BC_JOBDESCRIPTION);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            JobDescriptionCtrl jdCtrl = (JobDescriptionCtrl)this.LoadPSOFTControl(JobDescriptionCtrl.Path, "_jobDesc");
            jdCtrl.JobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["jobID"], jdCtrl.JobID);

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, jdCtrl);

            _links = (PsoftLinksControl)LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            _links.LinkGroup1.Caption = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CMT_JOBDESCRIPTION);

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (jdCtrl.JobID > 0)
                {
                    if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", jdCtrl.JobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true))
                    {
                        string specialDirectory = "";

                        if (Global.isModuleEnabled("spz"))
                        {
                            specialDirectory = "../SPZ/";
                        }
                        else if (Global.isModuleEnabled("laufenburg"))
                        {
                            specialDirectory = "../Laufenburg/";
                            PsoftPageLayout.ButtonPrintAttributes.Add(
                                "onClick",
                                "javascript: window.open('" + specialDirectory + "PrintJobDescription.aspx?jobID=" + jdCtrl.JobID + "');"
                                );
                        }
                        else if (Global.isModuleEnabled("bachem"))
                        {
                            specialDirectory = "../Bachem/";
                        }
                        else if (Global.isModuleEnabled("RPB"))
                        {
                            specialDirectory = "../RPB/";
                        }
                        if (Global.isModuleEnabled("energiedienst"))
                        {
                            specialDirectory = "../Energiedienst/";
                            PsoftPageLayout.ButtonPrintAttributes.Add(
                                "onClick",
                                "javascript: window.open('" + specialDirectory + "PrintJobDescription.aspx?jobID=" + jdCtrl.JobID + "');"
                                );
                        }
                        else
                        {
                            PsoftPageLayout.ButtonPrintAttributes.Add(
                            "onClick", "javascript: window.open('" + specialDirectory + "PrintJobDescription.aspx?jobID=" + jdCtrl.JobID + "');"
                             );
                        }
                        PsoftPageLayout.ButtonPrintVisible = true;
                        PsoftPageLayout.PageTitle = db.lookup("TITLE", "JOBPERSONV", "ID=" + jdCtrl.JobID, false);
                        if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", jdCtrl.JobID, DBData.APPLICATION_RIGHT.JOB_DESCRIPTION, true, true))
                        {
                            _links.LinkGroup1.AddLink("", _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CM_ADD_DUTY_COMPETENCE), "/FBS/AddDutyCompetenceValidity.aspx?jobID=" + jdCtrl.JobID);
                            if (!Global.isModuleEnabled("ahb"))
                            {
                                _links.LinkGroup1.AddLink("", _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CM_ADD_DUTY_COMPETENCE_FREE), "/FBS/AddDutyCompetenceValidityHandsFree.aspx?jobID=" + jdCtrl.JobID);

                            }
                        }
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

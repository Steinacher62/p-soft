using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Energiedienst.Controls;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Training;
using System;


namespace ch.appl.psoft.Energiedienst
{
    /// <summary>
    /// Summary description for Advancement.
    /// </summary>
    public partial class Advancement : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.GroupWide = true;

            // Setting parameters
            DBData db = DBData.getDBData(Session);
            try {
                AdvancementList list = (AdvancementList) LoadPSOFTControl(AdvancementList.Path, "_list");
                list.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], list.PersonID);
                list.AdvancementID = ch.psoft.Util.Validate.GetValid(Request.QueryString["advancementID"], list.getFirstAdvancementID());
                if (list.AdvancementID > 0){
                    //verify...
                    list.AdvancementID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "TRAINING_ADVANCEMENT", "ID=" + list.AdvancementID, false), list.getFirstAdvancementID());
                }
                if (list.PersonID <= 0 && list.AdvancementID > 0)
                {
                    list.PersonID = db.Training.getPersonID(list.AdvancementID);
                }

                list.OrderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderColumn"], list.OrderColumn);
                list.OrderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["OrderDir"], list.OrderDir);
                list.SortURL = Global.Config.baseURL + "/Energiedienst/Advancement.aspx?advancementID=" + list.AdvancementID + "&personID=" + list.PersonID;
                list.DetailURL = Global.Config.baseURL + "/Energiedienst/Advancement.aspx?advancementID=%ID&personID=" + list.PersonID + "&OrderColumn=" + list.OrderColumn + "&OrderDir=" + list.OrderDir;
                list.DetailEnabled = true;
                list.DeleteEnabled = false;

                AdvancementDetailCtrl detail = (AdvancementDetailCtrl) LoadPSOFTControl(AdvancementDetailCtrl.Path, "_advancement");
                detail.PersonID = list.PersonID;
                detail.AdvancementID = list.AdvancementID;
                detail.TrainingID = db.Training.getTrainingID(detail.AdvancementID);

                TrainingCatalogTreeCtrl tree = (TrainingCatalogTreeCtrl) LoadPSOFTControl(TrainingCatalogTreeCtrl.Path, "_tree");
                tree.AdvancementID = list.AdvancementID;
                tree.TrainingID = detail.TrainingID;
                if (tree.TrainingID > 0) 
                {
                    tree.Visible = true;
                }
                else
                {
                    tree.Visible = false;
                }
                 
    	
                PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
            
                db.connect();
                if (detail.PersonID > 0){
                    if (!Global.isModuleEnabled("energiedienst"))
                    {
                        PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintPersonAdvancement.aspx?personID=" + detail.PersonID + "&advancementID=" + detail.AdvancementID + "');");
                    }
                    else
                    {
                        long jobId = (long)db.lookup("JOB.ID","JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID","(JOB.HAUPTFUNKTION = 1) AND (PERSON.ID = "+detail.PersonID.ToString()+")",0L);
                        long oeId = (long)db.lookup("JOB.ORGENTITY_ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID", "(JOB.HAUPTFUNKTION = 1) AND (PERSON.ID = " + detail.PersonID.ToString() + ")",0L);
                        long LeaderId =db.Person.getLeader(detail.PersonID, jobId, oeId, false);
                        PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.location.href('../report/CrystalReportViewer.aspx?alias=PersonalentwicklungsbedarfEnergiedienst&param0=" + detail.AdvancementID + "&param1=" + db.Person.getWholeName(LeaderId) + "&param2=" + detail.PersonID.ToString() + "');");
                    }
                        PsoftPageLayout.ButtonPrintVisible = true;
                    BreadcrumbCaption = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_ADVANCEMENT);
                    links.LinkGroup1.Caption = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_CMT_TRAINING);
                    PsoftPageLayout.PageTitle = db.Person.getWholeName(detail.PersonID) + " - " + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_ADVANCEMENT);
                    if (detail.hasAuthorisation(db, DBData.AUTHORISATION.INSERT)){
                        links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MENMO_CM_ADD_ADVANCEMENT), "/Energiedienst/AdvancementEdit.aspx?personID=" + detail.PersonID);
                    }
                    if (!Global.isModuleEnabled("energiedienst"))
                    {
                        if (detail.hasAuthorisation(db, DBData.AUTHORISATION.UPDATE))
                        {
                            links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MENMO_CM_EDIT_ADVANCEMENT), "/Energiedienst/AdvancementEdit.aspx?personID=" + detail.PersonID + "&advancementID=" + detail.AdvancementID);
                        }
                    }
                    else
                    {   //wenn Energiediesnst Link Edit anzeigen
                        if (detail.hasAuthorisation(db, DBData.AUTHORISATION.UPDATE) || db.userId == list.PersonID)
                        {
                            links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MENMO_CM_EDIT_ADVANCEMENT), "/Energiedienst/AdvancementEdit.aspx?personID=" + detail.PersonID + "&advancementID=" + detail.AdvancementID);
                        }
                    }
                    //if (detail.hasAuthorisation(db, DBData.AUTHORISATION.DELETE)){
                    //    links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MENMO_CM_DELETE_ADVANCEMENT), "javascript: deleteAdvancementConfirm('"+detail.AdvancementID+"')");                       
                       list.DeleteEnabled = true;
                    //}

                    //Setting content layout user controls
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, detail);
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, tree);
                    SetPageLayoutContentControl(DDGLContentLayout.GROUP, list);
                    SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);
                }

            }
            catch (Exception ex) {
                ShowError(ex.Message);
            }
            finally {
                db.disconnect();
            }

		
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
        }
		#endregion

    }
}

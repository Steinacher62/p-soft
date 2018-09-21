using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Energiedienst.Controls;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Training;
using System;


namespace ch.appl.psoft.Energiedienst
{
    /// <summary>
    /// Summary description for AdvancementEdit.
    /// </summary>
    public partial class AdvancementEdit : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

            // Setting parameters
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                AdvancementEditCtrl edit = (AdvancementEditCtrl) LoadPSOFTControl(AdvancementEditCtrl.Path, "_ad");
                edit.PersonID = ch.psoft.Util.Validate.GetValid(Request.QueryString["personID"], edit.PersonID);
                edit.AdvancementID = ch.psoft.Util.Validate.GetValid(Request.QueryString["advancementID"], edit.AdvancementID);
                edit.TrainingID = ch.psoft.Util.Validate.GetValid(Request.QueryString["trainingID"], (db.Training.getTrainingID(edit.AdvancementID)));

                TrainingCatalogTreeCtrl tree = (TrainingCatalogTreeCtrl) LoadPSOFTControl(TrainingCatalogTreeCtrl.Path, "_tree");
                tree.AdvancementID = edit.AdvancementID;
                tree.TrainingID = edit.TrainingID;
                tree.LeafNodeUrl = "AdvancementEdit.aspx?personID=" + edit.PersonID + "&advancementID=" + tree.AdvancementID + "&trainingID=%ID";
                if (tree.AdvancementID <= 0 || tree.TrainingID > 0) 
                {
                    tree.Visible = true;
                }
                else
                {
                    tree.Visible = false;
                }
                edit.TreeCtrl = tree;

                //Setting content layout user controls
                SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, edit);
                SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, tree);
	
                if (edit.PersonID > 0)
                {
                    if (edit.AdvancementID > 0) 
                    {
                        BreadcrumbCaption = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_EDIT_ADVANCEMENT);
                        PsoftPageLayout.PageTitle = db.Person.getWholeName(edit.PersonID) + " - " + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_EDIT_ADVANCEMENT);
                    }
                    else 
                    {
                        BreadcrumbCaption = _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_ADD_ADVANCEMENT);
                        PsoftPageLayout.PageTitle = db.Person.getWholeName(edit.PersonID) + " - " + _mapper.get(TrainingModule.LANG_SCOPE_TRAINING, TrainingModule.LANG_MNEMO_BC_ADD_ADVANCEMENT);
                    }
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

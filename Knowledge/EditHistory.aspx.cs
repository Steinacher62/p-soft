using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Knowledge.Controls;
using ch.appl.psoft.LayoutControls;
using System;

namespace ch.appl.psoft.Knowledge
{
    public partial class EditHistory : PsoftEditPage {

        private const string PAGE_URL = "/Knowledge/EditHistory.aspx";

        static EditHistory() {
            SetPageParams(PAGE_URL, "knowledgeID", "mode");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public EditHistory() : base() {
            PageURL = PAGE_URL;
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");

            // Setting content layout of page layout
            DGLContentLayout dlgControl = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = dlgControl;

            // Load detail control
            EditHistoryCtrl edit = (EditHistoryCtrl) LoadPSOFTControl(EditHistoryCtrl.Path, "_edit");
            edit.KnowledgeId = GetQueryValue("knowledgeID", -1L);
			edit.InputType = InputMaskBuilder.InputType.Edit;
			switch(GetQueryValue("mode", "edit")){
				case "edit":
                    (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption =  _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_EDIT_HISTORY);                  					
					edit.Mode = EditHistoryCtrl.ControlMode.Edit;
					edit.InputType = InputMaskBuilder.InputType.Edit;
                    break;

                case "add":
                    (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_ADD_HISTORY);
					edit.Mode = EditHistoryCtrl.ControlMode.New;
					edit.InputType = InputMaskBuilder.InputType.Add;
                    break;
            }

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, edit);	
			
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

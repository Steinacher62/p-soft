using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Knowledge.Controls;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Text;

namespace ch.appl.psoft.Knowledge
{
    using Interface.DBObjects;

    public partial class EditKnowledge : PsoftTreeViewPage {

        private const string PAGE_URL = "/Knowledge/EditKnowledge.aspx";

        static EditKnowledge() {
            SetPageParams(PAGE_URL, "knowledgeID", "backURL", "mode", "linkingKnowledgeID", "title");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public EditKnowledge() : base() {
            PageURL = PAGE_URL;
        }

        protected override void AppendJavaScripts(StringBuilder javaScripts) {
            base.AppendJavaScripts(javaScripts);
            javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/WikiBoxTools.js'></script>\r\n");
            javaScripts.Append("<script language=\"JavaScript\" type='text/javascript' src='" + Global.Config.baseURL + "/JavaScript/Editor.js'></script>\r\n");
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");

            // Setting content layout of page layout
            DGLContentLayout dlgControl = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = dlgControl;
            
            DBData db = DBData.getDBData(Session);
            db.connect();
            try {
                // Load detail control
                KnowledgeEditCtrl edit = (KnowledgeEditCtrl) LoadPSOFTControl(KnowledgeEditCtrl.Path, "_edit");
                edit.KnowledgeID= GetQueryValue("knowledgeID", -1L);
                edit.LinkingKnowledgeID = GetQueryValue("linkingKnowledgeID", -1L);
                edit.NextURL = GetQueryValue("backURL", "");
                switch(GetQueryValue("mode", "edit")){
                    case "edit":
                        (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_EDIT_KNOWLEDGE);
                        edit.InputType = InputMaskBuilder.InputType.Edit;
                        ((PsoftPageLayout)PageLayoutControl).ShowButtonAuthorisation(Knowledge._TABLENAME, edit.KnowledgeID);
                        break;

                    case "add":
                        (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get(KnowledgeModule.LANG_SCOPE_KNOWLEDGE, KnowledgeModule.LANG_MNEMO_BC_ADD_KNOWLEDGE);
                        edit.InputType = InputMaskBuilder.InputType.Add;
                        edit.Title = GetQueryValue("title", "");
                        break;
                }
                
                // Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.DETAIL, edit);	
            }
            catch (Exception ex) {
                ShowError(ex.Message);
                Logger.Log(ex,Logger.ERROR);
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

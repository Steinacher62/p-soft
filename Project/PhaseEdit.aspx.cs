using ch.appl.psoft.Common;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using System;

namespace ch.appl.psoft.Project
{

    public partial class PhaseEdit : PsoftTreeViewPage {

        protected void Page_Load(object sender, System.EventArgs e) 
        {
			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
			PageLayoutControl = PsoftPageLayout;
            PsoftPageLayout.PageTitle = BreadcrumbCaption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_BC_EDITPHASE);

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout)LoadPSOFTControl(DGLContentLayout.Path, "_cl");
			PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting control
            PhaseEditCtrl pe = (PhaseEditCtrl)LoadPSOFTControl(PhaseEditCtrl.Path, "_pe");
            pe.PhaseID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"], -1);
            pe.NextURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["nextURL"], "");

            //Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, pe);		
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
            this.ID = "Add";

        }
		#endregion
    }
}
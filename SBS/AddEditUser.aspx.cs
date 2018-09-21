using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.SBS.Controls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.SBS
{
    public partial class AddEditUser : PsoftDetailPage
    {
        // Query string variables
        private string _mode = "detail";

        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
			db.connect();
			try
			{
				// Setting main page layout
				PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
				PageLayoutControl = PsoftPageLayout;

				// Setting content layout of page layout
				PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

				// Setting breadcrumb caption
                BreadcrumbCaption = _mapper.get(SbsModule.LANG_SCOPE_SBS, SbsModule.LANG_MNEMO_ADD_EDIT_USER);
                

				// Setting page-title
                ((PsoftPageLayout)PageLayoutControl).PageTitle = _mapper.get(SbsModule.LANG_SCOPE_SBS, SbsModule.LANG_MNEMO_USERMANAGEMENT);

                // Setting parameters
                AddEditUserDetail detail
                    = (AddEditUserDetail)this.LoadPSOFTControl(AddEditUserDetail.Path, "_detail");

                // Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);	

            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally
            {
                db.disconnect();
            }

        }
    }
}
using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.FBW.Controls;
using ch.appl.psoft.LayoutControls;
using System;


namespace ch.appl.psoft.FBW
{
    /// <summary>
    /// Summary description for FunctionRating.
    /// </summary>
    public partial class FunctionRating : PsoftDetailPage
	{

        public FunctionRating() : base()
        {
            ShowProgressBar = false;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BreadcrumbCaption = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_BC_FUNCTIONRATING);

			// Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

			// Setting content layout of page layout
            DGLContentLayout contentLayout = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;

			// Setting parameters
			FunctionRatingCtrl frCtrl = (FunctionRatingCtrl)this.LoadPSOFTControl(FunctionRatingCtrl.Path, "_funcRating");
            frCtrl.FunktionID = ch.psoft.Util.Validate.GetValid(Request.QueryString["functionID"], frCtrl.FunktionID);


			//Setting content layout user controls
			SetPageLayoutContentControl(DGLContentLayout.DETAIL, frCtrl);
	            
            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();
                if (frCtrl.FunktionID > 0)
                {
                    PsoftPageLayout.PageTitle = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_PT_FUNCTIONRATING).Replace("#1", db.lookup(db.langAttrName("FUNKTION","TITLE"), "FUNKTION", "ID=" + frCtrl.FunktionID, false));
                    frCtrl.FbwID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "FUNKTIONSBEWERTUNG", "FUNKTION_ID=" + frCtrl.FunktionID + " and GUELTIG_AB<GetDate() and GUELTIG_BIS>GetDate()", false), -1L);
                    if (frCtrl.FbwID > 0)
                    {
                        PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintFunctionRating.aspx?fbwID=" + frCtrl.FbwID + "');");
                        PsoftPageLayout.ButtonPrintVisible = true;
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

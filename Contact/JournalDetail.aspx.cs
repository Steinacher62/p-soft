using ch.appl.psoft.Common;
using ch.appl.psoft.Contact.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Contact
{
    public partial class JournalDetail : PsoftDetailPage
    {
        // Query string variables
        private long _ID = -1;
        private long _UID = -1;
        private long _contactID = -1;
        private string _orderColumn = "CREATED";
        private string _orderDir = "asc";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _ID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"], -1);
            _UID = ch.psoft.Util.Validate.GetValid(Request.QueryString["UID"], -1);
            _contactID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contactID"], -1);
            _orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
            _orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
        }

		#endregion
		
        protected void Page_Load(object sender, System.EventArgs e)
        {

            // Setting default breadcrumb caption
            /*
			// cst, 090713: multilanguage breadcrumbcaption added
			BreadcrumbCaption = "Journal detail";
			*/
			BreadcrumbCaption = _mapper.get("contact", "ctContactJournalDetail");
			

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();

                if (_UID > 0)
                    _ID = db.UID2ID(_UID);

                if (_ID <= 0)
                {
                    _ID = ch.psoft.Util.Validate.GetValid(db.lookup("JOURNAL_ID", "JOURNAL_CONTACT_V", "CONTACT_ID=" + _contactID + " order by " + _orderColumn + " " + _orderDir, false), -1);
                }

				//cst, 090713: commented out if, load in every case for correct representation
                //if (_ID > 0)
                //{
                // Loading and setting properities of content user controls
				JournalDetailCtrl detailControl = (JournalDetailCtrl)LoadPSOFTControl(JournalDetailCtrl.Path, "_jd");
				detailControl.JournalID = _ID;
				
				//cst, 090713: added logic for setting of BreadcrumbCaption - only set, if there is more than one 
				//journal_entry for that contact, otherwise let the default
				string countstr = db.lookup("COUNT(JOURNAL_ID)", "JOURNAL_CONTACT_V", "CONTACT_ID=" + _contactID, false);
				int count = Convert.ToInt32(countstr);
				if(count > 0)
					BreadcrumbCaption = detailControl.getTitle();
    
				JournalList jList = (JournalList)this.LoadPSOFTControl(JournalList.Path, "_jlst");
                jList.JournalID = _ID;
                jList.ContactID = _contactID;
                jList.OrderColumn = _orderColumn;
                jList.OrderDir = _orderDir;
                jList.SortURL = Global.Config.baseURL + "/Contact/JournalDetail.aspx?id=" + _ID + "&contactID=" + _contactID;
                jList.DetailURL = Global.Config.baseURL + "/Contact/JournalDetail.aspx?id=%JOURNAL_ID&contactID=" + _contactID + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                jList.DetailEnabled = true;
                
                //Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.DETAIL, detailControl);
                SetPageLayoutContentControl(DGLContentLayout.GROUP, jList);			
                //}

                PsoftPageLayout.PageTitle = BreadcrumbCaption;
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
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

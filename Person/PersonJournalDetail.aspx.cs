using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Person.Controls;
using ch.psoft.Util;
using System;
using System.Web;

namespace ch.appl.psoft.Person
{
    public partial class PersonJournalDetail : PsoftDetailPage
    {
        // Query string variables
        private long _contextID = -1;
		private long _journalID = -1;
        private string _orderColumn = "CREATED";
        private string _orderDir = "asc";

        #region Protected overridden methods from parent class
        protected override void Initialize()
        {
            // base initialize
            base.Initialize();

            // Retrieving query string values
            _contextID = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextID"], -1);
			_journalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["journalID"], -1);
            _orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
            _orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
        }

		

		#endregion
		
        protected void Page_Load(object sender, System.EventArgs e)
        {

            // Setting default breadcrumb caption
            BreadcrumbCaption = _mapper.get("journal", "journalDetail");

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            PageLayoutControl.ContentLayoutControl = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");

            DBData db = DBData.getDBData(Session);
            try
            {
                db.connect();

				if (_journalID <= 0) {
                   

                   if (Global.isModuleEnabled("spz"))
                   {
                       Int32 maMainJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + _contextID + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                       long maJobOrgId = Convert.ToInt32(db.lookup("ORGENTITY_ID", "JOB", "ID =" + maMainJobId.ToString()));
                       Int32 userJobId = Convert.ToInt32(db.lookup("JOB.ID", "EMPLOYMENT INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "EMPLOYMENT.PERSON_ID = " + db.userId + " AND (JOB.HAUPTFUNKTION = 1 OR JOB.HAUPTFUNKTION IS NULL)").ToString());
                       long userJobOrgId = Convert.ToInt32(db.lookup("ORGENTITY_ID", "JOB", "ID =" + userJobId.ToString()));

                       // If orgentity "Pflege" User can read all journals
                       string idsPflege = db.Orgentity.addAllSubOEIDs("92007");
                       if (idsPflege.IndexOf(userJobOrgId.ToString()) > 0 & (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "JOB", userJobId, DBData.APPLICATION_RIGHT.PERFORMANCE_RATING, true, true)))
                       {
                           _journalID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "PERSON_JOURNAL", "PERSON_ID=" + _contextID + " order by " + _orderColumn + " " + _orderDir, false), -1);
                       }
                       else
                       {
                           _journalID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "PERSON_JOURNAL", "PERSON_ID=" + _contextID + " AND CREATOR_PERSON_ID=" + db.userId + " order by " + _orderColumn + " " + _orderDir, false), -1);
                       }
                   }
                   else
                   {
                       _journalID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "PERSON_JOURNAL", "PERSON_ID=" + _contextID + " AND CREATOR_PERSON_ID=" + db.userId + " order by " + _orderColumn + " " + _orderDir, false), -1);
                   }
                     }

                
                PsoftPageLayout.ButtonPrintVisible = true;
                PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.location.href('../report/CrystalReportViewer.aspx?alias=Journal.rpt&param1=" + _journalID + "');");
              
				PersonJournalDetailCtrl detailControl = (PersonJournalDetailCtrl)LoadPSOFTControl(PersonJournalDetailCtrl.Path, "_pjd");
				detailControl._journalID = _journalID;
			
				string countstr = db.lookup("COUNT(ID)", "PERSON_JOURNAL", "PERSON_ID=" + _contextID, false);
				int count = Convert.ToInt32(countstr);
				if(count > 0)
					BreadcrumbCaption = detailControl.getTitle();

				PersonJournalList pjl = (PersonJournalList) LoadPSOFTControl(PersonJournalList.Path,"_pjl");
				pjl._journalID = _journalID;
				pjl._contextID = _contextID;
				pjl.OrderColumn = _orderColumn;
				pjl.OrderDir = _orderDir;
				pjl.SortURL = Global.Config.baseURL + "/Person/PersonJournalDetail.aspx?journalid=" + _journalID + "&contextID=" + _contextID;
				pjl.DetailURL = Global.Config.baseURL + "/Person/PersonJournalDetail.aspx?journalid=%ID" + "&contextID=" + _contextID + "&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                pjl.EditURL = pjl.EditURL + HttpUtility.UrlEncode(Request.RawUrl);
				pjl.DetailEnabled = true;
            
				//Setting content layout user controls
				SetPageLayoutContentControl(DGLContentLayout.DETAIL, detailControl);
				SetPageLayoutContentControl(DGLContentLayout.GROUP, pjl);			

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

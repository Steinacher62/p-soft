using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.psoft.Util;
using System;
using System.Web;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for JobExpectation.
    /// </summary>
    public partial class JobExpectation : PsoftDetailPage
	{
		// User controls variables
		private PsoftLinksControl _links = null;
		private ExpectationListView _pList = null;

		// Query string variables
		private long _jobID = -1;
		private long _expectationID = -1;
		private string _mode = "detail";
		private string _orderColumn = "VALID_DATE";
		private string _orderDir = "desc";


		#region Protected overrided methods from parent class
		protected override void Initialize()
		{
			// base initialize
			base.Initialize();

			// Retrieving query string values
			_jobID = ch.psoft.Util.Validate.GetValid(Request.QueryString["ID"], _jobID);
			_expectationID = ch.psoft.Util.Validate.GetValid(Request.QueryString["expectationID"], _expectationID);
			_mode = ch.psoft.Util.Validate.GetValid(Request.QueryString["mode"], _mode);
			_orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
			_orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);
		}

		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
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
				BreadcrumbCaption = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_JOBEXPECTATION + _mode.ToLower());
                BreadcrumbName += _mode.ToLower();

				// Setting page-title
				((PsoftPageLayout)PageLayoutControl).PageTitle = _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_JOBEXPECTATION) + " - " + db.lookup(db.langAttrName("JOB","TITLE"), "JOB", "ID=" + _jobID, false);

				// Setting links control for given person 
				_links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
				_links.LinkGroup1.Caption = _mapper.get("actions");

				//job expectations
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "JOB", _jobID, DBData.APPLICATION_RIGHT.JOB_EXPECTATION, true, true)){
                    _links.LinkGroup1.AddLink(_mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_JOBEXPECTATION), 
                        _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE,"addExpectation"), 
                        "/Performance/JobExpectation.aspx?id=" + _jobID + "&mode=add");
                    _links.LinkGroup1.AddLink(_mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, PerformanceModule.LANG_MNEMO_JOBEXPECTATION), _mapper.get(PerformanceModule.LANG_SCOPE_PERFORMANCE, "copyJobExpectations"),
                        "/Performance/CopyJobExpectations.aspx?jobID=" + _jobID + "&backURL=" + HttpUtility.UrlEncode(Request.RawUrl));
                }

                if (_expectationID <= 0 && _mode.ToLower() == "detail"){
                    _expectationID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "JOB_EXPECTATION", "JOB_REF=" + _jobID + " order by " + _orderColumn + " " + _orderDir, false), -1);
                }

				_pList = (ExpectationListView)this.LoadPSOFTControl(ExpectationListView.Path, "_exlst");
				_pList.JobID = _jobID;
                _pList.Mode = "job";
				_pList.OrderColumn = _orderColumn;
				_pList.OrderDir = _orderDir;
				_pList.SortURL = Global.Config.baseURL + "/Performance/JobExpectation.aspx?id=" + _jobID + "&mode=" + _mode + "&expectationID=" + _expectationID;
                _pList.DetailURL = "JobExpectation.aspx?ID=" + _jobID + "&expectationID=%ID&mode=detail&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                _pList.EditURL = "JobExpectation.aspx?ID=" + _jobID + "&expectationID=%ID&mode=edit&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
                _pList.DetailEnabled = true;
                _pList.HighlightRecordID = _expectationID;
                _pList.EditEnabled = db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "JOB", _jobID, DBData.APPLICATION_RIGHT.JOB_EXPECTATION, true, true);
                _pList.DeleteEnabled = db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "JOB", _jobID, DBData.APPLICATION_RIGHT.JOB_EXPECTATION, true, true);

				switch(_mode.ToLower())
				{
					case "add":
						// Loading and setting properities of content user controls
						ExpectationAddView  ed = (ExpectationAddView)this.LoadPSOFTControl(ExpectationAddView.Path, "_ea");
						ed.JobID = _jobID;
						ed.BackUrl = "JobExpectation.aspx?id=" + _jobID + "&mode=add";
						SetPageLayoutContentControl(DGLContentLayout.DETAIL, ed);
						break;
					case "detail":
						// Loading and setting properities of content user controls
						ExpectationDetailView  dv = (ExpectationDetailView)this.LoadPSOFTControl(ExpectationDetailView.Path, "_dv");
						dv.ExpectationID = _expectationID;
						SetPageLayoutContentControl(DGLContentLayout.DETAIL, dv);
						break;
					case "edit":
						// Loading and setting properities of content user controls
						ExpectationEditView  ev = (ExpectationEditView)this.LoadPSOFTControl(ExpectationEditView.Path, "_ed");
						ev.ExpectationID = _expectationID;
						ev.BackUrl = "JobExpectation.aspx?id=" + _jobID + "&expectationID=" + _expectationID + "&mode=detail";
						SetPageLayoutContentControl(DGLContentLayout.DETAIL, ev);
						break;
					default:
						break;
				}


				//Setting content layout user controls
				SetPageLayoutContentControl(DGLContentLayout.GROUP, _pList);
				SetPageLayoutContentControl(DGLContentLayout.LINKS, _links);
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

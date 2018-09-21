using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Tasklist.Controls;
using ch.psoft.Util;
using System;
using System.Web;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Tasklist
{
    /// <summary>
    /// Summary description for MeasureDetail.
    /// </summary>
    public partial class MeasureDetail : PsoftTreeViewPage
	{
		private const string PAGE_URL = "/Tasklist/MeasureDetail.aspx";

		static MeasureDetail()
		{
            SetPageParams(PAGE_URL, "context", "xID", "ID", "rootID", "orderColumn", "orderDir", "hideTasklist");
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		protected string _context = "tasklist";
        protected long _xID = -1;
        protected long _ID = -1;

        #region Protected overrided methods from parent class
        public MeasureDetail() : base()
		{
			PageURL = PAGE_URL;
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{
            _context = GetQueryValue("context", _context).ToLower();
            _xID = GetQueryValue("xID", _xID);
            _ID = GetQueryValue("ID", _ID);
            bool template = false;
            
            BreadcrumbCaption = _mapper.get("tasklist","measuredetail");

			// Setting main page layout
			PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            (PageLayoutControl as PsoftPageLayout).PageTitle = _mapper.get("tasklist","measuredetail");

            // Adding javascript for XML report
//            (PageLayoutControl as PsoftPageLayout).ButtonPrintAttributes.Add("onClick", 
//                "javascript: window.open('CreateReport.aspx?context=measure&xID=" + 
//                GetQueryValue("xID", "0") + "')");
//            (PageLayoutControl as PsoftPageLayout).ButtonPrintVisible = true;
            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout)this.LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.DetailLeftWidth = Unit.Percentage(30);
            contentLayout.GroupWide = true;

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                // verify ID und Vorlagen mit adäquatem Kontext
                object [] values = db.lookup(new string [] {"ID", "TEMPLATE"}, "MEASURE", "ID=" + _ID);
                _ID = DBColumn.GetValid(values[0], (long)-1);
                template = DBColumn.GetValid(values[1], "0") == "1";

                switch (_context){
					case psoft.Project.Controls.ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
                    case "tasklist":
                        if (template)
                        {
                            if (_xID <= 0)
                            {
                                _xID = DBColumn.GetValid(db.lookup("TASKLIST_ID", "MEASURE", "ID=" + _ID), 0L);
                            }
                            if (_ID <= 0L)
                            {
                                _ID = DBColumn.GetValid(db.lookup("ID", "MEASURE", "TASKLIST_ID=" + _xID), 0L);
                            }
                        }
                        else
                        {
                            if (_xID <= 0)
                            {
                                _xID = DBColumn.GetValid(db.lookup("TASKLIST_ID", "MEASURE", "ID=" + _ID + (SessionData.showDoneMeasures(Session)? "" : " and STATE=0")),0L);
                            }
                            if (_ID <= 0L)
                            {
                                _ID = DBColumn.GetValid(db.lookup("ID", "MEASURE", "TASKLIST_ID=" + _xID + (SessionData.showDoneMeasures(Session)? "" : " and STATE=0")),0L);
                            }
                        }
                        break;
                    case "selection":
                        if (_ID <= 0L){
                            _ID = DBColumn.GetValid(db.lookup("row_id", "searchresult", "id=" + _xID),0L);
                        }
                        break;

                    case "responsible":
                        if (_ID <= 0L){
                            _ID = DBColumn.GetValid(db.lookup("ID", "MEASURE", "RESPONSIBLE_PERSON_ID=" + _xID + (SessionData.showDoneMeasures(Session)? "" : " and STATE=0")), 0L);
                        }
                        break;
                }
                
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "MEASURE", _ID, true, true)){
                    // Setting parameters
			        PendenzenMeasureDetail _detail = (PendenzenMeasureDetail)this.LoadPSOFTControl(PendenzenMeasureDetail.Path, "_detail");
			        _detail.MeasureID = _ID;
                    _detail.Template = template;
					_detail.HideTasklist = Boolean.Parse(GetQueryValue("hideTasklist", "false"));

			        // Setting content layout user controls
			        SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, _detail);		

                    PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                    SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);	
                    links.LinkGroup1.Caption = _mapper.get("tasklist", "cmtMeasure");

                    // Load tree control
                    TasklistTreeCtrl tree = (TasklistTreeCtrl) LoadPSOFTControl(TasklistTreeCtrl.Path, "_tree");
                    tree.Template = template;
                    tree.SelectedID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_ID", "MEASURE", "ID=" + _ID, false), -1L);
                    tree.RootID = GetQueryValue("rootID", (long)-1);
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);

                    // Load list control
                    PendenzenMeasureList _list = (PendenzenMeasureList)this.LoadPSOFTControl(PendenzenMeasureList.Path, "_list");
                    SetPageLayoutContentControl(DDGLContentLayout.GROUP, _list);
                    _list.Kontext = _context;
                    _list.Template = template;
                    _list.SelectedID = _ID;
                    _list.XID = _xID;
                    _list.RootID = tree.RootID;
                    _list.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
                    _list.OrderDir = GetQueryValue("orderDir", "asc");
				    _list.SortURL = psoft.Tasklist.MeasureDetail.GetURL(
						    "context", _context,
						    "xID", _xID,
						    "ID", _ID,
						    "rootID", tree.RootID
					    );
				    _list.SortEnabled = true;
                    _list.DeleteURL = Request.RawUrl;
					_list.HideTasklist = Boolean.Parse(GetQueryValue("hideTasklist", "false"));

                    //list-button
                    _list.changeButtonList();

                    if (_ID > 0L){
                        links.LinkGroup1.AddLink(
						    _mapper.get("actions"),
						    _mapper.get("tasklist", "editmeasure"),
						    psoft.Tasklist.EditMeasure.GetURL("id", _ID, "backURL", Request.RawUrl)
					    );
                        links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("tasklist", "deletemeasure"), "javascript: deleteMeasureConfirm(" + _ID + ")");

                        if (Global.isModuleEnabled("contact")){
                            long contactGroupID = ch.psoft.Util.Validate.GetValid(db.lookup("CONTACT_GROUP_ID", "MEASURE", "ID=" + _ID, false), -1L);
                            if (contactGroupID > 0){
                                links.LinkGroup1.AddLink(_mapper.get("tasklist", "cmtContacts"), _mapper.get("tasklist", "cmContactGroup"), "/Contact/ContactDetail.aspx?mode=contactgroup&xID=" + contactGroupID);
                            }
                            else{
                                links.LinkGroup1.AddLink(_mapper.get("tasklist", "cmtContacts"), _mapper.get("tasklist", "cmAssignContactGroup"), "/Contact/ContactGroupAdd.aspx?ownerTable=MEASURE&ownerID=" + _ID + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?mode=contactgroup&xID=%ID"));
                            }
                        }

                        //     clipboard
                        object[] objs = db.lookup(new string[] {"CLIPBOARD_ID","UID"}, "MEASURE", "ID=" + _ID);
                        long clipboardID = DBColumn.GetValid(objs[0],-1L);
                        long triggerUID = DBColumn.GetValid(objs[1],-1L);
                        if (clipboardID > 0) {
                            if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardID, true, true)){
                                //     open clipboard
                                links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ID",clipboardID, "ownerTable","MEASURE"));
                            }
                        }
                        else {
                            //     create clipboard
                            links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("createClipboard"), psoft.Document.CreateClipboardM.GetURL("type",Clipboard.TYPE_PRIVATE, "ownerTable","MEASURE", "ownerID",_ID, "triggerUID",triggerUID));
                        }
                        //  subscription
                        if (!template
                            && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "MEASURE", _ID, true, true)
                        )
                        {
                            links.LinkGroup1.AddAboLinks("MEASURE", _ID, _mapper);
                        }

                        (PageLayoutControl as PsoftPageLayout).ShowButtonAuthorisation("MEASURE", _ID);
                    }
                    else{
                        BreadcrumbVisible = false;
                        Response.Redirect(NotFound.GetURL(), false);
                    }
                }
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally{
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

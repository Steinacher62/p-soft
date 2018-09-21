using ch.appl.psoft.Common;
using ch.appl.psoft.db;
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
    /// Summary description for QuelleDetail.
    /// </summary>
    public partial class TaskDetail : PsoftTreeViewPage
	{
		private const string PAGE_URL = "/Tasklist/TaskDetail.aspx";

		static TaskDetail()
		{
            SetPageParams(
				PAGE_URL,
				"context",
				"xID",
				"ID",
				"rootID",
				"showRoot",
				"assignPerson",
				"orderColumn",
				"orderDir",
				"assignMeasure",
				"subMenu",
				"modifTasklist"
			);
		}

		public static string GetURL(params object[] queryParams)
		{
			return CreateURL(PAGE_URL, queryParams);
		}

		private PsoftLinksControl _links = null;
        protected string _context = "tasklist";
        protected long _xID = -1;
        protected long _ID = -1;

        #region Protected overrided methods from parent class
        public TaskDetail() : base() {
			PageURL = PAGE_URL;
        }
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _context = GetQueryValue("context", _context).ToLower();
            _xID = GetQueryValue("xID", _xID);
            _ID = GetQueryValue("ID", _ID);
            bool template = false;
            string title = "";

            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                // verify ID und Vorlagen mit adäquatem Kontext
                object [] values = db.lookup(new string [] {"ID", "TEMPLATE"}, "TASKLIST", "ID=" + _ID);
                _ID = DBColumn.GetValid(values[0], (long)-1);
                template = DBColumn.GetValid(values[1], "0") == "1";

                if (template)
                {
                    title = _mapper.get("tasklist","tasklistdetailtemplate");
                    BreadcrumbName = "tasklistdetailtemplate";
                }
                else
                {
                    title = _mapper.get("tasklist","tasklistdetail");
                    BreadcrumbName = "tasklistdetail";
                }

                BreadcrumbCaption = title;

                // Setting main page layout
                PageLayoutControl = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
                (PageLayoutControl as PsoftPageLayout).PageTitle = title;

                switch (_context)
				{
                    case "oe":
                        _context = "tasklistgroup";
                        _xID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_GROUP_ID", "ORGENTITY", "ID=" + _xID, false), -1L);
                        _ID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_ID","TASKLIST_GROUP_TASKLIST","TASKLIST_GROUP_ID="+_xID, false), -1L);
                        break;

                    case "selection":
                        if (_ID <= 0L){
                            _ID = DBColumn.GetValid(db.lookup("row_id","searchresult","id="+_xID),0L);
                        }
                        break;

					case "tasklistgroup":
						if (_ID <= 0L)
						{
							_ID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_ID","TASKLIST_GROUP_TASKLIST","TASKLIST_GROUP_ID="+_xID, false), -1L);
						}

						break;
				}

                // Adding javascript for XML report
//                (PageLayoutControl as PsoftPageLayout).ButtonPrintAttributes.Add(
//						"onClick",
//						"javascript: window.open('"
//							+ Psoft.Tasklist.CreateReport.GetURL("context", "tasklist", "xID", _ID)
//							+ "')"
//					);
//                (PageLayoutControl as PsoftPageLayout).ButtonPrintVisible = true;

                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "TASKLIST", _ID, true, true)){
                    // Setting content layout of page layout
                    PageLayoutControl.ContentLayoutControl = (DDGLContentLayout)this.LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
                    ((DDGLContentLayout)PageLayoutControl.ContentLayoutControl).DetailLeftWidth = Unit.Percentage(30);

			        // Setting parameters
			        PendenzenTaskDetail _detail = (PendenzenTaskDetail)this.LoadPSOFTControl(PendenzenTaskDetail.Path, "_detail");
			        _detail.TasklistID = _ID;
                    _detail.Template = template;

                    TasklistTreeCtrl tree = (TasklistTreeCtrl) LoadPSOFTControl(TasklistTreeCtrl.Path, "_tree");
                    tree.SelectedID = _ID;
                    tree.RootID = GetQueryValue("rootID", (long)0);
                    tree.ShowRoot = bool.Parse(GetQueryValue("showRoot", "True"));
				    tree.Template = template;

			        // Setting content layout user controls
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, _detail);		
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);		

                    string assignPerson = GetQueryValue("assignPerson", "enable");

                    // Load list control
                    switch (_context){
                        case "tasklistgroup":
                        case "selection":
                            PendenzenTaskList tlist = (PendenzenTaskList)this.LoadPSOFTControl(PendenzenTaskList.Path, "_list");
                            tlist.Kontext = _context;
                            tlist.SelectedID = _ID;
                            tlist.XID = _xID;
                            tlist.RootID = tree.RootID;
                            tlist.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
                            tlist.OrderDir = GetQueryValue("orderDir", "asc");
                            tlist.SortURL = psoft.Tasklist.TaskDetail.GetURL(
								    "context", _context,
								    "xID", _xID,
								    "ID", _ID,
								    "rootID", tree.RootID
							    );
                            tlist.SortEnabled = true;
                            tlist.DetailURL = psoft.Tasklist.TaskDetail.GetURL(
								    "context", _context,
								    "xID", _xID,
								    "ID", "%ID",
								    "orderColumn", tlist.OrderColumn,
								    "orderDir", tlist.OrderDir
							    );
                            tlist.DeleteURL = Request.RawUrl;

                            // Setting content layout
                            SetPageLayoutContentControl(DDGLContentLayout.GROUP, tlist);
                            break;

                        default:
                            PendenzenMeasureList mlist = (PendenzenMeasureList)this.LoadPSOFTControl(PendenzenMeasureList.Path, "_list");
						    mlist.Kontext = "tasklist";
                            mlist.Template = template;
                            mlist.XID = _ID;
                            mlist.RootID = tree.RootID;
                            mlist.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
                            mlist.OrderDir = GetQueryValue("orderDir", "asc");
                            mlist.AssignMeasure = GetQueryValue("assignMeasure", "enable") == "enable";
                            mlist.AssignPerson = assignPerson;
                            mlist.SortURL = psoft.Tasklist.TaskDetail.GetURL(
							    "context", mlist.Kontext,
							    "xID", _xID,
							    "ID", _ID,
							    "rootID", tree.RootID
							    );
						    mlist.SortEnabled = true;
                            mlist.SubMenuEnable = GetQueryValue("subMenu" ,"enable") == "enable";
                            mlist.DeleteURL = Request.RawUrl;

                            // Setting content layout
                            SetPageLayoutContentControl(DDGLContentLayout.GROUP, mlist);

                            //list-button
                            mlist.changeButtonList();
                            break;
                    }
                    
                    if (_ID > 0L){
                        _links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
                        _links.LinkGroup1.Caption = _mapper.get("tasklist", "cmtTasklist");
                        _links.LinkGroup2.Caption = _mapper.get("tasklist", "measures");

                        if (GetQueryValue("modifTasklist", "enable") == "enable") {
                            _links.LinkGroup1.AddLink(
                                _mapper.get("new"), 
                                _mapper.get("tasklist", "newSubTasklist"),
                                psoft.Tasklist.AddTasklist.GetURL(
                                "parentId", _ID,
                                "template", template ? "true" : "false"
                                )
                                );
                            _links.LinkGroup1.AddLink(
                                _mapper.get("new"),
                                _mapper.get("tasklist", "assignTasklist"),
                                psoft.Tasklist.Search.GetURL("mode", "assign", "xID", _ID, "rootID", tree.RootID)
                                );
                            _links.LinkGroup1.AddLink(
                                _mapper.get("actions"),
                                _mapper.get("tasklist", "edittasklist"),
                                psoft.Tasklist.EditTasklist.GetURL(
                                "id", _ID,
                                "backURL", Request.RawUrl
                                )
                                );

                            if (db.Tasklist.isAssignedTasklist(_ID, tree.RootID)) {
                                _links.LinkGroup1.AddLink(
                                    _mapper.get("actions"), 
                                    _mapper.get("tasklist", "removeAssignedtasklist"),
                                    psoft.Tasklist.RemoveTasklistAssignment.GetURL("id", _ID, "rootID", tree.RootID)
                                    );
                            }
                            else {
                                _links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("tasklist", "deletetasklist"), "javascript: deleteTasklistConfirm(" + _ID + ")");
    							
                                if (!template) {
                                    _links.LinkGroup1.AddLink(
                                        _mapper.get("actions"),
                                        _mapper.get("tasklist", "tasklistToTemplate"),
                                        psoft.Tasklist.TasklistToTemplate.GetURL("ID", _ID)
                                        );
                                }
                            }

                        }

                        _links.LinkGroup2.AddLink(
                            _mapper.get("new"),
                            _mapper.get("tasklist", "newmeasure"),
                            psoft.Tasklist.AddMeasure.GetURL(
                            "assignPerson", assignPerson,
                            "tasklistID", _ID,
                            "backURL", Request.RawUrl,
                            "template", template ? "true" : "false"
                            )
                            );

                        if (Global.isModuleEnabled("contact")){
                            long contactGroupID = ch.psoft.Util.Validate.GetValid(db.lookup("CONTACT_GROUP_ID", "TASKLIST", "ID=" + _ID, false), -1L);
                            if (contactGroupID > 0){
                                _links.LinkGroup1.AddLink(_mapper.get("tasklist", "cmtContacts"), _mapper.get("tasklist", "cmContactGroup"), "/Contact/ContactDetail.aspx?mode=contactgroup&xID=" + contactGroupID);
                            }
                            else{
                                _links.LinkGroup1.AddLink(_mapper.get("tasklist", "cmtContacts"), _mapper.get("tasklist", "cmAssignContactGroup"), "/Contact/ContactGroupAdd.aspx?ownerTable=TASKLIST&ownerID=" + _ID + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?mode=contactgroup&xID=%ID"));
                            }
                        }

                        //     clipboard
                        object[] objs = db.lookup(new string[] {"CLIPBOARD_ID","UID"}, "TASKLIST", "ID=" + _ID);
                        long clipboardID = DBColumn.GetValid(objs[0],-1L);
                        long triggerUID = DBColumn.GetValid(objs[1],-1L);
                        if (clipboardID > 0) {
                            if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardID, true, true)){
                                //     open clipboard
                                _links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ID",clipboardID, "ownerTable","TASKLIST"));
                            }
                        }
                        else {
                            //     create clipboard
                            _links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("createClipboard"), psoft.Document.CreateClipboardM.GetURL("type",Clipboard.TYPE_PRIVATE, "ownerTable","TASKLIST", "ownerID",_ID, "triggerUID",triggerUID));
                        }
                        
                        //  oe
                        _links.LinkGroup1.AddLink(_mapper.get("organisation","oe"), _mapper.get("tasklist","oeAssignTasklistGroup"), "/Organisation/AssignGroups.aspx?ownerTable=TASKLIST&ownerID=" + _ID + "&backURL=" + HttpUtility.UrlEncode(Request.RawUrl) + "&nextURL=" + HttpUtility.UrlEncode(Request.RawUrl));

                        //  subscription
                        if (!template
                            && db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "TASKLIST", _ID, true, true)
                            ) {
                            _links.LinkGroup1.AddAboLinks("TASKLIST", _ID, _mapper);
                        }
                        SetPageLayoutContentControl(DDGLContentLayout.LINKS, _links);
                        (PageLayoutControl as PsoftPageLayout).ShowButtonAuthorisation("TASKLIST", _ID);
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

		private long tasklistToTemplate(long tasklistId)
		{
			long returnValue = -1;
			DBData db = DBData.getDBData(Session);
			db.connect();
			db.beginTransaction();

			try 
			{
				returnValue = db.Tasklist.copyAsTemplate(tasklistId);
				db.commit();
			}
            catch(Exception ex)
			{
                db.rollback();
				Logger.Log(ex, Logger.ERROR);
                ShowError(ex.Message);
            }
            finally
			{
                db.disconnect();
            }

			return returnValue;
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

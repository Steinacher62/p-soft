using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using ch.psoft.Util;
using System;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Project
{

    public partial class PhaseDetail : PsoftTreeViewPage
	{	
        private const string PAGE_URL = "/Project/PhaseDetail.aspx";

        static PhaseDetail() {
            SetPageParams(PAGE_URL, "ID", "context", "xID", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public PhaseDetail() : base() {
            PageURL = PAGE_URL;
        }

		protected string _context = "";
		protected long _xID = -1L;
		protected long _phaseID = -1L;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_context = GetQueryValue("context", _context).ToLower();
			_xID = GetQueryValue("xID", _xID);
			_phaseID = GetQueryValue("ID", -1L);

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;

            // Setting content layout of page layout
            DDGLContentLayout contentLayout = (DDGLContentLayout) LoadPSOFTControl(DDGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = contentLayout;
            contentLayout.DetailLeftWidth = Unit.Percentage(30);

            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
				switch (_context)
				{
					case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
						if (ch.psoft.Util.Validate.GetValid(db.lookup("ID", "MEASURE", "ID=" + _xID, false), -1L) <= 0)
						{
							_xID = -1;
							_context = "";
						}
						break;
				}
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PHASE", _phaseID, true, true)){
					ProjectTreeCtrl tree = (ProjectTreeCtrl) LoadPSOFTControl(ProjectTreeCtrl.Path, "_tree");
					tree.PhaseID = _phaseID;
					long projectID = tree.ProjectID = ch.psoft.Util.Validate.GetValid(db.lookup("PROJECT_ID", "PHASE", "ID=" + _phaseID, false), -1L);
					psoft.Tasklist.Controls.PendenzenMeasureList measureList = null;
					PhaseDependencyListCtrl phaseList = null;

                    // Setting parameters
					psoft.Tasklist.Controls.PendenzenMeasureDetail detailTL = null;
					PhaseDetailCtrl detail = null;
					switch(_context)
					{
						case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
							detailTL = (psoft.Tasklist.Controls.PendenzenMeasureDetail)  LoadPSOFTControl(psoft.Tasklist.Controls.PendenzenMeasureDetail.Path, "_detTL");
							detailTL.Template = false;
							detailTL.MeasureID = _xID;
							detailTL.HideTasklist = true;
							break;
						default:
							detail = (PhaseDetailCtrl) LoadPSOFTControl(PhaseDetailCtrl.Path, "_det");
							detail.PhaseID = _phaseID; 
							detail.PostDeleteURL = projectID > 0 ? psoft.Project.ProjectDetail.GetURL("ID",projectID) : psoft.Project.ProjectSearch.GetURL();
							break;
					}
                   

//                    // Load list control
//                    PhaseList list = (PhaseList) LoadPSOFTControl(PhaseList.Path, "_list");
//                    list.ProjectID = tree.ProjectID;
//                    list.PhaseID = detail.PhaseID;
//                    list.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
//                    list.OrderDir = GetQueryValue("orderDir", "asc");
//                    list.SortURL = Psoft.Project.PhaseDetail.GetURL("ID",detail.PhaseID);
//                    list.PostDeleteURL = Psoft.Project.ProjectDetail.GetURL("ID",tree.ProjectID);
//                    list.DetailURL = Psoft.Project.PhaseDetail.GetURL("ID","%ID", "OrderColumn",list.OrderColumn, "OrderDir",list.OrderDir);
//                    list.EditURL = Global.Config.baseURL + "/Project/PhaseEdit.aspx?ID=%ID";

					switch(_context.ToLower())
					{
						case "dependency":
							phaseList = (PhaseDependencyListCtrl) LoadPSOFTControl(PhaseDependencyListCtrl.Path, "_list");
							phaseList.PhaseID = _phaseID;
							phaseList.OrderColumn = GetQueryValue("orderColumn", "MASTER_STARTDATE");
							phaseList.OrderDir = GetQueryValue("orderDir", "asc");
							phaseList.DetailEnabled = false;
							phaseList.EditEnabled = false;
							phaseList.PostDeleteURL = psoft.Project.PhaseDetail.GetURL("ID",_phaseID,"context","dependency");
							break;
						default:
							measureList = (psoft.Tasklist.Controls.PendenzenMeasureList)this.LoadPSOFTControl(psoft.Tasklist.Controls.PendenzenMeasureList.Path, "_list");
							measureList.Kontext = ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE;//"tasklist";
							measureList.Template = false;
							measureList.XID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_ID", "PHASE", "ID=" + _phaseID, true), -1L);
							measureList.RootID = ch.psoft.Util.Validate.GetValid(db.lookup("ROOT_ID", "PROJECT", "ID=" + projectID, true), -1L);//tree.RootID;
							measureList.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
							measureList.OrderDir = GetQueryValue("orderDir", "asc");
							//					list.AssignMeasure = GetQueryValue("assignMeasure", "enable") == "enable";
							measureList.AssignPerson = "enable";
							measureList.DetailURL = GetURL("ID",_phaseID,"context",ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE,"xID","%ID");
							measureList.DeleteURL = Request.RawUrl;
							measureList.HideTasklist = true;
							break;
					}

					
					switch (_context)
					{
						case "dependency":
							break;
						case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
							measureList.SortURL = GetURL("ID",_phaseID,"context",ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE,"xID",_xID);
							measureList.SortEnabled = true;
							measureList.SelectedID = _xID;
							measureList.Visible = db.exists("MEASURE","TASKLIST_ID="+measureList.XID);
							measureList.changeButtonList(db,_mapper);
							break;						
						default:
							measureList.SortEnabled = false;
							measureList.Visible = db.exists("MEASURE","TASKLIST_ID="+measureList.XID);
							break;
					}

					switch (_context)
					{
						case "dependency":
							SetPageLayoutContentControl(DDGLContentLayout.GROUP, phaseList);
							break;
						default:
							SetPageLayoutContentControl(DDGLContentLayout.GROUP, measureList);
							break;
					}

                    PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
					object[] objs;
					long triggerUID;
					switch (_context)
					{
						case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
							links.LinkGroup1.Caption = _mapper.get("tasklist", "cmtMeasure");

							if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "MEASURE", _xID, true, true))
							{
								links.LinkGroup1.AddLink(_mapper.get("actions"),_mapper.get("tasklist", "editmeasure"),psoft.Tasklist.EditMeasure.GetURL("id", _xID, "backURL", Request.RawUrl, "HideTasklist", "true"));
							}
							if (db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "MEASURE", _xID, true, true))
							{
								links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("tasklist", "deletemeasure"), "javascript: deleteMeasureConfirm(" + _xID + ")");
							}

//							TODO: Das Erstellen von Kontaktgruppen aus diesem Kontext ist fehlerhaft
//							if (Global.isModuleEnabled("contact"))
//							{
//								long contactGroupID = ch.psoft.Util.Validate.GetValid(db.lookup("CONTACT_GROUP_ID", "MEASURE", "ID=" + _xID, false), -1L);
//								if (contactGroupID > 0)
//								{
//									if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CONTACT_GROUP", contactGroupID, true, true))
//									{
//										links.LinkGroup1.AddLink(_mapper.get("tasklist", "cmtContacts"), _mapper.get("tasklist", "cmContactGroup"), "/Contact/ContactDetail.aspx?mode=contactgroup&xID=" + contactGroupID);
//									}
//								}
//								else
//								{
//									if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "MEASURE", _xID, true, true))
//									{
//										links.LinkGroup1.AddLink(_mapper.get("tasklist", "cmtContacts"), _mapper.get("tasklist", "cmAssignContactGroup"), "/Contact/ContactGroupAdd.aspx?ownerTable=MEASURE&ownerID=" + _xID + "&nextURL=" + HttpUtility.UrlEncode("ContactDetail.aspx?mode=contactgroup&xID=%ID"));
//									}
//								}
//							}

							//     clipboard
							objs = db.lookup(new string[] {"CLIPBOARD_ID","UID"}, "MEASURE", "ID=" + _xID);
							long clipboardID = DBColumn.GetValid(objs[0],-1L);
							triggerUID = DBColumn.GetValid(objs[1],-1L);
							if (clipboardID > 0) 
							{
								if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardID, true, true))
								{
									//     open clipboard
									links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ID",clipboardID, "ownerTable","MEASURE"));
								}
							}
							else 
							{
								//     create clipboard
								if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "MEASURE", _xID, true, true))
								{
									links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("createClipboard"), psoft.Document.CreateClipboardM.GetURL("type",Clipboard.TYPE_PRIVATE, "ownerTable","MEASURE", "ownerID",_xID, "triggerUID",triggerUID));
								}
							}
							//  subscription
							if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "MEASURE", _xID, true, true))
							{
								links.LinkGroup1.AddAboLinks("MEASURE", _xID, _mapper);
							}
							break;
						default:
							objs = db.lookup(new string[] {"TASKLIST_ID","UID"}, "PHASE", "ID=" + _phaseID);
							long tasklistID = DBColumn.GetValid(objs[0],-1L);
							triggerUID = DBColumn.GetValid(objs[1],-1L);

							links.LinkGroup1.Caption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CMT_SELECTED_PHASE);

							if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "PHASE", _phaseID, true, true))
							{
								links.LinkGroup1.AddLink(_mapper.get("new"),_mapper.get("tasklist", "newmeasure"),psoft.Tasklist.AddMeasure.GetURL("assignPerson", "enable","tasklistID", tasklistID,"backURL", Request.RawUrl,"template", "false","hideTasklist","true"));
								links.LinkGroup1.AddLink(_mapper.get("new"),_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_PHASEDEPENDENCY),"/Project/PhaseDependencyAdd.aspx?phaseID=" + _phaseID);
							}
							
							if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PHASE", _phaseID, true, true))
							{
								links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_EDITPHASE), "/Project/PhaseEdit.aspx?ID=" + _phaseID);
							}
							if (db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "PHASE", _phaseID, true, true))
							{
								links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_DELETEPHASE), "javascript: deletePhaseConfirm(" + _phaseID + ")");
							}
							if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PHASE", _phaseID, true, true))
							{
								links.LinkGroup1.AddLink(_mapper.get("actions"),_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_PHASEDEPENDENCIES),"/Project/PhaseDetail.aspx?context=dependency&ID="+_phaseID.ToString());
								links.LinkGroup1.AddAboLinks("PHASE", _phaseID, _mapper);
							}
							break;
					}

                    // Setting content layout user controls
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);
					switch(_context)
					{
						case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
							SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detailTL);
							BreadcrumbCaption = _mapper.get("tasklist","measuredetail");
							PsoftPageLayout.PageTitle = db.lookup("TITLE", "PROJECT", "ID=" + tree.ProjectID, false) + " - " + db.lookup("TITLE", "PHASE", "ID=" + _phaseID, false) + " - " + BreadcrumbCaption;
							PsoftPageLayout.ShowButtonAuthorisation("MEASURE", _xID);
							break;
						default:
							SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);
							BreadcrumbCaption = db.lookup("TITLE", "PHASE", "ID=" + _phaseID, false);
							PsoftPageLayout.PageTitle = db.lookup("TITLE", "PROJECT", "ID=" + tree.ProjectID, false) + " - " + BreadcrumbCaption;
							PsoftPageLayout.ShowButtonAuthorisation("PHASE", _phaseID);
							PsoftPageLayout.ShowButtonRegistryEntries("PHASE", _phaseID, db.Registry.getRegistryIDs(DBData.BuildSQLArray(db.Project.getParentProjectIDList(projectID, true).ToArray()), "PROJECT"));
							break;
					}

					switch (_context.ToLower())
					{
						case "dependency":
							SetPageLayoutContentControl(DDGLContentLayout.GROUP, phaseList);
							break;
						default:
							SetPageLayoutContentControl(DDGLContentLayout.GROUP, measureList);
							break;
					}

                    SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);

                }
                else{
                    BreadcrumbVisible = false;
                    Response.Redirect(NotFound.GetURL(), false);
                }
            }
            catch(Exception ex)
            {
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

using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Project.Controls;
using ch.psoft.Util;
using System;
using System.Web;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Project
{

    public partial class ProjectDetail : PsoftTreeViewPage
	{
        private const string PAGE_URL = "/Project/ProjectDetail.aspx";

        static ProjectDetail() {
            SetPageParams(PAGE_URL, "ID", "context", "xID", "orderColumn", "orderDir", "billingID");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public ProjectDetail() : base() {
            PageURL = PAGE_URL;
        }

        protected string _context = "";
        protected long _xID = -1L;
        protected long _projectID = -1L;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _context = GetQueryValue("context", _context).ToLower();
            _xID = GetQueryValue("xID", _xID);
            _projectID = GetQueryValue("ID", -1L);

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
				if (_context == "oe")
				{
					_context = ProjectList.CONTEXT_PROJECTGROUP;
					_xID = ch.psoft.Util.Validate.GetValid(db.lookup("PROJECT_GROUP_ID", "ORGENTITY", "ID=" + _xID, false), -1L);
				}

                switch (_context){
                    case ProjectList.CONTEXT_PERSON:
                        string projectIDList = db.Project.getInvolvedProjects(_xID);
                        if (_projectID <= 0){
                            string[] projectIDs = projectIDList.Split(',');
                            if (projectIDs.Length > 0){
                                _projectID = ch.psoft.Util.Validate.GetValid(projectIDs[0], -1L);
                            }
                        }
                        break;

                    case ProjectList.CONTEXT_PROJECTGROUP:
                        if (_projectID <= 0){
                            _projectID = ch.psoft.Util.Validate.GetValid(db.lookup("PROJECT_ID", "PROJECT_GROUP_PROJECT", "PROJECT_GROUP_ID=" + _xID, false), -1);
                        }
                        break;

                    case ProjectList.CONTEXT_SEARCHRESULT:
                        if (_projectID <= 0){
                            _projectID = ch.psoft.Util.Validate.GetValid(db.lookup("row_id", "searchresult", "ID=" + _xID, false), -1);
                        }
                        break;

					case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
						if (ch.psoft.Util.Validate.GetValid(db.lookup("ID", "MEASURE", "ID=" + _xID, false), -1L) <= 0)
						{
							_xID = -1;
							_context = "";
						}
						break;
                }


                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "PROJECT", _projectID, true, true)){
                    ProjectTreeCtrl tree = (ProjectTreeCtrl) LoadPSOFTControl(ProjectTreeCtrl.Path, "_tree");
					PhaseDependencyListCtrl phaseList = null;
					psoft.Tasklist.Controls.PendenzenMeasureList list = null;
                    tree.ProjectID = _projectID;
					long parentID = ch.psoft.Util.Validate.GetValid(db.lookup("PARENT_ID", "PROJECT", "ID=" + tree.ProjectID, false), -1L);

					// Setting parameters
					psoft.Tasklist.Controls.PendenzenMeasureDetail detailTL = null;
					ProjectDetailCtrl detail = null;
					switch(_context)
					{
						case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
							detailTL = (psoft.Tasklist.Controls.PendenzenMeasureDetail)  LoadPSOFTControl(psoft.Tasklist.Controls.PendenzenMeasureDetail.Path, "_detTL");
							detailTL.Template = false;
							detailTL.MeasureID = _xID;
							detailTL.HideTasklist = true;
							break;
						default:
							detail = detail = (ProjectDetailCtrl) LoadPSOFTControl(ProjectDetailCtrl.Path, "_ad");
							detail.ProjectID = tree.ProjectID;
							detail.PostDeleteURL = parentID > 0 ? psoft.Project.ProjectDetail.GetURL("ID",parentID) : psoft.Project.ProjectSearch.GetURL();
							break;
					}
					
					if (_context == ProjectList.CONTEXT_PERSON || _context == ProjectList.CONTEXT_SEARCHRESULT || _context == ProjectList.CONTEXT_PROJECTGROUP)
					{
						ProjectList projList = (ProjectList) LoadPSOFTControl(ProjectList.Path, "_list");
						projList.xID = _xID;
						projList.Kontext = _context;
						projList.ProjectID = tree.ProjectID;
						projList.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
						projList.OrderDir = GetQueryValue("orderDir", "asc");
						projList.DetailURL = psoft.Project.ProjectDetail.GetURL("context",_context, "id","%ID", "xID",_xID);
						projList.DetailEnabled = true;
						projList.DeleteEnabled = false;
						projList.EditEnabled = false;
						SetPageLayoutContentControl(DDGLContentLayout.GROUP, projList);
					}
					else if(_context.ToLower().Equals("dependency"))
					{
						phaseList = (PhaseDependencyListCtrl) LoadPSOFTControl(PhaseDependencyListCtrl.Path, "_list");
						phaseList.ProjectID = _projectID;
						phaseList.OrderColumn = GetQueryValue("orderColumn", "MASTER_STARTDATE");
						phaseList.OrderDir = GetQueryValue("orderDir", "asc");
						phaseList.DetailEnabled = false;
						phaseList.EditEnabled = false;
						phaseList.PostDeleteURL = psoft.Project.ProjectDetail.GetURL("ID",_projectID,"context","dependency");
						SetPageLayoutContentControl(DDGLContentLayout.GROUP, phaseList);
					}
                    else if (_context.ToLower().Equals("billing"))
                    {
                        //the billing list (Projektrechnungen) is displayed
                        BillingListCtrl billingList = (BillingListCtrl)LoadPSOFTControl(BillingListCtrl.Path, "_list");
                        billingList.ProjectID = _projectID;
                        billingList.OrderColumn = GetQueryValue("orderColumn", "MASTER_STARTDATE");
                        billingList.OrderDir = GetQueryValue("orderDir", "asc");
                        billingList.ProjectBillingID = GetQueryValue("billingID", -1L);
                        billingList.DetailEnabled = false;
                        billingList.EditEnabled = true;
                        billingList.EditURL = psoft.Project.BillingDetail.GetURL("ID", "%ID", "projectID", _projectID);
                        billingList.PostDeleteURL = psoft.Project.ProjectDetail.GetURL("ID", _projectID, "context", "billing");
                        SetPageLayoutContentControl(DDGLContentLayout.GROUP, billingList);
                    }
                    else
					{
						list = (psoft.Tasklist.Controls.PendenzenMeasureList)this.LoadPSOFTControl(psoft.Tasklist.Controls.PendenzenMeasureList.Path, "_list");
						list.Kontext = ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE;//"tasklist";
						list.Template = false;
						list.XID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_ID", "PROJECT", "ID=" + _projectID, true), -1L);
						list.RootID = ch.psoft.Util.Validate.GetValid(db.lookup("ROOT_ID", "PROJECT", "ID=" + _projectID, true), -1L);//tree.RootID;
						list.OrderColumn = GetQueryValue("orderColumn", "DUEDATE");
						list.OrderDir = GetQueryValue("orderDir", "asc");
						list.AssignPerson = "enable";
						list.DetailURL = GetURL("ID",_projectID,"context",ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE,"xID","%ID");
						list.DeleteURL = Request.RawUrl;
						list.HideTasklist = true;
						switch (_context)
						{
							case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
								list.SortURL = GetURL("ID",_projectID,"context",ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE,"xID",_xID);
								list.SortEnabled = true;
								list.SelectedID = _xID;
								list.changeButtonList(db,_mapper);
								break;
							default:
								list.SortEnabled = false;
								break;
						}

						if (!db.exists("MEASURE","TASKLIST_ID="+list.XID))
							list.Visible = false;	
						SetPageLayoutContentControl(DDGLContentLayout.GROUP, list);
					}

					PsoftLinksControl links = (PsoftLinksControl)this.LoadPSOFTControl(PsoftLinksControl.Path, "_links");
					object[] objs;
					long triggerUID;
					long clipboardID;
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
							clipboardID = DBColumn.GetValid(objs[0],-1L);
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
							if (tree.ProjectID > 0)
							{
								long tasklistID = ch.psoft.Util.Validate.GetValid(db.lookup("TASKLIST_ID", "PROJECT", "ID=" + _projectID, true), -1L);

								links.LinkGroup1.Caption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CMT_SELECTED_PROJECT);

								if (_context == ProjectList.CONTEXT_PERSON || _context == ProjectList.CONTEXT_SEARCHRESULT || _context == ProjectList.CONTEXT_PROJECTGROUP)
								{
									links.LinkGroup2.Caption = _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CMT_LISTED_PROJECTS);
									links.LinkGroup2.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_PROJECT_SUMMARY), "/Project/ProjectSummary.aspx?context=" + _context + "&xID=" + _xID);
								}

								if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "PROJECT", _projectID, true, true))
								{
									links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_ADDPHASE), "/Project/PhaseAdd.aspx?projectID=" + tree.ProjectID);
									links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_ADDPROJECT), psoft.Project.ProjectAdd.GetURL("parentID",tree.ProjectID));
									links.LinkGroup1.AddLink(_mapper.get("new"),_mapper.get("tasklist", "newmeasure"),psoft.Tasklist.AddMeasure.GetURL("assignPerson", "enable","tasklistID", tasklistID,"backURL", Request.RawUrl,"template", "false","HideTasklist","true"));

                                    if (Global.Config.getModuleParam("project", "enableProjectCostControl", "0").Equals("1"))
                                    {
                                        links.LinkGroup1.AddLink(_mapper.get("new"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_NEW_BILLING), psoft.Project.BillingAdd.GetURL("projectID", tree.ProjectID));
                                    }
								}

								links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_SCORECARD), psoft.Project.ProjectScoreCard.GetURL("projectID",tree.ProjectID));
								if (db.hasRowAuthorisation(DBData.AUTHORISATION.UPDATE, "PROJECT", _projectID, true, true))
								{
									links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_SPEC), psoft.Project.ProjectEdit.GetURL("projectID",tree.ProjectID,"context",psoft.Project.ProjectEdit.CONTEXT_SPEC_EDIT));					
									links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_MANAGETEAM), psoft.Project.ManageTeam.GetURL("projectID",tree.ProjectID));
									links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_EDITPROJECT), psoft.Project.ProjectEdit.GetURL("projectID",tree.ProjectID,"context",psoft.Project.ProjectEdit.CONTEXT_PROJECT_EDIT));
								}
								else
								{
									links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_SHOWTEAM), psoft.Project.ManageTeam.GetURL("projectID",tree.ProjectID));
								}								
								if (db.hasRowAuthorisation(DBData.AUTHORISATION.DELETE, "PROJECT", _projectID, true, true))
								{
									links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_DELETEPROJECT), "javascript: deleteProjectConfirm(" + tree.ProjectID + ")");
								}
								links.LinkGroup1.AddLink(_mapper.get("actions"),_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_PHASEDEPENDENCIES),psoft.Project.ProjectDetail.GetURL("context","dependency","ID",tree.ProjectID));

                                if (Global.Config.getModuleParam("project", "enableProjectCostControl", "0").Equals("1"))
                                {
                                    //context is "billing"
                                    links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_COSTSCONTROL), psoft.Project.ProjectDetail.GetURL("context", "billing", "ID", tree.ProjectID));
                                }
								//     clipboard
								objs = db.lookup(new string[] {"CLIPBOARD_ID","UID"}, "PROJECT", "ID=" + tree.ProjectID);
								clipboardID = DBColumn.GetValid(objs[0],-1L);
								triggerUID = DBColumn.GetValid(objs[1],-1L);
								if (clipboardID > 0) 
								{
									if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, "CLIPBOARD", clipboardID, true, true))
									{
										//     open clipboard
										links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("clipboard"), psoft.Document.Clipboard.GetURL("ID",clipboardID, "ownerTable","PROJECT"));
									}
								}
								else 
								{
									if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "PROJECT", _projectID, true, true))
									{
										//     create clipboard
										links.LinkGroup1.AddLink(_mapper.get("shelf"), _mapper.get("createClipboard"), psoft.Document.CreateClipboardM.GetURL("type",Clipboard.TYPE_PRIVATE, "ownerTable","PROJECT", "ownerID",tree.ProjectID, "triggerUID",triggerUID));
									}
								}

								// organigrams
//uncomment to enable default-organigramm								links.LinkGroup1.AddLink(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CMT_ORGANIGRAMS), _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CM_PROJECT_ORGANISATION), Psoft.Organisation.Organigram.GetURL("id",db.Project.getDefaultChartID(tree.ProjectID)));
								long[] customChartIDs = db.Project.getCustomChartIDs(tree.ProjectID);
								foreach(long chartID in customChartIDs)
								{
									links.LinkGroup1.AddLink(_mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_CMT_ORGANIGRAMS), db.lookup("TITLE", "CHART", "ID=" + chartID, false), psoft.Organisation.Organigram.GetURL("id",chartID));
								}

								//  oe
								if (db.hasRowAuthorisation(DBData.AUTHORISATION.INSERT, "PROJECT", _projectID, true, true))
								{
									links.LinkGroup1.AddLink(_mapper.get("organisation","oe"), _mapper.get("project","oeAssignProjectGroup"), "/Organisation/AssignGroups.aspx?ownerTable=PROJECT&ownerID=" + tree.ProjectID + "&backURL=" + HttpUtility.UrlEncode(Request.RawUrl) + "&nextURL=" + HttpUtility.UrlEncode(Request.RawUrl));
								}

								//  subscription
								links.LinkGroup1.AddAboLinks("PROJECT", _projectID, _mapper);
							}
							break;
                        //
                        // mbo
                        /*
                        if (Global.isModuleEnabled("mbo")) {
                            // project objectives (Ziele)
                            links.LinkGroup1.AddLink(
                                _mapper.get("mbo","objectives"),
                                _mapper.get("mbo","projectObjectives"),
                                "/MbO/Detail.aspx?view=detail&context=PROJECT&contextId=" + tree.ProjectID
                                );
                        }
                        */
                        
                    }
					SetPageLayoutContentControl(DDGLContentLayout.LINKS, links);

                    // Setting content layout user controls
                    SetPageLayoutContentControl(DDGLContentLayout.DETAIL_LEFT, tree);
					switch(_context)
					{
						case ProjectTreeCtrl.CONTEXT_TASKLIST_MEASURE:
							SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detailTL);
							BreadcrumbCaption = _mapper.get("tasklist","measuredetail");
							PsoftPageLayout.PageTitle = db.lookup("TITLE", "PROJECT", "ID=" + tree.ProjectID, false) + " - " + BreadcrumbCaption;
							PsoftPageLayout.ShowButtonAuthorisation("MEASURE", _xID);
							break;
						default:
							SetPageLayoutContentControl(DDGLContentLayout.DETAIL_RIGHT, detail);
							BreadcrumbCaption = PsoftPageLayout.PageTitle = db.lookup("TITLE", "PROJECT", "ID=" + tree.ProjectID, false);
							PsoftPageLayout.ShowButtonAuthorisation("PROJECT", _projectID);
							PsoftPageLayout.ShowButtonRegistryEntries("PROJECT", _projectID, db.Registry.getRegistryIDs(DBData.BuildSQLArray(db.Project.getParentProjectIDList(parentID, true).ToArray()), "PROJECT"));
                            if (Global.Config.getModuleParam("project", "scoreCardPrintAsPdf", "0").Equals("1"))
                            {
                                PsoftPageLayout.ButtonPrintVisible = true;
                                PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('" + psoft.Project.CreateProjectReport.GetURL(psoft.Project.CreateProjectReport.ARGNAME_URL_ID, _projectID) + "','_blank')");
                            }
                            else
                            {
                                PsoftPageLayout.ButtonExcelVisible = true;
                                PsoftPageLayout.ButtonExcelAttributes.Add("onClick", "javascript: window.open('" + psoft.Project.CreateProjectReport.GetURL(psoft.Project.CreateProjectReport.ARGNAME_URL_ID, _projectID) + "','_blank')");
                            }
							break;
					}


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

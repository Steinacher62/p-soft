using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Document.Controls;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.Web.UI;

namespace ch.appl.psoft.Document
{
    /// <summary>
    /// Summary description for Detail.
    /// </summary>
    public partial class Detail : PsoftTreeViewPage {

        private const string PAGE_URL = "/Document/Detail.aspx";

        static Detail(){
            SetPageParams(PAGE_URL, "table", "xID", "clipboardID", "context", "contextID", "selectedFolderID", "registryEnable", "err6", "orderColumn", "orderDir");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }
        
        protected string _context = "";
        protected long _contextID = -1L;
		private string _orderColumn = "MESSAGENAME";
		private string _orderDir = "asc";

        public Detail() : base()
        {
           PageURL = PAGE_URL;
        }

        protected virtual void Page_Load(object sender, System.EventArgs e) 
		{
            DBData db = DBData.getDBData(Session);
            db.connect();
 
            long groupAccessorId = DBColumn.GetValid(db.lookup("ID", "ACCESSOR", "TITLE = 'HR'"),(long)-1);

            if ((Global.isModuleEnabled("spz") || Global.isModuleEnabled("frauenfeld")) & (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)))
            {
                string confirmCopyScript = "<script type=\"text/javascript\">"
                                    + "//show confirmation to copy expectation to other jobs\r\n"
                                    + "function confirmCopy(confirmText)\r\n"
                                    + "{\r\n"
                                    + "var test = (confirm(document.getElementById(confirmText).value));\r\n"
                                    + "__doPostBack(\"__Page\",test);\r\n"
                                    + "}\r\n"
                                    + "</script>\r\n";
    
                ScriptManager.RegisterStartupScript(Page,Page.GetType(), "confirmCopy", confirmCopyScript,false);
                //lists persons with same functions
                string confirmText = _mapper.get("expectation", "ConfirmCopy");
                lblConfirmText.Value = confirmText + "\n\n";
                string jobTitle = db.lookup("JOB.TITLE_DE", "PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "(JOB.HAUPTFUNKTION = 1) AND (PERSON.CLIPBOARD_ID = " + Request["ClipboardID"] + ")", " ");

                DataTable jobTable = db.getDataTableExt("SELECT PERSON.CLIPBOARD_ID FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE(JOB.HAUPTFUNKTION = 1) AND (JOB.TITLE_DE = '" + jobTitle + "') AND (person.CLIPBOARD_ID IS NOT NULL)", new object[0]); ;
                goCopy.Value = "Testwert";

                //get persons with this jobs
                foreach (DataRow job in jobTable.Rows)
                {
                    //get person name
                    string name = ch.psoft.Util.Validate.GetValid(db.lookup("FIRSTNAME + ' ' + PNAME AS NAME", "PERSON", "CLIPBOARD_ID = " + job[0]).ToString(), "");
                    lblConfirmText.Value += name + "\n";
                }

            }

            


            string PostBackStr = Page.ClientScript.GetPostBackEventReference(this, "test");

            if (Request["__EVENTARGUMENT"] == "true")
            {
                String clipBoardIdFrom = Request["ClipboardID"];
                long personIdFrom = db.lookup("id", "person", "clipboard_id=" + clipBoardIdFrom, 0L);
                string jobTitle = db.lookup("JOB.TITLE_DE", "PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID", "(JOB.HAUPTFUNKTION = 1) AND (PERSON.ID = " + personIdFrom + ")", " ");
                DataTable tblClipBoard = db.getDataTableExt("SELECT PERSON.CLIPBOARD_ID FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE(JOB.HAUPTFUNKTION = 1) AND (JOB.TITLE_DE = '" + jobTitle + "') AND PERSON.CLIPBOARD_ID IS NOT NULL", new object[0]);
                DataTable tbldoc = db.getDataTableExt("SELECT * FROM DOCUMENT WHERE ID= '" + GetQueryValue("xID", 0L) + "'", new object[0]);
                DataRow rowdoc = tbldoc.Rows[0];
               
                foreach (DataRow aktclipBoard in tblClipBoard.Rows)
                {
                    string sql;
                    long clipBoardIdTo = (long)aktclipBoard[0];
                    long folderRootId = db.lookup("folder_id", "clipboard", "id =" + aktclipBoard[0].ToString(), 0L);
                    long folder_id = db.lookup("id", "folder", "(root_id ='" + folderRootId + "') and (Title = 'Stellenbeschreibungen')", 0L);
                    long personId = db.lookup("id", "person", "clipboard_id=" + clipBoardIdTo, 0L);
                    if (personId != personIdFrom)
                    {
                        sql = "insert into document (FOLDER_ID,INHERIT,TITLE,DESCRIPTION,AUTHOR,FILENAME,XFILENAME,VERSION,CREATED,CHECKOUT_STATE,CHECKIN_PERSON_ID,TYP,NUMOFDOCVERSIONS)  VALUES(" + folder_id + ",'" + rowdoc[3] + "','" + rowdoc[4] + "','" + rowdoc[5] + "','" + rowdoc[6] + "','" + rowdoc[7] + "','" + rowdoc[8] + "','" + rowdoc[9] + "','" + Convert.ToDateTime(rowdoc[10]).ToString("yyyy-MM-dd HH:mm:ss") + "','" + rowdoc[11] + "','" + rowdoc[12] + "','" + rowdoc[16] + "','" + rowdoc[20] + "')";
                        db.execute(sql);
                    }
                    long docId = db.lookup("id", "document", "FOLDER_ID=" + folder_id + "and XFILENAME= '" + rowdoc[8] +"'", 0L);
                    sql = "update document set inherit = 0 where id = " + docId;
                    db.execute(sql);
                    long rowId = db.lookup("JOB.ID", "JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID", "(JOB.HAUPTFUNKTION = 1) AND (PERSON.ID = " + personId + ")", 0L);
                    DataTable accessors = db.getDataTable("select * from ACCESSORV where ID in (select distinct ACCESSOR_ID from ACCESS_RIGHT_RT where TABLENAME='JOB' and (ROW_ID= " + rowId + " or ROW_ID=0))");
                    int rows = accessors.Rows.Count - 1;
                    for (int i = 0; i <= rows; i++)
                    {
                        DataRow row1 = accessors.Rows[i];
                        int right = db.getRowAuthorisations((long)row1[0], "job", rowId, 21, true, true);
                        
                        if (row1[2].ToString() == "Administratoren")
                        {
                            db.grantRowAuthorisation((int)right, (long)row1[0], "DOCUMENT", docId);
                            db.grantRowAuthorisation((int)right, (long)row1[0], "DOCUMENT_HISTORY", docId);
                        }
                        else if (row1[2].ToString() == "Alle")
                        {
                            db.grantRowAuthorisation(0, (long)row1[0], "DOCUMENT", docId);
                            db.grantRowAuthorisation(0, (long)row1[0], "DOCUMENT_HISTORY", docId);
                        }
                        else if (row1[2].ToString() == "HR")
                        {
                            db.grantRowAuthorisation((int)right, (long)row1[0], "DOCUMENT", docId);
                            db.grantRowAuthorisation((int)right, (long)row1[0], "DOCUMENT_HISTORY", docId);
                        }
                        else
                        {
                            db.grantRowAuthorisation(2, (long)row1[0], "DOCUMENT", docId);
                            db.grantRowAuthorisation(2, (long)row1[0], "DOCUMENT_HISTORY", docId);
                        }
                    }
                }
                //db.disconnect();
                //Response.Redirect(Clipboard.GetURL("id", clipBoardIdFrom, "selectedFolderID", Request["FolderId"]), true);
            }

			try 
			{
				string tableName = GetQueryValue("table", "");
				long xID = GetQueryValue("xID", 0L);

				if (xID > 0)
				{
					// validate ID...
					xID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "FOLDERDOCUMENTV", "ID=" + xID, false), 0L);
				}
				long clipboardID = GetQueryValue("clipboardID", -1L);
				if (tableName == "FOLDER")
				{
					// let's behave like IE...
					if (clipboardID <= 0L)
					{
						clipboardID = ch.psoft.Util.Validate.GetValid(db.lookup("ID", "CLIPBOARD", "FOLDER_ID=(select ROOT_ID from FOLDER where ID=" + xID + ")", false), -1L);
					}
					Response.Redirect(Clipboard.GetURL("id",clipboardID, "selectedFolderID",xID), true);
				}

				_context = GetQueryValue("context", _context);
				_contextID = GetQueryValue("contextID", _contextID);
                

				BreadcrumbName += _context;

				// Setting main page layout
				PsoftPageLayout PsoftPageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");;
				PageLayoutControl = PsoftPageLayout;

				// Setting content layout of page layout
				DGLContentLayout contentLayout = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
				PageLayoutControl.ContentLayoutControl = contentLayout;

				// Setting detail parameters
                DokAblageDetail detail = (DokAblageDetail)this.LoadPSOFTControl(DokAblageDetail.Path, "_dokAbl");
				detail.XID = xID;
				detail.ClipboardID = clipboardID;
				detail.FolderID = GetQueryValue("selectedFolderID", 0L);
				detail.TableName = ch.psoft.Util.Validate.GetValid(tableName, detail.TableName);
				detail.RegistryEnable = bool.Parse(GetQueryValue("registryEnable","true"));
				detail.ShowErr6Warning = bool.Parse(GetQueryValue("err6", "false"));
				detail.ReloadURL = psoft.Document.Detail.GetURL("table",detail.TableName, "context",_context, "contextID",_contextID, "xID",detail.XID, "clipboardID",detail.ClipboardID, "selectedFolderID",detail.FolderID, "registryEnable",detail.RegistryEnable);
				detail.ActiveXErrorTooltip = PSOFTConvert.ToJavascript(_mapper.get("document","ActiveXErrorTooltip"));
				// Setting links
				PsoftLinksControl links = (PsoftLinksControl) LoadPSOFTControl(PsoftLinksControl.Path, "_links");
				links.LinkGroup1.Caption = "";

				switch (detail.TableName)
				{
					case "DOCUMENT":

                        if (_context == "checkOutUndo") {
                            db.Document.checkOutUndo(xID);
                        }

                        if (_context == "delete")
                        {
                            string FolderID = db.lookup("FOLDER_ID","FOLDERDOCUMENTV","ID=" + GetQueryValue("xID",0L).ToString()).ToString();
                            db.execute("DELETE FROM DOCUMENT WHERE ID = " + GetQueryValue("xID",0L).ToString());
                            Response.Redirect(Global.Config.baseURL + "//Document//Clipboard.aspx?ID=" + GetQueryValue("clipboardID", 0L).ToString() + "&selectedFolderID=" + FolderID);
                        }

						if (_context == "searchresult")
						{
							DocumentList list = (DocumentList) LoadPSOFTControl(DocumentList.Path, "_list");
							list.Kontext = DocumentList.CONTEXT_SEARCHRESULT;
							list.xID = _contextID;
							list.DetailURL = psoft.Document.Detail.GetURL("xID","%ID", "context","searchresult", "contextID",_contextID, "table",detail.TableName, "registryEnable",detail.RegistryEnable);
							list.DetailEnabled = true;
							list.DeleteEnabled = true;
							list.EditEnabled = true;
							list.EditURL = psoft.Document.Edit.GetURL("registryEnable",detail.RegistryEnable, "xID","%ID", "ClipboardID",detail.ClipboardID, "Table",detail.TableName, "selectedFolderID",detail.FolderID, "backURL",Request.RawUrl);
							list.OrderColumn = GetQueryValue("orderColumn", "TITLE");
							list.OrderDir = GetQueryValue("orderDir", "asc");
							list.SortURL = detail.ReloadURL;
							list.PostDeleteURL = psoft.Document.Detail.GetURL("xID",detail.XID, "context",_context, "contextID",_contextID, "table",detail.TableName, "registryEnable",detail.RegistryEnable);
							SetPageLayoutContentControl(DGLContentLayout.GROUP, list);

							if (detail.XID <= 0)
							{
								detail.XID = ch.psoft.Util.Validate.GetValid(db.lookup("ROW_ID", "SEARCHRESULT inner join DOCUMENT on SEARCHRESULT.ROW_ID=DOCUMENT.ID", "SEARCHRESULT.ID=" + _contextID, false), -1L);
							}

							list.DocumentID = detail.XID;
						}

						Interface.DBObjects.Document.DocType typ = (Interface.DBObjects.Document.DocType) DBColumn.GetValid(db.lookup("TYP","DOCUMENT","ID="+detail.XID),0);
						if (typ == Interface.DBObjects.Document.DocType.Document) 
						{
                            if (_context != "searchresult" && db.exists("DOCUMENT_HISTORY", "DOCUMENT_ID=" + detail.XID))
							{
								History history = (History) LoadPSOFTControl(History.Path, "_hist");
								history.DocumentID = detail.XID;
								history.OrderColumn = GetQueryValue("orderColumn", "VERSION");
								history.OrderDir = GetQueryValue("orderDir", "desc");
								history.SortURL = detail.ReloadURL;
								SetPageLayoutContentControl(DGLContentLayout.GROUP, history);
							}
							else
							{
								//temo: macht kein sinn da es keine Versionsgeschichte gibt, ausserdem fehlt der clipboardid parameter
                                //links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document","history"), psoft.Document.Detail.GetURL("table",detail.TableName, "xID",detail.XID, "registryEnable",detail.RegistryEnable));
							}

							if (db.Document.canCheckOut(detail.XID)) 
							{
								//temo: kein acitveX mehr. Lösung im getDocument anstatt per Javascript
                                //links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document","checkOut"), "javascript: checkOutClicked();", "id=\"checkOutLink\"");
                                links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document", "checkOut"), psoft.Document.GetDocument.GetURL("documentID", xID, "checkout", "true"), "id=\"checkOutLink\" onClick=\"delayedRefresher(2000, '" + psoft.Document.Detail.GetURL("table", detail.TableName, "xID", detail.XID, "clipboardID", clipboardID, "registryEnable", detail.RegistryEnable, "selectedFolder", detail.FolderID)+"')\"");
								
                                //temo: diese möglichkeit besteht nicht mehr das activeX rausgenommen wurde
                                //links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document","checkOutAndOpen"), "javascript: checkOutAndOpenClicked();", "title=\"" +  _mapper.get("document","checkOutAndOpenToolTip") + "\" id=\"checkOutAndOpenLink\"");
                               
                                // Gruppe HR bei SPZ Link für Kopieren der Stellenbewertungen 
                                if ((Global.isModuleEnabled("frauenfeld") || Global.isModuleEnabled("spz")) & (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true)))
                                {
                                    links.LinkGroup1.AddLink(_mapper.get("actions"), _mapper.get("document", "copyJobdescription"), "javascript: confirmCopy('" + lblConfirmText.ClientID + "');" + "\" id=\"confirmCopy\" \"confirmCopy\"");                             
                                }
                            }
							if (db.Document.hasValidFile(detail.XID)){
                                //temo: kein acitveX mehr. Lösung ohne Javascript
                                //links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document","get"), "javascript: getClicked();", "id=\"getDocumentLink\"");
								links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document","get"), psoft.Document.GetDocument.GetURL("documentID",xID));
							}
                            if (db.Document.canCheckIn(detail.XID)) 
							{
                                links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document", "checkIn"), psoft.Document.CheckIn.GetURL("table", detail.TableName, "xID", detail.XID, "clipboardID", clipboardID, "registryEnable", detail.RegistryEnable, "selectedFolder", detail.FolderID));
								//links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document","checkOutUndo"), "javascript: checkOutUndoClicked();", "id=\"checkOutUndoLink\"");
                                links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document", "checkOutUndo"), psoft.Document.Detail.GetURL("table", detail.TableName, "xID", detail.XID, "clipboardID", clipboardID, "registryEnable", detail.RegistryEnable, "selectedFolder", detail.FolderID, "context", "checkOutUndo"));
                            }

                            if ((Global.isModuleEnabled("spz") || Global.isModuleEnabled("frauenfeld")) && (db.Person.isLeaderOfPerson(db.lookup("id", "person", "clipboard_id=" + clipboardID, 0L), true) || (db.isAccessorGroupMember(db.userAccessorID, groupAccessorId, true))))
                            {
                                links.LinkGroup1.AddLink(_mapper.get("document", "document"), _mapper.get("document", "delete"), psoft.Document.Detail.GetURL("table", detail.TableName, "xID", xID, "context", "delete", "clipboardID", clipboardID));
                            }
						}

						if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, detail.TableName, detail.XID, true, true))
						{
							links.LinkGroup1.AddAboLinks("DOCUMENT", detail.XID, _mapper);
						}
						break;
					case "EXCHANGE":
						_orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], _orderColumn);
						_orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], _orderDir);

						//Refresh
						if(bool.Parse(GetQueryValue("registryEnable","false"))) 
						{
							ExchangeFolder ef = ExchangeHelper.getExchangeFolder(Session, xID);
							ArrayList exchangeMessages = ef.getMessages();
							int i = 1;
				
							db.execute("DELETE FROM EXCHANGE_MESSAGES_TMP WHERE SESSIONID='" + Session.SessionID + "'");
							db.execute("DELETE FROM EXCHANGE_ATTACHEMENTS_TMP WHERE SESSIONID='" + Session.SessionID + "'");
				
							foreach(ExchangeMessage emsg in exchangeMessages) 
							{
								db.execute("INSERT INTO EXCHANGE_MESSAGES_TMP (ID, SESSIONID, MESSAGENAME, FOLDERNAME, CREATIONDATE, HREF, EML_FROM, NROFATTACH) VALUES (" + 
									i + ",'" + Session.SessionID + "','" + emsg.HumanReadableTitle + "','" + ef.Title + 
									"', convert(datetime,'" + emsg.Creationdate.Substring(0,emsg.Creationdate.Length-1) + "',126),'" + emsg.Href + "', '" + 
									emsg.getAdditionalAttribute(ExchangeMessage.FROM) + "'," + emsg.getAdditionalAttribute(ExchangeMessage.NROFATTACHEMENTS) + ")");

								if(emsg.getAdditionalAttribute(ExchangeMessage.HASATTACHEMENTS) == "1") 
								{
									ArrayList exchangeAttachements = emsg.getAttachements();
									int k = 1;
									foreach(ExchangeAttachement eatta in exchangeAttachements) 
									{
										db.execute("INSERT INTO EXCHANGE_ATTACHEMENTS_TMP (ID, SESSIONID, FILENAME, FILEEXTENSION, MIMETYPE, FILESIZE, HREF, MESSAGE_ID) VALUES (" + 
											k++ + ",'" + Session.SessionID + "','" + eatta.Filename + "','" + eatta.Fileextension + 
											"','" + eatta.Mimetype + "','" + eatta.Filesize + "','" + eatta.Href + "'," + i + ")");
									}//foreach
								}//if

								i++;

							}//foreach
						}//if

						MailingListList mll = (MailingListList) LoadPSOFTControl(MailingListList.Path,"_mll");
						MailingListDetailCtrl mdl = (MailingListDetailCtrl) LoadPSOFTControl(MailingListDetailCtrl.Path,"_mld");

						mll._exchangeFolderID = xID;
						mdl._exchangeFolderID = xID;
						

						if(_contextID < 0) 
						{
							mll._messageID = 1;
							mdl._messageID = 1;
						} 
						else 
						{
							mll._messageID = _contextID;
							mdl._messageID = _contextID;
						}

						mll.OrderColumn = _orderColumn;
						mll.OrderDir = _orderDir;
						
						//Set URLS
						//mll.SortURL = GetURL("table","EXCHANGE","xID",xID,"contextID","%ID"); //	Global.Config.baseURL + "/Document/MailingListDetail.aspx?xID=" + detail.XID + "&contextID" + _contextID + "&orderDir=" + _orderDir + "&orderColumn=" + _orderColumn;
						//mll.DetailURL = GetURL("table","EXCHANGE","xID",xID,"contextID","%ID","orderColumn",orderColumn,"orderDir",orderDir); // Global.Config.baseURL + "/Document/Detail.aspx?table=EXCHANGE&xID=" + detail.XID + "&contextID=%ID";
						mll.SortURL = Global.Config.baseURL + "/Document/Detail.aspx?table=EXCHANGE&xID=" + xID + "&contextID=%ID";
						mll.DetailURL = Global.Config.baseURL + "/Document/Detail.aspx?table=EXCHANGE&xID=" + xID + "&contextID=%ID&orderColumn=" + _orderColumn + "&orderDir=" + _orderDir;
						mll.DetailEnabled = true;
						mll.UseJavaScriptToSort = false;
            
						//Setting content layout user 
						SetPageLayoutContentControl(DGLContentLayout.GROUP, mll);	
						SetPageLayoutContentControl(DGLContentLayout.DETAIL, mdl);
						break;
				}
				PsoftPageLayout.ShowButtonAuthorisation(detail.TableName, detail.XID);
				PsoftPageLayout.PageTitle = BreadcrumbCaption = db.lookup("TITLE", "FOLDERDOCUMENTV", "ID=" + detail.XID, false);

				//Setting content layout user controls
				if(detail.TableName != "EXCHANGE") 
				{
					SetPageLayoutContentControl(DGLContentLayout.DETAIL, detail);
					SetPageLayoutContentControl(DGLContentLayout.LINKS, links);
				}
			}
			catch (ExchangeException ex) 
			{
				ShowError(_mapper.get("exchange", ex.Message));
			}
			catch(Exception ex)
			{
				ShowError(ex.Message);
			}
			finally
			{
				db.disconnect();
			}
        }


		protected override void AppendBodyOnLoad(System.Text.StringBuilder bodyOnLoad)
		{
			base.AppendBodyOnLoad (bodyOnLoad);
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
        private void InitializeComponent() {    

        }
		#endregion
    }
}

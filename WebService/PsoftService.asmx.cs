using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using DSOFile;
using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.Services;

namespace ch.appl.psoft.WebService
{
    /// <summary>
    /// Summary description for PsoftService.
    /// </summary>

    [System.Web.Script.Services.ScriptService]
    public class PsoftService : System.Web.Services.WebService {
        public PsoftService() {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

		#region Component Designer generated code
		
        //Required by the Web Services Designer 
        private IContainer components = null;
				
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing ) {
            if(disposing && components != null) {
                components.Dispose();
            }
            base.Dispose(disposing);		
        }
		
		#endregion

        /// <summary>
        /// Get File Properties
        /// </summary>
        /// <param name="file">file name (ohne Pfad)</param>
        /// <returns>
        /// 0: title
        /// 1: comment
        /// 2: author
        /// 3: create date
        /// 4: version
        /// </returns>
        [WebMethod(EnableSession=true)]
        public string[] fileProperties(string file) {
            string[] properties = {"","","","",""};
            Config config = Global.Config;

            if (config.documentTempDirectory != "")
                file = config.documentTempDirectory+"\\"+file;

            OleDocumentPropertiesClass docProps = null;
            bool isOpen = false;

            try {
                docProps = new OleDocumentPropertiesClass();
                docProps.Open(file, true, dsoFileOpenOptions.dsoOptionDefault | dsoFileOpenOptions.dsoOptionOpenReadOnlyIfNoWriteAccess | dsoFileOpenOptions.dsoOptionDontAutoCreate);
                isOpen = true;
                properties[0] = docProps.SummaryProperties.Title;
                properties[1] = docProps.SummaryProperties.Comments;
                properties[2] = docProps.SummaryProperties.Author;
                properties[3] = docProps.SummaryProperties.DateCreated == null? "":((DateTime)docProps.SummaryProperties.DateCreated).ToString(SessionData.getDBColumn(Session).UserCulture.DateTimeFormat.ShortDatePattern);
                properties[4] = docProps.SummaryProperties.Version;
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                //the COM-object must be released, otherwise it keeps the file still open and it can't be accessed afterwards.
                if (docProps != null){
                    try{
                        if (isOpen){
                            docProps.Close(false);
                        }
                    }
                    catch (Exception e) {
                        Logger.Log(e,Logger.ERROR);
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(docProps);
                }
            }
            return properties;
        }
        /// <summary>
        /// Loescht row
        /// </summary>
        /// <param name="tableName">Tabellennamen</param>
        /// <param name="id">row id</param>
        /// <param name="rootEnable">Bei tableName == NODE: true: Wurzel kann gelöscht werden</param>
        /// <returns>>= 0: #geloeschte rows, -1: error</returns>
        [WebMethod(EnableSession=true)]
        public int deleteTableRow(string tableName,long id, bool rootEnable) {
            DBData db = DBData.getDBData(Session);
            string sql = "delete from "+tableName+" where id = "+id;
            int numDel = 1;
            
            try {
                db.connect();
                db.beginTransaction();
                switch (tableName) {
                case "DOCUMENT":
                    numDel = db.Document.delete(id, true);
                    break;

                case "FOLDER":
                    numDel = db.Folder.delete(id, true);
                    break;

                case "FOLDERDOCUMENTV":
                    numDel = db.Document.delete(id, true);
                    numDel += db.Folder.delete(id, true);
                    break;

                case "CLIPBOARD":
                    numDel = db.Clipboard.delete(id, true);
                    break;

                case "DOCUMENT_HISTORY":
                    numDel = db.DocumentHistory.delete(id, true);
                    break;

                case "PERSON":
                    //delete all person in case of company
                    numDel = db.Person.delete(id, true);
                    break;

                case "CONTACTV":
                    numDel = db.Person.delete(id, true);
                    numDel += db.execute("delete from FIRM where ID=" + id);
                    break;

                case "JOURNAL_CONTACT_V":
                    long journalID = ch.psoft.Util.Validate.GetValid(db.lookup("JOURNAL_ID" , "JOURNAL_CONTACT_V", "ID=" + id, false), -1);
                    numDel = db.execute("delete from JOURNAL_FIRM where ID=" + id);
                    numDel += db.execute("delete from JOURNAL_PERSON where ID=" + id);
                    long nrOfEntries = ch.psoft.Util.Validate.GetValid(db.lookup("count (ID)" , "JOURNAL_CONTACT_V", "JOURNAL_ID=" + journalID, false), -1);
                    if (nrOfEntries == 0)
                        numDel += db.execute("delete from JOURNAL where ID=" + journalID);
                    break;

                case "TASKLIST":
                    numDel = db.Tasklist.delete(id,true);
                    break;
                    
                case "MEASURE":
                    numDel = db.Measure.delete(id,true);
                    break;
                    
                case "PROJECT":
                    numDel = db.Project.delete(id, true);
                    break;
                case "PROJECT_BILLING":
                    numDel = db.ProjectBilling.delete(id, true);
                    break;

                case "PHASE":
					numDel = db.Phase.delete(id, true);
                    break;

                case "NEWS":
                    numDel = db.News.delete(id, true);
                    break;

                case "SUBSCRIPTION":
                    numDel = db.Subscription.delete(id, true);
                    break;

                case "OBJECTIVE":
                    numDel = db.Objective.delete(id,true);
                    break;

                case "KNOWLEDGE":
                    //if present associated suggestion execution must be also deleted
                    if(Global.Config.isModuleEnabled("suggestion")) 
                    {
                        long suggestionExecutionId = ch.psoft.Util.Validate.GetValid(db.lookup("SUGGESTION_EXECUTION_ID" , "KNOWLEDGE", "ID=" + id, false), -1L);
                        if(suggestionExecutionId > -1) 
                        {
                            numDel  = db.execute("delete from SUGGESTION_EXECUTION_STATI where SUGGESTION_EXECUTION_ID=" + suggestionExecutionId);
                            numDel += db.Knowledge.delete(id,true);
                            numDel += db.execute("delete from SUGGESTION_EXECUTION where ID=" + suggestionExecutionId);
                        } 
                        else 
                        {
                            numDel = db.Knowledge.delete(id,true);
                        }
                    }
                    else 
                    {
                        numDel = db.Knowledge.delete(id,true);
                    }
                    break;

                case "THEME":
                    numDel = db.Theme.delete(id,true);
                    break;

                case "UID_ASSIGNMENT_V":
                    numDel = db.execute("delete from UID_ASSIGNMENT where ID=" + id);
                    break;

                case "JOB":
                    numDel = db.Job.delete(id, true);
                    break;

                case "SUGGESTION_EXECUTION":
                    long knowledgeId = ch.psoft.Util.Validate.GetValid(db.lookup("ID" , "KNOWLEDGE", "SUGGESTION_EXECUTION_ID=" + id, false), -1); 
                    string theme_id_b = db.langAttrName("KNOWLEDGE", "BASE_THEME_ID");
                    long themeId = ch.psoft.Util.Validate.GetValid(db.lookup(theme_id_b , "KNOWLEDGE", "ID=" + knowledgeId, false), -1); 
                    //delete stati
                    numDel += db.execute("delete from SUGGESTION_EXECUTION_STATI where SUGGESTION_EXECUTION_ID=" + id);
                    //delete associated WE if any...
                    numDel += db.execute("delete from KNOWLEDGE where ID=" + knowledgeId);
                    numDel += db.execute("delete from THEME where ROOT_ID=" + themeId);
                    // delete SUGGESTION_EXECUTION
                    numDel += db.execute(sql);
                    break;
                default:
                    numDel = db.execute(sql);
                    break;
                }
                db.commit();
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                numDel = -1;
                db.rollback();
            }
            finally {
                db.disconnect();
            }
            return numDel;
        }
        /// <summary>
        /// Move Element an neuen Owner
        /// </summary>
        /// <param name="ownerColumn">Name of foreign key column referencing the owner</param>
        /// <param name="newOwnerID">Neue owner ID</param>
        /// <param name="table">Name of table</param>
        /// <param name="id">ID of record to move</param>
        /// <returns>0: fail, else ok</returns>
        [WebMethod(EnableSession=true)]
        public int move(string ownerColumn, long newOwnerID, string table, int ID) {
            DBData db = DBData.getDBData(Session);
            int state = 0;
            try {
                db.connect();
                ParameterCtx rows = new ParameterCtx("ROWS",null,ParameterDirection.Output,typeof(int));
                db.executeProcedure("MODIFYTABLEROW",
                    rows,
                    new ParameterCtx("USERID",db.userId),
                    new ParameterCtx("TABLENAME",table),
                    new ParameterCtx("ROWID",ID),
                    new ParameterCtx("MODIFY","update " + table + " set " + ownerColumn + "=" + newOwnerID + " where ID=" + ID,ParameterDirection.Input,typeof(string),true),
                    new ParameterCtx("INHERIT",1),
                    new ParameterCtx("ACTION",4)
                    );
                state = db.parameterValue(rows,0);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }
            return state;
        }
        /// <summary>
        /// Copy Element
        /// </summary>
        /// <param name="ownerTable">Owner tabelle</param>
        /// <param name="newOwnerID">!=0: Owner ID</param>
        /// <param name="table">Source tabelle</param>
        /// <param name="id">Source ID</param>
        /// <returns>ID der Kopie oder 0</returns>
        [WebMethod(EnableSession=true)]
        public long copy(string ownerTable, long newOwnerID, string table, long ID, bool cascade) {
            DBData db = DBData.getDBData(Session);
            long newId = 0;
            
            try {
                db.connect();
                db.beginTransaction();
                switch (table) {
                case "FOLDER":
                    switch (ownerTable) {
                    case "FOLDER":
                        if (db.Folder.find(ID, newOwnerID) == 0) {
                            newId = db.Folder.copy(ID, newOwnerID, -1L, cascade, true, true);
                        }
                        break;
                    default:
                        break;
                    }
                    break;

                case "DOCUMENT":
                    switch (ownerTable) {
                    case "FOLDER":
                        newId = db.Document.copy(ID, newOwnerID, -1L, true);
                        break;
                    default:
                        break;
                    }
                    break;

                case "TASKLIST":
                    switch (ownerTable) {
                    case "TASKLIST":
                        int typ = ch.psoft.Util.Validate.GetValid(db.lookup("TYP", "TASKLIST", "ID="+ID, false), Interface.DBObjects.Tasklist.TYPE_PUBLIC);
                        newId = db.Tasklist.copy(ID, -1, newOwnerID, -1, true, true, false, typ, false);
                        break;
                    default:
                        break;
                    }
                    break;

                case "MEASURE":
                    switch (ownerTable) {
                    case "TASKLIST":
                        newId = db.Measure.copy(ID, newOwnerID, -1, true, false, false);
                        break;
                    default:
                        break;
                    }
                    break;

                case "PROJECT":
                    switch (ownerTable) {
                    case "PROJECT":
                        newId = db.Project.copy(ID, -1, newOwnerID, true, true, false, false, true, true);
                        break;
                    default:
                        break;
                    }
                    break;

                case "PHASE":
                    switch (ownerTable) {
                    case "PROJECT":
                        newId = db.Phase.copy(ID, newOwnerID, true, false, false, true);
                        break;
                    default:
                        break;
                    }
                    break;

                default:
                    break;
                }
                db.commit();   
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
                db.rollback();
                newId = 0;
            }
            finally {
                db.disconnect();
            }
            return newId;
        }
        /// <summary>
        /// Check Out Document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>boolean: true for success, otherwise false</returns>
        [WebMethod(EnableSession=true)]
        public bool checkOutDocument(long documentID) {
            bool retValue = false;
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                retValue = db.Document.checkOut(documentID);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

            return retValue;
        }

        /// <summary>
        /// Check In Document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>boolean: true for success, otherwise false</returns>
        [WebMethod(EnableSession=true)]
        public bool checkInDocument(long documentID) {
            bool retValue = false;
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                retValue = db.Document.checkIn(documentID);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

            return retValue;
        }

        /// <summary>
        /// Undo Check Out Document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>boolean: true for success, otherwise false</returns>
        [WebMethod(EnableSession=true)]
        public bool checkOutUndoDocument(long documentID) {
            bool retValue = false;
            DBData db = DBData.getDBData(Session);
            try {
                db.connect();
                retValue = db.Document.checkOutUndo(documentID);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }

            return retValue;
        }
        [WebMethod(EnableSession=true)]
        public string extendTree(string envName,long parentId) {
            Common.Tree tree = (Common.Tree) Session[envName];
            StringBuilder build = new StringBuilder(512);

            tree.extendTree(DBData.getDBData(Session),parentId,"node",build);
            return build.ToString();
        }

    }
}

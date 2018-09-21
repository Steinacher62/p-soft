using ch.appl.psoft.db;
using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Provides methods for recurring tasks on dispatch-documents.
    /// </summary>
    public class DispatchDocument : DBObject
	{
        public static string TABLE_NAME = "DISPATCHDOCUMENT";
        private static string DOCUMENTS_LINK = "Documents/";

        public enum DOCUMENT_TYPE
        {
            MAILING_TEMPLATE = 0,
            MAIL_ATTACHMENT,
            MAILING_MERGED
        }

        public DispatchDocument(DBData db, HttpSessionState session) : base(db, session)
        {
        }

        public override int delete(long ID, bool cascade)
        {
            string serverFileName = _db.lookup("SERVER_FILENAME", TABLE_NAME, "ID=" + ID, false);
            if (serverFileName != "")
            {
                try
                {
                    File.Delete(DocumentsPath + serverFileName);
                }
                catch(Exception)
                {
                }
            }
            return _db.execute("delete from " + TABLE_NAME + " where ID=" + ID);
        }

        public int deleteAllTemporary()
        {
            int retValue = 0;

            DataTable table = _db.getDataTable("select ID from " + TABLE_NAME + " where TEMPORARY=1");
            foreach (DataRow row in table.Rows)
            {
                retValue += delete(ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1), true);
            }

            return retValue;
        }

        public long addFile(HttpPostedFile postedFile, DOCUMENT_TYPE documentType)
        {
            return addFile(postedFile, documentType, true);
        }

        public long addFile(HttpPostedFile postedFile, DOCUMENT_TYPE documentType, bool temporary)
        {
            long documentID = -1;
            if (postedFile != null && postedFile.FileName != "")
            {
                documentID = _db.newId(TABLE_NAME);
                string fileName = Path.GetFileName(postedFile.FileName);
                string serverFileName = documentID.ToString() + "_" + documentType + "_" + fileName;
                postedFile.SaveAs(DocumentsPath + serverFileName);
                _db.execute("insert into " + TABLE_NAME + " (ID, TYPE, FILENAME, SERVER_FILENAME, TEMPORARY) values (" + documentID + "," + (int)documentType + ",'" + fileName + "','" + serverFileName + "'," + (temporary? "1":"0") + ")");
            }
            
            return documentID;
        }

        public long addFile(string newFileName, DOCUMENT_TYPE documentType, bool temporary)
        {
            long documentID = -1;
            if (newFileName != "")
            {
                documentID = _db.newId(TABLE_NAME);
                string fileName = Path.GetFileName(newFileName);
                string serverFileName = documentID.ToString() + "_" + documentType + "_" + fileName;
                File.Move(newFileName, DocumentsPath + serverFileName);
                _db.execute("insert into " + TABLE_NAME + " (ID, TYPE, FILENAME, SERVER_FILENAME, TEMPORARY) values (" + documentID + "," + (int)documentType + ",'" + fileName + "','" + serverFileName + "'," + (temporary? "1":"0") + ")");
            }
            
            return documentID;
        }

        public string DocumentsPath {
            get {
                return AppDomain.CurrentDomain.BaseDirectory.ToString() + "Dispatch/" + DOCUMENTS_LINK;
            }
        }

        public string DocumentsURL {
            get {
                return Global.Config.baseURL + "/Dispatch/" + DOCUMENTS_LINK;
            }
        }

        public string getServerFileName(long ID)
        {
            return _db.lookup("SERVER_FILENAME", TABLE_NAME, "ID=" + ID, false);
        }

        public string getFileName(long ID)
        {
            return _db.lookup("FILENAME", TABLE_NAME, "ID=" + ID, false);
        }
    }
}

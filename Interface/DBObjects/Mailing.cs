using ch.appl.psoft.db;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.SessionState;


namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Provides methods for recurring tasks on mailings.
    /// </summary>
    public class Mailing : DBObject
	{
        public string TABLE_NAME = "MAILING";

//        public enum MAILING_PERSON_TYPE
//        {
//            EMAIL = 0,
//            LETTER
//        }
//
        public Mailing(DBData db, HttpSessionState session) : base(db, session)
        {
        }

        public override int delete(long ID, bool cascade)
        {
            return _db.execute("delete from " + TABLE_NAME + " where ID=" + ID);
        }

        public int deleteAllTemporary()
        {
            return _db.execute("delete from " + TABLE_NAME + " where TEMPORARY=1");
        }

        /// <summary>
        /// Creates a new mailing record
        /// </summary>
        /// <param name="target">target-group of mailing</param>
        /// <param name="type">type of mailing</param>
        /// <param name="temporary">just a temporary mailing?</param>
        /// <returns>ID of new mailing</returns>
        public long create(long reportLayoutID, bool temporary, string title)
        {
            long ID = _db.newId(TABLE_NAME);

            _db.execute("insert into " + TABLE_NAME + " (ID, REPORTLAYOUT_ID, TEMPORARY, TITLE) values (" + ID + "," + reportLayoutID + "," + (temporary? "1":"0") + ",'" + DBColumn.toSql(title) + "')");
            
            return ID;
        }

        /// <summary>
        /// Sets the title of the mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="title">Title</param>
        public void setTitle(long ID, string title)
        {
            _db.execute("update " + TABLE_NAME + " set TITLE='" + DBColumn.toSql(title) + "' where ID=" + ID);
        }

        /// <summary>
        /// Adds the email template to the mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="postedFile">Posted file (HTTP Post)</param>
        /// <param name="temporary">Just temporary?</param>
        /// <returns>ID of new document</returns>
        public long addEmailTemplate(long ID, HttpPostedFile postedFile, bool temporary)
        {
            long fileID = _db.DispatchDocument.addFile(postedFile, DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE, temporary);
            _db.execute("update " + TABLE_NAME + " set EMAIL_TEMPLATE_DISPATCHDOCUMENT_ID=" + fileID + " where ID=" + ID);
            return fileID;
        }

        /// <summary>
        /// Adds an existing email template to the mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="documentID">ID of document to add</param>
        public void addEmailTemplate(long ID, int documentID)
        {
            _db.execute("update " + TABLE_NAME + " set EMAIL_TEMPLATE_DISPATCHDOCUMENT_ID=" + documentID + " where ID=" + ID);
        }

        /// <summary>
        /// Adds the letter template to the mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="postedFile">Posted file (HTTP Post)</param>
        /// <param name="temporary">Just temporary?</param>
        /// <returns>ID of new document</returns>
        public long addLetterTemplate(long ID, HttpPostedFile postedFile, bool temporary)
        {
            long fileID = _db.DispatchDocument.addFile(postedFile, DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE, temporary);
            _db.execute("update " + TABLE_NAME + " set LETTER_TEMPLATE_DISPATCHDOCUMENT_ID=" + fileID + " where ID=" + ID);
            return fileID;
        }

        /// <summary>
        /// Adds an existing letter template to the mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="documentID">ID of document to add</param>
        public void addLetterTemplate(long ID, int documentID)
        {
            _db.execute("update " + TABLE_NAME + " set LETTER_TEMPLATE_DISPATCHDOCUMENT_ID=" + documentID + " where ID=" + ID);
        }

        /// <summary>
        /// Adds the generated document to the mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="temporary">Just temporary?</param>
        /// <returns>ID of new document</returns>
        public long addGeneratedDocument(long ID, string fileName, bool temporary)
        {
            long fileID = _db.DispatchDocument.addFile(fileName, DispatchDocument.DOCUMENT_TYPE.MAILING_MERGED, temporary);
            _db.execute("update " + TABLE_NAME + " set LETTER_GENERATED_DISPATCHDOCUMENT_ID=" + fileID + " where ID=" + ID);
            return fileID;
        }

        /// <summary>
        /// Adds an attachment to the mailing's attachment-list
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="postedFile">Posted file (HTTP Post)</param>
        /// <param name="temporary">Just temporary?</param>
        /// <returns>ID of new document</returns>
        public long addAttachment(long ID, HttpPostedFile postedFile, bool temporary)
        {
            long fileID = _db.DispatchDocument.addFile(postedFile, DispatchDocument.DOCUMENT_TYPE.MAIL_ATTACHMENT, temporary);
            long attachmentID = _db.newId("ATTACHMENT");
            _db.execute("insert into ATTACHMENT (ID, MAILING_ID, DISPATCHDOCUMENT_ID) values (" + attachmentID + "," + ID + "," + fileID + ")");
            return fileID;
        }

        /// <summary>
        /// Adds an existing document as attachment to the mailing's attachment-list
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="documentID">Document ID</param>
        /// <returns>ID of new attachment</returns>
        public long addAttachment(long ID, long documentID)
        {
            long attachmentID = _db.newId("ATTACHMENT");
            _db.execute("insert into ATTACHMENT (ID, MAILING_ID, DISPATCHDOCUMENT_ID) values (" + attachmentID + "," + ID + "," + documentID + ")");
            return attachmentID;
        }

        /// <summary>
        /// Removes an attachment from the mailing's attachment-list
        /// </summary>
        /// <param name="ID">Attachment ID</param>
        /// <param name="cascade">Delete also the attachment document if no longer referenced?</param>
        /// <returns>Number of deleted records</returns>
        public long deleteAttachment(long ID, bool cascade)
        {
            int retValue = 0;
            int docID = ch.psoft.Util.Validate.GetValid(_db.lookup("DOCUMENT_ID", "ATTACHMENT", "ID=" + ID, false), -1);
            if (cascade)
            {
                int refCount = ch.psoft.Util.Validate.GetValid(_db.lookup("count(*)", "ATTACHMENT", "ATTACHMENT.DISPATCHDOCUMENT_ID=" + docID, false), -1);
                if (refCount <= 1)
                    _db.DispatchDocument.delete(docID, true);
            }
            retValue += _db.execute("delete from ATTACHMENT where ID=" + ID);
            return retValue;
        }

        /// <summary>
        /// Sets the temporary flag on a mailing
        /// </summary>
        /// <param name="ID">Mailing ID</param>
        /// <param name="temporary">Just temporary?</param>
        public void setTemporary(long ID, bool temporary)
        {
            DataTable table = _db.getDataTable("select * from " + TABLE_NAME + " where ID=" + ID);
            if (table.Rows.Count > 0)
            {
                _db.execute("update " + TABLE_NAME + " set TEMPORARY=" + (temporary? "1":"0") + " where ID=" + ID);

                ArrayList docIDs = new ArrayList();
                docIDs.Add(table.Rows[0]["EMAIL_TEMPLATE_DISPATCHDOCUMENT_ID"].ToString());
                docIDs.Add(table.Rows[0]["LETTER_TEMPLATE_DISPATCHDOCUMENT_ID"].ToString());
                docIDs.Add(table.Rows[0]["LETTER_GENERATED_DISPATCHDOCUMENT_ID"].ToString());
                string inString = "";
                bool isFirst = true;
                foreach (string s in docIDs)
                {
                    if (s != "")
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            inString += ",";
                        inString += s;
                    }
                }
                if (inString != "")
                    _db.execute("update DISPATCHDOCUMENT set TEMPORARY=" + (temporary? "1":"0") + " where ID in (" + inString + ")");

                _db.execute("update DISPATCHDOCUMENT set TEMPORARY=" + (temporary? "1":"0") + " where ID in (select DISPATCHDOCUMENT_ID from ATTACHMENT where MAILING_ID=" + ID + ")");
            }
        }
    }
}

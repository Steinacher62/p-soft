using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.IO;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{
    /// <summary>
    /// Provides methods for recurring tasks on documents.
    /// Encapsulates application-logic on documents.
    /// </summary>
    public class DocumentHistory : DBObject
	{
        public DocumentHistory(DBData db, HttpSessionState session) : base(db, session) { }

        public int getNextVersion(long documentID) {
            int retValue = 1;
            
            string sql = "select max(VERSION) from DOCUMENT_HISTORY where DOCUMENT_ID=" + documentID;
            DataTable table = _db.getDataTable(sql);
            if (table.Rows.Count > 0) {
                retValue = ch.psoft.Util.Validate.GetValid(table.Rows[0][0].ToString(), 0) + 1;
            }

            return retValue;
        }

        public int getNumOfVersions(long documentID) {
            int retValue = 0;
            
            string sql = "select count(*) from DOCUMENT_HISTORY where DOCUMENT_ID=" + documentID;
            DataTable table = _db.getDataTable(sql);
            retValue = DBColumn.GetValid(table.Rows[0][0], 0);

            return retValue;
        }

        public override int delete(long documentHistoryID, bool cascade)
        {
            string sql = "select HFILENAME from DOCUMENT_HISTORY where HFILENAME is not null and ID=" + documentHistoryID;
            DataTable table = _db.getDataTable(sql);

            if (table.Rows.Count > 0)
            {
                string hFilename = ch.psoft.Util.Validate.GetValid(table.Rows[0]["HFILENAME"].ToString(), "");
                if (hFilename != "")
                {
                    string historyFilename = Global.Config.documentHistoryDirectory + "\\" + hFilename;
                    try 
                    {
                        File.Delete(historyFilename);
                    }
                    catch (Exception e) 
                    {
                        Logger.Log(e,Logger.WARNING);
                    }
                }
            }

            return _db.execute("delete from DOCUMENT_HISTORY where ID=" + documentHistoryID);
        }

        public int delete(long documentID, int numOfVersions) {
            string sql = "select ID from DOCUMENT_HISTORY where DOCUMENT_ID = "+documentID+" ORDER BY VERSION DESC";
            DataTable table = _db.getDataTable(sql);
            int num = 0;
            int num1 = 0;

            if (table.Rows.Count > 0) {
                foreach (DataRow row in table.Rows) {
                    num++;
                    if (num > numOfVersions) num1 += delete(DBColumn.GetValid(row[0],0L), true);
                }
            }

            return num1;
        }

        /// <summary>
        /// Creates an internal history filename
        /// </summary>
        /// <param name="id">DB id</param>
        /// <param name="version">file version</param>
        /// <param name="fileName">file namen</param>
        /// <returns></returns>
        public string EncodeHFileName(long id, int version, string fileName) 
        {
            return id + "_" + version + "_" + fileName;
        }

        /// <summary>
        /// Gibt DB id aus internem filenamen
        /// </summary>
        /// <param name="historyFileName">interner filenamen</param>
        /// <returns>ID</returns>
        public long IdFromHFile(string hFileName) 
        {
            int idx = hFileName.IndexOf("_");
            return Validate.GetValid(hFileName.Substring(0, idx),0 );
        }

        /// <summary>
        /// Returns version-number from internal histroy filename
        /// </summary>
        /// <param name="historyFileName">internal history file-name</param>
        /// <returns></returns>
        public int VersionFromHFile(string hFileName)
        {
            int idx1 = hFileName.IndexOf("_");
            int idx2 = hFileName.IndexOf("_",idx1+1);
            return Validate.GetValid(hFileName.Substring(idx1+1,idx2-idx1-1),0);
        }

        /// <summary>
        /// Gibt externer filanmen aus internem filenamen
        /// </summary>
        /// <param name="historyFileName">interner filenamen</param>
        /// <returns>filenamen</returns>
        public string FileFromHFile(string hFileName) 
        {
            int idx1 = hFileName.IndexOf("_");
            int idx2 = hFileName.IndexOf("_",idx1+1);
            return hFileName.Substring(idx2+1);
        }
    }
}

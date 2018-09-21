using ch.appl.psoft.db;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace ch.appl.psoft.WebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public class SBSService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateData(long UID, long ID, string URL, string TITLE, string DESCRIPTION, long SEMINAR_ID, long PARENT_ID, string AUTHOR, string DOZENT, long FOLDER_ID, long DOCUMENT_ID, string DATUM, string RELEASE)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);

            if (String.IsNullOrEmpty(RELEASE))
            {
                RELEASE = "null";
            }
            else
            {
                RELEASE = DateTime.ParseExact(RELEASE, "dd.MM.yyyy HH:mm", new System.Globalization.CultureInfo("en-US")).ToString("MM.dd.yyyy HH:mm");
                RELEASE = "'" + RELEASE + "'";
            }
            string type = GetType(UID);

            db.connect();
            DataTable dataSeminar = new DataTable();

            if (type.Equals("SEMINAR"))
            {
                db.execute("UPDATE SBS_SEMINARS SET TITLE='" + SQLColumn.toSql(TITLE) + "',DESCRIPTION='" + SQLColumn.toSql(DESCRIPTION) + "' WHERE ID ='" + ID + "'");
                dataSeminar = db.getDataTable("SELECT * FROM SBS_SEMINARS WHERE ID = '" + ID + "'");
            }
            if (type.Equals("FOLDER"))
            {
                db.execute("UPDATE SBS_FOLDERS SET TITLE='" + SQLColumn.toSql(TITLE) + "',DESCRIPTION='" + SQLColumn.toSql(DESCRIPTION) + "',DATUM='" + SQLColumn.toSql(DATUM) + "',RELEASE=" + RELEASE + " WHERE ID ='" + ID + "'");
                dataSeminar = db.getDataTable("SELECT * FROM SBS_FOLDERS WHERE ID = '" + ID + "'");
            }
            if (type.Equals("DOCUMENT"))
            {
                db.execute("UPDATE SBS_DOCUMENT SET TITLE='" + SQLColumn.toSql(TITLE) + "',DESCRIPTION='" + SQLColumn.toSql(DESCRIPTION) + "',AUTHOR='" + SQLColumn.toSql(AUTHOR) + "', DOZENT='" + SQLColumn.toSql(DOZENT) + "' WHERE ID ='" + ID + "'");
                dataSeminar = db.getDataTable("SELECT * FROM SBS_DOCUMENT WHERE ID = '" + ID + "'");
            }
            db.disconnect();
            return GetSemionardataJson(dataSeminar);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        private string GetType(long uid)
        {
            string[] types = new string[] { "SEMINAR", "FOLDER", "DOCUMENT" };
            string type = "";
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable typeTable = db.getDataTable("SELECT UID, ID, SEMINAR_ID,PARENT_ID, FOLDER_ID, DOCUMENT_ID FROM SBSSeminarData WHERE UID = " + uid);
            if (Convert.ToInt32(typeTable.Rows[0]["SEMINAR_ID"]) == 0 && Convert.ToInt32(typeTable.Rows[0]["PARENT_ID"]) == 0)
            {
                type = types[0];
            }
            if (Convert.ToInt32(typeTable.Rows[0]["SEMINAR_ID"]) > 0 && Convert.ToInt32(typeTable.Rows[0]["PARENT_ID"]) > 0)
            {
                type = types[1];
            }
            if (Convert.ToInt32(typeTable.Rows[0]["DOCUMENT_ID"]) > 0)
            {
                type = types[2];
            }
            db.disconnect();
            return type;
        }

        private string GetSemionardataJson(DataTable seminardata)
        {
            SeminarData seminarDaten = new SeminarData();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (seminardata.Columns.Contains("ID"))
                seminarDaten.ID = (long)seminardata.Rows[0]["ID"];
            else
                seminarDaten.ID = 0;
            if (seminardata.Columns.Contains("UID"))
                seminarDaten.UID = (long)seminardata.Rows[0]["UID"];
            else
                seminarDaten.UID = 0;
            if (seminardata.Columns.Contains("SEMINAR_ID"))
                seminarDaten.SEMINAR_ID = (long)seminardata.Rows[0]["SEMINAR_ID"];
            else
                seminarDaten.SEMINAR_ID = 0;
            if (seminardata.Columns.Contains("PARENT_ID"))
                seminarDaten.PARENT_ID = (long)seminardata.Rows[0]["PARENT_ID"];
            else
                seminarDaten.PARENT_ID = 0;
            if (seminardata.Columns.Contains("FOLDER_ID"))
                seminarDaten.FOLDER_ID = (long)seminardata.Rows[0]["FOLDER_ID"];
            else
                seminarDaten.FOLDER_ID = 0;
            if (seminardata.Columns.Contains("FOLDER"))
                seminarDaten.FOLDER = seminardata.Rows[0]["FOLDER"].ToString();
            else
                seminarDaten.FOLDER = "";
            if (seminardata.Columns.Contains("NAME"))
                seminarDaten.NAME = seminardata.Rows[0]["NAME"].ToString();
            else
                seminarDaten.NAME = "";
            if (seminardata.Columns.Contains("TITLE"))
                seminarDaten.TITLE = seminardata.Rows[0]["TITLE"].ToString();
            else
                seminarDaten.TITLE = "";
            if (seminardata.Columns.Contains("URL"))
                seminarDaten.URL = seminardata.Rows[0]["URL"].ToString();
            else
                seminarDaten.URL = "";
            if (seminardata.Columns.Contains("DESCRIPTION"))
                seminarDaten.DESCRIPTION = seminardata.Rows[0]["DESCRIPTION"].ToString();
            else
                seminarDaten.DESCRIPTION = "";
            if (seminardata.Columns.Contains("AUTHOR"))
                seminarDaten.AUTHOR = seminardata.Rows[0]["AUTHOR"].ToString();
            else
                seminarDaten.AUTHOR = "";
            if (seminardata.Columns.Contains("DOZENT"))
                seminarDaten.DOZENT = seminardata.Rows[0]["DOZENT"].ToString();
            else
                seminarDaten.DOZENT = "";
            if (seminardata.Columns.Contains("DOCUMENT_ID"))
                seminarDaten.DOCUMENT_ID = (long)seminardata.Rows[0]["DOCUMENT_ID"];
            else
                seminarDaten.DOCUMENT_ID = 0;
            if (seminardata.Columns.Contains("ROOT_ID"))
                if (!seminardata.Rows[0]["ROOT_ID"].ToString().Equals(""))
                {
                    seminarDaten.ROOT_ID = (long)seminardata.Rows[0]["ROOT_ID"];
                }
                else
                    seminarDaten.ROOT_ID = 0;
            if (seminardata.Columns.Contains("DATUM"))
                seminarDaten.DATUM = seminardata.Rows[0]["DATUM"].ToString();
            else
                seminarDaten.DATUM = "";
            if (seminardata.Columns.Contains("RELEASE"))
                seminarDaten.RELEASE = String.IsNullOrEmpty(seminardata.Rows[0]["RELEASE"].ToString()) ? DateTime.MinValue : (DateTime)seminardata.Rows[0]["RELEASE"];

            return (js.Serialize(seminarDaten));
        }

        private class SeminarData
        {
            public long UID, ID, SEMINAR_ID, PARENT_ID, FOLDER_ID, DOCUMENT_ID, ROOT_ID;
            public string URL, FOLDER, NAME, TITLE, DESCRIPTION, AUTHOR, DOZENT, DATUM;
            public DateTime RELEASE;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetIdTyp(string folder, string path, bool isDirectory)
        {
            idType _idType = new idType();
            long uid = GetUid(path);
            _idType.id = uid;
            _idType.type = GetType(uid);
            return new JavaScriptSerializer().Serialize(_idType);
        }

        private class idType
        {
            public long id;
            public string type;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public long GetUid(string path)
        {
            long uid = 0;
            string rootFolder = Global.Config.getModuleParam("SBS", "SeminarsURl", "/").ToString();
            string searchPath = path.Substring(path.IndexOf(rootFolder) + rootFolder.Length);
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            uid = (long)db.lookup("UID", "SBSSeminarData", "URL = '" + SQLColumn.toSql(searchPath) + "'", 0L);
            db.disconnect();
            return uid;
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string addNewSeminar(string newSeminarName)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            newSeminarName = newSeminarName.Trim();
            long rootId = (long)db.lookup("ID", "SBS_SEMINARS", "ROOT_ID IS null");
            db.execute("INSERT INTO SBS_SEMINARS (URL, NAME, ROOT_ID) VALUES (" + "'/" + SQLColumn.toSql(newSeminarName) + "', '" + SQLColumn.toSql(newSeminarName) + "', " + rootId + ")");
            Directory.CreateDirectory(Global.Config.getModuleParam("SBS", "SeminarsPath", "") + "\\" + newSeminarName);

            return db.lookup("max(ID)", "SBS_SEMINARS", "").ToString();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddNewFolder(string newFolder, string parentFolder, string path)
        {
            if (newFolder.Length == 0 || newFolder.Equals(" "))
            {
                throw new ArgumentException();
            }
            DBData db = DBData.getDBData(Session);
            db.connect();
            string ret = "";
            try
            {
                string folderUrl = Global.Config.getModuleParam("SBS", "SeminarsURl", "");

                string[] Folders = path.Split(new Char[] { '/' });
                string seminarName = "";
                if (!parentFolder.Equals(folderUrl.Substring(1, folderUrl.Length - 1)))
                {
                    seminarName = Folders[Array.IndexOf(Folders, folderUrl.Remove(0, 1)) + 1];
                }
                StringBuilder newFolderPat = new StringBuilder();
                newFolderPat.Append("/");
                for (int i = Array.IndexOf(Folders, folderUrl.Remove(0, 1)) + 1; i < Folders.Length; i++)
                {
                    newFolderPat.Append(Folders[i] + "/");
                }
                newFolderPat.Append(newFolder.Trim());
                DataTable newData = new DataTable();
                if ((int)db.lookup("count(UID)", "SBSSeminarData", "URL= '" + newFolderPat.Replace("'", "''") + "'") == 0)
                {
                    string seminarId = "0";
                    string parentId = "0";
                    if (!seminarName.Equals(""))
                    {
                        seminarId = db.lookup("ID", "SBS_SEMINARS", "Name ='" + seminarName.Replace("'", "''") + "'", "0").ToString();
                        parentId = db.lookup("ID", "SBS_FOLDERS", "SEMINAR_ID ='" + seminarId + "' AND NAME ='" + parentFolder.Replace("'", "''") + "'", "0").ToString();
                    }



                    JavaScriptSerializer js = new JavaScriptSerializer();
                    if (parentId.Equals("0"))
                    {
                        parentId = seminarId;
                    }
                    if (seminarId.Equals("0"))
                    {
                        long rootId = (long)db.lookup("id", "SBS_SEMINARS", "ROOT_ID is null");
                        db.execute("INSERT INTO SBS_SEMINARS (NAME, URL, ROOT_ID) VALUES ('" + newFolder + "', '" + newFolderPat.ToString().Replace("'", "''") + "', " + rootId + ")");
                        newData = db.getDataTable("SELECT * FROM SBS_SEMINARS WHERE ID = (SELECT MAX(ID) FROM SBS_SEMINARS)");
                        db.execute("INSERT INTO SBS_USER_SEMINARS (USER_REF, SEMINAR_REF) VALUES(" + db.userId + "," + newData.Rows[0]["ID"] + ")");
                    }
                    else
                    {
                        db.execute("INSERT INTO SBS_FOLDERS (NAME, SEMINAR_ID, URL, PARENT_ID) VALUES ('" + newFolder.Replace("'", "''") + "', " + seminarId + ",'" + newFolderPat.ToString() + "'," + parentId + ")");
                        newData = db.getDataTable("SELECT * FROM SBS_FOLDERS WHERE ID = (SELECT MAX(ID) FROM SBS_FOLDERS)");
                    }
                }
                return GetSemionardataJson(newData);
            }

            catch (Exception ex)
            {
                Context.Response.StatusCode = 500;
                Logger.Log(ex, Logger.ERROR);
                ret = ex.Message;
            }
            finally
            {
                db.disconnect();
            }

            return (ret);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetItem(string folderId)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            string ret = "";
            DataTable dataSeminar = new DataTable();
            try
            {
                dataSeminar = db.getDataTable("SELECT * FROM SBSSeminarData WHERE UID = '" + folderId + "'");
                return GetSemionardataJson(dataSeminar);
            }

            catch (Exception ex)
            {
                Context.Response.StatusCode = 500;
                Logger.Log(ex, Logger.ERROR);
                ret = ex.Message;
            }
            finally
            {
                db.disconnect();
            }

            return (ret);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string FileUploaded(string path, string fileName)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable newData = new DataTable();
            string ret = "";
            long folderUid = GetUid(path);
            try
            {
                string folderId;
                long newId = (long)db.lookup("Folder_ID", "SBS_DOCUMENT", "UID =" + folderUid, 0L);
                if (newId > 0)
                {
                    folderId = newId.ToString();
                    folderUid = (long)db.lookup("UID", "SBSSeminarData", "ID =" + folderId, 0L);
                }
                else
                {
                    folderId = db.lookup("ID", "SBSSeminarData", "UID=" + folderUid).ToString();
                }
                string path_t = db.lookup("URL", "SBSSeminarData", "UID=" + folderUid).ToString().Replace("'", "''");
                if ((int)db.lookup("COUNT(ID)", "SBS_DOCUMENT", "URL='" + path_t + "/" + fileName.Replace("'", "''") + "'") == 0)
                {
                    db.execute("INSERT INTO SBS_DOCUMENT (NAME, URL, FOLDER_ID) VALUES ('" + fileName.Replace("'", "''") + "', '" + path_t + "/" + fileName.Replace("'", "''") + "','" + folderId + "')");

                }
                newData = db.getDataTable("SELECT * FROM SBS_DOCUMENT WHERE ID = (SELECT MAX(ID) FROM SBS_DOCUMENT)");
                db.disconnect();

                //delete png and json of pdf if present.
                //this is needed in case of replacement of an existing pdf
                deletePdfAttachedFiles(Global.Config.getModuleParam("SBS", "SeminarsPath", "") + newData.Rows[0]["URL"].ToString().Replace("/", "\\"));


                return TableToJson(newData);
            }

            catch (Exception ex)
            {
                Context.Response.StatusCode = 500;
                Logger.Log(ex, Logger.ERROR);
                ret = ex.Message;
            }
            finally
            {
                db.disconnect();
            }

            return (ret);

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string FileExplorerFileAddedCancel(string fileName)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            db.execute("DELETE FROM SBS_DOCUMENT WHERE ID = (SELECT max(ID) FROM SBS_DOCUMENT WHERE NAME = '" + fileName + "')");
            db.disconnect();
            return "ok";
        }

        private string TableToJson(DataTable source)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in source.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in source.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return js.Serialize(rows);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string FileExplorerDelete(string path)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            long uid = GetUid(path);
            string typ = GetType(uid);
            long id = (long)db.lookup("ID", "SBSSeminarData", "UID=" + uid);
            delRet ret = new delRet();
            ret.parentPath = GetParentPath(db, uid, typ);
            ret.parentUid = Convert.ToInt32(db.lookup("UID", "SBSSeminarData", "URL ='" + ret.parentPath.Replace("'", "''") + "'"));
            ret.typ = GetType(ret.parentUid);

            switch (typ)
            {
                case "SEMINAR":
                    Directory.Delete(Global.Config.getModuleParam("SBS", "SeminarsPath", "").Replace("\\Seminars", "") + path.Replace("/", "\\"), true);
                    db.execute("DELETE FROM SBS_SEMINARS WHERE ID=" + id);
                    break;
                case "FOLDER":
                    Directory.Delete(Global.Config.getModuleParam("SBS", "SeminarsPath", "").Replace("\\Seminars", "") + path.Replace("/", "\\"), true);
                    db.execute("DELETE FROM SBS_FOLDERS WHERE ID=" + id);
                    break;
                case "DOCUMENT":
                    deletePdfAttachedFiles(Server.MapPath(path));
                    db.execute("DELETE FROM SBS_DOCUMENT WHERE ID=" + id);
                    break;
            }
            db.disconnect();


            return new JavaScriptSerializer().Serialize(ret);
        }

        // delete png and JSON files of specified pdf file
        // takes absolute path to pdf
        private void deletePdfAttachedFiles(string path)
        {
            if (path.EndsWith(".pdf"))
            {

                string fileName = path.Split(Path.DirectorySeparatorChar)[path.Split(Path.DirectorySeparatorChar).Length - 1];
                string fileNameWOExtention = fileName.Substring(0, fileName.Length - 4);
                string folder = path.Substring(0, path.Length - fileName.Length);

                if (File.Exists(folder + fileNameWOExtention + ".js"))
                {
                    File.Delete(folder + fileNameWOExtention + ".js");
                }

                if (File.Exists(folder + fileNameWOExtention + "_1.png"))
                {
                    DirectoryInfo di = new DirectoryInfo(folder);
                    FileInfo[] fi = di.GetFiles(fileNameWOExtention + "_*.png");
                    foreach (FileInfo file in fi)
                    {
                        if (Regex.IsMatch(file.Name, "\\A" + fileNameWOExtention + "_\\d+.png"))
                            File.Delete(file.FullName);
                    }

                }

            }

        }

        private class delRet
        {
            public string parentPath, typ;
            public long parentUid;

        }

        private string GetParentPath(DBData db, long uid, string typ)
        {
            string parentUrl = "";
            switch (typ)
            {
                case "SEMINAR":
                    parentUrl = db.lookup("URL", "SBSSeminarData", "ID = (SELECT ROOT_ID FROM SBSSeminarData WHERE UID = " + uid + ")").ToString();
                    break;
                case "FOLDER":
                    parentUrl = db.lookup("URL", "SBSSeminarData", "ID = (SELECT PARENT_ID FROM SBSSeminarData WHERE UID = " + uid + ")").ToString();
                    break;
                case "DOCUMENT":
                    parentUrl = db.lookup("URL", "SBSSeminarData", "ID = (SELECT FOLDER_ID FROM SBSSeminarData WHERE UID = " + uid + ")").ToString();
                    break;
            }

            return parentUrl;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string FileExplorerItemRenamed(string path, string newName, string renamedType)
        {
            if (newName.Length == 0 || newName.Equals(" "))
            {
                throw new ArgumentException();
            }
            string folderUrl = Global.Config.getModuleParam("SBS", "SeminarsURl", "");
            string oldPath = path.Substring(path.IndexOf(folderUrl, 0) + folderUrl.Length);
            string oldName = oldPath.Substring(oldPath.LastIndexOf("/"));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable updatTable = db.getDataTable("select uid,id,name, url from SBSSeminarData where url like('%" + oldPath.Replace("'", "''") + "%')");
            string type = "";
            string newPath = "";
            string updateUids = "";
            long id = 0;
            DataTable newData = new DataTable();
            foreach (DataRow row in updatTable.Rows)
            {
                long uid = (long)row["UID"];
                updateUids += uid.ToString() + ",";
                type = GetType(uid);
                id = (long)row["ID"];
                string url = (string)row["URL"];
                bool itemMove = newName.Contains("/");
                if (itemMove && renamedType.Equals("DOCUMENT"))
                {
                    string newNameTmp = path.Substring(path.LastIndexOf("/") + 1);
                    newPath = newName.Substring(newName.IndexOf("Seminars/") + 8).Replace("'", "''") + "/" + newNameTmp.Replace("'", "''");
                    newName = newNameTmp;
                    if ((int)db.lookup("count(ID)", "SBS_DOCUMENT", "URL='" + newPath + "'", 0) > 0)
                    {
                        return "File bereits vorhanden";
                    }
                }
                else
                {
                    newPath = url.Replace(oldPath.Substring(oldPath.LastIndexOf("/") + 1), newName.Trim()).Replace("'", "''");
                }
                switch (type)
                {
                    case "SEMINAR":
                        db.execute("UPDATE SBS_SEMINARS SET NAME = '" + newName.Trim().Replace("'", "''") + "', URL = '" + newPath + "' WHERE ID=" + id);
                        break;
                    case "FOLDER":
                        if (renamedType.Equals("FOLDER"))
                        {
                            db.execute("UPDATE SBS_FOLDERS SET NAME = '" + newName.Trim().Replace("'", "''") + "', URL = '" + newPath + "' WHERE ID=" + id);
                        }
                        else
                        {
                            db.execute("UPDATE SBS_FOLDERS SET URL = '" + newPath + "' WHERE ID=" + id);
                        }
                        break;
                    case "DOCUMENT":
                        if (renamedType.Equals("DOCUMENT"))
                        {
                            db.execute("UPDATE SBS_DOCUMENT SET NAME = '" + newName.Trim().Replace("'", "''") + "', URL = '" + newPath + "' WHERE ID=" + id);
                        }
                        else
                        {
                            db.execute("UPDATE SBS_DOCUMENT SET URL = '" + newPath + "' WHERE ID=" + id);
                        }
                        deletePdfAttachedFiles(Global.Config.getModuleParam("SBS", "SeminarsPath", "") + oldPath.Replace("/", "\\"));
                        break;
                }


            }

            newData = db.getDataTable("SELECT * FROM SBSSeminarData WHERE UID in (" + updateUids.Substring(0, updateUids.Length - 1) + ")");
            db.disconnect();
            return TableToJson(newData);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string FileExplorerItemMove(string oldPath, string newPath, bool isDirectory, string itemName)
        {
            string folderUrl = Global.Config.getModuleParam("SBS", "SeminarsURl", "");
            string oldPathShort = oldPath.Substring(oldPath.IndexOf(folderUrl, 0) + folderUrl.Length);
            string newPathShort = newPath.Substring(newPath.IndexOf(folderUrl, 0) + folderUrl.Length);
            string updateUids = "";
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            long newFolderId = (long)db.lookup("id", "SBSSeminarData", "URL='" + newPathShort + "'");
            string type = GetType((long)db.lookup("uid", "SBSSeminarData", "URL='" + oldPathShort + "'"));
            string destinationType = GetType((long)db.lookup("uid", "SBSSeminarData", "URL='" + newPathShort + "'"));


            long newFolderUid = (long)db.lookup("uid", "SBSSeminarData", "URL='" + newPath.Substring(oldPath.IndexOf(folderUrl, 0) + folderUrl.Length) + "'");

            bool error = false;
            try
            {
                if (destinationType.Equals("SEMINAR") && !isDirectory) //is document moved to Seminar or root?
                {
                    error = true;
                }

                if (error == false)
                {

                    DataTable updatTable = db.getDataTable("select uid,id,name, url from SBSSeminarData where url like('%" + oldPathShort + "%')");
                    updatTable.Columns.Add("newUrl");
                    updatTable.Columns.Add("newParentId");
                    updatTable.Columns.Add("type");
                    foreach (DataRow row in updatTable.Rows)
                    {
                        row["type"] = GetType(Convert.ToInt32(row["uid"]));
                        if (updatTable.Rows.Count > 1)
                        {

                            int dirNumNewPath = Regex.Matches(newPathShort, Regex.Escape("/")).Count + 1;
                            int dirNumOldPath = Regex.Matches(oldPathShort, Regex.Escape("/")).Count + 1;
                            int dirNum = 0;
                            string oldUrl = (string)row["URL"];
                            string newUrl = "";

                            int i = 0;
                            int i1 = 0;
                            if (dirNumNewPath == dirNumOldPath)
                            {
                                dirNum = dirNumNewPath - 1;
                                int lengthFirstPartOldUrl = 0;
                                int lengthFirstPartNewUrl = 0;

                                for (int i2 = 0; i2 <= oldPathShort.Length - 1; i2++)
                                {
                                    if (oldPathShort.Substring(i2, 1).Equals("/"))
                                    {
                                        i = i + 1;
                                        if (i == dirNum)
                                        {
                                            lengthFirstPartOldUrl = i2;
                                        }
                                    }
                                }
                                i = 0;
                                i1 = 0;
                                for (int i2 = 0; i2 <= newPathShort.Length - 1; i2++)
                                {
                                    if (newPathShort.Substring(i2, 1).Equals("/"))
                                    {
                                        i = i + 1;
                                        if (i == dirNum)
                                        {
                                            lengthFirstPartNewUrl = i2;
                                        }
                                    }
                                }

                                newUrl = newPathShort + oldUrl.Substring(lengthFirstPartOldUrl);
                                row["newUrl"] = newUrl;


                            }

                            if (dirNumNewPath > dirNumOldPath)
                            {
                                dirNum = dirNumNewPath;
                                do
                                {
                                    newUrl = newUrl + oldUrl.Substring(i1, 1);
                                    if (newUrl.Substring(i1, 1).Equals("/"))
                                    {
                                        i = i + 1;
                                    }
                                    i1 = i1 + 1; ;
                                } while (i < dirNum);
                                newUrl = oldUrl.Replace(newUrl, newPathShort + "/");
                                row["newUrl"] = newUrl;
                            }

                            if (dirNumNewPath < dirNumOldPath)
                            {
                                dirNum = dirNumOldPath;
                                i = 0;
                                i1 = 0;
                                int endFirstPartUrl = 0;
                                int startSecondPartUrl = 0;
                                do
                                {
                                    if (oldPathShort.Substring(i1, 1).Equals("/"))
                                    {
                                        i = i + 1;
                                    }
                                    if (oldPathShort.Substring(i1, 1).Equals("/") && i == dirNumNewPath)
                                    {
                                        endFirstPartUrl = i1;
                                    }
                                    if (oldPathShort.Substring(i1, 1).Equals("/") && i == dirNumOldPath - 1)
                                    {
                                        startSecondPartUrl = i1;
                                    }
                                    i1 = i1 + 1; ;

                                } while (i < dirNum - 1);

                                //newUrl = oldUrl.Substring(0, endFirstPartUrl) + oldUrl.Substring(startSecondPartUrl);
                                newUrl = oldUrl.Replace(oldUrl.Substring(0, endFirstPartUrl), newPathShort); // + oldUrl.Substring(startSecondPartUrl);
                                row["newUrl"] = newUrl;

                            }
                            if (updatTable.Rows[0] == row)
                            {
                                row["newParentId"] = newFolderId;
                            }
                            else
                            {
                                row["newParentId"] = 0;
                            }

                        }
                        else
                        {
                            row["newParentId"] = newFolderId;
                            row["newUrl"] = newPathShort + "/" + itemName;
                        }
                    }

                    DataTable newData = new DataTable();
                    db.beginTransaction();
                    foreach (DataRow row in updatTable.Rows)
                    {
                        long uid = (long)row["UID"];
                        string isRoot = db.lookup("ROOT_ID", "SBSSeminarData", "UID=" + newFolderUid, "ROOT").ToString();
                        updateUids += uid.ToString() + ",";

                        if (row["type"].ToString().Equals("FOLDER") && isRoot.Equals("ROOT")) //is Folder moved to root?
                        {
                            error = true;
                        }

                        if (error == false)
                        {
                            long id = (long)row["ID"];
                            switch ((string)row["type"])
                            {
                                case "FOLDER":
                                    if (Convert.ToInt32(row["newParentId"]) > 0)
                                    {
                                        db.execute("UPDATE SBS_FOLDERS SET URL = '" + (string)row["newUrl"] + "', PARENT_ID = " + Convert.ToInt32(row["newParentId"]) + " WHERE ID=" + id);
                                    }
                                    else
                                    {
                                        db.execute("UPDATE SBS_FOLDERS SET URL = '" + (string)row["newUrl"] + "' WHERE ID=" + id);
                                    }
                                    break;
                                case "DOCUMENT":
                                    if (!isDirectory)
                                    {
                                        if (Convert.ToInt32(row["newParentId"]) > 0)
                                        {
                                            db.execute("UPDATE SBS_DOCUMENT SET URL = '" + (string)row["newUrl"] + "', FOLDER_ID = " + Convert.ToInt32(row["newParentId"]) + " WHERE ID=" + id);
                                        }
                                        else
                                        {
                                            db.execute("UPDATE SBS_DOCUMENT SET URL = '" + (string)row["newUrl"] + "' WHERE ID=" + id);
                                        }
                                    }
                                    else
                                    {
                                        db.execute("UPDATE SBS_DOCUMENT SET URL = '" + (string)row["newUrl"] + "' WHERE ID=" + id);
                                    }
                                    break;
                            }
                            newData = db.getDataTable("SELECT * FROM SBSSeminarData WHERE UID in (" + updateUids.Substring(0, updateUids.Length - 1) + ")");
                        }
                        else
                        {
                            throw new Exception("Fehler-" + "Element kann nicht an diesen Ort verschoben werden!");
                        }
                    }
                    db.commit();
                    return TableToJson(newData);
                }
                else
                {
                    throw new Exception("Fehler-" + "Element kann nicht an diesen Ort verschoben werden!");
                }
            }

            catch (Exception ex)
            {
                Context.Response.StatusCode = 500;
                Logger.Log(ex, Logger.ERROR);
                db.rollback();
                return ex.Message;
            }
            finally
            {
                db.disconnect();
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SetPersonId(string id)
        {
            HttpContext.Current.Session["PERSON_ID"] = id;
        }

        private void copySokratesMap(long matrixId, long userId)
        {
            long matrixIdNew;
            string matrixTitle = "";
            DBData db = DBData.getDBData(Session);
            db.connect();
            db.executeProcedure("copy_sokrates_map ", 0, 180, new ParameterCtx("MATRIXID", matrixId), new ParameterCtx("USERID", db.userId), new ParameterCtx("copyColoration", true), new ParameterCtx("copyWirkungspakete", true));
            //db.execute("exec copy_sokrates_map " + matrixId.ToString() +"," + db.userId.ToString(),0);
            matrixIdNew = Convert.ToInt32(db.lookup("max(id)", "MATRIX", "id>1"));
            matrixTitle = db.lookup("title", "matrix", "id=" + matrixIdNew).ToString();
            db.grantRowAuthorisation(14, userId, "Matrix", Convert.ToInt32(matrixIdNew));
            //set rights for new local knowlege elements 
            DataTable localKnowlege = db.getDataTable("SELECT KNOWLEDGE.ID FROM KNOWLEDGE INNER JOIN CHARACTERISTIC ON KNOWLEDGE.ID = CHARACTERISTIC.KNOWLEDGE_ID INNER JOIN DIMENSION ON CHARACTERISTIC.DIMENSION_ID = DIMENSION.ID INNER JOIN MATRIX ON DIMENSION.MATRIX_ID = MATRIX.ID WHERE (KNOWLEDGE.LOCAL = 1) AND (MATRIX.ID = " + matrixIdNew + ")");
            foreach (DataRow row in localKnowlege.Rows)
            {
                db.execute("EXECUTE COPY_ACCESS_RIGHT_ROW MATRIX, " + matrixId + " ,KNOWLEDGE," + row[0]);
            }
            //set rights for Matrix if GFK template
            //if ((bool)db.lookup("IS_GFK_TEMPLATE", "MATRIX", "ID = " + matrixId))
            //{
            //    db.execute("EXECUTE COPY_ACCESS_RIGHT_ROW MATRIX, " + matrixId + " ,MATRIX," + matrixIdNew);
            //}
            db.disconnect();
            //return matrixIdNew;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void copySokratesMapForAllUsers(long matrixId)
        {

            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable users = db.getDataTable("SELECT ID FROM PERSON WHERE ID > 11700");
            foreach (DataRow row in users.Rows)
            {
                copySokratesMap(matrixId, long.Parse(row["ID"].ToString()));
            }
            db.disconnect();

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateMark(string mark)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ServiceMark annotation = serializer.Deserialize<ServiceMark>(mark);
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            string path = Server.UrlDecode(annotation.pdfPath).Replace(".." + Global.Config.getModuleParam("SBS", "SeminarsURl", ""), "");
            string documentId = db.lookup("ID", "SBS_DOCUMENT", "URL = '" + path + "'", 0L).ToString();
            string userId = db.userId.ToString();



            if (annotation.type.Equals("drawing"))
            {
                db.execute("INSERT INTO SBS_ANNOTATIONS (ANNOTATION_ID, DOCUMENT_ID, USER_ID, SELECTION_TEXT, HAS_SELECTION, COLOR, SELECTION_INFO, "
                      + "READONLY, TYPE, PAGE_WIDTH, PAGE_HEIGHT, PAGE_INDEX, POINTS) "
                      + "VALUES "
                      + "('" + annotation.id + "','" + documentId + "','" + userId + "','" + SQLColumn.toSql(annotation.selection_text)
                      + "','" + annotation.has_selection + "','" + annotation.color + "','" + annotation.selection_info + "','" + annotation.Readonly + "','" + annotation.type + "','"
                      + annotation.pageWidth + "','" + annotation.pageHeight + "','" + annotation.pageIndex + "','"
                      + annotation.points + "')");
            }
            if (annotation.type.Equals("note"))
            {
                db.execute("INSERT INTO SBS_ANNOTATIONS (ANNOTATION_ID, DOCUMENT_ID, USER_ID, SELECTION_TEXT, HAS_SELECTION, COLOR, SELECTION_INFO, "
                      + "READONLY, TYPE, PAGE_WIDTH, PAGE_HEIGHT, NOTE, POSITION_X, POSITION_Y, WIDTH, HEIGHT, COLLAPSED, BB, UB, WA, PAGE_INDEX, POINTS, VD) "
                      + "VALUES "
                      + "('" + annotation.id + "','" + documentId + "','" + userId + "','" + SQLColumn.toSql(annotation.selection_text)
                      + "','" + annotation.has_selection + "','" + annotation.color + "','" + annotation.selection_info + "','" + annotation.Readonly + "','" + annotation.type + "','"
                      + annotation.pageWidth + "','" + annotation.pageHeight + "','" + SQLColumn.toSql(annotation.note) + "','" + annotation.positionX + "','" + annotation.positionY + "','"
                      + annotation.width + "','" + annotation.height + "','" + annotation.collapsed + "','" + annotation.bb + "','" + annotation.ub + "','" + annotation.wa + "','" + annotation.pageIndex + "','"
                      + annotation.points + "','" + annotation.vd + "')");
            }
            if (annotation.type.Equals("strikeout"))
            {
                db.execute("INSERT INTO SBS_ANNOTATIONS (ANNOTATION_ID, DOCUMENT_ID, USER_ID, SELECTION_TEXT, HAS_SELECTION, COLOR, SELECTION_INFO, "
                      + "READONLY, TYPE, PAGE_WIDTH, PAGE_HEIGHT, PAGE_INDEX, POINTS,WA, PA) "
                      + "VALUES "
                      + "('" + annotation.id + "','" + documentId + "','" + userId + "','" + SQLColumn.toSql(annotation.selection_text)
                      + "','" + annotation.has_selection + "','" + annotation.color + "','" + annotation.selection_info + "','" + annotation.Readonly + "','" + annotation.type + "','"
                      + annotation.pageWidth + "','" + annotation.pageHeight + "','" + annotation.pageIndex + "','"
                      + annotation.points + "','" + annotation.wa + "','" + annotation.pa + "')");
            }

            if (annotation.type.Equals("highlight"))
            {
                db.execute("INSERT INTO SBS_ANNOTATIONS (ANNOTATION_ID, DOCUMENT_ID, USER_ID, SELECTION_TEXT, HAS_SELECTION, COLOR, SELECTION_INFO, "
                     + "READONLY, TYPE, PAGE_WIDTH, PAGE_HEIGHT, PAGE_INDEX, POINTS,WA, PA) "
                     + "VALUES "
                     + "('" + annotation.id + "','" + documentId + "','" + userId + "','" + SQLColumn.toSql(annotation.selection_text)
                     + "','" + annotation.has_selection + "','" + annotation.color + "','" + annotation.selection_info + "','" + annotation.Readonly + "','" + annotation.type + "','"
                     + annotation.pageWidth + "','" + annotation.pageHeight + "','" + annotation.pageIndex + "','"
                     + annotation.points + "','" + annotation.wa + "','" + annotation.pa + "')");
            }
            db.disconnect();


        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteMark(string id)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            db.execute("DELETE FROM SBS_ANNOTATIONS WHERE ANNOTATION_ID= '" + id + "'");
            db.disconnect();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ChangeMark(string mark)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ServiceMark annotation = serializer.Deserialize<ServiceMark>(mark);
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            db.execute("UPDATE SBS_ANNOTATIONS SET SELECTION_TEXT = '" + SQLColumn.toSql(annotation.selection_text) + "'"
                                                 + ", HAS_SELECTION = '" + annotation.has_selection + "'"
                                                 + ", COLOR = '" + annotation.color + "'"
                                                 + ", SELECTION_INFO = '" + annotation.selection_info + "'"
                                                 + ", READONLY = '" + annotation.Readonly + "'"
                                                 + ", TYPE = '" + annotation.type + "'"
                                                 + ", PAGE_WIDTH = '" + annotation.pageWidth + "'"
                                                 + ", PAGE_HEIGHT = '" + annotation.pageHeight + "'"
                                                 + ", NOTE = '" + SQLColumn.toSql(annotation.note) + "'"
                                                 + ", POSITION_X = '" + annotation.positionX + "'"
                                                 + ", POSITION_Y = '" + annotation.positionY + "'"
                                                 + ", WIDTH = '" + annotation.width + "'"
                                                 + ", HEIGHT = '" + annotation.height + "'"
                                                 + ", COLLAPSED = '" + annotation.collapsed + "'"
                                                 + ", BB = '" + annotation.bb + "'"
                                                 + ", UB = '" + annotation.ub + "'"
                                                 + ", WA = '" + annotation.wa + "'"
                                                 + ", PAGE_INDEX = '" + annotation.pageIndex + "'"
                                                 + ", POINTS = '" + annotation.points + "'"
                                                 + ", VD = '" + annotation.vd + "'"
                                                 + ", PA = '" + annotation.vd + "'"
                                                 + " WHERE ANNOTATION_ID= '" + annotation.id + "'");
            db.disconnect();
        }

        public class ServiceMark
        {
            [DataMember]
            public string id { get; set; }
            [DataMember]
            public string documnet_id { get; set; }
            [DataMember]
            public string user_id { get; set; }
            [DataMember]
            public string type { get; set; }
            [DataMember]
            public string selection_text { get; set; }
            [DataMember]
            public string has_selection { get; set; }
            [DataMember]
            public string color { get; set; }
            [DataMember]
            public string selection_info { get; set; }
            [DataMember]
            public string Readonly { get; set; }
            [DataMember]
            public string pageWidth { get; set; }
            [DataMember]
            public string pageHeight { get; set; }
            [DataMember]
            public string note { get; set; }
            [DataMember]
            public string positionX { get; set; }
            [DataMember]
            public string positionY { get; set; }
            [DataMember]
            public string width { get; set; }
            [DataMember]
            public string height { get; set; }
            [DataMember]
            public string collapsed { get; set; }
            [DataMember]
            public string bb { get; set; }
            [DataMember]
            public string ub { get; set; }
            [DataMember]
            public string wa { get; set; }
            [DataMember]
            public string pa { get; set; }
            [DataMember]
            public string pageIndex { get; set; }
            [DataMember]
            public string points { get; set; }
            [DataMember]
            public string vd { get; set; }
            [DataMember]
            public string pdfPath { get; set; }

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMarks(string pdfPath)
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            string path = Server.UrlDecode(pdfPath).Replace(".." + Global.Config.getModuleParam("SBS", "SeminarsURl", ""), "");
            string documentId = db.lookup("ID", "SBS_DOCUMENT", "URL = '" + path + "'", 0L).ToString();
            string userId = db.userId.ToString();
            DataTable Annotations = db.getDataTable("SELECT ANNOTATION_ID AS id, SELECTION_TEXT AS selection_text, HAS_SELECTION AS has_selection, COLOR AS color, "
                                                    + "SELECTION_INFO as selection_info, READONLY AS readonly, TYPE AS type, PAGE_WIDTH AS pageWidth, "
                                                    + "PAGE_HEIGHT AS pageHeight, NOTE AS note, POSITION_X AS positionX, POSITION_Y AS positionY, WIDTH AS width, "
                                                    + "HEIGHT AS height, COLLAPSED as collapsed, BB AS Bb, UB AS ub, WA as Wa, PAGE_INDEX AS pageIndex, POINTS AS points, "
                                                    + " VD AS Vd, PA AS Pa FROM SBS_ANNOTATIONS WHERE USER_ID = " + userId + " AND DOCUMENT_ID = " + documentId);
            return TableToJson(Annotations);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SetSelectedSeminarId(string SeminarId)
        {
            Session.Add("SelectedSeminar", SeminarId);
            return "OK";
        }
    }

}


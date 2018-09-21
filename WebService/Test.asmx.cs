using ch.appl.psoft.Admin;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Organisation;
using ch.appl.psoft.Report;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using Telerik.Web.UI;

namespace ch.appl.psoft.WebService
{
    [WebService(Namespace = "https://www.p-soft.ch/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class Test1 : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetData(String seminarsJSON)
        {

            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable dataSeminar = new DataTable();
            dataSeminar = db.getDataTable("SELECT * FROM SBSSeminarData");
            string ret = TableToJson(dataSeminar);
            db.disconnect();
            return (ret);
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
        public string GetUser()
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable userTable;
            if (HttpContext.Current.Session["SelectedSeminar"] == null)
            {
                userTable = db.getDataTable("SELECT ID, PERSONNELNUMBER, FIRM, TITLE, PNAME, FIRSTNAME, FUNKTION, EMAIL, FUNKTION, LOGIN, PASSWORD  FROM PERSON ORDER BY PNAME");
            }
            else
            {
                DataTable activeSeminar = db.getDataTable("SELECT ID, NAME FROM SBS_SEMINARS WHERE ID =" + HttpContext.Current.Session["SelectedSeminar"]);
                if (activeSeminar.Rows[0]["NAME"].ToString().Trim().Equals(""))
                {
                    userTable = db.getDataTable("SELECT ID, PERSONNELNUMBER, FIRM, TITLE, PNAME, FIRSTNAME, FUNKTION, EMAIL, FUNKTION, LOGIN, PASSWORD  FROM PERSON ORDER BY PNAME");
                }
                else
                {
                    userTable = db.getDataTable("SELECT ID, PERSONNELNUMBER, FIRM, TITLE, PNAME, FIRSTNAME, FUNKTION, EMAIL, FUNKTION, LOGIN, PASSWORD  FROM PERSON WHERE ID IN (SELECT USER_REF FROM SBS_USER_SEMINARS WHERE SEMINAR_REF = " + HttpContext.Current.Session["SelectedSeminar"] + ") ORDER BY PNAME");
                }
            }
            string ret = TableToJson(userTable);
            db.disconnect();
            return (ret);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateUser(object userJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string struserJSON = serializer.Serialize(userJSON);
            List<ServiceUser> users = (List<ServiceUser>)serializer.Deserialize(struserJSON, typeof(List<ServiceUser>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            foreach (ServiceUser serviceUser in users)
            {
                db.execute("UPDATE PERSON SET EMAIL= '" + toSql(serviceUser.Email) + "', FIRSTNAME = '" + toSql(serviceUser.Firstname) + "', FIRM = '" + toSql(serviceUser.Firm) + "', FUNKTION = '" + toSql(serviceUser.Funktion) + "', PERSONNELNUMBER = '" + toSql(serviceUser.PersonnelNumber) + "', TITLE = '" + toSql(serviceUser.Title) + "', PNAME = '" + toSql(serviceUser.PName) + "', LOGIN='" + toSql(serviceUser.Login) + "', PASSWORD='" + toSql(serviceUser.Password) + "' WHERE ID =" + serviceUser.Id);
            }
            db.disconnect();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public string DeleteUser(object userJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string struserJSON = serializer.Serialize(userJSON);
            List<ServiceUser> users = (List<ServiceUser>)serializer.Deserialize(struserJSON, typeof(List<ServiceUser>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable deletedUsers = db.getDataTable("SELECT ID, PERSONNELNUMBER, FIRM, TITLE, PNAME, FIRSTNAME, FUNKTION, EMAIL, LOGIN, PASSWORD  FROM PERSON WHERE ID = 0 ");
            foreach (ServiceUser serviceUser in users)
            {
                db.execute("DELETE SBS_USER_SEMINARS WHERE USER_REF=" + serviceUser.Id);
                db.execute("DELETE SBS_USER_MATRIX WHERE USER_REF=" + serviceUser.Id);
                db.execute("DELETE KNOWLEDGE WHERE BASE_THEME_ID_DE IN(SELECT ID FROM THEME WHERE CREATOR_PERSON_ID=" + serviceUser.Id + ") OR "
                          + "BASE_THEME_ID_FR IN(SELECT ID FROM THEME WHERE CREATOR_PERSON_ID=" + serviceUser.Id + ") OR "
                          + "BASE_THEME_ID_EN IN(SELECT ID FROM THEME WHERE CREATOR_PERSON_ID=" + serviceUser.Id + ") OR "
                          + "BASE_THEME_ID_IT IN(SELECT ID FROM THEME WHERE CREATOR_PERSON_ID=" + serviceUser.Id + ")");
                db.execute("DELETE THEME WHERE CREATOR_PERSON_ID=" + serviceUser.Id);
                db.execute("DELETE PERSON WHERE ID =" + serviceUser.Id);
                DataRow workRow = deletedUsers.NewRow();
                workRow["ID"] = serviceUser.Id;
                workRow["PERSONNELNUMBER"] = serviceUser.PersonnelNumber;
                workRow["FIRM"] = serviceUser.Firm;
                workRow["TITLE"] = serviceUser.Title;
                workRow["PNAME"] = serviceUser.PName;
                workRow["FIRSTNAME"] = serviceUser.Firstname;
                workRow["FUNKTION"] = serviceUser.Funktion;
                workRow["EMAIL"] = serviceUser.Email;
                workRow["LOGIN"] = serviceUser.Login;
                workRow["PASSWORD"] = serviceUser.Password;

                deletedUsers.Rows.Add(workRow);
            }
            db.disconnect();
            return TableToJson(deletedUsers);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]

        public string InsertUser(object userJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string struserJSON = serializer.Serialize(userJSON);
            List<ServiceUser> users = (List<ServiceUser>)serializer.Deserialize(struserJSON, typeof(List<ServiceUser>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable newUsers = db.getDataTable("SELECT ID, PERSONNELNUMBER, FIRM, TITLE, PNAME, FIRSTNAME, FUNKTION, EMAIL, LOGIN, PASSWORD  FROM PERSON WHERE ID = 0 ");
            foreach (ServiceUser serviceUser in users)
            {
                db.execute("INSERT INTO PERSON (EMAIL, FIRSTNAME, FIRM, FUNKTION, PERSONNELNUMBER, TITLE, PNAME, LOGIN, PASSWORD) VALUES('" + toSql(serviceUser.Email) + "','" + toSql(serviceUser.Firstname) + "','" + toSql(serviceUser.Firm) + "','" + toSql(serviceUser.Funktion) + "','" + toSql(serviceUser.PersonnelNumber) + "','" + toSql(serviceUser.Title) + "','" + toSql(serviceUser.PName) + "','" + toSql(serviceUser.Login) + "','" + toSql(serviceUser.Password) + "')");
                string userId = db.lookup("max(ID)", "PERSON", "").ToString();
                db.execute("insert into accessor (id, visible, tablename, row_id) values ( " + userId + ", 0, 'PERSON', " + userId + ")");
                db.execute("insert into ACCESSOR_GROUP_ASSIGNMENT (ACCESSOR_MEMBER_ID, ACCESSOR_GROUP_ID) values (" + userId + ", 1)");
                DataRow workRow = newUsers.NewRow();
                workRow["ID"] = userId;
                workRow["PERSONNELNUMBER"] = serviceUser.PersonnelNumber;
                workRow["FIRM"] = serviceUser.Firm;
                workRow["TITLE"] = serviceUser.Title;
                workRow["PNAME"] = serviceUser.PName;
                workRow["FIRSTNAME"] = serviceUser.Firstname;
                workRow["FUNKTION"] = serviceUser.Funktion;
                workRow["EMAIL"] = serviceUser.Email;
                workRow["LOGIN"] = serviceUser.Login;
                workRow["PASSWORD"] = serviceUser.Password;

                newUsers.Rows.Add(workRow);

                string activeSeminarName = db.lookup("NAME", "SBS_SEMINARS", "ID='" + HttpContext.Current.Session["SelectedSeminar"] + "'").ToString();
                if (!activeSeminarName.Equals(""))
                {
                    db.execute("INSERT INTO SBS_USER_SEMINARS ( USER_REF, SEMINAR_REF) VALUES(" + userId + ", " + HttpContext.Current.Session["SelectedSeminar"] + ")");
                }


            }
            db.disconnect();
            return TableToJson(newUsers);
        }

        [DataContract]

        public class ServiceUser
        {
            [DataMember]
            public string Id { get; set; }
            [DataMember]
            public string PersonnelNumber { get; set; }
            [DataMember]
            public string Title { get; set; }
            [DataMember]
            public string PName { get; set; }
            [DataMember]
            public string Firstname { get; set; }
            [DataMember]
            public string Funktion { get; set; }
            [DataMember]
            public string Email { get; set; }
            [DataMember]
            public string Firm { get; set; }
            [DataMember]
            public string Login { get; set; }
            [DataMember]
            public string Password { get; set; }
            [DataMember]
            public string __type { get; set; }
        }

        public class UserResult
        {
            public List<ServiceUser> Data { get; set; }
            public int Count { get; set; }
        }

        public class UserEditResult
        {
            public List<ServiceUser> models { get; set; }
        }

        public static string toSql(string val)
        {
            if (!(val == null))
            {
                return val.Replace("'", "''");
            }
            else
            {
                return "";
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetUserSeminars()
        {
            string userId = HttpContext.Current.Session["PERSON_ID"].ToString();
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable dataUserSeminar = new DataTable();
            string ret = "";
            try
            {
                //    dataUserSeminar = db.getDataTable("SELECT DISTINCT ID AS SEMINAR_ID, NAME AS SEMINAR_TITLE, cast(0 AS BIT) AS ISACTIVE , " + userId + " AS PERSON_ID FROM SBS_SEMINARS WHERE SBS_SEMINARS.ID > (select min(SBS_SEMINARS.ID)from SBS_SEMINARS) AND NOT ID IN(SELECT SEMINAR_REF FROM SBS_USER_SEMINARS WHERE USER_REF = " + userId + ") "
                //                                        + "UNION "
                //                                      + "SELECT DISTINCT SEMINAR_REF AS SEMINAR_ID, (SELECT NAME FROM SBS_SEMINARS WHERE ID = SBS_USER_SEMINARS.SEMINAR_REF)  AS SEMINAR_TITLE, IIF(SEMINAR_REF is NULL,  cast(1 AS BIT), cast(1 AS BIT)) AS ISACTIVE, USER_REF AS PERSON_ID FROM SBS_USER_SEMINARS WHERE USER_REF = " + userId + " ORDER BY ISACTIVE DESC, SEMINAR_TITLE");
                dataUserSeminar = db.getDataTable("SELECT DISTINCT ID AS SEMINAR_ID, NAME AS SEMINAR_TITLE, cast(0 AS BIT) AS ISACTIVE , " + userId + " AS PERSON_ID FROM SBS_SEMINARS WHERE SBS_SEMINARS.ID > (select min(SBS_SEMINARS.ID)from SBS_SEMINARS) AND NOT ID IN(SELECT SEMINAR_REF FROM SBS_USER_SEMINARS WHERE USER_REF = " + userId + ") "
                                                        + "UNION "
                                                      + "SELECT DISTINCT SEMINAR_REF AS SEMINAR_ID, (SELECT NAME FROM SBS_SEMINARS WHERE ID = SBS_USER_SEMINARS.SEMINAR_REF)  AS SEMINAR_TITLE, cast(1 AS BIT) AS ISACTIVE, USER_REF AS PERSON_ID FROM SBS_USER_SEMINARS WHERE USER_REF = " + userId + " ORDER BY ISACTIVE DESC, SEMINAR_TITLE");
            }

            catch (Exception ex)
            {
                //Context.Response.StatusCode = 500;
                //Logger.Log(ex, Logger.ERROR);
                ret = ex.Message;
            }
            finally
            {
                db.disconnect();
            }

            return TableToJson(dataUserSeminar);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]


        public void UpdateSeminars(object seminarsJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string strseminarsJSON = serializer.Serialize(seminarsJSON);
            List<ServiceSeminars> seminars = (List<ServiceSeminars>)serializer.Deserialize(strseminarsJSON, typeof(List<ServiceSeminars>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);

            db.connect();
            foreach (ServiceSeminars ServiceSeminars in seminars)
            {
                if (Convert.ToInt32(db.lookup("Count(ID)", "SBS_USER_SEMINARS", "USER_REF = " + ServiceSeminars.PERSON_ID + " AND SEMINAR_REF = " + ServiceSeminars.SEMINAR_ID)) == 0)
                {
                    if (ServiceSeminars.ISACTIVE)
                    {
                        db.execute("INSERT INTO SBS_USER_SEMINARS (USER_REF, SEMINAR_REF) VALUES (" + ServiceSeminars.PERSON_ID + "," + ServiceSeminars.SEMINAR_ID + ")");
                    }
                }
                else
                {
                    if (ServiceSeminars.ISACTIVE == false)
                    {
                        db.execute("DELETE FROM SBS_USER_SEMINARS WHERE USER_REF = " + ServiceSeminars.PERSON_ID + " AND SEMINAR_REF = " + ServiceSeminars.SEMINAR_ID);

                    }

                }
            }
            db.disconnect();
        }

        public class ServiceSeminars
        {
            [DataMember]
            public long SEMINAR_ID { get; set; }
            [DataMember]
            public string SEMINAR_TITLE { get; set; }
            [DataMember]
            public bool ISACTIVE { get; set; }
            [DataMember]
            public long PERSON_ID { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMatrix()
        {
            long userFilterId = 0;
            if (HttpContext.Current.Session["PERSON_ID"] != null)
            {
                userFilterId = Convert.ToInt32(HttpContext.Current.Session["PERSON_ID"]);
            }
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable matrixTable;
            if (userFilterId > 0)
            {
                matrixTable = db.getDataTable("SELECT DISTINCT ID AS MATRIX_ID, " + userFilterId + " AS PERSON_ID, TITLE AS MATRIX_TITLE, cast(0 AS BIT) AS ISACTIVE, cast(0 AS BIT) AS READWRITE FROM MATRIX WHERE IS_SBS_TEMPLATE = 1"
                                                       + " UNION "
                                                      + " SELECT DISTINCT MATRIX_REF AS MATRIX_ID, " + userFilterId + " AS PERSON_ID, (SELECT TITLE FROM MATRIX WHERE ID = SBS_USER_MATRIX.MATRIX_REF)  AS MATRIX_TITLE, cast(1 AS BIT) AS ISACTIVE, READWRITE FROM SBS_USER_MATRIX WHERE USER_REF = " + userFilterId + " ORDER BY ISACTIVE DESC, MATRIX_TITLE");
                matrixTable = delDoubleMatrixRecords(matrixTable);
            }
            else
            {
                matrixTable = new DataTable(); // db.getDataTable("SELECT 0 AS MATRIX_ID, 0 AS PERSON_ID, '' AS MATRIX_TITLE, cast(0 AS BIT) AS ISACTIVE, cast(0 AS BIT) AS READWRITE FROM SBS_USER_MATRIX WHERE USER_REF = " + userFilterId);
            }

            string ret = TableToJson(matrixTable);
            db.disconnect();
            return (ret);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateMatrix(object matrixJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string strmatrixJSON = serializer.Serialize(matrixJSON);
            List<ServiceMatrixs> matrixs = (List<ServiceMatrixs>)serializer.Deserialize(strmatrixJSON, typeof(List<ServiceMatrixs>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            foreach (ServiceMatrixs ServiceMatrix in matrixs)
            {
                if (Convert.ToInt32(db.lookup("Count(ID)", "SBS_USER_MATRIX", "USER_REF = " + ServiceMatrix.PERSON_ID + " AND MATRIX_REF = " + ServiceMatrix.MATRIX_ID)) == 0)
                {
                    if (ServiceMatrix.ISACTIVE)
                    {
                        SokratesService sService = new SokratesService();
                        DataTable subMatrixs = getSubmatrixs(ServiceMatrix.MATRIX_ID);
                        long newRootMatrixId = sService.copyMatrixWithSubmatrixs((long)ServiceMatrix.PERSON_ID, subMatrixs, 14, false);

                        db.execute("INSERT INTO SBS_USER_MATRIX (USER_REF, MATRIX_REF, READWRITE, COPY_MATRIX_ID) VALUES (" + ServiceMatrix.PERSON_ID + "," + ServiceMatrix.MATRIX_ID + ",'" + ServiceMatrix.READWRITE + "', " + newRootMatrixId + ")");
                        db.execute("UPDATE SBS_USER_MATRIX SET COPY_MATRIX_ID = " + newRootMatrixId + " WHERE USER_REF = " + ServiceMatrix.PERSON_ID + " AND MATRIX_REF = " + subMatrixs.Rows[0]["matrixID"]);
                        db.execute("UPDATE MATRIX SET TITLE = REPLACE((SELECT TITLE FROM MATRIX WHERE ID = " + newRootMatrixId + "), 'Copy of ', '') WHERE ID =" + newRootMatrixId);

                    }
                }
                else
                {
                    if (ServiceMatrix.ISACTIVE == false)
                    {
                        string delMatrixId = db.lookup("COPY_MATRIX_ID", "SBS_USER_MATRIX", "USER_REF =" + ServiceMatrix.PERSON_ID + " AND MATRIX_REF =" + ServiceMatrix.MATRIX_ID).ToString();
                        db.execute("DELETE FROM SBS_USER_MATRIX WHERE USER_REF = " + ServiceMatrix.PERSON_ID + " AND MATRIX_REF = " + ServiceMatrix.MATRIX_ID);
                        SokratesService sService = new SokratesService();
                        sService.delSokrates(delMatrixId);
                    }
                    else
                    {
                        db.execute("UPDATE SBS_USER_MATRIX SET READWRITE = '" + ServiceMatrix.READWRITE + "' WHERE USER_REF = " + ServiceMatrix.PERSON_ID + " AND MATRIX_REF = " + ServiceMatrix.MATRIX_ID);
                    }

                }
                
            }
            db.disconnect();
        }

        public class ServiceMatrixs
        {
            [DataMember]
            public long MATRIX_ID { get; set; }
            [DataMember]
            public long PERSON_ID { get; set; }
            [DataMember]
            public string MATRIX_TITLE { get; set; }
            [DataMember]
            public bool ISACTIVE { get; set; }
            [DataMember]
            public bool READWRITE { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMatrixSeminar()
        {
            long seminarFilterId = 0;
            if (HttpContext.Current.Session["SelectedSeminar"] != null)
            {
                seminarFilterId = Convert.ToInt32(HttpContext.Current.Session["SelectedSeminar"]);
            }
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable matrixTable;
            if (seminarFilterId > 0)
            {
                matrixTable = db.getDataTable("SELECT DISTINCT ID AS MATRIX_ID, " + seminarFilterId + " AS SEMINAR_REF, TITLE AS MATRIX_TITLE, cast(0 AS BIT) AS ISACTIVE, cast(0 AS BIT) AS READWRITE FROM MATRIX WHERE IS_SBS_TEMPLATE = 1"
                                                       + " UNION "
                                                      + " SELECT DISTINCT MATRIX_REF AS MATRIX_ID, " + seminarFilterId + " AS SEMINAR_REF, (SELECT TITLE FROM MATRIX WHERE ID = SBS_SEMINAR_MATRIX.MATRIX_REF)  AS MATRIX_TITLE, cast(1 AS BIT) AS ISACTIVE, READWRITE FROM SBS_SEMINAR_MATRIX WHERE SEMINAR_REF = " + seminarFilterId + " ORDER BY ISACTIVE DESC, MATRIX_TITLE");
                matrixTable = delDoubleMatrixRecords(matrixTable);
            }
            else
            {
                matrixTable = new DataTable();
            }

            string ret = TableToJson(matrixTable);
            db.disconnect();
            return (ret);
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void UpdateMatrixSeminar(object matrixJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string strmatrixJSON = serializer.Serialize(matrixJSON);
            List<ServiceMatrixsSeminar> matrixs = (List<ServiceMatrixsSeminar>)serializer.Deserialize(strmatrixJSON, typeof(List<ServiceMatrixsSeminar>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            foreach (ServiceMatrixsSeminar ServiceMatrix in matrixs)
            {
                if (Convert.ToInt32(db.lookup("Count(ID)", "SBS_SEMINAR_MATRIX", "MATRIX_REF = " + ServiceMatrix.MATRIX_ID + " AND SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF)) == 0)
                {
                    DataTable userIds = db.getDataTable("SELECT USER_REF FROM  SBS_USER_SEMINARS WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + " AND NOT USER_REF IN (SELECT USER_REF FROM SBS_USER_MATRIX WHERE MATRIX_REF = " + ServiceMatrix.MATRIX_ID + ")");
                    if (ServiceMatrix.ISACTIVE)
                    {
                        db.execute("INSERT INTO SBS_SEMINAR_MATRIX (MATRIX_REF, SEMINAR_REF, READWRITE) VALUES (" + ServiceMatrix.MATRIX_ID + "," + ServiceMatrix.SEMINAR_REF + ",'" + ServiceMatrix.READWRITE + "')");
                        //add Matrix to each Seminaruser
                        db.execute("INSERT INTO SBS_USER_MATRIX (USER_REF, MATRIX_REF, READWRITE) SELECT USER_REF, " + ServiceMatrix.MATRIX_ID + " AS MATRIX_REF,'" + ServiceMatrix.READWRITE + "' AS READWRITE FROM SBS_USER_SEMINARS WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + " AND NOT USER_REF IN (SELECT USER_REF FROM SBS_USER_MATRIX WHERE MATRIX_REF = " + ServiceMatrix.MATRIX_ID + ")");

                        DataTable subMatrixs = getSubmatrixs(ServiceMatrix.MATRIX_ID);

                        SokratesService sService = new SokratesService();

                        if (ServiceMatrix.READWRITE)
                        {
                            bool linkSubmatrixs;

                            linkSubmatrixs = false;

                            psoftHub test = new psoftHub();
                            foreach (DataRow usrId in userIds.Rows)
                            {
                                DataTable userCopy = db.getDataTable("SELECT FIRSTNAME, PNAME FROM PERSON WHERE ID= " + (long)usrId["USER_REF"]);
                                string message = "Die Karten für " + userCopy.Rows[0][0].ToString() + " " + userCopy.Rows[0][1].ToString() + " werden bereitgestellt!";
                                test.SendNotifications(message);
                                long newRootMatrixId = sService.copyMatrixWithSubmatrixs((long)usrId["USER_REF"], subMatrixs, 14, linkSubmatrixs);
                                db.execute("UPDATE MATRIX SET TITLE = REPLACE((SELECT TITLE FROM MATRIX WHERE ID = " + newRootMatrixId + "), 'Copy of ', '') WHERE ID =" + newRootMatrixId);
                                db.execute("UPDATE SBS_USER_MATRIX SET COPY_MATRIX_ID = " + newRootMatrixId + " WHERE USER_REF = " + (long)usrId["USER_REF"] + " AND MATRIX_REF = " + subMatrixs.Rows[0]["matrixID"]);
                            }

                            test.SendNotifications("");
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    if (ServiceMatrix.ISACTIVE == false)
                    {

                        DataTable MatrixsDelete = db.getDataTable("SELECT COPY_MATRIX_ID FROM SBS_USER_MATRIX WHERE MATRIX_REF = " + ServiceMatrix.MATRIX_ID + " AND USER_REF IN (SELECT USER_REF FROM SBS_USER_SEMINARS WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + " AND USER_REF IN (SELECT USER_REF FROM SBS_USER_MATRIX AS SBS_USER_MATRIX_1 WHERE MATRIX_REF = " + ServiceMatrix.MATRIX_ID + "))");
                        DataTable userIds = db.getDataTable("SELECT USER_REF FROM  SBS_USER_SEMINARS WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + " AND USER_REF IN (SELECT USER_REF FROM SBS_USER_MATRIX WHERE MATRIX_REF = " + ServiceMatrix.MATRIX_ID + ")");
                        SokratesService sService = new SokratesService();
                        psoftHub hub = new psoftHub();
                        hub.SendNotifications("Die Karten werden gelöscht!");
                        foreach (DataRow delId in MatrixsDelete.Rows)
                        {
                            sService.delSokrates(delId["COPY_MATRIX_ID"].ToString());
                        }
                        db.execute("DELETE SBS_SEMINAR_MATRIX WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + " AND MATRIX_REF = " + ServiceMatrix.MATRIX_ID);
                        db.execute("DELETE FROM SBS_USER_MATRIX WHERE MATRIX_REF = " + ServiceMatrix.MATRIX_ID + " AND USER_REF IN (SELECT USER_REF FROM SBS_USER_SEMINARS WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + ")");
                        hub.SendNotifications("");

                    }
                    else
                    {
                        db.execute("UPDATE SBS_SEMINAR_MATRIX SET READWRITE = '" + ServiceMatrix.READWRITE + "' WHERE SEMINAR_REF = " + ServiceMatrix.SEMINAR_REF + " AND MATRIX_REF = " + ServiceMatrix.MATRIX_ID);
                    }

                }
            }
            db.disconnect();
        }

        private DataTable getSubmatrixs(long matrixId)
        {
            SokratesService sService = new SokratesService();
            //Wenn alle Submatrixen kopiert werden sollen
            //DataTable subMatrixs = sService.getMatrxWithSubmatrixs(ServiceMatrix.MATRIX_ID);
            //linkSubmatrixs = true
            //

            //wenn nur erste Matrix kopiert werden soll
            DataTable subMatrixs = new DataTable();
            DataColumn matrixIdCol = new DataColumn("matrixId", typeof(long));
            subMatrixs.Columns.Add(matrixIdCol);
            subMatrixs.Rows.Add();
            subMatrixs.Rows[0]["matrixId"] = matrixId; ;
            subMatrixs.AcceptChanges();
            return subMatrixs;
        }

        private DataTable delDoubleMatrixRecords(DataTable matrixTable)
        {
            DataTable tmpTable = matrixTable.Copy();
            foreach (DataRow row in tmpTable.Rows)
            {
                if ((bool)row["ISACTIVE"])
                {
                    DataRow[] delRow = matrixTable.Select("MATRIX_ID = " + row["MATRIX_ID"] + " AND ISACTIVE = 'False'");
                    if (delRow.Length > 0)
                        matrixTable.Rows.Remove(delRow[0]);


                }
            }
            return matrixTable;
        }

        public class ServiceMatrixsSeminar
        {
            [DataMember]
            public long MATRIX_ID { get; set; }
            [DataMember]
            public long SEMINAR_REF { get; set; }
            [DataMember]
            public string MATRIX_TITLE { get; set; }
            [DataMember]
            public bool ISACTIVE { get; set; }
            [DataMember]
            public bool READWRITE { get; set; }
        }


    }
}

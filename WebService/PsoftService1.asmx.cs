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

    public class PsoftService11 : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMyMaps()
        {
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            db.connect();
            DataTable myMatrixs = new DataTable();
            myMatrixs = db.getDataTable("SELECT DISTINCT ID, TITLE, DESCRIPTION, convert(datetime, CREATIONDATE, 104) AS CREATIONDATE, convert(datetime, LASTCHANGE, 104) AS LASTCHANGE  FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " ORDER BY TITLE ASC");
            DataColumn matrixLink = new DataColumn();
            matrixLink.ColumnName = "matrixLink";
            myMatrixs.Columns.Add(matrixLink);

            foreach (DataRow matrixRow in myMatrixs.Rows)
            {
                matrixRow["matrixLink"] = "MatrixDetail.aspx?matrixID=" + matrixRow["ID"].ToString();
                matrixRow["DESCRIPTION"] = HttpUtility.HtmlEncode(matrixRow["DESCRIPTION"].ToString());//.Replace("\"", "''");
            }
            string ret = TableToJson(myMatrixs);
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

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(EnableSession = true)]
        public string GetWages()
        {
            LanguageMapper map = LanguageMapper.getLanguageMapper(HttpContext.Current.Session);
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            string sql = "";
            db.connect();
            sql = "SELECT PERSON.ID AS PERSON_ID, ORGENTITY.TITLE_" + map.LanguageCode + " AS ORGENTITY, PERSON.PERSONNELNUMBER AS PERSONNELNUMBER, "
                          + "PERSON.PNAME AS NAME, PERSON.FIRSTNAME AS FIRSTNAME, JOB.TITLE_" + map.LanguageCode + " AS JOB, ";
            //if (Convert.ToInt16(Global.Config.getModuleParam("report", "tarifLohn", "0")) == 1)
            //{
            //    sql = sql + "CASE WHEN LOHN.TARIFLOEHNER = 0 THEN 'FALSE' ELSE 'TRUE' AS [" + map.get("WageTable", "TARIFLOEHNER") + "], LOHN.TARIFLOHN AS [" + map.get("WageTable", "TARIFLOHN") + "], LOHN.AUSSER_TARIFLOHN AS [" + map.get("WageTable", "AUSSERTARIFLOHN") + "], ";
            //}
            sql = sql + "LOHN.ISTLOHN AS WAGE, CAST(COALESCE(LOHN.AUSSCHLUSS_LOHN,0)as bit) AS EXCLUSION_WAGE,  CAST(COALESCE(LOHN.AUSSCHLUSS,0)as bit) AS EXCLUSION, CONVERT(VARCHAR(10), LOHN.AUSSCHLUSS_BIS, 104) AS EXCLUSION_TO "
                      + "FROM JOB INNER JOIN FUNKTION ON JOB.FUNKTION_ID = FUNKTION.ID INNER JOIN "
                      + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN "
                      + "PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID LEFT OUTER JOIN LOHN ON EMPLOYMENT.ID = LOHN.EMPLOYMENT_ID "
                      + "WHERE (PERSON.LEAVING < { fn NOW() } OR PERSON.LEAVING IS NULL) AND JOB.HAUPTFUNKTION = 1  ORDER BY ORGENTITY, Name";

            DataTable wage = db.getDataTable(sql, Logger.VERBOSE);
            //foreach(DataRow wageRow in wage.Rows)
            //{
            //   if(wageRow["WAGE"].ToString().Length == 0)
            //    {
            //        wageRow["WAGE"] = "0";
            //    }
            //}
            string ret = TableToJson(wage);
            db.disconnect();
            return (ret);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [WebMethod(EnableSession = true)]
        public void UpdateWages(object wageJSON)
        {
            LanguageMapper map = LanguageMapper.getLanguageMapper(HttpContext.Current.Session);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string strwageJSON = serializer.Serialize(wageJSON);
            List<userWagesList> wageList = (List<userWagesList>)serializer.Deserialize(strwageJSON, typeof(List<userWagesList>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            string sql = "";
            db.connect();
            string sqlPersonIds = "in( ";
            foreach (userWagesList userWage in wageList)
            {
                long employmentId = db.lookup("id", "employment", "PERSON_ID = " + userWage.PERSON_ID, 0L);
                sqlPersonIds += userWage.PERSON_ID + ",";

                //if not exists wage in table lohn --> insert
                if (db.lookup("id", "lohn", "employment_id = " + employmentId, 0L) == 0)
                {
                    long variante = db.lookup("ID", "VARIANTE", "HAUPTVARIANTE = 1", 0L);
                    sql = "INSERT INTO LOHN (EXTERNAL_REF, LOHNART, EMPLOYMENT_ID, VARIANTE_ID) VALUES ("
                         + userWage.PERSON_ID + ", " + 2 + ", "
                         + employmentId + ", " + variante + ")";
                    db.execute(sql);

                }

                sql = "UPDATE LOHN SET ";

                if (userWage.WAGE.Length > 0)
                {
                    sql += "ISTLOHN = " + userWage.WAGE;
                }
                else
                {
                    sql += "ISTLOHN = 0";
                }
                if (userWage.EXCLUSION)
                {
                    sql += ", AUSSCHLUSS = " + 1;
                }
                else
                {
                    sql += ", AUSSCHLUSS = " + 0;
                }
                if (userWage.EXCLUSION_WAGE)
                {
                    sql += ", AUSSCHLUSS_LOHN = " + 1;
                }
                else
                {
                    sql += ", AUSSCHLUSS_LOHN = " + 0;
                }
                if (!(userWage.EXCLUSION_TO == null || userWage.EXCLUSION_TO.Length == 0))
                {
                    sql += ", AUSSCHLUSS_BIS = '" + DateTime.Parse(userWage.EXCLUSION_TO).ToString("MM.dd.yyyy") + "'";
                }
                else
                {
                    sql += ", AUSSCHLUSS_BIS = NULL";
                }

                sql += " WHERE EMPLOYMENT_ID = " + employmentId;
                db.execute(sql);
            }
            sqlPersonIds = sqlPersonIds.Substring(0, sqlPersonIds.Length - 1);
            sqlPersonIds += ")";

            DataTable updateRec = db.getDataTable("SELECT PERSON.ID AS PERSON_ID, ORGENTITY.TITLE_de AS ORGENTITY, PERSON.PERSONNELNUMBER AS PERSONNELNUMBER, PERSON.PNAME AS NAME, PERSON.FIRSTNAME AS FIRSTNAME, JOB.TITLE_de AS JOB, LOHN.ISTLOHN AS WAGE, CAST(COALESCE(LOHN.AUSSCHLUSS_LOHN,0)as bit) AS EXCLUSION_WAGE,  CAST(COALESCE(LOHN.AUSSCHLUSS,0)as bit) AS EXCLUSION, CONVERT(VARCHAR(10), LOHN.AUSSCHLUSS_BIS, 104) AS EXCLUSION_TO FROM JOB INNER JOIN FUNKTION ON JOB.FUNKTION_ID = FUNKTION.ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID LEFT OUTER JOIN LOHN ON EMPLOYMENT.ID = LOHN.EMPLOYMENT_ID WHERE (PERSON.LEAVING < { fn NOW() } OR PERSON.LEAVING IS NULL) AND JOB.HAUPTFUNKTION = 1 AND PERSON.ID " + sqlPersonIds + " ORDER BY ORGENTITY, Name");
            db.disconnect();
            // return TableToJson(updateRec);
        }

        private class userWagesList
        {
            private string _PERSONNELNUMBER;
            [DataMember]
            public long PERSON_ID { get; set; }
            [DataMember]
            public string ORGENTITY { get; set; }

            [DataMember]
            public string PERSONNELNUMBER
            {
                get { return _PERSONNELNUMBER; }
                set
                {
                    if (value == null)
                        _PERSONNELNUMBER = "";
                    else
                        _PERSONNELNUMBER = value;
                }
            }

            [DataMember]
            public string NAME { get; set; }
            [DataMember]
            public string FIRSTNAME { get; set; }
            [DataMember]
            public string WAGE { get; set; }

            [DataMember]
            public bool EXCLUSION_WAGE { get; set; }
            [DataMember]
            public bool EXCLUSION { get; set; }
            [DataMember]
            public string EXCLUSION_TO { get; set; }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(EnableSession = true)]
        public string GetWagesCorrection()
        {
            LanguageMapper map = LanguageMapper.getLanguageMapper(HttpContext.Current.Session);
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            string sql = "";
            db.connect();


            DataTable wage = db.getDataTable("SELECT ID, PERSONNELNUMBER,JobTitel AS JOB, PNAME AS NAME,FIRSTNAME,IstLohn AS WAGE,KORR1,KORR2,KORR3,KORR4,FIX FROM f_Solllohnkorrektur(" + Global.Config.getModuleParam("report", "anzMonatsloehne", "13") + ") ORDER BY JobTitel, PNAME");

            string ret = TableToJson(wage);
            db.disconnect();
            return (ret);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        [WebMethod(EnableSession = true)]
        public void UpdateWagesCorrection(object wageJSON)
        {
            LanguageMapper map = LanguageMapper.getLanguageMapper(HttpContext.Current.Session);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string strwageJSON = serializer.Serialize(wageJSON);
            List<wageCorretionList> wageCorrectionList = (List<wageCorretionList>)serializer.Deserialize(strwageJSON, typeof(List<wageCorretionList>));
            DBData db = DBData.getDBData(HttpContext.Current.Session);
            string sql = "";
            db.connect();
            try
            {
                foreach (wageCorretionList wageCorrection in wageCorrectionList)
                {
                    string isInCorrectionTable = db.lookup("PERSONNELNUMBER", "SOLLLOHNKORREKTUR", "PERSONNELNUMBER =" + wageCorrection.PERSONNELNUMBER, "").ToString();
                    if (isInCorrectionTable.Length == 0)
                    {
                        sql = "INSERT INTO SOLLLOHNKORREKTUR (PERSONNELNUMBER, KORR1, KORR2, KORR3, KORR4, FIX) VALUES('" + wageCorrection.PERSONNELNUMBER + "', '" + wageCorrection.KORR1 + "', '" + wageCorrection.KORR2 + "', '" + wageCorrection.KORR3 + "', '" + wageCorrection.KORR4 + "', '" + wageCorrection.FIX + "')";
                    }
                    else
                    {
                        sql = "UPDATE SOLLLOHNKORREKTUR SET KORR1 = '" + wageCorrection.KORR1 + "', KORR2 =  '" + wageCorrection.KORR2 + "', KORR3 =  '" + wageCorrection.KORR3 + "', KORR4 =  '" + wageCorrection.KORR4 + "', FIX =  '" + wageCorrection.FIX + "'  WHERE PERSONNELNUMBER = '" + wageCorrection.PERSONNELNUMBER + "'";
                    }
                    db.execute(sql);
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }
        }

        private class wageCorretionList
        {
            [DataMember]
            public long ID { get; set; }
            public string NAME { get; set; }
            [DataMember]
            public string FIRSTNAME { get; set; }
            [DataMember]
            public string ORGENTITY { get; set; }
            [DataMember]
            public string PERSONNELNUMBER { get; set; }
            [DataMember]
            public string WAGE { get; set; }
            [DataMember]
            public string KORR1 { get; set; }
            [DataMember]
            public string KORR2 { get; set; }
            [DataMember]
            public string KORR3 { get; set; }
            [DataMember]
            public string KORR4 { get; set; }
            [DataMember]
            public string FIX { get; set; }
        }
    }
}

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
/// <summary>
/// Zusammenfassungsbeschreibung für AdminService
/// </summary>.WebService
{
    [WebService(Namespace = "http://www.p-soft.ch/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AdminService : System.Web.Services.WebService
    {

        // ------------ Organisation ----------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RadTreeNodeData[] getOrganistionTreeData(long nodeId, Dictionary<string, string> attributes)
        {
            CustomDataErrorObj ret = GetRetObj();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable nodeTable = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            try
            {
                RadTreeNodeData nodeData = new RadTreeNodeData();
                nodeData.Attributes.Add("TYP", attributes.GetValueOrNull("TYP"));
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";


                nodeTable = db.getDataTable("SELECT * FROM JOB WHERE ID=" + nodeId);
                foreach (DataRow treeNode in nodeTable.Rows)
                {
                    nodeData.Text = treeNode["TITLE_" + lang].ToString();
                    nodeData.Value = treeNode["ID"].ToString();

                    if (db.lookup("PROXY_PERSON_ID", "JOB", "ID=" + nodeId).ToString().Equals(""))
                    {
                        nodeData.Attributes.Add("VAKANT", "TRUE");
                        nodeData.ImageUrl = imageUrl + "og_stelle_vakant.gif";
                    }
                    else
                    {
                        nodeData.Attributes.Add("VAKANT", "FALSE");
                        nodeData.ImageUrl = imageUrl + "og_stelle_vakant.gif";
                        nodeData.Text += " " + db.lookup("PNAME + ' ' + FIRSTNAME", "JOB INNER JOIN  EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID", "JOB.ID = " + nodeId).ToString();
                    }

                    result.Add(nodeData);
                }


            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentityReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return result.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getOrgentityFromJob(long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable oeTable = new DataTable();
            try
            {
                oeTable = db.getDataTable("SELECT ORGENTITY.ID FROM JOB INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID WHERE JOB.ID = " + jobId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentityReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj(TableToJson(oeTable));
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getOrgData(string id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable oeTable = new DataTable();
            try
            {
                oeTable = db.getDataTable("SELECT TITLE_" + lang + " AS TITLE, MNEMONIC_" + lang + " AS MNEMONIC, DESCRIPTION_" + lang + " AS DESCRIPTION, CLIPBOARD_ID,  CLIPBOARD.TITLE AS CLIPBOARD_TITLE FROM ORGENTITY LEFT OUTER JOIN CLIPBOARD ON ORGENTITY.CLIPBOARD_ID = CLIPBOARD.ID WHERE ORGENTITY.ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentityReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }



            return ret.getErrorObj(TableToJson(oeTable));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveOrgentity(long id, string title, string mnemonic, string description)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            try
            {
                db.execute("UPDATE ORGENTITY SET TITLE_" + lang + " = '" + SQLColumn.toSql(title) + "', MNEMONIC_" + lang + " = '" + SQLColumn.toSql(mnemonic) + "', DESCRIPTION_" + lang + " = '" + SQLColumn.toSql(description) + "' WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveOrgentityFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteOrgentity(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                string oeIds = db.Orgentity.addAllSubOEIDs(id.ToString());
                DataTable oeTable = db.getDataTable("SELECT ID FROM ORGENTITY WHERE ID IN (" + oeIds + ")");
                foreach (DataRow oe in oeTable.Rows)
                {
                    DataTable oeJobs = db.getDataTable("SELECT ID FROM JOB WHERE ORGENTITY_ID = " + oe["ID"].ToString());
                    string jobIds = "";
                    foreach (DataRow job in oeJobs.Rows)
                    {
                        jobIds += job["ID"].ToString() + ",";
                    }
                    if (jobIds.Length > 0)
                    {
                        jobIds = jobIds.Remove(jobIds.Length - 1, 1);
                        DeleteJob(jobIds);
                    }
                    DataTable chartNodes = db.getDataTable("SELECT ID FROM CHARTNODE WHERE ORgENTITY_ID = " + oe["ID"].ToString());
                    foreach (DataRow node in chartNodes.Rows)
                    {
                        db.execute("DELETE CHARTTEXT WHERE CHARTNODE_ID =" + node["ID"].ToString());
                    }

                    db.execute("DELETE FROM CHARTNODE WHERE ORGENTITY_ID = " + oe["ID"].ToString());
                    db.execute("UPDATE EMPLOYMENT SET ORGENTITY_ID = null WHERE ORGENTITY_ID = " + oe["ID"].ToString());
                    db.execute("UPDATE OBJECTIVE SET ORGENTITY_ID = null WHERE ORGENTITY_ID IN(" + oeIds + ")");
                    db.execute("UPDATE ORGENTITY SET PARENT_ID = null WHERE ID IN(" + oeIds + ")");
                    db.execute("DELETE FROM ORGENTITY WHERE ID = " + oe["ID"].ToString() + " OR PARENT_ID = " + oe["ID"].ToString());

                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteOrgentityFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddOrgentity(long parentId)
        {
            CustomDataErrorObj ret = GetRetObj();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DataTable orgentity = new DataTable();
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                string orgentityName = _map.get("organisation", "oe");
                long rootId = db.lookup("ID", "ORGENTITY", "PARENT_ID IS null", 0L);

                db.execute("INSERT INTO ORGENTITY (ID, PARENT_ID, ROOT_ID, TITLE_" + lang + ") VALUES(" + db.newId("ORGENTITY") + ", " + parentId + ", " + rootId + ", '" + orgentityName + "')");
                orgentity = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM ORGENTITY WHERE ID = (SELECT MAX(ID) FROM ORGENTITY)");

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "addOrgentityFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj(TableToJson(orgentity));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveOrgentity(long sourceOeId, long targetOeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            try
            {
                db.execute("UPDATE ORGENTITY SET PARENT_ID = " + targetOeId + " WHERE ID =" + sourceOeId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveOrgentityFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }


        //--------------  JOB ---------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getJobData(string id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable jobTable = new DataTable();
            try
            {
                jobTable = db.getDataTable("SELECT JOB.ID AS JOB_ID, JOB.TITLE_" + lang + "  AS JOB_TITLE, ORGENTITY.TITLE_" + lang + "  AS ORGENTITY_TITLE, FUNKTION.ID AS FUNKTION_ID, FUNKTION.TITLE_" + lang + "  AS FUNKTION_TITLE, EMPLOYMENT.ID AS EMPLOYMENT_ID, "
                                                   + "EMPLOYMENT.TITLE_" + lang + "  AS EMPLOYMENT_TITLE, JOB.PROXY_PERSON_ID, (SELECT PNAME + ' ' + FIRSTNAME FROM PERSON WHERE ID = JOB.PROXY_PERSON_ID) AS PROXY_NAME, JOB.MNEMONIC_" + lang + "  AS JOb_MNEMONIC, JOB.FROM_DATE, JOB.TO_DATE, JOB.ENGAGEMENT, JOB.DESCRIPTION_" + lang + "  AS JOB_DESCRIPTION, "
                                                   + "JOB.TYP, JOB.HAUPTFUNKTION "
                                            + "FROM JOB INNER JOIN "
                                                   + "ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN "
                                                   + "FUNKTION ON JOB.FUNKTION_ID = FUNKTION.ID INNER JOIN "
                                                   + "EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID WHERE JOB.ID = " + id);
                if (jobTable.Rows.Count == 0)
                {
                    jobTable = db.getDataTable("SELECT JOB.ID AS JOB_ID, JOB.TITLE_DE AS JOB_TITLE, ORGENTITY.TITLE_DE AS ORGENTITY_TITLE, FUNKTION.ID AS FUNKTION_ID, FUNKTION.TITLE_DE AS FUNKTION_TITLE, NULL "
                           + "AS EMPLOYMENT_ID, NULL AS EMPLOYMENT_TITLE, JOB.PROXY_PERSON_ID, "
                           + "(SELECT PNAME + ' ' + FIRSTNAME AS Expr1 FROM PERSON WHERE (ID = JOB.PROXY_PERSON_ID)) AS PROXY_NAME, JOB.MNEMONIC_DE AS JOb_MNEMONIC, JOB.FROM_DATE, JOB.TO_DATE, JOB.ENGAGEMENT, "
                           + "JOB.DESCRIPTION_DE AS JOB_DESCRIPTION, JOB.TYP, JOB.HAUPTFUNKTION "
                      + "FROM JOB INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID INNER JOIN FUNKTION ON JOB.FUNKTION_ID = FUNKTION.ID "
                      + "WHERE JOB.ID =" + id);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readJobFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj(TableToJson(jobTable));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string JobSetVacant(long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DataTable job = new DataTable();
            db.connect();

            try
            {
                db.execute("UPDATE JOB SET EMPLOYMENT_ID = NULL WHERE ID=" + jobId);
                job = db.getDataTable("SELECT ID, EMPLOYMENT_ID, TITLE_" + lang + " AS TITLE FROM JOB WHERE ID =" + jobId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "proxysetVacantFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(job));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SetJobOwner(long personId, long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable jobTable = new DataTable();
            try
            {
                long employmentId = db.lookup("ID", "EMPLOYMENT", "PERSON_ID  =" + personId, 0L);
                db.execute("UPDATE JOB SET EMPLOYMENT_ID = " + employmentId + " WHERE ID = " + jobId);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "setJobOwnerFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"OK\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveJob(long id, string jobTitle, string mnemonic, string bg, string description, int typ, bool mainfunction, long funktionId, string toDate)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);

            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            try
            {
                string sql = "UPDATE JOB SET TITLE_" + lang + " = '" + SQLColumn.toSql(jobTitle) + "', MNEMONIC_" + lang + " = '" + SQLColumn.toSql(mnemonic) + "', DESCRIPTION_" + lang + " = '" + SQLColumn.toSql(description) + "', TYP = " + typ + ", FUNKTION_ID = " + funktionId;
                if (mainfunction)
                {
                    sql += ", HAUPTFUNKTION = 1";
                }
                else
                {
                    sql += ", HAUPTFUNKTION = 0";
                }
                if (!(toDate == null))
                {
                    sql += ", TO_DATE = '" + DateTime.ParseExact(toDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                }
                if (bg.Length > 0)
                {
                    sql += ", ENGAGEMENT = " + bg;
                }

                sql += " WHERE ID =" + id;

                db.execute(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveJobFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddJob(long oeId, long personId)
        {
            CustomDataErrorObj ret = GetRetObj();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable newJob = new DataTable();
            try
            {
                long employmentId = db.lookup("ID", "EMPLOYMENT", "PERSON_ID  =" + personId, 0L);
                int existJobEmployment = (int)db.lookup("count(id)", "JOB", "EMPLOYMENT_ID = " + employmentId);
                int hauptfunktion = 1;
                if (existJobEmployment > 0)
                {
                    hauptfunktion = 0;
                }
                DataTable defaultFunktionTable = db.getDataTable("SELECT ID, TITLE_DE, TITLE_FR, TITLE_EN, TITLE_IT FROM FUNKTION WHERE DFLT = 1");
                if (defaultFunktionTable.Rows.Count == 0)
                {
                    return "[{\"ERROR\":\" Keine Defaultfunktion definiert.\"}]";
                }

                if (employmentId > 0)
                {
                    db.execute("INSERT INTO JOB ( TITLE_DE, TITLE_FR, TITLE_EN, TITLE_IT, FUNKTION_ID, ORGENTITY_ID, EMPLOYMENT_ID, HAUPTFUNKTION, FROM_DATE, ENGAGEMENT) VALUES('" + defaultFunktionTable.Rows[0]["TITLE_DE"] + "', '" + defaultFunktionTable.Rows[0]["TITLE_FR"] + "', '" + defaultFunktionTable.Rows[0]["TITLE_EN"] + "', '" + defaultFunktionTable.Rows[0]["TITLE_IT"] + "', " + defaultFunktionTable.Rows[0]["ID"] + ", " + oeId + ", " + employmentId + ", " + hauptfunktion + ", '" + System.DateTime.Now.ToString("MM.dd.yyyy hh:mm:ss") + "', 0)");
                }
                else
                {
                    db.execute("INSERT INTO JOB ( TITLE_DE, TITLE_FR, TITLE_EN, TITLE_IT, FUNKTION_ID, ORGENTITY_ID, HAUPTFUNKTION, FROM_DATE, ENGAGEMENT) VALUES('" + defaultFunktionTable.Rows[0]["TITLE_DE"] + "', '" + defaultFunktionTable.Rows[0]["TITLE_FR"] + "', '" + defaultFunktionTable.Rows[0]["TITLE_EN"] + "', '" + defaultFunktionTable.Rows[0]["TITLE_IT"] + "', " + defaultFunktionTable.Rows[0]["ID"] + ", " + oeId + ", " + hauptfunktion + ", '" + System.DateTime.Now.ToString("MM.dd.yyyy hh:mm:ss") + "', 0)");
                }
                newJob = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM JOB WHERE ID = (SELECT MAX(ID) FROM JOB)");
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveJobFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj(TableToJson(newJob));
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteJob(string id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                //set JOB_ID to null for PERFORMANCERATING
                if (Global.isModuleEnabled("performance"))
                {
                    db.execute("update PERFORMANCERATING set JOB_ID = null where JOB_ID in (" + id + ")");
                }

                //set JOB_ID to null and update date for JOB_DEVELOPMENT 

                db.execute("update JOB_DEVELOPMENT set TO_DATE = getdate() where TO_DATE is null and JOB_ID in (" + id + ")");
                db.execute("update JOB_DEVELOPMENT set JOB_ID = null where JOB_ID in (" + id + ")");

                db.execute("DELETE FROM JOB WHERE ID in (" + id + ")");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteJobFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveJob(long sourceJobId, long targetOeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                if (targetOeId == 0)
                {
                    targetOeId = db.lookup("ID", "ORGENTITY", "PARENT_ID ID null", 0L);
                }
                db.execute("UPDATE JOB SET ORGENTITY_ID = " + targetOeId + " WHERE ID = " + sourceJobId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteJobFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"RESULT\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetJobsPerson(long personId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable jobs = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            try
            {
                jobs = db.getDataTable("SELECT JOB.ID, JOB.HAUPTFUNKTION, JOB.TITLE_" + lang + " AS TITLE FROM PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID WHERE PERSON.ID = " + personId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "getJobsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj(TableToJson(jobs));
        }

        //------------- Person ---------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPersonFromJob(long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable person = new DataTable();
            try
            {
                person = db.getDataTable("SELECT PERSON.ID FROM JOB INNER JOIN EMPLOYMENT ON JOB.EMPLOYMENT_ID = EMPLOYMENT.ID INNER JOIN PERSON ON EMPLOYMENT.PERSON_ID = PERSON.ID WHERE JOB.ID = " + jobId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "getPersonFromJobFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(person));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SubstituteSetVacant(long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("UPDATE JOB SET PROXY_PERSON_ID = NULL WHERE ID=" + jobId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "SubstituteSetVacantFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj("[{\"proxiPersonId\":\"NULL\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetEmployment(long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable employment = new DataTable();
            try
            {
                long employmentId = db.lookup("EMPLOYMENT_ID", "JOB", "ID =" + jobId, 0L);
                employment = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM EMPLOYMENT WHERE ID = " + employmentId);
                employment.Columns.Add("ENGAGEMENT", System.Type.GetType("System.Decimal"));

                double eng = Convert.ToDouble(db.lookup("SUM(ENGAGEMENT)", "JOB", "EMPLOYMENT_ID = " + employmentId));
                employment.Rows[0]["ENGAGEMENT"] = eng;

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readEmploymentFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj(TableToJson(employment));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SetEmployment(long employmentId, string employmentTitle)
        {
            CustomDataErrorObj ret = GetRetObj();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("UPDATE EMPLOYMENT SET TITLE_" + lang + " = '" + SQLColumn.toSql(employmentTitle) + "' WHERE ID = " + employmentId);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "writeEmploymentFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getPersonData(string personId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            DataTable person = new DataTable();
            try
            {
                person = db.getDataTable("SELECT PERSON.FIRM_ID, PERSON.CLIPBOARD_ID, CLIPBOARD.TITLE AS CLIPBOARD_TITLE, PERSON.PNAME, PERSON.FIRSTNAME, PERSON.MNEMO, PERSON.TITLE, PERSON.PERSONNELNUMBER, PERSON.SEX, "
                                            + "PERSON.MARTIAL, PERSON.DATEOFBIRTH, PERSON.ENTRY, PERSON.LEAVING, PERSON.KNOWLEDGE, PERSON.LOGIN, PERSON.PASSWORD, PERSON.EMAIL, "
                                            + "PERSON.PHONE, PERSON.MOBILE, PERSON.PHOTO, PERSON.SALUTATION_ADDRESS, PERSON.SALUTATION_LETTER, PERSON.BESCH_GRAD, PERSON.BERUFSERFAHRUNG, "
                                            + "PERSON.LEADERSHIP, PERSON.COMMENTS "
                                            + "FROM PERSON LEFT OUTER JOIN CLIPBOARD ON PERSON.CLIPBOARD_ID = CLIPBOARD.ID WHERE PERSON.ID = " + personId);
                if (person.Rows[0]["CLIPBOARD_TITLE"].ToString().Length == 0)
                {
                    person.Rows[0]["CLIPBOARD_TITLE"] = _map.get("global", "createClipboard");
                    person.Rows[0]["CLIPBOARD_ID"] = "0";
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "personReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj(TableToJson(person));

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RadTreeNodeData[] getPersonTreeData(RadTreeNodeData node, object context)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            try
            {
                db.connect();
                IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
                string name = contextDictionary["name"].ToString().Replace("'", "''");
                string firstname = contextDictionary["firstname"].ToString().Replace("'", "''");
                string mnemo = contextDictionary["mnemo"].ToString().Replace("'", "''");
                string personnelnumber = contextDictionary["personnelnumber"].ToString().Replace("'", "''");
                string orgentity = contextDictionary["orgentity"].ToString().Replace("'", "''");
                bool showformer = Convert.ToBoolean(contextDictionary["showformer"].ToString());

                string _sql = "select DISTINCT ID, PNAME, FIRSTNAME from PERSONOEV where TYP = 1 ";
                if (name.Length > 0)
                {
                    _sql += "AND PNAME LIKE '%" + name + "%'";
                }
                if (firstname.Length > 0)
                {
                    _sql += "AND FIRSTNAME LIKE '%" + firstname + "%'";
                }
                if (mnemo.Length > 0)
                {
                    _sql += "AND MNEMO LIKE '%" + mnemo + "%'";
                }
                if (personnelnumber.Length > 0)
                {
                    _sql += "AND PERSONNELNUMBER LIKE '%" + personnelnumber + "%'";
                }
                if (!(orgentity == "0"))
                {
                    _sql += "AND OE_ID = '" + orgentity + "'";
                }

                if (!showformer)
                {
                    _sql += "AND (LEAVING IS NULL OR LEAVING > '" + DateTime.Now.ToString("yyyy.MM.dd") + "')";
                }
                _sql += " ORDER BY PNAME, FIRSTNAME";

                DataTable personTable = db.getDataTable(_sql);

                foreach (DataRow person in personTable.Rows)
                {
                    RadTreeNodeData nodeData = new RadTreeNodeData();
                    nodeData.Text = person["PNAME"].ToString() + " " + person["FIRSTNAME"].ToString(); ;
                    nodeData.Value = person["ID"].ToString();
                    result.Add(nodeData);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "personReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            // add data for child nodes           

            return result.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string savePerson(long id, string firm_id, string name, string firstname, string mnemo, string title, string personnelnumber, string sex, string martial, string dateofbirth, string entry, string leaving, string login, string password, string email, string phone, string mobile, string photo, string salutationaddress, string salutationletter, string beschgrad, string berufserfahrung, string leadership, string comment)
        {
            CustomDataErrorObj ret = GetRetObj();
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                if (leadership.Equals("True"))
                {
                    leadership = "1";
                }
                else
                {
                    leadership = "0";
                }
                //new person
                if (id == 0)
                {
                    string sql = "INSERT INTO PERSON  (TYP, PNAME, FIRSTNAME, MNEMO, TITLE, PERSONNELNUMBER, SEX, MARTIAL, LOGIN, PASSWORD, EMAIL, PHONE, MOBILE, PHOTO, SALUTATION_ADDRESS, SALUTATION_LEtTER, BESCH_GRAD, BERUFSERFAHRUNG, LEADERSHIP, COMMENTS";
                    if (!(dateofbirth == null))
                    {
                        sql += ", DATEOFBIRTH";
                    }
                    if (!(entry == null))
                    {
                        sql += ", ENTRY";
                    }
                    if (!(leaving == null))
                    {
                        sql += ", LEAVING";
                    }
                    sql += ") VALUES(1,'" + SQLColumn.toSql(name) + "', '" + SQLColumn.toSql(firstname) + "', '" + SQLColumn.toSql(mnemo) + "', '" + SQLColumn.toSql(title) + "', '" + SQLColumn.toSql(personnelnumber) + "', '" + sex + "', '" + martial + "', '" + SQLColumn.toSql(login) + "', '" + SQLColumn.toSql(password) + "', '" + SQLColumn.toSql(email) + "', '" + SQLColumn.toSql(phone) + "', '" + SQLColumn.toSql(mobile) + "', '" + SQLColumn.toSql(photo) + "', '" + SQLColumn.toSql(salutationaddress) + "', '" + SQLColumn.toSql(salutationletter) + "', '" + beschgrad + "', '" + berufserfahrung + "', '" + leadership + "', '" + SQLColumn.toSql(comment) + "'";
                    if (!(dateofbirth == null))
                    {
                        sql += ", '" + DateTime.ParseExact(dateofbirth, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    }
                    if (!(entry == null))
                    {
                        sql += ", '" + DateTime.ParseExact(entry, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    }
                    if (!(leaving == null))
                    {
                        sql += ", '" + DateTime.ParseExact(leaving, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    }
                    sql += ")";
                    db.execute(sql);
                    id = (long)db.lookup("max(id)", "PERSON", "");
                    long oeId = db.lookup("ID", "ORGENTITY", "PARENT_ID IS NULL", 0L);

                    db.execute("INSERT INTO EMPLOYMENT (TITLE_DE, TITLE_FR, TITLE_EN, TITLE_IT, PERSON_ID, ORGANISATION_ID) VALUES ('" + _map.get("organisation", "employment") + "', '" + _map.get("organisation", "employment") + "', '" + _map.get("organisation", "employment") + "', '" + _map.get("organisation", "employment") + "', " + id + ", " + oeId + ")");
                    if (Global.isModuleEnabled("morph"))
                    {
                        db.execute("INSERT INTO MATRIX_USER_GROUP (OWNER_ID) VALUES(" + id + ")");
                    }
                }
                //update person
                else
                {
                    string sql = "UPDATE PERSON SET PNAME = '" + SQLColumn.toSql(name) + "', FIRSTNAME= '" + SQLColumn.toSql(firstname) + "', MNEMO= '" + SQLColumn.toSql(mnemo) + "', TITLE= '" + SQLColumn.toSql(title) + "', PERSONNELNUMBER= '" + SQLColumn.toSql(personnelnumber) + "', SEX= '" + sex + "', MARTIAL= '" + martial + "', LOGIN = '" + SQLColumn.toSql(login) + "', PASSWORD= '" + SQLColumn.toSql(password) + "', EMAIL= '" + SQLColumn.toSql(email) + "', PHONE= '" + SQLColumn.toSql(phone) + "', MOBILE= '" + SQLColumn.toSql(mobile) + "', PHOTO= '" + SQLColumn.toSql(photo) + "', SALUTATION_ADDRESS= '" + SQLColumn.toSql(salutationaddress) + "', SALUTATION_LETTER= '" + SQLColumn.toSql(salutationletter) + "', BESCH_GRAD= '" + beschgrad + "', BERUFSERFAHRUNG= '" + berufserfahrung + "', LEADERSHIP= '" + leadership + "', COMMENTS = '" + SQLColumn.toSql(comment) + "'";
                    if (!firm_id.Equals("0"))
                    {
                        sql += ", FIRM_ID = '" + firm_id + "'";
                    }
                    if (!(dateofbirth == null))
                    {
                        sql += ", DATEOFBIRTH = '" + DateTime.ParseExact(dateofbirth, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    }
                    if (!(entry == null))
                    {
                        sql += ", ENTRY = '" + DateTime.ParseExact(entry, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    }
                    if (!(leaving == null))
                    {
                        sql += ", LEAVING = '" + DateTime.ParseExact(leaving, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    }
                    else
                    {
                        sql += ", LEAVING = NULL ";
                    }
                    sql += " WHERE ID =" + id;
                    db.execute(sql);

                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "personAddFailed";
                ret.Message = ex.ToString();

            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(id.ToString());
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string deletePerson(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("DELETE FROM PERSON WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "personDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ExistPersonnelnumber(string pNr, long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            if (!Global.isModuleEnabled("morph"))
            {
                {
                    db.connect();
                    try
                    {
                        detailData = db.getDataTable("SELECT ID, PERSONNELNUMBER, PNAME, FIRSTNAME FROM PERSON WHERE PERSONNELNUMBER = '" + pNr.Trim() + "' AND NOT ID = " + id);
                        if (detailData.Rows.Count > 0)
                        {
                            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
                            DataColumn ERROR_MESSAGE = new DataColumn("ERROR_MESSAGE");
                            detailData.Columns.Add(ERROR_MESSAGE);
                            detailData.Rows[0]["ERROR_MESSAGE"] = _map.get("error", "personelnumberDouble").Replace("#1", pNr.Trim()).Replace("#2", detailData.Rows[0]["FIRSTNAME"].ToString() + " " + detailData.Rows[0]["PNAME"].ToString());
                        }


                    }

                    catch (Exception ex)
                    {
                        Logger.Log(ex, Logger.ERROR);
                        ret.shortMessage = "readDataFailed";
                        ret.Message = ex.ToString();
                    }
                    finally
                    {
                        db.disconnect();
                    }
                }
            }
            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string setSubstitute(long personId, long jobId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("UPDATE JOB SET PROXY_PERSON_ID = " + personId + " WHERE ID = " + jobId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "setSubstituteeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            JavaScriptSerializer j = new JavaScriptSerializer();
            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }


        //---------------  Clipboard ------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetClipboardFolderId(long clipboardId, long id, string typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            ServiceResponse response = new ServiceResponse();
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();
            long folderId = 0;
            try
            {
                if (clipboardId == 0)
                {
                    long xid = Convert.ToInt32(db.lookup("UID", typ, "ID=" + id));
                    folderId = db.newId("FOLDER");
                    db.execute("insert into FOLDER (ID, ROOT_ID, TRIGGER_UID, TITLE, NUMOFDOCVERSIONS) values(" + folderId + ", " + folderId + ", " + xid + ", '" + _map.get("CLIPBOARD", "DEFAULTTITLE") + "', " + Global.Config.numberOfDocumentVersions + ")");
                    db.execute("insert into CLIPBOARD (ID, FOLDER_ID, TRIGGER_UID, TITLE, TYP) values(" + folderId + ", " + folderId + ", " + xid + ", '" + _map.get("CLIPBOARD", "DEFAULTTITLE") + "', " + 1 + ")");
                    if (typ.Equals("person"))
                    {
                        db.execute("UPDATE PERSON SET CLIPBOARD_ID = " + folderId + " WHERE ID = " + id);
                    }
                    if (typ.Equals("orgentity"))
                    {
                        db.execute("UPDATE ORGENTITY SET CLIPBOARD_ID = " + folderId + " WHERE ID = " + id);
                    }
                }
                else
                {
                    folderId = db.lookup("FOLDER_ID", "CLIPBOARD", "ID=" + clipboardId, 0L);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readClipboardFailed";
                ret.Message = ex.ToString();
            }

            finally
            {

                db.disconnect();
            }


            return ret.getErrorObj("[{\"folderId\":\"" + folderId.ToString() + "\"}]");

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateFolder(long folderId, string title, string description, string created, int numofdocversions)
        {
            CustomDataErrorObj ret = GetRetObj();
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                if (db.lookup("PARENT_ID", "FOLDER", "ID=" + folderId).ToString().Length == 0)
                {
                    db.execute("UPDATE CLIPBOARD SET TITLE = '" + SQLColumn.toSql(title) + "' WHERE FOLDER_ID =" + folderId);
                }
                string sql = "UPDATE FOLDER SET TITLE = '" + SQLColumn.toSql(title) + "', DESCRIPTION = '" + SQLColumn.toSql(description) + "', NUMOFDOCVERSIONS = " + (numofdocversions - 2);
                if (!(created == "null"))
                {
                    sql += ", CREATED = '" + DateTime.ParseExact(created, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                }
                sql += " WHERE ID = " + folderId;
                db.execute(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveClipboardFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNewFolder(long parentFolderId, long rootId, long id, string typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable newFolderData = new DataTable();
            try
            {
                long folderId = db.newId("FOLDER");
                //long rootId = db.lookup("ROOT_ID", "FOLDER", "ID=" + parentFolderId, 0L);             
                long xid = Convert.ToInt32(db.lookup("UID", typ, "ID=" + id));

                db.execute("INSERT INTO FOLDER (ID, PARENT_ID, ROOT_ID, ORDNUMBER, INHERIT, TITLE, TYP, TRIGGER_UID, NUMOFDOCVERSIONS) VALUES(" + folderId + ", " + parentFolderId + ", " + rootId + ", 1,1,'Neuer Ordner'," + xid + ",0, 2)");
                newFolderData = db.getDataTable("SELECT ID, ROOT_ID, TITLE FROM FOLDER WHERE ID = " + folderId);
                DataColumn imageURL = new DataColumn("IMAGEURL");
                newFolderData.Columns.Add(imageURL);
                newFolderData.Rows[0]["IMAGEURL"] = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/ordner_zu.gif";

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "newClipboardFolderFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj(TableToJson(newFolderData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFolder(long folderId, long id, string typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                if (db.lookup("PARENT_ID", "FOLDER", "ID=" + folderId).ToString().Length == 0)
                {
                    if (typ.Equals("person"))
                    {
                        db.execute("UPDATE PERSON SET CLIPBOARD_ID = null WHERE ID =" + id);
                    }
                    if (typ.Equals("orgentity"))
                    {
                        db.execute("UPDATE ORGENTITY SET CLIPBOARD_ID = null WHERE ID =" + id);
                    }
                    db.execute("DELETE FROM CLIPBOARD WHERE FOLDER_ID = " + folderId);
                }
                db.Folder.delete(folderId, true);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteFolderFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }


            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RadTreeNodeData[] getClipboardTreeData(RadTreeNodeData node, object context)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable nodeTable = new DataTable();
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();

            try
            {
                long folderId;
                if (node.Attributes["IsRoot"].Equals("true"))
                {
                    folderId = db.lookup("FOLDER_ID", "CLIPBOARD", "ID=" + node.Value, 0L);
                }
                else
                {
                    folderId = Convert.ToInt32(node.Value);
                }
                nodeTable = db.getDataTable("SELECT ID, TITLE FROM FOLDER WHERE PARENT_ID=" + folderId);

                foreach (DataRow treeNode in nodeTable.Rows)
                {
                    RadTreeNodeData nodeData = new RadTreeNodeData();
                    nodeData.Text = treeNode["TITLE"].ToString();
                    nodeData.Value = treeNode["ID"].ToString();
                    nodeData.Attributes.Add("IsRoot", "false");
                    string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                    nodeData.ImageUrl = imageUrl + "ordner_zu.gif";
                    if ((int)db.lookup("COUNT(ID)", "FOLDER", "PARENT_ID=" + treeNode["ID"].ToString()) > 0)
                    {
                        nodeData.ExpandMode = TreeNodeExpandMode.WebService;
                    }

                    result.Add(nodeData);
                }


            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readClipboardFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return result.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFolderData(long FolderId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable folderTable = new DataTable();
            try
            {
                folderTable = db.getDataTable("SELECT TITLE, DESCRIPTION, CREATED,  NUMOFDOCVERSIONS + 2 AS NUMOFDOCVERSIONS FROM FOLDER WHERE ID=" + FolderId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readClipboardFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(folderTable));
        }

        //------------ Permissions -------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAccessors(long id, string typ)
        {
            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            CustomDataErrorObj ret = GetRetObj();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable accessors = new DataTable();
            try
            {
                accessors = db.getDataTable("SELECT ID, TABLENAME, TITLE_" + lang + " AS TITLE, VISIBLE, ROW_ID FROM ACCESSORV WHERE ID in (SELECT DISTINCT ACCESSOR_ID from ACCESS_RIGHT_RT where TABLENAME='" + typ + "' and (ROW_ID=" + id + " OR ROW_ID=0))ORDER BY TITLE_" + lang);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                DataColumn image = new DataColumn("IMAGE");
                accessors.Columns.Add(image);
                string pic = "";
                foreach (DataRow accessor in accessors.Rows)
                {
                    switch (accessor["TABLENAME"].ToString())
                    {
                        case "ACCESSORGROUP":
                            pic = "bp_personengruppe.gif";
                            break;
                        case "PERSON":
                            pic = "bp_person.gif";
                            break;
                        case "ORGENTITY":
                            pic = "og_abteilung.gif";
                            break;
                    }
                    accessor["IMAGE"] = imageUrl + pic;
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readClipboardFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(accessors));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetModule(string typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            List<module> moduleList = new List<module>();
            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            if (typ == "ORGENTITY" || typ == "JOB")
            {
                foreach (psoftModule module in Global.s_ModulesDict.Values)
                {
                    switch (module.Name)
                    {
                        case "skills":
                            module modul = new module();
                            modul.modulname = _map.get("module", "skills");
                            modul.modulvalue = 31;
                            moduleList.Add(modul);
                            break;
                        case "mbo":
                            module modul1 = new module();
                            modul1.modulname = _map.get("module", "mbo");
                            modul1.modulvalue = 61;
                            moduleList.Add(modul1);
                            break;
                        case "fbs":
                            module modul2 = new module();
                            modul2.modulname = _map.get("module", "fbs");
                            modul2.modulvalue = 21;
                            moduleList.Add(modul2);
                            break;
                        case "training":
                            module modul3 = new module();
                            modul3.modulname = _map.get("module", "training");
                            modul3.modulvalue = 41;
                            moduleList.Add(modul3);
                            break;
                        case "FBW":
                            module modul4 = new module();
                            modul4.modulname = _map.get("module", "fbw");
                            modul4.modulvalue = 51;
                            moduleList.Add(modul4);
                            break;
                        case "performance":
                            module modul5 = new module();
                            modul5.modulname = _map.get("module", "performance");
                            modul5.modulvalue = 11;
                            moduleList.Add(modul5);
                            module modul5a = new module();
                            modul5a.modulname = _map.get("module", "jobexpectation");
                            modul5a.modulvalue = 12;
                            moduleList.Add(modul5a);
                            break;
                        case "lohn":
                            module modul6 = new module();
                            modul6.modulname = _map.get("module", "lohn");
                            modul6.modulvalue = 110;
                            moduleList.Add(modul6);
                            break;
                    }
                }
            }

            if (typ == "RELEASE")
            {
                foreach (psoftModule module in Global.s_ModulesDict.Values)
                {
                    switch (module.Name)
                    {
                        case "skills":
                            module modul = new module();
                            modul.modulname = _map.get("module", "modulskills");
                            modul.modulvalue = 30;
                            moduleList.Add(modul);
                            break;
                        case "mbo":
                            module modul1 = new module();
                            modul1.modulname = _map.get("module", "modulmbo");
                            modul1.modulvalue = 60;
                            moduleList.Add(modul1);
                            break;
                        case "fbs":
                            module modul2 = new module();
                            modul2.modulname = _map.get("module", "modulfbs");
                            modul2.modulvalue = 20;
                            moduleList.Add(modul2);
                            break;
                        case "training":
                            module modul3 = new module();
                            modul3.modulname = _map.get("module", "modultraining");
                            modul3.modulvalue = 40;
                            moduleList.Add(modul3);
                            break;
                        case "FBW":
                            module modul4 = new module();
                            modul4.modulname = _map.get("module", "modulfbw");
                            modul4.modulvalue = 50;
                            moduleList.Add(modul4);
                            break;
                        case "performance":
                            module modul5 = new module();
                            modul5.modulname = _map.get("module", "modulperformance");
                            modul5.modulvalue = 10;
                            moduleList.Add(modul5);
                            break;
                        case "lohn":
                            module modul6 = new module();
                            modul6.modulname = _map.get("module", "modullohn");
                            modul6.modulvalue = 110;
                            moduleList.Add(modul6);
                            break;
                        case "administration":
                            module modul7 = new module();
                            modul7.modulname = _map.get("module", "moduladministration");
                            modul7.modulvalue = 1;
                            moduleList.Add(modul7);
                            break;
                    }
                }
            }

            if (typ == "MENUITEM" || typ == "MENUGROUP")
            {
                module modul = new module();
                modul.modulname = _map.get("module", "Generally");
                modul.modulvalue = 0;
                moduleList.Add(modul);
            }

            if (typ == "FOLDER")
            {
                module modul = new module();
                modul.modulname = _map.get("module", "Generally");
                modul.modulvalue = 0;
                moduleList.Add(modul);
            }


            List<module> SortedList = moduleList.OrderBy(o => o.modulname).ToList();

            return ret.getErrorObj(new JavaScriptSerializer().Serialize(SortedList));
        }

        private class module
        {
            public string modulname { get; set; }
            public int modulvalue { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPermissions(long accessorId, long modulId, long id, string typ, bool inherited)
        {
            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable accessorPermissions = new DataTable();
            Dictionary<string, bool> permissionDetail = new Dictionary<string, bool>();
            int permission = 0;
            try
            {
                string accessors = db.getAccessorIDsSQLInClause(accessorId);
                if (inherited)
                {
                    accessorPermissions = db.getDataTable("SELECT DISTINCT AUTHORISATION FROM ACCESS_RIGHT_RT where (ROW_ID = 0 or ROW_ID =" + id + ") AND TABLENAME = '" + typ + "' and APPLICATION_RIGHT = " + modulId + " AND ACCESSOR_ID  " + accessors);
                }
                else
                {
                    accessorPermissions = db.getDataTable("SELECT DISTINCT AUTHORISATION FROM ACCESS_RIGHT_RT where (ROW_ID = 0 or ROW_ID =" + id + ") AND TABLENAME = '" + typ + "' and APPLICATION_RIGHT = " + modulId + " AND ACCESSOR_ID  =" + accessorId);
                }
                if (accessorPermissions.Rows.Count > 0)
                {
                    permission = (int)accessorPermissions.Rows[0][0];
                }

                permissionDetail = GetPermissionDetail(permission);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readPermissionsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(new JavaScriptSerializer().Serialize(permissionDetail));
        }

        private Dictionary<string, bool> GetPermissionDetail(int permission)
        {
            Dictionary<string, bool> permissionDetail = new Dictionary<string, bool>();

            if ((permission & DBData.AUTHORISATION.READ) > 0)
            {
                permissionDetail.Add("read", true);
            }
            else
            {
                permissionDetail.Add("read", false);
            }
            if ((permission & DBData.AUTHORISATION.INSERT) > 0)
            {
                permissionDetail.Add("insert", true);
            }
            else
            {
                permissionDetail.Add("insert", false);
            }
            if ((permission & DBData.AUTHORISATION.UPDATE) > 0)
            {
                permissionDetail.Add("update", true);
            }
            else
            {
                permissionDetail.Add("update", false);
            }
            if ((permission & DBData.AUTHORISATION.DELETE) > 0)
            {
                permissionDetail.Add("delete", true);
            }
            else
            {
                permissionDetail.Add("delete", false);
            }
            if ((permission & DBData.AUTHORISATION.ADMIN) > 0)
            {
                permissionDetail.Add("admin", true);
            }
            else
            {
                permissionDetail.Add("admin", false);
            }
            if ((permission & DBData.AUTHORISATION.EXECUTE) > 0)
            {
                permissionDetail.Add("execute", true);
            }
            else
            {
                permissionDetail.Add("execute", false);
            }
            return permissionDetail;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePermission(long id, long accessorId, long modulId, int permissions, string typ, bool inherited)
        {
            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                DataTable accessorPermissions = new DataTable();
                int permissionOld = 0;
                string accessors = db.getAccessorIDsSQLInClause(accessorId);
                if (inherited)
                {
                    accessorPermissions = db.getDataTable("SELECT DISTINCT AUTHORISATION FROM ACCESS_RIGHT_RT where (ROW_ID = 0 or ROW_ID =" + id + ") AND TABLENAME = '" + typ + "' and APPLICATION_RIGHT = " + modulId + " AND ACCESSOR_ID  " + accessors);
                }
                else
                {
                    accessorPermissions = db.getDataTable("SELECT DISTINCT AUTHORISATION FROM ACCESS_RIGHT_RT where (ROW_ID = 0 or ROW_ID =" + id + ") AND TABLENAME = '" + typ + "' and APPLICATION_RIGHT = " + modulId + " AND ACCESSOR_ID  =" + accessorId);
                }
                if (accessorPermissions.Rows.Count > 0)
                {
                    permissionOld = (int)accessorPermissions.Rows[0][0];
                }

                if (permissionOld > 0 && (permissionOld - permissions) > 0)
                {
                    db.execute("EXEC DROP_ACCESS_RIGHT_ROW " + accessorId + ", " + (permissionOld - permissions) + ", '" + typ + "', " + id + ", " + modulId);
                }

                db.execute("EXEC SET_ACCESS_RIGHT_ROW " + accessorId + ", " + permissions + ", '" + typ + "', " + id + ", " + modulId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "savePermissionsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPermissionInherited(long id, string typ)
        {
            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable inherited = new DataTable();
            db.connect();
            try
            {
                inherited = db.getDataTable("SELECT INHERIT FROM " + typ + " WHERE ID = " + id);
                if (inherited.Rows.Count == 0)
                {
                    inherited.Rows.Add(0);
                }
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readPermissionsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(inherited));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePermissionInherited(int inherited, long id, string typ)
        {
            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("UPDATE " + typ + " SET INHERIT =  " + inherited + " WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readPermissionsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteAccessor(long id, long accessorId, string typ, string[] modules)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            if (typ.Equals("FIRM"))
            {
                typ = "ORGENTITY";
            }
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                if (!typ.Equals("RELEASE"))
                {
                    db.execute("EXEC DROP_ACCESS_RIGHT_ROW " + accessorId + ", 126, '" + typ + "', " + id + ", 0");
                    foreach (string modul in modules)
                    {
                        db.execute("EXEC DROP_ACCESS_RIGHT_ROW " + accessorId + ", 126, '" + typ + "', " + id + ", " + js.Deserialize<ActiveModule>(modul).value);
                    }
                }
                else
                {
                    db.execute("DROP_ACCESS_RIGHT_APPLICATION " + accessorId + ", 126, 1");
                    foreach (string modul in modules)
                    {
                        db.execute("DROP_ACCESS_RIGHT_APPLICATION " + accessorId + ", 126," + js.Deserialize<ActiveModule>(modul).value);
                        //db.execute("EXEC DROP_ACCESS_RIGHT_TABLE " + accessorId + ", 126, '" + typ + "', " + id + ", " + js.Deserialize<ActiveModule>(modul).value);
                    }

                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readPermissionsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        private class ActiveModule
        {
            public int value { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddAccessorApplication(long accessorId, int applicationright, int permission, string tablename)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                if (tablename.Length == 0)
                {
                    db.execute("EXEC SET_ACCESS_RIGHT_APPLICATION " + accessorId + ", " + permission + ", " + applicationright);
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "savePermissionsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAccessorList(long id, string typ)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable accessors = new DataTable();
            db.connect();
            try
            {
                accessors = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, TABLENAME from ACCESSORV WHERE ID NOT IN (SELECT ACCESSOR_ID FROM ACCESS_RIGHT_RT WHERE TABLENAME='" + typ + "' and (ROW_ID=" + id + " OR ROW_ID=0)) order by TITLE_" + lang);

                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                DataColumn image = new DataColumn("IMAGE");
                accessors.Columns.Add(image);
                string pic = "";
                foreach (DataRow accessor in accessors.Rows)
                {
                    switch (accessor["TABLENAME"].ToString())
                    {
                        case "ACCESSORGROUP":
                            pic = "bp_personengruppe.gif";
                            break;
                        case "PERSON":
                            pic = "bp_person.gif";
                            break;
                        case "ORGENTITY":
                            pic = "og_abteilung.gif";
                            break;
                    }
                    accessor["IMAGE"] = imageUrl + pic;
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readAccessorsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(accessors));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAccessorListEditable(int typ, string accessor, int editable)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable accessors = new DataTable();
            db.connect();
            try
            {
                string sql = "SELECT ID, TITLE_" + lang + " AS TITLE, TABLENAME, VISIBLE FROM ACCESSORV";
                if (typ > 0 || accessor.Length > 0 || editable > 0)
                {
                    sql += " WHERE";
                    if (typ > 0)
                    {
                        switch (typ)
                        {
                            case 1:
                                sql += " TABLENAME = 'ACCESSORGROUP'";
                                break;
                            case 2:
                                sql += " TABLENAME = 'PERSON'";
                                break;
                            case 3:
                                sql += " TABLENAME = 'ORGENTITY'";
                                break;
                        }
                        if (accessor.Length > 0 || editable > 0)
                        {
                            sql += " AND";
                        }
                    }
                    if (accessor.Length > 0)
                    {
                        sql += " TITLE_" + lang + " LIKE('%" + accessor + "%')";
                        if (editable > 0)
                        {
                            sql += " AND";
                        }
                    }
                    if (editable > 0)
                    {
                        int visible = 0;
                        if (editable == 1)
                        {
                            visible = 1;
                        }
                        if (editable == 2)
                        {
                            visible = 0;
                        }
                        sql += " VISIBLE = " + visible;
                    }

                }
                sql += " ORDER BY TITLE_" + lang;

                accessors = db.getDataTable(sql);

                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                DataColumn image = new DataColumn("IMAGE");
                accessors.Columns.Add(image);
                string pic = "";
                foreach (DataRow accessorRow in accessors.Rows)
                {
                    switch (accessorRow["TABLENAME"].ToString())
                    {
                        case "ACCESSORGROUP":
                            pic = "bp_personengruppe.gif";
                            break;
                        case "PERSON":
                            pic = "bp_person.gif";
                            break;
                        case "ORGENTITY":
                            pic = "og_abteilung.gif";
                            break;
                    }
                    accessorRow["IMAGE"] = imageUrl + pic;
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readAccessorsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(accessors));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAccessorMemberOf(long accessorId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable accessors = new DataTable();
            db.connect();
            try
            {
                accessors = db.getDataTable("SELECT ID, TABLENAME, TITLE_" + lang + " AS TITLE, VISIBLE, ACCESSOR_GROUP_ID FROM ACCESSORV INNER JOIN ACCESSOR_GROUP_ASSIGNMENT ON ACCESSOR_GROUP_ID = ID and ACCESSOR_MEMBER_ID = " + accessorId + " ORDER BY TITLE_" + lang);

                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                DataColumn image = new DataColumn("IMAGE");
                accessors.Columns.Add(image);
                string pic = "";
                foreach (DataRow accessorRow in accessors.Rows)
                {
                    switch (accessorRow["TABLENAME"].ToString())
                    {
                        case "ACCESSORGROUP":
                            pic = "bp_personengruppe.gif";
                            break;
                        case "PERSON":
                            pic = "bp_person.gif";
                            break;
                        case "ORGENTITY":
                            pic = "og_abteilung.gif";
                            break;
                    }
                    accessorRow["IMAGE"] = imageUrl + pic;
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readAccessorsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(accessors));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAccessorGroupMember(long accessorId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable accessors = new DataTable();
            db.connect();
            try
            {
                accessors = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, VISIBLE, ACCESSOR_GROUP_ID, TABLENAME FROM ACCESSORV INNER JOIN ACCESSOR_GROUP_ASSIGNMENT ON ACCESSOR_MEMBER_ID=ID and ACCESSOR_GROUP_ID= " + accessorId + " ORDER BY TITLE_" + lang);

                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                DataColumn image = new DataColumn("IMAGE");
                accessors.Columns.Add(image);
                string pic = "";
                foreach (DataRow accessorRow in accessors.Rows)
                {
                    switch (accessorRow["TABLENAME"].ToString())
                    {
                        case "ACCESSORGROUP":
                            pic = "bp_personengruppe.gif";
                            break;
                        case "PERSON":
                            pic = "bp_person.gif";
                            break;
                        case "ORGENTITY":
                            pic = "og_abteilung.gif";
                            break;
                    }
                    accessorRow["IMAGE"] = imageUrl + pic;
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readAccessorsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(accessors));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAccessorDetail(long accessorId, string typ)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable data = new DataTable();
            db.connect();
            try
            {
                if (typ.Equals("PERSON"))
                {
                    data = db.getDataTable("SELECT PNAME + ' ' + FIRSTNAME AS ACCESSOR FROM PERSON WHERE ID = " + accessorId);
                }
                if (typ.Equals("ORGENTITY"))
                {
                    data = db.getDataTable("select TITLE AS TITLE_ACCESSOR from ACCESSOR where ID=" + accessorId);
                    DataColumn oe = new DataColumn("OE_TITLE");
                    data.Columns.Add(oe);
                    string rowId = db.lookup("ROW_ID", "ACCESSORV", "ID=" + accessorId).ToString();
                    string organisation = db.lookup("ORGANISATION.TITLE_" + lang, "ORGANISATION inner join ORGENTITY ON ORGENTITY.ROOT_ID = ORGANISATION.ORGENTITY_ID", "ORGENTITY.ID = " + rowId).ToString();
                    string oeTitle = db.lookup("TITLE_" + lang, "ORGENTITY", "ID=" + rowId).ToString();
                    data.Rows[0]["OE_TITLE"] = oeTitle + "(" + organisation + ")";
                }
                if (typ.Equals("ACCESSORGROUP"))
                {
                    data = db.getDataTable("select TITLE_" + lang + " AS GROUP_NAME from ACCESSORV where ID=" + accessorId);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readAccessorsFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(data));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddAccessorToGroup(long accesorId, long accessorGroupId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("insert into ACCESSOR_GROUP_ASSIGNMENT (ACCESSOR_MEMBER_ID, ACCESSOR_GROUP_ID) values (" + accesorId + ", " + accessorGroupId + ")");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "addAccessorsToGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddAccessorGroup(string groupTitle)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable data = new DataTable();
            db.connect();
            try
            {
                long newid = db.newId("ACCESSOR");
                db.execute("insert into ACCESSOR (TITLE,ID) values ('" + SQLColumn.toSql(groupTitle) + "'," + newid + ")");
                data = db.getDataTable("SELECT ID, TITLE FROM ACCESSOR WHERE ID =" + newid);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/bp_personengruppe.gif";
                DataColumn image = new DataColumn("IMAGE");
                data.Columns.Add(image);
                data.Rows[0]["image"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "addAccessorsToGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(data));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RenameAccessorGroup(string newName, long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("UPDATE ACCESSOR SET TITLE='" + SQLColumn.toSql(newName) + "' WHERE ID=" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "renameAccessorgroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteAccessorGroup(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("DELETE FROM ACCESSOR WHERE id=" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteAccessorsGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteAccessorFromGroup(long accesorId, long accessorGroupId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                db.execute("delete from ACCESSOR_GROUP_ASSIGNMENT where ACCESSOR_MEMBER_ID = " + accesorId + " and ACCESSOR_GROUP_ID = " + accessorGroupId);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteAccessorsFromGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"ERROR\":\"ok\"}]");
        }


        //------- FUNCTION --------------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RadTreeNodeData[] getFunctionTreeData(RadTreeNodeData node, object context)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable nodeTable = new DataTable();
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;

            try
            {
                nodeTable = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, FUNKTION_GROUP_ID  FROM FUNKTION ORDER BY TITLE_" + lang);

                foreach (DataRow treeNode in nodeTable.Rows)
                {
                    RadTreeNodeData nodeData = new RadTreeNodeData();
                    nodeData.Text = treeNode["TITLE"].ToString();
                    nodeData.Value = treeNode["ID"].ToString();
                    nodeData.Attributes.Add("TYP", "FUNCTION");
                    string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                    nodeData.ImageUrl = imageUrl + "Images/fx_funktion.gif";
                    result.Add(nodeData);
                }


            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionTreeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return result.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String GetFunctionSearchList(string name, string nameShort)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable functions = new DataTable();
            db.connect();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion.gif";
                string sql = "SELECT ID, TITLE_" + lang + " AS TITLE, FUNKTION_GROUP_ID, 'FUNCTION' AS TYP,'" + imageUrl + "' AS IMAGE   FROM FUNKTION ";
                if (name.Length > 0 || nameShort.Length > 0)
                {
                    sql += "WHERE ";
                    if (name.Length > 0)
                    {
                        sql += "TITLE_" + lang + " LIKE'%" + name + "%' ";
                    }
                    if (name.Length > 0 && nameShort.Length > 0)
                    {
                        sql += "AND ";
                    }
                    if (nameShort.Length > 0)
                    {
                        sql += "MNEMONIC_" + lang + " LIKE'%" + nameShort + "%' ";
                    }

                }

                sql += "ORDER BY TITLE_" + lang;
                functions = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionListFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(functions));

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionOrGroupDetailData(long id, string typ)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                string sql = "";
                if (typ == "FUNCTION")
                {
                    sql = "SELECT FUNKTION.TITLE_" + lang + " AS TITLE, FUNKTION.MNEMONIC_" + lang + " AS TITLESHORT, FUNKTION.DESCRIPTION_" + lang + " AS DESCRIPTION, FUNKTION.DFLT, FUNKTION.FUNKTION_TYP_ID, FUNKTION.FBW_REVISION, FUNKTION.BONUSLEVEL, FUNKTION_GROUP.TITLE_" + lang + " AS GROUP_NAME "
                         + "FROM FUNKTION INNER JOIN FUNKTION_GROUP ON FUNKTION.FUNKTION_GROUP_ID = FUNKTION_GROUP.ID WHERE FUNKTION.ID=" + id;
                }
                if (typ == "GROUP")
                {
                    sql = "SELECT FUNKTION_GROUP.TITLE_" + lang + " AS TITLE, FUNKTION_GROUP.DESCRIPTION_" + lang + " AS DESCRIPTION, FUNKTION_GROUP_1.TITLE_" + lang + " AS TITLEPARENT "
                         + "FROM FUNKTION_GROUP LEFT OUTER JOIN FUNKTION_GROUP AS FUNKTION_GROUP_1 ON FUNKTION_GROUP.PARENT_ID = FUNKTION_GROUP_1.ID WHERE FUNKTION_GROUP.ID=" + id;
                }

                detailData = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionOrGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveFunction(long id, string typ, string name, string nameShort, string description, bool dflt, long functiontyp, long functionGroupId, string fbwRevision, float bonusPart)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                string sql = "";
                string revision = "";
                if (fbwRevision != null)
                {
                    revision = DateTime.ParseExact(fbwRevision, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy");
                }
                else
                {
                    revision = DateTime.Now.ToString("MM-dd-yyyy");
                }

                int dflt_t = (Convert.ToInt16(dflt));
                string functiontyp_t = "";
                if (functiontyp == 0)
                {
                    functiontyp_t = "null";
                }
                else
                {
                    functiontyp_t = functiontyp.ToString();
                }

                if (id == 0)
                {
                    id = db.newId("FUNKTION");
                    sql = "INSERT INTO FUNKTION (ID, FUNKTION_GROUP_ID, TITLE_" + lang + ", MNEMONIC_" + lang + ", DESCRIPTION_" + lang + ", DFLT, FUNKTION_TYP_ID, FBW_REVISION,  BONUSLEVEL) VALUES(" + id + ", " + functionGroupId + ", '" + SQLColumn.toSql(name) + "', '" + SQLColumn.toSql(nameShort) + "', '" + SQLColumn.toSql(description) + "', " + dflt_t + ", " + functiontyp_t + ", '" + revision + "', " + bonusPart + ")";
                }
                else
                {
                    sql = "UPDATE FUNKTION SET FUNKTION_GROUP_ID = " + functionGroupId + ", TITLE_" + lang + " = '" + SQLColumn.toSql(name) + "', MNEMONIC_" + lang + " = '" + SQLColumn.toSql(nameShort) + "', DESCRIPTION_" + lang + " = '" + SQLColumn.toSql(description) + "',  DFLT = " + dflt_t + ", FUNKTION_TYP_ID = " + functiontyp_t + ",  FBW_REVISION =  '" + revision + "', BONUSLEVEL = " + bonusPart + " WHERE ID = " + id;
                }


                db.execute(sql);

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM FUNKTION WHERE ID = " + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveFunctionFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveFunctionGroup(long id, long functionGroupId, string typ, string name, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long rootId = db.lookup("ID", "FUNKTION_GROUP", "PARENT_ID IS NULL", 0L);
                if (id == 0)
                {
                    id = db.newId("FUNKTION_GROUP");
                    db.execute("INSERT INTO FUNKTION_GROUP (ID, PARENT_ID, ROOT_ID, TITLE_" + lang + ", DESCRIPTION_" + lang + ") VALUES(" + id + ", " + functionGroupId + ", " + rootId + ", '" + SQLColumn.toSql(name) + "', '" + SQLColumn.toSql(description) + "')");
                }
                else
                {
                    db.execute("UPDATE FUNKTION_GROUP SET TITLE_" + lang + " = '" + SQLColumn.toSql(name) + "', DESCRIPTION_" + lang + "= '" + SQLColumn.toSql(description) + "' WHERE ID = " + id);
                }
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM FUNKTION_GROUP WHERE ID = " + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktionsgruppe.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveFunctionGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFunction(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM JOB_HISTORY WHERE FUNKTION_ID = " + id);
                db.execute("DELETE FROM FUNKTION WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "DeleteFunctionFailed";

                if (Convert.ToInt32(ex.Data["HelpLink.EvtID"]) == 547)
                {
                    LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
                    ret.shortMessage = "FunctionJobReferencesError";
                }
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFunctionGroup(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID FROM FUNKTION WHERE FUNKTION_GROUP_ID = " + id);
                foreach (DataRow row in detailData.Rows)
                {
                    db.execute("DELETE FROM JOB_HISTORY WHERE FUNKTION_ID = " + row["ID"]);
                    db.execute("DELETE FROM FUNKTION WHERE ID = " + row["ID"]);
                }

                db.execute("DELETE FROM FUNKTION_GROUP WHERE ID = " + id);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "DeleteFunctionGroupFailed";
                if (Convert.ToInt32(ex.Data["HelpLink.EvtID"]) == 547)
                {
                    LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
                    ret.shortMessage = "FunctionGroupJobReferencesError";
                }
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveNodeInTree(long sourceId, string sourceTyp, long destId, string destTyp)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (sourceTyp == "GROUP")
                {
                    db.execute("UPDATE FUNKTION_GROUP SET PARENT_ID = " + destId + " WHERE ID = " + sourceId);
                }
                else
                {
                    db.execute("UPDATE FUNKTION SET FUNKTION_GROUP_ID = " + destId + " WHERE ID = " + sourceId);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveTreeElementFailet";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Menue --------------------------------------
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string GetMenuGroup(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, MNEMO FROM MENUGROUP WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readMenuGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string GetMenuItem(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, LINK, TARGET FROM MENUITEM WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readMenuItemFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveMenugroup(long id, long parentId, string name, string nameShort, int ordNumber)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (id == 0)
                {
                    id = db.newId("MENUGROUP");
                    long rootID = (long)db.lookup("ROOT_ID", "MENUGROUP", "ID = " + parentId);
                    if (db.lookup("MAX(ORDNUMBER)", "MENUGROUP", "PARENT_ID = " + parentId) != System.DBNull.Value)
                    {
                        ordNumber = Convert.ToInt16(db.lookup("MAX(ORDNUMBER)", "MENUGROUP", "PARENT_ID = " + parentId)) + 1;
                    }
                    else
                    {
                        ordNumber = 0;
                    }
                    db.execute("INSERT INTO MENUGROUP (ID, PARENT_ID, ROOT_ID, ORDNUMBER, MNEMO, TITLE_" + lang + ") VALUES (" + id + ", " + parentId + ", " + rootID + ", " + ordNumber + ", '" + SQLColumn.toSql(nameShort) + "', '" + SQLColumn.toSql(name) + "')");
                }

                else
                {
                    db.execute("UPDATE MENUGROUP SET PARENT_ID = " + parentId + ", ORDNUMBER = " + ordNumber + ", MNEMO = '" + SQLColumn.toSql(nameShort) + "', TITLE_" + lang + "= '" + SQLColumn.toSql(name) + "' WHERE ID = " + id);
                }
                detailData = db.getDataTable("SELECT ID, PARENT_ID, ORDNUMBER, MNEMO, TITLE_" + lang + " AS TITLE FROM  MENUGROUP WHERE ID =" + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/m_menuegruppe.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveMenuGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveMenuItem(long id, long parentId, string name, string link, string target, int ordNumber)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (id == 0)
                {
                    id = db.newId("MENUITEM");
                    if (db.lookup("MAX(ORDNUMBER)", "MENUITEM", "MENUGROUP_ID = " + parentId) != System.DBNull.Value)
                    {
                        ordNumber = Convert.ToInt16(db.lookup("MAX(ORDNUMBER)", "MENUITEM", "MENUGROUP_ID = " + parentId)) + 1;
                    }
                    else
                    {
                        ordNumber = 0;
                    }
                    db.execute("INSERT INTO MENUITEM (ID, MENUGROUP_ID, ORDNUMBER, LINK, TARGET, TITLE_" + lang + ") VALUES (" + id + ", " + parentId + ", " + ordNumber + ", '" + SQLColumn.toSql(link) + "', '" + SQLColumn.toSql(target) + "', '" + SQLColumn.toSql(name) + "')");
                }
                else
                {
                    db.execute("UPDATE MENUITEM SET MENUGROUP_ID = " + parentId + ", ORDNUMBER = " + ordNumber + ", LINK = '" + SQLColumn.toSql(link) + "', TARGET = '" + SQLColumn.toSql(target) + "', TITLE_" + lang + "= '" + SQLColumn.toSql(name) + "' WHERE ID = " + id);
                }

                detailData = db.getDataTable("SELECT ID, MENUGROUP_ID, ORDNUMBER, LINK, TARGET, TITLE_" + lang + " AS TITLE FROM  MENUITEM WHERE ID =" + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/m_menue.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveMenuItemFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteMenuItem(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM MENUITEM WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteMenuItemFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteMenuGroup(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM MENUITEM WHERE MENUGROUP_ID = " + id);
                db.execute("DELETE FROM MENUGROUP WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteMenuGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSearchlistItems(int typ, string filter)
        {    //Typ
             //"0" Report "1" Organigramm "2" Karte "3" Dokumentablage "4" Wissenselement
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                string sql = "";
                switch (typ)
                {
                    case 0:
                        sql = "SELECT UID, ALIAS AS TITLE FROM UID_ALIAS WHERE ALIAS LIKE '%" + filter + "%' ORDER BY ALIAS ";
                        break;
                    case 1:
                        sql = "SELECT UID, TITLE_" + lang + " AS TITLE FROM CHART WHERE TITLE_" + lang + " LIKE '%" + filter + "%' ORDER BY TITLE_" + lang;
                        break;
                    case 2:
                        sql = "SELECT UID, TITLE FROM MATRIX WHERE TITLE LIKE '%" + filter + "%' ORDER BY TITLE";
                        break;
                    case 3:
                        sql = "SELECT UID, TITLE FROM CLIPBOARD WHERE TITLE LIKE '%" + filter + "%' ORDER BY TITLE";
                        break;
                    case 4:
                        sql = "SELECT UID, TITLE FROM THEME WHERE TITLE LIKE '%" + filter + "%' AND PARENT_ID IS NULL ORDER BY TITLE";
                        break;
                }

                detailData = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "lodSearchListFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveMenuEntryToGroup(long sourceId, long destId, string typSource)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (typSource == "MENUITEM")
                {
                    double maxOrdNumber = db.lookup("max(ordnumber)", "MENUITEM", "MENUGROUP_ID =" + destId, 0D);
                    db.execute("UPDATE MENUITEM SET ORDNUMBER = " + maxOrdNumber + 1 + ", MENUGROUP_ID = " + destId + " WHERE ID = " + sourceId);
                    detailData = db.getDataTable("SELECT ID, ORDNUMBER FROM MENUITEM WHERE ID = " + sourceId);
                }
                if (typSource == "MENUGROUP")
                {
                    double maxOrdNumber = db.lookup("max(ordnumber)", "MENUGROUP", "ID =" + destId, 0D);
                    db.execute("UPDATE MENUGROUP SET ORDNUMBER = " + maxOrdNumber + 1 + ", PARENT_ID = " + destId + " WHERE ID = " + sourceId);
                    detailData = db.getDataTable("SELECT ID, ORDNUMBER FROM MENUGROUP WHERE ID = " + sourceId);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteMenuGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveMenuItem(long sourceId, long destId, string dropPosition)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                double sourceOrdNumber = db.lookup("ordnumber", "MENUITEM", "ID =" + sourceId, 0D);
                double destOrdNumber = db.lookup("ordnumber", "MENUITEM", "ID =" + destId, 0D);
                long menuGroupId = db.lookup("MENUGROUP_ID", "MENUITEM", "ID =" + destId, 0L);
                if (dropPosition == "above")
                {
                    db.execute("UPDATE MENUITEM SET ORDNUMBER = ORDNUMBER +1 WHERE ORDNUMBER >=" + destOrdNumber + " AND ORDNUMBER <=" + sourceOrdNumber + " AND MENUGROUP_ID = " + menuGroupId);
                    db.execute("UPDATE MENUITEM SET ORDNUMBER = " + (destOrdNumber).ToString() + " WHERE ID = " + sourceId);
                }
                if (dropPosition == "below")
                {
                    db.execute("UPDATE MENUITEM SET ORDNUMBER = ORDNUMBER -1 WHERE ORDNUMBER < =" + destOrdNumber + " AND ORDNUMBER > " + sourceOrdNumber + " AND MENUGROUP_ID = " + menuGroupId);
                    db.execute("UPDATE MENUITEM SET ORDNUMBER = " + (destOrdNumber) + " WHERE ID = " + sourceId);
                }
                detailData = db.getDataTable("SELECT ID, ORDNUMBER FROM MENUITEM WHERE ID = " + sourceId);


            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteMenuGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }




        //-------------------Address ------------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetAddress(long personId)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long addressId = db.lookup("ID", "ADDRESS", "PERSON_ID =" + personId, 0L);
                if (addressId == 0)
                {
                    addressId = db.newId("ADDRESS");
                    db.execute("INSERT INTO ADDRESS (ID, PERSON_ID, COUNTRY, TYP) VALUES (" + addressId + ", " + personId + ", 'CH', 0)");
                }

                detailData = db.getDataTable("SELECT ID, ADDRESS1, ADDRESS2, ADDRESS3, ZIP, CITY, COUNTRY, PHONE, FAX, MOBILE, EMAIL_PRIVATE FROM ADDRESS WHERE ID =" + addressId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readAddessFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveAddress(long personId, string address1, string address2, string address3, string zip, string city, int country, string phone, string fax, string mobil, string email)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                string countryCode = _map.getEnum("ADDRESS", "COUNTRYLIST")[0, country];

                long addressId = db.lookup("ID", "ADDRESS", "PERSON_ID =" + personId, 0L);
                db.execute("UPDATE ADDRESS SET ADDRESS1='" + SQLColumn.toSql(address1) + "' , ADDRESS2 = '" + SQLColumn.toSql(address2) + "' , ADDRESS3 ='" + SQLColumn.toSql(address3) + "' , ZIP ='" + SQLColumn.toSql(zip) + "' , CITY ='" + SQLColumn.toSql(city) + "' , COUNTRY ='" + countryCode + "' , PHONE ='" + SQLColumn.toSql(phone) + "' , FAX ='" + SQLColumn.toSql(fax) + "' , MOBILE ='" + SQLColumn.toSql(mobil) + "' , EMAIL_PRIVATE ='" + SQLColumn.toSql(email) + "'  WHERE ID =" + addressId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveAddessFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteAddress(long personId)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long addressId = db.lookup("ID", "ADDRESS", "PERSON_ID =" + personId, 0L);
                db.execute("DELETE ADDRESS WHERE ID =" + addressId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteAddessFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Functiondescription ------------------------
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getDutyTreeData(long sourceNodeId, Dictionary<string, string> attributes)
        {
            CustomDataErrorObj ret = GetRetObj();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable nodeTable = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                nodeTable = db.getDataTable("SELECT ID, ORDNUMBER, NUMBER + ' ' + TITLE_" + lang + " AS TITLE, 'DUTY' AS TYP FROM DUTYV WHERE  DUTYGROUP_ID = " + sourceNodeId + " ORDER BY ORDNUMBER");
                foreach (DataRow treeNode in nodeTable.Rows)
                {
                    RadTreeNodeData nodeData = new RadTreeNodeData();
                    string typ = attributes.GetValueOrNull("TYP");
                    nodeData.Attributes.Add("TYP", typ);
                    nodeData.Text = treeNode["TITLE"].ToString();
                    nodeData.Value = treeNode["ID"].ToString();
                    nodeData.Attributes.Add("ORDNUMBER", treeNode["ORDNUMBER"].ToString());
                    switch (typ)
                    {
                        case "DUTY":
                            nodeData.ImageUrl = imageUrl + "fb_aufgaben.gif";
                            break;
                    }

                    result.Add(nodeData);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentityReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(result.ToArray());
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveFunctionDescription(long functionId, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE FUNKTION SET DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "' WHERE ID = " + functionId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveFunctionDescriptionFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDutyGroupData(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM DUTYGROUP WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDutyData(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT NUMBER, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION, VALID_FROM, VALID_TO FROM DUTY_VALIDITY_V WHERE DUTY_ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddDuty(long groupId, float number, string title, string description, string from, string to)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long dutyId = db.newId("DUTY");
                db.execute("UPDATE DUTY SET ORDNUMBER = ORDNUMBER + 1 WHERE ORDNUMBER >= " + number + " AND DUTYGROUP_ID =" + groupId);
                db.execute("INSERT INTO DUTY (ID, ORDNUMBER, DUTYGROUP_ID) VALUES (" + dutyId + ", " + number + ", " + groupId + ")");
                long id = db.newId("DUTY_VALIDITY");
                db.execute("INSERT INTO DUTY_VALIDITY(ID, DUTY_ID, NUMBER,  TITLE_" + lang + ", DESCRIPTION_" + lang + ",  VALID_FROM, VALID_TO) VALUES(" + id + ", " + dutyId + ", " + number + ", '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(description) + "', '" + DateTime.ParseExact(from, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "', '" + DateTime.ParseExact(to, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "')");
                detailData = db.getDataTable("select DUTY_ID, NUMBER,  TITLE_" + lang + ", DESCRIPTION_" + lang + ",  VALID_FROM, VALID_TO FROM DUTY_VALIDITY WHERE DUTY_ID = " + dutyId);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fb_aufgaben.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string AddDutyGroup(long groupId, string title, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long id = db.newId("DUTYGROUP");
                long rootId = db.lookup("ROOT_ID", "DUTYGROUP", "PARENT_ID IS NULL", 0L);
                int ordNumber = db.lookup("CAST(MAX(ORDNUMBER) AS INT)", "DUTYGROUP", "PARENT_ID = " + groupId, 0) + 1;
                db.execute("INSERT INTO DUTYGROUP (ID, PARENT_ID, ROOT_ID, ORDNUMBER, TITLE_" + lang + ", DESCRIPTION_" + lang + ") VALUES (" + id + ", " + groupId + ", " + rootId + ", " + ordNumber + ", '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(description) + "')");
                detailData = db.getDataTable("SELECT ID, ORDNUMBER, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM DUTYGROUP WHERE ID = " + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fb_aufgabengruppe.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateDuty(long id, float number, string title, string description, string from, string to)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE DUTY SET ORDNUMBER = " + number + " WHERE ID = " + id);
                db.execute("UPDATE DUTY_VALIDITY SET NUMBER= " + number + ", TITLE_" + lang + " ='" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + "= '" + DBColumn.toSql(description) + "', VALID_FROM = '" + DateTime.ParseExact(from, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "', VALID_TO = '" + DateTime.ParseExact(to, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "' WHERE DUTY_ID =" + id);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateDutyGroup(long id, string title, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE DUTYGROUP SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "' WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteDutyGroup(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID FROM DUTY WHERE DUTYGROUP_ID = " + id);
                foreach (DataRow row in detailData.Rows)
                {
                    db.execute("DELETE FROM DUTY_COMPETENCE_VALIDITY WHERE DUTY_ID =" + row["ID"].ToString());
                    db.execute("DELETE FROM DUTY_VALIDITY WHERE ID =" + row["ID"].ToString());
                    db.execute("DELETE FROM DUTY WHERE ID =" + row["ID"].ToString());
                }
                db.execute("DELETE FROM DUTYGROUP WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveDuty(long dutyId, long groupId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE DUTY SET DUTYGROUP_ID = " + groupId + " WHERE ID = " + dutyId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveDutyGroup(long sourceGroupId, long targetGroupId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                int ordNumber = db.lookup("CAST(MAX(ORDNUMBER) AS INT)", "DUTYGROUP", "PARENT_ID = " + targetGroupId, 0) + 1;
                db.execute("UPDATE DUTYGROUP SET PARENT_ID = " + targetGroupId + ", ORDNUMBER = " + ordNumber + " WHERE ID = " + sourceGroupId);
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, ORDNUMBER FROM DUTYGROUP WHERE ID = " + sourceGroupId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteDuty(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM DUTY_COMPETENCE_VALIDITY WHERE DUTY_ID =" + id);
                db.execute("DELETE FROM DUTY_VALIDITY WHERE ID =" + id);
                db.execute("DELETE FROM DUTY WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetnewItemNumber(long groupId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT MAX(NUMBER) + 1 AS NUMBER FROM DUTY_VALIDITY_V WHERE DUTYGROUP_ID = " + groupId);
                if (detailData.Rows[0][0].ToString() == "")
                {
                    detailData.Rows[0][0] = 1;
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDutySearchList(string name, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable listItems = new DataTable();
            db.connect();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fb_aufgaben.gif";
                string sql = "SELECT ID, TITLE_" + lang + " AS TITLE, DUTYGROUP_ID, 'DUTY' AS TYP,'" + imageUrl + "' AS IMAGE   FROM DUTYV ";
                if (name.Length > 0 || description.Length > 0)
                {
                    sql += "WHERE ";
                    if (name.Length > 0)
                    {
                        sql += "TITLE_" + lang + " LIKE'%" + DBColumn.toSql(name) + "%' ";
                    }
                    if (name.Length > 0 && description.Length > 0)
                    {
                        sql += "AND ";
                    }
                    if (description.Length > 0)
                    {
                        sql += "DESCRIPTION_" + lang + " LIKE'%" + DBColumn.toSql(description) + "%' ";
                    }

                }

                sql += "ORDER BY TITLE_" + lang;
                listItems = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionListFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(listItems));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RadTreeNodeData[] GetFunctionDescriptionDutiyGroups(RadTreeNodeData node, object context)
        {
            CustomDataErrorObj ret = GetRetObj();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable nodeTable = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";

                if (node.Attributes["TYP"].Equals("FUNCTION"))
                {
                    nodeTable = db.getDataTable("SELECT ID, ORDNUMBER, TITLE_" + lang + " AS TITLE FROM DUTYGROUP WHERE ID IN (SELECT DUTYGROUP_ID FROM DUTY_COMPETENCE__DUTY_VALIDITY_V WHERE FUNKTION_ID = " + node.Value + ") ORDER BY ORDNUMBER");

                    foreach (DataRow treeNode in nodeTable.Rows)
                    {
                        RadTreeNodeData nodeData = new RadTreeNodeData();
                        nodeData.Text = treeNode["TITLE"].ToString();
                        nodeData.Value = treeNode["ID"].ToString();
                        nodeData.Attributes.Add("FUNCTIONID", node.Value.ToString());
                        nodeData.Attributes.Add("TYP", "DUTYGROUP");
                        nodeData.Attributes.Add("ORDNUMBER", treeNode["ORDNUMBER"].ToString());
                        nodeData.ImageUrl = imageUrl + "fb_aufgabengruppe.gif";
                        nodeData.ExpandMode = TreeNodeExpandMode.WebService;
                        result.Add(nodeData);
                    }
                }

                if (node.Attributes["TYP"].Equals("DUTYGROUP"))
                {
                    string functionId = node.Attributes["FUNCTIONID"].ToString();
                    string dutyGroupId = node.Value;
                    nodeTable = db.getDataTable("SELECT ID, DUTY_ID, NUM_TITLE_" + lang + " AS TITLE FROM DUTY_COMPETENCE__DUTY_VALIDITY_V WHERE FUNKTION_ID = " + functionId + " AND DUTYGROUP_ID = " + dutyGroupId + " ORDER BY NUM_TITLE_" + lang);
                    foreach (DataRow treeNode in nodeTable.Rows)
                    {
                        RadTreeNodeData nodeData = new RadTreeNodeData();
                        nodeData.Text = treeNode["TITLE"].ToString();
                        nodeData.Value = treeNode["ID"].ToString();
                        nodeData.Attributes.Add("TYP", "DUTY");
                        nodeData.Attributes.Add("DUTY_ID", treeNode["DUTY_ID"].ToString());
                        nodeData.ImageUrl = imageUrl + "fb_aufgaben.gif";
                        result.Add(nodeData);
                    }
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentityReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return result.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string GetFunctionDetail(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM FUNKTION WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionOrGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDutyTree(string name, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable detailDat = new DataTable();
            string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
            string dutyImage = imageUrl + "fb_aufgaben.gif";
            string DutyGroupImage = imageUrl + "fb_aufgabenGruppe.gif";

            try
            {
                string sql = "SELECT ID, PARENT_ID, TITLE_" + lang + " AS TITLE, CASE WHEN ISDuty = 0 THEN 'DUTYGROUP' ELSE 'DUTY' END AS TYP, CASE WHEN ISDuty = 0 THEN '" + DutyGroupImage + "' ELSE '" + dutyImage + "' END AS IMAGE, ORDNUMBER FROM DUTYGROUPTREEV";
                if (name.Length > 0 || description.Length > 0)
                {
                    sql += " WHERE(";
                    if (name.Length > 0)
                    {
                        sql += " TITLE_" + lang + " LIKE '%" + SQLColumn.toSql(name) + "%'";
                    }
                    if (name.Length > 0 && description.Length > 0)
                    {
                        sql += " AND";
                    }
                    if (description.Length > 0)
                    {
                        sql += " DESCRIPTION_" + lang + "   LIKE '%" + SQLColumn.toSql(description) + "%'";
                    }
                    sql += ") OR ISDuty = 0";

                }
                sql += " ORDER BY TYP DESC, PARENT_ID ";
                detailDat = db.getDataTableExt(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailDat));

        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddDutyFunctionsdescription(long targetId, long sourceId)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long id = db.newId("DUTY_COMPETENCE_VALIDITY");
                db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (ID, DUTY_ID, FUNKTION_ID) VALUES (" + id + ", " + sourceId + ", " + targetId + ")");
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion.gif";
                detailData = db.getDataTable("SELECT ID, DUTY_ID, DUTYGROUP_ID, FUNKTION_ID, NUM_TITLE_" + lang + " AS TITLE FROM DUTY_COMPETENCE__DUTY_VALIDITY_V WHERE ID = " + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "addDutyFunctionsdescriptionFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteDutyFunctiondescription(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM DUTY_COMPETENCE_VALIDITY WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFunctiondescription(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM DUTY_COMPETENCE_VALIDITY WHERE FUNKTION_ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCompetences(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, COMPETENCE_LEVEL_ID FROM COMPETENCE WHERE DUTY_COMPETENCE_VALIDITY_ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readCompetencesFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }
            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveCompetences(object[] competences, long dutyCompetenceId, long functionId)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            var jss = new JavaScriptSerializer();
            try
            {
                //IDictionary<string, string> comp = new Dictionary<string, string>();
                foreach (object competence in competences)
                {
                    IDictionary comp = (competence as IDictionary);
                    long competenceLevelId = Convert.ToInt32(comp["competenceId"]);
                    bool active = Convert.ToBoolean(comp["checked"]);
                    long competenceId = db.lookup("ID", "COMPETENCE", "DUTY_COMPETENCE_VALIDITY_ID = " + dutyCompetenceId + " AND COMPETENCE_LEVEL_ID = " + competenceLevelId, 0L);

                    if (competenceId == 0 && active)
                    {
                        long newCompetenceId = db.newId("COMPETENCE");
                        db.execute("INSERT INTO COMPETENCE (ID, DUTY_COMPETENCE_VALIDITY_ID, COMPETENCE_LEVEL_ID) VALUES (" + newCompetenceId + ", " + dutyCompetenceId + ", " + competenceLevelId + ")");
                    }
                    if (competenceId > 0 && !active)
                    {
                        db.execute("DELEte FROM COMPETENCE WHERE ID = " + competenceId);
                    }


                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveCompetencesFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionWithDuty(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, VALID_FROM, VALID_TO FROM DUTYCOMPETENCEFUNKTIONV WHERE id=" + id + " ORDER BY TITLE_" + lang);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CopyFunctiondescription(long destId, long sourceId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                DataTable duties = db.getDataTable("SELECT * FROM DUTY_COMPETENCE_VALIDITY WHERE FUNKTION_ID = " + sourceId);
                foreach (DataRow duty in duties.Rows)
                {
                    long newDutyId = db.newId("DUTY_COMPETENCE_VALIDITY");
                    db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (ID, VALID_FROM, VALID_TO, DUTY_ID, FUNKTION_ID) VALUES (" + newDutyId + ", '" + DateTime.Parse(duty["VALID_FROM"].ToString()).ToString("MM.dd.yyyy") + "', '" + DateTime.Parse(duty["VALID_TO"].ToString()).ToString("MM.dd.yyyy") + "', " + duty["DUTY_ID"] + ", " + destId + ")");

                    DataTable competenceTable = db.getDataTable("SELECT COMPETENCE_LEVEL_ID FROM COMPETENCE WHERE DUTY_COMPETENCE_VALIDITY_ID = " + duty["ID"]);
                    foreach (DataRow competence in competenceTable.Rows)
                    {
                        long newCompetenceID = db.newId("COMPETENCE");
                        db.execute("INSERT INTO COMPETENCE (ID, DUTY_COMPETENCE_VALIDITY_ID, COMPETENCE_LEVEL_ID) VALUES(" + newCompetenceID + ", " + newDutyId + ", " + competence["COMPETENCE_LEVEL_ID"] + ")");
                    }
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "copyFunctionsdescriptionFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ CompetencesLevel --------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCompetenceList(string title, string titleShort)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                string sql = "SELECT ID, NUMBER + ' ' + TITLE_" + lang + " AS TITLE FROM COMPETENCE_LEVEL";

                if (title.Length > 0 || titleShort.Length > 0)
                {
                    sql += " WHERE ";
                    if (title.Length > 0)
                    {
                        sql += "TITLE_" + lang + " LIKE'%" + DBColumn.toSql(title) + "%' ";
                    }
                    if (title.Length > 0 && titleShort.Length > 0)
                    {
                        sql += "AND ";
                    }
                    if (titleShort.Length > 0)
                    {
                        sql += "DESCRIPTION_" + lang + " LIKE'%" + DBColumn.toSql(titleShort) + "%' ";
                    }

                }
                sql += " ORDER BY NUMBER";
                detailData = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readCompetencesFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCompetenceLevelDetail(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable(" SELECT NUMBER, TITLE_" + lang + " AS TITLE, MNEMO_" + lang + " AS MNEMO, DESCRIPTION_" + lang + " AS DESCRIPTION FROM COMPETENCE_LEVEL WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readCompetencesFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveCompetenceLevelDetail(long id, int number, string title, string mnemo, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (id > 0)
                {
                    db.execute("UPDATE COMPETENCE_LEVEL SET NUMBER = '" + number + "', TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', MNEMO_" + lang + " = '" + DBColumn.toSql(mnemo) + "', DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "' WHERE ID = " + id);
                }
                else
                {
                    id = db.newId("COMPETENCE_LEVEL");
                    db.execute("INSERT INTO COMPETENCE_LEVEL (ID, NUMBER, TITLE_" + lang + ", MNEMO_" + lang + ", DESCRIPTION_" + lang + ")VALUES(" + id + ", '" + number + "', '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(mnemo) + "', '" + DBColumn.toSql(description) + "')");
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readCompetencesFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteCompetenceLevel(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM COMPETENCE_LEVEl WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteCompetenceLevelFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Skills ------------------------
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getSkillTreeData(long sourceNodeId, Dictionary<string, string> attributes)
        {
            CustomDataErrorObj ret = GetRetObj();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable nodeTable = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
                nodeTable = db.getDataTable("SELECT ID, ORDNUMBER, TITLE_" + lang + " AS TITLE, 'SKILL' AS TYP FROM SKILLV WHERE  SKILLGROUP_ID = " + sourceNodeId + " ORDER BY ORDNUMBER");
                foreach (DataRow treeNode in nodeTable.Rows)
                {
                    RadTreeNodeData nodeData = new RadTreeNodeData();
                    string typ = attributes.GetValueOrNull("TYP");
                    nodeData.Attributes.Add("TYP", typ);
                    nodeData.Text = treeNode["TITLE"].ToString();
                    nodeData.Value = treeNode["ID"].ToString();
                    nodeData.Attributes.Add("ORDNUMBER", treeNode["ORDNUMBER"].ToString());
                    switch (typ)
                    {
                        case "SKILL":
                            nodeData.ImageUrl = imageUrl + "sk_skill.gif";
                            break;
                    }

                    result.Add(nodeData);
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readSkillFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(result.ToArray());
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string SaveFunctionDescription(long functionId, string description)
        //{
        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        db.execute("UPDATE FUNKTION SET DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "' WHERE ID = " + functionId);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "saveFunctionDescriptionFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSkillGroupData(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM SKILLGROUP WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readSkillGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSkillData(long id)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT NUMBER, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION, VALID_FROM, VALID_TO FROM SKILL_VALIDITY_V WHERE SKILL_ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readSkillFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddSkill(long groupId, float number, string title, string description, string from, string to)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long skillId = db.newId("SKILL");
                db.execute("UPDATE SKILL SET ORDNUMBER = ORDNUMBER + 1 WHERE ORDNUMBER >= " + number + " AND SKILLGROUP_ID =" + groupId);
                db.execute("INSERT INTO SKILL (ID, ORDNUMBER, SKILLGROUP_ID) VALUES (" + skillId + ", " + number + ", " + groupId + ")");
                long id = db.newId("SKILL_VALIDITY");
                db.execute("INSERT INTO SKILL_VALIDITY(ID, SKILL_ID, NUMBER,  TITLE_" + lang + ", DESCRIPTION_" + lang + ",  VALID_FROM, VALID_TO) VALUES(" + id + ", " + skillId + ", " + number + ", '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(description) + "', '" + DateTime.ParseExact(from, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "', '" + DateTime.ParseExact(to, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "')");
                detailData = db.getDataTable("select SKILL_ID, NUMBER,  TITLE_" + lang + ", DESCRIPTION_" + lang + ",  VALID_FROM, VALID_TO FROM SKILL_VALIDITY WHERE SKILL_ID = " + skillId);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/sk_skill.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveDutyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string AddSkillGroup(long groupId, string title, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long id = db.newId("SKILLGROUP");
                long rootId = db.lookup("ROOT_ID", "SKILLGROUP", "PARENT_ID IS NULL", 0L);
                int ordNumber = db.lookup("CAST(MAX(ORDNUMBER) AS INT)", "SKILLGROUP", "PARENT_ID = " + groupId, 0) + 1;
                db.execute("INSERT INTO SKILLGROUP (ID, PARENT_ID, ROOT_ID, ORDNUMBER, TITLE_" + lang + ", DESCRIPTION_" + lang + ") VALUES (" + id + ", " + groupId + ", " + rootId + ", " + ordNumber + ", '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(description) + "')");
                detailData = db.getDataTable("SELECT ID, ORDNUMBER, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM SKILLGROUP WHERE ID = " + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/sk_skillgruppe.gif";
                detailData.Rows[0]["IMAGE"] = imageUrl;
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveSkillGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateSkill(long id, float number, string title, string description, string from, string to)
        {

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE SKILL SET ORDNUMBER = " + number + " WHERE ID = " + id);
                db.execute("UPDATE SKILL_VALIDITY SET NUMBER= " + number + ", TITLE_" + lang + " ='" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + "= '" + DBColumn.toSql(description) + "', VALID_FROM = '" + DateTime.ParseExact(from, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "', VALID_TO = '" + DateTime.ParseExact(to, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "' WHERE SKILL_ID =" + id);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveSkillFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateSkillGroup(long id, string title, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE SKILLGROUP SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "' WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveSkillGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteSkillGroup(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID FROM SKILL WHERE SKILLGROUP_ID = " + id);
                foreach (DataRow row in detailData.Rows)
                {
                    db.execute("DELETE FROM SKILL_COMPETENCE_VALIDITY WHERE DUTY_ID =" + row["ID"].ToString());
                    db.execute("DELETE FROM SKILL_VALIDITY WHERE ID =" + row["ID"].ToString());
                    db.execute("DELETE FROM SKILL WHERE ID =" + row["ID"].ToString());
                }
                db.execute("DELETE FROM SKILLGROUP WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteSkillGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveSkill(long skillId, long groupId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE DUTY SET DUTYGROUP_ID = " + groupId + " WHERE ID = " + skillId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveSkillFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveSkillGroup(long sourceGroupId, long targetGroupId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                int ordNumber = db.lookup("CAST(MAX(ORDNUMBER) AS INT)", "SKILLGROUP", "PARENT_ID = " + targetGroupId, 0) + 1;
                db.execute("UPDATE SKILLGROUP SET PARENT_ID = " + targetGroupId + ", ORDNUMBER = " + ordNumber + " WHERE ID = " + sourceGroupId);
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, ORDNUMBER FROM SKILLGROUP WHERE ID = " + sourceGroupId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveSkillGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ReorderSkill(long sourceSkillId, long destinationSkillId, string dropPosition)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long skillGroupId = db.lookup("SKILLGROUP_ID", "SKILL", "ID = " + sourceSkillId, 0L);
                double dropSkillPosNr = db.lookup("ORDNUMBER", "SKILL", "ID=" + destinationSkillId, 0d);
                if (dropPosition.Equals("above"))
                {
                    db.execute("UPDATE SKILL SET ORDNUMBER = ORDNUMBER + 2 WHERE ORDNUMBER >=  " + dropSkillPosNr + " AND SKILLGROUP_ID = " + skillGroupId);
                    db.execute("UPDATE SKILL SET ORDNUMBER = " + dropSkillPosNr + " WHERE ID =" + sourceSkillId);
                }
                if (dropPosition.Equals("below"))
                {
                    db.execute("UPDATE SKILL SET ORDNUMBER = ORDNUMBER + 1 WHERE ORDNUMBER >  " + dropSkillPosNr + " AND SKILLGROUP_ID = " + skillGroupId);
                    db.execute("UPDATE SKILL SET ORDNUMBER = " + (dropSkillPosNr + 1) + " WHERE ID =" + sourceSkillId);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "moveSkillFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteSkill(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM SKILL_LEVEL_VALIDITY WHERE SKILL_ID =" + id);
                db.execute("DELETE FROM SKILL_VALIDITY WHERE ID =" + id);
                db.execute("DELETE FROM SKILL WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteSkillFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetnewSkillNumber(long groupId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT MAX(NUMBER) + 1 AS NUMBER FROM SKILL_VALIDITY_V WHERE SKILLGROUP_ID = " + groupId);
                if (detailData.Rows[0][0].ToString() == "")
                {
                    detailData.Rows[0][0] = 1;
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSkillSearchList(string name, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable listItems = new DataTable();
            db.connect();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/sk_skill.gif";
                string sql = "SELECT ID, TITLE_" + lang + " AS TITLE, SKILLGROUP_ID, 'SKILL' AS TYP,'" + imageUrl + "' AS IMAGE   FROM SKILLV ";
                if (name.Length > 0 || description.Length > 0)
                {
                    sql += "WHERE ";
                    if (name.Length > 0)
                    {
                        sql += "TITLE_" + lang + " LIKE'%" + DBColumn.toSql(name) + "%' ";
                    }
                    if (name.Length > 0 && description.Length > 0)
                    {
                        sql += "AND ";
                    }
                    if (description.Length > 0)
                    {
                        sql += "DESCRIPTION_" + lang + " LIKE'%" + DBColumn.toSql(description) + "%' ";
                    }

                }

                sql += "ORDER BY TITLE_" + lang;
                listItems = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readSkillListFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(listItems));
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public RadTreeNodeData[] GetFunctionDescriptionDutiyGroups(RadTreeNodeData node, object context)
        //{
        //    CustomDataErrorObj ret = GetRetObj();
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    DBData db = DBData.getDBData(Session);
        //    db.connect();
        //    DataTable nodeTable = new DataTable();
        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    List<RadTreeNodeData> result = new List<RadTreeNodeData>();
        //    try
        //    {
        //        string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";

        //        if (node.Attributes["TYP"].Equals("FUNCTION"))
        //        {
        //            nodeTable = db.getDataTable("SELECT ID, ORDNUMBER, TITLE_" + lang + " AS TITLE FROM DUTYGROUP WHERE ID IN (SELECT DUTYGROUP_ID FROM DUTY_COMPETENCE__DUTY_VALIDITY_V WHERE FUNKTION_ID = " + node.Value + ") ORDER BY ORDNUMBER");

        //            foreach (DataRow treeNode in nodeTable.Rows)
        //            {
        //                RadTreeNodeData nodeData = new RadTreeNodeData();
        //                nodeData.Text = treeNode["TITLE"].ToString();
        //                nodeData.Value = treeNode["ID"].ToString();
        //                nodeData.Attributes.Add("FUNCTIONID", node.Value.ToString());
        //                nodeData.Attributes.Add("TYP", "DUTYGROUP");
        //                nodeData.Attributes.Add("ORDNUMBER", treeNode["ORDNUMBER"].ToString());
        //                nodeData.ImageUrl = imageUrl + "fb_aufgabengruppe.gif";
        //                nodeData.ExpandMode = TreeNodeExpandMode.WebService;
        //                result.Add(nodeData);
        //            }
        //        }

        //        if (node.Attributes["TYP"].Equals("DUTYGROUP"))
        //        {
        //            string functionId = node.Attributes["FUNCTIONID"].ToString();
        //            string dutyGroupId = node.Value;
        //            nodeTable = db.getDataTable("SELECT ID, DUTY_ID, NUM_TITLE_" + lang + " AS TITLE FROM DUTY_COMPETENCE__DUTY_VALIDITY_V WHERE FUNKTION_ID = " + functionId + " AND DUTYGROUP_ID = " + dutyGroupId + " ORDER BY NUM_TITLE_" + lang);
        //            foreach (DataRow treeNode in nodeTable.Rows)
        //            {
        //                RadTreeNodeData nodeData = new RadTreeNodeData();
        //                nodeData.Text = treeNode["TITLE"].ToString();
        //                nodeData.Value = treeNode["ID"].ToString();
        //                nodeData.Attributes.Add("TYP", "DUTY");
        //                nodeData.Attributes.Add("DUTY_ID", treeNode["DUTY_ID"].ToString());
        //                nodeData.ImageUrl = imageUrl + "fb_aufgaben.gif";
        //                result.Add(nodeData);
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "orgentityReadFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return result.ToArray();
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        //public string GetFunctionDetail(long id)
        //{
        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM FUNKTION WHERE ID = " + id);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "readFunctionOrGroupFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetSkillTree(string name, string description)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable detailDat = new DataTable();
            string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";
            string dutyImage = imageUrl + "fb_aufgaben.gif";
            string DutyGroupImage = imageUrl + "fb_aufgabenGruppe.gif";

            try
            {
                string sql = "SELECT ID, PARENT_ID, TITLE_" + lang + " AS TITLE, CASE WHEN ISDuty = 0 THEN 'DUTYGROUP' ELSE 'DUTY' END AS TYP, CASE WHEN ISDuty = 0 THEN '" + DutyGroupImage + "' ELSE '" + dutyImage + "' END AS IMAGE, ORDNUMBER FROM DUTYGROUPTREEV";
                if (name.Length > 0 || description.Length > 0)
                {
                    sql += " WHERE(";
                    if (name.Length > 0)
                    {
                        sql += " TITLE_" + lang + " LIKE '%" + SQLColumn.toSql(name) + "%'";
                    }
                    if (name.Length > 0 && description.Length > 0)
                    {
                        sql += " AND";
                    }
                    if (description.Length > 0)
                    {
                        sql += " DESCRIPTION_" + lang + "   LIKE '%" + SQLColumn.toSql(description) + "%'";
                    }
                    sql += ") OR ISDuty = 0";

                }
                sql += " ORDER BY TYP DESC, PARENT_ID ";
                detailDat = db.getDataTableExt(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDutyGroupFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailDat));

        }


        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string AddDutyFunctionsdescription(long targetId, long sourceId)
        //{

        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        long id = db.newId("DUTY_COMPETENCE_VALIDITY");
        //        db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (ID, DUTY_ID, FUNKTION_ID) VALUES (" + id + ", " + sourceId + ", " + targetId + ")");
        //        string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion.gif";
        //        detailData = db.getDataTable("SELECT ID, DUTY_ID, DUTYGROUP_ID, FUNKTION_ID, NUM_TITLE_" + lang + " AS TITLE FROM DUTY_COMPETENCE__DUTY_VALIDITY_V WHERE ID = " + id);
        //        DataColumn image = new DataColumn("IMAGE");
        //        detailData.Columns.Add(image);
        //        detailData.Rows[0]["IMAGE"] = imageUrl;
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "addDutyFunctionsdescriptionFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string DeleteDutyFunctiondescription(long id)
        //{

        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        db.execute("DELETE FROM DUTY_COMPETENCE_VALIDITY WHERE ID =" + id);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "deleteDutyFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string DeleteFunctiondescription(long id)
        //{

        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        db.execute("DELETE FROM DUTY_COMPETENCE_VALIDITY WHERE FUNKTION_ID =" + id);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "deleteDutyFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string GetCompetences(long id)
        //{
        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        detailData = db.getDataTable("SELECT ID, COMPETENCE_LEVEL_ID FROM COMPETENCE WHERE DUTY_COMPETENCE_VALIDITY_ID = " + id);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "readCompetencesFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }
        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string SaveCompetences(object[] competences, long dutyCompetenceId, long functionId)
        //{

        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    var jss = new JavaScriptSerializer();
        //    try
        //    {
        //        //IDictionary<string, string> comp = new Dictionary<string, string>();
        //        foreach (object competence in competences)
        //        {
        //            IDictionary comp = (competence as IDictionary);
        //            long competenceLevelId = Convert.ToInt32(comp["competenceId"]);
        //            bool active = Convert.ToBoolean(comp["checked"]);
        //            long competenceId = db.lookup("ID", "COMPETENCE", "DUTY_COMPETENCE_VALIDITY_ID = " + dutyCompetenceId + " AND COMPETENCE_LEVEL_ID = " + competenceLevelId, 0L);

        //            if (competenceId == 0 && active)
        //            {
        //                long newCompetenceId = db.newId("COMPETENCE");
        //                db.execute("INSERT INTO COMPETENCE (ID, DUTY_COMPETENCE_VALIDITY_ID, COMPETENCE_LEVEL_ID) VALUES (" + newCompetenceId + ", " + dutyCompetenceId + ", " + competenceLevelId + ")");
        //            }
        //            if (competenceId > 0 && !active)
        //            {
        //                db.execute("DELEte FROM COMPETENCE WHERE ID = " + competenceId);
        //            }


        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "saveCompetencesFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string GetFunctionWithDuty(long id)
        //{

        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, VALID_FROM, VALID_TO FROM DUTYCOMPETENCEFUNKTIONV WHERE id=" + id + " ORDER BY TITLE_" + lang);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "readDataFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public string CopyFunctiondescription(long destId, long sourceId)
        //{
        //    string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
        //    CustomDataErrorObj ret = GetRetObj();
        //    DBData db = DBData.getDBData(Session);
        //    DataTable detailData = new DataTable();
        //    db.connect();
        //    try
        //    {
        //        DataTable duties = db.getDataTable("SELECT * FROM DUTY_COMPETENCE_VALIDITY WHERE FUNKTION_ID = " + sourceId);
        //        foreach (DataRow duty in duties.Rows)
        //        {
        //            long newDutyId = db.newId("DUTY_COMPETENCE_VALIDITY");
        //            db.execute("INSERT INTO DUTY_COMPETENCE_VALIDITY (ID, VALID_FROM, VALID_TO, DUTY_ID, FUNKTION_ID) VALUES (" + newDutyId + ", '" + DateTime.Parse(duty["VALID_FROM"].ToString()).ToString("MM.dd.yyyy") + "', '" + DateTime.Parse(duty["VALID_TO"].ToString()).ToString("MM.dd.yyyy") + "', " + duty["DUTY_ID"] + ", " + destId + ")");

        //            DataTable competenceTable = db.getDataTable("SELECT COMPETENCE_LEVEL_ID FROM COMPETENCE WHERE DUTY_COMPETENCE_VALIDITY_ID = " + duty["ID"]);
        //            foreach (DataRow competence in competenceTable.Rows)
        //            {
        //                long newCompetenceID = db.newId("COMPETENCE");
        //                db.execute("INSERT INTO COMPETENCE (ID, DUTY_COMPETENCE_VALIDITY_ID, COMPETENCE_LEVEL_ID) VALUES(" + newCompetenceID + ", " + newDutyId + ", " + competence["COMPETENCE_LEVEL_ID"] + ")");
        //            }
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Log(ex, Logger.ERROR);
        //        ret.shortMessage = "copyFunctionsdescriptionFailed";
        //        ret.Message = ex.ToString();
        //    }
        //    finally
        //    {
        //        db.disconnect();
        //    }

        //    return ret.getErrorObj(TableToJson(detailData));
        //}

        //------------------ Function Rating ---------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionRating(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, FUNKTIONSWERT, GUELTIG_AB, GUELTIG_BIS FROM FUNKTIONSBEWERTUNG WHERE FUNKTION_ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionratingRootItems(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG, ORDNUMBER, (SELECT CASE (SELECT ISNULL(SUM(STUFE_PUNKTEZAHL), 0) AS Punkte FROM ANFORDERUNGDETAILV WHERE FUNKTIONSBEWERTUNG_ID = " + id + " AND KRITERIUM_ID = FBW_KRITERIUM.ID) "
                                                + "WHEN 0 THEN 0 "
                                                + "ELSE (SELECT SUM(STUFE_PUNKTEZAHL) FROM ANFORDERUNGDETAILV WHERE FUNKTIONSBEWERTUNG_ID = " + id + " AND KRITERIUM_ID = FBW_KRITERIUM.ID) END) AS Punkte FROM FBW_KRITERIUM ORDER BY ORDNUMBER ");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRatingTreeRoot(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long argumentKatalogId = db.lookup("FBW_ARGUMENT_KATALOG_ID", "FBW_KRITERIUM", "ID = " + id, 0L);

                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG, BESCHREIBUNG, ERLAEUTERUNG, ORDNUMBER FROM  FBW_ARGUMENT_KATALOG WHERE ID =" + argumentKatalogId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public RadTreeNodeData[] GetFunctionRatingRatingTreeNodes(RadTreeNodeData node, object context)
        {
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

            CustomDataErrorObj ret = GetRetObj();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBData db = DBData.getDBData(Session);
            string FBWId = contextDictionary["FUNKTION_ID"].ToString();
            db.connect();
            DataTable nodeTable = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            List<RadTreeNodeData> result = new List<RadTreeNodeData>();
            try
            {
                string imageUrl = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/";

                if (node.Attributes["TYP"].Equals("GROUP"))
                {
                    nodeTable = db.getDataTable("SELECT ID, BEZEICHNUNG, BESCHREIBUNG, ERLAEUTERUNG, ORDNUMBER FROM  FBW_ARGUMENT_KATALOG WHERE PARENT_ID =" + node.Value + " ORDER BY ORDNUMBER");

                    foreach (DataRow treeNode in nodeTable.Rows)
                    {
                        RadTreeNodeData nodeData = new RadTreeNodeData();
                        nodeData.Text = treeNode["BEZEICHNUNG"].ToString();
                        nodeData.Value = treeNode["ID"].ToString();
                        nodeData.Attributes.Add("BESCHREIBUNG", treeNode["BESCHREIBUNG"].ToString());
                        nodeData.Attributes.Add("ERLAEUTERUNG", treeNode["ERLAEUTERUNG"].ToString());
                        nodeData.Attributes.Add("TYP", "GROUP");
                        nodeData.Attributes.Add("ORDNUMBER", treeNode["ORDNUMBER"].ToString());
                        nodeData.ImageUrl = imageUrl + "am_argumentgruppe.gif";
                        nodeData.ExpandMode = TreeNodeExpandMode.WebService;
                        result.Add(nodeData);
                    }
                }

                DataTable nodeTableDetail = db.getDataTable("SELECT ID, BEZEICHNUNG, BESCHREIBUNG, ERLAEUTERUNG, ORDNUMBER, 0 AS SELECTED FROM FBW_ARGUMENT WHERE FBW_ARGUMENT_KATALOG_ID = " + node.Value + " ORDER BY ORDNUMBER");
                foreach (DataRow treeNode in nodeTableDetail.Rows)
                {
                    RadTreeNodeData nodeData = new RadTreeNodeData();
                    nodeData.Text = treeNode["BEZEICHNUNG"].ToString();
                    nodeData.Value = treeNode["ID"].ToString();
                    nodeData.Attributes.Add("BESCHREIBUNG", treeNode["BESCHREIBUNG"].ToString());
                    nodeData.Attributes.Add("ERLAEUTERUNG", treeNode["ERLAEUTERUNG"].ToString());
                    nodeData.Attributes.Add("TYP", "NODE");
                    nodeData.Attributes.Add("ORDNUMBER", treeNode["ORDNUMBER"].ToString());
                    int isRated = db.lookup("COUNT(ID)", "FBW_ANFORDERUNG", "FUNKTIONSBEWERTUNG_ID = " + FBWId + " AND FBW_ARGUMENT_ID = " + treeNode["ID"].ToString(), 0);
                    if (isRated == 0)
                    {
                        nodeData.ImageUrl = imageUrl + "am_argument_inaktiv.gif";
                        nodeData.Attributes.Add("ACTIVE", "0");
                    }
                    else
                    {
                        nodeData.ImageUrl = imageUrl + "am_argument.gif";
                        nodeData.Attributes.Add("ACTIVE", "1");
                    }
                    result.Add(nodeData);
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentityReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return result.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GeRequirement(long fbwId, long rootItemId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ANFORDERUNGDETAILV.ID, FBW_ARGUMENT_KATALOG.BEZEICHNUNG + ' ---> ' + ANFORDERUNGDETAILV.BEZEICHNUNG AS BEZEICHNUNG, ANFORDERUNGDETAILV.STUFE_PUNKTEZAHL "
                                                + "FROM ANFORDERUNGDETAILV INNER JOIN FBW_ARGUMENT_KATALOG ON ANFORDERUNGDETAILV.FBW_ARGUMENT_KATALOG_ID = FBW_ARGUMENT_KATALOG.ID "
                                                + "WHERE ANFORDERUNGDETAILV.FUNKTIONSBEWERTUNG_ID =" + fbwId + " AND ANFORDERUNGDETAILV.KRITERIUM_ID = " + rootItemId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRatingTreeNodesToRequirementItem(long itemId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            DataColumn PARENT_ID = new DataColumn("PARENT_ID");
            detailData.Columns.Add(PARENT_ID);
            string parentId = "";

            db.connect();
            try
            {
                long fbwArgumnetId = db.lookup("FBW_ARGUMENT_ID", "FBW_ANFORDERUNG", "ID = " + itemId, 0L);
                parentId = db.lookup("FBW_ARGUMENT_KATALOG_ID", "FBW_ARGUMENT", "ID = " + fbwArgumnetId).ToString();
                if (!parentId.Equals("NULL"))
                {
                    DataRow row = detailData.NewRow();
                    row["PARENT_ID"] = parentId;
                    detailData.Rows.Add(row);
                }

                while (parentId != "")
                {
                    parentId = db.lookup("PARENT_ID", "FBW_ARGUMENT_KATALOG", " ID = " + parentId).ToString();
                    if (!parentId.Equals(""))
                    {
                        DataRow row = detailData.NewRow();
                        row["PARENT_ID"] = parentId;
                        detailData.Rows.Add(row);
                    }

                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFunctionRating(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM FBW_ANFORDERUNG WHERE FUNKTIONSBEWERTUNG_ID = " + id);
                db.execute("DELETE FROM FUNKTIONSBEWERTUNG WHERE ID = " + id);
                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                DataRow row = detailData.NewRow();
                row["IMAGE"] = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion_inaktiv.gif";
                detailData.Rows.Add(row);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "deleteFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddFunctionRating(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            DataTable ratingItems = new DataTable();
            db.connect();
            try
            {
                long ratingId = db.newId("FUNKTIONSBEWERTUNG");
                db.execute("INSERT INTO FUNKTIONSBEWERTUNG (ID, FUNKTION_ID) VALUES (" + ratingId + ", " + id + ")");

                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                DataRow row = detailData.NewRow();
                row["IMAGE"] = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion.gif";
                detailData.Rows.Add(row);
            }
   
            catch (Exception ex)
            { 
                ret.shortMessage = "addFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddRatingItem(long ratingId, long argumentId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long itemId = db.newId("FBW_ANFORDERUNG");
                db.execute("INSERT INTO FBW_ANFORDERUNG (ID, FBW_ARGUMENT_ID, FUNKTIONSBEWERTUNG_ID ) VALUES (" + itemId + ", " + argumentId + ", " + ratingId + ")");
                double fbwTot = Convert.ToDouble(db.lookup("SUM(FBW_STUFE.PUNKTEZAHL)", "FBW_ANFORDERUNG INNER JOIN FBW_ARGUMENT ON FBW_ANFORDERUNG.FBW_ARGUMENT_ID = FBW_ARGUMENT.ID INNER JOIN FBW_STUFE ON FBW_ARGUMENT.FBW_STUFE_ID = FBW_STUFE.ID", "FBW_ANFORDERUNG.FUNKTIONSBEWERTUNG_ID = " + ratingId, 0D));
                db.execute("UPDATE FUNKTIONSBEWERTUNG SET FUNKTIONSWERT = '" + fbwTot.ToString(CultureInfo.CreateSpecificCulture("en-US")) + "' WHERE ID =" + ratingId);

                UpdateFBWKommentar(ratingId);

                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                DataRow row = detailData.NewRow();
                row["IMAGE"] = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/am_argument.gif";
                detailData.Rows.Add(row);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "addRatingArgumentFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string RemoveRatingItem(long ratingId, long argumentId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM FBW_ANFORDERUNG WHERE FUNKTIONSBEWERTUNG_ID = " + ratingId + " AND FBW_ARGUMENT_ID = " + argumentId);
                decimal fbwTot = Convert.ToDecimal(db.lookup("SUM(FBW_STUFE.PUNKTEZAHL)", "FBW_ANFORDERUNG INNER JOIN FBW_ARGUMENT ON FBW_ANFORDERUNG.FBW_ARGUMENT_ID = FBW_ARGUMENT.ID INNER JOIN FBW_STUFE ON FBW_ARGUMENT.FBW_STUFE_ID = FBW_STUFE.ID", "FBW_ANFORDERUNG.FUNKTIONSBEWERTUNG_ID = " + ratingId, 0D));
                db.execute("UPDATE FUNKTIONSBEWERTUNG SET FUNKTIONSWERT = " + fbwTot + "WHERE ID =" + ratingId);

                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                DataRow row = detailData.NewRow();
                row["IMAGE"] = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/am_argument_inaktiv.gif";
                detailData.Rows.Add(row);
                UpdateFBWKommentar(ratingId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "removeRatingArgumentFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionRatingValue(long id)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT FUNKTIONSWERT FROM FUNKTIONSBEWERTUNG WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        private void UpdateFBWKommentar(long ratingId)
        {
            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable CriteriaWeights = db.getDataTable("SELECT FUNKTIONSBEWERTUNG.ID AS FBW_ID, ANFORDERUNGDETAILV.KRITERIUM_ID, SUM(ANFORDERUNGDETAILV.STUFE_PUNKTEZAHL) AS PUNKTE "
                                                            +"FROM FUNKTIONSBEWERTUNG INNER JOIN ANFORDERUNGDETAILV ON FUNKTIONSBEWERTUNG.ID = ANFORDERUNGDETAILV.FUNKTIONSBEWERTUNG_ID "
                                                            +"GROUP BY FUNKTIONSBEWERTUNG.ID, ANFORDERUNGDETAILV.KRITERIUM_ID  "
                                                            +"HAVING FUNKTIONSBEWERTUNG.ID = " + ratingId +" "
                                                            +"UNION "
                                                            + "SELECT FBW_ID = " + ratingId + ", ID AS KRITERIUM_ID, PUNKTE = 0 FROM  FBW_KRITERIUM WHERE ID NOT IN(SELECT ANFORDERUNGDETAILV.KRITERIUM_ID "
                                                            + "FROM FUNKTIONSBEWERTUNG INNER JOIN ANFORDERUNGDETAILV ON FUNKTIONSBEWERTUNG.ID = ANFORDERUNGDETAILV.FUNKTIONSBEWERTUNG_ID "
                                                            +"GROUP BY FUNKTIONSBEWERTUNG.ID, ANFORDERUNGDETAILV.KRITERIUM_ID "
                                                            + "HAVING FUNKTIONSBEWERTUNG.ID = " + ratingId + ")");
            foreach (DataRow criteria in CriteriaWeights.Rows)
            {
                db.execute("UPDATE FBW_KOMMENTAR SET PUNKTEZAHL = " + criteria["PUNKTE"] + " WHERE FBW_KRITERIUM_ID = " + criteria["KRITERIUM_ID"] + " AND FUNKTIONSBEWERTUNG_ID = " + criteria["FBW_ID"]);
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRootItemValue(long itemId, long ratingId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, (SELECT CASE (SELECT ISNULL(SUM(STUFE_PUNKTEZAHL), 0) AS Punkte FROM ANFORDERUNGDETAILV WHERE FUNKTIONSBEWERTUNG_ID = " + ratingId + " AND KRITERIUM_ID = " + itemId + ") WHEN 0 THEN 0 ELSE (SELECT SUM(STUFE_PUNKTEZAHL) FROM ANFORDERUNGDETAILV WHERE FUNKTIONSBEWERTUNG_ID = " + ratingId + " AND KRITERIUM_ID = " + itemId + ") END) AS Punkte FROM FBW_KRITERIUM");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetArgumentFromAnforderung(long anforderungId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT FBW_ARGUMENT_ID FROM FBW_ANFORDERUNG WHERE ID = " + anforderungId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionsArgumentReference(long argumentId)
        {
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT DISTINCT FUNKTION.ID, FUNKTION.TITLE_" + lang + " AS TITLE "
                                                + "FROM FBW_ANFORDERUNG INNER JOIN FUNKTIONSBEWERTUNG ON FBW_ANFORDERUNG.FUNKTIONSBEWERTUNG_ID = FUNKTIONSBEWERTUNG.ID INNER JOIN FUNKTION ON FUNKTIONSBEWERTUNG.FUNKTION_ID = FUNKTION.ID "
                                                + "WHERE FBW_ANFORDERUNG.FBW_ARGUMENT_ID = " + argumentId + " ORDER BY TITLE_" + lang);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String GetArgumentDetail(long argumentId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT FBW_ARGUMENT.ID, FBW_ARGUMENT.BEZEICHNUNG, FBW_ARGUMENT.BESCHREIBUNG, FBW_ARGUMENT.ERLAEUTERUNG, FBW_STUFE.PUNKTEZAHL "
                                                + "FROM FBW_ARGUMENT INNER JOIN FBW_STUFE ON FBW_ARGUMENT.FBW_STUFE_ID = FBW_STUFE.ID WHERE FBW_ARGUMENT.ID = " + argumentId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readDataFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveArgumentDetail(long id, string description, string example)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE FBW_ARGUMENT SET BESCHREIBUNG = '" + DBColumn.toSql(description) + "', ERLAEUTERUNG = '" + DBColumn.toSql(example) + "' WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveArgumentDetailFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CopyFunctionRating(long SourceFunctionRatingId, long destFunctionId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                long ratingId = db.newId("FUNKTIONSBEWERTUNG");
                double ratingValue = db.lookup("FUNKTIONSWERT", "FUNKTIONSBEWERTUNG", "ID = " + SourceFunctionRatingId, 0D);
                db.execute("INSERT INTO FUNKTIONSBEWERTUNG (ID, FUNKTIONSWERT, FUNKTION_ID) VALUES (" + ratingId + ", " + ratingValue.ToString(CultureInfo.CreateSpecificCulture("en-US")) + ", " + destFunctionId + ")");
                DataTable argumentTable = db.getDataTable("SELECT FBW_ARGUMENT_ID FROM FBW_ANFORDERUNG WHERE FUNKTIONSBEWERTUNG_ID = " + SourceFunctionRatingId);
                foreach (DataRow argument in argumentTable.Rows)
                {
                    long argumentId = db.newId("FBW_ANFORDERUNG");
                    db.execute("INSERT INTO FBW_ANFORDERUNG(ID, FBW_ARGUMENT_ID, FUNKTIONSBEWERTUNG_ID) VALUES (" + argumentId + ", " + argument["FBW_ARGUMENT_ID"].ToString() + ", " + ratingId + ")");
                }

                DataColumn image = new DataColumn("IMAGE");
                detailData.Columns.Add(image);
                DataRow row = detailData.NewRow();
                row["IMAGE"] = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/fx_funktion.gif";
                detailData.Rows.Add(row);
                UpdateFBWKommentar(ratingId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "copyFunctionRatingFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Functiontyp --------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionTypList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG FROM FUNKTION_TYP WHERE BEZEICHNUNG LIKE '%" + title + "%'");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionTypReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionTypDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG, BESCHREIBUNG FROM FUNKTION_TYP WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionTypReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveUpdateFunctionTyp(long id, string bezeichnung, string beschreibung)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (id == 0)
                {
                    id = db.newId("FUNKTION_TYP");
                    db.execute("INSERT INTO FUNKTION_TYP (ID, BEZEICHNUNG, BESCHREIBUNG) VALUES (" + id + ", '" + DBColumn.toSql(bezeichnung) + "', '" + DBColumn.toSql(beschreibung) + "')");
                }
                else
                {
                    db.execute("UPDATE FUNKTION_TYP SET BEZEICHNUNG = '" + DBColumn.toSql(bezeichnung) + "', BESCHREIBUNG = '" + DBColumn.toSql(beschreibung) + "' WHERE ID =" + id);
                }
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG FROM FUNKTION_TYP WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionTypSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFunctionTyp(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM FUNKTION_TYP WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionTypDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Functionoptions ----------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOptionList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG FROM FBW_KRITERIUM WHERE BEZEICHNUNG LIKE '%" + title + "%'");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionOptionReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCatalogOptionDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, FBW_MITTELUNGSART_FLAG, FBW_PUNKTMAXIMUM FROM FBW_KRITERIUM WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionOptionReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveCatalogOption(long id, int averageTyp, double maxValue)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("UPDATE FBW_KRITERIUM SET FBW_MITTELUNGSART_FLAG = " + averageTyp + ", FBW_PUNKTMAXIMUM = " + maxValue + " WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionOptionSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Performancerating Lockdate -----------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetLockdate()
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT DATUM_WERT FROM PROPERTY WHERE GRUPPE='performance' AND TITLE='lock'");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readLockdatFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveLockdate(string lockdate)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                if (lockdate != null)
                {
                    db.execute("UPDATE PROPERTY SET DATUM_WERT='" + DateTime.ParseExact(lockdate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "' WHERE GRUPPE='performance' AND TITLE='lock'");
                    db.execute("update performancerating set lock=1 where rating_date < (select dateadd(d, +1, datum_wert) from PROPERTY where gruppe='performance' and title='lock')");
                    db.execute("update performancerating set lock=0 where rating_date > (select dateadd(d, +1, datum_wert) from PROPERTY where gruppe='performance' and title='lock')");
                }
                else
                {
                    db.execute("UPDATE PROPERTY SET DATUM_WERT= null WHERE GRUPPE='performance' AND TITLE='lock'");
                    db.execute("UPDATE PERFORMANCERATING SET LOCK = 0");
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "saveLockdatFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Performancerating Ratinglevels -------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRatingLevelList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM PERFORMANCE_LEVEL WHERE TITLE_" + lang + " LIKE '%" + title + "%'");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "ratingLevelsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRatingLevelDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION, RELATIV_WEIGHT, VALID + 1 AS VALID FROM PERFORMANCE_LEVEL WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "ratingLevelsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveRatinglevel(long id, string title, string description, double relativeWeight, int valid)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                if (id == 0)
                {
                    id = db.newId("PERFORMANCE_LEVEL");
                    db.execute("INSERT INTO PERFORMANCE_LEVEL (ID, TITLE_" + lang + ", DESCRIPTION_" + lang + ", RELATIV_WEIGHT, VALID) VALUES (" + id + ", '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(description) + "', " + relativeWeight + ", " + valid + ")");
                }
                else
                {
                    db.execute("UPDATE PERFORMANCE_LEVEL SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + "= '" + DBColumn.toSql(description) + "', RELATIV_WEIGHT = " + relativeWeight + ", VALID = " + valid + " WHERE ID =" + id);
                }
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION, RELATIV_WEIGHT, VALID + 1 AS VALID FROM PERFORMANCE_LEVEL WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionTypSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteRatingLevel(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("DELETE FROM PERFORMANCE_LEVEL WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "ratingLevelDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Job Expectation Default --------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRatingItemList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM PERFORMANCE_CRITERIA WHERE TITLE_" + lang + " LIKE '%" + title + "%'");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "ratingItemsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetJobExpectationDefaultDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT CRITERIA_REF, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM JOB_EXPECTATION_DEFAULT WHERE CRITERIA_REF = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "jobExpectationReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavejobExpectationDefault(long id, string title, string description)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("UPDATE JOB_EXPECTATION_DEFAULT SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + "= '" + DBColumn.toSql(description) + "' WHERE CRITERIA_REF =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "jobExpectationDefaultSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Rating Items -------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRatingCriteriaDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT PERFORMANCE_CRITERIA.ID, PERFORMANCE_CRITERIA.TITLE_" + lang + " AS TITLE,  PERFORMANCE_CRITERIA.DESCRIPTION_" + lang + " AS DESCRIPTION, FBW_KRITERIUM.ID AS FBW_KRITERIUM "
                                                + "FROM PERFORMANCE_CRITERIA LEFT OUTER JOIN FBW_KRITERIUM ON PERFORMANCE_CRITERIA.ID = FBW_KRITERIUM.PERFORMANCE_CRITERIA_REF WHERE PERFORMANCE_CRITERIA.ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "performanceratigItemReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePerformanceratingItem(long id, string title, string description, long functionRatingLinkId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("UPDATE PERFORMANCE_CRITERIA SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + "= '" + DBColumn.toSql(description) + "' WHERE  ID =" + id);
                db.execute("UPDATE FBW_KRITERIUM SET PERFORMANCE_CRITERIA_REF = " + id + " WHERE ID = " + functionRatingLinkId);
                detailData = db.getDataTable("SELECT PERFORMANCE_CRITERIA.ID, PERFORMANCE_CRITERIA.TITLE_" + lang + " AS TITLE,  PERFORMANCE_CRITERIA.DESCRIPTION_" + lang + " AS DESCRIPTION, FBW_KRITERIUM.ID AS FBW_KRITERIUM "
                                                + "FROM PERFORMANCE_CRITERIA LEFT OUTER JOIN FBW_KRITERIUM ON PERFORMANCE_CRITERIA.ID = FBW_KRITERIUM.PERFORMANCE_CRITERIA_REF WHERE PERFORMANCE_CRITERIA.ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "performanceratigItemSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Functioncriterias --------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetFunctionWaigthingTable(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, FUNKTION_TITLE_" + lang + " AS NAME, CRITERIA_TITLE_" + lang + " AS CRITERIA, WEIGHT, AUTOWEIGHT FROM CRITERIA_FUNCTION_V WHERE FUNCTION_REF = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionWaigthingReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveFunctionCriteriasWeight(long id, bool autoweight, object[] items)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                foreach (object item in items)
                {
                    IDictionary itm = (item as IDictionary);
                    db.execute("UPDATE CRITERIA_FUNCTION_WEIGHT SET WEIGHT=" + itm["weight"] + " where ID= " + itm["Id"] + " AND FUNCTION_REF = " + id);

                }
                db.execute("UPDATE FUNKTION SET AUTOWEIGHT =" + Convert.ToInt32(autoweight) + " WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "functionWaigthingSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Chart --------------------------------------

        private static string[] chartTexts = new string[] { "LEITERABTEILUNG", "NAMEABTEILUNG" };
        private static string[] chartLinks = new string[] { };



        /*
         * Feld TEXT 0 - Freitext (""), 1 - Leiter der Abteilung ("{lookup
         * param1=NAME param2=PERSON param3=ID=%ID}"), 2 - Name der Abteilung
         * ("{lookup param1=TITLE param2=ORGENTITY param3=ID=%ID}"),
         */


        //private static string LINK_KEIN_LINK = "0";
        //private static string LINK_LEITER_ABTEILUNG = "1";
        //private static string LINK_LISTE_MITARBEITER = "2";
        //private static string LINK_ORGANIGRAMM = "3";
        //private static string LINK_FREIER_LINK = "4";
        //private static string LINK_KONTAKTE = "5";
        //private static string LINK_PROJEKTE = "6";
        //private static string LINK_PENDENZENLISTEN = "7";
        //private static string LINK_DOKUMENTENABLAGE = "8";

        private const string LINKVALUE_LEITER_ABTEILUNG = "/Person/DetailFrame.aspx?mode=OELeader&xID=%ID&index=%INDEX";
        private const string LINKVALUE_LISTE_MITARBEITER = "/Person/DetailFrame.aspx?mode=OE&xID=%ID";
        private const string LINKVALUE_ORGANIGRAMM = "/goto.aspx?UID=";
        private const string LINKVALUE_KONTAKTE = "/Contact/ContactDetail.aspx?mode=OE&xID=%ID";
        private const string LINKVALUE_PROJEKTE = "/Project/ProjectDetail.aspx?context=OE&xID=%ID";
        private const string LINKVALUE_PENDENZENLISTEN = "/Tasklist/TaskDetail.aspx?context=OE&xID=%ID";
        private const string LINKVALUE_DOKUMENTENABLAGE = "/Document/Clipboard.aspx?ownerTable=ORGENTITY&ID=%CLIPBOARD_ID";

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetChartList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHART WHERE TITLE_" + lang + " LIKE '%" + title + "%' ORDER BY TITLE_" + lang);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartListReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        private Chart _chart = null;
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetChart(long chartId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                _chart = Organisation.Chart.BuildChart(db, chartId, Global.Config.organisationImageDirectory);
                string now = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
                System.Drawing.Image _img = _chart.GetImageGraph();
                String fullName = Server.MapPath("").Replace("WebService", "reports\\") + "Organigramm_" + chartId + "_" + SessionData.getSessionID(Session) + "_" + now + ".jpg";
                _img.Save(fullName);
                DataColumn path = new DataColumn("path");
                detailData.Columns.Add(path);
                DataColumn width = new DataColumn("imageWidth");
                detailData.Columns.Add(width);
                DataColumn height = new DataColumn("imageHeight");
                detailData.Columns.Add(height);
                DataColumn map = new DataColumn("map");
                detailData.Columns.Add(map);
                DataColumn chartTitle = new DataColumn("chartTitle");
                detailData.Columns.Add(chartTitle);
                DataRow row = detailData.NewRow();
                row["path"] = HttpContext.Current.Request.Url.AbsoluteUri.Replace("WebService/AdminService.asmx/GetChart", "reports/" + "Organigramm_" + chartId + "_" + SessionData.getSessionID(Session) + "_" + now + ".jpg");
                row["imageWidth"] = _img.Width;
                row["imageHeight"] = _img.Height;
                row["chartTitle"] = _chart.Title;
                StringBuilder _map = new StringBuilder(1024);
                //_map.Append("<map name=\"TreeMap\">\n");
                _chart.AppendImageMapInfoAdmin(_map);
                //_map.Append("</map>");
                row["map"] = _map.ToString();
                detailData.Rows.Add(row);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartListReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetChartDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, (SELECT TITLE_" + lang + " FROM ORGANISATION WHERE ID = CHART.ORGANISATION_ID) AS ORGANISATION, CHARTLAYOUT_ID, TEXTLAYOUT_ID, CHARTALIGNMENT_ID FROM CHART WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNodeDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, (SELECT TITLE_" + lang + " FROM ORGENTITY WHERE ID = CHARTNODE.ORGENTITY_ID) AS ORGENTITY, (SELECT TITLE_" + lang + " FROM CHART WHERE ID = CHARTNODE.CHART_ID) AS CHART,"
                                            + "LAYOUT_ID, CHILDLAYOUT_ID, TEXTLAYOUT_ID, CHARTALIGNMENT_ID, TYP, SHOWEMPLOYEES, VERTICAL_ALIGN_OFFSET, GAP_VERTICAL, GAP_HORIZONTAL FROM CHARTNODE WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNodeTexts(long nodeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TEXT_" + lang + " AS TEXT, LINK, ORDNUMBER FROM CHARTTEXT WHERE CHARTNODE_ID = " + nodeId + " AND TYP = 0 ORDER BY ORDNUMBER");
                foreach (DataRow row in detailData.Rows)
                {
                    row["TEXT"] = _map.get("charttext", row["TEXT"].ToString().ToLower());
                    row["LINK"] = GetLinkName(row["LINK"].ToString());
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        private string GetLinkName(string Link)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            string linkName = "";
            if (Link.Length > 0)
            {
                if (Link.StartsWith(LINKVALUE_ORGANIGRAMM))
                {
                    Link = LINKVALUE_ORGANIGRAMM;
                }
                switch (Link)
                {
                    case LINKVALUE_LEITER_ABTEILUNG:
                        linkName = _map.get("charttext", "leiterabteilung");
                        break;
                    case LINKVALUE_LISTE_MITARBEITER:
                        linkName = _map.get("charttext", "listepersonen");
                        break;
                    case LINKVALUE_ORGANIGRAMM:
                        linkName = _map.get("charttext", "linkorganigramm");
                        break;
                    case LINKVALUE_KONTAKTE:
                        linkName = _map.get("charttext", "kontakte");
                        break;
                    case LINKVALUE_PROJEKTE:
                        linkName = _map.get("charttext", "projekte");
                        break;
                    case LINKVALUE_PENDENZENLISTEN:
                        linkName = _map.get("charttext", "pendenzenlisten");
                        break;
                    case LINKVALUE_DOKUMENTENABLAGE:
                        linkName = _map.get("charttext", "dokumentenablage");
                        break;
                    default:
                        linkName = _map.get("charttext", "freilink");
                        break;
                }
            }
            else
            {
                linkName = _map.get("charttext", "keinlink");
            }

            return linkName;

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNodeIconLinks(long nodeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TEXT_" + lang + " AS TEXT, LINK, ORDNUMBER, (SELECT TITLE_" + lang + " FROM CHARTPIKTOLAYOUT WHERE ID = CHARTTEXT.CHARTPIKTOLAYOUT_ID) AS ICON FROM CHARTTEXT WHERE CHARTNODE_ID = " + nodeId + " AND TYP = 1 ORDER BY ORDNUMBER");
                foreach (DataRow row in detailData.Rows)
                {
                    row["TEXT"] = _map.get("charttext", row["TEXT"].ToString().ToLower());
                    row["LINK"] = GetLinkName(row["LINK"].ToString());
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveChartDetail(long chartId, long layoutId, long textLayoutId, long aligId, string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            try
            {
                db.execute("UPDATE CHART SET TITLE_" + lang + " = '" + title + "', CHARTLAYOUT_ID = " + layoutId + ", TEXTLAYOUT_ID = " + textLayoutId + ", CHARTALIGNMENT_ID = " + aligId + " WHERE ID = " + chartId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveNodeDetail(long nodeId, string layoutId, string layoutSubnodesId, string textLayoutId, string aligId, int typ, bool showPerson)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            try
            {
                if (layoutId.Equals("0"))
                    layoutId = "null";
                if (layoutSubnodesId.Equals("0"))
                    layoutSubnodesId = "null";
                if (textLayoutId.Equals("0"))
                    textLayoutId = "null";
                if (aligId.Equals("0"))
                    aligId = "null";
                db.execute("UPDATE CHARTNODE SET LAYOUT_ID = " + layoutId + ", CHILDLAYOUT_ID = " + layoutSubnodesId + ", CHARTALIGNMENT_ID = " + aligId + ", TEXTLAYOUT_ID = " + textLayoutId + ", TYP = " + typ + ", SHOWEMPLOYEES = " + Convert.ToInt32(showPerson) + " WHERE ID = " + nodeId);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateNewChart(long oeId, string title, long layoutId, long textLayoutId, long aligId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            DataTable treeTable = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            try
            {
                treeTable = GetTreeTable(db, oeId, lang);

                long newChartId = db.newId("CHART");
                long organisationId = db.lookup("ID", "ORGANISATiON", "MAINORGANISATION = 1", 0L);
                db.execute("INSERT INTO CHART (ID, CHARTLAYOUT_ID,TEXTLAYOUT_ID,CHARTALIGNMENT_ID,TITLE_" + lang + ",ORGANISATION_ID) values (" + newChartId + ", " + layoutId + ", " + textLayoutId + ", " + aligId + ", '" + DBColumn.toSql(title) + "', " + organisationId + ")");

                long newNodeId = db.newId("CHARTNODE");
                db.execute("INSERT INTO CHARTNODE (ID,CHART_ID,ORGENTITY_ID,ORDNUMBER, SHOWEMPLOYEES) values (" + newNodeId + ", " + newChartId + ", " + oeId + ", 1, 1)");

                long chartTextId;
                chartTextId = db.newId("CHARTTEXT");
                db.execute("INSERT INTO CHARTTEXT(ID, CHARTNODE_ID, ORDNUMBER, TEXT_" + lang + ", TARGETFRAME) values(" + chartTextId + ", " + newNodeId + ", 1, '" + chartTexts[1] + "', '_self')");

                int position = 1;
                string parentId = "";
                foreach (DataRow node in treeTable.Rows)
                {
                    if (parentId.Equals(node["PARENT_ID"].ToString()))
                    {
                        position += 1;
                    }
                    else
                    {
                        position = 1;
                        parentId = node["PARENT_ID"].ToString();
                    }
                    newNodeId = db.newId("CHARTNODE");
                    db.execute("INSERT INTO CHARTNODE (ID,CHART_ID,ORGENTITY_ID,ORDNUMBER, SHOWEMPLOYEES) values (" + newNodeId + ", " + newChartId + ", " + node["ID"].ToString() + ", " + position + ", 1)");

                    chartTextId = db.newId("CHARTTEXT");
                    db.execute("INSERT INTO CHARTTEXT(ID, CHARTNODE_ID, ORDNUMBER, TEXT_" + lang + ", TARGETFRAME) values(" + chartTextId + ", " + newNodeId + ", 1, '" + chartTexts[1] + "', '_self')");


                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHART WHERE ID = " + newChartId);

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDetailsSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        private DataTable GetTreeTable(DBData db, long oeId, string language)
        {
            return db.getDataTable("WITH TREE (ID,  PARENT_ID, LEVEL, TITLE) AS "
                                         + "( "
                                            + "SELECT ID, PARENT_ID, 0 as LEVEL, TITLE_" + language + " "
                                            + "FROM ORGENTITY "
                                            + "WHERE PARENT_ID =" + oeId + " "
                                            + "UNION ALL "
                                            + "SELECT c2.ID, c2.PARENT_ID, tree.LEVEL + 1, c2.TITLE_" + language + " "
                                            + "FROM ORGENTITY c2 "
                                            + "INNER JOIN tree ON tree.ID = c2.PARENT_ID "
                                         + ") "
                                         + "SELECT * "
                                         + "FROM TREE ORDER BY PARENT_ID");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteChartnode(long chartId, long nodeId, bool deleteSubnodes)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();

            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            db.connect();
            try
            {
                long oeId = db.lookup("ORGENTITY_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long parentOeID = db.lookup("PARENT_ID", "ORGENTITY", "ID=" + oeId, 0L);
                //get subOrgentities
                DataTable orgentityTable = db.getDataTable("WITH TREE (ID,  PARENT_ID, LEVEL, TITLE) AS "
                                         + "( "
                                            + "SELECT ID, PARENT_ID, 0 as LEVEL, TITLE_" + lang + " "
                                            + "FROM ORGENTITY "
                                            + "WHERE PARENT_ID =" + oeId + " "
                                            + "UNION ALL "
                                            + "SELECT c2.ID, c2.PARENT_ID, tree.LEVEL + 1, c2.TITLE_" + lang + " "
                                            + "FROM ORGENTITY c2 "
                                            + "INNER JOIN tree ON tree.ID = c2.PARENT_ID "
                                         + ") "
                                         + "SELECT ID "
                                         + "FROM TREE");
                string sql = "(";
                foreach (DataRow oeRow in orgentityTable.Rows)
                {
                    sql += oeRow["ID"].ToString() + ", ";
                }
                if (sql.Length > 1)
                {
                    sql = sql.Substring(0, sql.Length - 2) + ")";

                    DataTable nodeTable = db.getDataTable("SELECT ID FROM CHARTNODE WHERE ORGENTITY_ID IN" + sql + " AND CHART_ID =" + chartId);

                    sql = "(";
                    foreach (DataRow nodeRow in nodeTable.Rows)
                    {
                        sql += nodeRow["ID"].ToString() + ", ";
                    }
                    if (sql.Length > 1)
                    {
                        sql = sql.Substring(0, sql.Length - 2) + ")";

                        //delete subOrgentities
                        db.execute("Delete FROM CHARTTEXT WHERE CHARTNODE_ID IN" + sql);
                        db.execute("Delete FROM CHARTNODE WHERE ID IN" + sql);
                    }
                }
                if (!deleteSubnodes)
                {
                    //delete top orgentity
                    db.execute("Delete FROM CHARTTEXT WHERE CHARTNODE_ID =" + nodeId);
                    db.execute("Delete FROM CHARTNODE WHERE ID  =" + nodeId);

                    //set new ordnumbers
                    DataTable orgFromParent = db.getDataTable("SELECT ID FROM ORGENTITY WHERE PARENT_ID =" + parentOeID);
                    sql = "(";
                    foreach (DataRow oeRow in orgFromParent.Rows)
                    {
                        sql += oeRow["ID"].ToString() + ", ";
                    }
                    sql = sql.Substring(0, sql.Length - 2) + ")";

                    DataTable subNodesFromParent = db.getDataTable("SELECT ID, ORDNUMBER FROM CHARTNODE WHERE ORGENTITY_ID IN " + sql + " AND CHART_ID =" + chartId + " ORDER BY ORDNUMBER");

                    int ordnumber = 1;
                    foreach (DataRow row in subNodesFromParent.Rows)
                    {
                        db.execute("UPDATE CHARTNODE SET ORDNUMBER = " + ordnumber + " WHERE ID = " + row["ID"].ToString());
                        ordnumber += 1;
                    }
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartNodeDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteChart(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                db.execute("delete from charttext where chartnode_id in (select id from chartnode where chart_id =" + id + ")");
                db.execute("delete from chartnode where chart_id =" + id);
                db.execute("delete from chart where id =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveNodeBefore(long nodeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                long chartId = db.lookup("CHART_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long oeId = db.lookup("ORGENTITY_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long parentOeId = db.lookup("PARENT_ID", "ORGENTITY", "ID=" + oeId, 0L);

                DataTable orgentities = db.getDataTable("SELECT ID FROM ORgENTITY WHERE PARENT_ID = " + parentOeId);
                string sql = "(";
                foreach (DataRow oeRow in orgentities.Rows)
                {
                    sql += oeRow["ID"].ToString() + ", ";
                }
                sql = sql.Substring(0, sql.Length - 2) + ")";
                DataTable subNodesFromParent = db.getDataTable("SELECT ID, ORDNUMBER FROM CHARTNODE WHERE ORGENTITY_ID IN " + sql + " AND CHART_ID =" + chartId + " ORDER BY ORDNUMBER");
                for (int i = 0; i < subNodesFromParent.Rows.Count; i++)
                {
                    if (Convert.ToUInt32(subNodesFromParent.Rows[i]["ID"]) == nodeId && i > 0)
                    {
                        int ordNumberNodeBevor = Convert.ToUInt16(subNodesFromParent.Rows[i - 1]["ORDNUMBER"]);
                        long idNodeBevor = Convert.ToUInt32(subNodesFromParent.Rows[i - 1]["ID"]);
                        int ordNumberNodeToMove = Convert.ToUInt16(subNodesFromParent.Rows[i]["ORDNUMBER"]);
                        db.execute("UPDATE CHARTNODE SET ORDNUMBER=" + ordNumberNodeToMove + " WHERE ID =" + idNodeBevor);
                        db.execute("UPDATE CHARTNODE SET ORDNUMBER=" + ordNumberNodeBevor + " WHERE ID =" + nodeId);
                        break;
                    }
                }


            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartMoveNodeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveNodeAfter(long nodeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                long chartId = db.lookup("CHART_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long oeId = db.lookup("ORGENTITY_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long parentOeId = db.lookup("PARENT_ID", "ORGENTITY", "ID=" + oeId, 0L);

                DataTable orgentities = db.getDataTable("SELECT ID FROM ORgENTITY WHERE PARENT_ID = " + parentOeId);
                string sql = "(";
                foreach (DataRow oeRow in orgentities.Rows)
                {
                    sql += oeRow["ID"].ToString() + ", ";
                }
                sql = sql.Substring(0, sql.Length - 2) + ")";
                DataTable subNodesFromParent = db.getDataTable("SELECT ID, ORDNUMBER FROM CHARTNODE WHERE ORGENTITY_ID IN " + sql + " AND CHART_ID =" + chartId + " ORDER BY ORDNUMBER");
                for (int i = 0; i < subNodesFromParent.Rows.Count; i++)
                {
                    if (Convert.ToUInt32(subNodesFromParent.Rows[i]["ID"]) == nodeId && i < subNodesFromParent.Rows.Count - 1)
                    {
                        int ordNumberAfter = Convert.ToUInt16(subNodesFromParent.Rows[i + 1]["ORDNUMBER"]);
                        long idNodeAfter = Convert.ToUInt32(subNodesFromParent.Rows[i + 1]["ID"]);
                        int ordNumberNodeToMove = Convert.ToUInt16(subNodesFromParent.Rows[i]["ORDNUMBER"]);
                        db.execute("UPDATE CHARTNODE SET ORDNUMBER=" + ordNumberNodeToMove + " WHERE ID =" + idNodeAfter);
                        db.execute("UPDATE CHARTNODE SET ORDNUMBER=" + ordNumberAfter + " WHERE ID =" + nodeId);
                        break;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartMoveNodeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string InsertMissingNode(long nodeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                long chartId = db.lookup("CHART_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long oeId = db.lookup("ORGENTITY_ID", "CHARTNODE", "ID=" + nodeId, 0L);


                DataTable oeTreeTable = GetTreeTable(db, oeId, lang);
                DataTable chartNodeTable = db.getDataTable("SELECT ORGENTITY_ID FROM CHARTNODE WHERE CHART_ID = " + chartId);

                foreach (DataRow oeRow in oeTreeTable.Rows)
                {
                    DataRow[] oeInChartNodes = chartNodeTable.Select("ORGENTITY_ID =" + oeRow["ID"].ToString());
                    if (oeInChartNodes.Count() == 0)
                    {
                        long newNodeId = db.newId("CHARTNODE");
                        db.execute("INSERT INTO CHARTNODE (ID,CHART_ID,ORGENTITY_ID,ORDNUMBER, SHOWEMPLOYEES) values (" + newNodeId + ", " + chartId + ", " + oeRow["ID"].ToString() + ", 999, 1)");

                        long chartTextId = db.newId("CHARTTEXT");
                        db.execute("INSERT INTO CHARTTEXT(ID, CHARTNODE_ID, ORDNUMBER, TEXT_" + lang + ", TARGETFRAME) values(" + chartTextId + ", " + newNodeId + ", 1, '" + chartTexts[1] + "', '_self')");

                        //set new ordnumbers
                        long parentOeId = db.lookup("PARENT_ID", "ORGENTITY", "ID=" + oeRow["ID"].ToString(), 0L);
                        DataTable orgFromParent = db.getDataTable("SELECT ID FROM ORGENTITY WHERE PARENT_ID =" + parentOeId);
                        string sql = "(";
                        foreach (DataRow oeRow_t in orgFromParent.Rows)
                        {
                            sql += oeRow_t["ID"].ToString() + ", ";
                        }
                        sql = sql.Substring(0, sql.Length - 2) + ")";

                        DataTable subNodesFromParent = db.getDataTable("SELECT ID, ORDNUMBER FROM CHARTNODE WHERE ORGENTITY_ID IN " + sql + " AND CHART_ID =" + chartId + " ORDER BY ORDNUMBER");

                        int ordnumber = 1;
                        foreach (DataRow row in subNodesFromParent.Rows)
                        {
                            db.execute("UPDATE CHARTNODE SET ORDNUMBER = " + ordnumber + " WHERE ID = " + row["ID"].ToString());
                            ordnumber += 1;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartMoveNodeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveNodeLink(long id, long nodeId, int textIndex, int linkIndex, string linkChartId, bool newWindow, long layoutId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string ordnumber = db.lookup("MAX(ORDNUMBER)", "CHARTTEXT", "CHARTNODE_ID = " + nodeId + " AND TYP = 0").ToString();
                if (ordnumber.Length > 0)
                    ordnumber = (Convert.ToInt16(ordnumber) + 1).ToString();
                else
                    ordnumber = "1";
                string text = "'" + chartTexts[textIndex] + "'";
                string window = "'_self'";
                if (newWindow)
                    window = "'_blank'";

                string layout;

                if (layoutId == 0)
                    layout = "null";
                else
                    layout = layoutId.ToString();
                string link = "null";
                switch (linkIndex)
                {
                    case (0):
                        link = "null";
                        break;
                    case (1):
                        link = "'" + LINKVALUE_LEITER_ABTEILUNG + "'";
                        break;
                    case (2):
                        link = "'" + LINKVALUE_LISTE_MITARBEITER + "'";
                        break;
                    case (3):
                        link = "'" + LINKVALUE_DOKUMENTENABLAGE + "'";
                        break;
                    case (4):
                        if (linkChartId != "null")
                            link = "'" + LINKVALUE_ORGANIGRAMM + db.lookup("UID", "CHART", "ID=" + linkChartId).ToString() + "'";
                        else
                            link = "null";
                        break;
                }


                if (id == 0)
                {
                    id = db.newId("CHARTTEXT");
                    db.execute("INSERT INTO CHARTTEXT(ID, ORDNUMBER, CHARTNODE_ID, TEXT_" + lang + ", LINK, TARGETFRAME, LAYOUT_ID, TYP) VALUES(" + id + ", " + ordnumber + ", " + nodeId + ", " + text + " ," + link + ", " + window + ", " + layout + ", 0)");
                }
                else
                {
                    db.execute("UPDATE CHARTTEXT SET TEXT_" + lang + " =  " + text + ", LINK = " + link + ", TARGETFRAME = " + window + ",  LAYOUT_ID = " + layout + " WHERE ID = " + id);
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartSaveNodeLinkFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"linkID\":\"" + id + "\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNodeLinkData(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT TEXT_" + lang + " AS TEXT, TARGETFRAME, LAYOUT_ID, LINK FROM CHARTTEXT WHERE ID = " + id);
                string text = detailData.Rows[0]["TEXT"].ToString();
                string targetframe = detailData.Rows[0]["TARGETFRAME"].ToString();
                string layoutId = detailData.Rows[0]["LAYOUT_ID"].ToString();
                string link = detailData.Rows[0]["LINK"].ToString();
                DataColumn linkChartId = new DataColumn("LINK_CHART_ID");
                detailData.Columns.Add(linkChartId);

                if (text.Length > 0)
                {
                    for (int i = 0; i < chartTexts.Length; i++)
                    {
                        if (chartTexts[i].Equals(text))
                            detailData.Rows[0]["TEXT"] = i;
                    }
                }
                if (targetframe.Equals("_self"))
                {
                    detailData.Rows[0]["TARGETFRAME"] = 0;
                }
                else
                    detailData.Rows[0]["TARGETFRAME"] = 1;
                if (layoutId.Length == 0)
                {
                    detailData.Rows[0]["LAYOUT_ID"] = 0;
                }
                if (link.Length == 0)
                {
                    detailData.Rows[0]["LINK"] = 0;
                    detailData.Rows[0]["LINK_CHART_ID"] = 0;
                }
                else
                {
                    long chartLinkId = 0;
                    if (detailData.Rows[0]["LINK"].ToString().StartsWith(LINKVALUE_ORGANIGRAMM))
                    {
                        chartLinkId = db.lookup("ID", "CHART", "UID = " + detailData.Rows[0]["LINK"].ToString().Remove(0, LINKVALUE_ORGANIGRAMM.Length).ToString(), 0L);
                        detailData.Rows[0]["LINK"] = LINKVALUE_ORGANIGRAMM;
                    }

                    switch (detailData.Rows[0]["LINK"])
                    {
                        case (LINKVALUE_LEITER_ABTEILUNG):
                            detailData.Rows[0]["LINK"] = 1;
                            break;
                        case (LINKVALUE_LISTE_MITARBEITER):
                            detailData.Rows[0]["LINK"] = 2;
                            break;
                        case (LINKVALUE_DOKUMENTENABLAGE):
                            detailData.Rows[0]["LINK"] = 3;
                            break;
                        case (LINKVALUE_ORGANIGRAMM):
                            detailData.Rows[0]["LINK"] = 4;
                            detailData.Rows[0]["LINK_CHART_ID"] = chartLinkId;
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartReadNodeLinkFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNodePiktoData(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT TEXT_" + lang + " AS TEXT, TARGETFRAME, CHARTPIKTOLAYOUT_ID AS LAYOUT_ID, LINK FROM CHARTTEXT WHERE ID = " + id);
                string text = detailData.Rows[0]["TEXT"].ToString();
                string targetframe = detailData.Rows[0]["TARGETFRAME"].ToString();
                string layoutId = detailData.Rows[0]["LAYOUT_ID"].ToString();
                string link = detailData.Rows[0]["LINK"].ToString();
                DataColumn linkChartId = new DataColumn("LINK_CHART_ID");
                detailData.Columns.Add(linkChartId);

                if (text.Equals("LEITERABTEILUNG") || text.Equals("NAMEABTEILUNG"))
                {
                    for (int i = 0; i < chartTexts.Length; i++)
                    {
                        if (chartTexts[i].Equals(text))
                            detailData.Rows[0]["TEXT"] = i;
                    }
                }
                else

                if (targetframe.Equals("_self"))
                {
                    detailData.Rows[0]["TARGETFRAME"] = 0;
                }
                else
                    detailData.Rows[0]["TARGETFRAME"] = 1;
                if (layoutId.Length == 0)
                {
                    detailData.Rows[0]["LAYOUT_ID"] = 0;
                }
                if (link.Length == 0)
                {
                    detailData.Rows[0]["LINK"] = 0;
                    detailData.Rows[0]["LINK_CHART_ID"] = 0;
                }
                else
                {
                    long chartLinkId = 0;
                    if (detailData.Rows[0]["LINK"].ToString().StartsWith(LINKVALUE_ORGANIGRAMM))
                    {
                        chartLinkId = db.lookup("ID", "CHART", "UID = " + detailData.Rows[0]["LINK"].ToString().Remove(0, LINKVALUE_ORGANIGRAMM.Length).ToString(), 0L);
                        detailData.Rows[0]["LINK"] = LINKVALUE_ORGANIGRAMM;
                    }

                    switch (detailData.Rows[0]["LINK"])
                    {
                        case (LINKVALUE_LEITER_ABTEILUNG):
                            detailData.Rows[0]["LINK"] = 1;
                            break;
                        case (LINKVALUE_LISTE_MITARBEITER):
                            detailData.Rows[0]["LINK"] = 2;
                            break;
                        case (LINKVALUE_DOKUMENTENABLAGE):
                            detailData.Rows[0]["LINK"] = 3;
                            break;
                        case (LINKVALUE_ORGANIGRAMM):
                            detailData.Rows[0]["LINK"] = 4;
                            detailData.Rows[0]["LINK_CHART_ID"] = chartLinkId;
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartReadNodeLinkFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePiktoLink(long id, long nodeId, int textIndex, int linkIndex, string linkChartId, bool newWindow, long layoutId, string freeText)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string ordnumber = db.lookup("MAX(ORDNUMBER)", "CHARTTEXT", "CHARTNODE_ID = " + nodeId + " AND TYP = 1").ToString();
                if (ordnumber.Length > 0)
                    ordnumber = (Convert.ToInt16(ordnumber) + 1).ToString();
                else
                    ordnumber = "1";
                string text;
                if (freeText.Length > 0)
                {
                    text = "'" + DBColumn.toSql(freeText) + "'";
                }
                else
                {
                    text = "'" + chartTexts[textIndex] + "'";
                }
                string window = "'_self'";
                if (newWindow)
                    window = "'_blank'";

                string layout;

                if (layoutId == 0)
                    layout = "null";
                else
                    layout = layoutId.ToString();
                string link = "null";
                switch (linkIndex)
                {
                    case (0):
                        link = "null";
                        break;
                    case (1):
                        link = "'" + LINKVALUE_LEITER_ABTEILUNG + "'";
                        break;
                    case (2):
                        link = "'" + LINKVALUE_LISTE_MITARBEITER + "'";
                        break;
                    case (3):
                        link = "'" + LINKVALUE_DOKUMENTENABLAGE + "'";
                        break;
                    case (4):
                        if (linkChartId != "null")
                            link = "'" + LINKVALUE_ORGANIGRAMM + db.lookup("UID", "CHART", "ID=" + linkChartId).ToString() + "'";
                        else
                            link = "null";
                        break;
                }


                if (id == 0)
                {
                    id = db.newId("CHARTTEXT");
                    db.execute("INSERT INTO CHARTTEXT(ID, ORDNUMBER, CHARTNODE_ID, TEXT_" + lang + ", LINK, TARGETFRAME, CHARTPIKTOLAYOUT_ID, TYP) VALUES(" + id + ", " + ordnumber + ", " + nodeId + ", " + text + " ," + link + ", " + window + ", " + layout + ", 1)");
                }
                else
                {
                    db.execute("UPDATE CHARTTEXT SET TEXT_" + lang + " =  " + text + ", LINK = " + link + ", TARGETFRAME = " + window + ",  CHARTPIKTOLAYOUT_ID = " + layout + " WHERE ID = " + id);
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartSaveNodeLinkFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj("[{\"linkID\":\"" + id + "\"}]");
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteNodeLink(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("DELETE FROM CHARTTEXT WHERE ID=" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartDeleteNodeLinkFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveLinkUp(long id, long nodeId, int typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                DataTable linkTabel = db.getDataTable("SELECT ID, ORDNUMBER FROM CHARTTEXT WHERE CHARTNODE_ID = " + nodeId + " AND TYP =" + typ + " ORDER BY ORDNUMBER");
                if (linkTabel.Rows.Count >= 2)
                {
                    for (int i = 0; i < linkTabel.Rows.Count; i++)
                    {
                        if (Convert.ToUInt32(linkTabel.Rows[i]["ID"]) == id && i > 0)
                        {
                            int ordNumberBevor = Convert.ToUInt16(linkTabel.Rows[i - 1]["ORDNUMBER"]);
                            long idNodeBevor = Convert.ToUInt32(linkTabel.Rows[i - 1]["ID"]);
                            int ordNumberNodeToMove = Convert.ToUInt16(linkTabel.Rows[i]["ORDNUMBER"]);
                            db.execute("UPDATE CHARTTEXT SET ORDNUMBER=" + ordNumberNodeToMove + " WHERE ID =" + idNodeBevor);
                            db.execute("UPDATE CHARTTEXT SET ORDNUMBER=" + ordNumberBevor + " WHERE ID =" + id);
                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartMoveNodeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string MoveLinkDown(long id, long nodeId, int typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                DataTable linkTabel = db.getDataTable("SELECT ID, ORDNUMBER FROM CHARTTEXT WHERE CHARTNODE_ID = " + nodeId + " AND TYP =" + typ + " ORDER BY ORDNUMBER");
                if (linkTabel.Rows.Count >= 2)
                {
                    for (int i = 0; i < linkTabel.Rows.Count; i++)
                    {
                        if (Convert.ToUInt32(linkTabel.Rows[i]["ID"]) == id && i < linkTabel.Rows.Count - 1)
                        {
                            int ordNumberAfter = Convert.ToUInt16(linkTabel.Rows[i + 1]["ORDNUMBER"]);
                            long idNodeAfter = Convert.ToUInt32(linkTabel.Rows[i + 1]["ID"]);
                            int ordNumberNodeToMove = Convert.ToUInt16(linkTabel.Rows[i]["ORDNUMBER"]);
                            db.execute("UPDATE CHARTTEXT SET ORDNUMBER=" + ordNumberNodeToMove + " WHERE ID =" + idNodeAfter);
                            db.execute("UPDATE CHARTTEXT SET ORDNUMBER=" + ordNumberAfter + " WHERE ID =" + id);
                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartMoveNodeFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CopyLinkToSubnotes(long id, long nodeId)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                long chartId = db.lookup("CHART_ID", "CHARTNODE", "ID=" + nodeId, 0L);
                long oeId = db.lookup("ORGENTITY_ID", "CHARTNODE", "ID=" + nodeId, 0L);


                DataTable oeTreeTable = GetTreeTable(db, oeId, lang);
                DataTable chartNodeTable = db.getDataTable("SELECT ORGENTITY_ID, ID FROM CHARTNODE WHERE CHART_ID = " + chartId);
                DataTable origLinkRow = db.getDataTable("SELECT * FROM CHARTTEXT WHERE ID = " + id);
                string textDe = origLinkRow.Rows[0]["TEXT_DE"].ToString();
                string textFr = origLinkRow.Rows[0]["TEXT_FR"].ToString();
                string textEn = origLinkRow.Rows[0]["TEXT_EN"].ToString();
                string textIt = origLinkRow.Rows[0]["TEXT_IT"].ToString();
                string link = origLinkRow.Rows[0]["LINK"].ToString();
                string targetframe = origLinkRow.Rows[0]["TARGETFRAME"].ToString();
                int ordnumber = 999;
                string layoutId = origLinkRow.Rows[0]["LAYOUT_ID"].ToString();
                string chartPiktoLayoutId = origLinkRow.Rows[0]["CHARTPIKTOLAYOUT_ID"].ToString();
                string typ = origLinkRow.Rows[0]["TYP"].ToString();

                foreach (DataRow oeRow in oeTreeTable.Rows)
                {
                    DataRow[] oeInChartNodes = chartNodeTable.Select("ORGENTITY_ID =" + oeRow["ID"].ToString());
                    if (oeInChartNodes.Count() > 0)
                    {
                        long chartTextId = db.newId("CHARTTEXT");
                        string sql = "INSERT INTO CHARTTEXT(ID, TEXT_DE, TEXT_FR, TEXT_EN, TEXT_IT, ";
                        if (link.Length > 0)
                            sql += "LINK, ";
                        sql += "TARGETFRAME, ORDNUMBER, ";
                        if (layoutId.Length > 0)
                            sql += "LAYOUT_ID, ";
                        sql += "CHARTNODE_ID, ";
                        if (chartPiktoLayoutId.Length > 0)
                            sql += "CHARTPIKTOLAYOUT_ID, ";
                        sql += "TYP) VALUES(" + chartTextId + ", '" + textDe + "', '" + textFr + "', '" + textEn + "', '" + textIt + "', ";
                        if (link.Length > 0)
                            sql += "'" + link + "', ";
                        sql += "'" + targetframe + "', " + ordnumber + ", ";
                        if (layoutId.Length > 0)
                            sql += layoutId + ", ";
                        sql += oeInChartNodes[0]["ID"] + ", ";
                        if (chartPiktoLayoutId.Length > 0)
                            sql += chartPiktoLayoutId + ", ";
                        sql += typ + ")";

                        db.execute(sql);

                        //set new ordnumbers

                        DataTable linkTable = db.getDataTable("SELECT CHARTTEXT.ID, CHARTTEXT.ORDNUMBER FROM CHARTTEXT INNER JOIN CHARTNODE ON CHARTTEXT.CHARTNODE_ID = CHARTNODE.ID WHERE CHARTNODE.CHART_ID = " + chartId + " AND CHARTNODE.ID = " + oeInChartNodes[0]["ID"] + " ORDER BY  CHARTTEXT.ORDNUMBER");

                        int i = 1;
                        foreach (DataRow linkRow in linkTable.Rows)
                        {
                            db.execute("UPDATE CHARTTEXT SET ORDNUMBER = " + i + " WHERE ID = " + linkRow["ID"]);
                            i++;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "chartLinkCopyFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTextLayoutList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTTEXTLAYOUT WHERE TITLE_" + lang + " LIKE '%" + title + "%' ORDER BY TITLE_" + lang);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "textlayoutReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTextlayoutDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, FONTFAMILY, FONTSIZE, FONTSTYLE, STR(FONTCOLOR) AS FONTCOLOR, HORIZONTAL_ALIGN  FROM CHARTTEXTLAYOUT WHERE ID=" + id);
                detailData.Columns["FONTCOLOR"].ReadOnly = false;
                detailData.Rows[0]["FONTCOLOR"] = ColorTranslator.ToHtml(Color.FromArgb(Convert.ToInt32(detailData.Rows[0]["FONTCOLOR"])));

            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "textlayoutReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveTextLayoutDetail(long id, string name, int align, string fontFamily, int fontsize, bool bold, bool italic, string color)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                int fontstyle = 0;
                if (bold)
                    fontstyle = 1;
                if (italic)
                    fontstyle = 2;
                if (bold && italic)
                    fontstyle = 3;

                int ArgbColor = ColorTranslator.FromHtml(color).ToArgb();


                if (id == 0)
                {
                    id = db.newId("CHARTTEXTLAYOUT");
                    db.execute("INSERT INTO CHARTTEXTLAYOUT (ID, TITLE_" + lang + ", FONTFAMILY, FONTSIZE, FONTSTYLE, FONTCOLOR, HORIZONTAL_ALIGN) VALUES(" + id + ", '" + DBColumn.toSql(name) + "', '" + DBColumn.toSql(fontFamily) + "', " + fontsize + ", " + fontstyle + ", " + ArgbColor + ", " + align + ")");
                }
                else
                {
                    db.execute("UPDATE CHARTTEXTLAYOUT SET TITLE_" + lang + " = '" + DBColumn.toSql(name) + "', FONTFAMILY = '" + DBColumn.toSql(fontFamily) + "', FONTSTYLE = " + fontstyle + ", FONTCOLOR = " + ArgbColor + ", FONTSIZE =" + fontsize + ", HORIZONTAL_ALIGN = " + align + " WHERE id=" + id);
                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTTEXTLAYOUT WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "textlayoutReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteTextLayout(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("DELETE FROM CHARTTEXTLAYOUT WHERE ID =" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "textLayoutDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOrgentityLayoutList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTNODELAYOUT WHERE TITLE_" + lang + " LIKE '%" + title + "%' ORDER BY TITLE_" + lang);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentitylayoutReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetOrgentityLayoutDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, IMAGE, NODEWIDTH, NODEHEIGHT, LINEWIDTH,  STR(LINECOLOR) AS LINECOLOR, PADDING_TOP, PADDING_LEFT, PADDING_RIGHT, STR(BACKGROUNDCOLOR) AS BACKGROUNDCOLOR FROM CHARTNODELAYOUT WHERE ID=" + id);
                detailData.Columns["LINECOLOR"].ReadOnly = false;
                detailData.Rows[0]["LINECOLOR"] = ColorTranslator.ToHtml(Color.FromArgb(Convert.ToInt32(detailData.Rows[0]["LINECOLOR"])));
                detailData.Columns["BACKGROUNDCOLOR"].ReadOnly = false;
                detailData.Rows[0]["BACKGROUNDCOLOR"] = ColorTranslator.ToHtml(Color.FromArgb(Convert.ToInt32(detailData.Rows[0]["BACKGROUNDCOLOR"])));
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "orgentitylayoutReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveNodeLayout(long id, string name, string picture, string width, string height, string paddingTop, string paddingLeft, string paddingRight, string lineWidth, string lineColor, string backgroundcolor)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                int ArgbLineColor = ColorTranslator.FromHtml(lineColor).ToArgb();
                int ArgbBackgroundColor = ColorTranslator.FromHtml(backgroundcolor).ToArgb();
                string sql = "";

                if (id == 0)
                {
                    id = db.newId("CHARTNODELAYOUT");
                    sql += "INSERT INTO CHARTNODELAYOUT (ID, TITLE_" + lang + ", IMAGE";
                    if (width.Length > 0)
                        sql += ", NODEWIDTH";
                    if (height.Length > 0)
                        sql += ", NODEHEIGHT";
                    if (paddingTop.Length > 0)
                        sql += ", PADDING_TOP";
                    if (paddingLeft.Length > 0)
                        sql += ", PADDING_LEFT";
                    if (paddingRight.Length > 0)
                        sql += ", PADDING_RIGHT";
                    if (lineWidth.Length > 0)
                        sql += ", LINEWIDTH";
                    sql += ", LINECOLOR, BACKGROUNDCOLOR) VALUES(" + id + ", '" + DBColumn.toSql(name) + "', '" + DBColumn.toSql(picture) + "'";
                    if (width.Length > 0)
                        sql += ", " + width;
                    if (height.Length > 0)
                        sql += ", " + height;
                    if (paddingTop.Length > 0)
                        sql += ", " + paddingTop;
                    if (paddingLeft.Length > 0)
                        sql += ", " + paddingLeft;
                    if (paddingRight.Length > 0)
                        sql += ", " + paddingRight;
                    if (lineWidth.Length > 0)
                        sql += ", " + lineWidth;
                    sql += ", " + ArgbLineColor + ", " + ArgbBackgroundColor + ")";

                    db.execute(sql);
                }
                else
                {
                    sql += "UPDATE CHARTNODELAYOUT SET TITLE_" + lang + " = '" + DBColumn.toSql(name) + "', IMAGE = '" + DBColumn.toSql(picture) + "'";
                    if (width.Length > 0)
                        sql += ", NODEWIDTH = " + width;
                    if (height.Length > 0)
                        sql += ", NODEHEIGHT =" + height;
                    if (paddingTop.Length > 0)
                        sql += ", PADDING_TOP =" + paddingTop;
                    if (paddingLeft.Length > 0)
                        sql += ", PADDING_LEFT =" + paddingLeft;
                    if (paddingRight.Length > 0)
                        sql += ", PADDING_RIGHT =" + paddingRight;
                    if (lineWidth.Length > 0)
                        sql += ", LINEWIDTH =" + lineWidth;
                    sql += ", LINECOLOR = " + ArgbLineColor + ", BACKGROUNDCOLOR = " + ArgbBackgroundColor + " WHERE ID = " + id;

                    db.execute(sql);
                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM CHARTNODELAYOUT WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "nodeLayoutSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteNodeLayout(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("DELETE FROM CHARTNODELAYOUT WHERE ID =" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "nodeLayoutDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveDemandDetail(long id, string title, string ordnumber, string mnemo, string description)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string sql = "";

                if (id == 0)
                {
                    id = db.newId("TRAINING_DEMAND");
                    sql += "INSERT INTO TRAINING_DEMAND (ID, TITLE_" + lang;
                    if (ordnumber.Length > 0)
                        sql += ", NUMBER";
                    if (mnemo.Length > 0)
                        sql += ", MNEMO_" + lang;
                    if (description.Length > 0)
                        sql += ", DESCRIPTION_" + lang;
                    sql += ") VALUES(" + id + ", '" + DBColumn.toSql(title) + "'";
                    if (mnemo.Length > 0)
                        sql += ", '" + DBColumn.toSql(mnemo) + "'";
                    if (ordnumber.Length > 0)
                        sql += ", " + ordnumber;
                    if (description.Length > 0)
                        sql += ", '" + DBColumn.toSql(description) + "'";
                    sql += ")";

                    db.execute(sql);
                }
                else
                {
                    sql += "UPDATE TRAINING_DEMAND SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "'";
                    if (ordnumber.Length > 0)
                        sql += ", NUMBER = " + ordnumber;
                    if (mnemo.Length > 0)
                        sql += ", MNEMO_" + lang + "= '" + DBColumn.toSql(mnemo) + "'";
                    if (description.Length > 0)
                        sql += ", DESCRIPTION_" + lang + "= '" + DBColumn.toSql(description) + "'";
                    sql += " WHERE ID = " + id;

                    db.execute(sql);
                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM TRAINING_DEMAND WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "demandDetailSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteTrainingDemand(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("DELETE FROM TRAINING_DEMAND WHERE ID =" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDemandDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ MbO --------------------------------------
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetMboRoundList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM OBJECTIVE_TURN WHERE TITLE_" + lang + " LIKE '%" + title + "%' ORDER BY TITLE_" + lang);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "mboRoundReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRoundDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION, STARTDATE, ENDDATE FROM OBJECTIVE_TURN WHERE ID = " + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "mboRoundReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveRoundDetail(long id, string title, string description, string start, string end)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string sql = "";

                if (id == 0)
                {
                    id = db.newId("OBJECTIVE_TURN");
                    sql += "INSERT INTO OBJECTIVE_TURN (ID, TITLE_" + lang + ", DESCRIPTION_" + lang;
                    if (start.Length > 0)
                        sql += ", STARTDATE";
                    if (end.Length > 0)
                        sql += ", ENDDATE";
                    sql += ") VALUES(" + id + ", '" + DBColumn.toSql(title) + "', '" + DBColumn.toSql(description) + "'";
                    if (start.Length > 0)
                        sql += ", '" + DateTime.ParseExact(start, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (end.Length > 0)
                        sql += ", '" + DateTime.ParseExact(end, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    sql += ")";
                    db.execute(sql);
                }
                else
                {
                    sql += "UPDATE OBJECTIVE_TURN SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "', DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "'";
                    if (start.Length > 0)
                        sql += ", STARTDATE= '" + DateTime.ParseExact(start, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (end.Length > 0)
                        sql += ", ENDDATE= '" + DateTime.ParseExact(end, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    sql += " WHERE ID = " + id;

                    db.execute(sql);
                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM OBJECTIVE_TURN WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "roundDetailSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteRound(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("DELETE FROM OBJECTIVE_TURN WHERE ID =" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "roundDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDetailDataConfiguration()
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT WERT, TITLE FROM PROPERTY WHERE GRUPPE = 'mbo' AND TITLE IN ('turn','validation_from','validation_to','objectiveFilter')");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "mboConfigurationReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveDetailDataConfiguration(long turn, int objectiveFilter, string validation_from, string validation_to)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", false);
            db.connect();
            try
            {
                string filter = "";
                if (objectiveFilter == 0)
                    filter = "5";
                if (objectiveFilter == 1)
                    filter = "0,1,2,3,4,5";

                db.execute("UPDATE PROPERTY SET WERT =" + turn + " WHERE GRUPPE = 'mbo' AND TITLE = 'turn'");
                db.execute("UPDATE PROPERTY SET WERT ='" + filter + "' WHERE GRUPPE = 'mbo' AND TITLE = 'objectiveFilter'");
                db.execute("UPDATE PROPERTY SET WERT ='" + DateTime.ParseExact(validation_from, "dd.MM.yyyy", culture).ToString("MMM d, yyyy h:mm:ss tt", culture) + "' WHERE GRUPPE = 'mbo' AND TITLE = 'validation_from'");
                db.execute("UPDATE PROPERTY SET WERT ='" + DateTime.ParseExact(validation_to, "dd.MM.yyyy", culture).ToString("MMM d, yyyy h:mm:ss tt", culture) + "' WHERE GRUPPE = 'mbo' AND TITLE = 'validation_to'");
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "mboConfigurationSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        //------------------ Wage --------------------------------------
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetWageVariantList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG AS TITLE FROM VARIANTE WHERE BEZEICHNUNG LIKE '%" + title + "%' ORDER BY BEZEICHNUNG");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "wageVariantReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetWageVariantDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG AS TITLE, HAUPTVARIANTE, AUSTRITTBESCHRAENKUNG, EINTRITTBESCHRAENKUNG, FIX_POINT_VALUE FROM VARIANTE WHERE ID=" + id + " ORDER BY BEZEICHNUNG");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "wageVariantReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteWageVariant(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                db.execute("DELETE FROM VARIANTE WHERE ID =" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "dataDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveWageVariant(long id, string title, bool hauptvariante, double fix_point_value, string austrittbeschraenkung, string eintrittbeschraenkung)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string sql = "";

                if (id == 0)
                {
                    long lohnrundeId = db.lookup("MAX(ID)", "LOHNRUNDE", "", 0L);
                    long organisationId = db.lookup("ID", "ORGANISATION", "MAINORGANISATION = 1", 0L);
                    id = db.newId("VARIANTE");
                    sql += "INSERT INTO VARIANTE (ID, BEZEICHNUNG, LEVEL_1_AKTIVIERT, ORGANISATION_ID, LOHNRUNDE_ID, HAUPTVARIANTE, FIX_POINT_VALUE";
                    if (austrittbeschraenkung.Length > 0)
                        sql += ", AUSTRITTBESCHRAENKUNG";
                    if (eintrittbeschraenkung.Length > 0)
                        sql += ", EINTRITTBESCHRAENKUNG";
                    sql += ") VALUES(" + id + ", '" + DBColumn.toSql(title) + "', 1, " + organisationId + ", " + lohnrundeId + ", " + Convert.ToInt32(hauptvariante) + ", " + fix_point_value;
                    if (austrittbeschraenkung.Length > 0)
                        sql += ", '" + DateTime.ParseExact(austrittbeschraenkung, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (eintrittbeschraenkung.Length > 0)
                        sql += ", '" + DateTime.ParseExact(eintrittbeschraenkung, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    sql += ")";

                    db.execute(sql);
                }
                else
                {
                    sql += "UPDATE VARIANTE SET BEZEICHNUNG= '" + DBColumn.toSql(title) + "', HAUPTVARIANTE= " + Convert.ToInt32(hauptvariante) + ", FIX_POINT_VALUE = " + fix_point_value;
                    if (austrittbeschraenkung.Length > 0)
                        sql += ", AUSTRITTBESCHRAENKUNG = '" + DateTime.ParseExact(austrittbeschraenkung, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (eintrittbeschraenkung.Length > 0)
                        sql += ", EINTRITTBESCHRAENKUNG = '" + DateTime.ParseExact(eintrittbeschraenkung, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    sql += " WHERE ID = " + id;

                    db.execute(sql);
                }

                detailData = db.getDataTable("SELECT ID, BEZEICHNUNG AS TITLE FROM VARIANTE WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "wageVariantDetailSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }
        //------------------ Training ----------------------------------
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTrainingList(string title, string place, string trainer)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string sql = "SELECT ID, TITLE_" + lang + " AS TITLE FROM TRAINING WHERE TITLE_" + lang + " LIKE '%" + title + "%' ";
                if (place.Length > 0)
                    sql += "AND INSTRUCTOR LIKE '%" + trainer + "%' ";
                if (trainer.Length > 0)
                    sql += "AND LOCATION  LIKE '%" + place + "%' ";
                sql += "ORDER BY TITLE_" + lang;

                detailData = db.getDataTable(sql);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingListReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTrainingDetailData(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, ORDNUMBER, TRAININGGROUP_ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION, VALID_FROM, VALID_TO, COST_EXTERNAL, COST_INTERNAL, LOCATION, PARTICIPANT_NUMBER, INSTRUCTOR FROM TRAINING WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDetailReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTrainingGroupDetailData(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, ORDNUMBER, PARENT_ID, TITLE_" + lang + " AS TITLE, DESCRIPTION_" + lang + " AS DESCRIPTION FROM TRAININGGROUP WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDetailReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveTrainingDetail(long id, long parentId, string title, string description, string validFrom, string validTo, string costExternal, string location, string costInternal, string participantNumber, string instructor)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string sql = "";

                if (id == 0)
                {
                    id = db.newId("TRAINING");
                    sql += "INSERT INTO TRAINING (ID, TRAININGGROUP_ID, TITLE_" + lang;
                    if (description.Length > 0)
                        sql += ", DESCRIPTION_" + lang;
                    if (validFrom.Length > 0)
                        sql += ", VALID_FROM";
                    if (validTo.Length > 0)
                        sql += ", VALID_TO";
                    if (costExternal.Length > 0)
                        sql += ", COST_EXTERNAL";
                    if (costInternal.Length > 0)
                        sql += ", COST_INTERNAL";
                    if (location.Length > 0)
                        sql += ", LOCATION";
                    if (participantNumber.Length > 0)
                        sql += ", PARTICIPANT_NUMBER";
                    if (instructor.Length > 0)
                        sql += ", INSTRUCTOR";
                    sql += ") VALUES(" + id + ", " + parentId + ", '" + DBColumn.toSql(title) + "'";
                    if (description.Length > 0)
                        sql += ", '" + DBColumn.toSql(description) + "'";
                    if (validFrom.Length > 0)
                        sql += ", '" + DateTime.ParseExact(validFrom, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (validTo.Length > 0)
                        sql += ", '" + DateTime.ParseExact(validTo, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (costExternal.Length > 0)
                        sql += ", " + costExternal;
                    if (costInternal.Length > 0)
                        sql += ", " + costInternal;
                    if (location.Length > 0)
                        sql += ", '" + DBColumn.toSql(location) + "'";
                    if (participantNumber.Length > 0)
                        sql += ", " + participantNumber;
                    if (instructor.Length > 0)
                        sql += ", '" + DBColumn.toSql(instructor) + "'";
                    sql += ")";

                    db.execute(sql);
                }
                else
                {
                    sql += "UPDATE TRAINING SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "'";
                    if (description.Length > 0)
                        sql += ", DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "'";
                    if (validFrom.Length > 0)
                        sql += ", VALID_FROM = '" + DateTime.ParseExact(validFrom, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";
                    if (validTo.Length > 0)
                        sql += ", VALID_TO = '" + DateTime.ParseExact(validTo, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("MM-dd-yyyy") + "'";

                    if (costExternal.Length > 0)
                        sql += ", COST_EXTERNAL = " + costExternal;
                    if (costInternal.Length > 0)
                        sql += ", COST_INTERNAL = " + costInternal;
                    if (location.Length > 0)
                        sql += ", LOCATION = '" + DBColumn.toSql(location) + "'";
                    if (participantNumber.Length > 0)
                        sql += ", PARTICIPANT_NUMBER = " + participantNumber;
                    if (instructor.Length > 0)
                        sql += ", INSTRUCTOR = '" + DBColumn.toSql(instructor) + "'";
                    sql += " WHERE ID = " + id;

                    db.execute(sql);
                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM TRAINING WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDetailSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveTrainingGroupDetail(long id, long parentId, string title, string description)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                string sql = "";

                if (id == 0)
                {
                    id = db.newId("TRAININGGROUP");
                    long rootId = db.lookup("ID", "TRAININGGROUP", "PARENT_ID IS null", 0L);
                    sql += "INSERT INTO TRAININGGROUP (ID, ROOT_ID, PARENT_ID,  TITLE_" + lang;
                    if (description.Length > 0)
                        sql += ", DESCRIPTION_" + lang;
                    sql += ") VALUES(" + id + ", " + rootId + ", " + parentId + ", '" + DBColumn.toSql(title) + "'";
                    if (description.Length > 0)
                        sql += ", '" + DBColumn.toSql(description) + "'";
                    sql += ")";

                    db.execute(sql);
                }
                else
                {
                    sql += "UPDATE TRAININGGROUP SET TITLE_" + lang + " = '" + DBColumn.toSql(title) + "'";
                    if (description.Length > 0)
                        sql += ", DESCRIPTION_" + lang + " = '" + DBColumn.toSql(description) + "'";
                    sql += " WHERE ID = " + id;

                    db.execute(sql);
                }

                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM TRAININGGROUP WHERE ID=" + id);
            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDetailGroupSaveFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DeleteFolderOrTraining(long id, string typ)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                if (typ.Equals("ITEM"))
                {
                    db.execute("DELETE FROM TRAINING WHERE ID =" + id);
                }
                else
                {
                    db.execute("DELETE FROM TRAINING WHERE TRAININGGROUP_ID =" + id);
                    db.execute("DELETE FROM TRAININGGROUP WHERE ID =" + id);
                }

            }


            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "dataDeleteFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTrainingDemandList(string title)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM TRAINING_DEMAND WHERE TITLE_" + lang + " LIKE '%" + title + "%' ORDER BY NUMBER");
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDemandReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetDemandDetail(long id)
        {
            CustomDataErrorObj ret = GetRetObj();
            DBData db = DBData.getDBData(Session);
            DataTable detailData = new DataTable();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            db.connect();
            try
            {
                detailData = db.getDataTable("SELECT ID, NUMBER, TITLE_" + lang + " AS TITLE, MNEMO_" + lang + " AS MNEMO, DESCRIPTION_" + lang + " AS DESCRIPTION FROM TRAINING_DEMAND WHERE ID =" + id);
            }

            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "trainingDemandReadFailed";
                ret.Message = ex.ToString();
            }
            finally
            {
                db.disconnect();
            }

            return ret.getErrorObj(TableToJson(detailData));
        }


        //------------------ General ------------------------------------

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Translate(string text, string scope)
        {
            CustomDataErrorObj ret = GetRetObj();
            string txt = "";
            try
            {

                LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
                txt = _map.get(scope, text);

            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                ret.shortMessage = "readClipboardFailed";
                ret.Message = ex.ToString();
            }
            finally
            {

            }

            return ret.getErrorObj("[{\"Text\":\"" + txt + "\"}]");
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
        public void FolderCheck()
        {
            //Function to check web service folder
        }

        private CustomDataErrorObj GetRetObj()
        {
            CustomDataErrorObj ret = new CustomDataErrorObj();
            ret.session = Session;
            ret.shortMessage = "";
            ret.Message = "";

            return ret;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetImagePath()
        {
            CustomDataErrorObj ret = GetRetObj();
            JavaScriptSerializer js = new JavaScriptSerializer();
            return ret.getErrorObj(js.Serialize(HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("WebService")) + "admin/images/"));
        }

        private IDictionary<T, V> toDictionary<T, V>(Object objAttached)
        {
            var dicCurrent = new Dictionary<T, V>();
            foreach (DictionaryEntry dicData in (objAttached as IDictionary))
            {
                dicCurrent.Add((T)dicData.Key, (V)dicData.Value);
            }
            return dicCurrent;
        }


    }
}



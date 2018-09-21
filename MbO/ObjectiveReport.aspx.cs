using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Data;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for ObjectiveReport.
    /// </summary>
    public partial class ObjectiveReport : System.Web.UI.Page {
        protected string onLoad = "";

        protected void Page_Load(object sender, System.EventArgs e) {
            string context = ch.psoft.Util.Validate.GetValid(Request.QueryString["context"],"");
            long contextId = ch.psoft.Util.Validate.GetValid(Request.QueryString["contextId"], 0L);
            DBData db = DBData.getDBData(Session);
            DataTable table = null;
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
            string sql = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");

            db.connect();
            try {
                switch (context) {
                case Objective.PERSON:
                    PersonObjectiveReport personReport = new PersonObjectiveReport(Session,imageDirectory);

                    // get turn id from querystring, if not set, use default
                    string turnid = db.Objective.turnId.ToString();
                    if (Request.QueryString["turnid"] != null && Request.QueryString["turnid"] != "")
                    {
                        turnid = Request.QueryString["turnid"];
                    }

                    sql =  "select O.*,R.RATING,R.RATING_WEIGHT,R.RATING_DATE from OBJECTIVEPERSONV O left join OBJECTIVE_PERSON_RATING R on O.ID = R.OBJECTIVE_ID"
                        +" where O.PERSONID="+contextId + " and O.PERSON_ID = "+contextId + "and O.OBJECTIVE_TURN_ID = "+turnid
                        //+" where O.PERSONID="+contextId + " and O.OBJECTIVE_TURN_ID = "+db.Objective.turnId
                        +" and O.TYP in ("+db.Objective.objectiveFilter+")";
                    if (!db.Objective.isPersonFilterOnly && Global.Config.isModuleEnabled("project")) {
                        string projectIds = db.Project.getInvolvedProjects(contextId);
                        sql = "("+sql;
                        sql += " union ";
                        sql += "select *,NULL PERSONID,null RATING, null RATING_WEIGHT,null RATING_DATE from OBJECTIVE where PROJECT_ID IN ("+(projectIds == "" ? "0" : projectIds)+"))";
                    }
                    sql += " order by typ,title";
                    table = db.getDataTable(sql);

                    personReport.writeReport(contextId,table, turnid);
                    personReport.saveReport(outputDirectory, "personObjective_"+contextId);

                    Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + personReport.PDFFilename, false);
                    break;
                default:
                    onLoad = "javascript: window.close()";
                    break;
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
            }
                
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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

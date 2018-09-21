using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using ch.psoft.Util;
using System;
using System.Data;
using System.Reflection;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for PrintGlobalPerformance.
    /// </summary>
    public partial class PrintGlobalPerformance : System.Web.UI.Page
    {
        protected long _performanceRatingID = -1;
        protected long _employmentID = -1;
        protected long _mboID = -1;
        protected long _skillsAppraisalID = -1;
        protected string _onloadString;
        private string _cutoffDay = "";
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            _employmentID = ch.psoft.Util.Validate.GetValid(Request.QueryString["employmentID"], -1L);
            _performanceRatingID = ch.psoft.Util.Validate.GetValid(Request.QueryString["performanceRatingID"], -1L);   
            _skillsAppraisalID = ch.psoft.Util.Validate.GetValid(Request.QueryString["skillsAppraisalID"], -1L);
            _mboID = ch.psoft.Util.Validate.GetValid(Request.QueryString["mboID"], -1L);
            _cutoffDay = ch.psoft.Util.Validate.GetValid(Request.QueryString["cutoffDay"], _cutoffDay);
                      
            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            
            
            DBData db = DBData.getDBData(Session);
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

            try
            {
                db.connect();

                bool performance = db.Performance.hasCompositionPart(_employmentID,(int)Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.PerformanceRating);
                bool skills = db.Performance.hasCompositionPart(_employmentID,(int)Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.SkillsRating);
                bool mbo = db.Performance.hasCompositionPart(_employmentID,(int)Interface.DBObjects.Performance.ES_PERFORMANCE_CompositionType.MboRating);

                string sql = "";
                DataTable table = null;

                if (_cutoffDay == ""){
                    _cutoffDay = "GetDate()";
                    if (_performanceRatingID > 0){
                        _cutoffDay = GetValid(db.lookup("RATING_DATE","PERFORMANCERATING","ID="+_performanceRatingID,false), DateTime.Now);
                    }
                }

                if (performance)
                {
                    if (_performanceRatingID < 0)
                    {
                        sql = "select * from performancerating where rating_date<=" + _cutoffDay +
                            " and is_selfrating = 0 and employment_ref=" + _employmentID +
                            " order by rating_date desc";
                        table = db.getDataTable(sql);
                        foreach (DataRow row in table.Rows) {
                            _performanceRatingID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(),-1);
                            break;
                        }
                    }
                }
                if (skills)
                {
                    if (_skillsAppraisalID < 0)
                    {
                        sql = "select skills_appraisal.*,employment.id empId" +
                            " from skills_appraisal,employment" +
                            " where skills_appraisal.person_id = employment.person_id" +
                            " and appraisalDate<=" + _cutoffDay + " and employment.id=" + _employmentID +
                            " order by skills_appraisal.appraisalDate desc";
                        table = db.getDataTable(sql);
                        foreach (DataRow row in table.Rows)
                        {
                            _skillsAppraisalID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(),-1);
                            break;
                        }
                    }
                }
                if (mbo) {
                    if (_mboID < 0) {
                        long persId =  DBColumn.GetValid(db.lookup("person_id","employment","id="+_employmentID),0L);
                        _mboID = DBColumn.GetValid(db.lookup("id","objective_person_rating","person_id="+persId+
                            " and rating_date<=" + _cutoffDay + " order by rating_date desc"),0L);
                    }
                }

				string reportClassName = PerformanceModule.getGlobalPerformanceReportClassName;
				Type reportClass = Type.GetType(reportClassName,true,false);
				ConstructorInfo[] constr = reportClass.GetConstructors();
				PReport report = (PReport) constr[0].Invoke(new object[]{Session,imageDirectory});

                fileName = "globalPerformance" + _employmentID;
                string graphName = outputDirectory + "/" + fileName + "_graph_" + SessionData.getSessionID(Session).ToString() + ".jpg";
				report.createReport(new object[]{_employmentID,_performanceRatingID,graphName,_mboID,_skillsAppraisalID});

                if (report != null)
                    report.saveReport(outputDirectory, fileName);

                Response.Redirect(Global.Config.baseURL + ReportModule.REPORTS_DIRECTORY + "/" + report.PDFFilename, false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex,Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }

        }

        public static string GetValid(string str, DateTime dflt) 
        {
            if (str == null || str.Equals("")) return "'" + dflt.ToUniversalTime().ToString("yyyy'-'MM'-'dd") + "'";
            else 
            {
                try 
                {
                    return "'" + DateTime.Parse(str).ToUniversalTime().ToString("yyyy'-'MM'-'dd") + "'";
                }
                catch 
                {
                    return "'" + dflt.ToUniversalTime().ToString("yyyy'-'MM'-'dd") + "'";
                }
            }
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
        private void InitializeComponent()
        {    
        }
		#endregion
    }
}

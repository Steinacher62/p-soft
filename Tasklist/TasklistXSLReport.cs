using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using System.Web.SessionState;

namespace ch.appl.psoft.Tasklist
{
    public class TasklistXSLReport : TasklistReport
    {

        #region Properties

        #endregion

        #region Constructors

        public TasklistXSLReport(DBData db, HttpSessionState session) : base(db, session)
        {
            this.Suffix = DefaultValues.XMLSuffix;
        }

        #endregion

        #region Public Methods

        public string createXSL()
        {
            XSLTransformer transformer = new XSLTransformer(TaskListModule.excelstylesheet, ReportModule.debugXML);
            transformer.transform(this.XMLDoc, getOutputfileAbsolutePath());

            return Global.Config.baseURL + getOutputfileRelativePath();
        }

        #endregion
    }
}

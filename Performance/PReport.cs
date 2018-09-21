using ch.appl.psoft.Report;
using System.Web.SessionState;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for AveragePerformanceReport.
    /// </summary>
    public class PReport : PsoftPDFReport
    {

        public PReport(HttpSessionState Session, string imageDirectory) : base(Session, imageDirectory) {}

 

        public virtual void createReport(object[] paramter)
        {
			// public override void createReport(..)
        }

     }


}

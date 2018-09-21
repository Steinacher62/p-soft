using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;

namespace ch.appl.psoft.Report
{
    public class ReportFactory
    {
        protected static Queue<ReportDocument> reportQueue = new Queue<ReportDocument>();

        protected static ReportDocument CreateReport()
        {
            ReportDocument report = (ReportDocument) Activator.CreateInstance(typeof(ReportDocument));
            reportQueue.Enqueue(report);
            return report;
        }

        public static ReportDocument GetReport()
        {
            //clear queue if more than 50 reports open
            //Logger.Log("Creating ReportDocument. There are " + reportQueue.Count + " Documents in the Queue.", Logger.VERBOSE);
            if (reportQueue.Count > 50)
            {
                //Logger.Log("Queue-Limit reached, disposing oldest ReportDocument!", Logger.VERBOSE);
                ((ReportDocument)reportQueue.Dequeue()).Dispose();
            }
            return CreateReport();
        }
    }
}

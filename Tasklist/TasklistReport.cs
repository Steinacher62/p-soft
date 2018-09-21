using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Report;
using System;
using System.Data;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Tasklist
{
    public class TasklistReport : XMLReport
    {
        #region Properties

        DBData DB { get; set; }
        protected string Suffix { get; set; }

        #endregion

        #region Constructors

        protected TasklistReport() : base()
        {
        }

        public TasklistReport(DBData db, HttpSessionState session)
            : base(session)
        {
            this.DB = db;
            this.Title = Mapper.get("reportLayout", "TaskListTaskListIDHeader");
        }

        public TasklistReport(DBData db, HttpSessionState session, string title) : base(session, title)
        {
            this.DB = db;
        }

        #endregion

        #region Public Methods

        public XmlDocument createXML()
        {
            XmlElement root = this.XMLDoc.CreateElement("suggestions");
            root.SetAttribute("name", this.Title);

            appendColumnnames(root);

            DataTable table = getDataTable();
            foreach (DataRow row in table.Rows)
            {
                string tasklistname = DB.Tasklist.getTitle(row["TASKLIST_ID"].ToString());
                string authorname = DB.Person.getWholeName(row["AUTHOR_PERSON_ID"].ToString());
                string responsiblename = DB.Person.getWholeName(row["RESPONSIBLE_PERSON_ID"].ToString());
                string state = Mapper.getEnum("tasklist","state",true)[int.Parse(row["STATE"].ToString())];

                XmlElement measure = appendChild(root, "measure");
                appendChild(measure, "tasklist", tasklistname);
                appendChild(measure, "measurename", row["TITLE"]);
                appendChild(measure, "measurenumber", row["NUMMER"]);
                appendChild(measure, "description", row["DESCRIPTION"]);
                appendChild(measure, "author", authorname);
                appendChild(measure, "responsible", responsiblename);
                appendChild(measure, "datedue", row["DUEDATE"]);
                appendChild(measure, "dateopened", row["CREATIONDATE"]);
                appendChild(measure, "status", state);
            }

            this.XMLDoc.AppendChild(this.XMLDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
            this.XMLDoc.AppendChild(root);

            return this.XMLDoc;
        }

        public string createXML(string outputfile)
        {
            createXML().Save(outputfile);
            return outputfile;
        }

        public string getOutputfileRelativePath()
        {
            return getOutputfileRelativePath(this.Suffix);
        }

        public string getOutputfileRelativePath(string suffix)
        {
            return Global.Config.getCommonSetting("tmpdir", DefaultValues.TmpDirectory) + this.Title + "_" + DateTime.Now.ToString("yyMMdd") + suffix;
        }

        public string getOutputfileAbsolutePath()
        {
            return getOutputfileAbsolutePath(this.Suffix);
        }

        public string getOutputfileAbsolutePath(string suffix)
        {
            return AppDomain.CurrentDomain.BaseDirectory + getOutputfileRelativePath(suffix);
        }

        public string getWebPath()
        {
            return getWebPath(this.Suffix);
        }

        public string getWebPath(string suffix)
        {
            return Global.Config.baseURL + getOutputfileRelativePath(suffix);
        }

        #endregion

        #region Private Methods

        private DataTable getDataTable()
        {
            string sql = (string) Session["measurelistsql"];
            Session.Remove("measurelistsql");

            DataTable data = null;

            try
            {
                DB.connect();
                data = DB.getDataTableExt(sql, "MEASURE");
            }
            finally
            {
                DB.disconnect();
            }

            return data;
        }

        private void appendColumnnames(XmlElement root)
        {
            string tasklist_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnTaskList");
            string measurename_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnTitle");
            string measurenumber_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnNummer");
            string description_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnDescription");
            string author_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnAuthorPerson");
            string responsible_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnResponsiblePerson");
            string datedue_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnDuedate");
            string dateopened_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnCreationdate");
            string status_header = Mapper.get("reportLayout", "TaskListTaskListIDColumnState");

            appendChild(root, "columnname", "name", "tasklist", tasklist_header);
            appendChild(root, "columnname", "name", "measurename", measurename_header);
            appendChild(root, "columnname", "name", "measurenumber", measurenumber_header);
            appendChild(root, "columnname", "name", "description", description_header);
            appendChild(root, "columnname", "name", "author", author_header);
            appendChild(root, "columnname", "name", "responsible", responsible_header);
            appendChild(root, "columnname", "name", "datedue", datedue_header);
            appendChild(root, "columnname", "name", "dateopened", dateopened_header);
            appendChild(root, "columnname", "name", "status", status_header);
        }

        #endregion
    }
}

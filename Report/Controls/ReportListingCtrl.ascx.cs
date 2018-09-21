using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Reflection;

namespace ch.appl.psoft.Report.Controls
{
    /// <summary>
    /// Summary description for ReportListing.
    /// </summary>
    public partial class ReportListingCtrl : PSOFTMapperUserControl
    {
        protected string _onloadScript = "";

        #region Properties
        public static string Path
        {
            get {return Global.Config.baseURL + "/Report/Controls/ReportListingCtrl.ascx";}
        }

        protected string _backURL = "../default.html";
        public string BackURL
        {
            get {return _backURL;}
            set {_backURL = value;}
        }

        protected string _param0 = "";
        public string Param0
        {
            get {return _param0;}
            set {_param0 = value;}
        }

        protected string _param1 = "";
        public string Param1
        {
            get {return _param1;}
            set {_param1 = value;}
        }

        protected string _param2 = "";
        public string Param2
        {
            get {return _param2;}
            set {_param2 = value;}
        }

        protected string _filterQuery = "";
        public string FilterQuery
        {
            get {return _filterQuery;}
            set {_filterQuery = value;}
        }

        protected long _reportLayoutID = -1;
        public long ReportLayoutID
        {
            get {return _reportLayoutID;}
            set {_reportLayoutID = value;}
        }
        #endregion

        /// <summary>
        /// Load page
        /// </summary>
        /// <param name="ID">Parameter 0</param>
        /// <param name="reportLayoutID">Reportlayout Id</param>
        /// <param name="BackURL">URL to go back</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (_reportLayoutID > 0)
                {

                    //New Report XLS (XML) Creation - Begin
                    DBData db = DBData.getDBData(Session);

                    bool toggle = false;

                    db.connect();
                    try
                    {
                        string reporttitle = db.lookup("TITLE_MNEMO", "reportlayout", "id=" + _reportLayoutID, "");
                        switch (reporttitle)
                        {
                            case "TaskListTaskListIDEXCEL":
                                Tasklist.TasklistReport tasklistreport = new Tasklist.TasklistReport(db, Session);
                                tasklistreport.createXML(tasklistreport.getOutputfileAbsolutePath());
                                BackURL = tasklistreport.getWebPath();
                                Response.ContentType = "application/vnd.ms-excel";
                                toggle = true;
                            break;
                        }
                    }
                    finally
                    {
                        db.disconnect();
                    }

                    if (toggle)
                    {
                        Response.Redirect(BackURL, false);
                        return;
                    }
                    //New Report XLS (XML) Creation - End
                    
                    string path = Server.MapPath("~" + ReportModule.REPORTS_DIRECTORY);
                    string imagePath = Server.MapPath("~/Images");
                    object[] obj = db.lookup(new string[]{"REPORTCLASS","TYPE"},"REPORTLAYOUT","ID="+_reportLayoutID,false);
                    string reportClassName = DBColumn.GetValid(obj[0],"");
                    int reportType = Int32.Parse(DBColumn.GetValid(obj[1],"0")); 
                    Type reportClass = Type.GetType(reportClassName,true,false);
                    ConstructorInfo[] constr = reportClass.GetConstructors();
                    XMLPDFList reportXMLPDF = null;
                    XLSList reportXLS = null;
                    /* ReportListing is called by the "goto(TABLEEXT.DETAIL_URL)" mechanism
                     * and only makes sense for List or ListExcel type
                    */
                    switch(reportType)
                    {
                        case (int) ReportModule.ReportType.List:
                            reportXMLPDF = (XMLPDFList) constr[0].Invoke(null);
                            break;
                        case (int) ReportModule.ReportType.ListExcel:
                            reportXLS = (XLSList) constr[0].Invoke(null);
                            break;
                    }
                    DataTable data = null;

                    db.connect();
                    try
                    {
                        if (_filterQuery != "") 
                        {
                            string sql = db.lookup("sql","reportlayout","id="+_reportLayoutID,false);
                            string tableName = db.lookup("dbtable","reportlayout","id="+_reportLayoutID,false);
                            int idx = sql.ToLower().LastIndexOf("where");

                            if (idx > 0)
                                sql = sql.Replace("where","where "+_filterQuery+" and ");
                            else
                                sql += "where "+_filterQuery;

                            if (_param0 != "")
                            {
                                switch(reportType)
                                {
                                    case (int) ReportModule.ReportType.List:
                                        sql = reportXMLPDF.substitute(sql,_param0,_param1,_param2);
                                        break;
                                    case (int) ReportModule.ReportType.ListExcel:
                                        sql = reportXLS.substitute(sql,_param0,_param1,_param2);
                                        break;
                                }
                            }

                            if (tableName != "")
                                data = db.getDataTableExt(sql,tableName);
                            else
                                data = db.getDataTable(sql);
                        }
                        else 
                        {
                            _filterQuery = db.lookup("filter","reportlayout","id="+_reportLayoutID,false);
                            if (_filterQuery != "") 
                            {
                                _filterQuery = _filterQuery.Replace("$0",_param0).Replace("$1",_param1).Replace("$2",_param2);
                                Response.Redirect(_filterQuery.Replace("$backURL",_backURL).Replace("$ID",_reportLayoutID.ToString()));
                                return;
                            }
                        }
                        if (_param0 != "")
                        {
                            if (_param1 == "") 
                                _param1 = _param0;
                            if (_param2 == "")
                                _param2 = _param0;

                            switch(reportType)
                            {
                                case (int) ReportModule.ReportType.List:
                                    reportXMLPDF.writeList(_reportLayoutID,path,imagePath,_mapper,db,data,Session,_param0,_param1,_param2);
                                    break;
                                case (int) ReportModule.ReportType.ListExcel:
                                    reportXLS.writeList(_reportLayoutID,path,imagePath,_mapper,db,data,Session,_param0,_param1,_param2);
                                    break;
                            }
                        }
                        else
                        {
                            switch(reportType)
                            {
                                case (int) ReportModule.ReportType.List:
                                    reportXMLPDF.writeList(_reportLayoutID,path,imagePath,_mapper, db,data, Session);
                                    break;
                                case (int) ReportModule.ReportType.ListExcel:
                                    reportXLS.writeList(_reportLayoutID,path,imagePath,_mapper, db,data, Session);
                                    break;
                            }
                        }
                        switch(reportType)
                        {
                            case (int) ReportModule.ReportType.List:
                                //_onloadScript = "bodyOnLoad(); window.location.href='.." + ReportModule.REPORTS_DIRECTORY + "/" +reportXMLPDF.pdfFilename + "'";

                                // redirect direct to report / 23.11.09 / mkr
                                Response.Redirect(".." + ReportModule.REPORTS_DIRECTORY + "/" +reportXMLPDF.pdfFilename);
                                break;
                            case (int) ReportModule.ReportType.ListExcel:
                                //_onloadScript = "bodyOnLoad(); window.location.href='.." + ReportModule.REPORTS_DIRECTORY + "/" +reportXLS.excelFilename + "'";

                                // redirect direct to report / 23.11.09 / mkr
                                Response.Redirect(".." + ReportModule.REPORTS_DIRECTORY + "/" + reportXLS.excelFilename);
                                break;
                        }
                    }
                    finally 
                    {
                        db.disconnect();
                    }
                }
                else
                    Response.Redirect(_backURL);
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

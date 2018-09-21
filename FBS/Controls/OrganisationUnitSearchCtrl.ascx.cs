///TODO
///
namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using Common;
    using db;
    using Interface;
    using System;
    using System.Data;
    /// <summary>
    ///		Summary description for OrganisationUnitSearchCtrl.
    /// </summary>
    public partial  class OrganisationUnitSearchCtrl : PSOFTSearchUserControl 
    {

        private DBData _db = null;
        private DataTable _table = null;


        private string reportLocation = "";

        public enum ReportTypeEnum
        {
            RESPONSABILITY,
            COMPETENCE
        } 

        private ReportTypeEnum reportType = ReportTypeEnum.RESPONSABILITY;
        public ReportTypeEnum ReportType 
        {
            set { this.reportType = value; }
        }


        public static string Path 
        {
            get {return Global.Config.baseURL + "/FBS/Controls/OrganisationUnitSearchCtrl.ascx";}
        }

		#region Properties

		#endregion

        protected void Page_Load(object sender, System.EventArgs e) 
        {
            Execute();
        }

        protected override void DoExecute() 
        {
            base.DoExecute ();

            if (!IsPostBack) 
            {
                apply.Text = _mapper.get("print");
            }
            if (Visible) loadDetail();
        }

        private void loadDetail() 
        {
            _db = DBData.getDBData(Session);
            _db.connect();
            try 
            {
                // load details of tasklist
                detailTab.Rows.Clear();
                _table = _db.getDataTableExt("select * from ORGENTITY where ID=-1","ORGENTITY");
                DataTable mainorg = _db.getDataTable("select id from organisation where MAINORGANISATION = 1"); 
                DataTable themeTypeTab = _db.getDataTable("select ID, TITLE_DE from ORGENTITY  ORDER by TITLE_DE");
                if(mainorg.Rows.Count >0)
                {
                    long mainId = (long)mainorg.Rows[0][0];
                    themeTypeTab = _db.getDataTable("select ID, TITLE_DE from ORGENTITY where ROOT_ID = " + mainId + " ORDER by TITLE_DE");
                }


                _table.Columns[_db.langAttrName("ORGENTITY", "MNEMONIC")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.INVISIBLE;
                _table.Columns[_db.langAttrName("ORGENTITY", "TITLE")].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.INVISIBLE;

                _table.Columns["ID"].ExtendedProperties["Visibility"] = (int) DBColumn.Visibility.SEARCH + (int) DBColumn.Visibility.EDIT;
                _table.Columns["ID"].ExtendedProperties["In"] = themeTypeTab;
                _table.Columns["ID"].ExtendedProperties["InputControlType"] = typeof(DropDownCtrl);


                base.LoadInput(_db, _table, detailTab);
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();
            }
        }

        private void mapControls() 
        { 
             apply.Click += new System.EventHandler(apply_Click);
        }

       
        private void apply_Click(object sender, System.EventArgs e) 
        {
            if (!base.checkInputValue(_table,detailTab))
                return;

            string unitId = (string)this.getInputValue(_table,detailTab,"ID");
            if(unitId == "") unitId = "0";

            DBData db = DBData.getDBData(Session);
            db.connect();

            try
            {
                string url = "";
                switch(this.reportType) 
                {
                    case ReportTypeEnum.RESPONSABILITY:
                        url = printResponsability(db, long.Parse(unitId));
                        break;
                    case ReportTypeEnum.COMPETENCE:
                        url = printCompetence(db, long.Parse(unitId));
                        break;
                    default:
                        url = printResponsability(db, long.Parse(unitId));                        
                        break;
                }
                if(!streamOutput()) 
                {
                    Response.Redirect(url,false);
                }
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

        /// <summary>
        /// generates an output stream so that the file is directly opened in excel.
        /// </summary>
        /// <returns></returns>
        private bool streamOutput()
        {
            //InputStream - lesen
            bool ret = false;
            if(this.reportLocation == "") 
                return ret;
            System.IO.FileStream emlStream = null;
            try 
            {
              
                emlStream = new System.IO.FileStream(reportLocation, System.IO.FileMode.Open);

                //Content-Type setzen
                Response.ContentType = "application/msexcel";

                //Content-Disposition setzen und Filenam 
                string filename = this.reportLocation.Substring(this.reportLocation.LastIndexOf("/")+1, this.reportLocation.Length - this.reportLocation.LastIndexOf("/")-1);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);

                byte [] buffer = new byte [4096];
                int len = 0;
                while ((len = emlStream.Read(buffer,0,buffer.Length)) > 0 )
                {
                    //erst flushen
                    Response.Flush();
                    //rausschreiben
                    Response.OutputStream.Write(buffer,0,len);
                }              
                Response.End();
            }
            catch(Exception e) 
            { 
                Logger.Log(e,Logger.WARNING);
                ret = false;
            }
            finally 
            {
                if(emlStream!=null) emlStream.Close();
            }
            return ret;
            
        }

        private string printResponsability(DBData db, long orgId) 
        {
            int userID = ch.psoft.Util.Validate.GetValid(Request.QueryString["userID"], -1);

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

  

            ResponsabilitiesMatrixReport report = new ResponsabilitiesMatrixReport(Session, imageDirectory, orgId);

            fileName = "matrixResponsabilities" + userID;
               
            report.createReport();

            if (report != null) 
            {
                reportLocation = report.saveReport(outputDirectory, fileName);
            }
            return Global.Config.baseURL + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY + "/" + report.Filename;
      
        }



        private string printCompetence(DBData db, long orgId) 
        {
            int userID = ch.psoft.Util.Validate.GetValid(Request.QueryString["userID"], -1);

            string fileName = "";
            string outputDirectory = Request.MapPath("~" + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY); 
            string imageDirectory = Request.MapPath("~/images");
            
            
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);

  

            CoreCompetenceMatrixReport report = new CoreCompetenceMatrixReport(Session, imageDirectory, orgId);

            fileName = "matrixCoreCompetence" + userID;
               
            report.createReport();


            if (report != null) 
            {
                reportLocation = report.saveReport(outputDirectory, fileName);
            }
            return Global.Config.baseURL + ch.appl.psoft.Report.ReportModule.REPORTS_DIRECTORY + "/" + report.Filename;
      
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
            mapControls();
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

        }
		#endregion
	}
}

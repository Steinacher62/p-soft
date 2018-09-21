namespace ch.appl.psoft.FBS.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    public partial class DutyCompetenceValidityList : PSOFTListViewUserControl
    {
        private long _dutyID = -1;
        private long _jobID = -1;
        private long _dutyCompetenceValidityID = -1;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public static string Path
        {
            get {return Global.Config.baseURL + "/FBS/Controls/DutyCompetenceValidityList.ascx";}
        }

        public DutyCompetenceValidityList() : base()
        {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            OrderColumn = "VALID_FROM";
            OrderDir = "asc";
        }

		#region Properities
        public long DutyID
        {
            get {return _dutyID;}
            set {_dutyID = value;}
        }

        public long JobID
        {
            get {return _jobID;}
            set {_jobID = value;}
        }

        public long DutyCompetenceValidityID
        {
            get {return _dutyCompetenceValidityID;}
            set {_dutyCompetenceValidityID = value;}
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            DBData db = DBData.getDBData(Session);
            try 
            {
                db.connect();
                pageTitle.Text = _mapper.get(FBSModule.LANG_SCOPE_FBS, FBSModule.LANG_MNEMO_CT_DCV_LIST).Replace("#1", db.lookup("TITLE", "DUTYV", "ID=" + DutyID, false).ToString());

                long funktionID = ch.psoft.Util.Validate.GetValid(db.lookup("FUNKTION_ID", "JOB", "ID=" + JobID, false), -1);
                string sql = "select * from DUTY_COMPETENCE_VALIDITY where DUTY_ID=" + _dutyID + " and (FUNKTION_ID=" + funktionID + " or JOB_ID=" + JobID + ")";
                sql += " order by " + OrderColumn + " " + OrderDir;
                DataTable table = db.getDataTableExt(sql, "DUTY_COMPETENCE_VALIDITY");

                if (_dutyCompetenceValidityID > 0)
                {
                    HighlightRecordID = _dutyCompetenceValidityID;
                    LoadList(db, table, listTab);
                }
            }
            catch (Exception ex) 
            {
                DoOnException(ex);
            }
            finally 
            {
                db.disconnect();
            }
        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell) 
        {
            if (ListBuilder.IsDeleteCell(cell) && ch.psoft.Util.Validate.GetValid(row["FUNKTION_ID"].ToString(), -1) > 0)
            {
                r.Cells.Remove(cell);
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
		#endregion
    }
}

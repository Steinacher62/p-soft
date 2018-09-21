using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI.WebControls;


namespace ch.appl.psoft.Person.Controls
{
    public partial class PersonJobDevelopmentList : PSOFTListViewUserControl
    {
        private DBData _db = null;

        protected System.Web.UI.HtmlControls.HtmlTable LinksTable;

        public PersonJobDevelopmentList() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
			
        }



        public static string Path
        {
            get { return Global.Config.baseURL + "/Person/Controls/PersonJobDevelopmentList.ascx"; }
        }

        public long PersonID
        {
            get { return GetLong("PersonID"); }
            set { SetParam("PersonID", value); }
        }
		

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            _db = DBData.getDBData(Session);

            try
            {
                _db.connect();
                pageTitle.Text = _mapper.get("jobDevelopment", "jobDevelopmentList").Replace("#1", _db.lookup("isnull(firstname + ' ','') + isnull(pname,'')", "PERSON", "ID=" + PersonID, false).ToString());
                string sql = "select * from JOB_DEVELOPMENT where PERSON_ID = " + PersonID;


                IDColumn = "ID";

                if (PersonID > 0)
                {
                   //TODO
                   // HighlightRecordID = ch.psoft.Util.Validate.GetValid(_db.lookup("ID", "JOB_DEVELOPMENT", "ID=" + ..., false), -1);
                }
                sql += " order by FROM_DATE desc";
                DataTable table = _db.getDataTableExt(sql, "JOB_DEVELOPMENT");
                table.Columns[_db.langAttrName("JOB_DEVELOPMENT", "EMPLOYMENT_NAME")].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["HAUPTFUNKTION"].ExtendedProperties["In"] = _mapper.getEnum("jobDevelopment", "hauptfunktionEnum");
                table.Columns["TYP"].ExtendedProperties["In"] = _mapper.getEnum("jobDevelopment", "typ");
                LoadList(_db, table, listTab);

            }
            catch (Exception ex)
            {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
            }

        }

        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell cell)
        {
            int i = 0;		
        }


    }
}
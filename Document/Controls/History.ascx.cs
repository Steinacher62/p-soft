using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Document.Controls
{
    /// <summary>
    /// Summary description for List.
    /// </summary>
    public partial class History : PSOFTListViewUserControl
    {
        public const string PARAM_DOCUMENT_ID = "PARAM_DOCUMENT_ID";

        
        protected DBData _db = null;
        protected string _deleteURL = "";

        public static string Path
        {
            get {return Global.Config.baseURL + "/Document/Controls/History.ascx";}
        }

        #region Properities
        public long DocumentID
        {
            get {return GetLong(PARAM_DOCUMENT_ID);}
            set {SetParam(PARAM_DOCUMENT_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();
            _db = DBData.getDBData(Session);

            try
            {

                if (!IsPostBack)
                {
                    _deleteURL = Request.RawUrl;
                    HistoryListTitle.Text = _mapper.get("document","historyListTitle");

                    _db.connect();
                    string sql = "select * from DOCUMENT_HISTORY where DOCUMENT_ID=" + DocumentID;
                    sql += " order by " + OrderColumn + " " + OrderDir;
                    DataTable table = _db.getDataTableExt(sql,"DOCUMENT_HISTORY");
                    table.Columns["PERSON_ID"].ExtendedProperties["In"] = this;

                    _listBuilder.addCell += new AddCellHandler(addCell);
                    DeleteEnabled = true;
                    EditEnabled = false;
                    DetailEnabled = false;
                    HeaderEnabled = true;
                    IDColumn = "ID";
                    LoadList(_db, table, historyList);
                }
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
            
        public string lookup(DataColumn col, object id, bool http) 
        {
            string retValue = "";

            if (col != null && !(id is System.DBNull))
            {
                switch (col.ColumnName)
                {
                    case "PERSON_ID":
                        retValue = _db.Person.getWholeName(id.ToString());
                        break;
                }
            }
            return retValue;
        }

        private void addCell(DataRow row, DataColumn col, TableRow r, TableCell c)
        {
            if (r.Cells[0] == c)
            {
                if (c.Text.StartsWith("<img"))
                    c.Text = c.Text.Replace("images/info.gif","images/file.gif");
            }
            if (col != null && col.ColumnName == "FILENAME" && ch.psoft.Util.Validate.GetValid(c.Text) != "")
            {
                HyperLink link = new HyperLink();
                c.Controls.Clear(); 
                c.Controls.Add(link);
                link.CssClass = "List";
                link.Target = "_blank";
                link.NavigateUrl = psoft.Document.GetDocument.GetURL("historyID",row["ID"]);
                link.Text = c.Text;
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

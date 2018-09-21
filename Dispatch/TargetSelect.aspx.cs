using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Dispatch
{
    /// <summary>
    /// Summary description for TargetSelect
    /// </summary>
    public partial class TargetSelect : System.Web.UI.Page
	{
        protected string _onloadString = "";

        protected DBData _db = null;
        protected LanguageMapper _map = null;
        protected ListBuilder _listBuilder = null;
        protected int _xID = -1;
        protected int _reportLayoutID = -1;
        protected int _searchResultID = -1;
        protected string _backURL = "";
        protected string _orderColumn;
        protected string _orderDir;
        protected string[] _substituteValues = null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _db = DBData.getDBData(Session);
            _map = LanguageMapper.getLanguageMapper(Session);
            _xID = ch.psoft.Util.Validate.GetValid(Request.QueryString["xID"], _xID);
            _reportLayoutID = ch.psoft.Util.Validate.GetValid(Request.QueryString["reportLayoutID"], _reportLayoutID);
            _searchResultID = ch.psoft.Util.Validate.GetValid(Request.QueryString["searchResultID"], _searchResultID);
            _backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"], _backURL);
            _orderColumn = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderColumn"], "NAME");
            _orderDir = ch.psoft.Util.Validate.GetValid(Request.QueryString["orderDir"], "asc");
            _substituteValues = new String[] {ch.psoft.Util.Validate.GetValid(Request.QueryString["param0"], ""), ch.psoft.Util.Validate.GetValid(Request.QueryString["param1"], ""), ch.psoft.Util.Validate.GetValid(Request.QueryString["param2"], "")};
            _listBuilder = new ListBuilder();

            if (!IsPostBack)
            {
                select.Text = _map.get("next");
                cbAll.Text = _map.get("dispatch", "selectAll");
                cbAll.Checked = _searchResultID < 1;
            }

            loadList();
        }

        private void loadList()
        {
            listTab.Controls.Clear();

            try
            {
                _db.connect();
                string sql = "";
                string DBTable = "";
                DataTable table = _db.getDataTable("select * from REPORTLAYOUT where ID=" + _reportLayoutID);
                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];
                    sql = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName("REPORTLAYOUT","SQL")].ToString(), "").Replace("%ID", _xID.ToString());
                    if (_substituteValues != null){
                        for (int i=0; i<_substituteValues.Length; i++){
                            sql = sql.Replace("$"+i, _substituteValues[i]);
                        }
                    }
                    DBTable = ch.psoft.Util.Validate.GetValid(row["DBTABLE"].ToString(), "");
                }

                sql += " order by " + _orderColumn + " " + _orderDir;
                table = _db.getDataTableExt(sql, DBTable);

                _listBuilder.rowsPerPage = SessionData.getRowsPerListPage(Session);
                _listBuilder.detailEnable = false;
                _listBuilder.deleteEnable = false;
                _listBuilder.editEnable = false;
                _listBuilder.headerEnable = true;
                _listBuilder.infoBoxEnable = false;
                _listBuilder.checkBoxEnable = true;
                _listBuilder.radioButtons = false;
                _listBuilder.checkBoxChecked = _searchResultID < 1;
                _listBuilder.idColumn = table.Columns["ID"];
                _listBuilder.orderColumn = table.Columns[_orderColumn];
                _listBuilder.orderDir = _orderDir;
                _listBuilder.addRow += new AddRowHandler(onAddRow);
                int numRec = _listBuilder.load(_db, table, listTab, _map);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
            }
        }

        private void onAddRow(DataRow row, TableRow r)
        {
            if (_searchResultID > 0)
            {
                int id = ch.psoft.Util.Validate.GetValid(ListBuilder.getID(r), -1);
                if (id > 0)
                {
                    string sID = _db.lookup("ROW_ID", "SEARCHRESULT", "ID=" + _searchResultID + " and ROW_ID=" + id, false);
                    if (sID.Length > 0)
                        ListBuilder.setChecked(r, true);
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

        protected void select_Click(object sender, System.EventArgs e)
        {
            try
            {
                _db.connect();
                long searchResultID = _db.newId("SEARCHRESULT");

                //save selected items in DB
                foreach (TableRow r in listTab.Rows) 
                {
                    if (ListBuilder.isChecked(r))
                    {
                        string recordID = ListBuilder.getID(r);
                        //save record in DB

                        _db.execute("INSERT INTO SEARCHRESULT (ID, TABLENAME, ROW_ID) VALUES (" + searchResultID + ",'PERSON'," + recordID + ")");
                    }
                }
                
                if (searchResultID > 0)
                    _backURL = _backURL.Replace("&searchResultID=" + _searchResultID, "&searchResultID=" + searchResultID);

                _onloadString = "RefreshAndClose();";
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
            }
        }

        protected void cbAll_CheckedChanged(object sender, System.EventArgs e)
        {
            foreach (TableRow r in listTab.Rows)
            {
                ListBuilder.setChecked(r, cbAll.Checked);
            }
        }
	}
}

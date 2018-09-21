using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for AccessorSelect.
    /// </summary>
    /// <param name="nextURL">URL auf welche nach erfolgter Selektion weitergeleitet wird</param>
    /// <param name="backUR">URL auf welche bei Abbruch weitergeleitet wird</param>
    public partial class AccessorSelect : System.Web.UI.Page
    {

        protected LanguageMapper _map = null;
        protected DBData _db = null;
        protected string _backURL = "";
        protected string _nextURL = "";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            _db = DBData.getDBData(Session);
            _map = LanguageMapper.getLanguageMapper(Session);

            _backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"], "");
            _nextURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["nextURL"], "");

            accessorSelectTitle.Text = _map.get("authorisation", "selectAccessorTitle");
            BackButton.Text = _map.get("back");
            SelectButton.Text = _map.get("next");
            NewUserButton.Text = _map.get("addNewUser");
            RadWindowManager1.Windows[0].Title = _map.get("addNewUser");
            LabelUsername.Text = _map.get("loginUsername");
            ButtonNewUser.Text = _map.get("add");
            

            if (Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "0")
            {
                NewUserButton.Visible = false;

            }
            ListBuilder accessorList = new ListBuilder();

            try
            {
                _db.connect();
                string sql;
                if (Global.Config.getModuleParam("morph", "permissionMultiuser", "0") == "1")
                {
                    long matrixUserGroupId = _db.lookup("ID", "MATRIX_USER_GROUP", "OWNER_ID = " + _db.userId, 0L);

                    sql = "SELECT * FROM ACCESSORV WHERE ID IN (SELECT USER_ID FROM MATRIX_USER WHERE MATRIX_USER_GOUP_ID = " + matrixUserGroupId + ") OR ID = " + _db.userId;
                }
                else
                {
                    sql = "select * from ACCESSORV order by " + _db.langAttrName("ACCESSORV", "TITLE");
                }
                DataTable table = _db.getDataTableExt(sql, "ACCESSORV");

                table.Columns["VISIBLE"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                table.Columns["TABLENAME"].ExtendedProperties["Visibility"] = DBColumn.Visibility.INVISIBLE;
                accessorList.rowsPerPage = SessionData.getRowsPerListPage(Session);
                accessorList.deleteEnable = false;
                accessorList.detailEnable = false;
                accessorList.editEnable = false;
                accessorList.headerEnable = false;
                accessorList.idColumn = table.Columns["ID"];
                accessorList.infoBoxEnable = true;
                accessorList.checkBoxEnable = true;
                accessorList.orderColumn = table.Columns[_db.langAttrName("ACCESSORV", "TITLE")];
                accessorList.useFirstLetterAsPageSelector = true;
                accessorList.addCell += new AddCellHandler(addCell);
                accessorList.load(_db, table, accessorTab, _map);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
                errorText.Text = _map.get("MESSAGES", "unknownError");
                errorText.Visible = true;
            }
            finally
            {
                _db.disconnect();
            }
        }

        private void addCell(DataRow row, DataColumn col, TableRow r, TableCell c)
        {
            if (c.Text.StartsWith("<img"))
            {
                if (row["TABLENAME"].ToString().Equals("PERSON"))
                    c.Text = c.Text.Replace("info.gif", "user.gif");
                else
                    c.Text = c.Text.Replace("info.gif", "group.gif");
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

        protected void BackButton_Click(object sender, System.EventArgs e)
        {
            Response.Redirect(_backURL);
        }

        protected void SelectButton_Click(object sender, System.EventArgs e)
        {
            string selection = "";

            foreach (TableRow r in this.accessorTab.Rows)
            {
                if (ListBuilder.isChecked(r))
                {
                    if (!selection.Equals(""))
                        selection += ",";
                    selection += ListBuilder.getID(r);
                }
            }

            Response.Redirect(_nextURL + ((_nextURL.IndexOf("?") >= 0) ? "&" : "?") + "selection=" + selection);
        }

        protected void ButtonNewUser_Click(object sender, EventArgs e)
        {
            string user = TBUsername.Text.Trim();
            _db.connect();
            long userId = _db.lookup("ID", "PERSON", "LOGIN = '" + user.Replace("'", "''") + "'", 0L);
            if (userId > 0)
            {
                long matrixUserGroupId = _db.lookup("ID", "MATRIX_USER_GROUP", "OWNER_ID = " + _db.userId, 0L);
                _db.execute("INSERT INTO MATRIX_USER (MATRIX_USER_GOUP_ID, USER_ID) VALUES (" + matrixUserGroupId + ", " + userId + ")");
                Response.Redirect(Request.RawUrl);
            }
            else
            {
                _map = LanguageMapper.getLanguageMapper(Session);
                ErrorMessage.Text = _map.get("addUserFailt");
                RadWindowManager1.Windows[0].VisibleOnPageLoad = true;

            }
            _db.disconnect();
        }
    }
}

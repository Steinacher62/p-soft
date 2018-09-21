using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for Authorisations.
    /// </summary>
    public partial class Authorisations : System.Web.UI.Page {
    
        protected LanguageMapper _map = null;
        protected DBData _db = null;
        protected string _tableName = "";
        protected long _rowID = -1;
        protected bool _accessDenied = true;
        protected string _onloadString = "";
        protected string _column = "";
        protected long _selectedID = -1L;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        /// <param name="column"></param>
        protected void Page_Load(object sender, System.EventArgs e) {
            _db = DBData.getDBData(Session);
            _map = LanguageMapper.getLanguageMapper(Session);

            _tableName = ch.psoft.Util.Validate.GetValid(Request.QueryString["tableName"],"");
            _rowID = ch.psoft.Util.Validate.GetValid(Request.QueryString["rowID"],_rowID);
            _column = ch.psoft.Util.Validate.GetValid(Request.QueryString["column"],"");
            _selectedID = ch.psoft.Util.Validate.GetValid(Request.QueryString["selectedID"],_selectedID);

            CBReadEffective.Enabled = false;
            CBInsertEffective.Enabled = false;
            CBUpdateEffective.Enabled = false;
            CBAdminEffective.Enabled = false;
            CBDeleteEffective.Enabled = false;
            CBExecuteEffective.Enabled = false;

            if (!IsPostBack) {
                authorisationsTitle.Text = _map.get("authorisation", "authorisationsTitle");
                LabelAccessors.Text = _map.get("authorisation", "accessors");
                LabelAuthorisations.Text = _map.get("authorisation", "authorisations");

                AddAccessorButton.Text = _map.get("authorisation", "addAccessor");
                RemoveAccessorButton.Text = _map.get("authorisation", "removeAccessor");

                LabelRead.Text = _map.get("authorisation", "read");
                LabelInsert.Text = _map.get("authorisation", "insert");
                LabelUpdate.Text = _map.get("authorisation", "update");
                LabelDelete.Text = _map.get("authorisation", "delete");
                LabelAdmin.Text = _map.get("authorisation", "admin");
                LabelExecute.Text = _map.get("authorisation", "execute");

                CBInheritance.Text = _map.get("authorisation", "inheritance");
                displayAccessors();
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

        protected void accessors_SelectedIndexChanged(object sender, System.EventArgs e) {
            displayAuthorisations();
        }

        private void displayAccessors() {
            accessors.Items.Clear();
            try {
                long [] accessorIDs = null;
                _db.connect();

                authorisationsTitle.Text = authorisationsTitle.Text.Replace("#1", _db.UID2NiceName(_db.ID2UID(_rowID, _tableName), _map, true));
                if (_tableName != "") {
                    bool inheritFlagExists = false;

                    if (_rowID > 0) {
                        // row-authorisations
                        bool inherit = false;
                        inheritFlagExists = _db.getInheritFlag(_tableName, _rowID, out inherit);
                        CBInheritance.Checked = inherit;
                        accessorIDs = _db.getRowAccessorIDs(_tableName, _rowID);
                        _accessDenied = !_db.hasRowAuthorisation(DBData.AUTHORISATION.ADMIN, _tableName, _rowID, true, true);
                    }
                    else if (_column != "") {
                        // column-authorisations
                        accessorIDs = _db.getColumnAccessorIDs(_tableName, _column);
                        _accessDenied = !_db.hasColumnAuthorisation(DBData.AUTHORISATION.ADMIN, _tableName, _column, true, true);
                    }
                    else {
                        // table-authorisations
                        accessorIDs = _db.getTableAccessorIDs(_tableName);
                        _accessDenied = !_db.hasTableAuthorisation(DBData.AUTHORISATION.ADMIN, _tableName, true);
                    }

                    if (!_accessDenied){
                        int i=0;
                        foreach (int accessorID in accessorIDs) {
                            string sql = "select * from ACCESSORV where ID=" + accessorID;
                            DataTable table = _db.getDataTable(sql);

                            if (table.Rows.Count > 0) {
                                string tablename = table.Rows[0]["TABLENAME"].ToString();
                                string itemText = table.Rows[0][_db.langAttrName("ACCESSORV", "TITLE")].ToString();
                                accessors.Items.Add(new ListItem(itemText, accessorID.ToString()));
                                if (_selectedID == accessorID){
                                    accessors.SelectedIndex = i;
                                }
                                i++;
                            }
                        }
                        if (accessors.Items.Count > 0 && accessors.SelectedIndex < 0){
                            accessors.SelectedIndex = 0;
                        }

                        CBRead.AutoPostBack = CBInsert.AutoPostBack = CBUpdate.AutoPostBack = CBDelete.AutoPostBack = CBAdmin.AutoPostBack = CBExecute.AutoPostBack = CBInheritance.AutoPostBack = true;
                        CBInheritance.Enabled = inheritFlagExists;
                        AddAccessorButton.Enabled = RemoveAccessorButton.Enabled = true;
                        displayAuthorisations();
                    }
                    else{
                        _onloadString += "window.close();";
                    }
                }
                else {
                    Logger.Log("No authorisation-object specified", Logger.ERROR);
                }

            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
                errorText.Text = _map.get("MESSAGES", "unknownError");
                errorText.Visible = true;
            }
            finally {
                _db.disconnect();
            }
        }

        private void displayAuthorisations() {
            int accessorID = 0;
            if (accessors.SelectedIndex >= 0)
                accessorID = ch.psoft.Util.Validate.GetValid(accessors.SelectedItem.Value,-1);
            else {
                CBRead.Enabled = false;
                CBInsert.Enabled = false;
                CBUpdate.Enabled = false;
                CBDelete.Enabled = false;
                CBAdmin.Enabled = false;
                CBExecute.Enabled = false;
            }

            if (accessors.Items.Count == 0)
                RemoveAccessorButton.Enabled = false;

            CBRead.Checked = false;
            CBInsert.Checked = false;
            CBUpdate.Checked = false;
            CBDelete.Checked = false;
            CBAdmin.Checked = false;
            CBExecute.Checked = false;

            if (_tableName != "") {
                int authorisations = 0;
                int effeciveAuthorisations = 0;
                int inheritedTableAuthorisations = 0;
                if (_rowID > 0){
                    authorisations = _db.getRowAuthorisations(accessorID, _tableName, _rowID, false, false);
                    effeciveAuthorisations = _db.getRowAuthorisations(accessorID, _tableName, _rowID, true, true);
                    inheritedTableAuthorisations = _db.getRowAuthorisations(accessorID, _tableName, _rowID, false, true);
                }
                else if (_column != ""){
                    authorisations = _db.getColumnAuthorisations(accessorID, _tableName, _column, false, false);
                    effeciveAuthorisations = _db.getColumnAuthorisations(accessorID, _tableName, _column, true, true);
                    inheritedTableAuthorisations = _db.getColumnAuthorisations(accessorID, _tableName, _column, false, true);
                }
                else{
                    authorisations = _db.getTableAuthorisations(accessorID, _tableName, false);
                    inheritedTableAuthorisations = effeciveAuthorisations = _db.getTableAuthorisations(accessorID, _tableName, true);
                }

                CBRead.Checked = (authorisations & DBData.AUTHORISATION.READ) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.READ) > 0;
                CBInsert.Checked = (authorisations & DBData.AUTHORISATION.INSERT) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.INSERT) > 0;
                CBUpdate.Checked = (authorisations & DBData.AUTHORISATION.UPDATE) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.UPDATE) > 0;
                CBDelete.Checked = (authorisations & DBData.AUTHORISATION.DELETE) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.DELETE) > 0;
                CBAdmin.Checked = (authorisations & DBData.AUTHORISATION.ADMIN) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.ADMIN) > 0;
                CBExecute.Checked = (authorisations & DBData.AUTHORISATION.EXECUTE) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.EXECUTE) > 0;

                CBRead.Enabled = (authorisations & DBData.AUTHORISATION.READ) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.READ) == 0;
                CBInsert.Enabled = (authorisations & DBData.AUTHORISATION.INSERT) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.INSERT) == 0;
                CBUpdate.Enabled = (authorisations & DBData.AUTHORISATION.UPDATE) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.UPDATE) == 0;
                CBDelete.Enabled = (authorisations & DBData.AUTHORISATION.DELETE) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.DELETE) == 0;
                CBAdmin.Enabled = (authorisations & DBData.AUTHORISATION.ADMIN) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.ADMIN) == 0;
                CBExecute.Enabled = (authorisations & DBData.AUTHORISATION.EXECUTE) > 0 || (inheritedTableAuthorisations & DBData.AUTHORISATION.EXECUTE) == 0;

                CBReadEffective.Checked = (effeciveAuthorisations & DBData.AUTHORISATION.READ) > 0;
                CBInsertEffective.Checked = (effeciveAuthorisations & DBData.AUTHORISATION.INSERT) > 0;
                CBUpdateEffective.Checked = (effeciveAuthorisations & DBData.AUTHORISATION.UPDATE) > 0;
                CBDeleteEffective.Checked = (effeciveAuthorisations & DBData.AUTHORISATION.DELETE) > 0;
                CBAdminEffective.Checked = (effeciveAuthorisations & DBData.AUTHORISATION.ADMIN) > 0;
                CBExecuteEffective.Checked = (effeciveAuthorisations & DBData.AUTHORISATION.EXECUTE) > 0;
            }
        }

        private void grantAuthorisation(int authorisation) {
            try {
                _db.connect();
                int accessorID = ch.psoft.Util.Validate.GetValid(accessors.SelectedItem.Value,-1);

                if (_tableName != "") {
                    if (_rowID > 0)
                        _db.grantRowAuthorisation(authorisation, accessorID, _tableName, _rowID);
                        if(_tableName.Equals("MATRIX"))
                        {
                           grantLocalKnowledge(_db, authorisation, accessorID);
                        }
                    else if (_column != "")
                        _db.grantColumnAuthorisation(authorisation, accessorID, _tableName, _column);
                    else
                        _db.grantTableAuthorisation(authorisation, accessorID, _tableName);
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
                errorText.Text = _map.get("MESSAGES", "unknownError");
                errorText.Visible = true;
            }
            finally {
                _db.disconnect();
            }
        }

        private void grantLocalKnowledge(DBData _db, int authorisation, int accessorID)
        {
            DataTable localKonowledge =  _db.getDataTable("SELECT KNOWLEDGE.ID FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                        + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID INNER JOIN "
                                                        + "KNOWLEDGE ON CHARACTERISTIC.KNOWLEDGE_ID = KNOWLEDGE.ID "
                                                        + "WHERE  MATRIX.ID = " + _rowID + " AND KNOWLEDGE.LOCAL = 1");
            foreach (DataRow row in localKonowledge.Rows)
            {
                _db.grantRowAuthorisation(authorisation, accessorID, "KNOWLEDGE", Convert.ToInt32(row["ID"]));
            }
        }

        private void revokeAuthorisation(int authorisation) {
            try {
                _db.connect();
                int accessorID = ch.psoft.Util.Validate.GetValid(accessors.SelectedItem.Value,-1);

                if (_tableName != "") {
                    if (_rowID > 0)
                    {
                        if (_tableName.Equals("MATRIX"))
                        {
                            revokeLocalKnowledge(_db, authorisation, accessorID);
                        }
                        _db.revokeRowAuthorisation(authorisation, accessorID, _tableName, _rowID);
                    }
                    else if (_column != "")
                        _db.revokeColumnAuthorisation(authorisation, accessorID, _tableName, _column);
                    else
                        _db.revokeTableAuthorisation(authorisation, accessorID, _tableName);
                }
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
                errorText.Text = _map.get("MESSAGES", "unknownError");
                errorText.Visible = true;
            }
            finally {
                _db.disconnect();
            }
        }

        private void revokeLocalKnowledge(DBData _db, int authorisation, int accessorID)
        {
            DataTable localKonowledge = _db.getDataTable("SELECT KNOWLEDGE.ID FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                        + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID INNER JOIN "
                                                        + "KNOWLEDGE ON CHARACTERISTIC.KNOWLEDGE_ID = KNOWLEDGE.ID "
                                                        + "WHERE  MATRIX.ID = " + _rowID + " AND KNOWLEDGE.LOCAL = 1");
            foreach (DataRow row in localKonowledge.Rows)
            {
                _db.revokeRowAuthorisation(authorisation, accessorID, "KNOWLEDGE", Convert.ToInt32(row["ID"]));
            }
        }
        protected void CBRead_CheckedChanged(object sender, System.EventArgs e) {
            if (CBRead.Checked)
                grantAuthorisation(DBData.AUTHORISATION.READ);
            else
                revokeAuthorisation(DBData.AUTHORISATION.READ);
        }

        protected void CBInsert_CheckedChanged(object sender, System.EventArgs e) {
            if (CBInsert.Checked)
                grantAuthorisation(DBData.AUTHORISATION.INSERT);
            else
                revokeAuthorisation(DBData.AUTHORISATION.INSERT);
        }

        protected void CBUpdate_CheckedChanged(object sender, System.EventArgs e) {
            if (CBUpdate.Checked)
                grantAuthorisation(DBData.AUTHORISATION.UPDATE);
            else
                revokeAuthorisation(DBData.AUTHORISATION.UPDATE);
        }

        protected void CBDelete_CheckedChanged(object sender, System.EventArgs e) {
            if (CBDelete.Checked)
                grantAuthorisation(DBData.AUTHORISATION.DELETE);
            else
                revokeAuthorisation(DBData.AUTHORISATION.DELETE);
        }

        protected void CBAdmin_CheckedChanged(object sender, System.EventArgs e) {
            if (CBAdmin.Checked)
                grantAuthorisation(DBData.AUTHORISATION.ADMIN);
            else
                revokeAuthorisation(DBData.AUTHORISATION.ADMIN);
        }

        protected void CBExecute_CheckedChanged(object sender, System.EventArgs e) {
            if (CBExecute.Checked)
                grantAuthorisation(DBData.AUTHORISATION.EXECUTE);
            else
                revokeAuthorisation(DBData.AUTHORISATION.EXECUTE);
        }

        protected void AddAccessorButton_Click(object sender, System.EventArgs e) {
            Response.Redirect("AccessorSelect.aspx?backURL=" + HttpUtility.UrlEncode("Authorisations.aspx?tableName=" + _tableName + "&rowID=" + _rowID + "&column=" + _column) + "&nextURL=" + HttpUtility.UrlEncode("AddAccessor.aspx?tableName=" + _tableName + "&rowID=" + _rowID + "&column=" + _column));
        }

        protected void RemoveAccessorButton_Click(object sender, System.EventArgs e) {
            int accessorID = ch.psoft.Util.Validate.GetValid(accessors.SelectedItem.Value,-1);

            if (_tableName != "") {
                try {
                    _db.connect();
                    if ((_db.getRowAuthorisations(accessorID, _tableName, _rowID, false, true) & ~_db.getRowAuthorisations(accessorID, _tableName, _rowID, false, false)) == 0){
                        if (_rowID > 0)
                        {
                            _db.revokeRowAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, accessorID, _tableName, _rowID);
                            if (_tableName.Equals("MATRIX"))
                            {
                                removeLocalKnowledge(_db, accessorID);
                            }
                        }
                        else if (_column != "")
                            _db.revokeColumnAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, accessorID, _tableName, _column);
                        else
                            _db.revokeTableAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, accessorID, _tableName);

                        accessors.Items.RemoveAt(accessors.SelectedIndex);
                        if (accessors.Items.Count > 0)
                            accessors.SelectedIndex = 0;
                        else
                            accessors.SelectedIndex = -1;

                        displayAuthorisations();
                    }
                    else{
                        _onloadString += "alert('" + _map.get("authorisation", "accessorCantBeDeleted") + "');";
                    }
                }
                catch (Exception ex) {
                    Logger.Log(ex,Logger.ERROR);
                    errorText.Text = _map.get("MESSAGES", "unknownError");
                    errorText.Visible = true;
                }
                finally {
                    _db.disconnect();
                }
            }
        }

        private void removeLocalKnowledge(DBData _db, int accessorID)
        {
            DataTable localKonowledge = _db.getDataTable("SELECT KNOWLEDGE.ID FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                        + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID INNER JOIN "
                                                        + "KNOWLEDGE ON CHARACTERISTIC.KNOWLEDGE_ID = KNOWLEDGE.ID "
                                                        + "WHERE  MATRIX.ID = " + _rowID + " AND KNOWLEDGE.LOCAL = 1");
            foreach (DataRow row in localKonowledge.Rows)
            {
                _db.revokeRowAuthorisation(DBData.AUTHORISATION.FULL_ACCESS, accessorID, "KNOWLEDGE", Convert.ToInt32(row["ID"]));
            }
        }

        protected void CBInheritance_CheckedChanged(object sender, System.EventArgs e) {
            try {
                _db.connect();
                _db.setInheritFlag(_tableName, _rowID, CBInheritance.Checked);
            }
            catch (Exception ex) {
                Logger.Log(ex,Logger.ERROR);
                errorText.Text = _map.get("MESSAGES", "unknownError");
                errorText.Visible = true;
            }
            finally {
                _db.disconnect();
            }
        }
    }
}

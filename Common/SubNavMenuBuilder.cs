using ch.appl.psoft.db;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for SubNavMenuBuilder.
    /// </summary>
    public class SubNavMenuBuilder {

        protected class MenuItem {
            public string _ID;
            public string _title;
            public string _link;
            public string _target;
            public bool _isGroup;
            public ListDictionary _itemList = new ListDictionary();
            public int _indentSize = 6;
            public int _indentOffset = 10;
            public bool _showBranchesOpen = false;
            private const string GROUP_PREFIX = "\u25c6 ";
            private const string ITEM_PREFIX = "";

            public MenuItem(string ID, bool isGroup, string title, string link, string target) {
                _ID = ID;
                _isGroup = isGroup;
                _title = title;
                _link = link;
                _target = target;
            }

            public void add(string parentID, MenuItem menuItem) {
                if (parentID == null || parentID == "")
                    _itemList.Add(menuItem._ID, menuItem);
                else {
                    MenuItem parentMenuItem = find(parentID);
                    if (parentMenuItem != null && parentMenuItem._isGroup)
                        parentMenuItem.add(null, menuItem);
                    else
                        add(null, menuItem);
                }
            }

            public MenuItem find(string ID) {
                MenuItem retValue = _itemList[ID] as MenuItem;

                if (retValue == null) {
                    foreach (MenuItem menuItem in _itemList.Values) {
                        retValue = menuItem.find(ID);
                        if (retValue != null)
                            break;
                    }
                }

                return retValue;
            }

            public void buildTableRow(Table subNavTable, int level) {
                Table baseTable = subNavTable;

                if (level >= 0) {
                    bool root = true;
                    if (level > 0)
                        root = false;

                    TableRow row = new TableRow();
                    row.ID = "MenuRow" + _ID;
                    row.Attributes.Add("root", root.ToString());
                    subNavTable.Rows.Add(row);

                    TableCell cell = new TableCell();
                    cell.Attributes.Add("root", root.ToString());
                    cell.Style.Add("padding-left", ((int) _indentOffset + (level > 0? _indentSize : 0)).ToString() + "px");
                    row.Cells.Add(cell);

                    HyperLink link = new HyperLink();
                    link.ID = _ID;
                    link.Attributes.Add("root", root.ToString());
                    cell.Controls.Add(link);

                    string cssClass = "subNavItem";
                    if (_isGroup) {
                        cssClass = "subNavGroup";
                        baseTable = new Table();
                        baseTable.ID = "MenuItemTable" + _ID;
                        baseTable.Attributes.Add("isMenuGroup", "True");
                        baseTable.CellPadding = 0;
                        baseTable.CellSpacing = 2;
                        baseTable.Width = Unit.Percentage(100);
                        baseTable.CssClass = cssClass;
                        cell.Controls.Add(baseTable);
                        link.Text = GROUP_PREFIX + _title;
                        if (!_showBranchesOpen){
                            baseTable.Style.Add("display", "none");
                            link.NavigateUrl = "javascript:;";
                            link.Attributes.Add("onClick", "ShowMenuGroup(" + baseTable.ClientID + ");");
                        }
                    }
                    else {
                        link.Target = _target;
                        link.NavigateUrl = _link;
                        link.Text = ITEM_PREFIX + _title;
                        link.Attributes.Add("onClick", "top.highlightSubNavigation('" + link.ClientID + "')");
                    }

                    row.CssClass = cell.CssClass = link.CssClass = cssClass;
                }

                if (_isGroup) {
                    foreach (MenuItem menuItem in _itemList.Values) {
                        menuItem.buildTableRow(baseTable, level+1);
                    }
                }
            }
        }


        public string _defaultTarget = "contentFrame";
        protected MenuItem _baseMenuItem = null;
        protected string _startPageLink = "";
        protected string _startPageTarget = "";
        private bool _dummyAdded = false;

        /// <summary>
        /// 
        /// </summary>
        public SubNavMenuBuilder() {
            _baseMenuItem = new MenuItem("_base_subNavMenuitem", true, "", "", _defaultTarget);
        }

        /// <summary>
        /// Title of the menu.
        /// </summary>
        public string Title {
            set{_baseMenuItem._title = value;}
            get{return _baseMenuItem._title;}
        }

        /// <summary>
        /// Link for the Title of the menu.
        /// </summary>
        public string TitleLink {
            set{_baseMenuItem._link = value;}
            get{return _baseMenuItem._link;}
        }

        /// <summary>
        /// Target window of the link for the Title of the menu.
        /// </summary>
        public string TitleTarget {
            set{_baseMenuItem._target = value != ""? value : _defaultTarget;}
            get{return _baseMenuItem._target;}
        }

        /// <summary>
        /// Builds the menu-table based on the defined groups and items.
        /// </summary>
        public void build(Table menuTable) {
            build(menuTable, null);
        }

        /// <summary>
        /// Build the menu based on the parent-ID.
        /// </summary>
        /// <param name="menuTable">Table, where the menu will be built on.</param>
        /// <param name="parentID">Identifier of the parent-menugroup which builds the base of the menu.</param>
        public void build(Table menuTable, string parentID) {
            if (!_dummyAdded){
                // empty dummy-item added to draw the separator-line after the last item.
                add(null, new MenuItem("dummy", false, "", "", ""));
                _dummyAdded = true;
            }
            menuTable.CellPadding = 0;
            menuTable.CellSpacing = 2;
            menuTable.CssClass = "subNavGroup";
            menuTable.Width = Unit.Percentage(100);

            MenuItem menuItem = _baseMenuItem;
            if (parentID != null && parentID != "")
                menuItem = _baseMenuItem.find(parentID);

            if (menuItem != null) {
                if (menuItem._title != "") {
                    TableRow titleRow = new TableRow();
                    menuTable.Rows.Add(titleRow);
                    titleRow.CssClass = "subNavTitle";
                    TableCell cell = new TableCell();
                    titleRow.Cells.Add(cell);
                    cell.CssClass = "subNavTitle";
                    if (menuItem._link != ""){
                        HyperLink link = new HyperLink();
                        cell.Controls.Add(link);
                        link.CssClass = "subNavTitle";
                        link.Text = menuItem._title;
                        link.NavigateUrl = menuItem._link;
                        link.Target = menuItem._target;
                    }
                    else {
                        cell.Text = menuItem._title;
                    }
                }
                menuItem.buildTableRow(menuTable, -1);
            }
        }

        /// <summary>
        /// Build the menu from the database.
        /// </summary>
        /// <param name="menuTable">Table, where the menu will be built on.</param>
        /// <param name="db">Instance of DBData.</param>
        /// <param name="parentID">Identifier of the parent-menugroup which builds the base of the menu.</param>
        public void build(Table menuTable, DBData db, string parentID) {
            add(db, parentID);
            build(menuTable, null);
        }

        private void add(DBData db, string parentID) {
            // add groups and items...
            string titleColumnName = db.langAttrName("MENUV", "TITLE");
            string sql = "select * from MENUV where PARENT_ID" + (parentID == null || parentID == "" ? " is null" : "="+parentID) + " order by ORDNUMBER asc, " + titleColumnName + " asc";
            DataTable table = db.getDataTable(sql, "MENUV");
            foreach (DataRow row in table.Rows) {
                if (db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, row, true, true)){
                    string ID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), "");
                    string title = ch.psoft.Util.Validate.GetValid(row[titleColumnName].ToString(), "");
                    string tablename = ch.psoft.Util.Validate.GetValid(row["TABLENAME"].ToString(), "");
                    switch (tablename){
                        case "MENUGROUP":
                            addMenuGroup(parentID, ID, title);
                            add(db, ID);
                            break;

                        case "MENUITEM":
                            string link = ch.psoft.Util.Validate.GetValid(row["LINK"].ToString(), "");
                            if (link.IndexOf("://") <= 0)
                                link = Global.Config.baseURL + link;
                            string target = ch.psoft.Util.Validate.GetValid(row["TARGET"].ToString(), "");
                            addMenuItem(parentID, ID, title, link, target);
                            if (ch.psoft.Util.Validate.GetValid(row["ISSTARTPAGE"].ToString(), 0) > 0){
                                StartPageLink = TitleLink = link;
                                StartPageTarget = TitleTarget = target;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Appends a MenuItem to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the MenuItem should be added.</param>
        /// <param name="menuItem">The MenuItem to be added.</param>
        private void add(string parentID, MenuItem menuItem) {
            _baseMenuItem.add(parentID, menuItem);
        }

        /// <summary>
        /// Appends a menugroup to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the new menugroup should be added.</param>
        /// <param name="ID">Identifier of the new menugrou.</param>
        /// <param name="title">Title of the new menugroup.</param>
        public void addMenuGroup(string parentID, string ID, string title) {
            add(parentID, new MenuItem(ID, true, title, "", _defaultTarget));
        }

        /// <summary>
        /// Appends a menu-item to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the new menu-item should be added.</param>
        /// <param name="ID">Identifier of the new menu-item.</param>
        /// <param name="title">Title of the new menu-item.</param>
        /// <param name="link">Link of the new mene-item. The target-frame is the default target-frame (usually contentFrame).</param>
        public void addMenuItem(string parentID, string ID, string title, string link) {
            add(parentID, new MenuItem(ID, false, title, link, _defaultTarget));
        }

        /// <summary>
        /// Appends a menu-item to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the new menu-item should be added.</param>
        /// <param name="ID">Identifier of the new menu-item.</param>
        /// <param name="title">Title of the new menu-item.</param>
        /// <param name="link">Link of the new mene-item.</param>
        /// <param name="target">The target-frame for the link.</param>
        public void addMenuItem(string parentID, string ID, string title, string link, string target) {
            if (target == null || target == "")
                target = _defaultTarget;
            add(parentID, new MenuItem(ID, false, title, link, target));
        }

        /// <summary>
        /// Sets the menu's start-page link.
        /// </summary>
        public string StartPageLink{
            get {return _startPageLink;}
            set {_startPageLink = value;}
        }

        /// <summary>
        /// Sets the menu's start-page target.
        /// </summary>
        public string StartPageTarget{
            get {return _startPageTarget != "" ? _startPageTarget : _defaultTarget;}
            set {_startPageTarget = value;}
        }
    }
}

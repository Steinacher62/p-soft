using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using System;

namespace ch.appl.psoft.LayoutControls
{
    /// <summary>
    /// Summary description for MenuControl.
    /// </summary>
    public partial class MenuControl : PSOFTUserControl {
        protected SubNavMenuBuilder _menuBuilder = null;

        public MenuControl() : base() {
            _menuBuilder = new SubNavMenuBuilder();
        }
        
        public static string Path {
            get {return Global.Config.baseURL + "/LayoutControls/MenuControl.ascx";}
        }

        protected string StartPageScript{
            get{
                string retValue = "";

                if (_menuBuilder.StartPageLink != ""){
                    retValue = "loadURLinFrame('" + _menuBuilder.StartPageTarget + "','" + _menuBuilder.StartPageLink + "')";
                }

                return retValue;
            }
        }

        #region Properties
        private string _baseID = "";
        /// <summary>
        /// ID of Menugroup which builds the base of the menu to display.
        /// </summary>
        public string BaseID {
            set {_baseID = value;}
            get {return _baseID;}
        }

        private string _mnemo = "";
        /// <summary>
        /// Mnemo of Menugroup in database which builds the base of the menu to display.
        /// Considered only if LoadFromDB is true.
        /// </summary>
        public string Mnemo {
            set {_mnemo = value;}
            get {return _mnemo;}
        }

        private bool _loadFromDB = false;
        /// <summary>
        /// If true, the menu is loaded from the database, based on Mnemo and BaseID.
        /// </summary>
        public bool LoadFromDB {
            set {_loadFromDB = value;}
            get {return _loadFromDB;}
        }

        /// <summary>
        /// Title of the base menugroup. Only considered if LoadFromDB is false.
        /// </summary>
        public string Title {
            set {_menuBuilder.Title = value;}
            get {return _menuBuilder.Title;}
        }

        /// <summary>
        /// Link for the Title of the base menugroup. Only considered if LoadFromDB is false.
        /// </summary>
        public string TitleLink {
            set {_menuBuilder.TitleLink = value;}
            get {return _menuBuilder.TitleLink;}
        }

        /// <summary>
        /// Target window for the Title of the base menugroup. Only considered if LoadFromDB is false.
        /// </summary>
        public string TitleTarget {
            set {_menuBuilder.TitleTarget = value;}
            get {return _menuBuilder.TitleTarget;}
        }

        /// <summary>
        /// Link of the startpage to display.
        /// </summary>
        public string StartPageLink {
            set {_menuBuilder.StartPageLink = value;}
            get {return _menuBuilder.StartPageLink;}
        }

        /// <summary>
        /// Target window where the startpage should be displayed.
        /// </summary>
        public string StartPageTarget {
            set {_menuBuilder.StartPageTarget = value;}
            get {return _menuBuilder.StartPageTarget;}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        /// <summary>
        /// If LoadFromDB is false: Appends a menugroup to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the new menugroup should be added.</param>
        /// <param name="ID">Identifier of the new menugrou.</param>
        /// <param name="title">Title of the new menugroup.</param>
        public void addMenuGroup(string parentID, string ID, string title) {
            _menuBuilder.addMenuGroup(parentID, ID, title);
        }

        /// <summary>
        /// If LoadFromDB is false: Appends a menu-item to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the new menu-item should be added.</param>
        /// <param name="ID">Identifier of the new menu-item.</param>
        /// <param name="title">Title of the new menu-item.</param>
        /// <param name="link">Link of the new mene-item. The target-frame is the default target-frame (contentFrame).</param>
        public void addMenuItem(string parentID, string ID, string title, string link) {
            _menuBuilder.addMenuItem(parentID, ID, title, link);
        }

        /// <summary>
        /// If LoadFromDB is false: Appends a menu-item to the menu.
        /// </summary>
        /// <param name="parentID">Identifier of the parent-menugroup where the new menu-item should be added.</param>
        /// <param name="ID">Identifier of the new menu-item.</param>
        /// <param name="title">Title of the new menu-item.</param>
        /// <param name="link">Link of the new mene-item.</param>
        /// <param name="target">The target-frame for the link.</param>
        public void addMenuItem(string parentID, string ID, string title, string link, string target) {
            _menuBuilder.addMenuItem(parentID, ID, title, link, target);
        }

        protected override void DoExecute() {
            base.DoExecute ();
            if (_loadFromDB) {
                DBData db = DBData.getDBData(Session);
                try {
                    db.connect();
                    if (_baseID == "")
                        _baseID = db.lookup("ID", "MENUGROUP", "MNEMO='" + _mnemo + "'", false);
					if (_menuBuilder.Title == "")
						_menuBuilder.Title = db.lookup("TITLE", "MENUGROUP", "ID=" + _baseID, false);
                    _menuBuilder.build(MenuTable, db, _baseID);
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    db.disconnect();
                }
            }
            else {
                _menuBuilder.build(MenuTable);
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

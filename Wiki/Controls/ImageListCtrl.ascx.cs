namespace ch.appl.psoft.Wiki.Controls
{
    using db;
    using Interface.DBObjects;
    using LayoutControls;
    using System;
    using System.Data;

    public partial  class ImageListCtrl : PSOFTListViewUserControl {
        protected DBData _db = null;

        protected System.Web.UI.WebControls.Button next;

        public static string Path {
            get {return Global.Config.baseURL + "/Wiki/Controls/ImageListCtrl.ascx";}
        }

        #region Properities

        protected long _ownerUID = -1L;
        public long OwnerUID {
            get {return _ownerUID;}
            set {_ownerUID = value;}
        }

        #endregion
    
        public ImageListCtrl() : base() {
            HeaderEnabled = true;
            DeleteEnabled = true;
            DetailEnabled = false;
            EditEnabled = false;
            UseJavaScriptToSort = false;
            UseFirstLetterAsPageSelector = true;
            CheckOrder = true;
            OrderColumn = "TITLE";
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            _db = DBData.getDBData(Session);
            if (Visible)
                loadList();
        }

        private void loadList() {
            _db.connect();
            try {
                string sql = "select * from " + WikiImage._TABLENAME + " where OWNER_UID=" + OwnerUID;
                pageTitle.Text = _mapper.get(WikiModule.LANG_SCOPE_WIKI, WikiModule.LANG_MNEMO_ST_IMAGES).Replace("#1", _db.UID2NiceName(OwnerUID, _mapper));

                sql += " order by " + OrderColumn + " " + OrderDir;

                DataTable table = _db.getDataTableExt(sql, WikiImage._TABLENAME);               
                IDColumn = "ID";

                listTable.Rows.Clear();
                LoadList(_db, table, listTable);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                _db.disconnect();
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
		
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
		#endregion
    }
}

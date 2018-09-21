namespace ch.appl.psoft.MbO.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for DetailView.
    /// </summary>

    public partial class OrganisationView : PSOFTDetailViewUserControl {
        protected long _contextId = 0;
        protected long _id = 0;
        protected long _rootId = 0;

        private DBData _db = null;

        protected System.Web.UI.WebControls.Table detailTab;

        public static string Path
        {
            get { return Global.Config.baseURL + "/MbO/Controls/OrganisationView.ascx"; }
        }

        #region Properities

        /// <summary>
        /// Get/Set context (=organisation) id 
        /// </summary>
        public long contextId
        {
            get { return _contextId; }
            set { _contextId = value; }
        }

        /// <summary>
        /// Get/Set objective id
        /// </summary>
        public long id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Get encoded back url
        /// </summary>
        public long rootId
        {
            get { return _rootId; }
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute();

            _db = DBData.getDBData(Session);

            _db.connect();
            try
            {
                if (!IsPostBack)
                {
                    oe.Text = _mapper.get("mbo", "mainOrganisation");
                    string title = _db.langAttrName("ORGANISATION", "TITLE");
                    DataTable table = _db.getDataTable("select ORGENTITY_ID," + title + " from ORGANISATION where MAINORGANISATION = 1 order by " + title);
                    selectOE.DataSource = table;
                    selectOE.DataTextField = title;
                    selectOE.DataValueField = "ORGENTITY_ID";
                    selectOE.DataBind();
                    if (_contextId > 0)
                    {
                        int idx = 0;
                        foreach (ListItem item in selectOE.Items)
                        {
                            if (item.Value == _contextId.ToString())
                            {
                                selectOE.SelectedIndex = idx;
                                break;
                            }
                            idx++;
                        }
                    }
                }
                if (selectOE.SelectedIndex >= 0) _rootId = ch.psoft.Util.Validate.GetValid(selectOE.Items[selectOE.SelectedIndex].Value, 0L);
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
        private void oeChanged(object sender, System.EventArgs e)
        {
            _rootId = ch.psoft.Util.Validate.GetValid(selectOE.Items[selectOE.SelectedIndex].Value, 0L);
        }

        private void mapControls()
        {
            selectOE.ServerChange += new System.EventHandler(oeChanged);
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

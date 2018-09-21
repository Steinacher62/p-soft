using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Web;

namespace ch.appl.psoft.MbO
{
    /// <summary>
    /// Summary description for OrganisationTree.
    /// </summary>
    public partial class OrganisationTree : System.Web.UI.Page {
        protected long _rootId = 0;
        protected long _highlightOEId = 0;

        protected void Page_Load(object sender, System.EventArgs e) {
            _rootId = ch.psoft.Util.Validate.GetValid(Request.QueryString["rootId"],0L);   
            _highlightOEId = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"],_highlightOEId);   
        }
        /// <summary>
        /// Get encoded back url
        /// </summary>
        public string encodeBackURL {
            get {return HttpUtility.UrlEncode("OrganisationTree.aspx?rootId="+_rootId+"&id=");}
        }
	
        protected string buildTree {
            get {
                DBData db = DBData.getDBData(Session);

                Response.Write("<script language=\"javascript\">\n");
                Common.Tree tree = new Common.Tree("ORGENTITY", "OBJECTIVEOEV", "ORGENTITY_ID", Response, "", ""); 
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.LeafOrderColumn = "TITLE";
                tree.BranchToolTipColum = "DESCRIPTION";
                tree.LeafToolTipColum = "DESCRIPTION";
                tree.build(db, _rootId, "organisation");
                return "</script>";
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



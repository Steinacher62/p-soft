namespace ch.appl.psoft.Organisation.Controls
{
    using ch.appl.psoft.LayoutControls;
    using Common;
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;
    using System.Web;

    /// <summary>
    ///		Summary description for AssignGroupTree.
    /// </summary>
    public partial class AssignGroupTree : PSOFTInputViewUserControl
	{
		public const string PARAM_TABLE_NAME = "PARAM_TABLE_NAME";
        public const string PARAM_OWNERTTABLE = "PARAM_OWNERTTABLE";
        public const string PARAM_XID = "PARAM_XID";
        public const string PARAM_BACK_URL = "PARAM_BACK_URL";
        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public const string PARAM_OWNER_ID = "PARAM_OWNER_ID";

        protected Config _config = null;
		protected DBData _db = null;
		protected DataTable _table;
		private ArrayList _registryList = new ArrayList();
		private ArrayList _parentRegistryList = new ArrayList();


		public static string Path
		{
			get {return Global.Config.baseURL + "/Organisation/Controls/AssignGroupTree.ascx";}
		}

		#region Properities
        public string TableName
        {
            get {return GetString(PARAM_TABLE_NAME);}
            set {SetParam(PARAM_TABLE_NAME, value);}
        }
        
        public string BackUrl {
            get {return GetString(PARAM_BACK_URL);}
            set {SetParam(PARAM_BACK_URL, value);}
        }
        
        public string NextUrl 
        {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public long XID
        {
            get {return GetLong(PARAM_XID);}
            set {SetParam(PARAM_XID, value);}
        }

        public string OwnerTable
        {
            get {return GetString(PARAM_OWNERTTABLE);}
            set {SetParam(PARAM_OWNERTTABLE, value);}
        }

        public long OwnerID
        {
            get {return GetLong(PARAM_OWNER_ID);}
            set {SetParam(PARAM_OWNER_ID, value);}
        }
        #endregion

		protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();

            _config = Global.Config;
            _db = DBData.getDBData(Session);

            try 
            {
                if (!IsPostBack) 
                {
                    apply.Text = _mapper.get("apply");                   
                }

                _db.connect();


                if (OwnerTable != "" && OwnerID > 0)
                {
                    switch(OwnerTable)
                    {
                        case "TASKLIST":
                            _registryList = new ArrayList(_db.Orgentity.getOrgentityIDs(OwnerID.ToString(), Orgentity.TABLE_TASKLIST_GROUP).Split(','));
                            break;
                        case "CONTACTV":
                            string contactTable = DBColumn.GetValid(_db.lookup("TABLENAME",OwnerTable,"ID="+OwnerID),"");
                            switch(contactTable)
                            {
                                case "FIRM":
                                    _registryList = new ArrayList(_db.Orgentity.getOrgentityIDs(OwnerID.ToString(), Orgentity.TABLE_CONTACT_GROUP_FIRM).Split(','));
                                    break;
                                case "PERSON":
                                    _registryList = new ArrayList(_db.Orgentity.getOrgentityIDs(OwnerID.ToString(), Orgentity.TABLE_CONTACT_GROUP_PERSON).Split(','));
                                    break;
                            }
                            break;
                        case "PROJECT":
                            _registryList = new ArrayList(_db.Orgentity.getOrgentityIDs(OwnerID.ToString(), Orgentity.TABLE_PROJECT_GROUP).Split(','));
                            break;
                    }
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

        protected string buildTree 
        {
            get 
            {
                Common.Tree tree = new Common.Tree("ORGENTITY", Response, "");
                tree.extendNode += new ExtendNodeHandler(this.extendNode);
                tree.MaxCaptionLength = SessionData.getMaxTreeCaptionLength(Session);
                tree.BranchToolTipColum = "DESCRIPTION";
                Response.Write("<script language=\"javascript\">\n");
                
                tree.build(_db, _db.Orgentity.getRootID(), "oeTree");
               
                return "</script>";
            }
        }

        private bool extendNode(HttpResponse response, string nodeName, DataRow row, int level) 
		{
			if (_registryList.Contains(row["ID"].ToString())) 
			{
				response.Write(nodeName+".prependHTML=\"<input type='checkbox' checked ID='RegFlag-"+row["ID"]+"'>\";\n");
				response.Write(nodeName+".setInitialOpen(1);\n");
			}
			else if (_parentRegistryList.Contains(row["ID"].ToString())) 
			{
				response.Write(nodeName+".prependHTML = \"<input type='checkbox' checked disabled ID='RegFlag-"+row["ID"]+"'>\";\n");
				response.Write(nodeName+".setInitialOpen(1);\n");
			}
			else response.Write(nodeName+".prependHTML = \"<input type='checkbox' ID='RegFlag"+row["ID"]+"'>\";\n");
			return true;
		}
		

        
        private void mapControls() {
            apply.Click += new System.EventHandler(apply_Click);
            apply.Attributes["onClick"] = "readRegistryFlag(" + registryFlags.ClientID + ");";
		}

        

        private void apply_Click(object sender, System.EventArgs e) 
        {

            _db.connect();
            try 
            {
                _db.beginTransaction();                
                
                if (registryFlags.Value != "") 
                {                    
                    _db.Orgentity.updateOrgentityGroupAssignmentEntries(registryFlags.Value, TableName, OwnerID);
                }  

                _db.commit();

                
            }
            catch (Exception ex) 
            {
                _db.rollback();
                DoOnException(ex);
            }
            finally 
            {
                _db.disconnect();   
            }

            if (NextUrl != "") 
            {
                Response.Redirect(NextUrl);
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
			mapControls();
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}

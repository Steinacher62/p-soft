namespace ch.appl.psoft.LayoutControls
{
    using db;
    using Interface;
    using Interface.DBObjects;
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    ///		Summary description for LinkGroupControl.
    /// </summary>
    public partial  class LinkGroupControl : PSOFTUserControl
	{

		private LinkSets _linkSets = new LinkSets();

		public static string Path
		{
			get {return Global.Config.baseURL + "/LayoutControls/LinkGroupControl.ascx";}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		protected override void OnPreRender( System.EventArgs e )
		{
            if (_linkSets.Count > 0) {
                lnkCellList.Text = _linkSets.ToString();
            }
            else{
                Visible = false;
            }
			base.OnPreRender(e);
		}

        public override bool Visible{
            get{return _linkSets.Count > 0;}
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public string Caption
		{
			get {return this.captionLabel.Text;}
			set {this.captionLabel.Text = value;}
		}

		public void AddLink(string linkSetName, string itemCaption, string itemLink)
		{
			AddLink(linkSetName, itemCaption, itemLink, "");
		}

        public void AddLink(string linkSetName, string itemCaption, string itemLink, string parameters) {
            AddLink(linkSetName, itemCaption, itemLink, parameters, "");
        }

        public void AddLink(string linkSetName, string itemCaption, string itemLink, string parameters, string target)
		{
			_linkSets.Add(linkSetName, itemCaption, itemLink, parameters, target);
		}

        /// <summary>
        /// Sorts the links in the specified link-set
        /// </summary>
        /// <param name="linkSetName">Name of the link-set within this link-group</param>
        public void Sort(string linkSetName){
            _linkSets.Sort(linkSetName);
        }

        /// <summary>
        /// Adds a list of links to p-soft-objects which are assigned through UID-assignments.
        /// Links are only added if the logged user has read-right on the linked object.
        /// </summary>
        /// <param name="linkSetName">Name of the link-set within this link-group</param>
        /// <param name="fromTablename">Tablename of source-object</param>
        /// <param name="fromID">ID of source-object</param>
        /// <param name="toTablenames">Array of target-tablenames to consider, or null if all should be considered</param>
        /// <param name="excludedToTablenames">Array of target-tablenames to exclude, or null if none should be excluded</param>
        /// <param name="typ">Type of assignment or -1 to consider any type</param>
        /// <param name="structureID">ID of UID-structure to which the new assignment belongs, or -1 if its a general assignment</param>
        /// <param name="includeGlobalAssignments">Should global assignments be included?</param>
        /// <param name="includePersonalAssignments">Should personal assignments be included?</param>
        /// <param name="mapper">Language-mapper</param>
        /// <param name="showObjectType">If true, appends the type of the objects in brackets.</param>
        public void AddUIDAssignmentLinks(string linkSetName, string fromTablename, long fromID, string[] toTablenames, string[] excludedToTablenames, int typ, long structureID, bool includeGlobalAssignments, bool includePersonalAssignments, LanguageMapper mapper, bool showObjectType){
            DBData db = DBData.getDBData(Session);
            db.connect();
            try{
                Hashtable UIDs = new Hashtable();
                if (includeGlobalAssignments){
                    // global assignments...
                    long[] globalUIDs = db.getUIDAssignments(fromTablename, fromID, toTablenames, excludedToTablenames, typ, -1L, structureID);
                    foreach(long UID in globalUIDs){
                        if (!UIDs.ContainsKey(UID)){
                            UIDs.Add(UID, db.UID2NiceName(UID, mapper, showObjectType));
                        }
                    }
                }

                if (includePersonalAssignments){
                    // personal assignments...
                    long[] personalUIDs = db.getUIDAssignments(fromTablename, fromID, toTablenames, excludedToTablenames, typ, db.userId, structureID);
                    foreach(long UID in personalUIDs){
                        if (!UIDs.ContainsKey(UID)){
                            UIDs.Add(UID, db.UID2NiceName(UID, mapper, showObjectType));
                        }
                    }
                }

                foreach(long UID in UIDs.Keys){
                    if (db.hasUIDAuthorisation(DBData.AUTHORISATION.READ, UID, DBData.APPLICATION_RIGHT.COMMON, true, true)){
                        AddLink(linkSetName, (string)UIDs[UID], psoft.Goto.GetURL("UID", UID));
                    }
                }
                Sort(linkSetName);
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        /// <summary>
        /// Adds context-links for the abo-management.
        /// </summary>
        /// <param name="tablename">Tablename of the abo-capable object</param>
        /// <param name="ID">ID of the abo-capable object</param>
        /// <param name="mapper">Language-mapper</param>
        public void AddAboLinks(string tablename, long ID, LanguageMapper mapper){
            if (Global.Config.ShowNews){
                DBData db = DBData.getDBData(Session);
                db.connect();
                try{
                    DataTable subsTab = db.getDataTable(db.Subscription.subscriptionQuery(db.userId, tablename, ID, false) + " order by title");
                    AddLink(mapper.get("news", "subscriptions"), mapper.get("news", "addSUBSCRIPTION"), psoft.Subscription.Detail.GetURL("view","add", "context",News.CONTEXT.SUBSCRIPTION, "triggerName",tablename, "triggerId",ID, "backURL",Request.RawUrl));
                    foreach (DataRow row in subsTab.Rows) {
                        AddLink(mapper.get("news","subscriptions"), row["TITLE"].ToString(), psoft.Subscription.Detail.GetURL("view","edit", "context",News.CONTEXT.SUBSCRIPTION, "triggerName",tablename, "triggerId",ID, "id",row["ID"], "backURL",Request.RawUrl), " title='" + row["TITLE"] + " " + mapper.get("edit") + "'");
                    }
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    db.disconnect();
                }
            }
        }
	}
}

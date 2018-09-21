namespace ch.appl.psoft.Subscription.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using Interface.DBObjects;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    /// <summary>
    ///		Summary description for ListView.
    /// </summary>
    public partial  class ListView : PSOFTListViewUserControl {
        protected string _backURL = "";
        protected string _query = "";
        protected string _context = News.CONTEXT.SUBSCRIPTION.ToString();
        protected long _id = 0;
        protected long _currentId = 0;
        protected string _title = "";
        protected bool _isFirst = true;


        public static string Path {
            get {return Global.Config.baseURL + "/Subscription/Controls/ListView.ascx";}
        }

        #region Properities


        /// <summary>
        /// Get/Set list id
        /// </summary>
        public long id {
            get {return _id;}
            set {_id = value;}
        }
        /// <summary>
        /// Get/Set query for the list
        /// </summary>
        public string query {
            get {return _query;}
            set {_query = value;}
        }
        /// <summary>
        /// Get/Set back url
        /// </summary>
        public string backURL {
            get {return _backURL;}
            set {_backURL = value;}
        }

        /// <summary>
        /// Get/set current list element id
        /// </summary>
        public long currentId {
            get {return _currentId;}
            set {_currentId = value;}
        }
        /// <summary>
        /// Get/set context (objective, person, oe, job)
        /// </summary>
        public string context {
            get {return _context;}
            set {_context = value;}
        }

        /// <summary>
        /// Get/set page title
        /// </summary>
        public string title {
            get {return _title;}
            set {_title = value;}
        }
        #endregion
    
        protected void Page_Load(object sender, System.EventArgs e) {
            if (base.Visible) base.Execute();
        }
        protected override void DoExecute() {
            if (!IsPostBack) {
                titleLbl.Text = title;
                all.Text = _mapper.get("all");
                loadList();
            }
        }

        private void loadList() {
            DBData db = DBData.getDBData(Session);
            string sql = "";
            DataTable table = null;
                
            DetailEnabled = true;
            DeleteEnabled = true;
            EditEnabled = _context == News.CONTEXT.SUBSCRIPTION.ToString();
            InfoBoxEnabled = true;
            HeaderEnabled = true;
            string url = psoft.Subscription.Detail.GetURL("view","detail", "id","%ID", "context",_context);
            base.DetailURL = url;
            url = psoft.Subscription.Detail.GetURL("view","edit", "id","%ID", "context",_context);
            base.EditURL = url;
            db.connect();
            try {
				if (_query != "") {
					sql = _query;
				} else if (_context == News.CONTEXT.NEWS.ToString()) {
					sql = "select NEWS.*,SUBSCRIPTION.TYP,SUBSCRIPTION.ID SUBSID,NEWSASSIGN.ID DELID from (NEWS inner join NEWSASSIGN on NEWS.ID = NEWSASSIGN.NEWS_ID) inner join SUBSCRIPTION on NEWSASSIGN.SUBSCRIPTION_ID = SUBSCRIPTION.id where (SUBSCRIPTION.person_id="+db.userId+" or SUBSCRIPTION.typ = 1) and SUBSCRIPTION.EMAILENABLE = 0 and NEWS.VISIBILITY = 1 and ISNULL(NEWS.VALID_FROM,GETDATE()) <= GETDATE() ";
					
					if (!all.Checked) {
						sql += " and ISNULL(NEWS.VALID_FROM,GETDATE()) <= GETDATE() and ISNULL(NEWS.VALID_TO,GETDATE()) >= GETDATE()";
						sql += " and SUBSCRIPTION.ACTIVE = 1 and ISNULL(SUBSCRIPTION.VALID_FROM,GETDATE()) <= GETDATE() and ISNULL(SUBSCRIPTION.VALID_TO,GETDATE()) >= GETDATE()";
					}//if

				} else if (_context == News.CONTEXT.SUBSCRIPTION.ToString()) {
					sql = "select * from SUBSCRIPTION where TYP = 0 and PERSON_ID="+db.userId;
					
					if (!all.Checked) {
						sql += " and ACTIVE = 1 and ISNULL(VALID_FROM,GETDATE()) <= GETDATE() and ISNULL(VALID_TO,GETDATE()) >= GETDATE()";
					}//if

				} else { 
					return;
				}//if
                deleteMessage = _mapper.get("news", "delete"+_context+"Confirm");

                sql += " order by SUBSCRIPTION.typ desc," + OrderColumn + " " + OrderDir;

                table = db.getDataTableExt(sql,_context);  
                listTab.Rows.Clear();

                if (table.Rows.Count > 0){
                    foreach (DataRow row in table.Rows) {

                        if (context == News.CONTEXT.NEWS.ToString()) addNews (db, table, row);
                        else if (context == News.CONTEXT.SUBSCRIPTION.ToString()) addSubscription (db, table, row);
                    }			
                }
                else{
                    TableRow r = new TableRow(); 
                    TableCell c = new TableCell(); 
                    listTab.Rows.Add(r);
                    r.Cells.Add(c);
                    c.CssClass = "List";
                    c.Text = _mapper.get("noEntriesFound");
                }
            }
            catch (Exception ex) {
                DoOnException(ex);
            }
            finally {
                db.disconnect();
            }
        }

        private void addSubscription (DBData db, DataTable table, DataRow row) {
            TableRow r;
            TableCell c;
            HyperLink link;
            long triggerId = db.News.getTriggerId(row);
				
            // title
            r = new TableRow(); 
            c = new TableCell(); 
            listTab.Rows.Add(r);
            r.Cells.Add(c);
            c.ColumnSpan = 5;
            c.CssClass = "Detail_special";
            if (_isFirst){
                c.Style.Add("border-top", "1px solid #CCC;");
                _isFirst = false;
            }
            link = new HyperLink();
            c.Controls.Add(link);
            link.Text = db.GetDisplayValue(table.Columns[db.langAttrName("SUBSCRIPTION","TITLE")],row[db.langAttrName("SUBSCRIPTION","TITLE")],true);
            link.NavigateUrl = psoft.Subscription.Detail.GetURL("context",News.CONTEXT.SUBSCRIPTION, "id",row["ID"], "view","detail");

            // Trigger
            r = new TableRow(); 
            c = new TableCell(); 
            listTab.Rows.Add(r);
            c.ColumnSpan = 5;
            r.Cells.Add(c);
            //c.CssClass = "Detail_special";
            string name = db.News.getTriggerName(row);
            if (name != "") {
                Label lbl = new Label();
                lbl.Text = _mapper.get("news",name)+": ";
                c.Controls.Add(lbl);

                link = new HyperLink();
                c.Controls.Add(link);
				if(name == "KNOWLEDGE")
				{					
					link.Text = db.Knowledge.getTitle((long) row["TRIGGER_ID"]);
				}
				else
				{
					link.Text = db.News.getTriggerValue(row);
				}
                if (triggerId > 0) setTriggerURL(link, name, db.News.getTriggerUID(row));
            }
            // event
            r = new TableRow(); 
            c = new TableCell();
            c.CssClass = "detail_data";
            c.Style.Add("border-bottom", "1px solid #CCC;");
            r.Cells.Add(c);
            listTab.Rows.Add(r);
            int ev = DBColumn.GetValid(row["EVENTS"],0);
            if (ev > 0) {
                c.Text = _mapper.get("SUBSCRIPTION", "EVENTS");
                c.Text += ": ";

                c = new TableCell();
                c.Style.Add("border-bottom", "1px solid #CCC;");
                r.Cells.Add(c);

                c.Text = "";
                if (((int)News.ACTION.DELETE & ev) > 0) {
                    c.Text = _mapper.get("delete");
                }
                if (((int)News.ACTION.EDIT & ev) > 0) {
                    if (c.Text != "") c.Text += ", ";
                    c.Text += _mapper.get("edit");
                }
                if (((int)News.ACTION.NEW & ev) > 0) {
                    if (c.Text != "") c.Text += ", ";
                    c.Text += _mapper.get("insert");
                }
            }

            // edit
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_data";
            c.Style.Add("border-bottom", "1px solid #CCC;");
            link = new HyperLink();
            link.CssClass = "List";
            c.Controls.Add(link);
            link.NavigateUrl = psoft.Subscription.Detail.GetURL("context",News.CONTEXT.SUBSCRIPTION, "view","edit", "id",row["ID"], "backURL",_backURL);
            link.Text = "E";
            link.ToolTip = _mapper.get("edit");

            // delete
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_data";
            c.Style.Add("border-bottom", "1px solid #CCC;");
            link = new HyperLink();
            link.CssClass = "List";
            c.Controls.Add(link);
            c.EnableViewState = false;
            link.NavigateUrl = "javascript: deleteRowConfirm('"+_context+"','"+table.Columns["ID"]+"',"+row["ID"]+")";
            link.Text = "D";
            link.ToolTip = _mapper.get("delete");

            // semaphore
            c = new TableCell(); 
            c.HorizontalAlign = HorizontalAlign.Center;
            c.Style.Add("border-bottom", "1px solid #CCC;");
            r.Cells.Add(c);
            System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
            c.Controls.Add(image);
            if (triggerId == 0) {
                image.ImageUrl = "../../images/ampelRot.gif";
                image.ToolTip = _mapper.get("news","invalidSubscription");
            }
            else if (DBColumn.GetValid(row["ACTIVE"],0) != 1)  {
                image.ImageUrl = "../../images/ampelGrau.gif";
                image.ToolTip = _mapper.get("news","inactiveSubscription");
            }
            else if (!all.Checked || db.exists("SUBSCRIPTION","ID = "+row["ID"]+" and VALID_FROM <= GETDATE() and ISNULL(VALID_TO,GETDATE()) >= GETDATE()")) {
                image.ImageUrl = "../../images/ampelGruen.gif";
                image.ToolTip = _mapper.get("news","activeSubscription");
            }
            else {
                image.ImageUrl = "../../images/ampelGrau.gif";
                image.ToolTip = _mapper.get("news","inactiveSubscription");
            }

        }
        private void addNews (DBData db, DataTable table, DataRow row) {
            TableRow r;
            TableCell c;
            HyperLink link;
            string temp;
            int typ = DBColumn.GetValid(row["TYP"],0);
            long triggerId = db.News.getTriggerId(row);
						
            // title
            r = new TableRow(); 
            c = new TableCell(); 
            listTab.Rows.Add(r);
            r.Cells.Add(c);
            c.CssClass = "Detail_special";
            if (_isFirst){
                c.Style.Add("border-top", "1px solid #CCC;");
                _isFirst = false;
            }
            c.ColumnSpan = 2;
            if (typ == 1) {
                // title
                Label lbl = new Label();
                lbl.Text = db.GetDisplayValue(table.Columns[db.langAttrName("NEWS","TITLE")],row[db.langAttrName("NEWS","TITLE")],true) + " ";
                c.Controls.Add(lbl);
                // link
                link = new HyperLink();
                link.CssClass = "List";
                c.Controls.Add(link);
                temp = db.GetDisplayValue(table.Columns["URL"],row["URL"], false);
                if (temp != "") {
                    link.NavigateUrl = temp.StartsWith("http://")? temp : "http://" + temp;
                    link.Text = ch.psoft.Util.Validate.GetValid(db.GetDisplayValue(table.Columns[db.langAttrName("NEWS","URL_TEXT")],row[db.langAttrName("NEWS","URL_TEXT")],true), temp);
                    link.Target = db.GetDisplayValue(table.Columns["TARGET"],row["TARGET"],false) == "0"? "_self" : "_blank";
                    listTab.Rows.Add(r);
                }
                // description
                r = new TableRow(); 
                c = new TableCell(); 
                listTab.Rows.Add(r);
                r.Cells.Add(c);
                c.Text = db.GetDisplayValue(table.Columns[db.langAttrName("NEWS","DESCRIPTION")],row[db.langAttrName("NEWS","DESCRIPTION")],true);
            }
            else {
                string name = db.News.getTriggerName(row);
                if (name != "") {
                    Label lbl = new Label();

                    lbl.Text = _mapper.get("news",name)+": ";
                    c.Controls.Add(lbl);

                    // Trigger
                    link = new HyperLink();
                    c.Controls.Add(link);
					if(name == "KNOWLEDGE")
					{					
						link.Text = db.Knowledge.getTitle((long) row["TRIGGER_ID"]);
					}
					else
					{
						link.Text = db.News.getTriggerValue(row);
					}

                    if (triggerId > 0) setTriggerURL(link, name, db.News.getTriggerUID(row));

                    DataTable subsTab = db.getDataTable("select * from subscription where id = "+row["SUBSID"]);
                    if (subsTab.Rows.Count > 0 && DBColumn.GetValid(subsTab.Rows[0]["TRIGGER_ID"],0L) != DBColumn.GetValid(row["TRIGGER_ID"],-1L)) {
                        lbl = new Label();
                        lbl.CssClass = "detail_data";
                        lbl.Text = " (";
                        name = db.News.getTriggerName(subsTab.Rows[0]);
                        lbl.Text += _mapper.get("news",name)+": ";
                        lbl.Text += db.News.getTriggerValue(subsTab.Rows[0]);
                        lbl.Text += ")";
                        c.Controls.Add(lbl);
                    }
                }
            }

            // event
            r = new TableRow(); 
            c = new TableCell();
            c.Style.Add("border-bottom", "1px solid #CCC;");
            r.Cells.Add(c);
            int ev = DBColumn.GetValid(row["EVENT"],0);
            if (ev > 0) {
                c.CssClass = "detail_data";
				string name = db.News.getTriggerName(row);
				if(name == "KNOWLEDGE")
				{
						c.Text = _mapper.get("news", "NEWVERSION");
				}
					else
				{
						c.Text = _mapper.get("news", ((News.ACTION) ev).ToString());
				}
                temp = db.GetDisplayValue(table.Columns["CREATED"],row["CREATED"],true);
                c.Text += temp;
                c.Text += " (";
                c.Text += db.Person.getWholeName(row["PERSON_ID"].ToString(), false);
                c.Text += ")";
                listTab.Rows.Add(r);
            }

            // delete
            if (typ == 0) {
                c = new TableCell();
                r.Cells.Add(c);
                c.Style.Add("border-bottom", "1px solid #CCC;");
                c.CssClass = "Detail_data";
                link = new HyperLink();
                link.CssClass = "List";
                c.Controls.Add(link);
                c.EnableViewState = false;
                link.NavigateUrl = "javascript: deleteRowConfirm('"+_context+"','"+table.Columns["ID"]+"',"+row["DELID"]+")";
                link.Text = "D";
                link.ToolTip = _mapper.get("delete");
            }
        }
        private void setTriggerURL(HyperLink link, string name, long uID) {
            link.NavigateUrl = psoft.Goto.GetURL("uid",uID);
        }

        protected override void onAddRow(DataRow row, TableRow r) {
        }
        
        protected override void onAddCell(DataRow row, DataColumn col, TableRow r, TableCell c) {
        }
        
        private void mapControls() {
            all.CheckedChanged += new System.EventHandler(allChanged);
        }
        
        private void allChanged(object sender, System.EventArgs e) {
            loadList();
        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {

		}
		#endregion
    }
}

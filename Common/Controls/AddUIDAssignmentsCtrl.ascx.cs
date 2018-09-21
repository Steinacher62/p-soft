namespace ch.appl.psoft.Common.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Data;
    using System.Web.UI.WebControls;


    public partial class AddUIDAssignmentsCtrl : PSOFTMapperUserControl {
        protected DBData _db;



        const string VALUE_MATRIX = "matrix";
        const string VALUE_SLAVE  = "slave";

        public static string Path {
            get {return Global.Config.baseURL + "/Common/Controls/AddUIDAssignmentsCtrl.ascx";}
        }

		#region Properities

        public const string PARAM_NEXT_URL = "PARAM_NEXT_URL";
        public string NextURL {
            get {return GetString(PARAM_NEXT_URL);}
            set {SetParam(PARAM_NEXT_URL, value);}
        }

        public const string PARAM_FROM_UID = "PARAM_FROM_UID";
        public long FromUID {
            get {return GetLong(PARAM_FROM_UID);}
            set {SetParam(PARAM_FROM_UID, value);}        
        }

        private int fromFCK = 0;
        public int FromFCK
        {
            get { return fromFCK; }
            set { fromFCK = value; }
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            Execute();
        }

        protected override void DoExecute() {
            base.DoExecute ();
            _db = DBData.getDBData(Session);

            if (!IsPostBack) {
                next.Text = _mapper.get("next");
                lblObjectTypes.Text = _mapper.get("uidAssignment", "lblObjectTypes");
                lblStructure.Text = _mapper.get("uidAssignment", "lblStructure");
                lblPersonal.Text = _mapper.get("uidAssignment", "lblPersonal");
                lblTyp.Text = _mapper.get("uidAssignment", "lblTyp");

                _db.connect();
                try {
                    // types...
                    if (Global.isModuleEnabled("knowledge")){
                        addObjectType(_mapper.get("uidAssignment", "objKnowledge"), "knowledge");
                    }
                    addObjectType(_mapper.get("uidAssignment", "objPerson"), "person");
                    addObjectType(_mapper.get("uidAssignment", "objDocument"), "document");
					addObjectType(_mapper.get("clipboard"), "clipboard");
                    
					if (Global.isModuleEnabled("contact")){
                        addObjectType(_mapper.get("uidAssignment", "objContact"), "contact");
                    }
                    if (Global.isModuleEnabled("tasklist")){
                        addObjectType(_mapper.get("uidAssignment", "objTasklist"), "tasklist");
                        addObjectType(_mapper.get("uidAssignment", "objMeasure"), "measure");
                    }
                    if (Global.isModuleEnabled("project")){
                        addObjectType(_mapper.get("uidAssignment", "objProject"), "project");
                    }

                    //Matrix und Slave UUID
                    if(Global.isModuleEnabled("morph")) {
                        addObjectType(_mapper.get("uidAssignment", "objMatrix"), VALUE_MATRIX);
                        addObjectType(_mapper.get("uidAssignment", "objSlaves"), VALUE_SLAVE);
                    }


                    rbListObjectTypes.SelectedIndex = 0;

                    bool hasUpdateRight = _db.hasUIDAuthorisation(DBData.AUTHORISATION.UPDATE, FromUID, DBData.APPLICATION_RIGHT.COMMON, true, true);
                    cbPersonal.Checked = !hasUpdateRight;
                    cbPersonal.Enabled = hasUpdateRight;
                    
                    // structures...
                    string titleColName = _db.langAttrName("UID_STRUCTURE", "TITLE");
                    DataTable structures = _db.getDataTable("select ID, " + titleColName + " from UID_STRUCTURE where TYP=0 and (OWNER_PERSON_ID is null or OWNER_PERSON_ID=" + _db.userId + ")");
                    if (structures.Rows.Count > 0){
                        ddStructure.Items.Add(new ListItem(" ", ""));
                        foreach(DataRow row in structures.Rows){
                            string title = DBColumn.GetValid(row[titleColName], "");
                            long id = DBColumn.GetValid(row["ID"], -1L);
                            ddStructure.Items.Add(new ListItem(title, id.ToString()));
                        }
                    }
                    else{
                        structureRow.Visible = false;
                    }

                    // types...
                    string[] types = _mapper.getEnum("uidAssignment", "types", true);
                    for (int i=0; i<types.Length; i++){
                        ddTyp.Items.Add(new ListItem(types[i], i.ToString()));
                    }
                }
                catch (Exception ex) {
                    DoOnException(ex);
                }
                finally {
                    _db.disconnect();
                }
            }
        }

        private void addObjectType(string name, string objType){
            rbListObjectTypes.Items.Add(new ListItem(name, objType));
        }

        private void mapControls () {
            next.Click += new System.EventHandler(next_Click);
        }

        private void next_Click(object sender, System.EventArgs e) {
            _db.connect();
            try {
                int typ = DBData.ASSIGNMENT.UNDEFINED;
                if (ddTyp != null && ddTyp.SelectedItem != null){
                    typ = ch.psoft.Util.Validate.GetValid(ddTyp.SelectedItem.Value, DBData.ASSIGNMENT.UNDEFINED);
                }
                long ownerPersonID = cbPersonal.Checked? _db.userId : -1L;
                long structureID = -1L;
                if (ddStructure != null && ddStructure.SelectedItem != null){
                    structureID = ch.psoft.Util.Validate.GetValid(ddStructure.SelectedItem.Value, -1L);
                }
                string searchURL = "";
                string assignURL = psoft.Common.AddUIDAssignments.GetURL("fromUID",FromUID, "searchResultID","%SearchResultID", "nextURL",NextURL, "typ",typ, "ownerPersonID",ownerPersonID, "structureID",structureID,"fck",fromFCK);
                if (rbListObjectTypes.SelectedItem != null){
                    switch(rbListObjectTypes.SelectedItem.Value){
                        case "person":
                            searchURL = psoft.Person.SearchFrame.GetURL("backURL",assignURL);
                            break;

                        case "contact":
                            searchURL = psoft.Contact.ContactSearch.GetURL("nextURL",assignURL);
                            break;

                        case "tasklist":
                            searchURL = psoft.Tasklist.Search.GetURL("base","tasklist", "nextURL",assignURL);
                            break;

                        case "measure":
                            searchURL = psoft.Tasklist.Search.GetURL("base","measure", "nextURL",assignURL);
                            break;

                        case "project":
                            searchURL = psoft.Project.ProjectSearch.GetURL("nextURL",assignURL);
                            break;

                        case "knowledge":
                            searchURL = psoft.Knowledge.Search.GetURL("nextURL",assignURL);
                            break;

                        case "document":
                            searchURL = psoft.Document.DocumentSearch.GetURL("nextURL",assignURL);
                            break;

						case "clipboard":
							searchURL = psoft.Document.ClipboardSearch.GetURL("nextURL",assignURL);
							break;

                        case VALUE_MATRIX:
                            searchURL = psoft.Morph.MatrixSearch.GetURL("type","master","nextURL",assignURL);
                            break;
							
                        case VALUE_SLAVE:
                            searchURL = psoft.Morph.MatrixSearch.GetURL("type","slave","nextURL",assignURL);
                            break;
                    }
                }
                if (searchURL != ""){
                    Response.Redirect(searchURL, false);
                }
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
            mapControls();
        }
		
        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {

        }
		#endregion
    }
}

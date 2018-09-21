using System;
using System.Data;

namespace ch.appl.psoft.Common
{
    using ch.appl.psoft.Common.Controls;
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.Util;
    using db;
    using System.Text;

    public partial class AddUIDAssignments : PsoftDetailPage {
        public const string PAGE_URL = "/Common/AddUIDAssignments.aspx";

        static AddUIDAssignments(){
            SetPageParams(PAGE_URL, "fromUID", "searchResultID", "nextURL", "typ", "ownerPersonID", "structureID","fck");
        }

        public static string GetURL(params object[] queryParams){
            return CreateURL(PAGE_URL, queryParams);
        }

        public AddUIDAssignments() : base(){
            PageURL = PAGE_URL;
        }

        private long _searchResultID = -1L;
        private long _fromUID = -1L;
        private string _nextURL = "";
        private int _typ = DBData.ASSIGNMENT.UNDEFINED;
        private long _ownerPersonID = -1L;
        private long _structureID = -1L;
        private int fromFCKEditor = 0;
        protected string bla = "null";

        protected override void Initialize() {
            base.Initialize();

            _searchResultID = GetQueryValue("searchResultID", -1L);
            _fromUID = GetQueryValue("fromUID", -1L);
            _nextURL = GetQueryValue("nextURL", "");
            _typ = GetQueryValue("typ", DBData.ASSIGNMENT.UNDEFINED);
            _ownerPersonID = GetQueryValue("ownerPersonID", -1L);
            _structureID = GetQueryValue("structureID", -1L);
            this.fromFCKEditor = GetQueryValue("fck", 0);
        }

        protected override void  AppendBodyOnLoad(StringBuilder builder)
        {
            base.AppendBodyOnLoad(builder);

            if (_searchResultID > 0)
            {
                DBData db = DBData.getDBData(Session);
                db.connect();
                try
                {
                    DataTable table = db.getDataTable("select TABLENAME, ROW_ID from SEARCHRESULT where ID=" + _searchResultID);
                    string tablename = table.Rows[0]["TABLENAME"].ToString();
                    long rowID = DBColumn.GetValid(table.Rows[0]["ROW_ID"], -1L);
                    long toUID = db.ID2UID(rowID, tablename);
                    string nicename = db.UID2NiceName(toUID);
                    string url = psoft.Goto.GetURL("UID",toUID);
                    if (fromFCKEditor == 1)
                    {
                        string target = tablename == "DOCUMENT" ? "target='_blank'" : "";
                        string link = "<a href='" + url + "' " + target + " >" + nicename + "</a>";
                        builder.Append("var FCK = window.opener.FCK; FCK.Focus(); FCK.InsertHtml(\"" + link + "\"); window.close();");
                    }
                    else
                    {
                        builder.Append("window.opener.SetUrl('" + url + "'); window.close();");
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
       
         protected void Page_Load(object sender, System.EventArgs e) {
            DBData db = DBData.getDBData(Session);

            // Setting main page layout
            PageLayoutControl = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_pl");

            // Setting content layout of page layout
            DGLContentLayout dglControl = (DGLContentLayout) LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            PageLayoutControl.ContentLayoutControl = dglControl;
            
            db.connect();
            try {
                if (_fromUID > 0L){
                    if (_searchResultID > 0L){
                    

                        if (_nextURL != "")
                        {
                            Response.Redirect(_nextURL, false);
                        }
                        else
                        {
                            if (fromFCKEditor == 0 )
                            {
                                // let's add the UID-assignments...
                                db.beginTransaction();
                                DataTable table = db.getDataTable("select TABLENAME, ROW_ID from SEARCHRESULT where ID=" + _searchResultID);
                                foreach (DataRow row in table.Rows)
                                {
                                    string tablename = row["TABLENAME"].ToString();
                                    long rowID = DBColumn.GetValid(row["ROW_ID"], -1L);
                                    long toUID = db.ID2UID(rowID, tablename);
                                    WikiBoxCtrl.SET_TO_UID(Session, toUID);
                                    db.addUIDAssignment(_fromUID, toUID, _typ, _ownerPersonID, _structureID);
                                }
                                db.commit();
                                BreadcrumbVisible = false;

                                Response.Redirect(psoft.Goto.GetURL("UID", _fromUID), false);
                            }
                        }
                    }
                    else{
                        (PageLayoutControl as PsoftPageLayout).PageTitle = BreadcrumbCaption = _mapper.get("uidAssignment", "newAssignment");
                        (PageLayoutControl as PsoftPageLayout).PageTitle += " - " + db.UID2NiceName(_fromUID, _mapper);

                        AddUIDAssignmentsCtrl ctrl = (AddUIDAssignmentsCtrl) LoadPSOFTControl(AddUIDAssignmentsCtrl.Path, "_ctrl");
                        ctrl.NextURL = _nextURL;
                        ctrl.FromUID = _fromUID;
                        ctrl.FromFCK = fromFCKEditor;

                        SetPageLayoutContentControl(DGLContentLayout.DETAIL, ctrl);		
                    }
                }
            }
            catch (Exception ex) {
                ShowError(ex.Message);
                Logger.Log(ex,Logger.ERROR);
            }
            finally {
                db.disconnect();
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

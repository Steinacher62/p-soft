using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Lohn.Controls;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Lohn
{
    /// <summary>
    /// Page für Eingaben und Übersichten, für beide DLA-Komponenten 'salaire' und 'prime'
    /// </summary>
    public partial class SalaryAdjustment : PsoftContentPage {
        private const string PAGE_URL = "/Lohn/SalaryAdjustment.aspx";
        private PersonDetailControl _detail = null;

        static SalaryAdjustment() {
            SetPageParams(
                PAGE_URL, 
                "orgId", 
                "salaryComponent",
                "context",
                "lohnId", 
                "readOnly", 
                "orderColumn", 
                "orderDir",
                "budgettypId"
                );
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public SalaryAdjustment() : base() {
            PageURL = PAGE_URL;
        }

        #region Protected overrided methods from parent class
        protected override void AppendBodyOnLoad(StringBuilder bodyOnLoad) {
            base.AppendBodyOnLoad (bodyOnLoad);
            bodyOnLoad.Append("personControlOnLoad();");
        }
		#endregion

        protected void Page_Load(object sender, System.EventArgs e) {
            long lohnId = GetQueryValue("lohnId", (long)-1);
            long orgentityId = GetQueryValue("orgId", (long)-1);
            string salaryComponent = GetQueryValue("salaryComponent", "");
            string kontext = GetQueryValue("context", "adjustment");
            long budgettypId = GetQueryValue("budgettypId", 0L);

            this.BreadcrumbCaption = _mapper.get(LohnModule.KundenModuleName, salaryComponent + kontext);
            this.BreadcrumbName = "lohnSalaryAdjustment" +salaryComponent + kontext;

            // Setting main page layout
            PsoftPageLayout PsoftPageLayout
                = (PsoftPageLayout)this.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            PageLayoutControl = PsoftPageLayout;
            
            PsoftPageLayout.PageTitle = _mapper.get(LohnModule.KundenModuleName, salaryComponent + kontext);
            DBData db = DBData.getDBData(Session);
            db.connect();

            try {
                string titleAttribute = db.langAttrName("ORGENTITY", "TITLE");
                string oeTitle = DBColumn.GetValid(
                    db.lookup(titleAttribute, "ORGENTITY","ID = " + orgentityId),
                    ""
                    );

                if (oeTitle != "") {
                    PsoftPageLayout.PageTitle += " - " + oeTitle;
                }
            }
            finally {
                db.disconnect();
            }

            // Setting content layout of page layout
            DGLContentLayout contentControl
                = (DGLContentLayout)this.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            contentControl.DetailHeight = Unit.Percentage(75);
            PageLayoutControl.ContentLayoutControl = contentControl;

            // Setting parameters
            PersonControl list = (PersonControl)this.LoadPSOFTControl(PersonControl.Path, "_list");
            list.HighlightRecordID = lohnId;
            list.OrgentityId = orgentityId;
            list.SalaryComponent = salaryComponent;
            list.Kontext = kontext;
            list.OrderColumn = GetQueryValue("orderColumn", "");
            list.OrderDir = GetQueryValue("orderDir", "");
            list.BudgettypId = budgettypId;

            // Setting content layout user controls
            SetPageLayoutContentControl(DGLContentLayout.DETAIL, list);	

            if (kontext == "adjustment") {
                list.OnLoadList += new LoadListHandler(loadPersonList);
                string module = LohnModule.KundenModuleName;
                // Setting parameters
                BudgetControl budgetList = null;
                try {
                    budgetList = (BudgetControl)this.LoadPSOFTControl(BudgetControl.getPath(module), "_budgetList");
                }
                catch {
                    budgetList = (BudgetControl)this.LoadPSOFTControl(BudgetControl.Path, "_budgetList");
                }
                budgetList.SalaryComponent = salaryComponent;
                budgetList.OrgentityId = orgentityId;
                budgetList.BudgettypId = budgettypId;

                // Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.GROUP, budgetList);	

                // Setting parameters
                try {
                    _detail = (PersonDetailControl)this.LoadPSOFTControl(PersonDetailControl.getPath(module),"_detail");
                }
                catch {
                    _detail = (PersonDetailControl)this.LoadPSOFTControl(PersonDetailControl.Path,"_detail");
                }
                _detail.OrgentityId = orgentityId;
                _detail.LohnId = lohnId;
                _detail.SalaryComponent = salaryComponent;
                _detail.ReadOnly = bool.Parse(GetQueryValue("readOnly", "false"));
                _detail.Kontext = ""; // nicht History
                _detail.Visible = lohnId > 0;
                _detail.BudgettypId = budgettypId;

                // Setting content layout user controls
                SetPageLayoutContentControl(DGLContentLayout.LINKS, _detail);	
            }
        }

        private void loadPersonList(object sender, ListEventArgs e) {
            if (e.EndLoad) {
                if (e.List.Rows.Count == 2) {
                    string url = Request.RawUrl;
                    long id = ListBuilder.IdColumnValue(e.List.Rows[1]);

                    if (id > 0 &&  _detail.LohnId != id) {
                        int idx = url.IndexOf("&lohnId=");
                        if (idx > 0) {
                            int idx2 = url.IndexOf("&",idx+8);
                            url = url.Substring(0,idx)+(idx2 > 0 ? url.Substring(idx2,url.Length-idx2) : "");
                        }
                        url += "&lohnId="+id;
                        Response.Redirect(url,false);
                    }
                }
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
            this.ID = "SalaryAdjustment";
        }
		#endregion
    }
}

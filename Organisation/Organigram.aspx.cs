using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Organisation.Controls;
using ch.psoft.Util;
using System;


namespace ch.appl.psoft.Organisation
{
    /// <summary>
    /// Summary description for Organigram.
    /// </summary>
    public partial class Organigram : PsoftContentPage
	{
        private const string PAGE_URL = "/Organisation/Organigram.aspx";

        static Organigram() {
            SetPageParams(PAGE_URL, "id");
        }

        public static string GetURL(params object[] queryParams) {
            return CreateURL(PAGE_URL, queryParams);
        }

        public Organigram() : base() {
            PageURL = PAGE_URL;
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Setting main page layout
            PsoftPageLayout pageLayout = (PsoftPageLayout) LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
			PageLayoutControl = pageLayout;

			// Setting content layout of page layout
			PageLayoutControl.ContentLayoutControl = (SimpleContentLayout) LoadPSOFTControl(SimpleContentLayout.Path, "_sC");

			// Load chart control
			OrganigrammGraph _graph = (OrganigrammGraph) LoadPSOFTControl(OrganigrammGraph.Path, "_graph");
			_graph.ChartId = GetQueryValue("id", -1);
			_graph.Execute();

			// Setting breadcrumb menu for chart title
			BreadcrumbCaption = _graph.ChartTitle;
            BreadcrumbName = _graph.ChartTitle;
            pageLayout.PageTitle = _graph.ChartTitle;

			SetPageLayoutContentControl(SimpleContentLayout.CONTENT, _graph);
            if (_graph.ChartId > 0){
                pageLayout.ButtonPrintVisible = true;
                pageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('PrintOrganigram.aspx?id="+_graph.ChartId+"','_blank')");
                DBData db = DBData.getDBData(Session);
                db.connect();
                try{
                    string printableFile = ch.psoft.Util.Validate.GetValid(db.lookup("PRINTABLE_FILE", "CHART", "ID=" + _graph.ChartId, false), "");
                    if (printableFile != ""){
                        pageLayout.ButtonPrintAttributes.Add("onClick", "javascript: window.open('printables/" + printableFile + "','_blank')");
                    }
                }

                catch(Exception ex){
                    Logger.Log(ex,Logger.ERROR);
                }
                finally {
                    db.disconnect();
                }

                //show visio button if export enabled
                if(Global.Config.enableVisio)
                {
                    pageLayout.ButtonVisioVisible = true;
                    pageLayout.ButtonVisioAttributes.Add("onClick", "javascript: window.open('VisioExport/VisioExport.aspx?id=" + _graph.ChartId + "','_blank')");
                }
            }
        }

        protected override void DoSetPageLayoutControl(ch.appl.psoft.LayoutControls.PageLayoutControl control)
        {
            //LiteralControl lc = new LiteralControl("
            
            base.DoSetPageLayoutControl(control);
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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}

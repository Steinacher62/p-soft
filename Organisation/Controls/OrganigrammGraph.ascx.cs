namespace ch.appl.psoft.Organisation.Controls
{
    using ch.appl.psoft.LayoutControls;
    using db;
    using System;
    using System.Text;

    /// <summary>
    ///		Summary description for OrganigrammGraph.
    /// </summary>
    public partial  class OrganigrammGraph : PSOFTUserControl
	{
		public const string PARAM_CHART_ID = "PARAM_CHART_ID";
		public const string PARAM_TARGET_FRAME = "PARAM_TARGET_FRAME";
		public const string PARAM_NAVIGATE_URL = "PARAM_NAVIGATE_URL";

		private static int MapId = 0;
		private Chart _chart = null;

		protected string _organigramRoot = Global.Config.getStringProperty("System", "OrganigramRoot", 0);


		public static string Path
		{
			get {return Global.Config.baseURL + "/Organisation/Controls/OrganigrammGraph.ascx";}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
            //Execute();
        }

		public long ChartId
		{
			get {return GetLong(PARAM_CHART_ID);}
			set {SetParam(PARAM_CHART_ID, value);}
		}

		public string TargetFrame
		{
			get {return GetString(PARAM_TARGET_FRAME);}
			set {SetParam(PARAM_TARGET_FRAME, value);}
		}

		public string NavigateUrl
		{
			get {return GetString(PARAM_NAVIGATE_URL);}
			set {SetParam(PARAM_NAVIGATE_URL, value);}
		}

		public string ChartTitle
		{
			get {return (_chart != null) ? _chart.Title + " [" +  _chart.Organisation.Title + "]" : "No Chart";}
		}

		protected override void DoExecute()
		{
			base.DoExecute();
			try
			{
				if (!this.IsPostBack) 
				{
					MapId++;
				}
				CreateChartNode();
			}
			catch (Exception ex) 
			{
				DoOnException(ex);
			}
		}

		private void CreateChartNode()
		{
			DBData db = DBData.getDBData(Session);
			try 
			{
				db.connect();
				_chart = Chart.BuildChart(db, ChartId, Global.Config.organisationImageDirectory);
				if (_chart != null)
				{
					_chart.NavigateUrl = NavigateUrl;
					_chart.TargetFrame = TargetFrame;
					Label1.Text = "";
					Label1.Visible = false;
					//Label1.Text = "Chart title: " + _chart.Title + ", All nodes count:  " + _chart.Count +  ", Organisation: " + _chart.Organisation.Title + ", organisation nodes count: " + _chart.Organisation.Count;
				}
				else
				{
					navigationImage.Visible = false;
					Label1.Text = "There is no chart with ID = " + ChartId;
					return;
				}

				System.Drawing.Image _img = _chart.GetImageGraph();
                this.Session["OrganisationImage"] = _img;
                this.Session["OrganisationImageWidth"] = _img.Width;
                this.Session["OrganisationImageHeight"] = _img.Height;

                //temp: export to Visio / 24.08.10 / mkr
                //_chart.ExportVisio("C:\\Temp\\visio_export.vsd");

				navigationImage.Width = _img.Width;
				navigationImage.Height = _img.Height;
				navigationImage.Src = Global.Config.baseURL + "/Organisation/NavigationTree.aspx?id="+ChartId+"&map="+MapId; // hack for correct image load
			}
			catch (Exception e) 
			{
				DoOnException(e);
				Label1.Text = e.Message;
			}
			finally 
			{
				db.disconnect();
			}
		}

		public string BuildImageMap()
		{
			if (_chart == null)
				return "";

			StringBuilder _map = new StringBuilder(1024);
			_map.Append("<map name=\"TreeMap\">\n");
			_chart.AppendImageMapInfo(_map);
			_map.Append("</map>");
			return _map.ToString();
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
	}
}

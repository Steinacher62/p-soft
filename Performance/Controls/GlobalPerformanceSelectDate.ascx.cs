namespace ch.appl.psoft.Performance.Controls
{
    using ch.appl.psoft.LayoutControls;
    using ch.psoft.db;
    using ch.psoft.Util;
    using db;
    using System;
    using System.Data;

    /// <summary>
    ///		Summary description for GlobalPerformanceSelectDate.
    /// </summary>
    public partial class GlobalPerformanceSelectDate : PSOFTInputViewUserControl
	{
        public const string PARAM_CUTOFFDAY = "PARAM_CUTOFFDAY";
        public const string PARAM_CONTEXT = "PARAM_CONTEXT";
        public const string PARAM_EMPLOYMENT_ID = "PARAM_EMPLOYMENT_ID";

        private const string _TABLE = "PERFORMANCERATING";
        private const string _VIEW = "DATESELECTORVIEW";
        private const string _ATTR = "RATING_DATE";
        protected DataTable _table;

		public static string Path
		{
			get {return Global.Config.baseURL + "/Performance/Controls/GlobalPerformanceSelectDate.ascx";}
		}

		#region Properities
        public string CutoffDay 
        {
            get {return GetString(PARAM_CUTOFFDAY);}
            set {SetParam(PARAM_CUTOFFDAY, value);}
        }
        public string ContextSearch
        {
            get {return GetString(PARAM_CONTEXT);}
            set {SetParam(PARAM_CONTEXT, value);}
        }
        public long EmploymentID 
        {
            get {return GetLong(PARAM_EMPLOYMENT_ID);}
            set {SetParam(PARAM_EMPLOYMENT_ID, value);}
        }
 		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DBData db = DBData.getDBData(Session);
            string sql = "select * from " + _TABLE;

            sql = "select ID, " + _ATTR + " from " + _TABLE + " where id = -1";

            
			try
			{
				if (!IsPostBack)
				{
					apply.Text = _mapper.get("apply");
                    CBShowOpposite.Visible = false;
                }
				db.connect();

				_table = db.getDataTableExt(sql, _TABLE);
                _table.Columns[_ATTR].ExtendedProperties["Visibility"] = DBColumn.Visibility.ADD;
                               

                switch (ContextSearch) 
                {
                    default:
                        break;
                }
             
				base.CheckOrder = true;
                base.View = _VIEW;
				base.LoadInput(db,_table,searchTab);
                setInputValue(_table, searchTab, "RATING_DATE", DateTime.Now.ToString(db.dbColumn.UserCulture.DateTimeFormat.ShortDatePattern));
            }
			catch (Exception ex)
			{
                DoOnException(ex);
            }
			finally
			{
				db.disconnect();
			}
		}


		private void mapControls ()
		{
			apply.Click += new System.EventHandler(apply_Click);
		}

		private void apply_Click(object sender, System.EventArgs e)
		{
            if (checkInputValue(_table, searchTab)) {
                string src = base.getSql(_table, searchTab);
                string match1 = "insert into " + _TABLE + " (" + _ATTR + ") values('";
                string match2 = "')";


                switch (ContextSearch) {
                    default:
                        src = src.Replace(match1,"");
                        src = src.Replace(match2,"");
                        CutoffDay = GetValid(src,DateTime.Now);
                        break;
                }

                Response.Redirect(Global.Config.baseURL + "/Performance/PrintGlobalPerformance.aspx?employmentID=" + EmploymentID + "&cutoffDay=" + CutoffDay);
            }
		}

        public static string GetValid(string str, DateTime dflt) 
        {
            if (str == null || str.Equals("")) return "'" + dflt.ToString("yyyy'-'MM'-'dd") + "'";
            else 
            {
                try 
                {
                    // str deliverte by input is 'en-us' -> ? 
                    return "'" + DateTime.Parse(str,SQLColumn.DBCulture).ToString("yyyy'-'MM'-'dd") + "'";
                }
                catch(Exception ex)
                {
                    Logger.Log(ex, Logger.ERROR);
                    return "'" + dflt.ToString("yyyy'-'MM'-'dd") + "'";
                }
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

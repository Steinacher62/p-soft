namespace ch.appl.psoft.FBW.Controls
{
	using ch.appl.psoft.LayoutControls;
	using db;
	using Interface;
	using System;
	using System.Data;
	using System.Web.UI.WebControls;

	/// <summary>
	///		Summary description for FunctionRatingCtrl.
	/// </summary>
	public partial class FunctionRatingCtrl : PSOFTMapperUserControl {



		protected Config _config = null;
		protected DBData _db = null;
		protected long _funktionID = -1L;
		protected long _fbwID = -1L;


		public static string Path {
			get {return Global.Config.baseURL + "/FBW/Controls/FunctionRatingCtrl.ascx";}
		}

		#region Properities
		public long FunktionID {
			get {return _funktionID;}
			set {_funktionID =  value;}
		}
		public long FbwID 
		{
			get {return _fbwID;}
			set {_fbwID =  value;}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e) {
			if (!IsPostBack) {
				
			}
			Execute();
		}

		protected override void DoExecute() {
			base.DoExecute();
			loadList();
		}

		private void loadList() {
			_config = Global.Config;          
			_db = DBData.getDBData(Session);
			contentList.Rows.Clear();
			
			try {
				_db.connect();

				if (!IsPostBack) {
					contentListTitle.Text = _mapper.get(FBWModule.LANG_SCOPE_FBW, FBWModule.LANG_MNEMO_CT_ANFORDERUNGEN);               

				}
				if (FunktionID > 0) {

					string funktionswert = ch.psoft.Util.Validate.GetValid(_db.lookup("FUNKTIONSWERT","FUNKTIONSBEWERTUNG","ID="+_fbwID,false),"");
					contentListTitle.Text += " - " + _mapper.get("FUNKTIONSBEWERTUNG","FUNKTIONSWERT") + " " + funktionswert;

					DataTable criteriasTable = _db.getDataTable("select ID, BEZEICHNUNG from FBW_KRITERIUM order by ORDNUMBER asc");
					
					bool isFirstCriteria = true;
					foreach (DataRow critRow in criteriasTable.Rows) {
						long criteriaID = ch.psoft.Util.Validate.GetValid(critRow["ID"].ToString(), -1L);                          
							
						string sql = "";
						DataTable table = null;
						sql = "select * from ANFORDERUNGDETAILV where FUNKTIONSBEWERTUNG_ID=" + _fbwID + " and KRITERIUM_ID=" + criteriaID+ " ORDER BY FBW_ARGUMENT_KATALOG_ID";
						table = _db.getDataTable(sql, "ANFORDERUNGDETAILV");
						addCriteria(table, ref isFirstCriteria, critRow, criteriaID);
					}
				}
			}
			catch (Exception ex) {
				DoOnException(ex);
			}
			finally {
				_db.disconnect();
			}

		}

		private void addCriteria(DataTable table, ref bool isFirstCriteria, DataRow grpRow, long criteriaID) {
			TableRow r = null;
			TableCell c = null;
			bool isFirst = true;
			foreach (DataRow row in table.Rows) {
				if (isFirst) {
					isFirst = false;
					if (isFirstCriteria) {
						isFirstCriteria = false;
					}
					else {
						r = new TableRow();
						c = new TableCell();
						r.Cells.Add(c);
						c.ColumnSpan = 3;
						c.Height = 10;
						contentList.Rows.Add(r);
					}
					r = new TableRow();
					contentList.Rows.Add(r);
					r.CssClass = "Detail_mainTitle";
					r.BackColor = System.Drawing.Color.LightGray;
					c = new TableCell();
					r.Cells.Add(c);
					c.ColumnSpan = 3;
					if (grpRow != null) {
						c.Text = ch.psoft.Util.Validate.GetValid(grpRow[_db.langAttrName(grpRow.Table.TableName, "BEZEICHNUNG")].ToString(), "");
					}
					DataTable table2 = _db.getDataTableExt("select * from FBWKOMMENTARV where FUNKTIONSBEWERTUNG_ID=" + _fbwID + " and FBW_KRITERIUM_ID=" + criteriaID, "FBWKOMMENTARV");
					addBewertung(table2);
				}
				addAnforderung(row);
			}
		}

 
		private void addBewertung(DataTable table)
		{
			foreach (DataRow row in table.Rows) 
			{
				TableRow r = new TableRow();
				TableCell c = new TableCell();
				r.Cells.Add(c);
				c.ColumnSpan = 3;
				c.Height = 5;
				contentList.Rows.Add(r);

				string kommentar = ch.psoft.Util.Validate.GetValid(row["TEXT"].ToString(), "");
				if (kommentar != "")
				{
					r = new TableRow();
					contentList.Rows.Add(r);
					r.VerticalAlign = VerticalAlign.Top;
					c = new TableCell();
					r.Cells.Add(c);
					c.CssClass = "Detail_special";
					c.Width = Unit.Percentage(5);
					c.Text = _mapper.get("FBWKOMMENTARV","TEXT");;
					c = new TableCell();
					r.Cells.Add(c);
					c.CssClass = "Detail";
					c.Text = kommentar;
				}

				r = new TableRow();
				contentList.Rows.Add(r);
				r.VerticalAlign = VerticalAlign.Top;
				c = new TableCell();
				r.Cells.Add(c);
				c.CssClass = "Detail_special";
				c.Width = Unit.Percentage(5);
				c.Text = _mapper.get("FBWKOMMENTARV","PUNKTEZAHL");
				c = new TableCell();
				r.Cells.Add(c);
				c.CssClass = "Detail";
				c.Text = ch.psoft.Util.Validate.GetValid(row["PUNKTEZAHL"].ToString(), "");

				//Einstufung entfernt MSr 2013.05.23

				//r = new TableRow();
				//contentList.Rows.Add(r);
				//r.VerticalAlign = VerticalAlign.Top;
				//c = new TableCell();
				//r.Cells.Add(c);
				//c.CssClass = "Detail_special";
				//c.Width = Unit.Percentage(5);
				//c.Text = _mapper.get("FBWKOMMENTARV","BEZEICHNUNG");;
				//c = new TableCell();
				//r.Cells.Add(c);
				//c.CssClass = "Detail";
				//c.Text = ch.psoft.Util.Validate.GetValid(row["BEZEICHNUNG"].ToString(), "");
  
			}
		}

		private void addAnforderung(DataRow row) {           

			TableRow r = new TableRow();
			TableCell c = null;

			c = new TableCell();
			r.Cells.Add(c);
			c.ColumnSpan = 3;
			c.Height = 5;
			contentList.Rows.Add(r);

			r = new TableRow();
			contentList.Rows.Add(r);
			r.VerticalAlign = VerticalAlign.Top;
			c = new TableCell();
			c.ColumnSpan = 3;
			r.Cells.Add(c);
			c.CssClass = "Detail_special";
			c.Text = FBWModule.getCatalogPath(_db, ch.psoft.Util.Validate.GetValid(row["FBW_ARGUMENT_KATALOG_ID"].ToString(), -1L), false) + row[_db.langAttrName(row.Table.TableName, "BEZEICHNUNG")].ToString();           
			string anpassung = ch.psoft.Util.Validate.GetValid(row["ELEMENT_BEZEICHNUNG"].ToString(), "");
			string punktezahl = ch.psoft.Util.Validate.GetValid(row["STUFE_PUNKTEZAHL"].ToString(), "");
			if (anpassung != "")
			{
				c.Text += " (" + anpassung + ")";
			}
			if (punktezahl != "")
			{
				c.Text += " [" + punktezahl + "]";
			}
			//Description
			string beschreibung = ch.psoft.Util.Validate.GetValid(row[_db.langAttrName(row.Table.TableName, "BESCHREIBUNG")].ToString(), "");
			if (beschreibung != "")
			{
				r = new TableRow();
				contentList.Rows.Add(r);
				r.VerticalAlign = VerticalAlign.Top;
				c = new TableCell();
				r.Cells.Add(c);
				c.ColumnSpan = 3;
				c.CssClass = "Detail";
				c.Text = beschreibung;
			}

		}

		private void MapButtonMethods() {
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e) {
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			MapButtonMethods();
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

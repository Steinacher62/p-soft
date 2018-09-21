using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace ch.appl.psoft.Lohn.Controls
{
    /// <summary>
    ///	Import- und Exportmenü
    /// </summary>
    public partial class ImportExportControl : PSOFTInputViewUserControl
	{

		public static string Path
		{
			get {return Global.Config.baseURL + "/Lohn/Controls/ImportExportControl.ascx";}
		}

        #region Properities
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!(Request["__EVENTARGUMENT"] == null) && !Request["__EVENTARGUMENT"].Equals("") && !(Request["__EVENTARGUMENT"].Equals("noFile")))
            {
                GetExcelSheets(Request["__EVENTARGUMENT"], ".xlsx", "Yes");
            }
            if (!(Request["__EVENTARGUMENT"] == null))
            {
                if ((Request["__EVENTARGUMENT"].Equals("noFile")))
                {
                    ImportExportWindowManager.RadAlert("Die Daten konnten nich importiert werden da keine Datei selektiert ist!", 300, 170, "Fehler", "");
                }
            }
            if (!IsPostBack)
            {
                Import.Text = _mapper.get("lohn", "import");

                string[] paths = new string[] { "~" + Global.Config.getModuleParam("report", "ExLohnsimURl", "/").ToString() };

                FileExplorer.Configuration.ViewPaths = paths;
                FileExplorer.Configuration.UploadPaths = paths;
                FileExplorer.Configuration.DeletePaths = paths;
            }
            try {

            }
            catch (Exception ex) {
                DoOnException(ex);
            }
        }

        //private void mapControls ()
        //{
        //    this.Import.Click += new System.EventHandler(this.Import_Click);
        //}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			//mapControls();
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
        }
		#endregion



        private void GetExcelSheets(string FileName, string Extension, string isHDR)
        {

            string conStr = "";
            DataTable ExLohnTableSchema;
            DataTable ExLohnTable = new DataTable();
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;

                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                    break;
            }

            //Get the Sheets in Excel WorkBoo
            try
            {
                conStr = String.Format(conStr, Global.Config.getModuleParam("report", "ExLohnsimPath", "") + FileName, isHDR);
                OleDbConnection connExcel = new OleDbConnection(conStr);
                OleDbCommand cmdExcel = new OleDbCommand();
                OleDbDataAdapter oda = new OleDbDataAdapter();
                cmdExcel.Connection = connExcel;
                connExcel.Open();
                ExLohnTableSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string SheetName = ExLohnTableSchema.Rows[0]["TABLE_NAME"].ToString();
                cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
                oda.SelectCommand = cmdExcel;

                oda.Fill(ExLohnTable);
                connExcel.Close();
            }

            catch(Exception e)
            {
                ImportExportWindowManager.RadAlert("Importdaten konnten nicht geöffnet werden!", 300, 170, "Fehler", "");
            }

            

            DBData db = DBData.getDBData(Session);
            db.connect();
            try
            {
                foreach (DataRow row in ExLohnTable.Rows)
                {
                    if (!row["SalMarktoB"].ToString().Equals(""))
                    {
                        db.execute("update lohn set REFLOHN =" + row["SalMarktoB"] + " where EXTERNAL_REf = " + row["Referenz"]);
                    }

                }
                ImportExportWindowManager.RadAlert("Die Daten wurden erfolgreich importiert.", 300, 170, "Info", "");
            }
                

            catch (Exception e)
            {
                ImportExportWindowManager.RadAlert("Die Importierten Daten konnten nicht übernommen werden! Bitte überprüfen Sie das Format der importierten Datei.", 400, 250, "Fehler", "");
            }

            

        }

    }
}

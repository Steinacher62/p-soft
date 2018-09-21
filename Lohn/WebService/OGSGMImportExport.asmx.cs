using System.ComponentModel;
using System.Web.Services;

namespace ch.appl.psoft.Lohn.WebService
{
    /// <summary>
    /// Summary description for OGSGMImportExport.
    /// </summary>
    public class OGSGMImportExport : System.Web.Services.WebService
	{
        public OGSGMImportExport()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
        }

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

        [WebMethod]
        public string getDefaultDbName()
        {
            return LohnModule.DefaultDBName;
        }

        [WebMethod]
		public int loadAll(string moduleName, string dbName)
		{
            Transfer transfer = LohnModule.getNewTransfer(moduleName);
            return transfer.loadAll(dbName);
		}

        [WebMethod]
		public int loadAllHistory(string moduleName, string dbName, int anzahlLohnrunden)
		{
            Transfer transfer = LohnModule.getNewTransfer(moduleName);
            return transfer.loadAll(dbName, anzahlLohnrunden);
		}

        [WebMethod]
        public void storeAll(string moduleName, string dbName, string salaryComponente)
        {
            Transfer transfer = LohnModule.getNewTransfer(moduleName);
            transfer.storeAll(dbName, salaryComponente);
        }
    }
}

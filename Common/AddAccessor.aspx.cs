using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for AccessorSelect.
    /// </summary>
    public partial class AddAccessor : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.Label authorisationsTitle;

        protected DBData _db = null;
        protected string _tableName = "";
        protected long _rowID = -1;
        protected string _column = "";
        protected string _selection = "";
    
		protected void Page_Load(object sender, System.EventArgs e)
		{
            _tableName = ch.psoft.Util.Validate.GetValid(Request.QueryString["tableName"],"");
            _rowID = ch.psoft.Util.Validate.GetValid(Request.QueryString["rowID"],_rowID);
            _column = ch.psoft.Util.Validate.GetValid(Request.QueryString["column"],"");
            _selection = ch.psoft.Util.Validate.GetValid(Request.QueryString["selection"],"");
            long selectedID = -1L;

            if (!_selection.Equals(""))
            {
                _db = DBData.getDBData(Session);
                try
                {
                    string [] accessorIDs = _selection.Split(',');
                    _db.connect();

                    foreach (string strAccessorID in accessorIDs)
                    {
                        long accessorID = ch.psoft.Util.Validate.GetValid(strAccessorID,-1);
                        if (_tableName != "")
                        {
                            try
                            {
                                if (_rowID > 0)
                                    _db.grantRowAuthorisation(DBData.AUTHORISATION.READ, accessorID, _tableName, _rowID);
                                    if (_tableName.Equals("MATRIX"))
                                    {
                                        grantLocalKnowledge(_db, accessorID, _rowID);
                                    }
                                else if (_column != "")
                                    _db.grantColumnAuthorisation(DBData.AUTHORISATION.READ, accessorID, _tableName, _column);
                                else
                                    _db.grantTableAuthorisation(DBData.AUTHORISATION.READ, accessorID, _tableName);

                                if (selectedID <= 0)
                                    selectedID = accessorID;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log(ex,Logger.ERROR);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex,Logger.ERROR);
                }
                finally
                {
                    _db.disconnect();
                }
            }
            Response.Redirect("Authorisations.aspx?tableName=" + _tableName + "&rowID=" + _rowID + "&column=" + _column + "&selectedID=" + selectedID);
        }

        private void grantLocalKnowledge(DBData _db, long accessorID, long matrixID)
        {
            DataTable localKonowledge = _db.getDataTable("SELECT KNOWLEDGE.ID FROM MATRIX INNER JOIN DIMENSION ON MATRIX.ID = DIMENSION.MATRIX_ID INNER JOIN "
                                                        + "CHARACTERISTIC ON DIMENSION.ID = CHARACTERISTIC.DIMENSION_ID INNER JOIN "
                                                        + "KNOWLEDGE ON CHARACTERISTIC.KNOWLEDGE_ID = KNOWLEDGE.ID "
                                                        + "WHERE  MATRIX.ID = " + _rowID + " AND KNOWLEDGE.LOCAL = 1");
            foreach (DataRow row in localKonowledge.Rows)
            {
                _db.grantRowAuthorisation(DBData.AUTHORISATION.READ, accessorID, "KNOWLEDGE", Convert.ToInt32(row["ID"]));
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

using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.psoft.Util;
using System;
using System.Data;
using System.Text;

namespace ch.appl.psoft.Document.Controls
{
    /// <summary>
    /// Summary description for ShelfLocalCopy.
    /// </summary>
    public partial class ShelfLocalCopyCtrl : PSOFTMapperUserControl
	{
        public const string PARAM_FOLDER_ID = "PARAM_FOLDER_ID";

        protected string _copyScript;
        protected string _codeBase;
        protected StringBuilder _sb = new StringBuilder();
        protected DBData _db;
        protected System.Web.UI.WebControls.Label FileName;
        protected int _uniqueID = 0;
        protected string _loadingFinished;
        protected string _title;
        protected string _ftpDocumentServer;
        protected string _ftpDocumentSaveDirectory;

        public static string Path
        {
            get {return Global.Config.baseURL + "/Document/Controls/ShelfLocalCopyCtrl.ascx";}
        }

        #region Properities
        public int FolderID
        {
            get {return GetInt(PARAM_FOLDER_ID);}
            set {SetParam(PARAM_FOLDER_ID, value);}
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Execute();
        }

        protected override void DoExecute()
        {
            base.DoExecute ();
            _loadingFinished = PSOFTConvert.ToJavascript(_mapper.get("document", "gettingShelfFinished"));
            _ftpDocumentServer = Global.Config.ftpDocumentServer;
            _ftpDocumentSaveDirectory = Global.Config.ftpDocumentSaveDirectory;
            _db = DBData.getDBData(Session);
            _db.connect();

            if (!IsPostBack)
            {
                LoadingBar.ForeColor = Global.HighlightColor;
                LoadingLabel.Text = _mapper.get("document", "gettingShelf");
                cancelButton.Value = _mapper.get("cancel");
                FileNameLabel.Style.Add("word-wrap", "break-word");
            }

            try
            {
                DataTable table = _db.getDataTableExt("select * from FOLDERDOCUMENTV where ID=" + FolderID, "FOLDERDOCUMENTV");
                _title = PSOFTConvert.ToJavascript(getValidTitleFileName(table.Rows[0], FolderID.ToString()));

                appendSubFolders(FolderID, "");
            }
            catch (Exception ex)
            {
                DoOnException(ex);                
                _sb.Length = 0;
            }
            finally
            {
                _db.disconnect();
            }

            _copyScript = _sb.ToString();
        }

        private void appendLine(string line)
        {
            _sb.Append(line).Append("\r\n");
        }

        private static string getValidFileName(string fileName, string defaultValue)
        {
            string retValue = fileName;

            if (fileName != "")
            {
                retValue = retValue.Replace('\\', '_');
                retValue = retValue.Replace('/', '_');
                retValue = retValue.Replace(':', '_');
                retValue = retValue.Replace('*', '_');
                retValue = retValue.Replace('?', '_');
                retValue = retValue.Replace('"', '_');
                retValue = retValue.Replace('<', '_');
                retValue = retValue.Replace('>', '_');
                retValue = retValue.Replace('|', '_');
            }
            else
                retValue = defaultValue;

            return retValue;
        }

        private static string getValidTitleFileName(DataRow row, string defaultValue)
        {
            string retValue = defaultValue;

            if (row != null)
                retValue = getValidFileName(ch.psoft.Util.Validate.GetValid(row["TITLE"].ToString()), defaultValue);

            return retValue;
        }

        private string getUniqueFileName(string fileName, string previousFileName)
        {
            string retValue = fileName;

            if (fileName.ToLower() == previousFileName.ToLower())
            {
                int dotPos = fileName.LastIndexOf('.');
                if (dotPos >= 0)
                    retValue = fileName.Substring(0, dotPos) + "_" + ++_uniqueID + fileName.Substring(dotPos);
                else
                    retValue = fileName + "_" + ++_uniqueID;
            }
            else
                _uniqueID = 0;

            return retValue;
        }

        private void appendSubFolders(long folderID, string path)
        {
            DataTable table = _db.getDataTableExt("select * from FOLDERDOCUMENTV where FOLDER_ID=" + folderID + " order by TYPE desc, FILENAME asc, TITLE asc", "FOLDERDOCUMENTV");
            string prevTitle = "";

            foreach (DataRow row in table.Rows)
            {
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, row, true, true))
                    prevTitle = appendFolderDocument(row, prevTitle, path);
            }
        }

        private string appendFolderDocument(DataRow row, string previousTitle, string path)
        {
            string title;
            long ID = ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1);
            string type = row["TYPE"].ToString();;

            if (type == "FOLDER")
            {
                title = getValidTitleFileName(row, ID.ToString());
                string folderName = getUniqueFileName(title, previousTitle);
                path += folderName + "\\\\";
                appendLine("      contextArray[contextArray.length] = new FDContext('" + path + "', '', '', true);");
                appendSubFolders(ID, path);
            }
            else
            {
                title = getValidFileName(ch.psoft.Util.Validate.GetValid(row["FILENAME"].ToString()), ID.ToString());
                string fileName = getUniqueFileName(title, previousTitle);
                string xFileName = ch.psoft.Util.Validate.GetValid(row["XFILENAME"].ToString());
                appendLine("contextArray[contextArray.length] = new FDContext('" + path + "', '" + fileName + "', '" + xFileName + "', false);");
            }

            return title;
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

namespace ch.appl.psoft.Common
{
    using ch.psoft.Util;
    using Interface;
    using System;

    /// <summary>
    ///		Summary description for LockFile.
    /// </summary>
    public partial  class LockFile : System.Web.UI.UserControl
	{
        protected string _checkInConfirmMessage = "";
        protected string _codeBase = "";
        protected string _lfErrorGeneric = "";
        protected string _lfErrorDownloadFile = "";
        protected string _lfErrorFTPConnection = "";
        protected string _lfErrorFTPDirectory = "";
        protected string _lfErrorStartProcessNoHandle = "";
        protected string _lfErrorProcessAlreadyRunning = "";
        protected string _lfErrorStartProcess = "";
        protected string _lfErrorUploadFile = "";
        protected string _lfErrorCheckOutFailed = "";
        protected string _lfErrorCheckInFailed = "";
        protected string _lfErrorCheckOutUndoFailed = "";

		protected void Page_Load(object sender, System.EventArgs e)
		{
            LanguageMapper map = LanguageMapper.getLanguageMapper(Session);
           

            //_codeBase = Global.Config.baseURL + "/ocx/LockFile.ocx";

            _checkInConfirmMessage = PSOFTConvert.ToJavascript(map.get("document","checkInConfirmMessage"));
            _lfErrorGeneric = PSOFTConvert.ToJavascript(map.get("error","lfErrorGeneric"));
            _lfErrorDownloadFile = PSOFTConvert.ToJavascript(map.get("error","lfErrorDownloadFile"));
            _lfErrorFTPConnection = PSOFTConvert.ToJavascript(map.get("error","lfErrorFTPConnection"));
            _lfErrorFTPDirectory = PSOFTConvert.ToJavascript(map.get("error","lfErrorFTPDirectory"));
            _lfErrorStartProcessNoHandle = PSOFTConvert.ToJavascript(map.get("error","lfErrorStartProcessNoHandle"));
            _lfErrorProcessAlreadyRunning = PSOFTConvert.ToJavascript(map.get("error","lfErrorProcessAlreadyRunning"));
            _lfErrorStartProcess = PSOFTConvert.ToJavascript(map.get("error","lfErrorStartProcess"));
            _lfErrorUploadFile = PSOFTConvert.ToJavascript(map.get("error","lfErrorUploadFile"));
            _lfErrorCheckOutFailed = PSOFTConvert.ToJavascript(map.get("error","lfErrorCheckOutFailed"));
            _lfErrorCheckInFailed = PSOFTConvert.ToJavascript(map.get("error","lfErrorCheckInFailed"));
            _lfErrorCheckOutUndoFailed = PSOFTConvert.ToJavascript(map.get("error","lfErrorCheckOutUndoFailed"));
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

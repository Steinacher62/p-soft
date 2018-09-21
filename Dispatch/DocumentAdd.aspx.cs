using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Interface.DBObjects;
using ch.psoft.Util;
using System;

namespace ch.appl.psoft.Dispatch
{
    /// <summary>
    /// Summary description for DocumentAdd.
    /// </summary>
    public partial class DocumentAdd : System.Web.UI.Page
	{
        protected string _onloadString = "";
        protected string _backURL = "";

        protected System.Web.UI.WebControls.Table attachmentTab;

        protected DBData _db = null;
        protected LanguageMapper _map = null;
        protected int _mailingID = -1;
        protected DispatchDocument.DOCUMENT_TYPE _type = DispatchDocument.DOCUMENT_TYPE.MAIL_ATTACHMENT;
        protected bool _asEmailTemplate = false;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            _db = DBData.getDBData(Session);
            _map = LanguageMapper.getLanguageMapper(Session);
            _mailingID = ch.psoft.Util.Validate.GetValid(Request.QueryString["mailingID"], _mailingID);
            _backURL = ch.psoft.Util.Validate.GetValid(Request.QueryString["backURL"], _backURL);
            _type = (DispatchDocument.DOCUMENT_TYPE) ch.psoft.Util.Validate.GetValid(Request.QueryString["type"], (int) _type);
            _asEmailTemplate = bool.Parse(ch.psoft.Util.Validate.GetValid(Request.QueryString["asEmailTemplate"], "false"));

            if (!IsPostBack)
            {
                ok.Text = _map.get("next");
                switch (_type)
                {
                    case DispatchDocument.DOCUMENT_TYPE.MAIL_ATTACHMENT:
                        title.Text = _map.get("dispatch", "addAttachmentTitle");
                        break;

                    case DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE:
                        title.Text = _map.get("dispatch", "addTemplateTitle");
                        break;
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
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

        }
		#endregion

        protected void ok_Click(object sender, System.EventArgs e)
        {
            try
            {
                _db.connect();
                switch (_type)
                {
                    case DispatchDocument.DOCUMENT_TYPE.MAIL_ATTACHMENT:
                        _db.Mailing.addAttachment(_mailingID, fileNewDocument.PostedFile, true);
                        break;

                    case DispatchDocument.DOCUMENT_TYPE.MAILING_TEMPLATE:
                        if (_asEmailTemplate)
                            _db.Mailing.addEmailTemplate(_mailingID, fileNewDocument.PostedFile, true);
                        else
                            _db.Mailing.addLetterTemplate(_mailingID, fileNewDocument.PostedFile, true);
                        break;
                }
                _onloadString = "RefreshAndClose();";
            }
            catch(Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                _db.disconnect();
            }
        }
	}
}

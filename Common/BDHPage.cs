using ch.appl.psoft.LayoutControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for AbstractPage.
    /// </summary>
    public class PSOFTPage : System.Web.UI.Page
    {
        private PageLayoutControl _pageLayoutControl = null;
        private string _pageTitle = ".: p-soft :.";
        private bool _errorMessageSetted = false;
        private int _ctrlId = 0;
        private string _method = "post";
        public HtmlHead head = new HtmlHead();
        public ScriptManager sm = new ScriptManager();


        public PSOFTPage() : base() { }

        public PSOFTPage(string method) : base() { _method = method; }

        #region Properties
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; }
        }

        /// <summary>
        /// Page layut control. Main layout of this page.
        /// </summary>
        public PageLayoutControl PageLayoutControl
        {
            get { return _pageLayoutControl; }
            set { SetPageLayoutControl(value); }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method is a setter for PageLayoutcontrol property.
        /// Its uses DoSetPageLayoutControl method to put control
        /// on the form tag of this page.
        /// </summary>
        /// <param name="control">Page layout control to put on this form</param>
        public void SetPageLayoutControl(PageLayoutControl control)
        {
            _pageLayoutControl = control;
            DoSetPageLayoutControl(control);
        }

        public void ShowError(string errorMessage)
        {
            _errorMessageSetted = true;
            if (_pageLayoutControl != null)
                _pageLayoutControl.ErrorMessage = errorMessage;
        }

        protected virtual void ExceptionMethod(object sender, Exception except)
        {
            this.ShowError(except.Message);
        }

        public PSOFTUserControl LoadPSOFTControl(string path, string id)
        {
            _ctrlId++;
            PSOFTUserControl _ctrl = (PSOFTUserControl)Page.LoadControl(path);
            //PSOFTUserControl _ctrl = (PSOFTUserControl)Master.LoadControl(path);
            _ctrl.ID = id;
            return _ctrl;
        }

        public PSOFTUserControl LoadPSOFTControl(string path)
        {
            return LoadPSOFTControl(path, "_PSOFT_uc_id" + _ctrlId);
        }

        public void SetPageLayoutContentControl(string contentPlaceName, PSOFTUserControl control)
        {
            if (_pageLayoutControl != null)
            {
                _pageLayoutControl.SetContentControl(contentPlaceName, control);
                control.OnException += new ExceptionEventHandler(ExceptionMethod);
            }
        }
        #endregion

        #region Protected virtual methods to override in child classes

        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// This method sets page layout control on the <form> tag
        /// of this page.
        /// If you would like to put page layout control somewhere else
        /// you should override this method in your child classes
        /// (without invoking base.DoSetPageLayoutControl(control); )
        /// </summary>
        /// <param name="control"></param>
        protected virtual void DoSetPageLayoutControl(PageLayoutControl control)
        {
            // add Control to ContentPlaceHolder on Masterpage / 09.11.09 / mkr
            this.Master.FindControl("ContentPlaceHolder1").Controls.Add(control);

            //foreach (Control _ctrl in this.Controls)
            //    if (_ctrl is System.Web.UI.HtmlControls.HtmlForm)
            //    {
            //        _ctrl.Controls.Add(control);
            //        break;
            //    }
        }

        /// <summary>
        /// This method should return a whole content of aspx page with javascript
        /// links
        /// This method should be overrided in child classes.
        /// </summary>
        protected virtual void AppendJavaScripts(StringBuilder javaScripts)
        {
        }

        protected virtual void AppendCSSLink(StringBuilder cssLinks)
        {
        }

        protected virtual void AppendMetaTags(StringBuilder metaTags)
        {
            // disabled, is now handled by masterpage / 09.11.09 / mkr
            //metaTags.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">\n");
            //metaTags.Append("<meta http-equiv=\"Content-Language\" content=\"de-de\">\n");
            //metaTags.Append("<meta http-equiv=\"Content-Script-Type\" content=\"text/javascript\">\n");

            //metaTags.Append("<meta http-equiv=\"expires\" content=\"3600\">\n");

            // Show the class that generated this output.  Note that it uses
            // BaseType so that it shows the actual class name rather than
            // the ASP.NET generated derived page class name.
            // disabled, is now handled by masterpage / 09.11.09 / mkr
            //metaTags.Append("<meta name=\"GENERATOR\" content=\"");
            //metaTags.Append(GetType().BaseType);
            //metaTags.Append(" Class\">\n");
        }

        protected virtual void AppendAdditionalHeaderTags(StringBuilder headerTags)
        {
        }

        protected virtual void AppendBodyOnLoad(StringBuilder bodyOnLoad)
        {
        }

        protected virtual void AppendBodyOnBeforeUnload(StringBuilder bodyOnUnload)
        {
        }

        protected virtual void AppendAdditionalLiteralControls(StringBuilder literalControls)
        {
        }
        #endregion

        #region Protected overrided methods from parent class

        protected override void OnInit(System.EventArgs e)
        {
            Initialize();
            this.SmartNavigation = false;
            base.OnInit(e);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            _errorMessageSetted = false;
            //Add Scriptmanager auskommentiert (scriptmanager in Masterpage hinzugefügt) 2012.02.22 MSr
            BuildPage(GenerateHtmlForm());
            base.OnLoad(e);
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            // Hide (or not) error message
            if ((_pageLayoutControl != null) && (!_errorMessageSetted))
                _pageLayoutControl.ErrorMessage = null;

            // move controls to masterpage contentplaceholder / 26.10.09 / mkr
            //try
            //{
            //    //Control conPageLayout = this.Controls[2].Controls[2];
            //    //Control conMasterPage = this.Controls[2].Controls[1];
            //    //Control conPlaceHolder = conMasterPage.Controls[4].Controls[3];

            //    //conPlaceHolder.Controls.Add(conPageLayout);

            //    //this.Controls.RemoveAt(2);

            //    //this.Controls.Add(conMasterPage);

            //    Control conMasterPage = this.Controls[2].Controls[1];
            //    this.Controls.Add(conMasterPage);
            //    this.Controls.RemoveAt(2);
            //}
            //catch
            //{
            //    // do nothing if controls not available
            //}

            base.OnPreRender(e);
        }

        //protected override void Render(HtmlTextWriter writer)
        //{
        // *** Write the HTML into this string builder

        //StringBuilder sb = new StringBuilder();

        //StringWriter sw = new StringWriter(sb);

        //foreach (Control c in this.Controls)
        //{
        //    if (c.GetType().Equals(new HtmlForm()))
        //    {
        //        foreach (Control sc in c.Controls)
        //        {
        //            this.Controls.Add(sc);
        //        }
        //    }
        //}

        //Control f = this.Controls[2];

        //this.Controls.RemoveAt(2);

        //ContentPlaceHolder cph = new ContentPlaceHolder();


        //for (int idx = f.Controls.Count - 1; idx >= 0; idx--)
        //{
        //    //this.Controls.Add(f.Controls[idx]);
        //    cph.Controls.Add(f.Controls[idx]);
        //}

        //foreach (Control sc in f.Controls)
        //{
        //    this.Controls.Add(sc);
        //}

        //this.Controls.Add(cph);

        //ControlCollection temp = this.Controls;

        //Control conPageLayout = this.Controls[2].Controls[2];
        //Control conMasterPage = this.Controls[2].Controls[1];
        //Control conPlaceHolder = conMasterPage.Controls[4].Controls[3];

        //conPlaceHolder.Controls.Add(conPageLayout);

        //this.Controls.RemoveAt(2);

        //this.Controls.Add(conMasterPage);


        //    HtmlTextWriter hWriter = new HtmlTextWriter(sw);

        //    base.Render(hWriter);

        //    // *** store to a string

        //    string PageResult = sb.ToString();

        //    // *** Write it back to the server

        //    writer.Write(PageResult);

        //}

        /// <summary>
        /// Store the viewstate in an alternative media
        /// </summary>
        protected override void SavePageStateToPersistenceMedium(object viewState)
        {
            //this.Session["__PAGE_VIEW_STATE" + GetType().Name] = viewState;
            //base.SavePageStateToPersistenceMedium(viewState);

            //LosFormatter format = new LosFormatter();
            //StringWriter writer = new StringWriter();

            //format.Serialize(writer, viewState);
            //string viewStateStr = writer.ToString();

            //byte[] bytes = System.Convert.FromBase64String(viewStateStr);

            //bytes = Compress(bytes);
            //string vStateStr = System.Convert.ToBase64String(bytes);
            //ClientScript.RegisterHiddenField("__VSTATE", vStateStr);

            LosFormatter lf = new LosFormatter();
            StringWriter sw = new StringWriter();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string fileName = this.GenerateFileName();

            lf.Serialize(sw, viewState);
            sb = sw.GetStringBuilder();
            WriteFile(sb.ToString(), fileName);

            sb = null;
            lf = null;
            sw = null;
        }


        /// <summary>
        /// Load viewstate from an alternative media
        /// </summary>
        protected override object LoadPageStateFromPersistenceMedium()
        {
            //return this.Session["__PAGE_VIEW_STATE" + GetType().Name];
            //return base.LoadPageStateFromPersistenceMedium();

            //string vState = Request.QueryString["__VSTATE"];
            //byte[] bytes = System.Convert.FromBase64String(vState);
            //bytes = Decompress(bytes);

            //LosFormatter format = new LosFormatter();
            //return format.Deserialize(System.Convert.ToBase64String(bytes));

            Object deserializedValue = null;
            string fileName = GetFileName();
            LosFormatter lf = new LosFormatter();

            deserializedValue = lf.Deserialize(ReadFile(fileName));

            lf = null;

            return deserializedValue;
        }

        /// <summary>
        /// Compress data using IO.Compression
        /// </summary>
        private byte[] Compress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedZipStream = new GZipStream(ms, CompressionMode.Compress);
            compressedZipStream.Write(buffer, 0, buffer.Length);
            compressedZipStream.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Decompress data using IO.Compression
        /// </summary>
        private byte[] Decompress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);
            ms.Seek(0, SeekOrigin.Begin);
            GZipStream zipStream = new GZipStream(ms, CompressionMode.Decompress);
            List<byte> inflated = new List<byte>();

            while (true)
            {
                int b = zipStream.ReadByte();

                if (b == -1)
                {
                    break;
                }

                inflated.Add((byte)b);
            }

            return inflated.ToArray();
        }

        private string ReadFile(string fileName)
        {
            string fileContent = string.Empty;

            StreamReader objReader = new StreamReader(fileName, System.Text.Encoding.UTF8);
            fileContent = objReader.ReadToEnd();
            objReader.Close();
            objReader = null;
            return fileContent;
        }

        private void WriteFile(string fileContent, string fileName)
        {
            StreamWriter objWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
            objWriter.Write(fileContent);
            objWriter.Close();

            objWriter = null;
        }

        private string GenerateFileName()
        {
            string[] dir_arr = Request.Url.AbsolutePath.Split(new char[] { '/' });
            string dir;
            if (Global.Config.baseURL.Trim().Length == 0)
            {
                dir = "";
            }
            else
            {
                dir = "/" + dir_arr[1];
            }
            string pfad = Server.MapPath(dir + "/tmp");
            string fileName = Path.Combine(pfad, Guid.NewGuid().ToString());
            ClientScript.RegisterHiddenField("__VIEWSTATEGUID__", fileName);
            return fileName;
        }

        private string GetFileName()
        {
            string fileName = string.Empty;

            if (Request.QueryString["__VIEWSTATEGUID__"] != null)
            {
                fileName = Request.QueryString["__VIEWSTATEGUID__"];
            }
            else
            {
                fileName = Request.Form["__VIEWSTATEGUID__"];
            }

            if (!File.Exists(fileName))
                throw new FileNotFoundException("The viewstate file cannot be found", fileName);

            return fileName;
        }

        #endregion

        #region protected attribute access
        protected string Method
        {
            get { return this._method; }
            set { this._method = value; }
        }

        #endregion

        #region Layout engine
        private void BuildPage(HtmlForm form)
        {
            this.Controls.Add(new LiteralControl(RenderHeader()));
            //this.Controls.Add(head);
            //sm.EnableScriptGlobalization = true;
            //sm.EnableScriptLocalization = true;
            //this.Controls.Add(sm);
            // add ScriptManager to Masterpage / 18.05.10 / mkr
            if (!Request.RawUrl.Contains("goto.aspx"))
            {
                try
                {
                    //this.Master.FindControl("ContentPlaceHolder1").Controls.Add(sm);
                }
                catch
                {
                }
            }
            form.Method = _method;
            //this.Controls.Add(form);
            this.Controls.Add(new LiteralControl(RenderFooter()));
        }

        private HtmlForm GenerateHtmlForm()
        {
            HtmlForm form = new HtmlForm();
            //AddControlsFromDerivedPage(form);
            return form;
        }

        private void AddControlsFromDerivedPage(HtmlForm form)
        {
            ControlCollection lst = form.Controls;

            // DIV WebService
            lst.Add(new LiteralControl(RenderAdditionalLiteralControls()));

            // derived page
            int count = this.Controls.Count;
            for (int i = 0; i < count; ++i)
            {
                System.Web.UI.Control ctrl = this.Controls[0];
                lst.Add(ctrl);
                this.Controls.Remove(ctrl);
            }
        }

        private string RenderAdditionalLiteralControls()
        {
            StringBuilder _additionalLiteral = new StringBuilder();
            AppendAdditionalLiteralControls(_additionalLiteral);
            return _additionalLiteral.ToString();
        }

        private string RenderHeader()
        {
            // disabled, HTML is generated from masterpage / 26.10.09 / mkr

            // append Javascripts to Masterpage
            if (!Request.RawUrl.Contains("goto.aspx"))
            {
                try
                {
                    StringBuilder js = new StringBuilder();
                    AppendJavaScripts(js);
                    LiteralControl lc = new LiteralControl(js.ToString());
                    this.Master.FindControl("head").Controls.Add(lc);
                }
                catch
                {
                }
            }

            //string test = this.GetType().ToString();

            //if (temp != null)
            //{
            //    temp.Controls.Add(lc);
            //}

            //Control temp = this.Master.Controls[0];

            return "";

            //StringBuilder strHeader = new StringBuilder(1024);
            //StringBuilder strHeader2 = new StringBuilder(1024);
            //// Write out the stock header
            //strHeader.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">\n");
            //strHeader.Append("<html>\n");
            //strHeader2.Append("<title>");
            //strHeader2.Append(HttpUtility.HtmlEncode(PageTitle));
            //strHeader2.Append("</title>\n");

            //AppendMetaTags(strHeader2);
            //AppendCSSLink(strHeader2);
            //AppendJavaScripts(strHeader2);
            //AppendAdditionalHeaderTags(strHeader2);

            //strHeader2.Append("<script language=\"JavaScript\">\n");
            //strHeader2.Append("function bodyOnLoad()\n"); 
            //strHeader2.Append("{\n");
            //AppendBodyOnLoad(strHeader2);
            //strHeader2.Append("}\n"); 
            //strHeader2.Append("\n"); 
            //strHeader2.Append("function bodyOnUnLoad()\n"); 
            //strHeader2.Append("{\n");
            //AppendBodyOnBeforeUnload(strHeader2);
            //strHeader2.Append("}\n"); 
            //strHeader2.Append("</script>\n"); 

            //strHeader2.Append("\n");
            //head.InnerHtml = strHeader2.ToString();

            //strHeader.Append("<body onLoad=\"bodyOnLoad()\" onBeforeUnload=\"bodyOnUnLoad()\">\n");
            //return strHeader.ToString();
        }

        protected override void InitializeCulture()
        {
            // set culture according to selected language / 12.01.10 / mkr
            string cultureInfo = "de-CH";

            if (Session["culture"] != null && Session["culture"].ToString() != "")
            {
                cultureInfo = Session["culture"].ToString();
            }

            CultureInfo culture = new CultureInfo(cultureInfo);

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            base.InitializeCulture();
        }

        private string RenderFooter()
        {
            // disabled, HTML is generated from masterpage / 26.10.09 / mkr
            return "";

            //return "\n</body>\n</html>\n";
        }
        #endregion
    }
}
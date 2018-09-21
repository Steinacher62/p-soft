using ch.psoft.Util;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for PropertyBox.
    /// </summary>
    public partial class InfoBox : System.Web.UI.Page {
        public static string Path {
            get {return Global.Config.baseURL + "/Common/InfoBox.aspx";}
        }

        public static void addInfoHandler(WebControl ctrl, string info, int maxWidth, int maxHeight) {
            string url = InfoBox.Path+"?info="+PSOFTConvert.ToJavascript(PSOFTConvert.ToJavascript(info))
                +"&maxWidth="+maxWidth+"&maxHeight="+maxHeight;
            
            ctrl.Attributes.Add("onmouseout","hideInfo()");
            ctrl.Attributes.Add("onmouseover","showInfo('"+url+"')");
        }


        protected void Page_Load(object sender, System.EventArgs e) {
            TableRow r = new TableRow();
            string info = ch.psoft.Util.Validate.GetValid(Request.QueryString["info"],"Info");
            int maxWidth = ch.psoft.Util.Validate.GetValid(Request.QueryString["maxWidth"],0);
            int maxHeight = ch.psoft.Util.Validate.GetValid(Request.QueryString["maxHeight"],0);
            TableCell infoCell = new TableCell();


            r.Cells.Add(infoCell);
            infoTab.Rows.Add(r);

            infoCell.CssClass = "message";
            infoCell.HorizontalAlign = HorizontalAlign.Justify;
            infoCell.Text = PSOFTConvert.ToHtml(info);
            if (maxWidth > 0) infoTab.Width = maxWidth;
            if (maxHeight > 0) infoTab.Height = maxHeight;

        }

		#region Web Form Designer generated code
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {    
        }
		#endregion
    }
}

using ch.psoft.Util;
using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for PropertyBox.
    /// </summary>
    public partial class PropertyBox : System.Web.UI.Page {
        public static string Path {
            get {return Global.Config.baseURL + "/Common/PropertyBox.aspx";}
        }

        public static void addPropertyHandler(WebControl ctrl, string info) {
            string url = PropertyBox.Path+"?"+PSOFTConvert.ToJavascript(PSOFTConvert.ToJavascript(info));
            
            ctrl.Attributes.Add("onmouseout","hidePropertyBox()");
            ctrl.Attributes.Add("onmouseover","showPropertyBox('"+url+"')");
        }

        protected void Page_Load(object sender, System.EventArgs e) {
            System.Collections.Specialized.NameValueCollection properties = this.Request.Params;
            string[] keys = properties.AllKeys;
            string[] values;
            int idx = 0;
            TableRow r;
            TableCell c;

            foreach (string s in keys) {
                if (s.EndsWith(":")) {
                    r = new TableRow();
                    r.CssClass = "PropertyBox";
                    r.BorderWidth = 0;
                    c = new TableCell();
                    c.CssClass = "PropertyBox_Label";
                    c.BorderWidth = 0;
                    c.Wrap = false;
                    c.Text = PSOFTConvert.ToHtml(s);
                    c.VerticalAlign = VerticalAlign.Top;
                    r.Cells.Add(c);
                    c = new TableCell();
                    c.BorderWidth = 0;
                    c.Width = 5;
                    r.Cells.Add(c);
                    c = new TableCell();
                    c.CssClass = "PropertyBox_Value";
                    c.Wrap = true;
                    values = properties.GetValues(idx);
                    c.Text = PSOFTConvert.ToHtml(values[0]);
                    r.Cells.Add(c);
                    propertyTab.Rows.Add(r);
                }
                idx++;
            }
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

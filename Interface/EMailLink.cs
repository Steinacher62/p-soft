namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Wrapper class for eMails
    /// </summary>
    public class EMailLink : System.Web.UI.WebControls.HyperLink {
        public EMailLink() : base() {
        }
        /// <summary>
        /// Get/Set eMail address
        /// </summary>
        public new string NavigateUrl {
            get { return base.NavigateUrl; }
            set { 
                if (value != null && value != "") base.NavigateUrl = "mailto:"+value; 
                else base.NavigateUrl = value;
            }
        }
    }
}

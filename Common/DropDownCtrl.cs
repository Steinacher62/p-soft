using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for DropDownCtrl.
    /// </summary>
    public class DropDownCtrl : DropDownList
	{
		public DropDownCtrl() : base()
		{
            base.Attributes.Add("onkeypress", "quickSearchDropDownPressed()");
            // set CSS class / 02.02.10 / mkr
            base.CssClass = "inputMask_Value";
        }
	}
}

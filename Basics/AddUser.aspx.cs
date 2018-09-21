using System;
using System.Web.UI;
using Telerik.Web.UI;



namespace ch.appl.psoft.Basics
{
    public partial class AddUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DropDownListItem item = new DropDownListItem();
                item.Text = "Frau";
                DropDownListSalutation.Items.Add(item);
                DropDownListItem item1 = new DropDownListItem();
                item1.Text = "Herr";
                DropDownListSalutation.Items.Add(item1);
                DropDownListSalutation.DefaultMessage = "Wählen";
                DropDownListSalutation.SelectedIndex = -1;
            }
        }

    }
}

using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Basics
{
    public partial class chooseProduct : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("login.aspx");
        }

        protected void Description_Click(object sender, EventArgs e)
        {

        }

        protected void Oder_Click(object sender, EventArgs e)
        {
            string mapQuantity ="";
            string price ="";
            string plan = "";
            string operationaltime ="";

            switch (((Button)sender).CommandArgument)
            {
                case "1":
                    mapQuantity = this.MapQuantity1.Text;
                    price = this.Price1.Text;
                    plan = this.Plan1.Text;
                    operationaltime = this.OperationalTime1.Text;
                    break;
                case "2":
                    mapQuantity = this.MapQuantity2.Text;
                    price = this.Price2.Text;
                    plan = this.Plan2.Text;
                    operationaltime = this.OperationalTime2.Text;
                    break;
                case "3":
                    mapQuantity = this.MapQuantity3.Text;
                    price = this.Price3.Text;
                    plan = this.Plan3.Text;
                    operationaltime = this.OperationalTime3.Text;
                    break;
                case "4":
                    mapQuantity = this.MapQuantity4.Text;
                    price = this.Price4.Text;
                    plan = this.Plan4.Text;
                    operationaltime = this.OperationalTime4.Text;
                    break;
            }

            Session.Add("mapQuantity", mapQuantity);
            Session.Add("price", price);
            Session.Add("plan", plan);
            Session.Add("operationaltime", operationaltime);

            Response.Redirect("addUser.aspx");



        }
    }
}
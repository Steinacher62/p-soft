using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Morph.Controls;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;


namespace ch.appl.psoft.Morph
{
    public partial class MyMaps : PsoftDetailPage
    {
        private const string PAGE_URL = "/Morph/MyMaps.aspx";

        static MyMaps()
        {
            SetPageParams(PAGE_URL, "context");
        }

        public static string GetURL(params object[] queryParams)
        {
            return CreateURL(PAGE_URL, queryParams);
        }

        public MyMaps() : base()
        {
            PageURL = PAGE_URL;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["culture"] = SessionData.getDBColumn(Session).UserCultureName;

            this.BreadcrumbCaption = _mapper.get("morph", "myProducts");
            this.BreadcrumbLink = Global.Config.baseURL + "/Morph/MyMaps.aspx";

            string myURL = this.Request.RawUrl;

            //// Setting main page layout
            //PageLayoutControl = (MainPageLayout)this.LoadPSOFTControl(MainPageLayout.Path, "_mainPl");

            //// Setting content layout of page layout
            //PageLayoutControl.ContentLayoutControl = (MainContentLayout)this.LoadPSOFTControl(MainContentLayout.Path, "_mainCl");

            titleTb.Text = "Meine Karten";
            ClientDataSource.DataSource.WebServiceDataSourceSettings.BaseUrl = Global.Config.baseURL + "/WebService/PsoftService1.asmx/";


            if (Global.Config.BackgroundStartpage.Length > 0)
            {

                Container.Style.Add("background-image", ".." + Global.Config.BackgroundStartpage);
                Container.Style.Add("background-size", "100%");
                //mainLayoutTable.Style.Add("opacity", "0.1");
                //mainLayoutTable.Style.Add("filter", "alpha(opacity = 50)");
            }
        }

        protected void myProductsGrid_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {

            //GridDataItem item = e.Item as GridDataItem;


            //RadToolTip toolTip = new RadToolTip();
            //toolTip.ID = "RadToolTip1";
            //toolTip.Text = "test";
            //toolTip.TargetControlID = "Label16702"; //item.Controls[1].ClientID;
            //toolTip.IsClientID = true;
            //toolTip.RelativeTo = ToolTipRelativeDisplay.Element;
            //toolTip.Position = ToolTipPosition.MiddleRight;
            //toolTip.ShowDelay = 50;
            //toolTip.AutoCloseDelay = 20000;
            //toolTip.Width = 400;



            //item.Cells[1].Controls.Add(toolTip);
            //Button btn2 = new Button();
            //btn2.Text = "Another button";
            //btn2.ID = "Button2";
            //item["Column2"].Controls.Add(btn2);

        }

        protected void myProductsGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                //GridDataItem item = e.Item as GridDataItem;
                //RadToolTip toolTip = new RadToolTip();
                //toolTip.ID = "RadToolTip1";
                //toolTip.Text = "test";
                //toolTip.TargetControlID = "Label16702"; //item.Controls[1].ClientID;
                //toolTip.IsClientID = true;
                //toolTip.RelativeTo = ToolTipRelativeDisplay.Element;
                //toolTip.Position = ToolTipPosition.MiddleRight;
                //toolTip.ShowDelay = 50;
                //toolTip.AutoCloseDelay = 20000;
                //toolTip.Width = 400;



                //item.Cells[1].Controls.Add(toolTip);
            }
        }
    }
}
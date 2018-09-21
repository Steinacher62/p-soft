using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using System;
using System.Data;
using System.Web.UI.HtmlControls;



namespace ch.appl.psoft.Morph
{
    public partial class MapSelection : PsoftTreeViewPage
    {
        //// Fields
        //private MatrixListCtrl _list;
        //private MatrixSearchCtrl _search;
        //public const string MODE_SLAVE = "slave";
        //public const string ORDER_COLUMN = "TITLE";
        //private const string PAGE_URL = "/Morph/MapSelection.aspx";

        //// Methods
        //static MapSelection()
        //{
        //    PsoftPage.SetPageParams("/Morph/MapSelection.aspx", new string[] { "type", "nextURL" });
        //}

        //public MapSelection()
        //{
        //    base.PageURL = "/Morph/MapSelection.aspx";
        //}

        //public static string GetURL(params object[] queryParams)
        //{
        //    return PsoftPage.CreateURL("/Morph/MapSelection.aspx", queryParams);
        //}

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //}

        //private void InitializeComponent()
        //{
        //}

        //protected override void OnInit(EventArgs e)
        //{
        //    this.InitializeComponent();
        //    base.OnInit(e);
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            base.BreadcrumbCaption = base._mapper.get("morph", "bcMapSelection");
            base.PageLayoutControl = (PsoftPageLayout)base.LoadPSOFTControl(PsoftPageLayout.Path, "_sL");
            //MapSelectionCtrl mapSelectionCtrl = (MapSelectionCtrl)base.LoadPSOFTControl(MapSelectionCtrl.Path, "_detail");
            (base.PageLayoutControl as PsoftPageLayout).PageTitle = base.BreadcrumbCaption;
            //base.SetPageLayoutContentControl("MAP SELECTION", mapSelectionCtrl);
            
            DBData db = DBData.getDBData(base.Session);
            DataTable submatrix;
            if (!Global.isModuleEnabled("gfk"))
            {
                submatrix = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " ORDER BY TITLE");
            }
            else
            {
                submatrix = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 0 ORDER BY TITLE DESC");
                DataTable submatrix1 = db.getDataTable("SELECT DISTINCT ID, TITLE FROM MATRIX " + db.getAccessRightsRowInnerJoinSQL("Matrix", DBData.AUTHORISATION.ADMIN, DBData.APPLICATION_RIGHT.COMMON, true, true) + " WHERE IS_GFK_TEMPLATE = 1 ORDER BY TITLE DESC");
                submatrix.Merge(submatrix1);
                submatrix.DefaultView.Sort = "TITLE";
            }


            HtmlTable tableList = new HtmlTable();
            Boolean alternate = true;

            foreach (DataRow row in submatrix.Rows)
            {
                HtmlTableRow newRow = new HtmlTableRow();
                HtmlTableCell newCell = new HtmlTableCell();
                HtmlAnchor newAnchor = new HtmlAnchor();

                newAnchor.InnerText = row.ItemArray[1].ToString();
                newAnchor.HRef = Global.Config.baseURL+"/Morph/MatrixDetail.aspx?matrixId="+row.ItemArray[0];

                newCell.Controls.Add(newAnchor);
                newRow.Controls.Add(newCell);

                if (alternate)
                {
                    newRow.Style.Add("background-color", "rgb(237, 240, 255)");
                }

                alternate = !alternate;

                tableList.Controls.Add(newRow);
            }
            tableList.CellPadding = 3;
            tableList.Attributes.CssStyle.Add("top", "6px");
            tableList.Attributes.CssStyle.Add("position", "relative");
            tableList.Attributes.CssStyle.Add("width", "100%");
            this.PageLayoutControl.Controls[0].Controls[3].Controls[0].Controls.Add(tableList);
        }
    }
}
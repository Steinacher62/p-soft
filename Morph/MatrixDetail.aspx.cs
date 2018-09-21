using ch.appl.psoft.Common;
using ch.appl.psoft.db;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Morph.Controls;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Web.UI;

namespace ch.appl.psoft.Morph
{
    public partial class MatrixDetail : PsoftDetailPage
    {
        // Fields
        public const string MATRIX_ID = "matrixID";
        public const string PAGE_URL = "/Morph/MatrixDetail.aspx";
        public const string SLAVE = "slave";

        // Methods
        static MatrixDetail()
        {
            PsoftPage.SetPageParams("/Morph/MatrixDetail.aspx", new string[] { "matrixID", "slave" });
        }

        public MatrixDetail()
        {
            base.PageURL = "/Morph/MatrixDetail.aspx";
            this.ShowProgressBar = false;
        }

        public static string GetURL(params object[] queryParams)
        {
            return PsoftPage.CreateURL("/Morph/MatrixDetail.aspx", queryParams);
        }

        private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitializeComponent();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

    
            }
            //Kompatibilitätsmodus ausschalten für diese Seite MSr
            Response.AddHeader("X-UA-Compatible", "IE=edge");

            DBData data = DBData.getDBData(this.Session);
            PsoftPageLayout PsoftPageLayout = (PsoftPageLayout)base.LoadPSOFTControl(PsoftPageLayout.Path, "_pl");
            base.PageLayoutControl = PsoftPageLayout;
            DGLContentLayout layout = (DGLContentLayout)base.LoadPSOFTControl(DGLContentLayout.Path, "_cl");
            base.PageLayoutControl.ContentLayoutControl = layout;

            ////show print button / 07.12.10 / mkr
            //PsoftPageLayout.ButtonPrintAttributes.Add("onClick", "javascript:window.open(document.location.href + '&print=true')");
            //PsoftPageLayout.ButtonPrintVisible = true;
            String matrixId = base.GetQueryValue("matrixID","-1");
            long queryValue;
            Boolean isNovisReport;
            if (matrixId.StartsWith("Novis"))
            {
                long.TryParse(matrixId.Replace("Novis", ""), out queryValue);
                isNovisReport = true;
            }
            else
            {
                queryValue = base.GetQueryValue("matrixID", (long)(-1L));
                isNovisReport = false;
            }


            bool flag = true;
            try
            {
                flag = bool.Parse(base.GetQueryValue("slave", "false"));
            }
            catch (Exception exception)
            {
                flag = false;
                Logger.Log(exception, Logger.DEBUG);
            }
            data.connect();
            try
            {
                string tableName = flag ? "SLAVE" : "MATRIX";
                if ((queryValue > 0L) && data.hasRowAuthorisation(2, tableName, queryValue, true, true))
                {
                    (base.PageLayoutControl as PsoftPageLayout).PageTitle = base.BreadcrumbCaption = data.Matrix.getTitle(queryValue);

                    MatrixDetailCtrl control =  (MatrixDetailCtrl)base.LoadPSOFTControl(MatrixDetailCtrl.Path, "_detail");
                    
                    if (!flag)
                    {
                        control.MatrixID = queryValue;
                        control.IsNovisReport = isNovisReport;
                    }
                    else
                    {
                        control.SlaveID = queryValue;
                        long valid = SQLColumn.GetValid(data.lookup("MATRIX_ID", "SLAVE", " ID = " + queryValue), (long)(-1L));
                        string str2 = SQLColumn.GetValid(data.lookup("TITLE", "MATRIX", " ID = " + valid), "");
                        (base.PageLayoutControl as PsoftPageLayout).PageTitle = base.BreadcrumbCaption = data.Matrix.getSlaveTitle(queryValue) + " - " + str2;
                    }
                    base.SetPageLayoutContentControl("DETAIL", control);
                }
                else
                {
                    base.BreadcrumbVisible = false;
                    base.Response.Redirect(NotFound.GetURL(new object[0]), false);
                }
            }
            catch (Exception exception2)
            {
                base.ShowError(exception2.Message);
                Logger.Log(exception2, Logger.ERROR);
            }
            finally
            {
                data.disconnect();
            }
        }
    }
}


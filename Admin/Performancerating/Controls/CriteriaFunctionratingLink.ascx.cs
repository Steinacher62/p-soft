using ch.appl.psoft.Admin.Performancerating.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Performancerating.Controls
{
    public partial class CriteriaFunctionratingLink : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            ContainerTitle.Text = _map.get("performanceRating", "functionRatingItemsCriterias");
            FunctionRatingItemTitle.Text = _map.get("performanceRating", "functionRatingItems");
            CriteriaTitle.Text = _map.get("performanceRating", "criterias");
            DBData db = DBData.getDBData(Session);
            db.connect();
            if (!IsPostBack)
            {
                DataTable functioncriteria = db.getDataTable("SELECT ID, BEZEICHNUNG FROM FBW_KRITERIUM");
                FunctionCriteriaListBox.DataSource = functioncriteria;
                FunctionCriteriaListBox.DataValueField = "ID";
                FunctionCriteriaListBox.DataTextField = "BEZEICHNUNG";
                FunctionCriteriaListBox.DataBind();

                DataTable ratingItemLink = db.getDataTable("SELECT PERFORMANCE_CRITERIA.ID, FBW_KRITERIUM.PERFORMANCE_CRITERIA_REF, PERFORMANCE_CRITERIA.TITLE_" + lang+" AS TITLE FROM FBW_KRITERIUM INNER JOIN PERFORMANCE_CRITERIA ON FBW_KRITERIUM.PERFORMANCE_CRITERIA_REF = PERFORMANCE_CRITERIA.ID");
                CriteriaListBoxLink.DataSource = ratingItemLink;
                CriteriaListBoxLink.DataValueField = "ID";
                CriteriaListBoxLink.DataTextField = "TITLE";
                CriteriaListBoxLink.DataBind();
                db.disconnect();
            }

        }
    }
}
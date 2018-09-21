using ch.appl.psoft.Admin.Performancerating.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Performancerating.Controls
{
    public partial class CriteriaDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            detailTitle.Text = _map.get("performanceRating", "criteria");
            titleLabel.Text = _map.get("performanceRating", "title");
            descriptionLabel.Text = _map.get("performanceRating", "description");
            FunctionRatingLabel.Text = _map.get("performanceRating", "linkToFunctionRatingItem");

            DBData db = DBData.getDBData(Session);
            db.connect();
            DataTable functioncriteria = db.getDataTable("SELECT ID, BEZEICHNUNG FROM FBW_KRITERIUM");
            DataRow emtyRow = functioncriteria.NewRow();
            emtyRow["ID"] = 0;
            emtyRow["BEZEICHNUNG"] = "";
            functioncriteria.Rows.Add(emtyRow);

            FunctionRatingLinkDD.DataSource = functioncriteria;
            FunctionRatingLinkDD.DataValueField = "ID";
            FunctionRatingLinkDD.DataTextField = "BEZEICHNUNG";
            FunctionRatingLinkDD.DataBind();
            FunctionRatingLinkDD.Items.Sort();

            db.disconnect();
        }
    }
}
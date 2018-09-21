using ch.appl.psoft.Admin.Functions.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Functions
{
    public partial class FunctionRating : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORMRU_Layout.MainTitle = _map.get("functionRating", "name");
            LRORMRU_Layout.pageViewL.Controls.Add((FunctionRatingTreeCtrl)LoadControl("Controls/FunctionRatingTreeCtrl.ascx"));
            LRORMRU_Layout.pageViewRO.Controls.Add((FunctionRatingDetailCtrl)LoadControl("Controls/FunctionRatingDetailCtrl.ascx"));
            LRORMRU_Layout.pageViewRM.Controls.Add((FunctionRatingRatingDetailCtrl)LoadControl("Controls/FunctionRatingRatingDetailCtrl.ascx"));
            LRORMRU_Layout.pageViewRU.Controls.Add((FunctionRatingRatingTreeCtrl)LoadControl("Controls/FunctionRatingRatingTreeCtrl.ascx"));
            ReferenceTitle.Text = _map.get("functionRating", "functionArgumnetReferences");
            RatingItemDetailWindow.Title = _map.get("functionRating", "descriptionExamples");
            itemTitleLabel.Text = _map.get("functionRating", "title");
            itemValueTitle.Text = _map.get("functionRating", "value");
            itemDescriptionTitle.Text = _map.get("functionRating", "description");
            itemExamplesTitle.Text = _map.get("functionRating", "examples");
        }
    }
}
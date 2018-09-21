using ch.appl.psoft.Admin.Performancerating.Controls;
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

namespace ch.appl.psoft.Admin.Performancerating
{
    public partial class RatingLevel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORU_Layout.MainTitle = _map.get("performanceRating", "ratingLevels");
            LRORU_Layout.pageViewL.Controls.Add((RatingLevelDetailCtrl)LoadControl("Controls/RatingLevelDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((RatingLevelSearchCtrl)LoadControl("Controls/RatingLevelSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((RatingLevelSearchListCtrl)LoadControl("Controls/RatingLevelSearchListCtrl.ascx"));
        }
    }
}
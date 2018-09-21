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

namespace ch.appl.psoft.Admin.Performancerating.Controls
{
    public partial class JobExpectationDefaultDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            detailTitle.Text = _map.get("performanceRating", "expectation");
            titleLabel.Text = _map.get("performanceRating", "title");
            descriptionLabel.Text = _map.get("performanceRating", "description");
        }
    }
}
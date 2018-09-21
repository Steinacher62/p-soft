using ch.appl.psoft.Admin.Wage.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Wage
{
    public partial class WageVariant : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORU_Layout.MainTitle = _map.get("wage", "variant");
            LRORU_Layout.pageViewL.Controls.Add((VariantDetailCtrl)LoadControl("Controls/VariantDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((VariantSearchCtrl)LoadControl("Controls/VariantSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((VariantSearchListCtrl)LoadControl("Controls/VariantSearchListCtrl.ascx"));
        }
    }
}
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
    public partial class FunctionCriterias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORU_Layout.MainTitle = _map.get("performanceRating", "functionCriterias");
            LRORU_Layout.pageViewL.Controls.Add((FunctionCriteriaFunctionsTreeCtrl)LoadControl("Controls/FunctionCriteriaFunctionsTreeCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((FunctionCriteriaTypeCtrl)LoadControl("Controls/FunctionCriteriaTypeCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((FunctionCriteriaDetailCtrl)LoadControl("Controls/FunctionCriteriaDetailCtrl.ascx"));
        }
    }
}
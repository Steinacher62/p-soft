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
    public partial class FunctionRatingFunctionTyps : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LRORU_Layout.MainTitle = _map.get("functionRating", "functionTyp");
            LRORU_Layout.pageViewL.Controls.Add((FunctionTypDetailCtrl)LoadControl("Controls/FunctionTypDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((FunctionTypSeyrchCtrl)LoadControl("Controls/FunctionTypSeyrchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((FunctionTypSearchListCtrl)LoadControl("Controls/FunctionTypSearchListCtrl.ascx"));
           
        }
    }
}
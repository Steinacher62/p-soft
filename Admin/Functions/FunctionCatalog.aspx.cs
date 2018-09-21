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
    public partial class FunctionCatalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
                LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
                LMRORU_Layout.MainTitle = _map.get("function", "functions");
                LMRORU_Layout.pageViewL.Controls.Add((FunctionTreeCtrl)LoadControl("Controls/FunctionTreeCtrl.ascx"));
                LMRORU_Layout.pageViewRO.Controls.Add((FunctionSearchCtrl)LoadControl("Controls/FunctionSearchCtrl.ascx"));
                LMRORU_Layout.pageViewRU.Controls.Add((FunctionSearchListCtrl)LoadControl("Controls/FunctionSearchListCtrl.ascx"));
                LMRORU_Layout.pageViewM.Controls.Add((FunctionDetailCtrl)LoadControl("Controls/FunctionDetailCtrl.ascx"));
            if (!IsPostBack)
            {
                RadPageView GroupDetailPageView = new RadPageView();
                GroupDetailPageView.ID = "PageViewM1";
                GroupDetailPageView.EnableViewState = false;
                GroupDetailPageView.Controls.Add((FunctionGroupDetailCtrl)LoadControl("Controls/FunctionGroupDetailCtrl.ascx"));
                LMRORU_Layout.pageViewM.Parent.Controls.Add(GroupDetailPageView);
            }
        }
    }
}
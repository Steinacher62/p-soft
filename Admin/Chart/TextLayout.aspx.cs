using ch.appl.psoft.Admin.Chart.Controls;
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

namespace ch.appl.psoft.Admin.Chart
{
    public partial class TextLayout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
            LRORU_Layout.MainTitle = _map.get("chart", "textLayout");

            LRORU_Layout.pageViewL.Controls.Add((TextLayoutDetailCtrl)LoadControl("Controls/TextLayoutDetailCtrl.ascx"));
            LRORU_Layout.pageViewRO.Controls.Add((TextLayoutSearchCtrl)LoadControl("Controls/TextLayoutSearchCtrl.ascx"));
            LRORU_Layout.pageViewRU.Controls.Add((TextLayoutSearchListCtrl)LoadControl("Controls/TextLayoutSearchListCtrl.ascx"));
        }
    }
}
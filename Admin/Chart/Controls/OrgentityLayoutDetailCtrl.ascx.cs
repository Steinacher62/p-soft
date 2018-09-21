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

namespace ch.appl.psoft.Admin.Chart.Controls
{
    public partial class OrgentityLayoutDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            OrgentityLayoutTitle.Text = _map.get("CHARTNODELAYOUT", "ORGENTITY_LAYOUT_EDIT");
            NameTitle.Text = _map.get("CHARTNODELAYOUT", "TITLE");
            PictureTitle.Text = _map.get("CHARTNODELAYOUT", "IMAGE");
            WidthTitle.Text = _map.get("CHARTNODELAYOUT", "NODEWIDTH");
            HeightTitle.Text = _map.get("CHARTNODELAYOUT", "NODEHEIGHT");
            PaddingTopTitle.Text = _map.get("CHARTNODELAYOUT", "PADDING_TOP");
            PaddingLeftTitle.Text = _map.get("CHARTNODELAYOUT", "PADDING_LEFT");
            PaddingRightTitle.Text = _map.get("CHARTNODELAYOUT", "PADDING_RIGHT");
            LineWidthTitle.Text = _map.get("CHARTNODELAYOUT", "LINEWIDTH");
            LineColorTitle.Text = _map.get("CHARTNODELAYOUT", "LINECOLOR");
            BackgroundColorTitle.Text = _map.get("CHARTNODELAYOUT", "BACKGROUNDCOLOR");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin
{
    public partial class HelpVideoPlayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VideoPlayer.Source = "help/videos/" + Request.QueryString["video"];
            VideoPlayer.HDSource = "help/videos/" + Request.QueryString["video"];
            VideoPlayer.AutoPlay = true;
        }
    }
}
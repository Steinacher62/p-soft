using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Admin.Controls
{
    public partial class EmploymentDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            Employment.Text = _map.get("organisation", "employment");
            EmploymentTitle.Text = _map.get("EMPLOYMENT", "TITLE");
            EmploymentEngagementTitle.Text = _map.get("JOB", "ENGAGEMENT");
        }
    }
}
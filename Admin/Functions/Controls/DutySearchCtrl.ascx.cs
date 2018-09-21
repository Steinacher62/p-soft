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
namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class DutySearchCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LabelTitle.Text = _map.get("functionDescription", "duty");
            LabelName.Text = _map.get("functionDescription", "title");
            LabelDescriptionSearch.Text = _map.get("functionDescription", "description");
        }
    }
}
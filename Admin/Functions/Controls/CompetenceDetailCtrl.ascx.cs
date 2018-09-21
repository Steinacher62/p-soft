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
    public partial class CompetenceDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            detailTitle.Text = _map.get("competences", "competencLevel");
            numberLabel.Text = _map.get("competences", "level");
            titleLabel.Text = _map.get("competences", "label");
            titleShortLabel.Text = _map.get("competences", "labelShort");
            descriptionLabel.Text = _map.get("competences", "description");
        }
    }
}
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
    public partial class OrganisationDetailCtrl : System.Web.UI.UserControl
    {
        protected LanguageMapper _map = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _map = LanguageMapper.getLanguageMapper(Session);

            OrgentityTitle.Text = _map.get("organisation", "OE");
            DivisionTitle.Text = _map.get("organisation", "OE");
            MnemonicTitle.Text = _map.get("organisation", "oeMnemonic");
            DecriptionTitle.Text = _map.get("ORGENTITY", "DESCRIPTION");
            ClipboardTitle.Text = _map.get("global", "clipboard");
            ClipboardButton.Text = _map.get("global", "createClipboard");
        }
    }
}
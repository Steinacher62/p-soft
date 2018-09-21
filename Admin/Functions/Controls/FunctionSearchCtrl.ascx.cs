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
    public partial class FunctionSearchCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LabelTitle.Text = _map.get("function", "function");
            LabelName.Text = _map.get("FUNKTION", "TITLE");
            LabelNameShort.Text = _map.get("FUNKTION", "MNEMONIC");
        }
    }
}
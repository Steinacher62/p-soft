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
    public partial class ClipboardDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);

            ClipboardTitle.Text = _map.get("tableName", "FOLDER");
            ClipboardName.Text = _map.get("CLIPBOARD", "TITLE");
            ClipboardDecriptionTitle.Text = _map.get("CLIPBOARD", "DESCRIPTION");
            ClipboardCreatetTitle.Text = _map.get("FOLDER", "CREATED");
            ClipboardNumVersionsTitle.Text = _map.get("FOLDER", "NUMOFDOCVERSIONS");

            ClipboardNumVersionsData.DataSource = Adminutilities.GetIListFromXML(Session, "folder", "documentVersions", false);
            ClipboardNumVersionsData.DataValueField = "ID";
            ClipboardNumVersionsData.DataTextField = "ENTRY";
            ClipboardNumVersionsData.DataBind();
        }
    }
}
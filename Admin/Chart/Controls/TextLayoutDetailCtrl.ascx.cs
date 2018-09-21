using ch.appl.psoft.Admin.Chart.Controls;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Chart.Controls
{
    public partial class TextLayoutDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            TextLayoutTitle.Text = _map.get("CHARTTEXTLAYOUT", "EDITTEXTLAYOUT");
            NameTitle.Text = _map.get("CHARTTEXTLAYOUT", "TITLE");
            AlignmentTitle.Text = _map.get("CHARTTEXTLAYOUT", "HORIZONTAL_ALIGN");
            FontTitle.Text = _map.get("CHARTTEXTLAYOUT", "FONTFAMILY");
            FontsizeTitle.Text = _map.get("CHARTTEXTLAYOUT", "FONTSIZE");
            FontBoldTitle.Text = _map.get("CHARTTEXTLAYOUT", "BOLD");
            FontItalicTitle.Text = _map.get("CHARTTEXTLAYOUT", "ITALIC");
            FontColorTitle.Text = _map.get("CHARTTEXTLAYOUT", "FONTCOLOR");
            FontExampleTitle.Text = _map.get("CHARTTEXTLAYOUT", "PREVIEW");
            FontExampleData.Text = _map.get("CHARTTEXTLAYOUT", "EXAMPLETEXT");

            AlignmentData.DataSource = Adminutilities.GetIListFromXML(Session, "charttextlayout", "horizontalAlign", false);
            AlignmentData.DataValueField = "ID";
            AlignmentData.DataTextField = "ENTRY";
            AlignmentData.DataBind();

            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            FontData.DataSource = installedFontCollection.Families;
            //FontData.DataValueField = "Name";
            FontData.DataTextField = "Name";
            FontData.DataBind();

            FontsizeData.DataSource = Adminutilities.GetIListFromXML(Session, "charttextlayout", "fontsize", false);
            FontsizeData.DataValueField = "ENTRY";
            FontsizeData.DataTextField = "ENTRY";
            FontsizeData.DataBind();

            FontColorData.Columns = 18;
            FontColorData.Items.AddRange(FontColorData.GetDefaultColors());
            FontColorData.Items.AddRange(FontColorData.GetGrayscaleColors());




        }
    }
}
using ch.appl.psoft.Admin.Training.Controls;
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

namespace ch.appl.psoft.Admin.Training
{
    public partial class TrainingCatalog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            LMRORU_Layout.MainTitle = _map.get("training", "trainingCatalog");
            LMRORU_Layout.pageViewL.Controls.Add((CatalogTreeCtrl)LoadControl("Controls/CatalogTreeCtrl.ascx"));
            LMRORU_Layout.pageViewRO.Controls.Add((CatalogSearchCtrl)LoadControl("Controls/CatalogSearchCtrl.ascx"));
            LMRORU_Layout.pageViewRU.Controls.Add((CatalogSearchListCtrl)LoadControl("Controls/CatalogSearchListCtrl.ascx"));
            LMRORU_Layout.pageViewM.Controls.Add((CatalogDetailCtrl)LoadControl("Controls/CatalogDetailCtrl.ascx"));
        }
    }
}
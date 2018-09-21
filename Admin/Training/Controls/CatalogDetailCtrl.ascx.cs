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

namespace ch.appl.psoft.Admin.Training.Controls
{
    public partial class CatalogDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            TrainigDetailTitle.Text = _map.get("training", "training");
            NameTitle.Text = _map.get("training", "title");
            DesacriptionTitle.Text = _map.get("training", "description");
            ValidFromTitle.Text = _map.get("training", "validFrom");
            ValidToTitle.Text = _map.get("training", "validTo");
            CostExternTitle.Text = _map.get("training", "costExtern");
            CostInternTitle.Text = _map.get("training", "costIntern");
            PlaceTitle.Text = _map.get("training", "place");
            NumberParticipantTitle.Text = _map.get("training", "numParticipant");
            TrainerTite.Text = _map.get("training", "trainer");

            GroupTitletitle.Text = _map.get("training", "title");
            TrainingGroupTitle.Text = _map.get("training", "trainingGroup");
            GrouDescriptionTitle.Text = _map.get("training", "description");
        }
    }
}
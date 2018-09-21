using ch.appl.psoft.Common;
using ch.appl.psoft.Contact;
using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.LayoutControls;
using ch.appl.psoft.Performance.Controls;
using ch.appl.psoft.Person.Controls;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin.Functions.Controls
{
    public partial class FunctionDescriptionDutyDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper _map = LanguageMapper.getLanguageMapper(Session);
            detailTitle.Text = _map.get("functionDescription", "duty");
            numberLabel.Text = _map.get("functionDescription", "number");
            titleLabel.Text = _map.get("functionDescription", "title");
            descriptionLabel.Text = _map.get("functionDescription", "description");
            CompetenceTitle.Text = _map.get("functionDescription", "competences");
            Label FunctionTableTitleLabel=  FunctionDutyWindow.ContentContainer.FindControl("FunctionTableTitleLabel") as Label;
            FunctionTableTitleLabel.Text = _map.get("functionDescription", "functionsWithSelectedDuty");

            DBData db = DBData.getDBData(Session);
            db.connect();
            string lang = ((LanguageMapper)(base.Application["LangMap"])).LanguageCode;
             
            DataTable competences = db.getDataTable("SELECT ID, TITLE_" + lang + " AS TITLE FROM COMPETENCE_LEVEL ORDER BY NUMBER");
            int i = 0;
            foreach (DataRow competence  in competences.Rows)
            {
                HtmlGenericControl rowDiv = new HtmlGenericControl("DIV");
                rowDiv.Style.Add("display", "table-row");
                rowDiv.Attributes.Add("class", "dataRow");
                HtmlGenericControl cellTitle = new HtmlGenericControl("DIV");
                cellTitle.Style.Add("display", "table-cell");
                cellTitle.Attributes.Add("class", "titleLabelCell");
                HtmlGenericControl labelTitle = new HtmlGenericControl("LABEL");
                labelTitle.ID = "CompetenceName" + i.ToString();
                labelTitle.Attributes.Add("runat", "server");
                labelTitle.InnerText = competence["TITLE"].ToString();
                HtmlGenericControl cellData = new HtmlGenericControl("DIV");
                cellData.Style.Add("display", "table-cell");
                cellData.Attributes.Add("class", "dataLabelCell");
                RadCheckBox cbCompetence = new RadCheckBox();
                cbCompetence.ID = "CBCompetence" + i.ToString();
                cbCompetence.AutoPostBack = false;
                cbCompetence.Value = competence["ID"].ToString();
                rowDiv.Controls.Add(cellTitle);
                cellTitle.Controls.Add(labelTitle);
                rowDiv.Controls.Add(cellData);
                cellData.Controls.Add(cbCompetence);
                competenceTable.Controls.Add(rowDiv);

                i += 1;

            }

        }
    }
}
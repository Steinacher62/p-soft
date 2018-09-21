using ch.appl.psoft.Interface;
using System;

namespace ch.appl.psoft.Suggestion
{
    public partial class Legend : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LanguageMapper languagemapper = LanguageMapper.getLanguageMapper(Session);

            legend.HeaderRow.Cells[0].Text = languagemapper.get("SUGGESTION_STATI", "TITLE");
            legend.HeaderRow.Cells[1].Text = languagemapper.get("SUGGESTION_STATI", "COLOR");
            //pageTitle.Text = languagemapper.get("suggestion_execution", "legend");
        }
    }
}

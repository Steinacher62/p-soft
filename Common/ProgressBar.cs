using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for ProgressBar.
    /// </summary>
    [
        DefaultProperty("Text"), 
		ToolboxData("<{0}:ProgressBar runat=server></{0}:ProgressBar>")
    ]
	public class ProgressBar : WebControl
	{
        private int donePercentage;
        private int nrOfCells;
        private int cellSpacing;
	
        // Constructor
        //
        public ProgressBar() 
        {
            BackColor = System.Drawing.Color.LightGray;
            ForeColor = System.Drawing.Color.Blue;
            donePercentage = 0;
            nrOfCells = 20;
            cellSpacing = 0;
        }

        [
            Bindable(true), 
            Category("Appearance"), 
            DefaultValue("")
        ] 
        public int DonePercentage 
        {
            get
            {
                return donePercentage;
            }

            set
            {
                if (value > 100)
                    donePercentage = 100;
                else if (value < 0)
                    donePercentage = 0;
                else
                    donePercentage = value;
            }
        }

        [
            Bindable(true), 
            Category("Appearance"), 
            DefaultValue("")
        ] 
        public int NrOfCells 
        {
            get
            {
                return nrOfCells;
            }

            set
            {
                if (value > 1000)
                    nrOfCells = 1000;
                else if (value < 1)
                    nrOfCells = 1;
                else
                    nrOfCells = value;
            }
        }

        [
            Bindable(true), 
            Category("Appearance"), 
            DefaultValue("")
        ] 
        public int CellSpacing 
        {
            get
            {
                return cellSpacing;
            }

            set
            {
                cellSpacing = value;
            }
        }

        public string RGB(System.Drawing.Color color)
        {
            string r = color.R.ToString("x").PadRight(2,'0');
            string g = color.G.ToString("x").PadRight(2,'0');
            string b = color.B.ToString("x").PadRight(2,'0');
                
            return "#"+r+g+b; 
        }

        /// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
            // This block enables the control's percentage to be set dynamically from JavaScript
            output.WriteLine("<script language='javascript'>");
//            output.WriteLine("function pause(numberMillis)");
//            output.WriteLine("{");
//            output.WriteLine("    var dialogScript = 'window.setTimeout(' + ' function () { window.close(); }, ' + numberMillis + ');';");
//            output.WriteLine("    var result = window.showModalDialog('javascript:document.writeln(' + '\"<script>' + dialogScript + '<' + '/script>\")');");
//            output.WriteLine("}");

            output.WriteLine("function set" + ID + "(donePercent)");
            output.WriteLine("{");
            output.WriteLine("    var progressCells = document.getElementById('" + ClientID + "').getElementsByTagName('TD');");
            output.WriteLine("    var nrOfCells = 0;");
            output.WriteLine("    if (donePercent == 0)");
            output.WriteLine("        nrOfCells = 0;");
            output.WriteLine("    else if (donePercent >= 100)");
            output.WriteLine("        nrOfCells = progressCells.length;");
            output.WriteLine("    else");
            output.WriteLine("        nrOfCells = progressCells.length / 100 * donePercent;");
            output.WriteLine("    var i=1;");     
            output.WriteLine("    for (i=1; i<=progressCells.length; i++)");
            output.WriteLine("    {");
            output.WriteLine("        if (i <= nrOfCells)");
            output.WriteLine("            progressCells[i-1].bgColor = '" + RGB(ForeColor) + "';");
            output.WriteLine("        else");
            output.WriteLine("            progressCells[i-1].bgColor = '" + RGB(BackColor) + "';");
            output.WriteLine("    }");
//            output.WriteLine("    pause(0);");
            output.WriteLine("}");
            output.WriteLine("</script>");

            Table table = new Table();
            TableRow row = new TableRow();

            table.CellPadding = 0;
            table.CellSpacing = cellSpacing;
            table.BorderWidth = BorderWidth;
            table.BorderColor = BorderColor;
            table.Width = Width;
            table.Height = Height;
            table.TabIndex = TabIndex;
            table.Visible = Visible;
            table.ID = ID;
            table.Attributes.Add("style", Attributes["style"]);

            table.Rows.Add(row);

            int nrOfDoneCells = 0;
            if (donePercentage == 0)
                nrOfDoneCells = 0;
            else if (donePercentage >= 100)
                nrOfDoneCells = nrOfCells;
            else
                nrOfDoneCells = nrOfCells * donePercentage / 100;

            for (int i = 0; i < nrOfCells; i++ ) 
            {
                TableCell td = new TableCell();

                if (i < nrOfDoneCells)
                    td.Attributes.Add("bgColor", RGB(ForeColor));
                else
                    td.Attributes.Add("bgColor", RGB(BackColor));

                row.Cells.Add(td);
            }

            Controls.Add(table);
            table.RenderControl(output);
        }
	}
}

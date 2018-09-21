namespace ch.appl.psoft.Organisation.VisioExport
{
    public class VisioEmployee
    {
        public string Name;
        public VisioStyle Style;
        public VisioText Text;

        public VisioEmployee()
        {
            this.Style = new VisioStyle();
            this.Text = new VisioText();
        }
    }
}

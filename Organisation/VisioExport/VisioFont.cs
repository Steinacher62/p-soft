namespace ch.appl.psoft.Organisation.VisioExport
{
    public class VisioFont
    {
        public string Name;
        public int Size;
        public string Style;
        public string HorizontalAlign;
        public VisioColor Color;

        public VisioFont()
        {
            this.Color = new VisioColor();
        }
    }
}

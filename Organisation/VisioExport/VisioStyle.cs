namespace ch.appl.psoft.Organisation.VisioExport
{
    public class VisioStyle
    {
        public string Alignment;
        public VisioColor BackgroundColor;
        public VisioColor LineColor;
        public int LineWidth;

        public VisioStyle()
        {
            this.BackgroundColor = new VisioColor();
            this.LineColor = new VisioColor();
        }
    }
}

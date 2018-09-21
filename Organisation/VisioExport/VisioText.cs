namespace ch.appl.psoft.Organisation.VisioExport
{
    public class VisioText
    {
        public string Text;
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;
        public double HorizontalStartX;
        public double HorizontalStartY;
        public double HorizontalEndX;
        public double HorizontalEndY;
        public double VerticalStartX;
        public double VerticalStartY;
        public double VerticalEndX;
        public double VerticalEndY;


        public VisioStyle Style;
        public VisioFont Font;

        public VisioText()
        {
            this.Style = new VisioStyle();
            this.Font = new VisioFont();
        }
    }
}

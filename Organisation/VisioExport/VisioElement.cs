using System.Collections.Generic;

namespace ch.appl.psoft.Organisation.VisioExport
{
    public class VisioElement
    {
        public long Id;
        public string Title;
        public double X1;
        public double Y1;
        public double X2;
        public double Y2;
        public List<VisioEmployee> Employees;
        public VisioStyle Style;
        public List<VisioText> Text;
        public List<VisioLine> Line;
   

        public VisioElement()
        {
            this.Employees = new List<VisioEmployee>();
            this.Style = new VisioStyle();
            this.Text = new List<VisioText>();
            this.Line = new List<VisioLine>();
        }

        public VisioElement(string _title, double _X1, double _Y1, double _X2, double _Y2)
        {
            this.Title = _title;
            this.X1 = _X1;
            this.Y1 = _Y1;
            this.X2 = _X2;
            this.Y2 = _Y2;
            this.Employees = new List<VisioEmployee>();
            this.Style = new VisioStyle();
            this.Text = new List<VisioText>();
            this.Line = new List<VisioLine>();
        }
    }
}

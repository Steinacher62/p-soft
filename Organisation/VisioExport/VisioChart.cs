using ch.appl.psoft.db;
using System;
using System.Collections.Generic;

namespace ch.appl.psoft.Organisation.VisioExport
{
    public class VisioChart
    {
        public List<VisioElement> elements;
        public string orentation = "portrait";
        public string title;
        public double imgHeight;
        public double imgWight;
        private Random random = new Random(DateTime.Now.Millisecond);

        public VisioChart(DBData db, long chartId)
        {
            elements = new List<VisioElement>();
            
            //temp array for elements to get coordinates
            List<VisioElement> tempElements = new List<VisioElement>();

            db.connect();
            Chart chart = Chart.BuildChart(db, chartId, Global.Config.organisationImageDirectory);
            db.disconnect();

            //set organigramm title
            title = chart.Title;


            System.Drawing.Image img = chart.GetImageGraph(tempElements);
            //Auskommentiert MSr 2012.10.30
            //img.Save("c:\\temp\\organigramm.bmp");

            imgHeight = img.Height/37.8;
            imgWight = img.Width/37.8;
            if (img.Width > img.Height)
                orentation = "landscape";

            foreach (ChartNode node in chart._nodes)
            {
                listChildren(node, tempElements);
            }

        }

        private void listChildren(ChartNode node, List<VisioElement> tempElements)
        {
            VisioElement elem = new VisioElement();
            elem.Title = node.Orgentity.Title;
            elem.Id = node.Id;

            // get coordinates from temp array
            VisioElement searchElement = tempElements.Find(delegate(VisioElement e) { return e.Id == elem.Id; });

            elem.X1 = searchElement.X1;
            elem.X2 = searchElement.X2;
            elem.Y1 = searchElement.Y1;
            elem.Y2 = searchElement.Y2;

            elem.Style.Alignment = node.ChartAlignment.GraphAlignment.ToString();
            elem.Style.BackgroundColor.R = node.Layout.BackgroundColor.R;
            elem.Style.BackgroundColor.G = node.Layout.BackgroundColor.G;
            elem.Style.BackgroundColor.B = node.Layout.BackgroundColor.B;

            elem.Style.LineColor.R = node.Layout.LineColor.R;
            elem.Style.LineColor.G = node.Layout.LineColor.G;
            elem.Style.LineColor.B = node.Layout.LineColor.B;

            elem.Style.LineWidth = node.Layout.LineWidth;

            // get texts from array
            elem.Text = searchElement.Text;

            //ChartText text = node.GetText(0);
            //for (int idx = 0; idx < elem.Text.Count; idx++)
            //{
            //    elem.Text[idx].Font.Name = text.ChartTextLayout.Font.Name;
            //    elem.Text[idx].Font.Size = (int)text.ChartTextLayout.Font.Size;
            //    elem.Text[idx].Font.Style = text.ChartTextLayout.FontStyle.ToString();
            //    elem.Text[idx].Font.HorizontalAlign = text.ChartTextLayout.HorizontalAlign.ToString();
            //    elem.Text[idx].Font.Color.B = text.ChartTextLayout.FontColor.B;
            //    elem.Text[idx].Font.Color.G = text.ChartTextLayout.FontColor.G;
            //    elem.Text[idx].Font.Color.R = text.ChartTextLayout.FontColor.R;
            //}

            //get lines
            elem.Line = searchElement.Line;



            for (int idx = 0; idx < node.EmployeeCount; idx++)
            {
                ChartEmployee emp = node.GetEmployee(idx);
                
                VisioEmployee vEmp = new VisioEmployee();
                vEmp.Name = emp.Title;

                elem.Employees.Add(vEmp);
            }

            // get employees from arry
            elem.Employees = searchElement.Employees;

            // only add it if we don't have it already in the array
            VisioElement existingElement = elements.Find(delegate(VisioElement e) { return e.Id == elem.Id; });
            if (existingElement == null)
            {
                elements.Add(elem);
            }

            foreach (ChartNode child in node)
            {
                listChildren(child, tempElements);
            }
        }
    }
}

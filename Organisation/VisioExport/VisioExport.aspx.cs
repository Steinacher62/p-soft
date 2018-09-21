using ch.appl.psoft.db;
using System;
using Visio = Microsoft.Office.Interop.Visio;



namespace ch.appl.psoft.Organisation.VisioExport
{
    public partial class VisioExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBData db = DBData.getDBData(Session);
            long chartId = ch.psoft.Util.Validate.GetValid(Request.QueryString["id"], 0L);
            VisioChart vChart = new VisioChart(db, chartId);
            
            Visio.Application VisApp = new Visio.Application();
            Visio.Document Visdoc = VisApp.Documents.Add("");
            Visio.Shape VisPage = Visdoc.Pages[1].PageSheet;

            double minY = 100000;
            double maxY = 0;
            int zoomfactor = 100;
            double moveX;
            int moveY; 



            if (vChart.orentation == "portrait")
            {
                VisPage.get_CellsU("PageWidth").Formula = "210 mm";
                VisPage.get_CellsU("PageHeight").Formula = "297 mm";
                Visdoc.PrintLandscape = true;
                moveX = (210 - ((int)vChart.imgWight * 10)) * 2;
                moveY = 900;
            }
            else
            {
                VisPage.get_CellsU("PageWidth").Formula = "297 mm";
                VisPage.get_CellsU("PageHeight").Formula = "210 mm";
                Visdoc.PrintLandscape = false;
                moveX =  (297 - ((int)vChart.imgWight * 10))*2;
                moveY = 600;
            }

            
            // set organigramm title
            Visio.Shape textBox;
            if (vChart.orentation == "portrait")
            {
               textBox = VisPage.DrawRectangle(0, 11.1, 8.27, 10.6);
            }
            else
            {
               textBox = VisPage.DrawRectangle(0, 7.8, 11.69, 7.3);
            }
            textBox.TextStyle = "Normal";
            textBox.LineStyle = "Text Only";
            textBox.FillStyle = "Text Only";
            int fontIndex = VisApp.ActiveDocument.Fonts["Arial Black"].ID;
            textBox.get_CellsU("Char.Font").Formula = fontIndex.ToString();
            textBox.get_CellsU("Char.Size").Formula = "0.35";

            Visio.Characters textBoxText;
            textBoxText = textBox.Characters;
            textBoxText.Begin = 2;
            textBoxText.End = 4;
            textBox.Text = vChart.title;
            

            //Add departments 

            Visio.Shape[]organisation = new Visio.Shape[vChart.elements.Count];
            Visio.Cell[] connectionponts = new Visio.Cell[vChart.elements.Count * 6];
           
            int indexOe = 0;
            int indexConnectionPoint = 0;
            

            //get min and max Y position
            foreach (VisioElement oe in vChart.elements)
            {
                if (oe.Y1 > maxY) maxY = oe.Y1;
                if (oe.Y1 < minY) minY = oe.Y1;
            }

            foreach (VisioElement oe in vChart.elements)
            {
                //mirror organigramm
                oe.Y1 = newY(maxY, minY, oe.Y1, moveY);
                oe.Y2 = newY(maxY, minY, oe.Y2, moveY);
                //draw oe element
                organisation[indexOe] = VisPage.DrawRectangle((oe.X1 + moveX) / zoomfactor, oe.Y1 / zoomfactor, (oe.X2 + moveX) / zoomfactor , oe.Y2 / zoomfactor);

                //draw oe texst

                Visio.Shape[] oeLeader = new Visio.Shape[oe.Text.Count];
                Visio.Characters[] oeLeaderText = new Visio.Characters[oe.Text.Count];
                Visio.Cell[] oeLeaderCell = new Visio.Cell[oe.Text.Count];
                

                string[,] fontStyle = new string[,] { { "Bold", "17" }, { "Italic", "18" }, { "UnderLine", "19" }, { "SmallCaps", "20" } };

                int indexLeader = 0;
                foreach (VisioText textElement in oe.Text) 
                {
                    textElement.Y1 = newY(maxY, minY, textElement.Y1, moveY)+25;
                    textElement.Y2 = newY(maxY, minY, textElement.Y2, moveY)+25;


                    oeLeader[indexLeader] = VisPage.DrawRectangle((textElement.X1 + moveX) / zoomfactor, textElement.Y1 / zoomfactor, (textElement.X2 + moveX) / zoomfactor, textElement.Y2 / zoomfactor);
                    
                    //linecolor with
                    oeLeaderCell[indexLeader] = oeLeader[indexLeader].get_CellsSRC((short)Visio.VisSectionIndices.visSectionObject,
                                               (short)Visio.VisRowIndices.visRowLine,
                                               (short)Visio.VisCellIndices.visLinePattern);
                    oeLeaderCell[indexLeader].Formula = "0";
                    oeLeaderText[indexLeader] = oeLeader[indexLeader].Characters;
                    oeLeaderText[indexLeader].Text = textElement.Text;
                    oeLeaderText[indexLeader].Begin = 0;
                    oeLeaderText[indexLeader].End = textElement.Text.Length;
                    
                    if (textElement.Font.Style == ("Bold"))
                    {
                        oeLeaderText[indexLeader].set_CharProps(2, 17);
                    }
                    if (textElement.Font.Style ==("Italic"))
                    {
                        oeLeaderText[indexLeader].set_CharProps(2, 18);
                    }                    
                   oeLeaderCell[indexLeader] = oeLeader[indexLeader].get_CellsSRC((short)Visio.VisSectionIndices.visSectionCharacter,
                                                (short)Visio.VisRowIndices.visRowCharacter,
                                                (short)Visio.VisCellIndices.visCharacterColor);
                   oeLeaderCell[indexLeader].FormulaForceU = "RGB(" + textElement.Font.Color.R +", "+  textElement.Font.Color.G + ", " + textElement.Font.Color.B +")";
                   oeLeaderText[indexLeader].set_CharProps(7, (short)textElement.Font.Size);
                   indexLeader ++;
                }
                connectionponts[indexConnectionPoint] = AddConnectionPoint(organisation[indexOe], "bottom");
                indexConnectionPoint++;
                connectionponts[indexConnectionPoint] = AddConnectionPoint(organisation[indexOe], "bottomLeft");
                indexConnectionPoint++;
                connectionponts[indexConnectionPoint] = AddConnectionPoint(organisation[indexOe], "bottomReight");
                indexConnectionPoint++;
                connectionponts[indexConnectionPoint] = AddConnectionPoint(organisation[indexOe], "top");
                indexConnectionPoint++;
                connectionponts[indexConnectionPoint] = AddConnectionPoint(organisation[indexOe], "left");
                indexConnectionPoint++;
                connectionponts[indexConnectionPoint] = AddConnectionPoint(organisation[indexOe], "reight");
                indexConnectionPoint++;

                //Draw ConnectionLine

                int indexLine = 0;
                Visio.Shape[] lines = new Visio.Shape[oe.Line.Count];
                
                foreach (VisioLine Line in oe.Line)
                {

                    oe.Line[indexLine].Y1 = newY(maxY, minY, oe.Line[indexLine].Y1, moveY)/100+0.35 ;
                    oe.Line[indexLine].Y2 = newY(maxY, minY, oe.Line[indexLine].Y2, moveY)/100+0.35;

                    VisPage.DrawLine((oe.Line[indexLine].X1 + moveX) / zoomfactor, oe.Line[indexLine].Y1, (oe.Line[indexLine].X2 + moveX) / zoomfactor, oe.Line[indexLine].Y2);
                    indexLine++;
                }
 
               
            
                //Add Employee

                Visio.Shape[]employee = new Visio.Shape[oe.Employees.Count];
                Visio.Characters[] employeeText = new Visio.Characters[oe.Employees.Count];
                Visio.Cell[] employeeCell = new Visio.Cell[oe.Employees.Count];
                int indexEmployee = 0;
                Array.Resize(ref connectionponts, connectionponts.Length + oe.Employees.Count);
                foreach (VisioEmployee emp in oe.Employees) 
                {                   
                    emp.Text.Y1 = newY(maxY, minY, emp.Text.Y1, moveY);
                    emp.Text.Y2 = newY(maxY, minY, emp.Text.Y2, moveY);
                    employee[indexEmployee] = VisPage.DrawRectangle((emp.Text.X1 + moveX) / zoomfactor, emp.Text.Y1 / zoomfactor, (emp.Text.X2 + moveX)/ zoomfactor, emp.Text.Y2 / zoomfactor);
                    //linecolor with
                    employeeCell[indexEmployee] = employee[indexEmployee].get_CellsSRC((short)Visio.VisSectionIndices.visSectionObject,
                                               (short)Visio.VisRowIndices.visRowLine,
                                               (short)Visio.VisCellIndices.visLinePattern);
                    employeeCell[indexEmployee].Formula = "0";
                    
                    employeeText[indexEmployee] = employee[indexEmployee].Characters;
                    employeeText[indexEmployee].Text = emp.Text.Text;

                    employeeText[indexEmployee].Begin = 0;
                    employeeText[indexEmployee].End = emp.Text.Text.Length;
                    if (emp.Text.Font.Style == ("Bold"))
                    {
                        employeeText[indexEmployee].set_CharProps(2, 17);
                    }
                    if (emp.Text.Font.Style == ("Italic"))
                    {
                        employeeText[indexEmployee].set_CharProps(2, 18);
                    }
                    employeeCell[indexEmployee] = employee[indexEmployee].get_CellsSRC((short)Visio.VisSectionIndices.visSectionCharacter,
                                                 (short)Visio.VisRowIndices.visRowCharacter,
                                                 (short)Visio.VisCellIndices.visCharacterColor);
                    employeeCell[indexEmployee].FormulaForceU = "RGB(" + emp.Text.Font.Color.R + ", " + emp.Text.Font.Color.G + ", " + emp.Text.Font.Color.B + ")";
                    employeeCell[indexEmployee] = employee[indexEmployee].get_CellsSRC((short)Visio.VisSectionIndices.visSectionParagraph,
                                                 (short)Visio.VisRowIndices.visRowCharacter,
                                                 (short)Visio.VisCellIndices.visHorzAlign);
                    employeeCell[indexEmployee].FormulaForceU ="0";
                    employeeText[indexEmployee].set_CharProps(7, (short)emp.Text.Font.Size);

                    connectionponts[indexConnectionPoint] = AddConnectionPoint(employee[indexEmployee], "left");
                    indexConnectionPoint++;
                    
                    emp.Text.HorizontalStartY = newY(maxY, minY, emp.Text.HorizontalStartY, moveY)/zoomfactor;
                    emp.Text.HorizontalEndY = newY(maxY, minY, emp.Text.HorizontalEndY, moveY)/zoomfactor;
                    VisPage.DrawLine((emp.Text.HorizontalStartX + moveX)/zoomfactor, emp.Text.HorizontalStartY, (emp.Text.HorizontalEndX + moveX)/zoomfactor, emp.Text.HorizontalEndY);

                    emp.Text.VerticalStartY = newY(maxY, minY, emp.Text.VerticalStartY, moveY) / zoomfactor;
                    emp.Text.VerticalEndY = newY(maxY, minY, emp.Text.VerticalEndY, moveY) / zoomfactor;
                    VisPage.DrawLine((emp.Text.VerticalStartX + moveX) / zoomfactor, emp.Text.VerticalStartY, (emp.Text.VerticalEndX + moveX) / zoomfactor, emp.Text.VerticalEndY);


                    indexEmployee ++;
                }

                indexOe++;
            
            }
            //Visdoc.SaveAs("C:\\Temp\\export1.vsd");

            //save file
            string filename = Global.Config.documentTempDirectory +"\\"+ vChart.title + ".vsd";
            Visdoc.SaveAs(filename);
            VisApp.Quit();

            //send file to browser
            Response.Clear();
            Response.ClearHeaders();

            Response.AddHeader("content-type", "application/vnd.visio");
            Response.AddHeader("content-disposition", "attachment; filename=" + vChart.title + ".vsd");

            Response.WriteFile(filename);
            Response.Flush();
        }

        private Visio.Cell AddConnectionPoint(Visio.Shape shape, string orientation)
        {

            int newConnectionPointIndex;
            Visio.Row newConnectionPoint;

            newConnectionPointIndex = shape.AddRow((short)Visio.VisSectionIndices.visSectionConnectionPts, (short)Visio.VisRowIndices.visRowLast, (short)Visio.VisRowTags.visTagCnnctPt);
            //Get a reference to the new connection point.
            newConnectionPoint = shape.get_Section((short)Visio.VisSectionIndices.visSectionConnectionPts)[(short)newConnectionPointIndex];
            //newConnectionPoint = newConnectionPointIndex;

            ////Create the connection points.
            switch (orientation)
            {
                case "bottom":
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctX).FormulaU = "Width*0.5";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctY).FormulaU = "Height*0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirX).FormulaU = "0.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirY).FormulaU = "1.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctType).FormulaU = "0";
                    return shape.get_CellsU("Connections.X" + (newConnectionPointIndex + 1).ToString());
                case "bottomLeft":
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctX).FormulaU = "Width*0.2";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctY).FormulaU = "Height*0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirX).FormulaU = "0.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirY).FormulaU = "1.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctType).FormulaU = "0";
                    return shape.get_CellsU("Connections.X" + (newConnectionPointIndex + 1).ToString());
                case "bottomReight":
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctX).FormulaU = "Width*0.8";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctY).FormulaU = "Height*0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirX).FormulaU = "0.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirY).FormulaU = "1.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctType).FormulaU = "0";
                    return shape.get_CellsU("Connections.X" + (newConnectionPointIndex + 1).ToString());
                case "top":
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctX).FormulaU = "Width*0.5";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctY).FormulaU = "Height*1";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirX).FormulaU = "0.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirY).FormulaU = "1.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctType).FormulaU = "1";
                    return shape.get_CellsU("Connections.X" + (newConnectionPointIndex + 1).ToString());
                case "left":
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctX).FormulaU = "Width*0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctY).FormulaU = "Height*0.5";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirX).FormulaU = "1.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirY).FormulaU = "0.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctType).FormulaU = "0";
                    return shape.get_CellsU("Connections.Y" + (newConnectionPointIndex + 1).ToString());
                case "reight":
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctX).FormulaU = "Width*1";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctY).FormulaU = "Height*0.5";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirX).FormulaU = "1.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctDirY).FormulaU = "0.0";
                    newConnectionPoint.get_CellU(Visio.tagVisCellIndices.visCnnctType).FormulaU = "1";
                    return shape.get_CellsU("Connections.Y" + (newConnectionPointIndex + 1).ToString());
                default:
                    return shape.get_CellsU("Connections.X" + (newConnectionPointIndex + 1).ToString());

            }
            
        }
        private Visio.Cell AddTextField(Visio.Shape shape,string text)
        {
            
            Visio.Row newTextField;
       
            int newTextFieldIndex = shape.AddRow((short)Visio.VisSectionIndices.visSectionTextField, (short)Visio.VisRowIndices.visRowLast, (short)Visio.VisRowTags.visTagCnnctPt);
            //Get a reference to the new textfield.
            newTextField = shape.get_Section((short)Visio.VisSectionIndices.visSectionTextField)[(short)newTextFieldIndex];
            //newtextfield = newtextfieldindex;
            newTextField.get_CellU(Visio.tagVisCellIndices.vis1DBeginX).FormulaU = "1";
            return shape.get_CellsU("Text" + newTextFieldIndex.ToString());
        }

        private double newY(double maxY, double minY, double Y, int moveY)
        {
            //mirror and move organigramm up
            
           return  ((Y - ((maxY - minY) / 2)) * -1)+ moveY ;
        }

        
        
    }
}


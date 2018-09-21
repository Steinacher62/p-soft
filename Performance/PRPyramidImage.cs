using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Drawing;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Base class to draw performance rating pyramide image.
    /// </summary>
    public class PRPyramidImage {

        private Graphics _graph = null;
        private long _prId = 0;
        private DBData _db = null;
        private LanguageMapper _mapper = null;
        private int _w,_h;
        private static Pen _blackPen1 = new Pen(Color.Black,1);
        private static Pen _blackPen2 = new Pen(Color.Black,2);
        private static Pen _blackPen3 = new Pen(Color.Black,3);
        private static Pen _bluePen2 = new Pen(Color.Blue,2);
        private static Brush _greenBrush = new SolidBrush(Color.LightGreen);
        private Point _pT;  // Top  
        private Point _pL;  // Left
        private Point _pR;  // Right
        private Point _pH;  // Height base
        private double _tanT = 0.0; // tangens half top angle
        private Font _font1R = new Font("Arial",12,FontStyle.Regular);
        private static Brush _textBrush = new SolidBrush(Color.Black);
        //cst, 071115 - read right bottom text of pyramid from config
		//private string _rightBottomText = "100%";
		private string _rightBottomText = Global.Config.getModuleParam("performance", "performanceRatingBase", "100") + "%";
		private string _leftBottomText = "0%";
        private double _bottomBoxWidth = 0.4; 
        private double _leftSpace = 0.2; 
        private float _resolution = 192; // default


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="mapper">language mapper</param>
        /// <param name="w">image width in px</param>
        /// <param name="h">image height in px</param>
        public PRPyramidImage(DBData db, LanguageMapper mapper, int w, int h) {
            _db = db;
            _mapper = mapper;
            _w = w;
            _h = h;
        }

        /// <summary>
        /// Set font size (default 12)
        /// </summary>
        public int fontSize {
            set {_font1R = new Font("Arial",value,FontStyle.Regular);}
    }

        /// <summary>
        /// Set/Get resolution (default 192 dpi)
        /// </summary>
        public float resolution {
            set {_resolution = value;}
            get {return _resolution;}
        }

        /// <summary>
        /// Get/Set left/right pyramid bottom box in % (default 40%)
        /// </summary>
        public int bottomBoxWidth {
            get {return (int) _bottomBoxWidth*100; }
            set {_bottomBoxWidth = value/100;}
        }

        /// <summary>
        /// Set/Get left space from pyramid in % (default 20%)
        /// </summary>
        public int leftSpace {
            get {return (int) _leftSpace*100; }
            set {_leftSpace = ((double)value)/100;}
        }

        /// <summary>
        /// Set/Get pyramid right bottom text (default "0%")
        /// </summary>
        public string rightBottomText {
            get {return _rightBottomText; }
            set {_rightBottomText = value;}
        }

        /// <summary>
        /// Get/Set pyramid left bottom text (default "100%")
        /// </summary>
        public string leftBottomText {
            get {return _leftBottomText; }
            set {_leftBottomText = value;}
        }

        /// <summary>
        /// Draw image
        /// </summary>
        /// <param name="prId">Performance rating ID</param>
        /// <returns>image</returns>
        public Bitmap draw(long prId) {
            Bitmap map  = new Bitmap(_w,_h);

            _prId = prId;
            if (_resolution > 0) map.SetResolution(_resolution,_resolution);
            _graph = Graphics.FromImage(map);
            _graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _graph.Clear(Color.White);
            //Logger.Log("dp="+map.HorizontalResolution+"X"+map.VerticalResolution,Logger.DEBUG);

            drawPyramid();
            drawCriteria();

            _graph.Dispose();
            return map;
        }

        private void drawCriteria() {
            try {
                // calculations moved to seperate class "PerformanceCalc" / 20.04.10 / mkr
                PerformanceCalc calc = new PerformanceCalc(_db, _prId.ToString(), true);

                double scale = _pH.Y/calc.getSum();

                Point pL1 = new Point(_pL.X,_pT.Y+2);
                Point pL2 = new Point(_pT.X,_pT.Y+2);
                Point pB = _pT;
                int h = 0;
                string text = "";
                double tan50 = _tanT/calc.getAvgRelativWeight();
                double tempSum = 0;

                foreach (DataRow critRow in calc.getCritTable().Rows) {
                    DataRow row = calc.getCalcRow(critRow);
                    if (row == null)
                    {
                        continue;
                    }
                    double criteriaWeight = DBColumn.GetValid(row["CRITERIA"], 0.0);
                    if (criteriaWeight <= 0.0)
                    {
                        continue;
                    }
                    else
                    {
                        switch (critRow[0].ToString())
                        {
                            case "1234":
                                criteriaWeight = criteriaWeight * calc.getSkillsWeight() / 100;
                                break;
                            case "1235":
                                criteriaWeight = criteriaWeight * calc.getMboWeight() / 100;
                                break;
                            default:
                                criteriaWeight = criteriaWeight * calc.getJobExpectationWeight() / 100;
                                break;
                        }
                    }

                    // criteria-text and weight
                    bool showPyramidWeight = PerformanceModule.showPyramidWeight;
                    const int LINE_SPACE = 3;
                    _graph.DrawLine(_blackPen2,pL1,pL2);
                    tempSum += criteriaWeight;
                    h = (int) (criteriaWeight*scale);
                    text = row[calc.getItemTitleCol()].ToString();
                    if (showPyramidWeight)
                    {
                        text += " " + criteriaWeight.ToString("##.##") + " %";
                    }
                    SizeF size = _graph.MeasureString(text,_font1R);
                    int yOffsetText = Math.Max(0,(int) (h - size.Height - (showPyramidWeight? size.Height + LINE_SPACE : 0))/2);
                    _graph.DrawString(text,_font1R,_textBrush,new Point(0,pL1.Y+yOffsetText));
                    
                    switch (critRow[0].ToString())
                    {
                        case "1234":
                            if (!calc.getSkillsAvailable())
                            {
                                int yOffsetWeight = yOffsetText + (int)size.Height + LINE_SPACE;
                                string info = "(nicht bewertet)";
                                _graph.DrawString(info, _font1R, _textBrush, new Point(0, pL1.Y + yOffsetWeight));
                            }
                            break;
                        case "1235":
                            if (!calc.getMboAvailable())
                            {
                                int yOffsetWeight = yOffsetText + (int)size.Height + LINE_SPACE;
                                string info = "(nicht bewertet)";
                                _graph.DrawString(info, _font1R, _textBrush, new Point(0, pL1.Y + yOffsetWeight));
                            }
                            break;
                        default:
                            break;
                    }

                    pL1.Y = _pT.Y+((int) (tempSum*scale));
                    pL2.Y = pL1.Y;
                    pL2.X = _pT.X+((int) ((double)pL2.Y*_tanT));
                  
                    // performance
                    double a = (double)row["PERFORMANCE"];

                    switch (critRow[0].ToString())
                    {
                        case "1234":
                            if (calc.getSkillsAvailable())
                            {
                                a = (a - calc.getMinRating()) / calc.getSkillsFactor();
                            }
                            else
                            {
                                a = 50;
                            }
                            break;
                        case "1235":
                            if (calc.getMboAvailable())
                            {
                                a = a / calc.getMboFactor();
                            }
                            else
                            {
                                a = 50;
                            }
                            break;
                        default:
                            break;
                    }

                    a = (a-50.0)*tan50;
                    Point p = new Point(pB.X+((int)(h*a)),pL1.Y);
                    _graph.DrawLine(_bluePen2,pB,p);
                    pB = p;
                }
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }

        private void drawPyramid() {
            int top = 0;
            int left = (int) (_w*_leftSpace);
            int right = _w - 1;
            int middle = (_w+left)/2;
            int bottom = _h - 1;
            StringFormat format = new StringFormat();
            String text = "";
            int textH = (int) Math.Max(_graph.MeasureString(_mapper.get("performance","lessTasks"),_font1R).Height,_graph.MeasureString(_mapper.get("performance","moreTasks"),_font1R).Height);
            int bottomOffset = textH+8;

            bottom -= bottomOffset;
            bool showPyramidFooter = PerformanceModule.showPyramidFooter && (_leftBottomText != "" || _rightBottomText != "");
            if (showPyramidFooter) {
                bottom -= bottomOffset;
            }

            _pT = new Point(middle,top);    // Top
            _pL = new Point(left,bottom);   // Left
            _pR = new Point(right,bottom);  // Right
            _pH = new Point(middle,bottom); // Height
            _tanT = ((double)(_pR.X-_pH.X))/(double)_pH.Y;  // tangens half top angle
            // draw pyramide
            _graph.DrawLines(_blackPen3,new Point[] {_pT,_pL,_pR,_pT});

            bool showLine = PerformanceModule.showLine;
            if (showLine){
              _graph.DrawLine(_blackPen1,_pT,_pH);
            }
            //
            // bottom boxes
            Point p1 = new Point(left,bottom+bottomOffset-1);
            Point p2 = new Point(right,p1.Y);
            _graph.DrawLines(_blackPen2,new Point[] {_pL,p1,p2,_pR});
            Point p4 = new Point((int) (_pL.X+(_pR.X-_pL.X)*_bottomBoxWidth),_pL.Y);
            Point p5 = new Point(p4.X,p2.Y);
            _graph.DrawLine(_blackPen2,p4,p5);
            Point p6 = new Point((int) (_pR.X-(_pR.X-_pL.X)*_bottomBoxWidth),_pL.Y);
            Point p7 = new Point(p6.X,p5.Y);
            _graph.DrawLine(_blackPen2,p6,p7);
            _graph.FillRectangle(_greenBrush, p4.X+1, p4.Y+1, p6.X-p4.X-2, p5.Y-p4.Y-2);
            // boxes text
            text = _mapper.get("performance","lessTasks");
            int yOffset = Math.Max(0,(int) (((p1.Y-_pL.Y-textH)/2.0)+0.5));
            _graph.DrawString(text,_font1R,_textBrush,new Point(_pL.X,_pL.Y+yOffset));
            //
            text = _mapper.get("performance","moreTasks");
            format.Alignment = StringAlignment.Far;
            _graph.DrawString(text,_font1R,_textBrush,new Point(_pR.X,_pR.Y+yOffset),format);
            //
            // bottom text
            if (showPyramidFooter) {
                p1.Y += 3;
                _graph.DrawString(_leftBottomText,_font1R,_textBrush,p1);
                p2.Y += 3;
                _graph.DrawString(_rightBottomText,_font1R,_textBrush,p2,format);
            }
        }
    }
}

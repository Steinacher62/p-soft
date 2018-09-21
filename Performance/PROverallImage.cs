using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.Util;
using System;
using System.Data;
using System.Drawing;

namespace ch.appl.psoft.Performance
{
    /// <summary>
    /// Summary description for PROverallImage.
    /// </summary>
    public class PROverallImage {
        private Graphics _graph = null;
        private DBData _db = null;
        private LanguageMapper _mapper = null;
        private int _w,_h;
        private static Pen _blackPen1 = new Pen(Color.Black,1);
        private Font _font1R = new Font("Arial",5,FontStyle.Regular);
        private Font _font2B = new Font("Arial",6,FontStyle.Bold);
        private static Brush _textBrush = new SolidBrush(Color.Black);
        private int _maxBarWidth = 40;
        private int _barWidth = 20;
        private int _barSpace = 10;
        private float _resolution = 192; // default
        private long _mboId = 0;
		private long _prId = 0;
		private long _srId = 0;
		private int _numberOfBars = 0;
        private int _barNumber = 0;
        private Rectangle _barBox;
        private Rectangle _legendBox;
        private double _prRating = 0.0;
        private double _skillRating = 0.0;
        private double _mboRating = 0.0;
		// Gewichte für Gesamtbewertung
		private double _prWeight = 0.0;
		private double _skillWeight = 0.0;
		private double _mboWeight = 0.0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db">DB connection</param>
        /// <param name="mapper">language mapper</param>
        /// <param name="w">image width in px</param>
        /// <param name="h">image height in px</param>
        public PROverallImage(DBData db, LanguageMapper mapper, int w, int h) {
            _db = db;
            _mapper = mapper;
            _w = w;
            _h = h;
        }
        /// <summary>
        /// Set/Get resolution (default 192 dpi)
        /// </summary>
        public float resolution {
            set {_resolution = value;}
            get {return _resolution;}
        }

        /// <summary>
        /// Set font size (default 5)
        /// </summary>
        public int fontSize {
            set {_font1R = new Font("Arial",value,FontStyle.Regular);}
        }

        /// <summary>
        /// Set header font size (default 6)
        /// </summary>
        public int hederFontSize {
            set {_font2B = new Font("Arial",value,FontStyle.Bold);}
        }

        /// <summary>
        /// Draw image
        /// </summary>
        /// <param name="mboId">MBO rating id</param>
        /// <param name="prId">Performance rating id</param>
        /// <param name="srId">skill appraisal id</param>
		/// <param name="mboWeight">MBO-Gewicht für Gesamtbewertung</param>
		/// <param name="prWeight">LB-Gewicht für Gesamtbewertung</param>
		/// <param name="srWeight">Skill-Gewicht für Gesamtbewertung</param>
		/// <returns>image</returns>
        public Bitmap draw (
				long mboId,
				long prId,
				long srId,
				double mboWeight,
				double prWeight,
				double srWeight
			) 
		{
            Bitmap map  = new Bitmap(_w,_h);

            _mboId = mboId;
            _prId = prId;
            _srId = srId;
			// Gewicht 0, falls Bewertung nicht vorhanden (id == -1)
			_mboWeight = (_mboId == -1 ? 0.0 : mboWeight);
			_prWeight = (_prId == -1 ? 0.0 : prWeight);
			_skillWeight = (_srId == -1 ? 0.0 : srWeight);
			_barNumber = 0;
            if (_resolution > 0) map.SetResolution(_resolution,_resolution);
            _graph = Graphics.FromImage(map);
            _graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _graph.Clear(Color.White);
            //Logger.Log("dp="+map.HorizontalResolution+"X"+map.VerticalResolution,Logger.DEBUG);

            drawLayout();
            _numberOfBars = numberOfBars;
            _barWidth = (int) (_barBox.Width/(_numberOfBars*3+1));
            _barWidth *= 2;
            _barWidth = Math.Min(_barWidth,_maxBarWidth);
            _barSpace = _barWidth/2;
            drawMBO();
            drawSkill();
            drawPerformance();
            drawOverall();
            _graph.Dispose();
            return map;
        }

        private void drawLayout() {
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Far;
            SizeF size = _graph.MeasureString("100",_font1R);
            int bottom = (int) (_h*0.6);
            int xOffset = (int) size.Width;
            int yOffset = (int) (_graph.MeasureString("Abcdefg",_font2B).Height*4.0);
            Point pL = new Point(xOffset,yOffset);
            Size boxSz = new Size(_w-pL.X-1,bottom - pL.Y);
            //
            // Grid
            _barBox = new Rectangle(pL,boxSz);
            yOffset = _barBox.Height/20;
            int y = _barBox.Y-yOffset;
            for (int i=100; i>=0; i -= 5) {
                y += yOffset;
                _graph.DrawString(i.ToString(),_font1R,_textBrush,_barBox.X,y-size.Height/2,format);
                _graph.DrawLine(_blackPen1,_barBox.X,y,_w-2,y);
            }
            _barBox.Height = y-_barBox.Y;
            _barBox.X += 6;
            _graph.DrawLine(_blackPen1,_barBox.X,_barBox.Y,_barBox.X,_barBox.Bottom);
            _graph.DrawLine(_blackPen1,_w-1,_barBox.Y,_w-1,_barBox.Bottom);
            //
            // Legend
            boxSz = new Size((int) (_w*0.75),(int) ((_h-_barBox.Bottom)*0.8));
            Point pos = new Point((_w-boxSz.Width)/2,_h-boxSz.Height-5);
            _legendBox = new Rectangle(pos,boxSz);
            _graph.DrawRectangle(_blackPen1,_legendBox);            
        }
        private void drawOverall() {
            _db.connect();
            try {
                double total = _prWeight * _prRating + _skillWeight * _skillRating + _mboWeight * _mboRating;
                double sumWeight  = _prWeight + _skillWeight + _mboWeight;

				if (sumWeight > 0.0)
				{
                    total /= sumWeight;
                    int h = (int) (total+0.5);
                    Brush brush = new SolidBrush(Color.Salmon);
                    Brush brush1 = new SolidBrush(Color.LightSalmon);
                    int left = _w;
                    int right = 0;
                    StringFormat format = new StringFormat();

                    format.Alignment = StringAlignment.Center;
                    _barNumber++;
                    drawBar(_barNumber,h, brush,brush1,ref left, ref right);
                    drawLegend(_barNumber,h.ToString()+" "+_mapper.get("total"), brush);
                    //_graph.DrawString(_mapper.get("total"),_font2B,_textBrush,(left+right)/2,_barBox.Y/4,format);
                }
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }
        private void drawMBO() {
            if (_mboId <= 0) return;
            _db.connect();
            try {
                _mboRating = DBColumn.GetValid(_db.lookup("rating","objective_person_rating","id = "+_mboId),0);
                Brush brush = new SolidBrush(Color.Pink);
                Brush brush1 = new SolidBrush(Color.LightPink);
                int h = (int) (_mboRating+0.5);
                int left = _w;
                int right = 0;
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Far;
                format.FormatFlags = StringFormatFlags.DirectionVertical;
                _barNumber++;
                drawBar(_barNumber,h, brush,brush1,ref left, ref right);
                drawLegend(_barNumber,h.ToString()+" "+_mapper.get("mbo","repObjectiveRating"), brush);
                //_graph.DrawString(_mapper.get("mbo","repObjectiveRating"),_font2B,_textBrush,(left+right)/2,_barBox.Y/4,format);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }
        private void drawSkill() {
            if (_srId <= 0) return;
            _db.connect();
            try {
                _skillRating = DBColumn.GetValid(_db.lookup("avg(rating_level_percentage)","skill_rating","skills_appraisal_id = "+_srId),0);
                Brush brush = new SolidBrush(Color.Green);
                Brush brush1 = new SolidBrush(Color.LightGreen);
                int h = (int) (_skillRating+0.5);
                int left = _w;
                int right = 0;
                StringFormat format = new StringFormat();

                format.Alignment = StringAlignment.Far;
                format.FormatFlags =  StringFormatFlags.DirectionVertical;
                _barNumber++;
                drawBar(_barNumber,h, brush,brush1,ref left, ref right);
                drawLegend(_barNumber,h.ToString()+" "+_mapper.get("skills","repSkillsRating"), brush);
               // _graph.DrawString(_mapper.get("skills","repSkillsRating"),_font2B,_textBrush,(left+right)/2,_barBox.Y/4,format);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }
        private void drawPerformance() {
            if (_prId <= 0) return;
            string itemTitleCol = _db.langAttrName("PERFORMANCERATING_ITEMS","CRITERIA_TITLE");

            _db.connect();
            try {
                Brush brush = new SolidBrush(Color.SteelBlue);
                Brush brush1 = new SolidBrush(Color.LightBlue);
                StringFormat format = new StringFormat();
                double sum = 0.0;
                double sumCriteriaWeight = 0.0;
                int h = 0;
                int left = _w;
                int right = 0;

                format.Alignment = StringAlignment.Center;
                string sql = "select min(" + itemTitleCol + ") " + itemTitleCol + ","
					+ " avg(relativ_weight) PERFORMANCE,"
					+ " avg(criteria_weight) CRITERIA_WEIGHT"
					+ " from performancerating_items"
					+ " where relativ_weight >= 0 and criteria_weight > 0"
					+ " and performancerating_ref = " + _prId
					+ " group by criteria_ref_persisting";
                DataTable table = _db.getDataTable(sql);

				foreach (DataRow row in table.Rows) {
                    double cw = DBColumn.GetValid(row["CRITERIA_WEIGHT"], 0.0);

					if (cw <= 0.0){
                        continue;
                    }

                    _barNumber++;
                    double pw = DBColumn.GetValid(row["PERFORMANCE"],0.0);
                    sumCriteriaWeight += cw;
                    double r = pw * cw;
                    h = (int) (pw+0.5);
                    drawBar(_barNumber,h, brush,brush1, ref left, ref right);
                    drawLegend(_barNumber,h.ToString()+" "+_mapper.get("performance","reportAveragePerformance")+" "+row[itemTitleCol].ToString(),brush);
                    sum += r;
                }

                _barNumber++;
                brush = new SolidBrush(Color.Blue);

				if (sumCriteriaWeight == 0.0)
				{
					_prRating = 0.0;
				}
				else
				{
					_prRating = sum / sumCriteriaWeight;
				}

                h = (int) (_prRating+0.5);
                drawBar(_barNumber,h, brush,brush1,ref left, ref right);
                drawLegend(_barNumber,h.ToString()+" "+_mapper.get("performance","reportAveragePerformance")+" "+_mapper.get("performance","achievement"),brush);
                //_graph.DrawString(_mapper.get("performance","performanceRating"),_font2B,_textBrush,(left+right)/2,_barBox.Y/4,format);
            }
            catch (Exception e) {
                Logger.Log(e,Logger.ERROR);
            }
            finally {
                _db.disconnect();
            }
        }

        private void drawBar(int num, int h, Brush fillBar, Brush fillBox, ref int left, ref int right) {
            Size sz = new Size(_barWidth,(int) (_barBox.Height*h/100.0));
            Point pos = new Point(_barBox.X+((num*3)-1)*_barSpace,_barBox.Bottom-sz.Height);
            Point p1 = new Point(pos.X+_barSpace,_barBox.Y);
            Point p2 = new Point(p1.X,_barBox.Bottom);
            Rectangle box = new Rectangle(pos,sz);
            StringFormat format = new StringFormat();

            format.Alignment = StringAlignment.Center;

            left = Math.Min(left,p1.X);
            right = Math.Max(right,p1.X);
            _graph.DrawLine(_blackPen1,p1,p2);
            _graph.FillRectangle(fillBar,box);

            SizeF szText = _graph.MeasureString("100",_font1R);
            sz.Width = (int) (szText.Width*1.1+0.5);
            sz.Height = (int) (szText.Height*1.1+0.5);

            pos.X = p1.X-sz.Width/2;
            pos.Y -= (int) (szText.Height*2.0);
            box = new Rectangle(pos,sz);
            if ((pos.Y+sz.Height) < _barBox.Y) _graph.DrawLine(_blackPen1,new Point(p1.X,pos.Y+sz.Height),p1);
            _graph.FillRectangle(fillBox,box);
            _graph.DrawRectangle(_blackPen1,box);

            string text = h.ToString();
            pos.X = p1.X;
            pos.Y += (int) ((float)sz.Height-szText.Height+1.5)/2;
            _graph.DrawString(text,_font1R,_textBrush,pos,format);

            _graph.DrawString(num.ToString(),_font1R,_textBrush,p2,format);
        }

        private void drawLegend(int num, string text, Brush fill) {
            SizeF size = _graph.MeasureString(text,_font1R);
            double h = (double)_legendBox.Height/((_numberOfBars*3.0)+1);
            int y = (int) ((num-1)*3.0*h+h+0.5);
            Point pos = new Point(_legendBox.X+5,_legendBox.Y+y);
            Size sz = new Size(80,(int) (h*2.0));
            Rectangle box = new Rectangle(pos,sz);

            _graph.FillRectangle(fill,box);
            text = num.ToString()+": "+text;
            _graph.DrawString(text,_font1R,_textBrush,(float)box.Right,(float)(box.Y+(box.Height-size.Height)/2.0));
        }

        private int numberOfBars {
            get {
                int num = 0;
                _db.connect();
                try {
                    //performance rating
                    if (_prId > 0) {
                        num = DBColumn.GetValid(_db.lookup("count(distinct "+_db.langAttrName("PERFORMANCERATING_ITEMS","CRITERIA_TITLE")+")","performancerating_items","relativ_weight >= 0 and criteria_weight > 0 and performancerating_ref = "+_prId),0);
                        num++;
                    }
                    //skill rating
                    if (_srId > 0) num++;
                    //MBO rating
                    if (_mboId > 0) num++;

                    if (num > 0) num++;
                }
                catch (Exception e) {
                    Logger.Log(e,Logger.ERROR);
                }
                finally {
                    _db.disconnect();
                }

                return num;
            }

        }
    }
}

using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Web;

namespace ch.appl.psoft.Project
{
    public class BaseGraph {

        protected const int FONT_SIZE = 8;

 
        protected DBData _db = null;
        protected LanguageMapper _mapper = null;
        protected string _targetFrame = "_self";

        protected int _paddingTop = 3;
        protected int _paddingBottom = 3;
        protected int _paddingLeft = 3;
        protected int _paddingRight = 3;
        protected int _lineSpace = 2;

        // Brushes & Pens & ...
        public static Pen penBlack = new Pen(Brushes.Black);
        public static Pen penGoldenRod = new Pen(Brushes.Goldenrod);
        public static StringFormat stringFormatLeft = new StringFormat(StringFormatFlags.NoWrap);
        public static StringFormat stringFormatCenter = new StringFormat(StringFormatFlags.NoWrap);
        public static StringFormat stringFormatRight = new StringFormat(StringFormatFlags.NoWrap);
        public static StringFormat stringFormatMultiline = new StringFormat();
        public static Font fontRegular = new Font("arial, helvetica, sans-serif", FONT_SIZE, FontStyle.Regular);
        public static Font fontBold = new Font("arial, helvetica, sans-serif", FONT_SIZE, FontStyle.Bold);
        protected static int fontHeight = 0;

        protected StringBuilder _imageMapInfo = new StringBuilder(1024);

        static BaseGraph(){
            stringFormatLeft.Alignment = StringAlignment.Near;
            stringFormatCenter.Alignment = StringAlignment.Center;
            stringFormatRight.Alignment = StringAlignment.Far;
        }

        public BaseGraph(DBData db, LanguageMapper mapper, string targetFrame){
            _db = db;
            _mapper = mapper;
            _targetFrame = targetFrame;
        }

        protected string createAreaElement(string link, string target, string tooltip, int left, int top, int right, int bottom){
            StringBuilder areaBuilder = new StringBuilder(256);
            areaBuilder.Append("<area href=\"");
            areaBuilder.Append(link);
            areaBuilder.Append("\" target=\"");
            areaBuilder.Append(target);
            areaBuilder.Append("\" title=\"");
            areaBuilder.Append(HttpUtility.HtmlEncode(tooltip));
            areaBuilder.Append("\" shape=\"rect\" coords=\"");
            areaBuilder.Append(left);
            areaBuilder.Append(",");
            areaBuilder.Append(top);
            areaBuilder.Append(",");
            areaBuilder.Append(right);
            areaBuilder.Append(",");
            areaBuilder.Append(bottom);
            areaBuilder.Append("\">\n");
            return areaBuilder.ToString();
        }

        protected string createPolygonAreaElement(string link, string target, string tooltip, Point[] polyPoints)
        {
            StringBuilder areaBuilder = new StringBuilder(256);
            areaBuilder.Append("<area href=\"");
            areaBuilder.Append(link);
            areaBuilder.Append("\" target=\"");
            areaBuilder.Append(target);
            areaBuilder.Append("\" title=\"");
            areaBuilder.Append(HttpUtility.HtmlEncode(tooltip));
            areaBuilder.Append("\" shape=\"poly\" coords=\"");
            for (int j = 0; j < polyPoints.Length; j++)
            {
                Point pt = polyPoints[j];
                areaBuilder.Append(pt.X);
                areaBuilder.Append(",");
                areaBuilder.Append(pt.Y);
                if (j != polyPoints.Length - 1)
                {
                    areaBuilder.Append(",");
                }
            }
            areaBuilder.Append("\">\n");
            return areaBuilder.ToString();
        }

        /// <summary>
        /// For each phase add the image map info.
        /// </summary>
        /// <param name="imageMap"></param>
        public virtual void AppendImageMapInfo(StringBuilder imageMap) {
            imageMap.Append(_imageMapInfo);
        }

        public static void drawText(Graphics graphics, int x, int y, int width, int height, string text, Font font, StringFormat stringFormat){
            Size strSize =  graphics.MeasureString(text, font, width, stringFormat).ToSize();
            Rectangle rect = new Rectangle(x, y, width, Math.Min(height, strSize.Height + 1));
            graphics.DrawString(text, font, Brushes.Black, rect, stringFormat);
        }

        public static void drawSpacedVerticalLine(Graphics graphics, Pen pen, int x, int y1, int y2, int spaceLength, int lineLength){
            int currentY = y1;
            while (currentY < y2){
                graphics.DrawLine(pen, x, currentY, x, Math.Min(currentY+lineLength, y2));
                currentY += lineLength + spaceLength;
            }
        }
    }



    public class PhaseGraph : BaseGraph {

        private ScoreCard _scoreCard = null;
        private DataRow _row = null;
        public int _index = 0;
        private long _phaseID = -1L;
        private string _title = "";
        private int _achievedPercentage = 0;
        private DateTime _startDate = DateTime.MinValue;
        private DateTime _dueDate = DateTime.MaxValue;
        private int _criticalDays = 1;
        private Brush _fillBrush = Brushes.Gray;
        private int _leftX = -1;
        private int _rightX = -1;
        private int _startX = -1;
        private int _topY = -1;
        private int _endX = 0;
        private int _height = 0;
        private int _titleWidth = 100;

        private bool _hasMilestone = false;
        private string _milestoneDescription = "";
        Point[] _milestonePolygon;

        public bool IsTitle { get; set; }

        public int Level { get; set; }

        string _subprojectTitle;

        public PhaseGraph(ScoreCard scoreCard, DBData db, LanguageMapper mapper, DataRow phaseRow, string targetFrame, int criticalDays, int level) : base(db, mapper, targetFrame){
            _scoreCard = scoreCard;
            _row = phaseRow;
            _criticalDays = criticalDays;

            IsTitle = false;
            Level = level;
            load();
        }

        public PhaseGraph(ScoreCard scoreCard, DBData db, LanguageMapper mapper, string targetFrame, string subprojectTitle, int level)
            : base(db, mapper, targetFrame)
        {
            _scoreCard = scoreCard;
            IsTitle = true;
            Level = level;
            _subprojectTitle = subprojectTitle;
        }

        private void load(){
            _phaseID = DBColumn.GetValid(_row["ID"], _phaseID);
            _title = DBColumn.GetValid(_row["TITLE"], _title);
            _startDate = DBColumn.GetValid(_row["STARTDATE"], _startDate);
            _dueDate = DBColumn.GetValid(_row["DUEDATE"], _dueDate);
            _achievedPercentage = DBColumn.GetValid(_row["ACHIEVED"], _achievedPercentage);
            switch (_db.Phase.getSemaphore(_phaseID, _criticalDays)){
                case 0:
                    _fillBrush = Brushes.Red;
                    break;
                case 1:
                    _fillBrush = Brushes.Orange;
                    break;
                case 2:
                    _fillBrush = Brushes.LightGreen;
                    break;
                case 3:
                    _fillBrush = Brushes.LightGray;
                    break;
            }

            int hasMilestone = DBColumn.GetValid(_row["HAS_MILESTONE"], 0);
            _hasMilestone = hasMilestone == 0 ? false : true;
            if (_hasMilestone)
            {
                _milestoneDescription = DBColumn.GetValid(_row["MILESTONE_DESCRIPTION"], "");
            }
        }

        public void arrange(int leftX, int width, int titleWidth, int topY, int height){
            _height = height;
            _topY = topY;
            _leftX = leftX;
            _rightX = _leftX + width;
            _titleWidth = titleWidth;

            _startX = _scoreCard.xByDateTime(_startDate);
            _endX = _scoreCard.xByDateTime(_dueDate);
        }

        public void draw(Graphics graphics){

            if (IsTitle)
            {
                //this is the subproject title
                int textTopY = _topY + (_height - fontHeight) / 2;
                if (Level > 0)
                {
                    String s = new String(' ', Level);
                    this._subprojectTitle = s + this._subprojectTitle;
                }
                drawText(graphics, _leftX + _paddingLeft, textTopY, _titleWidth - _paddingLeft - _paddingRight, fontHeight, this._subprojectTitle, fontBold, stringFormatLeft);
            }
            else
            {
                // draw title...
                if (Level > 0)
                {
                    String s = new String(' ', Level);
                    this._title = s + this._title;
                }

                int textTopY = _topY + (_height - fontHeight) / 2;
                drawText(graphics, _leftX + _paddingLeft, textTopY, _titleWidth - _paddingLeft - _paddingRight, fontHeight, _title, fontRegular, stringFormatLeft);

                // draw bar...
                Rectangle rect = new Rectangle(_startX, _topY + _paddingTop, _endX - _startX, _height - _paddingTop - _paddingBottom);
                graphics.FillRectangle(_fillBrush, rect);

                // draw milestone...
                if (_hasMilestone && (_endX - _startX) > 0)
                {
                    Brush milestoneBrush = Brushes.DarkRed;
                    _milestonePolygon = new Point[3];
                    int halfWidth = rect.Size.Height / 2;
                    int milestoneX = Math.Abs(rect.Right - halfWidth);
                    _milestonePolygon[0] = new Point(milestoneX + halfWidth, rect.Y);
                    _milestonePolygon[1] = new Point(milestoneX, rect.Y + rect.Size.Height);
                    _milestonePolygon[2] = new Point(milestoneX + halfWidth * 2, rect.Y + rect.Size.Height);
                    graphics.FillPolygon(milestoneBrush, _milestonePolygon);
                    //graphics.DrawPolygon(Pens.WhiteSmoke,_milestonePolygon);
                    //write date text 
                    string milestoneText = _dueDate.ToString("dd.MM.yyyy");
                    graphics.DrawString(milestoneText, new Font("Times New Roman", FONT_SIZE, FontStyle.Regular), Brushes.Black, new Point(rect.Right + halfWidth, rect.Y));

                    // set the image-map for milestones... 
                    _imageMapInfo.Append(createPolygonAreaElement(psoft.Project.PhaseDetail.GetURL("ID", _phaseID), "_self", _milestoneDescription, _milestonePolygon));
                }

                // draw percentage-label...
                string percentage = _achievedPercentage.ToString() + "%";
                int width = graphics.MeasureString(percentage, fontRegular, 300, stringFormatLeft).ToSize().Width + 1;
                int percentageX = Math.Min(_rightX - width, Math.Max(_leftX + _titleWidth, (_startX + _endX) / 2 - width / 2));
                drawText(graphics, percentageX, textTopY, width, fontHeight, percentage, fontRegular, stringFormatLeft);

                // set the image-map...
                _imageMapInfo.Append(createAreaElement(psoft.Project.PhaseDetail.GetURL("ID", _phaseID), "_self", _title, _leftX, _topY + _paddingTop, _rightX, _topY + _height - _paddingBottom));
            }
        }
    }

    public class ScoreCard : BaseGraph {

 
        private long _projectID = -1L;
        private string _title = "";
        private DateTime _startDate = DateTime.MinValue;
        private DateTime _dueDate = DateTime.MaxValue;
        private DateTime _updateDate = DateTime.Now;
        private string _problems = "";
        private string _comments = "";
        private int _criticalDays = 1;
        private double _costInternalNominal = 0.0;
        private double _costInternalActual = 0.0;
        private double _costExternalNominal = 0.0;
        private double _costExternalActual = 0.0;
        private string _costUnit = "";
        private int _nrOfPhases = 0;
        private int _width = 1000;
        private int _height = 1000;
        private int _titleHeight = 50;
        private int _phasesTitleHeight = 35;
        private int _phaseTitleWidth = 200;
        private int _phaseHeight = 25;
        private int _costLabelWidth = 70;
        private int _costValueWidth = 80;
        private int _commentsHeight = 60;
        private int _maxNrOfMonthsBefore = 2;
        private int _maxNrOfMonthsAfter = 10;
        
        private const int WHOLE_CONTENT_MONTHS_BEFORE = 1;
        private const int WHOLE_CONTENT_MONTHS_AFTER = 3;
        private const int NOT_SCHRINKED_PIXEL_PER_TIMEUNIT = 4;

        private DateTime _graphStartDate = DateTime.Now.AddMonths(-2);
        private DateTime _graphEndDate = DateTime.Now.AddMonths(10);
        private TimeSpan _graphSpan = TimeSpan.MinValue;
        private ArrayList _phaseGraphs = new ArrayList();

        public string Title{
            get{return _title;}
        }


  
        public ScoreCard(DBData db, LanguageMapper mapper, long projectID, string targetFrame) 
            : this(db, mapper, projectID, targetFrame, true, false) 
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="mapper"></param>
        /// <param name="projectID"></param>
        /// <param name="targetFrame"></param>
        /// <param name="wholeContent">if true start and end date are always contained in the graph image</param>
        /// <param name="shrinked">if true image is shrinked</param>
        public ScoreCard(DBData db, LanguageMapper mapper, long projectID, string targetFrame, bool wholeContent, bool shrinked)
            : base(db, mapper, targetFrame)
        {
            _projectID = projectID;

            //load data from database
            load();

            //set depending on the type 
            if (wholeContent)
            {
                try
                {
                    if ( _startDate > (DateTime.MinValue.AddMonths(WHOLE_CONTENT_MONTHS_BEFORE)) ) {
                        _graphStartDate = _startDate.AddMonths(-WHOLE_CONTENT_MONTHS_BEFORE);
                    } else {
                        _graphStartDate = DateTime.Now.AddMonths(-_maxNrOfMonthsBefore);
                    }
                    if ( _dueDate < (DateTime.MaxValue.AddMonths(-WHOLE_CONTENT_MONTHS_BEFORE)) )
                    {
                        _graphEndDate = _dueDate.AddMonths(WHOLE_CONTENT_MONTHS_AFTER);
                    }
                    else
                    {
                        _graphEndDate = DateTime.Now.AddMonths(_maxNrOfMonthsAfter);
                    }
                }
                catch(ArgumentOutOfRangeException outofrange)
                {
                    _graphStartDate = DateTime.Now.AddMonths(-_maxNrOfMonthsBefore);
                    _graphEndDate = DateTime.Now.AddMonths(_maxNrOfMonthsAfter);
                }
            }
            else
            {
                // determine graph start & end-date...
                if (_startDate.AddMonths(_maxNrOfMonthsBefore + _maxNrOfMonthsAfter) > _dueDate)
                {
                    // short -> show entire
                    _graphStartDate = _startDate;
                    _graphEndDate = _dueDate;
                }
                else if (_updateDate > _dueDate.AddMonths(-_maxNrOfMonthsAfter))
                {
                    // near end or over -> show end
                    _graphEndDate = _dueDate;
                    _graphStartDate = _dueDate.AddMonths(-(_maxNrOfMonthsBefore + _maxNrOfMonthsAfter));
                }
                else if (_startDate.AddMonths(_maxNrOfMonthsBefore) > _updateDate)
                {
                    // near beginning or not yet started -> show start
                    _graphStartDate = _startDate;
                    _graphEndDate = _startDate.AddMonths(_maxNrOfMonthsBefore + _maxNrOfMonthsAfter);
                }
                else
                {
                    _graphStartDate = _updateDate.AddMonths(-_maxNrOfMonthsBefore);
                    _graphEndDate = _updateDate.AddMonths(_maxNrOfMonthsAfter);
                }
            }
            _graphSpan = _graphEndDate - _graphStartDate;

            if (!shrinked)
            {
                 int tmpWidth = (int)(_graphSpan.Duration().TotalDays * NOT_SCHRINKED_PIXEL_PER_TIMEUNIT);
                 _width = (_width > tmpWidth)? _width : tmpWidth;
            }

        }

        private void load(){
            // loading project-details...
            DataTable table = _db.getDataTableExt("select * from PROJECT where ID=" + _projectID, "PROJECT");
            if (table.Rows.Count > 0 && _db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, 0, true, true)){
                DataRow row = table.Rows[0];
                _title = DBColumn.GetValid(row["TITLE"], _title);
                _criticalDays = DBColumn.GetValid(row["CRITICALDAYS"], _criticalDays);
                _startDate = DBColumn.GetValid(row["STARTDATE"], _startDate);
                _dueDate = DBColumn.GetValid(row["DUEDATE"], _dueDate);
                _problems = DBColumn.GetValid(row["SPEC_PROBLEM"], _problems);
                _comments = DBColumn.GetValid(row["SPEC_COMMENT"], _comments);
                _updateDate = DBColumn.GetValid(row["SPEC_MODIFY_DATE"], _updateDate);
                _costInternalNominal = DBColumn.GetValid(row["COST_INTERNAL_NOMINAL"], _costInternalNominal);
                _costInternalActual = DBColumn.GetValid(row["COST_INTERNAL_ACTUAL"], _costInternalActual);
                _costExternalNominal = DBColumn.GetValid(row["COST_EXTERNAL_NOMINAL"], _costExternalNominal);
                _costExternalActual = DBColumn.GetValid(row["COST_EXTERNAL_ACTUAL"], _costExternalActual);
                _costUnit = table.Columns["COST_INTERNAL_NOMINAL"].ExtendedProperties["Unit"].ToString();
                if (_costUnit != ""){
                    _costUnit = " " + _costUnit;
                }

                // loading phases...
                loadPhases(this._projectID, 0);
                // load subprojects...
                loadSubProjectPhases(this._projectID, 0);
            }

            
        }

        private void loadPhases(long projectId, int level)
        {
            DataTable table = _db.getDataTableExt("select * from PHASE where PROJECT_ID=" + projectId + " order by DUEDATE asc", "PHASE");
            foreach (DataRow phaseRow in table.Rows)
            {
                if (_db.hasRowAuthorisation(DBData.AUTHORISATION.READ, table, phaseRow, true, true))
                {
                    _phaseGraphs.Add(new PhaseGraph(this, _db, _mapper, phaseRow, _targetFrame, _criticalDays, level));
                }
            }
        }

        private void loadSubProjectPhases(long projectID, int level)
        {
            level++;
            string sql = "select ID, TITLE from project where parent_id = " + projectID;
            DataTable table = _db.getDataTableExt(sql, "PROJECT");
            foreach (DataRow subProject in table.Rows)
            {
                //project title
                _phaseGraphs.Add(new PhaseGraph(this, _db, _mapper, _targetFrame, (string)subProject[1], level));

                //load phase
                loadPhases((long)subProject[0], level);
                loadSubProjectPhases((long)subProject[0], level); //recursive
            }

          

        }
        

        public int xByDateTime(DateTime dateTime){
            TimeSpan span = dateTime - _graphStartDate;
            return Math.Min(_width, Math.Max(_phaseTitleWidth, _phaseTitleWidth + (int)((_width - _phaseTitleWidth) / _graphSpan.TotalHours * span.TotalHours)));
        }

        private void drawMonthString(Graphics graphics, DateTime dateTime, int prevX, int x, int y){
            string monthString = dateTime.ToString("MMM yy", _db.dbColumn.UserCulture.DateTimeFormat);
            int width =  graphics.MeasureString(monthString, fontRegular, 300, stringFormatLeft).ToSize().Width + 1;
            if (width < x-prevX){
                drawText(graphics, prevX + (x-prevX-width)/2, y, width, fontHeight, monthString, fontRegular, stringFormatLeft); 
            }
        }

        private void drawVerticalLine(Graphics graphics, DateTime dateTime, Pen pen){
            if (_graphStartDate < dateTime && _graphEndDate > dateTime){
                int updateX = xByDateTime(dateTime);
                drawSpacedVerticalLine(graphics, pen, updateX, _titleHeight + fontHeight, _height - _commentsHeight - _paddingBottom, 3, 4);
                graphics.DrawLine(penGoldenRod, updateX - 3, _titleHeight + fontHeight - 3, updateX, _titleHeight + fontHeight - 6);
                graphics.DrawLine(penGoldenRod, updateX + 3, _titleHeight + fontHeight - 3, updateX, _titleHeight + fontHeight - 6);
                graphics.DrawLine(penGoldenRod, updateX - 3, _titleHeight + fontHeight - 3, updateX, _titleHeight + fontHeight);
                graphics.DrawLine(penGoldenRod, updateX + 3, _titleHeight + fontHeight - 3, updateX, _titleHeight + fontHeight);
            }
        }

        private void draw(Graphics graphics){

            // estimate heights...
            fontHeight = graphics.MeasureString("blabla", fontRegular, 100, stringFormatLeft).ToSize().Height + 1;
            int problemsWidth = _width - _costLabelWidth - 3 * _costValueWidth - _paddingLeft - _paddingRight;
            _titleHeight = Math.Max(_paddingTop + 3 * fontHeight + 2 * _lineSpace + _paddingBottom, _paddingTop + fontHeight + _lineSpace + _paddingBottom + graphics.MeasureString(_problems, fontRegular, problemsWidth, stringFormatMultiline).ToSize().Height + 1);
            int commentsWidth = _width - _paddingLeft - _paddingRight;
            _commentsHeight = _paddingTop + fontHeight + _lineSpace + _paddingBottom + graphics.MeasureString(_comments, fontRegular, commentsWidth, stringFormatMultiline).ToSize().Height + 1;


            // arrange phases...
            _nrOfPhases = 0;
            foreach (PhaseGraph phaseGraph in _phaseGraphs){
                phaseGraph._index = _nrOfPhases++;
                phaseGraph.arrange(0, _width, _phaseTitleWidth, _titleHeight + _phasesTitleHeight + (phaseGraph._index * _phaseHeight), _phaseHeight);
            }

            _height = _titleHeight + _phasesTitleHeight + (_nrOfPhases * _phaseHeight) + _commentsHeight + _paddingBottom;

            // draw outline...
            graphics.DrawRectangle(penBlack, 0, 0, _width, _height);
            graphics.DrawLine(penBlack, _costLabelWidth + 3*_costValueWidth, 0, _costLabelWidth + 3*_costValueWidth, _titleHeight);
            graphics.DrawLine(penBlack, 0, _titleHeight, _width, _titleHeight);
            graphics.DrawLine(penBlack, 0, _height - _commentsHeight, _width, _height - _commentsHeight);

            // draw costs...
            drawText(graphics, _paddingLeft, _paddingTop, _costLabelWidth - _paddingLeft - _paddingRight, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_COST), fontBold, stringFormatLeft);
            drawText(graphics, _costLabelWidth + _paddingLeft, _paddingTop, _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_COST_NOMINAL), fontBold, stringFormatCenter);
            drawText(graphics, _costLabelWidth + _costValueWidth + _paddingLeft, _paddingTop, _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_COST_ACTUAL), fontBold, stringFormatCenter);
            drawText(graphics, _costLabelWidth + 2*_costValueWidth + _paddingLeft, _paddingTop, _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_COST_DIFFERENCE), fontBold, stringFormatCenter);

            drawText(graphics, _paddingLeft, _paddingTop + _lineSpace + fontHeight, _costLabelWidth - _paddingLeft - _paddingRight, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_COST_INTERNAL), fontRegular, stringFormatLeft);
            drawText(graphics, _costLabelWidth + _paddingLeft, _paddingTop + _lineSpace + fontHeight, _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _costInternalNominal.ToString("0.00") + _costUnit, fontRegular, stringFormatRight);
            drawText(graphics, _costLabelWidth + _costValueWidth + _paddingLeft, _paddingTop + _lineSpace + fontHeight, _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _costInternalActual.ToString("0.00") + _costUnit, fontRegular, stringFormatRight);
            if (_costInternalNominal != 0.0){
                double costInternalDifference = (_costInternalActual/_costInternalNominal - 1)*100;
                drawText(graphics, _costLabelWidth + 2*_costValueWidth + _paddingLeft, _paddingTop + _lineSpace + fontHeight, _costValueWidth - _paddingLeft - _paddingRight, fontHeight, costInternalDifference.ToString("0.0") + " %", fontRegular, stringFormatRight);
            }

            drawText(graphics, _paddingLeft, _paddingTop + 2*(_lineSpace + fontHeight), _costLabelWidth - _paddingLeft - _paddingRight, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_COST_EXTERNAL), fontRegular, stringFormatLeft);
            drawText(graphics, _costLabelWidth + _paddingLeft, _paddingTop + 2*(_lineSpace + fontHeight), _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _costExternalNominal.ToString("0.00") + _costUnit, fontRegular, stringFormatRight);
            drawText(graphics, _costLabelWidth + _costValueWidth + _paddingLeft, _paddingTop + 2*(_lineSpace + fontHeight), _costValueWidth - _paddingLeft - _paddingRight, fontHeight, _costExternalActual.ToString("0.00") + _costUnit, fontRegular, stringFormatRight);
            if (_costExternalNominal != 0.0){
                double costExternalDifference = (_costExternalActual/_costExternalNominal - 1)*100;
                drawText(graphics, _costLabelWidth + 2*_costValueWidth + _paddingLeft, _paddingTop + 2*(_lineSpace + fontHeight), _costValueWidth - _paddingLeft - _paddingRight, fontHeight, costExternalDifference.ToString("0.0") + " %", fontRegular, stringFormatRight);
            }

            // draw problems...
            drawText(graphics, _costLabelWidth + 3*_costValueWidth + _paddingLeft, _paddingTop, problemsWidth, fontHeight, _mapper.get("PROJECT", "SPEC_PROBLEM"), fontBold, stringFormatLeft);
            drawText(graphics, _costLabelWidth + 3*_costValueWidth + _paddingLeft, _paddingTop + fontHeight + _lineSpace, problemsWidth, _height - _paddingTop - _paddingBottom - fontHeight - _lineSpace, _problems, fontRegular, stringFormatMultiline);

            // draw comments...
            drawText(graphics, _paddingLeft, _height - _commentsHeight + _paddingTop, commentsWidth, fontHeight, _mapper.get("PROJECT", "SPEC_COMMENT"), fontBold, stringFormatLeft);
            drawText(graphics, _paddingLeft, _height - _commentsHeight + _paddingTop + fontHeight+ _lineSpace, commentsWidth, _commentsHeight - _paddingTop - _paddingBottom - fontHeight - _lineSpace, _comments, fontRegular, stringFormatMultiline);

            // draw today-line...
            drawVerticalLine(graphics, DateTime.Now, new Pen(Brushes.DarkGray));

            // draw update-line...
            drawVerticalLine(graphics, _updateDate, penGoldenRod);

            // draw months...
            int monthsWidth = _phaseTitleWidth - _paddingLeft - _paddingRight;
            drawText(graphics, _paddingLeft, _titleHeight + _paddingTop, monthsWidth, fontHeight, _mapper.get(ProjectModule.LANG_SCOPE_PROJECT, ProjectModule.LANG_MNEMO_PROJECT_STATE), fontBold, stringFormatLeft);
            DateTime endOfMonth = new DateTime(_graphStartDate.Year, _graphStartDate.Month, 1);
            int prevX = _phaseTitleWidth;
            int monthY = _titleHeight + _paddingTop + fontHeight + _lineSpace;
            endOfMonth = endOfMonth.AddMonths(1);
            while (endOfMonth < _graphEndDate){
                int x = xByDateTime(endOfMonth);
                graphics.DrawLine(penBlack, x, monthY, x, _height - _commentsHeight - _paddingBottom);

                drawMonthString(graphics, endOfMonth.AddMonths(-1), prevX, x, monthY);
                prevX = x;
                endOfMonth = endOfMonth.AddMonths(1);
            }
            drawMonthString(graphics, endOfMonth.AddMonths(-1), prevX, _width, monthY);

            // draw phases...
            foreach (PhaseGraph phaseGraph in _phaseGraphs){
                phaseGraph.draw(graphics);
            }
        }

        public System.Drawing.Image GetImage(){
            Bitmap bitmap = null;

            bitmap = new Bitmap(_width+1, _height+1);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            draw(graphics);

            return bitmap.Clone(new Rectangle(0,0,_width+1, _height+1), PixelFormat.Undefined);
        }

        public override void AppendImageMapInfo(StringBuilder imageMap) {
            base.AppendImageMapInfo(imageMap);
            foreach (PhaseGraph phaseGraph in _phaseGraphs){
                if(!phaseGraph.IsTitle)
                phaseGraph.AppendImageMapInfo(imageMap);
            }
        }
    }
}
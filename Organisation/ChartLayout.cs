using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;

namespace ch.appl.psoft.Organisation
{
    using ch.appl.psoft.Organisation.VisioExport;
    using Interface.DBObjects;

    #region Helpers - classes with common properities
    public abstract class OrganisationBaseNodes : IEnumerable
    {
        public ArrayList _nodes = new ArrayList();

        internal OrganisationBaseNodes() { }

        #region Properities
        public int Count
        {
            get { return _nodes.Count; }
        }
        #endregion

        #region IEnumerable support
        public System.Collections.IEnumerator GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
        #endregion
    }

    public abstract class OrganisationBaseNode : OrganisationBaseNodes
    {
        protected long _id = -1;
        protected string _title = "";

        internal OrganisationBaseNode(long id, string title)
            : base()
        {
            _id = id;
            _title = title;
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }
        #endregion
    }
    #endregion

    #region Chart Node Layout helpers (Loader and chartnodelayout)
    public class ImageLoader
    {
        public static Image loadImageFromFile(string filename)
        {
            Image image = null;
            try
            {
                image = Image.FromFile(filename);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, Logger.ERROR);
            }
            return image;
        }
    }

    public class ChartNodeLayoutLoader : OrganisationBaseNodes
    {
        private DBData _db = null;
        private string _imagePath = "";

        public ChartNodeLayoutLoader(DBData db, string imagePath)
            : base()
        {
            _db = db;
            _imagePath = imagePath;
        }

        #region Properities
        public ChartNodeLayout this[long chartNodeLayoutId]
        {
            get
            {
                ChartNodeLayout layout = null;
                foreach (ChartNodeLayout cnl in _nodes)
                    if (cnl.Id == chartNodeLayoutId)
                    {
                        layout = cnl;
                        break;
                    }
                if (layout == null)
                {
                    layout = Load(chartNodeLayoutId);
                }
                return layout;
            }
        }
        #endregion

        #region Private method to load chart layout from database
        private ChartNodeLayout Load(long chartNodeLayoutId)
        {
            ChartNodeLayout layout = ChartNodeLayout.Load(_db, chartNodeLayoutId, _imagePath);
            _nodes.Add(layout);
            return layout;
        }
        #endregion
    }

    public class ChartNodeLayout
    {
        private long _id = -1;
        private string _title = "";
        private string _imageName = "";
        private string _imagePath = "";
        private int _nodeWidth = 0;
        private int _nodeHeight = 0;
        private int _lineWidth = 1;
        private Color _lineColor = Color.Black;
        private Color _backgroundColor = Color.White;

        // Default horizontal right/left padding between node-line and node-text
        private int _paddingLeft = 4;
        private int _paddingRight = 4;
        // Default vertical top padding between node-line and node-text
        private int _paddingTop = 2;

        internal ChartNodeLayout(DBData db, DataRow row, string imagePath)
        {
            _imagePath = imagePath;
            InitFromRow(db, row);
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }

        public int NodeWidth
        {
            get { return _nodeWidth; }
        }

        public int NodeHeight
        {
            get { return _nodeHeight; }
        }

        public string ImageName
        {
            get { return _imageName; }
        }

        public int LineWidth
        {
            get { return _lineWidth; }
        }

        public Color LineColor
        {
            get { return _lineColor; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
        }

        public System.Drawing.Image Image
        {
            get { return (_imageName != "") ? ImageLoader.loadImageFromFile(_imagePath + "\\" + _imageName) : null; }
        }

        public int PaddingLeft
        {
            get { return _paddingLeft; }
        }

        public int PaddingRight
        {
            get { return _paddingRight; }
        }

        public int PaddingTop
        {
            get { return _paddingTop; }
        }
        #endregion

        #region Private methods to init node from data row
        private void InitFromRow(DBData db, DataRow row)
        {
            _id = DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"];
            _imageName = DBColumn.IsNull(row["IMAGE"]) ? "" : (string)row["IMAGE"];
            _title = DBColumn.IsNull(row[db.langAttrName("CHARTNODELAYOUT", "TITLE")]) ? "" : (string)row[db.langAttrName("CHARTNODELAYOUT", "TITLE")];
            _nodeWidth = DBColumn.IsNull(row["NODEWIDTH"]) ? -1 : (int)row["NODEWIDTH"];
            _nodeHeight = DBColumn.IsNull(row["NODEHEIGHT"]) ? -1 : (int)row["NODEHEIGHT"];
            _lineWidth = DBColumn.IsNull(row["LINEWIDTH"]) ? 1 : (int)row["LINEWIDTH"];
            _lineColor = DBColumn.IsNull(row["LINECOLOR"]) ? Color.Black : Color.FromArgb((int)row["LINECOLOR"]);
            _backgroundColor = DBColumn.IsNull(row["BACKGROUNDCOLOR"]) ? Color.White : Color.FromArgb((int)row["BACKGROUNDCOLOR"]);
            _paddingLeft = DBColumn.IsNull(row["PADDING_LEFT"]) ? _paddingLeft : (int)row["PADDING_LEFT"];
            _paddingRight = DBColumn.IsNull(row["PADDING_RIGHT"]) ? _paddingRight : (int)row["PADDING_RIGHT"];
            _paddingTop = DBColumn.IsNull(row["PADDING_TOP"]) ? _paddingTop : (int)row["PADDING_TOP"];
        }
        #endregion

        #region Static methods to load chart layout from database
        public static string PrepareSqlStatement(DBData db, long chartNodeLayoutId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, ");
            str.Append(db.langAttrName("CHARTNODELAYOUT", "TITLE"));
            str.Append(", IMAGE, NODEWIDTH, NODEHEIGHT, LINEWIDTH, LINECOLOR, BACKGROUNDCOLOR, PADDING_TOP, PADDING_LEFT, PADDING_RIGHT ");
            str.Append("FROM CHARTNODELAYOUT ");
            str.Append("WHERE (ID = ");
            str.Append(chartNodeLayoutId);
            str.Append(")");
            return str.ToString();
        }

        public static ChartNodeLayout Load(DBData db, long chartNodeLayoutId, string imagePath)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartNodeLayoutId), "CHARTNODELAYOUT");

            if (table.Rows.Count == 0)
                return null;

            return new ChartNodeLayout(db, table.Rows[0], imagePath);
        }
        #endregion
    }
    #endregion

    #region Chart Text Layout helpers (Loader and charttextlayout)
    public class ChartTextLayoutLoader : OrganisationBaseNodes
    {
        private DBData _db = null;

        public ChartTextLayoutLoader(DBData db)
            : base()
        {
            _db = db;
        }

        #region Properities
        public ChartTextLayout this[long chartTextLayoutId]
        {
            get
            {
                ChartTextLayout layout = null;
                foreach (ChartTextLayout cnl in _nodes)
                    if (cnl.Id == chartTextLayoutId)
                    {
                        layout = cnl;
                        break;
                    }
                if (layout == null)
                {
                    layout = Load(chartTextLayoutId);
                }
                return layout;
            }
        }
        #endregion

        #region Private method to load chart layout from database
        private ChartTextLayout Load(long chartTextLayoutId)
        {
            ChartTextLayout layout = ChartTextLayout.Load(_db, chartTextLayoutId);
            _nodes.Add(layout);
            return layout;
        }
        #endregion
    }

    public class ChartTextLayout
    {
        public enum HorizontalAlignments
        {
            Left,
            Center,
            Right
        }

        private long _id = -1;
        private string _title = "";

        private string _fontFamily = "arial";
        private int _fontSize = 8;
        private FontStyle _fontStyle = FontStyle.Bold;
        private Color _fontColor = Color.Black;
        private HorizontalAlignments _horizontalAlign = HorizontalAlignments.Center;

        private Font _font = null;

        internal ChartTextLayout(DBData db, DataRow row)
        {
            InitFromRow(db, row);
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }

        public string FontFamily
        {
            get { return _fontFamily; }
        }

        public int FontSize
        {
            get { return _fontSize; }
        }

        public FontStyle FontStyle
        {
            get { return _fontStyle; }
        }

        public Color FontColor
        {
            get { return _fontColor; }
        }

        public Font Font
        {
            get { return _font; }
        }

        public HorizontalAlignments HorizontalAlign
        {
            get { return _horizontalAlign; }
        }
        #endregion

        #region Private methods to init node from data row
        private void ResetFont(DataRow row)
        {
            if (!DBColumn.IsNull(row["FONTFAMILY"]))
                _fontFamily = (string)row["FONTFAMILY"];
            if (!DBColumn.IsNull(row["FONTSIZE"]))
                _fontSize = (int)row["FONTSIZE"];
            if (!DBColumn.IsNull(row["FONTSTYLE"]))
                _fontStyle = (FontStyle)row["FONTSTYLE"];

            _font = new Font(_fontFamily, _fontSize, _fontStyle);
        }

        private void InitFromRow(DBData db, DataRow row)
        {
            _id = DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"];
            _title = DBColumn.IsNull(row[db.langAttrName("CHARTTEXTLAYOUT", "TITLE")]) ? "" : (string)row[db.langAttrName("CHARTTEXTLAYOUT", "TITLE")];
            _fontColor = DBColumn.IsNull(row["FONTCOLOR"]) ? Color.Black : Color.FromArgb((int)row["FONTCOLOR"]);
            _horizontalAlign = DBColumn.IsNull(row["HORIZONTAL_ALIGN"]) ? HorizontalAlignments.Center : (HorizontalAlignments)(int)row["HORIZONTAL_ALIGN"];
            ResetFont(row);
        }
        #endregion

        #region Static methods to load chart layout from database
        public static string PrepareSqlStatement(DBData db, long chartTextLayoutId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, ");
            str.Append(db.langAttrName("CHARTTEXTLAYOUT", "TITLE"));
            str.Append(",FONTFAMILY, FONTSIZE, FONTSTYLE, FONTCOLOR, HORIZONTAL_ALIGN ");
            str.Append("FROM CHARTTEXTLAYOUT ");
            str.Append("WHERE (ID = ");
            str.Append(chartTextLayoutId);
            str.Append(")");
            return str.ToString();
        }

        public static ChartTextLayout Load(DBData db, long chartTextLayoutId)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartTextLayoutId), "CHARTTEXTLAYOUT");

            if (table.Rows.Count == 0)
                return null;

            return new ChartTextLayout(db, table.Rows[0]);
        }
        #endregion
    }
    #endregion

    #region Chart Pikto Layout helpers (Loader and chartpiktolayout)
    public class ChartPiktoLayoutLoader : OrganisationBaseNodes
    {
        private DBData _db = null;
        private string _imagePath = "";

        public ChartPiktoLayoutLoader(DBData db, string imagePath)
            : base()
        {
            _db = db;
            _imagePath = imagePath;
        }

        #region Properities
        public ChartPiktoLayout this[long chartPiktoLayoutId]
        {
            get
            {
                ChartPiktoLayout layout = null;
                foreach (ChartPiktoLayout cpl in _nodes)
                    if (cpl.Id == chartPiktoLayoutId)
                    {
                        layout = cpl;
                        break;
                    }
                if (layout == null)
                {
                    layout = Load(chartPiktoLayoutId);
                }
                return layout;
            }
        }
        #endregion

        #region Private method to load chart pikto layout from database
        private ChartPiktoLayout Load(long chartPiktoLayoutId)
        {
            ChartPiktoLayout layout = ChartPiktoLayout.Load(_db, chartPiktoLayoutId, _imagePath);
            _nodes.Add(layout);
            return layout;
        }
        #endregion
    }

    public class ChartPiktoLayout
    {
        private long _id = -1;
        private string _title = "";
        private string _imageName = "";
        private string _imagePath = "";
        private int _width = 0;
        private int _height = 0;
        private int _lineWidth = 1;
        private Color _lineColor = Color.Black;
        private Color _backgroundColor = Color.White;

        internal ChartPiktoLayout(DBData db, DataRow row, string imagePath)
        {
            _imagePath = imagePath;
            InitFromRow(db, row);
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public string ImageName
        {
            get { return _imageName; }
        }

        public int LineWidth
        {
            get { return _lineWidth; }
        }

        public Color LineColor
        {
            get { return _lineColor; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
        }

        public System.Drawing.Image Image
        {
            get { return (_imageName != "") ? ImageLoader.loadImageFromFile(_imagePath + "\\" + _imageName) : null; }
        }
        #endregion

        #region Private methods to init chart pikto layout from data row
        private void InitFromRow(DBData db, DataRow row)
        {
            _id = DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"];
            _imageName = DBColumn.IsNull(row["IMAGE"]) ? "" : (string)row["IMAGE"];
            _title = DBColumn.IsNull(row[db.langAttrName("CHARTPIKTOLAYOUT", "TITLE")]) ? "" : (string)row[db.langAttrName("CHARTPIKTOLAYOUT", "TITLE")];
            _width = DBColumn.IsNull(row["WIDTH"]) ? -1 : (int)row["WIDTH"];
            _height = DBColumn.IsNull(row["HEIGHT"]) ? -1 : (int)row["HEIGHT"];
            _lineWidth = DBColumn.IsNull(row["LINEWIDTH"]) ? 0 : (int)row["LINEWIDTH"];
            _lineColor = DBColumn.IsNull(row["LINECOLOR"]) ? Color.Black : Color.FromArgb((int)row["LINECOLOR"]);
            _backgroundColor = DBColumn.IsNull(row["BACKGROUNDCOLOR"]) ? Color.White : Color.FromArgb((int)row["BACKGROUNDCOLOR"]);
        }
        #endregion

        #region Static methods to load chart layout from database
        public static string PrepareSqlStatement(DBData db, long chartPiktoLayoutId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, ");
            str.Append(db.langAttrName("CHARTPIKTOLAYOUT", "TITLE"));
            str.Append(", IMAGE, WIDTH, HEIGHT, LINEWIDTH, LINECOLOR, BACKGROUNDCOLOR ");
            str.Append("FROM CHARTPIKTOLAYOUT ");
            str.Append("WHERE (ID = ");
            str.Append(chartPiktoLayoutId);
            str.Append(")");
            return str.ToString();
        }

        public static ChartPiktoLayout Load(DBData db, long chartPiktoLayoutId, string imagePath)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartPiktoLayoutId), "CHARTPIKTOLAYOUT");

            if (table.Rows.Count == 0)
                return null;

            return new ChartPiktoLayout(db, table.Rows[0], imagePath);
        }
        #endregion
    }
    #endregion

    #region Chart Alignment helpers
    public class ChartAlignmentLoader : OrganisationBaseNodes
    {
        private DBData _db = null;

        public ChartAlignmentLoader(DBData db)
            : base()
        {
            _db = db;
        }

        #region Properities
        public ChartAlignment this[long chartAlignmentId]
        {
            get
            {
                ChartAlignment layout = null;
                foreach (ChartAlignment cnl in _nodes)
                    if (cnl.Id == chartAlignmentId)
                    {
                        layout = cnl;
                        break;
                    }
                if (layout == null)
                {
                    layout = Load(chartAlignmentId);
                }
                return layout;
            }
        }

        public ChartAlignment Default
        {
            get { return new ChartAlignment(); }
        }
        #endregion

        #region Private method to load chart alignment from database
        private ChartAlignment Load(long chartAlignmentId)
        {
            ChartAlignment layout = ChartAlignment.Load(_db, chartAlignmentId);
            _nodes.Add(layout);
            return layout;
        }
        #endregion
    }

    public class ChartAlignment
    {
        private long _id = -1;
        private string _title = "Default";
        private string _alignType = GraphAlignment.HorizontalCenter.ToString();

        public ChartAlignment(DBData db, DataRow row)
        {
            InitFromRow(db, row);
        }

        internal ChartAlignment()
        {
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public string Title
        {
            get { return _title; }
        }

        public GraphAlignment GraphAlignment
        {
            get { return (_alignType != "") ? (GraphAlignment)Enum.Parse(typeof(GraphAlignment), _alignType) : GraphAlignment.HorizontalCenter; }
        }
        #endregion

        #region Private method to init from row
        private void InitFromRow(DBData db, DataRow row)
        {
            _id = DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"];
            _title = DBColumn.IsNull(row[db.langAttrName("CHARTALIGNMENT", "TITLE")]) ? "" : (string)row[db.langAttrName("CHARTALIGNMENT", "TITLE")];
            _alignType = DBColumn.IsNull(row["ALIGNTYPE"]) ? "" : (string)row["ALIGNTYPE"];
        }
        #endregion

        #region Static methods to load chart alignment from database
        public static string PrepareSqlStatement(DBData db, long chartAlignmentId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID,");
            str.Append(db.langAttrName("CHARTALIGNMENT", "TITLE"));
            str.Append(", ALIGNTYPE FROM CHARTALIGNMENT ");
            str.Append("WHERE (ID = ");
            str.Append(chartAlignmentId);
            str.Append(")");
            return str.ToString();
        }

        public static ChartAlignment Load(DBData db, long chartAlignmentId)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartAlignmentId), "CHARTALIGNMENT");

            if (table.Rows.Count == 0)
                return null;

            return new ChartAlignment(db, table.Rows[0]);
        }
        #endregion
    }

    #endregion

    #region Chart Text and Pikto
    public abstract class ChartPiktoText
    {
        private long _id = -1;
        private int _typ = -1;
        private long _chartNodeId = -1;
        private string _text = "";
        private string _link = "";
        private string _targetFrame = "";
        private double _ordNumber = 1;

        internal ChartPiktoText(DBData db, DataRow row)
        {
            InitFromRow(db, row);
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public long ChartNodeId
        {
            get { return _chartNodeId; }
        }

        public string Text
        {
            get { return _text; }
        }

        public string Link
        {
            get { return _link; }
        }

        public string TargetFrame
        {
            get { return _targetFrame; }
        }

        public double OrdNumber
        {
            get { return _ordNumber; }
        }
        #endregion

        #region Private method to init chartpikto/text from row
        protected virtual void InitFromRow(DBData db, DataRow row)
        {
            _id = DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"];
            _typ = DBColumn.IsNull(row["TYP"]) ? -1 : (int)row["TYP"];
            _chartNodeId = DBColumn.IsNull(row["CHARTNODE_ID"]) ? -1 : (long)row["CHARTNODE_ID"];
            _ordNumber = DBColumn.IsNull(row["ORDNUMBER"]) ? 1 : (double)row["ORDNUMBER"];
            _text = DBColumn.IsNull(row[db.langAttrName("CHARTTEXT", "TEXT")]) ? "" : (string)row[db.langAttrName("CHARTTEXT", "TEXT")];
            _link = DBColumn.IsNull(row["LINK"]) ? "" : (string)row["LINK"];
            _targetFrame = DBColumn.IsNull(row["TARGETFRAME"]) ? "" : (string)row["TARGETFRAME"];
        }
        #endregion

        #region Public methods
        public void Prepare()
        {

        }
        #endregion

        #region Static method with sql statement for all charttext for given node
        public static string PrepareSqlStatementForAll(long chartNodeId, int typ)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID ");
            str.Append("FROM CHARTTEXT ");
            str.Append("WHERE CHARTNODE_ID=");
            str.Append(chartNodeId);
            str.Append(" AND TYP=");
            str.Append(typ);
            str.Append(" ORDER BY ORDNUMBER");
            return str.ToString();
        }

        public static string PrepareSqlStatement(DBData db, long chartTextId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, TYP, ");
            str.Append(db.langAttrName("CHARTTEXT", "TEXT"));
            str.Append(", LINK, TARGETFRAME, ORDNUMBER, LAYOUT_ID, CHARTNODE_ID, CHARTPIKTOLAYOUT_ID ");
            str.Append("FROM CHARTTEXT ");
            str.Append("WHERE ID=");
            str.Append(chartTextId);
            return str.ToString();
        }
        #endregion
    }

    public class ChartText : ChartPiktoText
    {
        public static int TYP = 0;
        private long _textLayoutId = -1;
        private ChartTextLayout _textLayout = null;

        internal ChartText(DBData db, DataRow row) : base(db, row) { }

        #region Properties
        public long TextLayoutId
        {
            get { return _textLayoutId; }
        }

        public ChartTextLayout ChartTextLayout
        {
            get { return _textLayout; }
            set { _textLayout = value; }
        }
        #endregion

        #region Private method to init charttext from row
        protected override void InitFromRow(DBData db, DataRow row)
        {
            base.InitFromRow(db, row);
            _textLayoutId = DBColumn.IsNull(row["LAYOUT_ID"]) ? -1 : (long)row["LAYOUT_ID"];
        }
        #endregion

        #region Static method with sql statement for all charttext for given node
        public static string PrepareSqlStatementForAll(long chartNodeId)
        {
            return PrepareSqlStatementForAll(chartNodeId, TYP);
        }

        public static ChartText Load(DBData db, long chartTextId)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartTextId), "CHARTTEXT");

            if (table.Rows.Count == 0)
                return null;

            return new ChartText(db, table.Rows[0]);
        }
        #endregion
    }

    public class ChartPikto : ChartPiktoText
    {
        public static int TYP = 1;
        private long _piktoLayoutId = -1;
        private ChartPiktoLayout _piktoLayout = null;

        internal ChartPikto(DBData db, DataRow row) : base(db, row) { }

        #region Properties
        public long PiktoLayoutId
        {
            get { return _piktoLayoutId; }
        }

        public ChartPiktoLayout ChartPiktoLayout
        {
            get { return _piktoLayout; }
            set { _piktoLayout = value; }
        }
        #endregion

        #region Private method to init chartpikto from row
        protected override void InitFromRow(DBData db, DataRow row)
        {
            base.InitFromRow(db, row);
            _piktoLayoutId = DBColumn.IsNull(row["CHARTPIKTOLAYOUT_ID"]) ? -1 : (long)row["CHARTPIKTOLAYOUT_ID"];
        }
        #endregion

        #region Static method with sql statement for all chartpikto for given node
        public static string PrepareSqlStatementForAll(long chartNodeId)
        {
            return PrepareSqlStatementForAll(chartNodeId, TYP);
        }

        public static ChartPikto Load(DBData db, long chartPiktoId)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartPiktoId), "CHARTTEXT");

            if (table.Rows.Count == 0)
                return null;

            return new ChartPikto(db, table.Rows[0]);
        }
        #endregion
    }
    #endregion

    #region ChartEmployee - Node with employee data
    public class ChartEmployee : OrganisationBaseNode
    {
        private long _uid = -1;

        public ChartEmployee(long id, long uid, string personName)
            : base(id, personName)
        {
            _uid = uid;
        }

        public long Uid
        {
            get { return _uid; }
        }
    }
    #endregion

    #region Chart and ChartNode
    public class Chart : OrganisationBaseNode
    {
        public static DBData dbData;

        public OrganisationNode _organisation = null;
        public ChartNodeLayout _layout = null;
        public ChartTextLayout _textLayout = null;
        public ChartAlignment _chartAlignment = null;
        public ChartNodesGraph _rootGraph = null;

        private string _backgroundImageName = "";
        private string _imagePath = "";

        private string _targetFrame = "";
        private string _navigateUrl = "";


        internal Chart(DataRow row) :
            base(
            DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"],
            DBColumn.IsNull(row[dbData.langAttrName("CHART", "TITLE")]) ? "" : (string)row[dbData.langAttrName("CHART", "TITLE")]
            ) { }

        #region Properities
        public ChartNode this[long orgentityId]
        {
            get
            {
                ChartNode _node = null;
                foreach (ChartNode n in _nodes)
                    if (n.OrgentityId == orgentityId)
                    {
                        _node = n;
                        break;
                    }
                return _node;
            }
        }

        public OrganisationNode Organisation
        {
            get { return _organisation; }
        }

        public ChartNodeLayout ChartNodeLayout
        {
            get { return _layout; }
        }

        public ChartTextLayout ChartTextLayout
        {
            get { return _textLayout; }
        }

        public ChartAlignment ChartAlignment
        {
            get { return _chartAlignment; }
        }

        public ChartNodesGraph RootGraph
        {
            get { return _rootGraph; }
        }

        public string BackgroundImageName
        {
            get { return _backgroundImageName; }
        }

        public System.Drawing.Image BackgroundImage
        {
            get { return (_backgroundImageName != "") ? ImageLoader.loadImageFromFile(_imagePath + "\\" + _backgroundImageName) : null; }
        }

        public string TargetFrame
        {
            get { return _targetFrame; }
            set { _targetFrame = value; }
        }

        public string NavigateUrl
        {
            get { return _navigateUrl; }
            set { _navigateUrl = value; }
        }
        #endregion

        #region Public methods
        public void Add(ChartNode node)
        {
            if (node != null)
                _nodes.Add(node);
        }

        public void Remove(ChartNode node)
        {
            _nodes.Remove(node);
        }

        public System.Drawing.Image GetImageGraph()
        {
            List<VisioElement> elements = new List<VisioElement>();
            return GetImageGraph(elements);
        }

        public System.Drawing.Image GetImageGraph(List<VisioElement> elements)
        {
            _rootGraph = new ChartNodesGraph(this);
            _rootGraph.NavigateUrl = _navigateUrl;
            _rootGraph.TargetFrame = _targetFrame;
            foreach (ChartNode n in _nodes)
                if (n.Parent == null)
                    n.AppendToGraphNode(_rootGraph);

            _rootGraph.ResetPositionX(_rootGraph.GetWidth() / 2);
            _rootGraph.ResetPositionY(0);
            _rootGraph.RedrawGraph();

            int w = _rootGraph.GetDrawingWidth() + 2;
            int h = _rootGraph.GetDrawingHeight() + 2;

            _rootGraph.OffsetX = -1 * _rootGraph.GetMinX();
            _rootGraph.OffsetY = -1 * (_rootGraph.GetMinY() + _rootGraph.OuterHeight);

            Image backgroundImage = BackgroundImage;
            if (backgroundImage != null)
            {
                w = Math.Max(w, backgroundImage.Width);
                h = Math.Max(h, backgroundImage.Height);
            }
            Bitmap map = new Bitmap(w, h);

            Graphics graphics = Graphics.FromImage(map);
            graphics.Clear(Color.White);
            if (backgroundImage != null)
            {
                graphics.DrawImageUnscaled(backgroundImage, 0, 0);
            }
            _rootGraph.Draw(graphics, elements);

            return map;
        }



        public void AppendImageMapInfo(StringBuilder imageMap)
        {
            if (_rootGraph != null)
                _rootGraph.AppendImageMapInfo(imageMap);
        }

        public void AppendImageMapInfoAdmin(StringBuilder imageMap)
        {
            if (_rootGraph != null)
                _rootGraph.AppendImageMapInfoAdmin(imageMap);
        }
        #endregion

        #region Private method to set proper tree order using organisation tree
        private void ResetChartNodesOrder()
        {
            if (_organisation == null)
                throw new Exception("Cannot build chart tree. Organisation is null. Chart id: " + Id);

            foreach (ChartNode _node in _nodes)
            {
                // Looking for associated orgentity
                OrgentityNode _orgentityNode = _organisation[_node.OrgentityId];

                if (_orgentityNode == null)
                    throw new Exception("Cannot build chart tree. Cannot find orgentity node for chart layout. Chart layout id: " + _node.Id);

                // Setting orgentity node for chart layout node
                _node.Orgentity = _orgentityNode;

                // Looking for proper parent chart layout node in chart layout tree
                // using organisation tree information
                ChartNode parentChartNode = null;
                OrgentityNode _begginerOrgentityNode = _orgentityNode;
                while ((parentChartNode == null) && (_begginerOrgentityNode != null))
                {
                    parentChartNode = this[_begginerOrgentityNode.Id];
                    if (parentChartNode == _node)
                        parentChartNode = null;
                    if (parentChartNode == null)
                        _begginerOrgentityNode = _begginerOrgentityNode.Parent;
                }
                if (parentChartNode != null)
                {
                    _node.Parent = parentChartNode;
                }
            }
        }

        private void ResetChartNodesLayout(
            ChartNodeLayoutLoader layoutLoader,
            ChartTextLayoutLoader layoutTextLoader,
            ChartPiktoLayoutLoader layoutPiktoLoader,
            ChartAlignmentLoader alignLoader)
        {
            // we are looking only for layout. ChildLayout we can skip.
            foreach (ChartNode node in _nodes)
            {
                if (node.LayoutId != -1)
                {
                    // Chart layout is defined
                    node.Layout = layoutLoader[node.LayoutId];
                }
                else
                {
                    // Looking for first child layout in parents
                    ChartNode parentChartNode = node.Parent;
                    while ((parentChartNode != null) && (node.Layout == null))
                    {
                        if (parentChartNode.ChildLayoutId != -1)
                            node.Layout = layoutLoader[parentChartNode.ChildLayoutId];
                        parentChartNode = parentChartNode.Parent;
                    }

                    // Setting lauoyt from chart
                    if (node.Layout == null)
                        node.Layout = _layout;
                }

                if (node.ChartAlignmentId != -1)
                {
                    // Chart alignment is defined
                    node.ChartAlignment = alignLoader[node.ChartAlignmentId];
                }
                else
                {
                    // Looking for first alignment in parents
                    ChartNode parentChartNode = node.Parent;
                    while ((parentChartNode != null) && (node.ChartAlignment == null))
                    {
                        if (parentChartNode.ChartAlignmentId != -1)
                            node.ChartAlignment = alignLoader[parentChartNode.ChartAlignmentId];
                        parentChartNode = parentChartNode.Parent;
                    }

                    // Setting default alignment from chart
                    if (node.ChartAlignment == null)
                        node.ChartAlignment = ChartAlignment;
                }

                if (node.GapHorizontal == -1)
                {
                    // Looking for first horizontal gap in parents
                    ChartNode parentChartNode = node.Parent;
                    while ((parentChartNode != null) && (node.GapHorizontal == -1))
                    {
                        node.GapHorizontal = parentChartNode.GapHorizontal;
                        parentChartNode = parentChartNode.Parent;
                    }
                }

                if (node.GapVertical == -1)
                {
                    // Looking for first vertical gap in parents
                    ChartNode parentChartNode = node.Parent;
                    while ((parentChartNode != null) && (node.GapVertical == -1))
                    {
                        node.GapVertical = parentChartNode.GapVertical;
                        parentChartNode = parentChartNode.Parent;
                    }
                }

                if (node.VerticalAlignOffset == -1)
                {
                    // Looking for first vertical align offset in parents
                    ChartNode parentChartNode = node.Parent;
                    while ((parentChartNode != null) && (node.VerticalAlignOffset == -1))
                    {
                        node.VerticalAlignOffset = parentChartNode.VerticalAlignOffset;
                        parentChartNode = parentChartNode.Parent;
                    }
                }

                // setting up text layout for each node text
                for (int i = 0; i < node.TextCount; i++)
                {
                    ChartText text = node.GetText(i);
                    // text layout is defined
                    if (text.TextLayoutId != -1)
                        text.ChartTextLayout = layoutTextLoader[text.TextLayoutId];
                    else
                    {
                        // Looking for parent text layout (start with this node)
                        ChartNode parentChartNode = node;
                        while ((parentChartNode != null) && (text.ChartTextLayout == null))
                        {
                            if (parentChartNode.TextLayoutId != -1)
                                text.ChartTextLayout = layoutTextLoader[parentChartNode.TextLayoutId];
                            parentChartNode = parentChartNode.Parent;
                        }

                        // Get textlayout from chart
                        if (text.ChartTextLayout == null)
                            text.ChartTextLayout = ChartTextLayout;
                    }
                }

                // setting up pikto layout for each node pikto
                for (int i = 0; i < node.PiktoCount; i++)
                {
                    ChartPikto pikto = node.GetPikto(i);
                    if (pikto.PiktoLayoutId != -1)
                        pikto.ChartPiktoLayout = layoutPiktoLoader[pikto.PiktoLayoutId];
                    else
                    {
                        // No pikto layout defined!
                    }
                }
            }
        }
        #endregion

        #region Static methods to build chart structure
        public static string PrepareSqlStatement(DBData db, long chartId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, ORGANISATION_ID, CHARTLAYOUT_ID, TEXTLAYOUT_ID, CHARTALIGNMENT_ID, BACKGROUND_IMAGE, ");
            str.Append(db.langAttrName("CHART", "TITLE"));
            str.Append(" FROM CHART WHERE (ID = ");
            str.Append(chartId);
            str.Append(")"); // AND (VERSION_FLAG = 'P')
            return str.ToString();
        }

        public static Chart BuildChart(DBData db, long chartId, string imagePath)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, chartId), "CHART");

            if (table.Rows.Count == 0)
                return null;

            dbData = db;
            // Initialize chart loaders
            ChartTextLayoutLoader layoutTextLoader = new ChartTextLayoutLoader(db);
            ChartPiktoLayoutLoader layoutPiktoLoader = new ChartPiktoLayoutLoader(db, imagePath);
            ChartNodeLayoutLoader layoutNodeLoader = new ChartNodeLayoutLoader(db, imagePath);
            ChartAlignmentLoader alignLoader = new ChartAlignmentLoader(db);

            // Create chart node
            DataRow r = table.Rows[0];
            Chart chart = new Chart(r);

            // Load organisation tree associated with this chart
            if (!DBColumn.IsNull(r["ORGANISATION_ID"]))
            {
                chart._organisation = OrganisationNode.BuildOrganisationNode(db, (long)r["ORGANISATION_ID"]);
            }

            // Load default chart layout for chart
            if (!DBColumn.IsNull(r["CHARTLAYOUT_ID"]))
            {
                chart._layout = layoutNodeLoader[(long)r["CHARTLAYOUT_ID"]];
            }

            // Load default text layout for chart
            if (!DBColumn.IsNull(r["TEXTLAYOUT_ID"]))
            {
                chart._textLayout = layoutTextLoader[(long)r["TEXTLAYOUT_ID"]];
            }

            // Load default alignment for chart
            if (!DBColumn.IsNull(r["CHARTALIGNMENT_ID"]))
            {
                chart._chartAlignment = alignLoader[(long)r["CHARTALIGNMENT_ID"]];
            }

            // Load background image
            chart._backgroundImageName = DBColumn.IsNull(r["BACKGROUND_IMAGE"]) ? "" : (string)r["BACKGROUND_IMAGE"];
            chart._imagePath = imagePath;

            // Load chart layout nodes
            table = db.getDataTable(ChartNode.PrepareSqlStatementForAllNodes(chartId), "CHARTNODE");
            foreach (DataRow row in table.Rows)
            {
                chart.Add(ChartNode.Load(db, (long)row["ID"]));
            }

            // Reset proper chart layout tree order using organisation tree
            chart.ResetChartNodesOrder();

            // Reset for all nodes proper chart layout
            chart.ResetChartNodesLayout(layoutNodeLoader, layoutTextLoader, layoutPiktoLoader, alignLoader);

            return chart;
        }
        #endregion
    }

    public class ChartNode : OrganisationBaseNodes
    {
        private long _id = -1;
        private long _orgentityId = -1;
        private long _layoutId = -1;
        private long _childLayoutId = -1;
        private long _chartAlignmentId = -1;
        private long _textLayoutId = -1;
        private long _clipboardId = -1;
        private int _typ = -1;
        private bool _showEmployees = false;
        private int _gapHorizontal = -1;
        private int _gapVertical = -1;
        private int _verticalAlignOffset = -1;

        private ChartNode _parent = null;
        private OrgentityNode _orgentity = null;

        private ChartNodeLayout _layout = null;
        private ChartNodeLayout _childLayout = null;
        private ChartAlignment _chartAlignment = null;

        private ArrayList _textList = new ArrayList();
        private ArrayList _piktoList = new ArrayList();
        private ArrayList _employees = new ArrayList();
        private ArrayList _lines = new ArrayList();


        internal ChartNode(DataRow row)
            : base()
        {
            InitFromRow(row);
        }

        #region Properities
        public long Id
        {
            get { return _id; }
        }

        public long OrgentityId
        {
            get { return _orgentityId; }
        }

        public long LayoutId
        {
            get { return _layoutId; }
        }

        public long ChildLayoutId
        {
            get { return _childLayoutId; }
        }

        public long ChartAlignmentId
        {
            get { return _chartAlignmentId; }
        }

        public long TextLayoutId
        {
            get { return _textLayoutId; }
        }

        public long ClipboardId
        {
            get { return _clipboardId; }
        }

        public bool ShowEmployees
        {
            get { return _showEmployees; }
        }

        public int GapHorizontal
        {
            get { return _gapHorizontal; }
            set { _gapHorizontal = value; }
        }

        public int GapVertical
        {
            get { return _gapVertical; }
            set { _gapVertical = value; }
        }

        public int VerticalAlignOffset
        {
            get { return _verticalAlignOffset; }
            set { _verticalAlignOffset = value; }
        }

        public bool IsAssistant
        {
            get { return _typ == 1; }
        }

        public ChartNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    _parent.Remove(this);
                if (value != null)
                    value.Add(this);
            }
        }

        public OrgentityNode Orgentity
        {
            get { return _orgentity; }
            set
            {
                if ((value != null) && (value.Id == _orgentityId))
                    _orgentity = value;
                else if (value == null)
                    _orgentity = null;
            }
        }

        public ChartNodeLayout Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }

        public ChartNodeLayout ChildLayout
        {
            get { return _childLayout; }
            set { _childLayout = value; }
        }

        public ChartAlignment ChartAlignment
        {
            get { return _chartAlignment; }
            set { _chartAlignment = value; }
        }

        public int TextCount
        {
            get { return _textList.Count; }
        }

        public int PiktoCount
        {
            get { return _piktoList.Count; }
        }

        public int EmployeeCount
        {
            get { return _employees.Count; }
        }
        #endregion

        #region Private methods to init node from data row
        private void InitFromRow(DataRow row)
        {
            _id = DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"];
            _orgentityId = DBColumn.IsNull(row["ORGENTITY_ID"]) ? -1 : (long)row["ORGENTITY_ID"];
            _layoutId = DBColumn.IsNull(row["LAYOUT_ID"]) ? -1 : (long)row["LAYOUT_ID"];
            _childLayoutId = DBColumn.IsNull(row["CHILDLAYOUT_ID"]) ? -1 : (long)row["CHILDLAYOUT_ID"];
            _chartAlignmentId = DBColumn.IsNull(row["CHARTALIGNMENT_ID"]) ? -1 : (long)row["CHARTALIGNMENT_ID"];
            _textLayoutId = DBColumn.IsNull(row["TEXTLAYOUT_ID"]) ? -1 : (long)row["TEXTLAYOUT_ID"];
            _clipboardId = DBColumn.IsNull(row["CLIPBOARD_ID"]) ? -1 : (long)row["CLIPBOARD_ID"];
            _typ = DBColumn.IsNull(row["TYP"]) ? -1 : (int)row["TYP"];
            _showEmployees = DBColumn.IsNull(row["SHOWEMPLOYEES"]) ? false : (int)row["SHOWEMPLOYEES"] == 1;
            _gapHorizontal = DBColumn.IsNull(row["GAP_HORIZONTAL"]) ? -1 : (int)row["GAP_HORIZONTAL"];
            _gapVertical = DBColumn.IsNull(row["GAP_VERTICAL"]) ? -1 : (int)row["GAP_VERTICAL"];
            _verticalAlignOffset = DBColumn.IsNull(row["VERTICAL_ALIGN_OFFSET"]) ? -1 : (int)row["VERTICAL_ALIGN_OFFSET"];
        }
        #endregion

        #region Public methods
        public void Add(ChartNode child)
        {
            _nodes.Add(child);
            child._parent = this;
        }

        public void Remove(ChartNode child)
        {
            _nodes.Remove(child);
            child._parent = null;
        }

        public void AppendToGraphNode(ChartNodesGraph parentNode)
        {
            ChartNodesGraph thisGraph = new ChartNodesGraph(this);
            if (parentNode != null)
            {
                parentNode.AddChild(thisGraph);
                thisGraph.TargetFrame = parentNode.TargetFrame;
                thisGraph.NavigateUrl = parentNode.NavigateUrl;
            }
            foreach (ChartNode n in _nodes)
                n.AppendToGraphNode(thisGraph);
        }

        public void AddText(ChartText text)
        {
            if (text != null)
                _textList.Add(text);
        }

        public void RemoveText(ChartText text)
        {
            _textList.Remove(text);
        }

        public ChartText GetText(int index)
        {
            return (ChartText)_textList[index];
        }

        public void AddPikto(ChartPikto pikto)
        {
            if (pikto != null)
                _piktoList.Add(pikto);
        }

        public void RemovePikto(ChartPikto pikto)
        {
            _piktoList.Remove(pikto);
        }

        public ChartPikto GetPikto(int index)
        {
            return (ChartPikto)_piktoList[index];
        }

        public void AddEmployee(ChartEmployee employee)
        {
            if (employee != null)
                _employees.Add(employee);
        }


        public void RemoveEmployee(ChartEmployee employee)
        {
            _employees.Remove(employee);
        }

        public ChartEmployee GetEmployee(int index)
        {
            return (ChartEmployee)_employees[index];
        }
        #endregion

        #region Static methods to prepare sql statements and load chartnode
        public static string PrepareSqlStatementForAllNodes(long chartId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT chn.ID ");
            str.Append("FROM CHARTNODE chn, ORGENTITY org ");
            str.Append("WHERE (chn.ORGENTITY_ID = org.ID) AND (chn.CHART_ID = ");
            str.Append(chartId);
            str.Append(") ORDER BY org.PARENT_ID, chn.ORDNUMBER");
            return str.ToString();
        }

        public static string PrepareSqlStatementForEmployees(long orgentityId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT DISTINCT pers.ID, pers.PNAME, pers.FIRSTNAME, pers.UID, MARKER=" + Person.INACTIVE_PERSON_MARKER_SQL + " ");
            str.Append("FROM PERSON pers, EMPLOYMENT emp, JOB j ");
            str.Append("WHERE (j.EMPLOYMENT_ID = emp.ID) AND (emp.PERSON_ID = pers.ID) AND (j.typ=0) AND ");
            str.Append("(j.ORGENTITY_ID = ");
            str.Append(orgentityId);
            str.Append(") ORDER BY pers.PNAME, pers.FIRSTNAME");
            return str.ToString();
        }

        public static string PrepareSqlStatement(long chartNodeId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT CHARTNODE.ID, CHARTNODE.CHART_ID, CHARTNODE.ORGENTITY_ID, CHARTNODE.LAYOUT_ID, CHARTNODE.CHILDLAYOUT_ID, ORGENTITY.CLIPBOARD_ID, ");
            str.Append("CHARTNODE.TEXTLAYOUT_ID, CHARTNODE.CHARTALIGNMENT_ID, CHARTNODE.TYP, CHARTNODE.SHOWEMPLOYEES, CHARTNODE.VERTICAL_ALIGN_OFFSET, CHARTNODE.GAP_HORIZONTAL, CHARTNODE.GAP_VERTICAL ");
            str.Append("FROM CHARTNODE inner join ORGENTITY on CHARTNODE.ORGENTITY_ID=ORGENTITY.ID ");
            str.Append("WHERE CHARTNODE.ID = ");
            str.Append(chartNodeId);
            return str.ToString();
        }

        public static ChartNode Load(DBData db, long chartNodeId)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(chartNodeId), "CHARTNODE");

            if (table.Rows.Count == 0)
                return null;

            // Create chartnode
            ChartNode n = new ChartNode(table.Rows[0]);

            // Load employee data
            if (n.ShowEmployees)
            {
                table = db.getDataTable(PrepareSqlStatementForEmployees(n.OrgentityId), "PERSON");
                foreach (DataRow row in table.Rows)
                {
                    string name = "";
                    if (Global.isModuleEnabled("energiedienst"))
                    {
                        name = DBColumn.IsNull(row["PNAME"]) ? "" : (string)row["PNAME"];
                        if (!DBColumn.IsNull(row["FIRSTNAME"]))
                            name += " " + (string)row["FIRSTNAME"];
                    }
                    else
                    {
                        name = DBColumn.IsNull(row["FIRSTNAME"]) ? "" : (string)row["FIRSTNAME"];
                        if (!DBColumn.IsNull(row["PNAME"]))
                            name += " " + (string)row["PNAME"] + (string)row["MARKER"];
                    }
                    // show engagement if enabled
                    if (Global.Config.showEngagement)
                    {
                        object engagement = db.lookup("JOB.ENGAGEMENT", "PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID", "PERSON.ID = " + row["ID"].ToString() + " AND ORGENTITY.ID = " + n.OrgentityId);
                        name += " " + engagement + "%";
                    }

                    n.AddEmployee(new ChartEmployee((long)row["ID"], (long)row["UID"], name));
                }
            }

            // Load charttext
            table = db.getDataTable(ChartText.PrepareSqlStatementForAll(n.Id), "CHARTTEXT");
            foreach (DataRow row in table.Rows)
                n.AddText(ChartText.Load(db, (long)row["ID"]));

            // Load chartpikto
            table = db.getDataTable(ChartPikto.PrepareSqlStatementForAll(n.Id), "CHARTTEXT");
            foreach (DataRow row in table.Rows)
                n.AddPikto(ChartPikto.Load(db, (long)row["ID"]));

            return n;
        }
        #endregion
    }
    #endregion

    #region Organisation and OrgentityNode
    public class OrganisationNode : OrganisationBaseNode
    {
        private long _orgentityId = -1;
        private OrgentityNode _rootOrgentity = null;

        internal OrganisationNode(DBData db, DataRow row) :
            this(
            DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"],
            DBColumn.IsNull(row[db.langAttrName("ORGANISATION", "TITLE")]) ? "" : (string)row[db.langAttrName("ORGANISATION", "TITLE")]
            )
        {
            _orgentityId = DBColumn.IsNull(row["ORGENTITY_ID"]) ? -1 : (long)row["ORGENTITY_ID"];
        }

        internal OrganisationNode(long id, string title) :
            base(id, title) { }

        #region Properities
        public OrgentityNode this[long orgentityId]
        {
            get
            {
                OrgentityNode node = null;
                foreach (OrgentityNode n in _nodes)
                    if (n.Id == orgentityId)
                    {
                        node = n;
                        break;
                    }
                return node;
            }
        }

        public long OrgentityId
        {
            get { return _orgentityId; }
        }

        public OrgentityNode RootOrgentity
        {
            get { return _rootOrgentity; }
        }
        #endregion

        #region Public methods
        public void Add(OrgentityNode node)
        {
            _nodes.Add(node);
        }

        public void Remove(OrgentityNode node)
        {
            _nodes.Remove(node);
        }
        #endregion

        #region Static methods to build organisation structure
        public static string PrepareSqlStatement(DBData db, long organisationId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, " + db.langAttrName("ORGANISATION", "TITLE") + ", ORGENTITY_ID FROM ORGANISATION WHERE (ID = ");
            str.Append(organisationId);
            str.Append(")"); // AND (VERSION_FLAG = 'P')
            return str.ToString();
        }

        public static OrganisationNode BuildOrganisationNode(DBData db, long organisationId)
        {
            DataTable table = db.getDataTable(PrepareSqlStatement(db, organisationId), "ORGANISATION");

            if (table.Rows.Count == 0)
                return null;

            OrganisationNode org = new OrganisationNode(db, table.Rows[0]);
            org._rootOrgentity = OrgentityNode.BuildOrgentityTree(db, org);

            return org;
        }
        #endregion
    }

    public class OrgentityNode : OrganisationBaseNode
    {
        private OrgentityNode _parent = null;
        private string _mnemonic = "";

        internal OrgentityNode(DBData db, DataRow row) :
            this(db, row, null) { }

        internal OrgentityNode(DBData db, DataRow row, OrgentityNode parent) :
            this(
            DBColumn.IsNull(row["ID"]) ? -1 : (long)row["ID"],
            DBColumn.IsNull(row[db.langAttrName("ORGENTITY", "TITLE")]) ? "" : (string)row[db.langAttrName("ORGENTITY", "TITLE")],
            DBColumn.IsNull(row[db.langAttrName("ORGENTITY", "MNEMONIC")]) ? "" : (string)row[db.langAttrName("ORGENTITY", "MNEMONIC")],
            parent
            ) { }

        internal OrgentityNode(long id, string title, string mnemonic) :
            this(id, title, mnemonic, null) { }

        internal OrgentityNode(long id, string title, string mnemonic, OrgentityNode parent) :
            base(id, title)
        {
            _mnemonic = mnemonic;
            if (parent != null)
                parent.Add(this);
        }

        #region Properities
        public OrgentityNode this[long orgentityId]
        {
            get
            {
                OrgentityNode node = null;
                foreach (OrgentityNode n in _nodes)
                    if (n.Id == orgentityId)
                    {
                        node = n;
                        break;
                    }
                return node;
            }
        }

        public OrgentityNode Parent
        {
            get { return _parent; }
        }

        public long ParentId
        {
            get { return (_parent != null) ? _parent.Id : -1; }
        }

        public string Mnemonic
        {
            get { return _mnemonic; }
        }
        #endregion

        #region Public methods
        public void Add(OrgentityNode childNode)
        {
            _nodes.Add(childNode);
            childNode._parent = this;
        }

        public void Remove(OrgentityNode childNode)
        {
            childNode._parent = null;
            _nodes.Remove(childNode);
        }
        #endregion

        #region Static methods to build orgentity tree
        public static string PrepareSqlStatementForRoot(DBData db, long orgentityId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, " + db.langAttrName("ORGENTITY", "TITLE", "MNEMONIC") + " FROM ORGENTITY WHERE (ID = ");
            str.Append(orgentityId);
            str.Append(")");
            return str.ToString();
        }

        public static string PrepareSqlStatementForChildren(DBData db, long rootOrgentityId)
        {
            StringBuilder str = new StringBuilder(255);
            str.Append("SELECT ID, " + db.langAttrName("ORGENTITY", "TITLE", "MNEMONIC") + " FROM ORGENTITY WHERE (PARENT_ID = ");
            str.Append(rootOrgentityId);
            str.Append(") ORDER BY ORDNUMBER ASC");
            return str.ToString();
        }

        private static void AppendChildren(DBData db, OrganisationNode organisation, OrgentityNode rootNode)
        {
            DataTable table = db.getDataTable(PrepareSqlStatementForChildren(db, rootNode.Id), "ORGENTITY");
            foreach (DataRow row in table.Rows)
            {
                OrgentityNode node = new OrgentityNode(db, row, rootNode);
                organisation.Add(node);
                AppendChildren(db, organisation, node);
            }
        }

        public static OrgentityNode BuildOrgentityTree(DBData db, OrganisationNode organisation)
        {
            DataTable table = db.getDataTable(PrepareSqlStatementForRoot(db, organisation.OrgentityId), "ORGENTITY");
            if (table.Rows.Count == 0)
                return null;

            DataRow row = table.Rows[0];
            OrgentityNode node = new OrgentityNode(db, row);
            organisation.Add(node);
            AppendChildren(db, organisation, node);
            return node;
        }
        #endregion
    }
    #endregion

    #region Graph - drawing orgentity graph class (ChartNodesGraph, GraphAlignment)
    public enum GraphAlignment
    {
        HorizontalCenter,
        HorizontalLeft,
        HorizontalRight,
        VerticalLeft,
        VerticalRight
    }

    public class ChartNodesGraph
    {

        private ChartNodesGraph _parent = null;
        private ArrayList _children = new ArrayList();
        private ArrayList _assistants = new ArrayList();

        private ChartNode _node = null;
        private Chart _chart = null;

        private int _offsetX = 0;
        private int _offsetY = 0;
        private int _startX = 0;
        private int _startY = 0;
        private int _maxYChart = 0;
        private int _minYChart = 0;

        // Default width and height (if not set in layout)
        private int _width = 130;
        private int _height = 80;

        // Default line-width (if not set in layout)
        private int _lineWidth = 1;

        // Default horizontal gap between two nodes (if not set in node)
        private int _gapHorizontal = 10;
        // Default vertical gap between two nodes (if not set in node)
        private int _gapVertical = 10;

        private int _employeeWidth = 150;
        private int _employeeHeight = 15;
        private int _employeeDeltaWidth = 5;
        private int _employeeDeltaHeight = 3;

        // Default width and height for piktos
        private int _piktoWidth = 16;
        private int _piktoHeight = 16;

        // Offset for vertical alignment right/left from left/right corner of parent node.
        private int _verticalAlignOffset = 15;

        private static System.Drawing.Brush _brushLine = System.Drawing.Brushes.Black;
        private static System.Drawing.Pen _penLine = new System.Drawing.Pen(_brushLine, 1);

        private string _targetFrame = "";
        private string _navigateUrl = psoft.Goto.GetURL("alias", "PhoneListOE") + "&param0=%ID";

        private StringBuilder _employeeMapInfo = new StringBuilder(1024);
        private StringBuilder _textMapInfo = new StringBuilder(1024);
        private StringBuilder _piktoMapInfo = new StringBuilder(1024);

        public ChartNodesGraph(ChartNode node)
        {
            _node = node;
            if (_node != null)
            {
                System.Drawing.Image _img = _node.Layout.Image;
                if (_img == null)
                {
                    _lineWidth = _node.Layout.LineWidth;
                }
                else
                {
                    _lineWidth = 0;
                }

                // Reset node width and height from layout
                if ((_node.Layout.NodeHeight != 0) && (_node.Layout.NodeWidth != 0))
                {
                    _width = _node.Layout.NodeWidth;
                    _height = _node.Layout.NodeHeight;
                }
                else
                {
                    // Reset node width and height from layout image
                    if (_img != null)
                    {
                        _width = _img.Width;
                        _height = _img.Height;
                    }
                }

                if (_node.GapHorizontal > -1)
                {
                    _gapHorizontal = _node.GapHorizontal;
                }

                if (_node.GapVertical > -1)
                {
                    _gapVertical = _node.GapVertical;
                }

                if (_node.VerticalAlignOffset > -1)
                {
                    _verticalAlignOffset = _node.VerticalAlignOffset;
                }
            }
        }

        internal ChartNodesGraph(Chart chart)
        {
            _chart = chart;
        }

        #region Properities: GraphAlignment, Count, IsAssistent...
        public GraphAlignment GraphAlignment
        {
            get { return (_node != null) ? _node.ChartAlignment.GraphAlignment : GraphAlignment.HorizontalCenter; }
        }

        public bool IsHorizontal
        {
            get
            {
                return ((GraphAlignment == GraphAlignment.HorizontalCenter) || (GraphAlignment == GraphAlignment.HorizontalLeft) ||
                    (GraphAlignment == GraphAlignment.HorizontalRight));
            }
        }

        public bool IsVertical
        {
            get { return !IsHorizontal; }
        }

        public bool IsLeftAlignment
        {
            get
            {
                return ((GraphAlignment == GraphAlignment.HorizontalLeft) || (GraphAlignment == GraphAlignment.VerticalLeft));
            }
        }

        public bool IsAssistant
        {
            get { return (_node == null) ? false : _node.IsAssistant; }
        }

        public bool IsRight
        {
            get
            {
                if (_parent == null)
                    return false;

                int modResult = _parent.IsLeftAlignment ? 1 : 0;
                return (IsAssistant) ? (_parent._assistants.IndexOf(this) % 2) != modResult :
                    (_parent._children.IndexOf(this) % 2) != modResult;
            }
        }

        public int Count
        {
            get { return _children.Count; }
        }

        public int CountAssistants
        {
            get { return _assistants.Count; }
        }

        public int CountAll
        {
            get { return Count + CountAssistants; }
        }

        public string TargetFrame
        {
            get { return _targetFrame; }
            set { _targetFrame = value; }
        }

        public string NavigateUrl
        {
            get { return _navigateUrl; }
            set { _navigateUrl = value; }
        }

        public bool HasEmployees
        {
            get { return (_node != null) ? _node.ShowEmployees && _node.EmployeeCount > 0 : false; }
        }
        #endregion

        #region Children management
        public ChartNodesGraph Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    _parent.RemoveChild(this);
                if (value != null)
                    value.AddChild(this);
            }
        }

        public void AddChild(ChartNodesGraph child)
        {
            if (child.IsAssistant)
                _assistants.Add(child);
            else
                _children.Add(child);
            child._parent = this;
        }

        public void RemoveChild(ChartNodesGraph child)
        {
            if (child.IsAssistant)
                _assistants.Remove(child);
            else
                _children.Remove(child);
            child._parent = null;
        }
        #endregion

        #region Setting begin position X and Y, RedrawGraph, ResetPositionX, ResetPositionY
        private void MoveTree(int offsetX)
        {
            _startX += offsetX;
            foreach (ChartNodesGraph cng in _children)
                cng.MoveTree(offsetX);
            foreach (ChartNodesGraph cng in _assistants)
                cng.MoveTree(offsetX);
        }

        private void MoveParent(int startX)
        {
            foreach (ChartNodesGraph cng in _assistants)
                cng.MoveTree(startX - _startX);
            _startX = startX;
            ChartNodesGraph parent = _parent;
            while ((parent != null) && (parent.Count == 1) && (parent.IsHorizontal))
            {
                parent.MoveParent(startX);
                if ((parent.IsAssistant) && (parent._parent != null) && (!parent._parent.IsAssistant))
                    parent = null;
                else
                    parent = parent._parent;
            }
        }

        internal void RedrawGraph()
        {
            foreach (ChartNodesGraph cng in _assistants)
                cng.RedrawGraph();

            foreach (ChartNodesGraph cng in _children)
                cng.RedrawGraph();

            if (IsHorizontal)
            {
                /*
                     * Clear horizontal white space and set optimal _startX position
                     * for tree (begins with this node).
                     * Make HorizontalRight and HorizontalLeft alignment.
                     * Setting proper position of parent node(s).
                     */
                if (Count > 1)
                {
                    for (int i = 1; i < Count; i++)
                    {
                        ChartNodesGraph prev = (ChartNodesGraph)_children[i - 1];
                        ChartNodesGraph next = (ChartNodesGraph)_children[i];

                        next.MoveTree(prev.GetMaxX() - next.GetMinX() + prev._gapHorizontal);
                    }

                    switch (GraphAlignment)
                    {
                        case GraphAlignment.HorizontalCenter:
                            MoveParent((((ChartNodesGraph)_children[0]).OuterLeftX + ((ChartNodesGraph)_children[Count - 1]).OuterLeftX + ((ChartNodesGraph)_children[Count - 1]).OuterWidth) / 2);
                            break;
                        case GraphAlignment.HorizontalRight:
                            MoveParent(((ChartNodesGraph)_children[0])._startX);
                            break;
                        case GraphAlignment.HorizontalLeft:
                            MoveParent(((ChartNodesGraph)_children[Count - 1])._startX);
                            break;
                    }
                } // if Count > 1
                else if (Count == 1)
                {
                    ChartNodesGraph _next = (ChartNodesGraph)_children[0];
                    _next.MoveTree(_startX - _next._startX);
                }  // else if Count == 1
            } // if IsHorizontal
            else
            {
                switch (GraphAlignment)
                {
                    case GraphAlignment.VerticalLeft:
                        foreach (ChartNodesGraph cng in _children)
                            cng.MoveTree(MiddleX - cng.GetMaxX() - cng._gapHorizontal);
                        break;
                    case GraphAlignment.VerticalRight:
                        foreach (ChartNodesGraph cng in _children)
                            cng.MoveTree(MiddleX - cng.GetMinX() + cng._gapHorizontal);
                        break;
                }
            } // else if IsHorizontal

            if (IsAssistant)
            {
                if (IsRight)
                {
                    MoveTree(_parent.MiddleX - GetMinX() + _gapHorizontal);
                }
                else
                {
                    MoveTree(_parent.MiddleX - GetMaxX() - _gapHorizontal);
                }
            }
        }

        internal void ResetPositionX(int startX)
        {
            _startX = startX;
            int sign = (GraphAlignment == GraphAlignment.VerticalLeft) ? -1 : 1;
            if (IsHorizontal)
            {
                if (Count > 1)
                {
                    int childWidth = 0;

                    for (int i = 0; i < _children.Count; i++)
                    {
                        ChartNodesGraph cng = (ChartNodesGraph)_children[i];
                        if (i > 0)
                        {
                            childWidth += ((ChartNodesGraph)_children[i - 1])._gapHorizontal;
                        }
                        childWidth += cng.GetWidth();
                    }

                    int firstX = MiddleX - childWidth / 2;

                    for (int i = 0; i < Count; i++)
                    {
                        ChartNodesGraph cng = (ChartNodesGraph)_children[i];
                        if (i > 0)
                        {
                            ChartNodesGraph prevCng = (ChartNodesGraph)_children[i];
                            firstX += prevCng.GetWidth() / 2 + prevCng._gapHorizontal;
                        }
                        firstX += cng.GetWidth() / 2;
                        cng.ResetPositionX(firstX);
                    }
                }
                else if (Count == 1)
                {
                    ChartNodesGraph firstChild = (ChartNodesGraph)_children[0];
                    firstChild.ResetPositionX(MiddleX);
                }
            }
            else
            {
                foreach (ChartNodesGraph cng in _children)
                {
                    cng.ResetPositionX(MiddleX + (sign * (cng.GetWidth() / 2 + cng._gapHorizontal)));
                }
            }

            //Reset Assistant position X
            foreach (ChartNodesGraph cng in _assistants)
            {
                sign = (cng.IsRight) ? 1 : -1;
                cng.ResetPositionX(MiddleX + (sign * (cng.GetWidth() / 2 + cng._gapHorizontal)));
            }
        }

        internal void ResetPositionY(int startY)
        {
            _startY = startY;

            //Reset Assistants position Y
            if (CountAssistants > 0)
            {
                int evenHeight = _startY + OuterHeight + GetPiktoHeight() + _gapVertical + GetEmployeeHeight();
                int oddHeight = evenHeight;
                for (int i = 0; i < CountAssistants; i++)
                {
                    ChartNodesGraph grp = (ChartNodesGraph)_assistants[i];
                    if (i % 2 == 0)
                    {
                        grp.ResetPositionY(evenHeight);
                        evenHeight += grp.GetHeight() + grp._gapVertical;
                    }
                    else
                    {
                        grp.ResetPositionY(oddHeight);
                        oddHeight += grp.GetHeight() + grp._gapVertical;
                    }
                }
            }

            int firstHight = _startY + OuterHeight + GetPiktoHeight() + _gapVertical + GetEmployeeHeight() + (GetAssistantHeight() > 0 ? GetAssistantHeight() + _gapVertical : 0);
            if (IsHorizontal)
            {
                foreach (ChartNodesGraph cng in _children)
                    cng.ResetPositionY(firstHight + cng._gapVertical);
            }
            else
            {
                foreach (ChartNodesGraph cng in _children)
                {
                    cng.ResetPositionY(firstHight);
                    firstHight += cng.GetHeight() + cng._gapVertical;
                }
            }
        }
        #endregion

        #region Public methods to determine width, height, min. and max. image size (x, y)
        public int GetEmployeeWidth()
        {
            return (HasEmployees) ? _employeeWidth : 0; //this is not the effective width, because it depends on the string-length, see DrawEmployees().
        }

        public int GetEmployeeHeight()
        {
            return (HasEmployees) ? _employeeHeight * _node.EmployeeCount + _employeeDeltaHeight * (_node.EmployeeCount - 1) : 0;
        }

        public int GetEmployeeMinX()
        {
            int min = OuterLeftX;
            bool isL = IsLeftAlignment;
            if ((_children.Count == 0) && (_assistants.Count == 0))
            {
                isL = false;
            }
            if (isL)
                min = Math.Min(min, OuterRightX - _verticalAlignOffset - (GetEmployeeWidth() + _employeeDeltaWidth));
            return min;
        }

        public int GetEmployeeMaxX()
        {
            int max = OuterRightX;
            bool isL = IsLeftAlignment;
            if ((_children.Count == 0) && (_assistants.Count == 0))
            {
                isL = false;
            }
            if (!isL)
                max = Math.Max(max, OuterLeftX + _verticalAlignOffset + GetEmployeeWidth() + _employeeDeltaWidth);
            return max;
        }

        public int GetPiktoHeight()
        {
            int max = 0;
            if (_node != null)
            {
                for (int i = 0; i < _node.PiktoCount; i++)
                {
                    ChartPiktoLayout cpl = _node.GetPikto(i).ChartPiktoLayout;
                    max = Math.Max(max, cpl != null ? cpl.Height : _piktoHeight);
                }
            }
            return max;
        }

        public int GetPiktoHeightAt(int posX)
        {
            int height = 0;
            if (_node != null)
            {
                int x = OuterLeftX;
                for (int i = 0; i < _node.PiktoCount; i++)
                {
                    ChartPiktoLayout cpl = _node.GetPikto(i).ChartPiktoLayout;
                    x += (cpl != null ? cpl.Width : _piktoWidth) - 1;
                    if (x >= posX)
                    {
                        height = cpl != null ? cpl.Height : _piktoHeight;
                        break;
                    }
                }
            }
            return height;
        }

        public int GetAssistantHeight()
        {
            int max = 0;
            if (CountAssistants > 0)
            {
                int odd = 0;
                int even = 0;
                for (int i = 0; i < CountAssistants; i++)
                {
                    ChartNodesGraph grp = (ChartNodesGraph)_assistants[i];
                    if (i % 2 == 0)
                    {
                        if (i > 1)
                            even += ((ChartNodesGraph)_assistants[i - 2])._gapVertical;
                        even += grp.GetHeight();
                    }
                    else
                    {
                        if (i > 1)
                            odd += ((ChartNodesGraph)_assistants[i - 2])._gapVertical;
                        odd += grp.GetHeight();
                    }
                }
                max = Math.Max(even, odd);
            }
            return max;
        }

        public int GetAssistantMinX()
        {
            int min = int.MaxValue;
            foreach (ChartNodesGraph cng in _assistants)
                min = Math.Min(min, cng.GetMinX());
            return min;
        }

        public int GetAssistantMaxX()
        {
            int max = int.MinValue;
            foreach (ChartNodesGraph cng in _assistants)
                max = Math.Max(max, cng.GetMaxX());
            return max;
        }

        public int GetAssistantMinY()
        {
            int min = int.MaxValue;
            foreach (ChartNodesGraph cng in _assistants)
                min = Math.Min(min, cng.GetMinY());
            return min;
        }

        public int GetAssistantMaxY()
        {
            int max = 0;
            foreach (ChartNodesGraph cng in _assistants)
                max = Math.Max(max, cng.GetMaxY());
            return max;
        }

        public int GetWidth()
        {
            return GetMaxX() - GetMinX();
        }

        public int GetHeight()
        {
            return GetMaxY() - GetMinY();
        }

        public int GetMinX()
        {
            int min = Math.Min(OuterLeftX, GetAssistantMinX());
            foreach (ChartNodesGraph cng in _children)
                min = Math.Min(min, cng.GetMinX());
            min = Math.Min(GetEmployeeMinX(), min);
            return min;
        }

        public int GetMaxX()
        {
            int max = Math.Max(OuterRightX, GetAssistantMaxX());
            max = Math.Max(max, GetEmployeeMaxX());
            foreach (ChartNodesGraph cng in _children)
                max = Math.Max(max, cng.GetMaxX());
            return max;
        }

        public int GetMinY()
        {
            int min = Math.Min(OuterTopY, GetAssistantMinY());
            foreach (ChartNodesGraph cng in _children)
                min = Math.Min(min, cng.GetMinY());
            return min;
        }

        public int GetMaxY()
        {
            int max = Math.Max(OuterBottomY + GetPiktoHeight() + GetEmployeeHeight(), GetAssistantMaxY());
            foreach (ChartNodesGraph cng in _children)
                max = Math.Max(max, cng.GetMaxY());
            return max;
        }



        public int GetDrawingWidth()
        {
            return GetMaxX() - GetMinX();
        }

        public int GetDrawingHeight()
        {
            return GetMaxY() - GetMinY();
        }
        #endregion

        #region Public methods to get and set image offset vector
        public int OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                foreach (ChartNodesGraph cng in _children)
                    cng.OffsetX = value;
                foreach (ChartNodesGraph cng in _assistants)
                    cng.OffsetX = value;
            }
        }

        public int OffsetY
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                foreach (ChartNodesGraph cng in _children)
                    cng.OffsetY = value;
                foreach (ChartNodesGraph cng in _assistants)
                    cng.OffsetY = value;
            }
        }
        #endregion

        #region Points used to draw rectangles and linies
        public int OuterLeftX
        {
            get { return _startX - OuterWidth / 2 + _offsetX; }
        }

        public int InnerLeftX
        {
            get { return OuterLeftX + _lineWidth; }
        }

        public int OuterRightX
        {
            get { return OuterLeftX + OuterWidth; }
        }

        public int InnerRightX
        {
            get { return OuterRightX - _lineWidth; }
        }

        public int OuterTopY
        {
            get { return _startY + _offsetY; }
        }

        public int InnerTopY
        {
            get { return OuterTopY + _lineWidth; }
        }

        public int OuterBottomY
        {
            get { return OuterTopY + OuterHeight; }
        }

        public int InnerBottomY
        {
            get { return OuterBottomY - _lineWidth; }
        }

        public int OuterWidth
        {
            get { return _width + _lineWidth; }
        }

        public int OuterHeight
        {
            get { return _height + _lineWidth; }
        }

        public int InnerWidth
        {
            get { return _width - _lineWidth; }
        }

        public int InnerHeight
        {
            get { return _height - _lineWidth; }
        }

        public int MiddleX
        {
            get
            {
                switch (GraphAlignment)
                {
                    case GraphAlignment.HorizontalCenter:
                    case GraphAlignment.HorizontalLeft:
                    case GraphAlignment.HorizontalRight:
                        return TopMiddleX;
                    case GraphAlignment.VerticalLeft:
                        return OuterRightX - _verticalAlignOffset;
                    case GraphAlignment.VerticalRight:
                        return OuterLeftX + _verticalAlignOffset;
                }
                return TopMiddleX;
            }
        }

        public int TopMiddleX
        {
            get { return OuterLeftX + OuterWidth / 2; }
        }

        public int MiddleY
        {
            get { return OuterTopY + OuterHeight / 2; }
        }

        public Rectangle DrawRectangle
        {
            get { return new Rectangle(OuterLeftX + _lineWidth / 2, OuterTopY + _lineWidth / 2, _width, _height); }
        }
        #endregion

        #region Points and method to draw lines (Vertical and horizontal)
        private Point MiddleXTop
        {
            get { return new Point(TopMiddleX, OuterTopY); }
        }

        private Point MiddleXParent
        {
            get { return new Point(TopMiddleX, OuterTopY - _gapVertical); }
        }
        private Point MiddleXParentBottom
        {
            get { return new Point(TopMiddleX, OuterBottomY + _gapVertical); }
        }

        private Point MiddleXBottom
        {
            get { return new Point(MiddleX, OuterBottomY + GetPiktoHeightAt(MiddleX)); }
        }

        private Point MiddleXChild
        {
            get { return new Point(MiddleX, OuterBottomY + _gapVertical); }
        }

        private Point MiddleYLeft
        {
            get { return new Point(OuterLeftX, MiddleY); }
        }

        private Point MiddleYRight
        {
            get { return new Point(OuterRightX, MiddleY); }
        }

        private Point MiddleYParent
        {
            get { return new Point(_parent.MiddleX, MiddleY); }
        }

        private void DrawLineToParent(Graphics graph, VisioElement thisElement)
        {
            // Draw vertical line | on top
            if ((_parent != null) && (_parent._parent != null))
            {
                if (IsAssistant)
                {
                    if (IsRight)
                    {
                        graph.DrawLine(_penLine, MiddleYParent, MiddleYLeft);
                        AddLineVisio(MiddleYParent, MiddleYLeft, thisElement);
                    }
                    else
                    {
                        graph.DrawLine(_penLine, MiddleYRight, MiddleYParent);
                        AddLineVisio(MiddleYRight, MiddleYParent, thisElement);
                    }
                }


                else
                {
                    switch (_parent.GraphAlignment)
                    {
                        case GraphAlignment.HorizontalCenter:
                        case GraphAlignment.HorizontalLeft:
                        case GraphAlignment.HorizontalRight:
                            if (Global.isModuleEnabled("spz"))
                            {
                                if (GraphAlignment == GraphAlignment.VerticalRight)
                                    graph.DrawLine(_penLine, MiddleXParentBottom, new Point(MiddleXParentBottom.X, MiddleXBottom.Y));
                                else
                                    graph.DrawLine(_penLine, MiddleXBottom, MiddleXParentBottom);
                            }
                            else
                            {
                                graph.DrawLine(_penLine, MiddleXTop, MiddleXParent);
                                AddLineVisio(MiddleXTop, MiddleXParent, thisElement);
                            }
                            break;
                        case GraphAlignment.VerticalLeft:
                            graph.DrawLine(_penLine, MiddleYRight, MiddleYParent);
                            AddLineVisio(MiddleYRight, MiddleYParent, thisElement);
                            break;
                        case GraphAlignment.VerticalRight:
                            graph.DrawLine(_penLine, MiddleYParent, MiddleYLeft);
                            
                            AddLineVisio(MiddleYParent, MiddleYLeft, thisElement);
                            break;
                    }
                }

                //Drawing lines between assistants and parent
                if (CountAssistants > 0)
                {
                    graph.DrawLine(_penLine, MiddleXBottom, MiddleXChild);
                    foreach (ChartNodesGraph cng in _assistants)
                    {
                        if (cng.IsRight)
                        {
                            graph.DrawLine(_penLine, MiddleXBottom, cng.MiddleYParent);
                            AddLineVisio(MiddleXBottom, cng.MiddleYParent, thisElement);

                        }
                        else
                        {
                            graph.DrawLine(_penLine, MiddleXBottom, cng.MiddleYParent);
                            AddLineVisio(MiddleXBottom, cng.MiddleYParent, thisElement);
                        }
                    }
                }
            }
        }

        private void AddLineVisio(Point start, Point end, VisioElement thisElement)
        {
            VisioLine thisLine = new VisioLine();

            thisLine.X1 = start.X;
            thisLine.Y1 = start.Y;
            thisLine.X2 = end.X;
            thisLine.Y2 = end.Y;
            thisElement.Line.Add(thisLine);


        }


        private void DrawHorizontalLines(Graphics graph, VisioElement thisElement)
        {
            // Draw vertical line | on bottom (SPZ top)
            if (Count > 0)
            {
                ChartNodesGraph cng = (ChartNodesGraph)_children[0];
                if (Global.isModuleEnabled("spz"))
                {
                    graph.DrawLine(_penLine, MiddleXTop, new Point(MiddleX, cng.MiddleXParentBottom.Y));
                    AddLineVisio(MiddleXTop, new Point(MiddleX, cng.MiddleXParentBottom.Y), thisElement);
                }
                else
                {
                    graph.DrawLine(_penLine, MiddleXBottom, new Point(MiddleX, cng.MiddleXParent.Y));
                    AddLineVisio(MiddleXBottom, new Point(MiddleX, cng.MiddleXParent.Y), thisElement);
                }


                
                
                
                //graph.DrawLine(_penLine, MiddleXBottom, MiddleXChild);

                // Draw horizontal line between children
                if (Count > 1)
                {
                    ChartNodesGraph grp1 = null;
                    ChartNodesGraph grp2 = null;

                    for (int i = 0; i < Count - 1; i++)
                    {
                        grp1 = (ChartNodesGraph)_children[i];
                        grp2 = (ChartNodesGraph)_children[i + 1];
                        if (Global.isModuleEnabled("spz"))
                        {
                            graph.DrawLine(_penLine, grp1.MiddleXParentBottom, grp2.MiddleXParentBottom);
                            AddLineVisio(grp1.MiddleXParentBottom, grp2.MiddleXParentBottom, thisElement);
                        }
                        else
                        {
                            graph.DrawLine(_penLine, grp1.MiddleXParent, grp2.MiddleXParent);
                            AddLineVisio(grp1.MiddleXParent, grp2.MiddleXParent, thisElement);
                        }
                    }
                    grp1 = (ChartNodesGraph)_children[Count - 2];
                    grp2 = (ChartNodesGraph)_children[Count - 1];
                    if (Global.isModuleEnabled("spz"))
                    {
                        graph.DrawLine(_penLine, grp1.MiddleXParentBottom, grp2.MiddleXParentBottom);
                        AddLineVisio(grp1.MiddleXParentBottom, grp2.MiddleXParentBottom, thisElement);
                    }
                    else
                    {
                        graph.DrawLine(_penLine, grp1.MiddleXParent, grp2.MiddleXParent);
                        AddLineVisio(grp1.MiddleXParent, grp2.MiddleXParent, thisElement);
                    }

                }
                /*
                else if (Count == 1)
                {
                    //Draw line between one child and his parent

                    cng = (ChartNodesGraph)_children[0];
                    graph.DrawLine(_penLine, MiddleXBottom, cng.MiddleXParent);
                }
                */
            }
        }

        private void DrawVerticalLeftLines(Graphics graph, VisioElement thisElement)
        {
            foreach (ChartNodesGraph cng in _children)
            {
                graph.DrawLine(_penLine, MiddleXBottom, cng.MiddleYParent);
                AddLineVisio(MiddleXBottom, cng.MiddleYParent, thisElement);
            }
        }

        private void DrawVerticalRightLines(Graphics graph, VisioElement thisElement)
        {
            foreach (ChartNodesGraph cng in _children)
            {
                if (Global.isModuleEnabled("spz"))
                {
                    graph.DrawLine(_penLine, new Point(MiddleXBottom.X, MiddleXTop.Y), cng.MiddleYParent);
                    AddLineVisio(new Point(MiddleXBottom.X, MiddleXTop.Y), cng.MiddleYParent, thisElement);
                }
                else
                {
                    graph.DrawLine(_penLine, MiddleXBottom, cng.MiddleYParent);
                    AddLineVisio(MiddleXBottom, cng.MiddleYParent, thisElement);
                }
            }
        }
        #endregion

        #region Public and private methods use to draw image, Draw, DrawText, DrawPikto

        public bool isExternalUrl(String url)
        {
            String tmp = url.ToLower();
            if (tmp.StartsWith("http://"))
            {
                return true;
            }
            else if (tmp.StartsWith("https://"))
            {
                return true;
            }
            else if (tmp.StartsWith("ftp://"))
            {
                return true;
            }
            return false;
        }

        private string createAreaElement(string link, string target, string tooltip, int left, int top, int right, int bottom)
        {
            StringBuilder areaBuilder = new StringBuilder(256);
            areaBuilder.Append("<area href=\"");
            if ((!isExternalUrl(link)) && (!link.StartsWith(Global.Config.baseURL)))
            {
                areaBuilder.Append(Global.Config.baseURL);
            }
            areaBuilder.Append(link.Replace("%25ID", "%ID").Replace("%ID", _node.OrgentityId.ToString()).Replace("%25CLIPBOARD_ID", "%CLIPBOARD_ID").Replace("%CLIPBOARD_ID", _node.ClipboardId.ToString()));
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

        private string createAreaElement(long nodeId, int left, int top, int right, int bottom)
        {
            StringBuilder areaBuilder = new StringBuilder(256);
            areaBuilder.Append("<area ");
            areaBuilder.Append("href=\"javascript: OrgentityClicked("+ nodeId +");");
            areaBuilder.Append("\" shape=\"rect\" coords=\"");
            areaBuilder.Append(left);
            areaBuilder.Append(",");
            areaBuilder.Append(top);
            areaBuilder.Append(",");
            areaBuilder.Append(right);
            areaBuilder.Append(",");
            areaBuilder.Append(bottom);
            areaBuilder.Append("\" id=\"");
            areaBuilder.Append(nodeId);
            areaBuilder.Append("\">\n");
            return areaBuilder.ToString();
        }

        private ArrayList ResolveText(string textMnemo)
        {
            ArrayList texts = null;
            if (Chart.dbData.getLookupText(textMnemo) != null)
            {
                //lookup definition by LOOKUP table
                texts = Chart.dbData.lookupMnemo(textMnemo, "%ID", _node.OrgentityId.ToString(), "ID");
            }
            else
            {
                texts = new ArrayList();
                texts.Add(new string[] { textMnemo });
            }
            return texts;
        }

        private void DrawText(Graphics graph, VisioElement thisElement)
        {
            if (_node == null)
                return;

            System.Drawing.StringFormat format = new StringFormat();
            format.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;

            string drawingString = "";
            int sy = _node.Layout.PaddingTop;
            for (int i = 0; i < _node.TextCount; i++)
            {
                if (sy < InnerHeight)
                {
                    ChartText txt = _node.GetText(i);
                    Font font = txt.ChartTextLayout.Font;
                    switch (txt.ChartTextLayout.HorizontalAlign)
                    {
                        case ChartTextLayout.HorizontalAlignments.Center:
                            format.Alignment = System.Drawing.StringAlignment.Center;
                            break;

                        case ChartTextLayout.HorizontalAlignments.Left:
                            format.Alignment = System.Drawing.StringAlignment.Near;
                            break;

                        case ChartTextLayout.HorizontalAlignments.Right:
                            format.Alignment = System.Drawing.StringAlignment.Far;
                            break;
                    }
                    System.Drawing.Brush brushText = new System.Drawing.SolidBrush(txt.ChartTextLayout.FontColor);

                    if ((txt.Text != null) && (txt.Text != ""))
                    {
                        ArrayList texts = ResolveText(txt.Text);
                        if (texts != null)
                        {
                            int index = 0;
                            foreach (string[] text in texts)
                            {
                                drawingString = text[0];

                                // show engagement if enabled
                                if (Global.Config.showEngagement)
                                {
                                    if (txt.Text == "LEITERABTEILUNG" && text[1] != "")
                                    {
                                        object engagement = Chart.dbData.lookup("JOB.ENGAGEMENT", "PERSON INNER JOIN EMPLOYMENT ON PERSON.ID = EMPLOYMENT.PERSON_ID INNER JOIN JOB ON EMPLOYMENT.ID = JOB.EMPLOYMENT_ID INNER JOIN ORGENTITY ON JOB.ORGENTITY_ID = ORGENTITY.ID", "PERSON.ID = " + text[1] + " AND ORGENTITY.ID = " + _node.OrgentityId);
                                        drawingString += " " + engagement + "%";
                                    }
                                }

                                int stringWidth = InnerWidth - _node.Layout.PaddingLeft - _node.Layout.PaddingRight;
                                int stringX = InnerLeftX + _node.Layout.PaddingLeft;
                                int stringY = InnerTopY + sy;
                                Size strSize = graph.MeasureString(drawingString, font, stringWidth, format).ToSize();
                                Rectangle rect = new Rectangle(stringX, stringY, stringWidth, strSize.Height + 1);
                                // draw employee name
                                graph.DrawString(drawingString, font, brushText, rect, format);

                                if ((txt.Link != null) && (txt.Link != ""))
                                {
                                    _textMapInfo.Append(createAreaElement(txt.Link.Replace("%25INDEX", "%INDEX").Replace("%INDEX", index.ToString()), txt.TargetFrame, drawingString, stringX, stringY, stringX + stringWidth, stringY + strSize.Height + 1));
                                }
                                sy += strSize.Height + 2;
                                index++;

                                //add text to VisioElement
                                //TODO: set font, style etc. (can also be got from here)

                                int yKorrektur = 4;
                                VisioText thisText = new VisioText();
                                thisText.Text = drawingString;
                                thisText.X1 = stringX - 4;
                                thisText.X2 = stringX + _node.Layout.NodeWidth - 6;
                                thisText.Y1 = stringY - strSize.Height + yKorrektur;
                                thisText.Y2 = stringY + yKorrektur;
                                thisText.Font.Name = font.Name;
                                thisText.Font.Size = (int)font.Size;
                                thisText.Font.Color.R = txt.ChartTextLayout.FontColor.R;
                                thisText.Font.Color.G = txt.ChartTextLayout.FontColor.G;
                                thisText.Font.Color.B = txt.ChartTextLayout.FontColor.B;



                                thisElement.Text.Add(thisText);
                            }
                        }
                    }
                }
                else
                    break;
            }
        }

        private void DrawPikto(Graphics graph)
        {
            if (_node == null)
                return;

            int leftX = OuterLeftX;
            int topY = OuterBottomY;

            for (int i = 0; i < _node.PiktoCount; i++)
            {
                ChartPikto pikto = _node.GetPikto(i);
                System.Drawing.Brush lineBrush = System.Drawing.Brushes.Black;
                System.Drawing.Pen linePen = new System.Drawing.Pen(lineBrush, 2);
                System.Drawing.Brush backgroundBrush = System.Drawing.Brushes.White;
                System.Drawing.Image image = null;
                int width = _piktoWidth;
                int height = _piktoHeight;
                bool drawRectangleOutline = true;

                if (pikto.ChartPiktoLayout != null)
                {
                    backgroundBrush = new System.Drawing.SolidBrush(pikto.ChartPiktoLayout.BackgroundColor);
                    if (pikto.ChartPiktoLayout.LineWidth > 0)
                    {
                        lineBrush = new System.Drawing.SolidBrush(pikto.ChartPiktoLayout.LineColor);
                        linePen = new System.Drawing.Pen(lineBrush, pikto.ChartPiktoLayout.LineWidth);
                    }
                    else
                    {
                        drawRectangleOutline = false;
                    }
                    image = pikto.ChartPiktoLayout.Image;
                    width = pikto.ChartPiktoLayout.Width;
                    height = pikto.ChartPiktoLayout.Height;
                }

                // Drawing image & rectangle
                Rectangle rectangle = new Rectangle(leftX, topY, width, height);
                if (image != null)
                {
                    graph.DrawImage(new Bitmap(image, rectangle.Size), rectangle);
                }
                rectangle.Width -= 1;
                rectangle.Height -= 1;
                if (image == null)
                {
                    graph.FillRectangle(backgroundBrush, rectangle);
                }
                if (drawRectangleOutline)
                {
                    graph.DrawRectangle(linePen, rectangle);
                }

                if (pikto.Link != null && pikto.Link != "")
                {
                    string toolTip = "";
                    ArrayList texts = ResolveText(pikto.Text);
                    if (texts != null)
                    {
                        bool isFirst = true;
                        foreach (string[] text in texts)
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                            }
                            else
                            {
                                toolTip += ", ";
                            }
                            toolTip += text[0];
                        }
                    }
                    _piktoMapInfo.Append(createAreaElement(pikto.Link.Replace("%25INDEX", "%INDEX").Replace("%INDEX", ""), pikto.TargetFrame, toolTip, rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height));
                }

                leftX += width - 1;
            }
        }

        private void DrawEmployees(Graphics graph, VisioElement thisElement)
        {
            if (!HasEmployees)
                return;

            System.Drawing.Brush brushText = new System.Drawing.SolidBrush(Color.Black);
            System.Drawing.StringFormat format = new StringFormat();
            format.Alignment = System.Drawing.StringAlignment.Near;
            format.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;

            Point begin;
            if (Global.isModuleEnabled("spz"))
            {
                begin = new Point(MiddleX, OuterTopY - _employeeHeight);
            }
            else
            {
                begin = new Point(MiddleX, OuterBottomY + GetPiktoHeight());
            }
            
            bool isL = IsLeftAlignment;
            if ((_children.Count == 0) && (_assistants.Count == 0))
            {
                begin = new Point(OuterLeftX + _verticalAlignOffset, begin.Y);
                isL = false;
            }
            int sign = (isL) ? -1 : 1;
            for (int i = 0; i < _node.EmployeeCount; i++)
            {
                ChartEmployee emp = _node.GetEmployee(i);

                float fontSize = 8;
                if (Global.Config.showEngagement)
                {
                    fontSize = 7;
                }

                int employeeWidth = graph.MeasureString(emp.Title, new Font("arial", fontSize, FontStyle.Regular), Math.Max(_employeeWidth, isL ? begin.X - GetMinX() - _employeeDeltaWidth : GetMaxX() - begin.X - _employeeDeltaWidth), format).ToSize().Width + 2;
                Rectangle rect;
                if (Global.isModuleEnabled("spz"))
                {
                    rect = new Rectangle(isL ? begin.X - _employeeDeltaWidth - employeeWidth : begin.X + _employeeDeltaWidth,
                        begin.Y - _employeeDeltaHeight - (_employeeHeight + _employeeDeltaHeight) * i,
                        employeeWidth, _employeeHeight);
                }
                else
                {
                    rect = new Rectangle(isL ? begin.X - _employeeDeltaWidth - employeeWidth : begin.X + _employeeDeltaWidth,
                        begin.Y + _employeeDeltaHeight + (_employeeHeight + _employeeDeltaHeight) * i,
                        employeeWidth, _employeeHeight);
                }


                // draw employee name
                graph.DrawString(emp.Title, new Font("arial", fontSize, FontStyle.Regular), brushText, rect, format);

                int bx = begin.X;
                int by;
                if (Global.isModuleEnabled("spz"))
                    by = begin.Y - _employeeDeltaHeight - (_employeeHeight + _employeeDeltaHeight) * i ;
                else
                    by = begin.Y + _employeeDeltaHeight + (_employeeHeight + _employeeDeltaHeight) * i;
                
                // draw employee connection line
                // horisontal line
                graph.DrawLine(_penLine, bx, by + _employeeHeight / 2, bx + (sign * _employeeDeltaWidth), by + _employeeHeight / 2);

                // vertical line
                if (Global.isModuleEnabled("spz"))
                graph.DrawLine(_penLine, bx, begin.Y +15, bx, by + _employeeHeight / 2);
                else
                    graph.DrawLine(_penLine, bx, begin.Y, bx, by + _employeeHeight / 2);

                _employeeMapInfo.Append(createAreaElement(psoft.Person.DetailFrame.GetURL("mode", "OE", "xID", "%ID", "UID", emp.Uid), _targetFrame, emp.Title, bx + (sign * _employeeDeltaWidth), by, bx + (sign * (_employeeDeltaWidth + employeeWidth)), by + _employeeHeight));

                // save info to array for visio export
                // TODO: add style information
                VisioEmployee vEmp = new VisioEmployee();
                int xKorrektur = 16;
                int yKorrektur = -29;
                vEmp.Name = emp.Title;
                VisioText vText = new VisioText();
                vText.Text = emp.Title;
                vText.X1 = begin.X + xKorrektur;
                vText.X2 = begin.X + employeeWidth + xKorrektur + 20;
                vText.Y1 = by - _employeeHeight / 2 + yKorrektur;
                vText.Y2 = by + _employeeHeight / 2 + yKorrektur;
                vText.HorizontalStartX = bx + xKorrektur;
                vText.HorizontalStartY = by + yKorrektur;
                vText.HorizontalEndX = bx - (sign * _employeeDeltaWidth) + xKorrektur;
                vText.HorizontalEndY = by + yKorrektur;
                vText.VerticalStartX = bx - (sign * _employeeDeltaWidth) + xKorrektur;
                vText.VerticalStartY = begin.Y + yKorrektur - 7;
                vText.VerticalEndX = bx - (sign * _employeeDeltaWidth) + xKorrektur;
                vText.VerticalEndY = by + _employeeHeight / 2 + yKorrektur - 7;
                vEmp.Text = vText;
                vText.Font.Size = (int)fontSize;
                vText.Font.Name = "arial";
                vText.Font.Color.R = ((System.Drawing.SolidBrush)(brushText)).Color.R;
                vText.Font.Color.G = ((System.Drawing.SolidBrush)(brushText)).Color.G;
                vText.Font.Color.B = ((System.Drawing.SolidBrush)(brushText)).Color.B;


                thisElement.Employees.Add(vEmp);


            }
        }

        
        public void Draw(Graphics graph, List<VisioElement> elements)
        {

            if (_parent == null && Global.isModuleEnabled("spz"))
            {
                _maxYChart = GetMaxYChart(_children);
                _minYChart = 0; // GetMinYChart(_children);
                SetNewYCoordinates(_children);
            }
            
            if (_parent != null)
            {
                VisioElement thisElement = new VisioElement();
                thisElement.Id = _node.Id;
                thisElement.X1 = DrawRectangle.X;
                thisElement.Y1 = DrawRectangle.Y - DrawRectangle.Height;
                thisElement.X2 = DrawRectangle.X + DrawRectangle.Width;
                thisElement.Y2 = DrawRectangle.Y;


                System.Drawing.Brush lineBrush = (IsAssistant) ? System.Drawing.Brushes.Gray : System.Drawing.Brushes.Black;
                System.Drawing.Pen linePen = new System.Drawing.Pen(lineBrush, (IsAssistant) ? 1 : 2);
                System.Drawing.Brush backgroundBrush = System.Drawing.Brushes.White;

                // Setting drawing variables
                ChartNodeLayout layout = null;
                System.Drawing.Image image = null;
                if (_node != null)
                    layout = _node.Layout;
                else if (_chart != null)
                    layout = _chart.ChartNodeLayout;

                if (layout != null)
                {
                    image = layout.Image;
                    lineBrush = new System.Drawing.SolidBrush(layout.LineColor);
                    linePen = new System.Drawing.Pen(lineBrush, layout.LineWidth);
                    backgroundBrush = new System.Drawing.SolidBrush(layout.BackgroundColor);
                }

                // Drawing rectangle or image background
                if (image != null)
                    graph.DrawImage(new Bitmap(image, DrawRectangle.Size), DrawRectangle);
                else
                {
                    graph.FillRectangle(backgroundBrush, DrawRectangle);
                    graph.DrawRectangle(linePen, DrawRectangle);
                }

                // Drawing text & pikto
                if (_node != null)
                {
                    DrawText(graph, thisElement);
                    DrawPikto(graph);
                }


                // Drawing line to parent node
                DrawLineToParent(graph, thisElement);

                // Drawing children lines
                switch (GraphAlignment)
                {
                    case GraphAlignment.HorizontalCenter:
                    case GraphAlignment.HorizontalLeft:
                    case GraphAlignment.HorizontalRight:
                        DrawHorizontalLines(graph, thisElement);
                        break;
                    case GraphAlignment.VerticalLeft:
                        DrawVerticalLeftLines(graph, thisElement);
                        break;
                    case GraphAlignment.VerticalRight:
                        DrawVerticalRightLines(graph, thisElement);
                        break;
                }

                // Drawing employees
                DrawEmployees(graph, thisElement);

                elements.Add(thisElement);
            }

            // Drawing children
            foreach (ChartNodesGraph cng in _children)
            {
                cng._maxYChart = cng.Parent._maxYChart;
                cng._minYChart = cng.Parent._minYChart;
                cng.Draw(graph, elements);
            }

            // Drawing assistants
            foreach (ChartNodesGraph cng in _assistants)
                cng.Draw(graph, elements);
        }

        private void SetNewYCoordinates(ArrayList children)
        {
            foreach (ChartNodesGraph cng in children)
            {
                int newY = 0;
                if (cng.DrawRectangle.Y > _maxYChart / 2)
                {
                    //newY = (_maxYChart - cng.DrawRectangle.Y + _minYChart);
                    //cng._startY -= (DrawRectangle.Y - newY);
                    newY = _maxYChart / 2 - (cng.DrawRectangle.Y - _maxYChart / 2 - 2 * cng.OuterHeight);
                    cng._startY = newY;
                    //cng._offsetY = 82; // cng._offsetY / 2;
                }
                else
                {
                    newY = (_maxYChart / 2 - cng.DrawRectangle.Y + _maxYChart / 2 + _minYChart);
                    cng._startY += newY - cng.DrawRectangle.Y;
                    //cng._offsetY = 82; //cng._offsetY / 2;
                }
                if (cng._children.Count > 0)
                {
                    SetNewYCoordinates(cng._children);
                }
            }
        }

        private int max = 0;
        public int GetMaxYChart(ArrayList children)
        {
            foreach (ChartNodesGraph cng in children)
            {
                max = Math.Max(max, cng._startY + cng.GetEmployeeHeight());

                    GetMaxYChart(cng._children);
                
            }

            return max;
        }
        //private int min = 0;
        //private int maxY = 0;
        //public int GetMinYChart(ArrayList children)
        //{
        //    foreach (ChartNodesGraph cng in children)
        //    {
        //        if (cng._children.Count == 0 && cng.DrawRectangle.Y + cng.DrawRectangle.Height + cng.GetEmployeeHeight() > maxY)
        //        {
        //            maxY = cng.DrawRectangle.Y + cng.DrawRectangle.Height + cng.GetEmployeeHeight();
        //            if(cng.DrawRectangle.Y + cng.DrawRectangle.Height + cng.GetEmployeeHeight() > maxY || min == 0 )
        //             min = cng.GetEmployeeHeight();
        //        }
        //        if (cng._children.Count > 0)
        //        {
        //            GetMinYChart(cng._children);
        //        }
        //    }

        //    return min;
        //}
        #endregion

        #region Public method to append image map information
        public void AppendImageMapInfo(StringBuilder imageMap)
        {
            if (_node != null)
            {
                imageMap.Append(_employeeMapInfo);
                imageMap.Append(_textMapInfo);
                imageMap.Append(_piktoMapInfo);
            }

            foreach (ChartNodesGraph cng in _children)
                cng.AppendImageMapInfo(imageMap);
            foreach (ChartNodesGraph cng in _assistants)
                cng.AppendImageMapInfo(imageMap);
        }

        private StringBuilder _orgentityMapInfo = new StringBuilder(1024);
        public void AppendImageMapInfoAdmin(StringBuilder imageMap)
        {
            if (_node != null)
            {
                imageMap.Append(_orgentityMapInfo);
                //_orgentityMapInfo.Append(createAreaElement("", "", "", _node.));
            }

            foreach (ChartNodesGraph cng in _children)
            {
                _orgentityMapInfo.Append(createAreaElement(cng._node.Id,cng.DrawRectangle.Left, cng.DrawRectangle.Top, cng.DrawRectangle.Right, cng.DrawRectangle.Bottom));
                imageMap.Append(_orgentityMapInfo);
                cng.AppendImageMapInfoAdmin(imageMap);
            }
            foreach (ChartNodesGraph cng in _assistants)
            {
                _orgentityMapInfo.Append(createAreaElement(cng._node.Id, cng.DrawRectangle.Left, cng.DrawRectangle.Top, cng.DrawRectangle.Right, cng.DrawRectangle.Bottom));
                imageMap.Append(_orgentityMapInfo);
                cng.AppendImageMapInfoAdmin(imageMap);
            }
            
        }
        #endregion
    }
    #endregion
}